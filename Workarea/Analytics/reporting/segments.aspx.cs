using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using Ektron.Cms;
using Ektron.Cms.Analytics;
using Ektron.Cms.Analytics.Providers;
using Ektron.Cms.Common;
using Ektron.Cms.Workarea;
using Microsoft.Security.Application;

public class SegmentDisplayData : Ektron.Cms.Analytics.Providers.SegmentData
{
    public SegmentDisplayData(SegmentData segment)
    {
        Id = segment.Id;
        Name = segment.Name;
        SegmentType = segment.SegmentType;
    }
    public bool Selected { get; set; }
    public string CmsSegmentValue { get; set; }
}
public partial class Analytics_reporting_segments : workareabase
{
    private string _siteCookieName = "";
    private IAnalytics _dataManager = ObjectFactory.GetAnalytics();
    private string _SelectedSiteId = "SiteAnalyticsSelectedSite";
    private string SegmentPersistenceId = string.Empty;
    private string _currentProvider;
    private string _user;
    private string _visitor;
    private string _identity = "";
    private bool _updateCookie = true;
    private string _widgetSegmentIds = string.Empty;

    protected List<string> CookieSegments
    {
        get
        {
            List<string> segmentIds = new List<string>();
            HttpCookie cookie = Request.Cookies[SegmentPersistenceId];
            if (cookie != null && !string.IsNullOrEmpty(cookie.Value))
            {
                segmentIds.AddRange(cookie.Value.Split(','));
            }
            return segmentIds;
        }
        set
        {
            string idList = string.Join(",", value.ToArray());
            HttpCookie cookie = new HttpCookie(SegmentPersistenceId, idList);
            cookie.Expires = System.DateTime.Today.AddHours(23).AddMinutes(59).AddSeconds(59);
            Response.Cookies.Add(cookie);
        }
    }
     
    protected override void OnInit(EventArgs e)
	{
        long userId = _dataManager.RequestInformation.UserId;
        _siteCookieName = userId.ToString() + "_" + _SelectedSiteId;
        base.OnInit(e);
        this.SetTitleBarToMessage("sam segments");
        if (!string.IsNullOrEmpty(Request.QueryString["updatecookie"]))
        {
            _updateCookie = Convert.ToBoolean(Request.QueryString["updatecookie"]);
        }
        if (!string.IsNullOrEmpty(Request.QueryString["identity"]))
        {
            _identity = AntiXss.HtmlEncode(Request.QueryString["identity"]);
        }
        if (!string.IsNullOrEmpty(Request.QueryString["segmentIds"]))
        {
            _widgetSegmentIds = AntiXss.HtmlEncode(Request.QueryString["segmentIds"]);
        }
        if (!string.IsNullOrEmpty(Request.QueryString["provider"]))
        {
            _currentProvider = AntiXss.HtmlEncode(Request.QueryString["provider"]);
        }
        else
        {
            string providerCookieName = userId.ToString() + "_SiteAnalyticsSelectedProvider";
            HttpCookie selectedProviderCookie = Request.Cookies[providerCookieName];
            if (selectedProviderCookie != null && selectedProviderCookie.Value != "")
            {
                _currentProvider = selectedProviderCookie.Value;
            }
        }

        if (!string.IsNullOrEmpty(_currentProvider) 
            && _dataManager.HasProvider(_currentProvider))
        {
                SegmentPersistenceId = _dataManager.GetSegmentFilterCookieName(_currentProvider);
        }

        _user = this.GetMessage("generic user");
        _visitor = this.GetMessage("lbl visitor");
        if (Page.IsPostBack)
        {
            List<string> selectedIds = new List<string>();
            foreach (string key in Request.Form.AllKeys)
            {
                if (key.IndexOf("chk_") == 0)
                {
                    string idPair = string.Empty;
                    string typeVal = key.Replace("chk_", "");
                    if (0 == typeVal.IndexOf("prop_"))
                    {
                        typeVal = Request.Form[key].ToString();
                        string[] objIds = Request.Form[typeVal].Split(',');
                        foreach (string id in objIds)
                        {
                            if (id.Length > 0)
                            {
                                idPair = id + "|" + typeVal.Substring(typeVal.LastIndexOf("_") + 1);
                                if (!selectedIds.Contains(idPair))
                                {
                                    selectedIds.Add(idPair);
                                }
                            }
                        }
                    }
                    else
                    {
                        idPair = typeVal + "|0";
                        if (!selectedIds.Contains(idPair))
                        {
                            selectedIds.Add(idPair);
                        }
                    }
                }
            }
            if (_updateCookie)
            {
                CookieSegments = selectedIds;
            }
            Page.ClientScript.RegisterStartupScript(this.GetType(), "UpdateSegments", "UpdateSegments('" + string.Join(",", selectedIds.ToArray()) + "', '" + _identity + "');", true);
        }
        else
        {
            try
            {
				this.AddButtonwithMessages(this.AppImgPath + "../UI/Icons/cancel.png", "#", "generic cancel", "generic cancel", " onclick=\"self.close();\" ", StyleHelper.CancelButtonCssClass, true);
                this.AddButtonwithMessages(this.AppImgPath + "../UI/Icons/save.png", "#", "btn save", "btn save", " onclick=\"forms[0].submit();return false;\" ", StyleHelper.SaveButtonCssClass, true);

                workareamenu actionMenu_1 = new workareamenu("action", this.GetMessage("lbl add custom segments"), this.AppImgPath + "../UI/Icons/add.png");
                // actionMenu_1.AddItem(m_refContentApi.AppPath + "images/ui/icons/user.png", this.GetMessage("lbl user id"), "AddCustomSegment(1, \"" + this.GetMessage("generic user") + "\");");
                // actionMenu_1.AddItem(m_refContentApi.AppPath + "images/ui/icons/users.png", this.GetMessage("lbl visitor id"), "AddCustomSegment(2, \"" + this.GetMessage("lbl visitor") + "\");");
                // this.AddMenu(actionMenu_1);

                
                if (_dataManager.IsAnalyticsViewer())
                {
                    HttpCookie selectedSiteCookie = Request.Cookies[_siteCookieName];
                    if (_updateCookie && selectedSiteCookie != null && selectedSiteCookie.Value != "")
                    {
                        try
                        {
                            BindItems(_dataManager.GetSegments(selectedSiteCookie.Value), CookieSegments);
                        }
                        catch
                        {
                            BindItems(_dataManager.GetSegments(_currentProvider), CookieSegments);
                        }
                    }
                    else if (false == _updateCookie)
                    {
                        List<string> segmentIds = new List<string>();
                        if (!string.IsNullOrEmpty(_widgetSegmentIds))
                        {
                            _widgetSegmentIds = _widgetSegmentIds.Replace("&#124;", "|");
                            segmentIds.AddRange(_widgetSegmentIds.Split(','));
                        }
                        BindItems(_dataManager.GetSegments(_currentProvider), segmentIds);
                    }
                    else
                    {
                        List<SegmentData> segments = new List<SegmentData>();
                        if (!string.IsNullOrEmpty(_currentProvider) 
                            && _dataManager.HasProvider(_currentProvider))
                        {
                            segments = _dataManager.GetSegments(_currentProvider);
                        }
                        else
                            segments.Add(new SegmentData("0", GetMessage("lbl all visits"), SegmentType.Default));
                        BindItems(segments, CookieSegments);
                    }
                }
            }
            catch (Exception ex)
            {
                Utilities.ShowError(ex.Message);
            }
        }
        this.SelectStatement.Text = GetMessage("lbl select up to four segments");
        Page.Title = GetMessage("lbl provider segment filter");
    }

