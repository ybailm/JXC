using Project.ViewModel;
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

namespace Project.View
{
    /// <summary>
    /// SaleView.xaml 的交互逻辑
    /// </summary>
    public partial class SaleReportView : Window
    {
        public SaleReportView()
        {
            InitializeComponent();
            this.DataContext = new SaleReportViewModel();
        }

        private void dgSale_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            NewSaleView nsv = new NewSaleView(this.dgSale.SelectedItem as Sale);
            nsv.Show();
        }        

        private void btn_Close_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void btn_Refresh_Click(object sender, RoutedEventArgs e)
        {
            this.DataContext = new SaleViewModel();
        }
    }
}
