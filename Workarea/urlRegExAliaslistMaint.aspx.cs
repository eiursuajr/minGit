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
using Ektron.Cms.UrlAliasing;

public partial class Workarea_urlregexaliaslistmaint : System.Web.UI.Page
{

    private UrlAliasRegExApi _regexAliaslist = new Ektron.Cms.UrlAliasing.UrlAliasRegExApi();
    private UrlAliasManualApi _manualAliaslist = new Ektron.Cms.UrlAliasing.UrlAliasManualApi();
    private UrlAliasSettingsApi _aliasSettingsApi = new Ektron.Cms.UrlAliasing.UrlAliasSettingsApi();
    private int _currentPageNumber = 1;
    private int TotalPagesNumber = 1;
    private int contentLanguage;
    private System.Collections.Generic.Dictionary<long, string> siteDictionary = new System.Collections.Generic.Dictionary<long, string>();
    private System.Collections.Generic.KeyValuePair<long, string> siteList;
    private string strKeyWords = string.Empty;
    private EkEnumeration.RegExAliasSearchField strSelectedItem = Ektron.Cms.Common.EkEnumeration.RegExAliasSearchField.None;
    protected StyleHelper _refStyle = new StyleHelper();
    protected EkMessageHelper msgHelper;
    protected CommonApi _refCommonApi = new CommonApi();
    protected ContentAPI _refContentApi = new ContentAPI();

    protected void Page_Load(object sender, System.EventArgs e)
    {
        msgHelper = _refCommonApi.EkMsgRef;
        string pageAction = "";
        long siteID = 0;
        Ektron.Cms.Content.EkContent objContentRef;
        objContentRef = _refContentApi.EkContentRef;
        RegisterResources();
        SetServerJSVariables();
        //Licensing For 7.6
        if (!Ektron.Cms.DataIO.LicenseManager.LicenseManager.IsFeatureEnable(_refContentApi.RequestInformationRef, Ektron.Cms.DataIO.LicenseManager.Feature.UrlAliasing, false))
        {
            Utilities.ShowError(_refContentApi.EkMsgRef.GetMessage("feature locked error"));
            return;
        }

        if (_refCommonApi.RequestInformationRef.IsMembershipUser > 0 || _refCommonApi.RequestInformationRef.UserId == 0)
        {
            Utilities.ShowError(_refContentApi.EkMsgRef.GetMessage("msg login cms user"));
            return;
        }

        if (!(_refCommonApi.IsAdmin() || objContentRef.IsARoleMember((long)Ektron.Cms.Common.EkEnumeration.CmsRoleIds.UrlAliasingAdmin, _refCommonApi.RequestInformationRef.UserId, false)))
        {
            Utilities.ShowError(_refContentApi.EkMsgRef.GetMessage("User not authorized"));
            return;
        }
        PageLabel.Text = PageLabel.ToolTip = msgHelper.GetMessage("lbl pagecontrol page");
        OfLabel.Text = OfLabel.ToolTip = msgHelper.GetMessage("lbl pagecontrol of");

        FirstPage.ToolTip = msgHelper.GetMessage("lbl first page");
        PreviousPage1.ToolTip = msgHelper.GetMessage("lbl previous page");
        NextPage.ToolTip = msgHelper.GetMessage("lbl next page");
        LastPage.ToolTip = msgHelper.GetMessage("lbl last page");

        FirstPage.Text = "[" + msgHelper.GetMessage("lbl first page") + "]";
        PreviousPage1.Text = "[" + msgHelper.GetMessage("lbl previous page") + "]";
        NextPage.Text = "[" + msgHelper.GetMessage("lbl next page") + "]";
        LastPage.Text = "[" + msgHelper.GetMessage("lbl last page") + "]";
        if (!(Request.QueryString["LangType"] == null))
        {
            if (Request.QueryString["LangType"] != "")
            {
                contentLanguage = Convert.ToInt32(Request.QueryString["LangType"]);
                _refContentApi.SetCookieValue("LastValidLanguageID", contentLanguage.ToString());
            }
            else
            {
                if (_refContentApi.GetCookieValue("LastValidLanguageID") != "")
                {
                    contentLanguage = Convert.ToInt32(_refContentApi.GetCookieValue("LastValidLanguageID"));
                }
            }
        }
        else
        {
            if (_refContentApi.GetCookieValue("LastValidLanguageID") != "")
            {
                contentLanguage = Convert.ToInt32(_refContentApi.GetCookieValue("LastValidLanguageID"));
            }
        }

        _refContentApi.FilterByLanguage = contentLanguage;
        _refCommonApi.EkContentRef.RequestInformation.ContentLanguage = contentLanguage;
        if (!String.IsNullOrEmpty(Request.QueryString["action"]))
        {
            pageAction = Request.QueryString["action"];
        }
        if (!String.IsNullOrEmpty(Request.QueryString["fId"]))
        {
            siteID = Convert.ToInt64(Request.QueryString["fId"]);
        }

        if ((string)(pageAction) == "removealias")
        {
            AddDeleteMenuBar();
            DeleteRegex(siteID.ToString());
        }
        else if ((string)(pageAction) == "clear")
        {
            ClearCache(siteID.ToString());
        }
        else if ((string)(pageAction) == "")
        {
            AddDefaultMenuBar(siteID.ToString());
        }

        if (((pageAction == "" || pageAction == "removealias") && !Page.IsPostBack) || ((pageAction == "" || pageAction == "removealias") && Page.IsPostBack && Request.Form[isCPostData.UniqueID] != ""))
        {
            LoadRegExlAliasList(siteID.ToString());
        }

    }

