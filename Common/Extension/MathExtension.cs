using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Extension
{
    public static class MathExtension
    {
        /// <summary>
        ///  求双曲正切
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static double Tanh(this double input)
        {
            return Math.Tanh(input);
        }
        /// <summary>
        /// 求双曲正切的导数,Dtanh=1-tanh^2(x) ,这里input=tanh(x)
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static double DTanh(this double input)
        {
            double result = 0;
            result = 1 - Math.Pow(input,2);
            return result;
        }
        /// <summary>
        /// 获得行
        /// </summary>
        /// <param name="input"></param>
        /// <param name="row"></param>
        /// <returns></returns>
        public static double[] GetRow(this double[,] input, int row)
        {
            double[] result=new double[input.GetLength(1)];
            for(int i=0;i<result.Length;i++)
            {
                result[i] = input[row, i];
            }
            return result;
        }
        /// <summary>
        /// 获得列
        /// </summary>
        /// <param name="input"></param>
        /// <param name="column"></param>
        /// <returns></returns>
        public static double[] GetColumn(this double[,] input, int column)
        {
            double[] result = new double[input.GetLength(0)];
            for (int i = 0; i < result.Length; i++)
            {
                result[i] = input[i,column];
            }
            return result;
        }
        public static double[,] ToMatrix(this double[] input)
        {
            double[,] result = new double[1, input.Length];
            for(int i=0;i<input.Length;i++)
            {
                result[0, i] = input[i];
            }
            return result;
        }
        public static double[] ToDouble(this string[] input)
        {
            double[] result = new double[input.Length];
            for(int i=0;i<input.Length;i++)
            {
                result[i] = Convert.ToDouble(input[i]);
            }
            return result;
        }
    }
}
