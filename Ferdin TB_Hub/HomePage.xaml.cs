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
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace Ferdin_TB_Hub
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class HomePage : Page
    {
        private List<Page> pageInstances = new List<Page>();
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
                    contentFrame.Navigate(typeof(AccountBuyer));
                    break;
                case "Home":
                    contentFrame.Navigate(typeof(Home));
                    break;
                case "Browse":
                    contentFrame.Navigate(typeof(BrowseWater));
                    break;
                case "Cart":
                    contentFrame.Navigate(typeof(Cart));
                    break;

            }
        }
        private void GoingBack(NavigationView sender, NavigationViewBackRequestedEventArgs args)
        {
            if (pageInstances.Count > 1)
            {
                // Navigate to the previous instance of the page
                Frame.Navigate(pageInstances[pageInstances.Count - 2].GetType());
            }
            else
            {
                // Navigate to a different page or handle as needed when there's only one instance
                Frame.Navigate(typeof(HomePage));
            }
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

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
