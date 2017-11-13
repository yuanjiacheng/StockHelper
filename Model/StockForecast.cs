using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model
{
    public class StockForecast
    {
        public string StockCode { get; set; }
        public string Date { get; set; }
        public double ForeCast { get; set; }
        public double Fact { get; set; }
    }
}
