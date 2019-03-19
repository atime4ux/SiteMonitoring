using LibSiteMonitoring.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;

namespace LibSiteMonitoring.Parsing
{
    /// <summary>
    /// 중고나라
    /// </summary>
    public class Joonggonara : IParsing
    {
        /// <summary>
        /// search.boardtype
        /// L - 제목
        /// I - 앨범
        /// </summary>
        private string BoardType
        {
            get
            {
                return "I";
            }
        }

        /// <summary>
        /// search.menuid
        /// 749 - 태블릿
        /// </summary>
        private string MenuId
        {
            get
            {
                return "749";
            }
        }

        private string listUrl
        {
            get
            {
                return $"https://cafe.naver.com/ArticleList.nhn?search.clubid=10050146&search.boardtype={BoardType}&search.questionTab=A&search.totalCount=151&search.menuid={MenuId}" + "&search.page=";
            }
        }

        private string itemBaseUrl = "https://cafe.naver.com/joonggonara?iframe_url=";
        private string itemBaseUrlMobile = "https://m.cafe.naver.com/ArticleRead.nhn?clubid=10050146&articleid={0}&page=1&boardtype=L";

        /// <summary>
        /// 지난번 itemId 여기 도달할때까지 처리한다
        /// </summary>
        private MonitoringItem lastItem { get; set; }


        /// <summary>
        /// 지난번 실행된 시간
        /// </summary>
        private DateTime lastRunDate = DateTime.Now;


        private bool isBlocked { get; set; }

        private int sleepSecond = 10;
        private int limitPageNo = 5;


        Action<string> FuncLog;


        public Joonggonara(Action<string> funcLog)
        {
            FuncLog = funcLog;
        }


        public ParsingTarget GetParsingType()
        {
            return ParsingTarget.중고나라;
        }


        public void SetSleepSecond(int sec)
        {
            sleepSecond = sec;
        }


        public int GetSleepSecond()
        {
            return sleepSecond;
        }


