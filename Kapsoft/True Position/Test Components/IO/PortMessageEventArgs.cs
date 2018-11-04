using System;
using System.Collections.Generic;
using System.Text;

namespace TruePosition.Test.IO
{
    /// <summary>
    /// Event argument class for stream message reception and transmission events
    /// </summary>
    public class PortMessageEventArgs : System.EventArgs
    {
        public string Name { get; private set; }
        public string Message { get; private set; }
        public PortMessageEventArgs(string name, string message)
        {
            if (string.IsNullOrEmpty(name))
                throw new ArgumentNullException("name");
            if (string.IsNullOrEmpty(message))
                throw new ArgumentNullException("message");

            Name = name;
            Message = message;
        }
    }
}
