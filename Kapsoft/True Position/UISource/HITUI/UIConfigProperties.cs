using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;

using TruePosition.Test.IO;
using TruePosition.Test.UI;
using System.Xml.Serialization;

namespace HITUI
{

    [XmlRoot(ElementName="SerialPort")]
    public class UIConfigProperties
    {
        private string Name { get; set; }
        private ISerialConfig SerialConfig1 { get; set; }
        private ISerialConfig SerialConfig2 { get; set; }
        private ISerialConfig SerialConfig3 { get; set; }
        private ISerialConfig SerialConfig4 { get; set; }
        private IGlobalConfig GlobalConfig { get; set; }
        private IGpibConfig SigGenConfig1 { get; set; }
        private IGpibConfig SigGenConfig2 { get; set; }
        private IGpibConfig PowerConfig1 { get; set; }
        private IGpibConfig PowerConfig2 { get; set; }
        private IGpibConfig PowerConfig3 { get; set; }
        private IGpibConfig PowerConfig4 { get; set; }
        private IGpibConfig PowerMeter { get; set; }
        private ITelnetConfig TelnetConfig { get; set; }
    

        public UIConfigProperties() { }
        public UIConfigProperties(string name, IGlobalConfig globalConfig,
            ISerialConfig serialConfig1, ISerialConfig serialConfig2, ISerialConfig serialConfig3, ISerialConfig serialConfig4,
            IGpibConfig sigGenConfig1, IGpibConfig sigGenConfig2,
            IGpibConfig powerConfig1, IGpibConfig powerConfig2, IGpibConfig powerConfig3, IGpibConfig powerConfig4, IGpibConfig powerMeter,
            ITelnetConfig telentConfig)
        {
            Name = name;
            SerialConfig1 = serialConfig1;
            SerialConfig2 = serialConfig2;
            SerialConfig3 = serialConfig3;
            SerialConfig4 = serialConfig4;
            GlobalConfig = globalConfig;
            SigGenConfig1 = sigGenConfig1;
            SigGenConfig2 = sigGenConfig2;
            PowerConfig1 = powerConfig1;
            PowerConfig2 = powerConfig2;
            PowerConfig3 = powerConfig3;
            PowerConfig4 = powerConfig4;
            PowerMeter = powerMeter;
            TelnetConfig = telentConfig;
        }

        #region Telnet SMLC
        [CategoryAttribute("Telnet SMLC"), DescriptionAttribute("HostName (IP Address)")]
        public string TelnetHostname
        {
            get { return TelnetConfig.HostName; }
            set { TelnetConfig.HostName = value; }
        }

        [CategoryAttribute("Telnet SMLC"), DescriptionAttribute("Port number")]
        public int TelnetPortNumber
        {
            get { return TelnetConfig.Port; }
            set { TelnetConfig.Port = value; }
        }

        [CategoryAttribute("Telnet SMLC"), DescriptionAttribute("Username")]
        public string TelnetUsername
        {
            get { return TelnetConfig.Username; }
            set { TelnetConfig.Username = value; }
        }
        [CategoryAttribute("Telnet SMLC"), DescriptionAttribute("Username")]
        public string TelnetPassword
        {
            get { return TelnetConfig.Password; }
            set { TelnetConfig.Password = value; }
        }
        [CategoryAttribute("Telnet SMLC"), DescriptionAttribute("Input trailer")]
        public string TelnetInputTrailer
        {
            get { return TelnetConfig.InputTrailer; }
            set { TelnetConfig.InputTrailer = value; }
        }

        [CategoryAttribute("Telnet SMLC"), DescriptionAttribute("Output trailer")]
        public string TelnetOutputTrailer
        {
            get { return TelnetConfig.OutputTrailer; }
            set { TelnetConfig.OutputTrailer = value; }
        }

        [CategoryAttribute("Telnet SMLC"), DescriptionAttribute("Receive timeout")]
        public int TelnetReceiveTimeout
        {
            get { return TelnetConfig.ReceiveTimeout; }
            set { TelnetConfig.ReceiveTimeout = value; }
        }

