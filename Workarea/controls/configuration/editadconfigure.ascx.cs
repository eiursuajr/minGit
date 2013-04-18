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

	public partial class editadconfigure : System.Web.UI.UserControl
	{
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
		protected UserGroupData cGroup;
		protected bool m_bADAdvanced = false;
	
		
		private void Page_Load(System.Object sender, System.EventArgs e)
		{
			//Put user code to initialize the page here
			m_refUserApi = new UserAPI();
            //ltrHttpHose.Text = Request.ServerVariables[http_host).ToLower();
			m_bADAdvanced = m_refUserApi.RequestInformationRef.ADAdvancedConfig;
			jsUniqueID.Text = UniqueID + "_";
			m_refMsg = (new CommonApi()).EkMsgRef;
			RegisterResources();
		}
		public bool EditAdConfiguration()
		{ 
			try
			{
				if (!(Page.IsPostBack))
				{
					Display_EditConfiguration();
				}
				else
				{
					Update();
					return (true);
				}
			}
			catch (Exception ex)
			{
				throw (new Exception(ex.Message));
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
		private bool Display_EditConfiguration()
		{
            string[] arrOrg;
            string[] arrItem;
			long arrCount;
			string[] arrDomain;
			long arrCount2;
            string[] arrOrg2;
			bool isUnit = false;
            string[] arrServer;
            string[] arrLDAPDomain;
			string strLDAPDomain = "";
			string[] arrLDAPDomainElement;
			bool first = true;
			string adselectedstate = "";
			
			m_refUserApi = new UserAPI();
			m_refSiteApi = new SiteAPI();
			AppImgPath = m_refUserApi.AppImgPath;
			AppName = m_refUserApi.AppName;
			setting_data = m_refSiteApi.GetSiteVariables(-1);
			mapping_data = m_refUserApi.GetADMapping(m_refUserApi.UserId, "userprop", 1, 0, 1);
			cGroup = m_refUserApi.GetUserGroupById(1);
			sync_data = m_refUserApi.GetADStatus();
			
			try
			{
				domain_data = m_refUserApi.GetDomains(1, 0);
			}
			catch
			{
				domain_data = null;
			}
			
			EditToolBar(); //POPULATE TOOL BAR
			//VERSION
			versionNumber.InnerHtml = m_refMsg.GetMessage("version") + "&nbsp;" + m_refSiteApi.Version + "&nbsp;" + m_refSiteApi.ServicePack;
			//BUILD NUMBER
			buildNumber.InnerHtml = "<i>(" + m_refMsg.GetMessage("build") + m_refSiteApi.BuildNumber + ")</i>";
			
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
            //Ektron.Cms.Sync doesn't have "visible"
            //else
            //{
            //    Ektron.Cms.Sync.visible = false;
            //}
			
			if (setting_data.IsAdInstalled)
			{
				installed.InnerHtml = m_refMsg.GetMessage("active directory installed");
			}
			else
			{
				installed.InnerHtml = m_refMsg.GetMessage("active directory not installed");
			}

			if (setting_data.ADAuthentication == 1)
			{
				EnableADAuth.Checked = true;
				adselectedstate = "";
				//OrgUnitText.Disabled = True
				OrgText.Disabled = true;
				ServerText.Disabled = true;
				drp_LDAPtype.Enabled = false;
				PortText.Disabled = true;
				LDAPDomainText.Disabled = true;
				txtLDAPAttribute.Enabled = false;
				LDAP_SSL.Disabled = true;
				admingroupdomain.Disabled = false;
				admingroupname.Disabled = false;
                drpAutoAddType.Enabled = true;
			}
			else if (setting_data.ADAuthentication == 0)
			{
				DisableAD.Checked = true;
				adselectedstate = "disabled";
				//OrgUnitText.Disabled = True
				OrgText.Disabled = true;
				ServerText.Disabled = true;
				drp_LDAPtype.Enabled = false;
				PortText.Disabled = true;
				LDAPDomainText.Disabled = true;
				txtLDAPAttribute.Enabled = false;
				LDAP_SSL.Disabled = true;
				admingroupdomain.Disabled = true;
				admingroupname.Disabled = true;
			}
			else
			{
				EnableLDAP.Checked = true;
				adselectedstate = "disabled";
				//OrgUnitText.Disabled = False
				OrgText.Disabled = false;
				ServerText.Disabled = false;
				drp_LDAPtype.Enabled = true;
				PortText.Disabled = false;
				LDAPDomainText.Disabled = false;
				txtLDAPAttribute.Enabled = true;
				LDAP_SSL.Disabled = false;
				admingroupdomain.Disabled = true;
				admingroupname.Disabled = true;
			}

            lgd_autoAddType.InnerHtml = m_refMsg.GetMessage("lbl auto add header");
            autoAddTypeProperty.InnerHtml = m_refMsg.GetMessage("lbl auto add user type");
            drpAutoAddType.Items.Add(new ListItem(m_refMsg.GetMessage("lbl author"), "0"));
            drpAutoAddType.Items.Add(new ListItem(m_refMsg.GetMessage("lbl member"), "1"));
            drpAutoAddType.SelectedIndex = setting_data.ADAutoUserAddType.GetHashCode();

			if (setting_data.ADIntegration)
			{
				EnableADInt.Checked = true;
			}
			if (!(setting_data.ADAuthentication == 1))
			{
				EnableADInt.Disabled = true;
			}
			if (setting_data.ADAutoUserAdd)
			{
				EnableAutoUser.Checked = true;
			}
			if (!(setting_data.ADAuthentication == 1))
			{
				EnableAutoUser.Disabled = true;
			}
			//EnableAutoUserToGroup
			if (setting_data.ADAutoUserToGroup)
			{
				EnableAutoUserToGroup.Checked = true;
			}
			if (!(setting_data.ADAuthentication == 1))
			{
				EnableAutoUserToGroup.Disabled = true;
			}

            userProperty.InnerHtml = m_refMsg.GetMessage("user property mapping");
			cmsProperty.InnerHtml = m_refMsg.GetMessage("cms property value");
			activeDirectoryProperty.InnerHtml = m_refMsg.GetMessage("active directory property value");
			userpropcount.Value = mapping_data.Length.ToString();
			
			int i;
			System.Text.StringBuilder result = new System.Text.StringBuilder();
			if (!(mapping_data == null))
			{
				for (i = 0; i <= mapping_data.Length - 1; i++)
				{
					result.Append("<tr>");
                    result.Append("<td class=\"label\">" + GetResourseText(mapping_data[i].CmsName) + ":</td>");
					result.Append("<td>");
					result.Append("<input type=\"hidden\" maxlength=\"50\" size=\"50\" name=\"userpropname" + (i + 1) + "\"  id=\"userpropname" + (i + 1) + "\" value=\"" + mapping_data[i].CmsName + "\">");
					result.Append("<input type=\"text\" maxlength=\"50\" size=\"25\" " + adselectedstate + " name=\"userpropvalue" + (i + 1) + "\" id=\"userpropvalue" + (i + 1) + "\" value=\"" + mapping_data[i].AdName + "\">");
					result.Append("</td>");
					result.Append("</tr>");
				}
				mapping_list.Text = result.ToString();
				result = null;
			}
			adminGroupMap.InnerHtml = m_refMsg.GetMessage("cms admin group map");
			adGroupName.InnerHtml = m_refMsg.GetMessage("AD Group Name");
			adDomain.InnerHtml = m_refMsg.GetMessage("AD Domain");
			admingroupname.Value = cGroup.GroupName;
			admingroupdomain.Value = cGroup.GroupDomain;
			drp_LDAPtype.Items.Add(new ListItem(m_refMsg.GetMessage("LDAP AD"), "AD"));
			drp_LDAPtype.Items.Add(new ListItem(m_refMsg.GetMessage("LDAP NO"), "NO"));
			drp_LDAPtype.Items.Add(new ListItem(m_refMsg.GetMessage("LDAP SU"), "SU"));
			drp_LDAPtype.Items.Add(new ListItem(m_refMsg.GetMessage("LDAP OT"), "OT"));
			drp_LDAPtype.Attributes.Add("onchange", "javascript:CheckLDAP(\'\', true);");
			if (setting_data.ADAuthentication == 2)
			{
				if (setting_data.ADDomainName.IndexOf("&lt;/p&gt;") + 1 > 0) //defect 17813 - SMK
				{
					setting_data.ADDomainName = setting_data.ADDomainName.Replace("&lt;", "<");
					setting_data.ADDomainName = setting_data.ADDomainName.Replace("&gt;", ">");
					setting_data.ADDomainName = setting_data.ADDomainName.Replace("&quot;", "\"");
					setting_data.ADDomainName = setting_data.ADDomainName.Replace("&#39;", "\'");
				}
				LDAPSettingsData ldapsettings;
				ldapsettings = Ektron.Cms.Common.EkFunctions.GetLDAPSettings(setting_data.ADDomainName);
				
                
                /* From VB source
                arrDomain = Split(setting_data.ADDomainName, "</server>")
                arrServer = Split(arrDomain(0), "</p>")
                */

                string tempDomainName = setting_data.ADDomainName;
                tempDomainName = tempDomainName.Replace("</server>", "|").Replace("</p>", "^");
                arrDomain = tempDomainName.Split("|".ToCharArray());
                arrServer = arrDomain[0].ToString().Split("^".ToCharArray());
				
				ServerText.Value = ldapsettings.Server;
				PortText.Value = ldapsettings.Port.ToString();
				LDAPjs.Text += "<script language=\"javascript\" type=\"text/javascript\">" + Environment.NewLine;
				
				drp_LDAPtype.SelectedIndex = ldapsettings.ServerType.GetHashCode();
				LDAPjs.Text += "     CheckLDAP(\'" + drp_LDAPtype.Items[drp_LDAPtype.SelectedIndex].Value + "\', false);" + Environment.NewLine;
				LDAP_SSL.Checked = ldapsettings.EncryptionType == Ektron.Cms.Common.EkEnumeration.LDAPEncryptionType.SSL;
				txtLDAPAttribute.Text = ldapsettings.Attribute;
				if ((arrServer.Length - 1) > 1)
				{
                    // arrLDAPDomain = Split(arrServer(2), ",") VB Source
                    arrLDAPDomain = arrServer[2].Split(',');
					for (arrCount = 0; arrCount <= (arrLDAPDomain.Length - 1); arrCount++)
					{
                        arrLDAPDomainElement = arrLDAPDomain.GetValue(arrCount).ToString().Split('=');
						if (arrLDAPDomainElement[0] == "dc")
						{
							if (!(strLDAPDomain == ""))
							{
								strLDAPDomain += ".";
							}
                            strLDAPDomain += arrLDAPDomainElement[1];
						}
					}
					LDAPDomainText.Value = strLDAPDomain;
				}

                arrOrg2 = arrDomain[1].Split(new string[] {"</>"}, StringSplitOptions.None);
				for (arrCount2 = 0; arrCount2 <= (arrOrg2.Length - 1); arrCount2++)
				{
					//Response.Write(arrOrg2(arrCount2) & "<br/>")
					if (!(arrOrg2.GetValue(arrCount2).ToString() == ""))
					{
                        arrOrg = arrOrg2.GetValue(arrCount2).ToString().Split(',');
						for (arrCount = 0; arrCount <= (arrOrg.Length - 1); arrCount++)
						{
							if (!(arrOrg.GetValue(arrCount).ToString() == ""))
							{
								//arrItem = Strings.Split(arrOrg(arrCount), "=", -1, 0);
                                arrItem = arrOrg.GetValue(arrCount).ToString().Split('=');
								if ((arrItem[0].Trim() == "o") && arrCount2 == (arrOrg2.Length - 1))
								{
									OrgText.Value = arrItem.GetValue(1).ToString();
									//ElseIf (arrItem(0) = "ou" Or arrItem(0) = " ou") Then
									//    If (Not first) Then
									//        OrgUnitText.Value &= ","
									//    End If
									//    OrgUnitText.Value &= "ou=" & arrItem(1)
									//    isUnit = True
									//    first = False
								}
								else
								{
									if (! first)
									{
										OrgUnitText.Value += ",";
									}
									OrgUnitText.Value += arrOrg.GetValue(arrCount);
									isUnit = true;
									first = false;
								}
							}
						}
						if (isUnit)
						{
							OrgUnitText.Value += "</>";
							isUnit = false;
							first = true;
						}
					}
				}
			}
			if (domain_data == null)
			{
				searchLink.InnerHtml = "<a href=\"#\" OnClick=\"javascript:alert(\'" + m_refMsg.GetMessage("javascript: alert cannot search no domains") + "\\n" + m_refMsg.GetMessage("generic check ad config msg") + "\'); return false;\">" + m_refMsg.GetMessage("generic Search") + "</a>";
			}
			else if (domain_data.Length == 0)
			{
				searchLink.InnerHtml = "<a href=\"#\" OnClick=\"javascript:alert(\'" + m_refMsg.GetMessage("javascript: alert cannot search no domains") + "\\n" + m_refMsg.GetMessage("generic check ad config msg") + "\'); return false;\">" + m_refMsg.GetMessage("generic Search") + "</a>";
			}
			else
			{
				searchLink.InnerHtml = "<a href=\"#\" OnClick=\"javascript:DoSearch();\">" + m_refMsg.GetMessage("generic Search") + "</a>";
			}
			domain.InnerHtml = m_refMsg.GetMessage("domain title") + ":";
			result = new System.Text.StringBuilder();
			result.Append("&nbsp;");

            if (domain_data == null && !m_refUserApi.RequestInformationRef.ADAdvancedConfig)
			{
				string selected = "";
				result.Append("<select " + adselectedstate + " name=\"domainname\" id=\"domainname\">");
				if (setting_data.ADDomainName == "")
				{
					selected = " selected";
				}
				// Keep the "All Domains" drop down for continuity
				result.Append("<option value=\"\" " + selected + ">" + m_refMsg.GetMessage("all domain select caption") + "</option>");
				result.Append("</select>");
			}
			else if ((domain_data == null &&  m_refUserApi.RequestInformationRef.ADAdvancedConfig)
                    || domain_data.Length == 0)
			{
				result.Append("<font color=\"red\"><strong>" + m_refMsg.GetMessage("generic no domains found") + " " + m_refMsg.GetMessage("generic check ad config msg") + "</strong></font>");
			}
			else
			{
                if (m_refUserApi.RequestInformationRef.ADAdvancedConfig)
                {
                    for (i = 0; i <= domain_data.Length - 1; i++)
                    {
                        if (i > 0)
                            result.Append("&nbsp;");
                        result.Append(domain_data[i].Name).Append("<br/>");
                    }
                }
                else
                {
                    string selected = "";
    			    result.Append("<select " + adselectedstate + " name=\"domainname\" id=\"domainname\">");
				    if (setting_data.ADDomainName == "")
				    {
					    selected = " selected";
				    }
				
				    result.Append("<option value=\"\" " + selected + ">" + m_refMsg.GetMessage("all domain select caption") + "</option>");
				    for (i = 0; i <= domain_data.Length - 1; i++)
				    {
					    if (domain_data[i].Name == setting_data.ADDomainName)
					    {
						    selected = " selected";
					    }
					    else
					    {
						    selected = "";
					    }
					    result.Append("<option value=\"" + domain_data[i].Name + "\"" + selected + ">" + domain_data[i].Name + "</option>");
				    }
				    result.Append("</select>");
                }
			}
			domainDropdown.InnerHtml = result.ToString();
            return false;
		}
		private void EditToolBar()
		{
			System.Text.StringBuilder result = new System.Text.StringBuilder();
			try
			{
				txtTitleBar.InnerHtml = m_refStyle.GetTitleBar(m_refMsg.GetMessage("adconfig page title"));
				
				result.Append("<table><tr>");
				result.Append(m_refStyle.GetButtonEventsWCaption(AppImgPath + "../UI/Icons/back.png", "adconfigure.aspx", m_refMsg.GetMessage("alt back button text"), m_refMsg.GetMessage("btn back"), "", StyleHelper.BackButtonCssClass, true));
				if (domain_data == null)
				{
					result.Append(m_refStyle.GetButtonEventsWCaption(AppImgPath + "../UI/Icons/save.png", "#", m_refMsg.GetMessage("alt update settings button text"), m_refMsg.GetMessage("btn update"), "Onclick=\"javascript:return SubmitForm(\'config\', \'VerifyForm()\');\"", StyleHelper.SaveButtonCssClass, true));
				}
				else if (domain_data.Length == 0)
				{
					result.Append(m_refStyle.GetButtonEventsWCaption(AppImgPath + "../UI/Icons/save.png", "#", m_refMsg.GetMessage("alt update settings button text"), m_refMsg.GetMessage("btn update"), "Onclick=\"javascript:alert(\'" + m_refMsg.GetMessage("javascript: alert cannot update no domains") + "\\n" + m_refMsg.GetMessage("generic check ad config msg") + "\'); return false;\"", StyleHelper.SaveButtonCssClass, true));
				}
				else
				{
					result.Append(m_refStyle.GetButtonEventsWCaption(AppImgPath + "../UI/Icons/save.png", "#", m_refMsg.GetMessage("alt update settings button text"), m_refMsg.GetMessage("btn update"), "Onclick=\"javascript:return SubmitForm(\'config\', \'VerifyForm()\');\"", StyleHelper.SaveButtonCssClass, true));
				}
				result.Append(StyleHelper.ActionBarDivider);
				result.Append("<td>");
				result.Append(m_refStyle.GetHelpButton("editadconfigure_ascx", ""));
				result.Append("</td>");
				result.Append("</tr></table>");
				htmToolBar.InnerHtml = result.ToString();
				result = null;
			}
			catch (Exception ex)
			{
				Utilities.ShowError(ex.Message);
			}
		}
		private bool Update()
		{
			try
			{
				int i;
				string Org;
				string Port;
				string LDAPDomain = "";
				Array arrLDAPDomain;
				Array arrOrgU;
				Array arrOrgUSep;
				long arrCount2;
				long arrCount;
				string sChar = ":";
				Hashtable page_addata = new Hashtable();
				System.Collections.Specialized.NameValueCollection pagedata = new System.Collections.Specialized.NameValueCollection();
				for (i = 1; i <= System.Convert.ToInt32(Request.Form[userpropcount.UniqueID]); i++)
				{
                    if (!string.IsNullOrEmpty(Request.Form["userpropvalue" + i.ToString()]))
                    {
                        pagedata.Add(Request.Form["userpropname" + i.ToString()].ToString(), Request.Form["userpropvalue" + i.ToString()].ToString());
                    }
				}
				UserAPI m_refUserApi = new UserAPI();
				//TODO: The following comments added by UDAI on 11/22/05.  for defect#16785
				//while implementing LDAP, ADGroupSelect hardcoded and widely used in this page for VS2003.  I am keeping the same hardcode
				//parameter according to VS2005 (:  to  $).  This page needs radio group using servercontrol property.
				//TODO: The following comments added by SCOTTK on 1/04/06.  for defect# 17121 & 17367
				// We were getting issues with the : to $ switch in 5.1.x, so I am placing code here to detect the right char to show
				if (!string.IsNullOrEmpty(Request.Form[UniqueID + "$ADGroupSelect"]))
				{
					sChar = "$";
				}
				else if (!string.IsNullOrEmpty(Request.Form[UniqueID + "_ADGroupSelect"]))
				{
					sChar = "_";
				}
				else
				{
					sChar = ":";
				}
				if (Request.Form[UniqueID + sChar + "ADGroupSelect"] == "enable_adauth")
				{
					//only update user properties if AD Authentication is to be enabled
					m_refUserApi.UpdateADMapping(m_refUserApi.UserId, pagedata);
				}
				
				if (Request.Form[UniqueID + sChar + "ADGroupSelect"] == "enable_adauth")
				{
					page_addata.Add("ADAuthentication", 1);
				}
				else if (Request.Form[UniqueID + sChar + "ADGroupSelect"] == "disable_adauth")
				{
					page_addata.Add("ADAuthentication", 0);
				}
				else
				{
					page_addata.Add("ADAuthentication", 2);
				}
				if (!string.IsNullOrEmpty(Request.Form[EnableADInt.UniqueID]))
				{
					page_addata.Add("ADIntegration", 1);
				}
				else
				{
					page_addata.Add("ADIntegration", 0);
				}
				
				if (!string.IsNullOrEmpty(Request.Form[EnableAutoUser.UniqueID]))
				{
					page_addata.Add("ADAutoUserAdd", 1);
				}
				else
				{
					page_addata.Add("ADAutoUserAdd", 0);
				}

                page_addata.Add("ADAutoUserAddType", Request.Form[drpAutoAddType.UniqueID]);
                
				if (!string.IsNullOrEmpty(Request.Form[EnableAutoUserToGroup.UniqueID]))
				{
					page_addata.Add("ADAutoUserToGroup", 1);
				}
				else
				{
					page_addata.Add("ADAutoUserToGroup", 0);
				}
				
				if (Request.Form[UniqueID + sChar + "ADGroupSelect"] == "enable_adauth")
				{
                    if (!string.IsNullOrEmpty(Request.Form["domainname"]))
                    { 
                        page_addata.Add("ADDomainName", Request.Form["domainname"].ToString());
                    }
				}
				else if (Request.Form[UniqueID + sChar + "ADGroupSelect"] == "enable_LDAP")
				{
					Org = Request.Form[ServerText.UniqueID].ToString();
					Port = Request.Form[PortText.UniqueID].ToString();
                    if (Request.Form[LDAPDomainText.UniqueID] != null)
                        arrLDAPDomain = (Request.Form[LDAPDomainText.UniqueID].ToString()).Split('.');
                    else
                        arrLDAPDomain = new List<string>().ToArray();
					for (arrCount = 0; arrCount <= (arrLDAPDomain.Length - 1); arrCount++)
					{
						if (!(LDAPDomain == ""))
						{
							LDAPDomain += ",";
						}
						LDAPDomain += "dc=";
						LDAPDomain += arrLDAPDomain.GetValue(arrCount).ToString();
					}
					arrCount = 0;
					Org += "</p>";
					Org += Port;
					if (!string.IsNullOrEmpty(LDAPDomain))
					{
						Org += "</p>";
						Org += LDAPDomain;
					}
					Org += "</server>";
					arrOrgUSep = (Request.Form[OrgUnitText.UniqueID].ToString()).Split(new string[] {"</>"}, StringSplitOptions.None);
					for (arrCount2 = 0; arrCount2 <= (arrOrgUSep.Length - 1); arrCount2++)
					{
						if (!(arrOrgUSep.GetValue(arrCount2).ToString() == ""))
						{
							arrOrgU = Strings.Split(arrOrgUSep.GetValue(arrCount2).ToString(), ",", -1, 0);
							bool first;
							first = true;
							for (arrCount = 0; arrCount <= (arrOrgU.Length - 1); arrCount++)
							{
								if (!(arrOrgU.GetValue(arrCount).ToString() == ""))
								{
									if (!(first))
									{
										Org += ",";
									}
									first = false;
									//Org &= "ou="
									Org += arrOrgU.GetValue(arrCount).ToString();
								}
							}
							Org += "</>";
						}
					}
					if (!string.IsNullOrEmpty(Request.Form[OrgText.UniqueID]))
					{
						Org += "o=";
						Org += Request.Form[OrgText.UniqueID].ToString();
					}
					Org += (string) ("</server>" + Request.Form[drp_LDAPtype.UniqueID]);
					if (!string.IsNullOrEmpty(Request.Form[LDAP_SSL.UniqueID]))
					{
						Org += "</server>" + "SSL";
					}
					else
					{
						Org += "</server>";
					}
					if (!string.IsNullOrEmpty(Request.Form[txtLDAPAttribute.UniqueID]))
					{
						Org += (string) ("</server>" + Ektron.Cms.Common.EkFunctions.GetDbString(Request.Form[txtLDAPAttribute.UniqueID], 20, true));
					}
					else
					{
						Org += "</server>";
					}
					page_addata.Add("ADDomainName", Org);
				}
				else
				{
                    if (!string.IsNullOrEmpty(Request.Form["domainname"]))
                    {
                        page_addata.Add("ADDomainName", Request.Form["domainname"].ToString());
                    }
				}
				
				SiteAPI m_refSiteApi = new SiteAPI();
				
				m_refSiteApi.UpdateSiteVariables(page_addata);
				
				
				if (!string.IsNullOrEmpty(Request.Form[EnableADInt.UniqueID]))
				{
					//only update admin mapping if AD turned on
					m_refUserApi.MapCMSUserGroupToAD(1, Request.Form[admingroupname.UniqueID].ToString(), Request.Form[admingroupdomain.UniqueID].ToString());
				}
				return (true);
				//Response.Redirect("adconfigure.aspx", False)
			}
			catch (Exception ex)
			{
				throw (new Exception(ex.Message));
			}
		}
		private void RegisterResources()
		{
			Ektron.Cms.API.JS.RegisterJS(this, Ektron.Cms.API.JS.ManagedScript.EktronEmpJSFuncJS);
			Ektron.Cms.API.JS.RegisterJS(this, Ektron.Cms.API.JS.ManagedScript.EktronToolBarRollJS);
			Ektron.Cms.API.JS.RegisterJS(this, Ektron.Cms.API.JS.ManagedScript.EktronWorkareaHelperJS);
		}
	}