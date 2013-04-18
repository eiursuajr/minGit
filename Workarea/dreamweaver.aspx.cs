using System;
using System.Data;
using System.Configuration;
using System.Collections;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using System.IO;
using Ektron.Cms;
using Ektron.Cms.Common;
using System.Windows.Forms;
using Ektron.ASM.AssetConfig;
using System.Collections.Generic;
using Ektron.Cms.API;
using Ektron.Cms.UI.CommonUI;
using Ektron.Cms.Library;
using Ektron.Cms.Content;
using Ektron.Cms.Modules;
using Microsoft.VisualBasic;
using Ektron.Cms.User;
using System.Text;

public partial class dreamweaver : System.Web.UI.Page
{
    ApplicationAPI AppUI = new ApplicationAPI();
    const int ALL_CONTENT_LANGUAGES = -1;
    const int CONTENT_LANGUAGES_UNDEFINED = 0;
    string AppPath = "";
    string AppImgPath = "";
    int ContentLanguage;
    EkMessageHelper MsgHelper;
    EkLibrary m_refLib;
    EkContent m_refContent;
    EkModule m_refModule;
    object CurrentUserId;
    object sitePath;
    object AppName;
    object AppeWebPath;
    object EnableMultilingual;
    string username;
    string foldertype;
    string password;
    string action;
    string ErrString = "";
    object xmlstring;
    long catid;
    long uid;
    object siteid;
    long ContentID;
    int loopcount;
    string domain;
    string protocol;
    bool ret;
    string ErrorString;
    string myerrormsg;
    Collection cFolders;
    Collection cFolder;
    EkUser userObj;
    Collection gtNav;
    Collection gtNavs;
    Collection cCont;
    Collection cConts;
    int loopnumber = 0;
    long cid;
    Collection myFolders;
    Collection UserRights;
    object[] NavArray;
    EkRequestInformation RI;

    public object addLBpaths(Collection cFolder, long currentUserID, object Site)
    {
        object returnValue;
        int lbICount;
        int lbFCount;
        Collection lb;
        Collection cLbs;
        lbICount = 0;
        lbFCount = 0;
        m_refLib = AppUI.EkLibraryRef;
        cLbs = m_refLib.GetAllLBPaths("images");
        if (cLbs.Count > 0)
        {
            foreach (Collection tempLoopVar_lb in (IEnumerable)cLbs)
            {
                lb = tempLoopVar_lb;
                lbICount++;
                cFolder.Add("LoadBalanceImagePath_" + lbICount, (string)getPhysicalPath(lb["LoadBalancePath"]), null, null);
            }
        }
        cFolder.Add("LoadBalanceImageCount", Convert.ToString(lbICount), null, null);
        cLbs = null;
        lb = null;
        cLbs = m_refLib.GetAllLBPaths("files");
        if (cLbs.Count > 0)
        {
            foreach (Collection tempLoopVar_lb in (IEnumerable)cLbs)
            {
                lb = tempLoopVar_lb;
                lbFCount++;
                cFolder.Add("LoadBalanceFilePath_" + lbFCount, (string)getPhysicalPath(lb["LoadBalancePath"]), null, null);
            }
        }
        cFolder.Add("LoadBalanceFileCount", Convert.ToString(lbFCount), null, null);
        cLbs = null;
        returnValue = false;
        return returnValue;
    }
    public object getPhysicalPath(object path)
    {
        object returnValue;
        //On Error Resume Next VBConversions Warning: On Error Resume Next not supported in C#
        returnValue = Server.MapPath(path.ToString());
        Microsoft.VisualBasic.CompilerServices.ProjectData.ClearProjectError();
        return returnValue;
    }
    public void OutputContentFolders(string IncomingString, long Parent, ref string[] TestArray)
    {
        Collection cContent = new Collection();
        cContent.Add(Parent, "ParentID", null, null);
        cContent.Add("name", "OrderBy", null, null);
        cFolders = m_refContent.GetAllViewableChildFoldersv2_0(cContent);

        foreach (Collection tempLoopVar_cFolder in (IEnumerable)cFolders)
        {
            cFolder = tempLoopVar_cFolder;
            Array.Resize(ref TestArray, loopnumber + 1 + 1);

            int iftype = Convert.ToInt32(cFolder["FolderType"]);
            if ((foldertype == "blog" && iftype == 1) || (foldertype == "forum" && iftype == 3) || foldertype == "all")
            {
                TestArray[loopnumber] = "<catid>" + cFolder["ID"] + "</catid>" + "<catname>" + IncomingString + "\\" + cFolder["name"] + "</catname>";
                loopnumber++;
            }
            OutputContentFolders(IncomingString + "\\" + cFolder["name"], Convert.ToInt64(cFolder["ID"]), ref TestArray);

        }
    }
    public object Login(string user, string pass, string domain)
    {
        xmlstring = "";
        protocol = AppUI.AuthProtocol;
        userObj = AppUI.EkUserRef;
        domain = Ektron.Cms.Common.EkConstants.CreateADsPathFromDomain(domain);
        Hashtable cUserHash = userObj.logInUser(username, password, (string)Request.ServerVariables["SERVER_NAME"], domain, protocol, false, EkEnumeration.AutoAddUserTypes.Author);
        userObj = null;
        if ((cUserHash.Count > 0) && ((string)ErrString == ""))
        {
            xmlstring = "<count>1</count>" + "<userid>" + cUserHash["UserID"] + "</userid><siteid>" + sitePath + "," + cUserHash["LoginNumber"] + "</siteid><sitepath>" + sitePath + "</sitepath>";
            Response.Write(xmlstring);
        }
        else
        {
            xmlstring = "<count>0</count><sitepath>" + sitePath + "</sitepath>";
            Response.Write(xmlstring);
        }
        return null;
    }

