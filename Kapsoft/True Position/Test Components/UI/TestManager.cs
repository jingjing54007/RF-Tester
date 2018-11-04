using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Reflection;
using System.Diagnostics;
using System.Threading;
using System.Text;
using System.Windows.Forms;
using System.Xml.Linq;

using TruePosition.Test.DataLayer;
using TruePosition.Test.IO;
using TruePosition.Test.QF;
using TruePosition.Test.Process;
using TruePosition.Test.Prompt;
using TruePosition.Test.Recorder;

namespace TruePosition.Test.UI
{
    /// <summary>
    /// Orchestrates ActiveObject management and operation between UI and TQF
    /// </summary>
    public static class TestManager
    {
        private static object thisLock = new object();
        private static EventWaitHandle testComplete = new EventWaitHandle(false, EventResetMode.AutoReset);
        private static event EventHandler<TestUnloadedEventArgs> TestUnloaded;

        private class StepComparer : IEqualityComparer<Step>
        {
            #region IEqualityComparer<Step> Members

            public bool Equals(Step x, Step y)
            {
                if ((x.Actor.Type == y.Actor.Type) && (x.Actor.Name == y.Actor.Name))
                    return true;
                else
                    return false;
            }

            public int GetHashCode(Step obj)
            {
                return (obj.Actor.Type.ToString() + obj.Actor.Name).GetHashCode();
            }

            #endregion
        }

        private static void PortDataReceived(object sender, PortMessageEventArgs args)
        {
            Debug.Print("<Event=TestManager.PortDataReceived,\tPort=" + args.Name + ",\tMessage='" + args.Message + "'>");
        }
        private static void PortError(object sender, PortErrorEventArgs args)
        {
            Debug.Print("<Event=TestManager.PortError,\tPort=" + args.Name + ",\tError='" + args.Error + "',\tAdditional='" + args.AdditionalDescription + "'>");
        }
        private static void PortReceiveTimeoutExpired(object sender, PortTimeoutExpiredEventArgs args)
        {
            Debug.Print("<Event=TestManager.PortReceiveTimeoutExpired,\tPort=" + args.Name + "'>");
        }

        private static void ProcessLaunched(object sender, ProcessEventArgs args)
        {
            Debug.Print("<Event=TestManager.ProcessLaunched,\tProcess=" + args.Name + ",\tProcess Name='" + args.ProcessName+ "'>");
        }
        private static void ProcessKilled(object sender, ProcessEventArgs args)
        {
            Debug.Print("<Event=TestManager.ProcessKilled,\tProcess=" + args.Name + ",\tProcess Name='" + args.ProcessName + "'>");
        }
        private static void ProcessTimeout(object sender, ProcessEventArgs args)
        {
            Debug.Print("<Event=TestManager.ProcessTimeout,\tProcess=" + args.Name + ",\tProcess Name='" + args.ProcessName + "'>");
        }
        private static void ProcessError(object sender, ProcessErrorEventArgs args)
        {
            Debug.Print("<Event=TestManager.ProcessError,\tProcess=" + args.Name + ",\tError='" + args.Error + "',\tAdditional='" + args.AdditionalDescription + "'>");
        }

        private static void PromptShowing(string text, bool modal, ref Form form)
        {
            Debug.Print("<Event=TestManager.PromptShowing,\tText=" + text + "'>");
        }
        private static void PromptClosed(object sender, PromptEventArgs args)
        {
            Debug.Print("<Event=TestManager.PromptClosed,\tPrompt=" + args.Name + ",\tPrompt Name='" + args.Text + "'>");
        }
        private static void PromptError(object sender, PromptErrorEventArgs args)
        {
            Debug.Print("<Event=TestManager.PromptError,\tPrompt=" + args.Name + ",\tError='" + args.Error + "'>");
        }

