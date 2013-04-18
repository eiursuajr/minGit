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
using System.Net;
using System.IO;
using System.Xml;
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

	public partial class xml_verify : System.Web.UI.Page
	{
        protected CommonApi commonAPI = new CommonApi();
		protected string XmlPath = "";
		protected Ektron.Cms.Common.EkMessageHelper m_refMsg;
		
		private void Page_Load(System.Object sender, System.EventArgs e)
		{
			Response.CacheControl = "no-cache";
			Response.AddHeader("Pragma", "no-cache");
			Response.Expires = -1;
			
			XmlPath = commonAPI.XmlPath;
			m_refMsg = commonAPI.EkMsgRef;
			verifydata.Text += GetResponse();
		}
		private string GetResponse()
		{
			System.Text.StringBuilder result = new System.Text.StringBuilder();
			object relativepath;
			object fullpath;
			object strResult;
			relativepath = Request.QueryString["path"];
			if ((Strings.Left(relativepath.ToString(), 7) == "http://") || (Strings.Left(relativepath.ToString(), 8) == "https://"))
			{
				fullpath = relativepath;
			}
			else
			{
				fullpath = Server.MapPath(XmlPath + "\\" + relativepath);
			}
			strResult = Testfile(fullpath.ToString());
			if (Strings.Len(strResult) > 0)
			{
				result.Append("<script language=\"javascript\">" + "\r\n");
				result.Append("function CompleteParserVerify() {" + "\r\n");
				result.Append("if (document.layers) {" + "\r\n");
				result.Append("bPassed = false;" + "\r\n");
				result.Append("strXslErrorMsg = \"" + Strings.Replace(strResult.ToString(), "\"", "\\\"", 1, -1, 0).Replace("\r\n", "\\n") + "\";");
				result.Append("bVerifying = false;" + "\r\n");
				result.Append("}" + "\r\n");
				result.Append("else {" + "\r\n");
				result.Append("parent.bPassed = false;" + "\r\n");
				result.Append("parent.strXslErrorMsg = \"" + Strings.Replace(strResult.ToString(), "\"", "\\\"", 1, -1, 0).Replace("\r\n", "\\n") + "\";");
				result.Append("parent.bVerifying = false;" + "\r\n");
				result.Append("}" + "\r\n");
				result.Append("}" + "\r\n");
				result.Append("</script>" + "\r\n");
			}
			else
			{
				result.Append("<script language=\"javascript\">" + "\r\n");
				result.Append("function CompleteParserVerify() {" + "\r\n");
				result.Append("if (document.layers) {" + "\r\n");
				result.Append("bPassed = true;" + "\r\n");
				result.Append("bVerifying = false;" + "\r\n");
				result.Append("}" + "\r\n");
				result.Append("else {" + "\r\n");
				result.Append("parent.bPassed = true;" + "\r\n");
				result.Append("parent.bVerifying = false;" + "\r\n");
				result.Append("}" + "\r\n");
				result.Append("}" + "\r\n");
				result.Append("</script>" + "\r\n");
			}
			return (result.ToString());
		}
		private string Testfile(string XDAT)
		{
			string returnValue;
			string xtype;
			string strRetValue = "";
			string strTmpLine = "";
			string strTmpLine1 = "";
			int lLineWidth = 0;
			string strOutLine = "";
			int lLoop;
			string strChar = "";
			string strTmpWord = "";
			int lNumofLines = 0;
			int lLineNumber = 0;
			System.Xml.XmlDocument xdoc;
			try
			{
                if (!IsUserAuthenticated())
                    throw new Ektron.Cms.Exceptions.InvalidAccessException();
				lLineWidth = 60;
				lNumofLines = 10;
				verifydata.Text += m_refMsg.GetMessage("url link label") + "<BR>" + XDAT + "<BR>";
				xtype = Request.QueryString["xtype"];
				if (XDAT.Length == 0)
				{
					returnValue = "";
					return returnValue;
				}
                Stream stream = null;
                if (!string.IsNullOrEmpty(XDAT) &&
                    XDAT.IndexOf(":\\") == 1)
                {
                    stream = new FileStream(XDAT, FileMode.Open);
                }
                else if (Ektron.Cms.Common.EkFunctions.IsURL(XDAT))
                {
                    HttpWebRequest request = (HttpWebRequest)
                    WebRequest.Create(XDAT);
                    // execute the request
                    HttpWebResponse response = (HttpWebResponse)
                        request.GetResponse();
                    // we will read data via the response stream
                    stream = response.GetResponseStream();
                }
                else
                {
                    stream = new MemoryStream(Encoding.Default.GetBytes(XDAT));
                }
                XmlReaderSettings settings = new XmlReaderSettings();

                // allow entity parsing but do so more safely
                settings.ProhibitDtd = false;
                settings.MaxCharactersFromEntities = 1024;
                settings.XmlResolver = null;

                XmlReader reader = XmlReader.Create(stream, settings);
                XmlDocument doc = new XmlDocument();
                doc.Load(reader);

				verifydata.Text += "ok";
				strRetValue = "";
			}
			catch (System.Xml.XmlException ex)
			{
				Response.Write("failed");
				strRetValue += "Error: loading XSL Document:" + "\\n";
				strRetValue += "----------------------------" + "\\n";
				strRetValue += "Error Line:" + ex.LineNumber + "\\n";
				strRetValue += "Error Position:" + ex.LinePosition + "\\n";
				strRetValue += "Error Position:" + ex.Message + "\\n\\n";
				strOutLine = "";
				strTmpLine1 = "";
				strTmpWord = "";
				lLineNumber = 1;
				for (lLoop = 1; lLoop <= strTmpLine.Length; lLoop++)
				{
					strChar = strTmpLine.Substring(lLoop - 1, 1);
					if ((strChar == " ") || (strChar == "\r" ) || (strChar == "\n"))
					{
						if ((strTmpLine1.Length + strTmpWord.Length ) < lLineWidth)
						{
							strTmpLine1 = strTmpLine1 + strChar + strTmpWord;
						}
						else
						{
							lLineNumber++;
							if (lLineNumber > lNumofLines)
							{
								strOutLine = strOutLine + strTmpLine1;
								strTmpLine1 = "";
								strTmpWord = "";
								break;
							}
							else
							{
								strOutLine = strOutLine + strTmpLine1 + "\\n";
								strTmpLine1 = strTmpWord;
							}
						}
						strTmpWord = "";
					}
					else
					{
						strTmpWord = strTmpWord + strChar;
					}
				}
				strOutLine = strOutLine + strTmpLine1 + " " + strTmpWord;
				strRetValue = strRetValue + strOutLine + "\\n\\n";
                EkException.LogException(strRetValue);
                strRetValue = commonAPI.EkMsgRef.GetMessage("generic error");
			}
			catch (Exception ex)
			{
				if (ex.Message.IndexOf("Could not find a part of the path") == -1)
				{
					strRetValue = ex.Message.Replace("\\", "\\\\");
				}
				else
				{
					strRetValue = m_refMsg.GetMessage("msg could not find path");
				}
                EkException.LogException(strRetValue);
                strRetValue = commonAPI.EkMsgRef.GetMessage("generic error");
			}
			xdoc = null;
			returnValue = strRetValue;
			return returnValue;
		}
	    private bool IsUserAuthenticated()
		{
			bool isAuthenticated = false;
			if (commonAPI.UserId > 0 && commonAPI.UniqueId > 0)
			{
				Ektron.Cms.UserData user = Ektron.Cms.ObjectFactory.GetUser(commonAPI.RequestInformationRef).GetItem(commonAPI.UserId);
				if (user != null)
					isAuthenticated = (user.LoginIdentification == commonAPI.UniqueId.ToString());
			}
			return isAuthenticated;
		}
	}