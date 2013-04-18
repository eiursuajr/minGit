using System;
using System.Collections.Generic;
using System.Text;
using System.Web.UI;
using Ektron.Cms;
using Ektron.Cms.Common;
using Ektron.Cms.Workarea;
using Microsoft.VisualBasic;

public partial class threadeddisc_addedittopic : workareabase
{


    Ektron.ContentDesignerWithValidator ctlEditor;
    long m_iFolderID = 0;
    long m_BoardId = -1;
    PermissionData security_data;
    bool closeOnFinish = false;
    string _TaxonomyTreeIdList = "";
    string _TaxonomyTreeParentIdList = "";
    bool _TaxonomyRoleExists = true;
    long TaxonomyOverrideId = 0;
    List<long> _selectedTaxonomyList = new List<long>();

    protected void Page_Init(object sender, System.EventArgs e)
    {

        ctlEditor = (Ektron.ContentDesignerWithValidator)LoadControl("../controls/Editor/ContentDesignerWithValidator.ascx");
        pnl_message_editor.Controls.Add(ctlEditor);
        ctlEditor.Visible = false;
        ctlEditor.ID = "content_html";
        ctlEditor.ToolsFile = m_refContentApi.ApplicationPath + "ContentDesigner/configurations/InterfaceBlog.aspx?EmoticonSelect=1&WMV=1";

    }

    protected override void Page_Load(object sender, System.EventArgs e)
    {
        base.Page_Load(sender, e);
		Utilities.ValidateUserLogin();
        if (Request.QueryString["folderid"] != "")
        {
            m_iFolderID = Convert.ToInt64(Request.QueryString["folderid"]);
        }

        hdn_uniqueId.Value = Page.UniqueID;

        if (Request.QueryString["close"] != null && Request.QueryString["close"] != "")
        {
            closeOnFinish = true;
        }

        if (Page.IsPostBack)
        {
            m_sPageAction = Request.Form[hdn_action.UniqueID];
            m_iFolderID = Convert.ToInt64(Request.Form[hdn_folderid.UniqueID]);
            m_iID = Convert.ToInt64(Request.Form[hdn_topicid.UniqueID]);
            switch (base.m_sPageAction)
            {
                case "add":
                    Process_Add();
                    break;
                case "edit":
                    Process_Edit();
                    break;
                case "view":
                    Display_View();
                    break;
            }
        }
        else
        {
            switch (base.m_sPageAction)
            {
                case "add":
                    Display_Add();
                    break;
                case "edit":
                    Display_Edit();
                    break;
                case "approve":
                    Process_Approve();
                    break;
                case "view":
                    Display_View();
                    break;
            }

            litTabProp.Text = base.GetMessage("properties text");
            litTabSumm.Text = base.GetMessage("summary text");
            litTabCategories.Text = base.GetMessage("lbl category");
        }
        hdn_action.Value = m_sPageAction;
        hdn_folderid.Value = m_iFolderID.ToString();
        hdn_topicid.Value = m_iID.ToString();
    }


    #region Display
    public void Display_View()
    {
        DiscussionTopic topic_data;
        DiscussionBoard board_data = null;
        string sTitle = "";
        Ektron.Cms.Content.EkContent brContent = m_refContentApi.EkContentRef;
        Ektron.Cms.API.User.User userAPI = new Ektron.Cms.API.User.User();

        board_data = brContent.GetTopicbyID(m_iID.ToString());
        m_BoardId = board_data.Id;
        topic_data = board_data.Forums[0].Topics[0];
        security_data = m_refContentApi.LoadPermissions(m_iID, "content", 0);
        if (this.m_iFolderID == 0)
        {
            this.m_iFolderID = topic_data.FolderId;
        }
        if (!(security_data.IsAdmin || security_data.CanAddToImageLib))
        {
            tr_priority.Visible = false;
            hdn_prior.Value = topic_data.Priority.GetHashCode().ToString();
        }
        base.SetTitleBarToMessage("view topic msg");

		if (closeOnFinish != true)
		{
			base.AddBackButton((string)("../content.aspx?action=ViewContentByCategory&id=" + m_iFolderID.ToString() + "&ContType=" + Ektron.Cms.Common.EkConstants.CMSContentType_BlogComments + "&LangType=" + m_refContentApi.ContentLanguage + "&contentid=" + m_iID.ToString()));
		}

        if (security_data.CanEdit)
        {
            base.AddButtonwithMessages(m_refContentApi.AppPath + "images/UI/Icons/contentEdit.png", (string)("addedittopic.aspx?id=" + m_iID + "&action=edit&LangType=" + m_refContentApi.ContentLanguage), "alt edit properties button text", "btn edit topic properties", "", StyleHelper.EditButtonCssClass, true);
        }
        
        base.AddHelpButton("EditTopic");

        tr_createddate.Visible = true;
        tr_createdby.Visible = true;

        ltr_title.Text = base.GetMessage("generic subject label");
        ltr_summ.Text = base.GetMessage("Summary text");
        ltr_created.Text = base.GetMessage("content dc label");
        ltr_created_by.Text = base.GetMessage("created by label");
        ltr_priority.Text = base.GetMessage("lbl priority") + ":";
        chk_adt_lock.Checked = topic_data.LockTopic;
        rb_normal.Text = base.GetMessage("lbl normal");
        rb_sticky.Text = base.GetMessage("lbl sticky");
        rb_announcement.Text = base.GetMessage("lbl announce");
        ltr_adt_lock.Text = base.GetMessage("lbl lock");
        switch (topic_data.Priority)
        {
            case EkEnumeration.DiscussionObjPriority.Announcement:
                rb_announcement.Checked = true;
                break;
            case EkEnumeration.DiscussionObjPriority.Normal:
                rb_normal.Checked = true;
                break;
            case EkEnumeration.DiscussionObjPriority.Sticky:
                rb_sticky.Checked = true;
                break;
        }
        sTitle = topic_data.Title;
        sTitle = sTitle.Replace("&lt;", "<");
        sTitle = sTitle.Replace("&gt;", ">");
        sTitle = sTitle.Replace("&quot;", "\"");
        sTitle = sTitle.Replace("&#39;", "\'");
        txt_topic_title.Text = sTitle;
        txt_summ.Text = topic_data.Teaser;
        ltr_created_data.Text = topic_data.DateCreated.ToLongDateString() + " " + topic_data.DateCreated.ToShortTimeString();
        ltr_created_by_data.Text = userAPI.GetUser(topic_data.UserId, false, true).Username;
        RenderJS("viewtopic");
        ViewAssignedTaxonomy();
        Disable_Feilds();
    }

