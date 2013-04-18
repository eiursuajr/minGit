using System;
using System.Collections;
using System.Configuration;
using System.Data;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using Ektron.Cms.Widget;
using Ektron.Cms;
using Ektron.Cms.API;
using Ektron.Cms.PageBuilder;
using Ektron.Cms.Common;
using System.Collections.Generic;
using Ektron.Cms.Personalization;
using Ektron.Cms.Activity;



public partial class widget_ActivityStream : System.Web.UI.UserControl, IWidget
{
    #region properties
    IWidgetHost _host;
    Ektron.Cms.Personalization.WidgetSpaceData WidgetSpace = null;
    protected CommonApi _api;
    private long _objectid;
    private string _feedtype;
    private int _maxresults;
    private DirectoryUserData[] friendList;
    private CommunityGroupData[] groupList;
    private UserData userInfo;
    IActivityStream _activityApi = ObjectFactory.GetActivityStream();
    private System.Collections.Generic.List<Ektron.Cms.Activity.ActivityData> ActivityData = new List<Ektron.Cms.Activity.ActivityData>();
    private List<string> _preferences = new List<string>();
    private List<string> _groupPreferences = new List<string>();
    private List<string> _friendPreferences = new List<string>();
    private HtmlControl divFilter;
    protected ContentAPI _contentApi;
    //Paging Variables
    protected int currentPageNumber = 1;
    private int totalPagesNumber = 1;
    [WidgetDataMember(0)]
    public long ObjectId { get { return _objectid; } set { _objectid = value; } }
    [WidgetDataMember("User")]
    public string FeedType { get { return _feedtype; } set { _feedtype = value; } }
    [WidgetDataMember(10)]
    public int MaxResults { get { return _maxresults; } set { _maxresults = value; } }
    [WidgetDataMember()]
    public List<string> Preferences
    {
        get
        {
            return _preferences;
        }
        set
        {
            _preferences = value;
        }
    }
    [WidgetDataMember()]
    public List<string> FriendPreferences
    {
        get
        {
            return _friendPreferences;
        }
        set
        {
            _friendPreferences = value;
        }
    }
    [WidgetDataMember()]
    public List<string> GroupPreferences
    {
        get
        {
            return _groupPreferences;
        }
        set
        {
            _groupPreferences = value;
        }
    }

    #endregion




    protected void Page_Init(object sender, EventArgs e)
    {
        divFilter = View.FindControl("EkActivityFeedPrefenceSelection") as HtmlControl;
        PageBuilder p = Page as PageBuilder;
        _host = Ektron.Cms.Widget.WidgetHost.GetHost(this);
        _host.LoadWidgetDataMembers();
        _host.Title = "Activity Stream";
        
        string sitepath = new CommonApi().SitePath;
        JS.RegisterJSInclude(this, JS.ManagedScript.EktronJS);
        JS.RegisterJSInclude(this, sitepath + "widgets/ActivityStream/js/jquery.autocomplete.js", "EktronWidgetAutocompleteJS");
        JS.RegisterJSInclude(this, sitepath + "widgets/ActivityStream/js/activityStream.js", "EktronWidgetActivityStreamJS");
        JS.RegisterJSInclude(this, JS.ManagedScript.EktronInputLabelJS);
        JS.RegisterJSBlock(this, String.Format("Ektron.Widgets.ActivityFeed.Init('{0}', '{1}');", View.FindControl("query").ClientID, ClientID), ClientID + "JS");
        Css.RegisterCss(this, sitepath + "widgets/ActivityStream/css/activityStream.css", "EktronAFWidgetCSS");
        Css.RegisterCss(this, sitepath + "widgets/ActivityStream/css/activityStream.ltIE7.css", "EktronAFWidgetIE7CSS", Css.BrowserTarget.LessThanEqualToIE7);
        Css.RegisterCss(this, sitepath + "widgets/ActivityStream/css/activityStream.IE6.css", "EktronAFWidgetIE6CSS", Css.BrowserTarget.IE6);
        

        hdnClientId.Value = this.ClientID;
        
        _host.Maximize += new MaximizeDelegate(delegate() { Visible = true; });
        _host.Minimize += new MinimizeDelegate(delegate() { Visible = false; });
        _host.Edit += new EditDelegate(EditEvent);
        _host.Create += new CreateDelegate(delegate() { EditEvent(""); });
        if (!(_host.IsEditable))
        {
            btnFilter.Visible = false;  
        }
        if (p != null)
        {
            btnFilter.Visible = false;
        }
        else
        {
            checkDashboard();
        }
        SetProperties();
        PreRender += new EventHandler(delegate(object PreRenderSender, EventArgs Evt) { 
            if (ViewSet.GetActiveView() != Edit) SetActivityFeed(); 
        });
        ViewSet.SetActiveView(View);
        if (p == null)
        {
            rowobjectid.Visible = false;
            rowfeedtype.Visible = false;
            cmsActivityFeed.DynamicObjectParameter = "id,groupid,profileid";
            LoadFriendGroupList();
            UpdateFriends();
        }
        if (Page.IsCallback && Preferences.Count > 0)
        {

            if (Request.Form[UniqueID + "$hdnCurrentPageNumber"] != null)
            {
                string pageNumber = Request.Form[UniqueID + "$hdnCurrentPageNumber"].ToString();
                currentPageNumber = String.IsNullOrEmpty(pageNumber) ? 1 : Int32.Parse(pageNumber);
            }
            SetActivityFeed();
            
        }
              
    }

