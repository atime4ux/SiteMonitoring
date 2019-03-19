using LibSiteMonitoring.Alarm;
using System.Collections.Generic;

namespace LibSiteMonitoring.Model
{
    public class MonitoringInfo
    {
        public List<Keyword> lstKeyword { get; set; }
        public List<AlarmInfo> lstAlarmInfo { get; set; }

        public void SendAlarm(List<MonitoringItem> lst)
        {
            if (this.lstAlarmInfo != null)
            {
                foreach (var alarmInfo in this.lstAlarmInfo)
                {
                    alarmInfo.SendAlarm(lst);
                }
            }
        }

        public void SendAlarm(string title, string content)
        {
            foreach (var alarmInfo in this.lstAlarmInfo)
            {
                alarmInfo.SendAlarm(title, content);
            }
        }
    }
}
