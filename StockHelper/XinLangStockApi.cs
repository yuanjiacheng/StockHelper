using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Model;
using Common.Helper;

namespace StockHelper
{
    public class XinLangStockApi
    {
        private WebClient wc = new WebClient();
        private string getRequestUrl(string UrlStr, string Code)
        {
            if (Code.Length > 6)
            {
                if (Code == "000001.ss")
                    return UrlStr + "/list=sh000001";   //指数情况下
                else
                    return UrlStr + "/list=sz399001";
            }
            else
            {
                if (Convert.ToInt32(Code) >= 600000)
                {
                    return UrlStr + "/list=sh" + Code;
                }
                else
                {
                    return UrlStr + "/list=sz" + Code;
                }
            }
        }
        public StockHistoryData getDataFromXinLang(string Code)
        {
            StockHistoryData result = new StockHistoryData();
            try
            {
                string xinLangApiUrl = DataHelper.GetConfig("getStockDataUrl");
                string request = getRequestUrl(xinLangApiUrl, Code);
                string[] data = wc.DownloadString(request).Split(',');
                result.StockCode = Code;
                result.SClose = Convert.ToDecimal(data[2]);
                result.SOpen = Convert.ToDecimal(data[3]);
                result.SHigh = Convert.ToDecimal(data[4]);
                result.SLow = Convert.ToDecimal(data[5]);
                result.SVolume = Convert.ToInt64(data[8]);
                result.StockHistoryDate = Convert.ToDateTime(data[30]).ToString("yyyy-MM-dd");
            }
            catch (Exception ex)
            {
                LogHelper.WriteLog(string.Format("下载股票数据失败<br/>股票代码：{0}<br/>错误原因：{1}", Code, ex.Message));
                result.StockCode = "-1";        //表示下载失败
            }
            return result;
        }
    }
}
