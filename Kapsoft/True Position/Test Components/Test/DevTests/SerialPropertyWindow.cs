using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
//using WeifenLuo.WinFormsUI.Docking;

using TruePosition.Test.IO;
using TruePosition.Test.UI;

namespace DockSample
{
    // DESIGN NOTE:
    // Keep a separate property grid for each different flavor of config
    public partial class SerialPropertyWindow //: ToolWindow
    {
        string Name { get; set; }
        ISerialConfig serialConfig = null;
        private SerialConfigProperties _dataConfig = null;
        public SerialPropertyWindow(string name)
        {
            Name = name;

            if (!ConfigManager.Exists(Name))
                ConfigManager.Create(Name, out serialConfig);
            else
                ConfigManager.Load(Name, out serialConfig);

            _dataConfig = new SerialConfigProperties(Name, serialConfig);

            //InitializeComponent();
            //propertyGrid.SelectedObject = _dataConfig;
        }

        public void Save()
        {
            ConfigManager.Save(Name, serialConfig);
        }
    }
}
