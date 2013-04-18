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
using Ektron.Cms.Workarea;
using Ektron.Cms.ToDo;
using Ektron.Cms.Framework.User;

public partial class communitygroupaddedit : workareabase, ICallbackEventHandler
{
    protected CommunityGroupData cgGroup = new Ektron.Cms.CommunityGroupData();
    protected bool bAccess = false;
    protected string TaxonomyTreeIdList = "";
    protected string TaxonomyTreeParentIdList = "";
    protected string updateFieldId = "";
    protected string commparams = "";
    protected bool TaxonomyRoleExists = false;
    protected long m_intTaxFolderId = 0;
    protected long TaxonomyOverrideId = 0;
    protected bool bThickBox = false;
    protected LanguageData[] language_data = null;
    protected SiteAPI m_refSiteApi = new SiteAPI();
    protected long TaxonomyId = 0;
    protected long profileTaxonomyId = 0;
    protected int TaxonomyLanguage = 1033;
    protected string AppPath = "";
    protected string m_callbackresult = "";
    protected string m_strKeyWords = "";
    protected string m_strSearchText = "";
    protected string m_searchMode = "display_name";
    protected int m_recipientsPage = 1;
    protected int m_intTotalPages = 0;
    protected bool m_friendsOnly = false;
    protected long m_userId = 0;
    protected UserAPI m_refUserApi = null;
    protected UserData[] m_user_list;
    protected string m_uniqueId = "__Page";
    protected bool m_bAdmin = false;
    protected EkEnumeration.GroupMemberStatus m_mMemberStatus = EkEnumeration.GroupMemberStatus.NotInGroup;
    protected bool m_bGroupAdmin = false;
    private Ektron.Cms.API.User.User userList = new Ektron.Cms.API.User.User();
    private Ektron.Cms.Community.MessageBoardAPI _MessageBoardApi = new Ektron.Cms.Community.MessageBoardAPI();
    private Ektron.Cms.Content.Calendar.WebCalendar _CalendarApi;
    private bool groupMessageBoardModerate;
    protected bool bSuppressTaxTab = false;
    Ektron.Cms.Common.Calendar.WebCalendarData calendardata = new Ektron.Cms.Common.Calendar.WebCalendarData();
    protected long _doesForumExists = 0;
    protected string groupAliasList = string.Empty;
    protected MailServerData mailServer = null;
    protected Ektron.Cms.Framework.Activity.Activity _activityApi = new Ektron.Cms.Framework.Activity.Activity();
    protected string[] strAdminUsers;
    protected Ektron.Cms.Community.CommunityGroup commGroup;
    
    /// <summary>
    /// Returns true if there ar emore than one languages enabled for the site.
    /// </summary>
    /// <value></value>
    /// <returns></returns>
    /// <remarks></remarks>
    public bool IsSiteMultilingual
    {
        get
        {
            if (m_refUserApi.EnableMultilingual == 0)
            {
                return false;
            }
            int languageEnabledCount = 0;
            foreach (LanguageData lang in languageDataArray)
            {
                if (lang.SiteEnabled)
                {
                    languageEnabledCount++;
                }
                if (languageEnabledCount > 1)
                {
                    break;
                }
            }

            return languageEnabledCount > 1;
        }

    }

    public LanguageData[] languageDataArray
    {
        get
        {
            if (language_data == null)
            {
                language_data = m_refSiteApi.GetAllActiveLanguages();
            }

            return language_data;
        }

    }

    protected void Page_Init(object sender, System.EventArgs e)
    {
        m_user_list = (Ektron.Cms.UserData[])Array.CreateInstance(typeof(Ektron.Cms.UserData), 0);
        _CalendarApi = new Ektron.Cms.Content.Calendar.WebCalendar(m_refContentApi.RequestInformationRef);
        commGroup = new Ektron.Cms.Community.CommunityGroup(m_refContentApi.RequestInformationRef);
    }

    protected override void Page_Load(object sender, System.EventArgs e)
    {
        base.Page_Load(sender, e);
        Ektron.Cms.API.Css.RegisterCss(this, Ektron.Cms.API.Css.ManagedStyleSheet.EktronJQueryUiDefaultCss);
        FeaturesTodo_CB.Visible = false;

        CommonApi refCommonAPI = new CommonApi();
        if (!Ektron.Cms.DataIO.LicenseManager.LicenseManager.IsFeatureEnable(m_refContentApi.RequestInformationRef, Ektron.Cms.DataIO.LicenseManager.Feature.SocialNetworking))
        {
            Utilities.ShowError(m_refContentApi.EkMsgRef.GetMessage("feature locked error"));
        }
        m_userId = refCommonAPI.RequestInformationRef.UserId;
        AppPath = m_refContentApi.AppPath;
        lblUpload.Text = m_refMsg.GetMessage("upload txt");
        mailServer = GetNotificationEmailServer();
        lblProperties.Text = m_refMsg.GetMessage("generic properties");
        lblTags.Text = m_refMsg.GetMessage("lbl personal tags");
        lblCategory.Text = m_refMsg.GetMessage("lbl category");
        try
        {
            if (!this.IsCallback)
            {
                RegisterResources();

                //cgae_userselect_done_btn.Attributes.Add("onclick", "GetCommunityMsgObject(\'" + m_uniqueId + "\').MsgSaveMessageTargetUI(); return false;");
                cgae_userselect_done_btn.Attributes.Add("onclick", "AddAdminUsers();return false;");
                cgae_userselect_done_btn.Attributes.Add("class", "EktMsgTargetsDoneBtn");
                cgae_userselect_done_btn.Text = m_refMsg.GetMessage("btn done");
                //cgae_userselect_done_btn.Tooltip = m_refMsg.GetMessage("btn done")

                cgae_userselect_cancel_btn.Attributes.Add("onclick", "GetCommunityMsgObject(\'" + m_uniqueId + "\').MsgCancelMessageTargetUI(); return false;");
                cgae_userselect_cancel_btn.Attributes.Add("class", "EktMsgTargetsCancelBtn");
                cgae_userselect_cancel_btn.Text = m_refMsg.GetMessage("btn cancel");
                //cgae_userselect_cancel_btn.Tooltip = m_refMsg.GetMessage("btn cancel")

                Invite_UsrSel.SingleSelection = true;

                CheckAccess();
                if (!bAccess)
                {
                    throw (new Exception(this.m_iID > 0 ? (this.GetMessage("err communityaddedit no access")) : (this.GetMessage("err no perm add cgroup"))));
                }
                this.GroupAvatar_TB.Attributes.Add("onkeypress", "updateavatar();");
                if (!string.IsNullOrEmpty(Request.QueryString["thickbox"]))
                {
                    bThickBox = true;
                    Ektron.Cms.API.Css.RegisterCss(this, this.AppPath + "csslib/ektron.communitygroup.addedit.tb.ui.css", "EktronCommunityGroupAddEditTbUiCSS");
                }
                int langID = m_refContentApi.DefaultContentLanguage;
                if (Request.QueryString["LangType"] == null && Request.QueryString["LangType"] == "")
                {
                    langID = m_refContentApi.ContentLanguage;
                }
                else
                {
                    langID = Convert.ToInt32(Request.QueryString["LangType"]);
                }
                if (langID == Ektron.Cms.Common.EkConstants.CONTENT_LANGUAGES_UNDEFINED || langID == Ektron.Cms.Common.EkConstants.ALL_CONTENT_LANGUAGES)
                {
                    langID = int.Parse(refCommonAPI.GetCookieValue("SiteLanguage"));
                }

                m_refContentApi.SetCookieValue("LastValidLanguageID", langID.ToString());
                if (Request.QueryString["thickbox"] != "" && (Request.QueryString["tid"] != null))
                {
                    long.TryParse((string)(Request.QueryString["tid"].ToString()), out TaxonomyId);
                    TaxonomyLanguage = langID;
                }
                if (Request.QueryString["profileTaxonomyId"] != "" && Information.IsNumeric(Request.QueryString["profileTaxonomyId"]) && Convert.ToInt64(Request.QueryString["profileTaxonomyId"]) > 0)
                {
                    profileTaxonomyId = Convert.ToInt64(Request.QueryString["profileTaxonomyId"]);
                }
                if (Page.IsPostBack)
                {
                    if (!Page.IsCallback)
                    {
                        Process_EditGroup();
                    }
                }
                else
                {
                    //Invite_UsrSel.Initialize()
                    switch (this.m_sPageAction)
                    {
                        case "delete":
                            Process_DeleteGroup();
                            break;
                        default:
                            EmitJavascript();
                            RenderRecipientSelect();
                            EditGroup();
                            SetTaxonomy(this.m_iID);
                            SetAlias(this.m_iID);
                            break;
                    }
                }
                SetLabels();
            }
        }
        catch (Exception ex)
        {
            Utilities.ShowError(ex.Message);
        }
    }

    public void ucTestEmailSettingsButton_Click(object sender, EventArgs e)
    {
    }

    private void RegisterResources()
    {
        //CSS
        Ektron.Cms.API.Css.RegisterCss(this, AppPath + "csslib/community.css", "EktronCommunityCss");
        Ektron.Cms.API.Css.RegisterCss(this, Ektron.Cms.API.Css.ManagedStyleSheet.EktronUITabsCss);
        Ektron.Cms.API.Css.RegisterCss(this, Ektron.Cms.API.Css.ManagedStyleSheet.EktronFixedPositionToolbarCss);
        Ektron.Cms.API.Css.RegisterCss(this, Ektron.Cms.API.Css.ManagedStyleSheet.EktronModalCss);

        //JS
        Ektron.Cms.API.JS.RegisterJS(this, Ektron.Cms.API.JS.ManagedScript.EktronJS);
        Ektron.Cms.API.JS.RegisterJS(this, Ektron.Cms.API.JS.ManagedScript.EktronUICoreJS);
        Ektron.Cms.API.JS.RegisterJS(this, Ektron.Cms.API.JS.ManagedScript.EktronUITabsJS);
        Ektron.Cms.API.JS.RegisterJS(this, Ektron.Cms.API.JS.ManagedScript.EktronThickBoxJS);
        Ektron.Cms.API.JS.RegisterJS(this, Ektron.Cms.API.JS.ManagedScript.EktronUIButtonJS);
        Ektron.Cms.API.JS.RegisterJS(this, Ektron.Cms.API.JS.ManagedScript.EktronModalJS);
    }

