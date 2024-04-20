using Ferdin_TB_Hub.Classes;
using Ferdin_TB_Hub.HomePage_NavigationView;
using Ferdin_TB_Hub.NewAccount;
using System;
using System.Collections.Generic;
using System.ComponentModel;
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

namespace Ferdin_TB_Hub.Seller
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class SellerAccount : Page, INotifyPropertyChanged
    {
        private SellerDetails _seller;

        public SellerDetails Seller
        {
            get { return _seller; }
            set
            {
                if (_seller != value)
                {
                    _seller = value;
                    OnPropertyChanged(nameof(Seller));
                }
            }
        }

        public SellerAccount()
        {
            this.InitializeComponent();
            this.DataContext = this; // Set the DataContext of the page to itself

        }

        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }


        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            // Check if the parameter passed during navigation is a SellerDetails object
            if (e.Parameter != null && e.Parameter is SellerDetails)
            {
                // Cast the parameter to SellerDetails and assign it to the Seller property
                Seller = e.Parameter as SellerDetails;
                // Notify the UI that the Seller property has changed
                OnPropertyChanged(nameof(Seller));
            }
        }

        private async void Logout_Click(object sender, RoutedEventArgs e)
        {
            // Create a confirmation dialog
            ContentDialog confirmDialog = new ContentDialog
            {
                Title = "Logout",
                Content = "Are you sure you want to logout?",
                PrimaryButtonText = "Yes",
                CloseButtonText = "No"
            };

            // Show the confirmation dialog and wait for user response
            ContentDialogResult result = await confirmDialog.ShowAsync();

            // Process the user's response
            if (result == ContentDialogResult.Primary)
            {
                // Navigate back to the MainPage to logout
                _seller = null;
                Frame.Navigate(typeof(MainPage), null, new SlideNavigationTransitionInfo() { Effect = SlideNavigationTransitionEffect.FromLeft });
            }
            else
            {
                // User chose not to logout, do nothing or provide feedback
            }
        }

        private async void UpdateSellerInfo_Click(object sender, RoutedEventArgs e)
        {
            // Check if any textbox is empty
            if (string.IsNullOrWhiteSpace(tbxStore.Text) || string.IsNullOrWhiteSpace(tbxEmail.Text) ||
                string.IsNullOrWhiteSpace(tbxUsername.Text) || string.IsNullOrWhiteSpace(tbxLastName.Text) ||
                string.IsNullOrWhiteSpace(tbxFirstName.Text) || string.IsNullOrWhiteSpace(tbxMiddleName.Text) ||
                string.IsNullOrWhiteSpace(tbxPassword.Password) || string.IsNullOrWhiteSpace(tbxPhoneNumber.Text) ||
                string.IsNullOrWhiteSpace(tbxAddressLine1.Text) || string.IsNullOrWhiteSpace(tbxAddressLine2.Text))
            {
                // Display error message
                Buttons.ShowMessage("Please fill in all fields.");
                return;
            }

            // Extract updated information from TextBoxes
            string businessName = tbxStore.Text;
            string email = tbxEmail.Text;
            string username = tbxUsername.Text;
            string lastName = tbxLastName.Text;
            string firstName = tbxFirstName.Text;
            string middleName = tbxMiddleName.Text;
            string password = tbxPassword.Password;
            string phoneNumber = tbxPhoneNumber.Text;
            string addressLine1 = tbxAddressLine1.Text;
            string addressLine2 = tbxAddressLine2.Text;

            // Retrieve the seller's Id from the text box
            int seller_id;
            if (!int.TryParse(tbxID.Text, out seller_id))
            {
                // Handle invalid input (e.g., display an error message)
                return;
            }

            // Create a confirmation dialog
            ContentDialog confirmDialog = new ContentDialog
            {
                Title = "Update Seller Information",
                Content = "Are you sure you want to update your information?",
                PrimaryButtonText = "Yes",
                CloseButtonText = "No"
            };

            // Show the confirmation dialog and wait for user response
            ContentDialogResult result = await confirmDialog.ShowAsync();

            // Process the user's response
            if (result == ContentDialogResult.Primary)
            {
                // Call the method to update seller info in the database using Id
                Database.UpdateSellerInfoFromDatabase(seller_id, businessName, email, username, lastName, firstName, middleName, password, phoneNumber, addressLine1, addressLine2);
            }
            else
            {
                // User chose not to update, do nothing or provide feedback
            }
        }



        private async void DeleteSellerAccount_Click(object sender, RoutedEventArgs e)
        {
            // Display a confirmation dialog
            ContentDialog deleteConfirmationDialog = new ContentDialog
            {
                Title = "Delete Account",
                Content = "Are you sure you want to proceed? Deleting the account will delete all of your records.",
                PrimaryButtonText = "Yes",
                CloseButtonText = "No"
            };

            ContentDialogResult result = await deleteConfirmationDialog.ShowAsync();

            // If the user confirms deletion
            if (result == ContentDialogResult.Primary)
            {
                // Perform deletion of the seller account
                Database.DeleteSellerAccountFromDatabase(tbxUsername.Text); // Assuming tbxUsername contains the seller's username

                // Navigate back to the main page
                Frame.Navigate(typeof(MainPage), null, new SlideNavigationTransitionInfo() { Effect = SlideNavigationTransitionEffect.FromLeft });
            }
        }

        private void NavigateToLocatePage()
        {
            // Pass the Seller object to the Locate page when navigating
            Frame.Navigate(typeof(Locate), Seller);
        }

        private void NavigationView_SelectionChanged(NavigationView sender, NavigationViewSelectionChangedEventArgs args)
        {
            NavigationViewItem item = args.SelectedItem as NavigationViewItem;

            switch (item.Tag.ToString())
            {
                case "ProductPage":
                    contentFrame.Navigate(typeof(YourWaterProducts));
                    break;

                case "AddPage":
                    contentFrame.Navigate(typeof(AddProduct));
                    break;

       

                case "SellerCompletePage":
                    contentFrame.Navigate(typeof(SellerComplete));
                    break;

             

            }
        }

        private void NavigateToAddProduct()
        {
            // Pass SellerID as parameter when navigating to AddProduct page
            Frame.Navigate(typeof(AddProduct), tbxID.Text);
        }


        private void NavigationView_Loaded(object sender, RoutedEventArgs e)
        {
            contentFrame.Navigate(typeof(YourWaterProducts));
        }

        private void ToggledModifyAccount(object sender, RoutedEventArgs e)
        {
            bool isToggleOn = (sender as ToggleSwitch).IsOn;

            // Enable or disable TextBoxes based on toggle state
            tbxStore.IsEnabled = isToggleOn;
            tbxUsername.IsEnabled = isToggleOn;
            tbxFirstName.IsEnabled = isToggleOn;
            tbxMiddleName.IsEnabled = isToggleOn;
            tbxLastName.IsEnabled = isToggleOn;
            tbxPhoneNumber.IsEnabled = isToggleOn;
            tbxAddressLine1.IsEnabled = isToggleOn;
            tbxAddressLine2.IsEnabled = isToggleOn;
            tbxEmail.IsEnabled = isToggleOn;
            tbxPassword.IsEnabled = isToggleOn;

            tbxPassword.PasswordRevealMode = isToggleOn ? PasswordRevealMode.Visible : PasswordRevealMode.Hidden;


            // Enable or disable buttons based on toggle state
            btnUpdateSellerInfo.IsEnabled = isToggleOn;
            btnDeleteSellerAccount.IsEnabled = isToggleOn;
        }
    }
}