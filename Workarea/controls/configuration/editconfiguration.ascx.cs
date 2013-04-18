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


public partial class editconfiguration : System.Web.UI.UserControl
{


    #region  Web Form Designer Generated Code

    protected StyleHelper m_refStyle = new StyleHelper();
    protected Ektron.Cms.Common.EkMessageHelper m_refMsg;
    protected string AppImgPath = "";
    protected string AppPath = "";
    protected string AppName = "";
    protected string SITEPATH = "";
    protected string VerifyTrue = "";
    protected string VerifyFalse = "";
    protected string m_SelectedEditControl = "";
    #endregion

    private void Page_Load(System.Object sender, System.EventArgs e)
    {
        RegisterResources();
        jsUniqueID.Text = "editconfiguration";
        m_refMsg = (new CommonApi()).EkMsgRef;
    }

    private void RegisterResources()
    {
        Ektron.Cms.Interfaces.Context.ICmsContextService cmsContextService = Ektron.Cms.Framework.UI.ServiceFactory.CreateCmsContextService();

        //CSS
        Ektron.Cms.API.Css.RegisterCss(this, Ektron.Cms.API.Css.ManagedStyleSheet.EktronUITabsCss);

        //JS
        Ektron.Cms.API.JS.RegisterJS(this, Ektron.Cms.API.JS.ManagedScript.EktronUICoreJS);
        Ektron.Cms.API.JS.RegisterJS(this, Ektron.Cms.API.JS.ManagedScript.EktronUITabsJS);
        Ektron.Cms.API.JS.RegisterJS(this, cmsContextService.WorkareaPath + "/java/jfunct.js", "EktronJFunctJS");
        Ektron.Cms.API.JS.RegisterJS(this, cmsContextService.WorkareaPath + "/java/toolbar_roll.js", "EktronToolbarRollJS");
    }

