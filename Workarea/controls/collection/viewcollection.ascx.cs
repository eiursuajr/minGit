using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Ektron.Cms.UI.CommonUI;
using Microsoft.VisualBasic;
using Ektron.Cms;
using Ektron.Cms.Common;
using System.Data;

public partial class Workarea_controls_collection_viewcollection : System.Web.UI.UserControl
{
    const Int64 ALL_CONTENT_LANGUAGES = -1;
    protected EkMessageHelper MsgHelper;
    private PermissionData _PermissionData;
    long _folderId = 0;
    string bpage = string.Empty;
    protected bool reloadTree = false;
    protected bool report = false;
    protected void Page_Load(object sender, EventArgs e)
    {
        MsgHelper = new EkMessageHelper(ContentApi.RequestInformationRef);
        StyleHelper m_refStyle = new StyleHelper();
        Collection gtNavs = new Collection();
        string action = "";
        string checkout = "";
        string ErrorString = "";
        string CollectionTitle = "";
        ApplicationAPI AppUI = new ApplicationAPI();
        Int32 ContentLanguage = 0;
        if (Request.QueryString["LangType"] != null)
        {
            if (!string.IsNullOrEmpty(Request.QueryString["LangType"]))
            {
                ContentLanguage = Convert.ToInt32(Request.QueryString["LangType"]);
                if (ContentLanguage == 0)
                    ContentLanguage = AppUI.DefaultContentLanguage;
                ContentApi.SetCookieValue("LastValidLanguageID", ContentLanguage.ToString());
            }
            else
            {
                if (!string.IsNullOrEmpty(ContentApi.GetCookieValue("LastValidLanguageID")))
                {
                    ContentLanguage = Convert.ToInt32(ContentApi.GetCookieValue("LastValidLanguageID"));
                }
            }
        }
        else
        {
            if (!string.IsNullOrEmpty(ContentApi.GetCookieValue("LastValidLanguageID")))
            {
                ContentLanguage = Convert.ToInt32(ContentApi.GetCookieValue("LastValidLanguageID"));
                if (ContentLanguage == 0)
                {
                    ContentLanguage = AppUI.DefaultContentLanguage;
                    ContentApi.SetCookieValue("LastValidLanguageID", ContentLanguage.ToString());
                }
            }
        }
        AppUI.FilterByLanguage = ContentLanguage;
        long nId = Convert.ToInt64(Request.QueryString["nid"]);
        if (!string.IsNullOrEmpty(Request.QueryString["action"]))
            action = Request.QueryString["action"];
        if (!string.IsNullOrEmpty(Request.QueryString["bpage"]) | !string.IsNullOrEmpty(Request.QueryString["treeViewId"]))
            bpage = "&bpage=reports";
        if (action.ToString().ToLower() == "viewstage")
            gtNavs = AppUI.EkContentRef.GetEcmStageCollectionByID(nId, false, false, ref ErrorString, true, false, false);
        else
        {
            gtNavs = AppUI.EkContentRef.GetEcmCollectionByID(nId, false, false, ref ErrorString, true, false, false);
            checkout = "";
        }
        if (Convert.ToBoolean(gtNavs["ApprovalRequired"]) == true && (((string)gtNavs["Status"] == "A") || ((string)gtNavs["Status"] == "S")))
            checkout = "&checkout=true";
        if (Convert.ToBoolean(gtNavs["ApprovalRequired"]) == true && (string)gtNavs["Status"] == "O")
            checkout = checkout + "&status=o";
        if (string.IsNullOrEmpty(Request.QueryString["folderid"]))
        {
            _folderId = Convert.ToInt64(gtNavs["FolderID"]);
            report = true;
        }
        else
        {
            _folderId = Convert.ToInt64(Request.QueryString["folderid"]);          
        }
        if (ErrorString == "")
        {
            if (gtNavs.Count > 0)
                CollectionTitle = gtNavs["CollectionTitle"].ToString();
        }
        if (ErrorString != "")
        {
            divError.InnerHtml = ErrorString;
        }

        // Toolbar
        if (Request.QueryString["bpage"] == "reports" || _folderId == 0 || report)
            litButtons.Text = m_refStyle.GetButtonEventsWCaption(AppUI.AppPath + "images/UI/Icons/back.png", "collections.aspx?action=ViewCollectionReport", MsgHelper.GetMessage("alt back button text"), MsgHelper.GetMessage("btn back"), "", StyleHelper.BackButtonCssClass, true);
        else
            litButtons.Text = m_refStyle.GetButtonEventsWCaption(AppUI.AppPath + "images/UI/Icons/back.png", "collections.aspx?folderid=" + _folderId, MsgHelper.GetMessage("alt back button text"), MsgHelper.GetMessage("btn back"), "", StyleHelper.BackButtonCssClass, true);

        bool primaryStyleApplied = false;

        if (action.ToString().ToLower() == "viewstage")
        {
            litButtons.Text += m_refStyle.GetButtonEventsWCaption(AppUI.AppPath + "images/UI/Icons/contentViewPublished.png", "collections.aspx?action=View&LangType=" + ContentLanguage + "&nid=" + nId + "&folderid=" + _folderId, MsgHelper.GetMessage("btn view publish"), MsgHelper.GetMessage("btn view publish"), "", StyleHelper.ViewPublishedButtonCssClass, !primaryStyleApplied);

            primaryStyleApplied = true;

            if ((string)gtNavs["Status"] == "S")
            {
                if (IsCollectionApprover)
                {
                    litButtons.Text += m_refStyle.GetButtonEventsWCaption(AppUI.AppPath + "images/UI/Icons/contentPublish.png", "collections.aspx?action=doPublishCol&LangType=" + ContentLanguage + "&nid=" + nId + "&folderid=" + _folderId, MsgHelper.GetMessage("generic Publish"), MsgHelper.GetMessage("generic Publish"), "");
                    litButtons.Text += m_refStyle.GetButtonEventsWCaption(AppUI.AppPath + "images/ui/icons/approvalDenyItem.png", "collections.aspx?action=doDeclineApprCol&LangType=" + ContentLanguage + "&nid=" + nId + "&folderid=" + _folderId, MsgHelper.GetMessage("lbl decline"), MsgHelper.GetMessage("lbl decline"), "");

                }
            }
            else if ((string)gtNavs["Status"] == "M")
            {
                if (CanDoCollections)
                {
                    litButtons.Text += m_refStyle.GetButtonEventsWCaption(AppUI.AppPath + "images/UI/Icons/contentPublish.png", "collections.aspx?action=doDelete&nId=" + nId + "&folderid=" + _folderId + checkout + bpage, MsgHelper.GetMessage("alt: delete collection text"), MsgHelper.GetMessage("btn delete"), "onclick=\"return ConfirmNavDelete();\"");
                    litButtons.Text += m_refStyle.GetButtonEventsWCaption(AppUI.AppPath + "images/ui/icons/approvalDenyItem.png", "collections.aspx?action=doDeclineDelCol&LangType=" + ContentLanguage + "&nid=" + nId + "&folderid=" + _folderId, MsgHelper.GetMessage("lbl decline"), MsgHelper.GetMessage("lbl decline"), "");
                }
            }
            else
            {
                if (IsCollectionApprover)
                    litButtons.Text += m_refStyle.GetButtonEventsWCaption(AppUI.AppPath + "images/UI/Icons/contentPublish.png", "collections.aspx?action=doPublishCol&LangType=" + ContentLanguage + "&nid=" + nId + "&folderid=" + _folderId, MsgHelper.GetMessage("generic Publish"), MsgHelper.GetMessage("generic Publish"), "");
                else
                    litButtons.Text += m_refStyle.GetButtonEventsWCaption(AppUI.AppPath + "images/UI/Icons/approvalSubmitFor.png", "collections.aspx?action=doSubmitCol&LangType=" + ContentLanguage + "&nid=" + nId + "&folderid=" + _folderId, MsgHelper.GetMessage("generic Submit"), MsgHelper.GetMessage("generic Submit"), "");
                litButtons.Text += m_refStyle.GetButtonEventsWCaption(AppUI.AppPath + "images/UI/Icons/contentRestore.png", "collections.aspx?action=doUndoCheckoutCol&LangType=" + ContentLanguage + "&nid=" + nId + "&folderid=" + _folderId, MsgHelper.GetMessage("generic Undocheckout"), MsgHelper.GetMessage("generic Undocheckout"), "");
            }
        }
        else
        {
            if ((string)gtNavs["Status"] != "A")
            {
                litButtons.Text += m_refStyle.GetButtonEventsWCaption(AppUI.AppPath + "images/UI/Icons/preview.png", "collections.aspx?action=ViewStage&LangType=" + ContentLanguage + "&nid=" + nId + "&folderid=" + _folderId, MsgHelper.GetMessage("btn view stage"), MsgHelper.GetMessage("btn view stage"), "", StyleHelper.ViewStagedButtonCssClass, !primaryStyleApplied);

                primaryStyleApplied = true;
            }
        }
        string enableQDOparam = null;

        if (gtNavs["EnableReplication"].ToString().Length == 1)
            enableQDOparam = "&qdo=1";

        litButtons.Text += m_refStyle.GetButtonEventsWCaption(AppUI.AppPath + "images/UI/Icons/contentAdd.png", "collections.aspx?LangType=" + ContentLanguage + "&action=AddLink&nid=" + nId + "&folderid=" + _folderId + checkout + enableQDOparam, MsgHelper.GetMessage("alt: add collection items text"), MsgHelper.GetMessage("add collection items"), "", StyleHelper.AddButtonCssClass, !primaryStyleApplied);

        primaryStyleApplied = true;

        if (((Collection)gtNavs["Contents"]).Count > 0)
        {
            litButtons.Text += m_refStyle.GetButtonEventsWCaption(AppUI.AppPath + "images/UI/Icons/remove.png", "collections.aspx?LangType=" + ContentLanguage + "&action=DeleteLink&nid=" + nId + "&folderid=" + _folderId + checkout, MsgHelper.GetMessage("alt: remove collection items text"), MsgHelper.GetMessage("remove collection items"), "", StyleHelper.RemoveButtonCssClass);
            litButtons.Text += m_refStyle.GetButtonEventsWCaption(AppUI.AppPath + "images/UI/Icons/arrowUpDown.png", "collections.aspx?LangType=" + ContentLanguage + "&action=ReOrderLinks&nid=" + nId + "&folderid=" + _folderId + checkout, MsgHelper.GetMessage("alt: reorder collection text"), MsgHelper.GetMessage("btn reorder"), "", StyleHelper.ReOrderButtonCssClass);
        }

        if ((string)gtNavs["Status"] != "M")
        {
            if (CanDoCollections)
            {
                if (string.IsNullOrEmpty(bpage) && !string.IsNullOrEmpty(Request.QueryString["folderid"]) && Request.QueryString["folderid"] == "0")
                {
                    bpage = "&bpage=reports";
                }

                litButtons.Text += m_refStyle.GetButtonEventsWCaption(AppUI.AppPath + "images/UI/Icons/delete.png", "collections.aspx?action=doDelete&nId=" + nId + "&folderid=" + _folderId + checkout + bpage, MsgHelper.GetMessage("alt: delete collection text"), MsgHelper.GetMessage("btn delete"), "onclick=\"return ConfirmNavDelete();\"", StyleHelper.DeleteButtonCssClass);

            }
            else
                litButtons.Text += m_refStyle.GetButtonEventsWCaption(AppUI.AppPath + "images/UI/Icons/delete.png", "collections.aspx?action=doSubmitDelCol&nId=" + nId + "&folderid=" + _folderId + checkout, MsgHelper.GetMessage("alt: delete collection text"), MsgHelper.GetMessage("btn delete"), "onclick=\"return ConfirmNavDelete();\"", StyleHelper.DeleteButtonCssClass);
        }

        litButtons.Text += StyleHelper.ActionBarDivider;

        if (action == "ViewStage")
            litButtons.Text += m_refStyle.GetButtonEventsWCaption(AppUI.AppPath + "images/UI/Icons/properties.png", "collections.aspx?LangType=" + ContentLanguage + "&action=ViewStageAttributes&nid=" + nId + "&folderid=" + _folderId + checkout + enableQDOparam, MsgHelper.GetMessage("alt collection properties button text"), MsgHelper.GetMessage("properties text"), "", StyleHelper.ViewPropertiesButtonCssClass);
        else
            litButtons.Text += m_refStyle.GetButtonEventsWCaption(AppUI.AppPath + "images/UI/Icons/properties.png", "collections.aspx?LangType=" + ContentLanguage + "&action=ViewAttributes&nid=" + nId + "&folderid=" + _folderId + enableQDOparam, MsgHelper.GetMessage("alt collection properties button text"), MsgHelper.GetMessage("properties text"), "", StyleHelper.ViewPropertiesButtonCssClass);

        if (((string)gtNavs["Status"] == "A") && (Convert.ToInt32(gtNavs["EnableReplication"]) == 1))
        {
            litButtons.Text += StyleHelper.ActionBarDivider;
            litButtons.Text += m_refStyle.GetButtonEventsWCaption(AppUI.AppPath + "images/UI/Icons/translation.png", "DynReplication.aspx?collid=" + nId, MsgHelper.GetMessage("alt quickdeploy collection button text"), MsgHelper.GetMessage("alt quickdeploy collection button text"), "", StyleHelper.TranslationButtonCssClass);
        }

        litViewCollection.Text = m_refStyle.GetTitleBar(MsgHelper.GetMessage("view collection title") + " \"" + CollectionTitle + "\"");

        litCollItems.Text = StyleHelper.ActionBarDivider + "<td>" + m_refStyle.GetHelpButton("ViewCollectionItems", "") + "</td>";

        string genericView = MsgHelper.GetMessage("generic View");
        Collection gtLinks = (Collection)gtNavs["Contents"];

        DataTable dtCollection = new DataTable();
        dtCollection.Columns.Add("HTML");
        dtCollection.Columns.Add("ContentLanguage");
        dtCollection.Columns.Add("ContentID");
        dtCollection.Columns.Add("ContentLinks");

        foreach (Collection gtLink in gtLinks)
        {
            DataRow dRow = dtCollection.NewRow();
            String backPage = "Action=View&nid=" + nId + "&folderid=" + _folderId;
            String contentUrl = "content.aspx?action=View&LangType=" + gtLink["ContentLanguage"].ToString() + "&id=" + gtLink["ContentID"] + "&callerpage=collections.aspx&origurl=" + EkFunctions.UrlEncode(backPage);
            String contentTitle = genericView + " " + gtLink["ContentTitle"].ToString().Replace("'", "`");
            String iconurl = "";
            try
            {
                iconurl = gtLink["ImageUrl"].ToString();
            }
            catch
            {
                // ignore errors if we try getting imageurl on regular content
            }
            ContentApi.ContentLanguage = ContentLanguage;
            string dmsmenuhtml = ContentApi.GetDmsContextMenuHTML(Convert.ToInt64(gtLink["ContentID"]), Convert.ToInt64(gtLink["ContentLanguage"]), Convert.ToInt64(gtLink["ContentType"]), Convert.ToInt64(gtLink["ContentSubtype"]), gtLink["ContentTitle"].ToString(), MsgHelper.GetMessage("generic Title") + " " + gtLink["ContentTitle"], contentUrl, "", iconurl);

            dRow["HTML"] = dmsmenuhtml;
            dRow["ContentLanguage"] = gtLink["ContentLanguage"].ToString();
            dRow["ContentID"] = gtLink["ContentID"].ToString();
            dRow["ContentLinks"] = gtLink["ContentLinks"].ToString();
            dtCollection.Rows.Add(dRow);
        }
        rptColl.DataSource = dtCollection;
        rptColl.DataBind();

        if (ContentApi.EnableMultilingual == 1)
        {
            litEnableMult.Text = StyleHelper.ActionBarDivider;
            litViewLang.Text = MsgHelper.GetMessage("view language") + LangDD(false, "");
        }
        gtNavs = null;

        if ((Request.QueryString["rf"] != null && Request.QueryString["rf"] == "1") || (Request.QueryString["reloadtrees"] != null && Request.QueryString["reloadtrees"] == "coll"))
        {            
            ReloadClientScript("");           
            litRefreshCollAccordion.Text = "<script language=\"javascript\">" + ("\r\n" + "top.refreshCollectionAccordion(") + ContentLanguage + ");" + ("\r\n" + "</script>") + "\r\n";
        }
    }
    public string LangDD(object showAllOpt, string bgColor)
    {
        string myTemp = Request.ServerVariables["PATH_INFO"].Substring(Request.ServerVariables["PATH_INFO"].LastIndexOf("/") + 1) + "?" + Ektron.Cms.Common.EkFunctions.HtmlEncode(Request.ServerVariables["QUERY_STRING"].ToString().Replace("LangType", "L"));
        myTemp = myTemp.Replace("&amp;", "&");

        StyleHelper m_refStyle = new StyleHelper();
        string returnValue;
        returnValue = m_refStyle.ShowAllActiveLanguage(Convert.ToBoolean(showAllOpt), bgColor, "javascript:SelLanguage(this.value, '" + myTemp + "');", "");
        return returnValue;
    }

