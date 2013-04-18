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
using Ektron.Cms.Common;
//using Ektron.Cms.Common.EkConstants;

public partial class viewapprovals : System.Web.UI.UserControl
{
    protected bool IsApprovalChainExists = false;
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
    public bool ViewApproval()
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
        jsType.Text = ItemType;
        jsId.Text = m_intId.ToString();
        jsAction.Text = m_strPageAction;
        CurrentUserId = m_refContentApi.UserId;
        AppImgPath = m_refContentApi.AppImgPath;
        SitePath = m_refContentApi.SitePath;
        EnableMultilingual = m_refContentApi.EnableMultilingual;

        Display_ViewApprovals();
        return false;

    }
    #region APPROVAL - ViewApprovals
    private void Display_ViewApprovals()
    {
        ApprovalItemData[] approval_data;
        security_data = m_refContentApi.LoadPermissions(m_intId, ItemType, 0);

        bool IsInherited = false;
        int intApprovalMethod = 0;
        if (ItemType == "folder")
        {
            folder_data = m_refContentApi.GetFolderById(m_intId);
            intApprovalMethod = folder_data.ApprovalMethod;
            IsInherited = folder_data.Inherited;
        }
        else
        {
            content_data = m_refContentApi.GetContentById(m_intId, 0);
            intApprovalMethod = content_data.ApprovalMethod;
            IsInherited = content_data.IsInherited;
        }

        approval_data = m_refContentApi.GetItemApprovals(m_intId, ItemType);
        if (approval_data != null)
        {
            if (approval_data.Length > 0)
            {
                IsApprovalChainExists = true;
            }
        }
        ViewApprovalToolBar();
        if (IsInherited)
        {
            lblInherited.Text = m_refMsg.GetMessage("approval chain inherited msg");
        }
        else
        {
            pnlInherited.Visible = false;
        }

        if (intApprovalMethod == 1)
        {
            lblMethod.Text = m_refMsg.GetMessage("display for force all approvers");
        }
        else
        {
            lblMethod.Text = m_refMsg.GetMessage("display for do not force all approvers");
        }
        Populate_ViewApprovalsGrid(approval_data);

    }
    private void Populate_ViewApprovalsGrid(ApprovalItemData[] approval_data)
    {
        System.Web.UI.WebControls.BoundColumn colBound = new System.Web.UI.WebControls.BoundColumn();
        colBound.DataField = "TITLE";
        colBound.HeaderText = m_refMsg.GetMessage("user or group name title");
        colBound.ItemStyle.Wrap = false;
        colBound.ItemStyle.VerticalAlign = VerticalAlign.Top;
        ViewApprovalsGrid.Columns.Add(colBound);

        colBound = new System.Web.UI.WebControls.BoundColumn();
        colBound.DataField = "ID";
        colBound.HeaderText = m_refMsg.GetMessage("generic ID");
        colBound.ItemStyle.VerticalAlign = VerticalAlign.Top;
        colBound.ItemStyle.Wrap = false;
        ViewApprovalsGrid.Columns.Add(colBound);

        colBound = new System.Web.UI.WebControls.BoundColumn();
        colBound.DataField = "ORDER";
        colBound.HeaderText = m_refMsg.GetMessage("approval order title");
        colBound.ItemStyle.VerticalAlign = VerticalAlign.Top;
        colBound.ItemStyle.Wrap = false;
        ViewApprovalsGrid.Columns.Add(colBound);

        DataTable dt = new DataTable();
        DataRow dr;

        dt.Columns.Add(new DataColumn("TITLE", typeof(string)));
        dt.Columns.Add(new DataColumn("ID", typeof(string)));
        dt.Columns.Add(new DataColumn("ORDER", typeof(string)));


        bool bInherited = false;
        if (ItemType == "folder")
        {
            bInherited = folder_data.Inherited;
        }
        else
        {
            bInherited = content_data.IsInherited;
        }
        int i;
        if (!(approval_data == null))
        {
            for (i = 0; i <= approval_data.Length - 1; i++)
            {
                dr = dt.NewRow();
                if (approval_data[i].UserId != 0)
                {
                    dr[0] = "<img class=\"imgUsers\" src=\"" + m_refContentApi.AppPath + "images/UI/Icons/user.png\" />" + approval_data[i].DisplayUserName;
                    dr[1] = approval_data[i].UserId;
                }
                else
                {
                    dr[0] = "<img class=\"imgUsers\" src=\"" + m_refContentApi.AppPath + "images/UI/Icons/users.png\" />" + approval_data[i].DisplayUserGroupName;
                    dr[1] = approval_data[i].GroupId;
                }
                dr[2] = approval_data[i].ApprovalOrder;

                dt.Rows.Add(dr);
            }
        }

        DataView dv = new DataView(dt);
        ViewApprovalsGrid.DataSource = dv;
        ViewApprovalsGrid.DataBind();
    }
    private void ViewApprovalToolBar()
    {
        System.Text.StringBuilder result = new System.Text.StringBuilder();
        bool bInherited = false;
        string WorkareaTitlebarTitle = "";
        bool bFolderUserAdmin;

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
        if (ItemType == "folder")
        {
            bInherited = folder_data.Inherited;
        }
        else
        {
            bInherited = content_data.IsInherited;
        }
        if (ItemType == "folder")
        {
            WorkareaTitlebarTitle = (string)(m_refMsg.GetMessage("view folder approvals msg") + " \"" + folder_data.Name + "\"");
        }
        else
        {
            WorkareaTitlebarTitle = (string)(m_refMsg.GetMessage("view content approvals msg") + " \"" + content_data.Title + "\"");
        }
        txtTitleBar.InnerHtml = m_refStyle.GetTitleBar(WorkareaTitlebarTitle);
        result.Append("<table><tr>");

		if (ItemType == "folder")
		{
			result.Append(m_refStyle.GetButtonEventsWCaption(m_refContentApi.AppPath + "images/UI/Icons/back.png", (string)("content.aspx?action=ViewFolder&id=" + m_intId + "&LangType=" + ContentLanguage), m_refMsg.GetMessage("alt back button text"), m_refMsg.GetMessage("btn back"), "", StyleHelper.BackButtonCssClass, true));
		}
		else
		{
			result.Append(m_refStyle.GetButtonEventsWCaption(m_refContentApi.AppPath + "images/UI/Icons/back.png", (string)("content.aspx?LangType=" + ContentLanguage + "&action=View&id=" + m_intId), m_refMsg.GetMessage("alt back button text"), m_refMsg.GetMessage("btn back"), "", StyleHelper.BackButtonCssClass, true));
		}

        if (bFolderUserAdmin && (bInherited == false))
        {
			result.Append(m_refStyle.GetButtonEventsWCaption(m_refContentApi.AppPath + "images/UI/Icons/add.png", (string)("content.aspx?LangType=" + ContentLanguage + "&action=AddApproval&id=" + m_intId + "&type=" + ItemType), m_refMsg.GetMessage("alt add button text (approvals)"), m_refMsg.GetMessage("btn add"), "", StyleHelper.AddButtonCssClass, true));
            if (IsApprovalChainExists)
            {
				result.Append(m_refStyle.GetButtonEventsWCaption(m_refContentApi.AppPath + "images/UI/Icons/delete.png", (string)("content.aspx?LangType=" + ContentLanguage + "&action=DeleteApproval&id=" + m_intId + "&type=" + ItemType), m_refMsg.GetMessage("alt delete button text (approvals)"), m_refMsg.GetMessage("btn delete"), "", StyleHelper.DeleteButtonCssClass));
				result.Append(m_refStyle.GetButtonEventsWCaption(m_refContentApi.AppPath + "images/UI/Icons/arrowUpDown.png", (string)("content.aspx?LangType=" + ContentLanguage + "&action=EditApprovalOrder&id=" + m_intId + "&type=" + ItemType), m_refMsg.GetMessage("alt edit button text (approvals)"), m_refMsg.GetMessage("btn reorder"), "", StyleHelper.ReOrderButtonCssClass));
            }
			result.Append(m_refStyle.GetButtonEventsWCaption(m_refContentApi.AppPath + "images/UI/Icons/contentEdit.png", (string)("content.aspx?LangType=" + ContentLanguage + "&action=EditApprovalMethod&id=" + m_intId + "&type=" + ItemType), "Edit Approval Method", m_refMsg.GetMessage("btn edit"), "", StyleHelper.EditButtonCssClass));
        }
        
        if (EnableMultilingual == 1)
        {
            SiteAPI m_refsite;
            m_refsite = new SiteAPI();
            LanguageData[] language_data;
            language_data = m_refsite.GetAllActiveLanguages();
            int count = 0;
            if (ItemType == "folder")
            {
                result.Append("<td class=\"label\"> | " + m_refMsg.GetMessage("content language label") + ":&nbsp;");
                result.Append("<select id=\"selLang\" name=\"selLang\" OnChange=\"javascript:LoadApproval(\'frmContent\');\">");
                for (count = 0; count <= language_data.Length - 1; count++)
                {
                    if (language_data[count].Id == ContentLanguage)
                    {
                        result.Append("<option value=" + language_data[count].Id + " selected>" + language_data[count].Name + "</option>");
                    }
                    else
                    {
                        result.Append("<option value=" + language_data[count].Id + ">" + language_data[count].Name + "</option>");
                    }
                }
                result.Append("</select></td>");
            }
        }
		result.Append(StyleHelper.ActionBarDivider);
        result.Append("<td>");
        result.Append(m_refStyle.GetHelpButton(m_strPageAction, ""));
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

