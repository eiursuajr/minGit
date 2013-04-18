using System;
using System.Data;
using System.Configuration;
using System.Collections;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using System.Collections.Generic;
using Ektron.Cms.Commerce;
using Ektron.Cms;
using Ektron.Newtonsoft.Json;
using Ektron.Cms.API;
using Ektron.Cms.Common;
using Ektron.Cms.Commerce.Workarea.ProductTypes.Tabs.ClientData;

namespace Ektron.Cms.Commerce.Workarea.ProductTypes.Tabs.ClientData
{
    public class AttributesClientData
    {
        private int _Order;
        private long _Id;
        private string _Type;
        private string _Name;
        private string _Value;
        private bool _MarkedForDelete;

        public int Order
        {
            get
            {
                return _Order;
            }
            set
            {
                _Order = value;
            }
        }
        public long Id
        {
            get
            {
                return _Id;
            }
            set
            {
                _Id = value;
            }
        }
        public string Type
        {
            get
            {
                return _Type;
            }
            set
            {
                _Type = value;
            }
        }
        public string Name
        {
            get
            {
                return _Name;
            }
            set
            {
                _Name = value;
            }
        }
        public string Value
        {
            get
            {
                return _Value;
            }
            set
            {
                _Value = value;
            }
        }
        public bool MarkedForDelete
        {
            get
            {
                return _MarkedForDelete;
            }
            set
            {
                _MarkedForDelete = value;
            }
        }
    }
}

namespace Ektron.Cms.Commerce.Workarea.ProductTypes.Tabs
{
    public partial class Attributes : System.Web.UI.UserControl
    {
        #region Member Variables

        private bool _IsEditable;
        private ProductTypeData _ProductData;
        private DisplayModeValue _DisplayMode;
        private ContentAPI _contentApi;
        protected SiteAPI _siteApi;
        private String _sitePath;
        private String _applicationPath;
        private int _ItemCount;
        private List<AttributesClientData> _AttributeData;
        protected EkMessageHelper _MessageHelper;

        #endregion

        #region Enumerations

        public enum DisplayModeValue
        {
            View,
            Edit
        }

        #endregion

        #region Properties

        public DisplayModeValue DisplayMode
        {
            get
            {
                return _DisplayMode;
            }
            set
            {
                _DisplayMode = value;
            }
        }

        public ProductTypeData ProductData
        {
            get
            {
                return _ProductData;
            }
            set
            {
                _ProductData = value;
            }

        }

        public bool IsEditable
        {
            get
            {
                return _IsEditable;
            }
            set
            {
                _IsEditable = value;
            }
        }

        protected String SitePath
        {
            get
            {
                return _sitePath;
            }
            set
            {
                _sitePath = value;
            }
        }

        protected String ApplicationPath
        {
            get
            {
                return _applicationPath;
            }
            set
            {
                _applicationPath = value;
            }
        }

        protected int ItemCount
        {
            get
            {
                return _ItemCount;
            }
            set
            {
                _ItemCount = value;
            }
        }

        public List<AttributesClientData> AttributeData
        {
            get
            {
                return _AttributeData;
            }
            set
            {
                _AttributeData = value;
            }
        }

        #endregion

        #region Events

        protected Attributes()
        {
            _contentApi = new ContentAPI();
            _siteApi = new SiteAPI();
            _MessageHelper = _contentApi.EkMsgRef;
            this.SitePath = _contentApi.SitePath.TrimEnd(new char[] { '/' });
            this.ApplicationPath = _siteApi.ApplicationPath.TrimEnd(new char[] { '/' });
        }

        protected void Page_Init(object sender, EventArgs e)
        {
            string attributeJsonData = Request.Form["AttributeData"] ?? String.Empty;
            if (attributeJsonData != String.Empty)
            {
                _AttributeData = (List<AttributesClientData>)JsonConvert.DeserializeObject(attributeJsonData, typeof(List<AttributesClientData>));
            }

            this.RegisterCSS();
            this.RegisterJS();
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            this.IsEditable = (this.DisplayMode == DisplayModeValue.Edit) ? true : false;
            this.ItemCount = (this.ProductData != null) ? this.ProductData.Attributes.Count : 0;

            //set localized strings
            this.SetLocalizedStrings();

            //set empty row visibility
            phEmptyRow.Visible = this.ItemCount == 0 ? true : false;

            //set data field visibility
            phData.Visible = this.IsEditable;

            //set clone row visibility
            phCloneRow.Visible = this.IsEditable;

            //set add attribute button visibility
            phFooter.Visible = this.IsEditable;
            phModal.Visible = this.IsEditable;

            //bind repeater to Attributes data
            if (this.ItemCount > 0)
            {
                rptAttributesView.DataSource = this.ProductData.Attributes;
                rptAttributesView.DataBind();
            }
        }

