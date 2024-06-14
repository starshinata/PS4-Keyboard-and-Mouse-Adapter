using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Pizza.KeyboardAndMouseAdapter.Backend.Config;

namespace UnitTests.KeyboardAndMouseAdapter.Config.UserSettingsContainerTest
{
    [TestClass]
    public class IsVersion2_Test
    {
        private static readonly string PROJECT_ROOT = "..\\..\\..\\";
        private static readonly string TEST_DIRECTORY = PROJECT_ROOT + "UnitTests\\KeyboardAndMouseAdapter\\backend\\Config\\UserSettingsContainer\\IsVersionX\\";

        private void run_IsVersion2(string file, bool expected)
        {
            string json = File.ReadAllText(file);
            bool actual = UserSettingsContainer.IsVersion2(json);
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void Version1_ShouldReturn_False()
        {
            string file = TEST_DIRECTORY + "profile-v1-default-1.0.11.json";
            run_IsVersion2(file, false);
        }

        [TestMethod]
        public void Version2_ShouldReturn_True()
        {
            string file = TEST_DIRECTORY + "profile-v2.json";
            run_IsVersion2(file, true);
        }

        [TestMethod]
        public void Version3_ShouldReturn_False()
        {
            string file = TEST_DIRECTORY + "profile-v3.json";
            run_IsVersion2(file, false);
        }
    }
}
