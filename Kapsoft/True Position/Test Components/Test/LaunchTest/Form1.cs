using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace LaunchTest
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            //System.Threading.Thread.Sleep(10000);
        }

        delegate void OnTestSteppingCallBack(string testName, object step, ref string command);
        private void OnTestSteppingHandler(string testName, object step, ref string command)
        {
            if (InvokeRequired == true)
            {
                OnTestSteppingCallBack d = new OnTestSteppingCallBack(OnTestSteppingHandler);
                Invoke(d, new object[] { testName, step });
            }
            else
            {
            }
        }

        private event OnTestSteppingCallBack callback = null;
        private void button1_Click(object sender, EventArgs e)
        {
            callback += new OnTestSteppingCallBack(OnTestSteppingHandler);
            string command = null;
            System.Threading.ThreadPool.QueueUserWorkItem((o) => { callback("name", null, ref command); });
        }
    }
}
