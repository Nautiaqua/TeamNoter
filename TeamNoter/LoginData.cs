using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TeamNoter
{
    public class LoginData
    {
        public static int UserID { get; set; } = 0;
        public static string Username { get; set; } = "N/A";
        public static string Email { get; set; } = "N/A";
        public static string Password { get; set; } = "N/A";
        public static int TasksCompleted { get; set; } = 0;
        public static int TasksAssigned { get; set; } = 0;
        public static string AccountType { get; set; } = "N/A";
    }

    public class LoginCache
    {
        public string Server { get; set; } = "N/A";
        public string Port { get; set; } = "N/A";
        public string dbUsername { get; set; } = "N/A";
        public string dbPassword { get; set; } = "N/A";
        public string Email { get; set; } = "N/A";
        public string userPassword { get; set; } = "N/A";
        public bool isRemembering { get; set; }
    }
}
