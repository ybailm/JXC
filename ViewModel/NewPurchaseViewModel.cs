using System.Collections.Generic;
using System.Data;
using System.Linq;
using Project.Model;
using System.Collections.ObjectModel;
using Project.Extentions;
using Project.Print;
using Project.View;
using Project.Commands;

namespace Project.ViewModel
{
    #region 构造函数初始化
    public partial class NewPurchaseViewModel : NotificationObject
    {
        private string tipText;

        public string TipText
        {
            get =>tipText;
            set
            {
                tipText = value;
                RaisePropertyChanged(nameof(TipText));
            }
        }

        private bool canSave = false;

        public bool isEditable = true;
        public bool IsEditable
        {
            get => isEditable;
            set
            {
                isEditable = value;
                RaisePropertyChanged(nameof(IsEditable));
            }
        }

        public NewPurchaseViewModel():this(null,null,null)
        {
            
        }
        
        public NewPurchaseViewModel(Purchase purchase) : this(purchase, null,null)
        {

        }

        public NewPurchaseViewModel(Purchase purchase,ObservableCollection<PurchaseDetailItem> purchaseItemCollection) : this(purchase, purchaseItemCollection, null)
        {

        }

        public NewPurchaseViewModel(Purchase purchase,ObservableCollection<PurchaseDetailItem> purchaseItemCollection, ObservableCollection<Product> productCollection)
        {    
            if(purchase == null)
            {
                Purchase = new Purchase
                {
                    PurchaseID = Common.Instance.GetNewPurchaseID()
                };
                PurchaseItemCollection = new ObservableCollection<PurchaseDetailItem>();
            }
            else
            {
                Purchase = purchase;
                PurchaseItemCollection = purchaseItemCollection?? Common.Instance.GetPurchaseDetailItemByPurchaseID(purchase.PurchaseID);
                if (Purchase.Approved != true)
                {
                    oldPurchase = Extentions.CloneExtention.Clone(Purchase);
                    oldPurchaseItemCollection = Extentions.CloneExtention.Clone(PurchaseItemCollection);
                    ProductCollection = productCollection;
                }
            }           
                     
            
            IsEditable = !Purchase.Approved;         

            if (IsEditable == true)
            {
                if (ProductCollection==null)
                {
                    ProductCollection = Common.Instance.GetAllProduct();
                }
                SupplierList = Common.Instance.GetAllSupplier();
                AttendantList = Common.Instance.GetAllAttendant();
                
                PurchaseItemCollection.CollectionChanged += (seder, args) => PurchaseItemCollectionChanged();

                AttendantSelectionChangedCommand = new DelegateCommand<Attendant>(AttendantSelectionChanged);
                SupplierSelectionChangedCommand = new DelegateCommand<Model.Supplier>(SupplierSelectionChanged);
                CategorySelectionChangedCommand = new DelegateCommand<string>(CategorySelectionChanged);

                AddPurchaseItemCommand = new DelegateCommand<Product>(AddPurchaseItem);
                DeletePurchaseItemCommand = new DelegateCommand<PurchaseDetailItem>(DeletePurchaseItem);
                EditPurchaseItemCommand = new DelegateCommand<PurchaseDetailItem>(EditPurchaseItem);

                SavePurchaseItemCommand = new DelegateCommand(SavePurchaseItem, () => canSave);
                SavePurchaseItemAndNewCommand = new DelegateCommand(SavePurchaseItemAndNew, () => canSave);
                ApproveCommand = new DelegateCommand(Approve);

                PDMSearchCommand = new DelegateCommand<string>(PDMSearch);
                PYSearchCommand = new DelegateCommand<string>(PYSearch);
                ProductNameSearchCommand = new DelegateCommand<string>(ProductNameSearch);
                RefreshCommand = new DelegateCommand(Refresh);
                OpenPrintWindowCommand = new DelegateCommand(OpenPrintWindow);
                OpenSupplierViewCommand = new DelegateCommand(OpenSupplierView);
                OpenProductViewCommand = new DelegateCommand(OpenProductView);
            }
        }
        
    }
    #endregion

    #region 属性与普通方法
    public partial class NewPurchaseViewModel
    {
        #region 命令属性
        public DelegateCommand<string> CategorySelectionChangedCommand { get; set; }
        public DelegateCommand<Supplier> SupplierSelectionChangedCommand { get; set; }
        public DelegateCommand<Attendant> AttendantSelectionChangedCommand { get; set; }

