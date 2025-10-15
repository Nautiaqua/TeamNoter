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
        static string connectionString;
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

                try
                {
                    conn.Open();
                    Utility.NoterMessage("Connected", "Sucessfully connected");
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

        public static MySqlConnection GetConnection()
        {
            return new MySqlConnection(connectionString);
        }

        // allows us to retrieve the connection later throughout any point in the app by doing:
        // MySqlConnection conn = dbConnect.Connection;
        public static MySqlConnection? Connection => conn;
    }
}
