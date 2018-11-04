using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;

using qf4net;
using TruePosition.Test.DataLayer;
using TruePosition.Test.Recorder;
using System.Xml.Linq;

namespace TruePosition.Test.QF
{
    public class ActiveRecorder : QActive
    {
		#region Boiler plate static stuff

		private static new TransitionChainStore s_TransitionChainStore = 
			new TransitionChainStore(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        static ActiveRecorder()
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
        protected IRecorder Recorder { get; set; }
        protected string LocationFormat { get; set; }
        protected string EntryFormat { get; set; }
        protected EntryStyle EntryStyle { get; set; }
        protected string ActiveCommand { get; set; }
        protected IEnumerable<XElement> Values { get; set; }
        protected IEnumerable<Evaluator> CommandEvaluators { get; set; }
        protected IEnumerable<Evaluator> ResponseEvaluators { get; set; }

        protected internal QState m_StateCreated;
        protected internal QState m_StateOpening;
        protected internal QState m_StateOpened;
        protected internal QState m_StateClosing;
        protected internal QState m_StateClosed;
        protected internal QState m_StateFaulted;

        void Recorder_Opened(object sender, RecorderEventArgs args)
        {
            qf4net.QF.Instance.Publish(new RecorderEvent(QFSignal.RecorderOpened, Name, null));
        }
        void Recorder_Closed(object sender, RecorderEventArgs args)
        {
            qf4net.QF.Instance.Publish(new RecorderEvent(QFSignal.RecorderClosed, Name, null));
        }
        void Recorder_Error(object sender, RecorderErrorEventArgs args)
        {
            qf4net.QF.Instance.Publish(new RecorderEvent(QFSignal.Error, Name, args.Error, args.AdditionalDescription));
        }

        private void Initialize(IRecorder recorder)
        {
            Name = recorder.Name;
            Recorder = recorder;

            m_StateCreated = new QState(ActiveRecorder_Created);
            m_StateOpening = new QState(ActiveRecorder_Opening);
            m_StateOpened = new QState(ActiveRecorder_Opened);
            m_StateClosing = new QState(ActiveRecorder_Closing);
            m_StateClosed = new QState(ActiveRecorder_Closed);
            m_StateFaulted = new QState(ActiveRecorder_Faulted);

            Recorder.Opened += new EventHandler<RecorderEventArgs>(Recorder_Opened);
            Recorder.Closed += new EventHandler<RecorderEventArgs>(Recorder_Closed);
            Recorder.Error += new EventHandler<RecorderErrorEventArgs>(Recorder_Error);
        }

        public ActiveRecorder(IRecorder recorder) : base()
        {
            Initialize(recorder);
        }

        protected override void InitializeStateMachine()
        {
            Thread.CurrentThread.Name = this.ToString();
            qf4net.QF.Instance.Subscribe(this, (int)QFSignal.RecorderOpen);
            qf4net.QF.Instance.Subscribe(this, (int)QFSignal.RecorderRecord);
            qf4net.QF.Instance.Subscribe(this, (int)QFSignal.RecorderClose);
            InitializeState(m_StateCreated);
        }

        public override string ToString()
        {
            return "ActiveRecorder " + Name;
        }
        public override void Dispose()
        {
            if (Recorder != null)
                Recorder.Dispose();

            base.Dispose();
        }

        public void SubscribeOpening(RecorderOpeningDelegate handler)
        {
            Recorder.Opening += handler;
        }
        public void SubscribeOpened(EventHandler<RecorderEventArgs> handler)
        {
            Recorder.Opened += handler;
        }
        public void SubscribeRecording(RecorderRecordingDelegate handler)
        {
            Recorder.Recording += handler;
        }
        public void SubscribeClosed(EventHandler<RecorderEventArgs> handler)
        {
            Recorder.Closed += handler;
        }
        public void SubscribeError(EventHandler<RecorderErrorEventArgs> handler)
        {
            Recorder.Error += handler;
        }

        private void OnError(Exception ex)
        {
            if (Recorder != null)
                Recorder.OnError(ex);
        }

        private void Opening(RecorderEvent rEvent, int chainIndex)
        {
            if (Name == rEvent.Name)
            {
                TransitionTo(m_StateOpening, chainIndex);
                PostFIFO(new RecorderEvent(InternalSignal.RecorderOpening, rEvent));
            }
        }
        private IEnumerable<Evaluator> ProcessFilter(IEnumerable<string> filterExpressions)
        {
            try
            {
                List<Evaluator> evaluators = new List<Evaluator>();
                if (filterExpressions != null)
                {
                    foreach (string expression in filterExpressions)
                        evaluators.Add(new Evaluator(expression));
                }

                return evaluators;
            }
            catch (Exception ex)
            {
                Recorder_Error(null, new RecorderErrorEventArgs(Name, ex.Message));
                throw ex;
            }
        }
        private void Open(RecorderEvent rEvent)
        {
            if (Name == rEvent.Name)
            {
                try
                {
                    // DESIGN NOTE:
                    // If the Recorder is open, close it. This allows an existing Recorder to be reused to create additional files...
                    CloseRecorder();

                    CommandEvaluators = ProcessFilter(rEvent.CommandFilter);
                    ResponseEvaluators = ProcessFilter(rEvent.ResponseFilter);
                    EntryFormat = rEvent.EntryFormat;
                    EntryStyle = rEvent.EntryStyle;
                    Values = rEvent.Values;

                    // DESIGN NOTE:
                    // Detect if location is command dependent and defer open... 
                    if (rEvent.Location.Contains("{Command") || rEvent.Location.Contains("{Response"))
                    {
                        LocationFormat = rEvent.Location;
                        qf4net.QF.Instance.Publish(new RecorderEvent(QFSignal.RecorderOpenPending, Name, null));
                    }
                    else
                        Recorder.Open(rEvent.Location);

                    TransitionTo(m_StateOpened, s_TranIdx_Opening_Opened);
                }
                catch (Exception ex)
                {
                    TransitionTo(m_StateFaulted, s_TranIdx_Opening_Faulted);
                }
            }
        }
        private bool EvaluateEntry(IEnumerable<Evaluator> evaluators, string entry)
        {
            try
            {
                bool match = true;
                foreach (Evaluator evaluator in evaluators)
                {
                    match = evaluator.Evaluate(new ValueHelper(entry));
                    if (!match)
                        break;
                }
                return match;
            }
            catch (Exception ex)
            {
                OnError(ex);
                throw ex;
            }
        }
        private void Record(RecorderEvent rEvent)
        {
            // DESIGN NOTE:
            // Recorders listen to all broadcasts and filter by entry evaluation...
            if (!string.IsNullOrEmpty(rEvent.Entry))
            {
                try
                {
                    // TODO:
                    // 1. Detect & Save Command
                    // 2. On response detection, fire Writing event w/ Command & Response 
                    // 3. Either:
                    //      - Detect deferred Open and Open using formatted location
                    //      - Record resulting formatted entry

                    // TODO: Refine logic to include edge scenarios
                    // 1. EntryFormat only contains Command reference
                    // 2. EntryFormat only contains Response reference
                    // 3. EntryFormat contains neither
                    // 3. EntryFormat empty (null)

                    if (EvaluateEntry(CommandEvaluators, rEvent.Entry))
                    {
                        ActiveCommand = rEvent.Entry;
                    }
                    else if (!string.IsNullOrEmpty(ActiveCommand))
                    {
                        string location = string.Empty;
                        if (!string.IsNullOrEmpty(LocationFormat))
                        {
                            try
                            {
                                Recorder.OnOpening(rEvent.TestName, rEvent.StepNumber, LocationFormat, Values, ActiveCommand, rEvent.Entry, ref location);
                                if (string.IsNullOrEmpty(location))
                                    throw new ArgumentNullException("Invalid resulting Location in Recorder.");
                            }
                            catch (Exception ex)
                            {
                                OnError(ex);
                                throw ex;
                            }
                            Recorder.Open(location);
                            LocationFormat = string.Empty;
                        }

                        string entry = string.Empty;
                        if (EvaluateEntry(ResponseEvaluators, rEvent.Entry))
                        {
                            try
                            {
                                if (EntryStyle == EntryStyle.Single)
                                    Recorder.OnRecording(rEvent.TestName, rEvent.StepNumber, EntryFormat, Values, ActiveCommand, rEvent.Entry, ref entry);
                                else
                                {
                                    string lines = string.Empty;
                                    foreach (string line in rEvent.Entry.Split(new string[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries))
                                    {
                                        entry = string.Empty;
                                        string format = EntryFormat.Replace("{Response[*]}", line);
 
                                        Recorder.OnRecording(rEvent.TestName, rEvent.StepNumber, format, Values, ActiveCommand, rEvent.Entry, ref entry);
                                        lines += entry + "\r\n";
                                    }
                                    entry = lines;
                                }
                                if (string.IsNullOrEmpty(entry))
                                    throw new ArgumentNullException("Invalid resulting Entry in Recorder.");
                            }
                            catch (Exception ex)
                            {
                                OnError(ex);
                                throw ex;
                            }
                        }

                        Recorder.Record(entry);
                    }
                }
                catch (Exception ex)
                {
                    TransitionTo(m_StateFaulted, s_TranIdx_Opened_Faulted);
                }
            }
        }
        private void Closing(RecorderEvent rEvent)
        {
            if (Name == rEvent.Name)
            {
                TransitionTo(m_StateClosing, s_TranIdx_Opened_Closing);
                PostFIFO(new RecorderEvent(InternalSignal.RecorderClosing, rEvent));
            }
        }
        private void CloseRecorder()
        {
            try
            {
                Recorder.Close();

                ActiveCommand = string.Empty;
                CommandEvaluators = null;
                ResponseEvaluators = null;
                LocationFormat = string.Empty;
                EntryFormat = string.Empty;
                Values = null;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        private void Close(RecorderEvent rEvent)
        {
            if (Name == rEvent.Name)
            {
                try
                {
                    CloseRecorder();
                    TransitionTo(m_StateClosed, s_TranIdx_Closing_Closed);
                }
                catch (Exception ex)
                {
                    TransitionTo(m_StateFaulted, s_TranIdx_Closing_Faulted);
                }
            }
        }

        private static int s_TranIdx_Created_Opening = s_TransitionChainStore.GetOpenSlot();
        protected virtual QState ActiveRecorder_Created(IQEvent qEvent)
        {
            switch (qEvent.QSignal)
            {
                case (int)QSignals.Entry:
                    return null;

                case (int)QFSignal.RecorderOpen:
                    Opening((RecorderEvent)qEvent, s_TranIdx_Created_Opening);
                    return null;

                case (int)QSignals.Exit:
                    return null;
            }
            return this.TopState;
        }
        private static int s_TranIdx_Opening_Opened = s_TransitionChainStore.GetOpenSlot();
        private static int s_TranIdx_Opening_Faulted = s_TransitionChainStore.GetOpenSlot();
        protected virtual QState ActiveRecorder_Opening(IQEvent qEvent)
        {
            switch (qEvent.QSignal)
            {
                case (int)QSignals.Entry:
                    return null;

                case (int)InternalSignal.RecorderOpening:
                    Open((RecorderEvent)qEvent);
                    return null;

                case (int)QSignals.Exit:
                    return null;
            }
            return this.TopState;
        }
        private static int s_TranIdx_Opened_Closing = s_TransitionChainStore.GetOpenSlot();
        private static int s_TranIdx_Opened_Faulted = s_TransitionChainStore.GetOpenSlot();
        protected virtual QState ActiveRecorder_Opened(IQEvent qEvent)
        {
            switch (qEvent.QSignal)
            {
                case (int)QSignals.Entry:
                    return null;

                case (int)QFSignal.RecorderOpen:
                    Open((RecorderEvent)qEvent);
                    return null;

                case (int)QFSignal.RecorderRecord:
                    Record((RecorderEvent)qEvent);
                    return null;

                case (int)QFSignal.RecorderClose:
                    Closing((RecorderEvent)qEvent);
                    return null;

                case (int)QSignals.Exit:
                    return null;
            }
            return this.TopState;
        }
        private static int s_TranIdx_Closing_Closed = s_TransitionChainStore.GetOpenSlot();
        private static int s_TranIdx_Closing_Faulted = s_TransitionChainStore.GetOpenSlot();
        protected virtual QState ActiveRecorder_Closing(IQEvent qEvent)
        {
            switch (qEvent.QSignal)
            {
                case (int)QSignals.Entry:
                    return null;

                case (int)InternalSignal.RecorderClosing:
                    Close((RecorderEvent)qEvent);
                    return null;

                case (int)QSignals.Exit:
                    return null;
            }
            return this.TopState;
        }
        protected virtual QState ActiveRecorder_Closed(IQEvent qEvent)
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
        protected virtual QState ActiveRecorder_Faulted(IQEvent qEvent)
        {
            switch (qEvent.QSignal)
            {
                case (int)QSignals.Entry:
                    Debug.Print("<Entered Faulted state. Object=" + this.GetType().Name + ", Name=" + Name + ">");
                    return null;
                case (int)QFSignal.RecorderClose:
                    Close((RecorderEvent)qEvent);
                    return null;
                case (int)QSignals.Exit:
                    return null;
            }
            return this.TopState;
        }
    }
}
