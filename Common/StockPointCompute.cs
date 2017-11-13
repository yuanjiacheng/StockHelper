using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Model;

namespace Common
{
    public class StockPointCompute
    {
        #region 私有方法
        private double Get_NDays_LowsOrHighest(int days, bool LowsOrHighest, List<StockHistoryData> stock, int fromdaynum)//获取n天内的最高值或最低值ture为的最高值
        {
            double val = 0;
            double Val = 0;
            if (LowsOrHighest)
            {
                for (int i = 0; i < days; i++)
                {
                    Val = (Convert.ToDouble(stock[i + fromdaynum].SHigh) > Convert.ToDouble(stock[i + fromdaynum + 1].SHigh) ? Convert.ToDouble(stock[i + fromdaynum].SHigh) : Convert.ToDouble(stock[i + fromdaynum + 1].SHigh));
                    val = val > Val ? val : Val;
                }
            }
            else
            {
                for (int i = 0; i < days; i++)
                {
                    if (i == 0)
                    {
                        val = (Convert.ToDouble(stock[i + fromdaynum].SLow) < Convert.ToDouble(stock[i + 1 + fromdaynum].SLow) ? Convert.ToDouble(stock[i + fromdaynum].SLow) : Convert.ToDouble(stock[i + 1 + fromdaynum].SLow));
                    }
                    else
                    {
                        Val = (Convert.ToDouble(stock[i + fromdaynum].SLow) < Convert.ToDouble(stock[i + 1 + fromdaynum].SLow) ? Convert.ToDouble(stock[i + fromdaynum].SLow) : Convert.ToDouble(stock[i + 1 + fromdaynum].SLow));
                        val = val < Val ? val : Val;
                    }
                }
            }
            return val;
        }
        private string LineStyle(double[] Line1, double[] Line2)     //判断两线的状态
        {
            string Intersect = "non";
            if (Line1[0] <= Line2[0])
            {
                for (int i = 0; i < Line1.Length; i++)
                {
                    if (Line1[i] >= Line2[i])
                    {
                        Intersect = "down";
                        break;
                    }
                }
            }
            else
            {
                for (int i = 0; i < Line1.Length; i++)
                {
                    if (Line1[i] <= Line2[i])
                    {
                        Intersect = "up";
                        break;
                    }
                }
            }
            return Intersect;
        }
        /// <summary>
        /// 数据均线
        /// </summary>
        /// <param name="stock">股票数据</param>
        /// <param name="days">均线平均数</param>
        /// <param name="postion">输出日期定位</param>
        /// <returns>计算某日期的均线</returns>
        private double ComputeLineEqual(List<StockHistoryData> stock, int days, int postion)   //days指的是输出几日的线暂定为5日，15日，27日线
        {
            double sum = 0;
            double LineEqual = 0;
            for (int i = 0; i < days; i++)
            {
                sum = sum + Convert.ToDouble(stock[i + postion].SClose);
            }
            LineEqual = sum / days;
            return LineEqual;
        }

        public string[] Remove(string[] array, int index)   //移除数组中的某项，array为需被移除项的数组，index为第几项
        {
            int length = array.Length;
            string[] result = new string[length - 1];
            Array.Copy(array, result, index);
            Array.Copy(array, index + 1, result, index, length - index - 1);
            return result;
        }
        #endregion

