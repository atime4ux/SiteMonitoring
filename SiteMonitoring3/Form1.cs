using LibSiteMonitoring;
using LibSiteMonitoring.Model;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Windows.Forms;


namespace SiteMonitoring3
{
    public partial class Form1 : Form
    {
        FormHelper FormHelper = new FormHelper();

        System.Threading.Thread threadMainJob;

        bool LoopFlag = false;

        private string LastMonitoringInfoFullPath
        {
            get
            {
                string path = System.IO.Path.GetDirectoryName(Application.ExecutablePath);
                string fileName = "LastMonitoringInfo.log";

                return path + "\\" + fileName;
            }
        }


        public string MonitoringInfoJson
        {
            get
            {
                return FormHelper.GetRichTextBoxValue(txtMonitoringInfoJson);
            }
        }


        public Form1()
        {
            InitializeComponent();
        }


        private void Form1_Load(object sender, EventArgs e)
        {
            InitControl();
        }


        private void InitControl()
        {
            Application.ApplicationExit += new EventHandler(Application_ApplicationExit);


            this.lblSleepRemain.Text = "";

            string jsonMonitoringInfo = GetLastMonitoringInfoFile();
            if (string.IsNullOrEmpty(jsonMonitoringInfo) == true)
            {
                jsonMonitoringInfo = JsonConvert.SerializeObject(Monitoring.GetSampleMonitoringInfo(), Formatting.Indented);
            }
            txtMonitoringInfoJson.Text = jsonMonitoringInfo;
        }


        private void ChangeRunningState()
        {
            System.Threading.Thread changeStateThread;

            if (LoopFlag)
            {
                changeStateThread = new System.Threading.Thread(new System.Threading.ThreadStart(delegate
                {
                    FormHelper.SetRichTextBox(txtMonitoringInfoJson, "", false);
                    FormHelper.SetLabel(lblSleepRemain, "");
                    FormHelper.buttonToggle(btnSendTestMsg, "", false);
                    FormHelper.buttonToggle(btnRun, "중지", true);
                    FormHelper.SetTextBox(txtFilteredItemList, "", "N");
                }));
            }
            else
            {
                changeStateThread = new System.Threading.Thread(new System.Threading.ThreadStart(delegate
                {
                    FormHelper.buttonToggle(btnRun, "중지중...", true);
                    FormHelper.buttonToggle(btnSendTestMsg, "", true);
                    FormHelper.SetLabel(lblSleepRemain, "");
                    FormHelper.SetRichTextBox(txtMonitoringInfoJson, "", true);

                    while (threadMainJob.ThreadState != System.Threading.ThreadState.Stopped)
                    {
                        System.Threading.Thread.Sleep(100);
                    }

                    FormHelper.buttonToggle(btnRun, "실행", true);
                }));
            }

            changeStateThread.Start();
        }


        private void WriteStatus(string status)
        {
            if (txtLog.Text.Length > 8000)
            {
                FormHelper.SetTextBox(txtLog, "", "N");
            }

            FormHelper.SetTextBox(txtLog, status + "\r\n", "Y");
        }

        private void WriteAllItem(List<MonitoringItem> lstItem)
        {
            FormHelper.SetTextBox(txtItemList, "", "N");
            foreach (MonitoringItem item in lstItem)
            {
                FormHelper.SetTextBox(txtItemList
                    , $"{string.Format("{0:#,0.######}", item.ItemPrice / 10000.0)}만원 - {item.ItemTitle}\r\n", "Y");
            }
        }


        private void WriteFilteredItem(List<MonitoringItem> lstFilteredItem)
        {
            foreach (MonitoringItem filtered in lstFilteredItem)
            {
                FormHelper.SetTextBox(txtFilteredItemList
                  , $"{string.Format("{0:#,0.######}", filtered.ItemPrice / 10000.0)}만원 - {filtered.ItemTitle}[{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")}]\r\n", "Y");
            }
        }


        private void WriteExceptedItem(List<ExceptedItem> lstExceptedItem)
        {
            foreach (ExceptedItem excepted in lstExceptedItem)
            {
                MonitoringItem exceptedItem = excepted.Item;
                FormHelper.SetTextBox(txtFilteredItemList
                  , $"{string.Format("{0:#,0.######}", excepted.Item.ItemPrice / 10000.0)}만원 - {excepted.Item.ItemTitle}[{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")}]제외됨({excepted.ExceptWord})\r\n", "Y");
            }
        }


        private void WriteSleepStatus(string sleepStatus)
        {
            FormHelper.SetLabel(lblSleepRemain, sleepStatus);
        }


        private void SaveMonitoringInfoFile()
        {
            LibSiteMonitoring.Helper.CommonHelper.OverwriteFile(LastMonitoringInfoFullPath, txtMonitoringInfoJson.Text);
        }


        private string GetLastMonitoringInfoFile()
        {
            return LibSiteMonitoring.Helper.CommonHelper.ReadFile(LastMonitoringInfoFullPath);
        }


        private bool GetLoopFlag()
        {
            return LoopFlag;
        }


        private void StartAndStop()
        {
            if (!LoopFlag)
            {
                //start
                SaveMonitoringInfoFile();

                LogActionGroup logAction = new LogActionGroup(
                  writeStatus: WriteStatus
                  , writeAllItem: WriteAllItem
                  , writeFilteredItem: WriteFilteredItem
                  , writeExceptedItem: WriteExceptedItem
                  , writeSleepStatus: WriteSleepStatus);

                Monitoring monitoring = new Monitoring(MonitoringInfoJson, GetLoopFlag, logAction);

                LoopFlag = true;
                ChangeRunningState();
                threadMainJob = new System.Threading.Thread(new System.Threading.ThreadStart(monitoring.RunMonitoring));
                threadMainJob.Name = "loopStart";
                threadMainJob.Start();
            }
            else
            {
                //stop
                LoopFlag = false;
                ChangeRunningState();
            }
        }


        private void btnRun_Click(object sender, EventArgs e)
        {
            StartAndStop();
        }


        private void Application_ApplicationExit(object sender, EventArgs e)
        {
            if (threadMainJob != null)
            {
                threadMainJob.Abort();
            }
        }


        private void btnSendTestMsg_Click(object sender, EventArgs e)
        {
            if (Monitoring.SendTestAlarm(MonitoringInfoJson))
            {
                MessageBox.Show("테스트 알림 전송");
            }
            else
            {
                MessageBox.Show("오류 발생");
            }
        }
    }
}