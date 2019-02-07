using libMyUtil;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

using SiteMonitoring3.Alarm;
using SiteMonitoring3.Parsing;
using libCommon;

namespace SiteMonitoring3
{
    public partial class Form1 : Form
    {
        clsUtil objUtil = new clsUtil();

        System.Threading.Thread threadMainJob;

        NotifyIcon myNotify;

        Joonggonara joonggonara = new Joonggonara();


        AlarmJandi objJandi = null;
        AlarmMail objMail = null;


        bool loopFlag;

        readonly int minSleepSecond = 10;
        int curSleepSecond = 10;
        

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            this.AccountID.Text = "";
            this.AccountPass.Text = "";
            this.rcvAddr.Text = "";
            this.txtSrchWord01.Text = "";
            this.txtSrchWord02.Text = "";
            this.txtExceptWord.Text = "";
            this.lblSleepRemain.Text = "";

            Application.ApplicationExit += new EventHandler(Application_ApplicationExit);

            Trigger.Hide();

            myNotify = new NotifyIcon();
            myNotify.BalloonTipText = "최소화...";
            myNotify.BalloonTipTitle = "Site감시3";
            myNotify.BalloonTipIcon = ToolTipIcon.Info;
            myNotify.Icon = this.Icon;
            myNotify.MouseClick += new MouseEventHandler(myNotify_MouseClick);

            loopFlag = false;
        }

        private void InitAlarm()
        {
            objJandi = new AlarmJandi(objUtil.getAppCfg("jandiWebHookUrl"));
            objMail = new AlarmMail(AccountID.Text,
                                    AccountPass.Text,
                                    rcvAddr.Text,
                                    rcvAddr.Text);
        }

        private void StartAndStop()
        {
            if (txtSrchWord01.Text.Trim().Length > 0 || txtSrchWord02.Text.Trim().Length > 0)
            {
                if (!loopFlag)
                {
                    InitAlarm();

                    loopFlag = true;
                    changeRunningState();
                    threadMainJob = new System.Threading.Thread(new System.Threading.ThreadStart(MainJob));
                    threadMainJob.Name = "loopStart";
                    threadMainJob.Start();
                }
                else
                {
                    loopFlag = false;
                    changeRunningState();
                }
            }
            else
            {
                MessageBox.Show("검색어 입력");
            }
        }


        /// <summary>
        /// 시작버튼
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button1_Click(object sender, EventArgs e)
        {
            StartAndStop();
        }


        void Application_ApplicationExit(object sender, EventArgs e)
        {
            if (threadMainJob != null)
                threadMainJob.Abort();
        }


        void myNotify_MouseClick(object sender, MouseEventArgs e)
        {
            this.Show();
            this.WindowState = FormWindowState.Normal;
        }


        private void changeRunningState()
        {
            System.Threading.Thread changeStateThread;

            if (loopFlag)
            {
                changeStateThread = new System.Threading.Thread(new System.Threading.ThreadStart(delegate
                {
                    SetTextBox(AccountID, "", false);
                    SetTextBox(AccountPass, "", false);
                    SetTextBox(rcvAddr, "", false);
                    SetCheckBox(checkBox1, false);
                    SetCheckBox(checkBox2, false);
                    SetTextBox(txtSrchWord01, "", false);
                    SetTextBox(txtSrchWord02, "", false);
                    SetTextBox(txtExceptWord, "", false);
                    clsThread.SetLabel(lblSleepRemain, "");
                    clsThread.buttonToggle(button2, "", false);
                    clsThread.buttonToggle(button1, "중지", true);
                    clsThread.SetTextBox(txtFilteredItemList, "", false);
                }));
            }
            else
            {
                changeStateThread = new System.Threading.Thread(new System.Threading.ThreadStart(delegate {
                    clsThread.buttonToggle(button1, "중지중...", true);
                    clsThread.buttonToggle(button2, "", true);
                    clsThread.SetLabel(lblSleepRemain, "");
                    SetTextBox(txtExceptWord, "", true);
                    SetTextBox(txtSrchWord02, "", true);
                    SetTextBox(txtSrchWord01, "", true);
                    SetCheckBox(checkBox1, true);
                    SetCheckBox(checkBox2, true);
                    SetTextBox(rcvAddr, "", true);
                    SetTextBox(AccountPass, "", true);
                    SetTextBox(AccountID, "", true);

                    while (threadMainJob.ThreadState != System.Threading.ThreadState.Stopped)
                    {
                        System.Threading.Thread.Sleep(100);
                    }

                    clsThread.buttonToggle(button1, "실행", true);
                }));
            }

            changeStateThread.Start();
        }

