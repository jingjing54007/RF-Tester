using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;

using TruePosition.Test.IO;
using TruePosition.Test.UI;
using System.Xml.Serialization;

namespace DockSample
{
// The following I expect to get from Port Interfaces
    //COM port Assignments (LMU 1-4, port, baud rate, stop bits)	Text, Integers	Edits	None	Yes
    //SigGen #1 IEEE address	Integer	Edit	"none"	Yes
    //SigGen #2 IEEE address	Integer	Edit	"none"	Yes
    //Power Meter IEEE address	Integer	Edit	"none"	Yes

    // DESIGN NOTE:
    // General idea of patterns. 
    // 1. Passing serialConfig avoids UI details 'leaking' into framework. (i.e. attributes)
    // 2. Simplifies using default values from SerialPort driver.
    [XmlRoot(ElementName="SerialPort")]
    public class SerialConfigProperties : ISerialConfig 
    {
        private string Name { get; set; }
        private ISerialConfig SerialConfig { get; set; }
        public SerialConfigProperties() { }
        public SerialConfigProperties(string name, ISerialConfig serialConfig)
        {
            Name = name;
            SerialConfig = serialConfig;
        }

        #region ISerialConfig Members
        [CategoryAttribute("LMU"), DescriptionAttribute("Rx Buffer Size")]
        public int ReceiveBufferSize
        {
            get { return SerialConfig.ReceiveBufferSize; }
            set { SerialConfig.ReceiveBufferSize = value; }
        }
        [CategoryAttribute("LMU"), DescriptionAttribute("Tx Buffer Size")]
        public int TransmitBufferSize
        {
            get { return SerialConfig.TransmitBufferSize; }
            set { SerialConfig.TransmitBufferSize = value; }
        }
        [CategoryAttribute("LMU"), DescriptionAttribute("Port HandShaking")]
        public System.IO.Ports.Handshake Handshake
        {
            get { return SerialConfig.Handshake; }
            set { SerialConfig.Handshake = value; }
        }
        public System.IO.Ports.Parity Parity
        {
            get { return SerialConfig.Parity; }
            set { SerialConfig.Parity = value; }
        }
        public System.IO.Ports.StopBits StopBits
        {
            get { return SerialConfig.StopBits; }
            set { SerialConfig.StopBits = value; }
        }
        public string PortName
        {
            get { return SerialConfig.PortName; }
            set { SerialConfig.PortName = value; }
        }
        public int BaudRate
        {
            get { return SerialConfig.BaudRate; }
            set { SerialConfig.BaudRate = value; }
        }
        public int DataBits
        {
            get { return SerialConfig.DataBits; }
            set { SerialConfig.DataBits = value; }
        }
        public bool DiscardNull
        {
            get { return SerialConfig.DiscardNull; }
            set { SerialConfig.DiscardNull = value; }
        }
        [XmlAttribute]
        public string InputHeader
        {
            get { return SerialConfig.InputHeader; }
            set { SerialConfig.InputHeader = value; }
        }
        [XmlAttribute]
        public string OutputHeader
        {
            get { return SerialConfig.OutputHeader; }
            set { SerialConfig.OutputHeader = value; }
        }
        [XmlAttribute]
        public string InputTrailer
        {
            get { return SerialConfig.InputTrailer; }
            set { SerialConfig.InputTrailer = value; }
        }
        public bool InputEveryCharacter
        {
            get { return SerialConfig.InputEveryCharacter; }
            set { SerialConfig.InputEveryCharacter = value; }
        }
        [XmlAttribute]
        public string OutputTrailer
        {
            get { return SerialConfig.OutputTrailer; }
            set { SerialConfig.OutputTrailer = value; }
        }
        public PortMode Mode
        {
            get { return SerialConfig.Mode; }
            set { SerialConfig.Mode = value; }
        }
        public int ReceiveTimeout
        {
            get { return SerialConfig.ReceiveTimeout; }
            set { SerialConfig.ReceiveTimeout = value; }
        }
        public int TransmitRetries
        {
            get { return SerialConfig.TransmitRetries; }
            set { SerialConfig.TransmitRetries = value; }
        }
        #endregion
    }

