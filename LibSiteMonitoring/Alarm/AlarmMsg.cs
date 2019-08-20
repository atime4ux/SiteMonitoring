using LibSiteMonitoring.Model;
using System.Collections.Generic;

namespace LibSiteMonitoring.Alarm
{
  public class AlarmMsg
  {
    private List<MonitoringItem> lstMonitoringItem { get; set; }

    public AlarmMsg(List<MonitoringItem> lst)
    {
      this.lstMonitoringItem = lst;
    }

    public string Title
    {
      get
      {
        return "Site Monitoring - " + System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
      }
    }

    public string Message
    {
      get
      {
        return MakeMessage();
      }
    }

    public string MakeMessage()
    {
      System.Text.StringBuilder msg = new System.Text.StringBuilder();

      foreach (MonitoringItem item in this.lstMonitoringItem)
      {
        msg.AppendLine(item.itemTitle);
        msg.AppendLine(item.itemUrl);
        msg.AppendLine("=========================");
      }

      return msg.ToString();
    }
  }
}
