using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Collections.Generic;
using System.Xml;
using System.Xml.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using TruePosition.Test.Core;
using TruePosition.Test.DataLayer;
using TruePosition.Test.IO;
using TruePosition.Test.QF;
using TruePosition.Test.UI;
using TruePosition.Test.XML;
using TruePosition.Test.Process;
using TruePosition.Test.Prompt;
using TruePosition.Test.Recorder;
using System.Windows.Forms;

namespace UnitTests
{
    /// <summary>
    /// Summary description for UnitTest1
    /// </summary>
    [TestClass]
    public class DevTests
    {
        const string testXMLPath = @"..\..\..\Documents";
        const string originalXMLFile = testXMLPath + @"\lmu_dvt_testcases_all.xml";
        const string convertedXMLPath = testXMLPath + @"\Bootup Tests";
        const string COCommandXMLFile = "COCommandTest.xml";
        const string TestBootup1EXMLFile = "Test Case Bootup-1E.xml";
        const string ConfigPath = testXMLPath + @"\Config";

        public DevTests()
        {
            //
            // TODO: Add constructor logic here
            //
        }

        private TestContext testContextInstance;


        /// <summary>
        ///Gets or sets the test context which provides
        ///information about and functionality for the current test run.
        ///</summary>
        public TestContext TestContext
        {
            get
            {
                return testContextInstance;
            }
            set
            {
                testContextInstance = value;
            }
        }

        #region Additional test attributes
        //
        // You can use the following additional attributes as you write your tests:
        //
        // Use ClassInitialize to run code before running the first test in the class
        // [ClassInitialize()]
        // public static void MyClassInitialize(TestContext testContext) { }
        //
        // Use ClassCleanup to run code after all tests in a class have run
        // [ClassCleanup()]
        // public static void MyClassCleanup() { }
        //
        // Use TestInitialize to run code before running each test 
        // [TestInitialize()]
        // public void MyTestInitialize() { }
        //
        // Use TestCleanup to run code after each test has run
        // [TestCleanup()]
        // public void MyTestCleanup() { }
        //
        #endregion


        /// <summary>
        ///A test for TransformDocument
        ///</summary>
        [TestMethod()]
        public void TransformTest()
        {
            IEnumerable<XElement> actual = Transformer.Transform(originalXMLFile);
            Assert.IsNotNull(actual);
        }

        /// <summary>
        ///A test for Save
        ///</summary>
        [TestMethod()]
        public void SaveTest()
        {
            Transformer.Save(convertedXMLPath, Transformer.Transform(originalXMLFile));
        }

        /// <summary>
        ///A test for Hydrate
        ///</summary>
        [TestMethod()]
        public void HydrateTest()
        {
            IEnumerable<Test> actual = Hydrator.Hydrate(convertedXMLPath);
            Assert.IsNotNull(actual);
        }

        [TestMethod()]
        public void CopyTest()
        {
            Test test = Hydrator.Hydrate(testXMLPath, TestBootup1EXMLFile);

            Assert.IsNotNull(test);
            Test copy = test.Copy();
            copy.Save(testXMLPath, "copy output.xml");

            Step stepCopy = test.Steps[1].Copy((c) =>
                                                {
                                                    c.Retries = 3;
                                                    return c;
                                                });

            IEnumerable<Step> steps = test.Steps.CopyMany((c) =>
                                                            {
                                                                c.Retries = 3;
                                                                return c;
                                                            }).ToList();
            

            //Assert.IsTrue(actual.Steps[0].DeepEquals(copy));
        }

