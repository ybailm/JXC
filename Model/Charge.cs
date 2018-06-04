using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Project.Extentions;
using System.Data;

namespace Project.Model
{
    [Serializable]
    public class Charge
    {
        public int ChargeID { get; set; } = -1;
        public string ChargeName { get; set; } = "";
        public string Unit { get; set; } = "次";
        public int Number { get; set; } = 1;
        public double UnitPrice { get; set; } = 0;
        public double Price { get=>Number*UnitPrice; }
        public DateTime PayDate { get; set; } = DateTime.Now;
        public string Category { get; set; } = "";
        public string AttendantName { get; set; } = "";

        public Charge(int id, string chargeName,string unit,double unitPrice,int number,DateTime payDate, string category,string attendantName  )
        {
            ChargeID = id;
            ChargeName = chargeName;
            Unit = unit;          
            UnitPrice = unitPrice;
            Number = number;
            PayDate = payDate;
            Category = category;
            AttendantName = attendantName;
        }

        public Charge()
        {

        }
        public static Charge FromDataRow(DataRow dr)
        {
            if (dr == null)
            {
                return null;
            }

            var result = new Charge()
            {
                ChargeID = int.Parse(dr[0].ToString()),
                PayDate = DateTime.Parse(dr[1] != null ? dr[1].ToString() : ""),
                ChargeName = dr[2] != null ? dr[2].ToString() : "",
                Category = dr[3] != null ? dr[3].ToString() : "",
                Unit = dr[4] != null ? dr[4].ToString() : "",
                Number = int.Parse(dr[5].ToString().IsNullOrEmptyOrWhiteSpace() ? "0" : dr[5].ToString()),
                UnitPrice = double.Parse(dr[6].ToString().IsNullOrEmptyOrWhiteSpace() ? "0" : dr[6].ToString()),
                AttendantName = dr[7].ToString().IsNullOrEmptyOrWhiteSpace() ? "" : dr[7].ToString(),
            };
            return result;
        }
    }
}
