using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

using qf4net;
using TruePosition.Test.DataLayer;
using System.Diagnostics;

namespace TruePosition.Test.QF
{
    public delegate void TestSteppingDelegate(string testName, Step step, ref string augmentedCommand);

    internal class ActiveTest : QActive
    {
		#region Boiler plate static stuff

		private static new TransitionChainStore s_TransitionChainStore = 
			new TransitionChainStore(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        static ActiveTest()
		{
			s_TransitionChainStore.ShrinkToActualSize();
		}

		/// <summary>
		/// Getter for an optional <see cref="TransitionChainStore"/> that can hold cached
		/// <see cref="TransitionChain"/> objects that are used to optimize static transitions.
		/// </summary>
		protected override TransitionChainStore TransChainStore
		{
			get { return s_TransitionChainStore; }
		}

		#endregion

        public string Name { get; protected set; }
        protected DataLayer.Test Test { get; set; }
        protected int CurrentStep { get; set; }
        protected int Retries { get; set; }

        private string ResponseBuffer { get; set; }
        private QTimer TimeoutTimer{ get; set; }

#if UUT_TEST
        protected UUTTestMode Mode { get; set; }
#endif

        protected internal QState m_StateLoaded;
        protected internal QState m_StateRunning;
        protected internal QState m_StateStepping;
        protected internal QState m_StatePassed;
        protected internal QState m_StateFailed;
        protected internal QState m_StateAborted;
        protected internal QState m_StateEnded;

        protected event EventHandler<TestPassedEventArgs> TestPassed;
        protected event EventHandler<TestErrorEventArgs> TestError;
        protected event EventHandler<TestFailedEventArgs> TestFailed;
        protected event EventHandler<TestAbortedEventArgs> TestAborted;
        protected event TestSteppingDelegate TestStepping;
        protected event EventHandler<TestSteppedEventArgs> TestStepped;

        private void Initialize(DataLayer.Test test)
        {
            Name = test.Name;
            Test = test;

            m_StateLoaded = new QState(ActiveTest_Loaded);
            m_StateRunning = new QState(ActiveTest_Running);
            m_StateStepping = new QState(ActiveTest_Stepping);
            m_StatePassed = new QState(ActiveTest_Passed);
            m_StateFailed = new QState(ActiveTest_Failed);
            m_StateAborted = new QState(ActiveTest_Aborted);
            m_StateEnded = new QState(ActiveTest_Ended);

            TimeoutTimer = new QTimer(this);
        }

#if UUT_TEST == false
        public ActiveTest(DataLayer.Test test) : base()
        {
            Initialize(test);
        }
#else
        public ActiveTest(DataLayer.Test test) : this(test, UUTTestMode.Port) { }
        public ActiveTest(DataLayer.Test test, UUTTestMode mode)
            : base()
        {
            Initialize(test);
            Mode = mode;
        }
#endif

        protected override void InitializeStateMachine()
        {
            Thread.CurrentThread.Name = this.ToString();
            qf4net.QF.Instance.Subscribe(this, (int)QFSignal.TestRun);
            qf4net.QF.Instance.Subscribe(this, (int)QFSignal.TestAbort);

            qf4net.QF.Instance.Subscribe(this, (int)QFSignal.PortResponse);
            qf4net.QF.Instance.Subscribe(this, (int)QFSignal.ProcessLaunched);
            qf4net.QF.Instance.Subscribe(this, (int)QFSignal.PromptClosed);
            qf4net.QF.Instance.Subscribe(this, (int)QFSignal.PromptContinue);
            qf4net.QF.Instance.Subscribe(this, (int)QFSignal.RecorderOpenPending);
            qf4net.QF.Instance.Subscribe(this, (int)QFSignal.RecorderOpened);
            qf4net.QF.Instance.Subscribe(this, (int)QFSignal.Timeout);
            qf4net.QF.Instance.Subscribe(this, (int)QFSignal.Error);
            InitializeState(m_StateLoaded);
        }

        public override string ToString()
        {
            return "ActiveTest " + Name;
        }

        public override void Dispose()
        {
            if (TestPassed != null)
                Delegate.RemoveAll(TestPassed, TestPassed);
            if (TestError != null)
                Delegate.RemoveAll(TestError, TestError);
            if (TestFailed != null)
                Delegate.RemoveAll(TestFailed, TestFailed);
            if (TestFailed != null)
                Delegate.RemoveAll(TestAborted, TestAborted);
            if (TestStepping != null)
                Delegate.RemoveAll(TestStepping, TestStepping);
            if (TestStepped != null)
                Delegate.RemoveAll(TestStepped, TestStepped);

            base.Dispose();
        }

        public void SubscribeToTestPassed(EventHandler<TestPassedEventArgs> handler)
        {
            TestPassed += handler;
        }
        public void SubscribeToTestError(EventHandler<TestErrorEventArgs> handler)
        {
            TestError += handler;
        }
        public void SubscribeToTestFailed(EventHandler<TestFailedEventArgs> handler)
        {
            TestFailed += handler;
        }
        public void SubscribeToTestAborted(EventHandler<TestAbortedEventArgs> handler)
        {
            TestAborted += handler;
        }
        public void SubscribeToTestStepping(TestSteppingDelegate handler)
        {
            TestStepping += handler;
        }
        public void SubscribeToTestStepped(EventHandler<TestSteppedEventArgs> handler)
        {
            TestStepped += handler;
        }

        protected void TransitionToRunning()
        {
            TransitionTo(m_StateRunning, s_TranIdx_Stepping_Running);
            PostFIFO(new TestEvent(InternalSignal.TestStep, Name));
        }
        protected virtual void Run(TestEvent tEvent)
        {
            if (tEvent.Name == Name)
            {
                Retries = 0;
                CurrentStep = 0;
                TransitionTo(m_StateRunning, s_TranIdx_Loaded_Running);
                PostFIFO(new TestEvent(InternalSignal.TestStep, tEvent));
            }
        }
        protected virtual bool Request(string augmentedCommand)
        {
            bool complete = true;

            switch (Test.Steps[CurrentStep].Actor.Type)
            {
                case ActorType.Port:
#if UUT_TEST == false
                    qf4net.QF.Instance.Publish(new PortEvent(QFSignal.PortTransmit,
                                                             Test.Steps[CurrentStep].Actor.Name,
                                                             augmentedCommand));
#else
                    switch (Mode)
                    {
                        case UUTTestMode.Port:
                            qf4net.QF.Instance.Publish(new PortEvent(QFSignal.PortTransmit,
                                                                     Test.Steps[CurrentStep].Actor.Name,
                                                                     augmentedCommand));
                            break;

                        case UUTTestMode.Direct:
                            qf4net.QF.Instance.Publish(new PortEvent(QFSignal.PortResponse,
                                                                     Test.Steps[CurrentStep].Actor.Name,
                                                                     augmentedCommand));
                            break;
                    }
#endif
                    break;
                case ActorType.Process:
                    qf4net.QF.Instance.Publish(new ProcessEvent(QFSignal.ProcessLaunch, Test.Steps[CurrentStep].Actor.Name, augmentedCommand));
                    break;
                case ActorType.Prompt:
                    complete = false;
                    qf4net.QF.Instance.Publish(new PromptEvent(QFSignal.PromptShow, Test.Steps[CurrentStep].Actor.Name, augmentedCommand, Test.Steps[CurrentStep].Timeout));
                    break;
                case ActorType.Recorder:
                    IEnumerable<string> commandFilter;
                    IEnumerable<string> responseFilter;
                    try
                    {
                        commandFilter = Test.Steps[CurrentStep].Response.Elements[0].ExpectedResponses[0].Expressions.Select(ex => ex.Evaluator.RawExpression);
                    }
                    catch
                    {
                        commandFilter = null;
                    }
                    try
                    {
                        responseFilter = Test.Steps[CurrentStep].Response.Elements[1].ExpectedResponses[0].Expressions.Select(ex => ex.Evaluator.RawExpression);
                    }
                    catch
                    {
                        responseFilter = null;
                    }

                    qf4net.QF.Instance.Publish(new RecorderEvent(QFSignal.RecorderOpen, Test.Steps[CurrentStep].Actor.Name, augmentedCommand, Test.Steps[CurrentStep].Actor.EntryFormat, Test.Steps[CurrentStep].Actor.Style, Test.Steps[CurrentStep].Actor.Command.Descendants("Value"), commandFilter, responseFilter));
                    break;
            }
            qf4net.QF.Instance.Publish(new RecorderEvent(QFSignal.RecorderRecord, Name, CurrentStep + 1, augmentedCommand));

            return complete;
        }
        protected virtual void Step(TestEvent tEvent, int retries)
        {
            if (tEvent.Name == Name)
            {
                Retries = retries;
                ResponseBuffer = string.Empty;

                string augmentedCommand = Test.Steps[CurrentStep].Actor.Action;
                if (TestStepping != null)
                {
                    try
                    {
                        TestStepping(Name, Test.Steps[CurrentStep], ref augmentedCommand);
                        if (string.IsNullOrEmpty(augmentedCommand))
                            augmentedCommand = Test.Steps[CurrentStep].Actor.Action;
                    }
                    catch (Exception ex)
                    {
                        TransitionToFailed(ex.Message);
                        return;
                    }
                }

                TransitionTo(m_StateStepping, s_TranIdx_Running_Stepping);
                bool complete = Request(augmentedCommand);

                // DESIGN NOTE:
                // If the Actor explicitly controls step completion (such as Prompt), ignore timeout settings
                if (complete)
                {
                    // DESIGN NOTE:
                    // If no timeout has been specified, continue directly regardless of whether response processing has been requested
                    if (Test.Steps[CurrentStep].Timeout == 0)
                        CompleteStep();
                    else
                        TimeoutTimer.FireIn(new TimeSpan(0, 0, Test.Steps[CurrentStep].Timeout), new TestEvent(QFSignal.Timeout, Name));
                }
            }
        }
        protected void TransitionToAborted(TestEvent tEvent)
        {
            if (tEvent.Name == Name)
            {
                TransitionTo(m_StateAborted, s_TranIdx_Running_Aborted);
                PostFIFO(new TestEvent(InternalSignal.TestAborted, tEvent));
            }
        }
        protected virtual void Aborted(TestEvent tEvent)
        {
            if (tEvent.Name == Name)
            {
                TimeoutTimer.Disarm();

                qf4net.QF.Instance.Publish(new TestEvent(QFSignal.TestResult, tEvent.Name, tEvent.Reason, TestState.Aborted));
                if (TestAborted != null)
                    TestAborted(null, new TestAbortedEventArgs(tEvent.Name, tEvent.Reason));

                TransitionTo(m_StateEnded, s_TranIdx_Aborted_Ended);
            }
        }

        protected void TransitionToPassed()
        {
            TransitionTo(m_StatePassed, s_TranIdx_Running_Passed);
            PostFIFO(new TestEvent(InternalSignal.TestPassed, Name));
        }
        protected virtual void Passed(TestEvent tEvent)
        {
            if (tEvent.Name == Name)
            {
                TimeoutTimer.Disarm();

                qf4net.QF.Instance.Publish(new TestEvent(QFSignal.TestResult, tEvent.Name, TestState.Passed));
                if (TestPassed != null)
                    TestPassed(null, new TestPassedEventArgs(tEvent.Name));

                TransitionTo(m_StateEnded, s_TranIdx_Passed_Ended);
            }
        }

        protected void TransitionToFailed(string message)
        {
            TransitionTo(m_StateFailed, s_TranIdx_Running_Failed);
            PostFIFO(new TestEvent(InternalSignal.TestFailed, new TestEvent(QFSignal.TestResult, Name, message, Test.Steps[CurrentStep], TestState.Failed)));
        }
        protected virtual void Failed(TestEvent tEvent)
        {
            if (tEvent.Name == Name)
            {
                TimeoutTimer.Disarm();

                if (Test.Steps[CurrentStep].ContinueOnError)
                {
                    qf4net.QF.Instance.Publish(new TestEvent(QFSignal.Error, tEvent.Name, tEvent.Reason, tEvent.Step));
                    if (TestError != null)
                        TestError(null, new TestErrorEventArgs(tEvent.Name, tEvent.Reason, tEvent.Step));

                    CompleteStep();
                }
                else
                {
                    qf4net.QF.Instance.Publish(new TestEvent(QFSignal.TestResult, tEvent.Name, tEvent.Reason, tEvent.Step, tEvent.State));
                    if (TestFailed != null)
                        TestFailed(null, new TestFailedEventArgs(tEvent.Name, tEvent.Reason, tEvent.Step));

                    TransitionTo(m_StateEnded, s_TranIdx_Failed_Ended);
                }
            }
        }

        protected virtual bool ProcessPortResponse()
        {
            bool complete = false;

            TimeoutTimer.Disarm();

            if (Test.Steps[CurrentStep].Response != null)
            {
                if (!string.IsNullOrEmpty(Test.Steps[CurrentStep].Response.Header))
                {
                    int index = ResponseBuffer.LastIndexOf(Test.Steps[CurrentStep].Response.Header);
                    if (index >= 0)
                        ResponseBuffer = ResponseBuffer.Substring(index);
                }
            }

            if (!string.IsNullOrEmpty(ResponseBuffer))
                qf4net.QF.Instance.Publish(new RecorderEvent(QFSignal.RecorderRecord, Name, CurrentStep + 1, ResponseBuffer));

            ResponseProcessor.Process(Test.Name, Test.Steps[CurrentStep], ResponseBuffer);
            ResponseBuffer = string.Empty;
            complete = true;

            return complete;
        }
        protected virtual bool PortResponse(PortEvent pEvent)
        {
            bool complete = false;

            if (string.IsNullOrEmpty(pEvent.Message))
                return false;

#if UUT_TEST
            // In test mode, ignore the command being looped back...
            if ((pEvent.Message == Test.Steps[CurrentStep].Actor.Action) ||
                ((pEvent.Message.IndexOf(' ') >= 0) && (Test.Steps[CurrentStep].Actor.Action == pEvent.Message.Substring(0, pEvent.Message.IndexOf(' ')).Trim())))
                return false;
#endif

            string delimiter = "\r\n";
            string trailer = string.Empty;
            if (Test.Steps[CurrentStep].Response != null)
            {
                delimiter = Test.Steps[CurrentStep].Response.Delimiter;
                trailer = Test.Steps[CurrentStep].Response.Trailer;
            }

            bool processResponse = false;
            // Put response element delimiter back if it was stripped during port processing...
            string response = pEvent.Message.EndsWith(delimiter) ? pEvent.Message : pEvent.Message + delimiter;

            // Only accumulate response elements if a trailer has been defined...
            if ((!Test.Steps[CurrentStep].CompleteOnTimeout) && (string.IsNullOrEmpty(trailer)))
            {
                ResponseBuffer = response;
                processResponse = true;
            }
            // DESIGN NOTE: Also process the trailer (usually the UUT mode prompt) as part of the full response...
            else
            {
                ResponseBuffer += response;
                if ((!string.IsNullOrEmpty(trailer)) && (pEvent.Message.EndsWith(trailer)))
                    processResponse = true;
            }

            if (processResponse)
                complete = ProcessPortResponse();

            return complete;
        }

        protected void CompleteStep()
        {
            qf4net.QF.Instance.Publish(new TestEvent(QFSignal.TestStep, Name, Test.Steps[CurrentStep]));
            if (TestStepped != null)
                TestStepped(this, new TestSteppedEventArgs(Name, Test.Steps[CurrentStep]));

            if (++CurrentStep == Test.Steps.Count)
                TransitionToPassed();
            else
                TransitionToRunning();
        }
        protected virtual void Response(IQEvent qEvent)
        {
            IActor actor = (IActor)qEvent;
            if (actor.Name == Test.Steps[CurrentStep].Actor.Name)
            {
                try
                {
                    bool complete = false;
                    switch (actor.Type)
                    {
                        case ActorType.Port:
                            complete = PortResponse((PortEvent)qEvent);
                            break;
                        case ActorType.Process:
                        case ActorType.Prompt:
                        case ActorType.Recorder:
                        case ActorType.Log:
                            complete = true;
                            break;
                    }

                    if (complete)
                        CompleteStep();
                }
                catch (Exception ex)
                {
                    TransitionToFailed(ex.Message);
                }
            }
        }

        protected void Error(IQEvent qEvent)
        {
            string message = null;
            IError error = qEvent as IError;
            if (error == null)
                message = "Invalid TQF error event detected (type='" + qEvent.GetType().Name + "'). All error related TQF events must implement the IError interface.";
            else
                message = error.Error + " " + error.AdditionalDescription;

            qf4net.QF.Instance.Publish(new RecorderEvent(QFSignal.RecorderRecord, Name, CurrentStep + 1, message));

            IActor actor = (IActor)qEvent;
            if (actor.Name == Test.Steps[CurrentStep].Actor.Name)
            {
                TransitionToFailed(message);
            }
        }

        protected virtual void PortTimeout(PortEvent pEvent)
        {
            if ((Retries + 1) <= Test.Steps[CurrentStep].Retries)
                Step(new TestEvent(InternalSignal.TestStep, Name), Retries + 1);
            else
                TransitionToFailed("Response not recevied within timeout from Port '"+ pEvent.Name +"' in Solicited Mode.");
        }
        protected virtual void TestTimeout(TestEvent tEvent)
        {
            if (Test.Steps[CurrentStep].CompleteOnTimeout)
            {
                try
                {
                    if (Test.Steps[CurrentStep].Actor.Type == ActorType.Port)
                        ProcessPortResponse();

                    CompleteStep();
                }
                catch (Exception ex)
                {
                    TransitionToFailed(ex.Message);
                }
            }
            else if ((Retries + 1) <= Test.Steps[CurrentStep].Retries)
                Step(tEvent, Retries + 1);
            else
                TransitionToFailed("Response not recevied within timeout.");
        }
        protected virtual void ProcessTimeout(ProcessEvent tEvent)
        {
            if ((Retries + 1) <= Test.Steps[CurrentStep].Retries)
                Step(new TestEvent(InternalSignal.TestStep, Name), Retries + 1);
            else
                TransitionToFailed("Process did not launch successfully within timeout.");
        }
        protected void Timeout(IQEvent qEvent)
        {
            IActor actor = (IActor)qEvent;
            switch (actor.Type)
            {
                case ActorType.Test:
                    if (actor.Name == Name)
                        TestTimeout((TestEvent)qEvent);
                    break;
                case ActorType.Port:
                    if (actor.Name == Test.Steps[CurrentStep].Actor.Name)
                        PortTimeout((PortEvent)qEvent);
                    break;
                case ActorType.Process:
                    if (actor.Name == Test.Steps[CurrentStep].Actor.Name)
                        ProcessTimeout((ProcessEvent)qEvent);
                    break;
            }
        }

        protected static int s_TranIdx_Loaded_Running = s_TransitionChainStore.GetOpenSlot();
        protected virtual QState ActiveTest_Loaded(IQEvent qEvent)
        {
            switch (qEvent.QSignal)
            {
                case (int)QSignals.Entry:
                    return null;

                case (int)QFSignal.TestRun:
                    Run((TestEvent)qEvent);
                    return null;

                case (int)QFSignal.TestAbort:
                    TransitionToAborted((TestEvent)qEvent);
                    return null;

                case (int)QSignals.Exit:
                    return null;
            }
            return this.TopState;
        }
        protected static int s_TranIdx_Running_Stepping = s_TransitionChainStore.GetOpenSlot();
        protected static int s_TranIdx_Running_Passed = s_TransitionChainStore.GetOpenSlot();
        protected static int s_TranIdx_Running_Failed = s_TransitionChainStore.GetOpenSlot();
        protected static int s_TranIdx_Running_Aborted = s_TransitionChainStore.GetOpenSlot();
        protected virtual QState ActiveTest_Running(IQEvent qEvent)
        {
            switch (qEvent.QSignal)
            {
                case (int)QSignals.Entry:
                    return null;

                case (int)InternalSignal.TestStep:
                    Step((TestEvent)qEvent, 0);
                    return null;

                case (int)QFSignal.Timeout:
                    Timeout(qEvent);
                    return null;

                case (int)QFSignal.Error:
                    Error(qEvent);
                    return null;

                case (int)QFSignal.TestAbort:
                    TransitionToAborted((TestEvent)qEvent);
                    return null;

                case (int)QSignals.Exit:
                    return null;
            }
            return this.TopState;
        }
        protected static int s_TranIdx_Stepping_Running = s_TransitionChainStore.GetOpenSlot();
        protected virtual QState ActiveTest_Stepping(IQEvent qEvent)
        {
            switch (qEvent.QSignal)
            {
                case (int)QSignals.Entry:
                    return null;

                case (int)QFSignal.PortResponse:
                case (int)QFSignal.ProcessLaunched:
                case (int)QFSignal.PromptClosed:
                case (int)QFSignal.PromptContinue:
                case (int)QFSignal.RecorderOpened:
                case (int)QFSignal.RecorderOpenPending:
                    Response(qEvent);
                    return null;

                case (int)QSignals.Exit:
                    return null;
            }
            // DESIGN NOTE: Hierarchical state
            return m_StateRunning;
        }
        protected static int s_TranIdx_Passed_Ended = s_TransitionChainStore.GetOpenSlot();
        protected virtual QState ActiveTest_Passed(IQEvent qEvent)
        {
            switch (qEvent.QSignal)
            {
                case (int)QSignals.Entry:
                    return null;

                case (int)InternalSignal.TestPassed:
                    Passed((TestEvent)qEvent);
                    return null;

                case (int)QSignals.Exit:
                    return null;
            }
            return this.TopState;
        }
        protected static int s_TranIdx_Failed_Ended = s_TransitionChainStore.GetOpenSlot();
        protected virtual QState ActiveTest_Failed(IQEvent qEvent)
        {
            switch (qEvent.QSignal)
            {
                case (int)QSignals.Entry:
                    return null;

                case (int)InternalSignal.TestFailed:
                    Failed((TestEvent)qEvent);
                    return null;

                case (int)QSignals.Exit:
                    return null;
            }
            return this.TopState;
        }
        protected static int s_TranIdx_Aborted_Ended = s_TransitionChainStore.GetOpenSlot();
        protected virtual QState ActiveTest_Aborted(IQEvent qEvent)
        {
            switch (qEvent.QSignal)
            {
                case (int)QSignals.Entry:
                    return null;

                case (int)InternalSignal.TestAborted:
                    Aborted((TestEvent)qEvent);
                    return null;

                case (int)QSignals.Exit:
                    return null;
            }
            return this.TopState;
        }
        // DESIGN NOTE: Final Test state
        protected virtual QState ActiveTest_Ended(IQEvent qEvent)
        {
            switch (qEvent.QSignal)
            {
                case (int)QSignals.Entry:
                    Debug.Print("<Reached Ended state. Object=" + this.GetType().Name + ", Name=" + Name + ">");
                    return null;

                case (int)QSignals.Exit:
                    return null;
            }
            return this.TopState;
        }
    }
}