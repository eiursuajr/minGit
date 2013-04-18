using System;
using System.Collections.Generic;
using System.Text;
using System.Web;
using System.Web.UI;
using Ektron.Cms;
using Ektron.Cms.API;
using Ektron.Cms.Common;
using Ektron.Cms.Content;
using Ektron.Cms.Framework.UI;
using Microsoft.VisualBasic;


public partial class Workarea_DmsMetadata : System.Web.UI.Page
{
    public Workarea_DmsMetadata()
    {
        m_refMsg = m_ContentApi.EkMsgRef;

    }

    protected bool TaxonomyRoleExists = false;
    protected Ektron.Cms.ContentAPI m_ContentApi = new Ektron.Cms.ContentAPI();
    protected EkMessageHelper m_refMsg;
    protected string m_idString = "";
    protected StyleHelper m_refStyle = new StyleHelper();
    protected string AppImgPath = "";
    protected ContentAPI m_refContentApi = new ContentAPI();
    protected long m_folderId = -1;
    protected Collection page_meta_data;
    protected string m_intTaxFolderId = "0"; // note: this is used by an include file!

    private bool isFromMultiUpload
    {
        get
        {
            bool ret = false;
            if (Request.QueryString["displayUrlAlias"] != null || Request.Form["isFromMultiUpload"] == "true")
            {
                ret = true;
            }
            return ret;
        }
    }
    private System.Collections.Hashtable htImagesAssets;
    protected void Page_Load(object sender, System.EventArgs e)
    {
        long contentId = -1;
        Utilities.ValidateUserLogin();

        AppImgPath = (string)m_refContentApi.AppImgPath;
        ltrStyleSheetJs.Text = (new StyleHelper()).GetClientScript();
        EnhancedMetadataScript.Text = Ektron.Cms.CustomFields.GetEnhancedMetadataScript();
        EnhancedMetadataArea.Text = Ektron.Cms.CustomFields.GetEnhancedMetadataArea();
        RegisterResources();
        if (String.IsNullOrEmpty(Request.QueryString["idString"]))
        {
            m_idString = "";
        }
        else
        {
            m_idString = Request.QueryString["idString"];
        }

        if (!String.IsNullOrEmpty(Request.QueryString["folderId"]))
        {
            m_folderId = Convert.ToInt64(Request.QueryString["folderId"]);
            ltrTaxFolderId.Text = m_folderId.ToString();
            m_intTaxFolderId = m_folderId.ToString();
        }
        if ((m_ContentApi.EkContentRef.IsAllowed(m_folderId, 0, "folder", "add", m_ContentApi.RequestInformationRef.UserId) == false) || m_ContentApi.RequestInformationRef.UserId == 0)
        {
            Response.Redirect(m_ContentApi.ApplicationPath + "reterror.aspx?info=" + Server.UrlEncode(m_refMsg.GetMessage("msg login cms user")), false);
            return;
        }

        ltrActionPage.Text = "DMSMetadata.aspx?idString=" + EkFunctions.HtmlEncode(m_idString) + "&folderId=" + m_folderId;
        if (!string.IsNullOrEmpty(Request.QueryString["TaxonomyId"]))
        {
            ltrActionPage.Text += "&TaxonomyId=" + Request.QueryString["TaxonomyId"];
            jsTaxRedirectID.Text = Request.QueryString["TaxonomyId"];
        }
        if (!String.IsNullOrEmpty(Request.QueryString["action"]))
        {
            if (Request.QueryString["action"] == "Submit")
            {
                PublishContent();
                if (Request.QueryString["close"] == "true")
                {
                    string uniqueKey = Convert.ToString(m_ContentApi.UserId) + Convert.ToString(m_ContentApi.UniqueId) + "RejectedFiles";
                    if (HttpContext.Current != null && HttpContext.Current.Session[uniqueKey] != null && HttpContext.Current.Session[uniqueKey].ToString().Length > 0)
                    {
                        string failedUpload = Convert.ToString(HttpContext.Current.Session[uniqueKey]);
                        failedUpload = failedUpload.Replace("\\", "\\\\");
                        failedUpload = failedUpload.Replace("'", "\\'");
                        this.jsInvalidFileTypeMsg.Text = m_refMsg.GetMessage("lbl error message for multiupload") + " " + failedUpload;
                        HttpContext.Current.Session.Remove(uniqueKey);
                    }
                    else
                    {
                        this.jsInvalidFileTypeMsg.Text = "";
                    }
                    Page.ClientScript.RegisterClientScriptBlock(this.GetType(), "_closeTBScript", "CloseThickBoxandReload();", true);
                }
                else if (Request.QueryString["closeWindow"] == "true")
                {
                    Page.ClientScript.RegisterClientScriptBlock(this.GetType(), "_refreshTop", "top.location.href = top.location.href;", true);
                    Response.Redirect("close.aspx");
                }
                else
                {
                    Response.Redirect((string)("content.aspx?action=ViewContentByCategory&id=" + m_folderId));
                }
            }
        }

        if (!String.IsNullOrEmpty(Request.QueryString["action"]))
        {
            if (Request.QueryString["action"] == "Cancel")
            {
                if (Request.QueryString["close"] == "true")
                {
                    Page.ClientScript.RegisterClientScriptBlock(this.GetType(), "_closeTBScript", "if(parent != null && typeof parent.ektb_remove == \'function\'){parent.ektb_remove();}", true);
                }
                else if (Request.QueryString["closeWindow"] == "true")
                {
                    Page.ClientScript.RegisterClientScriptBlock(this.GetType(), "_refreshTop", "top.location.href = top.location.href;", true);
                    Response.Redirect("close.aspx");
                }
                else
                {
                    Response.Redirect((string)("content.aspx?action=ViewContentByCategory&id=" + m_folderId));
                }
            }
        }


        if (String.IsNullOrEmpty(Request.QueryString["contentId"]))
        {
            contentId = -1;
        }
        else
        {
            try
            {
                contentId = Convert.ToInt64(Request.QueryString["contentId"]); // should always be an integer unles an error is returned from the ajax page
            }
            catch
            {
                contentId = -1;
            }
        }

        if (contentId != -1 && m_folderId != -1)
        {
            myMetadata.Text = CaptureMetadata(contentId, m_folderId).ToString();
            myTaxonomy.Text = CaptureTaxonomy(contentId, m_folderId);
            DisplayUrlAlias(contentId, m_folderId);
        }
        else
        {
            myMetadata.Text = "<p class=" + "\"" + "nodata" + "\"" + ">No Metadata Available.</p>";
            myTaxonomy.Text = "<p class=" + "\"" + "nodata" + "\"" + ">No Taxonomy Data Available.</p>";
        }
        if (string.IsNullOrEmpty(myMetadata.Text) && jsURLRequired.Text == "false" && myTaxonomy.Text.ToLower() == "<div id=\"emptytree\"></div>")
        {
            // nothing to dispaly close the window
            if (Request.QueryString["close"] == "true")
            {
                Page.ClientScript.RegisterClientScriptBlock(this.GetType(), "_closeTBScript", "CloseThickBoxandReload();", true);
            }
            else if (Request.QueryString["closeWindow"] == "true")
            {
                Page.ClientScript.RegisterClientScriptBlock(this.GetType(), "_refreshTop", "top.location.href = top.location.href;", true);
                Response.Redirect("close.aspx");
            }
            else
            {
                Response.Redirect((string)("content.aspx?action=ViewContentByCategory&id=" + m_folderId));
            }
        }
        ToolBar();
    }

