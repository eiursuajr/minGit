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


public partial class FaceBookLiveStream : System.Web.UI.UserControl, IWidget
{
    #region properties
    private string _ApiKey;
    [WidgetDataMember("")]
    public string ApiKey { get { return _ApiKey; } set { _ApiKey = value; } }

    private string _ApplicationID;
    [WidgetDataMember("")]
    public string ApplicationID { get { return _ApplicationID; } set { _ApplicationID = value; } }

    private string _Width;
    [WidgetDataMember("100")]
    public string Width { get { return _Width; } set { _Width = value; } }

    private string _Height;
    [WidgetDataMember("100")]
    public string Height { get { return _Height; } set { _Height = value; } }

    #endregion

    IWidgetHost _host;
    protected string sitePath;
    protected string appPath;

    protected void Page_Init(object sender, EventArgs e)
    {
        //string sitepath = new CommonApi().SitePath;
         sitePath = new CommonApi().SitePath;
         appPath = new CommonApi().AppPath;
        _host = Ektron.Cms.Widget.WidgetHost.GetHost(this);
        _host.Title = "Facebook Live Stream";
        _host.Edit += new EditDelegate(EditEvent);
        _host.Maximize += new MaximizeDelegate(delegate() { Visible = true; });
        _host.Minimize += new MinimizeDelegate(delegate() { Visible = false; });
        _host.Create += new CreateDelegate(delegate() { EditEvent(""); });
        PreRender += new EventHandler(delegate(object PreRenderSender, EventArgs Evt) { SetOutput(); });
         ScriptManager.RegisterClientScriptInclude(this, this.GetType(), "FaceBookWidget", sitePath + "widgets/FacebookLiveStream/featureloader.js");
        
        ViewSet.SetActiveView(View);
    }

    void EditEvent(string settings)
    {
        JS.RegisterJSInclude(this, sitePath + "widgets/FacebookLiveStream/FacebookLiveStream.js", "FacebookLiveStreamJS");
        uxApiKey.Text = ApiKey;
        uxApplicationID.Text = ApplicationID;
        uxWidth.Text = Width;
        uxHeight.Text = Height;
        ViewSet.SetActiveView(Edit);

    }

    protected void SaveButton_Click(object sender, EventArgs e)
    {
        ApiKey = uxApiKey.Text;
        ApplicationID = uxApplicationID.Text;
        Width = uxWidth.Text;
        Height = uxHeight.Text;
        _host.SaveWidgetDataMembers();
        ViewSet.SetActiveView(View);
    }

    protected void SetOutput()
    {
       
        
        if (!(string.IsNullOrEmpty(ApiKey) || string.IsNullOrEmpty(ApplicationID)))
        {
            string jsVal = string.Format(@"FB.init(""{0}"");", ApiKey);
            string strVal = string.Format(@"<fb:live-stream event_app_id=""{0}"" xid=""YOUR_EVENT_XID""
                                          width=""{1}"" height=""{2}""></fb:live-stream>", ApplicationID,Width, Height);

            Ektron.Cms.API.JS.RegisterJSBlock(this, jsVal, "FacebookLiveStreamJSInit");
            uxFacebook.Text = strVal;
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

