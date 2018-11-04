using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;

using qf4net;
using TruePosition.Test.IO;
using TruePosition.Test.DataLayer;
using TruePosition.Test.Process;
using TruePosition.Test.Prompt;
using TruePosition.Test.Recorder;

namespace TruePosition.Test.QF
{
    /// <summary>
    /// The application gateway into the TQF (Test Quantum Framework)
    /// </summary>
    public sealed class QFManager
    {
        private volatile int priority = 0;
        private static readonly QFManager manager = new QFManager();
        private Dictionary<string, ActivePort> activePorts = new Dictionary<string, ActivePort>();
        private Dictionary<string, ActiveProcess> activeProcesses = new Dictionary<string, ActiveProcess>();
        private Dictionary<string, ActivePrompt> activePrompts = new Dictionary<string, ActivePrompt>();
        private Dictionary<string, ActiveRecorder> activeRecorders = new Dictionary<string, ActiveRecorder>();
        private Dictionary<string, ActiveTest> activeTests = new Dictionary<string, ActiveTest>();

#if UUT_TEST
        private Dictionary<string, ActiveUUT> activeUUTs = new Dictionary<string, ActiveUUT>();
#endif

        static QFManager()
        {
            qf4net.QF.Instance.Initialize((int)QFSignal.MaxSignal - 1);
        }

