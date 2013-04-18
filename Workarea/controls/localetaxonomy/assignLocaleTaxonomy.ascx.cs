//-----------------------------------------------------------------------
// <copyright file="assignLocaleTaxonomy.ascx.cs" company="Ektron" author="Rama Ila">
//     Copyright (c) Ektron, Inc. All rights reserved.
// </copyright>
// Assigns a locale taxonomy Item of type content,folder,user or language to a given translation taxonomy
//-----------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Web.UI.WebControls;
using Ektron.Cms;
using Ektron.Cms.BusinessObjects.Localization;
using Ektron.Cms.Common;
using Microsoft.VisualBasic;
using Ektron.Cms.Localization;

/// <summary>
/// Codebehind for AssigneLocaleTaxonomy.aspx
/// </summary>
public partial class assignLocaleTaxonomy : System.Web.UI.UserControl
{
    protected ContentAPI m_refContentApi = new ContentAPI();
    protected CommonApi m_refCommonApi = new CommonApi();
    protected StyleHelper m_refstyle = new StyleHelper();
    protected string AppImgPath = string.Empty;
    protected LocalizationAPI objLocalizationApi = new LocalizationAPI();
    protected string AppPath = string.Empty;
    protected EkMessageHelper m_refMsg;
    protected string m_strPageAction = string.Empty;
    protected Ektron.Cms.Content.EkContent m_refContent;
    protected int TaxonomyLanguage = -1;
    protected long TaxonomyId = 0;
    protected long TaxonomyParentId = 0;
    protected LanguageData language_data;
    protected AssetInfoData[] asset_data;
    protected int SelectedContentType = -1;
    protected long FolderId = 0;
    protected Collection folder_data_col;
    protected string FolderName = string.Empty;
    protected string FolderPath = string.Empty;
    protected long FolderParentId = 0;
    protected Collection folder_request_col;
    protected string ContentIcon;
    protected string pageIcon = string.Empty;
    protected string UserIcon;
    protected string FormsIcon = string.Empty;
    protected string m_selectedFoldersCSV = string.Empty;
    protected string m_prevFolderDescendantsCSV = string.Empty;
    protected string m_prevFolderChildrenCSV = string.Empty;
    protected EkEnumeration.CMSObjectTypes m_ObjectType = EkEnumeration.CMSObjectTypes.Content;
    protected EkEnumeration.TaxonomyItemType m_LocaleObjectType = EkEnumeration.TaxonomyItemType.Content;
    protected EkEnumeration.UserTypes m_UserType = EkEnumeration.UserTypes.AuthorType;
    protected string m_strSelectedItem = "-1";
    protected string m_strKeyWords = string.Empty;
    protected string m_strSearchText = string.Empty;
    protected int m_intCurrentPage = 1;
    protected int m_intTotalPages = 1;
    string localeTax = string.Empty;
    protected Ektron.Cms.API.Site ektonSite = new Ektron.Cms.API.Site();
    protected string contentFetchType = string.Empty;
    protected UserData[] userList = null;
    protected string _ViewItem = "item";
    protected CommunityGroupData[] cgroup_list = null;
    protected DirectoryAdvancedGroupData groupData = new DirectoryAdvancedGroupData();

    /// <summary>
    /// Handles the Navigation of the Page setting of the Content of the locale taxonomy.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
    public void NavigationLink_Click(object sender, CommandEventArgs e)
    {
        this.m_intTotalPages = Int32.Parse(TotalPages.Text);
        switch (e.CommandName)
        {
            case "First":
               this.m_intCurrentPage = 1;
                break;
            case "Last":
                this.m_intCurrentPage = this.m_intTotalPages;
                break;
            case "Next":
                this.m_intCurrentPage = Int32.Parse(CurrentPage.Text) + 1;
                CurrentPage.Text = this.m_intCurrentPage.ToString();
                break;
            case "Prev":
                this.m_intCurrentPage = Int32.Parse(CurrentPage.Text) - 1;
                CurrentPage.Text = this.m_intCurrentPage.ToString();
                break;
        }
        if ((this.m_intCurrentPage < 1))
        {
            this.m_intCurrentPage = 1;
        }
        if ((this.m_intCurrentPage > this.m_intTotalPages))
        {
            this.m_intCurrentPage = this.m_intTotalPages;
        }
        DisplayPage();
        isPostData.Value = "true";
    }

