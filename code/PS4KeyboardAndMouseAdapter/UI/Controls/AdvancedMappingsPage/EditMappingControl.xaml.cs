using Pizza.KeyboardAndMouseAdapter.Backend;
using Pizza.KeyboardAndMouseAdapter.Backend.Config;
using Pizza.KeyboardAndMouseAdapter.Backend.Mappings;
using Serilog;
using SFML.Window;
using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using Button = System.Windows.Controls.Button;
using Keyboard = SFML.Window.Keyboard;
using KeyEventArgs = System.Windows.Input.KeyEventArgs;

namespace Pizza.KeyboardAndMouseAdapter.UI.Controls.AdvancedMappingsPage
{

    public partial class EditMappingControl : UserControl
    {

        private Button lastClickedButton;
        private readonly double OpacityUnMappedButton = 0.5;
        private readonly UserSettingsContainer UserSettingsContainer;


        public EditMappingControl()
        {
            InitializeComponent();
        }


        private void Handler_AddMapping_GenericKeyDown(ExtraButtons extraValue, Keyboard.Key keyboardValue, MouseButton mouseValue)
        {

            if (lastClickedButton != null && lastClickedButton.Parent != null)
            {

                StackPanel parentStackPanel = (StackPanel)lastClickedButton.Parent;
                if (parentStackPanel.Tag != null)
                {
                    if (keyboardValue != Keyboard.Key.Escape)
                    {
                        VirtualKey vk = (VirtualKey)parentStackPanel.Tag;

                        int index = (int)lastClickedButton.Tag;
                        PhysicalKey valueOld = null;

                        if (UserSettingsContainer.GetInstance().MappingsContainsKey(vk))
                        {
                            if (index < UserSettingsContainer.GetInstance().Mappings[vk].PhysicalKeys.Count)
                            {
                                valueOld = UserSettingsContainer.GetInstance().Mappings[vk].PhysicalKeys[index];
                            }
                        }

                        PhysicalKey valueNew = new PhysicalKey();
                        valueNew.ExtraValue = extraValue;
                        valueNew.KeyboardValue = keyboardValue;
                        valueNew.MouseValue = mouseValue;

                        UserSettingsContainer.SetMapping(vk, valueOld, valueNew);
                    }

                    lastClickedButton = null;
                    ((MainViewModel)DataContext).RefreshData();
                    //WaitingForKeyPress_Hide();
                }
            }
        }

        private void Handler_AddMapping_OnKeyboardKeyDown(object sender, KeyEventArgs e)
        {
            foreach (Keyboard.Key key in Enum.GetValues(typeof(Keyboard.Key)).Cast<Keyboard.Key>())
            {
                if (Keyboard.IsKeyPressed(key))
                {
                    Handler_AddMapping_GenericKeyDown(ExtraButtons.Unknown, key, MouseButton.Unknown);
                }
            }
        }

        private void Handler_AddMapping_OnMouseDown(object sender, RoutedEventArgs e)
        {
            Array mouseButtons = Enum.GetValues(typeof(Mouse.Button));
            foreach (Mouse.Button button in mouseButtons)
            {
                if (Mouse.IsButtonPressed(button))
                {
                    MouseButton mouseButton = (MouseButton)button;
                    Handler_AddMapping_GenericKeyDown(ExtraButtons.Unknown, Keyboard.Key.Unknown, mouseButton);
                }
            }
        }

        private void Handler_AddMapping_OnMouseLeftButtonUp(object sender, RoutedEventArgs e)
        {
            Handler_AddMapping_GenericKeyDown(ExtraButtons.Unknown, Keyboard.Key.Unknown, MouseButton.Left);
        }

        private void Handler_AddMapping_OnMouseScroll(object sender, RoutedEventArgs e)
        {
            Log.Debug("Handler_AddMapping_OnMouseScroll");
            Log.Debug(DateTime.Now.ToString());

            System.Windows.Input.MouseWheelEventArgs mwea = (System.Windows.Input.MouseWheelEventArgs)e;
            ExtraButtons scrollAction = MouseWheelScrollProcessor.GetScrollAction(mwea);
            Handler_AddMapping_GenericKeyDown(scrollAction, Keyboard.Key.Unknown, MouseButton.Unknown);
        }

        private void Handler_ButtonClicked(object sender, RoutedEventArgs e)
        {
            Button button = (Button)sender;
            //WaitingForKeyPress_Show(button);
        }

        private void Handler_Loaded(object sender, RoutedEventArgs e)
        {
            Log.Debug("AdvancedMappingsPage.LOADED");
            //RefreshButtonContents();
        }

    }
}
