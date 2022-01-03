using Pizza.backend.vigem;
using Pizza.Common;
using PS4KeyboardAndMouseAdapter.Config;
using PS4KeyboardAndMouseAdapter.UI.Pages;
using System;
using System.Windows;
using System.Windows.Controls;
using KeyEventArgs = System.Windows.Input.KeyEventArgs;

namespace PS4KeyboardAndMouseAdapter
{
    public partial class MainWindowView : Window
    {
        private SimpleConfigPage simpleConfigPage;
        private AdvancedMappingsPage advancedMappingsPage;

        public MainWindowView()
        {
            InitializeComponent();
            KeyDown += MainWindowView_OnKeyDown;
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

            simpleConfigPage = new SimpleConfigPage();
            advancedMappingsPage = new AdvancedMappingsPage();
            MouseAdvancedConfigPage mouseAdvancedConfigPage = new MouseAdvancedConfigPage();
            MiscellaneousSettingsPage miscellaneousSettingsPage = new MiscellaneousSettingsPage();

            AddTab("Simple Config", simpleConfigPage);
            AddTab("Advanced mappings", advancedMappingsPage);
            AddTab("Mouse Advanced Config", mouseAdvancedConfigPage);
            AddTab("Miscellaneous Settings", miscellaneousSettingsPage);

            // remove the welcome tab
            tabs.Items.RemoveAt(0);

            // Refresh to ensure advancedMappingsPage isnt blank
            advancedMappingsPage.RefreshButtonContents();
        }

    }
}