    /// <summary>
    /// Handles the Load event of the Page control.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
    protected void Page_Load(object sender, System.EventArgs e)
    {
        m_refMsg = this.m_refContentApi.EkMsgRef;
        AppImgPath = this.m_refContentApi.AppImgPath;
        AppPath = this.m_refContentApi.AppPath;
        m_strPageAction = Request.QueryString["action"];
        object refApi = m_refContentApi as object;
        Utilities.SetLanguage(m_refContentApi);
        TaxonomyLanguage = this.m_refContentApi.ContentLanguage;
        if ((TaxonomyLanguage == -1))
        {
            TaxonomyLanguage = m_refContentApi.DefaultContentLanguage;
        }
        if ((Request.QueryString["view"] != null))
        {
            _ViewItem = Request.QueryString["view"];
        }
        if ((Request.QueryString["taxonomyid"] != null))
        {
            TaxonomyId = Convert.ToInt64(Request.QueryString["taxonomyid"]);
        }

        if ((Request.QueryString["parentid"] != null))
        {
            this.TaxonomyParentId = Convert.ToInt64(Request.QueryString["parentid"]);
        }

        if ((Request.QueryString["type"] != null) && Request.QueryString["type"].ToLower() == "author")
        {
            this.m_ObjectType = EkEnumeration.CMSObjectTypes.User;
            this.m_UserType = EkEnumeration.UserTypes.AuthorType;
        }

        else if ((Request.QueryString["type"] != null) && Request.QueryString["type"].ToLower() == "member")
        {
            this.m_ObjectType = EkEnumeration.CMSObjectTypes.User;
            this.m_UserType = EkEnumeration.UserTypes.MemberShipType;
        }

        else if ((Request.QueryString["type"] != null) && Request.QueryString["type"].ToLower() == "cgroup")
        {
            this.m_ObjectType = EkEnumeration.CMSObjectTypes.CommunityGroup;
        }

        else if ((Request.QueryString["type"] != null) && Request.QueryString["type"].ToLower() == "locales")
        {
            this.m_LocaleObjectType = EkEnumeration.TaxonomyItemType.Locale;
        }

        else if ((m_strPageAction == "addfolder"))
        {
            localization_folder_chkRecursive_div.Visible = true;
            this.m_ObjectType = EkEnumeration.CMSObjectTypes.Folder;
        }

        if ((Request.QueryString["contFetchType"] != null) && !string.IsNullOrEmpty(Request.QueryString["contFetchType"].ToLower()))
        {
            contentFetchType = Request.QueryString["contFetchType"];
        }

        this.m_refContent = this.m_refContentApi.EkContentRef;
        FormsIcon = "<img src=\"" + this.m_refContentApi.AppPath + "images/UI/Icons/contentForm.png\" alt=\"Form\">";
        ContentIcon = "<img src=\"" + this.m_refContentApi.AppPath + "images/UI/Icons/contentHtml.png\" alt=\"Content\">";
        pageIcon = "<img src=\"" + m_refContentApi.AppPath + "images/ui/icons/layout.png\" alt=\"Page\">";
        if (this.m_UserType == EkEnumeration.UserTypes.AuthorType)
        {
            UserIcon = "<img src=\"" + this.m_refContentApi.AppPath + "Images/ui/icons/user.png\" alt=\"Content\">";
        }
        else
        {
            UserIcon = "<img src=\"" + this.m_refContentApi.AppPath + "Images/ui/icons/userMembership.png\" alt=\"Content\">";
        }
        if ((Page.IsPostBack && (!string.IsNullOrEmpty(Request.Form[isPostData.UniqueID])) && (m_ObjectType == EkEnumeration.CMSObjectTypes.Content | (m_ObjectType == EkEnumeration.CMSObjectTypes.User & !string.IsNullOrEmpty(Request.Form["itemlist"])) | (m_ObjectType == EkEnumeration.CMSObjectTypes.CommunityGroup & !string.IsNullOrEmpty(Request.Form["itemlist"]) | (m_ObjectType == EkEnumeration.CMSObjectTypes.Folder & !string.IsNullOrEmpty(Request.Form["itemlist"])) | (m_ObjectType == EkEnumeration.CMSObjectTypes.TaxonomyNode & !string.IsNullOrEmpty(Request.Form["itemlist"]))))))
        {
            if ((this.m_strPageAction == "additem"))
            {
                TaxonomyRequest item_request = new TaxonomyRequest();
                item_request.TaxonomyId = TaxonomyId;
                item_request.TaxonomyIdList = Validate(Request.Form["itemlist"]);
                if (m_ObjectType == EkEnumeration.CMSObjectTypes.User)
                {
                    item_request.TaxonomyItemType = EkEnumeration.TaxonomyItemType.User;
                }
                else if (this.m_ObjectType == EkEnumeration.CMSObjectTypes.CommunityGroup)
                {
                    item_request.TaxonomyItemType = EkEnumeration.TaxonomyItemType.Group;
                }
                else if (this.m_LocaleObjectType == EkEnumeration.TaxonomyItemType.Locale)
                {
                    item_request.TaxonomyItemType = EkEnumeration.TaxonomyItemType.Locale;
                }
                item_request.TaxonomyLanguage = TaxonomyLanguage;
                m_refContent.AddTaxonomyItem(item_request);
            }
            else if (this.m_strPageAction == "addfolder")
            {
                TaxonomyRequest item_request = new TaxonomyRequest();
                item_request.TaxonomyId = this.TaxonomyId;
                item_request.TaxonomyLanguage = this.TaxonomyLanguage;

                item_request.TaxonomyIdList = this.Validate(Request.Form["newFolderDescendantsCSV"]);
                if (!String.IsNullOrEmpty(item_request.TaxonomyIdList))
                {
                    item_request.TaxonomyItemType = EkEnumeration.TaxonomyItemType.FolderDescendants;
                    this.m_refContent.AddTaxonomyItem(item_request);
                }

                item_request.TaxonomyIdList = this.Validate(Request.Form["newFolderChildrenCSV"]);
                if (!String.IsNullOrEmpty(item_request.TaxonomyIdList))
                {
                    item_request.TaxonomyItemType = EkEnumeration.TaxonomyItemType.FolderChildren;
                    this.m_refContent.AddTaxonomyItem(item_request);
                }
            }
            if ((Request.QueryString["iframe"] == "true"))
            {
                Response.Write("<script type=\"text/javascript\">parent.CloseChildPage();</script>");
            }
            else
            {
                if ((Request.QueryString["type"] != null))
                {
                    if (this.m_LocaleObjectType == EkEnumeration.TaxonomyItemType.Locale)
                    {
                        Response.Redirect("LocaleTaxonomy.aspx?action=view&view=" + this.m_LocaleObjectType + "&taxonomyid=" + this.TaxonomyId);
                    }
                    else
                    {
                        if (this.m_ObjectType == EkEnumeration.CMSObjectTypes.CommunityGroup)
                        {
                            Response.Redirect("LocaleTaxonomy.aspx?action=view&view=cGroup&taxonomyid=" + TaxonomyId);
                        }
                        else
                        {
                            Response.Redirect("LocaleTaxonomy.aspx?action=view&view=" + Convert.ToString(this.m_ObjectType).ToLower() + "&taxonomyid=" + TaxonomyId);
                        }
                       
                    }

                }
                else if (this.m_ObjectType == EkEnumeration.CMSObjectTypes.Folder)
                {
                    Response.Redirect("LocaleTaxonomy.aspx?action=view&view=" + Convert.ToString(this.m_ObjectType).ToLower() + "&taxonomyid=" + TaxonomyId);
                }
                else
                {
                    Response.Redirect("LocaleTaxonomy.aspx?action=view&taxonomyid=" + TaxonomyId);
                }

            }
        }
        else
        {
            this.FolderId = Convert.ToInt64(Request.QueryString["folderid"]);

            folder_data_col = this.m_refContent.GetFolderInfoWithPath(FolderId);
            FolderName = this.folder_data_col["FolderName"].ToString();
            FolderParentId = Convert.ToInt64(this.folder_data_col["ParentID"].ToString());
            FolderPath = this.folder_data_col["Path"].ToString();

            folder_request_col = new Collection();
            folder_request_col.Add(this.FolderId, "ParentID", null, null);
            folder_request_col.Add("name", "OrderBy", null, null);
            folder_data_col = this.m_refContent.GetAllViewableChildFoldersv2_0(folder_request_col);

            if ((m_strPageAction != "additem"))
            {
                if (!string.IsNullOrEmpty(Request.QueryString[EkConstants.ContentTypeUrlParam]))
                {
                    if (Information.IsNumeric(Request.QueryString[EkConstants.ContentTypeUrlParam]))
                    {
                        this.SelectedContentType = Convert.ToInt32(Request.QueryString[EkConstants.ContentTypeUrlParam]);
                        this.m_refContentApi.SetCookieValue(EkConstants.ContentTypeUrlParam.ToString(), SelectedContentType.ToString());
                    }
                }
                else if (!string.IsNullOrEmpty(Ektron.Cms.CommonApi.GetEcmCookie()[(EkConstants.ContentTypeUrlParam)]))
                {
                    if (Information.IsNumeric(Ektron.Cms.CommonApi.GetEcmCookie()[(EkConstants.ContentTypeUrlParam)]))
                    {
                        this.SelectedContentType = Convert.ToInt32(Ektron.Cms.CommonApi.GetEcmCookie()[(EkConstants.ContentTypeUrlParam)]);
                    }
                }
                asset_data = this.m_refContent.GetAssetSuperTypes();
            }
            this.RegisterResources();
            this.TaxonomyToolBar();
            if ((!Page.IsPostBack || m_ObjectType == EkEnumeration.CMSObjectTypes.User || this.m_ObjectType == EkEnumeration.CMSObjectTypes.CommunityGroup))
            {
                //// avoid redisplay when clicking next/prev buttons
                this.DisplayPage();
            }
        }
    }

