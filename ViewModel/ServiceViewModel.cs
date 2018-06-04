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
    public partial class ServiceViewModel : NotificationObject
    {
        public List<string> CategoryList { get; set; }

        public ServiceViewModel()
        {
            GridVisibility = Visibility.Collapsed;

            ServiceCollection = Common.Instance.GetAllService();

            DisplayServiceCollection = ServiceCollection;

            CategoryList = ServiceCollection.Select(p => p.Category).Distinct().ToList();

            GridCollapseCommand = new DelegateCommand(GridCollapse);

            NewServiceCommand = new DelegateCommand(NewService);
            EditServiceItemCommand = new DelegateCommand<Model.Service>(EditService);
            SaveServiceCommand = new DelegateCommand<Model.Service>(SaveService);
            ServiceNameSearchCommand = new DelegateCommand<string>(ServiceNameSearch);
            
            ServiceNameChangedCommand = new DelegateCommand<string>(ServiceNameChanged);
            CategorySelectionChangedCommand = new DelegateCommand<string>(CategorySelectionChanged);            
        }
                
    }
    #endregion

    #region 维修项目管理业务
    public partial class ServiceViewModel : NotificationObject
    {
        #region 命令属性
        public DelegateCommand GridCollapseCommand { get; set; }

        public DelegateCommand NewServiceCommand { get; set; }
        public DelegateCommand<Model.Service> DeleteServiceCommand { get; set; }
        public DelegateCommand<Model.Service> EditServiceItemCommand { get; set; }

        public DelegateCommand<Model.Service> SaveServiceCommand { get; set; }

        public DelegateCommand<string> ServiceNameSearchCommand { get; set; }

        public DelegateCommand<string> ServiceNameChangedCommand { get; set; }
        public DelegateCommand<string> CategorySelectionChangedCommand { get; set; }
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
        private ObservableCollection<Model.Service> displayServiceCollection;
        public ObservableCollection<Model.Service> DisplayServiceCollection
        {
            get
            {
                if(displayServiceCollection == null)
                {
                    displayServiceCollection = ServiceCollection;
                }
                return displayServiceCollection;
            }
            set
            {
                displayServiceCollection = value;
                RaisePropertyChanged(nameof(DisplayServiceCollection));
            }
        }

        public Model.Service currentService;
        public Model.Service CurrentService
        {
            get
            {
                return currentService;
            }
            set
            {
                currentService = value;
                RaisePropertyChanged(nameof(CurrentService));
            }
        }
       
        /// <summary>
        /// 所有货品列表
        /// </summary>
        public ObservableCollection<Model.Service> ServiceCollection { get; set; }
        #endregion

        #region 方法 
        

        public void GridCollapse()
        {
            GridVisibility = Visibility.Collapsed;
        }

        public void EditService(Model.Service service)
        {
            CurrentService = service;
            GridVisibility = Visibility.Visible;
        }

        public void NewService()
        {
            CurrentService = new Model.Service() { ServiceID = -1 };           
            GridVisibility = Visibility.Visible;
        }

        public void SaveService(Model.Service service)
        {
            if (service.ServiceName.IsNullOrEmptyOrWhiteSpace())
                return;

            Common.Instance.SaveService(service);

            DisplayServiceCollection = ServiceCollection = Common.Instance.GetAllService();
        }
        
        public void ServiceNameSearch(string serviceName)
        {
            DisplayServiceCollection = new ObservableCollection<Model.Service>( ServiceCollection.Where(p => p.ServiceName.Contains(serviceName)));
        }

        public void CategorySelectionChanged(string category)
        {
            CurrentService.Category = category;
            CurrentService = CloneExtention.Clone(currentService);
        }

        public void ServiceNameChanged(string serviceName)
        {
            CurrentService = CloneExtention.Clone(currentService);
        }
        #endregion
    }
    #endregion
}
