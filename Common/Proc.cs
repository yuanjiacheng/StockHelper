using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Common.Helper;
using System.Data;
using System.Data.SqlClient;
using Model;

namespace Common
{
    public class Proc
    {
        /// <summary>
        /// 插入更新股票列表（不重复）
        /// </summary>
        /// <param name="StockCode">股票代码</param>
        /// <param name="StockName">股票名称</param>
        /// <returns>1：成功，-201：失败</returns>
        public int insertUpdateStockList(string StockCode, String StockName)
        {
            SqlParameter[] para ={
                                    new SqlParameter("@StockCode",StockCode),
                                    new SqlParameter("@StockName",StockName)
                                };
            return SqlAccess.ExecuteNonQuery(SqlAccess.connstr, CommandType.StoredProcedure, "up_InsertUpdateStockList", para);
        }

        /// <summary>
        /// 插入股票历史数据
        /// </summary>
        /// <param name="dt">数据内容</param>
        /// <returns>1：成功，-201：失败</returns>
        public int insertStockHistoryData(DataTable dt)
        {
            SqlParameter param = new SqlParameter("@StockHistoryData", SqlDbType.Structured);//这个类型很关键
            param.Value = dt;
            SqlParameter[] para = { param };
            return (int)SqlAccess.ExecuteScalar(SqlAccess.connstr, CommandType.StoredProcedure, "up_InsertStockHistoryData", para);
        }

        /// <summary>
        /// 插入股票历史数据(日常）
        /// </summary>
        /// <param name="dt">数据内容</param>
        /// <param name="date">下载日期</param>
        /// <returns>1：成功，-201：失败</returns>
        public int insertStockHistoryData(DataTable dt, string date)
        {
            SqlParameter param = new SqlParameter("@StockHistoryData", SqlDbType.Structured);//这个类型很关键
            param.Value = dt;
            SqlParameter[] para = { param, new SqlParameter("@DownDate", date) };
            return (int)SqlAccess.ExecuteScalar(SqlAccess.connstr, CommandType.StoredProcedure, "up_InsertStockHistoryData", para);

        }

        /// <summary>
        /// 插入股票匹配指标
        /// </summary>
        /// <param name="dt"></param>
        /// <returns></returns>
        public int insertUpdateStockMatchedIndex(DataTable dt)
        {
            SqlParameter param = new SqlParameter("@StockMatchedIndex", SqlDbType.Structured);
            param.Value = dt;
            SqlParameter[] para = { param };
            return (int)SqlAccess.ExecuteScalar(SqlAccess.connstr, CommandType.StoredProcedure, "up_insertStockMatchedIndex", para);
        }

        /// <summary>
        /// 查询股票列表
        /// </summary>
        /// <param name="condition"></param>
        /// <returns></returns>
        public DataTable getStockList(string condition = "")
        {
            string commandStr = "select * from StockList where 1=1 " + condition;
            return SqlAccess.ExecuteDataset(SqlAccess.connstr, CommandType.Text, commandStr, null).Tables[0];
        }

        /// <summary>
        /// 获得当天未下载成功的股票代码数据
        /// </summary>
        /// <returns></returns>
        public DataTable getUndownloadStockList()
        {
            string commandStr = @"select * from StockList as sl left outer join
                                    (
                                       select * from DownDataStatus as dds where dds.DownDate=CONVERT(date, GETDATE())
                                    ) 
                                    as r on sl.StockCode=r.StockCode where ISNULL(r.[Status],0)=0";
            return SqlAccess.ExecuteDataset(SqlAccess.connstr, CommandType.Text, commandStr, null).Tables[0];
        }

        /// <summary>
        /// 获得股票行情数据
        /// </summary>
        /// <param name="startDate">开始日期</param>
        /// <param name="endDate">结束日期</param>
        /// <param name="stockCode">股票代码</param>
        /// <returns></returns>
        public DataTable getStockHistoryData(string startDate, string endDate, string stockCode, bool ignoreHalt = false,int top=0)
        {
            string commandStr = string.Format("select * from StockHistoryData where StockHistoryDate between '{0}' and '{1}' and StockCode='{2}'", startDate, endDate, stockCode);
            if (top > 0)
            {
                commandStr= string.Format("select top {0} * from StockHistoryData where StockHistoryDate between '{1}' and '{2}' and StockCode='{3}'",top, startDate, endDate, stockCode);
            }
            if (ignoreHalt)
            {
                commandStr += " and SVolume!=0";
            }
            return SqlAccess.ExecuteDataset(SqlAccess.connstr, CommandType.Text, commandStr, null).Tables[0];
        }

