using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using TruePosition.Test.DataLayer;
using TruePosition.Test.IO;
using TruePosition.Test.QF;
using TruePosition.Test.Process;
using TruePosition.Test.Prompt;
using TruePosition.Test.Recorder;

namespace TruePosition.Test.UI
{
    /// <summary>
    /// Public events emitted by the TQF that can be subscribed to.
    /// </summary>
    public interface IHandlerTarget
    {
        EventHandler<PortMessageEventArgs> PortTransmitted { get; set; }
        EventHandler<PortMessageEventArgs> PortReceive { get; set; }
        EventHandler<PortErrorEventArgs> PortError  { get; set; }
        EventHandler<PortTimeoutExpiredEventArgs> PortTimeout { get; set; }

        EventHandler<ProcessEventArgs> ProcessLaunched { get; set; }
        EventHandler<ProcessEventArgs> ProcessKilled { get; set; }
        EventHandler<ProcessEventArgs> ProcessTimeout { get; set; }
        EventHandler<ProcessErrorEventArgs> ProcessError { get; set; }

        PromptShowingDelegate PromptShowing { get; set; }
        EventHandler<PromptEventArgs> PromptClosed { get; set; }
        EventHandler<PromptErrorEventArgs> PromptError { get; set; }

        RecorderOpeningDelegate RecorderOpening { get; set; }
        EventHandler<RecorderEventArgs> RecorderOpened { get; set; }
        RecorderRecordingDelegate RecorderRecording { get; set; }
        EventHandler<RecorderEventArgs> RecorderClosed { get; set; }
        EventHandler<RecorderErrorEventArgs> RecorderError { get; set; }

        TestSteppingDelegate TestStepping { get; set; }
        EventHandler<TestErrorEventArgs> TestError { get; set; }
        EventHandler<TestSteppedEventArgs> TestStepped  { get; set; }
        EventHandler<TestPassedEventArgs> TestPassed  { get; set; }
        EventHandler<TestFailedEventArgs> TestFailed  { get; set; }
        EventHandler<TestAbortedEventArgs> TestAborted { get; set; }
        EventHandler<TestUnloadedEventArgs> TestUnloaded { get; set; }
    }

    public static class HandlerHelper
    {
        static HandlerHelper() { }

        class HandlerTarget : IHandlerTarget
        {
            public EventHandler<PortMessageEventArgs> PortTransmitted { get; set; }
            public EventHandler<PortMessageEventArgs> PortReceive { get; set; }
            public EventHandler<PortErrorEventArgs> PortError { get; set; }
            public EventHandler<PortTimeoutExpiredEventArgs> PortTimeout { get; set; }

            public EventHandler<ProcessEventArgs> ProcessLaunched { get; set; }
            public EventHandler<ProcessEventArgs> ProcessKilled { get; set; }
            public EventHandler<ProcessEventArgs> ProcessTimeout { get; set; }
            public EventHandler<ProcessErrorEventArgs> ProcessError { get; set; }

            public PromptShowingDelegate PromptShowing { get; set; }
            public EventHandler<PromptEventArgs> PromptClosed { get; set; }
            public EventHandler<PromptErrorEventArgs> PromptError { get; set; }

            public RecorderOpeningDelegate RecorderOpening { get; set; }
            public EventHandler<RecorderEventArgs> RecorderOpened { get; set; }
            public RecorderRecordingDelegate RecorderRecording { get; set; }
            public EventHandler<RecorderEventArgs> RecorderClosed { get; set; }
            public EventHandler<RecorderErrorEventArgs> RecorderError { get; set; }

            public TestSteppingDelegate TestStepping { get; set; }
            public EventHandler<TestErrorEventArgs> TestError { get; set; }
            public EventHandler<TestSteppedEventArgs> TestStepped { get; set; }
            public EventHandler<TestPassedEventArgs> TestPassed { get; set; }
            public EventHandler<TestFailedEventArgs> TestFailed { get; set; }
            public EventHandler<TestAbortedEventArgs> TestAborted { get; set; }
            public EventHandler<TestUnloadedEventArgs> TestUnloaded { get; set; }
        }

        private static readonly IHandlerTarget handlers = new HandlerTarget();
        public static IHandlerTarget Handlers { get { return handlers; } }

