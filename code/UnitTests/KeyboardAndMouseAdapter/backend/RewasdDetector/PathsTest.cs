using Microsoft.VisualStudio.TestTools.UnitTesting;
using Pizza.KeyboardAndMouseAdapter.Backend;
using System;

namespace UnitTests.KeyboardAndMouseAdapter.backend.Config
{
    [TestClass]
    public class RewasdDetectorTest
    {
        public void DebugPaths(string label, string[] paths)
        {
            Console.WriteLine(label);
            foreach (string path in paths)
            {
                Console.WriteLine(path);
            }
        }

        [TestMethod]
        public void ShouldBeCorrect()
        {
            string[] actual = RewasdDetector.PATHS;

            string[] expected = new string[] {
                @"C:\Program Files\reWASD\reWASD.exe",
                @"C:\Program Files (x86)\reWASD\reWASD.exe"
            };

            //this only logs if this test fails
            DebugPaths("actual", actual);
            DebugPaths("expected", expected);

            CollectionAssert.AreEqual(expected, actual);
        }
    }
}