    private void Display_Add()
    {
        security_data = m_refContentApi.LoadPermissions(m_iID, "folder", 0);
        if (security_data.CanAdd == false)
        {
            throw (new Exception("User does not have permission"));
        }
        if (this.m_iFolderID == 0)
        {
            this.m_iFolderID = m_iID;
            m_iID = 0;
        }
        if (!(security_data.IsAdmin || security_data.CanAddToImageLib)) // you cannot set sticky/announcement unless you are a moderator.
        {
            tr_priority.Visible = false;
        }
        base.SetTitleBarToMessage("add topic msg");
		
		if (closeOnFinish != true)
		{
			base.AddBackButton((string)("../content.aspx?action=ViewContentByCategory&id=" + m_iFolderID.ToString()));
		}
		
		base.AddButtonwithMessages(m_refContentApi.AppPath + "images/UI/Icons/save.png", "#", "alt save button text (content)", "btn save", "OnClick=\"javascript:SubmitForm();return false;\"", StyleHelper.SaveButtonCssClass, true);
        
        base.AddHelpButton("AddTopic");

        ltr_title.Text = base.GetMessage("generic subject label");
        ltr_desc.Text = base.GetMessage("lbl desc") + ":";
        ltr_summ.Text = base.GetMessage("Summary text");
        ltr_priority.Text = base.GetMessage("lbl priority") + ":";
        rb_normal.Text = base.GetMessage("lbl normal");
        rb_sticky.Text = base.GetMessage("lbl sticky");
        rb_announcement.Text = base.GetMessage("lbl announce");
        ltr_adt_lock.Text = base.GetMessage("lbl lock");
        rb_normal.Checked = true;
        tr_desc.Visible = true;

        DiscussionBoard board_data = null;
        Ektron.Cms.Content.EkContent m_refContent = m_refContentApi.EkContentRef;
        board_data = m_refContent.GetForumbyID(m_iFolderID.ToString());
        m_BoardId = board_data.Id;
        ctlEditor.FolderId = m_iFolderID;
        ctlEditor.SetPermissions(security_data);
        ctlEditor.AllowFonts = true;
        if (board_data != null && board_data.StyleSheet.Length > 0)
        {
            ctlEditor.Stylesheet = m_refContentApi.SitePath + board_data.StyleSheet;
        }
        ctlEditor.Visible = true;
        pnl_message_editor.Controls.Add(ctlEditor);
        //build onsubmit JS

        StringBuilder sbJS = new StringBuilder();

        sbJS.Append("    var objtitle = document.getElementById(\"txt_topic_title\");" + Environment.NewLine);
        sbJS.Append("    if (Trim(objtitle.value).length > 0)" + Environment.NewLine);
        sbJS.Append("    {" + Environment.NewLine);
        sbJS.Append("        return true; " + Environment.NewLine);
        sbJS.Append("    }" + Environment.NewLine);
        sbJS.Append("    else" + Environment.NewLine);
        sbJS.Append("    {" + Environment.NewLine);
        sbJS.Append("        alert(\"" + base.GetMessage("null topic warning msg") + "\");" + Environment.NewLine);
        sbJS.Append("        try {" + Environment.NewLine);
        sbJS.Append("            objtitle.focus();" + Environment.NewLine);
        sbJS.Append("        }" + Environment.NewLine);
        sbJS.Append("        catch (e) { }" + Environment.NewLine);
        sbJS.Append("        return false;" + Environment.NewLine);
        sbJS.Append("    }" + Environment.NewLine);

        PreSelectTaxonomies(0);
        DisplayTaxonomy();

        RenderJS("add");
        SuppressTitle();

    }

