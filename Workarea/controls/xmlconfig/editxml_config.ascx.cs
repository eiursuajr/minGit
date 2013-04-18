using System.Web.UI.WebControls;
using System.Configuration;
using System.Collections;
using System.Linq;
using System.Data;
using System.Web.Caching;
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
using Microsoft.Security.Application;

public partial class editxml_config : System.Web.UI.UserControl
{


    protected StyleHelper m_refStyle = new StyleHelper();
    protected EkMessageHelper m_refMsg;
    protected string PageAction = "";
    protected string AppImgPath = "";
    protected string AppPath = "";
    protected int EnableMultilingual = 0;
    protected int ContentLanguage = 0;
    protected ContentAPI _ContentApi = new ContentAPI();
    protected System.Web.UI.WebControls.DataGrid XMLList;
    protected string m_strOrderBy = "title";
    protected long ConfigId = 0;
    protected XmlConfigData cXmlCollection;
    protected long m_intId = 0;
    protected string pkDisplay;
    protected bool bDefaultXsltExists = false;
    protected string XmlPath = "";
    protected string Xslt1Checked = "";
    protected string m_strTitle;

    private void Page_Load(System.Object sender, System.EventArgs e)
    {
        //Put user code to initialize the page here
        RegisterResources();
        m_refMsg = _ContentApi.EkMsgRef;
        if (!(Request.QueryString["action"] == null))
        {
            if (Request.QueryString["action"] != "")
            {
                PageAction = Request.QueryString["action"].ToLower();
            }
        }
        AppImgPath = _ContentApi.AppImgPath;
        AppPath = _ContentApi.AppPath;
        XmlPath = _ContentApi.XmlPath;
    }

