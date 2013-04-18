using System.Web.UI.WebControls;
using System.Configuration;
using System.Collections;
using System.Linq;
using System.Data;
using System.Web.Caching;
using System.Xml.Linq;
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
using Ektron.Cms.Common;
using Ektron.Cms.Localization;
using Ektron.Cms.Framework.UI;
using Microsoft.Security.Application;

public class StyleHelper
{
	public const string ActionBarDivider = "<td><div class=\"actionbarDivider\">&nbsp;</div></td>";

	public static HtmlTableCell ActionBarDividerCell
	{
		get
		{
			var cell = new HtmlTableCell();
			cell.InnerHtml = ActionBarDivider;
			
			return cell;
		}
	}

	public const string PrimaryFunctionCssClass = "primary";
	public const string SecondaryFunctionCssClass = "secondary";

	public const string AddAssetButtonCssClass = "addAssetButton";
	public const string AddButtonCssClass = "addButton";
	public const string AddBookButtonCssClass = "addBookButton";
	public const string AddBrowseButtonCssClass = "addBrowseButton";
	public const string AddCommentButtonCssClass = "addCommentButton";
	public const string AddContentButtonCssClass = "addContentButton";
	public const string AddEmailButtonCssClass = "addEmailButton";
	public const string AddTaskButtonCssClass = "addTaskButton";
	public const string AddTranslationButtonCssClass = "addTranslationButton";
	public const string AddUserGroupButtonCssClass = "addUserGroupButton";
	public const string ApprovalsButtonCssClass = "approvalsButton";
	public const string ApproveButtonCssClass = "approveButton";
	public const string AssignFoldersButtonCssClass = "assignFoldersButton";
	public const string AssignItemsButtonCssClass = "assignItemsButton";

	public const string BackButtonCssClass = "backButton";
	public const string BrowseButtonCssClass = "browseButton";

	public const string CancelButtonCssClass = "cancelButton";
	public const string CautionButtonCssClass = "cautionButton";
	public const string CheckInButtonCssClass = "checkInButton";
	public const string CompareAnalyticsButtonCssClass = "compareAnalyticsButton";
	public const string CopyButtonCssClass = "copyButton";
	public const string ContextualHelpButtonCssClass = "contextualHelpButton";
	public const string CrawlFullButtonCssClass = "crawlFullButton";
	public const string CrawlIncrementalButtonCssClass = "crawlIncrementalButton";

	public const string DeclineButtonCssClass = "declineButton";
	public const string DeleteButtonCssClass = "deleteButton";
	public const string DeleteDriveButtonCssClass = "deleteDriveButton";
	public const string DeleteEmailButtonCssClass = "deleteEmailButton";
	public const string DeleteHistoryButtonCssClass = "deleteHistoryButton";
	public const string DownButtonCssClass = "downButton";

	public const string EditButtonCssClass = "editButton";
	public const string EditContentFormButtonCssClass = "editContentFormButton";
	public const string EditFallbackButtonCssClass = "editFallbackButton";
	public const string EditPropertiesButtonCssClass = "editPropertiesButton";
	public const string EditSmartformButtonCssClass = "editSmartformButton";
	public const string EmailButtonCssClass = "emailButton";
	public const string EnableButtonCssClass = "enableButton";
	public const string ExportButtonCssClass = "exportButton";
	public const string ExportTaxonomyButtonCssClass = "exportTaxonomyButton";
	public const string ExportTranslationButtonCssClass = "exportTranslationButton";

	public const string FilterReportButtonCssClass = "filterReportButton";
	public const string ForwardEmailButtonCssClass = "forwardEmailButton";

	public const string HistoryButtonCssClass = "historyButton";

	public const string ImportTaxonomyButtonCssClass = "importTaxonomyButton";
	public const string InsertButtonCssClass = "insertButton";
	public const string InsertBookButtonCssClass = "insertBookButton";
	public const string InviteButtonCssClass = "inviteButton";

	public const string LastStatusButtonCssClass = "lastStatusButton";
	public const string LocaleDoNotTranslateButtonCssClass = "localeDoNotTranslateButton";
	public const string LocaleNeedsTranslationButtonCssClass = "localeNeedsTranslationButton";
	public const string LocaleNotReadyButtonCssClass = "localeNotReadyButton";
	public const string LocaleOutForTranslationButtonCssClass = "localeOutForTranslationButton";
	public const string LocaleReadyButtonCssClass = "localeReadyButton";
	public const string LocaleTranslatedButtonCssClass = "localeTranslatedButton";
	public const string LogHistoryButtonCssClass = "logHistoryButton";

	public const string NextButtonCssClass = "nextButton";
	public const string NextDisabledButtonCssClass = "nextDisabledButton";

	public const string MoveButtonCssClass = "moveButton";
	public const string MoveContentButtonCssClass = "moveContentButton";

	public const string OpenFolderButtonCssClass = "openFolderButton";
	public const string OverwriteButtonCssClass = "overwriteButton";

	public const string PreApprovalButtonCssClass = "preApprovalButton";
	public const string PreviewButtonCssClass = "previewButton";
	public const string PreviousButtonCssClass = "previousButton";
	public const string PreviousDisabledButtonCssClass = "previousDisabledButton";
	public const string PreviewStatusButtonCssClass = "previewStatusButton";
    public const string PreviewStatusButtonCssNewClass = "previewStatusNewButton";
	public const string PrintButtonCssClass = "printButton";
	public const string PublishButtonCssClass = "publishButton";

	public const string RefreshButtonCssClass = "refreshButton";
	public const string RemoveButtonCssClass = "removeButton";
	public const string ReOrderButtonCssClass = "reOrderButton";
	public const string ReplyToEmailButtonCssClass = "replyToEmailButton";
	public const string ResolveConflictButtonCssClass = "resolveConflictButton";
	public const string RestoreButtonCssClass = "restoreButton";
	public const string ReviewConflictButtonCssClass = "reviewConflictButton";

	public const string SaveButtonCssClass = "saveButton";
	public const string SearchButtonCssClass = "searchButton";
	public const string SentEmailsButtonCssClass = "sentEmailsButton";
	public const string SubmitForApprovalButtonCssClass = "submitForApprovalButton";
	public const string SyncButtonCssClass = "syncButton";

	public const string TranslationButtonCssClass = "translationButton";

    public const string UndoCheckout = "undoCheckout";
	public const string UpButtonCssClass = "upButton";
	public const string UpdateQuicklinkButtonCssClass = "updateQuicklinkButton";

	public const string ViewAnalyticsButtonCssClass = "viewAnalyticsButton";
	public const string ViewApprovalsButtonCssClass = "viewApprovalsButton";
	public const string ViewBookButtonCssClass = "viewBookButton";
	public const string ViewDifferenceButtonCssClass = "viewDifferenceButton";
	public const string ViewGroupMembersButtonCssClass = "viewGroupMembersButton";
	public const string ViewPermissionsButtonCssClass = "viewPermissionsButton";
	public const string ViewPropertiesButtonCssClass = "viewPropertiesButton";
	public const string ViewPublishedButtonCssClass = "viewPublishedButton";
	public const string ViewReportButtonCssClass = "viewReportButton";
	public const string ViewStagedButtonCssClass = "viewStagedButton";
	public const string ViewTaskButtonCssClass = "viewTaskButton";
	public const string ViewXslButtonCssClass = "viewXslButton";

	public static string MergeCssClasses(string[] classNames)
	{
		var classString = "";

		if (classNames != null && classNames.Length > 0)
		{
			var len = classNames.Length;
			int i;

			classString = classNames[0];

			for (i = 1; i < len; i++ )
			{
				classString += " " + classNames[i];
			}
		}

		return classString;
	}

    private System.String myTemp = "";
    private System.Int32 MyButtonName = 100;
    private System.Boolean DisplayTransText = false;
    private EkMessageHelper m_refMsg = null;
    private CommonApi m_refAPI = null;
    private int ContentLanguage = -1;