    public void Display_Edit()
    {
        DiscussionTopic topic_data;
        DiscussionBoard board_data = null;
        string sTitle = "";
        Ektron.Cms.Content.EkContent brContent = m_refContentApi.EkContentRef;
        Ektron.Cms.API.User.User userAPI = new Ektron.Cms.API.User.User();

        board_data = brContent.GetTopicbyIDForEdit(m_iID.ToString());
        m_BoardId = board_data.Id;
        topic_data = board_data.Forums[0].Topics[0];
        security_data = m_refContentApi.LoadPermissions(m_iID, "content", 0);
        if (this.m_iFolderID == 0)
        {
            this.m_iFolderID = topic_data.FolderId;
        }
        if (!(security_data.IsAdmin || security_data.CanAddToImageLib)) // you cannot set sticky/announcement unless you are a moderator.
        {
            tr_priority.Visible = false;
            hdn_prior.Value = topic_data.Priority.GetHashCode().ToString();
        }
        base.SetTitleBarToMessage("edit topic msg");

		if (closeOnFinish != true)
		{
			base.AddBackButton((string)("../content.aspx?action=ViewContentByCategory&id=" + m_iFolderID.ToString() + "&ContType=" + Ektron.Cms.Common.EkConstants.CMSContentType_BlogComments + "&LangType=" + m_refContentApi.ContentLanguage + "&contentid=" + m_iID.ToString()));
		} 
		
		if (security_data.CanEdit)
        {
            base.AddButtonwithMessages(m_refContentApi.AppPath + "images/UI/Icons/save.png", "#", "alt save button text (content)", "btn save", "OnClick=\"javascript:SubmitForm();return false;\"", StyleHelper.SaveButtonCssClass, true);
        }
        
        base.AddHelpButton("EditTopic");

        // MyBase.Tabs.AddTabByMessage("Summary text", "dvSumm")
        // MyBase.Tabs.AddTabByMessage("properties text", "dvProp")

        tr_createddate.Visible = true;
        tr_createdby.Visible = true;

        ltr_title.Text = base.GetMessage("generic subject label");
        //ltr_desc.Text = MyBase.GetMessage("lbl desc") & ":"
        ltr_summ.Text = base.GetMessage("Summary text");
        ltr_created.Text = base.GetMessage("content dc label");
        ltr_created_by.Text = base.GetMessage("created by label");
        ltr_priority.Text = base.GetMessage("lbl priority") + ":";
        chk_adt_lock.Checked = topic_data.LockTopic;
        rb_normal.Text = base.GetMessage("lbl normal");
        rb_sticky.Text = base.GetMessage("lbl sticky");
        rb_announcement.Text = base.GetMessage("lbl announce");
        ltr_adt_lock.Text = base.GetMessage("lbl lock");
        switch (topic_data.Priority)
        {
            case EkEnumeration.DiscussionObjPriority.Announcement:
                rb_announcement.Checked = true;
                break;
            case EkEnumeration.DiscussionObjPriority.Normal:
                rb_normal.Checked = true;
                break;
            case EkEnumeration.DiscussionObjPriority.Sticky:
                rb_sticky.Checked = true;
                break;
        }
        sTitle = topic_data.Title;
        sTitle = sTitle.Replace("&lt;", "<");
        sTitle = sTitle.Replace("&gt;", ">");
        sTitle = sTitle.Replace("&quot;", "\"");
        sTitle = sTitle.Replace("&#39;", "\'");
        txt_topic_title.Text = sTitle;
        txt_summ.Text = topic_data.Teaser;
        ltr_created_data.Text = topic_data.DateCreated.ToLongDateString() + " " + topic_data.DateCreated.ToShortTimeString();

        ltr_created_by_data.Text = userAPI.GetUser(topic_data.UserId, false, true).Username;

        //If security_data.CanEdit Then
        //With ctlEditor
        //    .ShowCMSToolbar = False
        //    .ToolbarLevel = "Reduced"
        //    .ID = "content_html"
        //    .Width = New Unit(570, UnitType.Pixel)
        //    .Height = New Unit(200, UnitType.Pixel)
        //    .Text = content_data.Html
        //End With
        //pnl_message_editor.Controls.Add(ctlEditor)
        //    'pnl_message_editor.Controls.Add(New LiteralControl(MyBase.eWebWPEditor("content_html", 550, 400, content_data.Html)))
        //    SuppressTitle()

        PreSelectTaxonomies(m_iID);
        DisplayTaxonomy();

        RenderJS("");
        //Else
        //    txt_topic_title.Enabled = False
        //    pnl_message_editor.Controls.Add(New LiteralControl(content_data.Html))
        //    txt_summ.Enabled = False
        //End If

        PreSelectTaxonomies(m_iID);
        DisplayTaxonomy();

    }

    private void DisplayTaxonomy()
    {

        if (_selectedTaxonomyList.Count > 0)
        {

            for (int i = 0; i <= (_selectedTaxonomyList.Count - 1); i++)
            {

                if (taxonomyselectedtree.Value.Length > 0)
                {
                    taxonomyselectedtree.Value += "," + _selectedTaxonomyList[i];
                }
                else
                {
                    taxonomyselectedtree.Value += "" + _selectedTaxonomyList[i];
                }

            }

        }

        _TaxonomyTreeIdList = (string)taxonomyselectedtree.Value;

        _TaxonomyTreeParentIdList = m_refContentApi.EkContentRef.ReadDisableNodeList(m_iID);

    }

