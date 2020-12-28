using System;
using System.IO;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using PS4KeyboardAndMouseAdapter.Config;

namespace UnitTests.KeyboardAndMouseAdapter.Config.UserSettingsTest
{
    [TestClass]
    public class ImportValuesTest
    {
        private readonly string PROJECT_ROOT = "..\\..\\..\\";

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
        public void ShouldImport_1_0_11()
        {
            string inputFile = PROJECT_ROOT + "UnitTests\\KeyboardAndMouseAdapter\\Config\\UserSettings\\ImportValues--profile-1.0.11-default--input.json";
            string expectedFile = PROJECT_ROOT + "UnitTests\\KeyboardAndMouseAdapter\\Config\\UserSettings\\ImportValues--profile-1.0.11-default--expected.json";
            UserSettings.ImportValues(inputFile);
            UserSettings actual = UserSettings.GetInstance();
            actual.KeyboardMappings = null;
            UserSettings expected = ReadExpectedFile(expectedFile);
            AssertionExtensions.Should(actual).BeEquivalentTo(expected);
        }

        [TestMethod]
        public void ShouldImport_1_0_12_Default()
        {
            string inputFile = PROJECT_ROOT + "UnitTests\\KeyboardAndMouseAdapter\\Config\\UserSettings\\ImportValues--profile-1.0.12-default--input.json";
            string expectedFile = PROJECT_ROOT + "UnitTests\\KeyboardAndMouseAdapter\\Config\\UserSettings\\ImportValues--profile-1.0.12-default--expected.json";
            UserSettings.ImportValues(inputFile);
            UserSettings actual = UserSettings.GetInstance();
            actual.KeyboardMappings = null;
            UserSettings expected = ReadExpectedFile(expectedFile);
            AssertionExtensions.Should(actual).BeEquivalentTo(expected);
        }

        [TestMethod]
        public void ShouldImport_Empty()
        {
            string inputFile = PROJECT_ROOT + "UnitTests\\KeyboardAndMouseAdapter\\Config\\UserSettings\\ImportValues--profile-empty--input.json";
            string expectedFile = PROJECT_ROOT + "UnitTests\\KeyboardAndMouseAdapter\\Config\\UserSettings\\ImportValues--profile-empty--expected.json";
            UserSettings.ImportValues(inputFile);
            UserSettings actual = UserSettings.GetInstance();
            actual.KeyboardMappings = null;
            Console.WriteLine("actual" + actual.Mappings);
            UserSettings expected = ReadExpectedFile(expectedFile);
            AssertionExtensions.Should(actual).BeEquivalentTo(expected);
        }

    }
}
