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
using System.IO;
	public partial class assigntaxonomy : System.Web.UI.UserControl
	{
		protected ContentAPI m_refContentApi = new ContentAPI();
		protected CommonApi m_refCommonApi = new CommonApi();
		protected StyleHelper m_refstyle = new StyleHelper();
		protected string AppImgPath = "";
		protected string AppPath = "";
		protected EkMessageHelper m_refMsg;
		protected string m_strPageAction = "";
		protected Ektron.Cms.Content.EkContent m_refContent;
		protected int TaxonomyLanguage = -1;
		protected long TaxonomyId = 0;
		protected long TaxonomyParentId = 0;
		protected LanguageData language_data;
		protected AssetInfoData[] asset_data;
		protected int SelectedContentType = -1;
		protected long FolderId = 0;
		protected Collection folder_data_col;
		protected string FolderName = "";
		protected string FolderPath = "";
		protected long FolderParentId = 0;
		protected Collection folder_request_col;
		protected string ContentIcon;
		protected string CalendarIcon;
		protected string pageIcon = "";
		protected string UserIcon;
		protected string FormsIcon = "";
		protected string m_selectedFolderList = "";
		protected EkEnumeration.CMSObjectTypes m_ObjectType = Ektron.Cms.Common.EkEnumeration.CMSObjectTypes.Content;
		protected EkEnumeration.UserTypes m_UserType = Ektron.Cms.Common.EkEnumeration.UserTypes.AuthorType;
		protected string m_strSelectedItem = "-1";
		protected string m_strKeyWords = "";
		protected string m_strSearchText = "";
		protected int m_intCurrentPage = 1;
		protected int m_intTotalPages = 1;
		protected string contentFetchType = "";
		// Protected user_list As DirectoryUserData() = Array.CreateInstance(GetType(Ektron.Cms.DirectoryUserData), 0)
		protected UserData[] user_list;
		protected CommunityGroupData[] cgroup_list;
		protected DirectoryAdvancedGroupData groupData = new DirectoryAdvancedGroupData();
		
		protected void Page_Load(object sender, System.EventArgs e)
		{
			m_refMsg = m_refContentApi.EkMsgRef;
			AppImgPath = m_refContentApi.AppImgPath;
			AppPath = m_refContentApi.AppPath;
			m_strPageAction = Request.QueryString["action"];
            object refApi = m_refContentApi as object;
            Utilities.SetLanguage(m_refContentApi);
			//Utilities.SetLanguage(m_refContentApi);
			TaxonomyLanguage = m_refContentApi.ContentLanguage;
			if (TaxonomyLanguage == -1)
			{
				TaxonomyLanguage = m_refContentApi.DefaultContentLanguage;
			}
			if (Request.QueryString["taxonomyid"] != null)
			{
				TaxonomyId = Convert.ToInt64(Request.QueryString["taxonomyid"]);
			}
			if (Request.QueryString["parentid"] != null)
			{
				TaxonomyParentId = Convert.ToInt64(Request.QueryString["parentid"]);
			}
			if ((Request.QueryString["type"] != null) && Request.QueryString["type"].ToLower() == "author")
			{
				m_ObjectType = Ektron.Cms.Common.EkEnumeration.CMSObjectTypes.User;
				m_UserType = Ektron.Cms.Common.EkEnumeration.UserTypes.AuthorType;
			}
			else if ((Request.QueryString["type"] != null) && Request.QueryString["type"].ToLower() == "member")
			{
				m_ObjectType = Ektron.Cms.Common.EkEnumeration.CMSObjectTypes.User;
				m_UserType = Ektron.Cms.Common.EkEnumeration.UserTypes.MemberShipType;
			}
			else if ((Request.QueryString["type"] != null) && Request.QueryString["type"].ToLower() == "cgroup")
			{
				m_ObjectType = Ektron.Cms.Common.EkEnumeration.CMSObjectTypes.CommunityGroup;
			}
			
			if ((Request.QueryString["contFetchType"] != null) && Request.QueryString["contFetchType"].ToLower() != "")
			{
				contentFetchType = Request.QueryString["contFetchType"];
			}
			m_refContent = m_refContentApi.EkContentRef;
			CalendarIcon = "<img src=\"" + m_refContentApi.AppPath + "images/UI/Icons/calendar.png\" alt=\"Calendar Event\">";
			FormsIcon = "<img src=\"" + m_refContentApi.AppPath + "images/UI/Icons/contentForm.png\" alt=\"Form\">";
			ContentIcon = "<img src=\"" + m_refContentApi.AppPath + "images/UI/Icons/contentHtml.png\" alt=\"Content\">";
			pageIcon = "<img src=\"" + m_refContentApi.AppPath + "images/ui/icons/layout.png\" alt=\"Page\">"; //-HC-
			if (this.m_UserType == Ektron.Cms.Common.EkEnumeration.UserTypes.AuthorType)
			{
				UserIcon = "<img src=\"" + m_refContentApi.AppPath + "Images/ui/icons/user.png\" alt=\"Content\">";
			}
			else
			{
				UserIcon = "<img src=\"" + m_refContentApi.AppPath + "Images/ui/icons/userMembership.png\" alt=\"Content\">";
			}
            if ((Page.IsPostBack && (!string.IsNullOrEmpty(Request.Form[isPostData.UniqueID])) && (m_ObjectType == EkEnumeration.CMSObjectTypes.Content | (m_ObjectType == EkEnumeration.CMSObjectTypes.User & !string.IsNullOrEmpty(Request.Form["itemlist"])) | (m_ObjectType == EkEnumeration.CMSObjectTypes.CommunityGroup & !string.IsNullOrEmpty(Request.Form["itemlist"]) | (m_ObjectType == EkEnumeration.CMSObjectTypes.Folder & !string.IsNullOrEmpty(Request.Form["itemlist"]))))))
			//if (Page.IsPostBack && (Request.Form[isPostData.UniqueID] != "") && (m_ObjectType == Ektron.Cms.Common.EkEnumeration.CMSObjectTypes.Content || (m_ObjectType == Ektron.Cms.Common.EkEnumeration.CMSObjectTypes.User && Request.Form["itemlist"] != "") || (m_ObjectType == Ektron.Cms.Common.EkEnumeration.CMSObjectTypes.CommunityGroup && Request.Form["itemlist"] != "")))
				{
				if (m_strPageAction == "additem")
				{
					TaxonomyRequest item_request = new TaxonomyRequest();
					item_request.TaxonomyId = TaxonomyId;
					item_request.TaxonomyIdList = Validate(Request.Form["itemlist"]);
					if (m_ObjectType == Ektron.Cms.Common.EkEnumeration.CMSObjectTypes.User)
					{
						item_request.TaxonomyItemType = Ektron.Cms.Common.EkEnumeration.TaxonomyItemType.User;
					}
					else if (m_ObjectType == Ektron.Cms.Common.EkEnumeration.CMSObjectTypes.CommunityGroup)
					{
						item_request.TaxonomyItemType = Ektron.Cms.Common.EkEnumeration.TaxonomyItemType.Group;
					}
					item_request.TaxonomyLanguage = TaxonomyLanguage;
					m_refContent.AddTaxonomyItem(item_request);
				}
				else if (m_strPageAction == "addfolder")
				{
					TaxonomySyncRequest sync_request = new TaxonomySyncRequest();
					sync_request.TaxonomyId = TaxonomyId;
					sync_request.SyncIdList = Validate(Request.Form["selectedfolder"]); //Validate(Request.Form("itemlist"))
					//sync_request.SyncRecursiveIdList = Validate(Request.Form("recursiveidlist"))
					sync_request.TaxonomyLanguage = TaxonomyLanguage;
					m_refContent.AddTaxonomySyncFolder(sync_request);
				}
				if (Request.QueryString["iframe"] == "true")
				{
					Response.Write("<script type=\"text/javascript\">parent.CloseChildPage();</script>");
				}
				else
				{
					Response.Redirect((string) ("taxonomy.aspx?action=view&taxonomyid=" + TaxonomyId));
				}
			}
			else
			{
				FolderId = Convert.ToInt64(Request.QueryString["folderid"]);
				
				folder_data_col = m_refContent.GetFolderInfoWithPath(FolderId);
                FolderName = folder_data_col["FolderName"].ToString();
                FolderParentId = Convert.ToInt64(folder_data_col["ParentID"].ToString());
                FolderPath = folder_data_col["Path"].ToString();
				folder_request_col = new Collection();
				folder_request_col.Add(FolderId, "ParentID", null, null);
				folder_request_col.Add("name", "OrderBy", null, null);
				folder_data_col = m_refContent.GetAllViewableChildFoldersv2_0(folder_request_col);
				
				if (m_strPageAction != "additem")
				{
					if (Request.QueryString[Ektron.Cms.Common.EkConstants.ContentTypeUrlParam] != "")
					{
						if (Information.IsNumeric(Request.QueryString[Ektron.Cms.Common.EkConstants.ContentTypeUrlParam]))
						{
							SelectedContentType = System.Convert.ToInt32(Request.QueryString[Ektron.Cms.Common.EkConstants.ContentTypeUrlParam]);
							m_refContentApi.SetCookieValue(Ektron.Cms.Common.EkConstants.ContentTypeUrlParam, SelectedContentType.ToString());
						}
					}
					else if (Ektron.Cms.CommonApi.GetEcmCookie()[Ektron.Cms.Common.EkConstants.ContentTypeUrlParam] != "")
					{
						if (Information.IsNumeric(Ektron.Cms.CommonApi.GetEcmCookie()[Ektron.Cms.Common.EkConstants.ContentTypeUrlParam]))
						{
							SelectedContentType = System.Convert.ToInt32(Ektron.Cms.CommonApi.GetEcmCookie()[Ektron.Cms.Common.EkConstants.ContentTypeUrlParam]);
						}
					}
					asset_data = m_refContent.GetAssetSuperTypes();
				}
				RegisterResources();
				TaxonomyToolBar();
				if (! Page.IsPostBack || m_ObjectType == Ektron.Cms.Common.EkEnumeration.CMSObjectTypes.User || m_ObjectType == Ektron.Cms.Common.EkEnumeration.CMSObjectTypes.CommunityGroup)
				{
					// avoid redisplay when clicking next/prev buttons
					DisplayPage();
				}
			}
		}
		
		protected void Page_PreRender(object sender, System.EventArgs e)
		{
			PageSettings();
		}
		
		private void DisplayPage()
		{
			if (m_strPageAction != "addfolder")
			{
				PopulateGridData();
			}
			else
			{
				TaxonomyFolderSyncData[] taxonomy_sync_folder = null;
				TaxonomyBaseRequest tax_sync_folder_req = new TaxonomyBaseRequest();
				tax_sync_folder_req.TaxonomyId = TaxonomyId;
				tax_sync_folder_req.TaxonomyLanguage = TaxonomyLanguage;
				taxonomy_sync_folder = m_refContent.GetAllAssignedCategoryFolder(tax_sync_folder_req);
				if ((taxonomy_sync_folder != null)&& taxonomy_sync_folder.Length > 0)
				{
					for (int cnt = 0; cnt <= taxonomy_sync_folder.Length - 1; cnt++)
					{
						if (m_selectedFolderList.Length > 0)
						{
                            m_selectedFolderList = m_selectedFolderList + "," + taxonomy_sync_folder[cnt].FolderId.ToString();
						}
						else
						{
                            m_selectedFolderList = taxonomy_sync_folder[cnt].FolderId.ToString();
						}
					}
				}
				// add to the body's onload event to load up the folder tree
			}
		}
		
		private string Validate(string value)
		{
			if (value != null)
			{
				return value;
			}
			else
			{
				return "";
			}
		}
		private void PopulateGridData()
		{
			if (TaxonomyItemList.Columns.Count == 0)
			{
				TaxonomyItemList.Columns.Add(m_refstyle.CreateBoundField("ITEM1", "", "info", HorizontalAlign.NotSet, HorizontalAlign.NotSet, Unit.Percentage(0), Unit.Percentage(0), false, false));
			}
			
			string iframe = "";
			if ((Request.QueryString["iframe"] != null)&& Request.QueryString["iframe"] != "")
			{
				iframe = "&iframe=true";
			}
			DataTable dt = new DataTable();
			DataRow dr;
			dt.Columns.Add(new DataColumn("ITEM1", typeof(string)));
			
			dr = dt.NewRow();
			if ((m_strPageAction == "additem") && this.m_ObjectType == Ektron.Cms.Common.EkEnumeration.CMSObjectTypes.User)
			{
				dr[0] = m_refMsg.GetMessage("lbl select users") + "<br/>";
			}
			else if ((m_strPageAction == "additem") && this.m_ObjectType == Ektron.Cms.Common.EkEnumeration.CMSObjectTypes.CommunityGroup)
			{
				dr[0] = m_refMsg.GetMessage("lbl select cgroups") + "<br/>";
			}
			else if (m_strPageAction == "additem")
			{
				dr[0] = m_refMsg.GetMessage("assigntaxonomyitemlabel") + "<br/>";
			}
			else
			{
				dr[0] = m_refMsg.GetMessage("assigntaxonomyfolderlabel") + "<br/>";
			}
			dt.Rows.Add(dr);
			
			if (this.m_ObjectType == Ektron.Cms.Common.EkEnumeration.CMSObjectTypes.Content)
			{
				dr = dt.NewRow();
				dr[0] = m_refMsg.GetMessage("generic Path") + FolderPath;
				dt.Rows.Add(dr);
				
				dr = dt.NewRow();
				if (FolderId != 0)
				{
					dr[0] = "<a href=\"taxonomy.aspx?action=" + m_strPageAction + "&taxonomyid=" + TaxonomyId + "&folderid=" + FolderParentId + "&parentid=" + FolderParentId + iframe;
					dr[0] = dr[0] + "&title=\"" + m_refMsg.GetMessage("alt: generic previous dir text") + "\"><img src=\"" + m_refContentApi.AppPath + "images/ui/icons/folderUp.png" + "\" border=\"0\" title=\"" + m_refMsg.GetMessage("alt: generic previous dir text") + "\" alt=\"" + m_refMsg.GetMessage("alt: generic previous dir text") + "\" align=\"absbottom\">..</a>";
				}
				dt.Rows.Add(dr);
				
				
				if (folder_data_col != null)
				{
					foreach (Collection folder in folder_data_col)
					{
						dr = dt.NewRow();
						dr[0] = "<a href=\"taxonomy.aspx?action=" + m_strPageAction + "&taxonomyid=" + TaxonomyId + "&folderid=" + folder["id"] + "&parentid=" + FolderParentId + iframe;
						dr[0] += "&title=\"" + m_refMsg.GetMessage("alt: generic view folder content text") + "\"><img src=\"";
                        switch ((EkEnumeration.FolderType)Convert.ToInt32(folder["FolderType"]))
						{
							case EkEnumeration.FolderType.Catalog:
								dr[0] += m_refContentApi.AppPath + "images/ui/icons/folderGreen.png";
								break;
							case EkEnumeration.FolderType.Community:
								dr[0] += m_refContentApi.AppPath + "images/ui/icons/folderCommunity.png";
								break;
							case EkEnumeration.FolderType.Blog:
								dr[0] += m_refContentApi.AppPath + "images/ui/icons/folderBlog.png";
								break;
							case EkEnumeration.FolderType.DiscussionBoard:
								dr[0] += m_refContentApi.AppPath + "images/ui/icons/folderBoard.png";
								break;
							case EkEnumeration.FolderType.DiscussionForum:
								dr[0] += m_refContentApi.AppPath + "images/ui/icons/folderBoard.png";
								break;
							case EkEnumeration.FolderType.Calendar:
								dr[0] += m_refContentApi.AppPath + "images/ui/icons/folderCalendar.png";
								break;
							case EkEnumeration.FolderType.Domain:
								dr[0] += m_refContentApi.AppPath + "images/ui/icons/foldersite.png";
								break;
							default:
								dr[0] += m_refContentApi.AppPath + "images/ui/icons/folder.png";
								break;
						}
						dr[0] += "\" border=\"0\" title=\"" + m_refMsg.GetMessage("alt: generic view folder content text") + "\" alt=\"" + m_refMsg.GetMessage("alt: generic view folder content text") + "\" align=\"absbottom\"></a> ";
						dr[0] += "<a href=\"taxonomy.aspx?action=" + m_strPageAction + "&taxonomyid=" + TaxonomyId + "&folderid=" + folder["id"] + "&parentid=" + FolderParentId + iframe + ("&title=\"" + m_refMsg.GetMessage("alt: generic view folder content text")) + "\">" + folder["Name"] + "</a>";
						dt.Rows.Add(dr);
					}
				}
				if (m_strPageAction == "additem")
				{
					ContentData[] taxonomy_unassigneditem_arr;
					TaxonomyRequest request = new TaxonomyRequest();
					request.TaxonomyId = TaxonomyId;
					request.TaxonomyLanguage = TaxonomyLanguage;
					request.FolderId = FolderId;
					if (contentFetchType.ToLower() == "activecontent")
					{
						request.TaxonomyItemType = Ektron.Cms.Common.EkEnumeration.TaxonomyItemType.ActiveContent;
					}
                    else if (contentFetchType.ToLower() == "library")
                    {
                        request.TaxonomyItemType = Ektron.Cms.Common.EkEnumeration.TaxonomyItemType.Library;
                    }
					else if (contentFetchType.ToLower() == "archivedcontent")
					{
						request.TaxonomyItemType = Ektron.Cms.Common.EkEnumeration.TaxonomyItemType.ArchivedContent;
					}
					else
					{
						request.TaxonomyItemType = 0;
					}
					
					// get total #pages first because the API doesn't return it (lame slow way to do this)-:
					request.PageSize = 99999999;
					request.CurrentPage = 1;
					taxonomy_unassigneditem_arr = m_refContent.ReadAllUnAssignedTaxonomyItems(request);
                    m_intTotalPages = System.Convert.ToInt32(Math.Truncate(System.Convert.ToDecimal((taxonomy_unassigneditem_arr.Length + (m_refContentApi.RequestInformationRef.PagingSize - 1)) / m_refContentApi.RequestInformationRef.PagingSize)));
					
					// get the real page data set
                    request.PageSize = m_refContentApi.RequestInformationRef.PagingSize;
					request.CurrentPage = m_intCurrentPage;
					taxonomy_unassigneditem_arr = m_refContent.ReadAllUnAssignedTaxonomyItems(request);

                    if (request.TaxonomyItemType == Ektron.Cms.Common.EkEnumeration.TaxonomyItemType.Library)
                    {
                        LibraryData library_dat;

                        foreach (ContentData taxonomy_item in taxonomy_unassigneditem_arr)
                        {
                            if (taxonomy_item.Type == 7)
                            {
                                dr = dt.NewRow();
                                library_dat = m_refContentApi.GetLibraryItemByContentID(taxonomy_item.Id);
                                string extension = "";
                                extension = System.IO.Path.GetExtension(library_dat.FileName);
                                switch (extension.ToLower())
                                {
                                    case ".doc":
                                        dr[0] = "&nbsp;&nbsp;&nbsp;&nbsp;<input type=\"checkbox\" id=\"itemlist\" name=\"itemlist\" value=\"" + taxonomy_item.Id + "\"/>&nbsp;<img src=\"" + m_refContentApi.AppPath + "images/ui/icons/FileTypes/word.png" + "\" alt=\"" + taxonomy_item.AssetData.FileName + "\"></img>&nbsp;" + taxonomy_item.Title;
                                        break;
                                    case ".ppt":
                                        dr[0] = "&nbsp;&nbsp;&nbsp;&nbsp;<input type=\"checkbox\" id=\"itemlist\" name=\"itemlist\" value=\"" + taxonomy_item.Id + "\"/>&nbsp;<img src=\"" + m_refContentApi.AppPath + "images/ui/icons/FileTypes/powerpoint.png" + "\" alt=\"" + taxonomy_item.AssetData.FileName + "\"></img>&nbsp;" + taxonomy_item.Title;
                                        break;
                                    case ".pdf":
                                        dr[0] = "&nbsp;&nbsp;&nbsp;&nbsp;<input type=\"checkbox\" id=\"itemlist\" name=\"itemlist\" value=\"" + taxonomy_item.Id + "\"/>&nbsp;<img src=\"" + m_refContentApi.AppPath + "images/ui/icons/FileTypes/acrobat.png" + "\" alt=\"" + taxonomy_item.AssetData.FileName + "\"></img>&nbsp;" + taxonomy_item.Title;
                                        break;
                                    case ".xls":
                                        dr[0] = "&nbsp;&nbsp;&nbsp;&nbsp;<input type=\"checkbox\" id=\"itemlist\" name=\"itemlist\" value=\"" + taxonomy_item.Id + "\"/>&nbsp;<img src=\"" + m_refContentApi.AppPath + "images/ui/icons/FileTypes/excel.png" + "\" alt=\"" + taxonomy_item.AssetData.FileName + "\"></img>&nbsp;" + taxonomy_item.Title;
                                        break;
                                    case ".jpg":
                                    case ".jpeg":
                                    case ".png":
                                    case ".gif":
                                    case ".bmp":
                                        dr[0] = "&nbsp;&nbsp;&nbsp;&nbsp;<input type=\"checkbox\" id=\"itemlist\" name=\"itemlist\" value=\"" + taxonomy_item.Id + "\"/>&nbsp;<img src=\"" + m_refContentApi.AppPath + "images/ui/icons/FileTypes/image.png" + "\" alt=\"" + taxonomy_item.AssetData.FileName + "\"></img>&nbsp;" + taxonomy_item.Title;
                                        break;
                                    default: // other files
                                        dr[0] = "&nbsp;&nbsp;&nbsp;&nbsp;<input type=\"checkbox\" id=\"itemlist\" name=\"itemlist\" value=\"" + taxonomy_item.Id + "\"/>&nbsp;<img src=\"" + m_refContentApi.AppPath + "images/UI/Icons/book.png" + "\" alt=\"" + taxonomy_item.AssetData.FileName + "\"></img>&nbsp;" + taxonomy_item.Title;
                                        break;

                                }
                                dt.Rows.Add(dr);
                            }
                        }

                    }
                    else
                    {
                        foreach (ContentData taxonomy_item in taxonomy_unassigneditem_arr)
                        {
                            dr = dt.NewRow();
                           
                            if (taxonomy_item.Type == 1 || taxonomy_item.Type == 2)
                            {
                                dr[0] = "&nbsp;&nbsp;&nbsp;&nbsp;<input type=\"checkbox\" id=\"itemlist\" name=\"itemlist\" value=\"" + taxonomy_item.Id + "\"/>&nbsp;" + GetTypeIcon(taxonomy_item.Type, taxonomy_item.SubType) + "&nbsp;" + taxonomy_item.Title;
                            }
                            else if (taxonomy_item.Type == 3)
                            {
                                dr[0] = "&nbsp;&nbsp;&nbsp;&nbsp;<input type=\"checkbox\" id=\"itemlist\" name=\"itemlist\" value=\"" + taxonomy_item.Id + "\"/>&nbsp;<img src=\"" + m_refContentApi.AppPath + "Images/ui/icons/contentArchived.png" + "\"&nbsp;border=\"0\"  alt=\"" + taxonomy_item.AssetData.FileName + "\"></img>&nbsp;" + taxonomy_item.Title;
                            }
                            else if (taxonomy_item.Type == 1111)
                            {
                                dr[0] = "&nbsp;&nbsp;&nbsp;&nbsp;<input type=\"checkbox\" id=\"itemlist\" name=\"itemlist\" value=\"" + taxonomy_item.Id + "\"/>&nbsp;<img src=\"" + m_refContentApi.AppPath + "images/ui/icons/asteriskOrange.png" + "\" alt=\"" + taxonomy_item.AssetData.FileName + "\"></img>&nbsp;" + taxonomy_item.Title;
                            }
                            else if (taxonomy_item.Type == 1112)
                            {
                                dr[0] = "&nbsp;&nbsp;&nbsp;&nbsp;<input type=\"checkbox\" id=\"itemlist\" name=\"itemlist\" value=\"" + taxonomy_item.Id + "\"/>&nbsp;<img src=\"" + m_refContentApi.AppPath + "images/ui/icons/tree/folderBlog.png" + "\" alt=\"" + taxonomy_item.AssetData.FileName + "\"></img>&nbsp;" + taxonomy_item.Title;
                            }
                            
                            else if (taxonomy_item.Type == 3333)
                            {
                                dr[0] = "&nbsp;&nbsp;&nbsp;&nbsp;<input type=\"checkbox\" id=\"itemlist\" name=\"itemlist\" value=\"" + taxonomy_item.Id + "\"/>&nbsp;<img src=\"" + m_refContentApi.AppPath + "Images/ui/icons/brick.png" + "\" alt=\"" + taxonomy_item.AssetData.FileName + "\"></img>&nbsp;" + taxonomy_item.Title;
                            }
                            else if (taxonomy_item.AssetData.ImageUrl == "" && (taxonomy_item.Type != 1 && taxonomy_item.Type != 2 && taxonomy_item.Type != 3 && taxonomy_item.Type != 1111 && taxonomy_item.Type != 1112 && taxonomy_item.Type != 3333))
                            {
                                dr[0] = "&nbsp;&nbsp;&nbsp;&nbsp;<input type=\"checkbox\" id=\"itemlist\" name=\"itemlist\" value=\"" + taxonomy_item.Id + "\"/>&nbsp;<img src=\"" + m_refContentApi.AppPath + "images/UI/Icons/book.png" + "\" alt=\"" + taxonomy_item.AssetData.FileName + "\"></img>&nbsp;" + taxonomy_item.Title;
                            }
                            else
                            {
                                //Bad Approach however no other way untill AssetManagement/Images/ are updated with version 8 images or DMS points to workarea images
                                if (taxonomy_item.AssetData.ImageUrl == "")
                                {
                                    dr[0] = "&nbsp;&nbsp;&nbsp;&nbsp;<input type=\"checkbox\" id=\"itemlist\" name=\"itemlist\" value=\"" + taxonomy_item.Id + "\"/>&nbsp;<img src=\"" + m_refContentApi.AppPath + "images/UI/Icons/book.png" + "\" alt=\"" + taxonomy_item.AssetData.FileName + "\"></img>&nbsp;" + taxonomy_item.Title;
                                }
                                else
                                {
                                    if ((string)(Path.GetFileName(taxonomy_item.AssetData.ImageUrl).ToLower()) == "ms-word.gif")
                                    {
                                        dr[0] = "&nbsp;&nbsp;&nbsp;&nbsp;<input type=\"checkbox\" id=\"itemlist\" name=\"itemlist\" value=\"" + taxonomy_item.Id + "\"/>&nbsp;<img src=\"" + m_refContentApi.AppPath + "images/ui/icons/FileTypes/word.png" + "\" alt=\"" + taxonomy_item.AssetData.FileName + "\"></img>&nbsp;" + taxonomy_item.Title;
                                    }
                                    else if ((string)(Path.GetFileName(taxonomy_item.AssetData.ImageUrl).ToLower()) == "ms-excel.gif")
                                    {
                                        dr[0] = "&nbsp;&nbsp;&nbsp;&nbsp;<input type=\"checkbox\" id=\"itemlist\" name=\"itemlist\" value=\"" + taxonomy_item.Id + "\"/>&nbsp;<img src=\"" + m_refContentApi.AppPath + "images/ui/icons/FileTypes/excel.png" + "\" alt=\"" + taxonomy_item.AssetData.FileName + "\"></img>&nbsp;" + taxonomy_item.Title;
                                    }
                                    else if ((string)(Path.GetFileName(taxonomy_item.AssetData.ImageUrl).ToLower()) == "ms-powerpoint.gif")
                                    {
                                        dr[0] = "&nbsp;&nbsp;&nbsp;&nbsp;<input type=\"checkbox\" id=\"itemlist\" name=\"itemlist\" value=\"" + taxonomy_item.Id + "\"/>&nbsp;<img src=\"" + m_refContentApi.AppPath + "images/ui/icons/FileTypes/powerpoint.png" + "\" alt=\"" + taxonomy_item.AssetData.FileName + "\"></img>&nbsp;" + taxonomy_item.Title;
                                    }
                                    else if ((string)(Path.GetFileName(taxonomy_item.AssetData.ImageUrl).ToLower()) == "adobe-pdf.gif")
                                    {
                                        dr[0] = "&nbsp;&nbsp;&nbsp;&nbsp;<input type=\"checkbox\" id=\"itemlist\" name=\"itemlist\" value=\"" + taxonomy_item.Id + "\"/>&nbsp;<img src=\"" + m_refContentApi.AppPath + "images/ui/icons/FileTypes/acrobat.png" + "\" alt=\"" + taxonomy_item.AssetData.FileName + "\"></img>&nbsp;" + taxonomy_item.Title;
                                    }
                                    else if ((string)(Path.GetFileName(taxonomy_item.AssetData.ImageUrl).ToLower()) == "image.gif")
                                    {
                                        dr[0] = "&nbsp;&nbsp;&nbsp;&nbsp;<input type=\"checkbox\" id=\"itemlist\" name=\"itemlist\" value=\"" + taxonomy_item.Id + "\"/>&nbsp;<img src=\"" + m_refContentApi.AppPath + "images/ui/icons/FileTypes/image.png" + "\" alt=\"" + taxonomy_item.AssetData.FileName + "\"></img>&nbsp;" + taxonomy_item.Title;
                                    }
                                    else
                                    {
                                        dr[0] = "&nbsp;&nbsp;&nbsp;&nbsp;<input type=\"checkbox\" id=\"itemlist\" name=\"itemlist\" value=\"" + taxonomy_item.Id + "\"/>&nbsp;<img src=\"" + taxonomy_item.AssetData.ImageUrl + "\" alt=\"" + taxonomy_item.AssetData.FileName + "\"></img>&nbsp;" + taxonomy_item.Title;
                                    }
                                }
                            }
                            dt.Rows.Add(dr);
                        }
                    }
				}
			}
			else if (this.m_ObjectType == Ektron.Cms.Common.EkEnumeration.CMSObjectTypes.CommunityGroup)
			{
				CollectSearchText();
				dr = dt.NewRow();
				dr[0] = "<input type=text size=25 id=\"txtSearch\" name=\"txtSearch\" value=\"" + m_strKeyWords + "\" onkeydown=\"CheckForReturn(event)\">";
				dr[0] += "<input type=button value=\"Search\" id=\"btnSearch\" name=\"btnSearch\"  class=\"ektronWorkareaSearch\" onclick=\"searchuser();\" title=\"Search Users\">";
				dt.Rows.Add(dr);
				GetAssignedCommunityGroups();
				GetCommunityGroups();
				if (cgroup_list != null)
				{
					for (int j = 0; j <= (cgroup_list.Length - 1); j++)
					{
						
						dr = dt.NewRow();
						if (DoesGroupExistInList(cgroup_list[j].GroupId))
						{
							dr[0] = "&nbsp;&nbsp;&nbsp;&nbsp;" + GetTypeIcon(Ektron.Cms.Common.EkEnumeration.CMSObjectTypes.User.GetHashCode(), Ektron.Cms.Common.EkEnumeration.CMSContentSubtype.Content) + "<input type=\"checkbox\" checked=\"checked\" disabled=\"disabled\" id=\"itemlistNoId\" name=\"itemlistNoId\" value=\"" + cgroup_list[j].GroupId + "\"/>" + cgroup_list[j].GroupName;
						}
						else
						{
							dr[0] = "&nbsp;&nbsp;&nbsp;&nbsp;" + GetTypeIcon(Ektron.Cms.Common.EkEnumeration.CMSObjectTypes.User.GetHashCode(), Ektron.Cms.Common.EkEnumeration.CMSContentSubtype.Content) + "<input type=\"checkbox\" id=\"itemlist\" name=\"itemlist\" value=\"" + cgroup_list[j].GroupId + "\"/>" + cgroup_list[j].GroupName;
						}
						
						dt.Rows.Add(dr);
					}
				}
			}
			else if (this.m_ObjectType == Ektron.Cms.Common.EkEnumeration.CMSObjectTypes.User)
			{
				CollectSearchText();
				dr = dt.NewRow();
				dr[0] = "<input type=text size=25 id=\"txtSearch\" name=\"txtSearch\" value=\"" + m_strKeyWords + "\" onkeydown=\"CheckForReturn(event)\">";
				dr[0] += "<select id=\"searchlist\" name=\"searchlist\">";
				dr[0] += "<option value=-1" + IsSelected("-1") + ">All</option>";
				dr[0] += "<option value=\"last_name\"" + IsSelected("last_name") + ">Last Name</option>";
				dr[0] += "<option value=\"first_name\"" + IsSelected("first_name") + ">First Name</option>";
				dr[0] += "<option value=\"user_name\"" + IsSelected("user_name") + ">User Name</option>";
				dr[0] += "</select><input type=button value=\"Search\" id=\"btnSearch\" name=\"btnSearch\" class=\"ektronWorkareaSearch\"  onclick=\"searchuser();\" title=\"Search Users\">";
				dt.Rows.Add(dr);
				GetUsers();
				if (user_list != null)
				{
					for (int j = 0; j <= (user_list.Length - 1); j++)
					{
						dr = dt.NewRow();
						dr[0] = "&nbsp;&nbsp;&nbsp;&nbsp;" + GetTypeIcon(Ektron.Cms.Common.EkEnumeration.CMSObjectTypes.User.GetHashCode(), Ektron.Cms.Common.EkEnumeration.CMSContentSubtype.Content) + "<input type=\"checkbox\" id=\"itemlist\" name=\"itemlist\" value=\"" + user_list[j].Id + "\"/>" + ((user_list[j].DisplayName != "") ? (user_list[j].DisplayName) : (user_list[j].Username));
						dt.Rows.Add(dr);
					}
				}
			}
			DataView dv = new DataView(dt);
			TaxonomyItemList.DataSource = dv;
			TaxonomyItemList.DataBind();
		}
		private void GetUsers()
		{
			if (m_strSearchText.Trim() != "")
			{
				UserRequestData req = new UserRequestData();
				UserAPI m_refUserApi = new UserAPI();
				req.Type = System.Convert.ToInt32(this.m_UserType == Ektron.Cms.Common.EkEnumeration.UserTypes.AuthorType ? 0 : 1); // m_intGroupType
				req.Group = this.m_UserType == Ektron.Cms.Common.EkEnumeration.UserTypes.AuthorType ? 2 : 888888;
				req.RequiredFlag = 0;
				req.OrderBy = "";
				req.OrderDirection = "asc";
				req.SearchText = m_strSearchText;
				req.PageSize = m_refContentApi.RequestInformationRef.PagingSize;
				req.CurrentPage = m_intCurrentPage;
				user_list = m_refUserApi.GetAllUsers(ref req);
				// user_list = Me.m_refContent.GetAllUnAssignedDirectoryUser(TaxonomyId)
				m_intTotalPages = req.TotalPages;
			}
		}
		private void GetAssignedCommunityGroups()
		{
			if (Page.IsPostBack)
			{
				DirectoryGroupRequest cReq = new DirectoryGroupRequest();
				cReq.CurrentPage = m_intCurrentPage;
				cReq.PageSize = m_refCommonApi.RequestInformationRef.PagingSize;
				cReq.DirectoryId = TaxonomyId;
				cReq.DirectoryLanguage = TaxonomyLanguage;
				cReq.GetItems = true;
				cReq.SortDirection = "";
				groupData = m_refCommonApi.CommunityGroupRef.LoadDirectory(ref cReq);
			}
		}
		private bool DoesGroupExistInList(long GroupID)
		{
			if ((groupData != null)&& (groupData.DirectoryItems != null)&& groupData.DirectoryItems.Length > 0)
			{
				foreach (CommunityGroupData _gData in groupData.DirectoryItems)
				{
					if (_gData.GroupId == GroupID)
					{
						return true;
					}
				}
			}
			return false;
		}
		
		private void GetCommunityGroups()
		{
			if (Page.IsPostBack)
			{
				CommunityGroupRequest cReq = new CommunityGroupRequest();
				cReq.CurrentPage = m_intCurrentPage;
				cReq.SearchText = m_strKeyWords;
				cReq.PageSize = m_refContentApi.RequestInformationRef.PagingSize;
				cgroup_list = (new Ektron.Cms.Community.CommunityGroupAPI()).GetAllCommunityGroups(cReq);
				m_intTotalPages = cReq.TotalPages;
			}
		}
		private void CollectSearchText()
		{
			m_strKeyWords = Request.Form["txtSearch"];
			m_strSelectedItem = Request.Form["searchlist"];
			if (m_strSelectedItem == "-1")
			{
				m_strSearchText = " (first_name like \'%" + Quote(m_strKeyWords) + "%\' OR last_name like \'%" + Quote(m_strKeyWords) + "%\' OR user_name like \'%" + Quote(m_strKeyWords) + "%\') AND u.user_id not in (select taxonomy_item_id from taxonomy_item_tbl where taxonomy_item_type=1 and taxonomy_id=" + TaxonomyId + ")";
			}
			else if (m_strSelectedItem == "last_name")
			{
				m_strSearchText = " (last_name like \'%" + Quote(m_strKeyWords) + "%\') AND u.user_id not in (select taxonomy_item_id from taxonomy_item_tbl where taxonomy_item_type=1 and taxonomy_id=" + TaxonomyId + ")";
			}
			else if (m_strSelectedItem == "first_name")
			{
				m_strSearchText = " (first_name like \'%" + Quote(m_strKeyWords) + "%\') AND u.user_id not in (select taxonomy_item_id from taxonomy_item_tbl where taxonomy_item_type=1 and taxonomy_id=" + TaxonomyId + ")";
			}
			else if (m_strSelectedItem == "user_name")
			{
				m_strSearchText = " (user_name like \'%" + Quote(m_strKeyWords) + "%\') AND u.user_id not in (select taxonomy_item_id from taxonomy_item_tbl where taxonomy_item_type=1 and taxonomy_id=" + TaxonomyId + ")";
			}
		}
		private string PopulateFolderRecursive(TaxonomyFolderSyncData[] data, long id)
		{
			string result = "";
			foreach (TaxonomyFolderSyncData item in data)
			{
				if (id == item.FolderId)
				{
					result = result + "<div id=\"_dv" + id + "\" style=\"position:relative;display:block;\">";
					result = result + "<span id=\"spacechk\">&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;</span>";
					result = result + "<span id=\"spanchk\">Include subfolder(s).</span>";
					result = result + "</div>";
					break;
				}
			}
			if (result == "")
			{
				result = result + "<div id=\"_dv" + id + "\" style=\"position:relative;display:none;\"></div>";
			}
			return result;
		}
		private string Checked(TaxonomyFolderSyncData[] data, long id)
		{
			string result = "";
			foreach (TaxonomyFolderSyncData item in data)
			{
				if (id == item.FolderId)
				{
					result = " checked ";
					break;
				}
			}
			return result;
		}
		private string Checked(bool value)
		{
			if (value)
			{
				return "checked";
			}
			else
			{
				return "";
			}
		}
		private string OnClickEvent(object id)
		{
			string result = "";
			if (m_strPageAction != "additem")
			{
				result = " onclick=\"OnFolderCheck(" + id + ",this);\"";
			}
			return result;
		}
		private string GetTypeIcon(int type, EkEnumeration.CMSContentSubtype subType)
		{
			if (type == Ektron.Cms.Common.EkEnumeration.CMSObjectTypes.User.GetHashCode() && this.m_ObjectType == Ektron.Cms.Common.EkEnumeration.CMSObjectTypes.User)
			{
				return UserIcon;
			}
			else if (type == 2 && this.m_ObjectType == Ektron.Cms.Common.EkEnumeration.CMSObjectTypes.Content)
			{
				return FormsIcon;
			}
			else if (type == 1)
			{
				if (subType == Ektron.Cms.Common.EkEnumeration.CMSContentSubtype.PageBuilderData || subType == Ektron.Cms.Common.EkEnumeration.CMSContentSubtype.PageBuilderMasterData)
				{
					return pageIcon;
				}
				if (subType == EkEnumeration.CMSContentSubtype.WebEvent)
                {
                    return CalendarIcon;
                }
				return ContentIcon;
			}
			else
			{
				return null;
			}
		}
		
		private void TaxonomyToolBar()
		{
			if (m_strPageAction != "additem")
			{
				divTitleBar.InnerHtml = m_refstyle.GetTitleBar(m_refMsg.GetMessage("assign folders to taxonomy page title"));
			}
			else
			{
				divTitleBar.InnerHtml = m_refstyle.GetTitleBar(m_refMsg.GetMessage("assign items to taxonomy page title"));
			}
			
			System.Text.StringBuilder result = new System.Text.StringBuilder();
			result.Append("<table><tr>" + "\r\n");

			if (Request.QueryString["iframe"] == "true")
			{
				result.Append(m_refstyle.GetButtonEventsWCaption(AppPath + "images/UI/Icons/cancel.png", "#", m_refMsg.GetMessage("generic Cancel"), m_refMsg.GetMessage("generic Cancel"), "onclick=\"parent.CancelIframe();\"", StyleHelper.CancelButtonCssClass, true));
			}
			else
			{
				result.Append(m_refstyle.GetButtonEventsWCaption(AppPath + "images/UI/Icons/back.png", (string)("taxonomy.aspx?action=view&taxonomyid=" + TaxonomyId + "&LangType=" + m_refContentApi.RequestInformationRef.ContentLanguage), m_refMsg.GetMessage("alt back button text"), m_refMsg.GetMessage("btn back"), "", StyleHelper.BackButtonCssClass, true));
			}

			result.Append(m_refstyle.GetButtonEventsWCaption(AppPath + "images/UI/Icons/save.png", "#", m_refMsg.GetMessage("alt update button text (taxonomy)"), m_refMsg.GetMessage("btn update"), "onclick=\"Validate();\"", StyleHelper.SaveButtonCssClass, true));
			
			if (m_strPageAction == "additem")
			{
				result.Append(StyleHelper.ActionBarDivider);
				
				result.Append("<td class=\"label\">");
				result.Append(m_refMsg.GetMessage("view language"));
				result.Append("&nbsp;</td>");
				result.Append("<td><select id=\"typelist\" name=\"typelist\" OnChange=\"ChangeView();\">");
				result.Append("<option value=\'" +(int) Ektron.Cms.Common.EkEnumeration.CMSObjectTypes.Content + "\' " + (this.m_ObjectType == Ektron.Cms.Common.EkEnumeration.CMSObjectTypes.Content ? "selected" : "") + ">").Append(this.m_refMsg.GetMessage("content button text")).Append("</option>");
                result.Append("<option value=\'" + (int)Ektron.Cms.Common.EkEnumeration.CMSObjectTypes.User + (int)Ektron.Cms.Common.EkEnumeration.UserTypes.AuthorType + "\' " + (this.m_ObjectType == Ektron.Cms.Common.EkEnumeration.CMSObjectTypes.User && this.m_UserType == Ektron.Cms.Common.EkEnumeration.UserTypes.AuthorType ? "selected" : "") + ">").Append(this.m_refMsg.GetMessage("lbl cms authors")).Append("</option>");
                result.Append("<option value=\'" + (int)Ektron.Cms.Common.EkEnumeration.CMSObjectTypes.User + (int)Ektron.Cms.Common.EkEnumeration.UserTypes.MemberShipType + "\' " + (this.m_ObjectType == Ektron.Cms.Common.EkEnumeration.CMSObjectTypes.User && this.m_UserType == Ektron.Cms.Common.EkEnumeration.UserTypes.MemberShipType ? "selected" : "") + ">").Append(this.m_refMsg.GetMessage("lbl members")).Append("</option>");
                result.Append("<option value=\'" + (int)Ektron.Cms.Common.EkEnumeration.CMSObjectTypes.CommunityGroup + "\' " + (this.m_ObjectType == Ektron.Cms.Common.EkEnumeration.CMSObjectTypes.CommunityGroup ? "selected" : "") + ">").Append(this.m_refMsg.GetMessage("lbl community groups")).Append("</option>");
				result.Append("</select></td>");
				//End If
				if (this.m_ObjectType == Ektron.Cms.Common.EkEnumeration.CMSObjectTypes.Content)
				{
					result.Append("<td>&nbsp;</td>");
					result.Append("<td>");
					result.Append(m_refMsg.GetMessage("type label"));
					result.Append("&nbsp;</td><td><select id=\"contenttype\" name=\"contenttype\" OnChange=\"ChangeView();\">");
					result.Append("<option value=\'alltypes\'" + (contentFetchType.ToLower() == "" ? "selected" : "") + ">").Append(this.m_refMsg.GetMessage("lbl all types")).Append("</option>");
                    result.Append("<option value=\'activecontent\'" + (contentFetchType.ToLower() == "activecontent" ? "selected" : "") + ">").Append(this.m_refMsg.GetMessage("lbl content")).Append("</option>");
                    result.Append("<option value=\'library\'" + (contentFetchType.ToLower() == "library" ? "selected" : "") + ">").Append(this.m_refMsg.GetMessage("generic library title")).Append("</option>");
                    result.Append("<option value=\'archivedcontent\'" + (contentFetchType.ToLower() == "archivedcontent" ? "selected" : "") + ">").Append(this.m_refMsg.GetMessage("archive content")).Append("</option>");
					result.Append("</select></td>");
				}
				if (!(asset_data == null))
				{
					if (asset_data.Length > 0)
					{
						result.Append("<td>&nbsp;</td>");
						result.Append("<td><select id=selAssetSupertype name=selAssetSupertype OnChange=\"UpdateView();\">");
						if (Ektron.Cms.Common.EkConstants.CMSContentType_NonLibraryContent == SelectedContentType)
						{
							result.Append("<option value=\'" + Ektron.Cms.Common.EkConstants.CMSContentType_NonLibraryContent + "\' selected>"+this.m_refMsg.GetMessage("lbl all types")+"</option>");
						}
						else
						{
                            result.Append("<option value=\'" + Ektron.Cms.Common.EkConstants.CMSContentType_AllTypes + "\'>" + this.m_refMsg.GetMessage("lbl all types") + "</option>");
						}
						if (Ektron.Cms.Common.EkConstants.CMSContentType_Content == SelectedContentType)
						{
                            result.Append("<option value=\'" + Ektron.Cms.Common.EkConstants.CMSContentType_Content + "\' selected>"+this.m_refMsg.GetMessage("lbl html content")+"</option>");
						}
						else
						{
                            result.Append("<option value=\'" + Ektron.Cms.Common.EkConstants.CMSContentType_Content + "\'>" + this.m_refMsg.GetMessage("lbl html content") + "</option>");
						}
						foreach (AssetInfoData data in asset_data)
						{
							if (Ektron.Cms.Common.EkConstants.ManagedAsset_Min <= data.TypeId && data.TypeId <= Ektron.Cms.Common.EkConstants.ManagedAsset_Max)
							{
								if ("*" != data.PluginType)
								{
									result.Append("<option value=\'" + data.TypeId + "\'");
									if (data.TypeId == SelectedContentType)
									{
										result.Append(" selected");
									}
                                    result.Append(">" + GetMessageText(data.CommonName) + "</option>");
								}
							}
						}
						result.Append("</select></td>");
					}
				}
			}
			result.Append(StyleHelper.ActionBarDivider);
			if (m_strPageAction != "addfolder")
			{
				result.Append("<td>" + m_refstyle.GetHelpButton("AddTaxonomyOrCategoryItem", "") + "</td>");
			}
			else
			{
				result.Append("<td>" + m_refstyle.GetHelpButton("AddTaxonomyOrCategoryFolder", "") + "</td>");
			}
			result.Append("</tr></table>");
			divToolBar.InnerHtml = result.ToString();
			result = null;
		}
        public string GetMessageText(string st)
        {
            if (st == "office documents")
                st = this.m_refMsg.GetMessage("lbl office documents");
            else if (st == "managed files")
                st = this.m_refMsg.GetMessage("lbl managed files");
            else if (st == "Multimedia")
                st = this.m_refMsg.GetMessage("lbl multimedia");
            else if (st == "Open Office")
                st = this.m_refMsg.GetMessage("lbl open office");
            else if (st == "Images")
                st = this.m_refMsg.GetMessage("generic images");
            else if (st == "Forms/Survey")
                st = this.m_refMsg.GetMessage("generic FormsSurvey");
            else if (st == "Non Image Managed Files")
                st = this.m_refMsg.GetMessage("Non Image Managed Files");
            else if (st == "PDF")
                st = this.m_refMsg.GetMessage("generic pdf");
            else if (st.ToLower() == "managed asset")
                st = this.m_refMsg.GetMessage("managed asset");

            return st;
        }
		private string IsSelected(string val)
		{
			if (val == m_strSelectedItem)
			{
				return (" selected ");
			}
			else
			{
				return ("");
			}
		}
		public string getURL()
		{
			string sRet = "";
			if (Request.QueryString.Count > 0)
			{
				for (int i = 0; i <= (Request.QueryString.Count - 1); i++)
				{
					if (Request.QueryString.Keys[i].ToLower() != "type" && Request.QueryString.Keys[i].ToLower() != "contfetchtype")
					{
						sRet += (string) (Request.QueryString.Keys[i] + "=" + Request.QueryString[i] + "&");
					}
				}
			}
            if (sRet.Length > 0 && sRet[sRet.Length - 1].ToString() == "&")
			{
				sRet = (string) ("taxonomy.aspx?" + sRet.Substring(0, sRet.Length - 1));
			}
			else
			{
				sRet = (string) ("taxonomy.aspx?" + sRet);
			}
			if (sRet.ToLower().IndexOf("langtype") < 0)
			{
				sRet = sRet + "&LangType=" + m_refContentApi.RequestInformationRef.ContentLanguage;
			}
			return sRet;
		}
		private string Quote(string KeyWords)
		{
			string result = KeyWords;
			if (KeyWords.Length > 0)
			{
				result = KeyWords.Replace("\'", "\'\'");
			}
			return result;
		}
		
		private void RegisterResources()
		{
			if (m_strPageAction == "addfolder")
			{
				Ektron.Cms.API.Css.RegisterCss(this, m_refContentApi.ApplicationPath + "Tree/css/com.ektron.ui.contextmenu.css", "ContextMenuCSS");
				Ektron.Cms.API.Css.RegisterCss(this, m_refContentApi.ApplicationPath + "Tree/css/com.ektron.ui.tree.css", "TreeCSS");
				Ektron.Cms.API.JS.RegisterJS(this, m_refContentApi.ApplicationPath + "Tree/js/com.ektron.explorer.init.js", "ExplorerInitJS");
				Ektron.Cms.API.JS.RegisterJS(this, m_refContentApi.ApplicationPath + "Tree/js/com.ektron.explorer.js", "ExplorerJS");
				Ektron.Cms.API.JS.RegisterJS(this, m_refContentApi.ApplicationPath + "Tree/js/com.ektron.explorer.config.js", "ExplorerConfigJS");
				Ektron.Cms.API.JS.RegisterJS(this, m_refContentApi.ApplicationPath + "Tree/js/com.ektron.explorer.windows.js", "ExplorerWindowsJS");
				Ektron.Cms.API.JS.RegisterJS(this, m_refContentApi.ApplicationPath + "Tree/js/com.ektron.cms.types.js", "CMSTypesJS");
				Ektron.Cms.API.JS.RegisterJS(this, m_refContentApi.ApplicationPath + "Tree/js/com.ektron.cms.parser.js", "CMSParserJS");
				Ektron.Cms.API.JS.RegisterJS(this, m_refContentApi.ApplicationPath + "Tree/js/com.ektron.cms.toolkit.js", "CMSToolkitJS");
				Ektron.Cms.API.JS.RegisterJS(this, m_refContentApi.ApplicationPath + "Tree/js/com.ektron.cms.api.js", "CMSAPIJS");
				Ektron.Cms.API.JS.RegisterJS(this, m_refContentApi.ApplicationPath + "Tree/js/com.ektron.ui.contextmenu.js", "UIContextMenuJS");
				Ektron.Cms.API.JS.RegisterJS(this, m_refContentApi.ApplicationPath + "Tree/js/com.ektron.ui.iconlist.js", "UIIconlistJS");
				Ektron.Cms.API.JS.RegisterJS(this, m_refContentApi.ApplicationPath + "Tree/js/com.ektron.ui.explore.js", "UIExploreJS");
				Ektron.Cms.API.JS.RegisterJS(this, m_refContentApi.ApplicationPath + "Tree/js/com.ektron.ui.assignfolder.js", "UIAssignFolderJS");
				Ektron.Cms.API.JS.RegisterJS(this, m_refContentApi.ApplicationPath + "Tree/js/com.ektron.net.http.js", "NetHTTPJS");
				Ektron.Cms.API.JS.RegisterJS(this, m_refContentApi.ApplicationPath + "Tree/js/com.ektron.lang.exception.js", "LangExceptionJS");
				Ektron.Cms.API.JS.RegisterJS(this, m_refContentApi.ApplicationPath + "Tree/js/com.ektron.utils.form.js", "UtilsFormJS");
				Ektron.Cms.API.JS.RegisterJS(this, m_refContentApi.ApplicationPath + "Tree/js/com.ektron.utils.log.js", "UtilsLogJS");
				Ektron.Cms.API.JS.RegisterJS(this, m_refContentApi.ApplicationPath + "Tree/js/com.ektron.utils.dom.js", "UtilsDOMJS");
				Ektron.Cms.API.JS.RegisterJS(this, m_refContentApi.ApplicationPath + "Tree/js/com.ektron.utils.debug.js", "UtilsDebugJS");
				Ektron.Cms.API.JS.RegisterJS(this, m_refContentApi.ApplicationPath + "Tree/js/com.ektron.utils.string.js", "UtilsStringJS");
			}
			Ektron.Cms.API.JS.RegisterJS(this, Ektron.Cms.API.JS.ManagedScript.EktronJS);
			Ektron.Cms.API.JS.RegisterJS(this, Ektron.Cms.API.JS.ManagedScript.EktronInputLabelJS);
			Ektron.Cms.API.JS.RegisterJS(this, m_refContentApi.ApplicationPath + "java/ektron.workarea.searchBox.inputLabelInit.js", "EktronSearchBoxInputLabelInitJS");
		}
		
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
				TotalPages.ToolTip = TotalPages.Text;
				CurrentPage.Text = m_intCurrentPage.ToString();
				CurrentPage.ToolTip = CurrentPage.Text;
				PreviousPage.Enabled = true;
				FirstPage.Enabled = true;
				NextPage.Enabled = true;
				LastPage.Enabled = true;
				if (m_intCurrentPage == 1)
				{
					PreviousPage.Enabled = false;
					FirstPage.Enabled = false;
				}
				else if (m_intCurrentPage == m_intTotalPages)
				{
					NextPage.Enabled = false;
					LastPage.Enabled = false;
				}
			}
		}
		private void VisiblePageControls(bool flag)
		{
			TotalPages.Visible = flag;
			CurrentPage.Visible = flag;
			PreviousPage.Visible = flag;
			NextPage.Visible = flag;
			LastPage.Visible = flag;
			FirstPage.Visible = flag;
			PageLabel.Visible = flag;
			OfLabel.Visible = flag;
		}
		public void NavigationLink_Click(object sender, CommandEventArgs e)
		{
			m_intTotalPages = int.Parse((string) TotalPages.Text);
			switch (e.CommandName)
			{
				case "First":
					m_intCurrentPage = 1;
					break;
				case "Last":
					m_intCurrentPage = m_intTotalPages;
					break;
				case "Next":
					m_intCurrentPage = System.Convert.ToInt32(int.Parse((string) CurrentPage.Text) + 1);
					CurrentPage.Text = m_intCurrentPage.ToString();
					break;
				case "Prev":
					m_intCurrentPage = System.Convert.ToInt32(int.Parse((string) CurrentPage.Text) - 1);
					CurrentPage.Text = m_intCurrentPage.ToString();
					break;
			}
			if (m_intCurrentPage < 1)
			{
				m_intCurrentPage = 1;
			}
			if (m_intCurrentPage > m_intTotalPages)
			{
				m_intCurrentPage = m_intTotalPages;
			}
			DisplayPage();
			isPostData.Value = "true";
		}
	}
	
