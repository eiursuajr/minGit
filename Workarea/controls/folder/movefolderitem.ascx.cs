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
using Ektron.Cms.Commerce;
//using Ektron.Cms.Common.EkEnumeration;
using Ektron.Cms.Common;

	public partial class movefolderitem : System.Web.UI.UserControl
	{
		
		
		protected ContentAPI m_refContentApi = new ContentAPI();
		protected StyleHelper m_refStyle = new StyleHelper();
		protected EkMessageHelper m_refMsg;
		protected long m_intId = 0;
		protected FolderData folder_data;
		protected PermissionData security_data;
		protected string AppImgPath = "";
		protected int ContentType = 1;
		protected long CurrentUserId = 0;
		protected Collection pagedata;
		protected string m_strPageAction = "";
		protected string m_strOrderBy = "";
		protected int ContentLanguage = -1;
		protected int EnableMultilingual = 0;
		protected string SitePath = "";
		protected Ektron.Cms.Content.EkContent m_refContent;
		protected long m_intFolderId = -1;
		protected int m_rootFolderIsXml = 0;
		protected int m_intCurrentPage = 1;
		protected int m_intTotalPages = 1;
		protected int m_intTotalRecords = 0;
		protected string m_strDisabled = " disabled ";
		protected string m_strRadBtnCopy = " ";
		protected string m_strRadBtnMove = " checked ";
		protected bool m_bShowCheckin = true;
		protected string m_refCopyMoveHref = "";
		protected string FolderId = "";
		protected bool _initIsCommerceAdmin = false;
		protected bool _isCommerceAdmin = false;
		protected bool _initIsFolderAdmin = false;
		protected bool _isFolderAdmin = false;
		protected bool _initIsCopyOrMoveAdmin = false;
		protected bool _isCopyOrMoveAdmin = false;
		protected string pbcAction = "0";
		private void Page_Load(System.Object sender, System.EventArgs e)
		{
			m_refMsg = m_refContentApi.EkMsgRef;
			RegisterResources();
			InitFolderIsXmlFlags();
			if (!(Request.QueryString["id"] == null))
			{
				m_intId = Convert.ToInt64(Request.QueryString["id"]);
			}
			if (!(Request.QueryString["action"] == null))
			{
				m_strPageAction = Convert.ToString(Request.QueryString["action"]).ToLower().Trim();
			}
			if (!(Request.QueryString["orderby"] == null))
			{
				m_strOrderBy = Convert.ToString(Request.QueryString["orderby"]);
			}
			folder_data = m_refContentApi.GetFolderById(m_intId);
			if (!(Request.QueryString["LangType"] == null))
			{
				if (Request.QueryString["LangType"] != "")
				{
					ContentLanguage = Convert.ToInt32(Request.QueryString["LangType"]);
					m_refContentApi.SetCookieValue("LastValidLanguageID", ContentLanguage.ToString());
				}
				else
				{
					if (m_refContentApi.GetCookieValue("LastValidLanguageID") != "")
					{
						ContentLanguage = Convert.ToInt32(m_refContentApi.GetCookieValue("LastValidLanguageID"));
					}
				}
			}
			else
			{
				if (m_refContentApi.GetCookieValue("LastValidLanguageID") != "")
				{
					ContentLanguage = Convert.ToInt32(m_refContentApi.GetCookieValue("LastValidLanguageID"));
				}
			}
			m_refCopyMoveHref = (string) ("content.aspx?LangType=" + ContentLanguage + "&action=" + m_strPageAction + "&id=" + m_intId);
			if (ContentLanguage == Ektron.Cms.Common.EkConstants.CONTENT_LANGUAGES_UNDEFINED)
			{
				m_refContentApi.ContentLanguage = Ektron.Cms.Common.EkConstants.ALL_CONTENT_LANGUAGES;
			}
			else
			{
				m_refContentApi.ContentLanguage = ContentLanguage;
			}
			if ((Request.QueryString["op"] != null)&& Request.QueryString["op"] == "copy")
			{
				m_strRadBtnCopy = " checked ";
				m_strRadBtnMove = " ";
				m_strDisabled = "";
				m_bShowCheckin = false;
			}
			CurrentUserId = m_refContentApi.UserId;
			AppImgPath = m_refContentApi.AppImgPath;
			SitePath = m_refContentApi.SitePath;
			EnableMultilingual = m_refContentApi.EnableMultilingual;
			m_refContent = m_refContentApi.EkContentRef;
            if (IsPostBack && Request.Form[isPostData.UniqueID] != "")
			{
				Process_MoveMultiContent();
			}
			else
			{
				if (IsPostBack== false || (Request.Form.Count > 0 && Request.Form[isPostData.UniqueID] != ""))
				{
                    Ektron.Cms.Common.EkEnumeration.FolderType folderType = (Ektron.Cms.Common.EkEnumeration.FolderType)Enum.Parse(typeof(Ektron.Cms.Common.EkEnumeration.FolderType), folder_data.FolderType.ToString());
                    Display_Move(folderType);
					//Display_Move(folder_data.FolderType);
				}
			}
			Util_SetServerVariables();
			
		}
		
		private bool IsCommerceAdmin()
		{
			if (_initIsCommerceAdmin)
			{
				return _isCommerceAdmin;
			}
			_isCommerceAdmin = m_refContentApi.IsARoleMember(Ektron.Cms.Common.EkEnumeration.CmsRoleIds.CommerceAdmin);
			_initIsCommerceAdmin = true;
			return _isCommerceAdmin;
		}
		
		private bool IsFolderAdmin()
		{
			if (_initIsFolderAdmin)
			{
				return _isFolderAdmin;
			}
			_isFolderAdmin = m_refContentApi.IsARoleMemberForFolder_FolderUserAdmin(m_intId, 0, false);
			_initIsFolderAdmin = true;
			return _isFolderAdmin;
		}
		
		private bool IsCopyOrMoveAdmin()
		{
			if (_initIsCopyOrMoveAdmin)
			{
				return _isCopyOrMoveAdmin;
			}
			_isCopyOrMoveAdmin = m_refContentApi.IsARoleMemberForFolder(Convert.ToInt64(EkEnumeration.CmsRoleIds.MoveOrCopy), m_intId, m_refContentApi.UserId, false);
			_initIsCopyOrMoveAdmin = true;
			return _isCopyOrMoveAdmin;
		}
		
		#region Process
		
		
		private void Process_MoveMultiContent()
		{
			long intMoveFolderId = 0;
			string strContentIds = "";
			string[] arrArray;
			int i = 0;
			string strContentLanguages = "";
			string[] arrLanguages;
			Ektron.Cms.Content.EkContent m_refContent;
			string FolderPath;
			int contCount = 0;
			SiteAPI m_refsite = new SiteAPI();
			
			try
			{
				m_refContent = m_refContentApi.EkContentRef;
				m_intFolderId = Convert.ToInt64(Request.Form[folder_id.UniqueID]);
				intMoveFolderId = m_refContent.GetFolderID(Request.Form["move_folder_id"]);
				arrArray = Strings.Split(Request.Form[contentids.UniqueID], ",", -1, 0);
				for (i = 0; i <= (arrArray.Length - 1); i++)
				{
					if ((Request.Form["id_" + arrArray[i]]).ToString().IndexOf("on") + 1 > 0)
					{
						strContentIds = strContentIds + arrArray[i] + ",";
					}
				}
				if (strContentIds != "")
				{
					strContentIds = strContentIds.Substring(0, strContentIds.Length - 1);
				}
				arrLanguages = Strings.Split(Request.Form[contentlanguages.UniqueID], ",", -1, 0);
				for (i = 0; i <= (arrArray.Length - 1); i++)
				{
					if ((Request.Form["id_" + arrArray[i]]).ToString().IndexOf("on") + 1 > 0)
					{
						strContentLanguages = strContentLanguages + arrLanguages[i] + ",";
					}
				}
				if (strContentLanguages != "")
				{
					strContentLanguages = strContentLanguages.Substring(0, strContentLanguages.Length - 1);
				}
				if (Request.Form[hdnCopyAll.UniqueID] == "true")
				{
					bool publishContent = false;
					if (! (Request.Form["btn_PublishCopiedContent"] == null) && Request.Form["btn_PublishCopiedContent"].ToString() == "on")
					{
						publishContent = true;
					}
					var countContentIds = strContentIds.Split(",".ToCharArray());
					for (contCount = 0; contCount <= countContentIds.Length - 1; contCount++)
					{
						m_refContent.CopyAllLanguageContent(System.Convert.ToInt32(countContentIds[contCount]), intMoveFolderId, publishContent);
					}
				}
				else
				{
					if (! (Request.Form[RadBtnMoveCopyValue.UniqueID] == null) && Request.Form[RadBtnMoveCopyValue.UniqueID].ToString() == "copy")
					{
						bool bPublish = false;
						if (! (Request.Form["btn_PublishCopiedContent"] == null) && Request.Form["btn_PublishCopiedContent"].ToString() == "on")
						{
							bPublish = true;
						}
						m_refContent.CopyContentByID(strContentIds, strContentLanguages, intMoveFolderId, bPublish);
					}
					else
					{
						m_refContent.MoveContent(strContentIds, strContentLanguages, intMoveFolderId);
					}
				}
				
				FolderPath = m_refContent.GetFolderPath(intMoveFolderId);
				if ((FolderPath.Substring(FolderPath.Length - 1, 1) == "\\"))
				{
					FolderPath = FolderPath.Substring(FolderPath.Length - FolderPath.Length - 1, FolderPath.Length - 1);
				}
				FolderPath = FolderPath.Replace("\\", "\\\\");
				Response.Redirect((string) ("content.aspx?LangType=" + ContentLanguage + "&action=ViewContentByCategory&id=" + intMoveFolderId + "&reloadtrees=Forms,Content,Library&TreeNav=" + FolderPath), false);
				
			}
			catch (Exception ex)
			{
				int intError;
				string strError;
				strError = "because a record with the same title exists in the destination folder";
				intError = ex.Message.IndexOf(strError);
				if (intError > -1)
				{
					strError = ex.Message.Substring(0, intError + strError.Length);
					Response.Redirect((string) ("reterror.aspx?info=" + EkFunctions.UrlEncode(strError + ".")), false);
				}
				else
				{
					Response.Redirect((string) ("reterror.aspx?info=" + EkFunctions.UrlEncode(ex.Message)), false);
				}
			}
		}
		
		
		#endregion
		
		
		#region Display
		
		
		public void Display_Move(EkEnumeration.FolderType containerType)
		{
			
			if (containerType == Ektron.Cms.Common.EkEnumeration.FolderType.Catalog)
			{
				
				Display_MoveEntries();
			}
			else
			{
				
				Display_MoveContentByCategory();
			}
			
		}
		
		
		#region Entries
		
		
		private void Display_MoveEntries()
		{
			
			CatalogEntry CatalogManager = new CatalogEntry(m_refContentApi.RequestInformationRef);
			System.Collections.Generic.List<EntryData> entryList = new System.Collections.Generic.List<EntryData>();
			Ektron.Cms.Common.Criteria<EntryProperty> entryCriteria = new Ektron.Cms.Common.Criteria<EntryProperty>();
			
			security_data = m_refContentApi.LoadPermissions(m_intId, "folder", 0);
			
			entryCriteria.AddFilter(EntryProperty.CatalogId, Ektron.Cms.Common.CriteriaFilterOperator.EqualTo, m_intId);
			if (m_refContentApi.RequestInformationRef.ContentLanguage > 0)
			{
				entryCriteria.AddFilter(EntryProperty.LanguageId, Ektron.Cms.Common.CriteriaFilterOperator.EqualTo, m_refContentApi.RequestInformationRef.ContentLanguage);
			}
			entryCriteria.AddFilter(EntryProperty.IsArchived, Ektron.Cms.Common.CriteriaFilterOperator.EqualTo, false);
			entryCriteria.OrderByDirection = Ektron.Cms.Common.EkEnumeration.OrderByDirection.Ascending;
			entryCriteria.OrderByField = EntryProperty.Title;
			
			entryCriteria.PagingInfo.RecordsPerPage = m_refContentApi.RequestInformationRef.PagingSize;
			entryCriteria.PagingInfo.CurrentPage = m_intCurrentPage;
			
			entryList = CatalogManager.GetList(entryCriteria);
			
			m_intTotalPages = System.Convert.ToInt32(entryCriteria.PagingInfo.TotalPages);
			
			// td_copy.Visible = False
			
			MoveContentByCategoryToolBar();
			
			FolderId = folder_data.Id.ToString();
			
			source_folder_is_xml.Value = "1";
			
			Page.ClientScript.RegisterHiddenField("xmlinherited", "false");
			
			lblDestinationFolder.Text = "<input id=\"move_folder_id\" size=\"50%\" name=\"move_folder_id\" value=\"\\\" readonly=\"true\"/>  <a href=\"#\" onclick=\"LoadSelectCatalogFolderChildPage();return true;\">" + m_refMsg.GetMessage("lbl ecomm coupon select folder") + "</a>";
			
			folder_id.Value = m_intId.ToString();
			PageSettings();
			
			Populate_MoveCatalogGrid(entryList);
			
		}
		
		private void Populate_MoveCatalogGrid(System.Collections.Generic.List<EntryData> entryList)
		{
			
			MoveContentByGategoryGrid.Controls.Clear();
			contentids.Value = "";
			contentlanguages.Value = "";
			System.Web.UI.WebControls.BoundColumn colBound;
			colBound = new System.Web.UI.WebControls.BoundColumn();
			colBound.DataField = "BOX";
			colBound.HeaderText = "<input type=\"checkbox\" name=\"all\" onclick=\"checkAll(document.forms[0].all.checked);\">";
			colBound.ItemStyle.Width = Unit.Parse("1");
			colBound.ItemStyle.VerticalAlign = VerticalAlign.Top;
			colBound.ItemStyle.HorizontalAlign = HorizontalAlign.Center;
			colBound.HeaderStyle.CssClass = "title-header";
			MoveContentByGategoryGrid.Columns.Add(colBound);
			
			colBound = new System.Web.UI.WebControls.BoundColumn();
			colBound.DataField = "TITLE";
			colBound.HeaderText = "<a class=\"title-header\" href=\"content.aspx?action=MoveContentByCategory&orderby=Title&id=" + m_intId + "&LangType=" + ContentLanguage + "\" title=\"" + m_refMsg.GetMessage("click to sort msg") + "\">" + m_refMsg.GetMessage("generic Title") + "</a>";
			colBound.ItemStyle.VerticalAlign = VerticalAlign.Top;
			colBound.HeaderStyle.CssClass = "title-header";
			colBound.ItemStyle.Wrap = false;
			MoveContentByGategoryGrid.Columns.Add(colBound);
			
			colBound = new System.Web.UI.WebControls.BoundColumn();
			colBound.DataField = "ID";
			colBound.HeaderText = "<a class=\"title-header\" href=\"content.aspx?action=MoveContentByCategory&orderby=ID&id=" + m_intId + "&LangType=" + ContentLanguage + "\" title=\"" + m_refMsg.GetMessage("click to sort msg") + "\">" + m_refMsg.GetMessage("generic ID") + "</a>";
			colBound.HeaderStyle.CssClass = "title-header";
			colBound.ItemStyle.Wrap = false;
			colBound.ItemStyle.VerticalAlign = VerticalAlign.Top;
			colBound.ItemStyle.HorizontalAlign = HorizontalAlign.Center;
			MoveContentByGategoryGrid.Columns.Add(colBound);
			
			colBound = new System.Web.UI.WebControls.BoundColumn();
			colBound.DataField = "STATUS";
			colBound.HeaderText = "<a class=\"title-header\" href=\"content.aspx?action=MoveContentByCategory&orderby=status&id=" + m_intId + "&LangType=" + ContentLanguage + "\" title=\"" + m_refMsg.GetMessage("click to sort msg") + "\">" + m_refMsg.GetMessage("generic Status") + "</a>";
			colBound.ItemStyle.VerticalAlign = VerticalAlign.Top;
			colBound.HeaderStyle.CssClass = "title-header";
			colBound.ItemStyle.HorizontalAlign = HorizontalAlign.Center;
			colBound.ItemStyle.Wrap = false;
			MoveContentByGategoryGrid.Columns.Add(colBound);
			
			colBound = new System.Web.UI.WebControls.BoundColumn();
			colBound.DataField = "DATEMODIFIED";
			colBound.HeaderText = "<a class=\"title-header\" href=\"content.aspx?action=MoveContentByCategory&orderby=DateModified&id=" + m_intId + "&LangType=" + ContentLanguage + "\" title=\"" + m_refMsg.GetMessage("click to sort msg") + "\">" + m_refMsg.GetMessage("generic Date Modified") + "</a>";
			colBound.ItemStyle.VerticalAlign = VerticalAlign.Top;
			colBound.HeaderStyle.CssClass = "title-header";
			colBound.ItemStyle.Wrap = false;
			MoveContentByGategoryGrid.Columns.Add(colBound);
			
			colBound = new System.Web.UI.WebControls.BoundColumn();
			colBound.DataField = "EDITORNAME";
			colBound.HeaderText = "<a class=\"title-header\" href=\"content.aspx?action=MoveContentByCategory&orderby=editor&id=" + m_intId + "&LangType=" + ContentLanguage + "\" title=\"" + m_refMsg.GetMessage("click to sort msg") + "\">" + m_refMsg.GetMessage("generic Last Editor") + "</a>";
			colBound.ItemStyle.VerticalAlign = VerticalAlign.Top;
			colBound.HeaderStyle.CssClass = "title-header";
			colBound.ItemStyle.Wrap = false;
			MoveContentByGategoryGrid.Columns.Add(colBound);
			
			
			DataTable dt = new DataTable();
			DataRow dr;
			
			dt.Columns.Add(new DataColumn("BOX", typeof(string)));
			dt.Columns.Add(new DataColumn("TITLE", typeof(string)));
			dt.Columns.Add(new DataColumn("ID", typeof(long)));
			dt.Columns.Add(new DataColumn("STATUS", typeof(string)));
			dt.Columns.Add(new DataColumn("DATEMODIFIED", typeof(string)));
			dt.Columns.Add(new DataColumn("EDITORNAME", typeof(string)));
			
			int i;
			for (i = 0; i <= entryList.Count - 1; i++)
			{
				dr = dt.NewRow();
				dr[0] = "";
				if (entryList[i].ContentStatus == "A" || (m_bShowCheckin && entryList[i].ContentStatus == "I"))
				{
					if (contentids.Value == "")
					{
						contentids.Value = entryList[i].Id.ToString();
					}
					else
					{
						contentids.Value += "," + entryList[i].Id;
					}
					
					if (contentlanguages.Value == "")
					{
						contentlanguages.Value = entryList[i].LanguageId.ToString();
					}
					else
					{
						contentlanguages.Value += "," + entryList[i].LanguageId.ToString();
					}
					
					dr[0] = "<input type=\"checkbox\" onclick=\"checkAllFalse();\" name=\"id_" + entryList[i].Id + "\">";
				}
				if (entryList[i].EntryType == Ektron.Cms.Common.EkEnumeration.CatalogEntryType.SubscriptionProduct)
				{

                    dr[1] = dr[1] + "<img src=\"" + m_refContentApi.AppPath + "images/ui/icons/bookGreen.png" + "\" class=\"ektronRightSpaceVerySmall ektronLeft\"/>" + entryList[i].Title;
				}
				else if (entryList[i].EntryType == Ektron.Cms.Common.EkEnumeration.CatalogEntryType.ComplexProduct)
				{

                    dr[1] = dr[1] + "<img src=\"" + m_refContentApi.AppPath + "images/ui/icons/bricks.png" + "\"  class=\"ektronRightSpaceVerySmall ektronLeft\"/>" + entryList[i].Title;
				}
				else if (entryList[i].EntryType == Ektron.Cms.Common.EkEnumeration.CatalogEntryType.Kit)
				{

                    dr[1] = dr[1] + "<img src=\"" + m_refContentApi.AppPath + "images/ui/icons/box.png" + "\"  class=\"ektronRightSpaceVerySmall ektronLeft\"/>" + entryList[i].Title;
				}
				else if (entryList[i].EntryType == Ektron.Cms.Common.EkEnumeration.CatalogEntryType.Bundle)
				{

                    dr[1] = dr[1] + "<img src=\"" + m_refContentApi.AppPath + "images/ui/icons/package.png" + "\"  class=\"ektronRightSpaceVerySmall ektronLeft\"/>" + entryList[i].Title;
				}
				else if (entryList[i].EntryType == Ektron.Cms.Common.EkEnumeration.CatalogEntryType.Product)
				{

                    dr[1] = dr[1] + "<img src=\"" + m_refContentApi.AppPath + "images/ui/icons/brick.png" + "\"  class=\"ektronRightSpaceVerySmall ektronLeft\"/>" + entryList[i].Title;
				}
				dr[2] = entryList[i].Id;
				dr[3] = entryList[i].ContentStatus;
				dr[4] = entryList[i].DateModified.ToShortDateString();
				dr[5] = entryList[i].LastEditorLastName;
				dt.Rows.Add(dr);
			}
			
			DataView dv = new DataView(dt);
			MoveContentByGategoryGrid.DataSource = dv;
			MoveContentByGategoryGrid.DataBind();
		}
		
		
		#endregion
		
		
		#region Content
		
		
		private void Display_MoveContentByCategory()
		{
			Ektron.Cms.Common.EkContentCol contentdata = new Ektron.Cms.Common.EkContentCol();
			
			pagedata = new Microsoft.VisualBasic.Collection();
			pagedata.Add(m_intId, "FolderID", null, null);
			pagedata.Add(m_strOrderBy, "OrderBy", null, null);
			security_data = m_refContentApi.LoadPermissions(m_intId, "folder", 0);
			folder_data = m_refContentApi.GetFolderById(m_intId);

            contentdata = m_refContent.GetAllViewableChildInfov5_0(pagedata, m_intCurrentPage, m_refContentApi.RequestInformationRef.PagingSize, ref m_intTotalPages, Ektron.Cms.Common.EkEnumeration.CMSContentType.AllTypes, Ektron.Cms.Common.EkEnumeration.CMSContentSubtype.AllTypes);
			
			MoveContentByCategoryToolBar();
			FolderId = folder_data.Id.ToString();
			
			
			// javascript needs to know if source folder is using xml:
			source_folder_is_xml.Value = (!(folder_data.XmlConfiguration == null)) ? "1" : "0";
			
			// Obsolete: The recommended alternative is ClientScript.RegisterHiddenField(string
			// hiddenFieldName, string hiddenFieldInitialValue).
			// http://go.microsoft.com/fwlink/?linkid=14202
			Page.ClientScript.RegisterHiddenField("xmlinherited", "false");
			
			m_refContent = m_refContentApi.EkContentRef;
			//        cAllFolders = m_refContent.GetFolderTreeForUserIDWithXMLInfo(0)
			
			string destinationFolder;
			if (folder_data.FolderType == 9)
			{
				destinationFolder = "<span style=\"white-space:nowrap\"><input id=\"move_folder_id\" size=\"50%\" name=\"move_folder_id\" value=\"\\\" readonly=\"true\"/>  <a class=\"button buttonInline greenHover minHeight buttonCheckAll\" style=\"padding-top:.25em; padding-bottom:.25em;\" href=\"#\" onclick=\"LoadSelectCatalogFolderChildPage();return true;\">" + m_refMsg.GetMessage("lbl ecomm coupon select folder") + "</a></span>";
			}
			else
			{
				destinationFolder = "<span style=\"white-space:nowrap\"><input id=\"move_folder_id\" size=\"50%\" name=\"move_folder_id\" value=\"\\\" readonly=\"true\"/>  <a class=\"button buttonInline greenHover minHeight buttonCheckAll\" style=\"padding-top:.25em; padding-bottom:.25em;\" href=\"#\" onclick=\"LoadSelectFolderChildPage();return true;\">" + m_refMsg.GetMessage("lbl select folder") + "</a></span>";
			}
			
			lblDestinationFolder.Text = destinationFolder.ToString();
			folder_id.Value = m_intId.ToString();
			PageSettings();
			
			Populate_MoveContentByCategoryGrid(contentdata);
			
		}
		
		private void Populate_MoveContentByCategoryGrid(EkContentCol contentdata)
		{
			MoveContentByGategoryGrid.Controls.Clear();
			contentids.Value = "";
			contentlanguages.Value = "";
			System.Web.UI.WebControls.BoundColumn colBound;
			colBound = new System.Web.UI.WebControls.BoundColumn();
			colBound.DataField = "BOX";
			colBound.HeaderText = "<input type=\"checkbox\" name=\"all\" onclick=\"checkAll(document.forms[0].all.checked);\">";
			colBound.ItemStyle.Width = Unit.Parse("1");
			colBound.ItemStyle.VerticalAlign = VerticalAlign.Top;
			colBound.ItemStyle.HorizontalAlign = HorizontalAlign.Center;
			colBound.HeaderStyle.CssClass = "title-header";
			MoveContentByGategoryGrid.Columns.Add(colBound);
			
			colBound = new System.Web.UI.WebControls.BoundColumn();
			colBound.DataField = "TITLE";
			colBound.HeaderText = "<a class=\"title-header\" href=\"content.aspx?action=MoveContentByCategory&orderby=Title&id=" + m_intId + "&LangType=" + ContentLanguage + "\" title=\"" + m_refMsg.GetMessage("click to sort msg") + "\">" + m_refMsg.GetMessage("generic Title") + "</a>";
			colBound.ItemStyle.VerticalAlign = VerticalAlign.Top;
			colBound.HeaderStyle.CssClass = "title-header";
			colBound.ItemStyle.Wrap = false;
			MoveContentByGategoryGrid.Columns.Add(colBound);
			
			colBound = new System.Web.UI.WebControls.BoundColumn();
			colBound.DataField = "ID";
			colBound.HeaderText = "<a class=\"title-header\" href=\"content.aspx?action=MoveContentByCategory&orderby=ID&id=" + m_intId + "&LangType=" + ContentLanguage + "\" title=\"" + m_refMsg.GetMessage("click to sort msg") + "\">" + m_refMsg.GetMessage("generic ID") + "</a>";
			colBound.HeaderStyle.CssClass = "title-header";
			colBound.ItemStyle.Wrap = false;
			colBound.ItemStyle.VerticalAlign = VerticalAlign.Top;
			colBound.ItemStyle.HorizontalAlign = HorizontalAlign.Center;
			MoveContentByGategoryGrid.Columns.Add(colBound);
			
			colBound = new System.Web.UI.WebControls.BoundColumn();
			colBound.DataField = "STATUS";
			colBound.HeaderText = "<a class=\"title-header\" href=\"content.aspx?action=MoveContentByCategory&orderby=status&id=" + m_intId + "&LangType=" + ContentLanguage + "\" title=\"" + m_refMsg.GetMessage("click to sort msg") + "\">" + m_refMsg.GetMessage("generic Status") + "</a>";
			colBound.ItemStyle.VerticalAlign = VerticalAlign.Top;
			colBound.HeaderStyle.CssClass = "title-header";
			colBound.ItemStyle.HorizontalAlign = HorizontalAlign.Center;
			colBound.ItemStyle.Wrap = false;
			MoveContentByGategoryGrid.Columns.Add(colBound);
			
			colBound = new System.Web.UI.WebControls.BoundColumn();
			colBound.DataField = "DATEMODIFIED";
			colBound.HeaderText = "<a class=\"title-header\" href=\"content.aspx?action=MoveContentByCategory&orderby=DateModified&id=" + m_intId + "&LangType=" + ContentLanguage + "\" title=\"" + m_refMsg.GetMessage("click to sort msg") + "\">" + m_refMsg.GetMessage("generic Date Modified") + "</a>";
			colBound.ItemStyle.VerticalAlign = VerticalAlign.Top;
			colBound.HeaderStyle.CssClass = "title-header";
			colBound.ItemStyle.Wrap = false;
			MoveContentByGategoryGrid.Columns.Add(colBound);
			
			colBound = new System.Web.UI.WebControls.BoundColumn();
			colBound.DataField = "EDITORNAME";
			colBound.HeaderText = "<a class=\"title-header\" href=\"content.aspx?action=MoveContentByCategory&orderby=editor&id=" + m_intId + "&LangType=" + ContentLanguage + "\" title=\"" + m_refMsg.GetMessage("click to sort msg") + "\">" + m_refMsg.GetMessage("generic Last Editor") + "</a>";
			colBound.ItemStyle.VerticalAlign = VerticalAlign.Top;
			colBound.HeaderStyle.CssClass = "title-header";
			colBound.ItemStyle.Wrap = false;
			MoveContentByGategoryGrid.Columns.Add(colBound);
			
			
			DataTable dt = new DataTable();
			DataRow dr;
			
			dt.Columns.Add(new DataColumn("BOX", typeof(string)));
			dt.Columns.Add(new DataColumn("TITLE", typeof(string)));
			dt.Columns.Add(new DataColumn("ID", typeof(long)));
			dt.Columns.Add(new DataColumn("STATUS", typeof(string)));
			dt.Columns.Add(new DataColumn("DATEMODIFIED", typeof(string)));
			dt.Columns.Add(new DataColumn("EDITORNAME", typeof(string)));
			
			int i;
			for (i = 0; i <= contentdata.Count - 1; i++)
			{
				//If (contentdata.Item(i).ContentSubType <> CMSContentSubtype.PageBuilderData) Then
				dr = dt.NewRow();
				dr[0] = "";
                if (contentdata.get_Item(i).ContentStatus == "A" || (m_bShowCheckin && contentdata.get_Item(i).ContentStatus == "I"))
				{
                    if (contentdata.get_Item(i).ContentSubType == Ektron.Cms.Common.EkEnumeration.CMSContentSubtype.PageBuilderData)
					{
						pbcAction = "1";
					}
					if (contentids.Value == "")
					{
                        contentids.Value = contentdata.get_Item(i).Id.ToString();
					}
					else
					{
                        contentids.Value += "," + contentdata.get_Item(i).Id;
					}
					
					if (contentlanguages.Value == "")
					{
                        contentlanguages.Value = contentdata.get_Item(i).Language.ToString();
					}
					else
					{
                        contentlanguages.Value += "," + contentdata.get_Item(i).Language;
					}

                    dr[0] = "<input type=\"checkbox\" onclick=\"checkAllFalse();\" name=\"id_" + contentdata.get_Item(i).Id + "\">";
				}
                if (contentdata.get_Item(i).ContentType == Ektron.Cms.Common.EkEnumeration.CMSContentType.CatalogEntry)
				{
                    dr[1] = "<img src=\"" + m_refContentApi.AppPath + "images/ui/icons/brick.png\" class=\"ektronRightSpaceVerySmall ektronLeft\" />" + "<a href=\"content.aspx?LangType=" + contentdata.get_Item(i).Language + "&action=View&id=" + contentdata.get_Item(i).Id + "\" title=\'" + m_refMsg.GetMessage("generic View") + " \"" + Strings.Replace(contentdata.get_Item(i).Title, "\'", "`", 1, -1, 0) + "\"" + "\'>" + contentdata.get_Item(i).Title + "</a>";
				}
                else if (contentdata.get_Item(i).ContentType == Ektron.Cms.Common.EkEnumeration.CMSContentType.Forms)
				{
                    dr[1] = "<img src=\"images/ui/icons/contentForm.png\"/>&nbsp;" + "<a href=\"content.aspx?LangType=" + contentdata.get_Item(i).Language + "&action=View&id=" + contentdata.get_Item(i).Id + "\" title=\'" + m_refMsg.GetMessage("generic View") + " \"" + Strings.Replace(contentdata.get_Item(i).Title, "\'", "`", 1, -1, 0) + "\"" + "\'>" + contentdata.get_Item(i).Title + "</a>";
				}
                else if (contentdata.get_Item(i).ContentType == Ektron.Cms.Common.EkEnumeration.CMSContentType.Content)
				{
                    if (contentdata.get_Item(i).ContentSubType == Ektron.Cms.Common.EkEnumeration.CMSContentSubtype.PageBuilderData)
					{
                        dr[1] = "<img src=\"images/application/layout_content.png\"/>&nbsp;" + "<a href=\"content.aspx?LangType=" + contentdata.get_Item(i).Language + "&action=View&id=" + contentdata.get_Item(i).Id + "\" title=\'" + m_refMsg.GetMessage("generic View") + " \"" + Strings.Replace(contentdata.get_Item(i).Title, "\'", "`", 1, -1, 0) + "\"" + "\'>" + contentdata.get_Item(i).Title + "</a>";
					}
					else
					{
                        dr[1] = "<img src=\"images/ui/icons/contentHtml.png\"/>&nbsp;" + "<a href=\"content.aspx?LangType=" + contentdata.get_Item(i).Language + "&action=View&id=" + contentdata.get_Item(i).Id + "\" title=\'" + m_refMsg.GetMessage("generic View") + " \"" + Strings.Replace(contentdata.get_Item(i).Title, "\'", "`", 1, -1, 0) + "\"" + "\'>" + contentdata.get_Item(i).Title + "</a>";
					}
				}
				else
				{
                    dr[1] = "<img src=\"" + contentdata.get_Item(i).AssetInfo.ImageUrl + "\"  class=\"ektronRightSpaceVerySmall ektronLeft\"/>" + "<a href=\"content.aspx?LangType=" + contentdata.get_Item(i).Language + "&action=View&id=" + contentdata.get_Item(i).Id + "\" title=\'" + m_refMsg.GetMessage("generic View") + " \"" + Strings.Replace(contentdata.get_Item(i).Title, "\'", "`", 1, -1, 0) + "\"" + "\'>" + contentdata.get_Item(i).Title + "</a>";
				}

                dr[2] = contentdata.get_Item(i).Id;
                dr[3] = contentdata.get_Item(i).Status;
                dr[4] = contentdata.get_Item(i).DisplayDateModified;
                dr[5] = contentdata.get_Item(i).LastEditorLname;
				dt.Rows.Add(dr);
				//End If
			}
			
			
			DataView dv = new DataView(dt);
			MoveContentByGategoryGrid.DataSource = dv;
			MoveContentByGategoryGrid.DataBind();
		}
		
		
		#endregion
		
		
		#endregion
		
		
		#region Private Helpers
		
		
		private void Util_SetLabels()
		{
			
			if (folder_data.FolderType == 9)
			{
				
				ltr_publishcopied.Text = m_refMsg.GetMessage("lbl publish copied entry");
				
			}
			else
			{
				
				ltr_publishcopied.Text = m_refMsg.GetMessage("lbl publish copied content");
				
			}
			
		}
		
		private void MoveContentByCategoryToolBar()
		{
			System.Text.StringBuilder result = new System.Text.StringBuilder();
			Util_SetLabels();
			//If folder_data.FolderType = 9 Then
			//txtTitleBar.InnerHtml = m_refStyle.GetTitleBar(m_refMsg.GetMessage("just move contents of folder") & " """ & folder_data.Name & """")
			//Else
			txtTitleBar.InnerHtml = m_refStyle.GetTitleBar((string) (m_refMsg.GetMessage("move contents of folder") + " \"" + folder_data.Name + "\""));
			//End If
			result.Append("<table><tr>");
			result.Append(m_refStyle.GetButtonEventsWCaption(AppImgPath + "../UI/Icons/back.png", (string)("content.aspx?action=ViewContentByCategory&id=" + m_intId + "&LangType=" + ContentLanguage), m_refMsg.GetMessage("alt back button text"), m_refMsg.GetMessage("btn back"), "", StyleHelper.BackButtonCssClass, true));
			if (security_data.IsAdmin || (folder_data.FolderType == Convert.ToInt32(EkEnumeration.FolderType.Catalog) && true) || IsFolderAdmin() || IsCopyOrMoveAdmin())
			{
				//If folder_data.FolderType = EkEnumeration.FolderType.Catalog Then
				//result.Append(m_refStyle.GetButtonEventsWCaption(AppImgPath & "../UI/Icons/contentCopy.png", "#", m_refMsg.GetMessage("btn just move content"), m_refMsg.GetMessage("btn just move content"), "onclick=""checkMoveForm_Folder('" & m_refContent.GetFolderPath(m_intId).Replace("\", "\\") & "');return false;"""))
				//Else
				result.Append(m_refStyle.GetButtonEventsWCaption(AppImgPath + "../UI/Icons/contentCopy.png", "#", m_refMsg.GetMessage("btn move content"), m_refMsg.GetMessage("btn move content"), "onclick=\"checkMoveForm_Folder(\'" + m_refContent.GetFolderPath(m_intId).Replace("\\", "\\\\") + "\');return false;\"", StyleHelper.MoveContentButtonCssClass, true));
				//End If
			}
			result.Append(StyleHelper.ActionBarDivider);
			result.Append("<td>");
			result.Append(m_refStyle.GetHelpButton(m_strPageAction, ""));
			result.Append("</td>");
			result.Append("</tr></table>");
			htmToolBar.InnerHtml = result.ToString();
		}
		
		private void InitFolderIsXmlFlags()
		{
			FolderData fldrData;
			long nFolderID = 0;
			fldrData = m_refContentApi.GetFolderById(0);
			m_rootFolderIsXml = System.Convert.ToInt32((!(fldrData.XmlConfiguration == null)) ? 1 : 0);
			if (!(Request.QueryString["id"] == null))
			{
				nFolderID = Convert.ToInt64(Request.QueryString["id"]);
			}
			fldrData = m_refContentApi.GetFolderById(nFolderID);
			source_folder_is_xml.Value = (!(fldrData.XmlConfiguration == null)) ? "1" : "0";
		}
		
		private void RegisterResources()
		{
			Ektron.Cms.API.JS.RegisterJS(this, Ektron.Cms.API.JS.ManagedScript.EktronWorkareaHelperJS);
		}
		#endregion
		
		
		#region Paging
		
		
		private void PageSettings()
		{
			if (m_intTotalPages <= 1)
			{
				VisiblePageControls(false);
			}
			else
			{
				VisiblePageControls(true);
                TotalPages.Text = (System.Math.Ceiling(Convert.ToDouble(m_intTotalPages))).ToString();
				//hTotalPages.Value = (System.Math.Ceiling(m_intTotalPages)).ToString()
                TotalPages.ToolTip = TotalPages.Text;
				PageLabel.ToolTip = PageLabel.Text;
				OfLabel.ToolTip = OfLabel.Text;
                CurrentPage.Text = m_intCurrentPage.ToString();
				//hCurrentPage.Value = m_intCurrentPage.ToString()
                CurrentPage.ToolTip = CurrentPage.Text;
				
				ctrlPreviousPage.Enabled = true;
				ctrlPreviousPage.ToolTip = ctrlPreviousPage.Text;
				ctrlFirstPage.Enabled = true;
				ctrlFirstPage.ToolTip = ctrlFirstPage.Text;
				ctrlLastPage.Enabled = true;
				ctrlLastPage.ToolTip = ctrlLastPage.Text;
				ctrlNextPage.Enabled = true;
				ctrlNextPage.ToolTip = ctrlNextPage.Text;
				if (m_intCurrentPage == 1)
				{
					ctrlPreviousPage.Enabled = false;
					ctrlFirstPage.Enabled = false;
				}
				else if (m_intCurrentPage == m_intTotalPages)
				{
					ctrlNextPage.Enabled = false;
					ctrlLastPage.Enabled = false;
				}
			}
		}
		
		private void VisiblePageControls(bool flag)
		{
            TotalPages.Visible = flag;
            CurrentPage.Visible = flag;
			ctrlPreviousPage.Visible = flag;
			ctrlNextPage.Visible = flag;
			ctrlLastPage.Visible = flag;
			ctrlFirstPage.Visible = flag;
			PageLabel.Visible = flag;
			OfLabel.Visible = flag;
		}
		
		protected void NavigationLink_Click(object sender, CommandEventArgs e)
		{
			switch (e.CommandName)
			{
				case "First":
					m_intCurrentPage = 1;
					break;
				case "Last":
                    m_intCurrentPage = int.Parse((string)TotalPages.Text);
					break;
				case "Next":
					m_intCurrentPage = System.Convert.ToInt32(int.Parse((string) CurrentPage.Text) + 1);
					break;
				case "Prev":
                    m_intCurrentPage = System.Convert.ToInt32(int.Parse((string)CurrentPage.Text) - 1);
					break;
			}


            Ektron.Cms.Common.EkEnumeration.FolderType folderType = (Ektron.Cms.Common.EkEnumeration.FolderType)Enum.Parse(typeof(Ektron.Cms.Common.EkEnumeration.FolderType), folder_data.FolderType.ToString());
            Display_Move(folderType);
			//Display_Move(folder_data.FolderType);
			isPostData.Value = "true";
		}
		
		
		#endregion
		#region Utilities
		private void Util_SetServerVariables()
		{
			jsConfirmCopyAll.Text = m_refMsg.GetMessage("jsconfirm copy all");
		}
		#endregion
		
		
	}
	
