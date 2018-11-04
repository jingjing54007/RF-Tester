using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using WeifenLuo.WinFormsUI.Docking;

using TruePosition.Test.IO;
using TruePosition.Test.UI;

namespace HITUI
{
    
    public partial class DummyPropertyWindow : ToolWindow
    {

        
        private ISerialConfig _globalSerial1 = null;
        private ISerialConfig _globalSerial2 = null;
        private ISerialConfig _globalSerial3 = null;
        private ISerialConfig _globalSerial4 = null;
        private IGpibConfig   _globalSigGen = null;
        private IGpibConfig   _globalSigGen2 = null;
        private IGpibConfig   _globalPower1 = null;
        private IGpibConfig   _globalPower2 = null;
        private IGpibConfig   _globalPower3 = null;
        private IGpibConfig   _globalPower4 = null;
        private IGpibConfig   _globalPowerMeter = null;
        private ITelnetConfig _globalTelnet;
        private TruePosition.Test.UI.IGlobalConfig _globalConfig = null;
        public TruePosition.Test.UI.IGlobalConfig GlobalConfig
        {
            get { return _globalConfig; }
            set { _globalConfig = value; }
        }
        
        public DummyPropertyWindow()
        {

            foreach (ConfigUIName eConfigName in Enum.GetValues(typeof(ConfigUIName)))
            {
                string strName = StringEnum.GetStringValue(eConfigName);
                if (!ConfigManager.Exists(strName))
                {
                    CreateConfig(eConfigName);
                }
                else
                {
                    LoadConfig(eConfigName);
              }
            }

            GlobalConfigData.DataConfig = new UIConfigProperties("", _globalConfig,
                _globalSerial1, _globalSerial2, _globalSerial3, _globalSerial4,
                _globalSigGen, _globalSigGen2, 
                _globalPower1, _globalPower2, _globalPower3, _globalPower4, _globalPowerMeter,
                _globalTelnet);

            InitializeComponent();

            propertyGrid.SelectedObject = GlobalConfigData.DataConfig;
        }

        private void CreateConfig(ConfigUIName eName)
        {
            switch (eName)
            {
                case ConfigUIName.Global:
                    ConfigManager.Create (out _globalConfig);
                    break;
                case ConfigUIName.Serial1:
                    ConfigManager.Create(StringEnum.GetStringValue(eName), out _globalSerial1);
                    break;               
                case ConfigUIName.Serial2:
                    ConfigManager.Create(StringEnum.GetStringValue(eName), out _globalSerial2);
                    break;
                case ConfigUIName.Serial3:
                    ConfigManager.Create(StringEnum.GetStringValue(eName), out _globalSerial3);
                    break;
                case ConfigUIName.Serial4:
                    ConfigManager.Create(StringEnum.GetStringValue(eName), out _globalSerial4);
                    break;
                case ConfigUIName.SigGen:
                    ConfigManager.Create(StringEnum.GetStringValue(eName), out _globalSigGen);
                    break;
                case ConfigUIName.SigGen2:
                    ConfigManager.Create(StringEnum.GetStringValue(eName), out _globalSigGen2);
                    break;
                case ConfigUIName.Power1:
                    ConfigManager.Create(StringEnum.GetStringValue(eName), out _globalPower1);
                    break;
                case ConfigUIName.Power2:
                    ConfigManager.Create(StringEnum.GetStringValue(eName), out _globalPower2);
                    break;
                case ConfigUIName.Power3:
                    ConfigManager.Create(StringEnum.GetStringValue(eName), out _globalPower3);
                    break;
                case ConfigUIName.Power4:
                    ConfigManager.Create(StringEnum.GetStringValue(eName), out _globalPower4);
                    break;
                case ConfigUIName.PowerMeter:
                    ConfigManager.Create(StringEnum.GetStringValue(eName), out _globalPowerMeter);
                    break;
                case ConfigUIName.Telnet:
                    ConfigManager.Create(StringEnum.GetStringValue(eName), out _globalTelnet);
                    break;
                default: 
                    break;
            }
        }

