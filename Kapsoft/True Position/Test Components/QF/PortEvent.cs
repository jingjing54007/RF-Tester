using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using qf4net;
using TruePosition.Test.DataLayer;
using TruePosition.Test.IO;

namespace TruePosition.Test.QF
{
    public class PortEvent : QEvent, IActor, IError
    {
        public ActorType Type { get; private set; }
        public string Name { get; private set; }

        public string Message { get; private set; }
        public int Timeout { get; private set; }
        public int Retries { get; private set; }
        public string Error { get; private set; }
        public string AdditionalDescription { get; private set; }

        private PortEvent(QFSignal signal, string name, string message, int timeout, int retries, string error, string additionalDescription) 
            : base((int)signal) 
        {
            if (string.IsNullOrEmpty(name))
                throw new ArgumentNullException("name", "A valid port name must be provided.");

            switch (signal)
            {
                case QFSignal.PortOpen:
                case QFSignal.PortClose:
                case QFSignal.PortTransmit:
                case QFSignal.PortTransmitted:
                case QFSignal.PortResponse:
                case QFSignal.Timeout:
                case QFSignal.Error:
                    Type = ActorType.Port;
                    Name = name;
                    Message = message;
                    Timeout = timeout;
                    Retries = retries;
                    Error = error;
                    AdditionalDescription = additionalDescription;
                    break;
                default:
                    throw new ArgumentException("Invalid port signal.", "signal");
            }
        }
        internal PortEvent(QFSignal signal, string name) : this(signal, name, null, -1, -1, null, null) { }
        internal PortEvent(QFSignal signal, string name, string message) : this(signal, name, message, -1, -1, null, null) { }
        internal PortEvent(QFSignal signal, string name, string message, int timeout, int retries) : this(signal, name, message, timeout, retries, null, null) { }
        internal PortEvent(QFSignal signal, string name, string error, string additionalDescription) : this(signal, name, null, -1, -1, error, additionalDescription) { }

        internal PortEvent(InternalSignal signal, PortEvent source) 
            : base((int)signal)
        {
            switch (signal)
            {
                case InternalSignal.PortOpening:
                case InternalSignal.PortClosing:
                    Name = source.Name;
                    Message = source.Message;
                    Timeout = source.Timeout;
                    Retries = source.Retries;
                    Error = source.Error;
                    AdditionalDescription = source.AdditionalDescription;
                    break;
                default:
                    throw new ArgumentException("Invalid internal port signal.", "signal");
            }
        }
    }
}
