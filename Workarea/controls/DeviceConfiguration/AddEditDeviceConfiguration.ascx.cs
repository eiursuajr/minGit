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
using Ektron.Cms.Common;
using Ektron.Cms;
using Ektron.Cms.Device;
using System.Text;
using System.Collections.Generic;
using System.Xml;
using System.IO;

public partial class AddEditDeviceConfiguration : System.Web.UI.UserControl
{
    protected EkMessageHelper _MessageHelper;
    protected StyleHelper _StyleHelper;
    protected ContentAPI _ContentApi = new ContentAPI();
    protected string _action = string.Empty;
    protected long _Id = -1;
    protected StringBuilder sBuilder = new StringBuilder();

    protected void Page_Init(object sender, System.EventArgs e)
    {
        this.RegisterCSS();
        this.RegisterJS();
    }

    protected void Page_Load(object sender, EventArgs e)
    {
            _MessageHelper = _ContentApi.EkMsgRef;
            _StyleHelper = new StyleHelper();

            imgCloseAddItemModal.ImageUrl = _ContentApi.ApplicationPath + "images/ui/icons/cancel.png";
            if (Request.QueryString["os"] == "true")
            {
                devicetype.Attributes.Add("onkeypress", "return CheckKeyValue(event, '34,13, 60, 62');");
                selectDevice.Text = "Select a Device OS";
                iframeAddItemsModal.Attributes.Add("src", _ContentApi.ApplicationPath + "devicemodelselection.aspx?os=true");
            }
            else
            {
                selectDevice.Text = _MessageHelper.GetMessage("lbl select a device model");
                iframeAddItemsModal.Attributes.Add("src", _ContentApi.ApplicationPath + "devicemodelselection.aspx?os=false");
            }

            selectTemplate.Text = _MessageHelper.GetMessage("lbl pagebuilder select template");
            tbAddDeviceName.Attributes.Add("onkeypress", "return CheckKeyValue(event, '34,13, 60, 62');");
            tbEditDeviceName.Attributes.Add("onkeypress", "return CheckKeyValue(event, '34,13, 60, 62');");

            FileInfo fInfo = new FileInfo(Server.MapPath("~/App_Data/wurfl.xml.gz"));
            ltrNotification.Text = _MessageHelper.GetMessage("lbl device list update") + fInfo.LastWriteTime + ". " + _MessageHelper.GetMessage("lbl updated file") + " <a style=\"font-style: italic; font-weight: bold; cursor:pointer;\" href=\"http://sourceforge.net/projects/wurfl/files/WURFL/\" target=\"_blank\">" + _MessageHelper.GetMessage("lbl here") + "</a> ";

            setLocalizedString();

            if (!string.IsNullOrEmpty(Request.QueryString["action"]))
            {
                _action = EkFunctions.HtmlEncode(Request.QueryString["action"].ToString());
            }

            if (!string.IsNullOrEmpty(Request.QueryString["id"]))
            {
                _Id = Convert.ToInt64(Request.QueryString["id"]);
            }


            CmsDeviceConfiguration cDevice = new CmsDeviceConfiguration(_ContentApi.RequestInformationRef);
            CmsDeviceConfigurationData deviceData = new CmsDeviceConfigurationData();

            if (!Page.IsPostBack)
            {
                DeviceConfigurationToolBar();
                if (_action == "adddeviceconfiguration")
                {
                    deviceConfigurationView.SetActiveView(Add);
                    if (Request.QueryString["os"] == "true")
                    {
                        trDeviceType1.Visible = true;
						both.Selected = true;
                    }
                    else
                    {
                        trDeviceType1.Visible = false;
                    }
                }
                else
                {
                    if (_Id > -1)
                    {
                        deviceData = cDevice.GetItem(_Id);
                    }

                    deviceConfigurationView.SetActiveView(Edit);
                    if (deviceData != null)
                    {
                        //trDeviceType.Visible = true;
                        tbEditDeviceName.Text = deviceData.Name;
                        //tbEditPreviewTemplate.Text = deviceData.PreviewTemplate;
                        tbEditPreviewWidth.Value   = deviceData.Width.ToString();
                        tbEditPreviewHeight.Value = deviceData.Height.ToString();

                        if (Request.QueryString["os"] == "true")
                        {
                            trDeviceType.Visible = true;
                            
                            switch (deviceData.DeviceType)
                            {
                                case (0):
                                    eboth.Selected = true;
                                    break;
                                case (1):
                                    ehandheld.Selected = true;
                                    break;
                                case (2):
                                    etablet.Selected = true;
                                    break;
                                default:
                                    eboth.Selected = true;
                                    break;
                            }

                            if (deviceData.DeviceType != null)
                            {
                                deviceData.DeviceType = deviceData.DeviceType;
                            }
                        }

                        if (deviceData.Models != null && deviceData.Models.Count > 0)
                        {
                            sBuilder = new StringBuilder();
                            sBuilder.Append("arDeviceNodes = new Array(");
                            int order = 0;
                            foreach (string node in deviceData.Models)
                            {
                                if (order != 0)
                                    sBuilder.Append(",");
                                sBuilder.Append("new node('" + node + "','" + order + "')");
                                order += 1;
                            }
                            sBuilder.AppendLine(");");
                            sBuilder.AppendLine("renderDeviceModels();");
                        }
                        Page.ClientScript.RegisterStartupScript(this.GetType(), "renderDeviceList", sBuilder.ToString(), true);  
                    }
                }
            }
            else
            {
                long _deviceID = 0;
                string backpage = "settings.aspx?action=viewalldeviceconfigurations";
                if (_action == "adddeviceconfiguration")
                {
                    deviceData.Name = Request.Form[tbAddDeviceName.UniqueID];
                    deviceData.Id = 0; //Add

                    deviceData.Width = Convert.ToInt32(Request.Form[tbAddPreviewWidth.UniqueID]);
                    deviceData.Height = Convert.ToInt32(Request.Form[tbAddPreviewHeight.UniqueID]);
                    if (Request.QueryString["os"] == "true")
                    {
                        trDeviceType1.Visible = true;
                        deviceData.DisplayFor = 1;
                        devicetype.Attributes.Add("onkeypress", "return CheckKeyValue(event, '34,13, 60, 62');");
                        if (Request.Form[devicetype.UniqueID] != null)
                        {
                            deviceData.DeviceType = Convert.ToInt16(Request.Form[devicetype.UniqueID]);
                        }
                    }
                    else
                    {
                        trDeviceType1.Visible = false;
                        deviceData.DisplayFor = 0;
                    }
                    deviceData.Models = DeserializeDeviceModels();

                    _deviceID = cDevice.Add(deviceData);
                    if (_deviceID == 0)
                        Utilities.ShowError(_MessageHelper.GetMessage("lbl device configuration exists"));
                    else
                        Response.Redirect(backpage, false);
                }
                else
                {
                    deviceData = cDevice.GetItem(_Id);
                    deviceData.Name = Request.Form[tbEditDeviceName.UniqueID];

                    deviceData.Id = _Id;

                    deviceData.Width = Convert.ToInt32(Request.Form[tbEditPreviewWidth.UniqueID]);
                    deviceData.Height = Convert.ToInt32(Request.Form[tbEditPreviewHeight.UniqueID]);

                    if (Request.QueryString["os"] == "true")
                    {
                        trDeviceType.Visible = true;
                        trDeviceType1.Visible = true;
                        deviceData.DisplayFor = 1;

                        if (Request.Form[edevicetype.UniqueID] != null)
                        {
                            deviceData.DeviceType = Convert.ToInt16(Request.Form[edevicetype.UniqueID]);
                        }

                        else if (deviceData.DeviceType == null)
                        {
                            deviceData.DeviceType = deviceData.DeviceType;
                        }
                   
                    }
                    else
                    {
                        trDeviceType.Visible = false;
                        trDeviceType1.Visible = false;
                        deviceData.DisplayFor = 0;
                    }
                    deviceData.Models = DeserializeDeviceModels();

                    _deviceID = cDevice.Update(deviceData);
                    if (_deviceID == 0)
                        Utilities.ShowError(_MessageHelper.GetMessage("lbl device configuration exists"));
                    else
                        Response.Redirect("settings.aspx?action=viewdeviceconfiguration&id=" + _Id, false);
                }

            }
    }

