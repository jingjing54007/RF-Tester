using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using WeifenLuo.WinFormsUI.Docking;

namespace HITUI
{
    public partial class DummyToolbox : ToolWindow
    {
       
        // Set Events declares
        public event EventHandler RunTestSet = null;
        public event EventHandler LoadTestSet = null;
        public event EventHandler SaveTestSet = null;
        public event EventHandler OpenTestSet = null;
        public event EventHandler NewTestSet = null;        

        // Test Events declares
        public event EventHandler LoadTest = null;
        public event EventHandler SaveTest = null;
        public event EventHandler OpenTest = null;
        public event EventHandler NewTest = null;
        public event EventHandler RunTest = null;

        // Step Event declares
        public event EventHandler <StepAddArgs> StepAdd = null;

       



        public DummyToolbox()
        {
            InitializeComponent();
        }

        #region Set and Test Events
        private void navBarRunSet_LinkClicked(object sender, DevExpress.XtraNavBar.NavBarLinkEventArgs e)
        {
            RunTestSet (this, new EventArgs() );
             
        }

        private void navBarOpenSet_LinkClicked(object sender, DevExpress.XtraNavBar.NavBarLinkEventArgs e)
        {
            OpenTestSet(this, new EventArgs());
        }

        private void navBarSaveSet_LinkClicked(object sender, DevExpress.XtraNavBar.NavBarLinkEventArgs e)
        {
            SaveTestSet(this, new EventArgs());
        }

        private void navBarRunTest_LinkClicked(object sender, DevExpress.XtraNavBar.NavBarLinkEventArgs e)
        {
            RunTest(this, new EventArgs());
       
        }

        private void navBarNewTest_LinkClicked(object sender, DevExpress.XtraNavBar.NavBarLinkEventArgs e)
        {
            NewTest(this, new EventArgs());
        }

        private void navBarNewSet_LinkClicked(object sender, DevExpress.XtraNavBar.NavBarLinkEventArgs e)
        {
            NewTestSet(this, new EventArgs());
        }
        private void navBarSaveTest_LinkClicked(object sender, DevExpress.XtraNavBar.NavBarLinkEventArgs e)
        {
            SaveTest(this, new EventArgs());
        }

        private void navBarOpenTest_LinkClicked(object sender, DevExpress.XtraNavBar.NavBarLinkEventArgs e)
        {
            OpenTest(this, new EventArgs());
        }

        #endregion

        #region Step Add Events

        private void navBarLMUConsoleCmd_LinkClicked(object sender, DevExpress.XtraNavBar.NavBarLinkEventArgs e)
        {
            StepAdd(this, new StepAddArgs(StepIdentity.Console));
        }

        private void navBarProcessStep_LinkClicked(object sender, DevExpress.XtraNavBar.NavBarLinkEventArgs e)
        {
            StepAdd(this, new StepAddArgs(StepIdentity.Process));
        }

        private void navBarFreqStep_LinkClicked(object sender, DevExpress.XtraNavBar.NavBarLinkEventArgs e)
        {
            StepAdd(this, new StepAddArgs(StepIdentity.SigFreq));
        }

        private void navBarSigGenPower_LinkClicked(object sender, DevExpress.XtraNavBar.NavBarLinkEventArgs e)
        {
            StepAdd(this, new StepAddArgs(StepIdentity.SigPower));
        }

        private void navSigGenOn_LinkClicked(object sender, DevExpress.XtraNavBar.NavBarLinkEventArgs e)
        {
            StepAdd(this, new StepAddArgs(StepIdentity.SigOn));
        }

        private void navBarSigGenOff_LinkClicked(object sender, DevExpress.XtraNavBar.NavBarLinkEventArgs e)
        {
            StepAdd(this, new StepAddArgs(StepIdentity.SigOff));
        }

        private void navBarSigGenNoise_LinkClicked(object sender, DevExpress.XtraNavBar.NavBarLinkEventArgs e)
        {
            StepAdd(this, new StepAddArgs(StepIdentity.SigNoise));
        }

