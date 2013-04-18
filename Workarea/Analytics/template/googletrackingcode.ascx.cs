using System;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Windows.Forms;
using System.Collections.Generic;
using Ektron.Cms;
using Ektron.Cms.Common;

public partial class Analytics_Template_GoogleTrackingCode : Ektron.Cms.Analytics.BeaconTemplateControl
{
    protected void Page_Load(object sender, EventArgs e)
    {
        string userAcct = UserAccount;
        if (String.IsNullOrEmpty(userAcct))
        {
            string pagehost = Page.Request["HTTP_HOST"];
            Ektron.Cms.Analytics.IAnalytics dataManager = ObjectFactory.GetAnalytics();
            List<Ektron.Cms.Analytics.BeaconData> beacons = dataManager.GetBeaconData(pagehost, typeof(Ektron.Cms.Analytics.Providers.GoogleAnalyticsProvider));
            foreach (Ektron.Cms.Analytics.BeaconData beacon in beacons)
            {
                userAcct = beacon.UserAccount;
                if (!String.IsNullOrEmpty(userAcct))
                {
                    break; // exit loop
                }
            }
        }
        if (!String.IsNullOrEmpty(userAcct))
        {
            System.Text.StringBuilder sbVariables = new System.Text.StringBuilder();
            // sbVariables.AppendFormat("_gaq.push(['_setCustomVar', 1,'UserId', '{0}', 2]); ", Ektron.Site.SiteData.Current.User.UserId).Append(Environment.NewLine);
            // sbVariables.AppendFormat("_gaq.push(['_setCustomVar', 2,'VisitorId', '{0}', 2]); ", CommonApi.GetVisitorID(this.Page)).Append(Environment.NewLine);
            sbVariables.AppendFormat("_gaq.push(['_setCustomVar', 3,'Member', '{0}', 2]); ", Ektron.Cms.Framework.Context.UserContextService.Current.IsMembershipUser ? "0" : "1").Append(Environment.NewLine);
            // sbVariables.AppendFormat("_gaq.push(['_setCustomVar', 4,'SubscriberId', '{0}', 1]); ", Ektron.Cms.Context.Subscriber.Id.ToString()).Append(Environment.NewLine);
            // sbVariables.AppendFormat("_gaq.push(['_setCustomVar', 5,'CampaignId', '{0}', 1]); ", Ektron.Cms.Context.Campaign.Id.ToString()).Append(Environment.NewLine);
            this.variables.Text = sbVariables.ToString();
            this.GoogleUserAccount.Text = userAcct;
        }
    }
}