        [CategoryAttribute("Telnet SMLC"), DescriptionAttribute("Transmit retries")]
        public int TelnetTransmitRetries
        {
            get { return TelnetConfig.TransmitRetries; }
            set { TelnetConfig.TransmitRetries = value; }
        }

        #endregion

        #region Power supplys
        [CategoryAttribute("Power Supply GBIP"), DescriptionAttribute("Power Supply 1 Board ID Number")]
        public int PowerBoardID1
        {
            get { return PowerConfig1.BoardId; }
            set { PowerConfig1.BoardId = value; }
        }

        [CategoryAttribute("Power Supply GBIP"), DescriptionAttribute("Power Supply 1 Primary Address")]
        public byte PowerPrimaryAddress1
        {
            get { return PowerConfig1.PrimaryAddress; }
            set { PowerConfig1.PrimaryAddress = value; }
        }

        [CategoryAttribute("Power Supply GBIP"), DescriptionAttribute("Power Supply 2 Board ID Number")]
        public int PowerBoardID2
        {
            get { return PowerConfig2.BoardId; }
            set { PowerConfig2.BoardId = value; }
        }

        [CategoryAttribute("Power Supply GBIP"), DescriptionAttribute("Power Supply 2 Primary Address")]
        public byte PowerPrimaryAddress2
        {
            get { return PowerConfig2.PrimaryAddress; }
            set { PowerConfig2.PrimaryAddress = value; }
        }
        [CategoryAttribute("Power Supply GBIP"), DescriptionAttribute("Power Supply 3 Board ID Number")]
        public int PowerBoardID3
        {
            get { return PowerConfig3.BoardId; }
            set { PowerConfig3.BoardId = value; }
        }

        [CategoryAttribute("Power Supply GBIP"), DescriptionAttribute("Power Supply 3 Primary Address")]
        public byte PowerPrimaryAddress3
        {
            get { return PowerConfig3.PrimaryAddress; }
            set { PowerConfig3.PrimaryAddress = value; }
        }
        [CategoryAttribute("Power Supply GBIP"), DescriptionAttribute("Power Supply 3 Board ID Number")]
        public int PowerBoardID4
        {
            get { return PowerConfig4.BoardId; }
            set { PowerConfig4.BoardId = value; }
        }

        [CategoryAttribute("Power Supply GBIP"), DescriptionAttribute("Power Supply 4 Primary Address")]
        public byte PowerPrimaryAddress4
        {
            get { return PowerConfig4.PrimaryAddress; }
            set { PowerConfig4.PrimaryAddress = value; }
        }

        #endregion

        #region GPIB

        [CategoryAttribute("GBIP Instruments"), DescriptionAttribute("Power Meter Board ID Number")]
        public int PowerMeterBoardID
        {
            get { return PowerMeter.BoardId; }
            set { PowerMeter.BoardId = value; }
        }

        [CategoryAttribute("GBIP Instruments"), DescriptionAttribute("Power Meter Primary Address")]
        public byte PowerMeterPrimaryAddress
        {
            get { return PowerMeter.PrimaryAddress; }
            set { PowerMeter.PrimaryAddress = value; }
        }
        [CategoryAttribute("GBIP Instruments"), DescriptionAttribute("SigGen1 GPIB Board ID Number")]
        public int SigGen1BoardID
        {
            get { return SigGenConfig1.BoardId; }
            set { SigGenConfig1.BoardId = value; }
        }

        [CategoryAttribute("GBIP Instruments"), DescriptionAttribute("SigGen1 GPIB Primary Address")]
        public byte SigGen1PrimaryAddress
        {
            get { return SigGenConfig1.PrimaryAddress; }
            set { SigGenConfig1.PrimaryAddress = value; }
        }

        [CategoryAttribute("GBIP Instruments"), DescriptionAttribute("SigGen1 GPIB Secondary Address")]
        public byte SigGen1SecondaryAddress
        {
            get { return SigGenConfig1.SecondaryAddress; }
            set { SigGenConfig1.SecondaryAddress  = value; }
        }

