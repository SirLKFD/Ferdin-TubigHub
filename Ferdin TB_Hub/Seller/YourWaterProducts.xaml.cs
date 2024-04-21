using Ferdin_TB_Hub.Classes;
using System;
using System.Collections.Generic;
using System.ComponentModel;
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

    public sealed partial class YourWaterProducts : Page, INotifyPropertyChanged
    {
        private StorageFile selectedFile;
        private int currentSellerID;
        public int SellerID { get; set; }
        private ProductDetails _product;
        public ProductDetails Product
        {
            get { return _product; }
            set
            {
                if (_product != value)
                {
                    _product = value;
                    OnPropertyChanged(nameof(Product));
                }
            }
        }

        private SellerDetails _seller;
        public SellerDetails Seller
        {
            get { return _seller; }
            set
            {
                if (_seller != value)
                {
                    _seller = value;
                    OnPropertyChanged(nameof(Seller));
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }




        public YourWaterProducts()
        {
            try
            {
                this.InitializeComponent();

                // Initialize Seller object before accessing its properties
                Seller = new SellerDetails();

                // Subscribe to the TextChanged event for tbxSellerID
                tbxSellerID.TextChanged += tbxSellerID_TextChanged;

                // Check if Seller object is not null before accessing its properties
                if (Seller != null)
                {
                    // Set the initial seller ID to the one entered in the TextBox
                    int.TryParse(tbxSellerID.Text, out int initialSellerID);
                    SellerID = initialSellerID;

                    // Automatically invoke the refresh button based on the initial value of tbxSellerID
                    btnRefresh_Click(null, null);
                }
            }
            catch (Exception ex)
            {
                Buttons.ShowMessage(ex.Message);
            }
          
        }


        private void PopulateProductList()
        {
            try
            {
                int.TryParse(tbxSellerID.Text, out int newSellerID);
                SellerID = newSellerID;

                // Get the list of products for the initial SellerID
                List<ProductDetails> productDetailsList = Database.GetProductDetails(SellerID + 1);

                // Update the ListView with the retrieved product list
                ListProducts.ItemsSource = null; // Set the ItemsSource to null
                ListProducts.ItemsSource = productDetailsList; // Set the ItemsSource to the updated list
            }
           catch (Exception ex)
            {
                Buttons.ShowMessage(ex.Message);
            }

          
        }

        private async void ChangePicture_Click(object sender, RoutedEventArgs e)
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
            catch
            {
                Buttons.ShowMessage("An error occurred while changing the picture.");
            }
          
        }
        private async void UpdateProduct_Click(object sender, RoutedEventArgs e)
        {
            try
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
                    if (!double.TryParse(tbxProductPrice.Text, out price) || price < 0)
                    {
                        // Invalid or negative price input, ask the user to re-input
                        ShowMessageDialog("Please provide a valid non-negative price.");
                        return;
                    }

                    int quantity;
                    if (!int.TryParse(tbxProductQuantity.Text, out quantity) || quantity < 0)
                    {
                        // Invalid or negative quantity input, ask the user to re-input
                        ShowMessageDialog("Please provide a valid non-negative quantity.");
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

                    // Update the product details in the database using ProductSKU
                    Database.UpdateProductDetailsFromDatabase(selectedProduct.ProductSKU, selectedProduct.ProductName, selectedProduct.ProductCategory,
                        selectedProduct.ProductPrice, selectedProduct.ProductDescription, selectedProduct.ProductQuantity, selectedProduct.ProductPicture);

                    ChangePicture.IsEnabled = false;
                    UpdateProduct.IsEnabled = false;
                    DeleteProduct.IsEnabled = false;

                    // Clear textboxes
                    tbxProductName.Text = string.Empty;
                    tbxProductPrice.Text = string.Empty;
                    tbxProductDescription.Text = string.Empty;
                    tbxProductQuantity.Text = string.Empty;
                    lblProductSKU.Text = string.Empty;

                    // Clear combobox selection
                    cbxProductCategory.SelectedItem = null;

                    // Clear image
                    ProductImage.Source = null;

                    // Refresh the product list
                    if (int.TryParse(tbxSellerID.Text, out int sellerID))
                    {
                        // Update the ListView data based on the SellerID from tbxSellerID
                        UpdateListViewData(sellerID);
                    }
                }
            }
            catch (Exception ex)
            {
                Buttons.ShowMessage(ex.Message);
            }

        }



        private async void ShowMessageDialog(string message)
        {
            try
            {
                ContentDialog dialog = new ContentDialog
                {
                    Title = "Error",
                    Content = message,
                    CloseButtonText = "OK"
                };
                await dialog.ShowAsync();
            }
            catch
            {

            }
           
        }

        private async void DeleteProduct_Click(object sender, RoutedEventArgs e)
        {
            try
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
                    Database.DeleteProductDetailsFromDatabase(selectedProduct.ProductSKU);


                    ChangePicture.IsEnabled = false;
                    UpdateProduct.IsEnabled = false;
                    DeleteProduct.IsEnabled = false;

                    // Clear textboxes
                    tbxProductName.Text = string.Empty;
                    tbxProductPrice.Text = string.Empty;
                    tbxProductDescription.Text = string.Empty;
                    tbxProductQuantity.Text = string.Empty;
                    lblProductSKU.Text = string.Empty;

                    // Clear combobox selection
                    cbxProductCategory.SelectedItem = null;

                    // Clear image
                    ProductImage.Source = null;

                    // Refresh the product list
                    if (int.TryParse(tbxSellerID.Text, out int sellerID))
                    {
                        // Update the ListView data based on the SellerID from tbxSellerID
                        UpdateListViewData(sellerID);
                    }
                }
            }
            catch (Exception ex)
            {
                Buttons.ShowMessage(ex.Message);
            }
         
        }


        private void AutoSuggestBox_TextChanged(AutoSuggestBox sender, AutoSuggestBoxTextChangedEventArgs args)
        {
            try
            {
                if (args.Reason == AutoSuggestionBoxTextChangeReason.UserInput)
                {
                    // Fetch all product details from the database
                    int sellerID = SellerID;
                    if (tbxSellerID.Text == SellerID.ToString())
                    {
                        // Search based on the initial value of tbxSellerID
                        List<ProductDetails> sellerProductDetails = Database.GetProductDetails(SellerID);
                        // Filter the products based on user input
                        var filteredProducts = sellerProductDetails.Where(product =>
                            product.ProductName.Contains(sender.Text, StringComparison.OrdinalIgnoreCase) ||
                            product.ProductCategory.Contains(sender.Text, StringComparison.OrdinalIgnoreCase) ||
                            product.ProductPrice.ToString().Contains(sender.Text, StringComparison.OrdinalIgnoreCase) ||
                            product.ProductQuantity.ToString().Contains(sender.Text, StringComparison.OrdinalIgnoreCase)
                        ).ToList();
                        // Update the ListView with filtered items
                        ListProducts.ItemsSource = filteredProducts;
                    }
                    else
                    {
                        // Search based on the user input in tbxSellerID
                        if (int.TryParse(tbxSellerID.Text, out sellerID))
                        {
                            List<ProductDetails> sellerProductDetails = Database.GetProductDetails(sellerID);
                            // Filter the products based on user input
                            var filteredProducts = sellerProductDetails.Where(product =>
                                product.ProductName.Contains(sender.Text, StringComparison.OrdinalIgnoreCase) ||
                                product.ProductCategory.Contains(sender.Text, StringComparison.OrdinalIgnoreCase) ||
                                product.ProductPrice.ToString().Contains(sender.Text, StringComparison.OrdinalIgnoreCase) ||
                                product.ProductQuantity.ToString().Contains(sender.Text, StringComparison.OrdinalIgnoreCase)
                            ).ToList();
                            // Update the ListView with filtered items
                            ListProducts.ItemsSource = filteredProducts;
                        }
                    }
                }
            }
            catch
            (Exception ex)
            {
                Buttons.ShowMessage(ex.Message);
            }
          
        }

        private void ListProducts_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
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
                    lblProductSKU.Text = selectedProduct.ProductSKU.ToString();

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

                    // Enable the buttons
                    ChangePicture.IsEnabled = true;
                    UpdateProduct.IsEnabled = true;
                    DeleteProduct.IsEnabled = true;
                }

            }
            catch
            (Exception ex)
            {
                Buttons.ShowMessage(ex.Message);
            }
          
        }


        private async void SetProductImage(byte[] imageBytes)
        {
            try
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
            catch
            (Exception ex)
            {
                Buttons.ShowMessage(ex.Message);
            }

          
        }

        private void tbxSellerID_TextChanged(object sender, TextChangedEventArgs e)
        {
            try
            {
                if (int.TryParse(tbxSellerID.Text, out int sellerID))
                {
                    UpdateListViewData(sellerID);
                }
                else
                {
                    // Handle the case where the input is not a valid integer
                    // You can show a message to the user indicating that the SellerID must be a valid number
                }
            }
            catch (Exception ex)
            {
                Buttons.ShowMessage(ex.Message);
            }
       
        }

        private void UpdateListViewData(int sellerID)
        {
            try
            {
                var products = GetProductDetails(sellerID);
                ListProducts.ItemsSource = products;
            }
            catch (Exception ex)
            {
                Buttons.ShowMessage(ex.Message);
            }
         
        }

        private void btnRefresh_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                ChangePicture.IsEnabled = false;
                UpdateProduct.IsEnabled = false;
                DeleteProduct.IsEnabled = false;

                // Clear textboxes
                tbxProductName.Text = string.Empty;
                tbxProductPrice.Text = string.Empty;
                tbxProductDescription.Text = string.Empty;
                tbxProductQuantity.Text = string.Empty;
                lblProductSKU.Text = string.Empty;

                // Clear combobox selection
                cbxProductCategory.SelectedItem = null;

                // Clear image
                ProductImage.Source = null;


                // Check if tbxSellerID contains a valid integer value
                if (int.TryParse(tbxSellerID.Text, out int sellerID))
                {
                    // Update the ListView data based on the SellerID from tbxSellerID
                    UpdateListViewData(sellerID);
                }
                else
                {
                    // Handle the case where the input is not a valid integer
                    // You can show a message to the user indicating that the SellerID must be a valid number
                    //  ShowMessageDialog("Please enter a valid SellerID.");
                }
            }
            catch (Exception ex)
            {
                Buttons.ShowMessage(ex.Message);
            }
          
        }

    }
}
