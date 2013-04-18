using System;
using System.Data;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Ektron.Cms;
using Ektron.Cms.Common;
using Ektron.Cms.Content;
using Ektron.Cms.Workarea;

public partial class threadeddisc_replacewords : workareabase
{
    protected EkContent cContent;
    protected SiteAPI sSite = new SiteAPI();
    protected bool bIsAdmin;
    protected LanguageData[] aLang = null;
    protected DiscussionBoard[] aAllBoards = null;
    protected ReplaceWord rwReplace;
    protected ReplaceWord[] aReplace = null;
    protected long m_iBoardID = 0;
    protected LocalizationAPI objLocalizationApi = new LocalizationAPI();
    protected bool bfromboard = false;
    protected bool bIsEmoticon = false;
    protected string AppPath = "";

    protected override void Page_Load(object sender, System.EventArgs e)
    {
        cContent = m_refContentApi.EkContentRef;
        bIsAdmin = m_refContentApi.IsAdmin();

        base.Page_Load(sender, e);
        Ektron.Cms.CommonApi api = new Ektron.Cms.CommonApi();
        RegisterResources();
        AppPath = m_refContentApi.AppPath;

        if (Request.QueryString["boardid"] != "")
        {
            m_iBoardID = Convert.ToInt64(Request.QueryString["boardid"]);
        }

        if (Convert.ToBoolean(api.RequestInformationRef.IsMembershipUser) || api.RequestInformationRef.UserId == 0)
        {
            Response.Redirect(api.SitePath + "login.aspx", true);
            return;
        }
        else if (!m_refContentApi.IsARoleMemberForFolder_FolderUserAdmin(m_iBoardID, api.RequestInformationRef.UserId, false) && !m_refContentApi.IsAdmin())
        {
            Utilities.ShowError(m_refMsg.GetMessage("com: user does not have permission"));
            return;
        }
        else
        {
            if ((Request.QueryString["isemoticon"] != null) && Request.QueryString["isemoticon"] == "1")
            {
                base.SetTitleBarToString("Emoticons");
            }
            else
            {
                base.SetTitleBarToString(m_refMsg.GetMessage("lbl replace words"));
            }

            if (!string.IsNullOrEmpty(Request.QueryString["fromboard"]))
            {
                bfromboard = true;
            }

            if (Request.QueryString["isemoticon"] != "" && Request.QueryString["isemoticon"] == "1")
            {
                bIsEmoticon = true;
            }

            SetLabels();

            switch (this.m_sPageAction)
            {
                case "edit":
                    if (Page.IsPostBack)
                    {
                        Process_AddEdit();
                    }
                    else
                    {
                        AddEdit();
                    }
                    break;
                case "view":
                    View();
                    break;
                case "delete":
                    Delete();
                    break;
                default: //display
                    Display();
                    break;
            }

            GenerateJS();
        }
    }

    #region Display
    public void Display()
    {
        try
        {
            divAE.Visible = false;
            if (m_iBoardID > 0)
            {
                aReplace = cContent.SelectReplaceWordByBoard(this.m_iBoardID, 0);
            }
            else
            {
                aReplace = cContent.SelectReplaceWord(0, bIsEmoticon);
            }
            dgReplace.DataSource = CreateDataView(aReplace);
            dgReplace.DataBind();
        }
        catch (Exception ex)
        {
            Utilities.ShowError(ex.Message);
        }
    }