        private static void RecorderOpening(string testName, int stepNumber, string format, IEnumerable<XElement> values, string command, string response, ref string location)
        {
            Debug.Print("<Event=TestManager.RecorderOpening,\tTest=" + testName + "',\tNumber=" + stepNumber + ",\tFormat='" + format + ",\tCommand='" + command + ",\tResponse='" + response + ">");
            location = EntryProcessing(testName, stepNumber, format, values, command, response);
        }
        private static void RecorderOpened(object sender, RecorderEventArgs args)
        {
            Debug.Print("<Event=TestManager.RecorderOpened,\tRecorder=" + args.Name + ",\tFilename='" + args.FileName + ",\tPath='" + args.Path + "'>");
        }
        private static void RecorderRecording(string testName, int stepNumber, string format, IEnumerable<XElement> values, string command, string response, ref string entry)
        {
            Debug.Print("<Event=TestManager.RecorderRecording,\tTest=" + testName + "',\tNumber=" + stepNumber + ",\tFormat='" + format + ",\tCommand='" + command + ",\tResponse='" + response + ">");
            entry = EntryProcessing(testName, stepNumber, format, values, command, response);  
        }
        private static void RecorderClosed(object sender, RecorderEventArgs args)
        {
            Debug.Print("<Event=TestManager.RecorderClosed,\tRecorder=" + args.Name + ",\tFilename='" + args.FileName + ",\tPath='" + args.Path + "'>");
        }
        private static void RecorderError(object sender, RecorderErrorEventArgs args)
        {
            Debug.Print("<Event=TestManager.RecorderError,\tRecorder=" + args.Name + ",\tError='" + args.Error + "'>");
        }

