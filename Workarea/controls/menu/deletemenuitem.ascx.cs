using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Microsoft.VisualBasic;
using Ektron.Cms;
using Ektron.Cms.Common;
using Ektron.Cms.UI.CommonUI;
using System.Data;

public partial class Workarea_controls_menu_deletemenuitem : System.Web.UI.UserControl
{
    protected ContentAPI m_refContentApi = new ContentAPI();
    protected string cLinkArray = "";
    protected string fLinkArray = "";
    protected void Page_Load(object sender, EventArgs e)
    {
        EkMessageHelper MsgHelper = new EkMessageHelper(m_refContentApi.RequestInformationRef);
        StyleHelper m_refStyle = new StyleHelper();
        CommonApi m_refApi = new CommonApi();
        ApplicationAPI AppUI = new ApplicationAPI();
        String MenuTitle = String.Empty;
        long nId = Convert.ToInt64(Request.QueryString["nid"]);
        Collection gtLinks = AppUI.EkContentRef.GetMenuByID(nId, 0, false);

        if (gtLinks.Count > 0)
        {
            MenuTitle = gtLinks["MenuTitle"].ToString();
            gtLinks = (Collection)gtLinks["Items"];
        }
        string action = string.Empty;
        long folderId = 0;
        if (string.IsNullOrEmpty(Request.QueryString["action"]))
            action = Request.QueryString["action"].ToString();
        if (string.IsNullOrEmpty(Request.QueryString["folderId"]))
            folderId = Convert.ToInt64(Request.QueryString["folderId"]);
        litHelp.Text = StyleHelper.ActionBarDivider + "<td>" + m_refStyle.GetHelpButton(action, "") + "</td>";
        litTitle.Text = m_refStyle.GetTitleBar(MsgHelper.GetMessage("delete menu items title") + " \"" + MenuTitle + "\"");

		if (m_refApi.TreeModel == 1)
			litButtons.Text += m_refStyle.GetButtonEventsWCaption(AppUI.AppPath + "images/UI/Icons/back.png", "menutree.aspx?nid=" + nId + "&folderid=" + folderId, MsgHelper.GetMessage("alt back button text"), MsgHelper.GetMessage("btn back"), "", StyleHelper.BackButtonCssClass, true);
		else
			litButtons.Text += m_refStyle.GetButtonEventsWCaption(AppUI.AppPath + "images/UI/Icons/back.png", "collections.aspx?Action=ViewMenu&nid=" + nId + "&folderid=" + folderId, MsgHelper.GetMessage("alt back button text"), MsgHelper.GetMessage("btn back"), "", StyleHelper.BackButtonCssClass, true);

		litButtons.Text = m_refStyle.GetButtonEventsWCaption(AppUI.AppPath + "images/UI/Icons/delete.png", "#", MsgHelper.GetMessage("alt: delete menu items text"), MsgHelper.GetMessage("btn delete"), "onclick=\"return SubmitForm(\'selections\', \'GetIDs()\');\"", StyleHelper.DeleteButtonCssClass, true);
        
         Int32 lLoop = 0;
		 
		 string cLanguagesArray="";
         DataTable dtItems = new DataTable();
        dtItems.Columns.Add("ItemTitle");
        dtItems.Columns.Add("ItemID");
        dtItems.Columns.Add("ItemLink");
        dtItems.Columns.Add("ItemType");
        dtItems.Columns.Add("ID");
        dtItems.Columns.Add("ContentLanguage");
        dtItems.Columns.Add("lLoop");
        foreach (Collection gtLink in gtLinks)
        {
            DataRow dRow = dtItems.NewRow();
            dRow["ItemTitle"] = gtLink["ItemTitle"].ToString();
            dRow["ItemID"] = gtLink["ItemID"].ToString();
            dRow["ItemLink"] = gtLink["ItemLink"].ToString();
            dRow["ItemType"] = gtLink["ItemType"].ToString();
            dRow["ID"] = gtLink["ID"].ToString();
            dRow["ContentLanguage"] = gtLink["ContentLanguage"].ToString();
            dRow["lLoop"] = lLoop.ToString();
            dtItems.Rows.Add(dRow);
            cLinkArray = cLinkArray + "," + gtLink["ID"] + "_" + gtLink["ItemType"];
			cLanguagesArray=cLanguagesArray + "," + gtLink["ContentLanguage"] + "_" + gtLink["ItemType"];
			fLinkArray = fLinkArray + "," + folderId;
			lLoop++;
        }
        rptItems.DataSource = dtItems;
        rptItems.DataBind();
        if (cLinkArray.Length > 0)
        {
            cLinkArray = cLinkArray.Remove(0, 1);
            fLinkArray = fLinkArray.Remove(0, 1);
            cLanguagesArray = cLanguagesArray.Remove(0, 1);
		}
        CollectionID.Value = nId.ToString();
        form1.Action = "collections.aspx?Action=doDeleteMenuItem&folderid=" + folderId + "&nid=" + nId;
        litURL.Text = MsgHelper.GetMessage("generic URL Link");
        litID.Text = MsgHelper.GetMessage("generic ID");
        litGenericTitle.Text = MsgHelper.GetMessage("generic Title");
        aSelect.InnerHtml = MsgHelper.GetMessage("generic select all msg");
        aClear.InnerHtml = MsgHelper.GetMessage("generic clear all msg");
    }
}
