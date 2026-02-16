using Microsoft.VisualStudio.TestTools.UnitTesting;
using Pizza.KeyboardAndMouseAdapter.Backend.Config;
using Pizza.KeyboardAndMouseAdapter.UI.Controls.Welcome;
using Pizza.TestTools;
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

        [TestMethodForUiControl]
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

        [TestMethodForUiControl]
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
