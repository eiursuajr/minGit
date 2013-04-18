using System;
using System.Data;
using System.Configuration;
using System.Collections;
using System.Net;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using System.Xml;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using Ektron.Cms.Widget;
using System.Collections.Generic;
using System.Xml.Serialization;

[XmlRoot("YouTubeWidgetData")]
public class YouTubeWidgetData
{
    [XmlElement("YouTubeFeedURL")]
    public string YouTubeFeedURL;

    [XmlElement("YouTubeFeedCount")]
    public int YouTubeFeedCount;

    [XmlElement("")]
    public string YouTubeFeedTitle;

    public YouTubeWidgetData()
    {
        YouTubeFeedURL = "";
        YouTubeFeedCount = 15;
        YouTubeFeedTitle = "";
    }

    public static YouTubeWidgetData LoadFromSettings(string settings)
    {
        XmlSerializer xmlSerializer = new XmlSerializer(typeof(YouTubeWidgetData));
        YouTubeWidgetData retVal;
        byte[] data = ASCIIEncoding.Default.GetBytes(settings);
        using (MemoryStream memStream = new MemoryStream(data))
        {
            try
            {
                retVal = (YouTubeWidgetData)xmlSerializer.Deserialize(memStream);
            }
            catch
            {
                retVal = new YouTubeWidgetData();
            }
        }

        return retVal;
    }

    public string ToXml()
    {
        XmlSerializer xmlSerializer = new XmlSerializer(typeof(YouTubeWidgetData));
        string settings = "";

        using (MemoryStream memStream = new MemoryStream())
        {
            xmlSerializer.Serialize(memStream, this);
            byte[] data = memStream.ToArray();
            settings = ASCIIEncoding.UTF8.GetString(data);
        }

        return settings;
    }

}

public class YouTubeFeedPair
{
    public string FeedName;
    public string FeedUrl;

    public YouTubeFeedPair(string name, string url)
    {
        FeedName = name;
        FeedUrl = url;
    }
}

public partial class Widgets_YouTube : System.Web.UI.UserControl, IWidget
{
    #region properties
    #endregion

    List<YouTubeFeedPair> feeds;
    YouTubeWidgetData _data;
    IWidgetHost _host;

    protected void Page_Init(object sender, EventArgs e)
    {
        _host = WidgetHost.GetHost(this);
        _host.Minimize += new MinimizeDelegate(delegate() { Visible = false; });
        _host.Maximize += new MaximizeDelegate(delegate() { Visible = true; });
        _host.Edit += new EditDelegate(EditEvent);
        _host.Title = "YouTube";
        MainView();
    }

    protected void GenerateFeedList()
    {
        string searchURL;
        string searchquery="";
        string[] searchterms = KeywordSearchTextBox.Text.Split(' ');
        for (int x = 0; x < searchterms.Length; x++)
        {
            searchquery += searchterms[x];
        } 
        
        //searches on tags, title, keywords in content. 
        searchURL = "http://gdata.youtube.com/feeds/api/videos?q=" + searchquery;
                
        //Feeds (Atom) found here http://code.google.com/apis/youtube/developers_guide_protocol.html
        feeds = new List<YouTubeFeedPair>();
        feeds.Add(new YouTubeFeedPair("Top Rated", "http://gdata.youtube.com/feeds/api/standardfeeds/top_rated"));
        feeds.Add(new YouTubeFeedPair("Top Favorites", "http://gdata.youtube.com/feeds/api/standardfeeds/top_favorites"));
        feeds.Add(new YouTubeFeedPair("Most Viewed", "http://gdata.youtube.com/feeds/api/standardfeeds/most_viewed"));
        feeds.Add(new YouTubeFeedPair("Most Popular", "http://gdata.youtube.com/feeds/api/standardfeeds/most_popular"));
        feeds.Add(new YouTubeFeedPair("Most Recent", "http://gdata.youtube.com/feeds/api/standardfeeds/most_recent"));
        feeds.Add(new YouTubeFeedPair("Most Discussed", "http://gdata.youtube.com/feeds/api/standardfeeds/most_discussed"));
        feeds.Add(new YouTubeFeedPair("Most Linked", "http://gdata.youtube.com/feeds/api/standardfeeds/most_linked"));
        feeds.Add(new YouTubeFeedPair("Most Responded", "http://gdata.youtube.com/feeds/api/standardfeeds/most_responded"));
        feeds.Add(new YouTubeFeedPair("Recently Featured", "http://gdata.youtube.com/feeds/api/standardfeeds/recently_featured"));
        feeds.Add(new YouTubeFeedPair("Search", searchURL));
    }

