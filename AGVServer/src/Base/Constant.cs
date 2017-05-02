using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GiatiaAGV.Base
{
    public static class Constant
    {
        /// <summary>
        /// 行走动作定义
        /// </summary>
        public static class AGVActions
        {
            /// <summary>
            /// 前进
            /// </summary>
            public const string WALK = "WALK";
            /// <summary>
            /// 后退
            /// </summary>
            public const string WALK_BACK = "WALK_BACK";
            /// <summary>
            /// 转角
            /// </summary>
            public const string WALK_CYC = "WALK_CYC";
            /// <summary>
            /// 货叉提升
            /// </summary>
            public const string FORK_UP = "FORK_UP";
            /// <summary>
            /// 货叉下降
            /// </summary>
            public const string FORK_DOWN = "FORK_DOWN";
            /// <summary>
            /// 货叉停止
            /// </summary>
            public const string FORK_STOP = "FORK_STOP";
            /// <summary>
            /// 货叉回原点
            /// </summary>
            public const string FORK_INIT = "FORK_INIT";

        }

        /// <summary>
        /// AGV设备状态
        /// </summary>
        public class AGVStatus
        {
            /// <summary>
            /// 空闲
            /// </summary>
            public const int Idle = 0;
            /// <summary>
            /// 繁忙
            /// </summary>
            public const int Busy = 1;
            /// <summary>
            /// 故障
            /// </summary>
            public const int Fault = 2;

        }
        /// <summary>
        /// 最小日期
        /// </summary>
        public static DateTime MinDateTime = new DateTime(1970, 1, 1);

        /// <summary>
        /// 导航仪通信超时时间
        /// </summary>
        public static readonly int NAV_TIMEOUT = 300;


        /// <summary>
        /// 0x0FFFFFFF 无效的位置值，适用X，Y，Angle
        /// </summary>
        public static readonly int INVALID_POS_VALUE = 0x0FFFFFFF;

        /// <summary>-3600</summary>
        public static readonly int INVALID_ANGLE_VALUE = -3600;

        /// <summary>
        /// 3 Nav最大错误次数
        /// 如果Nav读取失败和数据逻辑错误累计超过这个值的话则不进行人工预测
        /// 而是直接进行故障报警
        /// </summary>
        public static readonly int NAV_MAX_FAILED_COUNT = 3;

        /// <summary>50 旋转编码器最大角度</summary>
        public static readonly int ANGLE_ENCODER_MAX_ANGLE = 50;

        /// <summary>-50 旋转编码器最小角度</summary>
        public static readonly int ANGLE_ENCODER_MIN_ANGLE = -50;

        /// <summary>50转角电机最大输入</summary>
        public static readonly int VEER_ANGLE_MAX = 50;

        /// <summary>-50 转角电机最小输入</summary>
        public static readonly int VEER_ANGLE_MIN = -50;

        /// <summary>1000</summary>
        public static readonly short WALKER_SPEED_MAX = 1000;

        /// <summary>2000.0</summary>
        public static readonly double WALKER_READ_INTERVAL_MAX = 2000.0;

        /// <summary>1000 滚筒电机最大速度</summary> 
        public static readonly short CONV_SPEED_MAX = 2000;

        /// <summary>8 一次CAN数据读取的最大帧数</summary>
        public static readonly int CAN_MAX_READ_COUNT = 8;

        // CAN 信息 id -----------------------------------------------
        /// <summary>1827 CAN 启动</summary>
        public static readonly uint CAN_MSGID_CAN_START = 1827;
        /// <summary>1828 CAN 启动</summary>
        public static readonly uint CAN_MSGID_CAN_START2 = 1828;

        /// <summary>419 行走电机</summary>
        public static readonly uint CAN_MSGID_WALKER_RESP = 419;

        /// <summary>387 旋转编码器</summary>
        public static readonly uint CAN_MSGID_ANGLE_ENCODER = 387;

        /// <summary> 420 滚筒</summary>
        public static readonly uint CAN_MSGID_DRUM_RESP = 420;
        /// <summary> 547</summary>
        public static readonly uint CAN_MSGID_WALKER = 547;
        /// <summary> 548</summary>
        public static readonly uint CAN_MSGID_CONV = 548;

        /// <summary>3 CAN状态未知标记</summary>
        public static readonly int CAN_STATUS_UNKNOWN = 3;

        /// <summary>502 IO 模块默认端口</summary>
        public static readonly int IO_MODULE_PORT = 502;

        /// <summary>9 WAGO反馈报文头部分长度</summary>
        public static readonly int WAGO_RESP_HEAD_LEN = 9;
        /// <summary>12</summary>
        public static readonly int WAGO_FC16_RESP_LEN = 12;
        /// <summary>7</summary>
        public static readonly int WAGO_RESP_RESULT_OFFSET = 7;
        /// <summary>8</summary>
        public static readonly int WAGO_RESP_DATALEN_OFFSET = 8;
        /// <summary>0x48</summary>
        public static readonly int WAGO_RESP_FAILED84 = 0x84;
        /// <summary>0x85</summary>
        public static readonly int WAGO_RESP_FAILED85 = 0x85;
        /// <summary>0x04</summary>
        public static readonly int WAGO_FC4 = 0x04;
        /// <summary>0x10</summary>
        public static readonly int WAGO_FC16 = 0x10;

        /// <summary>
        /// 3 IO采集到信号的 字长度
        /// </summary>
        public static readonly ushort MANUAL_INPUT_WORD_COUNT = 3;

        /// <summary>26201</summary>
        public static readonly int MANUAL_SPEED_DATA_ANALOG_DEFAULT_K = 26201;
        /// <summary>879</summary>
        public static readonly int MANUAL_MAX_SPEED_INPUT = 879;

        // 开关量位标记
        /// <summary>0x0001</summary>
        public static readonly ushort MANUAL_SWVBIT_FORWARD = 0x0001;
        /// <summary>0x0002</summary>
        public static readonly ushort MANUAL_SWVBIT_REVERSE = 0x0002;
        /// <summary>0x0004</summary>
        public static readonly ushort MANUAL_SWVBIT_COLLISION_ALARM = 0x0004;
        /// <summary>0x0008</summary>
        public static readonly ushort MANUAL_SWVBIT_GUNTONG_START = 0x0008;
        /// <summary>0x0010</summary>
        public static readonly ushort MANUAL_SWVBIT_GUNTONG_JIANSU = 0x0010;
        /// <summary>0x0020</summary>
        public static readonly ushort MANUAL_SWVBIT_GUNTONG_STOP = 0x0020;
        /// <summary>0x0080</summary>
        public static readonly ushort MANUAL_SWVBIT_MANUAL_SWITCH = 0x0040;
        /// <summary>0x0080</summary>
        public static readonly ushort MANUAL_DROP_LEAV_SIGNAL = 0x80;

        /// <summary>
        /// 1 手柄速度输入offset
        /// </summary>
        public static readonly ushort MANUAL_SPEED_OFFSET = 0;

        /// <summary>2112</summary>
        public static readonly int NAV_DEFAULT_PORT = 2112;
        /// <summary>(char)2</summary>
        public static readonly char NAV_TELEX_START = (char)2;
        /// <summary>(char)3</summary>
        public static readonly char NAV_TELEX_END = (char)3;
        /// <summary>6</summary>
        public static readonly int NAV_POS_DATA_LEN_MIN = 6;
        /// <summary>9</summary>
        public static readonly int NAV_POS_DATA_LEN = 9;
        /// <summary>1</summary>
        public static readonly string NAV_WAIT = "1";

        // Veer超时、波特率
        /// <summary>300</summary>
        public static readonly uint VEER_TIMEOUT = 300;
        /// <summary>115200</summary>
        public static readonly uint VEER_BAUDRATE = 115200;
        /// <summary>100</summary>
        public static readonly uint VEER_HOMINGACCELERATION = 100;
        /// <summary>10</summary>
        public static readonly uint VEER_SPEEDSWITCH = 10;
        /// <summary>1</summary>
        public static readonly uint VEER_SPEEDINDEX = 1;
        /// <summary>34</summary>
        public static readonly uint VEER_PROFILEVELOCITY = 34;
        /// <summary>268</summary>
        public static readonly uint VEER_PROFILEACCELERATION = 268;
        /// <summary>268</summary>
        public static readonly uint VEER_PROFILEDECELERATION = 268;

        /// <summary>
        /// 行走电机停止运动
        /// </summary>
        public static readonly int WALKER_停止 = 0;
        /// <summary>
        /// 行走电机前进
        /// </summary>
        public static readonly int WALKER_前进 = 1;
        /// <summary>
        /// 行走电机后退
        /// </summary>
        public static readonly int WALKER_后退 = 2;

        /// <summary>
        /// 21 手动角度采集的比例关系
        /// </summary>
        public static readonly int HAND_VEER_RATIO = 21;
        /// <summary>2062943</summary>
        public static readonly int HANDL_VEER_DATA_ANALOG_DEFAULT_K = 2062943;

        /// <summary>0</summary>
        public static readonly int VEER_HOMEOFFSET = 0;
        /// <summary>2300</summary>
        public static readonly int VEER_HOMEPOSITION = 2300;
        /// <summary>262145</summary>
        public static readonly int VEER_KEYHANDLE = 262145;
        /// <summary>0</summary>
        public static readonly int VEER_NEGATIVELIMIT = 0;
        /// <summary>1000</summary>
        public static readonly int VEER_CONSTANASECOND = 1000;
        /// <summary>0</summary>
        public static readonly int VEER_TARGETPOSITION = 0;
        /// <summary>EPOS2</summary>
        public static readonly string VEER_DEVICENAME = "EPOS2";
        /// <summary>MAXON_RS232</summary>
        public static readonly string VEER_PROTOCOLSTACKNAME = "MAXON_RS232";
        /// <summary>RS232</summary>
        public static readonly string VEER_INRTERFACENAME = "RS232";
        /// <summary>COM1</summary>
        public static readonly string VEER_PORTNAME = "COM1";
        /// <summary>true</summary>
        public static readonly bool VEER_ABSOLUTEMOVE = true;
        /// <summary>1</summary>
        public static readonly ushort VEER_NODEID = 1;
        /// <summary>100</summary>
        public static readonly ushort VEER_CURRENTTRESHOLD = 100;
        /// <summary>0</summary>
        public static readonly ushort VEER_CONSTANZERO = 0;
        /// <summary>2</summary>
        public static readonly ushort VEER_CONSTANTWO = 2;
        /// <summary>1537</summary>
        public static readonly ushort VEER_CAN_ID = 0x601;
        /// <summary>1537</summary>
        public static readonly ushort VEER_CAN_RESP_ID = 0x581;

        /// <summary>0x0001</summary>
        public static readonly ushort IO_OUT_RED_LIGHT = 0x0001;
        /// <summary>0x0002</summary>
        public static readonly ushort IO_OUT_GREEN_LIGHT = 0x0002;
        /// <summary>0x0040</summary>
        public static readonly ushort IO_OUT_PICK = 0x0040;

        /// <summary>400</summary>
        public static readonly int START_GREEN_LIGHT_TIME = 4000;

        /// <summary>800</summary>
        public static readonly ushort CONV_START_SPEED = 800;
        /// <summary>100</summary>
        public static readonly ushort CONV_SLOW_SPEED = 200;
        /// <summary>800.0</summary>
        public static readonly double EQUIDISTANCE_VALUE = 800.0;
        /// <summary>800</summary>
        public static readonly ushort CONV_DROP_SPEED = 800;
        /// <summary>200</summary>
        public static readonly ushort CONV_DROP_SLOW_SPEED = 200;
    }
}
