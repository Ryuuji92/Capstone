using System;
using SQLitePCL;
using SQLite;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Capstone
{
    public class SKU : Product
    {
        [PrimaryKey, AutoIncrement]
        public int ID { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public double Price { get; set; }
        public int Count { get; set; }
        public int Min { get; set; }
        public int Max { get; set; }

        public SKU()
        {

        }

        public SKU(Product product)
        {
            UPC = product.UPC;
            Manufacturer = product.Manufacturer;
            Cost = product.Cost;
            IsActive = product.IsActive;
        }
    }
}
