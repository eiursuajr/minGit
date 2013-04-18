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

public partial class ajaxfoldertree : System.Web.UI.Page
{
    private ContentAPI contentAPI = null;
    private SiteAPI siteAPI = null;
    private bool ShowAllFolder=false;
    protected void Page_Load(object sender, EventArgs e)
    {
        // cleanupid is the ID of the script tag to which the generated HTML 
        // will be sent

        bool.TryParse(Request.QueryString["showAllFolder"], out ShowAllFolder);

        string cleanupId = EkFunctions.HtmlEncode(Request.QueryString["cleanupid"]);
        // controlid is the ID of the control which is generating the HTML
        string controlId = EkFunctions.HtmlEncode(Request.QueryString["controlid"]);
        string username = EkFunctions.HtmlEncode(Request.QueryString["u"]);
        string password = EkFunctions.HtmlEncode(Request.QueryString["p"]);
        // folderid is the folder from which to get child folder html
        long folderId = long.Parse(Request.QueryString["folderid"]);
        // generate the html for the folder tree children
        string retval = "<div id=\"ekDiv" + controlId + "_" + folderId + "\">" +
                        GenerateTreeHtml(controlId, folderId, username, password) + "</div>";
        // and insert the callback to transfer the html to the client
        retval = "expandCallback(\"" + controlId +
                 "\", \"" + folderId + "\", \"" +
                 Escape(retval) +
            // make sure to clean up the client!!
                 "\"); cleanUp(\"" + cleanupId + "\");";
        Utilities.ValidateUserLogin();
        Response.Write(retval);
        Response.End();
    }

    /// <summary>
    /// Generates the HTML for the child folders under the given folder
    /// </summary>
    /// <param name="controlId">The ID of the surrounding AJAX folder control.</param>
    /// <param name="folderId">The folder ID.</param>
    /// <returns>The HTML list element for one row of the folder tree.</returns>
    protected string GenerateTreeHtml(string controlId, long folderId, string username, string password)
    {
        contentAPI = new Ektron.Cms.ContentAPI();
        siteAPI = new SiteAPI();
               
        if (!siteAPI.IsLoggedIn)
            return "<div class=\"ekError\">Please login</div>";

        // Get the children for the specified folder
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

        StringBuilder sb = new StringBuilder();
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
    /// Generate the HTML for one row of the file tree.
    /// </summary>
    /// <param name="controlId">The ID of the surrounding AJAX folder control.</param>
    /// <param name="folder">The folder data.</param>
    /// <returns>The HTML list element for one row of the folder tree.</returns>
    protected string GenerateFolderHtml(string controlId, FolderData folder)
    {
        siteAPI = new SiteAPI();

        // If we're at the root, return the site path
        string sitePath = siteAPI.SitePath.ToString().TrimEnd(new char[] { '/' })
                          .TrimStart(new char[] { '/' });
        if (sitePath != "")
            sitePath += "/";

        if (Page.Request.Url.Host.ToLower().Equals("localhost"))
            sitePath = Context.Request.Url.Scheme + Uri.SchemeDelimiter + System.Net.Dns.GetHostName() + "/" + sitePath;
        else
            sitePath = Context.Request.Url.Scheme + Uri.SchemeDelimiter + Page.Request.Url.Authority + "/" + sitePath;


        /* 
         * Generate the following HTML: 
         * 
         * <li id="ekFolder#controlId#_#folderId#\>
         *   <a href="#" onclick="toggleTree('#controlId#', #folderId);">
         *     <img src="#sitePath#Workarea/Tree/images/xp/plusopenfolder.gif" border="0"
         *     </img>
         *   </a>
         *   <a href="#" onclick="ekSelectFolder#controlId#(#folderId)">
         *     #folder.Name#
         *   </a>
         * </li>
         */

        StringBuilder sb = new StringBuilder();
        string folderId = folder.Id.ToString();
        sb.Append("<li id=\"ekFolder");
        sb.Append(controlId);
        sb.Append("_");
        sb.Append(folderId);
        sb.Append("\">");
        // If the folder has children, then output the folder image with a [+] next to it

        // Get a list of subfolders filtered according to user's privs. If the folder has children, 
        // then output the folder image with a [+] next to it
        List<FolderData> childFolders = new List<FolderData>(GetFilteredChildFolders(
            folder.Id,
            false,
            EkEnumeration.FolderOrderBy.Id));

        if (childFolders.Count > 0)
        {
            sb.Append("<a onclick=\"toggleTree('");
            sb.Append(controlId);
            sb.Append("', ");
            sb.Append(folderId);
            sb.Append(");\"><img src=\"");
            sb.Append(sitePath);
            sb.Append("Workarea/Tree/images/xp/plusopenfolder.gif\" border=\"0\"></img></a>");
        }
        // Otherwise output standard folder image
        else
        {
            sb.Append("<img src=\"");
            sb.Append(sitePath);
            sb.Append("Workarea/Tree/images/xp/folder.gif\"></img>");
        }
        sb.Append("<a href=\"#\" onclick=\"ekSelectFolder");
        sb.Append(controlId);
        sb.Append("(this,");
        sb.Append(folderId);
        sb.Append(")\">");
        sb.Append(folder.Name);
        sb.Append("</a></li>");
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
                if (CurrentUserHasPermission(folder))
                {
                    if (ShowAllFolder||
                        folder.FolderType == (int)EkEnumeration.FolderType.Content ||
                        folder.FolderType == (int)EkEnumeration.FolderType.Root)
                    {
                        filteredFolders.Add(folder);
                    }
                }
            }
        }

        return filteredFolders;
    }

    private bool CurrentUserHasPermission(FolderData folder)
    {
        bool hasPermission = true;

        PermissionData folderPermissions = contentAPI.LoadPermissions(
            folder.Id,
            "folder",
            ContentAPI.PermissionResultType.All);

        if (!folderPermissions.IsAdmin && !folderPermissions.CanAdd)
        {
            hasPermission = false;
        }

        return hasPermission;
    }

    /// <summary>
    /// Escapes a string so that it can be safely passed through Javascript.
    /// </summary>
    /// <param name="str">String to be escaped.</param>
    /// <returns>A Javascript safe string</returns>
    string Escape(string str)
    {
        return str.Replace("\\", "\\\\").Replace("\"", "\\\"");
    }
}
