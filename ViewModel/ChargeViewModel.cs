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
using Project.Commands;

namespace Project.ViewModel
{
    #region 构造函数初始化
    public partial class ChargeViewModel : NotificationObject
    {
        public List<string> UnitList { get; set; }
        public List<string> CategoryList { get; set; }
        public List<string> AttendantList { get; set; }

        public ChargeViewModel()
        {
            GridVisibility = Visibility.Collapsed;

            ChargeCollection = Common.Instance.GetAllCharge();

            DisplayChargeCollection = ChargeCollection;
            
            UnitList = ChargeCollection.Select(p => p.Unit).Distinct().ToList();
            CategoryList = ChargeCollection.Select(p => p.Category).Distinct().ToList();
            AttendantList = ChargeCollection.Select(p => p.AttendantName).Distinct().ToList();

            GridCollapseCommand = new DelegateCommand(GridCollapse);

            NewChargeCommand = new DelegateCommand(NewCharge);
            EditChargeItemCommand = new DelegateCommand<Charge>(EditCharge);
            SaveChargeCommand = new DelegateCommand<Charge>(SaveCharge);
            ChargeNameSearchCommand = new DelegateCommand<string>(ChargeNameSearch);
            CategorySearchCommand = new DelegateCommand<string>(CategorySearch);

            ChargeNameChangedCommand = new DelegateCommand<string>(ChargeNameChanged);
            CategorySelectionChangedCommand = new DelegateCommand<string>(CategorySelectionChanged);
            UnitSelectionChangedCommand = new DelegateCommand<string>(UnitSelectionChanged);
            AttendantSelectionChangedCommand = new DelegateCommand<string>(AttendantSelectionChanged);

            DateReport();
            CategoryReport();
        }       
    }
    #endregion

    #region 费用管理业务
    public partial class ChargeViewModel : NotificationObject
    {
        #region 命令属性
        public DelegateCommand GridCollapseCommand { get; set; }

        public DelegateCommand NewChargeCommand { get; set; }
        public DelegateCommand<Charge> DeleteChargeCommand { get; set; }
        public DelegateCommand<Charge> EditChargeItemCommand { get; set; }

        public DelegateCommand<Charge> SaveChargeCommand { get; set; }

        public DelegateCommand<string> ChargeNameSearchCommand { get; set; }
        public DelegateCommand<string> PYSearchCommand { get; set; }
        public DelegateCommand<string> CategorySearchCommand { get; set; }

        public DelegateCommand<string> ChargeNameChangedCommand { get; set; }
        public DelegateCommand<string> CategorySelectionChangedCommand { get; set; }
        public DelegateCommand<string> UnitSelectionChangedCommand { get; set; }
        public DelegateCommand<string> AttendantSelectionChangedCommand { get; set; }
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
        private double sumPrice;

        public double SumPrice
        {
            get => sumPrice;
            set
            {
                sumPrice = value;
                SumCaptalPrice = value.ToCaptal();
                RaisePropertyChanged(nameof(SumPrice));
                RaisePropertyChanged(nameof(SumCaptalPrice));

            }
        }     
        public string SumCaptalPrice { get; set; }
        

        /// <summary>
        /// 显示的费用列表
        /// </summary>
        private ObservableCollection<Charge> displayChargeCollection;
        public ObservableCollection<Charge> DisplayChargeCollection
        {
            get
            {
                if(displayChargeCollection == null)
                {
                    displayChargeCollection = ChargeCollection;
                }
                return displayChargeCollection;
            }
            set
            {
                displayChargeCollection = value;
                SumPrice = displayChargeCollection.Sum(p => p.Price);
                RaisePropertyChanged(nameof(DisplayChargeCollection));
            }
        }

        private Charge currentCharge;
        public Charge CurrentCharge
        {
            get
            {
                return currentCharge;
            }
            set
            {
                currentCharge = value;
                RaisePropertyChanged(nameof(CurrentCharge));
            }
        }
       
        /// <summary>
        /// 所有费用列表
        /// </summary>
        public ObservableCollection<Charge> ChargeCollection { get; set; }
        #endregion

        #region 方法 
        public void GridCollapse()
        {
            GridVisibility = Visibility.Collapsed;
        }

