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
using Ektron.Cms.Workarea;

public partial class threadeddisc_addeditforum : workareabase
{
    protected Ektron.Cms.Content.EkContent _EkContentRef;
    protected Ektron.Cms.DiscussionCategory[] _DiscussionCategory;
    protected Ektron.Cms.DiscussionForum _DiscussionForum = new Ektron.Cms.DiscussionForum();
    protected long _FolderId = 0;
    protected long _GroupID = -1;
    protected bool usesModal = false;

    protected void Page_Init(object sender, System.EventArgs e)
    {
        _EkContentRef = m_refContentApi.EkContentRef;
        //register page components
        this.RegisterJS();
        this.RegisterCSS();
    }

    protected override void Page_Load(object sender, System.EventArgs e)
    {

        try
        {
            base.Page_Load(sender, e);
            if ((!string.IsNullOrEmpty(Request.QueryString["thickbox"])) && Request.QueryString["thickbox"] == "true")
            {
                usesModal = true;
            }
            if (usesModal)
            {
                TR_moderate.Visible = false;
            }
            if (!string.IsNullOrEmpty(Request.QueryString["groupId"]))
            {
                _GroupID = Convert.ToInt64(Request.QueryString["groupId"]);
            }
            if (_EkContentRef.RequestInformation.IsMembershipUser == 1 && _EkContentRef.IsAllowed(m_iID, 0, "folder", "EditFolders", 0) == false)
            {
                throw (new Exception(base.GetMessage("com: user does not have permission")));
            }
            if (!(Page.IsPostBack))
            {
                if (m_sPageAction == "edit")
                {
                    base.SetTitleBarToMessage("edit forum prop title");

					if (!usesModal)
					{
						base.AddBackButton((string)("addeditforum.aspx?LangType=" + m_refContentApi.ContentLanguage.ToString() + "&action=View&id=" + m_iID.ToString()));
					}

                    base.AddButtonwithMessages(m_refContentApi.AppPath + "images/UI/Icons/save.png", "#", "lbl alt edit forum", "btn save", " onclick=\"return CheckDiscussionForumParameters();\" ", StyleHelper.SaveButtonCssClass, true);
                    
					if (usesModal)
                    {
                        base.AddButtonwithMessages(AppImgPath + "../UI/Icons/remove.png", "#", "generic cancel", "generic cancel", " onclick=\"self.parent.ektb_remove();\" ", StyleHelper.RemoveButtonCssClass);
                    }
                    
                    base.AddHelpButton("EditForum");
                }
                else //view
                {
                    base.SetTitleBarToMessage("view forum prop title");
					base.AddBackButton((string)("../content.aspx?action=ViewContentByCategory&id=" + m_iID.ToString()));
                    base.AddButtonwithMessages(m_refContentApi.AppPath + "images/UI/Icons/contentEdit.png", (string)("addeditforum.aspx?LangType=" + m_refContentApi.ContentLanguage + "&action=Edit&id=" + m_iID.ToString()), "lbl alt edit forum properties", "edit forum prop title", "", StyleHelper.EditButtonCssClass, true);
                    base.AddHelpButton("ViewForumProp");
                }
                // display folder properties
                ltr_lock.Text = GetMessage("lbl lock");
                ltr_adf_moderate.Text = m_refMsg.GetMessage("lbl moderate comments");
                _DiscussionForum = _EkContentRef.GetForumProperties(m_iID);
                txt_adf_forumname.Text = _DiscussionForum.Name;
                txt_adf_forumtitle.Text = _DiscussionForum.Description;
                txt_adf_sortorder.Text = _DiscussionForum.SortOrder.ToString();
                chk_adf_moderate.Checked = _DiscussionForum.ModerateComments;
                chk_adf_lock.Checked = _DiscussionForum.LockForum;
                if (_EkContentRef.GetBoardbyID(_DiscussionForum.BoardID.ToString()).LockBoard)
                {
                    pnlLocked.Visible = true;
                    chk_board_locked.Font.Bold = false;
                    chk_board_locked.Text = this.m_refMsg.GetMessage("lbl board currently locked");
                    if (m_sPageAction == "edit")
                    {
                        chk_board_locked.Style.Add("color", "Red");
                    }
                    else
                    {
                        chk_board_locked.Enabled = false;
                    }
                }
                hdn_adf_folderid.Value = m_iID.ToString();
                ltr_forumid_data.Text = m_iID.ToString();
                // display forum labels
                if (m_sPageAction == "edit")
                {
                    hdn_adf_forumname.Value = _DiscussionForum.Name;
                    SetLabels();
                    Display_DiscussionForumJS();
                }
                else //view
                {
                    SetLabels();
                    SetDisabled();
                }
            }
            else
            {
                Process_DoUpdate();
            }
        }
        catch (Exception ex)
        {
            Utilities.ShowError(ex.Message);
        }
    }

