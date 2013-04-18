using System.Web.UI.WebControls;
using System.Configuration;
using System.Collections;
using System.Linq;
using System.Data;
using System.Web.Caching;
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
using Ektron.Cms.Content;
using Ektron.Cms.Common;


public partial class ViewApprovalList : System.Web.UI.UserControl
{


    #region Members

    private CommonApi _CommonApi = new CommonApi();
    private EkContent _EkContent;
    private EkMessageHelper _MessageHelper;
    private string _Folder = "";
    private Collection _ApprovalsCollection;
    private ArrayList _ApprovedList;
    protected StyleHelper _StyleHelper = new StyleHelper();
    protected int _EnableMultilingual = 0;
    protected int _ContentLanguage = -1;
    protected AssetInfoData[] _AssetInfoData;
    protected ContentAPI _ContentApi = new ContentAPI();
    protected int _ContentType = 0;
    protected string _pageLocation = "";

    #endregion

    #region Properties

    public int MultilingualEnabled
    {
        get
        {
            return _EnableMultilingual;
        }
        set
        {
            _EnableMultilingual = value;
        }
    }

    public int ContentLang
    {
        get
        {
            return _ContentLanguage;
        }
        set
        {
            _ContentLanguage = value;
        }
    }

    #endregion

    #region Events

    public ViewApprovalList()
    {

        _CommonApi = new CommonApi();
        _ApprovedList = new ArrayList();

    }

    private void Page_Init(System.Object sender, System.EventArgs e)
    {
        _CommonApi.ContentLanguage = _ContentLanguage;
        _EkContent = _CommonApi.EkContentRef;
        _MessageHelper = _CommonApi.EkMsgRef;

        //register js/css
        RegisterResources();

        _Folder = (string)((string.IsNullOrEmpty(Request.QueryString["id"])) ? string.Empty : (Request.QueryString["id"]));
        this.litAction.Text = Request.QueryString["action"];

        if (Request.QueryString["location"] != null && !string.IsNullOrEmpty(Request.QueryString["location"]))
        {
            _pageLocation = Request.QueryString["location"];
        }
        else
        {
            _pageLocation = "workarea";
        }

        //set hidden input field to submit button unique id - used in js submit (fires off lbSubmit_Click event)
        lbSubmitId.Value = this.lbSubmit.UniqueID;
    }

    private void Page_Load(System.Object sender, System.EventArgs e)
    {

        string ContentTypeUrlParam = Ektron.Cms.Common.EkConstants.ContentTypeUrlParam;
        if (!(Request.QueryString[ContentTypeUrlParam] == null) && (Request.QueryString[ContentTypeUrlParam] != ""))
        {
            if (Information.IsNumeric(Request.QueryString[ContentTypeUrlParam]))
            {
                _ContentType = Convert.ToInt32(Request.QueryString[ContentTypeUrlParam]);
                _ContentApi.SetCookieValue(ContentTypeUrlParam, _ContentType.ToString());
            }
        }
        else if (Ektron.Cms.CommonApi.GetEcmCookie()[ContentTypeUrlParam] != "")
        {
            if (Information.IsNumeric(Ektron.Cms.CommonApi.GetEcmCookie()[ContentTypeUrlParam]))
            {
                _ContentType = System.Convert.ToInt32(Ektron.Cms.CommonApi.GetEcmCookie()[ContentTypeUrlParam]);
            }
        }

        if (Page.IsPostBack)
        {
           
            GetApprovedItems();

            //lbSubmit click event not firing for some reason - sniff out postback
            //and if it's from lbSubmit, manually fire click event
            if (Request.Form["__EVENTTARGET"] == this.lbSubmit.UniqueID)
            {
                lbSubmit_Click(sender, e);
            }
        }

        //deserialize approved items
        GetSavedApprovedItems();
        ViewApprovalList_Renamed();

        //set up datagrid
        this.dgItemsNeedingApproval.PageSize = _CommonApi.RequestInformationRef.PagingSize;
        this.dgItemsNeedingApproval.DataSource = _ApprovalsCollection;
        this.dgItemsNeedingApproval.CurrentPageIndex = this.ucPaging.SelectedPage;
        this.dgItemsNeedingApproval.DataBind();
        if (this.dgItemsNeedingApproval.Items.Count >= 1)
        {
            this.hdnNeedingApproval.Value = this.dgItemsNeedingApproval.Items.Count.ToString();
        }
        else
        {
            this.hdnNeedingApproval.Value = "0";
        }
        if (this.dgItemsNeedingApproval.PageCount > 1)
        {
            this.ucPaging.TotalPages = this.dgItemsNeedingApproval.PageCount;
            this.ucPaging.CurrentPageIndex = this.dgItemsNeedingApproval.CurrentPageIndex;
        }
        else
        {
            this.ucPaging.Visible = false;
        }
    }

