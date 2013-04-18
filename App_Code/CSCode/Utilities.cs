using System.Web.UI.WebControls;
using System.Configuration;
using System.Collections;
using System.Linq;
using System.Data;
using System.Web.Caching;
using System.Web;
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
using Ektron.Cms.Site;
using Ektron.ASM.PluginManager;
using Ektron.Cms.Common;
using Microsoft.VisualBasic.CompilerServices;
using Ektron.Cms.API;

public class Utilities
{
    public static string UniqueIdFieldName = "ekUserUniqueId";
    public static string SessionIdClassName = "ektronsessionidfield";

    public static void ProcessThumbnail(string SrcPath, string SrcFile)
    {
        ProcessThumbnail(SrcPath, SrcFile, 125, 125, 0);
    }
    public static void ProcessEcommThumbnail(string SrcPath, string SrcFile)
    {
        ProcessThumbnail(SrcPath, SrcFile, 125, 125, 0, "ecomm");
    }
    public static void RegisterBaseUrl(System.Web.UI.Page page)
    {
        ContentAPI api = new ContentAPI();
        Literal baseTag = new Literal();
        baseTag.Text = "<base href=\"" + HttpContext.Current.Request.Url.Scheme + "://" + HttpContext.Current.Request.Url.Host + ((Convert.ToInt32(HttpContext.Current.Request.ServerVariables["SERVER_PORT"]) == 80) ? "" : (":" + HttpContext.Current.Request.ServerVariables["SERVER_PORT"])) + api.RequestInformationRef.SitePath + "\" />";
        page.Header.Controls.Add(baseTag);
    }
    public static bool ValidateUserLogin()
    {
        return ValidateUserLogin(true);
    }
    public static void AddSessionIdField(Control control)
    {
        if (control != null)
        {
            control.Controls.Add(GetSessionIdField());
        }
    }
    public static Control GetSessionIdField()
    {
        string sessionId = System.Web.HttpContext.Current.Session.SessionID;
        LiteralControl sessionIdField = new LiteralControl(
            string.Format(@"<input type=""hidden"" name=""{0}"" id=""{0}"" class=""{1}"" value=""{2}"" />",
                Guid.NewGuid().ToString(), 
                SessionIdClassName,
                sessionId
                )
            );
        return sessionIdField;
    }
    public static Control GetUniqueIdField(int uniqueId)
    {
        LiteralControl uniqueIdField = new LiteralControl(
            string.Format(@"<input type=""hidden"" name=""{0}"" id=""{0}"" value=""{1}"" />", Utilities.UniqueIdFieldName, uniqueId)
            );
        return uniqueIdField;
    }
    public static bool IsInternalPostback
    {
        get {
            if (!(System.Web.HttpContext.Current.Handler as Page).IsPostBack)
                return false;
            ContentAPI cApi = new ContentAPI();
            bool isInternalPostback =
                System.Web.HttpContext.Current.Request.Form.Count > 0 &&
                System.Web.HttpContext.Current.Request.Form[UniqueIdFieldName] != null &&
                System.Web.HttpContext.Current.Request.Form[UniqueIdFieldName] == cApi.RequestInformationRef.UniqueId.ToString() && 
                (
                !(null == System.Web.HttpContext.Current.Request.UrlReferrer ||
                System.Web.HttpContext.Current.Request.Url.Authority != System.Web.HttpContext.Current.Request.UrlReferrer.Authority)
                );
            return isInternalPostback;
        }
    }
    public static bool ValidateUserLogin(bool redirect)
    {
        ContentAPI cApi = new ContentAPI();
        if ((cApi.EkContentRef).IsAllowed(0, 0, "users", "IsLoggedIn", cApi.UserId) == false)
        {
            if (redirect)
                System.Web.HttpContext.Current.Response.Redirect(cApi.AppPath + "login.aspx?fromLnkPg=1", false);
            return false;
        }
        return true;
    }
    public static bool ValidateUserRole(EkEnumeration.CmsRoleIds roleId)
    {
        return ValidateUserRole(roleId, true);
    }
    public static bool ValidateUserRole(EkEnumeration.CmsRoleIds roleId, bool redirect)
    {
        ContentAPI cApi = new ContentAPI();
        if (!cApi.IsARoleMember(roleId))
        {
            if (redirect)
                System.Web.HttpContext.Current.Response.Redirect(cApi.AppPath + "login.aspx?fromLnkPg=1", false);
            return false;
        }
        return true;
    }
    public static void ProcessThumbnail(string SrcPath, string SrcFile, int Width, int height, int ThumbSize)
    {
        ProcessThumbnail(SrcPath, SrcFile, Width, height, ThumbSize, "thumb");
    }
    public static void ProcessThumbnail(string SrcPath, string SrcFile, int Width, int height, int ThumbSize, string prefix)
    {
        string strSrcLoc = "";
        string strDesLoc = "";
        string strExtn = "png";
        bool result = false;
        try
        {
            strSrcLoc = SrcPath + "\\" + SrcFile;
            strExtn = SrcFile.Substring(SrcFile.Length - 3, 3);
            strExtn = strExtn.ToLower();
            if ("gif" == strExtn)
            {
                strExtn = "png";
                if (ThumbSize == 0)
                {
                    strDesLoc = SrcPath + "\\thumb_" + SrcFile.Substring(0, SrcFile.Length - 3) + strExtn;
                }
                else
                {
                    strDesLoc = SrcPath + "\\thumb" + ThumbSize + "_" + SrcFile.Substring(0, SrcFile.Length - 3) + strExtn;
                }
            }
            else
            {
                if (ThumbSize == 0)
                {
                    strDesLoc = SrcPath + "\\thumb_" + SrcFile;
                }
                else
                {
                    strDesLoc = SrcPath + "\\thumb" + ThumbSize + "_" + SrcFile;
                }
            }
            EkFileIO obj = new EkFileIO();
            result = obj.CreateThumbnail(strSrcLoc, strDesLoc, Width, height);
            if (result == false)
            {
                throw (new Exception("<p style=\'background-color:red\'>ERROR Initializing:  </p>"));
            }
        }
        catch (Exception ex)
        {
            throw (new Exception(ex.Message));
        }
    }

