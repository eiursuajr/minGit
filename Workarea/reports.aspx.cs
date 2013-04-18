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
using Microsoft.Security.Application;

public partial class reports : System.Web.UI.Page
{
    #region Members
    //objects
    protected EkMessageHelper _MessageHelper;
    protected StyleHelper _StyleHelper;
    protected ContentAPI _ContentApi;
    protected AssetInfoData[] _AssetInfoData;
    protected System.Data.DataView _SiteActivityDataSource;
    protected Collection _SiteData;
    protected EmailHelper _EmailHelper = new EmailHelper();
    protected DataView _DataView;
    protected ContentData[] _ReportData = null;
    protected PagingInfo _PageInfo = new PagingInfo();

    //strings
    protected string _OrderBy;
    protected string _PreviousAction;
    protected string _ContentTypeSelected;
    protected string _FolderName;
    protected string _StartDate;
    protected string _EndDate;
    protected string _Folder;
    protected string _ReportDisplay;
    protected string _ApplicationName;
    protected string _AppImgPath;
    protected string _PageAction;
    protected string _Interval;
    protected string _TitleBarMsg;
    protected string _ReportType;
    protected string _FilterType;
    protected string _ReportTableHtml;
    protected string _JsOrderByQueryStringArg;

    //booleans
    protected bool _HasData;
    protected bool _IsSubFolderIncluded;

    //numerics
    protected long _FilterId;
    protected int _ContentLanguage;
    protected int _EnableMultilingual;
    protected int _ContentType;
    protected int _ContentType2;
    #endregion

    #region Events
    public reports()
    {
        //initialize members
        _ContentApi = new ContentAPI();
        _StyleHelper = new StyleHelper();
        _HasData = false;
        _ContentLanguage = -1;
        _EnableMultilingual = 0;
        _ContentTypeSelected = Ektron.Cms.Common.EkConstants.CMSContentType_AllTypes.ToString();
        _SiteActivityDataSource = new System.Data.DataView();
        _IsSubFolderIncluded = false;
        _ContentType2 = 0;
        _FilterId = 0;
        _MessageHelper = _ContentApi.EkMsgRef;
    }

    private void Page_Init(System.Object sender, System.EventArgs e)
    {

        //redirect if not logged in
        RedirectIfNotLoggedIn();

        //register page components
        RegisterJs();
        RegisterCss();
        litStyleSheetJs.Text = _StyleHelper.GetClientScript();

        //set multiview to legacy by default
        this.mvReports.SetActiveView(this.vwReportLegacy);
    }

