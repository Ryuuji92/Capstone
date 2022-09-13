using System;
using System.Collections.ObjectModel;
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
    /// Interaction logic for ProductsView.xaml
    /// </summary>
    public partial class ProductsView : Window
    {
        private ObservableCollection<Product> products;

        public ProductsView()
        {
            InitializeComponent();

            products = new ObservableCollection<Product>();

            PopulateList();
        }

        private void PopulateList()
        {
            products.Clear();
            SearchBox.Text = "";
            ProdTable.DataContext = products;

            foreach (Product product in App.Database.GetProductsAsync().Result)
            {
                products.Add(product);
            }
        }

        private void Add_Click(object sender, RoutedEventArgs e)
        {
            ProductEditView view = new ProductEditView();
            view.ShowDialog();
            PopulateList();
        }

        private void Assign_Click(object sender, RoutedEventArgs e)
        {
            Product prod = ProdTable.SelectedItem as Product;
            SKUEditView view = new SKUEditView(prod);
            view.ShowDialog();
            PopulateList();
        }

        private void Reactive_Click(object sender, RoutedEventArgs e)
        {
            if (MessageBox.Show("You are about to reactivate this product, do you want to continue?", "Attention", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
            {
                CostDialogWindow dialog = new CostDialogWindow(ProdTable.SelectedItem as Product);
                dialog.ShowDialog();
                PopulateList();
            }
        }

        private void Discontinued_Click(object sender, RoutedEventArgs e)
        {
            if (MessageBox.Show("You are about to mark this product as deactivated. Deactivated products will be automatically deleted after 30 days. Do you want to continue?", "Warning", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
            {
                Product product = ProdTable.SelectedItem as Product;
                product.Deactivate();
                MessageBox.Show("Product has been deactivated. You may reactivate the product within 30 days before it is deleted perminantly.", "Success");
                PopulateList();
            }
        }

        private void SearchButton_Click(object sender, RoutedEventArgs e)
        {
            List<Product> searchResults = new List<Product>();

            string[] keywords = SearchBox.Text.Split(new char[] { ' ' });
            for (int i = 0; i < keywords.Length; i++)
            {
                foreach (Product product in products)
                {
                    if (product.UPC.Contains(keywords[i]))
                        searchResults.Add(product);
                    if (product.Manufacturer.Contains(keywords[i]))
                        searchResults.Add(product);
                    if (product.Cost.ToString().Contains(keywords[i]))
                        searchResults.Add(product);
                }
            }

            if (searchResults.Count < 1)
            {
                MessageBox.Show("No results found.", "Attention");
                return;
            }

            ProdTable.DataContext = searchResults;
        }

        private void SearchBox_KeyUp(object sender, KeyEventArgs e)
        {

        }
    }
}
