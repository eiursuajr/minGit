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
using System.Xml.Xsl;
using Ektron.Cms;
using Ektron.Cms.Workarea.Dms;
using Ektron.Cms.Common;

public partial class Workarea_DmsMenu : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        //This document should never be cached.  When content status changes
        //the page is refreshed to show change in status.  This change does 
        //not appear when page is cached.
        Response.Cache.SetCacheability(HttpCacheability.NoCache);

        CommonApi myCommonApi = new CommonApi();
        if (myCommonApi.IsLoggedIn == true)
        {
            this.ProcessMenuRequest();
        }
        else
        {
            this.RejectMenuRequest();
        }
    }

    private void RejectMenuRequest()
    {
        Ektron.Cms.ContentAPI myContent = new Ektron.Cms.ContentAPI();
        int languageId = myContent.DefaultContentLanguage; ;
        string rejectionMessage = String.Empty;

        if (Request.QueryString["dmsLanguageId"] != null)
        {
            if (Request.QueryString["dmsLanguageId"] != String.Empty)
            {
                languageId = Convert.ToInt32(Request.QueryString["dmsLanguageId"]);
            }
        }
        EkMessageHelper messageHelper = myContent.EkMsgRef;
        DMSMenu.Text = "-1|" + messageHelper.GetMessageForLanguage("DmsMenuNotLoggedIn", languageId);
    }

    private void ProcessMenuRequest()
    {
        Ektron.Cms.ContentAPI myContent = new Ektron.Cms.ContentAPI();
        Ektron.Cms.CommonApi AppUI = new Ektron.Cms.CommonApi();
        Ektron.Cms.API.User.User myUser = new Ektron.Cms.API.User.User();

        long contentId = Convert.ToInt64(Request.QueryString["contentId"]);
        int languageId = -1;
        long taxonomyOverrideId = 0;
        if (Request.QueryString["dmsLanguageId"] != null)
        {
            if (Request.QueryString["dmsLanguageId"] != String.Empty)
            {
                languageId = Convert.ToInt32(Request.QueryString["dmsLanguageId"]);
                if (languageId == Ektron.Cms.Common.EkConstants.CONTENT_LANGUAGES_UNDEFINED || languageId == Ektron.Cms.Common.EkConstants.ALL_CONTENT_LANGUAGES)
                {
                    languageId = AppUI.DefaultContentLanguage;
                }
                AppUI.ContentLanguage = languageId;
                myContent.ContentLanguage = languageId;
            }
        }

        //createIeSpecificMenu is an argument passed to the constructor of the DMSMenuContentAPI class
        //This arg tells DMSMenuContentAPI whether or not to include the IE specific functionality
        //namely, this funcitonality is the office-specific stuff (Edit In Microsoft Office, View in Microsoft Office).
        Boolean createIeSpecificMenu = false;
        if (Request.QueryString["createIeSpecificMenu"] != String.Empty)
        {
            createIeSpecificMenu = Convert.ToBoolean(Request.QueryString["createIeSpecificMenu"]);
        }


        string dmsMenuType = String.Empty;
        string dmsMenuSubtype = String.Empty;
        long communityGroupID = 0;

        if (!String.IsNullOrEmpty(Request.QueryString["communityDocuments"]))
        {
            // Dms community menus may carry additional data (i.e. group id).
            string[] splitDmsMenuTypeInfo = Request.QueryString["communityDocuments"].Split(
                new char[] { '_' },
                StringSplitOptions.RemoveEmptyEntries);

            dmsMenuType = splitDmsMenuTypeInfo[0];

            if (splitDmsMenuTypeInfo.Length == 2)
            {
                long.TryParse(splitDmsMenuTypeInfo[1], out communityGroupID);
            }
        }


        Ektron.Cms.ContentData myContentData = new Ektron.Cms.ContentData();
        Ektron.Cms.API.Content.Content apiCont = new Ektron.Cms.API.Content.Content();
        Ektron.Cms.ContentStateData stateData = apiCont.GetContentState(contentId);
        if (stateData.Status == "A")
            myContentData = apiCont.GetContent(contentId, Ektron.Cms.ContentAPI.ContentResultType.Published);
        else
            myContentData = apiCont.GetContent(contentId, Ektron.Cms.ContentAPI.ContentResultType.Staged);

        string dmsMenuGuid;
        XsltArgumentList myDMSMenuArguments = new XsltArgumentList();
        Ektron.Cms.Workarea.Dms.DmsMenu myDMSMenu;
        if (dmsMenuType != "")
        {
            bool queryDynamicContentBox = false;
            string controlID = String.Empty;

            if (Request.QueryString["dynamicContentBox"] != null)
            {
                queryDynamicContentBox = Convert.ToBoolean(Request.QueryString["DynamicContentBox"]);
                if (dmsMenuType.ToLower() == "communityuser" || dmsMenuType.ToLower() == "communitygroup" && EkConstants.IsAssetContentType(myContentData.ContType, true) && myContentData.ContType !=(int) EkEnumeration.CMSContentType.Multimedia)
                {
                    if (myContentData.AssetData != null && !EkFunctions.IsImage("." + myContentData.AssetData.FileExtension) && myContentData.Type !=(int) EkEnumeration.CMSContentType.Multimedia)
                    {
                        queryDynamicContentBox = false;
                    }
                }
                
                myDMSMenuArguments.AddParam("dynamicContentBox", String.Empty, queryDynamicContentBox);
            }
            if (Request.QueryString["dmsEktControlID"] != null)
            {
                controlID = Request.QueryString["dmsEktControlID"].ToString();
                myDMSMenuArguments.AddParam("dmsEktControlID", String.Empty, controlID);
            }
            if (Request.QueryString["taxonomyOverrideId"] != null)
            {
                taxonomyOverrideId = Convert.ToInt64(Request.QueryString["taxonomyOverrideId"]);
            }
        }

        if (Request.QueryString["dmsMenuGuid"] != null)
        {
            dmsMenuGuid = Request.QueryString["dmsMenuGuid"].ToString();
            myDMSMenuArguments.AddParam("dmsMenuGuid", String.Empty, dmsMenuGuid);
        }

        Boolean IsPhotoGallery = true;
        if (Request.QueryString["dmsMenuSubtype"] != null)
        {
            dmsMenuSubtype = Request.QueryString["dmsMenuSubtype"].ToString();
            myDMSMenuArguments.AddParam("dmsMenuSubtype", String.Empty, dmsMenuSubtype);
            IsPhotoGallery = (dmsMenuSubtype == "photo");
        }

        string fromPage = String.Empty;
        if (Request.QueryString["fromPage"] != null)
        {
            fromPage = Request.QueryString["fromPage"].ToString();
            myDMSMenuArguments.AddParam("fromPage", String.Empty, fromPage);
        }


        switch (dmsMenuType.ToLower())
        {
            case "taxonomy":
                //Taxonomy Implementation
                //Use Taxnonomy constructor overload
                //public DmsMenu(ektronDmsMenuMenuType menuType, int contentId, int userId, int contentLanguage, int folderId, int contentType, Boolean createIESpecificMenu, int taxonomyOverrideId)
                if (Request.QueryString["communityGroupid"] != null)
                {
                    long.TryParse(Request.QueryString["communityGroupid"].ToString(), out communityGroupID);
                }
                myDMSMenu = new Ektron.Cms.Workarea.Dms.DmsMenu(ektronDmsMenuMenuType.Taxonomy,
                    contentId, myUser.UserId, languageId, myContentData.FolderId,
                    myContentData.Type, createIeSpecificMenu, taxonomyOverrideId, communityGroupID);
                break;
            case "communityuser":
                //Community User Implementation
                //Use Community User constructor overload
                //public DmsMenu(ektronDmsMenuMenuType menuType, int contentId, int userId, int contentLanguage, int folderId, int contentType, Boolean createIESpecificMenu, Boolean isPhotoGallery)
                myDMSMenu = new Ektron.Cms.Workarea.Dms.DmsMenu(ektronDmsMenuMenuType.CommunityUser, 
                    contentId, myUser.UserId, languageId, myContentData.FolderId, 
                    myContentData.Type, createIeSpecificMenu, IsPhotoGallery);
                break;
            case "communitygroup":
                //Community Group Implementation
                //Use Community Group constructor overload
                //public DmsMenu(ektronDmsMenuMenuType menuType, int contentId, int userId, int contentLanguage, int folderId, int contentType, Boolean createIESpecificMenu, int taxonomyOverrideId, int communityGroupId, Boolean isPhotoGallery)
                myDMSMenu = new Ektron.Cms.Workarea.Dms.DmsMenu(ektronDmsMenuMenuType.CommunityGroup, 
                    contentId, myUser.UserId, languageId, myContentData.FolderId, myContentData.Type, 
                    createIeSpecificMenu, taxonomyOverrideId, communityGroupID, IsPhotoGallery, fromPage);
                break;
            case "favorites":
                //Favorites Menu Implementation
                //Use Favorites constructor overload
                //public DmsMenu(ektronDmsMenuMenuType menuType, int contentId, int userId, int contentLanguage, int folderId, int contentType, Boolean createIESpecificMenu, int taxonomyOverrideId, int communityGroupId, Boolean isPhotoGallery)
                myDMSMenu = new Ektron.Cms.Workarea.Dms.DmsMenu(ektronDmsMenuMenuType.Favorites, 
                    contentId, myUser.UserId, languageId, myContentData.FolderId, 
                    myContentData.Type, createIeSpecificMenu, taxonomyOverrideId,
                    communityGroupID, IsPhotoGallery, fromPage);
                break;
            case "workarea":
            default:
                //Workarea Implementation
                //Use Workarea constructor overload
                //public DmsMenu(ektronDmsMenuMenuType menuType, int contentId, int userId, int contentLanguage, int folderId, int contentType, Boolean createIESpecificMenu)
                myDMSMenu = new Ektron.Cms.Workarea.Dms.DmsMenu(ektronDmsMenuMenuType.Workarea, 
                    contentId, myUser.UserId, languageId, myContentData.FolderId, 
                    myContentData.Type, createIeSpecificMenu, fromPage);
                break;
        }
        
        DMSMenu.Text = myDMSMenu.GetDmsMenu(myDMSMenuArguments);
    }

    private ektronDmsMenuMenuType GetDmsMenuType(string dmsMenuType)
    {
        switch (dmsMenuType.ToLower())
        {
            case "communityuser":
                return ektronDmsMenuMenuType.CommunityUser;
            case "communitygroup":
                return ektronDmsMenuMenuType.CommunityGroup;
            case "taxonomy":
                return ektronDmsMenuMenuType.Taxonomy;
            case "favorites":
                return ektronDmsMenuMenuType.Favorites;
            default:
                return ektronDmsMenuMenuType.Workarea;
        }
    }
}
