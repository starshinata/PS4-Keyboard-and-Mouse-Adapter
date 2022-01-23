using Microsoft.VisualStudio.TestTools.UnitTesting;
using Pizza.Common;
using Pizza.KeyboardAndMouseAdapter.Backend.Config;
using Pizza.KeyboardAndMouseAdapter.UI.Controls.Welcome;

namespace UnitTests.KeyboardAndMouseAdapter.UI.Controls.Welcome.EmulationPickerControlTest.SetInitialRadioChecked
{

    [TestClass]
    public class WhenVigemNotInstalled
    {

        EmulationPickerControl classUnderTest;

        [TestInitialize]
        public void BeforeEach()
        {
            classUnderTest = new EmulationPickerControl();
            classUnderTest.testonly_setIsVigemInstalled(false);
        }

        [TestMethod]
        public void ShouldIgnore__ONLY_PROCESS_INJECTION()
        {
            ApplicationSettings.GetInstance().EmulationMode = EmulationConstants.ONLY_PROCESS_INJECTION;

            classUnderTest.SetInitialRadioChecked();
            int actual = classUnderTest.GetValue();
            int expected = EmulationConstants.ONLY_PROCESS_INJECTION;
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void ShouldIgnore__ONLY_VIGEM()
        {
            ApplicationSettings.GetInstance().EmulationMode = EmulationConstants.ONLY_VIGEM;

            classUnderTest.SetInitialRadioChecked();
            int actual = classUnderTest.GetValue();
            int expected = EmulationConstants.ONLY_PROCESS_INJECTION;
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void ShouldIgnore__VIGEM_AND_PROCESS_INJECTION()
        {
            ApplicationSettings.GetInstance().EmulationMode = EmulationConstants.VIGEM_AND_PROCESS_INJECTION;

            classUnderTest.SetInitialRadioChecked();
            int actual = classUnderTest.GetValue();
            int expected = EmulationConstants.ONLY_PROCESS_INJECTION;
            Assert.AreEqual(expected, actual);
        }
    }
}
