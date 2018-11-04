using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;

namespace DockSample
{
// The following I expect to get from Port Interfaces
    //COM port Assignments (LMU 1-4, port, baud rate, stop bits)	Text, Integers	Edits	None	Yes
    //SigGen #1 IEEE address	Integer	Edit	"none"	Yes
    //SigGen #2 IEEE address	Integer	Edit	"none"	Yes
    //Power Meter IEEE address	Integer	Edit	"none"	Yes

   

    public class ConfigData 
    {
        public int ReceiveBufferSize { get; set; }
        // HIT Misc Group
        [CategoryAttribute("HIT Misc"), DescriptionAttribute("Active Cal File path and filename"),
        Editor(typeof(System.Windows.Forms.Design.FileNameEditor), typeof(System.Drawing.Design.UITypeEditor))]
        public string CalFilePathName {get; set;}
        [CategoryAttribute("HIT Misc"), DescriptionAttribute("Default value Step timeout (secs)")]
        public int StepTimeoutSecs { get;set; }
        [CategoryAttribute("HIT Misc"), DescriptionAttribute("Default values Step continue on error")]
        public bool StepContinueOnError { get;set; }
        [CategoryAttribute("HIT Misc"), DescriptionAttribute("Default value Step number of retries")]
        public int StepNumberRetries { get;set; }
        [CategoryAttribute("HIT Misc"), DescriptionAttribute("Log File destination directory"),
        Editor(typeof(System.Windows.Forms.Design.FolderNameEditor), typeof(System.Drawing.Design.UITypeEditor))]
        public string LogFileFolder {get; set;}
        
        // MAP group
        [CategoryAttribute("MAP Settings"), DescriptionAttribute("MAP Executable full path and filename"),
        Editor(typeof(System.Windows.Forms.Design.FileNameEditor), typeof(System.Drawing.Design.UITypeEditor))]
        public  string MapExePathName { get; set; }
        [CategoryAttribute("MAP Settings"), DescriptionAttribute("MAP Configuration Name")]
        public string MapConfigName { get; set; }
        [CategoryAttribute("MAP Settings"), DescriptionAttribute("MAP file output destination directory"),
        Editor(typeof(System.Windows.Forms.Design.FolderNameEditor), typeof(System.Drawing.Design.UITypeEditor))]
        public string MapFolderPath {get; set;}

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