    public StyleHelper()
    {
        m_refAPI = new CommonApi();
        string strLangID;
        strLangID = System.Web.HttpContext.Current.Request.QueryString["LangType"];
        if (strLangID != null && Information.IsNumeric(strLangID))
        {
            try
            {
                ContentLanguage = Convert.ToInt32(strLangID);
            }
            catch (Exception ex)
            {
                EkException.LogException(ex, System.Diagnostics.EventLogEntryType.Warning);
                EkException.WriteToEventLog((string)("Language string was: " + strLangID), System.Diagnostics.EventLogEntryType.Warning);
            }
            m_refAPI.SetCookieValue("LastValidLanguageID", ContentLanguage.ToString());
        }
        else
        {
            strLangID = m_refAPI.GetCookieValue("LastValidLanguageID");
            if (strLangID != null && Information.IsNumeric(strLangID))
            {
                ContentLanguage = Convert.ToInt32(strLangID);
            }
        }
        if (ContentLanguage == Ektron.Cms.Common.EkConstants.CONTENT_LANGUAGES_UNDEFINED)
        {
            m_refAPI.ContentLanguage = Ektron.Cms.Common.EkConstants.ALL_CONTENT_LANGUAGES;
        }
        else
        {
            m_refAPI.ContentLanguage = ContentLanguage;
        }
        DisplayTransText = m_refAPI.DisplayTransText;
        m_refMsg = m_refAPI.EkMsgRef;
    }
    public System.Web.UI.WebControls.BoundField CreateBoundField(string DataField, string HeaderText, string CssClass, System.Web.UI.WebControls.HorizontalAlign HeaderHorizontalAlign, System.Web.UI.WebControls.HorizontalAlign ItemHorizontalAlign, System.Web.UI.WebControls.Unit HeaderWidth, System.Web.UI.WebControls.Unit ItemWidth, bool HtmlEncode, bool ItemWrap)
    {
        System.Web.UI.WebControls.BoundField colBound = new System.Web.UI.WebControls.BoundField();
        colBound.DataField = DataField;
        colBound.HeaderText = HeaderText;
        colBound.HeaderStyle.CssClass = CssClass;
        colBound.HeaderStyle.Width = HeaderWidth;
        colBound.ItemStyle.Width = ItemWidth;
        colBound.ItemStyle.HorizontalAlign = ItemHorizontalAlign;
        colBound.HeaderStyle.HorizontalAlign = HeaderHorizontalAlign;
        colBound.HtmlEncode = HtmlEncode;
        colBound.ItemStyle.Wrap = ItemWrap;
        return colBound;
    }
    public System.String HyperlinkWCaption(System.String HrefPath, System.String DisplayText, System.String HeaderText, System.String specialEvents)
    {
        StringBuilder result = null;
        try
        {
            result = new StringBuilder();
            result.Append("<a href=\"" + HrefPath + "\" onMouseOver=\"");

            if (DisplayTransText)
            {
                if (HeaderText != "")
                {
                    //result.Append("ShowTransString(\'" + HeaderText.Replace("\'", "\\\'") + "\');");
                }
            }
            result.Append("\" onMouseOut=\"");
            if (DisplayTransText)
            {
                if (HeaderText != "")
                {
                    //result.Append("HideTransString();");
                }
            }
            result.Append("\">" + DisplayText + "</a>");

            MyButtonName++;
        }
        catch (Exception)
        {
            result.Length = 0;
        }
        finally
        {
        }
        return (result.ToString());
    }
    public System.String GetButtonEvents(System.String ImageFile, System.String hrefPath, System.String altText, System.String specialEvents)
    {
        StringBuilder result = null;
        try
        {
            result = new StringBuilder();
            result.Append("<td ");
            result.Append("id=\"image_cell_" + MyButtonName + "\" ");
            result.Append("class=\"button\" title=\"" + altText + "\">");
            result.Append("<a id=\"image_link_" + MyButtonName + "\" href=\"" + hrefPath + "\" " + specialEvents + " ");
            result.Append("onMouseOver=\"RollOver(this);\" onMouseOut=\"RollOut(this);\" style=\"cursor: default;\">");
            result.Append("<img onClick=\"" + "SelectButton(this);" + "\" src=\"" + ImageFile + "\" id=\"image_" + MyButtonName + "\" class=\"button\">");
            result.Append("</a></td>");
            MyButtonName++;
        }
        catch (Exception)
        {
            result.Length = 0;
        }
        return (result.ToString());
    }

    public System.String GetButtonEventsWCaption(System.String imageFile, System.String hrefPath, System.String altText, System.String HeaderText, System.String specialEvents)
    {
        return GetButtonEventsWCaption(imageFile, hrefPath, altText, HeaderText, specialEvents, null, false);
    }

	public System.String GetButtonEventsWCaption(System.String imageFile, System.String hrefPath, System.String altText, System.String HeaderText, System.String specialEvents, System.String aTagClassName)
	{
		return GetButtonEventsWCaption(imageFile, hrefPath, altText, HeaderText, specialEvents, aTagClassName, false);
	}

	public System.String GetButtonEventsWCaption(System.String imageFile, System.String hrefPath, System.String altText, System.String HeaderText, System.String specialEvents, System.String aTagClassName, bool isPrimary)
	{
		StringBuilder result = null;

		try
		{
			result = new StringBuilder();
			result.Append("<td ");
			result.Append("id=\"image_cell_" + MyButtonName + "\" ");

			if (isPrimary && aTagClassName != BackButtonCssClass && aTagClassName != CancelButtonCssClass)
			{
				result.Append("class=\"button\">");
			}
			else
			{
				result.Append("class=\"button\" title=\"" + HeaderText.Replace("\'", "\\\'") + "\">");
			}

			result.Append("<a ");

			string priorityCssClass = (isPrimary) ? PrimaryFunctionCssClass : SecondaryFunctionCssClass;

			if (!String.IsNullOrEmpty(aTagClassName))
			{
				result.Append("class=\"" + MergeCssClasses(new string[2] { priorityCssClass, aTagClassName }) + "\" ");
			}
			else
			{
				result.Append("class=\"" + priorityCssClass + "\" ");
			}

			result.Append("id=\"image_link_" + MyButtonName + "\" href=\"" + hrefPath + "\" " + specialEvents + " ");
			
			result.Append("onMouseOver=\"");
			if (DisplayTransText)
			{
				if (HeaderText != "")
				{
					//result.Append("ShowTransString(\'" + HeaderText.Replace("\'", "\\\'") + "\');");
				}
			}
			result.Append("RollOver(this);\" onMouseOut=\"");
			if (DisplayTransText)
			{
				if (HeaderText != "")
				{
					//result.Append("HideTransString();");
				}
			}
			result.Append("RollOut(this);\"");
			result.Append(" style=\"cursor: default;\">");

			if (isPrimary)
			{
				if (aTagClassName == BackButtonCssClass || aTagClassName == CancelButtonCssClass)
				{
					// leave blank for icon.
				}
				else
				{
					result.Append(HeaderText);
				}
			}
			else
			{
				result.Append("<img onClick=\"" + "SelectButton(this);" + "\" src=\"" + imageFile + "\" id=\"image_" + MyButtonName + "\" class=\"button\">");
			}
			
			result.Append("</a></td>");

			MyButtonName++;
		}
		catch (Exception)
		{
			result.Length = 0;
		}
		finally
		{
		}
		return (result.ToString());
	}

    /// <summary>
    /// Converts an ASP.Net image button so it works in the CMS400 toolbar
    /// All your code is handled in the codebehind for the button click
    /// </summary>
    /// <param name="btn">button to update</param>
    /// <param name="altText">alt text for button for flyover help</param>
    /// <param name="HeaderText">text to put in toolbar header on mouseover of button</param>
    /// <remarks></remarks>
    public void MakeToolbarButton(ImageButton btn, string altText, string HeaderText)
    {
        btn.AlternateText = altText;
       // btn.Attributes.Add("onMouseOver", "ShowTransString(\'" + HeaderText + "\'); RollOver(this);");
       // btn.Attributes.Add("onMouseOut", "HideTransString(); RollOut(this);");
        btn.Attributes.Add("style", "cursor:default;");
        btn.Attributes.Add("style", "border-width:1px");
    }