    private DataView CreateDataView(ReplaceWord[] aRW)
    {
        DataView dv = new DataView();
        System.Web.UI.WebControls.BoundColumn colBound = new System.Web.UI.WebControls.BoundColumn();

        colBound = new System.Web.UI.WebControls.BoundColumn();
        colBound.DataField = "OldWord";
        colBound.HeaderText = base.GetMessage(GetHeaderString("lbl old word"));
        dgReplace.Columns.Add(colBound);

        colBound = new System.Web.UI.WebControls.BoundColumn();
        colBound.DataField = "NewWord";
        colBound.HeaderText = base.GetMessage(GetHeaderString("lbl new word"));
        dgReplace.Columns.Add(colBound);

        colBound = new System.Web.UI.WebControls.BoundColumn();
        colBound.DataField = "Regex";
        colBound.HeaderText = base.GetMessage(GetHeaderString("lbl regex"));
        colBound.ItemStyle.HorizontalAlign = HorizontalAlign.Center;
        colBound.HeaderStyle.HorizontalAlign = HorizontalAlign.Center;
        dgReplace.Columns.Add(colBound);

        colBound = new System.Web.UI.WebControls.BoundColumn();
        colBound.DataField = "Language";
        colBound.HeaderText = base.GetMessage(GetHeaderString("generic language"));
        dgReplace.Columns.Add(colBound);

        colBound = new System.Web.UI.WebControls.BoundColumn();
        colBound.DataField = "ID";
        colBound.HeaderText = base.GetMessage(GetHeaderString("generic id"));
        dgReplace.Columns.Add(colBound);

        if (m_iBoardID == 0 && !bIsEmoticon)
        {
            colBound = new System.Web.UI.WebControls.BoundColumn();
            colBound.DataField = "AppliesTo";
            colBound.HeaderText = base.GetMessage(GetHeaderString("lbl applies to"));
            dgReplace.Columns.Add(colBound);
        }

        DataTable dt = new DataTable();
        DataRow dr;

        dt.Columns.Add(new DataColumn("OldWord", typeof(string)));
        dt.Columns.Add(new DataColumn("NewWord", typeof(string)));
        dt.Columns.Add(new DataColumn("Regex", typeof(string)));
        dt.Columns.Add(new DataColumn("Language", typeof(string)));
        dt.Columns.Add(new DataColumn("ID", typeof(string)));
        dt.Columns.Add(new DataColumn("AppliesTo", typeof(string)));

        for (int i = 0; i <= (aRW.Length - 1); i++)
        {
            dr = dt.NewRow();
            if (bIsEmoticon)
            {
                dr[0] = "<a style=\"text-decoration: none\" href=\"replacewords.aspx?action=view&boardid=" + this.m_iBoardID.ToString() + "&id=" + aRW[i].ID.ToString() + "&isemoticon=" + Convert.ToInt32(bIsEmoticon) + "\">" + aRW[i].OldWord.Replace("\\", "") + "</a>";
            }
            else
            {
                dr[0] = "<a href=\"replacewords.aspx?action=view&boardid=" + this.m_iBoardID.ToString() + "&id=" + aRW[i].ID.ToString() + "&isemoticon=" + Convert.ToInt32(bIsEmoticon) + "\">" + aRW[i].OldWord + "</a>";
            }
            dr[1] = "<a href=\"replacewords.aspx?action=view&boardid=" + this.m_iBoardID.ToString() + "&id=" + aRW[i].ID.ToString() + "&isemoticon=" + Convert.ToInt32(bIsEmoticon) + "\">" + ConditionalMakeImage(aRW[i].NewWord) + "</a>";
            dr[2] = "<input type=\"checkbox\" " + ((aRW[i].IsRegex == true) ? " checked " : "") + " disabled />";
            dr[3] = "<img src=\'" + objLocalizationApi.GetFlagUrlByLanguageID(aRW[i].LanguageID) + "\' border=\"0\" />";
            dr[4] = "<a href=\"replacewords.aspx?action=view&boardid=" + this.m_iBoardID.ToString() + "&id=" + aRW[i].ID.ToString() + "&isemoticon=" + Convert.ToInt32(bIsEmoticon) + "\">" + aRW[i].ID.ToString() + "</a>";
            if (m_iBoardID == 0)
            {
                for (int j = 0; j <= (aRW[i].AppliesTo.Length - 1); j++)
                {
                    dr[5] += "<img valign=\'center\' src=\'" + m_refContentApi.AppImgPath + "menu/users2.gif" + "\' />&nbsp;<a href=\"../content.aspx?action=ViewContentByCategory&id=" + aRW[i].AppliesTo[j].Id.ToString() + "\">" + aRW[i].AppliesTo[j].Name + "</a>";
                    if (j < aRW[i].AppliesTo.Length)
                    {
                        dr[5] += "<br/>";
                    }
                }
            }
            dt.Rows.Add(dr);
        }
        dv = new DataView(dt);
        return dv;
    }
    #endregion

