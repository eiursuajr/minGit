using System;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using System.Net;
using Ektron.Cms.Widget;
using Ektron.Cms;
using Ektron.Cms.API;
using Ektron.Cms.Common;
using Ektron.Cms.PageBuilder;
using System.Text.RegularExpressions;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Xml;
using System.Xml.Serialization;

public partial class Widgets_RSSFeed : System.Web.UI.UserControl, IWidget
{

    #region properties
    private string _RSSFeedURL;
    [WidgetDataMember("")]
    public string RSSFeedURL { get { return _RSSFeedURL; } set { _RSSFeedURL = value; } }
    #endregion

    IWidgetHost _host;

    protected void Page_Init(object sender, EventArgs e)
    {
        _host = Ektron.Cms.Widget.WidgetHost.GetHost(this);
        _host.Title = "RSS Widget";
        _host.Edit += new EditDelegate(EditEvent);
        _host.Maximize += new MaximizeDelegate(delegate() { Visible = true; });
        _host.Minimize += new MinimizeDelegate(delegate() { Visible = false; });
        _host.Create += new CreateDelegate(delegate() { EditEvent(""); });
        PreRender += new EventHandler(delegate(object PreRenderSender, EventArgs Evt) { SetOutput(); });
        ViewSet.SetActiveView(View);
    }

    void EditEvent(string settings)
    {
            try
        {
            ScriptManager.RegisterClientScriptBlock(this, this.GetType(), @"WidgetEnterCheck" + _host.WidgetInfo.ID.ToString(), "<script type='text/javascript'>function checkForEnter" + _host.WidgetInfo.ID.ToString() + @"(e, saveButtonID){var evt = e ? e : window.event;if(evt.keyCode == 13){document.forms[0].onsubmit = function () { return false; }; evt.cancelBubble = true; if (evt.stopPropagation) evt.stopPropagation(); return false; }}</script>", false);
            feedURL.Attributes.Add("onkeydown", @"javascript:checkForEnter" + _host.WidgetInfo.ID.ToString() + @"(event, '" + SaveButton.ClientID + "')");
            feedURL.Text = RSSFeedURL;
            ViewSet.SetActiveView(Edit);
        }
        catch
        {
            lblData.Text = "Error loading settings";
        }

              ViewSet.SetActiveView(Edit);
    }

    protected void SaveButton_Click(object sender, EventArgs e)
    {
        try
        {
            
            RSSFeedURL = feedURL.Text;
   
           
        }
        catch
        {
            lblData.Text = "Error saving settings";
           
        }
        _host.SaveWidgetDataMembers();
        ViewSet.SetActiveView(View);
    }

    protected void SetOutput()
    {
       try
        {
            List<RSSFeedWidgetRepeaterData> dataItems = new List<RSSFeedWidgetRepeaterData>();
            XmlDocument xmldoc = new XmlDocument();
            int count = 0;
            string blogTitle = "";
            string blogLink = "";
            bool titlePrinted = false;
            string blogTitleOriginal = "";

            if (RSSFeedURL != "")
            {
                lblData.Text = @"<style>.rssItem img{max-width:100%;} .rssItem embed{max-width:100%;}</style>";
                WebRequest request = WebRequest.Create(RSSFeedURL);
                request.Timeout = 5000;
                HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                Stream resStream = response.GetResponseStream();
                xmldoc.Load(resStream);
                XmlNode root = xmldoc.FirstChild;
                while (root.LocalName != "rss")
                {
                    root = root.NextSibling;
                }
                root = root.FirstChild;
                XmlNodeList feedNodes = root.ChildNodes;
                foreach (XmlNode tempNode in feedNodes)
                {
                    if (tempNode.Name.Equals("title"))
                    {
                        blogTitle = tempNode.InnerText;
                        blogTitleOriginal = blogTitle;
                    }
                    if (tempNode.Name.Equals("link"))
                    {
                        blogLink = tempNode.InnerText;
                    }
                    if (!titlePrinted)
                    {
                        if (blogTitle != "" && blogLink != "")
                        {
                            titlePrinted = true;
                            blogTitle = new Regex(@"\W*").Replace(blogTitle, string.Empty);
                        }
                    }
                    if (tempNode.Name.Equals("item"))
                    {
                        RSSFeedWidgetRepeaterData temp = new RSSFeedWidgetRepeaterData();
                        XmlNodeList itemNodes = tempNode.ChildNodes;
                        foreach (XmlNode item in itemNodes)
                        {
                            if (item.Name.Equals("description"))
                            {
                                temp.content = item.InnerText;
                            }
                            if (item.Name.Equals("title"))
                            {
                                temp.contentTitle = item.InnerText;
                            }
                            if (item.Name.Equals("link"))
                            {
                                temp.url = item.InnerText;
                            }
                        }
                        temp.hostid = _host.WidgetInfo.ID.ToString();
                        temp.count = count.ToString();
                        count++;
                        dataItems.Add(temp);
                    }
                }
                Repeater1.DataSource = dataItems.ToArray();
                Repeater1.DataBind();
            }
            else
            {
                lblData.Text = "No Feed Entered";
                blogTitleOriginal = "RSS Feed Widget";
            }

            _host.Title = Server.HtmlEncode(blogTitleOriginal);
        }
        catch
        {
            lblData.Text = "Invalid Feed URL";
            _host.Title = "RSS Feed Widget";
        }  
    }

    protected void CancelButton_Click(object sender, EventArgs e)
    {
        ViewSet.SetActiveView(View);
    }

   
}










   

public class RSSFeedWidgetRepeaterData
{
    public string hostid;
    public string count;
    public string content;
    public string url;
    public string contentTitle;

    public RSSFeedWidgetRepeaterData()
    {
        hostid = "";
        count = "";
        content = "";
        url = "";
        contentTitle = "";
    }
}