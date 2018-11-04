using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;

using TruePosition.Test.DataLayer;

namespace TruePosition.Test.XML
{
    public static class TestExtensions
    {
        public static string Filename(this DataLayer.Test test)
        {
            return test.Name.Split(Path.GetInvalidFileNameChars(),
                                   StringSplitOptions.RemoveEmptyEntries).Aggregate((agg, n) => agg + n) + ".xml";
        }
        private static void Save(string destinationPath, string filename, DataLayer.Test test)
        {
            XElement testXml = Dehydrator.Dehydrate(test);
            testXml.Save(Path.Combine(destinationPath, filename));
        }
        public static void Save(this DataLayer.Test test, string destinationPath, string filename)
        {
            if (!Directory.Exists(destinationPath))
                Directory.CreateDirectory(destinationPath);

            Save(destinationPath, filename, test);
        }
        public static void Save(this DataLayer.Test test, string destinationPath)
        {
            if (!Directory.Exists(destinationPath))
                Directory.CreateDirectory(destinationPath);

            Save(destinationPath, test.Filename(), test);
        }
        public static void Save(this IEnumerable<DataLayer.Test> tests, string destinationPath)
        {
            if (!Directory.Exists(destinationPath))
                Directory.CreateDirectory(destinationPath);

            foreach (DataLayer.Test test in tests)
                Save(destinationPath, test.Filename(), test);
        }
    }
}