        private static void TestStepping(string testName, Step step, ref string augmentedCommand)
        {
            Debug.Print("<Event=TestManager.TestStepping,\tTest=" + testName + ",\tCommand='" + step.Actor.Action + "',\tNumber=" + step.Number + ">");

            if (step.Actor.Type == ActorType.Recorder)
                augmentedCommand = StepProcessing(testName, step.Number, augmentedCommand, step.Actor.Command.Descendants("Value").ToList());
        }
        private static void TestError(object sender, TestErrorEventArgs args)
        {
            Debug.Print("<Event=TestManager.TestError,\tTest=" + args.Name + ",\tCommand='" + args.Step.Actor.Action + "',\tNumber=" + args.Step.Number + ",\tReason=" + args.Reason + ">");
        }
        private static void TestStepped(object sender, TestSteppedEventArgs args)
        {
            Debug.Print("<Event=TestManager.TestStepped,\tTest=" + args.Name + ",\tCommand='" + args.Step.Actor.Action + "',\tNumber=" + args.Step.Number + ">");
        }
        private static void TestPassed(object sender, TestPassedEventArgs args)
        {
            Debug.Print("<Event=TestManager.TestPassed,\tTest=" + args.Name + ">");
            testComplete.Set();
        }
        private static void TestFailed(object sender, TestFailedEventArgs args)
        {
            Debug.Print("<Event=TestManager.TestFailed,\tTest=" + args.Name + ",\tCommand='" + args.Step.Actor.Action + "',\tNumber=" + args.Step.Number + ",\tReason=" + args.Reason + ">");
            testComplete.Set();
        }
        private static void TestAborted(object sender, TestAbortedEventArgs args)
        {
            Debug.Print("<Event=TestManager.TestAborted,\tTest=" + args.Name + ",\tReason=" + args.Reason + ">");
            testComplete.Set();
        }

#if UUT_TEST == false
        private static void Load(DataLayer.Test test, IEnumerable<Actor> actors, IHandlerTarget handlerTarget, string configPath)
#else
        private static void Load(DataLayer.Test test, IEnumerable<Actor> actors, IHandlerTarget handlerTarget, string configPath, UUTTestMode mode)
#endif
        {
            if (!string.IsNullOrEmpty(configPath))
                ConfigManager.SetLocation(configPath);

            foreach (Actor actor in actors)
            {
                switch (actor.Type)
                {
                    case ActorType.Port:
                        QFManager.Port.Create((PortType)Enum.Parse(typeof(PortType), actor.SubType), ConfigManager.ReadStream(actor.Name));
                        QFManager.Port.Receive(actor.Name, PortDataReceived);
                        QFManager.Port.Error(actor.Name, PortError);
                        QFManager.Port.ReceiveTimeout(actor.Name, PortReceiveTimeoutExpired);
                        if (handlerTarget != null)
                        {
                            QFManager.Port.Receive(actor.Name, handlerTarget.PortReceive);
                            QFManager.Port.ReceiveTimeout(actor.Name, handlerTarget.PortTimeout);
                            QFManager.Port.Error(actor.Name, handlerTarget.PortError);
                        }
                        break;
                    case ActorType.Process:
                        QFManager.Process.Create(ConfigManager.ReadStream(actor.Name));
                        QFManager.Process.Launched(actor.Name, ProcessLaunched);
                        QFManager.Process.Killed(actor.Name, ProcessKilled);
                        QFManager.Process.Timeout(actor.Name, ProcessTimeout);
                        QFManager.Process.Error(actor.Name, ProcessError);
                        if (handlerTarget != null)
                        {
                            QFManager.Process.Launched(actor.Name, handlerTarget.ProcessLaunched);
                            QFManager.Process.Killed(actor.Name, handlerTarget.ProcessKilled);
                            QFManager.Process.Timeout(actor.Name, handlerTarget.ProcessTimeout);
                            QFManager.Process.Error(actor.Name, handlerTarget.ProcessError);
                        }
                        break;
                    case ActorType.Prompt:
                        QFManager.Prompt.Create(actor.Name);
                        QFManager.Prompt.Showing(actor.Name, PromptShowing);
                        QFManager.Prompt.Closed(actor.Name, PromptClosed);
                        QFManager.Prompt.Error(actor.Name, PromptError);
                        if (handlerTarget != null)
                        {
                            QFManager.Prompt.Showing(actor.Name, handlerTarget.PromptShowing);
                            QFManager.Prompt.Closed(actor.Name, handlerTarget.PromptClosed);
                            QFManager.Prompt.Error(actor.Name, handlerTarget.PromptError);
                        }
                        break;
                    case ActorType.Recorder:
                        QFManager.Recorder.Create(RecorderType.File, actor.Name);
                        QFManager.Recorder.Opening(actor.Name, RecorderOpening);
                        QFManager.Recorder.Opened(actor.Name, RecorderOpened);
                        QFManager.Recorder.Recording(actor.Name, RecorderRecording);
                        QFManager.Recorder.Closed(actor.Name, RecorderClosed);
                        QFManager.Recorder.Error(actor.Name, RecorderError);
                        if (handlerTarget != null)
                        {
                            QFManager.Recorder.Opened(actor.Name, handlerTarget.RecorderOpened);
                            QFManager.Recorder.Recording(actor.Name, handlerTarget.RecorderRecording);
                            QFManager.Recorder.Closed(actor.Name, handlerTarget.RecorderClosed);
                            QFManager.Recorder.Error(actor.Name, handlerTarget.RecorderError);
                        }
                        break;
                    default:
                        throw new NotImplementedException();
                }
            }

#if UUT_TEST == false
            QFManager.Test.Load(test);
#else
            QFManager.Test.Load(test, mode);
#endif

            QFManager.Test.Stepping(test.Name, TestStepping);
            QFManager.Test.Error(test.Name, TestError);
            QFManager.Test.Stepped(test.Name, TestStepped);
            QFManager.Test.Passed(test.Name, TestPassed);
            QFManager.Test.Failed(test.Name, TestFailed);
            QFManager.Test.Aborted(test.Name, TestAborted);
            if (handlerTarget != null)
            {
                QFManager.Test.Stepping(test.Name, handlerTarget.TestStepping);
                QFManager.Test.Error(test.Name, handlerTarget.TestError);
                QFManager.Test.Stepped(test.Name, handlerTarget.TestStepped);
                QFManager.Test.Aborted(test.Name, handlerTarget.TestAborted);
                QFManager.Test.Passed(test.Name, handlerTarget.TestPassed);
                QFManager.Test.Failed(test.Name, handlerTarget.TestFailed);
                if (TestUnloaded == null)
                    TestUnloaded += handlerTarget.TestUnloaded;
            }
        }
        private static void Bind(DataLayer.Test test, string path)
        {
            if (string.IsNullOrEmpty(path))
                path = Environment.CurrentDirectory;
            else if (!Directory.Exists(path))
                throw new ArgumentException("Path " + path + " does not exist.", "path");

            DirectoryInfo directories = new DirectoryInfo(path);
            FileInfo[] files = directories.GetFiles("*.dll").Where(f => f.Name.StartsWith("TruePosition.Test.Custom")).ToArray();

            foreach (FileInfo info in files)
            {
                try
                {
                    Assembly.LoadFile(info.FullName);
                }
                catch (Exception ex) { }
            }

            foreach (Assembly assembly in AppDomain.CurrentDomain.GetAssemblies())
                ResponseProcessor.BindEvents(test, assembly);
        }

