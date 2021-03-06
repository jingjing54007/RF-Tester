using System;
using System.Collections.Generic;
using System.Text;

namespace TruePosition.Test.Process
{
    /// <summary>
    /// Event argument class for Process error events
    /// </summary>
    public class ProcessErrorEventArgs : System.EventArgs
    {
        public string Name { get; private set; }
        public string Error { get; private set; }
        public string AdditionalDescription { get; private set; }

        public ProcessErrorEventArgs(string name, string error) : this(name, error, string.Empty) { }
        public ProcessErrorEventArgs(string name, string error, string additionalDescription)
        {
            Initialize(name, error, additionalDescription);
        }

        private void Initialize(string name, string error, string additionalDescription)
        {
            if (string.IsNullOrEmpty(name))
                throw new ArgumentNullException("name");
            if (string.IsNullOrEmpty(error))
                throw new ArgumentNullException("error");

            Name = name;
            Error = error;
            AdditionalDescription = additionalDescription;
        }
    }
}
