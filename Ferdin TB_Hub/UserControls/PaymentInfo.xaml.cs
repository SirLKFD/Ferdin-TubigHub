using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The User Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234236

namespace Ferdin_TB_Hub.UserControls
{
    public sealed partial class PaymentInfo : UserControl
    {
        public PaymentInfo()
        {
            this.InitializeComponent();
        }

        private void cbxBuyerPayment_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            string selectedPayment = ((ComboBoxItem)cbxBuyerPayment.SelectedItem)?.Content.ToString();

            // Hide/show controls based on the selected payment
            switch (selectedPayment)
            {
                case "GCash":
                    gcashTextBlock.Visibility = Visibility.Visible;
                    tbxBuyerGcash.Visibility = Visibility.Visible;
                    cardInfoTextBlock.Visibility = Visibility.Collapsed;
                    tbxBuyerCard.Visibility = Visibility.Collapsed;
                    break;

                case "Credit/Debit Card":
                    gcashTextBlock.Visibility = Visibility.Collapsed;
                    tbxBuyerGcash.Visibility = Visibility.Collapsed;
                    cardInfoTextBlock.Visibility = Visibility.Visible;
                    tbxBuyerCard.Visibility = Visibility.Visible;
                    break;

                case "Cash on Delivery":
                    gcashTextBlock.Visibility = Visibility.Collapsed;
                    tbxBuyerGcash.Visibility = Visibility.Collapsed;
                    cardInfoTextBlock.Visibility = Visibility.Collapsed;
                    tbxBuyerCard.Visibility = Visibility.Collapsed;
                    break;

                default:
                    // Handle other cases if needed
                    break;
            }
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            // Initial setup - Hide controls since payment info is blank
            gcashTextBlock.Visibility = Visibility.Collapsed;
            tbxBuyerGcash.Visibility = Visibility.Collapsed;
            cardInfoTextBlock.Visibility = Visibility.Collapsed;
            tbxBuyerCard.Visibility = Visibility.Collapsed;
        }

        private void gcash_NUMERIC(object sender, KeyRoutedEventArgs e)
        {
            // Check if the pressed key is a numeric key (0-9) or a control key
            bool isNumeric = (e.Key >= VirtualKey.Number0 && e.Key <= VirtualKey.Number9) ||
                             (e.Key >= VirtualKey.NumberPad0 && e.Key <= VirtualKey.NumberPad9) ||
                             e.Key == VirtualKey.Back || e.Key == VirtualKey.Delete || e.Key == VirtualKey.Left || e.Key == VirtualKey.Right;

            // Set e.Handled to true if the pressed key is not numeric or a control key
            e.Handled = !isNumeric;
        }

        private void card_NUMERIC(object sender, KeyRoutedEventArgs e)
        {
            // Check if the pressed key is a numeric key (0-9) or a control key
            bool isNumeric = (e.Key >= VirtualKey.Number0 && e.Key <= VirtualKey.Number9) ||
                             (e.Key >= VirtualKey.NumberPad0 && e.Key <= VirtualKey.NumberPad9) ||
                             e.Key == VirtualKey.Back || e.Key == VirtualKey.Delete || e.Key == VirtualKey.Left || e.Key == VirtualKey.Right;

            // Set e.Handled to true if the pressed key is not numeric or a control key
            e.Handled = !isNumeric;
        }

    }
}
