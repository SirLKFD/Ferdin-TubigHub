using Ferdin_TB_Hub.BuyerAccountPage;
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
        private BuyerDetails _buyer;
        private ProductDetails _productdetails;
        


        public HomePage()
        {
            try
            {
                this.InitializeComponent();
            }
            catch (Exception ex)
            {
                Buttons.ShowPrompt(ex.Message);
            }
        }

        private void NavigationView_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                contentFrame.Navigate(typeof(Home));

                // Select the "Home" NavigationViewItem
                foreach (NavigationViewItemBase item in navigationView.MenuItems)
                {
                    if (item is NavigationViewItem && (item as NavigationViewItem).Tag.ToString() == "Home")
                    {
                        navigationView.SelectedItem = item;
                        break;
                    }
                }
            }
            catch (Exception ex)
            {
                Buttons.ShowPrompt(ex.Message);
            }
        }




        private void NavigationView_SelectionChanged(NavigationView sender, NavigationViewSelectionChangedEventArgs args)
        {
            try
            {
                NavigationViewItem item = args.SelectedItem as NavigationViewItem;

                switch (item.Tag.ToString())
                {
                    case "Locate":
                        contentFrame.Navigate(typeof(Locate), _buyer);
                        break;
                    case "Account":
                        //THE BUYER INFO WILL BE PASSED TO THE ACCOUNT BUYER
                        contentFrame.Navigate(typeof(AccountBuyer), _buyer);
                        break;
                    case "Home":
                        contentFrame.Navigate(typeof(Home), _buyer);
                        break;
                    case "Cart":
                        contentFrame.Navigate(typeof(Cart), _buyer);
                        break;
                    case "History":
                        contentFrame.Navigate(typeof(OrderHistory), _buyer);
                        break;

                }
            }
            catch (Exception ex)
            {
                Buttons.ShowPrompt(ex.Message);
            }
          
        }
        private void GoingBack(NavigationView sender, NavigationViewBackRequestedEventArgs args)
        {
            try
            {
                // Create a logic that will go back to the previous page when the back button is clicked
                if (contentFrame.CanGoBack)
                {                  
                    contentFrame.GoBack();
                }
               
            }
            catch (Exception ex)
            {
                Buttons.ShowPrompt(ex.Message);
            }
                         
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            try
            {
                base.OnNavigatedTo(e);

                if (e.Parameter != null && e.Parameter is BuyerDetails)
                {
                    _buyer = e.Parameter as BuyerDetails;
                }

                // Add the current instance of the page to the collection
                pageInstances.Add(this);
            }
            catch (Exception ex)
            {
                Buttons.ShowPrompt(ex.Message);
            }
          

        }
        protected override void OnNavigatingFrom(NavigatingCancelEventArgs e)
        {
            try
            {
                base.OnNavigatingFrom(e);

                // Remove the current instance of the page from the collection
                pageInstances.Remove(this);
            }
            catch (Exception ex)
            {
                Buttons.ShowPrompt(ex.Message);
            }      
        }
    }
}
