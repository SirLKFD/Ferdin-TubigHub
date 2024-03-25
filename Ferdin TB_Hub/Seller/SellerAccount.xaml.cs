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

namespace Ferdin_TB_Hub.Seller
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class SellerAccount : Page
    {

        public SellerAccount()
        {
            this.InitializeComponent();

        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            // Check if the parameter passed during navigation is a SellerDetails object
            if (e.Parameter != null && e.Parameter is SellerDetails)
            {
                // Cast the parameter to SellerDetails
                SellerDetails seller = e.Parameter as SellerDetails;

                // Populate the text boxes with seller information
                tbxStore.Text = seller.BusinessName;
                tbxUsername.Text = seller.Username;
                tbxFirstName.Text = seller.FirstName;
                tbxMiddleName.Text = seller.MiddleName;
                tbxLastName.Text = seller.LastName;
                tbxPhoneNumber.Text = seller.PhoneNumber;
                tbxAddressLine1.Text = seller.AddressLine1;
                tbxAddressLine2.Text = seller.AddressLine2;
                tbxEmail.Text = seller.Email;
                tbxPassword.Text = seller.Password;
                tbxID.Text = seller.Id.ToString();
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
                Frame.Navigate(typeof(MainPage));
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
                string.IsNullOrWhiteSpace(tbxPassword.Text) || string.IsNullOrWhiteSpace(tbxPhoneNumber.Text) ||
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
            string password = tbxPassword.Text;
            string phoneNumber = tbxPhoneNumber.Text;
            string addressLine1 = tbxAddressLine1.Text;
            string addressLine2 = tbxAddressLine2.Text;

            // Retrieve the seller's Id from the text box
            int id;
            if (!int.TryParse(tbxID.Text, out id))
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
                Database.UpdateSellerInfoFromDatabase(id, businessName, email, username, lastName, firstName, middleName, password, phoneNumber, addressLine1, addressLine2);
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

                // Navigate back to the main page or log out
                Frame.Navigate(typeof(MainPage));
            }
        }


    }
}
