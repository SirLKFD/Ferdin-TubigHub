using System;
using Windows.System;
using Windows.UI.Core;
using Windows.UI.Popups;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;

namespace Ferdin_TB_Hub.Classes
{
    internal class Buttons
    {

        //MESSAGE BOX
        public static async void ShowMessage(string message)
        {
            try
            {
                if (Windows.ApplicationModel.Core.CoreApplication.MainView.CoreWindow != null)
                {
                    await Windows.ApplicationModel.Core.CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, async () =>
                    {
                        MessageDialog dialog = new MessageDialog(message);
                        _ = await dialog.ShowAsync();
                    });
                }
            }
            catch (Exception ex)
            {
                ShowPrompt("An error occurred: " + ex.Message);
            }
        }



        // TOGGLE PASSWORD VIEW
        public static void TogglePasswordReveal(PasswordBox passwordBox, CheckBox revealCheckbox)
        {
            try
            {
                passwordBox.PasswordRevealMode = revealCheckbox.IsChecked == true ? PasswordRevealMode.Visible : PasswordRevealMode.Hidden;
            }
            catch (Exception ex)
            {
                ShowPrompt("An error occurred: " + ex.Message);
            }
        }


        // NUMERIC INPUT HANDLING
        public static void HandleNumericInput(KeyRoutedEventArgs e)
        {
            try
            {
                bool isNumeric = (e.Key >= VirtualKey.Number0 && e.Key <= VirtualKey.Number9) ||
                                 (e.Key >= VirtualKey.NumberPad0 && e.Key <= VirtualKey.NumberPad9) ||
                                 e.Key == VirtualKey.Back || e.Key == VirtualKey.Delete || e.Key == VirtualKey.Left || e.Key == VirtualKey.Right;

                // Set e.Handled to true if the pressed key is not numeric or a control key
                e.Handled = !isNumeric;
            }
            catch (Exception ex)
            {
                ShowPrompt("An error occurred: " + ex.Message);
            }
        }

        // MESSAGE BOX VERSION 2

        public static async void ShowPrompt(string message)
        {
            try
            {
                ContentDialog promptDialog = new ContentDialog
                {
                    Title = "Attention",
                    Content = message,
                    PrimaryButtonText = "OK"
                };

                _ = await promptDialog.ShowAsync();
            }
            catch (Exception ex)
            {
                ShowPrompt("An error occurred: " + ex.Message);
            }
        }

    }
}
