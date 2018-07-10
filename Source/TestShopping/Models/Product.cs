﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestShopping.Models
{
    public class Product
    {
        /// <summary>
        /// Primary key
        /// </summary>
        public int ProductId { get; private set; }

        public string Title { get; set; }
        public bool Done { get; set; }

        public Product(string title)
        {
            Title = title;
            Done = false;
        }

        public Product() { }
    }
}
