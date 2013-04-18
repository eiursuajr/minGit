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
using Ektron.Cms;
using Ektron.Cms.Common;
using Ektron.Cms.DataIO.LicenseManager;

public partial class workareanavigationtrees : System.Web.UI.Page
{
    private Collection m_hashAllFolders;
    private ContentAPI m_refAPI = new ContentAPI();
    protected string TreeJS = "";
    private bool PerReadOnlyLib = false;
    private bool IsAdmin = false;
    private bool PerReadOnly = false;
    private long currentUserID = 1;
    protected ContentAPI m_refContentApi = new ContentAPI();
    private bool IsSystemAccount = false;

    private void Page_Load(System.Object sender, System.EventArgs e)
    {
        try
        {
            Response.CacheControl = "no-cache";
            Response.AddHeader("Pragma", "no-cache");
            Response.Expires = -1;
            currentUserID = m_refAPI.UserId;
            treeJsOutput.Text = GetClientScript();
			Utilities.ValidateUserLogin();
            // register CSS
            Ektron.Cms.API.Css.RegisterCss(this, Ektron.Cms.API.Css.ManagedStyleSheet.EktronWorkareaCss);
            Ektron.Cms.API.Css.RegisterCss(this, Ektron.Cms.API.Css.ManagedStyleSheet.EktronWorkareaIeCss, Ektron.Cms.API.Css.BrowserTarget.AllIE);

            // register JS
            Ektron.Cms.API.JS.RegisterJS(this, Ektron.Cms.API.JS.ManagedScript.EktronJS);
            Ektron.Cms.API.JS.RegisterJS(this, m_refAPI.AppPath + "java/ekfoldercontrol.js", "EktronFolderControlJS");
        }
        catch (Exception)
        {
        }
        finally
        {
        }
    }

    private string OutputFolders(int level, long parent, EkEnumeration.FolderTreeType FolderType)
    {
        Ektron.Cms.Common.EkEnumeration.FolderDestinationType[] DestType = new Ektron.Cms.Common.EkEnumeration.FolderDestinationType[1];
        string[] Link = new string[1];
        string[] DestName = new string[1];
        string[] ExtParams = new string[1];
        System.String result = "";
        Ektron.Cms.Content.EkContent objContentRef;
        try
        {
            objContentRef = m_refAPI.EkContentRef;
            if (FolderType == EkEnumeration.FolderTreeType.Content)
            {
                DestType[0] = EkEnumeration.FolderDestinationType.Frame; //EkContent.FolderDestinationType.Frame
                Link[0] = "content.aspx?action=ViewContentByCategory&id=";
                DestName[0] = "ek_main";
                ExtParams[0] = "";
                result = objContentRef.OutputFolders(level, parent, ref DestType, ref Link, ref DestName, ref ExtParams, ref m_hashAllFolders, EkEnumeration.FolderTreeType.Content);

            }
            else if (FolderType == EkEnumeration.FolderTreeType.Library)
            {
                DestType[0] = EkEnumeration.FolderDestinationType.Frame;
                Link[0] = "library.aspx?action=ViewLibraryByCategory&id=";
                DestName[0] = "ek_main";
                ExtParams[0] = "";
                result = objContentRef.OutputFolders(level, parent, ref DestType, ref Link, ref DestName, ref ExtParams, ref m_hashAllFolders, EkEnumeration.FolderTreeType.Library);
            }
        }
        catch (Exception)
        {
            result = "";
        }
        return (result);
    }

