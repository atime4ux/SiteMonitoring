using System.Collections.Generic;
using System.Linq;
using LibSiteMonitoring.Model;
using System;

namespace LibSiteMonitoring
{
    public class Keyword
    {
        /// <summary>
        /// 공백으로 여러 단어 구분
        /// </summary>
        public string RequireKeywords { get; set; }

        /// <summary>
        /// 공백으로 여러 단어 구분
        /// </summary>
        public string OptionKeywords { get; set; }

        /// <summary>
        /// 공백으로 여러 단어 구분
        /// </summary>
        public string ExceptKeywords { get; set; }

        public int MinPrice { get; set; }
        public int MaxPrice { get; set; }

        public Keyword(string require, string option, string except, int min, int max)
        {
            this.RequireKeywords = require;
            this.OptionKeywords = option;
            this.ExceptKeywords = except;
            this.MinPrice = min;
            this.MaxPrice = max;
        }

        private string SearchWord(MonitoringItem item, string[] arrSearchWord, bool isSearchAllWord, string defaultWord = "pass")
        {
            string result = "";

            string itemTitle = item.ItemTitle.Trim().Replace(" ", "").Replace("\r", "").Replace("\n", "").Replace("\t", "").ToUpper();

            if (arrSearchWord != null
                && arrSearchWord.Length > 0
                && arrSearchWord[0].Length > 0)
            {
                if (isSearchAllWord)//모든 단어가 포함되어야 함
                {
                    result = arrSearchWord.All(x => itemTitle.IndexOf(x.ToUpper()) >= 0) ? arrSearchWord[0] : "";
                }
                else
                {
                    result = arrSearchWord.FirstOrDefault(x => itemTitle.IndexOf(x.ToUpper()) >= 0) ?? "";
                }
            }
            else
            {
                result = defaultWord;
            }

            return result;
        }

        private bool SearchPrice(MonitoringItem item, int minPrice, int maxPrice)
        {
            bool result = true;

            if (item.ItemPrice > 0
                && (minPrice > 0 || maxPrice > 0))
            {
                if (item.ItemPrice < minPrice
                    || item.ItemPrice > maxPrice)
                {
                    //금액 확인
                    result = false;
                }
            }

            return result;
        }

        private bool Search(MonitoringItem item)
        {
            string[] arrRequireKeywords = (RequireKeywords ?? "").Split(' ');
            string[] arrOptionKeywords = (OptionKeywords ?? "").Split(' ');

            string searchWordResult = SearchWord(item, arrRequireKeywords, true);
            if (searchWordResult.Length > 0)
            {
                searchWordResult = SearchWord(item, arrOptionKeywords, false);
            }

            bool result = false;
            if (searchWordResult.Length > 0)
            {
                result = SearchPrice(item, MinPrice, MaxPrice);
            }

            return result;
        }

        private string FindExceptWords(MonitoringItem item)
        {
            string[] arrExceptWord = (ExceptKeywords ?? "").Split(' ');

            string result = SearchWord(item, arrExceptWord, false, "");

            return result;
        }

        public static MonitorResult FilterItems(List<MonitoringItem> lstItem, List<Keyword> lstKeyword)
        {
            MonitorResult objFilterResult = new MonitorResult();

            foreach (MonitoringItem item in lstItem)
            {
                foreach (Keyword keyword in lstKeyword)
                {
                    if (keyword.Search(item) == true)
                    {
                        string exceptWord = keyword.FindExceptWords(item);//제외 단어 찾기
                        if (exceptWord == "")
                        {
                            objFilterResult.FilteredItems.Add(item);
                            break;
                        }
                        else
                        {
                            //제외 목록에 추가
                            objFilterResult.ExceptedItems.Add(new ExceptedItem()
                            {
                                Item = item,
                                ExceptWord = exceptWord
                            });
                        }
                    }
                }
            }

            return objFilterResult;
        }
    }
}