        /// <summary>
        /// Operations that manage ActivePort objects within the TQF
        /// </summary>
        public sealed class Port
        {
            /// <summary>
            /// Creates an ActivePort within the TQF of the given PortType with the given name and default settings (9600 8N1)
            /// </summary>
            /// <param name="type">Type of underlying Port to associate with the ActivePort</param>
            /// <param name="name">Unique name for the ActivePort</param>
            public static void Create(PortType type, string name)
            {
                Create(PortFactory.Create(type, name));
            }
            /// <summary>
            /// Creates an ActivePort within the TQF of the given PortType and configuration provided by the given stream
            /// </summary>
            /// <param name="type">Type of ActivePort to create</param>
            /// <param name="stream">Configuration stream compatible with the underlying PortType</param>
            public static void Create(PortType type, Stream stream)
            {
                Create(PortFactory.Create(type, stream));
            }
            /// <summary>
            /// Creates an ActivePort within the TQF using an existing Port device
            /// </summary>
            /// <param name="port">An existing Port device</param>
            public static void Create(IPort port)
            {
                if (port == null)
                    throw new ArgumentNullException("port", "A valid port must be provided.");
                if ((string.IsNullOrEmpty(port.Name) || manager.activePorts.ContainsKey(port.Name)))
                    throw new ArgumentNullException("port.Name", "An ActivePort must have a valid, unique name.");

                manager.activePorts.Add(port.Name, new ActivePort(port));
                manager.activePorts[port.Name].Start(manager.priority++);

                ActivePort aPort = manager.activePorts[port.Name];
                while ((!aPort.IsInState(aPort.m_StateFaulted)) && (!aPort.IsInState(aPort.m_StateCreated)))
                    System.Threading.Thread.Sleep(1);
            }
            /// <summary>
            /// Open the underlying PortType associated with an ActivePort and prepare it for communications.
            /// </summary>
            /// <param name="name">ActivePort name</param>
            public static void Open(string name)
            {
                if (string.IsNullOrEmpty(name))
                    throw new ArgumentNullException("name", "A valid ActivePort name must be provided.");
                if (!manager.activePorts.ContainsKey(name))
                    throw new InvalidOperationException("An ActivePort must first be created.");
                if (manager.activePorts[name].State != PortState.Created)
                    throw new InvalidOperationException("The port cannot be opened. It is not in the Created state.");

                if (manager.activePorts[name].State == PortState.Created)
                {
                    qf4net.QF.Instance.Publish(new PortEvent(QFSignal.PortOpen, name));
                    ActivePort aPort = manager.activePorts[name];
                    while ((!aPort.IsInState(aPort.m_StateFaulted)) && (!aPort.IsInState(aPort.m_StateOpened)))
                        System.Threading.Thread.Sleep(1);

                    if (!aPort.IsInState(aPort.m_StateOpened))
                        throw new InvalidOperationException("The ActivePort '" + name + "' could not be opened. Please check the log for details.");
                }
            }
            /// <summary>
            /// Close the underlying PortType associated with an ActivePort and teardown communications.
            /// </summary>
            /// <param name="name">ActivePort name</param>
            public static void Close(string name)
            {
                if (string.IsNullOrEmpty(name))
                    throw new ArgumentNullException("name", "A valid ActivePort name must be provided.");
                if (!manager.activePorts.ContainsKey(name))
                    throw new InvalidOperationException("An ActivePort must first be created.");

                qf4net.QF.Instance.Publish(new PortEvent(QFSignal.PortClose, name));

                ActivePort aPort = manager.activePorts[name];
                while ((!aPort.IsInState(aPort.m_StateFaulted)) && (!aPort.IsInState(aPort.m_StateClosed)))
                    System.Threading.Thread.Sleep(1);
                manager.activePorts[name].Dispose();
                manager.activePorts.Remove(name);
            }
            /// <summary>
            /// Transmit a message using the underlying PortType of the associated ActivePort. 
            /// WARNING: Use at your own risk. Transmissions outside of a Test context will have undesirable side effects.
            /// </summary>
            /// <param name="name">ActivePort name</param>
            /// <param name="message">ASCII message to transmit</param>
            public static void Transmit(string name, string message)
            {
                if (string.IsNullOrEmpty(name))
                    throw new ArgumentNullException("name", "A valid ActivePort name must be provided.");
                if (!manager.activePorts.ContainsKey(name))
                    throw new InvalidOperationException("An ActivePort must first be created.");
                if (manager.activePorts[name].State != PortState.Opened)
                    throw new InvalidOperationException("Cannot transmit. The ActivePort has not been opened.");

                qf4net.QF.Instance.Publish(new PortEvent(QFSignal.PortTransmit, name, message));
            }
            /// <summary>
            /// Subscribe to the given ActivePort's Transmitted event.
            /// </summary>
            /// <param name="name">ActivePort name</param>
            /// <param name="handler">Transmitted event handler</param>
            public static void Transmitted(string name, EventHandler<PortMessageEventArgs> handler)
            {
                if (string.IsNullOrEmpty(name))
                    throw new ArgumentNullException("name", "A unique port name must be provided.");
                if (!manager.activePorts.ContainsKey(name))
                    throw new InvalidOperationException("An ActivePort must first be created.");

                manager.activePorts[name].SubscribeTransmitted(handler);
            }
            /// <summary>
            /// Subscribe to the given ActivePort's Receive event.
            /// </summary>
            /// <param name="name">ActivePort name</param>
            /// <param name="handler">Receive event handler</param>
            public static void Receive(string name, EventHandler<PortMessageEventArgs> handler)
            {
                if (string.IsNullOrEmpty(name))
                    throw new ArgumentNullException("name", "A unique port name must be provided.");
                if (!manager.activePorts.ContainsKey(name))
                    throw new InvalidOperationException("An ActivePort must first be created.");

                manager.activePorts[name].SubscribeReceive(handler);
            }
            /// <summary>
            /// Subscribe to the given ActivePort's Error event.
            /// </summary>
            /// <param name="name">ActivePort name</param>
            /// <param name="handler">Error event handler</param>
            public static void Error(string name, EventHandler<PortErrorEventArgs> handler)
            {
                if (string.IsNullOrEmpty(name))
                    throw new ArgumentNullException("name", "A unique port name must be provided.");
                if (!manager.activePorts.ContainsKey(name))
                    throw new InvalidOperationException("An ActivePort must first be created.");

                manager.activePorts[name].SubscribeError(handler);
            }
            /// <summary>
            /// Subscribe to the given ActivePort's ReceiveTimeout event.
            /// NOTE: An ActivePort's ReceiveTimeout is only used with PortMode.Solicited, which some PortTypes may not support.
            /// </summary>
            /// <param name="name">ActivePort name</param>
            /// <param name="handler">ReceiveTimeout event handler</param>
            public static void ReceiveTimeout(string name, EventHandler<PortTimeoutExpiredEventArgs> handler)
            {
                if (string.IsNullOrEmpty(name))
                    throw new ArgumentNullException("name", "A unique port name must be provided.");
                if (!manager.activePorts.ContainsKey(name))
                    throw new InvalidOperationException("An ActivePort must first be created.");

                manager.activePorts[name].SubscribeReceiveTimeout(handler);
            }
        }

