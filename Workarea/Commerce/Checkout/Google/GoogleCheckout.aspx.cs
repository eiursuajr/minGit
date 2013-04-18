using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Net;
using System.IO;

using Ektron.Cms;
using Ektron.Cms.Commerce;
using Ektron.Cms.Common;
using Ektron.Cms.Commerce.Providers;
using Ektron.Cms.Commerce.PaymentMethods;

public partial class Commerce_Checkout_Google_GoogleCheckout : System.Web.UI.Page
{


    IGoogle googleManager = null;

    protected void Page_Load(object sender, EventArgs e)
    {

        googleManager = ObjectFactory.GetGoogle();

        googleManager.InitializeFromGateway();

        if (Request.ServerVariables["HTTP_REFERER"] != null && Request.ServerVariables["HTTP_REFERER"] != "")
        {

            googleManager.EditCartUrl = Request.ServerVariables["HTTP_REFERER"];
            googleManager.ContinueShoppingUrl = (Request.QueryString["shoppingURL"] != null ? Request.QueryString["shoppingURL"] : googleManager.EditCartUrl);

        }

        SetGoogleCheckout();

    }

    private void SetGoogleCheckout()
    {

        Basket basket = (new BasketApi()).GetDefaultBasket();

        string googleResponse = googleManager.GetGoogleCheckoutUrl(basket);

        if (googleResponse != "")
            Response.Redirect(googleResponse, false);

    }

}
