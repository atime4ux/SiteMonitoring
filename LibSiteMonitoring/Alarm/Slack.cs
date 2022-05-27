using LibSiteMonitoring.Model;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace LibSiteMonitoring.Alarm
{
	public class Slack : IAlarm
  {
    public string WebHookUrl { get; set; }

    public Slack(string url)
    {
      this.WebHookUrl = url;
    }

    public async void Send(string msgTitle, string msgContent)
    {
      string jsonData = JsonConvert.SerializeObject(new
      {
        username = msgTitle,
        text = msgContent
      });
      await SendSlack(this.WebHookUrl, jsonData);
    }

    public async void Send(string msgTitle, List<MonitoringItem> lstMonitoringItem)
    {
      string jsonData = JsonConvert.SerializeObject(new
      {
        username = msgTitle,
        text = new AlarmMsg(lstMonitoringItem).MakeSlackMessage()
      });

      await SendSlack(this.WebHookUrl, jsonData);
    }

    private async Task<string> SendSlack(string webHookUrl, string jsonData)
    {
      string result = "";

      if (string.IsNullOrEmpty(webHookUrl) == false)
      {
        var client = new HttpClient();

        ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls;

        client.DefaultRequestHeaders.Clear();
        client.DefaultRequestHeaders.Add("ContentType", "application/x-www-form-urlencoded; charset=utf-8");


        HttpResponseMessage response;
        byte[] byteData = System.Text.Encoding.UTF8.GetBytes($"payload={jsonData}");

        using (var content = new ByteArrayContent(byteData))
        {
          content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/x-www-form-urlencoded");
          response = await client.PostAsync(webHookUrl, content);
        }
        result = await response.Content.ReadAsStringAsync();
      }

      return result;
    }
  }
}
