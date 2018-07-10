using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestShopping.Models
{
    public class BindableProduct : BindableBase
    {
        public BindableProduct(Product originalProduct)
        {
            OriginalProduct = originalProduct;
        }

        public bool Done
        {
            get { return OriginalProduct.Done; }
            set
            {
                OriginalProduct.Done = value;
                RaisePropertyChanged();
            }
        }

        public string Title
        {
            get { return OriginalProduct.Title; }
            set
            {
                OriginalProduct.Title = value;
                RaisePropertyChanged();
            }
        }

        public Product OriginalProduct { get; private set; }
    }
}
