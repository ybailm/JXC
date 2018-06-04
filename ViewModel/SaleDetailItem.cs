using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project.ViewModel
{
    [Serializable]
    public class SaleDetailItem : DetailItem
    {
        private string saleID;
        public string SaleID
        {
            get => saleID;
            set
            {
                saleID = value;
                RaisePropertyChanged(nameof(SaleID));
            }
        }
        public SaleDetailItem()
        {
            Note = "";
        }
        public SaleDetailItem(string saleID, Model.Product product)
        {
            if (product != null)
            {
                SaleID = saleID;
                Product = product;
                Number = 1;
                UnitPrice = product.UnitPrice;
                CostPrice = (product.Number > 0) ? product.CostPrice : 0;
            }
        }
        public static bool operator ==(SaleDetailItem item1, SaleDetailItem item2)
        {
            if( item1.SaleID == item2.SaleID &&
                item1.Product.ProductID==item2.Product.ProductID &&
                item1.Number==item2.Number &&
                item1.CostPrice ==item2.CostPrice &&
                item1.UnitPrice==item2.UnitPrice)
            {
                return true;
            }
            return false;
        }
        public static bool operator !=(SaleDetailItem item1,SaleDetailItem item2)
        {
            return !(item1 == item2);
        }
        //public override bool Equals(object obj)
        //{
        //    return this == (obj as SaleDetailItem);
        //}
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }
}