    public System.String GetTitleBar(System.String Title)
    {
        StringBuilder result = new StringBuilder();
        try
        {
            result.Append("<span id=\"WorkareaTitlebar\">" + Title + "</span>");
            result.Append("<span style=\"display:none\" id=\"_WorkareaTitlebar\"></span>");
        }
        catch (Exception)
        {
            result.Length = 0;
        }
        return (result.ToString());
    }
    public System.String GetShowAllActiveLanguage(System.Boolean showAllOpt, System.String bgColor, System.String OnChangeEvt, System.String SelLang)
    {
        return GetShowAllActiveLanguage(showAllOpt, bgColor, OnChangeEvt, SelLang, false);
    }
    public System.String GetShowAllActiveLanguage(System.Boolean showAllOpt, System.String bgColor, System.String OnChangeEvt, System.String SelLang, bool showOnlySiteEnabled)
    {
        return ("<td>" + ShowAllActiveLanguage(showAllOpt, bgColor, OnChangeEvt, SelLang, showOnlySiteEnabled) + "</td>");
    }
    public System.String ShowAllActiveLanguage(System.Boolean showAllOpt, System.String bgColor, System.String OnChangeEvt, System.String SelLang)
    {
        return ShowAllActiveLanguage(showAllOpt, bgColor, OnChangeEvt, SelLang, false);
    }
    public System.String ShowAllActiveLanguage(System.Boolean showAllOpt, System.String bgColor, System.String OnChangeEvt, System.String SelLang, bool showOnlySiteEnabled)
    {
        StringBuilder result = new StringBuilder();
        LanguageData[] language_data;
        SiteAPI m_refSiteApi = new SiteAPI();
        int LanguageId = m_refSiteApi.ContentLanguage;
        try
        {
            if (OnChangeEvt == "")
            {
                OnChangeEvt = "SelLanguage(this.value)";
            }
            if (SelLang.Trim() != "")
            {
                LanguageId = Convert.ToInt32(SelLang);
            }
            language_data = m_refSiteApi.GetAllActiveLanguages();
            result = new StringBuilder();
            if (m_refAPI.EnableMultilingual == 1)
            {
                result.Append("<select id=\"frm_langID\" name=\"frm_langID\" OnChange=\"" + OnChangeEvt + "\">" + "\r\n");
                if (showAllOpt)
                {
                    result.Append("<option value=\"" + Ektron.Cms.Common.EkConstants.ALL_CONTENT_LANGUAGES + "\"");
                    if (LanguageId == Ektron.Cms.Common.EkConstants.ALL_CONTENT_LANGUAGES)
                    {
                        result.Append(" selected=\"selected\"");
                    }
                    result.Append(">");
                    result.Append("All");
                    result.Append("</option>");
                }
                if (!(language_data == null))
                {
                    for (int iLang = 0; iLang <= language_data.Length - 1; iLang++)
                    {
                        LanguageData with_1 = language_data[iLang];
                        if (with_1.Id != 0 && (with_1.SiteEnabled || showOnlySiteEnabled == false))
                        {
                            result.Append("<option value=\"" + with_1.Id + "\"");
                            if (LanguageId == with_1.Id)
                            {
                                result.Append(" selected=\"selected\"");
                            }
                            else
                            {
                            }
                            result.Append(">");
                            result.Append(with_1.LocalName);
                            result.Append("</option>");
                        }
                    }
                }
                result.Append("</select>");
            }
        }
        catch (Exception)
        {
            result.Length = 0;
        }
        return (result.ToString());
    }
    //Public Shared Function GetClientScript() As String
    public string GetClientScript()
    {
        StringBuilder result;
        try
        {
            CommonApi m_refAPI = new CommonApi();
            if ((!(System.Web.HttpContext.Current.Request.QueryString["LangType"] == null)) && (System.Web.HttpContext.Current.Request.QueryString["LangType"] != ""))
            {
                m_refAPI.ContentLanguage = Convert.ToInt32(System.Web.HttpContext.Current.Request.QueryString["LangType"]);
            }

            System.Web.UI.Page page = HttpContext.Current.Handler as System.Web.UI.Page;
            Packages.Ektron.Workarea.Core.Register(page);

            Ektron.Cms.API.JS.RegisterJS(page, m_refAPI.AppPath + "java/stylehelper.js", "EktronStyleHelperJS");

            result = new StringBuilder();
            result.Append("<script type=\"text/javascript\">" + "\r\n");
            result.Append("<!--//--><![CDATA[//><!--" + "\r\n");
            result.Append(" " + "\r\n");
            result.Append("var g_relativeClassPath = \'" + m_refAPI.AppPath + "csslib/\';" + "\r\n");
            result.Append("g_relativeClassPath = g_relativeClassPath.toLowerCase();" + "\r\n");
            result.Append("UpdateWorkareaTitleToolbars();" + "\r\n");
            result.Append(" " + "\r\n");
            result.Append("function GetRelativeClassPath() {" + "\r\n");
            result.Append("    return(g_relativeClassPath);" + "\r\n");
            result.Append("}" + "\r\n");
            result.Append(" " + "\r\n");
            result.Append("function UpdateWorkareaTitleToolbars() {" + "\r\n");
            result.Append("    if (document.styleSheets.length > 0) {" + "\r\n");
            result.Append("        MakeClassPathRelative(\'*\', \'button\', \'backgroundImage\', \'" + m_refAPI.AppImgPath + "\', g_relativeClassPath)" + "\r\n");
            result.Append("        MakeClassPathRelative(\'*\', \'button-over\', \'backgroundImage\', \'" + m_refAPI.AppImgPath + "\', g_relativeClassPath)" + "\r\n");
            result.Append("        MakeClassPathRelative(\'*\', \'button-selected\', \'backgroundImage\', \'" + m_refAPI.AppImgPath + "\', g_relativeClassPath)" + "\r\n");
            result.Append("        MakeClassPathRelative(\'*\', \'button-selectedOver\', \'backgroundImage\', \'" + m_refAPI.AppImgPath + "\', g_relativeClassPath)" + "\r\n");
            result.Append("        MakeClassPathRelative(\'*\', \'ektronToolbar\', \'backgroundImage\', \'" + m_refAPI.AppImgPath + "\', g_relativeClassPath)" + "\r\n");
            result.Append("        MakeClassPathRelative(\'*\', \'ektronTitlebar\', \'backgroundImage\', \'" + m_refAPI.AppImgPath + "\', g_relativeClassPath)" + "\r\n");
            result.Append("    } else {" + "\r\n");
            result.Append("        setTimeout(\'UpdateWorkareaTitleToolbars()\', 500);" + "\r\n");
            result.Append("    }" + "\r\n");
            result.Append("}" + "\r\n");

            /*result.Append("function ShowTransString(Text) {" + "\r\n");
            result.Append("var ObjId = \"WorkareaTitlebar\";" + "\r\n");
            result.Append("var ObjShow = document.getElementById(\'_\' + ObjId);" + "\r\n");
            result.Append("var ObjHide = document.getElementById(ObjId);" + "\r\n");
            result.Append("if ((typeof ObjShow != \"undefined\") && (ObjShow != null)) {" + "\r\n");
            result.Append("ObjShow.innerHTML = Text;" + "\r\n");
            result.Append("ObjShow.style.display = \"inline\";" + "\r\n");
            result.Append("if ((typeof ObjHide != \"undefined\") && (ObjHide != null)) {" + "\r\n");
            result.Append("ObjHide.style.display = \"none\";" + "\r\n");
            result.Append("}" + "\r\n");
            result.Append("}" + "\r\n");

            result.Append("}");
            result.Append("function HideTransString() {" + "\r\n");
            result.Append("var ObjId = \"WorkareaTitlebar\";" + "\r\n");
            result.Append("var ObjShow = document.getElementById(ObjId);" + "\r\n");
            result.Append("var ObjHide = document.getElementById(\'_\' + ObjId);" + "\r\n");

            result.Append("if ((typeof ObjShow != \"undefined\") && (ObjShow != null)) {" + "\r\n");
            result.Append("ObjShow.style.display = \"inline\";");
            result.Append("if ((typeof ObjHide != \"undefined\") && (ObjHide != null)) {" + "\r\n");
            result.Append("ObjHide.style.display = \"none\";" + "\r\n");
            result.Append("}" + "\r\n");
            result.Append("}" + "\r\n");
            result.Append("}" + "\r\n");*/
            result.Append("function GetCellObject(MyObj) {" + "\r\n");
            result.Append("var tmpName = \"\";" + "\r\n");

            result.Append("tmpName = MyObj.id;" + "\r\n");
            result.Append("if (tmpName.indexOf(\"link_\") >= 0) {" + "\r\n");
            result.Append("tmpName = tmpName.replace(\"link_\", \"cell_\");" + "\r\n");
            result.Append("}");
            result.Append("else if (tmpName.indexOf(\"cell_\") >= 0) {" + "\r\n");
            result.Append("tmpName = tmpName;" + "\r\n");
            result.Append("}" + "\r\n");
            result.Append("else {" + "\r\n");
            result.Append("tmpName = tmpName.replace(\"image_\", \"image_cell_\");" + "\r\n");
            result.Append("}" + "\r\n");
            result.Append("MyObj = document.getElementById(tmpName);" + "\r\n");
            result.Append("return (MyObj);" + "\r\n");
            result.Append("}" + "\r\n");

            result.Append("var g_OldBtnObject = null;" + "\r\n");

            result.Append("function ClearPrevBtn() {" + "\r\n");
            result.Append("if (g_OldBtnObject){" + "\r\n");
            result.Append("  RollOut(g_OldBtnObject);" + "\r\n");
            result.Append("  g_OldBtnObject = null;" + "\r\n");
            result.Append("}" + "\r\n");
            result.Append("}" + "\r\n");

            result.Append("function RollOver(MyObj) {" + "\r\n");
            result.Append("ClearPrevBtn();" + "\r\n");
            result.Append("g_OldBtnObject = MyObj;" + "\r\n");
            result.Append("MyObj = GetCellObject(MyObj);");
            result.Append("MyObj.className = \"button-over\";" + "\r\n");
            result.Append("}" + "\r\n");

            result.Append("function RollOut(MyObj) {" + "\r\n");
            result.Append("if (g_OldBtnObject == MyObj){" + "\r\n");
            result.Append("g_OldBtnObject = null;" + "\r\n");
            result.Append("}" + "\r\n");
            result.Append("MyObj = GetCellObject(MyObj);" + "\r\n");
            result.Append("MyObj.className = \"button\";" + "\r\n");
            result.Append("}" + "\r\n");

            result.Append("function SelectButton(MyObj) {" + "\r\n");

            result.Append("}" + "\r\n");

            result.Append("function UnSelectButtons() { " + "\r\n");
            result.Append("var iLoop = 100; " + "\r\n");

            result.Append("while (document.getElementById(\"image_cell_\" + iLoop.toString()) != null) { " + "\r\n");
            result.Append("document.getElementById(\"image_cell_\" + iLoop.toString()).className = \"button\"; " + "\r\n");
            result.Append("iLoop++; " + "\r\n");
            result.Append("} " + "\r\n");
            result.Append("} " + "\r\n");

            result.Append("function Trim (string) { " + "\r\n");
            result.Append("if (string.length > 0) { " + "\r\n");
            result.Append("string = RemoveLeadingSpaces (string); " + "\r\n");
            result.Append("} " + "\r\n");
            result.Append("if (string.length > 0) { " + "\r\n");
            result.Append("string = RemoveTrailingSpaces(string); " + "\r\n");
            result.Append("} " + "\r\n");
            result.Append("return string; " + "\r\n");
            result.Append("} " + "\r\n");

            result.Append("function RemoveLeadingSpaces(string) {");
            result.Append("while(string.substring(0, 1) == \" \") { " + "\r\n");
            result.Append("string = string.substring(1, string.length); " + "\r\n");
            result.Append("} " + "\r\n");
            result.Append("return string; " + "\r\n");
            result.Append("} " + "\r\n");

            result.Append("function RemoveTrailingSpaces(string) { " + "\r\n");
            result.Append("while(string.substring((string.length - 1), string.length) == \" \") { " + "\r\n");
            result.Append("string = string.substring(0, (string.length - 1)); " + "\r\n");
            result.Append("} " + "\r\n");
            result.Append("return string; " + "\r\n");
            result.Append("} " + "\r\n");

            result.Append("function SelLanguage(inVal) { " + "\r\n");
            myTemp = (string)(System.Web.HttpContext.Current.Request.ServerVariables["PATH_INFO"].Substring(System.Convert.ToInt32(System.Web.HttpContext.Current.Request.ServerVariables["PATH_INFO"].LastIndexOf("/") + 1)));
            //myTemp = myTemp + "?" + Ektron.Cms.API.JS.Escape(Strings.Replace(System.Web.HttpContext.Current.Request.ServerVariables["QUERY_STRING"], "LangType", "", 1, -1, 0));
            myTemp = myTemp + "?" + Ektron.Cms.API.JS.Escape(EkFunctions.UrlEncode(System.Web.HttpContext.Current.Request.ServerVariables["QUERY_STRING"].ToString().Replace("LangType", "")));
            myTemp = myTemp.Replace("\'", "\"");
            myTemp = myTemp.Replace("\\x", "\\\\x");
            myTemp = myTemp.Replace("\\\\\\x", "\\\\x");
            myTemp = myTemp.Replace("\\u", "\\\\u");
            myTemp = myTemp.Replace("\\\\\\u", "\\\\u");
            myTemp = myTemp.Replace("&amp;", "&");
            myTemp = myTemp.Replace("SelectAll=1&", "");

            result.Append("top.notifyLanguageSwitch(inVal, -1)" + "\r\n");

            result.Append("document.location = decodeURIComponent(\'" + myTemp + "&LangType=\' + inVal) ; " + "\r\n");
            result.Append("} " + "\r\n");
            result.Append("//--><!]]>" + "\r\n");
            result.Append("</script> " + "\r\n");
        }
        catch (Exception)
        {
            result = new StringBuilder();
        }
        finally
        {
        }
        return (result.ToString());
    }
    public string GetAddAnchor(int Id)
    {
        string sResult = "";
        string sFormQuery = "";
        try
        {
            if (((!(System.Web.HttpContext.Current.Request.QueryString["ContType"] == null)) && (System.Web.HttpContext.Current.Request.QueryString["ContType"].ToString() == "2")) || ((!(Ektron.Cms.CommonApi.GetEcmCookie()["ContType"] == null)) && (Ektron.Cms.CommonApi.GetEcmCookie()["ContType"].ToString() == "2")))
            {
                sFormQuery = (string)("&folder_id=" + Id); //& "&callbackpage=content.aspx&parm1=action&value1=viewcontentbycategory&parm2=folder_id&value2=" & Id
            }
            sResult = (string)(GetButtonEventsWCaption(m_refAPI.AppPath + "images/UI/Icons/add.png", "#", m_refMsg.GetMessage("alt add content button text"), m_refMsg.GetMessage("btn add content"), "onclick=\"AddNewContent(\'LangType=" + ContentLanguage + "&type=add&createtask=1&id=" + Id + sFormQuery + "&" + GetBackParams() + "\');return false;\" "));
        }
        catch (Exception)
        {
            sResult = "";
        }
        return sResult;
    }