    private void SetTaxonomy(long groupid)
    {
        EditTaxonomyHtml.Text = "<table><tr><td id=\"TreeOutput\"></td></tr></table>";
        DirectoryData[] taxonomy_cat_arr = null;
        TaxonomyBaseData[] taxonomy_data_arr = null;

        taxonomy_cat_arr = this.m_refContentApi.EkContentRef.GetAllAssignedDirectory(groupid, Ektron.Cms.Common.EkEnumeration.TaxonomyItemType.Group);
        if ((taxonomy_cat_arr != null) && taxonomy_cat_arr.Length > 0)
        {
            foreach (DirectoryData taxonomy_cat in taxonomy_cat_arr)
            {
                if (taxonomyselectedtree.Value == "")
                {
                    taxonomyselectedtree.Value = Convert.ToString(taxonomy_cat.DirectoryId);
                }
                else
                {
                    taxonomyselectedtree.Value = taxonomyselectedtree.Value + "," + Convert.ToString(taxonomy_cat.DirectoryId);
                }
            }
        }
        taxonomy_data_arr = m_refContentApi.EkContentRef.GetAllTaxonomyByConfig(EkEnumeration.TaxonomyType.Group);
        if ((taxonomy_data_arr == null || taxonomy_data_arr.Length == 0) && (TaxonomyOverrideId == 0))
        {
            base.Tabs.RemoveAt(2); // taxonomy tab
            bSuppressTaxTab = true;
        }
        TaxonomyTreeIdList = (string)taxonomyselectedtree.Value;
        if (TaxonomyTreeIdList.Trim().Length > 0)
        {
            TaxonomyTreeParentIdList = m_refContentApi.EkContentRef.ReadDisableNodeList(this.m_iID, 2);
        }
        //If in thickbox with preassigned taxonomy, display that taxonomy as checked
        if ((Request.QueryString["thickbox"] != "") && (TaxonomyId > 0))
        {
            TaxonomyTreeIdList = TaxonomyId.ToString();
            if (TaxonomyTreeIdList.Trim().Length > 0)
            {
                TaxonomyBaseData[] taxonomyCategoryList = null;
                taxonomyCategoryList = m_refContentApi.EkContentRef.GetTaxonomyRecursiveToParent(TaxonomyId, TaxonomyLanguage, 0);
                if ((taxonomyCategoryList != null) && taxonomyCategoryList.Length > 0)
                {
                    foreach (TaxonomyBaseData taxonomy_cat in taxonomyCategoryList)
                    {
                        if (TaxonomyTreeParentIdList == "")
                        {
                            TaxonomyTreeParentIdList = taxonomy_cat.TaxonomyId.ToString();
                        }
                        else
                        {
                            TaxonomyTreeParentIdList = TaxonomyTreeParentIdList + "," + taxonomy_cat.TaxonomyId.ToString();
                        }
                    }
                }
            }
        }

        m_intTaxFolderId = 0;
        if ((Request.QueryString["TaxonomyId"] != null) && Request.QueryString["TaxonomyId"] != "")
        {
            TaxonomyOverrideId = Convert.ToInt64(Request.QueryString["TaxonomyId"]);
        }
        js_taxon.Text = Environment.NewLine;
        js_taxon.Text += "var taxonomytreearr=\"" + TaxonomyTreeIdList + "\".split(\",\");" + Environment.NewLine;
        js_taxon.Text += "var taxonomytreedisablearr=\"" + TaxonomyTreeParentIdList + "\".split(\",\");" + Environment.NewLine;
        js_taxon.Text += "var __TaxonomyOverrideId=\"" + TaxonomyOverrideId + "\".split(\",\");" + Environment.NewLine;
        js_taxon.Text += "var m_fullScreenView=false;var __EkFolderId = " + -2 + ";" + Environment.NewLine;
    }
    private void SetAlias(long groupId)
    {
        Ektron.Cms.API.UrlAliasing.UrlAliasCommunity _communityAlias = new Ektron.Cms.API.UrlAliasing.UrlAliasCommunity();
        System.Collections.Generic.List<Ektron.Cms.Common.UrlAliasCommunityData> aliasList;

        aliasList = _communityAlias.GetListGroup(groupId);
        if (aliasList.Count > 0)
        {
            foreach (Ektron.Cms.Common.UrlAliasCommunityData item in aliasList)
            {
                groupAliasList += "<a href= " + this.m_refContentApi.SitePath + item.AliasName + " target=_blank>" + this.m_refContentApi.SitePath + item.AliasName + "</a>";
                groupAliasList += "<br/>";
            }
        }
        else
        {
            phAliasTab.Visible = false;
            phAliasFrame.Visible = false;
        }
    }
    protected void EditGroup()
    {
        CommonApi refCommonAPI = new CommonApi();

        BuildJS();
        if (this.m_iID > 0)
        {
            cgGroup = m_refCommunityGroupApi.GetCommunityGroupByID(this.m_iID);
            lbl_id.Text = cgGroup.GroupId.ToString();
            List<Ektron.Cms.UserData> groupAdmins = new List<UserData>();
            //groupAdmins = commGroup.GetAllCommunityGroupAdmins(this.m_iID);
            commGroup = new Ektron.Cms.Community.CommunityGroup(m_refContentApi.RequestInformationRef);
            groupAdmins = commGroup.GetAllCommunityGroupAdmins(m_iID);
            PopulateData(cgGroup, groupAdmins);
        }
        else
        {
            this.PublicJoinYes_RB.Checked = true;
            if (!Ektron.Cms.DataIO.LicenseManager.LicenseManager.IsFeatureEnable(m_refContentApi.RequestInformationRef, Ektron.Cms.DataIO.LicenseManager.Feature.MembershipUsers))
            {
                this.FeaturesCalendar_CB.Checked = true;
                this.FeaturesForum_CB.Checked = true;
                this.EnableDistributeToSite_CB.Checked = true;
            }
            tr_ID.Visible = false;
            ucEktronGroupEmailSetting.Attributes.Add("style", "display:none;");
            ektouserid__Page.Value = m_refCommunityGroupApi.RequestInformationRef.CallerId.ToString();

        }
        TD_GroupTags.InnerHtml = GetGroupTags();
        txtEmailReplyServer.Value = mailServer.POPServer;
        txtEmailReplyServerPort.Value = mailServer.POPPort.ToString();
        chkUseSsl.Checked = mailServer.POPSSL;
        this.chkEnableEmail.Enabled = _activityApi.IsActivityEmailReplyEnabled || cgGroup.EnableGroupEmail;                
        this.lblEmailServerPortValue.Text = mailServer.POPPort.ToString();
        this.lblEmailServerValue.Text = mailServer.POPServer;
    }

    #region Process

    protected void Process_DeleteGroup()
    {
        cgGroup = this.m_refCommunityGroupApi.GetCommunityGroupByID(this.m_iID);
        if (cgGroup.GroupId > 0 && bAccess)
        {
            this.m_refCommunityGroupApi.DeleteCommunityGroupByID(this.m_iID);

            js_taxon.Text = "var __TaxonomyOverrideId=\"" + TaxonomyOverrideId + "\".split(\",\");" + Environment.NewLine;
            Ektron.Cms.API.JS.RegisterJSBlock(this, "self.parent.location.reload(true); self.parent.ektb_remove();", "EktronSelfParentLocationReloadJS");
        }
        else
        {
            throw (new Exception(GetMessage("err no perm del cgroup")));
        }
    }

