using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Xml.Linq;
using TruePosition.Test.XML;
using TruePosition.Test.DataLayer;

namespace UnitTests
{
    /// <summary>
    /// Summary description for UnitTest1
    /// </summary>
    [TestClass]
    public class Test_XML
    {
        string originalXMLFilePath = @"..\..\..\Documents\lmu_dvt_testcases_all.xml";
        string convertedXMLPath = @"..\..\..\Documents\Bootup Tests";
        string xmlPath = @"..\..\..\Documents";

        const string testXMLPath = @"..\..\..\Documents";
        const string TestBootup1EXMLFile = "Test Case Bootup-1E.xml";

        public Test_XML()
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
            IEnumerable<XElement> actual = Transformer.Transform(originalXMLFilePath);
            Assert.IsNotNull(actual);
        }

        /// <summary>
        ///A test for Save
        ///</summary>
        [TestMethod()]
        public void SaveTransformTest()
        {
            Transformer.Save(convertedXMLPath, Transformer.Transform(originalXMLFilePath));
        }

        [TestMethod()]
        public void HydrateFileTest()
        {
            Test test = Hydrator.Hydrate(testXMLPath, TestBootup1EXMLFile);
            Assert.IsNotNull(test);
        }
        [TestMethod()]
        public void HydrateFolderTest()
        {
            IEnumerable<Test> actual = Hydrator.Hydrate(convertedXMLPath);
            Assert.IsNotNull(actual);
        }
        [TestMethod()]
        public void DehydrateTest()
        {
            IEnumerable<Test> tests = Hydrator.Hydrate(convertedXMLPath);
            XElement test = Dehydrator.Dehydrate(tests.First());
            Assert.IsNotNull(test);
        }
        [TestMethod()]
        public void DehydrateNoResponseTest()
        {
            Test test = Hydrator.Hydrate(xmlPath, "Test 1A.xml");
            XElement xTest = Dehydrator.Dehydrate(test);
            Assert.IsNotNull(xTest);
        }
        [TestMethod]
        public void DehydrateSimpleResponseTest()
        {
            Test test = Hydrator.Hydrate(testXMLPath, TestBootup1EXMLFile);
            XElement xTest = Dehydrator.Dehydrate(test);
            Assert.IsNotNull(xTest);
        }
        [TestMethod()]
        public void SaveTest()
        {
            IEnumerable<Test> tests = Hydrator.Hydrate(convertedXMLPath);
            tests.First().Save(convertedXMLPath);
        }
        [TestMethod()]
        public void SaveNoResponseTest()
        {
            Test test = Hydrator.Hydrate(xmlPath, "Test 1A.xml");
            test.Save(xmlPath, "Test 1A1.xml");
        }
        [TestMethod]
        public void SaveSimpleResponseTest()
        {
            Test test = Hydrator.Hydrate(testXMLPath, TestBootup1EXMLFile);
            test.Save(testXMLPath, TestBootup1EXMLFile + ".test");
        }
        [TestMethod()]
        public void SaveAllTest()
        {
            IEnumerable<Test> tests = Hydrator.Hydrate(convertedXMLPath);
            tests.Save(convertedXMLPath);
        }
    }
}
