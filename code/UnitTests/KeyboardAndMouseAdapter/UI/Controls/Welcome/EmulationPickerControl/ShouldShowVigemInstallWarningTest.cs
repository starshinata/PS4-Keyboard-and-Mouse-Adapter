using Autofac.Extras.Moq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Pizza.KeyboardAndMouseAdapter.Backend.Vigem;
using Pizza.KeyboardAndMouseAdapter.UI.Controls.Welcome;

namespace UnitTests.KeyboardAndMouseAdapter.UI.Controls.Welcome.EmulationPickerControlTest
{

    [TestClass]
    public class ShouldShowVigemInstallWarningTest
    {

        [TestMethod]
        public void WhenInstalled_ShouldSet__Panel_VigemNotInstalled__toBeCollapsed()
        {
            using (AutoMock mock = AutoMock.GetLoose())
            {
                Mock<VigemManager> mocky = mock.Mock<VigemManager>();
                mocky.Setup(x => x.IsVigemDriverInstalled())
                    .Returns(true);

                EmulationPickerControl classUnderTest = new EmulationPickerControl();
                classUnderTest.ShouldShowVigemInstallWarning(mocky.Object);

                bool actual = classUnderTest.testonly_isVisible_Panel_VigemNotInstalled();
                bool expected = false;
                Assert.AreEqual(expected, actual);
            }
        }

        [TestMethod]
        public void WhenInstalled_ShouldSet__Panel_VigemNotInstalled__toBeVisible()
        {
            using (AutoMock mock = AutoMock.GetLoose())
            {
                Mock<VigemManager> mocky = mock.Mock<VigemManager>();
                mocky.Setup(x => x.IsVigemDriverInstalled())
                    .Returns(false);

                EmulationPickerControl classUnderTest = new EmulationPickerControl();
                classUnderTest.ShouldShowVigemInstallWarning(mocky.Object);

                bool actual = classUnderTest.testonly_isVisible_Panel_VigemNotInstalled();
                bool expected = true;
                Assert.AreEqual(expected, actual);
            }
        }

    }
}
