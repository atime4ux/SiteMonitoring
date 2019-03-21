using LibSiteMonitoring.Alarm;
using System.Collections.Generic;

namespace LibSiteMonitoring.Model
{
    public class FilterResult
    {
        public List<MonitoringItem> lstFiltered { get; set; }
        public List<ExceptedItem> lstExcepted { get; set; }

        public FilterResult()
        {
            lstFiltered = new List<MonitoringItem>();
            lstExcepted = new List<ExceptedItem>();
        }
    }
}
