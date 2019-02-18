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
            this.txtGmailID = new System.Windows.Forms.TextBox();
            this.txtGmailPass = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.btnSendTestMsg = new System.Windows.Forms.Button();
            this.txtReceiveMailAddress = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.chkUsingMailAccount = new System.Windows.Forms.CheckBox();
            this.txtLog = new System.Windows.Forms.TextBox();
            this.txtItemList = new System.Windows.Forms.TextBox();
            this.txtFilteredItemList = new System.Windows.Forms.TextBox();
            this.lblSleepRemain = new System.Windows.Forms.Label();
            this.txtKeywordJson = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // btnRun
            // 
            this.btnRun.Location = new System.Drawing.Point(78, 227);
            this.btnRun.Name = "btnRun";
            this.btnRun.Size = new System.Drawing.Size(75, 23);
            this.btnRun.TabIndex = 8;
            this.btnRun.Text = "실행";
            this.btnRun.UseVisualStyleBackColor = true;
            this.btnRun.Click += new System.EventHandler(this.btnRun_Click);
            // 
            // txtGmailID
            // 
            this.txtGmailID.Location = new System.Drawing.Point(78, 12);
            this.txtGmailID.Name = "txtGmailID";
            this.txtGmailID.Size = new System.Drawing.Size(124, 21);
            this.txtGmailID.TabIndex = 1;
            // 
            // txtGmailPass
            // 
            this.txtGmailPass.Location = new System.Drawing.Point(211, 12);
            this.txtGmailPass.Name = "txtGmailPass";
            this.txtGmailPass.Size = new System.Drawing.Size(124, 21);
            this.txtGmailPass.TabIndex = 2;
            this.txtGmailPass.UseSystemPasswordChar = true;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(6, 15);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(62, 12);
            this.label1.TabIndex = 5;
            this.label1.Text = "Gmail계정";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(6, 96);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(53, 12);
            this.label2.TabIndex = 6;
            this.label2.Text = "검색정보";
            // 
            // btnSendTestMsg
            // 
            this.btnSendTestMsg.Location = new System.Drawing.Point(341, 12);
            this.btnSendTestMsg.Name = "btnSendTestMsg";
            this.btnSendTestMsg.Size = new System.Drawing.Size(52, 49);
            this.btnSendTestMsg.TabIndex = 5;
            this.btnSendTestMsg.Text = "알림 테스트";
            this.btnSendTestMsg.UseVisualStyleBackColor = true;
            this.btnSendTestMsg.Click += new System.EventHandler(this.btnSendTestMsg_Click);
            // 
            // txtReceiveMailAddress
            // 
            this.txtReceiveMailAddress.Location = new System.Drawing.Point(78, 40);
            this.txtReceiveMailAddress.Name = "txtReceiveMailAddress";
            this.txtReceiveMailAddress.Size = new System.Drawing.Size(257, 21);
            this.txtReceiveMailAddress.TabIndex = 3;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(6, 43);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(57, 12);
            this.label4.TabIndex = 11;
            this.label4.Text = "받을 메일";
            // 
            // chkUsingMailAccount
            // 
            this.chkUsingMailAccount.AutoSize = true;
            this.chkUsingMailAccount.Location = new System.Drawing.Point(78, 71);
            this.chkUsingMailAccount.Name = "chkUsingMailAccount";
            this.chkUsingMailAccount.Size = new System.Drawing.Size(165, 16);
            this.chkUsingMailAccount.TabIndex = 4;
            this.chkUsingMailAccount.Text = "Gmail 계정으로 메일 수신";
            this.chkUsingMailAccount.UseVisualStyleBackColor = true;
            this.chkUsingMailAccount.CheckedChanged += new System.EventHandler(this.chkGmail_CheckedChanged);
            // 
            // txtLog
            // 
            this.txtLog.Location = new System.Drawing.Point(399, 12);
            this.txtLog.Multiline = true;
            this.txtLog.Name = "txtLog";
            this.txtLog.ReadOnly = true;
            this.txtLog.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.txtLog.Size = new System.Drawing.Size(200, 209);
            this.txtLog.TabIndex = 13;
            // 
            // txtItemList
            // 
            this.txtItemList.Location = new System.Drawing.Point(5, 256);
            this.txtItemList.Multiline = true;
            this.txtItemList.Name = "txtItemList";
            this.txtItemList.ReadOnly = true;
            this.txtItemList.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.txtItemList.Size = new System.Drawing.Size(594, 134);
            this.txtItemList.TabIndex = 16;
            // 
            // txtFilteredItemList
            // 
            this.txtFilteredItemList.Location = new System.Drawing.Point(5, 396);
            this.txtFilteredItemList.Multiline = true;
            this.txtFilteredItemList.Name = "txtFilteredItemList";
            this.txtFilteredItemList.ReadOnly = true;
            this.txtFilteredItemList.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.txtFilteredItemList.Size = new System.Drawing.Size(594, 186);
            this.txtFilteredItemList.TabIndex = 17;
            // 
            // lblSleepRemain
            // 
            this.lblSleepRemain.AutoSize = true;
            this.lblSleepRemain.Location = new System.Drawing.Point(159, 232);
            this.lblSleepRemain.Name = "lblSleepRemain";
            this.lblSleepRemain.Size = new System.Drawing.Size(97, 12);
            this.lblSleepRemain.TabIndex = 18;
            this.lblSleepRemain.Text = "잠자기 남은 시간";
            // 
            // txtKeywordJson
            // 
            this.txtKeywordJson.Location = new System.Drawing.Point(78, 93);
            this.txtKeywordJson.Multiline = true;
            this.txtKeywordJson.Name = "txtKeywordJson";
            this.txtKeywordJson.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.txtKeywordJson.Size = new System.Drawing.Size(315, 128);
            this.txtKeywordJson.TabIndex = 19;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(606, 590);
            this.Controls.Add(this.txtKeywordJson);
            this.Controls.Add(this.lblSleepRemain);
            this.Controls.Add(this.txtFilteredItemList);
            this.Controls.Add(this.txtItemList);
            this.Controls.Add(this.txtLog);
            this.Controls.Add(this.chkUsingMailAccount);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.txtReceiveMailAddress);
            this.Controls.Add(this.btnSendTestMsg);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.txtGmailPass);
            this.Controls.Add(this.txtGmailID);
            this.Controls.Add(this.btnRun);
            this.Name = "Form1";
            this.Text = "Form1";
            this.Load += new System.EventHandler(this.Form1_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.Button btnRun;
        private System.Windows.Forms.TextBox txtGmailID;
        private System.Windows.Forms.TextBox txtGmailPass;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button btnSendTestMsg;
        private System.Windows.Forms.TextBox txtReceiveMailAddress;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.CheckBox chkUsingMailAccount;
        private System.Windows.Forms.TextBox txtLog;
        private System.Windows.Forms.TextBox txtItemList;
        private System.Windows.Forms.TextBox txtFilteredItemList;
        private System.Windows.Forms.Label lblSleepRemain;
        private System.Windows.Forms.TextBox txtKeywordJson;
    }
}

