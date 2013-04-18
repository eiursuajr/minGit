using System;
using System.Data;
using System.Configuration;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;

using Ektron.Cms;
using Ektron.Cms.Common;

public partial class Community_DistributionWizard_FolderTreeData : System.Web.UI.Page
{
    private ContentAPI contentAPI = null;
    private SiteAPI siteAPI = null;
    
    bool checkAddPermissions = false;
    bool showSpecialFolders = false;

    /// <summary>
    /// This page generates the data (with HTML markup) to support the ajax
    /// folder tree (SelectFolder).
    /// </summary>
    protected void Page_Load(object sender, EventArgs e)
    {
        contentAPI = new Ektron.Cms.ContentAPI();
        siteAPI = new Ektron.Cms.SiteAPI();

        string cleanupId = Request.QueryString["cleanupid"];
        string controlId = Request.QueryString["controlid"];

        long folderId = long.Parse(Request.QueryString["folderid"]);

        string checkAddParam = Request.QueryString["CheckAddPermissions"];
        if (!String.IsNullOrEmpty(checkAddParam))
        {
            bool.TryParse(checkAddParam, out checkAddPermissions);
        }

        string showSpecialFoldersParam = Request.QueryString["ShowSpecialFolders"];
        if (!String.IsNullOrEmpty(showSpecialFoldersParam))
        {
            bool.TryParse(showSpecialFoldersParam, out showSpecialFolders);
        }

        string retval = string.Empty;

        if (folderId != -1)
        {
            retval = "<div id=\"ekDiv" + controlId + "_" + folderId + "\">" +
                            GenerateTreeHtml(controlId, folderId) + "</div>";
        }
        else
        {
            string folderHtml = string.Empty;

            FolderData rootFolder = contentAPI.GetFolderById(0);
            if (rootFolder != null)
            {
                folderHtml = GenerateFolderHtml(controlId, rootFolder);
            }
            else
            {
                folderHtml = "You do not have the permissions necessary to view the folder structure.";
            }

            retval = "<div id=\"ekDiv" + controlId + "_" + folderId + "\"><ul>" + folderHtml + "</ul></div>";
        }

        retval = "expandCallback(\"" + controlId +
                 "\", \"" + folderId + "\", \"" +
                 Escape(retval) +
                 "\"); cleanUp(\"" + cleanupId + "\");";

        Response.Write(retval);
        Response.End();
    }

    /// <summary>
    /// Generates the HTML for the child folders under the given folder
    /// </summary>
    /// <param name="controlId">The ID of the surrounding AJAX folder control.</param>
    /// <param name="folderId">The folder ID.</param>
    /// <returns>The HTML list element for one row of the folder tree.</returns>
    protected string GenerateTreeHtml(string controlId, long folderId)
    {
        StringBuilder sb = new StringBuilder();

        List<FolderData> folderList = null;
        try
        {
            folderList = new List<FolderData>(
                 GetFilteredChildFolders(
                     folderId,
                     false,
                     EkEnumeration.FolderOrderBy.Name));
        }
        catch 
        { 
            // Exception likely occurred due to the user not being logged in.
            // Unfortunately, ContentAPI.IsLoggedIn is not reliable enough (still
            // returns true if user is user is logged out from a different client)
            // to check in this case.
        }

        if (folderList != null)
        {
            if (folderList.Count != 0)
            {
                sb.Append("<ul>");
                foreach (FolderData folder in folderList)
                {
                    sb.Append(GenerateFolderHtml(controlId, folder));
                }
                sb.Append("</ul>");
            }
        }

        return sb.ToString();
    }

    /// <summary>
    /// Returns a (filtered) list of sub-folders for the specified folder.
    /// </summary>
    /// <param name="folderID">ID of folder</param>
    /// <param name="recursive">True if folder list should be recursive retrieved</param>
    /// <param name="orderBy">Field to order list by</param>
    /// <returns>A (filtered) list of sub-folders for the specified folder</returns>
    public IEnumerable<FolderData> GetFilteredChildFolders(long folderID, bool recursive, EkEnumeration.FolderOrderBy orderBy)
    {
        List<FolderData> filteredFolders = new List<FolderData>();

        IEnumerable<FolderData> allChildFolders = contentAPI.GetChildFolders(folderID, recursive, orderBy);
        if (allChildFolders != null)
        {
            foreach (FolderData folder in contentAPI.GetChildFolders(folderID, recursive, orderBy))
            {
                if (showSpecialFolders ||
                    folder.FolderType == (int)EkEnumeration.FolderType.Content ||
                    folder.FolderType == (int)EkEnumeration.FolderType.Root)
                {
                    filteredFolders.Add(folder);
                }
            }
        }

        return filteredFolders;
    }

