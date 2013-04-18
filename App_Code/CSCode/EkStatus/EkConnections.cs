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
using System.Net;
using System.Net.Mail;
using Ektron.Cms.Controls;
using System.IO;
using System.Text;
using System.ServiceProcess;
using Ektron.Cms; 

/// <summary>
/// Summary description for EkConnectivity
/// </summary>
public class EkConnections
{
    private System.Collections.Specialized.NameValueCollection m_webSystemSettings;
    private string m_sLastError = "";
    protected SiteAPI m_refSiteApi = new SiteAPI();
    private Ektron.Cms.Common.EkMessageHelper m_refMsg = null;

    public EkConnections(System.Collections.Specialized.NameValueCollection webSystemSettings)
    {
        m_webSystemSettings = new System.Collections.Specialized.NameValueCollection(webSystemSettings);
        m_refMsg = (new Ektron.Cms.ContentAPI()).EkMsgRef;
    }
    
    public string LastError
    {
        get { return m_sLastError; }
    }

    public bool TestServerControl()
    {
        bool bReturn = false;

        try
        {
            //----- Build a login control, if nothing throws assume server controls are working.
            Ektron.Cms.Controls.Login testLogin = new Ektron.Cms.Controls.Login();
            testLogin.Page = new System.Web.UI.Page();
            testLogin.Fill();

            if (testLogin != null)
                testLogin = null;

            bReturn = true;
        }
        catch (Exception exThrown)
        {
            //----- If something threw, record the last message.
            m_sLastError = exThrown.Message;
            bReturn = false;
        }

        return bReturn;
    }

    public bool TestServerControlWS(string sSiteUrl)
    {
        bool bReturn = false;
        string sResults = "";
        
        try
        {
            //----- Create url to the webservice.
            if(sSiteUrl != "")
            {
                sSiteUrl = sSiteUrl.Substring(0, sSiteUrl.LastIndexOf('/'));
                sSiteUrl = sSiteUrl.Substring(0, sSiteUrl.LastIndexOf('/') + 1);
                sSiteUrl = sSiteUrl + "ServerControlWS.asmx/TestConnection";
            }
            else
            {
                //----- Default to the usual if no value passed in.
                sSiteUrl = "http://localhost/CMS400Demo/WorkArea/ServerControlWS.asmx/TestConnection";
            }

            //----- Call to connect to url.
            sResults = ConnectToURL(sSiteUrl);

            //----- Check results with what we expect.
            if (sResults.Contains("<boolean xmlns=\"http://www.ektron.com/CMS400/Webservice\">true</boolean>"))
            {
                bReturn = true;
            }
            else
            {
                m_sLastError = sResults;
                bReturn = false;
            }
        }
        catch (Exception exThrown)
        {
            //----- If something threw, record the last message.
            m_sLastError = exThrown.Message;
            bReturn = false;
        }

        return bReturn;
    }

    public bool TestContentWS(string sSiteUrl)
    {
        bool bReturn = false;
        string sResults = "";
        string defaultContentLanguage = System.Configuration.ConfigurationManager.AppSettings["ek_DefaultContentLanguage"];
        Ektron.Cms.LanguageData language_data = new LanguageData();
        language_data = m_refSiteApi.GetLanguageById(Int32.Parse(defaultContentLanguage));
        string langName = language_data.LocalName;
        
        try
        {
            //----- Create url to the webservice.
            if (sSiteUrl != "")
            {
                sSiteUrl = sSiteUrl.Substring(0, sSiteUrl.LastIndexOf('/'));
                sSiteUrl = sSiteUrl.Substring(0, sSiteUrl.LastIndexOf('/') + 1);
                sSiteUrl = sSiteUrl + "webservices/ContentWS.asmx/ecmLanguageSelect?SiteLanguage=" + defaultContentLanguage;
            }
            else
            {
                //----- Default to the usual if no value passed in.
                sSiteUrl = "http://localhost/CMS400Demo/WorkArea/webservices/ContentWS.asmx/ecmLanguageSelect?SiteLanguage=" + defaultContentLanguage;
            }

            //----- Call to connect to url.
            sResults = ConnectToURL(sSiteUrl);

            //----- Check results with what we expect.
            if (sResults.Contains("&lt;option value=\"" + defaultContentLanguage + "\" selected=\"selected\"&gt;" + langName + "&lt;/option&gt;"))
            {
                bReturn = true;
            }
            else
            {
                m_sLastError = sResults;
                bReturn = false;
            }
        }
        catch (Exception exThrown)
        {
            //----- If something threw, record the last message.
            m_sLastError = exThrown.Message;
            bReturn = false;
        }

        return bReturn;
    }

