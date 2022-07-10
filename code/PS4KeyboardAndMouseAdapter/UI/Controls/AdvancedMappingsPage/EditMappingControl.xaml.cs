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
        private bool IsAcceptingInput = true;

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
        }

        private void Handler_AddMapping_OnKeyboardKeyDown(object sender, KeyEventArgs e)
        {
            if (IsAcceptingInput)
            {
                foreach (Keyboard.Key key in Enum.GetValues(typeof(Keyboard.Key)).Cast<Keyboard.Key>())
                {
                    if (key != Keyboard.Key.Escape && Keyboard.IsKeyPressed(key))

                    {
                        Handler_AddMapping_GenericKeyDown(ExtraButtons.Unknown, key, MouseButton.Unknown);
                    }
                }
            }
        }

        private void Handler_AddMapping_OnMouseDown(object sender, RoutedEventArgs e)
        {
            if (IsAcceptingInput)
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
            if (IsAcceptingInput)
            {
                Handler_AddMapping_GenericKeyDown(ExtraButtons.Unknown, Keyboard.Key.Unknown, MouseButton.Left);
            }
        }

        private void Handler_AddMapping_OnMouseScroll(object sender, RoutedEventArgs e)
        {
            Log.Debug("Handler_AddMapping_OnMouseScroll");
            Log.Debug(DateTime.Now.ToString());

            if (IsAcceptingInput)
            {
                System.Windows.Input.MouseWheelEventArgs mwea = (System.Windows.Input.MouseWheelEventArgs)e;
                ExtraButtons scrollAction = MouseWheelScrollProcessor.GetScrollAction(mwea);
                Handler_AddMapping_GenericKeyDown(scrollAction, Keyboard.Key.Unknown, MouseButton.Unknown);
            }
        }

        private void Handler_ButtonAddControllerInput(object sender, RoutedEventArgs e)
        {

            ControllerInput controllerInput = new ControllerInput();
            controllerInput.Height = 30;
            controllerInput.MinWidth = 100;
            controllerInput.HorizontalAlignment = UIConstants.ALIGNMENT_HORIZONAL_CENTER;
            controllerInput.VerticalAlignment = UIConstants.ALIGNMENT_VERTICAL_CENTER;
            
            PanelInputController.Children.Add(controllerInput);

        }

        private void Handler_ButtonAddRealInput(object sender, RoutedEventArgs e)
        {
            IsAcceptingInput = true;
        }

        private void Handler_ButtonCancel(object sender, RoutedEventArgs e)
        {
            HideThis();
        }

        private void Handler_ButtonDeleteMapping(object sender, RoutedEventArgs e)
        {

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
            System.Windows.Window window = System.Windows.Application.Current.MainWindow;
            if (window != null)
            {
                MainWindowView mainWindowView = ((MainWindowView)window);
                // var becuase AdvancedMappingsPage is also a namespace
                var page = mainWindowView.getPageAdvancedMappings();
                if (page != null)
                {
                    page.EditMapping_Hide();
                }
                else
                {
                    Log.Information("EditMappingControl.HideThis() called but AdvancedMappingsPage is null");
                }
            }
            else
            {
                Log.Information("EditMappingControl.HideThis() called but window is null");
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

            IsAcceptingInput = false;
            RenderRealInput();
        }


    }
}

