using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using TruePosition.Test.DataLayer;

namespace TruePosition.Test.QF
{
    public class TestSteppedEventArgs : EventArgs
    {
        public string Name { get; private set; }
        public Step Step { get; private set; }

        public TestSteppedEventArgs(string name, Step step)
        {
            if (name == null)
                throw new ArgumentNullException("name");
            if (step == null)
                throw new ArgumentNullException("step");

            Name = name;
            Step = step;
        }
    }
}