    private void AddDefaultMenuBar(string siteID)
    {

        bool canAddMenu = true;
        divTitleBar.InnerHtml = _refStyle.GetTitleBar(msgHelper.GetMessage("lbl regex aliased page name maintenance"));
        System.Text.StringBuilder result = new System.Text.StringBuilder();
        long curr_sitekey;


        result.Append("<table><tr>" + "\r\n");
        if (canAddMenu)
        {
            if (_aliasSettingsApi.IsRegExAliasEnabled)
            {
                result.Append(_refStyle.GetButtonEventsWCaption(_refCommonApi.AppPath + "images/UI/Icons/add.png", "#", msgHelper.GetMessage("msg add regex"), msgHelper.GetMessage("msg add regex"), "Onclick=\"javascript: addRegex();\"", StyleHelper.AddButtonCssClass, true));
                result.Append(_refStyle.GetButtonEventsWCaption(_refCommonApi.AppPath + "images/UI/Icons/delete.png", "#", msgHelper.GetMessage("msg delete regex"), msgHelper.GetMessage("msg remove regex"), "Onclick=\"javascript: removeRegex();\"", StyleHelper.DeleteButtonCssClass));
				result.Append(_refStyle.GetButtonEventsWCaption(_refCommonApi.AppPath + "images/ui/icons/driveDelete.png", "#", msgHelper.GetMessage("alt clear regex cache"), msgHelper.GetMessage("btn clear cache"), "Onclick=\"javascript: clearCache();\"", StyleHelper.DeleteDriveButtonCssClass));
            }
        }

		result.Append(StyleHelper.ActionBarDivider);
        result.Append("<td class=\"label\">&#160;" + msgHelper.GetMessage("lbl site") + ":&nbsp;<select name=\"siteSearchItem\" id=\"siteSearchItem\" ONCHANGE=\"SubmitForm(\'form1\',\'\');\"/>&nbsp;<br>");

        siteDictionary = _manualAliaslist.GetSiteList();
        if (Page.IsPostBack)
        {
            long.TryParse(Request.Form["siteSearchItem"], out curr_sitekey);
            foreach (System.Collections.Generic.KeyValuePair<long, string> tempLoopVar_siteList in siteDictionary)
            {
                siteList = tempLoopVar_siteList;
                if (siteList.Key == curr_sitekey)
                {
                    result.Append("<option value=\"" + siteList.Key.ToString() + "\">" + siteList.Value + "</option>");
                    break;
                }
            }
            foreach (System.Collections.Generic.KeyValuePair<long, string> tempLoopVar_siteList in siteDictionary)
            {
                siteList = tempLoopVar_siteList;
                if (siteList.Key != curr_sitekey)
                {
                    result.Append("<option value=\"" + siteList.Key.ToString() + "\">" + siteList.Value + "</option>");
                }
            }
        }
        else if (siteID != "")
        {
            long.TryParse(siteID, out curr_sitekey);
            foreach (System.Collections.Generic.KeyValuePair<long, string> tempLoopVar_siteList in siteDictionary)
            {
                siteList = tempLoopVar_siteList;
                if (siteList.Key == curr_sitekey)
                {
                    result.Append("<option value=\"" + siteList.Key.ToString() + "\">" + siteList.Value + "</option>");
                    break;
                }
            }
            foreach (System.Collections.Generic.KeyValuePair<long, string> tempLoopVar_siteList in siteDictionary)
            {
                siteList = tempLoopVar_siteList;
                if (siteList.Key != curr_sitekey)
                {
                    result.Append("<option value=\"" + siteList.Key.ToString() + "\">" + siteList.Value + "</option>");
                }
            }
        }
        else
        {
            foreach (System.Collections.Generic.KeyValuePair<long, string> tempLoopVar_siteList in siteDictionary)
            {
                siteList = tempLoopVar_siteList;
                result.Append("<option value=\"" + siteList.Key.ToString() + "\">" + siteList.Value + "</option>");
            }
        }
		result.Append(StyleHelper.ActionBarDivider);
        CollectSearchText();
        result.Append("<td class=\"label\"><label for=\"txtSearch\">" + msgHelper.GetMessage("generic search") + "</label>");
        result.Append("<input type=\"text\" size=\"25\" class=\"ektronTextXXSmall\" id=\"txtSearch\" name=\"txtSearch\" value=\"" + strKeyWords + "\" onkeydown=\"CheckForReturn(event)\" autocomplete=\"off\">");
        result.Append("<select id=\"searchlist\" name=\"searchlist\">");
        result.Append("<option value=" + EkEnumeration.RegExAliasSearchField.All.ToString() + IsSelected("" + EkEnumeration.RegExAliasSearchField.All.ToString() + "") + ">All</option>");
        result.Append("<option value=" + EkEnumeration.RegExAliasSearchField.ExpressionName.ToString() + IsSelected("" + EkEnumeration.RegExAliasSearchField.ExpressionName.ToString() + "") + ">Expression Name</option>");
        result.Append("</select><input type=button value=" + msgHelper.GetMessage("btn search") + " title=" + msgHelper.GetMessage("btn search") + " id=\"btnSearch\" name=\"btnSearch\" onclick=\"searchuser();\" class=\"ektronWorkareaSearch\" title=\"Search Users\" />");
		result.Append("</td>");
		result.Append(StyleHelper.ActionBarDivider);
		result.Append("<td>" + _refStyle.GetHelpButton("RegExMaint", "") + "</td>");
        result.Append("</tr></table>");
        divToolBar.InnerHtml = result.ToString();
        result = null;
        StyleSheetJS.Text = (new StyleHelper()).GetClientScript();
    }
    private void AddDeleteMenuBar()
    {
        divTitleBar.InnerHtml = _refStyle.GetTitleBar(msgHelper.GetMessage("msg remove regex"));
        System.Text.StringBuilder result = new System.Text.StringBuilder();
        result.Append("<table><tr>" + "\r\n");
		result.Append(_refStyle.GetButtonEventsWCaption(_refCommonApi.AppPath + "images/UI/Icons/back.png", "urlregexaliaslistmaint.aspx", msgHelper.GetMessage("alt back button text"), msgHelper.GetMessage("btn back"), "", StyleHelper.BackButtonCssClass, true));
		result.Append(_refStyle.GetButtonEventsWCaption(_refCommonApi.AppPath + "images/UI/Icons/delete.png", "#", msgHelper.GetMessage("lbl delete selected regex"), msgHelper.GetMessage("lbl delete regex"), "Onclick=\"javascript: SubmitForm(\'form1\',\'ConfirmDelete()\');\"", StyleHelper.DeleteDriveButtonCssClass, true));
		result.Append(StyleHelper.ActionBarDivider); 
		result.Append("<td>" + _refStyle.GetHelpButton("RemoveRegEx", "") + "</td>");
        result.Append("</tr></table>");
        divToolBar.InnerHtml = result.ToString();
        result = null;
        StyleSheetJS.Text = (new StyleHelper()).GetClientScript();
    }
    private void DeleteRegex(string siteId)
    {
        if (Page.IsPostBack && Request.Form[isCPostData.UniqueID] != "")
        {
            string strRegExIds = "";
            int i;
            string[] values;
            strRegExIds = Request.Form["deleteRegexId"];
            if (!(strRegExIds == null))
            {
                values = strRegExIds.Split(",".ToCharArray());
                long RegexID = 0;
                for (i = 0; i <= values.Length - 1; i++)
                {
                    long.TryParse(values[i], out RegexID);
                    try
                    {
                        _regexAliaslist.Delete(RegexID, _refContentApi.UserId);
                    }
                    catch (Exception ex)
                    {
                        Response.Redirect((string)("reterror.aspx?info=" + EkFunctions.UrlEncode(ex.Message) + "&LangType=" + contentLanguage), false);
                        return;
                    }
                }
            }
            Response.Redirect((string)("urlregexaliaslistmaint.aspx?fId=" + siteId), false);
        }
    }
    private void LoadRegExlAliasList(string siteID)
    {
        string defaultIcon = string.Empty;
        PagingInfo req = new PagingInfo();
        bool _isRemoveAlias = false;
        long currentSiteKey = 0;
        System.Collections.Generic.List<UrlAliasRegExData> mregexaliaslist;
        Ektron.Cms.Common.EkEnumeration.RegExOrderBy orderBy = Ektron.Cms.Common.EkEnumeration.RegExOrderBy.None;

        req = new PagingInfo(_refContentApi.RequestInformationRef.PagingSize);
        req.CurrentPage = _currentPageNumber;


        if (!String.IsNullOrEmpty(Request.QueryString["orderby"]))
        {
            orderBy = (EkEnumeration.RegExOrderBy)Enum.Parse(typeof(EkEnumeration.RegExOrderBy), Convert.ToString(Request.Form["orderby"]), true);
        }

        if (Request.QueryString["action"] == "removealias")
        {
            _isRemoveAlias = true;
        }

        if (Page.IsPostBack)
        {
            long.TryParse((string)(Request.Form["siteSearchItem"]), out currentSiteKey);
        }
        else if (_isRemoveAlias || siteID != "")
        {
            long.TryParse(siteID, out currentSiteKey);
        }
        else
        {
            siteDictionary = _manualAliaslist.GetSiteList();
            foreach (System.Collections.Generic.KeyValuePair<long, string> tempLoopVar_siteList in siteDictionary)
            {
                siteList = tempLoopVar_siteList;
                long.TryParse(siteList.Key.ToString(), out currentSiteKey);
                break;
            }
        }

        strKeyWords = String.IsNullOrEmpty(Request.Form["txtSearch"]) ? "" : Request.Form["txtSearch"];
        if (!String.IsNullOrEmpty(Request.Form["searchlist"]))
        {
            strSelectedItem = (EkEnumeration.RegExAliasSearchField)Enum.Parse(typeof(EkEnumeration.RegExAliasSearchField), Convert.ToString(Request.Form["searchlist"]), true);
        }

        mregexaliaslist = _regexAliaslist.GetList(req, currentSiteKey, false, strSelectedItem, strKeyWords, orderBy);
        TotalPagesNumber = req.TotalPages;
        PageSettings();

        if ((mregexaliaslist != null) && mregexaliaslist.Count > 0)
        {
            if (_isRemoveAlias)
            {
                CollectionListGrid.Columns.Add(_refStyle.CreateBoundField("DELETE", "Delete", "title-header", HorizontalAlign.Left, HorizontalAlign.NotSet, Unit.Percentage(3), Unit.Percentage(3), false, false));
                CollectionListGrid.Columns.Add(_refStyle.CreateBoundField("ACTIVE", "<a href=\"urlregexaliaslistmaint.aspx?orderby=" + EkEnumeration.RegExOrderBy.Active + "&action=removealias&fId=" + currentSiteKey.ToString() + "\">Active</a>", "title-header", HorizontalAlign.Left, HorizontalAlign.NotSet, Unit.Percentage(5), Unit.Percentage(5), false, false));
                CollectionListGrid.Columns.Add(_refStyle.CreateBoundField("ORDER", "<a href=\"urlregexaliaslistmaint.aspx?orderby=" + EkEnumeration.RegExOrderBy.Priority + "&action=removealias&fId=" + currentSiteKey.ToString() + "\">" + msgHelper.GetMessage("lbl priority") + "</a>", "title-header", HorizontalAlign.Left, HorizontalAlign.NotSet, Unit.Percentage(5), Unit.Percentage(5), false, false));
                CollectionListGrid.Columns.Add(_refStyle.CreateBoundField("EXPRESSION", "<a href=\"urlregexaliaslistmaint.aspx?orderby=" + EkEnumeration.RegExOrderBy.ExpressionName + "&action=removealias&fId=" + currentSiteKey.ToString() + "\">Expression Name</a>", "title-header", HorizontalAlign.Left, HorizontalAlign.NotSet, Unit.Percentage(35), Unit.Percentage(35), false, false));
                CollectionListGrid.Columns.Add(_refStyle.CreateBoundField("TARGET", "Example URL", "title-header", HorizontalAlign.Left, HorizontalAlign.NotSet, Unit.Percentage(35), Unit.Percentage(35), false, false));
            }
            else
            {
                CollectionListGrid.Columns.Add(_refStyle.CreateBoundField("ACTIVE", "<a href=\"urlregexaliaslistmaint.aspx?orderby=" + EkEnumeration.RegExOrderBy.Active + "&fId=" + currentSiteKey.ToString() + "\">Active</a>", "title-header", HorizontalAlign.Left, HorizontalAlign.NotSet, Unit.Percentage(5), Unit.Percentage(5), false, false));
                CollectionListGrid.Columns.Add(_refStyle.CreateBoundField("ORDER", "<a href=\"urlregexaliaslistmaint.aspx?orderby=" + EkEnumeration.RegExOrderBy.Priority + "&&fId=" + currentSiteKey.ToString() + "\">" + msgHelper.GetMessage("lbl priority") + "</a>", "title-header", HorizontalAlign.Left, HorizontalAlign.NotSet, Unit.Percentage(5), Unit.Percentage(5), false, false));
                CollectionListGrid.Columns.Add(_refStyle.CreateBoundField("EXPRESSION", "<a href=\"urlregexaliaslistmaint.aspx?orderby=" + EkEnumeration.RegExOrderBy.ExpressionName + "&fId=" + currentSiteKey.ToString() + "\">Expression Name</a>", "title-header", HorizontalAlign.Left, HorizontalAlign.NotSet, Unit.Percentage(35), Unit.Percentage(35), false, false));
                CollectionListGrid.Columns.Add(_refStyle.CreateBoundField("TARGET", "Example URL", "title-header", HorizontalAlign.Left, HorizontalAlign.NotSet, Unit.Percentage(35), Unit.Percentage(35), false, false));
            }

            DataTable dt = new DataTable();
            DataRow dr;
            if (_isRemoveAlias)
            {
                dt.Columns.Add(new DataColumn("DELETE", typeof(string)));
            }
            dt.Columns.Add(new DataColumn("EXPRESSION", typeof(string)));
            dt.Columns.Add(new DataColumn("TARGET", typeof(string)));
            dt.Columns.Add(new DataColumn("ACTIVE", typeof(string)));
            dt.Columns.Add(new DataColumn("ORDER", typeof(string)));

            for (int i = 0; i <= mregexaliaslist.Count - 1; i++)
            {
                dr = dt.NewRow();
                if (_isRemoveAlias)
                {
                    dr["DELETE"] = "<input type=\"checkbox\" name=\"deleteRegexId\" value=\"" + mregexaliaslist[i].RegExId.ToString() + "\">";
                }
                dr["EXPRESSION"] = "<a href=\"urlregexaliasmaint.aspx?action=view&id=" + mregexaliaslist[i].RegExId.ToString() + "&fId=" + currentSiteKey.ToString() + "\">" + mregexaliaslist[i].ExpressionName + "</a>";
                dr["TARGET"] = mregexaliaslist[i].TransformedUrl;
                if (mregexaliaslist[i].IsEnabled)
                {
                    dr["ACTIVE"] = "<center style=\"color:green\">On</center>"; //maliaslist(i).IsEnabled"
                }
                else
                {
                    dr["ACTIVE"] = "<center style=\"color:red\">Off</center>";
                }
                dr["ORDER"] = "<center>" + mregexaliaslist[i].Priority.ToString() + "</center>";

                dt.Rows.Add(dr);
            }
            DataView dv = new DataView(dt);
            CollectionListGrid.DataSource = dv;
            CollectionListGrid.DataBind();
        }
    }

