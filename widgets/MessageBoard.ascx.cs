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



public partial class widget_MessageBoard : System.Web.UI.UserControl
{

    #region properties
    private bool _enablePaging;
    private bool _Moderate;
    private int _pageSize;
    private int _MaxChar;

    [WidgetDataMember(true)]
    public bool EnablePaging { get { return _enablePaging; } set { _enablePaging = value; } }
    [WidgetDataMember(false)]
    public bool Moderate { get { return _Moderate; } set { _Moderate = value; } }
    [WidgetDataMember(10)]
    public int PageSize { get { return _pageSize; } set { _pageSize = value; } }
    [WidgetDataMember(256)]
    public int MaxChar { get { return _MaxChar; } set { _MaxChar = value; } }

    #endregion

    IWidgetHost _host;

    protected void Page_Init(object sender, EventArgs e)
    {
        string sitepath = new CommonApi().SitePath;
        Css.RegisterCss(this, sitepath + "widgets/MessageBoard/css/messageBoard.css", "wmbCSS");

        _host = Ektron.Cms.Widget.WidgetHost.GetHost(this);
        _host.Title = "MessageBoard Widget";
        _host.Edit += new EditDelegate(EditEvent);
        _host.Maximize += new MaximizeDelegate(delegate() { Visible = true; });
        _host.Minimize += new MinimizeDelegate(delegate() { Visible = false; });
        _host.Create += new CreateDelegate(delegate() { EditEvent(""); });
        Load += new EventHandler(delegate(object PreRenderSender, EventArgs Evt) { if(ViewSet.GetActiveView() != Edit) SetOutput(); });
        ViewSet.SetActiveView(View);
    }

    void EditEvent(string settings)
    {
         ModerateCheckBox.Checked = Moderate;
         EnablePagingCheckBox.Checked = EnablePaging;
         pagesizeTextBox.Text = PageSize.ToString();
         MaxCharTextBox.Text = MaxChar.ToString();
       ViewSet.SetActiveView(Edit);
    }

    protected void SaveButton_Click(object sender, EventArgs e)
    {

        Moderate = ModerateCheckBox.Checked;
        EnablePaging = EnablePagingCheckBox.Checked;
         if (!int.TryParse(pagesizeTextBox.Text, out _pageSize)) _pageSize = 10;
         if (!int.TryParse(MaxCharTextBox.Text, out _MaxChar)) _MaxChar = 256;

       _host.SaveWidgetDataMembers();
       ViewSet.SetActiveView(View);
       SetOutput();
    }

    protected void SetOutput()
    {
        PageBuilder p = Page as PageBuilder;
        cmsMessageBoard.DynamicObjectParameter = "ID";
        cmsMessageBoard.Moderate = Moderate;
        cmsMessageBoard.EnablePaging = EnablePaging;
        cmsMessageBoard.MaxResults = PageSize;
        cmsMessageBoard.ShowMaxCharacter = MaxChar;
        if (p != null && Request.QueryString["id"] == null)
        {
            cmsMessageBoard.DynamicObjectParameter = "PageID";
        }
        cmsMessageBoard.CacheInterval = ((Page as PageBuilder) != null) ? (Page as PageBuilder).CacheInterval : 0;
        cmsMessageBoard.Fill();
    }

    protected void CancelButton_Click(object sender, EventArgs e)
    {
        SetOutput();
       ViewSet.SetActiveView(View);
    }

}







