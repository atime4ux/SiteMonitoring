using System.Collections.Generic;

namespace SiteMonitoring3.Alarm
{
    public class Mail
    {
        private string accountId { get; set; }
        private string accountPassword { get; set; }
        private string fromAddress { get; set; }
        private string toAddress { get; set; }

        public Mail(string id, string password, string mailFrom, string mailTo)
        {
            this.accountId = id;
            this.accountPassword = password;
            this.fromAddress = mailFrom;
            this.toAddress = mailTo;
        }

        public void Send(List<MonitoringItem> lst)
        {
            AlarmMsg objMsg = new AlarmMsg(lst);

            libMyUtil.clsMail objMail = new libMyUtil.clsMail(this.accountId, this.accountPassword);
            objMail.SendMail(this.fromAddress, this.toAddress, objMsg.Title, objMsg.Message);
        }
    }
}
