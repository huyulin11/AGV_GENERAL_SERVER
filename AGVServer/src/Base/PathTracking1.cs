using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

using GiatiaAGV.Model;
using GiatiaAGV.Base;
using GiatiaAGV.Vehicle;

namespace GiatiaAGV.Base
{
    /// <summary>
    /// 路径追踪
    /// </summary>
    public class PathTracking1
    {
        /// <summary>
        /// 
        /// </summary>
        private static WalkerTask _task;

        private static TerminalInfo _terminals;
        private int _TaskIndex;
        private int _PathIndex;

        private terminalInfo terminalNow;

        private static NavConfig navConfig;
        private static Nav nav;
        private static FrmMain _frm;

        private static AGVConfig agvConfig;

        private static double AGVLength;
        /// <summary>
        /// 逼近减速最小速度
        /// </summary>
        private static int approving_speed_min = 200;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="frm">主窗体</param>
        public PathTracking1(FrmMain frm)
        {
            if (navConfig == null)
                navConfig = Config.navConfig;
            if (nav == null)
                nav = new Nav(navConfig.Ip, navConfig.Port);
            if (agvConfig == null)
                agvConfig = Config.agvConfig;
            _frm = frm;

            AGVLength = agvConfig.AGVLenth;


        }

        /// <summary>
        /// 计算角度
        /// </summary>
        /// <param name="pNow">导航仪位置</param>
        /// <param name="pTarget">目标位置路径点</param>
        /// <param name="action">前进、后退</param>
        /// <returns>角度</returns>
        private double CalcAngel(NavPosition pNow, PathInfo pTarget, int action)
        {
            double x1 = pNow.X;
            double y1 = pNow.Y;
            float phi = pNow.Phi / 1000F;
            int x2 = pTarget.X;
            int y2 = pTarget.Y;

            //当前点与目标点的距离
            double L = Math.Sqrt(Math.Abs(Math.Pow(x2 - x1, 2)) + Math.Abs(Math.Pow(y2 - y1, 2)));

            float absPhi = Math.Abs(phi);
            if (action == Constant.WALKER_前进)
            {
                if (x2 >= x1 && y2 >= y1)
                {
                    if (90 <= absPhi && absPhi < 270)
                    {
                        return (180 - phi) + Math.Acos((x2 - x1) / L) * 180 / Math.PI;
                    }

                    if (0 <= absPhi && absPhi < 90)
                    {
                        return (-90 - phi) - Math.Acos((y2 - y1) / L) * 180 / Math.PI;
                    }

                    if (270 <= absPhi && absPhi < 360)
                    {
                        return (180 - phi) - Math.Acos((y2 - y1) / L) * 180 / Math.PI;
                    }
                }
                if (x2 > x1 && y2 < y1)
                {
                    if (180 <= absPhi && absPhi < 270)
                    {
                        return (90 - phi) - Math.Acos((y1 - y2) / L) * 180 / Math.PI;
                    }

                    if (0 <= absPhi && absPhi < 180)
                    {
                        return (90 - phi) + Math.Acos((y1 - y2) / L) * 180 / Math.PI;
                    }


                    if (270 <= absPhi && absPhi < 360)
                    {
                        return (450 - phi) + Math.Acos((y1 - y2) / L) * 180 / Math.PI;
                    }
                }
                if (x2 < x1 && y2 >= y1)
                {
                    if (90 <= absPhi && absPhi < 360)
                    {
                        return (270 - phi) + Math.Acos((y2 - y1) / L) * 180 / Math.PI;
                    }

                    if (0 <= absPhi && absPhi < 90)
                    {
                        return (-phi - 90) + Math.Acos((y2 - y1) / L) * 180 / Math.PI;
                    }
                }
                if (x2 < x1 && y2 < y1)
                {
                    if (0 <= absPhi && absPhi < 270)
                    {
                        return (90 - phi) - Math.Acos((y1 - y2) / L) * 180 / Math.PI;
                    }

                    if (270 <= absPhi && absPhi < 360)
                    {
                        return (450 - phi) - Math.Acos((y1 - y2) / L) * 180 / Math.PI;
                    }
                }
            }
            /************************************************************************/
            if (action == Constant.WALKER_后退)
            {
                y1 = y1 + AGVLength * Math.Sin(phi);
                x1 = x1 + AGVLength * Math.Cos(phi);
                if (x2 >= x1 && y2 >= y1)
                {
                    if (0 <= absPhi && absPhi < 180)
                    {
                        return phi - Math.Acos((x2 - x1) / L) * 180 / Math.PI;
                    }

                    if (270 <= absPhi && absPhi < 360)
                    {
                        return (phi - 360) - Math.Acos((x2 - x1) / L) * 180 / Math.PI;
                    }

                    if (180 <= absPhi && absPhi < 270)
                    {
                        return Math.Acos((x2 - x1) / L) * 180 / Math.PI - phi;
                    }


                }

                if (x2 < x1 && y2 >= y1)
                {
                    if (0 <= absPhi && absPhi < 90)
                    {
                        return (phi - 180) + Math.Acos((x1 - x2) / L) * 180 / Math.PI;
                    }

                    if (270 <= absPhi && absPhi < 360)
                    {
                        return (540 - phi) - Math.Acos((x1 - x2) / L) * 180 / Math.PI;
                    }

                    if (180 <= absPhi && absPhi < 270)
                    {
                        return (phi - 180) + Math.Acos((x1 - x2) / L) * 180 / Math.PI;
                    }

                    if (90 <= absPhi && absPhi < 180)
                    {
                        return (phi - 180) + Math.Acos((x1 - x2) / L) * 180 / Math.PI;
                    }
                }

                if (x2 < x1 && y2 < y1)
                {
                    if (0 <= absPhi && absPhi < 90)
                    {
                        return phi + 90 + Math.Acos((y1 - y2) / L) * 180 / Math.PI;
                    }

                    if (180 <= absPhi && absPhi < 360)
                    {
                        return (phi - 270) + Math.Acos((y1 - y2) / L) * 180 / Math.PI;
                    }

                    if (90 <= absPhi && absPhi < 180)
                    {
                        return (phi - 180) - Math.Acos((x1 - x2) / L) * 180 / Math.PI;
                    }
                }
                if (x2 > x1 && y2 < y1)
                {
                    if (0 <= absPhi && absPhi < 90)
                    {
                        return phi + Math.Acos((x2 - x1) / L) * 180 / Math.PI;
                    }

                    if (270 <= absPhi && absPhi < 360)
                    {
                        return phi + Math.Acos((x2 - x1) / L) * 180 / Math.PI;
                    }
                    if (180 <= absPhi && absPhi < 270)
                    {
                        return (phi - 360) + Math.Acos((x2 - x1) / L) * 180 / Math.PI;
                    }

                    if (90 <= absPhi && absPhi < 180)
                    {
                        return (450 - phi) + Math.Acos((x2 - x1) / L) * 180 / Math.PI;
                    }
                }


            }
            return 0;
        }

