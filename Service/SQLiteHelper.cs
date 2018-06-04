using Projact.Service;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SQLite;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Project.Service
{

    public partial class SQLiteHelper : IHelper<SQLiteParameter, TransactionModel<SQLiteParameter>>
    {
        public string ConnString
        {
            get
            {
                return ConfigurationManager.AppSettings["SQLiteConnectionstring"];
            }
        }

        

        /// <summary>
        /// 执行通用的添加、修改、删除操作
        /// </summary>
        /// <param name="sql">Insert、Update、Delete语句</param>
        /// <param name="pars">参数数组</param>
        /// <returns>操作后受影响的行数</returns>
        public int ExecuteSqlReturnInt(string sql, SQLiteParameter[] pars)
        {
            using (SQLiteConnection conn = new SQLiteConnection(ConnString))
            {
                try
                {
                    conn.Open();
                    using (SQLiteCommand cmd = new SQLiteCommand(sql, conn))
                    {
                        if (pars != null && pars.Length > 0)
                        {
                            foreach (var par in pars)
                            {
                                cmd.Parameters.Add(par);
                            }
                        }
                        int count = cmd.ExecuteNonQuery();
                        return count;
                    }
                }
                catch
                {
                    return 0;
                }
                finally
                {
                    conn.Close();
                }
            }
        }

        /// <summary>
        /// 执行通用的添加、修改、删除操作事务
        /// </summary>
        /// <param name="trans"></param>
        public bool ExecuteTransaction(List<TransactionModel<SQLiteParameter>> trans)
        {
            using (SQLiteConnection conn = new SQLiteConnection(ConnString))
            {
                conn.Open();
                using (SQLiteCommand cmd = new SQLiteCommand())
                {
                    cmd.Connection = conn;
                    SQLiteTransaction myTran = conn.BeginTransaction();
                    cmd.Transaction = myTran;
                    try
                    {
                        foreach (var item in trans)
                        {
                            cmd.Parameters.Clear();
                            cmd.CommandText = item.SQL;

                            if (item.Pars != null && item.Pars.Length > 0)
                            {
                                foreach (var par in item.Pars)
                                {
                                    cmd.Parameters.Add(par);
                                }
                            }
                            cmd.ExecuteNonQuery();
                        }
                        myTran.Commit();
                        return true;
                    }
                    catch (Exception ex)
                    {
                        myTran.Rollback();
                        MessageBox.Show("事务执行失败，所有操作失效\n" + ex.ToString());
                        return false;
                    }
                    finally
                    {
                        conn.Close();
                    }
                }
            }
        }

        public DataTable GetTable(string sql, SQLiteParameter[] pars)
        {
            using (SQLiteConnection conn = new SQLiteConnection(ConnString))
            {
                using (SQLiteDataAdapter da = new SQLiteDataAdapter(sql, conn))
                {
                    if (pars != null && pars.Length > 0)
                    {
                        foreach (var par in pars)
                        {
                            da.SelectCommand.Parameters.Add(par);
                        }
                    }

                    DataTable table = new DataTable();
                    da.Fill(table);
                    if (table.Rows.Count > 0)
                        return table;
                    else
                        return null;
                }
            }
        }
        public DataTable GetTable(string tableName)
        {
            using (SQLiteConnection conn = new SQLiteConnection(ConnString))
            {
                try
                {
                    conn.Open();
                    SQLiteDataAdapter oda = new SQLiteDataAdapter("Select * From " + tableName, conn);
                    DataTable myDataTable = new DataTable();
                    oda.Fill(myDataTable);
                    conn.Close();

                    return myDataTable;

                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.ToString());
                    return null;
                }
                finally
                {
                    conn.Close();
                }
            }
        }

        public DataTable GetColumnMaxValue(string tableName, string columnName)
        {
            using (SQLiteConnection conn = new SQLiteConnection(ConnString))
            {
                try
                {
                    if (conn.State == ConnectionState.Closed)
                        conn.Open();
                    SQLiteDataAdapter oda = new SQLiteDataAdapter($"Select Max({columnName}) From {tableName}", conn);
                    DataTable myDataTable = new DataTable();
                    oda.Fill(myDataTable);
                    conn.Close();
                    return myDataTable;

                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.ToString());
                    return null;
                }
                finally
                {
                    conn.Close();
                }
            }
        }

        public DataTable GetTable(string tableName, string condition)
        {
            using (SQLiteConnection conn = new SQLiteConnection(ConnString))
            {
                try
                {
                    conn.Open();
                    SQLiteDataAdapter oda = new SQLiteDataAdapter("Select * From " + tableName + " Where " + condition, conn);
                    DataTable myDataTable = new DataTable();
                    oda.Fill(myDataTable);
                    conn.Close();

                    return myDataTable;

                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.ToString());
                    return null;
                }
                finally
                {
                    conn.Close();
                }
            }
        }
        public DataTable GetTableBySql(string sqlStr)
        {
            using (SQLiteConnection conn = new SQLiteConnection(ConnString))
            {
                try
                {
                    conn.Open();
                    SQLiteDataAdapter oda = new SQLiteDataAdapter(sqlStr, conn);
                    DataTable myDataTable = new DataTable();
                    oda.Fill(myDataTable);
                    conn.Close();

                    return myDataTable;

                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.ToString());
                    return null;
                }
                finally
                {
                    conn.Close();
                }
            }
        }
    }
    public partial class SQLiteHelper : IDisposable
    {

        private bool disposed = false;

        /// <summary>
        /// 实现IDisposable中的Dispose方法
        /// </summary>
        public void Dispose()
        {
            //必须为true
            Dispose(true);
            //通知垃圾回收机制不再调用终结器（析构器）
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// 必须，以备程序员忘记了显式调用Dispose方法
        /// </summary>
        ~SQLiteHelper()
        {
            //必须为false
            Dispose(false);
        }

        /// <summary>
        /// 非密封类修饰用protected virtual
        /// 密封类修饰用private
        /// </summary>
        /// <param name="disposing"></param>
        protected virtual void Dispose(bool disposing)
        {
            if (disposed)
            {
                return;
            }
            if (disposing)
            {
                // 清理托管资源                
            }
            // 清理非托管资源
            //if (conn != null)
            //{
            //    conn.Close();
            //    conn = null;
            //}
            //让类型知道自己已经被释放
            disposed = true;
        }
    }
}