using System;
using System.Data;
using System.Web.Configuration;
using Ektron.Cms;
using System.Configuration;


/// <summary>
/// Summary description for EkSql
/// </summary>
public class EkDb : IDisposable
{
    private const string SQL_VERSION = "select @@version";
    private const string ORACLE_VERSION = "select * from v$version";
    private const string QUERY_SQL_TEST = "select COUNT(*) from settings";
    private const string QUERY_ORACLE_TEST = "SELECT COUNT(*) AS EXPR1 FROM SETTINGS";
    private const string QUERY_ALL_USERNAME = "SELECT COUNT(*) FROM users WHERE user_name='{0}'";
    private const string QUERY_ALL_PWRD = "SELECT user_pwd FROM users WHERE user_name='{0}'";
    private const string QUERY_ALL_GETLIC = "SELECT lic_key FROM settings";

    private const string QUERY_AD_INT = "SELECT enable_adintegration FROM settings";
    private const string QUERY_AD_AUTOUSER = "SELECT enable_adautouseradd FROM settings";
    private const string QUERY_AD_AUTOUSERTOGROUP = "SELECT enable_adautousertogroup FROM settings";
    private const string QUERY_AD_AUTH = "SELECT enable_adauthentication FROM settings";
    private const string QUERY_AD_DOMAIN = "SELECT ad_domainname FROM settings";
    private const string QUERY_AD_DOMAINS = "SELECT ad_domain_tbl_id, domain_short_name, domain_path, domain_dns, netbios, username, password, server_ip FROM ad_domain_tbl";

    private const string PROVIDER_SQL = "System.Data.SqlClient";
    private const string PROVIDER_ORACLE = "System.Data.OracleClient";
    private const string NAME_SQL = "SQL";
    private const string NAME_ORACLE = "Oracle";
    private const string CONN_SOURCE = "data source=";
    private const string CONN_SERVER = "server=";
    private const string CONN_DATABASE = "database=";
    private const string CONN_CATALOG = "initial catalog=";
    private const string CONN_SECURITY = "integrated security=";
    private const string CONN_TRUSTED = "trusted_connection=";
    private const string CONN_USER = "user id=";
    private const string CONN_PASSWD = "password=";
    private const char CONN_SPLIT = ';';
    private const char CONN_EQUAL = '=';

    private ConnectionStringSettings m_cssEkDb;
    private System.Data.SqlClient.SqlConnection m_sqlConn = null;
    //private System.Data.OracleClient.OracleConnection m_oracleConn = null;
    private string m_sDbType = "";
    private string m_sLastError = "";
    private string m_sConnState = "";
    private string m_sConnString = "";
    private string m_sDbSource = "";
    private string m_sDbName = "";
    private string m_sSecurity = "";
    private string m_sUser = "";
    private string m_sPassword = "";
    private string m_sTrusted = "";

#region Constructor
    public EkDb()
	{
        //----- Grab the connection strings from web.config
        m_cssEkDb = WebConfigurationManager.ConnectionStrings["Ektron.DbConnection"];

        //----- Figure out which type of database to deal with.
        switch (m_cssEkDb.ProviderName)
        {
            case PROVIDER_SQL:
                {
                    m_sDbType = NAME_SQL;
                    break;
                }
            case PROVIDER_ORACLE:
                {
                    m_sDbType = NAME_ORACLE;
                    break;
                }
            default:
                {
                    m_sDbType = "Provider in web.config is undefined";
                    break;
                }
        }

        m_sConnString = m_cssEkDb.ConnectionString;

        //----- Parse the connection string to provide easy access to it's components.
        ParseDbConnectionString();
        //----- Try to connect and set connection status.
        ConnectToDB();
	}
#endregion

#region Properties
    public string databaseType
    {
        get { return m_sDbType; }
    }

    public string connectionState
    {
        get { return m_sConnState; }
    }

    public string lastError
    {
        get { return m_sLastError; }
    }

    public string connectionServer
    {
        get { return m_sDbSource; }
    }

    public string connectionDatabase
    {
        get { return m_sDbName; }
    }

    public string connectionSecurity
    {
        get { return m_sSecurity; }
    }

    public string connectionUser
    {
        get { return m_sUser; }
    }

    public string connectionPassword
    {
        get { return m_sPassword; }
    }

    public string connectionTrusted
    {
        get { return m_sTrusted; }
    }

