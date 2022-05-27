namespace SiteMonitoring3
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
        /// 이 메서드의 내용을 코드 편집기로 수정하지 마십시오.
        /// </summary>
        private void InitializeComponent()
        {
      this.btnRun = new System.Windows.Forms.Button();
      this.label2 = new System.Windows.Forms.Label();
      this.btnSendTestMsg = new System.Windows.Forms.Button();
      this.txtLog = new System.Windows.Forms.TextBox();
      this.txtItemList = new System.Windows.Forms.TextBox();
      this.txtFilteredItemList = new System.Windows.Forms.TextBox();
      this.lblSleepRemain = new System.Windows.Forms.Label();
      this.txtMonitoringInfoJson = new System.Windows.Forms.RichTextBox();
      this.SuspendLayout();
      // 
      // btnRun
      // 
      this.btnRun.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
      this.btnRun.Location = new System.Drawing.Point(566, 12);
      this.btnRun.Name = "btnRun";
      this.btnRun.Size = new System.Drawing.Size(90, 23);
      this.btnRun.TabIndex = 8;
      this.btnRun.Text = "실행";
      this.btnRun.UseVisualStyleBackColor = true;
      this.btnRun.Click += new System.EventHandler(this.btnRun_Click);
      // 
      // label2
      // 
      this.label2.AutoSize = true;
      this.label2.Location = new System.Drawing.Point(10, 17);
      this.label2.Name = "label2";
      this.label2.Size = new System.Drawing.Size(81, 12);
      this.label2.TabIndex = 6;
      this.label2.Text = "검색정보 json";
      // 
      // btnSendTestMsg
      // 
      this.btnSendTestMsg.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
      this.btnSendTestMsg.Location = new System.Drawing.Point(470, 12);
      this.btnSendTestMsg.Name = "btnSendTestMsg";
      this.btnSendTestMsg.Size = new System.Drawing.Size(90, 23);
      this.btnSendTestMsg.TabIndex = 5;
      this.btnSendTestMsg.Text = "알림 테스트";
      this.btnSendTestMsg.UseVisualStyleBackColor = true;
      this.btnSendTestMsg.Click += new System.EventHandler(this.btnSendTestMsg_Click);
      // 
      // txtLog
      // 
      this.txtLog.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
      this.txtLog.Location = new System.Drawing.Point(470, 41);
      this.txtLog.Multiline = true;
      this.txtLog.Name = "txtLog";
      this.txtLog.ReadOnly = true;
      this.txtLog.ScrollBars = System.Windows.Forms.ScrollBars.Both;
      this.txtLog.Size = new System.Drawing.Size(302, 237);
      this.txtLog.TabIndex = 13;
      // 
      // txtItemList
      // 
      this.txtItemList.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
      this.txtItemList.Location = new System.Drawing.Point(12, 284);
      this.txtItemList.Multiline = true;
      this.txtItemList.Name = "txtItemList";
      this.txtItemList.ReadOnly = true;
      this.txtItemList.ScrollBars = System.Windows.Forms.ScrollBars.Both;
      this.txtItemList.Size = new System.Drawing.Size(760, 100);
      this.txtItemList.TabIndex = 16;
      this.txtItemList.WordWrap = false;
      // 
      // txtFilteredItemList
      // 
      this.txtFilteredItemList.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
      this.txtFilteredItemList.Location = new System.Drawing.Point(12, 392);
      this.txtFilteredItemList.Multiline = true;
      this.txtFilteredItemList.Name = "txtFilteredItemList";
      this.txtFilteredItemList.ReadOnly = true;
      this.txtFilteredItemList.ScrollBars = System.Windows.Forms.ScrollBars.Both;
      this.txtFilteredItemList.Size = new System.Drawing.Size(760, 186);
      this.txtFilteredItemList.TabIndex = 17;
      this.txtFilteredItemList.WordWrap = false;
      // 
      // lblSleepRemain
      // 
      this.lblSleepRemain.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
      this.lblSleepRemain.AutoSize = true;
      this.lblSleepRemain.Location = new System.Drawing.Point(662, 17);
      this.lblSleepRemain.Name = "lblSleepRemain";
      this.lblSleepRemain.Size = new System.Drawing.Size(97, 12);
      this.lblSleepRemain.TabIndex = 18;
      this.lblSleepRemain.Text = "잠자기 남은 시간";
      // 
      // txtMonitoringInfoJson
      // 
      this.txtMonitoringInfoJson.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
      this.txtMonitoringInfoJson.Location = new System.Drawing.Point(12, 41);
      this.txtMonitoringInfoJson.Name = "txtMonitoringInfoJson";
      this.txtMonitoringInfoJson.Size = new System.Drawing.Size(452, 237);
      this.txtMonitoringInfoJson.TabIndex = 20;
      this.txtMonitoringInfoJson.Text = "";
      // 
      // Form1
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.ClientSize = new System.Drawing.Size(784, 590);
      this.Controls.Add(this.txtMonitoringInfoJson);
      this.Controls.Add(this.lblSleepRemain);
      this.Controls.Add(this.txtFilteredItemList);
      this.Controls.Add(this.txtItemList);
      this.Controls.Add(this.txtLog);
      this.Controls.Add(this.btnSendTestMsg);
      this.Controls.Add(this.label2);
      this.Controls.Add(this.btnRun);
      this.MinimumSize = new System.Drawing.Size(500, 500);
      this.Name = "Form1";
      this.Text = "Site Monitoring";
      this.Load += new System.EventHandler(this.Form1_Load);
      this.ResumeLayout(false);
      this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.Button btnRun;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button btnSendTestMsg;
        private System.Windows.Forms.TextBox txtLog;
        private System.Windows.Forms.TextBox txtItemList;
        private System.Windows.Forms.TextBox txtFilteredItemList;
        private System.Windows.Forms.Label lblSleepRemain;
        private System.Windows.Forms.RichTextBox txtMonitoringInfoJson;
    }
}

