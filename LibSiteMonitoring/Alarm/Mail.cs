namespace LibSiteMonitoring.Alarm
{
    public class Mail : IAlarm
    {
        Helper.Common commonHelper = new Helper.Common();

        public string accountId { get; set; }
        public string accountPassword { get; set; }
        public string fromAddress { get; set; }
        public string toAddress { get; set; }

        public Mail(string id, string password)
        {
            this.accountId = id;
            this.accountPassword = password;
            this.fromAddress = id;
            this.toAddress = id;
        }

        public Mail(string id, string password, string mailFrom, string mailTo)
        {
            this.accountId = id;
            this.accountPassword = password;
            this.fromAddress = mailFrom;
            this.toAddress = mailTo;
        }
        
        public void Send(string msgTitle, string msgContent)
        {
            if (ValidateMailAccountInfo() == true)
            {
                commonHelper.SendMail(this.accountId, this.accountPassword, this.fromAddress, this.toAddress, msgTitle, msgContent);
            }
        }

        private bool ValidateMailAccountInfo()
        {
            bool result = false;

            if (this.accountId.Length > 0
                && this.accountPassword.Length > 0
                && this.toAddress.Length > 0
                && this.fromAddress.Length > 0)
            {
                result = true;
            }

            return result;
        }
    }
}
