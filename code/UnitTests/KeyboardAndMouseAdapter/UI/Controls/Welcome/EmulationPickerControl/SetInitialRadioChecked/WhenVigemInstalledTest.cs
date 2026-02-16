using Microsoft.VisualStudio.TestTools.UnitTesting;
using Pizza.Common;
using Pizza.KeyboardAndMouseAdapter.Backend.Config;
using Pizza.KeyboardAndMouseAdapter.UI.Controls.Welcome;
using Pizza.TestTools;

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

        [TestMethodForUiControl]
        public void ShouldSelect__ONLY_PROCESS_INJECTION()
        {
            ApplicationSettings.GetInstance().EmulationMode = EmulationConstants.ONLY_PROCESS_INJECTION;

            classUnderTest.SetInitialRadioChecked();
            int actual = classUnderTest.GetValue();
            int expected = EmulationConstants.ONLY_PROCESS_INJECTION;
            Assert.AreEqual(expected, actual);
        }

        [TestMethodForUiControl]
        public void ShouldSelect__ONLY_VIGEM()
        {
            ApplicationSettings.GetInstance().EmulationMode = EmulationConstants.ONLY_VIGEM;

            classUnderTest.SetInitialRadioChecked();
            int actual = classUnderTest.GetValue();
            int expected = EmulationConstants.ONLY_VIGEM;
            Assert.AreEqual(expected, actual);
        }

        [TestMethodForUiControl]
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
