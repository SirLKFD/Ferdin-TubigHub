using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SQLitePCL;
using Windows.Storage;
using System.IO;
using Microsoft.Data.Sqlite;
using System.Security.Cryptography.X509Certificates;
using Windows.UI.Xaml;
using System.Net;
using Ferdin_TB_Hub.HomePage_NavigationView;


namespace Ferdin_TB_Hub.Classes
{
    public class Database
    {

        // THIS IS FROM THE DATABASE.CS

        public Database()
        {
            InitializeDB_BUYERACCOUNTS();
            InitializeDB_SELLERACCOUNTS();
            InitializeDB_PRODUCTDETAILS();
            InitializeDB_PRODUCTCART();
            InitializeDB_PRODUCTRECEIPT();
        }
        public class BuyerDetails
        {
            public int BUYER_ID { get; set; }
            public string Email { get; set; }
            public string Username { get; set; }
            public string LastName { get; set; }
            public string FirstName { get; set; }
            public string MiddleName { get; set; }
            public string Password { get; set; }
            public string PhoneNumber { get; set; }
            public string AddressLine1 { get; set; }
            public string AddressLine2 { get; set; }
        }

        public class SellerDetails
        {
            public int SELLER_ID { get; set; }
            public string BusinessName { get; set; }
            public string Email { get; set; }
            public string Username { get; set; }
            public string LastName { get; set; }
            public string FirstName { get; set; }
            public string MiddleName { get; set; }
            public string Password { get; set; }
            public string PhoneNumber { get; set; }
            public string AddressLine1 { get; set; }
            public string AddressLine2 { get; set; }
        }



        // THIS IS FROM THE DATABASE.CS

        public class ProductDetails
        {
            public int PRODUCTDETAILS_ID { get; set; }
            public int Seller_ID { get; set; }
            public long ProductSKU { get; set; }
            public string ProductName { get; set; }
            public string ProductCategory { get; set; }
            public double ProductPrice { get; set; }
            public string ProductDescription { get; set; }
            public int ProductQuantity { get; set; }
            public byte[] ProductPicture { get; set; }

        }

        public class ProductCart
        {
            // Give me the properties of the ProductCart
            public int ProductCart_ID { get; set; }
            public int Buyer_ID { get; set; }

            public int Seller_ID { get; set; }

            public int ProductDetails_ID { get; set; }

            public long ProductSKU { get; set; }
            public string ProductName { get; set; }
            public string ProductCategory { get; set; }
            public double ProductPrice { get; set; }
            public int ProductQuantity { get; set; }
            public byte[] ProductPicture { get; set; }

        }
        public class ProductReceipt
        {
            public int PRODUCTRECEIPT_ID { get; set; }

            public int Buyer_ID { get; set; }

            public int Seller_ID { get; set; }
            public long OrderNumber { get; set; }
            public string ProductName { get; set; }

            public string LastName { get; set; }
            public string FirstName { get; set; }
            public string MiddleName { get; set; }

            public string PhoneNumber { get; set; }
            public string ProductCategory { get; set; }

            public double ProductPrice { get; set; }

            public int ProductQuantity { get; set; }

            public string AddressLine1 { get; set; }
            public string AddressLine2 { get; set; }

            public string Email { get; set; }

            public string PaymentMethod { get; set; }
            public DateTime DatePurchased { get; set; }

        }





        /// <summary>
        /// BUYER
        /// </summary>

        //Initializing for Buyer Accounts Database
        public async static void InitializeDB_BUYERACCOUNTS()
        {
            try
            {
                await ApplicationData.Current.LocalFolder.CreateFileAsync("MyDatabase.db", CreationCollisionOption.OpenIfExists);
                string pathtoDB = Path.Combine(ApplicationData.Current.LocalFolder.Path, "MyDatabase.db");

                using (SqliteConnection con = new SqliteConnection($"Filename={pathtoDB}"))
                {
                    con.Open();
                    string initCMD = @"CREATE TABLE IF NOT EXISTS Buyers (
                            BUYER_ID INTEGER PRIMARY KEY AUTOINCREMENT,
                            Email TEXT NOT NULL,
                            Username TEXT NOT NULL,
                            LastName TEXT NOT NULL,
                            FirstName TEXT NOT NULL,
                            MiddleName TEXT NOT NULL,
                            Password TEXT NOT NULL,
                            PhoneNumber TEXT NOT NULL,
                            AddressLine1 TEXT NOT NULL,
                            AddressLine2 TEXT NOT NULL
                          )";

                    SqliteCommand CMDcreateTable = new SqliteCommand(initCMD, con);
                    CMDcreateTable.ExecuteReader();
                    con.Close();
                }
            }
            catch (Exception ex)
            {
                Buttons.ShowMessage(ex.Message);
            }

        }

        // Method to check if the buyer username already exists
        public static bool IsBuyerAlreadyExists(string username, string email)
        {
            string pathtoDB = Path.Combine(ApplicationData.Current.LocalFolder.Path, "MyDatabase.db");

            using (SqliteConnection con = new SqliteConnection($"Filename={pathtoDB}"))
            {
                con.Open();
                string selectCMD = "SELECT COUNT(*) FROM Buyers WHERE Username = @Username OR Email = @Email";

                SqliteCommand cmdSelectRecords = new SqliteCommand(selectCMD, con);
                cmdSelectRecords.Parameters.AddWithValue("@Username", username);
                cmdSelectRecords.Parameters.AddWithValue("@Email", email);
                int count = Convert.ToInt32(cmdSelectRecords.ExecuteScalar());

                con.Close();

                return count > 0;
            }
        }


        // Adding Buyer Account
        public static void AddBuyer(string email, string username, string lastName, string firstName, string middleName, string password, string phoneNumber, string addressLine1, string addressLine2)
        {
            try
            {
                string pathtoDB = Path.Combine(ApplicationData.Current.LocalFolder.Path, "MyDatabase.db");

                // Check if the username already exists
                if (IsBuyerAlreadyExists(username, email))
                {
                    // Show an error message indicating that the username is already taken
                    return; // Exit the method without proceeding further
                }

                using (SqliteConnection con = new SqliteConnection($"Filename={pathtoDB}"))
                {
                    con.Open();
                    string insertCMD = @"INSERT INTO Buyers (Email, Username, LastName, FirstName, MiddleName, Password, PhoneNumber, AddressLine1, AddressLine2) 
                            VALUES (@Email, @Username, @LastName, @FirstName, @MiddleName, @Password, @PhoneNumber, @AddressLine1, @AddressLine2)";

                    SqliteCommand cmdInsertRecord = new SqliteCommand(insertCMD, con);
                    cmdInsertRecord.Parameters.AddWithValue("@Email", email);
                    cmdInsertRecord.Parameters.AddWithValue("@Username", username);
                    cmdInsertRecord.Parameters.AddWithValue("@LastName", lastName);
                    cmdInsertRecord.Parameters.AddWithValue("@FirstName", firstName);
                    cmdInsertRecord.Parameters.AddWithValue("@MiddleName", middleName);
                    cmdInsertRecord.Parameters.AddWithValue("@Password", password);
                    cmdInsertRecord.Parameters.AddWithValue("@PhoneNumber", phoneNumber);
                    cmdInsertRecord.Parameters.AddWithValue("@AddressLine1", addressLine1);
                    cmdInsertRecord.Parameters.AddWithValue("@AddressLine2", addressLine2);

                    cmdInsertRecord.ExecuteReader();
                    con.Close();
                }
            }
            catch (Exception ex)
            {
                Buttons.ShowMessage(ex.Message);
            }

        }

        // Query to Retreive Buyer Account
        public static List<BuyerDetails> GetBuyerRecords()
        {
            List<BuyerDetails> buyerList = new List<BuyerDetails>();

            string pathtoDB = Path.Combine(ApplicationData.Current.LocalFolder.Path, "MyDatabase.db");

            using (SqliteConnection con = new SqliteConnection($"Filename={pathtoDB}"))
            {
                con.Open();
                string selectCMD = "SELECT * FROM Buyers";

                SqliteCommand cmdSelectRecords = new SqliteCommand(selectCMD, con);
                SqliteDataReader reader = cmdSelectRecords.ExecuteReader();

                while (reader.Read())
                {
                    BuyerDetails buyer = new BuyerDetails();
                    buyer.BUYER_ID = reader.GetInt32(0);
                    buyer.Email = reader.GetString(1);
                    buyer.Username = reader.GetString(2);
                    buyer.LastName = reader.GetString(3);
                    buyer.FirstName = reader.GetString(4);
                    buyer.MiddleName = reader.GetString(5);
                    buyer.Password = reader.GetString(6);
                    buyer.PhoneNumber = reader.GetString(7);
                    buyer.AddressLine1 = reader.GetString(8);
                    buyer.AddressLine2 = reader.GetString(9);

                    buyerList.Add(buyer);
                }

                reader.Close();
                con.Close();
            }

            return buyerList;
        }

