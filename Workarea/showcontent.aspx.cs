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
using System.IO;
using Ektron.Cms.Common;
using Ektron.Cms.Content;

public partial class showcontent : System.Web.UI.Page
{
    protected ContentAPI m_refContentApi;
    protected EkContent m_refContent;
    protected int ContentLanguage = -1;
    protected long ContentId = 0;
    private void Page_Load(System.Object sender, System.EventArgs e)
    {
        try
        {
            m_refContentApi = new ContentAPI();

            if (ContentBlock.EkItem != null)
            {
                if (ContentBlock.EkItem.Id > 0)
                {

                    Page.Title = ContentBlock.EkItem.Title;

                    lblLastModifiedBy.Text = string.Format("{0} {1}", ContentBlock.EkItem.LastEditorFname, ContentBlock.EkItem.LastEditorLname);
                    lblLastModifiedBy.ToolTip = lblLastModifiedBy.Text;
                    lblLastModifiedDate.Text = ContentBlock.EkItem.DateModified.ToString("MMM-dd-yyyy");
                    lblLastModifiedDate.ToolTip = lblLastModifiedDate.Text;
                    if (((int)ContentBlock.EkItem.ContentType > EkConstants.ManagedAsset_Min && (int)ContentBlock.EkItem.ContentType < EkConstants.ManagedAsset_Max) || ((int)ContentBlock.EkItem.ContentType > EkConstants.Archive_ManagedAsset_Min && (int)ContentBlock.EkItem.ContentType < EkConstants.Archive_ManagedAsset_Max))
                    {
                        DownloadAssetLink.Text = "<a style=\"text-decoration:none;\" href=\"" + m_refContentApi.SitePath + "assetmanagement/downloadasset.aspx?id=" + ContentBlock.EkItem.AssetInfo.Id + "&LangType=" + ContentBlock.EkItem.Language + "&version=" + ContentBlock.EkItem.AssetInfo.Version + "\"><img border=\"0\" src=\"" + m_refContentApi.ApplicationPath + "images/application/download.gif\" /></a>";
                    }
                    else
                    {
                        DownloadAssetLink.Visible = false;
                    }
                    ContentList.ContentIds = ContentBlock.EkItem.Id.ToString();
                    if (!string.IsNullOrEmpty(ContentBlock.EkItem.AssetInfo.FileExtension))
                    {
                        contentBody.Visible = false;
                    }

                    ContentBlock.Text = ContentBlock.Text.Replace("<p>&nbsp;</p>", "");

                    contentBody.Attributes.Add("style", "border-top: solid 1px #dddddd; margin-bottom: 30px; padding-top: 10px;");
                }
                else
                {
                    MessageBoard.Visible = false;
                    SocialBar.Visible = false;
                    ContentDetails.Visible = false;
                    ContentList.Visible = false;
                    DownloadAssetLink.Visible = false;

                    ContentBlock.Text = "The requested content does not exist or has been deleted.";
                    ContentBlock.ToolTip = ContentBlock.Text;

                    contentBody.Attributes.Add("style", "margin-bottom: 30px; padding-top: 10px;");
                }
            }
        }
        catch (Exception ex)
        {
            throw (new Exception(ex.Message));
        }
    }
}