    public string connectionString
    {
        get { return m_sConnString; }
    }

#endregion

#region Private Methods
    //----- Seperates different parts of the connection string and places values in 
    //----- associated properties.
    private void ParseDbConnectionString()
    {
        string [] saConnSplit;
        string sTemp = "";

        //----- Split the string based on ';' should seperator be the default for web.config.
        saConnSplit = m_cssEkDb.ConnectionString.Split(CONN_SPLIT);

        //----- Loop though the array placing each element in the correct property.
        if (saConnSplit.Length > 0)
        {
            for (int i = 0; i < saConnSplit.Length; i++)
            {
                sTemp = saConnSplit[i];

                switch (sTemp.Substring(0, sTemp.LastIndexOf(CONN_EQUAL) + 1).ToLower())
                {
                    case CONN_SERVER:
                    case CONN_SOURCE:
                        {
                            m_sDbSource = sTemp.Substring(sTemp.LastIndexOf(CONN_EQUAL) + 1);
                            break;
                        }
                    case CONN_DATABASE:
                    case CONN_CATALOG:
                        {
                            m_sDbName = sTemp.Substring(sTemp.LastIndexOf(CONN_EQUAL) + 1);
                            break;
                        }
                    case CONN_SECURITY:
                        {
                            if (sTemp.Substring(sTemp.LastIndexOf(CONN_EQUAL) + 1).ToLower() == "true")
                                m_sSecurity = "On";
                            else if (sTemp.Substring(sTemp.LastIndexOf(CONN_EQUAL) + 1).ToLower() == "false")
                                m_sSecurity = "Off";
                            else
                                m_sSecurity = sTemp.Substring(sTemp.LastIndexOf(CONN_EQUAL) + 1);

                            break;
                        }
                    case CONN_TRUSTED:
                        {
                            m_sTrusted = sTemp.Substring(sTemp.LastIndexOf(CONN_EQUAL) + 1);
                            break;
                        }
                    case CONN_USER:
                        {
                            m_sUser = sTemp.Substring(sTemp.LastIndexOf(CONN_EQUAL) + 1);
                            break;
                        }
                    case CONN_PASSWD:
                        {
                            m_sPassword = sTemp.Substring(sTemp.LastIndexOf(CONN_EQUAL) + 1);
                            break;
                        }
                    default:
                        {
                            //----- Do nothing.
                            break;
                        }
                } //----- End switch
            } //----- End for loop
        } //----- End if        
    }

    private bool ConnectToDB()
    {
        bool bReturn = false;

        try
        {
            //----- Check if sql.
            if (IsDbSql())
            {
                //----- Create the connection object and open the database.
                m_sqlConn = new System.Data.SqlClient.SqlConnection(m_cssEkDb.ConnectionString);
                m_sqlConn.Open();
                m_sConnState = m_sqlConn.State.ToString();
                bReturn = true;
            }

            //----- Check if sql.
            //if (IsDbOracle())
            //{
            //    //----- Create the connection object and open the database.
            //    m_oracleConn = new System.Data.OracleClient.OracleConnection(m_cssEkDb.ConnectionString);
            //    m_oracleConn.Open();
            //    m_sConnState = m_oracleConn.State.ToString();
            //    bReturn = true;
            //}
        }
        catch (Exception exThrown)
        {
            //----- Whenever something bad happens set the connection state and last error.
            m_sLastError = exThrown.Message;
            m_sConnState = ConnectionState.Closed.ToString();
            bReturn = false;
        }

        return bReturn;
    }
#endregion

#region Public Methods
    //----- True = sql db specified in web.config
    public bool IsDbSql()
    {
        bool bReturn;

        if (m_sDbType == NAME_SQL)
            bReturn = true;
        else
            bReturn = false;

        return bReturn;
    }

    //----- True = oracle db specified in web.config
    public bool IsDbOracle()
    {
        bool bReturn;

        if (m_sDbType == NAME_ORACLE)
            bReturn = true;
        else
            bReturn = false;

        return bReturn;
    }

