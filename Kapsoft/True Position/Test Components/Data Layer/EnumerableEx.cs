using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TruePosition.Test.DataLayer
{
    public static class EnumerableEx
    {
        public static TestList<T> ToTestList<T>(this IEnumerable<T> source)
            where T : ITestClass
        {
            return new TestList<T>(source);
        }
    }
}
