using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Common;
using Common.Helper;
using System.Text.RegularExpressions;
using System.Data;
using Model;
using System.IO;
using NeuralNetWork;
using Common.Extension;

namespace StockHelper
{
    public class Task
    {
        Proc proc = new Proc();
        OtherHelper oh = new OtherHelper();

        /// <summary>
        /// 下载股票列表
        /// </summary>
        public void DownloadStockList()
        {
            try
            {
                HttpWebRequestHelper requestHelper = new HttpWebRequestHelper();
                string getStockListUrl = DataHelper.GetConfig("getStockListUrl");
                string getPlateCode = DataHelper.GetConfig("getPlateCode");
                string htmlSource = requestHelper.GetHtmlSource(getStockListUrl, Encoding.Default);
                var stockList = GetStockList(htmlSource, getPlateCode);
                foreach (var item in stockList)
                {
                    var res = proc.insertUpdateStockList(item.Key, item.Value);
                }
            }
            catch (Exception ex)
            {
                LogHelper.WriteLog("下载股票列表失败", ex);
            }
        }

        public void DownLoadStockHistoryData(string startDate, string endDate)
        {
            try
            {
                DataTable stockList = proc.getStockList();
                var downFailList = new List<string>();
                foreach (DataRow dr in stockList.Rows)
                {
                    string code = dr["StockCode"].ToString();
                    WangYiStockApi wysa = new WangYiStockApi();
                    var data = wysa.getDataFromWangYi(code, Convert.ToDateTime(startDate), Convert.ToDateTime(endDate));
                    if (data.Count == 0)
                    {
                        downFailList.Add(code);
                        continue;
                    }
                    var dt = DataHelper.ToDataTable(data);
                    var res = proc.insertStockHistoryData(dt);
                    if (res != 1)
                    {
                        LogHelper.WriteLog(string.Format("下载股票列表数据失败，股票代码：{0}", code));
                    }
                    Console.WriteLine(string.Format("时间：{0}下载股票{1}成功", DateTime.Now.ToString(), code));
                }
                Console.WriteLine(string.Format("全量下载股票列表数据失败列表\r\n{0}", string.Join(",", downFailList)));
                LogHelper.WriteLog(string.Format("全量下载股票列表数据失败\r\n{0}", string.Join(",", downFailList)));
            }
            catch (Exception ex)
            {
                LogHelper.WriteLog("下载股票历史数据失败", ex);
            }
        }
        /// <summary>
        /// 下载指定股票代码行情数据
        /// </summary>
        /// <param name="startDate"></param>
        /// <param name="endDate"></param>
        /// <param name="StockCode"></param>
        /// 
        public void DownLoadStockHistoryData(string startDate, string endDate, string stockCode)
        {
            try
            {
                var downFailList = new List<string>();
                foreach (var StockCode in stockCode.Split(','))
                {
                    //YahooStockApi ysa = new YahooStockApi();
                    //var data = ysa.getDataFromYahoo(StockCode, Convert.ToDateTime(startDate), Convert.ToDateTime(endDate));
                    WangYiStockApi wysa = new WangYiStockApi();
                    var data = wysa.getDataFromWangYi(StockCode, Convert.ToDateTime(startDate), Convert.ToDateTime(endDate));
                    if (data.Count == 0)
                    {
                        downFailList.Add(StockCode);
                        continue;
                    }
                    var dt = DataHelper.ToDataTable(data);
                    var res = proc.insertStockHistoryData(dt);
                    if (res != 1)
                    {
                        LogHelper.WriteLog(string.Format("下载股票列表数据失败，股票代码：{0}", StockCode));
                    }
                    Console.WriteLine(string.Format("时间：{0}下载股票{1}成功", DateTime.Now.ToString(), StockCode));
                }
                Console.WriteLine(string.Format("全量下载股票列表数据失败列表\r\n{0}", string.Join(",", downFailList)));
                LogHelper.WriteLog(string.Format("全量下载股票列表数据失败\r\n{0}", string.Join(",", downFailList)));
            }
            catch (Exception ex)
            {
                LogHelper.WriteLog("下载股票历史数据失败", ex);
            }
        }