    private string GetMenuList(string FolderID, string OrderBy)
    {
        string RefString = "";

        Collection results = AppUI.EkContentRef.GetMenuReport(RefString);

        StringBuilder xml = new System.Text.StringBuilder();

        xml.Append("<count>" + results.Count + "</count>");
        foreach (Collection result in (IEnumerable)results)
        {
            xml.Append("<id>" + result["MenuID"].ToString() + "</id>");
            xml.Append("<title>" + result["MenuTitle"].ToString() + "</title>");
        }

        return xml.ToString();
    }
    private string GetLanguages()
    {
        Hashtable results;
        Hashtable result;

        results = AppUI.EkSiteRef.GetAllActiveLanguages();
        StringBuilder xml = new System.Text.StringBuilder();
        xml.Append("<count>" + results.Count + "</count>");
        foreach (string key in results.Keys)
        {
            result = (System.Collections.Hashtable)(results[key]);
            xml.Append("<id>" + result["ID"].ToString() + "</id>");
            xml.Append("<title>" + result["Name"].ToString() + "</title>");
        }
        return xml.ToString();
    }
    private string GetXmlConfigurationList(string OrderBy)
    {

        Collection results = AppUI.EkContentRef.GetAllXmlConfigurations(OrderBy);
        string xml;

        xml = "<count>" + results.Count + "</count>";

        foreach (Collection result in (IEnumerable)results)
        {
            xml += "<id>" + result["CollectionID"].ToString() + "</id>";
            xml += "<title>" + (result["CollectionTitle"].ToString() + "</title>");
        }

        return xml;
    }
    protected void Page_Load(object sender, System.EventArgs e)
    {
        MsgHelper = AppUI.EkMsgRef;
        m_refLib = AppUI.EkLibraryRef;
        CurrentUserId = AppUI.UserId;
        AppImgPath = (string)AppUI.AppImgPath;
        sitePath = AppUI.SitePath;
        AppName = AppUI.AppName;
        AppeWebPath = AppUI.AppeWebPath;
        AppPath = (string)AppUI.AppPath;
        EnableMultilingual = AppUI.EnableMultilingual;
        catid = Convert.ToInt64(Request.QueryString["catid"]);
        if (AppUI.ContentLanguage == -1)
        {
            AppUI.ContentLanguage = Convert.ToInt32(AppUI.GetCookieValue("DefaultLanguage"));
        }

        if (!string.IsNullOrEmpty(Request.QueryString["LangType"]))
        {
            ContentLanguage = Convert.ToInt32(Request.QueryString["LangType"]);
            AppUI.SetCookieValue("LastValidLanguageID", Convert.ToString(ContentLanguage));
        }
        else
        {
            if (AppUI.GetCookieValue("LastValidLanguageID").ToString() != "")
            {
                ContentLanguage = System.Convert.ToInt32(AppUI.GetCookieValue("LastValidLanguageID"));
            }
        }
        AppUI.ContentLanguage = ContentLanguage;

        RI = AppUI.RequestInformationRef;
        RI.CallerId = Convert.ToInt64(Request.QueryString["uid"]);
        RI.CookieSite = Request.QueryString["siteid"];
        RI.UserId = Convert.ToInt64(Request.QueryString["uid"]);
        m_refContent = new EkContent(RI);
        m_refLib = new Ektron.Cms.Library.EkLibrary(RI);
        m_refModule = new Ektron.Cms.Modules.EkModule(RI);


        action = Strings.Trim(Request.QueryString["action"]);

        if (action == "logintest")
        {
            username = Request.QueryString["u"];
            password = Request.QueryString["p"];
            domain = Request.QueryString["d"];
            Login(username, password, domain);
        }
        else if (action == "categories")
        {
            string[] TestArray = new string[] { };
            object loopnumber;
            loopnumber = 0;
            foldertype = Request.QueryString["foldertype"];
            cFolders = m_refContent.GetFolderInfov2_0(0);
            OutputContentFolders("", 0, ref TestArray);
            if (ErrString != "")
            {
                Response.Write("<count>" + ((TestArray.Length - 1) - 0 + "</count>"));
            }
            else
            {
                if (foldertype == "all")
                {
                    Response.Write("<count>" + ((TestArray.Length - 1) - 0 + 1) + "</count>");
                    Response.Write("<catid>0</catid><catname>\\</catname>");
                }
                else
                {
                    Response.Write("<count>" + ((TestArray.Length - 1) - 0 - 1) + "</count>");
                }

            }
            for (int g = 0; g <= (TestArray.Length - 1) - 1; g++)
            {
                Response.Write(TestArray[g]);
            }

        }
        else if (action == "content")
        {
            Collection cTmp = new Collection();
            int intTotalPages = 0;
            object[] ContentArray;
            ContentArray = new object[1];
            loopcount = 0;
            cTmp.Add(catid, "FolderID", null, null);
            cTmp.Add("Title", "OrderBy", null, null);
            EkContentCol datacCol = m_refContent.GetAllViewableChildContentInfoV5_0(cTmp, 1, 0, ref intTotalPages);
            foreach (ContentBase item in datacCol)
            {
                Array.Resize(ref ContentArray, loopcount + 1 + 1);
                ContentArray[loopcount] = "<id>" + item.Id + "</id>" + "<title>" + item.Title + "</title>";
                loopcount++;
            }
            Response.Write("<count>" + ((ContentArray.Length - 1) - 0 + "</count>"));
            for (int h = 0; h <= (ContentArray.Length - 1) - 1; h++)
            {
                Response.Write(ContentArray[h]);
            }
        }
        else if (action == "collections")
        {
            string orderby = "title";
            NavArray = new object[1];
            loopcount = 0;
            gtNavs = m_refContent.GetAllCollectionsInfo(catid, orderby);
            foreach (Collection tempLoopVar_gtNav in (IEnumerable)gtNavs)
            {
                gtNav = tempLoopVar_gtNav;
                Array.Resize(ref NavArray, loopcount + 1 + 1);
                NavArray[loopcount] = "<id>" + gtNav["CollectionID"] + "</id>" + "<title>" + gtNav["CollectionTitle"] + "</title>";
                loopcount++;
            }
            Response.Write("<count>" + ((NavArray.Length - 1) - 0 + "</count>"));
            for (int h = 0; h <= (NavArray.Length - 1) - 1; h++)
            {
                Response.Write(NavArray[h]);
            }
            gtNavs = null;
        }
        else if (action == "collections_all")
        {
            NavArray = new object[1];
            loopcount = 0;
            uid = Convert.ToInt64(Request.QueryString["uid"]);
            siteid = Request.QueryString["siteid"];
            gtNavs = m_refContent.GetCollectionsReport(ref ErrorString);
            foreach (Collection tempLoopVar_gtNav in (IEnumerable)gtNavs)
            {
                gtNav = tempLoopVar_gtNav;
                Array.Resize(ref NavArray, loopcount + 1 + 1);
                NavArray[loopcount] = "<id>" + gtNav["CollectionID"] + "</id>" + "<title>id=" + gtNav["CollectionID"] + ", " + gtNav["CollectionTitle"] + "</title>";
                loopcount++;
            }
            Response.Write("<count>" + ((NavArray.Length - 1) - 0 + "</count>"));
            for (int h = 0; h <= (NavArray.Length - 1) - 1; h++)
            {
                Response.Write(NavArray[h]);
            }
            gtNavs = null;
        }
        else if (action == "calendar")
        {
            NavArray = new object[1];
            loopcount = 0;
            gtNavs = m_refModule.GetAllCalendarInfo();
            foreach (Collection tempLoopVar_gtNav in (IEnumerable)gtNavs)
            {
                gtNav = tempLoopVar_gtNav;
                Array.Resize(ref NavArray, loopcount + 1 + 1);
                NavArray[loopcount] = "<id>" + gtNav["CalendarID"] + "</id>" + "<title>id=" + gtNav["CalendarID"] + ", " + gtNav["CalendarTitle"] + "</title>";
                loopcount++;
            }
            Response.Write("<count>" + ((NavArray.Length - 1) - 0 + "</count>"));
            for (int h = 0; h <= (NavArray.Length - 1) - 1; h++)
            {
                Response.Write(NavArray[h]);
            }
            gtNavs = null;
        }
        else if (action == "ecmforms")
        {
            object gtForms;

            NavArray = new object[1];
            loopcount = 0;
            gtForms = m_refModule.GetAllFormInfo();

            foreach (Collection gtForm in (IEnumerable)gtForms)
            {
                Array.Resize(ref NavArray, loopcount + 1 + 1);
                NavArray[loopcount] = "<id>" + gtForm["FormID"] + "</id>" + "<title>id=" + gtForm["FormID"] + ", " + gtForm["FormTitle"] + "</title>";
                loopcount++;
            }
            Response.Write("<count>" + ((NavArray.Length - 1) - 0 + "</count>"));
            for (int h = 0; h <= (NavArray.Length - 1) - 1; h++)
            {
                Response.Write(NavArray[h]);
            }
            gtNavs = null;
        }
        else if (action == "editcontent")
        {
            Collection cEContent;
            object myxmlstring;
            object PageInfo;
            object myhtmlcontent;
            object stylesheetpath = null;
            object mywhynoteditstring;
            Collection canI;

            PageInfo = "src=\"http://" + Request.ServerVariables["Server_name"] + "/";
            cid = Convert.ToInt64(Request.QueryString["cid"]);

            canI = m_refContent.CanIv2_0(cid, "content");
            if (Convert.ToBoolean(canI["CanIEdit"]))
            {
                cEContent = m_refContent.GetContentForEditingv2_0(cid);
                myhtmlcontent = cEContent["ContentHtml"];
                if (Strings.Trim((string)(cEContent["StyleSheet"])) != "")
                {
                    stylesheetpath = "http://" + Convert.ToString(Request.ServerVariables["Server_name"]) + sitePath + cEContent["StyleSheet"];
                }
                myxmlstring = "<ektron_head_html><!--   Do not remove these xml tags <ektron_head><ektron_edit>yes</ektron_edit><ektron_content_id>" + cid + "</ektron_content_id> <ektron_title>" + cEContent["ContentTitle"] + "</ektron_title> <ektron_content_comment>" + cEContent["Comment"] + "</ektron_content_comment> <ektron_content_stylesheet>" + stylesheetpath + "</ektron_content_stylesheet> <ektron_folder_id>" + cEContent["FolderID"] + "</ektron_folder_id> <ektron_content_language>" + cEContent["ContentLanguage"] + "</ektron_content_language> <ektron_go_live>" + cEContent["GoLive"] + "</ektron_go_live> <ektron_end_date>" + cEContent["EndDate"] + "</ektron_end_date> <ektron_MetadataNumber>" + ((Collection)cEContent["ContentMetadata"]).Count + "</ektron_MetadataNumber> <ektron_PreviousState>" + cEContent["CurrentContentStatus"] + "</ektron_PreviousState> <ektron_iMaxContLength>" + cEContent["MaxContentSize"] + "</ektron_iMaxContLength> <ektron_content_Path>" + cEContent["Path"] + "</ektron_content_Path></ektron_head> --> </ektron_head_html> ";
                myxmlstring = myxmlstring + "<ektron_body_html>" + myhtmlcontent + "</ektron_body_html>";
            }
            else
            {
                if (Convert.ToString(canI["ContentState"]) == "CheckedOut")
                {
                    mywhynoteditstring = "You can not edit this cotent, content is checked out to: " + canI["UserName"];
                }
                else
                {
                    mywhynoteditstring = "You can not edit this this content.  The content state is:  " + canI["ContentState"] + "; The user is " + canI["UserName"];
                }
                myxmlstring = "<ektron_head_html><!--   <ektron_edit>" + mywhynoteditstring + "</ektron_edit> --></ektron_head_html>";
            }
            Response.Write(myxmlstring);
            canI = null;

        }
        else if (action == "publish_update_content")
        {
            ErrorString = "";
            myerrormsg = "";
            cid = Convert.ToInt64(Request.QueryString["cid"]);

            Collection cCont = m_refContent.GetContentByIDv2_0(cid);

            cCont.Remove("ContentHtml");
            cCont.Add(Strings.Replace(Request.Form["ContentHTML"], "<myektronand/>", "&", 1, -1, 0), "ContentHtml", null, null);
            cCont.Remove("ContentTitle");
            cCont.Add(Request.Form["content_title"], "ContentTitle", null, null);
            cCont.Remove("Comment");
            cCont.Add(Request.Form["content_comment"], "Comment", null, null);
            cCont.Remove("ContentID");
            cCont.Add(Request.Form["ektron_content_id"], "ContentID", null, null);
            cCont.Remove("FolderID");
            cCont.Add(Request.Form["folder"], "FolderID", null, null);
            cCont.Remove("ContentLanguage");
            cCont.Add(Request.Form["ektron_content_language"], "ContentLanguage", null, null);
            //cCont.Remove("SearchText")
            cCont.Add(Strings.Replace(Request.Form["searchhtml"], "<myektronand/>", "&", 1, -1, 0), "SearchText", null, null);
            cCont.Remove("GoLive");
            cCont.Add(Request.Form["go_live"], "GoLive", null, null);
            cCont.Remove("EndDate");
            cCont.Add(Request.Form["end_date"], "EndDate", null, null);
            cCont.Remove("ContentType");
            cCont.Add(1, "ContentType", null, null);
            cCont.Remove("LockedContentLink");
            cCont.Add(true, "LockedContentLink", null, null);

            try
            {
                ret = m_refContent.SaveContentv2_0(cCont);
                if (ErrorString != "")
                {
                    myerrormsg = ErrorString;
                }
                else
                {
                    ContentID = Convert.ToInt64(Request.Form["ektron_content_id"]);
                    ret = m_refContent.CheckIn(ContentID, "");
                    if (ErrorString != "")
                    {
                        myerrormsg = ErrorString;
                    }
                    else
                    {
                        if (ret == false)
                        {
                            ret = m_refContent.SubmitForPublicationv2_0(ContentID, Convert.ToInt64(Request.Form["ektron_folder_id"]), 0, "");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                myerrormsg = ex.Message.ToString();
            }

            if (myerrormsg != "")
            {
                Response.Write(myerrormsg);
            }
            else
            {
                Response.Write("Content has been saved");
            }

        }
        else if (action == "add_new_content")
        {
            cCont = new Collection();
            try
            {
                cCont.Add((object)Strings.Replace(Convert.ToString(Request.Form["ContentHTML"]), "<myektronand/>", "&", 1, -1, 0), "ContentHtml", null, null);
                cCont.Add(Request.Form["content_title"], "ContentTitle", null, null);
                cCont.Add(Request.Form["content_comment"], "Comment", null, null);
                cCont.Add(System.Convert.ToInt32(Request.Form["folder"]), "FolderID", null, null);
                cCont.Add(Request.Form["ektron_content_language"], "ContentLanguage", null, null);
                cCont.Add(Strings.Replace(Request.Form["searchhtml"], "<myektronand/>", "&", 1, -1, 0), "SearchText", null, null);
                cCont.Add(Request.Form["go_live"], "GoLive", null, null);
                cCont.Add(Request.Form["end_date"], "EndDate", null, null);
                cCont.Add(true, "AddToQlink", null, null);
                cCont.Add(1, "ContentType", null, null);

                ErrorString = "";
                ContentID = m_refContent.AddNewContentv2_0(cCont);
            }
            catch (Exception ex)
            {
                ErrorString = ex.Message.ToString();
            }
            if (ErrorString != "")
            {
                myerrormsg = ErrorString;
            }
            else
            {
                ret = m_refContent.CheckIn(ContentID, "");
                if (ErrorString != "")
                {
                    myerrormsg = " Error CheckIn= " + ErrorString;
                }
                else
                {
                    if (ret == false)
                    {
                        ret = m_refContent.SubmitForPublicationv2_0(ContentID, Convert.ToInt64(Request.Form["ektron_folder_id"]), 0, "");
                        if (ErrorString != "")
                        {
                            myerrormsg = "Error SubmitForPublicationv2_0= " + ErrorString;
                        }
                    }
                }
            }
            if (myerrormsg != "")
            {
                Response.Write(myerrormsg);
            }
            else
            {
                Response.Write("Content has been saved");
            }

        }
        else if (action == "foldertypexml")
        {
            long folderid;
            folderid = Convert.ToInt64(Request.Form["folderid"]);
            myFolders = m_refContent.GetFolderInfov2_0(folderid, false, false);
            if (((Collection)myFolders["XmlConfiguration"]).Count == 0)
            {
                Response.Write("<XmlConfiguration>no</XmlConfiguration>");
            }
            else
            {
                Response.Write("<XmlConfiguration>yes</XmlConfiguration>");
            }
            myFolders = null;
        }
        else if (action == "canIsave")
        {
            cid = Convert.ToInt64(Request.Form["cid"]);

            cConts = m_refContent.GetContentByIDv2_0(cid);
            UserRights = m_refContent.CanIv2_0(cid, "content");
            if (Convert.ToString(cConts["ContentStatus"]) == "O" && Convert.ToBoolean(UserRights["CanIEdit"]))
            {
                Response.Write("<canISave>yes</canISave>");
            }
            else
            {
                if (Convert.ToString(cConts["ContentStatus"]) == "O")
                {
                    Response.Write("<canISave>Did not save!!!!,  Check out by " + Convert.ToString(UserRights["UserName"]) + "</canISave>");
                }
                else
                {
                    Response.Write("<canISave>Did not save!!!  Content is not checked out to you, Its status is Status=" + Convert.ToString(cConts["ContentStatus"]) + " The user associated with this is " + Convert.ToString(UserRights["UserName"]) + "</canISave>");
                }
            }

        }
        else if (action == "AddFolder")
        {
            object bRet1;
            object tmpPath;
            Collection libSettings;
            cFolder = new Collection();
            cFolder.Add(Request.Form["foldername"], "FolderName", null, null);
            cFolder.Add(Request.Form["folderdescription"], "FolderDescription", null, null);
            cFolder.Add(Request.Form["ParentID"], "ParentID", null, null);
            cFolder.Add(Request.Form["templatefilename"], "TemplateFileName", null, null);
            cFolder.Add(Request.Form["stylesheet"], "StyleSheet", null, null);
            libSettings = m_refLib.GetLibrarySettingsv2_0();

            tmpPath = libSettings["ImageDirectory"];
            cFolder.Add(getPhysicalPath(tmpPath), "AbsImageDirectory", null, null);
            tmpPath = libSettings["FileDirectory"];
            cFolder.Add(getPhysicalPath(tmpPath), "AbsFileDirectory", null, null);
            libSettings = null;
            bRet1 = addLBpaths(cFolder, uid, siteid);
            cFolder.Add(true, "XmlInherited", null, null);
            cFolder.Add(0, "XmlConfiguration", null, null);
            cFolder.Add(1, "InheritMetadata", null, null); //break inherit button is check.
            cFolder.Add(0, "InheritMetadataFrom", null, null);
            ret = m_refContent.AddContentFolderv2_0(ref cFolder);
            Response.Write("Folder added");
        }
        else if (action == "metadata")
        {
            int loopcounter = 0;
            object cMetadataTypes;
            object[] MetaDataArray;
            MetaDataArray = new object[1];
            cMetadataTypes = m_refContent.GetMetadataTypes("Name");
            foreach (Collection cMetadataType in (IEnumerable)cMetadataTypes)
            {
                Array.Resize(ref MetaDataArray, loopcounter + 1 + 1);
                MetaDataArray[loopcounter] = "<meta>" + (cMetadataType["MetaTypeName"]) + "</meta>";
                loopcounter++;
            }
            Response.Write("<count>" + ((MetaDataArray.Length - 1) - 0 + "</count>"));
            for (int j = 0; j <= (MetaDataArray.Length - 1) - 1; j++)
            {
                Response.Write(MetaDataArray[j]);
            }
        }
        else if (action == "ecmmenus")
        {
            object FolderID;
            FolderID = 0;
            Response.Write(GetMenuList(FolderID.ToString(), "Title"));
        }
        else if (action == "ecmxmconfig")
        {
            Response.Write(GetXmlConfigurationList(""));
        }
        else if (action == "ecmlanguages")
        {
            Response.Write(GetLanguages());
        }
        else
        {
            Response.Write("DreamWeaver - No Action parameter");
        }
    }
}