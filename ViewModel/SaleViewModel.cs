using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Project.Model;
using System.Collections.ObjectModel;
using Project.Commands;
using System.Windows;
using System.Threading;
using Project.View;

namespace Project.ViewModel
{
    #region 构造函数初始化
    public partial class SaleViewModel : NotificationObject
    {
        public DelegateCommand<Sale> SaleItemSelectionChangedCommand { get; set; }
        public DelegateCommand<Sale> OpenNewSaleViewCommand { get; set; }
        public DelegateCommand DateSearchCommand { get; set; }

        private DateTime dateFrom = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);

        public DateTime DateFrom
        {
            get => dateFrom;
            set
            {
                dateFrom = value;
                RaisePropertyChanged(nameof(DateFrom));
            }
        }

        private DateTime dateTo = DateTime.Now;

        public DateTime DateTo
        {
            get => dateTo;
            set
            {
                dateTo = value;
                RaisePropertyChanged(nameof(DateTo));
            }
        }

        private ObservableCollection<Sale> SaleCollection;

        /// <summary>
        /// 显示的销售单列表
        /// </summary>
        private ObservableCollection<Sale> displaySaleCollection = new ObservableCollection<Sale>();
        public ObservableCollection<Sale> DisplaySaleCollection
        {
            get
            {
                if (displaySaleCollection == null)
                {
                    displaySaleCollection = SaleCollection.Select(p=>p.SaleDate==DateTime.Now) as ObservableCollection<Sale>;
                }
                return displaySaleCollection;
            }
            set
            {
                displaySaleCollection = value;
                RaisePropertyChanged(nameof(DisplaySaleCollection));
            }
        }
        

        private double sumCostPrice;

        public double SumCostPrice
        {
            get { return sumCostPrice; }
            set
            {
                sumCostPrice = value;
                RaisePropertyChanged(nameof(SumCostPrice));
            }
        }

        private double productProfit;

        public double ProductProfit
        {
            get { return productProfit; }
            set
            {
                productProfit = value;
                RaisePropertyChanged(nameof(ProductProfit));
            }
        }

        private double sumProfit;

        public double SumProfit
        {
            get { return sumProfit; }
            set
            {
                sumProfit = value;
                RaisePropertyChanged(nameof(SumProfit));
            }
        }
        public void OpenNewSaleView(Sale sale)
        {
            NewSaleView nsv = new NewSaleView(Sale, SaleItemCollection, ServiceItemCollection);
            nsv.Show();
        }

        public SaleViewModel()
        {

            Sale = new Sale();
            SaleCollection = new ObservableCollection<Sale>(Common.Instance.GetAllSaleList().OrderBy(p => p.SaleID));

            DateTime begin = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
            DisplaySaleCollection = new ObservableCollection<Sale>(SaleCollection.Where(p => p.SaleDate >= begin));

            CustomerList = Common.Instance.GetAllCustomer();           

            SaleItemSelectionChangedCommand = new DelegateCommand<Sale>(SaleItemSelectionChanged);


            SaleIDSearchCommand = new DelegateCommand<string>(SaleIDSearch);
            PYSearchCommand = new DelegateCommand<string>(PYSearch);
            DateSearchCommand = new DelegateCommand(DateSearch);

            CustomerSelectionChangedCommand = new DelegateCommand<Customer>(CustomerSelectionChanged);
            OpenNewSaleViewCommand = new DelegateCommand<Sale>(OpenNewSaleView);
        }

        private void DateSearch()
        {
            DisplaySaleCollection.Clear();
            foreach (var item in SaleCollection.Where(p => p.SaleDate > DateFrom && p.SaleDate < DateTo))
            {
                DisplaySaleCollection.Add(item);
            }
        }

