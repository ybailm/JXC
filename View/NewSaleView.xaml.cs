using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Project.ViewModel;
using System.Windows.Media;
using System.Collections.ObjectModel;

namespace Project.View
{
    /// <summary>
    /// SalesView.xaml 的交互逻辑
    /// </summary>
    public partial class NewSaleView : Window
    {
        public NewSaleView()
            :this(null,null,null)
        {

        }

        public NewSaleView(Sale sale) : this(sale,null,null)
        {
            
        }

        public NewSaleView(Sale sale, ObservableCollection<SaleDetailItem> saleItemCollection, ObservableCollection<ServiceDetailItem> serviceItemCollection)
        {
            InitializeComponent();
            var dc = new NewSaleViewModel(sale, saleItemCollection, serviceItemCollection);

            dc.SaleItemCollection.CollectionChanged += (seder, args) => ScrollToEnd(dgSaleItem);
            dc.ServiceItemCollection.CollectionChanged += (seder, args) => ScrollToEnd(dgServiceItem);

            DataContext = dc;         
        }

        

        private void btn_AddNewCustomer_Click(object sender, RoutedEventArgs e)
        {
            CustomerView cv = new View.CustomerView();
            cv.ShowDialog();
        }

        private void ScrollToEnd(DataGrid dg)
        {
            if (dg.Items.Count > 0)
            {
                if (VisualTreeHelper.GetChild(dg, 0) is Border border)
                {
                    if (border.Child is ScrollViewer scroll) scroll.ScrollToEnd();
                }
            }
        }

        private void cboCustomer_GotFocus(object sender, RoutedEventArgs e)
        {
            this.cboCustomer.IsDropDownOpen = true;
        }

        private void cboCustomer_GotKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
        {
            this.cboCustomer.IsDropDownOpen = true;
        }

        private void btnService_Click(object sender, RoutedEventArgs e)
        {
            ServiceView sv = new ServiceView();
            sv.ShowDialog();
        }

        private void btn_Close_Click(object sender, RoutedEventArgs e)
        {
            if (MessageBox.Show("放弃保存数据并退出吗？", "操作提示", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
            {
                this.Close();
            }
        }        
    }
}