        private void LoadConfig(ConfigUIName eName)
        {
            switch (eName)
            {
                case ConfigUIName.Global:
                    ConfigManager.Load(out _globalConfig);
                    break;
                case ConfigUIName.Serial1:
                    ConfigManager.Load(StringEnum.GetStringValue(eName), out _globalSerial1);
                    break;
                case ConfigUIName.Serial2:
                    ConfigManager.Load(StringEnum.GetStringValue(eName), out _globalSerial2);
                    break;
                case ConfigUIName.Serial3:
                    ConfigManager.Load(StringEnum.GetStringValue(eName), out _globalSerial3);
                    break;
                case ConfigUIName.Serial4:
                    ConfigManager.Load(StringEnum.GetStringValue(eName), out _globalSerial4);
                    break;
                case ConfigUIName.SigGen:
                    ConfigManager.Load(StringEnum.GetStringValue(eName), out _globalSigGen);
                    break;
                case ConfigUIName.SigGen2:
                    ConfigManager.Load(StringEnum.GetStringValue(eName), out _globalSigGen2);
                    break;
                case ConfigUIName.Power1:
                    ConfigManager.Load(StringEnum.GetStringValue(eName), out _globalPower1);
                    break;
                case ConfigUIName.Power2:
                    ConfigManager.Load(StringEnum.GetStringValue(eName), out _globalPower2);
                    break;
                case ConfigUIName.Power3:
                    ConfigManager.Load(StringEnum.GetStringValue(eName), out _globalPower3);
                    break;
                case ConfigUIName.Power4:
                    ConfigManager.Load(StringEnum.GetStringValue(eName), out _globalPower4);
                    break;
                case ConfigUIName.PowerMeter:
                    ConfigManager.Load(StringEnum.GetStringValue(eName), out _globalPowerMeter);
                    break;                
                case ConfigUIName.Telnet:
                    ConfigManager.Load(StringEnum.GetStringValue(eName), out _globalTelnet);
                    break;
                default: 
                    break;
            }
        }

        public void OnSave(object sender, EventArgs args)
        {
            Save();
        }

        public void Save()
        {
            foreach (ConfigUIName eConfigName in Enum.GetValues(typeof(ConfigUIName)))
            {
                string strName = StringEnum.GetStringValue(eConfigName);
                switch (eConfigName)
                {
                    case ConfigUIName.Global:
                        ConfigManager.Save(_globalConfig);
                        break;
                    case ConfigUIName.Serial1:
                        ConfigManager.Save(StringEnum.GetStringValue(eConfigName), _globalSerial1);
                        break;
                    case ConfigUIName.Serial2:
                        ConfigManager.Save(StringEnum.GetStringValue(eConfigName), _globalSerial2);
                        break;
                    case ConfigUIName.Serial3:
                        ConfigManager.Save(StringEnum.GetStringValue(eConfigName), _globalSerial3);
                        break;
                    case ConfigUIName.Serial4:
                        ConfigManager.Save(StringEnum.GetStringValue(eConfigName), _globalSerial4);
                        break;
                    case ConfigUIName.SigGen:
                        ConfigManager.Save(StringEnum.GetStringValue(eConfigName), _globalSigGen);                        
                        break;
                    case ConfigUIName.SigGen2:
                        ConfigManager.Save(StringEnum.GetStringValue(eConfigName), _globalSigGen2);
                        break;
                    case ConfigUIName.Power1:
                        ConfigManager.Save(StringEnum.GetStringValue(eConfigName), _globalPower1);
                        break;
                    case ConfigUIName.Power2:
                        ConfigManager.Save(StringEnum.GetStringValue(eConfigName), _globalPower2);
                        break;
                    case ConfigUIName.Power3:
                        ConfigManager.Save(StringEnum.GetStringValue(eConfigName), _globalPower3);
                        break;
                    case ConfigUIName.Power4:
                        ConfigManager.Save(StringEnum.GetStringValue(eConfigName), _globalPower4);
                        break;
                    case ConfigUIName.PowerMeter:
                        ConfigManager.Save(StringEnum.GetStringValue(eConfigName), _globalPowerMeter);
                        break;
                    case ConfigUIName.Telnet:
                        ConfigManager.Save(StringEnum.GetStringValue(eConfigName), _globalTelnet);
                        break;
                    default:
                        break;
                }
            }
            //ConfigManager.Save(serialConfig);
        }
    }

    public enum ConfigUIName
    {
        [StringValue("GlobalSettings")]
        Global,
        [StringValue("LMU1")]
        Serial1,
        [StringValue("LMU2")]
        Serial2,
        [StringValue("LMU3")]
        Serial3,
        [StringValue("LMU4")]
        Serial4,
        [StringValue("SigGen")]
        SigGen,
        [StringValue("SigGen2")]
        SigGen2,
        [StringValue("Power1")]
        Power1,
        [StringValue("Power2")]
        Power2,
        [StringValue("Power3")]
        Power3,
        [StringValue("Power4")]
        Power4,
        [StringValue("PowerMeter")]
        PowerMeter,
        [StringValue("SMLC1")]
        Telnet,
    }

    static class GlobalConfigData
    {
        private static UIConfigProperties _dataConfig = null;
        public static UIConfigProperties DataConfig
        {
            get { return _dataConfig; }
            set { _dataConfig = value; }
        }

    }
}
