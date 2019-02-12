using System.Linq;

namespace SiteMonitoring3
{
    public class Filter
    {
        libCommon.clsUtil objUtil = new libCommon.clsUtil();

        public bool isExcepted = false;

        public bool FilterMonitoringItem(MonitoringItem item, Keyword keyword)
        {
            bool result = false;

            string[] arrSrchWord01 = objUtil.Split(keyword.requireKeywords.ToUpper(), " ");
            string[] arrSrchWord02 = objUtil.Split(keyword.optionKeywords.ToUpper(), " ");
            string[] arrExceptWord = objUtil.Split(keyword.exceptKeywords.ToUpper(), " ");

            if (keyword.requireKeywords.Length > 0)
            {
                if (IsContainSearchWords(item, arrSrchWord01, arrExceptWord, true))
                {
                    if (arrSrchWord02[0].Length > 0)
                    {
                        result = IsContainSearchWords(item, arrSrchWord02, arrExceptWord, false);
                    }
                    else
                    {
                        result = true;
                    }
                }
            }
            else if (keyword.optionKeywords.Length > 0)
            {
                result = IsContainSearchWords(item, arrSrchWord02, arrExceptWord, false);
            }
            else
            {
                result = true;
            }

            return result;
        }

        /// <summary>
        /// 검색어가 있으면 해당 검색어, 없으면 "" 리턴
        /// </summary>
        /// <param name="str">검사할 문자열</param>
        /// <param name="arrSearchWord">검색어가 들어간 배열</param>
        private bool IsContainSearchWords(MonitoringItem item, string[] arrSearchWord, string[] arrExceptWord, bool isSearchAllWord)
        {
            bool result = false;

            string itemTitle = item.itemTitle.Trim().Replace(" ", "");

            if (arrSearchWord != null && arrSearchWord.Length > 0 && arrSearchWord[0].Length > 0)
            {
                if (isSearchAllWord)
                {
                    if (arrSearchWord.Where(x => itemTitle.IndexOf(x) >= 0 || x.Trim().Length == 0).Count() == arrSearchWord.Length)
                    {
                        if (IsContainExceptWords(itemTitle, arrExceptWord) == true)
                        {
                            isExcepted = true;
                        }
                        else
                        {
                            result = true;
                        }
                    }
                }
                else
                {
                    var objSearch = arrSearchWord.Where(x => itemTitle.IndexOf(x) >= 0 || x.Trim().Length == 0).FirstOrDefault();
                    if (objSearch != null)
                    {
                        if (IsContainExceptWords(itemTitle, arrExceptWord) == true)
                        {
                            isExcepted = true;
                        }
                        else
                        {
                            result = true;
                        }
                    }
                }
            }

            return result;
        }

        private bool IsContainExceptWords(string itemTitle, string[] arrExceptWord)
        {
            bool result = false;

            if (arrExceptWord.Length > 0 && arrExceptWord[0].Trim().Length > 0 && arrExceptWord.Count(x => itemTitle.Contains(x)) > 0)
            {
                result = true;
            }

            return result;
        }
    }
}
