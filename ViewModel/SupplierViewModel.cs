using System.Data;
using System.Linq;
using Project.Model;
using System.Collections.ObjectModel;
using Project.Commands;
using System.Windows;
using Project.Extentions;

namespace Project.ViewModel
{
    #region 构造函数初始化
    public partial class SupplierViewModel : NotificationObject
    {
        public ObservableCollection<Supplier> SupplierCollection { get; private set; }

        public SupplierViewModel():this(null)
        { 
        }
        public SupplierViewModel(ObservableCollection<Supplier> supplierCollection)
        {
            GridVisibility = Visibility.Collapsed;
            if(supplierCollection?.Count>0)
            {
                SupplierCollection = supplierCollection;
            }
            else
            {
                SupplierCollection = Common.Instance.GetAllSupplier();
            }
            DisplaySupplierCollection = SupplierCollection;

            GridCollapseCommand = new DelegateCommand(GridCollapse);

            NewSupplierCommand = new DelegateCommand(NewSupplier);
            EditSupplierItemCommand = new DelegateCommand<Supplier>(EditSupplier);
            SaveSupplierCommand = new DelegateCommand<Supplier>(SaveSupplier);
            SupplierNameSearchCommand = new DelegateCommand<string>(SupplierNameSearch);
            PYSearchCommand = new DelegateCommand<string>(PYSearch);

        }
    }
    #endregion

    #region 供应商管理业务
    public partial class SupplierViewModel : NotificationObject
    {
        #region 命令属性
        public DelegateCommand GridCollapseCommand { get; set; }
        public DelegateCommand NewSupplierCommand { get; set; }
        public DelegateCommand<Supplier> DeleteSupplierCommand { get; set; }
        public DelegateCommand<Supplier> EditSupplierItemCommand { get; set; }
        public DelegateCommand<Supplier> SaveSupplierCommand { get; set; }
        public DelegateCommand<string> SupplierNameSearchCommand { get; set; }
        public DelegateCommand<string> PYSearchCommand { get; set; }
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
        /// 显示的供应商列表
        /// </summary>
        private ObservableCollection<Supplier> displaySupplierCollection;
        public ObservableCollection<Supplier> DisplaySupplierCollection
        {
            get
            {
                if(displaySupplierCollection == null)
                {
                    displaySupplierCollection = SupplierCollection;
                }
                return displaySupplierCollection;
            }
            set
            {
                displaySupplierCollection = value;
                RaisePropertyChanged(nameof(DisplaySupplierCollection));
            }
        }

        public Supplier currentSupplier;
        public Supplier CurrentSupplier
        {
            get
            {
                return currentSupplier;
            }
            set
            {
                currentSupplier = value;
                RaisePropertyChanged(nameof(CurrentSupplier));
            }
        }       
       
        #endregion

        #region 方法 
        

        public void GridCollapse()
        {
            GridVisibility = Visibility.Collapsed;
        }

        public void EditSupplier(Supplier supplier)
        {
            CurrentSupplier = supplier;
            GridVisibility = Visibility.Visible;
        }

        public void NewSupplier()
        {
            CurrentSupplier = new Model.Supplier();
            CurrentSupplier.SupplierID = -1;
            GridVisibility = Visibility.Visible;
        }

        public void SaveSupplier(Supplier supplier)
        {
            if (supplier.SupplierName.IsNullOrEmptyOrWhiteSpace())
                return;
            Common.Instance.SaveSupplier(supplier);
            DisplaySupplierCollection =  SupplierCollection = Common.Instance.GetAllSupplier(); 
        }
        
        public void SupplierNameSearch(string supplierName)
        {
            DisplaySupplierCollection = new ObservableCollection<Supplier>( SupplierCollection.Where(p => p.SupplierName.Contains(supplierName)).OrderByDescending(P=>P.SupplierID));
        }
        public void PYSearch(string py)
        {
            DisplaySupplierCollection = new ObservableCollection<Supplier>(SupplierCollection.Where(p => p.PY.Contains(py.ToUpper())).OrderByDescending(p => p.SupplierID)); 
        }
        #endregion        
    }
    #endregion
}