        private delegate void SetTextBoxCallBack(System.Windows.Forms.TextBox Textbox, string str, bool On_Off);
        public void SetTextBox(System.Windows.Forms.TextBox Textbox, string str, bool On_Off)
        {
            if (Textbox.InvokeRequired)
            {
                SetTextBoxCallBack dele = new SetTextBoxCallBack(SetTextBox);
                Textbox.Invoke(dele, Textbox, str, On_Off);
            }
            else
            {
                Textbox.Enabled = On_Off;
                if (str.Length > 0)
                    Textbox.AppendText(str);
            }
        }
        private delegate void SetCheckBoxCallBack(System.Windows.Forms.CheckBox Checkbox, bool On_Off);
        public void SetCheckBox(System.Windows.Forms.CheckBox Checkbox, bool On_Off)
        {
            if (Checkbox.InvokeRequired)
            {
                SetCheckBoxCallBack dele = new SetCheckBoxCallBack(SetCheckBox);
                Checkbox.Invoke(dele, Checkbox, On_Off);
            }
            else
                Checkbox.Enabled = On_Off;
        }

        private void MainJob()
        {
            while (loopFlag)
            {
                System.Threading.Thread threadSubJob = new System.Threading.Thread(new System.Threading.ThreadStart(SubJob));
                threadSubJob.Name = "DownloadAndParsing";
                threadSubJob.Start();
                threadSubJob.Join();

                if (joonggonara.isBlocked == true)
                {
                    objJandi.SendJandi("SiteMonitoring", "blocked");
                    StartAndStop();
                }

                if (loopFlag)
                {
                    WriteStatus($"start sleep {curSleepSecond} sec", true);
                    for (int i = 0; i < curSleepSecond; i++)
                    {
                        if (loopFlag == true)
                        {
                            clsThread.SetLabel(lblSleepRemain, $"sleep remain {(curSleepSecond - i).ToString()}");
                            System.Threading.Thread.Sleep(1000);
                        }
                        else
                        {
                            break;
                        }
                    }
                    clsThread.SetLabel(lblSleepRemain, "");
                    WriteStatus("end sleep", true);
                }
            }
        }

        private void SubJob()
        {
            WriteStatus("start", true);

            List<MonitoringItem> lstAll = new List<MonitoringItem>();
            List<MonitoringItem> lstFiltered = new List<MonitoringItem>();

            WriteStatus("downloading data", true);
            lstAll.AddRange(joonggonara.GetMonitoringList(txtStatus));

            if (lstAll.Count == 0)
            {
                curSleepSecond = curSleepSecond * 2;
            }
            else
            {
                curSleepSecond = Math.Max(minSleepSecond, curSleepSecond / 2);
            }

            WriteStatus($"success get {lstAll.Count} items", true);

            libMyUtil.clsThread.SetTextBox(txtItemList, "", false);
            foreach (MonitoringItem item in lstAll)
            {
                if (FilterMonitoringItem(item) == true)
                {
                    lstFiltered.Add(item);
                    libMyUtil.clsThread.SetTextBox(txtFilteredItemList, item.itemTitle + "\r\n", true);
                }
                libMyUtil.clsThread.SetTextBox(txtItemList, item.itemTitle + "\r\n", true);
            }


            if (lstFiltered.Count > 0)
            {
                WriteStatus($"found {lstFiltered.Count} items", true);

                if (checkBox2.Checked)
                {
                    NotifyNewItem(lstFiltered.Count);
                }


                if (ValidateMailAccountInfo())
                {
                    objMail.SendMail(lstFiltered);
                }


                WriteStatus($"send jandi msg for {lstFiltered.Count} items", true);
                objJandi.SendJandi(lstFiltered);
            }
            
            WriteStatus("finish\r\n====================", true);
        }

        private void WriteStatus(string status, bool addNewLine)
        {
            if (txtStatus.Text.Length > 8000)
            {
                clsThread.SetTextBox(txtStatus, "", false);
            }

            if (addNewLine == true)
            {
                status += "\r\n";
            }
            clsThread.SetTextBox(txtStatus, status, true);
        }


        private void NotifyNewItem(int newItemCnt)
        {
            clsThread.SetLabel(this.Trigger, newItemCnt.ToString());
        }


        /// <summary>
        /// 검색 조건 확인
        /// </summary>
        /// <param name="item"></param>
        private bool FilterMonitoringItem(MonitoringItem item)      
        {
            bool result = false;

            string[] arrSrchWord01 = objUtil.Split(txtSrchWord01.Text.ToUpper(), " ");
            string[] arrSrchWord02 = objUtil.Split(txtSrchWord02.Text.ToUpper(), " ");
            string[] arrExceptWord = objUtil.Split(txtExceptWord.Text.ToUpper(), " ");

            if (txtSrchWord01.Text.Length > 0)
            {
                if (IsContainSearchWords(item, arrSrchWord01, arrExceptWord, true))
                {
                    if (arrSrchWord02[0].Length > 0)
                    {
                        result = IsContainSearchWords(item, arrSrchWord02, arrExceptWord, false);
                    }
                    else
                    {
                        result = true;
                    }
                }
            }
            else if (txtSrchWord02.Text.Length > 0)
            {
                result = IsContainSearchWords(item, arrSrchWord02, arrExceptWord, false);
            }
            else
            {
                result = true;
            }

            return result;
        }


