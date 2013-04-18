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
//using Ektron.Cms.Common.EkConstants;
using Ektron.Cms.Common;
	public partial class librarytoolbar : System.Web.UI.UserControl
	{
		
		
		#region  Web Form Designer Generated Code
		
		
		private EkMessageHelper m_refMsg;
		private StyleHelper m_refStyle = new StyleHelper();
		private string m_strPageAction = "";
		private int m_intLanguage = 0;
		private string m_strAppImgPath = "";
		private FolderData m_folderdata = null;
		private string m_strLibType = "";
		private PermissionData m_security = null;
		private long m_intFolderId = -1;
		private string m_strOperation = "";
		private LibraryData m_libdata = null;
		private object m_LoadBalanceInfo = null;
		private int m_intContentType = -1;
		private ContentAPI _ContentAPI = new Ektron.Cms.ContentAPI();
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
		public string Operation
		{
			get
			{
				return (m_strOperation);
			}
			set
			{
				m_strOperation = value;
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
		public int ContentLanguage
		{
			get
			{
				return (m_intLanguage);
			}
			set
			{
				m_intLanguage = value;
			}
		}
		public long FolderId
		{
			get
			{
				return (m_intFolderId);
			}
			set
			{
				m_intFolderId = value;
			}
		}
		public FolderData FolderInfo
		{
			get
			{
				return (m_folderdata);
			}
			set
			{
				m_folderdata = value;
			}
		}
		public string LibType
		{
			get
			{
				return (m_strLibType);
			}
			set
			{
				m_strLibType = value;
			}
		}
		public PermissionData SecurityInfo
		{
			get
			{
				return (m_security);
			}
			set
			{
				m_security = value;
			}
		}
		public LibraryData LibraryInfo
		{
			get
			{
				return (m_libdata);
			}
			set
			{
				m_libdata = value;
			}
		}
		public object LoadBalanceInfo
		{
			get
			{
				return (m_LoadBalanceInfo);
			}
			set
			{
				m_LoadBalanceInfo = value;
			}
		}
		public int ContentType
		{
			get
			{
				return (m_intContentType);
			}
			set
			{
				m_intContentType = value;
			}
		}
		#endregion
		
		private void Page_Load(System.Object sender, System.EventArgs e)
		{
			StyleHelper m_refStyle = new StyleHelper();
			m_refMsg = (new CommonApi()).EkMsgRef;
			RegisterResources();
			if (PageAction == "viewlibrarycategory")
			{
				// txtTitleBar.InnerHtml = m_refStyle.GetTitleBar(m_refMsg.GetMessage("library folder title bar") & " """ & FolderInfo.Name & """")
				// ViewLibraryCategoryToolBar()
				divTitleBar.InnerHtml = m_refStyle.GetTitleBar((string) (m_refMsg.GetMessage("library folder title bar") + " \"" + FolderInfo.Name + "\\" + LibType + "\""));
				ViewLibraryByCategoryToolBar();
			}
			else if (PageAction == "viewlibrarybycategory")
			{
				divTitleBar.InnerHtml = m_refStyle.GetTitleBar((string) (m_refMsg.GetMessage("library folder title bar") + " \"" + FolderInfo.Name + "\\" + LibType + "\""));
				ViewLibraryByCategoryToolBar();
			}
			else if (PageAction == "updateqlinktemplatebycategory")
			{
				divTitleBar.InnerHtml = m_refStyle.GetTitleBar((string) (m_refMsg.GetMessage("Update URL Link Template for") + " \"" + FolderInfo.Name + "\\" + LibType + "\""));
				UpdateQLinkTemplateByCategoryToolBar();
			}
			else if (PageAction == "addlibraryitem")
			{
				if (Operation == "overwrite")
				{
					divTitleBar.InnerHtml = m_refStyle.GetTitleBar((string) (m_refMsg.GetMessage("overwrite folder title bar") + " \"" + FolderInfo.Name + "\\" + LibType + "\""));
				}
				else
				{
					divTitleBar.InnerHtml = m_refStyle.GetTitleBar((string) (m_refMsg.GetMessage("add to lib folder title bar") + " \"" + FolderInfo.Name + "\\" + LibType + "\""));
				}
				AddLibraryItemToolBar();
			}
			else if (PageAction == "viewlibraryitem")
			{
				divTitleBar.InnerHtml = m_refStyle.GetTitleBar((string) (m_refMsg.GetMessage("view lib folder title bar") + " \"" + FolderInfo.Name + "\\" + LibType + "\""));
				ViewLibraryItemToolBar();
			}
			else if (PageAction == "deletelibraryitem")
			{
				divTitleBar.InnerHtml = m_refStyle.GetTitleBar((string) (m_refMsg.GetMessage("delete lib folder title bar") + " \"" + FolderInfo.Name + "\\" + LibType + "\""));
				DeleteLibraryItemToolBar();
			}
			else if (PageAction == "editlibraryitem")
			{
				divTitleBar.InnerHtml = m_refStyle.GetTitleBar((string) (m_refMsg.GetMessage("edit lib folder title bar") + " \"" + FolderInfo.Name + "\\" + LibType + "\""));
				EditLibraryItemToolBar();
			}
			else if (PageAction == "viewlibrarysettings")
			{
				divTitleBar.InnerHtml = m_refStyle.GetTitleBar(m_refMsg.GetMessage("library page title"));
				ViewLibrarySettingsToolBar();
				//ElseIf (PageAction = "viewloadbalance") Then
				//    divTitleBar.InnerHtml = m_refStyle.GetTitleBar(m_refMsg.GetMessage("view lb settings"))
				//    ViewLoadBalanceToolBar()
				//ElseIf (PageAction = "addloadbalance") Then
				//    divTitleBar.InnerHtml = m_refStyle.GetTitleBar(m_refMsg.GetMessage("add lb path msg"))
				//    AddLoadBalanceToolBar()
				//ElseIf (PageAction = "editloadbalancesettings") Then
				//    divTitleBar.InnerHtml = m_refStyle.GetTitleBar(m_refMsg.GetMessage("edit lb path msg"))
				//    EditLoadBalanceSettingsToolBar()
				//ElseIf (PageAction = "removeloadbalance") Then
				//    divTitleBar.InnerHtml = m_refStyle.GetTitleBar(m_refMsg.GetMessage("remove lb path msg"))
				//    RemoveLoadBalanceToolBar()
			}
			else if (PageAction == "editlibrarysettings")
			{
				divTitleBar.InnerHtml = m_refStyle.GetTitleBar(m_refMsg.GetMessage("library page title"));
				EditLibrarySettingsToolBar();
			}
		}
		private void EditLibrarySettingsToolBar()
		{
			System.Text.StringBuilder result = new System.Text.StringBuilder();
			result.Append("<table><tr>" + "\r\n");
			result.Append(m_refStyle.GetButtonEventsWCaption(_ContentAPI.AppPath + "images/ui/icons/back.png", (string)("library.aspx?LangType=" + ContentLanguage + "&action=ViewLibrarySettings&id=" + Request.QueryString["id"]), m_refMsg.GetMessage("alt back button text"), m_refMsg.GetMessage("btn back"), "", StyleHelper.BackButtonCssClass, true));
			result.Append(m_refStyle.GetButtonEventsWCaption(_ContentAPI.AppPath + "images/ui/icons/save.png", "#", m_refMsg.GetMessage("alt update button text (library settings)"), m_refMsg.GetMessage("btn update"), "Onclick=\"javascript:return SubmitForm(\'LibraryItem\', \'CheckLibSettings()\', false);\"", StyleHelper.SaveButtonCssClass, true));
			result.Append(StyleHelper.ActionBarDivider); 
			result.Append("<td>");
			result.Append(m_refStyle.GetHelpButton(m_strPageAction, ""));
			result.Append("</td>");
			result.Append("</tr></table>");
			divToolBar.InnerHtml = result.ToString();
		}
		private void RemoveLoadBalanceToolBar()
		{
			System.Text.StringBuilder result = new System.Text.StringBuilder();
			result.Append("<table><tr>" + "\r\n");
			result.Append(m_refStyle.GetButtonEventsWCaption(_ContentAPI.AppPath + "images/ui/icons/back.png", (string)("library.aspx?LangType=" + ContentLanguage + "&action=ViewLoadBalance&id=" + Request.QueryString["id"]), m_refMsg.GetMessage("alt back button text"), m_refMsg.GetMessage("btn back"), "", StyleHelper.BackButtonCssClass, true));
			if (!(LoadBalanceInfo == null))
			{
				result.Append(m_refStyle.GetButtonEventsWCaption(_ContentAPI.AppPath + "images/ui/icons/remove.png", "#", m_refMsg.GetMessage("alt remove button text (lbpaths)"), m_refMsg.GetMessage("btn minus"), "Onclick=\"javascript:return SubmitForm(\'LibraryItem\', \'VerifyLbPathDeletion()\', false);\"", StyleHelper.SaveButtonCssClass, true));
			}
			result.Append(StyleHelper.ActionBarDivider);
			result.Append("<td>");
			result.Append(m_refStyle.GetHelpButton(m_strPageAction, ""));
			result.Append("</td>");
			result.Append("</tr></table>");
			divToolBar.InnerHtml = result.ToString();
		}
		private void EditLoadBalanceSettingsToolBar()
		{
			System.Text.StringBuilder result = new System.Text.StringBuilder();
			result.Append("<table><tr>" + "\r\n");
			result.Append(m_refStyle.GetButtonEventsWCaption(_ContentAPI.AppPath + "images/ui/icons/back.png", (string)("library.aspx?LangType=" + ContentLanguage + "&action=ViewLoadBalance&id=" + Request.QueryString["id"]), m_refMsg.GetMessage("alt back button text"), m_refMsg.GetMessage("btn back"), "", StyleHelper.BackButtonCssClass, true));
			result.Append(m_refStyle.GetButtonEventsWCaption(_ContentAPI.AppPath + "images/ui/icons/save.png", "#", m_refMsg.GetMessage("alt update button text (lbpath)"), m_refMsg.GetMessage("btn update"), "Onclick=\"javascript:return SubmitForm(\'LibraryItem\', \'VerifyLoadBalancePath()\', false);\"", StyleHelper.SaveButtonCssClass, true));
			result.Append(StyleHelper.ActionBarDivider); 
			result.Append("<td>");
			result.Append(m_refStyle.GetHelpButton(m_strPageAction, ""));
			result.Append("</td>");
			result.Append("</tr></table>");
			divToolBar.InnerHtml = result.ToString();
		}
		private void AddLoadBalanceToolBar()
		{
			System.Text.StringBuilder result = new System.Text.StringBuilder();
			result.Append("<table><tr>" + "\r\n");
			result.Append(m_refStyle.GetButtonEventsWCaption(_ContentAPI.AppPath + "images/ui/icons/back.png", (string)("library.aspx?LangType=" + ContentLanguage + "&action=ViewLoadBalance&id=" + Request.QueryString["id"]), m_refMsg.GetMessage("alt back button text"), m_refMsg.GetMessage("btn back"), "", StyleHelper.BackButtonCssClass, true));
			result.Append(m_refStyle.GetButtonEventsWCaption(_ContentAPI.AppPath + "images/ui/icons/save.png", "#", m_refMsg.GetMessage("alt add button text (lbpath)"), m_refMsg.GetMessage("btn save"), "Onclick=\"javascript:return SubmitForm(\'LibraryItem\', \'VerifyLoadBalancePath()\', false);\"", StyleHelper.SaveButtonCssClass, true));
			result.Append(StyleHelper.ActionBarDivider); 
			result.Append("<td>");
			result.Append(m_refStyle.GetHelpButton(m_strPageAction, ""));
			result.Append("</td>");
			result.Append("</tr></table>");
			divToolBar.InnerHtml = result.ToString();
		}
		private void ViewLoadBalanceToolBar()
		{
			System.Text.StringBuilder result = new System.Text.StringBuilder();
			result.Append("<table><tr>" + "\r\n");
			result.Append(m_refStyle.GetButtonEventsWCaption(_ContentAPI.AppPath + "images/ui/icons/back.png", (string)("library.aspx?LangType=" + ContentLanguage + "&action=ViewLibrarySettings&id=" + Request.QueryString["id"]), m_refMsg.GetMessage("alt back button text"), m_refMsg.GetMessage("btn back"), "", StyleHelper.BackButtonCssClass, true));
			result.Append(m_refStyle.GetButtonEventsWCaption(_ContentAPI.AppPath + "images/ui/icons/add.png", (string)("library.aspx?LangType=" + ContentLanguage + "&action=AddLoadBalance&id=" + Request.QueryString["id"]), m_refMsg.GetMessage("alt add button text (lbpath)"), m_refMsg.GetMessage("btn add"), "", StyleHelper.AddButtonCssClass, true));
			if (!(LoadBalanceInfo == null))
			{
				result.Append(m_refStyle.GetButtonEventsWCaption(_ContentAPI.AppPath + "images/ui/icons/remove.png", (string)("library.aspx?LangType=" + ContentLanguage + "&action=RemoveLoadBalance&id=" + Request.QueryString["id"]), m_refMsg.GetMessage("alt remove button text (lbpath)"), m_refMsg.GetMessage("btn minus"), "", StyleHelper.RemoveButtonCssClass));
			}
			result.Append(StyleHelper.ActionBarDivider);
			result.Append("<td>");
			result.Append(m_refStyle.GetHelpButton(m_strPageAction, ""));
			result.Append("</td>");
			result.Append("</tr></table>");
			divToolBar.InnerHtml = result.ToString();
		}
		
		private void ViewLibrarySettingsToolBar()
		{
			System.Text.StringBuilder result = new System.Text.StringBuilder();
			result.Append("<table><tr>" + "\r\n");

			result.Append(m_refStyle.GetButtonEventsWCaption(_ContentAPI.AppPath + "images/ui/icons/back.png", (string)("library.aspx?LangType=" + ContentLanguage + "&action=ViewLibraryByCategory&id=" + Request.QueryString["id"]), m_refMsg.GetMessage("alt back button text"), m_refMsg.GetMessage("btn back"), "", StyleHelper.BackButtonCssClass, true));

			if (SecurityInfo.IsAdmin)
			{
				if (System.Convert.ToInt32(Request.QueryString["id"]) == 0)
				{
					result.Append(m_refStyle.GetButtonEventsWCaption(_ContentAPI.AppPath + "images/ui/icons/contentEdit.png", (string)("library.aspx?LangType=" + ContentLanguage + "&action=EditLibrarySettings&id=" + Request.QueryString["id"]), m_refMsg.GetMessage("alt edit button text (library settings)"), m_refMsg.GetMessage("btn edit"), "", StyleHelper.EditButtonCssClass, true));
					//result.Append(m_refStyle.GetButtonEventsWCaption(_ContentApi.AppPath & "images/ui/icons/loadBalance.png", "library.aspx?LangType=" & ContentLanguage & "&action=ViewLoadBalance&id=" & Request.QueryString("id"), m_refMsg.GetMessage("alt load balance button text"), m_refMsg.GetMessage("btn load balance"), ""))
				}
				
			}
			result.Append(StyleHelper.ActionBarDivider);
			result.Append("<td>");
			result.Append(m_refStyle.GetHelpButton(m_strPageAction, ""));
			result.Append("</td>");
			result.Append("</tr></table>");
			divToolBar.InnerHtml = result.ToString();
		}
		private void EditLibraryItemToolBar()
		{
			System.Text.StringBuilder result = new System.Text.StringBuilder();
			result.Append("<table><tr>" + "\r\n");
			result.Append(m_refStyle.GetButtonEventsWCaption(_ContentAPI.AppPath + "images/ui/icons/back.png", "library.aspx?LangType=" + ContentLanguage + "&action=ViewLibraryItem&id=" + Request.QueryString["id"] + "&parent_id=" + FolderId + "", m_refMsg.GetMessage("alt back button text"), m_refMsg.GetMessage("btn back"), "", StyleHelper.BackButtonCssClass, true));
			if ((SecurityInfo.CanAddToImageLib && LibType == "images") || (SecurityInfo.CanAddToFileLib && LibType == "files") || (SecurityInfo.CanAddToHyperlinkLib && LibType == "hyperlinks") || (SecurityInfo.CanAddToQuicklinkLib && (LibType == "quicklinks" || LibType == "forms")))
			{
				result.Append(m_refStyle.GetButtonEventsWCaption(_ContentAPI.AppPath + "images/ui/icons/save.png", "#", m_refMsg.GetMessage("alt update button text (library)"), m_refMsg.GetMessage("btn update"), "Onclick=\"javascript:return SubmitForm(\'LibraryItem\', \'CheckLibraryForm(\\\'" + LibType + "\\\')\', true);\"", StyleHelper.SaveButtonCssClass, true));
			}
			result.Append(StyleHelper.ActionBarDivider);
			result.Append("<td>");
			result.Append(m_refStyle.GetHelpButton(m_strPageAction, ""));
			result.Append("</td>");
			result.Append("</tr></table>");
			divToolBar.InnerHtml = result.ToString();
		}
		private void DeleteLibraryItemToolBar()
		{
			System.Text.StringBuilder result = new System.Text.StringBuilder();
			result.Append("<table><tr>" + "\r\n");
			result.Append(m_refStyle.GetButtonEventsWCaption(_ContentAPI.AppPath + "images/ui/icons/back.png", "library.aspx?LangType=" + ContentLanguage + "&action=ViewLibraryItem&id=" + Request.QueryString["item_id"] + "&parent_id=" + FolderId + "", m_refMsg.GetMessage("alt back button text"), m_refMsg.GetMessage("btn back"), "", StyleHelper.BackButtonCssClass, true));
			if ((SecurityInfo.CanAddToImageLib && LibType == "images") || (SecurityInfo.CanAddToFileLib && LibType == "files") || (SecurityInfo.CanAddToHyperlinkLib && LibType == "hyperlinks") || (SecurityInfo.CanAddToQuicklinkLib && (LibType == "quicklinks" || LibType == "forms")))
			{
				result.Append(m_refStyle.GetButtonEventsWCaption(_ContentAPI.AppPath + "images/ui/icons/delete.png", "#", m_refMsg.GetMessage("alt delete button text (library)"), m_refMsg.GetMessage("btn delete"), "Onclick=\"javascript:return SubmitForm(\'LibraryItem\',\'ConfirmLibraryDelete(\\\'LibraryItem\\\', \\\'remove\\\')\', false);\"", StyleHelper.DeleteButtonCssClass, true));
			}
			result.Append(StyleHelper.ActionBarDivider);
			result.Append("<td>");
			result.Append(m_refStyle.GetHelpButton(m_strPageAction, ""));
			result.Append("</td>");
			result.Append("</tr></table>");
			divToolBar.InnerHtml = result.ToString();
		}
		private void ViewLibraryItemToolBar()
		{
			System.Text.StringBuilder result = new System.Text.StringBuilder();
			result.Append("<table><tr>" + "\r\n");

			if (Request.QueryString["backpage"] == "history")
			{
				result.Append(m_refStyle.GetButtonEventsWCaption(_ContentAPI.AppPath + "images/UI/Icons/back.png", "javascript:history.back()", m_refMsg.GetMessage("alt back button text"), m_refMsg.GetMessage("btn back"), "", StyleHelper.BackButtonCssClass, true));
			}
			else
			{
				result.Append(m_refStyle.GetButtonEventsWCaption(_ContentAPI.AppPath + "images/UI/Icons/back.png", "library.aspx?LangType=" + ContentLanguage + "&action=ViewLibraryByCategory&id=" + FolderId + "&type=" + LibraryInfo.Type + "", m_refMsg.GetMessage("alt back button text"), m_refMsg.GetMessage("btn back"), "", StyleHelper.BackButtonCssClass, true));
			}

			bool primaryCssApplied = false;

			if (! Ektron.Cms.Common.EkConstants.IsAssetContentType(ContentType, true))
			{
				if ((SecurityInfo.CanAddToImageLib && LibraryInfo.Type == "images") || SecurityInfo.CanDelete || (SecurityInfo.CanAddToFileLib && LibraryInfo.Type == "files") || (SecurityInfo.CanAddToHyperlinkLib && LibraryInfo.Type == "hyperlinks") || (SecurityInfo.CanAddToQuicklinkLib && LibraryInfo.Type == "quicklinks") || (SecurityInfo.CanAddToQuicklinkLib && LibraryInfo.Type == "forms"))
				{
					result.Append(m_refStyle.GetButtonEventsWCaption(_ContentAPI.AppPath + "images/ui/icons/contentEdit.png", "library.aspx?LangType=" + ContentLanguage + "&action=EditLibraryItem&id=" + Request.QueryString["id"] + "&parent_id=" + FolderId + "", m_refMsg.GetMessage("alt edit button text (library)"), m_refMsg.GetMessage("btn edit"), "", StyleHelper.EditButtonCssClass, !primaryCssApplied));

					primaryCssApplied = true;
				}
			}

            result.Append(m_refStyle.GetButtonEventsWCaption(_ContentAPI.AppPath + "images/UI/Icons/linkSearch.png", (string)("isearch.aspx?LangType=" + ContentLanguage + "&action=dofindcontent&folderid=0&" + (LibraryInfo.Type == "forms" ? "form_id" : "content_id") + "=" + LibraryInfo.ContentId + ((!Ektron.Cms.Common.EkConstants.IsAssetContentType(ContentType, true)) ? ("&libpath=" + EkFunctions.UrlEncode(LibraryInfo.FileName)) : ("&asset_name=" + Server.UrlEncode(LibraryInfo.FileName))) + "&libtype=" + LibraryInfo.Type), m_refMsg.GetMessage("alt Check for content that is linked to this"), m_refMsg.GetMessage("btn link search"), "", StyleHelper.SearchButtonCssClass, !primaryCssApplied));

			primaryCssApplied = true;

			if (! Ektron.Cms.Common.EkConstants.IsAssetContentType(ContentType, true))
			{
				if ((SecurityInfo.CanOverwriteLib) && ((LibraryInfo.Type == "files") || (LibraryInfo.Type == "images")))
				{
					result.Append(m_refStyle.GetButtonEventsWCaption(_ContentAPI.AppPath + "images/UI/Icons/contentOverwrite.png", (string) ("library.aspx?LangType=" + ContentLanguage + "&action=AddLibraryItem&id=" + Request.QueryString["id"] + "&folder=" + FolderId + "&operation=overwrite&type=" + LibraryInfo.Type), m_refMsg.GetMessage("alt overwrite button text wa library"), m_refMsg.GetMessage("btn overwrite"), "", StyleHelper.OverwriteButtonCssClass));
				}
				//result.Append(m_refStyle.GetButtonEventsWCaption(_ContentApi.AppPath & "images/ui/icons/linkSearch.png", "isearch.aspx?LangType=" & ContentLanguage & "&action=dofindcontent&folderid=0&content_id=" & LibraryInfo.ContentId & "&libpath=" & EkFunctions.UrlEncode(LibraryInfo.FileName) & "&libtype=" & LibraryInfo.Type, "Check for content that is linked to this", m_refMsg.GetMessage("btn link search"), ""))
				//In order for a user to delete a library item, apart from Library Read Only he should have Add Images or Add Files or Add Hyperlinks checked in folder permission. 
				if (SecurityInfo.CanDelete && (SecurityInfo.IsReadOnlyLib && (SecurityInfo.CanAddToImageLib || SecurityInfo.CanAddToFileLib || SecurityInfo.CanAddToHyperlinkLib)))
				{
					if (LibType == "images" || LibType == "files")
					{
						result.Append(m_refStyle.GetButtonEventsWCaption(_ContentAPI.AppPath + "images/UI/Icons/delete.png", "library.aspx?LangType=" + ContentLanguage + "&action=DeleteLibraryItem&item_id=" + Request.QueryString["id"] + "&parent_id=" + FolderId + "&type=" + LibraryInfo.Type + "", m_refMsg.GetMessage("alt delete button text (library)"), m_refMsg.GetMessage("btn delete"), "", StyleHelper.DeleteButtonCssClass));
					}
					else
					{
						if (m_intContentType == 7)
						{
							result.Append(m_refStyle.GetButtonEventsWCaption(_ContentAPI.AppPath + "images/UI/Icons/delete.png", "library.aspx?LangType=" + ContentLanguage + "&action=RemoveLibraryItem&item_id=" + Request.QueryString["id"] + "&parent_id=" + FolderId + "&type=" + LibraryInfo.Type + "", m_refMsg.GetMessage("alt delete button text (library)"), m_refMsg.GetMessage("btn delete"), "OnClick=\"javascript:return ConfirmLibraryDelete();\"", StyleHelper.DeleteButtonCssClass));
						}
					}
				}
			}
			result.Append(StyleHelper.ActionBarDivider);
			result.Append("<td>");
			result.Append(m_refStyle.GetHelpButton(m_strPageAction, ""));
			result.Append("</td>");
			result.Append("</tr></table>");
			divToolBar.InnerHtml = result.ToString();
		}
		private void AddLibraryItemToolBar()
		{
			System.Text.StringBuilder result = new System.Text.StringBuilder();
			string strModifier = "";
			
			result.Append("<table><tr>" + "\r\n");
			if ((SecurityInfo.CanAddToImageLib && LibType == "images") || (SecurityInfo.CanAddToFileLib && LibType == "files") || (SecurityInfo.CanAddToHyperlinkLib && LibType == "hyperlinks") || (SecurityInfo.CanAddToQuicklinkLib && (LibType == "quicklinks" || LibType == "forms")) || (SecurityInfo.CanOverwriteLib && ("overwrite" == Operation)))
				{
				// result.Append(m_refStyle.GetButtonEventsWCaption(AppImgPath & "btn_preview-nm.gif", "preview", m_refMsg.GetMessage("alt preview button text (library)"), m_refMsg.GetMessage("btn preview"), "onClick=""javascript:return CheckPreview('" & LibType & "', this.href, '" & Request.QueryString("operation") & "');"" target=""Preview"""))
				if (Operation == "overwrite")
				{
					strModifier = "overwrite";
					result.Append(m_refStyle.GetButtonEventsWCaption(_ContentAPI.AppPath + "images/ui/icons/back.png", "library.aspx?LangType=" + ContentLanguage + "&action=ViewLibraryItem&id=" + Request.QueryString["id"] + "&parent_id=" + FolderId + "&type=" + LibType + "", m_refMsg.GetMessage("alt back button text"), m_refMsg.GetMessage("btn back"), "", StyleHelper.BackButtonCssClass, true));
					result.Append(m_refStyle.GetButtonEventsWCaption(_ContentAPI.AppPath + "images/ui/icons/save.png", "#", m_refMsg.GetMessage("alt overwrite button text wa library"), m_refMsg.GetMessage("btn update"), "Onclick=\"javascript:return SubmitForm(\'LibraryItem\', \'CheckOverwriteSubmission(\\\'" + LibType + "\\\')\', true);\"", StyleHelper.SaveButtonCssClass, true));
				}
				else
				{
					result.Append(m_refStyle.GetButtonEventsWCaption(_ContentAPI.AppPath + "images/ui/icons/back.png", "library.aspx?LangType=" + ContentLanguage + "&action=ViewLibraryByCategory&id=" + FolderId + "&type=" + LibType + "", m_refMsg.GetMessage("alt back button text"), m_refMsg.GetMessage("btn back"), "", StyleHelper.BackButtonCssClass, true));
					result.Append(m_refStyle.GetButtonEventsWCaption(_ContentAPI.AppPath + "images/ui/icons/save.png", "#", m_refMsg.GetMessage("alt add button text (library2)"), m_refMsg.GetMessage("btn save"), "Onclick=\"javascript:return SubmitForm(\'LibraryItem\', \'CheckAddSubmission(\\\'" + LibType + "\\\')\', true);\"", StyleHelper.SaveButtonCssClass, true));
				}
			}
			result.Append(StyleHelper.ActionBarDivider);
			result.Append("<td>");
			result.Append(m_refStyle.GetHelpButton(m_strPageAction + strModifier, ""));
			result.Append("</td>");
			result.Append("</tr></table>");
			divToolBar.InnerHtml = result.ToString();
		}
		private void UpdateQLinkTemplateByCategoryToolBar()
		{
			System.Text.StringBuilder result = new System.Text.StringBuilder();
			result.Append("<table><tr>" + "\r\n");
			result.Append(m_refStyle.GetButtonEventsWCaption(_ContentAPI.AppPath + "images/ui/icons/back.png", (string)("library.aspx?LangType=" + ContentLanguage + "&action=ViewLibraryCategory&id=" + FolderId), m_refMsg.GetMessage("alt back button text"), m_refMsg.GetMessage("btn back"), "", StyleHelper.BackButtonCssClass, true));
			if ((SecurityInfo.CanAddToImageLib && LibType == "images") || (SecurityInfo.CanAddToFileLib && LibType == "files") || (SecurityInfo.CanAddToHyperlinkLib && LibType == "hyperlinks") || (SecurityInfo.CanAddToQuicklinkLib && LibType == "quicklinks"))
			{
				result.Append(m_refStyle.GetButtonEventsWCaption(AppImgPath + "btn_UpdateQuicklink-nm.gif", "#", m_refMsg.GetMessage("btn update quicklink"), m_refMsg.GetMessage("btn update quicklink"), "OnClick=\"javascript:checkUpdateForm();\"", StyleHelper.UpdateQuicklinkButtonCssClass, true));
			}
			result.Append(StyleHelper.ActionBarDivider);
			result.Append("<td>");
			result.Append(m_refStyle.GetHelpButton(m_strPageAction, ""));
			result.Append("</td>");
			result.Append("</tr></table>");
			divToolBar.InnerHtml = result.ToString();
		}
		private void ViewLibraryCategoryToolBar()
		{
			System.Text.StringBuilder result = new System.Text.StringBuilder();
			result.Append("<table><tr>" + "\r\n");
			result.Append(m_refStyle.GetButtonEventsWCaption(_ContentAPI.AppPath + "images/ui/icons/back.png", (string)("library.aspx?LangType=" + ContentLanguage + "&action=ViewLibraryCategory&id=" + FolderInfo.ParentId), m_refMsg.GetMessage("alt back button text"), m_refMsg.GetMessage("btn back"), "", StyleHelper.BackButtonCssClass, true));
			result.Append(m_refStyle.GetButtonEventsWCaption(_ContentAPI.AppPath + "images/UI/Icons/magnifier.png", (string)("isearch.aspx?LangType=" + ContentLanguage + "&action=showLibdlg&folderid=" + FolderInfo.Id), m_refMsg.GetMessage("alt Search for content by title"), m_refMsg.GetMessage("btn search"), "", StyleHelper.SearchButtonCssClass, true));
			result.Append(m_refStyle.GetButtonEventsWCaption(_ContentAPI.AppPath + "images/ui/icons/properties.png", (string)("library.aspx?LangType=" + ContentLanguage + "&action=ViewLibrarySettings&id=" + FolderInfo.Id), m_refMsg.GetMessage("alt library properties button text"), m_refMsg.GetMessage("btn properties"), "", StyleHelper.ViewPropertiesButtonCssClass));
			result.Append(StyleHelper.ActionBarDivider); 
			result.Append("<td>");
			result.Append(m_refStyle.GetHelpButton(m_strPageAction, ""));
			result.Append("</td>");
			result.Append("</tr></table>");
			divToolBar.InnerHtml = result.ToString();
		}
		private void ViewLibraryByCategoryToolBar()
		{
			System.Text.StringBuilder result = new System.Text.StringBuilder();
			result.Append("<table><tr>" + "\r\n");
			
			if (PageAction != "viewlibrarybycategory")
			{
				result.Append(m_refStyle.GetButtonEventsWCaption(_ContentAPI.AppPath + "images/ui/icons/back.png", (string)("library.aspx?LangType=" + ContentLanguage + "&action=ViewLibraryCategory&id=" + FolderId), m_refMsg.GetMessage("alt back button text"), m_refMsg.GetMessage("btn back"), "", StyleHelper.BackButtonCssClass, true));
			}

			bool primaryCssApplied = false;

			if ((SecurityInfo.CanAddToImageLib && LibType == "images") || (SecurityInfo.CanAddToFileLib && LibType == "files") || (SecurityInfo.CanAddToHyperlinkLib && LibType == "hyperlinks") || (SecurityInfo.CanAddToQuicklinkLib) && (LibType == "quicklinks" || LibType == "Forms"))
			{
				if (ContentLanguage != Ektron.Cms.Common.EkConstants.ALL_CONTENT_LANGUAGES)
				{
					result.Append(m_refStyle.GetButtonEventsWCaption(_ContentAPI.AppPath + "images/ui/icons/add.png", "library.aspx?LangType=" + ContentLanguage + "&action=AddLibraryItem&folder=" + FolderId + "&type=" + LibType + "", m_refMsg.GetMessage("alt add button text (library)"), m_refMsg.GetMessage("btn add"), "", StyleHelper.AddButtonCssClass, !primaryCssApplied));

					primaryCssApplied = true;
				}

				if ((LibType == "quicklinks") || (LibType == "Forms"))
				{
					result.Append(m_refStyle.GetButtonEventsWCaption(AppImgPath + "btn_UpdateQuicklink-nm.gif", "library.aspx?LangType=" + ContentLanguage + "&action=UpdateQlinkTemplateByCategory&id=" + FolderId + "&type=" + LibType + "", m_refMsg.GetMessage("msg update lnk temp"), m_refMsg.GetMessage("btn update quicklink"), "", StyleHelper.UpdateQuicklinkButtonCssClass, !primaryCssApplied));

					primaryCssApplied = true;
				}
			}

			result.Append(m_refStyle.GetButtonEventsWCaption(_ContentAPI.AppPath + "images/ui/icons/magnifier.png", (string)("isearch.aspx?LangType=" + ContentLanguage + "&action=showLibdlg&folderid=" + FolderInfo.Id), m_refMsg.GetMessage("alt Search for content by title"), m_refMsg.GetMessage("btn search"), "", StyleHelper.SearchButtonCssClass, !primaryCssApplied));

			primaryCssApplied = true;

			if (m_intFolderId == 0)
			{
				result.Append(m_refStyle.GetButtonEventsWCaption(_ContentAPI.AppPath + "images/ui/icons/properties.png", (string) ("library.aspx?LangType=" + ContentLanguage + "&action=ViewLibrarySettings&id=" + FolderInfo.Id), m_refMsg.GetMessage("alt library properties button text"), m_refMsg.GetMessage("btn properties"), "", StyleHelper.ViewPropertiesButtonCssClass));
			}
			if (!string.IsNullOrEmpty(Request.QueryString["type"]))
			{
				LibType = Request.QueryString["type"];
				Session["LibCategory"] = LibType;
			}
			else
			{
				LibType = Session["LibCategory"].ToString();
			}
			
			if (LibType == "")
			{
				LibType = "images";
				Session["LibCategory"] = LibType;
			}
			result.Append("<script language=\"Javascript\">");
			result.Append(" function ChangeLibraryType(SelObj)");
			result.Append("	{");
			result.Append("     document.location.href = \"library.aspx?LangType=" + this.ContentLanguage + "&action=ViewLibraryByCategory&id=" + this.FolderId + "&type=\" + SelObj.value;");
			result.Append("}");
			result.Append("</script>");
			result.Append(StyleHelper.ActionBarDivider);
			result.Append("<td><select onchange=\'ChangeLibraryType(this)\' align=right id=\"LibType\" name=\"LibType\">");
			result.Append("<option value=\"files\" " + SelectItem("files") + ">" + m_refMsg.GetMessage("generic files") + "</option>");
			result.Append("<option value=\"forms\" " + SelectItem("forms") + ">" + m_refMsg.GetMessage("forms button text") + "</option>");
			result.Append("<option value=\"hyperlinks\" " + SelectItem("hyperlinks") + ">" + m_refMsg.GetMessage("generic hyperlinks") + "</option>");
			result.Append("<option value=\"images\" " + SelectItem("images") + ">" + m_refMsg.GetMessage("generic images") + "</option>");
			result.Append("<option value=\"quicklinks\" " + SelectItem("quicklinks") + ">" + m_refMsg.GetMessage("lbl quicklink") + "</option>");
			result.Append("</select></td>");

			result.Append(StyleHelper.ActionBarDivider);
			
			result.Append(m_refStyle.GetShowAllActiveLanguage(true, "", "javascript:SelLanguage(this.value);", Convert.ToString((short) ContentLanguage)));

			result.Append(StyleHelper.ActionBarDivider);

			result.Append("<td>");
			result.Append(m_refStyle.GetHelpButton(m_strPageAction, ""));
			result.Append("</td>");
			result.Append("</tr></table>");
			divToolBar.InnerHtml = result.ToString();
		}
		private string SelectItem(string Value)
		{
			if (LibType == Value)
			{
				return " selected ";
				//ElseIf (LibType = "forms") Then
				//    Return " selected "
				//ElseIf (LibType = "hyperlinks") Then
				//    Return " selected "
				//ElseIf (LibType = "images") Then
				//    Return " selected "
				//ElseIf (LibType = "quicklinks") Then
				//    Return " selected "
			}
			return "";
		}
		private void RegisterResources()
		{
			Ektron.Cms.API.JS.RegisterJS(this, Ektron.Cms.API.JS.ManagedScript.EktronWorkareaHelperJS);
		}
	}
