using System.Web.UI.WebControls;
using System.Configuration;
using System.Collections;
using System.Linq;
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
//using Ektron.Cms.Common.EkConstants;
using Ektron.Cms.Common;


public partial class editpreapproval : System.Web.UI.UserControl
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
    protected string ItemType;
    protected ContentData content_data;

    private void Page_Load(System.Object sender, System.EventArgs e)
    {
        //Put user code to initialize the page here
        m_refMsg = m_refContentApi.EkMsgRef;
        RegisterResources();
    }
    public bool EditPreApproval()
    {
        if (!(Request.QueryString["type"] == null))
        {
            ItemType = Convert.ToString(Request.QueryString["type"]).Trim().ToLower();
        }
        if (!(Request.QueryString["id"] == null))
        {
            m_intId = Convert.ToInt64(Request.QueryString["id"]);
            if (m_intId == 0)
            {
                trInerhit.Visible = false;
                hdnFolderId.Value = m_intId.ToString();
            }
        }
        if (!(Request.QueryString["action"] == null))
        {
            m_strPageAction = Convert.ToString(Request.QueryString["action"]).ToLower().Trim();
        }
        if (!(Request.QueryString["orderby"] == null))
        {
            m_strOrderBy = Convert.ToString(Request.QueryString["orderby"]);
        }
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
        if (ContentLanguage == Ektron.Cms.Common.EkConstants.CONTENT_LANGUAGES_UNDEFINED)
        {
            m_refContentApi.ContentLanguage = Ektron.Cms.Common.EkConstants.ALL_CONTENT_LANGUAGES;
        }
        else
        {
            m_refContentApi.ContentLanguage = ContentLanguage;
        }

        CurrentUserId = m_refContentApi.UserId;
        AppImgPath = m_refContentApi.AppImgPath;
        SitePath = m_refContentApi.SitePath;
        EnableMultilingual = m_refContentApi.EnableMultilingual;
        if (!(Page.IsPostBack))
        {
            Display_EditPreApprovals();
        }
        else
        {
            Process_DoEditPreApproval();
        }
        return true;
    }

    #region ACTION -UpdatePreApproval
    private void Process_DoEditPreApproval()
    {
        Ektron.Cms.Content.EkContent m_refContent;
        try
        {
            m_refContent = m_refContentApi.EkContentRef;
            pagedata = new Collection();
            pagedata.Add(m_intId, "FolderID", null, null);
            if (Request.Form["selectusergroup"] == null)
            { 
                pagedata.Add(System.Convert.ToInt32(-1), "PreApprovalGroupID", null, null); }
            else
            {
                pagedata.Add(System.Convert.ToInt32(Request.Form["selectusergroup"]), "PreApprovalGroupID", null, null);
            }
            m_refContent.UpdateFolderPreapproval(pagedata);
            Response.Redirect((string)("content.aspx?action=ViewFolder&id=" + m_intId + "&LangType=" + ContentLanguage), false);
        }
        catch (Exception ex)
        {
            Response.Redirect((string)("reterror.aspx?info=" + EkFunctions.UrlEncode(ex.Message) + "&LangType=" + ContentLanguage), false);
        }
    }
    #endregion

    #region APPROVAL - Display_EditPreApprovals
    private void Display_EditPreApprovals()
    {
        int m_intApprovalMethoad = 0;
        Microsoft.VisualBasic.Collection cPreApproval;
        Ektron.Cms.Content.EkTask taskObj;
        Microsoft.VisualBasic.Collection userGroups;
        ltInheritPreApprovalGroup.Text = m_refMsg.GetMessage("lbl inherit parent configuration");


        cPreApproval = m_refContentApi.EkContentRef.GetFolderPreapprovalGroup(m_intId);
        taskObj = m_refContentApi.EkTaskRef;
        //If Not taskObj Is Nothing Then
        userGroups = taskObj.GetUsersForTask(CurrentUserId, -1);
        //End If

        //FormAction = "content.aspx?action=DoEditApprovalMethod&id=" & m_intId & "&type=" & ItemType & "&LangType=" & m_intContentLanguage
        //SetPostBackPage()
        security_data = m_refContentApi.LoadPermissions(m_intId, ItemType, 0);

        if (ItemType == "folder")
        {
            folder_data = m_refContentApi.GetFolderById(m_intId);
            m_intApprovalMethoad = folder_data.ApprovalMethod;
        }
        else
        {
            content_data = m_refContentApi.GetContentById(m_intId, 0);
            m_intApprovalMethoad = content_data.ApprovalMethod;
        }
        EditApprovalsToolBar();

        if ("-1" == cPreApproval["PreApprovalGroupID"].ToString())
        {
            chkInheritPreApprovalGroup.Checked = true;
        }
        else 
        {
            chkInheritPreApprovalGroup.Checked = false;
            hdnInherited.Value = "false";
        }

        lit_select_preapproval.Text += "<option value=\"0\" "; 
        
        if ("0" == cPreApproval["PreApprovalGroupID"].ToString())
        {
            lit_select_preapproval.Text += " selected";
        }
        lit_select_preapproval.Text += ">";
        lit_select_preapproval.Text += "(None)</option>";

        if ("-1" == cPreApproval["PreApprovalGroupID"].ToString())
        {
            Microsoft.VisualBasic.Collection cPreApprovalParent;
            cPreApprovalParent = m_refContentApi.EkContentRef.GetFolderPreapprovalGroup(folder_data.ParentId);
            foreach (Microsoft.VisualBasic.Collection userGroup in userGroups)
            {
                if (Information.IsNumeric(userGroup["UserGroupID"]))
                {
                    lit_select_preapproval.Text += "<option value=" + userGroup["UserGroupID"];
                    if (Convert.ToInt32(userGroup["UserGroupID"]) == Convert.ToInt32(cPreApprovalParent["PreApprovalGroupID"]))
                    {
                        lit_select_preapproval.Text += " selected ";
                    }
                    lit_select_preapproval.Text += ">";
                    lit_select_preapproval.Text += userGroup["DisplayUserGroupName"] + "</option>";
                }
            }
        }
        else
        {
            foreach (Microsoft.VisualBasic.Collection userGroup in userGroups)
            {
                if (Information.IsNumeric(userGroup["UserGroupID"]))
                {
                    lit_select_preapproval.Text += "<option value=" + userGroup["UserGroupID"];
                    if (Convert.ToInt32(userGroup["UserGroupID"]) == Convert.ToInt32(cPreApproval["PreApprovalGroupID"]))
                    {
                        lit_select_preapproval.Text += " selected ";
                    }
                    lit_select_preapproval.Text += ">";
                    lit_select_preapproval.Text += userGroup["DisplayUserGroupName"] + "</option>";
                }
            }
        }
    }

    private void EditApprovalsToolBar()
    {
        System.Text.StringBuilder result = new System.Text.StringBuilder();
        string WorkareaTitlebarTitle = "";
        WorkareaTitlebarTitle = m_refMsg.GetMessage("edit properties for folder msg");
        txtTitleBar.InnerHtml = m_refStyle.GetTitleBar(WorkareaTitlebarTitle) + " \"" + folder_data.Name + "\"";
        result.Append("<table><tr>");
        result.Append(m_refStyle.GetButtonEventsWCaption(m_refContentApi.AppPath + "images/UI/Icons/back.png", (string)("content.aspx?action=ViewFolder&id=" + m_intId + "&LangType=" + ContentLanguage), m_refMsg.GetMessage("alt back button text"), m_refMsg.GetMessage("btn back"), "", StyleHelper.BackButtonCssClass,true));
        result.Append(m_refStyle.GetButtonEventsWCaption(m_refContentApi.AppPath + "images/UI/Icons/save.png", "#", m_refMsg.GetMessage("alt update button text (folder)"), m_refMsg.GetMessage("btn update"), "Onclick=\"javascript:return SubmitForm(\'editfolder\', \'CheckPreApprovalSettings()\');\"", StyleHelper.SaveButtonCssClass,true));
		result.Append(StyleHelper.ActionBarDivider);
        result.Append("<td>");
        result.Append(m_refStyle.GetHelpButton("EditPreApproval", ""));
        result.Append("</td>");
        result.Append("</tr></table>");
        htmToolBar.InnerHtml = result.ToString();
    }
    #endregion
    private void RegisterResources()
    {
        Ektron.Cms.API.JS.RegisterJS(this, Ektron.Cms.API.JS.ManagedScript.EktronWorkareaHelperJS);
    }
}
