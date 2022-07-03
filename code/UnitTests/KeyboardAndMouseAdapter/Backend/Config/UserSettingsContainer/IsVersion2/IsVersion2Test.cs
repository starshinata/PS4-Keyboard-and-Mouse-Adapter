using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Pizza.KeyboardAndMouseAdapter.Backend.Config;

namespace UnitTests.KeyboardAndMouseAdapter.Config.UserSettingsContainerTest
{
    [TestClass]
    public class IsVersion2_Test
    {
        private static readonly string PROJECT_ROOT = "..\\..\\..\\";
        private static readonly string TEST_DIRECTORY = PROJECT_ROOT + "UnitTests\\KeyboardAndMouseAdapter\\backend\\Config\\UserSettingsContainer\\IsVersion2\\";

        private void run_IsVersion2(string file, bool expected)
        {
            string json = File.ReadAllText(file);
            bool actual = UserSettingsContainer.IsVersion2(json);
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void ShouldDetect_1_0_11_IsLegacy()
        {
            string file = TEST_DIRECTORY + "profile-v1-default-1.0.11.json";
            run_IsVersion2(file, false);
        }

        [TestMethod]
        public void ShouldDetect_2_0_0_IsCurrent()
        {
            string file = TEST_DIRECTORY + "profile-v2.json";
            run_IsVersion2(file, true);
        }
    }
}
