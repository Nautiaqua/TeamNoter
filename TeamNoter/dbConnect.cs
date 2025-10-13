using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Diagnostics;

namespace TeamNoter
{
    internal class dbConnect
    {
        // it returns bool to show if it connected successfully or not.
        public static bool Connect(string server, string port, string database,
                                   string username, string password, string sslMode,
                                   string caPath)
        {
            string connectionString;

            if (sslMode == "verifyca")
            {
                connectionString =
                "Server=" + server + ";" +
                "Port=" + port + ";" +
                "Database=" + database + ";" +
                "Uid=" + username + ";" +
                "Pwd=" + password + ";" +
                "SslMode=" + sslMode + ";" +
                "SslCa=" + caPath + ";";
            }
            else
            {
                connectionString =
                "Server=" + server + ";" +
                "Port=" + port + ";" +
                "Database=" + database + ";" +
                "Uid=" + username + ";" +
                "Pwd=" + password + ";" +
                "SslMode=" + sslMode + ";";
            }

            using (MySqlConnection conn = new MySqlConnection(connectionString))
            {
                try
                {
                    conn.Open();
                    //MessageBox.Show("✅ Connected successfully!");
                    //Debug.WriteLine("✅ Connected successfully!");

                    Utility.NoterMessage("Connection successful", "Successfully");
                    return true; // returns true if successful
                }
                catch (Exception ex)
                {
                    //MessageBox.Show("❌ ERROR: " + ex.Message);
                    //Debug.WriteLine("❌ ERROR: " + ex.Message);

                    Utility.NoterMessage("Database connection failed", ex.Message);
                    return false; // returns false if unsuccessful
                }
            }
        }

    }
}
