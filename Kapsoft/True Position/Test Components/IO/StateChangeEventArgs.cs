using System;
using System.Collections.Generic;
using System.Text;

namespace TruePosition.Test.IO
{
    /// <summary>
    /// Event argument class for stream status change events
    /// </summary>
    public class StateChangeEventArgs : System.EventArgs
    {
        public StateChangeEventArgs()
        {
            State = PortState.Closed;
        }

        public StateChangeEventArgs(PortState state)
        {
            if ((state < PortState.Closed) || (state > PortState.Closing))
            {
                throw new ArgumentOutOfRangeException("state");
            }
            else
            {
                State = state;
            }
        }

        public PortState State { get; private set; }
    }
}
