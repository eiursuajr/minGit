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

public partial class ekvalidate : Ektron.Cms.Workarea.Framework.WorkAreaBasePage
	{		
		private string m_strInvalidMessage;
		
		protected void Page_Load(object sender, System.EventArgs e)
		{
			AssertInternalReferrer();
			
			string output = "";
			
			try
			{
                string sXml = string.Empty;
                if (!string.IsNullOrEmpty(Request["xml"]))
				sXml = Request["xml"];
				
				System.Xml.Schema.XmlSchemaSet objSchemas = new System.Xml.Schema.XmlSchemaSet();
				
				int iXsd;
                string sXsd = string.Empty;
                string sNsUri = string.Empty;
				iXsd = 0;
                if (!string.IsNullOrEmpty(Request["xsd0"]))
				sXsd = Request["xsd0"];
                if (!string.IsNullOrEmpty(Request["nsuri0"]))
				sNsUri = Request["nsuri0"];
				if ((sXsd == null) || 0 == sXsd.Length)
				{
                    if (!string.IsNullOrEmpty(Request["xsd"]))
					sXsd = Request["xsd"];
                    if (!string.IsNullOrEmpty(Request["nsuri"]))
					sNsUri = Request["nsuri"];
				}
                while ((!string.IsNullOrEmpty(sXsd)) && sXsd.Length > 0)
				{
					if (! sXsd.Contains("<"))
					{
						string sXsdUri;
						try
						{
							sXsdUri = new Uri(Request.Url, sXsd).AbsoluteUri;
							objSchemas.Add(sNsUri, sXsdUri);
						}
						catch
						{
							try
							{
								sXsdUri = (string) (Server.MapPath(sXsd));
							}
							catch (System.Web.HttpException)
							{
								// Ignore; URL is likely not within this web application
								sXsdUri = sXsd;
							}
							objSchemas.Add(sNsUri, sXsdUri);
						}
					}
					else
					{
						objSchemas.Add(sNsUri, new System.Xml.XmlTextReader(new System.IO.StringReader(sXsd)));
					}
					iXsd++;
                    if (!string.IsNullOrEmpty(Request["xsd" + iXsd]))
                    {
                        sXsd = Request["xsd" + iXsd];
                    }
                    else
                    {
                        sXsd = string.Empty;
                    }
                    if (!string.IsNullOrEmpty(Request["nsuri" + iXsd]))
					sNsUri = Request["nsuri" + iXsd];
				}
				
				System.Xml.XPath.XPathDocument objXmlDoc;
				if (string.IsNullOrEmpty(sXml))
				{
					objXmlDoc = null;
				}
				else if (! sXml.Contains("<"))
				{
					string sXmlUri;
					try
					{
						sXmlUri = new Uri(Request.Url, sXml).AbsoluteUri;
						objXmlDoc = new System.Xml.XPath.XPathDocument(sXmlUri);
					}
					catch
					{
						try
						{
							sXmlUri = (string) (Server.MapPath(sXml));
						}
						catch (System.Web.HttpException)
						{
							// URL is likely not within this web application or is dynamic
							sXmlUri = sXml;
						}
						objXmlDoc = new System.Xml.XPath.XPathDocument(sXmlUri);
					}
				}
				else
				{
					objXmlDoc = new System.Xml.XPath.XPathDocument(new System.Xml.XmlTextReader(new System.IO.StringReader(sXml)));
				}
				
				if (objXmlDoc != null)
				{
					// No XML, we are validating schema(s)
					output = ValidateXml(objXmlDoc, objSchemas);
				}
				
			}
			catch (Exception ex)
			{
				output = ex.Message;
			}
			
			Response.ContentType = "text/html";
			Response.ContentEncoding = System.Text.Encoding.UTF8; // Safari does not encode properly even though this is set
			
			litContent.Text = output;
		}
		
		private string ValidateXml(System.Xml.XPath.XPathDocument objXmlDocument, System.Xml.Schema.XmlSchemaSet objSchemas)
		{
			System.Xml.XPath.XPathNavigator nav = objXmlDocument.CreateNavigator();
			m_strInvalidMessage = "";
			
			nav.CheckValidity(objSchemas, new System.Xml.Schema.ValidationEventHandler(ValidationHandler));
			
			return m_strInvalidMessage;
		}
		
		protected void ValidationHandler(object sender, System.Xml.Schema.ValidationEventArgs validationArgs)
		{
			// process XML/XSD validation warnings & errors errors here
			switch (validationArgs.Severity)
			{
				case System.Xml.Schema.XmlSeverityType.Error:
					m_strInvalidMessage = m_strInvalidMessage + validationArgs.Message + "\r\n" + "\r\n" + "\r\n";
					break;
				case System.Xml.Schema.XmlSeverityType.Warning:
					m_strInvalidMessage = m_strInvalidMessage + validationArgs.Message + "\r\n" + "\r\n" + "\r\n";
					break;
			}
		}
		
	}