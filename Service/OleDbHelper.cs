using Projact.Service;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.OleDb;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Project.Service
{
    public partial class OleDbHelper : IHelper<OleDbParameter, TransactionModel<OleDbParameter>>
    {
        static OleDbConnection conn;
        public OleDbHelper()
        {
            conn = new OleDbConnection(ConnString);
        }

        public string ConnString
        {
            get
            {
                return ConfigurationManager.AppSettings["OleDbConnectionstring"];
            }
        }

        /// <summary>
        /// 执行通用的添加、修改、删除操作
        /// </summary>
        /// <param name="OleDb">Insert、Update、Delete语句</param>
        /// <param name="pars">参数数组</param>
        /// <returns>操作后受影响的行数</returns>
        public int ExecuteSqlReturnInt(string sql, OleDbParameter[] pars)
        {

            try
            {
                conn.Open();
                OleDbCommand cmd = new OleDbCommand(sql, conn);
                if (pars != null && pars.Length > 0)
                {
                    foreach (var par in pars)
                    {
                        cmd.Parameters.Add(par);
                    }
                }

                int count = cmd.ExecuteNonQuery();
                conn.Close();
                return count;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"{sql}\n{ex.ToString()}");
                conn.Close();
                return 0;
            }
        }

        /// <summary>
        /// 执行通用的添加、修改、删除操作事务
        /// </summary>
        /// <param name="trans"></param>
        public bool ExecuteTransaction(List<TransactionModel<OleDbParameter>> trans)
        {
            conn.Open();

            OleDbCommand cmd = new OleDbCommand()
            {
                Connection = conn
            };

            OleDbTransaction myTran = conn.BeginTransaction();
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
                conn.Close();
                return true;
            }
            catch (Exception ex)
            {
                myTran.Rollback();
                conn.Close();
                MessageBox.Show($"事务执行失败，所有操作失效\n{ex.ToString()}");
                return false;
            }
        }

        public DataTable GetTable(string sql, OleDbParameter[] pars)
        {
            try
            {
                conn.Open();

                OleDbDataAdapter da = new OleDbDataAdapter(sql, conn);
                if (pars != null && pars.Length > 0)
                {
                    foreach (var par in pars)
                    {
                        da.SelectCommand.Parameters.Add(par);
                    }
                }

                DataTable table = new DataTable();
                da.Fill(table);
                conn.Close();
                if (table.Rows.Count > 0)
                    return table;
                else
                    return null;
            }
            catch
            {
                conn.Close();
                return null;
            }
        }

        public DataTable GetTable(string tableName)
        {
            try
            {
                conn.Open();

                OleDbDataAdapter oda = new OleDbDataAdapter($"Select * From {tableName}", conn);
                DataTable myDataTable = new DataTable();
                oda.Fill(myDataTable);
                conn.Close();
                return myDataTable;

            }
            catch (Exception ex)
            {
                conn.Close();
                MessageBox.Show(ex.ToString());
                return null;
            }
        }
        public DataTable GetColumnMaxValue(string tableName, string columnName)
        {
            try
            {
                conn.Open();

                OleDbDataAdapter oda = new OleDbDataAdapter($"Select Max({columnName}) From {tableName}", conn);
                DataTable myDataTable = new DataTable();
                oda.Fill(myDataTable);
                conn.Close();
                return myDataTable;

            }
            catch (Exception ex)
            {
                conn.Close();
                MessageBox.Show(ex.ToString());
                return null;
            }
        }

        public DataTable GetTable(string tableName, string condition)
        {
            try
            {
                conn.Open();

                OleDbDataAdapter oda = new OleDbDataAdapter($"Select * From {tableName} Where {condition}", conn);
                DataTable myDataTable = new DataTable();
                oda.Fill(myDataTable);
                conn.Close();
                return myDataTable;
            }
            catch (Exception ex)
            {
                conn.Close();
                MessageBox.Show(ex.ToString());
                return null;
            }
        }

        public DataTable GetTableBySql(string sqlStr)
        {
            using (OleDbConnection conn = new OleDbConnection(ConnString))
            {
                try
                {
                    conn.Open();
                    OleDbDataAdapter oda = new OleDbDataAdapter(sqlStr, conn);
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