        public DelegateCommand<Model.Product> AddPurchaseItemCommand { get; set; }
        public DelegateCommand<PurchaseDetailItem> DeletePurchaseItemCommand { get; set; }
        public DelegateCommand<PurchaseDetailItem> EditPurchaseItemCommand { get; set; }
        public DelegateCommand SavePurchaseItemCommand { get; set; }
        public DelegateCommand SavePurchaseItemAndNewCommand { get; set; }
        public DelegateCommand ApproveCommand { get; set; }
        public DelegateCommand<string> PDMSearchCommand { get; set; }
        public DelegateCommand<string> PYSearchCommand { get; set; }
        public DelegateCommand<string> ProductNameSearchCommand { get; set; }

        public DelegateCommand RefreshCommand { get; set; }
        public DelegateCommand OpenPrintWindowCommand { get; set; }
        public DelegateCommand OpenSupplierViewCommand { get; set; }
        public DelegateCommand OpenProductViewCommand { get; set; }
        #endregion

        #region 属性
        private Purchase purchase;
        public Purchase Purchase
        {
            get => purchase;
            set
            {
                purchase = value;
                RaisePropertyChanged(nameof(Purchase));
            }
        }
        private Purchase oldPurchase;
        private ObservableCollection<PurchaseDetailItem> oldPurchaseItemCollection;

        /// <summary>
        /// 合计金额大写
        /// </summary>
        public string SumCaptalPurchasePrice { get; set; }

        private double sumPurchasePrice;
        /// <summary>
        /// 合计金额
        /// </summary>
        public double SumPurchasePrice
        {
            get
            {
                return sumPurchasePrice;
            }
            set
            {
                sumPurchasePrice = value;
                SumCaptalPurchasePrice = sumPurchasePrice.ToCaptal();
                RaisePropertyChanged(nameof(SumPurchasePrice));
                RaisePropertyChanged(nameof(SumCaptalPurchasePrice));               
            }
        }

        public ObservableCollection<Attendant> attendantList;
        public ObservableCollection<Attendant> AttendantList
        {
            get { return attendantList; }
            set
            {
                attendantList = value;
                RaisePropertyChanged(nameof(AttendantList));
            }
        }

        public List<string> categoryList;
        public List<string> CategoryList
        {
            get
            {
                if (categoryList==null||categoryList.Count < 1)
                    categoryList = ProductCollection?.Select(p => p.Category).Distinct().ToList();
                return categoryList;
            }
            set
            {
                categoryList = value;
                RaisePropertyChanged(nameof(CategoryList));
            }
        }

        private ObservableCollection<Product> ProductCollection;

