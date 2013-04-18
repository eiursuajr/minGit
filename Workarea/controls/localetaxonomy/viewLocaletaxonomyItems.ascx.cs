using System;
using System.Data;
using Ektron.Cms;
using Ektron.Cms.Common;
using System.Collections.Generic;
using System.Web.UI.WebControls;
using System.IO;
using System.Text;

partial class viewLocaletaxonomyItems : System.Web.UI.UserControl
{
    protected CommonApi m_refCommon = new CommonApi();
    protected StyleHelper m_refstyle = new StyleHelper();
    protected string AppImgPath = "";
    protected EkMessageHelper m_refMsg;
    protected string m_strPageAction = "";
    protected Ektron.Cms.Content.EkContent m_refContent;
    protected long TaxonomyId = 0;
    protected int TaxonomyLanguage = -1;
    protected LanguageData language_data;
    protected TaxonomyRequest taxonomy_request;
    protected TaxonomyData taxonomy_data;
    protected long TaxonomyParentId = 0;
    protected string m_strViewItem = "item";
    protected bool AddDeleteIcon = false;
    protected string m_strTaxonomyName = "";
    protected int m_intCurrentPage = 1;
    protected int m_intTotalPages = 1;
    protected string m_strDelConfirm = "";
    protected string m_strDelItemsConfirm = "";
    protected string m_strSelDelWarning = "";

    protected LocalizationAPI objLocalizationApi = new LocalizationAPI();

    protected void Page_Load(object sender, System.EventArgs e)
    {
        m_refMsg = m_refCommon.EkMsgRef;
        AppImgPath = m_refCommon.AppImgPath;
        m_strPageAction = Request.QueryString["action"];
        //object refAPI = m_refCommon as object;
        Utilities.SetLanguage(m_refCommon);
        TaxonomyLanguage = m_refCommon.ContentLanguage;
        TaxonomyId = Convert.ToInt64(Request.QueryString["taxonomyid"]);
        if ((Request.QueryString["view"] != null))
        {
            m_strViewItem = Request.QueryString["view"];
        }
        taxonomy_request = new TaxonomyRequest();
        taxonomy_request.TaxonomyId = TaxonomyId;
        taxonomy_request.TaxonomyLanguage = TaxonomyLanguage;
        m_refContent = m_refCommon.EkContentRef;
        if ((Page.IsPostBack && !string.IsNullOrEmpty(Request.Form[isPostData.UniqueID])))
        {
            if ((Request.Form["submittedaction"] == "delete"))
            {
                m_refContent.DeleteTaxonomy(taxonomy_request);
                Response.Write("<script type=\"text/javascript\">parent.CloseChildPage();</script>");
            }
            else if ((Request.Form["submittedaction"] == "deleteitem"))
            {
                if ((m_strViewItem != "folder"))
                {
                    taxonomy_request.TaxonomyIdList = Request.Form["selected_items"];
                    if ((m_strViewItem.ToLower() == "cgroup"))
                    {
                        taxonomy_request.TaxonomyItemType = EkEnumeration.TaxonomyItemType.Group;
                    }
                    else if ((m_strViewItem.ToLower() == "user"))
                    {
                        taxonomy_request.TaxonomyItemType = EkEnumeration.TaxonomyItemType.User;
                    }
                    else
                    {
                        taxonomy_request.TaxonomyItemType = EkEnumeration.TaxonomyItemType.Content;
                    }
                    m_refContent.RemoveTaxonomyItem(taxonomy_request);
                }
                else
                {
                    TaxonomySyncRequest tax_folder = new TaxonomySyncRequest();
                    tax_folder.TaxonomyId = TaxonomyId;
                    tax_folder.TaxonomyLanguage = TaxonomyLanguage;
                    tax_folder.SyncIdList = Request.Form["selected_items"];
                    m_refContent.RemoveTaxonomyFolder(tax_folder);
                }
                if ((Request.Params["ccp"] == null))
                {
                    Response.Redirect("LocaleTaxonomy.aspx?" + Request.ServerVariables["query_string"] + "&ccp=true", true);
                }
                else
                {
                    Response.Redirect("LocaleTaxonomy.aspx?" + Request.ServerVariables["query_string"], true);
                }
            }
        }
        else if ((IsPostBack == false))
        {
            DisplayPage();
        }
        isPostData.Value = "true";
    }

