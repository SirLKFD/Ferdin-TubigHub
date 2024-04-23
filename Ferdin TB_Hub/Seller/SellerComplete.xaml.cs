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
using Windows.Storage.Pickers;
using OfficeOpenXml;
using Windows.Storage;
using Windows.Storage.Provider;


// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace Ferdin_TB_Hub.Seller
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class SellerComplete : Page
    {
        public List<ProductReceipt> ProductReceipts { get; set; }

        public SellerComplete()
        {
            try
            {
                this.InitializeComponent();
                LoadProductReceipts();
                SetTextValues();

                receiptDataGrid.ItemsSource = ProductReceipts;
                receiptDataGrid.Columns["ProductPrice"].Format = "₱0.00";
                receiptDataGrid.Columns["OrderNumber"].Format = "000000000000";

                // Modify the header names
                receiptDataGrid.Columns["OrderNumber"].ColumnName = "Order Number";
                receiptDataGrid.Columns["ProductName"].ColumnName = "Product Name";
                receiptDataGrid.Columns["ProductCategory"].ColumnName = "Product Category";
                receiptDataGrid.Columns["LastName"].ColumnName = "Last Name";
                receiptDataGrid.Columns["FirstName"].ColumnName = "First Name";
                receiptDataGrid.Columns["MiddleName"].ColumnName = "Middle Name";
                receiptDataGrid.Columns["PhoneNumber"].ColumnName = "Phone Number";
                receiptDataGrid.Columns["AddressLine1"].ColumnName = "Address Line 1";
                receiptDataGrid.Columns["AddressLine2"].ColumnName = "Address Line 2";
                receiptDataGrid.Columns["DatePurchased"].ColumnName = "Date Purchased";

            }
            catch (Exception ex)
            {
                
               
            }
        }


        private void LoadProductReceipts()
        {
            ProductReceipts = Database.GetProductReceipts();
        }

        private void SetTextValues()
        {
            // Set the total purchases
            tbxTotalPurchases.Text = ProductReceipts.Count.ToString();

            // Calculate the best selling category
            var categoryCount = ProductReceipts.GroupBy(r => r.ProductCategory)
                                               .Select(g => new { Category = g.Key, Count = g.Count() })
                                               .OrderByDescending(x => x.Count)
                                               .FirstOrDefault();
            tbxBestSelling.Text = categoryCount != null ? categoryCount.Category : "N/A";

            // Calculate the most purchased product
            var mostPurchasedProduct = ProductReceipts.GroupBy(r => r.ProductName)
                                                      .Select(g => new { ProductName = g.Key, Count = g.Count() })
                                                      .OrderByDescending(x => x.Count)
                                                      .FirstOrDefault();
            tbxMostPurchased.Text = mostPurchasedProduct != null ? mostPurchasedProduct.ProductName : "N/A";

            // Calculate the total sales with tax
            double totalSales = ProductReceipts.Sum(r => r.ProductPrice);
            double salesTax = totalSales * 0.12; // Assuming 12% sales tax
            double totalSalesWithTax = totalSales + salesTax;
            tbxTotalSales.Text = $"₱{totalSalesWithTax.ToString("N2")}"; // Display as PHP currency with two decimal places
        }

        private void AutoSuggestBox_TextChanged(AutoSuggestBox sender, AutoSuggestBoxTextChangedEventArgs args)
        {
            if (args.Reason == AutoSuggestionBoxTextChangeReason.UserInput)
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

        private void cbxShowA_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // Get the selected item from the ComboBox
            ComboBoxItem selectedItem = cbxShowA.SelectedItem as ComboBoxItem;
            if (selectedItem != null)
            {
                // Get the content of the selected item
                string selectedItemContent = selectedItem.Content.ToString();

                // Set the visibility of relevant TextBoxes and TextBlocks based on the selected item
                switch (selectedItemContent)
                {
                    case "Total Purchases":
                        tbxTotalPurchases.Visibility = Visibility.Visible;
                        lblTotalPurchases.Visibility = Visibility.Visible;
                        tbxMostPurchased.Visibility = Visibility.Collapsed;
                        lblMostPurchased.Visibility = Visibility.Collapsed;
                        tbxBestSelling.Visibility = Visibility.Collapsed;
                        lblBestSelling.Visibility = Visibility.Collapsed;
                        break;

                    case "Best Selling Category":
                        tbxTotalPurchases.Visibility = Visibility.Collapsed;
                        lblTotalPurchases.Visibility = Visibility.Collapsed;
                        tbxMostPurchased.Visibility = Visibility.Collapsed;
                        lblMostPurchased.Visibility = Visibility.Collapsed;
                        tbxBestSelling.Visibility = Visibility.Visible;
                        lblBestSelling.Visibility = Visibility.Visible;
                        break;
                    case "Most Purchased Product":
                        tbxTotalPurchases.Visibility = Visibility.Collapsed;
                        lblTotalPurchases.Visibility = Visibility.Collapsed;
                        tbxMostPurchased.Visibility = Visibility.Visible;
                        lblMostPurchased.Visibility = Visibility.Visible;
                        tbxBestSelling.Visibility = Visibility.Collapsed;
                        lblBestSelling.Visibility = Visibility.Collapsed;
                        break;
                    default:
                        break;
                }
            }
        }

        private void cbxShowA_Loaded(object sender, RoutedEventArgs e)
        {
            // Select the first item by default
            cbxShowA.SelectedItem = cbxShowA.Items.FirstOrDefault();
        }

        private void cbxTable_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // Get the selected item from the ComboBox
            ComboBoxItem selectedItem = cbxTable.SelectedItem as ComboBoxItem;
            if (selectedItem != null)
            {
                // Get the content of the selected item
                string selectedItemContent = selectedItem.Content.ToString();

                // Hide all columns initially
                foreach (var column in receiptDataGrid.Columns)
                {
                    column.Visible = false;
                }

                // Always show the OrderNumber column
                receiptDataGrid.Columns["OrderNumber"].Visible = true;

                // Show columns based on the selected item
                switch (selectedItemContent)
                {
                    case "All":
                        // Show all columns
                        foreach (var column in receiptDataGrid.Columns)
                        {
                            column.Visible = true;
                        }
                        break;
                    case "Product":
                        // Show only the specified columns
                        receiptDataGrid.Columns["ProductName"].Visible = true;
                        receiptDataGrid.Columns["ProductCategory"].Visible = true;
                        receiptDataGrid.Columns["ProductPrice"].Visible = true;
                        break;
                    case "Customer Info":
                        // Show only customer information columns
                        receiptDataGrid.Columns["LastName"].Visible = true;
                        receiptDataGrid.Columns["FirstName"].Visible = true;
                        receiptDataGrid.Columns["MiddleName"].Visible = true;
                        receiptDataGrid.Columns["PhoneNumber"].Visible = true;
                        receiptDataGrid.Columns["Email"].Visible = true;
                        break;
                    case "Addresses":
                        // Show only address columns
                        receiptDataGrid.Columns["AddressLine1"].Visible = true;
                        receiptDataGrid.Columns["AddressLine2"].Visible = true;
                        break;
                    case "Payment Details":
                        // Show only payment details columns
                        receiptDataGrid.Columns["PaymentMethod"].Visible = true;
                        receiptDataGrid.Columns["DatePurchased"].Visible = true;
                        break;
                    default:
                        break;
                }

                // Apply ID sorting
                ApplyIDSorting();
            }
        }



        private void ApplyIDSorting()
        {
            string buyerID = tbxSortBuyerID.Text.Trim();
            string sellerID = tbxSortSellerID.Text.Trim();

            // If both text boxes are blank, show all
            if (string.IsNullOrWhiteSpace(buyerID) && string.IsNullOrWhiteSpace(sellerID))
            {
                receiptDataGrid.ItemsSource = ProductReceipts;
                return;
            }

            // Filter the ProductReceipts based on buyerID and sellerID
            var filteredReceipts = ProductReceipts.Where(receipt =>
            {
                string buyerIDAsString = receipt.Buyer_ID.ToString();
                string sellerIDAsString = receipt.Seller_ID.ToString();

                bool matchBuyer = string.IsNullOrWhiteSpace(buyerID) || buyerIDAsString.Contains(buyerID, StringComparison.OrdinalIgnoreCase);
                bool matchSeller = string.IsNullOrWhiteSpace(sellerID) || sellerIDAsString.Contains(sellerID, StringComparison.OrdinalIgnoreCase);
                return matchBuyer && matchSeller;
            }).ToList();

            // Set the filtered list as the ItemsSource for your data grid
            receiptDataGrid.ItemsSource = filteredReceipts;
        }


        private void tbxSortBuyerID_TextChanged(object sender, TextChangedEventArgs e)
        {
            ApplyIDSorting();
        }

        private void tbxSortSellerID_TextChanged(object sender, TextChangedEventArgs e)
        {
            ApplyIDSorting();
        }



        private void btnExport_Click(object sender, RoutedEventArgs e)
        {
            ExportToExcel();
        }


        private async void ExportToExcel()
        {
            bool isFileSaved = false;

        
            // Create a FileSavePicker
            FileSavePicker savePicker = new FileSavePicker();
            savePicker.SuggestedStartLocation = PickerLocationId.DocumentsLibrary;
            savePicker.FileTypeChoices.Add("Excel Workbook", new List<string>() { ".xlsx" });
            savePicker.SuggestedFileName = "ProductReceipts";

            // Show the FileSavePicker
            StorageFile file = await savePicker.PickSaveFileAsync();
            if (file != null)
            {
                using (Stream stream = await file.OpenStreamForWriteAsync())
                {
                    // Create the ExcelPackage
                    using (ExcelPackage excelPackage = new ExcelPackage())
                    {
                        // Create the worksheet
                        ExcelWorksheet worksheet = excelPackage.Workbook.Worksheets.Add("ProductReceipts");

                        // Headers
                        worksheet.Cells[1, 1].Value = "Order Number";
                        worksheet.Cells[1, 2].Value = "Product Name";
                        worksheet.Cells[1, 3].Value = "Product Category";
                        worksheet.Cells[1, 4].Value = "Price";
                        worksheet.Cells[1, 5].Value = "Last Name";
                        worksheet.Cells[1, 6].Value = "First Name";
                        worksheet.Cells[1, 7].Value = "Middle Name";
                        worksheet.Cells[1, 8].Value = "Phone Number";
                        worksheet.Cells[1, 9].Value = "Address Line 1";
                        worksheet.Cells[1, 10].Value = "Address Line 2";
                        worksheet.Cells[1, 11].Value = "Payment";
                        worksheet.Cells[1, 12].Value = "Email";
                        worksheet.Cells[1, 13].Value = "Date Purchased";

                        // Data
                        int row = 2;
                        foreach (var receipt in ProductReceipts)
                        {
                            worksheet.Cells[row, 1].Value = receipt.OrderNumber;
                            worksheet.Cells[row, 2].Value = receipt.ProductName;
                            worksheet.Cells[row, 3].Value = receipt.ProductCategory;
                            worksheet.Cells[row, 4].Value = receipt.ProductPrice;
                            worksheet.Cells[row, 6].Value = receipt.LastName;
                            worksheet.Cells[row, 5].Value = receipt.FirstName;
                            worksheet.Cells[row, 7].Value = receipt.MiddleName;
                            worksheet.Cells[row, 8].Value = receipt.PhoneNumber;
                            worksheet.Cells[row, 9].Value = receipt.AddressLine1;
                            worksheet.Cells[row, 10].Value = receipt.AddressLine2;
                            worksheet.Cells[row, 11].Value = receipt.Email;
                            worksheet.Cells[row, 12].Value = receipt.PaymentMethod;
                            worksheet.Cells[row, 13].Value = receipt.DatePurchased;

                            worksheet.Cells[row, 13].Style.Numberformat.Format = "yyyy/mm/dd hh:mm:ss";

                            row++;
                        }

                        // Insert additional information above the table
                        worksheet.Cells[2, 15].Value = "Total Sales:";
                        worksheet.Cells[2, 16].Value = tbxTotalSales.Text;
                        worksheet.Cells[4, 15].Value = "Total Purchases:";
                        worksheet.Cells[4, 16].Value = tbxTotalPurchases.Text;
                        worksheet.Cells[6, 15].Value = "Best Selling Category:";
                        worksheet.Cells[6, 16].Value = tbxBestSelling.Text;
                        worksheet.Cells[8, 15].Value = "Most Purchased Product:";
                        worksheet.Cells[8, 16].Value = tbxMostPurchased.Text;
                        worksheet.Cells[10, 15].Value = "Generated Date:";
                        worksheet.Cells[10, 16].Value = DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss");

                        // Apply sorting before exporting
                        ApplyIDSorting();

                        // Save the ExcelPackage to the selected file
                        await stream.FlushAsync();
                        stream.Position = 0;
                        await excelPackage.SaveAsAsync(stream);

                        isFileSaved = true;

                        // Check if the file was saved successfully
                        if (isFileSaved)
                        {
                            // Notify the user about successful file saving
                            Buttons.ShowMessage("File saved successfully.");
                        }
                        else
                        {
                            // Notify the user about unsuccessful file saving
                            Buttons.ShowMessage("File failed to saved.");
                        }
                    }
                }      
            
            }



        }

        private void btnRefresh_Click(object sender, RoutedEventArgs e)
        {
            // Clear Buyer ID and Seller ID text boxes
            tbxSortBuyerID.Text = "";
            tbxSortSellerID.Text = "";

            // Reset ComboBoxes to their default values
            cbxShowA.SelectedIndex = 0;
            cbxTable.SelectedIndex = 0;

            // Reload original ProductReceipts data
            LoadProductReceipts();

            // Reset visibility settings based on default selection
            cbxShowA_SelectionChanged(null, null);

            // Set receiptDataGrid's ItemsSource back to original ProductReceipts
            receiptDataGrid.ItemsSource = ProductReceipts;
        }
    }


}



