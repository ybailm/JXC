using Projact.Service;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Project.Service
{

    public class SqlHelper : IHelper<SqlParameter, TransactionModel<SqlParameter>>
    {
        public string ConnString
        {
            get
            {
                return ConfigurationManager.AppSettings["SqlConnectionstring"];
            }
        }
        /// <summary>
        /// 执行通用的添加、修改、删除操作
        /// </summary>
        /// <param name="sql">Insert、Update、Delete语句</param>
        /// <param name="pars">参数数组</param>
        /// <returns>操作后受影响的行数</returns>
        public int ExecuteSqlReturnInt(string sql, SqlParameter[] pars)
        {
            using (SqlConnection conn = new SqlConnection(ConnString))
            {
                try
                {
                    conn.Open();
                    using (SqlCommand cmd = new SqlCommand(sql, conn))
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
        public bool ExecuteTransaction(List<TransactionModel<SqlParameter>> trans)
        {
            using (SqlConnection conn = new SqlConnection(ConnString))
            {
                conn.Open();
                using (SqlCommand cmd = new SqlCommand())
                {
                    cmd.Connection = conn;
                    SqlTransaction myTran = conn.BeginTransaction();
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

        public DataTable GetTable(string sql, SqlParameter[] pars)
        {
            using (SqlConnection conn = new SqlConnection(ConnString))
            {
                using (SqlDataAdapter da = new SqlDataAdapter(sql, conn))
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
            using (SqlConnection conn = new SqlConnection(ConnString))
            {
                try
                {
                    conn.Open();
                    SqlDataAdapter oda = new SqlDataAdapter("Select * From " + tableName, conn);
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
            using (SqlConnection conn = new SqlConnection(ConnString))
            {
                try
                {
                    if (conn.State == ConnectionState.Closed)
                        conn.Open();
                    SqlDataAdapter oda = new SqlDataAdapter($"Select Max({columnName}) From {tableName}", conn);
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
            using (SqlConnection conn = new SqlConnection(ConnString))
            {
                try
                {
                    conn.Open();
                    SqlDataAdapter oda = new SqlDataAdapter("Select * From " + tableName + " Where " + condition, conn);
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
            using (SqlConnection conn = new SqlConnection(ConnString))
            {
                try
                {
                    conn.Open();
                    SqlDataAdapter oda = new SqlDataAdapter(sqlStr, conn);
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
}