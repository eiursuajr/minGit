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

public partial class MyWorkspace_MyJournal : workareabase
{
    protected long m_iJournalId = -1;
    protected bool m_bAllowAdd = false;
    protected void Page_Load1(object sender, System.EventArgs e)
    {
        base.Page_Load(sender, e);
        try
        {
            if (!Ektron.Cms.DataIO.LicenseManager.LicenseManager.IsFeatureEnable(m_refContentApi.RequestInformationRef, Ektron.Cms.DataIO.LicenseManager.Feature.SocialNetworking))
            {
                Utilities.ShowError(m_refContentApi.EkMsgRef.GetMessage("feature locked error"));
            }
            if (this.m_iID == 0 && this.m_refContentApi.UserId > 0)
            {
                this.m_iID = this.m_refContentApi.UserId;
            }
            if (CheckAccess() == false)
            {
                throw (new Exception(this.GetMessage("err myjournal no access")));
            }
            m_iJournalId = this.m_refContentApi.GetUserBlog(this.m_iID);
            if (m_iJournalId > -1)
            {
                Response.Redirect(this.m_refContentApi.AppPath + "content.aspx?action=ViewContentByCategory&id=" + m_iJournalId.ToString(), false);
            }
            else
            {
                Display_View();
            }
            SetJS();
            SetLabels();
        }
        catch (Exception ex)
        {
            Utilities.ShowError(ex.Message);
        }
    }

    #region Display
    public void Display_View()
    {
        UserAPI m_refUserAPI = new UserAPI();
        UserData uUser = new UserData();
        uUser = m_refUserAPI.GetUserById(this.m_refContentApi.UserId, false, false);
        this.ltr_journal.Text = GetMessage("lbl journal desc");
        this.txtBlogName.Text = string.Format(GetMessage("lbl users journal"), uUser.DisplayName);
        this.txtTitle.Text = GetMessage("lbl journal for user");
        this.btn_Create.Attributes.Add("onclick", "javascript:VerifyBlog(); return false;");
        this.btn_Create.Visible = false;
        this.txtBlogName.Visible = false;
        this.ltr_blogname.Visible = false;
        this.ltr_blogtitle.Visible = false;
        this.txtTitle.Visible = false;
        this.ltr_comments.Visible = false;
        this.chkEnable.Visible = false;
        this.chkModerate.Visible = false;
        this.chkRequire.Visible = false;
        this.drpVisibility.Visible = false;
        this.ltr_visibility.Visible = false;
    }
    #endregion

    #region Process
    protected void btn_Create_Click(object sender, System.EventArgs e)
    {
    }

    protected void Process_Create()
    {
        string tmpPath;
        Collection libSettings;
        Ektron.Cms.Content.EkContent m_refContent;
        string sCatTemp = "";
        Collection pagedata = new Collection();

        m_refContent = m_refContentApi.EkContentRef;
        pagedata.Add(true, "IsBlog", null, null);
        pagedata.Add(Request.Form[txtBlogName.UniqueID], "FolderName", null, null);
        pagedata.Add("", "FolderDescription", null, null);
        pagedata.Add(Request.Form[txtTitle.UniqueID], "BlogTitle", null, null);
        pagedata.Add(Request.Form[drpVisibility.UniqueID], "BlogVisible", null, null);
        pagedata.Add(Request.Form[chkEnable.UniqueID], "CommentEnable", null, null);
        pagedata.Add(Request.Form[chkModerate.UniqueID], "CommentModerate", null, null);
        pagedata.Add(false, "SitemapPathInherit", null, null);
        pagedata.Add(Request.Form[chkRequire.UniqueID], "CommentRequire", null, null);
        pagedata.Add(-1, "ParentID", null, null);
        pagedata.Add("", "TemplateFileName", null, null);
        pagedata.Add(m_refContentApi.AppPath.Replace(m_refContentApi.SitePath, "") + "csslib/blogs.css", "StyleSheet", null, null);

        Ektron.Cms.Library.EkLibrary objLib;
        objLib = m_refContentApi.EkLibraryRef;
        libSettings = objLib.GetLibrarySettingsv2_0();
        tmpPath = (string)libSettings["ImageDirectory"];
        pagedata.Add(Server.MapPath(tmpPath), "AbsImageDirectory", null, null);
        tmpPath = (string)libSettings["FileDirectory"];
        pagedata.Add(Server.MapPath(tmpPath), "AbsFileDirectory", null, null);
        Utilities.AddLBpaths(pagedata);

        pagedata.Add(true, "XmlInherited", null, null);
        pagedata.Add(Request.Form["xmlconfig"], "XmlConfiguration", null, null);
        pagedata.Add(false, "PublishPdfActive", null, null);
        pagedata.Add(false, "PublishHtmlActive", null, null);
        pagedata.Add(false, "IsDomainFolder", null, null);
        pagedata.Add(0, "EnableReplication", null, null);
        pagedata.Add(sCatTemp, "blogcategories", null, null);
        pagedata.Add(null, "blogroll", null, null);
        pagedata.Add("", "break_inherit_button", null, null);
        pagedata.Add("", "folder_cfld_assignments", null, null);
        pagedata.Add(0, "InheritMetadata", null, null);
        pagedata.Add(0, "InheritMetadataFrom", null, null);

        m_refContent.AddContentFolderv2_0(ref pagedata);

        m_iJournalId = Convert.ToInt64(pagedata["FolderID"]);

        Response.Redirect(this.m_refContentApi.AppPath + "content.aspx?action=ViewContentByCategory&id=" + m_iJournalId.ToString(), false);
    }

