using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project.Model
{
    [Serializable]
    public class Service
    {
        public int ServiceID { get; set; } = -1;
        public string Category { get; set; } = "";
        public string ServiceName { get; set; } = "";
        public double UnitPrice { get; set; } = 0;
        public string Note { get; set; } = "";
        public Service(int serviceID, string category, string serviceName, double unitPrice)
        {
            ServiceID = serviceID;
            Category = category;
            ServiceName = serviceName;
            UnitPrice = unitPrice;
        }
        public Service()
        {

        }
        public static Service FromDataRow(DataRow dr)
        {
            if (dr == null)
            {
                return null;
            }
            var result = new Model.Service()
            {
                ServiceID = int.Parse(dr[0].ToString()),
                Category = dr[1].ToString(),
                ServiceName = dr[2].ToString(),
                UnitPrice = double.Parse(dr[3].ToString()),
            };
            return result;
        }
    }
}
