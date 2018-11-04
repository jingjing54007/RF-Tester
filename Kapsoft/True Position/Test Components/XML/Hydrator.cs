using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Linq;

using TruePosition.Test.DataLayer;
using TruePosition.Test.IO;
using TruePosition.Test.Recorder;

namespace TruePosition.Test.XML
{
    /// <summary>
    /// Loads a Test XML file and hydrates the Test Model with its contents.
    /// </summary>
    public static class Hydrator
    {
        private static readonly XNamespace Namespace = "";
        private static string Value(XAttribute attribute)
        {
            if (attribute == null)
                return null;
            else
                return attribute.Value;
        }
        private static string Value(XElement element)
        {
            if (element == null)
                return null;
            else
                return element.Value;
        }
        private static string Value(XElement element, string defaultValue)
        {
            if (element == null)
                return defaultValue;
            else
                return element.Value;
        }

        public static DataLayer.Test Hydrate(XElement root)
        {
            XElement test = root.DescendantsAndSelf(Namespace + "Test").SingleOrDefault();
            return HydrateTest(test);
        }
        public static DataLayer.Test Hydrate(string xmlPath, string filename)
        {
            return Hydrate(XElement.Load(Path.Combine(xmlPath, filename)));
        }
        public static IEnumerable<DataLayer.Test> Hydrate(string xmlPath)
        {
            List<DataLayer.Test> tests = new List<TruePosition.Test.DataLayer.Test>();

            foreach (string filePath in Directory.GetFiles(xmlPath, "*.xml", SearchOption.TopDirectoryOnly))
                tests.Add(Hydrate(Path.GetDirectoryName(filePath), Path.GetFileName(filePath)));

            return tests;
        }

        private static DataLayer.Test HydrateTest(XElement xmlTest)
        {
            DataLayer.Test test = new DataLayer.Test(xmlTest.Attribute("Name").Value);

            try
            {
                test.Steps = HydrateSteps(xmlTest.Descendants(Namespace + "Step"));
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("Error in XML. " + ex.Message + " Test " + test.Name + (test.Steps == null ? "" : ", step #" + (test.Steps.Count + 1)) + ".");
            }

            return test;
        }

        private static TestList<Step> HydrateSteps(IEnumerable<XElement> xmlSteps)
        {
            return (from step in xmlSteps
                    select HydrateStep(step)).ToTestList();
        }
        private static Step HydrateStep(XElement xmlStep)
        {
            Step step = new Step();

            step.Actor = HydrateActor(xmlStep.Element(Namespace + "Actor"));
            step.Timeout = Convert.ToInt32(Value(xmlStep.Element(Namespace + "Timeout"), "10"));
            step.Retries = Convert.ToInt32(Value(xmlStep.Element(Namespace + "Retries"), "1"));
            step.ContinueOnError = Convert.ToBoolean(Value(xmlStep.Element(Namespace + "ContinueOnError"), "false"));
            step.CompleteOnTimeout = Convert.ToBoolean(Value(xmlStep.Element(Namespace + "CompleteOnTimeout"), "false"));
            step.Description = Value(xmlStep.Element(Namespace + "Description"), string.Empty);

            if (xmlStep.Element(Namespace + "Response") != null)
            {
                step.Response = new Response(Value(xmlStep.Element(Namespace + "Response").Attribute(Namespace + "Delimiter")));
                step.Response.Header = Value(xmlStep.Element(Namespace + "Response").Attribute(Namespace + "Header"));
                step.Response.Trailer = Value(xmlStep.Element(Namespace + "Response").Attribute(Namespace + "Trailer"));
                step.Response.Elements = HydrateElements(xmlStep.Element(Namespace + "Response"));
            }

            return step;
        }

        private static string SetSubType(ActorType type, XElement xmlSubType)
        {
            string subType = xmlSubType.Value;
            switch (type)
            {
                case ActorType.Port:
                    PortType portType = (PortType)Enum.Parse(typeof(PortType), xmlSubType.Value);
                    break;
                case ActorType.Process:
                case ActorType.Prompt:
                    break;
                case ActorType.Recorder:
                    RecorderType recorderType = (RecorderType)Enum.Parse(typeof(RecorderType), xmlSubType.Value);
                    break;
                default:
                    throw new NotImplementedException();
            }

            return subType;
        }
        private static Actor HydrateActor(XElement xmlActor)
        {
            Actor actor = new Actor();

            actor.Name = xmlActor.Attribute(Namespace + "Name").Value;
            actor.Command = xmlActor.Element(Namespace + "Command");
            actor.Type = (ActorType)Enum.Parse(typeof(ActorType), xmlActor.Element(Namespace + "Type").Value);
            actor.SubType = SetSubType(actor.Type, xmlActor.Element(Namespace + "SubType"));

            return actor;
        }

        private static TestList<ResponseElement> HydrateElements(XElement response)
        {
            return (from e in response.Elements(Namespace + "Element")
                    select new ResponseElement(new Evaluator(ExpressionType.Simple, e.Element(Namespace + "KeyExpression").Value))
                    {
                        ExpectedResponses = HydrateExpected(e.Elements(Namespace + "Expected"))
                    }).ToTestList();
        }

        private static TestList<ExpectedResponse> HydrateExpected(IEnumerable<XElement> xmlExpected)
        {
            TestList<ExpectedResponse> expected = (from ex in xmlExpected
                                                   select new ExpectedResponse()
                                                   {
                                                       Key = ex.Element(Namespace + "KeyExpression") == null ? null : new Evaluator(ex.Element(Namespace + "KeyExpression").Value),
                                                       Trim = Convert.ToBoolean(Value(ex.Attribute(Namespace + "Trim"))),
                                                       Expressions = HydrateExpressions(ex),
                                                       Destination = ex.Element(Namespace + "Destination") == null ? null : new ResponseDestination
                                                       {
                                                           Name = ex.Element(Namespace + "Destination").Element(Namespace + "Name").Value,
                                                           Default = new ValueHelper(ex.Element(Namespace + "Destination").Element(Namespace + "Default").Value)
                                                       },
                                                       FailureMessage = ex.Element(Namespace + "FailureMessage") == null ? "" : ex.Element(Namespace + "FailureMessage").Value
                                                   }).ToTestList();

            if ((expected.Count > 1) && (expected.Any(er => (er.Key == null) && (er.Expressions.Count > 0))))
            {
                throw new InvalidOperationException("When a response element contains sub fields (i.e. multiple expected elements), every expected element must contain a KeyExpression.");
            }

            return expected;
        }

        private static TestList<TestExpression> HydrateExpressions(XElement expected)
        {
            return (from e in expected.Elements(Namespace + "Expression")
                    where !string.IsNullOrEmpty(e.Value)
                    select new TestExpression(e.Value)).ToTestList();
        }
    }
}
