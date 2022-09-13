using System;
using System.IO;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace Capstone
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private static DBLogic database;
        public static DBLogic Database
        {
            get
            {
                if (database == null)
                {
                    database = new DBLogic(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "inventorydb.db3"));
                }
                return database;
            }
        }
    }
}