    #region XmlConfigs
    public bool EditXmlConfig()
    {
        if (!(Request.QueryString["id"] == null))
        {
            if (Request.QueryString["id"] != "")
            {
                m_intId = Convert.ToInt64(Request.QueryString["id"]);
            }
        }
        if (!(Page.IsPostBack))
        {
            if (PageAction.ToString().ToLower() == "newinheritconfiguration")
            {
                Display_NewInheritConfiguration();
            }
            else
            {
                Display_EditXmlConfig();
            }
            return (false);
        }
        else
        {
            cXmlCollection = new XmlConfigData();
            if (PageAction == "newinheritconfiguration")
            {
                Process_ReplicateXmlConfig();
                return (true);
            }
            else
            {
                return (Process_UpdateXmlConfig());
            }
        }
    }
    public long AddXmlConfig()
    {
        long retval = 0;
        if (!(Page.IsPostBack))
        {
            Display_AddXmlConfig();
        }
        else
        {
            retval = Process_AddXmlConfig();
        }
        return retval;
    }
    private void Display_AddXmlConfig()
    {
        AddXmlConfigToolBar();
    }
    private void Display_EditXmlConfig()
    {
        cXmlCollection = _ContentApi.GetXmlConfiguration(m_intId);
        if (cXmlCollection == null)
        {
            cXmlCollection = new XmlConfigData();
        }
        EditXmlConfigToolBar();
        m_strTitle = cXmlCollection.Title;
        pkDisplay = cXmlCollection.PackageDisplayXslt;
    }
    private void Display_NewInheritConfiguration()
    {
        InheritXmlConfigToolBar();
    }
    private long Process_AddXmlConfig()
    {
        Collection cXml = new Collection();
        long retVal = 0;
        try
        {
            if ((Request.Form["frm_xmltitle"] == null) || Request.Form["frm_xmltitle"].Length == 0)
            {
                ShowAddXmlError(_ContentApi.EkMsgRef.GetMessage("js: alert title required"));
                return (retVal);
            }

            string title = Request.Form["frm_xmltitle"];

            // check if title already exists:
            XmlConfigData[] xmlConfig = _ContentApi.GetAllXmlConfigurations("title");
            if (xmlConfig != null && xmlConfig.Length > 0)
            {
                foreach (XmlConfigData xmlConfigItem in xmlConfig)
                {
                    if (xmlConfigItem.Title.ToLower() == title.ToLower())
                    {
                        ShowAddXmlError(_ContentApi.EkMsgRef.GetMessage("lbl smart form unique title required"));
                        return (retVal);
                    }
                }
            }

            cXml.Add(EkEnumeration.XmlConfigType.Content, "Type", null, null);
            cXml.Add(title, "CollectionTitle", null, null);
            cXml.Add(EkFunctions.HtmlEncode(Request.Form["frm_xmldescription"]), "CollectionDescription", null, null);
            cXml.Add(Request.Form["frm_editxslt"], "EditXslt", null, null);
            cXml.Add(Request.Form["frm_savexslt"], "SaveXslt", null, null);
            cXml.Add(ValidateXSLT(Request.Form["frm_Xslt1"]), "Xslt1", null, null);
            cXml.Add(ValidateXSLT(Request.Form["frm_Xslt2"]), "Xslt2", null, null);
            cXml.Add(ValidateXSLT(Request.Form["frm_Xslt3"]), "Xslt3", null, null);
            cXml.Add(ValidateXSLT(Request.Form["frm_Xslt4"]), "Xslt4", null, null);
            cXml.Add(ValidateXSLT(Request.Form["frm_Xslt5"]), "Xslt5", null, null);
            cXml.Add(Request.Form["frm_xmlschema"], "XmlSchema", null, null);
            cXml.Add(Request.Form["frm_xmlnamespace"], "XmlNameSpace", null, null);
            cXml.Add(Request.Form["frm_xmladvconfig"], "XmlAdvConfig", null, null);
            cXml.Add(Request.Form["frm_xsltdefault"], "DefaultXslt", null, null);
            cXml.Add(Server.MapPath(XmlPath), "PhysicalPath", null, null);
            retVal = _ContentApi.AddXmlConfiguration(cXml);
            if (retVal == -1)
            {
                ShowAddXmlError(_ContentApi.EkMsgRef.GetMessage("lbl smart form unique title required"));
            }
        }
        catch (Exception ex)
        {
            Utilities.ShowError(ex.Message.ToString());
        }
        return (retVal);
    }
    private void ShowAddXmlError(string msg)
    {
        Display_AddXmlConfig();
        lbl_addXmlError.Visible = true;
        lbl_addXmlError.Text = "<tr><td colspan=\"2\">" + msg + "</td></tr>";
    }
    private void Process_ReplicateXmlConfig()
    {
        string newTitle;
        long iRet;
        try
        {
            newTitle = Request.Form["frm_xmltitle"];
            iRet = _ContentApi.ReplicateXmlConfiguration(m_intId, newTitle);
            if (iRet > 0)
            {
                Response.Redirect((string)("xml_index_select.aspx?action=newinheritconfiguration&xml_collection_id=" + iRet), false);
            }
        }
        catch (Exception ex)
        {
            Utilities.ShowError(ex.Message.ToString());
        }
    }
    private bool Process_UpdateXmlConfig()
    {
        Collection cXml = new Collection();
        try
        {
            cXml.Add(Request.Form["frm_collectionid"], "CollectionID", null, null);
            cXml.Add(Request.Form["frm_xmltitle"], "CollectionTitle", null, null);
            cXml.Add(EkFunctions.HtmlEncode(Request.Form["frm_xmldescription"]), "CollectionDescription", null, null);
            cXml.Add(Request.Form["frm_editxslt"], "EditXslt", null, null);
            cXml.Add(Request.Form["frm_savexslt"], "SaveXslt", null, null);
            cXml.Add(ValidateXSLT(Request.Form["frm_Xslt1"]), "Xslt1", null, null);
            cXml.Add(ValidateXSLT(Request.Form["frm_Xslt2"]), "Xslt2", null, null);
            cXml.Add(ValidateXSLT(Request.Form["frm_Xslt3"]), "Xslt3", null, null);
            cXml.Add(ValidateXSLT(Request.Form["frm_Xslt4"]), "Xslt4", null, null);
            cXml.Add(ValidateXSLT(Request.Form["frm_Xslt5"]), "Xslt5", null, null);
            cXml.Add(Request.Form["frm_xmlschema"], "XmlSchema", null, null);
            cXml.Add(Request.Form["frm_xmlnamespace"], "XmlNameSpace", null, null);
            cXml.Add(Request.Form["frm_xmladvconfig"], "XmlAdvConfig", null, null);
            cXml.Add(Request.Form["frm_xsltdefault"], "DefaultXslt", null, null);
            cXml.Add(Server.MapPath(XmlPath), "PhysicalPath", null, null);
            _ContentApi.UpdateXmlConfiguration(cXml);
        }
        catch (Exception ex)
        {
            Utilities.ShowError(ex.Message.ToString());
        }
        return (true);
    }
    private void AddXmlConfigToolBar()
    {
        System.Text.StringBuilder result = new System.Text.StringBuilder();
        result.Append("<table><tr>");
        txtTitleBar.InnerHtml = m_refStyle.GetTitleBar(m_refMsg.GetMessage("add xml config msg"));
		result.Append(m_refStyle.GetButtonEventsWCaption(AppPath + "images/UI/Icons/back.png", "xml_config.aspx?page=xml_config.aspx&action=ViewAllXmlConfigurations", m_refMsg.GetMessage("alt back button text"), m_refMsg.GetMessage("btn back"), "", StyleHelper.BackButtonCssClass, true));
		result.Append(m_refStyle.GetButtonEventsWCaption(AppPath + "images/UI/Icons/save.png", "#", m_refMsg.GetMessage("lbl Select to continue"), m_refMsg.GetMessage("btn save"), "onclick=\"return SubmitForm(\'xmlconfiguration\', \'VerifyXmlForm()\');\"", StyleHelper.SaveButtonCssClass, true));
        result.Append(StyleHelper.ActionBarDivider);
		result.Append("<td>");
        result.Append(m_refStyle.GetHelpButton(PageAction, ""));
        result.Append("</td>");
        result.Append("</tr></table>");
        htmToolBar.InnerHtml = result.ToString();
        result = null;
    }
    private string ValidateXSLT(object GivenXslt)
    {
        string strReturn = "";
        try
        {
            strReturn = Convert.ToString(GivenXslt);
            //
            //#21753 - custom xsl getting errors when used with a smart form.
            //The error appears to be overriding the path to use the XSL with a ..\\file.xsl instead of ..\file.xsl.
            //
            //If (strReturn.Trim <> "" AndAlso strReturn.ToLower.IndexOf("http") = -1) Then
            //    If ((Left(strReturn, 1) <> "/") And (Left(strReturn, 1) <> "\")) Then
            //        strReturn = "/" & strReturn
            //    End If
            //End If
        }
        catch (Exception)
        {
            strReturn = "";
        }
        return (strReturn);
    }
    private void EditXmlConfigToolBar()
    {
        System.Text.StringBuilder result = new System.Text.StringBuilder();
        result.Append("<table><tr>");
        txtTitleBar.InnerHtml = m_refStyle.GetTitleBar((string)(m_refMsg.GetMessage("edit xml config msg") + " \"" + cXmlCollection.Title + "\""));
		result.Append(m_refStyle.GetButtonEventsWCaption(AppPath + "images/UI/Icons/back.png", "xml_config.aspx?action=ViewXmlConfiguration&id=" + m_intId + "", m_refMsg.GetMessage("alt back button text"), m_refMsg.GetMessage("btn back"), "", StyleHelper.BackButtonCssClass, true));
		result.Append(m_refStyle.GetButtonEventsWCaption(AppPath + "images/UI/Icons/save.png", "#", m_refMsg.GetMessage("alt update button text (xml config)"), m_refMsg.GetMessage("btn update"), "onclick=\"return SubmitForm(\'xmlconfiguration\', \'VerifyXmlForm()\');\"", StyleHelper.SaveButtonCssClass, true));
        result.Append(StyleHelper.ActionBarDivider);
		result.Append("<td>");
        result.Append(m_refStyle.GetHelpButton(PageAction, ""));
        result.Append("</td>");
        result.Append("</tr></table>");
        htmToolBar.InnerHtml = result.ToString();
        result = null;
    }
    private void InheritXmlConfigToolBar()
    {
        System.Text.StringBuilder result = new System.Text.StringBuilder();
        result.Append("<table><tr>");
        txtTitleBar.InnerHtml = m_refStyle.GetTitleBar(m_refMsg.GetMessage("add xml config msg"));
        //result.Append(m_refStyle.GetButtonEventsWCaption(AppPath & "images/UI/Icons/save.png", "#", m_refMsg.GetMessage("alt update button text (xml config)"), m_refMsg.GetMessage("btn update"), "Onclick=""javascript:return SubmitForm('xmlconfiguration', 'VerifyXmlForm()');"""))
		result.Append(m_refStyle.GetButtonEventsWCaption(AppPath + "images/UI/Icons/back.png", (string)("xml_config.aspx?action=ViewXmlConfiguration&id=" + m_intId), m_refMsg.GetMessage("alt back button text"), m_refMsg.GetMessage("btn back"), "", StyleHelper.BackButtonCssClass, true));
		result.Append(m_refStyle.GetButtonEventsWCaption(AppPath + "images/UI/Icons/save.png", "#", m_refMsg.GetMessage("lbl Select to continue"), m_refMsg.GetMessage("btn add xml"), "onclick=\"return SubmitForm(\'xmlconfiguration\', \'VerifyXmlForm()\');\"", StyleHelper.SaveButtonCssClass, true));
        result.Append(StyleHelper.ActionBarDivider);
		result.Append("<td>");
        result.Append(m_refStyle.GetHelpButton(PageAction, ""));
        result.Append("</td>");
        result.Append("</tr></table>");
        htmToolBar.InnerHtml = result.ToString();
        result = null;
    }
    #endregion

