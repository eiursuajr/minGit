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

public partial class Workarea_urlRegExAliasMaint : System.Web.UI.Page
{
    private CommonApi _refCommonApi = new CommonApi();
    private UrlAliasRegExApi _aliasRegexAPI = new Ektron.Cms.UrlAliasing.UrlAliasRegExApi();
    private System.Collections.Generic.List<EkEnumeration.RegExPriority> priorityList;
    protected StyleHelper _refStyle = new StyleHelper();
    protected EkMessageHelper msgHelper;
    protected ContentAPI _refContentApi = new ContentAPI();
    protected void Page_Load(object sender, System.EventArgs e)
    {
        msgHelper = _refCommonApi.EkMsgRef;
        string pageAction = "";
        long siteID = 0;
        Ektron.Cms.Content.EkContent objContentRef;
        objContentRef = _refContentApi.EkContentRef;
        RegisterResources();
        SetJSServerVariables();
        //Licensing For 7.6
        if (!Ektron.Cms.DataIO.LicenseManager.LicenseManager.IsFeatureEnable(_refContentApi.RequestInformationRef, Ektron.Cms.DataIO.LicenseManager.Feature.UrlAliasing, false))
        {
            Utilities.ShowError(_refContentApi.EkMsgRef.GetMessage("feature locked error"));
            return;
        }
        if (!(_refCommonApi.IsAdmin() || objContentRef.IsARoleMember((long)Ektron.Cms.Common.EkEnumeration.CmsRoleIds.UrlAliasingAdmin, _refCommonApi.RequestInformationRef.UserId, false)))
        {
            Utilities.ShowError(_refContentApi.EkMsgRef.GetMessage("User not authorized"));
            return;
        }
        //Labels got from resource file
        lblExpression.Text = msgHelper.GetMessage("lbl alias expression");
        lblExpression.ToolTip = lblExpression.Text;
        lblExpressionMap.Text = msgHelper.GetMessage("lbl alias expressionmap");
        lblExpressionMap.ToolTip = lblExpressionMap.Text;
        lblRequestedUrl.Text = msgHelper.GetMessage("lbl req URL");
        lblRequestedUrl.ToolTip = lblRequestedUrl.Text;
        lblTransformedUrl.Text = msgHelper.GetMessage("lbl trans URL");
        lblTransformedUrl.ToolTip = lblTransformedUrl.Text;
        lblSort.Text = msgHelper.GetMessage("lbl priority");
        lblSort.ToolTip = lblSort.Text;
        lblActive.Text = msgHelper.GetMessage("active label");
        lblActive.ToolTip = lblActive.Text;
        lblExpressionName.Text = msgHelper.GetMessage("lbl alias expression") + " " + msgHelper.GetMessage("lbl name");
        lblExpressionName.ToolTip = lblExpressionName.Text;
        lblExampleURL.Text = msgHelper.GetMessage("lbl example url");
        lblExampleURL.ToolTip = lblExampleURL.Text;
        closeDialogLink.Text = msgHelper.GetMessage("close title");
        closeDialogLink.ToolTip = closeDialogLink.Text;
        lblExpressionLib.Text = msgHelper.GetMessage("lbl select exp");
        closeDialogLink.NavigateUrl = "#" + System.Text.RegularExpressions.Regex.Replace(msgHelper.GetMessage("close this dialog"), "\\s+", "");

        if (!String.IsNullOrEmpty(Request.QueryString["action"]))
        {
            pageAction = Request.QueryString["action"];
        }
        if (!String.IsNullOrEmpty(Request.QueryString["fId"]))
        {
            siteID = Convert.ToInt64(Request.QueryString["fId"]);
        }
        BuildRegexLib();

        if (pageAction == "addregex")
        {
            DisplayAdd(siteID.ToString());
        }
        else if (pageAction == "view")
        {
            DisplayView();
        }
        else if (pageAction == "editalias")
        {
            DisplayEdit();
        }
    }
    private void BuildRegexLib()
    {

        System.Collections.Generic.List<RegExSampleData> RegExSampleList;
        Ektron.Cms.UrlAliasing.UrlAliasRegExApi regexSampleListApi = new Ektron.Cms.UrlAliasing.UrlAliasRegExApi();
        string escapedExp;

        RegExSampleList = regexSampleListApi.GetRegExSampleList();
        if ((RegExSampleList != null) && RegExSampleList.Count > 0)
        {
            regExPicker.Columns.Add(_refStyle.CreateBoundField("EXPRESSION", "Expression", "title-header", HorizontalAlign.Left, HorizontalAlign.NotSet, Unit.Percentage(25), Unit.Percentage(25), false, false));
            regExPicker.Columns.Add(_refStyle.CreateBoundField("EXPRESSION MAP", "Expression Map", "title-header", HorizontalAlign.Left, HorizontalAlign.NotSet, Unit.Percentage(30), Unit.Percentage(30), false, false));
            regExPicker.Columns.Add(_refStyle.CreateBoundField("TRANSFORMED URL", "Transformed URL", "title-header", HorizontalAlign.Left, HorizontalAlign.NotSet, Unit.Percentage(25), Unit.Percentage(25), false, false));

            DataTable dt = new DataTable();
            DataRow dr;

            dt.Columns.Add(new DataColumn("EXPRESSION", typeof(string)));
            dt.Columns.Add(new DataColumn("EXPRESSION MAP", typeof(string)));
            dt.Columns.Add(new DataColumn("TRANSFORMED URL", typeof(string)));

            for (int i = 0; i <= RegExSampleList.Count - 1; i++)
            {
                dr = dt.NewRow();
                escapedExp = (string)(RegExSampleList[i].Expression.Replace("\\", "\\\\"));
                dr["EXPRESSION"] = "<a href=\"#\" onclick=\"selectExpMap(\'" + RegExSampleList[i].ExpressionMap + "\',\'" + escapedExp + "\',\'" + RegExSampleList[i].TransformedUrl + "\');return false;\">" + RegExSampleList[i].Expression + "</a>"; //"<a href=""urlmanualaliasmaint.aspx?action=view&id=" & maliaslist(i).AliasId & """>" & maliaslist(i).DisplayAlias & "</a>"
                dr["EXPRESSION MAP"] = "<a href=\"#\" onclick=\"selectExpMap(\'" + RegExSampleList[i].ExpressionMap + "\',\'" + escapedExp + "\',\'" + RegExSampleList[i].TransformedUrl + "\');return false;\">" + RegExSampleList[i].ExpressionMap + "</a>";
                dr["TRANSFORMED URL"] = "<a href=\"#\" onclick=\"selectExpMap(\'" + RegExSampleList[i].ExpressionMap + "\',\'" + escapedExp + "\',\'" + RegExSampleList[i].TransformedUrl + "\');return false;\">" + RegExSampleList[i].TransformedUrl + "</a>";
                dt.Rows.Add(dr);
            }
            DataView dv = new DataView(dt);
            regExPicker.DataSource = dv;
            regExPicker.DataBind();
        }
    }
    private void DisplayAdd(string siteid)
    {
        Toolbar("add", 0);
        UrlAliasRegExData add_regex = new UrlAliasRegExData(string.Empty, 0, "", "");
        long currSiteID;
        long.TryParse(siteid, out currSiteID);

        if (Page.IsPostBack && !String.IsNullOrEmpty(Request.Form[isCPostData.UniqueID]))
        {

            add_regex.SiteId = currSiteID;
            add_regex.ExpressionName = (string)txtExpressionName.Text;
            add_regex.Expression = (string)txtExpression.Text;
            add_regex.ExpressionMap = (string)txtExpressionMap.Text;
            add_regex.Priority = (EkEnumeration.RegExPriority)Enum.ToObject(typeof(EkEnumeration.RegExPriority), GetPriority(ddlSort.SelectedValue)); ;
            add_regex.IsEnabled = System.Convert.ToBoolean(activeChkBox.Checked);
            add_regex.TransformedUrl = (string)txtExampleURL.Text;

            try
            {
                add_regex = _aliasRegexAPI.Add(add_regex, _refContentApi.UserId);
            }
            catch (Exception ex)
            {
                Response.Redirect((string)("reterror.aspx?info=" + EkFunctions.UrlEncode(ex.Message) + "&LangType=" + _refContentApi.ContentLanguage), false);
                return;
            }
            Response.Redirect((string)("urlregexaliaslistmaint.aspx?fId=" + currSiteID));

        }
        else if (Page.IsPostBack && String.IsNullOrEmpty(Request.Form[isCPostData.UniqueID]))
        {
            System.Uri uri;
            string transformedUrl = string.Empty;
            if (txtRequestedUrl.Text.Contains("http://localhost"))
            {
                uri = new System.Uri(txtRequestedUrl.Text);
            }
            else
            {
                uri = new System.Uri("http://localhost" + _refContentApi.SitePath + txtRequestedUrl.Text);
            }
            transformedUrl = _aliasRegexAPI.EvaluateExpression(uri, (string)txtExpression.Text, (string)txtExpressionMap.Text);
            txtTransformedUrl.Text = transformedUrl;
        }
        else
        {
            txtExpression.Text = string.Empty;
            txtExpressionMap.Text = string.Empty;
            txtTransformedUrl.Text = string.Empty;
            txtExampleURL.Text = string.Empty;
            txtRequestedUrl.Text = string.Empty;
            GetPriorityList();
        }
    }
    private void GetPriorityList()
    {
        priorityList = _aliasRegexAPI.GetPriorityList();
        ddlSort.DataSource = priorityList;
        ddlSort.DataBind();
    }
    private void DisplayView()
    {
        UrlAliasRegExData data = InitData();
        Toolbar("view", data.RegExId);
        GetPriorityList();
        txtExpressionName.Text = data.ExpressionName;
        txtExpressionName.Enabled = false;
        txtExpression.Text = data.Expression;
        txtExpression.Enabled = false;
        txtExpressionMap.Text = data.ExpressionMap;
        txtExpressionMap.Enabled = false;
        txtExampleURL.Text = data.TransformedUrl;
        txtExampleURL.Enabled = false;
        ddlSort.SelectedValue = data.Priority.ToString();
        ddlSort.Enabled = false;
        activeChkBox.Checked = data.IsEnabled;
        activeChkBox.Enabled = false;
        quickLinkSelect.Visible = false;
        aliasTest.Visible = false;
    }
    private void DisplayEdit()
    {
        UrlAliasRegExData data = null;
        data = InitData();
        Toolbar("edit", data.RegExId);

        if (Page.IsPostBack && !String.IsNullOrEmpty(Request.Form[isCPostData.UniqueID]))
        {
            data.ExpressionName = (string)txtExpressionName.Text;
            data.Expression = (string)txtExpression.Text;
            data.ExpressionMap = (string)txtExpressionMap.Text;
            data.IsEnabled = System.Convert.ToBoolean(activeChkBox.Checked);
            data.Priority = (EkEnumeration.RegExPriority)Enum.ToObject(typeof(EkEnumeration.RegExPriority), GetPriority(ddlSort.SelectedValue));
            data.TransformedUrl = (string)txtExampleURL.Text;
            try
            {
                data = _aliasRegexAPI.Update(data, _refContentApi.UserId);
            }
            catch (Exception ex)
            {
                Response.Redirect((string)("reterror.aspx?info=" + EkFunctions.UrlEncode(ex.Message) + "&LangType=" + _refContentApi.ContentLanguage), false);
                return;
            }

            Response.Redirect((string)("urlregexaliasmaint.aspx?action=view&id=" + data.RegExId.ToString() + "&fId=" + Request.QueryString["fId"]));
        }
        else if (Page.IsPostBack && String.IsNullOrEmpty(Request.Form[isCPostData.UniqueID]))
        {
            System.Uri uri;
            string transformedUrl = string.Empty;
            if (txtRequestedUrl.Text.Contains("http://localhost"))
            {
                uri = new System.Uri(txtRequestedUrl.Text);
            }
            else
            {
                uri = new System.Uri("http://localhost" + _refContentApi.SitePath + txtRequestedUrl.Text);
            }
            transformedUrl = _aliasRegexAPI.EvaluateExpression(uri, (string)txtExpression.Text, (string)txtExpressionMap.Text);
            txtTransformedUrl.Text = transformedUrl;
        }
        else
        {
            //get data and fill values
            GetPriorityList();
            txtExpressionName.Text = data.ExpressionName;
            txtExpression.Text = data.Expression;
            txtExpressionMap.Text = data.ExpressionMap;
            activeChkBox.Checked = data.IsEnabled;
            ddlSort.SelectedValue = data.Priority.ToString();
            txtExampleURL.Text = data.TransformedUrl;
            txtRequestedUrl.Text = data.TransformedUrl;
        }
    }
    private void Toolbar(string mode, long id)
    {
        if (mode == "view")
        {
            divTitleBar.InnerHtml = _refStyle.GetTitleBar(msgHelper.GetMessage("lbl view regex"));
        }
        else if (mode == "edit")
        {
            divTitleBar.InnerHtml = _refStyle.GetTitleBar(msgHelper.GetMessage("lbl edit regex"));
        }
        else
        {
            divTitleBar.InnerHtml = _refStyle.GetTitleBar(msgHelper.GetMessage("msg add regex"));
        }

        System.Text.StringBuilder result = new System.Text.StringBuilder();
        result.Append("<table><tr>");

		if (mode == "edit")
		{
			result.Append(_refStyle.GetButtonEventsWCaption(_refCommonApi.AppImgPath + "../UI/Icons/back.png", "urlregexaliasmaint.aspx?action=view&id=" + id.ToString() + "&fId=" + Request.QueryString["fId"] + "", msgHelper.GetMessage("alt back button text"), msgHelper.GetMessage("btn back"), "", StyleHelper.BackButtonCssClass, true));
		}
		else if (mode == "view")
		{
			result.Append(_refStyle.GetButtonEventsWCaption(_refCommonApi.AppImgPath + "../UI/Icons/back.png", "urlregexaliaslistmaint.aspx?fId=" + Request.QueryString["fId"] + "", msgHelper.GetMessage("alt back button text"), msgHelper.GetMessage("btn back"), "", StyleHelper.BackButtonCssClass, true));
		}
		else
		{
			result.Append(_refStyle.GetButtonEventsWCaption(_refCommonApi.AppImgPath + "../UI/Icons/back.png", "urlregexaliaslistmaint.aspx?fId=" + Request.QueryString["fId"] + "", msgHelper.GetMessage("alt back button text"), msgHelper.GetMessage("btn back"), "", StyleHelper.BackButtonCssClass, true));
		}

        if (mode == "view")
        {
			result.Append(_refStyle.GetButtonEventsWCaption(_refCommonApi.AppImgPath + "../UI/Icons/contentEdit.png", "urlregexaliasmaint.aspx?action=editalias&id=" + id.ToString() + "&fId=" + Request.QueryString["fId"] + "", msgHelper.GetMessage("btn edit"), msgHelper.GetMessage("btn edit"), "", StyleHelper.EditButtonCssClass, true));
        }
        else if (mode == "edit")
        {
			result.Append(_refStyle.GetButtonEventsWCaption(_refCommonApi.AppImgPath + "../UI/Icons/save.png", "#", msgHelper.GetMessage("btn save"), msgHelper.GetMessage("btn save"), "Onclick=\"javascript: SubmitForm(\'form1\',\'VerifyAddAlias()\');\"", StyleHelper.SaveButtonCssClass, true));
        }
        else
        {
			result.Append(_refStyle.GetButtonEventsWCaption(_refCommonApi.AppImgPath + "../UI/Icons/save.png", "#", msgHelper.GetMessage("btn save"), msgHelper.GetMessage("btn save"), "Onclick=\"javascript: SubmitForm(\'form1\',\'VerifyAddAlias()\');\"", StyleHelper.SaveButtonCssClass, true));
        }

		result.Append(StyleHelper.ActionBarDivider);

        if (mode == "edit")
        {
            result.Append("<td>" + _refStyle.GetHelpButton("EditRegex", "") + "</td>");
        }
        else if (mode == "view")
        {
            result.Append("<td>" + _refStyle.GetHelpButton("ViewRegex", "") + "</td>");
        }
        else
        {
            result.Append("<td>" + _refStyle.GetHelpButton("AddRegEx", "") + "</td>");
        }

        result.Append("</tr></table>");
        divToolBar.InnerHtml = result.ToString();
        result = null;
        StyleSheetJS.Text = (new StyleHelper()).GetClientScript();
    }
    private UrlAliasRegExData InitData()
    {
        long id = 0;
        UrlAliasRegExData data = null;

        long.TryParse(Request.QueryString["id"], out id);
        if (id == 0)
        {
            throw (new ArgumentException("RegEx Id does not exists."));
        }

        data = _aliasRegexAPI.GetItem(id);
        if (data == null)
        {
            throw (new NullReferenceException("RegEx is not found"));
        }

        return data;
    }
    private int GetPriority(string selectedValue)
    {
        int priorityEnum;
        if (ddlSort.SelectedValue == "High")
        {
            priorityEnum = (int)EkEnumeration.RegExPriority.High;
        }
        else if (ddlSort.SelectedValue == "Medium")
        {
            priorityEnum = (int)EkEnumeration.RegExPriority.Medium;
        }
        else if (ddlSort.SelectedValue == "Low")
        {
            priorityEnum = (int)EkEnumeration.RegExPriority.Low;
        }
        else
        {
            priorityEnum = (int)EkEnumeration.RegExPriority.None;
        }
        return priorityEnum;
    }
    private void SetJSServerVariables()
    {
        Ektron.Cms.API.JS.RegisterJS(this, Ektron.Cms.API.JS.ManagedScript.EktronJS);
        Ektron.Cms.API.JS.RegisterJS(this, Ektron.Cms.API.JS.ManagedScript.EktronStyleHelperJS);
        Ektron.Cms.API.JS.RegisterJS(this, Ektron.Cms.API.JS.ManagedScript.EktronToolBarRollJS);
        Ektron.Cms.API.JS.RegisterJS(this, Ektron.Cms.API.JS.ManagedScript.EktronJFunctJS);

        Ektron.Cms.API.Css.RegisterCss(this, Ektron.Cms.API.Css.ManagedStyleSheet.EktronModalCss);

        Ektron.Cms.API.JS.RegisterJS(this, Ektron.Cms.API.JS.ManagedScript.EktronModalJS);
    }
    private void RegisterResources()
    {
        ltr_valURL.Text = msgHelper.GetMessage("alert enter a valid URL");
        ltr_enterRegexName.Text = msgHelper.GetMessage("alert msg no name for regex entered");
        ltr_enterRegex.Text = msgHelper.GetMessage("alert msg no regex entered");
        ltr_enterRegExMap.Text = msgHelper.GetMessage("alert msg no regexmap entered");
        ltr_follErr.Text = msgHelper.GetMessage("alert msg foll err");
    }
}