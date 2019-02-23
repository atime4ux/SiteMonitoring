using LibSiteMonitoring;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Windows.Forms;


namespace SiteMonitoring3
{
    public partial class Form1 : Form
    {
        FormHelper FormHelper = new FormHelper();
        LibSiteMonitoring.Helper.Common commonHelper = new LibSiteMonitoring.Helper.Common();


        System.Threading.Thread threadMainJob;


        bool loopFlag = false;


        private string lastMonitoringInfoFullPath
        {
            get
            {
                string path = System.IO.Path.GetDirectoryName(Application.ExecutablePath);
                string fileName = "lastMonitoringInfo.log";

                return path + "\\" + fileName;
            }
        }


        public string monitoringInfoJson
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
                jsonMonitoringInfo = JsonConvert.SerializeObject(Monitoring.GetMonitoringInfoSample(), Formatting.Indented);
            }
            txtMonitoringInfoJson.Text = jsonMonitoringInfo;
        }


        private void changeRunningState()
        {
            System.Threading.Thread changeStateThread;

            if (loopFlag)
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
                changeStateThread = new System.Threading.Thread(new System.Threading.ThreadStart(delegate {
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


        private void WriteAllItem(List<LibSiteMonitoring.Model.MonitoringItem> lstItem)
        {
            FormHelper.SetTextBox(txtItemList, "", "N");
            foreach (LibSiteMonitoring.Model.MonitoringItem item in lstItem)
            {
                FormHelper.SetTextBox(txtItemList, item.itemTitle + "\r\n", "Y");
            }
        }


        private void WriteFilteredItem(List<LibSiteMonitoring.Model.MonitoringItem> lstFilteredItem)
        {
            foreach (LibSiteMonitoring.Model.MonitoringItem filtered in lstFilteredItem)
            {
                FormHelper.SetTextBox(txtFilteredItemList, $"{filtered.itemTitle}[{System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")}]\r\n", "Y");
            }
        }


        private void WriteExceptedItem(List<LibSiteMonitoring.Model.ExceptedItem> lstExceptedItem)
        {
            foreach (LibSiteMonitoring.Model.ExceptedItem excepted in lstExceptedItem)
            {
                FormHelper.SetTextBox(txtFilteredItemList, $"{excepted.item.itemTitle}[{System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")}]제외됨({excepted.exceptWord})\r\n", "Y");
            }
        }


        private void WriteSleepStatus(string sleepStatus)
        {
            FormHelper.SetLabel(lblSleepRemain, sleepStatus);
        }


        private void SaveMonitoringInfoFile()
        {
            commonHelper.OverwriteFile(lastMonitoringInfoFullPath, txtMonitoringInfoJson.Text);
        }


        private string GetLastMonitoringInfoFile()
        {
            return commonHelper.ReadFile(lastMonitoringInfoFullPath);
        }


        private bool GetLoopFlag()
        {
            return loopFlag;
        }


        private void StartAndStop()
        {
            if (!loopFlag)
            {
                //start
                SaveMonitoringInfoFile();

                Monitoring monitoring = new Monitoring(monitoringInfoJson, GetLoopFlag, WriteStatus, WriteAllItem, WriteFilteredItem, WriteExceptedItem, WriteSleepStatus);

                loopFlag = true;
                changeRunningState();
                threadMainJob = new System.Threading.Thread(new System.Threading.ThreadStart(monitoring.MainJob));
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
            if (Monitoring.SendTestAlarm(monitoringInfoJson))
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