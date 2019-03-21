using LibSiteMonitoring.Model;
using System.Collections.Generic;

namespace LibSiteMonitoring.Parsing
{
    public interface IParsing
    {
        ParsingTarget GetParsingTarget();

        void SetSleepSecond(int sec);

        int GetSleepSecond();

        List<MonitoringItem> GetMonitoringList();
    }

    public enum ParsingTarget
    {
        중고나라,
        클리앙,
        뽐뿌
    }
}
