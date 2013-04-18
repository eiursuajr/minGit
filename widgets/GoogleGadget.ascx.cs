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


public partial class Widgets_GoogleGadget : System.Web.UI.UserControl, IWidget
{

    #region properties
    private string _Text;
    private long _Width;
    private long _Height;
    [WidgetDataMember("")]
    public string Text { get { return _Text; } set { _Text = value; } }
    [WidgetDataMember(100)]
    public long Width { get { return _Width; } set { _Width = value; } }
    [WidgetDataMember(100)]
    public long Height { get { return _Height; } set { _Height = value; } }
    #endregion

    IWidgetHost _host;

    protected void Page_Init(object sender, EventArgs e)
    {
        _host = Ektron.Cms.Widget.WidgetHost.GetHost(this);
        _host.Title = "Google Gadget Widget";
        _host.Edit += new EditDelegate(EditEvent);
        _host.Maximize += new MaximizeDelegate(delegate() { Visible = true; });
        _host.Minimize += new MinimizeDelegate(delegate() { Visible = false; });
        _host.Create += new CreateDelegate(delegate() { EditEvent(""); });
        PreRender += new EventHandler(delegate(object PreRenderSender, EventArgs Evt) { SetOutput(); });
        ViewSet.SetActiveView(View);

        string sitepath = new CommonApi().SitePath;
        ScriptManager.RegisterClientScriptInclude(this, this.GetType(), "GoogleGadget1", sitepath + "widgets/GoogleGadget/js/googlegadget.js");
        Ektron.Cms.API.Css.RegisterCss(this, sitepath + "widgets/GoogleGadget/css/googlegadget.css", "googlegadgetcss");
        ScriptManager.RegisterOnSubmitStatement(this.Page, this.GetType(), "gadgetescape" + "_"+tbData.ClientID, "GadgetEscapeHTML('" + tbData.ClientID + "');");
    }

    void EditEvent(string settings)
    {
        ScriptManager.RegisterClientScriptBlock(this, this.GetType(), @"WidgetEnterCheck" + _host.WidgetInfo.ID.ToString(), "<script type='text/javascript'>function checkForEnter" + _host.WidgetInfo.ID.ToString() + @"(e, saveButtonID){var evt = e ? e : window.event;if(evt.keyCode == 13){document.forms[0].onsubmit = function () { return false; }; evt.cancelBubble = true; if (evt.stopPropagation) evt.stopPropagation(); return false; }}</script>", false);
        tbData.Attributes.Add("onkeydown", @"javascript:checkForEnter" + _host.WidgetInfo.ID.ToString() + @"(event, '" + SaveButton.ClientID + "')");

        //Takes the stored value from the data object and puts it in the text box
        tbData.Text = "<script src=\"" + Text + "\"></script>";

        //string j = String.Join(",", Array.ConvertAll<GoogleGadgetData, string>(_gadgets, delegate (GoogleGadgetData gadget) { return gadget.ToJSON(); }));
        Ektron.Cms.API.JS.RegisterJSBlock(this, String.Format("Ektron.Widget.GoogleGadget.Init('{0}', new Array({1}));", 
                                                              this.ClientID, 
                                                              String.Join(",", Array.ConvertAll<GoogleGadgetData, string>(_gadgets, delegate (GoogleGadgetData gadget) { return gadget.ToJSON(); }))), 
                                                              "googlegadgetinit");
    

        ViewSet.SetActiveView(Edit);
    }

    protected void SaveButton_Click(object sender, EventArgs e)
    {
       Match url = Regex.Match(ReplaceEncodeBrackets(tbData.Text), "src=\"([^\"]*)\"");  // OnSubmit javascript encoded brackets, server side decode 
        if(!url.Success) return;

        Text = url.Groups[1].Value.Replace(";output=js", "");

        Match width = Regex.Match(Text, ";w=(\\d+)&", RegexOptions.Multiline | RegexOptions.IgnoreCase);
        if(width.Success) Width = long.Parse(width.Groups[1].Value);

        Match height = Regex.Match(Text, ";h=(\\d+)&", RegexOptions.Multiline | RegexOptions.IgnoreCase);
        if(height.Success) Height = long.Parse(height.Groups[1].Value);

     
        //Sets the title of the widget which will appear at the top of the widget
        _host.Title = "GoogleGadget";

        //Sets the value of the label to the stored value
        lblData.Text = String.Format("<iframe width=\"100%\" height=\"{1}\" frameborder=\"0\" scrollable=\"no\" src=\"{2}\"></iframe>", 
                       Width.ToString(), Height.ToString(), Text);

        _host.SaveWidgetDataMembers();
        ViewSet.SetActiveView(View);
    }

    protected void SetOutput()
    {
        //Sets the value of the label to the stored value
        lblData.Text = String.Format("<iframe width=\"100%\" height=\"{1}\" frameborder=\"0\" scrollable=\"no\" src=\"{2}\"></iframe>",
                       Width.ToString(), Height.ToString(), Text);
    }

    protected void CancelButton_Click(object sender, EventArgs e)
    {
        ViewSet.SetActiveView(View);
    }

    protected string ReplaceEncodeBrackets(string encodetext)
    {
        encodetext = Regex.Replace(encodetext, "&lt;", "<");
        encodetext = Regex.Replace(encodetext, "&gt;", ">");
        return encodetext;
    }

