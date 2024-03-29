using Ferdin_TB_Hub.Classes;
using Ferdin_TB_Hub.HomePage_NavigationView;
using Ferdin_TB_Hub.NewAccount;
using Ferdin_TB_Hub.Seller;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Animation;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;
using static Ferdin_TB_Hub.Classes.Database;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace Ferdin_TB_Hub
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        public MainPage()
        {
            this.InitializeComponent();
            LoadMainPageImages();
        }

        private void ScrollViewer_ViewChanged(object sender, ScrollViewerViewChangedEventArgs e)
        {

        }

        private void RevealModeCheckbox_Changed(object sender, RoutedEventArgs e)
        {
            if (RevealPassCheck.IsChecked == true)
            {
                RevealPassMode.PasswordRevealMode = PasswordRevealMode.Visible;
            }
            else
            {
                RevealPassMode.PasswordRevealMode = PasswordRevealMode.Hidden;
            }

        }

        private void GoToHomePage(object sender, RoutedEventArgs e)
        {
            string usernameOrEmail = tbxUsernameorEmail.Text.Trim();
            string password = RevealPassMode.Password.Trim();

            if (string.IsNullOrEmpty(usernameOrEmail) || string.IsNullOrEmpty(password))
            {
                Buttons.ShowMessage("Please complete the fields.");
                return;
            }

            if (rbnBuyer.IsChecked == true)
            {
                // Check if the buyer exists in the database
                if (Database.IsBuyerExists(usernameOrEmail, password))
                {
                    // Retrieve buyer information from the database
                    BuyerDetails buyer = Database.GetBuyerByUsernameOrEmail(usernameOrEmail);
                    if (buyer != null)
                    {
                        Frame.Navigate(typeof(HomePage), buyer, new SlideNavigationTransitionInfo() { Effect = SlideNavigationTransitionEffect.FromRight });
                    }
                    else
                    {
                        // Buyer not found in the database
                        Buttons.ShowMessage("Buyer doesn't exist, please try again, or create an account.");
                    }
                }
                else
                {
                    // Buyer does not exist or invalid credentials, show an error message or handle it accordingly
                    Buttons.ShowMessage("Buyer doesn't exist, please try again, or create an account.");
                }
            }

            else if (rbnSeller.IsChecked == true)
            {
                // Check if the seller exists in the database
                if (Database.IsSellerExists(usernameOrEmail, password))
                {
                    // Retrieve seller information from the database
                    SellerDetails seller = Database.GetSellerByUsernameOrEmail(usernameOrEmail);
                    if (seller != null)
                    {
                        // Navigate to SellerAccount page and pass seller information as parameter
                        Frame.Navigate(typeof(SellerAccount), seller, new SlideNavigationTransitionInfo() { Effect = SlideNavigationTransitionEffect.FromRight });
                    }
                    else
                    {
                        // Seller not found in the database
                        Buttons.ShowMessage("Seller doesn't exist, please try again, or create an account.");
                    }
                }
                else
                {
                    // Seller does not exist or invalid credentials, show an error message or handle it accordingly
                    Buttons.ShowMessage("Seller doesn't exist, please try again, or create an account.");
                }
            }
        }



        private void GoToCreateAccount(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(CreateAccountPage), null, new SlideNavigationTransitionInfo() { Effect = SlideNavigationTransitionEffect.FromBottom });
        }

        private async void LoadMainPageImages()
        {
            ObservableCollection<StorageFile> imageFiles = new ObservableCollection<StorageFile>
        {

            await Windows.ApplicationModel.Package.Current.InstalledLocation.GetFileAsync("WaterSample1.jpg"),
            await Windows.ApplicationModel.Package.Current.InstalledLocation.GetFileAsync("WaterSample2.jpg"),
            await Windows.ApplicationModel.Package.Current.InstalledLocation.GetFileAsync("WaterSample3.jpg"),
            // Add more file paths as needed
        };

            foreach (StorageFile file in imageFiles)
            {
                BitmapImage bitmapImage = new BitmapImage();
                using (var stream = await file.OpenAsync(FileAccessMode.Read))
                {
                    await bitmapImage.SetSourceAsync(stream);
                }

                Image image = new Image();
                image.Source = bitmapImage;
                MainPage_FlipView.Items.Add(image);
            }
        }
    
        
    }
}

