
using System;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using Ektron.Cms.Widget;
using Ektron.Cms;
using Ektron.Cms.API;
using Ektron.Cms.Common;
using Ektron.Cms.PageBuilder;
using System.Text.RegularExpressions;
using System.Xml;


public partial class Widgets_Weather : System.Web.UI.UserControl, IWidget
{

    #region properties
    private string _ZipCode;
    [WidgetDataMember("03063")]
    public string ZipCode { get { return _ZipCode; } set { _ZipCode = value; } }
    #endregion

    IWidgetHost _host;
    private string weatherLocation = "http://xml.weather.yahoo.com/forecastrss?p=";

    protected void Page_Init(object sender, EventArgs e)
    {
        _host = Ektron.Cms.Widget.WidgetHost.GetHost(this);
        _host.Title = "Weather";
        _host.Edit += new EditDelegate(EditEvent);
        _host.Maximize += new MaximizeDelegate(delegate() { Visible = true; });
        _host.Minimize += new MinimizeDelegate(delegate() { Visible = false; });
        _host.Create += new CreateDelegate(delegate() { EditEvent(""); });
        PreRender += new EventHandler(delegate(object PreRenderSender, EventArgs Evt) { MainView(); });
        ViewSet.SetActiveView(View);
    }

    void EditEvent(string settings)
    {

        tbData.Text = ZipCode;
        ViewSet.SetActiveView(Edit);
    }

    protected void SaveButton_Click(object sender, EventArgs e)
    {
                   
         ZipCode = tbData.Text;
        _host.SaveWidgetDataMembers();
        ViewSet.SetActiveView(View);
    }

   
    protected void MainView()
    {
        try
        {
            ScriptManager.RegisterClientScriptBlock(this, this.GetType(), @"WidgetEnterCheck" + _host.WidgetInfo.ID.ToString(), "<script type='text/javascript'>function checkForEnter" + _host.WidgetInfo.ID.ToString() + @"(e, saveButtonID){var evt = e ? e : window.event;if(evt.keyCode == 13){document.forms[0].onsubmit = function () { return false; }; evt.cancelBubble = true; if (evt.stopPropagation) evt.stopPropagation(); return false; }}</script>", false);
            tbData.Attributes.Add("onkeydown", @"javascript:checkForEnter" + _host.WidgetInfo.ID.ToString() + @"(event, '" + SaveButton.ClientID + "')");
            if (ZipCode == null || ZipCode.Equals(""))
            {
                ViewSet.SetActiveView(Edit);
                _host.Title = Server.HtmlEncode("Weather");
            }
            else
            {
                lblData.Text = GetWeatherData(ZipCode);
                _host.Title = Server.HtmlEncode("Weather for " + ZipCode);
            }
        }
        catch
        {
            lblData.Text = "Error loading widget";
            ViewSet.SetActiveView(View);
        }
    }

    protected void CancelButton_Click(object sender, EventArgs e)
    {
        ViewSet.SetActiveView(View);
    }

    public string GetWeatherData(string zipCode)
    {

        string url = weatherLocation + zipCode;

        XmlDocument doc = Cache[url] as XmlDocument ?? (new XmlDocument());
        try
        {
            if (!doc.HasChildNodes) doc.Load(url);
        }
        catch
        {
            return string.Empty;
        }
        if (null == Cache[url]) Cache[url] = doc;

        XmlElement root = doc.DocumentElement;
        XmlNodeList nodes = root.SelectNodes("/rss/channel/item");
        string data = "";
        foreach (XmlNode node in nodes)
        {
            data = data + node["title"].InnerText;
            data = data + node["description"].InnerText;
        }
        return data;
    }
}





   