    private void Page_Load(System.Object sender, System.EventArgs e)
    {

        string action = "view";
        string inFolder = "";
        int k = 0;
        string[] id = null;
        string folderList = "";
        string folderId = "";
        long rootFolder = 0;
        string userNames = "None";
        string groupNames = "None";
        string userIdList = "";
        string groupIdList = "";
        string[] userId = null;
        string[] groupId = null;
        string userList = "";
        Ektron.Cms.UserAPI userAPI = new Ektron.Cms.UserAPI();
        int i = 0;
        bool doesExcludeAll = false;
        bool isSubmit = true;
        CommonApi commonApi = new CommonApi();
        bool continueSub = false;

        if ((_ContentApi.EkContentRef).IsAllowed(0, 0, "users", "IsLoggedIn", 0) == false)
        {
            Response.Redirect("login.aspx?fromLnkPg=1", false);
            return;
        }
        if (_ContentApi.RequestInformationRef.IsMembershipUser == 1 || _ContentApi.RequestInformationRef.UserId == 0)
        {
            Response.Redirect("reterror.aspx?info=Please login as cms user", false);
            return;
        }
        //set language
        _ContentLanguage = string.IsNullOrEmpty(Request.QueryString["LangType"]) ? Convert.ToInt32(_ContentApi.GetCookieValue("LastValidLanguageID")) : Convert.ToInt32(Request.QueryString["LangType"]);


        _ContentApi.SetCookieValue("LastValidLanguageID", _ContentLanguage.ToString());
        _ContentApi.ContentLanguage = System.Convert.ToInt32(_ContentLanguage == Ektron.Cms.Common.EkConstants.CONTENT_LANGUAGES_UNDEFINED ? Ektron.Cms.Common.EkConstants.ALL_CONTENT_LANGUAGES : _ContentLanguage);
        _EnableMultilingual = _ContentApi.EnableMultilingual;

        //set filter
        _FilterType = (string)((string.IsNullOrEmpty(Request.QueryString["filterType"])) ? string.Empty : (AntiXss.HtmlEncode(Request.QueryString["filtertype"])));
        if (!string.IsNullOrEmpty(Request.QueryString["filterid"]) )
        {
            folderId = Request.QueryString["filterid"];
            if (!folderId.Contains(","))
            {
                if (Request.QueryString["filterid"] != null)
                { _FilterId = long.Parse(Request.QueryString["filterid"]); }
            }
            if (_FilterType == "path")
            {
                id = folderId.Split(',');
                for (k = 0; k <= id.Length - 1; k++)
                {
                    if (!string.IsNullOrEmpty(_FolderName))
                    {
                        if (_FolderName.Length > 0)
                        {
                            _FolderName = _FolderName + ",";
                        }
                    }
                    _FolderName = _FolderName + _ContentApi.GetFolderById(long.Parse(id[k])).Name;
                    _FilterId = long.Parse(id[k]);
                    // Set limit for listing folder-names:
                    if ((k >= 0) && (k < (id.Length - 1)))
                    {
                        _FolderName += ", ...";
                        break;
                    }
                }
                //FldrName = m_refContentApi.GetFolderById(FilterId).Name & """"
                _FolderName += "\"";
                inFolder = " In Folder \"";
            }
        }

        //set page action - throw if contains ampersand
        _PageAction = (string)((string.IsNullOrEmpty(Request.QueryString["action"])) ? string.Empty : (EkFunctions.HtmlEncode(Convert.ToString(Request.QueryString["action"])).ToLower().Trim()));
        if (_PageAction.Contains("&"))
        {
            Utilities.ShowError(_MessageHelper.GetMessage("invalid querstring"));
            return;
        }
        _PageAction = AntiXss.HtmlEncode(_PageAction);
        //get querystrings
        _AppImgPath = _ContentApi.AppImgPath;
        _Interval = (string)((string.IsNullOrEmpty(Request.QueryString["interval"])) ? string.Empty : (AntiXss.HtmlEncode(Request.QueryString["interval"])));
        _OrderBy = (string)((string.IsNullOrEmpty(Request.QueryString["orderby"])) ? string.Empty : (AntiXss.HtmlEncode(Request.QueryString["orderby"])));

        if (!string.IsNullOrEmpty(Request.QueryString["rootfolder"]))
        {
            rootFolder = long.Parse(Request.QueryString["rootfolder"]);
        }
        if (!string.IsNullOrEmpty(Request.QueryString["subfldInclude"]))
        {
            _IsSubFolderIncluded = bool.Parse(Request.QueryString["subfldInclude"]);
        }
        if ((Request.QueryString["rptFolder"] == null) || (Request.QueryString["rptFolder"] == ""))
        {
            _Folder = " Content Folder ";
        }
        else if (!(Request.QueryString["filterType"] == null) && Request.QueryString["filterType"] != "")
        {
            id = (EkFunctions.HtmlEncode(Request.QueryString["filterid"])).Split(',');
            for (k = 0; k <= id.Length - 1; k++)
            {
                if (_Folder.Length > 0)
                {
                    _Folder = _Folder + ",";
                }
                _Folder = _Folder + _ContentApi.GetFolderById(long.Parse(id[k])).Name;
            }
        }
        else
        {
            _Folder = EkFunctions.HtmlEncode(Request.QueryString["rptFolder"]);
        }
        if (Request.QueryString[Ektron.Cms.Common.EkConstants.ContentTypeUrlParam] != "")
        {
            if (Information.IsNumeric(Request.QueryString[Ektron.Cms.Common.EkConstants.ContentTypeUrlParam]))
            {
                _ContentType = Convert.ToInt32(Request.QueryString[Ektron.Cms.Common.EkConstants.ContentTypeUrlParam]);
                _ContentTypeSelected = Request.QueryString[Ektron.Cms.Common.EkConstants.ContentTypeUrlParam].ToString();
                _ContentApi.SetCookieValue(Ektron.Cms.Common.EkConstants.ContentTypeUrlParam, _ContentTypeSelected);
            }
        }
        else if (Ektron.Cms.CommonApi.GetEcmCookie()[Ektron.Cms.Common.EkConstants.ContentTypeUrlParam] != "")
        {
            if (Information.IsNumeric(Ektron.Cms.CommonApi.GetEcmCookie()[Ektron.Cms.Common.EkConstants.ContentTypeUrlParam]))
            {
                _ContentTypeSelected = (Ektron.Cms.CommonApi.GetEcmCookie()[Ektron.Cms.Common.EkConstants.ContentTypeUrlParam]).ToString();
            }
        }

        switch (_PageAction)
        {
            case "checkinall":
                Process_CheckInAll();
                break;
            case "submitall":
                Process_SubmitAll();
                break;
            case "viewallreporttypes":
                Display_ReportTypes();
                break;
            case "viewasynchlogfile":
                Display_AsynchLogFile();
                break;
            case "contentreviews":
                Display_ContentReviews();
                break;
            case "contentflags":
                Display_ContentFlags();
                break;
            case "viewsearchphrasereport":
                SearchPhraseReport();
                break;
            case "viewpreapproval":
                Display_Preapproval();
                break;
            case "viewcheckedout":
                continueSub = true;
                _ReportType = "checkedout";
                _HasData = true;
                _TitleBarMsg = (string)(_MessageHelper.GetMessage("content reports checked out title") + inFolder + _FolderName);
                break;
            case "viewcheckedin":
                //look at last line in page_load for loading of viewcheckedin control
                continueSub = true;
                _ReportType = "checkedin";
                _HasData = true;
                _TitleBarMsg = (string)(_MessageHelper.GetMessage("content reports checked in title") + inFolder + _FolderName);
                break;
            case "viewsubmitted":
                continueSub = true;
                _ReportType = "submitted";
                _HasData = true;
                _TitleBarMsg = (string)(_MessageHelper.GetMessage("content reports submitted title") + inFolder + _FolderName);
                break;
            case "viewnewcontent":
                continueSub = true;
                _ReportType = "newcontent";
                _HasData = true;
                _TitleBarMsg = (string)(_MessageHelper.GetMessage("content reports new title") + inFolder + _FolderName);
                break;
            case "viewpending":
                continueSub = true;
                _ReportType = "pendingcontent";
                _HasData = true;
                _TitleBarMsg = (string)(_MessageHelper.GetMessage("content reports pending title") + inFolder + _FolderName);
                break;
            case "viewrefreshreport":
                continueSub = true;
                _ReportType = "refreshdcontent";
                _HasData = true;
                _TitleBarMsg = (string)(_MessageHelper.GetMessage("content reports refresh title") + inFolder + _FolderName);
                break;
            case "viewexpired":
                continueSub = true;
                _ReportType = "expiredcontent";
                _HasData = true;
                _TitleBarMsg = (string)(_MessageHelper.GetMessage("content reports expired title") + inFolder + _FolderName);
                break;
            case "viewtoexpire":
                continueSub = true;
                _ReportType = "expireToContent";
                _HasData = true;
                _TitleBarMsg = (string)(_MessageHelper.GetMessage("lbl cont expire") + "&nbsp;" + _Interval + "&nbsp;" + _MessageHelper.GetMessage("lbl sync monthly days") + inFolder + _FolderName);
                break;
            case "siteupdateactivity":
                continueSub = true;
                _ReportType = "updateactivityContent";
                _HasData = true;
                _TitleBarMsg = (string)(_MessageHelper.GetMessage("content reports site activity title") + inFolder + _FolderName);
                break;
            default:
                continueSub = true;
                _ReportType = "";
                _HasData = false;
                _TitleBarMsg = "";
                break;
        }

        //set js vars
        litPageAction.Text = _PageAction;
        litOrderBy.Text = _OrderBy;
        litFilterType.Text = _FilterType;
        litFilterId.Text = _FilterId.ToString();
        litInterval.Text = _Interval;

        if (continueSub == true)
        {
            int j;
            Collection pagedata = null;
            Collection cUser = null;
            long[] arUserIds;
            string sExclUserId = "";
            int idx = 0;

            //Dim sitedata As ContentData()
            if ((_PageAction == "viewcheckedout") || (_PageAction == "viewnewcontent"))
            {
                lblAction.Text = "var actionString = \"reports.aspx?action=CheckinAll&PreviousAction=" + _PageAction + "&filtertype=" + _FilterType + "&filterid=" + _FilterId + "&orderby=" + _OrderBy + "\";";
                lblAction.Text += "if (WarnAllCheckin()) { DisplayHoldMsg_Local(true); document.forms.selections.action = actionString; document.forms.selections.submit(); }";
            }
            if (_PageAction == "viewcheckedin")
            {
                lblAction.Text += "var actionString = \"reports.aspx?action=SubmitAll&PreviousAction=" + _PageAction + "&filtertype=" + _FilterType + "&filterid=" + _FilterId + "&orderby=" + _OrderBy + "\";";
                lblAction.Text += "if (WarnAllSubmit()) { DisplayHoldMsg_Local(true); document.forms.selections.action = actionString; document.forms.selections.submit(); }";
            }

            rptTitle.Value = _TitleBarMsg;
            top.Visible = false;

            if (_PageAction == "viewtoexpire")
            {
                selInterval.Visible = true;
                txtInterval.Visible = true;

                if (0 != _Interval.Length)
                {
                    txtInterval.Value = _Interval;
                }

                selInterval.Items.Clear();
                selInterval.Items.Add(new ListItem("Select Interval", ""));
                selInterval.Items.Add(new ListItem("10", "10"));
                selInterval.Items.Add(new ListItem("20", "20"));
                selInterval.Items.Add(new ListItem("30", "30"));
                selInterval.Items.Add(new ListItem("40", "40"));
                selInterval.Items.Add(new ListItem("50", "50"));
                selInterval.Items.Add(new ListItem("60", "60"));
                selInterval.Items.Add(new ListItem("70", "70"));
                selInterval.Items.Add(new ListItem("80", "80"));
                selInterval.Items.Add(new ListItem("90", "90"));

                if (0 != _Interval.Length)
                {
                    for (j = 1; j <= 9; j++)
                    {
                        if (_Interval == selInterval.Items[j].Value)
                        {
                            selInterval.Items[j].Selected = true;
                            break;
                        }
                    }
                }
                else
                {
                    selInterval.Items[0].Selected = true;
                }

                lblDays.Text = _MessageHelper.GetMessage("lbl sync monthly days");
                lblDays.Visible = true;
                top.Visible = true;
                IncludeContentToExpireJS();
            }
            else if (_PageAction == "siteupdateactivity")
            {
                System.Text.StringBuilder result = new System.Text.StringBuilder();
                EditScheduleHtml.Text = "";
                if (!(Request.QueryString["ex_users"] == null))
                {
                    if (Request.QueryString["ex_users"] != "")
                    {
                        userIdList = EkFunctions.HtmlEncode(Request.QueryString["ex_users"]);
                    }
                }
                if (!(Request.QueryString["ex_groups"] == null))
                {
                    if (Request.QueryString["ex_groups"] != "")
                    {
                        groupIdList = EkFunctions.HtmlEncode(Request.QueryString["ex_groups"]);
                    }
                }
                if (Request.QueryString["btnSubmit"] == null)
                {
                    isSubmit = false;
                    editSchedule.Visible = true;
                    lblDays.Visible = true;
                    tr_startDate.Visible = true;
                    tr_endDate.Visible = true;
                    lblStartDate.Text = _MessageHelper.GetMessage("generic start date label");
                    lblEndDate.Text = _MessageHelper.GetMessage("generic end date label");
                }
                else
                {
                    // User wants Site Activity report
                    isSubmit = true;
                    if (!(Request.Form["excludeAllUsers"] == null))
                    {
                        doesExcludeAll = Convert.ToBoolean(Request.Form["excludeAllUsers"]);
                    }
                    if (!(Request.Form["start_date"] == null))
                    {
                        _StartDate = Request.Form["start_date"];
                    }
                    else if (!(Request.QueryString["startdate"] == null))
                    {
                        _StartDate = EkFunctions.HtmlEncode(Request.QueryString["startdate"]);
                        _StartDate = _StartDate.Replace("\'", "");
                    }
                    if (!(Request.Form["end_date"] == null))
                    {
                        _EndDate = Request.Form["end_date"];
                        if (!Information.IsDate(_EndDate))
                        {
                            _EndDate = "";
                        }
                    }
                    else if (!(Request.QueryString["enddate"] == null))
                    {
                        _EndDate = EkFunctions.HtmlEncode(Request.QueryString["enddate"]);
                        _EndDate = _EndDate.Replace("\'", "");
                        if (!Information.IsDate(_EndDate))
                        {
                            _EndDate = "";
                        }
                    }
                }
                DisplayDateFields();

                //EditScheduleHtml.Text = EditScheduleHtml.Text & MakeIFrameArea()
                result.Append("<input id=\"fId\" type=\"hidden\" name=\"fId\" ");
                if (Request.QueryString["filterid"] == null)
                {
                    result.Append("value=\"\">" + "\r\n");
                }
                else
                {
                    result.Append("value=\"" + EkFunctions.HtmlEncode(Request.QueryString["filterid"]) + "\"/>" + "\r\n");
                }
                result.Append("<input id=\"rptType\" type=\"hidden\" name=\"rptType\"/>" + "\r\n");
                result.Append("<input id=\"rptFolderList\" type=\"hidden\" name=\"rptFolderList\"/>" + "\r\n");
                result.Append("<input id=\"rootFolder\" type=\"hidden\" name=\"rootFolder\" value=\"" + rootFolder + "\"/>" + "\r\n");
                result.Append("<input id=\"rptLink\" type=\"hidden\" name=\"rptLink\"/>" + "\r\n");
                result.Append("<input id=\"ContType\" type=\"hidden\" name=\"ContType\" value=\"" + _ContentType + "\"/>" + "\r\n");
                result.Append("<input id=\"subfldInclude\" type=\"hidden\" name=\"subfldInclude\" value=\"" + _IsSubFolderIncluded + "\"/>" + "\r\n");
                result.Append("<input id=\"LangType\" type=\"hidden\" name=\"LangType\" value=\"" + _ContentLanguage + "\"/>" + "\r\n");
                result.Append("<input id=\"excludeUserIds\" type=\"hidden\" name=\"excludeUserIds\" value=\"" + userIdList + "\"/>" + "\r\n");
                result.Append("<input id=\"excludeUserGroups\" type=\"hidden\" name=\"excludeUserGroups\" value=\"" + groupIdList + "\"/>" + "\r\n");
                result.Append("<input id=\"excludeAllUsers\" type=\"hidden\" name=\"excludeAllUsers\" value=\"" + doesExcludeAll + "\"/>" + "\r\n");
                //select folder
                result.Append("<table id=\"EditText\" width=\"100%\" class=\"ektronGrid\">");
                result.Append("<tr><td id=\"lblSelFolder\" class=\"label\">" + _MessageHelper.GetMessage("lbl select folder") + "</td>");
                result.Append("<td id=\"selectedFolderList\"><a title=\"" + _MessageHelper.GetMessage("lbl select folder") + "\" href=\"#\" id=\"hselFolder\" onclick=\"LoadFolderChildPage(\'" + _PageAction + "\',\'" + _ContentLanguage + "\');return true;\">");
                if ((Request.QueryString["filterid"] == null) || ("" == Request.QueryString["filterid"]))
                {
                    //result.Append(m_refMsg.GetMessage("lbl Root Folder"))
                    result.Append("\\");
                }
                else
                {
                    id = (EkFunctions.HtmlEncode(Request.QueryString["filterid"])).Split(',');
                    for (k = 0; k <= id.Length - 1; k++)
                    {
                        if (folderList.Length > 0)
                        {
                            folderList = folderList + ",";
                        }
                        folderList = folderList + _ContentApi.GetFolderById(long.Parse(id[k])).Name;
                    }
                    result.Append(folderList);
                }
                result.Append("</a></td></tr>" + "\r\n");
                string sShow = "display:none";
                if (_IsSubFolderIncluded)
                {
                    sShow = "display:block";
                }
                result.Append("<tr><td id=\"subfldIncludetxt\" colspan=\"2\" style=\"" + sShow + "\">");
                result.Append(_MessageHelper.GetMessage("lbl subfolder included"));
                result.Append("</td></tr>" + "\r\n");
                //report type
                _ReportDisplay = EkFunctions.HtmlEncode(Request.QueryString["report_display"]);
                result.Append("<tr><td class=\"label\">" + _MessageHelper.GetMessage("lbl report type") + "</td><td><select id=\"selDisplay\" name=\"selDisplay\">");
                result.Append("<option id=\"ev\" value=\"ev\"");
                if ("ev" == _ReportDisplay)
                {
                    result.Append(" SELECTED");
                }
                result.Append(">" + _MessageHelper.GetMessage("lbl executive view") + "</option>");
                result.Append("<option id=\"dv\" value=\"dv\"");
                if ("dv" == _ReportDisplay)
                {
                    result.Append(" selected=\'selected\'");
                }
                result.Append(">" + _MessageHelper.GetMessage("lbl detail view") + "</option>");
                result.Append("<option id=\"cv\" value=\"cv\"");
                if ("cv" == _ReportDisplay)
                {
                    result.Append(" selected=\'selected\'");
                }
                result.Append(">" + _MessageHelper.GetMessage("lbl combined view") + "</option></select></td></tr>" + "\r\n");
                //exclude user
                result.Append("<tr><td id=\"lblSelUser\" class=\"label\">" + _MessageHelper.GetMessage("lbl exclude users") + "</td>" + "\r\n");
                result.Append("<td>");
                result.Append("<div id=\"excludeUserList\">");
                if (userIdList.Length > 0 || groupIdList.Length > 0)
                {
                    if (userIdList.Length > 0)
                    {
                        userNames = "";
                        userId = userIdList.Split(",".ToCharArray());
                        for (i = 0; i <= userId.Length - 1; i++)
                        {
                            if (userNames.Length > 0)
                            {
                                userNames = userNames + ",";
                            }
                            userNames = userNames + userAPI.UserObject(int.Parse(userId[i])).Username;
                        }
                        if (userNames.Length == 0)
                        {
                            userNames = "None";
                        }
                    }
                    if (groupIdList.Length > 0)
                    {
                        groupNames = "";
                        groupId = groupIdList.Split(",".ToCharArray());
                        for (i = 0; i <= groupId.Length - 1; i++)
                        {
                            if (groupNames.Length > 0)
                            {
                                groupNames = groupNames + ",";
                            }
                            groupNames = groupNames + _ContentApi.EkUserRef.GetActiveUserGroupbyID(Convert.ToInt64(groupId[i])).GroupName;
                            if (groupNames.Length == 0)
                            {
                                groupNames = "None";
                            }
                        }
                    }
                    userList = "User (" + userNames.Replace(",", ", ") + ")<br />User Group (" + groupNames.Replace(",", ", ") + ")";
                    result.Append(userList);
                }
                result.Append("</div>");
                result.Append("<ul class=\'buttonWrapper buttonWrapperLeft\'><li><a title=\"" + _MessageHelper.GetMessage("lbl Select User or Group") + "\" class=\"button buttonInlineBlock greenHover buttonCheckAll\" href=\"javascript://\" id=\"selExclUser\" onclick=\"LoadUserListChildPage(\'" + _PageAction + "_siteRpt\');return true;\">");
                result.Append(_MessageHelper.GetMessage("lbl Select User or Group"));
                result.Append("</a></li></ul></td></tr>" + "\r\n");
                //generate report button
                result.Append("<tr><td class=\'label\'>&nbsp;</td><td><ul class=\'buttonWrapper buttonWrapperLeft\'><li><a class=\"button buttonInlineBlock greenHover buttonGetResult\" id=\"btnResult\" onclick=\"ReportSiteUpdateActivity()\" title=\"" + _MessageHelper.GetMessage("btn get result") + "\">" + _MessageHelper.GetMessage("btn get result") + "</a></li></ul></td></tr>" + "\r\n");
                result.Append("</table>");
                EditScheduleHtml.Text = EditScheduleHtml.Text + result.ToString();

                IncludeSiteUpdateActivityJS();
            }

            _AssetInfoData = _ContentApi.GetAssetSupertypes();
            if (Ektron.Cms.Common.EkConstants.CMSContentType_Content == Convert.ToInt32(_ContentTypeSelected) || Ektron.Cms.Common.EkConstants.CMSContentType_Archive_Content == Convert.ToInt32(_ContentTypeSelected) || Ektron.Cms.Common.EkConstants.CMSContentType_Forms == Convert.ToInt32(_ContentTypeSelected) || Ektron.Cms.Common.EkConstants.CMSContentType_Library == Convert.ToInt32(_ContentTypeSelected) || Ektron.Cms.Common.EkConstants.CMSContentType_NonImageLibrary == Convert.ToInt32(_ContentTypeSelected) || Ektron.Cms.Common.EkConstants.CMSContentType_PDF == Convert.ToInt32(_ContentTypeSelected))
            {
                _ContentType2 = int.Parse(_ContentTypeSelected);
            }
            else if (Ektron.Cms.Common.EkConstants.ManagedAsset_Min <= Convert.ToInt32(_ContentTypeSelected) && Convert.ToInt32(_ContentTypeSelected) <= Ektron.Cms.Common.EkConstants.ManagedAsset_Max)
            {
                if (DoesAssetSupertypeExist(_AssetInfoData, int.Parse(_ContentTypeSelected)))
                {
                    _ContentType2 = int.Parse(_ContentTypeSelected);
                }
            }

            if (_HasData)
            {
                if (("viewrefreshreport" == _PageAction | "refreshdcontent" == _PageAction | "viewpending" == _PageAction | "viewcheckedin" == _PageAction) && !_ContentApi.EkContentRef.IsAllowed(_ContentApi.RequestInformationRef.UserId, 0, "users", "IsAdmin"))
                {
                    _FilterType = "User";
                    _FilterId = _ContentApi.RequestInformationRef.UserId;
                }
                cUser = new Collection();
                pagedata = new Collection();
                pagedata.Add(_ReportType, "StateWanted", null, null);
                pagedata.Add(_FilterType, "FilterType", null, null);
                pagedata.Add(_FilterId, "FilterID", null, null);
                pagedata.Add(_OrderBy, "OrderBy", null, null);
                pagedata.Add(_Interval, "Interval", null, null);
                if (_ContentType2 > 0)
                {
                    pagedata.Add(_ContentType2, "ContentType", null, null);
                }

                if (_PageAction == "viewtoexpire")
                {
                    _ReportData = _ContentApi.GetExpireContent(pagedata);
                }
                else if ("siteupdateactivity" == _PageAction)
                {
                    pagedata.Add(_StartDate, "StartDate", null, null);
                    pagedata.Add(_EndDate, "EndDate", null, null);
                    pagedata.Add(_IsSubFolderIncluded, "SubFolders", null, null);
                    pagedata.Add(folderId, "FolderId", null, null);
                    pagedata.Add(rootFolder, "RootFolder", null, null);

                    if (groupIdList.Length > 0)
                    {
                        string[] temp = groupIdList.Split(",".ToCharArray());
                        long[] arrIdList = new long[temp.Length - 1 + 1];
                        int index;
                        for (index = 0; index <= temp.Length - 1; index++)
                        {
                            arrIdList[index] = Convert.ToInt64(temp[index]);
                        }
                        arUserIds = _ContentApi.EkUserRef.GetAllUsersIdsByGroup(arrIdList, "userid");
                        if (arUserIds.Length > 0)
                        {
                            for (idx = 0; idx <= arUserIds.Length - 1; idx++)
                            {
                                if (sExclUserId.Length > 0)
                                {
                                    sExclUserId = sExclUserId + ",";
                                }
                                sExclUserId = sExclUserId + arUserIds[idx].ToString(); //("UserID")
                            }
                        }
                    }
                    if (userIdList.Length > 0)
                    {
                        if (sExclUserId.Length > 0)
                        {
                            sExclUserId = sExclUserId + ",";
                        }
                        sExclUserId = sExclUserId + userIdList;
                    }
                    if (0 == sExclUserId.Length)
                    {
                        sExclUserId = "EktNone";
                    }
                    pagedata.Add(sExclUserId, "ExUserIds", null, null);
                    pagedata.Add(doesExcludeAll, "ExAllUsers", null, null);
                }
                else
                {
                    //_ReportData = _ContentApi.GetContentReport(pagedata)
                    _PageInfo.RecordsPerPage = _ContentApi.RequestInformationRef.PagingSize;
                    _PageInfo.CurrentPage = System.Convert.ToInt32(this.uxPaging.SelectedPage + 1);
                    try
                    {
                        _ReportData = _ContentApi.GetContentReport(pagedata, _PageInfo);
                    }
                    catch
                    {
                        Response.Redirect("reports.aspx?action=ViewPending");
                    }
                    if (_ReportData != null && _PageInfo.TotalPages > 1)
                    {
                        this.uxPaging.Visible = true;
                        this.uxPaging.TotalPages = _PageInfo.TotalPages;
                        this.uxPaging.CurrentPageIndex = _PageInfo.CurrentPage - 1;
                    }
                    else
                    {
                        this.uxPaging.Visible = false;
                    }
                }
            }

            reportstoolbar m_reportsToolBar;
            System.Web.UI.WebControls.BoundColumn colBound = new System.Web.UI.WebControls.BoundColumn();

            DataTable dt = new DataTable();
            DataRow dr;
            bool bIsChart = false;
            m_reportsToolBar = (reportstoolbar)(LoadControl("controls/reports/reportstoolbar.ascx"));
            ToolBarHolder.Controls.Add(m_reportsToolBar);
            m_reportsToolBar.AppImgPath = _AppImgPath;
            if (_ReportType.ToLower() == "updateactivitycontent")
            {
                m_reportsToolBar.Data = "";
            }
            else
            {
                m_reportsToolBar.Data = _ReportData;
            }
            m_reportsToolBar.PageAction = _PageAction;
            m_reportsToolBar.FilterType = _FilterType;
            m_reportsToolBar.TitleBarMsg = _TitleBarMsg;
            m_reportsToolBar.MultilingualEnabled = _EnableMultilingual;
            m_reportsToolBar.ContentLang = _ContentApi.ContentLanguage;
            m_reportsToolBar.HasData = isSubmit;

            //DATA DISPLAY
            // Grid is different for the activity report
            if (_ReportType.ToLower() != "updateactivitycontent")
            {
                chart.Visible = false;
                lblTbl.Visible = false;

                colBound.DataField = "TITLE";
                if ((_PageAction == "viewcheckedout") || (_PageAction == "viewcheckedin")){
                    colBound.HeaderText = "<input type=\"checkbox\" name=\"all\" onclick=\"javascript:checkAll(this);\"> ";
                }
                colBound.HeaderText += _MessageHelper.GetMessage("generic Title");
                colBound.ItemStyle.Wrap = false;
                colBound.ItemStyle.VerticalAlign = VerticalAlign.Top;
                colBound.ItemStyle.HorizontalAlign = HorizontalAlign.Left;
                colBound.HeaderStyle.CssClass = "title-header";
                dgReport.Columns.Add(colBound);

                colBound = new System.Web.UI.WebControls.BoundColumn();
                colBound.DataField = "ID";
                colBound.HeaderText = _MessageHelper.GetMessage("generic ID");
                colBound.ItemStyle.VerticalAlign = VerticalAlign.Top;
                colBound.HeaderStyle.CssClass = "title-header";
                colBound.ItemStyle.Wrap = false;
                dgReport.Columns.Add(colBound);

                colBound = new System.Web.UI.WebControls.BoundColumn();
                colBound.DataField = "LASTEDITOR";
                colBound.HeaderText = _MessageHelper.GetMessage("generic Last Editor");
                colBound.HeaderStyle.CssClass = "title-header";
                colBound.ItemStyle.Wrap = false;
                colBound.ItemStyle.VerticalAlign = VerticalAlign.Top;
                colBound.ItemStyle.HorizontalAlign = HorizontalAlign.Left;
                dgReport.Columns.Add(colBound);

                string msg = "";
                if (_PageAction == "viewpending")
                {
                    msg = _MessageHelper.GetMessage("generic Go Live");
                }
                else if (_PageAction == "viewexpired")
                {
                    msg = _MessageHelper.GetMessage("generic End Date");
                }
                else if (_PageAction == "viewtoexpire")
                {
                    msg = _MessageHelper.GetMessage("generic End Date");
                }
                else
                {
                    msg = _MessageHelper.GetMessage("generic Date Modified");
                }
                colBound = new System.Web.UI.WebControls.BoundColumn();
                colBound.DataField = "DATE";
                colBound.HeaderText = msg;
                colBound.ItemStyle.VerticalAlign = VerticalAlign.Top;
                colBound.ItemStyle.HorizontalAlign = HorizontalAlign.Left;
                colBound.HeaderStyle.CssClass = "title-header";
                colBound.ItemStyle.Wrap = false;
                dgReport.Columns.Add(colBound);

                colBound = new System.Web.UI.WebControls.BoundColumn();
                colBound.DataField = "PATH";
                colBound.HeaderText = _MessageHelper.GetMessage("generic Path");
                colBound.ItemStyle.VerticalAlign = VerticalAlign.Top;
                colBound.ItemStyle.HorizontalAlign = HorizontalAlign.Left;
                colBound.HeaderStyle.CssClass = "title-header";
                colBound.ItemStyle.Wrap = false;
                dgReport.Columns.Add(colBound);


                dgReport.BorderColor = System.Drawing.Color.White;
                string cLinkArray = "";
                string fLinkArray = "";
                string lLinkArray = "";

                dt.Columns.Add(new DataColumn("TITLE", typeof(string)));
                dt.Columns.Add(new DataColumn("ID", typeof(long)));
                dt.Columns.Add(new DataColumn("LASTEDITOR", typeof(string)));
                dt.Columns.Add(new DataColumn("DATE", typeof(string)));
                dt.Columns.Add(new DataColumn("PATH", typeof(string)));
                if (_PageAction == "viewcheckedout")
                {
                    action = "ViewStaged";
                }
                if (!(_ReportData == null))
                {
                    editSchedule.Visible = false;
                    dgReport.Visible = true;

                    for (i = 0; i <= _ReportData.Length - 1; i++)
                    {
                        if ((_PageAction == "viewcheckedout") || (_PageAction == "viewcheckedin"))
                        {
                            top.Visible = true;                            
                        }
                        dr = dt.NewRow();
                        if ((_PageAction == "viewcheckedout") || (_PageAction == "viewcheckedin"))
                        {
                            dr[0] += "<input type=\"checkbox\" name=\"frm_check" + i + "\" onclick=\"document.forms.selections[\'frm_hidden" + i + "\'].value=(this.checked ?" + _ReportData[i].Id + " : 0);\"> ";
                        }
                        else
                        {
                            dr[0] += "<input type=\"hidden\" name=\"frm_check" + i + "\" onclick=\"document.forms.selections[\'frm_hidden" + i + "\'].value=(this.checked ?" + _ReportData[i].Id + ": 0);\"> ";
                        }
                        if (_ReportData[i].ContType == 1)
                        {
                            if (_ReportData[i].SubType == EkEnumeration.CMSContentSubtype.WebEvent)
                            {
                                dr[0] += "&nbsp;<img id=\"img\" src=" + _ContentApi.AppPath + "images/ui/icons/calendarViewDay.png" + "></img>&nbsp;";
                            }
                            else
                            {
                                dr[0] += "&nbsp;<img id=\"img\" src=" + _ContentApi.AppPath + "images/ui/icons/contentHtml.png" + "></img>&nbsp;";
                            }
                        }
                        else if (_ReportData[i].ContType == 2)
                        {
                            dr[0] += "&nbsp;<img id=\"img\" src=" + _ContentApi.AppPath + "images/ui/icons/contentForm.png" + "></img>&nbsp;";
                        }
                        else if (_ReportData[i].ContType == 3)
                        {
                            dr[0] += "&nbsp;<img id=\"img\" src=" + _ContentApi.AppPath + "images/ui/icons/contentHTML.png" + "></img>&nbsp;";
                        }
                        else if (_ReportData[i].ContType == 1111)
                        {
                            dr[0] += "&nbsp;<img id=\"img\" src=" + _ContentApi.AppPath + "images/ui/icons/asteriskOrange.png" + "></img>&nbsp;";
                        }
                        else if (_ReportData[i].ContType == 3333)
                        {
                            dr[0] += "&nbsp;<img id=\"img\" src=" + _ContentApi.AppPath + "images/ui/icons/brick.png" + "></img>&nbsp;";
                        }
                        else
                        {
                            dr[0] += "&nbsp;<img id=\"img\" src=" + _ReportData[i].AssetData.Icon + "></img>&nbsp;";
                        }

                        dr[0] += "<input type=\"hidden\" cid=\"" + _ReportData[i].Id + "\" fid=\"" + _ReportData[i].FolderId + "\" name=\"frm_hidden" + i + "\" value=\"0\"> ";
                        if (_ReportData[i].ContType != 2)
                        {
                            dr[0] += "<a href=\"content.aspx?action=" + action + "&LangType=" + _ReportData[i].LanguageId + "&id=" + _ReportData[i].Id + "&callerpage=" + "reports.aspx" + "&origurl=" + EkFunctions.UrlEncode((string)("action=" + _PageAction + "&filtertype=" + _FilterType + "&filterid=" + _FilterId + "&orderby=" + _OrderBy + "&interval=" + _Interval)) + "\" title=\'" + _MessageHelper.GetMessage("generic View") + " \"" + Strings.Replace(_ReportData[i].Title, "\'", "`", 1, -1, 0) + "\"" + "\'>" + _ReportData[i].Title + "</a>";
                        }
                        else
                        {
                            //Link to cmsforms.aspx
                            dr[0] += "<a href=\"cmsform.aspx?action=ViewForm" + "&LangType=" + _ReportData[i].LanguageId + "&form_id=" + _ReportData[i].Id + "&folder_id=" + _ReportData[i].FolderId + "\" title=\'" + _MessageHelper.GetMessage("generic view") + " \"" + Strings.Replace(_ReportData[i].Title, "\'", "`", 1, -1, 0) + "\"" + "\'>" + _ReportData[i].Title + "</a>";
                        }
                        dr[1] = _ReportData[i].Id;
                        dr[2] = "<a href=\"reports.aspx?action=" + _PageAction + "&interval=" + _Interval + "&filtertype=USER&filterId=" + _ReportData[i].UserId + "&orderby=" + _OrderBy + "\" title=\"" + _MessageHelper.GetMessage("click to filter msg") + "\">" + _ReportData[i].EditorLastName + ", " + _ReportData[i].EditorFirstName + "</a>";
                        string _lnk = MakeLink(_ReportData[i]);
                        if (_lnk != "")
                        {
                            dr[2] = _lnk;
                        }
                        else
                        {
                            dr[2] = "<a href=\"reports.aspx?action=" + _PageAction + "&interval=" + _Interval + "&filtertype=USER&filterId=" + _ReportData[i].UserId + "&orderby=" + _OrderBy + "\" title=\"" + _MessageHelper.GetMessage("click to filter msg") + "\">" + _ReportData[i].EditorLastName + ", " + _ReportData[i].EditorFirstName + "</a>";
                        }
                        if (_PageAction == "viewpending")
                        {
                            dr[3] = _ReportData[i].DisplayGoLive;
                        }
                        else if ((_PageAction == "viewexpired") || (_PageAction == "viewtoexpire"))
                        {
                            dr[3] = _ReportData[i].DisplayEndDate;
                        }
                        else
                        {
                            dr[3] = _ReportData[i].DisplayLastEditDate;
                        }
                        if (_PageAction == "ViewToExpire")
                        {
                            dr[4] = _ReportData[i].Path;
                        }
                        else
                        {
                            dr[4] = "<a href=\"reports.aspx?action=" + _PageAction + "&interval=" + _Interval + "&filtertype=path&filterId=" + _ReportData[i].FolderId + "&orderby=" + _OrderBy + "\" title=\"" + _MessageHelper.GetMessage("click to filter msg") + "\">" + _ReportData[i].Path + "</a>";
                        }
                        cLinkArray = cLinkArray + "," + _ReportData[i].Id;
                        fLinkArray = fLinkArray + "," + _ReportData[i].FolderId;
                        lLinkArray = System.Convert.ToString(lLinkArray + "," + _ReportData[i].LanguageId);
                        dt.Rows.Add(dr);
                    }

                    if (cLinkArray.Length > 0)
                    {
                        cLinkArray = Strings.Right(cLinkArray, Strings.Len(cLinkArray) - 1);
                        fLinkArray = Strings.Right(fLinkArray, Strings.Len(fLinkArray) - 1);
                        lLinkArray = Strings.Right(lLinkArray, Strings.Len(lLinkArray) - 1);
                    }

                    litCollectionList.Text = cLinkArray;
                    litFolderList.Text = fLinkArray;
                    litLanguageList.Text = lLinkArray;

                    _DataView = new DataView(dt);
                    dgReport.DataSource = _DataView;
                    dgReport.DataBind();
                }
                else
                {
                    //Currently there is no data to report. Show such message
                    if (EditScheduleHtml.Text.IndexOf("Currently there is no data") == -1)
                    {
                        System.Text.StringBuilder result = new System.Text.StringBuilder();
                        result.Append("<table>");
                        result.Append("<tr><td>").Append(_MessageHelper.GetMessage("msg no data report")).Append("</td></tr>");
                        result.Append("</table>");
                        EditScheduleHtml.Text = EditScheduleHtml.Text + result.ToString();
                    }
                    editSchedule.Visible = true;
                    dgReport.Visible = false;
                }
            }
            else
            {
                // If it is not a chart and report for site activity
                if ((Request.QueryString["btnSubmit"] == "1") && !bIsChart)
                {
                    chart.Visible = false;
                    lblTbl.Visible = false;

                    _SiteData = _ContentApi.GetSiteActivityReportv2_0(pagedata);
                    if (!(_SiteData == null))
                    {
                        ShowSiteActivity();
                        if (!(Request.QueryString["reporttype"] == null) && "export" == Request.QueryString["reporttype"])
                        {
                            Process_Export();
                        }
                    }
                }
            }
        }
        EmailHelper ehelp = new EmailHelper();
        EmailArea.Text = ehelp.MakeEmailArea();

        switch (this._PageAction.ToLower())
        {
            case "viewcheckedin":
                Display_CheckedIn();
                break;
            case "viewcheckedout":
                Display_CheckedOut();
                break;
            case "viewnewcontent":
                Display_NewContent();
                break;
        }
    }

