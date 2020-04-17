namespace HW6_LeastSquares
{
    partial class Form1
    {
        /// <summary>
        /// 필수 디자이너 변수입니다.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 사용 중인 모든 리소스를 정리합니다.
        /// </summary>
        /// <param name="disposing">관리되는 리소스를 삭제해야 하면 true이고, 그렇지 않으면 false입니다.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form 디자이너에서 생성한 코드

        /// <summary>
        /// 디자이너 지원에 필요한 메서드입니다. 
        /// 이 메서드의 내용을 코드 편집기로 수정하지 마세요.
        /// </summary>
        private void InitializeComponent()
        {
            this.btnExeCircle = new System.Windows.Forms.Button();
            this.txtHistory = new System.Windows.Forms.TextBox();
            this.btnInfo = new System.Windows.Forms.Button();
            this.btnExeEllipse = new System.Windows.Forms.Button();
            this.btnClear = new System.Windows.Forms.Button();
            this.picMain = new System.Windows.Forms.PictureBox();
            ((System.ComponentModel.ISupportInitialize)(this.picMain)).BeginInit();
            this.SuspendLayout();
            // 
            // btnExeCircle
            // 
            this.btnExeCircle.Location = new System.Drawing.Point(498, 12);
            this.btnExeCircle.Name = "btnExeCircle";
            this.btnExeCircle.Size = new System.Drawing.Size(75, 23);
            this.btnExeCircle.TabIndex = 8;
            this.btnExeCircle.Text = "Circle";
            this.btnExeCircle.UseVisualStyleBackColor = true;
            this.btnExeCircle.Click += new System.EventHandler(this.btnExeCircle_Click);
            // 
            // txtHistory
            // 
            this.txtHistory.Location = new System.Drawing.Point(498, 41);
            this.txtHistory.Multiline = true;
            this.txtHistory.Name = "txtHistory";
            this.txtHistory.Size = new System.Drawing.Size(315, 331);
            this.txtHistory.TabIndex = 7;
            // 
            // btnInfo
            // 
            this.btnInfo.Location = new System.Drawing.Point(741, 12);
            this.btnInfo.Name = "btnInfo";
            this.btnInfo.Size = new System.Drawing.Size(75, 23);
            this.btnInfo.TabIndex = 6;
            this.btnInfo.Text = "Infomation";
            this.btnInfo.UseVisualStyleBackColor = true;
            this.btnInfo.Click += new System.EventHandler(this.btnInfo_Click);
            // 
            // btnExeEllipse
            // 
            this.btnExeEllipse.Location = new System.Drawing.Point(579, 12);
            this.btnExeEllipse.Name = "btnExeEllipse";
            this.btnExeEllipse.Size = new System.Drawing.Size(75, 23);
            this.btnExeEllipse.TabIndex = 8;
            this.btnExeEllipse.Text = "Ellipse";
            this.btnExeEllipse.UseVisualStyleBackColor = true;
            this.btnExeEllipse.Click += new System.EventHandler(this.btnExeEllipse_Click);
            // 
            // btnClear
            // 
            this.btnClear.Location = new System.Drawing.Point(660, 12);
            this.btnClear.Name = "btnClear";
            this.btnClear.Size = new System.Drawing.Size(75, 23);
            this.btnClear.TabIndex = 9;
            this.btnClear.Text = "Clear";
            this.btnClear.UseVisualStyleBackColor = true;
            this.btnClear.Click += new System.EventHandler(this.btnClear_Click);
            // 
            // picMain
            // 
            this.picMain.BackColor = System.Drawing.SystemColors.Window;
            this.picMain.Location = new System.Drawing.Point(12, 12);
            this.picMain.Name = "picMain";
            this.picMain.Size = new System.Drawing.Size(480, 360);
            this.picMain.TabIndex = 10;
            this.picMain.TabStop = false;
            this.picMain.MouseClick += new System.Windows.Forms.MouseEventHandler(this.picMain_MouseClick);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(825, 387);
            this.Controls.Add(this.picMain);
            this.Controls.Add(this.btnClear);
            this.Controls.Add(this.btnExeEllipse);
            this.Controls.Add(this.btnExeCircle);
            this.Controls.Add(this.txtHistory);
            this.Controls.Add(this.btnInfo);
            this.Name = "Form1";
            this.Text = "HW6_LeastSquare";
            ((System.ComponentModel.ISupportInitialize)(this.picMain)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btnExeCircle;
        private System.Windows.Forms.TextBox txtHistory;
        private System.Windows.Forms.Button btnInfo;
        private System.Windows.Forms.Button btnExeEllipse;
        private System.Windows.Forms.Button btnClear;
        private System.Windows.Forms.PictureBox picMain;
    }
}