    private GoogleGadgetData[] _gadgets = new GoogleGadgetData[]
    {
        new GoogleGadgetData("Mini Web", 
                             "http://img0.gmodules.com/ig/modules/ov/module_miniweb.png",
                             "Displays a mini search which results in mini websites to be read on the personalized homepage.", 
                             "<script src=\"http://www.gmodules.com/ig/ifr?url=http://www.google.com/ig/modules/ajaxsearch.xml&amp;up_localResults=1&amp;up_loc=Mountain%20View%2C%20CA&amp;up_disableAddressLookup=1&amp;up_webResults=0&amp;up_videoResults=0&amp;up_blogResults=0&amp;up_initialSearch=&amp;up_color=default&amp;synd=open&amp;w=320&amp;h=400&amp;title=__MSG_title__&amp;lang=all&amp;country=ALL&amp;border=%23ffffff%7C3px%2C1px+solid+%23999999&amp;output=js\"></%73cript>"), 
        new GoogleGadgetData("Today in History", 
                             "http://img0.gmodules.com/ig/cache/4b/52/4b529510173736870c3b21a16018e132.png",
                             "Expand your knowledge with amazing facts from this day in History.", 
                            "<script src=\"http://www.gmodules.com/ig/ifr?url=http://sheetheart.googlepages.com/todayinhistory.xml&amp;synd=open&amp;w=450&amp;h=400&amp;title=Today+in+History&amp;border=%23ffffff%7C3px%2C1px+solid+%23999999&amp;output=js\"></%73cript>"),
       
    
        new GoogleGadgetData("Daily News", 
                            "http://img0.gmodules.com/ig/cache/02/ca/02cab65bfb5558b16b04638fba19808c-thm.png",                
                            "headlines from the best news sources all in one place.", 
                            "<script src=\"http://www.gmodules.com/ig/ifr?url=http://chronita.com/ugn/db-gadget.xml&amp;up_local=10001&amp;synd=open&amp;w=470&amp;h=280&amp;title=Daily+Briefing&amp;border=%23ffffff%7C3px%2C1px+solid+%23999999&amp;output=js\"></%73script>"),
    
     new GoogleGadgetData("Nasa Tv", 
                            "http://img0.gmodules.com/ig/cache/ed/5c/ed5c03f717d14718353933d95013c67d-thm.png",                
                            "Watch live television from NASA.", 
                            "<script src=\"http://www.gmodules.com/ig/ifr?url=http://www.ricardocarlier.nl/overig/nasatv.xml&amp;synd=open&amp;w=300&amp;h=310&amp;title=Nasa+TV+Live&amp;border=%23ffffff%7C3px%2C1px+solid+%23999999&amp;output=js\"></%73script>"),
   
     new GoogleGadgetData("Currency Converter", 
                            "http://img0.gmodules.com/ig/cache/0c/04/0c043910bd9ff3277a70427e8ee40507-thm.png",                
                            "Converts between all the major currencies in the world.", 
                            "<script src=\"http://www.gmodules.com/ig/ifr?url=http://www.donalobrien.net/apps/google/currency.xml&amp;up_def_from=USD&amp;up_def_to=EUR&amp;synd=open&amp;w=320&amp;h=250&amp;title=Currency+Converter&amp;border=%23ffffff%7C3px%2C1px+solid+%23999999&amp;output=js\"></%73script>"),
   

     new GoogleGadgetData("Google News", 
                             "http://img0.gmodules.com/ig/modules/tabnews_content/us-thm.png",
                             "Customizable news gadget that shows different news sections in separate tabs.", 
                             "<script src=\"http://www.gmodules.com/ig/ifr?url=http://www.google.com/ig/modules/tabnews.xml&amp;up_ned=&amp;up_items=6&amp;up_show_image=1&amp;up_font_size=12pt&amp;up_selectedTab=0&amp;up_tabs=&amp;up_last_url=&amp;up_onebox=&amp;synd=open&amp;w=320&amp;h=480&amp;title=__MSG_title__&amp;lang=en&amp;country=ZW&amp;border=%23ffffff%7C3px%2C1px+solid+%23999999&amp;output=js\"></%73cript>")
    
    
    };


}


public struct GoogleGadgetData
{
    public GoogleGadgetData(string title, string thumbnailUrl, string description, string embed)
    {
        this.Title        = title;
        this.ThumbnailURL = thumbnailUrl;
        this.Description  = description;
        this.Embed        = embed; 
    }

    /// <summary>
    /// Escapes a string for safe use in a JSON object
    /// </summary>
    /// <param name="value">Unsafe JSON input string.</param>
    /// <returns>Safe JSON input string.</returns>
    private static string EscapeJSON(string value)
    {
        return value.Replace("\\", "\\\\").Replace("\"", "\\\"");
    }

    /// <summary>
    /// Converts a GoogleGadgetData object to a JSON string
    /// </summary>
    /// <returns>A JSON string</returns>
    public string ToJSON()
    {
        return "{title:\"" + EscapeJSON(this.Title)        + "\", thumbnailUrl:\""+
                             EscapeJSON(this.ThumbnailURL) + "\", description:\"" +
                             EscapeJSON(this.Description)  + "\", embed:\""       + 
                             EscapeJSON(this.Embed)        + "\"}";
    }

    public string Title;
    public string ThumbnailURL;
    public string Description;
    public string Embed;
}


    