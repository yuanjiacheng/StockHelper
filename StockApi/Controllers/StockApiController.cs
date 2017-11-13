using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Model;
using BLL;
using Common;
using Common.Helper;

namespace StockApi.Controllers
{
    public class StockApiController : ApiController
    {
        GetData gd = new GetData();
        /// <summary>
        /// 获得股票行情信息
        /// </summary>
        /// <param name="sDate">开始时间</param>
        /// <param name="eDate">结束时间</param>
        /// <param name="stockCode">股票代码</param>
        /// <returns>行情数据</returns>
        [HttpGet]
        public List<StockHistoryData> getStockHistoryData(string sDate, string eDate, string stockCode)
        {
            return gd.getStockHistoryData(sDate, eDate, stockCode);
        }
        /// <summary>
        /// 返回用户预测建议
        /// </summary>
        /// <param name="stockCode"></param>
        /// <param name="date"></param>
        /// <returns></returns>
        [HttpGet]
        public HttpResponseMessage getStockForecast(string stockCode, string date)
        {
            DateTime dt = new DateTime();
            Int32 sc = new Int32();
            if (Int32.TryParse(stockCode, out sc) && DateTime.TryParse(date, out dt))
            {
                var res = gd.getStockForecast(stockCode, date);
                return JsonHelper.toJson(res);
            }
            return JsonHelper.toJson("");
        }
        [HttpGet]
        public HttpResponseMessage getStockForecast(string date)
        {
            DateTime dt = new DateTime();
            if (DateTime.TryParse(date, out dt))
            {
                var res = gd.getStockForecasts(date);
                var plateRise = gd.getPlateRise(date);
                res.Add(new StockForecast() { StockCode = "plate", Fact = plateRise });
                return JsonHelper.toJson(res);
            }
            return JsonHelper.toJson("");
        }
        public HttpResponseMessage getTopStockForecast(string date)
        {
            DateTime dt = new DateTime();
            if (DateTime.TryParse(date, out dt))
            {
                var res = gd.getTopStockForecast(date, 50);
                return JsonHelper.toJson(res);
            }
            return JsonHelper.toJson("");
        }

        /// <summary>
        /// 开始游戏
        /// </summary>
        /// <returns>游戏所需行情数据</returns>
        //[HttpGet]
        //public List<StockHistoryData> GameStart()
        //{
        //    //
        //}
    }
}
