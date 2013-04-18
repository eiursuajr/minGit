using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Ektron.Cms.Common;
using Ektron.Cms;

public partial class MediaUploaderCommon : System.Web.UI.UserControl
{
    protected EkMessageHelper m_refMsg;
    protected ContentAPI cApi = new ContentAPI();

    protected void Page_Load(object sender, EventArgs e)
    {
        m_refMsg = cApi.EkMsgRef;
       
        jsEditorClosed.Text = m_refMsg.GetMessage("js: alert editor closed");
        jsScope.Text = Ektron.Cms.Common.EkFunctions.HtmlEncode(Request.QueryString["scope"]);
        jsEditorName.Text = Ektron.Cms.Common.EkFunctions.HtmlEncode(Request.QueryString["EditorName"]);
        jsDEntrylink.Text = Ektron.Cms.Common.EkFunctions.HtmlEncode(Request.QueryString["dentrylink"]);
    }
}