        protected void rptAttributesView_OnItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            ProductTypeAttributeData entryAttributeData = (ProductTypeAttributeData)e.Item.DataItem;
            EkEnumeration.ProductTypeAttributeDataType attributeType = (EkEnumeration.ProductTypeAttributeDataType)entryAttributeData.DataType;

            bool phBooleanVisible = false;
            bool phNumericVisible = false;
            bool phStringVisible = false;

            switch (attributeType)
            {
                case EkEnumeration.ProductTypeAttributeDataType.Boolean:
                    phBooleanVisible = true;
                    break;
                case EkEnumeration.ProductTypeAttributeDataType.Date:
                    break;
                case EkEnumeration.ProductTypeAttributeDataType.Numeric:
                    phNumericVisible = true;
                    break;
                case EkEnumeration.ProductTypeAttributeDataType.String:
                    phStringVisible = true;
                    break;
            }

            e.Item.FindControl("phBoolean").Visible = phBooleanVisible;
            e.Item.FindControl("phNumeric").Visible = phNumericVisible;
            e.Item.FindControl("phText").Visible = phStringVisible;
            e.Item.FindControl("phDataAndActions").Visible = this.IsEditable;
        }

        #endregion

        #region Helpers

        private void SetLocalizedStrings()
        {
            litAddAttributeOptionText.Text = "Text";
            litAddAttributeOptionBoolean.Text = "Yes/No";
            litAddAttributeOptionDate.Text = "Date";
            litAddAttributeOptionNumeric.Text = "Number";
            litEmptyRowLabel.Text = _MessageHelper.GetMessage("lbl No Attributes");

            //set today's date in server-side date format
            hdnTodaysDate.Value = DateTime.Today.ToString(System.Threading.Thread.CurrentThread.CurrentCulture.DateTimeFormat.ShortDatePattern);
            //set date format
            hdnDateFormat.Value = System.Threading.Thread.CurrentThread.CurrentCulture.DateTimeFormat.ShortDatePattern;
        }

        public string GetFieldsetLabel()
        {
            return "Attributes";
        }

