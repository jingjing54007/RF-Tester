using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace TruePosition.Test.DataLayer
{
    public enum ActorType
    {
        Test,
        Port,
        Log,
        Recorder,
        Process,
        Prompt
    }

    public enum EntryStyle
    {
        Single,
        PerLine
    }

    public interface ITestClass
    {
        int Number { get; set; }
    }

    public class TestList<T> : List<T>
        where T : ITestClass
    {
        public TestList() : base() { }
        public TestList(IEnumerable<T> source)
        {
            foreach (T t in source)
            {
                this.Add(t);
            }
        }

        public new void Add(T value)
        {
            base.Add(value);
            value.Number = this.Count;
        }
    }
    public class TestDictionary<U, T> : Dictionary<U, T>
        where T : ITestClass
    {
        public new void Add(U key, T value)
        {
            base.Add(key, value);
            value.Number = this.Count;
        }
    }

    public class Test
    {
        public Test(string name)
        {
            Name = name;
        }

        public string Name { get; private set; }
        public TestList<Step> Steps { get; set; }
    }

    public class Step : ITestClass
    {
        public int Number { get; set; }
        public string Description { get; set; }
        public int Timeout { get; set; }
        public int Retries { get; set; }
        public bool ContinueOnError { get; set; }
        public bool CompleteOnTimeout { get; set; }
        public Actor Actor { get; set; }
        public Response Response { get; set; }
    }

    public interface IActor
    {
        ActorType Type { get; }
        string Name { get; }
    }
    public class Actor : IActor
    {
        public string Name { get; set; }
        public ActorType Type { get; set; }
        public string SubType { get; set; }
        public XElement Command { get; set; }
        public string Action { get; set; }
        public string EntryFormat { get; set; }
        public EntryStyle Style { get; set; }
    }

    public class ResponseProcessingArgs : EventArgs
    {
        public string RawValue { get; private set; }
        public ResponseProcessingArgs(string rawValue)
        {
            RawValue = rawValue;
        }
    }
    public class ResponseProcessedArgs : EventArgs
    {
        public string RawValue { get; private set; }
        public ResponseProcessedArgs(string rawValue)
        {
            RawValue = rawValue;
        }
    }

    public class ResponseElementProcessingArgs : EventArgs
    {
        public string RawElement { get; private set; }
        public ResponseElementProcessingArgs(string rawElement)
        {
            RawElement = rawElement;
        }
    }
    public class ResponseElementProcessedArgs : EventArgs
    {
        public string RawElement { get; private set; }
        public ResponseElementProcessedArgs(string rawElement)
        {
            RawElement = rawElement;
        }
    }

    public class ExpectedResponseProcessingArgs : EventArgs
    {
        public string RawElement { get; private set; }
        public ExpectedResponseProcessingArgs(string rawElement)
        {
            RawElement = rawElement;
        }
    }
    public class ExpectedResponseProcessedArgs : EventArgs
    {
        public string RawElement { get; private set; }
        public string SubKey { get; private set; }
        public ValueHelper Value { get; private set; }
        public ExpectedResponseProcessedArgs(string rawElement, string subKey, ValueHelper value)
        {
            RawElement = rawElement;
            SubKey = subKey;
            Value = value;
        }
    }

    public class Response
    {
        public Response(string delimiter)
        {
            Delimiter = delimiter;
        }

        public string Delimiter { get; set; }
        public string Header { get; set; }
        public string Trailer { get; set; }
        public string RawResponse { get; private set; }
        public TestList<ResponseElement> Elements { get; set; }

        public event EventHandler<ResponseProcessingArgs> Processing;
        public void OnProcessing(string rawValue)
        {
            RawResponse = rawValue;
            if (Processing != null)
                Processing(this, new ResponseProcessingArgs(rawValue));
        }
        public event EventHandler<ResponseProcessedArgs> Processed;
        public void OnProcessed(string rawValue)
        {
            if (Processed != null)
                Processed(this, new ResponseProcessedArgs(rawValue));
        }
    }

    public class ResponseElement : ITestClass
    {
        public ResponseElement(Evaluator key)
        {
            Key = key;
        }

        public int Number { get; set; }
        public Evaluator Key { get; set; }
        public string RawElement { get; set; }
        public TestList<ExpectedResponse> ExpectedResponses { get; set; }

        public event EventHandler<ResponseElementProcessingArgs> Processing;
        public void OnProcessing(string rawElement)
        {
            if (Processing != null)
                Processing(this, new ResponseElementProcessingArgs(rawElement));
        }
        public event EventHandler<ResponseElementProcessedArgs> Processed;
        public void OnProcessed(string rawElement)
        {
            if (Processed != null)
                Processed(this, new ResponseElementProcessedArgs(rawElement));
        }
    }

    public class ExpectedResponse : ITestClass
    {
        public int Number { get; set; }
        public Evaluator Key { get; set; }
        public bool Trim { get; set; }
        public ResponseDestination Destination { get; set; }
        public string FailureMessage { get; set; }
        public TestList<TestExpression> Expressions { get; set; }

        public ValueHelper Value { get; set; }

        public event EventHandler<ExpectedResponseProcessingArgs> Processing;
        public void OnProcessing(string rawElement)
        {
            if (Processing != null)
                Processing(this, new ExpectedResponseProcessingArgs(rawElement));
        }
        public event EventHandler<ExpectedResponseProcessedArgs> Processed;
        public void OnProcessed(string rawElement, string subKey, ValueHelper value)
        {
            if (Processed != null)
                Processed(this, new ExpectedResponseProcessedArgs(rawElement, subKey, value));
        }
    }
    public class TestExpression : ITestClass
    {
        public int Number { get; set; }
        public Evaluator Evaluator { get; set; }

        public TestExpression(string expression)
        {
            Evaluator = new Evaluator(expression);
        }
    }
    public class ResponseDestination
    {
        public string Name { get; set; }
        public ValueHelper Default { get; set; }
    }
}
