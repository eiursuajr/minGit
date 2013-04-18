using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Ektron.Cms.API;
using System.Web.Script.Serialization;
using Ektron.Cms.Controls;
using System.Collections.Specialized;

public partial class UserSelector : System.Web.UI.UserControl
{
    #region Resource Paths
    //protected const string JQueryUICSSPath = "ui/css/cupertino/jquery-ui-1.8.1.custom.css";
    protected const string JQueryUICSSPath = "workarea/csslib/ektronTheme/jquery-ui-1.8.1.custom.css";
    protected const string JSPath = "controls/UserSelector/js/UserSelector.js";
    protected const string CSSPath = "controls/UserSelector/css/UserSelector.css";
    #endregion

    #region Services
    private Ektron.Cms.Framework.Core.Content.Content _contentService;
    private Ektron.Cms.Framework.Core.Content.Content ContentService
    {
        get
        {
            if (_contentService == null) _contentService = new Ektron.Cms.Framework.Core.Content.Content();
            return _contentService;
        }
    }

    private JavaScriptSerializer _serializer;
    public JavaScriptSerializer Serializer
    {
        get
        {
            if (_serializer == null) _serializer = new JavaScriptSerializer();
            return _serializer;
        }
    }
    #endregion

    #region Public Properties
    private string _handlerPath = "controls/UserSelector/Handlers/User.ashx";
    public string HandlerPath
    {
        get { return _handlerPath; }
        set { _handlerPath = value; }
    }

    private Dictionary<string, string> _customParameters;
    public Dictionary<string, string> CustomParameters
    {
        get
        {
            if (_customParameters == null)
            {
                _customParameters = new Dictionary<string, string>();
            }

            return _customParameters;
        }
    }

    public List<User> SelectedUsers
    {
        get
        {
            if (ucSelectedUserIds.Value == "") return new List<User>();
            return Serializer.Deserialize<List<User>>(ucSelectedUserIds.Value);
        }

        set
        {
            ucSelectedUserIds.Value = Serializer.Serialize(value);
        }
    }

    private string _searchField = "Username";
    public string SearchField
    {
        get
        {
            return _searchField;
        }

        set
        {
            _searchField = value;
        }
    }
    #endregion

    #region JS and CSS Registration Methods
    protected void RegisterCSS()
    {
        Css.RegisterCss(this, ContentService.SitePath + UserSelector.JQueryUICSSPath, "JQueryUICSS");
        Css.RegisterCss(this, ContentService.RequestInformation.ApplicationPath + UserSelector.CSSPath, "UserSelectorCSS");
    }

    protected void RegisterJS()
    {
        int minLength = 2;

        JS.RegisterJSInclude(this, JS.ManagedScript.EktronJS);
        JS.RegisterJSInclude(this, JS.ManagedScript.EktronUICoreJS);
        JS.RegisterJSInclude(this, JS.ManagedScript.EktronUIWidgetJS);
        JS.RegisterJSInclude(this, JS.ManagedScript.EktronUIMouseJS);
        JS.RegisterJSInclude(this, JS.ManagedScript.EktronUIButtonJS);
        JS.RegisterJSInclude(this, JS.ManagedScript.EktronUIPositionJS);
        JS.RegisterJSInclude(this, JS.ManagedScript.EktronUIDraggableJS);
        JS.RegisterJSInclude(this, JS.ManagedScript.EktronUIResizableJS);
        JS.RegisterJSInclude(this, JS.ManagedScript.EktronUIAutoCompleteJS);
        JS.RegisterJSInclude(this, JS.ManagedScript.EktronUIDialogJS);
        JS.RegisterJSInclude(this, JS.ManagedScript.EktronUIDatePickerJS);

        JS.RegisterJSInclude(this, ContentService.RequestInformation.ApplicationPath + UserSelector.JSPath, "UserSelectorJS");
        JS.RegisterJSBlock(
            this,
            String.Format("Ektron.Controls.UserSelector.init('{0}', '{1}', {2}, '{3}', ({4}));",
                ClientID,
                (new CmsConnection()).GetApplicationPath() + HandlerPath,
                minLength,
                SearchField,
                Serializer.Serialize(CustomParameters)
            ),
            "UserSelectorInitJS" + ClientID
        );
    }
    #endregion

    #region Hooked Events
    protected void Page_Load(object sender, EventArgs e)
    {
        RegisterJS();
        RegisterCSS();
        DataBind();
    }
    #endregion

    #region Data Classes
    [Serializable]
    public class User
    {
        private long _id;
        private string _username;

        public long Id
        {
            get { return _id; }
            set { _id = value; }
        }

        public string Username
        {
            get { return _username; }
            set { _username = value; }
        }
    }
    #endregion
}
