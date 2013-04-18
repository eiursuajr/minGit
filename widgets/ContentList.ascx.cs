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


public partial class widgets_ContentList : System.Web.UI.UserControl, IWidget
{

    #region properties
    private string _ContentIds;
    private bool _teaser;
    private bool _IncludeIcons;
    private string _Direction;
    private string _OrderKey;
    [WidgetDataMember("")]
    public string ContentIds { get { return _ContentIds; } set { _ContentIds = value; } }
    [WidgetDataMember(true)]
    public bool Teaser { get { return _teaser; } set { _teaser = value; } }
    [WidgetDataMember(true)]
    public bool IncludeIcons { get { return _IncludeIcons; } set { _IncludeIcons = value; } }
    [WidgetDataMember("Ascending")]
    public string Direction { get { return _Direction; } set { _Direction = value; } }
    [WidgetDataMember("Title")]
    public string OrderKey { get { return _OrderKey; } set { _OrderKey = value; } }
    #endregion

    IWidgetHost _host;

    protected void Page_Init(object sender, EventArgs e)
    {
        _host = Ektron.Cms.Widget.WidgetHost.GetHost(this);
        _host.Title = "ContentList Widget";
        _host.Edit += new EditDelegate(EditEvent);
        _host.ExpandOptions = Expandable.ExpandOnEdit;
        _host.Maximize += new MaximizeDelegate(delegate() { Visible = true; });
        _host.Minimize += new MinimizeDelegate(delegate() { Visible = false; });
        _host.Create += new CreateDelegate(delegate() { EditEvent(""); });
        PreRender += new EventHandler(delegate(object PreRenderSender, EventArgs Evt) { SetOutput(); });
        ViewSet.SetActiveView(View);
    }

    void EditEvent(string settings)
    {

        ids.Text = ContentIds;
        TeaserCheckBox.Checked = Teaser;
        IncludeIconsCheckBox.Checked = IncludeIcons;
        ContentListOrderKeyDropDownList.SelectedValue = OrderKey;
        ContentListDirectionSelectDropDownList.SelectedValue = Direction;
        ViewSet.SetActiveView(Edit);
    }

    protected void SaveButton_Click(object sender, EventArgs e)
    {
        try
        {
            ContentIds = ids.Text;
        }
        catch
        {
            ContentIds = "";
        }
        Teaser = TeaserCheckBox.Checked;
        IncludeIcons = IncludeIconsCheckBox.Checked;
        Direction = ContentListDirectionSelectDropDownList.SelectedValue;
        OrderKey = ContentListOrderKeyDropDownList.SelectedValue;
        _host.SaveWidgetDataMembers();
        ViewSet.SetActiveView(View);
    }

    protected void SetOutput()
    {
        ContentList1.ContentIds = ContentIds;
        ContentList1.DisplayXslt = (Teaser) ? "ecmTeaser" : "ecmNavigation";
        ContentList1.IncludeIcons = IncludeIcons;
        ContentList1.OrderByDirection = (Direction == "Ascending") ? EkEnumeration.OrderByDirection.Ascending : EkEnumeration.OrderByDirection.Descending;
        switch (OrderKey)
        {
            case "Title":
                ContentList1.OrderBy = Ektron.Cms.Controls.CmsWebService.ContentListOrderBy.Title;
                break;
            case "DateModified":
                ContentList1.OrderBy = Ektron.Cms.Controls.CmsWebService.ContentListOrderBy.DateModified;
                break;
            case "DateCreated":
                ContentList1.OrderBy = Ektron.Cms.Controls.CmsWebService.ContentListOrderBy.DateCreated;
                break;
            case "LastEditorFname":
                ContentList1.OrderBy = Ektron.Cms.Controls.CmsWebService.ContentListOrderBy.LastEditorFname;
                break;
            case "LastEditorLname":
                ContentList1.OrderBy = Ektron.Cms.Controls.CmsWebService.ContentListOrderBy.LastEditorLname;
                break;
            default: //ID
                ContentList1.OrderBy = Ektron.Cms.Controls.CmsWebService.ContentListOrderBy.OrderOfTheIds;
                break;
        }
            ContentList1.CacheInterval = ((Page as PageBuilder) != null) ? (Page as PageBuilder).CacheInterval : 0;
    }

    protected void CancelButton_Click(object sender, EventArgs e)
    {
        ViewSet.SetActiveView(View);
    }

}