        private bool CanRun()
        {
            bool result = false;

            if (isBlocked == true)
            {
                FuncLog("can't run - blocked");
            }
            else if (lastItem != null && lastItem.itemDate.AddSeconds(sleepSecond + 2) > DateTime.Now)
            {
                FuncLog("can't run - not enough sleep");
            }
            else
            {
                result = true;
            }

            return result;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="item">최근 item, 없으면 null</param>
        /// <returns></returns>
        public List<MonitoringItem> GetMonitoringList()
        {
            isBlocked = false;

            List<MonitoringItem> result = new List<MonitoringItem>();

            if (CanRun() == true)
            {
                lastRunDate = DateTime.Now;

                int pageNo = 1;
                while (pageNo > 0 && pageNo < limitPageNo)
                {
                    FuncLog($"downloading page {pageNo}");
                    List<MonitoringItem> lstItem = ParsingHTMLdocument(GetData(pageNo));

                    if (lstItem.Count == 0)
                    {
                        FuncLog($"blocked");
                        pageNo = 0;
                        isBlocked = true;
                        break;
                    }
                    else
                    {
                        if (lastItem == null)
                        {
                            result.AddRange(lstItem);
                            pageNo = 0;
                            break;
                        }
                        else
                        {
                            int cntRemove = lstItem.RemoveAll(x => Convert.ToInt64(x.itemId) <= Convert.ToInt64(lastItem.itemId));
                            result.AddRange(lstItem);
                            pageNo++;

                            if (cntRemove > 0)
                            {
                                FuncLog($"stop downloading");
                                pageNo = 0;
                                break;
                            }
                            else
                            {
                                FuncLog($"max - {lstItem.Max(x => Convert.ToInt64(x.itemId))}, min - {lstItem.Min(x => Convert.ToInt64(x.itemId))}");
                            }
                        }
                    }
                }


                if (result.Count > 0)
                {
                    string lastItemId = result.Max(x => Convert.ToInt64(x.itemId)).ToString();
                    lastItem = result.First(x => x.itemId == lastItemId);

                    FuncLog($"set lastItemId {lastItemId}");
                }
            }
            
            return result;
        }

        /// <summary>
        /// html dom 반환
        /// </summary>
        /// <param name="pageNo"></param>
        private HtmlAgilityPack.HtmlDocument GetData(int pageNo)
        {
            HtmlAgilityPack.HtmlDocument doc = new HtmlAgilityPack.HtmlDocument();

            WebRequest request = null;

            try
            {
                request = WebRequest.Create(listUrl + pageNo.ToString());
                using (WebResponse response = request.GetResponse())
                {
                    doc.Load(response.GetResponseStream());
                    response.Close();
                }
            }
            catch (Exception ex)
            {
                new Helper.Common().WriteLog(ex.ToString());
                FuncLog($"err : {ex.Message}");
            }

            if (doc.DocumentNode.HasChildNodes)
            {
                ParsingHTMLdocument(doc);
            }
            else
            {
                FuncLog($"download data is empty");
                doc = null;
            }

            return doc;
        }


        /// <summary>
        /// html dom을MonitoringItem 목록으로 반환
        /// </summary>
        /// <param name="doc"></param>
        /// <returns></returns>
        private List<MonitoringItem> ParsingHTMLdocument(HtmlAgilityPack.HtmlDocument doc)
        {
            List<MonitoringItem> lstItem = new List<MonitoringItem>();

            if (doc != null)
            {
                if (BoardType == "I")
                {
                    lstItem = ParsingAlbumType(doc);
                }
                else
                {
                    lstItem = ParsingTitleType(doc);
                }
            }
            else
            {
                FuncLog($"document is null");
            }

            return lstItem;
        }

        private List<MonitoringItem> ParsingTitleType(HtmlAgilityPack.HtmlDocument doc)
        {
            List<MonitoringItem> lstItem = new List<MonitoringItem>();

            HtmlAgilityPack.HtmlNode obj = doc.GetElementbyId("main-area");
            if (obj != null)
            {
                var lstArticle = obj.SelectNodes("div").Where(x => x.GetAttributeValue("class", "").Contains("article-board"));
                lstArticle = lstArticle.Where(x => x.GetAttributeValue("class", "").Contains("m-tcol-c"));
                lstArticle = lstArticle.Where(x => x.Id != "upperArticleList");

                if (lstArticle != null && lstArticle.Count() > 0)
                {
                    foreach (var trArticle in lstArticle.FirstOrDefault().SelectSingleNode("table").SelectSingleNode("tbody").SelectNodes("tr"))
                    {
                        var tdArticle = trArticle.SelectNodes("td").Where(x => x.GetAttributeValue("class", "").Contains("td_article")).FirstOrDefault();
                        tdArticle = tdArticle.SelectNodes("div").Where(x => x.GetAttributeValue("class", "").Contains("board-list")).FirstOrDefault();
                        tdArticle = tdArticle.SelectNodes("div").Where(x => x.GetAttributeValue("class", "").Contains("inner_list")).FirstOrDefault();

                        string articleUrl = tdArticle.SelectSingleNode("a").GetAttributeValue("href", "");
                        string articleId = "";
                        string articleTitle = tdArticle.SelectSingleNode("a").InnerText.Trim();

                        foreach (var param in articleUrl.Split('&'))
                        {
                            string[] arrParam = param.Split('=');
                            string paramKey = arrParam[0];

                            if (paramKey == "articleid" && arrParam.Length == 2)
                            {
                                articleId = arrParam[1];
                                break;
                            }
                        }

                        //articleUrl = itemBaseUrl + articleUrl.Replace("&", "%26");
                        articleUrl = string.Format(itemBaseUrlMobile, articleId);

                        lstItem.Add(new MonitoringItem()
                        {
                            itemId = articleId,
                            itemTitle = articleTitle,
                            itemUrl = articleUrl,
                            itemDate = DateTime.Now
                        });
                    }
                }
                else
                {
                    FuncLog($"could not find article area");
                }
            }
            else
            {
                FuncLog($"could not find main-area");
            }

            return lstItem;
        }

        private List<MonitoringItem> ParsingAlbumType(HtmlAgilityPack.HtmlDocument doc)
        {
            List<MonitoringItem> lstItem = new List<MonitoringItem>();

            HtmlAgilityPack.HtmlNode obj = doc.GetElementbyId("main-area");
            if (obj != null)
            {
                var lstArticle = obj.SelectNodes("ul").Where(x => x.GetAttributeValue("class", "").Contains("article-album-sub"));
                
                if (lstArticle != null && lstArticle.Count() > 0)
                {
                    foreach (var liArticle in lstArticle.FirstOrDefault().SelectNodes("li"))
                    {
                        string articleUrl = liArticle.SelectSingleNode("a").GetAttributeValue("href", "").Replace("&amp;", "&");
                        string articleId = "";

                        foreach (var param in articleUrl.Split('&'))
                        {
                            string[] arrParam = param.Split('=');
                            string paramKey = arrParam[0];

                            if (paramKey == "articleid" && arrParam.Length == 2)
                            {
                                articleId = arrParam[1];
                                break;
                            }
                        }

                        //articleUrl = itemBaseUrl + articleUrl.Replace("&", "%26");
                        articleUrl = string.Format(itemBaseUrlMobile, articleId);

                        string articleTitle = liArticle.SelectSingleNode("dl").SelectSingleNode("dt").InnerText.Trim();

                        string articlePrice = liArticle.SelectSingleNode("dl").SelectNodes("dd").Where(x => x.GetAttributeValue("class", "").Contains("price")).FirstOrDefault().InnerText;
                        articlePrice = articlePrice.Replace("원", "").Replace(",", "").Trim();


                        int tmpVal = 0;
                        if (Int32.TryParse(articlePrice, out tmpVal) == true)
                        {
                            articlePrice = string.Format("{0:n0}", tmpVal);
                        }

                        lstItem.Add(new MonitoringItem()
                        {
                            itemId = articleId,
                            itemTitle = $"{articleTitle}[{articlePrice}원]",
                            itemUrl = articleUrl,
                            itemPrice = tmpVal,
                            itemDate = DateTime.Now
                        });
                    }
                }
                else
                {
                    FuncLog($"could not find article area");
                }
            }
            else
            {
                FuncLog($"could not find main-area");
            }

            return lstItem;
        }
    }
}
