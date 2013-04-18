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
    public class MediaDefaultsClientData
    {
        private int _Order;
        private long _Id;
        private string _Name;
        private string _Width;
        private string _Height;
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
        public string Width
        {
            get
            {
                return _Width;
            }
            set
            {
                _Width = value;
            }
        }
        public string Height
        {
            get
            {
                return _Height;
            }
            set
            {
                _Height = value;
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
    public partial class MediaDefaults : System.Web.UI.UserControl
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
        private List<MediaDefaultsClientData> _ClientData;
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

        /// <summary>
        /// Contains data returned by the client - populated OnInit during postback
        /// </summary>
        public List<MediaDefaultsClientData> ClientData
        {
            get
            {
                return _ClientData;
            }
        }

        private EkMessageHelper MessageHelper
        {
            get
            {
                return _MessageHelper;
            }
            set
            {
                _MessageHelper = value;
            }
        }

        #endregion

        #region Constructor

        protected MediaDefaults()
        {
            _contentApi = new ContentAPI();
            _siteApi = new SiteAPI();
            _MessageHelper = _contentApi.EkMsgRef;

            this.SitePath = _contentApi.SitePath.TrimEnd(new char[] { '/' });
            this.ApplicationPath = _siteApi.ApplicationPath.TrimEnd(new char[] { '/' });
        }

        #endregion

        #region OnInit, Page_Load, rptAttributesView_OnItemDataBound

        protected void Page_Init(object sender, EventArgs e)
        {
            string attributeJsonData = Request.Form["MediaDefaultsData"] ?? String.Empty;
            if (attributeJsonData != String.Empty)
            {
                _ClientData = (List<MediaDefaultsClientData>)JsonConvert.DeserializeObject(attributeJsonData, typeof(List<MediaDefaultsClientData>));
            }

            this.RegisterCSS();
            this.RegisterJS();
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            this.IsEditable = (this.DisplayMode == DisplayModeValue.Edit) ? true : false;
            this.ItemCount = (this.ProductData != null) ? this.ProductData.DefaultThumbnails.Count : 0;

            //set localized strings
            this.SetLocalizedStrings();

            //set empty row visibility
            phEmptyRow.Visible = this.ItemCount == 0 ? true : false;

            //set data field visibility
            phData.Visible = this.IsEditable;

            //set clone row visibility
            phCloneRow.Visible = this.IsEditable;

            //set add attribute button visibility
            phAddThumbnail.Visible = this.IsEditable;
            phModal.Visible = this.IsEditable;
            phActionsHeader.Visible = this.IsEditable;

            //bind repeater to Attributes data
            if (this.ItemCount > 0)
            {
                rptAttributesView.DataSource = this.ProductData.DefaultThumbnails;
                rptAttributesView.DataBind();
            }
        }

        protected void rptAttributesView_OnItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            e.Item.FindControl("phActions").Visible = this.IsEditable;
        }

        public string GetTitle(Object e)
        {
            string title = e.ToString();
            return title.Replace("[filename]", "");
        }

        #endregion

        #region Localizers

        private void SetLocalizedStrings()
        {
            litEmptyRowLabel.Text = GetMessage("lbl thumbnails defaults not set");
        }

        public string GetRequiredText()
        {
            return "*" + GetMessage("lbl field cannot be blank");
        }

        public string GetNameLabel()
        {
            return GetMessage("generic name");
        }

        public string GetWidthLabel()
        {
            return GetMessage("lbl width");
        }

        public string GetHeightLabel()
        {
            return GetMessage("lbl height");
        }

        public string GetActionsLabel()
        {
            return GetMessage("generic actions");
        }

        public string GetAddThumbnailLabel()
        {
            return GetMessage("lbl add thumbnail");
        }

        public string GetCaption()
        {
            return GetMessage("lbl gen thmbnl");
        }

        public string GetLocalizedJavascriptStrings()
        {
            string widthHeightNumericMessage = _MessageHelper.GetMessage("lbl width & height numeric");
            string duplicateNameField = _MessageHelper.GetMessage("lbl name must be unique");
            string blankNameFieldMessage = _MessageHelper.GetMessage("lbl field cannot be blank");
            string specialCharactersMessage = _MessageHelper.GetMessage("lbl fields cannot include characters included");
            string notSet = _MessageHelper.GetMessage("lbl not set");

            return @"{
                        ""blankNameField"": """ + blankNameFieldMessage + @"!"",
                        ""duplicateNameField"": """ + duplicateNameField + @"!"",
                        ""cannotContainSpecialCharacters"": """ + specialCharactersMessage + @"!"",
                        ""cannotBeAlphabetic"": """ + widthHeightNumericMessage + @"!"",
                        ""noDefaultText"": """ + notSet + @"""
                    }";
        }

        public string GetMessage(string messageTitle)
        {
            return this.MessageHelper.GetMessage(messageTitle);
        }

        #endregion

        #region Helpers

        public string GetEmptyRowColspan()
        {
            return this.IsEditable == true ? "4" : "3";
        }  

        #endregion

        #region JS, CSS, Images

        private void RegisterJS()
        {

            JS.RegisterJS(this, JS.ManagedScript.EktronJS);
            JS.RegisterJS(this, JS.ManagedScript.EktronJsonJS);
            JS.RegisterJS(this, JS.ManagedScript.EktronModalJS);
            JS.RegisterJS(this, JS.ManagedScript.EktronDnRJS);
            JS.RegisterJS(this, this.ApplicationPath + "/Commerce/ProductTypes/MediaDefaults/js/MediaDefaults.js", "EktronCommerceMediaDefaultsJs");
            JS.RegisterJS(this, JS.ManagedScript.EktronWorkareaJS);

        }

        private void RegisterCSS()
        {

            Css.RegisterCss(this, Css.ManagedStyleSheet.EktronModalCss);
            Css.RegisterCss(this, this.ApplicationPath + "/Commerce/ProductTypes/MediaDefaults/css/MediaDefaults.css", "EktronCommerceMediaDefaultsCss");
            Css.RegisterCss(this, Css.ManagedStyleSheet.EktronWorkareaCss);
            Css.RegisterCss(this, Css.ManagedStyleSheet.EktronWorkareaIeCss, Css.BrowserTarget.LessThanEqualToIE7);

        }

        public string GetImagePath()
        {
            return this.ApplicationPath + "/Commerce/ProductTypes/MediaDefaults/css/images";
        }

        #endregion
    }
}