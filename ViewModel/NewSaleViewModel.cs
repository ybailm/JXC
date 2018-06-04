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
using Project.Service;
using System.Data.OleDb;
using Project.Extentions;
using System.Collections.Specialized;
using System.Threading;
using Project.Print;
using System.Diagnostics;

namespace Project.ViewModel
{
    public partial class NewSaleViewModel : NotificationObject
    {
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
        public string SumCaptalPrice { get; set; }
        private double sumPrice;

        public double SumPrice
        {
            get => sumPrice;
            set
            {
                sumPrice = value;
                SumCaptalPrice = sumPrice.ToCaptal();
                RaisePropertyChanged(nameof(SumPrice));
                RaisePropertyChanged(nameof(SumCaptalPrice));

            }
        }

        private ObservableCollection<ServiceDetailItem> oldServiceItemCollection;
        private ObservableCollection<SaleDetailItem> oldSaleItemCollection;
        private Sale oldSale;

        #region 命令绑定及相关方法
        public DelegateCommand<string> ServiceSearchCommand { get; set; }
        public DelegateCommand RefreshCommand { get; set; }
        public DelegateCommand SaveSaleItemCommand { get; set; }
        public DelegateCommand SaveSaleItemAndNewCommand { get; set; }
        public DelegateCommand<AttendantViewModel> AttendantSelectionChangedCommand { get; set; }
        public DelegateCommand<Customer> CustomerSelectionChangedCommand { get; set; }
        public DelegateCommand<Manufacturer> ManufacturerSelectionChangedCommand { get; set; }
        public DelegateCommand<string> CustomerDropDownOpenedCommand { get; set; }
        public DelegateCommand ApproveCommand { get; set; }
        public DelegateCommand OpenPrintViewCommand { get; set; }
        public void Refresh()
        {          
            var servicelist = Common.Instance.GetAllService();
            ServiceList = servicelist;
            DisplayServiceList = servicelist;

            var customerlist = Common.Instance.GetAllCustomer();
            CustomerList = customerlist;
            DisplayCustomerList = customerlist;

        }
        public void ServiceSearch(string serviceName)
        {
            var result = ServiceList?.Where(p => p.ServiceName.Contains(serviceName));
            if (result != null)
                DisplayServiceList = new ObservableCollection<Model.Service>(result);
        }
        public void AttendantSelectionChanged(AttendantViewModel attendantViewModel)
        {
            Sale.Attendant = attendantViewModel.Attendant;
            ChangeToEnableSave(true);
        }

        private void ChangeToEnableSave(bool isEnable)
        {
            canSave = isEnable;
            SaveSaleItemCommand?.RaiseCanExecuteChanged();
            SaveSaleItemAndNewCommand?.RaiseCanExecuteChanged();
        }

        public void CustomerSelectionChanged(Customer customer)
        {
            if (customer != null)
            {
                Sale.Customer = customer;
                ChangeToEnableSave(true);
            }
        }

        public void ManufacturerSelectionChanged(Manufacturer manufacturer)
        {            
            Sale.Manufacturer = manufacturer;
            ChangeToEnableSave(true);
        }

        public void CustomerDropDownOpened(string customerName)
        {            
            DisplayCustomerList = new ObservableCollection<Customer>(CustomerList.Where(p => p.CustomerName.Contains(customerName)));
        }

        public void OpenPrintView()
        {
            Sale.SalePrice = SumSalePrice;
            Sale.ServicePrice = SumServicePrice;
            Sale.SumPrice = SumPrice;
            var data = new SaleData { Sale = Sale, SaleItemCollection = SaleItemCollection, ServiceItemCollection = ServiceItemCollection };
            PrintWindow pw = new PrintWindow("print\\SaleFlowDocument.xaml", data, new SaleDocumentRenderer());
            pw.ShowDialog();
        }
        #endregion

        #region 属性
        private string tipText;
        public string TipText
        {
            get
            {
                if(IsEditable==false)
                    tipText = "已审核";
                return tipText;
            }
            set
            {
                tipText = value;
                RaisePropertyChanged(nameof(TipText));
            }
        }

