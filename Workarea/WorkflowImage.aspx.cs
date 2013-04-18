using System.Web.UI.WebControls;
using System.Configuration;
using System.Collections;
using System.Linq;
using System.Data;
using System.Web.Caching;
using System.Xml.Linq;
using System.Web.UI;
using System.Diagnostics;
using System.Web.Security;
using System;
using System.Text;
using Microsoft.VisualBasic;
using System.Web.UI.HtmlControls;
using System.Web.SessionState;
using System.Text.RegularExpressions;
using System.Web.Profile;
using System.Collections.Generic;
using System.Web.UI.WebControls.WebParts;
using System.Collections.Specialized;
using System.Web;
using Ektron.Cms.Common;
using Ektron.Cms.Commerce;
using Ektron.Cms;

public partial class WorkflowImage : System.Web.UI.Page
{
    protected Guid instanceId = Guid.Empty;
    protected long objectId = 0;
    protected string objectType = string.Empty;
    protected string wfimageUrl = string.Empty;
    protected void Page_Load(object sender, System.EventArgs e)
    {
        try
        {
            Ektron.Cms.CommonApi api = new Ektron.Cms.CommonApi();
			Utilities.ValidateUserLogin();
            if (!(api.UserId > 0) || (api.RequestInformationRef.IsMembershipUser > 0))
            {
                Utilities.ShowError("User authentication denied.");
            }
            if (!String.IsNullOrEmpty(Request.QueryString["type"]))
            {
                objectType = Request.QueryString["type"];
            }
            if (objectType == "preview")
            {
                wfimageUrl = api.AppPath + "/wfactivities.png?type=" + Request.QueryString["id"];
            }
            else
            {
                if (!String.IsNullOrEmpty(Request.QueryString["id"]))
                {
                    objectId = Convert.ToInt64(Request.QueryString["id"]);
                }
                if (objectId > 0)
                {
                    IOrderManager om = ObjectFactory.GetOrderManager();
                    instanceId = om.GetItem(objectId).WorkflowId;
                    wfimageUrl = api.AppPath + "/wfactivities.png?instanceid=" + instanceId.ToString();
                }
            }
        }
        catch (Exception ex)
        {
            Response.Write(ex.ToString());
        }
    }
}