using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TruePosition.Test.IO
{
    public interface ISerialConfig
    {
        int ReceiveBufferSize { get; set; }
        int TransmitBufferSize { get; set; }
        System.IO.Ports.Handshake Handshake { get; set; }
        System.IO.Ports.Parity Parity { get; set; }
        System.IO.Ports.StopBits StopBits { get; set; }
        string PortName { get; set; }
        int BaudRate { get; set; }
        int DataBits { get; set; }
        bool DiscardNull { get; set; }
        string InputHeader { get; set; }
        string OutputHeader { get; set; }
        string InputTrailer { get; set; }
        bool InputEveryCharacter { get; set; }
        string OutputTrailer { get; set; }
        SerialPortMode Mode { get; set; }
        int ReceiveTimeout { get; set; }
        int TransmitRetries { get; set; }
    }
}