        private Sale sale;
        public Sale Sale
        {
            get => sale;            
            set
            {
                sale = value;
                RaisePropertyChanged(nameof(Sale));
            }
        }

        public List<string> categoryList;
        public List<string> CategoryList
        {
            get
            {
                if (categoryList == null || categoryList.Count < 1)
                    categoryList = ProductCollection.Select(p => p.Category).Distinct().ToList();
                return categoryList;
            }
            set
            {
                categoryList = value;
                RaisePropertyChanged(nameof(CategoryList));
            }
        }

        /// <summary>
        /// 客户列表
        /// </summary>
        private ObservableCollection<Customer> CustomerList;
        private ObservableCollection<Customer> displayCustomerList;

        public ObservableCollection<Customer> DisplayCustomerList
        {
            get
            {
                if (displayCustomerList == null)
                    displayCustomerList = CustomerList;
                return displayCustomerList;
            }
            set
            {
                displayCustomerList = value;
                RaisePropertyChanged(nameof(DisplayCustomerList));            
            }
        }

        /// <summary>
        /// 服务人员列表
        /// </summary>
        private ObservableCollection<AttendantViewModel> attendantCollection;

        public ObservableCollection<AttendantViewModel> AttendantCollection
        {
            get=> attendantCollection;
            
            set
            {
                attendantCollection = value;
                RaisePropertyChanged(nameof(AttendantCollection));
            }
        }

        /// <summary>
        /// 生产厂家列表
        /// </summary>
        public ObservableCollection<Manufacturer> ManufacturerList { get; set; }
        #endregion

        public NewSaleViewModel()
            :this(null,null,null)
        {

        }

        public NewSaleViewModel(Sale sale) 
            : this(sale,null,null)
        { 

        }

        public NewSaleViewModel(Sale sale,ObservableCollection<SaleDetailItem> saleItemCollection) 
            : this(sale, saleItemCollection,null)
        { 
                      
        }
        public NewSaleViewModel(Sale sale,ObservableCollection<ServiceDetailItem> serviceItemCollection)
            : this(sale, null, serviceItemCollection)
        {

        }