    protected void PreSelectTaxonomies(long topicId)
    {

        string json = "";

        if (topicId > 0)
        {

            m_refContentApi.RequestInformationRef.ContentLanguage = ContentLanguage;
            m_refContentApi.ContentLanguage = ContentLanguage;

            TaxonomyBaseData[] taxonomy_cat_arr = m_refContentApi.EkContentRef.ReadAllAssignedCategory(m_iID);

            for (int i = 0; i <= (taxonomy_cat_arr.Length - 1); i++)
            {

                _selectedTaxonomyList.Add(taxonomy_cat_arr[i].TaxonomyId);
                if (json.Length > 0)
                {
                    json += (string)("," + GetTaxonomyJson(taxonomy_cat_arr[i].TaxonomyId));
                }
                else
                {
                    json += GetTaxonomyJson(taxonomy_cat_arr[i].TaxonomyId);
                }

            }
        }
        data.Value = "[" + json + "]";
    }
    #endregion
    #region Process
    private void Process_Add()
    {
        try
        {
            Collection page_content_data = new Collection();
            DiscussionBoard board_data = null;
            Ektron.Cms.Content.EkContent m_refContent = m_refContentApi.EkContentRef;
            Ektron.Cms.Content.EkTask ekmessage = m_refContentApi.EkTaskRef;
            string strContent = "";
            string strSearchText = "";
            string strContentTeaser = "";
            bool isapproved = false;

            board_data = m_refContent.GetForumbyID(m_iFolderID.ToString());
            security_data = m_refContentApi.LoadPermissions(m_iFolderID, "folder", 0);
            if ((board_data.Forums[0].ModerateComments == false) || (board_data.Forums[0].ModerateComments == true && (security_data.IsAdmin || security_data.CanAddToImageLib)))
            {
                isapproved = true;
            }
            if (!(security_data.IsAdmin || security_data.CanAddToImageLib)) // you cannot set sticky/announcement unless you are a moderator.
            {

            }
            this.m_iFolderID = Convert.ToInt64(Request.Form[hdn_folderid.UniqueID]);
            strContentTeaser = Request.Form[txt_summ.UniqueID];

            strContent = (string)ctlEditor.Content;

            strSearchText = Utilities.StripHTML((string)ctlEditor.Content);
            if (!(Request.Form["content_html_Action"] == null))
            {
                strContent = Server.HtmlDecode(strContent);
            }
            strContent = this.m_refContentApi.ReplaceWordsForBoardPosts(strContent, board_data.Id);
            page_content_data.Add(EkEnumeration.CMSContentType.DiscussionTopic, "ContentType", null, null);
            page_content_data.Add("", "ContentHtml", null, null);
            page_content_data.Add("", "Comment", null, null);
            page_content_data.Add(0, "ContentID", null, null);
            page_content_data.Add(Request.Form["content_language"], "ContentLanguage", null, null);
            page_content_data.Add(strContentTeaser, "ContentTeaser", null, null);
            page_content_data.Add(this.m_iFolderID, "FolderID", null, null);
            page_content_data.Add(strSearchText, "SearchText", null, null);
            page_content_data.Add("", "GoLive", null, null);
            page_content_data.Add("", "EndDate", null, null);
            page_content_data.Add("", "EndDateAction", null, null);
            page_content_data.Add(true, "IsDiscussionTopic", null, null);
            if (isapproved == false)
            {
                page_content_data.Add(false, "TopicApproved", null, null);
            }
            if (rb_normal.Checked == true)
            {
                page_content_data.Add(EkEnumeration.DiscussionObjPriority.Normal, "Priority", null, null);
            }
            else if (rb_sticky.Checked == true)
            {
                page_content_data.Add(EkEnumeration.DiscussionObjPriority.Sticky, "Priority", null, null);
            }
            else if (rb_announcement.Checked == true)
            {
                page_content_data.Add(EkEnumeration.DiscussionObjPriority.Announcement, "Priority", null, null);
            }
            if (!string.IsNullOrEmpty(Request.Form[chk_adt_lock.UniqueID]))
            {
                page_content_data.Add(true, "LockTopic", null, null);
            }
            else
            {
                page_content_data.Add(false, "LockTopic", null, null);
            }
            string topicTitle = this.m_refContentApi.ReplaceWordsForBoardPosts(Request.Form[txt_topic_title.UniqueID].ToString(), board_data.Id);
            page_content_data.Add(topicTitle, "ContentTitle", null, null);
            page_content_data.Add(false, "AddToQlink", null, null);
            page_content_data.Add(true, "IsSearchable", null, null);
            m_iID = m_refContent.AddNewContentv2_0(page_content_data);

            if (m_refContentApi.RequestInformationRef.ContentLanguage > 0)
            {
                ekmessage.ContentLanguage = m_refContentApi.RequestInformationRef.ContentLanguage;
                ekmessage.LanguageID = m_refContentApi.RequestInformationRef.ContentLanguage;
            }
            else
            {
                ekmessage.ContentLanguage = m_refContentApi.RequestInformationRef.DefaultContentLanguage;
                ekmessage.LanguageID = m_refContentApi.RequestInformationRef.DefaultContentLanguage;
            }
            ekmessage.ContentID = m_iID;
            ekmessage.AssignedByUserID = m_refContentApi.RequestInformationRef.UserId.ToString();
            ekmessage.CreatedByUserID = m_refContentApi.RequestInformationRef.UserId;
            ekmessage.DateCreated = (string)(DateTime.Now.ToLongDateString() + " " + DateTime.Now.ToLongTimeString());
            ekmessage.TaskTypeID = EkEnumeration.TaskType.TopicReply.GetHashCode();
            ekmessage.State = (string)(EkEnumeration.TaskState.Completed.GetHashCode().ToString());
            ekmessage.Description = strContent;
            ekmessage.TaskTitle = "TopicReply";
            ekmessage.ImpersonateUser = true;

            ekmessage.CommentDisplayName = ""; //m_refContentApi.RequestInformationRef.LoggedInUsername
            ekmessage.ParentID = -2;
            ekmessage.CommentEmail = "";
            ekmessage.CommentURI = Request.ServerVariables["REMOTE_ADDR"];
            ekmessage.HostURL = Request.ServerVariables["HTTP_HOST"];
            ekmessage.URLpath = this.m_refContentApi.SitePath + board_data.TemplateFileName;
            ekmessage.AddTask();

            Process_Taxonomy(m_iID);

            ContentData content_data = m_refContentApi.GetContentById(m_iID, ContentAPI.ContentResultType.Published);

            LibraryData lib_item = new LibraryData();
            lib_item.ContentId = m_iID;
            lib_item.DateCreated = content_data.DateCreated;
            lib_item.ContentType = Convert.ToInt32(EkEnumeration.CMSContentType.DiscussionTopic);
            lib_item.DisplayDateCreated = content_data.DisplayDateCreated;
            lib_item.DisplayLastEditDate = content_data.DisplayLastEditDate;
            lib_item.EditorFirstName = content_data.EditorFirstName;
            lib_item.EditorLastName = content_data.EditorLastName;
            lib_item.FileName = board_data.TemplateFileName + ((content_data.TemplateConfiguration.FileName.IndexOf("?") > -1) ? "&" : "?") + "g=posts&t=" + this.m_iID + "&boardid=" + board_data.Id.ToString();
            lib_item.FolderName = content_data.FolderName;
            lib_item.IsPrivate = content_data.IsPrivate;
            lib_item.LanguageId = content_data.LanguageId;
            lib_item.LastEditDate = content_data.LastEditDate;
            lib_item.MetaData = content_data.MetaData;
            lib_item.ParentId = content_data.FolderId;
            lib_item.Title = content_data.Title;
            lib_item.TypeId = 4;
            lib_item.Type = "quicklinks";
            lib_item.UserId = content_data.UserId;

            // Dim CallerID As Integer = m_refContentApi.RequestInformationRef.CallerId
            // m_refContentApi.RequestInformationRef.CallerId = Convert.ToInt32(EkConstants.InternalAdmin)
            this.m_refContentApi.AddLibraryItem(ref lib_item);
            // m_refContentApi.RequestInformationRef.CallerId = CallerID

            if (closeOnFinish == true)
            {
                Response.Redirect("../close.aspx", false);
            }
            else
            {
                Response.Redirect((string)("../content.aspx?action=ViewContentByCategory&id=" + m_iFolderID.ToString()), false);
            }
        }
        catch (Exception ex)
        {
            Utilities.ShowError(ex.Message);
        }
    }

