using System;
using System.Collections.Generic;

namespace LibSiteMonitoring.Model
{
	public class LogActionGroup
	{
		public Action<string> WriteStatus { get; set; }
		public Action<List<MonitoringItem>> WriteAllItem { get; set; }
		public Action<List<MonitoringItem>> WriteFilteredItem { get; set; }
		public Action<List<ExceptedItem>> WriteExceptedItem { get; set; }
		public Action<string> WriteSleepStatus { get; set; }
		
		public LogActionGroup(Action<string> writeStatus
			, Action<List<MonitoringItem>> writeAllItem
			, Action<List<MonitoringItem>> writeFilteredItem
			, Action<List<ExceptedItem>> writeExceptedItem
			, Action<string> writeSleepStatus)
		{
			WriteStatus = writeStatus != null ? writeStatus : (x => { });
			WriteAllItem = writeAllItem != null ? writeAllItem : (x => { });
			WriteFilteredItem = writeFilteredItem != null ? writeFilteredItem : (x => { });
			WriteExceptedItem = writeExceptedItem != null ? writeExceptedItem : (x => { });
			WriteSleepStatus = writeSleepStatus != null ? writeSleepStatus : (x => { });
		}
	}
}