    public string GetDbVersion()
    {
        string sReturn = "";
        
        try
        {
            if (IsDbSql() && m_sqlConn != null)
            {
                System.Data.SqlClient.SqlCommand sqlCommand = null;
                sqlCommand = m_sqlConn.CreateCommand();
                sqlCommand.CommandText = SQL_VERSION;
                sReturn = sqlCommand.ExecuteScalar().ToString();

                if (sqlCommand != null)
                    sqlCommand = null;
            }

            //if (IsDbOracle() && m_oracleConn != null)
            //{
            //    System.Data.OracleClient.OracleCommand oracleCommand = null;
            //    oracleCommand = m_oracleConn.CreateCommand();
            //    oracleCommand.CommandText = ORACLE_VERSION;
            //    sReturn = oracleCommand.ExecuteOracleScalar().ToString();

            //    if (oracleCommand != null)
            //        oracleCommand = null;
            //}
        }
        catch (Exception exThrown)
        {
            m_sLastError = exThrown.Message;
            sReturn = "Unable to determine version";
        }

        return sReturn;
    }

    public string GetDbConnectionString()
    {
        string sReturn = "";

        //----- Check db type and make sure we have a valid object.
        if (IsDbSql() && m_sqlConn != null)
            sReturn = m_sqlConn.ConnectionString;

        //if (IsDbOracle() && m_oracleConn != null)
        //    sReturn = m_oracleConn.ConnectionString;

        return sReturn;
    }

    public bool TestQueryDb(ref int nRows)
    {
        bool bReturn = false;

        try
        {
            //----- Make sure connection is established and open.
            if (m_sConnState == ConnectionState.Open.ToString())
            {
                //----- Check if SQL.
                if (IsDbSql() && m_sqlConn != null)
                {
                    System.Data.SqlClient.SqlCommand sqlCommand = null;
                    sqlCommand = m_sqlConn.CreateCommand();
                    sqlCommand.CommandText = QUERY_SQL_TEST;
                    nRows = Convert.ToInt32(sqlCommand.ExecuteScalar().ToString());

                    if (sqlCommand != null)
                        sqlCommand = null;
                }

                //if (IsDbOracle() && m_oracleConn != null)
                //{
                //    System.Data.OracleClient.OracleCommand oracleCommand = null;
                //    oracleCommand = m_oracleConn.CreateCommand();
                //    oracleCommand.CommandText = QUERY_ORACLE_TEST;
                //    nRows = Convert.ToInt32(oracleCommand.ExecuteOracleScalar().ToString());

                //    if (oracleCommand != null)
                //        oracleCommand = null;
                //}

                if (nRows > 0)
                    bReturn = true;
                else
                    m_sLastError = "No data in settings table.";
            }
        }
        catch(Exception exThrown)
        {
            m_sLastError = exThrown.Message;
            bReturn = false;
        }

        return bReturn;
    }

    public bool GetPassword(string sUserID, ref string sUserPass)
    {
        //----- Return values. 
        //----- true and password - user exists
        //----- false - user does not exist
        //----- false and password - exception thrown

        bool bReturn = false;
        int nRows = 0;

        sUserPass = "";
        
        try
        {
            //----- Make sure connection is established and open.
            if (m_sConnState == ConnectionState.Open.ToString())
            {
                //----- Check if SQL.
                if (IsDbSql() && m_sqlConn != null)
                {
                    System.Data.SqlClient.SqlCommand sqlCommand = null;
                    sqlCommand = m_sqlConn.CreateCommand();
                    sqlCommand.CommandText = String.Format(QUERY_ALL_USERNAME, sUserID);
                    nRows = Convert.ToInt32(sqlCommand.ExecuteScalar().ToString());

                    if (sqlCommand != null)
                        sqlCommand = null;
                }

                //if (IsDbOracle() && m_oracleConn != null)
                //{
                //    System.Data.OracleClient.OracleCommand oracleCommand = null;
                //    oracleCommand = m_oracleConn.CreateCommand();
                //    oracleCommand.CommandText = String.Format(QUERY_ALL_USERNAME, sUserID);
                //    nRows = Convert.ToInt32(oracleCommand.ExecuteOracleScalar().ToString());

                //    if (oracleCommand != null)
                //        oracleCommand = null;
                //}

                if (nRows == 1)
                {
                    //----- Check if SQL.
                    if (IsDbSql() && m_sqlConn != null)
                    {
                        System.Data.SqlClient.SqlCommand sqlCommand = null;
                        sqlCommand = m_sqlConn.CreateCommand();
                        sqlCommand.CommandText = String.Format(QUERY_ALL_PWRD, sUserID);
                        sUserPass = sqlCommand.ExecuteScalar().ToString();

                        if (sqlCommand != null)
                            sqlCommand = null;
                    }

                    //if (IsDbOracle() && m_oracleConn != null)
                    //{
                    //    System.Data.OracleClient.OracleCommand oracleCommand = null;
                    //    oracleCommand = m_oracleConn.CreateCommand();
                    //    oracleCommand.CommandText = String.Format(QUERY_ALL_PWRD, sUserID);
                    //    sUserPass = oracleCommand.ExecuteOracleScalar().ToString();

                    //    if (oracleCommand != null)
                    //        oracleCommand = null;
                    //}

                    bReturn = true;
                }
                else
                {
                    bReturn = false;
                }
            }
        }
        catch (Exception exThrown)
        {
            m_sLastError = sUserPass = exThrown.Message;
            bReturn = false;
        }

        return bReturn;
    }