    private void Process_Edit()
    {
        try
        {
            Collection page_content_data = new Collection();
            Ektron.Cms.Content.EkContent m_refContent = m_refContentApi.EkContentRef;
            string strContent = "";
            string strSearchText = "";
            string strContentTeaser = "";

            this.m_iFolderID = Convert.ToInt64(Request.Form[hdn_folderid.UniqueID]);
            this.m_iID = Convert.ToInt64(Request.Form[hdn_topicid.UniqueID]);
            strContentTeaser = Request.Form[txt_summ.UniqueID];
            strContent = (string)ctlEditor.Content;
            if (!(Request.Form["content_html_Action"] == null))
            {
                strContent = Server.HtmlDecode(strContent);
            }
            strSearchText = "";

            page_content_data.Add(EkEnumeration.CMSContentType.DiscussionTopic, "ContentType", null, null);
            page_content_data.Add(strContent, "ContentHtml", null, null);
            page_content_data.Add("", "Comment", null, null);
            page_content_data.Add(this.m_iID, "ContentID", null, null);
            page_content_data.Add(strContentTeaser, "ContentTeaser", null, null);
            page_content_data.Add(this.m_iFolderID, "FolderID", null, null);
            page_content_data.Add(strSearchText, "SearchText", null, null);
            page_content_data.Add(m_refContentApi.ContentLanguage, "ContentLanguage", null, null);
            page_content_data.Add("", "GoLive", null, null);
            page_content_data.Add("", "EndDate", null, null);
            page_content_data.Add("", "EndDateAction", null, null);
            page_content_data.Add(true, "IsDiscussionTopic", null, null);
            page_content_data.Add("", "Image", null, null);
            if (Convert.ToInt32(hdn_prior.Value) > 0)
            {
                page_content_data.Add(hdn_prior.Value, "Priority", null, null);
            }
            else
            {
                if (rb_normal.Checked == true)
                {
                    page_content_data.Add(EkEnumeration.DiscussionObjPriority.Normal, "Priority", null, null);
                }
                else if (rb_sticky.Checked == true)
                {
                    page_content_data.Add(EkEnumeration.DiscussionObjPriority.Sticky, "Priority", null, null);
                }
                else if (rb_announcement.Checked == true)
                {
                    page_content_data.Add(EkEnumeration.DiscussionObjPriority.Announcement, "Priority", null, null);
                }
            }
            if (!string.IsNullOrEmpty(Request.Form[chk_adt_lock.UniqueID]))
            {
                page_content_data.Add(true, "LockTopic", null, null);
            }
            else
            {
                page_content_data.Add(false, "LockTopic", null, null);
            }
            page_content_data.Add(Request.Form[txt_topic_title.UniqueID], "ContentTitle", null, null);
            page_content_data.Add(false, "AddToQlink", null, null);
            page_content_data.Add(false, "IsSearchable", null, null);
            m_refContent.GetContentForEditing(m_iID);
            m_refContent.SaveContentv2_0(page_content_data);
            m_refContent.CheckIn(m_iID, "");
            m_refContent.SubmitForPublicationv2_0(m_iID, this.m_iFolderID, "");

            Process_Taxonomy(m_iID);

            if (closeOnFinish == true)
            {
                Response.Redirect("../close.aspx", false);
            }
            else
            {
                Response.Redirect((string)("../content.aspx?action=ViewContentByCategory&id=" + m_iFolderID.ToString() + "&ContType=" + Ektron.Cms.Common.EkConstants.CMSContentType_BlogComments + "&LangType=" + m_refContentApi.ContentLanguage + "&contentid=" + m_iID.ToString()), false);
            }
        }
        catch (Exception ex)
        {
            Utilities.ShowError(ex.Message + ex.StackTrace);
        }
    }

