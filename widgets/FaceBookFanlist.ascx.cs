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


public partial class FaceBookFanlist : System.Web.UI.UserControl, IWidget
{
    #region properties
    private string _ApiKey;
    [WidgetDataMember("")]
    public string ApiKey { get { return _ApiKey; } set { _ApiKey = value; } }

    private string _ProfileID;
    [WidgetDataMember("")]
    public string ProfileID { get { return _ProfileID; } set { _ProfileID = value; } }

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
        _host.Title = "Face Book Fan List";
        _host.Edit += new EditDelegate(EditEvent);
        _host.Maximize += new MaximizeDelegate(delegate() { Visible = true; });
        _host.Minimize += new MinimizeDelegate(delegate() { Visible = false; });
        _host.Create += new CreateDelegate(delegate() { EditEvent(""); });
        ScriptManager.RegisterClientScriptInclude(this, this.GetType(), "FaceBookFanListWidgetInit", sitePath + "widgets/FacebookFanList/featureloader.js");
        PreRender += new EventHandler(delegate(object PreRenderSender, EventArgs Evt) { SetOutput(); });
        ViewSet.SetActiveView(View);
    }

    void EditEvent(string settings)
    {
        JS.RegisterJSInclude(this, sitePath + "widgets/FacebookFanList/FacebookFanList.js", "FaceBookLiveStreamJS");
        uxApiKey.Text = ApiKey;
        uxProfileID.Text = ProfileID;
        uxWidth.Text = Width;
        uxHeight.Text = Height;
        ViewSet.SetActiveView(Edit);

    }

    protected void SaveButton_Click(object sender, EventArgs e)
    {
        ApiKey = uxApiKey.Text;
        ProfileID = uxProfileID.Text;
        Width = uxWidth.Text;
        Height = uxHeight.Text;
        _host.SaveWidgetDataMembers();
        ViewSet.SetActiveView(View);
    }

    protected void SetOutput()
    {

        if (!(string.IsNullOrEmpty(ApiKey) || string.IsNullOrEmpty(ProfileID)))
        {
              
            string jsVal = string.Format(@"FB.init(""{0}"");", ApiKey);
            string strVal = string.Format(@"<fb:fan profile_id=""{0}"" stream=""1"" connections=""10"" logobar=""1"" width=""{1}"" height=""{2}""></fb:fan>", ProfileID, Width, Height);

            Ektron.Cms.API.JS.RegisterJSBlock(this, jsVal, "FacebookFanListJSInit");

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