        private static bool Validate(string testName, int stepNumber, string format, IEnumerable<XElement> values)
        {
            if ((string.IsNullOrEmpty(format)) && ((values == null) || (values.Count() == 0)))
                throw new InvalidOperationException("Invalid Command XML detected. No Format or Parameter values found. Test '" + testName + "' Step=" + stepNumber + ".");

            if ((!string.IsNullOrEmpty(format)) && ((values != null) && (values.Count() > 0)))
            {
                int valueCount = values.Count();
                int index = 0;
                int indexCount = format.Split(new char[] { '{', '}', ' ' }, StringSplitOptions.RemoveEmptyEntries).Where(t => Int32.TryParse(t, out index)).Count();
                if (valueCount < indexCount)
                    throw new InvalidOperationException("Invalid Command XML detected. Format index count (" + valueCount + ") is greater the parameter count (" + indexCount + "). Test '" + testName + "' Step=" + stepNumber + ".");
            }

            return true;
        }
        private static string RunProcessing(string testName, int stepNumber, string format, IEnumerable<XElement> values)
        {
            Validate(testName, stepNumber, format, values);
            if ((string.IsNullOrEmpty(format)) && ((values != null) && (values.Count() > 0)))
            {
                return values.Aggregate("", (a, n) => String.IsNullOrEmpty(a) ? n.Value : " " + n.Value);
            }
            else
            {
                IGlobalConfig globalConfig = null;
                ConfigManager.Load(out globalConfig);

                foreach (string token in format.Split(new char[] { '{', '}' }, StringSplitOptions.RemoveEmptyEntries).Where(t => t.Contains("Global.")))
                {
                    string name = token.Trim().Replace("Global.", "");
                    format = format.Replace("{" + token + "}", Convert.ToString(typeof(IGlobalConfig).GetProperty(name).GetValue(globalConfig, null)));
                }

                return format;
            }
        }
        private static string Evaluate(string testName, int stepNumber, string format, IEnumerable<XElement> values)
        {
            if (values.Count() == 0)
                return format;
            else
            {
                try
                {
                    return String.Format(format, values.Select(v =>
                                                                {
                                                                    if (v.Value.ToLower().Trim().Equals("now"))
                                                                        return (object)DateTime.Now;
                                                                    else
                                                                        return (object)v.Value;
                                                                }).ToArray());
                }
                catch (Exception ex)
                {
                    throw new InvalidOperationException("Invalid Command XML detected. Invalid format. Test '" + testName + "' Step=" + stepNumber + ".");
                }
            }
        }
        private static string StepProcessing(string testName, int stepNumber, string format, IEnumerable<XElement> values)
        {
            if (!format.Contains("{Command") && !format.Contains("{Response"))
                return Evaluate(testName, stepNumber, format, values);
            return format;
        }
        private static string Substitute(string testName, int stepNumber, string format, string source, string id)
        {
            string[] fields = source.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            foreach (string token in format.Split(new char[] { '{', '}' }, StringSplitOptions.RemoveEmptyEntries).Where(t => t.Contains(id)))
            {
                int index = 0;
                string fieldId = token.Replace(id, "").Split(new char[] { '[', ']' }, StringSplitOptions.RemoveEmptyEntries).First();
                if (fieldId == "*")
                    format = format.Replace("{" + token + "}", source);
                else if (!Int32.TryParse(fieldId, out index))
                    throw new InvalidOperationException("Invalid Entry Format detected. Invalid " + id + " indexing format. Test '" + testName + "' Step=" + stepNumber + " Indexer='" + token + "'.");
                else if ((index <= 0) || (index > fields.Length))
                    throw new InvalidOperationException("Invalid Entry Format detected. Invalid " + id + " index. Index > # of fields in actual " + id + ". Test '" + testName + "' Step=" + stepNumber + ".");
                else
                    format = format.Replace("{" + token + "}", fields[index - 1]);
            }

            return format;
        }
        private static string EntryProcessing(string testName, int stepNumber, string format, IEnumerable<XElement> values, string command, string response)
        {
            if (string.IsNullOrEmpty(format))
                return response;

            if (!string.IsNullOrEmpty(command))
                format = Substitute(testName, stepNumber, format, command, "Command");
            if (!string.IsNullOrEmpty(response))
                format = Substitute(testName, stepNumber, format, response, "Response");
            return Evaluate(testName, stepNumber, format, values);
        }
        private static string Value(XElement x)
        {
            if (x == null)
                return string.Empty;
            else
                return x.Value;
        }
        private static string Value(XElement element, string defaultValue)
        {
            if (element == null)
                return defaultValue;
            else
                return element.Value;
        }
        private static string Value(XAttribute element, string defaultValue)
        {
            if (element == null)
                return defaultValue;
            else
                return element.Value;
        }
        private static void Substitute(string testName, Step step)
        {
            IEnumerable<XElement> values = step.Actor.Command.Descendants("Value").ToList();

            XElement xFormat = null;
            switch (step.Actor.Type)
            {
                case ActorType.Recorder:
                    xFormat = step.Actor.Command.Element("FilenameFormat");
                    step.Actor.Action = RunProcessing(testName, step.Number, Value(xFormat), values);
                    xFormat = step.Actor.Command.Element("EntryFormat");
                    step.Actor.EntryFormat = RunProcessing(testName, step.Number, Value(xFormat), values);
                    step.Actor.Style = (EntryStyle)Enum.Parse(typeof(EntryStyle), Value(xFormat.Attribute("Style"), "Single"), true);
                    break;
                default:
                    xFormat = step.Actor.Command.Element("Format");
                    step.Actor.Action = Evaluate(testName,
                                                 step.Number,
                                                 RunProcessing(testName, step.Number, Value(xFormat), values),
                                                 values);
                    break;
            }
        }
        private static void Substitute(DataLayer.Test test)
        {
            foreach (Step step in test.Steps)
                Substitute(test.Name, step);
        }
        private static void Run(DataLayer.Test test, IEnumerable<Actor> actors)
        {
            foreach (Actor actor in actors)
            {
                switch (actor.Type)
                {
                    case ActorType.Port:
                        QFManager.Port.Open(actor.Name);
                        break;
                    case ActorType.Process:
                    case ActorType.Prompt:
                    case ActorType.Recorder:
                        // DESIGN NOTE:
                        // There is no 'implicit' pre-Run action for these actors...
                        break;
                    default:
                        throw new NotImplementedException();
                }
            }

            QFManager.Test.Run(test.Name);
        }
        private static void Unload(DataLayer.Test test, IEnumerable<Actor> actors)
        {
            foreach (Actor actor in actors)
            {
                switch (actor.Type)
                {
                    case ActorType.Port:
                        QFManager.Port.Close(actor.Name);
                        break;
                    case ActorType.Process:
                        QFManager.Process.Kill(actor.Name);
                        break;
                    case ActorType.Prompt:
                        break;
                    case ActorType.Recorder:
                        QFManager.Recorder.Close(actor.Name);
                        break;
                    default:
                        throw new NotImplementedException();
                }
            }

            try
            {
                QFManager.Test.End(test.Name);
                Debug.Print("<Event=Test successfully unloaded,\tTest=" + test.Name + ">");
                if (TestUnloaded != null)
                    TestUnloaded.BeginInvoke(null, new TestUnloadedEventArgs(test.Name), null, null);
            }
            catch (Exception ex)
            {
                QFManager.Test.Abort(test.Name, ex.Message);
            }
        }

