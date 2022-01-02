using Microsoft.VisualStudio.TestTools.UnitTesting;
using Pizza.Common;
using PS4KeyboardAndMouseAdapter.Config;
using PS4KeyboardAndMouseAdapter.UI.Controls.Welcome;

namespace UnitTests.KeyboardAndMouseAdapter.UI.Controls.Welcome.EmulationPickerControlTest.SetInitialRadioChecked
{

    [TestClass]
    public class WhenVigemInstalled
    {

        EmulationPickerControl classUnderTest;

        [TestInitialize]
        public void BeforeEach()
        {
            ApplicationSettings.GetInstance().EmulationMode = -1;

            classUnderTest = new EmulationPickerControl();
            classUnderTest.testonly_setIsVigemInstalled(true);
        }

        [TestMethod]
        public void ShouldSelect__ONLY_PROCESS_INJECTION()
        {
            ApplicationSettings.GetInstance().EmulationMode = EmulationConstants.ONLY_PROCESS_INJECTION;

            classUnderTest.SetInitialRadioChecked();
            int actual = classUnderTest.GetValue();
            int expected = EmulationConstants.ONLY_PROCESS_INJECTION;
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void ShouldSelect__ONLY_VIGEM()
        {
            ApplicationSettings.GetInstance().EmulationMode = EmulationConstants.ONLY_VIGEM;

            classUnderTest.SetInitialRadioChecked();
            int actual = classUnderTest.GetValue();
            int expected = EmulationConstants.ONLY_VIGEM;
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void ShouldSelect__VIGEM_AND_PROCESS_INJECTION()
        {
            ApplicationSettings.GetInstance().EmulationMode = EmulationConstants.VIGEM_AND_PROCESS_INJECTION;

            classUnderTest.SetInitialRadioChecked();
            int actual = classUnderTest.GetValue();
            int expected = EmulationConstants.VIGEM_AND_PROCESS_INJECTION;
            Assert.AreEqual(expected, actual);
        }
    }
}
