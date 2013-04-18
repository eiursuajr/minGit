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
        
        long taxid = 0;
        if (context.Request["taxid"] != null && long.TryParse(context.Request["taxid"], out taxid))
        {
            ContentAPI cApi = new ContentAPI();
            TaxonomyRequest taxrequest = new TaxonomyRequest();
            taxrequest.Depth = 1;
            taxrequest.IncludeItems = false;
            taxrequest.PageSize = 1000;
            taxrequest.TaxonomyId = taxid;
            taxrequest.TaxonomyItemType = Ektron.Cms.Common.EkEnumeration.TaxonomyItemType.Content;
            taxrequest.TaxonomyLanguage = cApi.RequestInformationRef.ContentLanguage;
            taxrequest.TaxonomyType = Ektron.Cms.Common.EkEnumeration.TaxonomyType.Content;
            taxrequest.ReadCount = false;
            Ektron.Cms.API.Content.Taxonomy tax = new Taxonomy();
            TaxonomyData t = tax.LoadTaxonomy(ref taxrequest);
			TaxonomyBaseData[] td = null;
            if (t != null)
            {
                td = t.Taxonomy;
            }
            System.Text.StringBuilder output = new System.Text.StringBuilder();
            if (td != null && td.Length > 0)
            {
                for (int i = 0; i < td.Length; i++)
                {
                    output.Append("<li class=\"closed");
                    if(i==td.Length-1){
                        output.Append(" last");
                    }
                    output.Append("\"><span class=\"folder\" data-ektron-taxid=\"");
                    output.Append(td[i].TaxonomyId.ToString());
                    output.Append("\">");
                    output.Append("<input type=\"checkbox\" class=\"categoryCheck\">");
                    output.Append(td[i].TaxonomyName);
                    output.Append("</span>");
                    if (td[i].TaxonomyHasChildren){
                        output.Append("<ul data-ektron-taxid=\"");
                        output.Append(td[i].TaxonomyId.ToString());
                        output.Append("\"></ul>");
                    }
                    output.Append("</li>");
                }
            }
            else
            {
                output.Append("No Children");
            }
            context.Response.Write(output.ToString());
        }
        context.Response.End();
    }
 
    public bool IsReusable {
        get {
            return false;
        }
    }

}