        [CategoryAttribute("GBIP Instruments"), DescriptionAttribute("SigGen1 GPIB Receive Timeout")]
        public int SigGen1ReceiveTimeout
        {
            get { return SigGenConfig1.ReceiveTimeout; }
            set { SigGenConfig1.ReceiveTimeout= value; }
        }

        [CategoryAttribute("GBIP Instruments"), DescriptionAttribute("SigGen2 GPIB Board ID Number")]
        public int SigGen2BoardID
        {
            get { return SigGenConfig2.BoardId; }
            set { SigGenConfig2.BoardId = value; }
        }


        [CategoryAttribute("GBIP Instruments"), DescriptionAttribute("SigGen2 GPIB Primary Address")]
        public byte SigGen2PrimaryAddress
        {
            get { return SigGenConfig2.PrimaryAddress; }
            set { SigGenConfig2.PrimaryAddress = value; }
        }

        [CategoryAttribute("GBIP Instruments"), DescriptionAttribute("SigGen2 GPIB Secondary Address")]
        public byte SigGen2SecondaryAddress
        {
            get { return SigGenConfig2.SecondaryAddress; }
            set { SigGenConfig2.SecondaryAddress = value; }
        }

        [CategoryAttribute("GBIP Instruments"), DescriptionAttribute("SigGen2 GPIB Receive Timeout")]
        public int SigGen2ReceiveTimeout
        {
            get { return SigGenConfig2.ReceiveTimeout; }
            set { SigGenConfig2.ReceiveTimeout = value; }
        }

        #endregion

        #region LMU1
        [CategoryAttribute("LMU1"), DescriptionAttribute("Rx Buffer Size")]
        public int ReceiveBufferSize
        {
            get { return SerialConfig1.ReceiveBufferSize; }
            set { SerialConfig1.ReceiveBufferSize = value; }
        }
        [CategoryAttribute("LMU1"), DescriptionAttribute("Tx Buffer Size")]
        public int TransmitBufferSize
        {
            get { return SerialConfig1.TransmitBufferSize; }
            set { SerialConfig1.TransmitBufferSize = value; }
        }
        [CategoryAttribute("LMU1"), DescriptionAttribute("Port HandShaking")]
        public System.IO.Ports.Handshake Handshake
        {
            get { return SerialConfig1.Handshake; }
            set { SerialConfig1.Handshake = value; }
        }
        [CategoryAttribute("LMU1"), DescriptionAttribute("Parity")]
        public System.IO.Ports.Parity Parity
        {
            get { return SerialConfig1.Parity; }
            set { SerialConfig1.Parity = value; }
        }
        [CategoryAttribute("LMU1"), DescriptionAttribute("Number of Stop bits")]
        public System.IO.Ports.StopBits StopBits
        {
            get { return SerialConfig1.StopBits; }
            set { SerialConfig1.StopBits = value; }
        }
        [CategoryAttribute("LMU1"), DescriptionAttribute("Port Name")]
        public string PortName
        {
            get { return SerialConfig1.PortName; }
            set { SerialConfig1.PortName = value; }
        }
        [CategoryAttribute("LMU1"), DescriptionAttribute("Baud Rate")]
        public int BaudRate
        {
            get { return SerialConfig1.BaudRate; }
            set { SerialConfig1.BaudRate = value; }
        }
        [CategoryAttribute("LMU1"), DescriptionAttribute("Number Data bits")]
        public int DataBits
        {
            get { return SerialConfig1.DataBits; }
            set { SerialConfig1.DataBits = value; }
        }
        [CategoryAttribute("LMU1"), DescriptionAttribute("Discard Nulls")]
        public bool DiscardNull
        {
            get { return SerialConfig1.DiscardNull; }
            set { SerialConfig1.DiscardNull = value; }
        }
        [CategoryAttribute("LMU1"), DescriptionAttribute("Input Header")]
        [XmlAttribute]
        public string InputHeader
        {
            get { return SerialConfig1.InputHeader; }
            set { SerialConfig1.InputHeader = value; }
        }
        [XmlAttribute]
        [CategoryAttribute("LMU1"), DescriptionAttribute("Output Header")]
        public string OutputHeader
        {
            get { return SerialConfig1.OutputHeader; }
            set { SerialConfig1.OutputHeader = value; }
        }
        [XmlAttribute]
        [CategoryAttribute("LMU1"), DescriptionAttribute("Input Trailer")]
        public string InputTrailer
        {
            get { return SerialConfig1.InputTrailer; }
            set { SerialConfig1.InputTrailer = value; }
        }
        [CategoryAttribute("LMU1"), DescriptionAttribute("Input Every Character")]
        public bool InputEveryCharacter
        {
            get { return SerialConfig1.InputEveryCharacter; }
            set { SerialConfig1.InputEveryCharacter = value; }
        }
        [XmlAttribute]
        [CategoryAttribute("LMU1"), DescriptionAttribute("Output Trailer")]
        public string OutputTrailer
        {
            get { return SerialConfig1.OutputTrailer; }
            set { SerialConfig1.OutputTrailer = value; }
        }
        [CategoryAttribute("LMU1"), DescriptionAttribute("Port Mode")]
        public SerialPortMode Mode
        {
            get { return SerialConfig1.Mode; }
            set { SerialConfig1.Mode = value; }
        }
        [CategoryAttribute("LMU1"), DescriptionAttribute("Receive Timeout (secs)")]
        public int ReceiveTimeout
        {
            get { return SerialConfig1.ReceiveTimeout; }
            set { SerialConfig1.ReceiveTimeout = value; }
        }
        [CategoryAttribute("LMU1"), DescriptionAttribute("Transmit retries")]
        public int TransmitRetries
        {
            get { return SerialConfig1.TransmitRetries; }
            set { SerialConfig1.TransmitRetries = value; }
        }
        #endregion

