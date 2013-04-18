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
using Ektron.Cms.Common;

namespace Ektron.ContentDesigner.DictionaryConfigurator
{
    /// <summary>
    /// Summary description for DictionaryConfigurator.
    /// </summary>
    public partial class DictionaryConfigurator : System.Web.UI.Page
    {
        protected EkMessageHelper m_refMsg;
        protected ContentAPI content_api = new ContentAPI();

        private void Page_PreInit(System.Object sender, System.EventArgs e)
        {
            m_refMsg = content_api.EkMsgRef;
            if (content_api.RequestInformationRef.IsMembershipUser == 1 || content_api.RequestInformationRef.UserId == 0)
            {
                Response.Redirect(content_api.ApplicationPath + "reterror.aspx?info=" + Server.UrlEncode(m_refMsg.GetMessage("msg login cms user")), false);
                return;
            }
        }
    }
}

