using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PS4KeyboardAndMouseAdapter.Config;

namespace UnitTests.KeyboardAndMouseAdapter.Config.UserSettingsTest
{
    [TestClass]
    public class ImportValuesTest
    {
        private readonly string PROJECT_ROOT="..\\..\\..\\";

        private void HasNoNullProperties(UserSettings settings) {
           
        }

        [TestMethod]
        public void ShouldImport_1_0_11()
        {
            string file = PROJECT_ROOT + "UnitTests\\KeyboardAndMouseAdapter\\Config\\UserSettings\\ImportValues-profile-1.0.11-default.json";
            UserSettings.Load(file);
        }

        //TODO
        //[TestMethod]
        //public void ShouldDetect_1_0_12_IsCurrent()
        //{
        //    string file = PROJECT_ROOT + "UnitTests\\KeyboardAndMouseAdapter\\Config\\UserSettings\\profile-1.0.12-provisional.json";
        //    UserSettings.Load(file);
        //}
    }
}
