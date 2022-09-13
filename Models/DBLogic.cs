using System;
using SQLite;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Capstone
{
    public class DBLogic
    {
        readonly SQLiteAsyncConnection database;

        public DBLogic(string connstring)
        {
            database = new SQLiteAsyncConnection(connstring);
            database.CreateTableAsync<Product>().Wait();
            database.CreateTableAsync<SKU>().Wait();
        }

        public Task<List<SKU>> GetSKUsAsync()
        {
            return database.Table<SKU>().ToListAsync();
        }

        public Task<SKU> GetSkuAsync(int ID)
        {
            return database.FindAsync<SKU>(ID);
        }

        public Task<List<Product>> GetProductsAsync()
        {
            return database.Table<Product>().ToListAsync();
        }

        public Task<Product> GetProductAsync(int upc)
        {
            return database.FindAsync<Product>(upc);
        }

        public Task<int> InsertSKUAsync(SKU sku)
        {
            sku.LastModified = DateTime.UtcNow;
            return database.InsertAsync(sku);
        }

        public Task<int> InsertProductAsync(Product product)
        {
            product.LastModified = DateTime.UtcNow;
            return database.InsertAsync(product);
        }

        public Task<int> UpdateSKUAsync(SKU sku)
        {
            sku.LastModified = DateTime.UtcNow;
            return database.UpdateAsync(sku);
        }

        public Task<int> UpdateProductAsync(Product product)
        {
            product.LastModified = DateTime.UtcNow;
            return database.UpdateAsync(product);
        }

        public Task<int> DeleteSKUAsync(SKU sku)
        {
            Product prod = new Product(sku);
            database.InsertAsync(prod);
            return database.DeleteAsync(sku);
        }

        public Task<int> DeleteProductAsync(Product product)
        {
            return database.DeleteAsync(product);
        }

        public Task<int> AssignSKUAsync(SKU sku)
        {
            database.Table<Product>().DeleteAsync(x => x.UPC == sku.UPC);
            return database.InsertAsync(sku);
        }

        public bool ExistsProductAsync(string upc)
        {
            int count1 = database.Table<Product>().CountAsync(x => x.UPC == upc).Result;
            int count2 = database.Table<SKU>().CountAsync(x => x.UPC == upc).Result;

            int exists = count1 + count2;

            if (exists > 0)
                return true;
            else
                return false;
        }
    }
}
