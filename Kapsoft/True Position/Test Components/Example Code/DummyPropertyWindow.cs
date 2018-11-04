using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using WeifenLuo.WinFormsUI.Docking;

namespace DockSample
{
    public partial class DummyPropertyWindow : ToolWindow
    {
        private ConfigData _dataConfig = new ConfigData();
        public DummyPropertyWindow()
        {
            InitializeComponent();
			propertyGrid.SelectedObject = propertyGrid;
            propertyGrid.SelectedObject = _dataConfig;
        }
    }
}