    private void DisplayPage()
    {
        switch (m_strViewItem.ToLower())
        {
            case "user":
                DirectoryUserRequest uReq = new DirectoryUserRequest();
                DirectoryAdvancedUserData uDirectory = new DirectoryAdvancedUserData();
                uReq.GetItems = true;
                uReq.DirectoryId = TaxonomyId;
                uReq.DirectoryLanguage = TaxonomyLanguage;
                uReq.PageSize = m_refCommon.RequestInformationRef.PagingSize;
                uReq.CurrentPage = m_intCurrentPage;
                uDirectory = this.m_refContent.LoadDirectory(ref uReq);
                if ((uDirectory != null))
                {
                    TaxonomyParentId = uDirectory.DirectoryParentId;
                    m_strTaxonomyName = uDirectory.DirectoryName;
                    m_intTotalPages = uReq.TotalPages;
                }
                PopulateUserGridData(uDirectory);
                TaxonomyToolBar();
                break;
            case "cgroup":
                DirectoryAdvancedGroupData dagdRet = new DirectoryAdvancedGroupData();
                DirectoryGroupRequest cReq = new DirectoryGroupRequest();

                cReq.CurrentPage = m_intCurrentPage;
                cReq.PageSize = m_refCommon.RequestInformationRef.PagingSize;
                cReq.DirectoryId = TaxonomyId;
                cReq.DirectoryLanguage = TaxonomyLanguage;
                cReq.GetItems = true;
                cReq.SortDirection = "";

                dagdRet = this.m_refCommon.CommunityGroupRef.LoadDirectory(ref cReq);
                if ((dagdRet != null))
                {
                    TaxonomyParentId = dagdRet.DirectoryParentId;
                    m_strTaxonomyName = dagdRet.DirectoryName;
                    m_intTotalPages = cReq.TotalPages;
                }
                m_intTotalPages = cReq.TotalPages;
                PopulateCommunityGroupGridData(dagdRet);
                TaxonomyToolBar();
                break;
            default:
                // Content
                taxonomy_request.IncludeItems = true;
                taxonomy_request.PageSize = m_refCommon.RequestInformationRef.PagingSize;
                taxonomy_request.CurrentPage = m_intCurrentPage;
                taxonomy_data = m_refContent.ReadTaxonomy(ref taxonomy_request);
                if ((taxonomy_data != null))
                {
                    TaxonomyParentId = taxonomy_data.TaxonomyParentId;
                    m_strTaxonomyName = taxonomy_data.TaxonomyName;
                    m_intTotalPages = taxonomy_request.TotalPages;
                }
                PopulateContentGridData();
                TaxonomyToolBar();
                break;
        }
    }
    private string ConfigName(int id)
    {
        switch (id)
        {
            case 0:
                return "Content";
            case 1:
                return "User";
            case 2:
                return "Group";
            default:
                return "Content";
        }
    }
    private void PopulateCommunityGroupGridData(DirectoryAdvancedGroupData cgDirectory)
    {
        TaxonomyItemList.Columns.Add(m_refstyle.CreateBoundField("CHECK", "<input type=\"Checkbox\" name=\"checkall\" onclick=\"javascript:checkAll('selected_items',false);\">", "title-header", HorizontalAlign.Center, HorizontalAlign.Center, Unit.Percentage(3), Unit.Percentage(2), false, false));
        TaxonomyItemList.Columns.Add(m_refstyle.CreateBoundField("ID", m_refMsg.GetMessage("generic id"), "title-header", HorizontalAlign.Left, HorizontalAlign.NotSet, Unit.Percentage(5), Unit.Percentage(5), false, false));
        TaxonomyItemList.Columns.Add(m_refstyle.CreateBoundField("COMMUNITYGROUP", m_refMsg.GetMessage("lbl community group"), "title-header", HorizontalAlign.Left, HorizontalAlign.NotSet, Unit.Percentage(30), Unit.Percentage(41), false, true));
        TaxonomyItemList.Columns.Add(m_refstyle.CreateBoundField("INFORMATION", "&#160;", "title-header", HorizontalAlign.Left, HorizontalAlign.NotSet, Unit.Percentage(30), Unit.Percentage(41), false, false));

        TaxonomyItemList.Columns[2].ItemStyle.VerticalAlign = VerticalAlign.Top;
        TaxonomyItemList.Columns[3].ItemStyle.VerticalAlign = VerticalAlign.Top;

        DataTable dt = new DataTable();
        DataRow dr = default(DataRow);
        dt.Columns.Add(new DataColumn("CHECK", typeof(string)));
        dt.Columns.Add(new DataColumn("ID", typeof(string)));
        dt.Columns.Add(new DataColumn("COMMUNITYGROUP", typeof(string)));
        dt.Columns.Add(new DataColumn("INFORMATION", typeof(string)));
        PageSettings();
        if ((cgDirectory != null && cgDirectory.DirectoryItems != null && cgDirectory.DirectoryItems.Length > 0))
        {
            AddDeleteIcon = true;
            foreach (CommunityGroupData item in cgDirectory.DirectoryItems)
            {
                dr = dt.NewRow();
                dr["CHECK"] = "<input type=\"checkbox\" name=\"selected_items\" id=\"selected_items\" value=\"" + item.GroupId + "\" onClick=\"javascript:checkAll('selected_items',true);\">";
                dr["COMMUNITYGROUP"] = "<img src=\"" + (!string.IsNullOrEmpty(item.GroupImage) ? item.GroupImage : this.m_refCommon.AppImgPath + "member_default.gif") + "\" align=\"left\" width=\"55\" height=\"55\" />";
                dr["COMMUNITYGROUP"] += item.GroupName;
                dr["COMMUNITYGROUP"] += " (" + (item.GroupEnroll ? this.m_refMsg.GetMessage("lbl enrollment open") : this.m_refMsg.GetMessage("lbl enrollment restricted")) + ")";
                dr["COMMUNITYGROUP"] += "<br/>";
                dr["COMMUNITYGROUP"] += item.GroupShortDescription;

                dr["ID"] = item.GroupId;

                dr["INFORMATION"] = this.m_refMsg.GetMessage("content dc label") + " " + item.GroupCreatedDate.ToShortDateString();
                dr["INFORMATION"] += "<br/>";
                dr["INFORMATION"] += this.m_refMsg.GetMessage("lbl members") + ": " + item.TotalMember.ToString();
                dt.Rows.Add(dr);
            }
        }
        else
        {
            dr = dt.NewRow();
            dt.Rows.Add(dr);
            TaxonomyItemList.GridLines = GridLines.None;
        }
        DataView dv = new DataView(dt);
        TaxonomyItemList.DataSource = dv;
        TaxonomyItemList.DataBind();
    }
    private void PopulateUserGridData(DirectoryAdvancedUserData uDirectory)
    {
        TaxonomyItemList.Columns.Add(m_refstyle.CreateBoundField("CHECK", "<input type=\"Checkbox\" name=\"checkall\" onclick=\"javascript:checkAll('selected_items',false);\">", "title-header", HorizontalAlign.Center, HorizontalAlign.Center, Unit.Percentage(3), Unit.Percentage(2), false, false));
        TaxonomyItemList.Columns.Add(m_refstyle.CreateBoundField("ID", m_refMsg.GetMessage("generic id"), "title-header", HorizontalAlign.Left, HorizontalAlign.NotSet, Unit.Percentage(5), Unit.Percentage(5), false, false));
        TaxonomyItemList.Columns.Add(m_refstyle.CreateBoundField("USERNAME", m_refMsg.GetMessage("generic username"), "title-header", HorizontalAlign.Left, HorizontalAlign.NotSet, Unit.Percentage(30), Unit.Percentage(41), false, false));
        TaxonomyItemList.Columns.Add(m_refstyle.CreateBoundField("DISPLAYNAME", m_refMsg.GetMessage("display name label"), "title-header", HorizontalAlign.Left, HorizontalAlign.NotSet, Unit.Percentage(30), Unit.Percentage(41), false, false));
        DataTable dt = new DataTable();
        DataRow dr = default(DataRow);
        dt.Columns.Add(new DataColumn("CHECK", typeof(string)));
        dt.Columns.Add(new DataColumn("ID", typeof(string)));
        dt.Columns.Add(new DataColumn("USERNAME", typeof(string)));
        dt.Columns.Add(new DataColumn("DISPLAYNAME", typeof(string)));
        PageSettings();
        if ((uDirectory != null && uDirectory.DirectoryItems != null && uDirectory.DirectoryItems.Length > 0))
        {
            AddDeleteIcon = true;
            foreach (DirectoryUserData item in uDirectory.DirectoryItems)
            {
                dr = dt.NewRow();
                dr["CHECK"] = "<input type=\"checkbox\" name=\"selected_items\" id=\"selected_items\" value=\"" + item.Id + "\" onClick=\"javascript:checkAll('selected_items',true);\">";
                dr["USERNAME"] = item.Username;
                //"<a href=""taxonomy.aspx?action=viewtree&taxonomyid=" & item.TaxonomyItemId & "&LangType=" & item.TaxonomyItemLanguage & """>" & item.TaxonomyItemTitle & "</a>"
                dr["ID"] = item.Id;
                dr["DISPLAYNAME"] = item.DisplayName;
                dt.Rows.Add(dr);
            }
        }
        else
        {
            dr = dt.NewRow();
            dt.Rows.Add(dr);
            TaxonomyItemList.GridLines = GridLines.None;
        }
        DataView dv = new DataView(dt);
        TaxonomyItemList.DataSource = dv;
        TaxonomyItemList.DataBind();
    }
    private void PopulateContentGridData()
    {
        if ((m_strPageAction == "removeitems"))
        {
            TaxonomyItemList.Columns.Add(m_refstyle.CreateBoundField("CHECK", "<input type=\"Checkbox\" name=\"checkall\" onclick=\"javascript:checkAll('selected_items',false);\">", "title-header", HorizontalAlign.Center, HorizontalAlign.Center, Unit.Percentage(2), Unit.Percentage(2), false, false));
        }
        TaxonomyItemList.Columns.Add(m_refstyle.CreateBoundField("TITLE", m_refMsg.GetMessage("generic title"), "title-header", HorizontalAlign.Left, HorizontalAlign.NotSet, Unit.Percentage(30), Unit.Percentage(88), false, false));
        TaxonomyItemList.Columns.Add(m_refstyle.CreateBoundField("ID", m_refMsg.GetMessage("generic id"), "title-header", HorizontalAlign.Left, HorizontalAlign.NotSet, Unit.Percentage(5), Unit.Percentage(5), false, false));
        TaxonomyItemList.Columns.Add(m_refstyle.CreateBoundField("LANGUAGE", m_refMsg.GetMessage("generic language"), "title-header", HorizontalAlign.Left, HorizontalAlign.NotSet, Unit.Percentage(5), Unit.Percentage(5), false, false));
        DataTable dt = new DataTable();
        DataRow dr = default(DataRow);
        ContentAPI m_refContentApi = new ContentAPI();
        if ((m_strPageAction == "removeitems"))
        {
            dt.Columns.Add(new DataColumn("CHECK", typeof(string)));
        }
        dt.Columns.Add(new DataColumn("TITLE", typeof(string)));
        dt.Columns.Add(new DataColumn("ID", typeof(string)));
        dt.Columns.Add(new DataColumn("LANGUAGE", typeof(string)));
        if ((m_strViewItem != "folder"))
        {
            PageSettings();
            if ((taxonomy_data != null && taxonomy_data.TaxonomyItems != null && taxonomy_data.TaxonomyItems.Length > 0))
            {
                AddDeleteIcon = true;
                string icon = "";
                string title = "";
                string link = "";
                string backPage = "";
                foreach (TaxonomyItemData item in taxonomy_data.TaxonomyItems)
                {
                    dr = dt.NewRow();
                    if ((m_strPageAction == "removeitems"))
                    {
                        dr["CHECK"] = "<input type=\"checkbox\" name=\"selected_items\" id=\"selected_items\" value=\"" + item.TaxonomyItemId + "\" onClick=\"javascript:checkAll('selected_items',true);\">";
                    }
                    if (item.ContentType == "1")
                    {
                        if ((item.ContentSubType == Ektron.Cms.Common.EkEnumeration.CMSContentSubtype.PageBuilderData))
                        {
                            icon = "&nbsp;<img src=\"" + m_refContentApi.AppPath + "images/ui/icons/layout.png\" />&nbsp;";
                        }
                        else if ((item.ContentSubType == EkEnumeration.CMSContentSubtype.WebEvent))
                        {
                            icon = "&nbsp;<img src=\"" + m_refContentApi.AppPath + "images/ui/icons/calendar.png" + "\" />&nbsp;";
                        }
                        else
                        {
                            icon = "&nbsp;<img src=\"" + m_refContentApi.AppPath + "images/ui/icons/contentHtml.png" + "\" />&nbsp;";
                        }
                    }
                    else if (item.ContentType == "2")
                    {
                        icon = "&nbsp;<img src=\"" + m_refContentApi.AppPath + "images/ui/icons/contentForm.png" + "\" />&nbsp;";
                    }
                    else if (item.ContentType == "1111")
                    {
                        icon = "&nbsp;<img src=\"" + m_refContentApi.AppPath + "images/ui/icons/tree/folderBoard.png" + "\" alt=\"" + item.TaxonomyItemAssetInfo.FileName + "\" />&nbsp;";
                    }
                    else if (item.ContentType == "1112")
                    {
                        icon = "&nbsp;<img src=\"" + m_refContentApi.AppPath + "images/ui/icons/tree/folderBlog.png" + "\" alt=\"" + item.TaxonomyItemAssetInfo.FileName + "\" />&nbsp;";
                    }
                    else if (item.ContentType == "3333")
                    {
                        icon = "&nbsp;<img src=\"" + m_refContentApi.AppPath + "images/ui/icons/brick.png" + "\" alt=\"" + item.TaxonomyItemAssetInfo.FileName + "\" />&nbsp;";
                    }
                    else
                    {
                        icon = "&nbsp;<img src=\"" + item.TaxonomyItemAssetInfo.ImageUrl + "\" alt=\"" + item.TaxonomyItemAssetInfo.FileName + "\" />&nbsp;";
                    }
                    if (string.IsNullOrEmpty(item.TaxonomyItemAssetInfo.ImageUrl) & (item.ContentType != "1" & item.ContentType != "2" & item.ContentType != "1111" & item.ContentType != "1112" & item.ContentType != "3333"))
                    {
                        icon = "&nbsp;<img src=\"" + m_refContentApi.AppPath + "images/UI/Icons/book.png" + "\" alt=\"" + item.TaxonomyItemAssetInfo.FileName + "\" />&nbsp;";
                    }

                    string taxItemTitle = item.TaxonomyItemTitle;
                     
                    title = m_refMsg.GetMessage("generic View") + " \"" +  taxItemTitle.Replace(" '", "`") + "\"";
                    backPage = EkFunctions.UrlEncode("Action=viewcontent&view=item&taxonomyid=" + TaxonomyId);
                    if ((item.ContentSubType == EkEnumeration.CMSContentSubtype.WebEvent && item.ContentType == "1"))
                    {
                        long fid = m_refContentApi.EkContentRef.GetFolderIDForContent(item.TaxonomyItemId);
                        link = "<a href='content.aspx?action=ViewContentByCategory&LangType=" + item.TaxonomyItemLanguage + "&id=" + fid + "&callerpage=LocaleTaxonomy.aspx&origurl=" + backPage + "' title='" + title + "'>" + item.TaxonomyItemTitle + "</a>";
                    }
                    else
                    {
                        link = "<a href='content.aspx?action=View&LangType=" + item.TaxonomyItemLanguage + "&id=" + item.TaxonomyItemId + "&callerpage=LocaleTaxonomy.aspx&origurl=" + backPage + "' title='" + title + "'>" + item.TaxonomyItemTitle + "</a>";
                    }
                    dr["TITLE"] = icon + link;
                    dr["ID"] = item.TaxonomyItemId;
                    dr["LANGUAGE"] = item.TaxonomyItemLanguage;
                    dt.Rows.Add(dr);
                }
            }
            else
            {
                dr = dt.NewRow();
                dt.Rows.Add(dr);
                TaxonomyItemList.GridLines = GridLines.None;
            }
        }
        else
        {
            VisiblePageControls(false);
            TaxonomyFolderSyncData[] taxonomy_sync_folder = null;
            TaxonomyBaseRequest tax_sync_folder_req = new TaxonomyBaseRequest();
            tax_sync_folder_req.TaxonomyId = TaxonomyId;
            tax_sync_folder_req.TaxonomyLanguage = TaxonomyLanguage;
            taxonomy_sync_folder = m_refContent.GetAllAssignedCategoryFolder(tax_sync_folder_req);
            if ((taxonomy_sync_folder != null && taxonomy_sync_folder.Length > 0))
            {
                AddDeleteIcon = true;
                for (int i = 0; i <= taxonomy_sync_folder.Length - 1; i++)
                {
                    dr = dt.NewRow();
                    dr["CHECK"] = "<input type=\"checkbox\" name=\"selected_items\" id=\"selected_items\" value=\"" + taxonomy_sync_folder[i].FolderId + "\" onClick=\"javascript:checkAll('selected_items',true);\">";
                    dr["TITLE"] = taxonomy_sync_folder[i].FolderTitle;
                    //& GetRecursiveTitle(item.FolderRecursive)
                    dr["ID"] = taxonomy_sync_folder[i].FolderId;
                    dr["LANGUAGE"] = taxonomy_sync_folder[i].TaxonomyLanguage;
                    dt.Rows.Add(dr);
                }
            }
            else
            {
                dr = dt.NewRow();
                dt.Rows.Add(dr);
                TaxonomyItemList.GridLines = GridLines.None;
            }
        }
        DataView dv = new DataView(dt);
        TaxonomyItemList.DataSource = dv;
        TaxonomyItemList.DataBind();
    }
    private string GetRecursiveTitle(bool value)
    {
        string result = "";
        if ((value))
        {
            result = "<span class=\"important\"> (Recursive)</span>";
        }
        return result;
    }
    private void TaxonomyToolBar()
    {
        string strDeleteMsg = "";
        if ((TaxonomyParentId > 0))
        {
            strDeleteMsg = m_refMsg.GetMessage("alt delete button text (category)");
            m_strDelConfirm = m_refMsg.GetMessage("delete category confirm");
            m_strDelItemsConfirm = m_refMsg.GetMessage("delete category items confirm");
            m_strSelDelWarning = m_refMsg.GetMessage("select category item missing warning");
        }
        else
        {
            strDeleteMsg = m_refMsg.GetMessage("alt delete button text (taxonomy)");
            m_strDelConfirm = m_refMsg.GetMessage("delete taxonomy confirm");
            m_strDelItemsConfirm = m_refMsg.GetMessage("delete taxonomy items confirm");
            m_strSelDelWarning = m_refMsg.GetMessage("select taxonomy item missing warning");
        }
        divTitleBar.InnerHtml = m_refstyle.GetTitleBar(m_refMsg.GetMessage("view taxonomy page title") + " \"" + EkFunctions.HtmlEncode(m_strTaxonomyName) + "\"" + "&nbsp;&nbsp;<img style='vertical-align:middle;' src='" + objLocalizationApi.GetFlagUrlByLanguageID(TaxonomyLanguage) + "' />");
        System.Text.StringBuilder result = new System.Text.StringBuilder();
        result.Append("<table><tr>");

		if ((m_strPageAction != "viewcontent"))
		{
			result.Append(m_refstyle.GetButtonEventsWCaption(AppImgPath + "../UI/Icons/back.png", "LocaleTaxonomy.aspx?action=viewcontent&view=item&taxonomyid=" + TaxonomyId, m_refMsg.GetMessage("alt back button text"), m_refMsg.GetMessage("btn back"), "", StyleHelper.BackButtonCssClass, true));
		}

        if ((m_strPageAction != "removeitems"))
        {
            result.Append(m_refstyle.GetButtonEventsWCaption(AppImgPath + "../UI/Icons/add.png", "LocaleTaxonomy.aspx?action=additem&taxonomyid=" + TaxonomyId + "&parentid=" + TaxonomyParentId, m_refMsg.GetMessage("lbl assign items"), m_refMsg.GetMessage("lbl assign items"), "", StyleHelper.AddButtonCssClass, true));
            result.Append(m_refstyle.GetButtonEventsWCaption(AppImgPath + "../UI/Icons/remove.png", "LocaleTaxonomy.aspx?action=removeitems&taxonomyid=" + TaxonomyId + "&parentid=" + TaxonomyParentId, m_refMsg.GetMessage("remove taxonomy items"), m_refMsg.GetMessage("remove taxonomy items"), "", StyleHelper.RemoveButtonCssClass));
            result.Append(m_refstyle.GetButtonEventsWCaption(AppImgPath + "../UI/Icons/properties.png", "LocaleTaxonomy.aspx?action=viewattributes&taxonomyid=" + TaxonomyId + "&parentid=" + TaxonomyParentId + "&LangType=" + TaxonomyLanguage, m_refMsg.GetMessage("btn properties"), m_refMsg.GetMessage("btn properties"), "", StyleHelper.ViewPropertiesButtonCssClass));
            // result.Append(m_refstyle.GetButtonEventsWCaption(AppImgPath & "btn_delete_folder-nm.gif", "#", strDeleteMsg, m_refMsg.GetMessage("btn delete"), "Onclick=""javascript:return DeleteItem();"""))
        }
        else
        {
            if ((AddDeleteIcon))
            {
                result.Append(m_refstyle.GetButtonEventsWCaption(AppImgPath + "../UI/Icons/remove.png", "#", m_refMsg.GetMessage("alt remove button text (taxonomyitems)"), m_refMsg.GetMessage("btn remove"), "Onclick=\"javascript:return DeleteItem('items');\"", StyleHelper.RemoveButtonCssClass, true));
            }
        }

        result.Append("<td nowrap=\"true\">");
        string addDD = null;
        addDD = GetLanguageForTaxonomy(TaxonomyId, "", false, false, "javascript:TranslateTaxonomy(" + TaxonomyId + ", " + TaxonomyParentId + ", this.value);");
        if (!string.IsNullOrEmpty(addDD))
        {
            addDD = "&nbsp;" + m_refMsg.GetMessage("add title") + ":&nbsp;" + addDD;
        }
        if (m_refCommon.EnableMultilingual == 1)
        {
            result.Append("View In:&nbsp;" + GetLanguageForTaxonomy(TaxonomyId, "", true, false, "javascript:LoadLanguage(this.value);") + "&nbsp;" + addDD + "<br>");
        }
        result.Append("</td>");

        if ((m_strPageAction != "viewcontent"))
        {
            result.Append(ViewTypeDropDown());
        }
		result.Append(StyleHelper.ActionBarDivider);
        result.Append("<td>" + m_refstyle.GetHelpButton("ViewLocaleTaxonomy", "") + "</td>");
        result.Append("</tr></table>");
        divToolBar.InnerHtml = result.ToString();
        result = null;
    }
    private string ViewTypeDropDown()
    {
        StringBuilder result = new StringBuilder();
        result.Append("<td class=\"label\">");
        result.Append(m_refMsg.GetMessage("lbl View") + ":");
        result.Append("</td>");
        result.Append("<td>");
        result.Append("<select id=\"selviewtype\" name=\"selviewtype\" onchange=\"javascript:LoadViewType(this.value);\">");
        result.Append("<option value=\"locale\"  " + FindSelected("locale") + ">").Append(this.m_refMsg.GetMessage("lbl languages button text")).Append("</option>");
        result.Append("<option value=\"folder\" " + FindSelected("folder") + ">").Append(this.m_refMsg.GetMessage("lbl folders")).Append("</option>");
        result.Append("<option value=\"item\"  " + FindSelected("item") + ">").Append(this.m_refMsg.GetMessage("content button text")).Append("</option>");
        result.Append("<option value=\"user\"  " + FindSelected("user") + ">").Append(this.m_refMsg.GetMessage("generic users")).Append("</option>");
        result.Append("<option value=\"cgroup\"  " + FindSelected("cgroup") + ">").Append(this.m_refMsg.GetMessage("lbl community groups")).Append("</option>");
        result.Append("</select>");
        result.Append("</td>");
        return result.ToString();
    }

