using System.Web.UI.WebControls;
using System.Configuration;
using System.Collections;
using System.Linq;
using System.Data;
using System.Web.Caching;
using System.Web.UI;
using System.Diagnostics;
using System.Web.Security;
using System;
using System.Text;
using Microsoft.VisualBasic;
using System.Web.UI.HtmlControls;
using System.Web.SessionState;
using System.Text.RegularExpressions;
using System.Web.Profile;
using System.Collections.Generic;
using System.Web.UI.WebControls.WebParts;
using System.Collections.Specialized;
using System.Web;
using Ektron.Cms.Widget;
using System.Reflection;
using Ektron.Cms;

public class PropertyData
{

    public string FullTypeName;
    public string TypeName;
    public string Name;
    [System.Xml.Serialization.XmlIgnore()]
    public Type Type
    {
        get
        {
            Type tmp = System.Type.GetType(TypeName);
            if (tmp == null)
            {
                tmp = System.Type.GetType(FullTypeName);
            }
            return tmp;
        }
        set
        {
            TypeName = value.Name;
            FullTypeName = value.AssemblyQualifiedName;
        }
    }
    public object Value;
}

public partial class Workarea_controls_widgetSettings_WidgetEdit : System.Web.UI.UserControl
{


    protected ContentAPI m_refContentApi = new Ektron.Cms.ContentAPI();
    protected Ektron.Cms.Common.EkMessageHelper m_refMsg;
    protected WidgetTypeModel m_refWidgetModel = new WidgetTypeModel();
    protected StyleHelper m_refStyle = new StyleHelper();

    protected WidgetTypeData info = null;
    protected long m_widgetID;
    protected Dictionary<PropertyInfo, GlobalWidgetData> propertydic = new Dictionary<PropertyInfo, GlobalWidgetData>();

    protected void Page_Load(object sender, System.EventArgs e)
    {
        Ektron.Cms.API.JS.RegisterJS(this, Ektron.Cms.API.JS.ManagedScript.EktronJS);
        Ektron.Cms.API.JS.RegisterJS(this, m_refContentApi.AppPath + "java/dateParser.js", "DateJS");

        m_refMsg = m_refContentApi.EkMsgRef;
        Save.ToolTip = Save.Text = m_refMsg.GetMessage("generic save");
        ToolBar();

        if (!string.IsNullOrEmpty(Request.QueryString["widgetid"]))
        {
            if (!long.TryParse(Request.QueryString["widgetid"], out m_widgetID))
            {
                ShowError("Could not find that widget");
                return;
            }
        }

        if (m_widgetID > -1)
        {
            if (!m_refWidgetModel.FindByID(m_widgetID, out info))
            {
                ShowError("Could not find that widget");
                return;
            }
        }

        if (info != null)
        {
            lblID.Text = info.ID.ToString();
            lblID.ToolTip = lblID.Text;
            lblFilename.Text = info.ControlURL;
            lblFilename.ToolTip = lblFilename.Text;
            txtTitle.Text = info.Title;
            txtTitle.ToolTip = txtTitle.Text;
            txtLabel.Text = info.ButtonText;
            txtLabel.ToolTip = txtLabel.Text;


            //get properties marked as global settings and fill in table
            LoadWidgetGlobalProps();
            SetupPropertyEditor();

            viewset.SetActiveView(viewSettings);

            if (Page.IsPostBack)
            {
                SaveProperties(null, null);
            }
        }
        else
        {
            ShowError("Unknown error");
            return;
        }
    }

