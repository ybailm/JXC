using Project.Service;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.OleDb;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project.ViewModel
{
    public static class Common
    {
        public static ICommon Instance;
        public static string DbType
        {
            get
            {
                return ConfigurationManager.AppSettings["DbType"].ToLower();
            }
        }
        static Common()
        {
            if (DbType == "access")
                Instance = new OleDbCommon();
            if (DbType == "sqlite")
                Instance = new SQLiteCommon();
        }
    }
}