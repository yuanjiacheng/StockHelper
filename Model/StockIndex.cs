using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model
{
    public class StockIndex
    {
        /// <summary>
        /// 股票指标ID
        /// </summary>
        public int StockIndexID { set; get; }
        /// <summary>
        /// 股票指标名称
        /// </summary>
        public string StockIndexName { get; set; }

    }
}