        public NewSaleViewModel(Sale sale, ObservableCollection<SaleDetailItem> saleItemCollection,ObservableCollection<ServiceDetailItem> serviceItemCollection )
        {
            if (sale == null)
            {
                Sale = new Sale
                {
                    SaleID = Common.Instance.GetNewSaleID()
                };
                SaleItemCollection = new ObservableCollection<ViewModel.SaleDetailItem>();
                ServiceItemCollection = new ObservableCollection<ServiceDetailItem>();
            }
            else
            {                
                Sale = sale;

                SaleItemCollection = saleItemCollection ?? Common.Instance.GetSaleDetailItemBySaleID(sale.SaleID);
                SalesItemCollectionChanged();

                ServiceItemCollection = serviceItemCollection ?? Common.Instance.GetServiceDetailItemBySaleID(sale.SaleID);
                ServiceItemCollectionChanged();

                if (sale.Approved != true)
                {
                    oldSale = Extentions.CloneExtention.Clone(Sale);
                    oldSaleItemCollection = Extentions.CloneExtention.Clone<ObservableCollection<SaleDetailItem>>(SaleItemCollection);
                    oldServiceItemCollection = Extentions.CloneExtention.Clone<ObservableCollection<ServiceDetailItem>>(ServiceItemCollection);
                }
            }
            IsEditable = !Sale.Approved;
            
            CategorySelectionChangedCommand = new DelegateCommand<string>(CategorySelectionChanged);

            AttendantCollection = GetAllAttendant();
            DisplayProductCollection = ProductCollection = Common.Instance.GetAllProduct();
            ServiceList = Common.Instance.GetAllService();
            CustomerList = Common.Instance.GetAllCustomer();
            ManufacturerList = Common.Instance.GetAllManufacturer();

            
            SaleItemCollection.CollectionChanged += (seder, args) => SalesItemCollectionChanged();
            
            ServiceItemCollection.CollectionChanged += (sender, args) => ServiceItemCollectionChanged();

            AttendantCollection.CollectionChanged += (send, args) => CheckProcess("144");
            CheckCommand = new DelegateCommand<string>(CheckProcess);

            AddServiceItemCommand = new DelegateCommand<Model.Service>(AddServiceItem);
            DeleteServiceItemCommand = new DelegateCommand<ServiceDetailItem>(DeleteServiceItem);
            EditServiceItemCommand = new DelegateCommand<ServiceDetailItem>(EditServiceItem);

            InputSaleItemListCommand = new DelegateCommand<string>(InputSaleItemListByPurchaseID);
            AddSaleItemCommand = new DelegateCommand<Product>(AddSaleItem);
            DeleteSaleItemCommand = new DelegateCommand<SaleDetailItem>(DeleteSaleItem);
            EditSaleItemCommand = new DelegateCommand<SaleDetailItem>(EditSaleItem);

            SaveSaleItemAndNewCommand = new DelegateCommand(SaveSaleItemAndNew, () => canSave);
            SaveSaleItemCommand = new DelegateCommand(SaveSaleItem, () => canSave);
            
            ApproveCommand = new DelegateCommand(Approve);


            PDMSearchCommand = new DelegateCommand<string>(PDMSearch);
            PYSearchCommand = new DelegateCommand<string>(PYSearch);
            ProductNameSearchCommand = new DelegateCommand<string>(ProductNameSearch);

            CustomerDropDownOpenedCommand = new DelegateCommand<string>(CustomerDropDownOpened);
            RefreshCommand = new DelegateCommand(Refresh);
            ServiceSearchCommand = new DelegateCommand<string>(ServiceSearch);

            AttendantSelectionChangedCommand = new DelegateCommand<AttendantViewModel>(AttendantSelectionChanged);
            CustomerSelectionChangedCommand = new DelegateCommand<Customer>(CustomerSelectionChanged);
            ManufacturerSelectionChangedCommand = new DelegateCommand<Manufacturer>(ManufacturerSelectionChanged);

            OpenPrintViewCommand = new DelegateCommand(OpenPrintView);

        }

        /// <summary>
        /// 获取所有服务人员信息
        /// </summary>
        /// <returns></returns>
        public static ObservableCollection<AttendantViewModel> GetAllAttendant()
        {
            ObservableCollection<AttendantViewModel> result = new ObservableCollection<AttendantViewModel>();
            foreach (var item in Common.Instance.GetAllAttendant())
            {
                result.Add(new AttendantViewModel() { Attendant = item, IsChecked = false, });
            }
            return result;
        }        
    }

    #region 维修业务
    public partial class NewSaleViewModel
    {
        #region 命令属性
        public DelegateCommand<string> CheckCommand { get; set; }

        public DelegateCommand<Model.Service> AddServiceItemCommand { get; set; }
        public DelegateCommand<ServiceDetailItem> DeleteServiceItemCommand { get; set; }
        public DelegateCommand<ServiceDetailItem> EditServiceItemCommand { get; set; }
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

        /// <summary>
        /// 收费标准列表
        /// </summary>
        public ObservableCollection<Project.Model.Service> ServiceList {get;set;}

        private ObservableCollection<Project.Model.Service> displayServiceList;
        public ObservableCollection<Project.Model.Service> DisplayServiceList
        {
            get
            {
                if (displayServiceList == null)
                    displayServiceList = Extentions.CloneExtention.Clone(ServiceList);
                return displayServiceList;
            }
            set
            {
                displayServiceList = value;
                RaisePropertyChanged(nameof(DisplayServiceList));
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
                int i =0;
                foreach (var item in serviceItemCollection)
                {
                    item.ID = ++i;
                }
                RaisePropertyChanged(nameof(ServiceItemCollection));
            }
        }
        #endregion

