using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Microsoft.VisualBasic;
using Ektron.Cms.UI.CommonUI;
using Ektron.Cms;
using Ektron.Cms.Common;

public partial class Workarea_controls_menu_addmenuitem : System.Web.UI.UserControl
{
    const Int64 ALL_CONTENT_LANGUAGES = -1;
    protected ContentAPI m_refContentApi = new ContentAPI();
    protected long MenuId = 0;
    protected long mpID = 0;
    protected long maID = 0;
    protected long FolderId = 0;
    protected Int32 ContentLanguage = 0;
    protected string noWorkAreaString = string.Empty;
    protected void Page_Load(object sender, EventArgs e)
    {
        EkMessageHelper MsgHelper = new EkMessageHelper(m_refContentApi.RequestInformationRef);
        StyleHelper m_refStyle = new StyleHelper();
        CommonApi m_refApi = new CommonApi();
        
        Collection gtNavs = new Collection();
        ApplicationAPI AppUI = new ApplicationAPI();
        FolderId = Convert.ToInt64(Request.QueryString["folderid"]);
        MenuId = Convert.ToInt64(Request.QueryString["nId"]);
        mpID = Convert.ToInt64(Request.QueryString["parentid"]);
        maID = Convert.ToInt64(Request.QueryString["ancestorid"]);
        if (!string.IsNullOrEmpty(Request.QueryString["LangType"]))
        {
            ContentLanguage = Convert.ToInt32(Request.QueryString["LangType"]);
            if (ContentLanguage == ALL_CONTENT_LANGUAGES)
            {
                ContentLanguage = AppUI.DefaultContentLanguage;
            }

            AppUI.FilterByLanguage = ContentLanguage;
        }
        else
            ContentLanguage = AppUI.ContentLanguage;
        if (!string.IsNullOrEmpty(Request.QueryString["noworkarea"]) && Request.QueryString["noworkarea"] == "1")
        {
            noWorkAreaString = "&noworkarea=1";
        }

        if (MenuId != 0)
        {
            gtNavs = AppUI.EkContentRef.GetMenuByID(MenuId, false, false);
        }
        
        litAddMenuTitle.Text = m_refStyle.GetTitleBar(MsgHelper.GetMessage("Add Menu Item Title"));

		bool addHelpDivider = true;

        if (!(string.IsNullOrEmpty(Request.QueryString["back"])))
			litButtons.Text = m_refStyle.GetButtonEventsWCaption(AppUI.AppPath + "images/UI/Icons/cancel.png", Request.QueryString["back"], MsgHelper.GetMessage("generic Cancel"), MsgHelper.GetMessage("generic Cancel"), "", StyleHelper.CancelButtonCssClass, true);
        else if (Request.QueryString["iframe"] == "true")
			litButtons.Text += m_refStyle.GetButtonEventsWCaption(AppUI.AppPath + "images/UI/Icons/cancel.png", "#", MsgHelper.GetMessage("generic Cancel"), MsgHelper.GetMessage("generic Cancel"), "onclick=\"parent.CancelIframe();\"", StyleHelper.CancelButtonCssClass, true);
        else
        {
			if (Request.QueryString["noworkarea"] != "1")
			{
				if (m_refApi.TreeModel == 1)
					litButtons.Text += m_refStyle.GetButtonEventsWCaption(AppUI.AppPath + "images/UI/Icons/back.png", "menutree.aspx?nid=" + MenuId + "&folderid=" + FolderId, MsgHelper.GetMessage("alt back button text"), MsgHelper.GetMessage("btn back"), "", StyleHelper.BackButtonCssClass, true);
				else
					litButtons.Text += m_refStyle.GetButtonEventsWCaption(AppUI.AppPath + "images/UI/Icons/back.png", "collections.aspx?action=ViewMenu&nId=" + MenuId + "&folderid=" + FolderId, MsgHelper.GetMessage("alt back button text"), MsgHelper.GetMessage("btn back"), "", StyleHelper.BackButtonCssClass, true);
			}
			else
			{
				addHelpDivider = false;
			}
        }

		if (addHelpDivider)
		{
			litHelp.Text = StyleHelper.ActionBarDivider + "<td>" + m_refStyle.GetHelpButton("AddMenuItem", "") + "</td>";
		}
		else
		{
			litHelp.Text = m_refStyle.GetHelpButton("AddMenuItem", "");
		}

        if (!string.IsNullOrEmpty(Request.QueryString["noworkarea"]) && Request.QueryString["noworkarea"].ToString() == "1")
        {
            //ItemType.Visible = true;
            ContentBlock.Visible = true;
        }
        subMenu.Text = MsgHelper.GetMessage("Sub Menu Label");
        Hyperlink.Text = MsgHelper.GetMessage("External Hyperlink label");
        ContentBlock.Text = MsgHelper.GetMessage("lbl new content block");
        Library.Text = MsgHelper.GetMessage("Library Asset label");
        content.Text = MsgHelper.GetMessage("lbl content item");
        if (!string.IsNullOrEmpty(Request.QueryString["back"]))
            frm_back.Value = Request.QueryString["back"].ToString();
    }
}
