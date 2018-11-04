using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using qf4net;

namespace TruePosition.Test.QF
{
#if UUT_TEST
    public enum UUTTestMode
    {
        Port,
        Direct
    }
#endif

    /// <summary>
    /// Available signals within the Test QF
    /// </summary>
    public enum QFSignal : int
    {
        /// <summary>
        /// Published into the TQF from the outside world to run a Test
        /// </summary>
        TestRun = QSignals.UserSig,
        /// <summary>
        /// Published by an ActiveTest indicating a given step passed
        /// </summary>
        TestStep,
        /// <summary>
        /// Published by an ActiveTest indicating the result of a given Test (Passed, Failed, Aborted)
        /// </summary>
        TestResult,
        /// <summary>
        /// Published into the TQF from the outside world to abort a Test
        /// </summary>
        TestAbort,
        /// <summary>
        /// Published into the TQF from the outside world to request the opening of an associated port
        /// </summary>
        PortOpen,
        /// <summary>
        /// Published by an ActiveTest to transmit a message through an ActivePort
        /// </summary>
        PortTransmit,
        /// <summary>
        /// Published by an ActivePort indicating a message has been successfully transmitted
        /// </summary>
        PortTransmitted,
        /// <summary>
        /// Published by an ActivePort indicating a response has been received 
        /// </summary>
        PortResponse,
        /// <summary>
        /// Published into the TQF from the outside world to request the closing of an associated port
        /// </summary>
        PortClose,
        /// <summary>
        /// Published by an ActiveTest to request the launch of a Windows Process
        /// </summary>
        ProcessLaunch,
        /// <summary>
        /// Published by an ActiveTest to kill a Windows Process
        /// </summary>
        ProcessKill,
        /// <summary>
        /// Published by an ActiveProcess indicating the given process was successfully launched
        /// </summary>
        ProcessLaunched,
        /// <summary>
        /// Published by an ActiveProcess indicating the given process was successfully killed
        /// </summary>
        ProcessKilled,
        /// </summary>
        /// Published by an ActiveTest to request the launch of a Windows Process
        /// </summary>
        PromptShow,
        /// <summary>
        /// Published by an ActivePrompt indicating the given prompt was successfully shown
        /// </summary>
        PromptShown,
        /// <summary>
        /// Published by an ActivePrompt indicating the given prompt was successfully closed
        /// </summary>
        PromptClosed,
        /// <summary>
        /// Published by an ActivePrompt indicating a non-blocking prompt was shown and the test should continue
        /// </summary>
        PromptContinue,
        /// <summary>
        /// Published by an ActiveTest to request the opening of a Recorder
        /// </summary>
        RecorderOpen,
        /// <summary>
        /// Published by an ActiveTest to broadcast an entry to be recorded
        /// </summary>
        RecorderRecord,
        /// <summary>
        /// Published by an ActiveTest to close a Recorder
        /// </summary>
        RecorderClose,
        /// <summary>
        /// Published by an ActiveRecorder indicating a command dependent open request is pending
        /// </summary>
        RecorderOpenPending,
        /// <summary>
        /// Published by an ActiveRecorder indicating the given recorder was successfully opened
        /// </summary>
        RecorderOpened,
        /// <summary>
        /// Published by an Activerecorder indicating the given recorder was successfully closed
        /// </summary>
        RecorderClosed,
        /// <summary>
        /// Published by any ActiveObject that supports timeouts indicating a timeout has expired
        /// </summary>
        Timeout,
        /// <summary>
        /// Published by any ActiveOject indicating an error or exception has occurred
        /// </summary>
        Error,
        /// <summary>
        /// Maximum QFSignal
        /// </summary>
        MaxSignal
    };

    /// <summary>
    /// TQF signals used internally for self posting
    /// </summary>
    internal enum InternalSignal : int
    {
        /// <summary>
        /// ActivePort: Initiate port open
        /// </summary>
        PortOpening = QFSignal.MaxSignal,
        /// <summary>
        /// ActivePort: Initiate port close
        /// </summary>
        PortClosing,
        /// <summary>
        /// ActiveTest: Initiate Step operation
        /// </summary>
        TestStep,
        /// <summary>
        /// ActiveTest: Initiate Test passed operation
        /// </summary>
        TestPassed,
        /// <summary>
        /// ActiveTest: Initiate Test failed operation
        /// </summary>
        TestFailed,
        /// <summary>
        /// ActiveTest: Initiate Test aborted operation
        /// </summary>
        TestAborted,
        /// <summary>
        /// ActiveProcess: Initiate process launch
        /// </summary>
        ProcessLaunching,
        /// <summary>
        /// ActiveProcess: Initiate process kill
        /// </summary>
        ProcessKilling,
        /// <summary>
        /// ActivePrompt: Initiate prompt show
        /// </summary>
        PromptShowing,
        /// <summary>
        /// ActiveProcess: Initiate prompt closing
        /// </summary>
        PromptClosing,
        /// <summary>
        /// ActiveRecorder: Initiate recorder open
        /// </summary>
        RecorderOpening,
        /// <summary>
        /// ActiveRecorder: Initiate recorder close
        /// </summary>
        RecorderClosing
    };
}