    protected void Process_EditGroup()
    {
        string adminUsersValue = hdnAdminUsers.Value.ToString();

        if (!String.IsNullOrEmpty(adminUsersValue))
        {
            // An admin was selected, tidy and split into array
            if (hdnAdminUsers.Value.ToString().IndexOf(",") == 0)
            {
                hdnAdminUsers.Value = hdnAdminUsers.Value.ToString().Substring(1);
            }
            hdnAdminUsers.Value = hdnAdminUsers.Value.ToString().Replace(",,", ",");
            if (hdnAdminUsers.Value.ToString().Substring(hdnAdminUsers.Value.ToString().Length - 1) == ",")
            {
                hdnAdminUsers.Value = hdnAdminUsers.Value.ToString().Remove(hdnAdminUsers.Value.ToString().Length - 1);
            }

            char[] delimiter = new char[] { ',' };
            strAdminUsers = hdnAdminUsers.Value.ToString().Split(delimiter);
        }
        else
        {
            // Admin not selected, current user becomes admin
            strAdminUsers = new string[1]{ m_userId.ToString() };
        }
    
        if (bAccess)
        {
            if (m_iID > 0)
            {
                cgGroup = this.m_refCommunityGroupApi.GetCommunityGroupByID(this.m_iID);
                groupMessageBoardModerate = _MessageBoardApi.IsModerated(m_iID, Ektron.Cms.Common.EkEnumeration.MessageBoardObjectType.CommunityGroup);
                if (groupMessageBoardModerate != false || chkMsgBoardModeration.Checked != false)
                {
                    _MessageBoardApi.Moderate(m_iID, Ektron.Cms.Common.EkEnumeration.MessageBoardObjectType.CommunityGroup, m_userId, System.Convert.ToBoolean(this.chkMsgBoardModeration.Checked));
                }
            }
            else
            {
                cgGroup = new CommunityGroupData();
            }

            cgGroup.GroupName = (string)(this.GroupName_TB.Text.Trim());
            calendardata.Name = "ekCalendar";
            calendardata.Description = "";
            if (Request.Form["ektouserid__Page"] != "" && Information.IsNumeric(Request.Form["ektouserid__Page"]) && Convert.ToInt64(Request.Form["ektouserid__Page"]) > 0)
            {
                if (m_iID > 0)
                {
                    Ektron.Cms.Common.Cache.ApplicationCache.Invalidate((string)("GroupAccess_" + m_iID.ToString() + "_" + cgGroup.GroupAdmin.Id.ToString())); // old
                }
                cgGroup.GroupAdmin.Id = Convert.ToInt64(Request.Form["ektouserid__Page"]);
                if (m_iID > 0)
                {
                    Ektron.Cms.Common.Cache.ApplicationCache.Invalidate((string)("GroupAccess_" + m_iID.ToString() + "_" + cgGroup.GroupAdmin.Id.ToString())); // new
                }
            }
            cgGroup.GroupShortDescription = (string)this.ShortDescription_TB.Text;
            cgGroup.GroupLongDescription = (string)this.Description_TB.Text;
            if (m_iID > 0 && !(cgGroup.GroupEnroll == this.PublicJoinYes_RB.Checked))
            {
                Ektron.Cms.Common.Cache.ApplicationCache.Invalidate((string)("GroupEnroll_" + m_iID.ToString()));
            }
            cgGroup.GroupEnroll = System.Convert.ToBoolean(this.PublicJoinYes_RB.Checked);
            cgGroup.GroupLocation = (string)this.Location_TB.Text;
            cgGroup.GroupHidden = System.Convert.ToBoolean(this.PublicJoinHidden_RB.Checked);
            cgGroup.GroupEnableDistributeToSite = System.Convert.ToBoolean(this.EnableDistributeToSite_CB.Checked);
            cgGroup.AllowMembersToManageFolders = System.Convert.ToBoolean(this.AllowMembersToManageFolders_CB.Checked);

            cgGroup.EnableDocumentsInNotifications = System.Convert.ToBoolean(this.chkEnableDocumentNotifications.Checked);
            cgGroup.EnableGroupEmail = System.Convert.ToBoolean(this.chkEnableEmail.Checked);
            cgGroup.EmailAccountName = EkFunctions.HtmlEncode(this.txtEmailAccount.Text);
            cgGroup.EmailAddress = EkFunctions.HtmlEncode(this.txtEmailAddressValue.Text);

            if (this.txtEmailPassword.Text != "*****")
            {
                cgGroup.EmailPassword = (string)this.txtEmailPassword.Text;
            }

            // taxonomy
            TaxonomyTreeIdList = Request.Form[taxonomyselectedtree.UniqueID];
            if (TaxonomyTreeIdList.Trim().EndsWith(","))
            {
                TaxonomyTreeIdList = TaxonomyTreeIdList.Substring(0, TaxonomyTreeIdList.Length - 1);
            }
            TaxonomyRequest tax_request = new TaxonomyRequest();
            tax_request.TaxonomyIdList = TaxonomyTreeIdList;
            cgGroup.GroupCategory = TaxonomyTreeIdList;
            // taxonomy
            // file
            string sfileloc = "";
            if (fileupload1.PostedFile != null && fileupload1.PostedFile.FileName != "") //Check to make sure we actually have a file to upload
            {
                string strLongFilePath = (string)fileupload1.PostedFile.FileName;
                string[] aNameArray = Strings.Split((string)fileupload1.PostedFile.FileName, "\\", -1, 0);
                string strFileName = "";
                if (aNameArray.Length > 0)
                {
                    strFileName = aNameArray[(aNameArray.Length - 1)];
                }
                strFileName = (string)((System.Guid.NewGuid().ToString()).Substring(0, 5) + "_g_" + strFileName);
                if ((((string)fileupload1.PostedFile.ContentType == "image/pjpeg") || ((string)fileupload1.PostedFile.ContentType == "image/jpeg")) || ((string)fileupload1.PostedFile.ContentType == "image/gif")) //Make sure we are getting a valid JPG/gif image
                {
                    fileupload1.PostedFile.SaveAs(Server.MapPath(m_refCommunityGroupApi.SitePath + "uploadedimages/" + strFileName));
                    lbStatus.Text = string.Format(m_refCommunityGroupApi.EkMsgRef.GetMessage("lbl success avatar uploaded"), strFileName, m_refCommunityGroupApi.SitePath + "uploadedimages/" + strFileName);
                    Utilities.ProcessThumbnail(Server.MapPath(m_refCommunityGroupApi.SitePath + "uploadedimages/"), strFileName);
                }
                else
                {
                    //Not a valid jpeg/gif image
                    lbStatus.Text = m_refCommunityGroupApi.EkMsgRef.GetMessage("lbl err avatar not valid extension");
                }
                sfileloc = m_refCommunityGroupApi.SitePath + "uploadedimages/thumb_" + Utilities.GetCorrectThumbnailFileWithExtn(strFileName);
                cgGroup.GroupImage = sfileloc;
            }
            else
            {
                cgGroup.GroupImage = (string)this.GroupAvatar_TB.Text;
            }
            // file
            if (m_iID > 0)
            {
                m_refCommunityGroupApi.UpdateCommunityGroup(cgGroup);
                UpdateGroupTags(false);
                InitiateProcessAction();
                RemoveCurrentAdminUsers();//Need to remove all admins then re-added
                AddMultipleGroupAdmins(strAdminUsers);
            }
            else
            {
                UserManager Usermanager = new UserManager();
                UserData userBaseData = null;
                List<UserData> userlist = new List<UserData>();
                long longUserId;
                foreach (string userId in strAdminUsers)
                {
                    if (long.TryParse(userId, out longUserId) && longUserId > 0)
                        userBaseData = Usermanager.GetItem(longUserId);
                    if (userBaseData != null && userBaseData.Id > 0) 
                        userlist.Add(userBaseData);
                }
                cgGroup.Admins = userlist;  
                m_iID = m_refCommunityGroupApi.AddCommunityGroup(cgGroup);

                //ADDTAXONOMYITEM to Group eIntranet
                if (!(string.IsNullOrEmpty(TaxonomyTreeIdList)))
                {
                    TaxonomyTreeIdList = TaxonomyTreeIdList + ",";
                }
                else
                {
                    TaxonomyTreeIdList = "";
                }
                if (profileTaxonomyId > 0)
                {
                    m_refCommunityGroupApi.EkContentRef.AddDirectoryItem(TaxonomyTreeIdList + profileTaxonomyId.ToString(), m_iID.ToString(), Ektron.Cms.Common.EkEnumeration.TaxonomyItemType.Group);
                }
                else
                {
                    profileTaxonomyId = m_refCommunityGroupApi.EkContentRef.GetTaxonomyIdByPath("\\" + m_refCommunityGroupApi.UserId + "\\Groups", 1);
                    m_refCommunityGroupApi.EkContentRef.AddDirectoryItem(TaxonomyTreeIdList + profileTaxonomyId.ToString(), m_iID.ToString(), Ektron.Cms.Common.EkEnumeration.TaxonomyItemType.Group);
                }

                groupMessageBoardModerate = _MessageBoardApi.IsModerated(m_iID, Ektron.Cms.Common.EkEnumeration.MessageBoardObjectType.CommunityGroup);
                if (groupMessageBoardModerate != false || chkMsgBoardModeration.Checked != false)
                {
                    _MessageBoardApi.Moderate(m_iID, Ektron.Cms.Common.EkEnumeration.MessageBoardObjectType.CommunityGroup, m_userId, System.Convert.ToBoolean(chkMsgBoardModeration.Checked));
                }


                if (cgGroup.GroupCategory != "")
                {
                    TaxonomyId = 0;
                }
                if (m_iID > 0)
                {
                    UpdateGroupTags(true);
                    InitiateProcessAction();
                }
                else
                {
                    EmitJavascript();
                    RenderRecipientSelect();
                    EditGroup();
                    SetTaxonomy(this.m_iID);
                    SetAlias(this.m_iID);
                    errmsg.InnerHtml = "Error occured while adding this group. Please verify the group name is unique and try again.";
                    errmsg.Attributes.Add("class", "excpt");
                    // GroupName_TB.Attributes.Add("onkeypress", "ClearErr();")
                    GroupName_TB.Focus();
                }
            }
            if (FeaturesCalendar_CB.Checked == true)
            {
                _CalendarApi.Add(calendardata, Ektron.Cms.Common.EkEnumeration.WorkSpace.Group, m_iID);
            }
            if (FeaturesForum_CB.Checked == true && FeaturesForum_CB.Enabled != false)
            {
                m_refCommunityGroupApi.AddCommunityGroupForum(Ektron.Cms.Common.EkEnumeration.WorkSpace.Group, m_iID);
            }

            if (FeaturesTodo_CB.Checked == true)
            {
                AddGroupTodoList();
            }

        }
        else
        {
            throw (new Exception(GetMessage("err no perm add cgroup")));
        }
    }
    protected void AddMultipleGroupAdmins(string[] strAdminUsers)
    {
        List<long> userIdlist = new List<long>();
        
        long[] userAdmin = Array.ConvertAll<string, long>(strAdminUsers, adminUsers => Convert.ToInt64(adminUsers));
        userIdlist.AddRange(userAdmin);

        commGroup.AssignCommunityGroupAdmins(m_iID, userIdlist);
    }
    protected void RemoveCurrentAdminUsers()
    {
        List<UserData> originalAdmins = commGroup.GetAllCommunityGroupAdmins(m_iID);
        List<long> toRemove = originalAdmins.ConvertAll<long>(UD => UD.Id);
        commGroup.RemoveCommunityGroupAdmins(m_iID, toRemove);
    }

    #endregion

