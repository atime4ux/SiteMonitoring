using System;
using LibSiteMonitoring.Model;
using System.Collections.Generic;

namespace LibSiteMonitoring.Parsing
{
    public class BaseParsing
    {
        /// <summary>
        /// 지난번 itemId 여기 도달할때까지 처리한다
        /// </summary>
        protected MonitoringItem lastItem { get; set; }

        /// <summary>
        /// 지난번 실행된 시간
        /// </summary>
        protected DateTime lastRunDate { get; set; }

        protected ParsingTarget parsingTarget { get; set; }
        protected int sleepSecond { get; set; }

        protected bool isBlocked { get; set; }

        public BaseParsing()
        {
            sleepSecond = 10;
            lastRunDate = DateTime.Now;
        }

        public ParsingTarget GetParsingTarget()
        {
            return parsingTarget;
        }

        public void SetSleepSecond(int sec)
        {
            sleepSecond = sec;
        }

        public int GetSleepSecond()
        {
            return sleepSecond;
        }
    }
}
