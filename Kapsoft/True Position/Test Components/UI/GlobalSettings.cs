using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TruePosition.Test.UI
{
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
        string LMU1CommunityString { get; set; }
        string LMU2CommunityString { get; set; }
        string LMU3CommunityString { get; set; }
        string LMU4CommunityString { get; set; }
        string CellBand { get; set; }
    }
    public class GlobalSettings : IGlobalConfig
    {
        public string CalFilePathName { get; set; }
        public int StepTimeoutSecs { get; set; }
        public bool StepContinueOnError { get; set; }
        public int StepNumberRetries { get; set; }
        public string LogFileFolder { get; set; }
        public string MapExePathName { get; set; }
        public string MapConfigName { get; set; }
        public string MapFolderPath { get; set; }
        public string SMLC1 { get; set; }
        public int SMLC1Port { get; set; }
        public string SMLC2 { get; set; }
        public int SMLC2Port { get; set; }
        public string SMLCVirtual { get; set; }
        public int SMLCVirtualPort { get; set; }
        public string LMU1_ID { get; set; }
        public string LMU2_ID { get; set; }
        public string LMU3_ID { get; set; }
        public string LMU4_ID { get; set; }
        public string LMU1CommunityString { get; set; }
        public string LMU2CommunityString { get; set; }
        public string LMU3CommunityString { get; set; }
        public string LMU4CommunityString { get; set; }
        public string CellBand { get; set; }
    }
}
