using LibSiteMonitoring.Helper;
using LibSiteMonitoring.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace LibSiteMonitoring.ParsingModule
{
  /// <summary>
  /// 중고나라
  /// </summary>
  public class Joonggonara : BaseParsingModule, IParsingModule
  {
    Action<string> _FuncLog;
    private void FuncLog(string logMsg)
    {
      if (_FuncLog == null)
      {
        _FuncLog = (x) => { };
      }

      _FuncLog(logMsg);
    }


    private string BoardType { get; set; }
    private string MenuId { get; set; }
    private string ItemListUrl { get; set; }
    private string ItemDetailBaseUrlDesktop { get; set; }
    private string ItemDetailBaseUrlMobile { get; set; }
    private int LimitPageNo { get; set; }

		public dynamic DefaultConfig
    {
      get
      {
        Dictionary<string, string> dicBoardCategory = new Dictionary<string, string>() {
          { "제목", "L" },
          { "앨범", "I" }//금액노출
        };
        string boardType = dicBoardCategory["앨범"];

        Dictionary<string, string> dicMenuCategory = new Dictionary<string, string>() {
          { "전체", "" },
          { "태블릿", "749" },
          { "노트북", "334" }
        };
        string menuId = dicMenuCategory["전체"];

        int limitPageNo = 5;

        return new
        {
          BoardCategory = dicBoardCategory,
          BoardType = boardType,
          MenuCategory = dicMenuCategory,
          MenuId = menuId,
          ItemListUrl = $"https://cafe.naver.com/ArticleList.nhn?search.clubid=10050146&search.boardtype={boardType}&search.questionTab=A&search.totalCount=151&search.menuid={menuId}&search.page=",
          ItemDetailBaseUrlDesktop = "https://cafe.naver.com/joonggonara/{0}",
          ItemDetailBaseUrlMobile = "https://m.cafe.naver.com/ArticleRead.nhn?clubid=10050146&articleid={0}&page=1&boardtype=L",
          LimitPageNo = limitPageNo
        };
      }
    }


		/// <summary>
		/// 
		/// </summary>
		/// <param name="funcLog">로그 기록 메소드</param>
		public Joonggonara(Action<string> funcLog = null) : base(SiteName.중고나라)
    {
      _FuncLog = funcLog;


      dynamic siteConfig = LoadSiteConfig();
      if (siteConfig == null)
      {
        siteConfig = DefaultConfig;
        SaveSiteConfig(siteConfig);
      }

      BoardType = siteConfig.BoardType;
      MenuId = siteConfig.MenuId;
      ItemListUrl = siteConfig.ItemListUrl;
      ItemDetailBaseUrlDesktop = siteConfig.ItemDetailBaseUrlDesktop;
      ItemDetailBaseUrlMobile = siteConfig.ItemDetailBaseUrlMobile;
      LimitPageNo = siteConfig.LimitPageNo;
    }


    /// <summary>
    /// 
    /// </summary>
    /// <param name="item">최근 item, 없으면 null</param>
    /// <returns></returns>
    public async Task<List<MonitoringItem>> GetMonitoringList()
    {
      Func<bool> CanRun = () => {
        bool canRunResult = false;

        if (base.IsBlocked == true)
        {
          FuncLog("can't run - blocked");
        }
        else if (base.LastItem != null && base.LastItem.ItemDate.AddSeconds(base.SleepSecond - 1) > DateTime.Now)
        {
          FuncLog("can't run - not enough sleep");
        }
        else
        {
          canRunResult = true;
        }

        return canRunResult;
      };


      Func<int, Task<HtmlAgilityPack.HtmlDocument>> getData = async (pageNo) => {
        HtmlAgilityPack.HtmlDocument doc = new HtmlAgilityPack.HtmlDocument();

        WebRequest request = null;

        try
        {
          request = WebRequest.Create(ItemListUrl + pageNo.ToString());
          using (WebResponse response = await request.GetResponseAsync())
          {
            doc.Load(response.GetResponseStream());
            response.Close();
          }
        }
        catch (Exception ex)
        {
          CommonHelper.WriteLog(ex.ToString());
          FuncLog($"err : {ex.Message}");
        }

        if (doc.DocumentNode.HasChildNodes == false)
        {
          FuncLog($"download data is empty");
          doc = null;
        }

        return doc;
      };



      base.IsBlocked = false;

      List<MonitoringItem> result = new List<MonitoringItem>();

      if (CanRun() == true)
      {
        base.LastRunDate = DateTime.Now;

        int curPageNo = 1;
        while (curPageNo > 0 && curPageNo < LimitPageNo)
        {
          FuncLog($"downloading page {curPageNo}");

          var domData = await getData(curPageNo);
          List<MonitoringItem> lstItem = ParsingHTMLdocument(domData);

          if (lstItem.Count == 0)
          {
            FuncLog($"blocked");
            curPageNo = 0;
            base.IsBlocked = true;
            break;
          }
          else
          {
            if (base.LastItem == null)
            {
              result.AddRange(lstItem);
              curPageNo = 0;
              break;
            }
            else
            {
              int cntRemove = lstItem.RemoveAll(x => Convert.ToInt64(x.ItemId) <= Convert.ToInt64(base.LastItem.ItemId));
              result.AddRange(lstItem);
              curPageNo++;

              if (cntRemove > 0)
              {
                FuncLog($"stop downloading");
                curPageNo = 0;
                break;
              }
              else
              {
                FuncLog($"max - {lstItem.Max(x => Convert.ToInt64(x.ItemId))}, min - {lstItem.Min(x => Convert.ToInt64(x.ItemId))}");
              }
            }
          }
        }


        if (result.Count > 0)
        {
          string lastItemId = result.Max(x => Convert.ToInt64(x.ItemId)).ToString();
          base.LastItem = result.First(x => x.ItemId == lastItemId);

          FuncLog($"set lastItemId {lastItemId}");
        }
      }

      return result;
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

            lstItem.Add(new MonitoringItem()
            {
              ItemId = articleId,
              ItemTitle = articleTitle,
              ItemUrlPc = string.Format(ItemDetailBaseUrlDesktop, articleId),
              ItemUrlMobile = string.Format(ItemDetailBaseUrlMobile, articleId),
              ItemDate = DateTime.Now
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
              ItemId = articleId,
              ItemTitle = articleTitle,
              ItemUrlPc = string.Format(ItemDetailBaseUrlDesktop, articleId),
              ItemUrlMobile = string.Format(ItemDetailBaseUrlMobile, articleId),
              ItemPrice = tmpVal,
              ItemDate = DateTime.Now
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
