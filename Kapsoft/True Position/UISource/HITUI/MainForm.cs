using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Reflection;
using System.Windows.Forms;
using System.IO;
using WeifenLuo.WinFormsUI.Docking;
using HITUI.Customization;

namespace HITUI
{
    public partial class MainForm : Form
    {
        // Step Event declares
        public event EventHandler<StepAddArgs> StepAdd = null;

        // Set Events declares
        public event EventHandler RunTestSet = null;
        public event EventHandler LoadTestSet = null;
        public event EventHandler SaveTestSet = null;
        public event EventHandler SaveAsSet = null;
        public event EventHandler SaveAsTest = null;
        public event EventHandler OpenTestSet = null;
        public event EventHandler NewTestSet = null;

        // Test Events declares
        public event EventHandler LoadTest = null;
        public event EventHandler SaveTest = null;
        public event EventHandler OpenTest = null;
        public event EventHandler NewTest = null;
        public event EventHandler RunTest = null;

        private const String strLMU1 = "LMU1 Console";
        private const String strLMU2 = "LMU2 Console";
        private const String strLMU3 = "LMU3 Console";
        private const String strLMU4 = "LMU4 Console";
        private const String strAll = "All LMU Log";

        private bool m_bSaveLayout = true;
		private DeserializeDockContent m_deserializeDockContent;
        private DummySolutionExplorer m_solutionExplorer = new DummySolutionExplorer();
        private DummyPropertyWindow m_propertyWindow = new DummyPropertyWindow();
        private TestSet m_testSetWindow = new TestSet();
		private DummyToolbox m_toolbox = new DummyToolbox();
		private DummyOutputWindow m_outputWindow1 = new DummyOutputWindow();
        private DummyOutputWindow m_outputWindow2 = new DummyOutputWindow(); 
        private DummyOutputWindow m_outputWindow3 = new DummyOutputWindow();
        private DummyOutputWindow m_outputWindow4 = new DummyOutputWindow();
        private DummyLog m_outputWindowAll= new DummyLog();
        private DummyTaskList m_taskList = new DummyTaskList();
        private DashBoard m_DashBoardWindow = new DashBoard();

        public MainForm()
        { 
            InitializeComponent();
            InitWindowTitles();
            RightToLeftLayout = false;
            //m_solutionExplorer.RightToLeftLayout = RightToLeftLayout;
			m_deserializeDockContent = new DeserializeDockContent(GetContentFromPersistString);
            WireEvents();
        }

        private void WireEvents()
        {
            
            #region SetEvents

            m_toolbox.RunTestSet += m_propertyWindow.OnSave;
            m_toolbox.RunTestSet += m_testSetWindow.OnRunTestSet;

            m_toolbox.RunTest += m_propertyWindow.OnSave;
            m_toolbox.RunTest += m_testSetWindow.OnRunTest;


            m_toolbox.SaveTestSet += m_testSetWindow.OnSaveTestSet;
            this.SaveTestSet += m_testSetWindow.OnSaveTestSet;

            m_toolbox.OpenTestSet += m_testSetWindow.OnOpenTestSet;
            this.OpenTestSet += m_testSetWindow.OnOpenTestSet;

            m_toolbox.NewTestSet += m_testSetWindow.OnNewTestSet;
            this.NewTestSet += m_testSetWindow.OnNewTestSet;

            m_DashBoardWindow.RunTestSet += m_testSetWindow.OnRunTestSet;
            this.RunTestSet += m_testSetWindow.OnRunTestSet;

            this.SaveAsSet += m_testSetWindow.OnSaveAsSet;
            this.SaveAsTest += m_testSetWindow.OnSaveAsTest;

            m_testSetWindow.UpdateDashboard += m_DashBoardWindow.OnUpdateDashboard;
            m_testSetWindow.UpdateSolutionExplorer += m_solutionExplorer.OnUpdateSet;
            m_solutionExplorer.UpdateTestSet += m_testSetWindow.OnUpdateTestSet;

            m_testSetWindow.UpdateDashboard += m_outputWindow1.OnUpdateOutputWindowState;
            m_testSetWindow.UpdateDashboard += m_outputWindow2.OnUpdateOutputWindowState;
            m_testSetWindow.UpdateDashboard += m_outputWindow3.OnUpdateOutputWindowState;
            m_testSetWindow.UpdateDashboard += m_outputWindow4.OnUpdateOutputWindowState;

            m_outputWindow1.UpdateLog += m_outputWindowAll.OnUpdateLog;
            m_outputWindow2.UpdateLog += m_outputWindowAll.OnUpdateLog;
            m_outputWindow3.UpdateLog += m_outputWindowAll.OnUpdateLog;
            m_outputWindow4.UpdateLog += m_outputWindowAll.OnUpdateLog;
            
            this.KeyPreview = true;
            this.KeyDown += new KeyEventHandler(m_testSetWindow.TestSet_KeyDown);
            #endregion

            #region TestEvents
            m_toolbox.SaveTest += m_testSetWindow.OnSaveTest;
            m_toolbox.OpenTest += m_testSetWindow.OnOpenTest;
            m_toolbox.NewTest += m_testSetWindow.OnNewTest;
            m_toolbox.RunTest += m_testSetWindow.OnRunTest;
            m_toolbox.StepAdd += m_testSetWindow.OnStepAdd;
            this.StepAdd += m_testSetWindow.OnStepAdd;
            #endregion           

        }
		private void menuItemExit_Click(object sender, System.EventArgs e)
		{
			Close();
		}

