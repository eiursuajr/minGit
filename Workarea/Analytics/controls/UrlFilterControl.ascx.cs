using System;
using System.Collections.Generic;
using System.Web.UI.WebControls;
using Ektron.Cms;
using Ektron.Cms.API;
using Ektron.Cms.Common;
using Ektron.Cms.Workarea;
using Ektron.Cms.Analytics;
using Ektron.Cms.Analytics.Reporting;
using Ektron.Cms.Analytics.DataIO;

public partial class UrlFilterControl : WorkareaBaseControl
{
    #region private properties

    private long itemId = 0;
    private List<string> _pagePaths = new List<string>();
    private ContentAPI _ContentAPI = null;
    private bool _hasRun = false;

    #endregion

    #region delegates and events

    public delegate void SelectionChangedHandler(object sender, EventArgs e);
    public event SelectionChangedHandler SelectionChanged;

    #endregion


    #region protected properties

    protected ContentAPI ContentApiInstance {
        get {
            if (null == _ContentAPI)
                _ContentAPI = new ContentAPI();
            return _ContentAPI; 
        }
        set { _ContentAPI = value; }
    }

    #endregion

    #region public properties

    public string[] PagePaths {
        get {
            if (_pagePaths.Count == 0) {
                GetId();
                PopulateFilterUi();
            }
            return _pagePaths.ToArray(); 
        }
    }

    #endregion

    protected void Page_Load(object sender, EventArgs e) {
        try {
            if (!IsPostBack) {
                InitializeUi();
                GetId();
                PopulateFilterUi();
            }
        }
        catch {}
    }

    protected void InitializeUi() {
        if (!IsPostBack) {
            litUrlFilterLabel.Text = GetMessage("lbl url filtering");
            litUrlFilterHeading.Text = GetMessage("lbl limit results to the following urls");
            btnAddUrlFilter.Text = GetMessage("lbl add filter");
            btnAddUrlFilter.ToolTip = btnAddUrlFilter.Text;
            btnUrlFilterAddAll.Text = GetMessage("generic add all title");
            btnUrlFilterAddAll.ToolTip = btnUrlFilterAddAll.Text;
            btnUrlFilterRemoveAll.Text = GetMessage("btn remove all");
            btnUrlFilterAddAll.ToolTip = btnUrlFilterAddAll.Text;
        }
    }

    protected void PopulateFilterUi() {
        if (itemId <= 0 || _hasRun)
            return;

        if (IsPostBack) {
            string oldFilterStatus = (IsUrlFilterStatusEmpty() ? String.Empty : tbUrlFilteringStatus.Text);
            tbUrlFilteringStatus.Text = String.Empty;

            // accept user-specified filtering URLs
            if (tbAddUrlFilter.Text.Length > 0) {
                AddListItem("/" + tbAddUrlFilter.Text.Trim('/'), true);
                tbAddUrlFilter.Text = String.Empty;
            }

            // add all url filters clicked
            if (Context.Request.Form["__EVENTTARGET"] != null && Context.Request.Form["__EVENTTARGET"] == this.btnUrlFilterAddAll.UniqueID) {
                foreach (ListItem li in cbl1.Items) {
                    li.Selected = true;
                }
            }

            // remove all url filters clicked
            if (Context.Request.Form["__EVENTTARGET"] != null && Context.Request.Form["__EVENTTARGET"] == this.btnUrlFilterRemoveAll.UniqueID) {
                foreach (ListItem li in cbl1.Items) {
                    li.Selected = false;
                }
            }

            // use each enabled (checked) url
            foreach (ListItem li in cbl1.Items) {
                if (li.Selected) {
                    _pagePaths.Add("/" + li.Value.Trim('/'));

                    if (tbUrlFilteringStatus.Text.Length > 0)
                        tbUrlFilteringStatus.Text += ", ";

                    tbUrlFilteringStatus.Text += "/" + li.Value.Trim('/');
                }
            }

            // fire any hooked handlers if selection has changed
            if (null != SelectionChanged && oldFilterStatus != tbUrlFilteringStatus.Text)
                SelectionChanged.Invoke(this, new EventArgs());

        } else {
            Analytics anl = new Analytics(ContentApiInstance.RequestInformationRef);
            string[] urls = anl.GetContentUrls(itemId);
            foreach (string url in urls) {
                if (!string.IsNullOrEmpty(url))
                    AddListItem("/" + url.Trim('/'), false);
            }
            foreach (ListItem li in cbl1.Items)
            {
                li.Selected = true;
                _pagePaths.Add("/" + li.Value.Trim('/'));
            }
        }

        _hasRun = true;

        if (tbUrlFilteringStatus.Text.Length == 0)
            tbUrlFilteringStatus.Text = GetMessage("lbl all urls allowed");
    }

    protected bool IsUrlFilterStatusEmpty() {
        return (tbUrlFilteringStatus.Text.Length == 0
            || tbUrlFilteringStatus.Text == GetMessage("lbl all urls allowed"));
    }

    protected void AddListItem(string text, bool selected) {
        ListItem li = new ListItem(text, text, true);
        li.Selected = selected;
        if (!cbl1.Items.Contains(li))
            cbl1.Items.Add(li);
    }

    protected void GetId() {
        long tempVal = 0;
        if (long.TryParse(Request.QueryString["itemid"], out tempVal))
            itemId = tempVal;
    }
}