    protected void BindItems(List<SegmentData> segments, List<string> selectedItems)
    {
        if (0 == segments.Count)
        {
            litErrorMessage.Text = GetMessage("lbl no segment setup for this provider");
            ErrorPanel.Visible = true;
        }

        List<SegmentDisplayData> defaultSegments = new List<SegmentDisplayData>();
        List<SegmentDisplayData> customSegments = new List<SegmentDisplayData>();
        foreach (SegmentData segment in segments)
        {
            SegmentDisplayData displaySegment = new SegmentDisplayData(segment);
            displaySegment.Selected = (selectedItems.FirstOrDefault(c => c == displaySegment.Id + "|0") != null);
            displaySegment.CmsSegmentValue = string.Empty;
            if (segment.SegmentType == SegmentType.Custom)
                customSegments.Add(displaySegment);
            else
                defaultSegments.Add(displaySegment);
        }

        if (this.CookieSegments.Count > 0)
        {
            int idx = 0;
            foreach (string idPair in this.CookieSegments)
            {
                string sSegProp = idPair.Substring(idPair.IndexOf("|") + 1);
                SegmentProperty segProp = (SegmentProperty)Convert.ToInt32(sSegProp);
                if (segProp != SegmentProperty.Id)
                {
                    string segValue = idPair.Replace("|" + sSegProp, "");
                    string segDisplayName = string.Empty;
                    switch (segProp)
                    {
                        case SegmentProperty.UserId:
                            segDisplayName = _user + " ";
                            break;
                        case SegmentProperty.VisitorId:
                            segDisplayName = _visitor + " ";
                            break;
                    }
                    SegmentData seg = new SegmentData("prop_" + idx.ToString() + "_" + sSegProp, segDisplayName, SegmentType.CMS);
                    SegmentDisplayData customSegment = new SegmentDisplayData(seg);
                    customSegment.Selected = true;
                    customSegment.CmsSegmentValue = segValue;
                    customSegments.Add(customSegment);
                    idx++;
                }
            }
        }
        rpt_Default.DataSource = defaultSegments;
        rpt_Default.DataBind();
        rpt_Custom.DataSource = customSegments;
        rpt_Custom.DataBind();
    }
}
