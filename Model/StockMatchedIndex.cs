using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model
{
    public class StockMatchedIndex
    {
        /// <summary>
        /// ID
        /// </summary>
        public Guid StockMatchedIndexID { get; set; }
        /// <summary>
        /// 匹配的指标ID
        /// </summary>
        public int StockIndexID { get; set; }
        /// <summary>
        /// 股票代码
        /// </summary>
        public string StockCode { get; set; }
        /// <summary>
        /// 日期
        /// </summary>
        public DateTime Date { get; set; }
        
    }
}
