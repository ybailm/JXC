using System.Data;
using System.Linq;
using Project.Model;
using System.Collections.ObjectModel;
using System.Windows;
using Project.Extentions;
using Project.Commands;

namespace Project.ViewModel
{
    #region 构造函数初始化
    public partial class CustomerViewModel : NotificationObject
    {
        public CustomerViewModel()
        {
            GridVisibility = Visibility.Collapsed;

            CustomerCollection = Common.Instance.GetAllCustomer();

            DisplayCustomerCollection = CustomerCollection;

            GridCollapseCommand = new DelegateCommand(GridCollapse);

            NewCustomerCommand = new DelegateCommand<Model.Customer>(NewCustomer);
            EditCustomerItemCommand = new DelegateCommand<Customer>(EditCustomer);
            SaveCustomerCommand = new DelegateCommand<Customer>(SaveCustomer);
            CustomerNameSearchCommand = new DelegateCommand<string>(CustomerNameSearch);
            PYSearchCommand = new DelegateCommand<string>(PYSearch);
            CustomerNameChangedCommand = new DelegateCommand<string>(CustomerNameChanged);

        }
    }
    #endregion

    #region 客户管理业务
    public partial class CustomerViewModel : NotificationObject
    {
        #region 命令属性
        public DelegateCommand GridCollapseCommand { get; set; }
        public DelegateCommand<Model.Customer> NewCustomerCommand { get; set; }
        public DelegateCommand<Customer> DeleteCustomerCommand { get; set; }
        public DelegateCommand<Customer> EditCustomerItemCommand { get; set; }
        public DelegateCommand<Customer> SaveCustomerCommand { get; set; }
        public DelegateCommand<string> CustomerNameSearchCommand { get; set; }
        public DelegateCommand<string> PYSearchCommand { get; set; }
        public DelegateCommand<string> CustomerNameChangedCommand { get; set; }
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
        /// 显示的客户列表
        /// </summary>
        private ObservableCollection<Customer> displayCustomerCollection;
        public ObservableCollection<Customer> DisplayCustomerCollection
        {
            get
            {
                if(displayCustomerCollection == null)
                {
                    displayCustomerCollection = CustomerCollection;
                }
                return displayCustomerCollection;
            }
            set
            {
                displayCustomerCollection = value;
                RaisePropertyChanged(nameof(DisplayCustomerCollection));
            }
        }

        public Customer currentCustomer;
        public Customer CurrentCustomer
        {
            get
            {
                return currentCustomer;
            }
            set
            {
                currentCustomer = value;
                RaisePropertyChanged(nameof(CurrentCustomer));
            }
        }
       
        /// <summary>
        /// 所有客户列表
        /// </summary>
        public ObservableCollection<Customer> CustomerCollection { get; set; }
        #endregion

        #region 方法 
        public void GridCollapse()
        {
            GridVisibility = Visibility.Collapsed;
        }

        public void EditCustomer(Customer customer)
        {
            CurrentCustomer = customer;
            GridVisibility = Visibility.Visible;
        }

        public void NewCustomer(Customer customer)
        {
            CurrentCustomer = new Model.Customer();           
            GridVisibility = Visibility.Visible;
        }

        public void SaveCustomer(Customer customer)
        {
            if (customer.CustomerName.IsNullOrEmptyOrWhiteSpace())
                return;

            Common.Instance.SaveCustomer(customer);
            DisplayCustomerCollection = CustomerCollection = Common.Instance.GetAllCustomer();            
        }
        
        public void CustomerNameSearch(string customerName)
        {
            DisplayCustomerCollection = new ObservableCollection<Customer>( CustomerCollection.Where(p => p.CustomerName.Contains(customerName)).OrderByDescending(P=>P.CustomerID));
        }
        public void PYSearch(string py)
        {
            DisplayCustomerCollection = new ObservableCollection<Customer>(CustomerCollection.Where(p => p.PY.Contains(py.ToUpper())).OrderByDescending(p => p.CustomerID)); 
        }
        public void CustomerNameChanged(string customerName)
        {
            currentCustomer.PY = customerName.ConvertToCaptal();
            CurrentCustomer = CloneExtention.Clone(currentCustomer);
        }
        #endregion        
    }
    #endregion
}