    #region Helper Functions
    protected void CheckAccess()
    {
        PermissionData m_pSecurity = this.m_refContentApi.LoadPermissions(0, "folder", 0);
        if (this.m_refContentApi.UserId > 0 && m_pSecurity.IsLoggedIn)
        {
            if (this.m_iID > 0)
            {
                EkEnumeration.GroupMemberStatus mMemberStatus;
                mMemberStatus = this.m_refCommunityGroupApi.GetGroupMemberStatus(this.m_iID, this.m_refContentApi.UserId);
                bAccess = System.Convert.ToBoolean(this.m_refContentApi.IsARoleMember(Ektron.Cms.Common.EkEnumeration.CmsRoleIds.CommunityGroupAdmin) || mMemberStatus == EkEnumeration.GroupMemberStatus.Leader);
                m_bGroupAdmin = bAccess;
            }
            else
            {
                bAccess = System.Convert.ToBoolean(this.m_refContentApi.IsARoleMember(Ektron.Cms.Common.EkEnumeration.CmsRoleIds.CommunityGroupCreate) ||
                    this.m_refContentApi.IsARoleMember(Ektron.Cms.Common.EkEnumeration.CmsRoleIds.CommunityGroupAdmin));
            }
        }
    }
    protected void InitiateProcessAction()
    {
        if (TaxonomyId > 0)
        {
            // add to taxonomy
            TaxonomyRequest item_request = new TaxonomyRequest();
            item_request.TaxonomyId = TaxonomyId;
            item_request.TaxonomyIdList = this.m_iID.ToString();
            item_request.TaxonomyItemType = Ektron.Cms.Common.EkEnumeration.TaxonomyItemType.Group;
            item_request.TaxonomyLanguage = TaxonomyLanguage;
            this.m_refContentApi.EkContentRef.AddTaxonomyItem(item_request);
        }
        if (bThickBox)
        {
            Response.Redirect("CloseThickbox.aspx", false);
        }
        else
        {
            Response.Redirect("community/groups.aspx", false);
        }
    }
    protected void BuildJS()
    {
        StringBuilder sbJS = new StringBuilder();

        sbJS.Append("<script type=\"text/javascript\">").Append(Environment.NewLine);
        sbJS.Append("<!--//--><![CDATA[//><!--").Append(Environment.NewLine);
        sbJS.Append("function SubmitForm() {" + Environment.NewLine);
        sbJS.Append("   var nsPopUser='" + mailServer.POPUsername + "';"); 
        sbJS.Append("   var groupName = document.getElementById(\'GroupName_TB\').value;").Append(Environment.NewLine);
        if (this.m_refContentApi.IsAdmin())
        {
            sbJS.Append("   if($ektron('input#hdnAdminUsers')[0].value == '')").Append(Environment.NewLine);
            sbJS.Append("   {").Append(Environment.NewLine);
            sbJS.Append("       $ektron('input#hdnAdminUsers')[0].value = \'" + m_refContentApi.RequestInformationRef.CallerId.ToString() + "\';").Append(Environment.NewLine);
            sbJS.Append("   }").Append(Environment.NewLine);
        }
        sbJS.Append("   $ektron('input#hdnAdminUsers')[0].value = $ektron('input#hdnAdminUsers')[0].value.split(',');").Append(Environment.NewLine);
        sbJS.Append("   if (Trim(groupName).length == 0)").Append(Environment.NewLine);
        sbJS.Append("   {alert(\'" + GetMessage("lbl please enter group name") + "\');").Append(Environment.NewLine);
        sbJS.Append("   return false;}").Append(Environment.NewLine);
        sbJS.Append("else{ " + Environment.NewLine);
        sbJS.Append("   if (!CheckGroupForillegalChar()) {" + Environment.NewLine);
        sbJS.Append("   		return false;" + Environment.NewLine);
        sbJS.Append("   } else if(document.getElementById('chkEnableEmail').checked && nsPopUser==document.getElementById('txtEmailAccount').value){alert('Group email account cannot be same as notification email reply account.'); return false;}else { document.forms[0].submit(); }" + Environment.NewLine);
        sbJS.Append("}" + Environment.NewLine);
        sbJS.Append("}" + Environment.NewLine);

        sbJS.Append("function ExecSearch() {" + Environment.NewLine);
        sbJS.Append("   var sTerm = Trim(document.getElementById(\'txtSearch\').value); " + Environment.NewLine);
        //sbJS.Append("   if (sTerm == '') {" & Environment.NewLine)
        //sbJS.Append("       alert('").Append(GetMessage("err js no search term")).Append("'); " & Environment.NewLine)
        //sbJS.Append("   } else {" & Environment.NewLine)
        sbJS.Append("	    document.getElementById(\'hdn_search\').value = true;" + Environment.NewLine);
        sbJS.Append("	    document.forms[0].submit();" + Environment.NewLine);
        //sbJS.Append("   }" & Environment.NewLine)
        sbJS.Append("}" + Environment.NewLine);
        sbJS.Append("function resetPostback() {" + Environment.NewLine);
        sbJS.Append("   document.forms[0].isPostData.value = \"\"; " + Environment.NewLine);
        sbJS.Append("}" + Environment.NewLine);

        sbJS.Append("function CheckGroupForillegalChar() {" + Environment.NewLine);
        sbJS.Append("   var val = document.forms[0]." + Strings.Replace((string)this.GroupName_TB.UniqueID, "$", "_", 1, -1, 0) + ".value;" + Environment.NewLine);
        sbJS.Append("   if ((val.indexOf(\";\") > -1) || (val.indexOf(\"\\\\\") > -1) || (val.indexOf(\"/\") > -1) || (val.indexOf(\":\") > -1)||(val.indexOf(\"*\") > -1) || (val.indexOf(\"?\") > -1)|| (val.indexOf(\"\\\"\") > -1) || (val.indexOf(\"<\") > -1)|| (val.indexOf(\">\") > -1) || (val.indexOf(\"|\") > -1) || (val.indexOf(\"&\") > -1) || (val.indexOf(\"\\\'\") > -1))" + Environment.NewLine);
        sbJS.Append("   {" + Environment.NewLine);
        sbJS.Append("       alert(\"" + string.Format(GetMessage("lbl group name disallowed chars"), "(\';\', \'\\\\\', \'/\', \':\', \'*\', \'?\', \' \\\" \', \'<\', \'>\', \'|\', \'&\', \'\\\'\')") + "\");" + Environment.NewLine);
        sbJS.Append("       return false;" + Environment.NewLine);
        sbJS.Append("   }" + Environment.NewLine);
        sbJS.Append("   return true;" + Environment.NewLine);
        sbJS.Append("}" + Environment.NewLine);

        sbJS.Append("  			function LoadLanguage(FormName){ ").Append(Environment.NewLine);
        sbJS.Append("  				var num=document.forms[FormName].selLang.selectedIndex; ").Append(Environment.NewLine);
        sbJS.Append("  				window.location.href=\"community/groups.aspx?action=viewallgroups\"+\"&LangType=\"+document.forms[FormName].selLang.options[num].value; ").Append(Environment.NewLine);
        sbJS.Append("  				//document.forms[FormName].submit(); ").Append(Environment.NewLine);
        sbJS.Append("  				return false; ").Append(Environment.NewLine);
        sbJS.Append("  			} ").Append(Environment.NewLine);

        sbJS.Append("        function CheckUpload() ").Append(Environment.NewLine);
        sbJS.Append("        { ").Append(Environment.NewLine);
        sbJS.Append("            var ofile = document.getElementById(\'fileupload1\'); ").Append(Environment.NewLine);
        sbJS.Append("            if (ofile.value == \'\') { ").Append(Environment.NewLine);
        sbJS.Append("               alert(\'").Append(this.m_refContentApi.EkMsgRef.GetMessage("js err select avatar upload")).Append("\'); ").Append(Environment.NewLine);
        sbJS.Append("               ofile.outerHTML = ofile.outerHTML; ").Append(Environment.NewLine);
        sbJS.Append("               return false; ").Append(Environment.NewLine);
        sbJS.Append("            } else { ").Append(Environment.NewLine);
        sbJS.Append("               if (!CheckUploadExt(ofile.value)) { ").Append(Environment.NewLine);
        sbJS.Append("                   alert(\'").Append(this.m_refContentApi.EkMsgRef.GetMessage("lbl err avatar not valid extension")).Append("\'); ").Append(Environment.NewLine);
        sbJS.Append("                   ofile.outerHTML = ofile.outerHTML; ").Append(Environment.NewLine);
        sbJS.Append("                   return false; ").Append(Environment.NewLine);
        sbJS.Append("               } else { ").Append(Environment.NewLine);
        sbJS.Append("                   document.getElementById(\'GroupAvatar_TB\').value = ofile.value; ").Append(Environment.NewLine);
        sbJS.Append("                   toggleVisibility(\'close\'); ").Append(Environment.NewLine);
        sbJS.Append("               } ").Append(Environment.NewLine);
        sbJS.Append("            } ").Append(Environment.NewLine);
        sbJS.Append("        } ").Append(Environment.NewLine);

        sbJS.Append("        function CheckUploadExt(filename) ").Append(Environment.NewLine);
        sbJS.Append("        { ").Append(Environment.NewLine);
        sbJS.Append("           var extArray = new Array(\".jpg\",\".jpeg\", \".gif\", \".jpeg\"); ").Append(Environment.NewLine);
        sbJS.Append("           allowSubmit = false; ").Append(Environment.NewLine);
        sbJS.Append("               if (filename.indexOf(\"\\\\\") == -1)");
        sbJS.Append("               {");
        sbJS.Append("                   ext = filename.slice(filename.lastIndexOf(\".\")).toLowerCase(); ").Append(Environment.NewLine);
        sbJS.Append("                   for (var i = 0; i < extArray.length; i++)  ").Append(Environment.NewLine);
        sbJS.Append("                   { ").Append(Environment.NewLine);
        sbJS.Append("               	    if (extArray[i] == ext) { allowSubmit = true; break; } ").Append(Environment.NewLine);
        sbJS.Append("                   } ").Append(Environment.NewLine);
        sbJS.Append("               }");
        sbJS.Append("           while (filename.indexOf(\"\\\\\") != -1) ").Append(Environment.NewLine);
        sbJS.Append("           { ").Append(Environment.NewLine);
        sbJS.Append("               filename = filename.slice(filename.indexOf(\"\\\\\") + 1); ").Append(Environment.NewLine);
        sbJS.Append("               ext = filename.slice(filename.lastIndexOf(\".\")).toLowerCase(); ").Append(Environment.NewLine);
        sbJS.Append("               for (var i = 0; i < extArray.length; i++)  ").Append(Environment.NewLine);
        sbJS.Append("               { ").Append(Environment.NewLine);
        sbJS.Append("               	if (extArray[i] == ext) { allowSubmit = true; break; } ").Append(Environment.NewLine);
        sbJS.Append("               } ").Append(Environment.NewLine);
        sbJS.Append("           } ").Append(Environment.NewLine);
        sbJS.Append("           return allowSubmit; ").Append(Environment.NewLine);
        sbJS.Append("        } ").Append(Environment.NewLine);
        sbJS.Append("//--><!]]>").Append(Environment.NewLine);
        sbJS.Append("</script>").Append(Environment.NewLine);

        ltr_js.Text = sbJS.ToString();
    }

