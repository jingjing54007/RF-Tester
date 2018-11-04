using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace TruePosition.Test.IO
{
    public interface IPort : ISerializable, IDisposable
    {
        PortState State { get; }
        string Name { get; }
        int ReceiveTimeout { get; set; }
        int TransmitRetries { get; set; }

        void Open();
        void Close();

        bool Transmit(string message);
        bool Transmit(string message, bool pack);

        event EventHandler<PortMessageEventArgs> DataReceived;
        event EventHandler<PortMessageEventArgs> DataTransmitted;
        event EventHandler<PortErrorEventArgs> Error;
        event EventHandler<StateChangeEventArgs> StateChanged;
        event EventHandler<PortTimeoutExpiredEventArgs> ReceiveTimeoutExpired;
    }
}
