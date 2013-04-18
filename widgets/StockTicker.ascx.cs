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

[XmlRoot("StockWidgetData")]
public class StockWidgetData
{
    [XmlElement("Symbols")]
    public List<string> Symbols;

    public StockWidgetData()
    {
        Symbols = new List<string>();
    }

    public static StockWidgetData LoadFromSettings(string settings)
    {
        XmlSerializer xmlSerializer = new XmlSerializer(typeof(StockWidgetData));
        StockWidgetData retVal;
        byte[] data = ASCIIEncoding.Default.GetBytes(settings);
        using (MemoryStream memStream = new MemoryStream(data))
        {
            try
            {
                retVal = (StockWidgetData)xmlSerializer.Deserialize(memStream);
            }
            catch
            {
                retVal = new StockWidgetData();
            }
        }

        return retVal;
    }

    public string ToXml()
    {
        XmlSerializer xmlSerializer = new XmlSerializer(typeof(StockWidgetData));
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

public class StockQuote
{
    private string[] SymbolData;

    public string Symbol;
    public Double LastTradePrice;
    public DateTime LastTradeTime;
    public Double PriceChange;
    public Double OpenPrice;
    public Double DayHighPrice;
    public Double DayLowPrice;
    public Int32 DayVolume;
    public bool IsHigherPrice;
    public bool IsLowerPrice;
    public string spanTag;

    public StockQuote(string StockSymbol)
    {
        Symbol = StockSymbol;
        string url;  //stores url of yahoo quote engine
        string buffer;
        WebRequest webRequest;
        WebResponse webResponse;

        url = "http://download.finance.yahoo.com/d/quotes.csv?s=" + Symbol + "&f=sl1d1t1c1ohgv&e=.csv";

        webRequest = HttpWebRequest.Create(url);
        webResponse = webRequest.GetResponse();
        using (StreamReader sr = new StreamReader(webResponse.GetResponseStream()))
        {
            buffer = sr.ReadToEnd();
            sr.Close();
        }

        buffer = buffer.Replace("\"", "");
        SymbolData = buffer.Split(new char[] { ',' });

        LastTradePrice = Convert.ToDouble(SymbolData[1]);

        string temp = SymbolData[3] + SymbolData[2];

        try
        {
            LastTradeTime = Convert.ToDateTime(temp);
        }
        catch
        {
            LastTradeTime = new DateTime();
        }
        try
        {
            PriceChange = Convert.ToDouble(SymbolData[4]);
        }
        catch
        {
            PriceChange = 0;
        }
        try
        {
            OpenPrice = Convert.ToDouble(SymbolData[5]);
        }
        catch
        {
            OpenPrice = 0;
        }
        try
        {
            DayHighPrice = Convert.ToDouble(SymbolData[6]);
        }
        catch
        {
            DayHighPrice = 0;
        }
        try
        {
            DayLowPrice = Convert.ToDouble(SymbolData[7]);
        }
        catch
        {
            DayLowPrice = 0;
        }
        try
        {
            DayVolume = Convert.ToInt32(SymbolData[8]);
        }
        catch
        {
            DayVolume = 0;
        }

        if (PriceChange == 0)
        {
            IsHigherPrice = false;
            IsLowerPrice = false;
        }
        else if (PriceChange > 0)
        {
            IsHigherPrice = true;
            IsLowerPrice = false;
        }
        else
        {
            IsLowerPrice = true;
            IsHigherPrice = false;
        }

        if (IsHigherPrice)
        {
            spanTag = @"<span class=""Higher"">&#9650;";
        }
        else if (IsLowerPrice)
        {
            spanTag = @"<span class=""Lower"">&#9660;";
        }
        else
        {
            spanTag = @"<span class=""Same"">";
        }

    }
}

public partial class Widgets_StockTickerWidget : System.Web.UI.UserControl, IWidget
{
    StockWidgetData _data;
    IWidgetHost _host;

    protected void Page_Init(object sender, EventArgs e)
    {
        _host = WidgetHost.GetHost(this);

        _host.Edit += new EditDelegate(EditEvent);
        _host.Minimize += new MinimizeDelegate(MinimizeEvent);
        _host.Maximize += new MaximizeDelegate(MaximizeEvent);
        _host.Create += new CreateDelegate(CreateEvent);
        _host.Title = Server.HtmlEncode("Stock Ticker");

        MainView();
    }

    void CreateEvent()
    {
        EditEvent(_host.WidgetInfo.Settings);
    }

    protected void MainView()
    {
        try
        {
            _data = StockWidgetData.LoadFromSettings(_host.WidgetInfo.Settings);
            List<StockQuote> quotes = new List<StockQuote>();
            foreach (string symbol in _data.Symbols)
            {
                quotes.Add(new StockQuote(symbol));
            }

            TickerRepeater.DataSource = quotes.ToArray();
            TickerRepeater.DataBind();

            ViewSet.SetActiveView(View);
        }
        catch
        {
            lbData.Text = "Error loading widget";
            ViewSet.SetActiveView(View);
        }
    }

    void EditEvent(string settings)
    {
        try
        {
            ScriptManager.RegisterClientScriptBlock(this, this.GetType(), @"WidgetEnterCheck" + _host.WidgetInfo.ID.ToString(), "<script type='text/javascript'>function checkForEnter" + _host.WidgetInfo.ID.ToString() + @"(e, saveButtonID){var evt = e ? e : window.event;if(evt.keyCode == 13){document.forms[0].onsubmit = function () { return false; }; evt.cancelBubble = true; if (evt.stopPropagation) evt.stopPropagation(); return false; }}</script>", false);
            newSymbol.Attributes.Add("onkeydown", @"javascript:checkForEnter" + _host.WidgetInfo.ID.ToString() + @"(event, '" + AddStockButton.ClientID + "')");
            _data = StockWidgetData.LoadFromSettings(settings);
            Repeater1.DataSource = _data.Symbols.ToArray();
            Repeater1.DataBind();
            ViewSet.SetActiveView(Edit);
        }
        catch
        {
            lbData.Text = "Error loading settings";
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

    protected void AddStockButton_Click(object sender, EventArgs e)
    {
        try
        {
            if (newSymbol.Text != String.Empty)
            {
                _data = StockWidgetData.LoadFromSettings(WidgetHost.GetHost(this).WidgetInfo.Settings);
                _data.Symbols.Add(newSymbol.Text.ToUpper());
                newSymbol.Text = "";
                Repeater1.DataSource = _data.Symbols;
                Repeater1.DataBind();
                WidgetHost.GetHost(this).Save(_data.ToXml());
            }
        }
        catch
        {
            lbData.Text = "Error adding stock";
            ViewSet.SetActiveView(View);
        }
    }

    protected void Repeater1_ItemCommand(object source, RepeaterCommandEventArgs e)
    {
        try
        {
            _data = StockWidgetData.LoadFromSettings(WidgetHost.GetHost(this).WidgetInfo.Settings);
            _data.Symbols.RemoveAt(e.Item.ItemIndex);
            WidgetHost.GetHost(this).Save(_data.ToXml());
            Repeater1.DataSource = _data.Symbols;
            Repeater1.DataBind();
        }
        catch
        {
            lbData.Text = "Error deleting widget";
            ViewSet.SetActiveView(View);
        }
    }

    protected void SaveButton_Click(object sender, EventArgs e)
    {
        MainView();
    }
	 protected void CancelButton_Click(object sender, EventArgs e)
    {
        MainView();
    }
}