        public string GetLocalizedJavascriptStrings()
        {
            string deleteAttributeMessage = _MessageHelper.GetMessage("js delete product attribute");
            string blankNameFieldMessage = "* " + _MessageHelper.GetMessage("lbl field cannot be blank");

            return @"{
                        ""applicationPath"": """ + this.ApplicationPath + @""",
                        ""blankNameField"": """ + blankNameFieldMessage + @"!"",
                        ""cannotContainSpecialCharacters"": ""Fields cannot include &gt; &lt;"",
                        ""text"": ""Text"",
                        ""booleanText"": ""Yes/No"",
                        ""numeric"": ""Number"",
                        ""date"": ""Date"",
                        ""noDefaultText"": ""Not Set"",
                        ""statusNew"": ""Not Published"",
                        ""statusActive"": ""Active"",
                        ""deleteAttibuteMessage"" : """ + deleteAttributeMessage + @"!"" 
                    }";
        }

        public string GetBooleanFriendlyLabel(Object e)
        {
            EntryAttributeData dataItem = (EntryAttributeData)e;
            return ((dataItem.DataType == EkEnumeration.ProductTypeAttributeDataType.Boolean) && ((bool)dataItem.CurrentValue == true)) ? @"Yes" : @"No";
        }

        public string GetBooleanSelectedItem(Object e, bool optionValue)
        {
            ProductTypeAttributeData dataItem = (ProductTypeAttributeData)e;
            return ((dataItem.DataType == EkEnumeration.ProductTypeAttributeDataType.Boolean) && (dataItem.DefaultValue.ToString().ToLower() == optionValue.ToString().ToLower())) ? @"selected=""selected""" : String.Empty;
        }

        public string GetDataTypeFriendlyLabel(Object e)
        {
            string returnValue = String.Empty;
            switch ((EkEnumeration.ProductTypeAttributeDataType)e)
            {
                case EkEnumeration.ProductTypeAttributeDataType.String:
                    returnValue = "Text";
                    break;
                case EkEnumeration.ProductTypeAttributeDataType.Numeric:
                    returnValue = "Number";
                    break;
                case EkEnumeration.ProductTypeAttributeDataType.Boolean:
                    returnValue = "Yes/No";
                    break;
                case EkEnumeration.ProductTypeAttributeDataType.Date:
                    returnValue = "Date";
                    break;
                default:
                    returnValue = "Unknown";
                    break;
            }
            return returnValue;
        }

        public string GetInactiveStatusClass(Object e)
        {
            ProductTypeAttributeData dataItem = (ProductTypeAttributeData)e;
            return dataItem.ActiveStatus == EkEnumeration.ProductTypeAttributeStatus.Inactive ? @"markedForDelete " : String.Empty;
        }

        public string GetInactiveStatusMarkedForDelete(Object e)
        {
            ProductTypeAttributeData dataItem = (ProductTypeAttributeData)e;
            return dataItem.ActiveStatus == EkEnumeration.ProductTypeAttributeStatus.Inactive ? @"true" : "false";
        }

        public string GetButtonInactiveStatusClass(Object e, string buttonClass)
        {
            ProductTypeAttributeData dataItem = (ProductTypeAttributeData)e;
            string display = String.Empty;
            string displayNone = @"style=""display:none""";
            string displayBlock = @"style=""display:inline""";
            switch (buttonClass)
            {
                case "edit":
                    display = dataItem.ActiveStatus == EkEnumeration.ProductTypeAttributeStatus.Inactive ? displayNone : displayBlock;
                    break;
                case "markedForDelete":
                    display = dataItem.ActiveStatus == EkEnumeration.ProductTypeAttributeStatus.Inactive ? displayNone : displayBlock;
                    break;
                case "restore":
                    display = dataItem.ActiveStatus == EkEnumeration.ProductTypeAttributeStatus.Inactive ? displayBlock : displayNone;
                    break;
            }
            return display;
        }

        public string GetPublishedStatusFriendlyLabel(Object e)
        {
            string returnValue = String.Empty;
            ProductTypeAttributeData dataItem = (ProductTypeAttributeData)e;
            switch (dataItem.ActiveStatus)
            {
                case EkEnumeration.ProductTypeAttributeStatus.ActiveAndVisible:
                    returnValue = "Active";
                    break;
                case EkEnumeration.ProductTypeAttributeStatus.Inactive:
                    returnValue = "Inactive";
                    break;
            }
            return returnValue;
        }

        public string GetFriendlyDefaultValue(Object e)
        {
            string returnValue = e.ToString();
            returnValue = (returnValue.ToLower() == "true") ? "Yes" : returnValue;
            returnValue = (returnValue.ToLower() == "false") ? "No" : returnValue;
            return returnValue;
        }

        #endregion

        #region JS, CSS, Images

        public string GetImagePath()
        {
            return this.ApplicationPath + "/Commerce/ProductTypes/Attributes/css/images";
        }

        private void RegisterJS()
        {

            if (Request.QueryString["action"] != null && Request.QueryString["action"] != "" && Request.QueryString["action"] != "viewproducttype")
            {

                Ektron.Cms.Framework.UI.Packages.jQuery.jQueryUI.Datepicker.Register(this);
                JS.RegisterJS(this, JS.ManagedScript.EktronJsonJS);
                JS.RegisterJS(this, JS.ManagedScript.EktronModalJS);
                JS.RegisterJS(this, JS.ManagedScript.EktronDnRJS);
                JS.RegisterJS(this, this.ApplicationPath + "/Commerce/ProductTypes/Attributes/js/metadata_associations.js", "EktronCommerceAttributesMetadataAssociationsJs");
                JS.RegisterJS(this, this.ApplicationPath + "/java/ektron.dateformat.js", "EktronDateFormatJs");
                JS.RegisterJS(this, this.ApplicationPath + "/Commerce/ProductTypes/Attributes/js/Attributes.js", "EktronCommerceAttributesJs");

            }

        }

        private void RegisterCSS()
        {

            Css.RegisterCss(this, Css.ManagedStyleSheet.EktronModalCss);
            Css.RegisterCss(this, this.ApplicationPath + "/Commerce/ProductTypes/Attributes/css/Attributes.css", "EktronCommerceAttributesCss");
            Css.RegisterCss(this, this.ApplicationPath + "/Commerce/ProductTypes/Attributes/css/Attributes.ie6.css", "EktronCommerceAttributesIe6Css", Css.BrowserTarget.LessThanEqualToIE6);
            Css.RegisterCss(this, Css.ManagedStyleSheet.EktronWorkareaCss);
            Css.RegisterCss(this, Css.ManagedStyleSheet.EktronWorkareaIeCss, Css.BrowserTarget.LessThanEqualToIE7);

        }

        public string GetMediaImageImagePath()
        {
            return this.ApplicationPath + "/Commerce/ProductTypes/Attributes/css/images";
        }

        #endregion
    }
}