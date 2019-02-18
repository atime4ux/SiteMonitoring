using System;
using System.Linq;
using System.Collections.Generic;

namespace SiteMonitoring3
{
    public class Filter
    {
        libCommon.clsUtil objUtil = new libCommon.clsUtil();

        public FilterResult objFilterResult { get; set; }

        public Filter()
        {
            objFilterResult = new FilterResult();
        }

        public FilterResult FilterItems(List<MonitoringItem> lstItem, List<Keyword> lstKeyword)
        {
            foreach (MonitoringItem item in lstItem)
            {
                foreach (Keyword keyword in lstKeyword)
                {
                    bool result = false;

                    string[] arrRequireKeywords = objUtil.Split(keyword.requireKeywords.ToUpper(), " ");
                    string[] arrOptionKeywords = objUtil.Split(keyword.optionKeywords.ToUpper(), " ");
                    string[] arrExceptKeywords = objUtil.Split(keyword.exceptKeywords.ToUpper(), " ");

                    if (keyword.requireKeywords.Length > 0)
                    {
                        if (FindSearchWords(item, arrRequireKeywords, arrExceptKeywords, true) != "")
                        {
                            if (arrOptionKeywords[0].Length > 0)
                            {
                                if (FindSearchWords(item, arrOptionKeywords, arrExceptKeywords, false) != "")
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
                        if (FindSearchWords(item, arrOptionKeywords, arrExceptKeywords, false) != "")
                        {
                            result = true;
                        }
                    }
                    else
                    {
                        result = true;
                    }

                    if (result == true)
                    {
                        string exceptWord = FindExceptWords(item, arrExceptKeywords);
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
        private string FindSearchWords(MonitoringItem item, string[] arrSearchWord, string[] arrExceptWord, bool isSearchAllWord)
        {
            string result = "";

            string itemTitle = item.itemTitle.Trim().Replace(" ", "");

            if (arrSearchWord != null && arrSearchWord.Length > 0 && arrSearchWord[0].Length > 0)
            {
                if (isSearchAllWord)
                {
                    //모든 단어가 포함되어야 함
                    if (arrSearchWord.Where(x => itemTitle.IndexOf(x) >= 0 || x.Trim().Length == 0).Count() == arrSearchWord.Length)
                    {
                        result = arrSearchWord[0];
                    }
                }
                else
                {
                    string searchWord = arrSearchWord.Where(x => itemTitle.IndexOf(x) >= 0 || x.Trim().Length == 0).FirstOrDefault();
                    if (string.IsNullOrEmpty(searchWord) == false)
                    {
                        result = searchWord;
                    }
                }
            }

            return result;
        }

        private string FindExceptWords(MonitoringItem item, string[] arrExceptWord)
        {
            string result = "";

            string itemTitle = item.itemTitle;
            if (arrExceptWord.Length > 0 && arrExceptWord[0].Trim().Length > 0)
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