		private void menuItemSolutionExplorer_Click(object sender, System.EventArgs e)
		{
			m_solutionExplorer.Show(dockPanel);
		}

		private void menuItemPropertyWindow_Click(object sender, System.EventArgs e)
		{
			m_propertyWindow.Show(dockPanel);
		}

		private void menuItemToolbox_Click(object sender, System.EventArgs e)
		{
			m_toolbox.Show(dockPanel);
		}		

		private void menuItemTaskList_Click(object sender, System.EventArgs e)
		{
			m_taskList.Show(dockPanel);
		}

		private void menuItemAbout_Click(object sender, System.EventArgs e)
		{
			AboutDialog aboutDialog = new AboutDialog();
			aboutDialog.ShowDialog(this);
		}

		private IDockContent FindDocument(string text)
		{
			if (dockPanel.DocumentStyle == DocumentStyle.SystemMdi)
			{
				foreach (Form form in MdiChildren)
					if (form.Text == text)
						return form as IDockContent;
				
				return null;
			}
			else
			{
				foreach (IDockContent content in dockPanel.Documents)
					if (content.DockHandler.TabText == text)
						return content;

				return null;
			}
		}

		private void menuItemNew_Click(object sender, System.EventArgs e)
		{
			DummyDoc dummyDoc = CreateNewDocument();
			if (dockPanel.DocumentStyle == DocumentStyle.SystemMdi)
			{
				dummyDoc.MdiParent = this;
				dummyDoc.Show();
			}
			else
				dummyDoc.Show(dockPanel);
		}

		private DummyDoc CreateNewDocument()
		{
			DummyDoc dummyDoc = new DummyDoc();

			int count = 1;
			//string text = "C:\\MADFDKAJ\\ADAKFJASD\\ADFKDSAKFJASD\\ASDFKASDFJASDF\\ASDFIJADSFJ\\ASDFKDFDA" + count.ToString();
            string text = "Document" + count.ToString();
            while (FindDocument(text) != null)
			{
				count ++;
                //text = "C:\\MADFDKAJ\\ADAKFJASD\\ADFKDSAKFJASD\\ASDFKASDFJASDF\\ASDFIJADSFJ\\ASDFKDFDA" + count.ToString();
                text = "Document" + count.ToString();
            }
			dummyDoc.Text = text;
			return dummyDoc;
		}

		private DummyDoc CreateNewDocument(string text)
		{
			DummyDoc dummyDoc = new DummyDoc();
			dummyDoc.Text = text;
			return dummyDoc;
		}

		private void menuItemFile_Popup(object sender, System.EventArgs e)
		{
			if (dockPanel.DocumentStyle == DocumentStyle.SystemMdi)
			{
				//menuItemClose.Enabled = menuItemCloseAll.Enabled = (ActiveMdiChild != null);
			}
			else
			{
				//menuItemClose.Enabled = (dockPanel.ActiveDocument != null);
				//menuItemCloseAll.Enabled = (dockPanel.DocumentsCount > 0);
			}
		}

		private void menuItemClose_Click(object sender, System.EventArgs e)
		{
			if (dockPanel.DocumentStyle == DocumentStyle.SystemMdi)
				ActiveMdiChild.Close();
			else if (dockPanel.ActiveDocument != null)
				dockPanel.ActiveDocument.DockHandler.Close();
		}

