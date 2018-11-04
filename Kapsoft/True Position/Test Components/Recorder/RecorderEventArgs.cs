using System;
using System.Collections.Generic;
using System.Text;

namespace TruePosition.Test.Recorder
{
    /// <summary>
    /// Event argument class for general Process events
    /// </summary>
    public class RecorderEventArgs : System.EventArgs
    {
        public string Name { get; private set; }
        public string Path { get; private set; }
        public string FileName { get; private set; }

        public RecorderEventArgs(string name, string path, string fileName)
        {
            Initialize(name, path, fileName);
        }

        private void Initialize(string name, string path, string fileName)
        {
            if (string.IsNullOrEmpty(name))
                throw new ArgumentNullException("name");
            if (string.IsNullOrEmpty(path))
                throw new ArgumentNullException("path");
            if (string.IsNullOrEmpty(fileName))
                throw new ArgumentNullException("fileName");

            Name = name;
            Path = path;
            FileName = fileName;
        }
    }
}
