
using libCommon;

namespace SiteMonitoring3.Helper
{
    public class Common
    {
        clsUtil objUtil = null;

        public Common()
        {
            objUtil = new clsUtil();
        }

        public string getAppCfg(string key)
        {
            return objUtil.getAppCfg(key);
        }
    }
}