        /// <summary>
        /// 计算均线指标（要保证数据按日期倒叙排序，且数据样本大于60）
        /// </summary>
        /// <param name="stock">行情数据</param>
        /// <returns>均线指标</returns>
        public List<int> Index_LineEqual(List<StockHistoryData> stock)
        {
            List<int> result = new List<int>();
            int Index = LineEqualShape.le_non;
            double[] LineEqual5 = new double[10];
            double[] LineEqual15 = new double[10];
            double[] LineEqual30 = new double[10];
            for (int i = 0; i < 10; i++)
            {
                LineEqual5[i] = ComputeLineEqual(stock, 5, i);
                LineEqual15[i] = ComputeLineEqual(stock, 15, i);
                LineEqual30[i] = ComputeLineEqual(stock, 30, i);
            }
            string Line5_Line15 = LineStyle(LineEqual5, LineEqual15);
            string Line5_Line30 = LineStyle(LineEqual5, LineEqual30);
            if (Line5_Line15 == "down" || Line5_Line30 == "down")
                Index = LineEqualShape.le_down;
            if (Line5_Line15 == "down" && Line5_Line30 == "down")
                Index = LineEqualShape.le_ddown;
            if (Line5_Line15 == "up" || Line5_Line30 == "up")
                Index = LineEqualShape.le_up;
            if (Line5_Line15 == "up" && Line5_Line30 == "up")
                Index = LineEqualShape.le_dup;
            if ((Line5_Line15 == "up" && Line5_Line30 == "down") || (Line5_Line15 == "down" && Line5_Line30 == "up"))
            {
                Index = LineEqualShape.le_non;
            }
            result.Add(Index);
            return result;
        }

        /// <summary>
        /// 计算k线形态
        /// </summary>
        /// <param name="stock">行情数据</param>
        /// <returns>k线指标</returns>
        public List<int> Index_LineK(List<StockHistoryData> stock)
        {
            var result = new List<int>();
            int Index = LineKShape.lk_non;
            double FifteenDay_Position = 0; //当天相对于15天内股价的位置
            double FifteenDay_Highest = Get_NDays_LowsOrHighest(15, true, stock, 0);  //15天内最高值
            double FifteenDay_Lowest = Get_NDays_LowsOrHighest(15, false, stock, 0); //15天内最低值
            double EntityToday = Math.Abs(Convert.ToDouble(stock[0].SClose - stock[0].SOpen));
            FifteenDay_Position = (Convert.ToDouble(stock[0].SClose) - FifteenDay_Lowest) / (FifteenDay_Highest - FifteenDay_Lowest);
            if (EntityToday / Convert.ToDouble(stock[0].SHigh - stock[0].SLow) <= 0.4)   //实体
            {
                double higher = Convert.ToDouble(stock[0].SClose < stock[0].SOpen ? stock[0].SClose : stock[0].SOpen);  //一天中开盘收盘较高的一项
                double positon = (Convert.ToDouble(stock[0].SHigh) - higher) / Convert.ToDouble(stock[0].SHigh - stock[0].SLow);
                if (positon >= 0.3 && positon <= 0.6 && EntityToday / Convert.ToDouble(stock[0].SHigh - stock[0].SLow) <= 0.1)
                {
                    Index = LineKShape.lk_cross;
                }
                else if (positon <= 0.15)
                {
                    Index = LineKShape.lk_hammer; //锤头型，T
                }
                else if (positon >= 0.85)
                {
                    Index = LineKShape.lk_fallHammer;    //反锤头型，反T
                }
            }
            else if (EntityToday / Convert.ToDouble(stock[0].SHigh - stock[0].SLow) >= 0.8)
            {
                if (stock[0].SClose > stock[0].SOpen)
                {
                    Index = LineKShape.lk_changYang;
                }
                else
                {
                    Index = LineKShape.lk_changYin;
                }
            }
            result.Add(Index);
            int lkpIndex = LineKPosition.lk_non;
            if (FifteenDay_Position < 0.2)
            {
                lkpIndex = LineKPosition.lk_positionLow;
            }
            else if (FifteenDay_Position > 0.8)
            {
                lkpIndex = LineKPosition.lk_positionHigh;
            }
            result.Add(lkpIndex);
            return result;
        }

