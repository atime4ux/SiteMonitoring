using LibSiteMonitoring.Model;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Net.Http;

namespace LibSiteMonitoring.Alarm
{
  public class Jandi : IAlarm
  {
    private readonly string MsgColor = "#FAC11B";

    public string WebHookUrl { get; set; }

    public Jandi(string url)
    {
      this.WebHookUrl = url;
    }

    public void Send(string msgTitle, string msgContent)
    {
      SendJandi(this.WebHookUrl, msgTitle, this.MsgColor, msgTitle, msgContent);
    }

    public void Send(string msgTitle, List<MonitoringItem> lstMonitoringItem)
    {
      string msgContent = new AlarmMsg(lstMonitoringItem).MakeMessage();
      SendJandi(this.WebHookUrl, "사이트 모니터링", this.MsgColor, msgTitle, msgContent);
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
