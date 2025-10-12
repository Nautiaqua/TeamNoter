using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;

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
        public ObservableCollection<Item> tasks { get; set; }
        
        public DataStorage()
        {
            tasks = new ObservableCollection<Item>
            {
                new Item { Deadline = DateTime.Today, Title = "Finish C# Code", Details = "Complete missing C# functionality",
                Users = "@User1, @User2", IsCompleted = false},

                new Item { Deadline = DateTime.Today, Title = "Wrap up Java Project", Details = "Package and post Java project",
                Users = "@User1", IsCompleted = false },

                new Item { Deadline = DateTime.Today, Title = "Wrap up Java Project", Details = "Package and post Java project",
                Users = "@User1", IsCompleted = false },

                new Item { Deadline = DateTime.Today, Title = "Wrap up Java Project", Details = "Package and post Java project",
                Users = "@User1", IsCompleted = false },

                new Item { Deadline = DateTime.Today, Title = "Wrap up Java Project", Details = "Package and post Java project",
                Users = "@User1", IsCompleted = false },

                new Item { Deadline = DateTime.Today, Title = "Wrap up Java Project", Details = "Package and post Java project",
                Users = "@User1", IsCompleted = false },

                new Item { Deadline = DateTime.Today, Title = "Wrap up Java Project", Details = "Package and post Java project",
                Users = "@User1", IsCompleted = false },

                new Item { Deadline = DateTime.Today, Title = "Wrap up Java Project", Details = "Package and post Java project",
                Users = "@User1", IsCompleted = false },

                new Item { Deadline = DateTime.Today, Title = "Wrap up Java Project", Details = "Package and post Java project",
                Users = "@User1", IsCompleted = false },

                new Item { Deadline = DateTime.Today, Title = "Wrap up Java Project", Details = "Package and post Java project",
                Users = "@User1", IsCompleted = false },

                new Item { Deadline = DateTime.Today, Title = "Wrap up Java Project", Details = "Package and post Java project",
                Users = "@User1", IsCompleted = false },

                new Item { Deadline = DateTime.Today, Title = "Wrap up Java Project", Details = "Package and post Java project",
                Users = "@User1", IsCompleted = false },
            };
        }
    }

    
}
