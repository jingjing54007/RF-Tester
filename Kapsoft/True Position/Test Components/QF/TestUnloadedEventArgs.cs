using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TruePosition.Test.QF
{
    public class TestUnloadedEventArgs : EventArgs
    {
        public string Name { get; private set; }

        private void Initialize(string name)
        {
            if (name == null)
                throw new ArgumentNullException("name");

            Name = name;
        }
        public TestUnloadedEventArgs(string name)
        {
            Initialize(name);
        }
    }
}