        #region LMU2
        [CategoryAttribute("LMU2"), DescriptionAttribute("Rx Buffer Size")]
        public int ReceiveBufferSize2
        {
            get { return SerialConfig2.ReceiveBufferSize; }
            set { SerialConfig2.ReceiveBufferSize = value; }
        }
        [CategoryAttribute("LMU2"), DescriptionAttribute("Tx Buffer Size")]
        public int TransmitBufferSize2
        {
            get { return SerialConfig2.TransmitBufferSize; }
            set { SerialConfig2.TransmitBufferSize = value; }
        }
        [CategoryAttribute("LMU2"), DescriptionAttribute("Port HandShaking")]
        public System.IO.Ports.Handshake Handshake2
        {
            get { return SerialConfig2.Handshake; }
            set { SerialConfig2.Handshake = value; }
        }
        [CategoryAttribute("LMU2"), DescriptionAttribute("Parity")]
        public System.IO.Ports.Parity Parity2
        {
            get { return SerialConfig2.Parity; }
            set { SerialConfig2.Parity = value; }
        }
        [CategoryAttribute("LMU2"), DescriptionAttribute("Number of Stop bits")]
        public System.IO.Ports.StopBits StopBits2
        {
            get { return SerialConfig2.StopBits; }
            set { SerialConfig2.StopBits = value; }
        }
        [CategoryAttribute("LMU2"), DescriptionAttribute("Port Name")]
        public string PortName2
        {
            get { return SerialConfig2.PortName; }
            set { SerialConfig2.PortName = value; }
        }
        [CategoryAttribute("LMU2"), DescriptionAttribute("Baud Rate")]
        public int BaudRate2
        {
            get { return SerialConfig2.BaudRate; }
            set { SerialConfig2.BaudRate = value; }
        }
        [CategoryAttribute("LMU2"), DescriptionAttribute("Number Data bits")]
        public int DataBits2
        {
            get { return SerialConfig2.DataBits; }
            set { SerialConfig2.DataBits = value; }
        }
        [CategoryAttribute("LMU2"), DescriptionAttribute("Discard Nulls")]
        public bool DiscardNull2
        {
            get { return SerialConfig2.DiscardNull; }
            set { SerialConfig2.DiscardNull = value; }
        }
        [CategoryAttribute("LMU2"), DescriptionAttribute("Input Header")]
        [XmlAttribute]
        public string InputHeader2
        {
            get { return SerialConfig2.InputHeader; }
            set { SerialConfig2.InputHeader = value; }
        }
        [XmlAttribute]
        [CategoryAttribute("LMU2"), DescriptionAttribute("Output Header")]
        public string OutputHeader2
        {
            get { return SerialConfig2.OutputHeader; }
            set { SerialConfig2.OutputHeader = value; }
        }
        [XmlAttribute]
        [CategoryAttribute("LMU2"), DescriptionAttribute("Input Trailer")]
        public string InputTrailer2
        {
            get { return SerialConfig2.InputTrailer; }
            set { SerialConfig2.InputTrailer = value; }
        }
        [CategoryAttribute("LMU2"), DescriptionAttribute("Input Every Character")]
        public bool InputEveryCharacter2
        {
            get { return SerialConfig2.InputEveryCharacter; }
            set { SerialConfig2.InputEveryCharacter = value; }
        }
        [XmlAttribute]
        [CategoryAttribute("LMU2"), DescriptionAttribute("Output Trailer")]
        public string OutputTrailer2
        {
            get { return SerialConfig2.OutputTrailer; }
            set { SerialConfig2.OutputTrailer = value; }
        }
        [CategoryAttribute("LMU2"), DescriptionAttribute("Port Mode")]
        public SerialPortMode Mode2
        {
            get { return SerialConfig2.Mode; }
            set { SerialConfig2.Mode = value; }
        }
        [CategoryAttribute("LMU2"), DescriptionAttribute("Receive Timeout (secs)")]
        public int ReceiveTimeout2
        {
            get { return SerialConfig2.ReceiveTimeout; }
            set { SerialConfig2.ReceiveTimeout = value; }
        }
        [CategoryAttribute("LMU2"), DescriptionAttribute("Transmit retries")]
        public int TransmitRetries2
        {
            get { return SerialConfig2.TransmitRetries; }
            set { SerialConfig2.TransmitRetries = value; }
        }
        #endregion

