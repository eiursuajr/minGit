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
using Ektron.Cms.API;
using Ektron.Cms.Common;

public partial class meta_data50 : System.Web.UI.Page
{

    #region Member Variables

    protected StyleHelper _StyleHelper;
    protected SearchHelper _SearchHelper;
    protected ContentAPI _ContentApi;
    protected string _ApplicationName;
    protected string _ApplicationPath;
    protected string _SitePath;
    protected EkMessageHelper _MessageHelper;
    protected int ContentLanguage;
    object CurrentUserId;
    protected string ErrorString;
    protected Collection cMetadata;
    protected string action = "";
    protected string EnableMultilingual;
    protected Collection cMetaType;
    string AppImgPath = "";
    Ektron.Cms.CustomFields cfo = new Ektron.Cms.CustomFields();

    protected SiteAPI AppUI = new SiteAPI();
    protected SiteAPI siteRef = new SiteAPI();
    protected SiteAPI sfSiteRef = new SiteAPI();

    protected const int ALL_CONTENT_LANGUAGES = -1;
    protected const int MetaTagType_Html = 0;
    protected const int MetaTagType_Meta = 1;
    protected const int MetaTagType_Collection = 2;
    protected const int MetaTagType_ListSummary = 3;
    protected const int MetaTagType_Content = 4;
    protected const int MetaTagType_Image = 5;
    protected const int MetaTagType_HyperLink = 6;
    protected const int MetaTagType_File = 7;
    protected const int MetaTagType_Menu = 8;
    protected const int MetaTagType_User = 9;
    protected const int MetaTagType_Searchable = 100;
    protected const int STANDARD_PROP = 0;
    protected const int DMS_PROP = 1;
    protected const int CUSTOM_PROP = 2;
    protected const string TEXT_PROP = "text";
    protected const string NUMBER_PROP = "number";
    protected const string BYTE_PROP = "byte";
    protected const string DOUBLE_PROP = "double";
    protected const string FLOAT_PROP = "float";
    protected const string INTEGER_PROP = "integer";
    protected const string LONG_PROP = "long";
    protected const string SHORT_PROP = "short";
    protected const string DATE_PROP = "date";
    protected const string SELECT_PROP = "select";
    protected const string SELECT1_PROP = "select1";
    protected const string BOOLEAN_PROP = "boolean";

    protected bool bView = false;
    protected string strHtml = "";
    protected bool bMetaCaseSensitive = false;
    protected string strMetaDefault = null;
    protected string strMetaNameTitle = null;
    protected bool bMetaRemoveDuplicates = false;
    protected string strMetaSeparator = null;
    protected bool bSelectableOnly = false;
    protected bool bAllowMulti = false;
    protected string strMetaSelectableText = null;
    protected long id = 0;
    protected bool bNew = false;
    protected string strAction = "";
    #endregion

    #region Events

    public meta_data50()
    {

        _StyleHelper = new StyleHelper();
        _SearchHelper = new SearchHelper();
        _ContentApi = new ContentAPI();
        _ApplicationName = _ContentApi.AppName;
        _ApplicationPath = _ContentApi.AppPath;
        _SitePath = _ContentApi.SitePath;
        _MessageHelper = _ContentApi.EkMsgRef;
        AppImgPath = siteRef.AppImgPath;
        CurrentUserId = siteRef.UserId;
        EnableMultilingual = siteRef.EnableMultilingual.ToString();

    }

    private void Page_Init(System.Object sender, System.EventArgs e)
    {

        RegisterJS();
        RegisterCSS();

    }