        public void EditCharge(Charge charge)
        {
            CurrentCharge = charge;
            GridVisibility = Visibility.Visible;
        }

        public void NewCharge()
        {
            CurrentCharge = new Model.Charge() { ChargeID = -1 };           
            GridVisibility = Visibility.Visible;
        }

        public void SaveCharge(Charge charge)
        {
            if (charge.ChargeName.IsNullOrEmptyOrWhiteSpace())
                return;
            Common.Instance.SaveCharge(charge);
            DisplayChargeCollection = ChargeCollection = Common.Instance.GetAllCharge();
            CurrentCharge = new Model.Charge() { ChargeID = -1, AttendantName = charge.AttendantName, Category="差旅费杂费",  };
        }

        public void CategorySearch(string categoryName)
        {
            DisplayChargeCollection = new ObservableCollection<Charge>(ChargeCollection.Where(p => p.Category.Contains(categoryName)));
        }
        public void ChargeNameSearch(string chargeName)
        {
            DisplayChargeCollection = new ObservableCollection<Charge>( ChargeCollection.Where(p => p.ChargeName.Contains(chargeName)));
        }

        public void AttendantSelectionChanged(string attendantName)
        {
            CurrentCharge.AttendantName = attendantName;
            CurrentCharge = CloneExtention.Clone(currentCharge);
        }

        public void UnitSelectionChanged(string unit)
        {
            CurrentCharge.Unit = unit;
            CurrentCharge = CloneExtention.Clone(currentCharge);
        }

        public void CategorySelectionChanged(string category)
        {
            CurrentCharge.Category = category;
            CurrentCharge = CloneExtention.Clone(currentCharge);
        }

        public void ChargeNameChanged(string chargeName)
        {
            CurrentCharge = CloneExtention.Clone(currentCharge);
        }
        #endregion
    }
    #endregion

    #region 费用报表业务
    public partial class ChargeViewModel : NotificationObject
    {
        private ObservableCollection<ChargeDateReport> chargeDateReportCollection = new ObservableCollection<ChargeDateReport>();
        public ObservableCollection<ChargeDateReport> ChargeDateReportCollection
        {
            get
            {
                return chargeDateReportCollection;
            }
            set
            {
                chargeDateReportCollection = value;
                RaisePropertyChanged(nameof(ChargeDateReportCollection));
            }
        }
        private ObservableCollection<ChargeCategoryReport> chargeCategoryReportCollection = new ObservableCollection<ChargeCategoryReport>();
        public ObservableCollection<ChargeCategoryReport> ChargeCategoryReportCollection
        {
            get
            {
                return chargeCategoryReportCollection;
            }
            set
            {
                chargeCategoryReportCollection = value;
                RaisePropertyChanged(nameof(ChargeCategoryReportCollection));
            }
        }
        private void DateReport()
        {

            DateTime min = DateTime.Parse(ChargeCollection.Min(p => p.PayDate).ToString("yyyy-MM"));
            DateTime max = DateTime.Parse(ChargeCollection.Max(p => p.PayDate).ToString("yyyy-MM"));
            int id = 1;
            DateTime begin = min;
            while (begin <= max)
            {                
                double sum = ChargeCollection.Where(p => p.PayDate.Year == begin.Year && p.PayDate.Month == begin.Month).Sum(p => p.Price);
                ChargeDateReportCollection.Add(new ChargeDateReport() { ID = id, Date = begin, Price = sum });
                id += 1;   
                begin = begin.AddMonths(1);
            }


        }
        

        public class ChargeDateReport
        {
            public int ID { get; set; }
            public DateTime Date { get; set; }
            public double Price { get; set; }
        }

        private void CategoryReport()
        {            
            int id = 1;            
            foreach(string item in CategoryList)
            {
                double sum = ChargeCollection.Where(p => p.Category == item).Sum(p => p.Price);
                ChargeCategoryReportCollection.Add(new ChargeCategoryReport() { ID = id, Category = item, Price = sum });
                id += 1;                
            }
        }


        public class ChargeCategoryReport
        {
            public int ID { get; set; }
            public string Category { get; set; }
            public double Price { get; set; }
        }

    }
    #endregion
}
