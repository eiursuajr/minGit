<%@ WebHandler Language="C#" Class="MoveCopyTaxonomyHandler" %>

using System;
using System.Web;
using Ektron.Cms;
using Ektron.Cms.Content;
using Ektron.Cms.Common;

public class MoveCopyTaxonomyHandler : IHttpHandler {
    
    public void ProcessRequest (HttpContext context) 
    {
        context.Response.ContentType = "text/plain";

        long sourceId = 0;
        long destinationId = 0;
        int languageId = 0;
        bool deleteSource = false;
        EkContent m_refContent;
        ContentAPI m_refContentApi = new ContentAPI();
        long newTaxId = 0;
        string moveOrCopy = "";

        m_refContent = m_refContentApi.EkContentRef;
        

        if (!String.IsNullOrEmpty(context.Request.QueryString["SourceId"]))
        {
            sourceId = long.Parse(context.Request.QueryString["SourceId"]);
        }
        if (!String.IsNullOrEmpty(context.Request.QueryString["destinationId"]))
        {
            destinationId = long.Parse(context.Request.QueryString["destinationId"]);
        }
        if (!String.IsNullOrEmpty(context.Request.QueryString["LangType"]))
        {
            languageId = System.Convert.ToInt32(context.Request.QueryString["LangType"]);
        }
        if (!String.IsNullOrEmpty(context.Request.QueryString["action"]))
        {
            moveOrCopy = context.Request.QueryString["action"].ToString();
            if (moveOrCopy.ToLower() == "copy")
            {
                deleteSource = false; 
            }
            else
            {
                deleteSource = true;
            }
        }
        try
        {
            if (deleteSource == true)
            {
                m_refContent.MoveTaxonomy(sourceId, destinationId, true);
                newTaxId = sourceId;
            }
            else
            {
                newTaxId = m_refContent.CloneTaxonomy(sourceId, destinationId, languageId, -1, true, deleteSource);
            }
        }
        catch (Exception ex)
        {
            context.Response.ContentType = "text/plain";
            context.Response.Write("error: " + EkFunctions.UrlEncode(ex.Message));
        }
    }
 
    public bool IsReusable {
        get {
            return false;
        }
    }

}