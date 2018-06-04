using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project.Model
{
    [Serializable]
    public class Manufacturer
    {
        public int ManufacturerID { get; set; }
        public string ManufacturerName { get; set; }
        public static Manufacturer FromDataRow(DataRow dr)
        {
            if (dr == null)
            {
                return null;
            }
            var result = new Manufacturer()
            {
                ManufacturerID = int.Parse(dr[0].ToString()),
                ManufacturerName = dr[1].ToString(),
            };
            return result;
        }
    }
}
