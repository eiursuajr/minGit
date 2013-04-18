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

public partial class Workarea_controls_menu_pAddMenuItem : System.Web.UI.UserControl
{
    protected ContentAPI m_refContentApi = new ContentAPI();
    protected EkMessageHelper MsgHelper;
    protected Int32 ContentLanguage = 0;
    protected long FolderId = 0;
    protected long MenuId = 0;
    protected string FolderPath = "";
    protected string enableQDOparam = "";
    protected string noWorkAreaString = string.Empty;

    protected void Page_Load(object sender, EventArgs e)
    {
        MsgHelper = new EkMessageHelper(m_refContentApi.RequestInformationRef);
        StyleHelper m_refStyle = new StyleHelper();
        CommonApi m_refApi = new CommonApi();
        ApplicationAPI AppUI = new ApplicationAPI();
        if (Request.QueryString["LangType"] == "-1")
        {
            ContentLanguage = AppUI.DefaultContentLanguage;
            AppUI.SetCookieValue("LastValidLanguageID", ContentLanguage.ToString());
            AppUI.ContentLanguage = ContentLanguage;
        }
        else
        {
            AppUI.ContentLanguage = Convert.ToInt32(Request.QueryString["LangType"]);
            ContentLanguage = AppUI.ContentLanguage;
        }

        if (!string.IsNullOrEmpty(Request.QueryString["noworkarea"]) && Request.QueryString["noworkarea"] == "1")
        {
            noWorkAreaString = "&noworkarea=1";
        }
        long mpID = Convert.ToInt64(Request.QueryString["parentid"]);
	    long maID = Convert.ToInt64(Request.QueryString["ancestorid"]);
        
        string AncestorIDParam = "";
        string ParentIDParam = "";
        Collection gtNavs = new Collection();
	    if (string.IsNullOrEmpty(Request.QueryString["ancestorid"]))
	        AncestorIDParam = "&ancestorid=" + Request.QueryString["ancestorid"];
	     if (string.IsNullOrEmpty(Request.QueryString["parentid"]))
	        ParentIDParam = "&parentid=" + Request.QueryString["parentid"];

        FolderId = Convert.ToInt64(Request.QueryString["folderid"]);
        MenuId = Convert.ToInt64(Request.QueryString["nId"]);
        string ItemType = Request.Form["ItemType"];
        
        if (mpID != 0)
        {
            gtNavs = AppUI.EkContentRef.GetMenuByID(mpID, 0, false);
            if (gtNavs.Count > 0)
            {
                if (gtNavs.Contains("EnableReplication"))
                    enableQDOparam = "&qdo=1";
            }
        }
        switch (ItemType)
        {
            case "content":
                Response.Redirect("collections.aspx?action=AddLink&addto=Menu&folderid=" + FolderId + "&nid=" + MenuId + "&LangType=" + ContentLanguage + "&iframe=" + Request.QueryString["iframe"] + AncestorIDParam + ParentIDParam + "&back=" + Server.UrlEncode(Request.QueryString["back"]) + enableQDOparam + noWorkAreaString);
                break;
            case "newcontent":
                Response.Redirect("collectiontree.aspx?action=AddLink&addto=menu&noworkarea=1&nid=" + MenuId + "&folderid=" + FolderId + "&LangType=" + ContentLanguage);
                break;
            case "submenu":
                string enableReplicationFlag = "";
		        if ((gtNavs.Count > 0) && (gtNavs.Contains("EnableReplication")))
	                enableReplicationFlag = gtNavs["EnableReplication"].ToString();
                string strPath = "collections.aspx?action=AddSubMenu&folderid=" + FolderId + "&nId=" + MenuId + "&parentid=" + mpID + "&ancestorid=" + maID + "&LangType=" + ContentLanguage + "&iframe=" + Request.QueryString["iframe"] + "&back=" + Server.UrlEncode(Request.QueryString["back"]) + "&QD=" + enableReplicationFlag + noWorkAreaString;
                Response.Redirect(strPath);
                break;
            case "library":
                divLibrary.Visible = true;
                Collection gtFolderInfo = AppUI.EkContentRef.GetFolderInfoWithPath(FolderId);
                FolderPath = gtFolderInfo["Path"].ToString();
       	        if (FolderPath.Substring(FolderPath.Length - 1, 1) == "\\")
			        FolderPath = FolderPath.Remove(FolderPath.Length - 1, 1);
                FolderPath = FolderPath.Replace(@"\", @"\\");
                divLibrary.Visible = true;
                litTitle.Text = m_refStyle.GetTitleBar(MsgHelper.GetMessage("Add Menu Item Title"));
				litHelp.Text = StyleHelper.ActionBarDivider + "<td>" + m_refStyle.GetHelpButton("pAddMenuItem", "") + "</td>";
                if (!string.IsNullOrEmpty(Request.QueryString["back"]))
                    litButtons.Text = m_refStyle.GetButtonEventsWCaption(AppUI.AppPath + "images/UI/Icons/cancel.png", Request.QueryString["back"], MsgHelper.GetMessage("generic Cancel"), MsgHelper.GetMessage("generic Cancel"), "", StyleHelper.CancelButtonCssClass, true);
                else if (Request.QueryString["iframe"] == "true")
					litButtons.Text = m_refStyle.GetButtonEventsWCaption(AppUI.AppPath + "images/UI/Icons/cancel.png", "#", MsgHelper.GetMessage("generic Cancel"), MsgHelper.GetMessage("generic Cancel"), "onclick=\"parent.CancelIframe();\"", StyleHelper.CancelButtonCssClass, true);
                else
					litButtons.Text = m_refStyle.GetButtonEventsWCaption(AppUI.AppPath + "images/UI/Icons/cancel.png", "#", MsgHelper.GetMessage("generic Cancel"), MsgHelper.GetMessage("generic Cancel"), "onclick=\"top.close();\"", StyleHelper.CancelButtonCssClass, true);
				litButtons.Text += m_refStyle.GetButtonEventsWCaption(AppUI.AppPath + "images/UI/Icons/save.png", "#", MsgHelper.GetMessage("alt Save Menu Item"), MsgHelper.GetMessage("btn save"), "onclick=\"return SubmitForm(\'AddMenuItem\', \'VerifyLibraryAssest()\');\"", StyleHelper.SaveButtonCssClass, true);
			break;
            default:
            divOther.Visible = true;
			litHelp1.Text = StyleHelper.ActionBarDivider + "<td>" + m_refStyle.GetHelpButton("pAddMenuItem", "") + "</td>";
            litTitle1.Text = m_refStyle.GetTitleBar(MsgHelper.GetMessage("Add Menu Item Title"));

			if (!string.IsNullOrEmpty(Request.QueryString["back"]))
				litButtons1.Text = m_refStyle.GetButtonEventsWCaption(AppUI.AppPath + "images/UI/Icons/cancel.png", Request.QueryString["back"], MsgHelper.GetMessage("generic Cancel"), MsgHelper.GetMessage("generic Cancel"), "", StyleHelper.CancelButtonCssClass, true);
            else if (Request.QueryString["iframe"] == "true")
				litButtons1.Text = m_refStyle.GetButtonEventsWCaption(AppUI.AppPath + "images/UI/Icons/cancel.png", "#", MsgHelper.GetMessage("generic Cancel"), MsgHelper.GetMessage("generic Cancel"), "onclick=\"parent.CancelIframe();\"", StyleHelper.CancelButtonCssClass, true);
            else
				litButtons1.Text = m_refStyle.GetButtonEventsWCaption(AppUI.AppPath + "images/UI/Icons/cancel.png", "#", MsgHelper.GetMessage("generic Cancel"), MsgHelper.GetMessage("generic Cancel"), "onclick=\"top.close();\"", StyleHelper.CancelButtonCssClass, true);
            litButtons1.Text += m_refStyle.GetButtonEventsWCaption(AppUI.AppPath + "images/UI/Icons/save.png", "#", MsgHelper.GetMessage("alt Save Menu Item"), MsgHelper.GetMessage("btn save"), "onclick=\"return SubmitForm(\'AddMenuItem\', \'VerifyAddMenuItem()\');\"", StyleHelper.SaveButtonCssClass, true);
            break;
        }
    }
}
