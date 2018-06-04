using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Project.Model;
using System.Collections.ObjectModel;
using System.Windows;
using Project.Service;
using System.Data.OleDb;
using Project.Extentions;
using Project.View;
using Project.Commands;

namespace Project.ViewModel
{
    #region 构造函数初始化
    public partial class ProductReportViewModel : NotificationObject
    {        
        public List<string> CategoryList { get; set; } 

        public ProductReportViewModel()
        {
            ProductCollection = Common.Instance.GetAllProduct();

            DisplayProductCollection = ProductCollection;
            
            CategoryList = ProductCollection.Select(p => p.Category).Distinct().ToList();

            DisplayZeroNumberCommand = new DelegateCommand<bool>(DisplayZeroNumber);
            ProductNameSearchCommand = new DelegateCommand<string>(ProductNameSearch);
            PYSearchCommand = new DelegateCommand<string>(PYSearch);
            PDMSearchCommand = new DelegateCommand<string>(PDMSearch);
            CaculateProductItemCommand = new DelegateCommand<Product> (CaculateProductItem);
            CategorySelectionChangedCommand = new DelegateCommand<string>(CategorySelectionChanged);

            OpenDetailWindowCommand = new DelegateCommand<ProductItem>(OpenDetailWindow);
        }
                
    }
    #endregion

    #region 货品管理业务
    public partial class ProductReportViewModel : NotificationObject
    {
        #region 命令属性

        public DelegateCommand<string> ProductNameSearchCommand { get; set; }
        public DelegateCommand<string> PYSearchCommand { get; set; }
        public DelegateCommand<string> PDMSearchCommand { get; set; }

        public DelegateCommand<Product> CaculateProductItemCommand { get; set; }
        public DelegateCommand<bool> DisplayZeroNumberCommand { get; set; }

        public DelegateCommand<string> CategorySelectionChangedCommand { get; set; }
        public DelegateCommand<ProductItem> OpenDetailWindowCommand { get; set; }
        #endregion

        #region 属性
        /// <summary>
        /// 显示的货品列表
        /// </summary>
        private ObservableCollection<Product> displayProductCollection;
        public ObservableCollection<Product> DisplayProductCollection
        {
            get
            {
                if (displayProductCollection == null)
                {
                    displayProductCollection = ProductCollection;
                }
                return displayProductCollection;
            }
            set
            {
                displayProductCollection = value;
                RaisePropertyChanged(nameof(DisplayProductCollection));
            }
        }

        private int sumPurchaseCount;
        public int SumPurchaseCount
        {
            get=>sumPurchaseCount;            
            set
            {
                sumPurchaseCount = value;
                RaisePropertyChanged(nameof(SumPurchaseCount));
            }
        }
        private int sumSaleCount;
        public int SumSaleCount
        {
            get => sumSaleCount;
            set
            {
                sumSaleCount = value;
                RaisePropertyChanged(nameof(SumSaleCount));
            }
        }

        private double sumPurchaseCostPrice;
        public double SumPurchaseCostPrice
        {
            get => sumPurchaseCostPrice;
            set
            {
                sumPurchaseCostPrice = value;
                RaisePropertyChanged(nameof(SumPurchaseCostPrice));
            }
        }

        private double purchaseCostPrice;
        public double PurchaseCostPrice
        {
            get => purchaseCostPrice;
            set
            {
                purchaseCostPrice = value;
                RaisePropertyChanged(nameof(PurchaseCostPrice));
            }
        }

        private double sumSaleCostPrice;
        public double SumSaleCostPrice
        {
            get => sumSaleCostPrice;
            set
            {
                sumSaleCostPrice = value;
                RaisePropertyChanged(nameof(SumSaleCostPrice));
            }
        }
        /// <summary>
        /// 所有货品列表
        /// </summary>
        public ObservableCollection<Product> ProductCollection { get; set; }
        public ObservableCollection<ProductItem> ProductItemPurchaseInCollection { get; set; } = new ObservableCollection<ProductItem>();
        public ObservableCollection<ProductItem> ProductItemSaleOutCollection { get; set; } = new ObservableCollection<ProductItem>();
        #endregion

        #region 方法 

