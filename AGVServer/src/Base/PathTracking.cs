using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GiatiaAGV.Base
{
    public class POINT
    {
        public long X;
        public long Y;
    }
    // 路径点
    public class PATH_POINT
    {
        public int id;
        public double x;
        public double y;
        public double a;
        public double v;
    }
    // 站点
    public class TERMINAL_POINT
    {
        public int id;
        public POINT terminal = new POINT();
        public sbyte dir;
        public double range;
    }
    // 车体形状 -------------------------------------
    public class VEHICLE_SHAPE // 形状
    {
        public int AheadNav; // 导航仪之前的车长
        public int BehindNav; // 导航仪之后的车长
        public int LeftNav; // 导航仪左侧的车宽
        public int RightNav; // 导航仪右侧的车宽
    }

    public class VEHICLE_BODY_PARAMETER
    {
        public int WheelSpace; // 前后轮距离
        public int ForkOffset; // 后货叉中心点相对导航仪偏移量
        public int FrontWheelOffset; // 前轮相对导航仪偏移量
        public VEHICLE_SHAPE Shape = new VEHICLE_SHAPE();
    }

    // 车体部分位置 ---------------------------------
    public class VEHICLE_BODY_POSITION
    {
        public POINT frontWheel = new POINT(); // 前轮位置
        public POINT fork = new POINT(); // 货叉中心点位置
        public int nav_x; // 导航仪x值
        public int nav_y; // 导航仪y值
        public float nav_angle; // 导航仪角度值
    }


    // 路径跟踪输入参数
    public class TRACK_PARAM
    {
        public PATH_POINT[] PathPoints; // 行走路径
        public int PathPointsCount; // 行走路径点个数
        public TERMINAL_POINT Terminal = new TERMINAL_POINT(); // 站点
        public int ClosingDelcelerate; // 到站是否减速, 如果为FALSE 到站后只是表示本次行走结束,并不进行减速
        public int TrackDirection; // TRACK_FORWARD, TRACE_FORWARD_REVERSE, TRACK_BACK
        public VEHICLE_BODY_POSITION Position = new VEHICLE_BODY_POSITION(); // 车辆位置
        public VEHICLE_BODY_PARAMETER VclBody = new VEHICLE_BODY_PARAMETER(); // 车体参数
        public int SafeDistance; // 离路径点的安全距离，如果超过这个距离将不可以进行自动行走
        public int ApprovingSpeedMin; // 靠站最小速度
    }


    // 路径跟踪输出结果
    public class TRACK_RESULT
    {
        public PATH_POINT target = new PATH_POINT(); // 目标点数据
        public int targetIndex; // 目标点索引
        public int distance2Path; // 车辆到路径的距离
        public string message = new string(new char[260]); // 文本消息
        public int isAtTerminal; // 是否已在站点范围
    }
    public class PathTracking
    {
        private static int TRACE_FORWARD = 1; // 前进
        private static int TRACE_BACK = 2;  // 后退
        private static int TERMNIAL_RANGE_MIN = 5;
        TRACK_PARAM m_param;   // 参数
        TRACK_RESULT m_result;  // 结果
        private static int TOO_CLOSING_DISTANCE_MIN = 1000;

        private int GetPrevPoint(PATH_POINT[] path, int pathCount, int curIndex, int forward, out PATH_POINT pPrev)
        {
            pPrev = new PATH_POINT();
            if (pathCount <= 1)
                return -1;
            int prevIndex = curIndex;

            if (forward == TRACE_FORWARD)
            {
                if (curIndex == 0)
                {
                    pPrev.x = path[curIndex].x + path[curIndex].x - path[curIndex + 1].x;
                    pPrev.y = path[curIndex].y + path[curIndex].y - path[curIndex + 1].y;
                    pPrev.v = path[curIndex].v;
                }
                else
                {
                    prevIndex = curIndex - 1;
                    pPrev.x = path[prevIndex].x;
                    pPrev.y = path[prevIndex].y;
                    pPrev.v = path[prevIndex].v;
                }
            }
            else
            {
                if (curIndex == pathCount - 1)
                {
                    pPrev.x = path[curIndex].x + path[curIndex].x - path[curIndex - 1].x;
                    pPrev.y = path[curIndex].y + path[curIndex].y - path[curIndex - 1].y;
                    pPrev.v = path[curIndex].v;
                }
                else
                {
                    prevIndex = curIndex + 1;
                    pPrev.x = path[prevIndex].x;
                    pPrev.y = path[prevIndex].y;
                    pPrev.v = path[prevIndex].v;
                }
            }

            return prevIndex;
        }

        private int GetNextPoint(PATH_POINT[] path, int pathCount, int curIndex, int forward, out PATH_POINT pNext)
        {
            pNext = new PATH_POINT();
            if (pathCount <= 1)
                return -1;

            //C++ TO C# CONVERTER TODO TASK: The memory management function 'memset' has no equivalent in C#:
            //memset(pNext, 0, sizeof(PATH_POINT));
            int nextIndex = curIndex;

            if (forward == TRACE_FORWARD)
            {
                if (curIndex == pathCount - 1)
                {
                    pNext.x = path[curIndex].x + path[curIndex].x - path[curIndex - 1].x;
                    pNext.y = path[curIndex].y + path[curIndex].y - path[curIndex - 1].y;
                    pNext.v = path[curIndex].v;
                }
                else
                {
                    nextIndex = curIndex + 1;
                    pNext.x = path[nextIndex].x;
                    pNext.y = path[nextIndex].y;
                    pNext.v = path[nextIndex].v;
                }
            }
            else
            {
                if (curIndex == 0)
                {
                    pNext.x = path[curIndex].x + path[curIndex].x - path[curIndex + 1].x;
                    pNext.y = path[curIndex].y + path[curIndex].y - path[curIndex + 1].y;
                    pNext.v = path[curIndex].v;
                }
                else
                {
                    nextIndex = curIndex - 1;
                    pNext.x = path[nextIndex].x;
                    pNext.y = path[nextIndex].y;
                    pNext.v = path[nextIndex].v;
                }
            }

            return nextIndex;
        }



        // 计算目标路径点 -----
        private int CalcTargetPoint(POINT cur_pos)
        {
            PATH_POINT target_point = new PATH_POINT();
            int target_index = -1;
            double minDistance = -1;
            do
            {
                int path_point_cnt = m_param.PathPointsCount;
                PATH_POINT[] path = m_param.PathPoints;
                if (path_point_cnt == 1)
                {
                    target_index = 0;
                    break;
                }

                double dist2Point = 0;
                double dc2t = 0;
                double dc2p = 0;
                double dp2t = 0;
                PATH_POINT prevPoint = new PATH_POINT();
                for (int i = 0; i < path_point_cnt; i++)
                {
                    PATH_POINT prev = new PATH_POINT();
                    GetPrevPoint(path, path_point_cnt, i, m_param.TrackDirection, out prev);

                    double d1 = (cur_pos.X - path[i].x) * (cur_pos.X - path[i].x) + (cur_pos.Y - path[i].y) * (cur_pos.Y - path[i].y);
                    double d2 = (prev.x - path[i].x) * (prev.x - path[i].x) + (prev.y - path[i].y) * (prev.y - path[i].y);
                    double d3 = (cur_pos.X - prev.x) * (cur_pos.X - prev.x) + (cur_pos.Y - prev.y) * (cur_pos.Y - prev.y);
                    double _a = Math.Sqrt(d1);
                    double _b = Math.Sqrt(d2);
                    double _c = Math.Sqrt(d3);

                    double mdist = 0;
                    if ((d2 + d1) >= d3 && (d2 + d3) >= d1)
                    { // 表示当前点与路径的垂足，在路径线段范围内；此情况下，当前点与路径段的最近距离，由垂线段长度决定
                        // 根据海伦公式求三角形高，从而得到距离
                        double _p = (_a + _b + _c) / 2.0;
                        double _s = Math.Sqrt(_p * (_p - _a) * (_p - _b) * (_p - _c));
                        mdist = 2 * _s / _b;
                    }
                    else // 否则，路径段端点与当前点距离中较小的作为路径与当前点的最小距离
                    {
                        mdist = Math.Min(_a, _c);
                    }

                    if (minDistance < 0 || ((int)minDistance) > ((int)mdist))
                    {
                        minDistance = mdist;
                        dist2Point = _a;
                        //C++ TO C# CONVERTER WARNING: The following line was determined to be a copy assignment (rather than a reference assignment) - this should be verified and a 'CopyFrom' method should be created if it does not yet exist:
                        //ORIGINAL LINE: prevPoint = prev;
                        prevPoint = prev;
                        target_index = i;
                        dc2t = d1;
                        dc2p = d3;
                        dp2t = d2;
                    }
                }

                //C++ TO C# CONVERTER TODO TASK: The memory management function 'memcpy' has no equivalent in C#:

                // memcpy(target_point, path + target_index, sizeof(PATH_POINT));
                //double tooClosingDistance = target_point.v *  CLOSING_VELOCITY_K;
                //if(tooClosingDistance < TOO_CLOSING_DISTANCE_MIN) tooClosingDistance = TOO_CLOSING_DISTANCE_MIN;
                double tooClosingDistance = TOO_CLOSING_DISTANCE_MIN;
                if (dist2Point > tooClosingDistance) // 如果离目标点不是太近，或者一段路径还没走到一半，则直接使用目标点
                //|| dc2p <= dc2t )
                {
                    break;
                }

                PATH_POINT next = new PATH_POINT();
                GetNextPoint(path, path_point_cnt, target_index, m_param.TrackDirection, out next);
                //C++ TO C# CONVERTER TODO TASK: The memory management function 'memcpy' has no equivalent in C#:
                target_point = next;

            } while (true);

            m_result.target.x = target_point.x;
            m_result.target.y = target_point.y;
            m_result.target.v = target_point.v;
            m_result.targetIndex = target_index;
            m_result.distance2Path = (int)minDistance;

            return 0;
        }

        // 计算车辆需要的角度
        private double CalcAngle(double CX, double CY, double CAngle, double TX, double TY, double distanceOfWheel)
        {
            double _xc = CY;
            double _yc = -CX;
            double _a = CAngle;
            _a = _a * 3.1415f / 180.0;

            double _xt = TY;
            double _yt = -TX;

            double _angle = 0;
            double xt = (_xt - _xc) * Math.Cos(_a) + (_yt - _yc) * Math.Sin(_a);
            double yt = (_yt - _yc) * Math.Cos(_a) - (_xt - _xc) * Math.Sin(_a);

            if (yt > 0)
            {
                double w = distanceOfWheel;

                double val = w * (2 * xt) / (xt * xt + yt * yt);
                if (val > 1.0)
                    val = 1.0;
                if (val < -1.0)
                    val = -1.0;

                double angle = 1.0 * Math.Asin(val) * 180.0 / 3.1415f;
                _angle = 0 - angle;
                if (_angle < -90)
                    _angle = -90;
                if (_angle > 90)
                    _angle = 90;
            }
            else
            {
                _angle = (xt >= 0) ? -90 : 90;
                //{
                //	_angle = -85;
                //}
                //else
                //{
                //	_angle = 85;
                //}
            }

            //
            //
            if (_angle < -60)
                _angle = -60;
            if (_angle > 60)
                _angle = 60;
            //

            return _angle;
        }
        public int Calc(out TRACK_RESULT presult)
        {
            int h = 0;
            presult = new TRACK_RESULT();
            do
            {
                //_stprintf(m_result.message, _T("在路上"));

                // 1 根据不同的行走方向 使用不同的基准点
                POINT cur_pos = (m_param.TrackDirection == TRACE_FORWARD) ? m_param.Position.frontWheel : m_param.Position.fork;
                //LPPOINT cur_pos = &m_param.Position.fork;
                double cur_angle = (m_param.TrackDirection == TRACE_FORWARD) ? m_param.Position.nav_angle : (m_param.Position.nav_angle + 180.0);

                // 2 计算目标点
                CalcTargetPoint(cur_pos);
                //if(m_result.distance2Path > m_param.SafeDistance) // 车辆离路径太远,不适合自动行走
                //{
                //	h = AGV_LEAVE_PATH_TOO_FAR;
                //	break;
                //}

                // 3 计算角度
                double a = CalcAngle(cur_pos.X, cur_pos.Y, cur_angle, m_result.target.x, m_result.target.y, m_param.VclBody.WheelSpace);
                m_result.target.a = a;

                // 4 判断是否进入站点逼近范围 -----------------------------------------------
                //double dx = fabs(cur_pos->x - ((double)m_param.Terminal.terminal.x));
                //double dy = fabs(cur_pos->y - ((double)m_param.Terminal.terminal.y));
                POINT terminal_base = m_param.Position.fork; // 已货叉中心点为对站依据
                double dx = Math.Abs(terminal_base.X - ((double)m_param.Terminal.terminal.X));
                double dy = Math.Abs(terminal_base.Y - ((double)m_param.Terminal.terminal.Y));
                double approach_range = m_result.target.v; // * 2;
                if (dx > approach_range || dy > approach_range)
                {
                    //_stprintf(m_result.message, _T("on the way"));
                    h = 0;
                    break;
                }

                // 5 判断是否走过头
                //BOOL isOverHead = IsOverHead(cur_pos);
                int isOverHead = IsOverHead(terminal_base);

                // 6 如果过站, 反向调整
                if (isOverHead != 0)
                {
                    int td = m_param.TrackDirection;
                    m_param.TrackDirection = (td == TRACE_FORWARD) ? TRACE_BACK : TRACE_FORWARD;
                    //C++ TO C# CONVERTER TODO TASK: The memory management function 'memset' has no equivalent in C#:
                    //memset(m_result, 0, sizeof(m_result));

                    // 6.1 计算目标点
                    CalcTargetPoint(cur_pos);
                    //if(m_result.distance2Path > m_param.SafeDistance) // 车辆离路径太远,不适合自动行走
                    //{
                    //	h = AGV_LEAVE_PATH_TOO_FAR;
                    //	break;
                    //}

                    // 6.2 计算角度
                    double over_head_angle = (m_param.TrackDirection == TRACE_FORWARD) ? m_param.Position.nav_angle : (m_param.Position.nav_angle + 180.0);
                    double _a = CalcAngle(cur_pos.X, cur_pos.Y, over_head_angle, m_result.target.x, m_result.target.y, m_param.VclBody.WheelSpace);
                    m_result.target.a = _a;
                }

                // 7 逼近减速
                double md = (m_param.Terminal.dir == 'x') ? dx : dy;
                if (md <= 0.001)
                    md = 1;

                int reached = Convert.ToInt32(((md <= m_param.Terminal.range) || (md <= TERMNIAL_RANGE_MIN)));
                m_result.isAtTerminal = reached;
                if (m_param.ClosingDelcelerate != 0)
                {
                    double dv = ClosingDelcelerate(reached, m_result.target.v, md, approach_range, m_param.ApprovingSpeedMin);
                    m_result.target.v = dv;
                }
                //_stprintf(m_result.message, _T("closing %c %d %d"), m_param.Terminal.dir, (int)md, (int)m_result.target.v);

                if (isOverHead != 0) // 过头速度反向
                {
                    m_result.target.v = 0 - m_result.target.v;
                }

                int isdx = Convert.ToInt32(m_param.Terminal.dir == 'x');
                //_stprintf(m_result.message, _T("靠站%s=%f,v=%0.2f"), isdx != 0?_T("dx"):_T("dy"), isdx != 0?dx:dy, m_result.target.v);

                h = 0;
            } while (true);

            if ((h == 0))
            {
                //C++ TO C# CONVERTER TODO TASK: The memory management function 'memcpy' has no equivalent in C#:
                presult = m_result;
            }

            return h;
        }
        private int IsOverHead(POINT cur_pos)
        {
            int isOverreached = 0;
            double dd = 0;
            double dd1 = 0;
            if (m_param.Terminal.dir == 'x')
            {
                dd = m_result.target.x - cur_pos.X;
                dd1 = cur_pos.X - m_param.Terminal.terminal.Y;
            }
            else
            {
                dd = m_result.target.y - cur_pos.Y;
                dd1 = cur_pos.Y - m_param.Terminal.terminal.Y;

            }
            isOverreached = Convert.ToInt32((dd * dd1) > 0);

            return isOverreached;
        }

        private double ClosingDelcelerate(int reach_terminal, double v, double md, double approach_range, int approving_speed_min)
        {
            if (reach_terminal != 0)
                return 0;

            double k = 1.0;
            double dv = v;
            double mm = 1000;
            double mv = 1000;
            if (md > mm)
            {
                double dd = md - mm;
                k = dd / (approach_range - mm);
                if (k > 1)
                    k = 1;

                if (v > mv)
                {
                    dv = (v - mv) * k + mv;

                }
            }
            else
            {
                k = md / mm;
                if (k > 1)
                    k = 1;
                dv = mv * k;

                if (dv > v)
                    dv = v;
            }

            //double k = 1.0;
            //k = reach_terminal? 0 : (md / approach_range);
            //if(k > 1) k = 1;
            //k = k*k;
            //k = 1.0 - k*k;
            //k = 1 - k;//(cos(k * PI / 2.0) + 1.0) / 2.0;

            //double dv = v * k;

            if (dv < approving_speed_min)
            {
                dv = approving_speed_min;
            }

            return dv;
        }
    }
}
