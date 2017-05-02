using System.Windows.Forms;
namespace AGV
{
    partial class AddTaskFrm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.Windows.Forms.DataGridView SingleTaskDTG;
            SingleTaskDTG = new System.Windows.Forms.DataGridView();
            ((System.ComponentModel.ISupportInitialize)(SingleTaskDTG)).BeginInit();
            this.SuspendLayout();
            // 
            // SingleTaskDTG
            // 
            SingleTaskDTG.AllowUserToAddRows = false;
            SingleTaskDTG.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            SingleTaskDTG.Dock = System.Windows.Forms.DockStyle.Fill;
            SingleTaskDTG.Location = new System.Drawing.Point(0, 0);
            SingleTaskDTG.MultiSelect = false;
            SingleTaskDTG.Name = "SingleTaskDTG";
            SingleTaskDTG.RowTemplate.Height = 23;
            SingleTaskDTG.Size = new System.Drawing.Size(815, 535);
            SingleTaskDTG.TabIndex = 0;
            // 
            // AddTaskFrm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(815, 535);
            this.Controls.Add(SingleTaskDTG);
            this.Name = "AddTaskFrm";
            this.Text = "添加任务";
            this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
            ((System.ComponentModel.ISupportInitialize)(SingleTaskDTG)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.DataGridViewComboBoxColumn palletCBC;
        private System.Windows.Forms.DataGridViewTextBoxColumn taskNameVTBC;




    }
}