    private string GetClientScript()
    {
        System.Text.StringBuilder result;
        string TreeRequested = "content";
        PermissionData security_data;
        Ektron.Cms.Common.EkMessageHelper objMessage;
        Ektron.Cms.Content.EkContent objContentRef;
        Ektron.Cms.UrlAliasing.UrlAliasSettingsApi m_urlAliasSettings = new Ektron.Cms.UrlAliasing.UrlAliasSettingsApi();
        result = new System.Text.StringBuilder();

        try
        {
            objMessage = m_refAPI.EkMsgRef;
            security_data = m_refAPI.LoadPermissions(0, "folder", 0);
            if (!(security_data.IsLoggedIn))
            {
                ShowLoginError();
                //break;
                return string.Empty;
            }
            if (!(security_data == null))
            {
                IsAdmin = security_data.IsAdmin;
                PerReadOnly = security_data.IsReadOnly;
                PerReadOnlyLib = security_data.IsReadOnlyLib;
            }
            IsSystemAccount = System.Convert.ToBoolean(m_refAPI.UserId == Ektron.Cms.Common.EkConstants.BuiltIn);
            if (!(Request.QueryString["tree"] == null))
            {
                TreeRequested = Request.QueryString["tree"].ToLower().Trim();
            }
            TreeRequested = Strings.LCase(Request.QueryString["tree"]);
            objContentRef = m_refAPI.EkContentRef;
            result.AppendLine("<script type=\"text/javascript\">");
            result.AppendLine("<!--");
           
            if ((TreeRequested) == ("content"))
            {
                if (PerReadOnly)
                {
                    result.Append("var urlInfoArray = new Array(\"frame\", \"content.aspx?action=ViewContentByCategory&id=0\", \"ek_main\");" + "\r\n");
                    result.Append("TopTreeLevel = CreateFolderInstance(\"" + objMessage.GetMessage("generic content title") + "\", urlInfoArray);" + "\r\n");
                }
                else
                {
                    result.Append("TopTreeLevel = CreateFolderInstance(\"" + objMessage.GetMessage("generic content title") + "\", \"\");" + "\r\n");
                }
                m_hashAllFolders = objContentRef.GetFolderTreeForUserID(0);
                result.Append(OutputFolders(0, 0, Ektron.Cms.Common.EkEnumeration.FolderTreeType.Content) + "\r\n");
            }
            else if ((TreeRequested) == ("library"))
            {
                if (PerReadOnlyLib)
                {
                    result.Append("var urlInfoArray = new Array(\"frame\", \"library.aspx?action=ViewLibraryByCategory&id=0\", \"ek_main\");");
                    result.Append("TopTreeLevel = CreateFolderInstance(\"" + objMessage.GetMessage("generic Library title") + "\", urlInfoArray);");
                }
                else
                {
                    result.Append("TopTreeLevel = CreateFolderInstance(\"" + objMessage.GetMessage("generic Library title") + "\", \"\");");
                }

                m_hashAllFolders = objContentRef.GetFolderTreeForUserID(0);
                result.Append(OutputFolders(0, 0, Ektron.Cms.Common.EkEnumeration.FolderTreeType.Library) + "\r\n");
              

                // commerce
            }
            else if ((TreeRequested) == ("admin"))
            {
                //Merging Module and Settings under Settings tab for Version 8.0.
                result.Append("TopTreeLevel = CreateFolderInstance(\"" + objMessage.GetMessage("administrate button text") + "\", \"\");");
                if (!IsSystemAccount)
                {

                    //Commerce
                    if (Ektron.Cms.DataIO.LicenseManager.LicenseManager.IsFeatureEnable(m_refAPI.RequestInformationRef, Feature.eCommerce) && (IsAdmin || m_refAPI.IsARoleMember(Ektron.Cms.Common.EkEnumeration.CmsRoleIds.CommerceAdmin)))
                    {

                        result.Append("level1 = InsertFolder(TopTreeLevel, CreateFolderInstance(\"" + objMessage.GetMessage("lbl commerce") + "\", \"\"));");

                        result.Append("var urlInfoArray = new Array(\"frame\", \"\", \"ek_main\");");
                        result.Append("level2 = InsertFolder(level1, CreateFolderInstance(\"" + objMessage.GetMessage("lbl commerce catalog") + "\", urlInfoArray));");
                        result.Append("var urlInfoArray = new Array(\"frame\", \"Commerce/producttypes.aspx\", \"ek_main\");");
                        result.Append("InsertFile(level2, CreateLink(\"" + objMessage.GetMessage("lbl product types") + "\", urlInfoArray));");
                        result.Append("var urlInfoArray = new Array(\"frame\", \"Commerce/Coupons/List/List.aspx\", \"ek_main\");");
                        result.Append("InsertFile(level2, CreateLink(\"" + objMessage.GetMessage("lbl coupons") + "\", urlInfoArray));");

                        result.Append("var urlInfoArray = new Array(\"frame\", \"\", \"ek_main\");");
                        result.Append("level2 = InsertFolder(level1, CreateFolderInstance(\"" + objMessage.GetMessage("lbl commerce config") + "\", urlInfoArray));");
                        result.Append("var urlInfoArray = new Array(\"frame\", \"Commerce/locale/country.aspx\", \"ek_main\");");
                        result.Append("InsertFile(level2, CreateLink(\"" + objMessage.GetMessage("lbl wa countries") + "\", urlInfoArray));");
                        result.Append("var urlInfoArray = new Array(\"frame\", \"Commerce/currency.aspx\", \"ek_main\");");
                        result.Append("InsertFile(level2, CreateLink(\"" + objMessage.GetMessage("lbl wa currency") + "\", urlInfoArray));");
                        result.Append("var urlInfoArray = new Array(\"frame\", \"Commerce/creditcardtypes.aspx\", \"ek_main\");");
                        result.Append("InsertFile(level2, CreateLink(\"" + objMessage.GetMessage("lbl cc wa") + "\", urlInfoArray));");
                        result.Append("var urlInfoArray = new Array(\"frame\", \"subscriptionmessages.aspx?mode=commerce\", \"ek_main\");");
                        result.Append("InsertFile(level2, CreateLink(\"" + objMessage.GetMessage("lbl messages") + "\", urlInfoArray));");
                        result.Append("var urlInfoArray = new Array(\"frame\", \"Commerce/paymentgateway.aspx\", \"ek_main\");");
                        result.Append("InsertFile(level2, CreateLink(\"" + objMessage.GetMessage("lbl payment options") + "\", urlInfoArray));");
                        result.Append("var urlInfoArray = new Array(\"frame\", \"Commerce/locale/region.aspx\", \"ek_main\");");
                        result.Append("InsertFile(level2, CreateLink(\"" + objMessage.GetMessage("lbl wa regions") + "\", urlInfoArray));");
                        //result.Append("var urlInfoArray = new Array(\"frame\", \"Commerce/pricebook.aspx\", \"ek_main\");");
                        //result.Append("InsertFile(level2, CreateLink(\"" + objMessage.GetMessage("lbl wa pricebooks") + "\", urlInfoArray));");
						
                        result.Append("var urlInfoArray = new Array(\"frame\", \"\", \"ek_main\");");
                        result.Append("level2 = InsertFolder(level1, CreateFolderInstance(\"" + objMessage.GetMessage("lbl fulfillment") + "\", urlInfoArray));");
                        result.Append("var urlInfoArray = new Array(\"frame\", \"Commerce/fulfillment.aspx\", \"ek_main\");");
                        result.Append("InsertFile(level2, CreateLink(\"" + objMessage.GetMessage("lbl orders") + "\", urlInfoArray));");
                        result.Append("var urlInfoArray = new Array(\"frame\", \"Commerce/fulfillment/workflow.aspx\", \"ek_main\");");
                        result.Append("InsertFile(level2, CreateLink(\"" + objMessage.GetMessage("lbl order workflow") + "\", urlInfoArray));");

                        result.Append("var urlInfoArray = new Array(\"frame\", \"\", \"ek_main\");");
                        result.Append("level2 = InsertFolder(level1, CreateFolderInstance(\"" + objMessage.GetMessage("generic reports title") + "\", urlInfoArray));");
                        result.Append("var urlInfoArray = new Array(\"frame\", \"Commerce/reporting/CustomerReports.aspx?page=normal\", \"ek_main\");");
                        result.Append("InsertFile(level2, CreateLink(\"" + objMessage.GetMessage("lbl customer reports") + "\", urlInfoArray));");
                        result.Append("var urlInfoArray = new Array(\"frame\", \"Commerce/inventory.aspx\", \"ek_main\");");
                        result.Append("InsertFile(level2, CreateLink(\"" + objMessage.GetMessage("lbl inventory reports") + "\", urlInfoArray));");
						result.Append("var urlInfoArray = new Array(\"frame\", \"Commerce/reporting/KeyPerformanceIndicators.aspx\", \"ek_main\");");
                        result.Append("InsertFile(level2, CreateLink(\"" + objMessage.GetMessage("lbl key performance indicators") + "\", urlInfoArray));");
                        result.Append("var urlInfoArray = new Array(\"frame\", \"Commerce/reporting/PaymentReports.aspx\", \"ek_main\");");
                        result.Append("InsertFile(level2, CreateLink(\"" + objMessage.GetMessage("lbl payment reports") + "\", urlInfoArray));");
                        result.Append("var urlInfoArray = new Array(\"frame\", \"Commerce/reporting/SalesTrend.aspx\", \"ek_main\");");
                        result.Append("InsertFile(level2, CreateLink(\"" + objMessage.GetMessage("lbl sales trend") + "\", urlInfoArray));");
                        result.Append("var urlInfoArray = new Array(\"frame\", \"Commerce/reporting/TopProducts.aspx\", \"ek_main\");");
                        result.Append("InsertFile(level2, CreateLink(\"" + objMessage.GetMessage("lbl top products") + "\", urlInfoArray));");

                        result.Append("var urlInfoArray = new Array(\"frame\", \"\", \"ek_main\");");
                        result.Append("level2 = InsertFolder(level1, CreateFolderInstance(\"" + objMessage.GetMessage("lbl wa shipping") + "\", urlInfoArray));");
                        result.Append("var urlInfoArray = new Array(\"frame\", \"Commerce/shipping/shippingmethods.aspx\", \"ek_main\");");
                        result.Append("InsertFile(level2, CreateLink(\"" + objMessage.GetMessage("lbl wa shipping methods") + "\", urlInfoArray));");
                        result.Append("var urlInfoArray = new Array(\"frame\", \"Commerce/shipping/shippingsource.aspx\", \"ek_main\");");
                        result.Append("InsertFile(level2, CreateLink(\"" + objMessage.GetMessage("lbl wa warehouses") + "\", urlInfoArray));");
                        result.Append("var urlInfoArray = new Array(\"frame\", \"Commerce/shipping/packagesize.aspx\", \"ek_main\");");
                        result.Append("InsertFile(level2, CreateLink(\"" + objMessage.GetMessage("lbl wa packagesize") + "\", urlInfoArray));");

                        result.Append("var urlInfoArray = new Array(\"frame\", \"\", \"ek_main\");");
                        result.Append("level2 = InsertFolder(level1, CreateFolderInstance(\"" + objMessage.GetMessage("lbl wa tax") + "\", urlInfoArray));");
                        result.Append("var urlInfoArray = new Array(\"frame\", \"Commerce/tax/taxclass.aspx\", \"ek_main\");");
                        result.Append("InsertFile(level2, CreateLink(\"" + objMessage.GetMessage("lbl wa tax classes") + "\", urlInfoArray));");
                        result.Append("var urlInfoArray = new Array(\"frame\", \"Commerce/tax/postaltaxtables.aspx\", \"ek_main\");");
                        result.Append("InsertFile(level2, CreateLink(\"" + objMessage.GetMessage("lbl wa postal tax tables") + "\", urlInfoArray));");
                        result.Append("var urlInfoArray = new Array(\"frame\", \"Commerce/tax/taxtables.aspx\", \"ek_main\");");
                        result.Append("InsertFile(level2, CreateLink(\"" + objMessage.GetMessage("lbl wa tax tables") + "\", urlInfoArray));");
                        result.Append("var urlInfoArray = new Array(\"frame\", \"Commerce/tax/countrytaxtables.aspx\", \"ek_main\");");
                        result.Append("InsertFile(level2, CreateLink(\"" + objMessage.GetMessage("lbl wa country tax tables") + "\", urlInfoArray));");
						

                        // commerce folder links
                        result.Append("var urlInfoArray = new Array(\"frame\", \"Commerce/audit/audit.aspx\", \"ek_main\");");
                        result.Append("InsertFile(level1, CreateLink(\"" + objMessage.GetMessage("lbl audit") + "\", urlInfoArray));");
                        result.Append("var urlInfoArray = new Array(\"frame\", \"Commerce/customers.aspx\", \"ek_main\");");
                        result.Append("InsertFile(level1, CreateLink(\"" + objMessage.GetMessage("lbl customers") + "\", urlInfoArray));");
                    }
                    //Commerce Ends

                    //Community Management
                    result.Append("level1 = InsertFolder(TopTreeLevel, CreateFolderInstance(\"" + objMessage.GetMessage("lbl community management") + "\", \"\"));");

                    // flagging
                    result.Append("level2 = InsertFolder(level1, CreateFolderInstance(\"" + objMessage.GetMessage("lbl flagging") + "\", \"\"));");
                    if (IsAdmin)
                    {
                        result.Append("var urlInfoArray = new Array(\"frame\", \"contentflagging/flagsets.aspx?communityonly=true\", \"ek_main\");");
                        result.Append("InsertFile(level2, CreateLink(\"" + objMessage.GetMessage("wa tree community flag def") + "\", urlInfoArray));");
                    }
                    result.Append("var urlInfoArray = new Array(\"frame\", \"reports.aspx?action=ContentFlags\", \"ek_main\");");
                    result.Append("InsertFile(level2, CreateLink(\"" + objMessage.GetMessage("lbl flagged content") + "\", urlInfoArray));");
                    if (IsAdmin)
                    {
                        result.Append("var urlInfoArray = new Array(\"frame\", \"contentflagging/flagsets.aspx\", \"ek_main\");");
                        result.Append("InsertFile(level2, CreateLink(\"" + objMessage.GetMessage("wa tree flag def") + "\", urlInfoArray));");
                    }

                    // memberships
                    if (Ektron.Cms.DataIO.LicenseManager.LicenseManager.IsFeatureEnable(m_refAPI.RequestInformationRef, Feature.MembershipUsers))
                    {
                        if (IsAdmin || m_refAPI.IsARoleMember(Ektron.Cms.Common.EkEnumeration.CmsRoleIds.AdminUsers) || m_refAPI.IsARoleMember(Ektron.Cms.Common.EkEnumeration.CmsRoleIds.CommunityAdmin))
                        {
                            result.Append("level2 = InsertFolder(level1, CreateFolderInstance(\"" + objMessage.GetMessage("lbl memberships") + "\", \"\"));");
                            result.Append("var urlInfoArray = new Array(\"frame\", \"users.aspx?backaction=viewallusers&LangType=" + m_refAPI.RequestInformationRef.ContentLanguage + "&action=viewallusers&grouptype=1&groupid=888888\", \"ek_main\");");
                            result.Append("InsertFile(level2, CreateLink(\"" + objMessage.GetMessage("generic Users") + "\", urlInfoArray));");
                            result.Append("var urlInfoArray = new Array(\"frame\", \"users.aspx?action=viewallgroups&LangType=" + m_refAPI.RequestInformationRef.ContentLanguage + "&grouptype=1\", \"ek_main\");");
                            result.Append("InsertFile(level2, CreateLink(\"" + objMessage.GetMessage("generic User Groups") + "\", urlInfoArray));");
                            result.Append("var urlInfoArray = new Array(\"frame\", \"users.aspx?backaction=viewallusers&LangType=" + m_refAPI.RequestInformationRef.ContentLanguage + "&action=viewallusers&grouptype=1&groupid=888888&ty=nonverify\", \"ek_main\");");
                            result.Append("InsertFile(level2, CreateLink(\"" + objMessage.GetMessage("generic Users not verified") + "\", urlInfoArray));");
                        }
                    }
                }

                if (Ektron.Cms.DataIO.LicenseManager.LicenseManager.IsFeatureEnable(m_refAPI.RequestInformationRef, Feature.SocialNetworking))
                {
                    if ((IsAdmin || m_refAPI.IsARoleMember(Ektron.Cms.Common.EkEnumeration.CmsRoleIds.CommunityAdmin)) && !IsSystemAccount)
                    {
                        //Notifications
                        result.Append("level2 = InsertFolder(level1, CreateFolderInstance(\"" + objMessage.GetMessage("lbl notifications") + "\", \"\"));");
                        result.Append("level3 = InsertFolder(level2, CreateFolderInstance(\"" + objMessage.GetMessage("lbl default preferences") + "\", \"\"));");
                        result.Append("var urlInfoArray = new Array(\"frame\", \"notifications/DefaultNotificationPreferences.aspx?mode=colleagues\", \"ek_main\");");
                        result.Append("InsertFile(level3, CreateLink(\"" + objMessage.GetMessage("lbl friends") + "\", urlInfoArray));");
                        result.Append("var urlInfoArray = new Array(\"frame\", \"notifications/DefaultNotificationPreferences.aspx?mode=groups\", \"ek_main\");");
                        result.Append("InsertFile(level3, CreateLink(\"" + objMessage.GetMessage("lbl groups") + "\", urlInfoArray));");
                        result.Append("var urlInfoArray = new Array(\"frame\", \"notifications/DefaultNotificationPreferences.aspx?mode=privacy\", \"ek_main\");");
                        result.Append("InsertFile(level3, CreateLink(\"" + objMessage.GetMessage("lbl privacy") + "\", urlInfoArray));");
                        result.Append("var urlInfoArray = new Array(\"frame\", \"notifications/ActivityTypes.aspx?mode=viewgrid\", \"ek_main\");");
                        result.Append("InsertFile(level2, CreateLink(\"" + objMessage.GetMessage("lbl activity types") + "\", urlInfoArray));");
                        result.Append("var urlInfoArray = new Array(\"frame\", \"notifications/agents.aspx?mode=viewgrid\", \"ek_main\");");
                        result.Append("InsertFile(level2, CreateLink(\"" + objMessage.GetMessage("lbl agent") + "\", urlInfoArray));");
                        result.Append("var urlInfoArray = new Array(\"frame\", \"notifications/notificationmessages.aspx?mode=viewnotificationmsggrid\", \"ek_main\");");
                        result.Append("InsertFile(level2, CreateLink(\"" + objMessage.GetMessage("lbl messages") + "\", urlInfoArray));");
                        result.Append("var urlInfoArray = new Array(\"frame\", \"notifications/settings.aspx?mode=view\", \"ek_main\");");
                        result.Append("InsertFile(level2, CreateLink(\"" + objMessage.GetMessage("administrate button text") + "\", urlInfoArray));");
                    }
                }

                if (!IsSystemAccount)
                {

                    // tags
                    result.Append("var urlInfoArray = new Array(\"frame\", \"Community/personaltags.aspx\", \"ek_main\");");
                    result.Append("level2 = InsertFolder(level1, CreateFolderInstance(\"" + objMessage.GetMessage("lbl tags") + "\", \"\"));");
                    result.Append("InsertFile(level2, CreateLink(\"" + objMessage.GetMessage("view all tags") + "\", urlInfoArray));");
                    if (IsAdmin)
                    {
                        result.Append("var urlInfoArray = new Array(\"frame\",\"Community/personaltags.aspx?action=viewdefaulttags&objectType=" + System.Convert.ToInt32(EkEnumeration.CMSObjectTypes.Content) + "\", \"ek_main\");");
                        result.Append("InsertFile(level2, CreateLink(\"" + objMessage.GetMessage("lbl default content tags") + "\", urlInfoArray));");
                        if (Ektron.Cms.DataIO.LicenseManager.LicenseManager.IsFeatureEnable(m_refAPI.RequestInformationRef, Feature.SocialNetworking))
                        {
                            result.Append("var urlInfoArray = new Array(\"frame\", \"Community/personaltags.aspx?action=viewdefaulttags&objectType=" + System.Convert.ToInt32(EkEnumeration.CMSObjectTypes.CommunityGroup) + "\", \"ek_main\");");
                            result.Append("InsertFile(level2, CreateLink(\"" + objMessage.GetMessage("lbl default group tags") + "\", urlInfoArray));");
                        }


                        result.Append("var urlInfoArray = new Array(\"frame\",\"Community/personaltags.aspx?action=viewdefaulttags&objectType=" + System.Convert.ToInt32(EkEnumeration.CMSObjectTypes.Library) + "\", \"ek_main\");");
                        result.Append("InsertFile(level2, CreateLink(\"" + objMessage.GetMessage("lbl default library tags") + "\", urlInfoArray));");
                        if (Ektron.Cms.DataIO.LicenseManager.LicenseManager.IsFeatureEnable(m_refAPI.RequestInformationRef, Feature.SocialNetworking))
                        {
                            result.Append("var urlInfoArray = new Array(\"frame\",\"Community/personaltags.aspx?action=viewdefaulttags&objectType=" + System.Convert.ToInt32(EkEnumeration.CMSObjectTypes.User) + "\", \"ek_main\");");
                            result.Append("InsertFile(level2, CreateLink(\"" + objMessage.GetMessage("lbl default user tags") + "\", urlInfoArray));");
                        }
                    }

                    // community groups
                    if (Ektron.Cms.DataIO.LicenseManager.LicenseManager.IsFeatureEnable(m_refAPI.RequestInformationRef, Feature.SocialNetworking))
                    {
                        result.Append("var urlInfoArray = new Array(\"frame\", \"Community/groups.aspx?action=viewallgroups\", \"ek_main\");");
                        result.Append("InsertFile(level1, CreateLink(\"" + objMessage.GetMessage("lbl groups") + "\", urlInfoArray));");
                    }

                    if (IsAdmin)
                    {
                        // messages
                        result.Append("var urlInfoArray = new Array(\"frame\", \"subscriptionmessages.aspx?mode=userprop\", \"ek_main\");");
                        result.Append("InsertFile(level1, CreateLink(\"" + objMessage.GetMessage("lbl messages") + "\", urlInfoArray));");
                    }
                    if (IsAdmin || objContentRef.IsARoleMember(Convert.ToInt64(Ektron.Cms.Common.EkEnumeration.CmsRoleIds.MessageBoardAdmin), currentUserID, false))
                    {
                        // message board
                        result.Append("var urlInfoArray = new Array(\"frame\", \"Community/messageboard.aspx\", \"ek_main\");");
                        result.Append("InsertFile(level1, CreateLink(\"" + objMessage.GetMessage("lbl msg board") + "\", urlInfoArray));");
                    }


                    // reviews
                    result.Append("var urlInfoArray = new Array(\"frame\", \"reports.aspx?action=ContentReviews\", \"ek_main\");");
                    result.Append("InsertFile(level1, CreateLink(\"" + objMessage.GetMessage("lbl reviews") + "\", urlInfoArray));");

                    // templates
                    if (Ektron.Cms.DataIO.LicenseManager.LicenseManager.IsFeatureEnable(m_refAPI.RequestInformationRef, Feature.SocialNetworking))
                    {
                        if (IsAdmin)
                        {
                            result.Append("var urlInfoArray = new Array(\"frame\",\"Community/communitytemplates.aspx?action=view\", \"ek_main\");");
                            result.Append("InsertFile(level1, CreateLink(\"" + objMessage.GetMessage("lbl templates") + "\", urlInfoArray));");
                        }
                    }
                    //Community Management Ends
                }

                //Configuration
                if (IsAdmin)
                {
                    result.Append("level1 = InsertFolder(TopTreeLevel, CreateFolderInstance(\"" + objMessage.GetMessage("config page html title") + "\", \"\"));");
                    SiteAPI m_refSiteApi = new SiteAPI();
                    SettingsData settings_data;
                    settings_data = m_refSiteApi.GetSiteVariables(m_refSiteApi.UserId);
                    if (!(settings_data == null))
                    {
                        if (settings_data.IsAdInstalled)
                        {
                            result.Append("level2 = InsertFolder(level1, CreateFolderInstance(\"" + objMessage.GetMessage("generic Active Directory") + "\", \"\"));");
                            if (m_refSiteApi.RequestInformationRef.ADAdvancedConfig == true)
                            {
                                result.Append("var urlInfoArray = new Array(\"frame\", \"AD/ADDomains.aspx\", \"ek_main\");");
                                result.Append("InsertFile(level2, CreateLink(\"" + objMessage.GetMessage("generic ad domains") + "\", urlInfoArray));");
                            }
                            result.Append("var urlInfoArray = new Array(\"frame\", \"adconfigure.aspx\", \"ek_main\");");
                            result.Append("InsertFile(level2, CreateLink(\"" + objMessage.GetMessage("generic Setup") + "\", urlInfoArray));");
                            result.Append("var urlInfoArray = new Array(\"frame\", \"adreports.aspx?action=ViewAllReportTypes\", \"ek_main\");");
                            result.Append("InsertFile(level2, CreateLink(\"" + objMessage.GetMessage("generic Status") + "\", urlInfoArray));");
                        }
                    }
                    if (!IsSystemAccount)
                    {
                        if (System.Configuration.ConfigurationSettings.AppSettings["ek_Dictionary"] != null && Convert.ToBoolean(System.Configuration.ConfigurationSettings.AppSettings["ek_Dictionary"]))
                        {
                            // Dictionary
                            result.Append("level2 = InsertFolder(level1, CreateFolderInstance(\"Dictionary\", \"\"));");
                            result.Append("var urlInfoArray = new Array(\"frame\", \"dictionary/addnew.aspx\", \"ek_main\");");
                            result.Append("InsertFile(level2, CreateLink(\"Add new dictionary\", urlInfoArray));");

                            ICmsDictionary dictionaryManager = ObjectFactory.GetCmsDictionary();
                            Criteria<CmsDictionaryProperty> dictionaryCriteria = new Criteria<CmsDictionaryProperty>();
                            dictionaryCriteria.OrderByDirection = EkEnumeration.OrderByDirection.Ascending;
                            dictionaryCriteria.OrderByField = CmsDictionaryProperty.Name;
                            List<CmsDictionaryData> dictionaryList = dictionaryManager.GetList(dictionaryCriteria);
                            for (int i = 0; i < dictionaryList.Count; i++)
                            {
                                result.Append("var urlInfoArray = new Array(\"frame\", \"dictionary/editvalues.aspx?id=" + dictionaryList[i].Id + "\", \"ek_main\");");
                                result.Append("InsertFile(level2, CreateLink(\"" + dictionaryList[i].Name + "\", urlInfoArray));");
                            }
                            // Dictionary
                        }

                        result.Append("level2 = InsertFolder(level1, CreateFolderInstance(\"" + objMessage.GetMessage("lbl web alert properties") + "\", \"\"));");
                        // email from list
                        result.Append("var urlInfoArray = new Array(\"frame\", \"subscriptionemailfromlist.aspx\", \"ek_main\");");
                        result.Append("InsertFile(level2, CreateLink(\"" + objMessage.GetMessage("lbl subscription emailfrom properties") + "\", urlInfoArray));");
                        // messages
                        result.Append("var urlInfoArray = new Array(\"frame\", \"subscriptionmessages.aspx\", \"ek_main\");");
                        result.Append("InsertFile(level2, CreateLink(\"" + objMessage.GetMessage("lbl subscription message properties") + "\", urlInfoArray));");
                        // subscriptions
                        result.Append("var urlInfoArray = new Array(\"frame\", \"subscriptions.aspx?action=ViewAllSubscriptions\", \"ek_main\");");
                        result.Append("InsertFile(level2, CreateLink(\"" + objMessage.GetMessage("lbl subscription properties") + "\", urlInfoArray));");

                        //search properties
                        result.Append("level2 = InsertFolder(level1, CreateFolderInstance(\"" + objMessage.GetMessage("generic search") + "\", \"\"));");
                        result.Append("var urlInfoArray = new Array(\"frame\", \"search/suggestedresults.aspx?action=ViewSuggestedResults\", \"ek_main\");");
                        result.Append("InsertFile(level2, CreateLink(\"" + objMessage.GetMessage("lbl suggested results") + "\", urlInfoArray));");
                        result.Append("var urlInfoArray = new Array(\"frame\", \"search/status.aspx?action=ViewSearchStatus\", \"ek_main\");");
                        result.Append("InsertFile(level2, CreateLink(\"" + objMessage.GetMessage("lbl search status") + "\", urlInfoArray));");
                        result.Append("var urlInfoArray = new Array(\"frame\", \"search/synonyms.aspx?action=ViewSynonyms\", \"ek_main\");");
                        result.Append("InsertFile(level2, CreateLink(\"" + objMessage.GetMessage("lbl synonyms") + "\", urlInfoArray));");
                        result.Append("var urlInfoArray = new Array(\"frame\", \"search/mappings.aspx\", \"ek_main\");");
                        result.Append("InsertFile(level2, CreateLink(\"" + objMessage.GetMessage("lbl integrated search mappings") + "\", urlInfoArray));");

                        //forum properties
                        result.Append("level2 = InsertFolder(level1, CreateFolderInstance(\"" + objMessage.GetMessage("lbl disc boards") + "\", \"\"));");
                        result.Append("var urlInfoArray = new Array(\"frame\", \"threadeddisc/replacewords.aspx?isemoticon=1\", \"ek_main\");");
                        result.Append("InsertFile(level2, CreateLink(\"" + objMessage.GetMessage("lbl emoticons") + "\", urlInfoArray));");
                        result.Append("var urlInfoArray = new Array(\"frame\", \"subscriptionmessages.aspx?mode=forum\", \"ek_main\");");
                        result.Append("InsertFile(level2, CreateLink(\"" + objMessage.GetMessage("lbl subscription message properties") + "\", urlInfoArray));");
                        result.Append("var urlInfoArray = new Array(\"frame\", \"threadeddisc/replacewords.aspx\", \"ek_main\");");
                        result.Append("InsertFile(level2, CreateLink(\"" + objMessage.GetMessage("lbl replace words") + "\", urlInfoArray));");
                        result.Append("var urlInfoArray = new Array(\"frame\", \"threadeddisc/restrictIP.aspx\", \"ek_main\");");
                        result.Append("InsertFile(level2, CreateLink(\"" + objMessage.GetMessage("lbl restricted ips") + "\", urlInfoArray));");
                        result.Append("var urlInfoArray = new Array(\"frame\", \"threadeddisc/userranks.aspx\", \"ek_main\");");
                        result.Append("InsertFile(level2, CreateLink(\"" + objMessage.GetMessage("lbl user ranks") + "\", urlInfoArray));");

                        //Url Aliasing 7.6
                        //Licensing For 7.6
                        if (Ektron.Cms.DataIO.LicenseManager.LicenseManager.IsFeatureEnable(m_refAPI.RequestInformationRef, Ektron.Cms.DataIO.LicenseManager.Feature.UrlAliasing, false))
                        {

                            result.Append("level2 = InsertFolder(level1, CreateFolderInstance(\"" + objMessage.GetMessage("lbl navtree urlaliasing") + "\", \"\"));");
                            // automatic
                            result.Append("var urlInfoArray = new Array(\"frame\", \"urlmanualaliaslistmaint.aspx?mode=auto\", \"ek_main\");");
                            result.Append("InsertFile(level2, CreateLink(\"" + objMessage.GetMessage("lbl tree url automatic aliasing") + "\", urlInfoArray));");
                            //community
                            result.Append("var urlInfoArray = new Array(\"frame\", \"urlmanualaliaslistmaint.aspx?mode=community\", \"ek_main\");");
                            result.Append("InsertFile(level2, CreateLink(\"" + objMessage.GetMessage("community text") + "\", urlInfoArray));");
                            // manual
                            result.Append("var urlInfoArray = new Array(\"frame\", \"urlmanualaliaslistmaint.aspx\", \"ek_main\");");
                            result.Append("InsertFile(level2, CreateLink(\"" + objMessage.GetMessage("lbl tree url manual aliasing") + "\", urlInfoArray));");
                            // regex
                            result.Append("var urlInfoArray = new Array(\"frame\", \"urlRegExAliaslistMaint.aspx\", \"ek_main\");");
                            result.Append("InsertFile(level2, CreateLink(\"" + objMessage.GetMessage("lbl tree url regex aliasing") + "\", urlInfoArray));");
                            // settings
                            result.Append("var urlInfoArray = new Array(\"frame\", \"urlaliassettings.aspx?action=view\", \"ek_main\");");
                            result.Append("InsertFile(level2, CreateLink(\"" + objMessage.GetMessage("administrate button text") + "\", urlInfoArray));");
                            //End If
                        }
                       
                            //personalization
                            result.Append("level2 = InsertFolder(level1, CreateFolderInstance(\"" + objMessage.GetMessage("lbl personalizations") + "\", \"\"));");
                            if (Ektron.Cms.DataIO.LicenseManager.LicenseManager.IsFeatureEnable(m_refAPI.RequestInformationRef, Ektron.Cms.DataIO.LicenseManager.Feature.Personalization, false))
                            {
                                result.Append("var urlInfoArray = new Array(\"frame\", \"widgetsettings.aspx?action=widgetspace\", \"ek_main\");");
                                result.Append("InsertFile(level2, CreateLink(\"" + objMessage.GetMessage("lbl widgets space") + "\", urlInfoArray));");
                            }
                            result.Append("var urlInfoArray = new Array(\"frame\", \"widgetsettings.aspx?action=widgetsync\", \"ek_main\");");
                            result.Append("InsertFile(level2, CreateLink(\"" + objMessage.GetMessage("lbl widgets") + "\", urlInfoArray));");

                            result.Append("var urlInfoArray = new Array(\"frame\", \"TargetedContent/targetcontentlist.aspx\", \"ek_main\");");
                            result.Append("InsertFile(level2, CreateLink(\"" + objMessage.GetMessage("lbl targeted content") + "\", urlInfoArray));");

                        //replication
                        if (m_refAPI.EnableReplication)
                        {
                            result.Append("var urlInfoArray = new Array(\"frame\", \"DynReplication.aspx\", \"ek_main\");");
                            result.Append("InsertFile(level1, CreateLink(\"" + objMessage.GetMessage("lbl navtree quickdeploy") + "\", urlInfoArray));");
                        }
                    }

                    // asset server setup
                    result.Append("var urlInfoArray = new Array(\"frame\", \"assetconfig.aspx\", \"ek_main\");");
                    result.Append("InsertFile(level1, CreateLink(\"" + objMessage.GetMessage("lbl asset server type") + "\", urlInfoArray));");
                    // custom properties
                    result.Append("var urlInfoArray = new Array(\"frame\", \"users.aspx?action=ViewCustomProp\", \"ek_main\");");
                    result.Append("InsertFile(level1, CreateLink(\"" + objMessage.GetMessage("custom properties") + "\", urlInfoArray));");
                    if (!IsSystemAccount)
                    {
						 // Devices
                        result.Append("var urlInfoArray = new Array(\"frame\", \"settings.aspx?action=viewalldeviceconfigurations\", \"ek_main\");");
                        result.Append("InsertFile(level1, CreateLink(\"" + objMessage.GetMessage("device configuration") + "\", urlInfoArray));");
                    }

                    if ((Convert.ToInt64(EkEnumeration.UserGroups.BuiltIn) != currentUserID) && (IsSystemAccount || IsAdmin))
                    {
                       if( System.IO.File.Exists(Server.MapPath(m_refAPI.AppPath + "sync/SyncDiag.aspx")))
                       {
                         result.Append("var urlInfoArray = new Array(\"frame\", \"sync/SyncDiag.aspx\", \"ek_main\");");
                         result.Append("InsertFile(level1, CreateLink(\"" + objMessage.GetMessage("lbl Diagnostics") + "\", urlInfoArray));");
                       }
                    }

                    if(!IsSystemAccount)
                    {
                        // fonts
                        result.Append("var urlInfoArray = new Array(\"frame\", \"font.aspx?action=ViewFontsByGroup\", \"ek_main\");");
                        result.Append("InsertFile(level1, CreateLink(\"" + objMessage.GetMessage("generic Fonts") + "\", urlInfoArray));");
                        
                        //integrated search folder (Hidden until this is supported in Search 4.0 - CB 1/4/11)
                        //result.Append("var urlInfoArray = new Array(\"frame\", \"IntegratedSearch.aspx?action=ViewAllIntegratedFolders\", \"ek_main\");");
                        //result.Append("InsertFile(level1, CreateLink(\"" + objMessage.GetMessage("lbl Integrated Search Folder") + "\", urlInfoArray));");

                                        //load balancing
                        if (m_refContentApi.GetAssetMgtConfigInfo()[System.Convert.ToInt32(AsetConfigType.LoadBalanced)].Value == "1")
                        {
                            result.Append("var urlInfoArray = new Array(\"frame\", \"loadbalancing.aspx\", \"ek_main\");");
                            result.Append("InsertFile(level1, CreateLink(\"" + objMessage.GetMessage("lbl load balancing") + "\", urlInfoArray));");
                        }

                        // metadata definition
                        result.Append("var urlInfoArray = new Array(\"frame\", \"meta_data50.aspx?action=ViewAllMetaDefinitions\", \"ek_main\");");
                        result.Append("InsertFile(level1, CreateLink(\"" + objMessage.GetMessage("meta_data page html title") + "\", urlInfoArray));");
                    }
                    
                    // setup
                    result.Append("var urlInfoArray = new Array(\"frame\", \"configure.aspx\", \"ek_main\");");
                    result.Append("InsertFile(level1, CreateLink(\"" + objMessage.GetMessage("generic Setup") + "\", urlInfoArray));");
                    if (!IsSystemAccount)
                    {
                        //smart form configurations
                        result.Append("var urlInfoArray = new Array(\"frame\", \"xml_config.aspx?action=ViewAllXmlConfigurations\", \"ek_main\");");
                        result.Append("InsertFile(level1, CreateLink(\"" + objMessage.GetMessage("generic XML Configurations") + "\", urlInfoArray));");
                        // synchronization
                        if (LicenseManager.IsFeatureEnable(m_refAPI.RequestInformationRef, Feature.eSync))
                        {
                            result.Append("var urlInfoArray = new Array(\"frame\", \"sync/Sync.aspx?action=viewallsync\", \"ek_main\");");
                            result.Append("InsertFile(level1, CreateLink(\"" + objMessage.GetMessage("lbl sync") + "\", urlInfoArray));");
                        }
                        // task types
                        result.Append("var urlInfoArray = new Array(\"frame\", \"tasks.aspx?action=ViewTaskType\", \"ek_main\");");
                        result.Append("InsertFile(level1, CreateLink(\"" + objMessage.GetMessage("lbl task types") + "\", urlInfoArray));");
                        // template configuration
                        result.Append("var urlInfoArray = new Array(\"frame\", \"template_config.aspx\", \"ek_main\");");
                        result.Append("InsertFile(level1, CreateLink(\"" + "" + objMessage.GetMessage("lbl Template Configuration") + "" + "\", urlInfoArray));");
                        //user property
                        
                    }
                }
                else
                {
                    result.Append("level1 = InsertFolder(TopTreeLevel, CreateFolderInstance(\"" + objMessage.GetMessage("config page html title") + "\", \"\"));");

                    // User is not part of administrator group, but has taxonomy administrator role, display Custom Property link with only Taxonomy Custom Property screen and hide the User option in the drop down list.

                    if (objContentRef.IsARoleMember(Convert.ToInt64(Ektron.Cms.Common.EkEnumeration.CmsRoleIds.TaxonomyAdministrator), currentUserID, false))
                    {                        
                        result.Append("var urlInfoArray = new Array(\"frame\", \"customproperties.aspx?action=viewall&type=1&objtype=0\", \"ek_main\");");
                        result.Append("InsertFile(level1, CreateLink(\"" + objMessage.GetMessage("custom properties") + "\", urlInfoArray));");
                    }
                    
                    if (objContentRef.IsARoleMember(Convert.ToInt64(Ektron.Cms.Common.EkEnumeration.CmsRoleIds.AdminPersonalize), currentUserID, false))
                    {
                         //personalization
                        result.Append("level2 = InsertFolder(level1, CreateFolderInstance(\"" + objMessage.GetMessage("lbl personalizations") + "\", \"\"));");
                        if (Ektron.Cms.DataIO.LicenseManager.LicenseManager.IsFeatureEnable(m_refAPI.RequestInformationRef, Ektron.Cms.DataIO.LicenseManager.Feature.Personalization, false))
                        {
                            result.Append("var urlInfoArray = new Array(\"frame\", \"widgetsettings.aspx?action=widgetspace\", \"ek_main\");");
                            result.Append("InsertFile(level2, CreateLink(\"" + objMessage.GetMessage("lbl widgets space") + "\", urlInfoArray));");
                        }
                        result.Append("var urlInfoArray = new Array(\"frame\", \"widgetsettings.aspx?action=widgetsync\", \"ek_main\");");
                        result.Append("InsertFile(level2, CreateLink(\"" + objMessage.GetMessage("lbl widgets") + "\", urlInfoArray));");

                        result.Append("var urlInfoArray = new Array(\"frame\", \"TargetedContent/targetcontentlist.aspx\", \"ek_main\");");
                        result.Append("InsertFile(level2, CreateLink(\"" + objMessage.GetMessage("lbl targeted content") + "\", urlInfoArray));");
                    }

                    if (objContentRef.IsARoleMember(Convert.ToInt64(Ektron.Cms.Common.EkEnumeration.CmsRoleIds.UrlAliasingAdmin), currentUserID, false))
                    {
                        if (Ektron.Cms.DataIO.LicenseManager.LicenseManager.IsFeatureEnable(m_refAPI.RequestInformationRef, Ektron.Cms.DataIO.LicenseManager.Feature.UrlAliasing, false))
                        {
                            result.Append("level2 = InsertFolder(level1, CreateFolderInstance(\"" + objMessage.GetMessage("lbl navtree urlaliasing") + "\", \"\"));");
                            result.Append("var urlInfoArray = new Array(\"frame\", \"urlaliassettings.aspx?action=view\", \"ek_main\");");
                            result.Append("InsertFile(level2, CreateLink(\"" + objMessage.GetMessage("administrate button text") + "\", urlInfoArray));");
                            //If (m_urlAliasSettings.IsManualAliasEnabled) Then
                            result.Append("var urlInfoArray = new Array(\"frame\", \"urlmanualaliaslistmaint.aspx\", \"ek_main\");");
                            result.Append("InsertFile(level2, CreateLink(\"" + objMessage.GetMessage("lbl tree url manual aliasing") + "\", urlInfoArray));");

                            result.Append("var urlInfoArray = new Array(\"frame\", \"urlmanualaliaslistmaint.aspx?mode=auto\", \"ek_main\");");
                            result.Append("InsertFile(level2, CreateLink(\"" + objMessage.GetMessage("lbl tree url automatic aliasing") + "\", urlInfoArray));");

                            //community
                            result.Append("var urlInfoArray = new Array(\"frame\", \"urlmanualaliaslistmaint.aspx?mode=community\", \"ek_main\");");
                            result.Append("InsertFile(level2, CreateLink(\"" + objMessage.GetMessage("community text") + "\", urlInfoArray));");

                            result.Append("var urlInfoArray = new Array(\"frame\", \"urlRegExAliaslistMaint.aspx\", \"ek_main\");");
                            result.Append("InsertFile(level2, CreateLink(\"" + objMessage.GetMessage("lbl tree url regex aliasing") + "\", urlInfoArray));");
                            //End If
                        }
                    }
                    //Search Admin to set Synonyms and Suggested Results
                    if (objContentRef.IsARoleMember(Convert.ToInt64(Ektron.Cms.Common.EkEnumeration.CmsRoleIds.SearchAdmin), currentUserID, false))
                    {
                        result.Append("level2 = InsertFolder(level1, CreateFolderInstance(\"" + objMessage.GetMessage("generic search") + "\", \"\"));");
                        result.Append("var urlInfoArray = new Array(\"frame\", \"search/suggestedresults.aspx?action=ViewSuggestedResults\", \"ek_main\");");
                        result.Append("InsertFile(level2, CreateLink(\"" + objMessage.GetMessage("lbl suggested results") + "\", urlInfoArray));");
                        result.Append("var urlInfoArray = new Array(\"frame\", \"search/status.aspx?action=ViewSearchStatus\", \"ek_main\");");
                        result.Append("InsertFile(level2, CreateLink(\"" + objMessage.GetMessage("lbl search status") + "\", urlInfoArray));");
                        result.Append("var urlInfoArray = new Array(\"frame\", \"search/synonyms.aspx?action=ViewSynonyms\", \"ek_main\");");
                        result.Append("InsertFile(level2, CreateLink(\"" + objMessage.GetMessage("lbl synonyms") + "\", urlInfoArray));");
                        result.Append("var urlInfoArray = new Array(\"frame\", \"search/mappings.aspx\", \"ek_main\");");
                        result.Append("InsertFile(level2, CreateLink(\"" + objMessage.GetMessage("lbl integrated search mappings") + "\", urlInfoArray));");
                    }
                    if (objContentRef.IsARoleMember(Convert.ToInt64(Ektron.Cms.Common.EkEnumeration.CmsRoleIds.AdminMetadata), currentUserID, false))
                    {
                        result.Append("var urlInfoArray = new Array(\"frame\", \"meta_data50.aspx?action=ViewAllMetaDefinitions\", \"ek_main\");");
                        result.Append("InsertFile(level1, CreateLink(\"" + objMessage.GetMessage("meta_data page html title") + "\", urlInfoArray));");
                    }
                    if (objContentRef.IsARoleMember(Convert.ToInt64(Ektron.Cms.Common.EkEnumeration.CmsRoleIds.SyncAdmin), currentUserID, false))
                    {
                        result.Append("var urlInfoArray = new Array(\"frame\", \"loadbalancing.aspx\", \"ek_main\");");
                        result.Append("InsertFile(level1, CreateLink(\"" + objMessage.GetMessage("lbl load balancing") + "\", urlInfoArray));");
                    }
                    if (objContentRef.IsARoleMember(Convert.ToInt64(Ektron.Cms.Common.EkEnumeration.CmsRoleIds.AdminXmlConfig), currentUserID, false))
                    {
                        result.Append("var urlInfoArray = new Array(\"frame\", \"xml_config.aspx?action=ViewAllXmlConfigurations\", \"ek_main\");");
                        result.Append("InsertFile(level1, CreateLink(\"" + objMessage.GetMessage("generic XML Configurations") + "\", urlInfoArray));");
                    }
                    
                    if (objContentRef.IsARoleMember(Convert.ToInt64(Ektron.Cms.Common.EkEnumeration.CmsRoleIds.TemplateConfigurations), currentUserID, false))
                    {
                        result.Append("var urlInfoArray = new Array(\"frame\", \"template_config.aspx\", \"ek_main\");");
                        result.Append("InsertFile(level1, CreateLink(\"" + objMessage.GetMessage("lbl Template Configuration") + "\", urlInfoArray));");
                    }                    
                    if (LicenseManager.IsFeatureEnable(m_refAPI.RequestInformationRef, Feature.eSync))
                    {
                        if (objContentRef.IsARoleMember(Convert.ToInt64(Ektron.Cms.Common.EkEnumeration.CmsRoleIds.SyncAdmin), currentUserID, false) || objContentRef.IsARoleMember(Convert.ToInt64(Ektron.Cms.Common.EkEnumeration.CmsRoleIds.SyncUser), currentUserID, false))
                        {
                            result.Append("var urlInfoArray = new Array(\"frame\", \"sync/Sync.aspx?action=viewallsync\", \"ek_main\");");
                            result.Append("InsertFile(level1, CreateLink(\"" + objMessage.GetMessage("lbl sync") + "\", urlInfoArray));");
                        }
                    }
                }
                //Configuration Ends.

                //Localization
                if (!IsSystemAccount)
                {
                    bool xliffEnabled = System.Convert.ToBoolean(m_refAPI.IsARoleMember(Ektron.Cms.Common.EkEnumeration.CmsRoleIds.AdminXliff) && Ektron.Cms.DataIO.LicenseManager.LicenseManager.IsFeatureEnable(m_refAPI.RequestInformationRef, Feature.Xliff));
                    if (xliffEnabled || IsAdmin)
                    {
                        result.Append("level1 = InsertFolder(TopTreeLevel, CreateFolderInstance(\"" + objMessage.GetMessage("lbl localization") + "\", \"\"));");
                        // Import XLIFF Files
                        if (xliffEnabled)
                        {
                            // dashboard
                            result.Append("var urlInfoArray = new Array(\"frame\", \"localization/Dashboard/CreateReport.aspx\", \"ek_main\");");
                            result.Append("InsertFile(level1, CreateLink(\"" + objMessage.GetMessage("sam dashboard") + "\", urlInfoArray));");
                            // export translation files
                            result.Append("var urlInfoArray = new Array(\"frame\", \"localization/exportjobs.aspx\", \"ek_main\");");
                            result.Append("InsertFile(level1, CreateLink(\"" + objMessage.GetMessage("lbl export for translation") + "\", urlInfoArray));");
                            // import translation files
                            result.Append("var urlInfoArray = new Array(\"frame\", \"localization/localization.aspx\", \"ek_main\");");
                            result.Append("InsertFile(level1, CreateLink(\"" + objMessage.GetMessage("lbl import translated files") + "\", urlInfoArray));");
                        }
                        // Languages and Regions
                        if (IsAdmin)
                        {
                            result.Append("var urlInfoArray = new Array(\"frame\", \"localization/languages.aspx\", \"ek_main\");");
                            result.Append("InsertFile(level1, CreateLink(\"" + objMessage.GetMessage("lbl language settings") + "\", urlInfoArray));");
                        }
                        if (xliffEnabled)
                        {
                            // Locale Taxonomy
                            result.Append("var urlInfoArray = new Array(\"frame\", \"Localization/localeTaxonomy.aspx\", \"ek_main\");");
                            result.Append("InsertFile(level1, CreateLink(\"" + objMessage.GetMessage("lbl translation packages") + "\", urlInfoArray));");}
                        }
                }
                //Roles
                if (IsAdmin && !IsSystemAccount)
                {
                    // Add roles:
                    result.Append("level1 = InsertFolder(TopTreeLevel, CreateFolderInstance(\"" + objMessage.GetMessage("lbl Roles") + "\", \"\"));");
                    result.Append("level2 = InsertFolder(level1, CreateFolderInstance(\"" + objMessage.GetMessage("lbl Built-In") + "\", \"\"));");
                    result.Append("level3 = InsertFolder(level2, CreateFolderInstance(\"" + objMessage.GetMessage("lbl System-Wide") + "\", \"\"));");
                    if (Ektron.Cms.DataIO.LicenseManager.LicenseManager.IsFeatureEnable(m_refAPI.RequestInformationRef, Feature.UrlAliasing))
                    {
                        result.Append("var urlInfoArray = new Array(\"frame\", \"roles.aspx?action=aliasedit&LangType=" + m_refAPI.RequestInformationRef.ContentLanguage + "\", \"ek_main\");");
                        result.Append("InsertFile(level3, CreateLink(\"" + objMessage.GetMessage("lbl Alias-Edit") + "\", urlInfoArray));");
                        result.Append("var urlInfoArray = new Array(\"frame\", \"roles.aspx?action=aliasadmin&LangType=" + m_refAPI.RequestInformationRef.ContentLanguage + "\", \"ek_main\");");
                        result.Append("InsertFile(level3, CreateLink(\"" + objMessage.GetMessage("lbl Alias-Admin") + "\", urlInfoArray));");
                    }
                    if (Ektron.Cms.DataIO.LicenseManager.LicenseManager.IsFeatureEnable(m_refAPI.RequestInformationRef, Feature.WebSiteAnalytics))
                    {
                        result.Append("var urlInfoArray = new Array(\"frame\", \"roles.aspx?action=analyticsviewer&LangType=" + m_refAPI.RequestInformationRef.ContentLanguage + "\", \"ek_main\");");
                        result.Append("InsertFile(level3, CreateLink(\"" + objMessage.GetMessage("lbl role analytics-viewer") + "\", urlInfoArray));");
                    }
                    result.Append("var urlInfoArray = new Array(\"frame\", \"roles.aspx?action=ruleedit&LangType=" + m_refAPI.RequestInformationRef.ContentLanguage + "\", \"ek_main\");");
                    result.Append("InsertFile(level3, CreateLink(\"" + objMessage.GetMessage("lbl Business Rule Editor") + "\", urlInfoArray));");
                    bool EnableClassicCalendar;
                    bool.TryParse(ConfigurationManager.AppSettings["ek_enableClassicCalendar"], out EnableClassicCalendar);
                    if (EnableClassicCalendar)
                    {
                        //Calendar
                        result.Append("var urlInfoArray = new Array(\"frame\", \"roles.aspx?action=calendaradmin&LangType=" + m_refAPI.RequestInformationRef.ContentLanguage + "\", \"ek_main\");");
                        result.Append("InsertFile(level3, CreateLink(\"" + objMessage.GetMessage("lbl Calendar-Admin") + "\", urlInfoArray));");
                        //Calendar Ends
                    }

                    result.Append("var urlInfoArray = new Array(\"frame\", \"roles.aspx?action=collectionmenuadmin&LangType=" + m_refAPI.RequestInformationRef.ContentLanguage + "\", \"ek_main\");");
                    result.Append("InsertFile(level3, CreateLink(\"" + objMessage.GetMessage("lbl Collection and Menu Admin") + "\", urlInfoArray));");

                    result.Append("var urlInfoArray = new Array(\"frame\", \"roles.aspx?action=collectionadmin&LangType=" + m_refAPI.RequestInformationRef.ContentLanguage + "\", \"ek_main\");");
                    result.Append("InsertFile(level3, CreateLink(\"" + objMessage.GetMessage("lbl collection admin") + "\", urlInfoArray));");

                    result.Append("var urlInfoArray = new Array(\"frame\", \"roles.aspx?action=collectionapprovers&LangType=" + m_refAPI.RequestInformationRef.ContentLanguage + "\", \"ek_main\");");
                    result.Append("InsertFile(level3, CreateLink(\"" + objMessage.GetMessage("lbl Collection Approver") + "\", urlInfoArray));");
                    if (Ektron.Cms.DataIO.LicenseManager.LicenseManager.IsFeatureEnable(m_refAPI.RequestInformationRef, Feature.eCommerce))
                    {
                        result.Append("var urlInfoArray = new Array(\"frame\", \"roles.aspx?action=commerceadmin&LangType=" + m_refAPI.RequestInformationRef.ContentLanguage + "\", \"ek_main\");");
                        result.Append("InsertFile(level3, CreateLink(\"" + objMessage.GetMessage("lbl role commerce-admin") + "\", urlInfoArray));");
                    }
                    if (Ektron.Cms.DataIO.LicenseManager.LicenseManager.IsFeatureEnable(m_refAPI.RequestInformationRef, Feature.SocialNetworking))
                    {
                        result.Append("var urlInfoArray = new Array(\"frame\", \"roles.aspx?action=communityadmin&LangType=" + m_refAPI.RequestInformationRef.ContentLanguage + "\", \"ek_main\");");
                        result.Append("InsertFile(level3, CreateLink(\"" + objMessage.GetMessage("lbl role community-admin") + "\", urlInfoArray));");
                        result.Append("var urlInfoArray = new Array(\"frame\", \"roles.aspx?action=communitygroupadmin&LangType=" + m_refAPI.RequestInformationRef.ContentLanguage + "\", \"ek_main\");");
                        result.Append("InsertFile(level3, CreateLink(\"" + objMessage.GetMessage("lbl role communitygroup-admin") + "\", urlInfoArray));");
                        result.Append("var urlInfoArray = new Array(\"frame\", \"roles.aspx?action=communitygroupcreate&LangType=" + m_refAPI.RequestInformationRef.ContentLanguage + "\", \"ek_main\");");
                        result.Append("InsertFile(level3, CreateLink(\"" + objMessage.GetMessage("lbl role communitygroup-create") + "\", urlInfoArray));");
                    }
                    result.Append("var urlInfoArray = new Array(\"frame\", \"roles.aspx?action=masterlayoutcreate&LangType=" + m_refAPI.RequestInformationRef.ContentLanguage + "\", \"ek_main\");");
                    result.Append("InsertFile(level3, CreateLink(\"" + objMessage.GetMessage("lbl masterlayout-create") + "\", urlInfoArray));");
                    result.Append("var urlInfoArray = new Array(\"frame\", \"roles.aspx?action=menuadmin&LangType=" + m_refAPI.RequestInformationRef.ContentLanguage + "\", \"ek_main\");");
                    result.Append("InsertFile(level3, CreateLink(\"" + objMessage.GetMessage("lbl menu admin") + "\", urlInfoArray));");
                    if (Ektron.Cms.DataIO.LicenseManager.LicenseManager.IsFeatureEnable(m_refAPI.RequestInformationRef, Feature.SocialNetworking))
                    {
                        result.Append("var urlInfoArray = new Array(\"frame\", \"roles.aspx?action=messageboardadmin&LangType=" + m_refAPI.RequestInformationRef.ContentLanguage + "\", \"ek_main\");");
                        result.Append("InsertFile(level3, CreateLink(\"" + objMessage.GetMessage("lbl Messageboard-Admin") + "\", urlInfoArray));");
                    }
                    result.Append("var urlInfoArray = new Array(\"frame\", \"roles.aspx?action=metadataadmin&LangType=" + m_refAPI.RequestInformationRef.ContentLanguage + "\", \"ek_main\");");
                    result.Append("InsertFile(level3, CreateLink(\"" + objMessage.GetMessage("lbl Metadata-Admin") + "\", urlInfoArray));");
                    result.Append("var urlInfoArray = new Array(\"frame\", \"roles.aspx?action=multivariatetester&LangType=" + m_refAPI.RequestInformationRef.ContentLanguage + "\", \"ek_main\");");
                    result.Append("InsertFile(level3, CreateLink(\"" + objMessage.GetMessage("alt multivariate tester") + "\", urlInfoArray));");
                    result.Append("var urlInfoArray = new Array(\"frame\", \"roles.aspx?action=searchadmin&LangType=" + m_refAPI.RequestInformationRef.ContentLanguage + "\", \"ek_main\");");
                    result.Append("InsertFile(level3, CreateLink(\"" + objMessage.GetMessage("lbl role search-admin") + "\", urlInfoArray));");
                    result.Append("var urlInfoArray = new Array(\"frame\", \"roles.aspx?action=xmlconfigadmin&LangType=" + m_refAPI.RequestInformationRef.ContentLanguage + "\", \"ek_main\");");
                    result.Append("InsertFile(level3, CreateLink(\"" + objMessage.GetMessage("lbl Smart Forms Admin") + "\", urlInfoArray));");
                    if (LicenseManager.IsFeatureEnable(m_refAPI.RequestInformationRef, Feature.eSync))
                    {
                        result.Append("var urlInfoArray = new Array(\"frame\", \"roles.aspx?action=syncadmin&LangType=" + m_refAPI.RequestInformationRef.ContentLanguage + "\", \"ek_main\");");
                        result.Append("InsertFile(level3, CreateLink(\"" + objMessage.GetMessage("lbl sync admin") + "\", urlInfoArray));");
                        result.Append("var urlInfoArray = new Array(\"frame\", \"roles.aspx?action=syncuser&LangType=" + m_refAPI.RequestInformationRef.ContentLanguage + "\", \"ek_main\");");
                        result.Append("InsertFile(level3, CreateLink(\"" + objMessage.GetMessage("lbl sync user") + "\", urlInfoArray));");
                    }
                    result.Append("var urlInfoArray = new Array(\"frame\", \"roles.aspx?action=taskcreate&LangType=" + m_refAPI.RequestInformationRef.ContentLanguage + "\", \"ek_main\");");
                    result.Append("InsertFile(level3, CreateLink(\"" + objMessage.GetMessage("lbl Task-Create") + "\", urlInfoArray));");
                    result.Append("var urlInfoArray = new Array(\"frame\", \"roles.aspx?action=taskdelete&LangType=" + m_refAPI.RequestInformationRef.ContentLanguage + "\", \"ek_main\");");
                    result.Append("InsertFile(level3, CreateLink(\"" + objMessage.GetMessage("lbl Task-Delete") + "\", urlInfoArray));");
                    result.Append("var urlInfoArray = new Array(\"frame\", \"roles.aspx?action=taskredirect&LangType=" + m_refAPI.RequestInformationRef.ContentLanguage + "\", \"ek_main\");");
                    result.Append("InsertFile(level3, CreateLink(\"" + objMessage.GetMessage("lbl Task-Redirect") + "\", urlInfoArray));");
                    result.Append("var urlInfoArray = new Array(\"frame\", \"roles.aspx?action=taxonomyadministrator&LangType=" + m_refAPI.RequestInformationRef.ContentLanguage + "\", \"ek_main\");");
                    result.Append("InsertFile(level3, CreateLink(\"" + objMessage.GetMessage("lbl Taxonomy Administrator") + "\", urlInfoArray));");
                    result.Append("var urlInfoArray = new Array(\"frame\", \"roles.aspx?action=templateconfig&LangType=" + m_refAPI.RequestInformationRef.ContentLanguage + "\", \"ek_main\");");
                    result.Append("InsertFile(level3, CreateLink(\"" + objMessage.GetMessage("lbl Template Configuration") + "\", urlInfoArray));");
                    result.Append("var urlInfoArray = new Array(\"frame\", \"roles.aspx?action=useradmin&LangType=" + m_refAPI.RequestInformationRef.ContentLanguage + "\", \"ek_main\");");
                    result.Append("InsertFile(level3, CreateLink(\"" + objMessage.GetMessage("lbl User Admin") + "\", urlInfoArray));");
                    result.Append("var urlInfoArray = new Array(\"frame\", \"roles.aspx?action=translationstateadmin&LangType=" + m_refAPI.RequestInformationRef.ContentLanguage + "\", \"ek_main\");");
                    result.Append("InsertFile(level3, CreateLink(\"" + objMessage.GetMessage("lbl translation state admin") + "\", urlInfoArray));");
                    result.Append("var urlInfoArray = new Array(\"frame\", \"roles.aspx?action=xliffadmin&LangType=" + m_refAPI.RequestInformationRef.ContentLanguage + "\", \"ek_main\");");
                    result.Append("InsertFile(level3, CreateLink(\"" + objMessage.GetMessage("lbl XLIFF admin") + "\", urlInfoArray));");
                    //Personalization
                    //result.Append("level4 = InsertFolder(level3, CreateFolderInstance(\"" + objMessage.GetMessage("lbl personalizations") + "\", \"\"));");
                    //result.Append("var urlInfoArray = new Array(\"frame\", \"roles.aspx?action=personalizationadmin&LangType=" + m_refAPI.RequestInformationRef.ContentLanguage + "\", \"ek_main\");");
                    //result.Append("InsertFile(level4, CreateLink(\"" + objMessage.GetMessage("lbl Admins") + "\", urlInfoArray));");
                    //result.Append("var urlInfoArray = new Array(\"frame\", \"roles.aspx?action=personalizationaddonly&LangType=" + m_refAPI.RequestInformationRef.ContentLanguage + "\", \"ek_main\");");
                    //result.Append("InsertFile(level4, CreateLink(\"" + objMessage.GetMessage("alt add web parts") + "\", urlInfoArray));");
                    //result.Append("var urlInfoArray = new Array(\"frame\", \"roles.aspx?action=personalizationeditonly&LangType=" + m_refAPI.RequestInformationRef.ContentLanguage + "\", \"ek_main\");");
                    //result.Append("InsertFile(level4, CreateLink(\"" + objMessage.GetMessage("alt edit web parts properties") + "\", urlInfoArray));");
                    //result.Append("var urlInfoArray = new Array(\"frame\", \"roles.aspx?action=personalizationmoveonly&LangType=" + m_refAPI.RequestInformationRef.ContentLanguage + "\", \"ek_main\");");
                    //result.Append("InsertFile(level4, CreateLink(\"" + objMessage.GetMessage("alt move web parts") + "\", urlInfoArray));");
                    result.Append("level3 = InsertFolder(level2, CreateFolderInstance(\"" + objMessage.GetMessage("lbl folder specific") + "\", \"\"));");
                    result.Append("var urlInfoArray = new Array(\"frame\", \"roles.aspx?action=folderuseradmin&LangType=" + m_refAPI.RequestInformationRef.ContentLanguage + "\", \"ek_main\");");
                    result.Append("InsertFile(level3, CreateLink(\"" + objMessage.GetMessage("lbl Folder User Admin") + "\", urlInfoArray));");
                    result.Append("var urlInfoArray = new Array(\"frame\", \"roles.aspx?action=moveorcopy&LangType=" + m_refAPI.RequestInformationRef.ContentLanguage + "\", \"ek_main\");");
                    result.Append("InsertFile(level3, CreateLink(\"" + objMessage.GetMessage("lbl move or copy") + "\", urlInfoArray));");
                    result.Append("var urlInfoArray = new Array(\"frame\", \"roles.aspx?action=managecustompermissions&LangType=" + m_refAPI.RequestInformationRef.ContentLanguage + "\", \"ek_main\");");
                    result.Append("InsertFile(level1, CreateLink(\"" + objMessage.GetMessage("lbl custom permissions") + "\", urlInfoArray));");

                }
                //Roles Ends
                if (!IsSystemAccount)
                {
                    //Business Rules
                    result.Append("var urlInfoArray = new Array(\"frame\", \"businessrules/ruleset.aspx\", \"ek_main\");");
                    result.Append("InsertFile(TopTreeLevel, CreateLink(\"" + objMessage.GetMessage("lbl Business Rules") + "\", urlInfoArray));");
                    //Business Rules Ends

                    bool EnableClassicCalendar = false;
                    bool.TryParse(ConfigurationManager.AppSettings["ek_enableClassicCalendar"], out EnableClassicCalendar);
                    if (EnableClassicCalendar)
                    {
                        //Calendar
                        result.Append("var urlInfoArray = new Array(\"frame\", \"cmscalendar.aspx?action=ViewAllCalendars\", \"ek_main\");");
                        result.Append("InsertFile(TopTreeLevel, CreateLink(\"" + objMessage.GetMessage("calendar lbl") + "\", urlInfoArray));");
                        //Calendar Ends
                    }

                }

                //User Groups
                if (IsAdmin)
                {
                    result.Append("var urlInfoArray = new Array(\"frame\", \"users.aspx?action=viewallgroups&LangType=" + m_refAPI.RequestInformationRef.ContentLanguage + "&grouptype=0\", \"ek_main\");");
                    result.Append("InsertFile(TopTreeLevel, CreateLink(\"" + objMessage.GetMessage("generic User Groups") + "\", urlInfoArray));");
                }
                else
                {
                    if (objContentRef.IsARoleMember(Convert.ToInt64(Ektron.Cms.Common.EkEnumeration.CmsRoleIds.AdminUsers), currentUserID, false))
                    {
                        result.Append("var urlInfoArray = new Array(\"frame\", \"users.aspx?action=viewallgroups&LangType=" + m_refAPI.RequestInformationRef.ContentLanguage + "&grouptype=0\", \"ek_main\");");
                        result.Append("level1 = InsertFolder(TopTreeLevel, CreateFolderInstance(\"" + objMessage.GetMessage("generic User Groups") + "\", urlInfoArray));");
                    }
                }
                //User Groups Ends

                //Users
                if (IsAdmin || (objContentRef.IsARoleMember(Convert.ToInt64(Ektron.Cms.Common.EkEnumeration.CmsRoleIds.AdminUsers), currentUserID, false)))
                {
                    result.Append("var urlInfoArray = new Array(\"frame\", \"users.aspx?backaction=viewallusers&action=viewallusers&LangType=" + m_refAPI.RequestInformationRef.ContentLanguage + "&grouptype=0&groupid=2&id=2&FromUsers=1\", \"ek_main\");");
                    result.Append("InsertFile(TopTreeLevel, CreateLink(\"" + objMessage.GetMessage("generic Users") + "\", urlInfoArray));");
                }
                else
                {
                    result.Append("var urlInfoArray = new Array(\"frame\", \"users.aspx?action=View&grouptype=0&LangType=" + m_refAPI.RequestInformationRef.ContentLanguage + "&groupid=2&id=" + currentUserID + "\", \"ek_main\");");
                    result.Append("level1 = InsertFolder(TopTreeLevel, CreateFolderInstance(\"" + objMessage.GetMessage("generic User Profile") + "\", urlInfoArray));");
                }

            }
            else if ((TreeRequested) == ("report"))
            {
                result.Append("var urlInfoArray = new Array(\"frame\", \"\", \"ek_main\");");
                result.Append("TopTreeLevel = CreateFolderInstance(\"" + objMessage.GetMessage("generic reports title") + "\", urlInfoArray);");

                //Commerce
                if (Ektron.Cms.DataIO.LicenseManager.LicenseManager.IsFeatureEnable(m_refAPI.RequestInformationRef, Feature.eCommerce) && (IsAdmin || m_refAPI.IsARoleMember(Ektron.Cms.Common.EkEnumeration.CmsRoleIds.CommerceAdmin)))
                {

                    result.Append("var urlInfoArray = new Array(\"frame\", \"\", \"ek_main\");");
                    result.Append("level2 = InsertFolder(TopTreeLevel, CreateFolderInstance(\"" + objMessage.GetMessage("lbl commerce") + "\", \"\"));");
                    result.Append("var urlInfoArray = new Array(\"frame\", \"Commerce/reporting/CustomerReports.aspx?page=normal\", \"ek_main\");");
                    result.Append("InsertFile(level2, CreateLink(\"" + objMessage.GetMessage("lbl customer reports") + "\", urlInfoArray));");
                    result.Append("var urlInfoArray = new Array(\"frame\", \"Commerce/inventory.aspx\", \"ek_main\");");
                    result.Append("InsertFile(level2, CreateLink(\"" + objMessage.GetMessage("lbl inventory reports") + "\", urlInfoArray));");
					result.Append("var urlInfoArray = new Array(\"frame\", \"Commerce/reporting/KeyPerformanceIndicators.aspx\", \"ek_main\");");
                    result.Append("InsertFile(level2, CreateLink(\"" + objMessage.GetMessage("lbl key performance indicators") + "\", urlInfoArray));");
                    result.Append("var urlInfoArray = new Array(\"frame\", \"Commerce/reporting/PaymentReports.aspx\", \"ek_main\");");
                    result.Append("InsertFile(level2, CreateLink(\"" + objMessage.GetMessage("lbl payment reports") + "\", urlInfoArray));");
                    result.Append("var urlInfoArray = new Array(\"frame\", \"Commerce/reporting/SalesTrend.aspx\", \"ek_main\");");
                    result.Append("InsertFile(level2, CreateLink(\"" + objMessage.GetMessage("lbl sales trend") + "\", urlInfoArray));");
                    result.Append("var urlInfoArray = new Array(\"frame\", \"Commerce/reporting/TopProducts.aspx\", \"ek_main\");");
                    result.Append("InsertFile(level2, CreateLink(\"" + objMessage.GetMessage("lbl top products") + "\", urlInfoArray));");

                }

                //Add a New folder for Content reports
                result.Append("var urlInfoArray = new Array(\"frame\", \"\", \"ek_main\");");
                result.Append("level2 = InsertFolder(TopTreeLevel, CreateFolderInstance(\"" + objMessage.GetMessage("lbl web alert contents") + "\", urlInfoArray));");

                result.Append("var urlInfoArray = new Array(\"frame\", \"approval.aspx?action=viewApprovalList&location=workarea\", \"ek_main\");");
                result.Append("InsertFile(level2, CreateLink(\"" + objMessage.GetMessage("generic Approvals") + "\", urlInfoArray));");

                result.Append("var urlInfoArray = new Array(\"frame\", \"reports.aspx?action=ViewCheckedIn\", \"ek_main\");");
                result.Append("InsertFile(level2, CreateLink(\"" + objMessage.GetMessage("content reports checked in title") + "\", urlInfoArray));");
                if (IsAdmin)
                {
                    result.Append("var urlInfoArray = new Array(\"frame\", \"reports.aspx?action=ViewCheckedOut\", \"ek_main\");");
                }
                else
                {
                    result.Append("var urlInfoArray = new Array(\"frame\", \"reports.aspx?action=ViewCheckedOut&interval=&filtertype=USER&filterId=" + m_refContentApi.RequestInformationRef.UserId + "&orderby=\", \"ek_main\");");
                }
                result.Append("InsertFile(level2, CreateLink(\"" + objMessage.GetMessage("content reports checked out title") + "\", urlInfoArray));");

                result.Append("var urlInfoArray = new Array(\"frame\", \"reports.aspx?action=ViewNewContent\", \"ek_main\");");
                result.Append("InsertFile(level2, CreateLink(\"" + objMessage.GetMessage("content reports new title") + "\", urlInfoArray));");

                result.Append("var urlInfoArray = new Array(\"frame\", \"reports.aspx?action=ViewSubmitted\", \"ek_main\");");
                result.Append("InsertFile(level2, CreateLink(\"" + objMessage.GetMessage("content reports submitted title") + "\", urlInfoArray));");

                result.Append("var urlInfoArray = new Array(\"frame\", \"reports.aspx?action=ViewPending\", \"ek_main\");");
                result.Append("InsertFile(level2, CreateLink(\"" + objMessage.GetMessage("content reports pending title") + "\", urlInfoArray));");

                result.Append("var urlInfoArray = new Array(\"frame\", \"reports.aspx?action=ViewRefreshReport\", \"ek_main\");");
                result.Append("InsertFile(level2, CreateLink(\"" + objMessage.GetMessage("content reports refresh title") + "\", urlInfoArray));");

                result.Append("var urlInfoArray = new Array(\"frame\", \"reports.aspx?action=ViewExpired\", \"ek_main\");");
                result.Append("InsertFile(level2, CreateLink(\"" + objMessage.GetMessage("content reports expired title") + "\", urlInfoArray));");

                result.Append("var urlInfoArray = new Array(\"frame\", \"reports.aspx?action=ViewToExpire&interval=10\", \"ek_main\");");
                result.Append("InsertFile(level2, CreateLink(\"" + objMessage.GetMessage("lbl expire all smrtdesk") + "\", urlInfoArray));");

                result.Append("var urlInfoArray = new Array(\"frame\", \"reports.aspx?action=SiteUpdateActivity\", \"ek_main\");");
                result.Append("InsertFile(level2, CreateLink(\"" + objMessage.GetMessage("lbl Site Update Activity Content Report") + "\", urlInfoArray));");

                result.Append("var urlInfoArray = new Array(\"frame\", \"reports.aspx?action=ViewAsynchLogFile\", \"ek_main\");");
                result.Append("InsertFile(level2, CreateLink(\"" + objMessage.GetMessage("lbl Asynchronous Log File Report") + "\", urlInfoArray));");

                result.Append("var urlInfoArray = new Array(\"frame\", \"reports.aspx?action=ViewSearchPhraseReport\", \"ek_main\");");
                result.Append("InsertFile(level2, CreateLink(\"" + objMessage.GetMessage("lbl Search Phrase Report") + "\", urlInfoArray));");

                if (PerReadOnly)
                {
                    SiteAPI m_refSiteApi = new SiteAPI();
                    SettingsData setting_data = new SettingsData();
                    setting_data = m_refSiteApi.GetSiteVariables(m_refSiteApi.UserId);
                    if (setting_data.EnablePreApproval)
                    {
                        result.Append("var urlInfoArray = new Array(\"frame\", \"reports.aspx?action=ViewPreapproval\", \"ek_main\");");
                        result.Append("InsertFile(level2, CreateLink(\"" + objMessage.GetMessage("lbl Preapproval Groups") + "\", urlInfoArray));");
                    }
                }

                result.Append("var urlInfoArray = new Array(\"frame\", \"BadLinkCheck.aspx\", \"ek_main\");");
                result.Append("InsertFile(level2, CreateLink(\"" + objMessage.GetMessage("lbl Bad Link Report") + "\", urlInfoArray));");

                result.Append("var urlInfoArray = new Array(\"frame\", \"reports.aspx?action=ContentFlags\", \"ek_main\");");
                result.Append("InsertFile(level2, CreateLink(\"" + objMessage.GetMessage("lbl flag report") + "\", urlInfoArray));");

                result.Append("var urlInfoArray = new Array(\"frame\", \"reports.aspx?action=ContentReviews\", \"ek_main\");");
                result.Append("InsertFile(level2, CreateLink(\"" + objMessage.GetMessage("lbl review report") + "\", urlInfoArray));");

                //Site Analytics
                Ektron.Cms.Analytics.IAnalytics dataManager = ObjectFactory.GetAnalytics();
                if (Ektron.Cms.DataIO.LicenseManager.LicenseManager.IsFeatureEnable(m_refAPI.RequestInformationRef, Feature.WebSiteAnalytics) && dataManager.IsAnalyticsViewer())
                {
                    result.AppendLine("var urlInfoArray = new Array(\"frame\", \"\", \"ek_main\");");
                    result.AppendLine("level1 = InsertFolder(TopTreeLevel, CreateFolderInstance(\"" + m_refAPI.EkMsgRef.GetMessage("lbl site analytics") + "\", urlInfoArray));");
                    System.Collections.Generic.List<string> analyticsProviders = new System.Collections.Generic.List<string>();
                    analyticsProviders = dataManager.GetProviderList();
                    if (analyticsProviders.Count() > 0)
                    {
                        foreach (string providerName in analyticsProviders)
                        {
                            string folderName = providerName;
                            if (providerName == "Google")
                                folderName = objMessage.GetMessage("generic google");
                            else if (providerName == "WebTrends")
                                folderName = objMessage.GetMessage("Generic WebTrends");
                            else if(providerName =="SiteCatalyst" )
                                folderName = objMessage.GetMessage("Generic SiteCatalyst");
                            result.AppendLine("var urlInfoArray = new Array(\"frame\", \"\", \"ek_main\");");
                            result.AppendLine("level2 = InsertFolder(level1, CreateFolderInstance(\"" + folderName + "\", urlInfoArray));"); //  
                            string providerType = dataManager.GetProviderType(providerName);
                            if ((string)(providerType) == "Ektron.Cms.Analytics.Providers.GoogleAnalyticsProvider")
                            {
                                AppendTreeItems(result, (System.Web.UI.HtmlControls.HtmlGenericControl)GoogleAnalyticsContainer.TreeContainer, 2, providerName, true);
                            }
                            else if ((string)(providerType) == "Ektron.Cms.Analytics.Providers.WebTrendsProvider")
                            {
                                AppendTreeItems(result, (System.Web.UI.HtmlControls.HtmlGenericControl)WebTrendsContainer.GetTreeContainer(providerName), 2, providerName, false);
                            }
                            else if ((string)(providerType) == "Ektron.Cms.Analytics.Providers.SiteCatalystProvider")
                            {
                                AppendTreeItems(result, (System.Web.UI.HtmlControls.HtmlGenericControl)SiteCatalystContainer.TreeContainer, 2, providerName, true);
                            }
                            else
                            {
                            }
                        }
                    }
                    result.Append("var urlInfoArray = new Array(\"frame\", \"Analytics/reporting/reports.aspx?report=CmsSearchTerms\", \"ek_main\");");
                    result.Append("InsertFile(level1, CreateLink(\"" + objMessage.GetMessage("analytics searches") + "\", urlInfoArray));");
                }
                //Site Analytics Ends

                //Pre Version 8 Analytics
                bool EnableClassicAnalytics;
                bool.TryParse(ConfigurationManager.AppSettings["ek_enableClassicAnalytics"], out EnableClassicAnalytics);
                if (EnableClassicAnalytics)
                {
                    result.Append("var urlInfoArray = new Array(\"frame\", \"ContentAnalytics.aspx\", \"ek_main\");");
                    result.Append("level1 = InsertFolder(TopTreeLevel, CreateFolderInstance(\"" + m_refAPI.EkMsgRef.GetMessage("lbl pre8 site analytics") + "\", urlInfoArray));");

                    result.Append("var urlInfoArray = new Array(\"frame\", \"ContentAnalytics.aspx?type=content\", \"ek_main\");");
                    result.Append("InsertFile(level1, CreateLink(\"" + m_refAPI.EkMsgRef.GetMessage("top content") + "\", urlInfoArray));");
                    result.Append("var urlInfoArray = new Array(\"frame\", \"ContentAnalytics.aspx?type=page\", \"ek_main\");");
                    result.Append("InsertFile(level1, CreateLink(\"" + m_refAPI.EkMsgRef.GetMessage("lbl top templates") + "\", urlInfoArray));");
                    result.Append("var urlInfoArray = new Array(\"frame\", \"ContentAnalytics.aspx?type=referring\", \"ek_main\");");
                    result.Append("InsertFile(level1, CreateLink(\"" + m_refAPI.EkMsgRef.GetMessage("top referrers") + "\", urlInfoArray));");
                    result.Append("var urlInfoArray = new Array(\"frame\", \"reports.aspx?action=ViewSearchPhraseReport\", \"ek_main\");");
                    result.Append("InsertFile(level1, CreateLink(\"" + m_refAPI.EkMsgRef.GetMessage("lbl Search Phrase Report") + "\", urlInfoArray));");
                }
                //Pre Version 8 Analytics Ends

                //new tasks folder
                result.Append("level2 = InsertFolder(TopTreeLevel, CreateFolderInstance(\"" + objMessage.GetMessage("lbl Tasks") + "\", \"\"));");
                result.Append("var urlInfoArray = new Array(\"frame\", \"tasks.aspx?action=ViewTasks&ty=all\", \"ek_main\");");
                if (IsAdmin)
                {
                    result.Append("level3 = InsertFile(level2, CreateLink(\"" + objMessage.GetMessage("lbl All Open Tasks") + "\", urlInfoArray));");
                }
                result.Append("var urlInfoArray = new Array(\"frame\", \"tasks.aspx?action=ViewTasks&ty=both\", \"ek_main\");");
                result.Append("level3 = InsertFile(level2, CreateLink(\"" + objMessage.GetMessage("assigned by and to me") + "\", urlInfoArray));");
                result.Append("var urlInfoArray = new Array(\"frame\", \"tasks.aspx?action=ViewTasks&ty=to\", \"ek_main\");");
                result.Append("level3 = InsertFile(level2, CreateLink(\"" + objMessage.GetMessage("lbl Assigned to me") + "\", urlInfoArray));");
                result.Append("var urlInfoArray = new Array(\"frame\", \"tasks.aspx?action=ViewTasks&ty=by\", \"ek_main\");");
                result.Append("level3 = InsertFile(level2, CreateLink(\"" + objMessage.GetMessage("lbl Assigned by me") + "\", urlInfoArray));");
                result.Append("var urlInfoArray = new Array(\"frame\", \"tasks.aspx?action=ViewTasks&ty=created\", \"ek_main\");");
                result.Append("level3 = InsertFile(level2, CreateLink(\"" + objMessage.GetMessage("lbl Created by me") + "\", urlInfoArray));");
                result.Append("var urlInfoArray = new Array(\"frame\", \"tasks.aspx?action=ViewTasks&ty=touser\", \"ek_main\");");
                result.Append("level3 = InsertFile(level2, CreateLink(\"" + objMessage.GetMessage("lbl Assigned to User") + "\", urlInfoArray));");
                result.Append("var urlInfoArray = new Array(\"frame\", \"tasks.aspx?action=ViewTasks&ty=notstarted\", \"ek_main\");");
                result.Append("level3 = InsertFile(level2, CreateLink(\"" + objMessage.GetMessage("lbl Not Started") + "\", urlInfoArray));");
                result.Append("var urlInfoArray = new Array(\"frame\", \"tasks.aspx?action=ViewTasks&ty=active\", \"ek_main\");");
                result.Append("level3 = InsertFile(level2, CreateLink(\"" + objMessage.GetMessage("Active label") + "\", urlInfoArray));");
                result.Append("var urlInfoArray = new Array(\"frame\", \"tasks.aspx?action=ViewTasks&ty=awaitingdata\", \"ek_main\");");
                result.Append("level3 = InsertFile(level2, CreateLink(\"" + objMessage.GetMessage("lbl Awaiting Data") + "\", urlInfoArray));");
                result.Append("var urlInfoArray = new Array(\"frame\", \"tasks.aspx?action=ViewTasks&ty=onhold\", \"ek_main\");");
                result.Append("level3 = InsertFile(level2, CreateLink(\"" + objMessage.GetMessage("lbl On hold") + "\", urlInfoArray));");
                result.Append("var urlInfoArray = new Array(\"frame\", \"tasks.aspx?action=ViewTasks&ty=pending\", \"ek_main\");");
                result.Append("level3 = InsertFile(level2, CreateLink(\"" + objMessage.GetMessage("lbl Pending") + "\", urlInfoArray));");
                result.Append("var urlInfoArray = new Array(\"frame\", \"tasks.aspx?action=ViewTasks&ty=reopened\", \"ek_main\");");
                result.Append("level3 = InsertFile(level2, CreateLink(\"" + objMessage.GetMessage("lbl Reopened") + "\", urlInfoArray));");
                result.Append("var urlInfoArray = new Array(\"frame\", \"tasks.aspx?action=ViewTasks&ty=completed\", \"ek_main\");");
                result.Append("level3 = InsertFile(level2, CreateLink(\"" + objMessage.GetMessage("lbl Completed") + "\", urlInfoArray));");
                result.Append("var urlInfoArray = new Array(\"frame\", \"tasks.aspx?action=ViewTasks&ty=archived\", \"ek_main\");");
                if (IsAdmin)
                {
                    result.Append("level3 = InsertFile(level2, CreateLink(\"" + objMessage.GetMessage("lbl Archived") + "\", urlInfoArray));");
                }
                result.Append("var urlInfoArray = new Array(\"frame\", \"tasks.aspx?action=ViewTasks&ty=deleted\", \"ek_main\");");
                if (IsAdmin)
                {
                    result.Append("level3 = InsertFile(level2, CreateLink(\"" + objMessage.GetMessage("lbl Deleted") + "\", urlInfoArray));");
                }

            }
            if (TreeRequested != "smartdesktop" && TreeRequested != "help")
            {
                result.Append("if (IsValid())" + "\r\n");
                result.Append("InitializeFolderControl();" + "\r\n");
            }
            //result.AppendLine("}); // Ektron.ready")
            result.AppendLine("//-->");
            result.AppendLine("</script>");
            result.Append("<script type=\"text/javascript\">" + "\r\n");
            result.Append("<!--" + "\r\n");
            result.Append("function OpenWorkareaFolder() {" + "\r\n");
            if (Request.QueryString["autonav"] != "")
            {
                result.Append("OpenFolder(\"" + Strings.Replace(EkFunctions.HtmlEncode(Request.QueryString["autonav"]), "\\", "\\\\", 1, -1, 0) + "\", false);" + "\r\n");
            }
            result.Append("}" + "\r\n");
            result.Append("OpenWorkareaFolder();" + "\r\n");
            result.Append("//--></script>" + "\r\n");
        }
        catch (Exception ex)
        {
            result.Length = 0;
            result.Append(ex.ToString());
        }
        finally
        {
        }
        return (result.ToString());
    }

