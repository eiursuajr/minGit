using System;
using System.Collections;
using System.Data;
using System.Text;
using System.Web.UI;
using System.Web.UI.WebControls;
using Ektron.Cms;
using Ektron.Cms.Content;
using Ektron.Cms.Workarea;

public partial class threadeddisc_restrictIP : workareabase
{
    protected EkContent cContent;
    protected RestrictedIP[] aRestricted;
    protected RestrictedIP riRestrict;
    protected DiscussionBoard[] aAllBoards;
    protected bool bIsAdmin = false;
    protected long m_iBoardID = 0;
    protected bool bfromboard = false;

    protected override void Page_Load(object sender, System.EventArgs e)
    {
        cContent = m_refContentApi.EkContentRef;
        aAllBoards = (DiscussionBoard[])Array.CreateInstance(typeof(DiscussionBoard), 0);
        bIsAdmin = m_refContentApi.IsAdmin();
        base.Page_Load(sender, e);
        Ektron.Cms.CommonApi api = new Ektron.Cms.CommonApi();

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
            if (!string.IsNullOrEmpty(Request.QueryString["fromboard"]))
            {
                bfromboard = true;
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
                aRestricted = cContent.SelectRestrictedIPByBoard(this.m_iBoardID);
            }
            else
            {
                aRestricted = cContent.SelectRestrictedIP(0);
            }
            dgRestricted.DataSource = CreateDataView(aRestricted);
            dgRestricted.DataBind();
        }
        catch (Exception ex)
        {
            Utilities.ShowError(ex.Message);
        }
    }

    private DataView CreateDataView(RestrictedIP[] aRI)
    {
        DataView dv = new DataView();
        System.Web.UI.WebControls.BoundColumn colBound = new System.Web.UI.WebControls.BoundColumn();

        colBound = new System.Web.UI.WebControls.BoundColumn();
        colBound.DataField = "IPMask";
        colBound.HeaderText = this.GetMessage("lbl block ip");
        dgRestricted.Columns.Add(colBound);

        colBound = new System.Web.UI.WebControls.BoundColumn();
        colBound.DataField = "ID";
        colBound.HeaderText = this.GetMessage("generic id");
        dgRestricted.Columns.Add(colBound);

        colBound = new System.Web.UI.WebControls.BoundColumn();
        colBound.DataField = "RestrictedSince";
        colBound.HeaderText = this.GetMessage("lbl since");
        dgRestricted.Columns.Add(colBound);

        if (m_iBoardID == 0)
        {
            colBound = new System.Web.UI.WebControls.BoundColumn();
            colBound.DataField = "AppliesTo";
            colBound.HeaderText = this.GetMessage("lbl applies to");
            dgRestricted.Columns.Add(colBound);
        }

        DataTable dt = new DataTable();
        DataRow dr;

        dt.Columns.Add(new DataColumn("IPMask", typeof(string)));
        dt.Columns.Add(new DataColumn("ID", typeof(string)));
        dt.Columns.Add(new DataColumn("RestrictedSince", typeof(string)));
        dt.Columns.Add(new DataColumn("AppliesTo", typeof(string)));

        for (int i = 0; i <= (aRI.Length - 1); i++)
        {
            dr = dt.NewRow();
            dr[0] = "<a href=\"restrictIP.aspx?action=view&boardid=" + this.m_iBoardID.ToString() + "&id=" + aRI[i].RestrictedID.ToString() + "\">" + aRI[i].IPMask + "</a>";
            dr[1] = "<a href=\"restrictIP.aspx?action=view&boardid=" + this.m_iBoardID.ToString() + "&id=" + aRI[i].RestrictedID.ToString() + "\">" + aRI[i].RestrictedID.ToString() + "</a>";
            dr[2] = aRI[i].RestrictedSince.ToLongDateString() + " " + aRI[i].RestrictedSince.ToShortTimeString();
            if (m_iBoardID == 0)
            {
                for (int j = 0; j <= (aRI[i].AppliesTo.Length - 1); j++)
                {
                    dr[3] += "<img valign=\'center\' src=\'" + m_refContentApi.AppImgPath + "menu/users2.gif" + "\' />&nbsp;<a href=\"../content.aspx?action=ViewContentByCategory&id=" + aRI[i].AppliesTo[j].Id.ToString() + "\">" + aRI[i].AppliesTo[j].Name + "</a>";
                    if (j < aRI[i].AppliesTo.Length)
                    {
                        dr[3] += "<br/>";
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
        aRestricted = cContent.SelectRestrictedIP(this.m_iID);
        aAllBoards = cContent.GetAllBoards();
        txt_mask.Text = aRestricted[0].IPMask;
        txt_mask.Enabled = false;
        if (m_iBoardID > 0)
        {
            tr_applies.Visible = false;
        }
        else
        {
            for (int i = 0; i <= (aAllBoards.Length - 1); i++)
            {
                bool bChecked = false;
                for (int j = 0; j <= (aRestricted[0].AppliesTo.Length - 1); j++)
                {
                    if (aRestricted[0].AppliesTo[j].Id == aAllBoards[i].Id)
                    {
                        bChecked = true;
                        break;
                    }
                }
                cl_boards.Items.Add(new ListItem(aAllBoards[i].Name + " - " + GetPath(aAllBoards[i].Path), aAllBoards[i].Id.ToString(), false));
                if (bChecked == true)
                {
                    cl_boards.Items[i].Selected = true;
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
                cContent.DeleteRestrictedIP(this.m_iID);
                Response.Redirect((string)("restrictIP.aspx?boardid=" + this.m_iBoardID.ToString()), false);
            }
            else
            {
                throw (new Exception("Can\'t Delete this Restricted IP"));
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
        txt_mask.Enabled = true;
        aAllBoards = cContent.GetAllBoards();
        if (this.m_iID > 0) //edit
        {
            aRestricted = cContent.SelectRestrictedIP(this.m_iID);
            txt_mask.Text = aRestricted[0].IPMask;
            if (m_iBoardID > 0)
            {
                tr_applies.Visible = false;
            }
            else
            {
                for (int i = 0; i <= (aAllBoards.Length - 1); i++)
                {
                    bool bChecked = false;
                    for (int j = 0; j <= (aRestricted[0].AppliesTo.Length - 1); j++)
                    {
                        if (aRestricted[0].AppliesTo[j].Id == aAllBoards[i].Id)
                        {
                            bChecked = true;
                            break;
                        }
                    }
                    cl_boards.Items.Add(new ListItem(aAllBoards[i].Name + " - " + GetPath(aAllBoards[i].Path), aAllBoards[i].Id.ToString()));
                    if (bChecked == true)
                    {
                        cl_boards.Items[i].Selected = true;
                    }
                }
            }
        }
        else
        {
            if (m_iBoardID > 0)
            {
                tr_applies.Visible = false;
            }
            else
            {
                for (int i = 0; i <= (aAllBoards.Length - 1); i++)
                {
                    cl_boards.Items.Add(new ListItem(aAllBoards[i].Name + " - " + GetPath(aAllBoards[i].Path), aAllBoards[i].Id.ToString()));
                }
            }
        }
    }

    public void Process_AddEdit()
    {
        ArrayList alTmp = new ArrayList();
        FolderData fdTmp;
        if (this.m_iID > 0)
        {
            aRestricted = cContent.SelectRestrictedIP(this.m_iID);
            riRestrict = aRestricted[0];
        }
        else
        {
            riRestrict = new RestrictedIP();
        }
        riRestrict.IPMask = Request.Form[txt_mask.UniqueID];
        if (m_iBoardID > 0)
        {
            riRestrict.AppliesTo = (FolderData[])Array.CreateInstance(typeof(FolderData), 1);
            riRestrict.AppliesTo[0] = new FolderData();
            riRestrict.AppliesTo[0].Id = this.m_iBoardID;
        }
        else
        {
            for (int i = 0; i <= (cl_boards.Items.Count - 1); i++)
            {
                if (cl_boards.Items[i].Selected == true)
                {
                    fdTmp = new FolderData();
                    fdTmp.Id = Convert.ToInt64(cl_boards.Items[i].Value);
                    alTmp.Add(fdTmp);
                }
            }
            riRestrict.AppliesTo = (FolderData[])alTmp.ToArray(typeof(FolderData));
        }
        if (this.m_iID > 0) // edit
        {
            if (m_iBoardID > 0)
            {
                riRestrict = cContent.UpdateRestrictedIP(riRestrict, this.m_iBoardID);
            }
            else
            {
                riRestrict = cContent.AddEditRestrictedIP(riRestrict);
            }
            Response.Redirect((string)("restrictIP.aspx?boardid=" + this.m_iBoardID.ToString() + "&action=view&id=" + this.m_iID.ToString()), false);
        }
        else // add
        {
            riRestrict = cContent.AddEditRestrictedIP(riRestrict);
            Response.Redirect((string)("restrictIP.aspx?boardid=" + this.m_iBoardID.ToString()), false);
        }
    }
    #endregion

    #region Helper Functions
    private void SetLabels()
    {
        base.SetTitleBarToMessage("lbl restricted ips");
        switch (this.m_sPageAction)
        {
            case "view":
            case "edit":
                if (this.m_sPageAction == "view")
                {
					base.AddBackButton((string)("restrictIP.aspx?boardid=" + this.m_iBoardID.ToString())); 
					base.AddButtonwithMessages(AppImgPath + "../UI/Icons/contentEdit.png", (string)("restrictIP.aspx?boardid=" + this.m_iBoardID.ToString() + "&action=edit&id=" + this.m_iID.ToString()), "btn alt edit restricted ip", (string)("btn edit"), "", StyleHelper.EditButtonCssClass, true);
                    base.AddButtonwithMessages(AppImgPath + "../UI/Icons/delete.png", (string)("restrictIP.aspx?boardid=" + this.m_iBoardID.ToString() + "&action=delete&id=" + this.m_iID.ToString()), "btn alt del restricted ip", "btn delete", " onclick=\"javascript:return confirm(\'" + base.GetMessage("js conf del restricted ip") + "\');\" ", StyleHelper.DeleteButtonCssClass);
                }
                else if (this.m_sPageAction == "edit")
                {
					if (bfromboard == true)
					{
						base.AddBackButton((string)("../content.aspx?action=ViewContentByCategory&id=" + this.m_iBoardID.ToString()));
					}
					else if (this.m_iID > 0)
					{
						base.AddBackButton((string)("restrictIP.aspx?boardid=" + this.m_iBoardID.ToString() + "&action=view&id=" + this.m_iID.ToString()));
					}
					else
					{
						base.AddBackButton((string)("restrictIP.aspx?boardid=" + this.m_iBoardID.ToString()));
					}
					
					base.AddButtonwithMessages(AppImgPath + "../UI/Icons/save.png", (string)("#" + this.m_iID.ToString()), "btn alt save restricted ip", "btn save", " onclick=\"javascript:return submitform();\" ", StyleHelper.SaveButtonCssClass, true);
                }
                ltr_mask.Text = this.GetMessage("lbl block ip");
                ltr_appliesto.Text = this.GetMessage("lbl applies to");
                break;
            default:
                if (this.m_iBoardID > 0)
                {
                    base.AddBackButton((string)("../content.aspx?action=ViewContentByCategory&id=" + this.m_iBoardID.ToString()));
                }
				base.AddButtonwithMessages(AppImgPath + "../UI/Icons/add.png", "restrictIP.aspx?boardid=" + this.m_iBoardID.ToString() + "&action=edit", (string)("btn alt add restricted ip"), (string)("generic add title"), "", StyleHelper.AddButtonCssClass, true);
                break;
        }
        base.AddHelpButton("restrictIP");
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
            sbJS.Append("    bVal = checkmask();").Append(Environment.NewLine);
            if (m_iBoardID == 0)
            {
                sbJS.Append("    bVal = checkselections(bVal);").Append(Environment.NewLine);
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

            sbJS.Append("function checkmask()").Append(Environment.NewLine);
            sbJS.Append("{").Append(Environment.NewLine);
            sbJS.Append("    var sText = document.getElementById(\'").Append(txt_mask.UniqueID).Append("\').value;").Append(Environment.NewLine);
            sbJS.Append("    return verifyIP(sText);").Append(Environment.NewLine);
            sbJS.Append("}").Append(Environment.NewLine);

            sbJS.Append("function checkselections(bVal)").Append(Environment.NewLine);
            sbJS.Append("{").Append(Environment.NewLine);
            sbJS.Append("    if (bVal == false) {").Append(Environment.NewLine);
            sbJS.Append("       return false;").Append(Environment.NewLine);
            sbJS.Append("    } else {").Append(Environment.NewLine);
            sbJS.Append("       bVal = false; //we only need one to reset to true").Append(Environment.NewLine);
            sbJS.Append("       for (var x = 0; x < ").Append((aAllBoards.Length).ToString()).Append("; x++) ").Append(Environment.NewLine);
            sbJS.Append("       {").Append(Environment.NewLine);
            sbJS.Append("           if (document.getElementById(\'").Append(cl_boards.UniqueID).Append("_\' + x).checked == true) ").Append(Environment.NewLine);
            sbJS.Append("           {").Append(Environment.NewLine);
            sbJS.Append("               bVal = true;").Append(Environment.NewLine);
            sbJS.Append("           }").Append(Environment.NewLine);
            sbJS.Append("       }").Append(Environment.NewLine);
            sbJS.Append("       if (bVal == false) {").Append(Environment.NewLine);
            sbJS.Append("           alert(\'You must select at least one board.\');").Append(Environment.NewLine);
            sbJS.Append("       }").Append(Environment.NewLine);
            sbJS.Append("       return bVal;").Append(Environment.NewLine);
            sbJS.Append("    }").Append(Environment.NewLine);
            sbJS.Append("}").Append(Environment.NewLine);

            sbJS.Append("function verifyIP(IPvalue) {").Append(Environment.NewLine);
            sbJS.Append("   var errorString = \'\';").Append(Environment.NewLine);
            sbJS.Append("   var ipArray = IPvalue.split(\'.\');").Append(Environment.NewLine);
            sbJS.Append("   if ((IPvalue.length > 15) || (ipArray.length != 4)) {").Append(Environment.NewLine);
            sbJS.Append("       errorString = (\'" + m_refMsg.GetMessage("alert msg not valid ip") + "\');").Append(Environment.NewLine);
            sbJS.Append("   } else if (IPvalue == \"0.0.0.0\") {").Append(Environment.NewLine);
            sbJS.Append("       errorString = errorString + IPvalue+\' is a special IP address and cannot be used.\';").Append(Environment.NewLine);
            sbJS.Append("   } else if (IPvalue == \"255.255.255.255\") {").Append(Environment.NewLine);
            sbJS.Append("       errorString = errorString + IPvalue+\' is a special IP address and cannot be used.\';").Append(Environment.NewLine);
            sbJS.Append("   }").Append(Environment.NewLine);
            sbJS.Append("   if (errorString == \'\') { //continue on").Append(Environment.NewLine);
            sbJS.Append("       for (i = 0; i < 4; i++) {").Append(Environment.NewLine);
            sbJS.Append("          thisSegment = ipArray[i];").Append(Environment.NewLine);
            sbJS.Append("           if (thisSegment.length > 3) {").Append(Environment.NewLine);
            sbJS.Append("               errorString = errorString + IPvalue+\' is not a valid IP address.\';").Append(Environment.NewLine);
            sbJS.Append("               i = 4;").Append(Environment.NewLine);
            sbJS.Append("           }").Append(Environment.NewLine);
            sbJS.Append("           if ((i == 0) && (thisSegment > 255)) {").Append(Environment.NewLine);
            sbJS.Append("               errorString = errorString + IPvalue+\' is a special IP address and cannot be used.\';").Append(Environment.NewLine);
            sbJS.Append("               i = 4;").Append(Environment.NewLine);
            sbJS.Append("           }").Append(Environment.NewLine);
            sbJS.Append("           if ( !( ( (thisSegment < 256) && (thisSegment > -1) ) || (thisSegment == \'*\') ) ) {").Append(Environment.NewLine);
            sbJS.Append("               errorString = errorString + IPvalue+\' is not a valid IP address.\';").Append(Environment.NewLine);
            sbJS.Append("               i = 4;").Append(Environment.NewLine);
            sbJS.Append("           }").Append(Environment.NewLine);
            sbJS.Append("       }").Append(Environment.NewLine);
            sbJS.Append("   }").Append(Environment.NewLine);
            sbJS.Append("   if (errorString == \'\') {").Append(Environment.NewLine);
            sbJS.Append("       return true;").Append(Environment.NewLine);
            sbJS.Append("   } else {").Append(Environment.NewLine);
            sbJS.Append("       alert (errorString);").Append(Environment.NewLine);
            sbJS.Append("       return false;").Append(Environment.NewLine);
            sbJS.Append("   }").Append(Environment.NewLine);
            sbJS.Append("}").Append(Environment.NewLine);

            sbJS.Append("function noenter() {" + Environment.NewLine);
            sbJS.Append("    if (window.event && window.event.keyCode == 13) {" + Environment.NewLine);
            sbJS.Append("        return submitform(); " + Environment.NewLine);
            sbJS.Append("    }" + Environment.NewLine);
            sbJS.Append("}" + Environment.NewLine);
            txt_mask.Attributes.Add("onkeypress", "javascript:return noenter();");

            sbJS.Append("</script>").Append(Environment.NewLine);

            LiteralControl ltr_de_css = new LiteralControl(sbJS.ToString());
            if (!(Page.Header == null))
            {
                Page.Header.Controls.Add(ltr_de_css);
            }
        }
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