        #region LMU3

        [CategoryAttribute("LMU3"), DescriptionAttribute("Rx Buffer Size")]
        public int ReceiveBufferSize3
        {
            get { return SerialConfig3.ReceiveBufferSize; }
            set { SerialConfig3.ReceiveBufferSize = value; }
        }
        [CategoryAttribute("LMU3"), DescriptionAttribute("Tx Buffer Size")]
        public int TransmitBufferSize3
        {
            get { return SerialConfig3.TransmitBufferSize; }
            set { SerialConfig3.TransmitBufferSize = value; }
        }
        [CategoryAttribute("LMU3"), DescriptionAttribute("Port HandShaking")]
        public System.IO.Ports.Handshake Handshake3
        {
            get { return SerialConfig3.Handshake; }
            set { SerialConfig3.Handshake = value; }
        }
        [CategoryAttribute("LMU3"), DescriptionAttribute("Parity")]
        public System.IO.Ports.Parity Parity3
        {
            get { return SerialConfig3.Parity; }
            set { SerialConfig3.Parity = value; }
        }
        [CategoryAttribute("LMU3"), DescriptionAttribute("Number of Stop bits")]
        public System.IO.Ports.StopBits StopBits3
        {
            get { return SerialConfig3.StopBits; }
            set { SerialConfig3.StopBits = value; }
        }
        [CategoryAttribute("LMU3"), DescriptionAttribute("Port Name")]
        public string PortName3
        {
            get { return SerialConfig3.PortName; }
            set { SerialConfig3.PortName = value; }
        }
        [CategoryAttribute("LMU3"), DescriptionAttribute("Baud Rate")]
        public int BaudRate3
        {
            get { return SerialConfig3.BaudRate; }
            set { SerialConfig3.BaudRate = value; }
        }
        [CategoryAttribute("LMU3"), DescriptionAttribute("Number Data bits")]
        public int DataBits3
        {
            get { return SerialConfig3.DataBits; }
            set { SerialConfig3.DataBits = value; }
        }
        [CategoryAttribute("LMU3"), DescriptionAttribute("Discard Nulls")]
        public bool DiscardNull3
        {
            get { return SerialConfig3.DiscardNull; }
            set { SerialConfig3.DiscardNull = value; }
        }
        [CategoryAttribute("LMU3"), DescriptionAttribute("Input Header")]
        [XmlAttribute]
        public string InputHeader3
        {
            get { return SerialConfig3.InputHeader; }
            set { SerialConfig3.InputHeader = value; }
        }
        [XmlAttribute]
        [CategoryAttribute("LMU3"), DescriptionAttribute("Output Header")]
        public string OutputHeader3
        {
            get { return SerialConfig3.OutputHeader; }
            set { SerialConfig3.OutputHeader = value; }
        }
        [XmlAttribute]
        [CategoryAttribute("LMU3"), DescriptionAttribute("Input Trailer")]
        public string InputTrailer3
        {
            get { return SerialConfig3.InputTrailer; }
            set { SerialConfig3.InputTrailer = value; }
        }
        [CategoryAttribute("LMU3"), DescriptionAttribute("Input Every Character")]
        public bool InputEveryCharacter3
        {
            get { return SerialConfig3.InputEveryCharacter; }
            set { SerialConfig3.InputEveryCharacter = value; }
        }
        [XmlAttribute]
        [CategoryAttribute("LMU3"), DescriptionAttribute("Output Trailer")]
        public string OutputTrailer3
        {
            get { return SerialConfig3.OutputTrailer; }
            set { SerialConfig3.OutputTrailer = value; }
        }
        [CategoryAttribute("LMU3"), DescriptionAttribute("Port Mode")]
        public SerialPortMode Mode3
        {
            get { return SerialConfig3.Mode; }
            set { SerialConfig3.Mode = value; }
        }
        [CategoryAttribute("LMU3"), DescriptionAttribute("Receive Timeout (secs)")]
        public int ReceiveTimeout3
        {
            get { return SerialConfig3.ReceiveTimeout; }
            set { SerialConfig3.ReceiveTimeout = value; }
        }
        [CategoryAttribute("LMU3"), DescriptionAttribute("Transmit retries")]
        public int TransmitRetries3
        {
            get { return SerialConfig3.TransmitRetries; }
            set { SerialConfig3.TransmitRetries = value; }
        }
        #endregion

