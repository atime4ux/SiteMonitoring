using LibSiteMonitoring.Model;
using System.Collections.Generic;

namespace LibSiteMonitoring.Alarm
{
  public class AlarmMsg
  {
    private List<MonitoringItem> LstMonitoringItem { get; set; }

    public AlarmMsg(List<MonitoringItem> lst)
    {
      this.LstMonitoringItem = lst;
    }

    public string MakeMessage()
    {
      System.Text.StringBuilder msg = new System.Text.StringBuilder();

      foreach (MonitoringItem item in this.LstMonitoringItem)
      {
        msg.AppendLine(item.ItemTitle);
        msg.AppendLine(item.ItemUrlPc);
        msg.AppendLine(item.ItemUrlMobile);
        msg.AppendLine("=========================");
      }

      return msg.ToString();
    }

    public string MakeSlackMessage()
    {
      System.Text.StringBuilder msg = new System.Text.StringBuilder();
      foreach (MonitoringItem item in this.LstMonitoringItem)
      {
        msg.AppendLine(item.ItemTitle);
        msg.AppendLine($"{(item.ItemPrice / 10000.0).ToString("#,###,###,##0.#")}만원");
        msg.AppendLine($"<{System.Net.WebUtility.UrlEncode(item.ItemUrlPc)}|PC> <{System.Net.WebUtility.UrlEncode(item.ItemUrlMobile)}|Mobile>");
        msg.AppendLine("=========================");
      }

      return msg.ToString();
    }
  }
}
