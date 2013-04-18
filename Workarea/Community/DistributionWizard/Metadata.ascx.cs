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
using Ektron.Cms.Common;
using Ektron.Cms.Content;
using Ektron.Cms.Site;


public partial class Community_DistributionWizard_Metadata : System.Web.UI.UserControl
{
    private ContentAPI refContentAPI = null;
    private EkContent refEkContent = null;
    private EkSite refEkSite = null;
    private SiteAPI siteAPI = new SiteAPI();
    private ContentAPI.ContentResultType resultType = ContentAPI.ContentResultType.Published;
    private long contentID = 0;
    private long folderID = 0;
    private bool forceNewWindow = false;
    private bool metadataRequired = false;
    private string metaupdatestring = "update";

    public long ContentID
    {
        get { return contentID; }
        set { contentID = value; }
    }

    public long FolderID
    {
        get { return folderID; }
        set { folderID = value; }
    }

    public bool ForceNewWindow
    {
        get { return forceNewWindow; }
        set { forceNewWindow = value; }
    }
    public bool MetadataRequired
    {
        get { return metadataRequired; }
        set { metadataRequired = value; }
    }
    public Hashtable Metadata
    {
        get { return GetMetadata(); }
    }

    public ContentAPI.ContentResultType ResultType
    {
        get { return resultType; }
        set { resultType = value; }
    }

    public string MetaUpdateString
    {
        get { return metaupdatestring; }
        set { metaupdatestring = value; }

    }

    protected void Page_Load(object sender, EventArgs e)
    {
        refContentAPI = new ContentAPI();
        refEkContent = refContentAPI.EkContentRef;
        refEkSite = refContentAPI.EkSiteRef;
        ContentMetaData[] metaData = null;
        Ektron.Cms.ContentData originalContent = null;

        ltrEnhancedMetadataArea.Text = CustomFields.GetEnhancedMetadataArea();
        if (ContentID != 0)
        {
            originalContent = refContentAPI.GetContentById(this.ContentID, ResultType);
            if (resultType == ContentAPI.ContentResultType.Staged && originalContent == null)
            {
                originalContent = refContentAPI.GetContentById(this.ContentID, ContentAPI.ContentResultType.Published);
            }
            if (originalContent.FolderId == FolderID)
            {
                metaData = originalContent.MetaData;
            }
        }
        if(metaData == null){
            metaData = refContentAPI.GetMetaDataTypes("id");
        }

        int validCounter = 0;

        if (originalContent != null || ContentID == 0)
        {
			foreach (ContentMetaData cMeta in metaData)
            {
                if (cMeta.Type.ToString() == "ImageSelector")
                {
                    cMeta.Text = cMeta.Text.Replace(SitePath + "assets/", "");
                    cMeta.Text = System.Text.RegularExpressions.Regex.Replace(cMeta.Text, "\\?.*", "");
                }
            }
			
            StringBuilder sbMetadata = Ektron.Cms.CustomFields.WriteFilteredMetadataForEdit(
                metaData,
                false,
                MetaUpdateString,
                this.FolderID,
                ref validCounter,
                refEkSite.GetPermissions(this.FolderID, 0, "folder"));

            if (!String.IsNullOrEmpty(sbMetadata.ToString()))
            {
                ltrMetadataHTML.Text = sbMetadata.ToString();
                if (sbMetadata.ToString().Contains("<span style=\"color:red\">"))
                   MetadataRequired =  metadataRequired = true;
            }
            else
            {
                ltrMetadataHTML.Text = "<span>"+siteAPI.EkMsgRef.GetMessage("lbl no metadata associated")+"</span>";
            }
        }

        Ektron.Cms.API.JS.RegisterJSInclude(this, refContentAPI.AppPath + "java/jfunct.js", "CommunityJFunctJS");
        Ektron.Cms.API.JS.RegisterJSInclude(this, refContentAPI.AppPath + "java/metadata_selectlist.js", "CommunityMetadataSelectListJS");
        Ektron.Cms.API.JS.RegisterJSInclude(this, refContentAPI.AppPath + "java/searchfuncsupport.js", "CommunitySearchFuncSupportJS");
        Ektron.Cms.API.JS.RegisterJSInclude(this, refContentAPI.AppPath + "java/internCalendarDisplayFuncs.js", "CommunityInternCalendarDisplayFuncsJS");
        Ektron.Cms.API.JS.RegisterJSInclude(this, refContentAPI.AppPath + "java/metadata_associations.js", "CommunityMetadataAssociationsJS");
        Ektron.Cms.API.JS.RegisterJSInclude(this, refContentAPI.AppPath + "java/optiontransfer.js", "CommunityOptionTransferJS");

        Ektron.Cms.API.JS.RegisterJSBlock(this, "window.ek_ma_ForceNewWindow = " + ForceNewWindow.ToString().ToLower(), "ek_ma_ForceNewWindow");
    }

