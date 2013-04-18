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

public partial class Workarea_controls_collection_ViewAttributes : System.Web.UI.UserControl
{
    const Int64 ALL_CONTENT_LANGUAGES = -1;
    protected ContentAPI m_refContentApi = new ContentAPI();
    protected void Page_Load(object sender, EventArgs e)
    {
        EkMessageHelper MsgHelper = new EkMessageHelper(m_refContentApi.RequestInformationRef);
        StyleHelper m_refStyle = new StyleHelper();
        CommonApi m_refApi = new CommonApi();
        Collection gtNavs = new Collection();
        ApplicationAPI AppUI = new ApplicationAPI();
        long nId = Convert.ToInt64(Request.QueryString["nid"]);
        if (!string.IsNullOrEmpty(Request.QueryString["LangType"]))
        {
            Int32 ContentLanguage = Convert.ToInt32(Request.QueryString["LangType"]);
            if (ContentLanguage == ALL_CONTENT_LANGUAGES)
            {
                ContentLanguage = AppUI.DefaultContentLanguage;
            }
	        
	        AppUI.FilterByLanguage=ContentLanguage;
        }
        string ErrorString = "";
	    if (!string.IsNullOrEmpty(Request.QueryString["action"]))
        {
            string action = Request.QueryString["action"].ToString();
            if (action.ToString().ToLower() == "viewstageattributes")
            {
                gtNavs = AppUI.EkContentRef.GetEcmStageCollectionByID(nId, false, false, ref ErrorString, true, false, true);
            }
            else
            {
                gtNavs = AppUI.EkContentRef.GetEcmCollectionByID( nId, false, false, ref ErrorString, true, false, true);
            }
        }
	    string checkout = "";
        if (Convert.ToBoolean(gtNavs["ApprovalRequired"]) == true && ((gtNavs["Status"].ToString() == "A") || (gtNavs["Status"].ToString() == "S")))
	        checkout = "&checkout=true";
        if (Convert.ToBoolean(gtNavs["ApprovalRequired"]) == true && gtNavs["Status"].ToString() == "O")
	        checkout = checkout + "&status=o";
        string CollectionTitle = "";
	    if (ErrorString == "")
        {
		    if (gtNavs.Count > 0)
			    CollectionTitle = gtNavs["CollectionTitle"].ToString();
	    }
        if (ErrorString != "")
        {
            titlebarerror.InnerHtml = ErrorString;
        }
        litViewCollection.Text = m_refStyle.GetTitleBar(MsgHelper.GetMessage("view collection title"));
        litViewToolBarCollection.Text = m_refStyle.GetTitleBar(MsgHelper.GetMessage("view collection title")) + " " + CollectionTitle;

        long folderId = 0;
        if (!string.IsNullOrEmpty(Request.QueryString["folderId"]))
        {
            folderId = Convert.ToInt64(Request.QueryString["folderId"]);
        }

		if (Request.QueryString["bpage"] == "reports")
			litButtons.Text += m_refStyle.GetButtonEventsWCaption(AppUI.AppPath + "images/UI/Icons/back.png", "collections.aspx?action=ViewCollectionReport", MsgHelper.GetMessage("alt back button text"), MsgHelper.GetMessage("btn back"), "", StyleHelper.BackButtonCssClass, true);
		else
			litButtons.Text += m_refStyle.GetButtonEventsWCaption(AppUI.AppPath + "images/UI/Icons/back.png", "collections.aspx?action=View&nid=" + nId + "&folderid=" + folderId, MsgHelper.GetMessage("alt back button text"), MsgHelper.GetMessage("btn back"), "", StyleHelper.BackButtonCssClass, true);

		litButtons.Text += m_refStyle.GetButtonEventsWCaption(AppUI.AppPath + "images/UI/Icons/contentEdit.png", "collections.aspx?action=Edit&nid=" + nId + "&folderid=" + folderId + checkout, MsgHelper.GetMessage("alt: edit collection text"), MsgHelper.GetMessage("btn edit"), "", StyleHelper.EditButtonCssClass, true);
		
        if (m_refApi.EnableMultilingual == 1)
        {
            litLang.Text = StyleHelper.ActionBarDivider + "<td>" + MsgHelper.GetMessage("view language") + LangDD(false, "") + "</td>";
        }
        litTitle.Text = MsgHelper.GetMessage("generic title label");
		litHelp.Text = StyleHelper.ActionBarDivider + "<td>" + m_refStyle.GetHelpButton("ViewCollectionItems", "") + "</td>";
        litLabel.Text = MsgHelper.GetMessage("id label");
        litPath.Text = MsgHelper.GetMessage("lbl path") + ":";
        litTemplate.Text = MsgHelper.GetMessage("generic template label");
        litContentLUE.Text = MsgHelper.GetMessage("content LUE label");
        litLED.Text = MsgHelper.GetMessage("content LED label");
        litDC.Text = MsgHelper.GetMessage("content DC label");
        litDesc.Text = MsgHelper.GetMessage("description label");
        litStatus.Text = MsgHelper.GetMessage("lbl linkcheck status");
        litSubFolders.Text = MsgHelper.GetMessage("generic include subfolders msg");
        litApproval.Text = MsgHelper.GetMessage("lbl approval required");

        tdTitle.InnerHtml = gtNavs["CollectionTitle"].ToString();
        tdID.InnerHtml = gtNavs["CollectionID"].ToString();
        if (gtNavs.Contains("FolderPath") && !string.IsNullOrEmpty(gtNavs["FolderPath"].ToString())) {
            tdPath.InnerHtml = gtNavs["FolderPath"].ToString();
        }
        tdTemplate.InnerHtml = gtNavs["TemplatePath"].ToString();
        tdLUE.InnerHtml = gtNavs["EditorFName"] + " " + gtNavs["EditorLName"];
        tdLastEditDate.InnerHtml = gtNavs["DisplayLastEditDate"].ToString();
        tdDateCreated.InnerHtml = gtNavs["DisplayDateCreated"].ToString();
        tdDesc.InnerHtml = gtNavs["CollectionDescription"].ToString();
        tdStatus.InnerHtml = gtNavs["Status"].ToString();
        frm_recursive.Checked = (Convert.ToInt32(gtNavs["Recursive"]) == 1 ? true : false);
        approval.Checked = Convert.ToBoolean(gtNavs["ApprovalRequired"]);
	    gtNavs = null;
    }
    public string LangDD(object showAllOpt, string bgColor)
    {

        string myTemp = Request.ServerVariables["PATH_INFO"].Substring(Request.ServerVariables["PATH_INFO"].LastIndexOf("/") + 1) + "?" + Ektron.Cms.Common.EkFunctions.HtmlEncode(Request.ServerVariables["QUERY_STRING"].ToString().Replace("LangType", "L"));
        myTemp = myTemp.Replace("&amp;", "&");

        StyleHelper m_refStyle = new StyleHelper();
        string returnValue;
        returnValue = m_refStyle.ShowAllActiveLanguage(Convert.ToBoolean(showAllOpt), bgColor, "javascript:SelLanguage(this.value, '" + myTemp + "');", "");
        return returnValue;
    }
}
