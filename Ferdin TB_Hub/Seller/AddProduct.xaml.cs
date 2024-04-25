using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage.Pickers;
using Windows.Storage;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;
using Ferdin_TB_Hub.Classes;
using static Ferdin_TB_Hub.Classes.Database;
using Windows.Graphics.Imaging;
using Windows.Storage.Streams;
using System.Threading.Tasks;
using Windows.Storage.Provider;
using Microsoft.Data.Sqlite;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace Ferdin_TB_Hub.Seller
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class AddProduct : Page
    {
        private StorageFile selectedFile;
        private int currentSellerID;
        private ProductDetails _productdetails;
        public AddProduct()
        {
            try
            {
                this.InitializeComponent();

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

                // Check if the parameter is not null and is of type string
                if (e.Parameter != null && e.Parameter is string)
                {
                    // Cast the parameter to string and assign it to SellerID TextBox
                    tbxSellerID.Text = e.Parameter as string;
                }
            }
            catch (Exception ex)
            {
                Buttons.ShowPrompt(ex.Message);
            }
          
        }

        private async void InsertPicture_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // Create file picker
                var picker = new FileOpenPicker();
                picker.ViewMode = PickerViewMode.Thumbnail;
                picker.SuggestedStartLocation = PickerLocationId.PicturesLibrary;
                picker.FileTypeFilter.Add(".jpg");
                picker.FileTypeFilter.Add(".jpeg");
                picker.FileTypeFilter.Add(".png");

                // Show file picker
                selectedFile = await picker.PickSingleFileAsync();
                if (selectedFile != null)
                {
                    // Open a stream for the selected file
                    using (var stream = await selectedFile.OpenAsync(FileAccessMode.Read))
                    {
                        // Set the image source to the selected bitmap
                        BitmapImage bitmapImage = new BitmapImage();
                        await bitmapImage.SetSourceAsync(stream);
                        ProductImage.Source = bitmapImage;
                    }
                }
            }
            catch (Exception ex)
            {
                Buttons.ShowPrompt(ex.Message);
            }
          
        }

        private async Task<byte[]> ConvertImageToByteArray(StorageFile file)
        {
            using (IRandomAccessStream stream = await file.OpenAsync(FileAccessMode.Read))
            {
                using (DataReader reader = new DataReader(stream))
                {
                    await reader.LoadAsync((uint)stream.Size);
                    byte[] bytes = new byte[stream.Size];
                    reader.ReadBytes(bytes);
                    return bytes;
                }
            }
        }

        // Method to check if RichEditBox content is empty
        private bool IsRichEditBoxEmpty(RichEditBox richEditBox)
        {
            string text;
            richEditBox.Document.GetText(Windows.UI.Text.TextGetOptions.None, out text);
            return string.IsNullOrWhiteSpace(text);
        }

        // Method to get the text from RichEditBox
        private string GetRichEditBoxText(RichEditBox richEditBox)
        {
            string text;
            richEditBox.Document.GetText(Windows.UI.Text.TextGetOptions.None, out text);
            return text;
        }

        private async void SubmitProduct_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // Check if any of the textboxes are empty
                if (string.IsNullOrWhiteSpace(tbxProductName.Text) ||
                 string.IsNullOrWhiteSpace(tbxProductPrice.Text) ||
                    string.IsNullOrWhiteSpace(tbxProductQuantity.Text) ||
             IsRichEditBoxEmpty(tbxProductDescription))
                {
                    // Prompt the user to reinput if any of the textboxes are empty
                    Buttons.ShowPrompt("Please fill in all fields before submitting.");
                    return;
                }

                // Check if product price is a valid number and not negative
                if (!double.TryParse(tbxProductPrice.Text, out double productPrice) || productPrice < 0)
                {
                    // Prompt the user to reinput if product price is not a valid number or negative
                    Buttons.ShowPrompt("Please enter a valid, non-negative price for the product.");
                    return;
                }

                // Check if product quantity is a valid number and not negative
                if (!int.TryParse(tbxProductQuantity.Text, out int productQuantity) || productQuantity < 0)
                {
                    // Prompt the user to reinput if product quantity is not a valid number or negative
                    Buttons.ShowPrompt("Please enter a valid, non-negative quantity for the product.");
                    return;
                }

                // Check if a category is selected
                if (cbxProductCategory.SelectedItem == null)
                {
                    // Prompt the user to select a category
                    Buttons.ShowPrompt("Please select a category for the product.");
                    return;
                }

                // Check if an image is selected
                if (selectedFile == null)
                {
                    // Prompt the user to select an image
                    Buttons.ShowPrompt("Please select an image for the product.");
                    return;
                }

                // Ask the user for confirmation
                ContentDialog confirmDialog = new ContentDialog
                {
                    Title = "Confirmation",
                    Content = "Are you sure you want to submit this product?",
                    PrimaryButtonText = "Yes",
                    CloseButtonText = "No"
                };

                ContentDialogResult result = await confirmDialog.ShowAsync();
                if (result == ContentDialogResult.Primary)
                {
                    var seller = DataContext as SellerDetails;

                    // If the user confirms, proceed to add the product
                    string productName = tbxProductName.Text;
                    string productCategory = (cbxProductCategory.SelectedItem as ComboBoxItem)?.Content.ToString();
                    string productDescription = GetRichEditBoxText(tbxProductDescription);
                    string sellerIDText = tbxSellerID.Text; // Get the text from the TextBox
                    int ADDsellerID;
                    int.TryParse(sellerIDText, out ADDsellerID); // Convert the text to an integer

                    byte[] productPicture = await ConvertImageToByteArray(selectedFile);

                    DatabaseAccess dbAccess = new DatabaseAccess();

                    //  int sellerID = dbAccess.RetrieveSellerIDFromDatabase(ADDsellerID);

                    Database.AddProduct(productName, productCategory, productPrice, productDescription, productQuantity, productPicture, ADDsellerID);

                    // Clear the textboxes
                    tbxProductName.Text = "";
                    tbxProductPrice.Text = "";
                    tbxProductDescription.Document.SetText(Windows.UI.Text.TextSetOptions.None, "");
                    tbxProductQuantity.Text = "";
                    cbxProductCategory.SelectedIndex = -1;
                    ProductImage.Source = null;


                   // Optionally, display a success message or navigate to another page
                }
                else
                {
                    // If the user cancels, do nothing
                    return;
                }
            }
            catch (Exception ex)
            {
                Buttons.ShowPrompt(ex.Message);
            }
         
        }





    }
}
