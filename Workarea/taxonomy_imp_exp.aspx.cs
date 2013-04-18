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
using System.IO;
using System.Xml;
using System.Xml.Serialization;
using Ektron.Cms.Common;

public partial class taxonomy_imp_exp : System.Web.UI.Page
{
    protected Ektron.Cms.Common.EkMessageHelper m_refMsg;
    protected StyleHelper m_refStyle = new StyleHelper();
    protected string m_strPageAction = "";
    protected string AppImgPath = "";
    protected CommonApi m_refCommon = new CommonApi();
    protected int EnableMultilingual = 0;
    protected Ektron.Cms.Content.EkContent m_refContent;
    protected long m_intTaxonomyId = 0;
    protected int m_intTaxonomyLanguage = 1033;
    protected int TaxonomyLanguage = -1;
    protected bool AddDeleteIcon = false;
    protected long CurrentUserId = 0;
    protected void Page_Load(object sender, System.EventArgs e)
    {
        try
        {
            CurrentUserId = m_refCommon.RequestInformationRef.UserId;
            m_refMsg = m_refCommon.EkMsgRef;
			Utilities.ValidateUserLogin();
            if (m_refCommon.RequestInformationRef.IsMembershipUser > 0 || CurrentUserId == 0)
            {
                Response.Redirect(m_refCommon.ApplicationPath + "reterror.aspx?info=" + Server.UrlEncode(m_refMsg.GetMessage("msg login cms user")), false);
                return;
            }
            else
            {
                AppImgPath = (string)m_refCommon.AppImgPath;
                EnableMultilingual = System.Convert.ToInt32(m_refCommon.EnableMultilingual);
                displaystylesheet.Text = m_refStyle.GetClientScript();
                if (!String.IsNullOrEmpty(Request.QueryString["action"]))
                {
                    m_strPageAction = Request.QueryString["action"].ToLower();
                }
                Utilities.SetLanguage(m_refCommon);
                m_refContent = m_refCommon.EkContentRef;
                TaxonomyLanguage = System.Convert.ToInt32(m_refCommon.ContentLanguage);

                RegisterResources();
                SetJsServerVariables();

                TaxonomyRequest taxonomy_request_data = null;
                if (Page.IsPostBack)
                {
                    string xml = "";
                    string title = "";
                    title = (string)txttitle.Text;
                    if (textimport.Text.Trim().Length > 0)
                    {
                        xml = (string)textimport.Text;
                    }
                    else
                    {
                        System.IO.Stream stream = fileimport.FileContent;
                        stream.Flush();
                        stream.Position = 0;
                        int FileLen = fileimport.PostedFile.ContentLength;
                        byte[] byteArr = new byte[FileLen + 1]; //= stream.ToArray
                        stream.Read(byteArr, 0, FileLen);
                        xml = System.Convert.ToBase64String(byteArr, 0, byteArr.Length);
                        UTF8Encoding utf8 = new UTF8Encoding();
                        xml = utf8.GetString(Convert.FromBase64String(xml));
                    }
                    if (xml.Trim().Length > 0 && title.Trim().Length > 0)
                    {
                        if (xml.IndexOf("<ArrayOfTaxonomyData ") != -1)
                        {
                            m_intTaxonomyId = m_refContent.ImportTaxonomyCollection(xml.Trim(), title);
                        }
                        else
                        {
                            m_intTaxonomyId = m_refContent.ImportTaxonomy(xml.Trim(), title);
                        }
                        string strConfig = string.Empty;
                        if (!string.IsNullOrEmpty(Request.Form[chkConfigContent.UniqueID]))
                        {
                            strConfig = "0";
                        }
                        if (!string.IsNullOrEmpty(Request.Form[chkConfigUser.UniqueID]))
                        {
                            if (string.IsNullOrEmpty(strConfig))
                            {
                                strConfig = "1";
                            }
                            else
                            {
                                strConfig = strConfig + ",1";
                            }
                        }
                        if (!string.IsNullOrEmpty(Request.Form[chkConfigGroup.UniqueID]))
                        {
                            if (string.IsNullOrEmpty(strConfig))
                            {
                                strConfig = "2";
                            }
                            else
                            {
                                strConfig = strConfig + ",2";
                            }
                        }
                        if (!(string.IsNullOrEmpty(strConfig)))
                        {
                            m_refContent.UpdateTaxonomyConfig(m_intTaxonomyId, strConfig);
                        }
                    }
                    if (m_intTaxonomyId > 0)
                    {
                        Response.Redirect("taxonomy.aspx?reloadtrees=tax", false);
                    }
                    else
                    {
                        textimport.Text = xml;
                        textimport.Attributes.Add("onkeypress", "ClearErr();");
                        textimport.Focus();
                    }
                }
                else
                {
                    if (m_strPageAction == "export")
                    {
                        Response.Clear();
                        Response.Charset = "";
                        Response.ContentType = "text/xml";
                        if (!String.IsNullOrEmpty(Request.QueryString["taxonomyid"]))
                        {
                            m_intTaxonomyId = Convert.ToInt64(Request.QueryString["taxonomyid"]);
                        }
                        TaxonomyDataCollection col = new TaxonomyDataCollection();
                        LanguageData[] langList = (new SiteAPI()).GetAllActiveLanguages();
                        foreach (LanguageData langitem in langList)
                        {
                            taxonomy_request_data = new TaxonomyRequest();
                            taxonomy_request_data.TaxonomyId = m_intTaxonomyId;
                            taxonomy_request_data.TaxonomyLanguage = langitem.Id;
                            taxonomy_request_data.Depth = -1;
                            taxonomy_request_data.IncludeItems = false;
                            taxonomy_request_data.ReadCount = false;
                            TaxonomyData data = m_refContent.LoadTaxonomy(ref taxonomy_request_data);
                            if (data != null)
                            {
                                col.Add(data);
                            }
                        }

                        string returnXml = (string)(EkXml.Serialize(typeof(TaxonomyDataCollection), col));
                        XmlDocument doc;
                        try
                        {
                            doc = new XmlDocument();
                            doc.LoadXml(returnXml);
                            string[] removablenode = new string[] { "TaxonomyParentId", "TaxonomyLevel", "TaxonomyPath", "TaxonomyCreatedDate", "TaxonomyItemCount", "TaxonomyHasChildren", "TemplateId", "TemplateName", "TemplateInherited", "TemplateInheritedFrom", "TaxonomyType", "Visible", "TaxonomyImage", "TaxonomyItems", "CategoryUrl", "FolderId" };
                            XmlNodeList nodelist = doc.GetElementsByTagName("TaxonomyData");
                            for (int i = 0; i <= nodelist.Count - 1; i++)
                            {
                                XmlNode node = nodelist[i];
                                for (int j = 0; j <= removablenode.Length - 1; j++)
                                {
                                    XmlNode n = node.SelectSingleNode(removablenode[j]);
                                    try
                                    {
                                        n.ParentNode.RemoveChild(n);
                                    }
                                    catch (Exception)
                                    {
                                    }
                                }
                            }
                            returnXml = doc.InnerXml;
                        }
                        catch (Exception)
                        {
                        }
                        Response.Write(returnXml);
                        Response.AddHeader("content-disposition", "attachment; filename=taxonomy_" + m_intTaxonomyId + ".xml");
                    }
                    ViewImportToolBar();
                }
            }
        }
        catch (Exception ex)
        {
            Response.Redirect((string)("reterror.aspx?info=" + EkFunctions.UrlEncode(ex.Message + ".") + "&LangType=" + TaxonomyLanguage), false);
        }
    }
    private void ViewImportToolBar()
    {
        divTitleBar.InnerHtml = m_refStyle.GetTitleBar((string)(m_refMsg.GetMessage("import taxonomy title")));
        System.Text.StringBuilder result = new System.Text.StringBuilder();
        result.Append("<table><tr>" + "\r\n");
        result.Append(m_refStyle.GetButtonEventsWCaption(AppImgPath + "../UI/Icons/back.png", "taxonomy.aspx", (string)(m_refMsg.GetMessage("alt back button text")), (string)(m_refMsg.GetMessage("btn back")), "", StyleHelper.BackButtonCssClass,true));
        result.Append(m_refStyle.GetButtonEventsWCaption(AppImgPath + "../UI/Icons/save.png", "javascript:Import();", (string)(m_refMsg.GetMessage("alt import taxonomy")), (string)(m_refMsg.GetMessage("btn import taxonomy")), "", StyleHelper.SaveButtonCssClass,true));
        result.Append(StyleHelper.ActionBarDivider);
        result.Append("<td>" + m_refStyle.GetHelpButton("ImportTaxonomy", "") + "</td>");
        result.Append("</tr></table>");
        divToolBar.InnerHtml = result.ToString();
        result = null;
    }
    private void SetJsServerVariables()
    {
        ltr_titleRequired.Text = m_refMsg.GetMessage("js title is required field");
        ltr_invalidInputOrXML.Text = m_refMsg.GetMessage("js invalid input. use file or xml as your import option.");
        ltr_fileOrXMLRequired.Text = m_refMsg.GetMessage("js file or xml selection is required.");
        ltr_invalidExtension.Text = m_refMsg.GetMessage("js invalid file specified.  please import a file with .xml extension.");
        ltr_enterValidFilePath.Text = m_refMsg.GetMessage("js please enter the valid file path.");
    }
    private void RegisterResources()
    {
        Ektron.Cms.API.JS.RegisterJS(this, m_refCommon.ApplicationPath + "java/jfunct.js", "EktronJFunctJS");
        Ektron.Cms.API.JS.RegisterJS(this, m_refCommon.ApplicationPath + "java/toolbar_roll.js", "EktronJToolbarRollJS");
        Ektron.Cms.API.JS.RegisterJS(this, m_refCommon.ApplicationPath + "java/workareahelper.js", "EktronWorkareaHelperJS");
    }
}