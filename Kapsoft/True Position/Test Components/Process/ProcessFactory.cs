using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Xml.Serialization;

namespace TruePosition.Test.Process
{
    internal sealed class ProcessFactory
    {
        public static IProcess Create(string name)
        {
            return new WindowsProcess(name);
        }
        public static IProcess Create(Stream stream)
        {
            if (stream == null)
                throw new ArgumentNullException("stream", "A valid stream must be provided.");

            XmlSerializer serializer = new XmlSerializer(typeof(WindowsProcess));
            IProcess process = (IProcess)serializer.Deserialize(stream);
            stream.Dispose();
            return process;
        }
    }
}
