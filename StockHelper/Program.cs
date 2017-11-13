using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StockHelper
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args.Length == 0)
            {
                #region 提示
                Console.WriteLine("001:下载股票列表");
                Console.WriteLine("002:全量下载股票数据");
                Console.WriteLine("003:增量下载股票数据");
                Console.WriteLine("004:计算股票技术面(日常)");
                Console.WriteLine("005:计算股票技术面(全量)");
                Console.WriteLine("006:下载指定股票代码行情数据,ps:不对数据做重复检测 要下载的股票代码");
                Console.WriteLine("007:数据维护（日常）");
                Console.WriteLine("008:数据维护（全量）");
                Console.WriteLine("009:预测股票数据（指定日期内）");
                Console.WriteLine("010:预测/回测股票数据");
                Console.WriteLine("011:预测股票数据(当天)");
                Console.ReadKey();
                #endregion
            }
            else
            {
                #region 计算
                Task t = new Task();
                var arg = args[0];
                DateTime date = DateTime.Now;
                switch (arg)
                {
                    case "001":
                        t.DownloadStockList();
                        break;
                    case "002":
                        t.DownLoadStockHistoryData("2017-04-29", DateTime.Now.ToString());
                        break;
                    case "003":
                        t.DownLoadStockData();
                        break;
                    case "004":
                        t.ComputeStockIndex(DateTime.Now.ToString("yyyy-MM-dd"), "");
                        break;
                    case "005":
                        date = Convert.ToDateTime("2017-04-26");
                        while (date <= DateTime.Now)
                        {
                            t.ComputeStockIndex(date.ToString("yyyy-MM-dd"), "");
                            date = date.AddDays(1);
                        }
                        break;
                    case "006":
                        {
                            t.DownLoadStockHistoryData("2000-01-01", DateTime.Now.ToString(), args[1].ToString());
                        }
                        break;
                    case "007":
                        {
                            t.DataMaintain(DateTime.Now.ToString("yyyy-MM-dd"));
                        }
                        break;
                    case "008":
                        {
                            date = Convert.ToDateTime("2000-01-01");
                            while (date <= DateTime.Now)
                            {
                                t.DataMaintain(date.ToString("yyyy-MM-dd"));
                                date = date.AddDays(1);
                            }
                        }
                        break;
                    case "009":
                        {
                            try
                            {
                                date = Convert.ToDateTime(args[1]);
                                while (date <= Convert.ToDateTime(args[2]))
                                {
                                    t.Train(1000, 500, date);
                                    t.BPNNForecastStock(date);
                                    t.ValideForecast(date.ToString("yyyy-MM-dd"));
                                    Console.WriteLine(string.Format("时间：{0}预测股票成功。日期：{1}", DateTime.Now.ToString(), date));
                                    date = date.AddDays(1);
                                }
                            }
                            catch (Exception)
                            {
                                Console.WriteLine("输入格式不正确，正确格式为： 009 开始时间 结束时间");
                                Console.ReadLine();
                            }
                        }
                        break;
                    case "010":
                        {
                            t.BPNNForecastStock(DateTime.Now);
                            date = DateTime.Now.AddDays(-60);
                            while (date <= DateTime.Now)
                            {
                                t.ValideForecast(date.ToString("yyyy-MM-dd"));
                                date = date.AddDays(1);
                            }
                        }
                        break;
                    case "011":
                        {
                            date = Convert.ToDateTime("2017-05-05");
                           //date = DateTime.Now;
                            t.Train(1000, 500, date);
                            t.BPNNForecastStock(date);
                            for (int i = 0; i < 10;i++)
                            {
                                t.ValideForecast(date.AddDays(-i).ToString("yyyy-MM-dd"));
                            }
                           
                        }
                        break;
                }
                #endregion
            }
        }
    }
}