    #region Product Types
    string m_sProductTypePage = "producttypes.aspx";

    public bool EditProductType()
    {
        if (!(Request.QueryString["id"] == null))
        {
            if (Request.QueryString["id"] != "")
            {
                m_intId = Convert.ToInt64(Request.QueryString["id"]);
            }
        }
        if (!(Page.IsPostBack))
        {
            if (PageAction.ToString().ToLower() == "newinheritconfiguration")
            {
                // Display_NewInheritProductType()
            }
            else
            {
                Display_EditProductType();
            }
            return (false);
        }
        else
        {
            cXmlCollection = new XmlConfigData();
            if (PageAction == "newinheritconfiguration")
            {
                // Process_ReplicateXmlConfig()
                return (true);
            }
            else
            {
                return (Process_UpdateProductType());
            }
        }
    }
    private void Display_EditProductType()
    {
        cXmlCollection = _ContentApi.GetXmlConfiguration(m_intId);
        if (cXmlCollection == null)
        {
            cXmlCollection = new XmlConfigData();
        }
        lbl_desc.Text = m_refMsg.GetMessage("generic description");
        // m_strTitle = pProductType.Title
        // pkDisplay = pProductType.PackageDisplayXslt
    }
    private void Display_NewInheritProductType()
    {
        InheritProductTypeToolBar();
    }
    private void Process_ReplicateProductType()
    {
        string newTitle;
        long iRet;
        try
        {
            newTitle = Request.Form["frm_xmltitle"];
            iRet = _ContentApi.ReplicateXmlConfiguration(m_intId, newTitle);
            if (iRet > 0)
            {
                Response.Redirect(m_sProductTypePage + "?action=ViewXmlConfiguration&id=" + iRet, false);
            }
        }
        catch (Exception ex)
        {
            Utilities.ShowError(ex.Message.ToString());
        }
    }
    private bool Process_UpdateProductType()
    {
        Collection cXml = new Collection();
        try
        {
            cXml.Add(m_intId, "CollectionID", null, null);
            cXml.Add(EkEnumeration.XmlConfigType.Product, "Type", null, null);
            // cXml.Add(Request.Form(drp_edittype.UniqueID), "SecondType")
            cXml.Add(Request.Form["frm_xmltitle"], "CollectionTitle", null, null);
            cXml.Add(EkFunctions.HtmlEncode(Request.Form["frm_xmldescription"]), "CollectionDescription", null, null);
            cXml.Add(Request.Form["frm_editxslt"], "EditXslt", null, null);
            cXml.Add(Request.Form["frm_savexslt"], "SaveXslt", null, null);
            cXml.Add(ValidateXSLT(Request.Form["frm_Xslt1"]), "Xslt1", null, null);
            cXml.Add(ValidateXSLT(Request.Form["frm_Xslt2"]), "Xslt2", null, null);
            cXml.Add(ValidateXSLT(Request.Form["frm_Xslt3"]), "Xslt3", null, null);
            cXml.Add(ValidateXSLT(Request.Form["frm_Xslt4"]), "Xslt4", null, null);
            cXml.Add(ValidateXSLT(Request.Form["frm_Xslt5"]), "Xslt5", null, null);
            cXml.Add(Request.Form["frm_xmlschema"], "XmlSchema", null, null);
            cXml.Add(Request.Form["frm_xmlnamespace"], "XmlNameSpace", null, null);
            cXml.Add(Request.Form["frm_xmladvconfig"], "XmlAdvConfig", null, null);
            cXml.Add(Request.Form["frm_xsltdefault"], "DefaultXslt", null, null);
            cXml.Add(Server.MapPath(XmlPath), "PhysicalPath", null, null);
            _ContentApi.UpdateXmlConfiguration(cXml);
        }
        catch (Exception ex)
        {
            Utilities.ShowError(ex.Message.ToString());
        }
        return (true);
    }
    private void InheritProductTypeToolBar()
    {
        //Dim result As New System.Text.StringBuilder
        //result.Append("<table><tr>")
        //txtTitleBar.InnerHtml = m_refStyle.GetTitleBar(m_refMsg.GetMessage("add xml config msg"))
        //result.Append(m_refStyle.GetButtonEventsWCaption(AppImgPath & "btn_addxml-nm.gif", "#", m_refMsg.GetMessage("lbl Select to continue"), m_refMsg.GetMessage("btn add xml"), "Onclick=""javascript:return SubmitForm('xmlconfiguration', 'VerifyXmlForm()');"""))
        //result.Append(m_refStyle.GetButtonEventsWCaption(AppImgPath & "../UI/Icons/back.png", m_sProductTypePage & "?action=viewproducttype&id=" & m_intId, m_refMsg.GetMessage("alt back button text"), m_refMsg.GetMessage("btn back"), ""))
        //result.Append("<td>")
        //result.Append(m_refStyle.GetHelpButton(PageAction))
        //result.Append("</td>")
        //result.Append("</tr></table>")
        //htmToolBar.InnerHtml = result.ToString
        //result = Nothing
    }
    #endregion
    private void RegisterResources()
    {
        Ektron.Cms.API.JS.RegisterJS(this, Ektron.Cms.API.JS.ManagedScript.EktronWorkareaHelperJS);
        Ektron.Cms.API.JS.RegisterJS(this, Ektron.Cms.API.JS.ManagedScript.EktronToolBarRollJS);
    }
}