    private void Process_Approve()
    {
        try
        {
            Collection page_content_data = new Collection();
            Ektron.Cms.Content.EkContent m_refContent = m_refContentApi.EkContentRef;
            security_data = m_refContentApi.LoadPermissions(m_iID, "folder", 0);
            if (security_data.IsAdmin == true || (security_data.CanAddToImageLib == true))
            {
                m_refContent.ApproveTopic(m_iID);

                if (closeOnFinish == true)
                {
                    Response.Redirect("../close.aspx", false);
                }
                else
                {
                    Response.Redirect((string)("../content.aspx?action=ViewContentByCategory&id=" + m_iFolderID.ToString() + "&ContType=" + Ektron.Cms.Common.EkConstants.CMSContentType_BlogComments + "&LangType=" + m_refContentApi.ContentLanguage + "&contentid=" + m_iID.ToString()), false);
                }
            }
            else
            {
                throw (new Exception(base.GetMessage("com: user does not have permission")));
            }
        }
        catch (Exception ex)
        {
            Utilities.ShowError(ex.Message + ex.StackTrace);
        }
    }

    public void Process_Taxonomy(long topicId)
    {
        if (TaxonomyOverrideId > 0)
        {
            _TaxonomyTreeIdList = TaxonomyOverrideId.ToString();
        }
        if ((Request.Form[taxonomyselectedtree.UniqueID] != null) && Request.Form[taxonomyselectedtree.UniqueID] != "")
        {
            _TaxonomyTreeIdList = Request.Form[taxonomyselectedtree.UniqueID];
            if (_TaxonomyTreeIdList.Trim().EndsWith(","))
            {
                _TaxonomyTreeIdList = _TaxonomyTreeIdList.Substring(0, _TaxonomyTreeIdList.Length - 1);
            }
        }
        TaxonomyContentRequest topic_request = new TaxonomyContentRequest();
        topic_request.ContentId = topicId;
        topic_request.TaxonomyList = _TaxonomyTreeIdList;
        m_refContentApi.AddTaxonomyItem(topic_request);
    }
    #endregion

    #region Private Functions

    protected string GetTaxonomyJson(long id)
    {
        return "{\"Id\":\" & id.ToString()" + "\",\"Name\":\"" + id.ToString() + "11\"" + ",\"Path\":\"" + id.ToString() + "\"" + ",\"Type\":\"Taxonomy\",\"SubType\":\"\",\"TypeCode\":\"2\",\"MarkedForDelete\":\"false\",\"NewlyAdded\":\"false\"}";
    }

    private void SuppressTitle()
    {
        ltr_js.Text += "<style type=\"text/css\">" + Environment.NewLine + "#content_html_ContentInfo" + Environment.NewLine + "{" + Environment.NewLine + "    display:none;" + Environment.NewLine + "}" + Environment.NewLine + "</style>";
    }

