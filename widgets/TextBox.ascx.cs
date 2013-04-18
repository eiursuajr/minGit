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


public partial class Widgets_TextBox : System.Web.UI.UserControl, IWidget
{

    #region properties
    private string _TextData;
    [WidgetDataMember("")]
    public string TextData { get { return _TextData; } set { _TextData = value; } }
    #endregion

    IWidgetHost _host;

    protected void Page_Init(object sender, EventArgs e)
    {
        _host = Ektron.Cms.Widget.WidgetHost.GetHost(this);
        _host.Title = "TextBox Widget";
        _host.Edit += new EditDelegate(EditEvent);
        _host.Maximize += new MaximizeDelegate(delegate() { Visible = true; });
        _host.Minimize += new MinimizeDelegate(delegate() { Visible = false; });
        _host.Create += new CreateDelegate(delegate() { EditEvent(""); });
        PreRender += new EventHandler(delegate(object PreRenderSender, EventArgs Evt) { SetOutput(); });
        ViewSet.SetActiveView(View);
    }

    void EditEvent(string settings)
    {

        string sitepath = new CommonApi().SitePath;
        ScriptManager.RegisterClientScriptInclude(this, this.GetType(), "widgetjavascript", sitepath + "widgets/widgets.js");
        ScriptManager.RegisterOnSubmitStatement(this.Page, this.GetType(), "gadgetescapehtml", "GadgetEscapeHTML('" + TextTextBox.ClientID + "');");
        TextTextBox.Text = TextData;
        ViewSet.SetActiveView(Edit);
    }

    protected void SaveButton_Click(object sender, EventArgs e)
    {
        TextData = ReplaceEncodeBrackets(TextTextBox.Text);
        _host.SaveWidgetDataMembers();
        ViewSet.SetActiveView(View);
    }

    protected void SetOutput()
    {
        if (TextData.Length > 0)
        {
            TextLabel.Text = TextData;
        }
        else
        {
            TextLabel.Text = "No Text";
        }
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

}

