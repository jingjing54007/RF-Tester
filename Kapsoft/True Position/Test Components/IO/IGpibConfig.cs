using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TruePosition.Test.IO
{
    public interface IGpibConfig
    {
        string Name { get; }
        int ReceiveTimeout { get; set; }
        int BoardId { get; set; }
        byte PrimaryAddress { get; set; }
        byte SecondaryAddress { get; set; }
    }
}
