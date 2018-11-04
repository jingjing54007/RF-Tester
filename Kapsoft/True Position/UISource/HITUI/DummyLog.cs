using System;
using System.IO;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace HITUI
{
    public partial class DummyLog : WeifenLuo.WinFormsUI.Docking.DockContent
    {
        private StreamWriter _fileLog = null;
        private string _fileName = "";

        public DummyLog()
        {
            InitializeComponent();
            CreateFilename();
            OpenFileLog();
            HideOnClose = true;
        }

        private void OpenFileLog()
        {
            _fileLog = new StreamWriter(_fileName);

        }

        public void OnUpdateLog(object sender, UpdateTestArgs args)
        {
            WriteMessage(args.Text1, args.Text2);
        }

        private void CreateFilename()
        {
            string LogDir = GlobalConfigData.DataConfig.LogFileFolder;

            // Make sure something is in config data for this value otherwise put it in working dir
            if (Directory.Exists(LogDir) == false)
            {
                LogDir = Directory.GetCurrentDirectory();
            }
            // Note: remove colons - not a valid filename character :(
            string tmp = String.Format("{0:u} LMU Console Log.txt", DateTime.Now);
            tmp = tmp.Replace(":", "");
            _fileName = Path.Combine(LogDir,tmp);
        }

        private void WriteMessage(string creator, string message)
        {
            try
            {
                string Tmp = String.Format("[{0}][{1}]{2}", creator, DateTime.Now, message);
                _fileLog.WriteLine(Tmp);
                _fileLog.Flush();
                TextBox1.AppendText(Tmp + "\r\n");

            }
            catch { }

        }
    }
}
