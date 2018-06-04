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
using Project.Extentions;
using System.Collections.Specialized;
using System.Threading;

namespace Project.ViewModel
{
    #region 构造函数初始化
    public partial class SaleReportViewModel : NotificationObject
    {   
        public DelegateCommand<Sale> SaleItemSelectionChangedCommand { get; set; }
        public DelegateCommand DateSearchCommond { get; set; }

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
                    DateTime begin = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
                    displaySaleCollection = SaleCollection.Select(p=>p.SaleDate>=begin) as ObservableCollection<Sale>;
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
        /// <summary>
        /// 汇总成本价
        /// </summary>
        public double SumCostPrice
        {
            get { return sumCostPrice; }
            set
            {
                sumCostPrice = value;
                RaisePropertyChanged(nameof(SumCostPrice));
            }
        }

        private double sumProductProfit;
        /// <summary>
        /// 汇总配件利润
        /// </summary>
        public double SumProductProfit
        {
            get { return sumProductProfit; }
            set
            {
                sumProductProfit = value;
                RaisePropertyChanged(nameof(SumProductProfit));
            }
        }

        private double sumProfit;
        /// <summary>
        /// 汇总总利润
        /// </summary>
        public double SumProfit
        {
            get { return sumProfit; }
            set
            {
                sumProfit = value;
                RaisePropertyChanged(nameof(SumProfit));
            }
        }

        public SaleReportViewModel()
        {           
            SaleCollection = Common.Instance.GetAllSaleList();

            DisplaySaleCollection.CollectionChanged += DisplaySaleCollectionChanged;

            var list = SaleCollection.Where(p => p.SaleDate >= dateFrom);
            foreach (var item in list)
            {
                DisplaySaleCollection.Add(item);
            }

            CustomerList = Common.Instance.GetAllCustomer();           

            //SaleItemSelectionChangedCommand = new DelegateCommand<Sale>(SaleItemSelectionChanged);
            SaleIDSearchCommand = new DelegateCommand<string>(SaleIDSearch);
            PYSearchCommand = new DelegateCommand<string>(PYSearch);
            DateSearchCommond = new DelegateCommand(DateSearch);

            CustomerSelectionChangedCommand = new DelegateCommand<Customer>(CustomerSelectionChanged);
        }

        private void DateSearch()
        {
            DisplaySaleCollection.Clear();
            foreach (var item in SaleCollection.Where(p => p.SaleDate > DateFrom && p.SaleDate < DateTo))
            {
                DisplaySaleCollection.Add(item);
            }
        }

        private void DisplaySaleCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            foreach(var item in DisplaySaleCollection)
            {
                SumPrice = DisplaySaleCollection.Sum(p => p.SumPrice);
                SumServicePrice = DisplaySaleCollection.Sum(p => p.ServicePrice);        
                SumProductProfit = DisplaySaleCollection.Sum(p => p.ProductProfit);
                SumProfit = DisplaySaleCollection.Sum(p => p.SumProfit);
                SumSalePrice = SumPrice - SumServicePrice;
                SumCostPrice = SumSalePrice - SumProductProfit;
            }
        }
        
    }
    #endregion

    #region 维修业务
    public partial class SaleReportViewModel : NotificationObject
    {
        #region 命令属性

        public DelegateCommand<Customer> CustomerSelectionChangedCommand { get; set; }

        #endregion

        #region 属性       

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
                RaisePropertyChanged(nameof(SumServicePrice));
            }
        }        
        #endregion

        #region 方法 
        public void CustomerSelectionChanged(Customer customer)
        {
            DisplaySaleCollection.Clear();
            foreach(var item in SaleCollection.Where(p => p.Customer.CustomerID == customer.CustomerID))
            {
                DisplaySaleCollection.Add(item);
            }
        }
    

        #endregion
    }
    #endregion

    #region 配件销售业务
    public partial class SaleReportViewModel
    {
        #region 命令属性      
        public DelegateCommand<string> SaleIDSearchCommand { get; set; }
        public DelegateCommand<string> PYSearchCommand { get; set; }
        #endregion

        #region 属性  
        public double sumPrice;
        /// <summary>
        /// 合计配件金额
        /// </summary>
        public double SumPrice
        {
            get
            {
                return sumPrice;
            }
            set
            {
                sumPrice = value;
                RaisePropertyChanged(nameof(sumPrice));
            }
        }

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
                RaisePropertyChanged(nameof(SumSalePrice));
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
            DisplaySaleCollection.Clear();
            foreach (var item in SaleCollection.Where(p => p.SaleID.Contains(id)))
            {
                DisplaySaleCollection.Add(item);
            }
        }
        public void PYSearch(string py)
        {
            DisplaySaleCollection.Clear();
            foreach (var item in SaleCollection.Where(p => p.Customer.PY.Contains(py)))
            {
                DisplaySaleCollection.Add(item);
            }
        }
        #endregion        
    }
    #endregion


   

}
