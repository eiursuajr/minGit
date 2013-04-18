#region Namespaces
using System;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using System.Collections.Generic;
using Ektron.Cms.Widget;
using Ektron.Cms;
using Ektron.Cms.API;
using Ektron.Cms.Common;
using Ektron.Cms.Controls.CmsWebService;
using System.Data;
#endregion


public class TwitterDataSet
{

    private string name;
    public string Name
    {
        get { return name; }
        set { name = value; }
    }

    private string url;
    public string URL
    {
        get { return url; }
        set { url = value; }
    }


}


public partial class widgets_TwitterFeed : System.Web.UI.UserControl, IWidget
{
    #region Members
    private List<TwitterDataSet> _tds = new List<TwitterDataSet>();
    IWidgetHost _host;
    protected CommonApi _api;
    protected string appPath;
    protected int langType;
    protected string uniqueId;
    protected string _TwitterParams;
    #endregion

    #region Properties    
    public List<TwitterDataSet> tds
    {
        get
        {
            return _tds;
        }
        set
        {
            _tds = value;
        }
    }
    [WidgetDataMember]
    public string TwitterParams { get { return _TwitterParams; } set { _TwitterParams = value; } }
    #endregion

    #region Events
    protected void Page_Init(object sender, EventArgs e)
    {
        _host = Ektron.Cms.Widget.WidgetHost.GetHost(this);
        _host.Title = "Twitter Feed Widget";
        _host.Edit += new EditDelegate(EditEvent);
        _host.Maximize += new MaximizeDelegate(delegate() { Visible = true; });
        _host.Minimize += new MinimizeDelegate(delegate() { Visible = false; });
        _host.Create += new CreateDelegate(delegate() { EditEvent(""); });
        _api = new Ektron.Cms.CommonApi();
        _host.HelpFile = _api.SitePath + "widgets/twitterFeed/Help/TwitterFeedWidgethelp.html";

        FormTwitterList(TwitterParams);
        RegisterCSSandJS();
        ViewSet.SetActiveView(View);

    }
    protected void repFriends_ItemDataBound(object sender, RepeaterItemEventArgs e)
    {
        LinkButton btnDelete = e.Item.FindControl("btnDelete") as LinkButton;
        LinkButton _btnUp = e.Item.FindControl("btnUp") as LinkButton;
        LinkButton _btnDown = e.Item.FindControl("btnDown") as LinkButton;

        if (e.Item.ItemIndex == -1)
        {

        }
        else if (e.Item.ItemIndex == 0)
        {

            _btnUp.Enabled = false;
            _btnUp.CssClass = "PreferenceListUpDisabled";
            if (tds.Count == 1)
            {
                _btnUp.Visible = false;
                _btnDown.Visible = false;

            }
        }
        else if (e.Item.ItemIndex == tds.Count - 1)
        {

            _btnDown.Enabled = false;
            _btnDown.CssClass = "PreferenceListDownDisabled";
        }
    }
    protected void repFriends_ItemCommand(object source, RepeaterCommandEventArgs e)
    {


        TwitterDataSet t_up;
        TwitterDataSet t_down;

        if (tds.Count > 0)
        {

            switch (e.CommandName)
            {
                case "btnDelete":

                    tds.RemoveAt(e.Item.ItemIndex);

                    break;

                case "up":


                    t_up = tds[e.Item.ItemIndex - 1];
                    t_down = tds[e.Item.ItemIndex];

                    tds[e.Item.ItemIndex] = t_up;
                    tds[e.Item.ItemIndex - 1] = t_down;



                    break;


                case "down":

                    t_up = tds[e.Item.ItemIndex];
                    t_down = tds[e.Item.ItemIndex + 1];

                    tds[e.Item.ItemIndex] = t_down;
                    tds[e.Item.ItemIndex + 1] = t_up;



                    break;


            }
        }

        FormTwitterString(tds);
        UpdateFriends();

    }
    protected void btnAdd_Click(object sender, EventArgs e)
    {
        HtmlInputControl feed = Edit.FindControl("feed") as HtmlInputControl;
        HtmlInputControl url = Edit.FindControl("url") as HtmlInputControl;
        TwitterDataSet t = new TwitterDataSet();
        t.Name = feed.Value;
        t.URL = url.Value;
        if (t.URL != "")
        {
            tds.Add(t);
            feed.Value = "";
            url.Value = "";
        }
        FormTwitterString(tds);
        UpdateFriends();
        _host.SaveWidgetDataMembers();
        ViewSet.SetActiveView(Edit);
    }
    protected void CancelButton_Click(object sender, EventArgs e)
    {
        FormTwitterList(TwitterParams);
        _host.SaveWidgetDataMembers();
        ViewSet.SetActiveView(View);
        Response.Redirect(Request.RawUrl.ToString());
    }
    protected void SaveButton_Click(object sender, EventArgs e)
    {
        FormTwitterList(TwitterParams);
        _host.SaveWidgetDataMembers();
        ViewSet.SetActiveView(View);
        Response.Redirect(Request.RawUrl.ToString());
    }
    #endregion

    #region Helpers
    void UpdateFriends()
    {
        FormTwitterList(TwitterParams);
        repFriends.DataSource = tds;
        repFriends.DataBind();
        _host.SaveWidgetDataMembers();
    }

    void EditEvent(string settings)
    {

        try
        {

            UpdateFriends();
            ViewSet.SetActiveView(Edit);

        }
        catch
        {
            ViewSet.SetActiveView(View);
        }
    }

   
    protected void FormTwitterString(List<TwitterDataSet> t)
    {

        string str = "";
        if (t.Count > 0)
        {
            foreach (TwitterDataSet var in t)
            {
                str += var.Name + "," + var.URL + "#";
            }
            str = str.Remove(str.Length - 1, 1);
        }
        TwitterParams = str;
        tbData.Value = TwitterParams;

    }
    protected void FormTwitterList(string str)
    {
        tds.Clear();
        TwitterDataSet td;
        if (str != null)
        {
            List<string> tList = new List<string>(str.Split(new char[] { '#' }));

            foreach (string var in tList)
            {
                List<string> temp = new List<string>(var.Split(','));
                if (temp.Count == 2)
                {
                    td = new TwitterDataSet();
                    td.Name = temp[0];
                    td.URL = temp[1];
                    tds.Add(td);
                }

            }
        }
    }
    #endregion

    #region Register CSS/JS
    private void RegisterCSSandJS()
    {       
        // Register CSS and JSS 
        Css.RegisterCss(this, _api.SitePath + "widgets/TwitterFeed/Twitterwidget.css", "TwitterWidgetCSS");
        Css.RegisterCss(this, _api.SitePath + "widgets/ActivityStream/css/activityStream.css", "EktronAFWidgetCSS");
        JS.RegisterJS(this, JS.ManagedScript.EktronJS);
        JS.RegisterJSInclude(this, _api.SitePath + "widgets/TwitterFeed/TwitterFeed.js", "TwitterFeedJS");
        JS.RegisterJSBlock(this, "Ektron.Widgets.Twitter.LoadDynamicFeedControl('" + TwitterParams + "', '" + ClientID + "');", "EktronTwitterFeedInit" + ClientID);
    }

    #endregion
}