        /// <summary>
        /// 计算行走
        /// </summary>
        /// <param name="errorMsg">错误信息</param>
        /// <returns>错误返回-1，成功返回0</returns>
        //private int Calc(out string errorMsg)
        //{
        //    var targetPath = _task.walkParams.Data[_TaskIndex].PathInfo.Rows[_PathIndex];
        //    //减速速度
        //    int _speed = terminalNow.delcelerate_V > approving_speed_min ? terminalNow.delcelerate_V : approving_speed_min;

        //    int x2 = targetPath.X;
        //    int y2 = targetPath.Y;

        //    errorMsg = string.Empty;
        //    //行走方向 TODO
        //    var walkFX = _task.walkParams.Data[_TaskIndex].direction == 0 ? Constant.WALKER_前进 : Constant.WALKER_后退;

        //    do
        //    {
        //        NavPosition pNow = nav.GetPositon();

        //        if (pNow == null)
        //        {
        //            errorMsg = "获取坐标信息出错，坐标系统出现故障！";
        //            return -1;
        //        }
        //        double x1 = pNow.X;
        //        double y1 = pNow.Y;
        //        float phi = pNow.Phi / 1000F;
        //        var dir = _task.walkParams.Data[_TaskIndex].dir;
        //        var da = Math.Abs(dir == "x" ? x2 - x1 : y2 - y1);

        //        int Angel = (int)CalcAngel(pNow, targetPath, walkFX);
        //        _frm.LogMessage("自动行走计算角度值：" + Angel);
        //        if (Angel > 90)
        //        {
        //            Angel = 90;
        //        }
        //        if (Angel <= -90)
        //            Angel = -90;
        //        //当前点与目标点的距离
        //        double L = Math.Sqrt(Math.Abs(Math.Pow(x2 - x1, 2)) + Math.Abs(Math.Pow(y2 - y1, 2)));