    private string FindSelected(string chk)
    {
        string val = "";
        if ((m_strViewItem == chk))
        {
            val = " selected ";
        }
        return val;
    }

    private string GetLanguageForTaxonomy(long TaxonomyId, string BGColor, bool ShowTranslated, bool ShowAllOpt, string onChangeEv)
    {
        string result = "";
        string frmName = "";
        IList<LanguageData> result_language = null;
        TaxonomyLanguageRequest taxonomy_language_request = new TaxonomyLanguageRequest();
        taxonomy_language_request.TaxonomyId = TaxonomyId;

        if ((ShowTranslated))
        {
            taxonomy_language_request.IsTranslated = true;
            result_language = m_refContent.LoadLanguageForTaxonomy(taxonomy_language_request);
            frmName = "frm_translated";
        }
        else
        {
            taxonomy_language_request.IsTranslated = false;
            result_language = m_refContent.LoadLanguageForTaxonomy(taxonomy_language_request);
            frmName = "frm_nontranslated";
        }

        result = "<select id=\"" + frmName + "\" name=\"" + frmName + "\" OnChange=\"" + onChangeEv + "\">" + "\n";

        if ((Convert.ToBoolean(ShowAllOpt)))
        {
            if (TaxonomyLanguage == -1)
            {
                result = result + "<option value=\"-1\" selected>All</option>";
            }
            else
            {
                result = result + "<option value=\"-1\">All</option>";
            }
        }
        else
        {
            if ((ShowTranslated == false))
            {
                result = result + "<option value=\"0\">-select language-</option>";
            }
        }
        if (((result_language != null) && (result_language.Count > 0) && (m_refCommon.EnableMultilingual == 1)))
        {
            foreach (LanguageData language in result_language)
            {
                if (TaxonomyLanguage == language.Id)
                {
                    result = result + "<option value=" + language.Id + " selected>" + language.Name + "</option>";
                }
                else
                {
                    result = result + "<option value=" + language.Id + ">" + language.Name + "</option>";
                }
            }
        }
        else
        {
            result = "";
        }
        if ((result.Length > 0))
        {
            result = result + "</select>";
        }
        return (result);
    }
    private void PageSettings()
    {
        if ((m_intTotalPages <= 1))
        {
            VisiblePageControls(false);
        }
        else
        {
            VisiblePageControls(true);
            TotalPages.Text = m_intTotalPages.ToString();
            CurrentPage.Text = m_intCurrentPage.ToString();
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
        switch (e.CommandName)
        {
            case "First":
                m_intCurrentPage = 1;
                break;
            case "Last":
                m_intCurrentPage = Int32.Parse(TotalPages.Text);
                break;
            case "Next":
                m_intCurrentPage = Int32.Parse(CurrentPage.Text) + 1;
                break;
            case "Prev":
                m_intCurrentPage = Int32.Parse(CurrentPage.Text) - 1;
                break;
        }
        DisplayPage();
        isPostData.Value = "true";
    }
}