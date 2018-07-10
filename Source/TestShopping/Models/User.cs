using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace TestShopping.Models
{

    public class User
    {
        /// <summary>
        /// Primary Key
        /// </summary>
        public int UserId { get; private set; }

        public string Name { get; private set; }

        public List<ShoppingList> ShoppingLists { get; set; }

        public User(string name)
        {
            Name = name;
            ShoppingLists = new List<ShoppingList>();
            ShoppingLists.Add(new ShoppingList());
        }

        public User() { }


    }
}