		private void menuItemCloseAll_Click(object sender, System.EventArgs e)
		{
			CloseAllDocuments();
		}

		private void CloseAllDocuments()
		{
			if (dockPanel.DocumentStyle == DocumentStyle.SystemMdi)
			{
				foreach (Form form in MdiChildren)
					form.Close();
			}
			else
			{
                for (int index = dockPanel.Contents.Count - 1; index >= 0; index--)
                {
                    if (dockPanel.Contents[index] is IDockContent)
                    {
                        IDockContent content = (IDockContent)dockPanel.Contents[index];
                        content.DockHandler.Close();
                    }
                }
			}
		}

		private IDockContent GetContentFromPersistString(string persistString)
		{
            if (persistString == typeof(DummySolutionExplorer).ToString())
                return m_solutionExplorer;
            else if (persistString == typeof(DummyPropertyWindow).ToString())
                return m_propertyWindow;
            else if (persistString == typeof(DummyToolbox).ToString())
                return m_toolbox;
            else if (persistString.IndexOf(typeof(DummyOutputWindow).ToString()) != -1)
                return FindOutputWindow(persistString);
            else if (persistString == typeof(DummyTaskList).ToString())
                return m_taskList;
            else if (persistString == typeof(TestSet).ToString())
                return m_testSetWindow;
            else if (persistString == typeof(DashBoard).ToString())
                return m_DashBoardWindow;
            else if (persistString == typeof(DummyLog).ToString())
                return m_outputWindowAll;
            else
            {
                // DummyDoc overrides GetPersistString to add extra information into persistString.
                // Any DockContent may override this value to add any needed information for deserialization.

                string[] parsedStrings = persistString.Split(new char[] { ',' });
                if (parsedStrings.Length != 3)
                    return null;

                if (parsedStrings[0] != typeof(DummyDoc).ToString())
                    return null;

                DummyDoc dummyDoc = new DummyDoc();
                if (parsedStrings[1] != string.Empty)
                    dummyDoc.FileName = parsedStrings[1];
                if (parsedStrings[2] != string.Empty)
                    dummyDoc.Text = parsedStrings[2];

                return dummyDoc;
            }
		}

        private DummyOutputWindow FindOutputWindow(string persistString)
        {
            DummyOutputWindow dockContentTmp = null;

            string[] parsedStrings = persistString.Split(new char[] { ',' });
            if (parsedStrings.Length  != 2)
                return null;
            
            if (persistString == typeof(DummyOutputWindow).ToString() +  "," + strLMU1)
                dockContentTmp=  m_outputWindow1;
            else if (persistString == typeof(DummyOutputWindow).ToString() + "," + strLMU2)
                dockContentTmp=  m_outputWindow2;
            else if (persistString == typeof(DummyOutputWindow).ToString() + "," + strLMU3)
                dockContentTmp = m_outputWindow3;
            else if (persistString == typeof(DummyOutputWindow).ToString() + "," + strLMU4)
                dockContentTmp = m_outputWindow4;
            
            // restore Titles to windows from elements of perists string if we found the window
            if (dockContentTmp != null)
            {
                dockContentTmp.Text = parsedStrings[1];
                dockContentTmp.TabText = parsedStrings[1];
            }
            return dockContentTmp;
        }

		private void MainForm_Load(object sender, System.EventArgs e)
		{
			string configFile = Path.Combine(Path.GetDirectoryName(Application.ExecutablePath), "DockPanel.config");

			if (File.Exists(configFile))
				dockPanel.LoadFromXml(configFile, m_deserializeDockContent);
		}

		private void MainForm_Closing(object sender, System.ComponentModel.CancelEventArgs e)
		{
			string configFile = Path.Combine(Path.GetDirectoryName(Application.ExecutablePath), "DockPanel.config");
            if (m_bSaveLayout)
                dockPanel.SaveAsXml(configFile);
            else if (File.Exists(configFile))
                File.Delete(configFile);
		}

		private void menuItemNewWindow_Click(object sender, System.EventArgs e)
		{
			MainForm newWindow = new MainForm();
			newWindow.Text = newWindow.Text + " - New";
			newWindow.Show();
		}

