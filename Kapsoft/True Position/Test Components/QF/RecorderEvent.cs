using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using qf4net;
using TruePosition.Test.DataLayer;
using System.Xml.Linq;

namespace TruePosition.Test.QF
{
    public class RecorderEvent : QEvent, IActor, IError
    {
        public ActorType Type { get; private set; }
        public string Name { get; private set; }

        public string TestName { get; private set; }
        public int StepNumber { get; private set; }

        public string Location { get; private set; }
        public string Entry { get; private set; }
        public string EntryFormat { get; private set; }
        public EntryStyle EntryStyle { get; private set; }
        public IEnumerable<XElement> Values { get; private set; }
        public IEnumerable<string> CommandFilter { get; private set; }
        public IEnumerable<string> ResponseFilter { get; private set; }
        public string Error { get; private set; }
        public string AdditionalDescription { get; private set; }

        private RecorderEvent(QFSignal signal) 
            : base((int)signal) 
        {
            switch (signal)
            {
                case QFSignal.RecorderOpen:
                case QFSignal.RecorderOpenPending:
                case QFSignal.RecorderOpened:
                case QFSignal.RecorderRecord:
                case QFSignal.RecorderClose:
                case QFSignal.RecorderClosed:
                case QFSignal.Error:
                    break;
                default:
                    throw new ArgumentException("Invalid Recorder signal.", "signal");
            }

            Type = ActorType.Recorder;
            Name = string.Empty;
            TestName = string.Empty;
            StepNumber = 0;
            Location = string.Empty;
            EntryFormat = string.Empty;
            EntryStyle = EntryStyle.Single;
            Values = null;
            CommandFilter = null;
            ResponseFilter = null;
            Entry = string.Empty;
            Error = string.Empty;
            AdditionalDescription = string.Empty;
        }
        internal RecorderEvent(QFSignal signal, string name, string location, string entryFormat, EntryStyle style, IEnumerable<XElement> values, IEnumerable<string> commandFilter, IEnumerable<string> responseFilter)
            : this(signal) 
        {
            if ((string.IsNullOrEmpty(name)))
                throw new ArgumentNullException("name", "A valid Recorder name must be provided.");

            Name = name;
            Location = location;
            EntryFormat = entryFormat;
            EntryStyle = style;
            Values = values;
            CommandFilter = commandFilter;
            ResponseFilter = responseFilter;
        }
        internal RecorderEvent(QFSignal signal, string testName, int stepNumber, string entry) 
            : this(signal)
        {
            TestName = testName;
            StepNumber = stepNumber;
            Entry = entry;
        }
        internal RecorderEvent(QFSignal signal, string name, string empty)
            : this(signal)
        {
            if ((string.IsNullOrEmpty(name)))
                throw new ArgumentNullException("name", "A valid Recorder name must be provided.");

            Name = name;
        }
        internal RecorderEvent(QFSignal signal, string name, string error, string additionalDescription) 
            : this(signal) 
        {
            if ((string.IsNullOrEmpty(name)))
                throw new ArgumentNullException("name", "A valid Recorder name must be provided.");

            Name = name;
            Error = error;
            AdditionalDescription = additionalDescription;
        }

        internal RecorderEvent(InternalSignal signal, RecorderEvent source) 
            : base((int)signal)
        {
            switch (signal)
            {
                case InternalSignal.RecorderOpening:
                case InternalSignal.RecorderClosing:
                    Name = source.Name;
                    Location = source.Location;
                    EntryFormat = source.EntryFormat;
                    EntryStyle = source.EntryStyle;
                    Values = source.Values;
                    CommandFilter = source.CommandFilter;
                    ResponseFilter = source.ResponseFilter;
                    Entry = source.Entry;
                    Error = source.Error;
                    AdditionalDescription = source.AdditionalDescription;
                    break;
                default:
                    throw new ArgumentException("Invalid internal recorder signal.", "signal");
            }
        }
    }
}