    private void RenderJS(string actiontype)
    {
        StringBuilder sbJS = new StringBuilder();
        sbJS.Append("<script language=\"javascript\" type=\"text/javascript\" >" + Environment.NewLine);
        sbJS.Append("function SubmitForm()" + Environment.NewLine);
        sbJS.Append("{" + Environment.NewLine);
        sbJS.Append("    var objtitle = document.getElementById(\"txt_topic_title\");" + Environment.NewLine);
        sbJS.Append("    if (Trim(objtitle.value).length > 0)" + Environment.NewLine);
        sbJS.Append("    {" + Environment.NewLine);
        sbJS.Append("           document.forms[0].submit();" + Environment.NewLine);
        sbJS.Append("    }" + Environment.NewLine);
        sbJS.Append("    else" + Environment.NewLine);
        sbJS.Append("    {" + Environment.NewLine);
        sbJS.Append("        alert(\"" + base.GetMessage("null topic warning msg") + "\");" + Environment.NewLine);
        sbJS.Append("        try {" + Environment.NewLine);
        sbJS.Append("            objtitle.focus();" + Environment.NewLine);
        sbJS.Append("        }" + Environment.NewLine);
        sbJS.Append("        catch (e) { }" + Environment.NewLine);
        sbJS.Append("        return false;" + Environment.NewLine);
        sbJS.Append("    }" + Environment.NewLine);
        sbJS.Append("}" + Environment.NewLine);
        sbJS.Append("</script>" + Environment.NewLine);
        ltr_js.Text += Environment.NewLine + sbJS.ToString();

        Ektron.Cms.API.Css.RegisterCss(this, Ektron.Cms.API.Css.ManagedStyleSheet.EktronWorkareaCss);
        Ektron.Cms.API.Css.RegisterCss(this, Ektron.Cms.API.Css.ManagedStyleSheet.EktronWorkareaIeCss, Ektron.Cms.API.Css.BrowserTarget.LessThanEqualToIE7);
        Ektron.Cms.API.Css.RegisterCss(this, Ektron.Cms.API.Css.ManagedStyleSheet.EktronFixedPositionToolbarCss);
        Ektron.Cms.API.JS.RegisterJS(this, Ektron.Cms.API.JS.ManagedScript.EktronJS);
        Ektron.Cms.API.JS.RegisterJS(this, Ektron.Cms.API.JS.ManagedScript.EktronUICoreJS);
        Ektron.Cms.API.JS.RegisterJS(this, Ektron.Cms.API.JS.ManagedScript.EktronUITabsJS);
        Ektron.Cms.API.JS.RegisterJS(this, Ektron.Cms.API.JS.ManagedScript.EktronJsonJS);
        Ektron.Cms.API.JS.RegisterJS(this, Ektron.Cms.API.JS.ManagedScript.EktronModalJS);
        if (actiontype != "viewtopic")
        {
            Ektron.Cms.API.Css.RegisterCss(this, this.m_refContentApi.ApplicationPath + "/Tree/css/com.Ektron.ui.tree.css", "EktronTreeCss");
            Ektron.Cms.API.JS.RegisterJS(this, this.m_refContentApi.ApplicationPath + "/Commerce/Coupons/SharedComponents/Scope/ItemsSelection/js/selectTaxonomy.js", "EktronCommerceCouponselectTaxonomyJs");
            Ektron.Cms.API.JS.RegisterJS(this, this.m_refContentApi.ApplicationPath + "/Commerce/CatalogEntry/js/CatalogEntry.PageFunctions.aspx?id=" + "default" + "&entrytype=" + Ektron.Cms.Common.EkEnumeration.CatalogEntryType.Product + "&folder_id=" + m_BoardId, "Ektron_CatalogEntry_PageFunctions_Js");
            Ektron.Cms.API.JS.RegisterJS(this, this.m_refContentApi.ApplicationPath + "/Commerce/CatalogEntry/js/CatalogEntry.Taxonomy.A.aspx?folderId=" + m_BoardId.ToString() + "&taxonomyOverrideId=" + TaxonomyOverrideId.ToString() + "&taxonomyTreeIdList=" + EkFunctions.UrlEncode(_TaxonomyTreeIdList) + "&taxonomyTreeParentIdList=" + Server.UrlEncode(_TaxonomyTreeParentIdList), "Ektron_CatalogEntry_Taxonomy_A_Js");
            Ektron.Cms.API.JS.RegisterJS(this, this.m_refContentApi.ApplicationPath + "/Commerce/CatalogEntry/js/CatalogEntry.Taxonomy.B.aspx?suppress_menu=true&showTaxonomy=" + _TaxonomyRoleExists.ToString() + "&taxonomyFolderId=" + m_BoardId.ToString(), "Ektron_CatalogEntry_Taxonomy_B_Js");
            Ektron.Cms.API.JS.RegisterJS(this, this.m_refContentApi.ApplicationPath + "/Tree/js/com.Ektron.utils.url.js", "EktronTreeUtilsUrlJs");
            Ektron.Cms.API.JS.RegisterJS(this, this.m_refContentApi.ApplicationPath + "/Tree/js/com.Ektron.explorer.init.js", "EktronTreeExplorerInitJs");
            Ektron.Cms.API.JS.RegisterJS(this, this.m_refContentApi.ApplicationPath + "/Tree/js/com.Ektron.explorer.js", "EktronTreeExplorerJs");
            Ektron.Cms.API.JS.RegisterJS(this, this.m_refContentApi.ApplicationPath + "/Tree/js/com.Ektron.explorer.config.js", "EktronTreeExplorerConfigJs");
            Ektron.Cms.API.JS.RegisterJS(this, this.m_refContentApi.ApplicationPath + "/Tree/js/com.Ektron.explorer.windows.js", "EktronTreeExplorerWindowsJs");
            Ektron.Cms.API.JS.RegisterJS(this, this.m_refContentApi.ApplicationPath + "/Tree/js/com.Ektron.cms.types.js", "EktronTreeCmsTypesJs");
            Ektron.Cms.API.JS.RegisterJS(this, this.m_refContentApi.ApplicationPath + "/Tree/js/com.Ektron.cms.parser.js", "EktronTreeCmsParserJs");
            Ektron.Cms.API.JS.RegisterJS(this, this.m_refContentApi.ApplicationPath + "/Tree/js/com.Ektron.cms.toolkit.js", "EktronTreeCmsToolkitJs");
            Ektron.Cms.API.JS.RegisterJS(this, this.m_refContentApi.ApplicationPath + "/Tree/js/com.Ektron.cms.api.js", "EktronTreeCmsApiJs");
            Ektron.Cms.API.JS.RegisterJS(this, this.m_refContentApi.ApplicationPath + "/Tree/js/com.Ektron.ui.contextmenu.js", "EktronTreeUiContextMenuJs");
            Ektron.Cms.API.JS.RegisterJS(this, this.m_refContentApi.ApplicationPath + "/Tree/js/com.Ektron.ui.iconlist.js", "EktronTreeUiIconListJs");
            Ektron.Cms.API.JS.RegisterJS(this, this.m_refContentApi.ApplicationPath + "/Tree/js/com.Ektron.ui.tabs.js", "EktronTreeUiTabsJs");
            Ektron.Cms.API.JS.RegisterJS(this, this.m_refContentApi.ApplicationPath + "/Tree/js/com.Ektron.ui.explore.js", "EktronTreeUiExploreJs");
            Ektron.Cms.API.JS.RegisterJS(this, this.m_refContentApi.ApplicationPath + "/Tree/js/com.Ektron.ui.taxonomytree.js", "EktronTreeUiTaxonomyTreeJs");
            Ektron.Cms.API.JS.RegisterJS(this, this.m_refContentApi.ApplicationPath + "/Tree/js/com.Ektron.net.http.js", "EktronTreeNetHttpJs");
            Ektron.Cms.API.JS.RegisterJS(this, this.m_refContentApi.ApplicationPath + "/Tree/js/com.Ektron.lang.exception.js", "EktronTreeLanguageExceptionJs");
            Ektron.Cms.API.JS.RegisterJS(this, this.m_refContentApi.ApplicationPath + "/Tree/js/com.Ektron.utils.form.js", "EktronTreeUtilsFormJs");
            Ektron.Cms.API.JS.RegisterJS(this, this.m_refContentApi.ApplicationPath + "/Tree/js/com.Ektron.utils.log.js", "EktronTreeUtilsLogJs");
            Ektron.Cms.API.JS.RegisterJS(this, this.m_refContentApi.ApplicationPath + "/Tree/js/com.Ektron.utils.dom.js", "EktronTreeUtilsDomJs");
            Ektron.Cms.API.JS.RegisterJS(this, this.m_refContentApi.ApplicationPath + "/Tree/js/com.Ektron.utils.debug.js", "EktronTreeUtilsDebugJs");
            Ektron.Cms.API.JS.RegisterJS(this, this.m_refContentApi.ApplicationPath + "/Tree/js/com.Ektron.utils.string.js", "EktronTreeUtilsStringJs");
            Ektron.Cms.API.JS.RegisterJS(this, this.m_refContentApi.ApplicationPath + "/Tree/js/com.Ektron.utils.cookie.js", "EktronTreeUtilsCookieJs");
            Ektron.Cms.API.JS.RegisterJS(this, this.m_refContentApi.ApplicationPath + "/Tree/js/com.Ektron.utils.querystring.js", "EktronTreeUtilsQuerystringJs");
            Ektron.Cms.API.JS.RegisterJS(this, this.m_refContentApi.ApplicationPath + "/Commerce/CatalogEntry/js/CatalogEntry.Taxonomy.C.js", "Ektron_CatalogEntry_Taxonomy_C_Js");
        }
    }