    private void Page_Load(System.Object sender, System.EventArgs e)
    {
        if ((_ContentApi.EkContentRef).IsAllowed(0, 0, "users", "IsLoggedIn", _ContentApi.RequestInformationRef.CallerId) == false)
        {
            Response.Redirect(_ContentApi.AppPath + "login.aspx?fromLnkPg=1", true);
            return;
        }
        if ((_ContentApi.RequestInformationRef.IsMembershipUser == 1 || !_ContentApi.IsARoleMember(Ektron.Cms.Common.EkEnumeration.CmsRoleIds.AdminMetadata)) && !_ContentApi.IsAdmin())
        {
            Response.Redirect((string)("reterror.aspx?info=" + _MessageHelper.GetMessage("msg login metadata administrator")), true);
            return;
        }
        this.Title = _ApplicationName + " " + _MessageHelper.GetMessage("meta_data page html title");
        if (!string.IsNullOrEmpty(Request.QueryString["LangType"]))
        {
            ContentLanguage = Convert.ToInt32(Request.QueryString["LangType"]);
            siteRef.SetCookieValue("LastValidLanguageID", ContentLanguage.ToString());
        }
        else
        {
            if (!string.IsNullOrEmpty(Convert.ToString(siteRef.GetCookieValue("LastValidLanguageID"))))
            {
                ContentLanguage = Convert.ToInt32(siteRef.GetCookieValue("LastValidLanguageID"));
            }
        }
        siteRef.ContentLanguage = ContentLanguage;
        StringBuilder sb = new StringBuilder();

        action = Request.QueryString["action"];
        if (action == "SubmitMetaDefinition" || action == "DeleteMetaDefinition" || action == "UpdateMetaDefinition")
        {
            try
            {
                Collection cMeta;
                if ("SubmitMetaDefinition" == action || "UpdateMetaDefinition" == action)
                {
                    cMeta = new Collection();
                    cMeta.Add(Request.Form["frm_metatypeid"], "MetaTypeID", null, null);
                    cMeta.Add(Request.Form["frm_metatypename"], "MetaTypeName", null, null);
                    cMeta.Add(Request.Form["frm_metatagtype"], "MetaTagType", null, null);
                    cMeta.Add(Request.Form["frm_metanametitle"], "MetaNameTitle", null, null);
                    cMeta.Add(Request.Form["frm_metaseparator"], "MetaSeparator", null, null);
                    cMeta.Add(Request.Form["frm_metadefault"], "MetaDefault", null, null);

                    if (Strings.LCase(Request.Form["frm_metaeditable"]) == "on")
                    {
                        cMeta.Add(true, "MetaEditable", null, null);
                    }
                    else
                    {
                        cMeta.Add(false, "MetaEditable", null, null);
                    }

                    if (Strings.LCase(Request.Form["frm_selectable_only"]) == "on")
                    {
                        cMeta.Add(true, "SelectableOnly", null, null);
                        if (Strings.LCase(Request.Form["frm_allow_multi"]) == "on")
                        {
                            cMeta.Add(true, "AllowMulti", null, null);
                        }
                        else
                        {
                            cMeta.Add(false, "AllowMulti", null, null);
                        }
                    }
                    else
                    {
                        cMeta.Add(false, "SelectableOnly", null, null);
                        cMeta.Add(false, "AllowMulti", null, null);
                    }

                    // Keep select list text even if not used in case the user re-enables it.
                    cMeta.Add(Request.Form["frm_MetaSelectableText"], "MetaSelectableText", null, null);
                    cMeta.Add(false, "MetaRequired", null, null);
                    if (Strings.LCase(Request.Form["frm_metaremoveduplicates"]) == "on")
                    {
                        cMeta.Add(true, "MetaRemoveDuplicates", null, null);
                    }
                    else
                    {
                        cMeta.Add(false, "MetaRemoveDuplicates", null, null);
                    }
                    if (Strings.LCase(Request.Form["frm_metacasesensitive"]) == "on")
                    {
                        cMeta.Add(true, "MetaCaseSensitive", null, null);
                    }
                    else
                    {
                        cMeta.Add(false, "MetaCaseSensitive", null, null);
                    }
                    if ("on" == Strings.LCase(Request.Form["frm_metaallowsearch"]))
                    {
                        cMeta.Add(true, "MetaAllowSearch", null, null);
                    }
                    else
                    {
                        cMeta.Add(false, "MetaAllowSearch", null, null);
                    }

                    if ("SubmitMetaDefinition" == action)
                    {
                        _ContentApi.EkContentRef.AddMetadataType(ref cMeta);
                    }
                    else
                    {
                        // Add info for data conversion:
                        if (Request.Form["frm_original_data_style"] != null && Request.Form["frm_target_data_style"] != null && Request.Form["frm_metadata_update_method"] != null)
                        {
                            cMeta.Add(Request.Form["frm_original_data_style"], "SrcDataType", null, null);
                            cMeta.Add(Request.Form["frm_target_data_style"], "TargDataType", null, null);
                            cMeta.Add(Request.Form["frm_metadata_update_method"], "UpdateMethod", null, null);
                        }

                        // check if nothing was edited and only do update if something was changed
                        // otherwise, we waste time doing a searchserver full crawl
                        id = Convert.ToInt64(Request.Form["frm_metatypeid"]);
                        cMetaType = siteRef.EkContentRef.GetMetadataTypeByID(id);
                        if ((cMetaType["MetaTypeName"].ToString() != Request.Form["frm_metatypename"]) ||
                            (cMetaType["MetaTagType"].ToString() != Request.Form["frm_metatagtype"]) ||
                            (cMetaType["MetaNameTitle"].ToString() != Request.Form["frm_metanametitle"]) ||
                            (cMetaType["MetaSeparator"].ToString() != Request.Form["frm_metaseparator"]) ||
                            (cMetaType["MetaDefault"].ToString() != Request.Form["frm_metadefault"]) ||
                            (((int)cMetaType["MetaEditable"] != 0) != (Strings.LCase(Request.Form["frm_metaeditable"]) == "on")) ||
                            (((int)cMetaType["AllowMulti"] != 0) != (Strings.LCase(Request.Form["frm_allow_multi"]) == "on")) ||
                            (((int)cMetaType["SelectableOnly"] != 0) != (Strings.LCase(Request.Form["frm_selectable_only"]) == "on")) ||
                            (((int)cMetaType["MetaRemoveDuplicates"] != 0) != (Strings.LCase(Request.Form["frm_metaremoveduplicates"]) == "on")) ||
                            (((int)cMetaType["MetaCaseSensitive"] != 0) != (Strings.LCase(Request.Form["frm_metacasesensitive"]) == "on")) ||
                            (((int)cMetaType["MetaAllowSearch"] != 0) != (Strings.LCase(Request.Form["frm_metaallowsearch"]) == "on")) ||
                            (cMetaType["MetaSelectableText"].ToString() != Request.Form["frm_MetaSelectableText"]))
                        {
                            _ContentApi.EkContentRef.UpdateMetadataTypeByID(cMeta);
                        }
                    }

                    if ("SubmitMetaDefinition" == action)
                    {
                        Response.Redirect("meta_data50.aspx?LangType=" + _ContentApi.ContentLanguage.ToString() + "&action=ViewAllMetaDefinitions", false);
                    }
                    else
                    {
                        Response.Redirect((string)("meta_data50.aspx?LangType=" + _ContentApi.ContentLanguage.ToString() + "&action=ViewMetaDefinition&id=" + Request.Form["frm_metatypeid"]), false);
                    }

                }
                else if (action == "DeleteMetaDefinition")
                {
                    _ContentApi.EkContentRef.DeleteMetadataType(System.Convert.ToInt64(Request.QueryString["id"]));
                    Response.Redirect("meta_data50.aspx?LangType=" + _ContentApi.ContentLanguage.ToString() + "&action=ViewAllMetaDefinitions", false);
                }

            }
            catch (Exception ex)
            {
                if (ex.Message.ToLower().Contains("no endpoint listening"))
                    Response.Redirect(_ContentApi.ApplicationPath + "reterror.aspx?info=" + Server.UrlEncode(_MessageHelper.GetMessage("error message ews not running")), true);
                else
                    Response.Redirect(_ContentApi.ApplicationPath + "reterror.aspx?info=" + Server.UrlEncode(ex.Message), true);
            }
        }
        else
        {
            if (action == "ViewAllMetaDefinitions")
            {
                pnlAddEditViewDef.Visible = false;
                if (EnableMultilingual == "1")
                {
                    if ((siteRef.ContentLanguage == ALL_CONTENT_LANGUAGES) || (siteRef.ContentLanguage == Ektron.Cms.Common.EkConstants.CONTENT_LANGUAGES_UNDEFINED))
                        siteRef.ContentLanguage = siteRef.DefaultContentLanguage;

                }
                cMetadata = siteRef.EkContentRef.GetMetadataTypes("Name");
                sb.Append(_StyleHelper.GetButtonEventsWCaption(siteRef.AppPath + "images/UI/Icons/add.png", "meta_data50.aspx?action=AddMetaDefinition&LangType=" + siteRef.ContentLanguage, _MessageHelper.GetMessage("alt add button text (metadata type)"), _MessageHelper.GetMessage("btn add metadata"), "", StyleHelper.AddButtonCssClass, true)).Append(Environment.NewLine);
                if (EnableMultilingual == "1")
                {
                    sb.Append(StyleHelper.ActionBarDivider);
                    sb.Append("<td>");
                    sb.Append(_MessageHelper.GetMessage("view in label") + ": " + _StyleHelper.ShowAllActiveLanguage(false, "#d8e6ff", "", siteRef.ContentLanguage.ToString())).Append(Environment.NewLine);
                    sb.Append("</td>").Append(Environment.NewLine);
                }
                sb.Append(StyleHelper.ActionBarDivider);
                sb.Append("<td>");
                sb.Append(_StyleHelper.GetHelpButton(action, ""));
                sb.Append("</td>");
                ltrToolbar.Text = sb.ToString();

                //ltrlist popuation
                sb = new StringBuilder();
                foreach (Collection temp_varCMetadata in cMetadata)
                {
                    if (((EkEnumeration.BlogPostDataType)temp_varCMetadata["ObjectType"]) == EkEnumeration.BlogPostDataType.Normal)
                    {
                        sb.Append("<tr>").Append(Environment.NewLine);
                        sb.Append("  <td>").Append(Environment.NewLine);
                        sb.Append("    <a href=\"meta_data50.aspx?action=ViewMetaDefinition&id=").Append(temp_varCMetadata["MetaTypeID"]).Append("&LangType=").Append(temp_varCMetadata["MetaLanguage"]).Append("\" title=").Append(_MessageHelper.GetMessage("view meta definition msg2")).Append(">").Append(Environment.NewLine);
                        sb.Append(temp_varCMetadata["MetaTypeName"]).Append(Environment.NewLine);
                        sb.Append("</a>").Append(Environment.NewLine);
                        sb.Append("</td>").Append(Environment.NewLine);
                        sb.Append("<td align=\"center\">").Append(temp_varCMetadata["MetaTypeID"]).Append("</td>").Append(Environment.NewLine);
                        sb.Append("<td>").Append(temp_varCMetadata["MetaDefault"]).Append("</td>");
                        sb.Append("</tr>").Append(Environment.NewLine);
                    }

                }
                ltrList.Text = sb.ToString();

            } // End ViewAllMetaDefinitions
            else if (action == "AddMetaDefinition" || action == "EditMetaDefinition" || action == "ViewMetaDefinition")
            {
                pnlAddEditViewDef.Visible = true;
                pnlViewAllDefs.Visible = false;
                bool bEdit = false;

                string strTitleMsg = null;
                string strQDWarning = null;

                bNew = ("AddMetaDefinition" == action);
                bEdit = ("EditMetaDefinition" == action);
                bView = ("ViewMetaDefinition" == action);
                if (bNew)
                {
                    strTitleMsg = _MessageHelper.GetMessage("add metadata definition msg");
                    strQDWarning = _MessageHelper.GetMessage("add metadata qdwarning msg");
                }
                else
                {
                    id = Convert.ToInt64(Request.QueryString["id"]);
                    if (bEdit)
                    {
                        strTitleMsg = _MessageHelper.GetMessage("edit metadata definition msg");
                    }
                    else if (bView)
                    {
                        strTitleMsg = _MessageHelper.GetMessage("view meta definition msg");
                    }
                }


                cMetaType = siteRef.EkContentRef.GetMetadataTypeByID(id);
                string strMetaTypeName = null;
                long nMetaTagType = -1;
                bool bMetaEditable = false;
                bool bMetaDisplayEE = false;
                bool nMetaAllowSearch = false;

                if (bNew)
                {
                    strAction = "SubmitMetaDefinition";
                    strMetaTypeName = "";
                    strMetaNameTitle = "";
                    nMetaTagType = MetaTagType_Searchable;
                    strMetaSeparator = ";";
                    bMetaEditable = true;
                    bMetaDisplayEE = false;
                    bMetaRemoveDuplicates = true;
                    bMetaCaseSensitive = false;
                    bSelectableOnly = false;
                    bAllowMulti = false;
                    strMetaSelectableText = "";
                    strMetaDefault = "";
                    nMetaAllowSearch = true;
                }
                else
                {
                    strAction = "UpdateMetaDefinition";
                    strMetaTypeName = cMetaType["MetaTypeName"].ToString();
                    strMetaNameTitle = cMetaType["MetaNameTitle"].ToString().ToLower();
                    nMetaTagType = Convert.ToInt64(cMetaType["MetaTagType"]);
                    if ((nMetaTagType == -1))
                    {
                        nMetaTagType = 1;
                    }
                    strMetaSeparator = cMetaType["MetaSeparator"].ToString();
                    bMetaEditable = Convert.ToBoolean(cMetaType["MetaEditable"]);
                    bMetaDisplayEE = Convert.ToBoolean(cMetaType["MetaDisplayEE"]);
                    bMetaRemoveDuplicates = Convert.ToBoolean(cMetaType["MetaRemoveDuplicates"]);
                    bMetaCaseSensitive = Convert.ToBoolean(cMetaType["MetaCaseSensitive"]);
                    bSelectableOnly = Convert.ToBoolean(cMetaType["SelectableOnly"]);
                    bAllowMulti = Convert.ToBoolean(cMetaType["AllowMulti"]);
                    strMetaSelectableText = cMetaType["MetaSelectableText"].ToString();
                    if (Strings.InStr(1, strMetaSelectableText, strMetaSeparator + " ", 0) == 0)
                    {
                        strMetaSelectableText = strMetaSelectableText.Replace(strMetaSeparator, strMetaSeparator);
                    }
                    strMetaDefault = cMetaType["MetaDefault"].ToString();
                    nMetaAllowSearch = Convert.ToBoolean(cMetaType["MetaAllowSearch"]);

                    strTitleMsg = strTitleMsg + " \"" + strMetaTypeName + "\"";
                }
                ltrTitle.Text = strTitleMsg;
                if (!bView)
                    JS.RegisterJS(this, _ContentApi.ApplicationPath + "java/optiontransfer.js", "EktronOptionTransferJS");

                //Toolbar
                sb = new StringBuilder();
                sb.Append("<table>").Append(Environment.NewLine);
                sb.Append("   <tr>").Append(Environment.NewLine);
                if (bNew)
                {
                    sb.Append(_StyleHelper.GetButtonEventsWCaption(siteRef.AppPath + "images/UI/Icons/back.png", "meta_data50.aspx?action=ViewAllMetaDefinitions&LangType=" + ContentLanguage, _MessageHelper.GetMessage("alt back button text"), _MessageHelper.GetMessage("btn back"), "", StyleHelper.BackButtonCssClass, true));
                    sb.Append(_StyleHelper.GetButtonEventsWCaption(siteRef.AppPath + "images/UI/Icons/save.png", "#", _MessageHelper.GetMessage("alt add button text (metadata type2)"), _MessageHelper.GetMessage("btn save"), "Onclick=\"return SubmitForm('metadefinition', 'VerifyMetaForm()');\"", StyleHelper.SaveButtonCssClass, true));
                }
                else if (bEdit)
                {
                    sb.Append(_StyleHelper.GetButtonEventsWCaption(siteRef.AppPath + "images/UI/Icons/back.png", "meta_data50.aspx?LangType=" + ContentLanguage + "&action=ViewMetaDefinition&id=" + id + "", _MessageHelper.GetMessage("alt back button text"), _MessageHelper.GetMessage("btn back"), "", StyleHelper.BackButtonCssClass, true));
                    sb.Append(_StyleHelper.GetButtonEventsWCaption(siteRef.AppPath + "images/UI/Icons/save.png", "#", _MessageHelper.GetMessage("alt update button text (metadata type)"), _MessageHelper.GetMessage("btn update"), "Onclick=\"return SubmitForm('metadefinition', 'VerifyMetaForm()');\"", StyleHelper.SaveButtonCssClass, true));
                }
                else if (bView)
                {
                    string _stringToCheck = " DateCreated , DateModified , GoLiveDate , ExpiryDate , ExpiryType , TaxCategory , ContentID , ContentLanguage , ContentType , FolderId , QuickLink , FolderName , MapLongitude , MapLatitude , MapAddress , EDescription , MetaInfo , CMSPath , CMSSize , InPerm , Searchable , MapDate ";
                    System.Text.RegularExpressions.Match _match = Regex.Match(_stringToCheck.ToLower(), "\\b" + cMetaType["MetaTypeName"].ToString().ToLower() + "\\b");
                    sb.Append(_StyleHelper.GetButtonEventsWCaption(siteRef.AppPath + "images/UI/Icons/back.png", "meta_data50.aspx?LangType=" + ContentLanguage + "&action=ViewAllMetaDefinitions", _MessageHelper.GetMessage("alt back button text"), _MessageHelper.GetMessage("btn back"), "", StyleHelper.BackButtonCssClass, true));
                    if (!_match.Success)
                    {
                        sb.Append(_StyleHelper.GetButtonEventsWCaption(siteRef.AppPath + "images/UI/Icons/contentEdit.png", "meta_data50.aspx?LangType=" + ContentLanguage + "&action=EditMetaDefinition&id=" + id + "", _MessageHelper.GetMessage("alt edit button text (metadata type)"), _MessageHelper.GetMessage("btn edit"), "", StyleHelper.EditButtonCssClass, true));
                        sb.Append(_StyleHelper.GetButtonEventsWCaption(siteRef.AppPath + "images/UI/Icons/delete.png", "meta_data50.aspx?LangType=" + ContentLanguage + "&action=DeleteMetaDefinition&id=" + id + "", _MessageHelper.GetMessage("alt delete button text (metadata type)"), _MessageHelper.GetMessage("btn delete"), "OnClick=\"return ConfirmDelete();\"", StyleHelper.DeleteButtonCssClass));
                    }
                }
                sb.Append(StyleHelper.ActionBarDivider);
                sb.Append("   <td>").Append(Environment.NewLine);
                sb.Append(_StyleHelper.GetHelpButton(action, "")).Append(Environment.NewLine);
                sb.Append("   </td>").Append(Environment.NewLine);
                sb.Append("  </tr>").Append(Environment.NewLine);
                sb.Append("</table>").Append(Environment.NewLine);
                ltrToolbar2.Text = sb.ToString();

                // End Toolbar

                sb = new StringBuilder();
                sb.Append("<table class=\"ektronGrid\">").Append(Environment.NewLine);
                if (bNew && _ContentApi.RequestInformationRef.EnableReplication)
                {
                    sb.Append("<tr><td colspan=\"2\"><i><b>").Append(Environment.NewLine);
                    sb.Append(strQDWarning).Append(Environment.NewLine);
                    sb.Append("</b></i></td></tr>").Append(Environment.NewLine);
                }
                sb.Append(" <tr>").Append(Environment.NewLine);
                sb.Append("   <td class=\"label\">").Append(Environment.NewLine);
                sb.Append("      <label for=\"MetaTypeName\" title=\"Name\">").Append(_MessageHelper.GetMessage("name label")).Append("</label>");
                sb.Append("   </td>").Append(Environment.NewLine);
                if (bView)
                {
                    sb.Append("<td>").Append(Environment.NewLine);
                    sb.Append(strMetaTypeName).Append(Environment.NewLine);
                    sb.Append("</td>").Append(Environment.NewLine);
                }
                else
                {
                    sb.Append("<td>").Append(Environment.NewLine);
                    sb.Append("<input type=\"text\" title=\"Enter Name here\" id=\"MetaTypeName\" name=\"frm_metatypename\" size=\"50\" maxlength=\"255\" value=\"").Append(strMetaTypeName).Append("\" />").Append(Environment.NewLine);
                    sb.Append("</td>").Append(Environment.NewLine);
                }
                sb.Append("</tr>").Append(Environment.NewLine);
                if (!bNew)
                {
                    sb.Append("<tr>").Append(Environment.NewLine);
                    sb.Append("   <td class=\"label\" title=\"ID\">").Append(Environment.NewLine);
                    sb.Append(_MessageHelper.GetMessage("id label")).Append(Environment.NewLine);
                    sb.Append("   </td>").Append(Environment.NewLine);
                    sb.Append("   <td title=\"").Append(cMetaType["MetaTypeID"]).Append("\">").Append(Environment.NewLine);
                    sb.Append(cMetaType["MetaTypeID"]).Append(Environment.NewLine);
                    sb.Append("   </td>").Append(Environment.NewLine);
                    sb.Append("</tr>").Append(Environment.NewLine);
                }
                sb.Append("<tr>").Append(Environment.NewLine);
                sb.Append("   <td class=\"label\">").Append(Environment.NewLine);
                sb.Append("      <label for=\"MetaTagName\" title=\"Type\">").Append(_MessageHelper.GetMessage("type label")).Append("</label>");
                sb.Append("   </td>").Append(Environment.NewLine);
                if (bView)
                {
                    sb.Append("<td>").Append(Environment.NewLine);
                    switch (nMetaTagType)
                    {
                        case MetaTagType_Searchable:
                            sb.Append("Searchable Property");
                            break;
                        case MetaTagType_Meta:
                            sb.Append(_MessageHelper.GetMessage("generic Meta Tag"));
                            break;
                        case MetaTagType_Html:
                            sb.Append(_MessageHelper.GetMessage("generic HTML Tag"));
                            break;
                        case MetaTagType_Collection:
                            sb.Append(_MessageHelper.GetMessage("lbl Collection Selector"));
                            break;
                        case MetaTagType_ListSummary:
                            sb.Append(_MessageHelper.GetMessage("lbl ListSummary Selector"));
                            break;
                        case MetaTagType_Content:
                            sb.Append(_MessageHelper.GetMessage("lbl Content Selector"));
                            break;
                        case MetaTagType_Image:
                            sb.Append(_MessageHelper.GetMessage("lbl Image Selector"));
                            break;
                        case MetaTagType_HyperLink:
                            sb.Append(_MessageHelper.GetMessage("lbl Hyperlink Selector"));
                            break;
                        case MetaTagType_File:
                            sb.Append(_MessageHelper.GetMessage("lbl File Selector"));
                            break;
                        case MetaTagType_Menu:
                            sb.Append(_MessageHelper.GetMessage("lbl menu selector"));
                            break;
                        case MetaTagType_User:
                            sb.Append(_MessageHelper.GetMessage("lbl user selector"));
                            break;
                    }
                    sb.Append("</td>").Append(Environment.NewLine);
                }
                else
                {
                    sb.Append("<td>").Append(Environment.NewLine);
                    sb.Append("<select id=\"MetaTagType\" name=\"frm_MetaTagType\" size=\"1\" onchange=\"OnChangeMetaTagType(this)\">").Append(Environment.NewLine);
                    sb.Append(" <option ").Append(_SearchHelper.SelectedValueAttr(MetaTagType_Searchable, nMetaTagType)).Append(" title=\"Searchable Property\">").Append(_MessageHelper.GetMessage("lbl Searchable Property")).Append("</option>").Append(Environment.NewLine);
                    sb.Append(" <option ").Append(_SearchHelper.SelectedValueAttr(MetaTagType_Meta, nMetaTagType)).Append(" title=\"Meta Tag\">").Append(_MessageHelper.GetMessage("generic Meta Tag")).Append("</option>").Append(Environment.NewLine);
                    sb.Append(" <option ").Append(_SearchHelper.SelectedValueAttr(MetaTagType_Html, nMetaTagType)).Append(" title=\"HTML Tag\">").Append(_MessageHelper.GetMessage("generic HTML Tag")).Append("</option>").Append(Environment.NewLine);
                    sb.Append(" <option ").Append(_SearchHelper.SelectedValueAttr(MetaTagType_Collection, nMetaTagType)).Append(" title=\"Collection Selector\">").Append(_MessageHelper.GetMessage("lbl Collection Selector")).Append("</option>").Append(Environment.NewLine);
                    sb.Append(" <option ").Append(_SearchHelper.SelectedValueAttr(MetaTagType_Content, nMetaTagType)).Append(" title=\"Content Selector\">").Append(_MessageHelper.GetMessage("lbl Content Selector")).Append("</option>").Append(Environment.NewLine);
                    sb.Append(" <option ").Append(_SearchHelper.SelectedValueAttr(MetaTagType_File, nMetaTagType)).Append(" title=\"File Selector\">").Append(_MessageHelper.GetMessage("lbl File Selector")).Append("</option>").Append(Environment.NewLine);
                    sb.Append(" <option ").Append(_SearchHelper.SelectedValueAttr(MetaTagType_HyperLink, nMetaTagType)).Append(" title=\"Hyperlink Selector\">").Append(_MessageHelper.GetMessage("lbl Hyperlink Selector")).Append("</option>").Append(Environment.NewLine);
                    sb.Append(" <option ").Append(_SearchHelper.SelectedValueAttr(MetaTagType_Image, nMetaTagType)).Append(" title=\"Image Selector\">").Append(_MessageHelper.GetMessage("lbl Image Selector")).Append("</option>").Append(Environment.NewLine);
                    sb.Append(" <option ").Append(_SearchHelper.SelectedValueAttr(MetaTagType_ListSummary, nMetaTagType)).Append(" title=\"ListSummary Selector\">").Append(_MessageHelper.GetMessage("lbl ListSummary Selector")).Append("</option>").Append(Environment.NewLine);
                    sb.Append(" <option ").Append(_SearchHelper.SelectedValueAttr(MetaTagType_Menu, nMetaTagType)).Append(" title=\"Menu Selector\">").Append(_MessageHelper.GetMessage("lbl menu Selector")).Append("</option>").Append(Environment.NewLine);
                    sb.Append(" <option ").Append(_SearchHelper.SelectedValueAttr(MetaTagType_User, nMetaTagType)).Append(" title=\"User Selector\">").Append(_MessageHelper.GetMessage("lbl user Selector")).Append("</option>").Append(Environment.NewLine);
                    sb.Append("</select>").Append(Environment.NewLine);
                    sb.Append("</td>").Append(Environment.NewLine);
                }
                sb.Append("</tr>").Append(Environment.NewLine);
                sb.Append("<tr>").Append(Environment.NewLine);
                sb.Append("   <td class=\"label\">").Append(Environment.NewLine);
                sb.Append("      <label for=\"MetaEditable\" title=\"Editable\">").Append(_MessageHelper.GetMessage("editable label")).Append("</label>");
                sb.Append("   </td>").Append(Environment.NewLine);

                if (bView)
                {
                    sb.Append("<td title=\"").Append(_SearchHelper.BoolToYesNo(bMetaEditable)).Append("\">").Append(Environment.NewLine);
                    sb.Append(_SearchHelper.BoolToYesNo(bMetaEditable)).Append(Environment.NewLine);
                    sb.Append("</td>").Append(Environment.NewLine);
                }
                else
                {
                    sb.Append("<td>").Append(Environment.NewLine);
                    sb.Append("<input type=\"checkbox\" title=\"Editable\" id=\"MetaEditable\" name=\"frm_metaeditable\" ").Append(_SearchHelper.CheckedAttr(bMetaEditable)).Append(" onclick=\"OnChangeMetaEditable(this)\" />").Append(Environment.NewLine);
                    sb.Append("<input type=\"hidden\" id=\"MetaDisplayEE\" name=\"frm_metadisplayee\" value=\"").Append(bMetaDisplayEE).Append("\" />").Append(Environment.NewLine);
                    sb.Append("</td>").Append(Environment.NewLine);
                }
                sb.Append("</tr>").Append(Environment.NewLine);
                sb.Append("</table>").Append(Environment.NewLine);

                //Searchable Text
                if (nMetaTagType == MetaTagType_Searchable || !bView)
                {

                    sb.Append(_SearchHelper.MetaTagTypeBoxTop(bView, MetaTagType_Searchable, _MessageHelper.GetMessage("lbl Searchable Property"))).Append(Environment.NewLine);
                    sb.Append("   <table class=\"ektronGrid\">").Append(Environment.NewLine);
                    sb.Append("   <tr>").Append(Environment.NewLine);
                    sb.Append("       <td class=\"label\">").Append(Environment.NewLine);
                    sb.Append("          <label for=\"frm_metaallowsearch\" title=\"Publicly Viewable\">").Append(_MessageHelper.GetMessage("lbl Publicly viewable")).Append("</label>");
                    sb.Append("       </td>").Append(Environment.NewLine);
                    if (bView)
                    {
                        sb.Append("<td>").Append(Environment.NewLine);
                        if (MetaTagType_Searchable == nMetaTagType)
                            sb.Append(_SearchHelper.BoolToYesNo(nMetaAllowSearch)).Append(Environment.NewLine);
                        else
                            sb.Append("(not applicable)").Append(Environment.NewLine);
                        sb.Append("</td>").Append(Environment.NewLine);
                    }
                    else
                    {
                        sb.Append("<td>").Append(Environment.NewLine);
                        sb.Append("<input type=\"checkbox\" title=\"Publicly Viewable Option\" id=\"frm_metaallowsearch\" name=\"frm_metaallowsearch\" ").Append(_SearchHelper.CheckedAttr(nMetaAllowSearch)).Append(" />").Append(Environment.NewLine);
                        sb.Append("</td>").Append(Environment.NewLine);
                    }
                    sb.Append("   </tr>").Append(Environment.NewLine);

                    sb.Append("<tr>").Append(Environment.NewLine);
                    sb.Append("   <td class=\"label\">").Append(Environment.NewLine);
                    sb.Append("      <label for=\"MetaNameTitle_").Append(MetaTagType_Searchable).Append("\" title=\"Style\">").Append(_MessageHelper.GetMessage("style label")).Append("</label>");
                    sb.Append("      <input type=\"hidden\" id=\"MetaDefault_").Append(MetaTagType_Searchable).Append("\" name=\"frm_metadefault_").Append(MetaTagType_Searchable).Append("\" />").Append(Environment.NewLine);
                    sb.Append("      <input type=\"hidden\"  name=\"frm_selectable_only_").Append(MetaTagType_Searchable).Append("\" value=\"\" />").Append(Environment.NewLine);
                    sb.Append("      <input type=\"hidden\"  name=\"frm_allow_multi_").Append(MetaTagType_Searchable).Append("\" value=\"\" />").Append(Environment.NewLine);
                    sb.Append("   </td>").Append(Environment.NewLine);

                    if (bView)
                    {
                        sb.Append("<td>").Append(Environment.NewLine);
                        if (MetaTagType_Searchable == nMetaTagType)
                        {
                            switch (strMetaNameTitle)
                            {
                                case TEXT_PROP:
                                    sb.Append(_MessageHelper.GetMessage("text"));
                                    break;
                                case NUMBER_PROP:
                                    sb.Append("Number (generic)");
                                    break;
                                case BYTE_PROP:
                                    sb.Append("Byte");
                                    break;
                                case DOUBLE_PROP:
                                    sb.Append("Double");
                                    break;
                                case FLOAT_PROP:
                                    sb.Append("Float");
                                    break;
                                case INTEGER_PROP:
                                    sb.Append("Integer");
                                    break;
                                case LONG_PROP:
                                    sb.Append("Long");
                                    break;
                                case SHORT_PROP:
                                    sb.Append("Short");
                                    break;
                                case DATE_PROP:
                                    sb.Append("Date");
                                    break;
                                case BOOLEAN_PROP:
                                    sb.Append("Yes or no");
                                    break;
                                case SELECT1_PROP:
                                    sb.Append("Select from a list");
                                    break;
                                case SELECT_PROP:
                                    sb.Append("Multiple selections");
                                    break;
                                default:
                                    sb.Append(strMetaNameTitle);
                                    break;
                            }
                        }
                        else
                        {
                            sb.Append("(not applicable)").Append(Environment.NewLine);
                        }
                        sb.Append("</td>").Append(Environment.NewLine);
                    }
                    else
                    {
                        sb.Append("<td>").Append(Environment.NewLine);
                        sb.Append("<select id=\"MetaNameTitle_").Append(MetaTagType_Searchable).Append("\" name=\"frm_metanametitle_").Append(MetaTagType_Searchable).Append("\" size=\"1\" onclick=\"OnChangeSearchPropStyle(this)\"  onchange=\"OnChangeSearchPropStyle(this)\">").Append(Environment.NewLine);
                        sb.Append(" <option ").Append(_SearchHelper.SelectedValueAttr(TEXT_PROP, strMetaNameTitle)).Append(" title=\"Text\">").Append("Text").Append("</option>").Append(Environment.NewLine);
                        sb.Append(" <option ").Append(_SearchHelper.SelectedValueAttr(NUMBER_PROP, strMetaNameTitle)).Append(" title=\"Number (generic)\">").Append("Number (generic)").Append("</option>").Append(Environment.NewLine);
                        sb.Append(" <option ").Append(_SearchHelper.SelectedValueAttr(BYTE_PROP, strMetaNameTitle)).Append(" title=\"Byte\">").Append("Byte").Append("</option>").Append(Environment.NewLine);
                        sb.Append(" <option ").Append(_SearchHelper.SelectedValueAttr(DOUBLE_PROP, strMetaNameTitle)).Append(" title=\"Double\">").Append("Double").Append("</option>").Append(Environment.NewLine);
                        sb.Append(" <option ").Append(_SearchHelper.SelectedValueAttr(FLOAT_PROP, strMetaNameTitle)).Append(" title=\"Float\">").Append("Float").Append("</option>").Append(Environment.NewLine);
                        sb.Append(" <option ").Append(_SearchHelper.SelectedValueAttr(INTEGER_PROP, strMetaNameTitle)).Append(" title=\"Integer\">").Append("Integer").Append("</option>").Append(Environment.NewLine);
                        sb.Append(" <option ").Append(_SearchHelper.SelectedValueAttr(LONG_PROP, strMetaNameTitle)).Append(" title=\"Long\">").Append("Long").Append("</option>").Append(Environment.NewLine);
                        sb.Append(" <option ").Append(_SearchHelper.SelectedValueAttr(SHORT_PROP, strMetaNameTitle)).Append(" title=\"Short\">").Append("Short").Append("</option>").Append(Environment.NewLine);
                        sb.Append(" <option ").Append(_SearchHelper.SelectedValueAttr(DATE_PROP, strMetaNameTitle)).Append(" title=\"Date\">").Append("Date").Append("</option>").Append(Environment.NewLine);
                        sb.Append(" <option ").Append(_SearchHelper.SelectedValueAttr(BOOLEAN_PROP, strMetaNameTitle)).Append(" title=\"Yes or no\">").Append("Yes or no").Append("</option>").Append(Environment.NewLine);
                        sb.Append(" <option ").Append(_SearchHelper.SelectedValueAttr(SELECT1_PROP, strMetaNameTitle)).Append(" title=\"Select from a list\">").Append("Select from a list").Append("</option>").Append(Environment.NewLine);
                        sb.Append(" <option ").Append(_SearchHelper.SelectedValueAttr(SELECT_PROP, strMetaNameTitle)).Append(" title=\"Multiple selections\">").Append("Multiple selections").Append("</option>").Append(Environment.NewLine);
                        sb.Append("</select>").Append(Environment.NewLine);
                        sb.Append("</td>").Append(Environment.NewLine);
                    }
                    sb.Append("</tr>").Append(Environment.NewLine);

                    sb.Append("<tr id=\"dataStyleContainer\" style=\"display:none\">").Append(Environment.NewLine);
                    sb.Append("  <td colspan=\"2\">").Append(Environment.NewLine);
                    sb.Append("     <div>").Append(Environment.NewLine);
                    sb.Append("         <table style=\"width:100%;\">").Append(Environment.NewLine);
                    sb.Append("             <tr>").Append(Environment.NewLine);
                    sb.Append("                 <td id=\"dataStyleChangeWarning\">").Append(Environment.NewLine);
                    sb.Append(_MessageHelper.GetMessage("js:confirm change to searchable property")).Append(Environment.NewLine);
                    sb.Append("                 </td>").Append(Environment.NewLine);
                    sb.Append("             </tr>").Append(Environment.NewLine);
                    sb.Append("             <tr>").Append(Environment.NewLine);
                    sb.Append("                 <td id=\"td_metadata_use_existing_data\" style=\"display:none\">").Append(Environment.NewLine);
                    sb.Append("                     <input type=\"radio\" title=\"Use Existing Data Option\" name=\"frm_metadata_update_method\" id=\"metadata_use_existing_data\" value=\"metadata_use_existing_data\" />Use&nbsp;existing&nbsp;data").Append(Environment.NewLine);
                    sb.Append("                 </td>").Append(Environment.NewLine);
                    sb.Append("             </tr>").Append(Environment.NewLine);
                    sb.Append("             <tr>").Append(Environment.NewLine);
                    sb.Append("                 <td id=\"td_metadata_use_existing_data_default\" style=\"display:none\">").Append(Environment.NewLine);
                    sb.Append("                     <input type=\"radio\" title=\"Use Existing Data if possible otherwise use the Default Option\" name=\"frm_metadata_update_method\" id=\"metadata_use_existing_data_default\" value=\"metadata_use_existing_data_default\" />Use&nbsp;existing&nbsp;data&nbsp;if&nbsp;possible,&nbsp;else&nbsp;default").Append(Environment.NewLine);
                    sb.Append("                 </td>").Append(Environment.NewLine);
                    sb.Append("             </tr>").Append(Environment.NewLine);
                    sb.Append("             <tr>").Append(Environment.NewLine);
                    sb.Append("                 <td id=\"td_metadata_use_default\" style=\"display:none\">").Append(Environment.NewLine);
                    sb.Append("                     <input type=\"radio\" title=\"Use Default Data Option\" name=\"frm_metadata_update_method\" id=\"metadata_use_default\" value=\"metadata_use_default\" />Use&nbsp;default&nbsp;value").Append(Environment.NewLine);
                    sb.Append("                 </td>").Append(Environment.NewLine);
                    sb.Append("             </tr>").Append(Environment.NewLine);
                    sb.Append("         </table>").Append(Environment.NewLine);
                    sb.Append("     </div>").Append(Environment.NewLine);
                    sb.Append("  </td>").Append(Environment.NewLine);
                    sb.Append("</tr>").Append(Environment.NewLine);

                    if ((SELECT1_PROP == strMetaNameTitle || SELECT_PROP == strMetaNameTitle) || bView == false)
                    {
                        sb.Append("<tr id=\"idSelectListSeparator\">").Append(Environment.NewLine);
                        sb.Append("   <td class=\"label\">").Append(Environment.NewLine);
                        sb.Append("        <label title=\"Separator\" id=\"MetaSeparatorLabel_").Append(MetaTagType_Searchable).Append("\" for=\"MetaSeparator_").Append(MetaTagType_Searchable).Append("\" >").Append(_MessageHelper.GetMessage("separator label")).Append("</label> ").Append(Environment.NewLine);
                        sb.Append("   </td>").Append(Environment.NewLine);
                        if (bView)
                        {
                            sb.Append("   <td title=\"").Append(strMetaSeparator).Append("\">").Append(Environment.NewLine);
                            sb.Append(strMetaSeparator);
                            sb.Append("   </td>").Append(Environment.NewLine);
                        }
                        else
                        {
                            sb.Append("   <td>").Append(Environment.NewLine);
                            sb.Append("   <input type=\"text\" title=\"Enter Separator Text here\" size=\"5\" maxlength=\"10\" id=\"MetaSeparator_").Append(MetaTagType_Searchable).Append("\" name=\"frm_metaseparator_").Append(MetaTagType_Searchable).Append("\" value=\"").Append(EkFunctions.HtmlEncode(strMetaSeparator)).Append("\" />").Append(Environment.NewLine);
                            sb.Append("   </td>").Append(Environment.NewLine);
                        }
                        sb.Append("</tr>").Append(Environment.NewLine);

                        sb.Append("<tr id=\"idSelectListLabel\" class=\"label\">").Append(Environment.NewLine);
                        sb.Append("   <td>").Append(Environment.NewLine);
                        if (bView)
                            strHtml = _MessageHelper.GetMessage("alt list of values");
                        else
                            strHtml = _MessageHelper.GetMessage("alt List (use separator between values)");
                        sb.Append("       <label id=\"MetaSelectableTextLabel_").Append(MetaTagType_Searchable).Append("\" for=\"MetaSelectableText_").Append(MetaTagType_Searchable).Append("\">").Append(strHtml).Append("</label>").Append(Environment.NewLine);
                        sb.Append("   </td>").Append(Environment.NewLine);
                        sb.Append("   <td id=\"idSelectListText\">").Append(Environment.NewLine);
                        if (bView)
                            sb.Append(strMetaSelectableText).Append(Environment.NewLine);
                        else
                            sb.Append(" <textarea id=\"MetaSelectableText_").Append(MetaTagType_Searchable).Append("\" name=\"frm_MetaSelectableText_").Append(MetaTagType_Searchable).Append("\" wrap=\"soft\" onclick=\"OnChangeSelectList(this)\" onkeyup=\"OnChangeSelectList(this)\" onchange=\"OnChangeSelectList(this)\">").Append(EkFunctions.HtmlDecode(strMetaSelectableText)).Append("</textarea>").Append(Environment.NewLine);
                        sb.Append("   </td>").Append(Environment.NewLine);
                        sb.Append("</tr>").Append(Environment.NewLine);
                    }
                    if (bView)
                        sb.Append(_SearchHelper.WriteMetadataTypeForView(strMetaNameTitle, _MessageHelper.GetMessage("lbl default"), strMetaDefault)).Append(Environment.NewLine);
                    else
                    {
                        sb.Append(_SearchHelper.WriteMetadataDefaultForEdit(_SearchHelper.IIfSet(TEXT_PROP == strMetaNameTitle, cMetaType, null), TEXT_PROP)).Append(Environment.NewLine);
                        sb.Append(_SearchHelper.WriteMetadataDefaultForEdit(_SearchHelper.IIfSet(NUMBER_PROP == strMetaNameTitle, cMetaType, null), NUMBER_PROP)).Append(Environment.NewLine);
                        sb.Append(_SearchHelper.WriteMetadataDefaultForEdit(_SearchHelper.IIfSet(BYTE_PROP == strMetaNameTitle, cMetaType, null), BYTE_PROP)).Append(Environment.NewLine);
                        sb.Append(_SearchHelper.WriteMetadataDefaultForEdit(_SearchHelper.IIfSet(DOUBLE_PROP == strMetaNameTitle, cMetaType, null), DOUBLE_PROP)).Append(Environment.NewLine);
                        sb.Append(_SearchHelper.WriteMetadataDefaultForEdit(_SearchHelper.IIfSet(FLOAT_PROP == strMetaNameTitle, cMetaType, null), FLOAT_PROP)).Append(Environment.NewLine);
                        sb.Append(_SearchHelper.WriteMetadataDefaultForEdit(_SearchHelper.IIfSet(INTEGER_PROP == strMetaNameTitle, cMetaType, null), INTEGER_PROP)).Append(Environment.NewLine);
                        sb.Append(_SearchHelper.WriteMetadataDefaultForEdit(_SearchHelper.IIfSet(LONG_PROP == strMetaNameTitle, cMetaType, null), LONG_PROP)).Append(Environment.NewLine);
                        sb.Append(_SearchHelper.WriteMetadataDefaultForEdit(_SearchHelper.IIfSet(SHORT_PROP == strMetaNameTitle, cMetaType, null), SHORT_PROP)).Append(Environment.NewLine);
                        sb.Append(_SearchHelper.WriteMetadataDefaultForEdit(_SearchHelper.IIfSet(DATE_PROP == strMetaNameTitle, cMetaType, null), DATE_PROP)).Append(Environment.NewLine);
                        sb.Append(_SearchHelper.WriteMetadataDefaultForEdit(_SearchHelper.IIfSet(SELECT1_PROP == strMetaNameTitle, cMetaType, null), SELECT1_PROP)).Append(Environment.NewLine);
                        sb.Append(_SearchHelper.WriteMetadataDefaultForEdit(_SearchHelper.IIfSet(SELECT_PROP == strMetaNameTitle, cMetaType, null), SELECT_PROP)).Append(Environment.NewLine);
                        sb.Append(_SearchHelper.WriteMetadataDefaultForEdit(_SearchHelper.IIfSet(BOOLEAN_PROP == strMetaNameTitle, cMetaType, null), BOOLEAN_PROP)).Append(Environment.NewLine);
                    }

                    sb.Append(_SearchHelper.MetaTagTypeBoxBottom(bView)).Append(Environment.NewLine);
                    sb.Append("</table>").Append(Environment.NewLine);

                }

                //Meta Tag
                if (MetaTagType_Meta == nMetaTagType || !bView)
                {
                    sb.Append(_SearchHelper.MetaTagTypeBoxTop(bView, MetaTagType_Meta, _MessageHelper.GetMessage("generic Meta Tag"))).Append(Environment.NewLine);
                    sb.Append(" <table class=\"ektronGrid\">").Append(Environment.NewLine);
                    sb.Append("   <tr>").Append(Environment.NewLine);
                    sb.Append("     <td class=\"label\">").Append(Environment.NewLine);
                    sb.Append("         <label id=\"MetaNameTitleLabel_").Append(MetaTagType_Meta).Append("\" for=\"MetaNameTitle_").Append(MetaTagType_Meta).Append("\" title=\"Style\">").Append(_MessageHelper.GetMessage("style label")).Append("</label>").Append(Environment.NewLine);
                    sb.Append("     </td>").Append(Environment.NewLine);
                    sb.Append("     <td>").Append(Environment.NewLine);
                    if (bView)
                    {
                        if (MetaTagType_Meta == nMetaTagType)
                            sb.Append(strMetaNameTitle.Replace("=", "")).Append(Environment.NewLine);
                        else
                            sb.Append("(not applicable)").Append(Environment.NewLine);
                    }
                    else
                    {
                        sb.Append("  <select id=\"MetaNameTitle_").Append(MetaTagType_Meta).Append("\" name=\"frm_metanametitle_").Append(MetaTagType_Meta).Append("\" size=\"1\">   ").Append(Environment.NewLine);
                        sb.Append("  <option ").Append(_SearchHelper.SelectedValueAttr("name=", strMetaNameTitle)).Append(" title=\"name\">name</option>").Append(Environment.NewLine);
                        sb.Append("  <option ").Append(_SearchHelper.SelectedValueAttr("http-equiv=", strMetaNameTitle)).Append(" title=\"http-equiv\">http-equiv</option>").Append(Environment.NewLine);
                        sb.Append("  </select>").Append(Environment.NewLine);
                    }
                    sb.Append("     </td>").Append(Environment.NewLine);
                    sb.Append("   </tr>").Append(Environment.NewLine);

                    //tr
                    sb.Append("   <tr>").Append(Environment.NewLine);
                    sb.Append("      <td class=\"label\">").Append(Environment.NewLine);
                    sb.Append("         <label id=\"MetaRemoveDuplicatesLabel_").Append(MetaTagType_Meta).Append("\" for=\"MetaRemoveDuplicates_").Append(MetaTagType_Meta).Append("\" title=\"Remove Duplicates\">").Append(_MessageHelper.GetMessage("remove duplicates label")).Append("</label>").Append(Environment.NewLine);
                    sb.Append("      </td>").Append(Environment.NewLine);
                    sb.Append("      <td>").Append(Environment.NewLine);
                    if (bView)
                    {
                        if (MetaTagType_Meta == nMetaTagType)
                            sb.Append(_SearchHelper.BoolToYesNo(bMetaRemoveDuplicates)).Append(Environment.NewLine);
                        else
                            sb.Append("(not applicable)").Append(Environment.NewLine);

                    }
                    else
                    {
                        sb.Append("<input type=\"checkbox\" title=\"Option to Remove Duplicates\" id=\"MetaRemoveDuplicates_").Append(MetaTagType_Meta).Append("\" name=\"frm_metaremoveduplicates_").Append(MetaTagType_Meta).Append("\"").Append(_SearchHelper.CheckedAttr(bMetaRemoveDuplicates)).Append(" onclick=\"OnChangeMetaRemoveDuplicates(this)\" />").Append(Environment.NewLine);
                    }
                    sb.Append("      </td>").Append(Environment.NewLine);
                    sb.Append("   </tr>").Append(Environment.NewLine);

                    //tr
                    sb.Append("   <tr>").Append(Environment.NewLine);
                    sb.Append("     <td class=\"label\">").Append(Environment.NewLine);
                    sb.Append("         <label id=\"MetaCaseSensitiveLabel_").Append(MetaTagType_Meta).Append("\" for=\"MetaCaseSensitive_").Append(MetaTagType_Meta).Append("\" title=\"Case Sensitive\">").Append(_MessageHelper.GetMessage("case sensitive label")).Append(" </label>").Append(Environment.NewLine);
                    sb.Append("     </td>").Append(Environment.NewLine);
                    sb.Append("     <td align=\"left\">").Append(Environment.NewLine);
                    if (bView)
                    {
                        if (MetaTagType_Meta == nMetaTagType && bMetaRemoveDuplicates)
                            sb.Append(_SearchHelper.BoolToYesNo(bMetaCaseSensitive)).Append(Environment.NewLine);
                        else
                            sb.Append("(not applicable)").Append(Environment.NewLine);
                    }
                    else
                    {
                        sb.Append("<input type=\"checkbox\" title=\"Case Sensitive Option\" id=\"MetaCaseSensitive_").Append(MetaTagType_Meta).Append("\" name=\"frm_metacasesensitive_").Append(MetaTagType_Meta).Append("\"").Append(_SearchHelper.CheckedAttr(bMetaCaseSensitive)).Append(_SearchHelper.DisabledAttr(!bMetaRemoveDuplicates)).Append(" />").Append(Environment.NewLine);
                    }
                    sb.Append("     </td>").Append(Environment.NewLine);
                    sb.Append("   </tr>").Append(Environment.NewLine);

                    //tr
                    sb.Append("   <tr>").Append(Environment.NewLine);
                    sb.Append("     <td class=\"label\">").Append(Environment.NewLine);
                    sb.Append("         <label title=\"Separator\" id=\"MetaSeparatorLabel_").Append(MetaTagType_Meta).Append("\" for=\"MetaSeparator_").Append(MetaTagType_Meta).Append("\" >").Append(_MessageHelper.GetMessage("separator label")).Append("</label>").Append(Environment.NewLine);
                    sb.Append("     </td>").Append(Environment.NewLine);
                    sb.Append("     <td>").Append(Environment.NewLine);
                    if (bView)
                        sb.Append(strMetaSeparator).Append(Environment.NewLine);
                    else
                        sb.Append("<input type=\"text\" title=\"Enter Text here\" size=\"5\" maxlength=\"10\" id=\"MetaSeparator_").Append(MetaTagType_Meta).Append("\" name=\"frm_metaseparator_").Append(MetaTagType_Meta).Append("\" value=\"").Append(EkFunctions.HtmlEncode(strMetaSeparator)).Append("\" />").Append(Environment.NewLine);
                    sb.Append("     </td>").Append(Environment.NewLine);
                    sb.Append("   </tr>").Append(Environment.NewLine);

                    //tr
                    sb.Append("   <tr>").Append(Environment.NewLine);
                    sb.Append("     <td class=\"label\">").Append(Environment.NewLine);
                    sb.Append("         <label title=\"Selectable Metadata\" id=\"SelectableOnlyLabel_").Append(MetaTagType_Meta).Append("\" for=\"SelectableOnly_").Append(MetaTagType_Meta).Append("\" >").Append(_MessageHelper.GetMessage("lbl Selectable Metadata")).Append("</label>").Append(Environment.NewLine);
                    sb.Append("     </td>").Append(Environment.NewLine);
                    sb.Append("     <td>").Append(Environment.NewLine);
                    if (bView)
                        sb.Append(_SearchHelper.BoolToYesNo(bSelectableOnly)).Append(Environment.NewLine);
                    else
                        sb.Append(" <input type=\"checkbox\" title=\"Only Selectable Option\" id=\"SelectableOnly_").Append(MetaTagType_Meta).Append("\" name=\"frm_selectable_only_").Append(MetaTagType_Meta).Append("\" ").Append(_SearchHelper.CheckedAttr(bSelectableOnly)).Append(" onclick=\"OnChangeSelectable(this)\" />").Append(Environment.NewLine);
                    sb.Append("     </td>").Append(Environment.NewLine);
                    sb.Append("   </tr>").Append(Environment.NewLine);

                    //tr
                    sb.Append("   <tr>").Append(Environment.NewLine);
                    sb.Append("     <td class=\"label\">").Append(Environment.NewLine);
                    sb.Append("         <label id=\"AllowMultiLabel_").Append(MetaTagType_Meta).Append("\" for=\"AllowMulti_").Append(MetaTagType_Meta).Append("\" title=\"Allow Multiple Selections\">").Append(_MessageHelper.GetMessage("lbl Allow Multiple Selections")).Append("</label>").Append(Environment.NewLine);
                    sb.Append("     </td>").Append(Environment.NewLine);
                    sb.Append("     <td>").Append(Environment.NewLine);
                    if (bView)
                    {
                        if (bSelectableOnly)
                            sb.Append(_SearchHelper.BoolToYesNo(bAllowMulti)).Append(Environment.NewLine);
                        else
                            sb.Append("(not applicable)").Append(Environment.NewLine);
                    }
                    else
                    {
                        sb.Append("<input type=\"checkbox\" id=\"AllowMulti_").Append(MetaTagType_Meta).Append("\" name=\"frm_allow_multi_").Append(MetaTagType_Meta).Append("\" ").Append(_SearchHelper.CheckedAttr(bSelectableOnly && bAllowMulti)).Append(_SearchHelper.DisabledAttr(!bSelectableOnly)).Append(" />").Append(Environment.NewLine);
                    }
                    sb.Append("     </td>").Append(Environment.NewLine);
                    sb.Append("   </tr>").Append(Environment.NewLine);

                    //tr
                    sb.Append("   <tr>").Append(Environment.NewLine);
                    sb.Append("     <td class=\"label\">").Append(Environment.NewLine);
                    sb.Append("          <label id=\"MetaSelectableTextLabel_").Append(MetaTagType_Meta).Append("\" for=\"MetaSelectableText_").Append(MetaTagType_Meta).Append("\">").Append(_MessageHelper.GetMessage("lbl Allowed Selectable Text")).Append("</label>").Append(Environment.NewLine);
                    sb.Append("     </td>").Append(Environment.NewLine);
                    sb.Append("     <td>").Append(Environment.NewLine);
                    if (bView)
                        sb.Append(strMetaSelectableText).Append(Environment.NewLine);
                    else
                    {
                        sb.Append("<textarea title=\"Enter Text here\" id=\"MetaSelectableText_").Append(MetaTagType_Meta).Append("\" name=\"frm_MetaSelectableText_").Append(MetaTagType_Meta).Append("\" ").Append(_SearchHelper.DisabledAttr(!bSelectableOnly)).Append(" wrap=\"soft\">").Append(EkFunctions.HtmlEncode(strMetaSelectableText)).Append("</textarea>").Append(Environment.NewLine);
                    }
                    sb.Append("     </td>").Append(Environment.NewLine);
                    sb.Append("   </tr>").Append(Environment.NewLine);

                    //tr
                    sb.Append("   <tr>").Append(Environment.NewLine);
                    sb.Append("     <td class=\"label\" title=\"Allowed Selectable Text\">").Append(Environment.NewLine);
                    strHtml = _MessageHelper.GetMessage("default text label");
                    if (!bView)
                        strHtml = strHtml + " (500 " + _MessageHelper.GetMessage("abbreviation for maximum") + ")";
                    sb.Append("         <label title=\"").Append(strHtml).Append("\" id=\"MetaDefaultLabel_").Append(MetaTagType_Meta).Append("\"  for=\"MetaDefault_").Append(MetaTagType_Meta).Append("\"> ").Append(strHtml).Append("</label>").Append(Environment.NewLine);
                    sb.Append("     </td>").Append(Environment.NewLine);
                    sb.Append("     <td>").Append(Environment.NewLine);
                    if (bView)
                        sb.Append(EkFunctions.HtmlEncode(strMetaDefault)).Append(Environment.NewLine);
                    else
                    {
                        sb.Append("<textarea title=\"").Append(EkFunctions.HtmlEncode(strMetaDefault)).Append("\" id=\"MetaDefault_").Append(MetaTagType_Meta).Append("\" name=\"frm_metadefault_").Append(MetaTagType_Meta).Append("\" wrap=\"soft\">").Append(EkFunctions.HtmlEncode(strMetaDefault)).Append("</textarea>").Append(Environment.NewLine);
                    }
                    sb.Append("     </td>").Append(Environment.NewLine);
                    sb.Append("   </tr>").Append(Environment.NewLine);

                    sb.Append(_SearchHelper.MetaTagTypeBoxBottom(bView)).Append(Environment.NewLine);
                    sb.Append("</table>").Append(Environment.NewLine);
                }

                //Html Tag
                if (MetaTagType_Html == nMetaTagType || !bView)
                {
                    sb.Append(_SearchHelper.MetaTagTypeBoxTop(bView, MetaTagType_Html, _MessageHelper.GetMessage("generic HTML Tag"))).Append(Environment.NewLine);
                    sb.Append("   <table class=\"ektronGrid\">").Append(Environment.NewLine);
                    sb.Append("     <tr>").Append(Environment.NewLine);
                    sb.Append("       <td class=\"label\">").Append(Environment.NewLine);
                    strHtml = _MessageHelper.GetMessage("default label");
                    if (!bView)
                        strHtml = strHtml + " (500 " + _MessageHelper.GetMessage("abbreviation for maximum") + ")";
                    sb.Append("           <label title=\"").Append(strHtml).Append("\" id=\"MetaDefaultLabel_").Append(MetaTagType_Html).Append("\" for=\"MetaDefault_").Append(MetaTagType_Html).Append("\"> ").Append(strHtml).Append("</label>").Append(Environment.NewLine);
                    sb.Append("       </td>").Append(Environment.NewLine);
                    sb.Append("       <td>").Append(Environment.NewLine);
                    if (bView)
                        sb.Append(EkFunctions.HtmlEncode(strMetaDefault)).Append(Environment.NewLine);
                    else
                        sb.Append("<textarea class=\"readOnlyValue\"  rows=\"8\" cols=\"80\" title=\"").Append(EkFunctions.HtmlEncode(strMetaDefault)).Append("\" id=\"MetaDefault_").Append(MetaTagType_Html).Append("\" name=\"frm_metadefault_").Append(MetaTagType_Html).Append("\" wrap=\"soft\">").Append(EkFunctions.HtmlEncode(strMetaDefault)).Append("</textarea>").Append(Environment.NewLine);
                    sb.Append("       </td>").Append(Environment.NewLine);
                    sb.Append("     </tr>").Append(Environment.NewLine);

                    sb.Append(_SearchHelper.MetaTagTypeBoxBottom(bView)).Append(Environment.NewLine);
                    sb.Append("</table>").Append(Environment.NewLine);

                }

                if (bNew)
                {
                    string metaTypeNames = "";
                    cMetadata = _ContentApi.EkContentRef.GetMetadataTypes("Name");
                    foreach (Collection cMetaType_loopVariable in cMetadata)
                    {
                        if (!String.IsNullOrEmpty(cMetaType_loopVariable["MetaTypeName"].ToString()))
                        {
                            if ((metaTypeNames.Length > 0))
                            {
                                metaTypeNames += ",";
                            }
                            metaTypeNames += cMetaType_loopVariable["MetaTypeName"];
                        }
                    }
                    sb.Append("<input type=\"hidden\" id=\"meta_type_names\" value=\"").Append(metaTypeNames).Append("\" />").Append(Environment.NewLine);
                }
                viewGrid.InnerHtml = sb.ToString();
            }
        }
    }