    #endregion

    #region Display

    //v8 reports
    private void Display_CheckedIn()
    {
        //Me.mvReports.SetActiveView(Me.vwReportV8)
        //_ViewCheckedIn = CType(LoadControl("controls/reports/ViewCheckedIn.ascx"), Ektron.Workarea.Reports.ViewCheckedIn)
        //_ViewCheckedIn.ID = "uxViewCheckedIn"
        //_ViewCheckedIn.ReportData = _ReportData
        //Me.phReportControl.Controls.Add(_ViewCheckedIn)
    }

    private void Display_CheckedOut()
    {
        // Me.mvReports.SetActiveView(Me.vwReportV8)
        //Me.ucViewCheckedOut.CurrentPage = _PageInfo.CurrentPage - 1
        //Me.ucViewCheckedOut.ReportData = _ReportData
        //Me.ucViewCheckedOut.TotalPages = _PageInfo.TotalRecords / _PageInfo.RecordsPerPage
        //If Me.ucViewCheckedOut.TotalPages > 1 Then
        //    Me.ucViewCheckedOut.ShowPaging = True
        //End If
        //_ViewCheckedOut = CType(LoadControl("controls/reports/ViewCheckedOut.ascx"), Ektron.Workarea.Reports.ViewCheckedOut)
        //_ViewCheckedOut.ID = "uxViewCheckedOut"
        //_ViewCheckedOut.ReportData = _ReportData
        //Me.phReportControl.Controls.Add(_ViewCheckedOut)
    }

    private void Display_NewContent()
    {
        //Me.mvReports.SetActiveView(Me.vwReportV8)
        //_NewContent = CType(LoadControl("controls/reports/NewContent.ascx"), Ektron.Workarea.Reports.NewContent)
        //_NewContent.ID = "uxNewContent"
        //_NewContent.ReportData = _ReportData
        //Me.phReportControl.Controls.Add(_NewContent)
    }

