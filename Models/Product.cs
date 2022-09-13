using System;
using SQLite;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Capstone
{
    public class Product
    {
        public string UPC { get; set; }
        public string Manufacturer { get; set; }
        public double Cost { get; set; }
        public bool IsActive { get; set; }
        public DateTime LastModified { get; set; }

        public Product()
        {

        }

        public Product(SKU sku)
        {
            UPC = sku.UPC;
            Manufacturer = sku.Manufacturer;
            Cost = sku.Cost;
            IsActive = sku.IsActive;
        }

        public void Deactivate()
        {
            IsActive = false;
            Cost = 0.00;
        }

        public void Reactivate(double cost)
        {
            IsActive = true;
            Cost = cost;
        }
    }
}
