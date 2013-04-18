
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
using System.Net;


public partial class InternationalWidgets_Weather : System.Web.UI.UserControl, IWidget
{

    #region properties
    private string _Address;
    private string _units;
    [WidgetDataMember("")]
    public string unit { get { return _units; } set { _units = value; } }
    [WidgetDataMember("")]
    public string Address { get { return _Address; } set { _Address = value; } }
    #endregion

    IWidgetHost _host;
    private string weatherLocation = "http://weather.yahooapis.com/forecastrss?w=";

    protected void Page_Init(object sender, EventArgs e)
    {
        _host = Ektron.Cms.Widget.WidgetHost.GetHost(this);
        _host.Title = "Weather";
        _host.Edit += new EditDelegate(EditEvent);
        _host.Maximize += new MaximizeDelegate(delegate() { Visible = true; });
        _host.Minimize += new MinimizeDelegate(delegate() { Visible = false; });
        _host.Create += new CreateDelegate(delegate() { EditEvent(""); });
        _host.ExpandOptions = Expandable.ExpandOnEdit;
        PreRender += new EventHandler(delegate(object PreRenderSender, EventArgs Evt) { MainView(); });
        ViewSet.SetActiveView(View);
    }

    void EditEvent(string settings)
    {
        if (unit != string.Empty)
            unitsDropDown.SelectedValue = unit;
        hdnAddr.Text = Address;
        ViewSet.SetActiveView(Edit);
    }

    protected void SaveButton_Click(object sender, EventArgs e)
    {
        string selectedUnit = "Metric Units";
        if(unitsDropDown.SelectedIndex>-1)
            selectedUnit = unitsDropDown.SelectedValue;
        unit = selectedUnit;
        hdnAddr.Text = txtStreet.Text + "|" + txtCity.Text + "|" + txtState.Text + "|" + txtCountry.Text + "|" + txtZipCode.Text;
        Address = hdnAddr.Text;
        _host.SaveWidgetDataMembers();
        ViewSet.SetActiveView(View);
    }
   
    protected void MainView()
    {
        try
        {
            ScriptManager.RegisterClientScriptBlock(this, this.GetType(), @"WidgetEnterCheck" + _host.WidgetInfo.ID.ToString(), "<script type='text/javascript'>function checkForEnter" + _host.WidgetInfo.ID.ToString() + @"(e, saveButtonID){var evt = e ? e : window.event;if(evt.keyCode == 13){document.forms[0].onsubmit = function () { return false; }; evt.cancelBubble = true; if (evt.stopPropagation) evt.stopPropagation(); return false; }}</script>", false);
            hdnAddr.Attributes.Add("onkeydown", @"javascript:checkForEnter" + _host.WidgetInfo.ID.ToString() + @"(event, '" + SaveButton.ClientID + "')");
            if (Address == null || Address.Equals(""))
            {
                ViewSet.SetActiveView(Edit);
                _host.Title = Server.HtmlEncode("Weather");
            }
            else
            {
                
                lblData.Text = GetWeatherData(Address,unit);
                _host.Title = Server.HtmlEncode("Weather for " + Address);
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
        MainView();

    }

    public string GetWeatherData(string Address, string unit)
    {
        string woeid = string.Empty;
        string Latitude = string.Empty;
        string Longitude = string.Empty;
        string url = string.Empty;
        
        Cms.Extensions.GoogleGeoCoder.Helper.GetResults(Address, out Longitude, out Latitude, out woeid);
        string[] ad = Address.Split('|');

        txtStreet.Text = ad.Length > 0 && !string.IsNullOrEmpty(ad[0]) ? ad[0] : string.Empty;
        txtCity.Text = ad.Length > 1 && !string.IsNullOrEmpty(ad[1]) ? ad[1] : string.Empty;
        txtState.Text = ad.Length > 2 && !string.IsNullOrEmpty(ad[2]) ? ad[2] : string.Empty;
        txtCountry.Text = ad.Length > 3 && !string.IsNullOrEmpty(ad[3]) ? ad[3] : string.Empty;
        txtZipCode.Text = ad.Length > 4 && !string.IsNullOrEmpty(ad[4]) ? ad[4] : string.Empty;

        if (unit == "English Units")
        {
             url = weatherLocation + woeid;
            
            
        }
        else
        {
             url = weatherLocation + woeid + "&u=c";
             
        }
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





   