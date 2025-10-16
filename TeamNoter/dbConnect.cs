using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace TeamNoter
{
    internal class dbConnect
    {
        private static MySqlConnection? conn;
        static string connectionString = "";
        public static bool Connect(string server, string port, string database,
                                   string username, string password, string sslMode,
                                   string caPath)
        {
            try
            {
                if (sslMode == "VerifyCA")
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

                // assigns the connection string to 
                conn = new MySqlConnection(connectionString);

                // code that just checcs if we can connect properly
                try
                {
                    conn.Open();
                    // Utility.NoterMessage("Connected", "Sucessfully connected");
                    conn.Close();
                    return true; // returns true if successful
                }
                catch (Exception ex)
                {
                    Utility.NoterMessage("Database connection failed", ex.Message + " " + sslMode);
                    return false; // returns false if unsuccessful
                }
            }
            catch (Exception ex)
            {
                Utility.NoterMessage("Database connection failed", ex.Message + " " + sslMode);
                return false;
            }

            
        }

        // use this if you're opening a connection
        public static MySqlConnection GetConnection()
        {
            return new MySqlConnection(connectionString);
        }
    }
}
