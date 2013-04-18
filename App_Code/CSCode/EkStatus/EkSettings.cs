using System;
using System.Data;
using System.IO;
using System.Configuration;
using System.Collections;
using System.Text;
using System.Web;
using System.Web.Configuration;
using System.Security.AccessControl;
using System.Security.Principal;

/// <summary>
/// Summary description for EkSettings
/// </summary>
public class EkSettings : IDisposable
{
    public const string EK_SMTP_SERVER = "ek_SMTPServer";
    public const string EK_SMTP_PORT = "ek_SMTPPort";
    public const string EK_SMTP_USER = "ek_SMTPUser";
    public const string EK_SMTP_PASS = "ek_SMTPPass";
    public const string EK_AD_ENABLED = "ek_ADEnabled";
    public const string EK_AD_ADVANCED_CONFIG = "ek_ADAdvancedConfig";
    public const string EK_AD_AUTH_PROTOCOL = "ek_AUTH_Protocol";
    public const string EK_AD_USER = "ek_ADUsername";
    public const string EK_AD_PWRD = "ek_ADPassword";
    public const string EK_DMS_CAT_STOR = "StorageLocation=";
    public const string EK_DMS_CAT_NAME = "CatalogName=";
    public const string EK_DMS_CAT_LOC = "CatalogLoc=";
    public const string EK_DMS_USER_NAME = "UserName=";
    public const string EK_DMS_USER_DOMAIN = "UserDomain=";

    private const string SMTP_SERVER = "SMTP Server";
    private const string SMTP_PORT = "SMTP Port";
    private const string SMTP_USER = "SMTP User";
    private const string SMTP_PASS = "SMTP Pass";
    private const string AD_ENABLED = "Active Directory Enabled";
    private const string AD_ADVANCED_CONFIG = "Active Directory Advanced Config";
    private const string AD_AUTH_PROTOCOL = "Active Directory Authentication Protocol";
    private const string AD_USER = "Active Directory Username";
    private const string AD_PWRD = "Active Directory Password";
    private const string DMS_CAT_NAME = "DMS Catalog Name";
    
    public const string DMS_CAT_STOR = "DMS Storage Location";
    public const string DMS_CAT_LOC = "DMS Catalog Location";
    public const string DMS_USER_NAME = "DMS User Name";
    public const string DMS_USER_DOMAIN = "DMS User Domain";

    private const string UNDEFINED = "Not defined";
    private const string NOSETTING = "Value not set in configuration";

    private System.Collections.Specialized.NameValueCollection m_config;
    private System.Collections.Specialized.NameValueCollection m_webConfigSettings = null;
    private string m_sAppPath = "";
    private string m_sDmsUser = "";
    private string m_sDmsUserDomain = "";
    private string m_sDmsFullUserName;
    private string m_sAspFullUserName;

    public EkSettings(string sAppPath)
	{
        m_config = new System.Collections.Specialized.NameValueCollection(WebConfigurationManager.AppSettings);
        m_webConfigSettings = new System.Collections.Specialized.NameValueCollection();
        m_sAppPath = sAppPath;
        GetSMTPSettings();
        GetADSettings();
        GetAssetSettings();
        m_sAspFullUserName = Environment.UserDomainName + "\\" + Environment.UserName;
	}

    public System.Collections.Specialized.NameValueCollection webConfigSettings
    {
        get { return m_webConfigSettings; }
    }

    public string ASPNETUserName
    {
        get { return m_sAspFullUserName; }
    }

    public string DMSUserName
    {
        get { return m_sDmsFullUserName; }
    }

    private void GetSMTPSettings()
    {
        //----- Read web.config for SMTP settings.  Check they exist first, this keeps the app from throwing.
        if (m_config[EK_SMTP_SERVER] != null)
            m_webConfigSettings.Add(SMTP_SERVER, m_config.Get(EK_SMTP_SERVER) != "" ? m_config.Get(EK_SMTP_SERVER) : NOSETTING);
        else
            m_webConfigSettings.Add(SMTP_SERVER, UNDEFINED);

        if (m_config[EK_SMTP_PORT] != null)
            m_webConfigSettings.Add(SMTP_PORT, m_config.Get(EK_SMTP_PORT) != "" ? m_config.Get(EK_SMTP_PORT) : NOSETTING);
        else
            m_webConfigSettings.Add(SMTP_PORT, UNDEFINED);

        if (m_config[EK_SMTP_USER] != null)
            m_webConfigSettings.Add(SMTP_USER, m_config.Get(EK_SMTP_USER) != "" ? m_config.Get(EK_SMTP_USER) : NOSETTING);
        else
            m_webConfigSettings.Add(SMTP_USER, UNDEFINED);

        if (m_config[EK_SMTP_PASS] != null)
            m_webConfigSettings.Add(SMTP_PASS, m_config.Get(EK_SMTP_PASS) != "" ? m_config.Get(EK_SMTP_PASS) : NOSETTING);
        else
            m_webConfigSettings.Add(SMTP_PASS, UNDEFINED);
    }

