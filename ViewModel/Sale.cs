using Project.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Project.ViewModel
{   
    [Serializable]
    public class Sale:NotificationObject
    {
        private string saleID;
        /// <summary>
        /// 销售单号
        /// </summary>
        public string SaleID
        {
            get
            {
                return saleID;
            }
            set
            {
                if (saleID != value)
                {
                    saleID = value;
                    RaisePropertyChanged(nameof(SaleID));
                }
            }
        }

        private Customer customer;
        /// <summary>
        /// 客户ID
        /// </summary>
        public Customer Customer
        {
            get
            {
                if (customer == null)
                    customer = new Customer();
                return customer;
            }
            set
            {
                if(customer?.CustomerID!=value?.CustomerID)
                {
                    customer = value;
                    RaisePropertyChanged(nameof(Customer));
                }                
            }
        }


        private Attendant attendant;
        /// <summary>
        /// 销售人员ID
        /// </summary>
        public Attendant Attendant
        {
            get
            {
                if(attendant==null)
                {
                    attendant = new Attendant() { AttendantID = 8, AttendantName="杨波"};
                }
                return attendant;
            }
            set
            {
                if (attendant?.AttendantID != value.AttendantID)
                {
                    attendant = value;
                    RaisePropertyChanged(nameof(Attendant));
                }                
            }
        }


        private DateTime saleDate = DateTime.Now;
        /// <summary>
        /// 销售日期
        /// </summary>
        public DateTime SaleDate
        {
            get
            {
                return saleDate;
            }
            set
            {
                if (saleDate != value)
                {
                    saleDate = value;
                    RaisePropertyChanged(nameof(SaleDate));
                }
                
            }
        }


        private Manufacturer manufacturer;
        /// <summary>
        /// 厂家ID
        /// </summary>
        public Manufacturer Manufacturer
        {
            get
            {
                if(manufacturer==null)
                {
                    manufacturer = new Manufacturer() { ManufacturerID = 0 };
                }
                return manufacturer;
            }
            set
            {
                if (manufacturer?.ManufacturerID != value?.ManufacturerID)
                {
                    manufacturer = value;
                    RaisePropertyChanged(nameof(Manufacturer));
                }
            }
        }        

        private string machineID;
        /// <summary>
        /// 整机编号
        /// </summary>
        public string MachineID
        {
            get
            {
                if(machineID==null)
                {
                    machineID = "";
                }
                return machineID;
            }
            set
            {
                if (machineID != value)
                {
                    machineID = value;
                    RaisePropertyChanged(nameof(MachineID));
                }
            }
        }
        
        private string engineID;
        /// <summary>
        /// 发动机编号
        /// </summary>
        public string EngineID
        {
            get
            {
                if(engineID==null)
                {
                    engineID = "";
                }
                return engineID;
            }
            set
            {
                if (engineID != value)
                {
                    engineID = value;
                    RaisePropertyChanged(nameof(EngineID));
                }
            }
        }

        private string note;
        /// <summary>
        /// 备注
        /// </summary>
        public string Note
        {
            get
            {
                if (note == null)
                {
                    note = "";
                }
                return note;
            }
            set
            {
                if (note != value)
                {
                    note = value;
                    RaisePropertyChanged(nameof(Note));
                }
            }
        }

        private bool approved = false;
        /// <summary>
        /// 审核
        /// </summary>
        public bool Approved
        {
            get
            {
                return approved;
            }
            set
            {
                if (approved != value)
                {
                    approved = value;
                    RaisePropertyChanged(nameof(Approved));
                }
            }
        }

        private double sumPrice;
        /// <summary>
        /// 总金额
        /// </summary>
        public double SumPrice
        {
            get
            {
                return sumPrice;
            }
            set
            {
                if (sumPrice != value)
                {
                    sumPrice = value;
                    RaisePropertyChanged(nameof(SumPrice));
                }
            }
        }

        private double salePrice;
        /// <summary>
        /// 配件金额
        /// </summary>
        public double SalePrice
        {
            get
            {
                return salePrice;
            }
            set
            {
                if (salePrice != value)
                {
                    salePrice = value;
                    RaisePropertyChanged(nameof(SalePrice));
                }
            }
        }

        private double servicePrice;
        /// <summary>
        /// 工时费
        /// </summary>
        public double ServicePrice
        {
            get
            {
                return servicePrice;
            }
            set
            {
                if (servicePrice != value)
                {
                    servicePrice = value;
                    RaisePropertyChanged(nameof(ServicePrice));
                }
            }
        }

        private double productProfit;
        /// <summary>
        /// 配件利润
        /// </summary>
        public double ProductProfit
        {
            get
            {
                return productProfit;
            }
            set
            {
                if (productProfit != value)
                {
                    productProfit = value;
                    RaisePropertyChanged(nameof(ProductProfit));
                }
            }
        }
        private double sumProfit;
        /// <summary>
        /// 总利润
        /// </summary>
        public double SumProfit
        {
            get
            {
                return ProductProfit+ServicePrice;
            }
            set
            {
                if (sumProfit != value)
                {
                    sumProfit = value;
                    RaisePropertyChanged(nameof(SumProfit));
                }
            }
        }

        public Sale()
        {
        }
        public static bool operator ==(Sale item1, Sale item2)
        {
            if (object.Equals(item1, null) || object.Equals(item2, null))
                return object.Equals(item1, item2);
            if (item1?.Attendant.AttendantID == item2?.Attendant.AttendantID &&
                item1?.Customer.CustomerID == item2.Customer.CustomerID &&
                item1.EngineID == item2.EngineID &&
                item1.MachineID == item2.MachineID &&
                item1.Manufacturer.ManufacturerID == item2.Manufacturer.ManufacturerID &&
                item1.Note == item2.Note &&
                item1.SaleDate == item2.SaleDate &&
                item1.SaleID == item2.SaleID &&
                item1.Approved == item2.Approved)
            {
                return true;
            }
            return false;
        }
        public static bool operator !=(Sale item1,Sale item2)
        {
            return !(item1 == item2);
        }

        //public override bool Equals(object obj)
        //{
        //    return this == (obj as Sale);
        //}
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }
}
