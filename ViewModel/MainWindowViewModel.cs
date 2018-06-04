using Project.Commands;
using Project.View;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using System.Globalization;
using Project.Extentions;

namespace Project.ViewModel
{
    class MainWindowViewModel : NotificationObject
    {
        public DelegateCommand<string> OpenWindowCommand { get; set; }
        public MainWindowViewModel()
        {
            OpenWindowCommand = new DelegateCommand<string>(OpenWindow);
        }

        private void OpenWindow(string windowName)
        {
            try
            {
                WindowName name = windowName.ConvertToWindowName();
                Window window = null;
                switch (name)
                {
                    case WindowName.NewSaleView:
                        window = new NewSaleView();
                        break;
                    case WindowName.CustomerView:
                        window = new CustomerView();
                        break;
                    case WindowName.NewPurchaseView:
                        window = new NewPurchaseView();
                        break;
                    case WindowName.SupplierView:
                        window = new SupplierView();
                        break;
                    case WindowName.SaleView:
                        window = new SaleView();
                        break;
                    case WindowName.PurchaseView:
                        window = new PurchaseView();
                        break;
                    case WindowName.ProductView:
                        window = new ProductView();
                        break;
                    case WindowName.ServiceView:
                        window = new ServiceView();
                        break;
                    case WindowName.ChargeView:
                        window = new ChargeView();
                        break;
                    case WindowName.SaleReportView:
                        window = new SaleReportView();
                        break;
                    case WindowName.PurchaseReportView:
                        window = new PurchaseReportView();
                        break;
                    case WindowName.ProductReportView:
                        window = new ProductReportView();
                        break;
                }
                
                window?.ShowDialog();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }
    }
    public enum WindowName
    {
        NewSaleView,
        CustomerView,
        NewPurchaseView,
        SupplierView,
        SaleView,
        PurchaseView,
        ProductView,
        ServiceView,
        ChargeView,
        SaleReportView,
        PurchaseReportView,
        ProductReportView,
    }
}