    public void dgItemsNeedingApproval_ItemDataBound(System.Object sender, System.Web.UI.WebControls.DataGridItemEventArgs e)
    {
        Collection collDataItem = (Collection)e.Item.DataItem;
        switch (e.Item.ItemType)
        {
            case ListItemType.Header:
                //cell 0 - checkbox
                //cell 1 - icon
                //cell 2 - title
                //cell 3 - request type
                //cell 4 - start date
                //cell 5 - date modified
                //cell 6 - submitted by
                //cell 7 - id
                //cell 8 - language
                //cell 9 - path
                ((Literal)(e.Item.Cells[2].FindControl("litTitleHeader"))).Text = _MessageHelper.GetMessage("generic title");
                ((Literal)(e.Item.Cells[3].FindControl("litRequestTypeHeader"))).Text = _MessageHelper.GetMessage("generic request type");//"Request Type";
                ((Literal)(e.Item.Cells[4].FindControl("litStartDateHeader"))).Text = _MessageHelper.GetMessage("generic go live");//"Start Date";
                ((Literal)(e.Item.Cells[5].FindControl("litModifiedDateHeader"))).Text = _MessageHelper.GetMessage("modified date");//"Modified Date";
                ((Literal)(e.Item.Cells[6].FindControl("litSubmittedByHeader"))).Text = _MessageHelper.GetMessage("submitted by label");//"Submitted By";
                ((Literal)(e.Item.Cells[7].FindControl("litIdHeader"))).Text = _MessageHelper.GetMessage("generic subscriptionid");//"ID";
                ((Literal)(e.Item.Cells[8].FindControl("litLanguageHeader"))).Text = _MessageHelper.GetMessage("generic language");//"Language";
                ((Literal)(e.Item.Cells[9].FindControl("litPathHeader"))).Text = _MessageHelper.GetMessage("generic path");//"Path";
                break;
            case ListItemType.Item:
            case ListItemType.AlternatingItem:
                ((CheckBox)(e.Item.Cells[0].FindControl("cbApproval"))).Checked = IsApproved(Convert.ToString(collDataItem["ContentID"]), Convert.ToString(collDataItem["ContentLanguage"]));
                ((HiddenField)(e.Item.Cells[0].FindControl("hdnId"))).Value = Convert.ToString(collDataItem["ContentID"]);
                ((HiddenField)(e.Item.Cells[0].FindControl("hdnLanguageID"))).Value = Convert.ToString(collDataItem["ContentLanguage"]);
                ((Image)(e.Item.Cells[1].FindControl("imgContentIcon"))).AlternateText = Convert.ToString(collDataItem["ContentTitle"]);
                ((Image)(e.Item.Cells[1].FindControl("imgContentIcon"))).Attributes.Add("title", Convert.ToString(collDataItem["ContentTitle"]));
                ((Image)(e.Item.Cells[1].FindControl("imgContentIcon"))).ImageUrl = GetImagePath(collDataItem);
                ((HyperLink)(e.Item.Cells[2].FindControl("aTitle"))).NavigateUrl = GetItemLink(e.Item.DataItem);
                ((HyperLink)(e.Item.Cells[2].FindControl("aTitle"))).Text = Convert.ToString(collDataItem["ContentTitle"]);
                ((HyperLink)(e.Item.Cells[2].FindControl("aTitle"))).Attributes.Add("title", (string)(collDataItem["ContentTitle"]));
                ((HtmlGenericControl)(e.Item.Cells[3].FindControl("spanRequestType"))).Attributes.Add("style", GetRequestTypeColor(e.Item.DataItem));
                ((HtmlGenericControl)(e.Item.Cells[3].FindControl("spanRequestType"))).InnerText = GetRequestTypeText(e.Item.DataItem);
                ((Literal)(e.Item.Cells[3].FindControl("litStartDateValue"))).Text = GetStartDate(e.Item.DataItem);
                ((Literal)(e.Item.Cells[3].FindControl("litModifiedDateValue"))).Text = Convert.ToString(collDataItem["DisplayLastEditDate"]);
                ((Literal)(e.Item.Cells[4].FindControl("litSubmittedByValue"))).Text = Convert.ToString(collDataItem["SubmittedBy"]);
                ((Literal)(e.Item.Cells[5].FindControl("litIdValue"))).Text = Convert.ToString(collDataItem["ContentID"]);
                ((Literal)(e.Item.Cells[6].FindControl("litLanguageValue"))).Text = Convert.ToString(collDataItem["ContentLanguage"]);
                ((HyperLink)(e.Item.Cells[7].FindControl("aPathValue"))).NavigateUrl = GetPathLink(e.Item.DataItem);
                ((HyperLink)(e.Item.Cells[7].FindControl("aPathValue"))).Text = Convert.ToString(collDataItem["Path"]);
                ((HyperLink)(e.Item.Cells[7].FindControl("aPathValue"))).Attributes.Add("title", Convert.ToString(collDataItem["Path"]));
                break;
        }
    }