    /// --------------------------------------------------------------------------------
    /// <summary>
    /// Prepends Path to URL if URL is relative, otherwise returns URL as is.
    /// </summary>
    /// <param name="Path">
    /// 	Path to prepend to URL unless URL is already qualified, that is, not a relative path.
    /// 	Value Type: <see cref="String" /> (System.String)
    /// </param>
    /// <param name="URL">
    /// 	The URL to be qualified if it's not already.
    /// 	Value Type: <see cref="String" /> (System.String)
    /// </param>
    /// <returns>A qualified URL.	<see cref="String" /> (System.String)</returns>
    /// <remarks>
    /// </remarks>
    /// --------------------------------------------------------------------------------
    public static string QualifyURL(string Path, string URL)
    {
        if ((Path == null) || string.Empty == Path)
        {
            return URL;
        }
        if ((URL == null) || string.Empty == URL)
        {
            return Path;
        }
        string strDelimiter;
        if (Path.IndexOf("/") >= 0 || URL.IndexOf("/") >= 0)
        {
            strDelimiter = "/";
            Path = Path.Replace("\\", strDelimiter);
            URL = URL.Replace("\\", strDelimiter);
        }
        else
        {
            strDelimiter = "\\";
            Path = Path.Replace("/", strDelimiter);
            URL = URL.Replace("/", strDelimiter);
        }
        if (URL.StartsWith(strDelimiter + strDelimiter) || URL.IndexOf(":") >= 0)
        {
            return URL;
        }
        else if (Path.EndsWith(strDelimiter) || URL.StartsWith(strDelimiter))
        {
            if (Path.EndsWith(strDelimiter) && URL.StartsWith(strDelimiter))
            {
                return Path + URL.Substring(strDelimiter.Length); // remove extra delimiter
            }
            else
            {
                return Path + URL;
            }
        }
        else
        {
            return Path + strDelimiter + URL;
        }
    }

    // calling FixId in EkFunctions
    public static string FixId(string Id)
    {
        return Ektron.Cms.Common.EkFunctions.FixId(Id);
    }

    public static void ShowError(string Message)
    {
        //http://support.microsoft.com/kb/q208427/
        //INFO: Maximum URL Length Is 2,083 Characters in Internet Explorer
        //View products that this article applies to.
        //Article ID : 208427
        //        Last(Review) : May(12, 2003)
        //Revision : 2.0
        //This article was previously published under Q208427
        //SUMMARY
        //Internet Explorer has a maximum uniform resource locator (URL) length of 2,083 characters, with a maximum path length of 2,048 characters. This limit applies to both POST and GET request URLs.
        //If you are using the GET method, you are limited to a maximum of 2,048 characters (minus the number of characters in the actual path, of course).
        //POST, however, is not limited by the size of the URL for submitting name/value pairs, because they are transferred in the header and not the URL.
        //RFC 2616, Hypertext Transfer Protocol -- HTTP/1.1 (ftp://ftp.isi.edu/in-notes/rfc2616.txt), does not specify any requirement for URL length.
        string strURL;
        int nDiff;
        SiteAPI m_refSiteAPI = new SiteAPI();
        const int MAX_URL_LENGTH = 2048;
        do
        {
            strURL = m_refSiteAPI.AppPath + "reterror.aspx?info=" + EkFunctions.UrlEncode(Message);
            nDiff = System.Convert.ToInt32(MAX_URL_LENGTH - strURL.Length);
            if (nDiff < 0)
            {
                // Shorten the message by a reasonable amount and try again.
                Message = Message.Substring(0, Message.Length + nDiff);
            }
        } while (nDiff < 0);
        System.Web.HttpContext.Current.Response.Redirect(strURL, false);
    }

    public static string SetPostBackPage(string FormAction)
    {
        return ("<script>document.forms[0].action = \"" + FormAction + "\";" + "document.forms[0].__VIEWSTATE.name = \'NOVIEWSTATE\';</script>");
    }

