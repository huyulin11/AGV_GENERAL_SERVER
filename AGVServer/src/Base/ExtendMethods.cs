using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SqlClient;
using System.Data;
using System.Reflection;
using System.Xml;
using System.Xml.Serialization;
using System.Text.RegularExpressions;

namespace GiatiaAGV.Base
{
    /// <summary>
    /// 扩展方法
    /// </summary>
    public static class ExtendMethods
    {
        /// <summary>
        /// 转换为string，出错返回err
        /// </summary>
        /// <param name="str">需要转换的对象</param>
        /// <param name="err">对象为空时的默认值</param>
        /// <returns>对象的字符串</returns>
        public static string ToStringEx(this object str, string err)
        {
            if (str != null)
                return str.ToString();
            return err;

        }

        /// <summary>
        /// Byte转16进制字符串
        /// </summary>
        /// <param name="bytes">需要转换的byte数组</param>
        /// <returns>16进制字符串</returns>
        public static string ToHexString(this byte[] bytes)
        {
            string hexString = string.Empty;

            if (bytes != null)
            {
                StringBuilder strB = new StringBuilder();
                for (int i = 0; i < bytes.Length; i++)
                {
                    strB.Append(bytes[i].ToString("X2"));
                }
                hexString = strB.ToString();
            }
            return hexString;
        }

        /// <summary>
        /// 时间转换为UNIX时间戳
        /// </summary>
        /// <param name="dt">时间</param>
        /// <returns>UNIX时间戳</returns>
        public static int ToUnixInt(this DateTime dt)
        {
            try
            {
                return (int)((dt.ToUniversalTime() - Constant.MinDateTime).TotalSeconds);

            }
            catch (Exception)
            {

                return 0;
            }
        }

        /// <summary>
        /// UNIX时间戳转datetime
        /// </summary>
        /// <param name="ts"></param>
        /// <returns></returns>
        public static DateTime UnixIntToDT(this int ts)
        {
            return TimeZone.CurrentTimeZone.ToLocalTime(Constant.MinDateTime).AddSeconds(ts);
        }

        /// <summary>
        /// 把对象转换称Decimal
        /// </summary>
        /// <param name="obj">需要转换的对象</param>
        /// <returns>转换后的值</returns>
        public static decimal ToDecimal(this object obj)
        {
            if (obj == null || DBNull.Value == obj)
                return 0M;

            return Convert.ToDecimal(obj);
        }
        /// <summary>
        /// 把对象转换称Decimal
        /// </summary>
        /// <param name="obj">需要转换的对象</param>
        /// <param name="format">格式化字符串</param>
        /// <returns>转换后的Decimal值</returns>
        public static decimal ToDecimal(this object obj, string format)
        {
            try
            {
                if (string.IsNullOrEmpty(format))
                    return obj.ToDecimal();
                return obj.ToInt32(0).ToString(format).ToDecimal();
            }
            catch
            {
                return obj.ToDecimal();
            }
        }
        /// <summary>
        /// 字符串转换成时间
        /// </summary>
        /// <param name="dt">时间字符串</param>
        /// <returns>转换后的时间</returns>
        public static DateTime ToDateTime(this string dt)
        {
            try
            {
                var elstr = @"-|年|月|日";

                Regex regEx = new Regex(elstr, System.Text.RegularExpressions.RegexOptions.IgnoreCase);
                dt = regEx.Replace(dt, "/");
                return Convert.ToDateTime(dt);

            }
            catch (Exception)
            {

                return DateTime.MinValue;
            }
        }


        /// <summary>
        /// 字符串转换称整数
        /// </summary>
        /// <param name="str">需要转换的字符串</param>
        /// <param name="error">转换不成功的默认值</param>
        /// <returns>转换错误，返回error</returns>
        public static int? ToInt(this string str, int? error)
        {
            try
            {
                return int.Parse(str);
            }
            catch (Exception)
            {
                return error;
            }
        }


        /// <summary>
        /// xml转实体类
        /// </summary>
        /// <typeparam name="T">需要转换成的泛型对象</typeparam>
        /// <param name="xmlStr">XML字符串</param>
        /// <returns>需要的泛型对象，泛型对象必须是可序列华的对象</returns>
        public static T XmlToEntity<T>(this string xmlStr) where T : new()
        {
            var model = new T();
            try
            {
                byte[] ret = Encoding.UTF8.GetBytes(xmlStr);
                using (System.IO.MemoryStream mem = new System.IO.MemoryStream(ret))
                {

                    XmlTextWriter writer = new XmlTextWriter(mem, Encoding.UTF8);
                    XmlSerializer xz = new XmlSerializer(typeof(T));
                    model = (T)xz.Deserialize(mem);
                }
                return model;
            }
            catch (Exception ex)
            {

                return new T();
            }
        }




