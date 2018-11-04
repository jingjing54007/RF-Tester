using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using System.Xml.Linq;

namespace TruePosition.Test.Recorder
{
    public delegate void RecorderOpeningDelegate(string testName, int stepNumber, string format, IEnumerable<XElement> values, string command, string response, ref string location);
    public delegate void RecorderRecordingDelegate(string testName, int stepNumber, string format, IEnumerable<XElement> values, string command, string response, ref string entry);
    public interface IRecorder : ISerializable, IDisposable
    {
        string Name { get; set; }

        void OnOpening(string testName, int stepNumber, string format, IEnumerable<XElement> values, string command, string response, ref string location);
        void Open(string location);
        void OnRecording(string testName, int stepNumber, string format, IEnumerable<XElement> values, string command, string response, ref string entry);
        void Record(string entry);
        void Close();
        void OnError(Exception ex);

        event RecorderOpeningDelegate Opening;
        event EventHandler<RecorderEventArgs> Opened;
        event RecorderRecordingDelegate Recording;
        event EventHandler<RecorderEventArgs> Closed;
        event EventHandler<RecorderErrorEventArgs> Error;
    }
}
