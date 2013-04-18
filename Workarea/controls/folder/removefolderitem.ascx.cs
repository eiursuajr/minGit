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
using Ektron.Cms.API;
using Ektron.Cms.Common;
using Ektron.Cms.Content;


	public partial class removefolderitem : System.Web.UI.UserControl
	{
		
		
		#region Member Variables
		
		protected ContentAPI _ContentApi = new ContentAPI();
		protected StyleHelper _StyleHelper = new StyleHelper();
		protected EkMessageHelper _MessageHelper;
		protected long _Id = 0;
		protected FolderData _FolderData;
		protected PermissionData _PermissionData;
		protected string _AppImgPath = "";
		protected int _ContentType = 1;
		protected long _CurrentUserId = 0;
		protected Collection _PageData;
		protected string _PageAction = "";
		protected string _OrderBy = "";
		protected int _ContentLanguage = -1;
		protected int _EnableMultilingual = 0;
		protected string _SitePath = "";
		protected EkContent _EkContent;
		protected long _FolderId = -1;
		protected int _CurrentPageId = 1;
		protected int _TotalPagesNumber = 1;
		protected int _TotalRecordsNumber = 0;
		protected bool _ShowArchive = false;
		
		#endregion
		
		#region Events
		
		private void Page_Load(System.Object sender, System.EventArgs e)
		{
			_MessageHelper = _ContentApi.EkMsgRef;
			if (!(Request.QueryString["id"] == null))
			{
				_Id = Convert.ToInt64(Request.QueryString["id"]);
			}
			if (!(Request.QueryString["action"] == null))
			{
				_PageAction = Convert.ToString(Request.QueryString["action"]).ToLower().Trim();
			}
			if (!(Request.QueryString["orderby"] == null))
			{
				_OrderBy = Convert.ToString(Request.QueryString["orderby"]);
			}
			if ((!(Request.QueryString["showarchive"] == null)) && (Request.QueryString["showarchive"].ToString().ToLower() == "true"))
			{
				_ShowArchive = true;
			}
			if (!(Request.QueryString["LangType"] == null))
			{
				if (Request.QueryString["LangType"] != "")
				{
					_ContentLanguage = Convert.ToInt32(Request.QueryString["LangType"]);
					_ContentApi.SetCookieValue("LastValidLanguageID", _ContentLanguage.ToString());
				}
				else
				{
					if (_ContentApi.GetCookieValue("LastValidLanguageID") != "")
					{
						_ContentLanguage = Convert.ToInt32(_ContentApi.GetCookieValue("LastValidLanguageID"));
					}
				}
			}
			else
			{
				if (_ContentApi.GetCookieValue("LastValidLanguageID") != "")
				{
					_ContentLanguage = Convert.ToInt32(_ContentApi.GetCookieValue("LastValidLanguageID"));
				}
			}
			
			if (Request.QueryString["currentpage"] != null)
			{
				_CurrentPageId = Convert.ToInt32(Request.QueryString["currentpage"]);
			}
			
			if (_ContentLanguage == Ektron.Cms.Common.EkConstants.CONTENT_LANGUAGES_UNDEFINED)
			{
				_ContentApi.ContentLanguage = Ektron.Cms.Common.EkConstants.ALL_CONTENT_LANGUAGES;
			}
			else
			{
				_ContentApi.ContentLanguage = _ContentLanguage;
			}
			_CurrentUserId = _ContentApi.UserId;
			_AppImgPath = _ContentApi.AppImgPath;
			_SitePath = _ContentApi.SitePath;
			_EkContent = _ContentApi.EkContentRef;
			_EnableMultilingual = _ContentApi.EnableMultilingual;
			contentids.Value = "";
			DeleteContentByCategory();
			isPostData.Value = "true";
			
			RegisterJS();
			
		}
		
		#endregion
		
		#region Helpers
		
		private void DeleteContentByCategory()
		{
			if (IsPostBack && Request.Form[isPostData.UniqueID] != "")
			{
				Process_submitMultiDelContAction();
			}
			else
			{
				if (IsPostBack== false || (Request.Form.Count > 0 && Request.Form[isPostData.UniqueID] != ""))
				{
					Display_Delete();
				}
			}
		}
		
		private void Process_submitMultiDelContAction()
		{
			string strContentIds = "";
			string[] arrArray;
			int i = 0;
			Ektron.Cms.Content.EkContent m_refContent;
			_FolderId = Convert.ToInt64(Request.QueryString["id"]);
			_CurrentPageId = Convert.ToInt32(Request.Form[page_id.UniqueID]);
			try
			{
				m_refContent = _ContentApi.EkContentRef;
				arrArray = Strings.Split(Request.Form[contentids.UniqueID], ",", -1, 0);
				for (i = 0; i <= (arrArray.Length - 1); i++)
				{
					if (Request.Form["id_" + arrArray[i]] == "on")
					{
						strContentIds = strContentIds + arrArray[i] + ",";
					}
				}
				if (strContentIds != "")
				{
					strContentIds = strContentIds.Substring(0, strContentIds.Length - 1);
					m_refContent.SubmitForDeletev2_0(strContentIds, _FolderId);
				}
				Response.Redirect((string) ("content.aspx?LangType=" + _ContentLanguage + "&action=ViewContentByCategory&id=" + _FolderId + (_CurrentPageId > 1 ? "&currentpage=" + _CurrentPageId : "")), false);
			}
			catch (Exception ex)
			{
				Response.Redirect((string) ("reterror.aspx?info=" + EkFunctions.UrlEncode(ex.Message)), false);
			}
		}
		
		#endregion
		
		#region Display
		
		private void Display_Delete()
		{
			
			_FolderData = _ContentApi.GetFolderById(_Id);
			_PermissionData = _ContentApi.LoadPermissions(_Id, "folder", 0);
			
			switch (_FolderData.FolderType)
			{
				case (int)Ektron.Cms.Common.EkEnumeration.FolderType.Catalog:
					Display_DeleteEntries();
					break;
				default:
					Display_DeleteContentByCategory();
					break;
			}
			
		}
		
		#region Entries
		
		
		private void Display_DeleteEntries()
		{
			
			CatalogEntry CatalogManager = new CatalogEntry(_ContentApi.RequestInformationRef);
			System.Collections.Generic.List<EntryData> entryList = new System.Collections.Generic.List<EntryData>();
			Ektron.Cms.Common.Criteria<EntryProperty> entryCriteria = new Ektron.Cms.Common.Criteria<EntryProperty>();
			
			entryCriteria.AddFilter(EntryProperty.CatalogId, Ektron.Cms.Common.CriteriaFilterOperator.EqualTo, _Id);
			if (_ContentApi.RequestInformationRef.ContentLanguage > 0)
			{
				entryCriteria.AddFilter(EntryProperty.LanguageId, Ektron.Cms.Common.CriteriaFilterOperator.EqualTo, _ContentApi.RequestInformationRef.ContentLanguage);
			}
			
			entryCriteria.OrderByDirection = Ektron.Cms.Common.EkEnumeration.OrderByDirection.Ascending;
			entryCriteria.OrderByField = EntryProperty.Title;
			
			entryCriteria.PagingInfo.RecordsPerPage = _ContentApi.RequestInformationRef.PagingSize;
			entryCriteria.PagingInfo.CurrentPage = _CurrentPageId;
			
			if (_ShowArchive == false)
			{
				entryCriteria.AddFilter(EntryProperty.IsArchived, Ektron.Cms.Common.CriteriaFilterOperator.EqualTo, false);
			}
			else
			{
				entryCriteria.AddFilter(EntryProperty.IsArchived, Ektron.Cms.Common.CriteriaFilterOperator.EqualTo, true);
			}
			
			entryList = CatalogManager.GetList(entryCriteria);
			
			_TotalPagesNumber = System.Convert.ToInt32(entryCriteria.PagingInfo.TotalPages);
			
			DeleteContentByCategoryToolBar();
			
			PageSettings();
			
			Populate_DeleteCatalogGrid(entryList);
			
			folder_id.Value =Convert.ToString(_Id);
			
		}
		private void Populate_DeleteCatalogGrid(System.Collections.Generic.List<EntryData> entryList)
		{
			
			DeleteContentByGategoryGrid.Controls.Clear();
			contentids.Value = "";
			System.Web.UI.WebControls.BoundColumn colBound;
			colBound = new System.Web.UI.WebControls.BoundColumn();
			colBound.DataField = "BOX";
			colBound.HeaderText = "<input type=\"checkbox\" name=\"all\" onclick=\"javascript:checkAll(document.forms[0].all.checked);\">";
			colBound.ItemStyle.Width = Unit.Parse("1");
			colBound.ItemStyle.VerticalAlign = VerticalAlign.Top;
			colBound.ItemStyle.HorizontalAlign = HorizontalAlign.Center;
			colBound.HeaderStyle.CssClass = "title-header";
			DeleteContentByGategoryGrid.Columns.Add(colBound);
			
			colBound = new System.Web.UI.WebControls.BoundColumn();
			colBound.DataField = "TITLE";
			colBound.HeaderText = "<a class=\"title-header\" href=\"content.aspx?action=DeleteContentByCategory&orderby=Title&id=" + _Id + "&LangType=" + _ContentLanguage + "\" title=\"" + _MessageHelper.GetMessage("click to sort msg") + "\">" + _MessageHelper.GetMessage("generic Title") + "</a>";
			colBound.ItemStyle.VerticalAlign = VerticalAlign.Top;
			colBound.HeaderStyle.CssClass = "title-header";
			colBound.ItemStyle.Wrap = false;
			DeleteContentByGategoryGrid.Columns.Add(colBound);
			
			colBound = new System.Web.UI.WebControls.BoundColumn();
			colBound.DataField = "ID";
			colBound.HeaderText = "<a class=\"title-header\" href=\"content.aspx?action=DeleteContentByCategory&orderby=ID&id=" + _Id + "&LangType=" + _ContentLanguage + "\" title=\"" + _MessageHelper.GetMessage("click to sort msg") + "\">" + _MessageHelper.GetMessage("generic ID") + "</a>";
			colBound.HeaderStyle.CssClass = "title-header";
			colBound.ItemStyle.Wrap = false;
			colBound.ItemStyle.VerticalAlign = VerticalAlign.Top;
			colBound.ItemStyle.HorizontalAlign = HorizontalAlign.Center;
			DeleteContentByGategoryGrid.Columns.Add(colBound);
			
			colBound = new System.Web.UI.WebControls.BoundColumn();
			colBound.DataField = "STATUS";
			colBound.HeaderText = "<a class=\"title-header\" href=\"content.aspx?action=DeleteContentByCategory&orderby=status&id=" + _Id + "&LangType=" + _ContentLanguage + "\" title=\"" + _MessageHelper.GetMessage("click to sort msg") + "\">" + _MessageHelper.GetMessage("generic Status") + "</a>";
			colBound.ItemStyle.VerticalAlign = VerticalAlign.Top;
			colBound.HeaderStyle.CssClass = "title-header";
			colBound.ItemStyle.HorizontalAlign = HorizontalAlign.Center;
			colBound.ItemStyle.Wrap = false;
			DeleteContentByGategoryGrid.Columns.Add(colBound);
			
			colBound = new System.Web.UI.WebControls.BoundColumn();
			colBound.DataField = "DATEMODIFIED";
			colBound.HeaderText = "<a class=\"title-header\" href=\"content.aspx?action=DeleteContentByCategory&orderby=DateModified&id=" + _Id + "&LangType=" + _ContentLanguage + "\" title=\"" + _MessageHelper.GetMessage("click to sort msg") + "\">" + _MessageHelper.GetMessage("generic Date Modified") + "</a>";
			colBound.ItemStyle.VerticalAlign = VerticalAlign.Top;
			colBound.HeaderStyle.CssClass = "title-header";
			colBound.ItemStyle.Wrap = false;
			DeleteContentByGategoryGrid.Columns.Add(colBound);
			
			colBound = new System.Web.UI.WebControls.BoundColumn();
			colBound.DataField = "EDITORNAME";
			colBound.HeaderText = "<a class=\"title-header\" href=\"content.aspx?action=DeleteContentByCategory&orderby=editor&id=" + _Id + "&LangType=" + _ContentLanguage + "\" title=\"" + _MessageHelper.GetMessage("click to sort msg") + "\">" + _MessageHelper.GetMessage("generic Last Editor") + "</a>";
			colBound.ItemStyle.VerticalAlign = VerticalAlign.Top;
			colBound.HeaderStyle.CssClass = "title-header";
			colBound.ItemStyle.Wrap = false;
			DeleteContentByGategoryGrid.Columns.Add(colBound);
			
			
			DataTable dt = new DataTable();
			DataRow dr;
			
			dt.Columns.Add(new DataColumn("BOX", typeof(string)));
			dt.Columns.Add(new DataColumn("TITLE", typeof(string)));
			dt.Columns.Add(new DataColumn("ID", typeof(long)));
			dt.Columns.Add(new DataColumn("STATUS", typeof(string)));
			dt.Columns.Add(new DataColumn("DATEMODIFIED", typeof(string)));
			dt.Columns.Add(new DataColumn("EDITORNAME", typeof(string)));
			
			int i;
			for (i = 0; i <= (entryList.Count - 1); i++)
			{
				dr = dt.NewRow();
				dr[0] = "";
				if (entryList[i].ContentStatus == "A")
				{
					if (_EkContent.IsAllowed(entryList[i].Id, System.Convert.ToInt32(entryList[i].LanguageId), "content", "delete", 0))
					{
						if (contentids.Value == "")
						{
							contentids.Value = entryList[i].Id.ToString();
						}
						else
						{
							contentids.Value += "," + entryList[i].Id.ToString();
						}
						dr[0] = "<input type=\"checkbox\" onclick=\"checkAllFalse();\" name=\"id_" + entryList[i].Id + "\">";
					}
				}
				
				dr[1] = "<a href=\"content.aspx?LangType=" + entryList[i].LanguageId + "&action=View&id=" + entryList[i].Id + "\" title=\'" + _MessageHelper.GetMessage("generic View") + " \"" + Strings.Replace((string) (entryList[i].Title), "\'", "`", 1, -1, 0) + "\"" + "\'>" + entryList[i].Title + "</a>";
				dr[2] = entryList[i].Id;
				dr[3] = entryList[i].Status;
				dr[4] = entryList[i].DateModified;
				dr[5] = entryList[i].LastEditorLastName;
				dt.Rows.Add(dr);
			}
			
			DataView dv = new DataView(dt);
			DeleteContentByGategoryGrid.DataSource = dv;
			DeleteContentByGategoryGrid.DataBind();
		}
		//Private Sub DeleteContentByCategoryToolBar()
		//    Dim result As New System.Text.StringBuilder
		
		//    txtTitleBar.InnerHtml = m_refStyle.GetTitleBar("Delete Contents of Folder " & " """ & folder_data.Name & """")
		//    result.Append("<table border=""0"" cellspacing=""0"" cellpadding=""0"" ID=""Table14""></tr>")
		//    If (security_data.IsAdmin _
		//     OrElse m_refContentApi.IsARoleMemberForFolder_FolderUserAdmin(folder_data.Id) _
		//     OrElse m_refContent.IsAllowed(folder_data.Id, m_refContentApi.RequestInformationRef.ContentLanguage, "folder", "delete")) Then
		//        result.Append(m_refStyle.GetButtonEventsWCaption(AppImgPath & "btn_delete_content-nm.gif", "#", "Delete Content", m_refMsg.GetMessage("btn delete content"), "OnClick=""javascript:checkDeleteForm();"""))
		//    End If
		//    result.Append(m_refStyle.GetButtonEventsWCaption(AppImgPath & "btn_back-nm.gif", "content.aspx?LangType=" & ContentLanguage & "&action=ViewContentByCategory&id=" & m_intId, m_refMsg.GetMessage("alt back button text"), m_refMsg.GetMessage("btn back"), ""))
		//    result.Append("<td>")
		//    result.Append(m_refStyle.GetHelpButton(m_refStyle.GetHelpAliasPrefix(folder_data) & m_strPageAction))
		//    result.Append("&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;</td>")
		//    result.Append("</tr></table>")
		//    htmToolBar.InnerHtml = result.ToString
		//End Sub
		
		
		#endregion
		
		#region Content
		
		
		private void Display_DeleteContentByCategory()
		{
			
			Ektron.Cms.Common.EkContentCol contentdataAll = new Ektron.Cms.Common.EkContentCol();
			
			DeleteContentByCategoryToolBar();
			
			_PageData = new Collection();
			_PageData.Add(_Id, "FolderID", null, null);
			_PageData.Add(_OrderBy, "OrderBy", null, null);
			if (_ShowArchive == false)
			{
				contentdataAll = _EkContent.GetAllViewableChildContentInfoV5_0(_PageData, _CurrentPageId, _ContentApi.RequestInformationRef.PagingSize, ref _TotalPagesNumber);
			}
			else
			{
				contentdataAll = _EkContent.GetAllViewArchiveContentInfov5_0(_PageData, _CurrentPageId, _ContentApi.RequestInformationRef.PagingSize, ref _TotalPagesNumber);
			}
			
			Ektron.Cms.Common.EkContentCol filteredcontentdata = new Ektron.Cms.Common.EkContentCol();
			foreach (Ektron.Cms.Common.ContentBase item in contentdataAll)
			{
				if (item.ContentSubType != Ektron.Cms.Common.EkEnumeration.CMSContentSubtype.PageBuilderMasterData)
				{
					filteredcontentdata.Add(item);
				}
			}
			contentdataAll = filteredcontentdata;
			
			PageSettings();
			
			Populate_DeleteContentByCategory(contentdataAll);
			
			folder_id.Value =Convert.ToString(_Id);
			
		}
		private void Populate_DeleteContentByCategory(EkContentCol contentdata)
		{
			DeleteContentByGategoryGrid.Controls.Clear();
			contentids.Value = "";
			System.Web.UI.WebControls.BoundColumn colBound;
			colBound = new System.Web.UI.WebControls.BoundColumn();
			colBound.DataField = "BOX";
			colBound.HeaderText = "<input type=\"checkbox\" name=\"all\" onclick=\"javascript:checkAll(document.forms[0].all.checked);\">";
			colBound.ItemStyle.Width = Unit.Parse("1");
			colBound.ItemStyle.VerticalAlign = VerticalAlign.Top;
			colBound.ItemStyle.HorizontalAlign = HorizontalAlign.Center;
			colBound.HeaderStyle.CssClass = "title-header";
			DeleteContentByGategoryGrid.Columns.Add(colBound);
			
			colBound = new System.Web.UI.WebControls.BoundColumn();
			colBound.DataField = "TITLE";
			colBound.HeaderText = "<a class=\"title-header\" href=\"content.aspx?action=DeleteContentByCategory&orderby=Title&id=" + _Id + "&LangType=" + _ContentLanguage + "\" title=\"" + _MessageHelper.GetMessage("click to sort msg") + "\">" + _MessageHelper.GetMessage("generic Title") + "</a>";
			colBound.ItemStyle.VerticalAlign = VerticalAlign.Top;
			colBound.HeaderStyle.CssClass = "title-header";
			colBound.ItemStyle.Wrap = false;
			DeleteContentByGategoryGrid.Columns.Add(colBound);
			
			colBound = new System.Web.UI.WebControls.BoundColumn();
			colBound.DataField = "ID";
			colBound.HeaderText = "<a class=\"title-header\" href=\"content.aspx?action=DeleteContentByCategory&orderby=ID&id=" + _Id + "&LangType=" + _ContentLanguage + "\" title=\"" + _MessageHelper.GetMessage("click to sort msg") + "\">" + _MessageHelper.GetMessage("generic ID") + "</a>";
			colBound.HeaderStyle.CssClass = "title-header";
			colBound.ItemStyle.Wrap = false;
			colBound.ItemStyle.VerticalAlign = VerticalAlign.Top;
			colBound.ItemStyle.HorizontalAlign = HorizontalAlign.Center;
			DeleteContentByGategoryGrid.Columns.Add(colBound);
			
			colBound = new System.Web.UI.WebControls.BoundColumn();
			colBound.DataField = "STATUS";
			colBound.HeaderText = "<a class=\"title-header\" href=\"content.aspx?action=DeleteContentByCategory&orderby=status&id=" + _Id + "&LangType=" + _ContentLanguage + "\" title=\"" + _MessageHelper.GetMessage("click to sort msg") + "\">" + _MessageHelper.GetMessage("generic Status") + "</a>";
			colBound.ItemStyle.VerticalAlign = VerticalAlign.Top;
			colBound.HeaderStyle.CssClass = "title-header";
			colBound.ItemStyle.HorizontalAlign = HorizontalAlign.Center;
			colBound.ItemStyle.Wrap = false;
			DeleteContentByGategoryGrid.Columns.Add(colBound);
			
			colBound = new System.Web.UI.WebControls.BoundColumn();
			colBound.DataField = "DATEMODIFIED";
			colBound.HeaderText = "<a class=\"title-header\" href=\"content.aspx?action=DeleteContentByCategory&orderby=DateModified&id=" + _Id + "&LangType=" + _ContentLanguage + "\" title=\"" + _MessageHelper.GetMessage("click to sort msg") + "\">" + _MessageHelper.GetMessage("generic Date Modified") + "</a>";
			colBound.ItemStyle.VerticalAlign = VerticalAlign.Top;
			colBound.HeaderStyle.CssClass = "title-header";
			colBound.ItemStyle.Wrap = false;
			DeleteContentByGategoryGrid.Columns.Add(colBound);
			
			colBound = new System.Web.UI.WebControls.BoundColumn();
			colBound.DataField = "EDITORNAME";
			colBound.HeaderText = "<a class=\"title-header\" href=\"content.aspx?action=DeleteContentByCategory&orderby=editor&id=" + _Id + "&LangType=" + _ContentLanguage + "\" title=\"" + _MessageHelper.GetMessage("click to sort msg") + "\">" + _MessageHelper.GetMessage("generic Last Editor") + "</a>";
			colBound.ItemStyle.VerticalAlign = VerticalAlign.Top;
			colBound.HeaderStyle.CssClass = "title-header";
			colBound.ItemStyle.Wrap = false;
			DeleteContentByGategoryGrid.Columns.Add(colBound);
			
			
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
                
				dr = dt.NewRow();
				dr[0] = "";
                if ((contentdata.get_Item(i).ContentStatus == "A") || (contentdata.get_Item(i).ContentStatus == "I"))
				{
                    if (_EkContent.IsAllowed(contentdata.get_Item(i).Id, contentdata.get_Item(i).Language, "content", "delete", 0))
					{
						if (contentids.Value == "")
						{
                            contentids.Value = contentdata.get_Item(i).Id.ToString();
						}
						else
						{
                            contentids.Value += "," + contentdata.get_Item(i).Id;
						}
                        dr[0] = "<input type=\"checkbox\" onclick=\"javascript:checkAllFalse();\" name=\"id_" + contentdata.get_Item(i).Id + "\">";
					}
				}

                dr[1] = "<a href=\"content.aspx?LangType=" + contentdata.get_Item(i).Language + "&action=View&id=" + contentdata.get_Item(i).Id + "\" title=\'" + _MessageHelper.GetMessage("generic View") + " \"" + Strings.Replace(contentdata.get_Item(i).Title, "\'", "`", 1, -1, 0) + "\"" + "\'>" + contentdata.get_Item(i).Title + "</a>";
                dr[2] = contentdata.get_Item(i).Id;
                dr[3] = contentdata.get_Item(i).Status;
                dr[4] = contentdata.get_Item(i).DisplayDateModified;
                dr[5] = contentdata.get_Item(i).LastEditorLname;
				dt.Rows.Add(dr);
			}
			
			DataView dv = new DataView(dt);
			DeleteContentByGategoryGrid.DataSource = dv;
			DeleteContentByGategoryGrid.DataBind();
		}
		private void DeleteContentByCategoryToolBar()
		{
			System.Text.StringBuilder result = new System.Text.StringBuilder();

            txtTitleBar.InnerHtml = _StyleHelper.GetTitleBar(_MessageHelper.GetMessage("lbl remove content header") + " " + " \"" + _FolderData.Name + "\"");
			result.Append("<table></tr>");
			result.Append(_StyleHelper.GetButtonEventsWCaption(_AppImgPath + "../UI/Icons/back.png", (string)("content.aspx?LangType=" + _ContentLanguage + "&action=ViewContentByCategory&id=" + _Id), _MessageHelper.GetMessage("alt back button text"), _MessageHelper.GetMessage("btn back"), "", StyleHelper.BackButtonCssClass, true));
			if (_PermissionData.IsAdmin || _ContentApi.IsARoleMemberForFolder_FolderUserAdmin(_FolderData.Id, 0, false) || _EkContent.IsAllowed(_FolderData.Id, _ContentApi.RequestInformationRef.ContentLanguage, "folder", "delete", 0))
			{
				result.Append(_StyleHelper.GetButtonEventsWCaption(_AppImgPath + "../UI/Icons/delete.png", "#", _MessageHelper.GetMessage("btn delete content"), _MessageHelper.GetMessage("btn delete content"), "OnClick=\"javascript:checkDeleteForm();\"", StyleHelper.DeleteButtonCssClass, true));
			}
			result.Append(StyleHelper.ActionBarDivider);
			result.Append("<td>");
			result.Append(_StyleHelper.GetHelpButton((string) (_StyleHelper.GetHelpAliasPrefix(_FolderData) + _PageAction), ""));
			result.Append("</td>");
			result.Append("</tr></table>");
			htmToolBar.InnerHtml = result.ToString();
		}
		
		
		
		#endregion
		
		#endregion
		
		#region Paging
		
		
		private void PageSettings()
		{
			
			if (_TotalPagesNumber > 1)
			{
				this.SetPagingUI();
			}
			
			//If (_TotalPagesNumber <= 1) Then
			//    VisiblePageControls(False)
			//    page_id.Value = 1
			//Else
			//    VisiblePageControls(True)
			//    TotalPages.Text = (System.Math.Ceiling(_TotalPagesNumber)).ToString()
			//    'hTotalPages.Value = (System.Math.Ceiling(m_intTotalPages)).ToString()
			//    CurrentPage.Text = _CurrentPageId.ToString()
			//    'hCurrentPage.Value = m_intCurrentPage.ToString()
			//    ctrlPreviousPage.Enabled = True
			//    ctrlFirstPage.Enabled = True
			//    ctrlLastPage.Enabled = True
			//    ctrlNextPage.Enabled = True
			//    If _CurrentPageId = 1 Then
			//        ctrlPreviousPage.Enabled = False
			//        ctrlFirstPage.Enabled = False
			//    ElseIf _CurrentPageId = _TotalPagesNumber Then
			//        ctrlNextPage.Enabled = False
			//        ctrlLastPage.Enabled = False
			//    End If
			//    page_id.Value = _CurrentPageId
			//End If
		}
		
		private void SetPagingUI()
		{
			
			//paging ui
			divPaging.Visible = true;
			
			litPage.Text = "Page";
			CurrentPage.Text = _CurrentPageId == 0 ? "1" : (_CurrentPageId.ToString());
            CurrentPage.ToolTip = CurrentPage.Text;
			int previousPageIndex = System.Convert.ToInt32(_CurrentPageId <= 1 ? 1 : _CurrentPageId - 1);
			int nextPageIndex = System.Convert.ToInt32(_CurrentPageId >= _TotalPagesNumber ? _TotalPagesNumber : _CurrentPageId + 1);
			litOf.Text = "of";
			TotalPages.Text = _TotalPagesNumber.ToString();
			
			ibFirstPage.ImageUrl = _ContentApi.ApplicationPath + "/images/ui/icons/arrowheadFirst.png";
			ibFirstPage.AlternateText = "First Page";
			ibFirstPage.ToolTip = "First Page";
			ibFirstPage.OnClientClick = "GoToDeletePage(1, " + _TotalPagesNumber.ToString() + "); return false;";
			
			ibPreviousPage.ImageUrl = _ContentApi.ApplicationPath + "/images/ui/icons/arrowheadLeft.png";
			ibPreviousPage.AlternateText = "Previous Page";
			ibPreviousPage.ToolTip = "Previous Page";
			ibPreviousPage.OnClientClick = "GoToDeletePage(" + previousPageIndex.ToString() + ", " + _TotalPagesNumber.ToString() + "); return false;";
			
			ibNextPage.ImageUrl = _ContentApi.ApplicationPath + "/images/ui/icons/arrowheadRight.png";
			ibNextPage.AlternateText = "Next Page";
			ibNextPage.ToolTip = "Next Page";
			ibNextPage.OnClientClick = "GoToDeletePage(" + nextPageIndex.ToString() + ", " + _TotalPagesNumber.ToString() + "); return false;";
			
			ibLastPage.ImageUrl = _ContentApi.ApplicationPath + "/images/ui/icons/arrowheadLast.png";
			ibLastPage.AlternateText = "Last Page";
			ibLastPage.ToolTip = "Last Page";
			ibLastPage.OnClientClick = "GoToDeletePage(" + _TotalPagesNumber.ToString() + ", " + _TotalPagesNumber.ToString() + "); return false;";
			
			ibPageGo.ImageUrl = _ContentApi.ApplicationPath + "/images/ui/icons/forward.png";
			ibPageGo.AlternateText = "Go To Page";
			ibPageGo.ToolTip = "Go To Page";
			ibPageGo.OnClientClick = "GoToDeletePage(document.getElementById(\'" + this.CurrentPage.ClientID + "\').value, " + _TotalPagesNumber.ToString() + ");return false;";
			
		}
		
		private void VisiblePageControls(bool flag)
		{
			//TotalPages.Visible = flag
			//CurrentPage.Visible = flag
			//ctrlPreviousPage.Visible = flag
			//ctrlNextPage.Visible = flag
			//ctrlLastPage.Visible = flag
			//ctrlFirstPage.Visible = flag
			//PageLabel.Visible = flag
			//OfLabel.Visible = flag
		}
		protected void NavigationLink_Click(object sender, CommandEventArgs e)
		{
			switch (e.CommandName)
			{
				case "First":
					_CurrentPageId = 1;
					break;
				case "Last":
					_CurrentPageId = int.Parse((string) TotalPages.Text);
					break;
				case "Next":
					_CurrentPageId = System.Convert.ToInt32(int.Parse((string) CurrentPage.Text) + 1);
					break;
				case "Prev":
					_CurrentPageId = System.Convert.ToInt32(int.Parse((string) CurrentPage.Text) - 1);
					break;
			}
			Display_Delete();
			isPostData.Value = "true";
		}
		
		protected string Util_GetPageURL(int pageid)
		{
			
			return "content.aspx" + Ektron.Cms.Common.EkFunctions.GetUrl(new string[] {"currentpage"}, new string[] {"pageid"}, Request.QueryString).Replace("pageid", (string) (pageid == -1 ? "\' + pageid + \'" : pageid.ToString())).Replace("&amp;", "&");
			
		}
		
		#endregion
		
		#region JS/CSS
		
		private void RegisterJS()
		{
			
			JS.RegisterJS(this, JS.ManagedScript.EktronJS);
			JS.RegisterJS(this, JS.ManagedScript.EktronWorkareaJS);
			JS.RegisterJS(this, JS.ManagedScript.EktronWorkareaHelperJS);
            browseurljs.Text = "content.aspx?LangType=" + this._ContentLanguage + "&action=DeleteContentByCategory&id=" + this._Id + "&currentpage=";
			pagebetweenjs.Text = string.Format(_MessageHelper.GetMessage("js: err page must be between"), _TotalPagesNumber);
		}
		
		private void RegisterCSS()
		{
			Css.RegisterCss(this, Css.ManagedStyleSheet.EktronWorkareaCss);
			Css.RegisterCss(this, Css.ManagedStyleSheet.EktronWorkareaIeCss, Css.BrowserTarget.LessThanEqualToIE7);
		}
		
		#endregion
		
	}
	

