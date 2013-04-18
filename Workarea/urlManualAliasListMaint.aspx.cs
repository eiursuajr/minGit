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

public partial class Workarea_urlmanualaliaslistmaint : System.Web.UI.Page
{
    private UrlAliasManualApi _manualAliasList;
    private UrlAliasAutoApi _autoAliasList;
    private UrlAliasCommunityApi _communityAliasList;
    private int currentPageNumber = 1;
    private int totalPagesNumber = 1;
    private int contentLanguage;
    private EkEnumeration.UrlAliasSearchField _strSelectedItem = Ektron.Cms.Common.EkEnumeration.UrlAliasSearchField.None;
    private EkEnumeration.AutoAliasSearchField _autoSelectedItem = Ektron.Cms.Common.EkEnumeration.AutoAliasSearchField.None;
    private string strKeyWords = string.Empty;
    private System.Collections.Generic.Dictionary<long, string> siteDictionary;
    private System.Collections.Generic.KeyValuePair<long, string> siteList;
    protected StyleHelper _refStyle = new StyleHelper();
    protected EkMessageHelper msgHelper;
    protected CommonApi _refCommonApi = new CommonApi();
    protected ContentAPI _refContentApi = new ContentAPI();

    protected void Page_Load(object sender, System.EventArgs e)
    {

        msgHelper = _refCommonApi.EkMsgRef;
        Ektron.Cms.Content.EkContent objContentRef;
        objContentRef = _refContentApi.EkContentRef;
        RegisterResources();
        SetServerJSVariables();
        string pageAction = "";
        long siteID = 0;

        //Licensing For 7.6
        if (!Ektron.Cms.DataIO.LicenseManager.LicenseManager.IsFeatureEnable(_refContentApi.RequestInformationRef, Ektron.Cms.DataIO.LicenseManager.Feature.UrlAliasing, false))
        {
            Utilities.ShowError(_refContentApi.EkMsgRef.GetMessage("feature locked error"));
            return;
        }
        if (!Utilities.ValidateUserLogin())
        {
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

        _manualAliasList = new Ektron.Cms.UrlAliasing.UrlAliasManualApi();
        _autoAliasList = new Ektron.Cms.UrlAliasing.UrlAliasAutoApi();
        _communityAliasList = new Ektron.Cms.UrlAliasing.UrlAliasCommunityApi();
        siteDictionary = new System.Collections.Generic.Dictionary<long, string>();

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

        if (!String.IsNullOrEmpty(Request.QueryString["LangType"]))
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
        if (pageAction == "removealias")
        {
            AddDeleteMenuBar(siteID.ToString());
            DeleteAlias(siteID.ToString());
        }
        else if (pageAction == "refresh")
        {
            RefreshManualAliasList(siteID.ToString());
            return;
        }
        else if ((string)(pageAction) == "clearcache")
        {
            ClearAutoAliasCache(siteID.ToString());
            return;
        }
        else if ((string)(pageAction) == "clearcommunitycache")
        {
            ClearCommunityCache(siteID.ToString());
            return;
        }
        else if ((string)(pageAction) == "")
        {
            AddDefaultMenuBar(siteID.ToString());
        }

        if (((pageAction == "" || pageAction == "removealias") && !Page.IsPostBack) || ((pageAction == "" || pageAction == "removealias") && Page.IsPostBack && Request.Form[isCPostData.UniqueID] != ""))
        {
            if (siteID > 0)
            {
                LoadManualAliasList(siteID.ToString());
            }
            else
            {
                LoadManualAliasList(string.Empty);
            }
        }
    }

    private void AddDefaultMenuBar(string siteID)
    {
        Ektron.Cms.UrlAliasing.UrlAliasSettingsApi _aliasSettingsApi = new Ektron.Cms.UrlAliasing.UrlAliasSettingsApi();
        System.Text.StringBuilder result = new System.Text.StringBuilder();
        result.Append("<table><tr>" + "\r\n");
        if (Request.QueryString["mode"] == "auto")
        {
            txtTitleBar.InnerHtml = _refStyle.GetTitleBar(msgHelper.GetMessage("lbl auto aliased page name maintenance"));

            if (_aliasSettingsApi.IsAutoAliasEnabled)
            {
                result.Append(_refStyle.GetButtonEventsWCaption(_refCommonApi.AppPath + "images/ui/icons/add.png", "#", msgHelper.GetMessage("msg add alias"), msgHelper.GetMessage("msg add alias"), "onclick=\" addAlias(\'" + contentLanguage.ToString() + "\',\'auto\');\"", StyleHelper.AddButtonCssClass, true));
                result.Append(_refStyle.GetButtonEventsWCaption(_refCommonApi.AppPath + "images/ui/icons/driveDelete.png", "#", msgHelper.GetMessage("alt clear automatic alias cache"), msgHelper.GetMessage("btn clear cache"), "onclick=\" clearCache(\'auto\');\"", StyleHelper.DeleteDriveButtonCssClass));
                result.Append(_refStyle.GetButtonEventsWCaption(_refCommonApi.AppPath + "images/ui/icons/delete.png", "#", msgHelper.GetMessage("msg delete alias"), msgHelper.GetMessage("msg delete alias"), "onclick=\" removeAlias(\'auto\');\"", StyleHelper.DeleteButtonCssClass));
            }
        }
        else if (Request.QueryString["mode"] == "community")
        {
            txtTitleBar.InnerHtml = _refStyle.GetTitleBar(msgHelper.GetMessage("lbl community aliased page name maintenance"));

            if (_aliasSettingsApi.IsCommunityAliasingEnabled)
            {
                if (contentLanguage != -1)
                {
					result.Append(_refStyle.GetButtonEventsWCaption(_refCommonApi.AppPath + "images/ui/icons/add.png", "#", msgHelper.GetMessage("msg add alias"), msgHelper.GetMessage("msg add alias"), "onclick=\" addAlias(\'" + contentLanguage.ToString() + "\',\'community\');\"", StyleHelper.AddButtonCssClass, true));
                }
				result.Append(_refStyle.GetButtonEventsWCaption(_refCommonApi.AppPath + "images/ui/icons/driveDelete.png", "#", msgHelper.GetMessage("alt clear manual alias cache"), msgHelper.GetMessage("btn clear cache"), "onclick=\" clearCache(\'community\');\"", StyleHelper.DeleteDriveButtonCssClass));
                if (contentLanguage != -1)
                {
					result.Append(_refStyle.GetButtonEventsWCaption(_refCommonApi.AppPath + "images/ui/icons/delete.png", "#", msgHelper.GetMessage("msg delete alias"), msgHelper.GetMessage("msg delete alias"), "onclick=\" removeAlias(\'community\');\"", StyleHelper.DeleteButtonCssClass));
                }
            }
        }
        else
        {
            txtTitleBar.InnerHtml = _refStyle.GetTitleBar(msgHelper.GetMessage("lbl manual aliased page name maintenance"));

            if (_aliasSettingsApi.IsManualAliasEnabled)
            {
				bool primaryCssApplied = false;

                if (contentLanguage != -1)
                {
					result.Append(_refStyle.GetButtonEventsWCaption(_refCommonApi.AppPath + "images/ui/icons/add.png", "#", msgHelper.GetMessage("msg add alias"), msgHelper.GetMessage("msg add alias"), "onclick=\" addAlias(\'" + contentLanguage.ToString() + "\',\'manual\');\"", StyleHelper.AddButtonCssClass, !primaryCssApplied));

					primaryCssApplied = true;
                }

				result.Append(_refStyle.GetButtonEventsWCaption(_refCommonApi.AppPath + "images/ui/icons/driveDelete.png", "#", msgHelper.GetMessage("alt clear manual alias cache"), msgHelper.GetMessage("btn clear cache"), "onclick=\" clearCache(\'manual\');\"", StyleHelper.DeleteDriveButtonCssClass, !primaryCssApplied));

				primaryCssApplied = true;
				
				if (contentLanguage != -1)
                {
                    result.Append(_refStyle.GetButtonEventsWCaption(_refCommonApi.AppPath + "images/ui/icons/delete.png", "#", msgHelper.GetMessage("msg delete alias"), msgHelper.GetMessage("msg delete alias"), "onclick=\" removeAlias();\"", StyleHelper.DeleteButtonCssClass));
                }
            }
        }
        if (_aliasSettingsApi.IsAutoAliasEnabled || _aliasSettingsApi.IsManualAliasEnabled)
        {
			
        }
        if (Request.QueryString["mode"] != "community")
        {
			result.Append(StyleHelper.ActionBarDivider);

            result.Append("<td class=\"label\">");
            result.Append("&nbsp;" + msgHelper.GetMessage("view in label") + ":&nbsp;" + _refStyle.ShowAllActiveLanguage(true, "", "", contentLanguage.ToString()));
            result.Append("</td>");
        }
        ///' Hiding the following select dropwodn as to hide the site '''''
        if (Request.QueryString["mode"] == "community")
        {
            result.Append("<td class=\"label\" style=\"display: none !important;\">&nbsp;");
        }
        else
        {
            result.Append("<td class=\"label\">&nbsp;");
        }
        result.Append(msgHelper.GetMessage("lbl site"));
        result.Append("</td>");
        if (Request.QueryString["mode"] == "community")
        {
            result.Append("<td class=\"label\" style=\"display: none !important;\">");
        }
        else
        {
            result.Append("<td class=\"label\">");
        }
        result.Append("<select name=\"siteSearchItem\" id=\"siteSearchItem\" ONCHANGE=\"SubmitForm(\'form1\',\'\');\"/>&nbsp;");
        siteDictionary = _manualAliasList.GetSiteList();

        if (Page.IsPostBack)
        {
            long curr_sitekey = System.Convert.ToInt64(Request.Form["siteSearchItem"]);
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
            foreach (System.Collections.Generic.KeyValuePair<long, string> tempLoopVar_siteList in siteDictionary)
            {
                siteList = tempLoopVar_siteList;
                if (siteList.Key == long.Parse(siteID))
                {
                    result.Append("<option value=\"" + siteList.Key.ToString() + "\">" + siteList.Value + "</option>");
                    break;
                }
            }
            foreach (System.Collections.Generic.KeyValuePair<long, string> tempLoopVar_siteList in siteDictionary)
            {
                siteList = tempLoopVar_siteList;
                if (siteList.Key != long.Parse(siteID))
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

        result.Append("</select>");
        result.Append("</td>");
        if (Request.QueryString["mode"] == "auto")
        {
            CollectSearchText();

			result.Append(StyleHelper.ActionBarDivider);

            result.Append("<td class=\"label\">");
            result.Append("<label for=\'txtSearch\'>" + msgHelper.GetMessage("btn search") + "</label>");
            result.Append("</td>");
            result.Append("<td>");
            result.Append("<input type=\"text\" class=\"ektronTextXXSmall\" id=\"txtSearch\" name=\"txtSearch\" value=\"" + strKeyWords + "\" autocomplete=\"off\" onkeydown=\"CheckForReturn(event)\">");
            result.Append("<select id=\"searchlist\" name=\"searchlist\">");
            result.Append("<option value=" + EkEnumeration.AutoAliasSearchField.All.ToString() + IsSelected("" + EkEnumeration.AutoAliasSearchField.All.ToString() + "") + ">All</option>");
            result.Append("<option value=" + EkEnumeration.AutoAliasSearchField.SourceName.ToString() + IsSelected("" + EkEnumeration.AutoAliasSearchField.SourceName.ToString() + "") + ">Source Name</option>");
            result.Append("</select>");
            result.Append("</td>");
            result.Append("<td><input type=\"image\" src=\"" + _refContentApi.AppPath + "images/ui/icons/magnifier.png\" value=" + msgHelper.GetMessage("btn search") + " alt=\"" + msgHelper.GetMessage("btn search") + "\" title=\"" + msgHelper.GetMessage("btn search") + "\" id=\"btnSearch\" name=\"btnSearch\" onclick=\"searchuser();return false;\" style=\"margin-left: .25em\" title=\"Search Users\" /></td>");
        }
        else if (Request.QueryString["mode"] == "community")
        {

        }
        else
        {
            CollectSearchText();

			result.Append(StyleHelper.ActionBarDivider);

            result.Append("<td class=\"label\">");
            result.Append("<label for=\'txtSearch\'>" + msgHelper.GetMessage("btn search") + "</label>");
            result.Append("</td>");
            result.Append("<td>");
            result.Append("<input type=\"text\" id=\"txtSearch\" class=\"ektronTextXXSmall\" name=\"txtSearch\" value=\"" + strKeyWords + "\" autocomplete=\"off\" onkeydown=\"CheckForReturn(event)\">");
            result.Append("<select id=\"searchlist\" name=\"searchlist\">");
            result.Append("<option value=" + EkEnumeration.UrlAliasSearchField.All.ToString() + IsSelected("" + EkEnumeration.UrlAliasSearchField.All.ToString() + "") + ">All</option>");
            result.Append("<option value=" + EkEnumeration.UrlAliasSearchField.Alias.ToString() + IsSelected("" + EkEnumeration.UrlAliasSearchField.Alias.ToString() + "") + ">Alias</option>");
            result.Append("<option value=" + EkEnumeration.UrlAliasSearchField.ContentId.ToString() + IsSelected("" + EkEnumeration.UrlAliasSearchField.ContentId.ToString() + "") + ">Content ID</option>");
            result.Append("<option value=" + EkEnumeration.UrlAliasSearchField.ContentTitle.ToString() + IsSelected("" + EkEnumeration.UrlAliasSearchField.ContentTitle.ToString() + "") + ">Content Title</option>");
            result.Append("<option value=" + EkEnumeration.UrlAliasSearchField.Target.ToString() + IsSelected("" + EkEnumeration.UrlAliasSearchField.Target.ToString() + "") + ">Original Link</option>");
            result.Append("</select>");
            result.Append("</td>");
            result.Append("<td><input type=\"image\" src=\"" + _refContentApi.AppPath + "images/ui/icons/magnifier.png\" value=" + msgHelper.GetMessage("btn search") + " alt=\"" + msgHelper.GetMessage("btn search") + "\" title=\"" + msgHelper.GetMessage("btn search") + "\" id=\"btnSearch\" name=\"btnSearch\" onclick=\"searchuser();return false;\" style=\"margin-left: .25em\" title=\"Search Users\" /></td>");
        }

		result.Append(StyleHelper.ActionBarDivider);

        if (Request.QueryString["mode"] == "auto")
        {
            result.Append("<td>" + _refStyle.GetHelpButton("AutoAliasPageMaint", "") + "</td>");
        }
        else if (Request.QueryString["mode"] == "community")
        {
            result.Append("<td>" + _refStyle.GetHelpButton("CommunityAliasPageMaint", "") + "</td>");
        }
        else
        {
            result.Append("<td>" + _refStyle.GetHelpButton("ManAliasPageMaint", "") + "</td>");
        }
        result.Append("</tr></table>");
        htmToolBar.InnerHtml = result.ToString();
        result = null;
        StyleSheetJS.Text = (new StyleHelper()).GetClientScript();
    }
    private string IsSelected(string val)
    {
        if (Request.QueryString["mode"] == "auto")
        {
            if (val == Convert.ToString((int)_autoSelectedItem))
            {
                return (" selected ");
            }
            else
            {
                return ("");
            }
        }
        else
        {
            if (val == Convert.ToString((int)_autoSelectedItem))
            {
                return (" selected ");
            }
            else
            {
                return ("");
            }
        }

    }
    private void AddDeleteMenuBar(string siteId)
    {
        txtTitleBar.InnerHtml = _refStyle.GetTitleBar(msgHelper.GetMessage("msg delete alias"));
        System.Text.StringBuilder result = new System.Text.StringBuilder();
        result.Append("<table><tr>" + "\r\n");
        string deleteButton = _refStyle.GetButtonEventsWCaption(_refCommonApi.AppPath + "images/ui/icons/delete.png", "#", msgHelper.GetMessage("lbl delete selected aliases"), msgHelper.GetMessage("lbl delete aliases"), "onclick=\" SubmitForm(\'form1\',\'ConfirmDelete()\');return false;\"", StyleHelper.DeleteButtonCssClass, true);
        if (Request.QueryString["mode"] == "auto")
        {
            txtTitleBar.InnerHtml = _refStyle.GetTitleBar(msgHelper.GetMessage("msg delete alias"));
            result.Append(_refStyle.GetButtonEventsWCaption(_refCommonApi.AppPath + "images/ui/icons/back.png", (string)("urlmanualaliaslistmaint.aspx?mode=auto&fId=" + siteId), msgHelper.GetMessage("alt back button text"), msgHelper.GetMessage("btn back"), "", StyleHelper.BackButtonCssClass, true));
			result.Append(deleteButton);
			result.Append("<td>" + _refStyle.GetHelpButton("RemoveAutoAlias", "") + "</td>");
        }
        else if (Request.QueryString["mode"] == "community")
        {
            txtTitleBar.InnerHtml = _refStyle.GetTitleBar(msgHelper.GetMessage("msg delete alias"));
			result.Append(_refStyle.GetButtonEventsWCaption(_refCommonApi.AppPath + "images/ui/icons/back.png", (string)("urlmanualaliaslistmaint.aspx?mode=community&fId=" + siteId), msgHelper.GetMessage("alt back button text"), msgHelper.GetMessage("btn back"), "", StyleHelper.BackButtonCssClass, true));
			result.Append(deleteButton); 
			result.Append("<td>" + _refStyle.GetHelpButton("RemoveCommunityAlias", "") + "</td>");
        }
        else
        {
			result.Append(_refStyle.GetButtonEventsWCaption(_refCommonApi.AppPath + "images/ui/icons/back.png", (string)("urlmanualaliaslistmaint.aspx?fId=" + siteId), msgHelper.GetMessage("alt back button text"), msgHelper.GetMessage("btn back"), "", StyleHelper.BackButtonCssClass, true));
			result.Append(deleteButton); 
			result.Append("<td>" + _refStyle.GetHelpButton("RemoveManAlias", "") + "</td>");
        }
        result.Append("</tr></table>");
        htmToolBar.InnerHtml = result.ToString();
        result = null;
        StyleSheetJS.Text = (new StyleHelper()).GetClientScript();

    }

    private void DeleteAlias(string siteId)
    {

        if (Page.IsPostBack && Request.Form[isCPostData.UniqueID] != "")
        {
            string strAliasIds = "";
            int i;
            string[] values;
            strAliasIds = Request.Form["deleteAliasId"];
            if (!(strAliasIds == null))
            {
                values = strAliasIds.Split(",".ToCharArray());
                long aliasID = 0;
                for (i = 0; i <= values.Length - 1; i++)
                {
                    long.TryParse(values[i], out aliasID);
                    try
                    {
                        if (Request.QueryString["mode"] == "auto")
                        {
                            _autoAliasList.Delete(aliasID, _refContentApi.UserId);
                        }
                        else if (Request.QueryString["mode"] == "community")
                        {
                            _communityAliasList.Delete(aliasID);
                        }
                        else
                        {
                            _manualAliasList.Delete(aliasID, _refContentApi.UserId);
                        }

                    }
                    catch (Exception ex)
                    {
                        Response.Redirect((string)("reterror.aspx?info=" + EkFunctions.UrlEncode(ex.Message) + "&LangType=" + contentLanguage), false);
                        return;
                    }
                }
            }
            if (Request.QueryString["mode"] == "auto")
            {
                Response.Redirect((string)("urlmanualaliaslistmaint.aspx?mode=auto&fId=" + siteId), false);
            }
            else if (Request.QueryString["mode"] == "community")
            {
                Response.Redirect((string)("urlmanualaliaslistmaint.aspx?mode=community&fId=" + siteId), false);
            }
            else
            {
                Response.Redirect((string)("urlmanualaliaslistmaint.aspx?fId=" + siteId), false);
            }
        }
    }
    private void PageSettings()
    {
        if (totalPagesNumber <= 1)
        {
            VisiblePageControls(false);
        }
        else
        {
            VisiblePageControls(true);
            TotalPages.Text = (System.Math.Ceiling((double)totalPagesNumber)).ToString();
            TotalPages.ToolTip = TotalPages.Text;
            CurrentPage.Text = currentPageNumber.ToString();
            CurrentPage.ToolTip = CurrentPage.Text;
            PreviousPage1.Enabled = true;
            FirstPage.Enabled = true;
            NextPage.Enabled = true;
            LastPage.Enabled = true;
            if (currentPageNumber == 1)
            {
                PreviousPage1.Enabled = false;
                FirstPage.Enabled = false;
            }
            else if (currentPageNumber == totalPagesNumber)
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
                currentPageNumber = 1;
                break;
            case "Last":
                currentPageNumber = int.Parse((string)TotalPages.Text);
                break;
            case "Next":
                currentPageNumber = System.Convert.ToInt32(int.Parse((string)CurrentPage.Text) + 1);
                break;
            case "Prev":
                currentPageNumber = System.Convert.ToInt32(int.Parse((string)CurrentPage.Text) - 1);
                break;
        }
        LoadManualAliasList(Request.QueryString["fId"]);
        isCPostData.Value = "true";
    }
    private void LoadManualAliasList(string siteID)
    {
        PagingInfo req = new PagingInfo();
        System.Collections.Generic.List<UrlAliasManualData> manualAliasList;
        System.Collections.Generic.List<UrlAliasAutoData> autoAliasList;
        System.Collections.Generic.List<UrlAliasCommunityData> communityAliasList;
        Ektron.Cms.Common.EkEnumeration.UrlAliasingOrderBy orderBy = Ektron.Cms.Common.EkEnumeration.UrlAliasingOrderBy.None;
        Ektron.Cms.Common.EkEnumeration.AutoAliasOrderBy orderAutoAlias = Ektron.Cms.Common.EkEnumeration.AutoAliasOrderBy.None;
        Ektron.Cms.Common.EkEnumeration.CommunityAliasOrderBy orderCommunityAlias = Ektron.Cms.Common.EkEnumeration.CommunityAliasOrderBy.None;
        LocalizationAPI objLocalizationApi = new LocalizationAPI();
        string defaultIcon = string.Empty;
        long currentSiteKey = 0;
        bool _isRemoveAlias = false;
        string mode = string.Empty;
        SiteAPI _refsiteApi = new SiteAPI();
        LanguageData[] languageData = _refsiteApi.GetAllActiveLanguages();
        string strSelectedLanguageName = "";
        string strName;

        req = new PagingInfo(_refContentApi.RequestInformationRef.PagingSize);
        req.CurrentPage = currentPageNumber;

        if (Request.QueryString["action"] == "removealias")
        {
            _isRemoveAlias = true;
        }


        if (Request.QueryString["mode"] == "auto" && !String.IsNullOrEmpty(Request.QueryString["orderby"]))
        {
            orderAutoAlias = (EkEnumeration.AutoAliasOrderBy)Enum.Parse(typeof(EkEnumeration.AutoAliasOrderBy), Convert.ToString(Request.QueryString["orderby"]), true);
        }
        else if (Request.QueryString["mode"] == "community" && !String.IsNullOrEmpty(Request.QueryString["orderby"]))
        {
            orderCommunityAlias = (EkEnumeration.CommunityAliasOrderBy)Enum.Parse(typeof(EkEnumeration.CommunityAliasOrderBy), Convert.ToString(Request.QueryString["orderby"]), true);
        }
        else if (!String.IsNullOrEmpty(Request.QueryString["orderby"]) && Request.QueryString["mode"] != "auto")
        {
            orderBy = (EkEnumeration.UrlAliasingOrderBy)Enum.Parse(typeof(EkEnumeration.UrlAliasingOrderBy), Convert.ToString(Request.QueryString["orderby"]), true);
        }
        if (Page.IsPostBack && (Request.Form["siteSearchItem"] != null))
        {
            long.TryParse((string)(Request.Form["siteSearchItem"]), out currentSiteKey);
        }
        else if (_isRemoveAlias || siteID != "")
        {
            long.TryParse(siteID, out currentSiteKey);
        }
        else
        {
            siteDictionary = _manualAliasList.GetSiteList();
            foreach (System.Collections.Generic.KeyValuePair<long, string> tempLoopVar_siteList in siteDictionary)
            {
                siteList = tempLoopVar_siteList;
                long.TryParse(siteList.Key.ToString(), out currentSiteKey);
                break;
            }
        }
        strKeyWords = Request.Form["txtSearch"];

        mode = Request.QueryString["mode"];

        if (mode == "auto")
        {
            if (!String.IsNullOrEmpty(Request.QueryString["searchlist"]))
            {
                _autoSelectedItem = (EkEnumeration.AutoAliasSearchField)Enum.Parse(typeof(EkEnumeration.AutoAliasSearchField), Convert.ToString(Request.QueryString["searchlist"]), true);
            }
            autoAliasList = _autoAliasList.GetList(req, currentSiteKey, Convert.ToInt32(_refContentApi.GetCookieValue("LastValidLanguageID")), _autoSelectedItem, strKeyWords, orderAutoAlias);
            totalPagesNumber = req.TotalPages;
            PageSettings();
            if ((autoAliasList != null) && autoAliasList.Count > 0)
            {
                if (_isRemoveAlias)
                {
                    CollectionListGrid.Columns.Add(_refStyle.CreateBoundField("DELETE", msgHelper.GetMessage("generic delete title"), "title-header", HorizontalAlign.Left, HorizontalAlign.NotSet, Unit.Percentage(3), Unit.Percentage(3), false, false));
                    CollectionListGrid.Columns.Add(_refStyle.CreateBoundField("ACTIVE", "<center><a href=\"urlmanualaliaslistmaint.aspx?orderby=" + EkEnumeration.AutoAliasOrderBy.Active + "&action=removealias&mode=auto&fId=" + currentSiteKey.ToString() + "\">" + msgHelper.GetMessage("lbl active") + "</a></center>", "title-header", HorizontalAlign.Left, HorizontalAlign.NotSet, Unit.Percentage(5), Unit.Percentage(5), false, false));
                    CollectionListGrid.Columns.Add(_refStyle.CreateBoundField("LANG", "<center><a href=\"urlmanualaliaslistmaint.aspx?orderby=" + EkEnumeration.AutoAliasOrderBy.ContentLanguage + "&mode=auto&fId=" + currentSiteKey.ToString() + "\">" + msgHelper.GetMessage("lbl language") + "</a></center>", "title-header", HorizontalAlign.Left, HorizontalAlign.NotSet, Unit.Percentage(4), Unit.Percentage(4), false, false));
                    CollectionListGrid.Columns.Add(_refStyle.CreateBoundField("SOURCENAME", "<a href=\"urlmanualaliaslistmaint.aspx?orderby=" + EkEnumeration.AutoAliasOrderBy.SourceName + "&action=removealias&mode=auto&fId=" + currentSiteKey.ToString() + "\">" + msgHelper.GetMessage("lbl source name") + "</a>", "title-header", HorizontalAlign.Left, HorizontalAlign.NotSet, Unit.Percentage(15), Unit.Percentage(15), false, false));
                    CollectionListGrid.Columns.Add(_refStyle.CreateBoundField("TYPE", "<center><a href=\"urlmanualaliaslistmaint.aspx?orderby=" + EkEnumeration.AutoAliasOrderBy.Type + "&action=removealias&mode=auto&fId=" + currentSiteKey.ToString() + "\">" + msgHelper.GetMessage("type label") + "</a></center>", "title-header", HorizontalAlign.Left, HorizontalAlign.NotSet, Unit.Percentage(4), Unit.Percentage(4), false, false));
                    CollectionListGrid.Columns.Add(_refStyle.CreateBoundField("ALIAS", msgHelper.GetMessage("lbl example alias"), "title-header", HorizontalAlign.Left, HorizontalAlign.NotSet, Unit.Percentage(25), Unit.Percentage(25), false, false));

                }
                else
                {
                    CollectionListGrid.Columns.Add(_refStyle.CreateBoundField("ACTIVE", "<center><a href=\"urlmanualaliaslistmaint.aspx?orderby=" + EkEnumeration.AutoAliasOrderBy.Active + "&mode=auto&fId=" + currentSiteKey.ToString() + "\">" + msgHelper.GetMessage("lbl active") + "</a></center>", "title-header", HorizontalAlign.Left, HorizontalAlign.NotSet, Unit.Percentage(5), Unit.Percentage(5), false, false));
                    CollectionListGrid.Columns.Add(_refStyle.CreateBoundField("LANG", "<center><a href=\"urlmanualaliaslistmaint.aspx?orderby=" + EkEnumeration.AutoAliasOrderBy.ContentLanguage + "&mode=auto&fId=" + currentSiteKey.ToString() + "\">" + msgHelper.GetMessage("lbl language") + "</a></center>", "title-header", HorizontalAlign.Left, HorizontalAlign.NotSet, Unit.Percentage(4), Unit.Percentage(4), false, false));
                    CollectionListGrid.Columns.Add(_refStyle.CreateBoundField("SOURCENAME", "<a href=\"urlmanualaliaslistmaint.aspx?orderby=" + EkEnumeration.AutoAliasOrderBy.SourceName + "&mode=auto&fId=" + currentSiteKey.ToString() + "\">" + msgHelper.GetMessage("lbl source name") + "</a>", "title-header", HorizontalAlign.Left, HorizontalAlign.NotSet, Unit.Percentage(15), Unit.Percentage(15), false, false));
                    CollectionListGrid.Columns.Add(_refStyle.CreateBoundField("TYPE", "<center><a href=\"urlmanualaliaslistmaint.aspx?orderby=" + EkEnumeration.AutoAliasOrderBy.Type + "&mode=auto&fId=" + currentSiteKey.ToString() + "\">" + msgHelper.GetMessage("type label") + "</a></center>", "title-header", HorizontalAlign.Left, HorizontalAlign.NotSet, Unit.Percentage(4), Unit.Percentage(4), false, false));
                    CollectionListGrid.Columns.Add(_refStyle.CreateBoundField("ALIAS", msgHelper.GetMessage("lbl example alias"), "title-header", HorizontalAlign.Left, HorizontalAlign.NotSet, Unit.Percentage(25), Unit.Percentage(25), false, false));
                }

                DataTable dt = new DataTable();
                DataRow dr;
                if (_isRemoveAlias)
                {
                    dt.Columns.Add(new DataColumn("DELETE", typeof(string)));
                }
                dt.Columns.Add(new DataColumn("SOURCENAME", typeof(string)));
                dt.Columns.Add(new DataColumn("TYPE", typeof(string)));
                dt.Columns.Add(new DataColumn("ACTIVE", typeof(string)));
                dt.Columns.Add(new DataColumn("ALIAS", typeof(string)));
                dt.Columns.Add(new DataColumn("LANG", typeof(string)));

                for (int i = 0; i <= autoAliasList.Count - 1; i++)
                {
                    dr = dt.NewRow();
                    if (_isRemoveAlias)
                    {
                        dr["DELETE"] = "<input type=\"checkbox\" name=\"deleteAliasId\" value=\"" + (autoAliasList[i].AutoId.ToString()) + "\">";
                    }
                    dr["SOURCENAME"] = "<a href=\"urlautoaliasmaint.aspx?action=view&LangType=" + _refContentApi.GetCookieValue("LastValidLanguageID") + "&fid=" + currentSiteKey + "&id=" + autoAliasList[i].AutoId + "\">" + autoAliasList[i].SourceName + "</a>";
                    if (autoAliasList[i].AutoAliasType == Ektron.Cms.Common.EkEnumeration.AutoAliasType.Folder)
                    {
                        dr["TYPE"] = "<center><img src =\"" + _refContentApi.AppPath + "images/ui/icons/folder.png\" alt=\"" + msgHelper.GetMessage("lbl folder") + "\" title=\"" + msgHelper.GetMessage("lbl folder") + "\"/ ></center>";
                    }
                    else
                    {
                        dr["TYPE"] = "<center><img src =\"" + _refContentApi.AppPath + "images/ui/icons/taxonomy.png\" alt=\"" + msgHelper.GetMessage("generic taxonomy lbl") + "\" title=\"" + msgHelper.GetMessage("generic taxonomy lbl") + "\"/ ></center>";
                    }
                    if (autoAliasList[i].IsEnabled)
                    {
                        dr["ACTIVE"] = "<center><span style=\"color:green\">On</span></center>"; //maliaslist(i).IsEnabled"
                    }
                    else
                    {
                        dr["ACTIVE"] = "<center><span style=\"color:red\">Off</span></center>";
                    }

                    dr["ALIAS"] = autoAliasList[i].Example;
                    if (autoAliasList[i].AutoAliasType == Ektron.Cms.Common.EkEnumeration.AutoAliasType.Taxonomy)
                    {
                        for (int iLang = 0; iLang <= languageData.Length - 1; iLang++)
                        {
                            LanguageData with_1 = languageData[iLang];
                            strName = with_1.LocalName;
                            if (autoAliasList[i].LanguageId == with_1.Id)
                            {
                                strSelectedLanguageName = strName;
                            }
                        }
                    }
                    else
                    {
                        strSelectedLanguageName = "N/A";
                    }
                    dr["LANG"] = "<center><img src=" + objLocalizationApi.GetFlagUrlByLanguageID(System.Convert.ToInt32(autoAliasList[i].LanguageId)) + " alt=\"" + strSelectedLanguageName + "\" title=\"" + strSelectedLanguageName + "\" /></center>";
                    dt.Rows.Add(dr);
                }
                DataView dv = new DataView(dt);
                CollectionListGrid.DataSource = dv;
                CollectionListGrid.DataBind();
            }
        }
        else if (mode == "community")
        {
            communityAliasList = _communityAliasList.GetList(req, currentSiteKey, orderCommunityAlias);
            totalPagesNumber = req.TotalPages;
            PageSettings();
            if (communityAliasList != null && communityAliasList.Count > 0)
            {
                if (_isRemoveAlias)
                {
                    CollectionListGrid.Columns.Add(_refStyle.CreateBoundField("DELETE", msgHelper.GetMessage("generic delete title"), "title-header", HorizontalAlign.Left, HorizontalAlign.NotSet, Unit.Percentage(3), Unit.Percentage(3), false, false));
                    CollectionListGrid.Columns.Add(_refStyle.CreateBoundField("DEFAULT", "<center>" + msgHelper.GetMessage("lbl primary") + "</center>", "title-header", HorizontalAlign.Left, HorizontalAlign.NotSet, Unit.Percentage(2), Unit.Percentage(2), false, false));
                    CollectionListGrid.Columns.Add(_refStyle.CreateBoundField("ACTIVE", "<center><a href=\"urlmanualaliaslistmaint.aspx?orderby=" + EkEnumeration.CommunityAliasOrderBy.Active + "&action=removealias&mode=community&fId=" + currentSiteKey.ToString() + "\">" + msgHelper.GetMessage("lbl active") + "</a></center>", "title-header", HorizontalAlign.Left, HorizontalAlign.NotSet, Unit.Percentage(5), Unit.Percentage(5), false, false));
                    CollectionListGrid.Columns.Add(_refStyle.CreateBoundField("ALIAS PATH", msgHelper.GetMessage("lbl alias path"), "title-header", HorizontalAlign.Left, HorizontalAlign.NotSet, Unit.Percentage(5), Unit.Percentage(5), false, false));
                    CollectionListGrid.Columns.Add(_refStyle.CreateBoundField("TYPE", "<center><a href=\"urlmanualaliaslistmaint.aspx?orderby=" + EkEnumeration.AutoAliasOrderBy.Type + "&action=removealias&mode=community&fId=" + currentSiteKey.ToString() + "\">" + msgHelper.GetMessage("generic type") + "</a></center>", "title-header", HorizontalAlign.Left, HorizontalAlign.NotSet, Unit.Percentage(4), Unit.Percentage(4), false, false));
                    CollectionListGrid.Columns.Add(_refStyle.CreateBoundField("ALIAS", msgHelper.GetMessage("lbl example alias"), "title-header", HorizontalAlign.Left, HorizontalAlign.NotSet, Unit.Percentage(25), Unit.Percentage(25), false, false));

                }
                else
                {
                    CollectionListGrid.Columns.Add(_refStyle.CreateBoundField("DEFAULT", "<center>" + msgHelper.GetMessage("lbl primary") + "</center>", "title-header", HorizontalAlign.Left, HorizontalAlign.NotSet, Unit.Percentage(2), Unit.Percentage(2), false, false));
                    CollectionListGrid.Columns.Add(_refStyle.CreateBoundField("ACTIVE", "<center><a href=\"urlmanualaliaslistmaint.aspx?orderby=" + EkEnumeration.CommunityAliasOrderBy.Active + "&mode=community&fId=" + currentSiteKey.ToString() + "\">" + msgHelper.GetMessage("lbl active") + "</a></center>", "title-header", HorizontalAlign.Left, HorizontalAlign.NotSet, Unit.Percentage(3), Unit.Percentage(3), false, false));
                    CollectionListGrid.Columns.Add(_refStyle.CreateBoundField("ALIAS PATH", msgHelper.GetMessage("lbl alias path"), "title-header", HorizontalAlign.Left, HorizontalAlign.NotSet, Unit.Percentage(5), Unit.Percentage(5), false, false));
                    CollectionListGrid.Columns.Add(_refStyle.CreateBoundField("TYPE", "<center><a href=\"urlmanualaliaslistmaint.aspx?orderby=" + EkEnumeration.CommunityAliasOrderBy.Type + "&mode=community&fId=" + currentSiteKey.ToString() + "\">" + msgHelper.GetMessage("generic type") + "</a></center>", "title-header", HorizontalAlign.Left, HorizontalAlign.NotSet, Unit.Percentage(4), Unit.Percentage(4), false, false));
                    CollectionListGrid.Columns.Add(_refStyle.CreateBoundField("ALIAS", msgHelper.GetMessage("lbl example alias"), "title-header", HorizontalAlign.Left, HorizontalAlign.NotSet, Unit.Percentage(25), Unit.Percentage(25), false, false));

                }

                DataTable dt = new DataTable();
                DataRow dr;
                if (_isRemoveAlias)
                {
                    dt.Columns.Add(new DataColumn("DELETE", typeof(string)));
                }

                dt.Columns.Add(new DataColumn("DEFAULT", typeof(string)));
                dt.Columns.Add(new DataColumn("TYPE", typeof(string)));
                dt.Columns.Add(new DataColumn("ACTIVE", typeof(string)));
                dt.Columns.Add(new DataColumn("ALIAS", typeof(string)));
                dt.Columns.Add(new DataColumn("ALIAS PATH", typeof(string)));

                for (int i = 0; i <= communityAliasList.Count - 1; i++)
                {
                    dr = dt.NewRow();
                    if (_isRemoveAlias)
                    {
                        dr["DELETE"] = "<input type=\"checkbox\" name=\"deleteAliasId\" value=\"" + (communityAliasList[i].Id.ToString()) + "\">";
                    }
                    if (communityAliasList[i].IsDefault)
                    {
                        defaultIcon = "<center><img src=\"" + _refContentApi.AppPath + "images/ui/icons/check.png\" border=\"0\" alt=\"Item is Enabled\" title=\"Item is Enabled\"/></center>";
                        dr["DEFAULT"] = defaultIcon;
                    }
                    if (communityAliasList[i].IsEnabled)
                    {
                        dr["ACTIVE"] = "<center><span style=\"color:green\">On</span></center>";
                    }
                    else
                    {
                        dr["ACTIVE"] = "<center><span style=\"color:red\">Off</span></center>";
                    }
                    if (communityAliasList[i].CommunityAliasType == Ektron.Cms.Common.EkEnumeration.CommunityAliasType.Group)
                    {
                        dr["TYPE"] = "<center><img src =\"" + _refContentApi.AppPath + "images/ui/icons/usersMemberGroups.png\" alt=\"" + msgHelper.GetMessage("lbl group") + "\" title=\"" + msgHelper.GetMessage("lbl group") + "\"/ ></center>";
                    }
                    else
                    {
                        dr["TYPE"] = "<center><img src =\"" + _refContentApi.AppPath + "images/ui/icons/user.png\" alt=\"" + msgHelper.GetMessage("lbl wa mkt user goals") + "\" title=\"" + msgHelper.GetMessage("lbl wa mkt user goals") + "\"/ ></center>";
                    }
                    for (int iLang = 0; iLang <= languageData.Length - 1; iLang++)
                    {
                        LanguageData with_2 = languageData[iLang];
                        strName = with_2.LocalName;
                        if (communityAliasList[i].LanguageId == with_2.Id)
                        {
                            strSelectedLanguageName = strName;
                        }
                    }
                    dr["ALIAS"] = communityAliasList[i].Example;
                    dr["ALIAS PATH"] = "<a href=\"urlcommunityaliasmaint.aspx?action=view&id=" + communityAliasList[i].Id + "&fId=" + communityAliasList[i].SiteId + "&LangType=" + _refContentApi.GetCookieValue("LastValidLanguageID") + "\">" + communityAliasList[i].AliasPath + "</a>";

                    dt.Rows.Add(dr);
                }
                DataView dv = new DataView(dt);
                CollectionListGrid.DataSource = dv;
                CollectionListGrid.DataBind();
            }
        }
        else
        {
            if (!String.IsNullOrEmpty(Request.Form["searchlist"]))
            {
                _strSelectedItem = (EkEnumeration.UrlAliasSearchField)Enum.Parse(typeof(EkEnumeration.UrlAliasSearchField), Convert.ToString((Request.Form["searchlist"])), true);
            }
           
            manualAliasList = _manualAliasList.GetList(req, currentSiteKey, Convert.ToInt32(_refContentApi.GetCookieValue("LastValidLanguageID")), _strSelectedItem, strKeyWords, orderBy);                                                      
            totalPagesNumber = req.TotalPages;      
	  
            PageSettings();

            if ((manualAliasList != null) && manualAliasList.Count > 0)
            {
                if (_isRemoveAlias)
                {
                    CollectionListGrid.Columns.Add(_refStyle.CreateBoundField("DELETE", msgHelper.GetMessage("generic delete title"), "title-header", HorizontalAlign.Left, HorizontalAlign.NotSet, Unit.Percentage(3), Unit.Percentage(3), false, false));
                    CollectionListGrid.Columns.Add(_refStyle.CreateBoundField("DEFAULT", "<a href=\"urlmanualaliaslistmaint.aspx?orderby=4&action=removealias&fId=" + currentSiteKey.ToString() + "\">" + msgHelper.GetMessage("lbl primary") + "</a>", "title-header", HorizontalAlign.Left, HorizontalAlign.NotSet, Unit.Percentage(4), Unit.Percentage(4), false, false));
                    CollectionListGrid.Columns.Add(_refStyle.CreateBoundField("ACTIVE", "<a href=\"urlmanualaliaslistmaint.aspx?orderby=5&action=removealias&fId=" + currentSiteKey.ToString() + "\">" + msgHelper.GetMessage("lbl active") + "</a>", "title-header", HorizontalAlign.Left, HorizontalAlign.NotSet, Unit.Percentage(3), Unit.Percentage(3), false, false));
                    CollectionListGrid.Columns.Add(_refStyle.CreateBoundField("LANG", "<a href=\"urlmanualaliaslistmaint.aspx?orderby=2&fId=" + currentSiteKey.ToString() + "\">"+msgHelper.GetMessage("lbl language")+"</a>", "title-header", HorizontalAlign.Left, HorizontalAlign.NotSet, Unit.Percentage(4), Unit.Percentage(4), false, false));
                    CollectionListGrid.Columns.Add(_refStyle.CreateBoundField("ALIAS", "<a href=\"urlmanualaliaslistmaint.aspx?orderby=10&action=removealias&fId=" + currentSiteKey.ToString() + "\">" + msgHelper.GetMessage("lbl alias") + "</a>", "title-header", HorizontalAlign.Left, HorizontalAlign.NotSet, Unit.Percentage(25), Unit.Percentage(25), false, false));
                    CollectionListGrid.Columns.Add(_refStyle.CreateBoundField("ORIGINAL LINK", "<a href=\"urlmanualaliaslistmaint.aspx?orderby=6&action=removealias&fId=" + currentSiteKey.ToString() + "\">" + msgHelper.GetMessage("lbl original link") + "</a>", "title-header", HorizontalAlign.Left, HorizontalAlign.NotSet, Unit.Percentage(35), Unit.Percentage(40), false, false));
                    CollectionListGrid.Columns.Add(_refStyle.CreateBoundField("CONTENT ID", "<a href=\"urlmanualaliaslistmaint.aspx?orderby=1&action=removealias&fId=" + currentSiteKey.ToString() + "\">" + msgHelper.GetMessage("generic content id") + "</a>", "title-header", HorizontalAlign.Left, HorizontalAlign.NotSet, Unit.Percentage(10), Unit.Percentage(10), false, false));
                }
                else
                {
                    CollectionListGrid.Columns.Add(_refStyle.CreateBoundField("DEFAULT", "<a href=\"urlmanualaliaslistmaint.aspx?orderby=4&fId=" + currentSiteKey.ToString() + "\">" + msgHelper.GetMessage("lbl primary") + "</a>", "title-header", HorizontalAlign.Left, HorizontalAlign.NotSet, Unit.Percentage(4), Unit.Percentage(4), false, false));
                    CollectionListGrid.Columns.Add(_refStyle.CreateBoundField("ACTIVE", "<a href=\"urlmanualaliaslistmaint.aspx?orderby=5&fId=" + currentSiteKey.ToString() + "\">" + msgHelper.GetMessage("lbl active") + "</a>", "title-header", HorizontalAlign.Left, HorizontalAlign.NotSet, Unit.Percentage(3), Unit.Percentage(3), false, false));
                    CollectionListGrid.Columns.Add(_refStyle.CreateBoundField("LANG", "<a href=\"urlmanualaliaslistmaint.aspx?orderby=2&fId=" + currentSiteKey.ToString() + "\">" + msgHelper.GetMessage("lbl language") + "</a>", "title-header", HorizontalAlign.Left, HorizontalAlign.NotSet, Unit.Percentage(4), Unit.Percentage(4), false, false));
                    CollectionListGrid.Columns.Add(_refStyle.CreateBoundField("ALIAS", "<a href=\"urlmanualaliaslistmaint.aspx?orderby=10&fId=" + currentSiteKey.ToString() + "\">" + msgHelper.GetMessage("lbl alias") + "</a>", "title-header", HorizontalAlign.Left, HorizontalAlign.NotSet, Unit.Percentage(25), Unit.Percentage(25), false, false));
                    CollectionListGrid.Columns.Add(_refStyle.CreateBoundField("ORIGINAL LINK", "<a href=\"urlmanualaliaslistmaint.aspx?orderby=6&fId=" + currentSiteKey.ToString() + "\">" + msgHelper.GetMessage("lbl original link") + "</a>", "title-header", HorizontalAlign.Left, HorizontalAlign.NotSet, Unit.Percentage(35), Unit.Percentage(40), false, false));
                    CollectionListGrid.Columns.Add(_refStyle.CreateBoundField("CONTENT ID", "<a href=\"urlmanualaliaslistmaint.aspx?orderby=1&fId=" + currentSiteKey.ToString() + "\">" + msgHelper.GetMessage("generic content id") + "</a>", "title-header", HorizontalAlign.Left, HorizontalAlign.NotSet, Unit.Percentage(10), Unit.Percentage(10), false, false));
                }

                DataTable dt = new DataTable();
                DataRow dr;
                if (_isRemoveAlias)
                {
                    dt.Columns.Add(new DataColumn("DELETE", typeof(string)));
                }
                dt.Columns.Add(new DataColumn("DEFAULT", typeof(string)));
                dt.Columns.Add(new DataColumn("ACTIVE", typeof(string)));
                dt.Columns.Add(new DataColumn("LANG", typeof(string)));
                dt.Columns.Add(new DataColumn("ALIAS", typeof(string)));
                dt.Columns.Add(new DataColumn("ORIGINAL LINK", typeof(string)));
                dt.Columns.Add(new DataColumn("CONTENT ID", typeof(string)));

                for (int i = 0; i <= manualAliasList.Count - 1; i++)
                {
                    dr = dt.NewRow();
                    if (_isRemoveAlias)
                    {
                        dr["DELETE"] = "<input type=\"checkbox\" name=\"deleteAliasId\" value=\"" + (manualAliasList[i].AliasId.ToString()) + "\">";
                    }
                    if (manualAliasList[i].IsDefault)
                    {
                        defaultIcon = "<center><img src=\"" + _refContentApi.AppPath + "images/ui/icons/check.png\" border=\"0\" alt=\"Item is Enabled\" title=\"Item is Enabled\"/></center>";
                        dr["DEFAULT"] = defaultIcon;
                    }
                    if (manualAliasList[i].IsEnabled)
                    {
                        dr["ACTIVE"] = "<center><span style=\"color:green\">On</span></center>"; //maliaslist(i).IsEnabled"
                    }
                    else
                    {
                        dr["ACTIVE"] = "<center><span style=\"color:red\">Off</span></center>";
                    }
                    for (int iLang = 0; iLang <= languageData.Length - 1; iLang++)
                    {
                        LanguageData with_3 = languageData[iLang];
                        strName = with_3.LocalName;
                        if (manualAliasList[i].ContentLanguage == with_3.Id)
                        {
                            strSelectedLanguageName = strName;
                        }
                    }
                    dr["LANG"] = "<center><img src=" + objLocalizationApi.GetFlagUrlByLanguageID(System.Convert.ToInt32(manualAliasList[i].ContentLanguage)) + " alt=\"" + strSelectedLanguageName + "\" /></center>";
                    dr["ALIAS"] = "<a href=\"urlmanualaliasmaint.aspx?action=view&id=" + manualAliasList[i].AliasId + "&fId=" + manualAliasList[i].SiteId + "&LangType=" + _refContentApi.GetCookieValue("LastValidLanguageID") + "\">" + manualAliasList[i].DisplayAlias + "</a>";
                    if (manualAliasList[i].Target.ToLower().IndexOf("downloadasset.aspx?") == -1)
                    {
                        dr["ORIGINAL LINK"] = manualAliasList[i].Target;
                    }
                    else
                    {
                        string workareaPath = string.Empty;
                        workareaPath = Strings.Replace(_refContentApi.AppPath, _refContentApi.SitePath, "", 1, 1, 0);
                        dr["ORIGINAL LINK"] = workareaPath + manualAliasList[i].Target;
                    }

                    dr["CONTENT ID"] = manualAliasList[i].ContentId;
                    dt.Rows.Add(dr);
                }
                DataView dv = new DataView(dt);
                CollectionListGrid.DataSource = dv;
                CollectionListGrid.DataBind();
            }
        }
    }
    private void CollectSearchText()
    {
        strKeyWords = (Request.Form["txtSearch"] == null ? "" : Request.Form["txtSearch"]);
        if (Request.Form["searchlist"] != null)
        {
            if (Request.QueryString["mode"] == "auto")
            {
                _autoSelectedItem = (EkEnumeration.AutoAliasSearchField)Enum.Parse(typeof(EkEnumeration.AutoAliasSearchField), Convert.ToString((Request.Form["searchlist"])), true);
            }
            else
            {
                _strSelectedItem = (EkEnumeration.UrlAliasSearchField)Enum.Parse(typeof(EkEnumeration.UrlAliasSearchField), Convert.ToString((Request.Form["searchlist"])), true);
            }
        }
    }
    public void RefreshManualAliasList(string siteId)
    {
        _manualAliasList.ClearCache();
        Response.Redirect("urlmanualaliaslistmaint.aspx?fId=" + siteId.ToString());
    }
    public void ClearAutoAliasCache(string siteId)
    {
        _autoAliasList.ClearCache();
        Response.Redirect((string)("urlmanualaliaslistmaint.aspx?mode=auto&fId=" + siteId));
    }
    public void ClearCommunityCache(string siteId)
    {
        _communityAliasList.ClearCache();
        Response.Redirect((string)("urlmanualaliaslistmaint.aspx?mode=community&fId=" + siteId));
    }
    private void RegisterResources()
    {
        Ektron.Cms.API.Css.RegisterCss(this, Ektron.Cms.API.Css.ManagedStyleSheet.EktronWorkareaCss);
        Ektron.Cms.API.Css.RegisterCss(this, Ektron.Cms.API.Css.ManagedStyleSheet.EktronWorkareaIeCss, Ektron.Cms.API.Css.BrowserTarget.AllIE);
        Ektron.Cms.API.Css.RegisterCss(this, Ektron.Cms.API.Css.ManagedStyleSheet.EktronFixedPositionToolbarCss);

        Ektron.Cms.API.JS.RegisterJS(this, Ektron.Cms.API.JS.ManagedScript.EktronStyleHelperJS);
        Ektron.Cms.API.JS.RegisterJS(this, Ektron.Cms.API.JS.ManagedScript.EktronToolBarRollJS);
        Ektron.Cms.API.JS.RegisterJS(this, Ektron.Cms.API.JS.ManagedScript.EktronJFunctJS);
        Ektron.Cms.API.JS.RegisterJS(this, Ektron.Cms.API.JS.ManagedScript.EktronJS);
        Ektron.Cms.API.JS.RegisterJS(this, Ektron.Cms.API.JS.ManagedScript.EktronInputLabelJS);
        Ektron.Cms.API.JS.RegisterJS(this, _refContentApi.AppPath + "java/ektron.workarea.searchBox.inputLabelInit.js", "EktronWorkareaSearchInputLabelInitJS");
    }
    private void SetServerJSVariables()
    {
        ltr_msgSelItem.Text = msgHelper.GetMessage("alert msg select item");
        ltr_msgDelAlias.Text = msgHelper.GetMessage("alert msg del sel alias");
    }
}