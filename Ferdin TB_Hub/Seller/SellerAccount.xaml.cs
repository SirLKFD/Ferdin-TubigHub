﻿using Ferdin_TB_Hub.Classes;
using Ferdin_TB_Hub.HomePage_NavigationView;
using System;
using System.ComponentModel;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
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
            get => _seller;
            set
            {
                if (_seller != value)
                {
                    _seller = value;
                    OnPropertyChanged(nameof(Seller));
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public SellerAccount()
        {
            try
            {
                InitializeComponent();
                DataContext = this; // Set the DataContext of the page to itself
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

                // Check if the parameter passed during navigation is a SellerDetails object
                if (e.Parameter != null && e.Parameter is SellerDetails)
                {
                    // Cast the parameter to SellerDetails and assign it to the Seller property
                    Seller = e.Parameter as SellerDetails;
                    // Notify the UI that the Seller property has changed
                    OnPropertyChanged(nameof(Seller));
                }
            }
            catch
            {

            }

        }

        private async void Logout_Click(object sender, RoutedEventArgs e)
        {
            try
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
                    _ = Frame.Navigate(typeof(MainPage), null, new SlideNavigationTransitionInfo() { Effect = SlideNavigationTransitionEffect.FromLeft });
                }
                else
                {
                    // User chose not to logout
                }
            }
            catch (Exception ex)
            {
                Buttons.ShowPrompt(ex.Message);
            }

        }

        private async void UpdateSellerInfo_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // Check if any textbox is empty
                if (string.IsNullOrWhiteSpace(tbxStore.Text) || string.IsNullOrWhiteSpace(tbxEmail.Text) ||
                    string.IsNullOrWhiteSpace(tbxUsername.Text) || string.IsNullOrWhiteSpace(tbxLastName.Text) ||
                    string.IsNullOrWhiteSpace(tbxFirstName.Text) || string.IsNullOrWhiteSpace(tbxMiddleName.Text) ||
                    string.IsNullOrWhiteSpace(tbxPassword.Password) || string.IsNullOrWhiteSpace(tbxPhoneNumber.Text) ||
                    string.IsNullOrWhiteSpace(tbxAddressLine1.Text) || string.IsNullOrWhiteSpace(tbxAddressLine2.Text))
                {
                    // Display error message
                    Buttons.ShowPrompt("Please fill in all fields.");
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
                if (!int.TryParse(tbxID.Text, out int seller_id))
                {
                    // Handle invalid input (e.g., display an error message)
                    Buttons.ShowPrompt("Invalid seller ID.");
                    return;
                }

                /*
                // Check if the username or email already exists
                if (Database.IsSellerAlreadyExists(username, email, businessName))
                {
                    // Show an error message indicating that the username or email is already taken
                    Buttons.ShowPrompt("Username, email, and store already exists. Please choose another one.");
                    return; // Exit the method without proceeding further
                }*/

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
            catch (Exception ex)
            {
                Buttons.ShowPrompt(ex.Message);
            }

        }



        private async void DeleteSellerAccount_Click(object sender, RoutedEventArgs e)
        {
            try
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
                    _ = Frame.Navigate(typeof(MainPage), null, new SlideNavigationTransitionInfo() { Effect = SlideNavigationTransitionEffect.FromLeft });
                }
            }
            catch
            (Exception ex)
            {
                Buttons.ShowPrompt(ex.Message);
            }

        }

        private void NavigateToLocatePage()
        {
            try
            {
                // Pass the Seller object to the Locate page when navigating
                _ = Frame.Navigate(typeof(Locate), Seller);
            }
            catch
            (Exception ex)
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
                    case "ProductPage":
                        _ = contentFrame.Navigate(typeof(YourWaterProducts));
                        break;

                    case "AddPage":
                        _ = contentFrame.Navigate(typeof(AddProduct));
                        break;
                    case "SellerCompletePage":
                        _ = contentFrame.Navigate(typeof(SellerComplete));
                        break;
                }
            }
            catch
            (Exception ex)
            {
                Buttons.ShowPrompt(ex.Message);
            }


        }

        private void NavigateToAddProduct()
        {
            try
            {
                // Pass SellerID as parameter when navigating to AddProduct page
                _ = Frame.Navigate(typeof(AddProduct), tbxID.Text);
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
                _ = contentFrame.Navigate(typeof(YourWaterProducts));
            }
            catch (Exception ex)
            {
                Buttons.ShowPrompt(ex.Message);
            }
        }

        private void ToggledModifyAccount(object sender, RoutedEventArgs e)
        {
            try
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
            catch (Exception ex)
            {
                Buttons.ShowPrompt(ex.Message);
            }

        }
    }
}