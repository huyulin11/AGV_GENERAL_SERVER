using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Threading;

namespace AGV
{
    public partial class AddTaskFrm : Form
    {

        private List<SingleTask> singleTaskList = AGVInitialize.getInitialize().getSingleTaskList();
        private int TASK_COLUMN_NUM = 15;
        ContextMenu palletMenu = new ContextMenu();
        ContextMenu cancelTasktMenu = new ContextMenu();

        private bool isUpdateFrm = false; //是否更新界面
        private bool isStop = false; //控制更新任务线程是否终止
        private static bool isFirstShow = true;

        public AddTaskFrm()
        {
            InitializeComponent();
            intHeader();
            this.FormClosing += singleTaskFormClosing;
        }

        /// <summary>
        /// 设置是否更新界面
        /// </summary>
        /// <param name="updateFrm"></param>
        public void setUpdateFrm(bool updateFrm)
        {
            this.isUpdateFrm = updateFrm;
        }

        private void intHeader()
        {
            this.SingleTaskDTG.MouseDown += singleTaskFormCellMouseDown;
            this.taskNameVTBC = new DataGridViewTextBoxColumn();

            this.taskNameVTBC.HeaderText = "任务名";
            this.taskNameVTBC.ReadOnly = true;
            this.SingleTaskDTG.Columns.Add(taskNameVTBC);
            this.SingleTaskDTG.ColumnCount = TASK_COLUMN_NUM;
            MenuItem addItem = new MenuItem("添加");

            addItem.Name = "addPalletItem";
            addItem.Name = "addPalletItem";
            addItem.Click += new EventHandler(palletItemClick);
            palletMenu.MenuItems.Add(addItem);

            MenuItem cancelItem = new MenuItem("取消");
            cancelItem.Name = "cancelTaskItem";
            cancelItem.Click += new EventHandler(cancelItemClick);
            cancelTasktMenu.MenuItems.Add(cancelItem);
        }

        void openNewForm()
        {
            int i = 0;
            DataGridViewRow dgvw = new DataGridViewRow();
            this.SingleTaskDTG.Rows.Clear();  //移除所有的行 重新绘制
            singleTaskList = AGVInitialize.getInitialize().getSingleTaskList();
            foreach (SingleTask sl in singleTaskList)
            {
                if (i % TASK_COLUMN_NUM == 0)
                {
                    dgvw = new DataGridViewRow();
                }
                DataGridViewTextBoxCell tb = new DataGridViewTextBoxCell();

                tb.Value = sl;
                if (sl.taskStat == TASKSTAT_T.TASK_READY_SEND || sl.taskStat == TASKSTAT_T.TASK_END) //待可以发送状态，可以取消该任务
                {
                    tb.Style.BackColor = Color.LightGray;
                }
                else if (sl.taskStat == TASKSTAT_T.TASK_SEND || sl.taskStat == TASKSTAT_T.TASK_SEND_SUCCESS)
                {
                    Console.WriteLine("sl name = " + sl.taskName);
                    tb.Style.BackColor = Color.Green;
                }

                dgvw.Cells.Add(tb);

                if ((i + 1) % TASK_COLUMN_NUM == 0)
                {
                    this.SingleTaskDTG.Rows.Add(dgvw);
                }
                i++;

            }
            if ((i + 1) % TASK_COLUMN_NUM != 0)
            {
                this.SingleTaskDTG.Rows.Add(dgvw);
            }
            if (isFirstShow)
            {
                isFirstShow = false;
                this.ShowDialog();
            } else
            {
                this.Update();
            }
        }

        public void initAddTaskFrm()
        {
            Thread thread;
            isStop = false;
            isUpdateFrm = true;
            isFirstShow = true;
            thread = new Thread(new ThreadStart(updateAddTaskFrm));
            thread.Start();
        }

        void _threadProc()
        {
            //定义一个委托实例，该实例执行打开窗口代码
            openNewForm();
        }

