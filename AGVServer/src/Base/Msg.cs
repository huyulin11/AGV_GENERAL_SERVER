using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GiatiaAGV.Base
{
    public class Msg
    {
        /// <summary>没有任何错误 0</summary>
        public static readonly int OK = 0;
        public static readonly int CONNECT_NAV_FAILED = 0x000001;
        public static readonly int NAV_FAILED_TOO_MANY = 0x000002;
        public static readonly int WAGO_READ_FAILED = 0x000003;
        public static readonly int WAGO_WRITE_FAILED = 0x000004;
        public static readonly int WAGO_RESP_FAILED = 0x000005;
        public static readonly int WAGO_RESP_UNKNOWN_ERROR = 0x000006;
        public static readonly int WAGO_CONNECT_FAILED = 0x000007;

        public static readonly int NAV_IP_FORMT_WRONG = 0x010002;
        public static readonly int NAV_INVALID_PORT = 0x010003;
        public static readonly int NAV_SENDTELEX_FAILED = 0x010004;
        public static readonly int NAV_PARSEPOSTIONTELEX = 0x010005;
        public static readonly int NAV_READTELEX_WRONG = 0x010006;
        public static readonly int NAV_FOUNDSTRAT_WRONG = 0x010007;
        public static readonly int NAV_FOUNDEND_WRONG = 0x010008;
        public static readonly int NAV_POS_DATA_LEN_MIN_WRONG = 0x010009;
        public static readonly int NAV_POSDATA_WRONG = 0x01000A;
        public static readonly int NAV_POS_DATA_LEN_WRONG = 0x01000B;
        public static readonly int NAV_CONNECT_FAILED = 0x01000C;
        public static readonly int NAV_ERR_RESP_AFTER_EXECUTION = 0x01000D;
        public static readonly int NAV_ERR_RESP_OF_GETPOS = 0x01000E;

        public static readonly int VERR_SETENABLESTATEF_FAILED = 0x01000F;
        public static readonly int VEER_SETDISABLESTARE_FAILED = 0x010010;
        public static readonly int VEER_HOMEMODETOFINDLIMISWICH_FAILED = 0x010011;
        public static readonly int VEER_HOME_FAILED = 0x010012;
        public static readonly int VEER_MOVETOPOSITIO_FAILED = 0x010013;
        public static readonly int VEER_READANGLE_FAILED = 0x010014;
        public static readonly int VEER_NEW_DM_FAILED = 0x010015;
        public static readonly int VEER_FIND_LIMIT_SWITCH_FAILED = 0x010016;
        public static readonly int VEER_GETINPUT_FAILED = 0x010017;
        public static readonly int VEER_MOVE2POSITION_FAILED = 0x010018;
        public static readonly int VEER_READ_ANGLE_FAILED = 0x010019;
        public static readonly int VEER_SET_ENABLE_STATE_FAILED = 0x01001A;
        public static readonly int VEER_SETDISABLE_FAILED = 0x01001B;
        public static readonly int VEER_READANGLE_EXCEPTION = 0x01001C;
        public static readonly int VEER_MOVE2POSITION_EXCEPTION = 0x01001D;
        public static readonly int VEER_READ_ANGLE_FAULT = 0x01001E;
        public static readonly int VEER_TOO_MANY_FAULT = 0x01001F;
        public static readonly int VEER_CLEAR_FAILED = 0x010020;
        public static readonly int VEER_ACTIVE_FAILED = 0x010021;
        public static readonly int VEER_ACTIVE_HOMING_FAILED = 0x010022;
        public static readonly int VEER_START_HOME_FAILED = 0x010023;
        public static readonly int VEER_SET_ENABLE_FAILED = 0x010024;
        public static readonly int VEER_ACTIVE_PPM_FAILED = 0x010025;
        public static readonly int VEER_SET_ANGLE_RESP_FAILED = 0x010026;
        public static readonly int VEER_SET_ABSOLUTE_RESP_FAILED = 0x010027;

        public static readonly int CALC_THREEPOINT_FAILED = 0x030002;
        public static readonly int USETHREETOCALC_TARGET_FAILED = 0x030003;
        public static readonly int CAN_INIT_FAILED = 0x030004;
        public static readonly int CAN_SETUP_ERR = 0x030005;
        public static readonly int CAN_CLOSE_ERR = 0x030006;
        public static readonly int CAN_OPEN_ERR = 0x030007;
        public static readonly int WALK_SEND_ERR = 0x030008;
        public static readonly int CAN_WRITE_ERR = 0x030009;
        public static readonly int CAN_READ_ERR = 0x03000A;
        public static readonly int TRACKING_TRAJECTORY_ERR = 0x03000B;

        /// <summary>
        /// 一般性错误
        /// </summary>
        public static readonly int ERROR = -1;

        public static bool FAILED(int e)
        {
            return e != OK;
        }

        public static bool SUCCESS(int e)
        {
            return e == OK;
        }
    }
}
