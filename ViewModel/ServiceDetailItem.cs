using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project.ViewModel
{
    [Serializable]
    public class ServiceDetailItem : NotificationObject
    {
        private int id;
        /// <summary>
        /// 序号
        /// </summary>
        public int ID 
        {
            get
            {
                return id;
            }
            set
            {
                id = value;
                RaisePropertyChanged(nameof(ID));
            }
        }
        public string SaleID { get; set; }

        /// <summary>
        /// 收费标准
        /// </summary>
        public Model.Service Service { get; set; }

        private int number = 1;
        /// <summary>
        /// 数量
        /// </summary>
        public int Number
        {
            get
            {
                return number;
            }
            set
            {
                number = value;
                RaisePropertyChanged(nameof(Number));
                RaisePropertyChanged(nameof(SumServicePrice));
            }
        }

        private double unitPrice;
        /// <summary>
        /// 单价
        /// </summary>
        public double UnitPrice
        {
            get
            {
                return unitPrice;
            }
            set
            {
                unitPrice = value;
                RaisePropertyChanged(nameof(UnitPrice));
                RaisePropertyChanged(nameof(SumServicePrice));
            }
        }
        /// <summary>
        /// 金额
        /// </summary>
        public double SumServicePrice { get { return UnitPrice * Number; } }

        public ServiceDetailItem()
        {
                
        }
        public ServiceDetailItem(string saleID, Model.Service service)
        {
            if (service != null)
            {
                SaleID = saleID;
                Service = service;
                Number = 1;
                UnitPrice = service.UnitPrice;
            }
        } 
        
        public static bool operator ==(ServiceDetailItem s1,ServiceDetailItem s2)
        {
            if( s1.Number == s2.Number &&
                s1.SaleID == s2.SaleID &&
                s1.Service.ServiceID == s2.Service.ServiceID &&
                s1.UnitPrice == s2.UnitPrice)
            {
                return true;
            }
            return false;
        }
        public static bool operator !=(ServiceDetailItem s1, ServiceDetailItem s2)
        {
            return !(s1 == s2);
        }
        //public override bool Equals(object obj)
        //{
        //    return this == (obj as ServiceDetailItem);
        //}
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }
}
