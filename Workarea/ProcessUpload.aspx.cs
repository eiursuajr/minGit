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
using Ektron.Cms.Content;
using Ektron.ASM.AssetConfig;
using System.IO;
using Microsoft.VisualBasic.CompilerServices;

public partial class ProcessUpload : System.Web.UI.Page
{

    static int bufSize = 1048576;
    private long userId;
    private int loginToken;
    protected EkMessageHelper m_refMsg;
    protected long TaxonomySelectId = 0;
    private string m_prevManualAliasName = "";
    private string m_prevManualAliasExt = "";
    private string m_currManualAliasName = "";
    private string m_currManualAliasExt = "";
    private bool isCallerDmsDragDropCtl = false;
    protected ContentAPI m_refContApi = new ContentAPI();
    private string actionType = "";
    private int ContentLanguage;
    protected CommonApi AppUI = new CommonApi();
    protected EkContent m_refContent;
    protected int ManagedAsset_Min = Ektron.Cms.Common.EkConstants.ManagedAsset_Min;
    protected int ManagedAsset_Max = Ektron.Cms.Common.EkConstants.ManagedAsset_Max;
    Hashtable asset_info = new Hashtable();
    public static string[] g_AssetTypeSubfieldKeys = new string[] { "ImageUrl" };
    public static string[] m_AssetInfoKeys = new string[] { "AssetID", "AssetVersion", "AssetFilename", "MimeType", "FileExtension", "MimeName", "ImageUrl", "MediaAsset" };
    private string DMSCookieName = "DMS_Office_ver";
    protected bool searc = true;

