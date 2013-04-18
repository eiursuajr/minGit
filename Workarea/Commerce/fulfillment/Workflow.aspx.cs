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
using Ektron;
using Ektron.Cms;
using Ektron.Cms.Common;
using Ektron.Cms.Workarea;
using Ektron.Cms.Commerce;
using System.Data.SqlClient;
public partial class FulfillmentWorkflow : workareabase
{
    protected string curTypeName = string.Empty;
    protected string m_strWfImgPath = string.Empty;
    protected string AppPath = "";

    protected override void Page_Load(object sender, System.EventArgs e)
    {

        base.Page_Load(sender, e);
        if (!Ektron.Cms.DataIO.LicenseManager.LicenseManager.IsFeatureEnable(m_refContentApi.RequestInformationRef, Ektron.Cms.DataIO.LicenseManager.Feature.eCommerce))
        {
            Utilities.ShowError(m_refContentApi.EkMsgRef.GetMessage("feature locked error"));
        }
        CommonApi api = new CommonApi();
        if (!Utilities.ValidateUserLogin())
        {
            return;
        }
        Util_CheckAccess();
        AppPath = api.AppPath;

        m_strWfImgPath = AppPath + "workflowimage.aspx?type=preview&id=";
        Ektron.Cms.API.Css.RegisterCss(this, Ektron.Cms.API.Css.ManagedStyleSheet.EktronModalCss);
        Ektron.Cms.API.Css.RegisterCss(this, Ektron.Cms.API.Css.ManagedStyleSheet.EktronFixedPositionToolbarCss);
        // Register necessary JS files
        Ektron.Cms.API.JS.RegisterJS(this, Ektron.Cms.API.JS.ManagedScript.EktronJS);
        Ektron.Cms.API.JS.RegisterJS(this, Ektron.Cms.API.JS.ManagedScript.EktronXmlJS);
        Ektron.Cms.API.JS.RegisterJS(this, Ektron.Cms.API.JS.ManagedScript.EktronModalJS);
        Ektron.Cms.API.JS.RegisterJS(this, Ektron.Cms.API.JS.ManagedScript.EktronCookieJS);
        Ektron.Cms.API.JS.RegisterJS(this, api.AppPath + "java/jfunct.js", "EktronJFunctJS");
        Ektron.Cms.API.JS.RegisterJS(this, Ektron.Cms.API.JS.ManagedScript.EktronStringJS);
        Ektron.Cms.API.JS.RegisterJS(this, Ektron.Cms.API.JS.ManagedScript.EktronScrollToJS);
        Ektron.Cms.API.JS.RegisterJS(this, api.AppPath + "java/toolbar_roll.js", "EktronToolbarRollJS");

        lnkWorkflow.Text = "<img style=\"cursor:pointer\"  alt=\'click here to preview the selected workflow\' src=\'" + api.AppPath + "Images/ui/icons/preview.png\' onclick=\"setImageUrl(\'" + m_strWfImgPath + "\');$ektron(\'#wfImgModal\').modalShow(); return false;\"/>";
        workflowTitle.Text = GetMessage("lbl view workflow");

        if (!Page.IsPostBack)
        {

            Util_BindData();
            Util_SetLabels();
        }
        else
        {
            Process_Save();
            Util_SetLabels();
        }
    }

    #region Process

    protected void Process_Save()
    {

        try
        {

            m_refContentApi.EkSiteRef.UpdateWorkflowType((string)ddWf.Text, Ektron.Cms.Common.EkEnumeration.WorkflowType.Order);

            ltr_workflow.Text = GetMessage("lbl workflow saved");

            Util_BindData();

        }
        catch (Exception ex)
        {

            ltr_workflow.Text = ex.Message;

        }

    }

    #endregion

    #region Util

    protected void Util_BindData()
    {

        curTypeName = m_refContentApi.EkSiteRef.GetWorkflowType(Ektron.Cms.Common.EkEnumeration.WorkflowType.Order);

        ddWf.DataSource = Ektron.Workflow.Runtime.WorkflowHandler.GetOrderingWorkflows();
        ddWf.DataBind();

        if (!string.IsNullOrEmpty(curTypeName))
        {
            if (ddWf.Items.FindByValue(curTypeName) != null)
            {
                ddWf.Items.FindByValue(curTypeName).Selected = true;
            }
        }

    }

    protected void Util_SetLabels()
    {

        if (!Page.IsPostBack)
        {
            ltr_workflow.Text = GetMessage("lbl avail workflows");
        }

        SetTitleBarToMessage("lbl order workflow");

        workareamenu actionMenu = new workareamenu("action", this.GetMessage("lbl action"), AppPath + "images/UI/Icons/check.png");
        actionMenu.AddItem(AppPath + "images/ui/icons/save.png", this.GetMessage("btn save"), " document.forms[0].submit(); ");
        AddMenu(actionMenu);

        AddHelpButton("orderworkflow");

    }

    protected void Util_CheckAccess()
    {
        try
        {
            if (!m_refContentApi.IsARoleMember(Ektron.Cms.Common.EkEnumeration.CmsRoleIds.CommerceAdmin))
            {
                throw (new Exception("error not role commerce-admin"));
            }
        }
        catch (Exception ex)
        {
            Utilities.ShowError(ex.Message);
        }

    }
    #endregion

}


