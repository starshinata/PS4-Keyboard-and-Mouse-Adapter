using Pizza.KeyboardAndMouseAdapter.Backend;
using Pizza.KeyboardAndMouseAdapter.Backend.Config;
using Pizza.KeyboardAndMouseAdapter.Backend.Mappings;
using Serilog;
using SFML.Window;
using System;
using System.Linq;
using System.Windows;
using System.Windows.Forms;
using Button = System.Windows.Controls.Button;
using Keyboard = SFML.Window.Keyboard;
using KeyEventArgs = System.Windows.Input.KeyEventArgs;

namespace Pizza.KeyboardAndMouseAdapter.UI.Controls.AdvancedMappingsPage
{

    public partial class EditMappingControl : System.Windows.Controls.UserControl
    {
        private bool IsAwaitingInput = false;

        private Mapping NewMapping;

        public EditMappingControl()
        {
            InitializeComponent();
        }

        private void Handler_AddMapping_GenericKeyDown(ExtraButtons extraValue, Keyboard.Key keyboardValue, MouseButton mouseValue)
        {

            PhysicalKey pk = new PhysicalKey();
            pk.ExtraValue = extraValue;
            pk.KeyboardValue = keyboardValue;
            pk.MouseValue = mouseValue;

            NewMapping.PhysicalKeys.Add(pk);
            RenderRealInput();
        }

        private void Handler_AddMapping_OnKeyboardKeyDown(object sender, KeyEventArgs e)
        {
            if (IsAwaitingInput)
            {
                if (Keyboard.IsKeyPressed(Keyboard.Key.Escape))
                {
                    WaitingForInput_Hide();
                    return;
                }

                var keyboardKeys = Enum.GetValues(typeof(Keyboard.Key)).Cast<Keyboard.Key>();
                foreach (Keyboard.Key key in keyboardKeys)
                {
                    if (key != Keyboard.Key.Escape && Keyboard.IsKeyPressed(key))
                    {
                        Handler_AddMapping_GenericKeyDown(ExtraButtons.Unknown, key, MouseButton.Unknown);
                        return;
                    }
                }

               
            }
        }

        private void Handler_AddMapping_OnMouseDown(object sender, RoutedEventArgs e)
        {
            if (IsAwaitingInput)
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
        }

        private void Handler_AddMapping_OnMouseLeftButtonUp(object sender, RoutedEventArgs e)
        {
            if (IsAwaitingInput)
            {
                Handler_AddMapping_GenericKeyDown(ExtraButtons.Unknown, Keyboard.Key.Unknown, MouseButton.Left);
            }
        }

        private void Handler_AddMapping_OnMouseScroll(object sender, RoutedEventArgs e)
        {
            Log.Debug("Handler_AddMapping_OnMouseScroll");
            Log.Debug(DateTime.Now.ToString());

            if (IsAwaitingInput)
            {
                System.Windows.Input.MouseWheelEventArgs mwea = (System.Windows.Input.MouseWheelEventArgs)e;
                ExtraButtons scrollAction = MouseWheelScrollProcessor.GetScrollAction(mwea);
                Handler_AddMapping_GenericKeyDown(scrollAction, Keyboard.Key.Unknown, MouseButton.Unknown);
            }
        }

        private void Handler_ButtonAddControllerInput(object sender, RoutedEventArgs e)
        {

            ControllerInput controllerInput = new ControllerInput();
            controllerInput.Height = 25;
            controllerInput.MinWidth = 100;
            controllerInput.HorizontalAlignment = UIConstants.ALIGNMENT_HORIZONAL_CENTER;
            controllerInput.VerticalAlignment = UIConstants.ALIGNMENT_VERTICAL_CENTER;

            PanelInputController.Children.Add(controllerInput);

        }

        private void Handler_ButtonAddRealInput(object sender, RoutedEventArgs e)
        {
            WaitingForInput_Show();
        }

        private void Handler_ButtonCancel(object sender, RoutedEventArgs e)
        {
            HideThis();
        }

