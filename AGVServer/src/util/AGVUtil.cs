using System;

using System.Net.Sockets;
using System.Net.NetworkInformation;
using System.Collections.Generic;
using System.Diagnostics;
using AGV.task;
using AGV.forklift;
using AGV.message;
using AGV.dao;

namespace AGV.util {
	/// <summary>
	/// 提供一些公用方法
	/// </summary>
	public class AGVUtil {

		public AGVUtil() {
		}

		public static int Count(int x, int y)
		{
			int result;
			if (y == 0) {
				return 1;
			}
			result = x * Count(x, y - 1);
			return result;
		}



		/// <summary>
		/// 获取本地以太网卡地址
		/// </summary>
		/// <returns>ip地址 format: xxx.xxx.xxx.xxx</returns>
		public static string getEnteherIP() {
			NetworkInterface[] nics = NetworkInterface.GetAllNetworkInterfaces();
			foreach (NetworkInterface adapter in nics) {
				//判断是否为以太网卡
				//Wireless80211         无线网卡    Ppp     宽带连接
				//Ethernet              以太网卡   
				//这里篇幅有限贴几个常用的，其他的返回值大家就自己百度吧！
				if (adapter.NetworkInterfaceType == NetworkInterfaceType.Ethernet) {
					//获取以太网卡网络接口信息
					IPInterfaceProperties ip = adapter.GetIPProperties();
					//获取单播地址集
					UnicastIPAddressInformationCollection ipCollection = ip.UnicastAddresses;
					foreach (UnicastIPAddressInformation ipadd in ipCollection) {
						//InterNetwork    IPV4地址      InterNetworkV6        IPV6地址
						//Max            MAX 位址
						if (ipadd.Address.AddressFamily == AddressFamily.InterNetwork)
							//判断是否为ipv4
							return ipadd.Address.ToString();//获取ip
					}
				}
			}

			return null;
		}

		public static void addAllTaskRecord() {
			List<SingleTask> singleTaskList = AGVCacheData.getSingleTaskList();  //获取所有的任务
			foreach (SingleTask st in singleTaskList) {
				if (st.taskType == TASKTYPE_T.TASK_TYPE_UP_DILIVERY) {
					TaskRecord tr = new TaskRecord();
					tr.singleTask = st;
					tr.taskRecordName = st.taskName;
					tr.taskRecordStat = TASKSTAT_T.TASK_READY_SEND;
					TaskReordService.getInstance().addTaskRecord(tr);
				}
			}
		}

		public static bool setForkCtrl(ForkLiftWrapper fl, int ctrl) {
			string cmd = "cmd=pause;pauseStat=" + ctrl;
			int times = 0;
			while (times < 3) {
				try {
					fl.getAGVSocketClient().SendMessage(cmd);
					break;
				} catch {
					AGVMessage message = new AGVMessage();
					message.setMessageType(AGVMessageHandler_TYPE_T.AGVMessageHandler_SENDPAUSE_ERR);
					message.setMessageStr("发送中断错误");
					AGVMessageHandler.getMessageHandler().setMessage(message);
				}
				times++;
			}
			Console.WriteLine("setForkCtrl forklift " + fl.getForkLift().id + "cmd = " + cmd);
			AGVLog.WriteInfo("setForkCtrl forklift " + fl.getForkLift().id + "cmd = " + cmd, new StackFrame(true));
			return true;
		}

		public static bool setForkCtrlWithPrompt(ForkLiftWrapper fl, int ctrl) {
			string cmd = "cmd=pause;pauseStat=" + ctrl;
			try {
				fl.getAGVSocketClient().SendMessage(cmd);
			} catch {
				Console.WriteLine("setForkCtrlWithPrompt forklift " + fl.getForkLift().id + "cmd = " + cmd + "failed");
				AGVLog.WriteInfo("setForkCtrlWithPrompt forklift " + fl.getForkLift().id + "cmd = " + cmd + "failed", new StackFrame(true));
			}

			Console.WriteLine("setForkCtrlWithPrompt forklift " + fl.getForkLift().id + "cmd = " + cmd + "success");
			AGVLog.WriteInfo("setForkCtrlWithPrompt forklift " + fl.getForkLift().id + "cmd = " + cmd + "success", new StackFrame(true));
			return true;
		}

		/// <summary>
		/// 解析taskrecord的Name 1_1.xml货1_2.xml 解析得到1.xml
		/// </summary>
		/// <param name="taskName"></param>
		/// <returns></returns>
		public static string parseTaskRecordName(string taskName) {
			int pos = -1;
			pos = taskName.IndexOf("_");
			if (pos != -1) {
				return taskName.Substring(0, pos) + ".xml";
			}
			return null;
		}

		public static void disableForklift(int forkliftNumber) //不使用某辆车，用于设置楼上只用一辆车运行任务
		{
			foreach (ForkLiftWrapper fl in AGVCacheData.getForkLiftWrapperList()) {
				if (fl.getForkLift().forklift_number == forkliftNumber) {
					fl.getForkLift().isUsed = 0;
				}
			}
		}
	}
}
