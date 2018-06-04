using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project.Model
{
    [Serializable]
    public class Company
    {
        /// <summary>
        /// 公司名称
        /// </summary>
        public string CompanyName { get; set; }

        /// <summary>
        /// 公司地址
        /// </summary>
        public string Address { get; set; }

        /// <summary>
        /// 公司电话
        /// </summary>
        public string PhoneNumber { get; set; }


        /// <summary>
        /// 公司传真
        /// </summary>
        public string Fax { get; set; }
    }
}