    public static string EditorScripts(string var2, string AppeWebPath, string BrowserCode)
    {
        StringBuilder result = new StringBuilder();
        if (!(BrowserCode == "ar" || BrowserCode == "da" || BrowserCode == "de" || BrowserCode == "en" || BrowserCode == "es" || BrowserCode == "fr" || BrowserCode == "he" || BrowserCode == "it" || BrowserCode == "ja" || BrowserCode == "ko" || BrowserCode == "nl" || BrowserCode == "pt" || BrowserCode == "ru" || BrowserCode == "sv" || BrowserCode == "zh"))
        {
            BrowserCode = "en";
        }
        result.Append("<script language=\"JavaScript1.2\">" + "\r\n");
        result.Append("var LicenseKeys = \"" + var2 + "\";" + "\r\n");
        result.Append("var eWebEditProPath = \"" + AppeWebPath + "\"; " + "\r\n");
        result.Append("var WIFXPath= \"" + AppeWebPath + "\";" + "\r\n");
        result.Append("var WebImageFXPath = \"" + AppeWebPath + "\";" + "\r\n");
        result.Append("var eWebEditProMsgsFilename = \"ewebeditpromessages\" + \"" + BrowserCode + "\"+ \".js\";" + "\r\n");


        result.Append("function InformationPassingParameters()" + "\r\n");
        result.Append("{" + "\r\n");
        result.Append("var strLoadPage = \"\";" + "\r\n");
        result.Append("var strParamChar = \"?\";" + "\r\n");

        result.Append("if(\"undefined\" != typeof eWebEditProPath)" + "\r\n");
        result.Append("{" + "\r\n");
        result.Append("  strLoadPage += strParamChar + \"instewep=\";" + "\r\n");
        result.Append("strLoadPage += eWebEditProPath;" + "\r\n");
        result.Append("strParamChar = \"&\";" + "\r\n");
        result.Append("}" + "\r\n");
        result.Append("else" + "\r\n");
        result.Append("{" + "\r\n");
        result.Append("strLoadPage += strParamChar + \"instewep=undefined\";" + "\r\n");
        result.Append("strParamChar = \"&\";" + "\r\n");
        result.Append("}" + "\r\n");

        result.Append(" if(\"undefined\" != typeof LicenseKeys)" + "\r\n");
        result.Append("{" + "\r\n");
        result.Append("strLoadPage += strParamChar + \"licnewep=\";" + "\r\n");
        result.Append("strLoadPage += LicenseKeys;" + "\r\n");
        result.Append("strParamChar = \"&\";" + "\r\n");
        result.Append("}    " + "\r\n");
        result.Append("else" + "\r\n");
        result.Append("{" + "\r\n");
        result.Append("strLoadPage += strParamChar + \"licnewep=undefined\";" + "\r\n");
        result.Append("strParamChar = \"&\";" + "\r\n");
        result.Append("}" + "\r\n");
        result.Append("if(\"undefined\" != typeof WebImageFXPath)" + "\r\n");
        result.Append("{" + "\r\n");
        result.Append("if (WebImageFXPath.length > 0)" + "\r\n");
        result.Append("{" + "\r\n");
        result.Append("strLoadPage += \"&instwifx=\";" + "\r\n");

        result.Append("strLoadPage += WebImageFXPath;" + "\r\n");
        result.Append("strParamChar =\"&\";" + "\r\n");
        result.Append(" }" + "\r\n");
        result.Append(" }" + "\r\n");

        result.Append("if(\"undefined\" != typeof WifxLicenseKeys)" + "\r\n");
        result.Append("{" + "\r\n");
        result.Append("if (WifxLicenseKeys.length > 0)" + "\r\n");
        result.Append("{" + "\r\n");
        result.Append("strLoadPage += \"&licnwifx=\";" + "\r\n");
        result.Append("strLoadPage += WifxLicenseKeys;" + "\r\n");
        result.Append("strParamChar = \"&\";" + "\r\n");
        result.Append("}" + "\r\n");
        result.Append("}" + "\r\n");

        result.Append("return(strLoadPage);" + "\r\n");
        result.Append("}" + "\r\n");
        result.Append("</script>" + "\r\n");

        result.Append("<script language=\"JavaScript1.2\" src=\"" + AppeWebPath + "cms_ewebeditpro.js\"></script>" + "\r\n");
        //result.Append("<script language=""JavaScript1.2"">	" & vbCrLf)
        //// The install popup was correctly created at the beginning
        //// (within the eWebEditProinstallPopupUrl variable)
        //// but it needs to be set into the installPopup.url value
        //// for it to automatically be used to install the editor.
        //// Otherwise, it sits in the variable doing nothing.
        //// eWebEditPro.parameters.installPopup.url = eWebEditProinstallPopupUrl + InformationPassingParameters();
        //result.Append("</script>" & vbCrLf)
        return (result.ToString());
    }

    public static string eWebEditProField(string EditorName, string FieldName, string SetContentType, string GetContentType, string ContentHtml)
    {
        StringBuilder result = new StringBuilder();
        if (EditorName != FieldName)
        {
            result.Append("<input type=\"hidden\" name=\"" + FieldName + "\" value=\"" + EkFunctions.HtmlEncode(ContentHtml) + "\">" + "\r\n");
        }
        result.Append("<script language=\"JavaScript1.2\" type=\"text/javascript\">" + "\r\n");
        result.Append("<!--" + "\r\n");
        result.Append("eWebEditPro.defineField(\"" + EditorName + "\", \"" + FieldName + "\", \"" + SetContentType + "\", \"" + GetContentType + "\");" + "\r\n");
        result.Append("//-->" + "\r\n");
        result.Append("</script>" + "\r\n");
        return (result.ToString());
    }

    public static string eWebEditProEditor(string FieldName, string Width, string Height, string ContentHtml)
    {
        StringBuilder result = new StringBuilder();
        result.Append("<input type=\"hidden\" name=\"" + FieldName + "\" value=\"" + EkFunctions.HtmlEncode(ContentHtml) + "\">");
        result.Append("<script language=\"JavaScript1.2\" type=\"text/javascript\">" + "\r\n");
        result.Append("<!--" + "\r\n");
        result.Append("eWebEditPro.create(\"" + FieldName + "\", \"" + Width + "\", \"" + Height + "\");" + "\r\n");
        result.Append("//-->" + "\r\n");
        result.Append("</script>");
        return (result.ToString());
    }

    public static string eWebEditProPopupButton(string ButtonName, string FieldName)
    {
        StringBuilder result = new StringBuilder();
        result.Append("<script language=\"JavaScript1.2\" type=\"text/javascript\">" + "\r\n");
        result.Append("<!--" + "\r\n");
        result.Append("eWebEditPro.createButton(\"" + ButtonName + "\", \"" + FieldName + "\");" + "\r\n");
        result.Append("//-->" + "\r\n");
        result.Append("</script>");
        return (result.ToString());
    }