        /// <summary>
        /// Operations that manage ActiveProcess objects within the TQF
        /// </summary>
        public sealed class Process
        {
            /// <summary>
            /// Creates an ActiveProcess within the TQF
            /// </summary>
            /// <param name="name">Unique name for the ActiveProcess</param>
            public static void Create(string name)
            {
                Create(ProcessFactory.Create(name));
            }
            /// <summary>
            /// Creates an ActiveProcess within the TQF
            /// </summary>
            /// <param name="stream">Configuration stream compatible with the WindowsProcess class</param>
            public static void Create(Stream stream)
            {
                Create(ProcessFactory.Create(stream));
            }
            private static void Create(IProcess process)
            {
                if (process == null)
                    throw new ArgumentNullException("process", "A valid process must be provided.");
                if ((string.IsNullOrEmpty(process.Name) || manager.activeProcesses.ContainsKey(process.Name)))
                    throw new ArgumentNullException("process.Name", "An ActiveProcess must have a valid, unique name.");

                manager.activeProcesses.Add(process.Name, new ActiveProcess(process));
                manager.activeProcesses[process.Name].Start(manager.priority++);

                ActiveProcess aProcess = manager.activeProcesses[process.Name];
                while ((!aProcess.IsInState(aProcess.m_StateFaulted)) && (!aProcess.IsInState(aProcess.m_StateCreated)))
                    System.Threading.Thread.Sleep(1);
            }
            /// <summary>
            /// Launch the given WindowsProcess 
            /// </summary>
            /// <param name="name">ActiveProcess name</param>
            public static void Launch(string name, string commandLine)
            {
                if (string.IsNullOrEmpty(name))
                    throw new ArgumentNullException("name", "A valid ActiveProcess name must be provided.");
                if (!manager.activeProcesses.ContainsKey(name))
                    throw new InvalidOperationException("An ActiveProcess must first be created.");

                qf4net.QF.Instance.Publish(new ProcessEvent(QFSignal.ProcessLaunch, name, commandLine));
                ActiveProcess aProcess = manager.activeProcesses[name];
                while ((!aProcess.IsInState(aProcess.m_StateFaulted)) && (!aProcess.IsInState(aProcess.m_StateLaunched)))
                    System.Threading.Thread.Sleep(1);
            }
            /// <summary>
            /// Kill the given Windows Process
            /// </summary>
            /// <param name="name">ActiveProcess name</param>
            public static void Kill(string name)
            {
                if (string.IsNullOrEmpty(name))
                    throw new ArgumentNullException("name", "A valid ActiveProcess name must be provided.");
                if (!manager.activeProcesses.ContainsKey(name))
                    throw new InvalidOperationException("An ActiveProcess must first be created.");

                qf4net.QF.Instance.Publish(new ProcessEvent(QFSignal.ProcessKill, name));

                ActiveProcess aProcess = manager.activeProcesses[name];
                while ((!aProcess.IsInState(aProcess.m_StateFaulted)) && (!aProcess.IsInState(aProcess.m_StateKilled)))
                    System.Threading.Thread.Sleep(1);
                manager.activeProcesses[name].Dispose();
                manager.activeProcesses.Remove(name);
            }
            /// <summary>
            /// Subscribe to the given ActiveProcess's Launched event.
            /// </summary>
            /// <param name="name">ActiveProcess name</param>
            /// <param name="handler">Launched event handler</param>
            public static void Launched(string name, EventHandler<ProcessEventArgs> handler)
            {
                if (string.IsNullOrEmpty(name))
                    throw new ArgumentNullException("name", "A unique process name must be provided.");
                if (!manager.activeProcesses.ContainsKey(name))
                    throw new InvalidOperationException("An ActiveProcess must first be created.");

                manager.activeProcesses[name].SubscribeLaunched(handler);
            }
            /// <summary>
            /// Subscribe to the given ActiveProcess's Killed event.
            /// </summary>
            /// <param name="name">ActiveProcess name</param>
            /// <param name="handler">Launched event handler</param>
            public static void Killed(string name, EventHandler<ProcessEventArgs> handler)
            {
                if (string.IsNullOrEmpty(name))
                    throw new ArgumentNullException("name", "A unique process name must be provided.");
                if (!manager.activeProcesses.ContainsKey(name))
                    throw new InvalidOperationException("An ActiveProcess must first be created.");

                manager.activeProcesses[name].SubscribeKilled(handler);
            }
            /// <summary>
            /// Subscribe to the given ActiveProcess's Error event.
            /// </summary>
            /// <param name="name">ActiveProcess name</param>
            /// <param name="handler">Error event handler</param>
            public static void Error(string name, EventHandler<ProcessErrorEventArgs> handler)
            {
                if (string.IsNullOrEmpty(name))
                    throw new ArgumentNullException("name", "A unique process name must be provided.");
                if (!manager.activeProcesses.ContainsKey(name))
                    throw new InvalidOperationException("An ActiveProcess must first be created.");

                manager.activeProcesses[name].SubscribeError(handler);
            }
            /// <summary>
            /// Subscribe to the given ActiveProcess's Timeout event.
            /// </summary>
            /// <param name="name">ActiveProcess name</param>
            /// <param name="handler">ReceiveTimeout event handler</param>
            public static void Timeout(string name, EventHandler<ProcessEventArgs> handler)
            {
                if (string.IsNullOrEmpty(name))
                    throw new ArgumentNullException("name", "A unique process name must be provided.");
                if (!manager.activeProcesses.ContainsKey(name))
                    throw new InvalidOperationException("An ActiveProcess must first be created.");

                manager.activeProcesses[name].SubscribeTimeout(handler);
            }
        }