        // Method to check if a buyer exists
        public static bool IsBuyerExists(string username, string password)
        {
            string pathtoDB = Path.Combine(ApplicationData.Current.LocalFolder.Path, "MyDatabase.db");

            using (SqliteConnection con = new SqliteConnection($"Filename={pathtoDB}"))
            {
                con.Open();
                string selectCMD = "SELECT COUNT(*) FROM Buyers WHERE Username = @Username AND Password = @Password";

                SqliteCommand cmdSelectRecords = new SqliteCommand(selectCMD, con);
                cmdSelectRecords.Parameters.AddWithValue("@Username", username);
                cmdSelectRecords.Parameters.AddWithValue("@Password", password);
                int count = Convert.ToInt32(cmdSelectRecords.ExecuteScalar());

                con.Close();

                return count > 0;
            }
        }

        // Method to get buyer details by username or email
        public static BuyerDetails GetBuyerByUsernameOrEmail(string usernameOrEmail)
        {
            BuyerDetails buyer = null;
            string pathtoDB = Path.Combine(ApplicationData.Current.LocalFolder.Path, "MyDatabase.db");

            using (SqliteConnection con = new SqliteConnection($"Filename={pathtoDB}"))
            {
                con.Open();
                string selectCMD = "SELECT BUYER_ID, Email, Username, LastName, FirstName, MiddleName, Password, PhoneNumber, AddressLine1, AddressLine2 FROM Buyers WHERE Username = @Username OR Email = @Email";

                SqliteCommand cmdSelectRecords = new SqliteCommand(selectCMD, con);
                cmdSelectRecords.Parameters.AddWithValue("@Username", usernameOrEmail);
                cmdSelectRecords.Parameters.AddWithValue("@Email", usernameOrEmail);

                SqliteDataReader reader = cmdSelectRecords.ExecuteReader();

                if (reader.Read())
                {
                    buyer = new BuyerDetails();
                    buyer.BUYER_ID = reader.GetInt32(0); // Assuming the ID is the first column in the result set
                    buyer.Email = reader.GetString(1);
                    buyer.Username = reader.GetString(2);
                    buyer.LastName = reader.GetString(3);
                    buyer.FirstName = reader.GetString(4);
                    buyer.MiddleName = reader.GetString(5);
                    buyer.Password = reader.GetString(6);
                    buyer.PhoneNumber = reader.GetString(7);
                    buyer.AddressLine1 = reader.GetString(8);
                    buyer.AddressLine2 = reader.GetString(9);
                }

                reader.Close();
                con.Close();
            }

            return buyer;
        }

        // Method to update buyer information in the database
        public static void UpdateBuyerInfoFromDatabase(int buyer_id, string email, string username, string lastName, string firstName, string middleName, string password, string phoneNumber, string addressLine1, string addressLine2)
        {
            try
            {
                // Check if the username or email already exists
                if (IsBuyerExists(username, email))
                {
                    Buttons.ShowMessage("Username or Email already exists. Please choose a different one.");
                    return;
                }

                // Proceed with updating the buyer info
                string pathtoDB = Path.Combine(ApplicationData.Current.LocalFolder.Path, "MyDatabase.db");
                using (SqliteConnection con = new SqliteConnection($"Filename={pathtoDB}"))
                {
                    con.Open();
                    string updateCMD = @"UPDATE Buyers SET Email = @Email, Username = @Username, LastName = @LastName, FirstName = @FirstName, 
                    MiddleName = @MiddleName, Password = @Password, PhoneNumber = @PhoneNumber, AddressLine1 = @AddressLine1, AddressLine2 = @AddressLine2 
                    WHERE BUYER_ID = @BUYER_ID";

                    SqliteCommand cmdUpdateRecord = new SqliteCommand(updateCMD, con);
                    cmdUpdateRecord.Parameters.AddWithValue("@BUYER_ID", buyer_id); // Add Id parameter
                    cmdUpdateRecord.Parameters.AddWithValue("@Email", email);
                    cmdUpdateRecord.Parameters.AddWithValue("@Username", username);
                    cmdUpdateRecord.Parameters.AddWithValue("@LastName", lastName);
                    cmdUpdateRecord.Parameters.AddWithValue("@FirstName", firstName);
                    cmdUpdateRecord.Parameters.AddWithValue("@MiddleName", middleName);
                    cmdUpdateRecord.Parameters.AddWithValue("@Password", password);
                    cmdUpdateRecord.Parameters.AddWithValue("@PhoneNumber", phoneNumber);
                    cmdUpdateRecord.Parameters.AddWithValue("@AddressLine1", addressLine1);
                    cmdUpdateRecord.Parameters.AddWithValue("@AddressLine2", addressLine2);

                    cmdUpdateRecord.ExecuteReader();
                    con.Close();
                }
            }
            catch (Exception ex)
            {
                Buttons.ShowMessage(ex.Message);
            }
        }

        // Method to delete a buyer account from the database
        public static void DeleteBuyerAccountFromDatabase(string usernameOrEmail)
        {
            try
            {
                string pathtoDB = Path.Combine(ApplicationData.Current.LocalFolder.Path, "MyDatabase.db");

                using (SqliteConnection con = new SqliteConnection($"Filename={pathtoDB}"))
                {
                    con.Open();
                    string deleteReceiptsCMD = "DELETE FROM ProductReceipts WHERE Buyer_ID IN (SELECT BUYER_ID FROM Buyers WHERE Username = @Username OR Email = @Email)";
                    string deleteBuyerCMD = "DELETE FROM Buyers WHERE Username = @Username OR Email = @Email";

                    // Delete associated records from ProductReceipts table
                    using (SqliteCommand cmdDeleteReceipts = new SqliteCommand(deleteReceiptsCMD, con))
                    {
                        cmdDeleteReceipts.Parameters.AddWithValue("@Username", usernameOrEmail);
                        cmdDeleteReceipts.Parameters.AddWithValue("@Email", usernameOrEmail);
                        cmdDeleteReceipts.ExecuteNonQuery();
                    }

                    // Delete the buyer account from the Buyers table
                    using (SqliteCommand cmdDeleteBuyer = new SqliteCommand(deleteBuyerCMD, con))
                    {
                        cmdDeleteBuyer.Parameters.AddWithValue("@Username", usernameOrEmail);
                        cmdDeleteBuyer.Parameters.AddWithValue("@Email", usernameOrEmail);
                        cmdDeleteBuyer.ExecuteNonQuery();
                    }

                    con.Close();
                }
            }
            catch (Exception ex)
            {
                Buttons.ShowMessage(ex.Message);
            }

        }


        /// <summary>
        /// SELLER
        /// </summary>

