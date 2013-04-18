using System;
using System.Collections.Generic;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Ektron.Cms;
using Ektron.Cms.Content.Calendar;
using Ektron.Cms.Common.Calendar;
using Ektron.Cms.Widget;
using Ektron.Cms.PageBuilder;
using Ektron.Cms.Controls;
using Ektron.Cms.API;
using System.Text;
using Ektron.Cms.Common;
using Ektron.Cms.Framework.UI;

public partial class widgets_WebCalendar : System.Web.UI.UserControl, IWidget
{
    #region Member Variables
    private List<CalendarDataSource> _calendarsource = new List<CalendarDataSource>();
    private IWidgetHost _host;
    private string UserIdQueryString = "id";
    private long _userid = -1;
    private CommonApi _commonapi = null;
    private Ektron.Cms.Framework.Organization.FolderManager _folder = null;
    #endregion
    #region Properties
    [WidgetDataMember()]
    public List<CalendarDataSource> CalendarSource { get { return _calendarsource; } set { _calendarsource = value; } }
    public bool InPageBuilder { get { return (Page as PageBuilder) != null; } }
    public IWidgetHost MyHost { get { if (_host == null) { _host = Ektron.Cms.Widget.WidgetHost.GetHost(this); } return _host; } }
    public long UserID { get { if (Request[UserIdQueryString] != null && _userid < 1) long.TryParse(Request[UserIdQueryString], out _userid); return _userid; } }
    public long LoggedInUserId { get { return commonapi.RequestInformationRef.UserId; } }
    public string LoggedInUserName { get { return commonapi.RequestInformationRef.LoggedInUsername; } }
    public CommonApi commonapi { get { if (_commonapi == null) _commonapi = new CommonApi(); return _commonapi; } }
    public Ektron.Cms.Framework.Organization.FolderManager folder { get { if (_folder == null) _folder = new Ektron.Cms.Framework.Organization.FolderManager(Ektron.Cms.Framework.ApiAccessMode.Admin); return _folder; } }
    public string SitePath { get { return commonapi.RequestInformationRef.SitePath; } }
    #endregion
    #region PageEvents
    protected void Page_Init(object sender, EventArgs e)
    {
        MyHost.Title = "WebCalendar Widget";
        _host.Edit += new EditDelegate(EditEvent);
        _host.Maximize += new MaximizeDelegate(delegate() { Visible = true; });
        _host.Minimize += new MinimizeDelegate(delegate() { Visible = false; });
        _host.ExpandOptions = Expandable.ExpandOnEdit;
        ViewSet.SetActiveView(View);

        foreach (CalendarDataSource cds in CalendarSource)
        {
            if (cds.CategoryIDs.Count == 0 && cds.SelectedTaxID <= 0 && cds.defaultId > 0) {
                // if category is required, then preset the calendar-events default category to the folders' first default category:
                Ektron.Cms.API.Folder folderApi = new Folder();
                FolderData folder = folderApi.GetFolder(cds.defaultId, true);              
                
                if (folder != null && folder.CategoryRequired) {
                    TaxonomyBaseData[] tax = folder.FolderTaxonomy;
                    if (tax != null && tax.Length > 0 && tax[0].Id > 0) {
                        cds.SelectedTaxID = tax[0].Id;
                    }
                }
            }
            calendar.DataSource.Add(cds);
        }
        PageBuilder pb = Page as PageBuilder;
        if(pb != null){
            calendar.AllowEventEditing = !_host.IsEditable;
            WarningNoEdit.Visible = _host.IsEditable;
            WarningNoEdit.Text = "Add/Edit Event is disabled during page edit.";
        }
        calendar.Fill();
        Ektron.Cms.Framework.UI.Css baseCss = new Ektron.Cms.Framework.UI.Css()
        {
            Path = commonapi.SitePath + "widgets/webcalendar/view.css",
            ID = "WebCalendarWidgetViewCss",
            BrowserTarget = Ektron.Cms.Framework.UI.BrowserTarget.LessThanEqualToIE7
        };
        baseCss.Register(this);
    }
    protected void sourcerepeater_databound(object sender, RepeaterItemEventArgs e)
    {
        CalendarDataSource cds = (e.Item.DataItem as CalendarDataSource);
        if (e.Item.ItemType == ListItemType.AlternatingItem || e.Item.ItemType == ListItemType.Item)
        {
            Control itemID = e.Item.FindControl("itemID");
            if(itemID != null){
                string sourcetype = Enum.GetName(typeof(SourceType), cds.sourceType).Replace("Calendar", "");
                (itemID as Literal).Text = sourcetype + "-" + cds.defaultId;
            }
            Control itemName = e.Item.FindControl("itemName");
            if (itemName != null)
            {
                string name = "";
                switch (cds.sourceType)
                {
                    case SourceType.GroupCalendar:
                        Ektron.Cms.API.Community.CommunityGroup cg = new Ektron.Cms.API.Community.CommunityGroup();
                        CommunityGroupData cgd = cg.GetCommunityGroupByID(cds.defaultId);
                        name = cgd.GroupName;
                        break;
                    case SourceType.SystemCalendar:
                        FolderData fd = this.folder.GetItem(cds.defaultId);
                        name = fd.Name;
                        break;
                    case SourceType.UserCalendar:
                        Ektron.Cms.Framework.User.UserManager user = new Ektron.Cms.Framework.User.UserManager(Ektron.Cms.Framework.ApiAccessMode.Admin);
                        UserData ud = user.GetItem(cds.defaultId);
                        name = ud.DisplayName;
                        break;
                    default:
                        break;
                }
                (itemName as Literal).Text = name;
            }
        }
    }
    #endregion
    #region UserEvents
    void EditEvent(string settings)
    {
        try
        {
            string sitepath = commonapi.SitePath;
            string webserviceURL = sitepath + "widgets/webcalendar/Handler.ashx";
            Package p = new Package()
            {
                Components = new List<Component>(){
                    Packages.jQuery.jQueryUI.ThemeRoller,
                    Packages.jQuery.jQueryUI.Tabs,
                    new Ektron.Cms.Framework.UI.Css() { Path = sitepath + "widgets/webcalendar/dd.css", ID =  "JQueryDDCss", Aggregate = true},
                    new Ektron.Cms.Framework.UI.Css() { Path = sitepath + "widgets/webcalendar/style.css", ID =  "WCWidgetCSS", Aggregate = true},
                    new JavaScript() { Path = sitepath + "widgets/webcalendar/uncompressed.jquery.dd.js", ID = "JQueryDD", Aggregate = true, Minify = true},
                    new JavaScript() { Path = sitepath + "widgets/webcalendar/behavior.js", ID = "WebCalendarWidgetBehaviorJS", Aggregate = true, Minify = true}
                }
            };
            p.Register(this);
                 
            JS.RegisterJSBlock(this, "Ektron.PFWidgets.WebCalendar.setupAll(\""+webserviceURL+"\"); ", "EktronPFWidgetsWCInit");

            StringBuilder datasources = new StringBuilder();
            for (int i = 0; i < CalendarSource.Count; i++)
            {
                datasources.Append(Enum.GetName(typeof(SourceType), CalendarSource[i].sourceType).Replace("Calendar", ""));
                datasources.Append("-");
                datasources.Append(CalendarSource[i].defaultId);
                datasources.Append("-");
                datasources.Append(Enum.GetName(typeof(EventColor), CalendarSource[i].backColor));
                if (i < CalendarSource.Count - 1)
                {
                    datasources.Append(":");
                }
            }
            CalendarList.Text = datasources.ToString();

            //build viewable list of sources here
            if (CalendarSource.Count > 0)
            {
                sourcerepeater.DataSource = CalendarSource;
                sourcerepeater.DataBind();
                nosources.Visible = false;
            }
            else
            {
                nosources.Visible = true;
            }

            //does user have a calendar?
            Ektron.Cms.Content.Calendar.WebCalendar calendar = new Ektron.Cms.Content.Calendar.WebCalendar(this.folder.RequestInformation);
            Ektron.Cms.Common.Calendar.WebCalendarData webCalendarData = calendar.GetPublicCalendar(EkEnumeration.WorkSpace.User, folder.UserId);
            divMyCalendar.Visible = (webCalendarData != null);
   


            ViewSet.SetActiveView(Edit);
        }
        catch (Exception e)
        {
            ViewSet.SetActiveView(View);
			string _error = e.Message;
        }
    }

