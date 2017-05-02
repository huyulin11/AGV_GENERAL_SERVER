using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Sockets;
using System.Net;
using System.Text.RegularExpressions;

namespace 单车AGV测试.Utils
{
   public class CommUtil
    {
       /// <summary>
       /// 获取本机IPV4
       /// </summary>
       /// <returns></returns>
       public static string GetLocalIPV4()
       {
           try
           {
               IPHostEntry ipentry = Dns.GetHostEntry(Dns.GetHostName());
               var _list = ipentry.AddressList;
               foreach (var item in _list)
               {
                   if (item.AddressFamily == AddressFamily.InterNetwork)
                   {
                       return item.ToString();
                   }
               }
               return "";
           }
           catch (Exception)
           {
               return "";
           }
       }

       // <summary>
       /// 判断字符串是否符合给定的标准
       /// </summary>
       /// <param name="value">待判断的值</param>
       /// <param name="regex">正则表达式</param>
       /// <returns></returns>
       public static bool IsMatch(string value, string regex)
       {
           Match m = Regex.Match(value, regex);

           if (m.Success)
           {
               return true;
           }
           else
           {
               return false;
           }
       }
       /// <summary>
       /// Ip地址
       /// </summary>
       /// <param name="value">地址</param>
       /// <returns></returns>
       public static bool IsMatchIPAddress(string value)
       {
           //string IPAddressRegex = @"^(\d{1,2}|1\d\d|2[0-4]\d|25[0-5])\.(\d{1,2}|1\d\d|2[0-4]\d|25[0-5])\.(\d{1,2}|1\d\d|2[0-4]\d|25[0-5])\.(\d{1,2}|1\d\d|2[0-4]\d|25[0-5])|localhost$";
           //string IPAddressRegex = @"^(0|[1-9]\d?|[0-1]\d{2}|2[0-4]\d|25[0-5])\.(0|[1-9]\d?|[0-1]\d{2}|2[0-4]\d|25[0-5])\.(0|[1-9]\d?|[0-1]\d{2}|2[0-4]\d|25[0-5])\.(0|[1-9]\d?|[0-1]\d{2}|2[0-4]\d|25[0-5])|localhost$";
           string IPAddressRegex = @"^(((2[0-4]\d|25[0-5]|[01]?\d\d?)\.){3}(2[0-4]\d|25[0-5]|[01]?\d\d?)|localhost)$";
           //string IPAddressRegex = @"^((((25[0-5]|2[0-4][0-9]|19[0-1]|19[3-9]|18[0-9]|17[0-1]|17[3-9]|1[0-6][0-9]|1[1-9]|[2-9][0-9]|[0-9])\.(25[0-5]|2[0-4][0-9]|1[0-9][0-9]|[1-9][0-9]|[0-9]))|(192\.(25[0-5]|2[0-4][0-9]|16[0-7]|169|1[0-5][0-9]|1[7-9][0-9]|[1-9][0-9]|[0-9]))|(172\.(25[0-5]|2[0-4][0-9]|1[0-9][0-9]|1[0-5]|3[2-9]|[4-9][0-9]|[0-9])))\.(25[0-5]|2[0-4][0-9]|1[0-9][0-9]|[1-9][0-9]|[0-9])\.(25[0-5]|2[0-4][0-9]|1[0-9][0-9]|[1-9][0-9]|[0-9])|localhost)$";

           return IsMatch(value, IPAddressRegex);
       }
    }
}
