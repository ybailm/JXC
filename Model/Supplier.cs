using Project.Extentions;
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
    public class Supplier
    {
        public int SupplierID { get; set; } = -1;
        public string SupplierName { get; set; } = "";        
        public string PY { get; set; } = "";        
        public string Address { get; set; } = "";
        public string Contact { get; set; } = "";
        public string PhoneNumber { get; set; } = "";

        public Supplier (int id,string name,string address,string contact,string phoneNumber)
        {
            SupplierID = id;
            SupplierName = name;
            Address = address;
            Contact = contact;
            PhoneNumber = phoneNumber;
        }
        public Supplier()
        {

        }
        public static Supplier FromDataRow(DataRow dr)
        {
            if(dr==null)
            {
                return null;
            }
            var result = new Supplier()
            {
                SupplierID = int.Parse(dr[0].ToString()),
                SupplierName = dr[1].ToString(),
                PY = dr[2].ToString(),
                Address = dr[3].ToString(),
                Contact = dr[4].ToString(),
                PhoneNumber = dr[5].ToString(),
            };
            return result;
        }
    }
}
