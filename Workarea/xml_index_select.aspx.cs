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
using Ektron.Cms.Interfaces.Context;
using Ektron.Cms.Framework.UI;

public partial class xml_index_select : System.Web.UI.Page
{
    protected ContentAPI m_refContentApi = new ContentAPI();
    protected string AppImgPath = "";
    protected StyleHelper m_refStyle = new StyleHelper();
    protected EkMessageHelper m_refMsg;
    protected long m_intId = 0;
    protected string m_strPageAction = "";
    protected int EnableMultiLanguage = -1;
    protected int ContentLanguage = -1;
    private Collection cSaved = null;
    private int m_intContentLanguage = 0;
    protected bool IsProduct = false;
    protected string sAction = "ViewXmlConfiguration";
    protected string sPage = "xml_config.aspx";
    protected string bPage = "editdesign.aspx";
    protected string bAction = "EditPackage";

    private void Page_Load(System.Object sender, System.EventArgs e)
    {
        Response.CacheControl = "no-cache";
        Response.AddHeader("Pragma", "no-cache");
        Response.Expires = -1;
        try
        {
            RegisterResources();
            if (m_refContentApi.RequestInformationRef.IsMembershipUser == 1 || m_refContentApi.RequestInformationRef.UserId == 0)
            {
                Response.Redirect("blank.htm", false);
                return;
            }
            if (Request.QueryString["action"] != null)
            {
                m_strPageAction = Request.QueryString["action"];
            }
            if (!String.IsNullOrEmpty(Request.QueryString["type"]))
            {
                IsProduct = System.Convert.ToBoolean(Request.QueryString["type"].ToLower() == "product");
                sPage = "commerce/producttypes.aspx";
                sAction = "viewproducttype";
            }
            m_refMsg = m_refContentApi.EkMsgRef;
            EnableMultiLanguage = m_refContentApi.EnableMultilingual;
            if (!String.IsNullOrEmpty(Request.QueryString["LangType"]))
            {
                m_intContentLanguage = Convert.ToInt32(Request.QueryString["LangType"]);
                m_refContentApi.SetCookieValue("LastValidLanguageID", m_intContentLanguage.ToString());
            }
            else
            {
                if (!String.IsNullOrEmpty(m_refContentApi.GetCookieValue("LastValidLanguageID")))
                {
                    m_intContentLanguage = int.Parse(m_refContentApi.GetCookieValue("LastValidLanguageID"));
                }
            }

            if (m_intContentLanguage == Ektron.Cms.Common.EkConstants.CONTENT_LANGUAGES_UNDEFINED || m_intContentLanguage == Ektron.Cms.Common.EkConstants.ALL_CONTENT_LANGUAGES)
            {
                m_intContentLanguage = m_refContentApi.DefaultContentLanguage;
            }
            if (m_intContentLanguage == Ektron.Cms.Common.EkConstants.CONTENT_LANGUAGES_UNDEFINED)
            {
                m_refContentApi.ContentLanguage = Ektron.Cms.Common.EkConstants.ALL_CONTENT_LANGUAGES;
            }
            else
            {
                m_refContentApi.ContentLanguage = m_intContentLanguage;
            }
            ContentLanguage = m_intContentLanguage;
            AppImgPath = m_refContentApi.AppImgPath;
            switch (m_strPageAction)
            {
                case "set":
                    DisplayXmlSet();
                    break;
                case "edit":
                    DisplayXmlEdit();
                    break;
                default:
                    if (Page.IsPostBack)
                    {
                        DisplayXmlSet();
                    }
                    else
                    {
                        DisplayXml();
                    }
                    break;
            }
            StyleSheetJS.Text = m_refStyle.GetClientScript();
        }
        catch (Exception ex)
        {
            Utilities.ShowError(ex.Message);
        }
    }
    private void DisplayXmlSet()
    {
        if (!(Request.QueryString["id"] == null))
        {
            m_intId = Convert.ToInt64(Request.QueryString["id"]);
        }
        SaveSettings(m_intId);
        XmlSetToolBar();
    }
    private void DisplayXmlEdit()
    {

        if (Request.QueryString["id"] != null)
        {
            m_intId = Convert.ToInt64(Request.QueryString["id"]);
        }
        try
        {
            if (!(Page.IsPostBack))
            {
                cSaved = LoadSettings(m_intId);
                TD_data.InnerHtml = DisplayXpathLabels();
            }
            else
            {
                EditXPathLabel(m_intId);
                Response.Redirect((string)("" + sPage + "?action=" + sAction + "&id=" + m_intId), false);
            }
        }
        catch (Exception)
        {
        }
        XmlEditToolBar();
    }
    private void DisplayXml()
    {
        try
        {
            m_intId = Convert.ToInt64(Request.Form["xml_collection_id"]);
            string fieldListXml = "";
            if (Request.QueryString["action"] == "newinheritconfiguration")
            {
                Int64.TryParse(Request.QueryString["xml_collection_id"], out m_intId);
                if (m_intId > 0)
                {
                    System.Xml.XmlDocument _doc = this.m_refContentApi.GetFieldListXML(m_intId);
                    if (_doc != null)
                    {
                        fieldListXml = _doc.InnerXml;
                    }
                }
            }
            else
                fieldListXml = Strings.Trim(Request.Form["datafieldlist_value"]);
            cSaved = LoadSettings(m_intId);
            string strRet;
            strRet = BuildTable(fieldListXml, (int)m_intId);
            if (strRet == "")
            {
                Response.Redirect((string)("" + sPage + "?action=" + sAction + "&id=" + m_intId), false);
            }
            else
            {
                TD_data.InnerHtml = strRet;
            }
        }
        catch (Exception ex)
        {
            TD_data.InnerHtml = ex.Message + "<br/>" + EkFunctions.HtmlEncode(Request.Form["datafieldlist_value"]);
        }
        XmlToolBar();
    }
    private void XmlSetToolBar()
		{
			System.Text.StringBuilder result = new System.Text.StringBuilder();
			divTitleBar.InnerHtml = m_refStyle.GetTitleBar(m_refMsg.GetMessage("btn edit ftsearch"));
			result.Append("<table><tr>");
			result.Append(m_refStyle.GetButtonEventsWCaption(AppImgPath + "../UI/Icons/back.png", bPage + "?id=" + m_intId + "&action=" + bAction, m_refMsg.GetMessage("alt back button text"), m_refMsg.GetMessage("btn back"), "OnClick=\"javascript:return SetAction(\'cancel\');\"", StyleHelper.BackButtonCssClass, true));
			result.Append(m_refStyle.GetButtonEventsWCaption(AppImgPath + "../UI/Icons/save.png", (string)("xml_index_select.aspx" + (IsProduct ? "?type=product" : "")), "Save Index Selection", m_refMsg.GetMessage("btn update"), "OnClick=\"javascript:return SetAction(\'update\');\"", StyleHelper.SaveButtonCssClass, true));
			result.Append(StyleHelper.ActionBarDivider);
			result.Append("<td>" + m_refStyle.GetHelpButton("indexxmlsetTextSearch", "") + "</td>");

			result.Append("</tr></table>");
			SetFormAction((string) ("xml_index_select.aspx?id=" + this.m_intId + "&action=set" + (IsProduct ? "&type=product" : "")));
			divToolBar.InnerHtml = result.ToString();
			result = null;
		}
    private void XmlEditToolBar()
    {
        System.Text.StringBuilder result = new System.Text.StringBuilder();
        divTitleBar.InnerHtml = m_refStyle.GetTitleBar(m_refMsg.GetMessage("btn edit ftsearch"));
        result.Append("<table><tr>");
        result.Append(m_refStyle.GetButtonEventsWCaption(AppImgPath + "../UI/Icons/back.png", bPage + "?id=" + m_intId + "&action=" + bAction, m_refMsg.GetMessage("alt back button text"), m_refMsg.GetMessage("btn back"), "OnClick=\"javascript:return SetAction(\'cancel\');\"", StyleHelper.BackButtonCssClass, true));
        result.Append(m_refStyle.GetButtonEventsWCaption(AppImgPath + "../UI/Icons/save.png", (string)("xml_index_select.aspx" + (IsProduct ? "?type=product" : "")), "Save Index Selection", m_refMsg.GetMessage("btn update"), "OnClick=\"javascript:return SetAction(\'update\');\"", StyleHelper.SaveButtonCssClass, true));
        if (EnableMultiLanguage == 1)
        {
            result.Append(StyleHelper.ActionBarDivider);
            result.Append("<td align=\"right\">" + m_refStyle.ShowAllActiveLanguage(false, "", "javascript:SelLanguage(this.value);", ContentLanguage.ToString()) + "</td>");
        }
        else
        {
            result.Append("<td>&nbsp;</td>");
        }
        result.Append(StyleHelper.ActionBarDivider);
        result.Append("<td>" + m_refStyle.GetHelpButton("EditFullTextSearch", "") + "</td>");
        result.Append("</tr></table>");
        divToolBar.InnerHtml = result.ToString();
        result = null;
    }
    private void XmlToolBar()
    {
        System.Text.StringBuilder result = new System.Text.StringBuilder();
        divTitleBar.InnerHtml = m_refStyle.GetTitleBar(m_refMsg.GetMessage("btn edit ftsearch"));
        result.Append("<table><tr>");
        result.Append(m_refStyle.GetButtonEventsWCaption(AppImgPath + "../UI/Icons/back.png", bPage + "?id=" + m_intId + "&action=" + bAction, m_refMsg.GetMessage("alt back button text"), m_refMsg.GetMessage("btn back"), "OnClick=\"javascript:return SetAction(\'cancel\');\"", StyleHelper.BackButtonCssClass, true));
        result.Append(m_refStyle.GetButtonEventsWCaption(AppImgPath + "../UI/Icons/save.png", (string)("xml_index_select.aspx?id=" + this.m_intId + (IsProduct ? "?type=product" : "")), "Save Index Selection", m_refMsg.GetMessage("btn update"), "OnClick=\"javascript:return SetAction(\'update\');\"", StyleHelper.SaveButtonCssClass, true));
        result.Append(StyleHelper.ActionBarDivider);
        result.Append("<td>" + m_refStyle.GetHelpButton("indexxmlTextSearch", "") + "</td>");
        result.Append("</tr></table>");
        SetFormAction((string)("xml_index_select.aspx?id=" + this.m_intId + "&action=set" + (IsProduct ? "&type=product" : "")));
        divToolBar.InnerHtml = result.ToString();
        result = null;
    }
    private object EditXPathLabel(long ID)
    {
        string[] arList;
        int iLoop;
        Collection cItems = new Collection();
        Collection Item;
        Ektron.Cms.Content.EkXmlIndexing IdxRef;
        IdxRef = m_refContentApi.EkXmlIndexingRef;