    private void GetADSettings()
    {
        if (m_config[EK_AD_ENABLED] != null)
            m_webConfigSettings.Add(AD_ENABLED, m_config.Get(EK_AD_ENABLED) != "" ? m_config.Get(EK_AD_ENABLED) : NOSETTING);
        else
            m_webConfigSettings.Add(AD_ENABLED, UNDEFINED);

        if (m_config[EK_AD_ADVANCED_CONFIG] != null)
            m_webConfigSettings.Add(AD_ADVANCED_CONFIG, m_config.Get(EK_AD_ADVANCED_CONFIG) != "" ? m_config.Get(EK_AD_ADVANCED_CONFIG) : NOSETTING);
        else
            m_webConfigSettings.Add(AD_ADVANCED_CONFIG, UNDEFINED);

        if (m_config[EK_AD_AUTH_PROTOCOL] != null)
            m_webConfigSettings.Add(AD_AUTH_PROTOCOL, m_config.Get(EK_AD_AUTH_PROTOCOL) != "" ? m_config.Get(EK_AD_AUTH_PROTOCOL) : NOSETTING);
        else
            m_webConfigSettings.Add(AD_AUTH_PROTOCOL, UNDEFINED);

        if (m_config[EK_AD_USER] != null)
            m_webConfigSettings.Add(AD_USER, m_config.Get(EK_AD_USER) != "" ? m_config.Get(EK_AD_USER) : NOSETTING);
        else
            m_webConfigSettings.Add(AD_USER, UNDEFINED);

        if (m_config[EK_AD_PWRD] != null)
            m_webConfigSettings.Add(AD_PWRD, m_config.Get(EK_AD_PWRD) != "" ? m_config.Get(EK_AD_PWRD) : NOSETTING);
        else
            m_webConfigSettings.Add(AD_PWRD, UNDEFINED);
    }

    private void GetAssetSettings()
    {
        string sPath;
        string sTemp;
        string sFileContents;

        sPath = m_sAppPath + "\\AssetManagement.config";

        if(File.Exists(sPath))
        {
            sFileContents = File.ReadAllText(sPath);

            if (sFileContents.Contains(EK_DMS_CAT_STOR))
            {
                sTemp = sFileContents.Substring(sFileContents.IndexOf(EK_DMS_CAT_STOR) + (EK_DMS_CAT_STOR.Length + 1));
                m_webConfigSettings.Add(DMS_CAT_STOR, sTemp.Substring(0, sTemp.IndexOf("\"")));
            }
            else
                m_webConfigSettings.Add(DMS_CAT_STOR, UNDEFINED);

            if (sFileContents.Contains(EK_DMS_CAT_LOC))
            {
                sTemp = sFileContents.Substring(sFileContents.IndexOf(EK_DMS_CAT_LOC) + (EK_DMS_CAT_LOC.Length + 1));
                m_webConfigSettings.Add(DMS_CAT_LOC, sTemp.Substring(0, sTemp.IndexOf("\"")));
            }
            else
                m_webConfigSettings.Add(DMS_CAT_LOC, UNDEFINED);

            if (sFileContents.Contains(EK_DMS_CAT_NAME))
            {
                sTemp = sFileContents.Substring(sFileContents.IndexOf(EK_DMS_CAT_NAME) + (EK_DMS_CAT_NAME.Length + 1));
                m_webConfigSettings.Add(DMS_CAT_NAME, sTemp.Substring(0, sTemp.IndexOf("\"")));
            }
            else
                m_webConfigSettings.Add(DMS_CAT_NAME, UNDEFINED);

            if (sFileContents.Contains(EK_DMS_USER_NAME))
            {
                sTemp = sFileContents.Substring(sFileContents.IndexOf(EK_DMS_USER_NAME) + (EK_DMS_USER_NAME.Length + 1));
                m_webConfigSettings.Add(DMS_USER_NAME, sTemp.Substring(0, sTemp.IndexOf("\"")));
                m_sDmsUser = sTemp.Substring(0, sTemp.IndexOf("\""));
            }
            else
                m_webConfigSettings.Add(DMS_USER_NAME, UNDEFINED);

            if (sFileContents.Contains(EK_DMS_USER_DOMAIN))
            {
                sTemp = sFileContents.Substring(sFileContents.IndexOf(EK_DMS_USER_DOMAIN) + (EK_DMS_USER_DOMAIN.Length + 1));
                m_webConfigSettings.Add(DMS_USER_DOMAIN, sTemp.Substring(0, sTemp.IndexOf("\"")));
                m_sDmsUserDomain = sTemp.Substring(0, sTemp.IndexOf("\""));
            }
            else
                m_webConfigSettings.Add(DMS_USER_DOMAIN, UNDEFINED);

            m_sDmsFullUserName = m_sDmsUser + "\\" + m_sDmsUserDomain;
        }
    }