        #region LMU4
        [CategoryAttribute("LMU4"), DescriptionAttribute("Rx Buffer Size")]
        public int ReceiveBufferSize4
        {
            get { return SerialConfig4.ReceiveBufferSize; }
            set { SerialConfig4.ReceiveBufferSize = value; }
        }
        [CategoryAttribute("LMU4"), DescriptionAttribute("Tx Buffer Size")]
        public int TransmitBufferSize4
        {
            get { return SerialConfig4.TransmitBufferSize; }
            set { SerialConfig4.TransmitBufferSize = value; }
        }
        [CategoryAttribute("LMU4"), DescriptionAttribute("Port HandShaking")]
        public System.IO.Ports.Handshake Handshake4
        {
            get { return SerialConfig4.Handshake; }
            set { SerialConfig4.Handshake = value; }
        }
        [CategoryAttribute("LMU4"), DescriptionAttribute("Parity")]
        public System.IO.Ports.Parity Parity4
        {
            get { return SerialConfig4.Parity; }
            set { SerialConfig4.Parity = value; }
        }
        [CategoryAttribute("LMU4"), DescriptionAttribute("Number of Stop bits")]
        public System.IO.Ports.StopBits StopBits4
        {
            get { return SerialConfig4.StopBits; }
            set { SerialConfig4.StopBits = value; }
        }
        [CategoryAttribute("LMU4"), DescriptionAttribute("Port Name")]
        public string PortName4
        {
            get { return SerialConfig4.PortName; }
            set { SerialConfig4.PortName = value; }
        }
        [CategoryAttribute("LMU4"), DescriptionAttribute("Baud Rate")]
        public int BaudRate4
        {
            get { return SerialConfig4.BaudRate; }
            set { SerialConfig4.BaudRate = value; }
        }
        [CategoryAttribute("LMU4"), DescriptionAttribute("Number Data bits")]
        public int DataBits4
        {
            get { return SerialConfig4.DataBits; }
            set { SerialConfig4.DataBits = value; }
        }
        [CategoryAttribute("LMU4"), DescriptionAttribute("Discard Nulls")]
        public bool DiscardNull4
        {
            get { return SerialConfig4.DiscardNull; }
            set { SerialConfig4.DiscardNull = value; }
        }
        [CategoryAttribute("LMU4"), DescriptionAttribute("Input Header")]
        [XmlAttribute]
        public string InputHeader4
        {
            get { return SerialConfig4.InputHeader; }
            set { SerialConfig4.InputHeader = value; }
        }
        [XmlAttribute]
        [CategoryAttribute("LMU4"), DescriptionAttribute("Output Header")]
        public string OutputHeader4
        {
            get { return SerialConfig4.OutputHeader; }
            set { SerialConfig4.OutputHeader = value; }
        }
        [XmlAttribute]
        [CategoryAttribute("LMU4"), DescriptionAttribute("Input Trailer")]
        public string InputTrailer4
        {
            get { return SerialConfig4.InputTrailer; }
            set { SerialConfig4.InputTrailer = value; }
        }
        [CategoryAttribute("LMU4"), DescriptionAttribute("Input Every Character")]
        public bool InputEveryCharacter4
        {
            get { return SerialConfig4.InputEveryCharacter; }
            set { SerialConfig4.InputEveryCharacter = value; }
        }
        [XmlAttribute]
        [CategoryAttribute("LMU4"), DescriptionAttribute("Output Trailer")]
        public string OutputTrailer4
        {
            get { return SerialConfig4.OutputTrailer; }
            set { SerialConfig4.OutputTrailer = value; }
        }
        [CategoryAttribute("LMU4"), DescriptionAttribute("Port Mode")]
        public SerialPortMode Mode4
        {
            get { return SerialConfig4.Mode; }
            set { SerialConfig4.Mode = value; }
        }
        [CategoryAttribute("LMU4"), DescriptionAttribute("Receive Timeout (secs)")]
        public int ReceiveTimeout4
        {
            get { return SerialConfig4.ReceiveTimeout; }
            set { SerialConfig4.ReceiveTimeout = value; }
        }
        [CategoryAttribute("LMU4"), DescriptionAttribute("Transmit retries")]
        public int TransmitRetries4
        {
            get { return SerialConfig4.TransmitRetries; }
            set { SerialConfig4.TransmitRetries = value; }
        }
        #endregion