    private void ToolBar()
    {
        divTitleBar.InnerHtml = m_refStyle.GetTitleBar((string)(m_refMsg.GetMessage("generic edit title") + " " + m_refMsg.GetMessage("generic widget")));
        System.Text.StringBuilder result = new System.Text.StringBuilder();
        result.Append("<table><tr>" + "\r\n");
        //result.Append(m_refStyle.GetButtonEventsWCaption(m_refContentApi.AppPath & "images/UI/Icons/save.png", _
        //                                                 "#", m_refMsg.GetMessage("alt update button text"), _
        //                                                 m_refMsg.GetMessage("btn update"), _
        //                                                 "onclick=""document.forms[0].submit();"""))
        result.Append(m_refStyle.GetButtonEventsWCaption(m_refContentApi.AppPath + "images/UI/Icons/back.png", "widgetsettings.aspx?action=widgetsync", m_refMsg.GetMessage("alt back button text"), m_refMsg.GetMessage("btn back"), "onclick=\"return closeParentWin();\"", StyleHelper.BackButtonCssClass,true));
        result.Append("</tr></table>");
        divToolBar.InnerHtml = result.ToString();
        result = null;
    }

    protected void ShowError(string message)
    {
        lblError.Text = message;
        viewset.SetActiveView(viewError);
    }

    protected void SaveProperties(object sender, System.EventArgs e)
    {
       
        StringBuilder str = new StringBuilder();

        if (m_widgetID < 0)
        {
            ShowError("Could not find that widget");
            return;
        }

        List<GlobalWidgetPropertySettings> collection = new List<GlobalWidgetPropertySettings>();
        if (propertydic.Count > 0)
        {
            foreach (KeyValuePair<PropertyInfo, GlobalWidgetData> item in propertydic)
            {
                GlobalWidgetPropertySettings conv = new GlobalWidgetPropertySettings();
                conv.PropertyName = (string)item.Key.Name;
                conv.Type = item.Key.PropertyType;
                conv.value = item.Value.getDefault;
                collection.Add(conv);
            }

            foreach (RepeaterItem propertyitem in globalProps.Items)
            {
                Label lblpropname = (Label)(propertyitem.FindControl("lblPropertyName"));
                foreach (GlobalWidgetPropertySettings listitem in collection)
                {
                    if (listitem.PropertyName == lblpropname.Text)
                    {
                        PlaceHolder ph = (PlaceHolder)(propertyitem.FindControl("phValue"));
                        Control cntpropval = ph.FindControl("value");
                        decodeValue(listitem.Type, cntpropval, ref listitem.value);
                    }
                }
            }
        }

        System.Xml.Serialization.XmlSerializer serializer = new System.Xml.Serialization.XmlSerializer(typeof(List<GlobalWidgetPropertySettings>));
        System.IO.StringWriter s = new System.IO.StringWriter();
        serializer.Serialize(s, collection);

        //now serialize collection into settings
        string title = Request.Form[txtTitle.UniqueID]; // for some reason, the submit doesn't automatically update txtTitle.Text and txtLabel.Text
        string label = Request.Form[txtLabel.UniqueID];
        if (!m_refWidgetModel.Update(m_widgetID, title, label, (string)lblFilename.Text, (string)(s.GetStringBuilder().ToString()), true))
        {
            ShowError("Could not update widget");
        }

        successmessage.Text = "Properties Successfully saved";
        successmessage.ToolTip = successmessage.Text;
        str.AppendLine("    window.parent.$ektron(\'#editWidget\').modalHide();");
        viewset.SetActiveView(viewSuccess);

        Ektron.Cms.API.JS.RegisterJSBlock(this, str.ToString(), "HideEditModal");
    }

    protected void LoadWidgetGlobalProps()
    {
        if (info != null)
        {
            UserControl uc;
            uc = (UserControl)(LoadControl(Request.ApplicationPath + "/Widgets/" + info.ControlURL));
            if (uc != null)
            {
                //collect properties and types
                PropertyInfo[] properties = uc.GetType().GetProperties();
                foreach (PropertyInfo pi in properties)
                {
                    GlobalWidgetData[] propertyattributes;
                    propertyattributes = (GlobalWidgetData[])(pi.GetCustomAttributes(typeof(GlobalWidgetData), true));
                    if ((propertyattributes != null) && propertyattributes.Length > 0)
                    {
                        propertydic.Add(pi, propertyattributes[0]);
                    }
                }
            }
        }
    }

