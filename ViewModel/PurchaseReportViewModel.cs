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
using Project.Extentions;
using System.Collections.Specialized;
using System.Threading;

namespace Project.ViewModel
{
    #region 构造函数初始化
    public partial class PurchaseReportViewModel : NotificationObject
    {       
        public DelegateCommand<Supplier> SupplierSelectionChangedCommand { get; set; }
        public DelegateCommand DateSearchCommond { get; set; }
        private DateTime dateFrom = new DateTime(DateTime.Now.Year,DateTime.Now.Month,1);

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

        private ObservableCollection<Purchase> PurchaseCollection;

        /// <summary>
        /// 显示的销售单列表
        /// </summary>
        private ObservableCollection<Purchase> displayPurchaseCollection = new ObservableCollection<Purchase>();
        public ObservableCollection<Purchase> DisplayPurchaseCollection
        {
            get
            {
                if (displayPurchaseCollection == null)
                {
                    DateTime begin = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
                    displayPurchaseCollection = PurchaseCollection.Select(p=>p.PurchaseDate>=begin) as ObservableCollection<Purchase>;
                }
                return displayPurchaseCollection;
            }
            set
            {
                displayPurchaseCollection = value;
                RaisePropertyChanged(nameof(DisplayPurchaseCollection));
            }
        }

        public PurchaseReportViewModel()
        {

            PurchaseCollection = Common.Instance.GetAllPurchaseList();            
            DisplayPurchaseCollection.CollectionChanged += DisplayPurchaseCollectionChanged;
            
            var list = PurchaseCollection.Where(p => p.PurchaseDate >= dateFrom);
            foreach (var item in list)
            {
                DisplayPurchaseCollection.Add(item);
            }

            SupplierList = Common.Instance.GetAllSupplier();           

            PurchaseIDSearchCommand = new DelegateCommand<string>(PurchaseIDSearch);
            PYSearchCommand = new DelegateCommand<string>(PYSearch);
            DateSearchCommond = new DelegateCommand(DateSearch);

            SupplierSelectionChangedCommand = new DelegateCommand<Supplier>(SupplierSelectionChanged);
        }

        private void DateSearch()
        {
            DisplayPurchaseCollection.Clear();
            foreach (var item in PurchaseCollection.Where(p => p.PurchaseDate>DateFrom&&p.PurchaseDate<DateTo))
            {
                DisplayPurchaseCollection.Add(item);
            }
        }

        private void SupplierSelectionChanged(Supplier supplier)
        {
            DisplayPurchaseCollection.Clear();
            foreach (var item in PurchaseCollection.Where(p =>p.Supplier.SupplierID ==supplier.SupplierID))
            {
                DisplayPurchaseCollection.Add(item);
            }
        }

        private void DisplayPurchaseCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            SumPrice = 0;
            foreach(var item in DisplayPurchaseCollection)
            {
                SumPrice += item.SumPrice;
            }
        }        
    }
    #endregion

    #region 配件入库业务
    public partial class PurchaseReportViewModel
    {
        #region 命令属性
        public DelegateCommand<string> PurchaseIDSearchCommand { get; set; }
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

        /// <summary>
        /// 供应商列表
        /// </summary>
        private ObservableCollection<Supplier> supplierList;

        public ObservableCollection<Supplier> SupplierList
        {
            get { return supplierList; }
            set
            {
                supplierList = value;
                RaisePropertyChanged(nameof(SupplierList));
            }
        }
        #endregion

        #region 方法 
        public void PurchaseIDSearch(string id)
        {
            DisplayPurchaseCollection.Clear();
            foreach (var item in PurchaseCollection.Where(p => p.PurchaseID.Contains(id)))
            {
                DisplayPurchaseCollection.Add(item);
            }
        }
        public void PYSearch(string py)
        {
            DisplayPurchaseCollection.Clear();
            foreach (var item in PurchaseCollection.Where(p => p.Supplier.PY.Contains(py)))
            {
                DisplayPurchaseCollection.Add(item);
            }
        }
        #endregion        
    }
    #endregion


   

}
