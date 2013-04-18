using System;
using System.Collections.Generic;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Ektron.Cms.Analytics;
using Ektron.Cms;

public partial class Analytics_controls_ProviderSelector : WorkareaBaseControl
{
	private string _cookieName = "";
    private IAnalytics _dataManager = ObjectFactory.GetAnalytics();

    #region delegates, events and event arguments

    public delegate void ProviderChangedHandler(object sender, ProviderChangedEventArgs e);
    public event ProviderChangedHandler OnProviderChanged;
    public class ProviderChangedEventArgs : EventArgs
    {
        private string _providerInfo = string.Empty;
        public ProviderChangedEventArgs(string providerName)
        {
            _providerInfo = providerName;
        }
        public string ProviderName
        {
            get { return _providerInfo; }
            set { _providerInfo = value; }
        }
    }

    #endregion

	private string _persistenceId = "SiteAnalyticsSelectedProvider";
	/// <summary>
	/// Used to store and retrieve date with client cookie.
	/// Default to none which prevents persistence.
	/// </summary>
	public string PersistenceId
	{
		get { return _persistenceId; }
		set { _persistenceId = value; }
	}

    private List<string> _providerlist = null;
    public List<string> ProviderList
    {
        set 
        { 
            _providerlist = value;
            reLoadProviderList();
        }
    }
    public string ProviderName
    {
        get { return this.SelectedText; }
        set { this.SelectedText = value; }
    }

    private void reLoadProviderList()
    {
        string currentSelectedProvider = this.ProviderName;
        if (_providerlist.Count > 0)
        {
            ProviderSelectorList.Items.Clear();
            int counter = 0;
            foreach (string item in _providerlist)
            {
                ProviderSelectorList.Items.Insert(counter, new ListItem(item, item));
                if (item == currentSelectedProvider)
                {
                    ProviderSelectorList.SelectedValue = currentSelectedProvider;
                }
                counter++;
            }
        }
    }

	protected override void OnInit(EventArgs e)
	{
		base.OnInit(e);

        if (_dataManager.IsAnalyticsViewer())
        {
			if (_persistenceId.Length > 0)
			{
				_cookieName = CommonApi.RequestInformationRef.UserId + "_" + _persistenceId;
			}

            if (null == _providerlist && (ProviderSelectorList.Items == null || ProviderSelectorList.Items.Count == 0))
            {
                List<string> providerList = _dataManager.GetProviderList();

                ProviderSelectorList.DataSource = providerList;
                ProviderSelectorList.DataBind();
            }
            else if (_providerlist.Count > 0)
            {
                reLoadProviderList();
            }
            if (_persistenceId.Length > 0)
            {
                // if found the selected provider cookie, set the ProviderSelectorList text.           
                HttpCookie selectedProviderCookie = Request.Cookies[_cookieName];
                if (selectedProviderCookie != null)
                {
                    this.SelectedText = selectedProviderCookie.Value;
                }
                else
                {
                    //remember the selection in cookie if it is never created.
                    HttpCookie selectedSiteCookie = new HttpCookie(_cookieName);
                    selectedSiteCookie.Value = ProviderSelectorList.SelectedItem.Value.ToString();
                    selectedSiteCookie.Expires = DateTime.MaxValue; // Never Expires
                    Response.Cookies.Add(selectedSiteCookie);
                }
            }
            this.ProviderName = ProviderSelectorList.SelectedValue;
        }
        lblProviderSelector.Text = GetMessage("lbl analytics provider");
        lblProviderSelector.ToolTip = lblProviderSelector.Text;
	}

    protected virtual void ProviderSelectorList_SelectionChanged(object sender, EventArgs e)
    {
        if (_persistenceId.Length > 0)
        {
            //remember the selection in cookie
            HttpCookie selectedSiteCookie = new HttpCookie(_cookieName);
            selectedSiteCookie.Value = ProviderSelectorList.SelectedItem.Value.ToString();
            selectedSiteCookie.Expires = DateTime.MaxValue; // Never Expires
            Response.Cookies.Add(selectedSiteCookie);
        }
        //reload the controls to select new report
        if (OnProviderChanged != null)
        {
            OnProviderChanged(this, new ProviderChangedEventArgs(this.SelectedText));
        }
    }

    public virtual string SelectedText
    {
        get { return (ProviderSelectorList != null && ProviderSelectorList.SelectedItem != null) ? ProviderSelectorList.SelectedValue : String.Empty; }
        set
        {
            if (ProviderSelectorList != null && ProviderSelectorList.Items != null && ProviderSelectorList.Items.Count > 0)
            {
                if (ProviderSelectorList.Items.Contains(new ListItem(value)))
                {
                    ProviderSelectorList.SelectedValue = value;
                }
            }
        }
    }

	public string CssClass
	{
		get { return ProviderSelectorContainer.Attributes["class"]; }
        set { ProviderSelectorContainer.Attributes["class"] = value; }
	}

    public bool AutoPostBack
    {
        get { return ProviderSelectorList.AutoPostBack; }
        set { ProviderSelectorList.AutoPostBack = value; }
    }

}