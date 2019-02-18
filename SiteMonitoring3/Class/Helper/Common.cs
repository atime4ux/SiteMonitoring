using System;
using System.IO;

namespace SiteMonitoring3.Helper
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
    }
}
