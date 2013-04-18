<%@ WebHandler Language="C#" Class="WidgetSpaceHandler" %>
using System;
using System.Web;
using Ektron.Cms;
using Ektron.Cms.Site;
using Ektron.Cms.Personalization;

public class WidgetSpaceHandler : IHttpHandler {
    protected Ektron.Cms.Common.EkMessageHelper m_refMsg;
    private SiteAPI _siteApi;
    
    public void ProcessRequest (HttpContext context) {
        context.Response.ContentType = "text/plain";
        string action;
        _siteApi = new SiteAPI();
        m_refMsg = _siteApi.EkMsgRef;
        action = context.Request.QueryString["action"].ToString();
        string ret = "";
        switch (action.ToLower())
        {
            case "add":
                ret = CheckAddWidgetSpaceName(context);
                break;
            case "edit":
                ret = CheckEditWidgetSpaceName(context);
                break;
        }
        context.Response.Write((string.IsNullOrEmpty(ret) ? "<success />" : ret));
    }

    private string CheckAddWidgetSpaceName(HttpContext context)
    {
        string ret = "";
        WidgetSpaceData widgetSpace = null;
        try
        {
            Ektron.Cms.Personalization.WidgetSpaceModel model = new Ektron.Cms.Personalization.WidgetSpaceModel();
            if ((context.Request.QueryString["name"] != null) && (context.Request.QueryString["name"].Length > 0))
            {
                widgetSpace = Ektron.Cms.Personalization.WidgetSpaceFactory.GetModel().GetWidgetSpaceByName(context.Request.QueryString["name"].ToString());
                if (widgetSpace.ID > 0)
                    ret = "<error>" + m_refMsg.GetMessage("widget space name already exists") + "</error>";
            }
        }
        catch (Exception ex)
        {
            ret = "<error>" + ex.Message + "</error>";
        }
        return ret;
    }

    private string CheckEditWidgetSpaceName(HttpContext context)
    {
        string ret = "";
        WidgetSpaceData widgetSpace = null;
        long id;
        try
        {
            Ektron.Cms.Personalization.WidgetSpaceModel model = new Ektron.Cms.Personalization.WidgetSpaceModel();
            if (context.Request.QueryString["name"] != null && context.Request.QueryString["name"].Length > 0 && context.Request.QueryString["id"] != null && Int64.TryParse(context.Request.QueryString["id"].ToString(), out id) && id > 0)
            {
                widgetSpace = Ektron.Cms.Personalization.WidgetSpaceFactory.GetModel().GetWidgetSpaceByName(context.Request.QueryString["name"].ToString(), id);
                if (widgetSpace.ID > 0)
                    ret = "<error>" + m_refMsg.GetMessage("widget space name already exists") + "</error>";
            }
        }
        catch (Exception ex)
        {
            ret = "<error>" + ex.Message + "</error>";
        }
        return ret;
    }
    
    public bool IsReusable {
        get {
            return false;
        }
    }
}