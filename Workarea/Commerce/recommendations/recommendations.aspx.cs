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
using Ektron.Cms.Common;
using Ektron.Cms.Workarea;
using Microsoft.Security.Application;

public partial class Commerce_recommendations : workareabase
{
    protected long m_iFolderId = 0;
    protected List<RecommendationItemData> RecommendationList = new List<RecommendationItemData>();
    protected RecommendationApi recommendationManager = null;
    protected EkEnumeration.RecommendationType RecType = EkEnumeration.RecommendationType.CrossSell;
    protected PermissionData security_data = null;

    #region Page Functions
    protected override void Page_Load(object sender, System.EventArgs e)
    {
        base.Page_Load(sender, e);
        if (!Ektron.Cms.DataIO.LicenseManager.LicenseManager.IsFeatureEnable(m_refContentApi.RequestInformationRef, Ektron.Cms.DataIO.LicenseManager.Feature.eCommerce))
        {
            Utilities.ShowError(m_refContentApi.EkMsgRef.GetMessage("feature locked error"));
        }
        Util_RegisterResources();
        try
        {
            if (!string.IsNullOrEmpty(Request.QueryString["folder"]))
            {
                m_iFolderId = Convert.ToInt64(Request.QueryString["folder"]);
            }
            Util_CheckAccess();
            switch (m_sPageAction)
            {
                case "crosssell":
                    RecType = EkEnumeration.RecommendationType.CrossSell;
                    if (Page.IsPostBack)
                    {
                        Process_CrossSell();
                    }
                    else
                    {
                        Display_CrossSell();
                    }
                    break;
                case "upsell":
                    RecType = EkEnumeration.RecommendationType.Upsell;
                    if (Page.IsPostBack)
                    {
                        Process_UpSell();
                    }
                    else
                    {
                        Display_UpSell();
                    }
                    break;
            }
            Util_SetLabels();
            Util_SetJS();
        }
        catch (Exception ex)
        {
            Utilities.ShowError(ex.Message);
        }
    }
    #endregion
    #region Display
    protected void Display_CrossSell()
    {
        recommendationManager = new Ektron.Cms.Commerce.RecommendationApi();
        RecommendationList = recommendationManager.GetList(m_iID, ContentLanguage, RecType);
        Util_BuildRecommendations();
    }
    protected void Display_UpSell()
    {
        recommendationManager = new Ektron.Cms.Commerce.RecommendationApi();
        RecommendationList = recommendationManager.GetList(m_iID, ContentLanguage, RecType);
        Util_BuildRecommendations();
    }
    #endregion
    #region Process
    protected void Process_CrossSell()
    {
        recommendationManager = new Ektron.Cms.Commerce.RecommendationApi();
        RecommendationList = Process_GetRecommendations();
        recommendationManager.UpdateCrossSell(m_iID, RecommendationList);

        Response.Redirect((string)("../../content.aspx?action=View&folder_id=" + m_iFolderId + "&id=" + m_iID + "&LangType=" + ContentLanguage + "&callerpage=content.aspx&origurl=action=%3dViewContentByCategory%26id%3d" + m_iFolderId), false);
    }
    protected void Process_UpSell()
    {
        recommendationManager = new Ektron.Cms.Commerce.RecommendationApi();
        RecommendationList = Process_GetRecommendations();
        recommendationManager.UpdateUpSell(m_iID, RecommendationList);

        Response.Redirect((string)("../../content.aspx?action=View&folder_id=" + m_iFolderId + "&id=" + m_iID + "&LangType=" + ContentLanguage + "&callerpage=content.aspx&origurl=action=%3dViewContentByCategory%26id%3d" + m_iFolderId), false);
    }
    public List<RecommendationItemData> Process_GetRecommendations()
    {
        System.Collections.Generic.List<RecommendationItemData> aRecommendations = new System.Collections.Generic.List<RecommendationItemData>();
        int i = 1;
        while (Request.Form["rec_" + i.ToString() + "_posidx"] != null)
        {
            RecommendationItemData Recommendation = new RecommendationItemData();
            int idx = Convert.ToInt32(Request.Form["rec_" + i.ToString() + "_posidx"]);
            Recommendation.DisplayOrder = i + 1;
            Recommendation.Id = Convert.ToInt32(Request.Form["rec_" + idx.ToString() + "_id"]);
            Recommendation.EntryId = Convert.ToInt32(Request.Form["rec_" + idx.ToString() + "_entryid"]);
            Recommendation.EntryLanguage = this.ContentLanguage;
            aRecommendations.Add(Recommendation);
            i++;
        }
        return aRecommendations;
    }
    #endregion
    #region Util
    protected void Util_SetLabels()
    {
        switch (m_sPageAction)
        {
            case "crosssell":
                SetTitleBarToMessage("lbl cross sell rec");
                break;
            case "upsell":
                SetTitleBarToMessage("lbl up sell rec");
                break;
        }

        workareamenu actionMenu = new workareamenu("action", this.GetMessage("lbl action"), m_refContentApi.AppPath + "images/ui/icons/brick.png"); // check2.gif
		AddBackButton((string)("../../content.aspx?action=View&folder_id=" + m_iFolderId + "&id=" + m_iID + "&LangType=" + ContentLanguage + "&callerpage=content.aspx&origurl=action=%3dViewContentByCategory%26id%3d" + m_iFolderId));
		if (security_data.CanEdit)
        {
            actionMenu.AddItem(m_refContentApi.AppPath + "images/ui/icons/save.png", this.GetMessage("btn save"), "document.forms[0].submit();");
            actionMenu.AddBreak();
        }
        actionMenu.AddLinkItem(m_refContentApi.AppPath + "images/ui/icons/cancel.png", this.GetMessage("generic cancel"), (string)("../../content.aspx?action=View&folder_id=" + m_iFolderId + "&id=" + m_iID + "&LangType=" + ContentLanguage + "&callerpage=content.aspx&origurl=action=%3dViewContentByCategory%26id%3d" + m_iFolderId));
        this.AddMenu(actionMenu);
        AddHelpButton(EkFunctions.HtmlEncode(m_sPageAction));
    }
    protected void Util_BuildRecommendations()
    {
        StringBuilder sbItems = new StringBuilder();
        sbItems = new StringBuilder();
        sbItems.Append("<table width=\"100%\" id=\"Table1\" runat=\"server\">		").Append(Environment.NewLine);
        sbItems.Append("<tr>").Append(Environment.NewLine);
        sbItems.Append("<td>").Append(Environment.NewLine);
        sbItems.Append("    <table width=\"100%\">").Append(Environment.NewLine);
        sbItems.Append("    <tr>").Append(Environment.NewLine);
        sbItems.Append("    <td>").Append(Environment.NewLine);
        sbItems.Append("        <table width=\"100%\">").Append(Environment.NewLine);
        sbItems.Append("        <tr>").Append(Environment.NewLine);
        sbItems.Append("        <td>").Append(Environment.NewLine);
        sbItems.Append("            <table align=\"left\" width=\"100%\">").Append(Environment.NewLine);
        sbItems.Append("            <tr>").Append(Environment.NewLine);
        sbItems.Append("            <td>").Append(Environment.NewLine);
        sbItems.Append("                <table align=\"center\" width=\"100%\">").Append(Environment.NewLine);
        sbItems.Append("                <tr>").Append(Environment.NewLine);
        sbItems.Append("                <td width=\"50%\">").Append(Environment.NewLine);
        sbItems.Append("                        <table id=\"tblRecommendations\" class=\"ektableutil ektronGrid\">").Append(Environment.NewLine);
        sbItems.Append("                        <thead>").Append(Environment.NewLine);
        sbItems.Append("                        <tr class=\"title-header\">").Append(Environment.NewLine);
        sbItems.Append("                            <th></th><th>").Append(GetMessage("generic id")).Append("</th><th>").Append(GetMessage("generic title")).Append("</th><th>&#160;</th><th>&#160;</th>").Append(Environment.NewLine);
        sbItems.Append("                        </tr>").Append(Environment.NewLine);
        sbItems.Append("                        </thead>").Append(Environment.NewLine);
        sbItems.Append("                        <tbody>").Append(Environment.NewLine);
        for (int i = 0; i <= (RecommendationList.Count - 1); i++)
        {
            sbItems.Append("<tr");
            if (i % 2 > 0)
            {
                sbItems.Append(" class=\"itemrow0\"");
            }
            sbItems.Append(">").Append(Environment.NewLine);
            sbItems.Append("<td>").Append(i + 1).Append("</td>").Append(Environment.NewLine);
            sbItems.Append("<td>").Append(RecommendationList[i].EntryId).Append("</td>").Append(Environment.NewLine);
            sbItems.Append("<td>").Append(RecommendationList[i].Title).Append("</td>").Append(Environment.NewLine);
            sbItems.Append("<td>");
            sbItems.Append("<input type=\"hidden\" id=\"rec_").Append(i + 1).Append("_id\" name=\"rec_").Append(i + 1).Append("_id\" value=\"").Append(RecommendationList[i].Id).Append("\" />");
            sbItems.Append("<input type=\"hidden\" id=\"rec_").Append(i + 1).Append("_entryid\" name=\"rec_").Append(i + 1).Append("_entryid\" value=\"").Append(RecommendationList[i].EntryId).Append("\" />");
            sbItems.Append("<input type=\"hidden\" id=\"rec_").Append(i + 1).Append("_posidx\" name=\"rec_").Append(i + 1).Append("_posidx\" value=\"").Append(i + 1).Append("\" />");
            sbItems.Append("</td>").Append(Environment.NewLine);
            sbItems.Append("<td><input type=\"radio\" value=\"").Append(i + 1).Append("\" name=\"radInput\" /></td>").Append(Environment.NewLine);
            sbItems.Append("</tr>").Append(Environment.NewLine);
        }
        sbItems.Append("                        </tbody>").Append(Environment.NewLine);
        sbItems.Append("                        </table>").Append(Environment.NewLine);
        sbItems.Append("<p>");
        if (security_data.CanEdit)
        {
            sbItems.Append("<div class=\"ektronTopSpace\">");
            sbItems.Append("    <a class=\"button buttonInline greenHover buttonAdd\" style=\"cursor: pointer; margin-right: .25em;\" title=\" ").Append(GetMessage("generic add title")).Append(" \" onclick=\"AddRecommendation();\">").Append(GetMessage("generic add title")).Append("</a>");
            sbItems.Append("    <a class=\"button buttonInline redHover buttonClear\" style=\"cursor: pointer;\" title=\"").Append(GetMessage("generic remove")).Append("\" onclick=\"DeleteRecommendation();\" />").Append(GetMessage("generic remove")).Append("</a>");
            sbItems.Append("</div>");
        }
        sbItems.Append("</p> ").Append(Environment.NewLine);
        sbItems.Append("		</td>").Append(Environment.NewLine);
        sbItems.Append("		</tr>").Append(Environment.NewLine);
        sbItems.Append("                </table>").Append(Environment.NewLine);
        sbItems.Append("            </td>").Append(Environment.NewLine);
        sbItems.Append("            </tr>").Append(Environment.NewLine);
        sbItems.Append("            </table>").Append(Environment.NewLine);
        sbItems.Append("        </td>").Append(Environment.NewLine);
        sbItems.Append("        </tr>").Append(Environment.NewLine);
        sbItems.Append("        </table>").Append(Environment.NewLine);
        sbItems.Append("    </td>").Append(Environment.NewLine);
        sbItems.Append("    </tr>").Append(Environment.NewLine);
        sbItems.Append("    </table>").Append(Environment.NewLine);
        sbItems.Append("</td>").Append(Environment.NewLine);
        sbItems.Append("</tr>").Append(Environment.NewLine);
        sbItems.Append("</table>").Append(Environment.NewLine);
        ltr_recommendations.Text = sbItems.ToString();
    }
    protected void Util_CheckAccess()
    {
        if (!Utilities.ValidateUserLogin())
        {
            return;
        }
        if (m_refContentApi.RequestInformationRef.IsMembershipUser == 1)
        {
            Response.Redirect(m_refContentApi.ApplicationPath + "reterror.aspx?info=" + m_refMsg.GetMessage("msg login cms user"), false);
        }
        security_data = m_refContentApi.LoadPermissions(m_iID, "content", 0);
    }
    protected void Util_SetJS()
    {
        StringBuilder sbJS = new StringBuilder();
        sbJS.Append("<script  type=\"text/javascript\">").Append(Environment.NewLine);
        sbJS.Append("   function AddRecommendation() { ").Append(Environment.NewLine);
        sbJS.Append("       ektb_show(\'<span>" + GetMessage("lbl select item to add") + "</span>\',\'../itemselection.aspx?action=").Append(m_sPageAction).Append("&id=").Append(m_iFolderId.ToString()).Append("&EkTB_iframe=true&height=300&width=500&modal=true\', null); ").Append(Environment.NewLine);
        sbJS.Append("   } ").Append(Environment.NewLine);
        sbJS.Append("   function DeleteRecommendation() {").Append(Environment.NewLine);
        sbJS.Append("       var iAttr = getCheckedInt(false);").Append(Environment.NewLine);
        sbJS.Append("        if (iAttr == -1) {").Append(Environment.NewLine);
        sbJS.Append("            alert(\'").Append(GetMessage("js please sel rec")).Append("\');").Append(Environment.NewLine);
        sbJS.Append("        } else {").Append(Environment.NewLine);
        sbJS.Append("            deleteChecked();").Append(Environment.NewLine);
        sbJS.Append("        }").Append(Environment.NewLine);
        sbJS.Append("   }").Append(Environment.NewLine);
        sbJS.Append("</script>").Append(Environment.NewLine);
        ltr_js.Text = sbJS.ToString();
    }
    protected void Util_RegisterResources()
    {
        Ektron.Cms.API.Css.RegisterCss(this, Ektron.Cms.API.Css.ManagedStyleSheet.EktronWorkareaCss);
        Ektron.Cms.API.Css.RegisterCss(this, Ektron.Cms.API.Css.ManagedStyleSheet.EktronWorkareaIeCss, Ektron.Cms.API.Css.BrowserTarget.AllIE);
        Ektron.Cms.API.Css.RegisterCss(this, this.m_refContentApi.AppPath + "csslib/tables/tableutil.css", "EktronTableUtilCSS");
        Ektron.Cms.API.Css.RegisterCss(this, this.m_refContentApi.AppPath + "csslib/box.css", "EktronBoxCSS");

        Ektron.Cms.API.JS.RegisterJS(this, Ektron.Cms.API.JS.ManagedScript.EktronJS);
        Ektron.Cms.API.JS.RegisterJS(this, this.m_refContentApi.AppPath + "java/dhtml/rectableutil.js", "EktronRectableUtilJS");
        Ektron.Cms.API.JS.RegisterJS(this, Ektron.Cms.API.JS.ManagedScript.EktronThickBoxJS);
    }
    #endregion

}