        /// <summary>
        /// 显示的库存配件列表
        /// </summary>
        private ObservableCollection<Product> displayProductCollection;
        public ObservableCollection<Product> DisplayProductCollection
        {
            get
            {
                if(displayProductCollection==null)
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
        

        private ObservableCollection<PurchaseDetailItem> purchaseItemCollection;
        /// <summary>
        /// 入库配件列表
        /// </summary>
        public ObservableCollection<PurchaseDetailItem> PurchaseItemCollection
        {
            get
            {
                return purchaseItemCollection;
            }
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
            get { return supplierList; }
            set
            {
                supplierList = value;
                RaisePropertyChanged(nameof(SupplierList));
            }
        }
        #endregion

        #region 方法

        private void ChangeToEnableSave(bool isEnable)
        {
            canSave = isEnable;
            SavePurchaseItemCommand?.RaiseCanExecuteChanged();
            SavePurchaseItemAndNewCommand?.RaiseCanExecuteChanged();
        }

        /// <summary>
        /// 当集合发生更改时，重新计算合计金额，并更新序号列
        /// </summary>
        private void PurchaseItemCollectionChanged()
        {
            SumPurchasePrice = 0;
            int i = 0;
            foreach (var item in PurchaseItemCollection)
            {
                item.ID = ++i;
                SumPurchasePrice += item.SumCostPrice;
            }
            ChangeToEnableSave(true);
        }

        private void AddPurchaseItem(Product product)
        {
            if (product == null)
                return;
            if (PurchaseItemCollection == null)
                PurchaseItemCollection = new ObservableCollection<PurchaseDetailItem>();

            if (PurchaseItemCollection.Where(p => p.Product.ProductID == product.ProductID).Count() > 0)
            {
                return;
            }
            PurchaseItemCollection.Add(new PurchaseDetailItem(product));
        }

        private void DeletePurchaseItem(PurchaseDetailItem obj)
        {
            PurchaseItemCollection.Remove(obj);
        }
        public void EditPurchaseItem(PurchaseDetailItem obj)
        {
            SumPurchasePrice = PurchaseItemCollection.Sum(p => p.SumCostPrice);
        }

        public void SupplierSelectionChanged(Supplier supplier)
        {
            if (supplier != null)
            {
                Purchase.Supplier = CloneExtention.Clone(supplier);
                ChangeToEnableSave(true);
            }
        }

        public void AttendantSelectionChanged(Attendant attendant)
        {
            if (attendant != null)
            {
                Purchase.Attendant = CloneExtention.Clone(attendant);
                ChangeToEnableSave(true);
            }
        }     
        
        public void CategorySelectionChanged(string category)
        {
            DisplayProductCollection = new ObservableCollection<Product>(ProductCollection.Where(p => p.Category.Contains(category)).OrderByDescending(P => P.Number));
        }

        public void Refresh()
        {
            SupplierList = Common.Instance.GetAllSupplier();
            ProductCollection = Common.Instance.GetAllProduct();
        }
        /// <summary>
        /// PDM查询
        /// </summary>
        /// <param name="pdm"></param>
        public void PDMSearch(string pdm)
        {
            DisplayProductCollection = new ObservableCollection<Product>( ProductCollection.Where(p => p.PDM.Contains(pdm)).OrderByDescending(P=>P.Number));
        }

        /// <summary>
        /// 简码查询
        /// </summary>
        /// <param name="py"></param>
        public void PYSearch(string py)
        {
            DisplayProductCollection = new ObservableCollection<Product>(ProductCollection.Where(p => p.PY.Contains(py.ToUpper())).OrderByDescending(p => p.Number)); 
        }

        /// <summary>
        /// 配件名称查询
        /// </summary>
        /// <param name="productName"></param>
        public void ProductNameSearch(string productName)
        {
            DisplayProductCollection = new ObservableCollection<Product>(ProductCollection.Where(p => p.ProductName.Contains(productName)).OrderByDescending(p => p.Number));
        }

        public void OpenPrintWindow()
        {
            var data = new PurchaseData { Purchase = Purchase, PurchaseItemCollection = PurchaseItemCollection };
            PrintWindow pw = new PrintWindow("print\\PurchaseFlowDocument.xaml", data, new PurchaseDocumentRenderer());
            pw.ShowDialog();
        }

        public void OpenSupplierView()
        {
            SupplierView sv = new SupplierView(SupplierList);
            sv.Show();
        }
        public void OpenProductView()
        {
            ProductView pv = new ProductView(ProductCollection);
            pv.Show();
        }
        #endregion        
    }
    #endregion

    #region 数据库相关方法
    public partial class NewPurchaseViewModel
    {
        /// <summary>
        /// 保存入库单
        /// </summary>
        public void SavePurchaseItem()
        {
            SavePurchase();
        }
        public bool SavePurchase()
        {
            ChangeToEnableSave(false);
            if (PurchaseItemCollection.Count < 1)
                return false;
            if (Purchase.Supplier == null)
                return false;
            if(Purchase.Attendant == null)
            {
                purchase.Attendant = new Attendant() { AttendantID = 8 };//如果没有选择入库人员，自动选中8号
            }
            bool result = false;
            if (oldPurchase == null)
            {
                result = Common.Instance.InsertPurchase(Purchase, PurchaseItemCollection);
            }
            else if (Purchase != oldPurchase || PurchaseItemCollection != oldPurchaseItemCollection)
            {
                
                result = Common.Instance.UpdatePurchase(
                    Purchase,
                    PurchaseItemCollection,
                    oldPurchase,
                    oldPurchaseItemCollection);
            }
            else
            {
                result = true;
            }
            if (result == true)
            {
                oldPurchase = Extentions.CloneExtention.Clone(Purchase);
                oldPurchaseItemCollection = Extentions.CloneExtention.Clone(PurchaseItemCollection);                
            }
            return result;

        }
        private void SavePurchaseItemAndNew()
        {
            if(SavePurchase())
            {
                Purchase = new Purchase
                {
                    PurchaseID = Common.Instance.GetNewPurchaseID()
                };
                PurchaseItemCollection.Clear();

                oldPurchase = null;
                oldPurchaseItemCollection = null;
            }
        }

        /// <summary>
        /// 审核销售单
        /// </summary>
        public void Approve()
        {
            ChangeToEnableSave(false);
            var isSaved = SavePurchase();
            if (isSaved == true)
            {

                bool result = Common.Instance.ApprovePurchaseItem(Purchase, PurchaseItemCollection);
                if (result == false)
                {
                    TipText = "审核失败!";
                    ChangeToEnableSave(true);
                }
                else
                {
                    TipText = "审核成功!";
                    IsEditable = false;
                }
            }
            else
            {
                TipText = "保存入库数据失败！";
            } 
        }
    }
    #endregion
}