    void checkDashboard()
    {
        Control ctl = this;
        while (ctl != null && ctl as WidgetControls_WidgetSpace == null)
        {

            ctl = ctl.Parent;

        }

        if (ctl != null)
        {
            WidgetSpace = (ctl as WidgetControls_WidgetSpace).WidgetSpace;

        }

        if (WidgetSpace.Scope == WidgetSpaceScope.CommunityGroup)
        {
            btnFilter.Visible = false;
        }
    }
    void UpdateFriends()
    {
        repFriends.DataSource = _preferences;
        if (_preferences.Count > 0)
        {
            repFriends.Visible = true;
        }
        repFriends.DataBind();
        _host.SaveWidgetDataMembers();
    }

    void EditEvent(string settings)
    {
        try
        {

            objectid.Text = ObjectId.ToString();
            FeedTypeSelectDropDownList.SelectedValue = FeedType;
            txtmaxresults.Text = MaxResults.ToString();   
            ViewSet.SetActiveView(Edit);
        }
        catch (Exception e)
        {
            errorLb.Text = e.Message + e.Data + e.StackTrace + e.Source + e.ToString();
            ViewSet.SetActiveView(View);
        }
    }

    protected void LoadFriendGroupList()
    {  
        Ektron.Cms.UserAPI _userApi = new UserAPI();
        Ektron.Cms.Community.FriendsAPI _friendsApi = new Ektron.Cms.Community.FriendsAPI();
        Ektron.Cms.Community.CommunityGroupAPI _communityGroupApi = new Ektron.Cms.Community.CommunityGroupAPI();
        int totalpages = 0;
        int totalgroups = 0;
        userInfo = _userApi.GetUserById(_userApi.RequestInformationRef.UserId, false, false);  
        friendList = _friendsApi.GetFriendsForUser(_friendsApi.RequestInformationRef.UserId);
        groupList = _communityGroupApi.GetCommunityGroupsForUser(_communityGroupApi.RequestInformationRef.UserId, 0, 1000, ref totalpages, ref totalgroups);
    }
    protected void SetProperties()
    {
        PageBuilder p = Page as PageBuilder;
        if (cmsActivityFeed.TemplateUserProfile == string.Empty)
        {
            _contentApi = new ContentAPI();
            TemplateData[] userTemplate; 
            userTemplate = _contentApi.GetCommunityTemplate(Ektron.Cms.Common.EkEnumeration.TemplateType.User);
            if (userTemplate != null)
            {
                for (int i = 0; i <= userTemplate.Length - 1; i++)
                {
                    if (userTemplate[i].SubType == EkEnumeration.TemplateSubType.Profile)
                    {
                        cmsActivityFeed.TemplateUserProfile = userTemplate[i].FileName.ToString();
                        cmsActivityFeed.ProfileParamName = "id";
                        break;
                    }
                }
            }
        }
        cmsActivityFeed.DefaultObjectID = ObjectId;
        cmsActivityFeed.ObjectType = (FeedType == "User") ? EkEnumeration.ActivityFeedType.User : EkEnumeration.ActivityFeedType.CommunityGroup;
        if (p == null && WidgetSpace.Scope == WidgetSpaceScope.CommunityGroup)
        {
            cmsActivityFeed.ObjectType = EkEnumeration.ActivityFeedType.CommunityGroup;
        }
        cmsActivityFeed.MaxResults = MaxResults; 
    }
    protected void SetActivityFeed()
    {

        if (Preferences.Count == 0)
        {

            PageSettings();
                       
        }
        else
        {
            LoadActivityData();
            cmsActivityFeed.LoadData(ActivityData);
        }
        cmsActivityFeed.CacheInterval = ((Page as PageBuilder) != null) ? (Page as PageBuilder).CacheInterval : 0;
        cmsActivityFeed.Fill();
    }
    protected void SaveButton_Click(object sender, EventArgs e)
    {
        // Save the variables
        if (!long.TryParse(objectid.Text, out _objectid)) _objectid = 0;
        FeedType = FeedTypeSelectDropDownList.SelectedValue;
        if(!int.TryParse (txtmaxresults.Text,out _maxresults)) _maxresults = 0;
        _host.SaveWidgetDataMembers();
        SetProperties();
        ViewSet.SetActiveView(View);
    }