    #region View
    public void View()
    {
        divList.Visible = false;
        aReplace = cContent.SelectReplaceWord(this.m_iID, bIsEmoticon);
        aLang = sSite.GetAllActiveLanguages();
        aAllBoards = cContent.GetAllBoards();
        txt_old.Text = HttpContext.Current.Server.HtmlDecode(aReplace[0].OldWord);
        txt_old.Enabled = false;
        txt_new.Text = HttpContext.Current.Server.HtmlDecode(aReplace[0].NewWord);
        txt_new.Enabled = false;
        chk_regex.Checked = aReplace[0].IsRegex;
        chk_regex.Enabled = false;
        drp_boards.Enabled = false;
        drp_lang.Enabled = false;
        if (aReplace[0].LanguageID == 0)
        {
            drp_lang.Items.Add(new ListItem("All Languages", "0", true));
        }
        else
        {
            drp_lang.Items.Add(new ListItem("All Languages", "0"));
        }
        for (int j = 0; j <= (aLang.Length - 1); j++)
        {
            if (aReplace[0].LanguageID == aLang[j].Id)
            {
                drp_lang.Items.Add(new ListItem(aLang[j].Name, aLang[j].Id.ToString(), true));
                drp_lang.SelectedIndex = j + 1;
            }
            else
            {
                drp_lang.Items.Add(new ListItem(aLang[j].Name, aLang[j].Id.ToString()));
            }
        }
        if (m_iBoardID > 0 || bIsEmoticon)
        {
            tr_applies.Visible = false;
        }
        else
        {
            for (int i = 0; i <= (aAllBoards.Length - 1); i++)
            {
                bool bChecked = false;
                for (int j = 0; j <= (aReplace[0].AppliesTo.Length - 1); j++)
                {
                    if (aReplace[0].AppliesTo[j].Id == aAllBoards[i].Id)
                    {
                        bChecked = true;
                        break;
                    }
                }
                drp_boards.Items.Add(new ListItem(aAllBoards[i].Name + " - " + GetPath(aAllBoards[i].Path), aAllBoards[i].Id.ToString()));
                if (bChecked == true)
                {
                    drp_boards.Items[i].Selected = true;
                }
            }
        }
    }
    #endregion

    #region Delete
    public void Delete()
    {
        try
        {
            if (this.m_iID > 0)
            {
                cContent.DeleteReplaceWord(this.m_iID);
                Response.Redirect((string)("replacewords.aspx?boardid=" + this.m_iBoardID.ToString() + "&isemoticon=" + Convert.ToInt32(bIsEmoticon)), false);
            }
            else
            {
                throw (new Exception(base.GetMessage("err no del replaceword")));
            }
        }
        catch (Exception ex)
        {
            Utilities.ShowError(ex.Message);
        }
    }
    #endregion

    #region AddEdit
    public void AddEdit()
    {
        divList.Visible = false;
        txt_old.Enabled = true;
        aAllBoards = cContent.GetAllBoards();
        aLang = sSite.GetAllActiveLanguages();
        if (this.m_iID > 0) //edit
        {
            aReplace = cContent.SelectReplaceWord(this.m_iID, bIsEmoticon);
            if (aReplace[0].LanguageID == 0)
            {
                drp_lang.Items.Add(new ListItem("All Languages", "0", true));
            }
            else
            {
                drp_lang.Items.Add(new ListItem("All Languages", "0"));
            }
            txt_old.Text = HttpContext.Current.Server.HtmlDecode(aReplace[0].OldWord);
            txt_new.Text = HttpContext.Current.Server.HtmlDecode((string)(aReplace[0].NewWord));
            chk_regex.Checked = aReplace[0].IsRegex;
            if (m_iBoardID > 0 || bIsEmoticon)
            {
                tr_applies.Visible = false;
            }
            else
            {
                for (int i = 0; i <= (aAllBoards.Length - 1); i++)
                {
                    bool bChecked = false;
                    for (int j = 0; j <= (aReplace[0].AppliesTo.Length - 1); j++)
                    {
                        if (aReplace[0].AppliesTo[j].Id == aAllBoards[i].Id)
                        {
                            bChecked = true;
                            break;
                        }
                    }
                    drp_boards.Items.Add(new ListItem(aAllBoards[i].Name + " - " + GetPath(aAllBoards[i].Path), aAllBoards[i].Id.ToString()));
                    if (bChecked == true)
                    {
                        drp_boards.Items[i].Selected = true;
                    }
                }
            }
            for (int j = 0; j <= (aLang.Length - 1); j++)
            {
                ListItem liItem = new ListItem(aLang[j].Name, aLang[j].Id.ToString());
                if (aReplace[0].LanguageID == aLang[j].Id)
                {
                    liItem.Selected = true;
                }
                drp_lang.Items.Add(liItem);
            }
        }
        else
        {
            drp_lang.Items.Add(new ListItem("All Languages", "0"));
            if (m_iBoardID > 0 || bIsEmoticon)
            {
                tr_applies.Visible = false;
            }
            else
            {
                for (int i = 0; i <= (aAllBoards.Length - 1); i++)
                {
                    drp_boards.Items.Add(new ListItem(aAllBoards[i].Name + " - " + GetPath(aAllBoards[i].Path), aAllBoards[i].Id.ToString()));
                }
            }
            for (int j = 0; j <= (aLang.Length - 1); j++)
            {
                drp_lang.Items.Add(new ListItem(aLang[j].Name, aLang[j].Id.ToString()));
            }
        }
    }

