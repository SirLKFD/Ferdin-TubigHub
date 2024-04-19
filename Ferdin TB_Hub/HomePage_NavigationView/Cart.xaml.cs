using Ferdin_TB_Hub.Classes;
using Microsoft.Toolkit.Uwp.Notifications;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Networking;
using Windows.UI.Notifications;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.Data.Xml.Dom; 
using Windows.UI.Xaml.Navigation;
using static Ferdin_TB_Hub.Classes.Database;


// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace Ferdin_TB_Hub.HomePage_NavigationView
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class Cart : Page, INotifyPropertyChanged
    {
        private BuyerDetails _buyer;
        public List<ProductCart> ProductCartList { get; set; }


        public BuyerDetails Buyer
        {
            get { return _buyer; }
            set
            {
                if (_buyer != value)
                {
                    _buyer = value;
                    OnPropertyChanged(nameof(Buyer));
                }
            }
        }

        public Cart()
        {
            this.InitializeComponent();
            this.DataContext = this; // Set the DataContext of the page to itself
            LoadCartItems();

            // Subscribe to TextChanged events
            tbxGcash.TextChanged += tbxGcash_TextChanged;
            tbxCard.TextChanged += tbxCard_TextChanged;

            // Initially update the Pay button state
            UpdatePayButtonState();

        }

        private void LoadCartItems()
        {


            // Retrieve the list of products in the cart from the database
            ProductCartList = Database.GetProductCart();
      

            // Calculate total price
            double totalPrice = 0;
            foreach (var product in ProductCartList)
            {
                totalPrice += product.ProductPrice;
            }

            // Add tax (assuming tax is 12%)
            double tax = totalPrice * 0.12;
            double totalPriceWithTax = totalPrice + tax;

            // Add shipping fee
            double shippingFee = 50;

            // Update the textboxes
            tbxPrice.Text = (totalPriceWithTax + shippingFee).ToString("Php 0.00"); // Including shipping fee
            tbxQuantity.Text = ProductCartList.Count.ToString(); // Total quantity is the count of items in the cart
            tbxTax.Text = tax.ToString("Php 0.00");
            tbxShippingFee.Text = shippingFee.ToString("Php 0.00"); // Display shipping fee

            // Update the ListView
            ListViewCart.ItemsSource = ProductCartList;
        }

        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }


        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            // Check if a BuyerDetails object was passed as a parameter
            if (e.Parameter != null && e.Parameter is BuyerDetails)
            {
                // Cast the parameter to BuyerDetails and assign it to the Buyer property
                Buyer = e.Parameter as BuyerDetails;
                // Notify the UI that the Buyer property has changed
                OnPropertyChanged(nameof(Buyer));
            }

            // Check if a product was passed as a parameter
            if (e.Parameter != null && e.Parameter is ProductDetails)
            {
                // Retrieve the selected product
                var selectedProduct = e.Parameter as ProductDetails;

                // Add the selected product to the cart's list view
                ListViewCart.Items.Add(selectedProduct);
            }
        }

        private void cbxBuyerPayment_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            string selectedPayment = ((ComboBoxItem)cbxBuyerPayment.SelectedItem)?.Content.ToString();

            // Hide/show controls based on the selected payment
            switch (selectedPayment)
            {
                case "GCash":
                    lblGcash.Visibility = Visibility.Visible;
                    tbxGcash.Visibility = Visibility.Visible;
                    lblCard.Visibility = Visibility.Collapsed;
                    tbxCard.Visibility = Visibility.Collapsed;
                    break;

                case "Credit/Debit Card":
                    lblGcash.Visibility = Visibility.Collapsed;
                    tbxGcash.Visibility = Visibility.Collapsed;
                    lblCard.Visibility = Visibility.Visible;
                    tbxCard.Visibility = Visibility.Visible;
                    break;

                case "Cash on Delivery":
                    lblGcash.Visibility = Visibility.Collapsed;
                    tbxGcash.Visibility = Visibility.Collapsed;
                    lblCard.Visibility = Visibility.Collapsed;
                    tbxCard.Visibility = Visibility.Collapsed;
                     btnPay.IsEnabled = true; // Enable Pay button
            break;

                default:
                    // Handle other cases if needed
                    break;
            }
        }



        private void gcash_NUMERIC(object sender, KeyRoutedEventArgs e)
        {
            // Check if the pressed key is a numeric key (0-9) or a control key
            Buttons.HandleNumericInput(e);
        }

        private void card_NUMERIC(object sender, KeyRoutedEventArgs e)
        {
            // Check if the pressed key is a numeric key (0-9) or a control key
            Buttons.HandleNumericInput(e);
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            // Initial setup - Hide controls since payment info is blank
            lblGcash.Visibility = Visibility.Collapsed;
            tbxGcash.Visibility = Visibility.Collapsed;
            lblCard.Visibility = Visibility.Collapsed;
            tbxCard.Visibility = Visibility.Collapsed;
        }



        private void btnPay_Click(object sender, RoutedEventArgs e)
        {
            // Get the values from the TextBoxes
            string firstName = tbxFirstName.Text;
            string middleName = tbxMiddleName.Text; // Assuming you have a TextBox for middle name
            string lastName = tbxLastName.Text;
            string phoneNumber = tbxPhoneNumber.Text;
            string addressLine1 = tbxAddressLine1.Text;
            string addressLine2 = tbxAddressLine2.Text;
            string email = tbxEmail.Text;
            string passbuyerID = tbxBuyerID.Text;

            // Check if any of the required fields are empty
            if (string.IsNullOrWhiteSpace(firstName) || string.IsNullOrWhiteSpace(lastName) ||
                string.IsNullOrWhiteSpace(addressLine1) || string.IsNullOrWhiteSpace(email))
            {
                // Notify the user to fill in all required buyer details
                ShowErrorPrompt("Please fill in all required buyer details.");
                return;
            }

            // Check if there are items in the cart
            if (ProductCartList == null || ProductCartList.Count == 0)
            {
                // Notify the user that the cart is empty
                ShowErrorPrompt("Your cart is empty. Please add items before proceeding to payment.");
                return;
            }

            // Get the selected payment method from the ComboBox
            string paymentMethod = ((ComboBoxItem)cbxBuyerPayment.SelectedItem)?.Content?.ToString();


            // Get the current date and time as the date of purchase
            DateTime datePurchased = DateTime.Now;

            // Retrieve the buyer's ID using the DatabaseAccess class
            DatabaseAccess dbAccess = new DatabaseAccess();
            var product = DataContext as ProductDetails;
            var seller = DataContext as SellerDetails;


            dbAccess.RetrieveBuyerIDFromDatabase(passbuyerID); // Assuming email is used to uniquely identify the buyer
          // dbAccess.RetrieveSellerIDFromDatabase(firstName); // Assuming email is used to uniquely identify the seller

            // Get the retrieved buyer ID from the BuyerAndSellerID class
            int buyerID = dbAccess.RetrieveBuyerIDFromDatabase(passbuyerID);
            //  int sellerID = BuyerAndSellerID.SellerID;

            // Prepare to pass the order summary and buyer's information to the receipt

            // Loop through the items in the cart and pass each item to the receipt
            foreach (var productCart in ProductCartList)
            {
                // Retrieve the corresponding product details from the database based on the product name
                var productDetails = Database.GetAllProductDetails().FirstOrDefault(p => p.ProductName == productCart.ProductName);

                // Check if the product details are not null
                if (productDetails != null)
                {
                    // Pass each item to the receipt with correct product category and buyer ID
                    PassProductToReceipt(productCart, lastName, firstName, middleName, phoneNumber, productDetails.ProductCategory, addressLine1, addressLine2, email, paymentMethod, datePurchased, buyerID);
                }
            }

            ShowOrderPlacedNotification();

            // Generate receipt content
            string receiptContent = GenerateReceiptContent();

            // Show receipt using prompt
            ShowReceiptPrompt(receiptContent);
        }


        public static void ShowOrderPlacedNotification()
        {
            // Construct the toast content
            var content = new ToastContentBuilder()
                .AddText("Order Placed")
                .AddText("Your order has been successfully placed!")
                .AddText("Thank you for shopping with us!\n A receipt will be shown shortly.")
                .GetToastContent();

            // Create the toast notification
            var toast = new ToastNotification(content.GetXml());

            // Show the toast notification
            ToastNotificationManager.CreateToastNotifier().Show(toast);
        }


        private void RemoveButton_Click(object sender, RoutedEventArgs e)
        {
            // Retrieve the button that raised the event
            Button button = (Button)sender;

            // Retrieve the DataContext of the button which should be the ProductCart object
            ProductCart selectedProduct = (ProductCart)button.DataContext;

            // Retrieve the product name and quantity of the selected product
            string productName = selectedProduct.ProductName;
            int quantity = selectedProduct.ProductQuantity;

            // Delete the product from the cart
            Database.DeleteProductFromCart(selectedProduct.ProductCart_ID);

            // Restore the product quantity in the database

            int sellerID = BuyerAndSellerID.SellerID;

            Database.RestoreProductQuantity(productName, quantity + 1);

            // Refresh the cart items
            LoadCartItems();



        }

        private string GenerateReceiptContent()
        {
            // Calculate total price
            double totalPrice = 0;
            foreach (var product in ProductCartList)
            {
                totalPrice += product.ProductPrice;
            }

            // Add tax (assuming tax is 12%)
            double tax = totalPrice * 0.12;
            double totalPriceWithTax = totalPrice + tax;

            // Add shipping fee
            double shippingFee = 50;

            // Prepare receipt content
            string receiptContent = "Order Summary:\n\n";

            // Loop through the items in the cart and add them to the receipt
            foreach (var productCart in ProductCartList)
            {
                receiptContent += $"{productCart.ProductName}: ₱{productCart.ProductPrice:n2}\n"; // Use ₱ for Philippine Peso sign
            }


            // Add total price, tax, and shipping fee to the receipt
            receiptContent += $"\nTotal Price: ₱{totalPriceWithTax + shippingFee:n2}\n";
            receiptContent += $"Total Quantity: {ProductCartList.Count}\n";
            receiptContent += $"VAT Tax: ₱{tax:n2}\n";
            receiptContent += $"Shipping Fee: ₱{shippingFee:n2}\n";

            // Add buyer information to the receipt
            receiptContent += $"\nBuyer Information:\n";
            receiptContent += $"Name: {tbxFirstName.Text} {tbxMiddleName.Text} {tbxLastName.Text}\n";
            receiptContent += $"Phone Number: {tbxPhoneNumber.Text}\n";
            receiptContent += $"Address: {tbxAddressLine1.Text}, {tbxAddressLine2.Text}\n";
            receiptContent += $"Email: {tbxEmail.Text}\n";

            // Add payment method to the receipt
            receiptContent += $"\nPayment Method: {((ComboBoxItem)cbxBuyerPayment.SelectedItem)?.Content}\n";

            receiptContent += "\nThank you for choosing Ferdin TB Hub!\n\nPlease screenshot this receipt for your reference.";

            // Return the generated receipt content
            return receiptContent;
        }


        private async void ShowErrorPrompt(string errorMessage)
        {
            // Show an error prompt
            ContentDialog errorDialog = new ContentDialog
            {
                Title = "Error",
                Content = errorMessage,
                CloseButtonText = "OK"
            };

            // Show the dialog
            await errorDialog.ShowAsync();
        }




        private async void ShowReceiptPrompt(string receiptContent)
        {
            // Show receipt using prompt
            ContentDialog receiptDialog = new ContentDialog
            {
                Title = "Order Receipt",
                Content = receiptContent,
                CloseButtonText = "Close"
            };

            // Handle the Closed event to clear the ProductCartList after the dialog is closed
            receiptDialog.Closed += ReceiptDialog_Closed;

            // Show the dialog
            ContentDialogResult result = await receiptDialog.ShowAsync();
        }

        private void ReceiptDialog_Closed(ContentDialog sender, ContentDialogClosedEventArgs args)
        {
            // Clear the ProductCartList after the receipt dialog is closed
            ProductCartList.Clear();
            LoadCartItems(); // Refresh the cart UI
        }

        private void AutoSuggestBox_TextChanged(AutoSuggestBox sender, AutoSuggestBoxTextChangedEventArgs args)
        {
            // If the user is typing
            if (args.Reason == AutoSuggestionBoxTextChangeReason.UserInput)
            {
                string userInput = sender.Text.ToLower(); // Get the user input and convert it to lowercase

                // Filter the ProductCartList based on the user input
                var filteredList = ProductCartList.Where(item => item.ProductName.ToLower().Contains(userInput)).ToList();

                // Assign the filtered list to the ItemsSource of the ListView
                ListViewCart.ItemsSource = filteredList;

                // If there's no user input, display all items
                if (string.IsNullOrWhiteSpace(userInput))
                {
                    ListViewCart.ItemsSource = ProductCartList;
                }
            }
        }

        private void tbxGcash_TextChanged(object sender, TextChangedEventArgs e)
        {
            UpdatePayButtonState();
        }

        private void tbxCard_TextChanged(object sender, TextChangedEventArgs e)
        {
            UpdatePayButtonState();
        }

        private void UpdatePayButtonState()
        {
            // Check if GCash and Debit Card text boxes are not empty
            bool gcashFilled = !string.IsNullOrWhiteSpace(tbxGcash.Text);
            bool cardFilled = !string.IsNullOrWhiteSpace(tbxCard.Text);

            // Enable the Pay button if both text boxes are filled, otherwise disable it
            btnPay.IsEnabled = gcashFilled || cardFilled;
        }
    }
}
