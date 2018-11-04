using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using System.IO;

using TruePosition.Test.DataLayer;

namespace TruePosition.Test.XML
{
    public static class Dehydrator
    {
        private static readonly XNamespace Namespace = "";

        public static XElement Dehydrate(DataLayer.Test test)
        {
            return DehydrateTest(test);
        }

        private static XElement DehydrateTest(DataLayer.Test test)
        {
            return new XElement("Test",
                                new XAttribute("Name", test.Name),
                                DehydrateSteps(test));
        }
        private static IEnumerable<XElement> DehydrateSteps(DataLayer.Test test)
        {
            return from step in test.Steps
                   select DehydrateStep(step);
        }
        private static XElement DehydrateStep(Step step)
        {
            return new XElement("Step",
                       new XElement("Actor",
                           new XAttribute("Name", step.Actor.Name),
                           new XElement("Type", step.Actor.Type.ToString()),
                           new XElement("SubType", step.Actor.SubType),
                           step.Actor.Command),
                       new XElement("Timeout", step.Timeout),
                       new XElement("Retries", step.Retries),
                       new XElement("ContinueOnError", step.ContinueOnError),
                       new XElement("CompleteOnTimeout", step.CompleteOnTimeout),
                       new XElement("Description", step.Description),
                       step.Response == null ? null : DehydrateResponse(step));
        }
        private static XElement DehydrateResponse(Step step)
        {
            return new XElement("Response",
                        new XAttribute("Delimiter", step.Response.Delimiter),
                        new XAttribute("Header", step.Response.Header),
                        new XAttribute("Trailer", step.Response.Trailer),
                        DeydrateElements(step.Response));
        }
        private static IEnumerable<XElement> DeydrateElements(Response response)
        {
            return from element in response.Elements
                   select DehydrateElement(element);
        }
        private static XElement DehydrateElement(ResponseElement element)
        {
            return new XElement("Element",
                        new XElement("KeyExpression", element.Key.RawExpression),
                        DehydrateExpected(element));
        }
        private static IEnumerable<XElement> DehydrateExpected(ResponseElement element)
        {
            return from expected in element.ExpectedResponses
                   select new XElement("Expected",
                            new XAttribute("Trim", expected.Trim),
                            expected.Key == null ? null : new XElement("KeyExpression", expected.Key.RawExpression),
                            DehydrateExpressions(expected),
                            new XElement("Destination",
                                new XElement("Name", expected.Destination == null ? null : expected.Destination.Name),
                                new XElement("Default", expected.Destination == null ? new object() : expected.Destination.Default)),
                            new XElement("FailureMessage", expected.FailureMessage));
        }
        private static IEnumerable<XElement> DehydrateExpressions(ExpectedResponse expected)
        {
            return (from ex in expected.Expressions
                    select new XElement("Expression", ex.Evaluator.RawExpression)).DefaultIfEmpty(new XElement("Expression", ""));
        }
    }
}
