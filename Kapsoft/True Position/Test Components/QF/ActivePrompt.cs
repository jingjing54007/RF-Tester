using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using qf4net;
using TruePosition.Test.Prompt;
using System.Threading;
using System.Diagnostics;

namespace TruePosition.Test.QF
{
    public class ActivePrompt : QActive
    {
		#region Boiler plate static stuff

		private static new TransitionChainStore s_TransitionChainStore = 
			new TransitionChainStore(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        static ActivePrompt()
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
        protected IPrompt Prompt { get; set; }

        protected internal QState m_StateCreated;
        protected internal QState m_StateShowing;
        protected internal QState m_StateShown;
        protected internal QState m_StateClosing;
        protected internal QState m_StateClosed;
        protected internal QState m_StateFaulted;

        void Prompt_Closed(object sender, PromptEventArgs args)
        {
            qf4net.QF.Instance.Publish(new PromptEvent(QFSignal.PromptClosed, Name));
        }
        void Prompt_Continue(object sender, PromptEventArgs args)
        {
            qf4net.QF.Instance.Publish(new PromptEvent(QFSignal.PromptContinue, Name));
        }
        void Prompt_Error(object sender, PromptErrorEventArgs args)
        {
            qf4net.QF.Instance.Publish(new PromptEvent(QFSignal.Error, Name, args.Error, args.AdditionalDescription));
        }

        private void Initialize(IPrompt prompt)
        {
            Name = prompt.Name;
            Prompt = prompt;

            m_StateCreated = new QState(ActivePrompt_Created);
            m_StateShowing = new QState(ActivePrompt_Showing);
            m_StateShown = new QState(ActivePrompt_Shown);
            m_StateClosing = new QState(ActivePrompt_Closing);
            m_StateClosed = new QState(ActivePrompt_Closed);
            m_StateFaulted = new QState(ActivePrompt_Faulted);

            Prompt.Closed += new EventHandler<PromptEventArgs>(Prompt_Closed);
            Prompt.Continue += new EventHandler<PromptEventArgs>(Prompt_Continue);
            Prompt.Error += new EventHandler<PromptErrorEventArgs>(Prompt_Error);
        }

        public ActivePrompt(IPrompt prompt) : base()
        {
            Initialize(prompt);
        }

        protected override void InitializeStateMachine()
        {
            Thread.CurrentThread.Name = this.ToString();
            qf4net.QF.Instance.Subscribe(this, (int)QFSignal.PromptShow);
            qf4net.QF.Instance.Subscribe(this, (int)QFSignal.PromptClosed);
            InitializeState(m_StateCreated);
        }

        public override string ToString()
        {
            return "ActivePrompt " + Name;
        }
        public override void Dispose()
        {
            if (Prompt != null)
                Prompt.Dispose();

            base.Dispose();
        }

        public void SubscribeShowing(PromptShowingDelegate handler)
        {
            Prompt.Showing += handler;
        }
        public void SubscribeClosed(EventHandler<PromptEventArgs> handler)
        {
            Prompt.Closed += handler;
        }
        public void SubscribeError(EventHandler<PromptErrorEventArgs> handler)
        {
            Prompt.Error += handler;
        }

        private void Showing(PromptEvent pEvent)
        {
            if (Name == pEvent.Name)
            {
                TransitionTo(m_StateShowing, s_TranIdx_Created_Showing);
                PostFIFO(new PromptEvent(InternalSignal.PromptShowing, pEvent));
            }
        }
        private void Show(PromptEvent pEvent)
        {
            if (Name == pEvent.Name)
            {
                try
                {
                    TransitionTo(m_StateShown, s_TranIdx_Showing_Shown);
                    qf4net.QF.Instance.Publish(new PromptEvent(QFSignal.PromptShown, Name));
                    Prompt.Show(pEvent.Text, pEvent.Timeout);
                }
                catch (Exception ex)
                {
                    TransitionTo(m_StateFaulted, s_TranIdx_Showing_Faulted);
                }
            }
        }
        private void Closing(PromptEvent pEvent)
        {
            if (Name == pEvent.Name)
            {
                TransitionTo(m_StateClosing, s_TranIdx_Shown_Closing);
                PostFIFO(new PromptEvent(InternalSignal.PromptClosing, pEvent));
            }
        }
        private void Close(PromptEvent pEvent)
        {
            if (Name == pEvent.Name)
            {
                TransitionTo(m_StateClosed, s_TranIdx_Closing_Closed);
            }
        }

        private static int s_TranIdx_Created_Showing = s_TransitionChainStore.GetOpenSlot();
        protected virtual QState ActivePrompt_Created(IQEvent qEvent)
        {
            switch (qEvent.QSignal)
            {
                case (int)QSignals.Entry:
                    return null;

                case (int)QFSignal.PromptShow:
                    Showing((PromptEvent)qEvent);
                    return null;

                case (int)QSignals.Exit:
                    return null;
            }
            return this.TopState;
        }
        private static int s_TranIdx_Showing_Shown = s_TransitionChainStore.GetOpenSlot();
        private static int s_TranIdx_Showing_Faulted = s_TransitionChainStore.GetOpenSlot();
        protected virtual QState ActivePrompt_Showing(IQEvent qEvent)
        {
            switch (qEvent.QSignal)
            {
                case (int)QSignals.Entry:
                    return null;

                case (int)InternalSignal.PromptShowing:
                    Show((PromptEvent)qEvent);
                    return null;

                case (int)QSignals.Exit:
                    return null;
            }
            return this.TopState;
        }
        private static int s_TranIdx_Shown_Closing = s_TransitionChainStore.GetOpenSlot();
        protected virtual QState ActivePrompt_Shown(IQEvent qEvent)
        {
            switch (qEvent.QSignal)
            {
                case (int)QSignals.Entry:
                    return null;

                case (int)QFSignal.PromptClosed:
                    Closing((PromptEvent)qEvent);
                    return null;

                case (int)QSignals.Exit:
                    return null;
            }
            return this.TopState;
        }
        private static int s_TranIdx_Closing_Closed = s_TransitionChainStore.GetOpenSlot();
        private static int s_TranIdx_Closing_Faulted = s_TransitionChainStore.GetOpenSlot();
        protected virtual QState ActivePrompt_Closing(IQEvent qEvent)
        {
            switch (qEvent.QSignal)
            {
                case (int)QSignals.Entry:
                    return null;

                case (int)InternalSignal.PromptClosing:
                    Close((PromptEvent)qEvent);
                    return null;

                case (int)QSignals.Exit:
                    return null;
            }
            return this.TopState;
        }
        protected virtual QState ActivePrompt_Closed(IQEvent qEvent)
        {
            switch (qEvent.QSignal)
            {
                case (int)QSignals.Entry:
                    Debug.Print("<Reached Closed state. Object=" + this.GetType().Name + ", Name=" + Name + ">");
                    return null;
                case (int)QSignals.Exit:
                    return null;
            }
            return this.TopState;
        }
        protected virtual QState ActivePrompt_Faulted(IQEvent qEvent)
        {
            switch (qEvent.QSignal)
            {
                case (int)QSignals.Entry:
                    Debug.Print("<Entered Faulted state. Object=" + this.GetType().Name + ", Name=" + Name + ">");
                    return null;
                case (int)QSignals.Exit:
                    return null;
            }
            return this.TopState;
        }
    }
}