    private void Page_Load(System.Object sender, System.EventArgs e)
    {
        try
        {
            bool fileChanged = true;
            string assetId;
            List<AssetFileData> assetIdFileNameList = null;
            List<AssetFileData> tempAssetIdFileNameList = new List<AssetFileData>();
            m_refContent = AppUI.EkContentRef;
            if (Request.Form["type"] != null)
            {
                actionType = (string)(Request.Form["type"].Trim().ToLower());
            }

            if (Request.Form["requireMetaTaxonomy"] != null)
            {
                isCallerDmsDragDropCtl = true;
            }

            m_refContApi = new ContentAPI();
            m_refMsg = m_refContApi.EkMsgRef;
            if (!String.IsNullOrEmpty(Request.Form["content_language"]))
            {

                ContentLanguage = Convert.ToInt32(Request.Form["content_language"]);
                if (ContentLanguage == Ektron.Cms.Common.EkConstants.CONTENT_LANGUAGES_UNDEFINED || ContentLanguage == Ektron.Cms.Common.EkConstants.ALL_CONTENT_LANGUAGES)
                {
                    ContentLanguage = AppUI.DefaultContentLanguage;
                }
                AppUI.ContentLanguage = ContentLanguage;
                m_refContApi.ContentLanguage = ContentLanguage;

            }
            if (actionType.ToLower() == "add" && (Request.QueryString["SelTaxonomyId"] != null) && Request.QueryString["SelTaxonomyId"] != "")
            {
                TaxonomySelectId = Convert.ToInt64(Request.QueryString["SelTaxonomyId"]);
            }
            userId = Convert.ToInt64(Ektron.Cms.CommonApi.GetEcmCookie()["user_id"]);
            loginToken = Convert.ToInt32(Ektron.Cms.CommonApi.GetEcmCookie()["unique_id"]);

            if ((Request.Form["taxonomyselectedtree"] != null) && Request.Form["taxonomyselectedtree"].ToString().Length > 1)
            {
                jsTaxonomyIdReloadFrame.Text = Request.Form["taxonomyselectedtree"].ToString();
                if (Request.Form["taxonomyselectedtree"].ToString().Split(",".ToCharArray()) != null)
                {
                    jsTaxonomyId.Text = (Request.Form["taxonomyselectedtree"].ToString().Split(",".ToCharArray()))[0];
                }
            }

            if (Request.Form["editaction"] == "cancel")
            {
                Session.Remove(Convert.ToString(userId) + Convert.ToString(loginToken) + "Attachments");
                if ((actionType != "add") && (Request.Form["content_id"] != "" && Convert.ToInt64(Request.Form["content_id"]) > 0))
                {
                    string status = m_refContApi.GetContentStatusById(Convert.ToInt64(Request.Form["content_id"]));
                    if (status.ToLower() == "o") //28710 - if asset is a office file status = "A" on saving in word
                    {
                        m_refContent.UndoCheckOutv2_0(Convert.ToInt64(Request.Form["content_id"]));
                    }
                    if (Request.Form["FromEE"] == "true" || Request.QueryString["close"] == "true")
                    {
                        if (Request.QueryString["close"] == "true")
                        {
                            Response.Redirect("close.aspx?reload=true", false);
                            return;
                        }
                        else
                        {
                            Response.Redirect("close.aspx", false);
                            return;
                        }
                    }
                    else
                    {
                        Response.Redirect((string)("content.aspx?id=" + Request.Form["content_id"] + "&action=viewstaged&LangType=" + m_refContApi.ContentLanguage), false);
                        return;
                    }
                }
                else
                {
                    if (Request.Form["FromEE"] == "true" || Request.QueryString["close"] == "true")
                    {
                        if (Request.QueryString["close"] == "true")
                        {
                            Response.Redirect("close.aspx?reload=true", false);
                            return;
                        }
                        else
                        {
                            Response.Redirect("close.aspx", false);
                            return;
                        }
                    }
                    else
                    {
                        Response.Redirect((string)("content.aspx?id=" + Request.Form["content_folder"] + "&action=ViewContentByCategory&LangType=" + m_refContApi.ContentLanguage), false);
                        return;
                    }
                }
            }

            int i;
            for (i = 0; i <= m_AssetInfoKeys.Length - 1; i++)
            {
                asset_info.Add(m_AssetInfoKeys[i], "");
            }
            if (actionType != "multiple,add")
            {
                if ((Request.Form["asset_assetid"] != null) && (Request.Form["asset_assetid"].Length > 1))
                {
                    assetId = Request.Form["asset_assetid"];
                }
                else
                {
                    assetId = System.Guid.NewGuid().ToString();
                }
                Response.Expires = -1;
                Response.AddHeader("Pragma", "no-cache");
                Response.AddHeader("cache-control", "no-store");
                string ErrDescription = "";
                string filename;
                if (Request.Files["fileupload"] != null)
                {
                    fileChanged = true;
                    filename = Request.Files["fileupload"].FileName;
					
					string[] DMSFileTypes = DocumentManagerData.Instance.FileTypes.Replace("*","").Split(',');
                    bool IsFileTypeAllowed = false;

                    if (DMSFileTypes != null && !string.IsNullOrEmpty(filename))
                    {
                        if (Array.FindIndex(DMSFileTypes, x => x.ToString().Trim().ToLower() == filename.Substring(filename.IndexOf(".")).ToLower()) >= 0)
                        {
                            IsFileTypeAllowed = true;
                        }
                    }
					
					
                    if (actionType == "add")
                    {
                        Ektron.ASM.AssetConfig.AssetData astData = new Ektron.ASM.AssetConfig.AssetData();
                        astData = m_refContent.GetAssetDataBasedOnFileName(Path.GetFileName(filename), Convert.ToInt64(Request.Form["content_folder"]), -1);
                        if (astData != null && astData.ID != "" && astData.Name != "")
                        {
                            ErrDescription = "A content with title: " + astData.Name + " and ID: " + astData.ID + " already exists with the file name: " + Path.GetFileName(filename) + ". Please use a different filename.";
                            Response.Redirect((string)("reterror.aspx?info=" + ErrDescription), false);
                            return;
                        }
                    }
                    if (filename.Trim() == "")
                    {
                        fileChanged = false;
                        filename = (string)(Request.Form["oldfilename"].ToString());
                    }
                    else
                    {
						if (!IsFileTypeAllowed)
                        {
                            EkException.LogException(String.Format("Attempt at uploading unallowed file type occurred from user id: {0}", this.userId));
                            ErrDescription = "The file has not been uploaded. Please see administrator.";
                            Response.Redirect("reterror.aspx?info=" + ErrDescription, false);
                            return;
                        }
                        try
                        {
                            string docFilePath = DocumentManagerData.Instance.WebSharePath;
                            if (!System.IO.Path.IsPathRooted(docFilePath))
                            {
                                docFilePath = Ektron.ASM.AssetConfig.Utilities.UrlHelper.GetAppPhysicalPath() + docFilePath;
                            }
                            string destFileName = docFilePath + Path.GetFileName(filename) + assetId;
                            using (BinaryReader br = new BinaryReader(Request.Files["fileupload"].InputStream))
                            {
                                byte[] buf;
                                using (BinaryWriter binaryWriter = new BinaryWriter(File.Open(destFileName, FileMode.Create)))
                                {
                                    buf = br.ReadBytes(bufSize);

                                    if (EkFunctions.IsImage(Path.GetExtension(filename)))
                                    {
                                        System.IO.Stream streamTmp=new System.IO.MemoryStream(buf);
                                        if (!EkFunctions.isImageStreamValid(streamTmp))
                                        {
                                            streamTmp.Flush();
                                            streamTmp.Close();
                                            throw new Exception("The image is corrupted or not in correct format.");
                                        }
                                        streamTmp.Flush();
                                        streamTmp.Close();
                                    }

                                    int index = 0;
                                    while (buf.Length > 0)
                                    {
                                        binaryWriter.Write(buf, 0, buf.Length);
                                        index += buf.Length;
                                        buf = br.ReadBytes(bufSize);
                                    }
                                    binaryWriter.Flush();
                                    binaryWriter.Close();
                                }

                                br.Close();
                            }
                        }
                        catch (Exception ex)
                        {

                            throw ex;
                        }
                    }
                }
                else
                {
                    filename = (string)(Request.Form["filename"].ToString());
                    fileChanged = false;
                }
                ProcessPage(filename, assetId, fileChanged, null);
            }
            else
            {
                bool isOfc2010 = false;
                if (Request.Cookies[DMSCookieName] != null && !string.IsNullOrEmpty(Request.Cookies[DMSCookieName].Value))
                {
                    if (Request.Cookies[DMSCookieName].Value == "2010")
                    {
                        isOfc2010 = true;
                    }
                }
                if (isOfc2010)
                {
                    ProcessUploadOffice2010();
                }
                else
                {
                    #region originalupload
                    if (Session[Convert.ToString(userId) + Convert.ToString(loginToken) + "Attachments"] != null)
                    {
                        assetIdFileNameList = (List<AssetFileData>)Session[Convert.ToString(userId) + Convert.ToString(loginToken) + "Attachments"];
                        Session.Remove(Convert.ToString(userId) + Convert.ToString(loginToken) + "Attachments");
                    }
                    //If coming from photo/image gallery, do not upload any files if a single invalid file is found
                    if ((Request.Form["isImage"] != null) && Request.Form["isImage"].ToString() == "1")
                    {
                        List<string> allFilesList = null;
                        if (Session[Convert.ToString(userId) + Convert.ToString(loginToken) + "AllPostedFiles"] != null)
                        {
                            allFilesList = (List<string>)Session[userId + loginToken + "AllPostedFiles"];
                            Session.Remove(Convert.ToString(userId) + Convert.ToString(loginToken) + "AllPostedFiles");
                        }
                        string invalidFilesMsg = "";
                        if (allFilesList != null)
                        {
                            for (int index = 0; index <= allFilesList.Count - 1; index++)
                            {
                                if (!EkFunctions.IsImage(Path.GetExtension(System.Convert.ToString(allFilesList[index]))))
                                {
                                    invalidFilesMsg = System.Convert.ToString(allFilesList[index]);
                                    this.jsInvalidFiles.Text = "Only images can be uploaded here, invalid file type found: " + invalidFilesMsg.Replace("\\", "\\\\").Replace("\'", "\\\'") + "\\nNo images have been uploaded.";
                                    this.uxAlertInvalidFileType.Visible = true;
                                    this.uxCloseThickBox.Visible = true;
                                    return;
                                }
                            }
                        }
                    }
                    if (assetIdFileNameList != null && assetIdFileNameList.Count > 0)
                    {
                        for (int index = 0; index <= assetIdFileNameList.Count - 1; index++)
                        {
                            if (assetIdFileNameList[index].FileName.IndexOfAny(new char[] { '&', '%', '+' }) > -1)
                            {
                                this.jsInvalidFiles.Text = "Some files with &,+ or % could not be uploaded";
                                this.uxAlertInvalidFileType.Visible = true;
                            }
                            else
                            {
                                tempAssetIdFileNameList.Add(assetIdFileNameList[index]);
                            }
                        }
                        assetIdFileNameList = tempAssetIdFileNameList;
                    }
                    fileChanged = true;
                    if ((assetIdFileNameList != null) && assetIdFileNameList.Count > 0)
                    {
                        ProcessPage("", "", true, assetIdFileNameList);
                    }
                    else
                    {
                        //if any files were rejected
                        if (Request.Form["FromEE"] == "true" || Request.QueryString["close"] == "true")
                        {
                            if (Request.QueryString["close"] == "true")
                            {
                                Response.Redirect("close.aspx?reload=true", false);
                            }
                            else
                            {
                                Response.Redirect("close.aspx", false);
                            }
                        }
                        else
                        {
                            if (isCallerDmsDragDropCtl == true)
                            {
                                if ((Session[Convert.ToString(userId) + Convert.ToString(loginToken) + "RejectedFiles"] != null) && Session[Convert.ToString(userId) + Convert.ToString(loginToken) + "RejectedFiles"].ToString() != "")
                                {
                                    this.jsInvalidFiles.Text = m_refMsg.GetMessage("lbl error message for multiupload") + " " + Session[Convert.ToString(userId) + Convert.ToString(loginToken) + "RejectedFiles"].ToString().Replace("\\", "\\\\").Replace("\'", "\\\'");
                                    this.jsInvalidFiles.Text += "\\n" + m_refMsg.GetMessage("js:cannot add file with add and plus");
                                    this.uxAlertInvalidFileType.Visible = true;
                                    Session.Remove(Convert.ToString(userId) + Convert.ToString(loginToken) + "RejectedFiles");
                                    this.uxCloseThickBox.Visible = true;
                                }
                                else
                                {
                                    Response.Redirect(Request.UrlReferrer.PathAndQuery + ((Request.UrlReferrer.PathAndQuery.IndexOf("showtab") > -1) ? "" : "&showtab=multiple"), false);
                                }

                            }
                            else
                            {
                                if ((Session[Convert.ToString(userId) + Convert.ToString(loginToken) + "RejectedFiles"] == null) || Session[Convert.ToString(userId) + Convert.ToString(loginToken) + "RejectedFiles"].ToString() == "")
                                {
                                    if ((Session[Convert.ToString(userId) + Convert.ToString(loginToken) + "Attachments"] == null) || Session[Convert.ToString(userId) + Convert.ToString(loginToken) + "Attachments"].ToString() == "")
                                    {
                                        if (Session[Convert.ToString(userId) + Convert.ToString(loginToken) + "NoFilesToUpload"] == null)
                                        {
                                            Session.Add(Convert.ToString(userId) + Convert.ToString(loginToken) + "NoFilesToUpload", "NoFilesToUpload");
                                        }
                                        else
                                        {
                                            Session[Convert.ToString(userId) + Convert.ToString(loginToken) + "NoFilesToUpload"] = "NoFilesToUpload";
                                        }
                                    }
                                }
                                Response.Redirect((string)("content.aspx?id=" + Request.Form["content_folder"] + "&action=ViewContentByCategory&LangType=" + ContentLanguage), false);
                            }
                        }
                    }
                    #endregion
                }
            }

        }
        catch (Exception ex)
        {
            Utilities.ShowError(ex.Message.ToString());
        }
    }

