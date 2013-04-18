using System;
using System.Data;
using System.IO;
using System.Configuration;
using System.Collections;
using System.Text;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;

public partial class EkStatus : System.Web.UI.Page
{
    //----- Constants for page.
    private const int ERR_DEFAULT = 0x0001;
    private const int ERR_NOT_LOCAL = 0x0002;

    private const string ERR_DEFAULT_RESPONSE = "Unknown error while loading EkStatus diagnostic utility";
    private const string ERR_NOT_LOCAL_RESPONSE = "EkStatus diagnostic utility is only accessible from the local machine.";

    private const string EVENT_CMS = "CMS400";
    private const string EVENT_APPLICATION = "Application";
    private const string EVENT_SECURITY = "Security";
    private const string EVENT_SYSTEM = "System";
    private const string EVENT_CMS_HEADER = "CMS400Events";
    private const string EVENT_APP_HEADER = "applicationHeader";
    private const string EVENT_SYS_HEADER = "systemHeader";
    private const string DMS_CAT_LOC = "DMSCatLocation";
    private const string DMS_STOR_LOC = "DMSStorageLocation";
    private const string DMS_DMDATA_LOC = "DMSdmdata";
    private const string DMS_ASSETS_LOC = "DMSassets";
    private const string DMS_CAT_LOC_HEADER = "DMSCatLocationHeader";
    private const string DMS_STOR_LOC_HEADER = "DMSStorageLocationHeader";
    private const string DMS_DMDATA_LOC_HEADER = "DMSdmdataHeader";
    private const string DMS_ASSETS_LOC_HEADER = "DMSassetsHeader";

    private bool m_bSmtpDefined = false;
    private string m_sSmtpServer = "";
    private string m_sSmtpPort = "";
    private string m_sSmtpUser = "";
    private string m_sSmtpPass = "";
    private bool m_bADEnabled = false;
    private bool m_bADAdvancedConfig = false;
    private string m_sADAuthProtocol = "";
    private string m_sADUser = "";
    private string m_sADPass = "";
    private int m_iAdAuth = 0;
    private bool m_bAdInt = false;
    private bool m_bAdAutoUser = false;
    private bool m_bAdAutoUsertoGroup = false;
    private string m_sADDomain = "";

    protected void Page_Load(object sender, EventArgs e)
    {
        StringBuilder saPage = new StringBuilder();

        //----- The header will always be the same.
        saPage.Append(LoadHeader());

        if (!this.Context.Request.IsLocal)
        {
            saPage.Append(LoadError(ERR_NOT_LOCAL));
        }
        else
        {
            try
            {
                ArrayList saTabsToCreate = new ArrayList();

                if (Page.Session["smtpServer"] == null || this.Request.Form["emailAddressTo"] == null || this.Request.Form["emailAddressTo"] == ",")
                {
                    saPage.Append(LoadTabContents(ref saTabsToCreate));
                    saPage.Append(LoadTabUI(saTabsToCreate));
                }
                else
                {
                    if (this.Request.Form["emailAddressTo"] != null)
                        Page.Session.Add("emailAddressTo", this.Request.Form["emailAddressTo"].TrimEnd(','));
                    if (this.Request.Form["emailComments"] != null)
                        Page.Session.Add("emailComments", this.Request.Form["emailComments"].TrimEnd(','));
                    if (this.Request.Form["emailAddressFrom"] != null)
                        Page.Session.Add("emailAddressFrom", this.Request.Form["emailAddressFrom"].TrimEnd(','));

                    this.Server.Transfer("sendMail.aspx");
                }
            }
            catch (Exception exThrown)
            {
                saPage.Append(LoadError(ERR_DEFAULT, "<b>Error:</b><br />" + exThrown.Message + "<br /><br />" + "<b>Source:</b><br />" + exThrown.Source + "<br /><br />" + "<b>Stack Trace:</b><br />" + exThrown.StackTrace));
            }
        }

        //----- The footer will always be the same.
        saPage.Append(LoadFooter());

        //if (!IsPostBack)  //This causes all the info to be written to htm file that can be accessed from any where- Security Issue- so removing the output fie
        //    File.WriteAllText(HttpContext.Current.Server.MapPath("") + "\\EkStatusOutput.htm", saPage.ToString());

        this.Page.Response.Write(saPage.ToString());
    }