    public ArrayList GetPermissions(string sPath)
    {
        ArrayList alReturn = new ArrayList();
        System.Collections.Specialized.NameValueCollection nvcTemp;
        int nIndex = -1;
        int i;

        if (sPath != string.Empty && Directory.Exists(sPath))
        {
            DirectorySecurity dirSec = Directory.GetAccessControl(sPath);
            foreach (FileSystemAccessRule tempRule in dirSec.GetAccessRules(true, true, typeof(NTAccount)))
            {
                for (i = 0; i < alReturn.Count; i++)
                {
                    if (((EkSecurityObject)alReturn[i]).NTAccountName == ((NTAccount)tempRule.IdentityReference).ToString())
                    {
                        nIndex = i;
                        break;
                    }
                }

                if (nIndex != i)
                {
                    alReturn.Add(new EkSecurityObject());
                    ((EkSecurityObject)alReturn[i]).NTAccountName = ((NTAccount)tempRule.IdentityReference).ToString();
                }

                nvcTemp = new System.Collections.Specialized.NameValueCollection(((EkSecurityObject)alReturn[i]).FileSystemRights);

                AddUpdateRight(ref nvcTemp, "Create Files", tempRule.AccessControlType, tempRule.FileSystemRights, FileSystemRights.CreateFiles);
                AddUpdateRight(ref nvcTemp, "Read Data", tempRule.AccessControlType, tempRule.FileSystemRights, FileSystemRights.ReadData);
                AddUpdateRight(ref nvcTemp, "Read Attributes", tempRule.AccessControlType, tempRule.FileSystemRights, FileSystemRights.ReadAttributes);
                AddUpdateRight(ref nvcTemp, "Read Extended Attributes", tempRule.AccessControlType, tempRule.FileSystemRights, FileSystemRights.ReadExtendedAttributes);
                AddUpdateRight(ref nvcTemp, "Create Folders", tempRule.AccessControlType, tempRule.FileSystemRights, FileSystemRights.CreateDirectories);
                AddUpdateRight(ref nvcTemp, "Write Attributes", tempRule.AccessControlType, tempRule.FileSystemRights, FileSystemRights.WriteAttributes);
                AddUpdateRight(ref nvcTemp, "Write Extended Attributes", tempRule.AccessControlType, tempRule.FileSystemRights, FileSystemRights.WriteExtendedAttributes);
                AddUpdateRight(ref nvcTemp, "Delete", tempRule.AccessControlType, tempRule.FileSystemRights, FileSystemRights.Delete);
                AddUpdateRight(ref nvcTemp, "Full Control", tempRule.AccessControlType, tempRule.FileSystemRights, FileSystemRights.FullControl);
                AddUpdateRight(ref nvcTemp, "Modify", tempRule.AccessControlType, tempRule.FileSystemRights, FileSystemRights.Modify);
                AddUpdateRight(ref nvcTemp, "Read", tempRule.AccessControlType, tempRule.FileSystemRights, FileSystemRights.Read);
                AddUpdateRight(ref nvcTemp, "Write", tempRule.AccessControlType, tempRule.FileSystemRights, FileSystemRights.Write);

                ((EkSecurityObject)alReturn[i]).FileSystemRights.Clear();
                ((EkSecurityObject)alReturn[i]).FileSystemRights = null;
                ((EkSecurityObject)alReturn[i]).FileSystemRights = new System.Collections.Specialized.NameValueCollection(nvcTemp);

                nvcTemp.Clear();
                nvcTemp = null;
            }
        }

        return alReturn;
    }

    private bool DotNetBitwiseAnd(System.Security.AccessControl.FileSystemRights accountRights, System.Security.AccessControl.FileSystemRights testRight)
    {
        return (((int)accountRights & (int)testRight) == (int)testRight);
    }

    private void AddUpdateRight(ref System.Collections.Specialized.NameValueCollection security, string sRight, AccessControlType controlType, FileSystemRights ruleRights, FileSystemRights constRights)
    {
        if (DotNetBitwiseAnd(ruleRights, constRights))
        {
            if ((security.Get(sRight) != null) && (security.Get(sRight) != "false"))
            {
                security.Remove(sRight);
                security.Add(sRight, controlType == AccessControlType.Allow ? "true" : "false");
            }
            else
            {
                security.Add(sRight, controlType == AccessControlType.Allow ? "true" : "false");
            }
        }
        else
        {
            if (security.Get(sRight) == null)
                security.Add(sRight, "notSpecified");
        }
    }

    //----- Cleanup
    public void Dispose()
    {
        if (m_config != null)
            m_config = null;

        if (m_webConfigSettings != null)
            m_webConfigSettings = null;
    }
}

[Serializable]
public class EkSecurityObject
{
    private System.Collections.Specialized.NameValueCollection m_nvcFileSystemRights;
    private string m_sNTAccount;
    private bool m_bInherited;

    public EkSecurityObject()
    {
        m_nvcFileSystemRights = new System.Collections.Specialized.NameValueCollection();
        m_sNTAccount = string.Empty;
    }

    public System.Collections.Specialized.NameValueCollection FileSystemRights
    {
        get { return m_nvcFileSystemRights; }
        set { m_nvcFileSystemRights = value; }
    }

    public string NTAccountName
    {
        get { return m_sNTAccount; }
        set { m_sNTAccount = value; }
    }

    public bool Inherited
    {
        get { return m_bInherited; }
        set { m_bInherited = value; }
    }
}