		private void menuItemTools_Popup(object sender, System.EventArgs e)
		{
            //menuItemLockLayout.Checked = true; // !this.dockPanel.AllowEndUserDocking;
		}

		private void menuItemLockLayout_Click(object sender, System.EventArgs e)
		{
			dockPanel.AllowEndUserDocking = !dockPanel.AllowEndUserDocking;
		}
        private void menuItemLayoutByXml_Click(object sender, System.EventArgs e)
		{
			dockPanel.SuspendLayout(true);

			// In order to load layout from XML, we need to close all the DockContents
			CloseAllContents();

			Assembly assembly = Assembly.GetAssembly(typeof(MainForm));
			Stream xmlStream = assembly.GetManifestResourceStream("DockSample.Resources.DockPanel.xml");
			dockPanel.LoadFromXml(xmlStream, m_deserializeDockContent);
			xmlStream.Close();

			dockPanel.ResumeLayout(true, true);
		}

		private void CloseAllContents()
		{
			// we don't want to create another instance of tool window, set DockPanel to null
			m_solutionExplorer.DockPanel = null;

            
			m_propertyWindow.DockPanel = null;

			m_toolbox.DockPanel = null;
			m_outputWindow1.DockPanel = null;
			m_taskList.DockPanel = null;

			// Close all other document windows
			CloseAllDocuments();
		}

		private void SetDocumentStyle(object sender, System.EventArgs e)
		{
			DocumentStyle oldStyle = dockPanel.DocumentStyle;
			DocumentStyle newStyle;
            newStyle = DocumentStyle.DockingMdi;
            if (oldStyle == DocumentStyle.SystemMdi || newStyle == DocumentStyle.SystemMdi)
				CloseAllDocuments();

			dockPanel.DocumentStyle = newStyle;
		}

		private void menuItemCloseAllButThisOne_Click(object sender, System.EventArgs e)
		{
			if (dockPanel.DocumentStyle == DocumentStyle.SystemMdi)
			{
				Form activeMdi = ActiveMdiChild;
				foreach (Form form in MdiChildren)
				{
					if (form != activeMdi)
						form.Close();
				}
			}
			else
			{
				foreach (IDockContent document in dockPanel.DocumentsToArray())
				{
					if (!document.DockHandler.IsActivated)
						document.DockHandler.Close();
				}
			}
		}

		private void showRightToLeft_Click(object sender, EventArgs e)
        {
            CloseAllContents();
        }

        private void exitWithoutSavingLayout_Click(object sender, EventArgs e)
        {
            m_bSaveLayout = false;
            Close();
            m_bSaveLayout = true;
        }

        private void mainMenu_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {
            if ((string)e.ClickedItem.Tag == "Set") //!= null)
            {
                int i = 0;

            }
        }

        private void lMU1ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            m_outputWindow1.Show(dockPanel);
 
        }

