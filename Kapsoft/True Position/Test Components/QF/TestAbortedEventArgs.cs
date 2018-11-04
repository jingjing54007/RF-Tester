using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using TruePosition.Test.DataLayer;

namespace TruePosition.Test.QF
{
    public class TestAbortedEventArgs : EventArgs
    {
        public string Name { get; private set; }
        public string Reason { get; private set; }

        private void Initialize(string name, string reason)
        {
            if (name == null)
                throw new ArgumentNullException("name");
            if (reason == null)
                throw new ArgumentNullException("message");

            Name = name;
            Reason = reason;
        }

        public TestAbortedEventArgs(string name, string reason)
        {
            Initialize(name, reason);
        }
    }
}
