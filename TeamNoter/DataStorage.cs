using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;


namespace TeamNoter
{
    public class DataStorage
    {
        public class TaskItem
        {
            public int TaskID { get; set; }
            public DateTime Deadline { get; set; }
            public string Title { get; set; }
            public string Details { get; set; }
            public string Users { get; set; }
            public bool IsCompleted { get; set; }

        }

        public ObservableCollection<TaskItem> tasks { get; set; } = new ObservableCollection<TaskItem>();

        public DataStorage()
        {
            using (MySqlConnection conn = dbConnect.GetConnection())
            {
                if (conn != null)
                {
                    conn.Open();

                    string queryString = "SELECT t.TASK_ID, t.DEADLINE, t.TASK_NAME, t.TASK_DESCRIPTION, t.IS_COMPLETED, " +
                                             "(SELECT GROUP_CONCAT(u.USERNAME SEPARATOR ', ') " +
                                             "FROM USER_TASKS ut LEFT JOIN USERS u ON u.USER_ID = ut.USER_ID " +
                                             "WHERE ut.TASK_ID = t.TASK_ID) AS TASK_USERS " +
                                          "FROM TASKS t ORDER BY DEADLINE ASC";
                                          // this will make ALL the tasks into items.
                    MySqlCommand query = new MySqlCommand(queryString, conn);

                    MySqlDataReader resultset = query.ExecuteReader();
                    while (resultset.Read())
                    {
                        var item = new TaskItem
                        {
                            TaskID = resultset.GetInt32("TASK_ID"),
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

        public CollectionView getTaskView()
        {
            CollectionView view = (CollectionView)CollectionViewSource.GetDefaultView(tasks);
            return view;
        }
    }
}