        //Initializing for Seller Accounts Database
        public async static void InitializeDB_SELLERACCOUNTS()
        {
            try
            {
                await ApplicationData.Current.LocalFolder.CreateFileAsync("MyDatabase.db", CreationCollisionOption.OpenIfExists);
                string pathtoDB = Path.Combine(ApplicationData.Current.LocalFolder.Path, "MyDatabase.db");

                using (SqliteConnection con = new SqliteConnection($"Filename={pathtoDB}"))
                {
                    con.Open();
                    string initCMD = @"CREATE TABLE IF NOT EXISTS Sellers (
                            SELLER_ID INTEGER PRIMARY KEY AUTOINCREMENT,                        
                            BusinessName TEXT NOT NULL,
                            Email TEXT NOT NULL,
                            Username TEXT NOT NULL,
                            LastName TEXT NOT NULL,
                            FirstName TEXT NOT NULL,
                            MiddleName TEXT NOT NULL,
                            Password TEXT NOT NULL,
                            PhoneNumber TEXT NOT NULL,
                            AddressLine1 TEXT NOT NULL,
                            AddressLine2 TEXT NOT NULL
                          )";

                    SqliteCommand CMDcreateTable = new SqliteCommand(initCMD, con);
                    CMDcreateTable.ExecuteReader();
                    con.Close();
                }
            }
            catch
            {

            }

        }




        // Method to check if the seller username, email, or business name already exists
        public static bool IsSellerAlreadyExists(string username, string email, string businessName)
        {
            string pathtoDB = Path.Combine(ApplicationData.Current.LocalFolder.Path, "MyDatabase.db");

            using (SqliteConnection con = new SqliteConnection($"Filename={pathtoDB}"))
            {
                con.Open();
                string selectCMD = "SELECT COUNT(*) FROM Sellers WHERE Username = @Username OR Email = @Email OR BusinessName = @BusinessName";

                SqliteCommand cmdSelectRecords = new SqliteCommand(selectCMD, con);
                cmdSelectRecords.Parameters.AddWithValue("@Username", username);
                cmdSelectRecords.Parameters.AddWithValue("@Email", email);
                cmdSelectRecords.Parameters.AddWithValue("@BusinessName", businessName);
                int count = Convert.ToInt32(cmdSelectRecords.ExecuteScalar());

                con.Close();

                return count > 0;
            }
        }

        // Adding Seller Account
        public static void AddSeller(string businessName, string email, string username, string lastName, string firstName, string middleName, string password, string phoneNumber, string addressLine1, string addressLine2)
        {
            try
            {
                string pathtoDB = Path.Combine(ApplicationData.Current.LocalFolder.Path, "MyDatabase.db");

                // Check if the username or email already exists
                if (IsSellerAlreadyExists(username, email, businessName))
                {
                    // Show an error message indicating that the username or email is already taken
                    return; // Exit the method without proceeding further
                }

                using (SqliteConnection con = new SqliteConnection($"Filename={pathtoDB}"))
                {
                    con.Open();
                    string insertCMD = @"INSERT INTO Sellers (BusinessName, Email, Username, LastName, FirstName, MiddleName, Password, PhoneNumber, AddressLine1, AddressLine2) 
                                    VALUES (@BusinessName, @Email, @Username, @LastName, @FirstName, @MiddleName, @Password, @PhoneNumber, @AddressLine1, @AddressLine2)";

                    SqliteCommand cmdInsertRecord = new SqliteCommand(insertCMD, con);
                    cmdInsertRecord.Parameters.AddWithValue("@BusinessName", businessName);
                    cmdInsertRecord.Parameters.AddWithValue("@Email", email);
                    cmdInsertRecord.Parameters.AddWithValue("@Username", username);
                    cmdInsertRecord.Parameters.AddWithValue("@LastName", lastName);
                    cmdInsertRecord.Parameters.AddWithValue("@FirstName", firstName);
                    cmdInsertRecord.Parameters.AddWithValue("@MiddleName", middleName);
                    cmdInsertRecord.Parameters.AddWithValue("@Password", password);
                    cmdInsertRecord.Parameters.AddWithValue("@PhoneNumber", phoneNumber);
                    cmdInsertRecord.Parameters.AddWithValue("@AddressLine1", addressLine1);
                    cmdInsertRecord.Parameters.AddWithValue("@AddressLine2", addressLine2);

                    cmdInsertRecord.ExecuteReader();
                    con.Close();
                }
            }
            catch (Exception ex)
            {
                Buttons.ShowMessage(ex.Message);
            }

        }

        // Query to Retrieve Seller Account
        public static List<SellerDetails> GetSellerRecords()
        {
            List<SellerDetails> sellerList = new List<SellerDetails>();

            string pathtoDB = Path.Combine(ApplicationData.Current.LocalFolder.Path, "MyDatabase.db");

            using (SqliteConnection con = new SqliteConnection($"Filename={pathtoDB}"))
            {
                con.Open();
                string selectCMD = "SELECT * FROM Sellers";

                SqliteCommand cmdSelectRecords = new SqliteCommand(selectCMD, con);
                SqliteDataReader reader = cmdSelectRecords.ExecuteReader();

                while (reader.Read())
                {
                    SellerDetails seller = new SellerDetails();
                    seller.SELLER_ID = reader.GetInt32(0);
                    seller.BusinessName = reader.GetString(1);
                    seller.Email = reader.GetString(2);
                    seller.Username = reader.GetString(3);
                    seller.LastName = reader.GetString(4);
                    seller.FirstName = reader.GetString(5);
                    seller.MiddleName = reader.GetString(6);
                    seller.Password = reader.GetString(7);
                    seller.PhoneNumber = reader.GetString(8);
                    seller.AddressLine1 = reader.GetString(9);
                    seller.AddressLine2 = reader.GetString(10);

                    sellerList.Add(seller);
                }

                reader.Close();
                con.Close();
            }

            return sellerList;
        }


        // Method to check if a seller exists
        public static bool IsSellerExists(string username, string password)
        {
            string pathtoDB = Path.Combine(ApplicationData.Current.LocalFolder.Path, "MyDatabase.db");

            using (SqliteConnection con = new SqliteConnection($"Filename={pathtoDB}"))
            {
                con.Open();
                string selectCMD = "SELECT COUNT(*) FROM Sellers WHERE Username = @Username AND Password = @Password";

                SqliteCommand cmdSelectRecords = new SqliteCommand(selectCMD, con);
                cmdSelectRecords.Parameters.AddWithValue("@Username", username);
                cmdSelectRecords.Parameters.AddWithValue("@Password", password);
                int count = Convert.ToInt32(cmdSelectRecords.ExecuteScalar());

                con.Close();

                return count > 0;
            }
        }

        // Method to get seller details by username or email
        public static SellerDetails GetSellerByUsernameOrEmail(string usernameOrEmail)
        {
            SellerDetails seller = null;
            string pathtoDB = Path.Combine(ApplicationData.Current.LocalFolder.Path, "MyDatabase.db");

            using (SqliteConnection con = new SqliteConnection($"Filename={pathtoDB}"))
            {
                con.Open();
                string selectCMD = "SELECT SELLER_ID, BusinessName, Email, Username, LastName, FirstName, MiddleName, Password, PhoneNumber, AddressLine1, AddressLine2 FROM Sellers WHERE Username = @Username OR Email = @Email";

                SqliteCommand cmdSelectRecords = new SqliteCommand(selectCMD, con);
                cmdSelectRecords.Parameters.AddWithValue("@Username", usernameOrEmail);
                cmdSelectRecords.Parameters.AddWithValue("@Email", usernameOrEmail);

                SqliteDataReader reader = cmdSelectRecords.ExecuteReader();

                if (reader.Read())
                {
                    seller = new SellerDetails();
                    seller.SELLER_ID = reader.GetInt32(0); // Assuming the ID is the first column in the result set
                    seller.BusinessName = reader.GetString(1);
                    seller.Email = reader.GetString(2);
                    seller.Username = reader.GetString(3);
                    seller.LastName = reader.GetString(4);
                    seller.FirstName = reader.GetString(5);
                    seller.MiddleName = reader.GetString(6);
                    seller.Password = reader.GetString(7);
                    seller.PhoneNumber = reader.GetString(8);
                    seller.AddressLine1 = reader.GetString(9);
                    seller.AddressLine2 = reader.GetString(10);
                }

                reader.Close();
                con.Close();
            }

            return seller;
        }


        // Method to update seller information in the database
        public static void UpdateSellerInfoFromDatabase(int seller_id, string businessName, string email, string username, string lastName, string firstName, string middleName, string password, string phoneNumber, string addressLine1, string addressLine2)
        {
            try
            {
                string pathtoDB = Path.Combine(ApplicationData.Current.LocalFolder.Path, "MyDatabase.db");

                using (SqliteConnection con = new SqliteConnection($"Filename={pathtoDB}"))
                {
                    con.Open();
                    string updateCMD = @"UPDATE Sellers SET BusinessName = @BusinessName, Email = @Email, Username = @Username, LastName = @LastName, FirstName = @FirstName, 
                            MiddleName = @MiddleName, Password = @Password, PhoneNumber = @PhoneNumber, AddressLine1 = @AddressLine1, AddressLine2 = @AddressLine2 
                            WHERE SELLER_ID = @SELLER_ID";

                    SqliteCommand cmdUpdateRecord = new SqliteCommand(updateCMD, con);
                    cmdUpdateRecord.Parameters.AddWithValue("@SELLER_ID", seller_id); // Add Id parameter
                    cmdUpdateRecord.Parameters.AddWithValue("@BusinessName", businessName);
                    cmdUpdateRecord.Parameters.AddWithValue("@Email", email);
                    cmdUpdateRecord.Parameters.AddWithValue("@Username", username);
                    cmdUpdateRecord.Parameters.AddWithValue("@LastName", lastName);
                    cmdUpdateRecord.Parameters.AddWithValue("@FirstName", firstName);
                    cmdUpdateRecord.Parameters.AddWithValue("@MiddleName", middleName);
                    cmdUpdateRecord.Parameters.AddWithValue("@Password", password);
                    cmdUpdateRecord.Parameters.AddWithValue("@PhoneNumber", phoneNumber);
                    cmdUpdateRecord.Parameters.AddWithValue("@AddressLine1", addressLine1);
                    cmdUpdateRecord.Parameters.AddWithValue("@AddressLine2", addressLine2);

                    cmdUpdateRecord.ExecuteReader();
                    con.Close();
                }
            }
            catch (Exception ex)
            {
                Buttons.ShowMessage(ex.Message);
            }

        }


        // Method to delete a seller account from the database along with associated product details
        public static void DeleteSellerAccountFromDatabase(string usernameOrEmail)
        {
            try
            {
                string pathtoDB = Path.Combine(ApplicationData.Current.LocalFolder.Path, "MyDatabase.db");

                using (SqliteConnection con = new SqliteConnection($"Filename={pathtoDB}"))
                {
                    con.Open();

                    // Delete associated records from ProductReceipts table
                    string deleteProductReceiptsCMD = "DELETE FROM ProductReceipts WHERE Seller_ID IN (SELECT SELLER_ID FROM Sellers WHERE Username = @Username OR Email = @Email)";
                    using (SqliteCommand cmdDeleteProductReceipts = new SqliteCommand(deleteProductReceiptsCMD, con))
                    {
                        cmdDeleteProductReceipts.Parameters.AddWithValue("@Username", usernameOrEmail);
                        cmdDeleteProductReceipts.Parameters.AddWithValue("@Email", usernameOrEmail);
                        cmdDeleteProductReceipts.ExecuteNonQuery();
                    }

                    // Delete associated records from ProductCart table
                    string deleteProductCartCMD = "DELETE FROM ProductCart WHERE Seller_ID IN (SELECT SELLER_ID FROM Sellers WHERE Username = @Username OR Email = @Email)";
                    using (SqliteCommand cmdDeleteProductCart = new SqliteCommand(deleteProductCartCMD, con))
                    {
                        cmdDeleteProductCart.Parameters.AddWithValue("@Username", usernameOrEmail);
                        cmdDeleteProductCart.Parameters.AddWithValue("@Email", usernameOrEmail);
                        cmdDeleteProductCart.ExecuteNonQuery();
                    }

                    // Delete associated records from ProductDetails table
                    string deleteProductDetailsCMD = "DELETE FROM ProductDetails WHERE Seller_ID IN (SELECT SELLER_ID FROM Sellers WHERE Username = @Username OR Email = @Email)";
                    using (SqliteCommand cmdDeleteProductDetails = new SqliteCommand(deleteProductDetailsCMD, con))
                    {
                        cmdDeleteProductDetails.Parameters.AddWithValue("@Username", usernameOrEmail);
                        cmdDeleteProductDetails.Parameters.AddWithValue("@Email", usernameOrEmail);
                        cmdDeleteProductDetails.ExecuteNonQuery();
                    }

                    // Delete the seller account from the Sellers table
                    string deleteSellerCMD = "DELETE FROM Sellers WHERE Username = @Username OR Email = @Email";
                    using (SqliteCommand cmdDeleteSeller = new SqliteCommand(deleteSellerCMD, con))
                    {
                        cmdDeleteSeller.Parameters.AddWithValue("@Username", usernameOrEmail);
                        cmdDeleteSeller.Parameters.AddWithValue("@Email", usernameOrEmail);
                        cmdDeleteSeller.ExecuteNonQuery();
                    }

                    con.Close();
                }
            }
            catch (Exception ex)
            {
                Buttons.ShowMessage(ex.Message);
            }
        }




        /// <summary>
        /// PRODUCT DETAILS
        /// </summary>

        //Initializing for Product Details Database


        //Initializing for Product Details Database
        public async static void InitializeDB_PRODUCTDETAILS()
        {
            try
            {
                await ApplicationData.Current.LocalFolder.CreateFileAsync("MyDatabase.db", CreationCollisionOption.OpenIfExists);
                string pathtoDB = Path.Combine(ApplicationData.Current.LocalFolder.Path, "MyDatabase.db");

                using (SqliteConnection con = new SqliteConnection($"Filename={pathtoDB}"))
                {
                    con.Open();
                    string initCMD = @"CREATE TABLE IF NOT EXISTS ProductDetails (
                          PRODUCTDETAILS_ID INTEGER PRIMARY KEY AUTOINCREMENT,
                          ProductName TEXT NOT NULL,
                          ProductCategory TEXT,
                          ProductPrice REAL,
                          ProductDescription TEXT,
                          ProductQuantity INTEGER,
                          ProductPicture BLOB,
                          ProductSKU INTEGER NOT NULL,
                          Seller_ID INTEGER NOT NULL,                       
                          FOREIGN KEY (Seller_ID) REFERENCES Sellers (SELLER_ID)

                        )";

                    SqliteCommand CMDcreateTable = new SqliteCommand(initCMD, con);
                    CMDcreateTable.ExecuteReader();
                    con.Close();
                }
            }
            catch (Exception ex)
            {
                Buttons.ShowMessage(ex.Message);
            }

        }


        // Adding Product to the Database
        public static void AddProduct(string productname, string productcategory, double productprice, string productdescription, int productquantity, byte[] productpicture, int seller_id)
        {
            try
            {
                string pathtoDB = Path.Combine(ApplicationData.Current.LocalFolder.Path, "MyDatabase.db");

                using (SqliteConnection con = new SqliteConnection($"Filename={pathtoDB}"))
                {
                    con.Open();

                    // Generate a random 12-digit SKU
                    Random random = new Random();
                    long productSKU;
                    bool isUniqueSKU = false;

                    // Loop until a unique SKU is generated
                    do
                    {
                        productSKU = (long)(random.NextDouble() * (999999999999L - 100000000000L) + 100000000000L);
                        string checkSKUQuery = "SELECT COUNT(*) FROM ProductDetails WHERE ProductSKU = @ProductSKU";

                        using (SqliteCommand cmdCheckSKU = new SqliteCommand(checkSKUQuery, con))
                        {
                            cmdCheckSKU.Parameters.AddWithValue("@ProductSKU", productSKU);
                            long existingCount = (long)cmdCheckSKU.ExecuteScalar();

                            if (existingCount == 0)
                                isUniqueSKU = true;
                        }
                    } while (!isUniqueSKU);

                    string insertCMD = @"INSERT INTO ProductDetails (ProductSKU, ProductName, ProductCategory, ProductPrice, ProductDescription, ProductQuantity, ProductPicture, Seller_ID) 
                             VALUES (@ProductSKU, @ProductName, @ProductCategory, @ProductPrice, @ProductDescription, @ProductQuantity, @ProductPicture, @Seller_ID)";

                    SqliteCommand cmdInsertRecord = new SqliteCommand(insertCMD, con);
                    cmdInsertRecord.Parameters.AddWithValue("@Seller_ID", seller_id);
                    cmdInsertRecord.Parameters.AddWithValue("@ProductName", productname);
                    cmdInsertRecord.Parameters.AddWithValue("@ProductCategory", productcategory);
                    cmdInsertRecord.Parameters.AddWithValue("@ProductPrice", productprice);
                    cmdInsertRecord.Parameters.AddWithValue("@ProductDescription", productdescription);
                    cmdInsertRecord.Parameters.AddWithValue("@ProductQuantity", productquantity);
                    cmdInsertRecord.Parameters.AddWithValue("@ProductPicture", productpicture);
                    cmdInsertRecord.Parameters.AddWithValue("@ProductSKU", productSKU);

                    cmdInsertRecord.ExecuteNonQuery();
                    con.Close();
                }
            }
            catch (Exception ex)
            {
                Buttons.ShowMessage(ex.Message);
            }


        }


        // Query to Retrieve Seller's product details
        public static List<ProductDetails> GetProductDetails(int seller_id)
        {

            List<ProductDetails> productList = new List<ProductDetails>();

            string pathtoDB = Path.Combine(ApplicationData.Current.LocalFolder.Path, "MyDatabase.db");

            using (SqliteConnection con = new SqliteConnection($"Filename={pathtoDB}"))
            {
                con.Open();
                string selectCMD = "SELECT PRODUCTDETAILS_ID, ProductSKU, ProductName, ProductCategory, ProductPrice, ProductDescription, ProductQuantity, ProductPicture FROM ProductDetails WHERE Seller_ID = @Seller_ID";

                SqliteCommand cmdSelectRecords = new SqliteCommand(selectCMD, con);
                cmdSelectRecords.Parameters.AddWithValue("@Seller_ID", seller_id); // Add parameter for seller ID
                SqliteDataReader reader = cmdSelectRecords.ExecuteReader();

                while (reader.Read())
                {
                    ProductDetails product = new ProductDetails();
                    product.PRODUCTDETAILS_ID = reader.GetInt32(0);
                    product.Seller_ID = seller_id;
                    product.ProductSKU = reader.GetInt64(1); // Retrieve ProductSKU from the reader
                    product.ProductName = reader.GetString(2);
                    product.ProductCategory = reader.IsDBNull(3) ? null : reader.GetString(3);
                    product.ProductPrice = reader.GetDouble(4);
                    product.ProductDescription = reader.GetString(5);
                    product.ProductQuantity = reader.GetInt32(6);
                    product.ProductPicture = reader.IsDBNull(7) ? null : GetByteArrayFromBlob(reader, 7); // Retrieve image data as byte array

                    productList.Add(product);
                }

                reader.Close();
                con.Close();
            }

            return productList;
        }


        // Query to Retrieve ALL Seller's product details
        public static List<ProductDetails> GetAllProductDetails()
        {
            List<ProductDetails> productList = new List<ProductDetails>();

            string pathtoDB = Path.Combine(ApplicationData.Current.LocalFolder.Path, "MyDatabase.db");

            using (SqliteConnection con = new SqliteConnection($"Filename={pathtoDB}"))
            {
                con.Open();
                string selectCMD = "SELECT PRODUCTDETAILS_ID, ProductSKU, ProductName, ProductCategory, ProductPrice, ProductDescription, ProductQuantity, ProductPicture, Seller_ID FROM ProductDetails";

                SqliteCommand cmdSelectRecords = new SqliteCommand(selectCMD, con);
                SqliteDataReader reader = cmdSelectRecords.ExecuteReader();

                while (reader.Read())
                {
                    ProductDetails product = new ProductDetails();
                    product.PRODUCTDETAILS_ID = reader.GetInt32(0);
                    product.ProductSKU = reader.GetInt64(1); // Retrieve ProductSKU from the reader
                    product.ProductName = reader.GetString(2);
                    product.ProductCategory = reader.IsDBNull(3) ? null : reader.GetString(3);
                    product.ProductPrice = reader.GetDouble(4);
                    product.ProductDescription = reader.GetString(5);
                    product.ProductQuantity = reader.GetInt32(6);
                    product.ProductPicture = reader.IsDBNull(7) ? null : GetByteArrayFromBlob(reader, 7); // Retrieve image data as byte array
                    product.Seller_ID = reader.GetInt32(8); // Retrieve Seller_ID from the reader

                    productList.Add(product);
                }

                reader.Close();
            }

            return productList;
        }






        // Update Product Details Method
        public static void UpdateProductDetailsFromDatabase(long productSKU, string productname, string productcategory, double productprice, string productdescription, int productquantity, byte[] productpicture)
        {
            try
            {
                string pathtoDB = Path.Combine(ApplicationData.Current.LocalFolder.Path, "MyDatabase.db");

                using (SqliteConnection con = new SqliteConnection($"Filename={pathtoDB}"))
                {
                    con.Open();
                    string updateCMD = @"UPDATE ProductDetails SET ProductName = @ProductName, ProductCategory = @ProductCategory, ProductPrice = @ProductPrice, ProductDescription = @ProductDescription, 
                    ProductQuantity = @ProductQuantity, ProductPicture = @ProductPicture
                    WHERE ProductSKU = @ProductSKU";

                    SqliteCommand cmdUpdateRecord = new SqliteCommand(updateCMD, con);
                    cmdUpdateRecord.Parameters.AddWithValue("@ProductSKU", productSKU);
                    cmdUpdateRecord.Parameters.AddWithValue("@ProductName", productname);
                    cmdUpdateRecord.Parameters.AddWithValue("@ProductCategory", productcategory);
                    cmdUpdateRecord.Parameters.AddWithValue("@ProductPrice", productprice);
                    cmdUpdateRecord.Parameters.AddWithValue("@ProductDescription", productdescription);
                    cmdUpdateRecord.Parameters.AddWithValue("@ProductQuantity", productquantity);
                    cmdUpdateRecord.Parameters.AddWithValue("@ProductPicture", productpicture);

                    cmdUpdateRecord.ExecuteReader();
                    con.Close();
                }
            }
            catch (Exception ex)
            {
                Buttons.ShowMessage(ex.Message);
            }

        }



        // Method to delete a product details from the database
        public static void DeleteProductDetailsFromDatabase(long productSKU)
        {
            try
            {
                string pathtoDB = Path.Combine(ApplicationData.Current.LocalFolder.Path, "MyDatabase.db");

                using (SqliteConnection con = new SqliteConnection($"Filename={pathtoDB}"))
                {
                    con.Open();
                    string deleteCMD = "DELETE FROM ProductDetails WHERE ProductSKU = @ProductSKU";

                    SqliteCommand cmdDeleteRecord = new SqliteCommand(deleteCMD, con);
                    cmdDeleteRecord.Parameters.AddWithValue("@ProductSKU", productSKU);

                    cmdDeleteRecord.ExecuteNonQuery();
                    con.Close();
                }
            }
            catch (Exception ex)
            {
                Buttons.ShowMessage(ex.Message);
            }

        }

        private static byte[] GetByteArrayFromBlob(SqliteDataReader reader, int columnIndex)
        {
            byte[] buffer = new byte[reader.GetBytes(columnIndex, 0, null, 0, int.MaxValue)];
            reader.GetBytes(columnIndex, 0, buffer, 0, buffer.Length);
            return buffer;
        }


        /// <summary>
        /// PRODUCT CART
        /// </summary>

        // Initializing for Product Cart Database


        public async static void InitializeDB_PRODUCTCART()
        {
            try
            {
                await ApplicationData.Current.LocalFolder.CreateFileAsync("MyDatabase.db", CreationCollisionOption.OpenIfExists);
                string pathtoDB = Path.Combine(ApplicationData.Current.LocalFolder.Path, "MyDatabase.db");

                using (SqliteConnection con = new SqliteConnection($"Filename={pathtoDB}"))
                {
                    con.Open();
                    string initCMD = @"CREATE TABLE IF NOT EXISTS ProductCart (
                    ProductCart_ID INTEGER PRIMARY KEY AUTOINCREMENT,                  
                    ProductName TEXT NOT NULL,
                    ProductPrice REAL NOT NULL,
                    ProductCategory TEXT NOT NULL,
                    ProductQuantity INTEGER NOT NULL,
                    ProductPicture BLOB NOT NULL,
                    Buyer_ID INTEGER NOT NULL,
                    Seller_ID INTEGER NOT NULL,   
                    ProductDetails_ID INTEGER NOT NULL,
                    ProductSKU INTEGER NOT NULL,
                    FOREIGN KEY (Buyer_ID) REFERENCES Buyers (BUYER_ID),
                    FOREIGN KEY (Seller_ID) REFERENCES Sellers (SELLER_ID),
                    FOREIGN KEY (ProductDetails_ID) REFERENCES ProductDetails (PRODUCTDETAILS_ID)
                  )";

                    SqliteCommand CMDcreateTable = new SqliteCommand(initCMD, con);
                    CMDcreateTable.ExecuteReader();
                    con.Close();
                }
            }
            catch (Exception ex)
            {
                Buttons.ShowMessage(ex.Message);
            }

        }


        // Method to add a product to the cart
        public static void AddProductToCart(string productName, double productPrice, string productCategory, int productQuantity, byte[] productPicture, int buyer_ID, int seller_ID, int productdetails_ID, long productSKU)
        {
            try
            {
                string pathtoDB = Path.Combine(ApplicationData.Current.LocalFolder.Path, "MyDatabase.db");

                using (SqliteConnection con = new SqliteConnection($"Filename={pathtoDB}"))
                {
                    con.Open();
                    string insertCMD = @"INSERT INTO ProductCart (ProductName, ProductPrice, ProductCategory, ProductQuantity, ProductPicture, Buyer_ID, Seller_ID, ProductDetails_ID, ProductSKU) 
                     VALUES (@ProductName, @ProductPrice, @ProductCategory, @ProductQuantity, @ProductPicture, @Buyer_ID, @Seller_ID, @ProductDetails_ID, @ProductSKU)";

                    SqliteCommand cmdInsertRecord = new SqliteCommand(insertCMD, con);
                    cmdInsertRecord.Parameters.AddWithValue("@ProductName", productName);
                    cmdInsertRecord.Parameters.AddWithValue("@ProductPrice", productPrice);
                    cmdInsertRecord.Parameters.AddWithValue("@ProductQuantity", productQuantity);
                    cmdInsertRecord.Parameters.AddWithValue("@ProductCategory", productCategory);
                    cmdInsertRecord.Parameters.AddWithValue("@ProductPicture", productPicture);
                    cmdInsertRecord.Parameters.AddWithValue("@Buyer_ID", buyer_ID);
                    cmdInsertRecord.Parameters.AddWithValue("@Seller_ID", seller_ID);
                    cmdInsertRecord.Parameters.AddWithValue("@ProductDetails_ID", productdetails_ID);
                    cmdInsertRecord.Parameters.AddWithValue("@ProductSKU", productSKU);


                    cmdInsertRecord.ExecuteNonQuery();
                    con.Close();
                }
            }
            catch (Exception ex)
            {
                Buttons.ShowMessage(ex.Message);
            }

        }

        // Method to delete a product from the cart
        public static void DeleteProductFromCart(int productCart_ID)
        {
            try
            {
                string pathtoDB = Path.Combine(ApplicationData.Current.LocalFolder.Path, "MyDatabase.db");

                using (SqliteConnection con = new SqliteConnection($"Filename={pathtoDB}"))
                {
                    con.Open();
                    string deleteCMD = "DELETE FROM ProductCart WHERE ProductCart_ID = @ProductCart_ID";

                    SqliteCommand cmdDeleteRecord = new SqliteCommand(deleteCMD, con);
                    cmdDeleteRecord.Parameters.AddWithValue("@ProductCart_ID", productCart_ID);

                    cmdDeleteRecord.ExecuteReader();
                    con.Close();
                }
            }
            catch (Exception ex)
            {
                Buttons.ShowMessage(ex.Message);
            }

        }

        // Method to get Product Quantity

        public static int GetProductQuantity(long productSKU)
        {
            int productQuantity = 0;
            string pathtoDB = Path.Combine(ApplicationData.Current.LocalFolder.Path, "MyDatabase.db");

            using (SqliteConnection con = new SqliteConnection($"Filename={pathtoDB}"))
            {
                con.Open();
                string selectCMD = "SELECT ProductQuantity FROM ProductDetails WHERE ProductSKU = @ProductSKU";

                SqliteCommand cmdSelectRecords = new SqliteCommand(selectCMD, con);
                cmdSelectRecords.Parameters.AddWithValue("@ProductSKU", productSKU);

                SqliteDataReader reader = cmdSelectRecords.ExecuteReader();

                if (reader.Read())
                {
                    productQuantity = reader.GetInt32(0);
                }

                reader.Close();
                con.Close();
            }

            return productQuantity;
        }



        // Query to Retrieve Product Cart
        public static List<ProductCart> GetProductCart()
        {
            List<ProductCart> productCartList = new List<ProductCart>();

            string pathtoDB = Path.Combine(ApplicationData.Current.LocalFolder.Path, "MyDatabase.db");

            using (SqliteConnection con = new SqliteConnection($"Filename={pathtoDB}"))
            {
                con.Open();
                string selectCMD = "SELECT * FROM ProductCart";

                SqliteCommand cmdSelectRecords = new SqliteCommand(selectCMD, con);

                SqliteDataReader reader = cmdSelectRecords.ExecuteReader();

                while (reader.Read())
                {
                    ProductCart productCart = new ProductCart();
                    productCart.ProductCart_ID = reader.GetInt32(0);
                    productCart.ProductName = reader.GetString(1);
                    productCart.ProductPrice = reader.GetDouble(2);
                    productCart.ProductQuantity = reader.GetInt32(3);
                    productCart.ProductCategory = reader.GetString(4);
                    productCart.Buyer_ID = reader.GetInt32(5);
                    productCart.Seller_ID = reader.GetInt32(6);

                    productCart.ProductPicture = reader.IsDBNull(5) ? null : GetByteArrayFromBlob(reader, 5); // Retrieve image data as byte array


                    productCartList.Add(productCart);
                }

                reader.Close();
                con.Close();
            }

            return productCartList;
        }


        // Method to decrease the quantity of a product in the ProductDetails table
        public static void DecreaseProductQuantity(long productSKU, int quantity)
        {
            try
            {
                string pathtoDB = Path.Combine(ApplicationData.Current.LocalFolder.Path, "MyDatabase.db");

                using (SqliteConnection con = new SqliteConnection($"Filename={pathtoDB}"))
                {
                    con.Open();
                    string updateCMD = @"UPDATE ProductDetails SET ProductQuantity = ProductQuantity - @Quantity WHERE ProductSKU = @ProductSKU";

                    SqliteCommand cmdUpdateRecord = new SqliteCommand(updateCMD, con);
                    cmdUpdateRecord.Parameters.AddWithValue("@Quantity", quantity);
                    cmdUpdateRecord.Parameters.AddWithValue("@ProductSKU", productSKU);

                    cmdUpdateRecord.ExecuteNonQuery(); // Use ExecuteNonQuery for UPDATE, INSERT, DELETE operations
                    con.Close();
                }
            }
            catch (Exception ex)
            {
                Buttons.ShowMessage(ex.Message);
            }

        }


        // Method to restore the quantity of a product in the ProductDetails table
        public static void RestoreProductQuantity(long productSKU, int quantity)
        {
            try
            {
                string pathtoDB = Path.Combine(ApplicationData.Current.LocalFolder.Path, "MyDatabase.db");

                using (SqliteConnection con = new SqliteConnection($"Filename={pathtoDB}"))
                {
                    con.Open();
                    string updateCMD = @"UPDATE ProductDetails SET ProductQuantity = ProductQuantity + @Quantity WHERE ProductSKU = @ProductSKU";

                    SqliteCommand cmdUpdateRecord = new SqliteCommand(updateCMD, con);
                    cmdUpdateRecord.Parameters.AddWithValue("@Quantity", quantity);
                    cmdUpdateRecord.Parameters.AddWithValue("@ProductSKU", productSKU);

                    cmdUpdateRecord.ExecuteNonQuery();
                    con.Close();
                }
            }
            catch (Exception ex)
            {
                Buttons.ShowMessage(ex.Message);
            }

        }

        public static void RestoreProductQuantityByProductName(string productName, int quantity)
        {
            try
            {
                string pathtoDB = Path.Combine(ApplicationData.Current.LocalFolder.Path, "MyDatabase.db");

                using (SqliteConnection con = new SqliteConnection($"Filename={pathtoDB}"))
                {
                    con.Open();
                    string updateCMD = @"UPDATE ProductDetails SET ProductQuantity = ProductQuantity + @Quantity WHERE ProductName = @ProductName";

                    SqliteCommand cmdUpdateRecord = new SqliteCommand(updateCMD, con);
                    cmdUpdateRecord.Parameters.AddWithValue("@Quantity", quantity);
                    cmdUpdateRecord.Parameters.AddWithValue("@ProductName", productName);

                    cmdUpdateRecord.ExecuteNonQuery();
                    con.Close();
                }
            }
            catch (Exception ex)
            {
                Buttons.ShowMessage(ex.Message);
            }
        }



        // Method to Pass Product to Cart

        public static void PassProductToCart(ProductDetails productDetails, int quantity, int buyer_ID, int seller_ID, int productdetails_ID, long productSKU)
        {
            try
            {
                // Call the AddProductToCart method with the details from the ProductDetails object
                AddProductToCart(productDetails.ProductName, productDetails.ProductPrice, productDetails.ProductCategory, quantity, productDetails.ProductPicture, buyer_ID, seller_ID, productdetails_ID, productSKU);

                // Decrease the quantity of the product in the ProductDetails table
                DecreaseProductQuantity(productDetails.ProductSKU, quantity);
            }
            catch (Exception ex)
            {
                Buttons.ShowMessage(ex.Message);
            }

        }


        /// <summary>
        /// PRODUCT RECEIPT
        /// </summary>

        //Initializing for Product Receipt Database

        public async static void InitializeDB_PRODUCTRECEIPT()
        {
            try
            {
                await ApplicationData.Current.LocalFolder.CreateFileAsync("MyDatabase.db", CreationCollisionOption.OpenIfExists);
                string pathtoDB = Path.Combine(ApplicationData.Current.LocalFolder.Path, "MyDatabase.db");

                using (SqliteConnection con = new SqliteConnection($"Filename={pathtoDB}"))
                {
                    con.Open();
                    string initCMD = @"CREATE TABLE IF NOT EXISTS ProductReceipts (
            PRODUCT_RECEIPT_ID INTEGER PRIMARY KEY AUTOINCREMENT,
            OrderNumber INTEGER NOT NULL,
            ProductName TEXT NOT NULL,                  
            ProductCategory TEXT NOT NULL,
            ProductPrice REAL,
            ProductQuantity INTEGER,
            LastName TEXT NOT NULL, 
            FirstName TEXT NOT NULL,
            MiddleName TEXT NOT NULL,
            PhoneNumber TEXT NOT NULL,
            AddressLine1 TEXT NOT NULL,
            AddressLine2 TEXT NOT NULL,
            Email TEXT NOT NULL,
            PaymentMethod TEXT NOT NULL,
            DatePurchased DATE NOT NULL,
            Buyer_ID INTEGER NOT NULL,
            Seller_ID INTEGER NOT NULL,
            FOREIGN KEY (Buyer_ID) REFERENCES Buyers(BUYER_ID),
            FOREIGN KEY (Seller_ID) REFERENCES Sellers(SELLER_ID)
                    )";

                    SqliteCommand CMDcreateTable = new SqliteCommand(initCMD, con);
                    CMDcreateTable.ExecuteReader();
                    con.Close();
                }
            }
            catch (Exception ex)
            {
                Buttons.ShowMessage(ex.Message);
            }

        }


        // Method to add the cart items to the receipt with a random 12 digit order number
        public static void AddProductToReceipt(string productName, string lastName, string firstName, string middleName, string phoneNumber, string productCategory, double productPrice, int productQuantity, string addressLine1, string addressLine2, string email, string paymentMethod, DateTime datePurchased, int buyer_ID, int seller_ID)
        {
            try
            {
                string pathtoDB = Path.Combine(ApplicationData.Current.LocalFolder.Path, "MyDatabase.db");

                using (SqliteConnection con = new SqliteConnection($"Filename={pathtoDB}"))
                {
                    con.Open();

                    // Generate a random 12-digit order number
                    Random random = new Random();
                    long orderNumber;
                    bool isUniqueOrderNumber = false;

                    // Loop until a unique order number is generated
                    do
                    {
                        orderNumber = (long)(random.NextDouble() * (999999999999L - 100000000000L) + 100000000000L);
                        string checkOrderNumberQuery = "SELECT COUNT(*) FROM ProductReceipts WHERE OrderNumber = @OrderNumber";

                        using (SqliteCommand cmdCheckOrderNumber = new SqliteCommand(checkOrderNumberQuery, con))
                        {
                            cmdCheckOrderNumber.Parameters.AddWithValue("@OrderNumber", orderNumber);
                            long existingCount = (long)cmdCheckOrderNumber.ExecuteScalar();

                            if (existingCount == 0)
                                isUniqueOrderNumber = true;
                        }
                    } while (!isUniqueOrderNumber);

                    string insertCMD = @"INSERT INTO ProductReceipts (OrderNumber, ProductName, LastName, FirstName, MiddleName, PhoneNumber, ProductCategory, ProductPrice, ProductQuantity, AddressLine1, AddressLine2, Email, PaymentMethod, DatePurchased, Buyer_ID, Seller_ID) 
                             VALUES (@OrderNumber, @ProductName, @LastName, @FirstName, @MiddleName, @PhoneNumber, @ProductCategory, @ProductPrice, @ProductQuantity, @AddressLine1, @AddressLine2, @Email,  @PaymentMethod, @DatePurchased, @Buyer_ID, @Seller_ID)";

                    SqliteCommand cmdInsertRecord = new SqliteCommand(insertCMD, con);
                    cmdInsertRecord.Parameters.AddWithValue("@OrderNumber", orderNumber);
                    cmdInsertRecord.Parameters.AddWithValue("@ProductName", productName);
                    cmdInsertRecord.Parameters.AddWithValue("@LastName", lastName);
                    cmdInsertRecord.Parameters.AddWithValue("@FirstName", firstName);
                    cmdInsertRecord.Parameters.AddWithValue("@MiddleName", middleName);
                    cmdInsertRecord.Parameters.AddWithValue("@PhoneNumber", phoneNumber);
                    cmdInsertRecord.Parameters.AddWithValue("@ProductCategory", productCategory);
                    cmdInsertRecord.Parameters.AddWithValue("@ProductPrice", productPrice);
                    cmdInsertRecord.Parameters.AddWithValue("@ProductQuantity", productQuantity);
                    cmdInsertRecord.Parameters.AddWithValue("@AddressLine1", addressLine1);
                    cmdInsertRecord.Parameters.AddWithValue("@AddressLine2", addressLine2);
                    cmdInsertRecord.Parameters.AddWithValue("@PaymentMethod", paymentMethod);
                    cmdInsertRecord.Parameters.AddWithValue("@Email", email);
                    cmdInsertRecord.Parameters.AddWithValue("@DatePurchased", datePurchased);
                    cmdInsertRecord.Parameters.AddWithValue("@Buyer_ID", buyer_ID);
                    cmdInsertRecord.Parameters.AddWithValue("@Seller_ID", seller_ID);



                    cmdInsertRecord.ExecuteNonQuery();
                    con.Close();
                }
            }
            catch (Exception ex)
            {
                Buttons.ShowMessage(ex.Message);
            }


        }

        // Query to Retrieve All Product Receipt by Order Number
        public static List<ProductReceipt> GetProductReceipts()
        {
            List<ProductReceipt> productReceiptList = new List<ProductReceipt>();

            string pathtoDB = Path.Combine(ApplicationData.Current.LocalFolder.Path, "MyDatabase.db");

            using (SqliteConnection con = new SqliteConnection($"Filename={pathtoDB}"))
            {
                con.Open();
                string selectCMD = "SELECT * FROM ProductReceipts";

                SqliteCommand cmdSelectRecords = new SqliteCommand(selectCMD, con);
                SqliteDataReader reader = cmdSelectRecords.ExecuteReader();

                while (reader.Read())
                {
                    ProductReceipt productReceipt = new ProductReceipt();
                    productReceipt.PRODUCTRECEIPT_ID = reader.GetInt32(0);
                    productReceipt.OrderNumber = reader.GetInt64(1);
                    productReceipt.ProductName = reader.GetString(2);
                    productReceipt.ProductCategory = reader.GetString(3);
                    productReceipt.ProductPrice = reader.GetDouble(4);
                    productReceipt.ProductQuantity = reader.GetInt32(5);
                    productReceipt.LastName = reader.GetString(6);
                    productReceipt.FirstName = reader.GetString(7);
                    productReceipt.MiddleName = reader.GetString(8);
                    productReceipt.PhoneNumber = reader.GetString(9);
                    productReceipt.AddressLine1 = reader.GetString(10);
                    productReceipt.AddressLine2 = reader.GetString(11);
                    productReceipt.Email = reader.GetString(12);
                    productReceipt.PaymentMethod = reader.GetString(13);
                    productReceipt.DatePurchased = reader.GetDateTime(14);
                    productReceipt.Buyer_ID = reader.GetInt32(15);
                    productReceipt.Seller_ID = reader.GetInt32(16);

                    productReceiptList.Add(productReceipt);
                }

                reader.Close();
                con.Close();
            }

            return productReceiptList;
        }

        // Query to Retrieve Only Product

        public static List<ProductReceipt> GetProductOnlyReceipts()
        {
            List<ProductReceipt> productReceiptList = new List<ProductReceipt>();

            string pathtoDB = Path.Combine(ApplicationData.Current.LocalFolder.Path, "MyDatabase.db");

            using (SqliteConnection con = new SqliteConnection($"Filename={pathtoDB}"))
            {
                con.Open();
                string selectCMD = "SELECT PRODUCT_RECEIPT_ID, Buyer_ID, Seller_ID, OrderNumber, ProductName, ProductCategory, ProductPrice, ProductQuantity FROM ProductReceipts";

                SqliteCommand cmdSelectRecords = new SqliteCommand(selectCMD, con);
                SqliteDataReader reader = cmdSelectRecords.ExecuteReader();

                while (reader.Read())
                {
                    ProductReceipt productReceipt = new ProductReceipt();

                    productReceipt.PRODUCTRECEIPT_ID = reader.GetInt32(0);
                    productReceipt.Buyer_ID = reader.GetInt32(1);
                    productReceipt.Seller_ID = reader.GetInt32(2);
                    productReceipt.OrderNumber = reader.GetInt64(3);
                    productReceipt.ProductName = reader.GetString(4);
                    productReceipt.ProductCategory = reader.GetString(5);
                    productReceipt.ProductPrice = reader.GetDouble(6);
                    productReceipt.ProductQuantity = reader.GetInt32(7);


                    productReceiptList.Add(productReceipt);
                }

                reader.Close();
                con.Close();
            }

            return productReceiptList;
        }

        // Query to Retrieve Only Customer Info
        public static List<ProductReceipt> GetCustomerInfoReceipts()
        {
            List<ProductReceipt> productReceiptList = new List<ProductReceipt>();

            string pathtoDB = Path.Combine(ApplicationData.Current.LocalFolder.Path, "MyDatabase.db");

            using (SqliteConnection con = new SqliteConnection($"Filename={pathtoDB}"))
            {
                con.Open();
                string selectCMD = "SELECT PRODUCT_RECEIPT_ID, Buyer_ID, Seller_ID, OrderNumber, LastName, FirstName, MiddleName, Email FROM ProductReceipts";

                SqliteCommand cmdSelectRecords = new SqliteCommand(selectCMD, con);
                SqliteDataReader reader = cmdSelectRecords.ExecuteReader();

                while (reader.Read())
                {
                    ProductReceipt productReceipt = new ProductReceipt();

                    productReceipt.PRODUCTRECEIPT_ID = reader.GetInt32(0);
                    productReceipt.Buyer_ID = reader.GetInt32(1);
                    productReceipt.Seller_ID = reader.GetInt32(2);
                    productReceipt.OrderNumber = reader.GetInt64(3);
                    productReceipt.LastName = reader.GetString(4);
                    productReceipt.FirstName = reader.GetString(5);
                    productReceipt.MiddleName = reader.GetString(6);
                    productReceipt.Email = reader.GetString(7);

                    productReceiptList.Add(productReceipt);
                }

                reader.Close();
                con.Close();
            }

            return productReceiptList;
        }


        // Query to Retrieve Only Customer Address
        public static List<ProductReceipt> GetCustomerAddressReceipts()
        {
            List<ProductReceipt> productReceiptList = new List<ProductReceipt>();

            string pathtoDB = Path.Combine(ApplicationData.Current.LocalFolder.Path, "MyDatabase.db");

            using (SqliteConnection con = new SqliteConnection($"Filename={pathtoDB}"))
            {
                con.Open();
                string selectCMD = "SELECT PRODUCT_RECEIPT_ID, Buyer_ID, Seller_ID, OrderNumber, AddressLine1, AddressLine2 FROM ProductReceipts";

                SqliteCommand cmdSelectRecords = new SqliteCommand(selectCMD, con);
                SqliteDataReader reader = cmdSelectRecords.ExecuteReader();

                while (reader.Read())
                {
                    ProductReceipt productReceipt = new ProductReceipt();

                    productReceipt.PRODUCTRECEIPT_ID = reader.GetInt32(0);
                    productReceipt.Buyer_ID = reader.GetInt32(1);
                    productReceipt.Seller_ID = reader.GetInt32(2);
                    productReceipt.OrderNumber = reader.GetInt64(3);
                    productReceipt.AddressLine1 = reader.GetString(4);
                    productReceipt.AddressLine2 = reader.GetString(5);


                    productReceiptList.Add(productReceipt);
                }

                reader.Close();
                con.Close();
            }

            return productReceiptList;
        }

        // Query to Retrieve Only Customer Address
        public static List<ProductReceipt> GetPaymentReceipts()
        {
            List<ProductReceipt> productReceiptList = new List<ProductReceipt>();

            string pathtoDB = Path.Combine(ApplicationData.Current.LocalFolder.Path, "MyDatabase.db");

            using (SqliteConnection con = new SqliteConnection($"Filename={pathtoDB}"))
            {
                con.Open();
                string selectCMD = "SELECT PRODUCT_RECEIPT_ID, Buyer_ID, Seller_ID, OrderNumber, PaymentMethod, DatePurchased FROM ProductReceipts";

                SqliteCommand cmdSelectRecords = new SqliteCommand(selectCMD, con);
                SqliteDataReader reader = cmdSelectRecords.ExecuteReader();

                while (reader.Read())
                {
                    ProductReceipt productReceipt = new ProductReceipt();

                    productReceipt.PRODUCTRECEIPT_ID = reader.GetInt32(0);
                    productReceipt.Buyer_ID = reader.GetInt32(1);
                    productReceipt.Seller_ID = reader.GetInt32(2);
                    productReceipt.OrderNumber = reader.GetInt64(3);
                    productReceipt.PaymentMethod = reader.GetString(4);
                    productReceipt.DatePurchased = reader.GetDateTime(5);

                    productReceiptList.Add(productReceipt);
                }

                reader.Close();
                con.Close();
            }

            return productReceiptList;
        }

        // Modify the GetProductReceipts method to accept Buyer_ID as a parameter
        public static List<ProductReceipt> GetBuyerProductReceipts(int buyerId)
        {
            List<ProductReceipt> buyerProductReceipts = new List<ProductReceipt>();

            // Fetch product receipts from the database filtered by buyer ID
            string pathtoDB = Path.Combine(ApplicationData.Current.LocalFolder.Path, "MyDatabase.db");

            using (SqliteConnection con = new SqliteConnection($"Filename={pathtoDB}"))
            {
                con.Open();
                string selectCMD = "SELECT * FROM ProductReceipts WHERE Buyer_ID = @BuyerId";

                SqliteCommand cmdSelectRecords = new SqliteCommand(selectCMD, con);
                cmdSelectRecords.Parameters.AddWithValue("@BuyerId", buyerId);

                SqliteDataReader reader = cmdSelectRecords.ExecuteReader();

                while (reader.Read())
                {
                    ProductReceipt productReceipt = new ProductReceipt();
                    productReceipt.PRODUCTRECEIPT_ID = reader.GetInt32(0);
                    productReceipt.OrderNumber = reader.GetInt64(1);
                    productReceipt.ProductName = reader.GetString(2);
                    productReceipt.ProductCategory = reader.GetString(3);
                    productReceipt.ProductPrice = reader.GetDouble(4);
                    productReceipt.ProductQuantity = reader.GetInt32(5);
                    productReceipt.LastName = reader.GetString(6);
                    productReceipt.FirstName = reader.GetString(7);
                    productReceipt.MiddleName = reader.GetString(8);
                    productReceipt.PhoneNumber = reader.GetString(9);
                    productReceipt.AddressLine1 = reader.GetString(10);
                    productReceipt.AddressLine2 = reader.GetString(11);
                    productReceipt.PaymentMethod = reader.GetString(12);
                    productReceipt.Email = reader.GetString(13);
                    productReceipt.DatePurchased = reader.GetDateTime(14);

                    buyerProductReceipts.Add(productReceipt);
                }

                reader.Close();
                con.Close();
            }

            return buyerProductReceipts;
        }


        // Method to pass the cart items to the receipt with product category, address, and date purchased
        public static void PassProductToReceipt(ProductCart productCart, string lastName, string firstName, string middleName, string phoneNumber, string productCategory, string addressLine1, string addressLine2, string email, string paymentMethod, DateTime datePurchased, int buyer_ID, int seller_ID)
        {
            try
            {
                // Call the AddProductToReceipt method with the details from the ProductCart object
                AddProductToReceipt(productCart.ProductName, lastName, firstName, middleName, phoneNumber, productCategory, productCart.ProductPrice, productCart.ProductQuantity + 1, addressLine1, addressLine2, email, paymentMethod, datePurchased, buyer_ID, seller_ID);

                // Delete the product from the cart
                DeleteProductFromCart(productCart.ProductCart_ID);
            }
            catch (Exception ex)
            {
                Buttons.ShowMessage(ex.Message);
            }

        }

    }
}