    //legacy
    private void IncludeContentToExpireJS()
    {
        string strJS;
        strJS = "<script type=\"text/javascript\">" + "\r\n";
        strJS += "function ReportContentToExpire() {" + "\r\n";
        strJS += " if (checkInterval()) {";
        strJS += "   document.location.href = \"reports.aspx?action=ViewToExpire&interval=\" + document.forms[0]." + this.txtInterval.ClientID + ".value;" + "\r\n";
        strJS += "} }" + "\r\n";

        strJS += "function checkInterval() {";
        strJS += "document.getElementById(\'" + txtInterval.ClientID + "\').value = Trim(document.getElementById(\'" + txtInterval.ClientID + "\').value);";
        strJS += "if ((document.getElementById(\'" + txtInterval.ClientID + "\').value == \"\" ) || !(isNumeric(document.getElementById(\'" + txtInterval.ClientID + "\').value))) ";
        strJS += "{ alert(\"You must enter a numeric value for the this field\");";
        //strJS += "document.getElementById(\'" + txtInterval.ClientID + "\').focus();return false;} else { return true;} }";
        strJS += "document.getElementById(\'" + selInterval.ClientID + "\').focus();return false;} else { return true;} }";

        strJS += "function selChange(sel) {" + "\r\n";
        strJS += "var Interval = sel[sel.selectedIndex].value;" + "\r\n";
        strJS += "document.getElementById(\'" + txtInterval.ClientID + "\').value = Interval;" + "\r\n";
        strJS += "ReportContentToExpire();" + "\r\n";
        strJS += "return Interval;" + "\r\n";
        strJS += "}" + "\r\n";

        strJS += "</script>" + "\r\n";
        Page.ClientScript.RegisterClientScriptBlock(this.GetType(), "contenttoexpire", strJS);
    }
    private void Display_AsynchLogFile()
    {
        string m_sReportText = "";
        reportstoolbar m_reportsToolBar;
        System.Web.UI.WebControls.BoundColumn colBound = new System.Web.UI.WebControls.BoundColumn();
        DataTable dt = new DataTable();
        DataRow dr;
        SiteAPI m_SiteAPI = new SiteAPI();


        m_reportsToolBar = (reportstoolbar)(LoadControl("controls/reports/reportstoolbar.ascx"));
        ToolBarHolder.Controls.Add(m_reportsToolBar);
        m_reportsToolBar.AppImgPath = _AppImgPath;

        _TitleBarMsg = _MessageHelper.GetMessage("lbl asynchronous log file report");

        m_reportsToolBar.PageAction = _PageAction;
        m_reportsToolBar.FilterType = _FilterType;
        m_reportsToolBar.TitleBarMsg = _TitleBarMsg;
        m_reportsToolBar.MultilingualEnabled = _EnableMultilingual;
        m_reportsToolBar.ContentLang = _ContentApi.ContentLanguage;

        txtInterval.Visible = false;
        selInterval.Visible = false;
        string st = _MessageHelper.GetMessage("alert asynchronous Data Processor");
        string st1 = _MessageHelper.GetMessage("alert problem communication");
        try
        {
            if (!(m_SiteAPI.GetSiteVariables(-1).AsynchronousLocation != ""))
            {
                throw (new Exception(st));
            }

            m_sReportText = _ContentApi.EkContentRef.ViewAsynchLogFile();
            m_sReportText = m_sReportText.Replace(Constants.vbLf, "<br/>");

            colBound.DataField = "Log File";
            colBound.HeaderText = _MessageHelper.GetMessage("lbl log file");
            colBound.ItemStyle.Wrap = false;
            colBound.ItemStyle.VerticalAlign = VerticalAlign.Top;
            colBound.ItemStyle.HorizontalAlign = HorizontalAlign.Left;
            colBound.HeaderStyle.CssClass = "title-header";

            dgReport.Columns.Add(colBound);
            dgReport.BorderColor = System.Drawing.Color.White;
            dgReport.Visible = true;

            dt.Columns.Add(new DataColumn("Log File", typeof(string)));
            dr = dt.NewRow();
            dr[0] = m_sReportText;
            dt.Rows.Add(dr);

            DataView dv = new DataView(dt);
            dgReport.DataSource = dv;
            dgReport.DataBind();
        }
        catch (Exception ex)
        {
            if (ex.Message.IndexOf( _MessageHelper.GetMessage("alert Please specify location")) > -1)
            {
                Utilities.ShowError(ex.Message);
            }
            else
            {
                Utilities.ShowError(st1);
            }
        }
        finally
        {
            _ContentApi = null;
        }
    }
    private void DisplayDateFields()
    {
        EkDTSelector dateSchedule;
        dateSchedule = this._ContentApi.EkDTSelectorRef;
        dateSchedule.formName = "selections";
        dateSchedule.extendedMeta = true;
        dateSchedule.formElement = "start_date";
        dateSchedule.spanId = "start_date_span";
        if (Information.IsDate(_StartDate))
        {
            dateSchedule.targetDate = DateTime.Parse(_StartDate);
        }
        dtStart.Text = dateSchedule.displayCultureDateTime(true, "", "");
        dateSchedule.formElement = "end_date";
        dateSchedule.spanId = "end_date_span";
        if (Information.IsDate(_EndDate))
        {
            dateSchedule.targetDate = DateTime.Parse(_EndDate);
        }
        else
        {
            if (_StartDate != "")
            {
                // if start-date is set, and no end-date given, chose now as the end-limit:
                //dateSchedule.targetDate = Date.Now
                DateTime tempDate = new DateTime();
                tempDate.AddHours(System.Convert.ToDouble(23 - tempDate.Hour));
                tempDate.AddMinutes(System.Convert.ToDouble(59 - tempDate.Minute));
                tempDate.AddSeconds(System.Convert.ToDouble(59 - tempDate.Second));
                tempDate.AddMilliseconds(System.Convert.ToDouble(999 - tempDate.Millisecond));
                dateSchedule.targetDate = tempDate;
                tempDate = Convert.ToDateTime(null);
            }
        }
        dtEnd.Text = dateSchedule.displayCultureDateTime(true, "", "");
    }
    private void Display_ReportTypes()
    {

    }
    private void Display_ContentFlags()
    {
        Ektron.Cms.ContentFlagData[] aFlags = (Ektron.Cms.ContentFlagData[])Array.CreateInstance(typeof(Ektron.Cms.ContentFlagData), 0);
        reportstoolbar m_reportsToolBar;
        System.Web.UI.WebControls.BoundColumn colBound = new System.Web.UI.WebControls.BoundColumn();
        DataTable dt = new DataTable();
        int iTotalCF = 0;

        m_reportsToolBar = (reportstoolbar)(LoadControl("controls/reports/reportstoolbar.ascx"));
        ToolBarHolder.Controls.Add(m_reportsToolBar);
        m_reportsToolBar.AppImgPath = _AppImgPath;

        _TitleBarMsg = this._MessageHelper.GetMessage("lbl report flags");

        m_reportsToolBar.PageAction = _PageAction;
        m_reportsToolBar.FilterType = _FilterType;
        m_reportsToolBar.TitleBarMsg = _TitleBarMsg;
        m_reportsToolBar.MultilingualEnabled = _EnableMultilingual;
        m_reportsToolBar.ContentLang = _ContentApi.ContentLanguage;

        txtInterval.Visible = false;
        selInterval.Visible = false;
        tr_phUpdateActivity.Visible = true;

        try
        {
            if (Page.IsPostBack == true && IsReviewPost() == true)
            {
                //
            }
            else
            {
                this.IncludeContentFlagJS();

                System.TimeSpan tSpan;
                int iValue = 10;
                if (Request.QueryString["span"] != "" && Information.IsNumeric(Request.QueryString["span"]) && Convert.ToInt32(Request.QueryString["span"]) > 0)
                {
                    iValue = System.Convert.ToInt32(Request.QueryString["span"]);
                }
                else if (Request.Form[hdn_timespan.ClientID] != "" && Information.IsNumeric(Request.Form[hdn_timespan.ClientID]))
                {
                    iValue = Convert.ToInt32(Request.Form[hdn_timespan.ClientID]);
                }
                hdn_timespan.Value = iValue.ToString();
                tSpan = new System.TimeSpan((iValue), 0, 0, 0);

                System.Web.UI.WebControls.TextBox txtDays = new System.Web.UI.WebControls.TextBox();
                txtDays.Text = iValue.ToString();
                txtDays.Width = Unit.Pixel(25);
                txtDays.ID = "txtdays";
                if (_FilterId > 0)
                {
                    System.Web.UI.WebControls.Literal ltrPath = new System.Web.UI.WebControls.Literal();
                    ltrPath.Text = (string)(_MessageHelper.GetMessage("lbl folder") + "&#160;&#160;" + _ContentApi.EkContentRef.GetFolderPath(_FilterId) + "<br />");
                    this.phUpdateActivity.Controls.Add(ltrPath);
                }
                System.Web.UI.WebControls.Literal ltrfront = new System.Web.UI.WebControls.Literal();
                ltrfront.Text = (string)(_MessageHelper.GetMessage("msg past flag show") + "&nbsp;");
                System.Web.UI.WebControls.Literal ltrback = new System.Web.UI.WebControls.Literal();
                ltrback.Text = "&nbsp;" + _MessageHelper.GetMessage("lbl days") + "&nbsp;";
                this.phUpdateActivity.Controls.Add(ltrfront);
                this.phUpdateActivity.Controls.Add(txtDays);
                this.phUpdateActivity.Controls.Add(ltrback);
                aFlags = (new Ektron.Cms.Community.FlaggingAPI()).GetAllFlagEntries(_FilterId, 0, 0, DateTime.Now.Subtract(tSpan), DateTime.Now, ref iTotalCF);
                if (aFlags.Length > 0)
                {
                    m_reportsToolBar.Data = aFlags;
                    //Me.SiteUpdateActivity.Visible = True
                    //tr_SiteUpdateActivity.Visible=True
                    //Me.SiteUpdateActivity.Text = "<a href=""javascript: CheckApproveReset(true);"" >Mark all approved</a>&nbsp;&nbsp;<a href=""javascript: CheckApproveReset(false);"" >Mark all declined</a>"
                }

                dgReport.DataSource = this.CreateContentFlagSource(aFlags);
                dgReport.CellPadding = 3;

                colBound = new System.Web.UI.WebControls.BoundColumn();
                colBound.DataField = "TITLE";
                colBound.HeaderText = _MessageHelper.GetMessage("generic Title");
                colBound.HeaderStyle.CssClass = "title-header";
                colBound.ItemStyle.VerticalAlign = VerticalAlign.Top;
                colBound.ItemStyle.Wrap = false;
                dgReport.Columns.Add(colBound);

                colBound = new System.Web.UI.WebControls.BoundColumn();
                colBound.DataField = "USERNAME";
                colBound.HeaderText = _MessageHelper.GetMessage("display name label");
                colBound.HeaderStyle.CssClass = "title-header";
                colBound.ItemStyle.VerticalAlign = VerticalAlign.Top;
                colBound.ItemStyle.HorizontalAlign = HorizontalAlign.Left;
                colBound.ItemStyle.Wrap = false;
                dgReport.Columns.Add(colBound);

                colBound = new System.Web.UI.WebControls.BoundColumn();
                colBound.DataField = "DATETIME";
                colBound.HeaderText = _MessageHelper.GetMessage("generic date no colon");
                colBound.HeaderStyle.CssClass = "title-header";
                colBound.ItemStyle.VerticalAlign = VerticalAlign.Top;
                colBound.ItemStyle.HorizontalAlign = HorizontalAlign.Left;
                colBound.ItemStyle.Wrap = false;
                dgReport.Columns.Add(colBound);

                colBound = new System.Web.UI.WebControls.BoundColumn();
                colBound.DataField = "FLAG";
                colBound.HeaderText = _MessageHelper.GetMessage("flag label");
                colBound.HeaderStyle.CssClass = "title-header";
                colBound.ItemStyle.Wrap = false;
                colBound.ItemStyle.VerticalAlign = VerticalAlign.Top;
                colBound.ItemStyle.HorizontalAlign = HorizontalAlign.Left;
                colBound.ItemStyle.Wrap = false;
                dgReport.Columns.Add(colBound);

                colBound = new System.Web.UI.WebControls.BoundColumn();
                colBound.DataField = "COMMENTS";
                colBound.HeaderText = _MessageHelper.GetMessage("comment text");
                colBound.HeaderStyle.CssClass = "title-header";
                colBound.ItemStyle.VerticalAlign = VerticalAlign.Top;
                colBound.ItemStyle.HorizontalAlign = HorizontalAlign.Left;
                colBound.ItemStyle.Wrap = true;
                dgReport.Columns.Add(colBound);

                dgReport.DataBind();
            }
        }
        catch (Exception)
        {

        }
        finally
        {
            _ContentApi = null;
        }
    }
    private void Display_ContentReviews()
    {
        Ektron.Cms.ContentReviewData[] aReviews = (Ektron.Cms.ContentReviewData[])Array.CreateInstance(typeof(Ektron.Cms.ContentReviewData), 0);
        reportstoolbar m_reportsToolBar;
        System.Web.UI.WebControls.BoundColumn colBound = new System.Web.UI.WebControls.BoundColumn();
        DataTable dt = new DataTable();
        int iTotalCR = 0;

        m_reportsToolBar = (reportstoolbar)(LoadControl("controls/reports/reportstoolbar.ascx"));
        ToolBarHolder.Controls.Add(m_reportsToolBar);
        m_reportsToolBar.AppImgPath = _AppImgPath;

        _TitleBarMsg = this._MessageHelper.GetMessage("lbl report reviews");

        m_reportsToolBar.PageAction = _PageAction;
        m_reportsToolBar.FilterType = _FilterType;
        m_reportsToolBar.TitleBarMsg = _TitleBarMsg;
        m_reportsToolBar.MultilingualEnabled = _EnableMultilingual;
        m_reportsToolBar.ContentLang = _ContentApi.ContentLanguage;

        txtInterval.Visible = false;
        selInterval.Visible = false;

        try
        {
            if (Page.IsPostBack == true && IsReviewPost() == true)
            {
                for (int i = 0; i <= (Request.Form.Count - 1); i++)
                {
                    if (Request.Form.Keys[i].ToString().IndexOf("cr_a_") == 0)
                    {
                        ContentReviewData crReview = new ContentReviewData();
                        int iReviewID = 0;
                        string sAction = "";

                        iReviewID = System.Convert.ToInt32(Strings.Split(Request.Form[i], "_", -1, 0)[2]);
                        sAction = (string)(Strings.Split(Request.Form[i], "_", -1, 0)[1]);
                        if (iReviewID > 0)
                        {
                            crReview = this._ContentApi.EkContentRef.GetContentRating(iReviewID);
                            if (sAction.ToLower() == "app")
                            {
                                crReview.State = Ektron.Cms.Common.EkEnumeration.ContentReviewState.Approved;
                            }
                            else if (sAction.ToLower() == "dec")
                            {
                                crReview.State = Ektron.Cms.Common.EkEnumeration.ContentReviewState.Rejected;
                            }
                            crReview = this._ContentApi.EkContentRef.UpdateContentReview(crReview);
                        }
                    }
                }
                this.Response.Redirect("reports.aspx?action=ContentReviews", false);
            }
            else
            {
                if (_FilterId > 0)
                {
                    System.Web.UI.WebControls.Literal ltrPath = new System.Web.UI.WebControls.Literal();
                    ltrPath.Text = (string)(_MessageHelper.GetMessage("lbl folder") + "&#160;&#160;" + _ContentApi.EkContentRef.GetFolderPath(_FilterId) + "<br /><br />");
                    this.phUpdateActivity.Controls.Add(ltrPath);
                    aReviews = this._ContentApi.EkContentRef.GetUserReviewTotals(this._ContentApi.RequestInformationRef.PagingSize, new long[] { _FilterId }, iTotalCR, _PageInfo);
                }
                else
                {
                    _PageInfo.RecordsPerPage = _ContentApi.RequestInformationRef.PagingSize;
                    _PageInfo.CurrentPage = System.Convert.ToInt32(this.uxPaging.SelectedPage + 1);
                    aReviews = this._ContentApi.EkContentRef.GetUserReviewTotals(this._ContentApi.RequestInformationRef.PagingSize, ref iTotalCR, _PageInfo);
                    if (aReviews != null && _PageInfo.TotalPages > 1)
                    {
                        this.uxPaging.Visible = true;
                        this.uxPaging.TotalPages = _PageInfo.TotalPages;
                        this.uxPaging.CurrentPageIndex = _PageInfo.CurrentPage - 1;
                    }
                    else
                    {
                        this.uxPaging.Visible = false;
                    }
                }
                }

                if (aReviews.Length > 0)
                {
                    m_reportsToolBar.Data = aReviews;
                    System.Web.UI.WebControls.Literal ltrPath = new System.Web.UI.WebControls.Literal();
                    ltrPath.Text = "<ul class=\'buttonWrapper buttonWrapperLeft\'><li><a class=\"greenHover button selectAllButton buttonInlineBlock\" href=\"#\" onclick=\"CheckApproveReset(true); return false;\" >" + _MessageHelper.GetMessage("lnk mark all approve") + "</a></li><li><a class=\"redHover button selectNoneButton buttonLeft\" href=\"#\" onclick=\"CheckApproveReset(false);\">" + _MessageHelper.GetMessage("lnk mark all declined") + "</a></li></ul>";
                    this.phUpdateActivity.Controls.Add(ltrPath);
                    

                dgReport.DataSource = this.CreateContentReviewSource(aReviews);
                dgReport.CellPadding = 3;

                colBound = new System.Web.UI.WebControls.BoundColumn();
                colBound.DataField = "APPROVE";
                colBound.HeaderText = _MessageHelper.GetMessage("generic approve title");
                colBound.HeaderStyle.CssClass = "title-header";
                colBound.ItemStyle.VerticalAlign = VerticalAlign.Top;
                colBound.ItemStyle.HorizontalAlign = HorizontalAlign.Center;
                colBound.ItemStyle.Wrap = false;
                dgReport.Columns.Add(colBound);

                colBound = new System.Web.UI.WebControls.BoundColumn();
                colBound.DataField = "DECLINE";
                colBound.HeaderText = _MessageHelper.GetMessage("btn decline");
                colBound.HeaderStyle.CssClass = "title-header";
                colBound.ItemStyle.VerticalAlign = VerticalAlign.Top;
                colBound.ItemStyle.HorizontalAlign = HorizontalAlign.Center;
                colBound.ItemStyle.Wrap = false;
                dgReport.Columns.Add(colBound);

                colBound = new System.Web.UI.WebControls.BoundColumn();
                colBound.DataField = "TITLE";
                colBound.HeaderText = _MessageHelper.GetMessage("generic Title");
                colBound.HeaderStyle.CssClass = "title-header";
                colBound.ItemStyle.VerticalAlign = VerticalAlign.Top;
                colBound.ItemStyle.Wrap = false;
                dgReport.Columns.Add(colBound);

                colBound = new System.Web.UI.WebControls.BoundColumn();
                colBound.DataField = "USERNAME";
                colBound.HeaderText = _MessageHelper.GetMessage("display name label");
                colBound.HeaderStyle.CssClass = "title-header";
                colBound.ItemStyle.VerticalAlign = VerticalAlign.Top;
                colBound.ItemStyle.HorizontalAlign = HorizontalAlign.Left;
                colBound.ItemStyle.Wrap = false;
                dgReport.Columns.Add(colBound);

                colBound = new System.Web.UI.WebControls.BoundColumn();
                colBound.DataField = "DATETIME";
                colBound.HeaderText = _MessageHelper.GetMessage("generic date no colon");
                colBound.HeaderStyle.CssClass = "title-header";
                colBound.ItemStyle.VerticalAlign = VerticalAlign.Top;
                colBound.ItemStyle.HorizontalAlign = HorizontalAlign.Left;
                colBound.ItemStyle.Wrap = false;
                dgReport.Columns.Add(colBound);

                colBound = new System.Web.UI.WebControls.BoundColumn();
                colBound.DataField = "RATING";
                colBound.HeaderText = _MessageHelper.GetMessage("rating label");
                colBound.HeaderStyle.CssClass = "title-header";
                colBound.ItemStyle.Wrap = false;
                colBound.ItemStyle.VerticalAlign = VerticalAlign.Top;
                colBound.ItemStyle.HorizontalAlign = HorizontalAlign.Left;
                dgReport.Columns.Add(colBound);

                colBound = new System.Web.UI.WebControls.BoundColumn();
                colBound.DataField = "USER_COMMENTS";
                colBound.HeaderText = _MessageHelper.GetMessage("comment text");
                colBound.HeaderStyle.CssClass = "title-header";
                colBound.ItemStyle.VerticalAlign = VerticalAlign.Top;
                colBound.ItemStyle.HorizontalAlign = HorizontalAlign.Left;
                colBound.ItemStyle.Wrap = true;
                dgReport.Columns.Add(colBound);

                dgReport.DataBind();
            }
        }
        catch (Exception)
        {

        }
        finally
        {
            _ContentApi = null;
        }
    }

