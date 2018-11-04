using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

using qf4net;
using TruePosition.Test.IO;
using System.Diagnostics;

namespace TruePosition.Test.QF
{
    internal class ActivePort : QActive
    {
		#region Boiler plate static stuff

		private static new TransitionChainStore s_TransitionChainStore = 
			new TransitionChainStore(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        static ActivePort()
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
        protected IPort Port { get; set; }

        protected internal QState m_StateCreated;
        protected internal QState m_StateOpening;
        protected internal QState m_StateOpened;
        protected internal QState m_StateClosing;
        protected internal QState m_StateClosed;
        protected internal QState m_StateFaulted;

        private void port_DataTransmitted(object sender, PortMessageEventArgs args)
        {
            qf4net.QF.Instance.Publish(new PortEvent(QFSignal.PortTransmitted, Name, args.Message));
        }
        private void port_DataReceived(object sender, PortMessageEventArgs args)
        {
            qf4net.QF.Instance.Publish(new PortEvent(QFSignal.PortResponse, Name, args.Message));
        }
        private void port_Error(object sender, PortErrorEventArgs args)
        {
            qf4net.QF.Instance.Publish(new PortEvent(QFSignal.Error, Name, args.Error, args.AdditionalDescription));
        }
        private void port_ReceiveTimeoutExpired(object sender, PortTimeoutExpiredEventArgs args)
        {
            qf4net.QF.Instance.Publish(new PortEvent(QFSignal.Timeout, Name));
        }

        private void Initialize(IPort port)
        {
            Name = port.Name;
            Port = port;

            m_StateCreated = new QState(ActivePort_Created);
            m_StateOpening = new QState(ActivePort_Opening);
            m_StateOpened = new QState(ActivePort_Opened);
            m_StateClosing = new QState(ActivePort_Closing);
            m_StateClosed = new QState(ActivePort_Closed);
            m_StateFaulted = new QState(ActivePort_Faulted);

            port.DataTransmitted += new EventHandler<PortMessageEventArgs>(port_DataTransmitted);
            Port.DataReceived += new EventHandler<PortMessageEventArgs>(port_DataReceived);
            Port.Error += new EventHandler<PortErrorEventArgs>(port_Error);
            Port.ReceiveTimeoutExpired += new EventHandler<PortTimeoutExpiredEventArgs>(port_ReceiveTimeoutExpired);
        }
        public ActivePort(IPort port) : base()
        {
            Initialize(port);
        }

        protected override void InitializeStateMachine()
        {
            Thread.CurrentThread.Name = this.ToString();
            qf4net.QF.Instance.Subscribe(this, (int)QFSignal.PortOpen);
            qf4net.QF.Instance.Subscribe(this, (int)QFSignal.PortClose);
            qf4net.QF.Instance.Subscribe(this, (int)QFSignal.PortTransmit);
            InitializeState(m_StateCreated);
        }

        public PortState State
        {
            get
            {
                return Port.State;
            }
        }
        public override string ToString()
        {
            return "ActivePort " + Name;
        }

        public override void Dispose()
        {
            if (Port != null)
                Port.Dispose();

            base.Dispose();
        }

        public void SubscribeTransmitted(EventHandler<PortMessageEventArgs> handler)
        {
            Port.DataTransmitted += handler;
        }
        public void SubscribeReceive(EventHandler<PortMessageEventArgs> handler)
        {
            Port.DataReceived += handler;
        }
        public void SubscribeError(EventHandler<PortErrorEventArgs> handler)
        {
            Port.Error += handler;
        }
        public void SubscribeReceiveTimeout(EventHandler<PortTimeoutExpiredEventArgs> handler)
        {
            Port.ReceiveTimeoutExpired += handler;
        }

        protected void Opening(PortEvent pEvent)
        {
            if (Name == pEvent.Name)
            {
                TransitionTo(m_StateOpening, s_TranIdx_Created_Opening);
                PostFIFO(new PortEvent(InternalSignal.PortOpening, pEvent));
            }
        }
        protected void Open(PortEvent pEvent)
        {
            if ((Name == pEvent.Name) && (Port.State == PortState.Created))
            {
                Port.Open();
                if (Port.State != PortState.Opened)
                {
                    TransitionTo(m_StateFaulted, s_TranIdx_Opening_Faulted);
                }
                else
                {
                    TransitionTo(m_StateOpened, s_TranIdx_Opening_Opened);
                }
            }
        }
        protected void Closing(PortEvent pEvent)
        {
            if (Name == pEvent.Name)
            {
                TransitionTo(m_StateClosing, s_TranIdx_Opened_Closing);
                PostFIFO(new PortEvent(InternalSignal.PortClosing, pEvent));
            }
        }
        protected void Close(PortEvent pEvent)
        {
            if ((Name == pEvent.Name) && (Port.State == PortState.Opened))
            {
                Port.Close();
                if (Port.State != PortState.Closed)
                {
                    TransitionTo(m_StateFaulted, s_TranIdx_Closing_Faulted);
                }
                else
                {
                    TransitionTo(m_StateClosed, s_TranIdx_Closing_Closed);
                }
            }
        }
        protected void Transmit(PortEvent pEvent)
        {
            if (Name == pEvent.Name)
            {
                if (pEvent.Timeout >= 0)
                    Port.ReceiveTimeout = pEvent.Timeout;
                if (pEvent.Retries >= 0)
                    Port.TransmitRetries = pEvent.Retries;

                Port.Transmit(pEvent.Message, true);
            }
        }

        private static int s_TranIdx_Created_Opening = s_TransitionChainStore.GetOpenSlot();
        protected virtual QState ActivePort_Created(IQEvent qEvent)
        {
            switch (qEvent.QSignal)
            {
                case (int)QSignals.Entry:
                    return null;

                case (int)QFSignal.PortOpen:
                    Opening((PortEvent)qEvent);
                    return null;

                case (int)QSignals.Exit:
                    return null;
            }
            return this.TopState;
        }
        private static int s_TranIdx_Opening_Opened = s_TransitionChainStore.GetOpenSlot();
        private static int s_TranIdx_Opening_Faulted = s_TransitionChainStore.GetOpenSlot();
        protected virtual QState ActivePort_Opening(IQEvent qEvent)
        {
            switch (qEvent.QSignal)
            {
                case (int)QSignals.Entry:
                    return null;

                case (int)InternalSignal.PortOpening:
                    Open((PortEvent)qEvent);
                    return null;

                case (int)QSignals.Exit:
                    return null;
            }
            return this.TopState;
        }
        private static int s_TranIdx_Opened_Closing = s_TransitionChainStore.GetOpenSlot();
        protected virtual QState ActivePort_Opened(IQEvent qEvent)
        {
            switch (qEvent.QSignal)
            {
                case (int)QSignals.Entry:
                    return null;

                case (int)QFSignal.PortTransmit:
                    Transmit((PortEvent)qEvent);
                    return null;

                case (int)QFSignal.PortClose:
                    Closing((PortEvent)qEvent);
                    return null;

                case (int)QSignals.Exit:
                    return null;
            }
            return this.TopState;
        }
        private static int  s_TranIdx_Closing_Closed = s_TransitionChainStore.GetOpenSlot();
        private static int s_TranIdx_Closing_Faulted = s_TransitionChainStore.GetOpenSlot();
        protected virtual QState ActivePort_Closing(IQEvent qEvent)
        {
            switch (qEvent.QSignal)
            {
                case (int)QSignals.Entry:
                    return null;

                case (int)InternalSignal.PortClosing:
                    Close((PortEvent)qEvent);
                    return null;

                case (int)QSignals.Exit:
                    return null;
            }
            return this.TopState;
        }
        protected virtual QState ActivePort_Closed(IQEvent qEvent)
        {
            switch (qEvent.QSignal)
            {
                case (int)QSignals.Entry:
                case (int)QSignals.Exit:
                    return null;
            }
            return this.TopState;
        }
        protected virtual QState ActivePort_Faulted(IQEvent qEvent)
        {
            switch (qEvent.QSignal)
            {
                case (int)QSignals.Entry:
                    Debug.Print("<Reached Faulted state. Object=" + this.GetType().Name + ", Name=" + Name + ">");
                    return null;

                case (int)QFSignal.PortClose:
                    Close((PortEvent)qEvent);
                    return null;

                case (int)QSignals.Exit:
                    return null;
            }
            return this.TopState;
        }
    }
}