    private void DisplayUrlAlias(long contentId, long folderId)
    {
        bool required = false;
        ContentData cont = m_ContentApi.GetContentById(contentId);
        FolderData fdTmp = this.m_ContentApi.EkContentRef.GetFolderById(folderId);
        Ektron.Cms.UrlAliasing.UrlAliasSettingsApi m_urlAliasSettings = new Ektron.Cms.UrlAliasing.UrlAliasSettingsApi();
        
        if (m_urlAliasSettings.IsManualAliasEnabled)
        {
            if (fdTmp.AliasRequired)
            {
                required = true;
            }
        }
        if (required && !isFromMultiUpload)
        {
            if (Ektron.ASM.AssetConfig.ConfigManager.IsImageAsset(cont.AssetData.FileExtension))
                required = false;
        }
        if (required)
        {
            if (!isFromMultiUpload)
            {
                jsURLRequired.Text = "true";
                ltrShowUrlAlias.Text = "<li><a title=\"Set URL Alias\" id=\"urlAnchor\" onclick=\"dmsMetadataShowHideCategory('urlalias');return false;\" href=\"#\">URL Alias</a></li>";
                ltrUrlAliasBody.Text = EditAliasHtmlScripts(cont, fdTmp, m_urlAliasSettings);
            }
            else
            {
                jsURLRequired.Text = "false";
                ltrShowUrlAlias.Text = "<input type='hidden' id='isFromMultiUpload' name='isFromMultiUpload' value='true' />";
            }
        }
        else
        {
            jsURLRequired.Text = "false";
            ltrShowUrlAlias.Text = "";
        }
    }
    private string EditAliasHtmlScripts(ContentData content_edit_data, FolderData folder_data, Ektron.Cms.UrlAliasing.UrlAliasSettingsApi m_urlAliasSettings)
    {
        

        StringBuilder sbHtml = new StringBuilder();
        Ektron.Cms.UrlAliasing.UrlAliasManualApi m_aliaslist = new Ektron.Cms.UrlAliasing.UrlAliasManualApi();
        Ektron.Cms.UrlAliasing.UrlAliasAutoApi m_autoaliasApi = new Ektron.Cms.UrlAliasing.UrlAliasAutoApi();
        System.Collections.Generic.List<string> ext_alias;
        string ext = "";
        int i;

        Ektron.Cms.Common.UrlAliasManualData d_alias = new Ektron.Cms.Common.UrlAliasManualData(0, 0, string.Empty, string.Empty);
        System.Collections.Generic.List<UrlAliasAutoData> auto_aliaslist = new System.Collections.Generic.List<UrlAliasAutoData>();

        bool IsStagingServer;

        IsStagingServer = this.m_ContentApi.RequestInformationRef.IsStaging;

        ext_alias = m_aliaslist.GetFileExtensions();
        if (content_edit_data != null)
        {
            d_alias = m_aliaslist.GetDefaultAlias(content_edit_data.Id);
        }
        string m_strManualAliasExt = d_alias.AliasName;
        string m_strManualAlias = d_alias.FileExtension;

        //sbHtml.Append("<div id=\"dvAlias\">");
        if (m_urlAliasSettings.IsManualAliasEnabled)
        {
            if (m_ContentApi.IsARoleMember(Ektron.Cms.Common.EkEnumeration.CmsRoleIds.EditAlias))
            {
                sbHtml.Append("<input type=\"hidden\" name=\"frm_manalias_id\" value=\"" + d_alias.AliasId + "\">");
                sbHtml.Append("<input type=\"hidden\" id=\"prev_frm_manalias_name\" name=\"prev_frm_manalias_name\" value=\"" + d_alias.AliasName + "\">");
                sbHtml.Append("<input type=\"hidden\" name=\"prev_frm_manalias_ext\" value=\"" + d_alias.FileExtension + "\">");
                sbHtml.Append("<div class=\"ektronHeader\">" + m_refMsg.GetMessage("lbl tree url manual aliasing") + "</div>");
                sbHtml.Append("<table class=\"ektronForm\">");
                sbHtml.Append("<tr>");
                sbHtml.Append("<td class=\"label\">");
                sbHtml.Append(m_refMsg.GetMessage("lbl primary") + " " + m_refMsg.GetMessage("lbl alias name") + ":");
                sbHtml.Append("</td>");
                sbHtml.Append("<td class=\"value\">");

                if (IsStagingServer && folder_data.DomainStaging != string.Empty)
                {
                    sbHtml.Append("<td width=\"95%\">http://" + folder_data.DomainStaging + "/<input type=\"text\" id=\"frm_manalias\" size=\"35\" name=\"frm_manalias\" value=\"" + d_alias.AliasName + "\">");
                }
                else if (folder_data.IsDomainFolder)
                {
                    sbHtml.Append("http://" + folder_data.DomainProduction + "/<input type=\"text\" id=\"frm_manalias\" size=\"35\" name=\"frm_manalias\" value=\"" + d_alias.AliasName + "\">");
                }
                else
                {
                    sbHtml.Append(m_ContentApi.SitePath + "<input type=\"text\" id=\"frm_manalias\" size=\"35\" name=\"frm_manalias\" value=\"" + d_alias.AliasName + "\">");
                }
                for (i = 0; i <= ext_alias.Count - 1; i++)
                {
                    if (ext != "")
                    {
                        ext = ext + ",";
                    }
                    ext = ext + ext_alias[i];
                }
                sbHtml.Append(m_ContentApi.RenderHTML_RedirExtensionDD("frm_ManAliasExt", d_alias.FileExtension, ext));
                sbHtml.Append("</td>");
                sbHtml.Append("</tr>");
                sbHtml.Append("</table>");
                if (m_ContentApi.RedirectorManExt.IndexOf(",") + 1 <= 0)
                {
                    //ast_frm_manaliasExt.Value = m_ContentApi.RedirectorManExt;
                }
            }
           
        }
        return sbHtml.ToString();
       
    }
    private StringBuilder CaptureMetadata(long contentId, long folderId)
    {
        StringBuilder metadataOutput = new StringBuilder();
        ContentAPI myContentAPI = new ContentAPI();
        ContentData myContentEditData = new ContentData();
        ContentMetaData[] myContentMetadata;
        string myType = "update";
        int myCounter = 0;
        Ektron.Cms.Site.EkSite myEkSite = new Ektron.Cms.Site.EkSite();

        int ContentLanguage = EkConstants.CONTENT_LANGUAGES_UNDEFINED;

        if (Page.Request.QueryString["LangType"] != null)
        {
            if (Page.Request.QueryString["LangType"] != "")
            {
                ContentLanguage = Convert.ToInt32(Page.Request.QueryString["LangType"]);
                myContentAPI.SetCookieValue("LastValidLanguageID", ContentLanguage.ToString());
            }
            else
            {
                if (myContentAPI.GetCookieValue("LastValidLanguageID") != "")
                {
                    ContentLanguage = Convert.ToInt32(myContentAPI.GetCookieValue("LastValidLanguageID"));
                }
            }
        }
        else
        {
            if (myContentAPI.GetCookieValue("LastValidLanguageID") != "")
            {
                ContentLanguage = Convert.ToInt32(myContentAPI.GetCookieValue("LastValidLanguageID"));
            }
        }

        if (ContentLanguage == EkConstants.CONTENT_LANGUAGES_UNDEFINED)
        {
            myContentAPI.ContentLanguage = EkConstants.ALL_CONTENT_LANGUAGES;
        }
        else
        {
            myContentAPI.ContentLanguage = ContentLanguage;
        }

        if (contentId != -1)
        {
            myEkSite = myContentAPI.EkSiteRef;
            myContentEditData = myContentAPI.GetContentById(contentId, 0);
            myContentMetadata = myContentEditData.MetaData;

            if (myContentMetadata.Length > 0)
            {
                metadataOutput = Ektron.Cms.CustomFields.WriteFilteredMetadataForEdit(myContentMetadata, false, myType, folderId, ref myCounter, myEkSite.GetPermissions(folderId, 0, "folder"));
                if (metadataOutput.Length > 0)
                {
                    ltrShowMetadata.Text = "<li><a id=\"metadataAnchor\" href=\"#\" onclick=\"dmsMetadataShowHideCategory(\'metadata\');return false;\" title=\"View Metadata\" class=\"selected\">" + myContentAPI.EkMsgRef.GetMessage("metadata text") + "</a></li>";
                }
            }
        }

        return metadataOutput;
    }