    public bool GetLicenseKey(ref string sLicKey)
    {
        bool bReturn = false;

        try
        {
            //----- Make sure connection is established and open.
            if (m_sConnState == ConnectionState.Open.ToString())
            {
                //----- Check if SQL.
                if (IsDbSql() && m_sqlConn != null)
                {
                    System.Data.SqlClient.SqlCommand sqlCommand = null;
                    sqlCommand = m_sqlConn.CreateCommand();
                    sqlCommand.CommandText = QUERY_ALL_GETLIC;
                    sLicKey = sqlCommand.ExecuteScalar().ToString();

                    if (sqlCommand != null)
                        sqlCommand = null;
                }

                //if (IsDbOracle() && m_oracleConn != null)
                //{
                //    System.Data.OracleClient.OracleCommand oracleCommand = null;
                //    oracleCommand = m_oracleConn.CreateCommand();
                //    oracleCommand.CommandText = QUERY_ALL_GETLIC;
                //    sLicKey = oracleCommand.ExecuteOracleScalar().ToString();

                //    if (oracleCommand != null)
                //        oracleCommand = null;
                //}

                if (sLicKey != "")
                    bReturn = true;
                else
                    m_sLastError = "No data in settings table.";
            }
        }
        catch (Exception exThrown)
        {
            m_sLastError = exThrown.Message;
            bReturn = false;
        }

        return bReturn;
    }

    public int GetADAuth()
    {
        int iReturn = 0;
        string sAuth = "";
        try
        {
            //----- Make sure connection is established and open.
            if (m_sConnState == ConnectionState.Open.ToString())
            {
                //----- Check if SQL.
                if (IsDbSql() && m_sqlConn != null)
                {
                    System.Data.SqlClient.SqlCommand sqlCommand = null;
                    sqlCommand = m_sqlConn.CreateCommand();
                    sqlCommand.CommandText = QUERY_AD_AUTH;
                    sAuth = sqlCommand.ExecuteScalar().ToString();

                    if (sqlCommand != null)
                        sqlCommand = null;
                }

                //if (IsDbOracle() && m_oracleConn != null)
                //{
                //    System.Data.OracleClient.OracleCommand oracleCommand = null;
                //    oracleCommand = m_oracleConn.CreateCommand();
                //    oracleCommand.CommandText = QUERY_AD_AUTH;
                //    sAuth = oracleCommand.ExecuteOracleScalar().ToString();

                //    if (oracleCommand != null)
                //        oracleCommand = null;
                //}

                if (sAuth != "")
                    iReturn = Convert.ToInt32(sAuth);
                else
                    m_sLastError = "No data in settings table.";
            }
        }
        catch (Exception exThrown)
        {
            m_sLastError = exThrown.Message;
            // iReturn = 0;
        }

        return iReturn;
    }

    public bool GetADInt()
    {
        bool bReturn = false;
        string sInt = "";
        try
        {
            //----- Make sure connection is established and open.
            if (m_sConnState == ConnectionState.Open.ToString())
            {
                //----- Check if SQL.
                if (IsDbSql() && m_sqlConn != null)
                {
                    System.Data.SqlClient.SqlCommand sqlCommand = null;
                    sqlCommand = m_sqlConn.CreateCommand();
                    sqlCommand.CommandText = QUERY_AD_INT;
                    sInt = sqlCommand.ExecuteScalar().ToString();

                    if (sqlCommand != null)
                        sqlCommand = null;
                }

                //if (IsDbOracle() && m_oracleConn != null)
                //{
                //    System.Data.OracleClient.OracleCommand oracleCommand = null;
                //    oracleCommand = m_oracleConn.CreateCommand();
                //    oracleCommand.CommandText = QUERY_AD_INT;
                //    sInt = oracleCommand.ExecuteOracleScalar().ToString();

                //    if (oracleCommand != null)
                //        oracleCommand = null;
                //}

                if (sInt != "")
                    bReturn = Convert.ToBoolean(Convert.ToInt32(sInt));
                else
                    m_sLastError = "No data in settings table.";
            }
        }
        catch (Exception exThrown)
        {
            m_sLastError = exThrown.Message;
            // bReturn = false;
        }

        return bReturn;
    }