        public void updateAddTaskFrm()
        {
            Console.WriteLine("isStop = " + isStop);
            while (!isStop)
            {
                //Console.WriteLine("isUpdateFrm = " + isUpdateFrm);
                if (!isUpdateFrm)  //如果不需要更新
                {
                    Thread.Sleep(1000);  //休眠1S钟，再坚持是否需要更新界面
                    continue;
                }
               
                Thread newThread = new Thread(new ThreadStart(_threadProc));
                newThread.Start();

                isUpdateFrm = false; //更新后，等待下次需要更新
            }
        }

        private void palletItemClick(object sender, EventArgs e)
        {
            MenuItem item = (MenuItem) sender;
            Console.WriteLine("Name = " + item.Name + "value = " + this.SingleTaskDTG.CurrentCell.Value);
            SingleTask st = null;
            st = (SingleTask) this.SingleTaskDTG.CurrentCell.Value;
            if (st != null)
            {
                st.taskStat = TASKSTAT_T.TASK_READY_SEND;
                AGVInitialize.getInitialize().getSchedule().addTaskRecord(TASKSTAT_T.TASK_READY_SEND, st);
                this.SingleTaskDTG.CurrentCell.Style.BackColor = Color.LightGray;
            }
        }

        private void cancelItemClick(object sender, EventArgs e)
        {
            MenuItem item = (MenuItem)sender;
            Console.WriteLine("Name = " + item.Name + "value = " + this.SingleTaskDTG.CurrentCell.Value);
            SingleTask st = null;
            if (item.Name.Equals("cancelTaskItem"))
            {

                st = (SingleTask)this.SingleTaskDTG.CurrentCell.Value;
            }
            if (st != null)
            {
                st.taskStat = TASKSTAT_T.TASK_NOT_ASSIGN;
                this.SingleTaskDTG.CurrentCell.Style.BackColor = Color.White;
                AGVInitialize.getInitialize().getSchedule().removeTaskRecord(st, TASKSTAT_T.TASK_READY_SEND);
            }
        }

        private void singleTaskFormCellMouseDown(object sender, MouseEventArgs e)
        {
            
            if (e.Button == MouseButtons.Right)
            {
                DataGridView.HitTestInfo hitinfo;
                hitinfo = this.SingleTaskDTG.HitTest(e.X, e.Y);
                if (hitinfo.RowIndex >= 0)
                {
                    this.SingleTaskDTG.ClearSelection();
                    this.SingleTaskDTG.Rows[hitinfo.RowIndex].Cells[hitinfo.ColumnIndex].Selected = true;
                    this.SingleTaskDTG.CurrentCell = this.SingleTaskDTG.Rows[hitinfo.RowIndex].Cells[hitinfo.ColumnIndex];
                    SingleTask sl = (SingleTask) this.SingleTaskDTG.CurrentCell.Value;
                    Console.WriteLine("this.SingleTaskDTG.CurrentRow value = " + this.SingleTaskDTG.CurrentCell.Value);
                    //string sql = "select * from taskrecord, singletask where taskRecordStat in (1, 2, 3) and singleTask = singletask.id and singletask.taskName = '" + (string)this.SingleTaskDTG.CurrentRow.Cells[0].Value + "'";
                    if (sl.taskStat == TASKSTAT_T.TASK_NOT_ASSIGN || sl.taskStat == TASKSTAT_T.TASK_END)
                    {
                        Console.WriteLine("x = " + e.X + "y = " + e.Y);
                        palletMenu.Show(this.SingleTaskDTG, new Point(e.X, e.Y));
                    }
                    else if (sl.taskStat == TASKSTAT_T.TASK_READY_SEND)   //该状态可以取消任务
                    {
                        cancelTasktMenu.Show(this.SingleTaskDTG, new Point(e.X, e.Y));
                    }
                }
                Console.WriteLine("row = " + this.SingleTaskDTG.CurrentRow);
            }
        }

        private void singleTaskFormClosing(object sender, FormClosingEventArgs e)
        {
            isStop = true;
            isUpdateFrm  = false;
        }

    }
}
