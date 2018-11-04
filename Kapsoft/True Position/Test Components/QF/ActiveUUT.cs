using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Xml.Linq;

using qf4net;
using TruePosition.Test.DataLayer;

namespace TruePosition.Test.QF
{
#if UUT_TEST
    internal class ActiveUUT : ActiveTest
    {
        #region Boiler plate static stuff

        private static new TransitionChainStore s_TransitionChainStore =
            new TransitionChainStore(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        static ActiveUUT()
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

        string ResponseFilePath { get; set; }
        string TestSetName { get; set; }
        IEnumerable<XElement> Steps { get; set; }
        string CurrentResponse { get; set; }
        int ResponseCount { get; set; }
        readonly XNamespace Namespace = "";
        XElement Responses { get; set; }

        //private string[] responses = { "ESN: " + "\r\n" +
        //                              "GBE_CTLR:     NOT INSTALLED            " + "\r\n" +
        //                              "GBE_LNA:      NOT INSTALLED            " + "\r\n" +
        //                              "GBE:          NOT INSTALLED            " + "\r\n" +
        //                              "GBE CUST ESN: NOT INSTALLED            " + "\r\n" +
        //                              "LMU:          06162200D010082006501DA6 " + "\r\n" +
        //                              "GPS RCVR:     06162200D010082006501DA6 " + "\r\n" +
        //                              "RECEIVER:     06163900B0100820070403C2 " + "\r\n" +
        //                              "BDC:          06164000C11008200704063B " + "\r\n" +
        //                              "PSUPPLY:      06163400G0100820064300C1 " + "\r\n" +
        //                              "CP/DSP:       06164100B1100820064700C2 " + "\r\n" +
        //                              "DCARD:        06160301B010082006440005 " + "\r\n" +
        //                              "EBOARD:       NOT INSTALLED            " + "\r\n" +
        //                              "CUSTESN:      TRULMU5207872AE          " + "\r\n" +
        //                              "TEMPERATURES: recvr 31 bdc 28 power supply 35" + "\r\n" +
        //                              "TPESN:        06630000D010082007050130" + "\r\n" + 
        //                              "LMU>\r\n"};

        public ActiveUUT(string uutName, string responseFilePath, string testCollectionName, DataLayer.Test test, UUTTestMode mode) 
            : base(test, mode) 
        {
            Name = uutName;
            TestSetName = testCollectionName;
            ResponseFilePath = responseFilePath;

            qf4net.QF.Instance.Subscribe(this, (int)QFSignal.TestStep);
            qf4net.QF.Instance.Subscribe(this, (int)QFSignal.TestResult);
        }
        public ActiveUUT(string uutName, string responseFilePath, DataLayer.Test test, UUTTestMode mode)
            : this(uutName, responseFilePath, null, test, mode)
        {
        }

        public override string ToString()
        {
            return "ActiveUUT " + Name;
        }

        protected override void Run(TestEvent tEvent)
        {
            if (tEvent.Name == Name)
            {
                try
                {
                    Responses = XElement.Load(ResponseFilePath);

                    if (string.IsNullOrEmpty(TestSetName))
                        Steps = (from tc in Responses.Descendants(Namespace + "Test")
                                 where tc.Attribute("Name").Value == Test.Name
                                 select tc).SingleOrDefault().Descendants(Namespace + "Step");
                    else
                        Steps =  (from cases in (from tc in Responses.Descendants(Namespace + "TestSet")
                                                 where tc.Attribute("Name").Value == TestSetName
                                                 select tc).SingleOrDefault().Descendants(Namespace + "Test")
                                  where cases.Attribute("Name").Value == Test.Name
                                  select cases).SingleOrDefault().Descendants(Namespace + "Step");


                    ResponseCount = Steps.Count();

                    CurrentStep = 0;
                    TransitionTo(m_StateStepping, s_TranIdx_Running_Stepping);
                }
                catch (Exception ex)
                {
                    TransitionToFailed(ex.Message);
                }
            }
        }

        protected virtual string FindResponse(string command)
        {
            XElement response = (from s in Steps
                                 where s.Element(Namespace + "Command").Value == command
                                 select s.Element(Namespace + "Response")).SingleOrDefault();

            if (response == null)
            {
            }


            string value = response.Attribute("Header").Value + response.Descendants(Namespace + "Element").Aggregate(string.Empty, (agg, n) => agg + n.Value + response.Attribute(Namespace + "Delimiter").Value) + response.Attribute("Trailer").Value;

            System.Threading.Thread.Sleep(Convert.ToInt32(response.Attribute("Delay").Value) * 1000);

            return value;
        }

        protected override void Step(TestEvent tEvent, int retries)
        {
            if (tEvent.Name == Name)
            {
                Retries = retries;
                switch (Mode)
                { 
                    case UUTTestMode.Port:
                        qf4net.QF.Instance.Publish(new PortEvent(QFSignal.PortTransmit,
                                                                 Test.Steps[CurrentStep].Actor.Name,
                                                                 CurrentResponse));
                        break;

                    case UUTTestMode.Direct:
                        string delimiter = "\r\n";
                        if (Test.Steps[CurrentStep].Response != null)
                            delimiter = Test.Steps[CurrentStep].Response.Delimiter;

                        foreach (string element in CurrentResponse.Split(new string[] { delimiter }, StringSplitOptions.RemoveEmptyEntries))
                            qf4net.QF.Instance.Publish(new PortEvent(QFSignal.PortResponse,
                                                                     Test.Steps[CurrentStep].Actor.Name,
                                                                     element));
                        break;
                }
            }
        }
        protected virtual void NextStep()
        {
            Retries = 0;
            CurrentStep++;
        }
        protected override bool PortResponse(PortEvent pEvent)
        {
            if (pEvent.Name == Test.Steps[CurrentStep].Actor.Name)
            {
                try
                {
                    if ((!string.IsNullOrEmpty(CurrentResponse)) && (CurrentResponse.Contains(pEvent.Message)))
                        return false;

                    // TEST ONLY...
                    //System.Threading.Thread.Sleep(100);

                    // DESIGN NOTE:
                    // - Finding the appropriate response by command allows the responses to be out of sequence with the steps of a test
                    // PROBLEM: Each response must be unique to allow lookup. This means if a test contains duplicate commands the count of responses will be less than the count of steps in the test.
                    CurrentResponse = FindResponse(pEvent.Message == Test.Steps[CurrentStep].Actor.Action ? Test.Steps[CurrentStep].Actor.Action : pEvent.Message.IndexOf(' ') == -1 ? pEvent.Message : pEvent.Message.Substring(0, pEvent.Message.IndexOf(' ')).Trim());
                    Step(new TestEvent(InternalSignal.TestStep, Name), Retries);
                    if (++Retries > Test.Steps[CurrentStep].Retries)
                    {
                        NextStep();
                    }
                }
                catch (Exception ex)
                {
                    TransitionToFailed(ex.Message);
                }
            }
            return false;
        }

        protected override void Response(IQEvent qEvent)
        {
            IActor actor = (IActor)qEvent;
            if (actor.Name == Test.Steps[CurrentStep].Actor.Name)
            {
                try
                {
                    switch (actor.Type)
                    {
                        case ActorType.Port:
                            PortResponse((PortEvent)qEvent);
                            break;
                        case ActorType.Process:
                            break;
                    }

                }
                catch (Exception ex)
                {
                    TransitionToFailed(ex.Message);
                }
            }
        }

        protected override QState ActiveTest_Running(IQEvent qEvent)
        {
            QState result = base.ActiveTest_Running(qEvent);
            if (result == null)
                return null;
            else
            {
                switch (qEvent.QSignal)
                {
                    case (int)QFSignal.TestStep:
                        NextStep();
                        return null;
                    // DESIGN NOTE: Wait for the associated ActiveTest to complete...
                    case (int)QFSignal.TestResult:
                        if (string.IsNullOrEmpty(((TestEvent)qEvent).Reason))
                            TransitionToPassed();
                        else
                            TransitionToFailed(((TestEvent)qEvent).Reason);
                        return null;
                }
                return result;
            }
        }
    }
#endif
}
