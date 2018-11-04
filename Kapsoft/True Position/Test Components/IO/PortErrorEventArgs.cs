using System;
using System.Collections.Generic;
using System.Text;

namespace TruePosition.Test.IO
{
    /// <summary>
    /// Event argument class for asynchronous communication error events
    /// </summary>
    public class PortErrorEventArgs : System.EventArgs
    {
        public string Name { get; private set; }
        public string Error { get; private set; }
        public string AdditionalDescription { get; private set; }

        public PortErrorEventArgs(string name, string error) : this(name, error, string.Empty) { }
        public PortErrorEventArgs(string name, string error, string additionalDescription)
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
