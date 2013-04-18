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

public partial class MyWorkspace_MyFavorites : workareabase
{
    protected DirectoryContentData[] m_aFavs = new List<DirectoryContentData>().ToArray();
    protected TaxonomyData m_FavoritesDir = new Ektron.Cms.TaxonomyData();
    protected int m_intCurrentPage = 1;
    protected int m_intTotalPages = 1;
    protected string m_strKeyWords = "";
    protected int m_PageSize = 50;
    protected string m_strSelectedItem = "-1";
    protected bool m_bAllowAdd = false;
    protected int m_iMaxLength = 50;

    protected void Page_Load1(object sender, System.EventArgs e)
    {
        base.Page_Load(sender, e);
		Utilities.ValidateUserLogin();
        if (!Ektron.Cms.DataIO.LicenseManager.LicenseManager.IsFeatureEnable(m_refContentApi.RequestInformationRef, Ektron.Cms.DataIO.LicenseManager.Feature.SocialNetworking))
        {
            Utilities.ShowError(m_refContentApi.EkMsgRef.GetMessage("feature locked error"));
        }
        if (Request.Form["txtSearch"] != "")
        {
            m_strKeyWords = Request.Form["txtSearch"];
        }
        m_PageSize = this.m_refContentApi.RequestInformationRef.PagingSize;
        try
        {
            if (CheckAccess() == false)
            {
                throw (new Exception(this.GetMessage("err myfriends no access")));
            }
            if (Page.IsPostBack)
            {
                switch (this.m_sPageAction)
                {
                    default: // "viewall"
                        if (Request.Form["isDeleted"] == "1")
                        {
                            Process_Remove();
                        }
                        else if (Request.Form["isDeleted"] == "2")
                        {
                            Process_AddFolder();
                        }
                        else if (Request.Form["isDeleted"] == "3")
                        {
                            Process_MoveItems();
                        }
                        break;
                }
            }
            else
            {
                switch (this.m_sPageAction)
                {
                    default: // "viewall"
                        Display_View();
                        break;
                }
                BuildJS();
                SetLabels();
            }

        }
        catch (Exception ex)
        {
            Utilities.ShowError(ex.Message + ex.StackTrace);
        }

    }

    #region Display

    public void Display_View()
    {
        int m_intTotalFav = 0;
        m_FavoritesDir = m_refFavoritesApi.GetFavoritesDirectoryForUser(this.m_refContentApi.UserId, this.m_iID, m_intCurrentPage, System.Convert.ToInt32(m_PageSize > 0 ? m_PageSize : 0), ref m_intTotalPages, ref m_intTotalFav);
        m_aFavs = StepTo(m_FavoritesDir.TaxonomyItems);
        Populate_ViewFavsGrid(m_FavoritesDir, m_aFavs);
    }

    #endregion

    #region Process

    protected void Process_AddFolder()
    {
        string foldername = Request.Form["myfavorites_fName"];
        string folderdesc = Request.Form["myfavorites_fDesc"];
        int m_intTotalFav = 0;
        if ((folderdesc != null) && folderdesc.Length > this.m_iMaxLength)
        {
            folderdesc = folderdesc.Substring(0, m_iMaxLength);
        }

        m_FavoritesDir = m_refFavoritesApi.GetFavoritesDirectoryForUser(this.m_refContentApi.UserId, this.m_iID, m_intCurrentPage, System.Convert.ToInt32(m_PageSize > 0 ? m_PageSize : 0), ref m_intTotalPages, ref m_intTotalFav);

        this.m_refContentApi.AddPersonalDirectoryFolder(m_FavoritesDir.TaxonomyId, foldername, folderdesc);

        if (this.m_iID > 0)
        {
            Response.Redirect((string)("MyFavorites.aspx?id=" + m_FavoritesDir.TaxonomyId), false);
        }
        else
        {
            Response.Redirect("MyFavorites.aspx", false);
        }
    }