        #region GlobalConfig
        // HIT Misc Group
        [CategoryAttribute("HIT Misc"), DescriptionAttribute("Active Cal File path and filename"),
        Editor(typeof(System.Windows.Forms.Design.FileNameEditor), typeof(System.Drawing.Design.UITypeEditor))]
        public string CalFilePathName
        {
            get { return GlobalConfig.CalFilePathName; }
            set { GlobalConfig.CalFilePathName = value; }
        }
        [CategoryAttribute("HIT Misc"), DescriptionAttribute("Default value Step timeout (secs)")]
        public int StepTimeoutSecs
        {
            get { return GlobalConfig.StepTimeoutSecs; }
            set { GlobalConfig.StepTimeoutSecs = value;}
        }
        [CategoryAttribute("HIT Misc"), DescriptionAttribute("Default values Step continue on error")]
        public bool StepContinueOnError
        {
            get { return GlobalConfig.StepContinueOnError ; }
            set { GlobalConfig.StepContinueOnError  = value; }
        }

        [CategoryAttribute("HIT Misc"), DescriptionAttribute("Default value Step number of retries")]
        public int StepNumberRetries
        {
            get { return GlobalConfig.StepNumberRetries; }
            set { GlobalConfig.StepNumberRetries = value; }
        }

        [CategoryAttribute("HIT Misc"), DescriptionAttribute("Log File destination directory"),
        Editor(typeof(System.Windows.Forms.Design.FolderNameEditor), typeof(System.Drawing.Design.UITypeEditor))]
        public string LogFileFolder
        {
            get { return GlobalConfig.LogFileFolder; }
            set { GlobalConfig.LogFileFolder = value; }
        }

