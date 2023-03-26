using Microsoft.VisualStudio.TestTools.UnitTesting;
using Pizza.KeyboardAndMouseAdapter.Backend.Config;
using Pizza.TestTools;
using System.IO;

namespace UnitTests.KeyboardAndMouseAdapter.Config.ApplicationSettingsTest
{
    [TestClass]
    public class ImportValuesTest
    {
        private static readonly string PROJECT_ROOT = "..\\..\\..\\..\\";
        private static readonly string PROFILE_DIRECTORY = PROJECT_ROOT + "UnitTests\\KeyboardAndMouseAdapter\\backend\\Config\\ApplicationSettings\\";

        private string ReadExpectedFile(string file)
        {
            return File.ReadAllText(file);
        }

        [TestInitialize]
        public void BeforeEach()
        {
            ApplicationSettings.TestOnly_ResetApplicationSettings();
        }

        [TestMethod]
        public void ShouldImport_Default_whenEmpty()
        {
            string inputFile = PROFILE_DIRECTORY + "ImportValues--Default_whenEmpty--input.json";
            string expectedFile = PROFILE_DIRECTORY + "ImportValues--Default_whenEmpty--expected.json";
            ApplicationSettings.ImportValues(inputFile);
            string actual = ApplicationSettings.GetInstance().GetAsJson();
            string expected = ReadExpectedFile(expectedFile);

            ExtendedAsserts.JsonsBeEquivalent(actual, expected);
        }

        [TestMethod]
        public void ShouldImport_Property_ColourSchemeIsLight()
        {
            string inputFile = PROFILE_DIRECTORY + "ImportValues--property-ColourSchemeIsLight--input.json";
            string expectedFile = PROFILE_DIRECTORY + "ImportValues--property-ColourSchemeIsLight--expected.json";
            ApplicationSettings.ImportValues(inputFile);
            string actual = ApplicationSettings.GetInstance().GetAsJson();
            string expected = ReadExpectedFile(expectedFile);

            ExtendedAsserts.JsonsBeEquivalent(actual, expected);
        }

        [TestMethod]
        public void ShouldImport_Property_GamepadUpdaterNoSleep()
        {
            string inputFile = PROFILE_DIRECTORY + "ImportValues--property-GamepadUpdaterNoSleep--input.json";
            string expectedFile = PROFILE_DIRECTORY + "ImportValues--property-GamepadUpdaterNoSleep--expected.json";
            ApplicationSettings.ImportValues(inputFile);
            string actual = ApplicationSettings.GetInstance().GetAsJson();
            string expected = ReadExpectedFile(expectedFile);

            ExtendedAsserts.JsonsBeEquivalent(actual, expected);
        }

    }
}
