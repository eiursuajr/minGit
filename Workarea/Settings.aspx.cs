using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Ektron.Cms;
using Ektron.Cms.API;

public partial class Workarea_Settings : System.Web.UI.Page
{
    protected ContentAPI contentAPI;
    protected StyleHelper _StyleHelper = new StyleHelper();
    protected void Page_Load(object sender, EventArgs e)
    {
        contentAPI = new ContentAPI();
        string action = string.Empty;
        Utilities.ValidateUserLogin();
        RegisterResources();

        try
        {
            if (!contentAPI.IsAdmin())
            {
                Response.Redirect(contentAPI.ApplicationPath + "reterror.aspx?info=" + contentAPI.EkMsgRef.GetMessage("msg login cms administrator"), true);
                return;
            }
            if (!string.IsNullOrEmpty(Request.QueryString["action"]))
            {
                action = Request.QueryString["action"].ToString();
            }

            switch (action.ToLower())
            {
                case "viewalldeviceconfigurations":
                    ViewAllDeviceConfigurations m_vd;
                    m_vd = (ViewAllDeviceConfigurations)(LoadControl("controls/DeviceConfiguration/ViewAllDeviceConfigurations.ascx"));
                    m_vd.ID = "deviceConfiguration";
                    DataHolder.Controls.Add(m_vd);
                    break;
                case "adddeviceconfiguration":
                case "editdeviceconfiguration":
                    AddEditDeviceConfiguration m_ad;
                    m_ad = (AddEditDeviceConfiguration)(LoadControl("controls/DeviceConfiguration/AddEditDeviceConfiguration.ascx"));
                    m_ad.ID = "deviceConfiguration";
                    DataHolder.Controls.Add(m_ad);
                    break;
                case "viewdeviceconfiguration":
                    ViewDeviceConfiguration m_vsd;
                    m_vsd = (ViewDeviceConfiguration)(LoadControl("controls/DeviceConfiguration/ViewDeviceConfiguration.ascx"));
                    m_vsd.ID = "deviceConfiguration";
                    DataHolder.Controls.Add(m_vsd);
                    break;
                case "reorderdeviceconfigurations":
                    ReorderDeviceConfigurations m_rd;
                    m_rd = (ReorderDeviceConfigurations)(LoadControl("controls/DeviceConfiguration/ReorderDeviceConfigurations.ascx"));
                    m_rd.ID = "deviceConfiguration";
                    DataHolder.Controls.Add(m_rd);
                    break;
                case "deletedeviceconfiguration":
                    DeleteDeviceConfiguration m_dd;
                    m_dd = (DeleteDeviceConfiguration)(LoadControl("controls/DeviceConfiguration/DeleteDeviceConfiguration.ascx"));
                    m_dd.ID = "deviceConfiguration";
                    DataHolder.Controls.Add(m_dd);
                    break;
            }
        }
        catch (Exception ex)
        {
            Utilities.ShowError(ex.InnerException.ToString());
        }
    }

    private void RegisterResources()
    {
        Css.RegisterCss(this, Css.ManagedStyleSheet.EktronWorkareaCss);
        Css.RegisterCss(this, Css.ManagedStyleSheet.EktronWorkareaIeCss);
        Css.RegisterCss(this, Css.ManagedStyleSheet.EktronFixedPositionToolbarCss);

        ltrStyleSheetJS.Text = _StyleHelper.GetClientScript();
    }


}