        /// <summary>
        /// Operations that manage ActivePrompt objects within the TQF
        /// </summary>
        public sealed class Prompt
        {
            /// <summary>
            /// Creates an ActivePrompt within the TQF
            /// </summary>
            /// <param name="name">Unique name for the ActivePrompt</param>
            public static void Create(string name)
            {
                Create(PromptFactory.Create(name));
            }
            /// <summary>
            /// Creates an ActivePrompt within the TQF
            /// </summary>
            /// <param name="stream">Configuration stream compatible with the Prompt class</param>
            public static void Create(Stream stream)
            {
                Create(PromptFactory.Create(stream));
            }
            private static void Create(IPrompt prompt)
            {
                if (prompt == null)
                    throw new ArgumentNullException("prompt", "A valid prompt must be provided.");
                if ((string.IsNullOrEmpty(prompt.Name) || manager.activePrompts.ContainsKey(prompt.Name)))
                    throw new ArgumentNullException("prompt.Name", "An ActivePrompt must have a valid, unique name.");

                manager.activePrompts.Add(prompt.Name, new ActivePrompt(prompt));
                manager.activePrompts[prompt.Name].Start(manager.priority++);

                ActivePrompt aPrompt = manager.activePrompts[prompt.Name];
                while ((!aPrompt.IsInState(aPrompt.m_StateFaulted)) && (!aPrompt.IsInState(aPrompt.m_StateCreated)))
                    System.Threading.Thread.Sleep(1);
            }
            /// <summary>
            /// Show the given Prompt 
            /// </summary>
            /// <param name="name">ActivePrompt name</param>
            public static void Show(string name, string text, int timeout)
            {
                if (string.IsNullOrEmpty(name))
                    throw new ArgumentNullException("name", "A valid ActivePrompt name must be provided.");
                if (!manager.activePrompts.ContainsKey(name))
                    throw new InvalidOperationException("An ActivePrompt must first be created.");

                qf4net.QF.Instance.Publish(new PromptEvent(QFSignal.PromptShow, name, text, timeout));
                ActivePrompt aPrompt = manager.activePrompts[name];
                while ((!aPrompt.IsInState(aPrompt.m_StateFaulted)) && (!aPrompt.IsInState(aPrompt.m_StateShown)))
                    System.Threading.Thread.Sleep(1);
            }
            /// <summary>
            /// Subscribe to the given ActivePrompt's Launched event.
            /// </summary>
            /// <param name="name">ActivePrompt name</param>
            /// <param name="handler">Launched event handler</param>
            public static void Showing(string name, PromptShowingDelegate handler)
            {
                if (string.IsNullOrEmpty(name))
                    throw new ArgumentNullException("name", "A unique prompt name must be provided.");
                if (!manager.activePrompts.ContainsKey(name))
                    throw new InvalidOperationException("An ActivePrompt must first be created.");

                manager.activePrompts[name].SubscribeShowing(handler);
            }
            /// <summary>
            /// Subscribe to the given ActivePrompt's Killed event.
            /// </summary>
            /// <param name="name">ActivePrompt name</param>
            /// <param name="handler">Launched event handler</param>
            public static void Closed(string name, EventHandler<PromptEventArgs> handler)
            {
                if (string.IsNullOrEmpty(name))
                    throw new ArgumentNullException("name", "A unique prompt name must be provided.");
                if (!manager.activePrompts.ContainsKey(name))
                    throw new InvalidOperationException("An ActivePrompt must first be created.");

                manager.activePrompts[name].SubscribeClosed(handler);
            }
            /// <summary>
            /// Subscribe to the given ActivePrompt's Error event.
            /// </summary>
            /// <param name="name">ActivePrompt name</param>
            /// <param name="handler">Error event handler</param>
            public static void Error(string name, EventHandler<PromptErrorEventArgs> handler)
            {
                if (string.IsNullOrEmpty(name))
                    throw new ArgumentNullException("name", "A unique prompt name must be provided.");
                if (!manager.activePrompts.ContainsKey(name))
                    throw new InvalidOperationException("An ActivePrompt must first be created.");

                manager.activePrompts[name].SubscribeError(handler);
            }
        }