    public bool TestContentService(string sSiteUrl)
    {
        bool bReturn = false;
        string sResults = "";

        try
        {
            //----- Create url to the webservice.
            if (sSiteUrl != "")
            {
                sSiteUrl = sSiteUrl.Substring(0, sSiteUrl.LastIndexOf('/'));
                sSiteUrl = sSiteUrl.Substring(0, sSiteUrl.LastIndexOf('/') + 1);
                sSiteUrl = sSiteUrl + "webservices/ContentService.asmx/DoLogin";
            }
            else
            {
                //----- Default to the usual if no value passed in.
                sSiteUrl = "http://localhost/CMS400Demo/WorkArea/webservices/ContentService.asmx/DoLogin";
            }

            //----- Call to connect to url.
            sResults = ConnectToURL(sSiteUrl);

            //----- Check results with what we expect.
            if (sResults.Contains("<boolean xmlns=\"http://www.ektron.com/webservices/\">false</boolean>") || 
                sResults.Contains("<boolean xmlns=\"http://www.ektron.com/webservices/\">true</boolean>"))
            {
                bReturn = true;
            }
            else
            {
                m_sLastError = sResults;
                bReturn = false;
            }
        }
        catch (Exception exThrown)
        {
            //----- If something threw, record the last message.
            m_sLastError = exThrown.Message;
            bReturn = false;
        }

        return bReturn;
    }

    public bool TestAuthService(string sSiteUrl)
    {
        bool bReturn = false;
        string sResults = "";

        try
        {
            //----- Create url to the webservice.
            if (sSiteUrl != "")
            {
                sSiteUrl = sSiteUrl.Substring(0, sSiteUrl.LastIndexOf('/'));
                sSiteUrl = sSiteUrl.Substring(0, sSiteUrl.LastIndexOf('/') + 1);
                sSiteUrl = sSiteUrl + "webservices/AuthService.asmx/isValidUser";
            }
            else
            {
                //----- Default to the usual if no value passed in.
                sSiteUrl = "http://localhost/CMS400Demo/WorkArea/webservices/AuthService.asmx/isValidUser?username=builtin";
            }

            //----- Call to connect to url.
            sResults = ConnectToURL(sSiteUrl);

            //----- Check results with what we expect.
            if (sResults.Contains("<boolean xmlns=\"http://www.ektron.com/webservices/\">false</boolean>"))
            {
                bReturn = true;
            }
            else
            {
                m_sLastError = sResults;
                bReturn = false;
            }
        }
        catch (Exception exThrown)
        {
            //----- If something threw, record the last message.
            m_sLastError = exThrown.Message;
            bReturn = false;
        }

        return bReturn;
    }

    public string GetIndexerState()
    {
        string sReturn = "";
        int iSystemLocale = System.Globalization.CultureInfo.CurrentUICulture.LCID;
        string sServiceName = m_refMsg.GetMessageForLanguage("ms:indexingservicename", iSystemLocale);
        ServiceController sc = new ServiceController(sServiceName);

        if ((sc.Status.Equals(ServiceControllerStatus.Stopped)) ||
           (sc.Status.Equals(ServiceControllerStatus.StopPending)))
            sReturn = "Stopped";
        else if ((sc.Status.Equals(ServiceControllerStatus.Paused)) ||
                 (sc.Status.Equals(ServiceControllerStatus.PausePending)))
            sReturn = "Paused";
        else if (sc.Status.Equals(ServiceControllerStatus.StartPending))
            sReturn = "Starting";
        else
            sReturn = "Started";

        return sReturn;
    }

