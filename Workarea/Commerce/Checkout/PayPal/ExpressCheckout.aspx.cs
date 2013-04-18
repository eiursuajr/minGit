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

public partial class Commerce_Checkout_PayPal_ExpressCheckout : System.Web.UI.Page
{

    #region private members

    
    ContentAPI cAPI = new ContentAPI();
    IPayPal paypalManager = null;


    #endregion


    protected void Page_Load(object sender, EventArgs e)
    {

        try
        {

            paypalManager = ObjectFactory.GetPayPal();

            paypalManager.InitializeFromGateway();

            if (Request.QueryString["checkout"] != null && Request.QueryString["returnURL"] != null && Request.QueryString["returnURL"] != "")
            {

                paypalManager.ReturnUrl = StripPayPalValues(Request.QueryString["returnURL"], true);
                paypalManager.CancelUrl = StripPayPalValues((Request.QueryString["cancelURL"] != null ? Request.QueryString["cancelURL"] : paypalManager.ReturnUrl), true);

            }
            else if (Request.ServerVariables["HTTP_REFERER"] != null && Request.ServerVariables["HTTP_REFERER"] != "")
            {

                paypalManager.ReturnUrl = StripPayPalValues(Request.ServerVariables["HTTP_REFERER"]);
                paypalManager.CancelUrl = StripPayPalValues(Request.ServerVariables["HTTP_REFERER"]);

            }
            else if (Request.QueryString["returnURL"] != null && Request.QueryString["returnURL"] != "")
            {

                paypalManager.ReturnUrl = StripPayPalValues(Request.QueryString["returnURL"]);
                paypalManager.CancelUrl = StripPayPalValues((Request.QueryString["cancelURL"] != null ? Request.QueryString["cancelURL"] : paypalManager.ReturnUrl));

            }

            paypalManager.IntegrationPoint = (Request.QueryString["checkout"] != null ? IntegrationPoint.Checkout : IntegrationPoint.Cart);

            SetExpressCheckout();

        }
        catch (Exception ex)
        {

            Utilities.ShowError(ex.Message);

        }

    }

    private string StripPayPalValues(string url)
    {

        return StripPayPalValues(url, false);

    }
    private string StripPayPalValues(string url, bool withCheckout)
    {

        string[] urlArray = url.Split('?');
        NameValueCollection urlParams = new NameValueCollection();
        string result = "";

        if (urlArray.Length > 1)
        {

            result = String.Join("?", urlArray, 1, urlArray.Length - 1);
            urlParams = EkFunctions.ConvertToNameValueCollection(result, false);

        }

        url = urlArray[0] + cAPI.geturlstring(new string[] { "token", "payerid", "paypalpaynow" }, new string[] { "", "", (withCheckout ? "true" : "") }, urlParams);

        return url;

    }

    private void SetExpressCheckout()
    {

        Basket basket = (new BasketApi()).GetDefaultBasket();

        PayPalResponse paypalResponse = (paypalManager.IntegrationPoint == IntegrationPoint.Checkout ? paypalManager.SetExpressCheckoutEnd((new BasketApi()).CalculateBasket(basket.Id)) : paypalManager.SetExpressCheckout(basket));
        
        if (paypalResponse.Ack == AcknowledgementStatus.Success)
        {

            Response.Redirect(paypalManager.GetExpressCheckoutUrl(paypalResponse.ResponseFields["token"], basket.Subtotal), false);

        }
        else
            Utilities.ShowError(paypalResponse.Message);

    }
   

}
