using System;
using System.Data;
using System.Configuration;
using System.Collections;
using System.Collections.Generic;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using Ektron.Cms.Controls;
using Ektron.Cms.Controls.Commerce.Cart;
using Ektron.Cms.Controls.Commerce.CartControl;

public partial class CartUserControl : System.Web.UI.UserControl,
    Ektron.Cms.Controls.Commerce.CartControl.ICartUserControl
{
    #region private member variables

    private string _ApplicationImagePath = "../../../images/application/"; //String.Empty;
    private long _cartId = 0;
    private string _controlClientId = String.Empty;
    private ICartViewToPresenter _iCartViewToPresenter = null;
    private long _LanguageId = 0;
    private string _productUrl = String.Empty;
    private string _UserImagePath = String.Empty;
    private string _controlBasePath = String.Empty;

    #endregion

    #region protected methods

    protected override void OnInit(EventArgs e)
    {
        base.OnInit(e);
    }

    protected void Page_Load(object sender, EventArgs e)
    {
        if (null != Presenter)
            Presenter.PageLoadHandler();

        if (IsPostBack && null != Presenter)
        {
            string action = ActionCode.Value;
            ActionCode.Value = String.Empty;
            switch (action)
            {
                case "DeleteCart":
                    DeleteCart_Click(null, new EventArgs());
                    break;

                case "SelectCart":
                    SelectCart_Click(null, new EventArgs());
                    break;

                case "RemoveItem":
                    RemoveItem_Click(null, new EventArgs());
                    break;

                case "RemoveCoupon":
                    RemoveCoupon_Click(null, new EventArgs());
                    break;
            }
        }
    }

    public void EktronPageInit()
    {
        // remove design-time only items:
        DesignTimeWrapper.Text = String.Empty;

        // Initilize:
        CurrentCartTableBody.Controls.Clear();
        CurrentCartTableBody.InnerHtml = String.Empty;
        SavedCartTableBody.Controls.Clear();
        SavedCartTableBody.InnerHtml = String.Empty;
        SetSubTotal("");
        SavedCartContainer.Visible = false;
        AdvisoryMessageContainer.Visible = false;
        InitailizeUITextStrings();
        InitailizeOnClickEvents();
        InitailizeImages();
    }

    #endregion

    #region Ektron methods

    protected void InitailizeUITextStrings()
    {
        // initialize UI text strings:
        Text_YourShoppingCartString.InnerText = GetResourceString("lbl your shopping cart", "Your Cart");
        Text_CartLabelString.InnerText = GetResourceString("lbl cart label", "Cart:");
        Text_ForCurrentCart_ItemColumn.InnerText = GetResourceString("lbl catalog item", "Item");
        Text_ForCurrentCart_RemoveColumn.InnerText = GetResourceString("generic remove", "Remove");
        Text_ForCurrentCart_SkuColumn.InnerText = GetResourceString("lbl calatog entry sku", "SKU");
        Text_ForCurrentCart_QuantityColumn.InnerText = GetResourceString("lbl quantity", "Quantity");
        Text_ForCurrentCart_ListPriceColumn.InnerText = GetResourceString("lbl list price", "List Price");
        Text_ForCurrentCart_SalePriceColumn.InnerText = GetResourceString("lbl sale price", "Sale Price");
        Text_ForCurrentCart_TotalColumn.InnerText = GetResourceString("lbl total", "Total");
        Text_ForSavedCarts_SubtotalColumn.InnerText = Text_ForTextIn_SubtotalLabel.InnerText
            = GetResourceString("lbl subtotal", "Subtotal");
        Text_ForSavedCarts_CartColumn.InnerText = GetResourceString("lbl total", "Cart");
        Text_ForSavedCarts_LastUpdatedColumn.InnerText = GetResourceString("lbl last updated", "Last Updated");
        Text_ForSavedCarts_ItemsColumn.InnerText = GetResourceString("lbl items", "Items");
        Text_ForSavedCarts_DeleteColumn.InnerText = GetResourceString("btn delete", "Delete");
        Text_YourSavedCartsString.InnerText = GetResourceString("lbl your saved carts", "Your Saved Carts");
        Text_ForEnterCouponCode.InnerText = GetResourceString("lbl enter coupon code label", "Enter Coupon Code");

        // initialize link text strings:
        Link_ApplyCouponButton.Title = Text_ForTextIn_Link_ApplyCouponButton.InnerText = GetResourceString("lbl apply coupon", "Apply Coupon");
        Link_Checkout.Title = Text_ForTextIn_Link_Checkout.InnerText = GetResourceString("lbl checkout", "Checkout");
        Link_ContinueShopping_1.Title = Text_ForTextIn_Link_ContinueShopping_1.InnerText
            = Link_ContinueShopping_2.Title
            = Text_ForTextIn_Link_ContinueShopping_2.InnerText
            = GetResourceString("lbl continue shopping", "Continue Shopping");
        Link_CreateNewCart.Title = Text_ForTextIn_Link_CreateNewCart.InnerText = GetResourceString("lbl create new cart", "Create New Cart");
        Link_RemoveAllCartItems.ToolTip = Text_ForTextIn_Link_RemoveAllCartItems.InnerText = GetResourceString("lbl empty cart", "Empty Cart");
        Link_RenameCart.Title = GetResourceString("lbl rename cart", "Rename Cart");
        Text_ForTextIn_Link_UpdateSubtotal.InnerText = GetResourceString("lbl update subtotal", "Update Subtotal");
    }

    protected void InitailizeOnClickEvents()
    {
        // initialize onclick events; callbacks:
        Link_RemoveAllCartItems.Click += new EventHandler(EmptyCart_Click);
        Link_UpdateSubtotal.Click += new EventHandler(UpdateSubtotal_Click);
        Image_ApplyCouponOk.Click += new ImageClickEventHandler(ApplyCoupon_Click);
        Image_CreateCartOk.Click += new ImageClickEventHandler(CreateCart_Click);
        Image_RenameCartOK.Click += new ImageClickEventHandler(RenameCart_Click);

        // initialize onclick events; handled entirely client-side:
        Link_ApplyCouponButton.Attributes.Add("onclick", "$ektron('#" + ApplyCouponUI.ClientID
            + "').show(); $ektron('#" + Link_ApplyCouponButton.ClientID + "').hide(); return false;");
        Image_ApplyCouponCancel.Attributes.Add("onclick", "$ektron('#" + ApplyCouponUI.ClientID
            + "').hide(); $ektron('#" + Link_ApplyCouponButton.ClientID + "').show(); return false;");
        Link_CreateNewCart.Attributes.Add("onclick", "$ektron('#" + CreateCartUI.ClientID
            + "').show(); $ektron('#" + Link_CreateNewCart.ClientID + "').hide(); return false;");
        Image_CreateCartCancel.Attributes.Add("onclick", "$ektron('#" + CreateCartUI.ClientID
            + "').hide(); $ektron('#" + Link_CreateNewCart.ClientID + "').show(); return false;");
        Link_RenameCart.Attributes.Add("onclick", "$ektron('#" + RenameCartUI.ClientID
            + "').show(); $ektron('#" + Link_RenameCart.ClientID + "').hide(); return false;");
        Image_RenameCartCancel.Attributes.Add("onclick", "$ektron('#" + RenameCartUI.ClientID
            + "').hide(); $ektron('#" + Link_RenameCart.ClientID + "').show(); return false;");
    }

    protected void InitailizeImages()
    {
        // initialize image sources:
        Image_Checkout.Src = ApplicationImagePath + "/commerce/checkout.gif";
        Image_ContinueShopping_1.Src = Image_ContinueShopping_2.Src = ApplicationImagePath + "/commerce/continueShopping.gif";
        Image_Coupon.Src = ApplicationImagePath + "/commerce/coupon.gif";
        Image_CreateNewCart.Src = ApplicationImagePath + "/commerce/createNewBasket.gif";
        Image_DeleteCart_1.Src = Image_DeleteCart_2.Src = Image_DeleteCart_3.Src = ApplicationImagePath + "/commerce/deleteCart.gif";
        Image_RemoveFromCart_1.Src = Image_RemoveFromCart_2.Src = ApplicationImagePath + "/commerce/removefromcart2.gif";
        Image_RenameCart.Src = ApplicationImagePath + "/commerce/renameCart.gif";
        Image_RenameCartCancel.Src = Image_ApplyCouponCancel.Src = Image_CreateCartCancel.Src
            = ApplicationImagePath + "/commerce/renameBasketCancel.gif";
        Image_RenameCartOK.ImageUrl = Image_ApplyCouponOk.ImageUrl = Image_CreateCartOk.ImageUrl = ApplicationImagePath + "/commerce/renameBasketOK.gif";
        Image_UpdateSubtotal.Src = ApplicationImagePath + "/commerce/updateSubtotal.gif";

        // initialize image alt-text:
        Image_Checkout.Alt = GetResourceString("lbl checkout", "Checkout");
        Image_ContinueShopping_1.Alt = Image_ContinueShopping_2.Alt = GetResourceString("lbl continue shopping", "Continue Shopping");
        Image_Coupon.Alt = GetResourceString("lbl apply coupon", "Apply Coupon");
        Image_CreateNewCart.Alt = GetResourceString("lbl create new cart", "Create New Cart");
        Image_DeleteCart_1.Alt = Image_DeleteCart_2.Alt = Image_DeleteCart_3.Alt = GetResourceString("lbl delete cart", "Delete Cart");
        Image_RemoveFromCart_1.Alt = Image_RemoveFromCart_2.Alt = GetResourceString("lbl remove from cart", "Remove From Cart");
        Image_RenameCart.Alt = GetResourceString("lbl rename cart", "Rename Cart");
        Image_RenameCartCancel.Alt = Image_ApplyCouponCancel.Alt = Image_CreateCartCancel.Alt = GetResourceString("btn cancel", "Cancel");
        Image_RenameCartOK.ToolTip = Image_ApplyCouponOk.ToolTip = Image_CreateCartOk.ToolTip = GetResourceString("lbl ok", "Ok");
        Image_UpdateSubtotal.Alt = GetResourceString("lbl update subtotal", "Update Subtotal");
    }

    protected string GetResourceString(string key, string defaultText)
    {
        return ((null != Presenter) ? Presenter.GetResourceString(key, defaultText) : defaultText);
    }

    #endregion

    #region helper properties and methods

    public string ApplicationImagePath { get { return _ApplicationImagePath; } set { _ApplicationImagePath = value; } }

    public ICartViewToPresenter Presenter { get { return _iCartViewToPresenter; } }

    protected bool IsActiveCart(long cartId) { return (cartId == _cartId); }

    #endregion

    #region interface implementation

    // Accessors for direct UI manipulation, and support:

    public string GetCouponCode()
    { return TextBox_ApplyCoupon.Text; }

    public void SetAdvisoryMessage(string message)
    {
        Text_ForTextIn_AdvisoryMessage.InnerText = message;
        if (message.Length > 0)
            AdvisoryMessageContainer.Visible = true;
        else
            AdvisoryMessageContainer.Visible = false;
    }

    public void SetApplicationImagePath(string applicationImagePath)
    {
        ApplicationImagePath = applicationImagePath;
        InitailizeImages();
    }

    public void SetBasePath(string basePath)
    { _controlBasePath = basePath; }

    public void SetCheckoutUrl(string checkoutUrl)
    { Link_Checkout.HRef = checkoutUrl; }

    public void SetControlClientId(string controlClientId)
    { _controlClientId = controlClientId; }

    public void SetCoupons(List<ICartCoupon> coupons)
    {
        System.Web.UI.HtmlControls.HtmlTableRow row;
        System.Web.UI.HtmlControls.HtmlTableCell cell;
        System.Web.UI.WebControls.LinkButton linkButton;
        System.Web.UI.HtmlControls.HtmlImage image;
        //System.Web.UI.HtmlControls.HtmlInputText text;
        System.Web.UI.HtmlControls.HtmlGenericControl genericCtl;

        foreach (ICartCoupon coupon in coupons)
        {
            // column one:
            cell = new HtmlTableCell();
            cell.Attributes.Add("class", "colDiscountCoupon");
            cell.Attributes.Add("colspan", "6");
            cell.InnerHtml = GetResourceString("lbl coupon code label", "Coupon Code ") + "<span class='couponCode'>" + coupon.Code + "</span>, " + GetResourceString("lbl discount label", "Discount");
            row = new HtmlTableRow();
            row.Controls.Add(cell);

            // column two:
            cell = new HtmlTableCell();
            cell.Attributes.Add("class", "colTotal ieBorderFix");

            genericCtl = new HtmlGenericControl("span");
            genericCtl.Attributes.Add("class", "couponDiscountAmount");
            genericCtl.InnerText = coupon.Discount;
            cell.Controls.Add(genericCtl);

            image = new HtmlImage();
            image.Alt = GetResourceString("lbl remove coupon", "Remove Coupon");
            image.Src = ApplicationImagePath + "commerce/removefromcart2.gif";
            CouponEventArgs args = new CouponEventArgs(coupon.Code);
            linkButton = new LinkButton();
            //linkButton.Click += delegate { RemoveCoupon_Click(linkButton, args); };
            linkButton.OnClientClick = "document.getElementById('" + ActionCode.ClientID + "').value = 'RemoveCoupon';"
                + "document.getElementById('" + ActionArgument.ClientID + "').value = '" + coupon.Code + "';";
            linkButton.Attributes.Add("class", "removeCoupon");
            linkButton.ToolTip = GetResourceString("lbl remove from cart", "Remove From Cart");
            linkButton.Controls.Add(image);
            cell.Controls.Add(linkButton);

            row.Controls.Add(cell);
            row.Attributes.Add("class", "rowSku");
            row.Controls.Add(cell);

            CurrentCartTableBody.Controls.Add(row);
        }
    }

    private void ApplyCoupon_Click(object sender, ImageClickEventArgs e)
    {
        if (null != Presenter)
            Presenter.ApplyCouponHandler(TextBox_ApplyCoupon.Text);
    }

    private void CreateCart_Click(object sender, ImageClickEventArgs e)
    {
        if (null != Presenter)
            Presenter.CreateCartHandler(TextBox_CreateCart.Text);
    }

    private void DeleteCart_Click(object sender, EventArgs e)
    {
        if (null == Presenter)
            return;

        long id = 0;
        string actionArgument = ActionArgument.Value;
        long.TryParse(actionArgument, out id);
        Presenter.DeleteCartHandler(id);
    }

    private void EmptyCart_Click(object sender, EventArgs e)
    {
        if (null != Presenter)
            Presenter.EmptyCartHandler();
    }

    private void RemoveCoupon_Click(object sender, EventArgs e)
    {
        if (null != Presenter)
            Presenter.RemoveCouponHandler(ActionArgument.Value);
    }

    private void RemoveItem_Click(object sender, EventArgs e)
    {
        if (null == Presenter)
            return;

        long id = 0;
        string actionArgument = ActionArgument.Value;
        long.TryParse(actionArgument, out id);
        Presenter.RemoveItemHandler(id);
    }

    private void RenameCart_Click(object sender, ImageClickEventArgs e)
    {
        if (null != Presenter)
            Presenter.RenameCartHandler(TextBox_RenameCart.Text);
    }

    private void SelectCart_Click(object sender, EventArgs e)
    {
        if (null == Presenter)
            return;

        long id = 0;
        string actionArgument = ActionArgument.Value;
        long.TryParse(actionArgument, out id);
        Presenter.SelectCartHandler(id);
    }

    private void UpdateSubtotal_Click(object sender, EventArgs e)
    {
        if (null != Presenter)
            Presenter.UpdateSubtotalHandler();
    }

    public string GetProductQuantity(int cartItemIndex)
    {
        TextBox textBox = (TextBox)CurrentCartTableBody.FindControl("CartItems_ProductCount_" + cartItemIndex);
        if (null == textBox)
            return String.Empty;

        return textBox.Text;
    }

    public string GetProductCode(int cartItemIndex)
    {
        HiddenField hidden = (HiddenField)CurrentCartTableBody.FindControl("CartItems_ProductCode_" + cartItemIndex);
        if (null == hidden)
            return String.Empty;

        return hidden.Value;
    }

    public void SetICartViewToPresenter(ICartViewToPresenter interfaceReference)
    { _iCartViewToPresenter = interfaceReference; }

    public void SetEmpty()
    {
        CurrentCartTableBody.Controls.Clear();
        CurrentCartTableBody.InnerHtml = String.Empty;
        Text_ForTextIn_Subtotal.InnerText = "";
        Link_RemoveAllCartItems.Visible = false;
        CheckoutActionsWrapper.Visible = false;
        EmptyCartNotice.Visible = true;
    }

    public void SetNotEmpty()
    {
        Link_RemoveAllCartItems.Visible = true;
        CheckoutActionsWrapper.Visible = true;
        EmptyCartNotice.Visible = false;
    }

    public void SetId(long cartId) { _cartId = cartId; }

    public void SetItems(List<ICartItem> items)
    {
        System.Web.UI.HtmlControls.HtmlTableRow row;
        System.Web.UI.HtmlControls.HtmlTableCell cell;
        System.Web.UI.HtmlControls.HtmlAnchor anchor;
        System.Web.UI.WebControls.LinkButton linkButton;
        System.Web.UI.HtmlControls.HtmlImage image;
        //System.Web.UI.HtmlControls.HtmlInputText text;
        System.Web.UI.HtmlControls.HtmlGenericControl genericCtl;
        bool stripe = true;
        int count = 0;

        CurrentCartTableBody.Controls.Clear();
        foreach (ICartItem item in items)
        {
            stripe = !stripe;

            // column one:
            anchor = new HtmlAnchor();
            anchor.HRef = (!String.IsNullOrEmpty(_productUrl) ? _productUrl + "?id=" + item.ProductId.ToString() : "#");
            anchor.InnerText = item.Name + (item.NameExtended.Length > 0 ? " - " + item.NameExtended : "");
            anchor.Title = item.Name;

            cell = new HtmlTableCell();
            cell.Attributes.Add("class", "colItemName");
            cell.Controls.Add(anchor);

            foreach (IKitItem kitItem in item.KitItems)
            {
                genericCtl = new HtmlGenericControl("div");
                genericCtl.Attributes.Add("class", "colItemNameKitNames");
                genericCtl.InnerText = kitItem.Name + ": " + kitItem.OptionName;
                cell.Controls.Add(genericCtl);
            }

            row = new HtmlTableRow();
            row.Controls.Add(cell);
            row.Attributes.Add("class", stripe ? "rowSku stripe" : "rowSku");


            // column two:
            image = new HtmlImage();
            image.Alt = GetResourceString("lbl remove from cart", "Remove From Cart");
            image.Src = ApplicationImagePath + "commerce/removefromcart2.gif";

            linkButton = new LinkButton();
            linkButton.OnClientClick = "document.getElementById('" + ActionCode.ClientID + "').value = 'RemoveItem';"
                + "document.getElementById('" + ActionArgument.ClientID + "').value = '" + item.Id.ToString() + "';";
            linkButton.ToolTip = GetResourceString("lbl remove from cart", "Remove From Cart");
            linkButton.Controls.Add(image);

            cell = new HtmlTableCell();
            cell.Attributes.Add("class", "colRemove");
            cell.Controls.Add(linkButton);
            row.Controls.Add(cell);


            // column three:
            cell = new HtmlTableCell();
            cell.Attributes.Add("class", "colProductId");
            cell.InnerText = item.Sku;
            row.Controls.Add(cell);


            // column four:
            System.Web.UI.WebControls.TextBox textBox = new TextBox();
            textBox.Attributes.Add("class", "productQtyText");
            textBox.Text = item.Count.ToString();
            textBox.ID = "CartItems_ProductCount_" + count.ToString();
            cell = new HtmlTableCell();
            cell.Attributes.Add("class", "colQty");
            cell.Controls.Add(textBox);

            System.Web.UI.WebControls.HiddenField hidden = new HiddenField();
            hidden.ID = "CartItems_ProductCode_" + count.ToString();
            hidden.Value = item.ProductId.ToString();
            cell.Controls.Add(hidden);
            row.Controls.Add(cell);


            // column five:
            cell = new HtmlTableCell();
            cell.Attributes.Add("class", "colItemPrice");
            cell.InnerText = item.ListPrice;
            row.Controls.Add(cell);


            // column six:
            cell = new HtmlTableCell();
            cell.Attributes.Add("class", "colEarlyPrice");
            cell.InnerText = item.SalePrice;
            row.Controls.Add(cell);


            // column seven:
            cell = new HtmlTableCell();
            cell.Attributes.Add("class", "colTotal ieBorderFix");
            cell.InnerText = item.Total;
            row.Controls.Add(cell);

            ++count;
            CurrentCartTableBody.Controls.Add(row);
        }
    }

    public void SetLanguageId(long languageId)
    { _LanguageId = languageId; }

    public void SetName(string cartName)
    { Text_ForTextIn_Link_RenameCart.InnerText = TextBox_RenameCart.Text = cartName; }

    public void SetProductUrl(string productUrl)
    { _productUrl = productUrl; }

    public void SetSavedCarts(List<ISavedCart> savedCarts)
    {
        System.Web.UI.HtmlControls.HtmlTableRow row;
        System.Web.UI.HtmlControls.HtmlTableCell cell;
        //System.Web.UI.HtmlControls.HtmlAnchor anchor;
        System.Web.UI.WebControls.LinkButton linkButton;
        System.Web.UI.HtmlControls.HtmlImage image;
        //System.Web.UI.HtmlControls.HtmlInputText text;
        bool stripe = true;

        SavedCartTableBody.Controls.Clear();
        foreach (ISavedCart savedCart in savedCarts)
        {
            stripe = !stripe;
            row = new HtmlTableRow();
            if (stripe)
                row.Attributes.Add("class", "stripe");

            // column one:
            linkButton = new LinkButton();
            //SelectCartEventArgs args = new SelectCartEventArgs(savedCart.Id);
            //linkButton.Click += delegate(object sender, EventArgs e) { SelectCart_Click(linkButton, args); };
            linkButton.OnClientClick = "document.getElementById('" + ActionCode.ClientID + "').value = 'SelectCart';"
                + "document.getElementById('" + ActionArgument.ClientID + "').value = '" + savedCart.Id + "';";
            linkButton.Attributes.Add("class", "removeCoupon");
            linkButton.Attributes.Add("title", GetResourceString("lbl view cart", "View Cart") + " " + savedCart.Name);
            linkButton.Text = (!String.IsNullOrEmpty(savedCart.Name) ? savedCart.Name : GetResourceString("", "(No Name)"))
                + (IsActiveCart(savedCart.Id) ? " - " + GetResourceString("lbl active cart", "Active Cart") : "");

            cell = new HtmlTableCell();
            cell.Attributes.Add("class", "colCartName");
            cell.Controls.Add(linkButton);
            row.Controls.Add(cell);



            // column two:
            cell = new HtmlTableCell();
            cell.Attributes.Add("class", "colLastModified");
            cell.InnerText = savedCart.LastUpdated;
            row.Controls.Add(cell);


            // column three:
            cell = new HtmlTableCell();
            cell.Attributes.Add("class", "colCartItems");
            cell.InnerText = savedCart.Count.ToString();
            row.Controls.Add(cell);


            // column four:
            cell = new HtmlTableCell();
            cell.Attributes.Add("class", "colcartSubtotal");
            cell.InnerText = savedCart.Subtotal;
            row.Controls.Add(cell);


            // column five:
            if (!IsActiveCart(savedCart.Id))
            {
                image = new HtmlImage();
                image.Alt = GetResourceString("lbl delete cart", "Delete Cart");
                image.Src = ApplicationImagePath + "commerce/deleteCart.gif";

                //anchor = new HtmlAnchor();
                //anchor.HRef = "#";
                //anchor.Attributes.Add("onclick", "alert('delete cart: " + savedCart.Id.ToString() + "'); return false;");
                //anchor.Title = GetResourceString("lbl delete cart", "Delete Cart");
                //anchor.Controls.Add(image);

                linkButton = new LinkButton();
                linkButton.OnClientClick = "document.getElementById('" + ActionCode.ClientID + "').value = 'DeleteCart';"
                    + "document.getElementById('" + ActionArgument.ClientID + "').value = '" + savedCart.Id.ToString() + "';";
                linkButton.ToolTip = GetResourceString("lbl delete cart", "Delete Cart");
                linkButton.Controls.Add(image);

                cell = new HtmlTableCell();
                cell.Attributes.Add("class", "colDeleteSavedCart");
                cell.Controls.Add(linkButton);
                row.Controls.Add(cell);
            }
            else
            {
                cell = new HtmlTableCell();
                cell.Attributes.Add("class", "colDeleteSavedCart");
                row.Controls.Add(cell);
            }

            SavedCartTableBody.Controls.Add(row);
        }

        SavedCartContainer.Visible = true;
    }

    public void SetShoppingUrl(string shoppingUrl)
    { Link_ContinueShopping_1.HRef = Link_ContinueShopping_2.HRef = shoppingUrl; }

    public void SetSubTotal(string subTotal)
    { Text_ForTextIn_Subtotal.InnerText = subTotal; }

    #endregion

    private class CouponEventArgs : EventArgs
    {
        public CouponEventArgs(string couponCode)
        { _couponCode = couponCode; }

        protected string _couponCode;

        public string CouponCode { get { return _couponCode; } }
    }

    private class SelectCartEventArgs : EventArgs
    {
        public SelectCartEventArgs(long selectedCart)
        { _selectedCart = selectedCart; }

        protected long _selectedCart;

        public long SelectedCart { get { return _selectedCart; } }
    }
}
