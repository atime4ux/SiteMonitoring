using SiteMonitoring3.Alarm;
using SiteMonitoring3.Parsing;
using System;
using System.Collections.Generic;
using System.Windows.Forms;


namespace SiteMonitoring3
{
    public partial class Form1 : Form
    {
        Helper.Thread threadHelper = new Helper.Thread();
        Helper.Common commonHelper = new Helper.Common();


        System.Threading.Thread threadMainJob;
        

        Jandi objJandi = null;
        Mail objMail = null;


        bool loopFlag;


        readonly int minSleepSecond = 10;
        int curSleepSecond = 10;
        

        public Form1()
        {
            InitializeComponent();
        }


        private void Form1_Load(object sender, EventArgs e)
        {
            InitControl();
            loopFlag = false;
        }


        private void InitControl()
        {
            Application.ApplicationExit += new EventHandler(Application_ApplicationExit);
            
            this.lblSleepRemain.Text = "";
        }


        private void InitAlarm()
        {
            objJandi = new Jandi(commonHelper.getAppCfg("jandiWebHookUrl"));
            objMail = new Mail(txtGmailID.Text,
                                    txtGmailPass.Text,
                                    txtReceiveMailAddress.Text,
                                    txtReceiveMailAddress.Text);
        }


        private bool ValidateMailAccountInfo()
        {
            bool result = false;

            if (txtGmailID.Text.Length > 0
                && txtGmailPass.Text.Length > 0
                && (txtReceiveMailAddress.Text.Length > 0 || chkUsingMailAccount.Checked == true))
            {
                result = true;
            }

            return result;
        }


        private void changeRunningState()
        {
            System.Threading.Thread changeStateThread;

            if (loopFlag)
            {
                changeStateThread = new System.Threading.Thread(new System.Threading.ThreadStart(delegate
                {
                    threadHelper.SetTextBox(txtGmailID, "", false);
                    threadHelper.SetTextBox(txtGmailPass, "", false);
                    threadHelper.SetTextBox(txtReceiveMailAddress, "", false);
                    threadHelper.SetCheckBox(chkUsingMailAccount, false);
                    threadHelper.SetTextBox(txtSrchWord01, "", false);
                    threadHelper.SetTextBox(txtSrchWord02, "", false);
                    threadHelper.SetTextBox(txtExceptWord, "", false);
                    threadHelper.SetLabel(lblSleepRemain, "");
                    threadHelper.buttonToggle(btnSendTestMail, "", false);
                    threadHelper.buttonToggle(btnRun, "중지", true);
                    threadHelper.SetTextBox(txtFilteredItemList, "", "N");
                }));
            }
            else
            {
                changeStateThread = new System.Threading.Thread(new System.Threading.ThreadStart(delegate {
                    threadHelper.buttonToggle(btnRun, "중지중...", true);
                    threadHelper.buttonToggle(btnSendTestMail, "", true);
                    threadHelper.SetLabel(lblSleepRemain, "");
                    threadHelper.SetTextBox(txtExceptWord, "", true);
                    threadHelper.SetTextBox(txtSrchWord02, "", true);
                    threadHelper.SetTextBox(txtSrchWord01, "", true);
                    threadHelper.SetCheckBox(chkUsingMailAccount, true);
                    threadHelper.SetTextBox(txtReceiveMailAddress, "", true);
                    threadHelper.SetTextBox(txtGmailPass, "", true);
                    threadHelper.SetTextBox(txtGmailID, "", true);

                    while (threadMainJob.ThreadState != System.Threading.ThreadState.Stopped)
                    {
                        System.Threading.Thread.Sleep(100);
                    }

                    threadHelper.buttonToggle(btnRun, "실행", true);
                }));
            }

            changeStateThread.Start();
        }


        private void WriteStatus(string status)
        {
            if (txtLog.Text.Length > 8000)
            {
                threadHelper.SetTextBox(txtLog, "", "N");
            }

            threadHelper.SetTextBox(txtLog, status + "\r\n", "Y");
        }


