using System;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using System.Net;
using System.Collections;
using System.Collections.Generic;
using Ektron.Cms.Widget;
using Ektron.Cms;
using Ektron.Cms.API;
using Ektron.Cms.Common;
using Ektron.Cms.PageBuilder;
using System.Xml;
using System.Xml.Serialization;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;


public class NewsFeedPair
{
    public string FeedName;
    public string FeedUrl;

    public NewsFeedPair(string name, string url)
    {
        FeedName = name;
        FeedUrl = url;
    }
}


public partial class Widgets_News : System.Web.UI.UserControl, IWidget
{

    #region properties
    private string _NewsFeedURL;
    [WidgetDataMember("")]
    public string NewsFeedURL { get { return _NewsFeedURL; } set { _NewsFeedURL = value; } }
    #endregion

    IWidgetHost _host;
    List<NewsFeedPair> feeds;

    protected void Page_Init(object sender, EventArgs e)
    {
        _host = Ektron.Cms.Widget.WidgetHost.GetHost(this);
        _host.Title = "News Widget";
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
            GenerateFeedList();
            Repeater1.DataSource = feeds.ToArray();
            Repeater1.DataBind();
            ViewSet.SetActiveView(Edit);
        }
        catch
        {
            ViewSet.SetActiveView(View);
        }
    }

    
    protected void SetOutput()
    {
        try
        {
            
            List<NewsFeedWidgetRepeaterData> dataItems = new List<NewsFeedWidgetRepeaterData>();
            string blogTitleOriginal = "";
            int count = 0;
            string blogTitle = "";
            string blogLink = "";
            bool titlePrinted = false;
            XmlDocument xmldoc = new XmlDocument();

            if (NewsFeedURL != "")
            {
                lblData.Text = @"<style>.newsItem img{max-width:100%;} .newsItem embed{max-width:100%;}</style>";
                WebRequest request = WebRequest.Create(NewsFeedURL);
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
                        NewsFeedWidgetRepeaterData temp = new NewsFeedWidgetRepeaterData();
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
                Repeater2.DataSource = dataItems.ToArray();
                Repeater2.DataBind();
            }
            else
            {
                lblData.Text = "No News Feed Selected";
                blogTitleOriginal = "News Feed Widget";
            }

            _host.Title = Server.HtmlEncode(blogTitleOriginal);
        }
        catch
        {
            lblData.Text = "News Feed Error";
            _host.Title = "News Feed Widget";
        }
        
    }

    protected void CancelButton_Click(object sender, EventArgs e)
    {
        ViewSet.SetActiveView(View);
    }

    void GenerateFeedList()
    {
        feeds = new List<NewsFeedPair>();
        feeds.Add(new NewsFeedPair("CNN Top Stories", "http://rss.cnn.com/rss/cnn_topstories.rss"));
        feeds.Add(new NewsFeedPair("ABC News Top Stories", "http://feeds.feedburner.com/AbcNews_TopStories"));
        feeds.Add(new NewsFeedPair("MSNBC Top Stories", "http://rss.msnbc.msn.com/id/3032091/device/rss/rss.xml"));
        feeds.Add(new NewsFeedPair("CBS News Top Stories", "http://feeds.cbsnews.com/CBSNewsMain?format=xml"));
        feeds.Add(new NewsFeedPair("NYTimes HomePage", "http://www.nytimes.com/services/xml/rss/nyt/HomePage.xml"));
    }

    protected void Repeater1_ItemCommand(object source, RepeaterCommandEventArgs e)
    {
        try
        {
            GenerateFeedList();
            
            NewsFeedURL = feeds[e.Item.ItemIndex].FeedUrl;
            _host.SaveWidgetDataMembers();
            ViewSet.SetActiveView(View);
            SetOutput();
        }
        catch
        {
            ViewSet.SetActiveView(View);
        }
    }

}


public class NewsFeedWidgetRepeaterData
{
    public string hostid;
    public string count;
    public string content;
    public string url;
    public string contentTitle;

    public NewsFeedWidgetRepeaterData()
    {
        hostid = "";
        count = "";
        content = "";
        url = "";
        contentTitle = "";
    }
}