    #endregion

    #region Helpers

    private ICollection CreateContentFlagSource(ContentFlagData[] FlagList)
    {
        DataTable dt = new DataTable();
        DataRow dr;
        EkDTSelector dtS;
        dtS = _ContentApi.EkDTSelectorRef;

        dt.Columns.Add(new DataColumn("TITLE", typeof(string)));
        dt.Columns.Add(new DataColumn("USERNAME", typeof(string)));
        dt.Columns.Add(new DataColumn("DATETIME", typeof(string)));
        dt.Columns.Add(new DataColumn("FLAG", typeof(string)));
        dt.Columns.Add(new DataColumn("COMMENTS", typeof(string)));

        for (int i = 0; i <= (FlagList.Length - 1); i++)
        {
            if ((FlagList[i]).LanguageId == _ContentApi.ContentLanguage)
            {
                dr = dt.NewRow();
                if (FlagList[i].ContentType == 1)
                {
                    if (FlagList[i].SubType == EkEnumeration.CMSContentSubtype.WebEvent)
                    {
                        dr[0] = "<img src=" + _ContentApi.AppPath + "images/ui/icons/calendarViewDay.png" + "></img>&nbsp;";
                    }
                    else
                    {
                        dr[0] = "<img src=" + _ContentApi.AppPath + "images/ui/icons/contentHtml.png" + "></img>&nbsp;";
                    }
                }
                else if (FlagList[i].ContentType == 2)
                {
                    dr[0] = "<img src=" + _ContentApi.AppPath + "images/ui/icons/contentForm.png" + "></img>&nbsp;";
                }
                else
                {
                    dr[0] = "<img src=" + FlagList[i].AssetData.Icon + "></img>&nbsp;";
                }
                // dr(0) = "<a href=""contentflagging/addeditcontentflag.aspx?action=view&id=" & FlagList(i).EntryId.ToString() & "&cid=" & FlagList(i).Id.ToString() & """>" & FlagList(i).Title & "</a>"
                dr[0] += "<a href=\"ContentFlagging/addeditcontentflag.aspx?action=view&id=" + FlagList[i].FlagId.ToString() + "&cid=" + FlagList[i].Id.ToString() + "\">" + FlagList[i].Title + "</a>";
                if (FlagList[i].FlaggedUser.Id == 0)
                {
                    dr[1] = "<font color=\"gray\">" + this._MessageHelper.GetMessage("lbl anon") + "</font>";
                }
                else
                {
                    dr[1] = FlagList[i].FlaggedUser.DisplayName;
                }
                dr[2] = FlagList[i].FlagDate.ToShortDateString();
                dr[3] = FlagList[i].FlagName;
                dr[4] = FlagList[i].FlagComment;
                dt.Rows.Add(dr);
            }
        }
        DataView dv = new DataView(dt);
        return dv;
    }
    private void IncludeContentFlagJS()
    {
        string strJS;
        strJS = "<script type=\"text/javascript\">" + "\r\n";
        strJS += "function ReportContentFlags() {" + "\r\n";
        strJS += " if (checkInterval()) {";
        strJS += "   document.location.href = \"reports.aspx?action=ContentFlags&LangType=" + _ContentLanguage + "&span=\" + document.getElementById(\'txtdays\').value" + (_FilterId > 0 ? (" + \'&filterid=" + _FilterId.ToString() + "\'") : "") + ";" + "\r\n";
        strJS += "} }" + "\r\n";
        strJS += "function checkInterval() {";
        strJS += "if (!(isNumeric(document.getElementById(\'txtdays\').value)) || (document.getElementById(\'txtdays\').value < 1 )) ";
        strJS += "{ alert(\"You must enter a number greater than one for this field\");";
        strJS += "document.getElementById(\'txtdays\').focus();return false;} else { return true;} }";
        strJS += "</script>" + "\r\n";
        Page.ClientScript.RegisterClientScriptBlock(this.GetType(), "contenttoexpire", strJS);
    }
    private void RedirectIfNotLoggedIn()
    {
        if (_ContentApi.UserId == 0 || _ContentApi.RequestInformationRef.IsMembershipUser == 1)
        {
            Response.Redirect("login.aspx?fromLnkPg=1", false);
            return;
        }
    }
    private void ShowSiteActivity()
    {
        System.Web.UI.WebControls.BoundColumn colBound = new System.Web.UI.WebControls.BoundColumn();
        DataTable dt = new DataTable();
        //Dim dr As DataRow
        Repeater rptFolders = new Repeater();
        Repeater rptContents = new Repeater();
        int iTotalUpdated = 0;
        int iPagesUpdated = 0;
        int iTotalPages = 0;
        string sReport = "";
        StringBuilder sbHtml = new StringBuilder();
        int i = 0;
        int j = 0;
        Collection data; //ContentData()
        Table main;
        Table[] tblrep;
        Table[] Pageinnertbl;
        Table[] folderinnertbl;
        TableRow rowMain;
        TableCell cellMain;
        TableRow[] rowPath;
        TableCell[] cellPath;
        TableRow[] rowfolder;
        TableCell[] cellfolder;
        TableRow[] rowPage;
        TableCell[] cellPage;

        TableRow[] rowtbl12;
        TableRow[] rowtbl22;
        TableCell[] celltbl211;
        TableCell[] celltbl212;
        TableCell[] celltbl213;
        TableCell[] celltbl221;
        TableCell[] celltbl222;
        TableCell[] celltbl223;
        TableRow[] rowtbl31;
        TableCell[] celltbl311;
        TableCell[] celltbl312;
        TableCell[] celltbl313;
        TableCell[] celltbl314;

        TableRow[] rowtbl3;
        TableCell[] celltbl31;
        TableCell[] celltbl32;
        TableCell[] celltbl33;
        TableCell[] celltbl34;

        Label lbltbl2 = new Label();
        Label lbltbl3 = new Label();
        Label[] lblPath;

        Label[] lblFolderHeader1;
        Label[] lblFolderHeader2;
        Label[] lblFolderHeader3;
        Label[] lblTotalUpdated;
        Label[] lblPagesUpdated;
        Label[] lblTotalPages;
        Label[] lblPgHeader1;
        Label[] lblPgHeader2;
        Label[] lblPgHeader3;
        Label[] lblPgHeader4;
        Label[] lblPg1;
        Label[] lblPg2;
        Label[] lblPg3;
        Label[] lblPg4;

        ArrayList aPath = new ArrayList();
        ArrayList aTotalUpdated = new ArrayList();
        ArrayList aPagesUpdated = new ArrayList();
        ArrayList aTotalPages = new ArrayList();
        ArrayList aPageData = new ArrayList();
        ArrayList aFolder = new ArrayList();
        string sTempHolder = "";
        int iFolderCount = 0;
        string pathName;

        //ReportDataGrid
        _ReportTableHtml = ""; //clear the table for export
        if (!(_SiteData == null))
        {
            main = new Table();
            main.Controls.Clear();
            main.ID = "UpdateActivityTbl";
            rowMain = new TableRow();
            main.Controls.Add(rowMain);
            cellMain = new TableCell();

            data = _SiteData;
            for (i = 1; i <= data.Count; i++)
            {
                //loop through the data to create a collection of array for the information needed to display
                //((Collection)((ArrayList)aFolder[j - 1])[i])["FolderName"]
                //if (((Collection)((ArrayList)aFolder[j - 1])[i])["FolderName"] != aPath[j - 1])
                sReport = (string)((Collection)data[i])["FolderName"];
                if (sTempHolder != sReport)
                {
                    if (sTempHolder != "")
                    {
                        aPath.Add(sTempHolder);
                        aTotalUpdated.Add(iTotalUpdated);
                        aPagesUpdated.Add(iPagesUpdated);
                        aTotalPages.Add(iTotalPages);
                        aFolder.Add(aPageData);
                        iTotalUpdated = 0;
                        iPagesUpdated = 0;
                        iTotalPages = 0;
                        aPageData = null;
                        aPageData = new ArrayList();
                    }
                    sTempHolder = sReport;
                }
                iTotalUpdated = iTotalUpdated + Convert.ToInt32(((Collection)data[i])["Updates"]);
                //((Collection)data[i])["Updates"]
                if (Convert.ToInt32(((Collection)data[i])["Updates"]) > 0)
                {
                    iPagesUpdated++;
                }
                iTotalPages++;
                aPageData.Add(data[i]);
            }
            //add the last set of data to the arrays
            aPath.Add(sReport);
            aTotalUpdated.Add(iTotalUpdated);
            aPagesUpdated.Add(iPagesUpdated);
            aTotalPages.Add(iTotalPages);
            aFolder.Add(aPageData);

            if (1 == aPath.Count && Strings.Len(aPath[0]) == 0)
            {
                lblPath = new Label[2];
                lblPath[0] = new Label();
                lblPath[0].Text = "<p>" + _MessageHelper.GetMessage("no page found") + "</p>";
                cellMain.Controls.Add(lblPath[0]);
                rowMain.Controls.Add(cellMain);
            }
            else
            {
                iFolderCount = aPath.Count;
                tblrep = new Table[iFolderCount + 1];
                rowPath = new TableRow[iFolderCount + 1];
                cellPath = new TableCell[iFolderCount + 1];
                lblPath = new Label[iFolderCount + 1];
                rowfolder = new TableRow[iFolderCount + 1];
                cellfolder = new TableCell[iFolderCount + 1];
                rowPage = new TableRow[iFolderCount + 1];
                cellPage = new TableCell[iFolderCount + 1];
                Pageinnertbl = new Table[iFolderCount + 1];
                folderinnertbl = new Table[iFolderCount + 1];
                rowtbl12 = new TableRow[iFolderCount + 1];
                celltbl211 = new TableCell[iFolderCount + 1];
                celltbl212 = new TableCell[iFolderCount + 1];
                celltbl213 = new TableCell[iFolderCount + 1];
                rowtbl22 = new TableRow[iFolderCount + 1];
                celltbl221 = new TableCell[iFolderCount + 1];
                celltbl222 = new TableCell[iFolderCount + 1];
                celltbl223 = new TableCell[iFolderCount + 1];
                rowtbl31 = new TableRow[iFolderCount + 1];
                celltbl311 = new TableCell[iFolderCount + 1];
                celltbl312 = new TableCell[iFolderCount + 1];
                celltbl313 = new TableCell[iFolderCount + 1];
                celltbl314 = new TableCell[iFolderCount + 1];
                lblPgHeader1 = new Label[iFolderCount + 1];
                lblPgHeader2 = new Label[iFolderCount + 1];
                lblPgHeader3 = new Label[iFolderCount + 1];
                lblPgHeader4 = new Label[iFolderCount + 1];
                lblFolderHeader1 = new Label[iFolderCount + 1];
                lblFolderHeader2 = new Label[iFolderCount + 1];
                lblFolderHeader3 = new Label[iFolderCount + 1];
                lblTotalUpdated = new Label[iFolderCount + 1];
                lblPagesUpdated = new Label[iFolderCount + 1];
                lblTotalPages = new Label[iFolderCount + 1];
                for (j = 1; j <= iFolderCount; j++)
                {
                    //!!!--Important---!!!
                    //Var Definations:
                    //j: Outter loop, for the folders selected.
                    //i: mid-lv loop, same number of j, but used to seek the correct dataset.
                    //k: inner loop, generate the inner table for items being updated.
                    //____________________________________________________________________________________________________________________
                    //| <b>aPath(j-1)</b> "Folder Path"                                                                                   |
                    //|___________________________________________________________________________________________________________________|
                    //| Total Updates                            # Pages Updated                                       Total Pages        |
                    //| aTotalUpdated(j-1)	                    aPagesUpdated(j-1)                                    aTotalPages(j-1)   |
                    //|___________________________________________________________________________________________________________________|
                    //| _________________________________________________________________________________________________________________ |
                    //|| Page Name              | Updates                  | Last Updated                 | User Name                    ||
                    //|| aFolder(j)(i)("Title") | aFolder(j)(i)("Updates") | aFolder(j)(i)("LastUpdated") | aFolder(j)(i)("UserNames")   ||
                    //||________________________|__________________________|______________________________|______________________________||
                    //|___________________________________________________________________________________________________________________|

                    //folder table
                    tblrep[j] = new Table();
                    tblrep[j].CssClass = "ektronGrid";
                    tblrep[j].Controls.Clear();


                    //folder path cell
                    rowPath[j] = new TableRow();
                    tblrep[j].Controls.Add(rowPath[j]);
                    cellPath[j] = new TableCell();
                    lblPath[j] = new Label();

                    pathName = (string)(aPath[j - 1].ToString().Substring(1, Strings.Len(aPath[j - 1]) - 1));
                    if ("" == pathName || "/" == pathName)
                    {
                        pathName = _ContentApi.GetFolderById(0).Name;
                    }

                    if ("ev" == _ReportDisplay) // show expand link in executive view
                    {
                        lblPath[j].Text = "<a href=\"javascript://\" onclick=\"showDetails(\'" + j + "\');\"><b>" + pathName + "</b></a>";
                    }
                    else
                    {
                        lblPath[j].Text = pathName;
                    }
                    lblPath[j].Font.Size = FontUnit.Point(14);

                    //folder activity cell
                    rowfolder[j] = new TableRow();
                    tblrep[j].Controls.Add(rowfolder[j]);
                    cellfolder[j] = new TableCell();
                    //folder activity table, gives the summary of the folder activity.
                    folderinnertbl[j] = new Table();
                    folderinnertbl[j].ID = (string)("folderActivity_" + j);
                    folderinnertbl[j].Controls.Clear();
                    folderinnertbl[j].Width = 500;
                    if ("dv" == _ReportDisplay) // not show in detail view
                    {
                        folderinnertbl[j].Visible = false;
                    }
                    else
                    {
                        folderinnertbl[j].Visible = true;
                    }
                    rowtbl12[j] = new TableRow();
                    folderinnertbl[j].Controls.Add(rowtbl12[j]);
                    celltbl211[j] = new TableCell();
                    celltbl212[j] = new TableCell();
                    celltbl213[j] = new TableCell();
                    celltbl211[j].HorizontalAlign = HorizontalAlign.Center;
                    celltbl212[j].HorizontalAlign = HorizontalAlign.Center;
                    celltbl213[j].HorizontalAlign = HorizontalAlign.Center;
                    lblFolderHeader1[j] = new Label();
                    lblFolderHeader2[j] = new Label();
                    lblFolderHeader3[j] = new Label();
                    lblFolderHeader1[j].Text = _MessageHelper.GetMessage("lbl total updates");
                    lblFolderHeader2[j].Text = _MessageHelper.GetMessage("lbl pages updated");
                    lblFolderHeader3[j].Text = _MessageHelper.GetMessage("lbl total pages");
                    rowtbl22[j] = new TableRow();
                    folderinnertbl[j].Controls.Add(rowtbl22[j]);
                    celltbl221[j] = new TableCell();
                    celltbl222[j] = new TableCell();
                    celltbl223[j] = new TableCell();
                    celltbl221[j].HorizontalAlign = HorizontalAlign.Center;
                    celltbl222[j].HorizontalAlign = HorizontalAlign.Center;
                    celltbl223[j].HorizontalAlign = HorizontalAlign.Center;
                    lblTotalUpdated[j] = new Label();
                    lblPagesUpdated[j] = new Label();
                    lblTotalPages[j] = new Label();
                    lblTotalUpdated[j].Text = aTotalUpdated[j - 1].ToString();
                    lblPagesUpdated[j].Text = aPagesUpdated[j - 1].ToString();
                    lblTotalPages[j].Text = aTotalPages[j - 1].ToString();

                    //page activity cell
                    cellPage[j] = new TableCell();
                    rowPage[j] = new TableRow();
                    rowPage[j].BackColor = System.Drawing.Color.White;
                    tblrep[j].Controls.Add(rowPage[j]);
                    //page activity table, give the details of the page activity in this folder.
                    Pageinnertbl[j] = new Table();
                    Pageinnertbl[j].Controls.Clear();
                    Pageinnertbl[j].BorderWidth = 1;
                    Pageinnertbl[j].Width = 560;
                    //this Id is need for the JavaScript to hide and show it in executive view
                    Pageinnertbl[j].ID = (string)("PageActivity_" + j);
                    rowtbl31[j] = new TableRow();
                    Pageinnertbl[j].Controls.Add(rowtbl31[j]);
                    celltbl311[j] = new TableCell();
                    celltbl312[j] = new TableCell();
                    celltbl313[j] = new TableCell();
                    celltbl314[j] = new TableCell();
                    celltbl311[j].BackColor = System.Drawing.Color.Black;
                    celltbl311[j].ForeColor = System.Drawing.Color.White;
                    celltbl312[j].BackColor = System.Drawing.Color.Black;
                    celltbl312[j].ForeColor = System.Drawing.Color.White;
                    celltbl313[j].BackColor = System.Drawing.Color.Black;
                    celltbl313[j].ForeColor = System.Drawing.Color.White;
                    celltbl314[j].BackColor = System.Drawing.Color.Black;
                    celltbl314[j].ForeColor = System.Drawing.Color.White;
                    lblPgHeader1[j] = new Label();
                    lblPgHeader2[j] = new Label();
                    lblPgHeader3[j] = new Label();
                    lblPgHeader4[j] = new Label();
                    lblPgHeader1[j].Text = "<b>" + _MessageHelper.GetMessage("lbl page name") + "</b>";
                    lblPgHeader2[j].Text = "<b>" + _MessageHelper.GetMessage("lbl updates") + "</b>";
                    lblPgHeader3[j].Text = "<b>" + _MessageHelper.GetMessage("lbl last updated") + "</b>";
                    lblPgHeader4[j].Text = "<b>" + _MessageHelper.GetMessage("lbl user name") + "</b>";
                    celltbl311[j].Controls.Add(lblPgHeader1[j]);
                    celltbl312[j].Controls.Add(lblPgHeader2[j]);
                    celltbl313[j].Controls.Add(lblPgHeader3[j]);
                    celltbl314[j].Controls.Add(lblPgHeader4[j]);
                    Pageinnertbl[j].Controls.Add(rowtbl31[j]);
                    rowtbl31[j].Controls.Add(celltbl311[j]);
                    rowtbl31[j].Controls.Add(celltbl312[j]);
                    rowtbl31[j].Controls.Add(celltbl313[j]);
                    rowtbl31[j].Controls.Add(celltbl314[j]);



                    //rowtbl3 = new TableRow[j - 1];
                    //celltbl31 = new TableCell[j - 1];
                    //celltbl32 = new TableCell[j - 1];
                    //celltbl33 = new TableCell[j - 1];
                    //celltbl34 = new TableCell[j - 1];
                    //lblPg1 = new Label[j - 1];
                    //lblPg2 = new Label[j - 1];
                    //lblPg3 = new Label[j - 1];
                    //lblPg4 = new Label[j - 1];
                    
                    //looping through the inner table for the page activity

                    for (i = 0; i < aTotalPages.Count ; i++)
                    {
                        
                        rowtbl3 = new TableRow[(int)aTotalPages[i]];
                        celltbl31 = new TableCell[(int)aTotalPages[i]];
                        celltbl32 = new TableCell[(int)aTotalPages[i]];
                        celltbl33 = new TableCell[(int)aTotalPages[i]];
                        celltbl34 = new TableCell[(int)aTotalPages[i]];
                        lblPg1 = new Label[(int)aTotalPages[i]];
                        lblPg2 = new Label[(int)aTotalPages[i]];
                        lblPg3 = new Label[(int)aTotalPages[i]];
                        lblPg4 = new Label[(int)aTotalPages[i]];
                        for (int k = 0; k < (int)aTotalPages[i]; k++)
                        {
                            try
                            {
                                string name1 = ((Collection)((ArrayList)aFolder[i])[k])["FolderName"].ToString();
                                string name2 = aPath[j - 1].ToString();

                                //if (((Collection)((ArrayList)aFolder[j - 1])[i])["FolderName"] != aPath[j - 1])
                                if (name1 != name2)
                                {
                                    break;
                                }
                            }
                            catch
                            {
                                break;
                            }

                            rowtbl3[k] = new TableRow();
                            Pageinnertbl[j].Controls.Add(rowtbl3[k]);
                            celltbl31[k] = new TableCell();
                            celltbl32[k] = new TableCell();
                            celltbl33[k] = new TableCell();
                            celltbl34[k] = new TableCell();

                            if (Convert.ToBoolean((Convert.ToInt32(k) % 2)))
                            {
                                celltbl31[k].BackColor = System.Drawing.Color.LavenderBlush;
                                celltbl32[k].BackColor = System.Drawing.Color.LavenderBlush;
                                celltbl33[k].BackColor = System.Drawing.Color.LavenderBlush;
                                celltbl34[k].BackColor = System.Drawing.Color.LavenderBlush;
                            }
                            else
                            {
                                celltbl31[k].BackColor = System.Drawing.Color.LightCyan;
                                celltbl32[k].BackColor = System.Drawing.Color.LightCyan;
                                celltbl33[k].BackColor = System.Drawing.Color.LightCyan;
                                celltbl34[k].BackColor = System.Drawing.Color.LightCyan;
                            }
                            lblPg1[k] = new Label();
                            lblPg2[k] = new Label();
                            lblPg3[k] = new Label();
                            lblPg4[k] = new Label();
                            //lblPg1(i).Text = aFolder.Item(j - 1).item(i)("PageName")  'data(i)("PageName")
                            //if (((Collection)((ArrayList)aFolder[j - 1])[i])["FolderName"] != aPath[j - 1])

                            lblPg1[k].Text = (((Collection)((ArrayList)aFolder[j - 1])[k])["PageName"]).ToString(); //data(i)("PageName")
                            lblPg2[k].Text = (((Collection)((ArrayList)aFolder[j - 1])[k])["Updates"]).ToString(); //data(i)("Updates")
                            lblPg3[k].Text = (string)("&#160;" + ((Collection)((ArrayList)aFolder[j - 1])[k])["LastUpdated"]).ToString(); //data(i)("LastUpdated")
                            lblPg4[k].Text = (string)("&#160;" + ((Collection)((ArrayList)aFolder[j - 1])[k])["UserName"]).ToString(); //data(i)("UserName")
                            celltbl31[k].Controls.Add(lblPg1[k]);
                            celltbl32[k].Controls.Add(lblPg2[k]);
                            celltbl33[k].Controls.Add(lblPg3[k]);
                            celltbl34[k].Controls.Add(lblPg4[k]);

                            rowtbl3[k].Controls.Add(celltbl31[k]);
                            rowtbl3[k].Controls.Add(celltbl32[k]);
                            rowtbl3[k].Controls.Add(celltbl33[k]);
                            rowtbl3[k].Controls.Add(celltbl34[k]);
                        }
                    }

                    cellPage[j].Controls.Add(Pageinnertbl[j]);
                    rowPage[j].Controls.Add(cellPage[j]);

                    celltbl211[j].Controls.Add(lblFolderHeader1[j]);
                    celltbl212[j].Controls.Add(lblFolderHeader2[j]);
                    celltbl213[j].Controls.Add(lblFolderHeader3[j]);
                    celltbl221[j].Controls.Add(lblTotalUpdated[j]);
                    celltbl222[j].Controls.Add(lblPagesUpdated[j]);
                    celltbl223[j].Controls.Add(lblTotalPages[j]);
                    folderinnertbl[j].Controls.Add(rowtbl12[j]);
                    rowtbl12[j].Controls.Add(celltbl211[j]);
                    rowtbl12[j].Controls.Add(celltbl212[j]);
                    rowtbl12[j].Controls.Add(celltbl213[j]);
                    folderinnertbl[j].Controls.Add(rowtbl22[j]);
                    rowtbl22[j].Controls.Add(celltbl221[j]);
                    rowtbl22[j].Controls.Add(celltbl222[j]);
                    rowtbl22[j].Controls.Add(celltbl223[j]);
                    cellfolder[j].Controls.Add(folderinnertbl[j]);
                    rowfolder[j].Controls.Add(cellfolder[j]);

                    cellPath[j].Controls.Add(lblPath[j]);
                    rowPath[j].Controls.Add(cellPath[j]);

                    cellMain.Controls.Add(tblrep[j]);
                    rowMain.Controls.Add(cellMain);

                }
            }
            //tables of reports added to the placeholder phUpdateActivity
            phUpdateActivity.Controls.Clear();
            phUpdateActivity.Controls.Add(main);
            phUpdateActivity.Visible = true;
            tr_phUpdateActivity.Visible = true;

            //read the tables of reports in HTML.
            //store it in the member variable for export task.
            //store in the hidden field for email task.
            System.Text.StringBuilder sb = new System.Text.StringBuilder();
            System.IO.StringWriter stringWrite = new System.IO.StringWriter(sb);
            HtmlTextWriter htmlWrite = new HtmlTextWriter(stringWrite);
            string siteRptHtml = "";
            main.RenderControl(htmlWrite);
            _ReportTableHtml = sb.ToString();
            siteRptHtml = "<input id=\"siteRptHtml\" type=\"hidden\" name=\"siteRptHtml\" value=\"" + Ektron.Cms.Common.EkFunctions.HtmlEncode(_ReportTableHtml) + "\"/>";
            siteRptHtml = siteRptHtml + "<script type=\"text/javascript\">setDisplayMode(\'" + _ReportDisplay + "\');</script>";
            SiteActivityHtml.Text = siteRptHtml;
        }
    }
    private bool DoesAssetSupertypeExist(AssetInfoData[] asset_data, int lContentType)
    {
        int i = 0;
        bool result = false;
        if (!(asset_data == null))
        {
            for (i = 0; i <= asset_data.Length - 1; i++)
            {
                if (Ektron.Cms.Common.EkConstants.ManagedAsset_Min <= asset_data[i].TypeId && asset_data[i].TypeId <= Ektron.Cms.Common.EkConstants.ManagedAsset_Max)
                {
                    if (asset_data[i].TypeId == lContentType)
                    {
                        result = true;
                        break;
                    }
                }
            }
        }
        return (result);
    }
    private bool IsReviewPost()
    {
        bool bret = false;
        if (Request.Form != null)
        {
            for (int i = 0; i <= (Request.Form.Count - 1); i++)
            {
                if (Request.Form.Keys[i].ToString().IndexOf("cr_a_") == 0)
                {
                    bret = true;
                    break;
                }
            }
        }
        return bret;
    }
    private ICollection CreateContentReviewSource(ContentReviewData[] ReviewList)
    {
        DataTable dt = new DataTable();
        DataRow dr;
        EkDTSelector dtS;
        dtS = _ContentApi.EkDTSelectorRef;

        dt.Columns.Add(new DataColumn("APPROVE", typeof(string)));
        dt.Columns.Add(new DataColumn("DECLINE", typeof(string)));
        dt.Columns.Add(new DataColumn("TITLE", typeof(string)));
        dt.Columns.Add(new DataColumn("USERNAME", typeof(string)));
        dt.Columns.Add(new DataColumn("DATETIME", typeof(string)));
        dt.Columns.Add(new DataColumn("RATING", typeof(string)));
        dt.Columns.Add(new DataColumn("USER_COMMENTS", typeof(string)));

        for (int i = 0; i <= (ReviewList.Length - 1); i++)
        {
            dr = dt.NewRow();
            dr[0] += "<input type=\"radio\" id=\"cr_a_" + ReviewList[i].ID.ToString() + "\" name=\"cr_a_" + ReviewList[i].ID.ToString() + "\" value=\"cr_app_" + ReviewList[i].ID.ToString() + "\" />";
            dr[1] += "<input type=\"radio\" id=\"cr_a_" + ReviewList[i].ID.ToString() + "\" name=\"cr_a_" + ReviewList[i].ID.ToString() + "\" value=\"cr_dec_" + ReviewList[i].ID.ToString() + "\" />";
            if (ReviewList[i].ContentType == 1)
            {
                dr[2] = "<img src=" + _ContentApi.AppPath + "images/ui/icons/contentHtml.png" + "></img>&nbsp;";
            }
            else if (ReviewList[i].ContentType == 2)
            {
                dr[2] = "<img src=" + _ContentApi.AppPath + "images/ui/icons/contentForm.png" + "></img>&nbsp;";
            }
            else
            {
                dr[2] = "<img src=" + ReviewList[i].AssetData.Icon + "></img>&nbsp;";
            }
            dr[2] += "<a href=\"addeditcontentreview.aspx?action=edit&id=" + ReviewList[i].ID.ToString() + "&cid=" + ReviewList[i].ContentID.ToString() + "\">" + ReviewList[i].ContentTitle + "</a>";
            if (ReviewList[i].UserID == 0)
            {
                dr[3] = "<font color=\"gray\">" + this._MessageHelper.GetMessage("lbl anon") + "</font>";
            }
            else
            {
                dr[3] = ReviewList[i].UserDisplayName;
            }
            dr[4] = ReviewList[i].RatingDate.ToShortDateString();
            dr[5] = GenerateStars(System.Convert.ToInt32(ReviewList[i].Rating));
            dr[6] = EkFunctions.HtmlEncode(Server.UrlDecode(ReviewList[i].UserComments));
            dt.Rows.Add(dr);
        }
        DataView dv = new DataView(dt);
        return dv;
    }
    public string GenerateStars(int irating)
    {
        StringBuilder sbRating = new StringBuilder();
        for (int i = 1; i <= 10; i++)
        {
            sbRating.Append("<img border=\"0\" src=\"").Append(this._ContentApi.AppPath + "images/ui/icons/");
            if (i % 2 > 0)
            {
                if (irating < i)
                {
                    sbRating.Append("starEmptyLeft.png");
                }
                else
                {
                    sbRating.Append("starLeft.png");
                }
            }
            else
            {
                if (irating < i)
                {
                    sbRating.Append("starEmptyRight.png");
                }
                else
                {
                    sbRating.Append("starRight.png");
                }
            }
            sbRating.Append("\"/>");
        }
        return sbRating.ToString();
    }
    private void Toolbar_SearchPhraseReport()
    {
        reportstoolbar reportToolBar;
        reportToolBar = (reportstoolbar)(LoadControl("controls/reports/reportstoolbar.ascx"));
        ToolBarHolder.Controls.Add(reportToolBar);
        reportToolBar.AppImgPath = _AppImgPath;
        reportToolBar.Data = null;
        reportToolBar.PageAction = "";
        reportToolBar.FilterType = "";
        reportToolBar.TitleBarMsg = _MessageHelper.GetMessage("lbl search phrase reports");
        reportToolBar.MultilingualEnabled = 1;
        reportToolBar.ContentLang = _ContentApi.ContentLanguage;
        reportToolBar.EnableContentTypes = false;
        reportToolBar.EnableEmail = true;
        reportToolBar.EnableFolders = false;
        reportToolBar.EnableDefaultTitlePrefix = false;
    }
    private void MakePhraseFilterControls(bool init)
    {
        string strFilterControls = "";
        string strValue;

        lblDays.Visible = true;
        top.Visible = true;
        strFilterControls = "<tr>";
        strFilterControls += "<td class=\"label\">";
        strFilterControls += (string)(_MessageHelper.GetMessage("lbl minimum count") + ":");
        strFilterControls += "</td>";
        strFilterControls += "<td class=\"value\">";
        strFilterControls += "<input type=\'text\' id=\'min_phrase_count\' name=\'min_phrase_count\' size=\'6\' ";
        if (!(Request.Form["min_phrase_count"] == null))
        {
            strValue = AntiXss.HtmlEncode(Request.Form["min_phrase_count"]);
            if (strValue.Length == 0)
            {
                strValue = "1";
            }
            strFilterControls += " value=\'" + strValue + "\' ";
        }
        else
        {
            if (init && !IsPostBack)
            {
                strFilterControls += " value=\'4\' ";
            }
        }
        strFilterControls += " >";
        strFilterControls += "</td>";
        strFilterControls += "</tr>";
        strFilterControls += "<tr>";
        strFilterControls += "<td class=\"label\">";
        strFilterControls += (string)(_MessageHelper.GetMessage("lbl include site") + ":");
        strFilterControls += "</td>";
        strFilterControls += "<td>";
        strFilterControls += "<input type=\'checkbox\' id=\'include_site_phrases\' name=\'include_site_phrases\' ";
        if (!(Request.Form["include_site_phrases"] == null))
        {
            if ("on" == Request.Form["include_site_phrases"])
            {
                strFilterControls += " checked=\'true\' ";
            }
        }
        else
        {
            if (init && !IsPostBack)
            {
                strFilterControls += " checked=\'true\' ";
            }
        }
        strFilterControls += " />";
        strFilterControls += "</td>";
        strFilterControls += "</tr>";
        strFilterControls += "<tr>";
        strFilterControls += "<td class=\"label\">";
        strFilterControls += (string)(_MessageHelper.GetMessage("lbl include workarea") + ":");
        strFilterControls += "</td>";
        strFilterControls += "<td>";
        strFilterControls += "<input type=\'checkbox\' id=\'include_workarea_phrases\' name=\'include_workarea_phrases\' ";
        if (!(Request.Form["include_workarea_phrases"] == null))
        {
            if ("on" == Request.Form["include_workarea_phrases"])
            {
                strFilterControls += " checked=\'true\' ";
            }
        }
        strFilterControls += " />";
        strFilterControls += "</td>";
        strFilterControls += "</tr>";
        lblDays.Text = strFilterControls;
    }
    private void SearchPhraseReport()
    {
        System.Text.StringBuilder result = new System.Text.StringBuilder();
        Ektron.Cms.Content.EkContent contObj;
        DataView dv;
        Collection cRet;
        Collection cOptions;
        System.Web.UI.WebControls.BoundColumn colBound = new System.Web.UI.WebControls.BoundColumn();
        DataRow dr;
        DataTable dt = new DataTable();

        contObj = _ContentApi.EkContentRef;
        top.Visible = true;
        Toolbar_SearchPhraseReport();

        if (Request.QueryString["btnSubmit"] == null)
        {
            editSchedule.Visible = true;
            lblDays.Visible = true;
            MakePhraseFilterControls(true);
            tr_startDate.Visible = true;
            tr_endDate.Visible = true;
            lblStartDate.Text = _MessageHelper.GetMessage("generic start date label");
            lblEndDate.Text = _MessageHelper.GetMessage("generic end date label");
            EditScheduleHtml.Text = "";
            DisplayDateFields();

            result.Append("</td><td><input id=\"fId\" type=\"hidden\" name=\"fId\" value=\"0\" />");
            result.Append("<input id=\"rptType\" type=\"hidden\" name=\"rptType\" />");
            result.Append("<input id=\"rptFolder\" type=\"hidden\" name=\"rptFolder\" />");
            result.Append("<input id=\"rptLink\" type=\"hidden\" name=\"rptLink\" />");
            result.Append("<input id=\"ContType\" type=\"hidden\" name=\"ContType\" />");
            result.Append("<input id=\"LangType\" type=\"hidden\" name=\"LangType\" />");
            result.Append("<ul class=\'buttonWrapper buttonWrapperLeft\'><li><a class=\"button buttonInline greenHover buttonGetResult\" id=\"btnResult\" onclick=\"ReportSiteUpdateActivity()\" value=\"" + _MessageHelper.GetMessage("btn get result") + "\">" + _MessageHelper.GetMessage("btn get result") + "</a></li></ul></td>");
            EditScheduleHtml.Text = EditScheduleHtml.Text + result.ToString();
        }
        else
        {
            // run & show report
            if (!(Request.Form["start_date"] == null))
            {
                _StartDate = Request.Form["start_date"];
            }
            if (!(Request.Form["end_date"] == null))
            {
                _EndDate = Request.Form["end_date"];
                if (!Information.IsDate(_EndDate))
                {
                    _EndDate = "";
                }
            }

            MakePhraseFilterControls(false);
            DisplayDateFields();

            // run report:
            cOptions = new Collection();
            cOptions.Add(_StartDate, "StartDate", null, null);
            cOptions.Add(_EndDate, "EndDate", null, null);
            if (!(Request.Form["include_site_phrases"] == null))
            {
                cOptions.Add(Request.Form["include_site_phrases"], "IncludeSitePhrases", null, null);
            }
            else
            {
                cOptions.Add("off", "IncludeSitePhrases", null, null);
            }
            if (!(Request.Form["include_workarea_phrases"] == null))
            {
                cOptions.Add(Request.Form["include_workarea_phrases"], "IncludeWorkareaPhrases", null, null);
            }
            else
            {
                cOptions.Add("off", "IncludeWorkareaPhrases", null, null);
            }
            if (!(Request.Form["selLang"] == null))
            {
                cOptions.Add(Request.Form["selLang"], "LanguageID", null, null);
            }
            if (!(Request.Form["min_phrase_count"] == null))
            {
                cOptions.Add(Request.Form["min_phrase_count"], "MinimumPhraseCount", null, null);
            }
            cRet = contObj.GetSearchPhraseReport(ref cOptions);

            dgReport.BorderColor = System.Drawing.Color.White;

            colBound = new System.Web.UI.WebControls.BoundColumn();
            colBound.DataField = "phrase";
            colBound.HeaderText = _MessageHelper.GetMessage("lbl phrase");
            colBound.ItemStyle.Wrap = false;
            colBound.ItemStyle.VerticalAlign = VerticalAlign.Top;
            colBound.ItemStyle.HorizontalAlign = HorizontalAlign.Left;
            colBound.HeaderStyle.CssClass = "title-header";
            dgReport.Columns.Add(colBound);

            colBound = new System.Web.UI.WebControls.BoundColumn();
            colBound.DataField = "use_count";
            colBound.HeaderText = _MessageHelper.GetMessage("lbl use count");
            colBound.ItemStyle.VerticalAlign = VerticalAlign.Top;
            colBound.HeaderStyle.CssClass = "title-header";
            colBound.ItemStyle.Wrap = false;
            dgReport.Columns.Add(colBound);

            dt = new DataTable();

            dt.Columns.Add(new DataColumn("phrase", typeof(string)));
            dt.Columns.Add(new DataColumn("use_count", typeof(int)));

            foreach (Collection cVals in cRet)
            {
                dr = dt.NewRow();
                dr[0] = cVals["KeyWord"];
                dr[1] = cVals["KeywordCount"];
                dt.Rows.Add(dr);
            }
            dv = new DataView(dt);
            dgReport.DataSource = dv;
            dgReport.DataBind();

        }

        IncludeSearchPhraseReportJS();

        result = null;
        contObj = null;
        dv = null;
        dt = null;
        colBound = null;
        cOptions = null;
    }
    private void IncludeSearchPhraseReportJS()
    {
        string strJS;

        strJS = "<script type=\"text/javascript\">" + "\r\n";
        strJS += "function ReportSiteUpdateActivity() {" + "\r\n";

        strJS += " if (!EkDTCompareDates(document.forms[0].start_date, document.forms[0].end_date)) {" + "\r\n";
        strJS += "   alert (\"You can not have start date later than end date\");" + "\r\n";
        strJS += "   return false; }" + "\r\n";

        strJS += "   var selObj = document.forms[0].selDisplay;" + "\r\n";
        strJS += "   document.forms[0].action = \"reports.aspx?action=viewsearchphrasereport&btnSubmit=1&filtertype=&filterid=\";";
        strJS += " document.forms[0].submit();" + "\r\n" + "\r\n";
        strJS += "}" + "\r\n";

        strJS += "function setCheckbox(chk) {" + "\r\n";
        strJS += "if (chk.checked) {" + "\r\n";
        strJS += "document.selections.subfolder.value = 1; } else { document.selections.subfolder.value = 0; }" + "\r\n";
        strJS += "}" + "\r\n";

        strJS += "</script>" + "\r\n";
        Page.ClientScript.RegisterClientScriptBlock(this.GetType(), "reportsiteupdactivity", strJS);
    }
    private void IncludeSiteUpdateActivityJS()
    {
        string strJS;
        strJS = "<script type=\"text/javascript\">" + "\r\n";
        strJS += "function ReportSiteUpdateActivity() {" + "\r\n";
        strJS += "var stDate;" + "\r\n";
        strJS += " if ((document.forms[0].start_date.value != undefined) && (document.forms[0].start_date.value != \"\")) {" + "\r\n";
        strJS += "   stDate = ConvertEkDateToDate(document.forms[0].start_date);" + "\r\n";
        strJS += "   var dtToday = new Date();" + "\r\n";
        strJS += "   dtToday.setHours(23);" + "\r\n";
        strJS += "   dtToday.setMinutes(59);" + "\r\n";
        strJS += "   dtToday.setSeconds(59);" + "\r\n";
        strJS += "   dtToday.setMilliseconds(999);" + "\r\n";
        strJS += "   if (stDate >= dtToday) { " + "\r\n";
        strJS += "     alert (\"start date cannot be a future date\");" + "\r\n";
        strJS += "     return false; }" + "\r\n";
        strJS += " } " + "\r\n";

        strJS += " if (!EkDTCompareDates(document.forms[0].start_date, document.forms[0].end_date)) {" + "\r\n";
        strJS += "   if (document.forms[0].start_date.value == document.forms[0].end_date.value)" + "\r\n";
        strJS += "   { " + "\r\n";
        strJS += "     alert (\"Start date equals end date, nothing to report\");" + "\r\n";
        strJS += "   } " + "\r\n";
        strJS += "   else " + "\r\n";
        strJS += "   { " + "\r\n";
        strJS += "     alert (\"You can not have start date later than end date\");" + "\r\n";
        strJS += "   } " + "\r\n";
        strJS += "   return false; }" + "\r\n";

        strJS += " if (document.getElementById(\"rptFolder\").value == \"\") { document.getElementById(\"rptFolder\").value = document.getElementById(\"hselFolder\").innerHTML; } " + "\r\n";
        // If no content type selected get the value from the drop down
        strJS += " if (document.getElementById(\"ContType\").value == \"\") { var objSelSupertype = document.getElementById(\'selAssetSupertype\'); 	if (objSelSupertype != null) { document.getElementById(\"ContType\").value = objSelSupertype.value; } }" + "\r\n";
        strJS += " if (document.getElementById(\"LangType\").value == \"\") { var objLangtype = document.getElementById(\'selLang\'); 	if (objLangtype != null) { document.getElementById(\"LangType\").value = objLangtype.value; } }" + "\r\n";

        strJS += "   var selObj = document.forms[0].selDisplay;" + "\r\n";
        strJS += "   document.forms[0].action = \"reports.aspx?action=siteupdateactivity&btnSubmit=1&filtertype=path&filterid=\" + document.forms[0].fId.value + \"&start_date=\'\" + document.getElementById(\"start_date\").value + \"\'&end_date=\'\" + document.getElementById(\"end_date\").value +  \"\'&report_display=\" + selObj[selObj.selectedIndex].value + \"&ContType=\" + document.forms[0].ContType.value + \"&LangType=\" + document.forms[0].LangType.value + \"&ex_users=\" + document.getElementById(\"excludeUserIds\").value + \"&ex_groups=\" + document.getElementById(\"excludeUserGroups\").value + \"&rootfolder=\" + document.getElementById(\"rootFolder\").value + \"&subfldInclude=\" + document.getElementById(\"subfldInclude\").value + \"\"; " + "\r\n";
        //strJS &= "   document.forms[0].action = ""reports.aspx?action=siteupdateactivity&btnSubmit=1&filtertype=path&filterid="" + document.forms[0].fId.value + ""&rptFolder="" + document.getElementById(""rptFolder"").value + ""&start_date='"" + document.getElementById(""start_date"").value + ""'&end_date='"" + document.getElementById(""end_date"").value +  ""'&report_display="" + selObj[selObj.selectedIndex].value + ""&ContType="" + document.forms[0].ContType.value + ""&LangType="" + document.forms[0].LangType.value + ""&ex_users="" + document.getElementById(""excludeUserIds"").value + """"; " & vbCrLf
        //strJS &= " if (document.getElementById(""fId"").value != ""0"") {document.forms[0].action = document.forms[0].action + ""filtertype=path&filterid="" + document.forms[0].fId.value + "";}" & vbCrLf
        strJS += " document.forms[0].submit();" + "\r\n" + "\r\n";
        strJS += "}" + "\r\n";

        strJS += "</script>" + "\r\n";
        Page.ClientScript.RegisterClientScriptBlock(this.GetType(), "reportsiteupdactivity", strJS);
    }
    private void EditScheduleHtmlScripts()
    {
        System.Text.StringBuilder result = new System.Text.StringBuilder();
        EkDTSelector dateSchedule;
        dateSchedule = _ContentApi.EkDTSelectorRef;

        result.Append("<div  style=\"display:none;position: absolute;top:" + "px;Width:95%;Height:95%\" id=\"_dvSchedule\">");
        result.Append("<table width=\"100%\" border=\"0\" cellpadding=\"2\" cellspacing=\"2\">");
        result.Append("<tr valign=\"top\">");
        result.Append("<td>");
        result.Append("<table width=\"100%\" border=\"0\" cellpadding=\"2\" cellspacing=\"2\">");
        result.Append("<tr>");
        result.Append("<scripttype=\"text/javascript\">");
        result.Append("function OpenCalendar(bStartDate) {");
        result.Append("if (true == bStartDate) {");
        result.Append("document.forms[0].start_date.value = Trim(document.forms[0].start_date.value);CallCalendar(document.forms[0].start_date.value, \'calendar.aspx\', \'start_date\', \'selections\');");
        result.Append("} else if (false == bStartDate) {");
        result.Append("document.forms[0].end_date.value = Trim(document.forms[0].end_date.value);CallCalendar(document.forms[0].end_date.value, \'calendar.aspx\', \'end_date\', \'selections\');");
        result.Append("}");
        result.Append("}");
        result.Append("</script>");
        result.Append("<td width=\"5%\" nowrap=\"true\">" + _MessageHelper.GetMessage("generic start date label") + "</td>");
        result.Append("<td width=\"95%\" nowrap=\"true\">");
        dateSchedule.formName = "selections";
        dateSchedule.extendedMeta = true;
        dateSchedule.formElement = "start_date";
        dateSchedule.spanId = "start_date_span";
        if (_StartDate != "")
        {
            dateSchedule.targetDate = DateTime.Parse(_StartDate);
        }
        result.Append(dateSchedule.displayCultureDateTime(true, "", ""));
        result.Append("</td>");
        result.Append("</tr>");
        result.Append("<tr>");
        result.Append("<td width=\"5%\" nowrap=\"true\">" + _MessageHelper.GetMessage("generic end date label") + "</td>");
        result.Append("<td width=\"95%\" nowrap=\"true\">");
        dateSchedule.formName = "selections";
        dateSchedule.extendedMeta = true;
        dateSchedule.formElement = "end_date";
        dateSchedule.spanId = "end_date_span";
        if (_EndDate != "")
        {
            dateSchedule.targetDate = DateTime.Parse(_EndDate);
        }
        else
        {
            dateSchedule.targetDate = Convert.ToDateTime(null);
        }
        result.Append(dateSchedule.displayCultureDateTime(true, "", ""));
        result.Append("</td>");

        result.Append("</tr>");
        result.Append("</table>");
        result.Append("</div>");
        EditScheduleHtml.Text = EditScheduleHtml.Text + result.ToString();
        result = null;
    }
    private void Display_Preapproval()
    {
        reportstoolbar m_reportsToolBar;
        System.Web.UI.WebControls.BoundColumn colBound = new System.Web.UI.WebControls.BoundColumn();
        DataTable dt = new DataTable();
        DataRow dr;
        SiteAPI m_SiteAPI = new SiteAPI();
        Ektron.Cms.Content.EkContent contObj;
        Collection cTmp = new Collection();
        Collection cReports = new Collection();

        System.Text.StringBuilder result = new System.Text.StringBuilder();
        try
        {
            m_reportsToolBar = (reportstoolbar)(LoadControl("controls/reports/reportstoolbar.ascx"));
            ToolBarHolder.Controls.Add(m_reportsToolBar);
            m_reportsToolBar.AppImgPath = _AppImgPath;

            _TitleBarMsg = _MessageHelper.GetMessage("lbl preapproval groups report");

            m_reportsToolBar.PageAction = _PageAction;
            m_reportsToolBar.FilterType = _FilterType;
            m_reportsToolBar.TitleBarMsg = _TitleBarMsg;
            m_reportsToolBar.MultilingualEnabled = _EnableMultilingual;
            m_reportsToolBar.ContentLang = _ContentApi.ContentLanguage;

            txtInterval.Visible = false;
            selInterval.Visible = false;

            if (_OrderBy.Trim() != "")
            {
                cTmp.Add(_OrderBy.Trim(), "OrderBy", null, null);
            }
            else
            {
                cTmp.Add("", "OrderBy", null, null);
            }

            contObj = _ContentApi.EkContentRef;
            cReports = contObj.GetPreapprovalReport(cTmp);

            if (cReports != null && cReports.Count > 0)
            {
                editSchedule.Visible = false;
                dgReport.Visible = true;

                colBound = new System.Web.UI.WebControls.BoundColumn();
                colBound.DataField = "Folder";
                colBound.HeaderText = _MessageHelper.GetMessage("generic folder");
                colBound.ItemStyle.Wrap = false;
                colBound.ItemStyle.VerticalAlign = VerticalAlign.Top;
                colBound.ItemStyle.HorizontalAlign = HorizontalAlign.Left;
                colBound.HeaderStyle.CssClass = "title-header";

                dgReport.Columns.Add(colBound);

                colBound = new System.Web.UI.WebControls.BoundColumn();
                colBound.DataField = "FolderID";
                colBound.HeaderText = _MessageHelper.GetMessage("generic folder id");
                colBound.ItemStyle.Wrap = false;
                colBound.ItemStyle.VerticalAlign = VerticalAlign.Top;
                colBound.ItemStyle.HorizontalAlign = HorizontalAlign.Left;
                colBound.HeaderStyle.CssClass = "title-header";

                dgReport.Columns.Add(colBound);

                colBound = new System.Web.UI.WebControls.BoundColumn();
                colBound.DataField = "PreapprovalGroup";
                colBound.HeaderText = _MessageHelper.GetMessage("lbl preapproval group");
                colBound.ItemStyle.Wrap = false;
                colBound.ItemStyle.VerticalAlign = VerticalAlign.Top;
                colBound.ItemStyle.HorizontalAlign = HorizontalAlign.Left;
                colBound.HeaderStyle.CssClass = "title-header";

                dgReport.Columns.Add(colBound);

                colBound = new System.Web.UI.WebControls.BoundColumn();
                colBound.DataField = "GroupID";
                colBound.HeaderText = _MessageHelper.GetMessage("lbl group id");
                colBound.ItemStyle.Wrap = false;
                colBound.ItemStyle.VerticalAlign = VerticalAlign.Top;
                colBound.ItemStyle.HorizontalAlign = HorizontalAlign.Left;
                colBound.HeaderStyle.CssClass = "title-header";

                dgReport.Columns.Add(colBound);

                dgReport.BorderColor = System.Drawing.Color.White;
                dgReport.Visible = true;

                dt.Columns.Add(new DataColumn("Folder", typeof(string)));
                dt.Columns.Add(new DataColumn("FolderID", typeof(long)));
                dt.Columns.Add(new DataColumn("PreapprovalGroup", typeof(string)));
                dt.Columns.Add(new DataColumn("GroupID", typeof(long)));

                foreach (Collection cRep in cReports)
                {
                    dr = dt.NewRow();
                    dr[0] = "<a href=\"content.aspx?action=ViewContentByCategory&id=" + (cRep["FolderID"].ToString()) + "&callerpage=" + (EkFunctions.UrlEncode("reports.aspx")) + "&origurl=" + (EkFunctions.UrlEncode((string)("action=" + _PageAction + "&orderby=" + _OrderBy))) + "\" title=\'" + _MessageHelper.GetMessage("generic view") + " \"" + Strings.Replace(cRep["FolderName"].ToString(), "\'", "`", 1, -1, 0) + "\"" + "\'>" + (cRep["FolderName"].ToString()) + "</a>";
                    dr[1] = cRep["FolderID"];
                    if (0 == Convert.ToInt32(cRep["PreApprovalGroupID"]))
                    {
                        dr[2] = _MessageHelper.GetMessage("none w prths");
                    }
                    else
                    {
                        dr[2] = "<a href=\"users.aspx?action=viewallusers&grouptype=0&LangType=" + _ContentLanguage + "&groupid=" + (cRep["PreApprovalGroupID"].ToString()) + "&id=" + (cRep["PreApprovalGroupID"].ToString()) + "&callerpage=" + (EkFunctions.UrlEncode("reports.aspx?")) + (EkFunctions.UrlEncode((string)("action=" + _PageAction + "&orderby=" + _OrderBy))) + "\" title=\'" + _MessageHelper.GetMessage("generic view") + " \"" + Strings.Replace(cRep["PreApprovalGroup"].ToString(), "\'", "`", 1, -1, 0) + "\"" + "\'>" + (cRep["PreApprovalGroup"].ToString()) + "</a>";
                        //dr(2) = "<a href=""users.aspx?action=viewallusers&grouptype=0&LangType=" & ContentLanguage & "&groupid=" & (cRep("PreApprovalGroupID").ToString) & "&id=" & (cRep("PreApprovalGroupID").ToString) & "&callerpage=" & (EkFunctions.UrlEncode("reports.aspx&origurl=" & (EkFunctions.UrlEncode("action=" & m_strPageAction & "&orderby=" & m_strOrderBy)) & """ title='" & m_refMsg.GetMessage("generic View") & " """ & Replace(cRep("PreApprovalGroup"), "'", "`") & """" & "'>" & (cRep("PreApprovalGroup").ToString) & "</a>"
                    }
                    dr[3] = cRep["PreApprovalGroupID"];
                    dt.Rows.Add(dr);
                }
            }
            else
            {
                //Currently there is no data to report. Show such message
                if (EditScheduleHtml.Text.IndexOf("Currently there is no data") == -1)
                {
                    result.Append("<table>");
                    result.Append("<tr><td>").Append(_MessageHelper.GetMessage("msg no data report")).Append("</td></tr>");
                    result.Append("</table>");
                    EditScheduleHtml.Text = EditScheduleHtml.Text + result.ToString();
                }

                editSchedule.Visible = true;
                dgReport.Visible = false;
            }

            DataView dv = new DataView(dt);
            dgReport.DataSource = dv;
            dgReport.DataBind();
        }
        catch (Exception ex)
        {
            Utilities.ShowError(ex.Message);
        }
        finally
        {
            contObj = null;
        }
    }
    private string MakeLink(ContentData _data)
    {
        string _link = string.Empty;
        if (_EmailHelper.IsLoggedInUsersEmailValid())
        {
            string strNotesTitle = _data.Title;
            string strNotesName = "Content";
            _link = "<a href=\"#\"" + "onclick=\"LoadEmailChildPage('userid=" + _data.UserId + _EmailHelper.MakeNotes_Email(strNotesName, strNotesTitle) + "&contentid=" + _data.Id + "&emailclangid=" + _data.LanguageId + "')\"" + " title='" + _MessageHelper.GetMessage("alt send email to") + " \"" + _data.EditorLastName + ", " + _data.EditorFirstName + "\"" + "'>" + _data.EditorLastName + ", " + _data.EditorFirstName + _EmailHelper.MakeEmailGraphic() + "</a>";
        }
        return _link;
    }