    private void ProcessPage(string filename, string assetId, bool fileChanged, List<AssetFileData> assetIdFileNameList)
    {
        System.Text.StringBuilder result = new System.Text.StringBuilder();
        string[] Files;
        string[] AssetIds;
        string[] MimeTypes;
        string[] Extensions;
        int i = 0;
        int lMultiType = 0;
        Ektron.Cms.Common.AssetData[] asset_list;
        Ektron.Cms.Common.ContentResponseData[] AddMultiResult;
        Collection cMetadataTypes = new Collection();
        Collection cCont = new Collection();
        Collection cContRet = new Collection();
        string editaction = Request.Form["editaction"];
        int iContLoop = 1;
        string strSearchText = "";
        string go_live = "";
        string end_date = "";
        string end_date_action = "";
        string[] acMetaInfo = new string[4];
        long MetaCount = 0;
        Collection page_subscription_data = new Collection();
        Collection page_sub_temp = new Collection();
        Array arrSubscriptions;
        SubscriptionPropertiesData sub_prop_data = new SubscriptionPropertiesData();
        int isub = 0;
        string contentCount = "";
        long intContentId;
        Ektron.Cms.API.Metadata metaDataAPI = new Ektron.Cms.API.Metadata();
        CustomAttributeList MetaFields = new CustomAttributeList();

        if (assetIdFileNameList != null)
        {
            Files = new string[assetIdFileNameList.Count - 1 + 1];
            AssetIds = new string[assetIdFileNameList.Count - 1 + 1];
            MimeTypes = new string[assetIdFileNameList.Count - 1 + 1];
            Extensions = new string[assetIdFileNameList.Count - 1 + 1];
            for (int index = 0; index <= assetIdFileNameList.Count - 1; index++)
            {
                Files[index] = Path.GetFileName((string)(assetIdFileNameList[index].FileName));
                AssetIds[index] = (string)(assetIdFileNameList[index].AssetId);
                MimeTypes[index] = ConfigManager.GetMimeTypeForExt(ConfigManager.GetExtensionFromName(Files[index]));
                Extensions[index] = Path.GetExtension(Files[index]);
            }
        }
        else
        {
            Files = new string[] { Path.GetFileName(filename) };
            AssetIds = new string[] { assetId };
            MimeTypes = new string[] { ConfigManager.GetMimeTypeForExt(ConfigManager.GetExtensionFromName(filename)) };
            Extensions = new string[] { Path.GetExtension(filename) };
        }
        if (AppUI.UserId == 0)
        {
            throw (new Exception("Invalid User"));
        }

        lMultiType = GetAddMultiType();
        string TaxonomyTreeIdList = "";
        if ((Request.QueryString["taxoverride"] != null) && Convert.ToInt64(Request.QueryString["taxoverride"]) != 0)
        {
            TaxonomyTreeIdList = Request.QueryString["taxoverride"];
        }
        if (Request.Form["taxonomyselectedtree"] != null)
        {
            TaxonomyTreeIdList = Request.Form["taxonomyselectedtree"];
            if (TaxonomyTreeIdList.Trim().EndsWith(","))
            {
                TaxonomyTreeIdList = TaxonomyTreeIdList.Substring(0, TaxonomyTreeIdList.Length - 1);
            }
        }
        cCont.Add(TaxonomyTreeIdList, "Taxonomy", null, null);
        asset_list = new Ektron.Cms.Common.AssetData[Files.Length - 1 + 1];

        if ((actionType != "add") && (Request.Form["content_id"] != "-1" && Request.Form["content_id"] != "0"))
        {
            cCont.Add(AssetIds[0], m_AssetInfoKeys[0], null, null);
            cCont.Add("", m_AssetInfoKeys[1], null, null);
            cCont.Add(Files[0], m_AssetInfoKeys[2], null, null);
            cCont.Add(MimeTypes[0], m_AssetInfoKeys[3], null, null);
            cCont.Add(Extensions[0], m_AssetInfoKeys[4], null, null);
        }
        else
        {
            if (Path.GetFileName(filename) != "" || Files.Length > 0)
            {
                for (i = 0; i <= Files.Length - 1; i++)
                {
                    asset_list[i] = new Ektron.Cms.Common.AssetData();
                    asset_list[i].Id = AssetIds[i];
                    asset_list[i].FileName = Files[i];
                    if (ConfigManager.GetMimeTypeForExt(Path.GetExtension(filename)) == "")
                    {
                        asset_list[i].MimeType = "";
                    }
                    else
                    {
                        asset_list[i].MimeType = MimeTypes[i];
                    }
                    asset_list[i].FileExtension = Extensions[i];
                    asset_list[i].Type = lMultiType;
                    asset_list[i].Language = ContentLanguage;
                }
            }
            cCont.Add(AssetIds, "AssetIds", null, null);
        }
        if (editaction == null)
        {
            editaction = "publish";
        }
        cCont.Add(editaction, "Mode", null, null);
        if ((actionType == "add") || (Request.Form["content_id"] == "-1" || Request.Form["content_id"] == "0"))
        {
            cCont.Add(true, "BatchProcess", null, null);
            cCont.Add(asset_list, "AssetInfo", null, null);
        }
        cCont.Add(ContentLanguage, "ContentLanguage", null, null);
        cCont.Add(Request.Form["content_folder"], "FolderID", null, null);
        cCont.Add(Request.Form["mycollection"], "MyCollection", null, null);

        if (Request.Form["content_html"] == null)
        {
            cCont.Add("", "ContentHtml", null, null);
        }
        else
        {
            cCont.Add(Request.Form["content_html"], "ContentHtml", null, null);
        }

        if (Request.Form["content_id"] != "-1" && Request.Form["content_id"] != "0")
        {
            if (Convert.ToInt32(Request.Form["content_type"]) == 104)
            {
                cCont.Add("", "MediaText", null, null);
            }
            if (Request.Form["content_type"] != null)
            {
                cCont.Add(Request.Form["content_type"], "ContentType", null, null);
            }
            else
            {
                cCont.Add(lMultiType, "ContentType", null, null);
            }
        }
        else
        {
            cCont.Add(Request.Form["content_type"], "ContentType", null, null);
        }

        if (Convert.ToInt32(Request.Form["content_type"]) == 104)
        {
            if (Request.Form["content_html"] != null)
            {
                if (Request.Form["content_html"] != "")
                {
                    cCont.Remove("MediaText");
                    cCont.Add(Request.Form["content_html"].Replace("&", "&amp;"), "MediaText", null, null);
                    cCont.Remove("ContentHtml");
                    cCont.Add(Request.Form["content_html"], "ContentHtml", null, null);
                }
                else
                {
                    if (EkFunctions.DoesKeyExist(cCont, "MediaText"))
                    {
                        cCont.Remove("MediaText");
                    }
                    cCont.Add("", "MediaText", null, null);
                }
            }
            else
            {
                cCont.Remove("MediaText");
                cCont.Add("", "MediaText", null, null);
            }
        }

        string strContentTitle = "";
        if (!String.IsNullOrEmpty(Request.Form["content_title"]))
        {
            strContentTitle = Request.Form["content_title"];
        }
        else if (contentCount == null)
        {
            strContentTitle = Path.GetFileName(Files[0]);
        }
        string strImage = "";
        if (!String.IsNullOrEmpty(Request.Form["content_image"]))
        {
            strImage = Request.Form["content_image"];
        }
        else
        {
            strImage = "";
        }
        cCont.Add(strImage, "Image", null, null);
        cCont.Add(Request.Form["LockedContentLink"], "LockedContentLink", null, null);
        cCont.Add(strContentTitle, "ContentTitle", null, null);
        cCont.Add(Request.Form["content_comment"], "Comment", null, null);
        if (!String.IsNullOrEmpty(Request.Form["content_id"]) && Request.Form["content_id"] != "-1" && Request.Form["content_id"] != "0")
        {
            cCont.Add(Request.Form["content_id"], "ContentID", null, null);
        }
        else
        {
            cCont.Add(0, "ContentID", null, null);
        }

        if (!String.IsNullOrEmpty(Request.Form["frm_validcounter"]))
        {
            MetaCount = System.Convert.ToInt32(Request.Form["frm_validcounter"]);
            for (i = 1; i <= MetaCount; i++)
            {
                acMetaInfo[1] = Request.Form["frm_meta_type_id_" + i];
                if (Request.Form["content_id"] != "-1" && Request.Form["content_id"] != "0")
                {
                    acMetaInfo[2] = Request.Form["content_id"];
                }
                else
                {
                    acMetaInfo[2] = ""; // adding content, so no content ID
                }
                acMetaInfo[3] = Request.Form["frm_text_" + i];
                cMetadataTypes.Add(acMetaInfo, i.ToString(), null, null);
                acMetaInfo = new string[4];
            }
        }
        //Image Gallery metadata
        acMetaInfo = UpdateMetadataContent();
        if (acMetaInfo != null)
        {
            cMetadataTypes.Add(acMetaInfo, System.Convert.ToString(cMetadataTypes.Count + 1), null, null);
        }

        cCont.Add(Request.Form["createtask"], "CreateTask", null, null);
        cCont.Add(Request.QueryString["addto"], "AddToCollectionType", null, null);

        if (!String.IsNullOrEmpty(Request.QueryString["mycollection"]))
        {
            cCont.Add(Request.QueryString["mycollection"], "MyCollection", null, null);
        }
        else if (!String.IsNullOrEmpty(Request.Form["mycollection"]))
        {
            cCont.Add(Request.Form["mycollection"], "MyCollection", null, null);
        }
        cCont.Add(cMetadataTypes, "ContentMetadata", null, null);
        if (!String.IsNullOrEmpty(Request.Form["ekImagegalleryDescription"]))
        {
            cCont.Add(Request.Form["ekImagegalleryDescription"], "ContentTeaser", null, null);
        }
        else
        {
            if (Request.Form["content_teaser"] != null)
            {
                cCont.Add(Request.Form["content_teaser"], "ContentTeaser", null, null);
            }
            else
            {
                cCont.Add("", "ContentTeaser", null, null);
            }
        }

        while (Strings.Len(Request.Form["searchtext" + iContLoop]) > 0)
        {
            strSearchText = strSearchText + Request.Form["searchtext" + iContLoop];
            iContLoop = System.Convert.ToInt32(iContLoop + 1);
        }
        cCont.Add(strSearchText, "SearchText", null, null);

        if (Request.Form["go_live"] != "")
        {
            try
            {
                go_live = DateTime.Parse(Strings.Trim(Request.Form["go_live"])).ToString();
            }
            catch (Exception)
            {
                // ignore exceptions
            }
        }
        if (Request.Form["end_date"] != "")
        {
            try
            {
                end_date = DateTime.Parse(Strings.Trim(Request.Form["end_date"])).ToString();
            }
            catch (Exception)
            {
                //ignore exceptions
            }
            end_date_action = Request.Form["end_date_action_radio"];
        }
        cCont.Add(go_live, "GoLive", null, null);
        cCont.Add(end_date, "EndDate", null, null);
        cCont.Add(end_date_action, "EndDateAction", null, null);
        // dropupload should always add the quicklink

        if (!((Strings.Len(Request.Form["suppressnotification"])) > 0))
        {
            sub_prop_data.BreakInheritance = true;
            if (!String.IsNullOrEmpty(Request.Form["send_notification_button"]))
            {
                sub_prop_data.SendNextNotification = true;
                sub_prop_data.SuspendNextNotification = false;
            }
            else
            {
                sub_prop_data.SendNextNotification = false;
            }
            if (Request.Form["notify_option"] == ("Always"))
            {
                sub_prop_data.NotificationType = Ektron.Cms.Common.EkEnumeration.SubscriptionPropertyNotificationTypes.Always;
            }
            else if (Request.Form["notify_option"] == ("Initial"))
            {
                sub_prop_data.NotificationType = Ektron.Cms.Common.EkEnumeration.SubscriptionPropertyNotificationTypes.Initial;
                if (!(actionType == "update")) // if new, then set flag to email out
                {
                    sub_prop_data.SendNextNotification = true;
                    sub_prop_data.SuspendNextNotification = false;
                }
                else
                {
                    if (!String.IsNullOrEmpty((Request.Form["send_notification_button"])))
                    {
                        sub_prop_data.SendNextNotification = true;
                        sub_prop_data.SuspendNextNotification = false;
                    }
                    else
                    {
                        sub_prop_data.SendNextNotification = false;
                    }
                }
            }
            else if (Request.Form["notify_option"] == ("Never"))
            {
                sub_prop_data.NotificationType = Ektron.Cms.Common.EkEnumeration.SubscriptionPropertyNotificationTypes.Never;
            }
            if (!String.IsNullOrEmpty(Request.Form["suspend_notification_button"]))
            {
                sub_prop_data.SuspendNextNotification = true;
                sub_prop_data.SendNextNotification = false;
            }
            else
            {
                sub_prop_data.SuspendNextNotification = false;
            }
            sub_prop_data.OptOutID = Convert.ToInt64(Request.Form["notify_optoutid"]);
            if (!String.IsNullOrEmpty(Request.Form["use_message_button"]))
            {
                sub_prop_data.DefaultMessageID = Convert.ToInt64(Request.Form["notify_messageid"]);
            }
            else
            {
                sub_prop_data.DefaultMessageID = 0;
            }
            if (!String.IsNullOrEmpty(Request.Form["use_summary_button"]))
            {
                sub_prop_data.SummaryID = 1;
            }
            else
            {
                sub_prop_data.SummaryID = 0;
            }
            if (!String.IsNullOrEmpty(Request.Form["use_content_button"]))
            {
                sub_prop_data.ContentID = Convert.ToInt64(Request.Form["frm_content_id"]);
            }
            else
            {
                sub_prop_data.ContentID = 0;
            }
            sub_prop_data.UnsubscribeID = Convert.ToInt64(Request.Form["notify_unsubscribeid"]);

            if (!String.IsNullOrEmpty(Request.Form["notify_url"]))
            {
                sub_prop_data.URL = Request.Form["notify_url"];
            }
            else
            {
                sub_prop_data.URL = Request.ServerVariables["HTTP_HOST"];
            }

            if (!String.IsNullOrEmpty(Request.Form["notify_weblocation"]))
            {
                sub_prop_data.FileLocation = Server.MapPath(m_refContApi.AppPath + "subscriptions");
            }
            else
            {
                sub_prop_data.FileLocation = Server.MapPath(m_refContApi.AppPath + "subscriptions");
            }
            if (!String.IsNullOrEmpty(Request.Form["notify_weblocation"]))
            {
                sub_prop_data.WebLocation = Request.Form["notify_weblocation"];
            }
            else
            {
                sub_prop_data.WebLocation = "subscriptions";
            }
            if (!String.IsNullOrEmpty(Request.Form["notify_subject"]))
            {
                sub_prop_data.Subject = Request.Form["notify_subject"];
            }
            else
            {
                sub_prop_data.Subject = "";
            }
            if (!String.IsNullOrEmpty(Request.Form["notify_emailfrom"]))
            {
                sub_prop_data.EmailFrom = Request.Form["notify_emailfrom"];
            }
            else
            {
                sub_prop_data.EmailFrom = "";
            }

            sub_prop_data.UseContentTitle = "";

            if (!String.IsNullOrEmpty(Request.Form["use_contentlink_button"]))
            {
                sub_prop_data.UseContentLink = 1;
            }
            else
            {
                sub_prop_data.UseContentLink = 0;
            }

            if (!String.IsNullOrEmpty(Request.Form["content_sub_assignments"]))
            {
                arrSubscriptions = Strings.Split(Strings.Trim(Request.Form["content_sub_assignments"]), " ", -1, 0);
                if (arrSubscriptions.Length > 0)
                {
                    for (isub = 0; isub <= (arrSubscriptions.Length - 1); isub++)
                    {
                        page_sub_temp = new Collection();
                        page_sub_temp.Add(long.Parse(Strings.Mid(arrSubscriptions.GetValue(isub).ToString(), 10)), "ID", null, null);
                        page_subscription_data.Add(page_sub_temp, null, null, null);
                    }
                }
            }
            else
            {
                page_subscription_data = null;
            }
            page_sub_temp = null;

            if (!((Request.Form["content_id"] != "-1") && (Request.Form["content_id"] != "0") && (actionType != "add")))
            {
                if (!((contentCount == null) && (actionType != "multiple,add")))
                {
                    cCont.Add(sub_prop_data, "SubscriptionProperties", null, null);
                    cCont.Add(page_subscription_data, "Subscriptions", null, null);
                }
            }
        }

        if ((Request.Form["AddQlink"] == "AddQlink") || (editaction == ""))
        {
            cCont.Add(true, "AddToQlink", null, null);
        }
        else
        {
            //in case Drag and drop AddQlink is nothing
            if (Request.Form["AddQlink"] == null)
            {
                cCont.Add(true, "AddToQlink", null, null);
            }
            else if (Request.Form["AddQlink"] == "")
            {
                cCont.Add(true, "AddToQlink", null, null);
            }
            else if (Request.Form["AddQlink"] == "1")
            {
                cCont.Add(true, "AddToQlink", null, null);
            }
            else
            {
                cCont.Add(false, "AddToQlink", null, null);
            }
        }

        if (Request.Form["IsSearchable"] == "IsSearchable")
        {
            cCont.Add(true, "IsSearchable", null, null);
        }
        else
        {
            //in case Drag and drop IsSearchable is nothing
            if (Request.Form["IsSearchable"] == null)
            {

                cCont.Add(false, "IsSearchable", null, null);
            }
        }

        if (Request.Form["templateSelect"] != "")
        {
            cCont.Add(Request.Form["templateSelect"], "MultiTemplateID", null, null);
        }

        long manualAliasId;
        manualAliasId = System.Convert.ToInt64(Request.Form["frm_manalias_id"]);
        m_prevManualAliasName = Request.Form["prev_frm_manalias_name"];
        m_prevManualAliasExt = Request.Form["prev_frm_manalias_ext"];
        m_currManualAliasName = Request.Form["frm_manalias"];
        m_currManualAliasExt = Request.Form["frm_manaliasExt"];
        cCont.Add(manualAliasId, "UrlAliasId", null, null);
        cCont.Add(m_currManualAliasName, "NewUrlAliasName", null, null);
        cCont.Add(m_currManualAliasExt, "NewUrlAliasExt", null, null);
        cCont.Add(m_prevManualAliasName, "OldUrlAliasName", null, null);
        cCont.Add(m_prevManualAliasExt, "OldUrlAliasExt", null, null);

        if (Request.Form["Method"] == "updatepublish")
        {
            m_refContent.CheckContentOutv2_0(Convert.ToInt64(Request.Form["ContentId"]));
        }

        cCont.Add(fileChanged, "FileChanged", null, null);


        if ((Request.Form["content_id"] != "-1") && (Request.Form["content_id"] != "0") && (actionType != "add") && Request.Form["translate"] != "true")
        {

            intContentId = Convert.ToInt64(Request.Form["content_id"]);

            try
            {

                m_refContent.CheckContentOutv2_0(intContentId);
                m_refContent.SaveContentv2_0(cCont);
                if (!((Request.Form["suppress_notification"]) != ""))
                {
                    m_refContent.UpdateSubscriptionPropertiesForContent(intContentId, sub_prop_data);
                    m_refContent.UpdateSubscriptionsForContent(intContentId, page_subscription_data);
                }

                // process tag info
                ProcessTags(intContentId, ContentLanguage);

                if (editaction == "checkin")
                {
                    m_refContent.CheckIn(intContentId, "");
                }
                if (editaction == "publish")
                {
                    m_refContent.CheckIn(intContentId, "");
                    m_refContent.SubmitForPublicationv2_0(intContentId, Convert.ToInt64(Request.Form["FolderID"]), "");
                }
                if (Request.Form["Toolbar"] == "True")
                {
                    if (cCont["AssetFilename"].ToString().IndexOf(".dot") > 1)
                    {
                        m_refContent.UpdateDocumentMetadata(cCont["AssetID"].ToString(), intContentId, -1, "");
                    }
                }
                if (Request.Form["FromEE"] == "true" || Request.QueryString["close"] == "true")
                {
                    if (Request.QueryString["close"] == "true")
                    {
                        Response.Redirect("close.aspx?reload=true", false);
                    }
                    else
                    {
                        Response.Redirect("close.aspx", false);
                    }
                }
                else
                {
                    //if save was clicked redirect to edit screen else content.aspx
                    if (editaction == "save")
                    {
                        string queryStr = "";
                        long taxonomyId;
                        if ((Request.Form["TaxonomyOverrideId"] != null) && (long.TryParse(Request.Form["TaxonomyOverrideId"], out taxonomyId)) && (taxonomyId > 0))
                        {
                            queryStr = (string)("&TaxonomyId=" + Request.Form["TaxonomyOverrideId"]);
                        }
                        if ((!(Request.QueryString["pullapproval"] == null)) && (Request.QueryString["pullapproval"].Length > 0))
                        {
                            queryStr += (string)("&pullapproval=" + Request.QueryString["pullapproval"]);
                        }
                        Response.Redirect((string)("edit.aspx?close=false&LangType=" + ContentLanguage.ToString() + "&id=" + intContentId + "&type=update&back_file=content.aspx&back_action=View&back_folder_id=" + Request.Form["content_folder"] + "&back_id=" + intContentId + "&back_callerpage=content.aspx&back_origurl=action%3dViewContentByCategory%26id%3d" + Request.Form["content_folder"] + "&back_LangType=" + ContentLanguage + queryStr), false);
                    }
                    else
                    {
                        Response.Redirect((string)("content.aspx?id=" + Request.Form["content_id"] + "&action=viewstaged&LangType=" + ContentLanguage), false);
                    }
                }
            }
            catch (Exception ex)
            {
                Utilities.ShowError(ex.Message);
            }
        }
        else
        {
            if (((contentCount == null) && (actionType != "multiple,add")) || Request.Form["translate"] == "true")
            {
                try
                {
                    if (Request.Form["Method"] == "copy")
                    {
                        cCont.Add(true, "DontCreateAsset", null, null);
                        cCont.Remove("ContentID");
                        cCont.Add(Request.Form["ContentId"], "ContentID", null, null);
                    }
                    AddMultiResult = m_refContent.AddMultiContent(cCont);
                    intContentId = AddMultiResult[0].ContentId; //m_refContent.AddNewContentv2_0(cCont);
                    if (!((Request.Form["suppress_notification"]) != ""))
                    {
                        m_refContent.UpdateSubscriptionPropertiesForContent(intContentId, sub_prop_data);
                        m_refContent.UpdateSubscriptionsForContent(intContentId, page_subscription_data);
                    }

                    // process tag info
                    ProcessTags(intContentId, ContentLanguage);
                    /*
                    if (editaction == "checkin")
                    {
                        m_refContent.CheckIn(intContentId, "");
                    }
                    if (editaction == "publish")
                    {
                        m_refContent.CheckIn(intContentId, "");
                        m_refContent.SubmitForPublicationv2_0(intContentId, Convert.ToInt64(Request.Form["FolderID"]), "");
                    }*/
                    if (Request.Form["Toolbar"] == "True")
                    {
                        if (cCont["AssetFilename"].ToString().IndexOf(".dot") > 1)
                        {
                            m_refContent.UpdateDocumentMetadata(cCont["AssetID"].ToString(), intContentId, -1, "");
                        }
                    }
                }
                catch (Exception ex)
                {
                    Utilities.ShowError(ex.Message);
                }
                if (Request.Form["FromEE"] == "true" || Request.QueryString["close"] == "true")
                {
                    if (Request.QueryString["close"] == "true")
                    {
                        Response.Redirect("close.aspx?reload=true", false);
                    }
                    else
                    {
                        Response.Redirect("close.aspx", false);
                    }
                }
                else
                {
                    Response.Redirect((string)("content.aspx?id=" + Request.Form["content_folder"] + "&action=ViewContentByCategory&LangType=" + ContentLanguage), false);
                }
            }
            else
            {
                bool isUrlAliasRequired = false;

                FolderData fdTmp = this.m_refContApi.EkContentRef.GetFolderById(long.Parse(Request.Form["content_folder"]));
                searc = fdTmp.IscontentSearchable;
                Ektron.Cms.UrlAliasing.UrlAliasSettingsApi m_urlAliasSettings = new Ektron.Cms.UrlAliasing.UrlAliasSettingsApi();

                if (m_urlAliasSettings.IsManualAliasEnabled)
                {
                    if (fdTmp.AliasRequired)
                    {
                        isUrlAliasRequired = true;
                    }
                }
                
                AddMultiResult = m_refContent.AddMultiContent(cCont);
                for (i = 0; i <= AddMultiResult.Length - 1; i++)
                {
                    if (AddMultiResult[i].ErrorMessage != null && AddMultiResult[i].ErrorMessage.Length > 0)
                    {
                        //make sure an error code is returned if there is an error message!
                        result.Append(AddMultiResult[i].ErrorMessage);
                        Utilities.ShowError(result.ToString());
                        return;
                    }
                    else
                    {
                        ContentData cdTmp = this.m_refContent.GetContentById(AddMultiResult[i].ContentId);
                        //------------ContentSearchable----------------------
                        if(Request.UrlReferrer.Segments.Count(x=>x.ToLower()=="edit.aspx")==0) // run the next line only if the url is not coming from edit.aspx
                            IsContentSearchableSection(AddMultiResult[i].ContentId);
                        //--------------ContentSearchable End-------------
                        bool isImage = ConfigManager.IsImageAsset(cdTmp.AssetData.FileExtension);
                        if (!isImage && isUrlAliasRequired && ( isCallerDmsDragDropCtl ||  Request.Form["FromMenuMultiDMS"]!=null) )
                        {
                            if (cdTmp.Status == "A")//only need to process published content
                            {
                                m_refContent.CheckContentOutv2_0(AddMultiResult[i].ContentId);
                                m_refContent.CheckIn(AddMultiResult[i].ContentId, "");
                            }

                        }


                        if (Request.Form["Toolbar"] == "True")
                        {
                            if (Files[i].IndexOf(".dot") > 1)
                            {
                                m_refContent.UpdateDocumentMetadata(AssetIds[i], System.Convert.ToInt64(cCont["FolderID"]), -1, AddMultiResult[i].AssetVersion);
                            }
                        }

                        // process tag info
                        ProcessTags(AddMultiResult[i].ContentId, ContentLanguage);
                    }
                }
                //If some invalid file types did not get uploaded
                if ((actionType == "multiple,add") && (isCallerDmsDragDropCtl == false) && (Session[Convert.ToString(userId) + Convert.ToString(loginToken) + "RejectedFiles"] != null) && (Session[Convert.ToString(userId) + Convert.ToString(loginToken) + "RejectedFiles"].ToString().Length > 0))
                {
                    if (Request.Form["FromEE"] == "true" || Request.QueryString["close"] == "true")
                    {
                        if (Request.QueryString["close"] == "true")
                        {
                            Response.Redirect("close.aspx?reload=true", false);
                        }
                        else
                        {
                            Response.Redirect("close.aspx", false);
                        }
                    }
                    else
                    {
                        Response.Redirect((string)("content.aspx?id=" + Request.Form["content_folder"] + "&action=ViewContentByCategory&LangType=" + ContentLanguage), false);
                    }
                }
                else
                {
                    if (isCallerDmsDragDropCtl == true && Request.Form["requireMetaTaxonomy"].ToString().ToLower() == "true")
                    {
                        string _taxString = string.Empty;
                        string _idString = string.Empty;
                        string _contentId = string.Empty;
                        foreach (Ektron.Cms.Common.ContentResponseData multiData in AddMultiResult)
                        {
                            if (_idString.Length == 0)
                            {
                                _contentId = multiData.ContentId.ToString();
                            }
                            _idString += multiData.ContentId + ",";
                        }
                        Boolean metadataOrTaxonomyRequired = false;
                        if (!string.IsNullOrEmpty(Request.Form["requireMetaTaxonomy"]) && Request.Form["requireMetaTaxonomy"].ToLower().ToString() == "true")
                        {
                            metadataOrTaxonomyRequired = true;
                        }
                        if (metadataOrTaxonomyRequired)
                        {
                            if ((Request.Form["taxonomyselectedtree"] != null) && Request.Form["taxonomyselectedtree"].ToString().Length > 1)
                            {
                                _taxString = (string)("&taxonomyId=" + Request.Form["taxonomyselectedtree"].ToString());
                            }
                            Response.Redirect((string)("DMSMetadata.aspx?contentId=" + _contentId + "&idString=" + _idString + "&close=true&displayUrlAlias=false&folderId=" + Request.Form["content_folder"] + _taxString), false);
                        }
                    }
                    if (AddMultiResult != null && AddMultiResult.Length == 1)
                    {
                        if (Request.Form["FromEE"] == "true" || Request.QueryString["close"] == "true")
                        {
                            if (Request.QueryString["close"] == "true")
                            {
                                Response.Redirect("close.aspx?reload=true", false);
                            }
                            else
                            {
                                Response.Redirect("close.aspx", false);
                            }
                        }
                        else
                        {
                            //if save was clicked redirect to edit screen else content.aspx
                            if (editaction == "save")
                            {
                                string queryStr = "";
                                long taxonomyId;
                                if ((Request.Form["TaxonomyOverrideId"] != null) && (long.TryParse(Request.Form["TaxonomyOverrideId"], out taxonomyId)) && (taxonomyId > 0))
                                {
                                    queryStr = (string)("&TaxonomyId=" + Request.Form["TaxonomyOverrideId"]);
                                }
                                if ((!(Request.QueryString["pullapproval"] == null)) && (Request.QueryString["pullapproval"].Length > 0))
                                {
                                    queryStr += (string)("&pullapproval=" + Request.QueryString["pullapproval"]);
                                }
                                Response.Redirect((string)("edit.aspx?close=false&LangType=" + ContentLanguage.ToString() + "&id=" + AddMultiResult[0].ContentId + "&type=update&back_file=content.aspx&back_action=View&back_folder_id=" + Request.Form["content_folder"] + "&back_id=" + AddMultiResult[0].ContentId + "&back_callerpage=content.aspx&back_origurl=action%3dViewContentByCategory%26id%3d" + Request.Form["content_folder"] + "&back_LangType=" + ContentLanguage + queryStr), false);
                            }
                            else
                            {
                                //If coming from DragDropCtl.aspx, close thickbox
                                if (isCallerDmsDragDropCtl == true)
                                {
                                    //if any rejected files, display error message before closing thickbox
                                    if ((Session[Convert.ToString(userId) + Convert.ToString(loginToken) + "RejectedFiles"] != null) && Session[Convert.ToString(userId) + Convert.ToString(loginToken) + "RejectedFiles"].ToString() != "")
                                    {
                                        this.jsInvalidFiles.Text = m_refMsg.GetMessage("lbl error message for multiupload") + " " + Session[Convert.ToString(userId) + Convert.ToString(loginToken) + "RejectedFiles"].ToString().Replace("\\", "\\\\").Replace("\'", "\\\'");
                                        this.jsInvalidFiles.Text += "\\n" + m_refMsg.GetMessage("js:cannot add file with add and plus");
                                        this.uxAlertInvalidFileType.Visible = true;
                                        Session.Remove(Convert.ToString(userId) + Convert.ToString(loginToken) + "RejectedFiles");

                                    }
                                    this.uxCloseThickBox.Visible = true;
                                }
                                else
                                {
                                    Response.Redirect((string)("content.aspx?id=" + AddMultiResult[0].ContentId + "&action=viewstaged&LangType=" + ContentLanguage), false);
                                }
                            }
                        }
                    }
                    else
                    {
                        if (Request.Form["FromEE"] == "true" || Request.QueryString["close"] == "true")
                        {
                            if (Request.QueryString["close"] == "true")
                            {
                                Response.Redirect("close.aspx?reload=true", false);
                            }
                            else
                            {
                                Response.Redirect("close.aspx", false);
                            }
                        }
                        else
                        {
                            if (isCallerDmsDragDropCtl == true)
                            {
                                this.uxCloseThickBox.Visible = true;
                            }
                            else
                            {
                                Response.Redirect((string)("content.aspx?id=" + Request.Form["content_folder"] + "&action=ViewContentByCategory&LangType=" + ContentLanguage), false);
                            }
                        }
                    }
                }
            }
        }
    }
    private void IsContentSearchableSection(long contentID)
    {     
        Collection  pagedata = new Collection();
        pagedata.Add(contentID.ToString(), "ContentID", null, null);
        pagedata.Add(false, "XmlInherited", null, null);
        pagedata.Add(Request.Form["xmlconfig"], "CollectionID", null, null);
        if (searc)
        {
            pagedata.Add(1, "IsSearchable", null, null);
        }
        else
        {
            pagedata.Add(0, "IsSearchable", null, null);
        }

        long userID = m_refContent.RequestInformation.UserId;
        m_refContent.RequestInformation.CallerId = EkConstants.InternalAdmin;
        try
        {
            m_refContent.UpdateContentProperties(pagedata);
        }
        finally
        {
            m_refContent.RequestInformation.CallerId = userID;
        }
    }
    private void ProcessUploadOffice2010()
    {
        try
        {
            string File = null;
            Int64 contentID = -1;
            Collection cCont = new Collection();


            ContentEditData cData = null;
            int MetaCount = -1;
            //if (Session["EkDavSessionASDList"] == null)
            //{
            //    ClientScript.RegisterStartupScript(this.GetType(), "selectTab", "alert('" + this.m_refContApi.EkMsgRef.GetMessage("lbl no files selected for multiupload") + "')", true);
            //    return;
            //}
            //if (((List<Ektron.Cms.AssetFileData>)Session["EkDavSessionASDList"]).Count == 0)
            //{
            //    ClientScript.RegisterStartupScript(this.GetType(), "selectTab", "alert('" + this.m_refContApi.EkMsgRef.GetMessage("lbl no files selected for multiupload") + "')", true);
            //    return;
            //}
            foreach (NameValueCollection item in (List<System.Collections.Specialized.NameValueCollection>)Session["EkDavSessionASDList"])
            {
                if (Int64.TryParse(item.Keys[0], out contentID))
                {
                    if (contentID == -1)
                        continue;

                    cData = m_refContent.GetContentForEditing(contentID);
                    cData.Comment = Request.Form["content_comment"];
                    cData.IsSearchable = (Request.Form["IsSearchable"] == "IsSearchable");
                    cData.Teaser = Request.Form["content_teaser"];

                    if (Request.Form["go_live"] != "")
                    {
                        try { cData.GoLive = DateTime.Parse(Strings.Trim(Request.Form["go_live"])).ToString(); }
                        catch (Exception) { }
                    }
                    if (Request.Form["end_date"] != "")
                    {
                        try { cData.EndDate = DateTime.Parse(Strings.Trim(Request.Form["end_date"])).ToString(); }
                        catch (Exception) { }
                        int edAction = -1;
                        if (int.TryParse(Request.Form["end_date_action_radio"], out edAction))
                        { cData.EndDateAction = edAction; }
                    }

                    //Update META
                    Collection cMetadataTypes = new Collection();
                    string[] acMetaInfo = new string[4];
                    if (!String.IsNullOrEmpty(Request.Form["frm_validcounter"]))
                    {
                        MetaCount = System.Convert.ToInt32(Request.Form["frm_validcounter"]);


                        for (int i = 1; i <= MetaCount; i++)
                        {
                            acMetaInfo[1] = Request.Form["frm_meta_type_id_" + i];
                            acMetaInfo[2] = contentID.ToString();
                            acMetaInfo[3] = Request.Form["frm_text_" + i];
                            cMetadataTypes.Add(acMetaInfo, i.ToString(), null, null);
                            acMetaInfo = new string[4];
                        }
                    }
                    //Image Gallery metadata
                    acMetaInfo = UpdateMetadataContent();
                    if (acMetaInfo != null)
                    {
                        cMetadataTypes.Add(acMetaInfo, System.Convert.ToString(cMetadataTypes.Count + 1), null, null);
                    }
                    if (MetaCount > 0)
                        m_refContApi.EkContentRef.UpdateMetaData(cMetadataTypes);
                    //END Update META

                    //Update Taxnomoy
                    if (!string.IsNullOrEmpty(Request.Form["taxonomyselectedtree"]))
                    {
                        string taxIdstr = Request.Form["taxonomyselectedtree"];
                        TaxonomyContentRequest request_t = new TaxonomyContentRequest();
                        request_t.ContentId = contentID;
                        request_t.TaxonomyList = taxIdstr;
                        request_t.FolderID = Convert.ToInt64(Request.Form["FolderID"]);
                        m_refContApi.EkContentRef.AddTaxonomyItem(request_t);
                    }
                    cData.FileChanged = false;
                    m_refContent.CheckContentOutv2_0(contentID);
                    m_refContent.SaveContentv2_0(cData);

                    bool isUrlAliasRequired = false;

                    FolderData fdTmp = this.m_refContApi.EkContentRef.GetFolderById(cData.FolderId);
                    Ektron.Cms.UrlAliasing.UrlAliasSettingsApi m_urlAliasSettings = new Ektron.Cms.UrlAliasing.UrlAliasSettingsApi();

                    if (m_urlAliasSettings.IsManualAliasEnabled)
                    {
                        if (fdTmp.AliasRequired)
                        {
                            isUrlAliasRequired = true;
                        }
                    }
                    bool isImage = ConfigManager.IsImageAsset(cData.AssetData.FileExtension);

                    switch (Request.Form["editaction"])
                    {
                        case "checkin":
                            m_refContent.CheckIn(contentID, "");
                            break;
                        case "publish":
                            m_refContent.CheckIn(contentID, "");
                            if (isImage || (!isUrlAliasRequired))//publish image file or folder not require Url Alias
                                m_refContent.SubmitForPublicationv2_0(contentID, Convert.ToInt64(Request.Form["FolderID"]), "");
                            break;
                        case "save":
                            //do nothing. leave them in checkout state.
                            break;
                    }

                    File = item[item.Keys[0]];
                }
            }



            Response.Redirect((string)("content.aspx?id=" + Request.Form["content_folder"] + "&action=ViewContentByCategory&LangType=" + ContentLanguage), false);
        }
        catch (Exception ex)
        {
            Utilities.ShowError(ex.Message.ToString());
        }
        finally
        {
            //reset session vars
            Session["EkDavSessionVal"] = null;
            Session["EkDavSessionASDList"] = null;
        }

    }