    private void AppendTreeItems(StringBuilder result, HtmlGenericControl ctlItem, int level, string ProviderName, bool needLocalize)
    {
        try
        {
            foreach (Control ctlChild in ctlItem.Controls)
            {
                if (ctlChild is LiteralControl)
                {
                    LiteralControl litCaption = (LiteralControl)ctlChild;
                    string strCaption = litCaption.Text.Replace(Constants.vbCr, "").Replace(Constants.vbLf, "").Replace("\t", "").Trim();
                    if (!string.IsNullOrEmpty(strCaption))
                    {
                        if (needLocalize)
                        {
                            //litCaption.Text = m_refAPI.EkMsgRef.GetMessage(strCaption)
                            strCaption = m_refAPI.EkMsgRef.GetMessage(strCaption);
                            strCaption = Ektron.Cms.API.JS.Escape(strCaption);
                        }
                        if (ctlItem.Controls.Count > 1)
                        {
                            result.AppendLine("level" + (level + 1) + " = InsertFolder(level" + level + ", CreateFolderInstance(\"" + strCaption + "\", []));");
                            level++;
                        }
                        else
                        {
                            string strHref = ctlItem.Attributes["href"];
                            if (strHref.IndexOf("?") > -1)
                            {
                                strHref = strHref + "&provider=" + ProviderName;
                            }
                            else
                            {
                                strHref = strHref + "?provider=" + ProviderName;
                            }
                            strHref = Ektron.Cms.API.JS.Escape(strHref);
                            result.AppendLine("InsertFile(level" + level + ", CreateLink(\"" + strCaption + "\", [\"frame\", \"" + strHref + "\", \"ek_main\"]));");
                        }
                    }
                }
                if (ctlChild is HtmlGenericControl)
                {
                    AppendTreeItems(result, (System.Web.UI.HtmlControls.HtmlGenericControl)ctlChild, level, ProviderName, needLocalize);
                }
            }
        }
        catch (Exception)
        {
        }
    }

    private void ShowLoginError()
    {
        Response.Write("<table><tr><td class=\"exception\" ><b>Login Error: Process terminated.</b>  <br>User may not be logged in or logged out from another system.  To continue this process close this workarea and log-in back.</td></tr></table>");
    }
}


