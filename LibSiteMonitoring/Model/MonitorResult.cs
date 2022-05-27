using LibSiteMonitoring.Alarm;
using System.Collections.Generic;

namespace LibSiteMonitoring.Model
{
	public class MonitorResult
	{
		public List<MonitoringItem> FilteredItems { get; set; }
		public List<ExceptedItem> ExceptedItems { get; set; }

		public MonitorResult()
		{
			FilteredItems = new List<MonitoringItem>();
			ExceptedItems = new List<ExceptedItem>();
		}
	}
}