    public string GetAddAnchorByContentType(long Id, int contType, bool AllowNonFormattedHTML)
    {
        string sResult = "";
        string sFormQuery = "";
        try
        {
            if (contType == 2)
            {
                sFormQuery = (string)("&folder_id=" + Id); //& "&callbackpage=content.aspx&parm1=action&value1=viewcontentbycategory&parm2=folder_id&value2=" & Id
            }
            sResult = (string)("AddNewContent(\'LangType=" + ContentLanguage + "&type=add&createtask=1&id=" + Id + "&folderid=" + Id + sFormQuery + "&" + GetBackParams());
            if (AllowNonFormattedHTML)
            {
                sResult += "&AllowHTML=1";
            }
            sResult += "\', " + contType + ");";
        }
        catch (Exception)
        {
            sResult = "";
        }
        return sResult;
    }

    public string GetTypeOverrideAddAnchor(long Id, long xml_id, int contType)
    {
        string sResult = "AddNewContent(\'LangType=" + ContentLanguage + "&type=add&createtask=1&id=" + Id + "&xid=" + xml_id + "&" + GetBackParams() + "\'," + contType + "); ";
        return sResult;
    }
    public string GetAddOtherAnchor(object id)
    {
        string sResult = GetButtonEventsWCaption(m_refAPI.AppPath + "images/UI/Icons/add.png", "#", m_refMsg.GetMessage("alt add content button text"), m_refMsg.GetMessage("btn add content"), "OnClick=\"showMultiMenu(event)\" ");
        return sResult;
    }
    public string GetAddOtherMenuStyle()
    {
        string returnValue;
        string html = "" + "\r\n";
        html += "<STYLE>" + "\r\n";
        html += "	#xmladdMenu { " + "\r\n";
        html += "	position: absolute; " + "\r\n";
        html += "	visibility: hidden; " + "\r\n";
        html += "	width: 120px; " + "\r\n";
        html += "	background-color: lightgrey; " + "\r\n";
        html += "	layer-background-color: lightgrey; " + "\r\n";
        html += "	border: 2px outset white; " + "\r\n";
        html += "	} " + "\r\n";
        html += "	.A:Menu { " + "\r\n";
        html += "	color: black; " + "\r\n";
        html += "	text-decoration: none; " + "\r\n";
        html += "	cursor: default; " + "\r\n";
        html += "	width: 100% " + "\r\n";
        html += "	} " + "\r\n";
        html += "	.A:MenuOn { " + "\r\n";
        html += "	color: white; " + "\r\n";
        html += "	text-decoration: none; " + "\r\n";
        html += "	background-color: darkblue; " + "\r\n";
        html += "	cursor: default; " + "\r\n";
        html += "	width: 100% " + "\r\n";
        html += "	} " + "\r\n";
        html += "</STYLE> " + "\r\n";
        returnValue = html;
        return returnValue;
    }
    public string GetAddBlogPostAnchor(long Id)
    {
        string sResult = "";
        try
        {
            sResult = (string)(GetButtonEventsWCaption(m_refAPI.AppPath + "images/UI/Icons/add.png", "#", m_refMsg.GetMessage("alt add blogpost button text"), m_refMsg.GetMessage("btn add blogpost"), "OnClick=\"javascript:AddNewContent(\'LangType=" + ContentLanguage + "&type=add&createtask=1&id=" + Id + "&" + GetBackParams() + "\');return false;\" "));
        }
        catch (Exception)
        {
            sResult = "";
        }
        return sResult;
    }
    public string GetAddForumPostAnchor(long Id)
    {
        string sResult = "";
        try
        {
            sResult = (string)(GetButtonEventsWCaption(m_refAPI.AppPath + "images/UI/Icons/add.png", "#", m_refMsg.GetMessage("alt add forumpost button text"), m_refMsg.GetMessage("btn add forumpost"), "OnClick=\"javascript:AddNewContent(\'LangType=" + ContentLanguage + "&type=add&createtask=1&id=" + Id + "&" + GetBackParams() + "\');return false;\" "));
        }
        catch (Exception)
        {
            sResult = "";
        }
        return sResult;
    }
    public string GetAddMultiAnchor(long id, int ContType)
    {
        return (GetButtonEventsWCaption(m_refAPI.AppPath + "images/UI/Icons/add.png", "#", "Add Several Files", m_refMsg.GetMessage("btn add content"), "OnClick=\"javascript:AddNewContent(\'LangType=" + ContentLanguage + "&type=add&multi=" + ContType + "&createtask=1&id=" + id + "&" + GetBackParams() + "\',  " + Ektron.Cms.Common.EkConstants.CMSContentType_AllTypes + ");return false;\" "));
    }
    private string GetBackParams()
    {
        StringBuilder backURL;
        object Value;
        backURL = new StringBuilder();
        backURL.Append("back_file=content.aspx");
        Value = System.Web.HttpContext.Current.Request.QueryString["action"];
        if (Strings.Len(Value) > 0)
        {
            backURL.Append("&back_action=" + Value);
        }
        Value = System.Web.HttpContext.Current.Request.QueryString["folder_id"];
        if (Strings.Len(Value) > 0)
        {
            backURL.Append("&back_folder_id=" + Value);
        }
        Value = System.Web.HttpContext.Current.Request.QueryString["id"];
        if (Strings.Len(Value) > 0)
        {
            backURL.Append("&back_id=" + Value);
        }
        Value = ContentLanguage;
        if (Strings.Len(Value) > 0)
        {
            backURL.Append("&back_LangType=" + ContentLanguage);
        }
        return (backURL.ToString());
    }
    public string StatusWithToolTip(string Status)
    {
        string ToolTip = "";
        StringBuilder result = null;
        try
        {
            result = new StringBuilder();

            switch (Status.ToUpper())
            {
                case "A":
                    ToolTip = "The content has been through the workflow and published on the Web site.";
                    break;
                case "O":
                    ToolTip = "The content is currently being edited, and has not been checked in for publishing.";
                    break;
                case "I":
                    ToolTip = "The content has been checked in for other users to edit.";
                    break;
                case "S":
                    ToolTip = "The content block has been saved and submitted into the approval chain.";
                    break;
                case "M":
                    ToolTip = "This content has been requested to be deleted from Ektron CMS400.";
                    break;
                case "P":
                    ToolTip = "This content has been approved,but the Go Live date hasn’t occurred yet.";
                    break;
                case "T":
                    ToolTip = "This content has been submitted, but waiting for completion of associated tasks.";
                    break;
                case "D":
                    ToolTip = "This content has been mark for delete on the Go Live date.";
                    break;
            }
            result.Append("<a href=\"#Status\" onmouseover=\"ddrivetip(\'" + ToolTip + "\',\'ADC5EF\', 300);\" onmouseout=\"hideddrivetip();\" onclick=\"return false;\">" + Status + "</a>");
        }
        catch (Exception)
        {
        }
        finally
        {
        }
        return (result.ToString());
    }
    public string GetPermLayerTop()
    {
        string result = "";
        bool bNS6 = false;
        if ((Convert.ToString(System.Web.HttpContext.Current.Request.ServerVariables["http_user_agent"]).ToUpper()).IndexOf("GECKO") + 1 > 0)
        {
            bNS6 = true;
        }
        else
        {
            bNS6 = false;
        }
        if (!(bNS6))
        {
            result = result + "<ILAYER name=\"permLayer\"><LAYER name=\"standard\" visibility=\"show\"><NOLAYER>";
        }
        result = result + "<div id=\"standard\" style=\"display: block;\">";
        if (!(bNS6))
        {
            result = result + "</NOLAYER>";
        }
        return (result);
    }
    public string GetPermLayerMid()
    {
        string result = "";
        bool bNS6 = false;
        if ((Convert.ToString(System.Web.HttpContext.Current.Request.ServerVariables["http_user_agent"]).ToUpper()).IndexOf("GECKO") + 1 > 0)
        {
            bNS6 = true;
        }
        else
        {
            bNS6 = false;
        }
        if (!(bNS6))
        {
            result = result + "</LAYER><LAYER name=\"advanced\" visibility=\"hidden\"><NOLAYER>";
        }
        result = result + "</div><div id=\"advanced\" style=\"display: none;\">";
        if (!(bNS6))
        {
            result = result + "</NOLAYER>";
        }
        return (result);
    }
    public string GetPermLayerBottom()
    {
        bool bNS6 = false;
        string result = "";
        if ((Convert.ToString(System.Web.HttpContext.Current.Request.ServerVariables["http_user_agent"]).ToUpper()).IndexOf("GECKO") + 1 > 0)
        {
            bNS6 = true;
        }
        else
        {
            bNS6 = false;
        }
        if (!(bNS6))
        {
            result = result + "</LAYER></ILAYER><NOLAYER>";
        }
        result = result + "</div>";
        if (!(bNS6))
        {
            result = result + "</NOLAYER>";
        }
        return (result);
    }
    public int PermissionFlag(bool PermFlag)
    {
        int result = 0;
        if (PermFlag)
        {
            result = 1;
        }
        return (result);
    }
    public string GetEnableAllPrompt()
    {

        string enableAll;
        enableAll = "<div id=\"enablealldiv\" class=\"clearfix\">";
        enableAll += "<a href=\"#EnableAll\" class=\"button greenHover buttonLeft buttonCheckAll\" onclick=\"return SelectAllPerms();\" title=\"" + m_refMsg.GetMessage("enable all permissions") + "\">" + m_refMsg.GetMessage("generic Enable All") + "</a>";
        enableAll += "<a href=\"#DisableAll\" class=\"button redHover buttonLeft buttonClear\" onclick=\"return UnselectAllPerms();\" title=\"" + m_refMsg.GetMessage("generic Disable All") + "\">" + m_refMsg.GetMessage("generic Disable All") + "</a>";
        enableAll += "</div>";

        return enableAll;
    }
    public string GetSwapNavPrompt()
    {
        bool bNS6 = false;
        string result = "";
        if ((Convert.ToString(System.Web.HttpContext.Current.Request.ServerVariables["http_user_agent"]).ToUpper()).IndexOf("GECKO") + 1 > 0)
        {
            bNS6 = true;
        }
        else
        {
            bNS6 = false;
        }
        if (!(bNS6))
        {
            result = result + "<ILAYER name=\"messLayer\"><LAYER name=\"advancedMess\" visibility=\"show\"><NOLAYER>";
        }
        result = result + "<div id=\"advancedMess\" style=\"display: block;\">";
        if (!(bNS6))
        {
            result = result + "</NOLAYER>";
        }
        result = result + "<a href=\"#\" onClick=\"SwapPermDisplay();return false;\" title=\"" + m_refMsg.GetMessage("alt display adv perms text") + "\" alt=\"" + m_refMsg.GetMessage("alt display adv perms text") + "\">" + m_refMsg.GetMessage("display advanced permissions msg") + "</a>";
        if (!(bNS6))
        {
            result = result + "<NOLAYER>";
        }
        result = result + "</div>";
        if (!(bNS6))
        {
            result = result + "</NOLAYER></LAYER><LAYER name=\"standardMess\" visibility=\"hidden\"><NOLAYER>";
        }
        result = result + "<div id=\"standardMess\" style=\"display: none;\">";
        if (!(bNS6))
        {
            result = result + "</NOLAYER>";
        }
        result = result + "<a href=\"#\" onClick=\"SwapPermDisplay();return false;\" title=\"" + m_refMsg.GetMessage("alt display std perms text") + "\" alt=\"" + m_refMsg.GetMessage("alt display std perms text") + "\">" + m_refMsg.GetMessage("display standard permissions msg") + "</a>";
        if (!(bNS6))
        {
            result = result + "<NOLAYER>";
        }
        result = result + "</div>";
        if (!(bNS6))
        {
            result = result + "</NOLAYER></LAYER></ILAYER>";
        }
        return (result);
    }
    public string GetMemberShipNavSwap(string action, string strType, object id, object membership)
    {
        string result = "";
        if ((string)membership == "true")
        {
            result = "<a href=\"content.aspx?LangType=" + ContentLanguage + "&action=" + action + "&type=" + strType + "&id=" + id.ToString() + "&membership=false\">" + m_refMsg.GetMessage("lbl view cms users") + "</a>";
        }
        else
        {
            result = "<a href=\"content.aspx?LangType=" + ContentLanguage + "&action=" + action + "&type=" + strType + "&id=" + id + "&membership=true\">" + m_refMsg.GetMessage("lbl view memberShip users") + "</a>";
        }
        return (result);
    }
	public string GetCatalogEditAnchor(long Id, int Type, bool bFromApproval)
	{
		return GetCatalogEditAnchor(Id, Type, bFromApproval, false);
	}

