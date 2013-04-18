using System;
using System.IO;
using System.Data;
using System.Diagnostics;
using System.Configuration;
using System.Web;
using Microsoft.Win32;

/// <summary>
/// Summary description for EkVersion
/// </summary>
public class EkVersion
{
    public EkVersion()
    {

    }

    public string GetCMS400Version()
    {
        string sCMS400Ver = "CMS400 Not installed on this server";
        string sCurDir = "";

        sCurDir = HttpContext.Current.Server.MapPath("~/bin/Ektron.Cms.Common.dll");

        if (sCurDir != null)
        {
            sCMS400Ver = FileVersionInfo.GetVersionInfo(sCurDir).FileVersion;
        }

        return sCMS400Ver;
    }

    public string GetDotNetVersion()
    {
        string sReturn = "";

        sReturn = System.Environment.Version.ToString();

        return sReturn;
    }

    public string GetServerOSVersion()
    {
        return System.Environment.OSVersion.ToString();
    }

    public string GetIISVersion()
    {
        string sReturn = "Could not find registry key";
        string sIISVersionMaj, sIISVersionMin;

        if ((Registry.GetValue("HKEY_LOCAL_MACHINE\\SYSTEM\\CurrentControlSet\\Services\\W3SVC\\Parameters", "MajorVersion", "NoExist") != null) && 
            (Registry.GetValue("HKEY_LOCAL_MACHINE\\SYSTEM\\CurrentControlSet\\Services\\W3SVC\\Parameters", "MinorVersion", "NoExist") != null))
        {
            if ((sIISVersionMaj = Registry.GetValue("HKEY_LOCAL_MACHINE\\SYSTEM\\CurrentControlSet\\Services\\W3SVC\\Parameters", "MajorVersion", "NoExist").ToString())
                != "NoExist" &&
                ((sIISVersionMin = Registry.GetValue("HKEY_LOCAL_MACHINE\\SYSTEM\\CurrentControlSet\\Services\\W3SVC\\Parameters", "MinorVersion", "NoExist").ToString())
                != "NoExist"))
            {
                sReturn = sIISVersionMaj + "." + sIISVersionMin;
            }
        }

        return sReturn;
    }

    public string GetVSVersion()
    {
        string sVSVer = "Visual Studio Not installed on this server";
        string sVSInstall;

        //----- Checks english install version.
        if ((Registry.GetValue("HKEY_LOCAL_MACHINE\\SOFTWARE\\Microsoft\\VisualStudio\\8.0\\Setup\\VS\\BuildNumber", "1033", "NoExist") != null) && 
            (sVSInstall = Registry.GetValue("HKEY_LOCAL_MACHINE\\SOFTWARE\\Microsoft\\VisualStudio\\8.0\\Setup\\VS\\BuildNumber", "1033", "NoExist").ToString())
            != "NoExist")
        {
            sVSVer = sVSInstall;
        }
        else if ((Registry.GetValue("HKEY_LOCAL_MACHINE\\SOFTWARE\\Microsoft\\VisualStudio\\7.1\\Setup\\VS\\BuildNumber", "1033", "NoExist") != null) &&
                (sVSInstall = Registry.GetValue("HKEY_LOCAL_MACHINE\\SOFTWARE\\Microsoft\\VisualStudio\\7.1\\Setup\\VS\\BuildNumber", "1033", "NoExist").ToString())
                != "NoExist")
        {
            sVSVer = sVSInstall;
        }
        else if ((Registry.GetValue("HKEY_LOCAL_MACHINE\\SOFTWARE\\Microsoft\\VisualStudio\\7.0\\Setup\\VS\\BuildNumber", "1033", "NoExist") != null) &&
               (sVSInstall = Registry.GetValue("HKEY_LOCAL_MACHINE\\SOFTWARE\\Microsoft\\VisualStudio\\7.0\\Setup\\VS\\BuildNumber", "1033", "NoExist").ToString())
                 != "NoExist")
        {
                sVSVer = sVSInstall;
        }
        else
        {
            if (Registry.GetValue("HKEY_LOCAL_MACHINE\\SOFTWARE\\Microsoft\\VWDExpress\\8.0", "InstallDir", "NoExist") != null)
            {
                sVSVer = "Visual Studio Express";
            }
        }

        return sVSVer;
    }

    public string GetEktronWindowsServicesVersion()
    {
        string sServiceVersion = "";
        string sServicePath = "";

        if ((Registry.GetValue(@"HKEY_LOCAL_MACHINE\SYSTEM\CurrentControlSet\Services\EktronWindowsServices20", "ImagePath", "NoExist") != null) &&
           (sServicePath = Registry.GetValue(@"HKEY_LOCAL_MACHINE\SYSTEM\CurrentControlSet\Services\EktronWindowsServices20", "ImagePath", "NoExist").ToString())
            != "NoExist")
        {
            sServiceVersion = FileVersionInfo.GetVersionInfo(sServicePath).FileVersion;
        }
        else
        {
            sServiceVersion = "Not installed";
        }

        return sServiceVersion;
    }
    public string GetEktronWindowsServices30Version()
    {
        string sServiceVersion = "";
        string sServicePath = "";

        if ((Registry.GetValue(@"HKEY_LOCAL_MACHINE\SYSTEM\CurrentControlSet\Services\EktronWindowsServices30", "ImagePath", "NoExist") != null) &&
           (sServicePath = Registry.GetValue(@"HKEY_LOCAL_MACHINE\SYSTEM\CurrentControlSet\Services\EktronWindowsServices30", "ImagePath", "NoExist").ToString())
            != "NoExist")
        {
            sServiceVersion = FileVersionInfo.GetVersionInfo(sServicePath).FileVersion;
        }
        else
        {
            sServiceVersion = "Not installed";
        }

        return sServiceVersion;
    }
	public string GetEktronWindowsServices40Version() //--defect 55942 , checking EWS4.0 in registry
    {
        string sServiceVersion = "";
        string sServicePath = "";

        if ((Registry.GetValue(@"HKEY_LOCAL_MACHINE\SYSTEM\CurrentControlSet\Services\EktronWindowsServices40", "ImagePath", "NoExist") != null) &&
           (sServicePath = Registry.GetValue(@"HKEY_LOCAL_MACHINE\SYSTEM\CurrentControlSet\Services\EktronWindowsServices40", "ImagePath", "NoExist").ToString())
            != "NoExist")
        {
            sServiceVersion = FileVersionInfo.GetVersionInfo(sServicePath).FileVersion;
        }
        else
        {
            sServiceVersion = "Not installed";
        }

        return sServiceVersion;
    }
}