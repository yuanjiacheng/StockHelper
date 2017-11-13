using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Common.Helper
{
    public class DataHelper
    {
        /// <summary>
        /// 讲用逗号分隔的字符串转化为集合数据
        /// </summary>
        /// <typeparam name="T">数据类型</typeparam>
        /// <param name="str">字符串</param>
        /// <returns>数据</returns>
        public static List<T> StringToList<T>(string str)
        {
            List<T> result = new List<T>();
            string[] strList = str.Split(',');
            foreach (object i in strList)
            {
                result.Add((T)i);
            }
            return result;
        }
        /// <summary>
        /// 获得配置属性值
        /// </summary>
        /// <param name="key">属性名</param>
        /// <returns>属性值</returns>
        public static string GetConfig(string key)
        {
            var rvl = string.Empty;
            try
            {
                rvl = System.Configuration.ConfigurationManager.AppSettings[key].ToString();
            }
            catch { }
            return rvl;
        }

        /// <summary>
        /// 将一个字符串进行sha1加密
        /// </summary>
        /// <param name="str_sha1_in">加密字符串</param>
        /// <returns>加密结果</returns>
        public static string SHA1_Hash(string str_sha1_in)
        {
            SHA1 sha1 = new SHA1CryptoServiceProvider();
            byte[] bytes_sha1_in = UTF8Encoding.Default.GetBytes(str_sha1_in);
            byte[] bytes_sha1_out = sha1.ComputeHash(bytes_sha1_in);
            string str_sha1_out = BitConverter.ToString(bytes_sha1_out);
            str_sha1_out = str_sha1_out.Replace("-", "");
            return str_sha1_out;
        }

        #region 将一个实体集合转换为Datatable
        /// <summary>
        /// 将一个实体集合转换为Datatable
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="items">实体集合</param>
        /// <param name="dt">要合并的datatable</param>
        /// <returns></returns>
        public static DataTable ToDataTable<T>(List<T> items, DataTable dt = null)
        {
            if (dt == null)
                dt = new DataTable(typeof(T).Name);

            PropertyInfo[] props = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance);

            foreach (PropertyInfo prop in props)
            {
                Type t = GetCoreType(prop.PropertyType);
                dt.Columns.Add(prop.Name, t);
            }

            foreach (T item in items)
            {
                var values = new object[props.Length];

                for (int i = 0; i < props.Length; i++)
                {
                    values[i] = props[i].GetValue(item, null);
                }

                dt.Rows.Add(values);
            }

            return dt;
        }
        public static DataTable ToDataTable<T>(T item, DataTable dt = null)
        {
            if (dt == null)
                dt = new DataTable(typeof(T).Name);

            PropertyInfo[] props = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance);

            foreach (PropertyInfo prop in props)
            {
                Type t = GetCoreType(prop.PropertyType);
                dt.Columns.Add(prop.Name, t);
            }

            var values = new object[props.Length];

            for (int i = 0; i < props.Length; i++)
            {
                values[i] = props[i].GetValue(item, null);
            }

            dt.Rows.Add(values);
            return dt;
        }
        private static Type GetCoreType(Type t)
        {
            if (t != null && IsNullable(t))
            {
                if (!t.IsValueType)
                {
                    return t;
                }
                else
                {
                    return Nullable.GetUnderlyingType(t);
                }
            }
            else
            {
                return t;
            }
        }
        private static bool IsNullable(Type t)
        {
            return !t.IsValueType || (t.IsGenericType && t.GetGenericTypeDefinition() == typeof(Nullable<>));
        }
        #endregion
        
        /// <summary>
        /// 移除数组中的某项
        /// </summary>
        /// <param name="array">array为需被移除项的数组</param>
        /// <param name="index">index为第几项</param>
        /// <returns>结果</returns>
        public static string[] Remove(string[] array, int index)   //移除数组中的某项，array为需被移除项的数组，index为第几项
        {
            int length = array.Length;
            string[] result = new string[length - 1];
            Array.Copy(array, result, index);
            Array.Copy(array, index + 1, result, index, length - index - 1);
            return result;
        }
        /// <summary>
        /// 求一列数据的均值
        /// </summary>
        /// <param name="list">数据</param>
        /// <returns>均值</returns>
        public static double ListAverage(List<double> list)
        {
            try {
                double sum = 0;
                foreach (double item in list)
                {
                    sum += item;
                }
                return list.Count() == 0 ? 0 : sum / list.Count();
            }
            catch(Exception ex)
            {
                return 0;
            }
        }
        /// <summary>
        /// 求一组数组的控件距离
        /// </summary>
        /// <param name="list1"></param>
        /// <param name="list2"></param>
        /// <returns></returns>
        public static double ListDistance(List<double> list1, List<double> list2)
        {
            double sum = 0.0;
            for(int i=0;i<list1.Count;i++)
            {
                sum += Math.Pow(list1[i] - list2[i], 2);
            }
            return sum / list1.Count;
        }

    }
}
