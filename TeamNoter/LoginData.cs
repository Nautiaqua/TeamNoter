using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TeamNoter
{
    public static class LoginData
    {
        public static int UserID { get; set; } = 0;
        public static string Username { get; set; } = "N/A";
        public static string Email { get; set; } = "N/A";
        public static string Password { get; set; } = "N/A";
        public static int TasksCompleted { get; set; } = 0;
        public static int TasksAssigned { get; set; } = 0;
        public static string AccountType { get; set; } = "N/A";
    }
}