    public static bool IsAsset(int lContentType, string strAssetID)
    {
        bool result = false;
        result = System.Convert.ToBoolean((Ektron.Cms.Common.EkConstants.ManagedAsset_Min <= lContentType & lContentType <= Ektron.Cms.Common.EkConstants.ManagedAsset_Max) || (Ektron.Cms.Common.EkConstants.Archive_ManagedAsset_Min <= lContentType & lContentType <= Ektron.Cms.Common.EkConstants.Archive_ManagedAsset_Max) || strAssetID.Length > 0);
        return (result);
    }
    public static bool IsAssetType(int lContentType)
    {
        bool result = false;
        result = System.Convert.ToBoolean((Ektron.Cms.Common.EkConstants.ManagedAsset_Min <= lContentType & lContentType <= Ektron.Cms.Common.EkConstants.ManagedAsset_Max) || (Ektron.Cms.Common.EkConstants.Archive_ManagedAsset_Min <= lContentType & lContentType <= Ektron.Cms.Common.EkConstants.Archive_ManagedAsset_Max));
        return (result);
    }

    public static bool IsBrowserIE()
    {
        return ((System.Web.HttpContext.Current.Request.Browser.Type.IndexOf("IE") != -1) ? true : false);
    }

    public static bool IsPc()
    {
        bool returnValue;
        string str;
        str = System.Web.HttpContext.Current.Request.ServerVariables["HTTP_USER_AGENT"];
        returnValue = System.Convert.ToBoolean((str.IndexOf("Windows") + 1 > 0) ? true : false);
        return returnValue;
    }

    public static bool IsMac()
    {
        return (!IsPc());
    }

    public static string StripHTML(string strText)
    {
        return ContentAPI.StripHTML(strText);
    }

    public static SitemapPath[] DeserializeSitemapPath(System.Collections.Specialized.NameValueCollection form, int language)
    {
        string xml = System.Web.HttpUtility.UrlDecode(System.Convert.ToString(form["saved_sitemap_path"]));
        System.Xml.XmlDocument doc = new System.Xml.XmlDocument();
        System.Xml.XmlNodeList nodes;

        Ektron.Cms.Common.SitemapPath[] ret = null;
        Ektron.Cms.Common.SitemapPath smpNode;
        int iCount = 0;
        try
        {
            xml = xml.Replace("&", "&amp;");
            doc.LoadXml(xml);
            nodes = doc.SelectNodes("sitemap/node");
            foreach (System.Xml.XmlNode node in nodes)
            {
                smpNode = new Ektron.Cms.Common.SitemapPath();
                smpNode.Title = System.Web.HttpUtility.HtmlDecode((string)(node.SelectSingleNode("title").InnerXml));
                smpNode.Url = System.Web.HttpUtility.HtmlDecode(node.SelectSingleNode("url").InnerXml);
                smpNode.Order = int.Parse(node.SelectSingleNode("order").InnerXml);
                smpNode.Description = System.Web.HttpUtility.HtmlDecode((string)(node.SelectSingleNode("description").InnerXml));
                smpNode.Language = language;
                iCount++;
                Array.Resize(ref ret, iCount + 1);
                ret[iCount] = smpNode;
            }
        }
        catch
        {
        }
        return ret;
    }

    public static int FindSitemapPath(Ektron.Cms.Common.SitemapPath[] sitemapPaths, SitemapPath sitemapPath)
    {
        //return -1 if not found
        int iRet = -1;
        int iLoop = 1;
        Ektron.Cms.Common.SitemapPath node;
        if (sitemapPath == null)
        {
            return -1;
        }
        if (sitemapPaths != null)
        {
            for (iLoop = 1; iLoop <= sitemapPaths.Length - 1; iLoop++)
            {
                node = sitemapPaths[iLoop];
                if ((node.Title == sitemapPath.Title) && (node.Url == sitemapPath.Url) && (node.FolderId == sitemapPath.FolderId))
                {
                    iRet = iLoop;
                    break;
                }
            }
        }
        return iRet;
    }

