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

namespace Ferdin_TB_Hub.HomePage_NavigationView
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class AccountBuyer : Page
    {
        public AccountBuyer()
        {
            this.InitializeComponent();
        }

        private void Logout_Click(object sender, RoutedEventArgs e)
        {
            if (AccountBar != null)
            {
                AccountBar.IsPaneVisible = false;
            }

            // Navigate directly to MainPage without adding to back stack
            Frame rootFrame = Window.Current.Content as Frame;
            rootFrame.Navigate(typeof(MainPage));
        }
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            // Check if the parameter passed during navigation is a BuyerDetails object
            if (e.Parameter != null && e.Parameter is BuyerDetails)
            {
                // Cast the parameter to BuyerDetails
                BuyerDetails buyer = e.Parameter as BuyerDetails;

                // Populate the text boxes with buyer information
                tbxUsername.Text = buyer.Username;
                tbxFirstName.Text = buyer.FirstName;
                tbxMiddleName.Text = buyer.MiddleName;
                tbxLastName.Text = buyer.LastName;
                tbxPhoneNumber.Text = buyer.PhoneNumber;
                tbxAddressLine1.Text = buyer.AddressLine1;
                tbxAddressLine2.Text = buyer.AddressLine2;
                tbxEmail.Text = buyer.Email;
                tbxPassword.Text = buyer.Password;
                tbxID.Text = buyer.Id.ToString();
            }
        }

        private async void DeleteBuyerAccount_Click(object sender, RoutedEventArgs e)
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
                // Perform deletion of the buyer account
                Database.DeleteBuyerAccountFromDatabase(tbxUsername.Text); // Assuming tbxUsername contains the buyer's username

                // Navigate back to the main page or log out
                Frame.Navigate(typeof(MainPage));
            }
        }

        private async void UpdateBuyerInfo_Click(object sender, RoutedEventArgs e)
        {
            // Check if any textbox is empty
            if (string.IsNullOrWhiteSpace(tbxUsername.Text) || string.IsNullOrWhiteSpace(tbxLastName.Text) ||
                string.IsNullOrWhiteSpace(tbxFirstName.Text) || string.IsNullOrWhiteSpace(tbxMiddleName.Text) ||
                string.IsNullOrWhiteSpace(tbxPassword.Text) || string.IsNullOrWhiteSpace(tbxPhoneNumber.Text) ||
                string.IsNullOrWhiteSpace(tbxAddressLine1.Text) || string.IsNullOrWhiteSpace(tbxAddressLine2.Text))
            {
                // Display error message
                Buttons.ShowMessage("Please fill in all fields.");
                return;
            }

            // Extract updated information from TextBoxes
            string email = tbxEmail.Text;
            string username = tbxUsername.Text;
            string lastName = tbxLastName.Text;
            string firstName = tbxFirstName.Text;
            string middleName = tbxMiddleName.Text;
            string password = tbxPassword.Text;
            string phoneNumber = tbxPhoneNumber.Text;
            string addressLine1 = tbxAddressLine1.Text;
            string addressLine2 = tbxAddressLine2.Text;

            // Retrieve the buyer's Id from the text box
            int id;
            if (!int.TryParse(tbxID.Text, out id))
            {
                // Handle invalid input (e.g., display an error message)
                return;
            }

            // Create a confirmation dialog
            ContentDialog confirmDialog = new ContentDialog
            {
                Title = "Update Buyer Information",
                Content = "Are you sure you want to update your information?",
                PrimaryButtonText = "Yes",
                CloseButtonText = "No"
            };

            // Show the confirmation dialog and wait for user response
            ContentDialogResult result = await confirmDialog.ShowAsync();

            // Process the user's response
            if (result == ContentDialogResult.Primary)
            {
                // Call the method to update buyer info in the database using Id
                Database.UpdateBuyerInfoFromDatabase(id, email, username, lastName, firstName, middleName, password, phoneNumber, addressLine1, addressLine2);
            }
            else
            {
                // User chose not to update, do nothing or provide feedback
            }
        }
    }
    
}
 