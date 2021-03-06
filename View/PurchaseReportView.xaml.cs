﻿using Project.ViewModel;
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
    /// PurchaseView.xaml 的交互逻辑
    /// </summary>
    public partial class PurchaseReportView : Window
    {
        public PurchaseReportView()
        {
            InitializeComponent();
            this.DataContext = new PurchaseReportViewModel();
        }

        private void dgPurchase_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            NewPurchaseView nsv = new NewPurchaseView(this.dgPurchase.SelectedItem as Purchase);
            nsv.Show();
        }        

        private void btn_Close_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void btn_Refresh_Click(object sender, RoutedEventArgs e)
        {
            this.DataContext = new PurchaseViewModel();
        }
    }
}