        /// <summary>  
        /// DataTable转List
        /// </summary>  
        /// <param name="dt">需要转换的DataTable</param>  
        /// <returns></returns>  
        public static List<T> ToList<T>(this DataTable dt) where T : new()
        {

            // 定义集合  
            List<T> ts = new List<T>();

            // 获得此模型的类型  
            Type type = typeof(T);
            //定义一个临时变量  
            string tempName = string.Empty;
            //遍历DataTable中所有的数据行  
            foreach (DataRow dr in dt.Rows)
            {
                T t = new T();
                // 获得此模型的公共属性  
                PropertyInfo[] propertys = t.GetType().GetProperties();
                //遍历该对象的所有属性  
                foreach (PropertyInfo pi in propertys)
                {
                    tempName = pi.Name;//将属性名称赋值给临时变量  
                    //检查DataTable是否包含此列（列名==对象的属性名）    
                    if (dt.Columns.Contains(tempName))
                    {
                        // 判断此属性是否有Setter  
                        if (!pi.CanWrite) continue;//该属性不可写，直接跳出  
                        //取值  
                        object value = dr[tempName];
                        //如果非空，则赋给对象的属性  

                        if (value != DBNull.Value)
                        {
                            if (pi.PropertyType.IsGenericType)
                                pi.SetValue(t, Convert.ChangeType(value, pi.PropertyType.GetGenericArguments()[0]), null);
                            else
                                pi.SetValue(t, Convert.ChangeType(value, pi.PropertyType), null);
                        }
                    }
                }
                //对象添加到泛型集合中  
                ts.Add(t);
            }

            return ts;

        }

        /// <summary>
        /// 转为int类型
        /// </summary>
        /// <param name="obj">需要转换的对象</param>
        /// <returns>Int32</returns>
        public static int ToInt32(this object obj, int error)
        {
            try
            {
                if (obj.GetType() == typeof(decimal))
                {
                    return Decimal.ToInt32((decimal)obj);
                }
                return int.Parse(obj.ToString());
            }
            catch (Exception)
            {
                return error;
            }
        }

        /// <summary>
        /// 转为int类型,错误返回【Int32.MinValue】
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static int ToInt32(this object obj)
        {
            try
            {
                if (obj.GetType() == typeof(decimal))
                {
                    return Decimal.ToInt32((decimal)obj);
                }
                return int.Parse(obj.ToString());
            }
            catch (Exception)
            {
                return Int32.MinValue;
            }
        }


        /// <summary>
        /// 转为小时分钟数字类型,错误返回【Int32.MinValue】
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static int ToDateHM(this string obj)
        {
            try
            {
                return obj.Replace(":", "").ToInt32();
            }
            catch (Exception)
            {
                return 0;
            }

        }


        /// <summary>
        /// 转为long类型
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static long ToInt64(this object obj, long error)
        {
            try
            {
                return long.Parse(obj.ToString());
            }
            catch (Exception)
            {
                return error;
            }
        }

        /// <summary>
        /// 转为long类型【错误返回long.MinValue】
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static long ToInt64(this object obj)
        {
            try
            {
                return long.Parse(obj.ToString());
            }
            catch (Exception)
            {
                return long.MinValue;
            }
        }

        /// <summary>  
        /// 填充对象：用DataRow填充实体类
        /// </summary>  
        public static T FillModel<T>(this DataRow row) where T : new()
        {
            if (row == null)
            {
                return default(T);
            }

            //T model = (T)Activator.CreateInstance(typeof(T));  
            T model = new T();

            for (int i = 0; i < row.Table.Columns.Count; i++)
            {
                PropertyInfo propertyInfo = model.GetType().GetProperty(row.Table.Columns[i].ColumnName);

                if (propertyInfo != null && row[i] != DBNull.Value)
                {
                    if (propertyInfo.PropertyType.IsGenericType)
                        propertyInfo.SetValue(model, Convert.ChangeType(row[i], propertyInfo.PropertyType.GetGenericArguments()[0]), null);
                    else
                        propertyInfo.SetValue(model, Convert.ChangeType(row[i], propertyInfo.PropertyType), null);
                }

            }
            return model;
        }

        /// <summary>
        /// DataReader填充实体
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="rdr"></param>
        /// <returns></returns>
        public static T DataReaderToObj<T>(this SqlDataReader rdr)
        {
            T t = System.Activator.CreateInstance<T>();
            Type obj = t.GetType();

            if (rdr.Read())
            {
                // 循环字段  
                for (int i = 0; i < rdr.FieldCount; i++)
                {
                    object tempValue = null;
                    tempValue = rdr.GetValue(i);
                    var propertyInfo = obj.GetProperty(rdr.GetName(i));
                    if (propertyInfo != null && tempValue != DBNull.Value)
                    {
                        if (propertyInfo.PropertyType.IsGenericType)
                            propertyInfo.SetValue(t, Convert.ChangeType(tempValue, propertyInfo.PropertyType.GetGenericArguments()[0]), null);
                        else
                            propertyInfo.SetValue(t, tempValue, null);
                    }


                }
                return t;
            }
            else
                return default(T);

        }
    }
}