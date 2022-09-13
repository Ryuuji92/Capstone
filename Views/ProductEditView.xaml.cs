using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text.RegularExpressions;
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
    /// Interaction logic for AddProductView.xaml
    /// </summary>
    public partial class ProductEditView : Window
    {
        private Product product;
        private bool isNew;
        private bool isSaved;

        public ProductEditView()
        {
            InitializeComponent();

            product = new Product();
            Form.DataContext = product;

            isNew = true;
            isSaved = false;
            WindowTitle.Text = "New Product";
        }

        public ProductEditView(int prodID)
        {
            InitializeComponent();

            product = App.Database.GetProductAsync(prodID).Result;
            Form.DataContext = product;

            isNew = false;
            isSaved = false;
            WindowTitle.Text = "Edit Product";
        }

        private void Validate()
        {
            double cost;

            //UPC
            if (ProdUPC.Text.Length != 10)
            {
                MessageBox.Show("Please enter a valid 10 digit UPC.", "Attention");
                return;
            }

            if (!int.TryParse(ProdUPC.Text, out int i))
            {
                MessageBox.Show("Please enter a valid 10 digit UPC. Please use numbers only.", "Attention");
                return;
            }

            //Check if the UPC already exists in the database if the product is new
            if (isNew && App.Database.ExistsProductAsync(product.UPC))
            {
                MessageBox.Show("Product already exists. Please check both the Products and SKUs tables for your product.", "Attention");
                return;
            }

            //Manufacturer
            if (string.IsNullOrEmpty(ProdManufacturer.Text))
            {
                MessageBox.Show("Please enter a manufacturer.", "Attention");
                return;
            }

            //Cost
            if (!double.TryParse(ProdCost.Text, out cost))
            {
                MessageBox.Show("Please enter a valid product cost.", "Attention");
                return;
            }

            if (cost < 0)
            {
                MessageBox.Show("Please enter a valid cost.  Cost must be greater than $0.00");
                return;
            }

            product.Cost = cost;

            if(isNew)
                App.Database.InsertProductAsync(product);
            else
                App.Database.UpdateProductAsync(product);

            isSaved = true;
            this.Close();
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

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            Validate();
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            isSaved = true;
            this.Close();
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            if (!isSaved)
            {
                switch (MessageBox.Show("You are about to quit without saving your work. Would you like to save?", "Attention", MessageBoxButton.YesNoCancel))
                {
                    case MessageBoxResult.Yes:
                        Validate();
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
