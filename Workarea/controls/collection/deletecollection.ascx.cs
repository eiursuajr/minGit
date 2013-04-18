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
using System.Data;

public partial class Workarea_controls_collection_deletecollection : System.Web.UI.UserControl
{
    const Int64 ALL_CONTENT_LANGUAGES = -1;
    protected ContentAPI m_refContentApi = new ContentAPI();
    protected string cLinkArray = "";
    protected string fLinkArray = "";
    protected long folderId = 0;
    protected long nId = 0;
    protected void Page_Load(object sender, EventArgs e)
    {
        EkMessageHelper MsgHelper = new EkMessageHelper(m_refContentApi.RequestInformationRef);
        StyleHelper m_refStyle = new StyleHelper();
        ApplicationAPI AppUI = new ApplicationAPI();
        nId = Convert.ToInt64(Request.QueryString["nid"]);
        CollectionID.Value = nId.ToString();
        Collection gtLinks = new Collection();
        string ErrorString = "";
        string CollectionTitle = "";
	    if (!string.IsNullOrEmpty(Request.QueryString["LangType"]))
        {
            Int32 ContentLanguage = Convert.ToInt32(Request.QueryString["LangType"]);
            if (ContentLanguage == ALL_CONTENT_LANGUAGES)
            {
                ContentLanguage = AppUI.DefaultContentLanguage;
            }
	        
	        AppUI.FilterByLanguage=ContentLanguage;
        }
        string checkout = "";
        if (!string.IsNullOrEmpty(Request.QueryString["checkout"]))
            checkout = Request.QueryString["checkout"].ToString();

        if (checkout == "true" || Request.QueryString["status"] == "o")
    	    gtLinks = AppUI.EkContentRef.GetEcmStageCollectionByID( nId, false, false, ref ErrorString, true, false, true);
        else
    	    gtLinks = AppUI.EkContentRef.GetEcmCollectionByID( nId, false, false, ref ErrorString, true, false, true);
	    if (ErrorString == "")
        {
		    if (gtLinks.Count > 0)
            {
			    CollectionTitle = gtLinks["CollectionTitle"].ToString();
			    gtLinks = (Collection)gtLinks["Contents"];
		    }
            }
	    if (ErrorString != "")
        {
            divError.Visible = true;
        }
        if (string.IsNullOrEmpty(Request.QueryString["folderId"]))
        {
            folderId = Convert.ToInt64(Request.QueryString["folderId"]);
        }
        divTitle.InnerHtml = ErrorString;
        DeleteCollectionError.InnerHtml = m_refStyle.GetTitleBar(MsgHelper.GetMessage("delete collection items title"));
        DeleteCollection.InnerHtml = m_refStyle.GetTitleBar(MsgHelper.GetMessage("delete collection items title")) + " " + CollectionTitle;
        litHelp.Text = StyleHelper.ActionBarDivider + "<td>" + m_refStyle.GetHelpButton("deletecollectionitems", "") + "</td>";
		litButtons.Text = m_refStyle.GetButtonEventsWCaption(AppUI.AppPath + "images/UI/Icons/back.png", "collections.aspx?Action=View&nid=" + nId + "&folderid=" + folderId, MsgHelper.GetMessage("alt back button text"), MsgHelper.GetMessage("btn back"), "", StyleHelper.BackButtonCssClass, true);
		litButtons.Text += m_refStyle.GetButtonEventsWCaption(AppUI.AppPath + "images/UI/Icons/delete.png", "#", MsgHelper.GetMessage("alt: delete collection items text"), MsgHelper.GetMessage("btn delete"), "onclick=\"return SubmitForm(\'selections\', \'GetIDs()\');\"", StyleHelper.DeleteButtonCssClass, true);
        litGenericTitle.Text = MsgHelper.GetMessage("generic Title");
        litGenericID.Text = MsgHelper.GetMessage("generic ID");
        litGenericURL.Text = MsgHelper.GetMessage("generic URL Link");

        Int32 lLoop = 0;
		string cLanguagesArray = "";
        DataTable dtItems = new DataTable();
        dtItems.Columns.Add("lLoop");
        dtItems.Columns.Add("ContentLanguage");
        dtItems.Columns.Add("ContentID");
        dtItems.Columns.Add("ContentLinks");
        dtItems.Columns.Add("ContentIcon");
        foreach (Collection gtLink in gtLinks)
        {
            DataRow dRow = dtItems.NewRow();
            dRow["lLoop"] = lLoop;
            dRow["ContentLanguage"] = gtLink["ContentLanguage"].ToString();
            dRow["ContentID"] = gtLink["ContentID"].ToString();
            dRow["ContentLinks"] = gtLink["ContentLinks"].ToString();
            dRow["ContentIcon"] = getContentTypeIconAspx(Convert.ToInt32(gtLink["ContentType"]), gtLink) + gtLink["ContentTitle"].ToString();
            dtItems.Rows.Add(dRow);

            cLinkArray = cLinkArray + "," + gtLink["ContentID"];
			cLanguagesArray=cLanguagesArray + "," + gtLink["ContentLanguage"];
			fLinkArray = fLinkArray + "," + folderId;
			lLoop = lLoop + 1;
        }
        rptItems.DataSource = dtItems;
        rptItems.DataBind();

        if (cLinkArray.Length > 0)
        {
            cLinkArray = cLinkArray.Remove(0, 1);
            fLinkArray = fLinkArray.Remove(0, 1);
            cLanguagesArray = cLanguagesArray.Remove(0, 1);
        }
    }
    public string getContentTypeIconAspx(int ContentTypeID, Collection colContent)
    {
        ApplicationAPI AppUI = new ApplicationAPI();
        string contentIcon = "<img src=\"" + AppUI.AppPath + "images/UI/Icons/contentHtml.png\" alt=\"Content\">";
        string pageIcon = "<img src=\"" + AppUI.AppPath + "images/UI/Icons/layout.png\" alt=\"Content\">";
        string formsIcon = "<img src=\"" + AppUI.AppPath + "images/UI/Icons/contentForm.png\" alt=\"Content\">";
        string menuIcon = "<img src=\"" + AppUI.AppPath + "images/UI/Icons/menu.png\" alt=\"Content\">";
        string libraryIcon = "<img src=\"" + AppUI.AppPath + "images/UI/Icons/book.png\" alt=\"Content\">";
        string linkIcon = "<img src=\"" + AppUI.AppPath + "images/UI/Icons/link.png\" alt=\"Content\">";
        string returnValue;
        if (ContentTypeID == 2)
        {
            returnValue = formsIcon + "&nbsp;";
        }
        else if (ContentTypeID == 1)
        {
            if (Ektron.Cms.Common.EkFunctions.DoesKeyExist(colContent, "ContentSubType") && Convert.ToInt32(colContent["ContentSubType"]) == 1)
            {
                returnValue = pageIcon + "&nbsp;";
            }
            else
            {
                returnValue = contentIcon + "&nbsp;";
            }
        }
        else if (Ektron.Cms.Common.EkConstants.IsAssetContentType(ContentTypeID, false))
        {
            if (colContent["ImageUrl"].ToString() != "")
            {
                returnValue = "<img src=\"" + colContent["ImageUrl"] + "\"  alt=\"Content\">&nbsp;";
            }
            else
            {
                returnValue = libraryIcon + "&nbsp;";
            }
        }
        else
        {
            returnValue = contentIcon + "&nbsp;";
        }
        return returnValue;
    }
}
