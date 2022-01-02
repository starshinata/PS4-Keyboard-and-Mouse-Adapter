using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using PS4KeyboardAndMouseAdapter.Config;
using System.IO;

namespace UnitTests.KeyboardAndMouseAdapter.Config.UserSettingsTest
{
    [TestClass]
    public class ImportValuesTest
    {
        private static readonly string PROJECT_ROOT = "..\\..\\..\\";
        private static readonly string PROFILE_DIRECTORY = PROJECT_ROOT + "UnitTests\\KeyboardAndMouseAdapter\\backend\\Config\\UserSettings\\";

        private UserSettings ReadExpectedFile(string file)
        {
            string json = File.ReadAllText(file);
            return JsonConvert.DeserializeObject<UserSettings>(json);
        }

        [TestInitialize]
        public void BeforeEach()
        {
            UserSettings.TestOnly_ResetUserSettings();
        }

        [TestMethod]
        public void ShouldImport_Profile_1_0_11_Default()
        {
            string inputFile = PROFILE_DIRECTORY + "ImportValues--profile-1.0.11-default--input.json";
            string expectedFile = PROFILE_DIRECTORY + "ImportValues--profile-1.0.11-default--expected.json";
            UserSettings.ImportValues(inputFile);
            UserSettings actual = UserSettings.GetInstance();
            actual.KeyboardMappings = null;
            UserSettings expected = ReadExpectedFile(expectedFile);

            AssertionExtensions.Should(actual).BeEquivalentTo(expected);
        }

        [TestMethod]
        public void ShouldImport_Profile_2_0_0_Default()
        {
            string inputFile = PROFILE_DIRECTORY + "ImportValues--profile-2.0.0-default--input.json";
            string expectedFile = PROFILE_DIRECTORY + "ImportValues--profile-2.0.0-default--expected.json";
            UserSettings.ImportValues(inputFile);
            UserSettings actual = UserSettings.GetInstance();
            actual.KeyboardMappings = null;
            UserSettings expected = ReadExpectedFile(expectedFile);

            AssertionExtensions.Should(actual).BeEquivalentTo(expected);
        }

        [TestMethod]
        public void ShouldImport_Profile_Empty()
        {
            string inputFile = PROFILE_DIRECTORY + "ImportValues--profile-empty--input.json";
            string expectedFile = PROFILE_DIRECTORY + "ImportValues--profile-empty--expected.json";
            UserSettings.ImportValues(inputFile);
            UserSettings actual = UserSettings.GetInstance();
            actual.KeyboardMappings = null;
            UserSettings expected = ReadExpectedFile(expectedFile);

            AssertionExtensions.Should(actual).BeEquivalentTo(expected);
        }

        [TestMethod]
        public void ShouldImport_Property_AimToggle()
        {
            string inputFile = PROFILE_DIRECTORY + "ImportValues--property-AimToggle--input.json";
            string expectedFile = PROFILE_DIRECTORY + "ImportValues--property-AimToggle--expected.json";
            UserSettings.ImportValues(inputFile);
            UserSettings actual = UserSettings.GetInstance();
            actual.KeyboardMappings = null;
            UserSettings expected = ReadExpectedFile(expectedFile);

            AssertionExtensions.Should(actual).BeEquivalentTo(expected);
        }

        [TestMethod]
        public void ShouldImport_Property_AimToggleRetoggleDelay()
        {
            string inputFile = PROFILE_DIRECTORY + "ImportValues--property-AimToggleRetoggleDelay--input.json";
            string expectedFile = PROFILE_DIRECTORY + "ImportValues--property-AimToggleRetoggleDelay--expected.json";
            UserSettings.ImportValues(inputFile);
            UserSettings actual = UserSettings.GetInstance();
            actual.KeyboardMappings = null;
            UserSettings expected = ReadExpectedFile(expectedFile);

            AssertionExtensions.Should(actual).BeEquivalentTo(expected);
        }

        [TestMethod]
        public void ShouldImport_Property_MouseAimSensitivityEnabled()
        {
            string inputFile = PROFILE_DIRECTORY + "ImportValues--property-MouseAimSensitivityEnabled--input.json";
            string expectedFile = PROFILE_DIRECTORY + "ImportValues--property-MouseAimSensitivityEnabled--expected.json";
            UserSettings.ImportValues(inputFile);
            UserSettings actual = UserSettings.GetInstance();
            actual.KeyboardMappings = null;
            UserSettings expected = ReadExpectedFile(expectedFile);

            AssertionExtensions.Should(actual).BeEquivalentTo(expected);
        }

        [TestMethod]
        public void ShouldImport_Property_RemotePlayVolume()
        {
            string inputFile = PROFILE_DIRECTORY + "ImportValues--property-RemotePlayVolume--input.json";
            string expectedFile = PROFILE_DIRECTORY + "ImportValues--property-RemotePlayVolume--expected.json";
            UserSettings.ImportValues(inputFile);
            UserSettings actual = UserSettings.GetInstance();
            actual.KeyboardMappings = null;
            UserSettings expected = ReadExpectedFile(expectedFile);

            AssertionExtensions.Should(actual).BeEquivalentTo(expected);
        }
    }
}
