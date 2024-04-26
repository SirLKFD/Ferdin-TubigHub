using Ferdin_TB_Hub.Classes;
using System.Collections.Generic;
using Windows.UI.Xaml.Controls;
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
            InitializeComponent();
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