    //----- Creates the page header.
    private string LoadHeader()
    {
        StringBuilder sbReturn = new StringBuilder();

        sbReturn.AppendLine("<%%>");
        sbReturn.AppendLine("<!DOCTYPE html PUBLIC \"-//W3C//DTD XHTML 1.0 Transitional//EN\" \"http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd\">");
        sbReturn.AppendLine("<html xmlns=\"http://www.w3.org/1999/xhtml\">");
        sbReturn.AppendLine("<head runat=\"server\">");
        sbReturn.AppendLine("   <title>CMS400.NET Diagnostic Panel</title>");
        sbReturn.AppendLine("   <link rel='stylesheet' type='text/css' href='com.ektron.diagnostics.ui.css'/>");
        sbReturn.AppendLine("   <link rel='stylesheet' type='text/css' href='com.ektron.ui.tabs.css'/>");
        sbReturn.AppendLine("   <script type='text/javascript' language=\"javascript\" src='com.ektron.ui.tabs.js'></script>");
        sbReturn.AppendLine("</head>");
        sbReturn.AppendLine("<body>");
        sbReturn.AppendLine("   <script type='text/javascript' language=\"javascript\">");
        sbReturn.AppendLine("        function logHeaderClick(sName)");
        sbReturn.AppendLine("        {");
        sbReturn.AppendLine("            var container = document.getElementById(sName);");
        sbReturn.AppendLine("            container.style.display=\"inline\";");
        sbReturn.AppendLine("            if(sName == \"" + EVENT_CMS + "\")");
        sbReturn.AppendLine("            {");
        sbReturn.AppendLine("                container = document.getElementById(\"" + EVENT_APPLICATION + "\");");
        sbReturn.AppendLine("                container.style.display=\"none\";");
        sbReturn.AppendLine("                container = document.getElementById(\"" + EVENT_CMS_HEADER + "\");");
        sbReturn.AppendLine("                container.style.color=\"red\";");
        sbReturn.AppendLine("                container = document.getElementById(\"" + EVENT_APP_HEADER + "\");");
        sbReturn.AppendLine("                container.style.color=\"black\";");
        sbReturn.AppendLine("            }");
        sbReturn.AppendLine("            else if(sName == \"" + EVENT_APPLICATION + "\")");
        sbReturn.AppendLine("            {");
        sbReturn.AppendLine("                container = document.getElementById(\"" + EVENT_CMS + "\");");
        sbReturn.AppendLine("                container.style.display=\"none\";");
        sbReturn.AppendLine("                container = document.getElementById(\"" + EVENT_APP_HEADER + "\");");
        sbReturn.AppendLine("                container.style.color=\"red\";");
        sbReturn.AppendLine("                container = document.getElementById(\"" + EVENT_CMS_HEADER + "\");");
        sbReturn.AppendLine("                container.style.color=\"black\";");
        sbReturn.AppendLine("            }");
        sbReturn.AppendLine("            else");
        sbReturn.AppendLine("            {");
        sbReturn.AppendLine("                container = document.getElementById(\"" + EVENT_CMS + "\");");
        sbReturn.AppendLine("                container.style.display=\"none\";");
        sbReturn.AppendLine("                container = document.getElementById(\"" + EVENT_APPLICATION + "\");");
        sbReturn.AppendLine("                container.style.display=\"none\";");
        sbReturn.AppendLine("                container = document.getElementById(\"" + EVENT_CMS_HEADER + "\");");
        sbReturn.AppendLine("                container.style.color=\"black\";");
        sbReturn.AppendLine("                container = document.getElementById(\"" + EVENT_APP_HEADER + "\");");
        sbReturn.AppendLine("                container.style.color=\"black\";");
        sbReturn.AppendLine("            }");
        sbReturn.AppendLine("        }");
        sbReturn.AppendLine("        function logMouseOver(sName)");
        sbReturn.AppendLine("        {");
        sbReturn.AppendLine("            var container = document.getElementById(sName);");
        sbReturn.AppendLine("            if(container.style.color != \"red\")");
        sbReturn.AppendLine("               container.style.color=\"blue\";");
        sbReturn.AppendLine("            container.style.cursor=\"hand\";");
        sbReturn.AppendLine("        }");
        sbReturn.AppendLine("        function logMouseOut(sName)");
        sbReturn.AppendLine("        {");
        sbReturn.AppendLine("            var container = document.getElementById(sName);");
        sbReturn.AppendLine("            if(container.style.color != \"red\")");
        sbReturn.AppendLine("               container.style.color=\"black\";");
        sbReturn.AppendLine("            container.style.cursor=\"auto\";");
        sbReturn.AppendLine("        }");
        sbReturn.AppendLine("        function permissionHeaderClick(sName)");
        sbReturn.AppendLine("        {");
        sbReturn.AppendLine("            var container = document.getElementById(sName);");
        sbReturn.AppendLine("            container.style.display=\"inline\";");
        sbReturn.AppendLine("            if(sName == \"" + DMS_CAT_LOC + "\")");
        sbReturn.AppendLine("            {");
        sbReturn.AppendLine("                container = document.getElementById(\"" + DMS_STOR_LOC + "\");");
        sbReturn.AppendLine("                container.style.display=\"none\";");
        sbReturn.AppendLine("                container = document.getElementById(\"" + DMS_STOR_LOC_HEADER + "\");");
        sbReturn.AppendLine("                container.style.color=\"black\";");
        sbReturn.AppendLine("                container = document.getElementById(\"" + DMS_DMDATA_LOC + "\");");
        sbReturn.AppendLine("                container.style.display=\"none\";");
        sbReturn.AppendLine("                container = document.getElementById(\"" + DMS_DMDATA_LOC_HEADER + "\");");
        sbReturn.AppendLine("                container.style.color=\"black\";");
        sbReturn.AppendLine("                container = document.getElementById(\"" + DMS_ASSETS_LOC + "\");");
        sbReturn.AppendLine("                container.style.display=\"none\";");
        sbReturn.AppendLine("                container = document.getElementById(\"" + DMS_ASSETS_LOC_HEADER + "\");");
        sbReturn.AppendLine("                container.style.color=\"black\";");
        sbReturn.AppendLine("                container = document.getElementById(\"" + DMS_CAT_LOC_HEADER + "\");");
        sbReturn.AppendLine("                container.style.color=\"red\";");
        sbReturn.AppendLine("            }");
        sbReturn.AppendLine("            else if(sName == \"" + DMS_STOR_LOC + "\")");
        sbReturn.AppendLine("            {");
        sbReturn.AppendLine("                container = document.getElementById(\"" + DMS_CAT_LOC + "\");");
        sbReturn.AppendLine("                container.style.display=\"none\";");
        sbReturn.AppendLine("                container = document.getElementById(\"" + DMS_CAT_LOC_HEADER + "\");");
        sbReturn.AppendLine("                container.style.color=\"black\";");
        sbReturn.AppendLine("                container = document.getElementById(\"" + DMS_DMDATA_LOC + "\");");
        sbReturn.AppendLine("                container.style.display=\"none\";");
        sbReturn.AppendLine("                container = document.getElementById(\"" + DMS_DMDATA_LOC_HEADER + "\");");
        sbReturn.AppendLine("                container.style.color=\"black\";");
        sbReturn.AppendLine("                container = document.getElementById(\"" + DMS_ASSETS_LOC + "\");");
        sbReturn.AppendLine("                container.style.display=\"none\";");
        sbReturn.AppendLine("                container = document.getElementById(\"" + DMS_ASSETS_LOC_HEADER + "\");");
        sbReturn.AppendLine("                container.style.color=\"black\";");
        sbReturn.AppendLine("                container = document.getElementById(\"" + DMS_STOR_LOC_HEADER + "\");");
        sbReturn.AppendLine("                container.style.color=\"red\";");
        sbReturn.AppendLine("            }");
        sbReturn.AppendLine("            else if(sName == \"" + DMS_DMDATA_LOC + "\")");
        sbReturn.AppendLine("            {");
        sbReturn.AppendLine("                container = document.getElementById(\"" + DMS_CAT_LOC + "\");");
        sbReturn.AppendLine("                container.style.display=\"none\";");
        sbReturn.AppendLine("                container = document.getElementById(\"" + DMS_CAT_LOC_HEADER + "\");");
        sbReturn.AppendLine("                container.style.color=\"black\";");
        sbReturn.AppendLine("                container = document.getElementById(\"" + DMS_STOR_LOC + "\");");
        sbReturn.AppendLine("                container.style.display=\"none\";");
        sbReturn.AppendLine("                container = document.getElementById(\"" + DMS_STOR_LOC_HEADER + "\");");
        sbReturn.AppendLine("                container.style.color=\"black\";");
        sbReturn.AppendLine("                container = document.getElementById(\"" + DMS_ASSETS_LOC + "\");");
        sbReturn.AppendLine("                container.style.display=\"none\";");
        sbReturn.AppendLine("                container = document.getElementById(\"" + DMS_ASSETS_LOC_HEADER + "\");");
        sbReturn.AppendLine("                container.style.color=\"black\";");
        sbReturn.AppendLine("                container = document.getElementById(\"" + DMS_DMDATA_LOC_HEADER + "\");");
        sbReturn.AppendLine("                container.style.color=\"red\";");
        sbReturn.AppendLine("            }");
        sbReturn.AppendLine("            else if(sName == \"" + DMS_ASSETS_LOC + "\")");
        sbReturn.AppendLine("            {");
        sbReturn.AppendLine("                container = document.getElementById(\"" + DMS_CAT_LOC + "\");");
        sbReturn.AppendLine("                container.style.display=\"none\";");
        sbReturn.AppendLine("                container = document.getElementById(\"" + DMS_CAT_LOC_HEADER + "\");");
        sbReturn.AppendLine("                container.style.color=\"black\";");
        sbReturn.AppendLine("                container = document.getElementById(\"" + DMS_STOR_LOC + "\");");
        sbReturn.AppendLine("                container.style.display=\"none\";");
        sbReturn.AppendLine("                container = document.getElementById(\"" + DMS_STOR_LOC_HEADER + "\");");
        sbReturn.AppendLine("                container.style.color=\"black\";");
        sbReturn.AppendLine("                container = document.getElementById(\"" + DMS_DMDATA_LOC + "\");");
        sbReturn.AppendLine("                container.style.display=\"none\";");
        sbReturn.AppendLine("                container = document.getElementById(\"" + DMS_DMDATA_LOC_HEADER + "\");");
        sbReturn.AppendLine("                container.style.color=\"black\";");
        sbReturn.AppendLine("                container = document.getElementById(\"" + DMS_ASSETS_LOC_HEADER + "\");");
        sbReturn.AppendLine("                container.style.color=\"red\";");
        sbReturn.AppendLine("            }");
        sbReturn.AppendLine("        }");
        sbReturn.AppendLine("        function permissionMouseOver(sName)");
        sbReturn.AppendLine("        {");
        sbReturn.AppendLine("            var container = document.getElementById(sName);");
        sbReturn.AppendLine("            if(container.style.color != \"red\")");
        sbReturn.AppendLine("               container.style.color=\"blue\";");
        sbReturn.AppendLine("            container.style.cursor=\"hand\";");
        sbReturn.AppendLine("        }");
        sbReturn.AppendLine("        function permissionMouseOut(sName)");
        sbReturn.AppendLine("        {");
        sbReturn.AppendLine("            var container = document.getElementById(sName);");
        sbReturn.AppendLine("            if(container.style.color != \"red\")");
        sbReturn.AppendLine("               container.style.color=\"black\";");
        sbReturn.AppendLine("            container.style.cursor=\"auto\";");
        sbReturn.AppendLine("        }");
        sbReturn.AppendLine("   </script>");
        sbReturn.AppendLine("   <form name=\"email\" method=\"post\" >");
        sbReturn.AppendLine("   <img src=\"./images/cms400-25.gif\" class=\"logo\" alt=\"cms400.net Diagnostic Panel\"/>");
        sbReturn.AppendLine("   <div style=\"background:#ccc;width:100%;height:1px;overflow:hidden;\"></div>");
        sbReturn.AppendLine("   <div style=\"margin-bottom:25px;font-size:10pt;\"><b>CMS400.NET</b> Diagnostics");
        if (this.Context.Request.IsLocal)
        {
        sbReturn.Append("(<a href=\"http://" + Server.MachineName + "\" target=\"_blank\">" + Server.MachineName + "</a>)</div>");
        }
        sbReturn.AppendLine("   <div id=\"tabsContainer\"></div>");

        return sbReturn.ToString();
    }

    //----- Creates the dynamic page footer.
    private string LoadFooter()
    {
        StringBuilder sbReturn = new StringBuilder();

        sbReturn.AppendLine("   </form>");
        sbReturn.AppendLine("</body>");
        sbReturn.AppendLine("</html>");

        return sbReturn.ToString();
    }

    private string LoadTabUI(ArrayList saTabNames)
    {
        StringBuilder sbReturn = new StringBuilder();

        sbReturn.AppendLine("<script type='text/javascript' language=\"javascript\">");
        //sbReturn.AppendLine("var tabPanel = new TabPanel( \"tabsContainer\", 640, \"100%\", 70, 25 );");
        sbReturn.AppendLine("var tabPanel = new TabPanel( \"tabsContainer\", 710, 500, 70, 25 );");

        for (int i = 0; i < saTabNames.Count; i++)
        {
            string sTemp = saTabNames[i].ToString();
            sbReturn.AppendLine("tabPanel.addTab( \"" + sTemp + "\");");
            sbReturn.AppendLine("tabPanel.setPanel( \"" + sTemp + "\", document.getElementById(\"" + sTemp + "\").innerHTML );");
        }

        sbReturn.AppendLine("tabPanel.display();");
        sbReturn.AppendLine("window.resizeTo(765,800);");
        sbReturn.AppendLine("</script>");

        return sbReturn.ToString();
    }