    protected void SetLabels()
    {
        phCategoryTab.Visible = !bSuppressTaxTab;
        phCategoryFrame.Visible = !bSuppressTaxTab;
        this.ltr_groupname.Text = GetMessage("lbl community group name");
        this.ltr_groupid.Text = GetMessage("generic id");
        this.ltr_admin.Text = GetMessage("lbl administrator");
        this.ltr_groupjoin.Text = GetMessage("lbl enrollment");
        this.ltr_grouplocation.Text = GetMessage("generic location");
        this.ltr_groupsdesc.Text = GetMessage("lbl short desc");
        this.ltr_groupdesc.Text = GetMessage("generic description");
        this.ltr_groupavatar.Text = GetMessage("lbl group image");
        this.ltr_enabledistribute.Text = GetMessage("lbl enable distribute");
        this.ltr_AllowMembersToManageFolders.Text = GetMessage("lbl allow member to manage folders");
        this.ltr_upload.Text = GetMessage("upload txt");
        this.ltr_ok.Text = GetMessage("lbl ok");
        this.ltr_close.Text = GetMessage("close title");
        this.ltr_MsgBoardModeration.Text = GetMessage("lbl msgboardmoderation");
        this.ltrlEnableDocumentNotifications.Text = GetMessage("lbl Email Notifications");
        this.Literal1.Text = GetMessage("lbl Enable Group Emails");
        this.ltr_Emaildesc.Text = GetMessage("lbl Group Email");
        PublicJoinYes_RB.Text = GetMessage("lbl enrollment open");
        PublicJoinNo_RB.Text = GetMessage("lbl enrollment restricted");
        PublicJoinHidden_RB.Text = GetMessage("lbl enrollment hidden");
        this.ltr_groupfeatures.Text = GetMessage("lbl features") + ":";
        FeaturesCalendar_CB.Text = GetMessage("lbl enable group calendar");
        FeaturesForum_CB.Text = GetMessage("lbl enable group forum");
        FeaturesTodo_CB.Text = GetMessage("lbl enable group todo");

        if (this.m_iID > 0)
        {
			if (!bThickBox)
			{
				AddBackButton("community/groups.aspx?action=viewgroup&id=" + cgGroup.GroupId.ToString() + "");
			}

            base.AddButtonwithMessages(AppPath + "images/UI/Icons/save.png", "#", "lbl alt save cgroup", "btn save", " onclick=\"javascript: SubmitForm(); return false;\" ", StyleHelper.SaveButtonCssClass, true);
            
			if (bThickBox)
            {
                //MyBase.AddButtonwithMessages(AppPath & "images/UI/Icons/cancel.png", "#", "generic cancel", "generic cancel", " onclick=""self.parent.ektb_remove();"" return false;"" ")
                if (m_bGroupAdmin)
                {
                    base.AddButtonwithMessages(AppPath + "images/UI/Icons/delete.png", "communitygroupaddedit.aspx?action=delete&id=" + this.m_iID + "&thickbox=true", "alt del community group", "lbl del community group", " onclick=\"return confirm(\'" + GetMessage("js confirm del community group") + "\');\" ", StyleHelper.DeleteButtonCssClass);
                }
            }
            else
            {
                SetTitleBarToMessage("lbl edit cgroup");
                AddHelpButton("editcommunitygroup");
            }
        }
        else
        {
			if (!bThickBox)
			{
				AddBackButton("community/groups.aspx");
			}

            tr_admin.Visible = this.m_refContentApi.IsAdmin() || System.Convert.ToBoolean(this.m_refContentApi.IsARoleMember(Ektron.Cms.Common.EkEnumeration.CmsRoleIds.CommunityGroupCreate) ||
                    this.m_refContentApi.IsARoleMember(Ektron.Cms.Common.EkEnumeration.CmsRoleIds.CommunityGroupAdmin));
			base.AddButtonwithMessages(AppPath + "images/UI/Icons/save.png", "#", "lbl alt save cgroup", "btn save", " onclick=\"javascript: SubmitForm(); return false;\" ", StyleHelper.SaveButtonCssClass, true);
            if (bThickBox)
            {
                //MyBase.AddButtonwithMessages(AppPath & "images/UI/Icons/cancel.png", "#", "generic cancel", "generic cancel", " onclick=""self.parent.ektb_remove();"" return false;"" ")
            }
            else
            {
                SetTitleBarToMessage("lbl add cgroup");
                AddHelpButton("addcommunitygroup");
            }
        }

    }

    protected void PopulateData(CommunityGroupData cGrp, List<UserData> groupAdmins)
    {
        int i = 0;
        StringBuilder sBuilder = new StringBuilder();

        this.GroupName_TB.Text = cGrp.GroupName;
        this.ShortDescription_TB.Text = cGrp.GroupShortDescription;
        this.Description_TB.Text = cGrp.GroupLongDescription;
        this.PublicJoinYes_RB.Checked = cGrp.GroupEnroll;        
        this.PublicJoinNo_RB.Checked = !(cGrp.GroupEnroll);
        this.PublicJoinHidden_RB.Checked = cGrp.GroupHidden;
        this.Location_TB.Text = cGrp.GroupLocation;
        this.GroupAvatar_TB.Text = cGrp.GroupImage;
        this.EnableDistributeToSite_CB.Checked = cGrp.GroupEnableDistributeToSite;
        this.AllowMembersToManageFolders_CB.Checked = cGrp.AllowMembersToManageFolders;
        this.ltr_avatarpath.Text = "";
        // Me.tb_admin_name.Text = cGrp.GroupAdmin.DisplayName
        groupMessageBoardModerate = _MessageBoardApi.IsModerated(this.m_iID, Ektron.Cms.Common.EkEnumeration.MessageBoardObjectType.CommunityGroup);
        this.chkMsgBoardModeration.Checked = groupMessageBoardModerate;

        //ekpmsgto__Page.Value = cGrp.GroupAdmin.DisplayName;
        if (groupAdmins.Count > 0)
        {
            sBuilder.Append("arGroupAdmins = new Array(");
            int order = 0;
            for (i = 0; i < groupAdmins.Count; i++)
            {
                if(order!=0)
                    sBuilder.Append(",");
                if (groupAdmins[i].DisplayName != "")
                sBuilder.Append("new groupAdmin('" + groupAdmins[i].DisplayName + "','" + groupAdmins[i].Id + "')");
                else
                    sBuilder.Append("new groupAdmin('" + groupAdmins[i].FirstName  + "','" + groupAdmins[i].Id + "')");
                
                order += 1;
            }
            sBuilder.AppendLine(")");
            sBuilder.AppendLine("renderGroupAdmins(" + cGrp.GroupId + ");");
        }
        Page.ClientScript.RegisterStartupScript(this.GetType(), "renderGroupAdmins", sBuilder.ToString(), true);
        
        ektouserid__Page.Value = cGrp.GroupAdmin.Id.ToString();

        this.chkEnableDocumentNotifications.Checked = cGrp.EnableDocumentsInNotifications;
        this.chkEnableEmail.Checked = cGrp.EnableGroupEmail;
        this.chkEnableEmail.Enabled = _activityApi.IsActivityEmailReplyEnabled;
        this.txtEmailAccount.Text = EkFunctions.HtmlDecode(cGrp.EmailAccountName);
        this.txtEmailAddressValue.Text = EkFunctions.HtmlDecode(cGrp.EmailAddress);

        if (!string.IsNullOrEmpty(cGrp.EmailPassword))
        {
            this.txtEmailPassword.Text = "*****";
            this.txtEmailPassword.Attributes.Add("value", "*****");
        }
        ucEktronGroupEmailSetting.Attributes.Add("style", "display:none;");
        if (cGrp.EnableGroupEmail)
        {
            ucEktronGroupEmailSetting.Attributes.Add("style", "display:block;");
            ucEktronGroupEmailSetting.Visible = cGrp.EnableGroupEmail;
        }

        txtEmailReplyServer.Value = mailServer.POPServer;
        txtEmailReplyServerPort.Value = mailServer.POPPort.ToString();
        this.lblEmailServerPortValue.Text = mailServer.POPPort.ToString();
        this.lblEmailServerValue.Text = mailServer.POPServer;
        this.chkUseSsl.Checked = mailServer.POPSSL;
        chkUseSsl.Checked = mailServer.POPSSL;

        calendardata = _CalendarApi.GetPublicCalendar(Ektron.Cms.Common.EkEnumeration.WorkSpace.Group, this.m_iID);
        if (calendardata != null)
        {
            FeaturesCalendar_CB.Enabled = false;
            FeaturesCalendar_CB.Checked = true;
        }
        _doesForumExists = m_refCommunityGroupApi.DoesCommunityGroupForumExists(Ektron.Cms.Common.EkEnumeration.WorkSpace.Group, this.m_iID);
        if (_doesForumExists > 0)
        {
            FeaturesForum_CB.Enabled = false;
            FeaturesForum_CB.Checked = true;
        }

        var todoList = GetGroupTodoList();
        if (todoList != null)
        {
            FeaturesTodo_CB.Enabled = false;
            FeaturesTodo_CB.Checked = true;
        }

        if (this.m_sPageAction == "viewgroup")
        {
            this.GroupName_TB.Enabled = false;
            this.ShortDescription_TB.Enabled = false;
            this.Description_TB.Enabled = false;
            this.PublicJoinYes_RB.Enabled = false;
            this.PublicJoinNo_RB.Enabled = false;
            this.PublicJoinHidden_RB.Enabled = false;
            this.Location_TB.Enabled = false;
            this.EnableDistributeToSite_CB.Enabled = false;
            this.AllowMembersToManageFolders_CB.Enabled = false;
        }
    }

