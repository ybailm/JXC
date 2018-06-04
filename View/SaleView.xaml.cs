using Project.Print;
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
    public partial class SaleView : Window
    {
        public SaleView()
        {
            InitializeComponent();
            this.DataContext = new SaleViewModel();
        } 

        private void btn_Close_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void btn_Refresh_Click(object sender, RoutedEventArgs e)
        {
            this.DataContext = new SaleViewModel();
        }

        private void btn_Print_Click(object sender, RoutedEventArgs e)
        {
            var dc = this.DataContext as SaleViewModel;
            dc.Sale.SalePrice = dc.SumSalePrice;
            dc.Sale.ServicePrice = dc.SumServicePrice;
            dc.Sale.SumPrice = dc.SumPrice;
            var data = new SaleData { Sale = dc.Sale, SaleItemCollection = dc.SaleItemCollection, ServiceItemCollection = dc.ServiceItemCollection };
            PrintWindow pw = new PrintWindow("print\\SaleFlowDocument.xaml", data, new SaleDocumentRenderer());
            pw.ShowDialog();
        }
    }
}
