using Microsoft.VisualStudio.TestTools.UnitTesting;
using Pizza.Common;
using PS4KeyboardAndMouseAdapter.Config;
using PS4KeyboardAndMouseAdapter.UI.Controls.Welcome;
using System.Collections.Generic;

namespace UnitTests.KeyboardAndMouseAdapter.UI.Controls.Welcome.EmulationPickerControlTest
{

    [TestClass]
    public class DisableUnavailableRadioButtonsTest
    {

        [TestInitialize]
        public void BeforeEach()
        {
            ApplicationSettings.GetInstance().EmulationMode = -1;
        }

        [TestMethod]
        public void ShouldEnableAll_WhenVigemInstalled()
        {
            EmulationPickerControl classUnderTest = new EmulationPickerControl();
            classUnderTest.testonly_setIsVigemInstalled(true);

            classUnderTest.DisableUnavailableRadioButtons();

            List<System.Windows.Controls.RadioButton> actualRadioButtons = classUnderTest.testonly_getRadioButtons();
            Assert.AreEqual(true, actualRadioButtons[0].IsEnabled);
            Assert.AreEqual(true, actualRadioButtons[1].IsEnabled);
            Assert.AreEqual(true, actualRadioButtons[2].IsEnabled);
        }

        [TestMethod]
        public void ShouldEnableProcess_WhenVigemNotInstalled()
        {
            EmulationPickerControl classUnderTest = new EmulationPickerControl();
            classUnderTest.testonly_setIsVigemInstalled(false);

            classUnderTest.DisableUnavailableRadioButtons();

            List<System.Windows.Controls.RadioButton> actualRadioButtons = classUnderTest.testonly_getRadioButtons();
            Assert.AreEqual(false, actualRadioButtons[0].IsEnabled);
            Assert.AreEqual(false, actualRadioButtons[1].IsEnabled);
            Assert.AreEqual(true, actualRadioButtons[2].IsEnabled);
        }
    }
}