    /// <summary>
    /// Handles the Pre render event of the Page control.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
    protected void Page_PreRender(object sender, System.EventArgs e)
    {
        PageSettings();
    }

    /// <summary>
    /// Handles the Populating the taxonomy content grid Data Display page settings.
    /// </summary>
    private void DisplayPage()
    {
        if (this.m_strPageAction != "addfolder")
        {
            this.PopulateGridData();
        }
        else
        {
            this.m_selectedFoldersCSV = string.Empty;
            this.m_prevFolderDescendantsCSV = string.Empty;
            this.m_prevFolderChildrenCSV = string.Empty;
            Ektron.Cms.BusinessObjects.Localization.LocaleTaxonomy api = new Ektron.Cms.BusinessObjects.Localization.LocaleTaxonomy(m_refCommonApi.RequestInformationRef);
            List<LocalizableItem> folderItemList = api.GetList(TaxonomyId, m_refCommonApi.ContentLanguage, false);
            if (folderItemList != null && folderItemList.Count > 0)
            {
                for (int i = 0; i < folderItemList.Count; i++)
                {
                    if (LocalizableCmsObjectType.FolderContents == folderItemList[i].LocalizableType)
                    {
                        string folderId = folderItemList[i].Id.ToString();
                        if (folderItemList[i].Recursive)
                        {
                            if (this.m_prevFolderDescendantsCSV.Length > 0)
                            {
                                this.m_prevFolderDescendantsCSV += ",";
                            }

                            this.m_prevFolderDescendantsCSV += folderId;
                        }
                        else
                        {
                            if (this.m_prevFolderChildrenCSV.Length > 0)
                            {
                                this.m_prevFolderChildrenCSV += ",";
                            }

                            this.m_prevFolderChildrenCSV += folderId;
                        }

                        if (this.m_selectedFoldersCSV.Length > 0)
                        {
                            this.m_selectedFoldersCSV += ",";
                        }

                        this.m_selectedFoldersCSV += folderId;
                    }
                } // loop
            }

            //TaxonomyFolderSyncData[] taxonomy_sync_folder = null;
            //TaxonomyBaseRequest tax_sync_folder_req = new TaxonomyBaseRequest();
            //tax_sync_folder_req.TaxonomyId = TaxonomyId;
            //tax_sync_folder_req.TaxonomyLanguage = TaxonomyLanguage;
            //taxonomy_sync_folder = m_refContent.GetAllAssignedCategoryFolder(tax_sync_folder_req);
            //if ((taxonomy_sync_folder != null && taxonomy_sync_folder.Length > 0))
            //{
            //    for (int cnt = 0; cnt <= taxonomy_sync_folder.Length - 1; cnt++)
            //    {
            //        if ((m_selectedFoldersCSV.Length > 0))
            //        {
            //            this.m_selectedFoldersCSV = this.m_selectedFoldersCSV + "," + taxonomy_sync_folder[cnt].FolderId;
            //        }
            //        else
            //        {
            //            this.m_selectedFoldersCSV = taxonomy_sync_folder[cnt].FolderId.ToString();
            //        }
            //    }
            //}
          }
    }

    /// <summary>
    /// Handles the Validating the STring values of the content ids,language IDs,Folders Ids passed for assigning the as items tio the locale taxonomy.
    /// </summary>
    /// <param name="value">The value of the comparator.</param>
    private string Validate(string value)
    {
        if ((value != null))
        {
            return value;
        }
        else
        {
            return string.Empty;
        }
    }

