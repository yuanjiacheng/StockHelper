using System.Text;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;
using System.Collections.Generic;
using System;

namespace Common.Helper
{
    public class SqlAccess
    {
        public static string connstr = ConfigurationManager.ConnectionStrings["conn"].ConnectionString;

        public static SqlDataReader ExecuteReader(string connString, CommandType cmdType, string cmdText, params SqlParameter[] cmdParms)
        {
            SqlCommand cmd = new SqlCommand();
            SqlConnection conn = new SqlConnection(connString);
            try
            {
                PrepareCommand(cmd, conn, null, cmdType, cmdText, cmdParms);
                SqlDataReader rdr = cmd.ExecuteReader(CommandBehavior.CloseConnection);
                cmd.Parameters.Clear();
                return rdr;
            }
            catch
            {
                conn.Close();
                throw;
            }
        }
        public static SqlDataReader ExecuteReader(string connString, CommandType cmdType, string cmdText)
        {
            return ExecuteReader(connString, cmdType, cmdText, (SqlParameter[])null);
        }

        public static object ExecuteScalar(string connString, CommandType cmdType, string cmdText, params SqlParameter[] cmdParms)
        {
            SqlCommand cmd = new SqlCommand();

            using (SqlConnection conn = new SqlConnection(connString))
            {
                PrepareCommand(cmd, conn, null, cmdType, cmdText, cmdParms);
                //执行查询，并返回查询所返回的结果集中第一行的第一列。
                object val = cmd.ExecuteScalar();
                cmd.Parameters.Clear();
                return val;
            }
        }
        public static int ExecuteNonQuery(string connString, CommandType cmdType, string cmdText, params SqlParameter[] cmdParms)
        {

            SqlCommand cmd = new SqlCommand();

            using (SqlConnection conn = new SqlConnection(connString))
            {
                if(cmdParms==null)
                {
                    cmdText= addRollBack(cmdText);
                }
                PrepareCommand(cmd, conn, null, cmdType, cmdText, cmdParms);
                int val = cmd.ExecuteNonQuery();
                //清除cmd的参数
                cmd.Parameters.Clear();
                return val;
            }
        }
        public static int ExecuteNonQuery(string connString, CommandType cmdType, string cmdText)
        { return ExecuteNonQuery(SqlAccess.connstr, CommandType.Text, cmdText, (SqlParameter[])null); }
        public static DataSet ExecuteDataset(string connectionString, CommandType commandType, string commandText, params SqlParameter[] commandParameters)
        {
            SqlConnection cn = new SqlConnection(connectionString);
            cn.Open();

            //创建一个SqlCommand对象，并对其进行初始化
            SqlCommand cmd = new SqlCommand();
            PrepareCommand(cmd, cn, (SqlTransaction)null, commandType, commandText, commandParameters);

            //创建SqlDataAdapter对象以及DataSet
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            DataSet ds = new DataSet();

            //填充ds
            da.Fill(ds);
            cn.Close();
            // 清除cmd的参数集合	
            cmd.Parameters.Clear();

            //返回ds
            return ds;
        }
        public static DataSet ExecuteDataset(string connectionString, CommandType commandType, string commandText)
        {//重载ExecuteDataset方法
            return ExecuteDataset(connectionString, commandType, commandText, (SqlParameter[])null);
        }

        private static void PrepareCommand(SqlCommand cmd, SqlConnection conn, SqlTransaction trans, CommandType cmdType, string cmdText, SqlParameter[] cmdParms)
        {
            //判断连接的状态。如果是关闭状态，则打开
            if (conn.State != ConnectionState.Open)
                conn.Open();
            //cmd属性赋值
            cmd.Connection = conn;
            cmd.CommandText = cmdText;
            //是否需要用到事务处理
            if (trans != null)
                cmd.Transaction = trans;

            cmd.CommandType = cmdType;
            //添加cmd需要的存储过程参数
            if (cmdParms != null)
            {
                foreach (SqlParameter parm in cmdParms)
                    cmd.Parameters.Add(parm);
            }
        }    //PrepareCommand 
        private static string addRollBack(string commondTxt)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendFormat(@"begin tran DATATRAN {0} IF @@ERROR <> 0 ROLLBACK TRAN DATATRAN else COMMIT TRAN DATATRAN", commondTxt);
            return sb.ToString();
        }

        /// <summary>
        /// 将实体类通过反射组装成字符串
        /// </summary>
        /// <param name="t">实体类</param>
        /// <returns>组装的字符串</returns>
        public static SqlParameter[] NewParas<T>(T t)
        {
            List<SqlParameter> para = new List<SqlParameter>();
            Type type = t.GetType();
            System.Reflection.PropertyInfo[] propertyInfos = type.GetProperties();
            for (int i = 0; i < propertyInfos.Length; i++)
            {
                para.Add(new SqlParameter("@" + propertyInfos[i].Name, propertyInfos[i].GetValue(t, null) != null ? propertyInfos[i].GetValue(t, null) : DBNull.Value));
            }
            return para.ToArray();
        }

        /// <summary>
        /// 将datatable转换成实体（实体类属性需要和数据库列名相同）
        /// </summary>
        /// <typeparam name="T">转换的实体</typeparam>
        /// <param name="table">数据表</param>
        /// <returns>返回实体</returns>
        public static T GetEntity<T>(DataTable table) where T : new()
        {
            T entity = new T();
            foreach (DataRow row in table.Rows)
            {
                foreach (var item in entity.GetType().GetProperties())
                {
                    if (row.Table.Columns.Contains(item.Name))
                    {
                        if (DBNull.Value != row[item.Name])
                        {
                            item.SetValue(entity, Convert.ChangeType(row[item.Name], item.PropertyType), null);
                        }

                    }
                }
            }

            return entity;
        }
        /// <summary>
        /// 将datatable转换成实体集合（实体类属性需要和数据库列名相同）
        /// </summary>
        /// <typeparam name="T">转换的实体类集合</typeparam>
        /// <param name="table">数据表</param>
        /// <returns>返回的实体类集合</returns>
        public static IList<T> GetEntities<T>(DataTable table) where T : new()
        {
            IList<T> entities = new List<T>();
            foreach (DataRow row in table.Rows)
            {
                T entity = new T();
                foreach (var item in entity.GetType().GetProperties())
                {
                    item.SetValue(entity, Convert.ChangeType(row[item.Name], item.PropertyType), null);
                }
                entities.Add(entity);
            }
            return entities;
        }
    }
}
