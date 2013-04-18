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
//using Ektron.Cms.Common.EkConstants;
using Ektron.Cms.API;
using Microsoft.Security.Application;
using Ektron.Cms.Common;


public partial class viewconfiguration : System.Web.UI.UserControl
{
    protected StyleHelper m_refStyle = new StyleHelper();
    protected EkMessageHelper m_refMsg;
    protected string AppImgPath = "";
    protected string AppName = "";
    protected string SITEPATH = "";
    protected string VerifyTrue = "";
    protected string VerifyFalse = "";
    protected string m_SelectedEditControl = "";
    protected UserAPI m_refUserApi = new UserAPI();

    private void Page_Load(System.Object sender, System.EventArgs e)
    {
        RegisterResources();
       
        SiteAPI m_refSiteApi = new SiteAPI();
        m_refMsg = m_refSiteApi.EkMsgRef;
        SettingsData settings_data;
        UserData user_data;
        UserGroupData[] group_data;
        UserPreferenceData preference_data;
        AppImgPath = m_refSiteApi.AppImgPath;
        AppName = m_refSiteApi.AppName;
        SITEPATH = m_refSiteApi.SitePath;
        user_data = m_refUserApi.GetUserById(Ektron.Cms.Common.EkConstants.BuiltIn, false, false);
        preference_data = m_refUserApi.GetUserPreferenceById(0);
        group_data = m_refUserApi.GetAllUserGroups("GroupName");
        VerifyTrue = "<img src=\"" + AppImgPath + "../UI/Icons/check.png\" border=\"0\" alt=\"Item is Enabled\" title=\"Item is Enabled\">";
        VerifyFalse = "<img src=\"" + AppImgPath + "icon_redx.gif\" border=\"0\" alt=\"Item is Disabled\" title=\"Item is Disabled\">";
        SetStrings();
        settings_data = m_refSiteApi.GetSiteVariables(m_refSiteApi.UserId, true);
        //VERSION
        td_version.InnerHtml = m_refMsg.GetMessage("version") + "&nbsp;" + m_refSiteApi.Version + "&nbsp;" + m_refSiteApi.ServicePack;
        //BUILD NUMBER
        td_buildnumber.InnerHtml = "<i>(" + m_refMsg.GetMessage("build") + m_refSiteApi.BuildNumber + ")</i>";

        //Which Editor
        m_SelectedEditControl = Utilities.GetEditorPreference(Request);
        if (m_SelectedEditControl == "ContentDesigner")
            tr_wordclass.Visible = true;

        LanguageData language_data;
        language_data = m_refSiteApi.GetLanguageById(m_refSiteApi.DefaultContentLanguage);
        td_Language.InnerHtml = language_data.Name;
        //LICENSE

        if (settings_data.LicenseKey.Length > 0)
        {
            td_licensekey.InnerHtml = settings_data.LicenseKey;
        }
        else
        {
            td_licensekey.InnerHtml = m_refMsg.GetMessage("none specified msg");
        }

        //MODULE LICENSE
        System.Text.StringBuilder module_text = new System.Text.StringBuilder();
        int i = 0;
        if (!(settings_data.ModuleLicense == null))
        {
            for (i = 0; i <= settings_data.ModuleLicense.Length - 1; i++)
            {
                module_text.Append(i + 1 + "." + settings_data.ModuleLicense[i].License);
                module_text.Append("<br/>");
            }
        }
        else
        {
            module_text.Append(m_refMsg.GetMessage("none specified msg"));
        }

        td_modulelicense.InnerHtml = module_text.ToString();
        //LANGUAGE LIST
        LanguageData[] active_lang_list;
        active_lang_list = m_refSiteApi.GetAllActiveLanguages();

        td_languagelist.InnerHtml = m_refMsg.GetMessage("none specified msg");
        if (!(active_lang_list == null))
        {
            for (i = 0; i <= active_lang_list.Length - 1; i++)
            {
                if (Convert.ToString(active_lang_list[i].Id) == settings_data.Language)
                {
                    td_languagelist.InnerHtml = active_lang_list[i].Name;
                    break;
                }
            }
        }


        //MAX CONTENT SIZE

        //td_maxcontent.InnerHtml = settings_data.MaxContentSize

        //MAX SUMMARY SIZE

        //td_maxsummary.InnerHtml = settings_data.MaxSummarySize;


        //SYSTEM EMAIL


        if (settings_data.Email.Length > 0)
        {
            td_email.InnerHtml = settings_data.Email;
        }
        else
        {
            td_email.InnerHtml = m_refMsg.GetMessage("none specified msg");
        }

        //EMAIL NOTIFICATION

        if (settings_data.EnableMessaging)
        {
            td_email_msg.InnerHtml = m_refMsg.GetMessage("sending email enabled msg");
        }
        else
        {
            td_email_msg.InnerHtml = m_refMsg.GetMessage("sending email disabled msg");
        }

        //Server Type
        td_server_type.InnerHtml += GetCheckValue(bool.Parse(settings_data.AsynchronousStaging)) + "&nbsp;" + m_refMsg.GetMessage("lbl enable server type message");


        //Asyncronous Processor Location

        if (!(settings_data.AsynchronousLocation == null) && (settings_data.AsynchronousLocation.Length > 0))
        {
            td_asynch_location.InnerHtml = settings_data.AsynchronousLocation;
        }
        else
        {
            td_asynch_location.InnerHtml = m_refMsg.GetMessage("none specified msg");
        }

        //PUBPDF
        trPublishPDF.Visible = settings_data.PublishPdfSupported;
        td_publish_pdf.InnerHtml += GetCheckValue(settings_data.PublishPdfEnabled) + "&nbsp;" + m_refMsg.GetMessage("alt Enable office documents to be published in other format");

        //LIBRARY FOLDER CREATION
        td_libfolder.InnerHtml = GetCheckValue(settings_data.FileSystemSupport) + "&nbsp;";
        td_libfolder.InnerHtml += m_refMsg.GetMessage("library filesystem folder prompt") + "&nbsp;";

        //BUILT IN USER
        td_user.InnerHtml = user_data.Username;
        //td_removestyle.InnerHtml = GetCheckValue(settings_data.RemoveStyles) & "&nbsp;" & m_refMsg.GetMessage("remove styles")
        td_enablefont.InnerHtml = GetCheckValue(settings_data.EnableFontButtons) + "&nbsp;" + m_refMsg.GetMessage("enable font buttons") + "&nbsp;";
        td_wordstyle.InnerHtml = GetCheckValue(settings_data.PreserveWordStyles) + "&nbsp;" + m_refMsg.GetMessage("preserve word styles");
        td_wordclass.InnerHtml = GetCheckValue(settings_data.PreserveWordClasses) + "&nbsp;" + m_refMsg.GetMessage("preserve word classes");
        td_access.InnerHtml = GetAccessibilityValue(settings_data.Accessibility);


        if (preference_data.Template == "")
        {
            td_template.InnerHtml = m_refMsg.GetMessage("refresh login page msg");
        }
        else
        {
            td_template.InnerHtml = SITEPATH + EkFunctions.HtmlEncode(preference_data.Template);
        }

        td_folder.InnerHtml = "<input type=\"checkbox\" disabled ";
        if (Convert.ToString(preference_data.FolderId) == "")
        {
            td_folder.InnerHtml += " checked ";
        }
        td_folder.InnerHtml += " id=\"checkbox\" name=\"chkSmartDexktop\">";
        td_force.InnerHtml = "<input type=\"checkbox\" disabled ";
        if (preference_data.ForceSetting)
        {
            td_force.InnerHtml += " checked ";
        }
        td_force.InnerHtml += " id=\"Checkbox1\" name=\"forcePrefs\">";
        //td_titletext.InnerHtml = "<input type=\"checkbox\" id=\"disptitletext\" disabled ";
        //if (!string.IsNullOrEmpty(preference_data.DisplayTitleText))
        //{
            //td_titletext.InnerHtml += " checked ";
        //}
        //td_titletext.InnerHtml += "name=\"disptitletext\">";
        //td_height.InnerHtml = preference_data.Height & "px"
        //td_width.InnerHtml = preference_data.Width & "px"

        td_verify_user_on_add.InnerHtml = "<input type=\"checkbox\" disabled ";
        if (settings_data.VerifyUserOnAdd)
        {
            td_verify_user_on_add.InnerHtml += " checked ";
        }
        td_verify_user_on_add.InnerHtml += " id=\"chkVerifyUserOnAdd\" name=\"chkVerifyUserOnAdd\" >";

        td_enable_preapproval.InnerHtml = "<input type=\"checkbox\" disabled ";
        if (settings_data.EnablePreApproval)
        {
            td_enable_preapproval.InnerHtml += " checked ";
        }
        td_enable_preapproval.InnerHtml += " id=\"chkEnablePreapproval\" name=\"chkEnablePreapproval\" >";

    }