        /// <summary>
        /// Operations that manage ActiveProcess objects within the TQF
        /// </summary>
        public sealed class Recorder
        {
            /// <summary>
            /// Creates an ActiveRecorder within the TQF
            /// </summary>
            /// <param name="name">Unique name for the ActiveRecorder</param>
            public static void Create(RecorderType type, string name)
            {
                Create(RecorderFactory.Create(type, name));
            }
            private static void Create(IRecorder recorder)
            {
                if (recorder == null)
                    throw new ArgumentNullException("recorder", "A valid recorder must be provided.");
                if ((string.IsNullOrEmpty(recorder.Name) || manager.activeRecorders.ContainsKey(recorder.Name)))
                    throw new ArgumentNullException("recorder.Name", "An ActiveRecorder must have a valid, unique name.");

                manager.activeRecorders.Add(recorder.Name, new ActiveRecorder(recorder));
                manager.activeRecorders[recorder.Name].Start(manager.priority++);

                ActiveRecorder aRecorder = manager.activeRecorders[recorder.Name];
                while ((!aRecorder.IsInState(aRecorder.m_StateFaulted)) && (!aRecorder.IsInState(aRecorder.m_StateCreated)))
                    System.Threading.Thread.Sleep(1);
            }
            /// <summary>
            /// Open the given Recorder 
            /// </summary>
            /// <param name="name">ActiveRecorder name</param>
            public static void Open(string name, string location, IEnumerable<string> responseFilterExpressions)
            {
                if (string.IsNullOrEmpty(name))
                    throw new ArgumentNullException("name", "A valid ActiveRecorder name must be provided.");
                if (!manager.activeRecorders.ContainsKey(name))
                    throw new InvalidOperationException("An ActiveRecorder must first be created.");

                qf4net.QF.Instance.Publish(new RecorderEvent(QFSignal.RecorderOpen, name, location, null, EntryStyle.Single, null, null, responseFilterExpressions));
                ActiveRecorder aRecorder = manager.activeRecorders[name];
                while ((!aRecorder.IsInState(aRecorder.m_StateFaulted)) && (!aRecorder.IsInState(aRecorder.m_StateOpened)))
                    System.Threading.Thread.Sleep(1);
            }
            /// <summary>
            /// Broadcast an entry to all Recorders
            /// </summary>
            /// <param name="name">ActiveRecorder name</param>
            public static void Record(string entry)
            {
                qf4net.QF.Instance.Publish(new RecorderEvent(QFSignal.RecorderRecord, "User Initiated", 0, entry));
            }
            /// <summary>
            /// Close the given Recorder
            /// </summary>
            /// <param name="name">ActiveRecorder name</param>
            public static void Close(string name)
            {
                if (string.IsNullOrEmpty(name))
                    throw new ArgumentNullException("name", "A valid ActiveRecorder name must be provided.");
                if (!manager.activeRecorders.ContainsKey(name))
                    throw new InvalidOperationException("An ActiveRecorder must first be created.");

                qf4net.QF.Instance.Publish(new RecorderEvent(QFSignal.RecorderClose, name, null));

                ActiveRecorder aRecorder = manager.activeRecorders[name];
                while ((!aRecorder.IsInState(aRecorder.m_StateFaulted)) && (!aRecorder.IsInState(aRecorder.m_StateClosed)))
                    System.Threading.Thread.Sleep(1);
                manager.activeRecorders[name].Dispose();
                manager.activeRecorders.Remove(name);
            }
            /// <summary>
            /// Subscribe to the given ActiveRecorder's Opening event.
            /// </summary>
            /// <param name="name">ActiveRecorder name</param>
            /// <param name="handler">Opening event handler</param>
            public static void Opening(string name, RecorderOpeningDelegate handler)
            {
                if (string.IsNullOrEmpty(name))
                    throw new ArgumentNullException("name", "A unique recorder name must be provided.");
                if (!manager.activeRecorders.ContainsKey(name))
                    throw new InvalidOperationException("An ActiveRecorder must first be created.");

                manager.activeRecorders[name].SubscribeOpening(handler);
            }
            /// <summary>
            /// Subscribe to the given ActiveRecorder's Open event.
            /// </summary>
            /// <param name="name">ActiveRecorder name</param>
            /// <param name="handler">Opened event handler</param>
            public static void Opened(string name, EventHandler<RecorderEventArgs> handler)
            {
                if (string.IsNullOrEmpty(name))
                    throw new ArgumentNullException("name", "A unique recorder name must be provided.");
                if (!manager.activeRecorders.ContainsKey(name))
                    throw new InvalidOperationException("An ActiveRecorder must first be created.");

                manager.activeRecorders[name].SubscribeOpened(handler);
            }
            /// <summary>
            /// Subscribe to the given ActiveRecorder's Recording event.
            /// </summary>
            /// <param name="name">ActiveRecorder name</param>
            /// <param name="handler">Recording event handler</param>
            public static void Recording(string name, RecorderRecordingDelegate handler)
            {
                if (string.IsNullOrEmpty(name))
                    throw new ArgumentNullException("name", "A unique recorder name must be provided.");
                if (!manager.activeRecorders.ContainsKey(name))
                    throw new InvalidOperationException("An ActiveRecorder must first be created.");

                manager.activeRecorders[name].SubscribeRecording(handler);
            }
            /// <summary>
            /// Subscribe to the given ActiveRecorder's Close event.
            /// </summary>
            /// <param name="name">ActiveRecorder name</param>
            /// <param name="handler">Closed event handler</param>
            public static void Closed(string name, EventHandler<RecorderEventArgs> handler)
            {
                if (string.IsNullOrEmpty(name))
                    throw new ArgumentNullException("name", "A unique recorder name must be provided.");
                if (!manager.activeRecorders.ContainsKey(name))
                    throw new InvalidOperationException("An ActiveRecorder must first be created.");

                manager.activeRecorders[name].SubscribeClosed(handler);
            }
            /// <summary>
            /// Subscribe to the given ActiveRecorder's Error event.
            /// </summary>
            /// <param name="name">ActiveRecorder name</param>
            /// <param name="handler">Error event handler</param>
            public static void Error(string name, EventHandler<RecorderErrorEventArgs> handler)
            {
                if (string.IsNullOrEmpty(name))
                    throw new ArgumentNullException("name", "A unique recorder name must be provided.");
                if (!manager.activeRecorders.ContainsKey(name))
                    throw new InvalidOperationException("An ActiveRecorder must first be created.");

                manager.activeRecorders[name].SubscribeError(handler);
            }
        }


