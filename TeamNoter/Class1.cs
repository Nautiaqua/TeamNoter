using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace TeamNoter
{
    internal class Class1
    {
        public static void Connect()
        {
            string connectionString =
                "Server=ibestupid-teamnoter.j.aivencloud.com;" +
                "Port=28069;" +
                "Database=defaultdb;" +
                "Uid=avnadmin;" +
                "Pwd=AVNS_qtIuKAzlGVweZJe6e-C;" +
                "SslMode=VerifyCA;" +
                "SslCa=C:\\ca.pem;";

            using (MySqlConnection conn = new MySqlConnection(connectionString))
            {
                try
                {
                    conn.Open();
                    MessageBox.Show("✅ Connected successfully!");
                }
                catch (Exception ex)
                {
                    MessageBox.Show("❌ ERROR: " + ex.Message);
                }
            }
        }

    }
}
