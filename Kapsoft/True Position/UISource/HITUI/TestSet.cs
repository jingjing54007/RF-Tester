using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.IO;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Xml.Linq;
using System.Media;
using DevExpress.XtraGrid;

using TruePosition.Test.DataLayer;
using TruePosition.Test.Core;
using TruePosition.Test.XML;
using TruePosition.Test.UI;
using TruePosition.Test.Process;
using TruePosition.Test.QF;
using TruePosition.Test.Prompt;

namespace HITUI
{
    public partial class TestSet : WeifenLuo.WinFormsUI.Docking.DockContent
    {
        public event EventHandler<UpdateTestArgs> UpdateDashboard = null;
        public event EventHandler<UpdateSolutionExplorerArgs> UpdateSolutionExplorer;

        private const string strOpenSetFilter = "(*.Set)|*.Set;|All files (*.*)|*.*";
        private const string strOpenTestFilter = "(*.Xml)|*.Xml;|All files (*.*)|*.*";
        private const string errorMsg = "Test does not exist at specified location...";
        private List<StepWrapper> m_listWrapper = new List<StepWrapper>();
        private List<Test> m_listTests = new List<Test>();
        private XElement m_setElements = null;
        private string m_SetFilename = "";
        private int m_stepNumber;
        private int m_selectedTest;
        private bool m_RunFlag;
        private bool m_RunSingle;
        private int m_NewTestNum;
        private Step m_CopyPasteStep = null;

        private const int LISTVIEW_STATUS_CO = 1;
        private const int LISTVIEW_STARTTIME_COL = 2;
        private const int LISTVIEW_ENDTIME_COL = 3;

        Dictionary<int, string> status = new Dictionary<int, string>();

        #region Constructors
        public TestSet()
        {
            InitializeComponent();
            InitGridOptions();
            WireTestEvents();
            m_RunFlag = false;
            m_RunSingle = false;
            m_NewTestNum = 0;
        }
        #endregion

