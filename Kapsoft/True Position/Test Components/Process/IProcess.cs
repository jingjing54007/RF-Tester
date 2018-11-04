using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace TruePosition.Test.Process
{
    public interface IProcess : ISerializable, IDisposable
    {
        string Name { get; }
        int LaunchTimeout { get; set; }

        void Launch(string commandLine);
        void Kill();

        event EventHandler<ProcessEventArgs> Launched;
        event EventHandler<ProcessEventArgs> Killed;
        event EventHandler<ProcessErrorEventArgs> Error;
        event EventHandler<ProcessEventArgs> Timeout;
    }
}
