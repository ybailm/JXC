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
using System.Collections.Specialized;
using System.Threading;
using Project.View;
using Project.Print;

namespace Project.ViewModel
{
    #region 构造函数初始化
    public partial class PurchaseViewModel : NotificationObject
    {        
        public DelegateCommand DateSearchCommand { get; set; }

        private void DateSearch()
        {
            DisplayPurchaseCollection.Clear();
            foreach (var item in PurchaseCollection.Where(p => p.PurchaseDate > DateFrom && p.PurchaseDate < DateTo))
            {
                DisplayPurchaseCollection.Add(item);
            }
        }

        private DateTime dateFrom = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
        public DateTime DateFrom
        {
            get=>dateFrom;            
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
        private ObservableCollection<Purchase> displayPurchaseCollection;
        public ObservableCollection<Purchase> DisplayPurchaseCollection
        {
            get
            {
                if (displayPurchaseCollection == null)
                {
                    displayPurchaseCollection = PurchaseCollection.Select(p => p.PurchaseDate == DateTime.Now) as ObservableCollection<Purchase>;
                }
                return displayPurchaseCollection;
            }
            set
            {
                displayPurchaseCollection = value;
                RaisePropertyChanged(nameof(DisplayPurchaseCollection));
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

        public Purchase Purchase { get; set; }

        public PurchaseViewModel()
        {

            PurchaseCollection = new ObservableCollection<Purchase>(Common.Instance.GetAllPurchaseList().OrderBy(p => p.PurchaseID));
                
            DisplayPurchaseCollection = new ObservableCollection<Purchase>(PurchaseCollection.Where(p => p.PurchaseDate >= dateFrom));

            SupplierList = Common.Instance.GetAllSupplier();

            PurchaseItemSelectionChangedCommand = new DelegateCommand<Purchase>(PurchaseItemSelectionChanged);
            PurchaseIDSearchCommand = new DelegateCommand<string>(PurchaseIDSearch);
            PYSearchCommand = new DelegateCommand<string>(PYSearch);
            DateSearchCommand = new DelegateCommand(DateSearch);

            SupplierSelectionChangedCommand = new DelegateCommand<Supplier>(SupplierSelectionChanged);
            OpenNewPurchaseViewCommand = new DelegateCommand<Purchase>(OpenNewPurchaseView);
            OpenPrintWindowCommand = new DelegateCommand(OpenPrintWindow);
        }

        public void PurchaseItemSelectionChanged(Purchase purchase)
        {
            if (purchase != null)
            {
                Purchase = purchase;
                PurchaseItemCollection = Common.Instance.GetPurchaseDetailItemByPurchaseID(purchase.PurchaseID);
                SumPurchasePrice = PurchaseItemCollection.Sum(p => p.SumCostPrice);
                SumCostPrice = PurchaseItemCollection.Sum(p => p.CostPrice);

            }
        }
    }
    #endregion 
    #region 入库业务
    public partial class PurchaseViewModel
    {
        #region 命令属性 
        public DelegateCommand<string> PurchaseIDSearchCommand { get; set; }
        public DelegateCommand<string> PYSearchCommand { get; set; }
        public DelegateCommand<Purchase> PurchaseItemSelectionChangedCommand { get; set; }
        public DelegateCommand<Supplier> SupplierSelectionChangedCommand { get; set; }
        public DelegateCommand<Purchase> OpenNewPurchaseViewCommand { get; set; }
        public DelegateCommand OpenPrintWindowCommand { get; set; }
        #endregion

        #region 属性  

        public ObservableCollection<PurchaseDetailItem> PurchaseDetailList { get; set; }
        private double sumPurchasePrice;
        /// <summary>
        /// 合计配件金额
        /// </summary>
        public double SumPurchasePrice
        {
            get => sumPurchasePrice;
            set
            {
                sumPurchasePrice = value;
                RaisePropertyChanged(nameof(SumPurchasePrice));
            }
        }

        private ObservableCollection<PurchaseDetailItem> purchaseItemCollection;
        /// <summary>
        /// 销售配件列表
        /// </summary>
        public ObservableCollection<PurchaseDetailItem> PurchaseItemCollection
        {
            get => purchaseItemCollection;
            set
            {
                purchaseItemCollection = value;
                int i = 0;
                foreach (var item in purchaseItemCollection)
                {
                    item.ID = ++i;
                }
                RaisePropertyChanged(nameof(PurchaseItemCollection));
            }
        }

        /// <summary>
        /// 供应商列表
        /// </summary>
        private ObservableCollection<Supplier> supplierList;

        public ObservableCollection<Supplier> SupplierList
        {
            get => supplierList;
            set
            {
                supplierList = value;
                RaisePropertyChanged(nameof(SupplierList));
            }
        }
        #endregion

        #region 方法 
        public void SupplierSelectionChanged(Supplier supplier)
        {
            DisplayPurchaseCollection = new ObservableCollection<Purchase>(PurchaseCollection.Where(p => p.Supplier.SupplierName == supplier.SupplierName));
        }
        public void PurchaseIDSearch(string id)
        {
            DisplayPurchaseCollection = new ObservableCollection<Purchase>(PurchaseCollection.Where(p => p.PurchaseID.Contains(id)));
        }
        public void PYSearch(string py)
        {
            DisplayPurchaseCollection = new ObservableCollection<Purchase>(PurchaseCollection.Where(p => p.Supplier.PY.Contains(py.ToUpper())));
        }

        public void OpenNewPurchaseView(Purchase purchase)
        {
            NewPurchaseView npv = new NewPurchaseView(purchase, PurchaseItemCollection);
            npv.Show();
        }

        public void OpenPrintWindow()
        {
            var data = new PurchaseData { Purchase = Purchase, PurchaseItemCollection = PurchaseItemCollection };
            PrintWindow pw = new PrintWindow("print\\PurchaseFlowDocument.xaml", data, new PurchaseDocumentRenderer());
            pw.ShowDialog();
        }
        #endregion        
    }
    #endregion
}