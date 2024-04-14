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

namespace Ferdin_TB_Hub.BuyerAccountPage
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class BuyerComplete : Page
    {
        public List<ProductReceipt> ProductReceipts { get; set; }

        public BuyerComplete()
        {
            this.InitializeComponent();
            LoadProductReceipts();
        }

        private void LoadProductReceipts()
        {
            // Fetch product receipts from the database
            ProductReceipts = Database.GetProductReceipts();

            // Notify the UI that the data has changed
            ReceiptListView.ItemsSource = ProductReceipts;
        }
    }
}