        public void TestSet_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Control == true)
            {                
                switch (e.KeyCode)
                {                    
                    case Keys.D:
                        DeleteSelectedGridRow();
                        e.Handled = true;
                        e.SuppressKeyPress = true;
                        break;
                    case Keys.C:
                        if (CopyRow() == true)
                        {
                            e.Handled = true;
                            e.SuppressKeyPress = true;
                        }
                        break;
                    case Keys.X:
                        if (CutRow() == true)
                        {
                            e.Handled = true;
                            e.SuppressKeyPress = true;
                        }
                        break;
                    case Keys.V:
                        if (PasteRow() == true)
                        {
                            e.Handled = true;
                            e.SuppressKeyPress = true;
                        }
                        break;
                    case Keys.Up:
                        MoveUpSelectedGridRow();
                        e.Handled = true;
                        e.SuppressKeyPress = true;
                        break;
                    case Keys.Down:
                        MoveDownSelectedGridRow();
                        e.Handled = true;
                        e.SuppressKeyPress = true;
                        break;
                    default:
                        break;
                }
            }
        }

        private bool PasteRow()
        {
            try
            {
                if (HasValidRows() && m_CopyPasteStep != null)
                {
                    InsertStepRow(m_CopyPasteStep);
                    // Note: copy row again in case the user hits paste again before another copy
                    CopyRow();
                    return true;
                }
                else
                {
                    SystemSounds.Beep.Play();
                    return false;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Paste failed. Error detail:" +
                    ex.Message);
                return false;
            }
        }

        private bool CopyRow()
        {
            try
            {
                if (HasValidRows())
                {
                    CopyStepRow(gridView1.FocusedRowHandle);
                    return true;
                }
                else
                {
                    SystemSounds.Beep.Play();
                    return false;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Copy failed. Error detail:" +
                    ex.Message);
                return false;
            }
        }

        private bool CutRow()
        {
            try
            {
                if (HasValidRows())
                {
                    CopyStepRow(gridView1.FocusedRowHandle);
                    DeleteSelectedGridRow();
                    return true;
                }
                else
                {
                    SystemSounds.Beep.Play();
                    return false;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Cut failed. Error detail:" +
                    ex.Message);
                return false;
            }
        }

        private bool HasValidRows()
        {
            return gridView1.FocusedRowHandle >= 0 && m_listTests[m_selectedTest].Steps.Count > 0
                && gridView1.IsEditing == false;
        }

        private void MoveUpSelectedGridRow()
        {
            try
            {
                // Note: if grid is in edit mode just beep user and do nothing
                if (gridView1.IsEditing == true)
                {
                    SystemSounds.Beep.Play();
                    return;
                }
                // Can't be at top of grid or empty
                else if (gridView1.FocusedRowHandle != 0 && m_listTests[m_selectedTest].Steps.Count > 0)
                {
                    int rowFocus = gridView1.FocusedRowHandle;                    
                    // get a handy reference to step you want to move
                    Step stepTmp = m_listTests[m_selectedTest].Steps[gridView1.FocusedRowHandle];
                    m_listTests[m_selectedTest].Steps.RemoveAt(gridView1.FocusedRowHandle);
                    InsertStepRow(stepTmp, -1);                    
                    // Note: adjust focus row only set after grid is repopulated
                    gridView1.FocusedRowHandle = rowFocus - 1;
                    gridView1.RefreshData();
                                     
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Your Move down failed. Error detail:" +
                    ex.Message);
            }
        }

        private void CopyStepRow(int rowNumber)
        {
            m_CopyPasteStep = m_listTests[m_selectedTest].Steps[rowNumber].Copy();
        }

        private void InsertStepRow(Step step)
        {
            InsertStepRow(step, 0);
        }

        private void InsertStepRow(Step step, int offset)
        {
            m_listTests[m_selectedTest].Steps.Insert((gridView1.FocusedRowHandle + offset), step);
            ReNumberSteps(m_listTests[m_selectedTest].Steps);
            gridControl1.RefreshDataSource();
            gridView1.RefreshData();
        }

        private void MoveDownSelectedGridRow()
        {
            try
            {   // Note: if grid is in edit mode just beep user and do nothing
                if (gridView1.IsEditing == true)
                {
                    SystemSounds.Beep.Play();
                    return;
                }
                // Can't be at bottom of grid or empty
                else if (gridView1.FocusedRowHandle != (m_listTests[m_selectedTest].Steps.Count - 1)
                    && m_listTests[m_selectedTest].Steps.Count > 0)
                {
                    int rowFocus = gridView1.FocusedRowHandle;
                    // get a handy reference to step you want to move down
                    Step stepTmp = m_listTests[m_selectedTest].Steps[gridView1.FocusedRowHandle];
                    m_listTests[m_selectedTest].Steps.RemoveAt(gridView1.FocusedRowHandle);
                    InsertStepRow(stepTmp, 1);

                    // Note: adjust focus row only set after grid is repopulated
                    gridView1.FocusedRowHandle = rowFocus + 1;
                    gridView1.RefreshData();                   
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Your move down failed. Error detail:" +
                    ex.Message);
            }
        }
        private void DeleteSelectedGridRow()
        {
            try
            {
                if (m_listTests[m_selectedTest].Steps.Count > 0)
                {
                    m_listTests[m_selectedTest].Steps.RemoveAt(gridView1.FocusedRowHandle);
                    ReNumberSteps(m_listTests[m_selectedTest].Steps);

                    // Note: if the focus is past last row move it up one row to the actual last
                    if (gridView1.FocusedRowHandle == m_listTests[m_selectedTest].Steps.Count)
                    {
                        --gridView1.FocusedRowHandle;
                    }
                    gridControl1.RefreshDataSource();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("You Delete action failed. Sucks to be you. Error detail:" +
                    ex.Message);
            }
        }

        private object GetStatus(Step step)
        {

            string msg = null;
            status.TryGetValue(step.Number, out msg);
            return msg;
        }

        private void SetStatus(Step step, string msg)
        {

            status[step.Number] = msg;
            gridControl1.MainView.RefreshData();
        }

        public void OnUpdateTestSet(object sender, UpdateTestArgs args)
        {
            switch (args.eAction)
            {
                case UpdateTestAction.TestUp:
                    MoveListViewItemUp(args.Index);
                    break;
                case UpdateTestAction.TestDown:
                    MoveListViewItemDown(args.Index);
                    break;
                case UpdateTestAction.TestAdd:
                    DoNewTest();
                    break;
                case UpdateTestAction.TestDelete:
                    DeleteTestInList(args.Index);
                    break;

            }
        }

        #region ListView Update Methods

        private void UpdateListviewStatus(string status, Color color)
        {
            listViewSet.Items[m_selectedTest].SubItems[LISTVIEW_STATUS_CO].Text = status;
            listViewSet.Items[m_selectedTest].SubItems[LISTVIEW_STATUS_CO].ForeColor = color;

        }

        private void MoveListViewItemDown(int index)
        {
            try
            {
                // Note: can't be empty list or at bottom for this operation
                if (listViewSet.Items.Count > 0 && (index + 1) != listViewSet.Items.Count)
                {
                    // Save this for later restore of selection
                    int selectedIndex = GetListViewIndex();

                    MoveTestInList(index, 1);

                    // Restore selection
                    listViewSet.Items[selectedIndex].Selected = true;

                }
            }
            catch { }
        }

        private void MoveListViewItemUp(int index)
        {
            try
            {
                // Note: can't be empty list or at bottom for this operation
                if (index != 0)
                {
                    // Save this for later restore of selection
                    int selectedIndex = GetListViewIndex();

                    MoveTestInList(index, -1);

                    // Restore selection
                    listViewSet.Items[selectedIndex].Selected = true;

                }
            }
            catch { }
        }
        private void MoveTestInList(int index, int offset)
        {
            try
            {
                // Save, remove, and insert after this position in listview
                ListViewItem item = listViewSet.Items[index];
                listViewSet.Items.RemoveAt(index);
                listViewSet.Items.Insert(index + offset, item);

                List<XElement> listTests = m_setElements.Elements().ToList();
                XElement el = listTests[index];
                listTests.RemoveAt(index);
                listTests.Insert(index + offset, el);
                m_setElements.Elements().Remove();
                m_setElements.Add(listTests);

                // Now fix up tests and wrappers
                Test test = m_listTests[index];
                StepWrapper wrapper = m_listWrapper[index];

                m_listTests.RemoveAt(index); ;
                m_listTests.Insert(index + offset, test);
                m_listWrapper.RemoveAt(index);
                m_listWrapper.Insert(index + offset, wrapper);
            }
            catch { }
        }

        private void DeleteTestInList(int index)
        {
            try
            {
                if (listViewSet.Items.Count > 0)
                {
                    // Save this for later restore of selection
                    int selectedIndex = GetListViewIndex();
                    listViewSet.Items.RemoveAt(index);
                    List<XElement> listTests = m_setElements.Elements().ToList();
                    listTests.RemoveAt(index);
                    m_setElements.Elements().Remove();
                    m_setElements.Add(listTests);

                    // Now fix up tests and wrappers
                    m_listTests.RemoveAt(index); ;
                    m_listWrapper.RemoveAt(index);

                    // special case of count going to zero, must refresh grid
                    if (listViewSet.Items.Count == 0)
                    {
                        gridControl1.DataSource = null;
                        gridControl1.Refresh();
                        gridView1.RefreshData();
                    }
                    else
                    {
                        // Restore selection
                        listViewSet.Items[selectedIndex].Selected = true;
                    }
                }

                
            }
            catch { }
        }



        private void UpdateListviewStatus(string status)
        {
            UpdateListviewStatus(status, Color.Black);
        }

        private void UpdateStartTime()
        {

            listViewSet.Items[m_selectedTest].SubItems[LISTVIEW_STARTTIME_COL].Text =
                DateTime.Now.ToString("HH:mm:ss");

        }

        private void UpdateEndTime()
        {
            listViewSet.Items[m_selectedTest].SubItems[LISTVIEW_ENDTIME_COL].Text =
                DateTime.Now.ToString("HH:mm:ss");
        }

        private void AddTestToListViews(string filename)
        {
            int index = GetListViewIndex();
            if (index == -1)
            {
                index = 0;                
            }
           
            ListViewItem listItem = new ListViewItem(Path.GetFileNameWithoutExtension(filename));
            listItem.SubItems.Add("");
            listItem.SubItems.Add("");
            listItem.SubItems.Add("");
            listViewSet.Items.Insert(index,listItem);
            listViewSet.Refresh();

        }
        private void ClearSetListSubItems()
        {
            foreach (ListViewItem lvitm in listViewSet.Items)
            {
                lvitm.SubItems[LISTVIEW_STATUS_CO].Text = "";
                lvitm.SubItems[LISTVIEW_STARTTIME_COL].Text = "";
                lvitm.SubItems[LISTVIEW_ENDTIME_COL].Text = "";
            }
        }

        private int GetListViewIndex()
        {
            ListView.SelectedIndexCollection indexes = listViewSet.SelectedIndices;
            // Note: must be single selection!
            return (indexes.Count == 1 ? indexes[0] : -1);
        }

        private void listViewSet_ItemSelectionChanged(object sender, ListViewItemSelectionChangedEventArgs e)
        {
            if (e.IsSelected == true)
            {
                // Note: make sure this is not an empty set (no tests in Set yet).
                // Which will happen after you create a new set
                if (m_listWrapper.Count > 0)
                {
                    gridControl1.DataSource = m_listWrapper[e.ItemIndex];
                    m_selectedTest = e.ItemIndex;
                    gridControl1.MainView.PopulateColumns();
                    ShowMasterDetailRows();
                    gridView1.FocusedRowHandle = 0;
                }
            }
        }


        #endregion

        #region TestEvents

        private void WireTestEvents()
        {
            HandlerHelper.TestStepped += OnTestStepped;
            HandlerHelper.TestPassed += OnTestPassed;
            HandlerHelper.TestFailed += OnTestFailed;
            HandlerHelper.TestUnloaded += OnTestUnLoad;
            HandlerHelper.TestError += OnTestError;
            HandlerHelper.TestStepping += OnTestSteppingHandler;
            HandlerHelper.PromptShowing += PromptShowing;
            HandlerHelper.PromptError += PromptError;
            HandlerHelper.PromptClosed += PromptClosed;
            
        }

        delegate void OnTestSteppingCallBack(string testName, Step step, ref string command);
        private void OnTestSteppingHandler(string testName, Step step, ref string command)
        {
            if (InvokeRequired == true)
            {
                OnTestSteppingCallBack d = new OnTestSteppingCallBack(OnTestSteppingHandler);
                Invoke(d, new object[] { testName, step, command });
            }
            else
            {
                SetStatus(step, "Processing");
            }
        }

        delegate void OnTestStepCallBack(object sender, TestSteppedEventArgs args);
        private void OnTestStepped(object sender, TestSteppedEventArgs args)
        {
            try
            {
                if (InvokeRequired == true)
                {
                    OnTestStepCallBack d = new OnTestStepCallBack(OnTestStepped);
                    Invoke(d, new object[] { sender, args });
                }
                else
                {

                    // Make sure we are not past the end of steps
                    if (m_stepNumber <= m_listTests[m_selectedTest].Steps.Count)
                    {
                        m_stepNumber++;
                        gridView1.FocusedRowHandle = m_stepNumber;
                        gridView1.SelectRow(m_stepNumber);
                        SetStatus(args.Step, "Passed");
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error Test Stepped event. Detail:" + ex.Message);
            }
        }

        delegate void OnTestUnloadCallBack(object sender, TestUnloadedEventArgs args);
        private void OnTestUnLoad(object sender, TestUnloadedEventArgs args)
        {
            try
            {
                OnTestUnloadCallBack d = new OnTestUnloadCallBack(OnTestUnLoad);
                if (InvokeRequired == true)
                {
                    BeginInvoke(d, new object[] { sender, args });
                }
                else
                {
                    HandleEndTest(false);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error Test Passed event. Detail:" + ex.Message);
            }
        }
        delegate void OnTestPassedCallBack(object sender, TestPassedEventArgs args);
        private void OnTestPassed(object sender, TestPassedEventArgs args)
        {
            try
            {
                OnTestPassedCallBack d = new OnTestPassedCallBack(OnTestPassed);
                if (InvokeRequired == true)
                {
                    BeginInvoke(d, new object[] { sender, args });
                }
                else
                {
                    HandleTestUpdate("Pass");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error Test Passed event. Detail:" + ex.Message);
            }
        }

        delegate void OnTestErrorCallBack(object sender, TestErrorEventArgs args);
        private void OnTestError(object sender, TestErrorEventArgs args)
        {
            try
            {
                if (InvokeRequired == true)
                {
                    OnTestErrorCallBack d = new OnTestErrorCallBack(OnTestError);
                    BeginInvoke(d, new object[] { sender, args });
                }
                else
                {
                    UpdateListviewStatus("Test Error", Color.Red);
                    SetStatus(args.Step, "Error");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error Test Error event. Detail:" + ex.Message);
            }

        }

        delegate void OnTestFailedCallBack(object sender, TestFailedEventArgs args);
        private void OnTestFailed(object sender, TestFailedEventArgs args)
        {
            try
            {
                if (InvokeRequired == true)
                {
                    OnTestFailedCallBack d = new OnTestFailedCallBack(OnTestFailed);
                    BeginInvoke(d, new object[] { sender, args });
                }
                else
                {
                    HandleTestUpdate("Failed");
                    SetStatus(args.Step, "Failed");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error Test Fsiled event. Detail:" + ex.Message);
            }
        }

        private void PromptShowing(string text, bool modal,  ref Form form)
        {
            
            UIDisplayForm myForm;
            if (form == null)
            {
                form = new UIDisplayForm();
                myForm = (UIDisplayForm)form;                
                myForm.SetText(text);
            }

            if (modal)
            {
                form.ShowDialog();
                form.Dispose();
                form = null;
            }
            else
            {
                myForm = (UIDisplayForm)form;
                myForm.HideOKbutton();
                myForm.Show();
            }
            
        }

        //delegate void OnPromptError(object sender, PromptErrorEventArgs args);
        private void PromptError(object sender, PromptErrorEventArgs args)
        {
            //if (InvokeRequired == true)
            //{
                //OnPromptError d = new OnPromptError(PromptError);
                //BeginInvoke(d, new object[] { sender, args });
            //}
            //else
            //{
                //return;
            //}
        }

        //delegate void OnPromptClosed(object sender, PromptEventArgs args);
        private void PromptClosed(object sender, PromptEventArgs args)
        {
            //if (InvokeRequired == true)
            //{
                //OnPromptClosed d = new OnPromptClosed(PromptClosed);
                //BeginInvoke(d, new object[] { sender, args });
            //}
            //else
            //{
                //return;
            //}
        }

        #endregion

        #region Util and Misc Methods

        private void CreateUnNamedSet()
        {
            // Now open new Set with UnNamed tag if you get this far
            m_SetFilename = "UnNamed Set";
            if (m_setElements != null)
            {
                m_setElements.RemoveAll();
                m_listTests.Clear();
                m_listWrapper.Clear();
                listViewSet.Items.Clear();
                gridControl1.DataSource = null;
                gridControl1.RefreshDataSource();
                gridControl1.Refresh();
                m_setElements = null;
            }
            // Must create root element
            m_setElements = new XElement("Tests");
        }

        private void HandleTestUpdate(string status)
        {
            UpdateListviewStatus(status);
            UpdateEndTime();
        }

        private void HandleEndTest(bool stopTesting)
        {
            // Note if single run mode is on then this will be last test and we will turn off run flag
            if (m_RunSingle == false  || stopTesting == true)
            {
                m_RunFlag = false;
                UpdateDashboard(this, new UpdateTestArgs(UpdateTestAction.TestDoneAction, "", ""));
                UpdateGridRunMode(false);
                gridView1.FocusedRowHandle = m_stepNumber = 0;
            }
            else
            {
                SequenceToNextTest();
            }
        }

        private void UpdateGridRunMode(bool testOn)
        {
            return;
            if (testOn == true)
            {
                //gridView1.Appearance.FocusedRow.BackColor = Color.Lime;
                gridView1.Appearance.SelectedRow.BackColor = Color.Lime;
                gridControl1.Refresh();
                gridView1.RefreshData();
                gridControl1.Enabled = false;
                
            }
            else
            {
                gridControl1.Enabled = true;
                //gridView1.Appearance.FocusedRow.BackColor = Color.Blue;
                gridView1.Appearance.SelectedRow.BackColor = Color.Blue;
                gridControl1.Refresh();
                gridView1.RefreshData();
            }

        }

        private void SequenceToNextTest()
        {
            // Note if single run mode is on then this will be last test and we will turn off run flag
            if (m_RunSingle == false)
            {
                m_RunFlag = false;
            }
            else
            {
                m_selectedTest++;
                if (m_selectedTest < listViewSet.Items.Count)
                {
                    // Move the selected row on item index and then run/start that test
                    listViewSet.Items[m_selectedTest].Selected = true;
                    RunTestAtIndex(m_selectedTest);
                }
                else
                {
                    m_RunFlag = false;
                }
            }
            // Any turning off in run flag should result in DashBoard getting updated 
            if (m_RunFlag == false)
            {
                UpdateDashboard(this, new UpdateTestArgs(UpdateTestAction.TestDoneAction,
                    "", ""));
                UpdateGridRunMode(false);
            }

        }

        private void InitTestSetOpenFileDialog()
        {
            openSetDialog.FileName = "";
            openSetDialog.Title = "Open Test Set";
            openSetDialog.Filter = strOpenSetFilter;

        }

        private void InitTestOpenFileDialog()
        {
            openSetDialog.FileName = "";
            openSetDialog.Title = "Open Test";
            openSetDialog.Filter = strOpenTestFilter;

        }        

        private void InitSetSaveAsDialog()
        {
            saveFileDialog.FileName = "";
            saveFileDialog.Title = "Save Set As";
            saveFileDialog.Filter = strOpenSetFilter;

        }

        private void InitTestSaveAsDialog()
        {
            saveFileDialog.FileName = "";
            saveFileDialog.Title = "Save Test As";
            saveFileDialog.Filter = strOpenTestFilter;

        }
        public void OnSaveAsSet(object sender, EventArgs args)
        {
            try
            {
                if (m_SetFilename != "")
                {
                    InitSetSaveAsDialog();
                    saveFileDialog.ShowDialog();
                    m_SetFilename = saveFileDialog.FileName;
                    SaveTestSet();
                    UpdateDashboard(this, new UpdateTestArgs(UpdateTestAction.SetNameUpdate,
                        Path.GetFileNameWithoutExtension(m_SetFilename), ""));
                    UpdateSolutionExplorer(this, new UpdateSolutionExplorerArgs(m_SetFilename,
                        m_setElements));
                }
                else
                {
                    MessageBox.Show("No current Set is loaded to save");
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show("Error could SaveAs Set File. Detail: " + ex.Message);
            }

        }

        public void OnSaveAsTest(object sender, EventArgs args)
        {
            try
            {
                // must be in a set and a test selected to do this operation
                if (m_SetFilename != "")
                {
                    int selection = GetListViewIndex();
                    if (selection != -1)
                    {
                        InitTestSaveAsDialog();
                        saveFileDialog.ShowDialog();
                        string newfilename = saveFileDialog.FileName;
                        if (newfilename != "")
                        {

                            // Update Set elements with new file
                            List<XElement> listTests = m_setElements.Elements().ToList();
                            XElement el = listTests[selection];
                            el.Value = newfilename;
                            // Note: we will not save set with this new path and filename - yet
                            Test localtest;
                            string Tmp;
                            GetTestFromIndex(selection, out Tmp, out localtest);
                            localtest.Save(Path.GetDirectoryName(newfilename), Path.GetFileName(newfilename));
                            listViewSet.Items[selection].Text = Path.GetFileNameWithoutExtension(newfilename);

                            UpdateDashboard(this, new UpdateTestArgs(UpdateTestAction.SetNameUpdate,
                                Path.GetFileNameWithoutExtension(m_SetFilename), ""));
                            UpdateSolutionExplorer(this, new UpdateSolutionExplorerArgs(m_SetFilename,
                                m_setElements));
                        }
                    }
                    else
                    {
                        MessageBox.Show("No current Test is selected or available in list.");
                    }
                }
                else
                {
                    MessageBox.Show("No current Set is loaded.");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error could not SaveAs Test File. Detail: " + ex.Message);
            }

        }
        private void OpenTestSetFile()
        {
            try
            {
                InitTestSetOpenFileDialog();
                openSetDialog.ShowDialog();
                m_SetFilename = openSetDialog.FileName;
                LoadSetElements(m_SetFilename);
                LoadSetListViewItems();
                LoadAllTests();
                if (listViewSet.Items.Count > 0)
                {
                    listViewSet.Items[0].Selected = true;
                }
                UpdateDashboard(this, new UpdateTestArgs(UpdateTestAction.SetNameUpdate,
                    Path.GetFileNameWithoutExtension(m_SetFilename), ""));
                UpdateSolutionExplorer(this, new UpdateSolutionExplorerArgs(m_SetFilename,
                    m_setElements));
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error could not load Set File: " + ex.Message);
            }
        }

        private void OpenTestSetFile(string filename)
        {
            try
            {
                LoadSetElements(filename);
                LoadSetListViewItems();
                UpdateSolutionExplorer(this, new UpdateSolutionExplorerArgs(m_SetFilename, m_setElements));
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error could not load Set File: " + ex.Message);
            }
        }

        private void LoadSetElements(string testPathName)
        {
            try
            {
                m_setElements = XElement.Load(testPathName);

            }
            catch (Exception)
            {
                throw new InvalidOperationException("Can not Load Set at specified location");
            }
        }

        private void LoadSetListViewItems()
        {
            try
            {
                listViewSet.Items.Clear();
                IEnumerable<XElement> listEnum = m_setElements.Elements();
                foreach (XElement e in listEnum)
                {
                    string filename = (string)e;
                    ListViewItem itm;
                    if (File.Exists(filename) == true)
                    {
                        itm = new ListViewItem(Path.GetFileNameWithoutExtension(filename));

                    }
                    else
                    {
                        itm = new ListViewItem(errorMsg);
                        itm.ForeColor = Color.Red;
                    }
                    itm.SubItems.Add("");
                    itm.SubItems.Add("");
                    itm.SubItems.Add("");
                    listViewSet.Items.Add(itm);
                }
            }
            catch (Exception)
            {
                throw new InvalidOperationException("Can not Load Set items");
            }
        }

        private void LoadAllTests()
        {
            try
            {
                IEnumerable<XElement> listEnum = m_setElements.Elements();
                foreach (XElement e in listEnum)
                {
                    string filename = (string)e;
                    LoadOneTest(filename);
                }
            }
            catch (Exception)
            {
                throw new InvalidOperationException("Can not Load Tests");
            }
        }

        private void LoadOneTest(string filename)
        {
            try
            {
                Test localTest = Hydrator.Hydrate(Path.GetDirectoryName(filename), Path.GetFileName(filename));
                m_listTests.Add(localTest);
                m_listWrapper.Add(new StepWrapper(localTest.Steps, new GetValue(GetStatus)));
            }
            catch (Exception)
            {
                throw new InvalidOperationException("Can not Load Test");
            }
        }

        private void RunTestAtIndex(int index)
        {
            try
            {   // TDB
                if (m_SetFilename == "")
                {
                    MessageBox.Show("No Set is currently loaded");
                }
                else if (m_RunFlag == false)
                {
                    string pathBinaries = Directory.GetCurrentDirectory();  // Get from Config? need to ADD!!!!
                    string pathConfig = @"C:\Development\Kapsoft\True Position\Test Components\Documents\config";
                    Test localtest;
                    string filename;
                    GetTestFromIndex(index, out filename, out localtest);
                    // Note: updating dash board first to prevent delay in UI button changing
                    UpdateDashboard(this, new UpdateTestArgs(UpdateTestAction.RunAction, "", Path.GetFileNameWithoutExtension(filename)));
                    TestManager.Run(localtest, HandlerHelper.Handlers, pathConfig, pathConfig);
                    UpdateGridRunMode(true);
                    UpdateStartTime();
                    m_RunFlag = true;
                }
            }
            catch (Exception ex)
            {
                // On an error at run attempt to get everything reset
                HandleTestUpdate("Fail");                
                MessageBox.Show("Error running test. " + " Error Detail: " + ex.Message);
            }
        }

        private void GetTestFromIndex(int index, out string filename, out Test localtest)
        {
            try
            {
                IEnumerable<XElement> listEnum = m_setElements.Elements();
                filename = (string)listEnum.ElementAt(index);                
                // TBD This look SO WRONG, fix on line to follow
                //localtest = Hydrator.Hydrate(Path.GetDirectoryName(filename), Path.GetFileName(filename));
                localtest = m_listTests[index];
            }
            catch (Exception)
            {
                throw new InvalidOperationException("Can not Load Test from index");
            }

        }

        private void ReNumberSteps(List<Step> steps)
        {
            int i = 1;
            foreach (Step singleStep in steps)
            {
                singleStep.Number = i++;
            }
        }

        private void ClearStepStatus()
        {
            foreach (StepWrapper wrapper in m_listWrapper[m_selectedTest])
            {
                //foreach (  
            }

        }

        #endregion

        #region Events

        public void OnRunTestSet(object sender, EventArgs args)
        {
            if (m_RunFlag == false)
            {
                m_RunSingle = false;
                RunTestAtIndex(GetListViewIndex());
            }
            else
            {
                AbortAtIndex(m_selectedTest);
                m_RunFlag = false;
            }
        }

        public void OnRunTest(object sender, EventArgs args)
        {
            if (m_RunFlag == false)
            {
                m_RunSingle = true;
                RunTestAtIndex(GetListViewIndex());
            }
            else
            {
                AbortAtIndex(m_selectedTest);
                // Note: Must wait until test unloaed to set run flag to "off" or not running
            }
        }

        private void AbortAtIndex(int index)
        {
            try
            {
                string filename;
                Test localtest;
                GetTestFromIndex(index, out filename, out localtest);
                TestManager.Abort(localtest, "Operator aborted test");
      
            }
            catch (Exception)
            {
            }


        }

        public void OnSaveTestSet(object sender, EventArgs args)
        {
            SaveTestSet();
        }

        private void SaveTestSet()
        {
            try
            {
                // better have something loaded before you save it
                if (m_setElements != null)
                {
                    int i = 0;
                    IEnumerable<XElement> listEnum = m_setElements.Elements();                   
                    foreach (Test test in m_listTests)
                    {
                        string filename = (string)listEnum.ElementAt(i);
                        TestExtensions.Save(test, Path.GetDirectoryName(filename), Path.GetFileName(filename));
                        i++;
                    }
                    m_setElements.Save(m_SetFilename);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error saving test set: " + m_SetFilename + " Error Detail: " + ex.Message);
            }
        }

        public void OnOpenTestSet(object sender, EventArgs args)
        {
            OpenTestSetFile();
        }

        public void OnNewTestSet(object sender, EventArgs args)
        {
            try
            {
                if (m_setElements != null)
                {                
                        string message = "Would you like to Save the currently loaded Set before proceeding?";
                        string caption = "Save Set";
                        MessageBoxButtons buttons = MessageBoxButtons.YesNoCancel;
                        DialogResult result;
                        result = MessageBox.Show(message, caption, buttons);
                        if (result == System.Windows.Forms.DialogResult.Yes)
                        {
                            SaveTestSet();
                        }
                        else if (result == System.Windows.Forms.DialogResult.Cancel)
                        {
                            return;
                        }
                        
                }   
                CreateUnNamedSet();
                UpdateDashboard(this, new UpdateTestArgs(UpdateTestAction.SetNameUpdate,
                    Path.GetFileNameWithoutExtension(m_SetFilename), ""));
                DoNewTest();
                
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error adding new Set." + "Error detail:" + ex.Message);
            }
        }

 
        public void OnSaveTest(object sender, EventArgs args)
        {
            try
            {
                // Get index off listview of selected test
                int index = GetListViewIndex();
                if (index != -1)
                {
                    // Then this is the test we want to save get if from List
                    Test test = m_listTests[index];
                    IEnumerable<XElement> listEnum = m_setElements.Elements();
                    string filename = (string)listEnum.ElementAt(index);
                    TestExtensions.Save(test, Path.GetDirectoryName(filename), Path.GetFileName(filename));
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error saving Test: " + m_SetFilename + " Error Detail: " + ex.Message);
            }

        }

        public void OnOpenTest(object sender, EventArgs args)
        {
            try
            {
                InitTestOpenFileDialog();
                DialogResult result = openSetDialog.ShowDialog();
                if (result == DialogResult.OK)
                {
                    string filename = openSetDialog.FileName;
                    // if a set is currently open then ask user if they want to include it in this set
                    if (m_SetFilename != "")
                    {
                        string message = "Would you like to Open  the this test into this Set?";
                        string caption = "Set Include";
                        result = MessageBox.Show(message, caption, MessageBoxButtons.YesNo);
                        if (result == System.Windows.Forms.DialogResult.No)
                        {
                            CreateUnNamedSet();
                        }
                    }
                    else
                    {
                        CreateUnNamedSet();
                    }
                    LoadOneTest(filename);
                    AddTestToListViews(filename);
                    AddTestToElements(filename);

                }
                
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error Opening Test Test: " + m_SetFilename + " Error Detail: " + ex.Message);
            }
        }

        private void AddTestToElements(string filename)
        {
            try
            {
                int index = GetListViewIndex();
                if (index == -1)
                {
                    index = 0;
                }
                List<XElement> listTests = m_setElements.Elements().ToList();
                XElement el = new XElement("Tests", new XElement("TestName", filename));
                listTests.Insert(index, el);
                m_setElements.Elements().Remove();
                m_setElements.Add(listTests);
            }
            catch { }
        }

        public void OnNewTest(object sender, EventArgs args)
        {
            DoNewTest();

        }

        private void DoNewTest()
        {
            try
            {
                // Save this for later restore of selection
                int selectedIndex = GetListViewIndex();                
                // Note: if set not open currently this becomes unNamed set
                if (m_SetFilename == "")
                {
                    m_SetFilename = "UnNamed Set";
                    m_NewTestNum = 0;
                    UpdateDashboard(this, new UpdateTestArgs(UpdateTestAction.SetNameUpdate,
                        Path.GetFileNameWithoutExtension(m_SetFilename), ""));
                }

                string filename = Directory.GetCurrentDirectory() + @"\" + "NewTest " + m_NewTestNum;
                AddTestToElements(filename);
                AddTestToListViews(filename);
                // if no selection check count of items and if only one item make it first selection
                if (selectedIndex == -1)
                {
                    if (listViewSet.Items.Count == 1)
                    {
                        selectedIndex = 0;
                    }
                }
                Test test = new Test("");
                m_listTests.Insert(selectedIndex, test);
                test.Steps = new TestList<Step>();
                m_listWrapper.Insert(selectedIndex, new StepWrapper(test.Steps));
                UpdateSolutionExplorer(this, new UpdateSolutionExplorerArgs(m_SetFilename,
                        m_setElements));

                m_NewTestNum++;

                if (listViewSet.Items.Count > 0)
                {
                    listViewSet.Items[selectedIndex].Selected = true;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Could not Add new test to Set. Detail: " + ex.Message);
            }
        }

        public void OnStepAdd(object sender, StepAddArgs args)
        {
            try
            {
                // TBD location of tests steps from config?
                string filePath = @"C:\Development\Kapsoft\True Position\UISource\TruePosition.Test\Sets";
                string filenameStep = StringEnum.GetStringValue(args.eIdentity);
                Test template = Hydrator.Hydrate(filePath, filenameStep);
                int m_selectedTest = GetListViewIndex();
                if (m_selectedTest != -1)
                {
                    // Note: templates only contain one step and one step only!
                    // Note: we are inserting after focus or selected row
                    // or if this is empty put on first row
                    if (m_listTests[m_selectedTest].Steps.Count == 0)
                    {
                        m_listTests[m_selectedTest].Steps.Insert(0, template.Steps[0]);
                    }
                    else 
                    {
                        m_listTests[m_selectedTest].Steps.Insert(gridView1.FocusedRowHandle + 1, template.Steps[0]);
                    
                    }
                    ReNumberSteps(m_listTests[0].Steps);
                    gridControl1.RefreshDataSource();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Could not find Step template or Add Step problem. Detail: " + ex.Message);
            }
        }

        #endregion

        #region Grid Control
        private void ShowMasterDetailRows()
        {
            for (int i = 0; i < gridView1.RowCount; i++)
                gridView1.SetMasterRowExpanded(i, false);
        }

        private void InitGridOptions()
        {
            gridView1.OptionsView.ShowGroupPanel = false;
            gridView1.OptionsView.ColumnAutoWidth = true;
            gridView1.OptionsSelection.EnableAppearanceFocusedRow = true;
            gridView1.Appearance.Row.Options.UseBackColor = true;
        }

        private void TestSet_Load_1(object sender, EventArgs e)
        {
            gridControl1.MainView.PopulateColumns();
            ShowMasterDetailRows();
        }
        #endregion

        private void gridView1_RowStyle(object sender, DevExpress.XtraGrid.Views.Grid.RowStyleEventArgs e)
        {
            if (m_RunFlag == true)
            {
                if (e.RowHandle == m_stepNumber)
                {
                    e.Appearance.BackColor = System.Drawing.Color.Green;
                }
            }
            else
            {
                e.Appearance.BackColor = System.Drawing.Color.White;
            }
        }
    }

    #region Misc Classes
    public enum UpdateTestAction
    { 
        // Reserved 0 to 100 for DashBoard updates
        RunAction = 0, 
        SetNameUpdate, 
        TestDoneAction,
        // 101 to 200 for Explorer to TestSet command
        TestUp = 101,
        TestDown,
        TestDelete,
        TestAdd,
        // 201 to 300 Log Output update action
        UpdateLog = 201
    }

    public class UpdateTestArgs : EventArgs
    {
        public string Text1 { get; set; }
        public string Text2 { get; set; }
        public UpdateTestAction eAction { get; set; }
        public int Index { get; set; }

        public UpdateTestArgs(UpdateTestAction action, string text1, string text2)
        {
            eAction = action;
            Text1 = text1;
            Text2 = text2;
        }
        public UpdateTestArgs(UpdateTestAction action)
        {
            eAction = action;
        }

        public UpdateTestArgs(UpdateTestAction action, int index)
        {
            eAction = action;
            Index = index;
        }
    }

    public class UpdateSolutionExplorerArgs : EventArgs
    {
        public string SetName { get; set; }
        public XElement ListElements { get; set; }
        public UpdateSolutionExplorerArgs(string setname, XElement listelements)
        {
            SetName = setname;
            ListElements = listelements;
        }

    }
    #endregion
}
