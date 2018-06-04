using Microsoft.International.Converters.PinYinConverter;
using Project.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project.Extentions
{
    public static class StringExtention
    {
        public static bool ConvertToBoolean(this string str)
        {
            if (str.IsNullOrEmptyOrWhiteSpace())
                throw new ArgumentException();
            if (str == "0" || str.ToLower() == "false")
                return false;
            if (str == "1" || str.ToLower() == "true")
                return true;
            throw new ArgumentException("字符串不可转换为有效的bool值");
        }
        /// <summary>
        /// 把汉字字符串转换为拼音首字母大写
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static string ConvertToCaptal(this string str)
        {
            string r = string.Empty;
            foreach (char obj in str)
            {
                try
                {
                    ChineseChar chineseChar = new ChineseChar(obj);
                    string t = chineseChar.Pinyins[0].ToString();
                    r += t.Substring(0, 1);
                }
                catch
                {
                    r += obj.ToString();
                }
            }
            return r;
        }

        /// <summary>
        /// 判断字符串是否是null、空字符串或者空白字符串
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static bool IsNullOrEmptyOrWhiteSpace(this string str)
        {
            if (string.IsNullOrEmpty(str))
                return true;
            if (string.IsNullOrWhiteSpace(str))
                return true;
            return false;
        }
        public static WindowName ConvertToWindowName(this string str)
        {
            WindowName name = 0;
            switch (str)
            {
                case "NewSaleView":
                    name = WindowName.NewSaleView;
                    break;
                case "CustomerView":
                    name = WindowName.CustomerView;
                    break;
                case "NewPurchaseView":
                    name = WindowName.NewPurchaseView;
                    break;
                case "SupplierView":
                    name = WindowName.SupplierView;
                    break;
                case "SaleView":
                    name = WindowName.SaleView;
                    break;
                case "PurchaseView":
                    name = WindowName.PurchaseView;
                    break;
                case "ProductView":
                    name = WindowName.ProductView;
                    break;
                case "ServiceView":
                    name = WindowName.ServiceView;
                    break;
                case "ChargeView":
                    name = WindowName.ChargeView;
                    break;
                case "SaleReportView":
                    name = WindowName.SaleReportView;
                    break;
                case "PurchaseReportView":
                    name = WindowName.PurchaseReportView;
                    break;
                case "ProductReportView":
                    name = WindowName.ProductReportView;
                    break;
            }
            return name;
        }
    }
}
