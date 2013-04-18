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
using System.Data;

public partial class Workarea_controls_collection_viewothercollection : System.Web.UI.UserControl
{
    protected ContentAPI m_refContentApi = new ContentAPI();
    protected void Page_Load(object sender, EventArgs e)
    {
        EkMessageHelper MsgHelper = new EkMessageHelper(m_refContentApi.RequestInformationRef);
        StyleHelper m_refStyle = new StyleHelper();
        CommonApi m_refApi = new CommonApi();
        Collection gtNavs = null;
        Collection gtNav = null;
        string ErrorString = "";
        string FolderName = "";
        ApplicationAPI AppUI = new ApplicationAPI();
        long folderId = 0;
        string action = "";
        if (!string.IsNullOrEmpty(Request.QueryString["folderId"]))
        {
            folderId = Convert.ToInt64(Request.QueryString["folderId"]);
        }
        if (!string.IsNullOrEmpty(Request.QueryString["action"]))
        {
            action = Request.QueryString["action"].ToString();
        }
        string OrderBy = Request.QueryString["OrderBy"];
        if (OrderBy == "")
            OrderBy = "title";
        gtNavs = AppUI.EkContentRef.GetAllCollectionsInfo(folderId, "title");
        if (ErrorString.Length  == 0)
        {
            gtNav = AppUI.EkContentRef.GetFolderInfov2_0(folderId);
            if (ErrorString.Length == 0)
                FolderName = gtNav["FolderName"].ToString();
        }
        if (ErrorString != "")
        {
            divError.Visible = true;
           
        }
        titlebarerror.InnerHtml = ErrorString;
        litErrorButtons.Text = m_refStyle.GetButtonEventsWCaption(AppUI.AppPath + "images/UI/Icons/back.png", "collections.aspx", MsgHelper.GetMessage("alt back button text"), MsgHelper.GetMessage("btn back"), "", StyleHelper.BackButtonCssClass, true);
        litErrorHelp.Text = StyleHelper.ActionBarDivider + "<td>" + m_refStyle.GetHelpButton(action, "") + "<td>";
        litCollectionTitle.Text = m_refStyle.GetTitleBar(MsgHelper.GetMessage("view collections title")) + " " + FolderName;
        litErrorViewCollectionTitle.Text = m_refStyle.GetTitleBar(MsgHelper.GetMessage("view collections title"));
        litGenericError.Text = MsgHelper.GetMessage("generic page error message");
		litButtons.Text = m_refStyle.GetButtonEventsWCaption(AppUI.AppPath + "images/UI/Icons/back.png", "content.aspx?Action=ViewContentByCategory&id=" + folderId, MsgHelper.GetMessage("alt back button text"), MsgHelper.GetMessage("btn back"), "", StyleHelper.BackButtonCssClass, true);
		litButtons.Text += m_refStyle.GetButtonEventsWCaption(AppUI.AppPath + "images/UI/Icons/add.png", "collections.aspx?action=Add&folderid=" + folderId, MsgHelper.GetMessage("alt: add new collection text"), MsgHelper.GetMessage("btn add"), "", StyleHelper.AddButtonCssClass, true);
		litHelp.Text = StyleHelper.ActionBarDivider + "<td>" + m_refStyle.GetHelpButton("view_collections_in_folder", "") + "<td>";


        if (gtNavs.Count > 0)
        {
            DataTable dtItems = new DataTable();
            dtItems.Columns.Add("CollectionID");
            dtItems.Columns.Add("DisplayLastEditDate");
            dtItems.Columns.Add("CollectionTemplate");
            dtItems.Columns.Add("CollectionTitle");
            dtItems.Columns.Add("CollectionLink");

            foreach (Collection gtNa in gtNavs)
            {
                string colAction = ""; 
	            if (Convert.ToBoolean(gtNa["ApprovalRequired"]) && gtNa["Status"].ToString().ToUpper() != "A")
	                colAction = "&action=ViewStage";
	            else
	                colAction = "&action=View";
                DataRow dRow = dtItems.NewRow();
                dRow["CollectionID"] = gtNa["CollectionID"].ToString();
                dRow["DisplayLastEditDate"] = gtNa["DisplayLastEditDate"].ToString();
                dRow["CollectionTemplate"] = gtNa["CollectionTemplate"].ToString();
                dRow["CollectionTitle"] = gtNa["CollectionTitle"].ToString();
                dRow["CollectionLink"] = "collections.aspx?folderid=" + folderId + colAction + "&nid=" + gtNa["CollectionID"].ToString();
                dtItems.Rows.Add(dRow);
            }
            rptInfo.DataSource = dtItems;
            rptInfo.DataBind();
        }
        gtNavs = null;
        aGenericTitle.HRef = "collections.aspx?folderid=" + folderId + "&OrderBy=navname";
        aGenericTitle.InnerHtml = MsgHelper.GetMessage("generic Title");
        aGenericId.HRef = "collections.aspx?folderid=" + folderId + "&OrderBy=CollectionID";
        aGenericId.InnerHtml = MsgHelper.GetMessage("generic ID");
        aGenericLast.HRef = "collections.aspx?folderid=" + folderId + "&OrderBy=date";
        aGenericLast.InnerHtml = MsgHelper.GetMessage("generic Date Modified");
        aGenericURL.HRef = "collections.aspx?folderid=" + folderId + "&OrderBy=CollectionTemplate";
        aGenericURL.InnerHtml = MsgHelper.GetMessage("generic URL Link");

    }
}
