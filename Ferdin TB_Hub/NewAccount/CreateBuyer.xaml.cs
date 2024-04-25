using Ferdin_TB_Hub.Classes;
using Ferdin_TB_Hub.UserControls;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace Ferdin_TB_Hub.NewAccount
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class CreateBuyer : Page
    {
        public CreateBuyer()
        {
            this.InitializeComponent();
           
        }

        private void RevealModeCheckbox_Changed(object sender, RoutedEventArgs e)
        {
            Buttons.TogglePasswordReveal(RevealPassMode, RevealPassCheck);
        }

        private void TextBlock_SelectionChanged(object sender, RoutedEventArgs e)
        {

        }

        private void Proceed_Click(object sender, RoutedEventArgs e)
        {
            // Retrieve user inputs from the textboxes
            string email = tbxEmail.Text;
            string username = tbxUsername.Text;
            string lastName = tbxLastName.Text;
            string firstName = tbxFirstName.Text;
            string middleName = tbxMiddleName.Text;
            string password = RevealPassMode.Password; // Get password from PasswordBox
            string phoneNumber = tbxPhoneNumber.Text;
            string addressLine1 = tbxAddressLine1.Text;
            string addressLine2 = tbxAddressLine2.Text;

            // Check if any of the textboxes are empty or null
            if (string.IsNullOrWhiteSpace(email) ||
                string.IsNullOrWhiteSpace(username) ||
                string.IsNullOrWhiteSpace(lastName) ||
                string.IsNullOrWhiteSpace(firstName) ||
                string.IsNullOrWhiteSpace(password) ||
                string.IsNullOrWhiteSpace(phoneNumber) ||
                string.IsNullOrWhiteSpace(addressLine1) ||
                string.IsNullOrWhiteSpace(addressLine2))
            {
                // Show an error message indicating that all fields are required
                Buttons.ShowPrompt("All fields are required. Please fill in all the fields.");
                return; // Exit the method without proceeding further
            }

            // Check if the username already exists
            if (Database.IsBuyerAlreadyExists(username, email))
            {
                // Show an error message indicating that the username is already taken
                Buttons.ShowPrompt("Username or email already exists. Please choose another one.");
                return; // Exit the method without proceeding further
            }

            // Call the method to add the buyer to the database
            Database.AddBuyer(email, username, lastName, firstName, middleName, password, phoneNumber, addressLine1, addressLine2);

            // Retrieve the list of buyers
            List<Database.BuyerDetails> buyers = Database.GetBuyerRecords();

            // Check if the newly added buyer is present in the list
            bool isAdded = buyers.Any(buyer => buyer.Email == email);
            if (isAdded)
            {
                // The buyer has been successfully added
                Buttons.ShowPrompt("Buyer details added successfully!");
            }
            else
            {
                // There was an issue adding the buyer
                Buttons.ShowPrompt("Failed to add buyer, check your fields, and please try again.");
            }

            // Clear all the textboxes
            tbxEmail.Text = "";
            tbxUsername.Text = "";
            tbxLastName.Text = "";
            tbxFirstName.Text = "";
            tbxMiddleName.Text = "";
            RevealPassMode.Password = "";
            tbxPhoneNumber.Text = "";
            tbxAddressLine1.Text = "";
            tbxAddressLine2.Text = "";

        }



        private void Phone_Numeric(object sender, KeyRoutedEventArgs e)
        {
            Buttons.HandleNumericInput(e);
        }
    }
}