        public static void Clear()
        {
            Delegate.RemoveAll(handlers.PortTransmitted, handlers.PortTransmitted);
            Delegate.RemoveAll(handlers.PortReceive, handlers.PortReceive);
            Delegate.RemoveAll(handlers.PortTimeout, handlers.PortTimeout);
            Delegate.RemoveAll(handlers.PortError, handlers.PortError);

            Delegate.RemoveAll(handlers.ProcessError, handlers.ProcessError);
            Delegate.RemoveAll(handlers.ProcessKilled, handlers.ProcessKilled);
            Delegate.RemoveAll(handlers.ProcessLaunched, handlers.ProcessLaunched);
            Delegate.RemoveAll(handlers.ProcessTimeout, handlers.ProcessTimeout);

            Delegate.RemoveAll(handlers.PromptError, handlers.PromptError);
            Delegate.RemoveAll(handlers.PromptShowing, handlers.PromptShowing);
            Delegate.RemoveAll(handlers.PromptClosed, handlers.PromptClosed);

            Delegate.RemoveAll(handlers.RecorderOpening, handlers.RecorderOpening);
            Delegate.RemoveAll(handlers.RecorderOpened, handlers.RecorderOpened);
            Delegate.RemoveAll(handlers.RecorderRecording, handlers.RecorderRecording);
            Delegate.RemoveAll(handlers.RecorderClosed, handlers.RecorderClosed);
            Delegate.RemoveAll(handlers.RecorderError, handlers.RecorderError);

            Delegate.RemoveAll(handlers.TestAborted, handlers.TestAborted);
            Delegate.RemoveAll(handlers.TestError, handlers.TestError);
            Delegate.RemoveAll(handlers.TestFailed, handlers.TestFailed);
            Delegate.RemoveAll(handlers.TestPassed, handlers.TestPassed);
            Delegate.RemoveAll(handlers.TestUnloaded, handlers.TestUnloaded);
            Delegate.RemoveAll(handlers.TestStepped, handlers.TestStepped);
            Delegate.RemoveAll(handlers.TestStepping, handlers.TestStepping);
        }

        public static EventHandler<PortMessageEventArgs> PortTransmitted
        {
            get { return handlers.PortTransmitted; }
            set { handlers.PortTransmitted = value; }
        }
        public static EventHandler<PortMessageEventArgs> PortReceive
        {
            get { return handlers.PortReceive; }
            set { handlers.PortReceive = value; }
        }
        public static EventHandler<PortErrorEventArgs> PortError
        {
            get { return handlers.PortError; }
            set { handlers.PortError = value; }
        }
        public static EventHandler<PortTimeoutExpiredEventArgs> PortTimeout
        {
            get { return handlers.PortTimeout; }
            set { handlers.PortTimeout = value; }
        }

        public static EventHandler<ProcessEventArgs> ProcessLaunched
        {
            get { return handlers.ProcessLaunched; }
            set { handlers.ProcessLaunched = value; }
        }
        public static EventHandler<ProcessEventArgs> ProcessKilled
        {
            get { return handlers.ProcessKilled; }
            set { handlers.ProcessKilled = value; }
        }
        public static EventHandler<ProcessEventArgs> ProcessTimeout
        {
            get { return handlers.ProcessTimeout; }
            set { handlers.ProcessTimeout = value; }
        }
        public static EventHandler<ProcessErrorEventArgs> ProcessError
        {
            get { return handlers.ProcessError; }
            set { handlers.ProcessError = value; }
        }

        public static PromptShowingDelegate PromptShowing
        {
            get { return handlers.PromptShowing; }
            set { handlers.PromptShowing = value; }
        }
        public static EventHandler<PromptEventArgs> PromptClosed
        {
            get { return handlers.PromptClosed; }
            set { handlers.PromptClosed = value; }
        }
        public static EventHandler<PromptErrorEventArgs> PromptError
        {
            get { return handlers.PromptError; }
            set { handlers.PromptError = value; }
        }

        public static RecorderOpeningDelegate RecorderOpening
        {
            get { return handlers.RecorderOpening; }
            set { handlers.RecorderOpening = value; }
        }
        public static EventHandler<RecorderEventArgs> RecorderOpened
        {
            get { return handlers.RecorderOpened; }
            set { handlers.RecorderOpened = value; }
        }
        public static RecorderRecordingDelegate RecorderRecording
        {
            get { return handlers.RecorderRecording; }
            set { handlers.RecorderRecording = value; }
        }
        public static EventHandler<RecorderEventArgs> RecorderClosed
        {
            get { return handlers.RecorderClosed; }
            set { handlers.RecorderClosed = value; }
        }
        public static EventHandler<RecorderErrorEventArgs> RecorderError
        {
            get { return handlers.RecorderError; }
            set { handlers.RecorderError = value; }
        }

        public static TestSteppingDelegate TestStepping
        {
            get { return handlers.TestStepping; }
            set { handlers.TestStepping = value; }
        }
        public static EventHandler<TestErrorEventArgs> TestError
        {
            get { return handlers.TestError; }
            set { handlers.TestError = value; }
        }
        public static EventHandler<TestSteppedEventArgs> TestStepped
        {
            get { return handlers.TestStepped; }
            set { handlers.TestStepped = value; }
        }
        public static EventHandler<TestPassedEventArgs> TestPassed
        {
            get { return handlers.TestPassed; }
            set { handlers.TestPassed = value; }
        }
        public static EventHandler<TestFailedEventArgs> TestFailed
        {
            get { return handlers.TestFailed; }
            set { handlers.TestFailed = value; }
        }
        public static EventHandler<TestAbortedEventArgs> TestAborted
        {
            get { return handlers.TestAborted; }
            set { handlers.TestAborted = value; }
        }
        public static EventHandler<TestUnloadedEventArgs> TestUnloaded
        {
            get { return handlers.TestUnloaded; }
            set { handlers.TestUnloaded = value; }
        }
    }
}
