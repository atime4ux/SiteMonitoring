using System;
using System.Collections.Generic;
using System.Text;
using System.Net;
using System.Linq;
using libMyUtil;

namespace SiteMonitoring3.Parsing
{
    /// <summary>
    /// 중고나라
    /// </summary>
    public class Joonggonara
    {
        private string listUrl = "https://cafe.naver.com/ArticleList.nhn?search.clubid=10050146&search.boardtype=L&search.questionTab=A&search.totalCount=151&search.page=";
        private string itemBaseUrl = "https://cafe.naver.com/joonggonara?iframe_url=";

        /// <summary>
        /// 지난번 itemId 여기 도달할때까지 처리한다
        /// </summary>
        private MonitoringItem lastItem { get; set; }

        public bool isBlocked { get; set; }

        System.Windows.Forms.TextBox txtStatus = null;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="item">최근 item, 없으면 null</param>
        /// <returns></returns>
        public List<MonitoringItem> GetMonitoringList(System.Windows.Forms.TextBox statusControl)
        {
            txtStatus = statusControl;

            isBlocked = false;

            List<MonitoringItem> result = new List<MonitoringItem>();

            int pageNo = 1;
            if (lastItem == null)
            {
                clsThread.SetTextBox(txtStatus, $"downloading page {pageNo}\r\n", true);
                result = parsingHTMLdocument(GetData(pageNo));
            }
            else
            {
                clsThread.SetTextBox(txtStatus, $"get lastItemId {lastItem.itemId}\r\n", true);

                int limitPageNo = 20;

                for (int i = 0; i < limitPageNo; i++)
                {
                    clsThread.SetTextBox(txtStatus, $"downloading page {pageNo}\r\n", true);
                    List<MonitoringItem> lstItem = parsingHTMLdocument(GetData(pageNo));

                    if (lstItem.Count > 0)
                    {
                        result.AddRange(lstItem);
                        pageNo++;

                        if (lstItem.Count(x => Convert.ToInt64(x.itemId) <= Convert.ToInt64(lastItem.itemId)) > 0)
                        {
                            clsThread.SetTextBox(txtStatus, $"stop downloading\r\n", true);
                            break;
                        }
                        else
                        {
                            clsThread.SetTextBox(txtStatus, $"max - {lstItem.Max(x=>Convert.ToInt64(x.itemId))}, min - {lstItem.Min(x => Convert.ToInt64(x.itemId))}\r\n", true);
                        }
                    }
                    else
                    {
                        clsThread.SetTextBox(txtStatus, $"blocked\r\n", true);
                        isBlocked = true;
                        break;
                    }
                }
            }         


            if (result.Count > 0)
            {
                string lastItemId = result.Max(x => Convert.ToInt64(x.itemId)).ToString();
                lastItem = result.First(x => x.itemId == lastItemId);

                clsThread.SetTextBox(txtStatus, $"set lastItemId {lastItemId}\r\n", true);
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
                clsThread.SetTextBox(txtStatus, $"err : {ex.Message}\r\n", true);
            }

            if (doc.DocumentNode.HasChildNodes)
            {
                parsingHTMLdocument(doc);
            }
            else
            {
                clsThread.SetTextBox(txtStatus, $"download data is empty\r\n", true);
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
                                }
                            }

                            articleUrl = itemBaseUrl + articleUrl.Replace("&", "%26");

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
                        clsThread.SetTextBox(txtStatus, $"could not find article area\r\n", true);
                    }
                }
                else
                {
                    clsThread.SetTextBox(txtStatus, $"could not find main-area\r\n", true);
                }
            }
            else
            {
                clsThread.SetTextBox(txtStatus, $"document is null\r\n", true);
            }

            return lstItem;
        }
    }
}