    private void SetTaxonomy(long contentid, long ifolderid)
    {
        TaxonomyBaseData[] taxonomy_cat_arr = null;
        TaxonomyRequest taxonomy_request = new TaxonomyRequest();
        TaxonomyBaseData[] taxonomy_data_arr = null;

        taxonomy_request.TaxonomyId = ifolderid;
        taxonomy_request.TaxonomyLanguage = m_refContentApi.ContentLanguage;
        taxonomy_data_arr = m_refContentApi.EkContentRef.GetAllFolderTaxonomy(ifolderid);
        ltrTaxonomyOverrideId.Text = "0";
        if (!String.IsNullOrEmpty(Request.QueryString["TaxonomyOverrideId"]))
        {
            ltrTaxonomyOverrideId.Text = Request.QueryString["TaxonomyOverrideId"].ToString();
        }
        long taxonomyId = 0;
        if (!String.IsNullOrEmpty(Request.QueryString["TaxonomyId"]))
        {
            taxonomyId = Convert.ToInt64(Request.QueryString["TaxonomyId"]);
        }
        if (taxonomyId > 0)
        {

            ltrTaxonomyTreeIdList.Text = taxonomyId.ToString();
            if (ltrTaxonomyTreeIdList.Text.Trim().Length > 0)
            {
                taxonomy_cat_arr = m_refContentApi.EkContentRef.GetTaxonomyRecursiveToParent(taxonomyId, m_refContentApi.ContentLanguage, 0);
                if ((taxonomy_cat_arr != null) && taxonomy_cat_arr.Length > 0)
                {
                    foreach (TaxonomyBaseData taxonomy_cat in taxonomy_cat_arr)
                    {
                        if (ltrTaxonomyTreeParentIdList.Text == "")
                        {
                            ltrTaxonomyTreeParentIdList.Text = Convert.ToString(taxonomy_cat.TaxonomyId);
                        }
                        else
                        {
                            ltrTaxonomyTreeParentIdList.Text = ltrTaxonomyTreeParentIdList.Text + "," + Convert.ToString(taxonomy_cat.TaxonomyId);
                        }
                    }
                }
            }
        }

        ltrTaxFolderId.Text = ifolderid.ToString();
        m_intTaxFolderId = ifolderid.ToString();
    }
    private string CaptureTaxonomy(long contentId, long folderId)
    {
        StringBuilder taxonomyOutput = new StringBuilder();

        Folder myFolderApi = new Folder();
        FolderData myFolderData;
        ContentAPI myContentApi = new ContentAPI();
        TaxonomyBaseData[] myTaxonomyBaseData;
        List<long> myTaxonomyIds = new List<long>();
        System.Text.StringBuilder Js;
        Js = new System.Text.StringBuilder();
        long iTmpCaller = myContentApi.RequestInformationRef.CallerId;
        long iTmpuserID = myContentApi.RequestInformationRef.UserId;

        int ContentLanguage = EkConstants.CONTENT_LANGUAGES_UNDEFINED;

        if (!(Page.Request.QueryString["LangType"] == null))
        {
            if (Page.Request.QueryString["LangType"] != "")
            {
                ContentLanguage = Convert.ToInt32(Page.Request.QueryString["LangType"]);
                myContentApi.SetCookieValue("LastValidLanguageID", ContentLanguage.ToString());
            }
            else
            {
                if (myContentApi.GetCookieValue("LastValidLanguageID") != "")
                {
                    ContentLanguage = Convert.ToInt32(myContentApi.GetCookieValue("LastValidLanguageID"));
                }
            }
        }
        else
        {
            if (myContentApi.GetCookieValue("LastValidLanguageID") != "")
            {
                ContentLanguage = Convert.ToInt32(myContentApi.GetCookieValue("LastValidLanguageID"));
            }
        }

        if (ContentLanguage == EkConstants.CONTENT_LANGUAGES_UNDEFINED)
        {
            myContentApi.ContentLanguage = EkConstants.ALL_CONTENT_LANGUAGES;
        }
        else
        {
            myContentApi.ContentLanguage = ContentLanguage;
        }

        myContentApi.RequestInformationRef.CallerId = Ektron.Cms.Common.EkConstants.InternalAdmin;
        myContentApi.RequestInformationRef.UserId = Ektron.Cms.Common.EkConstants.InternalAdmin;

        myFolderData = myContentApi.GetFolderById(folderId, true);
        myTaxonomyBaseData = myFolderData.FolderTaxonomy;
        myContentApi.RequestInformationRef.CallerId = iTmpCaller;
        myContentApi.RequestInformationRef.UserId = iTmpuserID;

        if (!String.IsNullOrEmpty(Request.QueryString["TaxonomyId"]))
        {
            jsTaxRedirectID.Text=taxonomyselectedtree.Value = Request.QueryString["TaxonomyId"].ToString();
            
        }
        SetTaxonomy(contentId, folderId);

        Js.Append("function ValidateTax(){").Append(Environment.NewLine);
        if (myTaxonomyBaseData.Length > 0 && (myTaxonomyBaseData != null))
        {
            if (myFolderData.CategoryRequired == true)
            {
                Js.Append("      document.getElementById(\"taxonomyselectedtree\").value=\"\";").Append(Environment.NewLine);
                Js.Append("      for(var i=0;i<taxonomytreearr.length;i++){").Append(Environment.NewLine);
                Js.Append("         if(document.getElementById(\"taxonomyselectedtree\").value==\"\"){").Append(Environment.NewLine);
                Js.Append("             document.getElementById(\"taxonomyselectedtree\").value=taxonomytreearr[i];").Append(Environment.NewLine);
                Js.Append("         }else{").Append(Environment.NewLine);
                Js.Append("             document.getElementById(\"taxonomyselectedtree\").value=document.getElementById(\"taxonomyselectedtree\").value+\",\"+taxonomytreearr[i];").Append(Environment.NewLine);
                Js.Append("         }").Append(Environment.NewLine);
                Js.Append("       } ").Append(Environment.NewLine);
                Js.Append("      if (Trim(document.getElementById(\'taxonomyselectedtree\').value) == \'\') { ").Append(Environment.NewLine);
                Js.Append("         alert(\'" + m_refMsg.GetMessage("js tax cat req") + "\'); ").Append(Environment.NewLine);
                Js.Append("         return false; ").Append(Environment.NewLine);
                Js.Append("      } ").Append(Environment.NewLine);
                Js.Append("      return true; }").Append(Environment.NewLine);
            }
            else
            {
                Js.Append("      return true;}").Append(Environment.NewLine);
            }
            ltrTaxJS.Text = Js.ToString();

            ltrShowTaxonomy.Text = "<li><a id=\"taxonomyAnchor\" href=\"#\" onclick=\"dmsMetadataShowHideCategory(\'taxonomy\');return false;\" title=\"View Taxonomy\">" + m_refMsg.GetMessage("viewtaxonomytabtitle") + "</a></li>";
            string addTaxonomy = "<div id=" + "\"" + "TreeOutput" + "\"" + "></div>";
            return addTaxonomy;
        }
        else
        {
            Js.Append("      return true;}").Append(Environment.NewLine);
            ltrTaxJS.Text = Js.ToString();
            string addTaxonomyEmpty = "<div id=\"EmptyTree\"></div>";
            return addTaxonomyEmpty;
        }
    }
    private bool PublishContent()
    {
        ContentAPI contApi = new ContentAPI();
        EkContent contObj;
        int ContentLanguage = EkConstants.CONTENT_LANGUAGES_UNDEFINED;

        if (!(Page.Request.QueryString["LangType"] == null))
        {
            if (Page.Request.QueryString["LangType"] != "")
            {
                ContentLanguage = Convert.ToInt32(Page.Request.QueryString["LangType"]);
                contApi.SetCookieValue("LastValidLanguageID", ContentLanguage.ToString());
            }
            else
            {
                if (contApi.GetCookieValue("LastValidLanguageID") != "")
                {
                    ContentLanguage = Convert.ToInt32(contApi.GetCookieValue("LastValidLanguageID"));
                }
            }
        }
        else
        {
            if (contApi.GetCookieValue("LastValidLanguageID") != "")
            {
                ContentLanguage = Convert.ToInt32(contApi.GetCookieValue("LastValidLanguageID"));
            }
        }

        if (ContentLanguage == EkConstants.CONTENT_LANGUAGES_UNDEFINED)
        {
            contApi.ContentLanguage = EkConstants.ALL_CONTENT_LANGUAGES;
        }
        else
        {
            contApi.ContentLanguage = ContentLanguage;
        }

        object[] acMetaInfo = new object[4];
        string MetaSelect;
        string MetaSeparator;
        string MetaTextString = "";
        int ValidCounter = 0;
        int i = 0;
        bool hasMeta = false;
        if (Page.Request.Form["frm_validcounter"] != "")
        {
            ValidCounter = System.Convert.ToInt32(Page.Request.Form["frm_validcounter"]);
            hasMeta = true;
        }
        else
        {
            ValidCounter = 1;
        }

        string TaxonomyTreeIdList = "";
        TaxonomyTreeIdList = Page.Request.Form[taxonomyselectedtree.UniqueID];
        if ((!string.IsNullOrEmpty(TaxonomyTreeIdList)) && TaxonomyTreeIdList.Trim().EndsWith(","))
        {
            ltrTaxonomyTreeIdList.Text = TaxonomyTreeIdList.Substring(0, TaxonomyTreeIdList.Length - 1);
        }

        contObj = contApi.EkContentRef;
        string[] ids;
        string contId = "";
        ids = m_idString.Split(",".ToCharArray());
        htImagesAssets = new System.Collections.Hashtable();
        foreach (string tempLoopVar_contId in ids)
        {
            contId = tempLoopVar_contId;
            if (contId != "")
            {
                
                EditUrlAlias(long.Parse(contId),this.m_folderId);
                page_meta_data = new Collection();
                if (hasMeta == true)
                {
                    for (i = 1; i <= ValidCounter; i++)
                    {
                        acMetaInfo[1] = Page.Request.Form["frm_meta_type_id_" + i];
                        acMetaInfo[2] = contId;
                        MetaSeparator = Page.Request.Form["MetaSeparator_" + i];
                        MetaSelect = Page.Request.Form["MetaSelect_" + i];
                        if (Convert.ToInt32(MetaSelect) != 0)
                        {
                            MetaTextString = Strings.Replace(Page.Request.Form["frm_text_" + i], ", ", MetaSeparator.ToString(), 1, -1, 0);
                            if (MetaTextString.StartsWith(MetaSeparator))
                            {
                                MetaTextString = MetaTextString.Substring(MetaTextString.Length - (MetaTextString.Length - 1), (MetaTextString.Length - 1));
                            }
                            MetaTextString = CleanString(MetaTextString);
                            acMetaInfo[3] = MetaTextString;
                        }
                        else
                        {
                            MetaTextString = Strings.Replace(Page.Request.Form["frm_text_" + i], ";", MetaSeparator.ToString(), 1, -1, 0);
                            if (MetaTextString == null)
                                MetaTextString = "";
                            MetaTextString = CleanString(MetaTextString);
                            acMetaInfo[3] = MetaTextString;
                        }
                        page_meta_data.Add(acMetaInfo, i.ToString(), null, null);
                        acMetaInfo = new object[4];
                    }
                }
                if (!(string.IsNullOrEmpty(TaxonomyTreeIdList)))
                {

                    TaxonomyContentRequest request = new TaxonomyContentRequest();

                    request.ContentId = long.Parse(contId);
                    request.TaxonomyList = TaxonomyTreeIdList;
                    request.FolderID = m_folderId;
                    contObj.AddTaxonomyItem(request);
                }

                if (page_meta_data.Count > 0 && hasMeta == true)
                {
                    m_ContentApi.EkContentRef.UpdateMetaData(page_meta_data);
                }

            }
        }

        foreach (string tempLoopVar_contId in ids)
        {
            contId = tempLoopVar_contId;
            if (contId != "")
            {
                try
                {
                    string Status;
                    Status = (string)(contApi.GetContentStatusById(long.Parse(contId)));
                    long lcontId=long.Parse(contId);
                    if (htImagesAssets[lcontId] != null)// if item is exists in the hash table, the url alias is required.
                    {//process url alias required, publish image assets, checkin others
                        bool bIsPublish = (bool)htImagesAssets[lcontId];

                        if (Status == "O")
                        {
                            if (bIsPublish)
                                contApi.PublishContentById(lcontId, m_folderId, ContentLanguage, "true", -1, "");
                            else
                                contApi.EkContentRef.CheckIn(lcontId, "");
                        }
                        else if (Status == "I")
                        {
                            if(bIsPublish)
                                contApi.EkContentRef.SubmitForPublicationv2_0(lcontId, m_folderId, string.Empty);
                        }
                    }
                    else
                    {// normal process
                        if (Status == "O") // this will check in and publish
                        {
                            contApi.PublishContentById(lcontId, m_folderId, ContentLanguage, "true", -1, "");
                        }
                        else if (Status == "I") // this is just a publish
                        {
                            contApi.EkContentRef.SubmitForPublicationv2_0(lcontId, m_folderId, string.Empty);
                        }
                    }
                }
                catch (Exception)
                {
                    // I wrapped it in this try block because there is a current problem on the server where the content is already being put
                    // into published state if there are multiple pieces of content, the metadata still updates and is put in the right state.
                }
            }
        }

        return true;
    }
    public string CleanString(string inStr)
    {
        string returnValue;
        string outStr = string.Empty;
        int l;
        for (l = 1; l <= inStr.Length; l++)
        {
            if (Strings.Asc(inStr.Substring(l - 1, 1)) > 31)
            {
                outStr = outStr + inStr.Substring(l - 1, 1);
            }
        }
        returnValue = outStr;
        return returnValue;
    }

