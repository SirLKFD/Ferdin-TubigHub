using Ferdin_TB_Hub.Classes;
using System;
using System.Collections.Generic;
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
using Windows.UI.Xaml.Navigation;
using static Ferdin_TB_Hub.Classes.Database;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace Ferdin_TB_Hub.HomePage_NavigationView
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class BrowseWater : Page
    {
        public BrowseWater()
        {
            this.InitializeComponent();
            PopulateBrowseProducts();
        }

        private void PopulateBrowseProducts()
        {
            List<ProductDetails> productDetailsList = Database.GetProductDetails();
            // Set the grouped products as the ListView's items source
            BrowseProducts.ItemsSource = productDetailsList;
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
        }

        private void AutoSuggestBox_TextChanged(AutoSuggestBox sender, AutoSuggestBoxTextChangedEventArgs args)
        {
            if (args.Reason == AutoSuggestionBoxTextChangeReason.UserInput)
            {
                // Fetch all product details from the database
                List<ProductDetails> categoricalProductDetails = Database.GetProductDetails();

                // Filter the products based on user input
                var filteredProducts = categoricalProductDetails.Where(product =>
                    product.ProductName.Contains(sender.Text, StringComparison.OrdinalIgnoreCase)
                ).ToList();

                // Update the ListView with filtered items
                BrowseProducts.ItemsSource = filteredProducts;
            }
        }
    }
}
