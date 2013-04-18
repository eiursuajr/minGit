<%@ WebHandler Language="C#" Class="RotatingBanner" %>

using System;
using System.Text;
using System.Collections;
using System.Configuration;
using System.Data;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Xml;
using Ektron;

public class RotatingBanner : IHttpHandler
{

    public void ProcessRequest(HttpContext context)
    {
        context.Response.ContentType = "text/xml";
        string CollID = context.Request.QueryString["CollID"];
        long CollectionID;
        int MaxResult = 0;
        string max_result = context.Request.QueryString["max_result"];
        if (!string.IsNullOrEmpty(CollID))
        {

            long.TryParse(CollID, out CollectionID);
            int.TryParse(max_result, out MaxResult);

            Ektron.Cms.UI.CommonUI.ApplicationAPI oAppApi = new Ektron.Cms.UI.CommonUI.ApplicationAPI();
            Ektron.Cms.UI.CommonUI.ApiSupport.CollectionItemsResult oResult;

            oResult = oAppApi.GetEcmCollectionItems(CollectionID, true, true);
            if (oResult.Item.Length <= 0)
            {
                oResult = oAppApi.GetEcmCollectionItems(CollectionID, true, false);

            }
            // Create ML response as desired in JS file
            StringBuilder xmlResponse = new StringBuilder(string.Empty);
            xmlResponse.Append("<?xml version=\"1.0\" encoding=\"utf-8\"?><Highlights>");
            for (int i = 0,j = 0; i < oResult.Item.Length; i++)
            {
                if (j < MaxResult && IsImage(oResult.Item[i].Html))
                {
                    xmlResponse.Append("<highlight>");
                    xmlResponse.Append("<h3><a><title>" + oResult.Item[i].Title + "</title></a></h3>");
                    xmlResponse.Append("<image>" + oResult.Item[i].Links + "</image>");
                    xmlResponse.Append("<link>" + oResult.Item[i].Links + "</link>");
                    xmlResponse.Append("<description>" + oResult.Item[i].Teaser + "</description>");
                    xmlResponse.Append("</highlight>");
                    j++;
                }
            }
            xmlResponse.Append("</Highlights>");


            context.Response.Clear();
            context.Response.Write(xmlResponse.ToString());
            context.Response.End();
        }
    }

    public bool IsReusable
    {
        get
        {
            return false;
        }
    }

    private bool IsImage(string html)
    {
        if (html.IndexOf("<img ") == 0)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

}