    private void LoadActivityData()
    {
        PagingInfo _Page = new PagingInfo(cmsActivityFeed.MaxResults);
        _Page.CurrentPage = currentPageNumber;
        ActivityCriteria criteria = new ActivityCriteria();
        criteria.PagingInfo = _Page;
        criteria.OrderByDirection = EkEnumeration.OrderByDirection.Descending;
        criteria.OrderByField = Ektron.Cms.Activity.ActivityProperty.Date;
        List<long> userIdlist = new List<long>();
        List<long> groupIdList = new List<long>();

        foreach (string item in FriendPreferences)
        {
             
            if (userInfo.DisplayName.ToLower() == item.Substring(item.IndexOf(">")+1).ToLower())
            {
                userIdlist.Add(userInfo.Id); 
            }
            for (int i = 0; i <= friendList.Length - 1; i++)
            {
                if (friendList[i].DisplayName == item.Substring(item.IndexOf(">") + 1))
                {

                    userIdlist.Add(friendList[i].Id);
                }
            }
            
        }
        foreach (string groupItem in GroupPreferences)
        {
            for (int i = 0; i <= groupList.Length - 1; i++)
            {
                if (groupList[i].GroupName == groupItem.Substring(groupItem.IndexOf(">") + 1))
                {

                    groupIdList.Add(groupList[i].GroupId);
                }
            }
        }
        criteria.Condition = LogicalOperation.Or;
        if (userIdlist.Count > 0)
        {
            criteria.AddFilter(Ektron.Cms.Activity.ActivityProperty.ActionUserId, CriteriaFilterOperator.In, userIdlist);
        }
        if (groupIdList.Count > 0)
        {
            CriteriaFilterGroup<Ektron.Cms.Activity.ActivityProperty> group = new CriteriaFilterGroup<Ektron.Cms.Activity.ActivityProperty>();
            group.Condition = LogicalOperation.And;
            group.AddFilter(Ektron.Cms.Activity.ActivityProperty.ScopeObjectType, CriteriaFilterOperator.EqualTo, EkEnumeration.ActivityFeedType.CommunityGroup);
            group.AddFilter(Ektron.Cms.Activity.ActivityProperty.ScopeObjectId, CriteriaFilterOperator.In, groupIdList);

            criteria.FilterGroups.Add(group);
        }
        if (userIdlist.Count > 0 || groupIdList.Count > 0)
        {
            ActivityData = _activityApi.GetUserActivityStream(criteria, _activityApi.RequestInformation.UserId);
            totalPagesNumber = criteria.PagingInfo.TotalPages;
         }
         PageSettings();
    }
   
    protected void CancelButton_Click(object sender, EventArgs e)
    {
        ViewSet.SetActiveView(View);
    }


