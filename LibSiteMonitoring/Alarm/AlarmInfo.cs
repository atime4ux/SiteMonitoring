using LibSiteMonitoring.Model;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace LibSiteMonitoring.Alarm
{
  [JsonConverter(typeof(Newtonsoft.Json.Converters.StringEnumConverter))]
  public enum AlarmType
  {
    email,
    jandi,
    slack
  }

  public class AlarmInfo
  {
    public AlarmType alarmType { get; set; }
    public string connectionString { get; set; }
    private IAlarm alarm { get; set; }

    private string Title
    {
      get
      {
        return "Site Monitoring - " + System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
      }
    }

    public AlarmInfo()
    { }

    public AlarmInfo(AlarmType type, string conStr)
    {
      this.alarmType = type;
      this.connectionString = conStr;
    }

    private bool InitAlarm()
    {
      bool result = true;

      if (string.IsNullOrEmpty(this.connectionString) == false)
      {
        switch (this.alarmType)
        {
          case AlarmType.jandi:
            alarm = new Jandi(this.connectionString);
            break;
          case AlarmType.slack:
            alarm = new Slack(this.connectionString);
            break;
          case AlarmType.email:
            string[] arrCon = this.connectionString.Split(';');
            alarm = new Mail(id: arrCon[0],
                            password: arrCon[1],
                            mailFrom: arrCon[2],
                            mailTo: arrCon[3]);
            break;
          default:
            break;
        }
      }
      else
      {
        result = false;
      }

      return result;
    }

    public void SendAlarm(List<MonitoringItem> lst)
    {
      if (InitAlarm() == true)
      {
        alarm.Send(Title, lst);
      }
    }

    public void SendAlarm(string title, string content)
    {
      if (InitAlarm() == true)
      {
        alarm.Send(title, content);
      }
    }
  }
}
