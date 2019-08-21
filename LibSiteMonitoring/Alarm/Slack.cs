using LibSiteMonitoring.Model;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Net.Http;

namespace LibSiteMonitoring.Alarm
{
  public class Slack : IAlarm
  {
    public string webHookUrl { get; set; }

    public Slack(string url)
    {
      this.webHookUrl = url;
    }

    public void Send(string msgTitle, string msgContent)
    {
      string jsonData = JsonConvert.SerializeObject(new
      {
        username = msgTitle,
        text = msgContent
      });
      SendSlack(this.webHookUrl, jsonData);
    }

    public void Send(string msgTitle, List<MonitoringItem> lstMonitoringItem)
    {
      string jsonData = MakeSlackMessage(msgTitle, lstMonitoringItem);
      SendSlack(this.webHookUrl, jsonData);
    }

    private void SendSlack(string webHookUrl, string jsonData)
    {
      if (string.IsNullOrEmpty(webHookUrl) == false)
      {
        var client = new HttpClient();
        client.DefaultRequestHeaders.Clear();
        client.DefaultRequestHeaders.Add("ContentType", "application/x-www-form-urlencoded; charset=utf-8");


        HttpResponseMessage response;
        byte[] byteData = System.Text.Encoding.UTF8.GetBytes($"payload={jsonData}");

        using (var content = new ByteArrayContent(byteData))
        {
          content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/x-www-form-urlencoded");
          response = client.PostAsync(webHookUrl, content).Result;
        }
        var strResponseContent = response.Content.ReadAsStringAsync().Result;
      }
    }

    private string MakeSlackMessage(string msgTitle, List<MonitoringItem> lstMonitoringItem)
    {
      System.Text.StringBuilder msgContent = new System.Text.StringBuilder();
      foreach (MonitoringItem item in lstMonitoringItem)
      {
        msgContent.AppendLine($"<{System.Net.WebUtility.UrlEncode(item.itemUrl)}|{item.itemTitle}>");
        msgContent.AppendLine("=========================");
      }

      string jsonData = JsonConvert.SerializeObject(new
      {
        username = msgTitle,
        text = msgContent.ToString()
      });

      return jsonData;
    }
  }
}