    public MailServerData GetNotificationEmailServer()
    {
        IMailServer emailServerApi = ObjectFactory.GetMailServer();
        MailServerData mailServerData = new MailServerData();

        Criteria<MailServerProperty> criteria = new Criteria<MailServerProperty>();
        criteria.AddFilter(MailServerProperty.Type, CriteriaFilterOperator.EqualTo, MailServerType.CommunityEmailNotification);

        List<MailServerData> servers = emailServerApi.GetList(criteria);

        if (servers.Count > 0)
        {
            mailServerData = servers[0];
        }
        return mailServerData;
    }

     protected void AddGroupTodoList()
    {
        Ektron.Cms.Framework.ToDo.TodoList todoListApi = new Ektron.Cms.Framework.ToDo.TodoList();
        TodoListData todoListData = GetGroupTodoList();

        //if todo list doesnt exist, add it
        if (todoListData == null)
        {
            todoListData = new Ektron.Cms.ToDo.TodoListData() { Name = this.GroupName_TB.Text, ObjectType = Ektron.Cms.Common.EkEnumeration.CMSObjectTypes.CommunityGroup, ObjectId = this.m_iID };

            todoListApi.Add(todoListData);
        }


    }

    protected TodoListData GetGroupTodoList()
    {

        Ektron.Cms.Framework.ToDo.TodoList todoListApi = new Ektron.Cms.Framework.ToDo.TodoList();
        TodoListCriteria criteria = new TodoListCriteria();
        
        criteria.AddFilter(TodoListProperty.ObjectType, CriteriaFilterOperator.EqualTo, EkEnumeration.CMSObjectTypes.CommunityGroup);
        criteria.AddFilter(TodoListProperty.ObjectId, CriteriaFilterOperator.EqualTo, this.m_iID);

        List<TodoListData> list = todoListApi.GetList(criteria);

        if (list.Count > 0)
        {
            return list[0];
        }
        else
        {
            return null;
        }
    }

    protected void DeleteGroupTodoList()
    {
        Ektron.Cms.Framework.ToDo.TodoList todoListApi = new Ektron.Cms.Framework.ToDo.TodoList();
        TodoListData todoListData = GetGroupTodoList();

        if (todoListData != null)
        {
            todoListApi.Delete(todoListData.Id);
        }
    }
    #endregion

    #region Group Tags
    public string GetGroupTags()
    {
        string returnValue;
        System.Text.StringBuilder result = new System.Text.StringBuilder();
        TagData[] tdaGroup = (TagData[])Array.CreateInstance(typeof(TagData), 0);
        TagData[] tdaGroupDefault = (TagData[])Array.CreateInstance(typeof(TagData), 0);
        TagData td;
        Hashtable htTagsAssignedToGroup = new Hashtable();
        Hashtable htDefTagsAssignedToGroup = new Hashtable();
        try
        {

            error_TagsCantBeBlank.Text = m_refMsg.GetMessage("msg error Blank Tag");
            error_InvalidChars.Text = m_refMsg.GetMessage("msg error Tag invalid chars");

            result.Append("<div id=\"newTagNameDiv\" class=\"ektronWindow ektronModalStandard\">");
            result.Append("<div class=\"ektronModalHeader\">");
            result.Append("     <h3>");
            result.Append("         <span class=\"headerText\">" + m_refMsg.GetMessage("btn add personal tag") + "</span>");
            result.Append("         <a id=\"closeDialogLink3\" class=\"ektronModalClose\" href=\"#\" onclick=\"CancelSaveNewGroupTag();\"></a>");
            result.Append("     </h3>");
            result.Append("</div>");
            result.Append("<div class=\"ektronModalBody\">");
            result.Append(" <label class=\"nameWidth\">" + GetMessage("name label") + "</label>&#160;<input type=\"text\" id=\"newTagName\" value=\"\" size=\"20\" />");
            result.Append("<div class=\"ektronTopSpace\"/>");

            if (IsSiteMultilingual)
            {
                result.Append("<div class=\"ektronTopSpace\">");
            }
            else
            {
                result.Append("<div style=\"display:none;\" >");
            }
            result.Append(" <label class=\"nameWidth\">" + GetMessage("res_lngsel_lbl") + "</label>&#160;" + GetLanguageDropDownMarkup("TagLanguage"));
            result.Append("</div><br />");

            if (this.m_iID > 0)
            {
                tdaGroup = m_refTagsApi.GetTagsForObject(this.m_iID, EkEnumeration.CMSObjectTypes.CommunityGroup, -1);
            }
            StringBuilder appliedTagIds = new StringBuilder();
            if (tdaGroup != null)
            {
                foreach (TagData tempLoopVar_td in tdaGroup)
                {
                    td = tempLoopVar_td;
                    htTagsAssignedToGroup.Add(td.Id, td);
                    appliedTagIds.Append(td.Id.ToString() + ",");
                    //sAppliedTags = sAppliedTags & td.Id & ";"
                }
            }

            result.Append("<div class=\"ektronTopSpace\">");
            result.Append(" <ul class=\"buttonWrapper ui-helper-clearfix\">");
            result.Append("     <li>");
            result.Append("         <a class=\"button buttonInline redHover buttonClear\" type=\"button\" value=\"" + GetMessage("btn cancel") + "\" title=\"" + GetMessage("btn cancel") + "\" onclick=\"CancelSaveNewGroupTag();\">" + GetMessage("btn cancel") + "</a>");
            result.Append("     </li>");
            result.Append("     <li>");
            result.Append("         <a class=\"button buttonInline greenHover buttonUpdate\" type=\"button\" value=\"" + GetMessage("btn save") + "\" title=\"" + GetMessage("btn save") + "\" onclick=\"SaveNewGroupTag();\">" + GetMessage("btn save") + "</a> ");
            result.Append("     </li>");
            result.Append(" </ul>");
            result.Append("</div>");

            //create hidden list of current tags so we know to delete removed ones.
            result.Append("<input type=\"hidden\" id=\"currentTags\" name=\"currentTags\" value=\"" + appliedTagIds.ToString() + "\"  />");
            //hidden variable for capturing new tags
            result.Append("<input type=\"hidden\" id=\"newTagNameHdn\" name=\"newTagNameHdn\" />");

            result.Append("</div>");
            result.Append("</div>");
            result.Append("</div>");

            result.Append("<div id=\"newTagNameScrollingDiv\" class=\"ektronBorder\">");

            if (true)
            {
                tdaGroupDefault = m_refTagsApi.GetDefaultTags(EkEnumeration.CMSObjectTypes.CommunityGroup, -1);
                //create hidden list of current tags so we know to delete removed ones.
                result.Append("<input type=\"hidden\" id=\"currentTags\" name=\"currentTags\" value=\"" + appliedTagIds.ToString() + "\"  />");

                LocalizationAPI localizationApi = new LocalizationAPI();


                foreach (LanguageData lang in languageDataArray)
                {
                    //create hidden list of current tags so we know to delete removed ones.
                    result.Append("<input type=\"hidden\" id=\"flag_" + lang.Id + ("\"  value=\"" + localizationApi.GetFlagUrlByLanguageID(lang.Id) + "\"  />"));
                }

                if (tdaGroupDefault != null)
                {
                    foreach (TagData tempLoopVar_td in tdaGroupDefault)
                    {
                        td = tempLoopVar_td;
                        bool bCheck = false;
                        if (htTagsAssignedToGroup.ContainsKey(td.Id))
                        {
                            bCheck = true;
                            htDefTagsAssignedToGroup.Add(td.Id, td);
                        }
                        result.Append("<input type=\"checkbox\" " + (bCheck ? "checked=\"checked\"" : "") + (" id=\"userPTagsCbx_" + td.Id.ToString() + "\" name=\"userPTagsCbx_" + td.Id.ToString() + "\" onclick=\"ToggleCustomPTagsCbx(this, \'" + td.Text.Replace("\'", "\\\'") + "\');\" />&#160;"));
                        result.Append("<img src=\'" + localizationApi.GetFlagUrlByLanguageID(td.LanguageId) + "\' />");
                        result.Append("&#160;" + td.Text + "<br />");
                    }
                }
                if (tdaGroup != null)
                {
                    foreach (TagData tempLoopVar_td in tdaGroup)
                    {
                        td = tempLoopVar_td;
                        if (!htDefTagsAssignedToGroup.ContainsKey(td.Id))
                        {
                            result.Append("<input type=\"checkbox\" checked=\"checked\" id=\"userPTagsCbx_" + td.Id.ToString() + "\" name=\"userPTagsCbx_" + td.Id.ToString() + "\" onclick=\"ToggleCustomPTagsCbx(this, \'" + td.Text.Replace("\'", "\\\'") + "\');\");\' />&#160;");
                            result.Append("<img src=\'" + localizationApi.GetFlagUrlByLanguageID(td.LanguageId) + "\' />");
                            result.Append("&#160;" + td.Text + "<br />");
                        }
                    }
                }
            }

            result.Append("<div id=\"newAddedTagNamesDiv\"></div>");

            result.Append("</div>");

            result.Append("<div style=\"float:left;\">");
            result.Append("     <a class=\"button buttonLeft greenHover buttonAddTagWithText\" href=\"javascript:ShowAddGroupTagArea();\" title=\"" + GetMessage("alt add btn text (group tag)") + "\">");
            result.Append("" + GetMessage("btn add personal tag") + "</a>");
            result.Append("</div>");

        }
        catch (Exception)
        {
        }
        finally
        {
            returnValue = result.ToString();
            tdaGroup = null;
            td = null;
            htTagsAssignedToGroup = null;
            htDefTagsAssignedToGroup = null;
        }
        return returnValue;
    }
    public bool UpdateGroupTags(bool IsAdd)
    {
        bool returnValue;
        bool result = false;
        TagData[] defaultTags;
        TagData[] groupTags;

        string tagIdStr = "";


        try
        {
            string orginalTagIds;
            orginalTagIds = Request.Form["currentTags"].Trim().ToLower();

            // Assign all default group tags that are checked:
            // Remove tags that have been unchecked
            defaultTags = m_refTagsApi.GetDefaultTags(EkEnumeration.CMSObjectTypes.CommunityGroup, -1);
            groupTags = m_refTagsApi.GetTagsForObject(m_iID, EkEnumeration.CMSObjectTypes.CommunityGroup, -1);

            //Also, copy all users tags into defaultUserTags list
            //so that if they were removed, they can be deleted as well.
            int originalLength = defaultTags.Length;
            Array.Resize<TagData>(ref defaultTags, defaultTags.Length + groupTags.Length);
            groupTags.CopyTo(defaultTags, originalLength);

            if (defaultTags != null)
            {
                foreach (TagData td in defaultTags)
                {
                    tagIdStr = (string)("userPTagsCbx_" + td.Id.ToString());
                    if (!(Request.Form[tagIdStr] == null))
                    {

                        if (Request.Form[tagIdStr].Trim().ToLower() == "on")
                        {
                            //if tag is checked, but not in current tag list, add it
                            if (!orginalTagIds.Contains(td.Id.ToString() + ","))
                            {
                                m_refTagsApi.AddTagToCommunityGroup(td.Id, m_iID);
                            }
                        }
                        else
                        {
                            //if tag is unchecked AND in current list, delete
                            if (orginalTagIds.Contains(td.Id.ToString() + ","))
                            {
                                m_refTagsApi.DeleteTagOnObject(td.Id, m_iID, EkEnumeration.CMSObjectTypes.CommunityGroup, m_userId);
                            }
                        }
                    }
                    else
                    {
                        //if tag checkbox has no postback value AND is in current tag list, delete it
                        if (orginalTagIds.Contains(td.Id.ToString() + ","))
                        {
                            m_refTagsApi.DeleteTagOnObject(td.Id, m_iID, EkEnumeration.CMSObjectTypes.CommunityGroup, m_userId);
                        }
                    }
                }
            }

            // Now add any new custom tags, that the user created:
            // New tags are added to newTagNameHdn field in following format:  <TagText>~<LanguageID>;<TagText>~<LanguageID>;
            if (!(Request.Form["newTagNameHdn"] == null))
            {
                string custTags = Request.Form["newTagNameHdn"];
                string[] aCustTags = custTags.Split(";".ToCharArray());

                int languageId;

                foreach (string tag in aCustTags)
                {

                    string[] tagPropArray = tag.Split("~".ToCharArray());
                    if (tagPropArray.Length > 1)
                    {
                        if (tagPropArray[0].Trim().Length > 0)
                        {

                            //Default language to -1.
                            //"ALL" option in drop down is 0 - switch to -1.
                            if (!int.TryParse(tagPropArray[1], out languageId))
                            {
                                languageId = -1;
                            }
                            if (languageId == 0)
                            {
                                languageId = -1;
                            }

                            m_refTagsApi.AddTagToCommunityGroup(tagPropArray[0], m_iID, languageId);
                        }
                    }
                }
            }

            result = true;

            result = true;

        }
        catch (Exception)
        {
            result = false;
        }
        finally
        {
            returnValue = result;
        }
        return returnValue;
    }
    private string GetLanguageDropDownMarkup(string controlId)
    {

        int i;
        StringBuilder markup = new StringBuilder();

        if (IsSiteMultilingual)
        {
            markup.Append("<select id=\"" + controlId + "\" name=\"" + controlId + "\">");
            if (!(languageDataArray == null))
            {
                for (i = 0; i <= languageDataArray.Length - 1; i++)
                {
                    if (languageDataArray[i].SiteEnabled)
                    {
                        markup.Append("<option ");
                        if (this.m_refContentApi.DefaultContentLanguage == languageDataArray[i].Id)
                        {
                            markup.Append(" selected");
                        }
                        markup.Append(" value=" + languageDataArray[i].Id + ">" + languageDataArray[i].LocalName);
                    }
                }
            }
            markup.Append("</select>");
        }
        else
        {
            //hardcode to default site language
            markup.Append("<select id=\"" + controlId + "\" name=\"" + controlId + "\" selectedindex=\"0\" >");
            markup.Append(" <option selected value=" + m_refContentApi.DefaultContentLanguage + ">");
            markup.Append("</select>");

        }

        return markup.ToString();
    }
    #endregion

