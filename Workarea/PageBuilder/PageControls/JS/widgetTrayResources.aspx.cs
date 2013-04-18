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

public partial class Workarea_PageBuilder_PageControls_JS_widgetTrayResources : System.Web.UI.Page
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
        jsCancel.Text = m_refMsg.GetMessage("generic cancel");
        jsDropControlHere.Text = m_refMsg.GetMessage("lbl pagebuilder drop control here");
        jsEm.Text = m_refMsg.GetMessage("generic em");
        jsNewWidth.Text = m_refMsg.GetMessage("lbl pagebuilder new width");
        jsPixels.Text = m_refMsg.GetMessage("generic pixels");
        jsPercent.Text = m_refMsg.GetMessage("generic percent");
        jsSave.Text = m_refMsg.GetMessage("generic save");
        jsWidget.Text = m_refMsg.GetMessage("generic widget");
    }
}