    protected void Process_MoveItems()
    {
        string[] aUid = new List<string>().ToArray();
        long iTarget = 0;
        int m_intTotalFav = 0;
        m_FavoritesDir = m_refFavoritesApi.GetFavoritesDirectoryForUser(this.m_refContentApi.UserId, this.m_iID, m_intCurrentPage, System.Convert.ToInt32(m_PageSize > 0 ? m_PageSize : 0), ref m_intTotalPages, ref m_intTotalFav);

        for (int i = 0; i <= (Request.Form.Count - 1); i++)
        {
            if (Request.Form.Keys[i].ToLower().IndexOf("mvdir") == 0)
            {
                iTarget = Convert.ToInt64(Request.Form[i].Substring(System.Convert.ToInt32(("_mv_").Length)));
            }
        }
        aUid = Strings.Split(Request.Form["req_deleted_users"], ",", -1, 0);
        if ((aUid != null) && aUid.Length > 0)
        {
            for (int i = 0; i <= (aUid.Length - 1); i++)
            {
                if (aUid[i].IndexOf("f_") == 0)
                {
                    if (Information.IsNumeric(aUid[i].Substring(2).Trim()))
                    {
                        TaxonomyRequest tReq = new TaxonomyRequest();
                        tReq.TaxonomyId = Convert.ToInt64(aUid[i].Substring(2));
                        tReq.TaxonomyLanguage = this.m_refContentApi.ContentLanguage;
                        if (tReq.TaxonomyId > 0)
                        {
                            this.m_refContentApi.EkContentRef.MoveTaxonomy(Convert.ToInt64(aUid[i].Substring(2)), iTarget, true);
                        }
                    }
                }
                else if (aUid[i].IndexOf("i_") == 0)
                {
                    if (Information.IsNumeric(aUid[i].Substring(2).Trim()))
                    {
                        m_refFavoritesApi.MoveContentFavoriteForUser(Convert.ToInt64(aUid[i].Substring(2)), this.m_refFavoritesApi.UserId, iTarget);
                    }
                }
            }
        }
        Response.Redirect((string)("MyFavorites.aspx" + (this.m_iID > 0 ? "?id=" + this.m_iID : "")), false);
    }

    protected void Process_Remove()
    {
        string[] aUid = new List<string>().ToArray();
        aUid = Strings.Split(Request.Form["req_deleted_users"], ",", -1, 0);
        if ((aUid != null) && aUid.Length > 0)
        {
            for (int i = 0; i <= (aUid.Length - 1); i++)
            {
                if (aUid[i].IndexOf("f_") == 0)
                {
                    if (Information.IsNumeric(aUid[i].Substring(2).Trim()))
                    {
                        TaxonomyRequest tReq = new TaxonomyRequest();
                        tReq.TaxonomyId = Convert.ToInt64(aUid[i].Substring(2));
                        tReq.TaxonomyLanguage = this.m_refContentApi.ContentLanguage;
                        if (tReq.TaxonomyId > 0)
                        {
                            this.m_refContentApi.DeleteTaxonomy(tReq);
                        }
                    }
                }
                else if (aUid[i].IndexOf("i_") == 0)
                {
                    if (Information.IsNumeric(aUid[i].Substring(2).Trim()))
                    {
                        m_refFavoritesApi.DeleteMyContentFavorite(Convert.ToInt64(aUid[i].Substring(2)));
                    }
                }
            }
        }
        Response.Redirect((string)("MyFavorites.aspx" + (this.m_iID > 0 ? "?id=" + this.m_iID : "")), false);
    }

    #endregion

    #region Helper Functions