    public void lbSubmit_Click(System.Object sender, System.EventArgs e)
    {
        //this link button control is a placeholder - this event fires when user clicks submit button in toolbar
        string[] key;
        foreach (string keySet in _ApprovedList)
        {
            key = keySet.Split("_".ToCharArray());
            if (key[0] != string.Empty)
            {
                this._CommonApi.ContentLanguage = int.Parse(key[1]);
                if (Request["__EVENTARGUMENT"] == "true")
                {
                    this._EkContent.Approvev2_0(Convert.ToInt64(key[0]));
                }
                else
                {
                    this._EkContent.DeclineApproval2_0(Convert.ToInt64(key[0]), "");
                }
            }

        }
    }

    #endregion

    #region DataGrid Helpers

    private void GetApprovedItems()
    {

        foreach (DataGridItem dataGridItem in this.dgItemsNeedingApproval.Items)
        {
            if (dataGridItem.ItemType == ListItemType.Item || dataGridItem.ItemType == ListItemType.AlternatingItem)
            {

                //get .net controls from row
                string checkboxFieldId = ((CheckBox)(dataGridItem.Cells[0].FindControl("cbApproval"))).UniqueID;
                string contentidFieldId = ((HiddenField)(dataGridItem.Cells[0].FindControl("hdnId"))).UniqueID;
                string languageIdFieldId = ((HiddenField)(dataGridItem.Cells[0].FindControl("hdnLanguageID"))).UniqueID;

                bool approved = System.Convert.ToBoolean((Request.Form[checkboxFieldId] == "on") ? true : false);
                string contentId = (string)((string.IsNullOrEmpty(Request.Form[contentidFieldId])) ? string.Empty : (Request.Form[contentidFieldId]));
                string contentLanguage = (string)((string.IsNullOrEmpty(Request.Form[languageIdFieldId])) ? string.Empty : (Request.Form[languageIdFieldId]));


                //set value - either add to list or remove from list
                if (approved)
                {
                    _ApprovedList.Remove(contentId + "_" + contentLanguage);
                    _ApprovedList.Add(contentId + "_" + contentLanguage);
                }
                else
                {
                    _ApprovedList.Remove(contentId + "_" + contentLanguage);
                }
            }
        }

        //serialize to hidden field
        string[] approvedStringArray = (string[])_ApprovedList.ToArray(typeof(string));
        string approvedJoinedString = (string)(approvedStringArray.Length == 0 ? string.Empty : (string.Join(",", approvedStringArray)));
        this.hdnApprovedItems.Value = approvedJoinedString;

    }
    private void GetSavedApprovedItems()
    {

        string serializedApprovedItems = (string)((string.IsNullOrEmpty(Request.Form[this.hdnApprovedItems.UniqueID])) ? string.Empty : (Request.Form[this.hdnApprovedItems.UniqueID]));
        string[] serializedApprovedItemsArray = serializedApprovedItems.Split(",".ToCharArray());
        foreach (string approvedItem in serializedApprovedItemsArray)
        {
            _ApprovedList.Add(approvedItem);
        }

    }
    private bool IsApproved(string id, string languageId)
    {
        return ((_ApprovedList.IndexOf(id + "_" + languageId) > -1) ? true : false);
    }
    private string GetImagePath(Collection item)
    {
        string imagePath;
        if ((EkEnumeration.CMSContentSubtype)item["ContentSubType"] == Ektron.Cms.Common.EkEnumeration.CMSContentSubtype.PageBuilderData)
        {
            imagePath = _CommonApi.AppPath + "images/UI/Icons/contentWireframeTemplate.png";
        }
        else if ((int)item["ContentType"] == 1 || (int)item["ContentType"] == 2 || (int)item["ContentType"] == 3)
        {
            if ((int)item["ContentSubType"] == 2)
            {
                imagePath = _CommonApi.AppPath + "images/UI/Icons/calendarViewDay.png";
            }
            else
            {
                imagePath = _CommonApi.AppPath + "images/UI/Icons/contentHtml.png";
            }
        }
        else if ((int)item["ContentType"] == 3333)
        {
            imagePath = _CommonApi.AppPath + "images/UI/Icons/brick.png";
        }
        else
        {
            imagePath = Convert.ToString(item["Icon"]);
        }

        return imagePath;
    }
    private string GetItemLink(object dataItem)
    {
        Collection dItem = (Collection)dataItem;
        return _ContentApi.ApplicationPath + "/approval.aspx?action=viewContent&page=" + _pageLocation  +"&id=" + dItem["ContentID"] + "&LangType=" + dItem["ContentLanguage"] + "&rptType=" + dItem["ContentType"];
    }
    private string GetRequestTypeColor(object dataItem)
    {
        Collection dItem = (Collection)dataItem;
        return ((Convert.ToString(dItem["Status"]) == "S") ? "color:green;" : "color:red;");
    }
    private string GetRequestTypeText(object dataItem)
    {
        Collection dItem = (Collection)dataItem;
        return ((Convert.ToString(dItem["Status"]) == "S") ? (_MessageHelper.GetMessage("generic Publish")) : (_MessageHelper.GetMessage("generic Delete title")));
    }
    private string GetStartDate(object dataItem)
    {
        Collection dItem = (Collection)dataItem;
        return ((Convert.ToString(dItem["DisplayGoLive"]) != "" && Convert.ToString(dItem["DisplayGoLive"]) != DateTime.MinValue.ToString()) ? ((string)dItem["DisplayGoLive"]) : ((Convert.ToString(dItem["DateCreated"]) != "") ? ((string)dItem["DateCreated"]) : (string)(dItem["DisplayLastEditDate"])));
        
    }
    private string GetPathLink(object dataItem)
    {
        Collection dItem = (Collection)dataItem;
        return _ContentApi.ApplicationPath + "approval.aspx?action=viewApprovalList&fldid=" + dItem["FolderId"];
    }

