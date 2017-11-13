using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Model;
using Common.Helper;
using System.Data;
using Common;

namespace BLL
{
    public class GetData
    {
        Proc proc = new Proc();
        /// <summary>
        /// 获得股票历史数据
        /// </summary>
        /// <param name="sDate"></param>
        /// <param name="eDate"></param>
        /// <param name="stockCode"></param>
        /// <param name="ignoreHalt"></param>
        /// <returns></returns>
        public List<StockHistoryData> getStockHistoryData(string sDate, string eDate, string stockCode, bool ignoreHalt = false)
        {
            DataTable dt = proc.getStockHistoryData(sDate, eDate, stockCode, ignoreHalt);
            List<StockHistoryData> result = Utility.XHelper.FillListByDataTable<StockHistoryData>(dt);
            return result;
        }
        public double getPlateRise(string Date)
        {
            DataTable dt = proc.getStockHistoryData(Date, Convert.ToDateTime(Date).AddDays(15).ToString("yyyy-MM-dd"), "000001.ss", true);
            if (dt.Rows.Count < 6)
            {
                return 0;
            }
            double ssClose5 = Convert.ToDouble(dt.Rows[dt.Rows.Count - 6][4]);
            double ssClose0 = Convert.ToDouble(dt.Rows[dt.Rows.Count - 1][4]);
            double ssRise = (ssClose5 - ssClose0) / ssClose0;
            dt = proc.getStockHistoryData(Date, Convert.ToDateTime(Date).AddDays(15).ToString("yyyy-MM-dd"), "399001.sz", true);
            if (dt.Rows.Count < 6)
            {
                return 0;
            }

            double szClose5 = Convert.ToDouble(dt.Rows[dt.Rows.Count - 6][4]);
            double szClose0 = Convert.ToDouble(dt.Rows[dt.Rows.Count - 1][4]);
            double szRise = (szClose5 - szClose0) / szClose0;
            return (ssRise + szRise) / 2.0;
        }
        public StockForecast getStockForecast(string Date, string StockCode)
        {
            StockForecast result = new StockForecast();
            try
            {
                //今天的数据可能没有，那就获取昨天的
                string condition = string.Format(" and [Date]<='{0}' and StockCode='{1}' and Fact!=0 order by [Date] desc", Date, StockCode);
                result = getStockForecast(condition, 1).FirstOrDefault();

            }
            catch (Exception ex)
            {
                LogHelper.WriteLog("getStockForecast失败", ex);
                result = null;
            }
            return result;
        }
        public List<StockForecast> getTopStockForecast(string Date, int top)
        {
            List<StockForecast> result = new List<StockForecast>();
            try
            {
                //今天的数据可能没有，那就获取昨天的
                string condition = string.Format(" and [Date]='{0}' order by [Forecast] desc", Date);
                result = getStockForecast(condition, top);

            }
            catch (Exception ex)
            {
                LogHelper.WriteLog("getStockForecast失败", ex);
                result = null;
            }
            return result;
        }
        public List<StockForecast> getStockForecasts(string Date)
        {
            List<StockForecast> result = new List<StockForecast>();
            try
            {
                string condition = string.Format(" and [Date]='{0}'", Date);
                result = getStockForecast(condition, 0);

            }
            catch (Exception ex)
            {
                LogHelper.WriteLog("getStockForecast失败", ex);
                result = null;
            }
            return result;
        }
        #region 私有方法
        /// <summary>
        /// 获得预测/验证数据
        /// </summary>
        /// <param name="condition"></param>
        /// <returns></returns>
        private List<StockForecast> getStockForecast(string condition = "", int top = 0)
        {
            DataTable dt = proc.getForecast(condition, top);
            List<StockForecast> result = Utility.XHelper.FillListByDataTable<StockForecast>(dt);
            return result;
        }
        #endregion

    }
}
