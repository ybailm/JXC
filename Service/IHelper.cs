using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project.Service
{
    public interface IHelper<T,K> where T : class where K : class
    {
        string ConnString { get;}
  
        /// <summary>
        /// 执行通用的添加、修改、删除操作
        /// </summary>
        /// <param name="sql">Insert、Update、Delete语句</param>
        /// <param name="pars">参数数组</param>
        /// <returns>操作后受影响的行数</returns>
        int ExecuteSqlReturnInt(string sql, T[] pars);
        bool ExecuteTransaction(List<K> trans);
        DataTable GetTable(string sql, T[] pars);
        DataTable GetTable(string tableName);
        DataTable GetTable(string tableName,string sqlWhere);
        DataTable GetColumnMaxValue(string tableName, string columnName);
        DataTable GetTableBySql(string sqlStr);
    }
}
