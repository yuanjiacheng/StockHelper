using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Common;
using Common.Extension;
using Common.Helper;
using Model;
using NeuralNetWork;

namespace StockHelper
{
    public class TrainingRoom
    {
        public class Sample
        {
            public double[] feature { get; set; }
            public double[] result { get; set; }
        }

        /// <summary>
        /// 初始化
        /// </summary>
        /// <param name="inputLayerNum">输入层神经元数量</param>
        /// <param name="hiddenLayerNum">隐藏层层数和神经元数量</param>
        /// <param name="outputLayersNum">输出层神经元数量</param>
        /// <param name="eta">学习步长</param>
        /// <param name="lastEta">动态学习变化步长</param>
        /// <param name="errorLevel">错误级别</param>
        /// <param name="endTime">最大训练次数</param>
        /// <param name="samples">训练样本</param>
        public void TrainBPNN(int inputLayerNum, int[] hiddenLayerNum, int outputLayersNum, double eta, double lastEta, double errorLevel, DateTime endTime, List<Sample> samples, string networkSavePath)
        {
            BPNN net = new BPNN(inputLayerNum, hiddenLayerNum, outputLayersNum, eta, lastEta);
            //var networkSt = File.ReadAllText(networkSavePath);
            //var net = JsonHelper.DeserializeJsonToObject<BPNN>(networkSt);
            double error = Int32.MaxValue;
            while (DateTime.Now < endTime && error > errorLevel)
            {
                error = net.train(ref net, samples.Select(s => s.feature).ToArray(), samples.Select(s => s.result).ToArray());
                Console.WriteLine(error);
            }
            string networkStr = JsonHelper.SerializeObject(net);
            File.WriteAllText(networkSavePath, networkStr);
        }

        public double ForecastBPNN(List<Sample> samples, string networkPath, double bound)
        {
            var networkStr = File.ReadAllText(networkPath);
            var net = JsonHelper.DeserializeJsonToObject<BPNN>(networkStr);
            double a = 0, b = 0;
            foreach (var item in samples)
            {
                var res = net.forecast(ref net, item.feature);
                if ((bound <= 0 && res[0] <= bound && bound - 0.01 < res[0]) || (bound > -0.000001 && res[0] > bound && bound + 0.01 > res[0]))
                {
                    a++;
                    b += item.result[0];
                }
            }
            if (bound <= 0)
            {
                Console.WriteLine(string.Format("约束量<{0},样本总数：{1},均值：{2}", bound, a, b / a * 100));
            }
            else
            {
                Console.WriteLine(string.Format("约束量>{0},样本总数：{1},均值：{2}", bound, a, b / a * 100));
            }
            return b / a;
        }

        public List<Sample> GetSample(DateTime date)
        {
            Proc proc = new Proc();
            List<Sample> result = new List<Sample>();
            var res = proc.getSample(date);
            foreach (DataRow dr in res.Tables[1].Rows)
            {
                var stockCode = dr[0].ToString();
                var rise = Convert.ToDouble(dr[1]);
                List<double> tFeature = new List<double>();
                foreach (DataRow row in res.Tables[0].Rows)
                {
                    if (stockCode == row[0].ToString())
                    {
                        tFeature.Add(Convert.ToInt32(row[1]));
                    }
                }
                if (tFeature.Count > 0)
                {
                    List<double> Feature = new List<double>();
                    foreach (DataRow row in res.Tables[2].Rows)
                    {
                        if(tFeature.Contains(Convert.ToInt32(row[0])))
                        {
                            Feature.Add(1);
                        }
                        else
                        {
                            Feature.Add(0);
                        }
                    }
                    List<double> rises = new List<double>();
                    rises.Add(rise);
                    Sample item = new Sample() { feature = Feature.ToArray(), result = rises.ToArray() };
                    result.Add(item);
                }
            }
            return result;
        }

        #region 私有方法
        /// <summary>
        /// 获取样本缓存样本
        /// </summary>
        /// <param name="sampleCachePath">缓存样本地址</param>
        /// <returns></returns>
        private List<Sample> GetSampleCache(string sampleCachePath, int getNum)
        {
            List<Sample> result = new List<Sample>();
            try
            {
                if (File.Exists(sampleCachePath))
                {
                    string[] itemStrs = File.ReadAllLines(sampleCachePath);
                    int sampleNum = Convert.ToInt32(itemStrs.First());
                    for (int i = 0; i < sampleNum; i++)
                    {
                        Sample item = new Sample() { feature = itemStrs[i + 1].Trim().Split(',').ToDouble(), result = itemStrs[i + getNum + 1].Trim().Split(',').ToDouble() };
                        result.Add(item);
                    }
                }
                else
                {
                    result = null;
                }
            }
            catch (Exception ex)
            {
                result = null;
            }
            return result;
        }
        /// <summary>
        /// 保存样本缓存数据
        /// </summary>
        /// <param name="samples"></param>
        /// <param name="sampleCachePath"></param>
        private void SaveSampleCache(List<Sample> samples, string sampleCachePath)
        {
            string samplesStr = string.Empty;
            samplesStr += samples.Count.ToString() + "\r\n";
            for (int i = 0; i < samples.Count; i++)
            {
                samplesStr += string.Join(",", samples[i].feature) + "\r\n";
            }
            for (int i = 0; i < samples.Count; i++)
            {
                samplesStr += string.Join(",", samples[i].result) + "\r\n";
            }
            File.WriteAllText(sampleCachePath, samplesStr);
        }
        #endregion
    }
}
