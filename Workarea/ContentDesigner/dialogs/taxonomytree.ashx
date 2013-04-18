<%@ WebHandler Language="C#" Class="foldertree" %>

using System;
using System.Web;
using System.IO;
using System.Web.UI;
using Ektron.Newtonsoft.Json;
using System.Collections.Generic;
using System.Text.RegularExpressions;

using Ektron.Cms;
using Ektron.Cms.API.Content;

[JsonObject(MemberSerialization.OptIn)]
public class DirectoryInfo
{
    [JsonProperty("subdirectories")]
    public List<TaxData> SubDirectories = new List<TaxData>();
}

[JsonObject(MemberSerialization.OptIn)]
public class TaxData
{
    [JsonProperty("name")]
    public string Name = "";

    [JsonProperty("tid")]
    public long Tid = 0;

    [JsonProperty("haschildren")]
    public bool HasChildren = false;    
}


public class foldertree : IHttpHandler {
    public void ProcessRequest (HttpContext context)
    {
        context.Response.ContentType = "text/plain";
        context.Response.Buffer = false;
        
        DirectoryInfo directoryInfo = new DirectoryInfo();
        
        long taxid = 0;
        if (context.Request["resid"] != null && long.TryParse(context.Request["resid"], out taxid))
        {

            Taxonomy tax = new Taxonomy();
            TaxonomyRequest taxrequest = new TaxonomyRequest();
            taxrequest.Depth = 2;
            taxrequest.IncludeItems = false;
            taxrequest.PageSize = 1000;
            taxrequest.TaxonomyId = taxid;
            taxrequest.TaxonomyItemType = Ektron.Cms.Common.EkEnumeration.TaxonomyItemType.Content;
            taxrequest.TaxonomyLanguage = tax.RequestInformationRef.ContentLanguage;
            taxrequest.TaxonomyType = Ektron.Cms.Common.EkEnumeration.TaxonomyType.Content;
            TaxonomyData taxdata = tax.LoadTaxonomy(ref taxrequest);

            if (taxdata != null && taxdata.TaxonomyHasChildren)
            {
                foreach (TaxonomyData td in taxdata.Taxonomy)
                {
                    TaxData mytd = new TaxData();
                    mytd.Name = td.TaxonomyName;
                    mytd.Tid = td.TaxonomyId;
                    mytd.HasChildren = td.TaxonomyHasChildren;
                    directoryInfo.SubDirectories.Add(mytd);
                }
            }

            context.Response.Write(JsonConvert.SerializeObject(directoryInfo));
        }
        context.Response.End();
    }
 
    public bool IsReusable {
        get {
            return false;
        }
    }

}