    private Hashtable GetMetadata()
    {
        object[] acMetaInfo = new object[3];
        string metaSelect = null;
        string metaSeparator = null;
        int validCounter = 0;

        string metaTextString = string.Empty;

        if (!String.IsNullOrEmpty(Request.Form["frm_validcounter"])) 
        {
            int.TryParse(Request.Form["frm_validcounter"], out validCounter);
        }

        Hashtable pageMetaData = new Hashtable();

        int fieldIndex = 0;
        for(fieldIndex = 1; fieldIndex <= validCounter; fieldIndex++)
        {
            acMetaInfo[0] = Request.Form["frm_meta_type_id_" + fieldIndex.ToString()];
            acMetaInfo[1] = Request.Form["content_id"];
            metaSeparator = Request.Form["MetaSeparator_" + fieldIndex.ToString()];
            metaSelect = Request.Form["MetaSelect_" + fieldIndex.ToString()];
            if (Convert.ToInt32(metaSelect) != 0)
            {
                metaTextString = metaTextString.Replace(", ", metaSeparator);
                if(metaTextString.StartsWith(metaSeparator))
                {
                    metaTextString = metaTextString.Substring(1);
                }

                acMetaInfo[2] = metaTextString;
            }
            else
            {
                string myMeta = string.Empty;
                myMeta = Request.Form["frm_text_" + fieldIndex.ToString()];
                myMeta = Server.HtmlDecode(myMeta);
                metaTextString = myMeta.Replace(";", metaSeparator);
                myMeta = EkFunctions.HtmlEncode(metaTextString);
                acMetaInfo[2] = metaTextString;
            }
            
            pageMetaData.Add(fieldIndex,acMetaInfo);
            acMetaInfo = new object[3];
        }

        if (!String.IsNullOrEmpty(Request.Form["isblogpost"]))
        {
            fieldIndex++;
            acMetaInfo[0] = Request.Form["blogposttagsid"];
            acMetaInfo[1] = Request.Form["content_id"];
            metaSeparator = ";";
            acMetaInfo[2] = Request.Form["blogposttags"];
            pageMetaData.Add(fieldIndex, acMetaInfo);
            
            acMetaInfo = new object[3];

            fieldIndex++;
            acMetaInfo[0] = Request.Form["blogpostcatid"];
            acMetaInfo[1] = Request.Form["content_id"];
            metaSeparator = ";";

            if ( !String.IsNullOrEmpty(Request.Form["blogpostcatlen"]))
            {
                int blogPostCatLength = 0;
                int.TryParse(Request.Form["blogpostcatlen"], out blogPostCatLength);

                metaTextString = string.Empty;
                for(int y = 0; y <= blogPostCatLength; y++)
                {
                    if(!String.IsNullOrEmpty(Request.Form["blogcategories" + y.ToString()]))
                    {
                        metaTextString += Request.Form["blogcategories" + y.ToString()].Replace(";", "~@~@~") + ";";
                    }
                }
                if( metaTextString.EndsWith(";"))
                {
                    metaTextString = metaTextString.Substring(0, metaTextString.Length - 1);
                }

                acMetaInfo[2] = metaTextString;
            }
            else
            {
                acMetaInfo[2] = string.Empty;
            }

            pageMetaData.Add(fieldIndex.ToString(), acMetaInfo);
            
            acMetaInfo = new object[3];

            fieldIndex++;

            acMetaInfo[0] = Request.Form["blogposttrackbackid"];
            acMetaInfo[1] = Request.Form["content_id"];
            metaSeparator = ";";
            acMetaInfo[2] = Request.Form["trackback"];
            pageMetaData.Add(fieldIndex, acMetaInfo);
            
            acMetaInfo = new object[3];

            fieldIndex++;

            acMetaInfo[0] = Request.Form["blogpostchkpingbackid"];
            acMetaInfo[1] = Request.Form["content_id"];
            metaSeparator = ";";
            
            if (!String.IsNullOrEmpty(Request.Form["chkpingback"]))
            {
                acMetaInfo[2] = 1;
            }
            else
            {
                acMetaInfo[2] = 0;
            }

            pageMetaData.Add(fieldIndex, acMetaInfo);
        }

        return pageMetaData;
    }

    public string SitePath
    {
        get
        {
            return siteAPI.SitePath;
        }
    }
}
