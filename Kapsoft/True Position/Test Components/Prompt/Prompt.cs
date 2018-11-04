using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Security.Permissions;
using System.Runtime.Serialization;
using System.Windows.Forms;

namespace TruePosition.Test.Prompt
{
    public class UserPrompt : IPrompt
    {
        public event PromptShowingDelegate Showing;
        public event EventHandler<PromptEventArgs> Closed;
        public event EventHandler<PromptEventArgs> Continue;
        public event EventHandler<PromptErrorEventArgs> Error;

        public string Name { get; private set; }

        public UserPrompt(string name)
        {
            Name = name;
        }
        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }
        [SecurityPermissionAttribute(SecurityAction.Demand,SerializationFormatter=true)]
        protected UserPrompt(SerializationInfo info, StreamingContext context)
        {
            throw new NotImplementedException();
        }
        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            throw new NotImplementedException();
        }
        protected void Dispose(bool disposing)
        {
            try
            {
                if (disposing)
                {
                    // Just to be thorough, unhook all events...
                    if (Showing != null)
                        Delegate.RemoveAll(Showing, Showing);

                    if (Closed != null)
                        Delegate.RemoveAll(Closed, Closed);

                    if (Continue != null)
                        Delegate.RemoveAll(Continue, Continue);

                    if (Error != null)
                        Delegate.RemoveAll(Error, Error);
                }
            }
            finally
            {
            }
        }

        private void OnShowing(string text, bool modal, ref Form form)
        {
            if (Showing == null)
                throw new InvalidOperationException("Invalid Prompt delegate. At least one Showing delegate must be provided to display a user prompt.");
            else
                Showing(text, modal, ref form);
        }
        private void OnClosed(string text, int timeout)
        {
            if (Closed != null)
                Closed(this, new PromptEventArgs(Name, text, timeout));
        }
        private void OnContinue(string text, int timeout)
        {
            if (Continue != null)
                Continue(this, new PromptEventArgs(Name, text, timeout));
        }
        private void OnError(Exception ex)
        {
            if (Error != null)
                Error(null, new PromptErrorEventArgs(Name, ex.Message, ex.InnerException != null ? ex.InnerException.Message : null));
        }
        public void Show(string text)
        {
            Show(text, 0);
        }
        public void Show(int timeout)
        {
            Show(string.Empty, timeout);
        }
        private delegate void FormCallback();
        public void Show(string text, int timeout)
        {
            try
            {
                if ((string.IsNullOrEmpty(text)) && (timeout == 0))
                    throw new ArgumentException("Invalid Prompt values. Either text or timeout must be specified.");

                if (string.IsNullOrEmpty(text))
                {
                    Thread.Sleep(timeout * 1000);
                    OnClosed(text, timeout);
                }
                else if (timeout <= 0)
                {
                    Form form = null;
                    OnShowing(text, true, ref form);
                    OnClosed(text, timeout);
                }
                else
                {
                    ThreadPool.QueueUserWorkItem((s) =>
                                                {
                                                    Form form = null;
                                                    OnShowing(text, false, ref form);
                                                    if (form == null)
                                                        throw new InvalidOperationException("Invalid Prompt operation. When no timeout is specified, a Form reference must be returned.");

                                                    if (!form.Visible)
                                                        form.Show();

                                                    ThreadPool.RegisterWaitForSingleObject(new EventWaitHandle(false, EventResetMode.ManualReset),
                                                                                           (state, timedout) =>
                                                                                           {
                                                                                               if ((form != null) || (!form.IsDisposed))
                                                                                                   form.BeginInvoke(new FormCallback(() =>
                                                                                                                                     {
                                                                                                                                         form.Close();
                                                                                                                                         form.Dispose();
                                                                                                                                         form = null;
                                                                                                                                     }));

                                                                                               OnClosed(text, timeout);
                                                                                           },
                                                                                           null, timeout * 1000, true);

                                                    Application.Run(form);
                                                });
                    OnContinue(text, timeout);
                }
            }
            catch (Exception ex)
            {
                OnError(ex);
                throw ex;
            }
        }
    }
}
