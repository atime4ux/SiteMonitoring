using LibSiteMonitoring.Alarm;
using System.Collections.Generic;

namespace LibSiteMonitoring.Model
{
	public class MonitoringInfo
	{
		public List<Keyword> LstKeyword { get; set; }
		public List<AlarmInfo> LstAlarmInfo { get; set; }

		public void SendAlarm(List<MonitoringItem> lst)
		{
			if (this.LstAlarmInfo != null)
			{
				foreach (var alarmInfo in this.LstAlarmInfo)
				{
					alarmInfo.SendAlarm(lst);
				}
			}
		}

		public void SendAlarm(string title, string content)
		{
			foreach (var alarmInfo in this.LstAlarmInfo)
			{
				alarmInfo.SendAlarm(title, content);
			}
		}
	}
}
