using LibSiteMonitoring.Alarm;
using System.Collections.Generic;
using System;

namespace LibSiteMonitoring.Model
{
	public class MonitoringInfo
	{
		public string Title { get; set; }
		public List<Keyword> LstKeyword { get; set; }
		public List<AlarmInfo> LstAlarmInfo { get; set; }
		public int AliveAlarmTermMinute { get; set; }

		private DateTime LastSendAliveTime = DateTime.MinValue;

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

		public void SendAlarm(string alarmTitle, string alarmContent)
		{
			foreach (var alarmInfo in this.LstAlarmInfo)
			{
				alarmInfo.SendAlarm(alarmTitle, alarmContent);
			}
		}

		public void SendAliveSignal()
		{
			DateTime dateNow = DateTime.Now;
			if (LastSendAliveTime.AddMinutes(AliveAlarmTermMinute) < dateNow)
			{
				SendAlarm($"{Title} - Alive", dateNow.ToString("yyyy-MM-dd hh:mm:ss"));
				LastSendAliveTime = dateNow;
			}
		}

		public MonitorResult FilterItems(List<MonitoringItem> lstAll)
		{
			SendAliveSignal();
			return Keyword.FilterItems(lstAll, LstKeyword);
		}
	}
}
