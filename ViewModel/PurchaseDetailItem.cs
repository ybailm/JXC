using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project.ViewModel
{
    [Serializable]
    public class PurchaseDetailItem:DetailItem
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

        public PurchaseDetailItem()
        {
                
        }
        public PurchaseDetailItem(Model.Product product)
        {
            if (product!= null)
            {                
                Product = product;
                Number = 1;
                UnitPrice = product.UnitPrice;
                CostPrice = product.CostPrice;
            }
        }

        public static bool operator ==(PurchaseDetailItem item1, PurchaseDetailItem item2)
        {
            if (item1.PurchaseID == item2.PurchaseID &&
                item1.Product.ProductID == item2.Product.ProductID &&
                item1.Number == item2.Number &&
                item1.CostPrice == item2.CostPrice &&
                item1.UnitPrice == item2.UnitPrice)
            {
                return true;
            }
            return false;
        }
        public static bool operator !=(PurchaseDetailItem item1, PurchaseDetailItem item2)
        {
            return !(item1 == item2);
        }
        //public override bool Equals(object obj)
        //{
        //    return this == (obj as PurchaseDetailItem);
        //}
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }
}