        arList = Strings.Split(Request.Form["ekt_list"], ",", -1, 0);

        for (iLoop = 0; iLoop <= (arList.Length - 1); iLoop++)
        {
            Item = new Collection();
            if (Strings.Trim(Request.Form[arList[iLoop] + "_label"]) != "")
            {
                Item.Add(Request.Form[arList[iLoop] + "_id"], "ID", null, null);
                Item.Add(Request.Form[arList[iLoop] + "_xpath"], "xpath", null, null);
                Item.Add(Request.Form[arList[iLoop] + "_label"], "label", null, null);
            }
            if (Item.Count > 0)
            {
                cItems.Add(Item, null, null, null);
            }
        }
        if (cItems.Count > 0)
        {
            IdxRef.SetXpathLabels(ID, cItems);
        }

        Response.Redirect((string)("" + sPage + "?action=" + sAction + "&id=" + ID), false);
        return null;
    }
    private void SetFormAction(string action)
    {
        string sJS;
        sJS = "<script language=\"javascript\">";
        sJS += " document.forms[0].action = \'" + action + "\';";
        sJS += "</script>";
        Page.ClientScript.RegisterStartupScript(this.GetType(), "SetAction", sJS);
    }
    private string GetXml(string srcstring, string xmlStartTag, string xmlEndTag)
    {
        string returnValue;
        string xmldata = "";
        int mystart;
        int myend;
        mystart = srcstring.IndexOf(xmlStartTag) + 1;
        if (mystart > 0)
        {
            myend = srcstring.IndexOf(xmlEndTag) + 1;
            if (myend > 0)
            {
                mystart = mystart + xmlStartTag.Length;
                xmldata = srcstring.Substring(mystart - 1, myend - mystart);
            }
        }
        returnValue = xmldata;
        return returnValue;
    }
    private string BuildTable(string XML, long id)
    {
        System.Xml.XPath.XPathNavigator XPathNav;
        System.Xml.XmlDocument XmlDoc = new System.Xml.XmlDocument();
        System.Xml.XmlDocument FieldListXML = new System.Xml.XmlDocument();
        System.Text.StringBuilder Output = new System.Text.StringBuilder();
        int iCounter = 0;
        string NameList = "";
        string Name;
        Collection cItem;
        bool bTmp = false;
        bool bIndexXPath = false;
        string Type;
        string dataType;
        string baseType;
        string XPath;
        string indexed;
        System.Xml.XPath.XPathNodeIterator Iterator = null;

        try
        {
            try
            {
                if ((XML != null) && (XML.Length > 0))
                {
                    XmlDoc.LoadXml(XML);
                    XPathNav = XmlDoc.CreateNavigator();
                    Iterator = XPathNav.Select("/fieldlist/field");
                }
            }
            catch
            {
            }
            //33781 - If there are no fields, (field list is empty), save configuration, & go to view configuration screen
            if (Iterator != null)
            {
                Output.Append("<table class=\"ektronGrid ektronBorder\">");
                Output.Append("<tr class=\"title-header\">");
                Output.Append("<td width=\"30%\" align=\"left\">XPath</td>");
                Output.Append("<td align=\"left\">Label</td>");
                Output.Append("</tr>");
                while (Iterator.MoveNext())
                {
                    bIndexXPath = true;
                    indexed = Iterator.Current.GetAttribute("indexed", "");
                    if (indexed.Length > 0 && indexed.ToLower() == "true")
                    {
                        XPath = Iterator.Current.GetAttribute("xpath", "");
                        dataType = Strings.LCase(Iterator.Current.GetAttribute("datatype", ""));
                        baseType = Strings.LCase(Iterator.Current.GetAttribute("basetype", ""));
                        Type = "";

                        switch (dataType)
                        {
                            //handle HTML rich text fields as if they are strings
                            //otherwise do not display in the table
                            case "anytype":
                                if (baseType == "html")
                                {
                                    Type = "string";
                                }
                                else
                                {
                                    bIndexXPath = false;
                                }
                                break;
                            case "anysimpletype":
                            case "base64binary":
                            case "hexbinary":
                                bIndexXPath = false;
                                break;
                            case "anyuri":
                                Type = "string";
                                break;
                            case "string":
                            case "boolean":
                            case "date":
                            case "selection":
                                Type = dataType;
                                break;
                            case "choice":
                                Type = "selection";
                                break;
                            case "decimal":
                            case "float":
                            case "decnum2":
                            case "decnum":
                            case "double":
                                Type = "decimal";
                                break;
                            case "byte":
                            case "int":
                            case "integer":
                            case "long":
                            case "negativeinteger":
                            case "nonpositiveinteger":
                            case "positiveinteger":
                            case "short":
                            case "unsignedbyte":
                            case "unsignedint":
                            case "unsignedlong":
                            case "unsignedshort":
                            case "percent":
                                Type = "integer";
                                break;

                            case "nonnegativeinteger":
                            case "nonnegint":
                            case "nonneg":
                                Type = "nonnegativeinteger";
                                break;
                            case "password":
                            case "textarea":
                                Type = "string";
                                break;
                            case "custom":
                                if (baseType == "number")
                                {
                                    Type = "decimal";
                                }
                                else
                                {
                                    Type = "string";
                                }
                                break;
                            default: //for phone email zip etc
                                if (baseType == "number")
                                {
                                    Type = "decimal";
                                }
                                else
                                {
                                    Type = "string";
                                }
                                break;
                        }

                        if (bIndexXPath && Type.Length > 1)
                        {
                            iCounter++;
                            Output.Append("<tr>");
                            //Col1 XPATH
                            Output.Append("<td width=\"30%\"  nowrap=\"true\">");
                            Output.Append(XPath);
                            cItem = new Collection();
                            bTmp = false;
                            if (DoesKeyExist(cSaved, XPath))
                            {
                                cItem = (Collection)cSaved[XPath];
                                bTmp = true;
                            }

                            Name = (string)("ekt" + iCounter);
                            if (NameList == "")
                            {
                                NameList = Name;
                            }
                            else
                            {
                                NameList += (string)("," + Name);
                            }
                            Output.Append("<input type=\"hidden\" name=\"" + Name + "\" id=\"" + Name + "\" value=\"" + XPath + "\">");
                            Output.Append("</td>");

                            //Col2 Label
                            Output.Append("<td nowrap=\"true\">");
                            Name = "ekt" + iCounter + "_label";
                            Output.Append("<input type=\"hidden\" name=\"" + Name + "\" id=\"" + Name + "\" value=\"" + Iterator.Current.Value.ToString() + "\">");
                            Output.Append("<span>" + EkFunctions.HtmlEncode(Iterator.Current.Value.ToString()) + "</span>");
                            Name = "ekt" + iCounter + "_type";
                            Output.Append("<input type=\"hidden\" name=\"" + Name + "\" id=\"" + Name + "\" value=\"" + Type + "\">");
                            if (Type == "selection")
                            {
                                Name = "ekt" + iCounter + "_style";
                                if (bTmp)
                                {
                                    Output.Append("<input type=\"checkbox\" name=\"" + Name + "2\" id=\"" + Name + "2\" ");
                                    if (cItem["style"].ToString() == "2")
                                    {
                                        Output.Append("checked");
                                    }
                                    Output.Append(">Multiple");
                                }
                                else
                                {
                                    Output.Append("<input type=\"checkbox\" name=\"" + Name + "2\" id=\"" + Name + "2\">Multiple");
                                }
                            }
                            else
                            {
                                Output.Append("&nbsp;");
                            }
                            Output.Append("</td>");

                            Output.Append("</tr>");
                        }
                    }
                }
                Output.Append("</table>");
                Name = "ekt_list";
                Output.Append("<input type=\"hidden\" name=\"" + Name + "\" id=\"" + Name + "\" value=\"" + NameList + "\">");
                Output.Append("<scr" + "ipt language=\"Javascript\">function EnableMulti(objCheck, name) { ");
                Output.Append("	var obj = eval(\'document.forms[0].\' + name + \'2\');");
                Output.Append(" if (objCheck.checked) { ");
                Output.Append("		if (typeof obj != \'undefined\') { obj.disabled = false; }");
                Output.Append("	} else { if (typeof obj != \'undefined\') { obj.checked = false; obj.disabled = true; } }");
                Output.Append("}");
                Output.Append("</scr" + "ipt>");
            }

            if (iCounter == 0)
            {
                return "";
            }
        }
        catch (Exception ex)
        {
            throw (ex);
        }
        return Output.ToString();
    }

