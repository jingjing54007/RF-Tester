using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Xml.Serialization;

namespace TruePosition.Test.Prompt
{
    internal sealed class PromptFactory
    {
        public static IPrompt Create(string name)
        {
            return new UserPrompt(name);
        }
        public static IPrompt Create(Stream stream)
        {
            if (stream == null)
                throw new ArgumentNullException("stream", "A valid stream must be provided.");

            XmlSerializer serializer = new XmlSerializer(typeof(UserPrompt));
            IPrompt prompt = (IPrompt)serializer.Deserialize(stream);
            stream.Dispose();
            return prompt;
        }
    }
}