    public bool GetADAutoUser()
    {
        bool bReturn = false;
        string sAutoUser = "";
        try
        {
            //----- Make sure connection is established and open.
            if (m_sConnState == ConnectionState.Open.ToString())
            {
                //----- Check if SQL.
                if (IsDbSql() && m_sqlConn != null)
                {
                    System.Data.SqlClient.SqlCommand sqlCommand = null;
                    sqlCommand = m_sqlConn.CreateCommand();
                    sqlCommand.CommandText = QUERY_AD_AUTOUSER;
                    sAutoUser = sqlCommand.ExecuteScalar().ToString();

                    if (sqlCommand != null)
                        sqlCommand = null;
                }

                //if (IsDbOracle() && m_oracleConn != null)
                //{
                //    System.Data.OracleClient.OracleCommand oracleCommand = null;
                //    oracleCommand = m_oracleConn.CreateCommand();
                //    oracleCommand.CommandText = QUERY_AD_AUTOUSER;
                //    sAutoUser = oracleCommand.ExecuteOracleScalar().ToString();

                //    if (oracleCommand != null)
                //        oracleCommand = null;
                //}

                if (sAutoUser != "")
                    bReturn = Convert.ToBoolean(Convert.ToInt32(sAutoUser));
                else
                    m_sLastError = "No data in settings table.";
            }
        }
        catch (Exception exThrown)
        {
            m_sLastError = exThrown.Message;
            // bReturn = false;
        }

        return bReturn;
    }

    public bool GetADAutoUsertoGroup()
    {
        bool bReturn = false;
        string sAutoUsertoGroup = "";
        try
        {
            //----- Make sure connection is established and open.
            if (m_sConnState == ConnectionState.Open.ToString())
            {
                //----- Check if SQL.
                if (IsDbSql() && m_sqlConn != null)
                {
                    System.Data.SqlClient.SqlCommand sqlCommand = null;
                    sqlCommand = m_sqlConn.CreateCommand();
                    sqlCommand.CommandText = QUERY_AD_AUTOUSERTOGROUP;
                    sAutoUsertoGroup = sqlCommand.ExecuteScalar().ToString();

                    if (sqlCommand != null)
                        sqlCommand = null;
                }

                //if (IsDbOracle() && m_oracleConn != null)
                //{
                //    System.Data.OracleClient.OracleCommand oracleCommand = null;
                //    oracleCommand = m_oracleConn.CreateCommand();
                //    oracleCommand.CommandText = QUERY_AD_AUTOUSERTOGROUP;
                //    sAutoUsertoGroup = oracleCommand.ExecuteOracleScalar().ToString();

                //    if (oracleCommand != null)
                //        oracleCommand = null;
                //}

                if (sAutoUsertoGroup != "")
                    bReturn = Convert.ToBoolean(Convert.ToInt32(sAutoUsertoGroup));
                else
                    m_sLastError = "No data in settings table.";
            }
        }
        catch (Exception exThrown)
        {
            m_sLastError = exThrown.Message;
            // bReturn = false;
        }

        return bReturn;
    }

    public string GetADDomain()
    {
        string sReturn = "";
        string sDomain = "";
        try
        {
            //----- Make sure connection is established and open.
            if (m_sConnState == ConnectionState.Open.ToString())
            {
                //----- Check if SQL.
                if (IsDbSql() && m_sqlConn != null)
                {
                    System.Data.SqlClient.SqlCommand sqlCommand = null;
                    sqlCommand = m_sqlConn.CreateCommand();
                    sqlCommand.CommandText = QUERY_AD_DOMAIN;
                    sDomain = sqlCommand.ExecuteScalar().ToString();

                    if (sqlCommand != null)
                        sqlCommand = null;
                }

                //if (IsDbOracle() && m_oracleConn != null)
                //{
                //    System.Data.OracleClient.OracleCommand oracleCommand = null;
                //    oracleCommand = m_oracleConn.CreateCommand();
                //    oracleCommand.CommandText = QUERY_AD_DOMAIN;
                //    sDomain = oracleCommand.ExecuteOracleScalar().ToString();

                //    if (oracleCommand != null)
                //        oracleCommand = null;
                //}

                if (sDomain != "")
                    sReturn = sDomain;
                else
                    m_sLastError = "No data in settings table.";
            }
        }
        catch (Exception exThrown)
        {
            m_sLastError = exThrown.Message;
            // iReturn = 0;
        }

        return sReturn;
    }

