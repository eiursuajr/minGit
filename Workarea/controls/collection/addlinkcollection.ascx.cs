using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Ektron.Cms.Common;
using Ektron.Cms.UI.CommonUI;
using Ektron.Cms;
using System.Data;
using Microsoft.VisualBasic;
using Ektron.Cms.Content;
using System.Collections;
using System.Text;

public partial class Workarea_controls_collection_addlinkcollection : System.Web.UI.UserControl
{
    protected string ContentIcon = "";
    protected string WebEventIcon = "";
    protected string pageIcon = "";
    protected string formsIcon = "";
    protected string AppImgPath = "";
    protected long FolderId = 0;
    protected string actName = "AddLink";
    protected string notSupportIFrame = "";
    protected string AppName = "";
    protected ContentAPI m_refContentApi = new ContentAPI();
    protected StyleHelper m_refStyle = new StyleHelper();
    protected bool CanCreateContent = false;
    protected PermissionData perm_data;
    protected EkMessageHelper m_refMsg;
    protected Collection gtNavs;
    protected Collection cTmp;
    protected Collection cFolders;
    protected object FolderName;
    protected object ParentFolderId;
    protected string fPath;
    protected EkEnumeration.FolderType folderType = EkEnumeration.FolderType.Content;
    protected EkContent m_refContent;
    protected string AddType = "";
    protected EkContentCol cConts;
    protected Collection CollectionContentItems;
    protected long nId;
    protected long subfolderid;
    protected long locID;
    protected int g_ContentTypeSelected = -1;
    protected Collection cRecursive;
    protected bool rec;
    protected string MenuTitle;
    protected string CollectionTitle = "";
    protected string ErrorString;
    protected AssetInfoData[] asset_data;
    protected int lContentType;
    protected int ContentLanguage;
    protected string NoWorkAreaAttribute;
    protected long backId;
    protected int _currentPageNumber = 1;
    protected int TotalPagesNumber = 1;
    protected int count;
    protected long lAddMultiType;
    protected bool bSelectedFound;
    protected string status = "";
    protected const int ALL_CONTENT_LANGUAGES = -1;
    protected int CONTENT_LANGUAGES_UNDEFINED = 0;
    protected object gtMsgObj;
    protected object gtMess;
    protected object CollectionID;
    protected object msgs;
    protected object currentUserID;
    protected object PageTitle;
    protected object AppPath;
    protected object sitePath;
    protected object gtNav;
    protected object lLoop;
    protected object siteObj;
    protected Hashtable cPerms;
    protected object cLinkArray;
    protected object fLinkArray;
    protected object gtObj;
    protected Collection gtLinks;
    protected object OrderBy;
    protected object cLanguagesArray;
    protected object action;
    protected object folder;
    protected object menuIcon;
    protected object libraryIcon;
    protected object linkIcon;
    protected object sTitleBar;
    protected object maID;
    protected object mpID;
    protected object EnableMultilingual;
    protected ApplicationAPI AppUI = new Ektron.Cms.UI.CommonUI.ApplicationAPI();
    protected string Callbackpage = "";
    protected EkMessageHelper MsgHelper;
    protected EkContentCol contentData;
    protected CommonApi m_refApi = new CommonApi();
    protected ContentAPI contentApi = new ContentAPI();
    protected string AncestorIDParam = "";
    protected string ParentIDParam = "";
    protected string checkout = "";
    protected bool bCheckedout = false;
    protected string m_strKeyWords = "";
    protected long folderId;
    protected long CurrentUserId;
    protected string contentIcon;
    StringBuilder result = new StringBuilder();
    protected string ActionString = "";
    protected void Page_Load(object sender, EventArgs e)
    {
        MsgHelper = new EkMessageHelper(m_refContentApi.RequestInformationRef);
        AddType = Strings.LCase(Request.QueryString["addto"]);
        if (!string.IsNullOrEmpty(Request.QueryString["back"]))
            frm_back.Value = Request.QueryString["back"].ToString();
        hidCollectionID.Value = Request.QueryString["nid"].ToString();
        nId = Convert.ToInt64(Request.QueryString["nid"]);
        ContentLanguage = m_refApi.ContentLanguage;
        if (!string.IsNullOrEmpty(Request.QueryString["LangType"]))
        {
            ContentLanguage = Convert.ToInt32(Request.QueryString["LangType"]);
            if (ContentLanguage == ALL_CONTENT_LANGUAGES || ContentLanguage == 0)
            {
                ContentLanguage = AppUI.DefaultContentLanguage;
            }

            AppUI.FilterByLanguage = ContentLanguage;
        }

        if (!string.IsNullOrEmpty(Request.QueryString["noworkarea"]))
        {
            NoWorkAreaAttribute = "&noworkarea=1";
        }

        if (!IsPostBack)
        {
            FillContentFolderInfo();
        }
        else if (Request.Form["isPostData"] != null && Request.Form["isPostData"].ToString() != "")
        {
            string _backAction = string.Empty;
            if (!string.IsNullOrEmpty(Request.QueryString["back"]))
            {
                _backAction = EkFunctions.UrlEncode(Request.QueryString["back"]);
            }            

            if (AddType == "menu")
            {
                Server.Transfer((string)("collections.aspx?LangType=" + ContentLanguage + "&Action=doAddMenuItem&type=content&folderid=" + Request.QueryString["folderid"] + "&nid=" + Request.QueryString["nid"] + "&iframe=" + Request.QueryString["iframe"] + NoWorkAreaAttribute + "&back=" + _backAction));
            }
            else
            {               
                Server.Transfer((string)("collections.aspx?LangType=" + ContentLanguage + "&Action=doAddLinks&folderid=" + Request.QueryString["folderid"] + "&nid=" + Request.QueryString["nid"] + NoWorkAreaAttribute + status + "&back=" + _backAction));
            }
        }
    }