    private PermissionData Permissiondata
    {
        get { return (_PermissionData ?? (_PermissionData = ContentApi.LoadPermissions(_folderId, "folder", 0))); }
    }

    private bool? _isFolderAdmin;
    private bool IsFolderAdmin
    {
        get { return (_isFolderAdmin ?? (bool)(_isFolderAdmin = ContentApi.IsARoleMemberForFolder_FolderUserAdmin(_folderId, 0, false))); }
    }

    private bool? _isCollectionApprover;
    public bool IsCollectionApprover
    {
        get
        {
            return (_isCollectionApprover ?? (bool)(_isCollectionApprover = (Permissiondata.IsAdmin
                || IsFolderAdmin
                || ContentApi.IsARoleMember(Ektron.Cms.Common.EkEnumeration.CmsRoleIds.CollectionApprovers))));
        }
    }

    private bool? _canDoCollections;
    private bool CanDoCollections
    {
        get
        {
            return (_canDoCollections ?? (bool)(_canDoCollections = (Permissiondata.IsAdmin
            || Permissiondata.IsCollections
            || ContentApi.IsARoleMember(Ektron.Cms.Common.EkEnumeration.CmsRoleIds.AminCollectionMenu)
            || ContentApi.IsARoleMember(Ektron.Cms.Common.EkEnumeration.CmsRoleIds.AdminCollection)
            || ContentApi.IsARoleMember(Ektron.Cms.Common.EkEnumeration.CmsRoleIds.CollectionApprovers))));
        }
    }

    private ContentAPI _refContentApi;
    private ContentAPI ContentApi
    {
        get { return (_refContentApi ?? (_refContentApi = new ContentAPI())); }
    }
    private void ReloadClientScript(string idPath)
    {
        System.Text.StringBuilder result = new System.Text.StringBuilder();
        try
        {
            idPath = "/" + idPath + "/";
            result.Append("top.TreeNavigation(\"CollTree\", \"" + idPath + "\");" + "\r\n");
            Ektron.Cms.API.JS.RegisterJSBlock(this.Parent.Parent.Page.Header, result.ToString(), "ReloadClientScript");
        }
        catch (Exception)
        {
        }
    }
}