using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Project.Extentions;
using System.ComponentModel;
using System.Data;
using System.Windows;

namespace Project.Model
{
    [Serializable]
    public class Product
    {
        public int ProductID { get; set; } = -1;
        public string PDM { get; set; } = "";
        public string ProductName { get; set; } = "";
        public string PY { get; set; } = "";
        public string Category { get; set; } = "";
        public string Unit { get; set; } = "";
        public int Number { get; set; } = 0;
        public double UnitPrice { get; set; } = 0;
        public double CostPrice { get; set; } = 0;

        public Product(int productID, string pdm,string productName,string category, string unit,int number, double unitPrice, double costPrice)
        {
            ProductID = productID;
            PDM = pdm;            
            ProductName = productName;
            PY = ProductName.ConvertToCaptal();
            Category = category;
            Unit = unit;
            Number = number;
            UnitPrice = unitPrice;
            CostPrice = costPrice;
        }

        public Product()
        {

        }
        public static Product FromDataRow(DataRow dr)
        {
            if(dr==null)
            {
                return null;
            }
            try
            {
                Product result = new Product()
                {
                    ProductID = int.Parse(dr[0].ToString()),
                    PDM = dr[1].ToString(),
                    ProductName = dr[2].ToString(),
                    PY = dr[3].ToString(),
                    Category = dr[4].ToString(),
                    Unit = dr[5].ToString(),
                    Number = int.Parse(dr[6].ToString()),
                    UnitPrice = double.Parse(dr[7].ToString()),
                    CostPrice = double.Parse(dr[8].ToString()),

                };
                return result;
            }
            catch(Exception e)
            {
                MessageBox.Show(dr[0].ToString()+e.Message);
                return null;
            }
        }
        public override string ToString()
        {
            return ProductID + PDM + ProductName + PY + Category + Unit + Number + UnitPrice + CostPrice;
        }
        
    }
}
