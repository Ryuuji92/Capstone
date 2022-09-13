using System;
using System.Collections.ObjectModel;
using System.Timers;
using System.ComponentModel;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Capstone
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private ObservableCollection<SKU> SKUList;

        private static Timer dayTimer;
        private int counter = 0;

        public MainWindow()
        {
            InitializeComponent();
            SKUList = new ObservableCollection<SKU>();
            
            PopulateList();
            purgeOldProduct();

            setTimer();
        }

        public void PopulateList()
        {
            SKUList.Clear();
            SKUTable.DataContext = SKUList;
            SearchBox.Text = "";

            foreach (SKU sku in App.Database.GetSKUsAsync().Result)
            {
                SKUList.Add(sku);
            }
        }

        private void setTimer()
        {
            const double ticks = 1125000000;
            dayTimer = new Timer(ticks);
            dayTimer.Elapsed += DailyDBCheck;
            dayTimer.AutoReset = true;
            dayTimer.Start();
        }

        private void DailyDBCheck(Object source, ElapsedEventArgs e)
        {
            counter++;
            if (counter >= 768)
            {
                this.Dispatcher.Invoke(purgeOldProduct);
                this.Dispatcher.Invoke(PopulateList);
                counter = 0;
            }
        }

        private void purgeOldProduct()
        {
            foreach (Product product in App.Database.GetProductsAsync().Result)
            {
                if (product.IsActive == false && product.LastModified > DateTime.Now.AddDays(-30)) 
                    App.Database.DeleteProductAsync(product);
            }
        }

        private void SlowReport_Click(object sender, RoutedEventArgs e)
        {

        }

        private void NegativeReport_Click(object sender, RoutedEventArgs e)
        {
            NegativeReportView view = new NegativeReportView();
            view.ShowDialog();
        }

        private void Products_Click(object sender, RoutedEventArgs e)
        {
            ProductsView view = new ProductsView();
            view.ShowDialog();
            PopulateList();
        }

        private void AddSKU_Click(object sender, RoutedEventArgs e)
        {
            SKUEditView view = new SKUEditView();
            view.ShowDialog();
            PopulateList();
        }

        private void SearchBox_KeyUp(object sender, KeyEventArgs e)
        {
            if (SearchBox.Text.Length == 0)
            {
                PopulateList();
            }
        }

        private void SearchButton_Click(object sender, RoutedEventArgs e)
        {
            List<SKU> searchResults = new List<SKU>();

            string[] keywords = SearchBox.Text.Split(new char[] { ' ' });
            for (int i = 0; i < keywords.Length; i++)
            {
                foreach (SKU sku in SKUList)
                {
                    if (sku.ID.ToString().Contains(keywords[i]))
                        searchResults.Add(sku);
                    if (sku.Name.Contains(keywords[i]))
                        searchResults.Add(sku);
                    if (sku.Description.Contains(keywords[i]))
                        searchResults.Add(sku);
                    if (sku.UPC.Contains(keywords[i]))
                        searchResults.Add(sku);
                }
            }

            if (searchResults.Count < 1)
            {
                MessageBox.Show("No results found.", "Attention");
                return;
            }

            SKUTable.DataContext = searchResults;
        }

        private void Adjust_Click(object sender, RoutedEventArgs e)
        {
            SKU sku = SKUTable.SelectedItem as SKU;
            SKUCountAdjView view = new SKUCountAdjView(sku.ID);
            view.ShowDialog();
            PopulateList();
        }

        private void Details_Click(object sender, RoutedEventArgs e)
        {
            SKU sku = SKUTable.SelectedItem as SKU;
            SKUDetailView view = new SKUDetailView(sku.ID);
            view.ShowDialog();
            PopulateList();
        }

        private void Delete_Click(object sender, RoutedEventArgs e)
        {
            SKU sku = SKUTable.SelectedItem as SKU;
            if (sku.Count > 0)
            {
                MessageBox.Show("SKU shows current on hand greater than 0. Please check inventory, adjust on hand count, and then delete.", "Attention");
                return;
            }

            if (MessageBox.Show("You are about to perminently delete this SKU. Do you want to continue?", "Warning", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
            {
                App.Database.DeleteSKUAsync(sku);
                MessageBox.Show("SKU has been deleted!", "Success");
                PopulateList();
            }
        }
    }
}
