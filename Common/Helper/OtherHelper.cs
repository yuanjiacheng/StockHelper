using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Helper
{
    public class OtherHelper
    {
        private string m_GetWeekNow(DateTime date)
        {
            string strWeek = date.DayOfWeek.ToString();
            switch (strWeek)
            {
                case "Monday":
                    return "1";
                case "Tuesday":
                    return "2";
                case "Wednesday":
                    return "3";
                case "Thursday":
                    return "4";
                case "Friday":
                    return "5";
                case "Saturday":
                    return "6";
                case "Sunday":
                    return "7";
            }
            return "0";
        }
        /// <summary>
        /// 判断当前日期是否为工作日
        /// </summary>
        /// <param name="date"></param>
        /// <returns></returns>
        public bool IsWorkingDay(DateTime date)
        {
            string res = m_GetWeekNow(date);
            if(res=="6"||res=="7")
            {
                return false;
            }
            else
            {
                return true;
            }
        }
        /// <summary>
        /// 获得随机小数 ps:除以100
        /// </summary>
        /// <param name="min"></param>
        /// <param name="max"></param>
        /// <returns></returns>
        public static double getRandDouble(int min, int max)
        {
            var seed = Guid.NewGuid().GetHashCode();
            var random = new System.Random(seed);
            double result = ((double)random.Next(min, max)) / 100.00;
            if (result == 0)
            {
                result = getRandDouble(min, max);
            }
            return result;
        }
        public static int getRand(int min,int max)
        {
            var seed = Guid.NewGuid().GetHashCode();
            var random = new System.Random(seed);
            return random.Next(min, max);
        }
    }
}
