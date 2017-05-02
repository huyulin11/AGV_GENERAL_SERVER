using System;

using System.Net.Sockets;
using System.Net;
using System.Net.NetworkInformation;
using System.Collections.Generic;
using System.Xml;
using System.Diagnostics;
/// <summary>
/// 提供一些公用方法
/// </summary>
/// 
using AGV;
namespace AGV.util   //不同的目录不同的命名空间
{
    public class AGVUtil
    {
        private List<TaskRecord> taskRecordList = null;
        private List<User> userList = null;
        private List<ForkLift> forkLiftList = null;

        public AGVUtil()
        {
        }

        public static int Count(int x, int y)//x的y次方
        {
            int result;
            if (y == 0)
            {
                return 1;
            }
            result = x * Count(x, y - 1);
            return result;
        }  



        /// <summary>
        /// 获取本地以太网卡地址
        /// </summary>
        /// <returns>ip地址 format: xxx.xxx.xxx.xxx</returns>
        public static string getEnteherIP()
        {
            NetworkInterface[] nics = NetworkInterface.GetAllNetworkInterfaces();
            foreach (NetworkInterface adapter in nics)
            {
                //判断是否为以太网卡
                //Wireless80211         无线网卡    Ppp     宽带连接
                //Ethernet              以太网卡   
                //这里篇幅有限贴几个常用的，其他的返回值大家就自己百度吧！
                if (adapter.NetworkInterfaceType == NetworkInterfaceType.Ethernet)
                {
                    //获取以太网卡网络接口信息
                    IPInterfaceProperties ip = adapter.GetIPProperties();
                    //获取单播地址集
                    UnicastIPAddressInformationCollection ipCollection = ip.UnicastAddresses;
                    foreach (UnicastIPAddressInformation ipadd in ipCollection)
                    {
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

        public List<TaskRecord> updateTaskRecordList()
        {
            string sql = "select * from taskrecord where taskRecordStat != 4 order by taskRecordStat desc, taskLevel desc";
            //检测当前车子可用的车子数量，如果有两台可弹出用的车子，正常运行，如果只有一台可用的车子，弹出提示，请工作人员确认，如果无可用的车子，直接退出程序
            //确认的逻辑，连接车子ip，如果可以正常连接车子ip，则表示车子可用
            List<TaskRecord> taskRecordList = AGVInitialize.getInitialize().getDBConnect().SelectTaskRecordBySql(sql);

            return taskRecordList;
        }

        public List<TaskRecord> SelectReadySendTask()
        {
            string sql = "select * from taskrecord where taskRecordStat = 1 order by taskRecordStat desc, taskLevel desc";
            //检测当前车子可用的车子数量，如果有两台可弹出用的车子，正常运行，如果只有一台可用的车子，弹出提示，请工作人员确认，如果无可用的车子，直接退出程序
            //确认的逻辑，连接车子ip，如果可以正常连接车子ip，则表示车子可用
            List<TaskRecord> taskRecordList = AGVInitialize.getInitialize().getDBConnect().SelectTaskRecordBySql(sql);

            return taskRecordList;
        }


        public List<TaskRecord> SelectTaskRecordBySingleTaskID(int singleTaskID)
        {
            string sql = "select * from taskrecord where singleTask = " + singleTaskID + " order by taskRecordStat desc, taskLevel desc";
            //检测当前车子可用的车子数量，如果有两台可弹出用的车子，正常运行，如果只有一台可用的车子，弹出提示，请工作人员确认，如果无可用的车子，直接退出程序
            //确认的逻辑，连接车子ip，如果可以正常连接车子ip，则表示车子可用
            List<TaskRecord>  taskRecordList = AGVInitialize.getInitialize().getDBConnect().SelectTaskRecordBySql(sql);

            return taskRecordList;
        }

        public void addAllTaskRecord()
        {
            List<SingleTask> singleTaskList = AGVInitialize.getInitialize().getSingleTaskList();  //获取所有的任务
            foreach(SingleTask st in singleTaskList)
            {
                if (st.taskType == TASKTYPE_T.TASK_TYPE_UP_DILIVERY)
                {
                    TaskRecord tr = new TaskRecord();
                    tr.singleTask = st;
                    tr.taskRecordName = st.taskName;
                    tr.taskRecordStat = TASKSTAT_T.TASK_READY_SEND;
                    AGVInitialize.getInitialize().getSchedule().addTaskRecord(tr);
                }       
            }
        }

        public void deleteCacheTaskRecord()
        {
            string sql = "truncate table taskrecord";
            AGVInitialize.getInitialize().getDBConnect().DeleteWithSql(sql);
        }

        public List<User> getUsers()
        {
            if (userList == null)
                userList = AGVInitialize.getInitialize().getDBConnect().SelectUserList();

            return userList;
        }

        /// <summary>
        /// 车子查询后，不能重新查询，整个运行周期只维护一个Forklift列表
        /// </summary>
        /// <returns></returns>
        public List<ForkLift> getForkLiftList()
        {
            if (forkLiftList == null)
                forkLiftList = AGVInitialize.getInitialize().getDBConnect().SelectForkList();

            return forkLiftList;
        }

        /// <summary>
        /// 发送任务
        /// </summary>
        /// <param name="tr"></param>
        /// <param name="fl"></param>
        public int sendTask(TaskRecord tr, ForkLift fl)
        {
            string cmd = null;

	        int result = 0;
            Console.WriteLine("ready to send task: " + tr.singleTask.taskName + "forklist stat:" + fl.taskStep + "forklift finished:" + fl.finishStatus);
            cmd = "cmd=set task by name;name=" + tr.taskRecordName; //发送命令格式，如果有多个对应值用;隔开，如果后面没有命令了，不需要再加;号
            Console.WriteLine("send msg :" + cmd + "to " + fl.forklift_number);

            lock (fl.tcpClient.clientLock)
            {
                try
                {
                    fl.tcpClient.SendMessage(cmd);  //确保发送成功

                    tr.taskRecordStat = TASKSTAT_T.TASK_SEND;
                    tr.singleTask.taskStat = TASKSTAT_T.TASK_SEND;
                    AGVInitialize.getInitialize().getMainFrm().updateFrm(); //设置更新界面
                    tr.forkLift = fl;
                    fl.taskStep = ForkLift.TASK_STEP.TASK_SENDED;
                    fl.currentTask = tr.singleTask.taskText;
                    AGVInitialize.getInitialize().getDBConnect().UpdateTaskRecord(tr);
                    AGVInitialize.getInitialize().getDBConnect().updateForkLift(fl);  //更新车子状态
                }catch(Exception ex)
                {
                    Console.WriteLine(ex.ToString());
                    AGVLog.WriteError("发送" + tr.singleTask.taskText + " 任务到" + fl.forklift_number + "号车 失败", new StackFrame(true));
		            result = -1;
                }

            }

            AGVLog.WriteError("发送" + tr.singleTask.taskText + " 任务到" + fl.forklift_number + "号车 成功", new StackFrame(true));
		    return result;
        }

        public bool setForkCtrl(ForkLift fl, int ctrl)
        {
            string cmd = "cmd=pause;pauseStat=" + ctrl;
            int times = 0;
            while (times < 3)
            {
                try
                {
                    fl.tcpClient.SendMessage(cmd);
                    break;
                }
                catch
                {
                    Message message = new Message();
                    message.setMessageType(AGVMESSAGE_TYPE_T.AGVMESSAGE_SENDPAUSE_ERR);
                    message.setMessageStr("发送中断错误");

                    AGVInitialize.getInitialize().getAGVMessage().setMessage(message);
                }

                times++;
            }


            Console.WriteLine("setForkCtrl forklift " + fl.id + "cmd = " + cmd);
            AGVLog.WriteInfo("setForkCtrl forklift " + fl.id + "cmd = " + cmd, new StackFrame(true));
            return true;
        }

        public bool setForkCtrlWithPrompt(ForkLift fl, int ctrl)
        {
            string cmd = "cmd=pause;pauseStat=" + ctrl;
            try
            {
               fl.tcpClient.SendMessage(cmd);
            }
            catch
            {
                Console.WriteLine("setForkCtrlWithPrompt forklift " + fl.id + "cmd = " + cmd +  "failed");
                AGVLog.WriteInfo("setForkCtrlWithPrompt forklift " + fl.id + "cmd = " + cmd + "failed" , new StackFrame(true));
            }

            Console.WriteLine("setForkCtrlWithPrompt forklift " + fl.id + "cmd = " + cmd + "success");
            AGVLog.WriteInfo("setForkCtrlWithPrompt forklift " + fl.id + "cmd = " + cmd + "success" , new StackFrame(true));
            return true;
        }

        /// <summary>
        /// 解析taskrecord的Name 1_1.xml货1_2.xml 解析得到1.xml
        /// </summary>
        /// <param name="taskName"></param>
        /// <returns></returns>
        public static string parseTaskRecordName(string taskName)
        {
            int pos = -1;
            pos = taskName.IndexOf("_");
            if (pos != -1)
            {
                return taskName.Substring(0, pos) + ".xml";
            }

            return null;
        }

        public void disableForklift(int forkliftNumber) //不使用某辆车，用于设置楼上只用一辆车运行任务
        {    
             foreach(ForkLift fl in forkLiftList)
             {
                 if (fl.forklift_number == forkliftNumber)
                 {
                     fl.isUsed = 0;
                 }
            }
            
        }
    }
}
