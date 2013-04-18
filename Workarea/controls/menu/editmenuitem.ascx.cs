using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Microsoft.VisualBasic;
using Ektron.Cms.UI.CommonUI;
using Ektron.Cms.Common;
using Ektron.Cms;

public partial class Workarea_controls_menu_editmenuitem : System.Web.UI.UserControl
{
    protected ContentAPI m_refContentApi = new ContentAPI();
    protected EkMessageHelper MsgHelper;
    protected Collection gtLinks;
    protected long MenuId;
    protected long FolderId;
    protected string sitePath;
    protected string AppPath;
    protected void Page_Load(object sender, EventArgs e)
    {
        MsgHelper = new EkMessageHelper(m_refContentApi.RequestInformationRef);
        StyleHelper m_refStyle = new StyleHelper();
        CommonApi m_refApi = new CommonApi();
        ApplicationAPI AppUI = new ApplicationAPI();
        long Id = Convert.ToInt64(Request.QueryString["id"]);
        MenuId = Convert.ToInt64(Request.QueryString["nid"]);
        FolderId = Convert.ToInt64(Request.QueryString["folderid"]);
        gtLinks = AppUI.EkContentRef.GetMenuItemByID(MenuId, Id, true);
        sitePath = AppUI.SitePath;
        AppPath = AppUI.AppPath;
        long t_ID = Convert.ToInt64(gtLinks["ID"]);
        string t_ItemType = gtLinks["ItemType"].ToString();
        Int32 t_ContentLanguage = Convert.ToInt32(gtLinks["ContentLanguage"]);
        string t_iframe = "";
        if (!string.IsNullOrEmpty(Request.QueryString["iframe"]))
            t_iframe = Request.QueryString["iframe"].ToString();
        Title.Value = gtLinks["ItemTitle"].ToString();
        if ((string)gtLinks["ItemType"] == "1")
        {
            litInfo.Text += "<tr><td class=\"label\" title=\"Link\">";
            litInfo.Text += MsgHelper.GetMessage("lbl Link") + ":</td>";
            litInfo.Text += "<td class=\"value\">";
            litInfo.Text += "<input type=\"radio\" name=\"linkType\" value=\"0\" " + (((string)gtLinks["LinkType"] == "0" || (string)gtLinks["LinkType"] == "") ? "checked=\"checked\"" : "") + " title=\"QuickLink\" />" + MsgHelper.GetMessage("lbl QuickLink");
            litInfo.Text += "<input type=\"radio\" name=\"linkType\" value=\"1\" " + ((string)gtLinks["LinkType"] == "1" ? "checked=\"checked\"" : "") + " title=\"Menu Template\" />" + MsgHelper.GetMessage("lbl Menu Template") + "</td></tr>";
        }

        if ((Convert.ToInt32(t_ItemType) == 1) && (string.IsNullOrEmpty(gtLinks["ItemLink"].ToString())))
        {
            //Using the contentblock control to get the exact quicklink for asset which is not in contendata used above.
            //This is because the  contentdata or assetdata has nothing to show difference btw assets and privateassets.
            //Regarding the defect #58344
            Ektron.Cms.Controls.ContentBlock cBlock = new Ektron.Cms.Controls.ContentBlock();
            cBlock.Page = this.Page;
            cBlock.DefaultContentID = Convert.ToInt64(gtLinks["ItemID"]);
            cBlock.Fill();
            //urlLink.["URL"] = cBlock.EkItem.QuickLink;
            tdItemLink.Visible = false;
            tdURLLink.Visible = true;
            urlLink.Value = cBlock.EkItem.QuickLink;
        }
        if (t_ItemType != "5" && t_ItemType != "2")
        {
            if ((string)gtLinks["ItemLink"] != "")
                litInfo.Text += "<tr><td class=\"label\" title=\"Quick Link\">" + MsgHelper.GetMessage("lbl Quick Link") + ":</td><td class=\"readOnlyValue\">" + (string)gtLinks["ItemLink"] + "</td></tr>";
            if ((string)gtLinks["FolderId"] != "")
                litInfo.Text += "<tr><td class=\"label\" title=\"Folder Id\">" + MsgHelper.GetMessage("lbl Folder ID") + ":</td><td class=\"readOnlyValue\">" + (string)gtLinks["FolderId"] + "</td></tr>";
        }
        litImageLink.Text = MsgHelper.GetMessage("lbl Image Link");
        litTitle.Text = m_refStyle.GetTitleBar(MsgHelper.GetMessage("edit Menu items title"));
        litHelp.Text = StyleHelper.ActionBarDivider + "<td>" + m_refStyle.GetHelpButton("editmenuitem", "") + "</td>";
        litGenericTitle.Text = MsgHelper.GetMessage("generic title label");

        if (Request.QueryString["back"] != "")
            litButtons.Text = m_refStyle.GetButtonEventsWCaption(AppUI.AppPath + "images/UI/Icons/cancel.png", Request.QueryString["back"], MsgHelper.GetMessage("generic Cancel"), MsgHelper.GetMessage("generic Cancel"), "", StyleHelper.CancelButtonCssClass, true);
        else if (Request.QueryString["iframe"] == "true")
			litButtons.Text = m_refStyle.GetButtonEventsWCaption(AppUI.AppPath + "images/UI/Icons/cancel.png", "#", MsgHelper.GetMessage("generic Cancel"), MsgHelper.GetMessage("generic Cancel"), "onclick=\"parent.CancelIframe();\"", StyleHelper.CancelButtonCssClass, true);
        else
        {
            if (m_refApi.TreeModel == 1)
				litButtons.Text = m_refStyle.GetButtonEventsWCaption(AppUI.AppPath + "images/UI/Icons/back.png", "menutree.aspx?nid=" + MenuId, MsgHelper.GetMessage("alt back button text"), MsgHelper.GetMessage("btn back"), "", StyleHelper.BackButtonCssClass, true);
            else
				litButtons.Text = m_refStyle.GetButtonEventsWCaption(AppUI.AppPath + "images/UI/Icons/back.png", "collections.aspx?action=ViewMenu&nId=" + MenuId, MsgHelper.GetMessage("alt back button text"), MsgHelper.GetMessage("btn back"), "", StyleHelper.BackButtonCssClass, true);
        }

		litButtons.Text += m_refStyle.GetButtonEventsWCaption(AppUI.AppPath + "images/UI/Icons/save.png", "#", MsgHelper.GetMessage("alt save menu item"), MsgHelper.GetMessage("btn save"), "onclick=\"return SubmitForm(\'AddMenuItem\', \'VerifyAddMenuItem()\');\"", StyleHelper.SaveButtonCssClass, true);
    }
}