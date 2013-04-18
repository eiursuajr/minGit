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


public class EspnNewsFeedPair
{
    public string FeedName;
    public string FeedUrl;

    public EspnNewsFeedPair(string name, string url)
    {
        FeedName = name;
        FeedUrl = url;
    }
}


public partial class Widgets_Espn : System.Web.UI.UserControl, IWidget
{

    #region properties
    private string _NewsFeedURL;
    [WidgetDataMember("")]
    public string NewsFeedURL { get { return _NewsFeedURL; } set { _NewsFeedURL = value; } }
    #endregion

    IWidgetHost _host;
    List<EspnNewsFeedPair> feeds;

    protected void Page_Init(object sender, EventArgs e)
    {
        _host = Ektron.Cms.Widget.WidgetHost.GetHost(this);
        _host.Title = "ESPN News Widget";
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

            List<EspnNewsFeedWidgetRepeaterData> dataItems = new List<EspnNewsFeedWidgetRepeaterData>();
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
                        EspnNewsFeedWidgetRepeaterData temp = new EspnNewsFeedWidgetRepeaterData();
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
                blogTitleOriginal = "ESPN News Feed Widget";
            }

            _host.Title = Server.HtmlEncode(blogTitleOriginal);
        }
        catch
        {
            lblData.Text = "ESPN News Feed Error";
            _host.Title = "ESPN News Feed Widget";
        }

    }

    protected void CancelButton_Click(object sender, EventArgs e)
    {
        ViewSet.SetActiveView(View);
    }

    void GenerateFeedList()
    {
        feeds = new List<EspnNewsFeedPair>();
        feeds.Add(new EspnNewsFeedPair("Top Headlines", "http://sports.espn.go.com/espn/rss/news"));
        feeds.Add(new EspnNewsFeedPair("NFL Headlines", "http://sports.espn.go.com/espn/rss/nfl/news"));
        feeds.Add(new EspnNewsFeedPair("NBA Headlines", "http://sports.espn.go.com/espn/rss/nba/news"));
        feeds.Add(new EspnNewsFeedPair("MLB Headlines", "http://sports.espn.go.com/espn/rss/mlb/news"));
        feeds.Add(new EspnNewsFeedPair("NHL Headlines", "http://sports.espn.go.com/espn/rss/nhl/news"));

        feeds.Add(new EspnNewsFeedPair("Motorsports Headlines", "http://sports.espn.go.com/espn/rss/rpm/news"));
        feeds.Add(new EspnNewsFeedPair("Soccer Headlines", "http://soccernet.espn.go.com/rss/news"));
        feeds.Add(new EspnNewsFeedPair("ESPNU", "http://sports.espn.go.com/espn/rss/espnu/news"));
        feeds.Add(new EspnNewsFeedPair("College Basketball Headlines", "http://sports.espn.go.com/espn/rss/ncb/news"));
        feeds.Add(new EspnNewsFeedPair("College Football Headlines", "http://sports.espn.go.com/espn/rss/ncf/news"));


        feeds.Add(new EspnNewsFeedPair("ESPN Outdoors.com Headlines", "http://sports.espn.go.com/espn/rss/outdoors/news"));
        feeds.Add(new EspnNewsFeedPair("Bassmaster.com Headlines", "http://sports.espn.go.com/espn/rss/bassmaster/news"));
        feeds.Add(new EspnNewsFeedPair("ESPN Latest Videos", "http://sports.espn.go.com/broadband/ivp/rss"));
        feeds.Add(new EspnNewsFeedPair("Bill Simmons Columns", "http://search.espn.go.com/rss/bill-simmons/"));


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


public class EspnNewsFeedWidgetRepeaterData
{
    public string hostid;
    public string count;
    public string content;
    public string url;
    public string contentTitle;

    public EspnNewsFeedWidgetRepeaterData()
    {
        hostid = "";
        count = "";
        content = "";
        url = "";
        contentTitle = "";
    }
}

