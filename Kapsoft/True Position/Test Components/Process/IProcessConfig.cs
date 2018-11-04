using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TruePosition.Test.Process
{
    public interface IProcessConfig
    {
        int PreLaunchDelay { get; set; }
        int PostLaunchDelay { get; set; }
        int LaunchTimeout { get; set; }
    }
}