    protected void btnAdd_Click(object sender, EventArgs e)
    {
        HtmlInputControl query = View.FindControl("query") as HtmlInputControl;
        ContentAPI _contentApi = new ContentAPI();
        string userImg = "<img src=\"" + _contentApi.AppPath + "images/UI/Icons/user.png\" title=\"User\"/>";
        string groupImg = "<img src=\"" + _contentApi.AppPath + "Images/ui/icons/usersMemberGroups.png\" title=\"Group\"/>";

        if (!(_friendPreferences.Contains(userImg + query.Value)))
        {
            if (userInfo.DisplayName.ToLower() == query.Value.ToLower())
            {

                _friendPreferences.Add(userImg + query.Value);
            }
            for (int i = 0; i <= friendList.Length - 1; i++)
            {
                if (friendList[i].DisplayName == query.Value)
                {

                    _friendPreferences.Add(userImg + query.Value);
                }
            }
        }
        if (!(_groupPreferences.Contains(groupImg + query.Value)))
        {
            for (int i = 0; i <= groupList.Length - 1; i++)
            {
                if (groupList[i].GroupName == query.Value)
                {

                    _groupPreferences.Add(groupImg + query.Value);
                }
            }
        }
        // reintiniliaze textbox and List
        query.Value = "";
        _preferences.Clear();
        
        //Combine the group and user preferences.
        _preferences.InsertRange(0, _friendPreferences);
        _preferences.InsertRange(_preferences.Count, _groupPreferences); 
        UpdateFriends();

        divFilter.Visible = true;
        _host.SaveWidgetDataMembers();
    }
    protected void repFriends_ItemDataBound(object sender, RepeaterItemEventArgs e)
    {
        LinkButton btnDelete = e.Item.FindControl("btnDelete") as LinkButton;
    }
    protected void repFriends_ItemCommand(object source, RepeaterCommandEventArgs e)
    {
        switch (e.CommandName)
        {
            case "btnDelete":
                foreach( string item in _friendPreferences) // Remove the item from the friend pref 
                {
                    if(item == _preferences[e.Item.ItemIndex])
                    {
                        _friendPreferences.Remove(item);
                        break;
                    }
                }
                foreach (string item in _groupPreferences) // Remove the item from the group pref 
                {
                    if (item == _preferences[e.Item.ItemIndex])
                    {
                        _groupPreferences.Remove(item);
                        break;
                    }
                }
                _preferences.RemoveAt(e.Item.ItemIndex); // Remove the item from combined pref.
                UpdateFriends();
                divFilter.Visible = true;
                break;
        }
    }
    protected void btnFilter_Click(object sender, EventArgs e)
    {

        divFilter.Visible = true;
    }
    protected void CancelFilterBtn_Click(object sender, EventArgs e)
    {
        divFilter.Visible = false;
        query.Value = string.Empty;
       
    }
    protected void PageSettings()
    {
        if(totalPagesNumber <= 1)
        {
            VisiblePageControls(false);
        }
        else
        {
            VisiblePageControls(true);
            TotalPages.Text = totalPagesNumber.ToString();
            CurrentPage.Text = currentPageNumber.ToString(); 
            PreviousPage1.Enabled = true;
            FirstPage.Enabled = true;
            NextPage.Enabled = true;
            LastPage.Enabled = true;
            if (currentPageNumber == 1)
            {
                PreviousPage1.Enabled = false;
                FirstPage.Enabled = false;
            }
            else if(currentPageNumber == totalPagesNumber)
            {
                NextPage.Enabled = false;
                LastPage.Enabled = false;
            }
        }

    }
        
    private void VisiblePageControls(bool flag)
    {
        TotalPages.Visible = flag;
        CurrentPage.Visible = flag;
        PreviousPage1.Visible = flag;
        NextPage.Visible = flag;
        LastPage.Visible = flag;
        FirstPage.Visible = flag;
        PageLabel.Visible = flag;
        OfLabel.Visible = flag;
    }
    protected void NavigationLink_Click(object sender, EventArgs e)
    {
        switch (((System.Web.UI.WebControls.CommandEventArgs)(e)).CommandName)
          {
             case "First":
                 currentPageNumber = 1;
                break;
            case "Last":
                 currentPageNumber = Int32.Parse(TotalPages.Text);
                break;
            case "Next":
                 currentPageNumber = Int32.Parse(CurrentPage.Text) + 1;
                break;
            case "Prev":
                currentPageNumber = Int32.Parse(CurrentPage.Text) - 1;
                break;
         }
         hdnCurrentPageNumber.Value = currentPageNumber.ToString(); 
         LoadActivityData();
         
    }
        
       
}