    #region Private Methods
    private void DeviceConfigurationToolBar()
    {
        if (_action == "adddeviceconfiguration")
        {
            divTitleBar.InnerHtml = _StyleHelper.GetTitleBar(_MessageHelper.GetMessage("lbl Add device configuration"));
            trDeviceType1.Visible = true;
        }
        else
        {
            divTitleBar.InnerHtml = _StyleHelper.GetTitleBar(_MessageHelper.GetMessage("lbl Update device configuration"));
            trDeviceType1.Visible = false;
        }

        string backup = _StyleHelper.getCallBackupPage("settings.aspx?action=viewalldeviceconfigurations");
        StringBuilder  sb = new StringBuilder();
        sb.Append("<table><tbody><tr>");
        sb.Append(_StyleHelper.GetButtonEventsWCaption(_ContentApi.AppImgPath + "../UI/Icons/back.png", backup, _MessageHelper.GetMessage("alt back button text"), _MessageHelper.GetMessage("btn back"), "", StyleHelper.BackButtonCssClass,true));
        sb.Append(_StyleHelper.GetButtonEventsWCaption(_ContentApi.AppImgPath + "../UI/Icons/save.png", "#", _MessageHelper.GetMessage("alt save button text (device configuration)"), _MessageHelper.GetMessage("btn Save device configuration"), "onclick=\";Validate();\"", StyleHelper.SaveButtonCssClass,true));
        sb.Append(StyleHelper.ActionBarDivider);
        sb.Append("<td>");
        if (_action == "adddeviceconfiguration")
        {
            sb.Append(_StyleHelper.GetHelpButton("adddeviceconfiguration", ""));
        }
        else
        {
            sb.Append(_StyleHelper.GetHelpButton("editdeviceconfiguration", ""));
        }
        sb.Append("</td>");
        sb.Append("</tr></tbody></table>");

        divToolBar.InnerHtml = sb.ToString();
    }

