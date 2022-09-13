using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Capstone.Views
{
    /// <summary>
    /// Interaction logic for SlowReportView.xaml
    /// </summary>
    public partial class SlowReportView : Window
    {
        private List<SKU> Skus;

        public SlowReportView()
        {
            InitializeComponent();

            Skus = new List<SKU>();

            foreach (SKU sku in App.Database.GetSKUsAsync().Result)
            {
                if (sku.LastModified > DateTime.Now.AddDays(-30))
                    Skus.Add(sku);
            }

            Form.DataContext = Skus;
        }

        private void Close_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
