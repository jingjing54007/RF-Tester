using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using TruePosition.Test.DataLayer;

namespace TruePosition.Test.QF
{
    public class TestErrorEventArgs : EventArgs
    {
        public string Name { get; private set; }
        public string Reason { get; private set; }
        public Step Step { get; private set; }

        private void Initialize(string name, string reason, Step step)
        {
            if (name == null)
                throw new ArgumentNullException("name");
            if (reason == null)
                throw new ArgumentNullException("reason");

            Name = name;
            Reason = reason;
            Step = step;
        }
        public TestErrorEventArgs(string name, string reason) : this(name, reason, null) { }
        public TestErrorEventArgs(string name, string reason, Step step)
        {
            Initialize(name, reason, step);
        }
    }
}
