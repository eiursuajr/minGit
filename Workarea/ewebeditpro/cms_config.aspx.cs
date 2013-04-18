using System.Web.UI.WebControls;
using System.Configuration;
using System.Collections;
using System.Linq;
using System.Data;
using System.Web.Caching;
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

public partial class cms_config : System.Web.UI.Page
{
    protected Ektron.Cms.Common.EkMessageHelper m_refMsg;
    protected string InterfaceName = "standard";
    protected bool MinimalFeatureSet = false;
    protected string DefaultGetContentType = "htmlbody";
    protected string NoSrcView = "";
    protected string FormToolbarVisible = "";
    protected string FormToolbarEnabled = "";
    protected string EKSLK = "";
    protected string AppeWebPath = "";
    protected string extensions = "";
    protected string mode = "";
    protected string ExtUI = "";
    protected string PresWrdStyl = "";
    protected string PresWrdCls = "";
    protected string FontList = "";
    protected string sAccess = "none";
    protected string bAccessEval = "false";
    protected Ektron.Cms.SettingsData settings_data;
    protected int ContentLanguage;
    protected bool IsForum = false;
    protected Hashtable options = new Hashtable();
    protected string strButtons = "";
    protected bool bEnableFontButtons = false;
    protected bool bWikiButton = true;


    private void Page_Load(System.Object sender, System.EventArgs e)
    {
        Ektron.Cms.ContentAPI refContApi = new Ektron.Cms.ContentAPI();
        Ektron.Cms.SiteAPI refSiteApi = new Ektron.Cms.SiteAPI();
        Ektron.Cms.Site.EkSite refSite = new Ektron.Cms.Site.EkSite(refContApi.RequestInformationRef);
        long folder = 0;
        @internal.Text = refSite.GetEditorInternal();
        if (LCaseQueryString("mode") == "forum")
        {
            strButtons = Request.QueryString["toolButtons"];
            string[] arTools = strButtons.ToLower().Split(",".ToCharArray());
            foreach (string Item in arTools)
            {
                if (!(options.ContainsKey(Item)))
                {
                    options.Add(Item, Item);
                }
            }
        }
        Response.ContentType = "text/xml";
        m_refMsg = refSiteApi.EkMsgRef;
        settings_data = refSiteApi.GetSiteVariables(-1);
        refSite = refContApi.EkSiteRef;
        FontList = refContApi.GetFontConfigList();
        EKSLK = refSite.GetInternal();
        AppeWebPath = refContApi.AppPath + refContApi.AppeWebPath;

        if (LCaseQueryString("FormToolbarVisible") == "true")
        {
            FormToolbarVisible = "true";
            FormToolbarEnabled = "true";
        }
        else
        {
            FormToolbarVisible = "false";
            FormToolbarEnabled = "true";
        }
        if ("0" == LCaseQueryString("wiki"))
        {
            bWikiButton = bool.Parse("false");
        }
        else
        {
            bWikiButton = bool.Parse("true");
        }
        //Set the state for word styles
        if (settings_data.PreserveWordStyles == true)
        {
            PresWrdStyl = "true";
        }
        else
        {
            PresWrdStyl = "false";
        }
        if (settings_data.PreserveWordClasses == true)
        {
            PresWrdCls = "true";
        }
        else
        {
            PresWrdCls = "false";
        }
        switch (settings_data.Accessibility)
        {
            case "1":
                sAccess = "loose";
                bAccessEval = "true";
                break;
            case "2":
                sAccess = "strict";
                bAccessEval = "true";
                break;
            default:
                sAccess = "none";
                bAccessEval = "false";
                break;
        }
        NoSrcView = LCaseQueryString("nosrc");
        mode = LCaseQueryString("mode");
        if (mode == "")
        {
            mode = "wysiwyg";
        }

        if (mode == "forum")
        {
            mode = "wysiwyg";
            IsForum = true;
        }

        if ("datadesign" == mode || "dataentry" == mode)
        {
            DefaultGetContentType = "datadesignpackage";
        }
        else if ("formdesign" == mode)
        {
            DefaultGetContentType = "designpage";
        }

        ExtUI = LCaseQueryString("extui");

        InterfaceName = LCaseQueryString("InterfaceName");
        if ("" == InterfaceName)
        {
            if ("datadesign" == mode || "formdesign" == mode || "dataentry" == mode)
            {
                InterfaceName = mode;
                FormToolbarEnabled = "false";
            }
            else
            {
                InterfaceName = "standard";
            }
        }
        else if ("none" == InterfaceName)
        {
            MinimalFeatureSet = false;
            FormToolbarEnabled = "false";
        }
        else if ("minimal" == InterfaceName || "calendar" == InterfaceName || "task" == InterfaceName)
        {
            if (strButtons.Length > 0 && true == options.ContainsKey("wmv"))
            {
                MinimalFeatureSet = false;
            }
            else
            {
                MinimalFeatureSet = true;
            }
            FormToolbarEnabled = "false";
        }

        if (settings_data.EnableFontButtons || true == options.ContainsKey("fontmenu"))
        {
            bEnableFontButtons = true;
        }

        string strFolder;
        strFolder = Request.QueryString["folder"];
        if (strFolder != null)
        {
            if (Information.IsNumeric(strFolder))
            {
                folder = Convert.ToInt64(strFolder);
            }
        }
        Ektron.Cms.LibraryConfigData lib_settings;
        extensions = "";
        try
        {
            // Only make this call if we are logged in
            Ektron.Cms.ContentAPI refContentApi = new Ektron.Cms.ContentAPI();
            Ektron.Cms.Content.EkContent refContent;
            refContent = refContentApi.EkContentRef;

            if (refContent.IsAllowed(0, 0, "users", "IsLoggedIn", 0))
            {
                // An exception will be thrown if the user is not authenticated.
                lib_settings = refContApi.GetLibrarySettings(folder);
                if (lib_settings != null)
                {
                    extensions = lib_settings.ImageExtensions + "," + lib_settings.FileExtensions;
                }
            }
        }
        catch (Exception)
        {
            // ignore error
        }

        ContentLanguage = refContApi.RequestInformationRef.ContentLanguage;

        PopulateDataLists(refContApi);
    }

