using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Linq;

using TruePosition.Test.DataLayer;
using TruePosition.Test.IO;

namespace TruePosition.Test.XML
{
    /// <summary>
    /// Transform legacy XML exported from a test XLS file into well formed Test XML.
    /// </summary>
    public static class Transformer
    {
        private static readonly XNamespace m_Namespace = "urn:schemas-microsoft-com:office:spreadsheet";

        public static IEnumerable<XElement> Transform(string xmlFilePath)
        {
            XElement spreadSheet = XElement.Load(xmlFilePath);

            List<XElement> tests = new List<XElement>();
            foreach (XElement sheet in spreadSheet.Descendants(m_Namespace + "Worksheet"))
            {
                TransformWorksheet(tests, sheet);

                // TEST ONLY: Only process the bootup sheet for now...
                break;
            }

            return tests;
        }

        private static string Filename(XElement test)
        {
            return test.Attribute("Name").Value.Split(Path.GetInvalidFileNameChars(),
                                                      StringSplitOptions.RemoveEmptyEntries).Aggregate((agg, n) => agg + n) + ".xml";
        }
        private static void Save(string destinationPath, string filename, XElement test)
        {
            test.Save(Path.Combine(destinationPath, filename));
        }
        public static void Save(string destinationPath, XElement test)
        {
            if (!Directory.Exists(destinationPath))
                Directory.CreateDirectory(destinationPath);

            Save(destinationPath, Filename(test), test);
        }
        public static void Save(string destinationPath, IEnumerable<XElement> tests)
        {
            if (!Directory.Exists(destinationPath))
                Directory.CreateDirectory(destinationPath);

            foreach (XElement test in tests)
                Save(destinationPath, test);
        }

        private static void TransformWorksheet(List<XElement> tests, XElement sheet)
        {
            foreach (XElement test in from data in sheet.Descendants(m_Namespace + "Data")
                                      where data.Value.Contains("TestCase - ")
                                      select data.Ancestors(m_Namespace + "Row").First())
            {
                tests.Add(TransformTest(test));
            }
        }

        private static bool ContainsCommand(string[] valueNames, string[] fields)
        {
            bool found = false;
            int count = valueNames.Count();

            for (int i = 0; i < count; i++)
            {
                if ((valueNames[i] == "Command")  && (!string.IsNullOrEmpty(fields[i])))
                    found = true;
            }

            return found;
        }

        private static XElement TransformTest(XElement tcOriginal)
        {
            string[] names = tcOriginal.Value.Split(new string[] {" - "}, StringSplitOptions.RemoveEmptyEntries);
            XElement test = new XElement("Test",
                                  new XAttribute("Type", names[1].Trim().Replace(" ", "")),
                                  new XAttribute("Name", names.Count() < 3 ? "" : names[2].Trim().Replace(" ", "")));

            XNode header = (from r in tcOriginal.NodesAfterSelf()
                            where ((XElement)r).Descendants(m_Namespace + "Data").Any(d => d.Value == "Command")
                            select r).First();

            string[] valueNames = (from d in ((XElement)header).Descendants(m_Namespace + "Data")
                                   select d.Value).ToArray();

            XElement newStep = null;
            foreach (XElement step in header.ElementsAfterSelf().TakeWhile(r => !r.Descendants(m_Namespace + "Data").Any(d => d.Value.Contains("TestCase - "))))
            {            
                if (step.Descendants(m_Namespace + "Data").Count() > 0)
                {
                    string[] fields = (from c in step.Descendants(m_Namespace + "Cell")
                                       select c.Descendants(m_Namespace + "Data").Count() == 0 ? "" : c.Descendants(m_Namespace + "Data").Single().Value).Take(valueNames.Count()).ToArray();

                    if (ContainsCommand(valueNames, fields))
                    {
                        if (newStep != null)
                        {
                            test.Add(newStep);
                        }

                        newStep = new XElement("Step",
                                                test.Attribute("Type"),
                                                from i in valueNames.Aggregate(new { i = 0, indexes = new List<int>() },
                                                                               (agg, n) => { agg.indexes.Add(agg.i); return new { i = agg.i + 1, agg.indexes }; },
                                                                               agg => agg.indexes)
                                                let f = valueNames[i] == "Response" ?  
                                                            null : 
                                                            valueNames[i] == "Command" ? 
                                                                new XElement(AddActor(fields[i])) : 
                                                                new XElement(valueNames[i].Trim().Replace(" ", ""), (object)fields[i])
                                                select valueNames[i] == "Response" ? AddResponse(fields[i]) : f);
                        newStep.Add(new XElement("Description", string.Empty));
                        newStep.Add(new XElement("ContinueOnError", false));
                        newStep.Add(new XElement("CompleteOnTimeout", false));
                    }
                    else
                    {
                        newStep.Element("Response").Elements("Element").Last().AddAfterSelf((from i in valueNames.Aggregate(new { i = 0, indexes = new List<int>() },
                                                                                                                            (agg, n) => { agg.indexes.Add(agg.i); return new { i = agg.i + 1, agg.indexes }; },
                                                                                                                            agg => agg.indexes)
                                                                                             where valueNames[i] == "Response"
                                                                                             let r = new XElement("Element")
                                                                                             select AddResponseElement(r, fields[i])));
                    }
                }
            }

            if (newStep != null)
            {
                test.Add(newStep);
            }

            return test;
        }

