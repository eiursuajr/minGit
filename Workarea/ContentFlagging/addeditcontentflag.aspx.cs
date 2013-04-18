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


public partial class addeditcontentflag : workareabase
{
    protected string m_sPage = "addeditcontentflag.aspx";
    protected long content_id = 0;
    protected PermissionData security_data;
    protected ContentFlagData cfFlag;
    protected ContentFlagData[] aFlags = (Ektron.Cms.ContentFlagData[])Array.CreateInstance(typeof(Ektron.Cms.ContentFlagData), 0);
    protected FlagDefData fdFlagSet = new FlagDefData();

    protected override void Page_Load(object sender, System.EventArgs e)
    {
        base.Page_Load(sender, e);
        try
        {
            if (!string.IsNullOrEmpty(Request.QueryString["fid"]))
            {
                m_iID = Convert.ToInt64(Request.QueryString["fid"]);
            }
            if (!string.IsNullOrEmpty(Request.QueryString["cid"]))
            {
                content_id = Convert.ToInt64(Request.QueryString["cid"]);
            }

            CheckPermissions();

            if (Page.IsPostBack)
            {
                switch (base.m_sPageAction)
                {
                    default: // "edit"
                        Process_Edit();
                        break;
                }
            }
            else
            {
                cfFlag = this.m_refContentApi.EkContentRef.GetContentFlag(m_iID);
                RenderJS();
                switch (base.m_sPageAction)
                {
                    case "delete":
                        Process_Delete();
                        break;
                    case "edit":
                        Display_Edit();
                        break;
                    default: // "view"
                        Display_View();
                        break;
                }
                SetLabels();
            }
        }
        catch (Exception ex)
        {
            Utilities.ShowError(ex.Message);
        }
    }
    protected void Process_Edit()
    {
        cfFlag = this.m_refContentApi.EkContentRef.GetContentFlag(this.m_iID);
        if (cfFlag.EntryId > 0)
        {
            cfFlag.FlagComment = (string)this.txt_comment.Text;
            cfFlag.FlagId = long.Parse(drp_flag_data.SelectedValue);
            cfFlag = this.m_refContentApi.EkContentRef.UpdateContentFlag(cfFlag);
        }
        string pagemode = (string)("&page=" + Request.QueryString["page"]);
        Response.Redirect(m_sPage + "?action=view&id=" + this.m_iID + "&cid=" + this.content_id + pagemode, false);
    }
    protected void Process_Delete()
    {
        cfFlag = this.m_refContentApi.EkContentRef.GetContentFlag(this.m_iID);
        if (cfFlag.EntryId > 0)
        {
            this.m_refContentApi.EkContentRef.DeleteContentFlag(cfFlag.EntryId);
        }
        if (Request.QueryString["page"] == "workarea")
        {
            // redirect to workarea
            Response.Write("<script language=\"Javascript\">" + "top.switchDesktopTab();" + "</script>");
        }
        else
        {
            Response.Redirect((string)("../ContentStatistics.aspx?page=ContentStatistics.aspx&id=" + this.content_id + "&LangType=" + this.ContentLanguage), false);
        }
    }
    protected void Display_Edit()
    {
        if (cfFlag.FlaggedUser.Id == 0)
        {
            ltr_uname_data.Text = base.GetMessage("lbl anon");
        }
        else
        {
            ltr_uname_data.Text = cfFlag.FlaggedUser.Username;
        }
        ltr_date_data.Text = cfFlag.FlagDate.ToLongDateString() + " " + cfFlag.FlagDate.ToShortTimeString();
        txt_comment.Text = Server.HtmlDecode(cfFlag.FlagComment);

        //fdFlagSet = cfFlag.FlagDefinition
        fdFlagSet = this.m_refContentApi.EkContentRef.GetFlaggingDefinitionbyID(cfFlag.FlagDefinition.ID, true);

        for (int i = 0; i <= (fdFlagSet.Items.Length - 1); i++)
        {
            drp_flag_data.Items.Add(new ListItem(Server.HtmlDecode((string)(fdFlagSet.Items[i].Name)), fdFlagSet.Items[i].ID.ToString()));
            if (fdFlagSet.Items[i].ID == cfFlag.FlagId)
            {
                drp_flag_data.SelectedIndex = i;
            }
        }
    }
    protected void Display_View()
    {
        if (cfFlag.FlaggedUser.Id == 0)
        {
            ltr_uname_data.Text = base.GetMessage("lbl anon");
        }
        else
        {
            ltr_uname_data.Text = cfFlag.FlaggedUser.Username;
        }
        ltr_date_data.Text = cfFlag.FlagDate.ToLongDateString() + " " + cfFlag.FlagDate.ToShortTimeString();
        txt_comment.Text = Server.HtmlDecode(cfFlag.FlagComment);

        //fdFlagSet = cfFlag.FlagDefinition
        fdFlagSet = this.m_refContentApi.EkContentRef.GetFlaggingDefinitionbyID(cfFlag.FlagDefinition.ID, true);
        if (fdFlagSet != null)
        {
            for (int i = 0; i <= (fdFlagSet.Items.Length - 1); i++)
            {
                drp_flag_data.Items.Add(new ListItem(Server.HtmlDecode((string)(fdFlagSet.Items[i].Name)), fdFlagSet.Items[i].ID.ToString()));
                if (fdFlagSet.Items[i].ID == cfFlag.FlagId)
                {
                    drp_flag_data.SelectedIndex = i;
                }
            }
        }
    }
    protected void CheckPermissions()
    {
        security_data = this.m_refContentApi.LoadPermissions(this.content_id, "content", 0);
        switch (base.m_sPageAction)
        {
            case "edit":
                if (security_data.CanEdit == true)
                {
                    // we are good
                }
                else
                {
                    throw (new Exception(this.GetMessage("err no perm edit")));
                }
                break;
            default: // "view"
                if (security_data.IsReadOnly == true)
                {
                    // we are good
                }
                else
                {
                    throw (new Exception(this.GetMessage("err no perm view")));
                }
                break;
        }
    }
    protected void SetLabels()
    {
        this.ltr_date.Text = this.GetMessage("generic datecreated") + ":";
        this.ltr_uname.Text = this.GetMessage("generic username") + ":";
        this.ltr_flag.Text = this.GetMessage("flag label") + ":";
        this.ltr_comment.Text = this.GetMessage("comment text") + ":";
        StyleHelper m_refStyle = new StyleHelper();
        string pagemode = (string)("&page=" + Request.QueryString["page"]);
        switch (base.m_sPageAction)
        {
            case "edit":
                base.SetTitleBarToMessage("generic edit title");
				base.AddBackButton(m_sPage + "?action=view&id=" + this.m_iID + "&cid=" + this.content_id + pagemode);
                base.AddButtonwithMessages(AppImgPath + "../UI/Icons/save.png", "#", "alt save button text (flag)", "btn save", "OnClick=\"javascript:SubmitForm();return true;\"", StyleHelper.SaveButtonCssClass, true);
                break;
            default: // "view"
                this.drp_flag_data.Enabled = false;
                this.txt_comment.Enabled = false;
                base.SetTitleBarToMessage("generic view");

				if (Request.QueryString["page"] == "workarea")
				{
					// redirect to workarea when user clicks back button if we're in workarea
					base.AddButtonwithMessages(AppImgPath + "../UI/Icons/back.png", "#", "alt back button text", "btn back", " onclick=\"javascript:top.switchDesktopTab()\" ", StyleHelper.BackButtonCssClass, true);
				}
				else
				{
					base.AddBackButton((string)("../ContentStatistics.aspx?page=ContentStatistics.aspx&id=" + this.content_id + "&LangType=" + this.ContentLanguage));
				}

                if (security_data.CanEdit == true)
                {
					base.AddButtonwithMessages(AppImgPath + "../UI/Icons/contentEdit.png", m_sPage + "?action=edit&id=" + this.m_iID + "&cid=" + this.content_id + pagemode, "alt edit button text", "btn edit", "", StyleHelper.EditButtonCssClass, true);
					base.AddButtonwithMessages(AppImgPath + "../UI/Icons/delete.png", m_sPage + "?action=delete&id=" + this.m_iID + "&cid=" + this.content_id + pagemode, "btn alt del flag", (string)("btn delete"), " onclick=\"javascript:return confirm(\'" + base.GetMessage("js conf del flag") + "\');\" ", StyleHelper.DeleteButtonCssClass);
                }
                
                break;
        }
        base.AddHelpButton("AddEditFlags");
    }

    private void RenderJS()
    {
        StringBuilder sbJS = new StringBuilder();
        sbJS.Append("<script language=\"javascript\" type=\"text/javascript\" >" + Environment.NewLine);
        if (this.m_sPageAction == "edit")
        {
            sbJS.Append("function SubmitForm()" + Environment.NewLine);
            sbJS.Append("{" + Environment.NewLine);
            sbJS.Append("    document.forms[0].submit();" + Environment.NewLine);
            sbJS.Append("}" + Environment.NewLine);
        }
        sbJS.Append("</script>" + Environment.NewLine);
        ltr_js.Text += Environment.NewLine + sbJS.ToString();
    }

}
