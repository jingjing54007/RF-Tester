using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Security.Permissions;
using System.Runtime.Serialization;

using TruePosition.Test.DataLayer;
using System.Xml.Linq;

namespace TruePosition.Test.Recorder
{
    public class FileRecorder : IRecorder
    {
        public event RecorderOpeningDelegate Opening;
        public event EventHandler<RecorderEventArgs> Opened;
        public event RecorderRecordingDelegate Recording;
        public event EventHandler<RecorderEventArgs> Closed;
        public event EventHandler<RecorderErrorEventArgs> Error;

        public string Name { get; set; }

        private string FileName { get; set; }
        private string Path { get; set; }
        private System.IO.StreamWriter File { get; set; }

        private void Initialize()
        {
        }
        public FileRecorder()
        {
            Initialize();
        }
        public FileRecorder(string name)
        {
            Initialize();
            Name = name;
        }
        [SecurityPermissionAttribute(SecurityAction.Demand, SerializationFormatter = true)]
        protected FileRecorder(SerializationInfo info, StreamingContext context)
        {
            throw new NotImplementedException();
        }
        public void GetObjectData(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context)
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
                    if (Opened != null)
                        Delegate.RemoveAll(Opened, Opened);

                    if (Closed != null)
                        Delegate.RemoveAll(Closed, Closed);

                    if (Error != null)
                        Delegate.RemoveAll(Error, Error);

                    if (File != null)
                    {
                        File.Close();
                        File.Dispose();
                    }
                }
            }
            finally
            {
            }
        }

        public void OnError(Exception ex)
        {
            if (Error != null)
                Error(null, new RecorderErrorEventArgs(Name, ex.Message, ex.InnerException != null ? ex.InnerException.Message : null));
        }
        public void OnOpening(string testName, int stepNumber, string format, IEnumerable<XElement> values, string command, string response, ref string location)
        {
            if (Opening != null)
                Opening(testName, stepNumber, format, values, command, response, ref location);
        }
        private void OnOpened()
        {
            if ((Opened != null) && (File != null))
                Opened(null, new RecorderEventArgs(Name, Path, FileName));
        }
        public void OnRecording(string testName, int stepNumber, string format, IEnumerable<XElement> values, string command, string response, ref string entry)
        {
            if (File == null)
                throw new InvalidOperationException("Attempt to raise the Recording event failed. The Recorder's associated file has not been opened.");

            if ((Recording != null) && (File != null))
                Recording(testName, stepNumber, format, values, command, response, ref entry);
        }
        private void OnClosed()
        {
            if (Closed != null)
                Closed(null, new RecorderEventArgs(Name, Path, FileName));
        }

        private void Parse(string path)
        {
            Path = System.IO.Path.GetDirectoryName(path);
            FileName = System.IO.Path.GetFileName(path);

            if (string.IsNullOrEmpty(Path))
                throw new ArgumentNullException("Path");
            if (string.IsNullOrEmpty(FileName))
                throw new ArgumentNullException("FileName");
        }
        public void Open(string location)
        {
            try
            {
                Parse(location);
                File = System.IO.File.AppendText(location);
                OnOpened();
            }   
            catch (Exception ex)
            {
                OnError(ex);
                throw ex;
            }
        }
        public void Record(string entry)
        {
            try
            {
                if (File != null)
                    File.Write(entry.EndsWith("\r\n") ? entry : entry + "\r\n");
            }
            catch (Exception ex)
            {
                OnError(ex);
                throw ex;
            }
        }
        public void Close()
        {
            try
            {
                if (File != null)
                {
                    File.Flush();
                    File.Close();
                    File.Dispose();
                    File = null;
                    OnClosed();
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