    protected void SetupPropertyEditor()
    {
        List<GlobalWidgetPropertySettings> collection = new List<GlobalWidgetPropertySettings>();

        if (propertydic.Count > 0)
        {
            GlobalSettingsRow.Visible = true;
            foreach (KeyValuePair<PropertyInfo, GlobalWidgetData> item in propertydic)
            {
                GlobalWidgetPropertySettings conv = new GlobalWidgetPropertySettings();
                conv.PropertyName = (string)item.Key.Name;
                conv.Type = item.Key.PropertyType;
                conv.value = item.Value.getDefault;
                conv.Settings = item.Value.PropertySettings;
                collection.Add(conv);
            }
            LoadSavedProps(collection);
            globalProps.DataSource = collection;
            globalProps.DataBind();
        }
        else
        {
            GlobalSettingsRow.Visible = false;
        }
    }

    protected void LoadSavedProps(List<GlobalWidgetPropertySettings> autogenprops)
    {
        if (info.Settings != string.Empty)
        {
            List<GlobalWidgetPropertySettings> savedprops;
            System.Xml.Serialization.XmlSerializer serializer = new System.Xml.Serialization.XmlSerializer(typeof(List<GlobalWidgetPropertySettings>));
            System.IO.StringReader s = new System.IO.StringReader(info.Settings);
            try
            {
                savedprops = (List<GlobalWidgetPropertySettings>)(serializer.Deserialize(s));
            }
            catch
            {
                savedprops = new List<GlobalWidgetPropertySettings>();
            }
            foreach (GlobalWidgetPropertySettings toupdate in autogenprops)
            {
                foreach (GlobalWidgetPropertySettings iter in savedprops)
                {
                    if (toupdate.PropertyName == iter.PropertyName && toupdate.FullTypeName == iter.FullTypeName && (iter.value != null))
                    {
                        toupdate.value = iter.value;
                        break;
                    }
                }
            }
        }
    }

    protected void Properties_OnItemDataBound(object Sender, RepeaterItemEventArgs e)
    {
        GlobalWidgetPropertySettings data = (GlobalWidgetPropertySettings)e.Item.DataItem;
        Label lblPropertyName = (Label)(e.Item.FindControl("lblPropertyName"));
        Label lblType = (Label)(e.Item.FindControl("lblType"));
        PlaceHolder phValue = (PlaceHolder)(e.Item.FindControl("phValue"));
        lblPropertyName.Text = data.PropertyName;
        lblPropertyName.ToolTip = lblPropertyName.Text;
        lblType.Text = data.Type.Name;
        lblType.ToolTip = lblType.Text;
        phValue.Controls.Clear();
        GenerateInputField(phValue, data);
    }