        #region 查询方法
        public void PDMSearch(string pdm)
        {
            DisplayProductCollection = new ObservableCollection<Product>(ProductCollection.Where(p => p.PDM.Contains(pdm)).OrderByDescending(P => P.Number));
        }
        public void ProductNameSearch(string productName)
        {
            DisplayProductCollection = new ObservableCollection<Product>( ProductCollection.Where(p => p.ProductName.Contains(productName)).OrderByDescending(P=>P.Number));
        }
        public void PYSearch(string py)
        {
            DisplayProductCollection = new ObservableCollection<Product>(ProductCollection.Where(p => p.PY.Contains(py.ToUpper())).OrderByDescending(p => p.Number)); 
        }
        public void CategorySelectionChanged(string category)
        {
            DisplayProductCollection = new ObservableCollection<Product>(ProductCollection.Where(p => p.Category == category).OrderByDescending(p => p.Number));
        }
        public void DisplayZeroNumber(bool isDisplayZeroNumber)
        {
            if (isDisplayZeroNumber)
                DisplayProductCollection = new ObservableCollection<Product>(ProductCollection.Where(p => p.Number != 0).OrderByDescending(p => p.Number));
            else
                DisplayProductCollection = new ObservableCollection<Product>(ProductCollection);

        }
        #endregion

        public void CaculateProductItem(Product product)
        {
            SumProductItemPurchaseIn(product);
            SumProductItemSaleOut(product);
        }

        public void SumProductItemPurchaseIn(Product product)
        {
            int index = 0;
            SumPurchaseCount = 0;
            SumPurchaseCostPrice = 0;            
            ProductItemPurchaseInCollection?.Clear();

            ObservableCollection<PurchaseDetailItem> purchaseDetailItem = Common.Instance.GetAllPurchaseDetailItemByProduct(product);
            foreach (var item in purchaseDetailItem)
            {
                index++;
                ProductItemPurchaseInCollection.Add(new ProductItem()
                {
                    Index = index,
                    type = DetailType.PurchaseIn,
                    ID = item.PurchaseID,
                    Number = item.Number,
                    UnitPrice = item.UnitPrice,
                    SumPrice = item.SumSalePrice,
                    CostPrice = item.CostPrice,
                    SumCostPrice = item.SumCostPrice,
                });
                SumPurchaseCount += item.Number;
                SumPurchaseCostPrice += (item.Number * item.CostPrice);
                PurchaseCostPrice = SumPurchaseCostPrice / SumPurchaseCount;
            }
            
        }
        public void SumProductItemSaleOut(Product product)
        {
            int index = 0;
            SumSaleCount = 0;
            SumSaleCostPrice = 0;            
            ProductItemSaleOutCollection?.Clear();

            ObservableCollection<SaleDetailItem> saleDetailItem = Common.Instance.GetAllSaleDetailItemByProduct(product);
            foreach (var item in saleDetailItem)
            {
                index++;
                ProductItemSaleOutCollection.Add(new ProductItem()
                {
                    Index = index,
                    type = DetailType.SaleOut,
                    ID = item.SaleID,
                    Number = item.Number,
                    UnitPrice = item.UnitPrice,
                    SumPrice = item.SumSalePrice,
                    CostPrice = item.CostPrice,
                    SumCostPrice = item.SumCostPrice,
                });
                SumSaleCount += item.Number;
                SumSaleCostPrice += (item.Number * item.CostPrice);
            }
        }

  
        
        public void OpenDetailWindow(ProductItem productItem)
        {
            switch(productItem?.type)
            {
                case DetailType.PurchaseIn:
                    NewPurchaseView npv = new NewPurchaseView(Common.Instance.GetPurchaseByID(productItem.ID));
                    npv.Show();
                    break;
                case DetailType.SaleOut:
                    NewSaleView nsv = new NewSaleView(Common.Instance.GetSaleByID(productItem.ID));
                    nsv.Show();
                    break;
            }
        }
        #endregion
    }

    public class ProductItem
    {
        
        public ProductItem()
        {
        }

        public int Index { get; set; }
        public string ID { get; set; }
        public DetailType type { get; set; }
        public int Number { get; set; }
        public double UnitPrice { get; set; }
        public double CostPrice { get; set; }

        public double SumPrice { get; set; }
        public double SumCostPrice { get; set; }


    }
    public enum DetailType
    {
        SaleOut,
        PurchaseIn,
    }
    #endregion
}
