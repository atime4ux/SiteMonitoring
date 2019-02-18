using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;

namespace SiteMonitoring3.Parsing
{
    /// <summary>
    /// 중고나라
    /// </summary>
    public class Joonggonara
    {
        private string listUrl = "https://cafe.naver.com/ArticleList.nhn?search.clubid=10050146&search.boardtype=L&search.questionTab=A&search.totalCount=151&search.page=";
        private string itemBaseUrl = "https://cafe.naver.com/joonggonara?iframe_url=";
        private string itemBaseUrlMobile = "https://m.cafe.naver.com/ArticleRead.nhn?clubid=10050146&articleid={0}&page=1&boardtype=L";

        /// <summary>
        /// 지난번 itemId 여기 도달할때까지 처리한다
        /// </summary>
        private MonitoringItem lastItem { get; set; }


        public bool isBlocked { get; set; }

        public int sleepSecond = 10;
        int limitPageNo = 5;


        Action<string> FuncLog;


        public Joonggonara(Action<string> funcLog)
        {
            FuncLog = funcLog;
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

            int pageNo = 1;
            if (lastItem == null)
            {
                FuncLog($"downloading page {pageNo}");
                result = parsingHTMLdocument(GetData(pageNo));
            }
            else
            {
                FuncLog($"get lastItemId {lastItem.itemId}");

                for (int i = 0; i < limitPageNo; i++)
                {
                    FuncLog($"downloading page {pageNo}");
                    List<MonitoringItem> lstItem = parsingHTMLdocument(GetData(pageNo));

                    if (lstItem.Count > 0)
                    {
                        result.AddRange(lstItem);
                        pageNo++;

                        if (lstItem.Count(x => Convert.ToInt64(x.itemId) <= Convert.ToInt64(lastItem.itemId)) > 0)
                        {
                            result.RemoveAll(x=> Convert.ToInt64(x.itemId) <= Convert.ToInt64(lastItem.itemId));
                            FuncLog($"stop downloading");
                            break;
                        }
                        else
                        {
                            FuncLog($"max - {lstItem.Max(x => Convert.ToInt64(x.itemId))}, min - {lstItem.Min(x => Convert.ToInt64(x.itemId))}");
                        }
                    }
                    else
                    {
                        FuncLog($"blocked");
                        isBlocked = true;
                        break;
                    }
                }
            }         


            if (result.Count > 0)
            {
                string lastItemId = result.Max(x => Convert.ToInt64(x.itemId)).ToString();
                lastItem = result.First(x => x.itemId == lastItemId);

                FuncLog($"set lastItemId {lastItemId}");
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
                libMyUtil.clsFile.writeLog(ex.ToString());
                FuncLog($"err : {ex.Message}");
            }

            if (doc.DocumentNode.HasChildNodes)
            {
                parsingHTMLdocument(doc);
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
        private List<MonitoringItem> parsingHTMLdocument(HtmlAgilityPack.HtmlDocument doc)
        {
            List<MonitoringItem> lstItem = new List<MonitoringItem>();

            if (doc != null)
            {
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
                                itemUrl = articleUrl
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
            }
            else
            {
                FuncLog($"document is null");
            }

            return lstItem;
        }
    }
}
