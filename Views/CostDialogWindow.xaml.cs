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

namespace Capstone
{
    /// <summary>
    /// Interaction logic for CostDialogWindow.xaml
    /// </summary>
    public partial class CostDialogWindow : Window
    {
        private Product Product;

        public CostDialogWindow(Product product)
        {
            InitializeComponent();

            Product = product;

            Form.DataContext = Product;
        }

        private void Input_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex("^[+-]?[0-9]{1,3}(?:,?[0-9]{3})*\\.[0-9]{2}$");
            e.Handled = regex.IsMatch(e.Text);
        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            if (double.TryParse(Input.Text, out double cost))
            {
                Product.Reactivate(cost);
                App.Database.UpdateProductAsync(Product);
                this.Close();
            }

            else
            {
                Message.Text = "Please enter a new product cost, please use numbers only:";
                Message.Foreground = new SolidColorBrush(Colors.Red);
            }
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
