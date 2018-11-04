using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Xml.Serialization;

namespace TruePosition.Test.IO
{
    internal sealed class PortFactory
    {
        public static IPort Create(PortType type, string name)
        {
            switch (type)
            {
                case PortType.Serial:
                    return new SerialPort(name);
                case PortType.Telnet:
                    return new TelnetPort(name);
                case PortType.Gpib:
                    return new GpibPort(name);
                case PortType.Snmp:
                    return new SnmpPort(name);
                default:
                    throw new ArgumentException("Invalid port type.", "type");
            }
        }
        private static IPort Deserialze(Type port, Stream stream)
        {
            XmlSerializer serializer = new XmlSerializer(port);
            IPort iPort = (IPort)serializer.Deserialize(stream);
            stream.Dispose();
            return iPort;
        }
        public static IPort Create(PortType type, Stream stream)
        {
            if (stream == null)
                throw new ArgumentNullException("stream", "A valid stream must be provided.");

            switch (type)
            {
                case PortType.Serial:
                    return Deserialze(typeof(SerialPort), stream);
                case PortType.Telnet:
                    return Deserialze(typeof(TelnetPort), stream);
                case PortType.Gpib:
                    return Deserialze(typeof(GpibPort), stream);
                case PortType.Snmp:
                    return Deserialze(typeof(SnmpPort), stream);
                default:
                    throw new ArgumentException("Invalid port type.", "type");
            }
        }
    }
}
