using LibSiteMonitoring.Model;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace LibSiteMonitoring.Parsing
{
    public interface IParsing
    {
        ParsingTarget GetParsingTarget();

        void SetSleepSecond(int sec);

        int GetSleepSecond();

        Task<List<MonitoringItem>> GetMonitoringList();
    }

    public enum ParsingTarget
    {
        중고나라,
        클리앙,
        뽐뿌
    }
}