    /// <summary>
    /// Handles the Populating the content Grid Data when the user clicks on assign the items or folders to a locale taxonomy.
    /// </summary>
    private void PopulateGridData()
    {
        if ((TaxonomyItemList.Columns.Count == 0))
        {
            TaxonomyItemList.Columns.Add(m_refstyle.CreateBoundField("ITEM1", string.Empty, "info", HorizontalAlign.NotSet, HorizontalAlign.NotSet, Unit.Percentage(0), Unit.Percentage(0), false, false));
        }

        string iframe = string.Empty;
        if ((Request.QueryString["iframe"] != null && !string.IsNullOrEmpty(Request.QueryString["iframe"])))
        {
            iframe = "&iframe=true";
        }
        DataTable dt = new DataTable();
        DataRow dr = null;
        dt.Columns.Add(new DataColumn("ITEM1", typeof(string)));

        dr = dt.NewRow();
        if ((this.m_strPageAction == "additem") && this.m_ObjectType == EkEnumeration.CMSObjectTypes.User)
        {
            dr[0] = this.m_refMsg.GetMessage("lbl select users") + "<br/>";
        }
        else if ((m_strPageAction == "additem") && this.m_ObjectType == EkEnumeration.CMSObjectTypes.CommunityGroup)
        {
            dr[0] = this.m_refMsg.GetMessage("lbl select cgroups") + "<br/>";
        }
        else if ((m_strPageAction == "additem") && this.m_ObjectType == EkEnumeration.CMSObjectTypes.TaxonomyNode)
        {
            dr[0] = this.m_refMsg.GetMessage("lbl assign locale taxonomy item") + "<br/>";
        }
        else if ((m_strPageAction == "additem") && this.m_LocaleObjectType == EkEnumeration.TaxonomyItemType.Locale)
        {
            dr[0] = this.m_refMsg.GetMessage("assigntaxonomylocalelabel") + "<br/>";
        }
        else if ((this.m_strPageAction == "additem"))
        {
            dr[0] = this.m_refMsg.GetMessage("assigntaxonomyitemlabel") + "<br/>";
        }
        else
        {
            dr[0] = this.m_refMsg.GetMessage("assigntaxonomyfolderlabel") + "<br/>";
        }

        dt.Rows.Add(dr);

        if (this.m_ObjectType == EkEnumeration.CMSObjectTypes.Content && (m_LocaleObjectType != EkEnumeration.TaxonomyItemType.Locale))
        {
            dr = dt.NewRow();
            dr[0] = m_refMsg.GetMessage("generic Path") + FolderPath;
            dt.Rows.Add(dr);
            dr = dt.NewRow();
            if ((FolderId != 0))
            {
                dr[0] = "<a href=\"LocaleTaxonomy.aspx?action=" + m_strPageAction + "&taxonomyid=" + TaxonomyId + "&folderid=" + FolderParentId + "&parentid=" + FolderParentId + iframe;
                dr[0] = dr[0] + "&title=\"" + m_refMsg.GetMessage("alt: generic previous dir text") + "\"><img src=\"" + m_refContentApi.AppPath + "images/ui/icons/folderUp.png" + "\" border=\"0\" title=\"" + m_refMsg.GetMessage("alt: generic previous dir text") + "\" alt=\"" + m_refMsg.GetMessage("alt: generic previous dir text") + "\" align=\"absbottom\">..</a>";
            }

            dt.Rows.Add(dr);
            if ((folder_data_col != null))
            {
                foreach (Collection folder in folder_data_col)
                {
                    dr = dt.NewRow();
                    dr[0] = "<a href=\"LocaleTaxonomy.aspx?action=" + m_strPageAction + "&taxonomyid=" + TaxonomyId + "&folderid=" + folder["id"] + "&parentid=" + FolderParentId + iframe;
                    dr[0] += "&title=\"" + m_refMsg.GetMessage("alt: generic view folder content text") + "\"><img src=\"";
                    switch ((EkEnumeration.FolderType)folder["FolderType"])
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
                    dr[0] += "<a href=\"LocaleTaxonomy.aspx?action=" + m_strPageAction + "&taxonomyid=" + TaxonomyId + "&folderid=" + folder["id"] + "&parentid=" + FolderParentId + iframe + "&title=\"" + m_refMsg.GetMessage("alt: generic view folder content text") + "\">" + folder["Name"] + "</a>";
                    dt.Rows.Add(dr);
                }
            }
            if ((m_strPageAction == "additem"))
            {
                ContentData[] taxonomy_unassigneditem_arr = null;
                TaxonomyRequest request = new TaxonomyRequest();
                request.TaxonomyId = TaxonomyId;
                request.TaxonomyLanguage = TaxonomyLanguage;
                request.FolderId = FolderId;
                if ((contentFetchType.ToLower() == "activecontent"))
                {
                request.TaxonomyItemType = EkEnumeration.TaxonomyItemType.ActiveContent;
                }
                else if ((contentFetchType.ToLower() == "archivedcontent"))
                {
                    request.TaxonomyItemType = EkEnumeration.TaxonomyItemType.ArchivedContent;
                }
                else
                {
                    request.TaxonomyItemType = 0;
                }
                
                //// get total #pages first because the API doesn't return it (lame slow way to do this)-:
                request.PageSize = 99999999;
                request.CurrentPage = 1;
                Ektron.Cms.BusinessObjects.Localization.L10nManager l10nMgr = new Ektron.Cms.BusinessObjects.Localization.L10nManager(m_refContentApi.RequestInformationRef);
                Criteria<FolderData> criteria = new Criteria<FolderData>();
                criteria.PagingInfo.RecordsPerPage = 500;
                List<ILocalizable> donotTranslateList = l10nMgr.GetDoNotTranslateList(FolderId, true, TaxonomyLanguage, Ektron.Cms.Common.EkConstants.CMSContentType_AllTypes, criteria);
                List<long> NonTranslatedList = new List<long>();
                if (donotTranslateList.Count > 0)
                {
                    for (int l = 0; l < donotTranslateList.Count; l++)
                    {
                        NonTranslatedList.Add(donotTranslateList[l].Id);
                    }
                }

                taxonomy_unassigneditem_arr = m_refContent.ReadAllUnAssignedTaxonomyItems(request);
                this.m_intTotalPages = Convert.ToInt32((taxonomy_unassigneditem_arr.Length + (m_refContentApi.RequestInformationRef.PagingSize - 1)) / m_refContentApi.RequestInformationRef.PagingSize);
               //// get the real page data set
                request.PageSize =this.m_refContentApi.RequestInformationRef.PagingSize;
                request.CurrentPage = this.m_intCurrentPage;
                taxonomy_unassigneditem_arr = this.m_refContent.ReadAllUnAssignedTaxonomyItems(request);
                LibraryData library_dat = default(LibraryData);
                foreach (ContentData taxonomy_item in taxonomy_unassigneditem_arr)
                {
                    if (!NonTranslatedList.Contains(taxonomy_item.Id))
                    {
                        dr = dt.NewRow();
                        if (taxonomy_item.Type == 1 | taxonomy_item.Type == 2)
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
                        else if (taxonomy_item.Type == 7)
                        {
                            library_dat = this.m_refContentApi.GetLibraryItemByContentID(taxonomy_item.Id);
                            if (library_dat != null && !string.IsNullOrEmpty(library_dat.FileName))
                            {
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
                                    default:
                                        //// other files
                                        dr[0] = "&nbsp;&nbsp;&nbsp;&nbsp;<input type=\"checkbox\" id=\"itemlist\" name=\"itemlist\" value=\"" + taxonomy_item.Id + "\"/>&nbsp;<img src=\"" + m_refContentApi.AppPath + "images/UI/Icons/book.png" + "\" alt=\"" + taxonomy_item.AssetData.FileName + "\"></img>&nbsp;" + taxonomy_item.Title;
                                        break;
                                }
                            }
                        }
                        else if (taxonomy_item.Type == 3333)
                        {
                            dr[0] = "&nbsp;&nbsp;&nbsp;&nbsp;<input type=\"checkbox\" id=\"itemlist\" name=\"itemlist\" value=\"" + taxonomy_item.Id + "\"/>&nbsp;<img src=\"" + m_refContentApi.AppPath + "Images/ui/icons/brick.png" + "\" alt=\"" + taxonomy_item.AssetData.FileName + "\"></img>&nbsp;" + taxonomy_item.Title;
                        }
                        else if (string.IsNullOrEmpty(taxonomy_item.AssetData.ImageUrl) & (taxonomy_item.Type != 1 & taxonomy_item.Type != 2 & taxonomy_item.Type != 3 & taxonomy_item.Type != 1111 & taxonomy_item.Type != 1112 & taxonomy_item.Type != 3333))
                        {
                            dr[0] = "&nbsp;&nbsp;&nbsp;&nbsp;<input type=\"checkbox\" id=\"itemlist\" name=\"itemlist\" value=\"" + taxonomy_item.Id + "\"/>&nbsp;<img src=\"" + m_refContentApi.AppPath + "images/UI/Icons/book.png" + "\" alt=\"" + taxonomy_item.AssetData.FileName + "\"></img>&nbsp;" + taxonomy_item.Title;
                        }
                        else
                        {
                            ////Bad Approach however no other way untill AssetManagement/Images/ are updated with version 8 images or DMS points to workarea images
                            if (string.IsNullOrEmpty(taxonomy_item.AssetData.ImageUrl))
                            {
                                dr[0] = "&nbsp;&nbsp;&nbsp;&nbsp;<input type=\"checkbox\" id=\"itemlist\" name=\"itemlist\" value=\"" + taxonomy_item.Id + "\"/>&nbsp;<img src=\"" + m_refContentApi.AppPath + "images/UI/Icons/book.png" + "\" alt=\"" + taxonomy_item.AssetData.FileName + "\"></img>&nbsp;" + taxonomy_item.Title;
                            }
                            else
                            {
                                switch (Path.GetFileName(taxonomy_item.AssetData.ImageUrl).ToLower())
                                {
                                    case "ms-word.gif":
                                        dr[0] = "&nbsp;&nbsp;&nbsp;&nbsp;<input type=\"checkbox\" id=\"itemlist\" name=\"itemlist\" value=\"" + taxonomy_item.Id + "\"/>&nbsp;<img src=\"" + m_refContentApi.AppPath + "images/ui/icons/FileTypes/word.png" + "\" alt=\"" + taxonomy_item.AssetData.FileName + "\"></img>&nbsp;" + taxonomy_item.Title;
                                        break;
                                    case "ms-excel.gif":
                                        dr[0] = "&nbsp;&nbsp;&nbsp;&nbsp;<input type=\"checkbox\" id=\"itemlist\" name=\"itemlist\" value=\"" + taxonomy_item.Id + "\"/>&nbsp;<img src=\"" + m_refContentApi.AppPath + "images/ui/icons/FileTypes/excel.png" + "\" alt=\"" + taxonomy_item.AssetData.FileName + "\"></img>&nbsp;" + taxonomy_item.Title;
                                        break;
                                    case "ms-powerpoint.gif":
                                        dr[0] = "&nbsp;&nbsp;&nbsp;&nbsp;<input type=\"checkbox\" id=\"itemlist\" name=\"itemlist\" value=\"" + taxonomy_item.Id + "\"/>&nbsp;<img src=\"" + m_refContentApi.AppPath + "images/ui/icons/FileTypes/powerpoint.png" + "\" alt=\"" + taxonomy_item.AssetData.FileName + "\"></img>&nbsp;" + taxonomy_item.Title;
                                        break;
                                    case "adobe-pdf.gif":
                                        dr[0] = "&nbsp;&nbsp;&nbsp;&nbsp;<input type=\"checkbox\" id=\"itemlist\" name=\"itemlist\" value=\"" + taxonomy_item.Id + "\"/>&nbsp;<img src=\"" + m_refContentApi.AppPath + "images/ui/icons/FileTypes/acrobat.png" + "\" alt=\"" + taxonomy_item.AssetData.FileName + "\"></img>&nbsp;" + taxonomy_item.Title;
                                        break;
                                    case "image.gif":
                                        dr[0] = "&nbsp;&nbsp;&nbsp;&nbsp;<input type=\"checkbox\" id=\"itemlist\" name=\"itemlist\" value=\"" + taxonomy_item.Id + "\"/>&nbsp;<img src=\"" + m_refContentApi.AppPath + "images/ui/icons/FileTypes/image.png" + "\" alt=\"" + taxonomy_item.AssetData.FileName + "\"></img>&nbsp;" + taxonomy_item.Title;
                                        break;
                                    default:
                                        dr[0] = "&nbsp;&nbsp;&nbsp;&nbsp;<input type=\"checkbox\" id=\"itemlist\" name=\"itemlist\" value=\"" + taxonomy_item.Id + "\"/>&nbsp;<img src=\"" + taxonomy_item.AssetData.ImageUrl + "\" alt=\"" + taxonomy_item.AssetData.FileName + "\"></img>&nbsp;" + taxonomy_item.Title;
                                        break;
                                }
                            }
                        }
                        dt.Rows.Add(dr);
                    }

                }
            }
        }
        else if (this.m_ObjectType == EkEnumeration.CMSObjectTypes.CommunityGroup)
        {
            CollectSearchText();
            dr = dt.NewRow();
            dr[0] = "<input type=text size=25 id=\"txtSearch\" name=\"txtSearch\" value=\"" + this.m_strKeyWords + "\" onkeydown=\"CheckForReturn(event)\">";
            dr[0] += "<input type=button value=\"Search\" id=\"btnSearch\" name=\"btnSearch\"  class=\"ektronWorkareaSearch\" onclick=\"searchuser();\" title=\"Search Users\">";
            dt.Rows.Add(dr);
            this.GetAssignedCommunityGroups();
            this.GetCommunityGroups();
            if (this.cgroup_list != null)
            {

                for (int j = 0; j <= (cgroup_list.Length - 1); j++)
                {
                    dr = dt.NewRow();
                    if (DoesGroupExistInList(cgroup_list[j].GroupId))
                    {
                        dr[0] = "&nbsp;&nbsp;&nbsp;&nbsp;" + GetTypeIcon(EkEnumeration.CMSObjectTypes.User.GetHashCode(), EkEnumeration.CMSContentSubtype.Content) + "<input type=\"checkbox\" checked=\"checked\" disabled=\"disabled\" id=\"itemlistNoId\" name=\"itemlistNoId\" value=\"" + cgroup_list[j].GroupId + "\"/>" + cgroup_list[j].GroupName;
                    }
                    else
                    {
                        dr[0] = "&nbsp;&nbsp;&nbsp;&nbsp;" + GetTypeIcon(EkEnumeration.CMSObjectTypes.User.GetHashCode(), EkEnumeration.CMSContentSubtype.Content) + "<input type=\"checkbox\" id=\"itemlist\" name=\"itemlist\" value=\"" + cgroup_list[j].GroupId + "\"/>" + cgroup_list[j].GroupName;
                    }

                    dt.Rows.Add(dr);
                }
            }
        }
        else if (this.m_ObjectType == EkEnumeration.CMSObjectTypes.User)
        {
            CollectSearchText();
            dr = dt.NewRow();
            dr[0] = "<input type=text size=25 id=\"txtSearch\" name=\"txtSearch\" value=\"" + this.m_strKeyWords + "\" onkeydown=\"CheckForReturn(event)\">";
            dr[0] += "<select id=\"searchlist\" name=\"searchlist\">";
            dr[0] += "<option value=-1" + IsSelected("-1") + ">All</option>";
            dr[0] += "<option value=\"last_name\"" + IsSelected("last_name") + ">Last Name</option>";
            dr[0] += "<option value=\"first_name\"" + IsSelected("first_name") + ">First Name</option>";
            dr[0] += "<option value=\"user_name\"" + IsSelected("user_name") + ">User Name</option>";
            dr[0] += "</select><input type=button value=\"Search\" id=\"btnSearch\" name=\"btnSearch\" class=\"ektronWorkareaSearch\"  onclick=\"searchuser();\" title=\"Search Users\">";
            dt.Rows.Add(dr);

            GetUsers();
            if (userList != null)
            {
                for (int j = 0; j <= (userList.Length - 1); j++)
                {
                    dr = dt.NewRow();
                    dr[0] = "&nbsp;&nbsp;&nbsp;&nbsp;" + GetTypeIcon(EkEnumeration.CMSObjectTypes.User.GetHashCode(), EkEnumeration.CMSContentSubtype.Content) + "<input type=\"checkbox\" id=\"itemlist\" name=\"itemlist\" value=\"" + userList[j].Id + "\"/>" + (!string.IsNullOrEmpty(userList[j].DisplayName) ? userList[j].DisplayName : userList[j].Username);
                    dt.Rows.Add(dr);
                }
            }
        }
        else if (this.m_LocaleObjectType == EkEnumeration.TaxonomyItemType.Locale)
        {
            List<int> langList = new List<int>();
            Ektron.Cms.BusinessObjects.Localization.LocaleTaxonomy api = new Ektron.Cms.BusinessObjects.Localization.LocaleTaxonomy(m_refCommonApi.RequestInformationRef);
            langList = api.GetLocaleIdList(TaxonomyId, TaxonomyLanguage,true);
            Ektron.Cms.Framework.Localization.LocaleManager localizationApi = new Ektron.Cms.Framework.Localization.LocaleManager();
            List<Ektron.Cms.Localization.LocaleData> locData = localizationApi.GetEnabledLocales();
            ////Disable the checkbox for Default Language.and loop through all the enabled Languages.
            for (int k = 0; k < locData.Count; k++)
            {
                Boolean taxonomyItemAlreadyExists = langList.Contains(locData[k].Id);
                //// Boolean isTaxonomyItemDefault = langList.Contains(TaxonomyLanguage);
                if (!taxonomyItemAlreadyExists)
                {
                    if (locData[k].Id == TaxonomyLanguage)
                    {
                        dr = dt.NewRow();
                        dr[0] = "&nbsp;&nbsp;&nbsp;&nbsp;<input type=\"checkbox\" id=\"itemlist\" disabled name=\"itemlist\" value=\"" + locData[k].Id + "\"/>&nbsp;&nbsp;<img src='" + objLocalizationApi.GetFlagUrlByLanguageID(locData[k].Id) + "' />&nbsp;&nbsp;" + locData[k].CombinedName;
                        dt.Rows.Add(dr);
                    }
                    else
                    {
                        dr = dt.NewRow();
                        dr[0] = "&nbsp;&nbsp;&nbsp;&nbsp;<input type=\"checkbox\" id=\"itemlist\" name=\"itemlist\" value=\"" + locData[k].Id + "\"/>&nbsp;&nbsp;<img src='" + objLocalizationApi.GetFlagUrlByLanguageID(locData[k].Id) + "' />&nbsp;&nbsp;" + locData[k].CombinedName;
                        dt.Rows.Add(dr);
                    }
                }
            }
        }
        DataView dv = new DataView(dt);
        TaxonomyItemList.DataSource = dv;
        TaxonomyItemList.DataBind();
    }

    /// <summary>
    /// Handles the Getting all the users based on the search text passed by the user on UI.
    /// </summary>
    private void GetUsers()
    {
        if (!string.IsNullOrEmpty(Strings.Trim(this.m_strSearchText)))
        {
            UserRequestData req = new UserRequestData();
            UserAPI m_refUserApi = new UserAPI();
            req.Type = (this.m_UserType == EkEnumeration.UserTypes.AuthorType ? 0 : 1);
            req.Group = (this.m_UserType == EkEnumeration.UserTypes.AuthorType ? 2 : 888888);
            req.RequiredFlag = 0;
            req.OrderBy = string.Empty;
            req.OrderDirection = "asc";
            req.SearchText = this.m_strSearchText;
            req.PageSize = m_refContentApi.RequestInformationRef.PagingSize;
            req.CurrentPage = this.m_intCurrentPage;
            userList = m_refUserApi.GetAllUsers(ref req);
            this.m_intTotalPages = req.TotalPages;
        }
    }

    /// <summary>
    /// Handles the loading the Assigned Community Groups for a provided locale taxonmomy.
    /// </summary>
    private void GetAssignedCommunityGroups()
    {
        if (Page.IsPostBack)
        {
            DirectoryGroupRequest cReq = new DirectoryGroupRequest();
            cReq.CurrentPage = this.m_intCurrentPage;
            cReq.PageSize = m_refCommonApi.RequestInformationRef.PagingSize;
            cReq.DirectoryId = TaxonomyId;
            cReq.DirectoryLanguage = TaxonomyLanguage;
            cReq.GetItems = true;
            cReq.SortDirection = string.Empty;
            groupData = m_refCommonApi.CommunityGroupRef.LoadDirectory(ref cReq);
        }
    }

    /// <summary>
    /// Returns if the group exists in the provided Directory List
    /// </summary>
    /// <param name="GroupID">Group Id to check in the list of Community Group Data.</param>
    /// <returns>Returns true if the group exists in the provided Directory List</returns>
    private bool DoesGroupExistInList(long GroupID)
    {
        if (this.groupData != null && groupData.DirectoryItems != null && groupData.DirectoryItems.Length > 0)
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

    /// <summary>
    /// Handles Code for getting all the community groups from CMS Database.
    /// </summary>
    private void GetCommunityGroups()
    {
        if (Page.IsPostBack)
        {
            CommunityGroupRequest cReq = new CommunityGroupRequest();
            cReq.CurrentPage = this.m_intCurrentPage;
            cReq.SearchText = this.m_strKeyWords;
            cReq.PageSize = m_refContentApi.RequestInformationRef.PagingSize;
            cgroup_list = (new Ektron.Cms.Community.CommunityGroupAPI()).GetAllCommunityGroups(cReq);
            this.m_intTotalPages = cReq.TotalPages;
        }
    }

    /// <summary>
    /// Handles Code for forming the search text passed by the user and forming the search query to send to the ektron search.
    /// </summary>
    private void CollectSearchText()
    {
        this.m_strKeyWords = Request.Form["txtSearch"];
        this.m_strSelectedItem = Request.Form["searchlist"];
        if ((this.m_strSelectedItem == "-1"))
        {
            this.m_strSearchText = " (first_name like '%" + Quote(this.m_strKeyWords) + "%' OR last_name like '%" + Quote(this.m_strKeyWords) + "%' OR user_name like '%" + Quote(this.m_strKeyWords) + "%') AND u.user_id not in (select taxonomy_item_id from taxonomy_item_tbl where taxonomy_item_type=1 and taxonomy_id=" + this.TaxonomyId + ")";
        }
        else if ((this.m_strSelectedItem == "last_name"))
        {
            this.m_strSearchText = " (last_name like '%" + Quote(this.m_strKeyWords) + "%') AND u.user_id not in (select taxonomy_item_id from taxonomy_item_tbl where taxonomy_item_type=1 and taxonomy_id=" + this.TaxonomyId + ")";
        }
        else if ((this.m_strSelectedItem == "first_name"))
        {
            this.m_strSearchText = " (first_name like '%" + Quote(this.m_strKeyWords) + "%') AND u.user_id not in (select taxonomy_item_id from taxonomy_item_tbl where taxonomy_item_type=1 and taxonomy_id=" + this.TaxonomyId + ")";
        }
        else if ((this.m_strSelectedItem == "user_name"))
        {
            this.m_strSearchText = " (user_name like '%" + Quote(this.m_strKeyWords) + "%') AND u.user_id not in (select taxonomy_item_id from taxonomy_item_tbl where taxonomy_item_type=1 and taxonomy_id=" + this.TaxonomyId + ")";
        }
    }

    /// <summary>
    /// Returns if the content grid data result for a folder id.
    /// </summary>
    /// <param name="id">folder id.</param>
    /// <returns>html mark up of the Onclick event for the folder in locale taxonomy.</returns>
    private string OnClickEvent(object id)
    {
        string result = "";
        if ((this.m_strPageAction != "additem"))
        {
            result = " onclick=\"OnFolderCheck(" + id + ",this);\"";
        }
        return result;
    }

    /// <summary>
    /// Returns page icons for different types of the Locale taxonomies.
    /// </summary>
    /// <param name="type">ek enumeration of the type of the content for locale.</param>
    /// <param name="subType">ek enumeration of the sub type of the content for locale.</param>
    /// <returns>content icon for the differnet types of locale taxonomies.</returns>
    private string GetTypeIcon(int type, EkEnumeration.CMSContentSubtype subType)
    {
        if (type == EkEnumeration.CMSObjectTypes.User.GetHashCode() && this.m_ObjectType == EkEnumeration.CMSObjectTypes.User)
        {
            return UserIcon;
        }
        else if (type == 2 && this.m_ObjectType == EkEnumeration.CMSObjectTypes.Content)
        {
            return FormsIcon;
        }
        else if (type == 1)
        {
            if ((subType == EkEnumeration.CMSContentSubtype.PageBuilderData | subType == EkEnumeration.CMSContentSubtype.PageBuilderMasterData))
            {
                return pageIcon;
            }
            return ContentIcon;
        }
        else
        {
            return null;
        }
    }

    /// <summary>
    /// Handles Populating the taxonomy tool bar at the top of the locale taxonomies in Workareawhen assigning items to locale taxonomy..
    /// </summary>
    private void TaxonomyToolBar()
    {
        if ((m_strPageAction != "additem"))
        {
            divTitleBar.InnerHtml = m_refstyle.GetTitleBar(m_refMsg.GetMessage("assign folders to locale taxonomy page title"));
        }
        else
        {
            divTitleBar.InnerHtml = m_refstyle.GetTitleBar(m_refMsg.GetMessage("assign items to locale taxonomy page title"));
        }

        System.Text.StringBuilder result = new System.Text.StringBuilder();
        result.Append("<table><tr>" + Constants.vbCrLf);

		if ((Request.QueryString["iframe"] == "true"))
		{
			result.Append(m_refstyle.GetButtonEventsWCaption(AppPath + "images/UI/Icons/cancel.png", "#", m_refMsg.GetMessage("generic Cancel"), m_refMsg.GetMessage("generic Cancel"), "onclick=\"parent.CancelIframe();\"", StyleHelper.CancelButtonCssClass, true));
		}
		else
		{
			result.Append(m_refstyle.GetButtonEventsWCaption(AppPath + "images/UI/Icons/back.png", "LocaleTaxonomy.aspx?view=" + _ViewItem + "&action=view&taxonomyid=" + TaxonomyId, m_refMsg.GetMessage("alt back button text"), m_refMsg.GetMessage("btn back"), string.Empty, StyleHelper.BackButtonCssClass, true));
		}

		result.Append(m_refstyle.GetButtonEventsWCaption(AppPath + "images/UI/Icons/save.png", "#", m_refMsg.GetMessage("alt update button text (locale taxonomy)"), m_refMsg.GetMessage("btn locale update"), "onclick=\"Validate();\"", StyleHelper.SaveButtonCssClass, true));
        
        if ((m_strPageAction == "additem"))
        {
            if ((!((asset_data == null))))
            {
                if ((asset_data.Length > 0))
                {
                    result.Append("<td>&nbsp;</td>");
                    result.Append("<td><select id=selAssetSupertype name=selAssetSupertype OnChange=\"UpdateView();\">");
                    if (Ektron.Cms.Common.EkConstants.CMSContentType_NonLibraryContent == SelectedContentType)
                    {
                        result.Append("<option value='" + Ektron.Cms.Common.EkConstants.CMSContentType_NonLibraryContent + "' selected>" + this.m_refMsg.GetMessage("lbl all types") + "</option>");
                    }
                    else
                    {
                        result.Append("<option value='" + Ektron.Cms.Common.EkConstants.CMSContentType_AllTypes + "'>" + this.m_refMsg.GetMessage("lbl all types") + "</option>");
                    }
                    if (Ektron.Cms.Common.EkConstants.CMSContentType_Content == SelectedContentType)
                    {
                        result.Append("<option value='" + Ektron.Cms.Common.EkConstants.CMSContentType_Content + "' selected>" + this.m_refMsg.GetMessage("lbl html content") + "</option>");
                    }
                    else
                    {
                        result.Append("<option value='" + Ektron.Cms.Common.EkConstants.CMSContentType_Content + "'>" + this.m_refMsg.GetMessage("lbl html content") + "</option>");
                    }
                    foreach (AssetInfoData data in asset_data)
                    {
                        if ((Ektron.Cms.Common.EkConstants.ManagedAsset_Min <= data.TypeId & data.TypeId <= Ektron.Cms.Common.EkConstants.ManagedAsset_Max))
                        {
                            if ("*" != data.PluginType)
                            {
                                result.Append("<option value='" + data.TypeId + "'");
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
        if ((m_strPageAction != "addfolder"))
        {
            result.Append("<td>" + m_refstyle.GetHelpButton("AddLocaleTaxonomyItem", "") + "</td>");
        }
        else
        {
            result.Append("<td>" + m_refstyle.GetHelpButton("AddLocaleTaxonomyFolder", "") + "</td>");
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
    /// <summary>
    /// return a string for selected items in dropdown list of the users in locale taxonomy. 
    /// </summary>
    /// <param name="val">value to compare</param>
    /// <returns>selected string to the drop down of users in locale taxonomy.</returns>
    private string IsSelected(string val)
    {
        if ((val == this.m_strSelectedItem))
        {
            return (" selected ");
        }
        else
        {
            return (string.Empty);
        }
    }

    /// <summary>
    /// return a string by replacing the '' with ' for a provided input string.
    /// </summary>
    /// <param name="KeyWords">KeyWords to replace.</param>
    /// <returns>result string with replaced text.</returns>
    private string Quote(string KeyWords)
    {
        string result = KeyWords;
        if ((KeyWords.Length > 0))
        {
            result = KeyWords.Replace("'", "''");
        }
        return result;
    }

    /// <summary>
    /// Handles registering all the Javascript file sand CSS files.
    /// </summary>
    private void RegisterResources()
    {
        ApplicationPathLocale.Value = m_refContentApi.ApplicationPath;
        if ((m_strPageAction == "addfolder"))
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
            Ektron.Cms.API.JS.RegisterJS(this, m_refContentApi.ApplicationPath + "Tree/js/com.ektron.ui.explore.js", "UIExploreJSab");
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

    /// <summary>
    /// Handles setting the page settings for the Display Contents.
    /// </summary>
    private void PageSettings()
    {
        if ((this.m_intTotalPages <= 1))
        {
            VisiblePageControls(false);
        }
        else
        {
            this.VisiblePageControls(true);
            TotalPages.Text = (System.Math.Ceiling(Convert.ToDouble(this.m_intTotalPages))).ToString();
            CurrentPage.Text = this.m_intCurrentPage.ToString();
            PreviousPage.Enabled = true;
            FirstPage.Enabled = true;
            NextPage.Enabled = true;
            LastPage.Enabled = true;
            if (this.m_intCurrentPage == 1)
            {
                PreviousPage.Enabled = false;
                FirstPage.Enabled = false;
            }
            else if (this.m_intCurrentPage == this.m_intTotalPages)
            {
                NextPage.Enabled = false;
                LastPage.Enabled = false;
            }
        }
    }

    /// <summary>
    /// Handles setting the page controls visible based on provided flag.
    /// </summary>
    ///  <param name="flag">value of flag</param>
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

    public string getURL()
    {
        string sRet = "";
        if (Request.QueryString.Count > 0)
        {
            for (int i = 0; i <= (Request.QueryString.Count - 1); i++)
            {
                if ((Request.QueryString.Keys[i].ToLower() != "type" && Request.QueryString.Keys[i].ToLower() != "contfetchtype"))
                {
                    sRet += Request.QueryString.Keys[i] + "=" + Request.QueryString[i] + "&";
                }
            }
        }
        if (sRet.Length > 0 && sRet[sRet.Length - 1].ToString() == "&")
        {
            sRet = "LocaleTaxonomy.aspx?" + sRet.Substring(0, sRet.Length - 1);
        }
        else
        {
            sRet = "LocaleTaxonomy.aspx?" + sRet;
        }
        if (sRet.ToLower().IndexOf("langtype") < 0)
        {
            sRet = sRet + "&LangType=" + m_refContentApi.RequestInformationRef.ContentLanguage;
        }
        return sRet;
    }

}

