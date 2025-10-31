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
        public DataStorage()
        {
            _ = DataGather();
        }
        public class TaskItem
        {
            public int TaskID { get; set; }
            public DateTime Deadline { get; set; }
            public string Title { get; set; } = "n/a";
            public string Details { get; set; } = "n/a";
            public string Users { get; set; } = "n/a";
            public bool IsCompleted { get; set; }
            public string Priority { get; set; } = "Low";
            public string IsOverdue { get; set; }
        }

        public class UserItem
        {
            public int UserID { get; set; }
            public string Username { get; set; } = "n/a";
            public string Email { get; set; } = "n/a";
            // public string PASSWORD { get; set; } = "n/a";
            public DateTime Creation_Date { get; set; }
            public string Account_Type { get; set; } = "n/a";
            public int Completed_Tasks { get; set; }
            public int Assigned_Tasks { get; set; }
        }

        public ObservableCollection<TaskItem> tasks { get; set; } = new ObservableCollection<TaskItem>();
        public ObservableCollection<UserItem> users { get; set; } = new ObservableCollection<UserItem>();

        public async Task DataGather()
        {
            using (MySqlConnection conn = dbConnect.GetConnection())
            {
                if (conn != null)
                {
                    await conn.OpenAsync();

                    // Sets up the tasks table
                    string queryString = "SELECT t.TASK_ID, t.DEADLINE, t.TASK_NAME, t.TASK_DESCRIPTION, t.IS_COMPLETED, t.PRIORITY, " +
                                             "(SELECT GROUP_CONCAT(u.USERNAME SEPARATOR ', ') " +
                                             "FROM USER_TASKS ut LEFT JOIN USERS u ON u.USER_ID = ut.USER_ID " +
                                             "WHERE ut.TASK_ID = t.TASK_ID) AS TASK_USERS " +
                                          "FROM TASKS t ORDER BY DEADLINE ASC";
                                          // this will make ALL the tasks into items.
                    MySqlCommand query = new MySqlCommand(queryString, conn);

                    MySqlDataReader resultset = query.ExecuteReader();
                    while (await resultset.ReadAsync())
                    {
                        var item = new TaskItem
                        {
                            TaskID = resultset.GetInt32("TASK_ID"),
                            Deadline = resultset.IsDBNull(resultset.GetOrdinal("DEADLINE")) ? DateTime.MinValue
                                            : resultset.GetDateTime("DEADLINE"),
                            Title = resultset.IsDBNull(resultset.GetOrdinal("TASK_NAME")) ? "Untitled"
                                            : resultset.GetString("TASK_NAME"),
                            Details = resultset.IsDBNull(resultset.GetOrdinal("TASK_DESCRIPTION")) ? "N/A"
                                            : resultset.GetString("TASK_DESCRIPTION"),
                            Users = resultset.IsDBNull(resultset.GetOrdinal("TASK_USERS")) ? "Unassigned"
                                            : resultset.GetString("TASK_USERS"),
                            IsCompleted = !resultset.IsDBNull(resultset.GetOrdinal("IS_COMPLETED")) &&
                                           resultset.GetBoolean("IS_COMPLETED"),
                            Priority = resultset.IsDBNull(resultset.GetOrdinal("PRIORITY")) ? "Low"
                                            : char.ToUpper(resultset.GetString("PRIORITY").ToLower()[0]) + 
                                              resultset.GetString("PRIORITY").ToLower().Substring(1) + " Priority",

                            IsOverdue = resultset.GetDateTime("DEADLINE") < DateTime.Now ? "Overdue" : "Safe"
                            
                        };

                        tasks.Add(item);
                    }
                    resultset.Close();
                    // -----------


                    ////Sets up the users table
                    queryString = "SELECT u.USER_ID, u.USERNAME, u.EMAIL, u.CREATION_DATE, u.ACCOUNT_TYPE, " +
                                    "COUNT(ut.TASK_ID) AS ASSIGNED_TASKS, " +
                                    "(SELECT COUNT(*) FROM USER_TASKS JOIN TASKS ON USER_TASKS.TASK_ID = TASKS.TASK_ID WHERE USER_ID = u.USER_ID && IS_COMPLETED = TRUE) AS COMPLETED_TASKS " +
                                    "FROM USERS u LEFT JOIN USER_TASKS ut ON u.USER_ID = ut.USER_ID LEFT JOIN TASKS t ON ut.TASK_ID = t.TASK_ID " +
                                    "GROUP BY u.USER_ID, u.USERNAME, u.EMAIL, u.CREATION_DATE, u.ACCOUNT_TYPE";
                    query = new MySqlCommand(queryString, conn);
                    resultset = query.ExecuteReader();

                    while (resultset.Read())
                    {
                        var userItem = new UserItem
                        {
                            UserID = resultset.GetInt32("USER_ID"),
                            Username = resultset.GetString("USERNAME"),
                            Email = resultset.GetString("EMAIL"),
                            Creation_Date = resultset.GetDateTime("CREATION_DATE"),
                            Account_Type = resultset.GetString("ACCOUNT_TYPE"),
                            Completed_Tasks = resultset.GetInt32("ASSIGNED_TASKS"),
                            Assigned_Tasks = resultset.GetInt32("COMPLETED_TASKS"),
                        };

                        users.Add(userItem);
                    }
                    resultset.Close();
                    // -----------

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
