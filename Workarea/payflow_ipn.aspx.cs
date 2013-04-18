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
using System.IO;
using System.Net;
using System.Text;
using System.Diagnostics;
using Ektron.Cms;
using Ektron.Cms.Common;
using Ektron.Cms.Commerce;
using Ektron.Cms.Commerce.Subscriptions;
using Ektron.Cms.Instrumentation;

public partial class payflow_ipn : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        
        // default verification url for testing locally...
        string verificationUrl = "http://localhost/websrc/workarea/pay.htm";

        EkRequestInformation requestionInformation = ObjectFactory.GetRequestInfoProvider().GetRequestInformation();

        if (requestionInformation.CommerceSettings.TestMode) {
            verificationUrl = "https://www.sandbox.paypal.com/cgi-bin/webscr";
        }
        else {
            verificationUrl = "https://www.paypal.com/cgi-bin/webscr";
        }

        HttpWebRequest verificationRequest = (HttpWebRequest)WebRequest.Create(verificationUrl);

        //Set values for the request back
        verificationRequest.Method = "POST";
        verificationRequest.ContentType = "application/x-www-form-urlencoded";
        byte[] param = Request.BinaryRead(HttpContext.Current.Request.ContentLength);
        string strRequest = Encoding.ASCII.GetString(param);
        strRequest += "&cmd=_notify-validate";
        verificationRequest.ContentLength = strRequest.Length;
        
        //Send the request to PayPal and get the response
        StreamWriter streamOut = new StreamWriter(verificationRequest.GetRequestStream(), System.Text.Encoding.ASCII);
        streamOut.Write(strRequest);
        streamOut.Close();
        StreamReader streamIn = new StreamReader(verificationRequest.GetResponse().GetResponseStream());
        string verificationResponse = streamIn.ReadToEnd();
        streamIn.Close();

        if (verificationResponse != "VERIFIED")
        {

            IPaymentManager paymentManager = ObjectFactory.GetPaymentManager();

            //check the payment_status is Completed
            //check that txn_id has not been previously processed
            //check that receiver_email is your Primary PayPal email
            //check that payment_amount/payment_currency are correct
            //process payment

            //Supplied Keys:
            //txn_type - values: 1.subscr_failed 2.subscr_cancel 3.subscr_payment 4.subscr_signup 5.subscr_eot 6.subscr_modify 
            //subscr_id
            //txn_id
            //payment_status
            //payer_status

            string transactionType = Request.Form["txn_type"];
            string subscriptionId = Request.Form["subscr_id"];
            string transactionId = Request.Form["txn_id"];
            string paymentStatus = Request.Form["payment_status"];

            if (transactionType == string.Empty){
                
            }

            switch (transactionType) {
                case "subscr_payment":

                    if (paymentStatus == "Completed") {
                        paymentManager.RecurringPaymentProcessed(subscriptionId, transactionId, "PayFlow");
                    }
                    else{
                        Log.WriteError("Payflow IPN:  Unknown paymentStatus:" + paymentStatus);
                        paymentManager.RecurringPaymentFailed(subscriptionId, "PayFlow");
                    }
                    break;
                case "subscr_failed":
                    Log.WriteWarning("Payflow IPN:  subscription failed for subscription Id " + subscriptionId);
                    paymentManager.RecurringPaymentFailed(subscriptionId, "PayFlow");
                    break;
                case "subscr_cancel":
                      Log.WriteInfo("Payflow IPN:  subscription cancelled for subscription Id " + subscriptionId);
                    paymentManager.RecurringPaymentFailed(subscriptionId, "PayFlow");
                    break;
                case "subscr_eot":
                    Log.WriteInfo("Payflow IPN:  subscription term ended for subscription Id " + subscriptionId);
                    paymentManager.RecurringPaymentFailed(subscriptionId, "PayFlow");
                    break;
            }


        }
        else if (verificationResponse == "INVALID")
        {
            Log.WriteError("PayPal returns INVALID");
        }
        else
        {
            Log.WriteError("Unknown Response from Paypal IPN: " + verificationResponse);
        }
    }
}