        //        //逼近目标点
        //        if (_TaskIndex == _task.walkParams.Data[_TaskIndex].PathInfo.Rows.Count - 1)
        //        {
        //            //到达停站误差范围
        //            if (da <= terminalNow.range)
        //            {
        //                _frm.SetWalk(Constant.WALKER_停止, 0);
        //                _frm.SetAngel(0);
        //                //开始执行动作
        //                var stepsData = _task.walkParams.Data[_TaskIndex].steps.Data;
        //                foreach (var item in stepsData)
        //                {
        //                    switch (item.type)
        //                    {
        //                        case Constant.AGVActions.WALK_CYC:
        //                            _frm.SetAngel(90);
        //                            _frm.SetWalk(Constant.WALKER_前进, 200);
        //                            do
        //                            {
        //                                NavPosition _p = nav.GetPositon();
        //                                if (_p.Phi <= 10)
        //                                {
        //                                    _frm.SetWalk(Constant.WALKER_前进, 100);
        //                                }
        //                                if (_p.Phi == 0)
        //                                {
        //                                    _frm.SetWalk(Constant.WALKER_停止, 0);
        //                                    break;
        //                                }
        //                            } while (true);
        //                            break;
        //                    }
        //                }
        //                break;
        //            }
        //            if (da <= 0 && L <= 0)
        //            {
        //                //停止或后退，并报警
        //                _frm.SetWalk(Constant.WALKER_停止, 0);
        //                errorMsg = "停站位置错误，AGV超过了目标点";
        //                return -1;
        //            }
        //            //到达减速距离
        //            if (L < terminalNow.delcelerate_L)
        //            {

        //                _frm.SetAngel(Angel);
        //                _frm.SetWalk(walkFX, _speed);
        //                //发角度，速度
        //                //减速速度来自于调试数据
        //            }

        //        }
        //        else
        //        {
        //            if (dir == "x" && x2 - x1 <= 0)
        //            {
        //                break;
        //            }
        //            if (dir == "x" && y2 - y1 <= 0)
        //            {
        //                break;
        //            }
        //            //获取下一节点
        //            if (L < targetPath.V * 1)
        //            {
        //                break;
        //            }
        //            _frm.SetAngel(Angel);
        //            _frm.SetWalk(walkFX, targetPath.V);
        //        }
        //    } while (true);
        //    return 0;
        //}