    public string GetCatalogEditAnchor(long Id, int Type, bool bFromApproval, bool isPrimary)
    {
        string result = "";
        string SRC = "";
        string str;
        string backStr;

        if (bFromApproval)
        {
            backStr = "back_file=approval.aspx";
        }
        else
        {
            backStr = "back_file=content.aspx";
        }

        str = System.Web.HttpContext.Current.Request.QueryString["action"];
        if (str.Length > 0)
        {
            backStr = backStr + "&back_action=" + str;
        }

        if (bFromApproval)
        {
            if (!string.IsNullOrEmpty(System.Web.HttpContext.Current.Request.QueryString["page"]))
            {
                str = System.Web.HttpContext.Current.Request.QueryString["page"];
                if (str.Length > 0)
                {
                    backStr = backStr + "&back_page=" + str;
                }
            }
        }

        if (!bFromApproval)
        {
            str = System.Web.HttpContext.Current.Request.QueryString["folder_id"];
            if (str.Length > 0)
            {
                backStr = backStr + "&back_folder_id=" + str;
            }
        }

        str = System.Web.HttpContext.Current.Request.QueryString["id"];
        if (str.Length > 0)
        {
            backStr = backStr + "&back_id=" + str;
        }

        if (!(System.Web.HttpContext.Current.Request.QueryString["callerpage"] == null))
        {
            str = AntiXss.UrlEncode(System.Web.HttpContext.Current.Request.QueryString["callerpage"]);
            if (str.Length > 0)
            {
                backStr = backStr + "&back_callerpage=" + str;
            }
        }
        if (!(System.Web.HttpContext.Current.Request.QueryString["origurl"] == null))
        {
            str = System.Web.HttpContext.Current.Request.QueryString["origurl"];
            if (str.Length > 0)
            {
                backStr = backStr + "&back_origurl=" + EkFunctions.UrlEncode(str);
            }
        }
        str = ContentLanguage.ToString();
        if (str.Length > 0)
        {
            backStr = backStr + "&back_LangType=" + str + "&rnd=" + System.Convert.ToInt32(Conversion.Int((10 * VBMath.Rnd()) + 1));
        }

        SRC = (string)("commerce/catalogentry.aspx?close=false&LangType=" + ContentLanguage + "&id=" + Id + "&type=update&" + backStr);
        if (bFromApproval)
        {
            SRC += "&pullapproval=true";
        }

        result = GetButtonEventsWCaption(m_refAPI.AppPath + "images/UI/Icons/contentEdit.png", "#", m_refMsg.GetMessage("alt edit button text"), m_refMsg.GetMessage("btn edit"), "OnClick=\"javascript:top.document.getElementById(\'ek_main\').src=\'" + SRC + "\';return false;\"" + ",\'EDIT\',790,580,1,1);return false;\"", EditButtonCssClass, isPrimary);
        return (result);
    }

	public string GetPageBuilderEditAnchor(long Id, int languageId, string quickLink)
	{
		return GetPageBuilderEditAnchor(Id, languageId, quickLink, false);
	}
    public string GetPageBuilderEditAnchor(long Id, int languageId, string quickLink, bool isPrimary)
    {
        //make popup window with link to this pageids wireframe, and pass in the id and an edit flag
        ContentAPI capi = new ContentAPI();
        FolderData fd = capi.GetFolderById(capi.GetFolderIdForContentId(Id));

        string URL = "";
        Ektron.Cms.API.UrlAliasing.UrlAliasCommon u = new Ektron.Cms.API.UrlAliasing.UrlAliasCommon();
        URL = u.GetAliasForContent(Id);

        if (URL == string.Empty || fd.IsDomainFolder)
        {
            URL = quickLink;
        }

        if (URL.Contains("?"))
        {
            URL = URL + "&ektronPageBuilderEdit=true";
        }
        else
        {
            URL = URL + "?ektronPageBuilderEdit=true";
        }
        if (URL.IndexOf("LangType=") == -1)
        {
            URL = URL + "&LangType=" + languageId.ToString();
        }
        URL = this.m_refAPI.SitePath + URL;
        URL = GetButtonEventsWCaption(m_refAPI.AppImgPath + "layout_edit.gif", "#", m_refMsg.GetMessage("generic edit page layout"), m_refMsg.GetMessage("generic edit page layout"), "OnClick=\"window.open(\'" + URL + "\', \'CMS400EditPage\');return false;\"", EditButtonCssClass, isPrimary);
        return URL;
    }
	
	public string GetPageBuilderEditAnchor(long Id, ContentData contentdata)
	{
		return GetPageBuilderEditAnchor(Id, contentdata, false);
	}

