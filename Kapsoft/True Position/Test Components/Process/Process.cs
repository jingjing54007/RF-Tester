using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Security.Permissions;
using System.Runtime.Serialization;
using System.Timers;

namespace TruePosition.Test.Process
{
    public class WindowsProcess : IProcess, IProcessConfig
    {
        private const int DEF_LAUNCH_TIMEOUT = 30000;
        private int DEF_LAUNCH_DELAY = 0;

        public event EventHandler<ProcessEventArgs> Launched;
        public event EventHandler<ProcessEventArgs> Killed;
        public event EventHandler<ProcessEventArgs> Timeout;
        public event EventHandler<ProcessErrorEventArgs> Error;
        
        public string Name { get; set; }
        public int PreLaunchDelay { get; set; }
        public int PostLaunchDelay { get; set; }
        public int LaunchTimeout { get; set; }

        private string ExecutableName { get; set; }
        private string Path { get; set; }
        private string CommandLine { get; set; }
        private System.Diagnostics.Process Process { get; set; }
        private Timer TimeoutTimer = new Timer();

        private void Initialize()
        {
            PreLaunchDelay = DEF_LAUNCH_DELAY;
            PostLaunchDelay = DEF_LAUNCH_DELAY;
            LaunchTimeout = DEF_LAUNCH_TIMEOUT;

            TimeoutTimer.Enabled = false;
            TimeoutTimer.Elapsed += new ElapsedEventHandler(TimeoutTimer_Elapsed);
        }
        public WindowsProcess() 
        {
            Initialize();
        }
        public WindowsProcess(string name)
        {
            Initialize();
            Name = name;
        }
        [SecurityPermissionAttribute(SecurityAction.Demand,SerializationFormatter=true)]
        protected WindowsProcess(SerializationInfo info, StreamingContext context)
        {
            throw new NotImplementedException();
        }
        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            throw new NotImplementedException();
        }
        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }
        protected void Dispose(bool disposing)
        {
            try
            {
                if (disposing)
                {
                    // Just to be thorough, unhook all events...
                    if (Launched != null)
                        Delegate.RemoveAll(Launched, Launched);

                    if (Killed != null)
                        Delegate.RemoveAll(Killed, Killed);

                    if (Error != null)
                        Delegate.RemoveAll(Error, Error);

                    if (Timeout != null)
                        Delegate.RemoveAll(Timeout, Timeout);

                    if (TimeoutTimer != null)
                    {
                        TimeoutTimer.Enabled = false;
                        TimeoutTimer.Elapsed -= TimeoutTimer_Elapsed;
                        TimeoutTimer.Dispose();

                        TimeoutTimer = null;
                    }

                    if (Process != null)
                    {
                        Process.Close();
                        Process.Dispose();
                    }
                }
            }
            finally
            {
            }
        }

        private void TimeoutTimer_Elapsed(object sender, System.Timers.ElapsedEventArgs args)
        {
            TimeoutTimer.Enabled = false;
            Kill();
            OnTimeout();
        }
        private void OnError(Exception ex)
        {
            TimeoutTimer.Enabled = false;
            if (Error != null)
                Error(null, new ProcessErrorEventArgs(Name, ex.Message, ex.InnerException != null ? ex.InnerException.Message : null)); 
        }
        private void OnLaunched()
        {
            TimeoutTimer.Enabled = false;
            if ((Launched != null) && (Process != null) && (!Process.HasExited))
                Launched(null, new ProcessEventArgs(Name, ExecutableName, CommandLine));
        }
        private void OnKilled()
        {
            TimeoutTimer.Enabled = false;
            if (Killed != null)
                Killed(null, new ProcessEventArgs(Name, ExecutableName, CommandLine));
        }
        private void OnTimeout()
        {
            TimeoutTimer.Enabled = false;
            if (Timeout != null)
                Timeout(null, new ProcessEventArgs(Name, ExecutableName, CommandLine));
        }

        private void Parse(string commandLine)
        {
            int commandLineIndex = commandLine.ToLower().IndexOf(".exe") + 4;
            commandLineIndex += commandLine.Substring(commandLineIndex).IndexOf(" ");
            string exePath = commandLine.Substring(0, commandLineIndex).Trim('\"');

            Path = System.IO.Path.GetDirectoryName(exePath);
            ExecutableName = System.IO.Path.GetFileName(exePath);
            CommandLine = commandLine.Remove(0, commandLineIndex).Trim();

            if (string.IsNullOrEmpty(Path))
                throw new ArgumentNullException("Path");
            if (string.IsNullOrEmpty(ExecutableName))
                throw new ArgumentNullException("ExecutableName");
        }
        public void Launch(string commandLine)
        {
            try
            {
                Parse(commandLine);
                if (PreLaunchDelay != 0)
                    System.Threading.Thread.Sleep(PreLaunchDelay);

                ProcessStartInfo info = new ProcessStartInfo();
                info.FileName = System.IO.Path.Combine(Path, ExecutableName);
                info.Arguments = commandLine;
                info.UseShellExecute = false;
                info.WorkingDirectory = Path;

                TimeoutTimer.Interval = LaunchTimeout + PostLaunchDelay;
                TimeoutTimer.Enabled = true;
                Process = System.Diagnostics.Process.Start(info);
                if (PostLaunchDelay != 0)
                    System.Threading.Thread.Sleep(PostLaunchDelay);
                OnLaunched();
            }
            catch (Exception ex)
            {
                OnError(ex);
                throw ex;
            }
        }
        public void Kill()
        {
            try
            {
                if ((Process != null) && (!Process.HasExited))
                {
                    Process.Kill();
                    OnKilled();
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
