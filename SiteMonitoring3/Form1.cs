using Newtonsoft.Json;
using SiteMonitoring3.Alarm;
using SiteMonitoring3.Parsing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;


namespace SiteMonitoring3
{
    public partial class Form1 : Form
    {
        Helper.Thread threadHelper = new Helper.Thread();
        Helper.Common commonHelper = new Helper.Common();


        System.Threading.Thread threadMainJob;


        Joonggonara objJoonggonara = null;


        Jandi objJandi = null;
        Mail objMail = null;


        bool loopFlag = false;


        readonly int minSleepSecond = 10;


        private string lastKeywordFullPath
        {
            get
            {
                string path = System.IO.Path.GetDirectoryName(Application.ExecutablePath);
                string fileName = "lastKeyword.log";

                return path + "\\" + fileName;
            }
        }


        public List<Keyword> lstKeyword
        {
            get
            {
                List<Keyword> result = null;

                if (txtKeywordJson.Text.Trim().Length > 0)
                {
                    try
                    {
                        result = JsonConvert.DeserializeObject<List<Keyword>>(txtKeywordJson.Text);
                    }
                    catch (Exception ex)
                    { }
                }

                return result;
            }
        }



        public Form1()
        {
            InitializeComponent();
        }


        private void Form1_Load(object sender, EventArgs e)
        {
            InitControl();
            InitParser();
        }


        private void InitParser()
        {
            objJoonggonara = new Joonggonara(WriteStatus);
        }


        private void InitControl()
        {
            Application.ApplicationExit += new EventHandler(Application_ApplicationExit);

            this.lblSleepRemain.Text = "";

            string jsonKeyword = GetLastKeyword();
            if (string.IsNullOrEmpty(jsonKeyword) == true)
            {
                jsonKeyword = JsonConvert.SerializeObject(new List<Keyword>() {
                    new Keyword("필수 검색어1", "선택 검색어1", "제외 검색어1"),
                    new Keyword("필수 검색어2", "선택 검색어2", "제외 검색어2")
                }, Formatting.Indented);
            }
            txtKeywordJson.Text = jsonKeyword;
        }


        private void InitAlarm()
        {
            objJandi = new Jandi(commonHelper.GetAppCfg("jandiWebHookUrl"));
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
                    threadHelper.SetTextBox(txtKeywordJson, "", false);
                    threadHelper.SetLabel(lblSleepRemain, "");
                    threadHelper.buttonToggle(btnSendTestMsg, "", false);
                    threadHelper.buttonToggle(btnRun, "중지", true);
                    threadHelper.SetTextBox(txtFilteredItemList, "", "N");
                }));
            }
            else
            {
                changeStateThread = new System.Threading.Thread(new System.Threading.ThreadStart(delegate {
                    threadHelper.buttonToggle(btnRun, "중지중...", true);
                    threadHelper.buttonToggle(btnSendTestMsg, "", true);
                    threadHelper.SetLabel(lblSleepRemain, "");
                    threadHelper.SetTextBox(txtKeywordJson, "", true);
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


        private void WriteAllItem(List<MonitoringItem> lstItem)
        {
            threadHelper.SetTextBox(txtItemList, "", "N");
            foreach (MonitoringItem item in lstItem)
            {
                threadHelper.SetTextBox(txtItemList, item.itemTitle + "\r\n", "Y");
            }
        }


        private void WriteFilteredItem(List<MonitoringItem> lstFilteredItem)
        {
            foreach (MonitoringItem filtered in lstFilteredItem)
            {
                threadHelper.SetTextBox(txtFilteredItemList, $"{filtered.itemTitle}[{System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")}]\r\n", "Y");
            }
        }


        private void WriteExceptedItem(List<ExceptedItem> lstExceptedItem)
        {
            foreach (ExceptedItem excepted in lstExceptedItem)
            {
                threadHelper.SetTextBox(txtFilteredItemList, $"{excepted.item.itemTitle}[{System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")}]제외됨({excepted.exceptWord})\r\n", "Y");
            }
        }


        private void SaveKeyword()
        {
            commonHelper.OverwriteFile(lastKeywordFullPath, txtKeywordJson.Text);
        }


        private string GetLastKeyword()
        {
            return commonHelper.ReadFile(lastKeywordFullPath);
        }


        private void StartAndStop()
        {
            if (lstKeyword != null
                && (lstKeyword.Count(x => x.requireKeywords.Trim().Length > 0
                    || x.optionKeywords.Trim().Length > 0) > 0))
            {
                if (!loopFlag)
                {
                    //start
                    InitAlarm();

                    SaveKeyword();

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
            

            WriteStatus("downloading data");
            List<MonitoringItem> lstAll = objJoonggonara.GetMonitoringList();


            if (objJoonggonara.isBlocked == false)
            {
                if (lstAll.Count < 15)
                {
                    objJoonggonara.sleepSecond = objJoonggonara.sleepSecond * 2;
                }
                else
                {
                    objJoonggonara.sleepSecond = Math.Max(minSleepSecond, objJoonggonara.sleepSecond / 2);
                }

                WriteStatus($"success get {lstAll.Count} items");

                WriteAllItem(lstAll);


                FilterResult filterResult = new Filter().FilterItems(lstAll, lstKeyword);
                WriteFilteredItem(filterResult.lstFiltered);
                WriteExceptedItem(filterResult.lstExcepted);


                if (filterResult.lstFiltered.Count > 0)
                {
                    WriteStatus($"found {filterResult.lstFiltered.Count} items");

                    if (ValidateMailAccountInfo())
                    {
                        objMail.Send(filterResult.lstFiltered);
                    }


                    WriteStatus($"send jandi msg for {filterResult.lstFiltered.Count} items");
                    objJandi.Send(filterResult.lstFiltered);
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
                    WriteStatus($"start sleep {objJoonggonara.sleepSecond} sec");
                    for (int i = 0; i < objJoonggonara.sleepSecond; i++)
                    {
                        if (loopFlag == true)
                        {
                            threadHelper.SetLabel(lblSleepRemain, $"sleep remain {(objJoonggonara.sleepSecond - i).ToString()}");
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


        private void btnSendTestMsg_Click(object sender, EventArgs e)
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
    }
}