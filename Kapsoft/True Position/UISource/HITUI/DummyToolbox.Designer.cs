namespace HITUI
{
    partial class DummyToolbox
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(DummyToolbox));
            this.imageList = new System.Windows.Forms.ImageList(this.components);
            this.navBarControl1 = new DevExpress.XtraNavBar.NavBarControl();
            this.navBarGroup1 = new DevExpress.XtraNavBar.NavBarGroup();
            this.navBarRunSet = new DevExpress.XtraNavBar.NavBarItem();
            this.navBarSaveSet = new DevExpress.XtraNavBar.NavBarItem();
            this.navBarOpenSet = new DevExpress.XtraNavBar.NavBarItem();
            this.navBarNewSet = new DevExpress.XtraNavBar.NavBarItem();
            this.navBarGroup2 = new DevExpress.XtraNavBar.NavBarGroup();
            this.navBarRunTest = new DevExpress.XtraNavBar.NavBarItem();
            this.navBarSaveTest = new DevExpress.XtraNavBar.NavBarItem();
            this.navBarOpenTest = new DevExpress.XtraNavBar.NavBarItem();
            this.navBarNewTest = new DevExpress.XtraNavBar.NavBarItem();
            this.navBarGroup3 = new DevExpress.XtraNavBar.NavBarGroup();
            this.navBarRecorder = new DevExpress.XtraNavBar.NavBarItem();
            this.navBarProcessStep = new DevExpress.XtraNavBar.NavBarItem();
            this.navBarSNMPGet = new DevExpress.XtraNavBar.NavBarItem();
            this.navBarSNMPSet = new DevExpress.XtraNavBar.NavBarItem();
            this.navBarFreqStep = new DevExpress.XtraNavBar.NavBarItem();
            this.navBarSigGenPower = new DevExpress.XtraNavBar.NavBarItem();
            this.navSigGenOn = new DevExpress.XtraNavBar.NavBarItem();
            this.navBarSigGenOff = new DevExpress.XtraNavBar.NavBarItem();
            this.navBarSigGenNoise = new DevExpress.XtraNavBar.NavBarItem();
            this.navBarLMUConsoleCmd = new DevExpress.XtraNavBar.NavBarItem();
            this.navBarTDOA = new DevExpress.XtraNavBar.NavBarItem();
            this.navBarDisplayModalDialog = new DevExpress.XtraNavBar.NavBarItem();
            this.navBarTimerWithDialog = new DevExpress.XtraNavBar.NavBarItem();
            this.navBarDelayTest = new DevExpress.XtraNavBar.NavBarItem();
            this.navBarPowerSupplyOn = new DevExpress.XtraNavBar.NavBarItem();
            this.navBarPowerSupplyOff = new DevExpress.XtraNavBar.NavBarItem();
            this.navBarTemperature = new DevExpress.XtraNavBar.NavBarItem();
            this.imageList2 = new System.Windows.Forms.ImageList(this.components);
            this.imageList1 = new System.Windows.Forms.ImageList(this.components);
            this.navBarTelnet = new DevExpress.XtraNavBar.NavBarItem();
            ((System.ComponentModel.ISupportInitialize)(this.navBarControl1)).BeginInit();
            this.SuspendLayout();
            // 
            // imageList
            // 
            this.imageList.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imageList.ImageStream")));
            this.imageList.TransparentColor = System.Drawing.Color.Transparent;
            this.imageList.Images.SetKeyName(0, "Mouse.bmp");
            // 
            // navBarControl1
            // 
            this.navBarControl1.ActiveGroup = this.navBarGroup1;
            this.navBarControl1.ContentButtonHint = null;
            this.navBarControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.navBarControl1.Groups.AddRange(new DevExpress.XtraNavBar.NavBarGroup[] {
            this.navBarGroup1,
            this.navBarGroup2,
            this.navBarGroup3});
            this.navBarControl1.Items.AddRange(new DevExpress.XtraNavBar.NavBarItem[] {
            this.navBarNewSet,
            this.navBarRunSet,
            this.navBarSaveSet,
            this.navBarOpenSet,
            this.navBarRunTest,
            this.navBarSaveTest,
            this.navBarOpenTest,
            this.navBarNewTest,
            this.navBarFreqStep,
            this.navSigGenOn,
            this.navBarSigGenOff,
            this.navBarSigGenNoise,
            this.navBarLMUConsoleCmd,
            this.navBarSigGenPower,
            this.navBarProcessStep,
            this.navBarSNMPGet,
            this.navBarSNMPSet,
            this.navBarPowerSupplyOn,
            this.navBarPowerSupplyOff,
            this.navBarDisplayModalDialog,
            this.navBarTimerWithDialog,
            this.navBarDelayTest,
            this.navBarRecorder,
            this.navBarTDOA,
            this.navBarTemperature,
            this.navBarTelnet});
            this.navBarControl1.Location = new System.Drawing.Point(0, 2);
            this.navBarControl1.Name = "navBarControl1";
            this.navBarControl1.OptionsNavPane.ExpandedWidth = 171;
            this.navBarControl1.Size = new System.Drawing.Size(171, 353);
            this.navBarControl1.SmallImages = this.imageList2;
            this.navBarControl1.TabIndex = 0;
            this.navBarControl1.View = new DevExpress.XtraNavBar.ViewInfo.UltraFlatExplorerBarViewInfoRegistrator();
            // 
            // navBarGroup1
            // 
            this.navBarGroup1.Caption = "Sets";
            this.navBarGroup1.Expanded = true;
            this.navBarGroup1.ItemLinks.AddRange(new DevExpress.XtraNavBar.NavBarItemLink[] {
            new DevExpress.XtraNavBar.NavBarItemLink(this.navBarRunSet),
            new DevExpress.XtraNavBar.NavBarItemLink(this.navBarSaveSet),
            new DevExpress.XtraNavBar.NavBarItemLink(this.navBarOpenSet),
            new DevExpress.XtraNavBar.NavBarItemLink(this.navBarNewSet)});
            this.navBarGroup1.Name = "navBarGroup1";
            this.navBarGroup1.TopVisibleLinkIndex = 1;
            // 
            // navBarRunSet
            // 
            this.navBarRunSet.Caption = "Run!";
            this.navBarRunSet.LargeImage = ((System.Drawing.Image)(resources.GetObject("navBarRunSet.LargeImage")));
            this.navBarRunSet.Name = "navBarRunSet";
            this.navBarRunSet.LinkClicked += new DevExpress.XtraNavBar.NavBarLinkEventHandler(this.navBarRunSet_LinkClicked);
            // 
            // navBarSaveSet
            // 
            this.navBarSaveSet.Caption = "Save";
            this.navBarSaveSet.LargeImage = ((System.Drawing.Image)(resources.GetObject("navBarSaveSet.LargeImage")));
            this.navBarSaveSet.Name = "navBarSaveSet";
            this.navBarSaveSet.LinkClicked += new DevExpress.XtraNavBar.NavBarLinkEventHandler(this.navBarSaveSet_LinkClicked);
            // 
            // navBarOpenSet
            // 
            this.navBarOpenSet.Caption = "Open...";
            this.navBarOpenSet.LargeImage = ((System.Drawing.Image)(resources.GetObject("navBarOpenSet.LargeImage")));
            this.navBarOpenSet.Name = "navBarOpenSet";
            this.navBarOpenSet.LinkClicked += new DevExpress.XtraNavBar.NavBarLinkEventHandler(this.navBarOpenSet_LinkClicked);
            // 
            // navBarNewSet
            // 
            this.navBarNewSet.Caption = "New";
            this.navBarNewSet.LargeImage = ((System.Drawing.Image)(resources.GetObject("navBarNewSet.LargeImage")));
            this.navBarNewSet.Name = "navBarNewSet";
            this.navBarNewSet.LinkClicked += new DevExpress.XtraNavBar.NavBarLinkEventHandler(this.navBarNewSet_LinkClicked);
            // 
            // navBarGroup2
            // 
            this.navBarGroup2.Caption = "Test";
            this.navBarGroup2.ItemLinks.AddRange(new DevExpress.XtraNavBar.NavBarItemLink[] {
            new DevExpress.XtraNavBar.NavBarItemLink(this.navBarRunTest),
            new DevExpress.XtraNavBar.NavBarItemLink(this.navBarSaveTest),
            new DevExpress.XtraNavBar.NavBarItemLink(this.navBarOpenTest),
            new DevExpress.XtraNavBar.NavBarItemLink(this.navBarNewTest)});
            this.navBarGroup2.Name = "navBarGroup2";
            // 
            // navBarRunTest
            // 
            this.navBarRunTest.Caption = "Run Single Test";
            this.navBarRunTest.LargeImage = ((System.Drawing.Image)(resources.GetObject("navBarRunTest.LargeImage")));
            this.navBarRunTest.Name = "navBarRunTest";
            this.navBarRunTest.LinkClicked += new DevExpress.XtraNavBar.NavBarLinkEventHandler(this.navBarRunTest_LinkClicked);
            // 
            // navBarSaveTest
            // 
            this.navBarSaveTest.Caption = "Save";
            this.navBarSaveTest.LargeImage = ((System.Drawing.Image)(resources.GetObject("navBarSaveTest.LargeImage")));
            this.navBarSaveTest.Name = "navBarSaveTest";
            this.navBarSaveTest.LinkClicked += new DevExpress.XtraNavBar.NavBarLinkEventHandler(this.navBarSaveTest_LinkClicked);
            // 
            // navBarOpenTest
            // 
            this.navBarOpenTest.Caption = "Open...";
            this.navBarOpenTest.LargeImage = ((System.Drawing.Image)(resources.GetObject("navBarOpenTest.LargeImage")));
            this.navBarOpenTest.Name = "navBarOpenTest";
            this.navBarOpenTest.LinkClicked += new DevExpress.XtraNavBar.NavBarLinkEventHandler(this.navBarOpenTest_LinkClicked);
            // 
            // navBarNewTest
            // 
            this.navBarNewTest.Caption = "New";
            this.navBarNewTest.LargeImage = ((System.Drawing.Image)(resources.GetObject("navBarNewTest.LargeImage")));
            this.navBarNewTest.Name = "navBarNewTest";
            this.navBarNewTest.LinkClicked += new DevExpress.XtraNavBar.NavBarLinkEventHandler(this.navBarNewTest_LinkClicked);
            // 
            // navBarGroup3
            // 
            this.navBarGroup3.Caption = "Steps";
            this.navBarGroup3.Expanded = true;
            this.navBarGroup3.ItemLinks.AddRange(new DevExpress.XtraNavBar.NavBarItemLink[] {
            new DevExpress.XtraNavBar.NavBarItemLink(this.navBarRecorder),
            new DevExpress.XtraNavBar.NavBarItemLink(this.navBarProcessStep),
            new DevExpress.XtraNavBar.NavBarItemLink(this.navBarSNMPGet),
            new DevExpress.XtraNavBar.NavBarItemLink(this.navBarSNMPSet),
            new DevExpress.XtraNavBar.NavBarItemLink(this.navBarFreqStep),
            new DevExpress.XtraNavBar.NavBarItemLink(this.navBarSigGenPower),
            new DevExpress.XtraNavBar.NavBarItemLink(this.navSigGenOn),
            new DevExpress.XtraNavBar.NavBarItemLink(this.navBarSigGenOff),
            new DevExpress.XtraNavBar.NavBarItemLink(this.navBarSigGenNoise),
            new DevExpress.XtraNavBar.NavBarItemLink(this.navBarTelnet),
            new DevExpress.XtraNavBar.NavBarItemLink(this.navBarTDOA),
            new DevExpress.XtraNavBar.NavBarItemLink(this.navBarDisplayModalDialog),
            new DevExpress.XtraNavBar.NavBarItemLink(this.navBarLMUConsoleCmd),
            new DevExpress.XtraNavBar.NavBarItemLink(this.navBarTimerWithDialog),
            new DevExpress.XtraNavBar.NavBarItemLink(this.navBarDelayTest),
            new DevExpress.XtraNavBar.NavBarItemLink(this.navBarPowerSupplyOn),
            new DevExpress.XtraNavBar.NavBarItemLink(this.navBarPowerSupplyOff),
            new DevExpress.XtraNavBar.NavBarItemLink(this.navBarTemperature)});
            this.navBarGroup3.Name = "navBarGroup3";
            this.navBarGroup3.TopVisibleLinkIndex = 3;
            // 
            // navBarRecorder
            // 
            this.navBarRecorder.Caption = "Recorder";
            this.navBarRecorder.LargeImage = ((System.Drawing.Image)(resources.GetObject("navBarRecorder.LargeImage")));
            this.navBarRecorder.Name = "navBarRecorder";
            this.navBarRecorder.LinkClicked += new DevExpress.XtraNavBar.NavBarLinkEventHandler(this.navBarRecorder_LinkClicked);
            // 
            // navBarProcessStep
            // 
            this.navBarProcessStep.Caption = "Process Step";
            this.navBarProcessStep.LargeImage = ((System.Drawing.Image)(resources.GetObject("navBarProcessStep.LargeImage")));
            this.navBarProcessStep.Name = "navBarProcessStep";
            this.navBarProcessStep.LinkClicked += new DevExpress.XtraNavBar.NavBarLinkEventHandler(this.navBarProcessStep_LinkClicked);
            // 
            // navBarSNMPGet
            // 
            this.navBarSNMPGet.Caption = "SNMP Get";
            this.navBarSNMPGet.LargeImage = ((System.Drawing.Image)(resources.GetObject("navBarSNMPGet.LargeImage")));
            this.navBarSNMPGet.Name = "navBarSNMPGet";
            this.navBarSNMPGet.LinkClicked += new DevExpress.XtraNavBar.NavBarLinkEventHandler(this.navBarSNMPGet_LinkClicked);
            // 
            // navBarSNMPSet
            // 
            this.navBarSNMPSet.Caption = "SNMP Set";
            this.navBarSNMPSet.LargeImage = ((System.Drawing.Image)(resources.GetObject("navBarSNMPSet.LargeImage")));
            this.navBarSNMPSet.Name = "navBarSNMPSet";
            this.navBarSNMPSet.LinkClicked += new DevExpress.XtraNavBar.NavBarLinkEventHandler(this.navBarSNMPSet_LinkClicked);
            // 
            // navBarFreqStep
            // 
            this.navBarFreqStep.Caption = "SigGen Frequency";
            this.navBarFreqStep.LargeImage = ((System.Drawing.Image)(resources.GetObject("navBarFreqStep.LargeImage")));
            this.navBarFreqStep.Name = "navBarFreqStep";
            this.navBarFreqStep.LinkClicked += new DevExpress.XtraNavBar.NavBarLinkEventHandler(this.navBarFreqStep_LinkClicked);
            // 
            // navBarSigGenPower
            // 
            this.navBarSigGenPower.Caption = "SigGen Power";
            this.navBarSigGenPower.LargeImage = ((System.Drawing.Image)(resources.GetObject("navBarSigGenPower.LargeImage")));
            this.navBarSigGenPower.Name = "navBarSigGenPower";
            this.navBarSigGenPower.LinkClicked += new DevExpress.XtraNavBar.NavBarLinkEventHandler(this.navBarSigGenPower_LinkClicked);
            // 
            // navSigGenOn
            // 
            this.navSigGenOn.Caption = "SigGen On";
            this.navSigGenOn.LargeImage = ((System.Drawing.Image)(resources.GetObject("navSigGenOn.LargeImage")));
            this.navSigGenOn.Name = "navSigGenOn";
            this.navSigGenOn.LinkClicked += new DevExpress.XtraNavBar.NavBarLinkEventHandler(this.navSigGenOn_LinkClicked);
            // 
            // navBarSigGenOff
            // 
            this.navBarSigGenOff.Caption = "SigGen Off";
            this.navBarSigGenOff.LargeImage = ((System.Drawing.Image)(resources.GetObject("navBarSigGenOff.LargeImage")));
            this.navBarSigGenOff.Name = "navBarSigGenOff";
            this.navBarSigGenOff.LinkClicked += new DevExpress.XtraNavBar.NavBarLinkEventHandler(this.navBarSigGenOff_LinkClicked);
            // 
            // navBarSigGenNoise
            // 
            this.navBarSigGenNoise.Caption = "SigGen Noise";
            this.navBarSigGenNoise.LargeImage = ((System.Drawing.Image)(resources.GetObject("navBarSigGenNoise.LargeImage")));
            this.navBarSigGenNoise.Name = "navBarSigGenNoise";
            this.navBarSigGenNoise.LinkClicked += new DevExpress.XtraNavBar.NavBarLinkEventHandler(this.navBarSigGenNoise_LinkClicked);
            // 
            // navBarLMUConsoleCmd
            // 
            this.navBarLMUConsoleCmd.Caption = "LMU Console ";
            this.navBarLMUConsoleCmd.LargeImage = ((System.Drawing.Image)(resources.GetObject("navBarLMUConsoleCmd.LargeImage")));
            this.navBarLMUConsoleCmd.Name = "navBarLMUConsoleCmd";
            this.navBarLMUConsoleCmd.LinkClicked += new DevExpress.XtraNavBar.NavBarLinkEventHandler(this.navBarLMUConsoleCmd_LinkClicked);
            // 
            // navBarTDOA
            // 
            this.navBarTDOA.Caption = "TDOA Command";
            this.navBarTDOA.LargeImage = ((System.Drawing.Image)(resources.GetObject("navBarTDOA.LargeImage")));
            this.navBarTDOA.Name = "navBarTDOA";
            this.navBarTDOA.LinkClicked += new DevExpress.XtraNavBar.NavBarLinkEventHandler(this.navBarTDOA_LinkClicked);
            // 
            // navBarDisplayModalDialog
            // 
            this.navBarDisplayModalDialog.Caption = "UI Display Dialog";
            this.navBarDisplayModalDialog.LargeImage = ((System.Drawing.Image)(resources.GetObject("navBarDisplayModalDialog.LargeImage")));
            this.navBarDisplayModalDialog.Name = "navBarDisplayModalDialog";
            this.navBarDisplayModalDialog.LinkClicked += new DevExpress.XtraNavBar.NavBarLinkEventHandler(this.navBarDisplayModalDialog_LinkClicked);
            // 
            // navBarTimerWithDialog
            // 
            this.navBarTimerWithDialog.Caption = "UI Timer with Dialog";
            this.navBarTimerWithDialog.LargeImage = ((System.Drawing.Image)(resources.GetObject("navBarTimerWithDialog.LargeImage")));
            this.navBarTimerWithDialog.Name = "navBarTimerWithDialog";
            this.navBarTimerWithDialog.LinkClicked += new DevExpress.XtraNavBar.NavBarLinkEventHandler(this.navBarTimerWithDialog_LinkClicked);
            // 
            // navBarDelayTest
            // 
            this.navBarDelayTest.Caption = "Delay Test";
            this.navBarDelayTest.LargeImage = ((System.Drawing.Image)(resources.GetObject("navBarDelayTest.LargeImage")));
            this.navBarDelayTest.Name = "navBarDelayTest";
            this.navBarDelayTest.LinkClicked += new DevExpress.XtraNavBar.NavBarLinkEventHandler(this.navBarDelayTest_LinkClicked);
            // 
            // navBarPowerSupplyOn
            // 
            this.navBarPowerSupplyOn.Caption = "Power Supply On";
            this.navBarPowerSupplyOn.LargeImage = ((System.Drawing.Image)(resources.GetObject("navBarPowerSupplyOn.LargeImage")));
            this.navBarPowerSupplyOn.Name = "navBarPowerSupplyOn";
            this.navBarPowerSupplyOn.LinkClicked += new DevExpress.XtraNavBar.NavBarLinkEventHandler(this.navBarPowerSupplyOn_LinkClicked);
            // 
            // navBarPowerSupplyOff
            // 
            this.navBarPowerSupplyOff.Caption = "Power Supply Off";
            this.navBarPowerSupplyOff.LargeImage = ((System.Drawing.Image)(resources.GetObject("navBarPowerSupplyOff.LargeImage")));
            this.navBarPowerSupplyOff.Name = "navBarPowerSupplyOff";
            this.navBarPowerSupplyOff.LinkClicked += new DevExpress.XtraNavBar.NavBarLinkEventHandler(this.navBarPowerSupplyOff_LinkClicked);
            // 
            // navBarTemperature
            // 
            this.navBarTemperature.Caption = "Temperature";
            this.navBarTemperature.LargeImage = ((System.Drawing.Image)(resources.GetObject("navBarTemperature.LargeImage")));
            this.navBarTemperature.Name = "navBarTemperature";
            // 
            // imageList2
            // 
            this.imageList2.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imageList2.ImageStream")));
            this.imageList2.TransparentColor = System.Drawing.Color.Magenta;
            this.imageList2.Images.SetKeyName(0, "");
            this.imageList2.Images.SetKeyName(1, "");
            this.imageList2.Images.SetKeyName(2, "");
            this.imageList2.Images.SetKeyName(3, "");
            this.imageList2.Images.SetKeyName(4, "");
            this.imageList2.Images.SetKeyName(5, "");
            this.imageList2.Images.SetKeyName(6, "");
            this.imageList2.Images.SetKeyName(7, "");
            this.imageList2.Images.SetKeyName(8, "");
            // 
            // imageList1
            // 
            this.imageList1.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imageList1.ImageStream")));
            this.imageList1.TransparentColor = System.Drawing.Color.Magenta;
            this.imageList1.Images.SetKeyName(0, "");
            this.imageList1.Images.SetKeyName(1, "");
            this.imageList1.Images.SetKeyName(2, "");
            this.imageList1.Images.SetKeyName(3, "");
            this.imageList1.Images.SetKeyName(4, "");
            this.imageList1.Images.SetKeyName(5, "");
            this.imageList1.Images.SetKeyName(6, "");
            this.imageList1.Images.SetKeyName(7, "");
            this.imageList1.Images.SetKeyName(8, "");
            // 
            // navBarTelnet
            // 
            this.navBarTelnet.Caption = "Telnet Cmd";
            this.navBarTelnet.LargeImage = ((System.Drawing.Image)(resources.GetObject("navBarTelnet.LargeImage")));
            this.navBarTelnet.Name = "navBarTelnet";
            this.navBarTelnet.LinkClicked += new DevExpress.XtraNavBar.NavBarLinkEventHandler(this.navBarTelnet_LinkClicked);
            // 
            // DummyToolbox
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.BackColor = System.Drawing.SystemColors.ControlDarkDark;
            this.ClientSize = new System.Drawing.Size(171, 357);
            this.Controls.Add(this.navBarControl1);
            this.HideOnClose = true;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "DummyToolbox";
            this.Padding = new System.Windows.Forms.Padding(0, 2, 0, 2);
            this.ShowHint = WeifenLuo.WinFormsUI.Docking.DockState.DockLeftAutoHide;
            this.TabText = "HIT Toolbox";
            this.Text = "HIT Toolbox";
            ((System.ComponentModel.ISupportInitialize)(this.navBarControl1)).EndInit();
            this.ResumeLayout(false);

		}
		#endregion

        private System.Windows.Forms.ImageList imageList;
        private DevExpress.XtraNavBar.NavBarControl navBarControl1;
        private DevExpress.XtraNavBar.NavBarGroup navBarGroup1;
        private DevExpress.XtraNavBar.NavBarItem navBarRunSet;
        private DevExpress.XtraNavBar.NavBarItem navBarSaveSet;
        private DevExpress.XtraNavBar.NavBarItem navBarOpenSet;
        private DevExpress.XtraNavBar.NavBarGroup navBarGroup2;
        private DevExpress.XtraNavBar.NavBarGroup navBarGroup3;
        private System.Windows.Forms.ImageList imageList1;
        private System.Windows.Forms.ImageList imageList2;
        private DevExpress.XtraNavBar.NavBarItem navBarNewSet;
        private DevExpress.XtraNavBar.NavBarItem navBarRunTest;
        private DevExpress.XtraNavBar.NavBarItem navBarSaveTest;
        private DevExpress.XtraNavBar.NavBarItem navBarOpenTest;
        private DevExpress.XtraNavBar.NavBarItem navBarNewTest;
        private DevExpress.XtraNavBar.NavBarItem navBarFreqStep;
        private DevExpress.XtraNavBar.NavBarItem navSigGenOn;
        private DevExpress.XtraNavBar.NavBarItem navBarSigGenOff;
        private DevExpress.XtraNavBar.NavBarItem navBarSigGenNoise;
        private DevExpress.XtraNavBar.NavBarItem navBarLMUConsoleCmd;
        private DevExpress.XtraNavBar.NavBarItem navBarSigGenPower;
        private DevExpress.XtraNavBar.NavBarItem navBarProcessStep;
        private DevExpress.XtraNavBar.NavBarItem navBarSNMPGet;
        private DevExpress.XtraNavBar.NavBarItem navBarSNMPSet;
        private DevExpress.XtraNavBar.NavBarItem navBarPowerSupplyOn;
        private DevExpress.XtraNavBar.NavBarItem navBarPowerSupplyOff;
        private DevExpress.XtraNavBar.NavBarItem navBarDisplayModalDialog;
        private DevExpress.XtraNavBar.NavBarItem navBarTimerWithDialog;
        private DevExpress.XtraNavBar.NavBarItem navBarDelayTest;
        private DevExpress.XtraNavBar.NavBarItem navBarRecorder;
        private DevExpress.XtraNavBar.NavBarItem navBarTDOA;
        private DevExpress.XtraNavBar.NavBarItem navBarTemperature;
        private DevExpress.XtraNavBar.NavBarItem navBarTelnet;
    }
}