    public void Process_AddEdit()
    {
        try
        {
            if (this.m_iID > 0)
            {
                aReplace = cContent.SelectReplaceWord(this.m_iID, bIsEmoticon);
                rwReplace = aReplace[0];
            }
            else
            {
                rwReplace = new ReplaceWord();
            }
            rwReplace.OldWord = EkFunctions.HtmlEncode(Request.Form[txt_old.UniqueID]).Trim();
            rwReplace.NewWord = EkFunctions.HtmlEncode(Request.Form[txt_new.UniqueID]).Trim();
            rwReplace.IsRegex = System.Convert.ToBoolean(chk_regex.Checked);
            rwReplace.LanguageID = Convert.ToInt32(drp_lang.SelectedValue);

            if (m_iBoardID > 0 || bIsEmoticon)
            {
                rwReplace.AppliesTo = (FolderData[])Array.CreateInstance(typeof(FolderData), 1);
                rwReplace.AppliesTo[0] = new FolderData();
                rwReplace.AppliesTo[0].Id = this.m_iBoardID;
            }
            else if (drp_boards.Items.Count > 0)
            {
                rwReplace.AppliesTo = (FolderData[])Array.CreateInstance(typeof(FolderData), 1);
                rwReplace.AppliesTo[0] = new FolderData();
                rwReplace.AppliesTo[0].Id = Convert.ToInt64(drp_boards.SelectedValue);
            }
            Response.Write(rwReplace.LanguageID.ToString());
            rwReplace = cContent.AddEditReplaceWord(rwReplace, Convert.ToBoolean(bIsEmoticon));
            if (this.m_iID > 0) // edit
            {
                Response.Redirect((string)("replacewords.aspx?boardid=" + this.m_iBoardID.ToString() + "&action=view&id=" + this.m_iID.ToString() + "&isemoticon=" + Convert.ToInt32(bIsEmoticon)), false);
            }
            else // add
            {
                Response.Redirect((string)("replacewords.aspx?boardid=" + this.m_iBoardID.ToString() + "&isemoticon=" + Convert.ToInt32(bIsEmoticon)), false);
            }
        }
        catch (Exception ex)
        {
            Utilities.ShowError(ex.Message);
        }
    }
    #endregion

    #region Helper Functions
    private string GetHeaderString(string val)
    {
        if (!bIsEmoticon)
        {
            return val;
        }
        else
        {
            switch (val)
            {
                case "lbl old word":
                    return "lbl emoticon text";
                case "lbl new word":
                    return "lbl emoticon image";
                default:
                    return val;
            }
        }
    }

    private string ConditionalMakeImage(string val)
    {
        if (!bIsEmoticon)
        {
            return val;
        }
        else
        {
            if (val.IndexOf("/") > -1)
            {
                return "<img src=\"" + val + "\" />";
            }
            else
            {
                return "<img src=\"" + (m_refContentApi.RequestInformationRef.ApplicationPath + "/threadeddisc/emoticons/") + val + "\" />";
            }
        }
    }

