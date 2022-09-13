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

namespace Capstone
{
    /// <summary>
    /// Interaction logic for SKUDetailView.xaml
    /// </summary>
    public partial class SKUDetailView : Window
    {
        private SKU Sku;

        public SKUDetailView(int skuID)
        {
            InitializeComponent();

            Sku = App.Database.GetSkuAsync(skuID).Result;

            WindowTitle.Text = Sku.Name;

            Form.DataContext = Sku;
        }

        private void Edit_Click(object sender, RoutedEventArgs e)
        {
            SKUEditView view = new SKUEditView(Sku.ID);
            view.ShowDialog();
            Sku = App.Database.GetSkuAsync(Sku.ID).Result;
        }

        private void Adjust_Click(object sender, RoutedEventArgs e)
        {
            SKUCountAdjView view = new SKUCountAdjView(Sku.ID);
            view.ShowDialog();
            Sku = App.Database.GetSkuAsync(Sku.ID).Result;
        }

        private void Exit_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