        private void lMU2ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            m_outputWindow2.Show(dockPanel);
        }

        private void lMU3ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            m_outputWindow3.Show(dockPanel);
        }

        private void lMU4ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            m_outputWindow4.Show(dockPanel);
        }

        private void allToolStripMenuItem_Click(object sender, EventArgs e)
        {
            m_outputWindowAll.Show(dockPanel);
        }

        //private void menuItemTestLog_Click(object sender, EventArgs e)
        //{
            //m_outputWindowTestLog.Show(dockPanel);
        //}

        private void menuItemTestMatrix_Click(object sender, EventArgs e)
        {
            m_testSetWindow.Show(dockPanel);
        }

        private void InitWindowTitles()
        {
            m_outputWindow1.Text = strLMU1;
            m_outputWindow1.TabText = strLMU1;
            m_outputWindow2.Text = strLMU2;
            m_outputWindow2.TabText = strLMU2;
            m_outputWindow3.Text = strLMU3;
            m_outputWindow3.TabText = strLMU3;
            m_outputWindow4.Text = strLMU4;
            m_outputWindow4.TabText = strLMU4;
            m_outputWindowAll.Text = strAll;
            m_outputWindowAll.TabText = strAll;
            //m_outputWindowTestLog.Text = "Test log";
            //m_outputWindowTestLog.TabText = "Test log";
            m_testSetWindow.Text = "Test Set";
            m_testSetWindow.TabText = "Test Set";
            m_testSetWindow.HideOnClose = true;
            m_DashBoardWindow.Text = "Dash Board";
            m_DashBoardWindow.TabText = "Dash Board";

        }
        #region Step Adds
        private void preToolStripMenuItem_Click(object sender, EventArgs e)
        {
            m_propertyWindow.Show(dockPanel);
        }

        private void MenuItemDashBoard_Click(object sender, EventArgs e)
        {
            m_DashBoardWindow.Show(dockPanel);
        }

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            m_propertyWindow.Save();
        }

        public void recorderMenuItem_Click(object sender, EventArgs e)
        {
            StepAdd(this, new StepAddArgs(StepIdentity.Recorder));
        }

        private void processStepMenuItem_Click(object sender, EventArgs e)
        {
            StepAdd(this, new StepAddArgs(StepIdentity.Process));
        }

        private void SMTPGetMenuItem_Click(object sender, EventArgs e)
        {
            StepAdd(this, new StepAddArgs(StepIdentity.SNMPGET));
        }

        private void toolStripMenuItem_Click(object sender, EventArgs e)
        {
            StepAdd(this, new StepAddArgs(StepIdentity.SNMPSET));
        }

        private void sigGenFreqMenuItem_Click(object sender, EventArgs e)
        {
            StepAdd(this, new StepAddArgs(StepIdentity.SigFreq));
        }

        private void sigGenPowerMenuItem_Click(object sender, EventArgs e)
        {
            StepAdd(this, new StepAddArgs(StepIdentity.SigPower));
        }

        private void sigGenOnMenuItem_Click(object sender, EventArgs e)
        {
            StepAdd(this, new StepAddArgs(StepIdentity.SigOn));
        }

        private void sigGenOffMenuItem_Click(object sender, EventArgs e)
        {
            StepAdd(this, new StepAddArgs(StepIdentity.SigOff));
        }

        private void sigGenNoiseToolStripMenuItem_Click(object sender, EventArgs e)
        {
            StepAdd(this, new StepAddArgs(StepIdentity.SigNoise));
        }

        private void LMUConsolepMenuItem_Click(object sender, EventArgs e)
        {
            StepAdd(this, new StepAddArgs(StepIdentity.Console));
        }

        private void TDOAMenuItem_Click(object sender, EventArgs e)
        {
            StepAdd(this, new StepAddArgs(StepIdentity.TDOA));
        }

        private void userInterfaceDisplayMenuItem5_Click(object sender, EventArgs e)
        {
            StepAdd(this, new StepAddArgs(StepIdentity.UIWaitDialog));
        }

        private void UITimerWithDialogtMenuItem_Click(object sender, EventArgs e)
        {
            StepAdd(this, new StepAddArgs(StepIdentity.UIWaitWithTimeout));
        }

        private void delayMenuItem_Click(object sender, EventArgs e)
        {
            StepAdd(this, new StepAddArgs(StepIdentity.Delay));
        }

        private void SMLCTelentMenuItem_Click(object sender, EventArgs e)
        {
            StepAdd(this, new StepAddArgs(StepIdentity.Telnet));
        }

        private void powerSupplyMenuItem_Click(object sender, EventArgs e)
        {
            StepAdd(this, new StepAddArgs(StepIdentity.PowerSupplyOn));
        }

        private void powerSupplyOffMenuItem_Click(object sender, EventArgs e)
        {
            StepAdd(this, new StepAddArgs(StepIdentity.PowerSupplyOff));
        }

        private void temperatureSetMenuItem_Click(object sender, EventArgs e)
        {
            // TDB DJK
            //StepAdd(this, new StepAddArgs(StepIdentity.));
        }
        #endregion

        
        private void cutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SendKeys.Send("^(X)");
        }

        private void copyToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SendKeys.Send("^(C)");
        }

        private void pasteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SendKeys.Send("^(V)");
        }

        private void deleteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SendKeys.Send("^(D)");
        }

        private void moveStepUptoolStripMenuItem_Click(object sender, EventArgs e)
        {
            SendKeys.Send("^({UP})");
        }

        private void moveStepDowntoolStripMenuItem_Click(object sender, EventArgs e)
        {
            SendKeys.Send("^({DOWN})");
        }

        private void menuItemSave_Click(object sender, EventArgs e)
        {
            SendKeys.Send("^(S)");
        }

        private void setSaveAsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SaveAsSet(this, EventArgs.Empty);
        }

        private void testSaveAsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SaveAsTest(this, EventArgs.Empty);
        }

        
    }
}