    private void SetLabels()
    {
        if ((Request.QueryString["isemoticon"] != null) && Request.QueryString["isemoticon"] == "1")
        {
            base.SetTitleBarToString("Emoticons");
        }
        else
        {
            base.SetTitleBarToString(m_refMsg.GetMessage("lbl replace words"));
        }
        switch (this.m_sPageAction)
        {
            case "view":
            case "edit":
                if (this.m_sPageAction == "view")
                {
                    string delete_message;
                    string delete_message_conf;
                    string edit_message;
                    if (bIsEmoticon)
                    {
                        delete_message = "btn alt del emoticon";
                        delete_message_conf = "js conf del emoticon";
                        edit_message = "btn alt edit emoticon";
                    }
                    else
                    {
                        delete_message = "btn alt del replaceword";
                        delete_message_conf = "js conf del replaceword";
                        edit_message = "btn alt edit replaceword";
                    }
					base.AddBackButton((string)("replacewords.aspx?boardid=" + this.m_iBoardID.ToString() + "&isemoticon=" + Convert.ToInt32(bIsEmoticon)));
                    base.AddButtonwithMessages(AppPath + "images/UI/Icons/contentEdit.png", (string)("replacewords.aspx?boardid=" + this.m_iBoardID.ToString() + "&action=edit&id=" + this.m_iID.ToString() + "&isemoticon=" + Convert.ToInt32(bIsEmoticon)), edit_message, (string)("btn edit"), "", StyleHelper.EditButtonCssClass, true);
                    base.AddButtonwithMessages(AppPath + "images/UI/Icons/delete.png", (string)("replacewords.aspx?boardid=" + this.m_iBoardID.ToString() + "&action=delete&id=" + this.m_iID.ToString() + "&isemoticon=" + Convert.ToInt32(bIsEmoticon)), delete_message, (string)("btn delete"), " onclick=\"javascript:return confirm(\'" + base.GetMessage(delete_message_conf) + "\');\" ", StyleHelper.DeleteButtonCssClass, true);
                }
                else if (this.m_sPageAction == "edit")
                {
					if (bfromboard == true)
					{
						base.AddBackButton((string)("../content.aspx?action=ViewContentByCategory&id=" + this.m_iBoardID.ToString() + "&isemoticon=" + Convert.ToInt32(bIsEmoticon)));
					}
					else if (this.m_iID > 0)
					{
						base.AddBackButton((string)("replacewords.aspx?boardid=" + this.m_iBoardID.ToString() + "&action=view&id=" + this.m_iID.ToString() + "&isemoticon=" + Convert.ToInt32(bIsEmoticon)));
					}
					else
					{
						base.AddBackButton((string)("replacewords.aspx?boardid=" + this.m_iBoardID.ToString() + "&isemoticon=" + Convert.ToInt32(bIsEmoticon)));
					} 
					
					base.AddButtonwithMessages(AppPath + "images/UI/Icons/save.png", (string)("#" + this.m_iID.ToString() + "&isemoticon=" + Convert.ToInt32(bIsEmoticon)), (string)("btn alt save replaceword"), (string)("btn save"), " onclick=\"javascript:return submitform();\" ", StyleHelper.SaveButtonCssClass, true);
				}

                ltr_old.Text = base.GetMessage(GetHeaderString("lbl old word"));
                ltr_new.Text = base.GetMessage(GetHeaderString("lbl new word"));
                ltr_lang.Text = base.GetMessage(GetHeaderString("generic language"));
                ltr_regex.Text = base.GetMessage(GetHeaderString("lbl regex"));
                ltr_appliesto.Text = base.GetMessage(GetHeaderString("lbl applies to"));
                break;

            default:

				if (this.m_iBoardID > 0)
				{
					base.AddBackButton((string)("../content.aspx?action=ViewContentByCategory&id=" + this.m_iBoardID.ToString() + "&isemoticon=" + Convert.ToInt32(bIsEmoticon)));
				}

                if (bIsEmoticon)
                {
                    base.AddButtonwithMessages(AppPath + "images/UI/Icons/add.png", (string)("replacewords.aspx?boardid=" + this.m_iBoardID.ToString() + "&action=edit" + "&isemoticon=" + Convert.ToInt32(bIsEmoticon)), (string)("btn alt add emoticons"), (string)("generic add title"), "", StyleHelper.AddButtonCssClass, true);
                }
                else
                {
					base.AddButtonwithMessages(AppPath + "images/UI/Icons/add.png", (string)("replacewords.aspx?boardid=" + this.m_iBoardID.ToString() + "&action=edit" + "&isemoticon=" + Convert.ToInt32(bIsEmoticon)), (string)("btn alt add replaceword"), (string)("generic add title"), "", StyleHelper.AddButtonCssClass, true);
                }

                break;
        }
        if ((Request.QueryString["isemoticon"] != null) && Request.QueryString["isemoticon"] == "1")
        {
            base.AddHelpButton("emoticons");
        }
        else
        {
            base.AddHelpButton("replaceword");
        }

    }

