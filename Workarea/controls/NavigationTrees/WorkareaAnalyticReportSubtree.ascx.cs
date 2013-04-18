using System;
using System.Collections.Generic;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Ektron.Cms;
using Ektron.Cms.Common;
using Ektron.Cms.DataIO.LicenseManager;

public partial class WorkareaAnalyticReportSubtree : System.Web.UI.UserControl {
    
    public System.Web.UI.HtmlControls.HtmlControl TreeContainer 
    { get { return SiteAnalytics; } }

    private ContentAPI _contentApi = null;
    public ContentAPI ContentApi { get { return _contentApi ?? (_contentApi = new ContentAPI()); } }
    
    private EkMessageHelper _messageHelperRef = null;
    public EkMessageHelper MessageHelper { get { return (_messageHelperRef ?? (_messageHelperRef = ContentApi.EkMsgRef)); } }

    public string GetMessage(string key) 
    { return MessageHelper.GetMessage(key); }

    protected void Page_Init(object sender, EventArgs e) {
    }

    protected void Page_Load(object sender, EventArgs e) {
    }
}