    private Collection LoadSettings(long Id)
    {
        Ektron.Cms.Content.EkXmlIndexing IdxRef;
        IdxRef = m_refContentApi.EkXmlIndexingRef;
        try
        {
            return (IdxRef.GetXMLIndexConfig(Id));
        }
        catch (Exception ex)
        {
            throw (new Exception(ex.Message + "LoadSettings"));
        }
    }

    private void SaveSettings(long id)
    {
        string[] arList;
        Collection cItems = new Collection();
        Collection cItem = new Collection();
        Ektron.Cms.Content.EkXmlIndexing IdxRef;
        IdxRef = m_refContentApi.EkXmlIndexingRef;
        try
        {
            arList = Strings.Split(Request.Form["ekt_list"], ",", -1, 0);
            foreach (string sitem in arList)
            {
                cItem = new Collection();
                cItem.Add(Request.Form[sitem], "xpath", null, null); //\root\field
                cItem.Add(Request.Form[sitem + "_label"], "label", null, null); //xpath label - "label"
                cItem.Add(ContentLanguage, "ContentLanguage", null, null);
                cItem.Add(Request.Form[sitem + "_type"], "type", null, null);
                if (Request.Form[sitem + "_type"] == "string" || Request.Form[sitem + "_type"] == "selection")
                {
                    if (Request.Form[sitem + "_style2"] == "on")
                    {
                        cItem.Add("2", "style", null, null);
                    }
                    else if (Request.Form[sitem + "_style"] == "on")
                    {
                        cItem.Add("1", "style", null, null);
                    }
                    else
                    {
                        cItem.Add("0", "style", null, null);
                    }
                }
                else
                {
                    cItem.Add("0", "style", null, null);
                }
                //35173 - Error message when updating smartform  which is having select field list and choice field together both indexed and has Advanced field property type as content.
                if (!DoesKeyExist(cItems, (string)(Request.Form[sitem].ToString())))
                {
                    cItems.Add(cItem, Request.Form[sitem], null, null);
                }
            }

            IdxRef.SetXmlIndexConfig(id, cItems);
            Response.Redirect((string)("" + sPage + "?action=" + sAction + "&id=" + id), false);

        }
        catch (Exception ex)
        {
            throw (new Exception(ex.Message + "SaveSettings"));
        }
    }
    private string DisplayXpathLabels()
    {

        string strRet;
        int lCounter = 1;
        string strList = "";
        string Name = "";
        try
        {
            strRet = "";
            strRet = "<table width=\"100%\">";
            strRet += "<tr>";
            strRet += "<th align=left>XPath<hr/></th>";
            strRet += "<th align=left colspan=2>Label<hr/></th>";
            strRet += "</tr>";
            foreach (Collection Item in cSaved)
            {
                Name = (string)("ekt_" + lCounter.ToString());
                if (strList == "")
                {
                    strList = Name;
                }
                else
                {
                    strList += (string)("," + Name);
                }
                strRet += "<tr>";
                strRet += (string)("<td width=\"5%\" nowrap=\"true\">" + Item["xpath"]);
                strRet += "<input type=\"hidden\" name=\"" + Name + "_xpath\" id=\"" + Name + "_xpath\" value=\"" + Item["xpath"] + "\">";
                strRet += "<input type=\"hidden\" name=\"" + Name + "_id\" id=\"" + Name + "_id\" value=\"" + Item["ID"] + "\">";
                strRet += "</td>";
                strRet += "<td><input type=\"textbox\" name=\"" + Name + "_label\" value=\"";

                if (DoesKeyExist(((Collection)Item["labels"]), ContentLanguage.ToString()))
                {
                    strRet += (string)(((Collection)(((Collection)Item["labels"])[ContentLanguage.ToString()]))["label"] + "\">");
                }
                else
                {
                    strRet += (string)(Item["xpath"] + "&nbsp;&nbsp;" + "\">");
                }
                strRet += "</td>";
                strRet += "</tr>";
                lCounter++;
            }
            strRet += "</table>";
            strRet += "<input type=\"hidden\" name=\"ekt_list\" id=\"ekt_list\" value=\"" + strList + "\">";
            return strRet;

        }
        catch (Exception)
        {
        }
        return null;
    }
    private bool DoesKeyExist(Collection obj, string KeyName)
    {
        bool returnValue;
        Collection cTmp = new Collection();
        try
        {
            if (Information.TypeName(obj).ToLower() == "collection")
            {
                cTmp.Add(obj[KeyName], null, null, null);
                returnValue = true;
            }
            else
            {
                returnValue = obj.Contains(KeyName);
            }
        }
        catch (Exception)
        {
            returnValue = false;
        }
        finally
        {
            cTmp = null;
        }
        return returnValue;
    }
    private void RegisterResources()
    {
        ICmsContextService cmsContextService = ServiceFactory.CreateCmsContextService();

        // create a package that will register the UI JS and CSS we need
        Package searchResultsControlPackage = new Package()
        {
            Components = new List<Component>()
            {
                // Register JS Files
                Packages.Ektron.Workarea.Core,
                JavaScript.Create(cmsContextService.WorkareaPath + "/java/jfunct.js"),
                JavaScript.Create(cmsContextService.WorkareaPath + "/java/toolbar_roll.js"),
                JavaScript.Create(cmsContextService.WorkareaPath + "/java/workareahelper.js"),
                JavaScript.Create(cmsContextService.WorkareaPath + "/java/stylehelper.js"),

                // Register CSS Files
                Css.Create(cmsContextService.WorkareaPath + "/csslib/ektron.fixedPositionToolbar.css")
            }
        };
        searchResultsControlPackage.Register(this);
    }
}