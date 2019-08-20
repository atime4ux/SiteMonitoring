using LibSiteMonitoring.Model;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Net.Http;

namespace LibSiteMonitoring.Alarm
{
  public class Jandi : IAlarm
  {
    private string msgColor
    {
      get
      {
        return "#FAC11B";
      }
    }

    public string webHookUrl { get; set; }

    public Jandi(string url)
    {
      this.webHookUrl = url;
    }

    public void Send(string msgTitle, string msgContent)
    {
      SendJandi(this.webHookUrl, msgTitle, this.msgColor, msgTitle, msgContent);
    }

    public void Send(string msgTitle, List<MonitoringItem> lstMonitoringItem)
    {
      string msgContent = new AlarmMsg(lstMonitoringItem).MakeMessage();
      SendJandi(this.webHookUrl, "사이트 모니터링", this.msgColor, msgTitle, msgContent);
    }

    private void SendJandi(string webHookUrl, string jandiTitle, string jandiColor, string msgTitle, string msgContent)
    {
      if (string.IsNullOrEmpty(webHookUrl) == false)
      {
        string json = JsonConvert.SerializeObject(new
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