    #endregion

    #region Helpers

    private void ViewApprovalList_Renamed()
    {
        string OrderBy;
        bool bShowToolbar = true;
        Collection cTmp = new Collection();

        litApproveAllWarning.Text = _MessageHelper.GetMessage("js: alert approve all selected warning");
        litDeclineAllWarning.Text = _MessageHelper.GetMessage("js: alert decline all selected warning");
        litNoItemSelected.Text = _MessageHelper.GetMessage("js:no items selected");

        OrderBy = (string)((string.IsNullOrEmpty(Request.QueryString["orderby"])) ? (Request.QueryString["orderby"]) : "title");

        cTmp.Add(_CommonApi.UserId, "UserID", null, null);
        _Folder = (string)((string.IsNullOrEmpty(Request.QueryString["fldid"])) ? string.Empty : (Request.QueryString["fldid"]));
        cTmp.Add(_Folder, "FolderIDs", null, null);
        cTmp.Add("", "OrderBy", null, null);

        if (_ContentType > 0)
        {
            cTmp.Add(_ContentType, "ContentType", null, null);
        }

        _ApprovalsCollection = _EkContent.GetApprovalListForUserIDv1_1(cTmp);

        bShowToolbar = System.Convert.ToBoolean((Request.QueryString["notoolbar"] == "1") ? false : true);
        if (bShowToolbar)
        {
            ViewToolBar();
        }
    }

