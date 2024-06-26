﻿using Ferdin_TB_Hub.Classes;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Storage;
using Windows.Storage.Streams;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;
using static Ferdin_TB_Hub.Classes.Database;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace Ferdin_TB_Hub.HomePage_NavigationView
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>

    public sealed partial class Home : Page, INotifyPropertyChanged
    {
        private BuyerDetails _buyer;
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


        // Fetch all product details from the database
        private readonly List<ProductDetails> allProductDetails = Database.GetAllProductDetails();

        public Home()
        {
            InitializeComponent();
            DataContext = this; // Set the DataContext of the page to itself
            PopulateViewProducts();
            InitializePriceSlider();
        }


        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            // Check if the parameter passed during navigation is a BuyerDetails object
            if (e.Parameter != null && e.Parameter is BuyerDetails)
            {
                // Cast the parameter to BuyerDetails and assign it to the Buyer property
                Buyer = e.Parameter as BuyerDetails;
                // Notify the UI that the Buyer property has changed
                OnPropertyChanged(nameof(Buyer));

                // Update the tbxBuyerID text after setting the Buyer property
                tbxBuyerID.Text = Buyer.BUYER_ID.ToString();
            }
        }


        private void InitializePriceSlider()
        {
            if (allProductDetails.Any()) // Check if there are any elements in allProductDetails
            {
                double maxPrice = allProductDetails.Max(product => product.ProductPrice);
                PriceSlider.Maximum = maxPrice;
            }
            else
            {
                // Handle the case where allProductDetails is empty
                // For example, set a default maximum value for the slider
                PriceSlider.Maximum = 100; // Set a default maximum value of 100
            }

            // Set the slider's value to zero initially
            PriceSlider.Value = 0;

            // Display all products initially since the slider is at zero
            ViewProducts.ItemsSource = allProductDetails;
        }


        private void PopulateViewProducts()
        {
            ViewProducts.ItemsSource = allProductDetails;
        }

        private void AutoSuggestBox_TextChanged(AutoSuggestBox sender, AutoSuggestBoxTextChangedEventArgs args)
        {
            if (args.Reason == AutoSuggestionBoxTextChangeReason.UserInput)
            {
                // Filter the products based on user input
                List<ProductDetails> filteredProducts = allProductDetails.Where(product =>
                    product.ProductName.Contains(sender.Text, StringComparison.OrdinalIgnoreCase) ||
                    product.ProductDescription.ToString().Contains(sender.Text, StringComparison.OrdinalIgnoreCase) ||
                    product.ProductCategory.Contains(sender.Text, StringComparison.OrdinalIgnoreCase)

                ).ToList();

                // Update the ListView with filtered items
                ViewProducts.ItemsSource = filteredProducts;
            }
        }

        private void Slider_ValueChanged(object sender, RangeBaseValueChangedEventArgs e)
        {
            double sliderValue = Math.Round((sender as Slider).Value, 2); // Round to 2 decimal places

            // If slider value is 0, show all products
            if (sliderValue == 0)
            {
                ViewProducts.ItemsSource = allProductDetails;
            }
            else
            {
                // Filter the products to show only those with a price equal to or within 0.01 of the slider value
                List<ProductDetails> filteredProducts = allProductDetails.Where(product =>
                    Math.Abs(product.ProductPrice - sliderValue) <= 0.01
                ).ToList();

                // Update the GridView with filtered items
                ViewProducts.ItemsSource = filteredProducts;
            }
        }

        private void cbxCategories_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // Fetch all product details from the database
            List<ProductDetails> allProductDetails = Database.GetAllProductDetails();

            // Check if "All Products" is selected
            if (cbxCategories.SelectedIndex == 0) // Assuming "All Products" is the first item in the ComboBox
            {
                // Display all products
                ViewProducts.ItemsSource = allProductDetails;
            }
            else if (cbxCategories.SelectedItem != null)
            {
                // Filter the products based on the selected category
                string selectedCategory = ((ComboBoxItem)cbxCategories.SelectedItem).Content.ToString();
                List<ProductDetails> filteredProducts = allProductDetails.Where(product => product.ProductCategory == selectedCategory).ToList();

                // Update the GridView with filtered items
                ViewProducts.ItemsSource = filteredProducts;
            }
            else
            {
                // If no category is selected, display all products
                ViewProducts.ItemsSource = allProductDetails;
            }
        }

        // PASS THE SELECTED PRODUCT TO THE CART
        private void AddToCart_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // Get the clicked button and its DataContext (which should be the ProductDetails object)
                Button button = sender as Button;
                SellerDetails seller = DataContext as SellerDetails;

                // Check if the product is not null
                if (button?.DataContext is ProductDetails product)
                {
                    // Retrieve the buyer's ID using the DatabaseAccess class
                    DatabaseAccess dbAccess = new DatabaseAccess();

                    // Call the method to pass the product to the cart with default quantity of 1
                    Database.PassProductToCart(product, quantity: 1, Buyer.BUYER_ID, product.Seller_ID, product.PRODUCTDETAILS_ID, product.ProductSKU);

                    RefreshGridView(); // Assuming you have a method to refresh the grid view
                    UpdateQuantityText(product); // Assuming you have a method to update the quantity text
                }
            }
            catch (Exception ex)
            {
                Buttons.ShowPrompt(ex.Message);
            }
        }


        private void RefreshGridView()
        {
            // Get the data source for the GridView
            List<ProductDetails> products = Database.GetAllProductDetails();

            // Set the ItemsSource of the GridView to null to clear the previous data
            ViewProducts.ItemsSource = null;

            // Set the ItemsSource of the GridView to the updated list of products
            ViewProducts.ItemsSource = products;
        }

        private void AddToCart_Loaded(object sender, RoutedEventArgs e)
        {
            Button button = sender as Button;

            if (button.DataContext is ProductDetails product)
            {
                // Check if the product quantity is zero or less, then disable the button
                if (Database.GetProductQuantity(product.ProductSKU) <= 0)
                {
                    button.IsEnabled = false;
                }
            }
        }

        private void cbxPrice_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // Get the selected sorting option from the ComboBox
            ComboBoxItem selectedItem = (ComboBoxItem)cbxPrice.SelectedItem;
            string sortingOption = selectedItem.Content.ToString();

            // Fetch all product details from the database
            List<ProductDetails> allProductDetails = Database.GetAllProductDetails();

            switch (sortingOption)
            {
                case "Low to High":
                    // Sort the products by price in ascending order
                    allProductDetails = allProductDetails.OrderBy(product => product.ProductPrice).ToList();
                    break;
                case "High to Low":
                    // Sort the products by price in descending order
                    allProductDetails = allProductDetails.OrderByDescending(product => product.ProductPrice).ToList();
                    break;
                case "All":
                    // Do nothing, display all products (already handled in InitializePriceSlider)
                    break;
                default:
                    break;
            }

            // Update the GridView with the sorted items
            ViewProducts.ItemsSource = allProductDetails;
        }

        private void UpdateQuantityText(ProductDetails product)
        {
            // Get the quantity of the selected product
            int quantity = Database.GetProductQuantity(product.ProductSKU);

            // Update the quantity text block
            lblQuanitity.Text = quantity.ToString();
        }

        private void ViewProducts_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (ViewProducts.SelectedItem != null)
            {
                // Extract the product details from the selected item

                // Display the description outside of the GridView
                if (ViewProducts.SelectedItem is ProductDetails selectedProduct)
                {
                    lblProductName.Text = selectedProduct.ProductName;
                    lblDescription.Text = selectedProduct.ProductDescription;
                    lblQuanitity.Text = selectedProduct.ProductQuantity.ToString();
                    lblPrice.Text = "₱" + selectedProduct.ProductPrice.ToString("N2"); // Formats with 2 decimal places and thousands separator

                    // Set the product image
                    SetProductImage(selectedProduct.ProductPicture);
                }
            }
            else
            {
                // Clear the description if no item is selected
                //  lblDescription.Text = string.Empty;
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
                    _ = await stream.WriteAsync(imageBytes.AsBuffer());
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


