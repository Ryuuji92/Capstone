using System;
using System.Text.RegularExpressions;
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
using System.ComponentModel;

namespace Capstone
{
    /// <summary>
    /// Interaction logic for SKUCountAdjView.xaml
    /// </summary>
    public partial class SKUCountAdjView : Window
    {
        private SKU Sku;
        private bool isSaved;

        public SKUCountAdjView(int skuID)
        {
            InitializeComponent();

            Sku = App.Database.GetSkuAsync(skuID).Result;

            WindowTitle.Text = Sku.Name;
            isSaved = false;

            Form.DataContext = Sku;
        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(NewCount.Text))
            {
                MessageBox.Show("Please enter a valid quantity.", "Attention");
                return;
            }
            save();
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            isSaved = true;
            this.Close();
        }

        private void NewCount_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex("[^0-9]+$");
            e.Handled = regex.IsMatch(e.Text);
        }

        private void save()
        {
            Sku.Count = int.Parse(NewCount.Text);
            App.Database.UpdateSKUAsync(Sku);
            isSaved = true;
            this.Close();
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            if (!isSaved)
            {
                switch (MessageBox.Show("You have not saved your work.  Would you like to do so before leaving?", "Attention", MessageBoxButton.YesNoCancel))
                {
                    case MessageBoxResult.Yes:
                        if (string.IsNullOrEmpty(NewCount.Text))
                        {
                            MessageBox.Show("Please enter a valid quantity.", "Attention");
                            return;
                        }
                        save();
                        break;
                    case MessageBoxResult.No:
                        this.Close();
                        break;
                    case MessageBoxResult.Cancel:
                        e.Cancel = true;
                        break;
                }
            }
            base.OnClosing(e);
        }
    }
}
