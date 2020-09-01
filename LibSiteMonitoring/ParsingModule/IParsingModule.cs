using LibSiteMonitoring.Model;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace LibSiteMonitoring.ParsingModule
{
	public interface IParsingModule
	{
		SiteName GetParsingTarget();

		void SetSleepSecond(int sec);

		int GetSleepSecond();

		Task<List<MonitoringItem>> GetMonitoringList();
	}

	public enum SiteName
	{
		중고나라,
		클리앙,
		뽐뿌
	}
}
