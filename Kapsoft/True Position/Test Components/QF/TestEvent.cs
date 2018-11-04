using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using qf4net;
using TruePosition.Test.DataLayer;

namespace TruePosition.Test.QF
{
    public enum TestState
    {
        Active,
        Passed,
        Failed,
        Aborted
    }
    public class TestEvent : QEvent, IActor
    {
        public ActorType Type { get; private set; }
        public string Name { get; private set; }

        public string Reason { get; private set; }
        public Step Step { get; private set; }
        public TestState State { get; private set; }

        internal TestEvent(QFSignal signal, string name, string reason, Step step, TestState state)
            : base((int)signal)
        {
            if (string.IsNullOrEmpty(name))
                throw new ArgumentNullException("name", "A valid test name must be provided.");

            switch (signal)
            {
                case QFSignal.TestRun:
                case QFSignal.TestStep:
                case QFSignal.Error:
                case QFSignal.TestResult:
                case QFSignal.TestAbort:
                case QFSignal.Timeout:
                    Type = ActorType.Test;
                    Name = name;
                    Reason = reason;
                    Step = step;
                    State = state;
                    break;
                default:
                    throw new ArgumentException("Invalid Test signal.", "signal");
            }
        }
        internal TestEvent(QFSignal signal, string name) : this(signal, name, null, null, TestState.Active) { }
        internal TestEvent(QFSignal signal, string name, Step step) : this(signal, name, null, step, TestState.Active) { }
        internal TestEvent(QFSignal signal, string name, string reason) : this(signal, name, reason, null, TestState.Active) { }
        internal TestEvent(QFSignal signal, string name, string reason, Step step) : this(signal, name, reason, step, TestState.Active) { }
        internal TestEvent(QFSignal signal, string name, string reason, TestState state) : this(signal, name, reason, null, state) { }
        internal TestEvent(QFSignal signal, string name, TestState state) : this(signal, name, null, null, state) { }

        internal TestEvent(InternalSignal signal, string name) : this(signal, name, null, null) { }
        internal TestEvent(InternalSignal signal, TestEvent source) : this(signal, source.Name, source.Reason, source.Step) { }
        internal TestEvent(InternalSignal signal, string name, string reason, Step step)
            : base((int)signal)
        {
            switch (signal)
            {
                case InternalSignal.TestStep:
                case InternalSignal.TestPassed:
                case InternalSignal.TestAborted:
                case InternalSignal.TestFailed:
                    Name = name;
                    Reason = reason;
                    Step = step;
                    break;
                default:
                    throw new ArgumentException("Invalid internal test signal.", "signal");
            }
        }
    }
}
