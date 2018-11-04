using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using qf4net;
using TruePosition.Test.DataLayer;

namespace TruePosition.Test.QF
{
    public class ProcessEvent : QEvent, IActor, IError
    {
        public ActorType Type { get; private set; }
        public string Name { get; private set; }

        public int Timeout { get; private set; }
        public string CommandLine { get; private set; }
        public string Error { get; private set; }
        public string AdditionalDescription { get; private set; }

        private ProcessEvent(QFSignal signal, string name, string commandLine, int timeout, string error, string additionalDescription) 
            : base((int)signal) 
        {
            if (string.IsNullOrEmpty(name))
                throw new ArgumentNullException("name", "A valid process name must be provided.");

            switch (signal)
            {
                case QFSignal.ProcessLaunch:
                case QFSignal.ProcessLaunched:
                case QFSignal.ProcessKill:
                case QFSignal.ProcessKilled:
                case QFSignal.Timeout:
                case QFSignal.Error:
                    Type = ActorType.Process;
                    Name = name;
                    CommandLine = commandLine;
                    Timeout = timeout;
                    Error = error;
                    AdditionalDescription = additionalDescription;
                    break;
                default:
                    throw new ArgumentException("Invalid process signal.", "signal");
            }
        }
        internal ProcessEvent(QFSignal signal, string name) : this(signal, name, null, -1, null, null) { }
        internal ProcessEvent(QFSignal signal, string name, string commandLine) : this(signal, name, commandLine, -1, null, null) { }
        internal ProcessEvent(QFSignal signal, string name, int timeout) : this(signal, name, null, timeout, null, null) { }
        internal ProcessEvent(QFSignal signal, string name, string error, string additionalDescription) : this(signal, name, null, -1, error, additionalDescription) { }

        internal ProcessEvent(InternalSignal signal, ProcessEvent source) 
            : base((int)signal)
        {
            switch (signal)
            {
                case InternalSignal.ProcessKilling:
                case InternalSignal.ProcessLaunching:
                    Name = source.Name;
                    CommandLine = source.CommandLine;
                    Timeout = source.Timeout;
                    Error = source.Error;
                    AdditionalDescription = source.AdditionalDescription;
                    break;
                default:
                    throw new ArgumentException("Invalid internal process signal.", "signal");
            }
        }
    }
}
