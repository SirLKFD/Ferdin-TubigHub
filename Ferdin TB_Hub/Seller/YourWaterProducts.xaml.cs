using Ferdin_TB_Hub.Classes;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage;
using Windows.Storage.Pickers;
using Windows.Storage.Streams;
using Windows.System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;
using static Ferdin_TB_Hub.Classes.Database;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace Ferdin_TB_Hub.Seller
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    /// 

    public sealed partial class YourWaterProducts : Page
    {
        private StorageFile selectedFile;
        private int currentSellerID;
        public YourWaterProducts()
        {
            this.InitializeComponent();
            PopulateProductList();
        }
     
        private void PopulateProductList()
        {

            List<ProductDetails> productDetailsList = Database.GetProductDetails(); 

            // Bind the list of product details to the ListView
            ListProducts.ItemsSource = productDetailsList;
        }

        private async void ChangePicture_Click(object sender, RoutedEventArgs e)
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
        private async void UpdateProduct_Click(object sender, RoutedEventArgs e)
        {
            // Display a confirmation dialog before updating the product
            ContentDialog confirmDialog = new ContentDialog
            {
                Title = "Confirm Update",
                Content = "Are you sure you want to update this product?",
                PrimaryButtonText = "Yes",
                CloseButtonText = "No"
            };

            ContentDialogResult result = await confirmDialog.ShowAsync();
            if (result == ContentDialogResult.Primary)
            {
                // User confirmed the update
                // Retrieve the selected product and update its details
                ProductDetails selectedProduct = (ProductDetails)ListProducts.SelectedItem;

                // Validate input
                if (string.IsNullOrWhiteSpace(tbxProductName.Text) || string.IsNullOrWhiteSpace(tbxProductDescription.Text) ||
                    string.IsNullOrWhiteSpace(tbxProductPrice.Text) || string.IsNullOrWhiteSpace(tbxProductQuantity.Text))
                {
                    // Invalid input, ask the user to re-input
                    ShowMessageDialog("Please provide valid values for all fields.");
                    return;
                }

                double price;
                if (!double.TryParse(tbxProductPrice.Text, out price))
                {
                    // Invalid price input, ask the user to re-input
                    ShowMessageDialog("Please provide a valid price.");
                    return;
                }

                int quantity;
                if (!int.TryParse(tbxProductQuantity.Text, out quantity))
                {
                    // Invalid quantity input, ask the user to re-input
                    ShowMessageDialog("Please provide a valid quantity.");
                    return;
                }

                // Update product details
                selectedProduct.ProductName = tbxProductName.Text;
                selectedProduct.ProductCategory = ((ComboBoxItem)cbxProductCategory.SelectedItem).Content.ToString();
                selectedProduct.ProductPrice = price;
                selectedProduct.ProductDescription = tbxProductDescription.Text;
                selectedProduct.ProductQuantity = quantity;

                // Update the product picture if a new picture is selected
                if (selectedFile != null)
                {
                    // Open a stream for the selected file
                    using (var stream = await selectedFile.OpenAsync(FileAccessMode.Read))
                    {
                        // Convert the selected image file to a byte array
                        var reader = new DataReader(stream.GetInputStreamAt(0));
                        await reader.LoadAsync((uint)stream.Size);
                        byte[] imageBytes = new byte[stream.Size];
                        reader.ReadBytes(imageBytes);

                        // Set the selected image as the product picture
                        selectedProduct.ProductPicture = imageBytes;

                        // Set the selected image as the source for the ProductImage control
                        SetProductImage(imageBytes);
                    }
                }

                // Update the product details in the database
                Database.UpdateProductDetailsFromDatabase(selectedProduct.PRODUCTDETAILS_ID, selectedProduct.ProductName, selectedProduct.ProductCategory,
                    selectedProduct.ProductPrice, selectedProduct.ProductDescription, selectedProduct.ProductQuantity, selectedProduct.ProductPicture);

                // Refresh the product list
                PopulateProductList();
            }
        }

        private async void ShowMessageDialog(string message)
        {
            ContentDialog dialog = new ContentDialog
            {
                Title = "Error",
                Content = message,
                CloseButtonText = "OK"
            };
            await dialog.ShowAsync();
        }

        private async void DeleteProduct_Click(object sender, RoutedEventArgs e)
        {
            // Check if any product is selected for deletion
            if (ListProducts.SelectedItem == null)
            {
                // If no product is selected, display an error dialog
                ShowMessageDialog("Please select a product to delete.");
                return;
            }

            // Display a confirmation dialog before deleting the product
            ContentDialog confirmDialog = new ContentDialog
            {
                Title = "Confirm Delete",
                Content = "Are you sure you want to delete this product?",
                PrimaryButtonText = "Yes",
                CloseButtonText = "No"
            };

            ContentDialogResult result = await confirmDialog.ShowAsync();
            if (result == ContentDialogResult.Primary)
            {
                // User confirmed the deletion
                // Retrieve the selected product and delete it from the database
                ProductDetails selectedProduct = (ProductDetails)ListProducts.SelectedItem;
                Database.DeleteProductDetailsFromDatabase(selectedProduct.ProductName);

                // Refresh the product list
                PopulateProductList();
            }
        }

        private void AutoSuggestBox_TextChanged(AutoSuggestBox sender, AutoSuggestBoxTextChangedEventArgs args)
        {
            if (args.Reason == AutoSuggestionBoxTextChangeReason.UserInput)
            {
                // Fetch all product details from the database
                List<ProductDetails> allProductDetails = Database.GetProductDetails();



                // Filter the products based on user input
                var filteredProducts = allProductDetails.Where(product =>
                    product.ProductName.Contains(sender.Text, StringComparison.OrdinalIgnoreCase) ||
                    product.ProductCategory.Contains(sender.Text, StringComparison.OrdinalIgnoreCase) ||
                    product.ProductPrice.ToString().Contains(sender.Text, StringComparison.OrdinalIgnoreCase) ||
                    product.ProductQuantity.ToString().Contains(sender.Text, StringComparison.OrdinalIgnoreCase)
                ).ToList();

                // Update the ListView with filtered items
                ListProducts.ItemsSource = filteredProducts;
            }
        }

        private void ListProducts_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // Check if an item is selected
            if (ListProducts.SelectedItem != null)
            {
                // Retrieve the selected product
                ProductDetails selectedProduct = (ProductDetails)ListProducts.SelectedItem;

                // Display the selected product's details in the textboxes, combobox, and image
                tbxProductName.Text = selectedProduct.ProductName;
                tbxProductPrice.Text = selectedProduct.ProductPrice.ToString();
                tbxProductDescription.Text = selectedProduct.ProductDescription;
                tbxProductQuantity.Text = selectedProduct.ProductQuantity.ToString();

                // Select the category in the combobox
                foreach (ComboBoxItem item in cbxProductCategory.Items)
                {
                    if (item.Content.ToString() == selectedProduct.ProductCategory)
                    {
                        cbxProductCategory.SelectedItem = item;
                        break;
                    }
                }

                // Set the product image
                SetProductImage(selectedProduct.ProductPicture);
            }
        }


        private async void SetProductImage(byte[] imageBytes)
        {
            if (imageBytes != null && imageBytes.Length > 0)
            {
                // Convert byte array to BitmapImage
                BitmapImage bitmapImage = new BitmapImage();
                using (InMemoryRandomAccessStream stream = new InMemoryRandomAccessStream())
                {
                    await stream.WriteAsync(imageBytes.AsBuffer());
                    stream.Seek(0);
                    await bitmapImage.SetSourceAsync(stream);
                }

                // Set the BitmapImage as the source of the ProductImage control
                ProductImage.Source = bitmapImage;
            }
            else
            {
                // If no image is available, you can set a default image or clear the existing image
                ProductImage.Source = null;
            }
        }

       
    }
}
