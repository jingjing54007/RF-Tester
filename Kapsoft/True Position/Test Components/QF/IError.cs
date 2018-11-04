using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TruePosition.Test.QF
{
    public interface IError
    {
        string Error { get; }
        string AdditionalDescription { get; }
    }
}
