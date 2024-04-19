
using Microsoft.Data.Sqlite;
using System;
using System.IO;
using Windows.Storage;

namespace Ferdin_TB_Hub.Classes
{
    internal class BuyerAndSellerID
    {
        public static int BuyerID { get; set; }
        public static int SellerID { get; set; }
    }

    internal class DatabaseAccess
    {
        public int RetrieveBuyerIDFromDatabase(string username)
        {
            int buyerID = -1; // Default value in case retrieval fails

            string pathtoDB = Path.Combine(ApplicationData.Current.LocalFolder.Path, "MyDatabase.db");

            // SQL query to retrieve the buyer's ID based on the username
            string query = "SELECT BUYER_ID FROM Buyers WHERE Username = @Username";

            using (SqliteConnection con = new SqliteConnection($"Filename={pathtoDB}"))
            {
                con.Open();

                using (SqliteCommand command = new SqliteCommand(query, con))
                {
                    // Add parameter for username
                    command.Parameters.AddWithValue("@Username", username);

                    // Execute the command to retrieve the buyer's ID
                    object result = command.ExecuteScalar();

                    // Check if the result is not null and is convertible to int
                    if (result != null && int.TryParse(result.ToString(), out buyerID))
                    {
                        // Set the BuyerID property of BuyerAndSellerID class
                        BuyerAndSellerID.BuyerID = buyerID;
                    }
                    else
                    {
                        // Handle the case where the buyer's ID could not be retrieved
                        // For example, display an error message or handle it based on your application's logic
                    }
                }

                con.Close();
            }

            return buyerID;
        }

        public int RetrieveSellerIDFromDatabase(string username)
        {
            int sellerID = -1; // Default value in case retrieval fails

            string pathtoDB = Path.Combine(ApplicationData.Current.LocalFolder.Path, "MyDatabase.db");

            // SQL query to retrieve the seller's ID based on the username
            string query = "SELECT SELLER_ID FROM Sellers WHERE Username = @Username";

            using (SqliteConnection con = new SqliteConnection($"Filename={pathtoDB}"))
            {
                con.Open();

                using (SqliteCommand command = new SqliteCommand(query, con))
                {
                    // Add parameter for username
                    command.Parameters.AddWithValue("@Username", username);

                    // Execute the command to retrieve the seller's ID
                    object result = command.ExecuteScalar();

                    // Check if the result is not null and is convertible to int
                    if (result != null && int.TryParse(result.ToString(), out sellerID))
                    {
                        // Set the SellerID property of BuyerAndSellerID class
                        BuyerAndSellerID.SellerID = sellerID;
                    }
                    else
                    {
                        // Handle the case where the seller's ID could not be retrieved
                        // For example, display an error message or handle it based on your application's logic
                    }
                }

                con.Close();
            }

            return sellerID;
        }
    }
}










/*
 using Microsoft.Data.Sqlite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO; // Import the System.IO namespace
using System.Threading.Tasks;
using Windows.Storage;

namespace Ferdin_TB_Hub.Classes
{
    internal class BuyerAndSellerID
    {
        public static int BuyerID { get; set; }
        public static int SellerID { get; set; }

    }

    internal class DatabaseAccess
    {
        public void RetrieveBuyerIDFromDatabase(string username)
        {
            string pathtoDB = Path.Combine(ApplicationData.Current.LocalFolder.Path, "MyDatabase.db");

            // SQL query to retrieve the buyer's ID based on the username
            string query = "SELECT BUYER_ID FROM Buyers WHERE Username = @Username";

            using (SqliteConnection con = new SqliteConnection($"Filename={pathtoDB}"))
            {
                con.Open();

                using (SqliteCommand command = new SqliteCommand(query, con))
                {
                    // Add parameter for username
                    command.Parameters.AddWithValue("@Username", username);

                    // Execute the command to retrieve the buyer's ID
                    object result = command.ExecuteScalar();

                    // Check if the result is not null and is convertible to int
                    if (result != null && int.TryParse(result.ToString(), out int buyerID))
                    {
                        // Set the BuyerID property of BuyerAndSellerID class
                        BuyerAndSellerID.BuyerID = buyerID;
                    }
                    else
                    {
                        // Handle the case where the buyer's ID could not be retrieved
                        // For example, display an error message or handle it based on your application's logic
                    }
                }

                con.Close();
            }
        }

        public void RetrieveSellerIDFromDatabase(string username)
        {
            string pathtoDB = Path.Combine(ApplicationData.Current.LocalFolder.Path, "MyDatabase.db");

            // SQL query to retrieve the seller's ID based on the username
            string query = "SELECT SELLER_ID FROM Sellers WHERE Username = @Username";

            using (SqliteConnection con = new SqliteConnection($"Filename={pathtoDB}"))
            {
                con.Open();

                using (SqliteCommand command = new SqliteCommand(query, con))
                {
                    // Add parameter for username
                    command.Parameters.AddWithValue("@Username", username);

                    // Execute the command to retrieve the seller's ID
                    object result = command.ExecuteScalar();

                    // Check if the result is not null and is convertible to int
                    if (result != null && int.TryParse(result.ToString(), out int sellerID))
                    {
                        // Set the SellerID property of SellerAndSellerID class
                        BuyerAndSellerID.SellerID = sellerID;
                    }
                    else
                    {
                        // Handle the case where the seller's ID could not be retrieved
                        // For example, display an error message or handle it based on your application's logic
                    }
                }

                con.Close();
            }
        }
    }
}

*/