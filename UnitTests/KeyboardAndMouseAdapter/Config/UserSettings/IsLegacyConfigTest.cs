using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PS4KeyboardAndMouseAdapter.Config;

namespace UnitTests.KeyboardAndMouseAdapter.Config.UserSettingsTest
{
    [TestClass]
    public class IsLegacyConfigTest
    {
        private readonly string PROJECT_ROOT ="..\\..\\..\\";

        private void IsLegacyConfigRunner( string file, bool expected) {
            string json = File.ReadAllText(file);
            bool actual = UserSettings.IsLegacyConfig(json);
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void ShouldDetect_1_0_11_IsLegacy()
        {
            string file = PROJECT_ROOT + "UnitTests\\KeyboardAndMouseAdapter\\Config\\UserSettings\\IsLegacyConfig--profile-1.0.11-default.json";
            IsLegacyConfigRunner(file, true);
        }

        [TestMethod]
        public void ShouldDetect_1_0_12_IsCurrent()
        {
            string file = PROJECT_ROOT + "UnitTests\\KeyboardAndMouseAdapter\\Config\\UserSettings\\IsLegacyConfig--profile-1.0.12-provisional.json";
            IsLegacyConfigRunner(file, true);
        }
    }
}
