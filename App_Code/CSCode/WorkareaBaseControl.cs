using System;
using System.Collections.Generic;
using System.Web;
using Ektron.Cms;
using System.Web.UI;
using Ektron.Cms.Common;
using System.ComponentModel;

/// <summary>
/// Summary description for WorkareaBaseControl
/// </summary>
public abstract class WorkareaBaseControl : UserControl
{
	private CommonApi _commonAPIRef = new CommonApi();
	private EkMessageHelper _messageHelperRef;
	private ContentAPI _contentApi;

	public WorkareaBaseControl()
	{
		_messageHelperRef = _commonAPIRef.EkMsgRef;
	}

	public string GetMessage(string key)
	{
		return _messageHelperRef.GetMessage(key);
	}

	public CommonApi CommonApi
	{ get { return _commonAPIRef; } }

	public bool IsLoggedIn
	{ get { return ContentApi.IsLoggedIn; } }

	public bool IsAdmin
	{ get { return IsLoggedIn && ContentApi.IsAdmin(); } }

	public ContentAPI ContentApi
	{ get { return ((null == _contentApi) ? _contentApi = new ContentAPI() : _contentApi); } }

	protected virtual string GetEnumDescription(Enum enumValue)
	{
		System.Reflection.FieldInfo enumInfo = enumValue.GetType().GetField(enumValue.ToString());
		DescriptionAttribute[] enumAttributes = (DescriptionAttribute[])enumInfo.GetCustomAttributes(typeof(DescriptionAttribute), false);
		if (enumAttributes != null && enumAttributes.Length > 0)
		{
			string displayText = enumAttributes[0].Description;
			if (String.IsNullOrEmpty(displayText))
			{
				return enumValue.ToString();
			}
			else if (displayText.StartsWith("{") && displayText.EndsWith("}"))
			{
				return GetMessage(displayText.Trim(new char[] { '{', '}' }));
			}
			else
			{
				return displayText;
			}
		}
		return enumValue.ToString();
	}

	protected virtual string GoogleChartBaseUrl
	{
		get
		{
			if (_commonAPIRef.UseSsl)
			{
				return _commonAPIRef.AppPath + "controls/reports/GoogleChart.ashx";
			}
			else
			{
				return "http://chart.apis.google.com/chart";
			}
		}
	}
}

public static class EnumExtensions
{
	public static string ToDescriptionString(this Enum enumValue)
	{
		System.Reflection.FieldInfo enumInfo = enumValue.GetType().GetField(enumValue.ToString());
		DescriptionAttribute[] enumAttributes = (DescriptionAttribute[])enumInfo.GetCustomAttributes(typeof(DescriptionAttribute), false);
		if (enumAttributes != null && enumAttributes.Length > 0)
		{
			string displayText = enumAttributes[0].Description;
			if (String.IsNullOrEmpty(displayText))
			{
				return enumValue.ToString();
			}
			else
			{
				return displayText;
			}
		}
		return enumValue.ToString();
	}
}