    protected void SaveButton_Click(object sender, EventArgs e)
    {
        //extract data and save (format: type-id-color:id-type-color)
        string[] sources = CalendarList.Text.Split(':');
        CalendarSource.Clear();
        foreach(string source in sources){
            string[] parts = source.Split('-');

            // only process correctly formatted sources
            if (parts.Length < 2) continue;

            CalendarDataSource c = new CalendarDataSource();
            long sourceid = 0;
            if (long.TryParse(parts[1], out sourceid) && sourceid > 0)
            {
                c.defaultId = sourceid;
                string sourcetype = parts[0] + "Calendar";
                c.sourceType = (SourceType)Enum.Parse(typeof(SourceType), sourcetype);
                try
                {
                    c.backColor = (EventColor)Enum.Parse(typeof(EventColor), parts[2]);
                }
                catch (Exception ex)
                {
                    c.backColor = EventColor.AutoSelect;
                    string _error = ex.Message;
                }
                CalendarSource.Add(c);
            }
        }
        
        _host.SaveWidgetDataMembers();
        calendar.DataSource.Clear();
        foreach (CalendarDataSource cds in CalendarSource)
        {
            calendar.DataSource.Add(cds);
        }
        calendar.Fill();
        ViewSet.SetActiveView(View);
    }

    protected void CancelButton_Click(object sender, EventArgs e)
    {
        ViewSet.SetActiveView(View);
    }
    #endregion
}