        // MAP group
        [CategoryAttribute("MAP Settings"), DescriptionAttribute("MAP Executable full path and filename"),
        Editor(typeof(System.Windows.Forms.Design.FileNameEditor), typeof(System.Drawing.Design.UITypeEditor))]
        public string MapExePathName
        {
            get { return GlobalConfig.MapExePathName; }
            set { GlobalConfig.MapExePathName  = value; }
        }

        [CategoryAttribute("MAP Settings"), DescriptionAttribute("MAP Configuration Name")]
        public string MapConfigName
        {
            get { return GlobalConfig.MapConfigName ; }
            set { GlobalConfig.MapConfigName = value; }

        }
        [CategoryAttribute("MAP Settings"), DescriptionAttribute("MAP file output destination directory"),
        Editor(typeof(System.Windows.Forms.Design.FolderNameEditor), typeof(System.Drawing.Design.UITypeEditor))]
        public string MapFolderPath
        {
            get { return GlobalConfig.MapFolderPath; }
            set { GlobalConfig.MapFolderPath = value; }

        }

        // SNMP Group
        [CategoryAttribute("SNMP Settings"), DescriptionAttribute("SMLC1 IP Address")]
        public string SMLC1
        {
            get { return GlobalConfig.SMLC1; }
            set { GlobalConfig.SMLC1= value; }
        }
        [CategoryAttribute("SNMP Settings"), DescriptionAttribute("SMLC1 Port number")]
        public int SMLC1Port
        {
            get { return GlobalConfig.SMLC1Port; }
            set { GlobalConfig.SMLC1Port = value; }
        }
        [CategoryAttribute("SNMP Settings"), DescriptionAttribute("SMLC2 IP Address")]
        public string SMLC2
        {
            get { return GlobalConfig.SMLC2; }
            set { GlobalConfig.SMLC2 = value; }
        }

        [CategoryAttribute("SNMP Settings"), DescriptionAttribute("SMLC2 Port number")]
        public int SMLC2Port
        {
            get { return GlobalConfig.SMLC2Port; }
            set { GlobalConfig.SMLC2Port = value; }
        }
        [CategoryAttribute("SNMP Settings"), DescriptionAttribute("SMLC IP Address (Virtual or Active SMLC")]
        public string SMLCVirtual
        {
            get { return GlobalConfig.SMLCVirtual; }
            set { GlobalConfig.SMLCVirtual = value; }
        }
        [CategoryAttribute("SNMP Settings"), DescriptionAttribute("SMLC Port number (Virtual or Active SMLC")]
        public int SMLCVirtualPort
        {
            get { return GlobalConfig.SMLCVirtualPort; }
            set { GlobalConfig.SMLCVirtualPort = value; }
        }

        [CategoryAttribute("SNMP Settings"), DescriptionAttribute("LMU1 ID (Community string)")]
        public string LMU1_ID
        {
            get { return GlobalConfig.LMU1_ID; }
            set { GlobalConfig.LMU1_ID = value; }
        }
        [CategoryAttribute("SNMP Settings"), DescriptionAttribute("LMU2 ID (Community string)")]
        public string LMU2_ID
        {
            get { return GlobalConfig.LMU2_ID; }
            set { GlobalConfig.LMU2_ID= value; }
        }

        [CategoryAttribute("SNMP Settings"), DescriptionAttribute("LMU3 ID (Community string)")]
        public string LMU3_ID
        {
            get { return GlobalConfig.LMU3_ID; }
            set { GlobalConfig.LMU3_ID = value; }
        }
        [CategoryAttribute("SNMP Settings"), DescriptionAttribute("LMU4 ID (Community string)")]
        public string LMU4_ID
        {
            get { return GlobalConfig.LMU4_ID; }
            set { GlobalConfig.LMU4_ID = value; }
        }
        #endregion


    }
}