    #endregion

    #region JS/CSS
    private void RegisterJS()
    {

        Ektron.Cms.API.JS.RegisterJS(this, Ektron.Cms.API.JS.ManagedScript.EktronJS);
        Ektron.Cms.API.JS.RegisterJS(this, Ektron.Cms.API.JS.ManagedScript.EktronModalJS);
        Ektron.Cms.API.JS.RegisterJS(this, Ektron.Cms.API.JS.ManagedScript.EktronJFunctJS);
        Ektron.Cms.API.JS.RegisterJS(this, _ApplicationPath + "java/internCalendarDisplayFuncs.js", "EktronCalendarDisplayFunctionJs", false);
        Ektron.Cms.API.JS.RegisterJS(this, _ApplicationPath + "java/datejsfunc.js", "EktronDateJsFuncJs", false);
        Ektron.Cms.API.JS.RegisterJS(this, _ApplicationPath + "java/searchfuncsupport.js", "EktronSearchFunctionSupportJs", false);
        Ektron.Cms.API.JS.RegisterJS(this, Ektron.Cms.API.JS.ManagedScript.EktronToolBarRollJS);
        Ektron.Cms.API.JS.RegisterJS(this, Ektron.Cms.API.JS.ManagedScript.EktronWorkareaJS);

    }
    private void RegisterCSS()
    {
        Ektron.Cms.API.Css.RegisterCss(this, Ektron.Cms.API.Css.ManagedStyleSheet.EktronModalCss);
        Ektron.Cms.API.Css.RegisterCss(this, Ektron.Cms.API.Css.ManagedStyleSheet.EktronWorkareaCss);
        Ektron.Cms.API.Css.RegisterCss(this, Ektron.Cms.API.Css.ManagedStyleSheet.EktronWorkareaIeCss, Ektron.Cms.API.Css.BrowserTarget.LessThanEqualToIE7);
        Ektron.Cms.API.Css.RegisterCss(this, Ektron.Cms.API.Css.ManagedStyleSheet.EktronFixedPositionToolbarCss);
        StyleSheetJS.Text = _StyleHelper.GetClientScript();
    }

    #endregion
}

