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
public partial class linkit : System.Web.UI.Page
 {
		#region  Web Form Designer Generated Code
		//This call is required by the Web Form Designer.
		[System.Diagnostics.DebuggerStepThrough()]private void InitializeComponent()
		{
		}
		private void Page_Init(System.Object sender, System.EventArgs e)
		{
			//CODEGEN: This method call is required by the Web Form Designer
			//Do not modify it using the code editor.
			InitializeComponent();
		}
		#endregion
		private long folderID = 0;
		private long itemID = 0;
		private long libID = 0;
		private string strLinkIdentifier = "";
		protected ContentAPI m_refContentApi = new ContentAPI();
		private TemplateData templateData;
		private bool bUseQLink = false;
		private void Page_Load(System.Object sender, System.EventArgs e)
		{
			//Put user code to initialize the page here
			Collection cQuickLink = new Collection();
			string RedirectPath = string.Empty;
			string aliasName = string.Empty;
			string zLibPath = string.Empty;
			m_refContentApi = new ContentAPI();
			Ektron.Cms.UrlAliasing.UrlAliasCommonApi _refUrlCommonApi = new Ektron.Cms.UrlAliasing.UrlAliasCommonApi();
			string strNewQuery = Request.ServerVariables["QUERY_STRING"];
			if (! (Request.QueryString["ItemID"] == null))
			{
				itemID =Convert.ToInt64(Request.QueryString["ItemID"]);
				strNewQuery = (string) (strNewQuery.Replace((string) ("ItemID=" + itemID), "").Replace("&&", "&"));
			}
			if (! (Request.QueryString["libID"] == null))
			{
				libID =Convert.ToInt64(Request.QueryString["libID"]);
				strNewQuery = (string) (strNewQuery.Replace((string) ("libID=" + libID), "").Replace("&&", "&"));
			}
			if (! (Request.QueryString["LinkIdentifier"] == null))
			{
				strLinkIdentifier = Request.QueryString["LinkIdentifier"];
				strNewQuery = (string) (strNewQuery.Replace((string) ("LinkIdentifier=" + strLinkIdentifier), "").Replace("&&", "&"));
			}
			if (Request.QueryString["FolderID"] == null)
			{
				bUseQLink = true;
			}
			else
			{
				folderID = Convert.ToInt64(Request.QueryString["FolderID"]);
				strNewQuery = (string) (strNewQuery.Replace((string) ("FolderID=" + folderID), "").Replace("&&", "&"));
			}
			if (strLinkIdentifier == "")
			{
				strLinkIdentifier = "id";
			}
			if (bUseQLink)
			{
				//Use quicklink
				if (strLinkIdentifier != "ekfrm")
				{
					aliasName = _refUrlCommonApi.GetAliasForContent(itemID);
				}
				if (! libID.Equals(null) && libID > 0 && ! (aliasName.Length > 0))
				{
					Ektron.Cms.API.Library zapi = new Ektron.Cms.API.Library();
					aliasName = (string) (zapi.GetLibraryItem(libID).FileName.Replace(m_refContentApi.SitePath, "/"));
				}
				else
				{
                    cQuickLink = (Collection)m_refContentApi.EkLibraryRef.GetLibraryItemByContentID(itemID, _refUrlCommonApi.ContentLanguage,true);
				}
				if ((cQuickLink != null) && cQuickLink.Count >0 && aliasName == string.Empty)
				{
					if (Convert.ToInt32(cQuickLink["LibraryTypeID"]) == 1) //image, do not use the app/sitepath
					{
						if (Convert.ToInt32(cQuickLink["IsPrivate"]) == 0)
						{
							RedirectPath = m_refContentApi.SitePath + "assets/" + Server.HtmlDecode(Convert.ToString(cQuickLink["LibraryFilename"]));
						}
						else
						{
							RedirectPath = m_refContentApi.SitePath + "privateassets/" + Server.HtmlDecode(Convert.ToString(cQuickLink["LibraryFilename"]));
						}
					}
					else if ((cQuickLink["LibraryFilename"].ToString().IndexOf("javascript:void window.open") >= 0) && (cQuickLink["ContentID"].ToString() != ""))
					{
						RedirectPath = m_refContentApi.AppPath + "showcontent.aspx?id=" + itemID;
					}
					else if ((cQuickLink["LibraryFilename"].ToString().ToLower().IndexOf("downloadasset.aspx") >= 0) && (cQuickLink["ContentID"].ToString() != ""))
					{
						RedirectPath = m_refContentApi.AppPath + cQuickLink["LibraryFilename"].ToString();
					}
					else if (cQuickLink["LibraryFilename"].ToString().ToLower().IndexOf("linkit.aspx") < 0)
					{
						// make sure the quicklink doesn't link to this page or it'll be stuck in infinite loop
						RedirectPath = m_refContentApi.SitePath + Server.HtmlDecode(Convert.ToString(cQuickLink["LibraryFilename"]));
					}
				}
				else
				{
					RedirectPath = (string) ((m_refContentApi.SitePath + aliasName).Replace("//", "/")); // If there is an alias it should show if they use linkit as Quicklink.
				}
			}
			if (((RedirectPath == "") && bUseQLink == true) || (bUseQLink == false))
			{
				// see if content has a multitemplate attached to it first
				templateData = m_refContentApi.GetMultiTemplateASPX(itemID);
				if (templateData == null)
				{
					templateData = m_refContentApi.GetTemplatesByFolderId(m_refContentApi.GetJustFolderIdByContentId(itemID));
				}
				if (Convert.ToString(templateData.FileName).IndexOf("?") >= 0)
				{
					RedirectPath = m_refContentApi.SitePath + templateData.FileName + "&" + strLinkIdentifier + "=" + itemID;
				}
				else
				{
					RedirectPath = m_refContentApi.SitePath + templateData.FileName + "?" + strLinkIdentifier + "=" + itemID;
				}
			}
			if (string.IsNullOrEmpty(aliasName)) // Only if there is no alias append the following.
			{
				if (strNewQuery != "" && strNewQuery != "&")
				{
					strNewQuery = strNewQuery.Trim("&".ToCharArray());
					if (RedirectPath.IndexOf("?") >= 0)
					{
						RedirectPath = (string) (RedirectPath + "&" + strNewQuery);
					}
					else
					{
						RedirectPath = RedirectPath + "?" + strNewQuery;
					}
				}
			}
			// domain folder redirect only if RedirectPath is not a full URL
			if (! RedirectPath.StartsWith("http"))
			{
				string domain = m_refContentApi.GetDomainByContentId(itemID);
				if (domain != null&& domain != "" && m_refContentApi.SitePath == "/")
				{
					RedirectPath = (string) ("http://" + domain + "/" + RedirectPath.Substring(1));
				}
				else if (domain != null&& domain != "")
				{
					RedirectPath = (string) ("http://" + domain + "/" + RedirectPath.Replace(m_refContentApi.SitePath, ""));
				}
			}
			if (Page.IsPostBack && (!(cQuickLink == null)) && cQuickLink.Count>0 && (Convert.ToInt32(cQuickLink["LibraryTypeID"]) == 5)) 
			{
                string strUrl = string.Empty;
				strUrl = m_refContentApi.QualifyUrlForServerTransfer(RedirectPath);
				if (strUrl.Length > 0)
				{
					try
					{
						//#16799 - using the redirect to a page, the form data is not posting to specified template.
						//Changed form response redirect to server.transfer to forward form values when possible.
						//source:             CommonApi.vb(, ApplicationApi.vb, ServerControlLibrary / FormBlock / FormBlock.cs, workarea / linkit.aspx.vb)
						//PRB: "View State Is Invalid" Error Message When You Use Server.Transfer
						//http://support.microsoft.com/default.aspx?id=kb;en-us;Q316920
						//Note: I have not seen the problem mentioned in the MS KB. -doug domeny 2005-11-22
						Context.RewritePath(strUrl, false);
						Server.Transfer(strUrl, true);
						return;
					}
					catch (System.Threading.ThreadAbortException)
					{
					   //When you do a Server.Transfer() or Response.Redirect(), if you have code after this statement, it sometimes throws an error.
						return;
					}
					catch (Exception ex)
					{
						if (RedirectPath.IndexOf("?") >= 1)
						{
							RedirectPath += "&";
						}
						else
						{
							RedirectPath += "?";
						}
						RedirectPath += (string)("TransferError=" + EkFunctions.UrlEncode(ex.Message));
					}
				}
			}
			
			Response.Redirect(RedirectPath);
		}
		
	}