    protected void GenerateInputField(Control owner, GlobalWidgetPropertySettings data)
    {
        Type type = data.Type;
        object value = data.value;
        //DateTime, int, long, double, bool, string, Enumeration EkEnumeration.OrderByDirection()
        if (!type.IsEnum)
        {
            if (type.Name == typeof(string).Name)
            {
                TextBox text = new TextBox();
                text.ID = "value";
                text.Text = System.Convert.ToString(value);
                text.CssClass = "freetext";
                if (data.Settings == GlobalPropertyAttributes.PasswordField)
                {
                    text.TextMode = TextBoxMode.Password;
                    text.Attributes.Add("value", System.Convert.ToString(value));
                }
                owner.Controls.Add(text);
            }
            else if (type.Name == typeof(DateTime).Name)
            {
                HiddenField hdnvalue = new HiddenField();
                hdnvalue.ID = "value";
                hdnvalue.Value = System.Convert.ToDateTime(value).ToString();
                owner.Controls.Add(hdnvalue);
                TextBox text = new TextBox();
                text.ID = "textinput";
                text.Text = System.Convert.ToDateTime(value).ToString();
                text.CssClass = "datetime";
                text.Attributes.Add("hiddenfield", hdnvalue.ClientID);
                owner.Controls.Add(text);
                System.Web.UI.HtmlControls.HtmlGenericControl span = new System.Web.UI.HtmlControls.HtmlGenericControl();
                span.TagName = "span";
                span.Attributes.Add("class", "displayParsedDate");
                owner.Parent.FindControl("phExtraOutput").Controls.Add(span);
            }
            else if (type.Name == typeof(int).Name)
            {
                TextBox text = new TextBox();
                text.ID = "value";
                text.Text = System.Convert.ToString(System.Convert.ToInt32(value).ToString());
                text.CssClass = "integer";
                owner.Controls.Add(text);
            }
            else if (type.Name == typeof(long).Name)
            {
                TextBox text = new TextBox();
                text.ID = "value";
                text.Text = System.Convert.ToString(System.Convert.ToInt64(value).ToString());
                text.CssClass = "integer";
                owner.Controls.Add(text);
            }
            else if (type.Name == typeof(double).Name)
            {
                TextBox text = new TextBox();
                text.ID = "value";
                text.Text = System.Convert.ToString(System.Convert.ToDouble(value).ToString());
                text.CssClass = "double";
                owner.Controls.Add(text);
            }
            else if (type.Name == typeof(bool).Name)
            {
                DropDownList list = new DropDownList();
                list.ID = "value";
                list.Items.Add(new ListItem("True", "true"));
                list.Items.Add(new ListItem("False", "false"));
                if (System.Convert.ToBoolean(value) == true)
                {
                    list.SelectedIndex = 0;
                }
                else
                {
                    list.SelectedIndex = 1;
                }
                owner.Controls.Add(list);
            }
            else
            {
                string ex = "Unsupported global property specified in " + info.ControlURL + "\r\n";
                ex += "The type " + type.Name + " is not supported by this version of the Portal Framework. Only the following types are supported: " + "\r\n";
                ex += "DateTime, int, long, double, bool, string, Enumeration";
                throw (new Exception(ex));
            }
        }
        else //is an enum
        {
            DropDownList list = new DropDownList();
            list.ID = "value";
            string[] items = System.Enum.GetNames(type);
            foreach (string item in items)
            {
                ListItem li = new ListItem();
                li.Text = item;
                li.Value = item;
                if ((value != null) && item == System.Enum.GetName(type, value))
                {
                    li.Selected = true;
                }
                list.Items.Add(li);
            }
            owner.Controls.Add(list);
        }
    }

    protected void decodeValue(Type type, Control inputitem, ref object value)
    {
        if (!type.IsEnum)
        {
            if (type.Name == typeof(string).Name)
            {
                TextBox text = (TextBox)inputitem;
                value = System.Convert.ToString(text.Text);
            }
            else if (type.Name == typeof(DateTime).Name)
            {
                HiddenField text = (HiddenField)inputitem;
                DateTime parsedDate = new DateTime();
                if (DateTime.TryParse(text.Value, out parsedDate))
                {
                    value = parsedDate;
                }
            }
            else if (type.Name == typeof(int).Name)
            {
                TextBox text = (TextBox)inputitem;
                int parsedInt;
                if (int.TryParse(text.Text, out parsedInt))
                {
                    value = parsedInt;
                }
            }
            else if (type.Name == typeof(long).Name)
            {
                TextBox text = (TextBox)inputitem;
                long parsedLong;
                if (long.TryParse(text.Text, out parsedLong))
                {
                    value = parsedLong;
                }
            }
            else if (type.Name == typeof(double).Name)
            {
                TextBox text = (TextBox)inputitem;
                double parsedDouble;
                if (double.TryParse(text.Text, out parsedDouble))
                {
                    value = parsedDouble;
                }
            }
            else if (type.Name == typeof(bool).Name)
            {
                DropDownList list = (DropDownList)inputitem;
                bool parsedBool;
                if(bool.TryParse(list.SelectedValue, out parsedBool))
                {
                    value = parsedBool;
                }
            }
            else
            {
                string ex = "Unsupported global property specified in " + info.ControlURL + "\r\n";
                ex += "The type " + type.Name + " is not supported by this version of the Portal Framework. Only the following types are supported: " + "\r\n";
                ex += "DateTime, int, long, double, bool, string, Enumeration";
                throw (new Exception(ex));
            }
        }
        else //is an enum
        {
            DropDownList list = (DropDownList)inputitem;
            value = System.Enum.Parse(type, list.SelectedValue);
        }
    }
}