        private void Handler_ButtonDeleteMapping(object sender, RoutedEventArgs e)
        {
            UserSettingsContainer.DeleteMapping(NewMapping.uid);
            HideThis();
        }

        private void Handler_ButtonRemoveRealInput(object sender, RoutedEventArgs e)
        {
            if (e.Source != null)
            {
                Button button = (Button)e.Source;
                if (button != null && button.Tag != null)
                {
                    PhysicalKey pk = (PhysicalKey)button.Tag;
                    NewMapping.PhysicalKeys.Remove(pk);
                    RenderRealInput();
                }
            }
        }

        private void Handler_ButtonSaveMapping(object sender, RoutedEventArgs e)
        {
            if (IsMappingValid())
            {
                //UserSettingsContainer.SetMapping(vk, valueOld, valueNew);
                HideThis();
            }
            else
            {
                //TODO improve this message

                System.Windows.MessageBox.Show("Mapping is invalid, make sure there are Real Inputs and Controller Inputs!",
                    "error",
                  (MessageBoxButton)MessageBoxButtons.OK,
                  (MessageBoxImage)MessageBoxIcon.Error);
            }
        }

        private void HideThis()
        {
            // var becuase AdvancedMappingsPage is also a namespace
            var page = GetPageReference.AdvancedMappings();
            if (page != null)
            {
                page.EditMapping_Hide();
            }
            else
            {
                Log.Information("EditMappingControl.HideThis() called but AdvancedMappingsPage is null");
            }
        }

        private bool IsMappingValid()
        {
            return (NewMapping != null && NewMapping.PhysicalKeys.Count > 0 && NewMapping.VirtualKeys.Count > 0);
        }

        private void RenderRealInput()
        {
            PanelInputReal.Children.Clear();
            if (NewMapping != null && NewMapping.PhysicalKeys.Count > 0)
            {
                foreach (PhysicalKey pk in NewMapping.PhysicalKeys)
                {
                    Button button = new Button();
                    button.Click += Handler_ButtonRemoveRealInput;
                    button.Content = pk.ToString();
                    button.Tag = pk;
                    button.Height = 30;
                    button.MinWidth = 100;
                    button.HorizontalAlignment = UIConstants.ALIGNMENT_HORIZONAL_CENTER;
                    button.VerticalAlignment = UIConstants.ALIGNMENT_VERTICAL_CENTER;

                    PanelInputReal.Children.Add(button);
                }
            }


            PanelInputController.Children.Clear();
            if (NewMapping != null && NewMapping.VirtualKeys.Count > 0)
            {
                foreach (VirtualKey vk in NewMapping.VirtualKeys)
                {
                    //TODO refactor
                    ControllerInput controllerInput = new ControllerInput();
                    controllerInput.Height = 30;
                    controllerInput.MinWidth = 100;
                    controllerInput.HorizontalAlignment = UIConstants.ALIGNMENT_HORIZONAL_CENTER;
                    controllerInput.VerticalAlignment = UIConstants.ALIGNMENT_VERTICAL_CENTER;
                    controllerInput.SetSelected(vk);

                    PanelInputController.Children.Add(controllerInput);
                }
            }

            WaitingForInput_Hide();
        }

        public void ShowThis(Mapping _mapping)
        {
            if (_mapping != null)
            {
                NewMapping = _mapping.Clone();
            }
            else
            {
                NewMapping = new Mapping();
            }

            WaitingForInput_Hide();
            RenderRealInput();
        }


        private void WaitingForInput_Hide()
        {
            IsAwaitingInput = false;
            PanelWaitingForInput.Visibility = Visibility.Hidden;
            PanelInputReal.Visibility = Visibility.Visible;
        }

        private void WaitingForInput_Show()
        {
            IsAwaitingInput = true;
            PanelWaitingForInput.Visibility = Visibility.Visible;
            PanelInputReal.Visibility = Visibility.Hidden;
        }
    }
}

