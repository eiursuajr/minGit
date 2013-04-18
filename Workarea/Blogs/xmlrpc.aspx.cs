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
using System.Xml;
using System.IO;

public partial class Blogs_xmlrpc : System.Web.UI.Page
{
    XmlDocument m_xdRequest = new XmlDocument();
    XmlDocument m_xdResponse = new XmlDocument();
    ContentAPI m_refContAPI = new ContentAPI();
    Ektron.Cms.Content.Blog m_refBlog;

    private void Page_Load(System.Object sender, System.EventArgs e)
    {
        try
        {
            Response.Clear();
            Response.ContentType = "text/xml";
             //get the XML from the request.
            Stream stream = Request.InputStream;
            XmlReaderSettings settings = new XmlReaderSettings();
            // allow entity parsing but do so more safely
            settings.ProhibitDtd = false;
            settings.MaxCharactersFromEntities = 1024;
            settings.XmlResolver = null;

            XmlReader reader = XmlReader.Create(stream, settings);
            XmlDocument doc = new XmlDocument();
            m_xdRequest.Load(reader);
			
            m_refBlog = new Ektron.Cms.Content.Blog(m_refContAPI.RequestInformationRef);
            if (m_xdRequest.InnerXml.ToLower().IndexOf("<methodname>metaweblog.newmediaobject</methodname>") > -1)
            {
                Collection lib_setting_data = new Collection();
                Collection libdata = new Collection();
                Ektron.Cms.Library.EkLibrary m_refLibrary;
                long iblogid = 0;
                System.Xml.XmlNodeList xnlParams;
                string imgpath = "";
                string filepath = "";
                XmlElement xeElem;
                m_refLibrary = m_refContAPI.EkLibraryRef;
                xnlParams = m_xdRequest.SelectNodes("methodCall/params/param");
                if (xnlParams[0].FirstChild.HasChildNodes == true)
                {
                    iblogid = Convert.ToInt64(xnlParams[0].FirstChild.FirstChild.InnerText);
                }
                else
                {
                    iblogid = Convert.ToInt64(xnlParams[0].FirstChild.InnerText);
                }

                libdata = new Collection();
                libdata.Add(iblogid, "FolderID", null, null);
                libdata.Add(true, "Override", null, null);
                lib_setting_data = m_refLibrary.GetLibrarySettingsv2_2(libdata);
                if (lib_setting_data != null)
                {
                    if (lib_setting_data["ImageDirectory"] != null)
                        imgpath = (string)lib_setting_data["ImageDirectory"];
                    if (lib_setting_data["FileDirectory"] != null)
                        filepath = (string)lib_setting_data["FileDirectory"];
                }

                try
                {
                    imgpath = Server.MapPath(imgpath);
                    filepath = Server.MapPath(filepath);
                }
                catch (Exception)
                {
                    // eat this exception
                }

                // now attach to the xml.
                xeElem = m_xdRequest.CreateElement("member");
                xeElem.AppendChild(m_xdRequest.CreateElement("name"));
                xeElem.ChildNodes[0].InnerText = "imgpath";
                xeElem.AppendChild(m_xdRequest.CreateElement("value"));
                xeElem.ChildNodes[1].AppendChild(m_xdRequest.CreateElement("string"));
                xeElem.ChildNodes[1].ChildNodes[0].InnerText = imgpath;
                m_xdRequest.ChildNodes[1].ChildNodes[1].ChildNodes[3].ChildNodes[0].ChildNodes[0].AppendChild(xeElem);
                xeElem = m_xdRequest.CreateElement("member");
                xeElem.AppendChild(m_xdRequest.CreateElement("name"));
                xeElem.ChildNodes[0].InnerText = "filepath";
                xeElem.AppendChild(m_xdRequest.CreateElement("value"));
                xeElem.ChildNodes[1].AppendChild(m_xdRequest.CreateElement("string"));
                xeElem.ChildNodes[1].ChildNodes[0].InnerText = filepath;
                m_xdRequest.ChildNodes[1].ChildNodes[1].ChildNodes[3].ChildNodes[0].ChildNodes[0].AppendChild(xeElem);
            }

            m_xdResponse = m_refContAPI.MetaWeblogAPI(m_xdRequest);
            ShowResponse(m_xdResponse);
        }
        catch (Exception ex)
        {
            EkException.LogException(ex);
            ErrorResponse(1, this.m_refContAPI.EkMsgRef.GetMessage("generic error"));
        }
    }

    private void ErrorResponse(int errNum, string errText)
    {
        try
        {
            XmlDocument d = new XmlDocument();
            XmlElement root = d.CreateElement("response");
            d.AppendChild(root);
            XmlElement er = d.CreateElement("error");
            root.AppendChild(er);
            er.AppendChild(d.CreateTextNode(errNum.ToString()));
            if (errText != "")
            {
                System.Xml.XmlElement msg = d.CreateElement("message");
                root.AppendChild(msg);
                msg.AppendChild(d.CreateTextNode(errText));
            }

            d.Save(Response.Output);
            Response.End();
        }
        catch (Exception)
        {
            //handle the error.
        }
    }

    private void ShowResponse(XmlDocument ResponseXML)
    {
        try
        {
            ResponseXML.Save(Response.Output);
            // Response.End() ' this throws an error.
        }
        catch (Exception ex)
        {
            EkException.LogException(ex);
            ErrorResponse(1, this.m_refContAPI.EkMsgRef.GetMessage("generic error"));
        }
    }
}