    public bool CheckAccess()
    {
        if (m_refContentApi.UserId > 0 && this.m_refContentApi.MemberType == 0)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    private DirectoryContentData[] StepTo(TaxonomyItemData[] taxitems)
    {
        System.Collections.Generic.List<DirectoryContentData> idList = new System.Collections.Generic.List<DirectoryContentData>();
        if ((taxitems != null) && taxitems.Length > 0)
        {
            for (int i = 0; i <= (taxitems.Length - 1); i++)
            {
                DirectoryContentData dcFav = new DirectoryContentData();
                dcFav.Id = taxitems[i].TaxonomyItemId;
                dcFav.Title = taxitems[i].TaxonomyItemTitle;
                dcFav.Html = taxitems[i].TaxonomyItemHtml;
                dcFav.LanguageId = taxitems[i].TaxonomyItemLanguage;
                dcFav.Image = taxitems[i].TaxonomyItemImage;
                dcFav.ImageThumbnail = taxitems[i].TaxonomyItemThumbnail;
                dcFav.Teaser = taxitems[i].TaxonomyItemTeaser;
                dcFav.Quicklink = taxitems[i].TaxonomyItemQuickLink;
                idList.Add(dcFav);
            }
        }
        return idList.ToArray();
    }

    private string IsSelected(string val)
    {
        if (val == m_strSelectedItem)
        {
            return (" selected ");
        }
        else
        {
            return ("");
        }
    }

    public void SetLabels()
    {
		if (this.m_iID > 0 && (this.m_FavoritesDir != null))
		{
			if (this.m_FavoritesDir.TaxonomyPath.ToLower() == "\\" + this.m_refContentApi.UserId.ToString() + "\\favorites\\" + this.m_FavoritesDir.TaxonomyName.ToLower())
			{
				this.AddBackButton("MyFavorites.aspx");
			}
			else
			{
				this.AddBackButton((string)("MyFavorites.aspx?id=" + this.m_FavoritesDir.TaxonomyParentId));
			}
		}

        this.AddButtonwithMessages(this.m_refContentApi.AppImgPath + "btn_addfolder-nm.gif", "#", "alt addfolder my favs", "btn addfolder my favs", " onclick=\"toggleVisibility(\'myfavorites_addfolderpanel\');\" ", StyleHelper.AddButtonCssClass, true);
        if ((this.m_iID == 0 && ((this.m_FavoritesDir.Taxonomy != null) && this.m_FavoritesDir.Taxonomy.Length > 0)) || this.m_iID > 0)
        {
            this.AddButtonwithMessages(this.m_refContentApi.AppImgPath + "../UI/Icons/contentCopy.png", "#", "alt move my favs", "btn move my favs", " onclick=\"toggleVisibility(\'myfavorites_movepanel\');\" ", StyleHelper.CopyButtonCssClass);
            SetMove();
        }
        this.AddButtonwithMessages(this.m_refContentApi.AppImgPath + "btn_delete-nm.gif", "#", "alt remove from my favs", "btn remove from my favs", " onclick=\"return CheckDelete();\" ", StyleHelper.DeleteButtonCssClass);
        
        this.AddHelpButton("viewmyfavs");
        this.SetTitleBarToMessage("lbl my favorites");
        this.ltr_move.Text = GetMessage("lbl move my favs to folder");

        this.ltr_addfldrname.Text = this.GetMessage("generic name");
        this.ltr_addfldrdesc.Text = this.GetMessage("generic description");

        ltr_fcharspan.Text = "0/" + m_iMaxLength.ToString();
    }

    protected void SetMove()
    {
        StringBuilder sbText = new StringBuilder();

        sbText.Append("").Append(Environment.NewLine);
        if (this.m_iID > 0)
        {
            sbText.Append("<tr>").Append(Environment.NewLine);
            sbText.Append(" <td>").Append(Environment.NewLine);
            sbText.Append("     <input type=\"radio\" name=\"mvdir\" id=\"_mvdir_" + m_FavoritesDir.TaxonomyParentId.ToString() + "\" value=\"_mv_" + m_FavoritesDir.TaxonomyParentId.ToString() + "\" " + (this.m_FavoritesDir.Taxonomy.Length == 0 ? "checked " : "") + "/>   ").Append(Environment.NewLine);
            sbText.Append(" </td>").Append(Environment.NewLine);
            sbText.Append(" <td>").Append(Environment.NewLine);
            sbText.Append("     ").Append(this.GetMessage("lbl up one level")).Append(" ").Append(Environment.NewLine);
            sbText.Append(" </td>").Append(Environment.NewLine);
            sbText.Append("<tr>").Append(Environment.NewLine);
        }
        for (int i = 0; i <= (this.m_FavoritesDir.Taxonomy.Length - 1); i++)
        {
            sbText.Append("<tr>").Append(Environment.NewLine);
            sbText.Append(" <td>").Append(Environment.NewLine);
            sbText.Append("     <input type=\"radio\" name=\"mvdir\" id=\"" + "_mvdir_" + this.m_FavoritesDir.Taxonomy[i].TaxonomyId.ToString() + "\" value=\"" + "_mv_" + m_FavoritesDir.Taxonomy[i].TaxonomyId.ToString() + "\" " + (i == 0 ? "checked " : "") + "/>   ").Append(Environment.NewLine);
            sbText.Append(" </td>").Append(Environment.NewLine);
            sbText.Append(" <td>").Append(Environment.NewLine);
            sbText.Append("     ").Append(this.m_FavoritesDir.Taxonomy[i].TaxonomyName).Append(" ").Append(Environment.NewLine);
            sbText.Append(" </td>").Append(Environment.NewLine);
            sbText.Append("<tr>").Append(Environment.NewLine);
        }
        ltr_moverows.Text = sbText.ToString();
    }

    protected void BuildJS()
    {
        StringBuilder sbJS = new StringBuilder();

        sbJS.Append("<script language=\"javascript\" type=\"text/javascript\">").Append(Environment.NewLine);
        sbJS.Append("function SubmitForm() {" + Environment.NewLine);
        // TODO
        sbJS.Append("	document.forms[0].submit();" + Environment.NewLine);
        sbJS.Append("}" + Environment.NewLine);

        sbJS.Append("function ExecSearch() {" + Environment.NewLine);
        sbJS.Append("   var sTerm = Trim(document.getElementById(\'txtSearch\').value); " + Environment.NewLine);
        sbJS.Append("   if (sTerm == \'\') {" + Environment.NewLine);
        sbJS.Append("       alert(\'").Append(GetMessage("err js no search term")).Append("\'); " + Environment.NewLine);
        sbJS.Append("   } else {" + Environment.NewLine);
        sbJS.Append("	    document.getElementById(\'hdn_search\').value = true;" + Environment.NewLine);
        sbJS.Append("	    document.forms[0].submit();" + Environment.NewLine);
        sbJS.Append("   }" + Environment.NewLine);
        sbJS.Append("}" + Environment.NewLine);
        sbJS.Append("function resetPostback() {" + Environment.NewLine);
        sbJS.Append("   document.forms[0].isPostData.value = \"\"; " + Environment.NewLine);
        sbJS.Append("}" + Environment.NewLine);

        sbJS.Append("     function CheckDelete() ").Append(Environment.NewLine);
        sbJS.Append("     { ").Append(Environment.NewLine);
        sbJS.Append("         var bCheck = false; ").Append(Environment.NewLine);
        sbJS.Append("         for (var i=0;i<document.forms[0].elements.length;i++){ ").Append(Environment.NewLine);
        sbJS.Append(" 	        var e = document.forms[0].elements[i]; ").Append(Environment.NewLine);
        sbJS.Append(" 			if (e.name==\'req_deleted_users\' && e.checked){ ").Append(Environment.NewLine);
        sbJS.Append(" 			    bCheck = true; ").Append(Environment.NewLine);
        sbJS.Append(" 			} ").Append(Environment.NewLine);
        sbJS.Append(" 	    } ").Append(Environment.NewLine);
        sbJS.Append(" 	    if (bCheck) { if (confirm(\'").Append(GetMessage("js confirm remove from my favs")).Append("\')) { document.getElementById(\'isDeleted\').value = \'1\'; document.forms[0].submit(); } else { bCheck = false; } } ").Append(Environment.NewLine);
        sbJS.Append(" 	    else { alert(\'").Append(GetMessage("js err my favs please select item remove")).Append("\'); } ").Append(Environment.NewLine);
        sbJS.Append(" 	    return bCheck; ").Append(Environment.NewLine);
        sbJS.Append("     } ").Append(Environment.NewLine);

        sbJS.Append("     function CheckAddFolder() ").Append(Environment.NewLine);
        sbJS.Append("     { ").Append(Environment.NewLine);
        sbJS.Append("       var sCheck = \'\'; ").Append(Environment.NewLine);
        sbJS.Append("       var val = document.getElementById(\'myfavorites_fName\').value; ").Append(Environment.NewLine);
        sbJS.Append("       if ((Trim(val) == \'\') || (val.indexOf(\"\\\\\") > -1) || (val.indexOf(\"/\") > -1) || (val.indexOf(\":\") > -1)||(val.indexOf(\"*\") > -1) || (val.indexOf(\"?\") > -1)|| (val.indexOf(\"\\\"\") > -1) || (val.indexOf(\"<\") > -1)|| (val.indexOf(\">\") > -1) || (val.indexOf(\"|\") > -1) || (val.indexOf(\"&\") > -1) || (val.indexOf(\"\\\'\") > -1))  ").Append(Environment.NewLine);
        sbJS.Append("           { sCheck = \"").Append(string.Format(this.GetMessage("js alert folder name cant include"), "(\'\\\\\', \'/\', \':\', \'*\', \'?\', \' \\\" \', \'<\', \'>\', \'|\', \'&\', \'\\\'\')")).Append("\"; }  ").Append(Environment.NewLine);
        sbJS.Append(" 	    if (sCheck == \'\') { document.getElementById(\'isDeleted\').value = \'2\'; document.forms[0].submit(); }  ").Append(Environment.NewLine);
        sbJS.Append(" 	    else { alert(sCheck); } ").Append(Environment.NewLine);
        sbJS.Append("     } ").Append(Environment.NewLine);

        sbJS.Append("     function UpdateMoveStatus(idx) ").Append(Environment.NewLine);
        sbJS.Append("     { ").Append(Environment.NewLine);
        sbJS.Append("	    for (var i=0;i<document.forms[0].elements.length;i++){ ").Append(Environment.NewLine);
        sbJS.Append("		    var e = document.forms[0].elements[i]; ").Append(Environment.NewLine);
        sbJS.Append("		    if (e.name==\'req_deleted_users\' && (e.value == \'f_\' + idx)){ ").Append(Environment.NewLine);
        sbJS.Append("			    document.getElementById(\'_mvdir_\' + idx).disabled = e.checked; ").Append(Environment.NewLine);
        sbJS.Append("			    if (e.checked == true) { document.getElementById(\'_mvdir_\' + idx).checked = false; } ").Append(Environment.NewLine);
        sbJS.Append("			    break; ").Append(Environment.NewLine);
        sbJS.Append("		    } ").Append(Environment.NewLine);
        sbJS.Append("	    } ").Append(Environment.NewLine);
        sbJS.Append("     } ").Append(Environment.NewLine);

        sbJS.Append("     function CheckMoveItems() ").Append(Environment.NewLine);
        sbJS.Append("     { ").Append(Environment.NewLine);
        sbJS.Append("       // check something selected ").Append(Environment.NewLine);
        sbJS.Append("       var sCheck = \'\'; ").Append(Environment.NewLine);
        sbJS.Append("       var bCheck = false; ").Append(Environment.NewLine);
        sbJS.Append("       for (var i=0;i<document.forms[0].elements.length;i++){ ").Append(Environment.NewLine);
        sbJS.Append("       	var e = document.forms[0].elements[i]; ").Append(Environment.NewLine);
        sbJS.Append("       	if (e.name==\'req_deleted_users\' && e.checked){ ").Append(Environment.NewLine);
        sbJS.Append("       	    bCheck = true; ").Append(Environment.NewLine);
        sbJS.Append("       	} ").Append(Environment.NewLine);
        sbJS.Append("       } ").Append(Environment.NewLine);
        sbJS.Append("        ").Append(Environment.NewLine);
        sbJS.Append("       if (!bCheck) { sCheck = \'").Append(this.GetMessage("js err my favs please select item move")).Append("\' + \'.\'; } ").Append(Environment.NewLine);
        sbJS.Append("       // check destination ").Append(Environment.NewLine);
        sbJS.Append("       bCheck = false; ").Append(Environment.NewLine);
        sbJS.Append("       for (var i=0;i<document.forms[0].elements.length;i++){ ").Append(Environment.NewLine);
        sbJS.Append("       	var e = document.forms[0].elements[i]; ").Append(Environment.NewLine);
        sbJS.Append("       	if ((e.id.indexOf(\'_mvdir_\') == 0) && !e.disabled && e.checked){ ").Append(Environment.NewLine);
        sbJS.Append("       		bCheck = true; ").Append(Environment.NewLine);
        sbJS.Append("       	} ").Append(Environment.NewLine);
        sbJS.Append("       } ").Append(Environment.NewLine);
        sbJS.Append("       if (!bCheck) { sCheck = \'").Append(this.GetMessage("js err my favs please select dest move")).Append("\' + \'.\\n\' + sCheck; } ").Append(Environment.NewLine);
        sbJS.Append("       // take action ").Append(Environment.NewLine);
        sbJS.Append("       if (sCheck != \'\') { alert(sCheck); return false; } ").Append(Environment.NewLine);
        sbJS.Append("       else { document.getElementById(\'isDeleted\').value = \'3\'; document.forms[0].submit(); } ").Append(Environment.NewLine);
        sbJS.Append("     } ").Append(Environment.NewLine);

        sbJS.Append("		function CheckKeyValue(item) {").Append(Environment.NewLine);
        sbJS.Append("		    if (event.keyCode == 13) {").Append(Environment.NewLine);
        sbJS.Append("	            CheckAddFolder(); return false;").Append(Environment.NewLine);
        sbJS.Append("			}").Append(Environment.NewLine);
        sbJS.Append("		}").Append(Environment.NewLine);

        sbJS.Append("</script>").Append(Environment.NewLine);

        ltr_js.Text = sbJS.ToString();
    }

    private void Populate_ViewFavsGrid(TaxonomyData folders, DirectoryContentData[] data)
    {
        System.Web.UI.WebControls.BoundColumn colBound;
        bool bOffSet = false;

        colBound = new System.Web.UI.WebControls.BoundColumn();
        colBound.DataField = "CHECKL";
        colBound.ItemStyle.Wrap = false;
        colBound.ItemStyle.VerticalAlign = VerticalAlign.Top;
        colBound.HeaderStyle.Width = Unit.Percentage(5);
        colBound.ItemStyle.Width = Unit.Percentage(5);
        FavGrid.Columns.Add(colBound);


        colBound = new System.Web.UI.WebControls.BoundColumn();
        colBound.DataField = "LEFT";
        colBound.ItemStyle.Wrap = false;
        colBound.ItemStyle.Width = Unit.Percentage(45);
        colBound.ItemStyle.VerticalAlign = VerticalAlign.Top;
        FavGrid.Columns.Add(colBound);

        colBound = new System.Web.UI.WebControls.BoundColumn();
        colBound.DataField = "CHECKR";
        colBound.ItemStyle.Wrap = false;
        colBound.ItemStyle.VerticalAlign = VerticalAlign.Top;
        colBound.ItemStyle.Width = Unit.Percentage(5);
        FavGrid.Columns.Add(colBound);


        colBound = new System.Web.UI.WebControls.BoundColumn();
        colBound.DataField = "RIGHT";
        colBound.ItemStyle.Width = Unit.Percentage(45);
        colBound.ItemStyle.VerticalAlign = VerticalAlign.Top;
        colBound.ItemStyle.Wrap = false;
        FavGrid.Columns.Add(colBound);

        PageSettings();

        DataTable dt = new DataTable();
        DataRow dr;
        dt.Columns.Add(new DataColumn("CHECKL", typeof(string)));
        dt.Columns.Add(new DataColumn("LEFT", typeof(string)));
        dt.Columns.Add(new DataColumn("CHECKR", typeof(string)));
        dt.Columns.Add(new DataColumn("RIGHT", typeof(string)));
        int i = 0;

        dr = dt.NewRow();
        dr["CHECKL"] = "<img align=\"left\" src=\"" + this.m_refContentApi.AppImgPath + "my_favorites.gif" + "\" width=\"32\" height=\"32\"/>";
        dr["LEFT"] = FormatPath(this.m_FavoritesDir.TaxonomyPath);
        dt.Rows.Add(dr);
        dr = dt.NewRow();
        dr["CHECKL"] = "&#160;";
        dr["LEFT"] = "&#160;";
        dt.Rows.Add(dr);

        if (((folders.Taxonomy != null) && folders.Taxonomy.Length > 0) || ((data != null) && data.Length > 0))
        {
            // add select all row.
            dr = dt.NewRow();
            dr["CHECKL"] = "<input type=\"checkbox\" name=\"checkall\" id=\"req_deleted_users\" onClick=\"javascript:checkAll(\'\');\">";
            dr["LEFT"] = GetMessage("generic select all msg") + "<br/><br/>";
            dt.Rows.Add(dr);
            dr = dt.NewRow();
            dr["CHECKL"] = "&#160;";
            dr["LEFT"] = "&#160;";
            dt.Rows.Add(dr);
        }
        if (folders.Taxonomy != null)
        {
            bOffSet = System.Convert.ToBoolean((folders.Taxonomy.Length % 2) > 0);
            for (i = 0; i <= (folders.Taxonomy.Length - 1); i++)
            {
                dr = dt.NewRow();
                dr["CHECKL"] = "<input type=\"checkbox\" name=\"req_deleted_users\" id=\"req_deleted_users\" value=\"f_" + folders.Taxonomy[i].TaxonomyId + "\" onClick=\"javascript:UpdateMoveStatus(\'" + folders.Taxonomy[i].TaxonomyId + "\'); checkAll(\'req_deleted_users\');\">";
                dr["LEFT"] = "<img align=\"left\" src=\"" + this.m_refContentApi.AppImgPath + "folder.gif" + "\" width=\"32\" height=\"32\"/><a href=\"MyFavorites.aspx?id=" + folders.Taxonomy[i].TaxonomyId + "\" >" + folders.Taxonomy[i].TaxonomyName + "</a>";
                dr["LEFT"] += "<br />" + folders.Taxonomy[i].TaxonomyDescription;
                if (i < (folders.Taxonomy.Length - 1))
                {
                    i++;
                    dr["CHECKR"] = "<input type=\"checkbox\" name=\"req_deleted_users\" id=\"req_deleted_users\" value=\"f_" + folders.Taxonomy[i].TaxonomyId + "\" onClick=\"javascript:UpdateMoveStatus(\'" + folders.Taxonomy[i].TaxonomyId + "\'); checkAll(\'req_deleted_users\');\">";
                    dr["RIGHT"] = "<img align=\"left\" src=\"" + this.m_refContentApi.AppImgPath + "folder.gif\"/><a href=\"MyFavorites.aspx?id=" + folders.Taxonomy[i].TaxonomyId + "\" >" + folders.Taxonomy[i].TaxonomyName + "</a>";
                    dr["RIGHT"] += "<br />" + folders.Taxonomy[i].TaxonomyDescription;
                }
                else if (i == (folders.Taxonomy.Length - 1) && bOffSet && (data != null) && data.Length > 0)
                {
                    dr["CHECKR"] = "<input type=\"checkbox\" name=\"req_deleted_users\" id=\"req_deleted_users\" value=\"i_" + data[0].Id + "\" onClick=\"javascript:checkAll(\'req_deleted_users\');\">";
                    dr["RIGHT"] = "<img align=\"left\" src=\"" + this.m_refContentApi.AppImgPath + "content.gif\"/>" + data[0].Title;
                }
                dt.Rows.Add(dr);
            }
        }
        if (!(data == null))
        {
            int iStart = System.Convert.ToInt32(bOffSet ? 1 : 0);
            for (i = iStart; i <= data.Length - 1; i++)
            {
                dr = dt.NewRow();
                dr["CHECKL"] = "<input type=\"checkbox\" name=\"req_deleted_users\" id=\"req_deleted_users\" value=\"i_" + data[i].Id + "\" onClick=\"javascript:checkAll(\'req_deleted_users\');\">";
                dr["LEFT"] = "<img align=\"left\" src=\"" + this.m_refContentApi.AppImgPath + "content.gif" + "\" width=\"32\" height=\"32\"/>" + data[i].Title;
                if (i < (data.Length - 1))
                {
                    i++;
                    dr["CHECKR"] = "<input type=\"checkbox\" name=\"req_deleted_users\" id=\"req_deleted_users\" value=\"i_" + data[i].Id + "\" onClick=\"javascript:checkAll(\'req_deleted_users\');\">";
                    dr["RIGHT"] = "<img align=\"left\" src=\"" + this.m_refContentApi.AppImgPath + "content.gif\"/>" + data[i].Title;
                }
                dt.Rows.Add(dr);
            }
        }
        DataView dv = new DataView(dt);
        FavGrid.DataSource = dv;
        FavGrid.DataBind();
    }

    private void PageSettings()
    {
        if (m_intTotalPages <= 1)
        {
            VisiblePageControls(false);
        }
        else
        {
            VisiblePageControls(true);
            TotalPages.Text = (System.Math.Ceiling((double)m_intTotalPages)).ToString();
            TotalPages.ToolTip = TotalPages.Text;
            CurrentPage.Text = m_intCurrentPage.ToString();
            CurrentPage.ToolTip = CurrentPage.Text;
            PreviousPage1.Enabled = true;
            FirstPage.Enabled = true;
            NextPage.Enabled = true;
            LastPage.Enabled = true;
            if (m_intCurrentPage == 1)
            {
                PreviousPage1.Enabled = false;
                FirstPage.Enabled = false;
            }
            else if (m_intCurrentPage == m_intTotalPages)
            {
                NextPage.Enabled = false;
                LastPage.Enabled = false;
            }
        }
    }

    private void VisiblePageControls(bool flag)
    {
        TotalPages.Visible = flag;
        CurrentPage.Visible = flag;
        PreviousPage1.Visible = flag;
        NextPage.Visible = flag;
        LastPage.Visible = flag;
        FirstPage.Visible = flag;
        PageLabel.Visible = flag;
        OfLabel.Visible = flag;
    }

    private string Quote(string KeyWords)
    {
        string result = KeyWords;
        if (KeyWords.Length > 0)
        {
            result = KeyWords.Replace("\'", "\'\'");
        }
        return result;
    }

    private void CollectSearchText()
    {
        m_strKeyWords = Request.Form["txtSearch"];
        m_strSelectedItem = Request.Form["searchlist"];
        if (m_strSelectedItem == "-1")
        {
            //m_strSearchText = " (first_name like '%" & Quote(m_strKeyWords) & "%' OR last_name like '%" & Quote(m_strKeyWords) & "%' OR user_name like '%" & Quote(m_strKeyWords) & "%')"
        }
        else if (m_strSelectedItem == "last_name")
        {
            //m_strSearchText = " (last_name like '%" & Quote(m_strKeyWords) & "%')"
        }
        else if (m_strSelectedItem == "first_name")
        {
            //m_strSearchText = " (first_name like '%" & Quote(m_strKeyWords) & "%')"
        }
        else if (m_strSelectedItem == "user_name")
        {
            //m_strSearchText = " (user_name like '%" & Quote(m_strKeyWords) & "%')"
        }
    }

    protected string FormatPath(string sPath)
    {
        string[] aPaths = sPath.Split('\\');
        string sRet = "";
        for (int i = (aPaths.Length - 1); i >= 2; i--)
        {
            if (i == 2)
            {
                sRet = (string)("\\&#160;" + aPaths[i] + "&#160;" + sRet);
            }
            else if (i == (aPaths.Length - 1))
            {
                sRet = (string)("\\&#160;" + aPaths[i] + "&#160;" + sRet);
            }
            else if (i == (aPaths.Length - 2))
            {
                sRet = (string)("\\&#160;" + aPaths[i] + "&#160;" + sRet);
            }
            else
            {
                sRet = (string)("\\&#160;" + aPaths[i] + "&#160;" + sRet);
            }
        }
        return sRet;
    }

    #endregion

    #region Grid Events
    protected void Grid_ItemDataBound(object sender, DataGridItemEventArgs e)
    {
        if (this.m_sPageAction == "view")
        {
            switch (e.Item.ItemType)
            {
                case ListItemType.AlternatingItem:
                case ListItemType.Item:
                    if (e.Item.Cells[1].Text.Equals("REMOVE-ITEM") || e.Item.Cells[1].Text.Equals("important") || e.Item.Cells[1].Text.Equals("input-box-text"))
                    {
                        e.Item.Cells[0].Attributes.Add("align", "Left");
                        e.Item.Cells[0].ColumnSpan = 2;
                        if (e.Item.Cells[0].Text.Equals("REMOVE-ITEM"))
                        {
                            //e.Item.Cells(0).CssClass = ""
                        }
                        else
                        {
                            e.Item.Cells[0].CssClass = e.Item.Cells[1].Text;
                        }
                        e.Item.Cells.RemoveAt(1);
                    }
                    break;
            }
        }
    }

    public void NavigationLink_Click(object sender, CommandEventArgs e)
    {
        switch (e.CommandName)
        {
            case "First":
                m_intCurrentPage = 1;
                break;
            case "Last":
                m_intCurrentPage = int.Parse((string)TotalPages.Text);
                break;
            case "Next":
                m_intCurrentPage = System.Convert.ToInt32(int.Parse((string)CurrentPage.Text) + 1);
                break;
            case "Prev":
                m_intCurrentPage = System.Convert.ToInt32(int.Parse((string)CurrentPage.Text) - 1);
                break;
        }
        Display_View();
    }
    #endregion
}