using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project.ViewModel
{
    [Serializable]
    public class DetailItem:NotificationObject
    {
        private int id;
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

        public Model.Product Product { get; set; }

        private int number = 1;
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
                RaisePropertyChanged(nameof(SumCostPrice));
                RaisePropertyChanged(nameof(SumSalePrice));
            }
        }

        private double unitPrice;
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
                RaisePropertyChanged(nameof(SumSalePrice));
            }
        }

        private double costPrice;
        public double CostPrice
        {
            get
            {
                return costPrice;
            }
            set
            {
                costPrice = value;
                RaisePropertyChanged(nameof(CostPrice));
                RaisePropertyChanged(nameof(SumCostPrice));               
            }
        }

        public double SumSalePrice
        {
            get
            {
                return UnitPrice * Number;                
            }
            set
            {
                RaisePropertyChanged(nameof(SumSalePrice));
            }
        }

        public double SumCostPrice
        {
            get
            {
                return CostPrice * Number;
            }
            set
            {
                RaisePropertyChanged(nameof(SumCostPrice));
            }
        }

        private string note;
        public string Note
        {
            get
            {                
                return note??"";
            }
            set
            {
                note = value;
                RaisePropertyChanged(nameof(Note));
            }
        }
    }
}