        #region 方法
        /// <summary>
        /// 当集合发生更改时，重新计算合计金额，并更新序号列
        /// </summary>
        private void ServiceItemCollectionChanged()
        {
            SumServicePrice = 0;
            int i = 0;
            foreach (var item in ServiceItemCollection)
            {
                item.ID = ++i;
                SumServicePrice += item.SumServicePrice;
            }
            SumPrice = sumSalePrice + sumServicePrice;
            ChangeToEnableSave(true);
        }  
        
        /// <summary>
        /// 添加维修项目方法
        /// </summary>
        /// <param name="standard"></param>
        private void AddServiceItem(Model.Service service)
        {
            if (service == null)
                MessageBox.Show("service is null");
            if (ServiceItemCollection == null)
                ServiceItemCollection = new ObservableCollection<ServiceDetailItem>();
            if (ServiceItemCollection?.Where(p => p.Service.ServiceName == service?.ServiceName).Count() > 0)
            {
                TipText = "所选项目已经存在，请直接更改价格或数量";
                return;
            }
            ServiceItemCollection.Add(new ServiceDetailItem( Sale.SaleID, service ));
        }

        /// <summary>
        /// 删除维修项目方法
        /// </summary>
        /// <param name="obj"></param>
        private void DeleteServiceItem(ServiceDetailItem obj)
        {
            ServiceItemCollection.Remove(obj);
        }

        /// <summary>
        /// 编辑维修项目方法
        /// </summary>
        /// <param name="obj"></param>
        public void EditServiceItem(ServiceDetailItem obj)
        {
            SumServicePrice = ServiceItemCollection.Sum(p => p.SumServicePrice);           
        }

        public void CheckProcess(string txt)
        {
            Sale.Note = "";
            foreach (var item in AttendantCollection)
            {
                if (item.IsChecked == true)
                    Sale.Note += (item.Attendant.AttendantName + " ");
            }
            ChangeToEnableSave(true);
        }
        #endregion
    }
    #endregion

    #region 配件销售业务
    public partial class NewSaleViewModel
    {
        #region 命令属性
        public DelegateCommand<string> InputSaleItemListCommand { get; set; }
        public DelegateCommand<Model.Product> AddSaleItemCommand { get; set; }
        public DelegateCommand<SaleDetailItem> DeleteSaleItemCommand { get; set; }
        public DelegateCommand<SaleDetailItem> EditSaleItemCommand { get; set; }
        
        public DelegateCommand<string> PDMSearchCommand { get; set; }
        public DelegateCommand<string> PYSearchCommand { get; set; }
        public DelegateCommand<string> ProductNameSearchCommand { get; set; }
        public DelegateCommand<string> CategorySelectionChangedCommand { get; set; }
        #endregion

        #region 属性  
        /// <summary>
        /// 合计配件金额大写
        /// </summary>
        public string SumCaptalSalePrice { get; set; }

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
                SumCaptalSalePrice = sumSalePrice.ToCaptal();