    private string LoadTabContents(ref ArrayList saReturn)
    {
        StringBuilder sbReturn = new StringBuilder();
        EkVersion versionInfo = new EkVersion();
        EkDb dbInfo = new EkDb();
        EkSettings systemSettings = new EkSettings(HttpContext.Current.Server.MapPath("~"));
        EkConnections connStatus = new EkConnections(systemSettings.webConfigSettings);
        int nRowsReturned = 0;
        int nEventEntries = 50;
        string sIndexerState = "";
        string[] saUsers = { "admin", "admin2", "admin3", "jadmin", "vs", "explorer", "spanish", "jedit", "jmember", "supermember"};
        string[] saPasswrds = { "L4BqG+ErfFAMIOA/1rPvZA==", "L4BqGyo65VMMIOA/1rPvZA==", "L4BqG9dqrTcMIOA/1rPvZA==", "VHNR0zFL9+wMIOA/1rPvZA==", "9l3CwTXDEjkMIOA/1rPvZA==", "7CE5uGtjsc5toeQW1rPvZA==", "jMhvs7ut5q0MIOA/1rPvZA==", "/2UL1VsF/aoMIOA/1rPvZA==", "KT9/PwlZbo0MIOA/1rPvZA==", "UP7nv1Pr2naEh6PD1rPvZA==" };
        string sPassword = "";
        string sLicKey = "";

        //----- Begin building Version information.
        sbReturn.AppendLine("   <div id=\"Version\" style=\"display:none;\">");
        sbReturn.AppendLine("       <h3>Version Information</h3>");
        sbReturn.AppendLine("       <div class=\"field\">");
        sbReturn.AppendLine("       <span class=\"fieldName\">CMS400 Version</span>");
        sbReturn.AppendLine("           <span class=\"fieldValue\">" + versionInfo.GetCMS400Version() + "</span>");
        sbReturn.AppendLine("       </div>");
        sbReturn.AppendLine("       <div class=\"field\">");
        sbReturn.AppendLine("       <span class=\"fieldName\">Ektron Windows Services 2.0 Version</span>");
        sbReturn.AppendLine("           <span class=\"fieldValue\">" + versionInfo.GetEktronWindowsServicesVersion() + "</span>");
        sbReturn.AppendLine("       </div>");
        sbReturn.AppendLine("       <div class=\"field\">");
        sbReturn.AppendLine("       <span class=\"fieldName\">Ektron Windows Services 3.0 Version</span>");
        sbReturn.AppendLine("           <span class=\"fieldValue\">" + versionInfo.GetEktronWindowsServices30Version() + "</span>");
        sbReturn.AppendLine("       </div>");
        sbReturn.AppendLine("       <div class=\"field\">");
        sbReturn.AppendLine("       <span class=\"fieldName\">Ektron Windows Services 4.0 Version</span>");
        sbReturn.AppendLine("           <span class=\"fieldValue\">" + versionInfo.GetEktronWindowsServices40Version() + "</span>");
        sbReturn.AppendLine("       </div>");
        sbReturn.AppendLine("       <div class=\"field\">");
        sbReturn.AppendLine("           <span class=\"fieldName\">Server OS Version</span>");
        sbReturn.AppendLine("           <span class=\"fieldValue\">" + versionInfo.GetServerOSVersion() + "</span>");
        sbReturn.AppendLine("       </div>");
        sbReturn.AppendLine("       <div class=\"field\">");
        sbReturn.AppendLine("           <span class=\"fieldName\">IIS Version</span>");
        sbReturn.AppendLine("           <span class=\"fieldValue\">" + versionInfo.GetIISVersion() + "</span>");
        sbReturn.AppendLine("       </div>");
        sbReturn.AppendLine("       <div class=\"field\">");
        sbReturn.AppendLine("           <span class=\"fieldName\">.NET Runtime Version</span>");
        sbReturn.AppendLine("           <span class=\"fieldValue\">" + versionInfo.GetDotNetVersion() + "</span>");
        sbReturn.AppendLine("       </div>");
        sbReturn.AppendLine("       <div class=\"field\">");
        sbReturn.AppendLine("           <span class=\"fieldName\">Visual Studio Version</span>");
        sbReturn.AppendLine("           <span class=\"fieldValue\">" + versionInfo.GetVSVersion() + "</span>");
        sbReturn.AppendLine("       </div>");
        sbReturn.AppendLine("   </div>");
        //----- Add tab to list, will be loaded later in LoadTabUI
        saReturn.Add("Version");
        //----- End building Version information.

        //----- Begin building License information.
        sbReturn.AppendLine("   <div id=\"License\" style=\"display:none;\">");
        sbReturn.AppendLine("       <h3>License Information</h3>");

        if (!dbInfo.GetLicenseKey(ref sLicKey))
        {
            sbReturn.AppendLine("       <div class=\"field\">");
            sbReturn.AppendLine("       <span class=\"fieldName\">License Key</span>");
            sbReturn.AppendLine("           <span class=\"fieldValue\"><span class=\"statusError\">Failed</span>Could not get license key. ERROR: " + dbInfo.lastError + "</span>");
            sbReturn.AppendLine("       </div>");
        }
        else
        {
            string sTempLic;
            string[] saLicKeys;
            saLicKeys = sLicKey.Split(';');

            for (int k = 0; k < saLicKeys.Length; k++)
            {
                sbReturn.AppendLine("       <div class=\"field\">");
                sbReturn.AppendLine("       <span class=\"fieldName\">License Key</span>");
                sbReturn.AppendLine("           <span class=\"fieldValue\"><span class=\"statusSuccess\">OK</span> " + saLicKeys[k] + "</span>");
                sbReturn.AppendLine("       </div>");

                sbReturn.AppendLine("       <div class=\"field\">");
                sbReturn.AppendLine("       <span class=\"fieldName\">Expiration Date</span>");
                
                if (saLicKeys[k].IndexOf("(exp-") != -1)
                {
                    //sTempLic = "2006-04-11";
                    sTempLic = saLicKeys[k].Substring(saLicKeys[k].IndexOf("(exp-") + "(exp-".Length, 10);

                    string[] saDate = sTempLic.Split('-');
                    DateTime dtExpires = new DateTime(Convert.ToInt32(saDate[0]), Convert.ToInt32(saDate[1]), Convert.ToInt32(saDate[2]));

                    sTempLic = dtExpires.ToShortDateString();

                    if (dtExpires.CompareTo(DateTime.Today) >= 0)
                        sbReturn.AppendLine("           <span class=\"fieldValue\"><span class=\"statusSuccess\">OK</span> " + sTempLic + "</span>");
                    else
                        sbReturn.AppendLine("           <span class=\"fieldValue\"><span class=\"statusError\">Failed</span> " + sTempLic + "</span>");
                }
                else
                {
                    sbReturn.AppendLine("           <span class=\"fieldValue\"><span class=\"statusSuccess\">OK</span> No Expiration</span>");
                }

                sbReturn.AppendLine("       </div>");

                sbReturn.AppendLine("       <div class=\"field\">");
                sbReturn.AppendLine("       <span class=\"fieldName\">Enabled Features</span>");
                sTempLic = "";
                if (saLicKeys[k].Contains("(AD)"))
                    sTempLic += "Active Directory";
                if (saLicKeys[k].Contains("(XML)"))
                    sTempLic += sTempLic.Length > 0 ? ", eWebEditPro+XML" : "eWebEditPro+XML";
                if (saLicKeys[k].Contains("(DMS)"))
                    sTempLic += sTempLic.Length > 0 ? ", Document Management System" : "Document Management System";
                if (saLicKeys[k].Contains("(P)"))
                    sTempLic += sTempLic.Length > 0 ? ", Site Replication" : "Site Replication";
                sbReturn.AppendLine("           <span class=\"fieldValue\">" + sTempLic + "</span>");
                sbReturn.AppendLine("       </div>");

                sbReturn.AppendLine("       <div class=\"field\">");
                sbReturn.AppendLine("       <span class=\"fieldName\">Maximum Users</span>");
                sTempLic = saLicKeys[k].Substring(saLicKeys[k].IndexOf("(users-") + 7);
                sTempLic = sTempLic.Substring(0, sTempLic.IndexOf(')'));
                sbReturn.AppendLine("           <span class=\"fieldValue\">" + sTempLic + "</span>");
                sbReturn.AppendLine("       </div>");
            }
        }

        sbReturn.AppendLine("   </div>");
        //----- Add tab to list, will be loaded later in LoadTabUI
        saReturn.Add("License");
        //----- End building Security information.

        //----- Begin building Settings information.

        sbReturn.AppendLine("   <div id=\"Settings\" style=\"display:none;\">");
        sbReturn.AppendLine("       <h3>System Settings</h3>");
        if (systemSettings.webConfigSettings != null)
        {
            for (int i = 0; i < systemSettings.webConfigSettings.Count; i++)
            {
                sbReturn.AppendLine("       <div class=\"field\">");
                sbReturn.AppendLine("       <span class=\"fieldName\">" + systemSettings.webConfigSettings.GetKey(i) + "</span>");
                sbReturn.AppendLine("           <span class=\"fieldValue\">" + systemSettings.webConfigSettings.Get(i) + "</span>");
                sbReturn.AppendLine("       </div>");

                switch (systemSettings.webConfigSettings.GetKey(i))
                {
                    case "Active Directory Enabled":
                        {
                            try
                            {
                                m_bADEnabled = Convert.ToBoolean(systemSettings.webConfigSettings.Get(i));
                            }
                            catch
                            {
                                // m_bADEnabled = false;
                            }
                            break;
                        }
                    case "Active Directory Advanced Config":
                        {
                            try
                            {
                                m_bADAdvancedConfig = Convert.ToBoolean(systemSettings.webConfigSettings.Get(i).ToLower());
                            }
                            catch
                            {
                                // m_bADAdvancedConfig = false;
                            }
                            break;
                        }
                    case "Active Directory Authentication Protocol":
                        {
                            this.m_sADAuthProtocol = (systemSettings.webConfigSettings.Get(i));
                            break;
                        }
                    case "Active Directory Username":
                        {
                            this.m_sADUser = (systemSettings.webConfigSettings.Get(i));
                            break;
                        }
                    case "Active Directory Password":
                        {
                            this.m_sADPass = (systemSettings.webConfigSettings.Get(i));
                            break;
                        }
                    case "SMTP Server":
                        {
                            if (systemSettings.webConfigSettings.Get(i) != "")
                            {
                                m_bSmtpDefined = true;
                                m_sSmtpServer = systemSettings.webConfigSettings.Get(i);
                            }
                            break;
                        }
                    case "SMTP Port":
                        {
                            m_sSmtpPort = systemSettings.webConfigSettings.Get(i);
                            break;
                        }
                    case "SMTP User":
                        {
                            m_sSmtpUser = systemSettings.webConfigSettings.Get(i);
                            break;
                        }
                    case "SMTP Pass":
                        {
                            m_sSmtpPass = systemSettings.webConfigSettings.Get(i);
                            break;
                        }
                }
            }
        }
        sbReturn.AppendLine("   </div>");
        //----- Add tab to list, will be loaded later in LoadTabUI
        saReturn.Add("Settings");
        //----- End building Settings information.

        //----- Begin building Database information.

        sbReturn.AppendLine("   <div id=\"Database\" style=\"display:none;\">");
        sbReturn.AppendLine("       <h3>" + dbInfo.databaseType + "</h3>");
        sbReturn.AppendLine("       <div class=\"field\">");
        sbReturn.AppendLine("           <span class=\"fieldName\">Connection</span>");
        if (dbInfo.lastError != "")
            sbReturn.AppendLine("           <span class=\"fieldValue\"><span class=\"statusError\">Failed </span>" + dbInfo.lastError + "</span>");
        else
            sbReturn.AppendLine("           <span class=\"fieldValue\"><span class=\"statusSuccess\">OK </span></span>");
        sbReturn.AppendLine("       </div>");
        sbReturn.AppendLine("       <div class=\"field\">");
        sbReturn.AppendLine("           <span class=\"fieldName\">Test Query</span>");
        if (!dbInfo.TestQueryDb(ref nRowsReturned))
            sbReturn.AppendLine("           <span class=\"fieldValue\"><span class=\"statusError\">Failed </span>" + dbInfo.lastError + "</span>");
        else
            sbReturn.AppendLine("           <span class=\"fieldValue\"><span class=\"statusSuccess\">OK </span>" + nRowsReturned + " row(s) returned from settings table.</span>");
        sbReturn.AppendLine("       </div>");
        sbReturn.AppendLine("       <div class=\"field\">");
        sbReturn.AppendLine("           <span class=\"fieldName\">Version</span>");
        sbReturn.AppendLine("           <span class=\"fieldValue\">" + dbInfo.GetDbVersion() + "</span>");
        sbReturn.AppendLine("       </div>");
        sbReturn.AppendLine("       <div class=\"field\">");
        sbReturn.AppendLine("       <span class=\"fieldName\">Server</span>");
        sbReturn.AppendLine("           <span class=\"fieldValue\">" + dbInfo.connectionServer + "</span>");
        sbReturn.AppendLine("       </div>");
        if (dbInfo.IsDbSql())
        {
            sbReturn.AppendLine("       <div class=\"field\">");
            sbReturn.AppendLine("       <span class=\"fieldName\">Database Name</span>");
            sbReturn.AppendLine("           <span class=\"fieldValue\">" + dbInfo.connectionDatabase + "</span>");
            sbReturn.AppendLine("       </div>");
            sbReturn.AppendLine("       <div class=\"field\">");
            sbReturn.AppendLine("       <span class=\"fieldName\">Integrated Security</span>");
            sbReturn.AppendLine("           <span class=\"fieldValue\">" + dbInfo.connectionSecurity + "</span>");
            sbReturn.AppendLine("       </div>");
        }

        if (dbInfo.connectionSecurity == "Off" || dbInfo.IsDbOracle())
        {
            sbReturn.AppendLine("       <div class=\"field\">");
            sbReturn.AppendLine("       <span class=\"fieldName\">User ID</span>");
            sbReturn.AppendLine("           <span class=\"fieldValue\">" + dbInfo.connectionUser + "</span>");
            sbReturn.AppendLine("       </div>");
            sbReturn.AppendLine("       <div class=\"field\">");
            sbReturn.AppendLine("       <span class=\"fieldName\">Password</span>");
            sbReturn.AppendLine("           <span class=\"fieldValue\">" + dbInfo.connectionPassword + "</span>");
            sbReturn.AppendLine("       </div>");
        }

        sbReturn.AppendLine("   </div>");
        //----- Add tab to list, will be loaded later in LoadTabUI
        saReturn.Add("Database");
        //----- End building Database information.

        //----- Begin building Connection information.

        sbReturn.AppendLine("   <div id=\"Connections\" style=\"display:none;\">");
        sbReturn.AppendLine("       <h3>Ektron Connection Status</h3>");
        sbReturn.AppendLine("       <div class=\"field\">");
        sbReturn.AppendLine("       <span class=\"fieldName\">Server Controls</span>");
        if (!connStatus.TestServerControl())
            sbReturn.AppendLine("           <span class=\"fieldValue\"><span class=\"statusError\">Failed </span>" + connStatus.LastError + "</span>");
        else
            sbReturn.AppendLine("           <span class=\"fieldValue\"><span class=\"statusSuccess\">OK </span>Server Control Created.</span>");
        sbReturn.AppendLine("       </div>");
        sbReturn.AppendLine("       <div class=\"field\">");
        sbReturn.AppendLine("       <span class=\"fieldName\">Server Controls Web Service</span>");
        if (!connStatus.TestServerControlWS(this.Request.Url.OriginalString))
            sbReturn.AppendLine("           <span class=\"fieldValue\"><span class=\"statusError\">Failed </span>" + connStatus.LastError + "</span>");
        else
            sbReturn.AppendLine("           <span class=\"fieldValue\"><span class=\"statusSuccess\">OK </span>Connected Successfully</span>");
        sbReturn.AppendLine("       </div>");
        sbReturn.AppendLine("       <div class=\"field\">");
        sbReturn.AppendLine("       <span class=\"fieldName\">Content Web Service</span>");
        if (!connStatus.TestContentWS(this.Request.Url.OriginalString))
            sbReturn.AppendLine("           <span class=\"fieldValue\"><span class=\"statusError\">Failed </span>" + connStatus.LastError + "</span>");
        else
            sbReturn.AppendLine("           <span class=\"fieldValue\"><span class=\"statusSuccess\">OK </span>Connected Successfully</span>");
        sbReturn.AppendLine("       </div>");
        sbReturn.AppendLine("       <div class=\"field\">");
        sbReturn.AppendLine("       <span class=\"fieldName\">Content Service</span>");
        if (!connStatus.TestContentService(this.Request.Url.OriginalString))
            sbReturn.AppendLine("           <span class=\"fieldValue\"><span class=\"statusError\">Failed </span>" + connStatus.LastError + "</span>");
        else
            sbReturn.AppendLine("           <span class=\"fieldValue\"><span class=\"statusSuccess\">OK </span>Connected Successfully</span>");
        sbReturn.AppendLine("       </div>");
        sbReturn.AppendLine("       <div class=\"field\">");
        sbReturn.AppendLine("       <span class=\"fieldName\">Indexing Service</span>");
        if ((sIndexerState = connStatus.GetIndexerState()) != "Started")
            sbReturn.AppendLine("           <span class=\"fieldValue\"><span class=\"statusError\">Failed </span>Indexing Service " + sIndexerState + "</span>");
        else
            sbReturn.AppendLine("           <span class=\"fieldValue\"><span class=\"statusSuccess\">OK </span>Indexing Service Started</span>");
        sbReturn.AppendLine("       </div>");
        sbReturn.AppendLine("       <div class=\"field\">");
        sbReturn.AppendLine("       <span class=\"fieldName\">Extensibility Server</span>");
        System.Version ver = new Version(versionInfo.GetCMS400Version());
        System.Version ver2 = new Version("8.0.0.128");
        
        if ((ver < ver2) && (sIndexerState = connStatus.GetExtensionServerState()) != "Started")
            sbReturn.AppendLine("           <span class=\"fieldValue\"><span class=\"statusError\">Failed </span>Extensibility Server " + sIndexerState + "</span>");
        else
            sbReturn.AppendLine("           <span class=\"fieldValue\"><span class=\"statusSuccess\">OK </span>Extensibility Server Started</span>");
        sbReturn.AppendLine("       </div>");
        sbReturn.AppendLine("       <div class=\"field\">");
        sbReturn.AppendLine("       <span class=\"fieldName\">Windows Services 2.0</span>");
        try
        {
            if ((sIndexerState = connStatus.GetEktronWindowsServiceState()) != "Started")
                sbReturn.AppendLine("           <span class=\"fieldValue\"><span class=\"statusError\">Failed </span>Windows Services " + sIndexerState + "</span>");
            else
                sbReturn.AppendLine("           <span class=\"fieldValue\"><span class=\"statusSuccess\">OK </span>Windows Services Started</span>");
        }
        catch(Exception ex)
        {
            sbReturn.AppendLine("           <span class=\"fieldValue\"><span class=\"statusError\">Failed </span>Windows Services 2.0 " + ex.Message.ToString() + "</span>");
        }
        sbReturn.AppendLine("       </div>");
        sbReturn.AppendLine("       <div class=\"field\">");
        sbReturn.AppendLine("       <span class=\"fieldName\">Windows Services 3.0</span>");
        try
        {
            if ((sIndexerState = connStatus.GetEktronWindowsService30State()) != "Started")
                sbReturn.AppendLine("           <span class=\"fieldValue\"><span class=\"statusError\">Failed </span>Windows Services " + sIndexerState + "</span>");
            else
                sbReturn.AppendLine("           <span class=\"fieldValue\"><span class=\"statusSuccess\">OK </span>Windows Services Started</span>");
        }
        catch (Exception ex)
        {
            sbReturn.AppendLine("           <span class=\"fieldValue\"><span class=\"statusError\">Failed </span>Windows Services 3.0 " + ex.Message.ToString() + "</span>");
        }
        sbReturn.AppendLine("       </div>");
        sbReturn.AppendLine("       <div class=\"field\">");
        sbReturn.AppendLine("       <span class=\"fieldName\">Windows Services 4.0</span>");
        try
        {
            if ((sIndexerState = connStatus.GetEktronWindowsService40State()) != "Started")
                sbReturn.AppendLine("           <span class=\"fieldValue\"><span class=\"statusError\">Failed </span>Windows Services " + sIndexerState + "</span>");
            else
                sbReturn.AppendLine("           <span class=\"fieldValue\"><span class=\"statusSuccess\">OK </span>Windows Services Started</span>");
        }
        catch (Exception ex)
        {
            sbReturn.AppendLine("           <span class=\"fieldValue\"><span class=\"statusError\">Failed </span>Windows Services 4.0 " + ex.Message.ToString() + "</span>");
        }
        sbReturn.AppendLine("   </div>");
        //----- Add tab to list, will be loaded later in LoadTabUI
        saReturn.Add("Connections");
        //----- End building Connection information.

        //----- Begin building Security information.
        sbReturn.AppendLine("   <div id=\"Security\" style=\"display:none;\">");
        sbReturn.AppendLine("       <h3>Security Information</h3>");

        for (int j = 0; j < saUsers.Length; j++)
        {
            sbReturn.AppendLine("       <div class=\"field\">");
            sbReturn.AppendLine("       <span class=\"fieldName\">" + saUsers[j] + "</span>");

            if (dbInfo.GetPassword(saUsers[j], ref sPassword))
            {
                if (saPasswrds[j] == sPassword)
                    sbReturn.AppendLine("           <span class=\"fieldValue\">Exists.  Default password.</span>");
                else
                    sbReturn.AppendLine("           <span class=\"fieldValue\">Exists.  Password changed.</span>");
            }
            else if (sPassword != "")
            {
                sbReturn.AppendLine("           <span class=\"fieldValue\"><span class=\"statusError\">Failed</span> Could not verify. ERROR: " + dbInfo.lastError + "</span>");
            }
            else
            {
                sbReturn.AppendLine("           <span class=\"fieldValue\">Does Not Exist.</span>");
            }

            sbReturn.AppendLine("       </div>");
        }

        sbReturn.AppendLine("   </div>");
        //----- Add tab to list, will be loaded later in LoadTabUI
        saReturn.Add("Security");
        //----- End building Security information.

        //----- Begin building Permissions information.
        sbReturn.AppendLine("   <div id=\"Permissions\" style=\"display:none;\">");
        sbReturn.AppendLine("       <h3>DMS Effective Permissions</h3>");
        sbReturn.AppendLine("       <table class=\"eventLogNames\">");
        sbReturn.AppendLine("           <tr>");
        sbReturn.AppendLine("               <td id=\"DMSCatLocationHeader\" style=\"color:red;\" onclick=\"permissionHeaderClick('" + DMS_CAT_LOC + "')\" onmouseover=\"permissionMouseOver('DMSCatLocationHeader')\" onmouseout=\"permissionMouseOut('DMSCatLocationHeader')\">" + systemSettings.webConfigSettings[EkSettings.DMS_CAT_LOC] + "</td>");
        sbReturn.AppendLine("               <td style=\"width: 5px;\"></td>");
        sbReturn.AppendLine("               <td id=\"DMSStorageLocationHeader\" onclick=\"permissionHeaderClick('" + DMS_STOR_LOC + "')\" onmouseover=\"permissionMouseOver('DMSStorageLocationHeader')\" onmouseout=\"permissionMouseOut('DMSStorageLocationHeader')\">" + systemSettings.webConfigSettings[EkSettings.DMS_CAT_STOR] + "</td>");
        sbReturn.AppendLine("               <td style=\"width: 5px;\"></td>");
        sbReturn.AppendLine("               <td id=\"DMSdmdataHeader\" onclick=\"permissionHeaderClick('" + DMS_DMDATA_LOC + "')\" onmouseover=\"permissionMouseOver('DMSdmdataHeader')\" onmouseout=\"permissionMouseOut('DMSdmdataHeader')\">SiteDmdata</td>");
        sbReturn.AppendLine("               <td style=\"width: 5px;\"></td>");
        sbReturn.AppendLine("               <td id=\"DMSassetsHeader\" onclick=\"permissionHeaderClick('" + DMS_ASSETS_LOC + "')\" onmouseover=\"permissionMouseOver('DMSassetsHeader')\" onmouseout=\"permissionMouseOut('DMSassetsHeader')\">SiteAssests</td>");
        sbReturn.AppendLine("           </tr>");
        sbReturn.AppendLine("       </table>");

        sbReturn.AppendLine("       <div id=\"" + DMS_CAT_LOC + "\" style=\"display:inline;\">");
        sbReturn.AppendLine("           <table style=\"border-top-style: inset; border-right-style: inset; border-left-style: inset; border-bottom-style: inset;\">");
        sbReturn.AppendLine("               <tr>");
        sbReturn.AppendLine("                   <td></td>");

        ArrayList alTempESO = systemSettings.GetPermissions(systemSettings.webConfigSettings[EkSettings.DMS_CAT_LOC]);
        ArrayList alPermMissing = null;
        ArrayList alUserMissingPerm = null;

        for (int nUsers = 0; nUsers < alTempESO.Count; nUsers++)
        {
                sbReturn.AppendLine("                   <td style=\"border-right: thin solid; border-top: thin solid; border-left: thin solid; border-bottom: thin solid; vertical-align: middle; text-align: center\">" + 
            
                                    ((EkSecurityObject)alTempESO[nUsers]).NTAccountName + "</td>");
        }
        sbReturn.AppendLine("               </tr>");
        
        for (int nRights = 0; nRights < alTempESO.Count; nRights++) //Defect#55942 
        {
            sbReturn.AppendLine("            <tr>");
            sbReturn.AppendLine("                   <td style=\"border-right: thin solid; border-top: thin solid; border-left: thin solid; border-bottom: thin solid;\">" + 
                               ((EkSecurityObject)alTempESO[0]).FileSystemRights.GetKey(nRights) + "</td>");
            for (int nUsers3 = 0; nUsers3 < alTempESO.Count; nUsers3++)
            {
                if (((EkSecurityObject)alTempESO[nUsers3]).FileSystemRights[((EkSecurityObject)alTempESO[0]).FileSystemRights.GetKey(nRights)] == "true")
                    sbReturn.AppendLine("                   <td style=\"border-right: thin solid; border-top: thin solid; border-left: thin solid; border-bottom: thin solid; vertical-align: middle; text-align: center\"><input type=\"checkbox\" disabled=\"disabled\" checked=\"checked\" /></tr>");
                else
                    sbReturn.AppendLine("                   <td style=\"border-right: thin solid; border-top: thin solid; border-left: thin solid; border-bottom: thin solid; vertical-align: middle; text-align: center\"><input type=\"checkbox\" disabled=\"disabled\" /></tr>");
            }
        }
        sbReturn.AppendLine("            </tr>");
        sbReturn.AppendLine("           </table>");
        sbReturn.AppendLine("       </div>");

        sbReturn.AppendLine("       <div id=\"" + DMS_STOR_LOC + "\" style=\"display:none;\">");
        sbReturn.AppendLine("           <table style=\"border-top-style: inset; border-right-style: inset; border-left-style: inset; border-bottom-style: inset;\">");
        sbReturn.AppendLine("               <tr>");
        sbReturn.AppendLine("                   <td></td>");

        alPermMissing = null;
        alUserMissingPerm = null;
        alTempESO = null;
        alTempESO = systemSettings.GetPermissions(systemSettings.webConfigSettings[EkSettings.DMS_CAT_STOR]);
        alUserMissingPerm = new ArrayList();
        
        for (int nUsers = 0; nUsers < alTempESO.Count; nUsers++)
        {
            if (((EkSecurityObject)alTempESO[nUsers]).NTAccountName.ToLower() == systemSettings.ASPNETUserName.ToLower() && !HasPermission(((EkSecurityObject)alTempESO[nUsers]).FileSystemRights, "Full Control;Read Data|Read Attributes|Read Extended Attributes|Create Folders|Write Attributes|Write Extended Attributes|Delete|Read", ref alPermMissing))
            {
                sbReturn.AppendLine("                   <td title=\"Asp net account needs special permissions, Read Data|Read Attributes|Read Extended Attributes|Create Folders|Write Attributes|Write Extended Attributes|Delete|Read\" style=\"border-right: thin solid; border-top: thin solid; border-left: thin solid; border-bottom: thin solid; vertical-align: middle; text-align: center; background-color: red;\">" +
                                    ((EkSecurityObject)alTempESO[nUsers]).NTAccountName + "</td>");
                alUserMissingPerm.Add(nUsers);
            }
            else if (((EkSecurityObject)alTempESO[nUsers]).NTAccountName.ToLower() == "iis_wpg".ToLower() && !HasPermission(((EkSecurityObject)alTempESO[nUsers]).FileSystemRights, "Full Control;Read Data|Read Attributes|Read Extended Attributes|Create Folders|Write Attributes|Write Extended Attributes|Delete|Read", ref alPermMissing))
            {
                sbReturn.AppendLine("                   <td title=\"Win 2003 IIS group needs special permissions, Read Data|Read Attributes|Read Extended Attributes|Create Folders|Write Attributes|Write Extended Attributes|Delete|Read\" style=\"border-right: thin solid; border-top: thin solid; border-left: thin solid; border-bottom: thin solid; vertical-align: middle; text-align: center; background-color: red;\">" +
                                    ((EkSecurityObject)alTempESO[nUsers]).NTAccountName + "</td>");
                alUserMissingPerm.Add(nUsers);
            }
            else
                sbReturn.AppendLine("                   <td style=\"border-right: thin solid; border-top: thin solid; border-left: thin solid; border-bottom: thin solid; vertical-align: middle; text-align: center\">" +
                                    ((EkSecurityObject)alTempESO[nUsers]).NTAccountName + "</td>");
        }
        sbReturn.AppendLine("               </tr>");

        for (int nRights = 0; nRights < ((EkSecurityObject)alTempESO[0]).FileSystemRights.Count; nRights++)
        {
            sbReturn.AppendLine("            <tr>");
            sbReturn.AppendLine("                   <td style=\"border-right: thin solid; border-top: thin solid; border-left: thin solid; border-bottom: thin solid;\">" +
                               ((EkSecurityObject)alTempESO[0]).FileSystemRights.GetKey(nRights) + "</td>");
            for (int nUsers3 = 0; nUsers3 < alTempESO.Count; nUsers3++)
            {
                if (((EkSecurityObject)alTempESO[nUsers3]).FileSystemRights[((EkSecurityObject)alTempESO[0]).FileSystemRights.GetKey(nRights)] == "true")
                    sbReturn.AppendLine("                   <td style=\"border-right: thin solid; border-top: thin solid; border-left: thin solid; border-bottom: thin solid; vertical-align: middle; text-align: center\"><input type=\"checkbox\" disabled=\"disabled\" checked=\"checked\" /></tr>");
                else if(alUserMissingPerm.Contains(nUsers3) && alPermMissing.Contains(((EkSecurityObject)alTempESO[0]).FileSystemRights.GetKey(nRights)))
                    sbReturn.AppendLine("                   <td style=\"border-right: thin solid; border-top: thin solid; border-left: thin solid; border-bottom: thin solid; vertical-align: middle; text-align: center; background-color: red;\"><input type=\"checkbox\" disabled=\"disabled\" /></tr>");
                else
                    sbReturn.AppendLine("                   <td style=\"border-right: thin solid; border-top: thin solid; border-left: thin solid; border-bottom: thin solid; vertical-align: middle; text-align: center\"><input type=\"checkbox\" disabled=\"disabled\" /></tr>");
            }
        }
        sbReturn.AppendLine("            </tr>");
        sbReturn.AppendLine("           </table>");
        sbReturn.AppendLine("       </div>");

        sbReturn.AppendLine("       <div id=\"" + DMS_DMDATA_LOC + "\" style=\"display:none;\">");
        sbReturn.AppendLine("           <table style=\"border-top-style: inset; border-right-style: inset; border-left-style: inset; border-bottom-style: inset;\">");
        sbReturn.AppendLine("               <tr>");
        sbReturn.AppendLine("                   <td></td>");

        alUserMissingPerm.Clear();
        alPermMissing = null;
        alTempESO = null;
        alTempESO = systemSettings.GetPermissions(HttpContext.Current.Server.MapPath("~/AssetManagement/dmdata"));

        for (int nUsers = 0; nUsers < alTempESO.Count; nUsers++)
        {
            if (((EkSecurityObject)alTempESO[nUsers]).NTAccountName.ToLower() == systemSettings.ASPNETUserName.ToLower() && !HasPermission(((EkSecurityObject)alTempESO[nUsers]).FileSystemRights, "Full Control;Write|Read", ref alPermMissing))
            {
                sbReturn.AppendLine("                   <td title=\"Asp net user must have read & write permissions\" style=\"border-right: thin solid; border-top: thin solid; border-left: thin solid; border-bottom: thin solid; vertical-align: middle; text-align: center; background-color: red;\">" +
                                    ((EkSecurityObject)alTempESO[nUsers]).NTAccountName + "</td>");
                alUserMissingPerm.Add(nUsers);
            }
            else if (((EkSecurityObject)alTempESO[nUsers]).NTAccountName.ToLower() == "iis_wpg".ToLower() && !HasPermission(((EkSecurityObject)alTempESO[nUsers]).FileSystemRights, "Full Control;Write|Read", ref alPermMissing))
            {
                sbReturn.AppendLine("                   <td title=\"Win 2003 IIS group must have read & write permissions\" style=\"border-right: thin solid; border-top: thin solid; border-left: thin solid; border-bottom: thin solid; vertical-align: middle; text-align: center; background-color: red;\">" +
                                    ((EkSecurityObject)alTempESO[nUsers]).NTAccountName + "</td>");
                alUserMissingPerm.Add(nUsers);
            }
            else if (((EkSecurityObject)alTempESO[nUsers]).NTAccountName.ToLower() == systemSettings.DMSUserName.ToLower() && !HasPermission(((EkSecurityObject)alTempESO[nUsers]).FileSystemRights, "Full Control;Write", ref alPermMissing))
            {
                sbReturn.AppendLine("                   <td title=\"DMS User must have write permissions\" style=\"border-right: thin solid; border-top: thin solid; border-left: thin solid; border-bottom: thin solid; vertical-align: middle; text-align: center; background-color: red;\">" +
                                    ((EkSecurityObject)alTempESO[nUsers]).NTAccountName + "</td>");
                alUserMissingPerm.Add(nUsers);
            }
            else
                sbReturn.AppendLine("                   <td style=\"border-right: thin solid; border-top: thin solid; border-left: thin solid; border-bottom: thin solid; vertical-align: middle; text-align: center\">" +
                                    ((EkSecurityObject)alTempESO[nUsers]).NTAccountName + "</td>");
        }
        sbReturn.AppendLine("               </tr>");

        for (int nRights = 0; nRights < ((EkSecurityObject)alTempESO[0]).FileSystemRights.Count; nRights++)
        {
            sbReturn.AppendLine("            <tr>");
            sbReturn.AppendLine("                   <td style=\"border-right: thin solid; border-top: thin solid; border-left: thin solid; border-bottom: thin solid;\">" +
                               ((EkSecurityObject)alTempESO[0]).FileSystemRights.GetKey(nRights) + "</td>");
            for (int nUsers3 = 0; nUsers3 < alTempESO.Count; nUsers3++)
            {
                if (((EkSecurityObject)alTempESO[nUsers3]).FileSystemRights[((EkSecurityObject)alTempESO[0]).FileSystemRights.GetKey(nRights)] == "true")
                    sbReturn.AppendLine("                   <td style=\"border-right: thin solid; border-top: thin solid; border-left: thin solid; border-bottom: thin solid; vertical-align: middle; text-align: center\"><input type=\"checkbox\" disabled=\"disabled\" checked=\"checked\" /></tr>");
                else if (alUserMissingPerm.Contains(nUsers3) && alPermMissing.Contains(((EkSecurityObject)alTempESO[0]).FileSystemRights.GetKey(nRights)))
                    sbReturn.AppendLine("                   <td style=\"border-right: thin solid; border-top: thin solid; border-left: thin solid; border-bottom: thin solid; vertical-align: middle; text-align: center; background-color: red;\"><input type=\"checkbox\" disabled=\"disabled\" /></tr>");
                 else
                    sbReturn.AppendLine("                   <td style=\"border-right: thin solid; border-top: thin solid; border-left: thin solid; border-bottom: thin solid; vertical-align: middle; text-align: center\"><input type=\"checkbox\" disabled=\"disabled\" /></tr>");
            }
        }
        sbReturn.AppendLine("            </tr>");
        sbReturn.AppendLine("           </table>");
        sbReturn.AppendLine("       </div>");

        sbReturn.AppendLine("       <div id=\"" + DMS_ASSETS_LOC + "\" style=\"display:none;\">");
        sbReturn.AppendLine("           <table style=\"border-top-style: inset; border-right-style: inset; border-left-style: inset; border-bottom-style: inset;\">");
        sbReturn.AppendLine("               <tr>");
        sbReturn.AppendLine("                   <td></td>");

        alUserMissingPerm.Clear();
        alPermMissing = null;
        alTempESO = null;
        alTempESO = systemSettings.GetPermissions(HttpContext.Current.Server.MapPath("~/assets"));

        for (int nUsers = 0; nUsers < alTempESO.Count; nUsers++)
        {
            if (((EkSecurityObject)alTempESO[nUsers]).NTAccountName.ToLower() == systemSettings.ASPNETUserName.ToLower() && !HasPermission(((EkSecurityObject)alTempESO[nUsers]).FileSystemRights, "Full Control;Write|Read", ref alPermMissing))
            {
                sbReturn.AppendLine("                   <td title=\"Asp net account needs read & write permissions\" style=\"border-right: thin solid; border-top: thin solid; border-left: thin solid; border-bottom: thin solid; vertical-align: middle; text-align: center; background-color: red;\">" +
                                    ((EkSecurityObject)alTempESO[nUsers]).NTAccountName + "</td>");
                alUserMissingPerm.Add(nUsers);
            }
            else if (((EkSecurityObject)alTempESO[nUsers]).NTAccountName.ToLower() == "iis_wpg".ToLower() && !HasPermission(((EkSecurityObject)alTempESO[nUsers]).FileSystemRights, "Full Control;Write|Read", ref alPermMissing))
            {
                sbReturn.AppendLine("                   <td title=\"Win 2003 IIS group needs read & write permissions\" style=\"border-right: thin solid; border-top: thin solid; border-left: thin solid; border-bottom: thin solid; vertical-align: middle; text-align: center; background-color: red;\">" +
                                    ((EkSecurityObject)alTempESO[nUsers]).NTAccountName + "</td>");
                alUserMissingPerm.Add(nUsers);
            }
            else
                sbReturn.AppendLine("                   <td style=\"border-right: thin solid; border-top: thin solid; border-left: thin solid; border-bottom: thin solid; vertical-align: middle; text-align: center\">" +
                                    ((EkSecurityObject)alTempESO[nUsers]).NTAccountName + "</td>");
        }
        sbReturn.AppendLine("               </tr>");

        for (int nRights = 0; nRights < ((EkSecurityObject)alTempESO[0]).FileSystemRights.Count; nRights++)
        {
            sbReturn.AppendLine("            <tr>");
            sbReturn.AppendLine("                   <td style=\"border-right: thin solid; border-top: thin solid; border-left: thin solid; border-bottom: thin solid;\">" +
                               ((EkSecurityObject)alTempESO[0]).FileSystemRights.GetKey(nRights) + "</td>");
            for (int nUsers3 = 0; nUsers3 < alTempESO.Count; nUsers3++)
            {
                if (((EkSecurityObject)alTempESO[nUsers3]).FileSystemRights[((EkSecurityObject)alTempESO[0]).FileSystemRights.GetKey(nRights)] == "true")
                    sbReturn.AppendLine("                   <td style=\"border-right: thin solid; border-top: thin solid; border-left: thin solid; border-bottom: thin solid; vertical-align: middle; text-align: center\"><input type=\"checkbox\" disabled=\"disabled\" checked=\"checked\" /></tr>");
                else if (alUserMissingPerm.Contains(nUsers3) && alPermMissing.Contains(((EkSecurityObject)alTempESO[0]).FileSystemRights.GetKey(nRights)))
                    sbReturn.AppendLine("                   <td style=\"border-right: thin solid; border-top: thin solid; border-left: thin solid; border-bottom: thin solid; vertical-align: middle; text-align: center; background-color: red;\"><input type=\"checkbox\" disabled=\"disabled\" /></tr>");
                 else
                    sbReturn.AppendLine("                   <td style=\"border-right: thin solid; border-top: thin solid; border-left: thin solid; border-bottom: thin solid; vertical-align: middle; text-align: center\"><input type=\"checkbox\" disabled=\"disabled\" /></tr>");
            }
        }
        sbReturn.AppendLine("            </tr>");
        sbReturn.AppendLine("           </table>");
        sbReturn.AppendLine("       </div>");

        sbReturn.AppendLine("   </div>");
        //----- Add tab to list, will be loaded later in LoadTabUI
        saReturn.Add("Permissions");
        //----- End building Permissions information.

        //----- Begin building Event information.
        sbReturn.AppendLine("   <div id=\"Events\" style=\"display:none;\">");
        sbReturn.AppendLine("       <table class=\"eventLogNames\">");
        sbReturn.AppendLine("           <tr>");
        sbReturn.AppendLine("               <td id=\"CMS400Events\" style=\"color:red;\" onclick=\"logHeaderClick('" + EVENT_CMS + "')\" onmouseover=\"logMouseOver('CMS400Events')\" onmouseout=\"logMouseOut('CMS400Events')\">CMS400 Events</td>");
        sbReturn.AppendLine("               <td style=\"width: 5px;\"></td>");
        sbReturn.AppendLine("               <td id=\"applicationHeader\" onclick=\"logHeaderClick('" + EVENT_APPLICATION + "')\" onmouseover=\"logMouseOver('applicationHeader')\" onmouseout=\"logMouseOut('applicationHeader')\">Application log</td>");
        sbReturn.AppendLine("           </tr>");
        sbReturn.AppendLine("       </table>");
        sbReturn.AppendLine(LoadLogContents(EVENT_CMS, nEventEntries));
        sbReturn.AppendLine(LoadLogContents(EVENT_APPLICATION, nEventEntries));
        sbReturn.AppendLine("   </div>");
        //----- Add tab to list, will be loaded later in LoadTabUI
        saReturn.Add("Events");
        //----- End building Event information.

        //----- Begin building Contact information.
        sbReturn.AppendLine("   <div id=\"Contact\" class=\"mailTo\" style=\"display:none;\">");
        sbReturn.AppendLine("       <br/>");

        if (!m_bSmtpDefined)
        {
            sbReturn.AppendLine("       <a href=\"mailto:support@ektron.com?subject=Sent%20from%20diagnostic%20utility\">Email</a>&nbsp;Ektron Support");
        }
        else
        {
            sbReturn.AppendLine("          Enter comments: ");
            sbReturn.AppendLine("          <br/>");
            sbReturn.AppendLine("          <textarea rows=\"10\" cols=\"50\" name=\"emailComments\"></textarea>");
            sbReturn.AppendLine("          <br/>");
            sbReturn.AppendLine("          Send to (must be an email address): ");
            sbReturn.AppendLine("          <br/>");
            sbReturn.AppendLine("          <input type=\"text\" name=\"emailAddressTo\"/>");
            sbReturn.AppendLine("          <br/>");
            sbReturn.AppendLine("          Send from (must be an email address): ");
            sbReturn.AppendLine("          <br/>");
            sbReturn.AppendLine("          <input type=\"text\" name=\"emailAddressFrom\"/>");
            sbReturn.AppendLine("          <br/>");
            sbReturn.AppendLine("          <input type=\"submit\" value=\"Send\"/>");

            if (Page.Session["smtpServer"] != null)
                Page.Session.Remove("smtpServer");
            if (Page.Session["smtpPort"] != null)
                Page.Session.Remove("smtpPort");
            if (Page.Session["smtpUser"] != null)
                Page.Session.Remove("smtpUser");
            if (Page.Session["smtpPass"] != null)
                Page.Session.Remove("smtpPass");

            Page.Session.Add("smtpServer", m_sSmtpServer);
            Page.Session.Add("smtpPort", m_sSmtpPort);
            Page.Session.Add("smtpUser", m_sSmtpUser);
            Page.Session.Add("smtpPass", m_sSmtpPass);
        }

        sbReturn.AppendLine("   </div>");
        //----- Add tab to list, will be loaded later in LoadTabUI
        saReturn.Add("Contact");
        //----- End building Contact information.

        //----- Begin building Active Directory information.
        sbReturn.AppendLine("   <div id=\"Authentication\" style=\"display:none;\">");

        m_iAdAuth = dbInfo.GetADAuth();
        this.m_bAdInt = dbInfo.GetADInt();


        if (m_iAdAuth == 0)
        {
            sbReturn.AppendLine("       <h3>Active Directory</h3>");

            sbReturn.AppendLine("       <div class=\"field\">");
            sbReturn.AppendLine("       <span class=\"fieldName\">Enabled</span>");
            sbReturn.AppendLine("           <span class=\"fieldValue\">" + m_bADEnabled.ToString() + "</span>");
            sbReturn.AppendLine("       </div>");
            sbReturn.AppendLine("       <div class=\"field\">");
            sbReturn.AppendLine("       <span class=\"fieldName\">Integration</span>");
            sbReturn.AppendLine("           <span class=\"fieldValue\">" + m_bAdInt.ToString() + "</span>");
            sbReturn.AppendLine("       </div>");
            sbReturn.AppendLine("       <div class=\"field\">");
            sbReturn.AppendLine("       <span class=\"fieldName\">Advanced Enabled</span>");
            sbReturn.AppendLine("           <span class=\"fieldValue\">" + m_bADAdvancedConfig.ToString() + "</span>");
            sbReturn.AppendLine("       </div>");
            sbReturn.AppendLine("       <div class=\"field\">");
            sbReturn.AppendLine("       <span class=\"fieldName\">Authentication</span>");
            sbReturn.AppendLine("           <span class=\"fieldValue\">False</span>");
            sbReturn.AppendLine("       </div>");

            sbReturn.AppendLine("       <h3>LDAP</h3>");
            sbReturn.AppendLine("       <div class=\"field\">");
            sbReturn.AppendLine("       <span class=\"fieldName\">Enabled</span>");
            sbReturn.AppendLine("           <span class=\"fieldValue\">" + m_bADEnabled.ToString() + "</span>");
            sbReturn.AppendLine("       </div>");
            sbReturn.AppendLine("       <div class=\"field\">");
            sbReturn.AppendLine("       <span class=\"fieldName\">Authentication</span>");
            sbReturn.AppendLine("           <span class=\"fieldValue\">False</span>");
            sbReturn.AppendLine("       </div>");
        }
        else if (m_iAdAuth == 1)
        {
            this.m_bAdAutoUser = dbInfo.GetADAutoUser();
            this.m_bAdAutoUsertoGroup = dbInfo.GetADAutoUsertoGroup();

            sbReturn.AppendLine("       <h3>Active Directory</h3>");
            sbReturn.AppendLine("       <div class=\"field\">");
            sbReturn.AppendLine("       <span class=\"fieldName\">Enabled</span>");
            sbReturn.AppendLine("           <span class=\"fieldValue\">" + m_bADEnabled.ToString() + "</span>");
            sbReturn.AppendLine("       </div>");
            sbReturn.AppendLine("       <div class=\"field\">");
            sbReturn.AppendLine("       <span class=\"fieldName\">Advanced Enabled</span>");
            sbReturn.AppendLine("           <span class=\"fieldValue\">" + m_bADAdvancedConfig.ToString() + "</span>");
            sbReturn.AppendLine("       </div>");
            sbReturn.AppendLine("       <div class=\"field\">");
            sbReturn.AppendLine("           <span class=\"fieldName\">Username</span>");
            sbReturn.AppendLine("           <span class=\"fieldValue\">" + this.m_sADUser + "</span>");
            sbReturn.AppendLine("       </div>");
            sbReturn.AppendLine("       <div class=\"field\">");
            sbReturn.AppendLine("           <span class=\"fieldName\">Password</span>");
            sbReturn.AppendLine("           <span class=\"fieldValue\">" + this.m_sADPass + "</span>");
            sbReturn.AppendLine("       </div>");
            sbReturn.AppendLine("       <div class=\"field\">");
            sbReturn.AppendLine("           <span class=\"fieldName\">Protocol</span>");
            sbReturn.AppendLine("           <span class=\"fieldValue\">" + this.m_sADAuthProtocol + "</span>");
            sbReturn.AppendLine("       </div>");

            sbReturn.AppendLine("       <div class=\"field\">");
            sbReturn.AppendLine("       <span class=\"fieldName\">Authentication</span>");
            sbReturn.AppendLine("           <span class=\"fieldValue\">True</span>");
            sbReturn.AppendLine("       </div>");

            sbReturn.AppendLine("       <div class=\"field\">");
            sbReturn.AppendLine("       <span class=\"fieldName\">Integration</span>");
            sbReturn.AppendLine("           <span class=\"fieldValue\">" + m_bAdInt.ToString() + "</span>");
            sbReturn.AppendLine("       </div>");
            sbReturn.AppendLine("       <div class=\"field\">");
            sbReturn.AppendLine("       <span class=\"fieldName\">Auto Add User</span>");
            sbReturn.AppendLine("           <span class=\"fieldValue\">" + m_bAdAutoUser.ToString() + "</span>");
            sbReturn.AppendLine("       </div>");
            sbReturn.AppendLine("       <div class=\"field\">");
            sbReturn.AppendLine("       <span class=\"fieldName\">Auto Add User to Group</span>");
            sbReturn.AppendLine("           <span class=\"fieldValue\">" + m_bAdAutoUsertoGroup.ToString() + "</span>");
            sbReturn.AppendLine("       </div>");


            if (m_bADAdvancedConfig == true)
            {
                Ektron.Cms.ADDomain[] aDomains;
                aDomains = dbInfo.GetADDomains();
                sbReturn.AppendLine("       <h3>Domains</h3>");
                if (aDomains.Length > 0)
                {
                    for (int i = 0; i < aDomains.Length; i++)
                    {
                        sbReturn.AppendLine("       <div class=\"field\">");
                        sbReturn.AppendLine("       <span class=\"fieldName\">DNS<br/>NETBIOS<br/>User<br/>Pass<br/>Server</span>");
                        sbReturn.AppendLine("           <span class=\"fieldValue\">" + aDomains[i].DomainDNS + "<br/>" + aDomains[i].NetBIOS + "<br/>" + aDomains[i].Username + "<br/>" + aDomains[i].Password + "<br/>" + aDomains[i].ServerIP + "</span>");
                        sbReturn.AppendLine("       </div>");
                    }
                }
                else
                {
                    sbReturn.AppendLine("       <div class=\"field\">");
                    sbReturn.AppendLine("       <span class=\"fieldName\">Domains</span>");
                    sbReturn.AppendLine("           <span class=\"fieldValue\"><span class=\"statusError\">You do not have any domains specified.</span></span>");
                    sbReturn.AppendLine("       </div>");
                }
            }
            else
            {
                this.m_sADDomain = dbInfo.GetADDomain();
                sbReturn.AppendLine("       <div class=\"field\">");
                sbReturn.AppendLine("       <span class=\"fieldName\">Domain</span>");
                sbReturn.AppendLine("           <span class=\"fieldValue\">" + m_sADDomain + "</span>");
                sbReturn.AppendLine("       </div>");
            }
        }
        else if (m_iAdAuth == 2)
        {
            this.m_sADDomain = dbInfo.GetADDomain();
            string strServer;
            string[] arrDomain;
            string[] arrServer;
            string LDAPType = "";
            bool bSSL = false;
            string strPort = "";
            string strLDAPDomain = "";
            int orgincluded = 1;
            string baseDN = "";
            arrDomain = m_sADDomain.Split(new string[] { "</server>" }, StringSplitOptions.None);
            arrServer = arrDomain[0].Split(new string[] { "</p>" }, StringSplitOptions.None);
            if (arrDomain.Length >= 3)
            {
                LDAPType = arrDomain[2];
            }
            switch (LDAPType.ToUpper())
            {
                case "NO":
                    LDAPType = "Novell eDirectory/NDS";
                    break;
                case "SU":
                    LDAPType = "Sun Iplanet/JSDS";
                    break;
                case "AD":
                    LDAPType = "Active Directory (LDAP)";
                    break;
                case "OT":
                    LDAPType = "Other";
                    break;
            }
            if (arrDomain.Length >= 4 && arrDomain[3].ToString().Trim().ToUpper() == "SSL")
            {
                bSSL = true;
            }
            strServer = arrServer[0];
            strPort = arrServer[1];
            if (arrServer.GetUpperBound(0) > 1)
            {
                strLDAPDomain = arrServer[2];
            }
            string[] arrSeps;
            int arrConnectionsCount;
            string strOrg = "";
            string[] arrOrg;
            arrSeps = arrDomain[1].Split(new string[] { "</>" }, StringSplitOptions.None);
            arrConnectionsCount = 0;
            for (int arrCount = 0; arrCount < arrSeps.Length; arrCount++)
            {
                if ((arrSeps[arrCount] != ""))
                {
                    arrOrg = arrSeps[arrCount].Split(new string[] { "=" }, StringSplitOptions.None);
                    if (arrOrg[0] == "o")
                    {
                        strOrg = arrOrg[1];
                    }
                    else
                    {
                        arrConnectionsCount = arrConnectionsCount + 1;
                    }
                }
            }
            if (strLDAPDomain.Trim().ToLower() == "dc=" || strLDAPDomain.Trim().Length == 0)
            {
                baseDN = "o=" + strOrg;
            }
            else
            {
                baseDN = strLDAPDomain;
                orgincluded = 0;
            }

            sbReturn.AppendLine("       <h3>LDAP</h3>");
            sbReturn.AppendLine("       <div class=\"field\">");
            sbReturn.AppendLine("       <span class=\"fieldName\">Enabled</span>");
            sbReturn.AppendLine("           <span class=\"fieldValue\">" + m_bADEnabled.ToString() + "</span>");
            sbReturn.AppendLine("       </div>");
            sbReturn.AppendLine("       <div class=\"field\">");
            sbReturn.AppendLine("           <span class=\"fieldName\">Username</span>");
            sbReturn.AppendLine("           <span class=\"fieldValue\">" + this.m_sADUser + "</span>");
            sbReturn.AppendLine("       </div>");
            sbReturn.AppendLine("       <div class=\"field\">");
            sbReturn.AppendLine("           <span class=\"fieldName\">Password</span>");
            sbReturn.AppendLine("           <span class=\"fieldValue\">" + this.m_sADPass + "</span>");
            sbReturn.AppendLine("       </div>");

            sbReturn.AppendLine("       <div class=\"field\">");
            sbReturn.AppendLine("       <span class=\"fieldName\">Authentication</span>");
            sbReturn.AppendLine("           <span class=\"fieldValue\">True</span>");
            sbReturn.AppendLine("       </div>");

            sbReturn.AppendLine("       <div class=\"field\">");
            sbReturn.AppendLine("       <span class=\"fieldName\">Type</span>");
            sbReturn.AppendLine("           <span class=\"fieldValue\">" + LDAPType + "</span>");
            sbReturn.AppendLine("       </div>");
            sbReturn.AppendLine("       <div class=\"field\">");
            sbReturn.AppendLine("       <span class=\"fieldName\">Server</span>");
            sbReturn.AppendLine("           <span class=\"fieldValue\">" + strServer + "</span>");
            sbReturn.AppendLine("       </div>");
            sbReturn.AppendLine("       <div class=\"field\">");
            sbReturn.AppendLine("       <span class=\"fieldName\">Port</span>");
            sbReturn.AppendLine("           <span class=\"fieldValue\">" + strPort + "</span>");
            sbReturn.AppendLine("       </div>");

            sbReturn.AppendLine("       <div class=\"field\">");
            if (orgincluded == 1)
            {
                sbReturn.AppendLine("       <span class=\"fieldName\">Organization</span>");
            }
            else
            {
                sbReturn.AppendLine("       <span class=\"fieldName\">Domain</span>");
            }
            sbReturn.AppendLine("           <span class=\"fieldValue\">" + baseDN + "</span>");
            sbReturn.AppendLine("       </div>");

            sbReturn.AppendLine("       <div class=\"field\">");
            sbReturn.AppendLine("       <span class=\"fieldName\">SSL</span>");
            sbReturn.AppendLine("           <span class=\"fieldValue\">" + bSSL.ToString() + "</span>");
            sbReturn.AppendLine("       </div>");

            // list of paths
            sbReturn.AppendLine("       <div class=\"field\">");
            sbReturn.AppendLine("       <span class=\"fieldName\">Paths</span><span class=\"fieldValue\">");
            for (int arrCount = 0; arrCount < arrSeps.Length; arrCount++)
            {
                if (orgincluded == 1 && arrCount == (arrSeps.Length - 1))
                {
                    // nothing
                }
                else
                {
                    sbReturn.AppendLine("           " + arrSeps[arrCount] + "<br />");
                }
            }
            sbReturn.AppendLine("       </span></div>");
        }
        sbReturn.AppendLine("   </div>");
        //----- Add tab to list, will be loaded later in LoadTabUI
        saReturn.Add("Authentication");
        //----- End building Active Directory information.

        //----- Cleanup
        if (versionInfo != null)
            versionInfo = null;
        if (dbInfo != null)
            dbInfo = null;
        if (systemSettings != null)
            systemSettings = null;
        if (connStatus != null)
            connStatus = null;

        return sbReturn.ToString();
    }

private bool HasPermission(System.Collections.Specialized.NameValueCollection permissions, string sfindPermission, ref ArrayList sPermIndex)
{
    bool bReturn = false;
    string[] sFind;
    string[] sInclusive;

    sPermIndex = new ArrayList();
    sFind = sfindPermission.Split(';');

    for (int i = 0; i < sFind.Length; i++)
    {
        if (sFind[i].Contains("|"))
        {
            sInclusive = sFind[i].Split('|');

            for (int j = 0; j < sInclusive.Length; j++)
            {
                if (permissions.Get(sInclusive[j]) == null)
                {
                    bReturn = false;
                    break;
                }
                else if (permissions.Get(sInclusive[j]) != "true")
                {
                    bReturn = false;
                    sPermIndex.Add(sInclusive[j]);
                    break;
                }
                else
                    bReturn = true;
            }

            if (bReturn)
                break;
        }
        else
        {
            if (permissions.Get(sFind[i]) != null && (permissions.Get(sFind[i]) == "true"))
            {
                bReturn = true;
                break;
            }
        }
    }

    if (bReturn)
        sPermIndex.Clear();

    return bReturn;
}
    
private string LoadLogContents(string sID, int nNumEntries)
    {
        StringBuilder sbReturn = new StringBuilder();
        ArrayList saEvents = new ArrayList();
        EkEventLog eventLog = new EkEventLog();
        string[] eventSeparators = new string[] { EkEventLog.SPLIT_VAL };
        string[] sResult;

        //----- Load the events from the event viewer.
        if (sID == EVENT_APPLICATION || sID == EVENT_CMS)
            saEvents = eventLog.GetApplicationEntries(nNumEntries);
        else if (sID == EVENT_SECURITY)
            saEvents = eventLog.GetSecurityEntries(nNumEntries);
        else if (sID == EVENT_SYSTEM)
            saEvents = eventLog.GetSystemEntries(nNumEntries);
        else
            saEvents = null;

        //----- Add the heading so user knows what the columns display.
        if (sID == EVENT_CMS)
            sbReturn.AppendLine("       <div id=\"" + sID + "\" style=\"display:inline;\">");
        else
            sbReturn.AppendLine("       <div id=\"" + sID + "\" style=\"display:none;\">");

        sbReturn.AppendLine("           <table>");
        sbReturn.AppendLine("               <tr class=\"eventRowHeader\">");
        sbReturn.AppendLine("                   <td>Type</td>");
        sbReturn.AppendLine("                   <td>Source</td>");
        sbReturn.AppendLine("                   <td>Description</td>");
        sbReturn.AppendLine("                   <td>Time</td>");
        sbReturn.AppendLine("               </tr>");

        if (saEvents != null)
        {
            if (saEvents.Count < nNumEntries)
                nNumEntries = saEvents.Count;

            //----- Build the table rows that display the events.  Events are returned in a string using a seperator to 
            //----- seperate each column entry.
            for (int i = 0; i < nNumEntries; i++)
            {
                sResult = saEvents[i].ToString().Split(eventSeparators, StringSplitOptions.None);

                sResult[2] = sResult[2].Replace("<", "&#60;");
                sResult[2] = sResult[2].Replace(">", "&#62;");

                if ((sID != EVENT_CMS) || ((sID == EVENT_CMS) && (sResult[0].ToLower() == "cms400")))
                {
                    sbReturn.AppendLine("           <tr class=\"eventRowData\">");
                    sbReturn.AppendLine("               <td class=\"" + sResult[1] + "\">" + sResult[1] + "</td>");
                    sbReturn.AppendLine("               <td class=\"eventSource\">" + sResult[0] + "</td>");
                    sbReturn.AppendLine("               <td class=\"eventDesc\">" + sResult[2] + "</td>");
                    sbReturn.AppendLine("               <td class=\"eventDateTime\">" + sResult[3] + "</td>");
                    sbReturn.AppendLine("           </tr>");
                }
            }
        }

        sbReturn.AppendLine("           </table>");
        sbReturn.AppendLine("       </div>");

        if (eventLog != null)
            eventLog = null;
        if (saEvents != null)
            saEvents = null;

        return sbReturn.ToString();
    }

    //----- Used to load any error and/or error text into the aspx page.
    private string LoadError(int nErrorCode, string sMessage)
    {
        StringBuilder sbReturn = new StringBuilder();

        sbReturn.AppendLine("   <div id=\"error\">");

        switch (nErrorCode)
        {
            case ERR_DEFAULT:
                {
                    sbReturn.AppendLine("       " + ERR_DEFAULT_RESPONSE);
                    break;
                }
            case ERR_NOT_LOCAL:
                {
                    sbReturn.AppendLine("       " + ERR_NOT_LOCAL_RESPONSE);
                    break;
                }
            default:
                {
                    sbReturn.AppendLine("       " + ERR_DEFAULT_RESPONSE);
                    break;
                }
        }

        if (sMessage.Length > 0)
            sbReturn.AppendLine("       <p>" + sMessage + "</p>");

        sbReturn.AppendLine("   </div>");

        return sbReturn.ToString();
    }

    //----- Overloaded so a string does not have to be specified to load an error.
    private string LoadError(int nErrorCode)
    {
        return LoadError(nErrorCode, "");
    }
}