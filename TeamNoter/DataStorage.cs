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
                    // conn.Open();

                    string queryString = "SELECT t.TASK_ID, t.DEADLINE, t.TASK_NAME, t.TASK_DESCRIPTION, t.IS_COMPLETED, " +
                                             "(SELECT GROUP_CONCAT(u.USERNAME SEPARATOR ', ') " +
                                             "FROM USER_TASKS ut LEFT JOIN USERS u ON u.USER_ID = ut.USER_ID " +
                                             "WHERE ut.TASK_ID = t.TASK_ID) AS TASK_USERS " +
                                          "FROM TASKS t WHERE t.IS_COMPLETED = FALSE";
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
                    // conn.Close();
                }
                else
                    Utility.NoterMessage("Task error", "Empty connection. Cannot display tasks.");
            }
        }
    }
}
