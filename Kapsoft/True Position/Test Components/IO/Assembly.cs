using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TruePosition.Test.IO
{
    /// <summary>
    /// Hardware type that a Port represents.
    /// </summary>
    public enum PortType
    {
        /// <summary>
        /// Traditional RS-232 serial port
        /// </summary>
        Serial,
        /// <summary>
        /// Simple Telnet socket based port
        /// </summary>
        Telnet,
        /// <summary>
        /// Simple National Instruments GPIB based port
        /// </summary>
        Gpib,
        /// <summary>
        /// Simple SNMP based port
        /// </summary>
        Snmp
    }

    /// <summary>
    /// Communication mode of a Serial Port, either solicited or unsolicited.
    /// </summary>
    public enum SerialPortMode
    {
        /// <summary>
        /// Upon a message send, a receive event or timeout error must occur before the next message can be sent.
        /// </summary>
        Solicited,
        /// <summary>
        /// Messages can be received at any time, unassociated with sent messages.
        /// </summary>
        Unsolicited
    }

    /// <summary>
    /// Connection state of a Port
    /// </summary>
    public enum PortState
    {
        /// <summary>
        /// Port has been instantiated and is configurable, but not yet open or ready for use.
        /// </summary>
        Created,
        /// <summary>
        /// Port is being transitioned from the Created state to the Opened state. 
        /// </summary>
        Opening,
        /// <summary>
        /// Port is now open and ready to be used. 
        /// </summary>
        Opened,
        /// <summary>
        /// Port is transitioning to the Closed state.
        /// </summary>
        Closing,
        /// <summary>
        /// Port has been closed and is no longer usable.
        /// </summary>
        Closed,
        /// <summary>
        /// Port has encountered an error or fault from which it cannot recover and from which it is no longer usable.
        /// </summary>
        Faulted
    }

    public enum TelnetVerbs
    {
        WILL = 251,
        WONT = 252,
        DO = 253,
        DONT = 254,
        IAC = 255
    }
    public enum TelnetOptions
    {
        SGA = 3
    }
}
