using System;
using System.Data;
using System.Configuration;
using System.Collections;
using System.Text;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;

using Ektron.ASM.AssetConfig;
using Ektron.Cms;
using Ektron.Cms.Common;

public partial class Community_DistributionWizard_FolderData : System.Web.UI.Page
{
    private ContentAPI contentAPI = null;
    private SiteAPI siteAPI = null;

    /// <summary>
    /// This page generates the data (with HTML markup) necessary to populate
    /// the ajax content selection control (SelectContent).
    /// </summary>
    protected void Page_Load(object sender, EventArgs e)
    {
        long folderID = -1;
        string folderIDParam = Request.QueryString["FolderID"];     // ID of folder (Required)
        if (!String.IsNullOrEmpty(folderIDParam))
        {
            long.TryParse(folderIDParam, out folderID);
        }

        string dataRequestType = Request.QueryString["Request"];    // Type of data requested (Required)
        string cleanUpID = Request.QueryString["CleanUpID"];        // ID of script element (Required)

        bool IsDistributionWizard = true;
        bool.TryParse(Request.QueryString["IsDistWiz"], out IsDistributionWizard);

        bool EnableMultiSelectContent = false;
        bool.TryParse(Request.QueryString["EnableMulti"], out EnableMultiSelectContent);

        if (!String.IsNullOrEmpty(dataRequestType) && !String.IsNullOrEmpty(cleanUpID) && folderID > -1)
        {
            contentAPI = new ContentAPI();
            siteAPI = new SiteAPI();

            switch (dataRequestType)
            {
                // "Name" parameter indicates that the name of the specified
                // folder will be returned.
                case "Name":
                    string folderNameCallback = GetFolderName(folderID);
                    folderNameCallback = "displayFolderNameCallback(\"" +
                        Escape(folderNameCallback) +
                        "\"); cleanUp(\"" + cleanUpID + "\");";

                    Response.Write(folderNameCallback);
                    Response.End();
                    break;

                // "Content" parameter indicates that list of child contents
                // for the specified folder will be returned. This content
                // can be filtered with the "ContentTypeID" and AssetExtension
                // parameters.
                case "Content":
                    int languageID = -1;
                    int contentTypeID = -1;
                    bool showRelatedContent = false;
                    bool checkEditPermissions = false;
                    string assetExtension = String.Empty;

                    string languageIDParam = Request.QueryString["LanguageID"];     // ID of desired content language
                    if (!String.IsNullOrEmpty(languageIDParam))
                    {
                        int.TryParse(languageIDParam, out languageID);
                    }

                    assetExtension = Request.QueryString["AssetExtension"];        // Asset extension for filtering

                    string showRelatedContentParam = Request.QueryString["ShowRelated"];  // Enable relationship filtering
                    if (!String.IsNullOrEmpty(showRelatedContentParam))
                    {
                        bool.TryParse(showRelatedContentParam, out showRelatedContent);
                    }

                    string checkEditParam = Request.QueryString["CheckEditPermissions"];
                    if (!String.IsNullOrEmpty(checkEditParam))
                    {
                        bool.TryParse(checkEditParam, out checkEditPermissions);
                    }

                    string contentTypeIDParam = Request.QueryString["ContentTypeID"];          // Content type ID for filtering
                    int.TryParse(contentTypeIDParam, out contentTypeID);

                    string folderContentCallback = GetFolderContentList(
                        folderID,
                        assetExtension,
                        contentTypeID,
                        languageID,
                        checkEditPermissions,
                        showRelatedContent,
                        IsDistributionWizard,
                        EnableMultiSelectContent);

                    folderContentCallback = "displayContentCallback(\"" +
                        Escape(folderContentCallback) +
                        "\"); cleanUp(\"" + cleanUpID + "\");";

                    Response.Write(folderContentCallback);
                    Response.End();
                    break;
                default:
                    Response.Write("Error");
                    Response.End();
                    break;
            }
        }
        else
        {
            Response.Write("Error");
            Response.End();
        }

    }

    /// <summary>
    /// Returns the name of the specified folder.
    /// </summary>
    /// <param name="folderID">ID of the folder</param>
    /// <returns>Name of the specified folder</returns>
    private string GetFolderName(long folderID)
    {
        string folderName = string.Empty;

        FolderData folder = contentAPI.GetFolderById(folderID);
        if (folder != null)
        {
            folderName = folder.Name;
        }
        else
        {
            Response.Write("Error");
        }

        return EkFunctions.HtmlEncode(folderName);
    }

