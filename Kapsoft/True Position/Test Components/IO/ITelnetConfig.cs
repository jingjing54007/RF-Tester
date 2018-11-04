using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TruePosition.Test.IO
{
    public interface ITelnetConfig
    {
        string Name { get; }
        int ReceiveTimeout { get; set; }
        int TransmitRetries { get; set; }
        string HostName { get; set; }
        int Port { get; set; }
        string Username { get; set; }
        string Password { get; set; }
        string InputTrailer { get; set; }
        string OutputTrailer { get; set; }
    }
}