                RaisePropertyChanged(nameof(SumSalePrice));
                RaisePropertyChanged(nameof(SumCaptalSalePrice));                
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
                return displayProductCollection;
            }
            set
            {
                displayProductCollection = value;
                RaisePropertyChanged(nameof(DisplayProductCollection));
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

        #endregion

        #region 方法
        /// <summary>
        /// 当集合发生更改时，重新计算合计金额，并更新序号列
        /// </summary>
        private void SalesItemCollectionChanged()
        {
            SumSalePrice = 0;
            int i = 0;
            foreach (var item in SaleItemCollection)
            {
                item.ID = ++i;
                SumSalePrice += item.SumSalePrice;
            }
            SumPrice = sumSalePrice + sumServicePrice;
            ChangeToEnableSave(true);
        }

        private void InputSaleItemListByPurchaseID(string purchaseID)
        {
            var list = Common.Instance.GetPurchaseDetailItemByPurchaseID(purchaseID);
            if (list?.Count > 0)
            {
                foreach(var item in list)
                {
                    SaleItemCollection.Add(new SaleDetailItem()
                    {
                        Product = item.Product,
                        Number = item.Number,
                        CostPrice = item.CostPrice,
                        UnitPrice = item.UnitPrice,
                    });
                }
            }
            else
            {
                TipText = "不存在该入库单";
            }
        }

        private void AddSaleItem(Product product)
        {
            if (product == null)
                return;
            if (SaleItemCollection == null)
                SaleItemCollection = new ObservableCollection<SaleDetailItem>();

            if (SaleItemCollection.Where(p => p.Product.ProductID == product.ProductID).Count() > 0)
            {
                TipText = "所选项目已经存在，请直接更改价格或数量";
                return;
            }
            SaleItemCollection.Add(new SaleDetailItem(Sale.SaleID, product));           
        }

        private void DeleteSaleItem(SaleDetailItem obj)
        {
            SaleItemCollection.Remove(obj);            
        }
        public void EditSaleItem(SaleDetailItem obj)
        {
            SumSalePrice = SaleItemCollection.Sum(p => p.SumSalePrice);            
        }        
        
        public void PDMSearch(string pdm)
        {
            DisplayProductCollection = new ObservableCollection<Product>( ProductCollection.Where(p => p.PDM.Contains(pdm)).OrderByDescending(P=>P.Number));
        }
        public void PYSearch(string py)
        {
            DisplayProductCollection = new ObservableCollection<Product>(ProductCollection.Where(p => p.PY.Contains(py.ToUpper())).OrderByDescending(p => p.Number)); 
        }

        public void CategorySelectionChanged(string category)
        {
            DisplayProductCollection = new ObservableCollection<Product>(ProductCollection.Where(p => p.Category.Contains(category)).OrderByDescending(P => P.Number));
        }

        public void ProductNameSearch(string productName)
        {
            DisplayProductCollection = new ObservableCollection<Product>(ProductCollection.Where(p => p.ProductName.Contains(productName)).OrderByDescending(P => P.Number));
        }
        #endregion        
    }
    #endregion


    public partial class NewSaleViewModel
    {

        /// <summary>
        /// 保存销售单
        /// </summary>
        public void SaveSaleItem()
        {
            SaveSale();
        }
        public void SaveSaleItemAndNew()
        {
            if (SaveSale() == true)
            {
                Sale = new Sale()
                {
                    SaleID = Common.Instance.GetNewSaleID()
                };
                SaleItemCollection.Clear();
                ServiceItemCollection.Clear();

                oldSale = null;
                oldSaleItemCollection = null;
                oldServiceItemCollection = null;
            }
        }
        /// <summary>
        /// 保存销售单
        /// </summary>
        public bool SaveSale()
        {
            ChangeToEnableSave(false);
            if (SaleItemCollection.Count < 1 && ServiceItemCollection.Count < 1)
            {
                TipText = "没有添加销售或者维修项目";
                return false;
            }
            if (Sale.Customer.CustomerName.IsNullOrEmptyOrWhiteSpace())
            {
                TipText = "未选择客户";
                return false;
            }

            bool result = false;
            if (oldSale == null)
            {
                result = Common.Instance.InsertSale(Sale, SaleItemCollection, ServiceItemCollection);
            }
            else if (Sale != oldSale || SaleItemCollection != oldSaleItemCollection || ServiceItemCollection != oldServiceItemCollection)
            {
                result = Common.Instance.UpdateSale(
                    Sale,
                    SaleItemCollection,
                    ServiceItemCollection,
                    oldSale,
                    oldSaleItemCollection,
                    oldServiceItemCollection);
            }
            else
            {
                result = true;
            }
            if (result == true)
            {
                oldSale = Extentions.CloneExtention.Clone(Sale);
                oldSaleItemCollection = Extentions.CloneExtention.Clone(SaleItemCollection);
                oldServiceItemCollection = Extentions.CloneExtention.Clone(ServiceItemCollection);
            }
            return result;
        }
        /// <summary>
        /// 审核销售单
        /// </summary>
        public void Approve()
        {
            ChangeToEnableSave(false);
            var isSaved = SaveSale();
            if (isSaved == true)
            {

                bool result = Common.Instance.ApproveSaleItem(Sale, SaleItemCollection);
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
                TipText = "保存销售数据失败！";
            }
        }
    }
}
