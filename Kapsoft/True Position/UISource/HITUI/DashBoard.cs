using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using WeifenLuo.WinFormsUI.Docking;
using TruePosition.Test.IO;
using TruePosition.Test.UI;

namespace HITUI
{
    public partial class DashBoard : DockContent 
    {
        public event EventHandler RunTestSet = null;
        private Bitmap m_abortImage = new Bitmap(@"resources/error.png");
        private Bitmap m_runImage = new Bitmap(@"resources/Network_ConnectTo.png");
        private bool m_Run;

        public DashBoard()
        {
            InitializeComponent();
            m_Run = false;
            pictureBox1.Visible = false;
            HideOnClose = true;
        }

        private void buttonRun_Click(object sender, EventArgs e)
        {
            if (m_Run == false)
            {
                RunTestSet(this, new EventArgs());
                DoRun();
            }
            else
            {
                DoStop();
            }
        }

        private void DoStop()
        {
            // set abort in motion here
            buttonRun.Image = m_runImage;
            m_Run = false;
            buttonRun.Text = "Run!";
            pictureBox1.Visible = false;
            textBoxActiveTest.Text  = "";
        }

        private void DoRun()
        {
            if (m_Run == false)
            {
                buttonRun.Image = m_abortImage;
                m_Run = true;
                buttonRun.Text = "Abort!";
                pictureBox1.Visible = true;
            }
        }

        public void OnUpdateDashboard (object sender, UpdateTestArgs args)
        {
            switch (args.eAction)
            {
                case UpdateTestAction.RunAction:
                    textBoxActiveTest.Text = args.Text1;
                    DoRun();
                    break;
                case UpdateTestAction.SetNameUpdate:
                    textBoxCurrentSet.Text = args.Text2;
                    break;
                case UpdateTestAction.TestDoneAction:
                    DoStop();
                    break;
            }            
        }               
    }
}
