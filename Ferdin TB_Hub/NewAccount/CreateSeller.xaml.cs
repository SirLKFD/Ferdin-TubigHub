using Ferdin_TB_Hub.Classes;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace Ferdin_TB_Hub.NewAccount
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class CreateSeller : Page
    {
        public CreateSeller()
        {
            InitializeComponent();
        }

        private void RevealModeCheckbox_Changed(object sender, RoutedEventArgs e)
        {
            Buttons.TogglePasswordReveal(RevealPassMode, RevealPassCheck);
        }

        private void Proceed_Click(object sender, RoutedEventArgs e)
        {
            // Retrieve input data from form fields
            string businessName = tbxBusinessName.Text;
            string email = tbxEmail.Text;
            string username = tbxUsername.Text;
            string lastName = tbxLastName.Text;
            string firstName = tbxFirstName.Text;
            string middleName = tbxMiddleName.Text;
            string password = RevealPassMode.Password;
            string phoneNumber = tbxPhoneNumber.Text;
            string addressLine1 = tbxAddressLine1.Text;
            string addressLine2 = tbxAddressLine2.Text;

            // Check if any of the textboxes are empty or null
            if (string.IsNullOrWhiteSpace(businessName) ||
                string.IsNullOrWhiteSpace(email) ||
                string.IsNullOrWhiteSpace(username) ||
                string.IsNullOrWhiteSpace(lastName) ||
                string.IsNullOrWhiteSpace(firstName) ||
                string.IsNullOrWhiteSpace(middleName) ||
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
            if (Database.IsSellerAlreadyExists(username, email, businessName))
            {
                // Show an error message indicating that the username is already taken
                Buttons.ShowPrompt("Either store, username, and email already exists. Please choose another one.");
                return; // Exit the method without proceeding further
            }

            // Call the method to add the seller/buyer to the database
            if (this is CreateSeller)
            {
                Database.AddSeller(businessName, email, username, lastName, firstName, middleName, password, phoneNumber, addressLine1, addressLine2);
            }


            // Retrieve the list of sellers/buyers (optional)
            // List<Database.SellerDetails> sellers = Database.GetSellerRecords();
            // List<Database.BuyerDetails> buyers = Database.GetBuyerRecords();

            // Show success message (optional)
            Buttons.ShowPrompt("Account created successfully!");

            // Clear all form fields
            tbxBusinessName.Text = "";
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
