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
using Ektron.Cms.Content;

public partial class Menu : System.Web.UI.Page
{
    protected EkMessageHelper m_refMsg;
    protected string m_strPageAction = "";
    protected long CurrentUserId = 0;
    protected string AppImgPath = "";
    protected CommonApi m_refCommon = new CommonApi();
    protected int EnableMultilingual = 0;
    protected EkContent m_refContent;
    protected int MenuLanguage = -1;

    protected void Page_Load(object sender, System.EventArgs e)
    {
        try
        {
            Response.CacheControl = "no-cache";
            Response.AddHeader("Pragma", "no-cache");
            Response.Expires = -1;
            CurrentUserId = m_refCommon.RequestInformationRef.UserId;
            m_refContent = m_refCommon.EkContentRef;
            m_refMsg = m_refCommon.EkMsgRef;
            RegisterResources();
            //TODO: Ross - Not sure which role to check
            if ((CurrentUserId == 0) || (Convert.ToBoolean(m_refCommon.RequestInformationRef.IsMembershipUser) && m_refContent.IsARoleMember((long)Ektron.Cms.Common.EkEnumeration.CmsRoleIds.TaxonomyAdministrator, CurrentUserId, false) == false))
            {
                Response.Redirect("login.aspx?fromLnkPg=1", false);
                return;
            }
            else
            {
                AppImgPath = m_refCommon.AppImgPath;
                EnableMultilingual = m_refCommon.EnableMultilingual;
                if ((Request.QueryString["action"] != null) && Request.QueryString["action"] != "")
                {
                    m_strPageAction = Request.QueryString["action"].ToLower();
                }
                Utilities.SetLanguage(m_refCommon);
                MenuLanguage = m_refCommon.ContentLanguage;
                switch (m_strPageAction)
                {
                    case "deleted":
                        Message.Text = m_refMsg.GetMessage("lbl menu") + " \'" + Request.QueryString["title"] + "\' " + m_refMsg.GetMessage("lbl deleted");
                        Message.Text = Message.Text + "<script language=\"javascript\">" + "\r\n";
                        Message.Text = Message.Text + "top.refreshMenuAccordion(" + MenuLanguage + ");" + "\r\n";
                        Message.Text = Message.Text + "</script>" + "\r\n";
                        break;
                    case "viewcontent":
                    case "removeitems":
                        Control m_vi;
                        m_vi = (Control)(LoadControl("controls/menu/viewitems.ascx"));
                        m_vi.ID = "menu";
                        DataHolder.Controls.Add(m_vi);
                        break;
                    case "viewmenu":
                        Control m_va;
                        m_va = (Control)(LoadControl("controls/menu/viewmenu.ascx"));
                        m_va.ID = "menuprops";
                        DataHolder.Controls.Add(m_va);
                        break;
                }
            }
        }
        catch (System.Threading.ThreadAbortException)
        {
            //Do nothing
        }
        catch (Exception ex)
        {
            Response.Redirect((string)("reterror.aspx?info=" + EkFunctions.UrlEncode(ex.Message + ".") + "&LangType=" + MenuLanguage), false);
        }
    }

    private void RegisterResources()
    {
        Ektron.Cms.API.JS.RegisterJS(this, Ektron.Cms.API.JS.ManagedScript.EktronJS);
        Ektron.Cms.API.JS.RegisterJS(this, Ektron.Cms.API.JS.ManagedScript.EktronWorkareaJS);
        Ektron.Cms.API.JS.RegisterJS(this, m_refCommon.ApplicationPath + "java/jfunct.js", "EktronJFunctJS");
        Ektron.Cms.API.JS.RegisterJS(this, m_refCommon.ApplicationPath + "java/toolbar_roll.js", "EktronToolbarRollJS");
        Ektron.Cms.API.JS.RegisterJS(this, m_refCommon.ApplicationPath + "java/workareahelper.js", "EktronWorkareaHelperJS");

        Ektron.Cms.API.Css.RegisterCss(this, Ektron.Cms.API.Css.ManagedStyleSheet.EktronWorkareaCss);
        Ektron.Cms.API.Css.RegisterCss(this, m_refCommon.ApplicationPath + "csslib/ektron.fixedPositionToolbar.css", "FixedPosToolbarCss");
    }
}