    #endregion

    #region PROCESS SUBMIT
    private void Process_Export()
    {
        Response.AddHeader("content-disposition", "attachment; filename=Site_Report_Export.xls");
        //The following lines of code are extracted from dotnetjohn.com on "Export DataSets to Excel"
        //http://www.dotnetjohn.com/articles.aspx?articleid=36
        //first let's clean up the response.object
        Response.Clear();
        Response.Charset = "";
        //set the response mime type for excel
        Response.ContentType = "application/vnd.ms-excel";
        //create a string writer
        System.IO.StringWriter stringWrite = new System.IO.StringWriter();
        //create an htmltextwriter which uses the stringwriter
        System.Web.UI.HtmlTextWriter htmlWrite = new System.Web.UI.HtmlTextWriter(stringWrite);
        //all that's left is to output the html
        // might need to strip out links/etc. first:
        string cleanedHtml = _ReportTableHtml;
        if (_PageAction == "siteupdateactivity")
        {
            cleanedHtml = CleanHtml(cleanedHtml);
        }
        Response.Write(cleanedHtml);
        Response.End();
    }

    private string CleanHtml(string sourceHtml)
    {
        string returnValue;
        string retString = sourceHtml;
        string openTag;
        string closeTag;

        // Remove anchor tags:
        openTag = "<a ";
        closeTag = "</a>";
        retString = RemoveHtmlTag(sourceHtml, openTag, closeTag);

        // Done:
        returnValue = retString;
        return returnValue;
    }

