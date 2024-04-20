using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Devices.Geolocation;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Windows.UI.Xaml.Controls.Maps;
using static Ferdin_TB_Hub.Classes.Database;
using System.Collections.ObjectModel;
using Ferdin_TB_Hub.Classes;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace Ferdin_TB_Hub.HomePage_NavigationView
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class Locate : Page
    {
        public ObservableCollection<SellerDetails> Sellers { get; set; } = new ObservableCollection<SellerDetails>();
        public ObservableCollection<ProductDetails> Products { get; set; } = new ObservableCollection<ProductDetails>();

     

        public Locate()
        {
            this.InitializeComponent();
            PopulateSellers(); // Call method to populate SellersListView

        }

        private void PopulateSellers()
        {
            // Retrieve sellers' information from the database
            List<SellerDetails> sellers = Database.GetSellerRecords();

            // Clear the existing collection before adding new items
            Sellers.Clear();

            // Add retrieved sellers to the Sellers collection
            foreach (var seller in sellers)
            {
                Sellers.Add(seller);
            }
        }

        private void SellersListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // Get the selected seller from the SellersListView
            SellerDetails selectedSeller = SellersListView.SelectedItem as SellerDetails;

            if (selectedSeller != null)
            {
                // Retrieve products associated with the selected seller from the database
                List<ProductDetails> products = Database.GetProductDetails(selectedSeller.SELLER_ID);

                // Clear the existing collection before adding new items
                Products.Clear();

                // Add retrieved products to the Products collection
                foreach (var product in products)
                {
                    Products.Add(product);
                }
            }
        }



        private void AutoSuggestBox_TextChanged(AutoSuggestBox sender, AutoSuggestBoxTextChangedEventArgs args)
        {
            if (args.Reason == AutoSuggestionBoxTextChangeReason.UserInput)
            {
                // Get the search term entered by the user
                string searchTerm = sender.Text.Trim();

                // If the search term is empty, populate all sellers
                if (string.IsNullOrWhiteSpace(searchTerm))
                {
                    // Update the ListView with all sellers
                    SellersListView.ItemsSource = Sellers;
                }
                else
                {
                    // Filter sellers based on the search term
                    var filteredSellers = Sellers.Where(seller =>
                        seller.BusinessName.Contains(searchTerm, StringComparison.OrdinalIgnoreCase) ||
                        seller.Email.Contains(searchTerm, StringComparison.OrdinalIgnoreCase) ||
                        seller.Username.Contains(searchTerm, StringComparison.OrdinalIgnoreCase) ||
                        seller.PhoneNumber.Contains(searchTerm, StringComparison.OrdinalIgnoreCase) ||
                        seller.AddressLine1.Contains(searchTerm, StringComparison.OrdinalIgnoreCase) ||
                        seller.AddressLine2.Contains(searchTerm, StringComparison.OrdinalIgnoreCase)

                  
                    ).ToList();

                    // Update the ListView with filtered sellers
                    SellersListView.ItemsSource = filteredSellers;
                }
            }
        }

    }
}