    public static bool IsDefaultXmlConfig(long xml_id, XmlConfigData[] active_list)
    {
        foreach (XmlConfigData xmlData in active_list)
        {
            if (xmlData.Id == xml_id)
            {
                if (xmlData.IsDefault)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }
        return false;
    }

    public static bool IsHTMLDefault(XmlConfigData[] active_list)
    {
        foreach (XmlConfigData xmlData in active_list)
        {
            if (xmlData.IsDefault && xmlData.Id != 0)
            {
                return false;
            }
        }
        return true;
    }

    public static bool IsNonFormattedContentAllowed(XmlConfigData[] active_list)
    {
        return Ektron.Cms.Common.EkFunctions.IsNonFormattedContentAllowed(active_list);
    }

    public static long GetDefaultXmlConfig(long folder_id)
    {
        ContentAPI m_refContentApi = new ContentAPI();
        XmlConfigData[] active_list;

        active_list = m_refContentApi.GetEnabledXmlConfigsByFolder(folder_id);
        foreach (XmlConfigData xmlData in active_list)
        {
            if (xmlData.IsDefault)
            {
                return xmlData.Id;
            }
        }
        return 0;
    }

    public static void AddLBpaths(Microsoft.VisualBasic.Collection data)
    {
        ContentAPI apiCont = new ContentAPI();
        Ektron.Cms.Library.EkLibrary objLib;
        int lbICount;
        int lbFCount;
        Collection lb;
        Collection cLbs;
        lbICount = 0;
        lbFCount = 0;
        objLib = apiCont.EkLibraryRef;
        cLbs = objLib.GetAllLBPaths("images");
        if (cLbs.Count > 0)
        {
            foreach (Collection tempLoopVar_lb in cLbs)
            {
                lb = tempLoopVar_lb;
                lbICount++;
                data.Add(HttpContext.Current.Server.MapPath((string)(lb["LoadBalancePath"])), (string)("LoadBalanceImagePath_" + lbICount), null, null);
            }
        }
        data.Add(lbICount, "LoadBalanceImageCount", null, null);
        cLbs = null;
        lb = null;
        cLbs = objLib.GetAllLBPaths("files");
        if (cLbs.Count > 0)
        {
            foreach (Collection tempLoopVar_lb in cLbs)
            {
                lb = tempLoopVar_lb;
                lbFCount++;
                data.Add(HttpContext.Current.Server.MapPath((string)(lb["LoadBalancePath"])), (string)("LoadBalanceFilePath_" + lbFCount), null, null);
            }
        }
        data.Add(lbFCount, "LoadBalanceFileCount", null, null);
        cLbs = null;
    }

    //public static void AddLBpaths(Collection data)
    //{
    //    ContentAPI apiCont = new ContentAPI();
    //    Ektron.Cms.Library.EkLibrary objLib;
    //    int lbICount;
    //    int lbFCount;
    //    Collection lb;
    //    Collection cLbs;
    //    lbICount = 0;
    //    lbFCount = 0;
    //    objLib = apiCont.EkLibraryRef;
    //    cLbs = objLib.GetAllLBPaths("images");
    //    if (cLbs.Count > 0)
    //    {
    //        foreach (Collection tempLoopVar_lb in cLbs)
    //        {
    //            lb = tempLoopVar_lb;
    //            lbICount++;
    //            data.Add(HttpContext.Current.Server.MapPath((string)(lb["LoadBalancePath"])), "LoadBalanceImagePath_" + lbICount, null, null);
    //        }
    //    }
    //    data.Add(lbICount, "LoadBalanceImageCount", null, null);
    //    cLbs = null;
    //    lb = null;
    //    cLbs = objLib.GetAllLBPaths("files");
    //    if (cLbs.Count > 0)
    //    {
    //        foreach (Collection tempLoopVar_lb in cLbs)
    //        {
    //            lb = tempLoopVar_lb;
    //            lbFCount++;
    //            data.Add(HttpContext.Current.Server.MapPath((string)(lb["LoadBalancePath"])), "LoadBalanceFilePath_" + lbFCount, null, null);
    //        }
    //    }
    //    data.Add(lbFCount, "LoadBalanceFileCount", null, null);
    //    cLbs = null;
    //}
    //public static void SetLanguage(object api)
    //{
    //    int ContentLanguage = -1;
    //    if (!(System.Web.HttpContext.Current.Request.QueryString["LangType"] == null))
    //    {
    //        if (System.Web.HttpContext.Current.Request.QueryString["LangType"] != "")
    //        {
    //            ContentLanguage = Convert.ToInt32(System.Web.HttpContext.Current.Request.QueryString["LangType"]);
    //            Ektron.Cms.API.SetCookieValue("LastValidLanguageID", ContentLanguage);
    //        }
    //        else
    //        {
    //            if (Ektron.Cms.API.GetCookieValue("LastValidLanguageID") != "")
    //            {
    //                ContentLanguage = Convert.ToInt32(Ektron.Cms.API.GetCookieValue("LastValidLanguageID"));
    //            }
    //        }
    //    }
    //    else
    //    {
    //        if (Ektron.Cms.API.GetCookieValue("LastValidLanguageID") != "")
    //        {
    //            ContentLanguage = Convert.ToInt32(Ektron.Cms.API.GetCookieValue("LastValidLanguageID"));
    //        }
    //    }
    //    if (ContentLanguage == Ektron.Cms.Common.EkConstants.CONTENT_LANGUAGES_UNDEFINED)
    //    {
    //        Ektron.Cms.API.ContentLanguage = Ektron.Cms.Common.EkConstants.ALL_CONTENT_LANGUAGES;
    //    }
    //    else
    //    {
    //        Ektron.Cms.API.ContentLanguage = ContentLanguage;
    //    }
    //}
    public static void SetLanguage(ContentAPI api)
    {
        int ContentLanguage = -1;
        if (!(System.Web.HttpContext.Current.Request.QueryString["LangType"] == null))
        {
            if (System.Web.HttpContext.Current.Request.QueryString["LangType"] != "")
            {
                ContentLanguage = Convert.ToInt32(System.Web.HttpContext.Current.Request.QueryString["LangType"]);
                api.SetCookieValue("LastValidLanguageID", ContentLanguage.ToString());
            }
            else
            {
                if (api.GetCookieValue("LastValidLanguageID") != "")
                {
                    ContentLanguage = Convert.ToInt32(api.GetCookieValue("LastValidLanguageID"));
                }
            }
        }
        else
        {
            if (api.GetCookieValue("LastValidLanguageID") != "")
            {
                ContentLanguage = Convert.ToInt32(api.GetCookieValue("LastValidLanguageID"));
            }
        }
        if (ContentLanguage == Ektron.Cms.Common.EkConstants.CONTENT_LANGUAGES_UNDEFINED)
        {
            api.ContentLanguage = Ektron.Cms.Common.EkConstants.ALL_CONTENT_LANGUAGES;
        }
        else
        {
            api.ContentLanguage = ContentLanguage;
        }
    }
    public static void SetLanguage(CommonApi api)
    {
        int ContentLanguage = -1;
        if (!(System.Web.HttpContext.Current.Request.QueryString["LangType"] == null))
        {
            if (System.Web.HttpContext.Current.Request.QueryString["LangType"] != "")
            {
                ContentLanguage = Convert.ToInt32(System.Web.HttpContext.Current.Request.QueryString["LangType"]);
                api.SetCookieValue("LastValidLanguageID", ContentLanguage.ToString());
            }
            else
            {
                if (api.GetCookieValue("LastValidLanguageID") != "")
                {
                    ContentLanguage = Convert.ToInt32(api.GetCookieValue("LastValidLanguageID"));
                }
            }
        }
        else
        {
            if (api.GetCookieValue("LastValidLanguageID") != "")
            {
                ContentLanguage = Convert.ToInt32(api.GetCookieValue("LastValidLanguageID"));
            }
        }
        if (ContentLanguage == Ektron.Cms.Common.EkConstants.CONTENT_LANGUAGES_UNDEFINED)
        {
            api.ContentLanguage = Ektron.Cms.Common.EkConstants.ALL_CONTENT_LANGUAGES;
        }
        else
        {
            api.ContentLanguage = ContentLanguage;
        }
    }
    public static void SetLanguage(SiteAPI api)
    {
        int ContentLanguage = -1;
        if (!(System.Web.HttpContext.Current.Request.QueryString["LangType"] == null))
        {
            if (System.Web.HttpContext.Current.Request.QueryString["LangType"] != "")
            {
                ContentLanguage = Convert.ToInt32(System.Web.HttpContext.Current.Request.QueryString["LangType"]);
                api.SetCookieValue("LastValidLanguageID", ContentLanguage.ToString());
            }
            else
            {
                if (api.GetCookieValue("LastValidLanguageID") != "")
                {
                    ContentLanguage = Convert.ToInt32(api.GetCookieValue("LastValidLanguageID"));
                }
            }
        }
        else
        {
            if (api.GetCookieValue("LastValidLanguageID") != "")
            {
                ContentLanguage = Convert.ToInt32(api.GetCookieValue("LastValidLanguageID"));
            }
        }
        if (ContentLanguage == Ektron.Cms.Common.EkConstants.CONTENT_LANGUAGES_UNDEFINED)
        {
            api.ContentLanguage = Ektron.Cms.Common.EkConstants.ALL_CONTENT_LANGUAGES;
        }
        else
        {
            api.ContentLanguage = ContentLanguage;
        }
    }
    public static int GetLanguageId()
    {
        int languageId = 0;

        if ((HttpContext.Current != null) && (HttpContext.Current.Request != null) && !string.IsNullOrEmpty(HttpContext.Current.Request.QueryString["LangType"]) && int.TryParse(HttpContext.Current.Request.QueryString["LangType"], out languageId) && languageId > 0)
        {
            return languageId;
        }

        if ((HttpContext.Current != null) && (HttpContext.Current.Request != null) && (HttpContext.Current.Request.Cookies != null) && HttpContext.Current.Request.Cookies.Count > 0 && !string.IsNullOrEmpty((string)(HttpContext.Current.Request.Cookies["ecm"]["LastValidLanguageID"])) && int.TryParse((string)(HttpContext.Current.Request.Cookies["ecm"]["LastValidLanguageID"]), out languageId) && languageId > 0)
        {
            return languageId;
        }

        return 0;
    }

    public static int GetLanguageId(ContentAPI capi)
    {
        int languageId = GetLanguageId();

        if (languageId > 0)
        {
            return languageId;
        }

        if (capi == null)
        {
            return 0;
        }

        if (capi.RequestInformationRef.ContentLanguage > 0)
        {
            return capi.RequestInformationRef.ContentLanguage;
        }

        return capi.RequestInformationRef.DefaultContentLanguage;
    }


    //Gets the setting in web.config that allows/disallows executing developer samples
    public static bool AllowExecDevSamples()
    {
        bool returnValue;

        bool value = false;
        if (ConfigurationManager.AppSettings["ek_EnableDeveloperSamples"] != null)
        {
            value = System.Convert.ToBoolean(ConfigurationManager.AppSettings["ek_EnableDeveloperSamples"].ToString());
        }
        returnValue = value;

        return returnValue;
    }
    //Redirect to a information page if key is not set in web.config
    public static void CheckDevSampleEnabled()
    {
        if (!AllowExecDevSamples())
        {
            SiteAPI m_refSiteAPI = new SiteAPI();
            string strURL;
            strURL = m_refSiteAPI.SitePath + "Developer/InfoDevSample.aspx";
            HttpContext.Current.Response.Redirect(strURL, false);
        }
    }

    public static string GetMembershipAddContentJavascript(long folder_id, int lang_id, int height, int width)
    {
        return GetMembershipAddContentJavascript(folder_id, lang_id, height, width, 0);
    }

    public static string GetMembershipAddContentJavascript(long folder_id, int lang_id, int height, int width, long DefaultTaxonomyID)
    {
        SiteAPI m_refSiteAPI = new SiteAPI();
        StringBuilder str = new StringBuilder();

        string TaxonomyQuery = "";
        if ((HttpContext.Current.Request.QueryString["taxonomyid"] != null) && HttpContext.Current.Request.QueryString["taxonomyid"] != "")
        {
            TaxonomyQuery = (string)("&amp;taxonomyid=" + HttpContext.Current.Request.QueryString["taxonomyid"]);
        }
        else if (DefaultTaxonomyID > 0)
        {
            TaxonomyQuery = (string)("&amp;taxonomyid=" + DefaultTaxonomyID);
        }

        if (lang_id == -1 || lang_id == 0)
        {
            lang_id = m_refSiteAPI.RequestInformationRef.DefaultContentLanguage;
        }
        str.Append("{");
        str.Append("var cToolBar = \'toolbar=0,location=0,directories=0,status=1,menubar=0,scrollbars=1,resizable=1,width=");
        str.Append((short)width);
        str.Append(",height=");
        str.Append((short)height);
        str.Append("\';");
        str.Append("var url=\'");
        str.Append(m_refSiteAPI.AppPath);
        if (m_refSiteAPI.RequestInformationRef.IsMembershipUser == 1)
        {
            str.Append("/membership_add_content.aspx?mode=add&amp;mode_id=" + folder_id + "&amp;lang_id=" + lang_id + "\';");
        }
        else
        {
            str.Append("/edit.aspx?close=true" + TaxonomyQuery + "&ContType=1&LangType=" + m_refSiteAPI.RequestInformationRef.ContentLanguage + "&type=add&createtask=1&id=" + folder_id + "&AllowHTML=1\';");
        }
        str.Append("var taxonomyselectedtree = 0; ");
        str.Append("if (document.getElementById(\'taxonomyselectedtree\') != null) {");
        str.Append("taxonomyselectedtree = document.getElementById(\'taxonomyselectedtree\').value;");
        str.Append("}");
        str.Append(" if (taxonomyselectedtree >0) {url = url + \'&seltaxonomyid=\' + taxonomyselectedtree;} ");
        str.Append("var popupwin = window.open(url, \'Edit\', cToolBar);");
        str.Append("return popupwin; };return false;");
        return str.ToString();
    }

    public static string GetMembershipAddContentJavascript(long folder_id)
    {
        return Utilities.GetMembershipAddContentJavascript(folder_id, 0);
    }

    public static string GetMembershipAddContentJavascript(long folder_id, int lang_id)
    {
        int height = 660;
        int width = 790;
        return Utilities.GetMembershipAddContentJavascript(folder_id, lang_id, height, width);
    }

    public static string GetMembershipAddContentJavascript(long folder_id, int height, int width)
    {
        return Utilities.GetMembershipAddContentJavascript(folder_id, 0, height, width);
    }

    public static string GetAssetDownloadLink(long content_id)
    {
        Ektron.Cms.ContentAPI content_api = null;
        ContentData content_data = null;
        try
        {
            content_api = new Ektron.Cms.ContentAPI();
            content_data = content_api.GetContentById(content_id, 0);
            if (content_data.AssetData == null || content_data.AssetData.Id.Trim().Length == 0)
            {
                return string.Empty;
            }
            return content_api.RequestInformationRef.ApplicationPath + "/DownloadAsset.aspx?id=" + content_id;
        }
        catch (Exception)
        {
            return string.Empty;
        }
    }

    public static string AutoSummary(string sHTML)
    {
        return ContentAPI.AutoSummary(sHTML);
    }

    private static string FindFirstWords(string input, int howManyToFind)
    {
        return FindFirstWords(input, howManyToFind);
    }

    public static string WikiQLink(string strText, long folderID)
    {
        if (strText.Length > 0)
        {
            strText = Strings.Replace(strText, "[[", "<span style=\"color:blue;\" folderid=\"" + folderID + "\" class=\"MakeLink\" category=\"\" target=\"_self\" >", 1, -1, CompareMethod.Text);
            strText = Strings.Replace(strText, "]]", "</span>", 1, -1, CompareMethod.Text);
        }
        return (strText);
    }

    public static string BuildRegexToCheckMaxLength(int MaxLength)
    {
        // Example use:
        // RegularExpressionValidator.ValidationExpression = Utilities.BuildRegexToCheckMaxLength(iMaxContLength)
        //
        // Firefox 2.0 regular expression max repetition is 65535, that is, "{0,65535}"
        if (MaxLength <= 0)
        {
            throw (new ArgumentException("MaxLength must be positive", "MaxLength"));
        }
        StringBuilder strRegex = new StringBuilder();
        // Form if max <= 65535: ^[\w\W]{0,max}$
        // Form if max > 65535: ^([\w\W]{0,32768}){0,<%=max \ 32768%>}[\w\W]{0,<%=max Mod 32768%>}$
        // \w\W means any character including new line
        // Example,
        // 768000 => ^([\w\W]{0,32768}){0,23}[\w\W]{0,14336}$
        strRegex.Append("^");
        if (MaxLength > 65535)
        {
            strRegex.Append("([\\w\\W]{0,32768}){0,");
            strRegex.Append(Convert.ToString(MaxLength / 32768)); // quotient
            strRegex.Append("}");
        }
        strRegex.Append("[\\w\\W]{0,");
        if (MaxLength <= 65535)
        {
            strRegex.Append(MaxLength.ToString());
        }
        else
        {
            strRegex.Append(Convert.ToString(MaxLength % 32768)); // remainder
        }
        strRegex.Append("}$");
        return strRegex.ToString();
    }

    public static Ektron.Telerik.WebControls.EditorStripFormattingOptions MSWordFilterOptions(SettingsData settings_data)
    {
        Ektron.Telerik.WebControls.EditorStripFormattingOptions ConfigSetting = Ektron.Telerik.WebControls.EditorStripFormattingOptions.MSWord;
        if (settings_data.PreserveWordStyles && settings_data.PreserveWordClasses)
        {
            ConfigSetting = Ektron.Telerik.WebControls.EditorStripFormattingOptions.MSWordPreserveStyles;// +Ektron.Telerik.WebControls.EditorStripFormattingOptions.MSWordPreserveClasses;
        }
        else if (true == settings_data.PreserveWordStyles)
        {
            ConfigSetting = Ektron.Telerik.WebControls.EditorStripFormattingOptions.MSWordPreserveStyles;
        }
        else if (true == settings_data.PreserveWordClasses)
        {
            ConfigSetting = Ektron.Telerik.WebControls.EditorStripFormattingOptions.MSWordPreserveClasses;
        }
        return ConfigSetting;
    }

    public static string GetEditorPreference(System.Web.HttpRequest Request)
    {
        //TODO: Move the editor choices to an xml file specified by the server then key the possible values
        //off of a matrix setup in the xml file based on OS version and browser version
        string SelectedEditControl = "ContentDesigner";
        bool IsMac = false;
        try
        {
            if (Request.Browser.Platform.IndexOf("Win") == -1)
            {
                IsMac = true;
            }

            //Which Editor
            if (IsMac)
            {
                if (ConfigurationManager.AppSettings["ek_EditControlMac"] != null)
                {
                    SelectedEditControl = (string)(ConfigurationManager.AppSettings["ek_EditControlMac"].ToString());
                }
            }
            else
            {
                if (ConfigurationManager.AppSettings["ek_EditControlWin"] != null)
                {
                    SelectedEditControl = (string)(ConfigurationManager.AppSettings["ek_EditControlWin"].ToString());
                }
                if (SelectedEditControl.ToLower() == "userpreferred")
                {
                    CommonApi api = new CommonApi();
                    if (api.RequestInformationRef.UserEditorType == Ektron.Cms.Common.EkEnumeration.UserEditorType.ewebeditpro)
                    {
                        SelectedEditControl = "eWebEditPro";
                    }
                    else
                    {
                        SelectedEditControl = "ContentDesigner";
                    }
                }
            }
        }
        catch (Exception)
        {
            SelectedEditControl = "ContentDesigner";
        }

        return SelectedEditControl;
    }
    public static bool GetTemporaryMarkersPreference(System.Web.HttpRequest Request)
    {
        bool ShowTemporaryMarkers = true;
        try
        {
            if (ConfigurationManager.AppSettings["ek_ShowTemporaryMarkers"] != null)
            {
                bool.TryParse(ConfigurationManager.AppSettings["ek_ShowTemporaryMarkers"].ToString(), out ShowTemporaryMarkers);
            }
        }
        catch (Exception)
        {
            ShowTemporaryMarkers = true;
        }

        return ShowTemporaryMarkers;
    }
    public static string IsChecked(string val1, string val2)
    {
        if (val1.ToLower() == val2.ToLower())
        {
            return (" checked=\"checked\" ");
        }
        else
        {
            return ("");
        }
    }
    public static string IsSelected(string val1, string val2)
    {
        if (val1.ToLower() == val2.ToLower())
        {
            return (" selected=\"selected\" ");
        }
        else
        {
            return ("");
        }
    }

    public static string GetCorrectThumbnailFileWithExtn(string sFilename)
    {
        string[] aTemp = sFilename.Split('.');
        string sRet = "";
        try
        {
            if (aTemp.Length > 1)
            {
                if (aTemp[(aTemp.Length - 1)].ToLower() == "gif")
                {
                    aTemp[(aTemp.Length - 1)] = "png";
                    sRet = string.Join(".", aTemp);
                }
                else
                {
                    sRet = sFilename;
                }
            }
        }
        catch (Exception)
        {
            sRet = sFilename;
        }
        return sRet;
    }

    public static void DisableActionRewrite(HttpContext context)
    {
        if (context.Items["ActionAlreadyWritten"] == null)
        {
            context.Items["ActionAlreadyWritten"] = true;
        }
    }

    public static string GetFolderImage(EkEnumeration.FolderType type, string applicationImagePath)
    {

        string imageURL = applicationImagePath;

        if (type == Ektron.Cms.Common.EkEnumeration.FolderType.Community)
        {

            imageURL += "images/ui/icons/folderCommunity.png";
        }
        else if (type == Ektron.Cms.Common.EkEnumeration.FolderType.Catalog)
        {

            imageURL += "images/ui/icons/folderGreen.png";
        }
        else
        {

            imageURL += "images/ui/icons/folder.png";
        }

        return imageURL;

    }

    public static string GetProductImage(EkEnumeration.CatalogEntryType entryType, string applicationImagePath)
    {
        string productImage = applicationImagePath;
        if (entryType == Ektron.Cms.Common.EkEnumeration.CatalogEntryType.Bundle)
        {
            productImage += "images/ui/icons/package.png";
        }
        else if (entryType == Ektron.Cms.Common.EkEnumeration.CatalogEntryType.ComplexProduct)
        {
            productImage += "images/ui/icons/bricks.png";
        }
        else if (entryType == Ektron.Cms.Common.EkEnumeration.CatalogEntryType.Kit)
        {
            productImage += "images/ui/icons/bulletGreen.png";
        }
        else if (entryType == Ektron.Cms.Common.EkEnumeration.CatalogEntryType.SubscriptionProduct)
        {
            productImage += "images/ui/icons/bookGreen.png";
        }
        else
        {
            productImage += "images/ui/icons/brick.png";
        }
        return productImage;
    }
    public enum WorkareaTree
    {
        Content,
        Library,
        Tax
    }
    public static void ReloadTree(Control control, Utilities.WorkareaTree[] trees, string idPath, long folderId)
    {
        System.Text.StringBuilder result = new System.Text.StringBuilder();
        if (folderId > 0)
        {
            result
                .Append("if (typeof (reloadFolder) == 'function'){")
                .AppendFormat("reloadFolder({0});", folderId)
                .Append("}")
                .Append(Environment.NewLine);
        }
        foreach (Utilities.WorkareaTree tree in trees)
        {
            string treeType = tree.ToString() + "Tree";
            idPath = GetFormattedTreePath(idPath).Replace("\\", "\\\\");
            result.AppendFormat(
                "top.TreeNavigation(\"{0}\", \"{1}\");",
                treeType,
                idPath
                ).Append(Environment.NewLine);
        }
        Ektron.Cms.Framework.UI.JavaScript.RegisterJavaScriptBlock(control, result.ToString(), false);
    }
    private static string GetFormattedTreePath(string folderPath)
    {
        List<string> folderList = new List<string>();
        if (folderPath.IndexOf("/") > -1)
        {
            folderList.AddRange(folderPath.Split(new char[] { '/' }));
        }
        else
        {
            folderList.AddRange(folderPath.Split(new char[] { '\\' }));
        }
        folderList = folderList.FindAll(x => !string.IsNullOrEmpty(x));
        return "\\" + string.Join("\\", folderList.ToArray());
    }
}