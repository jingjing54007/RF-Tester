using System;
using System.Collections.Generic;
using System.Text;

namespace TruePosition.Test.Process
{
    /// <summary>
    /// Event argument class for general Process events
    /// </summary>
    public class ProcessEventArgs : System.EventArgs
    {
        public string Name { get; private set; }
        public string ProcessName { get; private set; }
        public string CommandLine { get; private set; }

        public ProcessEventArgs(string name, string processName, string commandLine)
        {
            Initialize(name, processName, commandLine);
        }

        private void Initialize(string name, string processName, string commandLine)
        {
            if (string.IsNullOrEmpty(name))
                throw new ArgumentNullException("name");
            if (string.IsNullOrEmpty(processName))
                throw new ArgumentNullException("processName");
            if (string.IsNullOrEmpty(processName))
                throw new ArgumentNullException("commandLine");

            Name = name;
            ProcessName = processName;
            CommandLine = commandLine;
        }
    }
}
