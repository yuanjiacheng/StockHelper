using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Common.Helper;
using Model;

namespace StockHelper
{
    public class WangYiStockApi
    {
        private WebClient wc = new WebClient();

        /// <summary>
        /// 拼接请求字符串
        /// </summary>
        /// <param name="UrlStr">请求地址</param>
        /// <param name="Code">请求股票代码</param>
        /// <param name="StartDate">请求数据开始时间</param>
        /// <param name="EndDate">请求数据结束时间</param>
        /// <returns>请求地址</returns>
        private string getRequestUrl(string UrlStr, string Code, DateTime StartDate, DateTime EndDate)
        {

            string param = string.Format("&start={0}&end={1}&fields=TCLOSE;HIGH;LOW;TOPEN;VOTURNOVER", StartDate.ToString("yyyyMMdd"), EndDate.ToString("yyyyMMdd"));
            string Url = "";
            if (Code.Length > 6)
            {
                if (Code.ToLower() == "000001.ss")
                {
                    Url = UrlStr + "code=0000001" + param;     //指数情况下
                }
                else if (Code.ToLower() == "399001.sz")
                {
                    Url = UrlStr + "code=1399001" + param;
                }
            }
            else
            {
                if (Convert.ToInt32(Code) >= 600000)
                    Url = UrlStr + "code=0" + Code + param;
                else
                    Url = UrlStr + "code=1" + Code + param;
            }
            return Url;
        }


        /// <summary>
        /// 通过网易接口获得股票历史数据
        /// </summary>
        /// <param name="Code">股票代码</param>
        /// <param name="StartDate">数据开始时间</param>
        /// <param name="EndDate">数据结束时间</param>
        /// <returns>改股票的历史数据</returns>
        public List<StockHistoryData> getDataFromWangYi(string Code, DateTime StartDate, DateTime EndDate)
        {

            List<StockHistoryData> result = new List<StockHistoryData>();
            try
            {
                string WangYiApiUrl = DataHelper.GetConfig("getWangYiStockHistoryDataUrl");
                string request = getRequestUrl(WangYiApiUrl, Code, StartDate, EndDate);
                string data = wc.DownloadString(request);
                string[] dataline = DataHelper.Remove(data.Split(new string[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries), 0);
                foreach (var item in dataline)
                {
                    string[] datarow = item.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);
                    StockHistoryData shd = new StockHistoryData();
                    shd.StockCode = Code;
                    shd.StockHistoryDate = Convert.ToDateTime(datarow[0]).ToString("yyyy-MM-dd");
                    shd.SOpen = decimal.Round(Convert.ToDecimal(datarow[6]), 2);
                    shd.SHigh = decimal.Round(Convert.ToDecimal(datarow[4]), 2);
                    shd.SLow = decimal.Round(Convert.ToDecimal(datarow[5]), 2);
                    shd.SClose = decimal.Round(Convert.ToDecimal(datarow[3]), 2);
                    shd.SVolume = Convert.ToInt64(datarow[7]);
                    result.Add(shd);
                }
            }
            catch (Exception ex)
            {
                LogHelper.WriteLog(string.Format("下载股票历史数据失败<br/>股票代码：{0}<br/>下载数据开始时间：{1}<br/>下载数据结束时间：{2}<br/>错误原因：{3}", Code, StartDate, EndDate, ex.Message));
            }
            return result;
        }
    }
}
