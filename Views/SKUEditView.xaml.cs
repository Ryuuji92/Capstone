using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Text.RegularExpressions;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Capstone
{
    /// <summary>
    /// Interaction logic for AddSKUView.xaml
    /// </summary>
    public partial class SKUEditView : Window
    {
        private SKU Sku;
        private bool isNew;
        private bool isAssign;
        private bool isSaved;

        public SKUEditView()
        {
            InitializeComponent();
            this.Title = "Add SKU";
            WindowTitle.Text = "New SKU";
            isNew = true;
            isAssign = false;
            Sku = new SKU();
            Sku.Price = 0.00;
            Form.DataContext = Sku;
            ProdSKU.Text = "New SKU";

            isSaved = false;
        }

        public SKUEditView(Product product)
        {
            InitializeComponent();
            this.Title = "Add SKU";
            WindowTitle.Text = "New SKU";
            isNew = true;
            isAssign = true;
            Sku = new SKU(product);
            Sku.Price = 0.00;
            Form.DataContext = Sku;
            ProdSKU.Text = "Assign SKU to Product";

            isSaved = false;
        }

        public SKUEditView(int skuID)
        {
            InitializeComponent();
            this.Title = "Edit SKU";
            isNew = false;
            isAssign = false;
            Sku = App.Database.GetSkuAsync(skuID).Result;
            WindowTitle.Text = "Edit " + Sku.Name;

            Form.DataContext = Sku;

            isSaved = false;
        }

        private async void Validate()
        {
            int upc;
            int min;
            int max;
            double price;
            double cost;

            //Name
            if (string.IsNullOrEmpty(Sku.Name))
            {
                MessageBox.Show("Please enter a valid product name.", "Attention");
                return;
            }

            //Description
            if (string.IsNullOrEmpty(Sku.Description))
            {
                MessageBox.Show("Please enter a SKU description.", "Attention");
                return;
            }

            //Price
            if (!double.TryParse(ProdPrice.Text, out price))
            {
                MessageBox.Show("Please enter a valid price.  Price must be greater than $0.02");
                return;
            }

            if (price < 0.02)
            {
                MessageBox.Show("Please enter a valid price.  Price must be greater than $0.02", "Attention");
                return;
            }

            //UPC
            if (ProdUPC.Text.Length < 10)
            {
                MessageBox.Show("Please enter a valid 10 digit UPC. Please exclude the two end numbers outside of the barcode.", "Attention");
                return;
            }

            if (!int.TryParse(ProdUPC.Text, out int i))
            {
                MessageBox.Show("Please enter only numeric charicters in the UPC field.", "Attention");
                return;
            }

            //If this is a new product verify that the UPC isn't already in use elsewhere in the database
            if (isNew && !isAssign && App.Database.ExistsProductAsync(Sku.UPC))
            {
                MessageBox.Show("Product already exists. Please check both the SKUs and Products tables for the matching UPC.", "Attention");
                return;
            }

            //Manufacturer
            if (string.IsNullOrEmpty(Sku.Manufacturer))
            {
                MessageBox.Show("Please enter a manufaturer.", "Attention");
                return;
            }

            //Cost
            if (!double.TryParse(ProdCost.Text, out cost))
            {
                MessageBox.Show("Please enter a valid product cost. Cost must be greater than $0.00", "Attention");
                return;
            }

            if (cost < 0)
            {
                MessageBox.Show("Please enter a valid cost. Cost must be greater than $0.00.", "Attention");
                return;
            }

            //Min and max
            if (!int.TryParse(ProdMin.Text, out min) || !int.TryParse(ProdMax.Text, out max))
            {
                MessageBox.Show("Please enter a numeric value in the min and max fields.", "Attention");
                return;
            }

            if (min > max)
            {
                MessageBox.Show("Minimum inventory level cannot be more than maximum inventory level.", "Attention");
                return;
            }

            if (min < 0 || max < 0)
            {
                MessageBox.Show("Minimum and maximum inventory levels must be positive numbers.", "Attention");
                return;
            }

            //Assign non string values to the Sku object
            Sku.Price = price;
            Sku.Cost = cost;
            Sku.Min = min;
            Sku.Max = max;
            Sku.LastModified = DateTime.UtcNow;

            //Insert if new, update if exists
            if (isNew)
                await App.Database.InsertSKUAsync(Sku);
            else if (isAssign)
                await App.Database.AssignSKUAsync(Sku);
            else
                await App.Database.UpdateSKUAsync(Sku);

            isSaved = true;

            this.Close();
        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            Validate();
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            if (!isSaved)
            {
                switch (MessageBox.Show("Your work has not been saved. If you leave now you will lose your work. Do you want to save before you exit?", "Warning", MessageBoxButton.YesNoCancel))
                {
                    case MessageBoxResult.Yes:
                        e.Cancel = true;
                        Validate();
                        break;
                    case MessageBoxResult.No:
                        break;
                    case MessageBoxResult.Cancel:
                        e.Cancel = true;
                        break;
                }
            }

            base.OnClosing(e);
        }

        private void Numeric_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex("[^0-9]+$");
            e.Handled = regex.IsMatch(e.Text);
        }

        private void Currency_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex("^[+-]?[0-9]{1,3}(?:,?[0-9]{3})*\\.[0-9]{2}$");
            e.Handled = regex.IsMatch(e.Text);
        }
    }
}