    private int GetAddMultiType()
    {
        int returnValue;
        // gets ID for "add multiple" asset type
        returnValue = 0;
        AssetInfoData[] asset_data;
        asset_data = m_refContent.GetAssetSuperTypes();
        int count;
        if (asset_data != null)
        {
            for (count = 0; count <= asset_data.Length - 1; count++)
            {
                if (ManagedAsset_Min <= asset_data[count].TypeId && asset_data[count].TypeId <= ManagedAsset_Max)
                {
                    if ("*" == asset_data[count].PluginType)
                    {
                        returnValue = asset_data[count].TypeId;
                    }
                }
            }
        }
        return returnValue;
    }

    private string[] UpdateMetadataContent()
    {
        string[] returnValue = null;
        try
        {
            if ((Request.Form["ekImagegalleryAddress"] != null) && (Request.Form["ekImagegalleryAddress"] != ""))
            {
                //get the metadata - address
                long metadataID = 0;
                Ektron.Cms.API.Metadata metadata = new Ektron.Cms.API.Metadata();
                Ektron.Cms.ContentMetaData[] metadataTypes;
                metadataTypes = metadata.GetMetaDataTypes("name");
                foreach (Ektron.Cms.ContentMetaData type in metadataTypes)
                {
                    if (type.TypeName.ToLower() == "mapaddress")
                    {
                        metadataID = type.TypeId;
                        break;
                    }
                }
                if (metadataID == 0)
                {
                    return null;
                }

                string[] acMetaInfo = new string[4];

                //Create the collection of data
                acMetaInfo[1] = metadataID.ToString();
                acMetaInfo[2] = "";
                acMetaInfo[3] = Request.Form["ekImagegalleryAddress"];
                return acMetaInfo;
            }

        }
        catch (Exception)
        {
            // Do Nothing
            return null;
        }
        return returnValue;
    }


