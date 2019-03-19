using System;
using System.IO;

namespace LibSiteMonitoring.Helper
{
    public class Common
    {
        libCommon.clsUtil objUtil = null;

        public Common()
        {
            objUtil = new libCommon.clsUtil();
        }

        public string GetAppCfg(string key)
        {
            return objUtil.getAppCfg(key);
        }

        public void OverwriteFile(string fileFullPath, string text)
        {
            try
            {
                File.WriteAllText(fileFullPath, text);
            }
            catch (SystemException ex)
            {
                //엑세스 이상
            }
        }

        public string ReadFile(string fileFullPath)
        {
            string result = "";

            try
            {
                result = File.ReadAllText(fileFullPath);
            }
            catch (Exception ex)
            { }

            return result;
        }

        public void WriteLog(string text)
        {
            libMyUtil.clsFile.writeLog(text);
        }

        public string[] Split(string text, string delimeter)
        {
            return objUtil.Split(text, delimeter);
        }

        public void SendMail(string accountId, string accountPassword, string fromAddress, string toAddress, string title, string content)
        {
            libMyUtil.clsMail objMail = new libMyUtil.clsMail(accountId, accountPassword);
            objMail.SendMail(fromAddress, toAddress, title, content);
        }
    }
}