    void EditUrlAlias(long contId, long folderId)
    {
        bool required = false;
        //ContentData cont = m_ContentApi.EkContentRef.gec(contId);
        FolderData fdTmp = this.m_ContentApi.EkContentRef.GetFolderById(folderId);
        Ektron.Cms.UrlAliasing.UrlAliasSettingsApi m_urlAliasSettings = new Ektron.Cms.UrlAliasing.UrlAliasSettingsApi();

        if (m_urlAliasSettings.IsManualAliasEnabled)
        {
            if (fdTmp.AliasRequired)
            {
                required = true;
            }
        }
        if (!required)
            return;
       

        if (isFromMultiUpload)
        {
            ContentData cData = m_ContentApi.GetContentById(contId);
            if (Ektron.ASM.AssetConfig.ConfigManager.IsImageAsset(cData.AssetData.FileExtension))
                htImagesAssets.Add(contId, true);
            else
                htImagesAssets.Add(contId, false);
            return;
        }
        else
        {
            Collection page_content_data = m_ContentApi.EkContentRef.GetContentByIDv2_0(contId);
            string m_strManualAlias = (Request.Form["frm_manalias"] != null ? Request.Form["frm_manalias"].ToString().Trim() : null);
            string m_strManualAliasExt = (Request.Form["frm_manaliasExt"] != null ? Request.Form["frm_manaliasExt"].ToString() : null);
            //ast_frm_manaliasExt.Value = Request.Form["frm_manaliasExt"];
            string m_prevManualAliasName = (Request.Form["prev_frm_manalias_name"] != null ? Request.Form["prev_frm_manalias_name"].ToString() : "");
            string m_prevManualAliasExt = (Request.Form["prev_frm_manalias_ext"] != null ? Request.Form["prev_frm_manalias_ext"].ToString() : "");
            string m_currManualAliasName = m_strManualAlias;
            string m_currManualAliasExt = m_strManualAliasExt;
            string m_currManualAliasStatus = "";
            long m_intManualAliasId = 0;

            if (m_prevManualAliasName == "" && m_currManualAliasName != "" || m_prevManualAliasExt == "" && m_currManualAliasExt != "")
            {
                m_currManualAliasStatus = "New";
            }
            else if (m_prevManualAliasName != "" && m_currManualAliasName != "" && (m_currManualAliasName != m_prevManualAliasName || m_prevManualAliasExt != m_currManualAliasExt))
            {
                m_currManualAliasStatus = "Modified";
            }
            else if (m_prevManualAliasName != "" && m_currManualAliasName == "")
            {
                m_currManualAliasStatus = "Deleted";
            }
            else
            {
                m_currManualAliasStatus = "None";
            }
            if (!string.IsNullOrEmpty(Request.Form["frm_manalias_id"]))
            {
                m_intManualAliasId = System.Convert.ToInt64(Request.Form["frm_manalias_id"]);
            }
            if (EkFunctions.DoesKeyExist(page_content_data, "FileChanged"))
                page_content_data.Remove("FileChanged");
            page_content_data.Add("False", "FileChanged", null, null);

            if (EkFunctions.DoesKeyExist(page_content_data, "NewUrlAliasName"))
                page_content_data.Remove("NewUrlAliasName");
            page_content_data.Add(m_strManualAlias, "NewUrlAliasName", null, null);

            if (EkFunctions.DoesKeyExist(page_content_data, "UrlAliasId"))
                page_content_data.Remove("UrlAliasId");
            page_content_data.Add(m_intManualAliasId, "UrlAliasId", null, null);


            if (EkFunctions.DoesKeyExist(page_content_data, "NewUrlAliasExt"))
                page_content_data.Remove("NewUrlAliasExt");
            page_content_data.Add(m_strManualAliasExt, "NewUrlAliasExt", null, null);

            if (EkFunctions.DoesKeyExist(page_content_data, "UrlAliasStatus"))
                page_content_data.Remove("UrlAliasStatus");
            page_content_data.Add(m_currManualAliasStatus, "UrlAliasStatus", null, null);

            if (EkFunctions.DoesKeyExist(page_content_data, "OldUrlAliasName"))
                page_content_data.Remove("OldUrlAliasName");
            page_content_data.Add(m_prevManualAliasName, "OldUrlAliasName", null, null);

            if (EkFunctions.DoesKeyExist(page_content_data, "OldUrlAliasExt"))
                page_content_data.Remove("OldUrlAliasExt");
            page_content_data.Add(m_prevManualAliasExt, "OldUrlAliasExt", null, null);

            if (EkFunctions.DoesKeyExist(page_content_data, "ManualAlias"))
                page_content_data.Remove("ManualAlias");
            page_content_data.Add(m_strManualAlias, "ManualAlias", null, null);

            if (EkFunctions.DoesKeyExist(page_content_data, "ManualAliasID"))
                page_content_data.Remove("ManualAliasID");
            page_content_data.Add(m_intManualAliasId, "ManualAliasID", null, null);

            if (page_content_data["ContentStatus"].ToString().ToUpper() != "O")
                m_ContentApi.EkContentRef.CheckContentOutv2_0(contId);
            m_ContentApi.EkContentRef.SaveContentv2_0(page_content_data);
        }
    }