    private void PopulateDataLists(Ektron.Cms.ContentAPI refContApi)
    {
        try
        {
            if ("datadesign" == mode || "formdesign" == mode || "xsltdesign" == mode || "dataentry" == mode)
            {
                Ektron.Cms.CollectionListData objCmsCollection;
                Collection colCollection;
                Collection colItems;

                string strErrorMessage;
                int nLangType;
                int nItemType;
                string strItemContent;
                long nListID;
                string strListName;
                string strCollectionName;
                bool bForChoice;
                bool bForList;
                string strListChoice;
                string strDataList;
                StringBuilder sbChoiceFld = new StringBuilder();
                StringBuilder sbListFld = new StringBuilder();
                StringBuilder sbDataLists = new StringBuilder();

                //refContApi.GetAllXmlConfigurations("")
                //refContApi.GetXmlConfiguration(0)
                // GetAllContentByXmlConfigID doesn't do what is says
                //refContApi.EkContentRef.GetAllContentByXmlConfigID(0)

                // Find collection named "CMS Data Lists"
                objCmsCollection = GetCollectionByTitle(refContApi, "CMS Data Lists");
                if (objCmsCollection == null)
                {
                    return;
                }
                strErrorMessage = "";
                colCollection = refContApi.EkContentRef.GetEcmCollectionByID(0, true, false, ref strErrorMessage, "", false, false);
                if (strErrorMessage.Length > 0)
                {
                    return;
                }

                nLangType = refContApi.RequestInformationRef.ContentLanguage;
                colItems = (Collection)colCollection["Contents"];
                foreach (Collection colItem in colItems)
                {
                    nItemType = Convert.ToInt32(colItem["ContentType"]);
                    if (1 == nItemType) // 1 = content
                    {
                        strItemContent = colItem["ContentHtml"].ToString();
                        if (strItemContent.Contains("<CmsDataList") && strItemContent.Contains("<ol"))
                        {
                            nListID = Convert.ToInt64(colItem["ContentID"]);
                            strCollectionName = colItem["ContentTitle"].ToString();
                            strListName = string.Format("cms{0:d}", nListID);
                            switch (mode)
                            {
                                case "datadesign":
                                    bForChoice = strItemContent.Contains("DataDesignChoiceFld");
                                    bForList = strItemContent.Contains("DataDesignListFld");
                                    break;
                                case "formdesign":
                                    bForChoice = strItemContent.Contains("FormDesignChoiceFld");
                                    bForList = strItemContent.Contains("FormDesignListFld");
                                    break;
                                default:
                                    bForChoice = false;
                                    bForList = false;
                                    break;
                            }
                            if (bForChoice || bForList)
                            {
                                // <listchoice data="{list-name}">{display-text}</listchoice>
                                strListChoice = string.Format("<listchoice data=\"{0}\">{1}</listchoice>", strListName, strCollectionName);
                                if (bForChoice)
                                {
                                    sbChoiceFld.AppendLine(strListChoice);
                                }
                                if (bForList)
                                {
                                    sbListFld.AppendLine(strListChoice);
                                }
                            }
                            // <datalist name="{list-name}" src="{AppeWebPath}cmsdatalist.aspx?id={id}" cache="false" select="/CmsDataList/ol/li" captionxpath="." valuexpath="@title" />
                            strDataList = string.Format("<datalist name=\"{0}\" src=\"{1}cmsdatalist.aspx?id={2:d}&amp;LangType={3:d}\" cache=\"false\" select=\"/CmsDataList/ol/li\" captionxpath=\".\" valuexpath=\"@title\" />", strListName, AppeWebPath, nListID, nLangType);
                            sbDataLists.AppendLine(strDataList);
                        }
                    }
                }

                switch (mode)
                {
                    case "datadesign":
                        DataDesignChoiceFld.Text = sbChoiceFld.ToString();
                        DataDesignListFld.Text = sbListFld.ToString();
                        break;
                    case "formdesign":
                        FormDesignChoiceFld.Text = sbChoiceFld.ToString();
                        FormDesignListFld.Text = sbListFld.ToString();
                        break;
                }
                DataLists.Text = sbDataLists.ToString();
            }
        }
        catch (Exception)
        {
            // ignore
        }
    }