    #region Browse Users
    protected UserAPI GetUserAPI()
    {
        if (m_refUserApi == null)
        {
            m_refUserApi = new UserAPI();
        }
        return m_refUserApi;
    }

    protected void EmitJavascript()
    {
        if ((Page != null) && !Page.ClientScript.IsClientScriptBlockRegistered("AjaxJavascript"))
        {
            Page.ClientScript.RegisterClientScriptBlock(this.GetType(), "AjaxJavascript", GetAjaxJavascript());

            EmitInitializationJavascript();
        }
    }

    protected string GetAjaxJavascript()
    {
        string result = "";
        System.Text.StringBuilder sb = new System.Text.StringBuilder();

        try
        {
            if (!(Page.IsCallback))
            {

                sb.Append("<script type=\"text/javascript\">" + Environment.NewLine);
                sb.Append("<!--//--><![CDATA[//><!--" + Environment.NewLine);

                string ServerCallFunctionInvocation = Page.ClientScript.GetCallbackEventReference(this, "args", "MsgTarg_AjaxDisplayResult", "context", "MsgTarg_AjaxDisplayError", false);

                sb.Append("function __MsgTargCallBackToServer" + m_uniqueId + "(args,context){" + Environment.NewLine);
                sb.Append(ServerCallFunctionInvocation);
                sb.Append("}" + Environment.NewLine);

                sb.Append("//--><!]]>" + Environment.NewLine);
                sb.Append("</script>" + Environment.NewLine);
            }

        }
        catch (Exception)
        {

        }
        finally
        {
            result = sb.ToString();
            sb = null;
        }

        return (result);
    }

    protected void EmitInitializationJavascript()
    {
        System.Text.StringBuilder sb = new System.Text.StringBuilder();

        try
        {
            if (!(Page.IsCallback))
            {
                // Create initialization code:
                sb.Append("<script type=\"text/javascript\">" + Environment.NewLine);
                sb.Append("<!--//--><![CDATA[//><!-- \\n " + Environment.NewLine);

                sb.Append("GetCommunityMsgObject(\'" + m_uniqueId + "\').SetUserSelectId(\'" + Invite_UsrSel.ControlId + "\');" + Environment.NewLine);
                //sb.Append("// Intialize:" + Environment.NewLine)
                //sb.Append("GetCommunityMsgObject('" + m_uniqueId + "').MsgInitSelectedUsers();" + Environment.NewLine)
                sb.Append("//--><!]]>" + Environment.NewLine);
                sb.Append("</script>" + Environment.NewLine);
                litInitialize.Text = sb.ToString();

                // Create browse button:
                BrowseUsers.Text = "<a class=\"button buttonInlineBlock blueHover btnUpload buttonBrowseUSer\" href=\"#\" onclick=\"GetCommunityMsgObject(\'" + m_uniqueId + "\').MsgShowMessageTargetUI(\'ektouserid" + m_uniqueId + "\', true); return false;\" >" + GetMessage("btn browse") + "</a>";
            }

        }
        catch (Exception)
        {
            if (Page.Request.Url.ToString().IndexOf("forum=1") == -1)
            {

            }
        }
        finally
        {
            sb = null;
        }
    }

    protected void RenderRecipientSelect()
    {
        string outStr = "";
        CollectSearchText();
        GetUsers();

        outStr = (string)("<div id=\"EktMsgTargetsBody" + m_uniqueId + "\" class=\"EktMsgTargetsBody\">" + Environment.NewLine);
        outStr += BuildRecipientSelect();
        outStr += "</div>";
        //Me.ltr_recipientselect.Text = outStr
    }

    private void CollectSearchText()
    {
        int taxType = 1; // Set to one for real data. For development only temporarily set to zero (zero allows creating freind taxonomies using existing taxonomy UI).

        long myTaxId = GetUserAPI().EkContentRef.GetTaxonomyIdByPath((string)("\\" + m_userId.ToString()), taxType);
        string mytaxonomyquery = " and user_id in (select taxonomy_item_id from taxonomy_item_tbl where (taxonomy_id=" + myTaxId.ToString() + " or (taxonomy_id in (select taxonomy_child_id from taxonomy_children_tbl where taxonomy_id=" + myTaxId.ToString() + "))) and taxonomy_item_type=1 )";

        if (m_searchMode == "all_names")
        {
            m_strSearchText = " (first_name like \'%" + Quote(m_strKeyWords) + "%\' OR last_name like \'%" + Quote(m_strKeyWords) + "%\' OR user_name like \'%" + Quote(m_strKeyWords) + "%\')";
        }
        else if (m_searchMode == "last_name")
        {
            m_strSearchText = " (last_name like \'%" + Quote(m_strKeyWords) + "%\') ";
        }
        else if (m_searchMode == "first_name")
        {
            m_strSearchText = " (first_name like \'%" + Quote(m_strKeyWords) + "%\')";
        }
        else if (m_searchMode == "display_name")
        {
            m_strSearchText = " (display_name like \'%" + Quote(m_strKeyWords) + "%\')";
        }

        if (m_friendsOnly)
        {
            m_strSearchText += mytaxonomyquery;
        }
    }

