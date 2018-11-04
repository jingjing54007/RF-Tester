using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using TruePosition.Test.IO;
using TruePosition.Test.UI;
using System.Diagnostics;
using System.ComponentModel;
using TruePosition.Test.Process;

namespace UnitTests
{
    /// <summary>
    /// Summary description for Test_UI
    /// </summary>
    [TestClass]
    public class Test_UI
    {
        const string testXMLPath = @"..\..\..\Documents";
        const string ConfigPath = testXMLPath + @"\Config";

        public Test_UI()
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
        [ClassInitialize()]
        public static void MyClassInitialize(TestContext testContext) 
        {
            ConfigManager.SetLocation(ConfigPath);
        }
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

        [TestMethod]
        public void CreateSerialTest()
        {
            ISerialConfig config = null;
            ConfigManager.Create("LMU1", out config);
        }
        [TestMethod]
        public void SaveSerialTest()
        {
            ISerialConfig config = null;
            ConfigManager.Create("LMU1", out config);
            ConfigManager.Save("LMU1", config);
        }
        [TestMethod]
        public void LoadSerialTest()
        {
            ISerialConfig config = null;
            ConfigManager.Load("LMU1", out config);
        }
        [TestMethod]
        public void CreateProcessTest()
        {
            IProcessConfig config = null;
            ConfigManager.Create("IE", out config);
        }
        [TestMethod]
        public void SaveProcessTest()
        {
            IProcessConfig config = null;
            ConfigManager.Create("IE", out config);
            ConfigManager.Save("IE", config);
        }
        [TestMethod]
        public void LoadProcessTest()
        {
            IProcessConfig config = null;
            ConfigManager.Load("IE", out config);
        }
        [TestMethod]
        public void CreateGlobalTest()
        {
            IGlobalConfig config = null;
            ConfigManager.Create(out config);
        }
        [TestMethod]
        public void SaveGlobalTest()
        {
            IGlobalConfig config = null;
            ConfigManager.Create(out config);
            ConfigManager.Save(config);
        }
        [TestMethod]
        public void LoadGlobalTest()
        {
            IGlobalConfig config = null;
            ConfigManager.Load(out config);
        }
        [TestMethod]
        public void CreateTelnetTest()
        {
            ITelnetConfig config = null;
            ConfigManager.Create("SMLC1", out config);
        }
        [TestMethod]
        public void SaveTelnetTest()
        {
            ITelnetConfig config = null;
            ConfigManager.Create("SMLC1", out config);
            ConfigManager.Save("SMLC1", config);
        }
        [TestMethod]
        public void LoadTelnetTest()
        {
            ITelnetConfig config = null;
            ConfigManager.Load("SMLC1", out config);
        }
        [TestMethod]
        public void CreateGpibTest()
        {
            IGpibConfig config = null;
            ConfigManager.Create("SigGen", out config);
        }
        [TestMethod]
        public void SaveGpibTest()
        {
            IGpibConfig config = null;
            //ConfigManager.Create("SigGen", out config);
            ConfigManager.Load("SigGen", out config);
            ConfigManager.Save("SigGen", config);
        }
        [TestMethod]
        public void LoadGpibTest()
        {
            IGpibConfig config = null;
            ConfigManager.Load("SigGen", out config);
        }
    }
}
