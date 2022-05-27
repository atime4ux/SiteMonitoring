using LibSiteMonitoring.Alarm;
using LibSiteMonitoring.Model;
using LibSiteMonitoring.ParsingModule;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;

namespace LibSiteMonitoring
{
	public class Monitoring
	{
		private bool MainJobTrace = false;//mainJob실행여부
		private List<MonitoringInfo> LstMonitoringInfo = null;


		public const int MinSleepSecond = 10;

		private bool LoopFlag
		{
			get
			{
				bool result = false;

				if (MainJobTrace == false)
				{
					result = true;
				}
				else
				{
					if (GetLoopFlag != null)
					{
						result = GetLoopFlag();
					}
				}

				return result;
			}
		}


		private Func<bool> GetLoopFlag;
		public LogActionGroup LogAction { get; set; }


		private List<IParsingModule> LstParsingModule = null;

		public Monitoring(string json)
		{
			Init(json: json
				, getLoopFlag: null
				, logAction: new LogActionGroup(null, null, null, null, null));
		}

		public Monitoring(string json
			, Func<bool> getLoopFlag
			, LogActionGroup logAction)
		{
			Init(json: json
				, getLoopFlag: getLoopFlag
				, logAction: logAction);
		}

		private void Init(string json
			, Func<bool> getLoopFlag
			, LogActionGroup logAction)
		{
			List<IParsingModule> lstParsingModule = new List<IParsingModule>() {
					new Joonggonara(logAction.WriteStatus)
			};

			Init(json: json
				, getLoopFlag: getLoopFlag
				, logAction: logAction
				, lstParsingModule: lstParsingModule);
		}

		private void Init(string json
			, Func<bool> getLoopFlag
			, LogActionGroup logAction
			, List<IParsingModule> lstParsingModule)
		{
			try
			{
				LstMonitoringInfo = JsonConvert.DeserializeObject<List<MonitoringInfo>>(json);
			}
			catch (Exception ex)
			{ }

			GetLoopFlag = getLoopFlag != null ? getLoopFlag : (() => { return false; }); ;
			LogAction = logAction;
			LstParsingModule = lstParsingModule;
		}

		private bool ValidateMonitoringInfo()
		{
			if (LstMonitoringInfo != null)
			{
				foreach (var monitoringInfo in LstMonitoringInfo)
				{
					foreach (var keyword in monitoringInfo.LstKeyword)
					{
						if (keyword.RequireKeywords.Trim().Length > 0
							|| keyword.OptionKeywords.Trim().Length > 0
							|| keyword.MinPrice > 0
							|| keyword.MaxPrice > 0)
						{
							return true;
						}
					}
				}
			}
			
			return false;
		}

		private async void RunParser()
		{
			if (ValidateMonitoringInfo() == true)
			{
				LogAction.WriteStatus("start");

				foreach (var parsing in LstParsingModule)
				{
					LogAction.WriteStatus("downloading data");

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

					LogAction.WriteStatus($"{parsing.GetParsingTarget()} : success get {lstAll.Count} items");
					LogAction.WriteAllItem(lstAll);

					foreach (var info in LstMonitoringInfo)
					{
						MonitorResult filterResult = info.FilterItems(lstAll);
						LogAction.WriteFilteredItem(filterResult.FilteredItems);
						LogAction.WriteExceptedItem(filterResult.ExceptedItems);

						if (filterResult.FilteredItems.Count > 0)
						{
							LogAction.WriteStatus($"found {filterResult.FilteredItems.Count} items");
							info.SendAlarm(filterResult.FilteredItems);
						}
					}
				}
			}
			else
			{
				LogAction.WriteStatus("invalid monitoring info");
			}


			LogAction.WriteStatus("finish\r\n====================");
		}


		public void RunMonitoring()
		{
			while (LoopFlag == true)
			{
				MainJobTrace = true;

				//스레딩처리할 필요 없음
				//System.Threading.Thread threadSubJob = new System.Threading.Thread(new System.Threading.ThreadStart(SubJob));
				//threadSubJob.Name = "DownloadAndParsing";
				//threadSubJob.Start();
				//threadSubJob.Join();
				RunParser();

				if (LoopFlag == true)
				{
					LogAction.WriteStatus($"start sleep {MinSleepSecond} sec");
					for (int i = 0; i < MinSleepSecond; i++)
					{
						if (LoopFlag == true)
						{
							LogAction.WriteSleepStatus($"sleep remain {(MinSleepSecond - i).ToString()}");
							System.Threading.Thread.Sleep(1000);
						}
						else
						{
							break;
						}
					}
					LogAction.WriteSleepStatus("");
					LogAction.WriteStatus("end sleep");
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
						ItemId = "",
						ItemTitle = "Test_" + DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss"),
						ItemUrlPc = "",
						ItemUrlMobile = ""
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
					LstKeyword = new List<Keyword>() {
						new Keyword("필수 검색어1", "선택 검색어1", "제외 검색어1", 0, 0),
						new Keyword("필수 검색어2", "선택 검색어2", "제외 검색어2", 0, 0)
						},
					LstAlarmInfo = AlarmInfo.GetAllAlarmInfo()
					}
				};

			return sample;
		}
	}
}