    public string GetPageBuilderEditAnchor(long Id, ContentData contentdata, bool isPrimary)
    {
        //make popup window with link to this pageids wireframe, and pass in the id and an edit flag

        string URL = "";
        if (contentdata.SubType == EkEnumeration.CMSContentSubtype.PageBuilderData || contentdata.SubType == EkEnumeration.CMSContentSubtype.PageBuilderMasterData)
        {
            if (contentdata.ContType == (int)EkEnumeration.CMSContentType.Content || (contentdata.ContType == (int)EkEnumeration.CMSContentType.Archive_Content && contentdata.EndDateAction != 1))
            {
                URL = GetPageBuilderEditAnchor(Id, contentdata.LanguageId, contentdata.Quicklink, isPrimary);
            }
        }
        return URL;
    }

	public string GetEditAnchor(long Id, int Type, bool bFromApproval, EkEnumeration.CMSContentSubtype subType)
	{
		return GetEditAnchor(Id, Type, bFromApproval, subType, false);
	}

    public string GetEditAnchor(long Id, int Type, bool bFromApproval, EkEnumeration.CMSContentSubtype subType, bool isPrimary)
    {
        string result = "";
        string SRC = "";
        string str;
        string backStr;
        if (Type == 3333)
        {
			return GetCatalogEditAnchor(Id, Type, bFromApproval, isPrimary);
        }
        else if (Type == 1)
        {
            if (bFromApproval)
            {
                backStr = "back_file=approval.aspx";
            }
            else
            {
                backStr = "back_file=content.aspx";
            }
        }
        else
        {
            backStr = "back_file=cmsform.aspx";
        }
        str = System.Web.HttpContext.Current.Request.QueryString["action"];
        if (str.Length > 0)
        {
            backStr = backStr + "&back_action=" + str;
        }

        if (bFromApproval)
        {
            if (!string.IsNullOrEmpty(System.Web.HttpContext.Current.Request.QueryString["page"]))
            {
                str = System.Web.HttpContext.Current.Request.QueryString["page"];
                if (str.Length > 0)
                {
                    backStr = backStr + "&back_page=" + str;
                }
            }
        }

        if (!bFromApproval)
        {
            if (!string.IsNullOrEmpty(System.Web.HttpContext.Current.Request.QueryString["folder_id"]))
            {
                str = System.Web.HttpContext.Current.Request.QueryString["folder_id"];
                if (str.Length > 0)
                {
                    backStr = backStr + "&back_folder_id=" + str + "&folderid=" + str;
                }
            }
        }

        if (Type == 1)
        {
            if (!string.IsNullOrEmpty(System.Web.HttpContext.Current.Request.QueryString["id"]))
            {
                str = System.Web.HttpContext.Current.Request.QueryString["id"];
                if (str.Length > 0)
                {
                    backStr = backStr + "&back_id=" + str;
                }
            }
        }
        else
        {
            if (!string.IsNullOrEmpty(System.Web.HttpContext.Current.Request.QueryString["form_id"]))
            {
                str = System.Web.HttpContext.Current.Request.QueryString["form_id"];
                if (str.Length > 0)
                {
                    backStr = backStr + "&back_form_id=" + str;
                }
            }
        }

        if (!string.IsNullOrEmpty(System.Web.HttpContext.Current.Request.QueryString["callerpage"]))
        {
            str = AntiXss.UrlEncode(System.Web.HttpContext.Current.Request.QueryString["callerpage"]);
            if (str.Length > 0)
            {
                backStr = backStr + "&back_callerpage=" + str;
            }
        }


        if (!string.IsNullOrEmpty(System.Web.HttpContext.Current.Request.QueryString["origurl"]))
        {
            str = System.Web.HttpContext.Current.Request.QueryString["origurl"];
            if (str.Length > 0)
            {
                backStr = backStr + "&back_origurl=" + EkFunctions.UrlEncode(str);
            }
        }

        str = ContentLanguage.ToString();
        if (str.Length > 0)
        {
            backStr = backStr + "&back_LangType=" + str + "&rnd=" + System.Convert.ToInt32(Conversion.Int((10 * VBMath.Rnd()) + 1));
        }

        SRC = (string)("edit.aspx?close=false&LangType=" + ContentLanguage + "&id=" + Id + "&type=update&" + backStr);
        if (bFromApproval)
        {
            SRC += "&pullapproval=true";
        }
        if (subType == EkEnumeration.CMSContentSubtype.PageBuilderData)
        {
            SRC += "&menuItemType=editproperties";
        }
        // Fixed #15583 - using FF, new window opens up when editing the form
		result = GetButtonEventsWCaption(m_refAPI.AppPath + "images/UI/Icons/contentEdit.png", "#", m_refMsg.GetMessage("alt edit button text"), m_refMsg.GetMessage("btn edit"), "OnClick=\"javascript:if(undefined != top.document.getElementById(\'ek_main\')){top.document.getElementById(\'ek_main\').src=\'" + SRC + "\';return false;} else {location.href =\'" + SRC + "\';return false;}\"" + ",\'EDIT\',790,580,1,1);return false;\"", EditButtonCssClass, isPrimary);
        return (result);
    }
    public string getCallBackupPage(string defualt)
    {
        string returnValue;
        string tmpStr = "";
        if (System.Web.HttpContext.Current.Request.QueryString["callbackpage"] != null)
        {
            tmpStr = System.Web.HttpContext.Current.Request.QueryString["callbackpage"];
            if (tmpStr == "cmsform.aspx")
            {
                if (!string.IsNullOrEmpty(System.Web.HttpContext.Current.Request.QueryString["fldid"]))
                {
                    tmpStr = tmpStr + "?folder_id=" + System.Web.HttpContext.Current.Request.QueryString["fldid"] + "&";
                }
                else if ((System.Web.HttpContext.Current.Request.QueryString["folder_id"] != "") && System.Web.HttpContext.Current.Request.QueryString["parm1"] != "folder_id" && System.Web.HttpContext.Current.Request.QueryString["parm2"] != "folder_id" && System.Web.HttpContext.Current.Request.QueryString["parm3"] != "folder_id" && System.Web.HttpContext.Current.Request.QueryString["parm4"] != "folder_id")
                {
                    tmpStr = tmpStr + "?folder_id=" + System.Web.HttpContext.Current.Request.QueryString["folder_id"] + "&";
                }
                else
                {
                    tmpStr = tmpStr + "?";
                }
            }
            else
            {
                tmpStr = tmpStr + "?";
            }
            tmpStr = tmpStr + System.Web.HttpContext.Current.Request.QueryString["parm1"] + "=" + System.Web.HttpContext.Current.Request.QueryString["value1"];

            if (System.Web.HttpContext.Current.Request.QueryString["parm2"] != "")
            {
                tmpStr = tmpStr + "&" + System.Web.HttpContext.Current.Request.QueryString["parm2"] + "=" + System.Web.HttpContext.Current.Request.QueryString["value2"];
                if (System.Web.HttpContext.Current.Request.QueryString["parm3"] != "")
                {
                    tmpStr = tmpStr + "&" + System.Web.HttpContext.Current.Request.QueryString["parm3"] + "=" + System.Web.HttpContext.Current.Request.QueryString["value3"];
                    if (System.Web.HttpContext.Current.Request.QueryString["parm4"] != "")
                    {
                        tmpStr = tmpStr + "&" + System.Web.HttpContext.Current.Request.QueryString["parm4"] + "=" + System.Web.HttpContext.Current.Request.QueryString["value4"];
                    }
                }
            }
            returnValue = tmpStr;
        }
        else
        {
            returnValue = defualt;
        }
        return returnValue;
    }
    //This function will pass pack the url paremeter so that they can be passed along
    public string BuildCallBackParms(string leadingChar)
    {
        string returnValue;
        string strTmp2 = "";
        if (System.Web.HttpContext.Current.Request.QueryString["callbackpage"] != "")
        {
            strTmp2 = (string)("callbackpage=" + System.Web.HttpContext.Current.Request.QueryString["callbackpage"] + "&parm1=" + System.Web.HttpContext.Current.Request.QueryString["parm1"] + "&value1=" + System.Web.HttpContext.Current.Request.QueryString["value1"]);
            if (System.Web.HttpContext.Current.Request.QueryString["parm2"] != "")
            {
                strTmp2 = strTmp2 + "&parm2=" + System.Web.HttpContext.Current.Request.QueryString["parm2"] + "&value2=" + System.Web.HttpContext.Current.Request.QueryString["value2"];
                if (System.Web.HttpContext.Current.Request.QueryString["parm3"] != "")
                {
                    strTmp2 = strTmp2 + "&parm3=" + System.Web.HttpContext.Current.Request.QueryString["parm3"] + "&value3=" + System.Web.HttpContext.Current.Request.QueryString["value3"];
                    if (System.Web.HttpContext.Current.Request.QueryString["parm4"] != "")
                    {
                        strTmp2 = strTmp2 + "&parm4=" + System.Web.HttpContext.Current.Request.QueryString["parm4"] + "&value4=" + System.Web.HttpContext.Current.Request.QueryString["value4"];
                    }
                }
            }
            strTmp2 = leadingChar + strTmp2;
            returnValue = strTmp2;
        }
        else
        {
            returnValue = "";
        }
        return returnValue;
    }

    public string getCallingpage(string defualt)
    {
        string returnValue;
        string tmp2 = "";
        if (System.Web.HttpContext.Current.Request.QueryString["callbackpage"] != "")
        {
            tmp2 = System.Web.HttpContext.Current.Request.QueryString["callbackpage"];
            returnValue = tmp2;
        }
        else
        {
            returnValue = defualt;
        }
        return returnValue;
    }

