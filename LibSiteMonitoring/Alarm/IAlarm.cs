namespace LibSiteMonitoring.Alarm
{
    public interface IAlarm
    {
        void Send(string msgTitle, string msgContent);
    }
}
