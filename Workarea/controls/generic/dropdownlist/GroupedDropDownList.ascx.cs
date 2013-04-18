using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Xml;
using Ektron.Cms.Common;

////////////////////////////////////////////////////////////////////////////////////////////////////
// GroupedDropDownList: Demo Data Initizization:
// Notes:
//    1. See classes GroupedDropDownList, GroupedListItem and CustomListItem in this file.
//    2. The following method would be in the page/control that consumes user control GroupedDropDownList.
//    3. ***VALUES MUST BE UNIQUE*** (searches for selections are done by value)
//
// protected void AddData() {
//    List<GroupedListItem> items = new List<GroupedListItem>();
//
//    // GroupedListItem: (string text, string value, bool enabled, bool selected, Object customObject, string groupName)
//
//    // group A
//    items.Add(new GroupedListItem("text1", "value1", true, true, "Group: A, Item 1", "Group-A"));
//    items.Add(new GroupedListItem("text2", "value2", true, false, "Group: A, Item 2", "Group-A"));
//    items.Add(new GroupedListItem("text3", "value3", true, false, "Group: A, Item 3", "Group-A"));
//
//    // group B
//    items.Add(new GroupedListItem("text1", "value1", true, false, "Group: B, Item 1", "Group-B"));
//    items.Add(new GroupedListItem("text2", "value2", true, false, "Group: B, Item 2", "Group-B"));
//
//    // groupless
//    items.Add(new GroupedListItem("text1", "value1", true, false, "Group: (none), Item 1", ""));
//    items.Add(new GroupedListItem("text2", "value2", true, false, "Group: (none), Item 2", ""));
//
//    // assumes user control ID is "ddl"
//    ddl.Items = items;
//}
////////////////////////////////////////////////////////////////////////////////////////////////////

public partial class GroupedDropDownList : System.Web.UI.UserControl, 
    System.Web.UI.IPostBackDataHandler
{
    private const string DROP_DOWN_CTL_ID = "GroupedDropDownCtl1";
    private GroupedDropDown _gdd = null;
    private string _viewStateValue = string.Empty;

    #region delegates and events

    public delegate void OnChanged(EventArgs args);

    public event OnChanged OnChangedHandler;

    #endregion

    protected GroupedDropDown GroupedDropDownControl {
        get { 
            return (_gdd ?? ((GroupedDropDown)ph1.FindControl(DROP_DOWN_CTL_ID) ?? AddSelectControl()));
        }
    }

    [TypeConverter(typeof(string))]
    public string ClassName {
        get { return GroupedDropDownControl.ClassName; }
        set { GroupedDropDownControl.ClassName = value; }
    }

    public GroupedListItem SelectedItem {
        get { return GroupedDropDownControl.SelectedItem; }
        set {
            if (value != null) {
                string key = GroupedDropDown.GetReferenceKey(value);
                GroupedDropDownControl.SelectItemByClientValue(key);
            }
        }
    }

    public string SelectedItemKey {
        get { return (GroupedDropDown.GetReferenceKey(GroupedDropDownControl.SelectedItem) ?? string.Empty); }
        set { GroupedDropDownControl.SelectItemByClientValue(value); }
    }

    public List<GroupedListItem> Items {
        set { 
            GroupedDropDownControl.Items = value; 
            GroupedDropDownControl.DataReady = true;
        }
    }

    [TypeConverter(typeof(bool))]
    public bool ChangeCausesPostback {
        get { return GroupedDropDownControl.ChangeCausesPostback; }
        set { GroupedDropDownControl.ChangeCausesPostback = value; }
    }

    bool IPostBackDataHandler.LoadPostData(string postDataKey, System.Collections.Specialized.NameValueCollection postCollection) {
        string clientValue = postCollection[GroupedDropDownControl.UniqueID];
        // if viable, update selection to posted value
        if (!string.IsNullOrEmpty(clientValue)) {
            GroupedListItem gli = GroupedDropDownControl.SelectItemByClientValue(clientValue);
            if (null != gli) {
                // if view state different from posted value, trigger event
                if (!string.IsNullOrEmpty(_viewStateValue) && _viewStateValue != clientValue) {
                    return true;
                }
            }
        }

        return false;
    }

    void IPostBackDataHandler.RaisePostDataChangedEvent() {
        if (null != OnChangedHandler)
            OnChangedHandler(new EventArgs());
    }

    protected override void OnInit(EventArgs e) {
        base.OnInit(e);
        base.PreRender += OnPreRender;

        AddSelectControl();
    }

    protected void OnPreRender(object sender, EventArgs e) {
        Page.RegisterRequiresPostBack(this);
    }

    protected override void LoadViewState(object savedState){
        if (savedState is System.Web.UI.Pair) {
            System.Web.UI.Pair pair = (System.Web.UI.Pair)savedState;
            if (pair != null && pair.First != null && pair.First.ToString() == this.ClientID) {
                if (null != pair.Second.ToString()) {
                    _viewStateValue = pair.Second.ToString();
                }
            }
        }
    }

    protected override object SaveViewState(){
        GroupedListItem gli = GroupedDropDownControl.SelectedItem;
        if (gli != null)
            return ((Object)new System.Web.UI.Pair(this.ClientID, GroupedDropDown.GetReferenceKey(gli)));

        return null;
    }

    protected GroupedDropDown AddSelectControl(){
        if (_gdd != null)
            return _gdd;

        _gdd = new GroupedDropDown();
        _gdd.ID = DROP_DOWN_CTL_ID;

        ph1.Controls.Add(_gdd);

        return _gdd;
    }
}

