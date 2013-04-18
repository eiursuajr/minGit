using System;
using System.Data;
using System.Configuration;
using System.Collections;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using Ektron.Cms;

public partial class Workarea_PageBuilder_Wizards_js_wizardResources : System.Web.UI.Page
{
    protected Ektron.Cms.Common.EkMessageHelper m_refMsg;
    protected SiteAPI m_refSiteApi = new SiteAPI();

    protected void Page_Load(object sender, EventArgs e)
    {
        Response.ContentType = "application/javascript";
        // initialize additional variables for later use
        m_refMsg = m_refSiteApi.EkMsgRef;

        // instantiate contentAPI reference
        ContentAPI contentApi = new ContentAPI();

        // assign the resource text values as needed
        jsAddPage.Text = m_refMsg.GetMessage("lbl pagebuilder add page");
        jsAppPath.Text = contentApi.AppPath;
        jsBack.Text = m_refMsg.GetMessage("back");
        jsCancel.Text = m_refMsg.GetMessage("btn cancel");
        jsFinish.Text = m_refMsg.GetMessage("btn finish");
        jsNext.Text = m_refMsg.GetMessage("btn next");
        jsOk.Text = m_refMsg.GetMessage("lbl ok");
        jsSavePageAs.Text = m_refMsg.GetMessage("lbl pagebuilder save page");
        jsWizardsPath.Text = (contentApi.AppPath + "pagebuilder/wizards/");
        jsErrorPageTitle.Text = m_refMsg.GetMessage("lbl pagebuilder error page title");
        jsErrorSelectLayout.Text = m_refMsg.GetMessage("lbl pagebuilder error select layout");
        jsErrorUrlAlias.Text = m_refMsg.GetMessage("lbl pagebuilder error url alias");
        jsErrorUrlAliasExists.Text = m_refMsg.GetMessage("lbl pagebuilder error url alias exists");
        jsdropdownMustMatch.Text = m_refMsg.GetMessage("lbl pagebuilder error url alias selection must match");
        jsinvalidExtension.Text = m_refMsg.GetMessage("lbl pagebuilder error url alias invalid extension");
        jsselectExtension.Text = m_refMsg.GetMessage("lbl pagebuilder error url alias select extension");
        jserrorMetadata.Text = m_refMsg.GetMessage("lbl pagebuilder error metadata");
        jserrorTaxonomy.Text = m_refMsg.GetMessage("lbl pagebuilder error taxonomy");
        jsloading.Text = m_refMsg.GetMessage("lbl sync loading");
        jsAddMaster.Text = m_refMsg.GetMessage("lbl Add Master Layout");
    }
}
