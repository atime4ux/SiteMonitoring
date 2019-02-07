using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SiteMonitoring3.Alarm
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
                System.Text.StringBuilder msg = new StringBuilder();

                foreach (MonitoringItem item in lstMonitoringItem)
                {
                    msg.AppendLine(item.itemTitle);
                    msg.AppendLine(item.itemUrl);
                    msg.AppendLine("=========================");
                }

                return msg.ToString();
            }
        }

        
    }
}