    public interface IGlobalConfig 
    {
        string CalFilePathName { get; set; }
        int StepTimeoutSecs { get; set; }
        bool StepContinueOnError { get; set; }
        int StepNumberRetries { get; set; }
        string LogFileFolder { get; set; }
        string MapExePathName { get; set; }
        string MapConfigName { get; set; }
        string MapFolderPath { get; set; }
        string SMLC1 { get; set; }
        int SMLC1Port { get; set; }
        string SMLC2 { get; set; }
        int SMLC2Port { get; set; }
        string SMLCVirtual { get; set; }
        int SMLCVirtualPort { get; set; }
        string LMU1_ID { get; set; }
        string LMU2_ID { get; set; }
        string LMU3_ID { get; set; }
        string LMU4_ID { get; set; }
    }
    public class GlobalConfigProperties : IGlobalConfig
    {
        private string Name { get; set; }
        private IGlobalConfig GlobalConfig { get; set; }
        public GlobalConfigProperties(string name, IGlobalConfig globalConfig)
        {
            Name = name;
            GlobalConfig = globalConfig;
        }

        // HIT Misc Group
        [CategoryAttribute("HIT Misc"), DescriptionAttribute("Active Cal File path and filename"),
        Editor(typeof(System.Windows.Forms.Design.FileNameEditor), typeof(System.Drawing.Design.UITypeEditor))]
        public string CalFilePathName 
        {
            get { return GlobalConfig.CalFilePathName; }
            set { GlobalConfig.CalFilePathName = value; }
        }
        [CategoryAttribute("HIT Misc"), DescriptionAttribute("Default value Step timeout (secs)")]
        public int StepTimeoutSecs { get; set; }
        [CategoryAttribute("HIT Misc"), DescriptionAttribute("Default values Step continue on error")]
        public bool StepContinueOnError { get; set; }
        [CategoryAttribute("HIT Misc"), DescriptionAttribute("Default value Step number of retries")]
        public int StepNumberRetries { get; set; }
        [CategoryAttribute("HIT Misc"), DescriptionAttribute("Log File destination directory"),
        Editor(typeof(System.Windows.Forms.Design.FolderNameEditor), typeof(System.Drawing.Design.UITypeEditor))]
        public string LogFileFolder { get; set; }

        // MAP group
        [CategoryAttribute("MAP Settings"), DescriptionAttribute("MAP Executable full path and filename"),
        Editor(typeof(System.Windows.Forms.Design.FileNameEditor), typeof(System.Drawing.Design.UITypeEditor))]
        public string MapExePathName { get; set; }
        [CategoryAttribute("MAP Settings"), DescriptionAttribute("MAP Configuration Name")]
        public string MapConfigName { get; set; }
        [CategoryAttribute("MAP Settings"), DescriptionAttribute("MAP file output destination directory"),
        Editor(typeof(System.Windows.Forms.Design.FolderNameEditor), typeof(System.Drawing.Design.UITypeEditor))]
        public string MapFolderPath { get; set; }

        // SNMP Group
        [CategoryAttribute("SNMP Settings"), DescriptionAttribute("SMLC1 IP Address")]
        public string SMLC1 { get; set; }
        [CategoryAttribute("SNMP Settings"), DescriptionAttribute("SMLC1 Port number")]
        public int SMLC1Port { get; set; }
        [CategoryAttribute("SNMP Settings"), DescriptionAttribute("SMLC2 IP Address")]
        public string SMLC2 { get; set; }
        [CategoryAttribute("SNMP Settings"), DescriptionAttribute("SMLC2 Port number")]
        public int SMLC2Port { get; set; }
        [CategoryAttribute("SNMP Settings"), DescriptionAttribute("SMLC IP Address (Virtual or Active SMLC")]
        public string SMLCVirtual { get; set; }
        [CategoryAttribute("SNMP Settings"), DescriptionAttribute("SMLC Port number (Virtual or Active SMLC")]
        public int SMLCVirtualPort { get; set; }
        [CategoryAttribute("SNMP Settings"), DescriptionAttribute("LMU1 ID (Community string)")]
        public string LMU1_ID { get; set; }
        [CategoryAttribute("SNMP Settings"), DescriptionAttribute("LMU2 ID (Community string)")]
        public string LMU2_ID { get; set; }
        [CategoryAttribute("SNMP Settings"), DescriptionAttribute("LMU3 ID (Community string)")]
        public string LMU3_ID { get; set; }
        [CategoryAttribute("SNMP Settings"), DescriptionAttribute("LMU4 ID (Community string)")]
        public string LMU4_ID { get; set; }
    }
}