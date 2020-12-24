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
        private readonly string PROJECT_ROOT="..\\..\\..\\";

        private UserSettings ReadExpectedfile(string file) {
            string json = File.ReadAllText(file);
            return JsonConvert.DeserializeObject<UserSettings>(json);
        }

        [TestMethod]
        public void ShouldImport_1_0_11()
        {
            string inputFile = PROJECT_ROOT + "UnitTests\\KeyboardAndMouseAdapter\\Config\\UserSettings\\ImportValues--profile-1.0.11-default--input.json";
            string expectedFile = PROJECT_ROOT + "UnitTests\\KeyboardAndMouseAdapter\\Config\\UserSettings\\ImportValues--profile-1.0.11-default--expected.json";
            UserSettings.ImportValues(inputFile);
            var actual = UserSettings.GetInstance();
            var expected = ReadExpectedfile(expectedFile);
            AssertionExtensions.Should(actual).BeEquivalentTo(expected);
        }

       // [TestMethod]
       // public void ShouldImport_1_0_12()
       // {
       //     string inputFile = PROJECT_ROOT + "UnitTests\\KeyboardAndMouseAdapter\\Config\\UserSettings\\ImportValues--profile-1.0.12-default--input.json";
       //     string expectedFile = PROJECT_ROOT + "UnitTests\\KeyboardAndMouseAdapter\\Config\\UserSettings\\ImportValues--profile-1.0.12-default--expected.json";
       //     UserSettings.ImportValues(inputFile);
       //     var actual = UserSettings.GetInstance();
       //     var expected = ReadExpectedfile(expectedFile);
       //     AssertionExtensions.Should(actual).BeEquivalentTo(expected);
       // }

    }
}