    #endregion

    #region Helper Functions

    public bool CheckAccess()
    {
        if (m_refContentApi.UserId > 0 && this.m_refContentApi.MemberType == 0 && (m_iID == m_refContentApi.UserId || this.m_refContentApi.IsAdmin()))
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    public void SetLabels()
    {
        this.ltr_blogname.Text = GetMessage("lbl journal name") + ":";
        this.ltr_blogtitle.Text = GetMessage("lbl journal title") + ":";
        this.ltr_visibility.Text = GetMessage("lbl visibility") + ":";
        this.ltr_comments.Text = GetMessage("comments label") + ":";
        if (drpVisibility.Items.Count == 2)
        {
            drpVisibility.Items[0].Text = GetMessage("lbl public");
            drpVisibility.Items[1].Text = GetMessage("lbl private");
        }
        this.AddHelpButton("startjournal");
        this.SetTitleBarToMessage("lbl create a journal for user");
    }

    private void SetJS()
    {
        StringBuilder sbblogjs = new StringBuilder();

        sbblogjs.Append("function UpdateBlogCheckBoxes() {" + Environment.NewLine);
        sbblogjs.Append("   if (document.forms[0]." + Strings.Replace((string)chkEnable.UniqueID, "$", "_", 1, -1, 0) + ".checked == true) {" + Environment.NewLine);
        sbblogjs.Append("       document.forms[0]." + Strings.Replace((string)chkModerate.UniqueID, "$", "_", 1, -1, 0) + ".disabled = false;" + Environment.NewLine);
        sbblogjs.Append("       document.forms[0]." + Strings.Replace((string)chkRequire.UniqueID, "$", "_", 1, -1, 0) + ".disabled = false;" + Environment.NewLine);
        sbblogjs.Append("   } else {" + Environment.NewLine);
        sbblogjs.Append("       document.forms[0]." + Strings.Replace((string)chkModerate.UniqueID, "$", "_", 1, -1, 0) + ".disabled = true;" + Environment.NewLine);
        sbblogjs.Append("       document.forms[0]." + Strings.Replace((string)chkRequire.UniqueID, "$", "_", 1, -1, 0) + ".disabled = true;" + Environment.NewLine);
        sbblogjs.Append("   }" + Environment.NewLine);
        sbblogjs.Append("}" + Environment.NewLine);

        sbblogjs.Append("function VerifyBlog() {" + Environment.NewLine);
        sbblogjs.Append("   document.forms.frmContent." + Strings.Replace((string)txtBlogName.UniqueID, "$", "_", 1, -1, 0) + ".value = Trim(document.forms.frmContent." + Strings.Replace((string)txtBlogName.UniqueID, "$", "_", 1, -1, 0) + ".value);" + Environment.NewLine);
        sbblogjs.Append("   if ((document.forms.frmContent." + Strings.Replace((string)txtBlogName.UniqueID, "$", "_", 1, -1, 0) + ".value == \"\"))" + Environment.NewLine);
        sbblogjs.Append("   {" + Environment.NewLine);
        sbblogjs.Append("   	alert(\"").Append(GetMessage("js supply name for journal")).Append(".\");" + Environment.NewLine);
        sbblogjs.Append("   	document.forms.frmContent." + Strings.Replace((string)txtBlogName.UniqueID, "$", "_", 1, -1, 0) + ".focus();" + Environment.NewLine);
        sbblogjs.Append("   	return false;" + Environment.NewLine);
        sbblogjs.Append("   }else if ((document.forms.frmContent." + Strings.Replace((string)txtTitle.UniqueID, "$", "_", 1, -1, 0) + ".value == \"\"))" + Environment.NewLine);
        sbblogjs.Append("   {" + Environment.NewLine);
        sbblogjs.Append("   	ShowPane(\'dvProperties\');" + Environment.NewLine);
        sbblogjs.Append("   	alert(\"").Append(GetMessage("js supply title for journal")).Append(".\");" + Environment.NewLine);
        sbblogjs.Append("   	document.forms.frmContent." + Strings.Replace((string)txtTitle.UniqueID, "$", "_", 1, -1, 0) + ".focus();" + Environment.NewLine);
        sbblogjs.Append("   	return false;" + Environment.NewLine);
        sbblogjs.Append("   }else {" + Environment.NewLine);
        sbblogjs.Append("   	if (!CheckBlogForillegalChar()) {" + Environment.NewLine);
        sbblogjs.Append("   		return false;" + Environment.NewLine);
        sbblogjs.Append("   	}" + Environment.NewLine);
        sbblogjs.Append("   }" + Environment.NewLine);
        sbblogjs.Append("   var regexp1 = /\"/gi;" + Environment.NewLine);
        sbblogjs.Append("   document.forms.frmContent." + Strings.Replace((string)txtBlogName.UniqueID, "$", "_", 1, -1, 0) + ".value = document.frmContent." + Strings.Replace((string)txtBlogName.UniqueID, "$", "_", 1, -1, 0) + ".value.replace(regexp1, \"\'\");" + Environment.NewLine);
        sbblogjs.Append("	document.forms[0].submit(); return true;" + Environment.NewLine);
        sbblogjs.Append("}" + Environment.NewLine);
        sbblogjs.Append("function CheckBlogForillegalChar() {" + Environment.NewLine);
        sbblogjs.Append("   var val = document.forms.frmContent." + Strings.Replace((string)txtBlogName.UniqueID, "$", "_", 1, -1, 0) + ".value;" + Environment.NewLine);
        sbblogjs.Append("   if ((val.indexOf(\";\") > -1) || (val.indexOf(\"\\\\\") > -1) || (val.indexOf(\"/\") > -1) || (val.indexOf(\":\") > -1)||(val.indexOf(\"*\") > -1) || (val.indexOf(\"?\") > -1)|| (val.indexOf(\"\\\"\") > -1) || (val.indexOf(\"<\") > -1)|| (val.indexOf(\">\") > -1) || (val.indexOf(\"|\") > -1) || (val.indexOf(\"&\") > -1) || (val.indexOf(\"\\\'\") > -1))" + Environment.NewLine);
        sbblogjs.Append("   {" + Environment.NewLine);
        sbblogjs.Append("       alert(\"").Append(GetMessage("js journal name cant include")).Append(" (\';\', \'\\\\\', \'/\', \':\', \'*\', \'?\', \' \\\" \', \'<\', \'>\', \'|\', \'&\', \'\\\'\').\");" + Environment.NewLine);
        sbblogjs.Append("       return false;" + Environment.NewLine);
        sbblogjs.Append("   }" + Environment.NewLine);
        sbblogjs.Append("   return true;" + Environment.NewLine);
        sbblogjs.Append("}" + Environment.NewLine);
        ltr_js.Text = sbblogjs.ToString();
    }
    #endregion
}
