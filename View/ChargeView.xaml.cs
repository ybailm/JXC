using Project.Extentions;
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
    /// NewPurchaseView.xaml 的交互逻辑
    /// </summary>
    public partial class ChargeView : Window
    {
        public ChargeView()
        {
            InitializeComponent();
            DataContext = new ChargeViewModel();
        }

        private void btn_Close_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void btn_Hide_Click(object sender, RoutedEventArgs e)
        {
            this.grid_Charge.Visibility = Visibility.Collapsed;
        }        
    }
}
