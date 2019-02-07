using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Web.Script.Serialization;

namespace SiteMonitoring3.Alarm
{
    public class AlarmJandi
    {
        private string msgBody
        {
            get
            {
                return "사이트 모니터링 알림";
            }
        }

        private string msgColor
        {
            get
            {
                return "#FAC11B";
            }
        }

        private string webHookUrl { get; set; }

        public AlarmJandi(string url)
        {
            this.webHookUrl = url;
        }

        public void SendJandi(List<MonitoringItem> lst)
        {
            AlarmMsg objMsg = new Alarm.AlarmMsg(lst);
            SendJandi(this.webHookUrl, this.msgBody, this.msgColor, objMsg.Title, objMsg.Message);
        }

        public void SendJandi(string msgTitle, string msgContent)
        {
            SendJandi(this.webHookUrl, this.msgBody, this.msgColor, msgTitle, msgContent);
        }

        private void SendJandi(string webHookUrl, string jandiTitle, string jandiColor, string msgTitle, string msgContent)
        {
            if (string.IsNullOrEmpty(webHookUrl) == false)
            {
                string json = new JavaScriptSerializer().Serialize(new
                {
                    body = jandiTitle,
                    connectColor = jandiColor,
                    connectInfo = new object[] {
                    new {
                        title = msgTitle,
                        description = msgContent
                    }
                }
                });


                var client = new HttpClient();
                client.DefaultRequestHeaders.Clear();
                client.DefaultRequestHeaders.Add("ContentType", "application/json; charset=utf-8");
                client.DefaultRequestHeaders.Add("Accept", "application/vnd.tosslab.jandi-v2+json");


                HttpResponseMessage response;
                byte[] byteData = System.Text.Encoding.UTF8.GetBytes(json);

                using (var content = new ByteArrayContent(byteData))
                {
                    content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/json");
                    response = client.PostAsync(webHookUrl, content).Result;
                }
                var strResponseContent = response.Content.ReadAsStringAsync().Result;
            }
        }
    }
}
