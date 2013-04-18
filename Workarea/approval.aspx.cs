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
//using Ektron.Cms.Common.EkConstants;
using Microsoft.Security.Application;
using Ektron.Cms.Common;


	public partial class approval : System.Web.UI.Page
	{
		
		#region Members
		
		private CommonApi _CommonApi = new CommonApi();
		private EkContent _EkContent;
		protected SiteAPI _SiteApi = new SiteAPI();
		protected UserAPI _UserApi = new UserAPI();
		protected string _PageAction = "";
		private ViewApprovalList _ViewApprovalList;
		private ViewApprovalContent _ViewApprovalContent;
		protected int _ContentLanguage = -1;
		protected int _EnableMultilingual = 0;
		
		#endregion
		
		#region Events
		
		private void Page_Init(System.Object sender, System.EventArgs e)
		{
			Response.CacheControl = "no-cache";
			Response.AddHeader("Pragma", "no-cache");
			Response.Expires = -1;
			RegisterResources();
		}
		private void Page_Load(System.Object sender, System.EventArgs e)
		{
            StyleHelper shelper = new StyleHelper();
            litStyleSheetJS.Text = shelper.GetClientScript();
            Utilities.ValidateUserLogin();
			if (!(Request.QueryString["action"] == null))
			{
				if (Request.QueryString["action"] != "")
				{
					_PageAction = (string) (Ektron.Cms.Common.EkFunctions.HtmlEncode(Request.QueryString["action"].ToLower()));
				}
			}
			if (!(Request.QueryString["LangType"] == null))
			{
				if (Request.QueryString["LangType"] != "")
				{
					_ContentLanguage = Convert.ToInt32(Request.QueryString["LangType"]);
					_CommonApi.SetCookieValue("LastValidLanguageID", _ContentLanguage.ToString());
				}
				else
				{
					if (_CommonApi.GetCookieValue("LastValidLanguageID") != "")
					{
						_ContentLanguage = Convert.ToInt32(_CommonApi.GetCookieValue("LastValidLanguageID"));
					}
				}
			}
			else
			{
				if (_CommonApi.GetCookieValue("LastValidLanguageID") != "")
				{
					_ContentLanguage = Convert.ToInt32(_CommonApi.GetCookieValue("LastValidLanguageID"));
				}
			}
			
			if (_ContentLanguage.ToString() == "CONTENT_LANGUAGES_UNDEFINED")
			{
				_CommonApi.ContentLanguage = Convert.ToInt32("ALL_CONTENT_LANGUAGES");
			}
			else
			{
				_CommonApi.ContentLanguage = _ContentLanguage;
			}
			
			_EnableMultilingual = System.Convert.ToInt32(_CommonApi.EnableMultilingual);
            EmailHelper ehelp = new EmailHelper();
            EmailArea.Text = ehelp.MakeEmailArea();
		}
		private void Page_PreRender(object sender, System.EventArgs e)
		{
			try
			{
				switch (_PageAction.ToLower())
				{
					case "viewapprovallist":
						_ViewApprovalList = (ViewApprovalList) (LoadControl("controls/approval/viewapprovallist.ascx"));
						_ViewApprovalList.MultilingualEnabled = _EnableMultilingual;
						_ViewApprovalList.ID = "viewApprovalList";
						_ViewApprovalList.ContentLang = _CommonApi.ContentLanguage;
						this.DataHolder.Controls.Add(_ViewApprovalList);
						break;
					case "approvecontentaction":
						ApproveContent();
						break;
					case "editContentAction":
						EditContent();
						break;
					case "declinecontentaction":
						DeclineContent();
						break;
					case "viewcontent":
						_ViewApprovalContent = (ViewApprovalContent) (LoadControl("controls/approval/viewapprovalcontent.ascx"));
						this.DataHolder.Controls.Add(_ViewApprovalContent);
						break;
				}
			}
			catch (Exception ex)
			{
				Utilities.ShowError(ex.Message);
			}
		}
		
		#endregion
		
		#region Helpers
		
		private void EditContent()
		{
			long lId;
			bool ret;
			string strPage = "";
			lId = long.Parse(Request.QueryString["id"]);
			try
			{
				strPage = Request.QueryString["page"];
				_EkContent = _CommonApi.EkContentRef;
				ret = System.Convert.ToBoolean(_EkContent.TakeOwnership(lId));
				Response.Redirect((string) ("edit.aspx?LangType=" + _CommonApi.ContentLanguage + "&id=" + lId + "&type=update&back_page=" + strPage), false);
			}
			catch (Exception ex)
			{
				Utilities.ShowError(ex.Message);
			}
		}
		private void DeclineContent()
		{
			long lId;
			bool ret;
			try
			{
				lId = long.Parse(Request.QueryString["id"]);
				_EkContent = _CommonApi.EkContentRef;
				string reason = "";
				if (! (Request.QueryString["comment"] == null))
				{
					reason = Request.QueryString["comment"];
				}
				ret = System.Convert.ToBoolean(_EkContent.DeclineApproval2_0(lId, reason));
				
				if (Request.QueryString["page"] == "workarea")
				{
					//' re-direct to the folder page.
					Response.Redirect((string) ("approval.aspx?action=viewApprovalList&fldid=" + Request.QueryString["fldid"]), false);
					// redirect to workarea
					//Response.Write("<script language=""Javascript"">" & _
					//                       "top.switchDesktopTab();" & _
					//                       "</script>")
				}
				else if (Request.QueryString["page"] == "dmsmenu")
				{
					// re-direct to the folder page.
					//Response.Redirect("approval.aspx?action=viewApprovalList&fldid=" & Request.QueryString("fldid"), False)
				}
				else
				{
					Response.Write("<script language=\"Javascript\">" + "top.opener.location.reload(true);" + "top.close();" + "</script>");
				}
			}
			catch (Exception ex)
			{
				Utilities.ShowError(ex.Message);
			}
			
		}
		private void ApproveContent()
		{
			long lId;
			bool ret;
			try
			{
				_EkContent = _CommonApi.EkContentRef;
				lId = System.Convert.ToInt64(Request.QueryString["id"]);
				ret = System.Convert.ToBoolean(_EkContent.Approvev2_0(lId));

                if (Request.QueryString["page"] == "workarea" || Request.QueryString["page"] == "tree")
				{
					//' redertrect to the folder page.
					//Response.Redirect("approval.aspx?action=viewApprovalList&fldid=" & Request.QueryString("fldid"), False)
					// redirect to workarea
					Response.Write("<script type=\"text/javascript\">" + "var rightFrame = top.document.getElementById(\'ek_main\');" + "var rightFrameUrl = \'approval.aspx?action=viewApprovalList\';" + ("var appPath  = \'" + _CommonApi.AppPath + "\';") + "rightFrame.src = appPath + rightFrameUrl;" + "</script>");
				}
				else
				{
					Response.Write("<script language=\"Javascript\">" + "top.opener.location.reload(true);" + "top.close();" + "</script>");
					
				}
			}
			catch (Exception ex)
			{
				Utilities.ShowError(ex.Message);
			}
		}
		
		#endregion
		
		#region Register JS/CSS
		
		private void RegisterResources()
		{
			Ektron.Cms.API.JS.RegisterJS(this, Ektron.Cms.API.JS.ManagedScript.EktronJFunctJS);
			Ektron.Cms.API.JS.RegisterJS(this, Ektron.Cms.API.JS.ManagedScript.EktronToolBarRollJS);
            noDataPrintMsg.Text = this._CommonApi.EkMsgRef.GetMessage("no data to print");
		}
		
		#endregion
	}