    private List<string> DeserializeDeviceModels()
    {
        string xml = HttpUtility.UrlDecode(Request.Form["savedDeviceModelList"]);
        XmlDocument doc = new System.Xml.XmlDocument();
        XmlNodeList nodes = null;
        List<string> deviceList = new List<string>();
        xml = xml.Replace("&", "&amp;");
        if (xml != "")
        {
            doc.LoadXml(xml);
            nodes = doc.SelectNodes("devicemodels/node");
            foreach (XmlNode node in nodes)
            {
                deviceList.Add(HttpUtility.HtmlDecode(node.SelectSingleNode("title").InnerXml));
            }
        }

        return deviceList;
    }


    private void setLocalizedString()
    {
        imgCloseAddItemModal.ToolTip = this._MessageHelper.GetMessage("close title");
        closeDialogLnk.ToolTip=closeDialogLink.ToolTip = _MessageHelper.GetMessage("tooltip:close the device selection screen");
            
        tdDeviceType.Attributes.Add("title", this._MessageHelper.GetMessage("lbl Device Type"));
        tdDeviceType.InnerHtml = this._MessageHelper.GetMessage("lbl Device Type");
        tdDeviceType1.Attributes.Add("title", this._MessageHelper.GetMessage("lbl Device Type"));
        tdDeviceType1.InnerHtml = this._MessageHelper.GetMessage("lbl Device Type");

        if (Request.QueryString["os"] == "true")
        {
            lbleditdisplayfor.Attributes.Add("title", this._MessageHelper.GetMessage("lbl Device OS"));
            lbleditdisplayfor.InnerHtml = this._MessageHelper.GetMessage("lbl Device OS") + ":";
            lbladddisplayfor.Attributes.Add("title", this._MessageHelper.GetMessage("lbl Device OS"));
            lbladddisplayfor.InnerHtml = this._MessageHelper.GetMessage("lbl Device OS") + ":";
            lnkEditDevice.InnerHtml = this._MessageHelper.GetMessage("lbl device config add os");
            lnkAddDevice.InnerHtml = this._MessageHelper.GetMessage("lbl device config add os");

        }
        else
        {
            lbleditdisplayfor.Attributes.Add("title", this._MessageHelper.GetMessage("lbl Device Model"));
            lbleditdisplayfor.InnerHtml = this._MessageHelper.GetMessage("lbl Device Model") + ":";
            lbladddisplayfor.Attributes.Add("title", this._MessageHelper.GetMessage("lbl Device Model"));
            lbladddisplayfor.InnerHtml = this._MessageHelper.GetMessage("lbl Device Model") + ":";
            lnkEditDevice.InnerHtml = this._MessageHelper.GetMessage("lbl device config add model");
            lnkAddDevice.InnerHtml = this._MessageHelper.GetMessage("lbl device config add model");
        }
    }

    #endregion

    #region CSS, JS

    private void RegisterCSS()
    {
        Ektron.Cms.API.Css.RegisterCss(this, Ektron.Cms.API.Css.ManagedStyleSheet.EktronWorkareaCss);
        Ektron.Cms.API.Css.RegisterCss(this, Ektron.Cms.API.Css.ManagedStyleSheet.EktronWorkareaIeCss, Ektron.Cms.API.Css.BrowserTarget.LessThanEqualToIE7);
        Ektron.Cms.API.Css.RegisterCss(this, Ektron.Cms.API.Css.ManagedStyleSheet.EktronModalCss);
        Ektron.Cms.API.Css.RegisterCss(this, Ektron.Cms.API.Css.ManagedStyleSheet.EktronTreeviewCss);
    }

    private void RegisterJS()
    {
        Ektron.Cms.API.JS.RegisterJS(this, Ektron.Cms.API.JS.ManagedScript.EktronJS);
        Ektron.Cms.API.JS.RegisterJS(this, Ektron.Cms.API.JS.ManagedScript.EktronWorkareaJS);
        Ektron.Cms.API.JS.RegisterJS(this, Ektron.Cms.API.JS.ManagedScript.EktronWorkareaHelperJS);
        Ektron.Cms.API.JS.RegisterJS(this, Ektron.Cms.API.JS.ManagedScript.EktronJFunctJS);
        Ektron.Cms.API.JS.RegisterJS(this, Ektron.Cms.API.JS.ManagedScript.EktronModalJS);
        Ektron.Cms.API.JS.RegisterJS(this, Ektron.Cms.API.JS.ManagedScript.EktronTreeviewJS);
    }

    #endregion

}