    private void ViewToolBar()
    {
        System.Text.StringBuilder result = new System.Text.StringBuilder();
        int count = 0;
        int lAddMultiType = 0;
        Collection fldr;

        if (_Folder == "")
        {
            divTitleBar.InnerHtml = _StyleHelper.GetTitleBar((string)(_MessageHelper.GetMessage("view all awaiting approval") + ""));
        }
        else
        {
            fldr = _EkContent.GetFolderInfov2_0(Convert.ToInt64(_Folder));
            divTitleBar.InnerHtml = _StyleHelper.GetTitleBar((string)(_MessageHelper.GetMessage("view awaiting approval (folder)") + " \"" + fldr["FolderName"] + "\""));
        }
        result.Append("<table><tr>");

		if (Request.QueryString["page"] == "workarea")
		{
			// redirect to workarea when user clicks back button if we're in workarea
			result.Append(_StyleHelper.GetButtonEventsWCaption(_CommonApi.AppPath + "images/UI/Icons/back.png", "javascript:top.switchDesktopTab()", _MessageHelper.GetMessage("alt back button text"), _MessageHelper.GetMessage("btn back"), "", StyleHelper.BackButtonCssClass, true));
		}
		else if (_Folder != "")
		{
			result.Append(_StyleHelper.GetButtonEventsWCaption(_CommonApi.AppPath + "images/UI/Icons/back.png", "approval.aspx?action=viewApprovalList", _MessageHelper.GetMessage("alt back button text"), _MessageHelper.GetMessage("btn back"), "", StyleHelper.BackButtonCssClass, true));
		}

		bool primaryCssApplied = false;

        if (_ApprovalsCollection.Count > 0)
        {
			result.Append(_StyleHelper.GetButtonEventsWCaption(_CommonApi.AppPath + "Images/ui/icons/approvalApproveItem.png", "#", _MessageHelper.GetMessage("alt approve all selected button text"), _MessageHelper.GetMessage("btn approve all"), "onclick=\"return Ektron.Workarea.Reports.Approval.submit(true);\"", StyleHelper.ApproveButtonCssClass, !primaryCssApplied));

			primaryCssApplied = true;

            result.Append(_StyleHelper.GetButtonEventsWCaption(_CommonApi.AppPath + "Images/ui/icons/approvalDenyItem.png", "#", _MessageHelper.GetMessage("alt deny all selected button text"), _MessageHelper.GetMessage("btn deny all"), "onclick=\"return Ektron.Workarea.Reports.Approval.submit(false);\"", StyleHelper.DeclineButtonCssClass));
        }
        
        if (!string.IsNullOrEmpty(_CommonApi.RequestInformationRef.SystemEmail))
        {
			result.Append(_StyleHelper.GetButtonEventsWCaption(_CommonApi.AppPath + "images/UI/Icons/email.png", "#", _MessageHelper.GetMessage("Email Report button text"), _MessageHelper.GetMessage("btn email"), "onclick=\"LoadUserListChildPage();\"", StyleHelper.EmailButtonCssClass, !primaryCssApplied));

			primaryCssApplied = true;
        }

		result.Append(_StyleHelper.GetButtonEventsWCaption(_CommonApi.AppPath + "images/UI/Icons/print.png", "#", _MessageHelper.GetMessage("Print Report button text"), _MessageHelper.GetMessage("btn print"), "onclick=\"PrintReport();\"", StyleHelper.PrintButtonCssClass, !primaryCssApplied));

		primaryCssApplied = true;

        result.Append(_StyleHelper.GetButtonEventsWCaption(_CommonApi.AppPath + "images/UI/Icons/folderopen.png", "#", _MessageHelper.GetMessage("alt select folder"), _MessageHelper.GetMessage("btn select folder"), "onclick=\"LoadFolderChildPage(\'viewapprovallist\',\'" + _ContentLanguage + "\');\"", StyleHelper.OpenFolderButtonCssClass));

		result.Append(StyleHelper.ActionBarDivider);

        if (_EnableMultilingual == 1)
        {
            SiteAPI m_refsite = new SiteAPI();
            LanguageData[] language_data = m_refsite.GetAllActiveLanguages();

			result.Append("<td class=\"label\">" + _MessageHelper.GetMessage("lbl View") + ":");
            result.Append("<select id=selLang name=selLang onchange=\"Ektron.Workarea.Reports.Approval.loadLanguage(\'frmMain\');\">");
            if (_ContentLanguage == -1)
            {
                result.Append("<option value=" + Ektron.Cms.Common.EkConstants.ALL_CONTENT_LANGUAGES);
                if (Request.QueryString["page"] == "tree" || Request.QueryString["location"] == "tree")
                    result.Append("&location=tree");
                result.Append(" selected>All</option>");
            }
            else
            {
                result.Append("<option value=" + Ektron.Cms.Common.EkConstants.ALL_CONTENT_LANGUAGES);
                if (Request.QueryString["page"] == "tree" || Request.QueryString["location"] == "tree")
                    result.Append("&location=tree");
                result.Append(">All</option>");
            }
            for (count = 0; count <= language_data.Length - 1; count++)
            {
                if (Convert.ToString((short)_ContentLanguage) == Convert.ToString(language_data[count].Id))
                {
                    result.Append("<option value=" + language_data[count].Id);
                    if (Request.QueryString["page"] == "tree" || Request.QueryString["location"] == "tree")
                        result.Append("&location=tree");
                    result.Append(" selected>" + language_data[count].Name + "</option>");
                }
                else
                {
                    result.Append("<option value=" + language_data[count].Id);
                    if (Request.QueryString["page"] == "tree" || Request.QueryString["location"] == "tree")
                        result.Append("&location=tree");
                    result.Append(">" + language_data[count].Name + "</option>");
                }
            }
            result.Append("</select></td>");
        }

        GetAddMultiType();
        // If there is no content type from querystring check for the cookie and restore it to that value else all types

        result.Append("<td><select id=selAssetSupertype name=selAssetSupertype onchange=\"Ektron.Workarea.Reports.Approval.updateView();\">");
        if (Ektron.Cms.Common.EkConstants.CMSContentType_AllTypes == _ContentType)
        {
            result.Append("<option value=\'" + Ektron.Cms.Common.EkConstants.CMSContentType_AllTypes + "\' selected>" + this._MessageHelper.GetMessage("lbl all types") + "</option>");
        }
        else
        {
            result.Append("<option value=\'" + Ektron.Cms.Common.EkConstants.CMSContentType_AllTypes + "\'>" + this._MessageHelper.GetMessage("lbl all types") + "</option>");
        }
        if (Ektron.Cms.Common.EkConstants.CMSContentType_Content == _ContentType)
        {
            result.Append("<option value=\'" + Ektron.Cms.Common.EkConstants.CMSContentType_Content + "\' selected>" + this._MessageHelper.GetMessage("lbl html content") + "</option>");
        }
        else
        {
            result.Append("<option value=\'" + Ektron.Cms.Common.EkConstants.CMSContentType_Content + "\'>" + this._MessageHelper.GetMessage("lbl html content") + "</option>");
        }
        if (!(_AssetInfoData == null))
        {
            if (_AssetInfoData.Length > 0)
            {
                for (count = 0; count <= _AssetInfoData.Length - 1; count++)
                {
                    if (Ektron.Cms.Common.EkConstants.ManagedAsset_Min <= _AssetInfoData[count].TypeId && _AssetInfoData[count].TypeId <= Ektron.Cms.Common.EkConstants.ManagedAsset_Max)
                    {
                        if ("*" == _AssetInfoData[count].PluginType)
                        {
                            lAddMultiType = _AssetInfoData[count].TypeId;
                        }
                        else
                        {
                            result.Append("<option value=\'" + _AssetInfoData[count].TypeId + "\'");
                            if (_AssetInfoData[count].TypeId == _ContentType)
                            {
                                result.Append(" selected");
                            }
                            result.Append(">" +GetMessageText( _AssetInfoData[count].CommonName) + "</option>");
                        }
                    }
                }
            }
        }
        if (Ektron.Cms.Common.EkConstants.CMSContentType_Forms == _ContentType)
        {
            result.Append("<option value=\'" + Ektron.Cms.Common.EkConstants.CMSContentType_Forms + "\' selected>" + this._MessageHelper.GetMessage("generic FormsSurvey") + "</option>");
        }
        else
        {
            result.Append("<option value=\'" + Ektron.Cms.Common.EkConstants.CMSContentType_Forms + "\'>" + this._MessageHelper.GetMessage("generic FormsSurvey") + "</option>");
        }
        //if (Ektron.Cms.Common.EkConstants.CMSContentType_Library == _ContentType)
        //{
        //    result.Append("<option value=\'" + Ektron.Cms.Common.EkConstants.CMSContentType_Library + "\' selected>Images</option>");
        //}
        //else
        //{
        //    result.Append("<option value=\'" + Ektron.Cms.Common.EkConstants.CMSContentType_Library + "\'>Images</option>");
        //}
        if (Ektron.Cms.Common.EkConstants.CMSContentType_NonImageLibrary == _ContentType)
        {
            result.Append("<option value=\'" + Ektron.Cms.Common.EkConstants.CMSContentType_NonImageLibrary + "\' selected>" + this._MessageHelper.GetMessage("Non Image Managed Files") + "</option>");
        }
        else
        {
            result.Append("<option value=\'" + Ektron.Cms.Common.EkConstants.CMSContentType_NonImageLibrary + "\'>" + this._MessageHelper.GetMessage("Non Image Managed Files") + "</option>");
        }
        if (Ektron.Cms.Common.EkConstants.CMSContentType_PDF == _ContentType)
        {
            result.Append("<option value=\'" + Ektron.Cms.Common.EkConstants.CMSContentType_PDF + "\' selected>"+this._MessageHelper.GetMessage("generic pdf")+"</option>");
        }
        else
        {
            result.Append("<option value=\'" + Ektron.Cms.Common.EkConstants.CMSContentType_PDF + "\'>"+this._MessageHelper.GetMessage("generic pdf")+"</option>");
        }
        result.Append("</select></td>");
		result.Append(StyleHelper.ActionBarDivider);
        result.Append("<td>");
        result.Append(_StyleHelper.GetHelpButton("viewApprovalList", ""));
        result.Append("</td>");
        result.Append("</tr></table>");
        divToolBar.InnerHtml = result.ToString();
    }
    public string GetMessageText(string st)
    {
        if (st == "office documents")
            st = this._MessageHelper.GetMessage("lbl office documents");
        else if (st == "managed files")
            st = this._MessageHelper.GetMessage("lbl managed files");
        else if (st == "Multimedia")
            st = this._MessageHelper.GetMessage("lbl multimedia");
        else if (st == "Open Office")
            st = this._MessageHelper.GetMessage("lbl open office");
        else if (st == "Images")
            st = this._MessageHelper.GetMessage("generic images");
        else if (st == "Forms/Survey")
            st = this._MessageHelper.GetMessage("generic FormsSurvey");
        else if (st == "Non Image Managed Files")
            st = this._MessageHelper.GetMessage("Non Image Managed Files");
        else if (st == "PDF")
            st = this._MessageHelper.GetMessage("generic pdf");
        else if (st.ToLower() == "managed asset")
            st = this._MessageHelper.GetMessage("managed asset");

        return st;
    }
    public long GetAddMultiType()
    {
        long returnValue;
        // gets ID for "add multiple" asset type
        returnValue = 0;
        int count;
        _AssetInfoData = _ContentApi.GetAssetSupertypes();
        if (_AssetInfoData != null)
        {

            for (count = 0; count <= _AssetInfoData.Length - 1; count++)
            {
                if (Ektron.Cms.Common.EkConstants.ManagedAsset_Min <= _AssetInfoData[count].TypeId && _AssetInfoData[count].TypeId <= Ektron.Cms.Common.EkConstants.ManagedAsset_Max)
                {
                    if ("*" == _AssetInfoData[count].PluginType)
                    {
                        returnValue = _AssetInfoData[count].TypeId;
                    }
                }
            }
        }
        return returnValue;
    }

    #endregion

    #region JS/CSS

    private void RegisterResources()
    {
        Ektron.Cms.API.Css.RegisterCss(this, Ektron.Cms.API.Css.ManagedStyleSheet.EktronFixedPositionToolbarCss);
        Ektron.Cms.API.JS.RegisterJS(this, Ektron.Cms.API.JS.ManagedScript.EktronWorkareaHelperJS);
    }

    #endregion

}
