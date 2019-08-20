using LibSiteMonitoring.Alarm;
using LibSiteMonitoring.Model;
using LibSiteMonitoring.Parsing;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;

namespace LibSiteMonitoring
{
  public class Monitoring
  {
    private bool mainJobTrace = false;//mainJob실행여부
    private List<MonitoringInfo> lstMonitoringInfo = null;


    public int minSleepSecond
    {
      get
      {
        return 10;
      }
    }
    private bool loopFlag
    {
      get
      {
        bool result = false;

        if (mainJobTrace == false)
        {
          result = true;
        }
        else
        {
          result = GetLoopFlag();
        }

        return result;
      }
    }


    private Func<bool> GetLoopFlag;
    private Action<string> WriteStatus;
    private Action<List<MonitoringItem>> WriteAllItem;
    private Action<List<MonitoringItem>> WriteFilteredItem;
    private Action<List<ExceptedItem>> WriteExceptedItem;
    private Action<string> WriteSleepStatus;


    private List<IParsing> lstParsing = null;

    public Monitoring(string json)
    {
      Init(json: json,
          getLoopFlag: null,
          writeStatus: null,
          writeAllItem: null,
          writeFilteredItem: null,
          writeExceptedItem: null,
          writeSleepStatus: null);
    }

    public Monitoring(string json, Func<bool> getLoopFlag, Action<string> writeStatus, Action<List<MonitoringItem>> writeAllItem, Action<List<MonitoringItem>> writeFilteredItem, Action<List<ExceptedItem>> writeExceptedItem, Action<string> writeSleepStatus)
    {
      Init(json: json,
          getLoopFlag: getLoopFlag,
          writeStatus: writeStatus,
          writeAllItem: writeAllItem,
          writeFilteredItem: writeFilteredItem,
          writeExceptedItem: writeExceptedItem,
          writeSleepStatus: writeSleepStatus);
    }

    private void Init(string json, Func<bool> getLoopFlag, Action<string> writeStatus, Action<List<MonitoringItem>> writeAllItem, Action<List<MonitoringItem>> writeFilteredItem, Action<List<ExceptedItem>> writeExceptedItem, Action<string> writeSleepStatus)
    {
      try
      {
        this.lstMonitoringInfo = JsonConvert.DeserializeObject<List<MonitoringInfo>>(json);
      }
      catch (Exception ex)
      { }

      GetLoopFlag = getLoopFlag != null ? getLoopFlag : (() => { return false; }); ;
      WriteStatus = writeStatus != null ? writeStatus : (x => { });
      WriteAllItem = writeAllItem != null ? writeAllItem : (x => { });
      WriteFilteredItem = writeFilteredItem != null ? writeFilteredItem : (x => { });
      WriteExceptedItem = writeExceptedItem != null ? writeExceptedItem : (x => { });
      WriteSleepStatus = writeSleepStatus != null ? writeSleepStatus : (x => { });


      lstParsing = new List<IParsing>() {
          new Joonggonara(Joonggonara.Category.노트북, WriteStatus)
      };
    }

    private bool ValidateMonitoringInfo()
    {
      bool result = false;

      if (lstMonitoringInfo != null
          && (lstMonitoringInfo.Count(x => x.lstKeyword.Count(y => y.requireKeywords.Trim().Length > 0) > 0) > 0
              || lstMonitoringInfo.Count(x => x.lstKeyword.Count(y => y.optionKeywords.Trim().Length > 0) > 0) > 0
              || lstMonitoringInfo.Count(x => x.lstKeyword.Count(y => y.minPrice > 0 || y.maxPrice > 0) > 0) > 0))
      {
        result = true;
      }

      return result;
    }

    private async void RunParser()
    {
      if (ValidateMonitoringInfo() == true)
      {
        WriteStatus("start");

        foreach (var parsing in lstParsing)
        {
          WriteStatus("downloading data");

          List<MonitoringItem> lstAll = await parsing.GetMonitoringList();

          //능동적 슬립은 보류
          //if (lstAll.Count == 0)
          //{
          //    parsing.SetSleepSecond(parsing.GetSleepSecond() + minSleepSecond);
          //}
          //else
          //{
          //    parsing.SetSleepSecond(Math.Max(minSleepSecond, parsing.GetSleepSecond() / 2));
          //}

          WriteStatus($"{parsing.GetParsingTarget()} : success get {lstAll.Count} items");
          WriteAllItem(lstAll);

          foreach (var info in lstMonitoringInfo)
          {
            FilterResult filterResult = new Filter().FilterItems(lstAll, info.lstKeyword);
            WriteFilteredItem(filterResult.lstFiltered);
            WriteExceptedItem(filterResult.lstExcepted);

            if (filterResult.lstFiltered.Count > 0)
            {
              WriteStatus($"found {filterResult.lstFiltered.Count} items");
              info.SendAlarm(filterResult.lstFiltered);
            }
          }
        }
      }
      else
      {
        WriteStatus("invalid monitoring info");
      }


      WriteStatus("finish\r\n====================");
    }


    public void RunMonitoring()
    {
      while (loopFlag == true)
      {
        mainJobTrace = true;

        //스레딩처리할 필요 없음
        //System.Threading.Thread threadSubJob = new System.Threading.Thread(new System.Threading.ThreadStart(SubJob));
        //threadSubJob.Name = "DownloadAndParsing";
        //threadSubJob.Start();
        //threadSubJob.Join();
        RunParser();

        if (loopFlag == true)
        {
          WriteStatus($"start sleep {minSleepSecond} sec");
          for (int i = 0; i < minSleepSecond; i++)
          {
            if (loopFlag == true)
            {
              WriteSleepStatus($"sleep remain {(minSleepSecond - i).ToString()}");
              System.Threading.Thread.Sleep(1000);
            }
            else
            {
              break;
            }
          }
          WriteSleepStatus("");
          WriteStatus("end sleep");
        }
      }
    }


    public static bool SendTestAlarm(string monitoringInfoJson)
    {
      bool result = true;


      List<MonitoringInfo> lstMonitoringInfo = null;
      try
      {
        lstMonitoringInfo = JsonConvert.DeserializeObject<List<MonitoringInfo>>(monitoringInfoJson);
      }
      catch (Exception ex)
      { }

      if (lstMonitoringInfo != null && lstMonitoringInfo.FirstOrDefault() != null)
      {
        List<MonitoringItem> lstTest = new List<MonitoringItem>() {
                    new MonitoringItem() {
                        itemId = "",
                        itemTitle = "Test_" + DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss"),
                        itemUrl = ""
                    }
                };

        try
        {
          lstMonitoringInfo.FirstOrDefault().SendAlarm(lstTest);
        }
        catch (Exception ex)
        {
          result = false;
        }
      }

      return result;
    }

    public static List<MonitoringInfo> GetSampleMonitoringInfo()
    {
      List<MonitoringInfo> sample = new List<MonitoringInfo>() {
                new MonitoringInfo() {
                    lstKeyword = new List<Keyword>() {
                        new Keyword("필수 검색어1", "선택 검색어1", "제외 검색어1", 0, 0),
                        new Keyword("필수 검색어2", "선택 검색어2", "제외 검색어2", 0, 0)
                    },
                    lstAlarmInfo = new List<AlarmInfo>() {
                        new AlarmInfo(AlarmType.email, "id;password;from;to"),
                        new AlarmInfo(AlarmType.jandi, "jandiWebhookUrl"),
                        new AlarmInfo(AlarmType.slack, "slackWebhookUrl")
                    }
                }
            };

      return sample;
    }
  }
}