    public void ProcessTags(long Id, int langId)
    {
        Ektron.Cms.Common.EkEnumeration.CMSObjectTypes tagtype = Ektron.Cms.Common.EkEnumeration.CMSObjectTypes.Content;
        TagData[] defaultTags;
        TagData[] Tags;
        Ektron.Cms.API.Community.Tags m_refTagsApi = new Ektron.Cms.API.Community.Tags();
        string orginalTagIds;
        string tagIdStr = "";
        string cTags = Page.Request.Form["currentTags"];
        if (cTags != null)
        {
            orginalTagIds = (string)(cTags.Trim().ToLower());
        }
        else
        {
            orginalTagIds = "";
        }
        //Assign all default user tags that are checked:Remove tags that have been unchecked
        defaultTags = m_refTagsApi.GetDefaultTags(tagtype, -1);
        Tags = m_refTagsApi.GetTagsForObject(Id, tagtype);

        //Also, copy all users tags into defaultTags list. so that if they were removed, they can be deleted as well.
        int originalLength = defaultTags.Length;
        Array.Resize(ref defaultTags, defaultTags.Length + Tags.Length - 1 + 1);
        Tags.CopyTo(defaultTags, originalLength);

        if (defaultTags != null)
        {

            foreach (TagData td in defaultTags)
            {
                tagIdStr = (string)("userPTagsCbx_" + td.Id.ToString());
                if (!(Page.Request.Form[tagIdStr] == null))
                {
                    if (Page.Request.Form[tagIdStr] == "on")
                    {
                        //if tag is checked, but not in current tag list, add it
                        if (!orginalTagIds.Contains((string)(td.Id.ToString() + ",")))
                        {
                            m_refTagsApi.AddTagToObject(td.Id, Id, tagtype, -1, langId);
                        }
                    }
                    else
                    {
                        //if tag is unchecked AND in current list, delete
                        if (orginalTagIds.Contains((string)(td.Id.ToString() + ",")))
                        {
                            m_refTagsApi.DeleteTagOnObject(td.Id, Id, tagtype, 0);
                        }
                    }
                }
                else
                {
                    //If tag checkbox has no postback value AND is in current tag list, delete it
                    if (orginalTagIds.Contains((string)(td.Id.ToString() + ",")))
                    {
                        m_refTagsApi.DeleteTagOnObject(td.Id, Id, tagtype, 0);
                    }
                }
            }

            // Now add any new custom tags, that the user created:
            // New tags are added to newTagNameHdn field in following format:  <TagText>~<LanguageID>;<TagText>~<LanguageID>

            if (Page.Request["newTagNameHdn"] != null)
            {
                string custTags = (Page.Request["newTagNameHdn"].ToString());
                char[] tagsep = { ';' };
                string[] aCustTags = custTags.Split(tagsep);

                int languageId;
                char[] langsep = { '~' };

                foreach (string tag in aCustTags)
                {
                    string[] tagPropArray = tag.Split(langsep);
                    if (tagPropArray.Length > 1)
                    {
                        if (tagPropArray[0].Trim().Length > 0)
                        {
                            if (!int.TryParse(tagPropArray[1], out languageId))
                            {
                                languageId = -1;
                            }
                            if (languageId == 0)
                            {
                                languageId = -1;
                            }

                            m_refTagsApi.AddTagToObject(tagPropArray[0], Id, tagtype, -1, languageId);
                        }
                    }
                }
            }
        }
    }
}