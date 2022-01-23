using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using Pizza.KeyboardAndMouseAdapter.Backend.Config;
using System.IO;

namespace UnitTests.KeyboardAndMouseAdapter.Config.ApplicationSettingsTest
{
    [TestClass]
    public class ImportValuesTest
    {
        private static readonly string PROJECT_ROOT = "..\\..\\..\\";
        private static readonly string PROFILE_DIRECTORY = PROJECT_ROOT + "UnitTests\\KeyboardAndMouseAdapter\\backend\\Config\\ApplicationSettings\\";

        private ApplicationSettings ReadExpectedFile(string file)
        {
            string json = File.ReadAllText(file);
            return JsonConvert.DeserializeObject<ApplicationSettings>(json);
        }

        [TestInitialize]
        public void BeforeEach()
        {
            ApplicationSettings.TestOnly_ResetApplicationSettings();
        }

        [TestMethod]
        public void ShouldImport_Empty()
        {
            string inputFile = PROFILE_DIRECTORY + "ImportValues--Empty--input.json";
            string expectedFile = PROFILE_DIRECTORY + "ImportValues--Empty--expected.json";
            ApplicationSettings.ImportValues(inputFile);
            ApplicationSettings actual = ApplicationSettings.GetInstance();
            ApplicationSettings expected = ReadExpectedFile(expectedFile);

            AssertionExtensions.Should(actual).BeEquivalentTo(expected);
        }

        [TestMethod]
        public void ShouldImport_Property_GamepadUpdaterNoSleep()
        {
            string inputFile = PROFILE_DIRECTORY + "ImportValues--property-GamepadUpdaterNoSleep--input.json";
            string expectedFile = PROFILE_DIRECTORY + "ImportValues--property-GamepadUpdaterNoSleep--expected.json";
            ApplicationSettings.ImportValues(inputFile);
            ApplicationSettings actual = ApplicationSettings.GetInstance();
            ApplicationSettings expected = ReadExpectedFile(expectedFile);

            AssertionExtensions.Should(actual).BeEquivalentTo(expected);
        }

    }
}
