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
using System.Collections.ObjectModel;
using Project.Print;
using Project.Model;

namespace Project.View
{
    /// <summary>
    /// NewPurchaseView.xaml 的交互逻辑
    /// </summary>
    public partial class NewPurchaseView : Window
    {  
        public NewPurchaseView():this(null,null,null)
        {
            
        }        

        public NewPurchaseView(Purchase purchase) : this(purchase, null,null)
        {

        }
        public NewPurchaseView(Purchase purchase,ObservableCollection<Product> productCollection) : this(purchase, null, productCollection)
        {

        }

        public NewPurchaseView(Purchase purchase, ObservableCollection<PurchaseDetailItem> purchaseItemCollection):this(purchase,purchaseItemCollection,null)
        {
          
        }

        public NewPurchaseView(Purchase purchase, ObservableCollection<PurchaseDetailItem> purchaseItemCollection,ObservableCollection<Product> productCollection)
        {
            InitializeComponent();
            DataContext = new NewPurchaseViewModel(purchase, purchaseItemCollection,productCollection);
        }       

        private void btn_Cancel_Click(object sender, RoutedEventArgs e)
        {
            if (MessageBox.Show("放弃保存数据并退出吗？", "操作提示", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
            {
                this.Close();
            }
        }        
    }
}