    public string GetHelpButton(string myAlias, string alignString)
    {
        StringBuilder ret = new StringBuilder();
        string linkString = "";
        string debugString = "";

		ret.Append("&nbsp;<a href=\"#\" class=\"" + ContextualHelpButtonCssClass + "\"><img ");
        ret.Append(" id=\"DeskTopHelp\" title= \"" + m_refMsg.GetMessage("alt help button text") + " \"");
        if (alignString.Length > 0)
        {
            ret.Append(" align=\"" + alignString + "\" ");
        }

        ret.Append(" src=\"" + m_refAPI.AppPath + "images/UI/Icons/help.png\" ");
        ret.Append(" onclick=\"PopUpWindow(\'");
        try
        {
            linkString = m_refAPI.fetchhelpLink(myAlias);
            ret.Append(linkString);
        }
        catch (Exception)
        {

        }
        ret.Append("\', \'SitePreview\', 600, 500, 1, 1);");

        if (m_refAPI.Debug_ShowHelpAlias)
        {
            debugString = "if (DebugMsg) DebugMsg(\'Alias = " + myAlias + ", Link = " + linkString + "\');";
            debugString += " else alert(\'Alias = " + myAlias + ", Link = " + linkString + "\');";
        }
        ret.Append(debugString + "return false;\">");
        ret.Append("</a>&nbsp;");
        return ret.ToString();
    }

    public string GetHelpAliasPrefix(FolderData folder_data)
    {
        string returnValue;
        string result = "";
        if (folder_data != null)
        {
            switch ((EkEnumeration.FolderType)Enum.ToObject(typeof(EkEnumeration.FolderType), (int)folder_data.FolderType))
            {
                case Ektron.Cms.Common.EkEnumeration.FolderType.Blog:
                    result = "blog_";
                    break;
                case Ektron.Cms.Common.EkEnumeration.FolderType.Content:
                    result = "";
                    break;
                case Ektron.Cms.Common.EkEnumeration.FolderType.DiscussionBoard:
                    result = "discussionboard_";
                    break;
                case Ektron.Cms.Common.EkEnumeration.FolderType.DiscussionForum:
                    result = "discussionforum_";
                    break;
                case Ektron.Cms.Common.EkEnumeration.FolderType.Domain:
                    result = "";
                    break;
                case Ektron.Cms.Common.EkEnumeration.FolderType.Root:
                    result = "";
                    break;
                default:
                    result = "";
                    break;
            }
        }
        returnValue = result;
        return returnValue;
    }

    public string GetExportTranslationButton(string hrefPath, string altText, string HeaderText)
    {
        StringBuilder result = new StringBuilder();
        try
        {
            if (m_refAPI.IsARoleMember(EkEnumeration.CmsRoleIds.AdminXliff) && Ektron.Cms.DataIO.LicenseManager.LicenseManager.IsFeatureEnable(m_refAPI.RequestInformationRef, Ektron.Cms.DataIO.LicenseManager.Feature.Xliff, false))
            {
				result.Append(GetButtonEventsWCaption(m_refAPI.AppPath + "images/UI/Icons/translation.png", hrefPath, altText, HeaderText, "", ExportTranslationButtonCssClass));
            }
        }
        catch (Exception)
        {
            // ignore
        }
        return result.ToString();
    }

    public bool IsExportTranslationSupportedForContentType(EkEnumeration.CMSContentType ContentType)
    {
        return (ContentType == EkEnumeration.CMSContentType.Content || ContentType == EkEnumeration.CMSContentType.Forms || ContentType == EkEnumeration.CMSContentType.Assets || ContentType == EkEnumeration.CMSContentType.Multimedia || ContentType == EkEnumeration.CMSContentType.NonLibraryContent || ((EkConstants.ManagedAsset_Min <= (int)ContentType && (int)ContentType <= EkConstants.ManagedAsset_Max)) || ContentType == EkEnumeration.CMSContentType.CatalogEntry);
    }

    public void GetTranslationStatusIconAndMessage(Ektron.Cms.Localization.LocalizationState locStatus, ref string statusIcon, ref string status)
    {
        switch (locStatus)
        {
            case Ektron.Cms.Localization.LocalizationState.DoNotTranslate:
                statusIcon = "images/UI/Icons/translationNotTranslatable.png";
                status = m_refMsg.GetMessage("lbl not translatable");
                break;
            case Ektron.Cms.Localization.LocalizationState.NotReady:
                statusIcon = "images/UI/Icons/translationNotReady.png";
                status = m_refMsg.GetMessage("lbl not ready for translation");
                break;
            case Ektron.Cms.Localization.LocalizationState.Ready:
                statusIcon = "images/UI/Icons/translationReady.png";
                status = m_refMsg.GetMessage("lbl ready for translation");
                break;
            case Ektron.Cms.Localization.LocalizationState.NeedsTranslation:
                statusIcon = "images/UI/Icons/translationNeedsTranslation.png";
                status = m_refMsg.GetMessage("lbl needs translation");
                break;
            case Ektron.Cms.Localization.LocalizationState.OutForTranslation:
                statusIcon = "images/UI/Icons/translationOutForTranslation.png";
                status = m_refMsg.GetMessage("lbl out for translation");
                break;
            case Ektron.Cms.Localization.LocalizationState.Translated:
                statusIcon = "images/UI/Icons/translationTranslated.png";
                status = m_refMsg.GetMessage("lbl translated");
                break;
            default:
                statusIcon = "images/UI/Icons/translationNotReady.png";
                status = m_refMsg.GetMessage("lbl not ready for translation");
                break;
        }
    }

    public string GetTranslationStatusMenu(ContentData contentdata, string altText, string HeaderText, Ektron.Cms.Localization.LocalizationState locStatus)
    {
		switch (locStatus)
		{
			case Ektron.Cms.Localization.LocalizationState.DoNotTranslate:
				return GetTranslationStatusMenu(contentdata, altText, HeaderText, locStatus, LocaleDoNotTranslateButtonCssClass);

			case Ektron.Cms.Localization.LocalizationState.NeedsTranslation:
				return GetTranslationStatusMenu(contentdata, altText, HeaderText, locStatus, LocaleNeedsTranslationButtonCssClass);

			case Ektron.Cms.Localization.LocalizationState.NotReady:
				return GetTranslationStatusMenu(contentdata, altText, HeaderText, locStatus, LocaleNotReadyButtonCssClass);

			case Ektron.Cms.Localization.LocalizationState.OutForTranslation:
				return GetTranslationStatusMenu(contentdata, altText, HeaderText, locStatus, LocaleOutForTranslationButtonCssClass);

			case Ektron.Cms.Localization.LocalizationState.Ready:
				return GetTranslationStatusMenu(contentdata, altText, HeaderText, locStatus, LocaleReadyButtonCssClass);

			case Ektron.Cms.Localization.LocalizationState.Translated:
				return GetTranslationStatusMenu(contentdata, altText, HeaderText, locStatus, LocaleTranslatedButtonCssClass);

			default:
				return GetTranslationStatusMenu(contentdata, altText, HeaderText, locStatus, null);
		}
    }

	private string GetTranslationStatusMenu(ContentData contentdata, string altText, string HeaderText, Ektron.Cms.Localization.LocalizationState locStatus, string aTagClassName)
	{
		StringBuilder result = new StringBuilder();
		try
		{
			if ((
                m_refAPI.IsARoleMember(EkEnumeration.CmsRoleIds.AdminTranslationState) 
                ||
                m_refAPI.IsARoleMember(EkEnumeration.CmsRoleIds.AdminXliff)
                )
                && Ektron.Cms.DataIO.LicenseManager.LicenseManager.IsFeatureEnable(m_refAPI.RequestInformationRef, Ektron.Cms.DataIO.LicenseManager.Feature.Xliff, false))
			{
				string statusIcon = string.Empty;
				string status = string.Empty;
				GetTranslationStatusIconAndMessage(locStatus, ref statusIcon, ref status);
				altText = status + " - " + altText;
				string clickEvent = "onclick=\"MenuUtil.use(event, \'translationaction\');\"";
				result.Append(GetButtonEventsWCaption(m_refAPI.AppPath + statusIcon, "#", altText, HeaderText, clickEvent, aTagClassName));
			}
		}
		catch (Exception)
		{
			// ignore
		}
		return result.ToString();
	}

