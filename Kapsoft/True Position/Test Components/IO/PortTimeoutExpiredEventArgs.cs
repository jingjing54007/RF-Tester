using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TruePosition.Test.IO
{
    /// <summary>
    /// Event argument class for stream message reception and transmission events
    /// </summary>
    public class PortTimeoutExpiredEventArgs : System.EventArgs
    {
        public string Name { get; private set; }
        public PortTimeoutExpiredEventArgs(string name) 
        {
            if (string.IsNullOrEmpty(name))
                throw new ArgumentNullException("name");

            Name = name;
        }
    }
}