public class GroupedDropDown : System.Web.UI.WebControls.WebControl {

    #region member variables and properties

    private List<GroupedListItem> _grouplessItems = null; // private List<CustomListItem> _grouplessCustomListItems = null;
    private Dictionary<string, List<GroupedListItem>> _optionGroups = null;
    private Dictionary<string, GroupedListItem> _itemValueReference = null;
    private GroupedListItem _selectedItem = null;
    private GroupedListItem _firstItem = null;

    private string _className = "GroupedDropDown";
    public string ClassName {
        get { return _className; }
        set { _className = value; }
    }

    private bool _dataReady = false;
    public bool DataReady {
        get { return _dataReady; }
        set {
            if (!_dataReady && value)
                BuildData();
        }
    }

    private string _Text = String.Empty;
    public string Text {
        get { return _Text; }
        set { _Text = value; }
    }

    private List<GroupedListItem> _items = null;
    public List<GroupedListItem> Items {
        get { return (_items ?? (_items = new List<GroupedListItem>())); }
        set { _items = value; }
    }

    public GroupedListItem SelectedItem {
        get {
            if (_selectedItem == null)
                return (_selectedItem = FindSelectedItem());

            return _selectedItem;
        }
    }

    public GroupedListItem SelectItemByClientValue(string value) {
        if (!DataReady)
            return null;

        _selectedItem = FindItemByClientValue(value);
        if (_selectedItem != null) {
            GroupedListItem gli = FindSelectedItem();
            gli.Selected = (gli == _selectedItem);
            _selectedItem.Selected = true;
        }

        return _selectedItem;
    }

    public GroupedListItem FindItemByClientValue(string value) {
        if (DataReady
            && !string.IsNullOrEmpty(value)
            && _itemValueReference != null
            && _itemValueReference.ContainsKey(value))
            return _itemValueReference[value];

        return null;
    }

    public GroupedListItem FindSelectedItem() {
        if (!DataReady)
            return null;

        foreach (GroupedListItem gli in _itemValueReference.Values) {
            if (gli.Selected)
                return gli;
        }

        return _firstItem;
    }

    private bool _changeCausesPostback = true;
    public bool ChangeCausesPostback {
        get { return _changeCausesPostback; }
        set { _changeCausesPostback = value; }
    }

    #endregion

    #region constructors

    public GroupedDropDown() { }

    #endregion

    #region protected members

    protected void BuildData() {
        List<GroupedListItem> groupedListItems;
        _grouplessItems = new List<GroupedListItem>();
        _optionGroups = new Dictionary<string, List<GroupedListItem>>();
        _itemValueReference = new Dictionary<string, GroupedListItem>();

        foreach (GroupedListItem gli in Items) {
            if (string.IsNullOrEmpty(gli.GroupName)) {
                AddGroupedListItem(_grouplessItems, gli);
            } else {
                if (_optionGroups.TryGetValue(gli.GroupName, out groupedListItems)) {
                    AddGroupedListItem(groupedListItems, gli);
                } else {
                    groupedListItems = new List<GroupedListItem>();
                    AddGroupedListItem(groupedListItems, gli);
                    _optionGroups.Add(gli.GroupName, groupedListItems);
                }
            }
        }

        _dataReady = true;
    }

