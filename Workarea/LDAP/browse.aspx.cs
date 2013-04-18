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
using Ektron.Cms.Workarea;

	public partial class LDAPbrowse : workareabase
	{
		
		//NOTE: The following placeholder declaration is required by the Web Form Designer.
		//Do not delete or move it.
		//private System.Object designerPlaceholderDeclaration;
		
		string sMethod = "";
		protected int m_intGroupType = 0;
		protected long m_intGroupId = 2;

        protected override void Page_Load(System.Object sender, System.EventArgs e)
		{
			//Put user code to initialize the page here
			if (! (Request.QueryString["grouptype"] == null) && Request.QueryString["grouptype"] != "")
			{
				m_intGroupType = Convert.ToInt32(Request.QueryString["grouptype"]);
			}
			if (! (Request.QueryString["groupid"] == null) && Request.QueryString["groupid"] != "")
			{
				m_intGroupId = Convert.ToInt64(Request.QueryString["groupid"]);
			}
			Hashtable hts = new Hashtable();
			string connection = "";
			UserAPI m_refUserApi = new UserAPI();
			Ektron.Cms.User.EkUser m_refUser;
			
			string tmpPath = "";
			string path = "";
			string strUser = "";
			string[] arrUser;
			SettingsData setting_data;
			SiteAPI m_refSiteApi = new SiteAPI();
			StringBuilder sbTemp = new StringBuilder();
			StringBuilder sbBrowseJS = new StringBuilder();
			
			
			try
			{
				if (Request.QueryString["method"] != "")
				{
					sMethod = Request.QueryString["method"];
					if (Page.IsPostBack == true && Request.Form["cn_path"] != "" && Request.Form["cn_name"] != "")
					{
						AddLDAPUsersToSystem();
						Response.Redirect("../users.aspx?action=viewallusers&grouptype=" + m_intGroupType.ToString() + "&groupid=" + m_intGroupId.ToString() + "&id=" + m_intGroupId.ToString() + "&OrderBy=user_name", false);
						return;
					}
				}
				m_refMsg = m_refUserApi.EkMsgRef;
				setting_data = m_refSiteApi.GetSiteVariables(-1);
				StyleSheetJS.Text = m_refStyle.GetClientScript();
				
				if (Request.QueryString["path"] != null)
				{
					path = (string) (Request.QueryString["path"].ToString().Replace("~", "="));
				}
				m_refUser = m_refUserApi.EkUserRef;
                if (setting_data.LDAPSettings.ServerType == Ektron.Cms.Common.EkEnumeration.LDAPServerType.ActiveDirectory ||
                    setting_data.LDAPSettings.ServerType == Ektron.Cms.Common.EkEnumeration.LDAPServerType.Novell)
				{
					hts = m_refUser.BrowseLDAPTree(path, ref connection, m_refUserApi.RequestInformationRef.ADUsername, m_refUserApi.RequestInformationRef.ADPassword);
				}
				else
				{
					hts = m_refUser.BrowseLDAPTree(path, ref connection, "", "");
				}
				if (path.EndsWith(","))
				{
					tmpPath = path.Remove(System.Convert.ToInt32(path.Length - 1), 1);
				}
				ltrorgdomain.Text = connection;
				ltrpath.Text = tmpPath;
				
				sbTemp.Append("<script language=\"javascript\">" + Environment.NewLine);
				sbTemp.Append("    var connection = \'" + connection + "\';" + Environment.NewLine);
				sbTemp.Append("    var path = \'" + tmpPath + "\';" + Environment.NewLine);
				sbTemp.Append("</script>" + Environment.NewLine);
				
				sbBrowseJS.Append("     <script language=\"javascript\">" + Environment.NewLine);
				sbBrowseJS.Append("	function toggleVisibility(me){").Append(Environment.NewLine);
				sbBrowseJS.Append("		if (me.style.visibility==\"hidden\"){").Append(Environment.NewLine);
				sbBrowseJS.Append("			me.style.visibility=\"visible\";").Append(Environment.NewLine);
				sbBrowseJS.Append("			}").Append(Environment.NewLine);
				sbBrowseJS.Append("		else {").Append(Environment.NewLine);
				sbBrowseJS.Append("			me.style.visibility=\"hidden\";").Append(Environment.NewLine);
				sbBrowseJS.Append("	    }").Append(Environment.NewLine);
				sbBrowseJS.Append("	}").Append(Environment.NewLine);
				
				sbBrowseJS.Append("			function SelectUser(cn) {" + Environment.NewLine);
				sbBrowseJS.Append("				var rExp;" + Environment.NewLine);
				sbBrowseJS.Append("				var path2;" + Environment.NewLine);
				sbBrowseJS.Append("				rExp = /ou=/gi;" + Environment.NewLine);
				sbBrowseJS.Append("				if (path.length == 0){path2 = path;} else {path2 = path + \',\';}" + Environment.NewLine);
				if (sMethod == "select")
				{
					// sbBrowseJS.Append("				alert(path2 + connection);" & Environment.NewLine)
					sbBrowseJS.Append("				toggleVisibility(document.getElementById(\'dvHoldMessage\'));" + Environment.NewLine);
					sbBrowseJS.Append("				document.getElementById(\'cn_name\').value = cn;" + Environment.NewLine);
					sbBrowseJS.Append("				document.getElementById(\'cn_path\').value = path2 + connection;" + Environment.NewLine);
					sbBrowseJS.Append("				document.forms[0].submit();" + Environment.NewLine);
				}
				else
				{
					if (Request.QueryString["from"] == "users")
					{
						sbBrowseJS.Append("				window.opener.document.forms[0]." + (Request.QueryString["uniqueid"]) + "LDAP_username.value = cn;" + Environment.NewLine);
                        if (setting_data.LDAPSettings.ServerType == Ektron.Cms.Common.EkEnumeration.LDAPServerType.ActiveDirectory)
						{
							sbBrowseJS.Append("				var rExp_cn;" + Environment.NewLine);
							sbBrowseJS.Append("				rExp_cn = /cn=/gi;" + Environment.NewLine);
						}
						sbBrowseJS.Append("				window.opener.document.forms[0]." + (Request.QueryString["uniqueid"]) + "LDAP_ldapdomain.value = path2 + connection;" + Environment.NewLine);
					}
					else if (Request.QueryString["from"] == "members")
					{
						sbBrowseJS.Append("				window.opener.userinfo.username.value = cn;" + Environment.NewLine);
						sbBrowseJS.Append("				window.opener.userinfo.domain.value = path2 + connection;" + Environment.NewLine);
					}
					else if (Request.QueryString["from"] == "setup")
					{
						sbBrowseJS.Append("				window.opener.userinfo.username.value = cn;" + Environment.NewLine);
						sbBrowseJS.Append("				window.opener.userinfo.domain.value = path2 + connection;" + Environment.NewLine);
					}
					sbBrowseJS.Append("				self.close();" + Environment.NewLine);
				}
				sbBrowseJS.Append("			}" + Environment.NewLine);
				sbBrowseJS.Append("		</script>" + Environment.NewLine);
				
				if (path.Length > 0)
				{
					path = path.Replace("=", "~");
					sbTemp.Append("<br/><a href=\"#\" onClick=\"history.back(1);\">Up</a><br/>");
				}
				else
				{
					sbTemp.Append("<br/>&nbsp;<br/>");
				}
				
				IDictionaryEnumerator Enumerator;
				Enumerator = hts.GetEnumerator();
				while (Enumerator.MoveNext())
				{
					//Response.Write(Enumerator.Key.ToString() & "<br/>")
                    if (setting_data.LDAPSettings.ServerType == Ektron.Cms.Common.EkEnumeration.LDAPServerType.ActiveDirectory)
					{
						string[] aRay;
						string cnstring = "";
						string userstring = "";
						string tString = Enumerator.Key.ToString();
						aRay = tString.Split(new string[] {"||"}, StringSplitOptions.None);
						userstring = aRay[0];
						cnstring = aRay[1];
						if (!string.IsNullOrEmpty(userstring) && (cnstring.ToLower().IndexOf("cn=") == 0) || (cnstring.ToLower().IndexOf("uid=") > -1))
						{
							strUser = userstring.Replace("\'", "\\\'");
							sbTemp.Append("<br><a href=\"#\" onclick=\"SelectUser(\'" + strUser + "\');\">");
							sbTemp.Append("<img src=\"" + AppImgPath + "../UI/Icons/user.png\" /></a>");
							sbTemp.Append("&nbsp;&nbsp;" + cnstring); //+ "<b>(" + Enumerator.Value.ToString() + ")</b>")
						}
						else if ((cnstring.ToLower().IndexOf("ou=") == 0) || (cnstring.ToLower().IndexOf("cn=") == 0))
						{
							sbTemp.Append("<br><a href=\"browse.aspx?grouptype=" + m_intGroupType + "&groupid=" + m_intGroupId.ToString() + "&method=" + sMethod + ("&from=" + Request.QueryString["from"] + "&uniqueid=" + Request.QueryString["uniqueid"] + "&path=" + cnstring.Replace("=", "~") + "," + path + "\">"));
							sbTemp.Append("<img src=\"" + AppImgPath + "ico_menu-closed.gif\" border=0/></a>&nbsp;&nbsp;");
							sbTemp.Append("&nbsp;&nbsp;" + cnstring.ToString()); //+ "<b>(" + Enumerator.Value.ToString() + ")</b>")
						}
					}
					else
					{
						if (Enumerator.Value.ToString().ToLower().IndexOf("nsmanageddomain") > -1 || Enumerator.Value.ToString().ToLower().IndexOf("person") > -1 || Enumerator.Value.ToString().ToLower().IndexOf("organizational") > -1 || Enumerator.Value.ToString().ToLower().IndexOf("organization") > -1)
						{
							if ((Enumerator.Key.ToString().ToLower().IndexOf("cn=") == 0) || (Enumerator.Key.ToString().ToLower().IndexOf("uid=") > -1))
							{
								arrUser = (Enumerator.Key.ToString()).Split('=');
								strUser = arrUser[1];
								strUser = strUser.Replace("\'", "\\\'");
								sbTemp.Append("<br><a href=\"#\" onclick=\"SelectUser(\'" + strUser + "\');\">");
								sbTemp.Append("<img src=\"" + AppImgPath + "../UI/Icons/user.png\" /></a>");
								sbTemp.Append("&nbsp;&nbsp;" + Enumerator.Key.ToString()); //+ "<b>(" + Enumerator.Value.ToString() + ")</b>")
							}
							else if ((Enumerator.Key.ToString().ToLower().IndexOf("ou=") == 0) || (Enumerator.Key.ToString().ToLower().IndexOf("o=") == 0) || (Enumerator.Key.ToString().ToLower().IndexOf("dc=") == 0))
							{
								sbTemp.Append("<br><a href=\"browse.aspx?grouptype=" + m_intGroupType + "&groupid=" + m_intGroupId.ToString() + "&method=" + sMethod + ("&from=" + Request.QueryString["from"] + "&uniqueid=" + Request.QueryString["uniqueid"] + "&path=" + Enumerator.Key.ToString().Replace("=", "~") + "," + path + "\">"));
								sbTemp.Append("<img src=\"" + AppImgPath + "ico_menu-closed.gif\" border=0/></a>&nbsp;&nbsp;");
								sbTemp.Append("&nbsp;&nbsp;" + Enumerator.Key.ToString()); //+ "<b>(" + Enumerator.Value.ToString() + ")</b>")
							}
						}
					}
				}
				ltrMain.Text = sbTemp.ToString();
				ltrBrowseJS.Text = sbBrowseJS.ToString();
				BrowseLDAPToolBar();
			}
			catch (Exception ex)
			{
				if (ex.Message.IndexOf("operational") > -1 || ex.Message.ToLower().IndexOf("no such object") > -1)
				{
					Utilities.ShowError(m_refMsg.GetMessage("ldap setup err")); //"Unable to connect to your LDAP Server. Please verify your setup configuration.")
				}
				else
				{
					Utilities.ShowError(ex.Message);
				}
			}
		}
		
		private void BrowseLDAPToolBar()
		{
			System.Text.StringBuilder result = new System.Text.StringBuilder();
			base.SetTitleBarToString("LDAP Users");
			//result.Append("<table><tr>")
            //result.Append(m_refStyle.GetButtonEventsWCaption(AppImgPath & "../UI/Icons/cancel.png", "javascript:self.close();", "Close", "Close", "", StyleHelper.CancelButtonCssClass, true))
			//result.Append("<td>")
			if (sMethod == "select")
			{
				base.AddBackButton((string) ("../users.aspx?action=AddUserToSystem&grouptype=" + m_intGroupType + "&groupid=" + m_intGroupId.ToString()));
			}
			else
			{
                base.AddButton(AppImgPath + "../UI/Icons/cancel.png", "javascript:self.close();", "Close", "Close", "", StyleHelper.CancelButtonCssClass, true);
			}
			//result.Append(m_refStyle.GetHelpButton("BrowseLDAP"))
			//result.Append("</td>")
			//result.Append("</tr></table>")
			//htmToolBar.InnerHtml = result.ToString
		}
		
		private void AddLDAPUsersToSystem()
		{
			Collection sdUsersNames = new Collection();
			Collection sdUsersDomains = new Collection();
			string strUsername = "";
			string strDomain = "";
			strUsername = "";
			strDomain = "";
			strUsername = Request.Form["cn_name"].ToString();
			strDomain = Request.Form["cn_path"].ToString();
			if ((strUsername != "") && (strDomain != ""))
			{
				sdUsersNames.Add(strUsername, "0", null, null);
				sdUsersDomains.Add(strDomain, "0", null, null);
			}
			Response.Write(strUsername + " " + strDomain + "<br /");
			Ektron.Cms.User.EkUser usr;
			bool ret = false;
			UserAPI m_refUserApi = new UserAPI();
			usr = m_refUserApi.EkUserRef;
			ret = usr.AddLDAPUsersToCMSByUsername(sdUsersNames, sdUsersDomains, m_intGroupType);
		}
	}
	

