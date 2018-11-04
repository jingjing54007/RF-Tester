using System;
using System.Collections.Generic;
using System.Text;

namespace TruePosition.Test.Prompt
{
    /// <summary>
    /// Event argument class for general Prompt events
    /// </summary>
    public class PromptEventArgs : System.EventArgs
    {
        public string Name { get; private set; }
        public string Text { get; private set; }
        public int Timeout { get; private set; }

        public PromptEventArgs(string name, string text, int timeout)
        {
            Initialize(name, text, timeout);
        }

        private void Initialize(string name, string text, int timeout)
        {
            if (string.IsNullOrEmpty(name))
                throw new ArgumentNullException("name");
            if (timeout < 0)
                throw new ArgumentNullException("timeout");

            Name = name;
            Text = text;
            Timeout = timeout;
        }
    }
}