        /// <summary>
        /// 计算kdj指标
        /// </summary>
        /// <param name="stock">行情数据</param>
        /// <returns>kdj指标</returns>
        public List<int> Index_KDJ(List<StockHistoryData> stock)
        {
            var result = new List<int>();
            int Index = KDJShape.kdj_non;
            double[] RSV = new double[15];
            for (int i = 0; i < 15; i++)
            {
                double NineDays_Lowest = Get_NDays_LowsOrHighest(9, false, stock, i);
                double NineDays_Highest = Get_NDays_LowsOrHighest(9, true, stock, i);
                RSV[i] = (Convert.ToDouble(stock[i].SClose) - NineDays_Lowest) / (NineDays_Highest - NineDays_Lowest);
            }
            double[] K = new double[15];
            for (int i = 0; i < 15; i++)
            {
                if (i == 0)
                {
                    K[14] = RSV[14];
                }
                else
                {
                    K[14 - i] = K[15 - i] * 2 / 3 + RSV[14 - i] / 3;
                }
                if (K[14 - i] > 1.0)
                {
                    K[14 - i] = 1.0;
                }
                if (K[14 - i] < 0)
                {
                    K[14 - i] = 0;
                }
            }
            double[] D = new double[15];
            for (int i = 0; i < 15; i++)
            {
                if (i == 0)
                {
                    D[14] = K[14];
                }
                else
                {
                    D[14 - i] = D[15 - i] * 2 / 3 + K[14 - i] / 3;
                }
                if (D[14 - i] > 1.0)
                {
                    D[14 - i] = 1.0;
                }
                if (K[14 - i] < 0)
                {
                    K[14 - i] = 0;
                }
            }
            double[] J = new double[15];
            for (int i = 0; i < 15; i++)
            {
                J[i] = 3 * K[i] - 2 * D[i];
                if (J[i] > 1.0)
                {
                    J[i] = 1.0;
                }
                if (J[i] < 0)
                {
                    J[i] = 0;
                }
            }
            double[] k = new double[3];
            for (int i = 0; i < 3; i++)
            {
                k[i] = K[i];
            }
            double[] d = new double[3];
            for (int i = 0; i < 3; i++)
            {
                d[i] = D[i];
            }
            string intersect = LineStyle(k, d);
            if (K[0] <= 0.25 && D[0] <= 0.25 && J[0] <= 0.25)
            {
                Index = KDJShape.kdj_overSell;
            }
            if (K[0] >= 0.75 && D[0] >= 0.75 && J[0] >= 0.75)
            {
                Index = KDJShape.kdj_overBuy;
            }
            result.Add(Index);
            int ldjlIndex = KDJLineShape.kdjl_non;
            if (intersect == "up")
            {
                ldjlIndex = KDJLineShape.kdjl_up;
            }
            if (intersect == "down")
            {
                ldjlIndex = KDJLineShape.kdjl_down;
            }
            return result;
        }

        /// <summary>
        /// 计算vr指标（成交量变异率指标）
        /// </summary>
        /// <param name="stock">行情数据</param>
        /// <returns>vr指标</returns>
        public List<int> Index_VR(List<StockHistoryData> stock)
        {
            List<int> result = new List<int>();
            int Index = VRShape.vr_non;
            double[] VR = new double[10];
            for (int j = 0; j < 10; j++)
            {
                double vr1 = 0;
                double vr2 = 0;
                for (int i = 0; i < 26; i++)
                {
                    if (stock[i + j].SClose > stock[i + j + 1].SClose)
                    {
                        vr1 = vr1 + stock[i + j].SVolume;
                    }
                    else if (stock[i + j].SClose < stock[i + j + 1].SClose)
                    {
                        vr2 = vr2 + stock[i + j].SVolume;
                    }
                    else
                    {
                        vr1 = vr1 + stock[i + j].SVolume / 2;
                        vr2 = vr2 + stock[i + j].SVolume / 2;
                    }
                }
                VR[j] = vr1 / vr2 * 100;
            }
            if (VR[0] <= 70)
            {
                Index = VRShape.vr_Low;
            }
            if (VR[0] > 70 && VR[0] <= 150)
            {
                Index = VRShape.vr_Normal;
            }
            if (VR[0] > 150 && VR[0] <= 350)
            {
                Index = VRShape.vr_Rise;
            }
            if (VR[0] > 350 && VR[0] <= 450)
            {
                Index = VRShape.vr_High;
            }
            if (VR[0] > 450)
            {
                Index = VRShape.vr_Hight;
            }
            result.Add(Index);
            return result;
        }