        public sealed class Test
        {
#if UUT_TEST == false
            public static void Load(DataLayer.Test test)
#else
            public static void Load(DataLayer.Test test)
            {
                Load(test, UUTTestMode.Port);
            }
            public static void Load(DataLayer.Test test, UUTTestMode mode)
#endif
            {
                if (test == null)
                    throw new ArgumentNullException("test", "A valid Test must be provided.");
                if ((string.IsNullOrEmpty(test.Name) || manager.activeTests.ContainsKey(test.Name)))
                    throw new ArgumentNullException("Test.Name", "An ActiveTest must have a unique name.");

#if UUT_TEST == false
                manager.activeTests.Add(test.Name, new ActiveTest(test));
#else
                manager.activeTests.Add(test.Name, new ActiveTest(test, mode));
#endif
                manager.activeTests[test.Name].Start(manager.priority++);

                ActiveTest aTest = manager.activeTests[test.Name];
                while ((!aTest.IsInState(aTest.m_StateEnded)) && (!aTest.IsInState(aTest.m_StateLoaded)))
                    System.Threading.Thread.Sleep(1);
            }
            public static void Run(string name)
            {
                ActiveTest aTest = manager.activeTests[name];

                if (string.IsNullOrEmpty(name))
                    throw new ArgumentNullException("name", "A unique Test name must be provided.");
                if (!manager.activeTests.ContainsKey(name))
                    throw new InvalidOperationException("An ActiveTest must first be loaded.");
                if (!aTest.IsInState(aTest.m_StateLoaded))
                    throw new InvalidOperationException("The Test cannot be ran. It has not been successfully loaded.");

                if (aTest.IsInState(aTest.m_StateLoaded))
                {
                    qf4net.QF.Instance.Publish(new TestEvent(QFSignal.TestRun, name));
                    while ((!aTest.IsInState(aTest.m_StateEnded)) && (!aTest.IsInState(aTest.m_StateRunning)))
                        System.Threading.Thread.Sleep(1);
                }
            }
            public static void Abort(string name, string reason)
            {
                if (string.IsNullOrEmpty(name))
                    throw new ArgumentNullException("name", "A unique Test name must be provided.");
                if (string.IsNullOrEmpty(reason))
                    throw new ArgumentNullException("reason", "An abort reason must be provided.");
                if (!manager.activeTests.ContainsKey(name))
                    throw new InvalidOperationException("An ActiveTest must first be loaded.");

                qf4net.QF.Instance.Publish(new TestEvent(QFSignal.TestAbort, name, reason));

                ActiveTest aTest = manager.activeTests[name];
                while (!aTest.IsInState(aTest.m_StateEnded))
                        System.Threading.Thread.Sleep(1);

                manager.activeTests[name].Dispose();
                manager.activeTests.Remove(name);
            }
            public static void End(string name)
            {
                if (string.IsNullOrEmpty(name))
                    throw new ArgumentNullException("name", "A unique Test name must be provided.");
                if (!manager.activeTests.ContainsKey(name))
                    throw new InvalidOperationException("An ActiveTest must first be loaded.");

                ActiveTest aTest = manager.activeTests[name];
                if (!aTest.IsInState(aTest.m_StateEnded))
                    throw new InvalidOperationException("ActiveTest cannot be ended. It is not in the ended state. Use Abort instead.");

                manager.activeTests[name].Dispose();
                manager.activeTests.Remove(name);
            }
            