    private void GenerateJS()
    {
        if (this.m_sPageAction == "edit")
        {
            StringBuilder sbJS = new StringBuilder();

            sbJS.Append("<script language=\"javascript\" type=\"text/javascript\">").Append(Environment.NewLine);
            sbJS.Append("function submitform()").Append(Environment.NewLine);
            sbJS.Append("{").Append(Environment.NewLine);
            sbJS.Append("    var bVal = true;").Append(Environment.NewLine);

            sbJS.Append("    var oBoardDDl = document.getElementById(\'").Append(drp_boards.UniqueID).Append("\');").Append(Environment.NewLine);
            sbJS.Append("    if(oBoardDDl != null && oBoardDDl.options.length==0){").Append(Environment.NewLine);
            sbJS.Append("       alert('" + m_refMsg.GetMessage("no discboard selected") + "'); bVal = false;}").Append(Environment.NewLine);

            sbJS.Append("    var oText = Trim(document.getElementById(\'").Append(txt_old.UniqueID).Append("\').value);").Append(Environment.NewLine);
            sbJS.Append("    var nText = Trim(document.getElementById(\'").Append(txt_new.UniqueID).Append("\').value);").Append(Environment.NewLine);
            sbJS.Append("    if (oText.length == 0 || nText.length == 0)").Append(Environment.NewLine);
            sbJS.Append("    {").Append(Environment.NewLine);
            if (bIsEmoticon)
            {
                sbJS.Append("        alert(\'" + m_refMsg.GetMessage("alert msg supply emoticon") + "\');").Append(Environment.NewLine);
            }
            else
            {
                sbJS.Append("        alert(\'" + m_refMsg.GetMessage("alert msg supply word") + "\');").Append(Environment.NewLine);
            }
            sbJS.Append("        bVal = false;").Append(Environment.NewLine);
            sbJS.Append("    }").Append(Environment.NewLine);
            if (bIsEmoticon)
            {
                sbJS.Append("   if( nText.split(\'.\').length < 2 && bVal ) { alert(\'" + m_refMsg.GetMessage("alert msg valid img") + "\'); bVal = false; }");
                sbJS.Append("   if( bVal ) { var parts = nText.split(\'.\'); var ext = parts[parts.length-1]; if( !(ext == \'png\' || ext == \'jpg\' || ext == \'gif\' || ext == \'jpeg\')) { alert(\'" + m_refMsg.GetMessage("alert msg valid img") + "\'); bVal = false; } }");
            }
            sbJS.Append("    if (bVal == false)").Append(Environment.NewLine);
            sbJS.Append("    {").Append(Environment.NewLine);
            sbJS.Append("        return false;").Append(Environment.NewLine);
            sbJS.Append("    }").Append(Environment.NewLine);
            sbJS.Append("    else").Append(Environment.NewLine);
            sbJS.Append("    {").Append(Environment.NewLine);
            sbJS.Append("        document.forms[0].submit();").Append(Environment.NewLine);
            sbJS.Append("        return true;").Append(Environment.NewLine);
            sbJS.Append("    }").Append(Environment.NewLine);
            sbJS.Append("}").Append(Environment.NewLine);

            sbJS.Append("function noenter() {" + Environment.NewLine);
            sbJS.Append("    if (window.event && window.event.keyCode == 13) {" + Environment.NewLine);
            sbJS.Append("        return submitform(); " + Environment.NewLine);
            sbJS.Append("    }" + Environment.NewLine);
            sbJS.Append("}" + Environment.NewLine);
            txt_old.Attributes.Add("onkeypress", "javascript:return noenter();");
            txt_new.Attributes.Add("onkeypress", "javascript:return noenter();");

            sbJS.Append("</script>").Append(Environment.NewLine);

            LiteralControl ltr_de_css = new LiteralControl(sbJS.ToString());
            if (!(Page.Header == null))
            {
                Page.Header.Controls.Add(ltr_de_css);
            }
        }
    }
    protected void RegisterResources()
    {
        Ektron.Cms.API.Css.RegisterCss(this, Ektron.Cms.API.Css.ManagedStyleSheet.EktronFixedPositionToolbarCss);
    }
    protected string GetPath(string path)
    {
        path = ("/" + path);
        string[] paths = path.Split(new char[] { '/' });

        Array.Resize(ref paths, (paths.Length - 2));
        path = string.Join("/", paths) + "/";

        return path;
    }
    #endregion


}

