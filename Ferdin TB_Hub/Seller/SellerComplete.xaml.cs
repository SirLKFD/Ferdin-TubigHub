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
            this.InitializeComponent();
            LoadProductReceipts();
            SetTextValues();
        }

        private void LoadProductReceipts()
        {
            // Fetch product receipts from the database
            ProductReceipts = Database.GetProductReceipts();

            // Notify the UI that the data has changed
            ReceiptListView.ItemsSource = ProductReceipts;
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
                // Filter the products based on user input
                var filteredProducts = ProductReceipts.Where(receipt =>
                 receipt.OrderNumber.ToString().Contains(sender.Text, StringComparison.OrdinalIgnoreCase) ||
                    receipt.ProductName.Contains(sender.Text, StringComparison.OrdinalIgnoreCase) ||
                    receipt.ProductCategory.Contains(sender.Text, StringComparison.OrdinalIgnoreCase) ||
                    receipt.ProductPrice.ToString().Contains(sender.Text, StringComparison.OrdinalIgnoreCase) ||
                    receipt.ProductQuantity.ToString().Contains(sender.Text, StringComparison.OrdinalIgnoreCase) ||
                    receipt.LastName.ToString().Contains(sender.Text, StringComparison.OrdinalIgnoreCase) ||
                    receipt.MiddleName.ToString().Contains(sender.Text, StringComparison.OrdinalIgnoreCase) ||
                    receipt.FirstName.ToString().Contains(sender.Text, StringComparison.OrdinalIgnoreCase) ||
                    receipt.Email.ToString().Contains(sender.Text, StringComparison.OrdinalIgnoreCase) ||
                    receipt.PhoneNumber.ToString().Contains(sender.Text, StringComparison.OrdinalIgnoreCase) ||
                    receipt.AddressLine1.ToString().Contains(sender.Text, StringComparison.OrdinalIgnoreCase) ||
                    receipt.AddressLine2.ToString().Contains(sender.Text, StringComparison.OrdinalIgnoreCase) ||
                    receipt.PaymentMethod.ToString().Contains(sender.Text, StringComparison.OrdinalIgnoreCase) ||
                    receipt.DatePurchased.ToString().Contains(sender.Text, StringComparison.OrdinalIgnoreCase)




                ).ToList();

                // Update the ListView with filtered items
                ReceiptListView.ItemsSource = filteredProducts;
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


        }

        private void btnExport_Click(object sender, RoutedEventArgs e)
        {
            ExportToExcel();
        }

        private async void ExportToExcel()
        {
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
                        worksheet.Cells[1, 11].Value = "Email";
                        worksheet.Cells[1, 12].Value = "Payment";
                        worksheet.Cells[1, 13].Value = "Date Purchased";



                        // Data
                        int row = 2;
                        foreach (var receipt in ProductReceipts)
                        {
                            worksheet.Cells[row, 1].Value = receipt.OrderNumber;
                            worksheet.Cells[row, 2].Value = receipt.ProductName;
                            worksheet.Cells[row, 3].Value = receipt.ProductCategory;
                            worksheet.Cells[row, 4].Value = receipt.ProductPrice;
                            worksheet.Cells[row, 5].Value = receipt.LastName;
                            worksheet.Cells[row, 6].Value = receipt.FirstName;
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

                        // Save the ExcelPackage to the selected file
                        await stream.FlushAsync();
                        stream.Position = 0;
                        await excelPackage.SaveAsAsync(stream);
                    }
                }
            }
        }

    }
}
