using System;
using System.Data;
using System.Configuration;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using System.Collections.Generic;

namespace Ektron.Cms.Commerce.Workarea.CatalogEntry.Tabs.Items.Subscriptions
{
    /// <summary>
    /// Required for any control (ascx) that implements Workarea Items tab functionality for Subscriptions
    /// Ektron Commerce Subscription Type
    /// </summary>
    public interface IEktronSubscriptionControl
    {
        Object SubscriptionData { get;set; }
        bool IsEditable { get;set; }
    }
}