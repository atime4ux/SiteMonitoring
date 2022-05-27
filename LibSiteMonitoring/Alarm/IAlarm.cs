using System.Collections.Generic;

namespace LibSiteMonitoring.Alarm
{
  public interface IAlarm
  {
    void Send(string msgTitle, string msgContent);
    void Send(string msgTitle, List<Model.MonitoringItem> lstItem);
  }
}