    private void GetUsers()
    {
        if (m_strSearchText.Trim() != "")
        {
            UserRequestData req = new UserRequestData();
            req.Type = -1; // IIf(m_UserType = UserTypes.AuthorType, 0, 1)
            if (GetUserAPI().IsAdmin())
            {
                req.Group = -1; // IIf(m_UserType = UserTypes.AuthorType, 2, 888888)
            }
            else
            {
                req.Group = userList.EkUserRef.GetCmsGroupForCommunityGroup(m_iID);
            }
            req.RequiredFlag = 0;
            req.OrderBy = "";
            req.OrderDirection = "asc";
            req.SearchText = m_strSearchText;
            req.PageSize = 4;
            req.CurrentPage = m_recipientsPage;
            m_user_list = userList.GetAllUsers(ref req);
            //m_user_list = GetUserAPI().GetAllUsers(req)
            m_intTotalPages = req.TotalPages;
        }
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

    protected string IsSelected(string msg)
    {
        string result = "";
        if (msg == m_searchMode)
        {
            result = " selected=\"selected\" ";
        }
        return (result);
    }

    protected string BuildRecipientSelect()
    {
        string result = "";
        string DisplayName = "";
        StringBuilder sbSelect = new StringBuilder();

        sbSelect.Append("	<div class=\"EktMsgTargetsTopControls\" >" + Environment.NewLine);

        sbSelect.Append("	<div class=\"EktMsgTargetsTopControlsSearch\" >" + Environment.NewLine);
        sbSelect.Append("		<input type=\"text\" class=\"EktMsgTargetsSearchText\" id=\"txtSearch" + m_uniqueId + "\" name=\"txtSearch" + m_uniqueId + "\" value=\"" + m_strKeyWords + ("\" >" + Environment.NewLine));
        sbSelect.Append("		<select id=\"searchModeSel" + m_uniqueId + "\" name=\"searchModeSel" + m_uniqueId + "\" >" + Environment.NewLine);
        sbSelect.Append("			<option value=\"display_name\"" + IsSelected("display_name") + (">" + m_refMsg.GetMessage("generic display name") + "</option>" + Environment.NewLine));
        sbSelect.Append("			<option value=\"last_name\"" + IsSelected("last_name") + (">" + m_refMsg.GetMessage("generic last name") + "</option>" + Environment.NewLine));
        sbSelect.Append("			<option value=\"first_name\"" + IsSelected("first_name") + (">" + m_refMsg.GetMessage("generic first name") + "</option>" + Environment.NewLine));
        sbSelect.Append("			<option value=\"all_names\"" + IsSelected("all_names") + (">" + m_refMsg.GetMessage("generic all") + "</option>" + Environment.NewLine));
        sbSelect.Append("		</select>" + Environment.NewLine);
        sbSelect.Append("		<input type=\"button\" value=\"Search\" id=\"btnSearch" + m_uniqueId + "\" name=\"btnSearch" + m_uniqueId + "\" onclick=\"GetCommunityMsgObject(\'" + m_uniqueId + "\').MsgTarg_Search(\'\',\'\');\">" + Environment.NewLine);
        sbSelect.Append("	</div>" + Environment.NewLine);

        if ((m_user_list != null) && m_user_list.Length > 0)
        {
            sbSelect.Append("	<div class=\"EktMsgTargetsTopControlsSelectAll\" >" + Environment.NewLine);
            // sbSelect.Append("		<input id=""EktMsgTargets_SelAll" + m_uniqueId + """ class=""EktMsgTargetCtlSelAll"" name=""EktMsgTargets_SelAll" + m_uniqueId + """ onclick=""GetCommunityMsgObject('" + m_uniqueId + "').MsgToggleSelectAllTarget(this)"" title=""" + m_refMsg.GetMessage("generic select all shown msg") + """ type=""checkbox"" >" + m_refMsg.GetMessage("generic select all shown msg") + "</input>" + Environment.NewLine)
            sbSelect.Append("	</div>" + Environment.NewLine);
            sbSelect.Append("	</div>" + Environment.NewLine);

            sbSelect.Append("	<div class=\"EktMsgTargetsMiddle\" >" + Environment.NewLine);
            sbSelect.Append("	<table class=\"EktMsgTargetTable\">" + Environment.NewLine);
            sbSelect.Append("		<tbody>" + Environment.NewLine);
            for (int idx = 0; idx <= (m_user_list.Length - 1); idx++)
            {
                DisplayName = (string)((m_user_list[idx].DisplayName.Trim().Length > 0) ? (m_user_list[idx].DisplayName.Trim()) : (m_user_list[idx].DisplayUserName.Trim()));
                sbSelect.Append("			<tr>" + Environment.NewLine);
                sbSelect.Append("				<td class=\"EktMsgTargetTableDataSelect\">" + Environment.NewLine);
                sbSelect.Append("					<input type=\"radio\" id=\"cb_EktMsgTarget_" + m_uniqueId + "_" + m_user_list[idx].Id.ToString() + "\" name=\"cb_EktMsgTarget_" + m_uniqueId + "_\" title=\"" + DisplayName + "\" onclick=\"GetCommunityMsgObject(\'" + m_uniqueId + "\').MsgUpdateSelectedUser(\'" + m_user_list[idx].Id.ToString() + "\',\'" + DisplayName + "\',this.checked)\" />" + Environment.NewLine);
                sbSelect.Append("				</td>" + Environment.NewLine);
                sbSelect.Append("				<td class=\"EktMsgTargetTableDataAvatar\"><div class=\"EktMsgTargetTableData_AvatarContainer\">" + Environment.NewLine);
                sbSelect.Append("					<img src=\"" + ((m_user_list[idx].Avatar != "") ? (AppendSitePathIfNone((string)(m_user_list[idx].Avatar))) : m_refContentApi.AppImgPath + "who.jpg") + "\" alt=\"avatar\" title=\"avatar_title\" />" + Environment.NewLine);
                sbSelect.Append("				</div></td>" + Environment.NewLine);
                sbSelect.Append("				<td class=\"EktMsgTargetTableDataMember\">" + Environment.NewLine);
                sbSelect.Append("					" + DisplayName + "<br />(" + m_user_list[idx].FirstName + " " + m_user_list[idx].LastName + ")" + Environment.NewLine);
                sbSelect.Append("				</td>" + Environment.NewLine);
                sbSelect.Append("			</tr>" + Environment.NewLine);
            }
            sbSelect.Append("		</tbody>" + Environment.NewLine);
            sbSelect.Append("	</table>" + Environment.NewLine);
            sbSelect.Append("	</div>" + Environment.NewLine);
        }
        else
        {
            sbSelect.Append("	<span class=\"EktMsgTargetsNoResults\" >" + (m_friendsOnly ? (m_refMsg.GetMessage("friend search empty result")) : (m_refMsg.GetMessage("user search empty result"))) + "</span>" + Environment.NewLine);
            sbSelect.Append("	</div>" + Environment.NewLine);
        }
        sbSelect.Append("	<div class=\"EktMsgTargetsBtmControls\">" + Environment.NewLine);

        sbSelect.Append("<span class=\"EktMsgTargetsPagePreviousBtn\" >" + Environment.NewLine);
        if (m_recipientsPage > 1)
        {
            sbSelect.Append("		<input type=\"image\" src=\"" + GetUserAPI().AppImgPath + "but_prev.gif\" onclick=\"GetCommunityMsgObject(\'" + m_uniqueId + "\').MsgTarg_PrevPage" + "(); return false;\" />" + Environment.NewLine);
        }
        else
        {
            sbSelect.Append("		<img src=\"" + GetUserAPI().AppImgPath + "but_prev_d.gif\" />" + Environment.NewLine);
        }
        sbSelect.Append("</span>" + Environment.NewLine);

        sbSelect.Append("<span class=\"EktMsgTargetsPageNextBtn\" >" + Environment.NewLine);
        if (m_recipientsPage < m_intTotalPages)
        {
            sbSelect.Append("		<input type=\"image\" src=\"" + GetUserAPI().AppImgPath + "but_next.gif\" onclick=\"GetCommunityMsgObject(\'" + m_uniqueId + "\').MsgTarg_NextPage" + "(); return false;\" />" + Environment.NewLine);
        }
        else
        {
            sbSelect.Append("		<img src=\"" + GetUserAPI().AppImgPath + "but_next_d.gif\" />" + Environment.NewLine);
        }
        sbSelect.Append("</span>" + Environment.NewLine);

        sbSelect.Append("		<input type=\"button\" title=\"" + m_refMsg.GetMessage("btn done") + "\" value=\"" + m_refMsg.GetMessage("btn done") + "\" onclick=\"GetCommunityMsgObject(\'" + m_uniqueId + "\').MsgSaveMessageTargetUI()\" class=\"EktMsgTargetsDoneBtn\" />" + Environment.NewLine);
        sbSelect.Append("		<input type=\"button\" title=\"" + m_refMsg.GetMessage("btn cancel") + "\" value=\"" + m_refMsg.GetMessage("btn cancel") + "\" onclick=\"GetCommunityMsgObject(\'" + m_uniqueId + "\').MsgCancelMessageTargetUI()\" class=\"EktMsgTargetsCancelBtn\" />" + Environment.NewLine);
        sbSelect.Append("		<input id=\"RecipientsPage" + m_uniqueId + "\" type=\"hidden\" value=\"" + m_recipientsPage.ToString() + "\" />" + Environment.NewLine);
        sbSelect.Append("	</div>" + Environment.NewLine);

        result = sbSelect.ToString();
        sbSelect = null;
        return (result);
    }
    protected string AppendSitePathIfNone(string avatar)
    {
        if ((avatar.Trim().IndexOf(m_refContentApi.SitePath) < 0) && (("/" + avatar).Trim().IndexOf(m_refContentApi.SitePath) < 0))
        {
            avatar = m_refContentApi.SitePath + avatar;
        }
        return avatar;
    }
    protected string IsUserSelected(long id)
    {
        string result = "false";
        return (result);
    }
    #endregion

    #region ICallBackEventHandler
    public string GetCallbackResult()
    {
        return (m_callbackresult);
    }

    public void RaiseCallbackEvent(string eventArgs)
    {
        System.Collections.Specialized.NameValueCollection postBackData = null;
        string recipientsPageStr = "1";

        m_callbackresult = "";
        postBackData = new System.Collections.Specialized.NameValueCollection();
        postBackData = System.Web.HttpUtility.ParseQueryString(eventArgs);

        if (!(postBackData["__searchtext"] == null))
        {
            m_strKeyWords = postBackData["__searchtext"];
        }

        if (!(postBackData["__searchmode"] == null))
        {
            m_searchMode = postBackData["__searchmode"];
        }

        if ((!(postBackData["__targpage"] == null)) && Information.IsNumeric(postBackData["__targpage"].Trim()))
        {
            recipientsPageStr = (string)(postBackData["__targpage"].Trim());
            m_recipientsPage = System.Convert.ToInt32((Information.IsNumeric(recipientsPageStr) && (System.Convert.ToInt32(recipientsPageStr) > 0)) ? (System.Convert.ToInt32(recipientsPageStr)) : 1);
        }


        CollectSearchText();
        GetUsers();
        m_callbackresult = BuildRecipientSelect();

    }

    #endregion
}
