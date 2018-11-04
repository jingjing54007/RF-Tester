using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using System.Windows.Forms;

namespace TruePosition.Test.Prompt
{
    public delegate void PromptShowingDelegate(string text, bool modal, ref Form form);
    public interface IPrompt : ISerializable, IDisposable
    {
        string Name { get; }
        void Show(string text, int timeout);

        event PromptShowingDelegate Showing;
        event EventHandler<PromptEventArgs> Closed;
        event EventHandler<PromptEventArgs> Continue;
        event EventHandler<PromptErrorEventArgs> Error;
    }
}
