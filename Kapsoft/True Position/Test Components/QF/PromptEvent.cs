using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using qf4net;
using TruePosition.Test.DataLayer;

namespace TruePosition.Test.QF
{
    public class PromptEvent : QEvent, IActor, IError
    {
        public ActorType Type { get; private set; }
        public string Name { get; private set; }

        public int Timeout { get; private set; }
        public string Text { get; private set; }
        public string Error { get; private set; }
        public string AdditionalDescription { get; private set; }

        private PromptEvent(QFSignal signal, string name, string text, int timeout, string error, string additionalDescription) 
            : base((int)signal) 
        {
            if (string.IsNullOrEmpty(name))
                throw new ArgumentNullException("name", "A valid prompt name must be provided.");

            switch (signal)
            {
                case QFSignal.PromptShow:
                case QFSignal.PromptShown:
                case QFSignal.PromptContinue:
                case QFSignal.PromptClosed:
                case QFSignal.Error:
                    Type = ActorType.Prompt;
                    Name = name;
                    Text = text;
                    Timeout = timeout;
                    Error = error;
                    AdditionalDescription = additionalDescription;
                    break;
                default:
                    throw new ArgumentException("Invalid prompt signal.", "signal");
            }
        }
        internal PromptEvent(QFSignal signal, string name) : this(signal, name, null, -1, null, null) { }
        internal PromptEvent(QFSignal signal, string name, string text) : this(signal, name, text, -1, null, null) { }
        internal PromptEvent(QFSignal signal, string name, string text, int timeout) : this(signal, name, text, timeout, null, null) { }
        internal PromptEvent(QFSignal signal, string name, string error, string additionalDescription) : this(signal, name, null, -1, error, additionalDescription) { }

        internal PromptEvent(InternalSignal signal, PromptEvent source) 
            : base((int)signal)
        {
            switch (signal)
            {
                case InternalSignal.PromptShowing:
                case InternalSignal.PromptClosing:
                    Name = source.Name;
                    Text = source.Text;
                    Timeout = source.Timeout;
                    Error = source.Error;
                    AdditionalDescription = source.AdditionalDescription;
                    break;
                default:
                    throw new ArgumentException("Invalid internal prompt signal.", "signal");
            }
        }
    }
}