        /// <summary>
        /// 下载当日股票详情
        /// </summary>
        public void DownLoadStockData()
        {
            try
            {
                var stockList = proc.getUndownloadStockList();
                XinLangStockApi xlsa = new XinLangStockApi();
                var loop = 3;
                while (loop > 0)
                {
                    foreach (DataRow dr in stockList.Rows)
                    {
                        string code = dr["StockCode"].ToString();
                        var data = xlsa.getDataFromXinLang(code);
                        if (data.StockCode == "-1")
                        {
                            Console.WriteLine(string.Format("时间：{0}下载股票{1}失败", DateTime.Now.ToString(), code));
                            LogHelper.WriteLog(string.Format("下载股票行情数据失败，股票代码：{0}<br/>下载日期：{1}", code, data.StockHistoryDate));
                            continue;
                        }
                        var dt = DataHelper.ToDataTable(data);
                        var res = proc.insertStockHistoryData(dt, data.StockHistoryDate);
                        if (res != 1)
                        {
                            LogHelper.WriteLog(string.Format("下载股票行情数据失败，股票代码：{0}<br/>下载日期：{1}", code, data.StockHistoryDate));
                        }
                        Console.WriteLine(string.Format("时间：{0}下载股票{1}成功", DateTime.Now.ToString(), code));
                    }
                    loop--;
                    stockList = proc.getUndownloadStockList();
                }
            }
            catch (Exception ex)
            {
                LogHelper.WriteLog("下载股票行情数据失败", ex);
            }
        }

        /// <summary>
        /// 数据维护
        /// </summary>
        /// <param name="Date"></param>
        public void DataMaintain(string Date)
        {
            try
            {
                var res = proc.dataMaintain(Date);
                if (res != 1)
                {
                    LogHelper.WriteLog(string.Format("数据维护运行失败，日期：{0}", Date));
                }
                else
                {
                    Console.WriteLine(string.Format("数据维护运行成功，日期：{0}", Date));
                }
            }
            catch (Exception ex)
            {
                LogHelper.WriteLog(string.Format("数据维护运行失败，日期：{0}", Date), ex);
            }
        }

        /// <summary>
        /// 计算股票指标
        /// </summary>
        /// <param name="date">计算指标的日期，为空计算当前日期</param>
        /// <param name="stockCode">计算指标的股票，为空计算所有股票</param>
        public void ComputeStockIndex(string date = "", string stockCode = "")
        {
            try
            {
                List<StockMatchedIndex> smiLst = new List<StockMatchedIndex>();
                DateTime qDate = string.IsNullOrEmpty(date) ? DateTime.Now : Convert.ToDateTime(date);
                if (oh.IsWorkingDay(qDate))
                {
                    if (string.IsNullOrEmpty(stockCode))
                    {
                        var stockList = proc.getStockList(" and len(StockCode)=6");
                        foreach (DataRow dr in stockList.Rows)
                        {
                            string code = dr["StockCode"].ToString();
                            var rs = GetStockIndex(qDate.AddMonths(-4).ToString(("yyyy-MM-dd")), qDate.ToString("yyyy-MM-dd"), code);
                            smiLst.AddRange(rs);
                        }
                    }
                    else
                    {
                        var rs = GetStockIndex(qDate.AddMonths(-4).ToString(("yyyy-MM-dd")), qDate.ToString("yyyy-MM-dd"), stockCode);
                        smiLst.AddRange(rs);
                    }
                    DataTable dt = Utility.XHelper.FillDataTableByEntity(smiLst);
                    var t1 = DateTime.Now;
                    var i = proc.insertUpdateStockMatchedIndex(dt);
                    var t = (DateTime.Now - t1).Seconds;
                    Console.WriteLine(string.Format("时间：{0}计算股票匹配指标成功。日期：{1}", DateTime.Now.ToString(), date));
                }
            }
            catch (Exception ex)
            {
                LogHelper.WriteLog("计算股票指标出错", ex);
            }
        }

