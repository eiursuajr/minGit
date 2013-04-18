using System.Web.UI.WebControls;
using System.Configuration;
using System.Collections;
using System.Linq;
using System.Data;
using System.Web.Caching;
using System.Xml.Linq;
using System.Web.UI;
using System.Diagnostics;
using System.Web.Security;
using System;
using System.Text;
using Microsoft.VisualBasic;
using System.Web.UI.HtmlControls;
using System.Web.SessionState;
using System.Text.RegularExpressions;
using System.Web.Profile;
using System.Collections.Generic;
using System.Web.UI.WebControls.WebParts;
using System.Collections.Specialized;
using System.Web;
using Ektron.Cms;
using Ektron.Cms.Common;
//using Ektron.Cms.Common.EkConstants;
	public partial class reportstoolbar : System.Web.UI.UserControl
	{
		protected ContentAPI m_refContentApi = new ContentAPI();
		protected string g_ContentTypeSelected = Ektron.Cms.Common.EkConstants.CMSContentType_AllTypes.ToString();
		public const int CMSContentType_Content = 1;
		public const int CMSContentType_Forms = 2;
		public const int CMSContentType_Library = 7;
		public const int CMSContentType_NonImageLibrary = 9;
		public const int CMSContentType_PDF = 10;
		private EkMessageHelper m_refMsg;
		private StyleHelper m_refStyle = new StyleHelper();
		private string m_strPageAction = "";
		private string m_strTitleBarMsg = "";
		private string m_strAppImgPath = "";
		private string m_strFilterType = "";
		private object m_data = null;
		protected int EnableMultilingual = 0;
		protected int ContentLanguage = -1;
		protected int lContentType = 0;
		protected AssetInfoData[] asset_data;
		private bool m_EnableEmail = true;
		private bool m_EnableFolders = true;
		private bool m_EnableContentTypes = true;
		private bool m_EnableDefaultTitlePrefix = true;
		private string m_strTitlePrefix = "";
		private bool m_HasData = true;
		
		public string PageAction
		{
			get
			{
				return (m_strPageAction);
			}
			set
			{
				m_strPageAction = value;
			}
		}
		public bool HasData
		{
			get
			{
				return (m_HasData);
			}
			set
			{
				m_HasData = value;
			}
		}
		public string TitleBarMsg
		{
			get
			{
				return (m_strTitleBarMsg);
			}
			set
			{
				m_strTitleBarMsg = value;
			}
		}
		public string AppImgPath
		{
			get
			{
				return (m_strAppImgPath);
			}
			set
			{
				m_strAppImgPath = value;
			}
		}
		public string FilterType
		{
			get
			{
				return (m_strFilterType);
			}
			set
			{
				m_strFilterType = value;
			}
		}
		public object Data
		{
			get
			{
				return (m_data);
			}
			set
			{
				m_data = value;
			}
		}
		public int MultilingualEnabled
		{
			get
			{
				return EnableMultilingual;
			}
			set
			{
				EnableMultilingual = value;
			}
		}
		public int ContentLang
		{
			get
			{
				return ContentLanguage;
			}
			set
			{
				ContentLanguage = value;
			}
		}
		public bool EnableEmail
		{
			get
			{
				if (string.IsNullOrEmpty(m_refContentApi.RequestInformationRef.SystemEmail))
				{
					m_EnableEmail = false;
				}
				return (m_EnableEmail);
			}
			set
			{
				m_EnableEmail = value;
			}
		}
		public bool EnableFolders
		{
			get
			{
				return (m_EnableFolders);
			}
			set
			{
				m_EnableFolders = value;
			}
		}
		public bool EnableContentTypes
		{
			get
			{
				return (m_EnableContentTypes);
			}
			set
			{
				m_EnableContentTypes = value;
			}
		}
		public bool EnableDefaultTitlePrefix
		{
			get
			{
				return (m_EnableDefaultTitlePrefix);
			}
			set
			{
				m_EnableDefaultTitlePrefix = value;
			}
		}
		public string TitlePrefix
		{
			get
			{
				return (m_strTitlePrefix);
			}
			set
			{
				m_strTitlePrefix = value;
			}
		}
		
		private void Page_Load(System.Object sender, System.EventArgs e)
		{
			int count = 0;
		  	m_refMsg = (new CommonApi()).EkMsgRef;
			System.Text.StringBuilder result = new System.Text.StringBuilder();
			Ektron.Cms.API.JS.RegisterJS(this, Ektron.Cms.API.JS.ManagedScript.EktronWorkareaHelperJS);
			AppImgPath = m_refContentApi.AppImgPath;
			result.Append("<table><tr>" + "\r\n");
			
			if (EnableDefaultTitlePrefix)
			{
				m_strTitlePrefix = m_refMsg.GetMessage("content reports title bar msg");
			}
			
			if (!(Request.QueryString["action"] == null))
			{
				PageAction = Request.QueryString["action"].ToLower();
			}

			bool addDivider = false;

			if (PageAction == "viewallreporttypes")
			{
				txtTitleBar.InnerHtml = m_refStyle.GetTitleBar(m_strTitlePrefix);
				result.Append(m_refStyle.GetButtonEventsWCaption(this.m_refContentApi.AppPath + "images/UI/Icons/back.png", "history.back()", m_refMsg.GetMessage("alt back button text"), m_refMsg.GetMessage("btn back"), "", StyleHelper.BackButtonCssClass, true));
				
				addDivider = true;
			}
			else
			{
				bool primaryCssApplied = false;

				txtTitleBar.InnerHtml = m_refStyle.GetTitleBar(m_strTitlePrefix + " " + TitleBarMsg);
				if (PageAction == "viewcheckedout")
				{
					if (!(Data == null))
					{
						result.Append(m_refStyle.GetButtonEventsWCaption(this.m_refContentApi.AppPath + "images/UI/Icons/checkIn.png", "#", m_refMsg.GetMessage("alt:checkin all selected icon text"), m_refMsg.GetMessage("btn checkin"), "onclick=\"return GetIDs();\"", StyleHelper.CheckInButtonCssClass, !primaryCssApplied));

						primaryCssApplied = true;
						addDivider = true;
					}
				}

				if (PageAction == "contentreviews")
				{
					if (!(Data == null))
					{
						result.Append(m_refStyle.GetButtonEventsWCaption(this.m_refContentApi.AppPath + "images/UI/Icons/save.png", "#", m_refMsg.GetMessage("alt:save all sel rev icon text"), m_refMsg.GetMessage("btn save"), "onclick=\"CheckApproveSelect(); return false;\"", StyleHelper.SaveButtonCssClass, !primaryCssApplied));

						primaryCssApplied = true;
						addDivider = true;
					}
				}

				if (PageAction == "contentflags")
				{
					result.Append(m_refStyle.GetButtonEventsWCaption(this.m_refContentApi.AppPath + "images/UI/Icons/chartBar.png", "#", "Click here to view report", m_refMsg.GetMessage("btn report"), "onclick=\"return ReportContentFlags();\"", StyleHelper.ViewReportButtonCssClass, !primaryCssApplied));

					primaryCssApplied = true;
					addDivider = true;
				}

				if (PageAction == "viewcheckedin")
				{
					if (!(Data == null))
					{
						result.Append(m_refStyle.GetButtonEventsWCaption(this.m_refContentApi.AppPath + "images/UI/Icons/approvalSubmitFor.png", "#", m_refMsg.GetMessage("alt:submit all selected icon text"), m_refMsg.GetMessage("btn submit"), "onclick=\"return GetIDs();\"", StyleHelper.ApproveButtonCssClass, !primaryCssApplied));

						primaryCssApplied = true;
						addDivider = true;
					}
				}

				if (PageAction == "viewtoexpire")
				{
					result.Append(m_refStyle.GetButtonEventsWCaption(this.m_refContentApi.AppPath + "images/UI/Icons/chartBar.png", "#", "Click here to view report", m_refMsg.GetMessage("btn report"), "onclick=\"return ReportContentToExpire();\"", StyleHelper.ViewReportButtonCssClass, !primaryCssApplied));

					primaryCssApplied = true;
					addDivider = true;
				}
				
				result.Append("<td>");

				if (true == HasData)
				{
					result.Append(m_refStyle.GetButtonEventsWCaption(this.m_refContentApi.AppPath + "images/UI/Icons/print.png", "#", m_refMsg.GetMessage("Print Report button text"), m_refMsg.GetMessage("btn print"), "onclick=\"PrintReport();\"", StyleHelper.PrintButtonCssClass, !primaryCssApplied));

					primaryCssApplied = true;

					if (EnableEmail)
					{
						result.Append(m_refStyle.GetButtonEventsWCaption(this.m_refContentApi.AppPath + "images/UI/Icons/email.png", "#", m_refMsg.GetMessage("Email Report button text"), m_refMsg.GetMessage("btn email"), "onclick=\"LoadUserListChildPage(\'" + PageAction + "\');\"", StyleHelper.EmailButtonCssClass));
					}

					addDivider = true;
				}
				
				if ((PageAction.ToLower() != "siteupdateactivity") && (PageAction.ToLower() != "viewasynchlogfile") && (PageAction.ToLower() != "viewpreapproval") && EnableFolders)
				{
					result.Append("<td>");
					result.Append(m_refStyle.GetButtonEventsWCaption(this.m_refContentApi.AppPath + "images/UI/Icons/folder.png", "#", m_refMsg.GetMessage("filter report folder"), m_refMsg.GetMessage("filter report folder"), "onclick=\"LoadFolderChildPage(\'" + PageAction + "\',\'" + ContentLanguage + "\');\"", StyleHelper.FilterReportButtonCssClass, !primaryCssApplied));

					primaryCssApplied = true;
					addDivider = true;
				}

				result.Append("<td>");
				if (! Utilities.IsMac() && "siteupdateactivity" == PageAction.ToLower() && true == HasData)
				{
					result.Append(m_refStyle.GetButtonEventsWCaption(this.m_refContentApi.AppPath + "images/UI/Icons/tableExport.png", "#", m_refMsg.GetMessage("btn export"), m_refMsg.GetMessage("btn export"), "onclick=\"export_result();\"", StyleHelper.ExportButtonCssClass, !primaryCssApplied));

					primaryCssApplied = true;
					addDivider = true;
				}
			}

			if (addDivider)
			{
				result.Append(StyleHelper.ActionBarDivider);
			}
			
			if (EnableMultilingual == 1 && (PageAction.ToLower() != "viewasynchlogfile") && (PageAction.ToLower() != "viewpreapproval"))
			{
				SiteAPI m_refsite = new SiteAPI();
				LanguageData[] language_data = m_refsite.GetAllActiveLanguages();
				count = 0;
				result.Append("<td class=\"label\">");
				result.Append(m_refMsg.GetMessage("lbl View") + ":");
				result.Append("</td>");
				result.Append("<td>");
				result.Append("<select id=selLang name=selLang OnChange=\"LoadLanguage(\'selections\');\">");
				if (ContentLanguage == -1)
				{
					result.Append("<option value=" + Ektron.Cms.Common.EkConstants.ALL_CONTENT_LANGUAGES + " selected>All</option>");
				}
				else
				{
					result.Append("<option value=" + Ektron.Cms.Common.EkConstants.ALL_CONTENT_LANGUAGES + ">All</option>");
				}
				for (count = 0; count <= language_data.Length - 1; count++)
				{
					if (Convert.ToString((short) ContentLanguage) == Convert.ToString(language_data[count].Id))
					{
						result.Append("<option value=" + language_data[count].Id + " selected>" + language_data[count].Name + "</option>");
					}
					else
					{
						result.Append("<option value=" + language_data[count].Id + ">" + language_data[count].Name + "</option>");
					}
				}
				result.Append("</select>");
				result.Append("</td>");
			}
			if ((PageAction.ToLower() != "viewasynchlogfile") && (PageAction.ToLower() != "viewpreapproval") && EnableContentTypes)
			{
				GetAddMultiType();
				// If there is no content type from querystring check for the cookie and restore it to that value else all types
				
				if (Request.QueryString[Ektron.Cms.Common.EkConstants.ContentTypeUrlParam] != "")
				{
					if (Information.IsNumeric(Request.QueryString[Ektron.Cms.Common.EkConstants.ContentTypeUrlParam]))
					{
						lContentType = Convert.ToInt32(Request.QueryString[Ektron.Cms.Common.EkConstants.ContentTypeUrlParam]);
						m_refContentApi.SetCookieValue(Ektron.Cms.Common.EkConstants.ContentTypeUrlParam, lContentType.ToString());
					}
				}
				else if (Ektron.Cms.CommonApi.GetEcmCookie()[Ektron.Cms.Common.EkConstants.ContentTypeUrlParam] != "")
				{
					if (Information.IsNumeric(Ektron.Cms.CommonApi.GetEcmCookie()[Ektron.Cms.Common.EkConstants.ContentTypeUrlParam]))
					{
						lContentType = System.Convert.ToInt32(Ektron.Cms.CommonApi.GetEcmCookie()[Ektron.Cms.Common.EkConstants.ContentTypeUrlParam]);
					}
				}
			}
			
			result.Append(StyleHelper.ActionBarDivider);

			result.Append("<td>");
			result.Append(m_refStyle.GetHelpButton(m_strPageAction, ""));
			result.Append("</td>");
			result.Append("</tr></table>");
			htmToolBar.InnerHtml = result.ToString();
		}
		public long GetAddMultiType()
		{
			long returnValue;
			// gets ID for "add multiple" asset type
			returnValue = 0;
			int count;
			asset_data = m_refContentApi.GetAssetSupertypes();
			if (asset_data != null)
			{
				
				for (count = 0; count <= asset_data.Length - 1; count++)
				{
					if (Ektron.Cms.Common.EkConstants.ManagedAsset_Min <= asset_data[count].TypeId && asset_data[count].TypeId <= Ektron.Cms.Common.EkConstants.ManagedAsset_Max)
					{
						if ("*" == asset_data[count].PluginType)
						{
							returnValue = asset_data[count].TypeId;
						}
					}
				}
			}
			return returnValue;
		}
	}