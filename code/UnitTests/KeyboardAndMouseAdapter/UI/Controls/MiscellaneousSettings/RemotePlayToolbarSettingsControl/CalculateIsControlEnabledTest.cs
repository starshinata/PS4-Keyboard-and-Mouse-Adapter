using Microsoft.VisualStudio.TestTools.UnitTesting;
using Pizza.Common;
using Pizza.KeyboardAndMouseAdapter.Backend.Config;
using Pizza.KeyboardAndMouseAdapter.UI;
using Pizza.KeyboardAndMouseAdapter.UI.Controls.MiscellaneousSettingsPage;

namespace UnitTests.KeyboardAndMouseAdapter.UI.Controls.RemotePlayToolbarSettingsControlTest
{

    [TestClass]
    public class CalculateIsControlEnabledTest
    {

        [TestMethod]
        public void WhenEmulationMode__ONLY_PROCESS_INJECTION__ShouldEnableTheControl()
        {
            ApplicationSettings.GetInstance().EmulationMode = EmulationConstants.ONLY_PROCESS_INJECTION;

            RemotePlayToolbarSettingsControl classUnderTest = new RemotePlayToolbarSettingsControl();
            classUnderTest.CalculateIsControlEnabled();

            Assert.AreEqual(UIConstants.VISIBILITY_COLLAPSED, classUnderTest.testonly_GetElement_TextBlock_Warning().Visibility);
            Assert.AreEqual(UIConstants.VISIBILITY_VISIBLE, classUnderTest.testonly_GetElement_TextBlock_Description().Visibility);
            Assert.AreEqual(UIConstants.VISIBILITY_VISIBLE, classUnderTest.testonly_GetElement_Toggle().Visibility);
        }

        [TestMethod]
        public void WhenEmulationMode__ONLY_VIGEM__ShouldDisableTheControl()
        {
            ApplicationSettings.GetInstance().EmulationMode = EmulationConstants.ONLY_VIGEM;

            RemotePlayToolbarSettingsControl classUnderTest = new RemotePlayToolbarSettingsControl();
            classUnderTest.CalculateIsControlEnabled();

            Assert.AreEqual(UIConstants.VISIBILITY_COLLAPSED, classUnderTest.testonly_GetElement_TextBlock_Description().Visibility);
            Assert.AreEqual(UIConstants.VISIBILITY_COLLAPSED, classUnderTest.testonly_GetElement_Toggle().Visibility);
            Assert.AreEqual(UIConstants.VISIBILITY_VISIBLE, classUnderTest.testonly_GetElement_TextBlock_Warning().Visibility);
        }

        [TestMethod]
        public void WhenEmulationMode__Unknown__ShouldEnableTheControl()
        {
            ApplicationSettings.GetInstance().EmulationMode = -1;

            RemotePlayToolbarSettingsControl classUnderTest = new RemotePlayToolbarSettingsControl();
            classUnderTest.CalculateIsControlEnabled();

            Assert.AreEqual(UIConstants.VISIBILITY_COLLAPSED, classUnderTest.testonly_GetElement_TextBlock_Warning().Visibility);
            Assert.AreEqual(UIConstants.VISIBILITY_VISIBLE, classUnderTest.testonly_GetElement_TextBlock_Description().Visibility);
            Assert.AreEqual(UIConstants.VISIBILITY_VISIBLE, classUnderTest.testonly_GetElement_Toggle().Visibility);
        }

        [TestMethod]
        public void WhenEmulationMode__VIGEM_AND_PROCESS_INJECTION__ShouldEnableTheControl()
        {
            ApplicationSettings.GetInstance().EmulationMode = EmulationConstants.VIGEM_AND_PROCESS_INJECTION;

            RemotePlayToolbarSettingsControl classUnderTest = new RemotePlayToolbarSettingsControl();
            classUnderTest.CalculateIsControlEnabled();

            Assert.AreEqual(UIConstants.VISIBILITY_COLLAPSED, classUnderTest.testonly_GetElement_TextBlock_Warning().Visibility);
            Assert.AreEqual(UIConstants.VISIBILITY_VISIBLE, classUnderTest.testonly_GetElement_TextBlock_Description().Visibility);
            Assert.AreEqual(UIConstants.VISIBILITY_VISIBLE, classUnderTest.testonly_GetElement_Toggle().Visibility);
        }

    }
}
