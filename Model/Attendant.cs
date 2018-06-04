using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project.Model
{
    [Serializable]
    public class Attendant
    {
        public int AttendantID { get; set; } = -1;
        public string AttendantName { get; set; } = "";
        public Attendant(int id, string name)
        {
            AttendantID = id;
            AttendantName = name;
        }
        public Attendant()
        {
                
        }
        public static Attendant FromDataRow(DataRow dr)
        {
            if(dr==null)
            {
                return null;
            }
            var result = new Attendant()
            {
                AttendantID = int.Parse(dr[0].ToString()),
                AttendantName = dr[1].ToString(),
            };
            return result;
        }
    }
}
