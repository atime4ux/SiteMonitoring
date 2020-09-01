using LibSiteMonitoring.Model;
using System.Collections.Generic;

namespace LibSiteMonitoring.Alarm
{
  public class Mail : IAlarm
  {
    public string AccountId { get; set; }
    public string AccountPassword { get; set; }
    public string FromAddress { get; set; }
    public string ToAddress { get; set; }

    public Mail(string id, string password)
    {
      this.AccountId = id;
      this.AccountPassword = password;
      this.FromAddress = id;
      this.ToAddress = id;
    }

    public Mail(string id, string password, string mailFrom, string mailTo)
    {
      this.AccountId = id;
      this.AccountPassword = password;
      this.FromAddress = mailFrom;
      this.ToAddress = mailTo;
    }

    public void Send(string msgTitle, string msgContent)
    {
      if (ValidateMailAccountInfo() == true)
      {
        Helper.CommonHelper.SendMail(this.AccountId
          , this.AccountPassword
          , this.FromAddress
          , this.ToAddress
          , msgTitle
          , msgContent);
      }
    }

    public void Send(string msgTitle, List<MonitoringItem> lstMonitoringItem)
    {
      string msgContent = new AlarmMsg(lstMonitoringItem).MakeMessage();
      Send(msgTitle, msgContent);
    }

    private bool ValidateMailAccountInfo()
    {
      bool result = false;

      if (this.AccountId.Length > 0
          && this.AccountPassword.Length > 0
          && this.ToAddress.Length > 0
          && this.FromAddress.Length > 0)
      {
        result = true;
      }

      return result;
    }
  }
}