        public void SaleItemSelectionChanged(Sale sale)
        {
            if(sale!=null)
            {
                Sale = sale;
                SaleItemCollection = Common.Instance.GetSaleDetailItemBySaleID(sale.SaleID);
                SumSalePrice = SaleItemCollection.Sum(p => p.SumSalePrice);
                SumCostPrice = SaleItemCollection.Sum(p => p.SumCostPrice);

                ServiceItemCollection = Common.Instance.GetServiceDetailItemBySaleID(sale.SaleID);
                SumServicePrice = serviceItemCollection.Sum(p => p.SumServicePrice);

                ProductProfit = SumSalePrice - SumCostPrice;
                SumProfit = ProductProfit + SumServicePrice;
            }
        }
    }
    #endregion

    #region 维修业务
    public partial class SaleViewModel : NotificationObject
    {
        #region 命令属性

        public DelegateCommand<Customer> CustomerSelectionChangedCommand { get; set; }

        #endregion

        #region 属性        

        private Sale sale;
        public Sale Sale
        {
            get
            {
                return sale;
            }
            set
            {
                sale = value;
                RaisePropertyChanged(nameof(Sale));
            }
        }

        

        private double sumServicePrice;
        /// <summary>
        /// 合计工时费金额
        /// </summary>
        public double SumServicePrice
        {
            get
            {
                return sumServicePrice;
            }
            set
            {
                sumServicePrice = value;
                SumPrice = sumServicePrice + SumSalePrice;
                RaisePropertyChanged(nameof(SumServicePrice));
                RaisePropertyChanged(nameof(SumPrice));
            }
        }

        private ObservableCollection<ServiceDetailItem> serviceItemCollection;
        /// <summary>
        /// 维修项目列表
        /// </summary>
        public ObservableCollection<ServiceDetailItem> ServiceItemCollection
        {
            get
            {
                return serviceItemCollection;
            }
            set
            {
                serviceItemCollection = value;
                int i = 0;
                foreach (var item in serviceItemCollection)
                {
                    item.ID = ++i;
                }
                RaisePropertyChanged(nameof(ServiceItemCollection));
            }
        }
        #endregion

        #region 方法 
        public void CustomerSelectionChanged(Customer customer)
        {
            DisplaySaleCollection = new ObservableCollection<Sale>(SaleCollection.Where(p => p.Customer.CustomerID == customer.CustomerID));
        }
    

        #endregion
    }
    #endregion

    #region 配件销售业务
    public partial class SaleViewModel
    {
        #region 命令属性
       
        
        public DelegateCommand<string> SaleIDSearchCommand { get; set; }
        public DelegateCommand<string> PYSearchCommand { get; set; }
        #endregion

        #region 属性  

        public ObservableCollection<SaleDetailItem> SaleDetailList { get; set; }

        /// <summary>
        /// 合计配件金额
        /// </summary>
        public double SumPrice { get; set; }

        private double sumSalePrice;
        /// <summary>
        /// 合计配件金额
        /// </summary>
        public double SumSalePrice
        {
            get
            {
                return sumSalePrice;
            }
            set
            {
                sumSalePrice = value;
                SumPrice = sumSalePrice + SumServicePrice;
                RaisePropertyChanged(nameof(SumSalePrice));
                RaisePropertyChanged(nameof(SumPrice));
            }
        }

        private ObservableCollection<SaleDetailItem> saleItemCollection;
        /// <summary>
        /// 销售配件列表
        /// </summary>
        public ObservableCollection<SaleDetailItem> SaleItemCollection
        {
            get
            {
                return saleItemCollection;
            }
            set
            {
                saleItemCollection = value;
                int i = 0;
                foreach (var item in saleItemCollection)
                {
                    item.ID = ++i;
                }
                RaisePropertyChanged(nameof(SaleItemCollection));
            }
        }

        /// <summary>
        /// 客户列表
        /// </summary>
        private ObservableCollection<Customer> customerList;

        public ObservableCollection<Customer> CustomerList
        {
            get { return customerList; }
            set
            {
                customerList = value;
                RaisePropertyChanged(nameof(CustomerList));
            }
        }
        #endregion

        #region 方法 
        public void SaleIDSearch(string id)
        {
            DisplaySaleCollection = new ObservableCollection<Sale>(SaleCollection.Where(p => p.SaleID.Contains(id)));
        }
        public void PYSearch(string py)
        {
            DisplaySaleCollection = new ObservableCollection<Sale>(SaleCollection.Where(p => p.Customer.PY.Contains(py.ToUpper())).OrderByDescending(p => p.SaleID)); 
        }
        #endregion        
    }
    #endregion
}