        private void StartAndStop()
        {
            if (txtSrchWord01.Text.Trim().Length > 0 || txtSrchWord02.Text.Trim().Length > 0)
            {
                if (!loopFlag)
                {
                    //start
                    InitAlarm();

                    loopFlag = true;
                    changeRunningState();
                    threadMainJob = new System.Threading.Thread(new System.Threading.ThreadStart(MainJob));
                    threadMainJob.Name = "loopStart";
                    threadMainJob.Start();
                }
                else
                {
                    //stop
                    loopFlag = false;
                    changeRunningState();
                }
            }
            else
            {
                MessageBox.Show("검색어 입력");
            }
        }


        private void SubJob()
        {
            WriteStatus("start");

            Joonggonara objJoonggonara = new Joonggonara(WriteStatus);
            Keyword objKeyword = new Keyword(txtSrchWord01.Text, txtSrchWord02.Text, txtExceptWord.Text);


            WriteStatus("downloading data");
            List<MonitoringItem> lstAll = objJoonggonara.GetMonitoringList();
            List<MonitoringItem> lstFiltered = new List<MonitoringItem>();

            if (objJoonggonara.isBlocked == false)
            {
                if (lstAll.Count < 15)
                {
                    curSleepSecond = curSleepSecond * 2;
                }
                else
                {
                    curSleepSecond = Math.Max(minSleepSecond, curSleepSecond / 2);
                }

                WriteStatus($"success get {lstAll.Count} items");

                threadHelper.SetTextBox(txtItemList, "", "N");
                foreach (MonitoringItem item in lstAll)
                {
                    threadHelper.SetTextBox(txtItemList, item.itemTitle + "\r\n", "Y");

                    Filter objFilter = new Filter();
                    if (objFilter.FilterMonitoringItem(item, objKeyword) == true)
                    {
                        lstFiltered.Add(item);
                        threadHelper.SetTextBox(txtFilteredItemList, $"{item.itemTitle}[{System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")}]\r\n", "Y");
                    }
                    else
                    {
                        if (objFilter.isExcepted == true)
                        {
                            threadHelper.SetTextBox(txtFilteredItemList, $"{item.itemTitle}[{System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")}]제외됨\r\n", "Y");
                        }
                    }
                }


                if (lstFiltered.Count > 0)
                {
                    WriteStatus($"found {lstFiltered.Count} items");

                    if (ValidateMailAccountInfo())
                    {
                        objMail.Send(lstFiltered);
                    }


                    WriteStatus($"send jandi msg for {lstFiltered.Count} items");
                    objJandi.Send(lstFiltered);
                }
            }
            else
            {
                objJandi.Send("SiteMonitoring", "blocked");
                StartAndStop();
            }

            WriteStatus("finish\r\n====================");
        }


        private void MainJob()
        {
            while (loopFlag)
            {
                System.Threading.Thread threadSubJob = new System.Threading.Thread(new System.Threading.ThreadStart(SubJob));
                threadSubJob.Name = "DownloadAndParsing";
                threadSubJob.Start();
                threadSubJob.Join();

                if (loopFlag)
                {
                    WriteStatus($"start sleep {curSleepSecond} sec");
                    for (int i = 0; i < curSleepSecond; i++)
                    {
                        if (loopFlag == true)
                        {
                            threadHelper.SetLabel(lblSleepRemain, $"sleep remain {(curSleepSecond - i).ToString()}");
                            System.Threading.Thread.Sleep(1000);
                        }
                        else
                        {
                            break;
                        }
                    }
                    threadHelper.SetLabel(lblSleepRemain, "");
                    WriteStatus("end sleep");
                }
            }
        }


        private void btnRun_Click(object sender, EventArgs e)
        {
            StartAndStop();
        }


        void Application_ApplicationExit(object sender, EventArgs e)
        {
            if (threadMainJob != null)
            {
                threadMainJob.Abort();
            }
        }


        private void btnSendTestMail_Click(object sender, EventArgs e)
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
                objMail.Send(lstTest);
            }

            objJandi.Send(lstTest);
        }


        private void chkGmail_CheckedChanged(object sender, EventArgs e)
        {
            if (chkUsingMailAccount.Checked)
            {
                txtReceiveMailAddress.Text = "";
                txtReceiveMailAddress.Enabled = false;
            }
            else
            {
                txtReceiveMailAddress.Enabled = true;
            }
        }


        private void Form1_Resize(object sender, EventArgs e)
        {
            if (FormWindowState.Minimized == this.WindowState)
            {
                this.Hide();
            }
        }
    }
}