        public string AutoWalker(WalkerTask task, TerminalInfo terminals)
        {
            string msg = string.Empty;
            _task = task;
            _terminals = terminals;
            if (_task == null || _task.walkParams == null || _task.walkParams.Data.Count == 0
                || _task.steps == null || _task.steps.Data.Count == 0)
            {
                return "任务文档有错误，请检查任务数据。";
            }
            if (_terminals == null || _terminals.terminals == null || _terminals.terminals.Count == 0)
            {
                return "没有站点数据，请配置站点信息。";
            }

            for (int i = 0; i < _task.steps.Data.Count; i++)
            {
                //int tid = _task.walkParams.Data[i].terminal;

                var steptemp = _task.steps.Data.Where(x => x.id == (i + 1)).FirstOrDefault();
                if (steptemp == null)
                {
                    //完成
                    return string.Empty;
                }

                //确定step任务类型
                switch (steptemp.type)
                {
                    case Constant.AGVActions.FORK_UP: //货叉提升
                    case Constant.AGVActions.FORK_DOWN: //货叉下降
                    case Constant.AGVActions.FORK_INIT: //货叉回原点

                        if (steptemp.number2 == 0)// 延时
                        {
                            _frm._plc.ForkAction(0, 0, steptemp.type, steptemp.number1);
                        }
                        else //指定高度提升或者回原点
                        {
                            _frm._plc.ForkAction(steptemp.number1, steptemp.number2, steptemp.type, 10000);
                        }
                        break;

                    case Constant.AGVActions.WALK_CYC: //转角

                        _frm.SetAngel(90);
                        _frm.SetWalk(Constant.WALKER_前进, 1000);

                        while (Math.Abs(steptemp.number1 - _frm.NavPosition.Phi) > 1)//误差1度
                        {
                            if (Math.Abs(steptemp.number1 - _frm.NavPosition.Phi) < 10)//小于10度减速
                            {
                                _frm.SetWalk(Constant.WALKER_前进, 300);

                            }
                            Thread.Sleep(125);
                        }

                        _frm.SetWalk(Constant.WALKER_停止, 0);
                        _frm.SetAngel(0);

                        break;
                    case Constant.AGVActions.WALK_BACK: //后退
                    //break;
                    case Constant.AGVActions.WALK: //前进
                        //steptemp.id
                        var wp = _task.walkParams.Data.Where(x => x.stepid == steptemp.id).FirstOrDefault();
                        if (wp == null)
                        {
                            return "配置不这确setp 无 wp对应";
                        }

                        var temi = _terminals.terminals.Where(x => x.id == wp.terminal).FirstOrDefault();//站点
                        if (temi == null)
                        {
                            return "配置不这确无站点与wp对应";
                        }

                        while (true)
                        {
                            //下一个路径点
                            PathInfo next = wp.PathInfo.Rows[0];
                            if (temi.dir == "x")//延X轴
                            {
                                for (int w = 0; w < wp.PathInfo.Rows.Count; w++)
                                {
                                    if (w != wp.PathInfo.Rows.Count - 1)
                                    {

                                    }
                                    else
                                    {
                                        int s = wp.PathInfo.Rows[w].X;
                                        int e = wp.PathInfo.Rows[w + 1].X;

                                        int min = Math.Min(s, e);
                                        int max = Math.Min(s, e);
                                        if (min < _frm.NavPosition.X && max > _frm.NavPosition.X)
                                        {
                                            next = wp.PathInfo.Rows[w + 1];
                                            break;
                                        }
                                    }

                                }
                            }
                            else //延y轴
                            {
                                for (int w = 0; w < wp.PathInfo.Rows.Count; w++)
                                {
                                    if (w != wp.PathInfo.Rows.Count - 1)
                                    {

                                    }
                                    else
                                    {
                                        int s = wp.PathInfo.Rows[w].Y;
                                        int e = wp.PathInfo.Rows[w + 1].Y;

                                        int min = Math.Min(s, e);
                                        int max = Math.Min(s, e);
                                        if (min < _frm.NavPosition.Y && max > _frm.NavPosition.Y)
                                        {
                                            next = wp.PathInfo.Rows[w + 1];
                                            break;
                                        }
                                    }

                                }
                            }
                            double distance = Math.Sqrt(Math.Abs(Math.Pow(next.X - _frm.NavPosition.X, 2)) + Math.Abs(Math.Pow(next.Y - _frm.NavPosition.Y, 2)));

                            if (distance < 1000 && next.id != wp.PathInfo.Rows.Max(x => x.id))
                            {
                                next = wp.PathInfo.Rows.Where(x => x.id == next.id + 1).FirstOrDefault();
                            }

                            int work = steptemp.type == Constant.AGVActions.WALK_BACK ? Constant.WALKER_后退 : Constant.WALKER_前进;



                            //当前点与目标点的距离
                            double L = Math.Sqrt(Math.Abs(Math.Pow(temi.x - _frm.NavPosition.X, 2)) + Math.Abs(Math.Pow(temi.y - _frm.NavPosition.Y, 2)));

                            int Angel = (int)CalcAngel(_frm.NavPosition, next, work);
                            _frm.SetAngel(Angel);
                            if (wp.closing.ToLower() == "true")
                            {
                                if (L < temi.delcelerate_L)
                                {
                                    _frm.SetWalk(work, temi.delcelerate_V);
                                }
                                else
                                {
                                    _frm.SetWalk(work, next.V);
                                }
                            }
                            else
                            {
                                _frm.SetWalk(work, next.V);
                            }

                            if (wp.stop.ToLower() == "true")
                            {
                                if (L < temi.range)
                                {
                                    _frm.SetWalk(Constant.WALKER_停止, 0);
                                }
                            }
                            else
                            {
                                if (L < temi.range)
                                {
                                    break;
                                }
                            }
                            Thread.Sleep(125);
                        }
                        //_frm.SetAngel(0);
                        break;
                    default:
                        break;


                }
            }
            return msg;
        }

        /// <summary>
        /// 逼近减速
        /// </summary>
        /// <returns></returns>
        public double ClosingDelcelerate(double v, double md)
        {
            if (v <= approving_speed_min)
                return approving_speed_min;
            return 200;

        }

    }
}
