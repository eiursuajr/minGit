<%@ WebHandler Language="C#" Class="AdvancedTemplate" %>

using System;
using System.Web;
using Ektron.Newtonsoft.Json;
using Ektron.Cms;
using Ektron.Cms.Common;
using System.Text;
using Ektron.Cms.Content;
using Ektron.Cms.Site;


[JsonObject(MemberSerialization.OptIn)]
public class ResponseObject
{
    [JsonProperty("Taxonomy")]
    public string Taxonomy = "";
    [JsonProperty("NoTaxonomyString")]
    public string NoTaxonomyString = "";
    [JsonProperty("Metadata")]
    public string Metadata = "";
    [JsonProperty("TaxonomyRequired")]
    public bool TaxonomyRequired = false;
    [JsonProperty("PreSelectedTaxonomy")]
    public string PreSelectedTaxonomy = "";
}

public class AdvancedTemplate : IHttpHandler {
    ContentAPI capi;
    
    public void ProcessRequest (HttpContext context) {
        capi = new ContentAPI();
        ResponseObject rs = new ResponseObject();

        if (context.Request["folderid"] != null)
        {
            long folderid = 0;
            string[] requestdata = context.Request["folderid"].Split('|');
            if (long.TryParse(requestdata[0], out folderid))
            {
                if (folderid > 0)
                {
                    bool required = false;
                    bool exists = false;
                    rs.Taxonomy = GetTaxonomyString(folderid, out required, out exists);
                    rs.TaxonomyRequired = required;
                    rs.NoTaxonomyString = "";
                    if (!exists)
                    {
                        rs.NoTaxonomyString = GetNoTaxonomyString();
                        rs.TaxonomyRequired = false;
                    }
                    rs.Metadata = GetMetaDataString(folderid);
                }
            }
            long selectedtaxid = 0;
            if (long.TryParse(requestdata[1], out selectedtaxid))
            {
                rs.PreSelectedTaxonomy = GetPathToTaxId(selectedtaxid);
            }
            
        }
        context.Response.Write(JsonConvert.SerializeObject(rs));
        context.Response.End();
    }
    public string GetPathToTaxId(long tid)
    {
        int langid = (capi.RequestInformationRef.ContentLanguage == -1) ? capi.RequestInformationRef.DefaultContentLanguage : capi.RequestInformationRef.ContentLanguage;

        TaxonomyRequest taxrequest = new TaxonomyRequest();
        taxrequest.IncludeItems = false;
        taxrequest.TaxonomyLanguage = langid;
        taxrequest.TaxonomyId = tid;
        taxrequest.TaxonomyType = Ektron.Cms.Common.EkEnumeration.TaxonomyType.Content;
        TaxonomyData td = capi.EkContentRef.LoadTaxonomy(ref taxrequest);
        string path = tid.ToString();
        while (td != null && td.TaxonomyLevel != 0 && td.TaxonomyParentId != 0)
        {
            path = td.TaxonomyParentId.ToString() + "," + path;
            taxrequest.IncludeItems = false;
            taxrequest.TaxonomyLanguage = langid;
            taxrequest.TaxonomyId = td.TaxonomyParentId;
            taxrequest.TaxonomyType = Ektron.Cms.Common.EkEnumeration.TaxonomyType.Content;
            td = capi.EkContentRef.LoadTaxonomy(ref taxrequest);
        }
        return path;
    }
    
    public string GetMetaDataString(long folderid)
    {
        StringBuilder sb = new StringBuilder();
        
        EkContent ekContent = capi.EkContentRef;
        EkSite refEkSite = capi.EkSiteRef;
        ContentMetaData[] metaData = null;

        string ltrEnhancedMetadataArea = CustomFields.GetEnhancedMetadataArea();
        string ltrMetadataHTML = "";
        metaData = capi.GetMetaDataTypes("id");
        int validCounter = 0;

        StringBuilder sbMetadata = Ektron.Cms.CustomFields.WriteFilteredMetadataForEdit(
            metaData,
            false,
            "update",
            folderid,
            ref validCounter,
            refEkSite.GetPermissions(folderid, 0, "folder"));

        if (!String.IsNullOrEmpty(sbMetadata.ToString()))
        {
            ltrMetadataHTML = sbMetadata.ToString();
        }
        else
        {
            ltrMetadataHTML = "<span>There is no metadata associated with this folder.</span>";
        }
        sb.Append(ltrEnhancedMetadataArea);
        sb.Append(ltrMetadataHTML);
        return sb.ToString();
    }
    
    public string GetNoTaxonomyString()
    {
        EkMessageHelper m_refMsg = capi.EkMsgRef;
        return m_refMsg.GetMessage("generic no taxonomy");
    }
    
    public string GetTaxonomyString(long folderid, out bool required, out bool exists)
    {
        required = false;
        exists = false;
        StringBuilder sb = new StringBuilder();
        
        FolderData fd = capi.GetFolderById(folderid, true, false);
        if (fd != null)
        {
            required = fd.CategoryRequired;
			sb.Append("<div class=\"treecontainer\">");
            sb.Append("<ul class=\"EktronTaxonomyTree\">");

            string outputtext = "<li class=\"closed\"><span class=\"folder\" data-ektron-taxid=\"{0}\">";
            outputtext += "<input type=\"checkbox\" class=\"categoryCheck\">";
            outputtext += "{1}</span><ul data-ektron-taxid=\"{0}\"></ul></li>";

            foreach(TaxonomyBaseData t in fd.FolderTaxonomy){
                sb.Append(string.Format(outputtext, t.TaxonomyId, t.TaxonomyName));
            }
            sb.Append("</ul>");
			sb.Append("</div>");
            if (fd.FolderTaxonomy.Length > 0) exists = true;
        }
        return sb.ToString();
    }
 
    public bool IsReusable {
        get {
            return false;
        }
    }

}
