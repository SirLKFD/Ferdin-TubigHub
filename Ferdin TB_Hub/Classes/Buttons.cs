using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.System;
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
            var dialog = new MessageDialog(message);
            await dialog.ShowAsync();
        }


        // TOGGLE PASSWORD VIEW
        public static void TogglePasswordReveal(PasswordBox passwordBox, CheckBox revealCheckbox)
        {
            if (revealCheckbox.IsChecked == true)
            {
                passwordBox.PasswordRevealMode = PasswordRevealMode.Visible;
            }
            else
            {
                passwordBox.PasswordRevealMode = PasswordRevealMode.Hidden;
            }
        }

        // NUMERIC INPUT HANDLING
        public static void HandleNumericInput(KeyRoutedEventArgs e)
        {
            bool isNumeric = (e.Key >= VirtualKey.Number0 && e.Key <= VirtualKey.Number9) ||
                             (e.Key >= VirtualKey.NumberPad0 && e.Key <= VirtualKey.NumberPad9) ||
                             e.Key == VirtualKey.Back || e.Key == VirtualKey.Delete || e.Key == VirtualKey.Left || e.Key == VirtualKey.Right;

            // Set e.Handled to true if the pressed key is not numeric or a control key
            e.Handled = !isNumeric;
        }

    }
}
