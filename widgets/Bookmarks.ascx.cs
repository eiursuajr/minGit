using System;
using System.Data;
using System.Configuration;
using System.Collections;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using System.Xml;
using System.Xml.Serialization;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Net;
using Ektron.Cms.Widget;

[XmlRoot("Bookmarks")]
public class Bookmarks
{
    [XmlElement("LinkTitlePairs")]
    public List<LinkTitlePair> LinkTitlePairs;

    public Bookmarks()
    {
        LinkTitlePairs = new List<LinkTitlePair>();
    }

    public static Bookmarks LoadFromSettings(string settings)
    {
        XmlSerializer xmlSerializer = new XmlSerializer(typeof(Bookmarks));
        Bookmarks retVal;
        byte[] data = ASCIIEncoding.Default.GetBytes(settings);
        using (MemoryStream memStream = new MemoryStream(data))
        {
            try
            {
                retVal = (Bookmarks)xmlSerializer.Deserialize(memStream);
            }
            catch
            {
                retVal = new Bookmarks();
            }
        }

        return retVal;
    }

    public string ToXml()
    {
        XmlSerializer xmlSerializer = new XmlSerializer(typeof(Bookmarks));
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

[XmlRoot("LinkTitlePair")]
public class LinkTitlePair
{
    [XmlElement("LinkHref")]
    public string LinkHref;

    [XmlElement("LinkTitle")]
    public string LinkTitle;

    public LinkTitlePair()
    {
        LinkHref = "";
        LinkTitle = "";
    }

    public LinkTitlePair(string href, string title)
    {
        LinkHref = href;
        LinkTitle = title;
    }
}

public partial class Widgets_Bookmarks : System.Web.UI.UserControl, IWidget
{
    private Bookmarks _data;
    private IWidgetHost _host;

    protected void Page_Init(object sender, EventArgs e)
    {
        _host = WidgetHost.GetHost(this);

        _host.Edit += new EditDelegate(EditEvent);
        _host.Minimize += new MinimizeDelegate(MinimizeEvent);
        _host.Maximize += new MaximizeDelegate(MaximizeEvent);
        _host.Create += new CreateDelegate(CreateEvent);
        _host.Title = Server.HtmlEncode("Bookmarks");

        _data = Bookmarks.LoadFromSettings(_host.WidgetInfo.Settings);

        MainView();
    }

    void CreateEvent()
    {
        EditEvent(_host.WidgetInfo.Settings);
    }

    protected void MainView()
    {
        TickerRepeater.DataSource = _data.LinkTitlePairs.ToArray() ;
        TickerRepeater.DataBind();

        ViewSet.SetActiveView(View);
    }

    void EditEvent(string settings)
    {
        try
        {
            ScriptManager.RegisterClientScriptBlock(this, this.GetType(), @"WidgetEnterCheck" + _host.WidgetInfo.ID.ToString(), "<script type='text/javascript'>function checkForEnter" + _host.WidgetInfo.ID.ToString() + @"(e, saveButtonID){var evt = e ? e : window.event;if(evt.keyCode == 13){document.forms[0].onsubmit = function () { return false; }; evt.cancelBubble = true; if (evt.stopPropagation) evt.stopPropagation(); return false; }}</script>", false);
            newLinkURL.Attributes.Add("onkeydown", @"javascript:checkForEnter" + _host.WidgetInfo.ID.ToString() + @"(event, '" + AddBookmarkButton.ClientID + "')");
            newLinkTitle.Attributes.Add("onkeydown", @"javascript:checkForEnter" + _host.WidgetInfo.ID.ToString() + @"(event, '" + AddBookmarkButton.ClientID + "')");
            _data = Bookmarks.LoadFromSettings(settings);
            Repeater1.DataSource = _data.LinkTitlePairs.ToArray();
            Repeater1.DataBind();
            ViewSet.SetActiveView(Edit);
        }
        catch
        {
            ViewSet.SetActiveView(View);
        }
    }

    void MinimizeEvent()
    {
        Visible = false;
    }

    void MaximizeEvent()
    {
        Visible = true;
    }

    protected void AddBookmarkButton_Click(object sender, EventArgs e)
    {
        try
        {
            _data = Bookmarks.LoadFromSettings(WidgetHost.GetHost(this).WidgetInfo.Settings);

            if (!newLinkURL.Text.Substring(0, 4).Equals("http"))
            {
                newLinkURL.Text = "http://" + newLinkURL.Text;
            }

            if (newLinkTitle.Text != "")
            {
                _data.LinkTitlePairs.Add(new LinkTitlePair(newLinkURL.Text, newLinkTitle.Text));
                Repeater1.DataSource = _data.LinkTitlePairs.ToArray();
                Repeater1.DataBind();
                WidgetHost.GetHost(this).Save(_data.ToXml());
            }
            newLinkURL.Text = "";
            newLinkTitle.Text = "";
        }
        catch
        {

        }
    }

    protected void Repeater1_ItemCommand(object source, RepeaterCommandEventArgs e)
    {
        try
        {
            _data = Bookmarks.LoadFromSettings(WidgetHost.GetHost(this).WidgetInfo.Settings);
            _data.LinkTitlePairs.RemoveAt(e.Item.ItemIndex);
            WidgetHost.GetHost(this).Save(_data.ToXml());
            Repeater1.DataSource = _data.LinkTitlePairs.ToArray();
            Repeater1.DataBind();
        }
        catch
        {
            ViewSet.SetActiveView(View);
        }
    }

    protected void SaveButton_Click(object sender, EventArgs e)
    {
        MainView();
    }
	protected void CancelButton_Click(object sender, EventArgs e)
    {
        ViewSet.SetActiveView(View);
    }
}
