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
    public class Customer
    {
        public int CustomerID { get; set; } = -1;
        public string CustomerName { get; set; } = "";
        public string PY { get; set; } = "";
        public string Address { get; set; } = "";
        public string Contact { get; set; } = "";
        public string PhoneNumber { get; set; } = "";

        public override string ToString()
        {
            return CustomerName;
        }
        public Customer()
        {

        }
        public static Customer FromDataRow(DataRow dr)
        {
            if (dr == null)
                return null;
            var result = new Customer()
            {
                CustomerID = int.Parse(dr[0].ToString()),
                CustomerName = dr[1].ToString(),
                PY = dr[2].ToString(),
                Address = dr[3].ToString(),
                Contact = dr[4].ToString(),
                PhoneNumber = dr[5].ToString(),
            };
            return result;
        }
        public Customer (int id,string name,string py,string address,string contact,string phoneNumber)
        {
            CustomerID = id;
            CustomerName = name;
            PY = py;
            Address = address;
            Contact = contact;
            PhoneNumber = phoneNumber;        
        }        
    }
}
