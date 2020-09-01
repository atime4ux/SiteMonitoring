using System.Collections.Generic;
using System.Linq;
using LibSiteMonitoring.Model;
using System;

namespace LibSiteMonitoring
{
	public class Keyword
	{
		/// <summary>
		/// 공백으로 여러 단어 입력
		/// </summary>
		public string RequireKeywords { get; set; }

		/// <summary>
		/// 공백으로 여러 단어 입력
		/// </summary>
		public string OptionKeywords { get; set; }

		/// <summary>
		/// 공백으로 여러 단어 입력
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

		/// <summary>
		/// 검색어가 있으면 해당 검색어, 없으면 "" 리턴
		/// </summary>
		/// <param name="str">검사할 문자열</param>
		/// <param name="arrSearchWord">검색어가 들어간 배열</param>
		private bool Search(MonitoringItem monitoringItem)
		{
			Func<MonitoringItem, string[], int, int, bool, bool> procSearch = (item, arrSearchWord, minPrice, maxPrice, isSearchAllWord) =>
			{
				bool rs = false;

				string itemTitle = item.ItemTitle.Trim().Replace(" ", "");

				if (arrSearchWord != null
					&& arrSearchWord.Length > 0
					&& arrSearchWord[0].Length > 0)
				{
					if (isSearchAllWord)//모든 단어가 포함되어야 함
					{
						if (arrSearchWord.Count(x => itemTitle.IndexOf(x) >= 0 || x.Trim().Length == 0) == arrSearchWord.Length)
						{
							rs = true;
						}
					}
					else
					{
						string searchWord = arrSearchWord.FirstOrDefault(x => itemTitle.IndexOf(x) >= 0 || x.Trim().Length == 0);
						if (string.IsNullOrEmpty(searchWord) == false)
						{
							rs = true;
						}
					}
				}
				else
				{
					rs = true;
				}

				if (item.ItemPrice > 0
					&& (minPrice > 0 || maxPrice > 0))
				{
					if (item.ItemPrice < minPrice
						|| item.ItemPrice > maxPrice)
					{
						//금액 확인
						rs = false;
					}
				}

				return rs;
			};


			bool result = false;
			string[] arrOptionKeywords = OptionKeywords.ToUpper().Split(' ');
			if (RequireKeywords.Length > 0)
			{
				if (procSearch(monitoringItem, RequireKeywords.ToUpper().Split(' '), MinPrice, MaxPrice, true) == true)
				{
					if (arrOptionKeywords[0].Length > 0)
					{
						if (procSearch(monitoringItem, arrOptionKeywords, MinPrice, MaxPrice, false) == true)
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
			else if (OptionKeywords.Length > 0)
			{
				if (procSearch(monitoringItem, arrOptionKeywords, MinPrice, MaxPrice, false) == true)
				{
					result = true;
				}
			}
			else
			{
				result = procSearch(monitoringItem, null, MinPrice, MaxPrice, false);
			}

			return result;
		}

		private string FindExceptWords(MonitoringItem item)
		{
			string result = "";

			string[] arrExceptWord = ExceptKeywords.ToUpper().Split(' ');
			
			string itemTitle = item.ItemTitle;
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
