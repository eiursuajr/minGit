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
using Ektron.Cms.Commerce;
using Ektron.Cms.Site;
using Ektron.Cms.Common;
using Ektron.Cms.Workarea;

public partial class Commerce_bydates : workareabase
{

    CommonApi api = new Ektron.Cms.CommonApi();
    protected string sTarget = "ekavatarpath";

    #region Variables
    protected Ektron.ContentDesignerWithValidator cdEditor;
    protected string m_sEditAction = "";
    protected string editorPackage = "";
    protected ProductType m_refProductType = null;
    protected ProductTypeData prod_type_data = null;
    protected int xid = 0;
    protected bool bSuppressTemplate = false;
    protected FolderData catalog_data = new FolderData();
    protected int lValidCounter = 0;
    protected ContentMetaData[] meta_data = null;
    protected EntryData entry_edit_data = null;
    protected EkSite m_refSite = null;
    protected int m_iFolder = 0;
    protected MeasurementData m_mMeasures = null;
    #endregion
    protected override void Page_Load(object sender, System.EventArgs e)
    {
        base.Page_Load(sender, e);
        if (!Ektron.Cms.DataIO.LicenseManager.LicenseManager.IsFeatureEnable(m_refContentApi.RequestInformationRef, Ektron.Cms.DataIO.LicenseManager.Feature.eCommerce))
        {
            Utilities.ShowError(m_refContentApi.EkMsgRef.GetMessage("feature locked error"));
        }
		Utilities.ValidateUserLogin();
        Utils_RegisterResources();

        EkDTSelector dateSchedule;
        int end_date_action = 1;
        string go_live = "";
        string end_date = "";

        ltr_errStartEndDate.Text = m_refContentApi.EkMsgRef.GetMessage("js err start end date");
        ltr_startdate.Text = "Start Date";
        ltr_enddate.Text = "End Date";
        if (entry_edit_data != null)
        {
            go_live = entry_edit_data.GoLive.ToString();
            if (!(entry_edit_data.EndDate == DateTime.MinValue || entry_edit_data.EndDate == DateTime.MaxValue))
            {
                end_date = entry_edit_data.EndDate.ToString();
            }
            end_date_action = entry_edit_data.EndDateAction;
        }

        dateSchedule = this.m_refContentApi.EkDTSelectorRef;
        dateSchedule.formName = "frmMain";
        dateSchedule.extendedMeta = true;
        // start
        dateSchedule.formElement = "go_live";
        dateSchedule.spanId = "go_live_span";
        if (!string.IsNullOrEmpty(go_live))
        {
            dateSchedule.targetDate = DateTime.Parse(go_live);
        }
        ltr_startdatesel.Text = dateSchedule.displayCultureDateTime(true, "", "");
        dateSchedule.formElement = "end_date";
        dateSchedule.spanId = "end_date_span";
        if (!string.IsNullOrEmpty(end_date))
        {
            dateSchedule.targetDate = DateTime.Parse(end_date);
        }
        else
        {
            dateSchedule.targetDate = DateTime.MinValue;
        }
        ltr_enddatesel.Text = dateSchedule.displayCultureDateTime(true, "", "");
        // end

    }
    protected void Utils_RegisterResources()
    {

        Ektron.Cms.API.Css.RegisterCss(this, Ektron.Cms.API.Css.ManagedStyleSheet.EktronWorkareaCss);
        Ektron.Cms.API.Css.RegisterCss(this, m_refContentApi.AppPath + "csslib/box.css", "EktronBoxCSS");
        Ektron.Cms.API.Css.RegisterCss(this, m_refContentApi.AppPath + "csslib/pop_style.css", "EktronPopStyleCSS");

        Ektron.Cms.API.JS.RegisterJS(this, Ektron.Cms.API.JS.ManagedScript.EktronJS);
        Ektron.Cms.API.JS.RegisterJS(this, Ektron.Cms.API.JS.ManagedScript.EktronWorkareaJS);
        Ektron.Cms.API.JS.RegisterJS(this, Ektron.Cms.API.JS.ManagedScript.EktronThickBoxJS);
        Ektron.Cms.API.JS.RegisterJS(this, m_refContentApi.AppPath + "java/internCalendarDisplayFuncs.js", "EktronInternCalendarDisplayFuncsJS");
        Ektron.Cms.API.JS.RegisterJS(this, m_refContentApi.AppPath + "wamenu/includes/com.ektron.ui.menu.js", "EktronUIMenuJS");

    }
}


