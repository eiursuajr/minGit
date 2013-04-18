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
using Ektron.Cms;
using System.IO;

public partial class davurl : System.Web.UI.Page
{
    private ContentAPI contentAPI = null;
    private SiteAPI siteAPI = null;

    protected void DisplayError(string error)
    {
        try
        {
            Response.Write("Error: " + error);
            Response.End();
        }
        catch (IOException ioex)
        {
            DisplayError(ioex.Message);
        }
    }

    protected void Page_Load(object sender, EventArgs e)
    {
        try
        {
            contentAPI = new ContentAPI();
            siteAPI = new SiteAPI();

            switch( Request.QueryString["action"])
            {
                case "getdavurl":
                    Response.Write(GetSharepointUploadPath());
                    break;
                case "getdetails":
                    Response.Write(
                        "showRedistributionStatusCallback(\"" + 
                        EscapeString(GetRedistributionDetails()) + 
                        "\");cleanUp(\"" + 
                        Request.QueryString["cleanupid"] + 
                        "\");");
                    break;
                default:
                    Response.Write(
                        "distributeCallback(" + 
                        IsDocumentPromoted().ToString().ToLowerInvariant()   + 
                        ");cleanUp(\"" + 
                        Request.QueryString["cleanupid"] + 
                        "\");");
                    break;
            }
        }
        catch (Exception ex)
        {
            DisplayError(ex.Message); 
        }
    }

    private string GetSharepointUploadPath()
    {
        string szDavFolder = "ekdavroot";
        long contentId = -1;
        int contentLanguage = siteAPI.RequestInformationRef.DefaultContentLanguage;
        long folderId = long.Parse(Request.QueryString["folderid"]);

        // If we're at the root, return the site path
        string sitePath = siteAPI.SitePath.ToString().TrimEnd(new char[] { '/' })
                          .TrimStart(new char[] { '/' });
        if (sitePath != "")
            sitePath += "/";

        contentId = contentAPI.GetDestinationContentId(new Guid(Request.QueryString["listId"]), long.Parse(Request.QueryString["itemId"]));

        if(contentAPI.ContentLanguage != -1)
            contentLanguage = contentAPI.ContentLanguage;

        if (contentId != -1)
            folderId = contentAPI.GetFolderIdForContentId(contentId);
        // Otherwise, return the relative path
        szDavFolder = Context.Request.Url.Scheme + Uri.SchemeDelimiter + Page.Request.Url.Authority + "/" + sitePath +
                          szDavFolder + "_" + Request.QueryString["userid"] + "_" + Request.QueryString["uniqueid"] +
                         "_" + contentLanguage.ToString()  + "_-1_" + contentId.ToString() + "/";


        string szFolderPath = contentAPI.EkContentRef.GetFolderPath(folderId);
        szDavFolder =  szDavFolder + szFolderPath.Replace("\\", "/").TrimStart('/');
        return szDavFolder;
    }

    private bool IsDocumentPromoted()
    {
        return contentAPI.GetDestinationContentId(new Guid(Request.QueryString["listId"]), long.Parse(Request.QueryString["itemId"])) != -1;
    }

    private string GetRedistributionDetails()
    {
        long destinationContentID = contentAPI.GetDestinationContentId(
            new Guid(Request.QueryString["listId"]), 
            long.Parse(Request.QueryString["itemId"]));

        ContentData destinationContent = contentAPI.GetContentById(
            destinationContentID,
            ContentAPI.ContentResultType.Published);

        string destinationContentTitle = destinationContent.Title;
        string destinationFolderPath = contentAPI.EkContentRef.GetFolderPath(destinationContent.FolderId);

        StringBuilder sbDetails = new StringBuilder();
        sbDetails.Append("<ul><li>");
        sbDetails.Append("It was previously distributed to the following location:");
        sbDetails.Append("<ul><li>");
        sbDetails.Append(destinationFolderPath);
        if (destinationFolderPath.LastIndexOf("\\") != destinationFolderPath.Length - 1)
        {
            sbDetails.Append("\\");
        }
        sbDetails.Append(destinationContentTitle);
        sbDetails.Append("</li></ul></li></ul>");
        sbDetails.Append("<div id=\"DistributionWizardRelationshipInformation\">You cannot currently distribute this document to a different folder. If you want to do that, first go to ");
        sbDetails.Append(destinationFolderPath);
        sbDetails.Append(" and delete the document from there.</div>");

        return sbDetails.ToString();
    }

    /// <summary>
    /// Returns a string escaped for inclusion in javascript.
    /// </summary>
    /// <param name="str">String to be escaped</param>
    /// <returns>String escaped for inclusion in javascript</returns>
    private string EscapeString(string str)
    {
        return str.Replace("\\", "\\\\").Replace("\"", "\\\"");
    }
}
