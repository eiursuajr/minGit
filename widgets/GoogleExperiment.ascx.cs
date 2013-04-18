using System;
using System.Collections.Generic;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Ektron.Cms;
using Ektron.Cms.API;
using Ektron.Cms.Widget;
using Ektron.Cms.PageBuilder;
using System.Text.RegularExpressions;

public partial class widgets_GoogleExperiment : System.Web.UI.UserControl
{
    private IWidgetHost _host;

    #region Properties
    private string _ControlScript;
    private string _TrackingScript;

    [WidgetDataMember("")]
    public string ControlScript { get { return _ControlScript; } set { _ControlScript = value; } }

    [WidgetDataMember("")]
    public string TrackingScript { get { return _TrackingScript; } set { _TrackingScript = value; } }
    #endregion

    protected void Page_Init(object sender, EventArgs e)
    {
        _host = Ektron.Cms.Widget.WidgetHost.GetHost(this);
        _host.Title = "Google Experiment";
        _host.Maximize += new MaximizeDelegate(delegate() { Visible = true; });
        _host.Minimize += new MinimizeDelegate(delegate() { Visible = false; });
        _host.Edit += new EditDelegate(EditEvent);

        if ((Page as PageBuilder).Status != Mode.Editing &&
            _ControlScript != "" &&
            _TrackingScript != "")
        {
            Page.Header.Controls.Add(new LiteralControl(_ControlScript));
            Page.Controls.Add(new LiteralControl(_TrackingScript));
            litViewText.Visible = false;
        }
        else
        {
            litViewText.Visible = true;
        }

        string sitepath = new CommonApi().SitePath;
        imgControl.ImageUrl = sitepath + "widgets/GoogleExperiment/images/controlthumb.gif";
        imgControl.CssClass = "google-thumbnail";
        imgTracking.ImageUrl = sitepath + "widgets/GoogleExperiment/images/trackingthumb.gif";
        imgTracking.CssClass = "google-thumbnail";
        ScriptManager.RegisterClientScriptInclude(this, this.GetType(), "GoogleExperiment1", sitepath + "widgets/GoogleExperiment/js/googleexperiment.js");
        Css.RegisterCss(this, sitepath + "widgets/GoogleExperiment/css/googleexperiment.css", "googleexperimentcss");
        ScriptManager.RegisterOnSubmitStatement(this.Page, this.GetType(), "googleexperiment",
            String.Format("Ektron.Widget.GoogleExperiment.EscapeHTML('{0}');Ektron.Widget.GoogleExperiment.EscapeHTML('{1}');",
            tbControlScript.ClientID,
            tbTrackingScript.ClientID));

        ViewSet.SetActiveView(View);
    }

    void EditEvent(string settings)
    {
        _host.LoadWidgetDataMembers();

        tbControlScript.Text = ControlScript;
 	    tbTrackingScript.Text = TrackingScript;

        ViewSet.SetActiveView(Edit);
    }

    protected void  btnSave_Click(object sender, EventArgs e)
    {
        ControlScript = ReplaceEncodeBrackets(tbControlScript.Text);
        TrackingScript = ReplaceEncodeBrackets(tbTrackingScript.Text);
        _host.SaveWidgetDataMembers();

        ViewSet.SetActiveView(View);
    }

    protected void  btnCancel_Click(object sender, EventArgs e)
    {
        ViewSet.SetActiveView(View);
    }

    protected string ReplaceEncodeBrackets(string encodetext)
    {
        encodetext = Regex.Replace(encodetext, "&lt;", "<");
        encodetext = Regex.Replace(encodetext, "&gt;", ">");
        return encodetext;
    }
}
