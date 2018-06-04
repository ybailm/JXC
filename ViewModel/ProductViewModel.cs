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

namespace Project.ViewModel
{
    #region 构造函数初始化
    public partial class ProductViewModel : NotificationObject
    {
        public List<string> UnitList { get; set; }
        public List<string> CategoryList { get; set; }        

        public ProductViewModel():this(null)
        {
            
        }

        public ProductViewModel(ObservableCollection<Product> productCollection)
        {
            if (productCollection?.Count > 0)
            {
                ProductCollection = productCollection;
            }
            else
            {
                ProductCollection = Common.Instance.GetAllProduct();
            }
            DisplayProductCollection = ProductCollection;

            GridVisibility = Visibility.Collapsed;
            UnitList = ProductCollection.Select(p => p.Unit).Distinct().ToList();
            CategoryList = ProductCollection.Select(p => p.Category).Distinct().ToList();

            GridCollapseCommand = new DelegateCommand(GridCollapse);

            UpdateCostPriceCommand = new DelegateCommand(UpdateCostPrice);
            NewProductCommand = new DelegateCommand(NewProduct);
            EditProductItemCommand = new DelegateCommand<Product>(EditProduct);
            SaveProductCommand = new DelegateCommand<Product>(SaveProduct);
            ProductNameSearchCommand = new DelegateCommand<string>(ProductNameSearch);
            PYSearchCommand = new DelegateCommand<string>(PYSearch);
            PDMSearchCommand = new DelegateCommand<string>(PDMSearch);
            DisplayZeroNumberCommand = new DelegateCommand<bool>(DisplayZeroNumber);

            ProductNameChangedCommand = new DelegateCommand<string>(ProductNameChanged);
            CategorySelectionChangedCommand = new DelegateCommand<string>(CategorySelectionChanged);
            SelectCategoryChangedCommand = new DelegateCommand<string>(SelectCategoryChanged);
            UnitSelectionChangedCommand = new DelegateCommand<string>(UnitSelectionChanged);
        }
        
    }
    #endregion

    #region 货品管理业务
    public partial class ProductViewModel : NotificationObject
    {
        #region 命令属性
        public DelegateCommand GridCollapseCommand { get; set; }

        public DelegateCommand UpdateCostPriceCommand { get; set; }
        public DelegateCommand NewProductCommand { get; set; }
        public DelegateCommand<Product> DeleteProductCommand { get; set; }
        public DelegateCommand<Product> EditProductItemCommand { get; set; }

        public DelegateCommand<Product> SaveProductCommand { get; set; }

        public DelegateCommand<string> ProductNameSearchCommand { get; set; }
        public DelegateCommand<string> PYSearchCommand { get; set; }
        public DelegateCommand<string> PDMSearchCommand { get; set; }
        public DelegateCommand<bool> DisplayZeroNumberCommand { get; set; }

        public DelegateCommand<string> ProductNameChangedCommand { get; set; }
        public DelegateCommand<string> CategorySelectionChangedCommand { get; set; }
        public DelegateCommand<string> SelectCategoryChangedCommand { get; set; }
        public DelegateCommand<string> UnitSelectionChangedCommand { get; set; }
        #endregion

        #region 属性
        public Visibility gridVisibility;
        public Visibility GridVisibility
        {
            get
            {
                return gridVisibility;
            }
            set
            {
                gridVisibility = value;
                RaisePropertyChanged(nameof(GridVisibility));
            }
        }

        /// <summary>
        /// 显示的货品列表
        /// </summary>
        private ObservableCollection<Product> displayProductCollection;
        public ObservableCollection<Product> DisplayProductCollection
        {
            get
            {
                if(displayProductCollection == null)
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

        public Product currentProduct;
        public Product CurrentProduct
        {
            get
            {
                return currentProduct;
            }
            set
            {
                currentProduct = value;
                RaisePropertyChanged(nameof(CurrentProduct));
            }
        }
       
        /// <summary>
        /// 所有货品列表
        /// </summary>
        public ObservableCollection<Product> ProductCollection { get; set; }
        #endregion

        #region 方法 
        

        public void GridCollapse()
        {
            GridVisibility = Visibility.Collapsed;
        }

        public void EditProduct(Product product)
        {
            CurrentProduct = product;
            GridVisibility = Visibility.Visible;
        }

        public void UpdateCostPrice()
        {
            foreach(var product in ProductCollection)
            {
                Common.Instance.UpdateProductCostPrice(product);
            }
        }

        public void NewProduct()
        {
            CurrentProduct = new Model.Product() { ProductID = -1 };           
            GridVisibility = Visibility.Visible;
        }

        public void SaveProduct(Product product)
        {
            if (product.PDM.IsNullOrEmptyOrWhiteSpace() || product.ProductName.IsNullOrEmptyOrWhiteSpace())
                return;

            Common.Instance.SaveProduct(product);
            DisplayProductCollection = ProductCollection = Common.Instance.GetAllProduct();
        }

        public void PDMSearch(string pdm)
        {
            DisplayProductCollection = new ObservableCollection<Product>(ProductCollection.Where(p => p.PDM.Contains(pdm)));
        }
        public void ProductNameSearch(string productName)
        {
            DisplayProductCollection = new ObservableCollection<Product>( ProductCollection.Where(p => p.ProductName.Contains(productName)));
        }
        public void PYSearch(string py)
        {
            DisplayProductCollection = new ObservableCollection<Product>(ProductCollection.Where(p => p.PY.Contains(py.ToUpper()))); 
        }

        public void DisplayZeroNumber(bool isDisplayZeroNumber)
        {
            if(isDisplayZeroNumber)
                DisplayProductCollection = new ObservableCollection<Product>(ProductCollection.Where(p => p.Number!=0).OrderByDescending(p => p.Number));
            else
                DisplayProductCollection = new ObservableCollection<Product>(ProductCollection);

        }



        public void UnitSelectionChanged(string unit)
        {
            CurrentProduct.Unit = unit;
            CurrentProduct = CloneExtention.Clone(currentProduct);
        }

        public void CategorySelectionChanged(string category)
        {
            CurrentProduct.Category = category;
            CurrentProduct = CloneExtention.Clone(currentProduct);
        }

        public void SelectCategoryChanged(string category)
        {
            DisplayProductCollection = new ObservableCollection<Product>(ProductCollection.Where(p => p.Category==category));
        }

        public void ProductNameChanged(string productName)
        {
            currentProduct.PY = productName?.ConvertToCaptal();
            CurrentProduct = CloneExtention.Clone(currentProduct);
        }
        #endregion
    }
    #endregion
}
