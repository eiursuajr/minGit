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

public partial class Widgets_AtomFeed : System.Web.UI.UserControl, IWidget
{

    #region properties
    private string _AtomFeedURL;
    private int _NumberOfPosts;
    [WidgetDataMember(5)]
    public int NumberOfPosts { get { return _NumberOfPosts; } set { _NumberOfPosts = value; } }
    [WidgetDataMember("")]
    public string AtomFeedURL { get { return _AtomFeedURL; } set { _AtomFeedURL = value; } }
    #endregion

    IWidgetHost _host;

    protected void Page_Init(object sender, EventArgs e)
    {
        _host = Ektron.Cms.Widget.WidgetHost.GetHost(this);
        _host.Title = "AtomFeed Widget";
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
            CommonApi _api = new CommonApi();
            JS.RegisterJSInclude(this, JS.ManagedScript.EktronJS);
            JS.RegisterJSInclude(this, _api.SitePath + "widgets/AtomFeed/AtomFeed.js", "AtomFeedJS");

            ScriptManager.RegisterClientScriptBlock(this, this.GetType(), @"WidgetEnterCheck" + _host.WidgetInfo.ID.ToString(), "<script type='text/javascript'>function checkForEnter" + _host.WidgetInfo.ID.ToString() + @"(e, saveButtonID){var evt = e ? e : window.event;if(evt.keyCode == 13){document.forms[0].onsubmit = function () { return false; }; evt.cancelBubble = true; if (evt.stopPropagation) evt.stopPropagation(); return false; }}</script>", false);
            feedURL.Attributes.Add("onkeydown", @"checkForEnter" + _host.WidgetInfo.ID.ToString() + @"(event, '" + SaveButton.ClientID + "')");
            numPosts.Attributes.Add("onkeydown", @"checkForEnter" + _host.WidgetInfo.ID.ToString() + @"(event, '" + SaveButton.ClientID + "')");

            feedURL.Text = AtomFeedURL;
            numPosts.Text = NumberOfPosts.ToString();
        }
        catch
        {
            lblData.Text = "Error loading edit page";
        }
        ViewSet.SetActiveView(Edit);
    }

    protected void SaveButton_Click(object sender, EventArgs e)
    {
         try
        {
            
            AtomFeedURL = feedURL.Text;
            try
            {
                NumberOfPosts = Convert.ToInt32(numPosts.Text);
            }
            catch
            {
                NumberOfPosts = 5;
            }
            
        }
        catch
        {
            lblData.Text = "Error saving settings";
            ViewSet.SetActiveView(View);
        }
        _host.SaveWidgetDataMembers();
        ViewSet.SetActiveView(View);
    }

    protected void SetOutput()
    {
         try
        {
        
            List<AtomFeedWidgetRepeaterData> dataItems = new List<AtomFeedWidgetRepeaterData>();
            XmlDocument xmldoc = new XmlDocument();
            int count = 0;
            string blogTitle = "";
            string blogLink = "";
            bool titlePrinted = false;
            string blogTitleOriginal = "";

            if (AtomFeedURL != "")
            {
                lblData.Text = @"<style>.atomEntry img{max-width:100%;float:none;} .atomEntry embed{max-width:100%;float:none;}</style>";
                string url = AtomFeedURL.Trim();
                if (AtomFeedURL.IndexOf("?") == -1)
                {
                    url = url + "?max-results=" + NumberOfPosts.ToString();
                }
                else
                {
                    url = url + "&max-results=" + NumberOfPosts.ToString();
                }
                WebRequest request = WebRequest.Create(url);
                request.Timeout = 5000;
                HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                Stream resStream = response.GetResponseStream();
                xmldoc.Load(resStream);
                XmlNode root = xmldoc.FirstChild;
                while(root.LocalName != "feed"){
                    root = root.NextSibling;
                }
                XmlNodeList feedNodes = root.ChildNodes;
                foreach (XmlNode tempNode in feedNodes)
                {
                    if (NumberOfPosts > 0 && count >= NumberOfPosts)
                    {
                        break;
                    }
                    if (tempNode.Name.Equals("title"))
                    {
                        blogTitle = tempNode.InnerText;
                        blogTitleOriginal = blogTitle;
                    }
                    if (tempNode.Name.Equals("link"))
                    {
                        bool correctLink = false;
                        string tempLink = "";
                        foreach (XmlAttribute att in tempNode.Attributes)
                        {
                            if (att.Name.Equals("rel") && att.Value.Equals("alternate"))
                            {
                                correctLink = true;
                            }
                            if (att.Name.Equals("href"))
                            {
                                tempLink = att.Value;
                            }
                        }
                        if (correctLink)
                        {
                            blogLink = tempLink;
                        }
                    }
                    if (!titlePrinted)
                    {
                        if (blogTitle != "" && blogLink != "")
                        {
                            titlePrinted = true;
                            blogTitle = new Regex(@"\W*").Replace(blogTitle, string.Empty);
                        }
                    }
                    if (tempNode.Name.Equals("entry"))
                    {
                        AtomFeedWidgetRepeaterData temp = new AtomFeedWidgetRepeaterData();
                        XmlNodeList entryNodes = tempNode.ChildNodes;
                        foreach (XmlNode entryItem in entryNodes)
                        {
                            if (entryItem.Name.Equals("content"))
                            {
                                temp.content = entryItem.InnerText;
                            }
                            if(entryItem.Name.Equals("summary"))
                            {
                                temp.summary = entryItem.InnerText;
                            }
                            if (entryItem.Name.Equals("title"))
                            {
                                temp.contentTitle = entryItem.InnerText;
                            }
                            if (entryItem.Name.Equals("link"))
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
                        temp.hostid = _host.WidgetInfo.ID.ToString();
                        temp.count = count.ToString();
                        dataItems.Add(temp);
                        count++;
                    }
                }
                Repeater1.DataSource = dataItems.ToArray();
                Repeater1.DataBind();
            }
            else
            {
                lblData.Text = "No Feed Entered";
                blogTitleOriginal = "Atom Feed Widget";
                Repeater1.DataSource = null;
                Repeater1.DataBind();
            }
            _host.Title = blogTitleOriginal;
        }
        catch 
        {
            lblData.Text = "Invalid Feed URL";
            _host.Title = "Atom Feed Widget";
            Repeater1.DataSource = null;
            Repeater1.DataBind();
        }

    }

    protected void CancelButton_Click(object sender, EventArgs e)
    {
        ViewSet.SetActiveView(View);
    }

   

}


public class AtomFeedWidgetRepeaterData
{
    public string hostid;
    public string count;
    public string content;
    public string url;
    public string contentTitle;
    public string summary;

    public AtomFeedWidgetRepeaterData()
    {
        hostid = "";
        count = "";
        content = "";
        url = "";
        contentTitle = "";
        summary = "";
    }
}