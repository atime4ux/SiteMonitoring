using LibSiteMonitoring.Model;
using System;

namespace LibSiteMonitoring.ParsingModule
{
	public class BaseParsingModule
	{
		/// <summary>
		/// 지난번 itemId 여기 도달할때까지 처리한다
		/// </summary>
		protected MonitoringItem LastItem { get; set; }

		/// <summary>
		/// 지난번 실행된 시간
		/// </summary>
		protected DateTime LastRunDate { get; set; }

		protected SiteName ParsingTarget { get; set; }
		protected int SleepSecond { get; set; }

		protected bool IsBlocked { get; set; }

		protected string ConfigFileName
		{
			get
			{
				return $"{ParsingTarget}.config";
			}
		}

		protected T LoadSiteConfig<T>()
		{
			dynamic result = null;
			string strJson = Helper.CommonHelper.ReadFile(ConfigFileName);
			if (string.IsNullOrEmpty(strJson) == false)
			{ 
				result = Newtonsoft.Json.JsonConvert.DeserializeObject<T>(strJson);
			}

			return result;
		}

		protected void SaveSiteConfig(object objInfo)
		{
			if (objInfo != null)
			{
				string strJson = Newtonsoft.Json.JsonConvert.SerializeObject(objInfo);
				Helper.CommonHelper.OverwriteFile(ConfigFileName, strJson);
			}
		}

		public BaseParsingModule(SiteName siteName)
		{
			ParsingTarget = siteName;
			SleepSecond = 10;
			LastRunDate = DateTime.Now;
		}

		public SiteName GetParsingTarget()
		{
			return ParsingTarget;
		}

		public void SetSleepSecond(int sec)
		{
			SleepSecond = sec;
		}

		public int GetSleepSecond()
		{
			return SleepSecond;
		}
	}
}
