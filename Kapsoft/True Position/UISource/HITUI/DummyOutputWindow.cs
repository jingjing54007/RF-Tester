using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using WeifenLuo.WinFormsUI.Docking;
using TruePosition.Test.IO;
using TruePosition.Test.QF;
using TruePosition.Test.UI;


namespace HITUI
{
    

    public partial class DummyOutputWindow : DockContent 
    {
        public event EventHandler<UpdateTestArgs> UpdateLog = null;
        private string PortName { get; set; }
        public ISerialConfig _port;
        public ISerialConfig Port
        {        
            get { return Port = _port; }
            set { _port = value;}
        }
       
      

        public DummyOutputWindow()
        {
            InitializeComponent();
            HideOnClose = true;
            TextBox1.AppendText(">");
            
        }

        public DummyOutputWindow(string portname) : base()
        {
            try
            {
                PortName = portname;
                InitPort();
            }
            catch { }
        }

        private void InitPort() 
        {
            try
            {
                // TBD DJK
                //ConfigManager.SetLocation(ConfigPath);
                ConfigManager.Load(PortName, out _port);
                QFManager.Port.Create((IPort)Port);
                QFManager.Port.Transmitted(PortName, OnDataTransmitted);
                QFManager.Port.Receive(PortName, OnDataReceived);
                QFManager.Port.Error(PortName, OnPortError);
                QFManager.Port.ReceiveTimeout(PortName, OnPortTimeout);
                QFManager.Port.Open(PortName);
            }
            catch
            {
            }        
        }

        private void OnDataReceived(object sender, PortMessageEventArgs args)
        {
            try
            {
                TextBox1.AppendText("/r/n");
                TextBox1.AppendText(">" + args.Message + "/r/n" + ">");
            }
            catch { }
            
        }

        private void OnPortError(object sender, PortErrorEventArgs args)
        {
            try
            {
               
            }
            catch { }

        }
        private void OnPortTimeout(object sender, PortTimeoutExpiredEventArgs args)
        {
            try
            {

            }
            catch { }

        }

        private void OnDataTransmitted(object sender, PortMessageEventArgs args)
        {
            try
            {
               
            }
            catch { }
        }

        private void LogData(string sender, string message)
        {
            
        }

        private void comboBox_SelectedIndexChanged(object sender, EventArgs e)
        {

        }
        protected override string GetPersistString()
        {
            // Add extra information into the persist string for this document
            // so that it is available when deserialized.
            return GetType().ToString() + "," + this.Text;
        }

        private void TextBox1_KeyDown(object sender, KeyEventArgs e)
        {

            
        }

        public void OnUpdateOutputWindowState(object sender, UpdateTestArgs args)
        {
            try
            {
                switch (args.eAction)
                {
                    case UpdateTestAction.RunAction:
                        QFManager.Port.Close(PortName);
                        break;
                    case UpdateTestAction.TestDoneAction:
                        QFManager.Port.Open(PortName);
                        break;
                }
            }
            catch { }
        }
      
        private int GetLineNumber()
        {
            return TextBox1.GetLineFromCharIndex(TextBox1.SelectionStart);
        }

        private void TextBox1_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    int linenumber = GetLineNumber();
                    string strMsg;
                    strMsg = TextBox1.Lines[linenumber-1];
                    // Just ignore first character ">" which we placed there before
                    strMsg = strMsg.Substring(1);
                    UpdateLog(this, new UpdateTestArgs(UpdateTestAction.UpdateLog,
                        "Operator " + PortName, strMsg));
                    // Send to Port here
                    QFManager.Port.Transmit(PortName, strMsg);
                    TextBox1.AppendText(">");
                }
            }
            catch { }
        }
    }
}