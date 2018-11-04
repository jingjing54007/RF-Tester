using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using qf4net;
using TruePosition.Test.Process;
using System.Threading;
using System.Diagnostics;

namespace TruePosition.Test.QF
{
    public class ActiveProcess : QActive
    {
		#region Boiler plate static stuff

		private static new TransitionChainStore s_TransitionChainStore = 
			new TransitionChainStore(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        static ActiveProcess()
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

        public string Name { get; private set; }
        protected IProcess Process { get; set; }

        protected internal QState m_StateCreated;
        protected internal QState m_StateLaunching;
        protected internal QState m_StateLaunched;
        protected internal QState m_StateKilling;
        protected internal QState m_StateKilled;
        protected internal QState m_StateFaulted;

        void Process_Launched(object sender, ProcessEventArgs args)
        {
            qf4net.QF.Instance.Publish(new ProcessEvent(QFSignal.ProcessLaunched, Name));
        }
        void Process_Killed(object sender, ProcessEventArgs args)
        {
            qf4net.QF.Instance.Publish(new ProcessEvent(QFSignal.ProcessKilled, Name));
        }
        void Process_Error(object sender, ProcessErrorEventArgs args)
        {
            qf4net.QF.Instance.Publish(new ProcessEvent(QFSignal.Error, Name, args.Error, args.AdditionalDescription));
        }
        void Process_Timeout(object sender, ProcessEventArgs e)
        {
            qf4net.QF.Instance.Publish(new ProcessEvent(QFSignal.Timeout, Name));
        }

        private void Initialize(IProcess process)
        {
            Name = process.Name;
            Process = process;

            m_StateCreated = new QState(ActiveProcess_Created);
            m_StateLaunching = new QState(ActiveProcess_Launching);
            m_StateLaunched = new QState(ActiveProcess_Launched);
            m_StateKilling = new QState(ActiveProcess_Killing);
            m_StateKilled = new QState(ActiveProcess_Killed);
            m_StateFaulted = new QState(ActiveProcess_Faulted);

            Process.Launched += new EventHandler<ProcessEventArgs>(Process_Launched);
            Process.Killed += new EventHandler<ProcessEventArgs>(Process_Killed);
            Process.Timeout += new EventHandler<ProcessEventArgs>(Process_Timeout);
            Process.Error += new EventHandler<ProcessErrorEventArgs>(Process_Error);
        }

        public ActiveProcess(IProcess process) : base()
        {
            Initialize(process);
        }

        protected override void InitializeStateMachine()
        {
            Thread.CurrentThread.Name = this.ToString();
            qf4net.QF.Instance.Subscribe(this, (int)QFSignal.ProcessLaunch);
            qf4net.QF.Instance.Subscribe(this, (int)QFSignal.ProcessKill);
            InitializeState(m_StateCreated);
        }

        public override string ToString()
        {
            return "ActiveProcess " + Name;
        }
        public override void Dispose()
        {
            if (Process != null)
                Process.Dispose();

            base.Dispose();
        }

        public void SubscribeLaunched(EventHandler<ProcessEventArgs> handler)
        {
            Process.Launched += handler;
        }
        public void SubscribeKilled(EventHandler<ProcessEventArgs> handler)
        {
            Process.Killed += handler;
        }
        public void SubscribeTimeout(EventHandler<ProcessEventArgs> handler)
        {
            Process.Timeout += handler;
        }
        public void SubscribeError(EventHandler<ProcessErrorEventArgs> handler)
        {
            Process.Error += handler;
        }

        private void Launching(ProcessEvent pEvent)
        {
            if (Name == pEvent.Name)
            {
                TransitionTo(m_StateLaunching, s_TranIdx_Created_Launching);
                PostFIFO(new ProcessEvent(InternalSignal.ProcessLaunching, pEvent));
            }
        }
        private void Launch(ProcessEvent pEvent)
        {
            if (Name == pEvent.Name)
            {
                try
                {
                    Process.Launch(pEvent.CommandLine);
                    TransitionTo(m_StateLaunched, s_TranIdx_Launching_Launched);
                }
                catch (Exception ex)
                {
                    TransitionTo(m_StateFaulted, s_TranIdx_Launching_Faulted);
                }
            }
        }
        private void Killing(ProcessEvent pEvent)
        {
            if (Name == pEvent.Name)
            {
                TransitionTo(m_StateKilling, s_TranIdx_Launched_Killing);
                PostFIFO(new ProcessEvent(InternalSignal.ProcessKilling, pEvent));
            }
        }
        private void Kill(ProcessEvent pEvent)
        {
            if (Name == pEvent.Name)
            {
                try
                {
                    Process.Kill();
                    TransitionTo(m_StateKilled, s_TranIdx_Killing_Killed);
                }
                catch (Exception ex)
                {
                    TransitionTo(m_StateFaulted, s_TranIdx_Killing_Faulted);
                }
            }
        }

        private static int s_TranIdx_Created_Launching = s_TransitionChainStore.GetOpenSlot();
        protected virtual QState ActiveProcess_Created(IQEvent qEvent)
        {
            switch (qEvent.QSignal)
            {
                case (int)QSignals.Entry:
                    return null;

                case (int)QFSignal.ProcessLaunch:
                    Launching((ProcessEvent)qEvent);
                    return null;

                case (int)QSignals.Exit:
                    return null;
            }
            return this.TopState;
        }
        private static int s_TranIdx_Launching_Launched = s_TransitionChainStore.GetOpenSlot();
        private static int s_TranIdx_Launching_Faulted = s_TransitionChainStore.GetOpenSlot();
        protected virtual QState ActiveProcess_Launching(IQEvent qEvent)
        {
            switch (qEvent.QSignal)
            {
                case (int)QSignals.Entry:
                    return null;

                case (int)InternalSignal.ProcessLaunching:
                    Launch((ProcessEvent)qEvent);
                    return null;

                case (int)QSignals.Exit:
                    return null;
            }
            return this.TopState;
        }
        private static int s_TranIdx_Launched_Killing = s_TransitionChainStore.GetOpenSlot();
        protected virtual QState ActiveProcess_Launched(IQEvent qEvent)
        {
            switch (qEvent.QSignal)
            {
                case (int)QSignals.Entry:
                    return null;

                case (int)QFSignal.ProcessKill:
                    Killing((ProcessEvent)qEvent);
                    return null;

                case (int)QSignals.Exit:
                    return null;
            }
            return this.TopState;
        }
        private static int s_TranIdx_Killing_Killed = s_TransitionChainStore.GetOpenSlot();
        private static int s_TranIdx_Killing_Faulted = s_TransitionChainStore.GetOpenSlot();
        protected virtual QState ActiveProcess_Killing(IQEvent qEvent)
        {
            switch (qEvent.QSignal)
            {
                case (int)QSignals.Entry:
                    return null;

                case (int)InternalSignal.ProcessKilling:
                    Kill((ProcessEvent)qEvent);
                    return null;

                case (int)QSignals.Exit:
                    return null;
            }
            return this.TopState;
        }
        protected virtual QState ActiveProcess_Killed(IQEvent qEvent)
        {
            switch (qEvent.QSignal)
            {
                case (int)QSignals.Entry:
                    Debug.Print("<Reached Killed state. Object=" + this.GetType().Name + ", Name=" + Name + ">");
                    return null;
                case (int)QSignals.Exit:
                    return null;
            }
            return this.TopState;
        }
        protected virtual QState ActiveProcess_Faulted(IQEvent qEvent)
        {
            switch (qEvent.QSignal)
            {
                case (int)QSignals.Entry:
                    Debug.Print("<Entered Faulted state. Object=" + this.GetType().Name + ", Name=" + Name + ">");
                    return null;
                case (int)QFSignal.ProcessKill:
                    Kill((ProcessEvent)qEvent);
                    return null;
                case (int)QSignals.Exit:
                    return null;
            }
            return this.TopState;
        }
    }
}
