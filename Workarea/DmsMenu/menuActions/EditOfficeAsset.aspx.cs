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
//using Ektron.Cms.Common.EkConstants;
using Ektron.Cms;
using Ektron.Cms.Common;
using Ektron.Cms.Content;
using System.IO;



	public partial class Workarea_EditOfficeAsset : System.Web.UI.Page
	{
		
		
		private string szdavfolder = "";
		private ContentEditData content_edit_data;
		
		protected void Page_Load(object sender, System.EventArgs e)
		{
			//30257- This document should never be cached.  When content status changes
			//the page is refreshed to show change in status.  This change does
			//not appear when page is cached.
			Response.Cache.SetCacheability(HttpCacheability.NoCache);
			
			long contentid = 0;
			//long folderid = 0;
			int ContentLanguage = -1;
			CommonApi AppUI = new CommonApi();
			Ektron.Cms.ContentAPI content_api = new Ektron.Cms.ContentAPI();
			
			if (! (Request.QueryString["id"] == null))
			{
				contentid = Convert.ToInt64(Request.QueryString["id"]);
				
				if ((!(Request.QueryString["LangType"] == null)) && (int.TryParse(Request.QueryString["LangType"], out ContentLanguage)) && (ContentLanguage > 0))
				{
					if (ContentLanguage == Ektron.Cms.Common.EkConstants.CONTENT_LANGUAGES_UNDEFINED || ContentLanguage == Ektron.Cms.Common.EkConstants.ALL_CONTENT_LANGUAGES)
					{
						ContentLanguage = AppUI.DefaultContentLanguage;
					}
					AppUI.ContentLanguage = ContentLanguage;
					content_api.ContentLanguage = ContentLanguage;
				}
				else
				{
					ContentLanguage = AppUI.DefaultContentLanguage;
				}
				content_edit_data = content_api.GetContentForEditing(contentid);
				if (! (Request.QueryString["executeActiveX"] == null) && Request.QueryString["executeActiveX"] == "true" )
				{
					string strfilename;
					AssetManagement.AssetManagementService assetmanagementService = new AssetManagement.AssetManagementService();
					Ektron.ASM.AssetConfig.AssetData assetData = assetmanagementService.GetAssetData(content_edit_data.AssetData.Id);
					strfilename = (string) (GetFolderPath(content_edit_data.FolderId) + assetData.Handle);
                    string sJS = "";
                    if (Page.Request.UserAgent.ToLower().Contains("firefox"))
                    {
                        sJS = "editDocumentWithProgIDNoUI('" + getFilePath(content_edit_data.FolderId) + assetData.Handle + "', '', 'SharePoint.OpenDocuments', '0', '" + getSiteAddress() + "', '0');";
                        string backURL = content_api.AppPath + Page.Request.QueryString["back_callerpage"] + "?" + Page.Request.QueryString["back_origurl"];
                        sJS += "window.location.href='" + this.Request.UrlReferrer + "'";
                    }else
                    {
                        sJS = "editInMSOffice(\'" + strfilename + "\');";
                    }
					Page.ClientScript.RegisterStartupScript(typeof(Page), "ShowInOffice", sJS.ToString(), true);
				}
			}
		}

        private string getSiteAddress()
        {
            string ret = "";
            if (Page.Request.Url.Host.ToLower() == "localhost")
            {
                ret = Page.Request.Url.Scheme + Uri.SchemeDelimiter + System.Net.Dns.GetHostName();
            }
            else
            {
                ret = Page.Request.Url.Scheme + Uri.SchemeDelimiter + Page.Request.Url.Authority; 
            }
            return ret;
        }

        private string getFilePath(long Id)
        {
            ContentAPI contentAPI = new ContentAPI();
            SiteAPI siteAPI = new SiteAPI();

            szdavfolder = "ekdavroot";

            string sitePath = (string)(siteAPI.SitePath.ToString().TrimEnd(new char[] { '/' }).TrimStart(new char[] { '/' }));
            szdavfolder = (string)(szdavfolder.TrimEnd(new char[] { '/' }).TrimStart(new char[] { '/' }));
            if (Page.Request.Url.Host.ToLower() == "localhost")
            {
                szdavfolder =  "/" + sitePath + "/" + szdavfolder + "_" + siteAPI.UserId + "_" + siteAPI.UniqueId + (((Context.Request.QueryString["LangType"] != null) ? ("_" + Context.Request.QueryString["LangType"].ToString()) : "")) + "/";
            }
            else
            {
                szdavfolder =  "/" + sitePath + "/" + szdavfolder + "_" + siteAPI.UserId + "_" + siteAPI.UniqueId + (((Context.Request.QueryString["LangType"] != null) ? ("_" + Context.Request.QueryString["LangType"].ToString()) : "")) + "/";
            }

            string szFolderPath = contentAPI.EkContentRef.GetFolderPath(Id);
            szFolderPath = szFolderPath.Replace("\\", "/");
            szFolderPath = szFolderPath.TrimStart(new char[] { '/' });
            szFolderPath = szFolderPath.Replace("\\\\", "/");
            if (szFolderPath.Length > 0)
            {
                szFolderPath = szdavfolder + szFolderPath + "/";
            }
            else
            {
                szFolderPath = szdavfolder;
            }

            return szFolderPath;
        }

		public string GetFolderPath(long Id)
		{
			ContentAPI contentAPI = new ContentAPI();
			SiteAPI siteAPI = new SiteAPI();
			
			szdavfolder = "ekdavroot";
			
			string sitePath = (string) (siteAPI.SitePath.ToString().TrimEnd(new char[] {'/'}).TrimStart(new char[] {'/'}));
			szdavfolder = (string) (szdavfolder.TrimEnd(new char[] {'/'}).TrimStart(new char[] {'/'}));
			if (Page.Request.Url.Host.ToLower() == "localhost")
			{
				szdavfolder = Page.Request.Url.Scheme + Uri.SchemeDelimiter + System.Net.Dns.GetHostName() + "/" + sitePath + "/" + szdavfolder + "_" + siteAPI.UserId + "_" + siteAPI.UniqueId + (((Context.Request.QueryString["LangType"] != null) ? ("_" + Context.Request.QueryString["LangType"].ToString()) : "") ) + "/";
			}
			else
			{
				szdavfolder = Page.Request.Url.Scheme + Uri.SchemeDelimiter + Page.Request.Url.Authority + "/" + sitePath + "/" + szdavfolder + "_" + siteAPI.UserId + "_" + siteAPI.UniqueId + (((Context.Request.QueryString["LangType"] != null) ? ("_" + Context.Request.QueryString["LangType"].ToString()) : "") ) + "/";
			}
			
			string szFolderPath = contentAPI.EkContentRef.GetFolderPath(Id);
			szFolderPath = szFolderPath.Replace("\\", "/");
			szFolderPath = szFolderPath.TrimStart(new char[] {'/'});
			szFolderPath = szFolderPath.Replace("\\\\", "/");
			if (szFolderPath.Length > 0)
			{
				szFolderPath = szdavfolder + szFolderPath + "/";
			}
			else
			{
				szFolderPath = szdavfolder;
			}
			
			return szFolderPath;
		}
		
	}
	