    public string RemoveHtmlTag(string sourceHtml, string openTag, string CloseTag)
    {
        string returnValue;
        string retString = sourceHtml;
        int indexStart;
        int indexEnd;

        // Remove all occurances of the opening tag:
        indexStart = retString.IndexOf(openTag);
        while (-1 < indexStart)
        {
            //If closeTag.Length Then
            //	indexEnd = retString.IndexOf(closeTag, indexStart)
            //	' remove everything between the tags - inclusive:
            //	retString = retString.Substring(0, indexStart + 1) & retString.Substring(indexEnd + 1 + closeTag.Length)
            //Else
            //	indexEnd = -1
            //	' poorly formed HTML (no closing tag?):
            indexEnd = retString.IndexOf(">", indexStart);
            if (-1 < indexEnd)
            {
                retString = (string)(retString.Substring(0, indexStart) + retString.Substring(indexEnd + 1));
            }
            //End If

            indexStart = retString.IndexOf(openTag);
        }

        // remove all occarances of the closing tag:
        retString = retString.Replace(CloseTag, "");

        returnValue = retString;
        return returnValue;
    }

    private void Process_CheckInAll()
    {
        Ektron.Cms.Content.EkContent contObj;
        if (Request.QueryString["filtertype"] != "")
        {
            _FilterType = EkFunctions.HtmlEncode(Request.QueryString["filtertype"]);
        }
        else
        {
            _FilterType = "";
        }
        if (Request.QueryString["filterid"] != "")
        {
            _FilterId = long.Parse(Request.QueryString["filterid"]);
        }
        else
        {
            _FilterId = 0;
        }

        if (Request.QueryString["PreviousAction"] != "")
        {
            _PreviousAction = Request.QueryString["PreviousAction"];
        }
        else
        {
            _PreviousAction = "";
        }

        Collection cTmp = new Collection();
        cTmp.Add(_FilterType, "FilterType", null, null);
        cTmp.Add(_FilterId, "FilterID", null, null);
        cTmp.Add(_OrderBy, "OrderBy", null, null);
        string[] idArray = Strings.Split(Request.Form["frm_content_ids"], ",", -1, 0);
        string[] langArray = Strings.Split(Request.Form["frm_language_ids"], ",", -1, 0);
        cTmp.Add("checkedout", "StateWanted", null, null);
        contObj = _ContentApi.EkContentRef;
        int lLoop = 0;
        bool retvalue = false;
        for (lLoop = 0; lLoop <= (idArray.Length - 1); lLoop++)
        {
            _ContentApi.ContentLanguage = System.Convert.ToInt32(langArray[lLoop]);
            retvalue = contObj.CheckIn(long.Parse(idArray[lLoop]), "");
        }
        Response.Redirect((string)("reports.aspx?action=" + _PreviousAction + "&orderby=" + _OrderBy + "&filtertype=" + _FilterType + "&filterid=" + _FilterId), false);
    }
    private void Process_SubmitAll()
    {
        Ektron.Cms.Content.EkContent contObj;
        if (Request.QueryString["filtertype"] != "")
        {
            _FilterType = EkFunctions.HtmlEncode(Request.QueryString["filtertype"]);
        }
        else
        {
            _FilterType = "";
        }
        if (Request.QueryString["filterid"] != "")
        {
            _FilterId = long.Parse(Request.QueryString["filterid"]);
        }
        else
        {
            _FilterId = 0;
        }

        if (Request.QueryString["PreviousAction"] != "")
        {
            _PreviousAction = Request.QueryString["PreviousAction"];
        }
        else
        {
            _PreviousAction = "";
        }

        Collection cTmp = new Collection();
        cTmp.Add(_FilterType, "FilterType", null, null);
        cTmp.Add(_FilterId, "FilterID", null, null);
        cTmp.Add(_OrderBy, "OrderBy", null, null);
        string[] idArray = Strings.Split(Request.Form["frm_content_ids"], ",", -1, 0);
        string[] langArray = Strings.Split(Request.Form["frm_language_ids"], ",", -1, 0);
        string[] folderArray = Strings.Split(Request.Form["frm_folder_ids"], ",", -1, 0);
        cTmp.Add("checkedin", "StateWanted", null, null);
        contObj = _ContentApi.EkContentRef;
        int lLoop = 0;
        bool retvalue = false;
        for (lLoop = 0; lLoop <= (idArray.Length - 1); lLoop++)
        {
            _ContentApi.ContentLanguage = System.Convert.ToInt32(langArray[lLoop]);
            retvalue = contObj.SubmitForPublicationv2_0(long.Parse(idArray[lLoop]), long.Parse(folderArray[lLoop]), "");
        }
        Response.Redirect((string)("reports.aspx?action=" + _PreviousAction + "&orderby=" + _OrderBy + "&filtertype=" + _FilterType + "&filterid=" + _FilterId), false);
    }
    #endregion

