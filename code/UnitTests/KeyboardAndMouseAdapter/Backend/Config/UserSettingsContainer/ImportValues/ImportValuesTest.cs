using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using Pizza.KeyboardAndMouseAdapter.Backend.Config;
using System;
using System.IO;

namespace UnitTests.KeyboardAndMouseAdapter.Config.UserSettingsV2Test
{
    [TestClass]
    public class ImportValuesTest
    {
        private static readonly string PROJECT_ROOT = "..\\..\\..\\";
        private static readonly string TEST_DIRECTORY = PROJECT_ROOT + "UnitTests\\KeyboardAndMouseAdapter\\backend\\Config\\UserSettingsContainer\\ImportValues\\";

        private UserSettingsV3 ReadExpectedFile(string file)
        {
            string json = File.ReadAllText(file);
            return JsonConvert.DeserializeObject<UserSettingsV3>(json);
        }

        private void Run_ImportValues(string inputFile, string expectedFile)
        {
            UserSettingsContainer.ImportValues(inputFile);
            UserSettingsV3 actual = UserSettingsContainer.GetInstance();
            actual.KeyboardMappings = null;
            UserSettingsV3 expected = ReadExpectedFile(expectedFile);

            AssertionExtensions.Should(actual).BeEquivalentTo(expected);
        }

        [TestInitialize]
        public void BeforeEach()
        {
            UserSettingsContainer.TestOnly_ResetUserSettings();
        }

        [TestMethod]
        public void ShouldImport_Profile_1_0_11_Default()
        {
            string inputFile = TEST_DIRECTORY + "ImportValues--profile-1.0.11-default--input.json";
            string expectedFile = TEST_DIRECTORY + "ImportValues--profile-1.0.11-default--expected.json";
            UserSettingsContainer.ImportValues(inputFile);
            UserSettingsV3 actual = UserSettingsContainer.GetInstance();
            actual.KeyboardMappings = null;
            UserSettingsV3 expected = ReadExpectedFile(expectedFile);

            AssertionExtensions.Should(actual).BeEquivalentTo(expected);
        }

        [TestMethod]
        public void ShouldImport_Profile_2_0_0_Default()
        {
            string inputFile = TEST_DIRECTORY + "ImportValues--profile-2.0.0-default--input.json";
            string expectedFile = TEST_DIRECTORY + "ImportValues--profile-2.0.0-default--expected.json";
            UserSettingsContainer.ImportValues(inputFile);
            UserSettingsV3 actual = UserSettingsContainer.GetInstance();
            actual.KeyboardMappings = null;
            UserSettingsV3 expected = ReadExpectedFile(expectedFile);

            AssertionExtensions.Should(actual).BeEquivalentTo(expected);
        }

        [TestMethod]
        public void ShouldImport_Profile_Empty()
        {
            string inputFile = TEST_DIRECTORY + "ImportValues--profile-empty--input.json";
            string expectedFile = TEST_DIRECTORY + "ImportValues--profile-empty--expected.json";
            UserSettingsContainer.ImportValues(inputFile);
            UserSettingsV3 actual = UserSettingsContainer.GetInstance();
            actual.KeyboardMappings = null;
            UserSettingsV3 expected = ReadExpectedFile(expectedFile);

            AssertionExtensions.Should(actual).BeEquivalentTo(expected);
        }

        [TestMethod]
        public void ShouldImport_Property_AimToggle()
        {
            string inputFile = TEST_DIRECTORY + "ImportValues--property-AimToggle--input.json";
            string expectedFile = TEST_DIRECTORY + "ImportValues--property-AimToggle--expected.json";
            Run_ImportValues(inputFile, expectedFile);
        }

        [TestMethod]
        public void ShouldImport_Property_AimToggleRetoggleDelay()
        {
            string inputFile = TEST_DIRECTORY + "ImportValues--property-AimToggleRetoggleDelay--input.json";
            string expectedFile = TEST_DIRECTORY + "ImportValues--property-AimToggleRetoggleDelay--expected.json";
            Run_ImportValues(inputFile, expectedFile);
        }

        [TestMethod]
        public void ShouldImport_Property_MouseAimSensitivityEnabled()
        {
            string inputFile = TEST_DIRECTORY + "ImportValues--property-MouseAimSensitivityEnabled--input.json";
            string expectedFile = TEST_DIRECTORY + "ImportValues--property-MouseAimSensitivityEnabled--expected.json";
            Run_ImportValues(inputFile, expectedFile);
        }

        [TestMethod]
        public void ShouldImport_Property_RemotePlayVolume()
        {
            string inputFile = TEST_DIRECTORY + "ImportValues--property-RemotePlayVolume--input.json";
            string expectedFile = TEST_DIRECTORY + "ImportValues--property-RemotePlayVolume--expected.json";
            Run_ImportValues(inputFile, expectedFile);
        }
    }
}