    private void Process_DoUpdate()
    {


        _DiscussionForum.Id = m_iID;
        _DiscussionForum.ForumName = Request.Form[txt_adf_forumname.UniqueID];
        _DiscussionForum.ForumTitle = Request.Form[txt_adf_forumtitle.UniqueID];
        if (Request.Form[chk_adf_moderate.UniqueID] != null)
        {
            _DiscussionForum.ModerateComments = true;
        }
        else
        {
            _DiscussionForum.ModerateComments = false;
        }
        if (Request.Form[chk_adf_lock.UniqueID] != null)
        {
            _DiscussionForum.LockForum = true;
        }
        else
        {
            _DiscussionForum.LockForum = false;
        }
        _DiscussionForum.SetSortOrder(Convert.ToInt32(Request.Form[txt_adf_sortorder.UniqueID]));
        _DiscussionForum.CategoryID = Convert.ToInt64(Request.Form[drp_adf_category.UniqueID]);
        if (Request.Form["EnableReplication"] == "1")
        {
            _DiscussionForum.ReplicationMethod = 1;
        }
        else
        {
            _DiscussionForum.ReplicationMethod = 0;
        }
        if (_GroupID != -1)
        {
            _EkContentRef.UpdateForum(_DiscussionForum, _GroupID);
        }
        else
        {
            _EkContentRef.UpdateForum(_DiscussionForum);
        }
        //If (Request.Form(hdn_adf_forumname.UniqueID) = Request.Form(txt_adf_forumname.UniqueID)) Then
        //    Response.Redirect("../content.aspx?LangType=" & ContentLanguage & "&action=ViewFolder&id=" & m_iID.ToString(), False)
        //Else
        //    Response.Redirect("../content.aspx?TreeUpdated=1&LangType=" & ContentLanguage & "&action=ViewFolder&id=" & m_iID.ToString() & "&reloadtrees=Forms,Content,Library", False)
        //End If
        if (usesModal)
        {
            Response.Redirect(m_refContentApi.ApplicationPath + "CloseThickbox.aspx", false);
        }
        else
        {
            Response.Redirect((string)("../content.aspx?TreeUpdated=1&LangType=" + ContentLanguage + "&reloadtrees=Forms,Content,Library&action=ViewContentByCategory&id=" + _DiscussionForum.Id.ToString()), false);
        }
    }

    private void Display_DiscussionForumJS()
    {
        StringBuilder sbdiscussionforumjs = new StringBuilder();
        sbdiscussionforumjs.Append(Environment.NewLine + Environment.NewLine);
        sbdiscussionforumjs.Append("function CheckDiscussionForumParameters() {" + Environment.NewLine);
        sbdiscussionforumjs.Append("document.forms.form1." + Strings.Replace((string)txt_adf_forumname.UniqueID, "$", "_", 1, -1, 0) + ".value = Trim(document.forms.form1." + Strings.Replace((string)txt_adf_forumname.UniqueID, "$", "_", 1, -1, 0) + ".value);" + Environment.NewLine);
        sbdiscussionforumjs.Append("document.forms.form1." + Strings.Replace((string)txt_adf_sortorder.UniqueID, "$", "_", 1, -1, 0) + ".value = Trim(document.forms.form1." + Strings.Replace((string)txt_adf_sortorder.UniqueID, "$", "_", 1, -1, 0) + ".value);" + Environment.NewLine);
        sbdiscussionforumjs.Append("var iSort = document.forms.form1." + Strings.Replace((string)txt_adf_sortorder.UniqueID, "$", "_", 1, -1, 0) + ".value;" + Environment.NewLine);
        sbdiscussionforumjs.Append("if ((document.forms.form1." + Strings.Replace((string)txt_adf_forumname.UniqueID, "$", "_", 1, -1, 0) + ".value == \"\"))" + Environment.NewLine);
        sbdiscussionforumjs.Append("{" + Environment.NewLine);
        sbdiscussionforumjs.Append("	alert(\"" + m_refMsg.GetMessage("alert msg name supply") + "\");" + Environment.NewLine);
        sbdiscussionforumjs.Append("	document.forms.form1." + Strings.Replace((string)txt_adf_forumname.UniqueID, "$", "_", 1, -1, 0) + ".focus();" + Environment.NewLine);
        sbdiscussionforumjs.Append("	return false;" + Environment.NewLine);
        //iSort
        sbdiscussionforumjs.Append("} else if (isNaN(iSort)||iSort<1||iSort>10000)" + Environment.NewLine);
        sbdiscussionforumjs.Append("{" + Environment.NewLine);
        sbdiscussionforumjs.Append("	alert(\"" + m_refMsg.GetMessage("msg sort") + "\");" + Environment.NewLine);
        sbdiscussionforumjs.Append("	document.forms.form1." + Strings.Replace((string)txt_adf_sortorder.UniqueID, "$", "_", 1, -1, 0) + ".focus();" + Environment.NewLine);
        sbdiscussionforumjs.Append("	return false;" + Environment.NewLine);
        sbdiscussionforumjs.Append("}else {" + Environment.NewLine);
        sbdiscussionforumjs.Append("	if (!CheckDiscussionForumForillegalChar()) {" + Environment.NewLine);
        sbdiscussionforumjs.Append("		return false;" + Environment.NewLine);
        sbdiscussionforumjs.Append("	}" + Environment.NewLine);
        sbdiscussionforumjs.Append("}" + Environment.NewLine);
        sbdiscussionforumjs.Append("var regexp1 = /\"/gi;" + Environment.NewLine);
        sbdiscussionforumjs.Append("document.forms.form1." + Strings.Replace((string)txt_adf_forumname.UniqueID, "$", "_", 1, -1, 0) + ".value = document.forms.form1." + Strings.Replace((string)txt_adf_forumname.UniqueID, "$", "_", 1, -1, 0) + ".value.replace(regexp1, \"\'\");" + Environment.NewLine);
        sbdiscussionforumjs.Append("	document.forms.form1.submit();" + Environment.NewLine);
        sbdiscussionforumjs.Append("}" + Environment.NewLine);
        sbdiscussionforumjs.Append("function CheckDiscussionForumForillegalChar() {" + Environment.NewLine);
        sbdiscussionforumjs.Append("   var val = document.forms.form1." + Strings.Replace((string)txt_adf_forumname.UniqueID, "$", "_", 1, -1, 0) + ".value;" + Environment.NewLine);
        sbdiscussionforumjs.Append("   if ((val.indexOf(\"\\\\\") > -1) || (val.indexOf(\"/\") > -1) || (val.indexOf(\":\") > -1)||(val.indexOf(\"*\") > -1) || (val.indexOf(\"?\") > -1)|| (val.indexOf(\"\\\"\") > -1) || (val.indexOf(\"<\") > -1)|| (val.indexOf(\">\") > -1) || (val.indexOf(\"|\") > -1) || (val.indexOf(\"&\") > -1) || (val.indexOf(\"\\\'\") > 0))" + Environment.NewLine);
        sbdiscussionforumjs.Append("   {" + Environment.NewLine);
        sbdiscussionforumjs.Append("       alert(\"" + m_refMsg.GetMessage("msg sort") + " " + "(\'\\\\\', \'/\', \':\', \'*\', \'?\', \' \\\" \', \'<\', \'>\', \'|\', \'&\', \'\\\'\').\");" + Environment.NewLine);
        sbdiscussionforumjs.Append("       return false;" + Environment.NewLine);
        sbdiscussionforumjs.Append("   }" + Environment.NewLine);
        sbdiscussionforumjs.Append("   return true;" + Environment.NewLine);
        sbdiscussionforumjs.Append("}" + Environment.NewLine);
        ltr_af_js.Text = sbdiscussionforumjs.ToString();
    }