        /// <summary>
        /// 用BPNN预测股票
        /// </summary>
        public void BPNNForecastStock(DateTime date)
        {
            try
            {
                var net = JsonHelper.DeserializeJsonToObject<BPNN>(File.ReadAllText(DataHelper.GetConfig("networkPath")));
                var todayStr = date.ToString("yyyy-MM-dd");
                proc.deleteStockForeCast(todayStr);
                StockPointCompute spc = new StockPointCompute();
                DataTable stockIndexCode = proc.getStockIndexCode(todayStr);
                foreach (DataRow dr in stockIndexCode.Rows)
                {
                    string stockCode = dr[0].ToString();
                    DataTable res = proc.getStockMatchedIndex(todayStr, stockCode);
                    List<double> features = new List<double>();
                    foreach (DataRow row in res.Rows)
                    {
                        features.Add(Convert.ToDouble(row[1]));
                    }
                    double result = net.forecast(ref net, features.ToArray())[0];
                    proc.insertStockForeCast(stockCode, todayStr, result);
                }
            }
            catch (Exception ex)
            {
                LogHelper.WriteLog("预测股票失败", ex);
            }
        }

        /// <summary>
        /// 验证预测
        /// </summary>
        public void ValideForecast(string date)
        {
            proc.validForecast(date);
        }
        /// <summary>
        /// 使用指定时间区间内的指标数据训练新的bpnn网络
        /// </summary>
        /// <param name="startDate">开始时间</param>
        /// <param name="endDate">结束时间</param>
        /// <param name="sOutput">网络保存地址</param>
        public void Train(int sampleNum, int tesNum,DateTime date)
        {
            string sOutput = DataHelper.GetConfig("networkPath");
            TrainingRoom tr = new TrainingRoom();
            var samples = new List<TrainingRoom.Sample>();
            var dates = getSimilarPlate(date, 5, 0.9, 0.033, 0.033, 0.033, 10);
            if(dates==null)
            {
                return;
            }
            for(int i=0;i<dates.Count;i++)
            {
                samples.AddRange(tr.GetSample(Convert.ToDateTime(dates[i])));
            }
            var samplesTest = new List<TrainingRoom.Sample>();
            var samplesTrain = new List<TrainingRoom.Sample>();
            while (sampleNum > 0)
            {
                var index = OtherHelper.getRand(0, samples.Count - 1);
                samplesTrain.Add(samples[index]);
                samples.RemoveAt(index);
                sampleNum--;
            }
            while (tesNum > 0)
            {
                var index = OtherHelper.getRand(0, samples.Count - 1);
                samplesTest.Add(samples[index]);
                samples.RemoveAt(index);
                tesNum--;
            }
            samples.Clear();
            tr.TrainBPNN(33, new int[2] { 12, 6 }, 1, 0.05, 0.01, 0.0001, DateTime.Now.AddMinutes(10), samplesTrain, sOutput);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="date"></param>
        /// <param name="takeN"></param>
        /// <param name="closeWeight"></param>
        /// <param name="upperShadowWeight"></param>
        /// <param name="lowerShadowWeight"></param>
        /// <param name="entityWeight"></param>
        /// <param name="topN"></param>
        /// <returns></returns>
        public List<String> getSimilarPlate(DateTime date,int takeN, double closeWeight, double upperShadowWeight,double lowerShadowWeight,double entityWeight, int topN)
        {
            WangYiStockApi wysa = new WangYiStockApi();
            StockPointCompute spc = new StockPointCompute();
            var ssPlate = wysa.getDataFromWangYi("000001.ss", date.AddYears(-5), date).Where(w => w.SVolume > 0).OrderBy(o => o.StockHistoryDate);
            var szPlate = wysa.getDataFromWangYi("399001.sz", date.AddYears(-5), date).Where(w => w.SVolume > 0).OrderBy(o => o.StockHistoryDate);
            if(Convert.ToDateTime( ssPlate.ToList()[ssPlate.Count()-1].StockHistoryDate)!=date)
            {
                return null;
            }
            Dictionary<string, decimal> meanClose = new Dictionary<string, decimal>();
            Dictionary<string, decimal> meanOpen = new Dictionary<string, decimal>();
            Dictionary<string, decimal> meanHigh = new Dictionary<string, decimal>();
            Dictionary<string, decimal> meanLow = new Dictionary<string, decimal>();
            foreach (var i in ssPlate)
            {
                foreach (var j in szPlate)
                {
                    if (i.StockHistoryDate == j.StockHistoryDate)
                    {
                        meanClose.Add(i.StockHistoryDate, i.SClose + j.SClose);
                        meanOpen.Add(i.StockHistoryDate, i.SOpen + j.SOpen);
                        meanHigh.Add(i.StockHistoryDate, i.SHigh + j.SHigh);
                        meanLow.Add(i.StockHistoryDate, i.SLow + j.SLow);
                        break;
                    }
                }
            }
            var meanCloseRise = spc.GetIncrease(meanClose.Values.ToList());
            List<double> upperShadow = new List<double>();
            List<double> lowerShadow = new List<double>();
            List<double> entity = new List<double>();
            for (int i = 1; i < meanClose.Count; i++)
            {
                var low = meanLow.Values.ToList()[i];
                var high = meanHigh.Values.ToList()[i];
                var open = meanOpen.Values.ToList()[i];
                var close = meanClose.Values.ToList()[i];
                upperShadow.Add((double)(high - (open > close ? open : close)) / (double)(high - low));
                entity.Add((double)Math.Abs(close - open) / (double)(high - low));
                lowerShadow.Add((double)((open < close ? open : close) - low) / (double)(high - low));
            }
            var closeRise = meanCloseRise.Skip(meanCloseRise.Count - takeN).ToList();
            var upperSw = upperShadow.Skip(upperShadow.Count - takeN).ToList();
            var lowerSw = lowerShadow.Skip(lowerShadow.Count - takeN).ToList();
            var ey = entity.Skip(entity.Count - takeN).ToList();
            var closeRiseArray = spc.spiltArray(meanCloseRise, takeN);
            var upperShadowArray = spc.spiltArray(upperShadow, takeN);
            var lowerShadowArray = spc.spiltArray(lowerShadow, takeN);
            var entityArray = spc.spiltArray(entity, takeN);
            Dictionary<string, double> resultDic = new Dictionary<string, double>();
            for (int i = 1; i < meanClose.Count - 6; i++)
            {
                double distanceClose = DataHelper.ListDistance(closeRise, closeRiseArray[i]);
                double distanceUpperSw = DataHelper.ListDistance(upperSw, upperShadowArray[i]);
                double distanceLowerSw = DataHelper.ListDistance(lowerSw, lowerShadowArray[i]);
                double distanceEntity = DataHelper.ListDistance(ey, entityArray[i]);
                resultDic.Add(meanClose.Keys.ToList()[i], distanceClose * closeWeight + distanceEntity * entityWeight+distanceLowerSw*lowerShadowWeight+distanceUpperSw*upperShadowWeight);
            }
            return resultDic.OrderBy(o => o.Value).Take(topN).Select(s => s.Key).ToList();
        }

        #region 私有方法

        /// <summary>
        /// 从html源码中提取数据
        /// </summary>
        /// <param name="htmlSource">html源码</param>
        /// <param name="plateCode">版块：60,002</param>
        /// <returns></returns>
        private Dictionary<string, string> GetStockList(string htmlSource, string plateCode)
        {
            Dictionary<string, string> result = new Dictionary<string, string>();
            var plateCodelist = plateCode.Split(',');
            Regex reg = new Regex(@">\S*\([0-9]\d{5}\)<");
            var matches = reg.Matches(htmlSource);
            foreach (Match item in matches)
            {
                var stockName = item.Value.Substring(item.Value.IndexOf('>') + 1, item.Value.IndexOf('(') - item.Value.IndexOf('>') - 1);
                var stockCode = item.Value.Substring(item.Value.IndexOf('(') + 1, item.Value.IndexOf(')') - item.Value.IndexOf('(') - 1);
                foreach (var i in plateCode.Split(','))
                {
                    if (stockCode.IndexOf(i) == 0)
                    {
                        if (result.Where(w => w.Key == stockCode).Count() == 0)
                            result.Add(stockCode, stockName);
                        break;
                    }
                }
            }
            return result;
        }

        /// <summary>
        /// 计算股票指标
        /// </summary>
        /// <param name="startDate">开始时间</param>
        /// <param name="endDate">结束时间ps:日期跨度要满足指标分析算法需求</param>
        /// <param name="stockCode">股票代码</param>
        /// <returns></returns>
        private List<StockMatchedIndex> GetStockIndex(string startDate, string endDate, string stockCode)
        {
            List<StockMatchedIndex> result = new List<StockMatchedIndex>();
            DataTable dt = proc.getStockHistoryData(startDate, endDate, stockCode);
            List<StockHistoryData> data = Utility.XHelper.FillListByDataTable<StockHistoryData>(dt);
            #region 检测数据
            if (data == null || data.Count == 0 || data.Count < 45 || Convert.ToDateTime(data[0].StockHistoryDate) != Convert.ToDateTime(endDate))
            {
                return result;
            }
            int suspended = 0;
            foreach (var item in data)
            {
                // 成交量为零，停牌超过10个交易日
                if (item.SVolume == 0)
                {
                    suspended++;
                    if (suspended > 5)
                    {
                        result.Add(new StockMatchedIndex() { Date = Convert.ToDateTime(endDate), StockCode = stockCode, StockIndexID = SpacialShape.Suspended, StockMatchedIndexID = Guid.NewGuid() });
                        return result;
                    }
                }
                // 涨跌超过10%，事件
                else if (Math.Abs((item.SClose - item.SOpen) / item.SOpen) > 0.11M)
                {
                    result.Add(new StockMatchedIndex() { Date = Convert.ToDateTime(endDate), StockCode = stockCode, StockIndexID = SpacialShape.Event, StockMatchedIndexID = Guid.NewGuid() });
                    return result;
                }
            }
            #endregion
            data = data.Where(w => w.SVolume > 0).ToList();
            if (data.Count < 45)
            {
                return result;
            }
            #region 计算指标
            try
            {
                StockPointCompute spc = new StockPointCompute();
                result.AddRange(getStockMatchedIndex(endDate, stockCode, spc.Index_KDJ(data)));
                result.AddRange(getStockMatchedIndex(endDate, stockCode, spc.Index_LineEqual(data)));
                result.AddRange(getStockMatchedIndex(endDate, stockCode, spc.Index_LineK(data)));
                result.AddRange(getStockMatchedIndex(endDate, stockCode, spc.Index_PSY(data)));
                result.AddRange(getStockMatchedIndex(endDate, stockCode, spc.Index_VR(data)));
                result.AddRange(getStockMatchedIndex(endDate, stockCode, spc.Index_W_R(data)));
            }
            catch (Exception ex)
            {
                LogHelper.WriteLog(string.Format("计算指标失败 startDate：{0}，endDate：{1}，stockCode：{2}", startDate, endDate, stockCode), ex);
            }
            #endregion
            return result.Where(w => w.StockIndexID % 100 != 0).ToList();
        }

        /// <summary>
        /// 计算股票匹配指标
        /// </summary>
        /// <param name="endDate"></param>
        /// <param name="stockCode"></param>
        /// <param name="lst"></param>
        /// <returns></returns>
        private List<StockMatchedIndex> getStockMatchedIndex(string endDate, string stockCode, List<int> lst)
        {
            List<StockMatchedIndex> result = new List<StockMatchedIndex>();
            foreach (var item in lst)
            {
                result.Add((new StockMatchedIndex() { Date = Convert.ToDateTime(endDate), StockCode = stockCode, StockIndexID = item, StockMatchedIndexID = Guid.NewGuid() }));
            }
            return result;
        }

        #endregion
    }
}
