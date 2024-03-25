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


namespace Ferdin_TB_Hub.Classes
{
    public class Database
    {
        public Database() 
        {
            InitializeDB_BUYERACCOUNTS();
            InitializeDB_SELLERACCOUNTS();
        }
        public class BuyerDetails
        {
            public int Id { get; set; }
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
            public int Id { get; set; }
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

        public class WaterProductDetails
        {
            public int Id { get; set; }

        }

        public class BuyerBoughtProductsDetails
        {
            public int Id { get; set; }

        }

        /// <summary>
        /// BUYER
        /// </summary>

        //Initializing for Buyer Accounts Database
        public async static void InitializeDB_BUYERACCOUNTS()
        {
            await ApplicationData.Current.LocalFolder.CreateFileAsync("MyDatabase.db", CreationCollisionOption.OpenIfExists);
            string pathtoDB = Path.Combine(ApplicationData.Current.LocalFolder.Path, "MyDatabase.db");

            using (SqliteConnection con = new SqliteConnection($"Filename={pathtoDB}")) 
            {
                con.Open();
                string initCMD = @"CREATE TABLE IF NOT EXISTS Buyers (
                            Id INTEGER PRIMARY KEY AUTOINCREMENT,
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
                string selectCMD = "SELECT Id, Email, Username, LastName, FirstName, MiddleName, Password, PhoneNumber, AddressLine1, AddressLine2 FROM Buyers WHERE Username = @Username OR Email = @Email";

                SqliteCommand cmdSelectRecords = new SqliteCommand(selectCMD, con);
                cmdSelectRecords.Parameters.AddWithValue("@Username", usernameOrEmail);
                cmdSelectRecords.Parameters.AddWithValue("@Email", usernameOrEmail);

                SqliteDataReader reader = cmdSelectRecords.ExecuteReader();

                if (reader.Read())
                {
                    buyer = new BuyerDetails();
                    buyer.Id = reader.GetInt32(0); // Assuming the ID is the first column in the result set
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
        public static void UpdateBuyerInfoFromDatabase(int id, string email, string username, string lastName, string firstName, string middleName, string password, string phoneNumber, string addressLine1, string addressLine2)
        {
            string pathtoDB = Path.Combine(ApplicationData.Current.LocalFolder.Path, "MyDatabase.db");

            using (SqliteConnection con = new SqliteConnection($"Filename={pathtoDB}"))
            {
                con.Open();
                string updateCMD = @"UPDATE Buyers SET Email = @Email, Username = @Username, LastName = @LastName, FirstName = @FirstName, 
                            MiddleName = @MiddleName, Password = @Password, PhoneNumber = @PhoneNumber, AddressLine1 = @AddressLine1, AddressLine2 = @AddressLine2 
                            WHERE Id = @Id";

                SqliteCommand cmdUpdateRecord = new SqliteCommand(updateCMD, con);
                cmdUpdateRecord.Parameters.AddWithValue("@Id", id); // Add Id parameter
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

        // Method to delete a buyer account from the database
        public static void DeleteBuyerAccountFromDatabase(string usernameOrEmail)
        {
            string pathtoDB = Path.Combine(ApplicationData.Current.LocalFolder.Path, "MyDatabase.db");

            using (SqliteConnection con = new SqliteConnection($"Filename={pathtoDB}"))
            {
                con.Open();
                string deleteCMD = "DELETE FROM Buyers WHERE Username = @Username OR Email = @Email";

                SqliteCommand cmdDeleteRecord = new SqliteCommand(deleteCMD, con);
                cmdDeleteRecord.Parameters.AddWithValue("@Username", usernameOrEmail);
                cmdDeleteRecord.Parameters.AddWithValue("@Email", usernameOrEmail);

                cmdDeleteRecord.ExecuteReader();
                con.Close();
            }
        }


        /// <summary>
        /// SELLER
        /// </summary>

        //Initializing for Seller Accounts Database
        public async static void InitializeDB_SELLERACCOUNTS()
        {
            await ApplicationData.Current.LocalFolder.CreateFileAsync("MyDatabase.db", CreationCollisionOption.OpenIfExists);
            string pathtoDB = Path.Combine(ApplicationData.Current.LocalFolder.Path, "MyDatabase.db");

            using (SqliteConnection con = new SqliteConnection($"Filename={pathtoDB}"))
            {
                con.Open();
                string initCMD = @"CREATE TABLE IF NOT EXISTS Sellers (
                            Id INTEGER PRIMARY KEY AUTOINCREMENT,
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
                string selectCMD = "SELECT Id, BusinessName, Email, Username, LastName, FirstName, MiddleName, Password, PhoneNumber, AddressLine1, AddressLine2 FROM Sellers WHERE Username = @Username OR Email = @Email";

                SqliteCommand cmdSelectRecords = new SqliteCommand(selectCMD, con);
                cmdSelectRecords.Parameters.AddWithValue("@Username", usernameOrEmail);
                cmdSelectRecords.Parameters.AddWithValue("@Email", usernameOrEmail);

                SqliteDataReader reader = cmdSelectRecords.ExecuteReader();

                if (reader.Read())
                {
                    seller = new SellerDetails();
                    seller.Id = reader.GetInt32(0); // Assuming the ID is the first column in the result set
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
        public static void UpdateSellerInfoFromDatabase(int id, string businessName, string email, string username, string lastName, string firstName, string middleName, string password, string phoneNumber, string addressLine1, string addressLine2)
        {
            string pathtoDB = Path.Combine(ApplicationData.Current.LocalFolder.Path, "MyDatabase.db");

            using (SqliteConnection con = new SqliteConnection($"Filename={pathtoDB}"))
            {
                con.Open();
                string updateCMD = @"UPDATE Sellers SET BusinessName = @BusinessName, Email = @Email, Username = @Username, LastName = @LastName, FirstName = @FirstName, 
                            MiddleName = @MiddleName, Password = @Password, PhoneNumber = @PhoneNumber, AddressLine1 = @AddressLine1, AddressLine2 = @AddressLine2 
                            WHERE Id = @Id";

                SqliteCommand cmdUpdateRecord = new SqliteCommand(updateCMD, con);
                cmdUpdateRecord.Parameters.AddWithValue("@Id", id); // Add Id parameter
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


        // Method to delete a seller account from the database
        public static void DeleteSellerAccountFromDatabase(string usernameOrEmail)
        {
            string pathtoDB = Path.Combine(ApplicationData.Current.LocalFolder.Path, "MyDatabase.db");

            using (SqliteConnection con = new SqliteConnection($"Filename={pathtoDB}"))
            {
                con.Open();
                string deleteCMD = "DELETE FROM Sellers WHERE Username = @Username OR Email = @Email";

                SqliteCommand cmdDeleteRecord = new SqliteCommand(deleteCMD, con);
                cmdDeleteRecord.Parameters.AddWithValue("@Username", usernameOrEmail);
                cmdDeleteRecord.Parameters.AddWithValue("@Email", usernameOrEmail);

                cmdDeleteRecord.ExecuteReader();
                con.Close();
            }
        }




    }
}
