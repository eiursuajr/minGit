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
using Ektron.Cms.Content;
//using Ektron.Cms.Common.EkEnumeration;


	public partial class purgehist : System.Web.UI.Page
	{
		
		protected CommonApi m_refAPI = new CommonApi();
		protected EkContent m_contentRef;
		private Ektron.Cms.Common.EkMessageHelper m_refMsg;
		protected StyleHelper m_refStyle = new StyleHelper();
		protected string m_strPageAction = "";
		protected long m_FolderId;
		
		protected void Page_Init(object sender, System.EventArgs e)
		{
			RegisterResources();
		}
		
		private void Page_Load(System.Object sender, System.EventArgs e)
		{
            
            StyleSheetJS.Text = (new StyleHelper()).GetClientScript().ToString();
			m_refMsg = m_refAPI.EkMsgRef;
			Utilities.ValidateUserLogin();
            setLocalText();
			if (m_refAPI.UserId == 0 || Convert.ToBoolean(m_refAPI.RequestInformationRef.IsMembershipUser))
			{
				Response.Redirect("login.aspx?fromLnkPg=1", false);
				return;
			}
			if (!(Request.QueryString["action"] == null))
			{
				if (Request.QueryString["action"] != "")
				{
					m_strPageAction = Request.QueryString["action"].ToLower();
				}
			}
		}
		
		private void Page_PreRender(object sender, System.EventArgs e)
		{
			try
			{
				switch (m_strPageAction.ToLower())
				{
					case "view":
						if (Page.IsPostBack)
						{
							Action_PurgeHistory();
						}
						else
						{
							Display_PurgeHistory();
						}
						break;
				}
			}
			catch (Exception ex)
			{
				Utilities.ShowError(ex.Message);
			}
		}
		
		private void RegisterResources()
		{
			Ektron.Cms.API.JS.RegisterJS(this, this.m_refAPI.ApplicationPath + "java/workareahelper.js", "EktronWorkareaHelperJS");
			Ektron.Cms.API.JS.RegisterJS(this, this.m_refAPI.ApplicationPath + "java/toolbar_roll.js", "EktronToolbarRollJS");
			Ektron.Cms.API.JS.RegisterJS(this, this.m_refAPI.ApplicationPath + "java/jfunct.js", "EktronJFunctJS");
			Ektron.Cms.API.JS.RegisterJS(this, this.m_refAPI.ApplicationPath + "java/internCalendarDisplayFuncs.js", "EktronInternCalendardDisplayFunctJS");
			
			Ektron.Cms.API.Css.RegisterCss(this, Ektron.Cms.API.Css.ManagedStyleSheet.EktronWorkareaCss);
			Ektron.Cms.API.Css.RegisterCss(this, Ektron.Cms.API.Css.ManagedStyleSheet.EktronFixedPositionToolbarCss);
		}
		
		private void Display_PurgeHistory()
		{
			Collection cFolder;
			EkDTSelector dateSel = m_refAPI.EkDTSelectorRef;
			m_contentRef = m_refAPI.EkContentRef;
			m_FolderId = System.Convert.ToInt64(Request.QueryString["folderid"]);
			cFolder = m_contentRef.GetFolderInfoWithPath(m_FolderId);
			litFolderPath.Text = cFolder["Path"].ToString();
			litFolderName.Text = cFolder["FolderName"].ToString();
			dateSel.formName = "frmMain";
			dateSel.formElement = "frm_purge_date";
			dateSel.spanId = "span_purge_date";
			litCal.Text = dateSel.displayCultureDate(true, "", "");
			frm_folder_id.Value = m_FolderId.ToString();
			ViewToolBar();
		}
		private void Action_PurgeHistory()
		{
			Collection iSrc = new Collection();
			bool bRet;
			string ErrorString = "";
			string datePurge = DateTime.Today.ToShortDateString();
			try
			{
				m_contentRef = m_refAPI.EkContentRef;
				if (Request.Form["frm_purge_date"] != "")
				{
					datePurge = Request.Form["frm_purge_date"];
				}
				iSrc.Add(datePurge, "PurgeDate", null, null);
				m_FolderId = System.Convert.ToInt64(frm_folder_id.Value);
				iSrc.Add(m_FolderId, "FolderID", null, null);
				if (this.chkRecursivePurge.Checked)
				{
					iSrc.Add(1, "Recursive", null, null);
				}
				else
				{
					iSrc.Add(0, "Recursive", null, null);
				}
				if (this.chkPurgePublished.Checked)
				{
					iSrc.Add(1, "IncludePublished", null, null);
				}
				else
				{
					iSrc.Add(0, "IncludePublished", null, null);
				}
				bRet = m_contentRef.PurgeContentHitory(iSrc, ErrorString);
				if (ErrorString == "")
				{
					Response.Redirect((string) ("content.aspx?action=ViewFolder&id=" + m_FolderId), false);
				}
			}
			catch (Exception ex)
			{
				Utilities.ShowError(ex.Message);
			}
		}
		
		private void ViewToolBar()
		{
			System.Text.StringBuilder result = new System.Text.StringBuilder();
			divTitleBar.InnerHtml = m_refStyle.GetTitleBar((string) (m_refMsg.GetMessage("purge content history for folder") + ""));
			result.Append("<table><tr>");
			result.Append(m_refStyle.GetButtonEventsWCaption(m_refAPI.AppPath + "images/UI/Icons/back.png", (string) ("content.aspx?action=ViewFolder&id=" + m_FolderId), m_refMsg.GetMessage("alt back button text"), m_refMsg.GetMessage("btn back"), "", StyleHelper.BackButtonCssClass,true));
            result.Append(m_refStyle.GetButtonEventsWCaption(m_refAPI.AppPath + "images/UI/Icons/historyDelete.png", "#", m_refMsg.GetMessage("alt purge history"), m_refMsg.GetMessage("btn purge history"), "Onclick=\"javascript:return SubmitForm();\"", StyleHelper.DeleteHistoryButtonCssClass,true));
			result.Append(StyleHelper.ActionBarDivider);
			result.Append("<td>");
			result.Append(m_refStyle.GetHelpButton("purghistory", ""));
			result.Append("</td>");
			result.Append("</tr></table>");
			divToolBar.InnerHtml = result.ToString();
		}
        private void setLocalText()
        {
            lblPurgeForFolder.Text = m_refMsg.GetMessage("lbl folder purge foldername");
            lblPurgeForFolder.ToolTip = "";
            lblFolderPath.Text = m_refMsg.GetMessage("lbl folder purge folderpath");
            lblFolderPath.ToolTip = "";
            lblPurgeDate.Text = m_refMsg.GetMessage("lbl folder purge versionlimit");
            lblPurgeDate.ToolTip = "";
            chkRecursivePurge.Text = m_refMsg.GetMessage("lbl folder purge recusive");
            chkRecursivePurge.ToolTip = "";
            chkPurgePublished.Text = m_refMsg.GetMessage("lbl folder purge purgepublish");
            chkPurgePublished.ToolTip = "";
            ltr_NodateMsg.Text = m_refMsg.GetMessage("js:purge histoty nodate");
        }
	}

