using Ferdin_TB_Hub.Classes;
using Ferdin_TB_Hub.ItemDetail;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Composition;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Animation;
using Windows.UI.Xaml.Navigation;
using static Ferdin_TB_Hub.Classes.Database;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace Ferdin_TB_Hub.HomePage_NavigationView
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class Home : Page
    {
        // Fetch all product details from the database
        List<ProductDetails> allProductDetails = Database.GetProductDetails();



        public Home()
        {
            this.InitializeComponent();
            PopulateViewProducts();        
            InitializePriceSlider();
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
                var filteredProducts = allProductDetails.Where(product =>
                    product.ProductName.Contains(sender.Text, StringComparison.OrdinalIgnoreCase)  ||                   
                    product.ProductDescription.ToString().Contains(sender.Text, StringComparison.OrdinalIgnoreCase)

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
                var filteredProducts = allProductDetails.Where(product =>
                    Math.Abs(product.ProductPrice - sliderValue) <= 0.01
                ).ToList();

                // Update the GridView with filtered items
                ViewProducts.ItemsSource = filteredProducts;
            }
        }

        private void cbxCategories_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // Fetch all product details from the database
            List<ProductDetails> allProductDetails = Database.GetProductDetails();

            // Check if "All Products" is selected
            if (cbxCategories.SelectedIndex == 0) // Assuming "All Products" is the first item in the ComboBox
            {
                // Display all products
                ViewProducts.ItemsSource = allProductDetails;
            }
            else if (cbxCategories.SelectedItem != null)
            {
                // Filter the products based on the selected category
                var selectedCategory = ((ComboBoxItem)cbxCategories.SelectedItem).Content.ToString();
                var filteredProducts = allProductDetails.Where(product => product.ProductCategory == selectedCategory).ToList();

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
            // Get the clicked button and its DataContext (which should be the ProductDetails object)
            var button = sender as Button;
            var product = button.DataContext as ProductDetails;

            // Check if the product is not null
            if (product != null)
            {
                // Call the method to pass the product to the cart with default quantity of 1
                Database.PassProductToCart(product, quantity: 1);

                RefreshGridView();

                // Optionally, you can provide feedback to the user that the product has been added to the cart
                // For example, display a message or update UI elements
            }
        }

        private void RefreshGridView()
        {
            // Get the data source for the GridView
            List<ProductDetails> products = Database.GetProductDetails();

            // Set the ItemsSource of the GridView to null to clear the previous data
            ViewProducts.ItemsSource = null;

            // Set the ItemsSource of the GridView to the updated list of products
            ViewProducts.ItemsSource = products;
        }

        private void AddToCart_Loaded(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            var product = button.DataContext as ProductDetails;

            if (product != null)
            {
                // Check if the product quantity is zero or less, then disable the button
                if (Database.GetProductQuantity(product.ProductName) <= 0)
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
            List<ProductDetails> allProductDetails = Database.GetProductDetails();

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

      
    }
}