        /// <summary>
        /// 获得指定日期下得样本数据
        /// </summary>
        /// <param name="date"></param>
        /// <returns></returns>
        public DataSet getSample(DateTime date)
        {
            SqlParameter[] para = { new SqlParameter("@Date", date.ToString("yyyy-MM-dd")) };
            return SqlAccess.ExecuteDataset(SqlAccess.connstr, CommandType.StoredProcedure, "up_GetSample", para);
        }

        /// <summary>
        /// 获得股票指标表数据
        /// </summary>
        /// <param name="condition">附加条件</param>
        /// <returns></returns>
        public DataTable getStockIndex(string condition = "")
        {
            string commandStr = "select * from StockIndex where 1=1 " + condition;
            return SqlAccess.ExecuteDataset(SqlAccess.connstr, CommandType.Text, commandStr, null).Tables[0];
        }

        /// <summary>
        /// 获取股票预测结果
        /// </summary>
        /// <param name="condition"></param>
        /// <returns></returns>
        public DataTable getForecast(string condition = "", int top = 0)
        {
            string commandStr = "select * from StockForecast where 1=1 " + condition;
            if (top > 0)
            {
                commandStr = "select top " + top + " * from StockForecast where 1=1 " + condition;
            }
            return SqlAccess.ExecuteDataset(SqlAccess.connstr, CommandType.Text, commandStr).Tables[0];
        }

        /// <summary>
        /// 删除无效的股票数据
        /// </summary>
        /// <param name="date"></param>
        /// <returns></returns>
        public int dataMaintain(string date)
        {
            SqlParameter[] para = { new SqlParameter("@Date", date) };
            return (int)SqlAccess.ExecuteScalar(SqlAccess.connstr, CommandType.StoredProcedure, "up_DataMaintain", para);
        }

        /// <summary>
        /// 验证预测数据结果
        /// </summary>
        /// <param name="date"></param>
        /// <returns></returns>
        public void validForecast(string date)
        {
            SqlParameter[] para = { new SqlParameter("@Date", date) };
            SqlAccess.ExecuteScalar(SqlAccess.connstr, CommandType.StoredProcedure, "up_ValidForecast", para);
        }

        /// <summary>
        /// 获得指定日期满足预测条件的股票
        /// </summary>
        /// <param name="Date"></param>
        /// <returns></returns>
        public DataTable getStockIndexCode(string Date)
        {
            string commandStr = "select DISTINCT StockCode from StockMatchedIndex where StockIndexID>100 group by StockCode,[Date] having [Date]='" + Date + "' and count(1)>0";
            return SqlAccess.ExecuteDataset(SqlAccess.connstr, CommandType.Text, commandStr, null).Tables[0];
        }

        /// <summary>
        /// 获得股票匹配指标数据
        /// </summary>
        /// <param name="Date"></param>
        /// <param name="StockCode"></param>
        /// <returns></returns>
        public DataTable getStockMatchedIndex(string Date, string StockCode)
        {
            string commandStr = @"select si.StockIndexID,matched=case when isnull(smi.StockIndexID,'')='' then 0 else 1 end from StockIndex as si left outer join StockMatchedIndex as smi 
            on smi.StockIndexID=si.StockIndexID and smi.[Date]='" + Date + "' and smi.StockCode='" + StockCode + "' where si.StockIndexID>100 order by si.StockIndexID asc  ";
            return SqlAccess.ExecuteDataset(SqlAccess.connstr, CommandType.Text, commandStr, null).Tables[0];
        }

        /// <summary>
        /// 插入预测数据
        /// </summary>
        /// <param name="stockCode"></param>
        /// <param name="date"></param>
        /// <param name="foreCast"></param>
        /// <param name="fact"></param>
        /// <returns></returns>
        public int insertStockForeCast(string stockCode, string date, double foreCast, double fact = 0)
        {
            string commandStr = string.Format(@"insert into StockForecast (StockCode,[Date],Forecast,Fact) values('{0}','{1}',{2},{3})", stockCode, date, foreCast, fact);
            return SqlAccess.ExecuteNonQuery(SqlAccess.connstr, CommandType.Text, commandStr, null);
        }
        /// <summary>
        /// 删除指定日期的股票预测数据
        /// </summary>
        /// <param name="date"></param>
        /// <returns></returns>
        public int deleteStockForeCast(string date)
        {
            string commandStr = string.Format(@"delete from StockForecast where [Date]='{0}'", date);
            return SqlAccess.ExecuteNonQuery(SqlAccess.connstr, CommandType.Text, commandStr, null);
        }
        

    }
}
