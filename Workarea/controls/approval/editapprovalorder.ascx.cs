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

public partial class editapprovalorder : System.Web.UI.UserControl
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
    protected string ItemType = "";
    protected ContentData content_data;

    private void Page_Load(System.Object sender, System.EventArgs e)
    {
        //Put user code to initialize the page here
        m_refMsg = m_refContentApi.EkMsgRef;
        RegisterResources();
    }
    public bool EditApprovalOrder()
    {
        if (!(Request.QueryString["type"] == null))
        {
            ItemType = Convert.ToString(Request.QueryString["type"]).Trim().ToLower();
        }
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
        //If (Not (Request.QueryString("membership") Is Nothing)) Then
        //    If (Request.QueryString("membership").Trim.ToLower <> "") Then
        //        m_bMemberShip = Convert.ToBoolean(Request.QueryString("membership").Trim.ToLower)
        //    End If
        //End If
        //If (Not (Request.QueryString("base") Is Nothing)) Then
        //    m_strBase = Request.QueryString("base").Trim.ToLower
        //End If
        CurrentUserId = m_refContentApi.UserId;
        AppImgPath = m_refContentApi.AppImgPath;
        SitePath = m_refContentApi.SitePath;
        EnableMultilingual = m_refContentApi.EnableMultilingual;
        if (!(Page.IsPostBack))
        {
            Display_EditApprovalOrder();
        }
        else
        {
            Process_DoUpdateApprovalOrder();
        }
        return true;
    }
    #region ACTION - UpdateApprovalOrder
    private void Process_DoUpdateApprovalOrder()
    {
        Ektron.Cms.Content.EkContent m_refContent;
        try
        {
            m_refContent = m_refContentApi.EkContentRef;
            pagedata = new Collection();
            if (Request.QueryString["type"] == "folder")
            {
                pagedata.Add(Request.QueryString["id"], "FolderID", null, null);
                pagedata.Add("", "ContentID", null, null);
            }
            else
            {
                pagedata.Add(Request.QueryString["id"], "ContentID", null, null);
                pagedata.Add("", "FolderID", null, null);
            }
            pagedata.Add(Request.Form[ApprovalOrder.UniqueID], "ApprovalOrder", null, null);

            m_refContent.UpdateApprovalOrderv2_0(pagedata);

            Response.Redirect((string)("content.aspx?LangType=" + ContentLanguage + "&action=ViewApprovals&id=" + Request.QueryString["id"] + "&type=" + Request.QueryString["type"]), false);

        }
        catch (Exception ex)
        {
            Response.Redirect((string)("reterror.aspx?info=" + EkFunctions.UrlEncode(ex.Message)), false);
        }
    }
    #endregion
    #region APPROVAL - EditApprovalOrder
    private void Display_EditApprovalOrder()
    {
        bool bFolderUserAdmin = false;
        //FormAction = "content.aspx?LangType=" & m_intContentLanguage & "&action=DoUpdateApprovalOrder&id=" & m_intId & "&type=" & ItemType
        //SetPostBackPage()
        if (ItemType == "folder")
        {
            folder_data = m_refContentApi.GetFolderById(m_intId);
        }
        else
        {
            content_data = m_refContentApi.GetContentById(m_intId, 0);
        }
        ApprovalItemData[] approval_data;
        approval_data = m_refContentApi.GetItemApprovals(m_intId, ItemType);
        security_data = m_refContentApi.LoadPermissions(m_intId, ItemType, 0);
        if (!(folder_data == null))
        {
            bFolderUserAdmin = security_data.IsAdmin || m_refContentApi.IsARoleMemberForFolder_FolderUserAdmin(folder_data.Id, 0, false);
        }
        else
        {
            if (!(content_data == null))
            {
                bFolderUserAdmin = security_data.IsAdmin || m_refContentApi.IsARoleMemberForFolder_FolderUserAdmin(content_data.FolderId, 0, false);
            }
            else
            {
                bFolderUserAdmin = security_data.IsAdmin;
            }
        }
        if (!(security_data.IsAdmin || bFolderUserAdmin))
        {
            throw (new Exception(m_refMsg.GetMessage("error: user not permitted")));
        }
        EditApprovalOrderToolbar();
        string strMsg = "";
        int i = 0;
        if (!(approval_data == null))
        {
            if (approval_data.Length < 20)
            {
                ApprovalList.Rows = approval_data.Length;
            }
            for (i = 0; i <= approval_data.Length - 1; i++)
            {
                if (approval_data[i].UserId > 0)
                {
                    ApprovalList.Items.Add(new ListItem(approval_data[i].DisplayUserName, "user." + approval_data[i].UserId));
                    if (strMsg.Length == 0)
                    {
                        strMsg = "user." + approval_data[i].UserId;
                    }
                    else
                    {
                        strMsg += ",user." + approval_data[i].UserId;
                    }
                }
                else
                {
                    ApprovalList.Items.Add(new ListItem(approval_data[i].DisplayUserGroupName, "group." + approval_data[i].GroupId));
                    if (strMsg.Length == 0)
                    {
                        strMsg = "group." + approval_data[i].GroupId;
                    }
                    else
                    {
                        strMsg += ",group." + approval_data[i].GroupId;
                    }
                }
            }
        }
        td_eao_link.InnerHtml = "<a href=\"javascript:Move(\'up\', document.forms[0]." + UniqueID + "_ApprovalList, document.forms[0]." + UniqueID + "_ApprovalOrder)\">";
        td_eao_link.InnerHtml += "<img src=\"" + m_refContentApi.AppPath + "Images/ui/icons/arrowHeadUp.png\" valign=middle border=0 width=16 height=16 alt=\"" + m_refMsg.GetMessage("move selection up msg") + "\" title=\"" + m_refMsg.GetMessage("move selection up msg") + "\"></a><br />";
        td_eao_link.InnerHtml += "<a href=\"javascript:Move(\'dn\', document.forms[0]." + UniqueID + "_ApprovalList, document.forms[0]." + UniqueID + "_ApprovalOrder)\">";
        td_eao_link.InnerHtml += "<img src=\"" + m_refContentApi.AppPath + "Images/ui/icons/arrowHeadDown.png\" valign=middle border=0 width=16 height=16 alt=\"" + m_refMsg.GetMessage("move selection down msg") + "\" title=\"" + m_refMsg.GetMessage("move selection down msg") + "\"></a>";

        td_eao_title.InnerHtml = m_refMsg.GetMessage("move within approvals msg");
        td_eao_msg.InnerHtml = "<label class=\"label\">" + m_refMsg.GetMessage("first approver msg") + "</label>";
        td_eao_ordertitle.InnerHtml = "<h2>" + m_refMsg.GetMessage("approval order title") + "</h2>";
        ApprovalOrder.Value = strMsg;
    }
    private void EditApprovalOrderToolbar()
    {
        System.Text.StringBuilder result = new System.Text.StringBuilder();
        string WorkareaTitlebarTitle = "";
        if (ItemType == "folder")
        {
            WorkareaTitlebarTitle = (string)(m_refMsg.GetMessage("edit folder approvals msg") + " \"" + folder_data.Name + "\"");
        }
        else
        {
            WorkareaTitlebarTitle = (string)(m_refMsg.GetMessage("edit content approvals msg") + " \"" + content_data.Title + "\"");
        }
        txtTitleBar.InnerHtml = m_refStyle.GetTitleBar(WorkareaTitlebarTitle);
        result.Append("<table><tr>");
        result.Append(m_refStyle.GetButtonEventsWCaption(m_refContentApi.AppPath + "images/UI/Icons/back.png", (string)("content.aspx?LangType=" + ContentLanguage + "&action=ViewApprovals&id=" + m_intId + "&type=" + ItemType), m_refMsg.GetMessage("alt back button text"), m_refMsg.GetMessage("btn back"), "", StyleHelper.BackButtonCssClass,true));
        result.Append("<td>" + m_refStyle.GetButtonEventsWCaption(m_refContentApi.AppPath + "images/UI/Icons/save.png", "#", m_refMsg.GetMessage("alt update button text (approvals)"), m_refMsg.GetMessage("btn update"), "Onclick=\"javascript:return SubmitForm(\'frmContent\', \'\');\"", StyleHelper.SaveButtonCssClass,true));
        result.Append("</td>");
        result.Append(StyleHelper.ActionBarDivider);
		result.Append("<td>");
        result.Append(m_refStyle.GetHelpButton(m_strPageAction, ""));
        result.Append("</td>");
        result.Append("</tr></table>");
        htmToolBar.InnerHtml = result.ToString();

    }

    private void RegisterResources()
    {
        Ektron.Cms.API.JS.RegisterJS(this, Ektron.Cms.API.JS.ManagedScript.EktronWorkareaHelperJS);
    }
    #endregion
}