    #region JS/CSS

    private void RegisterJs()
    {

        Ektron.Cms.API.JS.RegisterJS(this, Ektron.Cms.API.JS.ManagedScript.EktronJS);
        Ektron.Cms.API.JS.RegisterJS(this, Ektron.Cms.API.JS.ManagedScript.EktronJFunctJS);
        Ektron.Cms.API.JS.RegisterJS(this, Ektron.Cms.API.JS.ManagedScript.EktronEmpJSFuncJS);
        Ektron.Cms.API.JS.RegisterJS(this, Ektron.Cms.API.JS.ManagedScript.EktronToolBarRollJS);
        Ektron.Cms.API.JS.RegisterJS(this, _ContentApi.ApplicationPath + "java/internCalendarDisplayFuncs.js", "EktronInternalCalendarDisplayJs");
        Ektron.Cms.API.JS.RegisterJS(this, _ContentApi.ApplicationPath + "java/validation.js", "EktronValidationJs");
        Ektron.Cms.API.JS.RegisterJS(this, Ektron.Cms.API.JS.ManagedScript.EktronWorkareaJS);

    }

    private void RegisterCss()
    {

        Ektron.Cms.API.Css.RegisterCss(this, Ektron.Cms.API.Css.ManagedStyleSheet.EktronWorkareaCss);
        Ektron.Cms.API.Css.RegisterCss(this, Ektron.Cms.API.Css.ManagedStyleSheet.EktronWorkareaIeCss, Ektron.Cms.API.Css.BrowserTarget.LessThanEqualToIE7);
        Ektron.Cms.API.Css.RegisterCss(this, Ektron.Cms.API.Css.ManagedStyleSheet.EktronFixedPositionToolbarCss);

    }

    #endregion
}