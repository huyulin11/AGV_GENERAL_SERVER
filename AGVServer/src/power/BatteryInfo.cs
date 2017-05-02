using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using AGV;
namespace AGV.power
{
    /// <summary>
    /// 获取电池相关信息
    /// </summary>
    public class BatteryInfo
    {
        private int soc = 0; //电池电量百分比， 最后一位忽略不计，比如87.5%  只计算成87%

        private static int BATTERY_LOWPOWER_STAT = 30;
        private int lowpowerTimes = 0; //连续三次检测到电压低于某值,才确定低电压

        public void setBatterySoc(int soc)
        {
            if (soc < 10 && soc > 1000)
            {
                Console.WriteLine("battery soc err");
                return;
            }
            this.soc = soc / 10;


            if (this.soc < BATTERY_LOWPOWER_STAT)
            {
                lowpowerTimes++;
            }
            else
            {
                lowpowerTimes = 0;
            }
        }

        public int getBatterySoc()
        {
            return this.soc;
        }

        public bool isBatteryLowpower()
        {
            return lowpowerTimes > 3;
        }

    }
}