    private Ektron.Cms.CollectionListData GetCollectionByTitle(Ektron.Cms.ContentAPI refContApi, string Title)
    {
        try
        {
            Ektron.Cms.CollectionListData[] objCollectionList;
            int iCol;
            objCollectionList = refContApi.EkContentRef.GetCollectionList();
            if (objCollectionList == null)
            {
                return null;
            }
            for (iCol = 0; iCol <= (objCollectionList.Length - 1); iCol++)
            {
                if (Title == objCollectionList[iCol].Title)
                {
                    return objCollectionList[iCol];
                }
            }
        }
        catch (Exception)
        {
            // ignore
        }
        return null;
    }

    private string LCaseQueryString(string Name)
    {
        string strValue;
        strValue = Request.QueryString[Name];
        if (strValue == null)
        {
            strValue = "";
        }
        this.ValidateQueryString(strValue);
        return strValue.ToLower();
    }

    private void ValidateQueryString(string queryString)
    {
        queryString = queryString.ToUpper();
        queryString = Regex.Replace(queryString, @"\/\*[\w\W]*?\*\/", "");
        if ((queryString.IndexOf("<") > -1) || (queryString.IndexOf("%3C") > -1) || (queryString.IndexOf(">") > -1) || (queryString.IndexOf("%3E") > -1) || (queryString.IndexOf("\"") > -1) || (queryString.IndexOf("%22") > -1) || (queryString.IndexOf(":EXPRESSION(") > -1) || (queryString.IndexOf("JAVASCRIPT:") > -1))
        {
            throw new ArgumentException("Invalid Query String Value");
        }
    }
}
	

