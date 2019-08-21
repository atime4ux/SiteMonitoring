using LibSiteMonitoring.Helper;
using LibSiteMonitoring.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace LibSiteMonitoring.Parsing
{
  /// <summary>
  /// 중고나라
  /// </summary>
  public class Joonggonara : BaseParsing, IParsing
  {
    public enum Category
    {
      [StringValue("")]
      전체,
      [StringValue("749")]
      태블릿,
      [StringValue("334")]
      노트북
    }

    /// <summary>
    /// search.boardtype
    /// L - 제목
    /// I - 앨범(금액 노출)
    /// </summary>
    private string BoardType
    {
      get
      {
        return "I";
      }
    }

    private string MenuId
    {
      get
      {
        return new Common().GetStringValue(category);
      }
    }

    private string listUrl
    {
      get
      {
        return $"https://cafe.naver.com/ArticleList.nhn?search.clubid=10050146&search.boardtype={BoardType}&search.questionTab=A&search.totalCount=151&search.menuid={MenuId}" + "&search.page=";
      }
    }

    private string itemBaseUrl
    {
      get
      {
        return "https://cafe.naver.com/joonggonara?iframe_url=";
      }
    }

    private string itemBaseUrlMobile
    {
      get
      {
        return "https://m.cafe.naver.com/ArticleRead.nhn?clubid=10050146&articleid={0}&page=1&boardtype=L";
      }
    }

    private int limitPageNo
    {
      get
      {
        return 5;
      }
    }


    Category category;
    Action<string> FuncLog;


    public Joonggonara(Category cate, Action<string> funcLog) : base()
    {
      base.parsingTarget = ParsingTarget.중고나라;

      this.category = cate;

      FuncLog = funcLog;
    }


    private bool CanRun()
    {
      bool result = false;

      if (base.isBlocked == true)
      {
        FuncLog("can't run - blocked");
      }
      else if (base.lastItem != null && base.lastItem.itemDate.AddSeconds(base.sleepSecond - 1) > DateTime.Now)
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
    public async Task<List<MonitoringItem>> GetMonitoringList()
    {
      base.isBlocked = false;

      List<MonitoringItem> result = new List<MonitoringItem>();

      if (CanRun() == true)
      {
        base.lastRunDate = DateTime.Now;

        int pageNo = 1;
        while (pageNo > 0 && pageNo < limitPageNo)
        {
          FuncLog($"downloading page {pageNo}");

          var domData = await GetData(pageNo);
          List<MonitoringItem> lstItem = ParsingHTMLdocument(domData);

          if (lstItem.Count == 0)
          {
            FuncLog($"blocked");
            pageNo = 0;
            base.isBlocked = true;
            break;
          }
          else
          {
            if (base.lastItem == null)
            {
              result.AddRange(lstItem);
              pageNo = 0;
              break;
            }
            else
            {
              int cntRemove = lstItem.RemoveAll(x => Convert.ToInt64(x.itemId) <= Convert.ToInt64(base.lastItem.itemId));
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
          base.lastItem = result.First(x => x.itemId == lastItemId);

          FuncLog($"set lastItemId {lastItemId}");
        }
      }

      return result;
    }

    /// <summary>
    /// html dom 반환
    /// </summary>
    /// <param name="pageNo"></param>
    private async Task<HtmlAgilityPack.HtmlDocument> GetData(int pageNo)
    {
      HtmlAgilityPack.HtmlDocument doc = new HtmlAgilityPack.HtmlDocument();

      WebRequest request = null;

      try
      {
        request = WebRequest.Create(listUrl + pageNo.ToString());
        using (WebResponse response = await request.GetResponseAsync())
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

            string articlePrice = "0";
            var objPrice = liArticle.SelectSingleNode("dl").SelectNodes("dd").Where(x => x.GetAttributeValue("class", "").Contains("price")).FirstOrDefault();
            if (objPrice != null)
            {
              articlePrice = objPrice.InnerText.Replace("원", "").Replace(",", "").Replace(" ", "").Trim();
            }

            int tmpVal = 0;
            Int32.TryParse(articlePrice, out tmpVal);


            lstItem.Add(new MonitoringItem()
            {
              itemId = articleId,
              itemTitle = articleTitle,
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