    void EditEvent(string settings)
    {
        try
        {
            _host.Title = "YouTube";
            GenerateFeedList();
            Repeater2.DataSource = feeds.ToArray();
            Repeater2.DataBind();
            ViewSet.SetActiveView(Edit);
        }
        catch
        {
            ViewSet.SetActiveView(View);
        }
    }

    protected void Repeater2_ItemCommand(object source, RepeaterCommandEventArgs e)
    {
        try
        {
            GenerateFeedList();
            _data = YouTubeWidgetData.LoadFromSettings(WidgetHost.GetHost(this).WidgetInfo.Settings);
            _data.YouTubeFeedURL = feeds[e.Item.ItemIndex].FeedUrl;
            _data.YouTubeFeedTitle = feeds[e.Item.ItemIndex].FeedName;
            
            WidgetHost.GetHost(this).Save(_data.ToXml());
            MainView();
        }
        catch
        {
            ViewSet.SetActiveView(View);
        }
    }

    protected void MainView()
    {
        try
        {
            _data = YouTubeWidgetData.LoadFromSettings(_host.WidgetInfo.Settings);
            if (!_data.YouTubeFeedURL.Equals(""))
            {
                List<YouTubeWidgetRepeaterData> dataItems = new List<YouTubeWidgetRepeaterData>();
                XmlDocument xmldoc = new XmlDocument();
                int count = 0;

                lblData.Text = @"<style>.YouTube img{max-width:100%;} .YouTube embed{max-width:100%;}</style>";
                WebRequest request; 
                if (_data.YouTubeFeedTitle == "Search") //search URL already has parameter using ? so &max-result has to be used. 
                {
                    request = WebRequest.Create(_data.YouTubeFeedURL + @"&max-results=" + _data.YouTubeFeedCount.ToString());
                }
                else
                {
                    request = WebRequest.Create(_data.YouTubeFeedURL + @"?max-results=" + _data.YouTubeFeedCount.ToString());
                }

                _host.Title = "YouTube : " + _data.YouTubeFeedTitle;
                request.Timeout = 5000;
                HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                Stream resStream = response.GetResponseStream();
                xmldoc.Load(resStream);
                XmlNode root = xmldoc.FirstChild;
                while (root.LocalName != "feed")
                {
                    root = root.NextSibling;
                }
                XmlNodeList feedNodes = root.ChildNodes;
                foreach (XmlNode tempNode in feedNodes)
                {
                    if (tempNode.Name.Equals("entry"))
                    {
                        YouTubeWidgetRepeaterData temp = new YouTubeWidgetRepeaterData();
                        XmlNodeList entryNodes = tempNode.ChildNodes;
                        foreach (XmlNode entryItem in entryNodes)
                        {
                            if (entryItem.Name.Equals("content"))
                            {
                                string contentFeed = entryItem.InnerText;

                                // truncate the feed by x number of words
                                string[] truncateTheFeed = contentFeed.Split(' ');
                                string truncatedContentFeed = "";
                                int x = 0;

                                foreach (string s in truncateTheFeed)
                                {
                                    if (x < 20)
                                    {
                                        truncatedContentFeed += " " + s;
                                        x++;
                                    }
                                }

                                //then shorten any URLs found in the feed
                                string pattern = "c";

                                if (Regex.IsMatch(truncatedContentFeed, pattern))
                                {
                                    foreach (Match match in Regex.Matches(truncatedContentFeed, pattern))
                                    {
                                        if (match.Length > 20)
                                        {
                                            string newmatch = "<a href=\"" + match + "\">" + match.ToString().Substring(0, 20) + "...</a>";
                                            truncatedContentFeed = truncatedContentFeed.Replace(match.ToString(), newmatch);
                                        }
                                    }
                                }

                                temp.content = truncatedContentFeed + "...";
                            }
                            else if (entryItem.Name.Equals("summary"))
                            {
                                temp.summary = entryItem.InnerText;
                            }
                            else if (entryItem.Name.Equals("title"))
                            {
                                temp.contentTitle = entryItem.InnerText;
                            }
                            else if (entryItem.Name.Equals("media:group"))
                            {
                                XmlNodeList mediaNodes = entryItem.ChildNodes;
                                bool foundMediaSrc = false;
                                foreach (XmlNode media in mediaNodes)
                                {
                                    if (media.Name.Equals("media:content") && !foundMediaSrc)
                                    {
                                        XmlAttributeCollection attrs = media.Attributes;
                                        string tempSrc = "";
                                        foreach (XmlAttribute attr in attrs)
                                        {
                                            if (attr.Name.Equals("url"))
                                            {
                                                tempSrc = attr.Value;
                                            }
                                            else if (attr.Name.Equals("type") && attr.Value.Equals("application/x-shockwave-flash"))
                                            {
                                                foundMediaSrc = true;
                                            }
                                        }
                                        if (foundMediaSrc)
                                        {
                                            temp.mediaSrc = tempSrc;
                                        }
                                    }
                                }
                            }
                            else if (entryItem.Name.Equals("link"))
                            {
                                XmlAttributeCollection atts = entryItem.Attributes;
                                string tempUrl = "";
                                bool isCorrectUrl = false;
                                foreach (XmlAttribute att in atts)
                                {
                                    if (att.Name.Equals("href"))
                                    {
                                        tempUrl = att.Value;
                                    }
                                    if (att.Name.Equals("rel") && att.Value.Equals("alternate"))
                                    {
                                        isCorrectUrl = true;
                                    }
                                }
                                if (isCorrectUrl)
                                {
                                    temp.url = tempUrl;
                                }
                            }
                        }
                        if (temp.content == string.Empty)
                        {
                            temp.content = temp.summary;
                        }
                        if (!temp.mediaSrc.Equals(""))
                        {
                            temp.content += @"<object id=""youtube" + _host.WidgetInfo.ID.ToString() + @"widget" + count.ToString() + @""" classid=""clsid:D27CDB6E-AE6D-11cf-96B8-444553540000"" codebase=""http://download.macromedia.com/pub/shockwave/cabs/flash/swflash.cab#version=6,0,0,0"" width=""100%"" id=""movie"" align=""""><param name=""movie"" value=""" + temp.mediaSrc + @".swf""><embed quality=""high"" width=""100%"" name=""movie"" align="""" type=""application/x-shockwave-flash"" src=""" + temp.mediaSrc + @".swf"" plug inspage=""http://www.macromedia.com/go/getflashplayer""></object>";
                        }
                        temp.hostid = _host.WidgetInfo.ID.ToString();
                        temp.count = count.ToString();
                        dataItems.Add(temp);
                        count++;
                    }
                }
                Repeater1.DataSource = dataItems.ToArray();
                Repeater1.DataBind();
                Repeater1.Visible = true;
                if (_data.YouTubeFeedCount <= 1)
                {
                    LinkButton3.Visible = false;
                }
                else
                {
                    LinkButton3.Visible = true;
                }
                LinkButton2.Visible = true;
            }
            else
            {
                lblData.Text = "No YouTube Feed Selected";
                LinkButton2.Visible = false;
                LinkButton3.Visible = false;
            }
        }
        catch
        {
            lblData.Text = "YouTube Feed Error";
            Repeater1.Visible = false;
        }

        if (_data.YouTubeFeedCount <= 1)
        {
            LinkButton3.Visible = false;
        }
        ViewSet.SetActiveView(View);
    }

    protected void LinkButton2_Click(object sender, EventArgs e)
    {
        _data = YouTubeWidgetData.LoadFromSettings(WidgetHost.GetHost(this).WidgetInfo.Settings);
        _data.YouTubeFeedCount++;
        WidgetHost.GetHost(this).Save(_data.ToXml());
        MainView();
    }
    protected void LinkButton3_Click(object sender, EventArgs e)
    {
        _data = YouTubeWidgetData.LoadFromSettings(WidgetHost.GetHost(this).WidgetInfo.Settings);
        _data.YouTubeFeedCount--;
        WidgetHost.GetHost(this).Save(_data.ToXml());
        MainView();
    }
}

public class YouTubeWidgetRepeaterData
{
    public string hostid;
    public string count;
    public string content;
    public string url;
    public string contentTitle;
    public string summary;
    public string mediaSrc;

    public YouTubeWidgetRepeaterData()
    {
        hostid = "";
        count = "";
        content = "";
        url = "";
        contentTitle = "";
        summary = "";
        mediaSrc = "";
    }
}