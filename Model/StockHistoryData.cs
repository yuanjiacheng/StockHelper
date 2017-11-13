using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model
{
    #region 单日股票数据表结构
    /// <summary>
    /// 单日股票数据结构
    /// </summary>
    public class StockHistoryData
    {
        /// <summary>
        /// 股票历史数据ID
        /// </summary>
        public Guid StockHistoryDataID
        {
            get;
            set;
        }
        /// <summary>
        /// 股票编码
        /// </summary>
        public string StockCode
        {
            get;
            set;
        }
        /// <summary>
        /// 日期
        /// </summary>
        public string StockHistoryDate
        {
            get;
            set;
        }
        /// <summary>
        /// 开盘价
        /// </summary>
        public decimal SOpen
        {
            get;
            set;
        }
        /// <summary>
        /// 最高价
        /// </summary>
        public decimal SHigh
        {
            get;
            set;
        }
        /// <summary>
        /// 最低价
        /// </summary>
        public decimal SLow
        {
            get;
            set;
        }
        /// <summary>
        /// 收盘价
        /// </summary>
        public decimal SClose
        {
            get;
            set;
        }
        /// <summary>
        /// 成交量
        /// </summary>
        public long SVolume
        {
            get;
            set;
        }
    }
    #endregion
    #region 行情衍生指标数据
    /// <summary>
    /// 特殊情况
    /// </summary>
    public struct SpacialShape
    {
        /// <summary>
        /// 停牌
        /// </summary>
        public static int Suspended = 1;
        /// <summary>
        /// 事件
        /// </summary>
        public static int Event = 2;
    }

    /// <summary>
    /// 均线形态
    /// </summary>
    public struct LineEqualShape
    {
        /// <summary>
        /// 无特殊情况
        /// </summary>
        public static int le_non = 100;
        /// <summary>
        /// 5日均线向下击穿一条均线
        /// </summary>
        public static int le_down = 101;
        /// <summary>
        /// 5日均线向下击穿两条均线
        /// </summary>
        public static int le_ddown = 102;
        /// <summary>
        /// 5日均线像上击穿一条均线
        /// </summary>
        public static int le_up = 103;
        /// <summary>
        /// 5日均线向上击穿两条均线
        /// </summary>
        public static int le_dup = 104;
    
    }
    /// <summary>
    /// k线形态
    /// </summary>
    public struct LineKShape
    {
        /// <summary>
        /// 无特殊情况
        /// </summary>
        public static int lk_non = 200;
        /// <summary>
        /// 锤头型
        /// </summary>
        public static int lk_hammer = 201;
        /// <summary>
        /// 倒锤头型
        /// </summary>
        public static int lk_fallHammer = 202;
        /// <summary>
        /// 十字星
        /// </summary>
        public static int lk_cross = 203;
        /// <summary>
        /// 长阳线
        /// </summary>
        public static int lk_changYang = 204;
        /// <summary>
        /// 长阴线
        /// </summary>
        public static int lk_changYin = 205;
    }
    /// <summary>
    /// k线位置
    /// </summary>
    public struct LineKPosition
    {
        /// <summary>
        /// 无特殊情况
        /// </summary>
        public static int lk_non = 200;
        /// <summary>
        /// 股价位于15日低位
        /// </summary>
        public static int lk_positionLow = 211;
        /// <summary>
        /// 股价位于15日高位
        /// </summary>
        public static int lk_positionHigh = 212;
    }


    /// <summary>
    /// kdj指标
    /// </summary>
    public struct KDJShape
    {
        /// <summary>
        /// 无特殊情况
        /// </summary>
        public static int kdj_non = 300;
        /// <summary>
        /// kdj超卖
        /// </summary>
        public static int kdj_overSell = 301;
        /// <summary>
        /// kdj超买
        /// </summary>
        public static int kdj_overBuy = 302;
    }
    /// <summary>
    /// kdj线形态指标
    /// </summary>
    public struct KDJLineShape
    {
        /// <summary>
        /// 无特殊情况
        /// </summary>
        public static int kdjl_non = 300;
        /// <summary>
        /// k线向上击穿d线
        /// </summary>
        public static int kdjl_up = 311;
        /// <summary>
        /// k线向下击穿d线
        /// </summary>
        public static int kdjl_down = 312;
    }

    /// <summary>
    /// vr指标
    /// </summary>
    public struct VRShape
    {
        /// <summary>
        /// 无特殊情况
        /// </summary>
        public static int vr_non = 400;
        /// <summary>
        /// vr指数低价区域<70
        /// </summary>
        public static int vr_Low = 401;
        /// <summary>
        /// vr指数安全区域70-150
        /// </summary>
        public static int vr_Normal = 402;
        /// <summary>
        /// vr指数上升区域150-350
        /// </summary>
        public static int vr_Rise = 403;
        /// <summary>
        /// vr指数简介区域350-450
        /// </summary>
        public static int vr_High = 404;
        /// <summary>
        /// vr指数危险区域>450
        /// </summary>
        public static int vr_Hight = 405;
    }

    /// <summary>
    /// wr指标
    /// </summary>
    public struct WRShape
    {
        /// <summary>
        /// 无特殊情况
        /// </summary>
        public static int wr_non = 500;
        /// <summary>
        /// wr超卖
        /// </summary>
        public static int wr_overSell = 501;
        /// <summary>
        /// wr超买
        /// </summary>
        public static int wr_overBuy = 502;
    }
    public struct PSYShape
    {
        public static int psy_non = 600;
        public static int psy_low = 601;
        public static int psy_high = 602;
        public static int psy_lower = 603;
        public static int psy_higher = 604;
        public static int psy_lowest = 605;
        public static int psy_highest = 606;
    }
    #endregion
  
}
