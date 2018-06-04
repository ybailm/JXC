using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Project.Service
{
    public class SqlServerDataBaseControl:IDisposable
    {


        /// <summary> 
        /// 实例化一个数据库连接对象 
        /// </summary> 
        private SqlConnection conn;

        /// <summary> 
        /// 实例化一个新的数据库操作对象Comm 
        /// </summary> 
        private SqlCommand comm;

        /// <summary> 
        /// 要操作的数据库名称 
        /// </summary> 
        /// <summary> 
        /// 数据库连接字符串 
        /// </summary> 
        private string connectionString;
        public string ConnectionString
        {
            get { return connectionString; }
            set { connectionString = value; }
        }

        /// <summary> 
        /// SQL操作语句/存储过程 
        /// </summary> 
        private string strSQL;
        public string StrSQL
        {
            get { return strSQL; }
            set { strSQL = value; }
        }

        /// <summary> 
        /// 要操作的数据库名称 
        /// </summary> 
        private string dataBaseName;
        public string DataBaseName
        {
            get { return dataBaseName; }
            set { dataBaseName = value; }
        }

        /// <summary> 
        /// 数据库文件完整地址 
        /// </summary> 
        private string dataBase_MDF;
        public string DataBase_MDF
        {
            get { return dataBase_MDF; }
            set { dataBase_MDF = value; }
        }

        /// <summary> 
        /// 数据库日志文件完整地址 
        /// </summary> 
        private string dataBase_LDF;
        public string DataBase_LDF
        {
            get { return dataBase_LDF; }
            set { dataBase_LDF = value; }
        }

        ///<summary>
        ///附加数据库
        ///</summary>
        public void AttachDB()
        {
            try
            {
                conn = new SqlConnection(connectionString);
                conn.Open();
                comm = new SqlCommand
                {
                    Connection = conn,
                    CommandText = "sp_attach_db"//系统数据库master 中的一个附加数据库存储过程。
                };

                comm.Parameters.Add(new SqlParameter(@"dbname", SqlDbType.NVarChar));
                comm.Parameters[@"dbname"].Value = dataBaseName;
                comm.Parameters.Add(new SqlParameter(@"filename1", SqlDbType.NVarChar));  //一个主文件mdf，一个或者多个日志文件ldf，或次要文件ndf
                comm.Parameters[@"filename1"].Value = dataBase_MDF;
                comm.Parameters.Add(new SqlParameter(@"filename2", SqlDbType.NVarChar));
                comm.Parameters[@"filename2"].Value = dataBase_LDF;

                comm.CommandType = CommandType.StoredProcedure;
                comm.ExecuteNonQuery();

                System.Windows.MessageBox.Show("附加数据库成功", "提示");
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show(ex.Message, "提示");
            }
            finally
            {
                conn.Close();
            }
        }

        /// <summary> 
        /// 分离数据库 
        /// </summary> 
        public void DetachDB()
        {
            try
            {
                conn = new SqlConnection(connectionString);
                conn.Open();
                comm = new SqlCommand
                {
                    Connection = conn,
                    CommandText = @"sp_detach_db"//系统数据库master 中的一个分离数据库存储过程。
                };

                comm.Parameters.Add(new SqlParameter(@"dbname", SqlDbType.NVarChar));
                comm.Parameters[@"dbname"].Value = dataBaseName;

                comm.CommandType = CommandType.StoredProcedure;
                comm.ExecuteNonQuery();

                MessageBox.Show("分离数据库成功", "信息提示");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "信息提示");
            }
            finally
            {
                conn.Close();
            }
        }

        public void Dispose()
        {
            this.conn.Close();
            this.conn.Dispose();
            this.comm.Dispose();
        }



        //private void attachDB()
        //{
        //    SqlServerDataBaseControl DBC = new SqlServerDataBaseControl();
        //    DBC.ConnectionString = "Data Source=.; Initial Catalog=master;Integrated Security=SSPI";
        //    DBC.DataBaseName = "dbHotelSystem";
        //    DBC.DataBase_MDF = Directory.GetParent(@"Hotel.MDF") + "\\dbHotelSystem.MDF";
        //    DBC.DataBase_LDF = Directory.GetParent(@"Hotel.MDF") + "\\dbHotelSystem_log.ldf";
        //    DBC.AttachDB();

        //}

        //public void detachDB()
        //{
        //    SqlServerDataBaseControl DBC = new SqlServerDataBaseControl();
        //    DBC.ConnectionString = "Data Source=.; Initial Catalog=master;Integrated Security=SSPI";
        //    DBC.DataBaseName = "dbHotelSystem";
        //    DBC.DetachDB();
        //}
    }
}