    private void SetLabels()
    {
        ltr_adf_name.Text = base.GetMessage("lbl DiscussionForumName");
        ltr_adf_title.Text = base.GetMessage("lbl DiscussionForumTitle");
        ltr_adf_category.Text = base.GetMessage("lbl DiscussionForumSubject");
        ltr_adf_sortorder.Text = base.GetMessage("lbl discussionforumsortorder");
        ltr_forumid.Text = base.GetMessage("id label");

        _DiscussionCategory = _EkContentRef.GetCategoriesforBoard(_DiscussionForum.ParentId);
        if (!(_DiscussionCategory == null) && (_DiscussionCategory.Length > 0))
        {
            drp_adf_category.DataSource = _DiscussionCategory;
            drp_adf_category.DataTextField = "Name";
            drp_adf_category.DataValueField = "categoryID";
            drp_adf_category.SelectedValue = _DiscussionForum.CategoryID.ToString();
            drp_adf_category.DataBind();
        }
        else
        {
            throw (new Exception(m_refMsg.GetMessage("err NoBoardCategories")));
        }

        Ektron.Cms.SiteAPI refSiteApi = new Ektron.Cms.SiteAPI();
        if (refSiteApi.EnableReplication)
        {
            ltr_adf_dynreplication.Text = "<input type=\"hidden\" name=\"EnableReplication\" value=\"" + _DiscussionForum.ReplicationMethod.ToString() + "\" />";
        }
    }

    private void SetDisabled()
    {
        txt_adf_forumname.Enabled = false;
        txt_adf_forumtitle.Enabled = false;
        chk_adf_moderate.Enabled = false;
        chk_adf_lock.Enabled = false;
        txt_adf_sortorder.Enabled = false;
        drp_adf_category.Enabled = false;
    }

    private void RegisterJS()
    {
        Ektron.Cms.API.JS.RegisterJS(this, Ektron.Cms.API.JS.ManagedScript.EktronJS);
        Ektron.Cms.API.JS.RegisterJS(this, Ektron.Cms.API.JS.ManagedScript.EktronWorkareaJS);
    }

    private void RegisterCSS()
    {
        Ektron.Cms.API.Css.RegisterCss(this, Ektron.Cms.API.Css.ManagedStyleSheet.EktronWorkareaCss);
        Ektron.Cms.API.Css.RegisterCss(this, Ektron.Cms.API.Css.ManagedStyleSheet.EktronWorkareaIeCss, Ektron.Cms.API.Css.BrowserTarget.LessThanEqualToIE7);
    }

}


