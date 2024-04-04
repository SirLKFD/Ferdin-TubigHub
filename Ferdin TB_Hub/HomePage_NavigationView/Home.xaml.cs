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

        private void AddToCart_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
