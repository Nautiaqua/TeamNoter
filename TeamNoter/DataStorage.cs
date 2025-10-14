using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using MySql.Data.MySqlClient;


namespace TeamNoter
{
    public class DataStorage
    {
        // This class is placeholder data for NoterMain's Task List display
        // pls delete this later when we've got the database up and workin'
        // ~ Nautia
        public class Item
        {
            public DateTime Deadline { get; set; }
            public string Title { get; set; }
            public string Details { get; set; }
            public string Users { get; set; }
            public bool IsCompleted { get; set; }

        }

        public ObservableCollection<Item> tasks { get; set; } = new ObservableCollection<Item>();

        public DataStorage()
        {
            using (MySqlConnection conn = dbConnect.Connection)
            {
                if (conn != null)
                {
                    conn.Open();

                    string queryString = "SELECT DEADLINE, TASK_NAME, TASK_DESCRIPTION, TASK_USERS, IS_COMPLETED FROM TASKS WHERE IS_COMPLETED = FALSE";
                    MySqlCommand query = new MySqlCommand(queryString, conn);

                    MySqlDataReader resultset = query.ExecuteReader();
                    while (resultset.Read())
                    {
                        var item = new Item
                        {
                            Deadline = resultset.GetDateTime("DEADLINE"),
                            Title = resultset.GetString("TASK_NAME"),
                            Details = resultset.GetString("TASK_DESCRIPTION"),
                            Users = resultset.GetString("TASK_USERS"),
                            IsCompleted = resultset.GetBoolean("IS_COMPLETED")
                        };  

                        tasks.Add(item);
                    }

                    resultset.Close();
                    conn.Close();
                }
                else
                    Utility.NoterMessage("Task error", "Empty connection. Cannot display tasks.");
            }
        }
    }
}
