using System.Web.UI.WebControls;
using System.Configuration;
using System.Collections;

using System.Data;
using System.Web.Caching;

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
using Ektron.Cms.Content;
//using Ektron.Cms.Common.EkConstants;
using System.IO;


	public partial class DisplayResult : System.Web.UI.Page
	{
		
		
		protected int m_intContentLanguage = -1;
		protected ContentAPI m_refContApi = new ContentAPI();
		
		
		private void Page_Load(System.Object sender, System.EventArgs e)
		{
			RegisterResources();
		}
		
		public void renderInfo()
		{
			long libId;
			long contId;
			long formId;
			long folderId;
			long contentId=0;
			string strSummary;
			CommonApi m_refAPI = new CommonApi();
			Ektron.Cms.Content.EkContent ekc;
			Ektron.Cms.LibraryData libItem;
			Collection custFldsCol;
			Collection custFld;
			string custFldName;
			long custFldID;
			string metaValue;
			bool searchableMeta;
			Collection contCol;
			
			try
			{
				
				ekc = m_refAPI.EkContentRef;
                long.TryParse(Request.QueryString["libid"], out libId);
                long.TryParse(Request.QueryString["cid"], out contId);
                long.TryParse(Request.QueryString["fid"], out formId);
               
				// Ensure that the language is selected/initialized:
				if (!(Request.QueryString["LangType"] == null))
				{
					if (Request.QueryString["LangType"] != "")
					{
						m_intContentLanguage = Convert.ToInt32(Request.QueryString["LangType"]);
						m_refContApi.SetCookieValue("LastValidLanguageID", m_intContentLanguage.ToString());
					}
					else
					{
						if (m_refContApi.GetCookieValue("LastValidLanguageID") != "")
						{
							m_intContentLanguage = Convert.ToInt32(m_refContApi.GetCookieValue("LastValidLanguageID"));
						}
					}
				}
				else
				{
					if (m_refContApi.GetCookieValue("LastValidLanguageID") != "")
					{
						m_intContentLanguage = Convert.ToInt32(m_refContApi.GetCookieValue("LastValidLanguageID"));
					}
				}
				if (m_intContentLanguage == Ektron.Cms.Common.EkConstants.CONTENT_LANGUAGES_UNDEFINED)
				{
					m_refContApi.ContentLanguage = Ektron.Cms.Common.EkConstants.ALL_CONTENT_LANGUAGES;
				}
				else
				{
					m_refContApi.ContentLanguage = m_intContentLanguage;
				}
				
				if (libId > 0)
				{
					// Show the library item:
					
					// Get the library item object:
					libItem = m_refContApi.GetLibraryItemByID_UnAuth(libId);
					contentId = libItem.ContentId;
					
					// Show the title:
					Response.Write("<h2 class=\"displayresult_libitem_title\" >" + libItem.Title+ "</h2>");
					
					// Show the custom fields (based on the items' folder):
					//folderId = libItem.ParentId INVESTIGATE: The value returned in libItem from GetLibraryItemByID_UnAuth is wrong (as are others, like the flolder name!)...
					folderId = ekc.GetJustFolderIdByContentId(contentId);
					custFldsCol = ekc.GetFieldsByFolder(folderId, m_refContApi.ContentLanguage);
					//    Returns collection from DataIo:
					//        CustomFieldID As Integer
					//        CustomFieldName As String
					//        Required As Integer
					//        Assigned As Integer
					//        OwningFolderID As Integer
					//
					if (custFldsCol.Count > 0)
					{
						Response.Write("<div class=\"displayresult_libitem_metadata\" >");
						foreach (Collection tempLoopVar_custFld in custFldsCol)
						{
							custFld = tempLoopVar_custFld;
							custFldName = custFld["CustomFieldName"].ToString();
                            custFldID = Convert.ToInt64(custFld["CustomFieldID"]);
							// only show searchable custom fields, that are set to allow searching:
							searchableMeta = ekc.IsMetadataSearchableByID(custFldID);
							if (searchableMeta)
							{
								// Get the metadata:
								metaValue = ekc.GetJustMetaDataByContIdMetaId(contentId, custFldID);
								Response.Write("<div class=\"displayresult_libitem_metadataitem\" >");
								Response.Write(custFldName + ": " + metaValue);
								Response.Write("</div>");
							}
						}
						Response.Write("</div>");
					}
					
					// Show the image:
					Response.Write("<div class=\"displayresult_libitem_image\" >");
					Response.Write("<img src=\"" + libItem.FileName+ "\" />");
					Response.Write("</div>");
					
					// Show the Description (content teaser/summary)
					strSummary = m_refContApi.GetJustTeaserByContentId(contentId, libItem.LanguageId);
					Response.Write("<div class=\"displayresult_libitem_summary\" >");
					Response.Write(strSummary);
					Response.Write("</div>");
				}
				else if (contId > 0)
				{
					contCol = ekc.GetContentByIDv2_0(contId);
					folderId = System.Convert.ToInt32(contCol["FolderID"]);
					Response.Write("<h3>" + contCol["ContentTitle"] + "</h3>");
					
					custFldsCol = ekc.GetFieldsByFolder(folderId, m_refContApi.ContentLanguage);
					//    Returns collection from DataIo:
					//        CustomFieldID As Integer
					//        CustomFieldName As String
					//        Required As Integer
					//        Assigned As Integer
					//        OwningFolderID As Integer
					//
					foreach (Collection tempLoopVar_custFld in custFldsCol)
					{
						custFld = tempLoopVar_custFld;
						custFldName = custFld["CustomFieldName"].ToString();
                        custFldID = Convert.ToInt64(custFld["CustomFieldID"]);
						// only show searchable custom fields, that are set to allow searching:
						searchableMeta = ekc.IsMetadataSearchableByID(custFldID);
						if (searchableMeta)
						{
							// Get the metadata:
							metaValue = ekc.GetJustMetaDataByContIdMetaId(contentId, custFldID);
							Response.Write(custFldName + ": " + metaValue + "<br />");
						}
					}
					
					Response.Write("<br />");
					Response.Write(contCol["ContentHtml"]);
				}
				else if (formId > 0)
				{
					contCol = ekc.GetContentByIDv2_0(contId);
					folderId = System.Convert.ToInt32(contCol["FolderID"]);
					Response.Write("<h3>" + contCol["ContentTitle"] + "</h3>");
					
					custFldsCol = ekc.GetFieldsByFolder(folderId, m_refContApi.ContentLanguage);
					//    Returns collection from DataIo:
					//        CustomFieldID As Integer
					//        CustomFieldName As String
					//        Required As Integer
					//        Assigned As Integer
					//        OwningFolderID As Integer
					//
					foreach (Collection tempLoopVar_custFld in custFldsCol)
					{
						custFld = tempLoopVar_custFld;
						custFldName = custFld["CustomFieldName"].ToString();
                        custFldID = Convert.ToInt64(custFld["CustomFieldID"]);
						// only show searchable custom fields, that are set to allow searching:
						searchableMeta = ekc.IsMetadataSearchableByID(custFldID);
						if (searchableMeta)
						{
							// Get the metadata:
							metaValue = ekc.GetJustMetaDataByContIdMetaId(contentId, custFldID);
							Response.Write(custFldName + ": " + metaValue + "<br />");
						}
					}
					
					Response.Write("<br />");
					Response.Write(contCol["ContentHtml"]);
					//Response.Write(m_AppRef.ecmContentBlock(formId))
				}
				
				m_refContApi = null;
			}
			catch (Exception ex)
			{
				Utilities.ShowError(ex.Message);
			}
		}
		protected void RegisterResources()
		{
			Ektron.Cms.API.Css.RegisterCss(this, m_refContApi.AppPath + "default.css", "EktronDefaultCss");
			Ektron.Cms.API.Css.RegisterCss(this, m_refContApi.AppPath + "displayresult.css", "EktronDefaultResultCss");
		}
	}
	
