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
using Ektron.Cms.UI.CommonUI;
using Ektron.Cms.Site;
using Ektron.Cms.User;
using Ektron.Cms.Content;
using Ektron.Cms.Common;

	public partial class MetaSelect : System.Web.UI.Page
	{

		protected long UserId;
		protected StyleHelper m_refStyle = new StyleHelper();
		protected long m_nMetaTypeId = -1;
		protected int m_nMetaTagType = Ektron.Cms.Common.EkConstants.MetaTagType_Collection;
		protected long m_nId = -1;
		protected string m_strTitle = "";
		protected int m_nMetadataFormTagId;
		protected EkMessageHelper m_refMsg;
		protected string AppImgPath = "";
		protected EkContent m_refContent = new Ektron.Cms.Content.EkContent();
		protected SiteAPI m_refSiteApi = new SiteAPI();
		protected LanguageData language_data;
		protected int ContentLanguage = -1;
		protected bool _isCollection = false;
		protected bool _isMenu = false;
		protected bool _isUser = false;
		protected string m_strSelectedItem = "-1";
		protected string m_strKeyWords = "";
		protected string m_strSearchText = "";
		protected int _currentPageNumber = 1;
		protected int TotalPagesNumber = 1;
		protected ContentAPI m_refContentApi = new ContentAPI();
		
		private void Page_Load(System.Object sender, System.EventArgs e)
		{
			RegisterResources();
			m_refMsg = m_refSiteApi.EkMsgRef;
			ContentLanguage = m_refSiteApi.ContentLanguage;
			if (ContentLanguage == Ektron.Cms.Common.EkConstants.CONTENT_LANGUAGES_UNDEFINED || ContentLanguage == Ektron.Cms.Common.EkConstants.ALL_CONTENT_LANGUAGES)
			{
				ContentLanguage = m_refSiteApi.DefaultContentLanguage;
			}
			language_data = m_refSiteApi.GetLanguageById(ContentLanguage);
			
			m_refContent = m_refSiteApi.EkContentRef;
			AppImgPath = m_refSiteApi.AppImgPath;
			
			if (Request.QueryString["id"] != null && Information.IsNumeric(Request.QueryString["id"]))
			{
				m_nId = Convert.ToInt64(Request.QueryString["id"]);
			}
			if (Request.QueryString["type"] != null)
			{
				m_nMetaTypeId = Convert.ToInt64(Request.QueryString["type"]);
			}
			if (Request.QueryString["tagtype"] != null)
			{
				m_nMetaTagType = Convert.ToInt32(Request.QueryString["tagtype"]);
			}
			if (Ektron.Cms.Common.EkConstants.MetaTagType_Menu == m_nMetaTagType)
			{
				_isMenu = true;
			}
			else if (Ektron.Cms.Common.EkConstants.MetaTagType_User == m_nMetaTagType)
			{
				_isUser = true;
				CollectSearchText();
			}
			else
			{
				_isCollection = true;
			}
			if (Request.QueryString["title"] != null)
			{
				m_strTitle = Request.QueryString["title"];
			}
			if (Request.QueryString["metadataformtagid"] != null)
			{
				m_nMetadataFormTagId = Convert.ToInt32(Request.QueryString["metadataformtagid"]);
			}
			
			ShowControls();
		}
		
		// Render the UI:
		public void ShowControls()
		{
            Literal1.Text = string.Empty;
			System.Text.StringBuilder result = new System.Text.StringBuilder();
			CollectionListData[] gtNavs = null;
			EkContent gtObj = null;
			ApplicationAPI AppUI = new ApplicationAPI();
			
			long caller_id = AppUI.RequestInformationRef.CallerId;
			AppUI.RequestInformationRef.CallerId = EkConstants.InternalAdmin;
			gtObj = AppUI.EkContentRef;
			AppUI.RequestInformationRef.CallerId = caller_id;
			result.Append(m_refStyle.GetClientScript() + "\r\n");
			result.Append("<input type=HIDDEN value=\"true\" name=\"postback\" id=\"postback\"/>");
			result.Append("<table width=\"100%\" class=\"ektronGrid\">" + "\r\n");
			result.Append("	<tr>" + "\r\n");
			
			result.Append("		<td class=\"ektronTitlebar forceTitlebar\">" + "\r\n");
			
			if (_isMenu)
			{
				result.Append("			" + m_refMsg.GetMessage("lbl select menu") + "\r\n");
			}
			else if (_isUser)
			{
				result.Append("			" + m_refMsg.GetMessage("lbl select user") + "\r\n");
			}
			else
			{
				result.Append("			" + m_refMsg.GetMessage("lbl select collection") + "\r\n");
			}
			result.Append("		</td>" + "\r\n");
            result.Append("	</tr>" + "\r\n");
			if (_isUser)
			{
				result.Append("	<tr>" + "\r\n");
				result.Append("	<td class=\"ektronToolbar forceToolbar\">" + "\r\n");
				result.Append("			<table>" + "\r\n");
				result.Append("				<tr>" + "\r\n");
				
				result.Append("					<td>&nbsp;</td>" + "\r\n");
				
				result.Append("<td class=\"label\">&nbsp;" + m_refMsg.GetMessage("btn search") + ":<input type=text class=\"ektronTextMedium\" id=txtSearch name=txtSearch value=\"" + m_strKeyWords + "\" onkeydown=\"CheckForReturn(event)\">");
				result.Append("<select id=searchlist name=searchlist>");
				result.Append("<option value=-1" + IsSelected("-1") + ">All</option>");
				result.Append("<option value=\"last_name\"" + IsSelected("last_name") + ">" + m_refMsg.GetMessage("generic lastname") + "</option>");
				result.Append("<option value=\"first_name\"" + IsSelected("first_name") + ">" + m_refMsg.GetMessage("generic firstname") + "</option>");
				result.Append("<option value=\"display_name\"" + IsSelected("display_name") + ">" + m_refMsg.GetMessage("display name label") + "</option>");
				result.Append("</select><input type=button value=" + m_refMsg.GetMessage("btn search") + " id=btnSearch name=btnSearch onclick=\"searchuser();\" title=\"Search Users\"></td>");
				
				result.Append("				</tr>" + "\r\n");
				result.Append("			</table>" + "\r\n");
				result.Append("		</td>" + "\r\n");
				result.Append("	</tr>" + "\r\n");
			}
			result.Append("</table>" + "\r\n");
			
			
			if (_isMenu)
			{
				PageRequestData req = new PageRequestData();
				req.PageSize = m_refContentApi.RequestInformationRef.PagingSize;
				req.CurrentPage = _currentPageNumber;
				string searchText = string.Empty;
				caller_id = AppUI.RequestInformationRef.CallerId;
				AppUI.RequestInformationRef.CallerId = EkConstants.InternalAdmin;
				Collection menu_list = AppUI.EkContentRef.GetMenuReport(searchText, ref req);
				AppUI.RequestInformationRef.CallerId = caller_id;
				ConfigurePaging(req.TotalPages);
				
				result.Append("<table width=\"100%\" class=\"ektronGrid\">" + "\r\n");
				result.Append("	<tr>" + "\r\n");
				result.Append("		<td class=\"title-header\" width=\"30%\">" + m_refMsg.GetMessage("generic title") + "</td>" + "\r\n");
				result.Append("		<td class=\"title-header\" width=\"5%\">" + m_refMsg.GetMessage("generic id") + "</td>" + "\r\n");
				result.Append("		<td class=\"title-header\">" + m_refMsg.GetMessage("generic description") + "</td>" + "\r\n");
				result.Append("		<td class=\"title-header\">" + m_refMsg.GetMessage("generic language") + "</td>" + "\r\n");
				result.Append("		<td class=\"title-header\">" + m_refMsg.GetMessage("generic path") + "</td>" + "\r\n");
				result.Append("	</tr>" + "\r\n");
				
				if (menu_list != null && menu_list.Count > 0)
				{
					string strBoldStart = "";
					string strBoldEnd = "";
					string title = "";

                    foreach (Collection temp_cList in menu_list)
					{
                        title = Server.HtmlDecode(temp_cList["MenuTitle"].ToString());
                        if (m_nId == Convert.ToInt64(temp_cList["MenuID"]))
						{
							strBoldStart = "<b>";
							strBoldEnd = "</b>";
						}
						else
						{
							strBoldStart = "";
							strBoldEnd = "";
						}
						
						result.Append("	<tr>		" + "\r\n");
						result.Append("		<td>" + strBoldStart);
                        result.Append("			<a href=\"#\" onclick=\"UpdateFormData(\'" + temp_cList["MenuID"]);
						result.Append("\', \'" + title.Replace("\'", "\\\'") + "\', \'" + m_nMetadataFormTagId + "\');return false;\">");
						result.Append(title + "</a>");
						result.Append(strBoldEnd + "</td> " + "\r\n");
                        result.Append("		<td>" + strBoldStart + temp_cList["MenuID"] + strBoldEnd + "</td>" + "\r\n");
                        result.Append("		<td>" + strBoldStart + temp_cList["MenuDescription"] + strBoldEnd + "</td>" + "\r\n");
                        result.Append("		<td>" + strBoldStart + temp_cList["ContentLanguage"] + strBoldEnd + "</td>" + "\r\n");
                        result.Append("		<td>" + strBoldStart + temp_cList["Path"] + strBoldEnd + "</td>" + "\r\n");
						result.Append("	</tr>" + "\r\n");
					}
				}
				result.Append("</table>" + "\r\n");
				Literal1.Text += result.ToString();
				result = null;
				gtObj = null;
				menu_list = null;
				
			}
			else if (_isUser)
			{
				Ektron.Cms.API.User.User refUserApi = new Ektron.Cms.API.User.User();
				UserData[] user_list;
				UserRequestData req = new UserRequestData();
				int idx;
				string dispName;
				
				req.Type = -1; // Assigning -1 to retrieve all users in the system
				req.SearchText = m_strSearchText;
				req.PageSize = m_refContentApi.RequestInformationRef.PagingSize; // unlimited.
				req.CurrentPage = _currentPageNumber;
				user_list = refUserApi.GetAllUsers(ref req);
				ConfigurePaging(req.TotalPages);
				
				result.Append("<table width=\"100%\" class=\"ektronGrid forceMarginTop40\">" + "\r\n");
				result.Append("	<tr>" + "\r\n");
				result.Append("		<td class=\"title-header\" width=\"5%\">" + m_refMsg.GetMessage("generic id") + "</td>" + "\r\n");
				result.Append("		<td class=\"title-header\" >" + m_refMsg.GetMessage("display name label") + "</td>" + "\r\n");
				result.Append("		<td class=\"title-header\" >" + m_refMsg.GetMessage("lbl first name") + "</td>" + "\r\n");
				result.Append("		<td class=\"title-header\" >" + m_refMsg.GetMessage("lbl last name") + "</td>" + "\r\n");
				result.Append("	</tr>" + "\r\n");
				
				if (user_list != null)
				{
					for (idx = 0; idx <= user_list.Length - 1; idx++)
					{
						if (999999999 == user_list[idx].Id)
						{
							continue;
						}
						dispName = (string) ((0 < user_list[idx].DisplayName.Length) ? (user_list[idx].DisplayName) : (user_list[idx].FirstName));
						result.Append("	<tr>		" + "\r\n");
						result.Append("		<td>" + user_list[idx].Id + "</td>" + "\r\n");
						result.Append("		<td>");
						result.Append("			<a href=\"#\" onclick=\"UpdateFormData(\'" + user_list[idx].Id);
						result.Append("\', \'" + dispName.Replace("\'", "\\\'") + "\', \'" + m_nMetadataFormTagId + "\');return false;\">");
						result.Append(dispName + "</a>");
						result.Append("</td> " + "\r\n");
						result.Append("		<td>" + user_list[idx].FirstName + "</td>" + "\r\n");
						result.Append("		<td>" + user_list[idx].LastName + "</td>" + "\r\n");
						result.Append("	</tr>" + "\r\n");
					}
				}
				
				result.Append("</table>" + "\r\n");
				Literal1.Text += result.ToString();
				result = null;
				refUserApi = null;
				
			}
			else
			{
				// collections:
				result.Append("<table width=\"100%\" class=\"ektronGrid forceMarginTop40\">" + "\r\n");
				result.Append("	<tr>" + "\r\n");
				result.Append("		<td class=\"title-header\" width=\"30%\">" + m_refMsg.GetMessage("generic title") + "</td>" + "\r\n");
				result.Append("		<td class=\"title-header\" width=\"5%\">" + m_refMsg.GetMessage("generic id") + "</td>" + "\r\n");
				result.Append("		<td class=\"title-header\">" + m_refMsg.GetMessage("generic description") + "</td>" + "\r\n");
				result.Append("		<td class=\"title-header\">" + m_refMsg.GetMessage("generic path") + "</td>" + "\r\n");
				result.Append("	</tr>" + "\r\n");
				PageRequestData req = new PageRequestData();
				req.PageSize = m_refContentApi.RequestInformationRef.PagingSize;
				req.CurrentPage = _currentPageNumber;
				string searchText = string.Empty;
				gtNavs = gtObj.GetCollectionList(searchText, ref req);
				ConfigurePaging(req.TotalPages);
				if (gtNavs != null && gtNavs.Length  > 0)
				{
					int count = 0;
					string strBoldStart = "";
					string strBoldEnd = "";
					string title = "";

                    for (count = 0; count <= gtNavs.Length - 1; count++)
					{
						title = "";
						title = Server.HtmlDecode(gtNavs[count].Title.ToString());
						if (m_nId == gtNavs[count].Id)
						{
							strBoldStart = "<b>";
							strBoldEnd = "</b>";
						}
						else
						{
							strBoldStart = "";
							strBoldEnd = "";
						}
						
						result.Append("	<tr>		" + "\r\n");
						result.Append("		<td>" + strBoldStart);
						result.Append("			<a href=\"#\" onclick=\"UpdateFormData(\'" + gtNavs[count].Id);
						result.Append("\', \'" + title.Replace("\'", "\\\'") + "\', \'" + m_nMetadataFormTagId + "\');return false;\">");
						result.Append(title + "</a>");
						result.Append(strBoldEnd + "</td> " + "\r\n");
                        result.Append("		<td>" + strBoldStart + gtNavs[count].Id + strBoldEnd + "</td>" + "\r\n");
                        result.Append("		<td>" + strBoldStart + gtNavs[count].Description + strBoldEnd + "</td>" + "\r\n");
                        result.Append("		<td>" + strBoldStart + gtNavs[count].FolderPath + strBoldEnd + "</td>" + "\r\n");
						result.Append("	</tr>" + "\r\n");
						
					}
					gtObj = null;
					gtNavs = null;
				}
				result.Append("</table>" + "\r\n");
				
				Literal1.Text += result.ToString();
				result = null;
			}
			
		}
		
		private string IsSelected(string val)
		{
			if (val == m_strSelectedItem)
			{
				return (" selected ");
			}
			else
			{
				return ("");
			}
		}
		
		private void CollectSearchText()
		{
            if (!string.IsNullOrEmpty(Request.Form["txtSearch"]))
            {
                m_strKeyWords = Request.Form["txtSearch"];
            }
            if (!string.IsNullOrEmpty(Request.Form["searchlist"]))
            {
                m_strSelectedItem = Request.Form["searchlist"];
            }
			if (m_strSelectedItem == "-1")
			{
				m_strSearchText = " (first_name like \'%" + Quote(m_strKeyWords) + "%\' OR last_name like \'%" + Quote(m_strKeyWords) + "%\' OR display_name like \'%" + Quote(m_strKeyWords) + "%\')";
			}
			else if (m_strSelectedItem == "last_name")
			{
				m_strSearchText = " (last_name like \'%" + Quote(m_strKeyWords) + "%\')";
			}
			else if (m_strSelectedItem == "first_name")
			{
				m_strSearchText = " (first_name like \'%" + Quote(m_strKeyWords) + "%\')";
			}
			else if (m_strSelectedItem == "display_name")
			{
				m_strSearchText = " (display_name like \'%" + Quote(m_strKeyWords) + "%\')";
			}
		}
		
		private string Quote(string KeyWords)
		{
			string result = KeyWords;
			if (KeyWords.Length > 0)
			{
				result = KeyWords.Replace("\'", "\'\'");
			}
			return result;
		}
		protected void RegisterResources()
		{
			Ektron.Cms.API.Css.RegisterCss(this, Ektron.Cms.API.Css.ManagedStyleSheet.EktronWorkareaCss);
			
			Ektron.Cms.API.JS.RegisterJS(this, Ektron.Cms.API.JS.ManagedScript.EktronJS);
		}
		protected void NavigationLink_Click(object sender, CommandEventArgs e)
		{
			switch (e.CommandName)
			{
				case "First":
					_currentPageNumber = 1;
					break;
				case "Last":
                    _currentPageNumber = int.Parse((string)TotalPages.Text);
					break;
				case "Next":
					_currentPageNumber = System.Convert.ToInt32(int.Parse((string) CurrentPage.Text) + 1);
					break;
				case "Prev":
					_currentPageNumber = System.Convert.ToInt32(int.Parse((string) CurrentPage.Text) - 1);
					break;
			}
			ShowControls();
		}
		public void ConfigurePaging(int _TotalPages)
		{
			TotalPagesNumber = _TotalPages;
            TotalPages.Text = TotalPagesNumber.ToString();
            TotalPages.ToolTip = TotalPages.Text;
            CurrentPage.Text = _currentPageNumber.ToString();
            CurrentPage.ToolTip = CurrentPage.Text;
			if (TotalPagesNumber <= 1)
			{
                TotalPages.Visible = false;
                CurrentPage.Visible = false;
				lnkBtnPreviousPage.Visible = false;
				NextPage.Visible = false;
				LastPage.Visible = false;
				FirstPage.Visible = false;
				PageLabel.Visible = false;
				OfLabel.Visible = false;
			}
			else
			{

                TotalPages.Visible = true;
                CurrentPage.Visible = true;
				lnkBtnPreviousPage.Visible = true;
				NextPage.Visible = true;
				LastPage.Visible = true;
				FirstPage.Visible = true;
				PageLabel.Visible = true;
				OfLabel.Visible = true;
                TotalPages.Text = TotalPagesNumber.ToString();

                CurrentPage.Text = _currentPageNumber.ToString();
				
				if (_currentPageNumber == 1)
				{
					
					lnkBtnPreviousPage.Enabled = false;
					
					if (TotalPagesNumber > 1)
					{
						NextPage.Enabled = true;
					}
					else
					{
						NextPage.Enabled = false;
					}
					
				}
				else
				{
					
					lnkBtnPreviousPage.Enabled = true;
					
					if (_currentPageNumber == TotalPagesNumber)
					{
						NextPage.Enabled = false;
					}
					else
					{
						NextPage.Enabled = true;
					}
					
				}
			}
		}
	}