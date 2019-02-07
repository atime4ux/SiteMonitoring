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
            this.txtSrchWord01 = new System.Windows.Forms.TextBox();
            this.txtSrchWord02 = new System.Windows.Forms.TextBox();
            this.button1 = new System.Windows.Forms.Button();
            this.AccountID = new System.Windows.Forms.TextBox();
            this.AccountPass = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.button2 = new System.Windows.Forms.Button();
            this.Trigger = new System.Windows.Forms.Label();
            this.rcvAddr = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.checkBox1 = new System.Windows.Forms.CheckBox();
            this.checkBox2 = new System.Windows.Forms.CheckBox();
            this.txtStatus = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.txtExceptWord = new System.Windows.Forms.TextBox();
            this.txtItemList = new System.Windows.Forms.TextBox();
            this.txtFilteredItemList = new System.Windows.Forms.TextBox();
            this.lblSleepRemain = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // txtSrchWord01
            // 
            this.txtSrchWord01.Location = new System.Drawing.Point(87, 140);
            this.txtSrchWord01.Name = "txtSrchWord01";
            this.txtSrchWord01.Size = new System.Drawing.Size(207, 21);
            this.txtSrchWord01.TabIndex = 6;
            this.txtSrchWord01.Text = "1차 검색어";
            // 
            // txtSrchWord02
            // 
            this.txtSrchWord02.Location = new System.Drawing.Point(87, 168);
            this.txtSrchWord02.Name = "txtSrchWord02";
            this.txtSrchWord02.Size = new System.Drawing.Size(207, 21);
            this.txtSrchWord02.TabIndex = 7;
            this.txtSrchWord02.Text = "2차 검색어";
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(87, 222);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(75, 23);
            this.button1.TabIndex = 8;
            this.button1.Text = "실행";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // AccountID
            // 
            this.AccountID.Location = new System.Drawing.Point(87, 12);
            this.AccountID.Name = "AccountID";
            this.AccountID.Size = new System.Drawing.Size(100, 21);
            this.AccountID.TabIndex = 1;
            this.AccountID.Text = "ID";
            // 
            // AccountPass
            // 
            this.AccountPass.Location = new System.Drawing.Point(194, 12);
            this.AccountPass.Name = "AccountPass";
            this.AccountPass.Size = new System.Drawing.Size(100, 21);
            this.AccountPass.TabIndex = 2;
            this.AccountPass.Text = "Pass";
            this.AccountPass.UseSystemPasswordChar = true;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(15, 15);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(62, 12);
            this.label1.TabIndex = 5;
            this.label1.Text = "Gmail계정";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(15, 143);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(69, 12);
            this.label2.TabIndex = 6;
            this.label2.Text = "필수 검색어";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(15, 171);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(69, 12);
            this.label3.TabIndex = 7;
            this.label3.Text = "선택 검색어";
            // 
            // button2
            // 
            this.button2.Location = new System.Drawing.Point(87, 67);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(75, 23);
            this.button2.TabIndex = 5;
            this.button2.Text = "테스트";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // Trigger
            // 
            this.Trigger.AutoSize = true;
            this.Trigger.Location = new System.Drawing.Point(15, 72);
            this.Trigger.Name = "Trigger";
            this.Trigger.Size = new System.Drawing.Size(45, 12);
            this.Trigger.TabIndex = 9;
            this.Trigger.Text = "Trigger";
            this.Trigger.TextChanged += new System.EventHandler(this.Trigger_TextChanged);
            // 
            // rcvAddr
            // 
            this.rcvAddr.Location = new System.Drawing.Point(87, 40);
            this.rcvAddr.Name = "rcvAddr";
            this.rcvAddr.Size = new System.Drawing.Size(207, 21);
            this.rcvAddr.TabIndex = 3;
            this.rcvAddr.Text = "rcvAddr";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(15, 43);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(57, 12);
            this.label4.TabIndex = 11;
            this.label4.Text = "받을 메일";
            // 
            // checkBox1
            // 
            this.checkBox1.AutoSize = true;
            this.checkBox1.Location = new System.Drawing.Point(87, 96);
            this.checkBox1.Name = "checkBox1";
            this.checkBox1.Size = new System.Drawing.Size(97, 16);
            this.checkBox1.TabIndex = 4;
            this.checkBox1.Text = "Gmail로 받음";
            this.checkBox1.UseVisualStyleBackColor = true;
            this.checkBox1.CheckedChanged += new System.EventHandler(this.checkBox1_CheckedChanged);
            // 
            // checkBox2
            // 
            this.checkBox2.AutoSize = true;
            this.checkBox2.Location = new System.Drawing.Point(87, 118);
            this.checkBox2.Name = "checkBox2";
            this.checkBox2.Size = new System.Drawing.Size(88, 16);
            this.checkBox2.TabIndex = 12;
            this.checkBox2.Text = "알림창 표시";
            this.checkBox2.UseVisualStyleBackColor = true;
            // 
            // txtStatus
            // 
            this.txtStatus.Location = new System.Drawing.Point(300, 12);
            this.txtStatus.Multiline = true;
            this.txtStatus.Name = "txtStatus";
            this.txtStatus.ReadOnly = true;
            this.txtStatus.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.txtStatus.Size = new System.Drawing.Size(502, 233);
            this.txtStatus.TabIndex = 13;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(15, 198);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(69, 12);
            this.label5.TabIndex = 14;
            this.label5.Text = "제외 검색어";
            // 
            // txtExceptWord
            // 
            this.txtExceptWord.Location = new System.Drawing.Point(87, 195);
            this.txtExceptWord.Name = "txtExceptWord";
            this.txtExceptWord.Size = new System.Drawing.Size(207, 21);
            this.txtExceptWord.TabIndex = 8;
            this.txtExceptWord.Text = "제외 검색어";
            // 
            // txtItemList
            // 
            this.txtItemList.Location = new System.Drawing.Point(17, 251);
            this.txtItemList.Multiline = true;
            this.txtItemList.Name = "txtItemList";
            this.txtItemList.ReadOnly = true;
            this.txtItemList.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.txtItemList.Size = new System.Drawing.Size(785, 277);
            this.txtItemList.TabIndex = 16;
            // 
            // txtFilteredItemList
            // 
            this.txtFilteredItemList.Location = new System.Drawing.Point(17, 534);
            this.txtFilteredItemList.Multiline = true;
            this.txtFilteredItemList.Name = "txtFilteredItemList";
            this.txtFilteredItemList.ReadOnly = true;
            this.txtFilteredItemList.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.txtFilteredItemList.Size = new System.Drawing.Size(785, 260);
            this.txtFilteredItemList.TabIndex = 17;
            // 
            // lblSleepRemain
            // 
            this.lblSleepRemain.AutoSize = true;
            this.lblSleepRemain.Location = new System.Drawing.Point(168, 227);
            this.lblSleepRemain.Name = "lblSleepRemain";
            this.lblSleepRemain.Size = new System.Drawing.Size(38, 12);
            this.lblSleepRemain.TabIndex = 18;
            this.lblSleepRemain.Text = "label6";
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(814, 804);
            this.Controls.Add(this.lblSleepRemain);
            this.Controls.Add(this.txtFilteredItemList);
            this.Controls.Add(this.txtItemList);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.txtExceptWord);
            this.Controls.Add(this.txtStatus);
            this.Controls.Add(this.checkBox2);
            this.Controls.Add(this.checkBox1);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.rcvAddr);
            this.Controls.Add(this.Trigger);
            this.Controls.Add(this.button2);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.AccountPass);
            this.Controls.Add(this.AccountID);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.txtSrchWord02);
            this.Controls.Add(this.txtSrchWord01);
            this.Name = "Form1";
            this.Text = "Form1";
            this.Load += new System.EventHandler(this.Form1_Load);
            this.Resize += new System.EventHandler(this.Form1_Resize);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox txtSrchWord01;
        private System.Windows.Forms.TextBox txtSrchWord02;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.TextBox AccountID;
        private System.Windows.Forms.TextBox AccountPass;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.Label Trigger;
        private System.Windows.Forms.TextBox rcvAddr;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.CheckBox checkBox1;
        private System.Windows.Forms.CheckBox checkBox2;
        private System.Windows.Forms.TextBox txtStatus;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.TextBox txtExceptWord;
        private System.Windows.Forms.TextBox txtItemList;
        private System.Windows.Forms.TextBox txtFilteredItemList;
        private System.Windows.Forms.Label lblSleepRemain;
    }
}

