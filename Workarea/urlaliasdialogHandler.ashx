<%@ WebHandler Language="C#" Class="urlaliasdialogHandler" %>

using System;
using System.Web;
using System.Data;
using Ektron.Cms;
using Ektron.Cms.Common;
using Ektron.Cms.Site;
using System.Collections.Generic;
using System.Xml;
using System.Net.NetworkInformation;

public class urlaliasdialogHandler : IHttpHandler
{

    public void ProcessRequest(HttpContext context)
    {
        context.Response.ContentType = "text/plain";
        string action = "";
        
        if (context.Request.QueryString["action"] != null)
        {
            action = context.Request.QueryString["action"].ToString();
        }
        //else
        //{
        //    action = context.Request.Form["action"].ToString();
        //}
        switch (action.ToLower())
        {
            case "getaliaslist":
                context.Response.Write(GetAliasList(context));
                break;
            case "checkaliasname":
                context.Response.Write(CheckAliasName(context));
                break;
        }
    }

    public bool IsReusable
    {
        get
        {
            return false;
        }
    }

    private string GetAliasList(HttpContext context)
    {
        string ret = "";
        long contentId = 0;
        int i = 0;


        Int64.TryParse(context.Request.QueryString["contId"], out  contentId);


        PagingInfo req = new PagingInfo();
        CommonApi _refCommonApi= new CommonApi();
        ContentAPI _refContentApi = new ContentAPI(); 
        Ektron.Cms.UrlAliasing.UrlAliasSettingsApi settingsAliasApi = new Ektron.Cms.UrlAliasing.UrlAliasSettingsApi();  
        Ektron.Cms.UrlAliasing.UrlAliasManualApi manualAliasApi = new Ektron.Cms.UrlAliasing.UrlAliasManualApi();
        Ektron.Cms.UrlAliasing.UrlAliasAutoApi autoAliasApi = new Ektron.Cms.UrlAliasing.UrlAliasAutoApi();
        System.Collections.Generic.List<UrlAliasAutoData> autoAliasList;
        System.Collections.Generic.List<UrlAliasManualData> manualAliasList;

        autoAliasList = autoAliasApi.GetListForContent(contentId);
        manualAliasList = manualAliasApi.GetList(req, contentId, true, EkEnumeration.UrlAliasingOrderBy.None);
        try
        {
            if (settingsAliasApi.IsManualAliasEnabled || settingsAliasApi.IsAutoAliasEnabled)
            {
                if (autoAliasList.Count + manualAliasList.Count > 1)
                {
                        if (manualAliasList.Count != 0)
                            {
                                if (manualAliasList[0].HostName != String.Empty)
                                {
                                    ret += "<input type=\"radio\" name=\"aliasSelect\" id=\"aliasSelect\" value=\"" + "http://" + manualAliasList[0].HostName +"/" + manualAliasList[0].DisplayAlias + "\"checked>http://" + manualAliasList[0].HostName + "/" + manualAliasList[0].DisplayAlias + "</input>" + " " + "(" + _refCommonApi.EkMsgRef.GetMessage("lbl tree url manual aliasing") + " " + _refCommonApi.EkMsgRef.GetMessage("lbl alias") + ")";
                                }
                                else
                                {
                                    ret += "<input type=\"radio\" name=\"aliasSelect\" id=\"aliasSelect\" value=\"" + _refContentApi.SitePath + manualAliasList[0].DisplayAlias + "\"checked>" + _refContentApi.SitePath + manualAliasList[0].DisplayAlias + "</input>" + " " + "(" + _refCommonApi.EkMsgRef.GetMessage("lbl tree url manual aliasing") + " " + _refCommonApi.EkMsgRef.GetMessage("lbl alias") + ")";
                                }
                                ret += "<br/>"; 
                            }
                        for (i = 1; i <= manualAliasList.Count - 1; i++)
                            {
                                if (manualAliasList[i].HostName != String.Empty)
                                {
                                    ret += "<input type=\"radio\" name=\"aliasSelect\" id=\"aliasSelect\" value=\"" +"http://"+ manualAliasList[i].HostName +"/" + manualAliasList[i].DisplayAlias + "\">http://" + manualAliasList[i].HostName + "/" + manualAliasList[i].DisplayAlias + "</input>" + " " + "(" + _refCommonApi.EkMsgRef.GetMessage("lbl tree url manual aliasing") + " " + _refCommonApi.EkMsgRef.GetMessage("lbl alias") + ")";
                                }
                                else
                                {
                                    ret += "<input type=\"radio\" name=\"aliasSelect\" id=\"aliasSelect\" value=\"" + _refContentApi.SitePath + manualAliasList[i].DisplayAlias + "\">" + _refContentApi.SitePath + manualAliasList[i].DisplayAlias + "</input>" + " " + "(" + _refCommonApi.EkMsgRef.GetMessage("lbl tree url manual aliasing") + " " + _refCommonApi.EkMsgRef.GetMessage("lbl alias") + ")";
                                }
                                ret += "<br/>";
                            }


                            if (manualAliasList.Count == 0 && autoAliasList.Count != 0)
                            {

                                if (autoAliasList[0].HostName != String.Empty )
                                {
                                    ret += "<input type=\"radio\" name=\"aliasSelect\" id=\"aliasSelect\" value=\"" + "http://" + autoAliasList[0].HostName + "/" + autoAliasList[0].AliasName + "\"checked>http://" + autoAliasList[0].HostName + "/" + autoAliasList[0].AliasName + "</input>" + " " + "(" + _refCommonApi.EkMsgRef.GetMessage("lbl tree url automatic aliasing") + " " + _refCommonApi.EkMsgRef.GetMessage("lbl alias") + ")";
                                }
                                else
                                {
                                    ret += "<input type=\"radio\" name=\"aliasSelect\" id=\"aliasSelect\" value=\"" + _refContentApi.SitePath + autoAliasList[0].AliasName + "\"checked>" + _refContentApi.SitePath + autoAliasList[0].AliasName + "</input>" + " " + "(" + _refCommonApi.EkMsgRef.GetMessage("lbl tree url automatic aliasing") + " " + _refCommonApi.EkMsgRef.GetMessage("lbl alias") + ")";
                                }
                                ret += "<br/>";
                            }
                            else if(autoAliasList.Count != 0)
                            {
                                if (autoAliasList[0].HostName != String.Empty )
                                {
                                    ret += "<input type=\"radio\" name=\"aliasSelect\" id=\"aliasSelect\" value=\"" + "http://" + autoAliasList[0].HostName + "/" + autoAliasList[0].AliasName + "\">http://" + autoAliasList[0].HostName + "/" + autoAliasList[0].AliasName + "</input>" + " " + "(" + _refCommonApi.EkMsgRef.GetMessage("lbl tree url automatic aliasing") + " " + _refCommonApi.EkMsgRef.GetMessage("lbl alias") + ")";
                                }
                                else
                                {
                                    ret += "<input type=\"radio\" name=\"aliasSelect\" id=\"aliasSelect\" value=\"" + _refContentApi.SitePath + autoAliasList[0].AliasName + "\">" + _refContentApi.SitePath + autoAliasList[0].AliasName + "</input>" + " " + "(" + _refCommonApi.EkMsgRef.GetMessage("lbl tree url automatic aliasing") + " " + _refCommonApi.EkMsgRef.GetMessage("lbl alias") + ")";
                                }
                                ret += "<br/>";
                            }
                            for (i = 1; i <= autoAliasList.Count - 1; i++)
                            {
                                if (autoAliasList[i].HostName != String.Empty )
                                {
                                    ret += "<input type=\"radio\" name=\"aliasSelect\" id=\"aliasSelect\" value=\"" + "http://" + autoAliasList[i].HostName + "/" + autoAliasList[i].AliasName + "\">http://" + autoAliasList[i].HostName + "/" + autoAliasList[i].AliasName + "</input>" + " " + "(" + _refCommonApi.EkMsgRef.GetMessage("lbl tree url automatic aliasing") + " " + _refCommonApi.EkMsgRef.GetMessage("lbl alias") + ")";
                                }
                                else
                                {
                                    ret += "<input type=\"radio\" name=\"aliasSelect\" id=\"aliasSelect\" value=\"" + _refContentApi.SitePath + autoAliasList[i].AliasName + "\">" + _refContentApi.SitePath + autoAliasList[i].AliasName + "</input>" + " " + "(" + _refCommonApi.EkMsgRef.GetMessage("lbl tree url automatic aliasing") + " " + _refCommonApi.EkMsgRef.GetMessage("lbl alias") + ")";
                                }
                                ret += "<br/>";
                            }
                            if (_refContentApi.RequestInformationRef.LinkManagement)
                            {
                                ret += "<input type=\"radio\" name=\"aliasSelect\" id=\"aliasSelect\" value=\"" + _refContentApi.AppPath + "linkit.aspx?LinkIdentifier=id&ItemID=" + contentId + "\">" + _refContentApi.AppPath + "linkit.aspx?LinkIdentifier=id&ItemID=" + contentId + "</input>";
                                
                            }
                }

                else if (autoAliasList.Count + manualAliasList.Count == 1)
                {
                    if (manualAliasList.Count != 0)
                    {
                        if (manualAliasList[i].HostName != String.Empty )
                        {
                            ret += "<input type=\"radio\" name=\"aliasSelect\" id=\"aliasSelect\" value=\"" + "http://" + manualAliasList[i].HostName + "/" + manualAliasList[0].DisplayAlias + "\"checked>http://" + manualAliasList[i].HostName + "/" + manualAliasList[0].DisplayAlias + "</input>" + " " + "(" + _refCommonApi.EkMsgRef.GetMessage("lbl tree url manual aliasing") + " " + _refCommonApi.EkMsgRef.GetMessage("lbl alias") + ")";
                        }
                        else
                        {
                            ret += "<input type=\"radio\" name=\"aliasSelect\" id=\"aliasSelect\" value=\"" + _refContentApi.SitePath + manualAliasList[i].DisplayAlias + "\"checked>" + _refContentApi.SitePath + manualAliasList[i].DisplayAlias + "</input>" + " " + "(" + _refCommonApi.EkMsgRef.GetMessage("lbl tree url manual aliasing") + " " + _refCommonApi.EkMsgRef.GetMessage("lbl alias") + ")";   
                        }
                        
                        ret += "<br/>";
                    }
                    if (autoAliasList.Count != 0)
                    {
                        if (autoAliasList[i].HostName != String.Empty && autoAliasList[i].AutoAliasType != EkEnumeration.AutoAliasType.Taxonomy)
                        {
                            ret += "<input type=\"radio\" name=\"aliasSelect\" id=\"aliasSelect\"  value=\"" + "http://" + autoAliasList[i].HostName + "/" + autoAliasList[i].AliasName + "\" checked>http://" + autoAliasList[i].HostName + "/" + autoAliasList[i].AliasName + "</input>" + " " + "(" + _refCommonApi.EkMsgRef.GetMessage("lbl tree url automatic aliasing") + " " + _refCommonApi.EkMsgRef.GetMessage("lbl alias") + ")";
                        }
                        else
                        {
                            ret += "<input type=\"radio\" name=\"aliasSelect\" id=\"aliasSelect\"  value=\"" + _refContentApi.SitePath + autoAliasList[i].AliasName + "\" checked>" + _refContentApi.SitePath + autoAliasList[i].AliasName + "</input>" + " " + "(" + _refCommonApi.EkMsgRef.GetMessage("lbl tree url automatic aliasing") + " " + _refCommonApi.EkMsgRef.GetMessage("lbl alias") + ")";
                        }
                        ret += "<br/>";
                    }
                    if (_refContentApi.RequestInformationRef.LinkManagement)
                    {
                        ret += "<input type=\"radio\" name=\"aliasSelect\" id=\"aliasSelect\" value=\"" + _refContentApi.AppPath + "linkit.aspx?LinkIdentifier=id&ItemID=" + contentId + "\">" + _refContentApi.AppPath + "linkit.aspx?LinkIdentifier=id&ItemID=" + contentId + "</input>";
                        ret += "<linkmanage>" + "</linkmanage>";
                    }
                    ret += "<aliascount>" + "</aliascount>";
                }

                else
                {
                    ret = "<error>" + "</error>";

                }

            }
            else
            {
                ret = "<error>" + "</error>";
            }
        }
        catch(Exception ex)
        {
            ret = "<error>" + ex.Message + "</error>";
        }
        return ret;
    }
    private string CheckAliasName(HttpContext context)
    {
        string ret = "";
        Ektron.Cms.UrlAliasing.UrlAliasManualApi manualAliasApi = new Ektron.Cms.UrlAliasing.UrlAliasManualApi();
        string result = "";
        string aliasName = "";
        string fileExtension = string.Empty;
        string langType = string.Empty;
        string id = string.Empty;
        long folderId = 0;
         
        aliasName = context.Request.QueryString["aliasname"];
        fileExtension = context.Request.QueryString["fileextension"];
        langType = context.Request.QueryString["langtype"];
        id = context.Request.QueryString["folderid"];
        Int64.TryParse(id,out folderId);
        result = manualAliasApi.ValidateAliasName(folderId, aliasName, fileExtension, Convert.ToInt32(langType));

        if (result == "")
        {
            ret = "<aliasname><aliasname>";
        }
        else
        {
            ret = result;
        }

        return ret;
    }
                 

}