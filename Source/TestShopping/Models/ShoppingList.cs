using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestShopping.Models
{
    public class ShoppingList
    {
        /// <summary>
        /// Primary key
        /// </summary>
        public int ShoppingListId { get; private set; }

        public int UserId { get; set; }

        public DateTime CreationDate {get;set;}

        public ShoppingList(int userId)
        {
            UserId = userId;
            CreationDate = DateTime.Now;
        }

        public ShoppingList() { }
    }
}