        /// <summary>
        /// Abort the given Test and initiaite teardown
        /// </summary>
        /// <param name="test">Test to abort</param>
        /// <param name="reason">abort reason</param>
        public static void Abort(DataLayer.Test test, string reason)
        {
            QFManager.Test.Abort(test.Name, reason);
        }

        /// <summary>
        /// Orchestrates the setup, asynchronous execution and teardown of a Test in the TQF
        /// </summary>
        /// <param name="test">Test to run</param>
#if UUT_TEST == false
        public static void Run(DataLayer.Test test)
        {
            Run(test, null, string.Empty, string.Empty);
        }
#else
        public static void Run(DataLayer.Test test, UUTTestMode mode)
        {
            Run(test, null, mode);
        }
#endif

        /// <summary>
        /// Orchestrates the setup, asynchronous execution and teardown of a Test in the TQF
        /// </summary>
        /// <param name="test">Test to run</param>
        /// <param name="handlerTarget">Target for all ActiveObject events</param>
#if UUT_TEST == false
        public static void Run(DataLayer.Test test, IHandlerTarget handlerTarget)
        {
            Run(test, handlerTarget, string.Empty, string.Empty);
        }
#else
        public static void Run(DataLayer.Test test, IHandlerTarget handlerTarget, UUTTestMode mode)
        {
            Run(test, handlerTarget, string.Empty, string.Empty, mode);
        }
#endif