        private void navBarRecorder_LinkClicked(object sender, DevExpress.XtraNavBar.NavBarLinkEventArgs e)
        {
            StepAdd(this, new StepAddArgs(StepIdentity.Recorder));
        }

        private void navBarTDOA_LinkClicked(object sender, DevExpress.XtraNavBar.NavBarLinkEventArgs e)
        {
            StepAdd(this, new StepAddArgs(StepIdentity.TDOA));
        }

        private void navBarDisplayModalDialog_LinkClicked(object sender, DevExpress.XtraNavBar.NavBarLinkEventArgs e)
        {
            StepAdd(this, new StepAddArgs(StepIdentity.UIWaitDialog));
        }

        private void navBarTimerWithDialog_LinkClicked(object sender, DevExpress.XtraNavBar.NavBarLinkEventArgs e)
        {
            StepAdd(this, new StepAddArgs(StepIdentity.UIWaitWithTimeout));
        }

        private void navBarDelayTest_LinkClicked(object sender, DevExpress.XtraNavBar.NavBarLinkEventArgs e)
        {
            StepAdd(this, new StepAddArgs(StepIdentity.Delay));
        }

        private void navBarPowerSupplyOn_LinkClicked(object sender, DevExpress.XtraNavBar.NavBarLinkEventArgs e)
        {
            StepAdd(this, new StepAddArgs(StepIdentity.PowerSupplyOn));
        }

        private void navBarPowerSupplyOff_LinkClicked(object sender, DevExpress.XtraNavBar.NavBarLinkEventArgs e)
        {
            StepAdd(this, new StepAddArgs(StepIdentity.PowerSupplyOff));
        }

        private void navBarSNMPGet_LinkClicked(object sender, DevExpress.XtraNavBar.NavBarLinkEventArgs e)
        {
            StepAdd(this, new StepAddArgs(StepIdentity.SNMPGET));
        }

        private void navBarSNMPSet_LinkClicked(object sender, DevExpress.XtraNavBar.NavBarLinkEventArgs e)
        {
            StepAdd(this, new StepAddArgs(StepIdentity.SNMPSET));
        }

        private void navBarTelnet_LinkClicked(object sender, DevExpress.XtraNavBar.NavBarLinkEventArgs e)
        {
            StepAdd(this, new StepAddArgs(StepIdentity.Telnet));
        }

        #endregion

        
        
    }

    // These are adorned with filename of xml template to be used. Receiver of event will
    // get this filename out on the other side. 
    public enum StepIdentity
    {
        [StringValue("MapStep.xml")]
        Process = 0,
        [StringValue("SigPowerStep.xml")]
        SigPower,
        [StringValue("SigFreqStep.xml")]
        SigFreq,
        [StringValue("SigOnStep.xml")]
        SigOn,
        [StringValue("SigOffStep.xml")]
        SigOff,
        [StringValue("SigNoiseStep.xml")]
        SigNoise,
        [StringValue("ConsoleStep.xml")]
        Console,
        [StringValue("TelnetSMLC.xml")]
        Telnet,
        [StringValue("PowerOffStep.xml")]
        PowerSupplyOff,
        [StringValue("PowerOnStep.xml")]
        PowerSupplyOn,
        [StringValue("DelayStep.xml")]
        Delay,
        [StringValue("UIWaitWithTimeoutStep.xml")]
        UIWaitWithTimeout,
        [StringValue("UIWaitDialogStep.xml")]
        UIWaitDialog,
        [StringValue("SNMPGETStep.xml")]
        SNMPGET,
        [StringValue("SNMPSETStep.xml")]
        SNMPSET,
        [StringValue("RecorderStep.xml")]
        Recorder,
        [StringValue("TDOAStep.xml")]
        TDOA
    }

    public class StepAddArgs : EventArgs
    {
        public StepIdentity eIdentity { get; set; }

        public StepAddArgs(StepIdentity ident)
        {
            eIdentity = ident;
        }
    }
}