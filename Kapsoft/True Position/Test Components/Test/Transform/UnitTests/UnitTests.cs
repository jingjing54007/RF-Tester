using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Collections.Generic;
using System.Xml;
using System.Xml.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using TruePosition.Test.DataLayer;
using TruePosition.Test.IO;
using TruePosition.Test.XML;
using TruePosition.Test.QF;

namespace UnitTests
{
    /// <summary>
    /// Summary description for UnitTest1
    /// </summary>
    [TestClass]
    public class UnitTests
    {
        string originalXMLFilePath = @"..\..\..\Documents\\lmu_dvt_testcases_all.xml";
        string convertedXMLFilePath = @"..\..\..\Documents\\Bootup Tests.xml";
        string COCommandXML = @"..\..\..\Documents\\COCommandTest.xml";
        string TestBootup1EXML = @"..\..\..\Documents\Test Case Bootup-1E.xml";

        public UnitTests()
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
            XElement actual = Transformer.Transform(originalXMLFilePath);
            Assert.IsNotNull(actual);
        }

        /// <summary>
        ///A test for Save
        ///</summary>
        [TestMethod()]
        public void SaveTest()
        {
            Transformer.Save(convertedXMLFilePath, Transformer.Transform(originalXMLFilePath));
        }

        /// <summary>
        ///A test for Hydrate
        ///</summary>
        [TestMethod()]
        public void HydrateTest()
        {
            string name = "Bootup";
            TestSets actual = Hydrator.Hydrate(name, convertedXMLFilePath);
            Assert.IsNotNull(actual);
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
            XElement exerpt = XElement.Parse("<TestSets>" +
                                                "<TestSet Name=\"Bootup\">" +
                                                    "<Test Type=\"Bootup\" Name=\"1A\">" +
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
                                                    "</Test>" +
                                                  "</TestSet>" +
                                                "</TestSets>");

            TestSets collections = Hydrator.Hydrate("Test", exerpt);
            ResponseProcessor.Process("First Test", collections["Bootup"]["1A"].Steps[0], "TEMPERATURES: recvr 31 bdc 28 power supply 35");
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

            TestSets collections = Hydrator.Hydrate("Test", exerpt);
            ResponseProcessor.Process("First Test", collections["Bootup"]["1A"].Steps[0], "ESN: " + "\n" +
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
            TestSets testCollections = Hydrator.Hydrate("CO Command Test", COCommandXML);

            ResponseProcessor.BindEvents(testCollections["Bootup"], Assembly.GetAssembly(typeof(TruePosition.Test.Custom.CSharp.CustomCommandEvents))); //Assembly.GetAssembly(typeof(TruePosition.Test.Custom.CSharp.CustomCommandEvents)));

            ResponseProcessor.Process("1A", testCollections["Bootup"]["1A"].Steps[0], "ESN: " + "\n" +
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
        private void port_DataTransmitted(object sender, MessageEventArgs args)
        {
        }
        private void port_DataReceived(object sender, MessageEventArgs args)
        {
            string msg = args.Message;
        }
        private void port_Error(object sender, CommErrorEventArgs args)
        {
        }
        private void port_ReceiveTimeoutExpired(object sender, TimeoutExpiredEventArgs args)
        {
            //sendwait = false;
        }
        [TestMethod]
        public void SerialTest()
        {
            //SerialPort port = new SerialPort("COM1", 19200);
            //port.DataTransmitted += new EventHandler<MessageEventArgs>(port_DataTransmitted);
            //port.DataReceived += new EventHandler<MessageEventArgs>(port_DataReceived);
            //port.Error +=new EventHandler<CommErrorEventArgs>(port_Error);
            //port.ReceiveTimeoutExpired += new EventHandler<TimeoutExpiredEventArgs>(port_ReceiveTimeoutExpired);
            //port.Open();
            //Assert.AreEqual(PortState.Opened, port.State);
            //bool xmit = port.Transmit("Chicken!");
            //while (sendwait) { System.Threading.Thread.Sleep(1); }

            //sendwait = true;
            //xmit = port.Transmit("Is");
            //while (sendwait) { System.Threading.Thread.Sleep(1); }

            //sendwait = true;
            //xmit = port.Transmit("good");
            //while (sendwait) { System.Threading.Thread.Sleep(1); }

            //sendwait = true;
            //xmit = port.Transmit("for");
            //while (sendwait) { System.Threading.Thread.Sleep(1); }

            //sendwait = true;
            //xmit = port.Transmit("lunch");
            //while (sendwait) { System.Threading.Thread.Sleep(1); }
            //System.Threading.Thread.Sleep(5000);
            ////Assert.AreEqual(true, xmit);
            //port.Dispose();
        }

        [TestMethod]
        public void ActivePortTest()
        {
            QFManager.Port.Create(PortType.Serial, "COM1");
            QFManager.Port.Receive("COM1", port_DataReceived);
            QFManager.Port.Error("COM1", port_Error);
            QFManager.Port.ReceiveTimeout("COM1", port_ReceiveTimeoutExpired);
            QFManager.Port.Open("COM1");
            QFManager.Port.Transmit("COM1", "The Chicken has landed!");
            System.Threading.Thread.Sleep(5000);
            QFManager.Port.Close("COM1");
            System.Threading.Thread.Sleep(System.Threading.Timeout.Infinite);
        }

        private string test_Stepping(string testName, Step step)
        {
            Debug.Print("<Event=test_Stepping,\tTest=" + testName + ",\tCommand='" + step.Actor.Action + "',\tNumber=" + step.Number + ">");

            if (step.Actor.Action == "?CO")
                return step.Actor.Action + " XXX";
            if (step.Actor.Action == "G")
                return step.Actor.Action + " 40.064618 -75.460373 0 53";

            return step.Actor.Action;
        }
        private void test_Stepped(object sender, TestSteppedEventArgs args)
        {
            Debug.Print("<Event=test_Stepped,\tTest=" + args.Name + ",\tCommand='" + args.Step.Actor.Action + "',\tNumber=" + args.Step.Number + ">");
        }
        private void test_Passed(object sender, TestPassedEventArgs args)
        {
            Debug.Print("<Event=test_Passed,\tTest=" + args.Name + ">");
        }
        private void test_Failed(object sender, TestFailedEventArgs args)
        {
            Debug.Print("<Event=test_Failed,\tTest=" + args.Name + ",\tCommand='" + args.Step.Actor.Action + "',\tNumber=" + args.Step.Number + ",\tReason=" + args.Reason + ">");
        }
        private void test_Aborted(object sender, TestAbortedEventArgs args)
        {
            Debug.Print("<Event=test_Aborted,\tTest=" + args.Name + ",\tReason=" + args.Reason + ">");
        }

        [TestMethod]
        public void ActiveTestTest()
        {
            TestSets testCollections = Hydrator.Hydrate("CO Command Test", COCommandXML);
            ResponseProcessor.BindEvents(testCollections["Bootup"], Assembly.GetAssembly(typeof(TruePosition.Test.Custom.CSharp.CustomCommandEvents)));

            QFManager.Port.Create(PortType.Serial, "COM1");
            QFManager.Port.Receive("COM1", port_DataReceived);
            QFManager.Port.Error("COM1", port_Error);
            QFManager.Port.ReceiveTimeout("COM1", port_ReceiveTimeoutExpired);
            QFManager.Port.Open("COM1");

            Test test = testCollections["Bootup"]["1A"];
            QFManager.Test.Load(test);
            QFManager.Test.Failed(test.Name, test_Failed);
            QFManager.Test.Passed(test.Name, test_Passed);
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
            TestSets testCollections = Hydrator.Hydrate("CO Command Test", COCommandXML);
            ResponseProcessor.BindEvents(testCollections["Bootup"], Assembly.GetAssembly(typeof(TruePosition.Test.Custom.CSharp.CustomCommandEvents)));
            //ResponseProcessor.BindEvents(testCollections["Bootup"], Assembly.LoadFile(@"C:\Development\Kapsoft\True Position\Test Components\Output\Out.dll"));

            QFManager.Port.Create(PortType.Serial, "COM1");
            QFManager.Port.Receive("COM1", port_DataReceived);
            QFManager.Port.Error("COM1", port_Error);
            QFManager.Port.ReceiveTimeout("COM1", port_ReceiveTimeoutExpired);
            QFManager.Port.Open("COM1");

            Test test = testCollections["Bootup"]["1A"];
            QFManager.Test.Load(test, UUTTestMode.Direct);
            QFManager.Test.Stepping(test.Name, test_Stepping);
            QFManager.Test.TestStepped(test.Name, test_Stepped);
            QFManager.Test.Passed(test.Name, test_Passed);
            QFManager.Test.Failed(test.Name, test_Failed);

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
            TestSets testCollections = Hydrator.Hydrate("Bootup Tests", TestBootup1EXML);

            QFManager.Port.Create(PortType.Serial, "COM1");
            QFManager.Port.Receive("COM1", port_DataReceived);
            QFManager.Port.Error("COM1", port_Error);
            QFManager.Port.ReceiveTimeout("COM1", port_ReceiveTimeoutExpired);
            QFManager.Port.Open("COM1");

            Test test = testCollections["Bootup"]["1E"];
            QFManager.Test.Load(test, UUTTestMode.Direct);
            QFManager.Test.Stepping(test.Name, test_Stepping);
            QFManager.Test.TestStepped(test.Name, test_Stepped);
            QFManager.Test.Passed(test.Name, test_Passed);
            QFManager.Test.Failed(test.Name, test_Failed);
            QFManager.Test.Aborted(test.Name, test_Aborted);

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
#endif
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
