﻿using Ferdin_TB_Hub.Classes;
using Ferdin_TB_Hub.NewAccount;
using Ferdin_TB_Hub.Seller;
using System;
using System.Collections.ObjectModel;
using Windows.Storage;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Animation;
using Windows.UI.Xaml.Media.Imaging;
using static Ferdin_TB_Hub.Classes.Database;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace Ferdin_TB_Hub
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        private readonly DispatcherTimer timer;
        private int colorIndex = 0;
        private readonly SolidColorBrush[] colors = { new SolidColorBrush(Windows.UI.Colors.PowderBlue), new SolidColorBrush(Windows.UI.Colors.CornflowerBlue), new SolidColorBrush(Windows.UI.Colors.SkyBlue), new SolidColorBrush(Windows.UI.Colors.Aquamarine), new SolidColorBrush(Windows.UI.Colors.BlueViolet) };
        public MainPage()
        {
            InitializeComponent();
            LoadMainPageImages();
            ImageAnimation1.Begin();
            ImageAnimation2.Begin();

            // Initialize and start the timer
            timer = new DispatcherTimer
            {
                Interval = TimeSpan.FromSeconds(1) // Change the interval as needed
            };
            timer.Tick += Timer_Tick;
            timer.Start();
        }

        private void Timer_Tick(object sender, object e)
        {
            // Change the background color of the RelativePanel
            relativePanel.Background = colors[colorIndex];
            colorIndex = (colorIndex + 1) % colors.Length;
        }

        private void ScrollViewer_ViewChanged(object sender, ScrollViewerViewChangedEventArgs e)
        {

        }

        private void RevealModeCheckbox_Changed(object sender, RoutedEventArgs e)
        {
            try
            {
                RevealPassMode.PasswordRevealMode = RevealPassCheck.IsChecked == true ? PasswordRevealMode.Visible : PasswordRevealMode.Hidden;
            }
            catch (Exception ex)
            {
                Buttons.ShowPrompt(ex.Message);
            }

        }

        private void GoToHomePage(object sender, RoutedEventArgs e)
        {
            try
            {
                string usernameOrEmail = tbxUsernameorEmail.Text.Trim();
                string password = RevealPassMode.Password.Trim();

                if (string.IsNullOrEmpty(usernameOrEmail) || string.IsNullOrEmpty(password))
                {
                    Buttons.ShowPrompt("Please complete the fields.");
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
                            _ = Frame.Navigate(typeof(HomePage), buyer, new SlideNavigationTransitionInfo() { Effect = SlideNavigationTransitionEffect.FromRight });

                        }
                        else
                        {
                            // Buyer not found in the database
                            Buttons.ShowPrompt("Buyer doesn't exist, please try again, or create an account.");
                        }
                    }
                    else
                    {
                        // Buyer does not exist or invalid credentials, show an error message or handle it accordingly
                        Buttons.ShowPrompt("Buyer doesn't exist, please try again, or create an account.");
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
                            _ = Frame.Navigate(typeof(SellerAccount), seller, new SlideNavigationTransitionInfo() { Effect = SlideNavigationTransitionEffect.FromRight });
                        }
                        else
                        {
                            // Seller not found in the database
                            Buttons.ShowPrompt("Seller doesn't exist, please try again, or create an account.");
                        }
                    }
                    else
                    {
                        // Seller does not exist or invalid credentials, show an error message or handle it accordingly
                        Buttons.ShowPrompt("Seller doesn't exist, please try again, or create an account.");
                    }
                }
                else
                {
                    // Seller not found in the database
                    Buttons.ShowPrompt("Seller doesn't exist, please try again, or create an account.");
                }
            }
            catch (Exception ex)
            {
                Buttons.ShowPrompt(ex.Message);
            }

        }



        private void GoToCreateAccount(object sender, RoutedEventArgs e)
        {
            try
            {
                _ = Frame.Navigate(typeof(CreateAccountPage), null, new SlideNavigationTransitionInfo() { Effect = SlideNavigationTransitionEffect.FromRight });

            }
            catch (Exception ex)
            {
                Buttons.ShowPrompt(ex.Message);
            }
        }

        private async void LoadMainPageImages()
        {
            try
            {
                ObservableCollection<StorageFile> imageFiles = new ObservableCollection<StorageFile>
              {

            await Windows.ApplicationModel.Package.Current.InstalledLocation.GetFileAsync("2.png"),
            await Windows.ApplicationModel.Package.Current.InstalledLocation.GetFileAsync("1.png"),
            await Windows.ApplicationModel.Package.Current.InstalledLocation.GetFileAsync("WaterSample1.jpg"),
            // Add more file paths as needed
                  };

                foreach (StorageFile file in imageFiles)
                {
                    BitmapImage bitmapImage = new BitmapImage();
                    using (Windows.Storage.Streams.IRandomAccessStream stream = await file.OpenAsync(FileAccessMode.Read))
                    {
                        await bitmapImage.SetSourceAsync(stream);
                    }

                    Image image = new Image
                    {
                        Source = bitmapImage
                    };
                    MainPage_FlipView.Items.Add(image);
                }
            }
            catch (Exception ex)
            {
                Buttons.ShowPrompt(ex.Message);
            }
        }

    }
}