    private void PageSettings()
    {
        if (TotalPagesNumber <= 1)
        {
            VisiblePageControls(false);
        }
        else
        {
            VisiblePageControls(true);
            TotalPages.Text = (System.Math.Ceiling((double)TotalPagesNumber)).ToString();
            TotalPages.ToolTip = TotalPages.Text;
            CurrentPage.Text = _currentPageNumber.ToString();
            CurrentPage.ToolTip = CurrentPage.Text;
            PreviousPage1.Enabled = true;
            FirstPage.Enabled = true;
            NextPage.Enabled = true;
            LastPage.Enabled = true;
            if (_currentPageNumber == 1)
            {
                PreviousPage1.Enabled = false;
                FirstPage.Enabled = false;
            }
            else if (_currentPageNumber == TotalPagesNumber)
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
    public void NavigationLink_Click(object sender, CommandEventArgs e)
    {
        switch (e.CommandName)
        {
            case "First":
                _currentPageNumber = 1;
                break;
            case "Last":
                _currentPageNumber = int.Parse((string)TotalPages.Text);
                break;
            case "Next":
                _currentPageNumber = System.Convert.ToInt32(int.Parse((string)CurrentPage.Text) + 1);
                break;
            case "Prev":
                _currentPageNumber = System.Convert.ToInt32(int.Parse((string)CurrentPage.Text) - 1);
                break;
        }
        LoadRegExlAliasList(string.Empty);
        isCPostData.Value = "true";
    }
    private void CollectSearchText()
    {
        strKeyWords = String.IsNullOrEmpty(Request.Form["txtSearch"]) ? "" : Request.Form["txtSearch"];
        if (!String.IsNullOrEmpty((Request.Form["searchlist"])))
        {
            strSelectedItem = (EkEnumeration.RegExAliasSearchField)Enum.Parse(typeof(EkEnumeration.RegExAliasSearchField), Convert.ToString(Request.Form["searchlist"]), true);
        }
    }
    private string IsSelected(string val)
    {
        if (val == strSelectedItem.ToString())
        {
            return (" selected ");
        }
        else
        {
            return ("");
        }
    }
    private void ClearCache(string siteId)
    {
        _regexAliaslist.ClearCache();
        Response.Redirect((string)("urlregexaliaslistmaint.aspx?fId=" + siteId));
    }
    private void RegisterResources()
    {
        Ektron.Cms.API.Css.RegisterCss(this, Ektron.Cms.API.Css.ManagedStyleSheet.EktronFixedPositionToolbarCss);
        Ektron.Cms.API.JS.RegisterJS(this, Ektron.Cms.API.JS.ManagedScript.EktronJS);
        Ektron.Cms.API.JS.RegisterJS(this, Ektron.Cms.API.JS.ManagedScript.EktronStyleHelperJS);
        Ektron.Cms.API.JS.RegisterJS(this, Ektron.Cms.API.JS.ManagedScript.EktronToolBarRollJS);
        Ektron.Cms.API.JS.RegisterJS(this, Ektron.Cms.API.JS.ManagedScript.EktronJFunctJS);
        Ektron.Cms.API.JS.RegisterJS(this, Ektron.Cms.API.JS.ManagedScript.EktronInputLabelJS);
        Ektron.Cms.API.JS.RegisterJS(this, _refContentApi.AppPath + "java/ektron.workarea.searchBox.inputLabelInit.js", "EktronWorkareaSearchInputLabelInitJS");
    }
    private void SetServerJSVariables()
    {
        ltr_selItem.Text = msgHelper.GetMessage("alert msg select item");
        ltr_selRegex.Text = msgHelper.GetMessage("alert msg del sel regex");
    }
}