        /// <summary>
        /// 검색어가 있으면 해당 검색어, 없으면 "" 리턴
        /// </summary>
        /// <param name="str">검사할 문자열</param>
        /// <param name="arrSearchWord">검색어가 들어간 배열</param>
        private bool IsContainSearchWords(MonitoringItem item, string[] arrSearchWord, string[] arrExceptWord, bool isSearchAllWord)
        {
            bool result = false;

            string itemTitle = item.itemTitle.Trim().Replace(" ", "");

            if (arrSearchWord != null && arrSearchWord.Length > 0 && arrSearchWord[0].Length > 0)
            {
                if (isSearchAllWord)
                {
                    if (arrSearchWord.Where(x => itemTitle.IndexOf(x) >= 0 || x.Trim().Length == 0).Count() == arrSearchWord.Length)
                    {
                        if (IsContainExceptWords(itemTitle, arrExceptWord) == false)
                        {
                            result = true;
                        }
                    }
                }
                else
                {
                    var objSearch = arrSearchWord.Where(x => itemTitle.IndexOf(x) >= 0 || x.Trim().Length == 0).FirstOrDefault();
                    if (objSearch != null)
                    {
                        if (IsContainExceptWords(itemTitle, arrExceptWord) == false)
                        {
                            result = true;
                        }
                    }
                }
            }

            return result;
        }


        private bool IsContainExceptWords(string itemTitle, string[] arrExceptWord)
        {
            bool result = false;

            if (arrExceptWord.Length > 0 && arrExceptWord[0].Trim().Length > 0 && arrExceptWord.Count(x => itemTitle.Contains(x)) > 0)
            {
                libMyUtil.clsThread.SetTextBox(txtFilteredItemList, itemTitle + DateTime.Now.ToString("[yyyy-MM-dd HH:mm:ss]") + "(제외됨)" + "\r\n", true);
                result = true;
            }

            return result;
        }


        /// <summary>
        /// 테스트메일 발송
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button2_Click(object sender, EventArgs e)
        {
            InitAlarm();

            List<MonitoringItem> lstTest = new List<MonitoringItem>() {
                new MonitoringItem() {
                    itemId = "",
                    itemTitle = "Test_" + DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss"),
                    itemUrl = ""
                }
            };

            if (ValidateMailAccountInfo())
            {
                objMail.SendMail(lstTest);
            }

            objJandi.SendJandi(lstTest);
        }


        private bool ValidateMailAccountInfo()
        {
            if (AccountID.Text.Length == 0 || AccountPass.Text.Length == 0)
                return false;
            else
            {
                if (rcvAddr.Enabled == true && rcvAddr.Text.Length == 0)
                    return false;
                else
                    return true;
            }
        }


        /// <summary>
        /// 노티 출력용 트리거
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Trigger_TextChanged(object sender, EventArgs e)
        {
            AnimationNotify objNotify;
            using (objNotify = new AnimationNotify())
            {
                objNotify.URL = "about:blank";
                objNotify.title = ((Label)sender).Text + "개의 새 게시물";
                objNotify.Show();
            }
        }


        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox1.Checked)
            {
                rcvAddr.Text = "";
                rcvAddr.Enabled = false;
            }
            else
                rcvAddr.Enabled = true;
        }


        private void Form1_Resize(object sender, EventArgs e)
        {
            if (FormWindowState.Minimized == this.WindowState)
            {
                myNotify.Visible = true;
                this.Hide();
            }
            else if (FormWindowState.Normal == this.WindowState)
            {
                myNotify.Visible = false;
            }
        }


        /*
        private void toRunningState()
        {
            SetTextBox(AccountID, "", false);
            SetTextBox(AccountPass, "", false);
            SetTextBox(rcvAddr, "", false);
            SetCheckBox(checkBox1, false);
            SetCheckBox(checkBox2, false);
            SetTextBox(srchWrd1, "", false);
            SetTextBox(srchWrd2, "", false);
            clsThread.buttonToggle(button2, "", false);
            clsThread.buttonToggle(button1, "중지", true);
        }

        private void toReadyState()
        {
            clsThread.buttonToggle(button1, "중지중...", true);
            clsThread.buttonToggle(button2, "", true);
            SetTextBox(srchWrd2, "", true);
            SetTextBox(srchWrd1, "", true);
            SetCheckBox(checkBox1, true);
            SetCheckBox(checkBox2, true);
            SetTextBox(rcvAddr, "", true);
            SetTextBox(AccountPass, "", true);
            SetTextBox(AccountID, "", true);

            while (t1.ThreadState != System.Threading.ThreadState.Stopped)
                System.Threading.Thread.Sleep(100);

            clsThread.buttonToggle(button1, "실행", true);
        }
        */
    }
}