    private bool DisplayEditConfiguration()
    {
        SiteAPI m_refSiteApi = new SiteAPI();
        UserAPI m_refUserApi = new UserAPI();
        SettingsData settings_data;
        UserData user_data;
        UserGroupData[] group_data;
        UserPreferenceData preference_data;

        try
        {
            AppImgPath = m_refSiteApi.AppImgPath;
            AppName = m_refSiteApi.AppName;
            AppPath = m_refSiteApi.AppPath;
            SITEPATH = m_refSiteApi.SitePath;
            user_data = m_refUserApi.GetUserById(Ektron.Cms.Common.EkConstants.BuiltIn, false, false);
            preference_data = m_refUserApi.GetUserPreferenceById(0);
            group_data = m_refUserApi.GetAllUserGroups("GroupName");
            VerifyTrue = "<img src=\"" + AppPath + "images/UI/Icons/check.png\" border=\"0\" alt=\"Item is Enabled\" title=\"Item is Enabled\">";
            VerifyFalse = "<img src=\"" + AppImgPath + "icon_redx.gif\" border=\"0\" alt=\"Item is Disabled\" title=\"Item is Disabled\">";
            settings_data = m_refSiteApi.GetSiteVariables(m_refSiteApi.UserId, true);
            jsContentLanguage.Text = Convert.ToString(settings_data.Language);
            //VERSION
            td_version.InnerHtml = m_refMsg.GetMessage("version") + m_refSiteApi.Version + "&nbsp;" + m_refSiteApi.ServicePack;
            //BUILD NUMBER
            td_buildnumber.InnerHtml = "<i>(" + m_refMsg.GetMessage("build") + m_refSiteApi.BuildNumber + ")</i>";

            //Which Editor
            m_SelectedEditControl = Utilities.GetEditorPreference(Request);

            //LICENSE
			//defect: 60170
            //td_licensekey.InnerHtml = "<input type=\"text\" maxlength=\"4000\" name=\"license\" value=\"" + settings_data.LicenseKey + "\" />";
			td_licensekey.InnerHtml ="<TEXTAREA name=\"license\" rows=\"1\" cols=\"1\">" + settings_data.LicenseKey + "</TEXTAREA>";
            td_licensekey.InnerHtml += "<input type=\"hidden\" maxlength=\"4000\" name=\"license1\" value=\"" + settings_data.LicenseKey + "\" />";
            td_licensekey.InnerHtml += "<br/>";
            td_licensekey.InnerHtml += "<span class=\"ektronCaption\">" + m_refMsg.GetMessage("license key help text") + "</span>";
            //MODULE LICENSE
            System.Text.StringBuilder module_text = new System.Text.StringBuilder();
            int i = 0;
            if (!(settings_data.ModuleLicense == null))
            {
                for (i = 0; i <= settings_data.ModuleLicense.Length - 1; i++)
                {
                    if (i > 0)
                    {
                        module_text.Append("<div class=\"ektronTopSpaceSmall\"></div>");
                    }
                    module_text.Append(i + 1 + ". ");
                    module_text.Append("<input type=\"text\" maxlength=\"4000\" name=\"mlicense" + (i + 1) + "\" value=\"" + settings_data.ModuleLicense[i].License + "\">" + "\r\n");
                    module_text.Append("<input type=\"hidden\" name=\"mlicenseid" + (i + 1) + "\" value=\"" + settings_data.ModuleLicense[i].Id + "\">" + "\r\n");
                }
            }
            module_text.Append("<div class=\"ektronTopSpaceSmall\"></div>");
            module_text.Append(i + 1 + ". ");
            module_text.Append("<input type=\"text\" maxlength=\"4000\" name=\"mlicense" + (i + 1) + "\" value=\"\">" + "\r\n");
            module_text.Append("<input type=\"hidden\" name=\"mlicenseid" + (i + 1) + "\" value=\"0\">" + "\r\n");

            td_modulelicense.InnerHtml = module_text.ToString();
            //LANGUAGE LIST
            LanguageData[] active_lang_list;
            active_lang_list = m_refSiteApi.GetAllActiveLanguages();

            td_languagelist.InnerHtml = "<select id=\"language\" name=\"language\" selectedindex=\"0\">";
            if (!(active_lang_list == null))
            {
                for (i = 0; i <= active_lang_list.Length - 1; i++)
                {
                    //If (Convert.ToString(active_lang_list(i).Id) = settings_data.Language) Then
                    td_languagelist.InnerHtml += "<option  value=\"" + active_lang_list[i].Id + "\" ";
                    if (Convert.ToString(active_lang_list[i].Id) == settings_data.Language)
                    {
                        td_languagelist.InnerHtml += " selected";
                    }
                    td_languagelist.InnerHtml += "> " + active_lang_list[i].Name + "</option>";
                    //End If
                }
            }
            td_languagelist.InnerHtml += "</select>";

            //These settings only appliy to the eWebEditPro editor
            if (ConfigurationManager.AppSettings["ek_DataDesignControl"] != null && ConfigurationManager.AppSettings["ek_DataDesignControl"].ToString() == "eWebEditPro")
            {
                trSettings.Visible = true;
                trSummary.Visible = true;
                //MAX CONTENT SIZE
                td_maxcontent.InnerHtml = "<input type=\"Text\" maxlength=\"9\" size=\"9\" name=\"content_size\" value=\"" + settings_data.MaxContentSize + "\">";
                td_maxcontent.InnerHtml += "<br/>";
                td_maxcontent.InnerHtml += "<span class=\"ektronCaption\">" + m_refMsg.GetMessage("settings max content help text") + "</span>";

                //MAX SUMMARY SIZE

                td_maxsummary.InnerHtml = "<input type=\"Text\" maxlength=\"9\" size=\"9\" name=\"summary_size\" value=\"" + settings_data.MaxSummarySize + "\" >";
                td_maxsummary.InnerHtml += "<br/>";
                td_maxsummary.InnerHtml += "<span class=\"ektronCaption\">" + m_refMsg.GetMessage("settings max summary help text") + "</span>";
            }

            //SYSTEM EMAIL

            td_email.InnerHtml = "<input type=\"Text\" maxlength=\"50\" size=\"25\" name=\"SystemEmaillAddr\" value=\"" + settings_data.Email + "\">";
            td_email.InnerHtml += "<div class=\"ektronCaption\">";

            //EMAIL NOTIFICATION

            if (settings_data.EnableMessaging)
            {
                td_email.InnerHtml += "<input type=\"CHECKBOX\" name=\"EnableMessaging\" value=\"enable_msg\" CHECKED=\"True\">";
            }
            else
            {
                td_email.InnerHtml += "<input type=\"CHECKBOX\" name=\"EnableMessaging\" value=\"enable_msg\">";
            }
            td_email.InnerHtml += m_refMsg.GetMessage("enable mail messages") + "&nbsp;";
            td_email.InnerHtml += "</div>";

            //Server Type
            if (Convert.ToBoolean(settings_data.AsynchronousStaging))
            {
                td_server_type.InnerHtml += "<input type=\"CHECKBOX\" name=\"SystemAsynchStaging\" value=\"enable_msg\" CHECKED=\"True\">";
            }
            else
            {
                td_server_type.InnerHtml = "<input type=\"CHECKBOX\" name=\"SystemAsynchStaging\" value=\"enable_msg\" >";
            }
            td_server_type.InnerHtml += "Staging Server" + "&nbsp;";
            td_server_type.InnerHtml += "<br/>";
            td_server_type.InnerHtml += "<span class=\"ektronCaption\">" + m_refMsg.GetMessage("lbl enable server type message") + "</span>";


            //Asyncronous Processor Location

            if (!(settings_data.AsynchronousLocation == null))
            {
                // OldSystemAsynchLocation is needed because a disabled input field is not posted
                td_asynch_location.InnerHtml = "<input type=\"Hidden\" name=\"OldSystemAsynchLocation\" value=\"" + settings_data.AsynchronousLocation + "\">";
                td_asynch_location.InnerHtml += "<input type=\"Text\" maxlength=\"255\" name=\"SystemAsynchLocation\" value=\"" + settings_data.AsynchronousLocation + "\"";
            }
            else
            {
                td_asynch_location.InnerHtml = "<input type=\"Text\" maxlength=\"255\" name=\"SystemAsynchLocation\" value=\"\"";
            }
            td_asynch_location.InnerHtml += ">";
            td_asynch_location.InnerHtml += "<br/>";
            td_asynch_location.InnerHtml += "<span class=\"ektronCaption\">";
            td_asynch_location.InnerHtml += m_refMsg.GetMessage("lbl Example Location") + " http://localhost/CMS400Developer/Workarea/webservices/EktronAsyncProcessorWS.asmx";
            td_asynch_location.InnerHtml += "</span>";

            //PUBLISHPDF and PUBLISHHTML would check this flag.
            if (settings_data.PublishPdfSupported)
            {
                string schk = "";
                if (settings_data.PublishPdfEnabled)
                {
                    schk = " checked ";
                }
                PubPdf.Text = "<tr>";
                PubPdf.Text += "<td class=\"label shortLabel\">";
                PubPdf.Text += "<label for=\"PublishPdfEnabled\">" + m_refMsg.GetMessage("alt Enable office documents to be published in other format") + ":" + "</label>";
                PubPdf.Text += "</td>";
                PubPdf.Text += "<td class=\"value\">";
                PubPdf.Text += "<input type=\"checkbox\" name=\"PublishPdfEnabled\" id=\"PublishPdfEnabled\"" + schk + " >";
                PubPdf.Text += "</td>";
                PubPdf.Text += "</tr>";
            }
            else
            {
                PubPdf.Visible = false;
            }
            //LIBRARY FOLDER CREATION

            if (settings_data.FileSystemSupport)
            {
                td_libfolder.InnerHtml = "<input type=\"CHECKBOX\" name=\"filesystemsupport\" value=\"enable_msg\" CHECKED=\"True\" Onclick=\"javascript:return AreYouSure();\">";
            }
            else
            {
                td_libfolder.InnerHtml = "<input type=\"CHECKBOX\" name=\"filesystemsupport\" value=\"enable_msg\" Onclick=\"javascript: return AreYouSure()\">";
            }

            //BUILT IN USER
            //Dim x As System.Web.UI.WebControls.TextBox

            userid.Value = user_data.Id.ToString();
            username.Value = user_data.Username;
            TD_Pwd.InnerHtml = "<input type=\"password\" value=\"" + user_data.Password + "\" id=\"pwd\" name=\"pwd\" maxlength=\"50\" Onkeypress=\"javascript:return CheckKeyValue(event,\'34\');\">";
            TD_Pwd2.InnerHtml = "<input type=\"password\" value=\"" + user_data.Password + "\" id=\"confirmpwd\" name=\"confirmpwd\" maxlength=\"50\" Onkeypress=\"javascript:return CheckKeyValue(event,\'34\');\">";
            if ((m_refUserApi.RequestInformationRef.LoginAttempts != -1) && (m_refUserApi.IsAdmin() || m_refUserApi.UserId == 999999999 || (m_refUserApi.IsARoleMember(Ektron.Cms.Common.EkEnumeration.CmsRoleIds.AdminUsers))))
            {
                accountLocked.Text = "<input type=\"checkbox\" id=\"chkAccountLocked\" name=\"chkAccountLocked\" ";
                if (user_data.IsAccountLocked(m_refUserApi.RequestInformationRef))
                {
                    accountLocked.Text += " checked ";
                }
                accountLocked.Text += " />";
            }
            else
            {
                accountLocked.Text = "<input type=\"hidden\" id=\"chkAccountLocked\" name=\"chkAccountLocked\" ";
                if (user_data.IsAccountLocked(m_refUserApi.RequestInformationRef))
                {
                    accountLocked.Text += " value=\"on\" />";
                }
                else
                {
                    accountLocked.Text += " value=\"\" />";
                }
            }
            //styles.Items.Add(New ListItem(m_refMsg.GetMessage("remove styles"), "remove"))
            //styles.Items.Add(New ListItem(m_refMsg.GetMessage("do not remove styles"), ""))
            //If (settings_data.RemoveStyles) Then
            //    styles.Items(0).Selected = True
            //Else
            //    styles.Items(1).Selected = True
            //End If
            //styles.Attributes.Add("onClick", "javascript:checkWordStlyes();")
            //If (settings_data.RemoveStyles) Then
            //    'jsRemoveStyle = "1"
            //End If

            if (settings_data.EnableFontButtons)
            {
                font_style.Checked = true;
            }
            if (settings_data.PreserveWordStyles)
            {
                word_styles.Checked = true;
            }
            if (settings_data.PreserveWordClasses)
            {
                word_classes.Checked = true;
            }

            if (preference_data.Template != "")
            {
                templatefilename.Value = preference_data.Template;
            }
            if (Convert.ToString(preference_data.FolderId) == "")
            {
                chkSmartDesktop.Checked = true;
            }
            folderId.Value = preference_data.FolderId;
            if (preference_data.ForceSetting)
            {
                forcePrefs.Checked = true;
            }
            //if (preference_data.DisplayTitleText == "1")
            //{
                //disptitletext.Checked = true;
            //}
            //txtHeight.Value = Convert.ToString(preference_data.Height)
            //txtWidth.Value = Convert.ToString(preference_data.Width)

            if (settings_data.VerifyUserOnAdd)
            {
                chkVerifyUserOnAdd.Checked = true;
            }

            if (settings_data.EnablePreApproval)
            {
                chkEnablePreApproval.Checked = true;
            }

            access_def.Value = settings_data.Accessibility;

            SetAccessibilityRadioButtons();

            return (false);
        }
        catch
        {
            return true;
        }
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
    public bool EditConfigurationControl()
    {
        bool result = false;
        this.ID = "editconfiguration";
        if (!(IsPostBack))
        {

            result = DisplayEditConfiguration();
        }
        else
        {
            result = ProcessSubmission();
        }
        return (result);
    }
    public bool ProcessSubmission()
    {
        SiteAPI m_refSiteApi = new SiteAPI();
        Hashtable pagedata;
        Hashtable modulelicense;
        Hashtable modulelicenses = new Hashtable();
        Collection prefs = new Collection();
        long FolderId;
        int i = 0;

        pagedata = new Hashtable();

        while (Request.Form["mlicenseid" + (i + 1)] != null && Request.Form["mlicenseid" + (i + 1)].ToString().Length > 0)
        {
            modulelicense = new Hashtable();
            modulelicense.Add("ID", System.Convert.ToInt32(Request.Form["mlicenseid" + (i + 1)]));
            modulelicense.Add("License", Request.Form["mlicense" + (i + 1)]);
            modulelicense.Add("Type", 0);
            modulelicenses.Add(i, modulelicense);
            i++;
            modulelicense = null;
        }
        pagedata.Add("ModuleLicenses", modulelicenses);
        pagedata.Add("LicenseKey", Request.Form["license"]);
        pagedata.Add("AppLanguage", Request.Form["language"]);
        pagedata.Add("SystemEmail", Request.Form["SystemEmaillAddr"]);
        if (Request.Form["SystemAsynchStaging"] != null && (Request.Form["SystemAsynchStaging"]).ToString().Length > 0)
        {
            pagedata.Add("AsynchStaging", 1);
            m_refSiteApi.RequestInformationRef.IsStaging = true;
        }
        else
        {
            pagedata.Add("AsynchStaging", 0);
            m_refSiteApi.RequestInformationRef.IsStaging = false;
        }
        if (Request.Form["SystemAsynchLocation"] != null && (Request.Form["SystemAsynchLocation"]).ToString().Length > 0)
        {
            pagedata.Add("AsynchLocation", Request.Form["SystemAsynchLocation"]);
        }
        else
        {
            pagedata.Add("AsynchLocation", Request.Form["SystemAsynchLocation"]);
        }
        //These settings only appliy to the eWebEditPro editor, other wise set the defaut values in it.
        if (ConfigurationManager.AppSettings["ek_DataDesignControl"] != null && ConfigurationManager.AppSettings["ek_DataDesignControl"].ToString() == "eWebEditPro")
        {
            int ContentSize = 1000000;
            int SummarySize = 65000;
            if (int.TryParse(Request.Form["content_size"], out ContentSize))
            {
                if (ContentSize < 1000000)
                    ContentSize = 1000000;
            }
            if (int.TryParse(Request.Form["summary_size"], out SummarySize))
            {
                if (SummarySize < 65000)
                    SummarySize = 65000;
            }
            pagedata.Add("MaxContentSize", ContentSize.ToString());
            pagedata.Add("MaxSummarySize", SummarySize.ToString());
        }
        else
        {
            pagedata.Add("MaxContentSize", "1000000");
            pagedata.Add("MaxSummarySize", "65000");
        }
        //End content size
        if (Request.Form["EnableMessaging"] != null && Request.Form["EnableMessaging"] != "")
        {
            pagedata.Add("EnableMessaging", 1);
        }
        else
        {
            pagedata.Add("EnableMessaging", 0);
        }
        if (Request.Form["filesystemsupport"] != null && Request.Form["filesystemsupport"] != "")
        {
            pagedata.Add("FileSystemSupport", 1);
        }
        else
        {
            pagedata.Add("FileSystemSupport", 0);
        }
        if (!(Request.Form["PublishPdfEnabled"] == null))
        {
            if (Request.Form["PublishPdfEnabled"] != "")
            {
                pagedata.Add("PublishPdfEnabled", 1);
            }
            else
            {
                pagedata.Add("PublishPdfEnabled", 0);
            }
        }
        else
        {
            pagedata.Add("PublishPdfEnabled", 0);
        }
        if (Request.Form[font_style.UniqueID] != null && (Request.Form[font_style.UniqueID]).ToString().Length > 0)
        {
            pagedata.Add("EnableFontButtons", 1);
        }
        else
        {
            pagedata.Add("EnableFontButtons", 0);
        }
        //If (Len(Request.Form(styles.UniqueID))) Then
        //    pagedata.Add("RemoveStyles", 1)
        //Else
        //    pagedata.Add("RemoveStyles", 0)
        //End If
        if (Request.Form[word_styles.UniqueID] != null && (Request.Form[word_styles.UniqueID]).ToString().Length > 0)
        {
            pagedata.Add("PreserveWordStyles", 1);
        }
        else
        {
            pagedata.Add("PreserveWordStyles", 0);
        }
        if (Request.Form[word_classes.UniqueID] != null && (Request.Form[word_classes.UniqueID]).ToString().Length > 0)
        {
            pagedata.Add("PreserveWordClasses", 1);
        }
        else
        {
            pagedata.Add("PreserveWordClasses", 0);
        }
        pagedata.Add("PreApprovalGroup", "0");
        if (Request.Form[chkVerifyUserOnAdd.UniqueID] == "on")
        {
            pagedata.Add("VerifyUserOnAdd", "1");
        }
        else
        {
            pagedata.Add("VerifyUserOnAdd", "0");
        }
        if (Request.Form[chkEnablePreApproval.UniqueID] == "on")
        {
            pagedata.Add("EnablePreApproval", "1");
        }
        else
        {
            pagedata.Add("EnablePreApproval", "0");
        }
        //if (Request.Form["access"] != null && Request.Form["access"].ToString().Length > 0)
        //{
        //    pagedata.Add("accessibility", Request.Form["access"]);
        //}
        //else
        //{
        //    pagedata.Add("accessibility", 0);
        //}

        //if (access_none.Selected)
        //{
        //    pagedata.Add("accessibility", access_none.Value);
        //}
        //else if (access_loose.Selected)
        //{
        //    pagedata.Add("accessibility", access_loose.Value);
        //}
        //else if (access_strict.Selected)
        //{
        //    pagedata.Add("accessibility", access_strict.Value);
        //}
        //else
        //{
        //    pagedata.Add("accessibility", 0);
        //}

        if (Request.Form[access.UniqueID] != null)
        {
            pagedata.Add("accessibility", Request.Form[access.UniqueID]);
        }
        else
        {
            pagedata.Add("accessibility", 0);
        }


        prefs.Add("9999", "width", null, null);
        prefs.Add("9999", "height", null, null);
        prefs.Add(Request.Form[templatefilename.UniqueID], "template", null, null);
        if (Request.Form[chkSmartDesktop.UniqueID] == "on")
        {
            FolderId = -1;
        }
        else
        {
            if (Request.Form["folderId"] != "")
            {
                FolderId = Convert.ToInt64(Request.Form["folderId"]);
            }
            else
            {
                FolderId = 0;
            }
        }
        prefs.Add(FolderId, "folderid", null, null);
        if (Request.Form[forcePrefs.UniqueID] == "on")
        {
            prefs.Add("1", "forcesetting", null, null);
        }
        else
        {
            prefs.Add("0", "forcesetting", null, null);
        }

        prefs.Add("1", "dispborders", null, null);

        //if (Request.Form[disptitletext.UniqueID] == "on")
        //{
        //    prefs.Add("1", "disptitletext", null, null);
        //}
        //else
        //{
        //    prefs.Add("0", "disptitletext", null, null);
        //}

        m_refSiteApi.UpdateSiteVariables(pagedata);
        Ektron.Cms.DataIO.LicenseManager.LicenseManager.Reset(m_refSiteApi.RequestInformationRef);
        Collection oldUser = new Collection();
        oldUser.Add(Request.Form[userid.UniqueID], "UserID", null, null);
        oldUser.Add(Request.Form[username.UniqueID], "UserName", null, null);
        oldUser.Add(Request.Form["pwd"], "Password", null, null);
        oldUser.Add("", "Domain", null, null);
        oldUser.Add("BUILTIN", "FirstName", null, null);
        oldUser.Add("BUILTIN", "LastName", null, null);
        oldUser.Add("0", "Language", null, null);
        oldUser.Add("", "EditorOptions", null, null);
        oldUser.Add("", "EmailAddr1", null, null);
        oldUser.Add("1", "DisableMsg", null, null);
        if (Request.Form["chkAccountLocked"] != null && Request.Form["chkAccountLocked"] != "")
        {
            oldUser.Add(254, "LoginAttempts", null, null);
        }
        else
        {
            oldUser.Add(0, "LoginAttempts", null, null);
        }
        UserAPI m_refUserApi = new UserAPI();
        m_refUserApi.UpdateUser(oldUser);
        m_refUserApi.UpdateUserPreferences(0, prefs);

        return (true);
    }

    private void SetAccessibilityRadioButtons()
    {
        if (access_def.Value == "0" || access_def.Value == "")
        {
            access_none.Selected = true;
            //access_none.Attributes["title"] = m_refMsg.GetMessage("access none label");
        }

        if (access_def.Value == "1")
        {
            access_loose.Selected = true;
            //access_loose.Attributes["title"] = m_refMsg.GetMessage("access loose label");
        }

        if (access_def.Value == "2")
        {
            access_strict.Selected = true;
            //access_strict.Attributes["title"] = m_refMsg.GetMessage("access strict label");
        }
        access_none.Text = m_refMsg.GetMessage("access none label");
        access_loose.Text = m_refMsg.GetMessage("access loose label");
        access_strict.Text = m_refMsg.GetMessage("access strict label");
    }

}


