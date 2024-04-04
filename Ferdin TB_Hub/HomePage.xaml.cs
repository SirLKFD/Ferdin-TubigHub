using Ferdin_TB_Hub.Classes;
using Ferdin_TB_Hub.HomePage_NavigationView;
using Ferdin_TB_Hub.NewAccount;
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
using Windows.UI.Xaml.Media.Animation;
using Windows.UI.Xaml.Navigation;
using static Ferdin_TB_Hub.Classes.Database;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace Ferdin_TB_Hub
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class HomePage : Page
    {
        private List<Page> pageInstances = new List<Page>();

        //PASSING DATABASE
        private BuyerDetails _buyer;
        private ProductDetails _productdetails;
        private StoreAddress_Availability _storeAddressAvailability;


        public HomePage()
        {
            this.InitializeComponent();
        }

        private void NavigationView_Loaded(object sender, RoutedEventArgs e)
        {
            contentFrame.Navigate(typeof(Home));
        }

        private void NavigationView_SelectionChanged(NavigationView sender, NavigationViewSelectionChangedEventArgs args)
        {
            NavigationViewItem item = args.SelectedItem as NavigationViewItem;

            switch (item.Tag.ToString())
            {
                case "Locate":                  
                    contentFrame.Navigate(typeof(Locate));
                    break;
                case "Account":
                    //THE BUYER INFO WILL BE PASSED TO THE ACCOUNT BUYER
                    contentFrame.Navigate(typeof(AccountBuyer), _buyer);
                    break;
                case "Home":
                    contentFrame.Navigate(typeof(Home));
                    break;               
                case "Cart":
                    contentFrame.Navigate(typeof(Cart));
                    break;

            }
        }
        private void GoingBack(NavigationView sender, NavigationViewBackRequestedEventArgs args)
        {
           // Create a logic that will go back to the previous page when the back button is clicked
            if (contentFrame.CanGoBack)
            {
                // Fix the code below

                contentFrame.GoBack();
            }
          
           
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            if (e.Parameter != null && e.Parameter is BuyerDetails)
            {
                _buyer = e.Parameter as BuyerDetails;
            }

            // Add the current instance of the page to the collection
            pageInstances.Add(this);

        }

        protected override void OnNavigatingFrom(NavigatingCancelEventArgs e)
        {
            base.OnNavigatingFrom(e);

            // Remove the current instance of the page from the collection
            pageInstances.Remove(this);
        }
    }
}
