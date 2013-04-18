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

public partial class blogs_addblogroll : workareabase
{
    protected long m_iBlogId = 0;
    protected FolderData folder_data;
    protected PermissionData security_data;

    protected override void Page_Load(object sender, System.EventArgs e)
    {
        try
        {
            base.Page_Load(sender, e);
            folder_data = m_refContentApi.GetFolderById(m_iID, true, true);
            if (folder_data.FolderType == 1) //blog
            {
                if (!CheckAccess())
                {
                    throw (new Exception(this.GetMessage("com: user does not have permission")));
                }
                else
                {
                    if (Page.IsPostBack)
                    {
                        Process_Add();
                    }
                    else
                    {
                        Display_Add();
                        RegisterResources();
                    }
                }
            }
            else
            {
                throw (new Exception(this.GetMessage("blog not found")));
            }
        }
        catch (Exception ex)
        {
            Utilities.ShowError(ex.Message);
        }
    }

    #region Display

    private void Display_Add()
    {
        SetLabels();
        RenderJS();
    }

    #endregion

    #region Process

    private void Process_Add()
    {
        this.m_refContentApi.EkContentRef.AddBlogRollLink(this.m_iID, Request.Form["editfolder_linkname"], Request.Form["editfolder_url"], Request.Form["editfolder_short"], Request.Form["editfolder_rel"]);
        Response.Redirect((string)("../content.aspx?LangType=" + (this.ContentLanguage > 0 ? this.ContentLanguage : this.m_refContentApi.DefaultContentLanguage) + "&action=ViewFolder&id=" + this.m_iID.ToString()), false);
    }

    #endregion

    #region Private Helpers

    private bool CheckAccess()
    {
        security_data = m_refContentApi.LoadPermissions(m_iID, "folder", 0);
        return (security_data.CanEditFolders || security_data.CanEditApprovals) || m_refContentApi.IsARoleMemberForFolder_FolderUserAdmin(m_iID, 0, false);
    }

    private void SetLabels()
    {
        this.SetTitleBarToMessage("lbl add blog roll");
        ltr_linkname.Text = this.GetMessage("lbl roll link name");
        ltr_url.Text = this.GetMessage("lbl roll url");
        ltr_shortdesc.Text = this.GetMessage("lbl roll short desc");
        ltr_rel.Text = this.GetMessage("lbl roll relationship");
        ltr_edit.Text = this.GetMessage("generic edit title");
		this.AddBackButton((string)("../content.aspx?action=ViewContentByCategory&id=" + this.m_iID.ToString()));
        base.AddButtonwithMessages(AppImgPath + "../UI/Icons/save.png", "#", "alt save button text (blogroll)", "btn save", "OnClick=\"javascript:SubmitForm();return false;\"", StyleHelper.SaveButtonCssClass, true);
        this.AddHelpButton("addblogrollink");
    }

    private void RenderJS()
    {
        StringBuilder sbJS = new StringBuilder();
        sbJS.Append("<script language=\"javascript\" type=\"text/javascript\" >" + Environment.NewLine);
        sbJS.Append("var arrRollRel = new Array(0);" + Environment.NewLine);
        sbJS.Append("function SubmitForm()" + Environment.NewLine);
        sbJS.Append("{" + Environment.NewLine);
        sbJS.Append("   if (Trim(document.getElementById(\'editfolder_linkname\').value).length > 0) {" + Environment.NewLine);
        sbJS.Append("       if (Trim(document.getElementById(\'editfolder_url\').value).length > 0) {" + Environment.NewLine);
        sbJS.Append("           document.forms[0].submit();" + Environment.NewLine);
        sbJS.Append("       } else {" + Environment.NewLine);
        sbJS.Append("           alert(\'" + base.GetMessage("js err roll url") + "\');" + Environment.NewLine);
        sbJS.Append("       }" + Environment.NewLine);
        sbJS.Append("   } else {" + Environment.NewLine);
        sbJS.Append("       alert(\'" + base.GetMessage("js err roll link name") + "\');" + Environment.NewLine);
        sbJS.Append("   } " + Environment.NewLine);
        sbJS.Append("}" + Environment.NewLine);
        sbJS.Append("</script>" + Environment.NewLine);
        ltr_js.Text += Environment.NewLine + sbJS.ToString();
    }

    #endregion

    protected void RegisterResources()
    {
        Ektron.Cms.API.Css.RegisterCss(this, Ektron.Cms.API.Css.ManagedStyleSheet.EktronWorkareaIeCss, Ektron.Cms.API.Css.BrowserTarget.AllIE);
        Ektron.Cms.API.Css.RegisterCss(this, Ektron.Cms.API.Css.ManagedStyleSheet.EktronFixedPositionToolbarCss);
        Ektron.Cms.API.Css.RegisterCss(this, Ektron.Cms.API.Css.ManagedStyleSheet.EktronModalCss);
        Ektron.Cms.API.JS.RegisterJS(this, Ektron.Cms.API.JS.ManagedScript.EktronJS);
        Ektron.Cms.API.JS.RegisterJS(this, Ektron.Cms.API.JS.ManagedScript.EktronModalJS);
        Ektron.Cms.API.JS.RegisterJS(this, Ektron.Cms.API.JS.ManagedScript.EktronJFunctJS);
        Ektron.Cms.API.JS.RegisterJS(this, Ektron.Cms.API.JS.ManagedScript.EktronToolBarRollJS);
        Ektron.Cms.API.JS.RegisterJS(this, Ektron.Cms.API.JS.ManagedScript.EktronWorkareaHelperJS);
        Ektron.Cms.API.JS.RegisterJS(this, Ektron.Cms.API.JS.ManagedScript.EktronWorkareaJS);
    }
}


