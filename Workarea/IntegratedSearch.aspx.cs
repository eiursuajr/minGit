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
using Ektron.Cms.Common;
using System.IO;

	public partial class IntegratedSearch : System.Web.UI.Page
	{
		
		protected StyleHelper objStyle;
        protected SearchHelper _SearchHelper;
		protected Ektron.Cms.ContentAPI m_refContentApi = new Ektron.Cms.ContentAPI();
		protected EkMessageHelper gtMess;
        protected long CurrentUserId;
        protected string ErrorString = "";
        protected string action = "";
        protected string AppPath = "";
        protected string AppImgPath = "";
        protected int ContentLanguage;
        protected IntegratedFolderData[] cIntegratedFolderArray;
        protected IntegratedFolderData cIntegratedFolder;
        protected IntegratedFolderData cTempIntFolder;
        protected int EnableMultilingual;
        protected EkContent ContentObj;
        protected SiteAPI AppUI = new SiteAPI();
        protected SiteAPI siteRef = new SiteAPI();
        protected SiteAPI sfSiteRef = new SiteAPI();
        protected long id = 0;
        protected string strAction = "";
        protected bool bView = false;
        protected string strTitleMsg = "";

		private void Page_Load(System.Object sender, System.EventArgs e)
		{
			objStyle = new StyleHelper();
            _SearchHelper = new SearchHelper();
			gtMess = m_refContentApi.EkMsgRef;
            AppPath = siteRef.AppPath;
            AppImgPath = siteRef.AppImgPath;
            CurrentUserId = siteRef.UserId;
            EnableMultilingual = siteRef.EnableMultilingual;

            ltrStyleSheet.Text = objStyle.GetClientScript();
            if (!String.IsNullOrEmpty(Request.QueryString["LangType"]))
            {
                 ContentLanguage = Convert.ToInt32(Request.QueryString["LangType"]);
		         siteRef.SetCookieValue("LastValidLanguageID", ContentLanguage.ToString());
            }
		    else if (!String.IsNullOrEmpty(siteRef.GetCookieValue("LastValidLanguageID")))
            {
                ContentLanguage = Convert.ToInt32(siteRef.GetCookieValue("LastValidLanguageID"));
            }
		    
			RegisterResources();
			SetServerJSVariables();
			
			if (m_refContentApi.RequestInformationRef.IsMembershipUser == 1 || m_refContentApi.RequestInformationRef.UserId == 0 || ! m_refContentApi.EkUserRef.IsAllowed(m_refContentApi.UserId, 0, "users", "IsAdmin", 0))
			{
				Response.Redirect(m_refContentApi.ApplicationPath + "Login.aspx", true);
				return;
			}

            StringBuilder sb = new StringBuilder();
            action = Request.QueryString["action"];
            if ("SubmitIntegratedFolder" == action || "UpdateIntegratedFolder" == action || action == "DeleteIntegratedFolder")
            {
                try
                {
                    if (("SubmitIntegratedFolder" == action) || ("UpdateIntegratedFolder" == action))
				        {
					        cIntegratedFolder = new IntegratedFolderData();
					        cIntegratedFolder.TypeId = System.Convert.ToInt32(Request.Form["integrated_id"]);
					        cIntegratedFolder.DirectoryName = Request.Form["frm_directoryname"];
					        cIntegratedFolder.ExcludeFiles = Request.Form["frm_excludefiles"];
					        cIntegratedFolder.IncludeFiles = Request.Form["frm_includefiles"];
					        cIntegratedFolder.ExcludeDirs = Request.Form["frm_excludedirs"];
					        if (Strings.LCase(Request.Form["frm_recursive"]) == "on")
					        {
						        cIntegratedFolder.Recursive = true;
					        }
					        else
					        {
						        cIntegratedFolder.Recursive = false;
					        }
					        cIntegratedFolder.UserName = Request.Form["DomainUserName"];
					        cIntegratedFolder.Password = Request.Form["Password"];
        					
					        if (Path.IsPathRooted(cIntegratedFolder.DirectoryName))
					        {
						        cIntegratedFolder.AbsolutePath = cIntegratedFolder.DirectoryName;
					        }
					        else
					        {
						        cIntegratedFolder.AbsolutePath = (string) (EnsureTrailingSlash(HttpContext.Current.Request.MapPath((string) ("/" + cIntegratedFolder.DirectoryName))).Replace("/", "\\\\"));
						        if (! Directory.Exists(cIntegratedFolder.AbsolutePath))
						        {
							        cIntegratedFolder.AbsolutePath = (string) (EnsureTrailingSlash(HttpContext.Current.Request.MapPath((string) ("~/" + cIntegratedFolder.DirectoryName))).Replace("/", "\\\\"));
						        }
					        }
        					
					        if ("AddIntegratedFolder" == action)
					        {
                                m_refContentApi.EkContentRef.AddIntegratedFolder(cIntegratedFolder);
					        }
					        else
					        {
						       m_refContentApi.EkContentRef.UpdateIntegratedFolderByID(cIntegratedFolder);
					        }
        					
					        if ("AddIntegratedFolder" == action)
					        {
						        Response.Redirect("integratedsearch.aspx?action=ViewAllIntegratedFolders");
					        }
					        else
					        {
						        Response.Redirect((string) ("integratedsearch.aspx?action=ViewIntegratedFolder&id=" + Request.Form["integrated_id"]));
					        }
        					
				        }
				        else if (action == "DeleteIntegratedFolder")
				        {
					        m_refContentApi.EkContentRef.DeleteIntegratedFolder(System.Convert.ToInt64(Request.QueryString["id"]));
					        Response.Redirect("integratedsearch.aspx?action=ViewAllIntegratedFolders");
				        }
				
			    }
		        catch (Exception ex)
		        {
			        DebugErrLit.Text = ex.Message;
		        }
            }
            else
            {
                if (action == "ViewAllIntegratedFolders")
                {
                    pnlViewAllIntegratedFolders.Visible = true;
                    pnlAddEditViewIntegratedFolder.Visible = false;
                    ContentObj = siteRef.EkContentRef;
                    cIntegratedFolderArray = ContentObj.GetIntegratedFolders();
                    foreach (IntegratedFolderData cTempIntFolder in cIntegratedFolderArray)
                    {
                        sb.Append("<tr>").Append(Environment.NewLine);
                        sb.Append("  <td>").Append(Environment.NewLine);
                        sb.Append("     <a href=\"integratedsearch.aspx?action=ViewIntegratedFolder&id=").Append(cTempIntFolder.TypeId).Append("\" title=\"Click here to view Integrated Search Folder\">").Append(Environment.NewLine);
                        sb.Append(cTempIntFolder.DirectoryName).Append(Environment.NewLine);
                        sb.Append("     </a>").Append(Environment.NewLine);
                        sb.Append("  </td>").Append(Environment.NewLine);
                        sb.Append("  <td>").Append(cTempIntFolder.TypeId).Append("</td>").Append(Environment.NewLine);
                        sb.Append("  <td>").Append((cTempIntFolder.ExcludeFiles).Replace(",", ", ")).Append("</td>").Append(Environment.NewLine);
                        sb.Append("  <td>").Append((cTempIntFolder.IncludeFiles).Replace(",", ", ")).Append("</td>").Append(Environment.NewLine);
                        sb.Append("  <td>").Append((cTempIntFolder.ExcludeDirs).Replace(",", ", ")).Append("</td>").Append(Environment.NewLine);
                        sb.Append("  <td>").Append(cTempIntFolder.UserName).Append("</td>").Append(Environment.NewLine);
                        sb.Append("</tr>").Append(Environment.NewLine);
                    }
                    ltrFolderList.Text = sb.ToString();
                }
                else if ("AddIntegratedFolder" == action || "EditIntegratedFolder" == action || "ViewIntegratedFolder" == action)
                {
                    pnlViewAllIntegratedFolders.Visible = false;
                    pnlAddEditViewIntegratedFolder.Visible = true;

                    bool bNew = false;
                    bool bEdit = false;
                    ContentObj = siteRef.EkContentRef;

                    bNew = ("AddIntegratedFolder" == action);
                    bEdit = ("EditIntegratedFolder" == action);
                    bView = ("ViewIntegratedFolder" == action);
                    if (bNew)
                    {
                        id = 0;
                        strTitleMsg = gtMess.GetMessage("alt Click here to add Integrated Search Folder");
                        cIntegratedFolder = new IntegratedFolderData();
                    }
                    else
                    {
                        id = Convert.ToInt64(Request.QueryString["id"]);
                        if (bEdit)
                        {
                            strTitleMsg = gtMess.GetMessage("alt Edit Integrated Search Folder");
                        }
                        else if (bView)
                        {
                            strTitleMsg = gtMess.GetMessage("alt View Integrated Search Folder");
                        }
                        cIntegratedFolder = ContentObj.GetIntegratedFolderByID(id);
                    }

                    if (bNew)
                    {
                        strAction = "SubmitIntegratedFolder";
                    }
                    else
                    {
                        strAction = "UpdateIntegratedFolder";
                        strTitleMsg = strTitleMsg + " \"" + cIntegratedFolder.DirectoryName + "\"";
                    }

                    if (!bView)
                        Ektron.Cms.API.JS.RegisterJS(this, m_refContentApi.ApplicationPath + "java/optiontransfer.js", "EktronOptionTransferJS");


                    //Toolbar
                    sb = new StringBuilder();
                    if (bNew)
                    {
						sb.Append(objStyle.GetButtonEventsWCaption(AppPath + "images/UI/Icons/back.png", "integratedsearch.aspx?action=ViewAllIntegratedFolders", gtMess.GetMessage("alt back button text"), gtMess.GetMessage("btn back"), "", StyleHelper.BackButtonCssClass, true));
                        sb.Append(objStyle.GetButtonEventsWCaption(AppPath + "images/UI/Icons/save.png", "#", gtMess.GetMessage("alt Click here to add this integrated search folder"), gtMess.GetMessage("btn save"), "Onclick=\"javascript:return SubmitForm('integratedform', 'VerifyIntegratedSearchForm()');\"", StyleHelper.SaveButtonCssClass, true));
                    }
                    else if (bEdit)
                    {
						sb.Append(objStyle.GetButtonEventsWCaption(AppPath + "images/UI/Icons/back.png", "integratedsearch.aspx?action=ViewIntegratedFolder&id=" + id + "", gtMess.GetMessage("alt back button text"), gtMess.GetMessage("btn back"), "", StyleHelper.BackButtonCssClass, true));
                        sb.Append(objStyle.GetButtonEventsWCaption(AppPath + "images/UI/Icons/save.png", "#", gtMess.GetMessage("alt Click here to updated this integrated search folder"), gtMess.GetMessage("btn update"), "Onclick=\"javascript:return SubmitForm('integratedform', 'VerifyIntegratedSearchForm()');\"", StyleHelper.SaveButtonCssClass, true));
                    }
                    else if (bView)
                    {
						sb.Append(objStyle.GetButtonEventsWCaption(AppPath + "images/UI/Icons/back.png", "integratedsearch.aspx?action=ViewAllIntegratedFolders", gtMess.GetMessage("alt back button text"), gtMess.GetMessage("btn back"), "", StyleHelper.BackButtonCssClass, true));
                        sb.Append(objStyle.GetButtonEventsWCaption(AppPath + "images/UI/Icons/contentEdit.png", "integratedsearch.aspx?action=EditIntegratedFolder&id=" + id + "", gtMess.GetMessage("alt Edit this integrated search folder"), gtMess.GetMessage("btn edit"), "", StyleHelper.EditButtonCssClass, true));
                        sb.Append(objStyle.GetButtonEventsWCaption(AppPath + "images/UI/Icons/delete.png", "integratedsearch.aspx?action=DeleteIntegratedFolder&id=" + id + "", gtMess.GetMessage("alt Delete this integrated search folder"), gtMess.GetMessage("btn delete"), "OnClick=\"return ConfirmDelete();\"", StyleHelper.DeleteButtonCssClass));
                    }
					sb.Append(StyleHelper.ActionBarDivider);
                    sb.Append("<td>").Append(objStyle.GetHelpButton(action, "")).Append("</td>").Append(Environment.NewLine);
                    ltrToolBar.Text = sb.ToString();

                    //PageHolder
                    sb = new StringBuilder();
                    sb.Append("<table class=\"ektronForm\">	").Append(Environment.NewLine);
                    //tr
                    sb.Append("     <tr>").Append(Environment.NewLine);
                    sb.Append("         <td class=\"label\"><label for=\"DirectoryName\" title=\"Site Directory\">").Append(gtMess.GetMessage("lbl Site Directory")).Append("</label></td>").Append(Environment.NewLine);
                    if (bView)
                    {
                        sb.Append("     <td class=\"readOnlyValue\" title=\"").Append(cIntegratedFolder.DirectoryName).Append("\" >").Append(cIntegratedFolder.DirectoryName).Append("</td>").Append(Environment.NewLine);
                    }
                    else
                    {
                        sb.Append("     <td class=\"value\">").Append(Environment.NewLine);
                        sb.Append("         <input type=\"text\" title=\"Enter Directory Name here\" id=\"DirectoryName\" name=\"frm_directoryname\" maxlength=\"255\" value=\"").Append(EkFunctions.HtmlEncode(cIntegratedFolder.DirectoryName)).Append("\" />").Append(Environment.NewLine);
                        sb.Append("         <div class=\"ektronCaption\" title=\"Directory or virtual directory relative to site root\">").Append(gtMess.GetMessage("alt Directory or virtual directory relative to site root")).Append("</div>").Append(Environment.NewLine);
                        if (bNew || bEdit)
                            sb.Append("     <span class=\"ektronCaption\" style=\"color:Red\" title=\"Please make sure you create virtual directory first before adding the virtual directory name\">").Append(gtMess.GetMessage("alt the virtual directory name")).Append("</span>").Append(Environment.NewLine);
                        sb.Append("     </td>").Append(Environment.NewLine);
                    }
                    sb.Append("     </tr>").Append(Environment.NewLine);
                    if (!bNew)
                    {
                        sb.Append("  <tr>").Append(Environment.NewLine);
                        sb.Append("     <td class=\"label\" title=\"ID\">").Append(gtMess.GetMessage("id label")).Append("</td>").Append(Environment.NewLine);
                        sb.Append("     <td class=\"readOnlyValue\" title=\"").Append(cIntegratedFolder.TypeId).Append("\" >").Append(cIntegratedFolder.TypeId).Append("</td>").Append(Environment.NewLine);
                        sb.Append("  </tr>").Append(Environment.NewLine);
                    }
                    //tr
                    sb.Append("  <tr>").Append(Environment.NewLine);
                    sb.Append("     <td class=\"label\"><label for=\"Recursive\" title=\"Recursive\">").Append(gtMess.GetMessage("lbl Recursive")).Append(":</label></td>").Append(Environment.NewLine);
                    if (bView)
                        sb.Append("   <td class=\"readOnlyValue\" title=\"").Append(_SearchHelper.BoolToYesNo(cIntegratedFolder.Recursive)).Append("\" >").Append(_SearchHelper.BoolToYesNo(cIntegratedFolder.Recursive)).Append("</td>").Append(Environment.NewLine);
                    else
                        sb.Append("  <td class=\"value\"><input type=\"checkbox\" title=\"Recursive Option\" id=\"Recursive\" name=\"frm_recursive\" ").Append(_SearchHelper.CheckedAttr(cIntegratedFolder.Recursive)).Append(" /></td>").Append(Environment.NewLine);
                    sb.Append("  </tr>").Append(Environment.NewLine);

                    //tr
                    sb.Append("  <tr>").Append(Environment.NewLine);
                    sb.Append("      <td class=\"label\"><label for=\"IncludeFiles\" title=\"Exclude Directories\">").Append(gtMess.GetMessage("lbl exclude directories")).Append("</label></td>").Append(Environment.NewLine);
                    if (bView)
                    {
                        sb.Append("  <td class=\"readOnlyValue\" title=\"").Append((cIntegratedFolder.ExcludeDirs).Replace(",", ", ")).Append("\" >").Append((cIntegratedFolder.ExcludeDirs).Replace(",", ", ")).Append(":</td>").Append(Environment.NewLine);
                    }
                    else
                    {
                        sb.Append("  <td class=\"value\">").Append(Environment.NewLine);
                        sb.Append("    <input type=\"text\" title=\"Enter Exclude Directories here\" id=\"ExcludeDirs\" name=\"frm_excludedirs\" maxlength=\"255\" value=\"").Append(EkFunctions.HtmlEncode((cIntegratedFolder.ExcludeDirs).Replace(",", ", "))).Append("\"/>").Append(Environment.NewLine);
                        sb.Append("    <div class=\"ektronCaption\" title=\"Sub directories not to include in search\" >").Append(gtMess.GetMessage("alt Sub directories not to include in search")).Append("</div>").Append(Environment.NewLine);
                        sb.Append("  </td>").Append(Environment.NewLine);
                    }
                    sb.Append("  </tr>").Append(Environment.NewLine);
                    //tr
                    sb.Append("  <tr>").Append(Environment.NewLine);
                    sb.Append("     <td class=\"label\"><label for=\"ExcludeFiles\" title=\"Exclude Extensions\">").Append(gtMess.GetMessage("lbl exclude extension")).Append(":</label></td>").Append(Environment.NewLine);
                    if (bView)
                    {
                        sb.Append("  <td class=\"readOnlyValue\" title=\"").Append(cIntegratedFolder.ExcludeFiles).Append("\" >").Append(cIntegratedFolder.ExcludeFiles).Append("</td>").Append(Environment.NewLine);
                    }
                    else
                    {
                        sb.Append("  <td class=\"value\">").Append(Environment.NewLine);
                        sb.Append("     <input type=\"text\" title=\"Enter Excluded File Extensions here\" id=\"ExcludeFiles\" name=\"frm_excludefiles\" maxlength=\"255\" value=\"").Append(EkFunctions.HtmlEncode((cIntegratedFolder.ExcludeFiles).Replace(",", ", "))).Append("\" />").Append(Environment.NewLine);
                        sb.Append("     <div class=\"ektronCaption\" title=\"Extensions not to include *.aspx,*.ascx\">").Append(gtMess.GetMessage("alt Extensions not to include *.aspx,*.ascx")).Append("</div>").Append(Environment.NewLine);
                        sb.Append("  <td>").Append(Environment.NewLine);
                    }
                    sb.Append("  </tr>").Append(Environment.NewLine);
                    //tr
                    sb.Append("  <tr>").Append(Environment.NewLine);
                    sb.Append("       <td class=\"label\"><label for=\"IncludeFiles\" title=\"Include Extensions\">").Append(gtMess.GetMessage("lbl include extensions")).Append(":</label></td>").Append(Environment.NewLine);
                    if (bView)
                    {
                        sb.Append("   <td class=\"readOnlyValue\" title=\"").Append(cIntegratedFolder.IncludeFiles).Append("\">").Append(cIntegratedFolder.IncludeFiles).Append("</td>").Append(Environment.NewLine);
                    }
                    else
                    {
                        sb.Append("   <td class=\"value\">").Append(Environment.NewLine);
                        sb.Append("     <input type=\"text\" title=\"Enter Included File Extensions here\" id=\"IncludeFiles\" name=\"frm_includefiles\" maxlength=\"255\" value=\"").Append(EkFunctions.HtmlEncode((cIntegratedFolder.IncludeFiles).Replace(",", ", "))).Append("\"/>").Append(Environment.NewLine);
                        sb.Append("     <div class=\"ektronCaption\" title=\"*.html,*.doc exclude extensions is ignored if include is specified\">").Append(gtMess.GetMessage("alt extensions is ignored if include is specified")).Append("</div>").Append(Environment.NewLine);
                        sb.Append("   </td>").Append(Environment.NewLine);
                    }
                    sb.Append("  </tr>").Append(Environment.NewLine);
                    //tr
                    sb.Append("  <tr>").Append(Environment.NewLine);
                    sb.Append("     <td class=\"label\"><label title=\"Domain/User Name\" for=\"DomainUserName\">").Append(gtMess.GetMessage("lbl domain username")).Append(":</label></td>").Append(Environment.NewLine);
                    if (bView)
                    {
                        sb.Append("  <td class=\"readOnlyValue\">").Append(cIntegratedFolder.UserName).Append(Environment.NewLine);
                    }
                    else
                    {
                        sb.Append("  <td class=\"value\"><input type=\"text\" title=\"Enter Domain or User Name here\" id=\"DomainUserName\" name=\"DomainUserName\" maxlength=\"255\" value=\"").Append(EkFunctions.HtmlEncode(cIntegratedFolder.UserName)).Append("\"/>").Append(Environment.NewLine);
                    }
                    sb.Append("          <div class=\"ektronCaption\" title=\"Specify domain or username\">").Append(gtMess.GetMessage("alt domain username")).Append("</div>").Append(Environment.NewLine);
                    sb.Append("     </td>").Append(Environment.NewLine);
                    sb.Append("  </tr>").Append(Environment.NewLine);



                    if (bNew)
                    {
                        string integratedFolderNames = "";
                        cIntegratedFolderArray = ContentObj.GetIntegratedFolders();
                        foreach (IntegratedFolderData cTempIntFolder in cIntegratedFolderArray)
                        {
                            if (cTempIntFolder.DirectoryName != null && cTempIntFolder.DirectoryName.Length > 0)
                            {
                                if (integratedFolderNames.Length > 0)
                                {
                                    integratedFolderNames += ",";
                                }
                                integratedFolderNames += cTempIntFolder.DirectoryName;
                            }
                        }
                        sb.Append("<input type=\"hidden\" id=\"integrated_fol_names\" value=\"").Append(integratedFolderNames).Append("\" />").Append(Environment.NewLine);
                    }
                    ektronPageHolder.InnerHtml = sb.ToString();
                }
            }
		}

		protected void SetServerJSVariables()
		{
			ltr_dirRequired.Text = gtMess.GetMessage("alt Directory is required for integrated search folder definition");
			ltr_nonChar.Text = gtMess.GetMessage("alt Directory name should not contain any of the following characters");
			ltr_pwdCnfrm.Text = gtMess.GetMessage("js: alert user cannot confirm password");
			ltr_nameInUse.Text = gtMess.GetMessage("js: alert name in use").Replace("\'", "\\\'");
			ltr_confmMoment.Text = gtMess.GetMessage("js: confirm few moments");
			LoadingImg.Text = gtMess.GetMessage("one moment msg");
		}
		protected void RegisterResources()
		{
			Ektron.Cms.API.Css.RegisterCss(this, Ektron.Cms.API.Css.ManagedStyleSheet.EktronWorkareaCss);
			Ektron.Cms.API.Css.RegisterCss(this, Ektron.Cms.API.Css.ManagedStyleSheet.EktronModalCss);
			Ektron.Cms.API.Css.RegisterCss(this, Ektron.Cms.API.Css.ManagedStyleSheet.EktronWorkareaIeCss, Ektron.Cms.API.Css.BrowserTarget.AllIE);
			Ektron.Cms.API.Css.RegisterCss(this, Ektron.Cms.API.Css.ManagedStyleSheet.EktronFixedPositionToolbarCss);
			
			Ektron.Cms.API.JS.RegisterJS(this, Ektron.Cms.API.JS.ManagedScript.EktronJS);
			Ektron.Cms.API.JS.RegisterJS(this, Ektron.Cms.API.JS.ManagedScript.EktronModalJS);
			Ektron.Cms.API.JS.RegisterJS(this, Ektron.Cms.API.JS.ManagedScript.EktronWorkareaJS);
			Ektron.Cms.API.JS.RegisterJS(this, Ektron.Cms.API.JS.ManagedScript.EktronJFunctJS);
			Ektron.Cms.API.JS.RegisterJS(this, this.m_refContentApi.AppPath + "java/searchfuncsupport.js", "EktronSearchFuncSupportJS");
			Ektron.Cms.API.JS.RegisterJS(this, Ektron.Cms.API.JS.ManagedScript.EktronToolBarRollJS);
			
		}
        private static string EnsureTrailingSlash(string stringThatNeedsTrailingSlash)
        {
            stringThatNeedsTrailingSlash = stringThatNeedsTrailingSlash.Replace("\\", "/");
            if (!stringThatNeedsTrailingSlash.EndsWith("/"))
            {
                return stringThatNeedsTrailingSlash + "/";
            }
            else
            {
                return stringThatNeedsTrailingSlash;
            }
        }
	}