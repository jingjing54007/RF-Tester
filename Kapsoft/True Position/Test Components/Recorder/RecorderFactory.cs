using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Xml.Serialization;

namespace TruePosition.Test.Recorder
{
    internal sealed class RecorderFactory
    {
        public static IRecorder Create(RecorderType type, string name)
        {
            switch (type)
            {
                case RecorderType.File:
                    return new FileRecorder(name);
                default:
                    throw new ArgumentException("Invalid recorder type.", "type");
            }
        }
    }
}