    public ADDomain[] GetADDomains()
    {
        System.Collections.ArrayList alTemp = new System.Collections.ArrayList();
        ADDomain[] aReturn = (ADDomain[])Array.CreateInstance(typeof(ADDomain), 0);
        System.Data.IDataReader drReader = null;
        try
        {
            //----- Make sure connection is established and open.
            if (m_sConnState == ConnectionState.Open.ToString())
            {
                //----- Check if SQL.
                if (IsDbSql() && m_sqlConn != null)
                {
                    System.Data.SqlClient.SqlCommand sqlCommand = null;
                    sqlCommand = m_sqlConn.CreateCommand();
                    sqlCommand.CommandText = QUERY_AD_DOMAINS;
                    drReader = sqlCommand.ExecuteReader();

                    while (drReader.Read())
                    {
                        ADDomain dTemp = new ADDomain();
                        dTemp.ID = Convert.ToInt32(drReader["ad_domain_tbl_id"]);
                        dTemp.DomainShortName = Convert.ToString(drReader["domain_short_name"]);
                        dTemp.DomainPath = Convert.ToString(drReader["domain_path"]);
                        dTemp.DomainDNS = Convert.ToString(drReader["domain_dns"]);
                        dTemp.NetBIOS = Convert.ToString(drReader["netbios"]);
                        dTemp.Username = Convert.ToString(drReader["username"]);
                        dTemp.Password = Convert.ToString(drReader["password"]);
                        dTemp.ServerIP = Convert.ToString(drReader["server_ip"]);
                        alTemp.Add(dTemp);
                    }

                    if (sqlCommand != null)
                        sqlCommand = null;
                }

                //if (IsDbOracle() && m_oracleConn != null)
                //{
                //    System.Data.OracleClient.OracleCommand oracleCommand = null;
                //    oracleCommand = m_oracleConn.CreateCommand();
                //    oracleCommand.CommandText = QUERY_AD_DOMAINS;
                //    drReader = oracleCommand.ExecuteReader();

                //    while (drReader.Read())
                //    {
                //        ADDomain dTemp = new ADDomain();
                //        dTemp.ID = Convert.ToInt32(drReader["ad_domain_tbl_id"]);
                //        dTemp.DomainShortName = Convert.ToString(drReader["domain_short_name"]);
                //        dTemp.DomainPath = Convert.ToString(drReader["domain_path"]);
                //        dTemp.DomainDNS = Convert.ToString(drReader["domain_dns"]);
                //        dTemp.NetBIOS = Convert.ToString(drReader["netbios"]);
                //        dTemp.Username = Convert.ToString(drReader["username"]);
                //        dTemp.Password = Convert.ToString(drReader["password"]);
                //        dTemp.ServerIP = Convert.ToString(drReader["server_ip"]);
                //        alTemp.Add(dTemp);
                //    }

                //    if (oracleCommand != null)
                //        oracleCommand = null;
                //}
                
                if (alTemp.Count > 0)
                {
                    aReturn = (ADDomain[])alTemp.ToArray(typeof(ADDomain));
                }
            }
        }
        catch (Exception exThrown)
        {
            m_sLastError = exThrown.Message;
        }
        finally
        {
            alTemp = null;
            if (drReader != null)
            {
                drReader.Close();
                drReader = null;
            }
        }

        return aReturn;
    }

    //----- Cleanup objects and close connections upon deletion of EkDb object from the user.
    public void Dispose()
    {
        if (m_sqlConn != null)
        {
            m_sqlConn.Close();
            m_sqlConn.Dispose();
        }

        //if (m_oracleConn != null)
        //{
        //    m_oracleConn.Close();
        //    m_oracleConn.Dispose();
        //}

        m_cssEkDb = null;
    }
#endregion
}
