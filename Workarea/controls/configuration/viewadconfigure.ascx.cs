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

	public partial class viewadconfigure : System.Web.UI.UserControl
	{
		
		
        #region  Web Form Designer Generated Code
		
      
		protected StyleHelper m_refStyle = new StyleHelper();
		protected EkMessageHelper m_refMsg;
		protected string AppImgPath = "";
		protected string AppName = "";
		protected UserAPI m_refUserApi;
		protected SiteAPI m_refSiteApi;
		protected SettingsData setting_data;
		protected AdMappingData[] mapping_data;
		protected DomainData[] domain_data;
		protected AdSyncData sync_data;
		protected UserGroupData group_data;
		protected bool AdValid = false;
        
		#endregion
		
		private void Page_Load(System.Object sender, System.EventArgs e)
		{
			//Put user code to initialize the page here
			m_refMsg = (new CommonApi()).EkMsgRef;
		}
		public bool Display_ViewConfiguration()
		{
			m_refUserApi = new UserAPI();
			m_refSiteApi = new SiteAPI();
			AppImgPath = m_refUserApi.AppImgPath;
			AppName = m_refUserApi.AppName;
			RegisterResources();
			setting_data = m_refSiteApi.GetSiteVariables(m_refUserApi.UserId);
			mapping_data = m_refUserApi.GetADMapping(m_refUserApi.UserId, "userprop", 1, 0, 1);
			group_data = m_refUserApi.GetUserGroupById(1);
			sync_data = m_refUserApi.GetADStatus();
			//domain_data = m_refUserApi.GetDomains(1, 0)
			AdValid = setting_data.AdValid; //CBool(siteVars("AdValid"))
			ViewToolBar();
			//VERSION
			versionNumber.InnerHtml = m_refMsg.GetMessage("version") + "&nbsp;" + m_refSiteApi.Version + "&nbsp;" + m_refSiteApi.ServicePack;
			//BUILD NUMBER
			buildNumber.InnerHtml = "<i>(" + m_refMsg.GetMessage("build") + m_refSiteApi.BuildNumber + ")</i>";
			
			licenseMessageContainer.Visible = false;
			if (!(AdValid))
			{
				TR_domaindetail.Visible = false;
				licenseMessageContainer.Visible = true;
				licenseMessage.InnerHtml = m_refMsg.GetMessage("entrprise license with AD required msg");
			}
			else
			{
				if ((sync_data.SyncUsers) || (sync_data.SyncGroups) || (sync_data.SyncRelationships) || (sync_data.DeSyncUsers) || (sync_data.DeSyncGroups))
				{
					if (setting_data.ADAuthentication == 1)
					{
						ltr_status.Text = "<a href=\"adreports.aspx?action=ViewAllReportTypes\">" + m_refMsg.GetMessage("ad enabled not configured") + "</a>";
					}
					else
					{
						ltr_status.Text = "<a href=\"adreports.aspx?action=ViewAllReportTypes\">" + m_refMsg.GetMessage("ad disabled not configured") + "</a>";
					}
				}
				else
				{sync.Visible  =false;
					
				}
				if (setting_data.IsAdInstalled)
				{
					installed.InnerHtml = m_refMsg.GetMessage("active directory installed") + "&nbsp;";
				}
				else
				{
					installed.InnerHtml = m_refMsg.GetMessage("active directory not installed") + "&nbsp;";
				}
				TD_flag.InnerHtml = m_refMsg.GetMessage("active directory authentication flag");
				if (setting_data.ADAuthentication == 1)
				{
					TD_flagenabled.InnerHtml = m_refMsg.GetMessage("AD enabled");
				}
				else if (setting_data.ADAuthentication == 2)
				{
					TD_flagenabled.InnerHtml = m_refMsg.GetMessage("LDAP enabled");
				}
				else
				{
					TD_flagenabled.InnerHtml = m_refMsg.GetMessage("disabled");
				}
				TD_dirflag.InnerHtml = m_refMsg.GetMessage("active directory flag");
				if (setting_data.ADIntegration)
				{
					TD_intflag.InnerHtml = m_refMsg.GetMessage("enabled");
				}
				else
				{
					TD_intflag.InnerHtml = m_refMsg.GetMessage("disabled");
				}
				TD_autouser.InnerHtml = m_refMsg.GetMessage("auto add user flag");
				if (setting_data.ADAutoUserAdd)
				{
					TD_autouserflag.InnerHtml = m_refMsg.GetMessage("enabled");
				}
				else
				{
					TD_autouserflag.InnerHtml = m_refMsg.GetMessage("disabled");
				}

                lgd_autoAddType.InnerHtml = m_refMsg.GetMessage("lbl auto add header");
                autoAddTypeProperty.InnerHtml = m_refMsg.GetMessage("lbl auto add user type");
                if (setting_data.ADAutoUserAddType == Ektron.Cms.Common.EkEnumeration.AutoAddUserTypes.Member)
                {
                    autoAddTypeValue.InnerHtml = m_refMsg.GetMessage("lbl member");
                }
                else
                {
                    autoAddTypeValue.InnerHtml = m_refMsg.GetMessage("lbl author");
                }

				TD_autogroup.InnerHtml = m_refMsg.GetMessage("auto add user to group flag");
				
				if (setting_data.ADAutoUserToGroup)
				{
					TD_autogroupflag.InnerHtml = m_refMsg.GetMessage("enabled");
				}
				else
				{
					TD_autogroupflag.InnerHtml = m_refMsg.GetMessage("disabled");
				}
				userProperty.InnerHtml = m_refMsg.GetMessage("user property mapping title");
				TD_cmstitle.InnerHtml = m_refMsg.GetMessage("cms property title");
				TD_dirproptitle.InnerHtml = m_refMsg.GetMessage("active directory property title");
				int i = 0;
				if (!(mapping_data == null))
				{
					System.Text.StringBuilder result = new System.Text.StringBuilder();
					for (i = 0; i <= mapping_data.Length - 1; i++)
					{
						result.Append("<tr>");
                        result.Append("<td class=\"label\">" + GetResourseText(mapping_data[i].CmsName) + ":</td>");
						result.Append("<td class=\"readOnlyValue\">" + mapping_data[i].AdName + "</td>");
						result.Append("<tr>");
					}
					mapping_list.Text = result.ToString();
				}
				adminGroupMap.InnerHtml = m_refMsg.GetMessage("cms admin group map");
				TD_grpnameval.InnerHtml = group_data.GroupName;
				TD_grpDomainVal.InnerHtml = group_data.GroupDomain;
				domain.InnerHtml = m_refMsg.GetMessage("domain title") + ":";
				//If (domain_data.Length = 0) Then
				//domainValue.InnerHtml += "<font color=""red""><strong>" & m_refMsg.GetMessage("generic no domains found") & " " & m_refMsg.GetMessage("generic check ad config msg") & "</strong></font>"
				//Else
				if (setting_data.ADDomainName == "")
				{
					domainValue.InnerHtml += m_refMsg.GetMessage("all domain select caption");
				}
				else if (setting_data.ADAuthentication == 2)
				{
					domainValue.InnerHtml += m_refMsg.GetMessage("all domain select caption");
				}
				else
				{
					domainValue.InnerHtml += setting_data.ADDomainName;
				}
				//End If
			}
            return false;
		}
        private string GetResourseText(string st)
        {
            if (st == "EmailAddr1")
                st = m_refMsg.GetMessage("lbl EmailAddr1");
            else if (st == "FirstName")
                st = m_refMsg.GetMessage("generic firstname");
            else if (st == "LastName")
                st = m_refMsg.GetMessage("generic lastname");
            
            return st;
        }
		private void ViewToolBar()
		{
			System.Text.StringBuilder result = new System.Text.StringBuilder();
			try
			{
				
				divTitleBar.InnerHtml = m_refStyle.GetTitleBar(m_refMsg.GetMessage("adconfig page title"));
				if ((AdValid == true) || ((Strings.LCase(Request.ServerVariables["SERVER_NAME"]) == "localhost") || (Request.ServerVariables["SERVER_NAME"] == "127.0.01")) && ((setting_data.ADAuthentication == 1) || (setting_data.ADAuthentication == 2)))
				{
					result.Append("<table><tr>");
					//If (domain_data.Length = 0) Then
					//result.Append(m_refStyle.GetButtonEventsWCaption(AppImgPath & "../UI/Icons/contentEdit.png", "adconfigure.aspx?action=edit", m_refMsg.GetMessage("alt edit settings button text"), m_refMsg.GetMessage("btn edit"), "Onclick=""javascript:alert('" & m_refMsg.GetMessage("javascript: alert cannot edit no domains") & "\n" & m_refMsg.GetMessage("generic check ad config msg") & "'); return false;"""))
					//Else
					result.Append(m_refStyle.GetButtonEventsWCaption(AppImgPath + "../UI/Icons/contentEdit.png", "adconfigure.aspx?action=edit", m_refMsg.GetMessage("alt edit settings button text"), m_refMsg.GetMessage("btn edit"), "", StyleHelper.EditButtonCssClass,true));
					//End If
					result.Append(StyleHelper.ActionBarDivider);
					result.Append("<td>");
					result.Append(m_refStyle.GetHelpButton("viewadconfigure_ascx", ""));
					result.Append("</td>");
					result.Append("</tr></table>");
				}
				
				divToolBar.InnerHtml = result.ToString();
				result = null;
			}
			catch (Exception ex)
			{
				Utilities.ShowError(ex.Message);
			}
		}
		private void RegisterResources()
		{
			Ektron.Cms.API.JS.RegisterJS(this, Ektron.Cms.API.JS.ManagedScript.EktronEmpJSFuncJS);
			Ektron.Cms.API.JS.RegisterJS(this, Ektron.Cms.API.JS.ManagedScript.EktronToolBarRollJS);
			Ektron.Cms.API.JS.RegisterJS(this, Ektron.Cms.API.JS.ManagedScript.EktronWorkareaHelperJS);
		}
	}
	