        /// <summary>
        /// 计算wr指标(威廉超买超卖指数)
        /// </summary>
        /// <param name="stock">行情数据</param>
        /// <returns>wr指标</returns>
        public List<int> Index_W_R(List<StockHistoryData> stock)
        {
            List<int> result = new List<int>();
            int Index = WRShape.wr_non;
            double W_R = (Get_NDays_LowsOrHighest(14, true, stock, 0) - Convert.ToDouble(stock[0].SClose)) / (Get_NDays_LowsOrHighest(14, true, stock, 0) - Get_NDays_LowsOrHighest(14, false, stock, 0));
            if (W_R <= 0.2)
            {
                Index = WRShape.wr_overSell;
            }
            else if (W_R >= 0.8)
            {
                Index = WRShape.wr_overBuy;
            }
            result.Add(Index);
            return result;
        }

        /// <summary>
        /// 计算PSY指标
        /// </summary>
        /// <param name="stock">行情数据</param>
        /// <returns>PSY指标</returns>
        public List<int> Index_PSY(List<StockHistoryData> stock)
        {
            List<int> result = new List<int>();
            int Index = PSYShape.psy_non;
            double Twelve_DaysRaise = 0;
            for (int i = 0; i < 12; i++)
            {
                if (stock[i].SClose > stock[i].SOpen)
                {
                    Twelve_DaysRaise++;
                }
            }
            double PSY = Twelve_DaysRaise / 12 * 100;
            if (PSY <= 25)
            {
                Index = PSYShape.psy_low;
            }
            if (PSY >= 75)
            {
                Index = PSYShape.psy_high;
            }
            if (PSY <= 17)
            {
                Index = PSYShape.psy_lower;
            }
            if (PSY >= 83)
            {
                Index = PSYShape.psy_higher;
            }
            if (PSY <= 10)
            {
                Index = PSYShape.psy_lowest;
            }
            if (PSY >= 90)
            {
                Index = PSYShape.psy_highest;
            }
            result.Add(Index);
            return result;
        }

        /// <summary>
        /// 获得涨幅
        /// </summary>
        /// <param name="list"></param>
        /// <returns></returns>
        public List<double> GetIncrease(List<decimal> list)
        {
            List<double> result = new List<double>();
            for (int i = 1; i < list.Count; i++)
            {
                result.Add((double)((list[i] - list[i - 1]) / list[i - 1]) * 100.00);
            }
            return result;
        }
        public List<double> GetIncrease(List<double> list)
        {
            List<double> result = new List<double>();
            for (int i = 1; i < list.Count; i++)
            {
                result.Add((double)((list[i] - list[i - 1]) / list[i - 1]) * 100.00);
            }
            return result;
        }
        public List<double> GetIncrease(List<long> list)
        {
            List<double> result = new List<double>();
            for (int i = 1; i < list.Count; i++)
            {
                result.Add(((double)list[i] - (double)list[i - 1]) / (double)list[i - 1] * 100.00);
            }
            return result;
        }
        /// <summary>
        /// 将整段array数组切割成指定长度小段数组
        /// </summary>
        /// <param name="spiltLength"></param>
        /// <returns></returns>
        public List<List<double>> spiltArray(List<double> array,int spiltLength)
        {
            List<List<double>> result = new List<List<double>>();
            for (int i = 0; i < array.Count - spiltLength; i++)
            {
                List<double> res = new List<double>();
                for (int j = 0; j < spiltLength; j++)
                {
                    res.Add(array[i + j]);
                }
                result.Add(res);
            }
            return result;
        }
    }
}
