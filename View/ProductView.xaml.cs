using Project.Extentions;
using Project.Model;
using Project.ViewModel;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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

namespace Project.View
{
    /// <summary>
    /// NewPurchaseView.xaml 的交互逻辑
    /// </summary>
    public partial class ProductView : Window
    {
        public ProductView():this(null)
        {
            
        }
        public ProductView(ObservableCollection<Product> productCollection)
        {
            InitializeComponent();
            DataContext = new ProductViewModel(productCollection);
        }

        private void btn_Close_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void btn_Hide_Click(object sender, RoutedEventArgs e)
        {
            this.grid_Product.Visibility = Visibility.Collapsed;
        }

        private void txtProductName_TextChanged(object sender, TextChangedEventArgs e)
        {
            this.tbPY.Text = this.txtProductName.Text.ConvertToCaptal();
        }
    }
}