        /// <summary>
        /// Orchestrates the setup, asynchronous execution and teardown of a Test in the TQF
        /// </summary>
        /// <param name="test">Test to run</param>
        /// <param name="handlerTarget">Target for all ActiveObject events</param>
#if UUT_TEST == false
        public static void Run(DataLayer.Test test, IHandlerTarget handlerTarget, string configPath)
        {
            Run(test, handlerTarget, configPath, string.Empty);
        }
#else
        public static void Run(DataLayer.Test test, IHandlerTarget handlerTarget, string configPath, UUTTestMode mode)
        {
            Run(test, handlerTarget, configPath, string.Empty, mode);
        }
#endif

        /// <summary>
        /// Orchestrates the setup, asynchronous execution and teardown of a Test in the TQF
        /// </summary>
        /// <param name="test">Test to run</param>
        /// <param name="handlerTarget">Target for all ActiveObject events</param>
        /// <param name="path">path location of custom script binaries</param>
#if UUT_TEST == false
        public static void Run(DataLayer.Test test, IHandlerTarget handlerTarget, string configPath, string binaryPath)
#else
        public static void Run(DataLayer.Test test, IHandlerTarget handlerTarget, string configPath, string binaryPath, UUTTestMode mode)
#endif
        {
            if (test == null)
                throw new ArgumentNullException("test", "A valid test must be provided.");
            if ((test.Steps == null) || (test.Steps.Count == 0))
                throw new ArgumentNullException("test.Steps", "A test must contain one or more Steps.");

            IEnumerable<Actor> actors = test.Steps.Distinct(new StepComparer()).Select(s => s.Actor);
            if (actors == null)
                throw new InvalidOperationException("Test " + test.Name + " yielded no meaningful actions.");

            lock (thisLock)
            {
#if UUT_TEST == false
                Load(test, actors, handlerTarget, configPath);
#else
                Load(test, actors, handlerTarget, configPath, mode);
#endif
                Bind(test, binaryPath);
                Substitute(test);
                Run(test, actors);
                ThreadPool.RegisterWaitForSingleObject(testComplete,
                                                       (state, timedOut) =>
                                                       {
                                                           Unload(test, actors);
                                                       },
                                                       null, -1, true);
            }
        }
    }
}
