using Ferdin_TB_Hub.NewAccount;
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
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;

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
            Frame.Navigate(typeof(HomePage));
        }

        private void GoToCreateAccount(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(CreateAccountPage));
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