        private static XElement AddActor(string action)
        {
            return new XElement("Actor",
                                new XAttribute("Name", ""),
                                new XElement("Type", ActorType.Port.ToString()),
                                new XElement("SubType", PortType.Serial.ToString()),
                                new XElement("Command",
                                    new XAttribute("Type", "LMU"),
                                        new XElement("Format", ""),
                                        new XElement("Parameter",
                                        new XAttribute("Prompt", "Command"),
                                        new XAttribute("Access", "Show"),
                                            new XElement("Value", action))));
        }
        private static XElement AddResponse(string key)
        {
            return new XElement("Response",
                                new XAttribute("Header", ""),
                                new XAttribute("Trailer", ">"),
                                new XAttribute("Delimiter", "\r\n"),
                                new XComment("A response may contain multiple elements in the event one UUT response message contains multiple response elements separated by the response delimiter. (i.e. the ?CO command)"),
                                AddResponseElement(new XElement("Element"), key));
        }

        private static string ProcessAdvancedWildcards(string key)
        {
            return key.ToCharArray().Aggregate((string)null, (agg, n) => agg == null ? new string(n, VBHelpers.AdvancedWildcards.Any(aw => aw == n) ? 2:1) : VBHelpers.AdvancedWildcards.Any(aw => aw == n) ? agg + n + n : agg + n);
        }

        private static XElement AddResponseElement(XElement element, string key)
        {
            element.Add(new XElement("KeyExpression", "value like '" + ProcessAdvancedWildcards(key) + "*'"),
                        new XComment("A response element may contain multiple expected responses in the event one response element contains multiple sub fields. (i.e. the temperature response)"),
                        new XComment("NOTE: If sub fields exist in a response element, each must have a unique key specified. The sub field value is expected to follow the key directly, delimited by one or more whitespace characters."),
                        new XElement("Expected",
                            new XAttribute("Trim", true),
                           new XComment("TODO: Add one or more expressions to test the response element value against."),
                           new XComment("1. Expressions are in the form of a simple expression, i.e. value <= 70, value like 'recvr:*', value len 24"),
                           new XComment("2. Expressions may only contain a single parameter operand named <value> evaluated against a constant integer, double, or string."),
                           new XComment("3. String constants must be enclosed in single quotes, i.e. 'rcvr:*'"),
                           new XComment("4. Supported operators: =, !=, <>, <=, >=, <, >, len, like"),
                           new XComment("5. The like operator supports both VB and T-SQL like operation wildcards."),
                           new XComment("6. Multiple expressions must all be true for a response element to pass evaluation."),
                           new XComment("[Optional] <KeyExpression>value like 'recvr*'</KeyExpression>"),
                           new XElement("Expression",
                               new XComment("TODO: Add at least one expression.")),
                           new XElement("Destination",
                               new XElement("Name"),
                               new XElement("Default")),
                           new XElement("FailureMessage")));
            return element;
        }
    }
}