    private string GetStringFromEnumByKey(Int32 value)
    {
        //If found return string else return empty string
        string mystring = Enum.GetName(typeof(EkEnumeration.CMSContentType), value) as string;
        //Return string
        return mystring;
    }

    /// <summary>
    /// Returns an list of the specified folder's child content, formatted as HTML.
    /// </summary>
    /// <param name="folderID">ID of the folder</param>
    /// <param name="assetExtension">Display only assets with this file extension. Filter is ignored if null or empty.</param>
    /// <param name="contentTypeID">Display only content or this type. Filter is ignored if -1.</param>
    /// <returns>List of the specified folder's child content</returns>
    private string GetFolderContentList(long folderID, string assetExtension, int contentTypeID, int languageID, bool checkEditPermissions, bool showRelatedContent, bool IsDistributionWizard, bool EnableMultiSelectContent)
    {
        StringBuilder sbFolderContentList = new StringBuilder();

        int totalPages = 1;
        int currentPage = 0;

        sbFolderContentList.Append("<ul>");

        while (currentPage < totalPages)
        {
            ContentData[] contentList = contentAPI.GetChildContentByFolderId(
                folderID,
                false,
                "content_title",
                currentPage,
                ref totalPages,
                300);

            if (contentList != null)
            {
                foreach (ContentData content in contentList)
                {
                    // Check if the user has the privs to replace this content
                    if (CurrentUserHasPermission(content.Id, checkEditPermissions))
                    {
                        // Check if the content matches the desired language
                        if (languageID == -1 || content.LanguageId == languageID)
                        {
                            // Check if the content is already in a distribution relationship.
                            if (showRelatedContent || !IsRelatedContent(content.Id))
                            {
                                // If the content is an asset, compare it's file extension.
                                if (String.IsNullOrEmpty(assetExtension) ||
                                    assetExtension.ToLower() == content.AssetData.FileExtension.ToLower())
                                {
                                    // Check if the content matches the desired type
                                    if (contentTypeID == -1 ||
                                        (EkEnumeration.CMSContentType)contentTypeID == GetCMSContentType(content.Type, true))
                                    {
                                        //Check to see if it is of sub type pagebuilder
                                        if (content.SubType != EkEnumeration.CMSContentSubtype.PageBuilderData)
                                        {
                                            // Check if the content is smart form data.
                                            if (!IsDistributionWizard || !IsSmartFormContent(content.Id))
                                            {
                                                sbFolderContentList.Append("<li class=\"file " + content.AssetData.FileExtension + "\" title=\"File Type: ." + content.AssetData.FileExtension.ToUpper() + "\">");
                                                if (EnableMultiSelectContent)
                                                {
                                                    sbFolderContentList.Append("<input type=\"checkbox\" id=\"ChkContentItem_");
                                                    sbFolderContentList.Append(content.Id.ToString());
                                                    sbFolderContentList.Append("\" value=\"");
                                                    sbFolderContentList.Append(content.Id.ToString());
                                                    sbFolderContentList.Append("\" onclick=\"selectMultiContent('");
                                                    sbFolderContentList.Append(content.Id.ToString());
                                                    sbFolderContentList.Append("', true);\" />");
                                                }
                                                sbFolderContentList.Append("<a href=\"#\" id=\"ContentItem_");
                                                sbFolderContentList.Append(content.Id.ToString());
                                                if (EnableMultiSelectContent)
                                                {
                                                    sbFolderContentList.Append("\" onclick='selectMultiContent(\"");
                                                    sbFolderContentList.Append(content.Id.ToString());
                                                    sbFolderContentList.Append("\", false)' />");
                                                }
                                                else
                                                {
                                                    sbFolderContentList.Append("\" onclick=\"selectContent('");
                                                    sbFolderContentList.Append(content.Id.ToString());
                                                    sbFolderContentList.Append("')\">");
                                                }

                                                sbFolderContentList.Append(content.Title);
                                                sbFolderContentList.Append("</a></li>");
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }

            currentPage++;
        }

        sbFolderContentList.Append("</ul>");

        return sbFolderContentList.ToString();
    }

    /// <summary>
    /// Returns true if user permission to manipulate the specified content.
    /// </summary>
    /// <param name="contentID">ID of content to check</param>
    /// <param name="checkEditPermissions">True if user must have edit permissions</param>
    /// <returns>True if user permission to manipulate the specified content, false otherwise.</returns>
    private bool CurrentUserHasPermission(long contentID, bool checkEditPermissions)
    {
        bool hasPermission = true;
        if (checkEditPermissions)
        {
            PermissionData contentPermissions = contentAPI.LoadPermissions(
                contentID,
                "content",
                ContentAPI.PermissionResultType.All);

            if (checkEditPermissions)
            {
                if (!contentPermissions.IsAdmin && !contentPermissions.CanAdd)
                {
                    hasPermission = false;
                }
            }
        }

        return hasPermission;
    }

    /// <summary>
    /// Returns true if specified content is xml data (associated with a smart form).
    /// </summary>
    /// <param name="contentId">ID of content</param>
    /// <returns>True if specified content is xml data (associated with a smart form), false otherwise.</returns>
    private bool IsSmartFormContent(long contentId)
    {
        bool isSmartFormContent = false;

        ContentData content = contentAPI.GetContentById(contentId, ContentAPI.ContentResultType.Published);
        if (content != null)
        {
            isSmartFormContent = (content.XmlConfiguration != null);
        }

        return isSmartFormContent;
    }

    /// <summary>
    /// Returns true if the content is in a pre-existing distribution relationship.
    /// </summary>
    /// <param name="destinationContentID">ID of content</param>    
    /// <returns>True if the content is in a pre-existing distribution relationship, false otherwise</returns>
    private bool IsRelatedContent(long destinationContentID)
    {
        return contentAPI.GetSourceContentId(destinationContentID) > 0;
    }

    /// <summary>
    /// Returns the specified string escaped for inclusion in javascript.
    /// </summary>
    /// <param name="str">String to be escaped</param>
    /// <returns>String escaped for inclusion in javascript</returns>
    string Escape(string str)
    {
        return str.Replace("\\", "\\\\").Replace("\"", "\\\"");
    }

    /// <summary>
    /// Content type values do not strictly adhere to those values defined in the
    /// CMSContentType enumeration (e.g. assets) This method returns the corresponding
    /// enumeration value.
    /// </summary>
    /// <param name="contentType">Integer value representing a particular content type</param>
    /// <param name="combineArchive">Combine archived with non-archived content types</param>
    /// <returns>The corresponding CMSContentType enumeration value</returns>
    private EkEnumeration.CMSContentType GetCMSContentType(int contentType, bool combineArchive)
    {
        EkEnumeration.CMSContentType type = EkEnumeration.CMSContentType.AllTypes;

        if (contentType == EkConstants.CMSContentType_Content)
        {
            type = EkEnumeration.CMSContentType.Content;
        }

        if (contentType == EkConstants.CMSContentType_Forms)
        {
            type = EkEnumeration.CMSContentType.Forms;
        }

        if (contentType == EkConstants.CMSContentType_Library)
        {
            type = EkEnumeration.CMSContentType.LibraryItem;
        }

        if (contentType == EkConstants.CMSContentType_Media)
        {
            type = EkEnumeration.CMSContentType.Multimedia;
        }

        if (contentType == EkConstants.CMSContentType_NonLibraryContent)
        {
            type = EkEnumeration.CMSContentType.NonLibraryContent;
        }

        if (contentType >= EkConstants.ManagedAsset_Min && contentType <= EkConstants.ManagedAsset_Max)
        {
            type = EkEnumeration.CMSContentType.Assets;
        }

        if (contentType == EkConstants.CMSContentType_Archive_Content)
        {
            if (combineArchive)
            {
                type = EkEnumeration.CMSContentType.Content;
            }
            else
            {
                type = EkEnumeration.CMSContentType.Archive_Content;
            }
        }

        if (contentType == EkConstants.CMSContentType_Archive_Forms)
        {
            if (combineArchive)
            {
                type = EkEnumeration.CMSContentType.Forms;
            }
            else
            {
                type = EkEnumeration.CMSContentType.Archive_Forms;
            }
        }

        if (contentType == EkConstants.CMSContentType_Archive_Media)
        {
            if (combineArchive)
            {
                type = EkEnumeration.CMSContentType.Multimedia;
            }
            else
            {
                type = EkEnumeration.CMSContentType.Archive_Media;
            }
        }

        if (contentType >= EkConstants.Archive_ManagedAsset_Min && contentType <= EkConstants.Archive_ManagedAsset_Max)
        {
            if (combineArchive)
            {
                type = EkEnumeration.CMSContentType.Assets;
            }
            else
            {
                type = EkEnumeration.CMSContentType.Archive_Assets;
            }
        }

        return type;
    }
}