    private void SetStrings()
    {
        ShutDownClick.ToolTip = ShutDownClick.Text = m_refMsg.GetMessage("lbl restart");
    }

    private void RegisterResources()
    {
        //CSS
        Ektron.Cms.API.Css.RegisterCss(this, Ektron.Cms.API.Css.ManagedStyleSheet.EktronUITabsCss);

        //JS
        Ektron.Cms.API.JS.RegisterJS(this, Ektron.Cms.API.JS.ManagedScript.EktronUICoreJS);
        Ektron.Cms.API.JS.RegisterJS(this, Ektron.Cms.API.JS.ManagedScript.EktronUITabsJS);
    }

    private string GetCheckValue(bool value)
    {
        if (value)
        {
            return (VerifyTrue);
        }
        else
        {
            return (VerifyFalse);
        }
    }

    private string GetAccessibilityValue(string value)
    {
        string sAccessibility = "";
        if ((value) == "1") //loose
        {
            sAccessibility = m_refMsg.GetMessage("access loose label");
        } //strict
        else if ((value) == "2")
        {
            sAccessibility = m_refMsg.GetMessage("access strict label");
        } //value = 0 or null
        else
        {
            sAccessibility = m_refMsg.GetMessage("access none label");
        }
        return (VerifyTrue + " " + sAccessibility);
    }

    protected void ShutDownClick_Click(object sender, System.EventArgs e)
    {
        string message = string.Empty;
        Exception ex;

        message = "--- Application Restarted ---" + "\r\n";
        message = message + "Host: " + Request.Url.Host + "\r\n";
        message = message + "Time: " + DateTime.Now.ToString() + "\r\n";
        message = message + "UserID: " + m_refUserApi.UserId.ToString() + "\r\n";
        message = message + "Username: " + m_refUserApi.RequestInformationRef.LoggedInUsername;
        ex = new Exception(message);
        EkException.LogException(ex, System.Diagnostics.EventLogEntryType.Error);
        HttpRuntime.UnloadAppDomain();
    }
}