            public static void Stepping(string name, TestSteppingDelegate handler)
            {
                if (string.IsNullOrEmpty(name))
                    throw new ArgumentNullException("name", "A unique Test name must be provided.");
                if (!manager.activeTests.ContainsKey(name))
                    throw new InvalidOperationException("An ActiveTest must first be loaded.");

                manager.activeTests[name].SubscribeToTestStepping(handler);
            }
            public static void Error(string name, EventHandler<TestErrorEventArgs> handler)
            {
                if (string.IsNullOrEmpty(name))
                    throw new ArgumentNullException("name", "A unique Test name must be provided.");
                if (!manager.activeTests.ContainsKey(name))
                    throw new InvalidOperationException("An ActiveTest must first be loaded.");

                manager.activeTests[name].SubscribeToTestError(handler);
            }
            public static void Stepped(string name, EventHandler<TestSteppedEventArgs> handler)
            {
                if (string.IsNullOrEmpty(name))
                    throw new ArgumentNullException("name", "A unique Test name must be provided.");
                if (!manager.activeTests.ContainsKey(name))
                    throw new InvalidOperationException("An ActiveTest must first be loaded.");

                manager.activeTests[name].SubscribeToTestStepped(handler);
            }
            public static void Passed(string name, EventHandler<TestPassedEventArgs> handler)
            {
                if (string.IsNullOrEmpty(name))
                    throw new ArgumentNullException("name", "A unique Test name must be provided.");
                if (!manager.activeTests.ContainsKey(name))
                    throw new InvalidOperationException("An ActiveTest must first be loaded.");

                manager.activeTests[name].SubscribeToTestPassed(handler);
            }
            public static void Failed(string name, EventHandler<TestFailedEventArgs> handler)
            {
                if (string.IsNullOrEmpty(name))
                    throw new ArgumentNullException("name", "A unique Test name must be provided.");
                if (!manager.activeTests.ContainsKey(name))
                    throw new InvalidOperationException("An ActiveTest must first be loaded.");

                manager.activeTests[name].SubscribeToTestFailed(handler);
            }
            public static void Aborted(string name, EventHandler<TestAbortedEventArgs> handler)
            {
                if (string.IsNullOrEmpty(name))
                    throw new ArgumentNullException("name", "A unique Test name must be provided.");
                if (!manager.activeTests.ContainsKey(name))
                    throw new InvalidOperationException("An ActiveTest must first be loaded.");

                manager.activeTests[name].SubscribeToTestAborted(handler);
            }
        }

#if UUT_TEST
        public sealed class UUT
        {
            public static void Load(string uutName, string responseFilePath, DataLayer.Test test, UUTTestMode mode)
            {
                if (test == null)
                    throw new ArgumentNullException("test", "A valid Test must be provided.");
                if ((string.IsNullOrEmpty(uutName) || manager.activeUUTs.ContainsKey(uutName)))
                    throw new ArgumentNullException("Test.Name", "An ActiveUUT must have a unique name.");

                manager.activeUUTs.Add(uutName, new ActiveUUT(uutName, responseFilePath, test, mode));
                manager.activeUUTs[uutName].Start(manager.priority++);

                ActiveUUT aUUT = manager.activeUUTs[uutName];
                while ((!aUUT.IsInState(aUUT.m_StateEnded)) && (!aUUT.IsInState(aUUT.m_StateLoaded)))
                    System.Threading.Thread.Sleep(1);
            }
            public static void Run(string uutName)
            {
                ActiveUUT aUUT = manager.activeUUTs[uutName];

                if (string.IsNullOrEmpty(uutName))
                    throw new ArgumentNullException("uutName", "A unique Test name must be provided.");
                if (!manager.activeUUTs.ContainsKey(uutName))
                    throw new InvalidOperationException("An activeUUT must first be loaded.");
                if (!aUUT.IsInState(aUUT.m_StateLoaded))
                    throw new InvalidOperationException("The Test cannot be ran. It has not been loaded.");

                if (manager.activeUUTs[uutName].CurrentStateName == "ActiveTest_Loaded")
                {
                    qf4net.QF.Instance.Publish(new TestEvent(QFSignal.TestRun, uutName));
                    while ((!aUUT.IsInState(aUUT.m_StateEnded)) && (!aUUT.IsInState(aUUT.m_StateRunning)))
                        System.Threading.Thread.Sleep(1);
                }
            }
            public static void Abort(string uutName, string reason)
            {
                if (string.IsNullOrEmpty(uutName))
                    throw new ArgumentNullException("uutName", "A unique UUT name must be provided.");
                if (string.IsNullOrEmpty(reason))
                    throw new ArgumentNullException("reason", "An abort reason must be provided.");
                if (!manager.activeUUTs.ContainsKey(uutName))
                    throw new InvalidOperationException("An ActiveUUT must first be loaded.");

                qf4net.QF.Instance.Publish(new TestEvent(QFSignal.TestAbort, uutName, reason));

                ActiveUUT aUUT = manager.activeUUTs[uutName];
                while (!aUUT.IsInState(aUUT.m_StateEnded))
                    System.Threading.Thread.Sleep(1);

                manager.activeUUTs[uutName].Dispose();
                manager.activeUUTs.Remove(uutName);
            }
            public static void End(string name)
            {
                if (string.IsNullOrEmpty(name))
                    throw new ArgumentNullException("uutName", "A unique Test name must be provided.");
                if (!manager.activeUUTs.ContainsKey(name))
                    throw new InvalidOperationException("An ActiveUUT must first be loaded.");

                ActiveUUT aUUT = manager.activeUUTs[name];
                if (!aUUT.IsInState(aUUT.m_StateEnded))
                    throw new InvalidOperationException("ActiveUUT cannot be ended. It is not in the ended state. Use Abort instead.");

                manager.activeUUTs[name].Dispose();
                manager.activeUUTs.Remove(name);
            }
            public static void Failed(string uutName, EventHandler<TestFailedEventArgs> handler)
            {
                if (string.IsNullOrEmpty(uutName))
                    throw new ArgumentNullException("uutName", "A unique Test name must be provided.");
                if (!manager.activeUUTs.ContainsKey(uutName))
                    throw new InvalidOperationException("An activeUUT must first be loaded.");

                manager.activeUUTs[uutName].SubscribeToTestFailed(handler);
            }
            public static void Passed(string uutName, EventHandler<TestPassedEventArgs> handler)
            {
                if (string.IsNullOrEmpty(uutName))
                    throw new ArgumentNullException("uutName", "A unique Test name must be provided.");
                if (!manager.activeUUTs.ContainsKey(uutName))
                    throw new InvalidOperationException("An activeUUT must first be loaded.");

                manager.activeUUTs[uutName].SubscribeToTestPassed(handler);
            }
        }
#endif
    }
}
