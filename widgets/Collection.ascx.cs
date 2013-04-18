using System;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using Ektron.Cms.Widget;
using Ektron.Cms;
using Ektron.Cms.API;
using Ektron.Cms.Common;
using Ektron.Cms.Controls.CmsWebService;
using Ektron.Cms.PageBuilder;
using System.Collections.Generic;
using System.Collections;


public partial class widgets_Collection : System.Web.UI.UserControl, IWidget
{

    #region properties
    private long _CollectionId;
    private long DynamicId;
    private bool _Teaser;
    private bool _EnablePaging;
    private int _PageSize;
    private bool _IncludeIcons;
    private string _addText;
    private long _selTaxonomyID;
    private bool _displaySelectedContent;
    [WidgetDataMember(0)]
    public long CollectionId { get { return _CollectionId; } set { _CollectionId = value; } }
    [WidgetDataMember(true)]
    public bool Teaser { get { return _Teaser; } set { _Teaser = value; } }
    [WidgetDataMember(false)]
    public bool EnablePaging { get { return _EnablePaging; } set { _EnablePaging = value; } }
    [WidgetDataMember(10)]
    public int PageSize { get { return _PageSize; } set { _PageSize = value; } }
    [WidgetDataMember(false)]
    public bool IncludeIcons { get { return _IncludeIcons; } set { _IncludeIcons = value; } }
    [WidgetDataMember("Add Content")]
    public string AddText { get { return _addText; } set { _addText = value; } }
    [WidgetDataMember(0)]
    public long SelTaxonomyID { get { return _selTaxonomyID; } set { _selTaxonomyID = value; } }
    [WidgetDataMember(false)]
    public bool DisplaySelectedContent { get { return _displaySelectedContent; } set { _displaySelectedContent = value; } }
    #endregion

    IWidgetHost _host;

    protected void Page_Init(object sender, EventArgs e)
    {
        _host = Ektron.Cms.Widget.WidgetHost.GetHost(this);
        _host.Title = "Collection Widget";
        _host.Edit += new EditDelegate(EditEvent);
        _host.Maximize += new MaximizeDelegate(delegate() { Visible = true; });
        _host.Minimize += new MinimizeDelegate(delegate() { Visible = false; });
        _host.Create += new CreateDelegate(delegate() { EditEvent(""); });
        _host.ExpandOptions = Expandable.ExpandOnEdit;
        Load += new EventHandler(delegate(object PreRenderSender, EventArgs Evt) { if (ViewSet.GetActiveView() != Edit)SetOutput(); });
        ViewSet.SetActiveView(View);
    }

    protected void SetOutput()
    {
        if (CollectionId > 0)
        {
            Ektron.Cms.Controls.Collection Collection1 = new Ektron.Cms.Controls.Collection();
            Collection1.Page = Page;
            Collection1.DefaultCollectionID = CollectionId;
            Collection1.DisplayXslt = (Teaser) ? "ecmTeaser" : "ecmNavigation";
            Collection1.IncludeIcons = IncludeIcons;
            Collection1.MaxResults = PageSize;
            Collection1.EnablePaging = EnablePaging;
            Collection1.AddText = AddText;
            Collection1.SelTaxonomyID = SelTaxonomyID;
            Collection1.ContentParameter = DisplaySelectedContent ? "id" : "no_id";
            if (DisplaySelectedContent && Request.QueryString["id"] != null)
            {
                long.TryParse(Request.QueryString["id"], out DynamicId);
                Collection1.DefaultCollectionID = DynamicId;
            }
            Collection1.CacheInterval = ((Page as PageBuilder) != null) ? (Page as PageBuilder).CacheInterval : 0;
            Collection1.Visible = true;
            View.Controls.Add(Collection1);
            Text.Visible = false;
        } else {
            Text.Visible = true;
        }
    }


    void EditEvent(string settings)
    {
        try
        {
            pagesize.Text = PageSize.ToString();
            TeaserCheckBox.Checked = Teaser;
            IncludeIconsCheckBox.Checked = IncludeIcons;
            EnablePagingCheckBox.Checked = EnablePaging;
            AddTextTextBox.Text = AddText;
            SelTaxonomyIDTextBox.Text = SelTaxonomyID.ToString();
            DisplaySelectedContentCheckBox.Checked = DisplaySelectedContent;

            //get list of collections from system
            ContentAPI capi = new ContentAPI();
            CollectionListData[] res = capi.EkContentRef.GetCollectionList();

            collectionlist.Items.Clear();
            if (res == null || res.Length == 0)
            {
                ListItem li = new ListItem();
                li.Text = "No collections defined";
                li.Value = "0";
                li.Attributes.Add("description", "No collections defined");
                collectionlist.Items.Add(li);
            }
            else
            {
                foreach (CollectionListData item in res)
                {
                    ListItem li = new ListItem();
                    li.Text = item.Id.ToString() + ": " + item.Title;
                    li.Value = item.Id.ToString();
                    li.Selected = (CollectionId == item.Id);
                    li.Attributes.Add("description", item.Description.Replace("\"", "'"));
                    collectionlist.Items.Add(li);
                }
            }
            if (collectionlist.SelectedItem == null) collectionlist.SelectedIndex = 0;

            description.InnerHtml = collectionlist.Items[0].Attributes["description"];
            string script = "$ektron('#" + collectionlist.ClientID + "').change(function(){";
            script += "var desc = $ektron(this).find(':selected');";
            script += "desc = ('undefined' === typeof(desc.attr('description')))? '' : desc.attr('description');";
            script += "$ektron(this).parents('.ekColEditView').find('.ekcoldescription').html(desc);";
            script += "});";
            Ektron.Cms.API.JS.RegisterJSBlock(this, script, "collectionedit" + this.ClientID);

            ViewSet.SetActiveView(Edit);
        }
        catch (Exception e)
        {
			 string error = e.Message;
		  // errorLb.Text = e.Message + e.Data + e.StackTrace + e.Source + e.ToString();
            ViewSet.SetActiveView(Edit);
        }
    }

    protected void CancelButton_Click(object sender, EventArgs e)
    {
        SetOutput();
        ViewSet.SetActiveView(View);
    }

    protected void SaveButton_Click(object sender, EventArgs e)
    {
        // Save the variables
        if (!long.TryParse(collectionlist.SelectedValue, out _CollectionId)) _CollectionId = 0;
        if (!int.TryParse(pagesize.Text, out _PageSize)) _PageSize = 2;
        if (!long.TryParse(SelTaxonomyIDTextBox.Text, out _selTaxonomyID)) _selTaxonomyID = 0;
        Teaser = TeaserCheckBox.Checked;
        IncludeIcons = IncludeIconsCheckBox.Checked;
        EnablePaging = EnablePagingCheckBox.Checked;
        AddText = (AddTextTextBox.Text.Length > 1) ? AddTextTextBox.Text : "Add Content";
        DisplaySelectedContent = DisplaySelectedContentCheckBox.Checked;
        _host.SaveWidgetDataMembers();
        SetOutput();
        ViewSet.SetActiveView(View);
    }
}
