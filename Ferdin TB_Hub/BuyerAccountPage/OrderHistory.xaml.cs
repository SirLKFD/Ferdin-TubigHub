﻿using Ferdin_TB_Hub.Classes;
using OfficeOpenXml;
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
using Windows.UI.Xaml.Navigation;
using static Ferdin_TB_Hub.Classes.Database;
using System.ComponentModel;
using Windows.UI.Popups;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace Ferdin_TB_Hub.BuyerAccountPage
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class OrderHistory : Page, INotifyPropertyChanged
    {
        public List<ProductReceipt> ProductReceipts { get; set; }
        public int BuyerID { get; set; }

        private BuyerDetails _buyer;
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

        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }



        public OrderHistory()
        {
            try
            {
                this.InitializeComponent();
                LoadProductReceipts();
                this.DataContext = this; // Set the DataContext of the page to itself

                // Initialize Buyer object before accessing its properties
                Buyer = new BuyerDetails();

                // Check if Buyer object is not null before accessing its properties
                if (Buyer != null)
                {
                    // Set the initial buyer ID to the one entered in the TextBox
                    int.TryParse(tbxBuyerID.Text, out int initialBuyerID);
                    BuyerID = initialBuyerID;

                    // Call GetBuyerProductReceipts with the initial buyer ID
                    receiptDataGrid.ItemsSource = GetBuyerProductReceipts(BuyerID + 1);
                }

                receiptDataGrid.Columns["ProductPrice"].Format = "₱0.00";
                receiptDataGrid.Columns["OrderNumber"].Format = "000000000000";
            }
            catch (Exception ex)
            {
          
            }
        }





        private void LoadProductReceipts()
        {
            // Fetch product receipts from the database
            ProductReceipts = Database.GetProductReceipts();

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

                // Trigger the TextChanged event manually to update the receiptDataGrid
                tbxBuyerID_TextChanged(tbxBuyerID, null);
            }
        }


        private void AutoSuggestBox_TextChanged(AutoSuggestBox sender, AutoSuggestBoxTextChangedEventArgs args)
        {
            if (args.Reason == AutoSuggestionBoxTextChangeReason.UserInput)
            {
                if (ProductReceipts != null) // Ensure ProductReceipts is not null
                {
                    // Filter the ProductReceipts based on user input
                    var filteredReceipts = ProductReceipts.Where(receipt =>
                        receipt.OrderNumber.ToString().Contains(sender.Text, StringComparison.OrdinalIgnoreCase) ||
                        receipt.ProductName.Contains(sender.Text, StringComparison.OrdinalIgnoreCase) ||
                        receipt.ProductCategory.Contains(sender.Text, StringComparison.OrdinalIgnoreCase) ||
                        receipt.LastName.Contains(sender.Text, StringComparison.OrdinalIgnoreCase) ||
                        receipt.FirstName.Contains(sender.Text, StringComparison.OrdinalIgnoreCase) ||
                        receipt.MiddleName.Contains(sender.Text, StringComparison.OrdinalIgnoreCase) ||
                        receipt.Email.Contains(sender.Text, StringComparison.OrdinalIgnoreCase) ||
                        receipt.AddressLine1.Contains(sender.Text, StringComparison.OrdinalIgnoreCase) ||
                        receipt.AddressLine2.Contains(sender.Text, StringComparison.OrdinalIgnoreCase) ||
                        receipt.PaymentMethod.Contains(sender.Text, StringComparison.OrdinalIgnoreCase)
                    // Add more properties to search as needed
                    ).ToList();

                    // Set the filtered list as the ItemsSource for your data grid
                    receiptDataGrid.ItemsSource = filteredReceipts;
                }
            }
        }

        private void tbxBuyerID_TextChanged(object sender, TextChangedEventArgs e)
        {
            // Update the buyer ID whenever the text in the TextBox changes
            int.TryParse(tbxBuyerID.Text, out int newBuyerID);
            BuyerID = newBuyerID;

            // Call GetBuyerProductReceipts with the new buyer ID and update the data grid
            receiptDataGrid.ItemsSource = GetBuyerProductReceipts(BuyerID);
        }
    } 
    }