        /// <summary>
        ///A test for Evaluate
        ///</summary>
        [TestMethod()]
        public void EvaluateTest()
        {
            //string expression = "-1.0 <= value < 1.1";
            //string expression = "value = -10";
            //string expression = "value like '*[T]empurature:*rcvr*'";
            //string expression = "value same";
            //string expression = "30 = value";
            string expression = "value validesn";
            Evaluator target = new Evaluator(expression);
            //ValueHelper value = new ValueHelper(-10);
            //ValueHelper value = new ValueHelper(1.0000009);
            //ValueHelper value = new ValueHelper("sdfsdfsdfsdfsdfsTempurature:slkdkfhjfsklrcvdjfsdjfksdjdf");
            //ValueHelper value = new ValueHelper("ffffffffffffffffffff");
            ValueHelper value = new ValueHelper("06160301B010082006440005");
            //ValueHelper value = new ValueHelper(30);
            bool expected = true;
            bool actual = target.Evaluate(value);
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void TempuratureResponseTest()
        {
            XElement exerpt = XElement.Parse("<Test Type=\"Bootup\" Name=\"1A\">" +
                                                   "<Step Type=\"Bootup\">" +
                                                    "<Command>pro</Command>" +
                                                    "<Response Delimiter=\"&#xA;\">" +
                                                     "<Element>" +
                                                       "<KeyExpression>value like 'TEMPERATURES:*'</KeyExpression>" +
                                                       "<Expected Trim=\"true\">" +
                                                         "<KeyExpression>value like '*recvr*'</KeyExpression>" +
                                                         "<Expression>value &gt;= -10</Expression>" +
                                                         "<Expression>value &lt;= 70</Expression>" +
                                                         "<Destination>" +
                                                             "<Name></Name>" +
                                                             "<Default></Default>" +
                                                         "</Destination>" +
                                                         "<FailureMessage>Receiver Temperature outside limits</FailureMessage>" +
                                                       "</Expected>" +
                                                       "<Expected Trim=\"true\"> Trim=\"true\"" +
                                                         "<KeyExpression>value like '*bdc*'</KeyExpression>" +
                                                         "<Expression>value &gt;= -10</Expression>" +
                                                         "<Expression>value &lt;= 28</Expression>" +
                                                         "<Destination>" +
                                                             "<Name></Name>" +
                                                             "<Default></Default>" +
                                                         "</Destination>" +
                                                         "<FailureMessage>Receiver Temperature outside limits</FailureMessage>" +
                                                       "</Expected>" +
                                                       "<Expected Trim=\"true\">" +
                                                         "<KeyExpression>value like '*power supply*'</KeyExpression>" +
                                                         "<Expression>value &gt;= -10</Expression>" +
                                                         "<Expression>value &lt;= 35</Expression>" +
                                                         "<Destination>" +
                                                             "<Name></Name>" +
                                                             "<Default></Default>" +
                                                         "</Destination>" +
                                                         "<FailureMessage>Receiver Temperature outside limits</FailureMessage>" +
                                                       "</Expected>" +
                                                     "</Element>" +
                                                    "</Response>" +
                                                    "<Timeout>180</Timeout>" +
                                                    "<BeginState>BOOT</BeginState>" +
                                                    "<EndState>DSHELL</EndState>" +
                                                    "<Retries>0</Retries>" +
                                                  "</Step>" +
                                                "</Test>");

            Test test = Hydrator.Hydrate(exerpt);
            ResponseProcessor.Process("First Test", test.Steps[0], "TEMPERATURES: recvr 31 bdc 28 power supply 35");
        }

        [TestMethod]
        public void COResponseStepTest()
        {
            XElement exerpt = XElement.Parse("<TestSets>" +
                                                "<TestSet Name=\"Bootup\">" +
                                                    "<Test Type=\"Bootup\" Name=\"1A\">" +
                                                        "<Step Type=\"Bootup\">" +
                                                            "<Command>pro</Command>" +
                                                            "<Response Delimiter=\"&#xA;\">" +
                                                             "<Element>" +
                                                               "<KeyExpression>value like 'GPS RCVR:*'</KeyExpression>" +
                                                               "<Expected Trim=\"true\">" +
                                                                 "<Expression>value lenge 24</Expression>" +
                                                                 "<Expression>value validesn</Expression>" +
                                                                 "<Expression>value not same</Expression>" +
                                                                 "<Destination>" +
                                                                     "<Name></Name>" +
                                                                     "<Default></Default>" +
                                                                 "</Destination>" +
                                                                 "<FailureMessage></FailureMessage>" +
                                                               "</Expected>" +
                                                             "</Element>" +
                                                             "<Element>" +
                                                               "<KeyExpression>value like 'RECEIVER:*'</KeyExpression>" +
                                                               "<Expected Trim=\"true\">" +
                                                                 "<Expression>value len 24</Expression>" +
                                                                 "<Destination>" +
                                                                     "<Name></Name>" +
                                                                     "<Default></Default>" +
                                                                 "</Destination>" +
                                                                 "<FailureMessage></FailureMessage>" +
                                                               "</Expected>" +
                                                             "</Element>" +
                                                             "<Element>" +
                                                               "<KeyExpression>value like 'BDC:*'</KeyExpression>" +
                                                               "<Expected Trim=\"true\">" +
                                                                 "<Expression>value lenge 24</Expression>" +
                                                                 "<Destination>" +
                                                                     "<Name></Name>" +
                                                                     "<Default></Default>" +
                                                                 "</Destination>" +
                                                                 "<FailureMessage>Receiver Temperature outside limits</FailureMessage>" +
                                                               "</Expected>" +
                                                             "</Element>" +
                                                             "<Element>" +
                                                               "<KeyExpression>value like 'PSUPPLY:*'</KeyExpression>" +
                                                               "<Expected Trim=\"true\">" +
                                                                 "<Expression>value lenge 24</Expression>" +
                                                                 "<Destination>" +
                                                                     "<Name></Name>" +
                                                                     "<Default></Default>" +
                                                                 "</Destination>" +
                                                                 "<FailureMessage>Receiver Temperature outside limits</FailureMessage>" +
                                                               "</Expected>" +
                                                             "</Element>" +
                                                             "<Element>" +
                                                               "<KeyExpression>value like 'TEMPERATURES:*'</KeyExpression>" +
                                                               "<Expected Trim=\"true\">" +
                                                                 "<KeyExpression>value like '*recvr*'</KeyExpression>" +
                                                                 "<Expression>value &gt;= -10</Expression>" +
                                                                 "<Expression>value &lt;= 31</Expression>" +
                                                                 "<Destination>" +
                                                                     "<Name></Name>" +
                                                                     "<Default></Default>" +
                                                                 "</Destination>" +
                                                                 "<FailureMessage>Receiver Temperature outside limits</FailureMessage>" +
                                                               "</Expected>" +
                                                               "<Expected Trim=\"true\">" +
                                                                 "<KeyExpression>value like '*bdc*'</KeyExpression>" +
                                                                 "<Expression>value &gt;= -10</Expression>" +
                                                                 "<Expression>value &lt;= 28</Expression>" +
                                                                 "<Destination>" +
                                                                     "<Name></Name>" +
                                                                     "<Default></Default>" +
                                                                 "</Destination>" +
                                                                 "<FailureMessage>BDC Temperature outside limits</FailureMessage>" +
                                                               "</Expected>" +
                                                               "<Expected Trim=\"true\">" +
                                                                 "<KeyExpression>value like '*power supply*'</KeyExpression>" +
                                                                 "<Expression>value &gt;= -10</Expression>" +
                                                                 "<Expression>value &lt;= 35</Expression>" +
                                                                 "<Destination>" +
                                                                     "<Name></Name>" +
                                                                     "<Default></Default>" +
                                                                 "</Destination>" +
                                                                 "<FailureMessage>Power supply Temperature outside limits</FailureMessage>" +
                                                               "</Expected>" +
                                                             "</Element>" +
                                                             "<Element>" +
                                                               "<KeyExpression>value like 'CP/DSP:*'</KeyExpression>" +
                                                               "<Expected Trim=\"true\">" +
                                                                 "<Expression>value len 24</Expression>" +
                                                                 "<Destination>" +
                                                                     "<Name></Name>" +
                                                                     "<Default></Default>" +
                                                                 "</Destination>" +
                                                                 "<FailureMessage>Receiver Temperature outside limits</FailureMessage>" +
                                                               "</Expected>" +
                                                             "</Element>" +
                                                             "<Element>" +
                                                               "<KeyExpression>value like 'DCARD:*'</KeyExpression>" +
                                                               "<Expected Trim=\"true\">" +
                                                                 "<Expression>value lenge 24</Expression>" +
                                                                 "<Expression>value validesn</Expression>" +
                                                                 "<Expression>value not same</Expression>" +
                                                                 "<Destination>" +
                                                                     "<Name></Name>" +
                                                                     "<Default></Default>" +
                                                                 "</Destination>" +
                                                                 "<FailureMessage>Receiver Temperature outside limits</FailureMessage>" +
                                                               "</Expected>" +
                                                             "</Element>" +
                                                             "<Element>" +
                                                               "<KeyExpression>value like 'EBOARD:*'</KeyExpression>" +
                                                               "<Expected Trim=\"false\">" +
                                                                 "<Expression>value lenge 24</Expression>" +
                                                                 "<Destination>" +
                                                                     "<Name></Name>" +
                                                                     "<Default></Default>" +
                                                                 "</Destination>" +
                                                                 "<FailureMessage>Receiver Temperature outside limits</FailureMessage>" +
                                                               "</Expected>" +
                                                             "</Element>" +
                                                             "<Element>" +
                                                               "<KeyExpression>value like 'CUSTESN:*'</KeyExpression>" +
                                                               "<Expected Trim=\"true\">" +
                                                                 "<Expression>value lenge 15</Expression>" +
                                                                 "<Destination>" +
                                                                     "<Name></Name>" +
                                                                     "<Default></Default>" +
                                                                 "</Destination>" +
                                                                 "<FailureMessage>Receiver Temperature outside limits</FailureMessage>" +
                                                               "</Expected>" +
                                                             "</Element>" +
                                                             "<Element>" +
                                                               "<KeyExpression>value like 'TPESN:*'</KeyExpression>" +
                                                               "<Expected Trim=\"true\">" +
                                                                 "<Expression>value lenge 24</Expression>" +
                                                                 "<Destination>" +
                                                                     "<Name></Name>" +
                                                                     "<Default></Default>" +
                                                                 "</Destination>" +
                                                                 "<FailureMessage>Receiver Temperature outside limits</FailureMessage>" +
                                                               "</Expected>" +
                                                             "</Element>" +
                                                            "</Response>" +
                                                            "<Timeout>180</Timeout>" +
                                                            "<BeginState>BOOT</BeginState>" +
                                                            "<EndState>DSHELL</EndState>" +
                                                            "<Retries>0</Retries>" +
                                                        "</Step>" +
                                                    "</Test>" +
                                                  "</TestSet>" +
                                                "</TestSets>");

            Test test = Hydrator.Hydrate(exerpt);
            ResponseProcessor.Process("First Test", test.Steps[0], "ESN: " + "\n" +
                                                                   "GBE_CTLR:     NOT INSTALLED            " + "\n" +
                                                                   "GBE_LNA:      NOT INSTALLED            " + "\n" +
                                                                   "GBE:          NOT INSTALLED            " + "\n" +
                                                                   "GBE CUST ESN: NOT INSTALLED            " + "\n" +
                                                                   "GPS RCVR:     06162200D010082006501DA6 " + "\n" +
                                                                   "RECEIVER:     06163900B0100820070403C2 " + "\n" +
                                                                   "BDC:          06164000C11008200704063B " + "\n" +
                                                                   "PSUPPLY:      06163400G0100820064300C1 " + "\n" +
                                                                   "CP/DSP:       06164100B1100820064700C2 " + "\n" +
                                                                   "DCARD:        06160301B010082006440005 " + "\n" +
                                                                   "EBOARD:       NOT INSTALLED            " + "\n" +
                                                                   "CUSTESN:      TRULMU5207872AE          " + "\n" +
                                                                   "TEMPERATURES: recvr 31 bdc 28 power supply 35" + "\n" +
                                                                   "TPESN:        06630000D010082007050130");
                               //element.Save(@"C:\Development\Kapsoft\True Position\Documents\COCommandTest.xml");
        }

        [TestMethod]
        public void COResponseFileTest()
        {
            IEnumerable<Test> tests = Hydrator.Hydrate(COCommandXMLFile);
            //ResponseProcessor.BindEvents(tests, Assembly.GetAssembly(typeof(TruePosition.Test.Custom.CSharp.CustomCommandEvents))); //Assembly.GetAssembly(typeof(TruePosition.Test.Custom.CSharp.CustomCommandEvents)));

            Test test = tests.Single(t => t.Name == "1A"); 
            ResponseProcessor.Process("1A", test.Steps[0], "ESN: " + "\n" +
                                                           "GBE_CTLR:     NOT INSTALLED            " + "\n" +
                                                           "GBE_LNA:      NOT INSTALLED            " + "\n" +
                                                           "GBE:          NOT INSTALLED            " + "\n" +
                                                           "GBE CUST ESN: NOT INSTALLED            " + "\n" +
                                                           "LMU:          06162200D010082006501DA6 " + "\n" +
                                                           "GPS RCVR:     06162200D010082006501DA6 " + "\n" +
                                                           "RECEIVER:     06163900B0100820070403C2 " + "\n" +
                                                           "BDC:          06164000C11008200704063B " + "\n" +
                                                           "PSUPPLY:      06163400G0100820064300C1 " + "\n" +
                                                           "CP/DSP:       06164100B1100820064700C2 " + "\n" +
                                                           "DCARD:        06160301B010082006440005 " + "\n" +
                                                           "EBOARD:       NOT INSTALLED            " + "\n" +
                                                           "CUSTESN:      TRULMU5207872AE          " + "\n" +
                                                           "TEMPERATURES: recvr 31 bdc 28 power supply 35" + "\n" +
                                                           "TPESN:        06630000D010082007050130");
        }

        public class Value
        {
            public ResponseElement element;
            public ValueHelper value;
            public string rawElement;
            public int Number;
        }

        //[TestMethod]
        //public void L2OTest()
        //{
        //    TestSets testCollections = Hydrator.Hydrate("Bootup Test", COCommandXML);

        //    testCollections["Bootup"]["1A"].Steps[0].Response.Elements[3].RawElement = "Chicken!";
        //    testCollections["Bootup"]["1A"].Steps[0].Response.Elements[3].ExpectedResponses[0].Value = new ValueHelper(new object());
        //    testCollections["Bootup"]["1A"].Steps[0].Response.Elements[3].ExpectedResponses[0].Value.Value = "Chicken!";

        //    // TEST 1
        //    // - Queries that emit full range variables produce direct references to source data
        //    // - This allows source data to be modified directly
        //    //var value = (from e in testCollections["Bootup"]["1A"].Steps[0].Response.Elements
        //    //             where e.Key.RawExpression.Contains("RECEIVER")
        //    //             select e).SingleOrDefault();
        //    //value.RawElement = "This is a test.";
        //    //value.ExpectedResponses[0].Value.Value = "This is a test."; //new ValueHelper("This is a test.");
        //    //value.Number = 17;

        //    // TEST 2
        //    // - Queries that emit anonymous types or newly created instances do not allow modification of source types
        //    // - Additionally annonymous types are inherently read only
        //    var value = (from e in testCollections["Bootup"]["1A"].Steps[0].Response.Elements
        //                   where e.Key.RawExpression.Contains("RECEIVER")
        //                   select new //Value
        //                   {
        //                       element = e,
        //                       value = e.ExpectedResponses[0].Value,
        //                       rawElement = e.RawElement,
        //                       Number = e.Number
        //                   }).SingleOrDefault();
        //    value.element.RawElement = "This is a test.";
        //    //if (value.value == null)
        //    //    value.value.Value = "This is a test."; //new ValueHelper("This is a test.");
        //    value.Number = 17;
        //}

        private bool sendwait = true;
        private void PortTransmitted(object sender, PortMessageEventArgs args)
        {
            Debug.Print("<Event=PortDataTransmitted,\tPort=" + args.Name + ",\tMessage='" + args.Message + ">");
        }
        public void PortReceive(object sender, PortMessageEventArgs args)
        {
            string msg = args.Message;
            Debug.Print("<Event=PortReceive,\tPort=" + args.Name + ",\tMessage='" + args.Message + "'>");
        }
        public void PortError(object sender, PortErrorEventArgs args)
        {
            Debug.Print("<Event=PortError,\tPort=" + args.Name + ",\tError='" + args.Error + "'>");
        }
        public void PortTimeout(object sender, PortTimeoutExpiredEventArgs args)
        {
            //sendwait = false;
            Debug.Print("<Event=PortTimeout,\tPort=" + args.Name + ">");
        }
        [TestMethod]
        public void SerialTest()
        {
            SerialPort port = new SerialPort("COM1");
            port.DataReceived += new EventHandler<PortMessageEventArgs>(PortReceive);
            port.Error += new EventHandler<PortErrorEventArgs>(PortError);
            port.ReceiveTimeoutExpired += new EventHandler<PortTimeoutExpiredEventArgs>(PortTimeout);
            port.Open();
            Assert.AreEqual(PortState.Opened, port.State);
            bool xmit = port.Transmit("Chicken!");
            while (sendwait) { System.Threading.Thread.Sleep(1); }

            sendwait = true;
            xmit = port.Transmit("Is");
            while (sendwait) { System.Threading.Thread.Sleep(1); }

            sendwait = true;
            xmit = port.Transmit("good");
            while (sendwait) { System.Threading.Thread.Sleep(1); }

            sendwait = true;
            xmit = port.Transmit("for");
            while (sendwait) { System.Threading.Thread.Sleep(1); }

            sendwait = true;
            xmit = port.Transmit("lunch");
            while (sendwait) { System.Threading.Thread.Sleep(1); }
            System.Threading.Thread.Sleep(5000);
            //Assert.AreEqual(true, xmit);
            port.Dispose();
        }


        private void PromptShowing(string text, bool modal, ref Form form)
        {
            Debug.Print("<Event=prompt_Showing,\tText='" + text + ">");

            if (form == null)
                form = new Form();

            form.Text = text;
            if (modal)
            {
                form.ShowDialog();
                form.Dispose();
                form = null;
            }
            else
            {
                form.Show();
            }
        }
        private void PromptClosed(object sender, PromptEventArgs args)
        {
            Debug.Print("<Event=prompt_Closed,\tPrompt=" + args.Name + ",\tText='" + args.Text + ">");
        }
        private void PromptError(object sender, PromptErrorEventArgs args)
        {
            Debug.Print("<Event=prompt_Error,\tPrompt=" + args.Name + ",\tError='" + args.Error + "',\tAdditional=" + args.AdditionalDescription + ">");
        }
        [TestMethod]
        public void UserPromptTest()
        {
            UserPrompt prompt = new UserPrompt("WaitingOnYou");
            prompt.Showing += new PromptShowingDelegate(PromptShowing);
            prompt.Show("Waiting on you.", 10);
            System.Threading.Thread.Sleep(System.Threading.Timeout.Infinite);
        }

        private string TDOAResponse = "TDOA command: channel = 187, ifType= 1, numberSample = 512, blockCount = 16" +
                                        "ADG cdd9f938, TS ce634d78" +
                                        "Send RF Tuning to Utility Services task" +
                                        "Send RF coll data to UCC task, dsp 0" +
                                        "StoreTransId" +
                                        "StoreTransId: i = 0, trans_id = 711" +
                                        "Go to task delay" +
                                        "Format and send collect data to dsp 0" +
                                        "Format and send tdoa data request to dsp 0" +
                                        "" +
                                        "TDOA 0 P 0 B 1 C 0 G -32768,0 V 16 W 512 |%6A\\B5\\;#HTO%9Z0CP].E=\\?'>]>ZF|" +
                                        "LMU>" +
                                        "TDOA 0 P 0 B 1 C 1 G -32768,0 V 16 W 512 |ZR3N?.K9[MCJF.\\FZD_O@.GW\\ OGV_| " +
                                        "LMU>" +
                                        "TDOA 0 P 0 B 2 C 0 G -32768,0 V 16 W 512 |\\YX81_0 &'KT.!B/]' 8I?2T&,?$9]| " +
                                        "LMU>" +
                                        "TDOA 0 P 0 B 2 C 1 G -32768,0 V 16 W 512 |&H$&,QJD!9,:O04N&L %$AK#!28P" +
                                        "LMU>" +
                                        "TDOA 0 P 0 B 3 C 0 G -32768,0 V 16 W 512 | 7CDR@$8Y,4 L^3* \"SDQO^AY,_GE$| " +
                                        "LMU>" +
                                        "TDOA 0 P 0 B 3 C 1 G -32768,0 V 16 W 512 |Y6<%K.6&!D?EE@:AY:(&S.6I!NSE>.| " +
                                        "LMU>" +
                                        "TDOA 0 P 0 B 4 C 0 G -32768,0 V 16 W 512 |\"S$8V N6&*X+PAB8\"48E O/&(X@6Z| " +
                                        "LMU>" +
                                        "TDOA 0 P 0 B 4 C 1 G -32768,0 V 16 W 512 |%A;P!Q7C[[\\5RN^>%:OO?A5U[S,5_L| " +
                                        "LMU>" +
                                        "TDOA 0 P 0 B 5 C 0 G -32768,0 V 16 W 512 |Z]3MM>M][@SK2NY+ZPONI^JJ[QKJ;P| " +
                                        "LMU>" +
                                        "TDOA 0 P 0 B 5 C 1 G -32768,0 V 16 W 512 |\\HT7L_+F%^7S2Q@?\\[08[V7!EU]J@9| " +
                                        "LMU>" +
                                        "TDOA 0 P 0 B 6 C 0 G -32768,0 V 16 W 512 |&A$'_AHG!ZX:. =K&E,'!AIM.L&P\\#| " +
                                        "LMU>" +
                                        "TDOA 0 P 0 B 6 C 1 G -32768,0 V 16 W 512 | D'DV@'MY-H!<N3. ,CDR  8Y,G6SD| " +
                                        "LMU>" +
                                        "TDOA 0 P 0 B 7 C 0 G -32768,0 V 16 W 512 |Y1@$.^4X!.3E2 5-Y6 %L^ 0@*YA (| " +
                                        "LMU>";
        void recorder_Opened(object sender, RecorderEventArgs e)
        {
            Debug.Print("<Event=recorder_Opened,\tName=" + e.Name + "\tFilename='" + e.FileName + "\tPath=" + e.Path + ">");
        }
        void recorder_Recording(string testName, int stepNumber, string format, IEnumerable<XElement> values, string command, string response, ref string entry)
        {
            Debug.Print("<Event=recorder_Recording,\tTest=" + testName + "',\tNumber=" + stepNumber + ",\tFormat='" + format + ",\tCommand='" + command + ",\tResponse='" + response + ">");
        }
        void recorder_Timeout(object sender, RecorderEventArgs e)
        {
            Debug.Print("<Event=recorder_Timeout,\tName=" + e.Name + "\tFilename='" + e.FileName + "\tPath=" + e.Path + ">");
        }
        void  recorder_Error(object sender, RecorderErrorEventArgs e)
        {
            Debug.Print("<Event=recorder_Error,\tName=" + e.Name + "\tError='" + e.Error + "\tAdditional=" + e.AdditionalDescription + ">");
        }
        void  recorder_Closed(object sender, RecorderEventArgs e)
        {
            Debug.Print("<Event=recorder_Closed,\tName=" + e.Name + "\tFilename='" + e.FileName + "\tPath=" + e.Path + ">");
        }
        [TestMethod]
        public void FileRecorderTest()
        {
            FileRecorder recorder = new FileRecorder("SomeFile");
            recorder.Opened += new EventHandler<RecorderEventArgs>(recorder_Opened);
            recorder.Closed += new EventHandler<RecorderEventArgs>(recorder_Closed);
            recorder.Error += new EventHandler<RecorderErrorEventArgs>(recorder_Error);

            recorder.Open(testXMLPath + @"\SomeFile");
            recorder.Record(TDOAResponse);
            recorder.Close();

        }
        [TestMethod]
        public void ActiveRecorderTest()
        {
            QFManager.Recorder.Create(RecorderType.File, "SomeFile");
            QFManager.Recorder.Opened("SomeFile", new EventHandler<RecorderEventArgs>(recorder_Opened));
            QFManager.Recorder.Closed("SomeFile", new EventHandler<RecorderEventArgs>(recorder_Closed));
            QFManager.Recorder.Error("SomeFile", new EventHandler<RecorderErrorEventArgs>(recorder_Error));

            List<string> expressions = new List<string>();
            expressions.Add("value like '*TDOA Command*'");
            QFManager.Recorder.Open("SomeFile", testXMLPath + @"\SomeFile", expressions);

            for (int i = 0; i < 10; i++)
                QFManager.Recorder.Record("Entry #" + i + " " + TDOAResponse + "\r\n\r\n");

            //System.Threading.Thread.Sleep(System.Threading.Timeout.Infinite);
            System.Threading.Thread.Sleep(2000);
            QFManager.Recorder.Close("SomeFile");
        }

        [TestMethod]
        public void ActivePromptTest()
        {
            QFManager.Prompt.Create("WaitingforYou");
            QFManager.Prompt.Showing("WaitingforYou", PromptShowing);
            QFManager.Prompt.Closed("WaitingforYou", PromptClosed);
            QFManager.Prompt.Show("WaitingforYou", "Waiting. Still waiting.", 5);
            System.Threading.Thread.Sleep(System.Threading.Timeout.Infinite);
        }

        [TestMethod]
        public void ActivePortSerialTest()
        {
            //QFManager.Port.Create(PortType.Serial, "COM1");

            ISerialConfig port = null;
            ConfigManager.SetLocation(ConfigPath);
            ConfigManager.Load("LMU1", out port);
            QFManager.Port.Create((IPort)port);

            QFManager.Port.Transmitted("LMU1", PortTransmitted);
            QFManager.Port.Receive("LMU1", PortReceive);
            QFManager.Port.Error("LMU1", PortError);
            QFManager.Port.ReceiveTimeout("LMU1", PortTimeout);
            QFManager.Port.Open("LMU1");
            QFManager.Port.Transmit("LMU1", "The Chicken has landed!");
            System.Threading.Thread.Sleep(5000);
            QFManager.Port.Close("LMU1");
            System.Threading.Thread.Sleep(System.Threading.Timeout.Infinite);
        }
        [TestMethod]
        public void ActivePortSNMPTest()
        {
            QFManager.Port.Create(PortType.Snmp, "SMLC1");
            QFManager.Port.Receive("SMLC1", PortReceive);
            QFManager.Port.Error("SMLC1", PortError);
            QFManager.Port.ReceiveTimeout("SMLC1", PortTimeout);
            QFManager.Port.Open("SMLC1");
            QFManager.Port.Transmit("SMLC1", "-Get -c=64.3.1.37 -V=v2 192.168.173.173 1.3.6.1.4.1.12060.1.2.5.1.2.1");
            System.Threading.Thread.Sleep(5000);
            QFManager.Port.Close("SMLC1");
            System.Threading.Thread.Sleep(System.Threading.Timeout.Infinite);
        }

        public void TestStepping(string testName, Step step, ref string augmentedCommand)
        {
            Debug.Print("<Event=test_Stepping,\tTest=" + testName + ",\tCommand='" + step.Actor.Action + "',\tNumber=" + step.Number + ">");

            string newCommand = null;
            if (step.Actor.Action == "?CO")
                newCommand = step.Actor.Action + " XXX";
            else if (step.Actor.Action == "G")
                newCommand = step.Actor.Action + " 40.064618 -75.460373 0 53";

            if (!string.IsNullOrEmpty(newCommand))
                augmentedCommand = newCommand;
        }
        public void TestError(object sender, TestErrorEventArgs args)
        {
            Debug.Print("<Event=test_Error,\tTest=" + args.Name + ",\tCommand='" + args.Step.Actor.Action + "',\tNumber=" + args.Step.Number + ",\tReason=" + args.Reason + ">");
        }
        public void TestStepped(object sender, TestSteppedEventArgs args)
        {
            Debug.Print("<Event=test_Stepped,\tTest=" + args.Name + ",\tCommand='" + args.Step.Actor.Action + "',\tNumber=" + args.Step.Number + ">");
        }
        public void TestPassed(object sender, TestPassedEventArgs args)
        {
            Debug.Print("<Event=test_Passed,\tTest=" + args.Name + ">");
        }
        static bool once = false;
        public void TestUnloaded(object sender, TestUnloadedEventArgs args)
        {
            Debug.Print("<Event=test_Unloaded,\tTest=" + args.Name + ">");

            //if (!once)
            //{
            //    once = true;
            //    System.Threading.ThreadPool.QueueUserWorkItem((s) =>
            //                                                    {
            //                                                        //System.Threading.Thread.Sleep(2000);
            //                                                        //Test test = Hydrator.Hydrate(testXMLPath, "Test Case - Test 21a.xml");
            //                                                        //TestManager.Run(test, HandlerHelper.Handlers, ConfigPath, null, UUTTestMode.Direct);
            //                                                        QFManager.UUT.End("LMU1");
            //                                                        Test21aTest();
            //                                                    });
            //}
        }
        public void TestFailed(object sender, TestFailedEventArgs args)
        {
            Debug.Print("<Event=test_Failed,\tTest=" + args.Name + ",\tCommand='" + args.Step.Actor.Action + "',\tNumber=" + args.Step.Number + ",\tReason=" + args.Reason + ">");
        }
        public void TestAborted(object sender, TestAbortedEventArgs args)
        {
            Debug.Print("<Event=test_Aborted,\tTest=" + args.Name + ",\tReason=" + args.Reason + ">");
        }

        [TestMethod]
        public void ActiveTestTest()
        {
            Test test = Hydrator.Hydrate(testXMLPath, COCommandXMLFile);
            //ResponseProcessor.BindEvents(test, Assembly.GetAssembly(typeof(TruePosition.Test.Custom.CSharp.CustomCommandEvents)));

            QFManager.Port.Create(PortType.Serial, "COM1");
            QFManager.Port.Receive("COM1", PortReceive);
            QFManager.Port.Error("COM1", PortError);
            QFManager.Port.ReceiveTimeout("COM1", PortTimeout);
            QFManager.Port.Open("COM1");

            QFManager.Test.Load(test);
            QFManager.Test.Failed(test.Name, TestFailed);
            QFManager.Test.Passed(test.Name, TestPassed);
            QFManager.Test.Run(test.Name);

            System.Threading.Thread.Sleep(System.Threading.Timeout.Infinite);
        }

        private void uut_Failed(object sender, TestFailedEventArgs args)
        {
            Debug.Print("<Event=uut_Failed,\tUUT=" + args.Name + ",\tCommand='" + args.Step.Actor.Action + "',\tNumber=" + args.Step.Number + ",\tReason=" + args.Reason + ">");
        }
        private void uut_Passed(object sender, TestPassedEventArgs args)
        {
            Debug.Print("<Event=uut_Passed,\tUUT=" + args.Name + ">");
        }

#if UUT_TEST
        [TestMethod]
        public void UUTCOCommandTest()
        {
            Test test = Hydrator.Hydrate(testXMLPath, COCommandXMLFile);
            //ResponseProcessor.BindEvents(test, Assembly.GetAssembly(typeof(TruePosition.Test.Custom.CSharp.CustomCommandEvents)));
            //ResponseProcessor.BindEvents(test, Assembly.LoadFile(@"C:\Development\Kapsoft\True Position\Test Components\Output\Out.dll"));

            QFManager.Port.Create(PortType.Serial, "COM1");
            QFManager.Port.Receive("COM1", PortReceive);
            QFManager.Port.Error("COM1", PortError);
            QFManager.Port.ReceiveTimeout("COM1", PortTimeout);
            QFManager.Port.Open("COM1");

            QFManager.Test.Load(test, UUTTestMode.Direct);
            QFManager.Test.Error(test.Name, TestError);
            QFManager.Test.Stepping(test.Name, TestStepping);
            QFManager.Test.Stepped(test.Name, TestStepped);
            QFManager.Test.Passed(test.Name, TestPassed);
            QFManager.Test.Failed(test.Name, TestFailed);

            QFManager.UUT.Load("LMU1", @"..\..\..\Documents\\UUT CO Response.xml", test, UUTTestMode.Direct);
            QFManager.UUT.Failed("LMU1", uut_Failed);
            QFManager.UUT.Passed("LMU1", uut_Passed);

            QFManager.UUT.Run("LMU1");
            QFManager.Test.Run(test.Name);

            System.Threading.Thread.Sleep(System.Threading.Timeout.Infinite);
        }
        [TestMethod]
        public void UUTBootup1ETest()
        {
            Test test = Hydrator.Hydrate(testXMLPath, TestBootup1EXMLFile);

            QFManager.Port.Create(PortType.Serial, "COM1");
            QFManager.Port.Receive("COM1", PortReceive);
            QFManager.Port.Error("COM1", PortError);
            QFManager.Port.ReceiveTimeout("COM1", PortTimeout);
            QFManager.Port.Open("COM1");

            QFManager.Test.Load(test, UUTTestMode.Direct);
            QFManager.Test.Stepping(test.Name, TestStepping);
            QFManager.Test.Error(test.Name, TestError);
            QFManager.Test.Stepped(test.Name, TestStepped);
            QFManager.Test.Passed(test.Name, TestPassed);
            QFManager.Test.Failed(test.Name, TestFailed);
            QFManager.Test.Aborted(test.Name, TestAborted);

            //QFManager.Test.Abort(test.Name, "Abort before we even got started.");
            //System.Threading.Thread.Sleep(System.Threading.Timeout.Infinite);

            QFManager.UUT.Load("LMU1", @"..\..\..\Documents\\UUT Bootup-1E Responses.xml", test, UUTTestMode.Direct);
            QFManager.UUT.Failed("LMU1", uut_Failed);
            QFManager.UUT.Passed("LMU1", uut_Passed);

            QFManager.UUT.Run("LMU1");
            QFManager.Test.Run(test.Name);

            System.Threading.Thread.Sleep(System.Threading.Timeout.Infinite);

            //System.Threading.Thread.Sleep(1000);
            //QFManager.Test.Abort(test.Name, "Because I wanted to. That's why...");
        }

        private void HookHandlers()
        {
            HandlerHelper.TestStepping += TestStepping;
            HandlerHelper.TestError += TestError;
            HandlerHelper.TestStepped += TestStepped;
            HandlerHelper.TestPassed += TestPassed;
            HandlerHelper.TestFailed += TestFailed;
            HandlerHelper.TestAborted += TestAborted;
            HandlerHelper.TestUnloaded += TestUnloaded;
            HandlerHelper.ProcessLaunched += ProcessLaunched;
            HandlerHelper.ProcessKilled += ProcessKilled;
            HandlerHelper.ProcessTimeout += ProcessTimeout;
            HandlerHelper.ProcessError += ProcessError;
            HandlerHelper.PromptError += PromptError;
            HandlerHelper.PromptShowing += PromptShowing;
            HandlerHelper.PromptClosed += PromptClosed;
            HandlerHelper.RecorderOpened += recorder_Opened;
            HandlerHelper.RecorderRecording += recorder_Recording;
            HandlerHelper.RecorderClosed += recorder_Closed;
            HandlerHelper.RecorderError += recorder_Error;
        }

        [TestMethod]
        public void TestManagerRunTest()
        {
            //Test test = Hydrator.Hydrate(testXMLPath, COCommandXMLFile);
            Test test = Hydrator.Hydrate(testXMLPath, TestBootup1EXMLFile);

            //QFManager.UUT.Load("LMU1", @"..\..\..\Documents\UUT CO Response.xml", test, UUTTestMode.Port);
            QFManager.UUT.Load("LMU1", @"..\..\..\Documents\UUT Bootup-1E Responses.xml", test, UUTTestMode.Direct);
            QFManager.UUT.Failed("LMU1", uut_Failed);
            QFManager.UUT.Passed("LMU1", uut_Passed);
            QFManager.UUT.Run("LMU1");

            HookHandlers();

            string path = @"C:\Development\Kapsoft\True Position\Test Components\Output";
            TestManager.Run(test, HandlerHelper.Handlers, ConfigPath, null, UUTTestMode.Direct);

            //System.Threading.Thread.Sleep(1000);
            //TestManager.Abort(test, "Abort before we even got started.");

            System.Threading.Thread.Sleep(System.Threading.Timeout.Infinite);
        }
        [TestMethod]
        public void Test21aTest()
        {
            //Test test = Hydrator.Hydrate(testXMLPath, COCommandXMLFile);
            Test test = Hydrator.Hydrate(testXMLPath, "Test Case - Test 21a.xml");

            //QFManager.UUT.Load("LMU1", @"..\..\..\Documents\UUT CO Response.xml", test, UUTTestMode.Port);
            QFManager.UUT.Load("LMU1", testXMLPath + @"\UUT Test 21a Responses.xml", test, UUTTestMode.Direct);
            QFManager.UUT.Failed("LMU1", uut_Failed);
            QFManager.UUT.Passed("LMU1", uut_Passed);
            QFManager.UUT.Run("LMU1");

            HookHandlers();

            string path = @"C:\Development\Kapsoft\True Position\Test Components\Output";
            TestManager.Run(test, HandlerHelper.Handlers, ConfigPath, null, UUTTestMode.Direct);

            //System.Threading.Thread.Sleep(1000);
            //TestManager.Abort(test, "Abort before we even got started.");

            System.Threading.Thread.Sleep(System.Threading.Timeout.Infinite);
        }

        [TestMethod]
        public void TestTDOATest()
        {
            //Test test = Hydrator.Hydrate(testXMLPath, COCommandXMLFile);
            Test test = Hydrator.Hydrate(testXMLPath, "Test Case - TDOA.xml");

            //QFManager.UUT.Load("LMU1", @"..\..\..\Documents\UUT CO Response.xml", test, UUTTestMode.Port);
            QFManager.UUT.Load("LMU1", testXMLPath + @"\UUT TDOA Responses.xml", test, UUTTestMode.Direct);
            QFManager.UUT.Failed("LMU1", uut_Failed);
            QFManager.UUT.Passed("LMU1", uut_Passed);
            QFManager.UUT.Run("LMU1");

            HookHandlers();

            string path = @"C:\Development\Kapsoft\True Position\Test Components\Output";
            TestManager.Run(test, HandlerHelper.Handlers, ConfigPath, null, UUTTestMode.Direct);

            //System.Threading.Thread.Sleep(1000);
            //TestManager.Abort(test, "Abort before we even got started.");

            System.Threading.Thread.Sleep(System.Threading.Timeout.Infinite);
        }
        //[TestMethod]
        //public void PropertyPageTest()
        //{
        //    string testXMLPath = @"..\..\..\Documents";
        //    string ConfigPath = testXMLPath + @"\Config";
        //    ConfigManager.SetLocation(config)

        //    string Name = "LMU2";
        //    ISerialConfig serialConfig = null;

        //    if (!ConfigManager.Exists(Name))
        //        ConfigManager.Create(Name, out serialConfig);
        //    else
        //        ConfigManager.Load(Name, out serialConfig);

        //    DockSample.SerialConfigProperties properties = new DockSample.SerialConfigProperties(Name, serialConfig);
        //    ConfigManager.Save(Name, properties);
        //}
#else

        [TestMethod]
        public void RunTest()
        {
            Test test = Hydrator.Hydrate(testXMLPath, "Test 1A.xml");
            //Test test = Hydrator.Hydrate(testXMLPath, TestBootup1EXMLFile);

            HandlerHelper.TestStepping += TestStepping;
            HandlerHelper.TestError += TestError;
            HandlerHelper.TestStepped += TestStepped;
            HandlerHelper.TestPassed += TestPassed;
            HandlerHelper.TestFailed += TestFailed;
            HandlerHelper.TestAborted += TestAborted;
            HandlerHelper.PortReceive += PortReceive;
            HandlerHelper.PortError += PortError;
            HandlerHelper.PortTimeout += PortTimeout;
            HandlerHelper.ProcessLaunched += ProcessLaunched;
            HandlerHelper.ProcessKilled += ProcessKilled;
            HandlerHelper.ProcessTimeout += ProcessTimeout;
            HandlerHelper.ProcessError += ProcessError;
            HandlerHelper.PromptError += PromptError;
            HandlerHelper.PromptShowing += PromptShowing;
            HandlerHelper.PromptClosed += PromptClosed;
            HandlerHelper.RecorderOpened += recorder_Opened;
            HandlerHelper.RecorderClosed += recorder_Closed;
            HandlerHelper.RecorderError += recorder_Error;

            TestManager.Run(test, HandlerHelper.Handlers, ConfigPath);
            System.Threading.Thread.Sleep(System.Threading.Timeout.Infinite);
        }
#endif

        [TestMethod]
        public void TestSNMPGet()
        {
            SnmpPort port = new SnmpPort("SMLC1");
            port.Transmit("-Get -c=64.3.1.37 -V=v2 192.168.173.173 1.3.6.1.4.1.12060.1.2.5.1.2.1");
        }
        [TestMethod]
        public void TestSNMPSet()
        {
            SnmpPort port = new SnmpPort("SMLC1");
            port.Transmit("-Set -c=64.3.1.37 -V=v2 192.168.173.173 1.3.6.1.4.1.12060.1.2.5.1.2.1 i 43");
        }

        [TestMethod]
        public void ProcessTest()
        {
            //WindowsProcess process = new WindowsProcess("IE");
            //process.Path = @"C:\Program Files\Internet Explorer";
            //process.ExecutableName = "iexplore.exe";

            WindowsProcess process = new WindowsProcess("LaunchTest");

            //process.LaunchDelay = new TimeSpan(0, 0, 20);
            process.LaunchTimeout = 5000;
            process.Error += new EventHandler<ProcessErrorEventArgs>(ProcessError);
            process.Killed += new EventHandler<ProcessEventArgs>(ProcessKilled);
            process.Launched += new EventHandler<ProcessEventArgs>(ProcessLaunched);
            process.Timeout += new EventHandler<ProcessEventArgs>(ProcessTimeout);
            //process.Launch(@"C:\Development\Kapsoft\True Position\Test Components\Test\LaunchTest\bin\Debug\launchtest.exe");
            process.Launch(@"C:\Program Files\Internet Explorer\iexplore.exe");
            process.Kill();
        }
        private static void ProcessLaunched(object sender, ProcessEventArgs args)
        {
            Debug.Print("<Event=process_Launched,\tProcess=" + args.Name + ",\tProcess Name='" + args.ProcessName + "'>");
        }
        private static void ProcessKilled(object sender, ProcessEventArgs args)
        {
            Debug.Print("<Event=process_Killed,\tProcess=" + args.Name + ",\tProcess Name='" + args.ProcessName + "'>");
        }
        private static void ProcessTimeout(object sender, ProcessEventArgs args)
        {
            Debug.Print("<Event=process_Timeout,\tProcess=" + args.Name + ",\tProcess Name='" + args.ProcessName + "'>");
        }
        private static void ProcessError(object sender, ProcessErrorEventArgs args)
        {
            Debug.Print("<Event=process_Error,\tProcess=" + args.Name + ",\tError='" + args.Error + "',\tAdditional='" + args.AdditionalDescription + "'>");
        }

        SomeThing thing = null;
        System.Threading.Timer timer = null;
        void callback(object state)
        {
            string test = thing.bigString;
            timer.Dispose();
        }

        [TestMethod]
        public void DisposeTest()
        {
            thing = new SomeThing();
            timer = new System.Threading.Timer(new System.Threading.TimerCallback(callback), null, 35000, System.Threading.Timeout.Infinite);
            thing.Dispose();

            GC.Collect();
            GC.WaitForPendingFinalizers();
            GC.Collect();
            System.Threading.Thread.Sleep(1000);

            GC.Collect();
            GC.WaitForPendingFinalizers();
            GC.Collect();
            System.Threading.Thread.Sleep(1000);

            GC.Collect();
            GC.WaitForPendingFinalizers();
            GC.Collect();
            System.Threading.Thread.Sleep(System.Threading.Timeout.Infinite);
        }
    }

    public class SomeThing : IDisposable
    {
        public string bigString = new string(' ', 1024 * 1024);
        public void Dispose() { }
    }
}