    private void Disable_Feilds()
    {
        txt_topic_title.Enabled = false;
        rb_announcement.Enabled = false;
        rb_sticky.Enabled = false;
        rb_normal.Enabled = false;
        chk_adt_lock.Enabled = false;
        txt_summ.Enabled = false;
    }

    private void ViewAssignedTaxonomy()
    {
        Ektron.Cms.Content.EkContent cref;
        cref = m_refContentApi.EkContentRef;
        TaxonomyBaseData[] taxonomy_cat_arr = null;
        List<string> result = new List<string>();
        taxonomy_cat_arr = cref.ReadAllAssignedCategory(m_iID);
        if ((taxonomy_cat_arr != null) && taxonomy_cat_arr.Length > 0)
        {

            result.Add("<ul class=\"assignedTaxonomyList\">");
            foreach (TaxonomyBaseData taxonomy_cat in taxonomy_cat_arr)
            {
                result.Add(("<li>" + taxonomy_cat.TaxonomyPath.Remove(0, 1).Replace("\\", " > ") + "</li>"));
            }
            result.Add("</ul>");
            EditTaxonomyHtml.Text = string.Join(string.Empty, result.ToArray());
        }
        else
        {
            EditTaxonomyHtml.Text = m_refMsg.GetMessage("lbl nocatselected");
        }
    }
    #endregion
}
