using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using GiatiaAGV.Model;

namespace GiatiaAGV.Base
{
    /// <summary>
    /// 读取配置信息
    /// </summary>
    public class Config
    {
        //ExeConfigurationFileMap map = new ExeConfigurationFileMap();
        //map.ExeConfigFilename = @"Configs\App.config"; 
        //Configuration config = ConfigurationManager.OpenMappedExeConfiguration(map, ConfigurationUserLevel.None);
        private readonly static string configPath = @"Configs\AGVConfig.xml";

        /// <summary>
        /// 获取导航仪胚子和信息
        /// </summary>
        /// <returns>导航仪配置</returns>
        public static NavConfig navConfig
        {
            get
            {
                XmlDocument xmlDoc = new XmlDocument();
                xmlDoc.Load(configPath);
                XmlNode xmldocSelect = xmlDoc.SelectSingleNode("configs/Nav");
                return new NavConfig()
                {
                    Ip = xmldocSelect.Attributes["ip"].InnerText,
                    Port = xmldocSelect.Attributes["port"].InnerText.ToInt32(0),
                    Type = xmldocSelect.Attributes["type"].InnerText
                };
            }
        }
        /// <summary>
        /// RS232配置
        /// </summary>
        public static Rs232Config rs232Config
        {
            get
            {
                XmlDocument xmlDoc = new XmlDocument();
                xmlDoc.Load(configPath);
                XmlNode xmldocSelect = xmlDoc.SelectSingleNode("configs/Can");
                return new Rs232Config()
                {
                    PortName = xmldocSelect.Attributes["CanPortName"].InnerText,
                    BaudRate = xmldocSelect.Attributes["BaudRate"].InnerText.ToInt32(0),
                    Type_Adv = xmldocSelect.Attributes["type_adv"].InnerText,
                    Type = xmldocSelect.Attributes["type"].InnerText
                };
            }
        }
        /// <summary>
        /// PLC 配置
        /// </summary>
        public static PLCConfig plcConfig
        {
            get
            {
                XmlDocument xmlDoc = new XmlDocument();
                xmlDoc.Load(configPath);
                XmlNode xmldocSelect = xmlDoc.SelectSingleNode("configs/PLC");
                return new PLCConfig()
                {
                    Ip = xmldocSelect.Attributes["ip"].InnerText,
                    Port = xmldocSelect.Attributes["port"].InnerText.ToInt32(0),
                    LocalIP = xmldocSelect.Attributes["localIP"].InnerText
                };
            }
        }

        /// <summary>
        /// 保存配置到配置文件
        /// </summary>
        /// <param name="navConfig">导航仪配置</param>
        /// <param name="rs232Config">RS232COM口配置</param>
        /// <param name="plcConfig">PLC配置</param>
        /// <returns>保存成功返回True</returns>
        public static bool Save(NavConfig navConfig, Rs232Config rs232Config, PLCConfig plcConfig)
        {
            if (navConfig == null && rs232Config == null && plcConfig == null)
            {
                return false;
            }
            try
            {
                XmlDocument xmlDoc = new XmlDocument();
                xmlDoc.Load(configPath);
                if (navConfig != null)
                {
                    XmlNode xmldocSelect = xmlDoc.SelectSingleNode("configs/Nav");
                    xmldocSelect.Attributes["ip"].InnerText = navConfig.Ip;
                    xmldocSelect.Attributes["port"].InnerText = navConfig.Port.ToString();
                }
                if (rs232Config != null)
                {
                    XmlNode xmldocSelect = xmlDoc.SelectSingleNode("configs/Can");
                    xmldocSelect.Attributes["CanPortName"].InnerText = rs232Config.PortName;
                    xmldocSelect.Attributes["BaudRate"].InnerText = rs232Config.BaudRate.ToString();
                }
                if (plcConfig != null)
                {
                    XmlNode xmldocSelect = xmlDoc.SelectSingleNode("configs/PLC");
                    xmldocSelect.Attributes["ip"].InnerText = plcConfig.Ip;
                    xmldocSelect.Attributes["port"].InnerText = plcConfig.Port.ToString();
                }
                xmlDoc.Save(@"Configs\AGVConfig.xml");
                return true;
            }
            catch (Exception ex)
            {
                Logger.Error("保存硬件配置文件失败.", ex);
                return false;
            }
        }
        /// <summary>
        /// 加载AGV配置信息
        /// </summary>
        public static AGVConfig agvConfig
        {
            get
            {
                XmlDocument xmlDoc = new XmlDocument();
                xmlDoc.Load(configPath);
                XmlNode xmldocSelect = xmlDoc.SelectSingleNode("configs/AGV");
                return new AGVConfig()
                {
                    AGVLenth = xmldocSelect.Attributes["length"].InnerText.ToInt32(0),
                    m_nZeroDQC = xmldocSelect.Attributes["m_nZeroDQC"].InnerText.ToInt32(0),
                    angel_QC = xmldocSelect.Attributes["angel_QC"].InnerText.ToInt32(0)
                };
            }
        }

    }
}
