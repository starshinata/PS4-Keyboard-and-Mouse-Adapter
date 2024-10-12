using AdonisUI;
using Pizza.Common;
using Pizza.KeyboardAndMouseAdapter.Backend;
using Pizza.KeyboardAndMouseAdapter.Backend.Config;
using Pizza.KeyboardAndMouseAdapter.Backend.Vigem;
using Pizza.KeyboardAndMouseAdapter.UI.Pages;
using System;
using System.Windows;
using System.Windows.Controls;
using KeyEventArgs = System.Windows.Input.KeyEventArgs;

namespace Pizza.KeyboardAndMouseAdapter.UI
{
    public partial class MainWindowView : Window
    {
        private SimpleConfigPage simpleConfigPage;
        private AdvancedMappingsPage advancedMappingsPage;
        private OnScreenControllerPage onScreenControllerPage;

        public MainWindowView()
        {
            InitializeComponent();
            KeyDown += MainWindowView_OnKeyDown;
            ChangeColourScheme(ApplicationSettings.GetInstance().ColourSchemeIsLight);
            WelcomeStep3Done_ConnectAdapter();
        }

        private void AddTab(string tabText, UserControl control)
        {
            TextBlock textBlock = new TextBlock();
            textBlock.Text = tabText;
            textBlock.Width = 150;

            TabItem tabItem = new TabItem();
            tabItem.Content = control;
            tabItem.Header = tabText;

            tabs.Items.Add(tabItem);
        }

        public void ChangeColourScheme(bool isLight)
        {
            System.Uri colourScheme = isLight ? ResourceLocator.LightColorScheme : ResourceLocator.DarkColorScheme;
            ResourceLocator.SetColorScheme(Application.Current.Resources, colourScheme);

            if (onScreenControllerPage != null)
            {
                onScreenControllerPage.ChangeScheme(colourScheme);
            }

            if (simpleConfigPage != null)
            {
                simpleConfigPage.ChangeScheme(colourScheme);
            }
        }

        private void MainWindowView_OnActivated(object sender, EventArgs e)
        {
            RefreshRemotePlayProcess();
        }

        private void MainWindowView_OnDeactivated(object sender, EventArgs e)
        {
            RefreshRemotePlayProcess();
        }

        public void MainWindowView_OnKeyDown(object sender, KeyEventArgs e)
        {
            if (simpleConfigPage != null)
            {
                simpleConfigPage.gamepadMappingController.Handler_OnKeyDown(sender, e);
            }
        }

        private void MainWindowView_Resized(object sender, EventArgs e)
        {
            if (onScreenControllerPage != null)
            {
                onScreenControllerPage.WindowResized();
            }
        }

        private void RefreshRemotePlayProcess()
        {
            if (InstanceSettings.GetInstance().GetRemotePlayProcess() != null)
            {
                InstanceSettings.GetInstance().GetRemotePlayProcess().Refresh();
            }
        }

        public void WelcomeStep1Done_SetupChecked()
        {
            pageWelcomePage.CompleteStep1();
        }

        public void WelcomeStep2Done_RemotePlayStarted()
        {
            pageWelcomePage.CompleteStep2();
        }

        public void WelcomeStep3Done_ConnectAdapter()
        {
            if (ApplicationSettings.GetInstance().EmulationMode == EmulationConstants.ONLY_VIGEM ||
                ApplicationSettings.GetInstance().EmulationMode == EmulationConstants.VIGEM_AND_PROCESS_INJECTION)
            {
                VigemManager.Start();
            }

            RefreshRemotePlayProcess();

            ApplicationSettings.Save();
            InstanceSettings.GetInstance().EnableMouseInput = true;

            advancedMappingsPage = new AdvancedMappingsPage();
            onScreenControllerPage = new OnScreenControllerPage();
            simpleConfigPage = new SimpleConfigPage();

            MouseAdvancedConfigPage mouseAdvancedConfigPage = new MouseAdvancedConfigPage();
            MiscellaneousSettingsPage miscellaneousSettingsPage = new MiscellaneousSettingsPage();

            //TODO dont leave this as the FIRST TAB
            AddTab("On Screen Controller", onScreenControllerPage);

            AddTab("Simple Config", simpleConfigPage);
            AddTab("Advanced mappings", advancedMappingsPage);
            AddTab("Mouse Advanced Config", mouseAdvancedConfigPage);
            AddTab("Miscellaneous Settings", miscellaneousSettingsPage);
            //TODO
            //AddTab("On Screen Controller", onScreenControllerPage);

            // remove the welcome tab
            tabs.Items.RemoveAt(0);

            // Refresh to ensure advancedMappingsPage isnt blank
            advancedMappingsPage.RefreshButtonContents();
        }

    }
}