    public string GetExtensionServerState()
    {
        string sReturn = "";
        ServiceController sc = new ServiceController("Ektron Extensibility Server");

        if ((sc.Status.Equals(ServiceControllerStatus.Stopped)) ||
           (sc.Status.Equals(ServiceControllerStatus.StopPending)))
            sReturn = "Stopped";
        else if ((sc.Status.Equals(ServiceControllerStatus.Paused)) ||
                 (sc.Status.Equals(ServiceControllerStatus.PausePending)))
            sReturn = "Paused";
        else if (sc.Status.Equals(ServiceControllerStatus.StartPending))
            sReturn = "Starting";
        else
            sReturn = "Started";

        return sReturn;
    }

    public string GetEktronWindowsServiceState()
    {
        string sReturn = "";
        ServiceController sc = new ServiceController("Ektron Windows Services 2.0");

        if ((sc.Status.Equals(ServiceControllerStatus.Stopped)) ||
           (sc.Status.Equals(ServiceControllerStatus.StopPending)))
            sReturn = "Stopped";
        else if ((sc.Status.Equals(ServiceControllerStatus.Paused)) ||
                 (sc.Status.Equals(ServiceControllerStatus.PausePending)))
            sReturn = "Paused";
        else if (sc.Status.Equals(ServiceControllerStatus.StartPending))
            sReturn = "Starting";
        else
            sReturn = "Started";

        return sReturn;
    }

    public string GetEktronWindowsService30State()
    {
        string sReturn = "";
        ServiceController sc = new ServiceController("Ektron Windows Services 3.0");

        if ((sc.Status.Equals(ServiceControllerStatus.Stopped)) ||
           (sc.Status.Equals(ServiceControllerStatus.StopPending)))
            sReturn = "Stopped";
        else if ((sc.Status.Equals(ServiceControllerStatus.Paused)) ||
                 (sc.Status.Equals(ServiceControllerStatus.PausePending)))
            sReturn = "Paused";
        else if (sc.Status.Equals(ServiceControllerStatus.StartPending))
            sReturn = "Starting";
        else
            sReturn = "Started";

        return sReturn;
    }
	public string GetEktronWindowsService40State() //Defect#55942 -- Checking the status of EWS4.0
    {
        string sReturn = "";
        ServiceController sc = new ServiceController("Ektron Windows Services 4.0");

        if ((sc.Status.Equals(ServiceControllerStatus.Stopped)) ||
           (sc.Status.Equals(ServiceControllerStatus.StopPending)))
            sReturn = "Stopped";
        else if ((sc.Status.Equals(ServiceControllerStatus.Paused)) ||
                 (sc.Status.Equals(ServiceControllerStatus.PausePending)))
            sReturn = "Paused";
        else if (sc.Status.Equals(ServiceControllerStatus.StartPending))
            sReturn = "Starting";
        else
            sReturn = "Started";

        return sReturn;
    }
    private string ConnectToURL(string sUrl)
    {
        HttpWebRequest reqServerWS = null;
        HttpWebResponse respServerWS = null;
        Stream respStream = null;
        StreamReader readStream = null;
        string sReturn = "";

        //----- Create webRequest and get the response back.
        reqServerWS = (HttpWebRequest)HttpWebRequest.Create(sUrl);
        reqServerWS.UseDefaultCredentials = true;
        respServerWS = (HttpWebResponse)reqServerWS.GetResponse();

        respStream = respServerWS.GetResponseStream();
        readStream = new StreamReader(respStream, Encoding.UTF8);

        sReturn = readStream.ReadToEnd();

        //----- Cleanup
        if (reqServerWS != null)
            reqServerWS = null;
        if (respServerWS != null)
            respServerWS = null;
        if (readStream != null)
            readStream = null;

        return sReturn;
    }

    public bool ConnectToSMTP(string sServer, int iPort, string sUserID, string sPassword)
    {
        return true;
    }
}
