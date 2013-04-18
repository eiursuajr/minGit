using System;
using System.Collections.Generic;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.IO;

using Ektron.Cms;
using Ektron.Cms.Common;
using Ektron.Cms.Commerce;
using Ektron.Cms.Commerce.Providers;
using Ektron.Cms.Commerce.PaymentMethods;

using GCheckout.AutoGen;
using GCheckout.MerchantCalculation;
using GCheckout.Util;


public partial class Commerce_Checkout_Google_MerchantCalculations : System.Web.UI.Page
{

    IGoogle googleManager = ObjectFactory.GetGoogle();

    protected void Page_Load(object sender, EventArgs e)
    {

        try
        {

			Stream RequestStream = Request.InputStream;
            StreamReader RequestStreamReader = new StreamReader(RequestStream);
            string RequestXml = RequestStreamReader.ReadToEnd();
            RequestStream.Close();
            
            switch (EncodeHelper.GetTopElement(RequestXml)) 
            {

                case "new-order-notification":
                    
                    googleManager.ProcessGoogleOrder(RequestXml);

                    break;

                case "merchant-calculation-callback":

                    MerchantCalculationCallback merchantCallback = (MerchantCalculationCallback)EncodeHelper.Deserialize(RequestXml, typeof(MerchantCalculationCallback));

                    string responseXML = @"<?xml version=""1.0"" encoding=""UTF-8""?>
                        <merchant-calculation-results xmlns=""http://checkout.google.com/schema/2"">
    <results>
        <result shipping-name=""Silver"" address-id=""";
                                                       responseXML += merchantCallback.calculate.addresses[0].id;
                                                       responseXML +=  @""">
            <shipping-rate currency=""USD"">22.03</shipping-rate>
            <shippable>true</shippable>            
            <total-tax currency=""USD"">14.67</total-tax>
                </merchant-code-results>
        </result>
        <result shipping-name=""Gold"" address-id=""";
                                                       responseXML += merchantCallback.calculate.addresses[0].id;
                                                       responseXML += @""">
            <shipping-rate currency=""USD"">122.03</shipping-rate>
            <shippable>true</shippable>            
            <total-tax currency=""USD"">54.67</total-tax>
                </merchant-code-results>
        </result>
    </results>
</merchant-calculation-results>";

                    Response.ContentType = "text/xml";
                    Response.Write(responseXML);

                    break;

                //case "risk-information-notification":
                    
                //    RiskInformationNotification N2 = (RiskInformationNotification) EncodeHelper.Deserialize(RequestXml, typeof(RiskInformationNotification));
                //    // This notification tells us that Google has authorized the order and it has passed the fraud check.
                //    // Use the data below to determine if you want to accept the order, then start processing it.
                //    string OrderNumber2 = N2.googleordernumber;
                //    string AVS = N2.riskinformation.avsresponse;
                //    string CVN = N2.riskinformation.cvnresponse;
                //    bool SellerProtection = N2.riskinformation.eligibleforprotection;
                //    break;

                //case "order-state-change-notification":
                    
                //    OrderStateChangeNotification N3 = (OrderStateChangeNotification) EncodeHelper.Deserialize(RequestXml, typeof(OrderStateChangeNotification));
                //    // The order has changed either financial or fulfillment state in Google's system.
                //    string OrderNumber3 = N3.googleordernumber;
                //    string NewFinanceState = N3.newfinancialorderstate.ToString();
                //    string NewFulfillmentState = N3.newfulfillmentorderstate.ToString();
                //    string PrevFinanceState = N3.previousfinancialorderstate.ToString();
                //    string PrevFulfillmentState = N3.previousfulfillmentorderstate.ToString();
                //    break;

                //case "charge-amount-notification":
                    
                //    ChargeAmountNotification N4 = (ChargeAmountNotification) EncodeHelper.Deserialize(RequestXml, typeof(ChargeAmountNotification));
                //    // Google has successfully charged the customer's credit card.
                //    string OrderNumber4 = N4.googleordernumber;
                //    decimal ChargedAmount = N4.latestchargeamount.Value;
                //    break;

                //case "refund-amount-notification":
                    
                //    RefundAmountNotification N5 = (RefundAmountNotification) EncodeHelper.Deserialize(RequestXml, typeof(RefundAmountNotification));
                //    // Google has successfully refunded the customer's credit card.
                //    string OrderNumber5 = N5.googleordernumber;
                //    decimal RefundedAmount = N5.latestrefundamount.Value;
                //    break;

                //case "chargeback-amount-notification":
                    
                //      ChargebackAmountNotification N6 = (ChargebackAmountNotification) EncodeHelper.Deserialize(RequestXml, typeof(ChargebackAmountNotification));
                //    // A customer initiated a chargeback with his credit card company to get her money back.
                //    string OrderNumber6 = N6.googleordernumber;
                //    decimal ChargebackAmount = N6.latestchargebackamount.Value;
                //    break;

              default:
                break;

            }


        }
        catch (Exception) 
        { }   

    }
}
