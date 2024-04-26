using Ferdin_TB_Hub.Classes;
using Microsoft.Toolkit.Uwp.Notifications;
using PdfSharpCore.Drawing;
using PdfSharpCore.Pdf;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using Windows.Storage;
using Windows.Storage.Provider;
using Windows.UI.Notifications;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
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
            get => _buyer;
            set
            {
                if (_buyer != value)
                {
                    _buyer = value;
                    OnPropertyChanged(nameof(Buyer));
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }


        public Cart()
        {
            try
            {
                InitializeComponent();
                DataContext = this; // Set the DataContext of the page to itself
                LoadCartItems();

                // Subscribe to TextChanged events
                tbxGcash.TextChanged += tbxGcash_TextChanged;
                tbxCard.TextChanged += tbxCard_TextChanged;

                // Initially update the Pay button state
                UpdatePayButtonState();
            }
            catch (Exception ex)
            {
                Buttons.ShowPrompt(ex.Message);
            }



        }

        private void LoadCartItems()
        {
            try
            {
                // Retrieve the list of products in the cart from the database
                ProductCartList = Database.GetProductCart();


                // Calculate total price
                double totalPrice = 0;
                foreach (ProductCart product in ProductCartList)
                {
                    totalPrice += product.ProductPrice;
                }

                // Add tax (assuming tax is 12%)
                double tax = totalPrice * 0.12;
                double totalPriceWithTax = totalPrice + tax;

                // Add shipping fee
                double shippingFee = 50;

                // Update the textboxes
                tbxPrice.Text = (totalPriceWithTax + shippingFee).ToString("₱ 0.00"); // Including shipping fee
                tbxQuantity.Text = ProductCartList.Count.ToString(); // Total quantity is the count of items in the cart
                tbxTax.Text = tax.ToString("₱ 0.00");
                tbxShippingFee.Text = shippingFee.ToString("₱ 0.00"); // Display shipping fee

                // Update the ListView
                ListViewCart.ItemsSource = ProductCartList;
            }
            catch (Exception ex)
            {
                Buttons.ShowPrompt(ex.Message);
            }



        }



        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            try
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
                    ProductDetails selectedProduct = e.Parameter as ProductDetails;

                    // Add the selected product to the cart's list view
                    ListViewCart.Items.Add(selectedProduct);
                }
            }
            catch (Exception ex)
            {
                Buttons.ShowPrompt(ex.Message);
            }

        }

        private void cbxBuyerPayment_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
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
            catch (Exception ex)
            {
                Buttons.ShowPrompt(ex.Message);
            }

        }



        private void gcash_NUMERIC(object sender, KeyRoutedEventArgs e)
        {
            try
            {
                // Check if the pressed key is a numeric key (0-9) or a control key
                Buttons.HandleNumericInput(e);
            }
            catch (Exception ex)
            {
                Buttons.ShowPrompt(ex.Message);
            }

        }

        private void card_NUMERIC(object sender, KeyRoutedEventArgs e)
        {
            try
            {
                // Check if the pressed key is a numeric key (0-9) or a control key
                Buttons.HandleNumericInput(e);
            }
            catch (Exception ex)
            {
                Buttons.ShowPrompt(ex.Message);
            }

        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                // Initial setup - Hide controls since payment info is blank
                lblGcash.Visibility = Visibility.Collapsed;
                tbxGcash.Visibility = Visibility.Collapsed;
                lblCard.Visibility = Visibility.Collapsed;
                tbxCard.Visibility = Visibility.Collapsed;
            }
            catch (Exception ex)
            {
                Buttons.ShowPrompt(ex.Message);
            }

        }



        private void btnPay_Click(object sender, RoutedEventArgs e)
        {
            try
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

                if (int.TryParse(passbuyerID, out int buyerID))
                {
                    // Conversion successful, 'buyerID' contains the integer value
                    // Now you can use 'buyerID' as an integer
                }
                else
                {
                    // Conversion failed, handle the case where the input is not a valid integer
                }

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
                ProductDetails product = DataContext as ProductDetails;
                SellerDetails seller = DataContext as SellerDetails;


                _ = dbAccess.RetrieveBuyerIDFromDatabase(passbuyerID);
                // dbAccess.RetrieveSellerIDFromDatabase(firstName); 

                // Get the retrieved buyer ID from the BuyerAndSellerID class
                // int buyerID = dbAccess.RetrieveBuyerIDFromDatabase(passbuyerID);
                //  int sellerID = BuyerAndSellerID.SellerID;

                // Prepare to pass the order summary and buyer's information to the receipt

                // Loop through the items in the cart and pass each item to the receipt
                foreach (ProductCart productCart in ProductCartList)
                {
                    // Retrieve the corresponding product details from the database based on the product name
                    ProductDetails productDetails = Database.GetAllProductDetails().FirstOrDefault(p => p.ProductName == productCart.ProductName);

                    // Check if the product details are not null
                    if (productDetails != null)
                    {
                        // Pass each item to the receipt with correct product category and buyer ID
                        PassProductToReceipt(productCart, lastName, firstName, middleName, phoneNumber, productDetails.ProductCategory, addressLine1, addressLine2, email, paymentMethod, datePurchased, Buyer.BUYER_ID, productDetails.Seller_ID);
                    }
                }

                ShowOrderPlacedNotification();

                // Generate receipt content
                string receiptContent = GenerateReceiptContent();

                // Show receipt using prompt
                ShowReceiptPrompt(receiptContent);
            }
            catch (Exception ex)
            {
                Buttons.ShowPrompt(ex.Message);
            }

        }


        public static void ShowOrderPlacedNotification()
        {
            try
            {
                // Construct the toast content
                ToastContent content = new ToastContentBuilder()
                    .AddText("Order Placed")
                    .AddText("Your order has been successfully placed!")
                    .AddText("Thank you for shopping with us!\n A receipt will be generated shortly.")
                    .GetToastContent();

                // Create the toast notification
                ToastNotification toast = new ToastNotification(content.GetXml());

                // Show the toast notification
                ToastNotificationManager.CreateToastNotifier().Show(toast);
            }
            catch (Exception ex)
            {
                Buttons.ShowPrompt(ex.Message);
            }

        }


        private void RemoveButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // Retrieve the button that raised the event
                Button button = (Button)sender;

                // Retrieve the DataContext of the button which should be the ProductCart object
                ProductCart selectedProduct = (ProductCart)button.DataContext;

                // Retrieve the product name and quantity of the selected product
                string productName = selectedProduct.ProductName;
                //  long productSKU = selectedProduct.ProductSKU;
                int quantity = selectedProduct.ProductQuantity;

                // Delete the product from the cart
                Database.DeleteProductFromCart(selectedProduct.ProductCart_ID);

                // Restore the product quantity in the database

                int sellerID = BuyerAndSellerID.SellerID;

                Database.RestoreProductQuantityByProductName(selectedProduct.ProductName, selectedProduct.ProductQuantity + 1);

                // Refresh the cart items
                LoadCartItems();
            }
            catch (Exception ex)
            {
                Buttons.ShowPrompt(ex.Message);
            }

        }

        private string GenerateReceiptContent()
        {
            // Calculate total price
            double totalPrice = 0;
            foreach (ProductCart product in ProductCartList)
            {
                totalPrice += product.ProductPrice;
            }

            // Add tax (assuming tax is 12%)
            double tax = totalPrice * 0.12;
            double totalPriceWithTax = totalPrice + tax;

            // Add shipping fee
            double shippingFee = 50;
            // Prepare receipt content
            string receiptContent = "ORDER RECEIPT\n";

            // Add buyer information to the receipt
            receiptContent += $"\n\n\nVAT REG. TIN #. 032-143-323 469\n";
            receiptContent += $"Serial # DF43J1232Z\n";
            receiptContent += $"Permit # 0234-458-76436-123\n";
            receiptContent += $"\nDate Generated: {DateTime.Now:yyyy-MM-dd HH:mm:ss}\n\n";




            // Loop through the items in the cart and add them to the receipt
            foreach (ProductCart productCart in ProductCartList)
            {
                receiptContent += $"{productCart.ProductName}: ₱{productCart.ProductPrice:n2}\n";
            }

            receiptContent += $"\nBUYER INFO\n";
            receiptContent += $"NAME: {tbxFirstName.Text} {tbxMiddleName.Text} {tbxLastName.Text}\n";
            receiptContent += $"PHONE NUMBER: {tbxPhoneNumber.Text}\n";
            receiptContent += $"ADDRESS: {tbxAddressLine1.Text}, {tbxAddressLine2.Text}\n";
            receiptContent += $"EMAIL: {tbxEmail.Text}\n";

            // Add payment method to the receipt
            receiptContent += $"\nPAYMENT METHOD: {((ComboBoxItem)cbxBuyerPayment.SelectedItem)?.Content}\n";

            // Add total price, tax, and shipping fee to the receipt
            receiptContent += $"TOTAL QUANTITY: {ProductCartList.Count}\n";
            receiptContent += $"VAT: ₱{tax:n2}\n";
            receiptContent += $"SHIPPING FEE: ₱{shippingFee:n2}\n";
            receiptContent += $"\nTOTAL PRICE: ₱{totalPriceWithTax + shippingFee:n2}\n";

            // Return the generated receipt content
            return receiptContent;
        }


        private async void ShowErrorPrompt(string errorMessage)
        {
            try
            {
                // Show an error prompt
                ContentDialog errorDialog = new ContentDialog
                {
                    Title = "Error",
                    Content = errorMessage,
                    CloseButtonText = "OK"
                };

                // Show the dialog
                _ = await errorDialog.ShowAsync();
            }
            catch (Exception ex)
            {
                Buttons.ShowPrompt(ex.Message);
            }

        }


        private async void ShowReceiptPrompt(string receiptContent)
        {
            try
            {
                // Create a TextBlock to contain the receipt content
                TextBlock textBlock = new TextBlock
                {
                    Text = receiptContent,
                    TextWrapping = TextWrapping.Wrap,
                    Margin = new Thickness(20) // Add margin for better readability
                };

                // Create a ScrollViewer to make the content scrollable
                ScrollViewer scrollViewer = new ScrollViewer
                {
                    Content = textBlock,
                    VerticalScrollBarVisibility = ScrollBarVisibility.Auto // Enable vertical scrollbar
                };

                // Create the receipt dialog with the scrollable content
                ContentDialog receiptDialog = new ContentDialog
                {
                    Title = "Order Receipt",
                    Content = scrollViewer, // Set the ScrollViewer as the content
                    CloseButtonText = "Close"
                };

                // Handle the Closed event to clear the ProductCartList after the dialog is closed
                receiptDialog.Closed += ReceiptDialog_Closed;

                // Show the dialog
                ContentDialogResult result = await receiptDialog.ShowAsync();

            }
            catch (Exception ex)
            {
                Buttons.ShowPrompt(ex.Message);
            }

        }



        private async void ReceiptDialog_Closed(ContentDialog sender, ContentDialogClosedEventArgs args)
        {
            try
            {
                // Generate receipt content
                string receiptContent = GenerateReceiptContent();

                // Create a new PDF document
                PdfDocument document = new PdfDocument();


                // Add a page to the document with custom dimensions
                // Here, I'm setting a very long height for demonstration purposes
                PdfPage page = document.AddPage();
                page.Width = XUnit.FromInch(8.5); // Standard letter size width
                page.Height = XUnit.FromInch(20); // Custom height - adjust as needed

                // Create XGraphics object from the PDF page
                XGraphics gfx = XGraphics.FromPdfPage(page);

                // Load the image to use as the header
                StorageFile imageFile = await StorageFile.GetFileFromApplicationUriAsync(new Uri("ms-appx:///Icons/Title3.png")); // Replace "header_image.jpg" with the actual file name of your image
                XImage headerImage = XImage.FromStream(() => imageFile.OpenStreamForReadAsync().Result);

                // Calculate the position for the top right corner of the header
                double headerImageX = 450; // Adjust the value as needed for horizontal position
                double headerImageY = 10; // Adjust the value as needed for vertical position
                double headerImageWidth = 150; // Adjust the value as needed for image width
                double headerImageHeight = 100; // Adjust the value as needed for image height

                // Draw the image as the header at the calculated position
                gfx.DrawImage(headerImage, new XRect(headerImageX, headerImageY, headerImageWidth, headerImageHeight));

                // Set font and brush for drawing text
                XFont font = new XFont("Verdana", 8, XFontStyle.Bold);
                XBrush brush = XBrushes.Black;

                // Split receipt content into lines
                string[] lines = receiptContent.Split('\n');

                // Set starting position for drawing text
                double textYPosition = 40;

                // Draw each line of the receipt content
                foreach (string line in lines)
                {
                    gfx.DrawString(line, font, brush, new XPoint(40, textYPosition));
                    textYPosition += 20; // Adjust line spacing as needed
                }


                // Draw footer
                string footerText1 = "Thank you for choosing Ferdin TubigHub :)";
                XSize footerTextSize1 = gfx.MeasureString(footerText1, font);
                double footerX1 = (page.Width - footerTextSize1.Width) / 2;
                double footerY1 = page.Height - 60; // Adjust the value as needed for vertical position
                gfx.DrawString(footerText1, font, brush, new XPoint(footerX1, footerY1));

                string footerText2 = "Please save this receipt for your reference.";
                XSize footerTextSize2 = gfx.MeasureString(footerText2, font);
                double footerX2 = (page.Width - footerTextSize2.Width) / 2;
                double footerY2 = page.Height - 40; // Adjust the value as needed for vertical position
                gfx.DrawString(footerText2, font, brush, new XPoint(footerX2, footerY2));

                // Save the PDF document to a file
                StorageFile pdfFile = await ApplicationData.Current.LocalFolder.CreateFileAsync("Receipt.pdf", CreationCollisionOption.ReplaceExisting);
                using (Stream fileStream = await pdfFile.OpenStreamForWriteAsync())
                {
                    document.Save(fileStream, false);
                }

                // Show file picker for the user to save the PDF file
                Windows.Storage.Pickers.FileSavePicker savePicker = new Windows.Storage.Pickers.FileSavePicker
                {
                    SuggestedStartLocation = Windows.Storage.Pickers.PickerLocationId.DocumentsLibrary
                };
                savePicker.FileTypeChoices.Add("Adobe PDF Document", new List<string>() { ".pdf" });
                savePicker.SuggestedFileName = "Receipt";

                StorageFile file = await savePicker.PickSaveFileAsync();
                if (file != null)
                {
                    CachedFileManager.DeferUpdates(file);
                    await pdfFile.CopyAndReplaceAsync(file);
                    FileUpdateStatus status = await CachedFileManager.CompleteUpdatesAsync(file);
                    if (status == FileUpdateStatus.Complete)
                    {
                        Buttons.ShowPrompt("Receipt saved successfully");
                    }
                    else
                    {
                        Buttons.ShowPrompt("Receipt failed to save");
                    }
                }
                else
                {
                    Buttons.ShowPrompt("User cancelled to save the receipt");
                }

                // Clear the ProductCartList after the receipt dialog is closed
                ProductCartList.Clear();
                LoadCartItems(); // Refresh the cart UI
            }
            catch (Exception ex)
            {
                Buttons.ShowPrompt(ex.Message);
            }

        }




        private void AutoSuggestBox_TextChanged(AutoSuggestBox sender, AutoSuggestBoxTextChangedEventArgs args)
        {
            try
            {
                // If the user is typing
                if (args.Reason == AutoSuggestionBoxTextChangeReason.UserInput)
                {
                    string userInput = sender.Text.ToLower(); // Get the user input and convert it to lowercase

                    // Filter the ProductCartList based on the user input
                    List<ProductCart> filteredList = ProductCartList.Where(item => item.ProductName.ToLower().Contains(userInput)).ToList();

                    // Assign the filtered list to the ItemsSource of the ListView
                    ListViewCart.ItemsSource = filteredList;

                    // If there's no user input, display all items
                    if (string.IsNullOrWhiteSpace(userInput))
                    {
                        ListViewCart.ItemsSource = ProductCartList;
                    }
                }
            }
            catch (Exception ex)
            {
                Buttons.ShowPrompt(ex.Message);
            }

        }

        private void tbxGcash_TextChanged(object sender, TextChangedEventArgs e)
        {
            try
            {
                UpdatePayButtonState();
            }
            catch (Exception ex)
            {
                Buttons.ShowPrompt(ex.Message);
            }
        }

        private void tbxCard_TextChanged(object sender, TextChangedEventArgs e)
        {
            try
            {
                UpdatePayButtonState();

            }
            catch (Exception ex)
            {
                Buttons.ShowPrompt(ex.Message);
            }
        }

        private void UpdatePayButtonState()
        {
            try
            {
                // Check if GCash and Debit Card text boxes are not empty
                bool gcashFilled = !string.IsNullOrWhiteSpace(tbxGcash.Text);
                bool cardFilled = !string.IsNullOrWhiteSpace(tbxCard.Text);

                // Enable the Pay button if both text boxes are filled, otherwise disable it
                btnPay.IsEnabled = gcashFilled || cardFilled;
            }
            catch (Exception ex)
            {
                Buttons.ShowPrompt(ex.Message);
            }

        }


        private void tbxPhoneNumber_PreviewKeyDown(object sender, KeyRoutedEventArgs e)
        {
            try
            {
                // Check if the pressed key is a numeric key (0-9) or a control key
                Buttons.HandleNumericInput(e);
            }
            catch (Exception ex)
            {
                Buttons.ShowPrompt(ex.Message);
            }
        }
    }
}
