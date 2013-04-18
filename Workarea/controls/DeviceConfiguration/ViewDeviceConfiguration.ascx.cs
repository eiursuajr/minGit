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
using System.Text;
using Ektron.Cms.Common;
using Ektron.Cms;
using Ektron.Cms.Device;
using System.Collections.Generic;

public partial class ViewDeviceConfiguration : System.Web.UI.UserControl
{
    protected EkMessageHelper _MessageHelper;
    protected StyleHelper _StyleHelper;
    protected ContentAPI _ContentApi = new ContentAPI();
    protected long _Id;
    protected void Page_Init(object sender, System.EventArgs e)
    {
        this.RegisterCSS();
        this.RegisterJS();
    }

    protected void Page_Load(object sender, EventArgs e)
    {
        _MessageHelper = _ContentApi.EkMsgRef;
        _StyleHelper = new StyleHelper();



        if (!String.IsNullOrEmpty(Request.QueryString["id"]))
        {
            _Id = Convert.ToInt64(Request.QueryString["id"]);
        }
        ViewDeviceToolBar();
        BindData();

    }

    #region Private Methods

    private void ViewDeviceToolBar()
    {
        StringBuilder sb = new StringBuilder();

        divTitleBar.InnerHtml = _StyleHelper.GetTitleBar(_MessageHelper.GetMessage("view all device configurations"));

        sb.Append("<table><tbody><tr>");
		sb.Append(_StyleHelper.GetButtonEventsWCaption(_ContentApi.AppImgPath + "../UI/Icons/back.png", "settings.aspx?action=viewalldeviceconfigurations", _MessageHelper.GetMessage("alt back button text"), _MessageHelper.GetMessage("btn back"), "", StyleHelper.BackButtonCssClass, true));
        if (_Id > 1)
        {
            if (Request.QueryString["os"] == "true")
            {
                sb.Append(_StyleHelper.GetButtonEventsWCaption(_ContentApi.AppImgPath + "../UI/Icons/contentEdit.png", "settings.aspx?action=editdeviceconfiguration&os=true&id=" + _Id.ToString(), _MessageHelper.GetMessage("alt edit button text (device configuration)"), _MessageHelper.GetMessage("btn edit device configuration"), "", StyleHelper.EditButtonCssClass, true));
            }
            else
            {
                sb.Append(_StyleHelper.GetButtonEventsWCaption(_ContentApi.AppImgPath + "../UI/Icons/contentEdit.png", "settings.aspx?action=editdeviceconfiguration&os=false&id=" + _Id.ToString(), _MessageHelper.GetMessage("alt edit button text (device configuration)"), _MessageHelper.GetMessage("btn edit device configuration"), "", StyleHelper.EditButtonCssClass, true));
            }
            sb.Append(_StyleHelper.GetButtonEventsWCaption(_ContentApi.AppImgPath + "../UI/Icons/delete.png", "settings.aspx?action=deletedeviceconfiguration&id=" + _Id.ToString(), _MessageHelper.GetMessage("alt delete button text (device configuration)"), _MessageHelper.GetMessage("btn delete device configuration"), "OnClick=\"javascript: return ConfirmConfigurationDelete();\"", StyleHelper.DeleteButtonCssClass));
        }
		sb.Append(StyleHelper.ActionBarDivider);
        sb.Append("<td>");
        sb.Append(_StyleHelper.GetHelpButton("viewalldeviceconfigurations", ""));
        sb.Append("</td>");
        sb.Append("</tr></tbody></table>");

        divToolBar.InnerHtml = sb.ToString();
    }

    private void BindData()
    {
        CmsDeviceConfiguration cDevice = new CmsDeviceConfiguration(_ContentApi.RequestInformationRef);
        CmsDeviceConfigurationData deviceData = new CmsDeviceConfigurationData();

        deviceData = cDevice.GetItem(_Id);

        if (Request.QueryString["os"] != null)
        {
            if (deviceData != null)
            {
                lblDeviceName.Text = EkFunctions.HtmlEncode(deviceData.Name);
                lblPreviewWidth.Text = deviceData.Width.ToString();
                lblPreviewHeight.Text = deviceData.Height.ToString();

                if (deviceData.Models.Count > 0)
                {
                    if (Request.QueryString["os"] == "true")
                    {
                        trDeviceType.Visible = true;
                        lblDeviceType.Attributes.Add("title", _MessageHelper.GetMessage("lbl Device Type"));
                        lblDeviceType.InnerHtml = _MessageHelper.GetMessage("lbl Device Type");

                        ltrModels.Text = "<td class='label'>" + _MessageHelper.GetMessage("lbl operating systems") + ":</td><td>";
                        switch (deviceData.DeviceType)
                        {
                            case (0): ltrDeviceType.Text = _MessageHelper.GetMessage("lbl devicetype both");
                                break;
                            case (1): ltrDeviceType.Text = _MessageHelper.GetMessage("lbl devicetype handheld");
                                break;
                            case (2): ltrDeviceType.Text = _MessageHelper.GetMessage("lbl devicetype tablet");
                                break;
                            default:
                                ltrDeviceType.Text = _MessageHelper.GetMessage("lbl devicetype both");
                                break;
                        }
                    }
                    else
                    {
                        ltrModels.Text = "<td class='label'>" + _MessageHelper.GetMessage("lbl Device Models") + "</td><td>";
                    }
                    foreach (string mItem in deviceData.Models)
                    {
                        ltrModels.Text += "" + mItem + " ,";
                    }
                    ltrModels.Text = ltrModels.Text.ToString().TrimEnd(new char[] { ',' });
                    ltrModels.Text += "</td>";

                }
                else
                {
                    dvModels.Visible = false;
                }
            }
        }
        else
        {
            if (deviceData.DisplayFor == 0)
            {
                Response.Redirect("settings.aspx?action=viewdeviceconfiguration&os=false&id=" + deviceData.Id);
            }
            else
            {
                Response.Redirect("settings.aspx?action=viewdeviceconfiguration&os=true&id=" + deviceData.Id);
            }
        }
    }
    #endregion

    #region CSS, JS

    private void RegisterCSS()
    {
        Ektron.Cms.API.Css.RegisterCss(this, Ektron.Cms.API.Css.ManagedStyleSheet.EktronWorkareaCss);
        Ektron.Cms.API.Css.RegisterCss(this, Ektron.Cms.API.Css.ManagedStyleSheet.EktronWorkareaIeCss, Ektron.Cms.API.Css.BrowserTarget.LessThanEqualToIE7);
    }

    private void RegisterJS()
    {
        Ektron.Cms.API.JS.RegisterJS(this, Ektron.Cms.API.JS.ManagedScript.EktronJS);
        Ektron.Cms.API.JS.RegisterJS(this, Ektron.Cms.API.JS.ManagedScript.EktronWorkareaJS);
        Ektron.Cms.API.JS.RegisterJS(this, Ektron.Cms.API.JS.ManagedScript.EktronWorkareaHelperJS);
        Ektron.Cms.API.JS.RegisterJS(this, Ektron.Cms.API.JS.ManagedScript.EktronJFunctJS);
    }

    #endregion

}
