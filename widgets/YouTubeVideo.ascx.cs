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

public partial class widgets_YouTubeVideo : System.Web.UI.UserControl, IWidget
{
    #region properties


    public string appPath;
    public string _VideoID;
    [WidgetDataMember("")]
    public string VideoID { get { return _VideoID; } set { _VideoID = value; } }

    public string MovieSourceURL
    {
        get
        {
            return string.Format("http://www.youtube.com/v/{0}?f=videos&app=youtube_gdata&autoplay=0", VideoID);
        }
    }

    #endregion



    IWidgetHost _host;



    protected void Page_Init(object sender, EventArgs e)
    {
        _host = Ektron.Cms.Widget.WidgetHost.GetHost(this);
        _host.Title = "YouTubeVideo Widget";
        _host.Edit += new EditDelegate(EditEvent);
        _host.Create += new CreateDelegate(delegate() { EditEvent(""); });
        PreRender += new EventHandler(delegate(object PreRenderSender, EventArgs Evt) { SetOutput(); });
        Ektron.Cms.CommonApi _api = new Ektron.Cms.CommonApi();
        Ektron.Cms.API.JS.RegisterJSInclude(tbData, Ektron.Cms.API.JS.ManagedScript.EktronJS);
        Ektron.Cms.API.JS.RegisterJSInclude(tbData, _api.SitePath + "widgets/YouTubeVideo/js/YouTubeVideo.js", "EktronWidgetYouTubeJS");
        Ektron.Cms.API.Css.RegisterCss(tbData, _api.SitePath + "widgets/YouTubeVideo/css/YouTubeVideo.css", "YouTubecss");
        appPath = _api.AppPath;
        ViewSet.SetActiveView(View);


    }

    void EditEvent(string settings)
    {
        //Ektron.Cms.API.JS.RegisterJSBlock(tbData, "Ektron.Widget.YouTubeVideo.AddWidget('" + this.ClientID + "','" + token + "', '" + tbData.ClientID + "', '" + SaveButton.ClientID + "');", "jsblock" + this.ClientID);
        Ektron.Cms.API.JS.RegisterJSBlock(tbData, "Ektron.Widget.YouTubeVideo.AddWidget('" + this.ClientID + "', '" + tbData.ClientID + "', '" + SaveButton.ClientID + "');", "jsblock" + this.ClientID);
        ViewSet.SetActiveView(Edit);
    }

    protected void SaveButton_Click(object sender, EventArgs e)
    {
        VideoID = tbData.Text;
        _host.SaveWidgetDataMembers();
        ViewSet.SetActiveView(View);
    }

    protected void SetOutput()
    {
        if (!string.IsNullOrEmpty(VideoID))
        {
            phContent.Visible = true;
            phHelpText.Visible = false;
        }
        else
        {
            phContent.Visible = false;
            phHelpText.Visible = true;
        }


        if (!(_host == null || _host.IsEditable == false))
        {
            divHelpText.Visible = true;
        }
        else
        {
            divHelpText.Visible = false;
        }
    }

    protected void CancelButton_Click(object sender, EventArgs e)
    {
        ViewSet.SetActiveView(View);
    }
}








