using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Data;

namespace Capstone
{
    /// <summary>
    /// Interaction logic for NegativeReportView.xaml
    /// </summary>
    public partial class NegativeReportView : Window
    {
        private List<SKU> Skus;

        public NegativeReportView()
        {
            InitializeComponent();

            Skus = new List<SKU>();

            foreach (SKU sku in App.Database.GetSKUsAsync().Result)
            {
                if (sku.Count < 0)
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
