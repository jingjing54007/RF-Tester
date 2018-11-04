using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace TruePosition.Test.DataLayer
{
    public enum ProcessingEvent
    {
        ResponseProcessing,
        ElementProcessing,
        ExpectedResponseProcessing,
    }
    public enum ProcessedEvent
    {
        ResponseProcessed,
        ElementProcessed,
        ExpectedResponseProcessed
    }

    public interface IResponseEventAttribute
    {
        string TestName { get; }
        string StepAction { get; }
        int StepNumber { get; }
        int ElementNumber { get; }
        int ExpectedResponseNumber { get; }
    }

    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = false)]
    public sealed class ResponseProcessingAttribute : Attribute, IResponseEventAttribute
    {
        public ProcessingEvent Event { get; private set; }
        public string TestName { get; private set; }
        public string StepAction { get; private set; }
        public int StepNumber { get; private set; }
        public int ElementNumber { get; private set; }
        public int ExpectedResponseNumber { get; private set; }

        private void Initialize(ProcessingEvent processingevent, string testName, string stepAction, int stepNumber, int elementNumber, int expectedResponseNumber)
        {
            Event = processingevent;
            TestName = testName;
            StepAction = stepAction;
            StepNumber = stepNumber;
            ElementNumber = elementNumber;
            ExpectedResponseNumber = expectedResponseNumber;
        }

        /// <summary>
        /// Use this attribute to denote the method is to be attached to all instances of a given test class level. Either at the step response, response element, or expected response levels.
        /// </summary>
        /// <param name="processingEvent">event type to attach the delegate to</param>
        public ResponseProcessingAttribute(ProcessingEvent processingEvent)
        {
            Initialize(processingEvent, "", "", -1, -1, -1);
        }

        /// <summary>
        /// Use this attribute to attach the method to a specific step response by step number.
        /// </summary>
        /// <param name="testName">unique test name</param>
        /// <param name="stepNumber">step number within test</param>
        public ResponseProcessingAttribute(string testName, int stepNumber)
        {
            if (string.IsNullOrEmpty(testName))
                throw new ArgumentOutOfRangeException("testName", "Must not be null or empty.");
            if (stepNumber <= 0)
                throw new ArgumentOutOfRangeException("stepNumber", "Must be > 0.");

            Initialize(ProcessingEvent.ResponseProcessing, testName, "", stepNumber, -1, -1);
        }
        /// <summary>
        /// Use this attribute to attach the method to a specific step response by step command.
        /// </summary>
        /// <param name="testName">unique test name</param>
        /// <param name="stepCommand">step command</param>
        public ResponseProcessingAttribute(string testName, string stepCommand)
        {
            if (string.IsNullOrEmpty(testName))
                throw new ArgumentOutOfRangeException("testName", "Must not be null or empty.");
            if (string.IsNullOrEmpty(stepCommand))
                throw new ArgumentOutOfRangeException("stepCommand", "Must not be null or empty.");

            Initialize(ProcessingEvent.ResponseProcessing, testName, stepCommand, -1, -1, -1);
        }

        public ResponseProcessingAttribute(string testName, int stepNumber, int elementNumber)
        {
            if (string.IsNullOrEmpty(testName))
                throw new ArgumentOutOfRangeException("testName", "Must not be null or empty.");
            if (stepNumber <= 0)
                throw new ArgumentOutOfRangeException("stepNumber", "Must be > 0.");
            if (elementNumber <= 0)
                throw new ArgumentOutOfRangeException("elementNumber", "Must be > 0.");

            Initialize(ProcessingEvent.ElementProcessing, testName, "", stepNumber, elementNumber, -1);
        }
        public ResponseProcessingAttribute(string testName, string stepCommand, int elementNumber)
        {
            if (string.IsNullOrEmpty(testName))
                throw new ArgumentOutOfRangeException("testName", "Must not be null or empty.");
            if (string.IsNullOrEmpty(stepCommand))
                throw new ArgumentOutOfRangeException("stepCommand", "Must not be null or empty.");
            if (elementNumber <= 0)
                throw new ArgumentOutOfRangeException("elementNumber", "Must be > 0.");

            Initialize(ProcessingEvent.ElementProcessing, testName, stepCommand, -1, elementNumber, -1);
        }

        public ResponseProcessingAttribute(string testName, int stepNumber, int elementNumber, int expectedResponseNumber)
        {
            if (string.IsNullOrEmpty(testName))
                throw new ArgumentOutOfRangeException("testName", "Must not be null or empty.");
            if (stepNumber <= 0)
                throw new ArgumentOutOfRangeException("stepNumber", "Must be > 0.");
            if (elementNumber <= 0)
                throw new ArgumentOutOfRangeException("elementNumber", "Must be > 0.");
            if (expectedResponseNumber <= 0)
                throw new ArgumentOutOfRangeException("expectedResponseNumber", "Must be > 0.");

            Initialize(ProcessingEvent.ExpectedResponseProcessing, testName, "", stepNumber, elementNumber, expectedResponseNumber);
        }
        public ResponseProcessingAttribute(string testName, string stepCommand, int elementNumber, int expectedResponseNumber)
        {
            if (string.IsNullOrEmpty(testName))
                throw new ArgumentOutOfRangeException("testName", "Must not be null or empty.");
            if (string.IsNullOrEmpty(stepCommand))
                throw new ArgumentOutOfRangeException("stepCommand", "Must not be null or empty.");
            if (elementNumber <= 0)
                throw new ArgumentOutOfRangeException("elementNumber", "Must be > 0.");
            if (expectedResponseNumber <= 0)
                throw new ArgumentOutOfRangeException("expectedResponseNumber", "Must be > 0.");

            Initialize(ProcessingEvent.ExpectedResponseProcessing, testName, stepCommand, -1, elementNumber, expectedResponseNumber);
        }
    }
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = false)]
    public sealed class ResponseProcessedAttribute : Attribute, IResponseEventAttribute
    {
        public ProcessedEvent Event { get; private set; }
        public string TestName { get; private set; }
        public string StepAction { get; private set; }
        public int StepNumber { get; private set; }
        public int ElementNumber { get; private set; }
        public int ExpectedResponseNumber { get; private set; }

        private void Initialize(ProcessedEvent processedEvent, string testName, string stepCommand, int stepNumber, int elementNumber, int expectedResponseNumber)
        {
            Event = processedEvent;
            TestName = testName;
            StepAction = stepCommand;
            StepNumber = stepNumber;
            ElementNumber = elementNumber;
            ExpectedResponseNumber = expectedResponseNumber;
        }

        /// <summary>
        /// Use this attribute to denote the method is to be attached to all instances of a given test class level. Either at the step response, response element, or expected response levels.
        /// </summary>
        /// <param name="ProcessedEvent">event type to attach the delegate to</param>
        public ResponseProcessedAttribute(ProcessedEvent processedEvent)
        {
            Initialize(processedEvent, "", "", -1, -1, -1);
        }

        /// <summary>
        /// Use this attribute to attach the method to a specific step response by step number.
        /// </summary>
        /// <param name="testName">unique test name</param>
        /// <param name="stepNumber">step number within test</param>
        public ResponseProcessedAttribute(string testName, int stepNumber)
        {
            if (string.IsNullOrEmpty(testName))
                throw new ArgumentOutOfRangeException("testName", "Must not be null or empty.");
            if (stepNumber <= 0)
                throw new ArgumentOutOfRangeException("stepNumber", "Must be > 0.");

            Initialize(ProcessedEvent.ResponseProcessed, testName, "", stepNumber, -1, -1);
        }
        /// <summary>
        /// Use this attribute to attach the method to a specific step response by step command.
        /// </summary>
        /// <param name="testName">unique test name</param>
        /// <param name="stepCommand">step command</param>
        public ResponseProcessedAttribute(string testName, string stepCommand)
        {
            if (string.IsNullOrEmpty(testName))
                throw new ArgumentOutOfRangeException("testName", "Must not be null or empty.");
            if (string.IsNullOrEmpty(stepCommand))
                throw new ArgumentOutOfRangeException("stepCommand", "Must not be null or empty.");

            Initialize(ProcessedEvent.ResponseProcessed, testName, stepCommand, -1, -1, -1);
        }

        public ResponseProcessedAttribute(string testName, int stepNumber, int elementNumber)
        {
            if (string.IsNullOrEmpty(testName))
                throw new ArgumentOutOfRangeException("testName", "Must not be null or empty.");
            if (stepNumber <= 0)
                throw new ArgumentOutOfRangeException("stepNumber", "Must be > 0.");
            if (elementNumber <= 0)
                throw new ArgumentOutOfRangeException("elementNumber", "Must be > 0.");

            Initialize(ProcessedEvent.ElementProcessed, testName, "", stepNumber, elementNumber, -1);
        }
        public ResponseProcessedAttribute(string testName, string stepCommand, int elementNumber)
        {
            if (string.IsNullOrEmpty(testName))
                throw new ArgumentOutOfRangeException("testName", "Must not be null or empty.");
            if (string.IsNullOrEmpty(stepCommand))
                throw new ArgumentOutOfRangeException("stepCommand", "Must not be null or empty.");
            if (elementNumber <= 0)
                throw new ArgumentOutOfRangeException("elementNumber", "Must be > 0.");

            Initialize(ProcessedEvent.ElementProcessed, testName, stepCommand, -1, elementNumber, -1);
        }

        public ResponseProcessedAttribute(string testName, int stepNumber, int elementNumber, int expectedResponseNumber)
        {
            if (string.IsNullOrEmpty(testName))
                throw new ArgumentOutOfRangeException("testName", "Must not be null or empty.");
            if (stepNumber <= 0)
                throw new ArgumentOutOfRangeException("stepNumber", "Must be > 0.");
            if (elementNumber <= 0)
                throw new ArgumentOutOfRangeException("elementNumber", "Must be > 0.");
            if (expectedResponseNumber <= 0)
                throw new ArgumentOutOfRangeException("expectedResponseNumber", "Must be > 0.");

            Initialize(ProcessedEvent.ExpectedResponseProcessed, testName, "", stepNumber, elementNumber, expectedResponseNumber);
        }
        public ResponseProcessedAttribute(string testName, string stepCommand, int elementNumber, int expectedResponseNumber)
        {
            if (string.IsNullOrEmpty(testName))
                throw new ArgumentOutOfRangeException("testName", "Must not be null or empty.");
            if (string.IsNullOrEmpty(stepCommand))
                throw new ArgumentOutOfRangeException("stepCommand", "Must not be null or empty.");
            if (elementNumber <= 0)
                throw new ArgumentOutOfRangeException("elementNumber", "Must be > 0.");
            if (expectedResponseNumber <= 0)
                throw new ArgumentOutOfRangeException("expectedResponseNumber", "Must be > 0.");

            Initialize(ProcessedEvent.ExpectedResponseProcessed, testName, stepCommand, -1, elementNumber, expectedResponseNumber);
        }
    }

    public static class ResponseProcessor
    {
        private static string BuildExceptionDetail(string testName, int stepNumber, string stepCommand)
        {
            return BuildExceptionDetail(testName, stepNumber, stepCommand, -1, -1);
        }
        private static string BuildExceptionDetail(string testName, int stepNumber, string stepCommand, int elementNumber)
        {
            return BuildExceptionDetail(testName, stepNumber, stepCommand, elementNumber, -1);
        }
        private static string BuildExceptionDetail(string testName, int stepNumber, string stepCommand, int elementNumber, int expectedNumber)
        {
            return "<" + BuildDetail(testName, stepNumber, stepCommand, elementNumber, expectedNumber) + ">";
        }

        private static string BuildAttributeDetail(string testName, int stepNumber, string stepCommand)
        {
            return BuildAttributeDetail(testName, stepNumber, stepCommand, -1, -1);
        }
        private static string BuildAttributeDetail(string testName, int stepNumber, string stepCommand, int elementNumber)
        {
            return BuildAttributeDetail(testName, stepNumber, stepCommand, elementNumber, -1);
        }
        private static string BuildAttributeDetail(string testName, int stepNumber, string stepCommand, int elementNumber, int expectedNumber)
        {
            return "[" + BuildDetail(testName, stepNumber, stepCommand, elementNumber, expectedNumber) + "]";
        }

        private static string BuildDetail(string testName, int stepNumber, string stepCommand, int elementNumber, int expectedNumber)
        {
            return "Test='" + testName + "', Step=" + stepNumber + (string.IsNullOrEmpty(stepCommand) ? "" : ", Action='" + stepCommand + "'") +
                   (elementNumber == -1 ? "" : ", Element=" + elementNumber) +
                   (expectedNumber == -1 ? "" : ", Expected=" + expectedNumber);
        }

        public static void Process(string testName, Step step, string rawResponse)
        {
            if (step.Response == null)
                return;

            try
            {
                step.Response.OnProcessing(rawResponse);
            }
            catch (Exception ex)
            {
                throw (Exception)Activator.CreateInstance(ex.GetType(), ex.Message.TrimEnd('.') + ". " + BuildExceptionDetail(testName, step.Number, step.Actor.Action));
            }

            string[] elements = rawResponse.Split(new string[] { step.Response.Delimiter }, StringSplitOptions.RemoveEmptyEntries);
            foreach (ResponseElement element in step.Response.Elements)
            {
                if (!elements.Any(e => element.Key.Evaluate(new ValueHelper(e))))
                    throw new Exception("A response element was not found in the UUT response that matched Key=<" + element.Key.RawExpression + ">. " + BuildExceptionDetail(testName, step.Number, step.Actor.Action, element.Number));

                string rawElement = null;
                try
                {
                    rawElement = (from e in elements
                                  where element.Key.Evaluate(new ValueHelper(e))
                                  select e).SingleOrDefault();
                }
                catch (InvalidOperationException)
                {
                    throw new InvalidOperationException("A response element with the Key=<" + element.Key.RawExpression + ">" + " matched multiple elements in the UUT response. " + BuildExceptionDetail(testName, step.Number, step.Actor.Action, element.Number));
                }

                element.RawElement = rawElement;

                try
                {
                    element.OnProcessing(rawElement);
                }
                catch (Exception ex)
                {
                    throw (Exception)Activator.CreateInstance(ex.GetType(), ex.Message.TrimEnd('.') + ". " + BuildExceptionDetail(testName, step.Number, step.Actor.Action, element.Number));
                }

                // DESIGN NOTE:
                // Remove the elements of the key from the value
                string[] keyElements = element.Key.Constant.Split(VBHelpers.SimpleWildcards, StringSplitOptions.RemoveEmptyEntries); 
                string value = rawElement;
                foreach (string keyElement in keyElements)
                {
                    value = value.Replace(keyElement, "");
                }

                foreach (ExpectedResponse er in element.ExpectedResponses)
                {
                    try
                    {
                        er.OnProcessing(rawElement);
                    }
                    catch (Exception ex)
                    {
                        throw (Exception)Activator.CreateInstance(ex.GetType(), ex.Message.TrimEnd('.') + ". " + BuildExceptionDetail(testName, step.Number, step.Actor.Action, element.Number, er.Number));
                    }

                    string subKey = null;
                    string subValue = null;
                    if (er.Key != null)
                    {
                        if (! er.Key.Evaluate(new ValueHelper(value)))
                            throw new InvalidOperationException("An expected sub field was not found in the Response Element=<" + rawElement + "> that matched Key=<" + er.Key.RawExpression + ">. " + BuildExceptionDetail(testName, step.Number, step.Actor.Action, element.Number, er.Number));

                        subKey = er.Key.Constant.Split(VBHelpers.SimpleWildcards, StringSplitOptions.RemoveEmptyEntries)[0];

                        if (value.Split(new string[] { subKey }, StringSplitOptions.RemoveEmptyEntries).Count() > 2)
                            throw new InvalidOperationException("An expected response with the Key=<" + er.Key.RawExpression + ">" + " matched multiple sub fields in the Response Element=<" + rawElement + ">. " + BuildExceptionDetail(testName, step.Number, step.Actor.Action, element.Number, er.Number));

                        subValue = value.Substring(value.IndexOf(subKey));
                        subValue = subValue.Replace(subKey, "").Trim();
                        subValue = subValue.Substring(0, subValue.IndexOf(' ') == -1 ? subValue.Length: subValue.IndexOf(' '));
                    }
                    else
                    {
                        subValue = er.Trim ? value.Trim(): value;
                    }

                    er.Value = new ValueHelper(subValue);
                    foreach (TestExpression te in er.Expressions)
                    {
                        if (!te.Evaluator.Evaluate(er.Value))
                        {
                            throw new InvalidOperationException((string.IsNullOrEmpty(er.FailureMessage) ? "An invalid response was found." : er.FailureMessage).TrimEnd('.') + ". " + BuildExceptionDetail(testName, step.Number, step.Actor.Action, element.Number, er.Number));
                        }
                    }

                    try
                    {
                        er.OnProcessed(rawElement, subKey, er.Value);
                    }
                    catch (Exception ex)
                    {
                        throw (Exception)Activator.CreateInstance(ex.GetType(), ex.Message.TrimEnd('.') + ". " + BuildExceptionDetail(testName, step.Number, step.Actor.Action, element.Number, er.Number));
                    }
                }

                try
                {
                    element.OnProcessed(rawElement);
                }
                catch (Exception ex)
                {
                    throw (Exception)Activator.CreateInstance(ex.GetType(), ex.Message.TrimEnd('.') + ". " + BuildExceptionDetail(testName, step.Number, step.Actor.Action, element.Number));
                }
            }

            try
            {
                step.Response.OnProcessed(rawResponse);
            }
            catch (Exception ex)
            {
                throw (Exception)Activator.CreateInstance(ex.GetType(), ex.Message.TrimEnd('.') + ". " + BuildExceptionDetail(testName, step.Number, step.Actor.Action));
            }
        }

        public static void BindEvents(IEnumerable<Test> tests, Assembly assembly)
        {
            foreach (Test test in tests)
                BindEvents(test, assembly);
        }
        public static void BindEvents(Test test, Assembly assembly)
        {
            foreach (var d in from mi in assembly.GetTypes().SelectMany(t => t.GetMethods(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static))
                              from a in  mi.GetCustomAttributes(false)
                              where a is ResponseProcessingAttribute || a is ResponseProcessedAttribute
                              select new 
                                  {
                                      Info = mi,
                                      Attribute = (Attribute)a
                                  })
            {
                if (d.Attribute is ResponseProcessingAttribute)
                {
                    ResponseProcessingAttribute processing = (ResponseProcessingAttribute)d.Attribute;
                    switch (processing.Event)
                    {
                        case ProcessingEvent.ResponseProcessing:
                            if (string.IsNullOrEmpty(processing.TestName))
                            {
                                foreach (Step step in test.Steps)
                                {
                                    try
                                    {
                                        step.Response.Processing += (EventHandler<ResponseProcessingArgs>)EventHandler.CreateDelegate(typeof(EventHandler<ResponseProcessingArgs>),
                                                                                                                                      null,
                                                                                                                                      d.Info);
                                    }
                                    catch (Exception)
                                    {
                                        throw new ArgumentException("Could not bind method <" +
                                                                    d.Info.Name +
                                                                    "> to the Processing event of all response instances.");
                                    }
                                }
                            }
                            else
                            {
                                Step step = (from s in test.Steps
                                             where s.Number == processing.StepNumber || s.Actor.Action == processing.StepAction
                                             select s).SingleOrDefault();

                                if (step == null)
                                    throw new ArgumentException("Could not find a response instance that matches binding attribute " +
                                                                BuildAttributeDetail(processing.TestName,
                                                                                     processing.StepNumber,
                                                                                     processing.StepAction,
                                                                                     processing.ElementNumber,
                                                                                     processing.ExpectedResponseNumber) +
                                                                " on method <" + d.Info.Name + ">.");

                                try
                                {
                                    step.Response.Processing += (EventHandler<ResponseProcessingArgs>)EventHandler.CreateDelegate(typeof(EventHandler<ResponseProcessingArgs>),
                                                                                                                                  null,
                                                                                                                                  d.Info);
                                }
                                catch (Exception ex)
                                {
                                    throw new ArgumentException("Could not bind method <" +
                                                                d.Info.Name +
                                                                "> to the Processing event on response instance " +
                                                                BuildExceptionDetail(processing.TestName,
                                                                                     processing.StepNumber,
                                                                                     processing.StepAction,
                                                                                     processing.ElementNumber,
                                                                                     processing.ExpectedResponseNumber));
                                }
                            }
                            break;
                        case ProcessingEvent.ElementProcessing:
                            if (string.IsNullOrEmpty(processing.TestName))
                            {
                                foreach (ResponseElement element in test.Steps.Where(ts => ts.Response != null).SelectMany(ts => ts.Response.Elements))
                                {
                                    try
                                    {
                                        element.Processing += (EventHandler<ResponseElementProcessingArgs>)EventHandler.CreateDelegate(typeof(EventHandler<ResponseElementProcessingArgs>),
                                                                                                                                       null,
                                                                                                                                       d.Info);
                                    }
                                    catch (Exception ex)
                                    {
                                        throw new ArgumentException("Could not bind method <" +
                                                                    d.Info.Name +
                                                                    "> to the Processing event of all response element instances.");
                                    }
                                }
                            }
                            else
                            {
                                ResponseElement element = (from s in test.Steps
                                                           where s.Number == processing.StepNumber || s.Actor.Action == processing.StepAction
                                                           select s into step
                                                           from e in step.Response.Elements
                                                           where e.Number == processing.ElementNumber
                                                           select e).SingleOrDefault();

                                if (element == null)
                                    throw new ArgumentException("Could not find an response element instance that matches binding attribute " +
                                                                BuildAttributeDetail(processing.TestName,
                                                                                     processing.StepNumber,
                                                                                     processing.StepAction,
                                                                                     processing.ElementNumber,
                                                                                     processing.ExpectedResponseNumber) +
                                                                " on method <" + d.Info.Name + ">.");

                                try
                                {
                                    element.Processing += (EventHandler<ResponseElementProcessingArgs>)EventHandler.CreateDelegate(typeof(EventHandler<ResponseElementProcessingArgs>),
                                                                                                                                   null,
                                                                                                                                   d.Info);
                                }
                                catch (Exception ex)
                                {
                                    throw new ArgumentException("Could not bind method <" +
                                                                d.Info.Name +
                                                                "> to the Processing event on response element instance " +
                                                                BuildExceptionDetail(processing.TestName,
                                                                                     processing.StepNumber,
                                                                                     processing.StepAction,
                                                                                     processing.ElementNumber,
                                                                                     processing.ExpectedResponseNumber));
                                }
                            }
                            break;
                        case ProcessingEvent.ExpectedResponseProcessing:
                            if (string.IsNullOrEmpty(processing.TestName))
                            {
                                foreach (ExpectedResponse er in test.Steps.Where(ts => ts.Response != null).SelectMany(ts => ts.Response.Elements).SelectMany(re => re.ExpectedResponses))
                                {
                                    try
                                    {
                                        er.Processing += (EventHandler<ExpectedResponseProcessingArgs>)EventHandler.CreateDelegate(typeof(EventHandler<ExpectedResponseProcessingArgs>), null, d.Info);
                                    }
                                    catch (Exception ex)
                                    {
                                        throw new ArgumentException("Could not bind method <" +
                                                                    d.Info.Name +
                                                                    "> to the Processing event of all expected response instances.");
                                    }
                                }
                            }
                            else
                            {
                                ExpectedResponse expected = (from s in test.Steps
                                                             where (s.Response != null) && (s.Number == processing.StepNumber || s.Actor.Action == processing.StepAction)
                                                             select s into step
                                                             from e in step.Response.Elements
                                                             where e.Number == processing.ElementNumber
                                                             select e into element
                                                             from er in element.ExpectedResponses
                                                             where er.Number == processing.ExpectedResponseNumber
                                                             select er).SingleOrDefault();

                                if (expected == null)
                                    throw new ArgumentException("Could not find an expected response instance that matches binding attribute " +
                                                                BuildAttributeDetail(processing.TestName,
                                                                                     processing.StepNumber,
                                                                                     processing.StepAction,
                                                                                     processing.ElementNumber,
                                                                                     processing.ExpectedResponseNumber) +
                                                                " on method <" + d.Info.Name + ">.");

                                try
                                {
                                    expected.Processing += (EventHandler<ExpectedResponseProcessingArgs>)EventHandler.CreateDelegate(typeof(EventHandler<ExpectedResponseProcessingArgs>), null, d.Info);
                                }
                                catch (Exception ex)
                                {
                                    throw new ArgumentException("Could not bind method <" +
                                                                d.Info.Name +
                                                                "> to the Processing event on expected response instance " +
                                                                BuildExceptionDetail(processing.TestName,
                                                                                     processing.StepNumber,
                                                                                     processing.StepAction,
                                                                                     processing.ElementNumber,
                                                                                     processing.ExpectedResponseNumber));
                                }
                            }
                            break;
                    }
                }
                else if (d.Attribute is ResponseProcessedAttribute)
                {
                    ResponseProcessedAttribute processed = (ResponseProcessedAttribute)d.Attribute;
                    switch (processed.Event)
                    {
                        case ProcessedEvent.ResponseProcessed:
                            if (string.IsNullOrEmpty(processed.TestName))
                            {
                                foreach (Step step in test.Steps)
                                {
                                    try
                                    {
                                        step.Response.Processed += (EventHandler<ResponseProcessedArgs>)EventHandler.CreateDelegate(typeof(EventHandler<ResponseProcessedArgs>),
                                                                                                                                    null,
                                                                                                                                    d.Info);
                                    }
                                    catch (Exception ex)
                                    {
                                        throw new ArgumentException("Could not bind method <" +
                                                                    d.Info.Name +
                                                                    "> to the Processed event of all response instances.");
                                    }
                                }
                            }
                            else
                            {
                                Step step = (from s in test.Steps
                                             where s.Number == processed.StepNumber || s.Actor.Action == processed.StepAction
                                             select s).SingleOrDefault();

                                if (step == null)
                                    throw new ArgumentException("Could not find a response instance that matches binding attribute " +
                                                                BuildAttributeDetail(processed.TestName,
                                                                                     processed.StepNumber,
                                                                                     processed.StepAction,
                                                                                     processed.ElementNumber,
                                                                                     processed.ExpectedResponseNumber) +
                                                                " on method <" + d.Info.Name + ">.");

                                try
                                {
                                    step.Response.Processed += (EventHandler<ResponseProcessedArgs>)EventHandler.CreateDelegate(typeof(EventHandler<ResponseProcessedArgs>),
                                                                                                                                null,
                                                                                                                                d.Info);
                                }
                                catch (Exception ex)
                                {
                                    throw new ArgumentException("Could not bind method <" +
                                                                d.Info.Name +
                                                                "> to the Processed event on response instance " +
                                                                BuildExceptionDetail(processed.TestName,
                                                                                     processed.StepNumber,
                                                                                     processed.StepAction,
                                                                                     processed.ElementNumber,
                                                                                     processed.ExpectedResponseNumber));
                                }
                            }
                            break;
                        case ProcessedEvent.ElementProcessed:
                            if (string.IsNullOrEmpty(processed.TestName))
                            {
                                foreach (ResponseElement element in test.Steps.Where(ts => ts.Response != null).SelectMany(ts => ts.Response.Elements))
                                {
                                    try
                                    {
                                        element.Processed += (EventHandler<ResponseElementProcessedArgs>)EventHandler.CreateDelegate(typeof(EventHandler<ResponseElementProcessedArgs>),
                                                                                                                                     null,
                                                                                                                                     d.Info);
                                    }
                                    catch (Exception ex)
                                    {
                                        throw new ArgumentException("Could not bind method <" +
                                                                    d.Info.Name +
                                                                    "> to the Processed event of all response element instances.");
                                    }
                                }
                            }
                            else
                            {
                                ResponseElement element = (from s in test.Steps
                                                           where (s.Response != null) && (s.Number == processed.StepNumber || s.Actor.Action == processed.StepAction)
                                                           select s into step
                                                           from e in step.Response.Elements
                                                           where e.Number == processed.ElementNumber
                                                           select e).SingleOrDefault();

                                if (element == null)
                                    throw new ArgumentException("Could not find an response element instance that matches binding attribute " +
                                                                BuildAttributeDetail(processed.TestName,
                                                                                     processed.StepNumber,
                                                                                     processed.StepAction,
                                                                                     processed.ElementNumber,
                                                                                     processed.ExpectedResponseNumber) +
                                                                " on method <" + d.Info.Name + ">.");

                                try
                                {
                                    element.Processed += (EventHandler<ResponseElementProcessedArgs>)EventHandler.CreateDelegate(typeof(EventHandler<ResponseElementProcessedArgs>),
                                                                                                                                 null,
                                                                                                                                 d.Info);
                                }
                                catch (Exception ex)
                                {
                                    throw new ArgumentException("Could not bind method <" +
                                                                d.Info.Name +
                                                                "> to the Processed event on response element instance " +
                                                                BuildExceptionDetail(processed.TestName,
                                                                                     processed.StepNumber,
                                                                                     processed.StepAction,
                                                                                     processed.ElementNumber,
                                                                                     processed.ExpectedResponseNumber));
                                }
                            }
                            break;
                        case ProcessedEvent.ExpectedResponseProcessed:
                            if (string.IsNullOrEmpty(processed.TestName))
                            {
                                foreach (ExpectedResponse er in test.Steps.Where(ts => ts.Response != null).SelectMany(ts => ts.Response.Elements).SelectMany(re => re.ExpectedResponses))
                                {
                                    try
                                    {
                                        er.Processed += (EventHandler<ExpectedResponseProcessedArgs>)EventHandler.CreateDelegate(typeof(EventHandler<ExpectedResponseProcessedArgs>), null, d.Info);
                                    }
                                    catch (Exception ex)
                                    {
                                        throw new ArgumentException("Could not bind method <" +
                                                                    d.Info.Name +
                                                                    "> to the processed event of all expected response instances.");
                                    }
                                }
                            }
                            else
                            {
                                ExpectedResponse expected = (from s in test.Steps
                                                             where (s.Response != null) && (s.Number == processed.StepNumber || s.Actor.Action == processed.StepAction)
                                                             select s into step
                                                             from e in step.Response.Elements
                                                             where e.Number == processed.ElementNumber
                                                             select e into element
                                                             from er in element.ExpectedResponses
                                                             where er.Number == processed.ExpectedResponseNumber
                                                             select er).SingleOrDefault();

                                if (expected == null)
                                    throw new ArgumentException("Could not find an expected response instance that matches binding attribute " +
                                                                BuildAttributeDetail(processed.TestName,
                                                                                     processed.StepNumber,
                                                                                     processed.StepAction,
                                                                                     processed.ElementNumber,
                                                                                     processed.ExpectedResponseNumber) +
                                                                " on method <" + d.Info.Name + ">.");

                                try
                                {
                                    expected.Processed += (EventHandler<ExpectedResponseProcessedArgs>)EventHandler.CreateDelegate(typeof(EventHandler<ExpectedResponseProcessedArgs>), null, d.Info);
                                }
                                catch (Exception ex)
                                {
                                    throw new ArgumentException("Could not bind method <" +
                                                                d.Info.Name +
                                                                "> to the processed event on expected response instance " +
                                                                BuildExceptionDetail(processed.TestName,
                                                                                     processed.StepNumber,
                                                                                     processed.StepAction,
                                                                                     processed.ElementNumber,
                                                                                     processed.ExpectedResponseNumber));
                                }
                            }
                            break;
                    }
                }
            }
        }
    }
}