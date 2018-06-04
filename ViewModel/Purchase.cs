using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Project.Model;

namespace Project.ViewModel
{
    [Serializable]
    public class Purchase : NotificationObject
    {
        private string purchaseID;

        public string PurchaseID
        {
            get { return purchaseID; }
            set
            {
                purchaseID = value;
                RaisePropertyChanged(nameof(PurchaseID));
            }
        }

        private Supplier supplier;

        public Supplier Supplier
        {
            get { return supplier; }
            set
            {
                supplier = value;
                RaisePropertyChanged(nameof(Supplier));
            }
        }

        private Attendant attendant;

        public Attendant Attendant
        {
            get
            {
                if(attendant == null)
                {
                    attendant = new Attendant() { AttendantID = 8, AttendantName = "杨波" };
                }
                return attendant;
            }
            set
            {
                attendant = value;
                RaisePropertyChanged(nameof(Attendant));
            }
        }

        private DateTime purchaseDate = DateTime.Now;

        public DateTime PurchaseDate
        {
            get { return purchaseDate; }
            set
            {
                purchaseDate = value;
                RaisePropertyChanged(nameof(PurchaseDate));
            }
        }

        private string note ="";

        public string Note
        {
            get => note;
            set
            {
                note = value;
                RaisePropertyChanged(nameof(Note));
            }
        }

        private double sumPrice;
        public double SumPrice
        {
            get=>sumPrice;
            set
            {
                sumPrice = value;
                RaisePropertyChanged(nameof(SumPrice));
            }
        }

        private bool approved;
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
                approved = value;
                RaisePropertyChanged(nameof(Approved));
            }
        }

        public static bool operator ==(Purchase item1,Purchase item2)
        {
            if (object.Equals(item1, null) || object.Equals(item2, null))
                return object.Equals(item1, item2);
            if (item1?.Attendant.AttendantID == item2?.Attendant.AttendantID &&
                item1?.Supplier.SupplierID == item2.Supplier.SupplierID &&                
                item1.Note == item2.Note &&
                item1.PurchaseDate == item2.PurchaseDate &&
                item1.PurchaseID == item2.PurchaseID &&
                item1.Approved == item2.Approved)
            {
                return true;
            }
            return false;
        }
        public static bool operator !=(Purchase item1, Purchase item2)
        {
            return !(item1 == item2);
        }
        //public override bool Equals(object obj)
        //{
        //    return this == (obj as Purchase);
        //}
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }
}
