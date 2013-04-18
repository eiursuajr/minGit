using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

using Net.SourceForge.WURFL.Core;
using System.Text;

public partial class Workarea_diagnostics_images_DeviceDiagnostics : System.Web.UI.Page
{
    private IDevice device;
    private string userAgent;
    private StringBuilder sb = new StringBuilder();

    protected void Page_Init(object sender, EventArgs e)
    {
        IWURFLManagerProvider<IWURFLManager> wurflManagerProvider = new WURFLManagerProvider();
        IWURFLManager wurflManager = wurflManagerProvider.WURFLManager;


        userAgent = Request.ServerVariables["HTTP_USER_AGENT"];
        device = wurflManager.GetDeviceForRequest(userAgent);
       
    }


    protected void Page_Load(object sender, EventArgs e)
    {
        Utilities.ValidateUserLogin();


        sb.Append("<b>User Agent :</b><br/>").Append(userAgent).Append(Environment.NewLine);
        sb.Append(String.Format("<b>{0}:{1}</b><br/>","Device UserAgent", device.UserAgent)).Append(Environment.NewLine);
        sb.Append(String.Format("<b>{0}:{1}</b><br/>","Device ID", device.ID)).Append(Environment.NewLine);
        Outputter("model_name");
        Outputter("brand_name");
        Outputter("max_image_height");
        Outputter("max_image_width");
        Outputter("resolution_height");
        Outputter("resolution_width");
        ltrUI.Text = sb.ToString();
    }
    
    private void Outputter(string capability)
    {
        sb.Append(String.Format("<b>{0}</b> - {1}<br/>", capability, device.GetCapability(capability))).Append(Environment.NewLine);
    }
}