    private void DisplayToolBarAndContent()
    {
        if (AddType != "menu")
        {
            AppUI.FilterByLanguage = ContentLanguage;
            if (!string.IsNullOrEmpty(Request.QueryString["checkout"]))
                checkout = Request.QueryString["checkout"].ToString();
            gtLinks = AppUI.EkContentRef.GetEcmCollectionByID(nId, false, false, ref ErrorString, true, false, true);

            if (checkout == "true")
                gtLinks = AppUI.EkContentRef.GetEcmStageCollectionByID(nId, false, false, ref ErrorString, true, false, true);
            if (gtLinks.Count > 0)
                CollectionTitle = gtLinks["CollectionTitle"].ToString();

            string strInfo = "";
            if (!string.IsNullOrEmpty(m_refStyle.GetTitleBar(MsgHelper.GetMessage("add collection items title"))))
                strInfo = m_refStyle.GetTitleBar(MsgHelper.GetMessage("add collection items title"));

            litTitle.Text = strInfo + " \"" + CollectionTitle + "\"";
            litHelp.Text = StyleHelper.ActionBarDivider + "<td>" + m_refStyle.GetHelpButton("AddCollectionItems", "") + "<td>";
            ActionString = "collections.aspx?action=doAddLinks&nId=" + nId + "&folderId=" + folderId;
        }
        else
        {
            ActionString = "collections.aspx?LangType=" + ContentLanguage + "&Action=doAddMenuItem&type=content&folderid=" + folderId + "&nid=" + nId + "&iframe=" + Request.QueryString["iframe"];

            litTitle.Text = m_refStyle.GetTitleBar(MsgHelper.GetMessage("Add Menu Item Title") + " \"" + MenuTitle + "\"");
			litHelp.Text = StyleHelper.ActionBarDivider + "<td>" + m_refStyle.GetHelpButton("AddMenuItems", "") + "<td>";
        }
        if (string.IsNullOrEmpty(Request.QueryString["ancestorid"]))
            AncestorIDParam = "&ancestorid=" + Request.QueryString["ancestorid"];
        if (string.IsNullOrEmpty(Request.QueryString["parentid"]))
            ParentIDParam = "&parentid=" + Request.QueryString["parentid"];
        
		// Toolbar
		string saveButton = m_refStyle.GetButtonEventsWCaption(AppPath + "images/UI/Icons/save.png", "#", MsgHelper.GetMessage("alt: add selected collection items text"), MsgHelper.GetMessage("btn add"), "onclick=\"return SubmitFormSelections(\'GetIDs()\');\"", StyleHelper.SaveButtonCssClass, true);
		if (AddType == "menu")
        {
            if (!string.IsNullOrEmpty(Request.QueryString["back"]))
            {
                litButtons.Text = m_refStyle.GetButtonEventsWCaption(AppPath + "images/UI/Icons/cancel.png", Request.QueryString["back"], MsgHelper.GetMessage("generic Cancel"), MsgHelper.GetMessage("generic Cancel"), "", StyleHelper.CancelButtonCssClass, true);
				litButtons.Text += saveButton;
				if (CanCreateContent)
                    litButtons.Text += m_refStyle.GetButtonEventsWCaption(AppPath + "images/UI/Icons/contentAdd.png", "#", MsgHelper.GetMessage("alt add content button text"), MsgHelper.GetMessage("btn add content"), "onclick=\"PopUpWindow('editarea.aspx?LangType=" + ContentLanguage + "&type=add&id=" + locID + "', 'Edit', 790, 580, 1, 1);return false;\" ", StyleHelper.AddContentButtonCssClass);

            }
			else if (Request.QueryString["iframe"] == "true")
			{
				litButtons.Text += m_refStyle.GetButtonEventsWCaption(AppPath + "images/UI/Icons/back.png", "collections.aspx?LangType=" + ContentLanguage + "&Action=AddMenuItem+nid=" + Request.QueryString["nid"] + "&folderid=" + Request.QueryString["folderid"] + NoWorkAreaAttribute + AncestorIDParam + ParentIDParam, MsgHelper.GetMessage("alt back button text"), MsgHelper.GetMessage("btn back"), "", StyleHelper.BackButtonCssClass, true);
				litButtons.Text = m_refStyle.GetButtonEventsWCaption(AppPath + "images/UI/Icons/cancel.png", "#", MsgHelper.GetMessage("generic Cancel"), MsgHelper.GetMessage("generic Cancel"), "onclick=\"parent.CancelIframe();\"", StyleHelper.CancelButtonCssClass, true);
				litButtons.Text += saveButton;
				if (CanCreateContent)
					litButtons.Text += m_refStyle.GetButtonEventsWCaption(AppPath + "images/UI/Icons/contentAdd.png", "#", MsgHelper.GetMessage("alt add content button text"), MsgHelper.GetMessage("btn add content"), "onclick=\"PopUpWindow('editarea.aspx?LangType=" + ContentLanguage + "&type=add&id=" + locID + "', 'Edit', 790, 580, 1, 1);return false;\" ", StyleHelper.AddContentButtonCssClass);
				else
					litButtons.Text += m_refStyle.GetButtonEventsWCaption(AppPath + "images/UI/Icons/contentAdd.png", "#", MsgHelper.GetMessage("alt add content button text"), MsgHelper.GetMessage("btn add content"), "onclick=\"PopUpWindow('editarea.aspx?LangType=" + ContentLanguage + "&type=add&id=" + locID + "', 'Edit', 790, 580, 1, 1);return false;\" ", StyleHelper.AddContentButtonCssClass);
			}
			else
			{
				litButtons.Text = saveButton;
			}
        }
        else
        {
			if (!string.IsNullOrEmpty(Request.QueryString["noworkarea"]) && Request.QueryString["noworkarea"].ToString() == "1")
				litButtons.Text = m_refStyle.GetButtonEventsWCaption(AppPath + "images/UI/Icons/cancel.png", "#", MsgHelper.GetMessage("generic Cancel"), MsgHelper.GetMessage("generic Cancel"), "onclick=\"self.close();return false;\" ", StyleHelper.CancelButtonCssClass, true);
			else
				litButtons.Text = m_refStyle.GetButtonEventsWCaption(AppPath + "images/UI/Icons/back.png", "collections.aspx?LangType=" + ContentLanguage + "&Action=View&nid=" + Request.QueryString["nid"] + "&folderid=" + Request.QueryString["folderid"], MsgHelper.GetMessage("alt back button text"), MsgHelper.GetMessage("btn back"), "", StyleHelper.BackButtonCssClass, true);

			litButtons.Text += saveButton;

            if (CanCreateContent)
                litButtons.Text += m_refStyle.GetButtonEventsWCaption(AppPath + "images/UI/Icons/contentAdd.png", "#", MsgHelper.GetMessage("alt add content button text"), MsgHelper.GetMessage("btn add content"), "onclick=\"PopUpWindow('editarea.aspx?LangType=" + ContentLanguage + "&type=add&id=" + locID + "', 'Edit', 790, 580, 1, 1);return false;\" ", StyleHelper.AddContentButtonCssClass);
        }

        if (asset_data != null)
        {
            if (asset_data.Length > 0)
            {
				result.Append(StyleHelper.ActionBarDivider);
                result.Append("<td><select id=selAssetSupertype name=selAssetSupertype OnChange=\"UpdateView();\">");
                if (Ektron.Cms.Common.EkConstants.CMSContentType_NonLibraryContent == lContentType)
                    result.Append("<option value='" + Ektron.Cms.Common.EkConstants.CMSContentType_NonLibraryContent + "' selected>" + this.m_refMsg.GetMessage("lbl all types") + "</option>");
                else
                    result.Append("<option value='" + Ektron.Cms.Common.EkConstants.CMSContentType_AllTypes + "'>" + this.m_refMsg.GetMessage("lbl all types") + "</option>");
                if (Ektron.Cms.Common.EkConstants.CMSContentType_Content == lContentType)
                    result.Append("<option value='" + Ektron.Cms.Common.EkConstants.CMSContentType_Content + "' selected>" + this.m_refMsg.GetMessage("lbl html content") + "</option>");
                else
                    result.Append("<option value='" + Ektron.Cms.Common.EkConstants.CMSContentType_Content + "'>" + this.m_refMsg.GetMessage("lbl html content") + "</option>");
                for (int count = 0; count < asset_data.Length; count++)
                {
                    if (Ektron.Cms.Common.EkConstants.ManagedAsset_Min <= asset_data[count].TypeId && asset_data[count].TypeId <= Ektron.Cms.Common.EkConstants.ManagedAsset_Max)
                    {
                        if ("*" == asset_data[count].PluginType)
                            lAddMultiType = asset_data[count].TypeId;
                        else
                        {
                            result.Append("<option value='" + asset_data[count].TypeId + "'");
                            if (asset_data[count].TypeId == lContentType)
                            {
                                result.Append(" selected");
                                bSelectedFound = true;
                            }
                            result.Append(">" + GetMessageText(asset_data[count].CommonName) + "</option>");
                        }
                    }
                }
                result.Append("</select></td>");
            }
            result.Append("<script language=\"Javascript\">\n");
            result.Append("<!--//\n");
            result.Append("var replaceQueryString = \"\"\n");
            result.Append("function BuildQueryString() {\n");
            result.Append("    replaceQueryString = \"\"\n");
            result.Append("    var search = location.href.split(\"?\");\n");
            result.Append("    if (search.length > 1){ \n");
            result.Append("        var vals=search[1].split(\"&\");\n");
            result.Append("        var request= new Array(); \n");
            result.Append("        for (var i in vals) { \n");
            result.Append("            vals[i] = vals[i].replace(/\\+/g, \" \").split(\"=\"); \n");
            result.Append("            if (unescape(vals[i][0]).toLowerCase() != \"conttype\") { //we just ignore langtype.\n");
            result.Append("                if (replaceQueryString == \"\") { replaceQueryString = unescape(vals[i][0]) + \"=\" + (vals[i][1]) } \n");
            result.Append("                else { replaceQueryString += \"&\" +  unescape(vals[i][0]) + \"=\" + (vals[i][1])}\n");
            result.Append("            }\n");
            result.Append("            request[unescape(vals[i][0])] =unescape(vals[i][1]); \n");
            result.Append("        }\n");
            result.Append("    }\n");
            result.Append("}\n");
            result.Append("BuildQueryString();\n");
            result.Append("//-->\n");
            result.Append("</script>\n");
            litResult.Text = result.ToString();
        }
        PopulateGridData(true);
    }
    public string GetMessageText(string st)
    {
        if (st == "office documents")
            st = this.m_refMsg.GetMessage("lbl office documents");
        else if (st == "managed files")
            st = this.m_refMsg.GetMessage("lbl managed files");
        else if (st == "Multimedia")
            st = this.m_refMsg.GetMessage("lbl multimedia");
        else if (st == "Open Office")
            st = this.m_refMsg.GetMessage("lbl open office");
        else if (st == "Images")
            st = this.m_refMsg.GetMessage("generic images");
        else if (st == "Forms/Survey")
            st = this.m_refMsg.GetMessage("generic FormsSurvey");
        else if (st == "Non Image Managed Files")
            st = this.m_refMsg.GetMessage("Non Image Managed Files");
        else if (st == "PDF")
            st = this.m_refMsg.GetMessage("generic pdf");
        else if (st.ToLower() == "managed asset")
            st = this.m_refMsg.GetMessage("managed asset");

        return st;
    }
    protected void NavigationLink_Click(object sender, CommandEventArgs e)
    {
        switch (e.CommandName)
        {
            case "First":
                _currentPageNumber = 1;
                break;
            case "Last":
                _currentPageNumber = int.Parse((string)TotalPages.Text);
                break;
            case "Next":
                _currentPageNumber = System.Convert.ToInt32(int.Parse((string)CurrentPage.Text) + 1);
                break;
            case "Prev":
                _currentPageNumber = System.Convert.ToInt32(int.Parse((string)CurrentPage.Text) - 1);
                break;
        }
        FillContentFolderInfo();
    }
    private void FillContentFolderInfo()
    {
        try
        {
            bool bCheckout = false;
            NextPage.Attributes.Add("onclick", "return resetPostback()");
            lnkBtnPreviousPage.Attributes.Add("onclick", "return resetPostback()");
            FirstPage.Attributes.Add("onclick", "return resetPostback()");
            LastPage.Attributes.Add("onclick", "return resetPostback()");
            m_refMsg = m_refContentApi.EkMsgRef;
            //Put user code to initialize the page here
            AppImgPath = m_refContentApi.AppImgPath;
            AppName = m_refContentApi.AppName;
            ContentIcon = "<img src=\"" + AppPath + "images/UI/Icons/contentHtml.png\" alt=\"Content\" />"; //-HC-
            WebEventIcon = "<img src=\"" + AppPath + "images/UI/Icons/calendarViewDay.png\" alt=\"WebEvent\" />";
            formsIcon = "<img src=\"" + AppPath + "images/UI/Icons/contentForm.png\" alt=\"Form\" />"; //-HC-
            pageIcon = "<img src=\"" + AppPath + "images/UI/Icons/layout.png\" alt=\"Page\" />"; //-HC-
            //intQString = Request.QueryString
            AddType = Strings.LCase(Request.QueryString["addto"]);
            nId = Convert.ToInt64(Request.QueryString["nid"]);
            subfolderid = Convert.ToInt64(Request.QueryString["subfolderid"]);
            FolderId = Convert.ToInt64(Request.QueryString["folderid"]);
            bool showQDContentOnly = System.Convert.ToBoolean(Request.QueryString["qdo"] == "1");

            m_refContent = m_refContentApi.EkContentRef;

            if (!(Request.QueryString["subfolderid"] == null))
            {
                locID = subfolderid;
            }
            else
            {
                locID = FolderId;
            }
            gtNavs = m_refContent.GetFolderInfoWithPath(locID);
            if (!Ektron.Cms.Common.EkFunctions.DoesKeyExist(gtNavs, "FolderName"))
            {
                // invalid folder, so start at root instead of taking an error
                locID = 0;
                gtNavs = m_refContent.GetFolderInfoWithPath(0);
            }

            //Set content type
            if (Request.QueryString[Ektron.Cms.Common.EkConstants.ContentTypeUrlParam] != "")
            {
                if (Information.IsNumeric(Request.QueryString[Ektron.Cms.Common.EkConstants.ContentTypeUrlParam]))
                {
                    g_ContentTypeSelected = System.Convert.ToInt32(Request.QueryString[Ektron.Cms.Common.EkConstants.ContentTypeUrlParam]);
                    m_refContentApi.SetCookieValue(Ektron.Cms.Common.EkConstants.ContentTypeUrlParam, g_ContentTypeSelected.ToString());
                }
            }
            else if (Request.Cookies["ecm"][Ektron.Cms.Common.EkConstants.ContentTypeUrlParam] != "")
            {
                if (Information.IsNumeric(Request.Cookies["ecm"][Ektron.Cms.Common.EkConstants.ContentTypeUrlParam]))
                {
                    g_ContentTypeSelected = System.Convert.ToInt32(Request.Cookies["ecm"][Ektron.Cms.Common.EkConstants.ContentTypeUrlParam]);
                }
            }

            ///end set content type

            if (AddType == "menu")
            {
                if (m_refContentApi.TreeModel == 1)
                {
                    cRecursive = m_refContent.GetMenuByID(nId, 0, false);
                }
                else
                {
                    cRecursive = m_refContent.GetMenuByID(nId, 0, false);
                }
                if (cRecursive.Count > 0)
                {
                    MenuTitle = cRecursive["MenuTitle"].ToString();
                    rec = Convert.ToBoolean(cRecursive["Recursive"]);
                }
            }
            else
            {
                if (Request.QueryString["checkout"] != null)
                {
                    bCheckout = System.Convert.ToBoolean(Request.QueryString["checkout"].ToString());
                }
                if (bCheckout)
                {
                    cRecursive = m_refContent.GetEcmStageCollectionByID(nId, false, false, ref ErrorString, true, false, true);
                }
                else
                {
                    cRecursive = m_refContent.GetEcmCollectionByID(nId, false, false, ref ErrorString, true, false, true);
                }
                if (cRecursive.Count > 0)
                {
                    CollectionTitle = cRecursive["CollectionTitle"].ToString();
                    rec = Convert.ToBoolean(cRecursive["Recursive"]);
                }

            }
            perm_data = m_refContentApi.LoadPermissions(locID, "folder", 0);
            if (!perm_data.CanTraverseFolders)
            {
                rec = false;
            }
            if (rec)
            {
                cTmp = new Collection();

                cTmp.Add(locID, "ParentID", null, null);
                cTmp.Add("name", "OrderBy", null, null);
                cFolders = m_refContent.GetAllViewableChildFoldersv2_0(cTmp);
            }

            FolderName = gtNavs["FolderName"].ToString();
            backId = Convert.ToInt64(gtNavs["ParentID"]);
            fPath = gtNavs["Path"].ToString();

            EkEnumeration.FolderType folderType = (EkEnumeration.FolderType)Enum.Parse(typeof(EkEnumeration.FolderType), gtNavs["FolderType"].ToString());

            cTmp = new Collection();
            cTmp.Add("name", "OrderBy", null, null);
            cTmp.Add(FolderId, "FolderID", null, null);
            cTmp.Add(FolderId, "ParentID", null, null);
            EkEnumeration.CMSContentType ContentTypeSelected = (EkEnumeration.CMSContentType)Enum.Parse(typeof(EkEnumeration.CMSContentType), g_ContentTypeSelected.ToString());

            if (AddType == "menu")
            {
                cConts = m_refContent.GetAllContentNotInEcmMenu(nId, locID, "title", _currentPageNumber, m_refContentApi.RequestInformationRef.PagingSize, ref TotalPagesNumber, ContentTypeSelected);
            }
            else
            {
                if (bCheckout || ((Request.QueryString["status"] != null) && Request.QueryString["status"].ToUpper() == "O"))
                {
                    cConts = m_refContent.GetAllContentNotInEcmCollection(nId, locID, "title", _currentPageNumber, m_refContentApi.RequestInformationRef.PagingSize, ref TotalPagesNumber, ContentTypeSelected, Ektron.Cms.Content.EkContent.ContentResultType.Staged);
                }
                else
                {
                    // Defect#: 49013
                    // cConts = m_refContent.GetAllContentNotInEcmCollection(nId, CLng(locID), "title", _currentPageNumber, m_refContentApi.RequestInformationRef.PagingSize, TotalPagesNumber, g_ContentTypeSelected)

                    EkEnumeration.CMSContentType myContentType = (EkEnumeration.CMSContentType)Enum.Parse(typeof(EkEnumeration.CMSContentType), g_ContentTypeSelected.ToString());
                    cConts = GetAllContent(nId, locID, "title", _currentPageNumber, m_refContentApi.RequestInformationRef.PagingSize, ref TotalPagesNumber, myContentType);
                }
            }

            if (showQDContentOnly && (gtNavs["ReplicationMethod"].ToString() != "1") && m_refContentApi.RequestInformationRef.EnableReplication)
            {
                // only display QD content, clean out the content list
                cConts.Clear();
            }
            gtNavs = null;

            if (TotalPagesNumber <= 1)
            {
                TotalPages.Visible = false;
                CurrentPage.Visible = false;
                lnkBtnPreviousPage.Visible = false;
                NextPage.Visible = false;
                LastPage.Visible = false;
                FirstPage.Visible = false;
                PageLabel.Visible = false;
                OfLabel.Visible = false;
            }
            else
            {

                TotalPages.Visible = true;
                CurrentPage.Visible = true;
                lnkBtnPreviousPage.Visible = true;
                NextPage.Visible = true;
                LastPage.Visible = true;
                FirstPage.Visible = true;
                PageLabel.Visible = true;
                OfLabel.Visible = true;
                TotalPages.Text = (System.Math.Ceiling(Convert.ToDouble(TotalPagesNumber))).ToString();
                TotalPages.ToolTip = TotalPages.Text;

                    CurrentPage.Text = _currentPageNumber.ToString();
                CurrentPage.ToolTip = CurrentPage.Text;

                if (_currentPageNumber == 1)
                {

                    lnkBtnPreviousPage.Enabled = false;

                    if (TotalPagesNumber > 1)
                    {
                        NextPage.Enabled = true;
                    }
                    else
                    {
                        NextPage.Enabled = false;
                    }

                }
                else
                {

                    lnkBtnPreviousPage.Enabled = true;

                    if (_currentPageNumber == TotalPagesNumber)
                    {
                        NextPage.Enabled = false;
                    }
                    else
                    {
                        NextPage.Enabled = true;
                    }

                }
            }

            Ektron.Cms.TemplateData[] templatelist = m_refContentApi.GetEnabledTemplatesByFolder(locID);
            bool hasNormalTemplate = false;


            foreach (TemplateData template in templatelist)
            {
                if (template.Type == EkEnumeration.TemplateType.Default && template.SubType == EkEnumeration.TemplateSubType.Default)
                {
                    hasNormalTemplate = true;
                    break;
                }
            }
            CanCreateContent = System.Convert.ToBoolean((perm_data.CanAdd && hasNormalTemplate) && !(folderType == EkEnumeration.FolderType.Catalog || folderType == EkEnumeration.FolderType.Calendar));

            asset_data = m_refContent.GetAssetSuperTypes();

            if (Ektron.Cms.Common.EkConstants.CMSContentType_Content == g_ContentTypeSelected || Ektron.Cms.Common.EkConstants.CMSContentType_Archive_Content == g_ContentTypeSelected)
            {
                lContentType = g_ContentTypeSelected;
            }
            else if (Ektron.Cms.Common.EkConstants.ManagedAsset_Min <= g_ContentTypeSelected & g_ContentTypeSelected <= Ektron.Cms.Common.EkConstants.ManagedAsset_Max)
            {
                if (DoesAssetSupertypeExist(asset_data, g_ContentTypeSelected))
                {
                    lContentType = g_ContentTypeSelected;
                }
            }
            DisplayToolBarAndContent();
        }
        catch (Exception ex)
        {
            Utilities.ShowError(ex.Message);
        }

    }
    public bool DoesAssetSupertypeExist(AssetInfoData[] asset_data, int lContentType)
    {
        int i = 0;
        bool result = false;
        if (!(asset_data == null))
        {
            for (i = 0; i <= asset_data.Length - 1; i++)
            {
                if (Ektron.Cms.Common.EkConstants.ManagedAsset_Min <= asset_data[i].TypeId && asset_data[i].TypeId <= Ektron.Cms.Common.EkConstants.ManagedAsset_Max)
                {
                    if (asset_data[i].TypeId == lContentType)
                    {
                        result = true;
                        break;
                    }
                }
            }
        }
        return (result);
    }
    protected EkContentCol GetAllContent(long nID, long RequestedFolderID, string OrderBy, int currentPage, int pageSize, ref int totalPages, EkEnumeration.CMSContentType ContentType)
    {
        EkContentCol returnValue;
        Collection cCollection = new Collection();
        Collection cTmp = new Collection();
        Collection cTmps = new Collection();
        EkContentCol cAllViewable = new EkContentCol();
        // Dim lLoop As Integer
        bool bIsAllowed = false;
        Ektron.Cms.CommonApi api = new Ektron.Cms.CommonApi();
        object GetContents = "";

        try
        {
            returnValue = null;

            bIsAllowed = System.Convert.ToBoolean(m_refContent.IsAllowed(RequestedFolderID, 0, "Folder", "Collections", 0) 
                || m_refContent.IsARoleMember(Convert.ToInt64(Ektron.Cms.Common.EkEnumeration.CmsRoleIds.AminCollectionMenu), api.RequestInformationRef.CallerId, false)
                || m_refContent.IsARoleMember(Convert.ToInt64(Ektron.Cms.Common.EkEnumeration.CmsRoleIds.AdminCollection), api.RequestInformationRef.CallerId, false));
            if (!bIsAllowed)
            {
                throw (new Exception(m_refContentApi.EkMsgRef.GetMessage("com: user does not have permission")));
            }

            string strReturn = "";
            cCollection = m_refContent.pGetEcmCollectionByID(nID, false, false, ref strReturn, EkContent.ContentResultType.Published, GetContents, true, true);
            if (cCollection.Count == 0)
            {
                throw (new Exception(m_refContentApi.EkMsgRef.GetMessage("com: collection not found")));
            }

            cTmp.Add(RequestedFolderID, "FolderID", null, null);
            cTmp.Add(OrderBy, "OrderBy", null, null);
            cTmp.Add(1, "FilterContentAssetType", null, null);
            if (ContentType != Ektron.Cms.Common.EkEnumeration.CMSContentType.AllTypes)
            {
                cTmp.Add(ContentType, "ContentType", null, null);
            }
            //m_refContent.RequestInformation.ContentLanguage = AppUI.ContentLanguage;
            EkEnumeration.CMSContentSubtype subType = (EkEnumeration.CMSContentSubtype)Enum.Parse(typeof(EkEnumeration.CMSContentSubtype), "-1");
            cAllViewable = m_refContent.GetAllViewableChildInfov5_0(cTmp, currentPage, pageSize, ref totalPages, ContentType, subType);
            CollectionContentItems = (Collection)cCollection["Contents"];
            returnValue = cAllViewable;
        }
        catch (Exception)
        {
            return null;
        }
        finally
        {
            cCollection = null;
            cTmp = null;
            cTmps = null;
            cAllViewable = null;
        }

        return returnValue;
    }
    public string getContentTypeIcon(Ektron.Cms.Common.ContentBase objCont)
    {
        try
        {
            int ContentTypeID;
            string strAssetIcon;
            Ektron.Cms.Common.ContentBase contentObj;

            contentObj = (Ektron.Cms.Common.ContentBase)objCont;

            ContentTypeID = System.Convert.ToInt32(objCont.ContentType);
            if (ContentTypeID == 2)
            {
                return (formsIcon);
            }
            else if (ContentTypeID > Ektron.Cms.Common.EkConstants.ManagedAsset_Min && ContentTypeID < Ektron.Cms.Common.EkConstants.ManagedAsset_Max)
            {
                try
                {
                    strAssetIcon = (string)objCont.AssetInfo.ImageUrl;
                    strAssetIcon = "<img src=\"" + strAssetIcon + "\" alt=\"Asset\">";
                    return (strAssetIcon);
                }
                catch (Exception)
                {
                    return ("<img src=\"" + AppPath + "images/ui/icons/filetypes/file.png\" alt=\"Content\" />");
                }
            }
            else if (ContentTypeID == 3333)
            {
                Ektron.Cms.Commerce.CatalogEntry catalogEntry = new Ektron.Cms.Commerce.CatalogEntry();
                catalogEntry.RequestInformation = m_refContent.RequestInformation;
                Ektron.Cms.Commerce.EntryData catalogEntryData;

                catalogEntryData = catalogEntry.GetItem(contentObj.Id, long.Parse(contentObj.Language.ToString()));
                return ("<img title=\"" + catalogEntryData.Title + "\" src=\"" + GetProductIcon(catalogEntryData.EntryType) + "\" alt=\"" + catalogEntryData.Title + "\" />");
            }
            else
            {
                if (Convert.ToInt32(objCont.ContentSubType) == 1)
                {
                    return (pageIcon);
                }
                else if (Convert.ToInt32(objCont.ContentSubType) == 2)
                {
                    return (WebEventIcon);
                }
                return (ContentIcon);
            }
        }
        catch (Exception)
        {
            return (ContentIcon);
        }
    }
    protected string GetProductIcon(EkEnumeration.CatalogEntryType entryType)
    {
        string productImage;
        if (entryType == Ektron.Cms.Common.EkEnumeration.CatalogEntryType.Bundle)
        {
            productImage = m_refContentApi.ApplicationPath + "images/ui/icons/package.png";
        }
        else if (entryType == Ektron.Cms.Common.EkEnumeration.CatalogEntryType.ComplexProduct)
        {
            productImage = m_refContentApi.ApplicationPath + "images/ui/icons/bricks.png";
        }
        else if (entryType == Ektron.Cms.Common.EkEnumeration.CatalogEntryType.Kit)
        {
            productImage = m_refContentApi.ApplicationPath + "images/ui/icons/box.png";
        }
        else if (entryType == Ektron.Cms.Common.EkEnumeration.CatalogEntryType.SubscriptionProduct)
        {
            productImage = m_refContentApi.ApplicationPath + "images/ui/icons/bookGreen.png";
        }
        else
        {
            productImage = m_refContentApi.ApplicationPath + "images/ui/icons/brick.png";
        }

        return productImage;
    }
    private void PopulateGridData(bool ShowFolders)
    {
        bool showQDContentOnly = System.Convert.ToBoolean(Request.QueryString["qdo"] == "1");
        string enableQDOparam = "";
        if (showQDContentOnly)
        {
            enableQDOparam = "&qdo=1";
        }
        System.Web.UI.WebControls.BoundColumn colBound = new System.Web.UI.WebControls.BoundColumn();
        bool bDoNotShow = false;
        colBound.DataField = "ITEM1";
        colBound.HeaderText = "";
        colBound.ItemStyle.CssClass = "info";
        colBound.ItemStyle.Wrap = false;
        colBound.HeaderStyle.Height = Unit.Percentage(0);
        ContentGrid.Columns.Add(colBound);

        colBound = new System.Web.UI.WebControls.BoundColumn();
        colBound.DataField = "ITEM2";
        colBound.HeaderText = "";
        colBound.ItemStyle.CssClass = "info";
        colBound.ItemStyle.Wrap = false;
        colBound.HeaderStyle.Height = Unit.Percentage(0);
        ContentGrid.Columns.Add(colBound);

        colBound = new System.Web.UI.WebControls.BoundColumn();
        colBound.DataField = "ITEM3";
        colBound.HeaderText = "";
        colBound.ItemStyle.CssClass = "info";
        colBound.ItemStyle.Wrap = false;
        colBound.HeaderStyle.Height = Unit.Percentage(0);
        ContentGrid.Columns.Add(colBound);


        DataTable dt = new DataTable();
        DataRow dr;
        dt.Columns.Add(new DataColumn("ITEM1", typeof(string)));
        dt.Columns.Add(new DataColumn("ITEM2", typeof(string)));
        dt.Columns.Add(new DataColumn("ITEM3", typeof(string)));

        dr = dt.NewRow();
        if (ShowFolders)
        {
            dr[0] = MsgHelper.GetMessage("alt Please select content by navigating the folders below") + "<br>";
        }
        else
        {
            dr[0] = "";
        }
        dr[1] = "remove-item";
        dr[2] = "remove-item";
        dt.Rows.Add(dr);

        dr = dt.NewRow();
        dr[0] = "Path: <span class=\"pathInfo\">" + fPath + "</span>";
        dr[1] = "remove-item";
        dr[2] = "remove-item";
        dt.Rows.Add(dr);

        dr = dt.NewRow();
        if (locID != 0 && rec)
        {
            string linkMarkup = "";
            linkMarkup = (string)("<a href=\"collections.aspx?Action=AddLink" + status + "&LangType=" + ContentLanguage + "&nId=" + nId + "&folderid=" + FolderId + "&subfolderid=" + (backId) + enableQDOparam);
            if (!String.IsNullOrEmpty(Request.QueryString["back"]))
            {
                linkMarkup = linkMarkup + "&back=" + EkFunctions.UrlEncode(Request.QueryString["back"]);
            }
            if (!String.IsNullOrEmpty(Request.QueryString["noworkarea"]))
            {
                linkMarkup = linkMarkup + "&noworkarea=" + Request.QueryString["noworkarea"];
            }
            if (AddType == "menu")
            {
                linkMarkup = linkMarkup + "&addto=" + (AddType);
                linkMarkup = linkMarkup + "&iframe=" + Request.QueryString["iframe"] + NoWorkAreaAttribute;
            }
            linkMarkup = linkMarkup + "&title=\"" + m_refMsg.GetMessage("alt: generic previous dir text") + "\">";
            m_refContent = m_refContentApi.EkContentRef;
            //Users might not have collection rights with the parent folders # Defect: 47566
            if (AddType != "menu" && (m_refContent.IsAllowed(backId, 0, "Folder", "Collections", 0)) || m_refContent.IsARoleMember((long)EkEnumeration.CmsRoleIds.AminCollectionMenu, m_refContent.RequestInformation.CallerId, false) || m_refContent.IsARoleMember((long)EkEnumeration.CmsRoleIds.AdminCollection, m_refContent.RequestInformation.CallerId, false))
            {
                dr[0] = linkMarkup + "<img src=\"" + AppPath + "images/UI/Icons/folderUp.png" + "\" border=\"0\" title=\"" + m_refMsg.GetMessage("alt: generic previous dir text") + "\" alt=\"" + m_refMsg.GetMessage("alt: generic previous dir text") + "\"  /></a>" + linkMarkup + "..</a>";
            }
            else if (AddType == "menu")
            {
                dr[0] = linkMarkup + "<img src=\"" + AppPath + "images/UI/Icons/folderUp.png" + "\" border=\"0\" title=\"" + m_refMsg.GetMessage("alt: generic previous dir text") + "\" alt=\"" + m_refMsg.GetMessage("alt: generic previous dir text") + "\"  /></a>" + linkMarkup + "..</a>";
            }
        }
        dr[1] = "&nbsp;";
        dr[2] = "&nbsp;";
        dt.Rows.Add(dr);


        if (cFolders != null)
        {
            foreach (Collection folder in cFolders)
            {
                EkEnumeration.FolderType foldertype = (EkEnumeration.FolderType)Enum.Parse(typeof(EkEnumeration.FolderType), folder["FolderType"].ToString());
                if ((EkEnumeration.FolderType.Content == foldertype) || (EkEnumeration.FolderType.Blog == foldertype) || (EkEnumeration.FolderType.Domain == foldertype) || (EkEnumeration.FolderType.Catalog == foldertype) || (EkEnumeration.FolderType.Calendar == foldertype) || (EkEnumeration.FolderType.Community == foldertype))
                {
                    dr = dt.NewRow();

                    dr[0] += "<a href=\"collections.aspx?Action=AddLink" + status + "&LangType=" + ContentLanguage + "&nId=" + nId + "&folderid=" + FolderId + "&subfolderid=" + folder["ID"] + NoWorkAreaAttribute + enableQDOparam;
                    if (AddType == "menu")
                    {
                        dr[0] += "&addto=" + AddType + "&iframe=" + Request.QueryString["iframe"];
                    }
                    if (!string.IsNullOrEmpty(Request.QueryString["back"]))
                    {
                        dr[0] = dr[0] + "&back=" + Server.UrlEncode(Request.QueryString["back"]);
                    }

                    string linkValue = dr[0].ToString();
                    string folder_img = "";

                    EkEnumeration.FolderType folderType = (EkEnumeration.FolderType)Enum.Parse(typeof(EkEnumeration.FolderType), folder["FolderType"].ToString());
                    switch (folderType)
                    {
                        case EkEnumeration.FolderType.Domain:
                            folder_img = (string)(AppPath + "images/UI/Icons/folderSite.png");
                            break;
                        case EkEnumeration.FolderType.Community:
                            folder_img = (string)(AppPath + "images/UI/Icons/folderCommunity.png");
                            break;
                        case EkEnumeration.FolderType.Catalog:
                            folder_img = (string)(AppPath + "images/UI/Icons/folderGreen.png");
                            break;
                        case EkEnumeration.FolderType.Calendar:
                            folder_img = (string)(AppPath + "images/UI/Icons/folderCalendar.png");
                            break;
                        default:
                            folder_img = (string)(AppPath + "images/UI/Icons/folder.png");
                            break;
                    }

                    dr[0] += "&title=\"" + m_refMsg.GetMessage("alt: generic view folder content text") + "\"><img src=\"" + folder_img + "\" title=\"" + m_refMsg.GetMessage("alt: generic view folder content text") + "\" alt=\"" + m_refMsg.GetMessage("alt: generic view folder content text") + "\"  /></a>" + linkValue + ("&title=\"" + m_refMsg.GetMessage("alt: generic view folder content text")) + "\">" + folder["Name"].ToString() + "</a>";
                    if (m_refContentApi.RequestInformationRef.EnableReplication && (Convert.ToInt32(folder["ReplicationMethod"]) == 1))
                    {
                        dr[0] += "&nbsp; (QuickDeploy)";
                    }
                    dr[1] = "&nbsp;";
                    dr[2] = "&nbsp;";
                    dt.Rows.Add(dr);
                }
            }
        }

        string ContentName;
        int lLoop = 0;
        string cLinkArray = "";
        string fLinkArray = "";
        string cLanguagesArray = "";
        bool IsAdded = false;
        // For displaying child content - exclude this for reports
        foreach (ContentBase contInfo in cConts)
        {
            if ((Ektron.Cms.Common.EkConstants.CMSContentType_AllTypes == g_ContentTypeSelected) || (Convert.ToInt32(contInfo.ContentType) == g_ContentTypeSelected))
            {
                bDoNotShow = false;
                IsAdded = IsContentInEcmCollection(contInfo.Id);
                if (contInfo.ContentType == Ektron.Cms.Common.EkEnumeration.CMSContentType.Forms)
                {
                    ContentName = (string)(AppPath + "linkit.aspx?LinkIdentifier=ekfrm&ItemID=" + contInfo.Id); //m_refContent.GetContentFormlink(contInfo.Id, contInfo.FolderId)
                }
                else if (((Convert.ToInt32(contInfo.ContentType) >= Ektron.Cms.Common.EkConstants.ManagedAsset_Min && Convert.ToInt32(contInfo.ContentType) <= Ektron.Cms.Common.EkConstants.ManagedAsset_Max) || (contInfo.ContentType == Ektron.Cms.Common.EkEnumeration.CMSContentType.Assets)) && (contInfo.ContentType != Ektron.Cms.Common.EkEnumeration.CMSContentType.Multimedia))
                {
                    ContentName = (string)(AppPath + "linkit.aspx?LinkIdentifier=ID&ItemID=" + contInfo.Id); //m_refContent.GetContentQlink(contInfo.Id, contInfo.FolderId)
                    if (ContentName.ToLower().IndexOf("javascript") + 1 > 0)
                    {
                        ContentName = "";
                    }
                }
                else if ((contInfo.ContentType == Ektron.Cms.Common.EkEnumeration.CMSContentType.Content) || (contInfo.ContentType == Ektron.Cms.Common.EkEnumeration.CMSContentType.Content) || (contInfo.ContentType == Ektron.Cms.Common.EkEnumeration.CMSContentType.Multimedia) || (contInfo.ContentType == Ektron.Cms.Common.EkEnumeration.CMSContentType.CatalogEntry))
                {
                    ContentName = (string)(AppPath + "linkit.aspx?LinkIdentifier=ID&ItemID=" + contInfo.Id); //m_refContent.GetContentQlink(contInfo.Id, contInfo.FolderId)
                }
                else
                {
                    //do not show
                    bDoNotShow = true;
                }
                if (contInfo.ContentSubType == Ektron.Cms.Common.EkEnumeration.CMSContentSubtype.PageBuilderMasterData)
                {
                    bDoNotShow = true;
                }
                if (!(bDoNotShow))
                {
                    dr = dt.NewRow();

                    if (IsAdded)
                    {
                        dr[0] = "&nbsp;&nbsp;" + getContentTypeIcon(contInfo) + "<input size= " + contInfo.Id + " type=\"hidden\" name=\"frm_hidden" + lLoop + "\" value=\"0\">" + "<input type=\"checkbox\" disabled=\"disabled\" checked=\"checked\" name=\"content\" value=\"" + contInfo.Id + "\" ID=\"content\">" + contInfo.Title + "<input  type=\"hidden\" name=\"frm_languages" + lLoop + "\" value=\"" + contInfo.Language + "\">";
                    }
                    else
                    {
                        dr[0] = "&nbsp;&nbsp;" + getContentTypeIcon(contInfo) + "<input size= " + contInfo.Id + " type=\"hidden\" name=\"frm_hidden" + lLoop + "\" value=\"0\">" + "<input type=\"checkbox\" name=\"content\" value=\"" + contInfo.Id + "\" ID=\"content\" onclick=\"document.forms[0][\'frm_hidden" + (lLoop) + "\'].value=(this.checked?" + contInfo.Id + " : 0);\">" + contInfo.Title + "<input  type=\"hidden\" name=\"frm_languages" + lLoop + "\" value=\"" + contInfo.Language + "\">";
                    }
                    dr[1] = "remove-item";
                    dr[2] = "remove-item";
                    dt.Rows.Add(dr);
                }
                cLinkArray = cLinkArray + "," + contInfo.Id;
                cLanguagesArray = cLanguagesArray + "," + contInfo.Language;
                fLinkArray = fLinkArray + "," + FolderId;
                lLoop++;
            }
        }
        if (cLinkArray.Length > 0)
        {
            cLinkArray = cLinkArray.Remove(0, 1);
            fLinkArray = fLinkArray.Remove(0, 1);
            cLanguagesArray = cLanguagesArray.Remove(0, 1);
        }
        DataView dv = new DataView(dt);
        ContentGrid.DataSource = dv;
        ContentGrid.DataBind();
    }
    protected bool IsContentInEcmCollection(long ContentID)
    {

        if (CollectionContentItems != null)
        {
            foreach (Collection cTmp in CollectionContentItems)
            {
                if (Convert.ToInt64(cTmp["ContentID"]) == ContentID)
                {
                    return true;
                }
            }
        }
        return false;
    }
    protected void Grid_ItemDataBound(object sender, DataGridItemEventArgs e)
    {
        switch (e.Item.ItemType)
        {
            case ListItemType.AlternatingItem:
            case ListItemType.Item:
                if (e.Item.Cells[1].Text.Equals("remove-item") && e.Item.Cells[2].Text.Equals("remove-item"))
                {
                    e.Item.Cells[0].ColumnSpan = 3;
                    e.Item.Cells.RemoveAt(2);
                    e.Item.Cells.RemoveAt(1);
                }
                break;
        }

    }
}