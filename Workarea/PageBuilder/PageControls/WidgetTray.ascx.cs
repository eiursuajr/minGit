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
using Ektron.Cms.Widget;
using Ektron.Cms.PageBuilder;
using Ektron.Cms.Common;


public partial class pagebuilder_PlaceHolder_WidgetTray : System.Web.UI.UserControl
{
    protected EkRequestInformation requestInformation = null;
    protected EkRequestInformation RequestInformation
    {
        get
        {
            if (requestInformation == null)
            {
                requestInformation = ObjectFactory.GetRequestInfoProvider().GetRequestInformation();
            }
            return requestInformation;
        }
    }
    //private SiteAPI __siteapi = null;
    //protected SiteAPI _siteApi
    //{
    //    get
    //    {
    //        if (__siteapi == null) __siteapi = new SiteAPI();
    //        return __siteapi;
    //    }
    //}

    protected void Page_Load(object sender, EventArgs e)
    {
        long wireframeid = 0;
        PageBuilder page = (Page as PageBuilder);
        if (page != null && page.Pagedata != null && page.Pagedata.pageID != 0 && page.Status != Ektron.Cms.PageBuilder.Mode.AnonViewing)
        {
            IWireframeModel wfm = ObjectFactory.GetWireframeModel();
            WireframeData wfd = wfm.FindByPageID(page.Pagedata.pageID);
            if (wfd != null && wfd.ID != 0)
            {
                wireframeid = wfd.ID;
                WidgetTypeData[] wtd = wfm.GetAssociatedWidgetTypes(wireframeid);
                if (wtd != null && wtd.Length > 0)
                {
                    repWidgetTypes.DataSource = wtd;
                    repWidgetTypes.DataBind();
                }
            }
        }
    }
}