    public string PopupTranslationMenu(ContentData contentdata, Ektron.Cms.Localization.LocalizationState locStatus)
    {
        return PopupTranslationMenu(contentdata, locStatus, string.Empty, string.Empty, string.Empty, true);
    }
    public string PopupTranslationMenu(ContentData contentdata, Ektron.Cms.Localization.LocalizationState locStatus, string addToMenu, string addToMenuMsg, string addToMenuIcon, bool includeScript)
    {
        StringBuilder result = new StringBuilder();
        long intId = contentdata.Id;
        if (contentdata.Type == 3333)
            contentdata.ContType = 3333;
        try
        {
            string menuItem = "    translationActionMenu.addItem(\"&#160;<img src=\'{0}\' />&#160;&#160;{1}\", function() {{ window.location.href = \'content.aspx?LangType=" + ContentLanguage + "&action={2}&backpage=ViewContentByCategory&contType=" + contentdata.ContType + "&id=" + intId + "\'; }} );";
            bool sourceContentMenu = false;
            switch (locStatus)
            {
                case Ektron.Cms.Localization.LocalizationState.DoNotTranslate:
                case Ektron.Cms.Localization.LocalizationState.NotReady:
                case Ektron.Cms.Localization.LocalizationState.Ready:
                    sourceContentMenu = true;
                    break;
                case Ektron.Cms.Localization.LocalizationState.NeedsTranslation:
                case Ektron.Cms.Localization.LocalizationState.OutForTranslation:
                case Ektron.Cms.Localization.LocalizationState.Translated:
                    sourceContentMenu = false;
                    break;
            }
            if (includeScript)
            {
                result.AppendLine("<script type=\"text/javascript\">");
                result.AppendLine("<!--");
            }
            result.AppendLine("    var translationActionMenu = new Menu(\"translationaction\");");
            if (sourceContentMenu)
            {
                result.AppendFormat(menuItem, "images/UI/Icons/translationReady.png",
                    MakeBold(m_refMsg.GetMessage("lbl mark ready for translation"), locStatus == LocalizationState.Ready)
                    , "translatereadystatus").AppendLine();
                result.AppendFormat(menuItem, "images/UI/Icons/translationNotReady.png", 
                    MakeBold(m_refMsg.GetMessage("lbl mark not ready for translation"), locStatus == LocalizationState.NotReady)
                    , "translatenotreadystatus").AppendLine();
                result.AppendFormat(menuItem, "images/UI/Icons/translationNotTranslatable.png",
                    MakeBold(m_refMsg.GetMessage("lbl mark not translatable"), locStatus == LocalizationState.DoNotTranslate)
                    , "translatenotallowstatus").AppendLine();
            }
            else
            {
                result.AppendFormat(menuItem, "images/UI/Icons/translationNeedsTranslation.png", 
                    MakeBold(m_refMsg.GetMessage("lbl mark needs translation"), locStatus == LocalizationState.NeedsTranslation)
                    , "translateneedstranslationstatus").AppendLine();
                result.AppendFormat(menuItem, "images/UI/Icons/translationOutForTranslation.png", 
                    MakeBold(m_refMsg.GetMessage("lbl mark out for translation"), locStatus == LocalizationState.OutForTranslation)
                    , "translateoutfortranslationstatus").AppendLine();
                result.AppendFormat(menuItem, "images/UI/Icons/translationTranslated.png",
                    MakeBold(m_refMsg.GetMessage("lbl mark translated"), locStatus == LocalizationState.Translated)
                    , "translatetranslatedstatus").AppendLine();
            }
            if (string.IsNullOrEmpty(addToMenu))
                result.AppendLine("    MenuUtil.add(translationActionMenu);");
            else
                result.Append("    ").Append(addToMenu).Append(".addMenu(\"&nbsp;<img src=\'" + addToMenuIcon + "\' alt=\\\"" + addToMenuMsg + "\\\" />&nbsp;&nbsp;" + addToMenuMsg + "\", translationActionMenu);" + Environment.NewLine);
            if (includeScript)
            {
                result.AppendLine("// -->");
                result.AppendLine("</script>");
            }
        }
        catch (Exception)
        {
            // ignore
        }
        return result.ToString();
    }
    private string MakeBold(string message, bool bold)
    {
        if (bold)
            return "<strong>" + message + "</strong>";
        else
            return message;
    }

    #region Calendar Enhancement Functions added April, 2005 by Doug Glennon
    public string ShowActiveLangsInList(bool showAllOpt, string bgColor, string OnChangeEvt, int SelLang, string includeList)
    {
        // This function works like "ShowAllActiveLanguage" except it accepts a comma-seperated
        // string value of Language IDs. Use this for showing a drop-down of languages that
        // a piece of content, or other item, has been translated to.
        //
        // For example, if your active languages are French, German and English and you have a
        // piece of content that is translated in English and French already but not German, you would
        // call ShowActiveLangsInList(showAllOpt, bgColor, OnChangeEvt, SelLang, "1033,1036"). This function
        // will go to the DB, determine what 1033 and 1036 is and display them in a drop down, but would
        // not show German.
        //
        return ShowFilteredLangs(showAllOpt, bgColor, OnChangeEvt, SelLang, includeList, false);
    }
    public string ShowActiveLangsNotInList(bool showAllOpt, string bgColor, string OnChangeEvt, int SelLang, string excludelist)
    {
        // This function works like ShowActiveLangsInList, however, it does the opposite. It
        // shows all languages in the drop-down that are not in the excludelist. This way you
        // can show a drop-down of all the languages for which a piece of content, or other item,
        // has not yet been translated.
        //
        // For example, if your active languages are French, German and English and you have a
        // piece of content that is translated in English and French already but not German, you would
        // call ShowActiveLangsNotInList(showAllOpt, bgColor, OnChangeEvt, SelLang, "1033,1036"). This function
        // will go to the DB, get all the active languages and build a drop-down without 1033 and 1036 in it, thereby
        // leaving only German (and "All" if available).
        //
        return ShowFilteredLangs(showAllOpt, bgColor, OnChangeEvt, SelLang, excludelist, true);
    }

    private string ShowFilteredLangs(bool showAllOpt, string bgColor, string OnChangeEvt, int SelLang, string csvLangList, bool ExcludeLangsInList)
    {
        // See "ShowActiveLangsInList" and "ShowActiveLangsNotInList"
        StringBuilder sbResult = new StringBuilder();
        int[] aryLangFilterList;
        SiteAPI refSiteApi = new SiteAPI();
        int nContentLanguage;

        if (m_refAPI.EnableMultilingual == 1)
        {
            nContentLanguage = SelLang;
            if (Ektron.Cms.Common.EkConstants.CONTENT_LANGUAGES_UNDEFINED == nContentLanguage)
            {
                nContentLanguage = refSiteApi.ContentLanguage;
            }

            if (OnChangeEvt == "")
            {
                OnChangeEvt = "SelLanguage(this.value)";
            }

            string[] aryLangList;
            aryLangList = csvLangList.Split(",".ToCharArray());
            if (aryLangList.Length > 0)
            {
                aryLangFilterList = new int[aryLangList.Length - 1 + 1];
                for (int iLang = 0; iLang <= aryLangList.Length - 1; iLang++)
                {
                    string strLang = aryLangList[iLang].Trim();
                    if (Information.IsNumeric(strLang))
                    {
                        aryLangFilterList[iLang] = Convert.ToInt32(strLang);
                    }
                    else
                    {
                        aryLangFilterList[iLang] = Ektron.Cms.Common.EkConstants.CONTENT_LANGUAGES_UNDEFINED;
                    }
                }
            }
            else
            {
                if (ExcludeLangsInList)
                {
                    aryLangFilterList = null;
                }
                else
                {
                    aryLangFilterList = new int[1];
                    aryLangFilterList[0] = nContentLanguage;
                }
            }

            sbResult.Append("<select id=\"frm_langID\" name=\"frm_langID\" OnChange=\"" + OnChangeEvt + "\">" + "\r\n");

            if (showAllOpt)
            {
                if (Ektron.Cms.Common.EkConstants.ALL_CONTENT_LANGUAGES == nContentLanguage)
                {
                    sbResult.Append("<option value=\"" + Ektron.Cms.Common.EkConstants.ALL_CONTENT_LANGUAGES + "\" selected=\"selected\">All</option>");
                }
                else
                {
                    sbResult.Append("<option value=\"" + Ektron.Cms.Common.EkConstants.ALL_CONTENT_LANGUAGES + "\">All</option>");
                }
            }

            if (ExcludeLangsInList)
            {
                sbResult.Append("<option value=\"0\">Select a Language</option>");
            }

            LanguageData[] aryLangData;
            aryLangData = refSiteApi.GetAllActiveLanguages();

            for (int iLang = 0; iLang <= aryLangData.Length - 1; iLang++)
            {
                LanguageData with_1 = aryLangData[iLang];
                if (with_1.Id != Ektron.Cms.Common.EkConstants.CONTENT_LANGUAGES_UNDEFINED)
                {
                    if (IsInList(with_1.Id, aryLangFilterList) ^ ExcludeLangsInList)
                    {
                        sbResult.Append("<option value=\"" + with_1.Id + "\"");
                        if (with_1.Id == nContentLanguage)
                        {
                            sbResult.Append(" selected=\"selected\"");
                        }
                        else
                        {
                        }
                        sbResult.Append(">");
                        sbResult.Append(with_1.LocalName);
                        sbResult.Append("</option>");
                    }
                }
            }
            sbResult.Append("</select>");
        }

        return sbResult.ToString();
    }

    private bool IsInList(int Value, int[] ValueList)
    {
        if (ValueList == null)
        {
            return false;
        }
        for (int iItem = 0; iItem <= ValueList.Length - 1; iItem++)
        {
            if (Value == ValueList[iItem])
            {
                return true;
            }
        }
        return false;
    }

    public string enabledIcon(bool inEnabled)
    {
        string strRet;

        if (inEnabled)
        {
            strRet = "<img src=\"" + m_refAPI.AppPath + "images/UI/Icons/check.png\" alt=\"Enabled\" title=\"Enabled\">";
        }
        else
        {
            strRet = "<img src=\"" + m_refAPI.AppImgPath + "icon_redx.gif\" alt=\"Disabled\" title=\"Disabled\">";
        }

        return (strRet);
    }
    public string inputTag(string tagName, string tagValue, int tagSize)
    {
        return ("<input type=\"text\" name=\"" + tagName + "\" value=\"" + tagValue + "\" size=\"" + tagSize + "\"/>");
    }
    public string inputTagHidden(string tagName, string tagValue)
    {
        return ("<input type=\"hidden\" name=\"" + tagName + "\" value=\"" + tagValue + "\"/>");
    }
    public string inputTagCheckbox(string tagName, bool isChecked, string checkedValue)
    {
        if (isChecked)
        {
            return ("<input type=\"checkbox\" name=\"" + tagName + "\" value=\"" + checkedValue + "\" checked=\"true\"/>");
        }
        else
        {
            return ("<input type=\"checkbox\" name=\"" + tagName + "\" value=\"" + checkedValue + "\"/>");
        }
    }

    #endregion

    #region Catalog
    public string GetCatalogAddAnchor(long Id)
    {
        string sResult = "";
        try
        {
            sResult = (string)("AddNewContent(\'LangType=" + ContentLanguage + "&type=add&createtask=1&id=" + Id + "&" + GetBackParams());
            sResult += "&AllowHTML=1";
            sResult += "\', 3333);";
        }
        catch (Exception)
        {
            sResult = "";
        }
        return sResult;
    }

    public string GetCatalogAddAnchorType(long Id, long xml_id)
    {
        string sResult = "AddNewContent(\'LangType=" + ContentLanguage + "&type=add&createtask=1&id=" + Id + "&xid=" + xml_id + "&" + GetBackParams() + "\',3333); ";
        return sResult;
    }
    #endregion

}