using System;
using System.Threading;
using System.Windows.Forms;
using System.Diagnostics;

using System.Runtime.InteropServices;

using AGV.util;
using AGV.schedule;
using AGV.init;
namespace AGV.message {
	/// <summary>
	/// 用于处理系统消息，并弹出提示框显示
	/// </summary>
	///         
	public enum AGVMessageHandler_TYPE_T {
		AGVMessageHandler_MIN = 0,
		AGVMessageHandler_LOWPOWER,
		AGVMEASAGE_LIFT_UPDOWN, //升降机楼上楼下都有货
		AGVMessageHandler_LIFT_COM,  //升降机端口问题

		AGVMessageHandler_LIFT_BUG,  //升降机 卡货

		AGVMessageHandler_SENDTASK_ERR,
		AGVMessageHandler_SENDPAUSE_ERR,

		AGVMessageHandler_NET_ERR, //网络错误

		AGVMessageHandler_AGV_ALARM, //防撞信号
		AGVMessageHandler_MAX,
	};
}
