using System;
using System.Collections.Generic;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Ektron.Cms;
using System.Text.RegularExpressions;
using Ektron.Cms.Widget;
using Ektron.Cms.PageBuilder;
using Ektron.Cms.API;

public partial class widgets_GoogleConversion : System.Web.UI.UserControl
{
    private IWidgetHost _host;

    #region Properties
    private string _ConversionScript;

    [WidgetDataMember("")]
    public string ConversionScript { get { return _ConversionScript; } set { _ConversionScript = value; } }
    #endregion

    protected void Page_Init(object sender, EventArgs e)
    {
        _host = Ektron.Cms.Widget.WidgetHost.GetHost(this);
        _host.Title = "Google Conversion";
        _host.Maximize += new MaximizeDelegate(delegate() { Visible = true; });
        _host.Minimize += new MinimizeDelegate(delegate() { Visible = false; });
        _host.Edit += new EditDelegate(EditEvent);

        if ((Page as PageBuilder).Status != Mode.Editing &&
            _ConversionScript != "")
        {
            Page.Controls.Add(new LiteralControl(_ConversionScript));
            litViewText.Visible = false;
        }
        else
        {
            litViewText.Visible = true;
        }

        string sitepath = new CommonApi().SitePath;
        imgConversion.ImageUrl = sitepath + "widgets/GoogleConversion/images/trackingthumb.gif";
        imgConversion.CssClass = "google-thumbnail";
        ScriptManager.RegisterClientScriptInclude(this, this.GetType(), "GoogleConversion1", sitepath + "widgets/GoogleConversion/js/googleconversion.js");
        Css.RegisterCss(this, sitepath + "widgets/GoogleConversion/css/googleconversion.css", "googleconversioncss");
        ScriptManager.RegisterOnSubmitStatement(this.Page, this.GetType(), "googleconversion",
            String.Format("Ektron.Widget.GoogleConversion.EscapeHTML('{0}');",
            tbConversionScript.ClientID));

        ViewSet.SetActiveView(View);
    }

    void EditEvent(string settings)
    {
        _host.LoadWidgetDataMembers();

        tbConversionScript.Text = ConversionScript;

        ViewSet.SetActiveView(Edit);
    }

    protected void btnSave_Click(object sender, EventArgs e)
    {
        ConversionScript = ReplaceEncodeBrackets(tbConversionScript.Text);
        _host.SaveWidgetDataMembers();

        ViewSet.SetActiveView(View);
    }

    protected void btnCancel_Click(object sender, EventArgs e)
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
