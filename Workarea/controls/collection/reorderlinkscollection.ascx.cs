using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Ektron.Cms.UI.CommonUI;
using Microsoft.VisualBasic;
using Ektron.Cms.Common;
using Ektron.Cms;

public partial class Workarea_controls_collection_reorderlinkscollection : System.Web.UI.UserControl
{
    const Int64 ALL_CONTENT_LANGUAGES = -1;
    protected Int32 ContentLanguage;
    protected string checkout = "";
    protected string bAction = "";
    protected long nId = 0;
    protected string reOrderList = "";
    protected ContentAPI m_refContentApi = new ContentAPI();
    protected void Page_Load(object sender, EventArgs e)
    {
        Collection gtLink = new Collection();
        EkMessageHelper MsgHelper = new EkMessageHelper(m_refContentApi.RequestInformationRef);
        StyleHelper m_refStyle = new StyleHelper();
	    nId = Convert.ToInt64(Request.QueryString["nid"]);
        ApplicationAPI AppUI = new ApplicationAPI();
        ContentLanguage = Convert.ToInt32(Request.QueryString["LangType"]);
        if (ContentLanguage == ALL_CONTENT_LANGUAGES)
        {
            ContentLanguage = AppUI.DefaultContentLanguage;
        }
        
        AppUI.FilterByLanguage=ContentLanguage;
	    
        if (!string.IsNullOrEmpty(Request.QueryString["checkout"]))
            checkout = "&checkout=" + Request.QueryString["checkout"].ToString();
        Collection gtLinks = new Collection();
        string ErrorString = "";
        if (checkout != "")
    	    gtLinks = AppUI.EkContentRef.GetEcmStageCollectionByID(nId, false, false, ref ErrorString, true, false, true);
        else
        {
	        if (string.IsNullOrEmpty(Request.QueryString["status"]) && Request.QueryString["status"] == "o")
    	        gtLinks = AppUI.EkContentRef.GetEcmStageCollectionByID(nId, false, false, ref ErrorString,true, false, true);
	        else
    	        gtLinks = AppUI.EkContentRef.GetEcmCollectionByID(nId, false, false, ref ErrorString,true, false, true);
        }
        if (!string.IsNullOrEmpty(Request.QueryString["status"]))
        {
            if (Request.QueryString["status"].ToString().ToLower() == "o")
                bAction = "&status=o";
        }
        else
        {
            if (Convert.ToBoolean(gtLinks["ApprovalRequired"]) == true && gtLinks["Status"].ToString().ToUpper() == "O")
                bAction = "&status=o";
        }
        string CollectionTitle = "";
	    if (ErrorString == "")
        {
		    if (gtLinks.Count > 0)
            {
			    CollectionTitle = gtLinks["CollectionTitle"].ToString();
			    gtLink = (Collection)gtLinks["Contents"];
		    }
        }
        OrderList.Size =(gtLinks.Count < 20 ? gtLinks.Count : 20);
        reOrderList = "";
        foreach (Collection gtNav in gtLink)
        {
            if (reOrderList.Length > 0)
                reOrderList = reOrderList + "," + gtNav["ContentID"] + "|" + gtNav["ContentLanguage"];			   
            else
                reOrderList = gtNav["ContentID"] + "|" + gtNav["ContentLanguage"];
            OrderList.Items.Add(new ListItem(Server.HtmlDecode(gtNav["ContentTitle"].ToString()), gtNav["ContentID"] + "|" + gtNav["ContentLanguage"]));				  
        }
        if (gtLinks.Count > 0)
            OrderList.SelectedIndex = 0;
        long folderId = 0;
        if (string.IsNullOrEmpty(Request.QueryString["folderId"]))
        {
            folderId = Convert.ToInt64(Request.QueryString["folderId"]);
        }
	    reOrderList = "";
	    if (ErrorString != "")
        {
            divError.InnerHtml = ErrorString;
            divErrors.Visible = true;
        }
        litReOrder.Text = m_refStyle.GetTitleBar(MsgHelper.GetMessage("reorder collection title"));
        litReOrderTitle.Text = m_refStyle.GetTitleBar(MsgHelper.GetMessage("reorder collection title") + " \"" + CollectionTitle + "\"");

		litButtons.Text = m_refStyle.GetButtonEventsWCaption(AppUI.AppPath + "images/UI/Icons/back.png", "collections.aspx?Action=View&nid=" + nId + "&folderid=" + folderId, MsgHelper.GetMessage("alt back button text"), MsgHelper.GetMessage("btn back"), "", StyleHelper.BackButtonCssClass, true);
		litButtons.Text += m_refStyle.GetButtonEventsWCaption(AppUI.AppPath + "images/UI/Icons/save.png", "#", MsgHelper.GetMessage("alt: update collection order text"), MsgHelper.GetMessage("btn update"), "onclick=\"return SubmitForm(\'link_order\', \'\');\"", StyleHelper.SaveButtonCssClass, true);
        
        litHelp.Text = StyleHelper.ActionBarDivider + "<td>" + m_refStyle.GetHelpButton("ReOrderLinks", "") + "</td>";
        
        frm_folder_id.Value = folderId.ToString();
        Up.Src = AppUI.AppPath + "images/UI/Icons/arrowHeadUp.png";
        Up.Alt = MsgHelper.GetMessage("move selection up msg");
        Down.Src = AppUI.AppPath + "images/UI/Icons/arrowHeadDown.png";
        Down.Alt = MsgHelper.GetMessage("move selection down msg");
    }
}
