using LibSiteMonitoring.Model;
using Newtonsoft.Json;
using System.Collections.Generic;
using System;

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
    public AlarmType AlarmTypeName { get; set; }
    public string ConnectionString { get; set; }
		private IAlarm Alarm { get; set; }

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
      this.AlarmTypeName = type;
      this.ConnectionString = conStr;
    }

    public static List<AlarmInfo> GetAllAlarmInfo()
    {
      return new List<AlarmInfo>() {
        new AlarmInfo(AlarmType.email, "id;password;from;to"),
        new AlarmInfo(AlarmType.jandi, "jandiWebhookUrl"),
        new AlarmInfo(AlarmType.slack, "slackWebhookUrl")
        };
    }

    private bool InitAlarm()
    {
      bool result = true;

      if (string.IsNullOrEmpty(this.ConnectionString) == false)
      {
        switch (this.AlarmTypeName)
        {
          case AlarmType.jandi:
            Alarm = new Jandi(this.ConnectionString);
            break;
          case AlarmType.slack:
            Alarm = new Slack(this.ConnectionString);
            break;
          case AlarmType.email:
            string[] arrCon = this.ConnectionString.Split(';');
            Alarm = new Mail(
              id: arrCon[0]
              , password: arrCon[1]
              , mailFrom: arrCon[2]
              , mailTo: arrCon[3]
              );
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
        Alarm.Send(Title, lst);
      }
    }

    public void SendAlarm(string title, string content)
    {
      if (InitAlarm() == true)
      {
        Alarm.Send(title, content);
      }
    }
  }
}