    /// <summary>
    /// Returns true if the user has all of the permissions necessary to distribute
    /// content to the specified folder.
    /// </summary>
    /// <param name="folderID">ID of folder where content will be distributed</param>
    /// <returns>True if user has permission to distribute to folder</returns>
    private bool CurrentUserHasPermission(long folderID)
    {
        bool hasPermission = true;
       
        if (!contentAPI.IsAdmin() && !this.HasNecessaryFolderPermissions(folderID) )
        {
            hasPermission = false;
        }

        return hasPermission;
    }

    /// <summary>
    /// Returns true if the user has the permissions necessary for
    /// contentn distribution on the specified folder.
    /// </summary>
    /// <param name="folderID">ID of folder</param>
    /// <returns>True if the user has the permissions necessary for content distribution on the specified folder</returns>
    private bool HasNecessaryFolderPermissions(long folderID)
    {
        return contentAPI.EkContentRef.IsAllowed(folderID, 0, "folder", "Add", contentAPI.UserId) &&
            contentAPI.EkContentRef.IsAllowed(folderID, 0, "folder", "Delete", contentAPI.UserId) &&
            contentAPI.EkContentRef.IsAllowed(folderID, 0, "folder", "Restore", contentAPI.UserId);
    }

    /// <summary>
    /// Generate the HTML for one row of the file tree.
    /// </summary>
    /// <param name="controlId">The ID of the surrounding AJAX folder control.</param>
    /// <param name="folder">The folder data.</param>
    /// <returns>The HTML list element for one row of the folder tree.</returns>
    protected string GenerateFolderHtml(string controlId, FolderData folder)
    {
        StringBuilder sb = new StringBuilder();
        string folderId = folder.Id.ToString();
        sb.Append("<li id=\"ekFolder");
        sb.Append(controlId);
        sb.Append("_");
        sb.Append(folderId);
        sb.Append("\">");

        // Get a list of subfolders filtered according to user's privs. If the folder has children, 
        // then output the folder image with a [+] next to it
        List<FolderData> childFolders = new List<FolderData>(GetFilteredChildFolders(
            folder.Id, 
            false, 
            EkEnumeration.FolderOrderBy.Id));

        if (childFolders.Count > 0)
        {
            sb.Append("<img onclick=\"toggleTree('");
            sb.Append(controlId);
            sb.Append("', ");
            sb.Append(folderId);
            sb.Append(");\" src=\"");
            sb.Append(this.SitePath);
            sb.Append("Workarea/images/ui/icons/tree/folderCollapsed.png\" border=\"0\"></img>");
        }
        else
        {
            // Otherwise output standard folder image
            sb.Append("<img src=\"");
            sb.Append(this.SitePath);
            sb.Append("Workarea/images/ui/icons/tree/folder.png\"></img>");
        }
        
        bool hasDistributionPrivs = DistributionWizardHelperMethods.HasDistributionPrivileges(folder.Id);
        
        if (hasDistributionPrivs)
        {
            sb.Append("<a href=\"#\" onclick=\"ekSelectFolder");
            sb.Append(controlId);
            sb.Append("(");
            sb.Append(folderId);
            sb.Append(")\" ");
            sb.Append("id=\"ekFolderAnchor");
            sb.Append(controlId);
            sb.Append("_");
            sb.Append(folderId);
            sb.Append("\">");
        }

        sb.Append(folder.Name);

        if (hasDistributionPrivs)
        {
            sb.Append("</a>");
        }

        sb.Append("</a></li>");

        return sb.ToString();
    }

    /// <summary>
    /// Escape string to make it suitable for inclusion in javascript blocks.
    /// </summary>
    /// <param name="str">String to be escaped</param>
    /// <returns>Escaped representation of the specified string</returns>
    string Escape(string str)
    {
        return str.Replace("\\", "\\\\").Replace("\"", "\\\"");
    }

    
    /// <summary>
    /// Returns the site path.
    /// </summary>
    private string SitePath
    {
        get
        {
            string sitePath = siteAPI.SitePath.ToString().TrimEnd(new char[] { '/' })
                  .TrimStart(new char[] { '/' });
            if (sitePath != "")
                sitePath += "/";

            if (Page.Request.Url.Host.ToLower().Equals("localhost"))
                sitePath = Context.Request.Url.Scheme + Uri.SchemeDelimiter + System.Net.Dns.GetHostName() + "/" + sitePath;
            else
                sitePath = Context.Request.Url.Scheme + Uri.SchemeDelimiter + Page.Request.Url.Authority + "/" + sitePath;

            return sitePath;
        }
    }
}
