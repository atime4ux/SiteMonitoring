using System.Collections.Generic;
using System.Linq;

namespace LibSiteMonitoring.Model
{
    public class Filter
    {
        Helper.Common commonHelper = null;

        public FilterResult objFilterResult { get; set; }

        public Filter()
        {
            commonHelper = new Helper.Common();
            objFilterResult = new FilterResult();
        }

        public FilterResult FilterItems(List<MonitoringItem> lstItem, List<Keyword> lstKeyword)
        {
            foreach (MonitoringItem item in lstItem)
            {
                foreach (Keyword keyword in lstKeyword)
                {
                    bool result = false;

                    string[] arrRequireKeywords = commonHelper.Split(keyword.requireKeywords.ToUpper(), " ");
                    string[] arrOptionKeywords = commonHelper.Split(keyword.optionKeywords.ToUpper(), " ");
                    string[] arrExceptKeywords = commonHelper.Split(keyword.exceptKeywords.ToUpper(), " ");

                    if (keyword.requireKeywords.Length > 0)
                    {
                        if (Search(item, arrRequireKeywords, keyword.minPrice, keyword.maxPrice, true) == true)
                        {
                            if (arrOptionKeywords[0].Length > 0)
                            {
                                if (Search(item, arrOptionKeywords, keyword.minPrice, keyword.maxPrice, false) == true)
                                {
                                    result = true;
                                }
                            }
                            else
                            {
                                result = true;
                            }
                        }
                    }
                    else if (keyword.optionKeywords.Length > 0)
                    {
                        if (Search(item, arrOptionKeywords, keyword.minPrice, keyword.maxPrice, false) == true)
                        {
                            result = true;
                        }
                    }
                    else
                    {
                        result = Search(item, null, keyword.minPrice, keyword.maxPrice, false);
                    }


                    if (result == true)
                    {
                        string exceptWord = FindExceptWords(item, arrExceptKeywords);//제외 단어 찾기
                        if (exceptWord == "")
                        {
                            objFilterResult.lstFiltered.Add(item);
                            break;
                        }
                        else
                        {
                            //제외 목록에 추가
                            objFilterResult.lstExcepted.Add(new ExceptedItem() {
                                item = item,
                                exceptWord = exceptWord
                            });
                            result = false;
                        }
                    }
                }
            }

            return objFilterResult;
        }

        /// <summary>
        /// 검색어가 있으면 해당 검색어, 없으면 "" 리턴
        /// </summary>
        /// <param name="str">검사할 문자열</param>
        /// <param name="arrSearchWord">검색어가 들어간 배열</param>
        private bool Search(MonitoringItem item, string[] arrSearchWord, int minPrice, int maxPrice, bool isSearchAllWord)
        {
            bool result = false;

            string itemTitle = item.itemTitle.Trim().Replace(" ", "");

            if (arrSearchWord != null && arrSearchWord.Length > 0 && arrSearchWord[0].Length > 0)
            {
                if (isSearchAllWord)
                {
                    //모든 단어가 포함되어야 함
                    if (arrSearchWord.Where(x => itemTitle.IndexOf(x) >= 0 || x.Trim().Length == 0).Count() == arrSearchWord.Length)
                    {
                        result = true;
                    }
                }
                else
                {
                    string searchWord = arrSearchWord.Where(x => itemTitle.IndexOf(x) >= 0 || x.Trim().Length == 0).FirstOrDefault();
                    if (string.IsNullOrEmpty(searchWord) == false)
                    {
                        result = true;
                    }
                }
            }
            else
            {
                result = true;
            }


            if (item.itemPrice > 0
                && (minPrice > 0 || maxPrice > 0))
            {
                if (item.itemPrice < minPrice
                    || item.itemPrice > maxPrice)
                {
                    //금액 확인
                    result = false;
                }
            }

            return result;
        }

        private string FindExceptWords(MonitoringItem item, string[] arrExceptWord)
        {
            string result = "";

            string itemTitle = item.itemTitle;
            if (arrExceptWord != null && arrExceptWord.Length > 0 && arrExceptWord[0].Trim().Length > 0)
            {
                string exceptWord = arrExceptWord.Where(x => itemTitle.Contains(x)).FirstOrDefault();
                if (string.IsNullOrEmpty(exceptWord) == false)
                {
                    result = exceptWord;
                }
            }

            return result;
        }
    }

    public class FilterResult
    {
        public List<MonitoringItem> lstFiltered { get; set; }
        public List<ExceptedItem> lstExcepted { get; set; }

        public FilterResult()
        {
            lstFiltered = new List<MonitoringItem>();
            lstExcepted = new List<ExceptedItem>();
        }
    }

    public class ExceptedItem
    {
        public MonitoringItem item { get; set; }
        public string exceptWord { get; set; }
    }
}
