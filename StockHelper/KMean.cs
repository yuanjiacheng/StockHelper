using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StockHelper
{
    public class kMean
    {
        #region K-Mean聚类
        /// <summary>
        /// k-mean聚类算法
        /// </summary>
        /// <param name="trainSet">要进行聚类的训练样本</param>
        /// <param name="count">聚类数</param>
        /// <returns></returns>
        public List<Crowd> K_Mean(List<List<double>> trainSet, int count)
        {
            var crowds = new List<Crowd>(count);
            for (var i = 0; i < count; i++)
            {
                crowds.Add(new Crowd());
                //这里根据业务手动设置初始值,15日下跌值，7%，-7%
                var maxVaule = int.MaxValue;
                var minValue = int.MinValue;
                if(i==0)
                {
                    minValue = 7 - i;
                }
                else if(i==count-i)
                {
                    maxVaule = 7 - i;
                }
                else
                {
                    maxVaule = 7 - i;
                    minValue = 6 - i;
                }
                crowds[i].Center = trainSet.Where(w=>w.Sum()>minValue&&w.Sum()<maxVaule).FirstOrDefault();
            }

            while (crowds.Sum(crowd => crowd.Change) > 0.01)
            {
                Console.WriteLine(crowds.Sum(crowd => crowd.Change));
                //Empty List and refresh Center
                crowds.ForEach(
                    crowd =>
                    {
                        if (!crowd.List.Any()) return;
                        crowd.RefreshCenter();
                        crowd.List.Clear();
                    });
                foreach (var TrainSet in trainSet)
                {
                    var index = 0; var minDistance = double.MaxValue;
                    for (var i = 0; i < count; i++)
                    {
                        var distance = getDistance(crowds[i].Center, TrainSet);
                        if (!(distance < minDistance)) continue;
                        index = i; minDistance = distance;
                    }
                    crowds[index].List.Add(TrainSet);
                }
            }
            crowds.ForEach(
                   crowd =>
                   {
                       if (!crowd.List.Any()) return;
                       crowd.RefreshCenter();
                       crowd.List.Clear();
                   });
            foreach (var TrainSet in trainSet)
            {
                var index = 0; var minDistance = double.MaxValue;
                for (var i = 0; i < count; i++)
                {
                    var distance = getDistance(crowds[i].Center, TrainSet);
                    if (!(distance < minDistance)) continue;
                    index = i; minDistance = distance;
                }
                crowds[index].List.Add(TrainSet);
                crowds[index].TrainSetList.Add(TrainSet);
            }

            return crowds.ToList();
        }

        /// <summary>
        /// 集群
        /// </summary>
        public class Crowd
        {
            public List<List<double>> TrainSetList = new List<List<double>>();
            public List<List<double>> List { get; set; }
            public List<double> Average { get { return Average(List,Center.Count); } }
            public List<double> Center { get; set; }
            public double Change { get; private set; }
            public Crowd()
            {
                Change = double.MaxValue;
                List = new List<List<double>>();
            }
            public void RefreshCenter()
            {
                Change = getDistance(Average, Center);
                Center = Average;
            }
        }
        #endregion
        private static double getDistance(List<double> a, List<double> b)
        {
            double sum = 0;
            for (int i = 0; i < a.Count; i++)
            {
                sum += Math.Pow(a[i] - b[i], 2);
            }
            var result=sum / a.Count;
            return result == double.NaN ? 0 : result;
        }
        private static List<double> Average(List<List<double>> items,int count)
        {
            List<double> result = new List<double>();
            for (int i = 0; i < count;i++ )
            {
                double sum = 0;
                foreach(var item in items)
                {
                    sum += item[i];
                }
                result.Add(sum / items.Count);
            }
            return result;
        }
    }
}
