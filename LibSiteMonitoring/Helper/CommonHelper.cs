﻿using System;
using System.IO;
using System.Reflection;
using System.Linq;

namespace LibSiteMonitoring.Helper
{
	public class CommonHelper
	{
		private static libCommon.clsUtil objUtil = new libCommon.clsUtil();

		public static string GetAppCfg(string key)
		{
			return objUtil.getAppCfg(key);
		}

		public static void OverwriteFile(string fileFullPath, string text)
		{
			try
			{
				File.WriteAllText(fileFullPath, text);
			}
			catch (SystemException ex)
			{
				//엑세스 이상
			}
		}

		public static string ReadFile(string fileFullPath)
		{
			string result = "";

			try
			{
				result = File.ReadAllText(fileFullPath);
			}
			catch (Exception ex)
			{ }

			return result;
		}

		public static void WriteLog(string text)
		{
			libMyUtil.clsFile.writeLog(text);
		}

		public static void SendMail(string accountId, string accountPassword, string fromAddress, string toAddress, string title, string content)
		{
			libMyUtil.clsMail objMail = new libMyUtil.clsMail(accountId, accountPassword);
			objMail.SendMail(fromAddress, toAddress, title, content);
		}

		/// <summary>
		/// enum에 StringValueAttribute 속성 추가 후 이름 사용 가능
		/// </summary>
		/// <param name="value"></param>
		/// <returns></returns>
		public static string GetStringValue(Enum value)
		{
			Type type = value.GetType();
			FieldInfo fieldInfo = type.GetField(value.ToString());

			StringValueAttribute[] attribs = fieldInfo.GetCustomAttributes(typeof(StringValueAttribute), false) as StringValueAttribute[];

			return attribs.Length > 0 ? attribs[0].StringValue : null;
		}

		public static bool HasClass(HtmlAgilityPack.HtmlNode node, params string[] arrClassName)
		{
			return arrClassName.All(x => node.GetAttributeValue("class", "").Split(' ').Contains(x));
		}

		public static HtmlAgilityPack.HtmlNode GetSingleNode(HtmlAgilityPack.HtmlNode node, params string[] arrTagName)
		{
			HtmlAgilityPack.HtmlNode result = node;
			arrTagName.ToList().ForEach(x =>
			{
				result = result.SelectSingleNode(x);
			});
			return result;
		}
	}

	public class StringValueAttribute : Attribute
	{
		public string StringValue { get; protected set; }

		public StringValueAttribute(string value)
		{
			this.StringValue = value;
		}
	}
}