    protected void AddGroupedListItem(List<GroupedListItem> groupedItems, GroupedListItem gli) {
        groupedItems.Add(gli);
        _itemValueReference.Add(GetReferenceKey(gli), gli);
        if (null == _firstItem)
            _firstItem = gli;
    }

    public static string GetReferenceKey(GroupedListItem gli) {
        return ((gli != null && gli.Value != null) ? gli.Value : string.Empty);
    }

    protected override void Render(HtmlTextWriter writer) {
        List<GroupedListItem> groupedListItems;

        writer.WriteBeginTag("select");
        writer.WriteAttribute("id", this.ClientID);
        writer.WriteAttribute("name", this.UniqueID);
        if (ChangeCausesPostback)
            writer.WriteAttribute("onchange", "document.forms[0].submit();");
        if (ClassName.Length > 0)
            writer.WriteAttribute("class", ClassName);
        writer.Write(HtmlTextWriter.TagRightChar);

        if (DataReady) {
            if (_grouplessItems != null && _grouplessItems.Count > 0)
                WriteOptions(writer, _grouplessItems);

            foreach (string groupName in _optionGroups.Keys) {
                writer.WriteBeginTag("optgroup");
                writer.WriteAttribute("label", groupName);
                writer.Write(HtmlTextWriter.TagRightChar);
                if (_optionGroups.TryGetValue(groupName, out groupedListItems)) {
                    WriteOptions(writer, groupedListItems);
                }
                writer.WriteEndTag("optgroup");
            }
        }

        writer.WriteEndTag("select");
    }

    private void WriteOptions(HtmlTextWriter writer, List<GroupedListItem> groupedListItems) {
        foreach (GroupedListItem gli in groupedListItems) {
            writer.WriteBeginTag("option");
            writer.WriteAttribute("value", GetReferenceKey(gli)); // writer.WriteAttribute("value", gli.Value);
            if (!gli.Enabled)
                writer.WriteAttribute("disabled", "disabled");

            if (gli.Selected)
                writer.WriteAttribute("selected", "selected");
            writer.Write(HtmlTextWriter.TagRightChar);
            writer.Write(gli.Text);
            writer.WriteEndTag("option");
        }
    }

    #endregion
}

public class GroupedListItem : CustomListItem {

    private string _groupName = string.Empty;
    public string GroupName {
        get { return _groupName; }
        set { _groupName = value; }
    }

    public GroupedListItem() { }

    public GroupedListItem(string text) { base.Text = text; }

    public GroupedListItem(string text, string value) { base.Text = text; base.Value = value; }

    public GroupedListItem(string text, string value, bool enabled) { base.Text = text; base.Value = value; base.Enabled = enabled; }

    public GroupedListItem(string text, string value, bool enabled, bool selected) { base.Text = text; base.Value = value; base.Enabled = enabled; base.Selected = selected; }

    public GroupedListItem(string text, string value, bool enabled, bool selected, Object customObject) { base.Text = text; base.Value = value; base.Enabled = enabled; base.Selected = selected; base.CustomObject = customObject; }

    public GroupedListItem(string text, string value, bool enabled, bool selected, Object customObject, string groupName) { base.Text = text; base.Value = value; base.Enabled = enabled; base.Selected = selected; base.CustomObject = customObject; GroupName = groupName; }
}

public class CustomListItem : Object {

    private string _text = string.Empty;
    public string Text {
        get { return _text; }
        set { _text = value; }
    }

    private string _value = string.Empty;
    public string Value {
        get { return _value; }
        set { _value = value; }
    }

    private bool _enabled = false;
    public bool Enabled {
        get { return _enabled; }
        set { _enabled = value; }
    }

    private bool _selected = false;
    public bool Selected {
        get { return _selected; }
        set { _selected = value; }
    }

    private Object _customObject = null;
    public Object CustomObject {
        get { return _customObject; }
        set { _customObject = value; }
    }

    public CustomListItem() { }

    public CustomListItem(string text) { Text = text; }

    public CustomListItem(string text, string value) { Text = text; Value = value; }

    public CustomListItem(string text, string value, bool enabled) { Text = text; Value = value; Enabled = enabled; }

    public CustomListItem(string text, string value, bool enabled, bool selected) { Text = text; Value = value; Enabled = enabled; Selected = selected; }

    public CustomListItem(string text, string value, bool enabled, bool selected, Object customObject) { Text = text; Value = value; Enabled = enabled; Selected = selected; CustomObject = customObject; }
}
