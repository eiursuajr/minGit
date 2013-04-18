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
using System.Text;
using Ektron;
using Ektron.Cms;
using Ektron.Cms.Common;
using Ektron.Cms.Workarea;

namespace Ektron.Cms.Commerce.Workarea.CatalogEntry
{
    public partial class CatalogEntry_PageFunctions_Js : System.Web.UI.Page
    {
        #region Member Variables

        private ContentAPI _ContentApi;
        private EkMessageHelper _MessageHelper;
        private workareajavascript _JsLibrary;
        private string _ApplicationPath;

        #endregion

        #region Properties

        public ContentAPI ContentApi
        {
            get
            {
                return _ContentApi;
            }
            set
            {
                _ContentApi = value;
            }
        }

        public EkMessageHelper MessageHelper
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

        public workareajavascript JsLibrary
        {
            get
            {
                return _JsLibrary;
            }
            set
            {
                _JsLibrary = value;
            }
        }

        protected String ApplicationPath
        {
            get
            {
                return _ApplicationPath;
            }
            set
            {
                _ApplicationPath = value;
            }
        }

        #endregion

        #region Constructor

        protected CatalogEntry_PageFunctions_Js()
        {
            _ContentApi = new ContentAPI();
            _MessageHelper = _ContentApi.EkMsgRef;
            _JsLibrary = new workareajavascript();
            _ApplicationPath = _ContentApi.ApplicationPath.TrimEnd(new char[] { '/' });
        }

        #endregion

        #region Init

        protected override void OnInit(EventArgs e)
        {
            this.Context.Response.Charset = "utf-8";
            this.Context.Response.ContentType = "application/javascript";

            //set js server variables
            this.SetJsServerVariables();

            base.OnInit(e);
        }

        #endregion

        #region Methods - set js server variables

        private void SetJsServerVariables()
        {
            litEditorName.Text = Request.QueryString["id"];

            //show taxonomy required conidtion only if taxonomy required is set to true
            string taxonomyRequired = Request.QueryString["taxonomyRequired"] ?? "false";
            phTaxonomyCategoryRequired.Visible = taxonomyRequired.ToLower() == "true" ? true : false;

            //show manual alias required conidtion only if taxonomy required is set to true
            string aliasRequired = Request.QueryString["aliasRequired"] ?? "false";
            phAliasRequired.Visible = aliasRequired.ToLower() == "true" ? true : false;

            //set internationalized alert strings
            litMessageConfirmCloseNoSaveEntry1.Text = GetMessage("js alert confirm close no save entry");
            litMessageConfirmCloseNoSaveEntry2.Text = GetMessage("js alert confirm close no save entry");
            litMessageErrorEntryTitleRequired.Text = GetMessage("js err entry title req");
            litMessageErrorBillingInterval.Text = GetMessage("js billing interval greater than 0");
            litMessageEntryDisallowedCharacters.Text = GetMessage("lbl entry disallowed chars");
            litMessageErrorEntryQuantityRequired.Text = GetMessage("js err entry quantity req");
            litMessageNoItems.Text = GetMessage("js no items");
            litMessageNotEnoughItemsBundle.Text = GetMessage("js not enough items bundle");
            litMessageNotEnoughItemsBundle1.Text = GetMessage("js not enough items bundle");
            litMessageNotEnoughItemsKit.Text = GetMessage("js not enough items kit");
            litMessageNotEnoughItemsKit1.Text = GetMessage("js not enough items kit");
            litMessageNotValidDimensions.Text = GetMessage("js not valid dimensions");
            litMessageNotValidInventory.Text = GetMessage("js not valid inventory");
            litInvalidExcessDimensions.Text = GetMessage("js invalid excess dimensions");
            litMessageNotValidList.Text = GetMessage("js not valid list");
            litMessageNotValidNumericAttribute.Text = GetMessage("js not valid numeric attribute");
            litMessageEmptyDateAttribute.Text = GetMessage("js not valid date attribute");             
            litMessageNotValidPrice.Text = GetMessage("js not valid price");
            litMessageNotValidPropQuantity.Text = GetMessage("js not valid prop quantity");
            litMessageNotValidPurchase.Text = GetMessage("js not valid purchase");
            litMessageNotValidQuantity.Text = GetMessage("js not valid quantity");
            litMessageNotValidSales.Text = GetMessage("js not valid sales");
            litMessageNotValidSku.Text = GetMessage("js not valid sku");
            litMessagePleaseSelectItem.Text = GetMessage("js please sel item");
            litMessageTaxonomyCategoryRequired.Text = GetMessage("js tax cat req");
            litMessageAliasRequired.Text = GetMessage("js manual alias req");
            litConfirmBasePrice.Text = GetMessage("js cnfrm baseprice");
            litConfirmFloatPrice.Text = GetMessage("js cnfrm floatprice");
            litCannotContaintSpecialCharacters.Text = "SKU " + GetMessage("js alert field cannot include")+ " < >";
        }

        private string GetMessage(string messageKey)
        {
            return this.MessageHelper.GetMessage(messageKey);
        }

        public string GetJsLibraryResetErrorFunctionName()
        {
            return this.JsLibrary.ResetErrorFunctionName;
        }

        public string GetJsLibraryAddErrorFunctionName()
        {
            return this.JsLibrary.AddErrorFunctionName;
        }

        public string GetJsLibraryToggleDiv()
        {
            return this.JsLibrary.ToggleDiv();
        }

        public string GetJsLibraryRemoveHTML()
        {
            return this.JsLibrary.RemoveHTML();
        }

        public string GetJsLibraryToggleDivFunctionName()
        {
            return this.JsLibrary.ToggleDivFunctionName;
        }

        public string GetJsLibraryShowErrorFunctionName()
        {
            return this.JsLibrary.ShowErrorFunctionName;
        }

        public string GetId()
        {
            return Request.QueryString["taxonomyRequired"] ?? "-1";
        }

        public string GetFolderId()
        {
            return Request.QueryString["folder_id"] ?? "-1";
        }

        public EkEnumeration.CatalogEntryType GetEntryType()
        {
            return (EkEnumeration.CatalogEntryType)Enum.Parse(typeof(EkEnumeration.CatalogEntryType), Request.QueryString["entryType"]);
        }

        public string GetJsLibraryAddError()
        {
            return this.JsLibrary.AddError("aSubmitErr");
        }

        public string GetJsLibraryShowError()
        {
            return this.JsLibrary.ShowError("aSubmitErr");
        }

        public string GetJsLibraryResetError()
        {
            return this.JsLibrary.ResetError("aSubmitErr");
        }

        public string GetJsLibraryHasIllegalCharacters()
        {
            return this.JsLibrary.HasIllegalCharacters(workareajavascript.ErrorType.ErrorCollection);
        }

        public string GetJsLibraryResizeFrame()
        {
            return this.JsLibrary.ResizeFrame();
        }

        #endregion
    }
}