using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Ektron.Cms.UI.CommonUI;
using Microsoft.VisualBasic;
using Ektron.Cms;
using Ektron.Cms.Common;

public partial class Workarea_controls_menu_reordermenuitems : System.Web.UI.UserControl
{
    protected ContentAPI m_refContentApi = new ContentAPI();
    protected long nId = 0;
    protected string reOrderList = "";
    protected void Page_Load(object sender, EventArgs e)
    {
        EkMessageHelper MsgHelper = new EkMessageHelper(m_refContentApi.RequestInformationRef);
        StyleHelper m_refStyle = new StyleHelper();
        CommonApi m_refApi = new CommonApi();
        
        string MenuTitle = "";
        
        ApplicationAPI AppUI = new ApplicationAPI();
        nId = Convert.ToInt64(Request.QueryString["nid"]);
        Collection gtLinks = AppUI.EkContentRef.GetMenuByID(nId, 0, false);
        if (gtLinks.Count > 0)
        {
            MenuTitle = gtLinks["MenuTitle"].ToString();
            gtLinks = (Collection)gtLinks["Items"];
        }
        long folderId = 0;
        if (!string.IsNullOrEmpty(Request.QueryString["folderId"]))
            folderId = Convert.ToInt64(Request.QueryString["folderId"]);
       
        string action = "";
        if (!string.IsNullOrEmpty(Request.QueryString["action"]))
            action = Request.QueryString["action"].ToString();
		litHelp.Text = StyleHelper.ActionBarDivider + "<td>" + m_refStyle.GetHelpButton(action, "") + "</td>";
        litTitle.Text = m_refStyle.GetTitleBar(MsgHelper.GetMessage("reorder menu item title") + " \"" + MenuTitle + "\"");
        
		if (string.IsNullOrEmpty(Request.QueryString["back"]))
           litButtons.Text = m_refStyle.GetButtonEventsWCaption(AppUI.AppPath + "images/UI/Icons/cancel.png", Request.QueryString["back"], MsgHelper.GetMessage("generic Cancel"), MsgHelper.GetMessage("generic Cancel"), "", StyleHelper.CancelButtonCssClass, true);
        else if (Request.QueryString["iframe"] == "true")
			litButtons.Text = m_refStyle.GetButtonEventsWCaption(AppUI.AppPath + "images/UI/Icons/cancel.png", "#", MsgHelper.GetMessage("generic Cancel"), MsgHelper.GetMessage("generic Cancel"), "onclick=\"parent.CancelIframe();\"", StyleHelper.CancelButtonCssClass, true);
        else
        {
            if (m_refApi.TreeModel == 1)
            {
                string treeViewIdParam = "";
                if (!string.IsNullOrEmpty(Request.QueryString["treeViewId"]))
                    treeViewIdParam = "&treeViewId=" + Request.QueryString["treeViewId"].ToString();
				litButtons.Text = m_refStyle.GetButtonEventsWCaption(AppUI.AppPath + "images/UI/Icons/back.png", "menu.aspx?Action=viewcontent&menuid=" + nId + "&folderid=" + folderId + treeViewIdParam, MsgHelper.GetMessage("alt back button text"), MsgHelper.GetMessage("btn back"), "", StyleHelper.BackButtonCssClass, true);
            }
            else
				litButtons.Text = m_refStyle.GetButtonEventsWCaption(AppUI.AppPath + "images/UI/Icons/back.png", "collections.aspx?Action=ViewMenu&nid=" + nId + "&folderid=" + folderId, MsgHelper.GetMessage("alt back button text"), MsgHelper.GetMessage("btn back"), "", StyleHelper.BackButtonCssClass, true);
        }

		litButtons.Text += m_refStyle.GetButtonEventsWCaption(AppUI.AppPath + "images/UI/Icons/save.png", "#", MsgHelper.GetMessage("alt: update menu order text"), MsgHelper.GetMessage("btn update"), "onclick=\"return SubmitForm(\'link_order\', \'true\');\"", StyleHelper.SaveButtonCssClass, true);

        if (gtLinks.Count < 20) 
            OrderList.Size = gtLinks.Count;
        else
            OrderList.Size = 20;
        reOrderList = "";
        
        foreach (Collection gtNav in gtLinks)
        {
            if (reOrderList.Length > 0)
				reOrderList = reOrderList + "," + gtNav["ID"];
			else
				reOrderList = gtNav["ID"].ToString();

            if (Char.IsNumber(gtNav["ID"].ToString(), 0))
            {
                OrderList.Items.Add(new ListItem(gtNav["ItemTitle"].ToString(), gtNav["ID"].ToString()));
            }
        }
        if (gtLinks.Count > 0)
            OrderList.SelectedIndex = 0;
        frmfolderid.Value = folderId.ToString();
        UP.Src = AppUI.AppPath + "images/UI/Icons/arrowHeadUp.png";
        UP.Alt = MsgHelper.GetMessage("move selection up msg");
        DOWN.Src = AppUI.AppPath + "images/UI/Icons/arrowHeadDown.png";
        DOWN.Alt = MsgHelper.GetMessage("move selection down msg");
    }
}