    private void ToolBar()
    {
        string closeWin = "";
        System.Text.StringBuilder result;
        result = new System.Text.StringBuilder();
        List<string> lstTitleBar = new List<string>();
        
        if (ltrShowTaxonomy.Text != "")
        {
            //divTitleBar.InnerHtml = m_refStyle.GetTitleBar((string)(m_refMsg.GetMessage("dms taxonomy title")));
            lstTitleBar.Add("Taxonomy");
        }
        if (ltrShowMetadata.Text != "")
        {
            //divTitleBar.InnerHtml = m_refStyle.GetTitleBar((string)(m_refMsg.GetMessage("dms metadata title")));
            lstTitleBar.Add("Metadata");
        }
        if (ltrShowUrlAlias.Text != "")
        {
            lstTitleBar.Add("Url Alias");
        }

        if (lstTitleBar.Count > 0)
        {
            string titlebarFormat = "Add {0} for files";
            divTitleBar.InnerHtml = m_refStyle.GetTitleBar(string.Format(titlebarFormat, string.Join(",", lstTitleBar.ToArray())));
        }
        else
        {
            divTitleBar.InnerHtml = m_refStyle.GetTitleBar("");
        }

        if (Request.QueryString["close"] == "true")
        {
            closeWin = "close";
        }
        result.Append("<table ><tr>");
        result.Append(m_refStyle.GetButtonEventsWCaption((string)(m_refContentApi.AppPath + "images/UI/Icons/cancel.png"), "#", (string)(m_refMsg.GetMessage("generic Cancel")), (string)(m_refMsg.GetMessage("generic Cancel")), "onclick=\"return CancelForm(\'form1\', \'\', \'" + closeWin + "\');\"", StyleHelper.CancelButtonCssClass,true));
        result.Append(m_refStyle.GetButtonEventsWCaption((string)(m_refContentApi.AppPath + "images/ui/icons/contentpublish.png"), "#", (string)(m_refMsg.GetMessage("generic Publish")), (string)(m_refMsg.GetMessage("generic Publish")), "onclick=\"return SubmitForm(\'form1\', \'\', \'" + closeWin + "\');\"", StyleHelper.PublishButtonCssClass,true));
        result.Append(StyleHelper.ActionBarDivider);
        result.Append("<td>");
        result.Append(m_refStyle.GetHelpButton("addmetadataforfiles", ""));
        result.Append("</td>");
        result.Append("</tr></table>");
        divToolBar.InnerHtml = result.ToString();
    }

    private void RegisterResources()
    {
        JS.RegisterJS(this, m_refContentApi.AppPath + "java/workareahelper.js", "EktronWorkareaHelperJS");
        Packages.Ektron.Workarea.Core.Register(this);
    }
}
