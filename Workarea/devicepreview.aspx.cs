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
using Ektron.Cms;
using Ektron.Cms.Device;
using System.Collections.Generic;
using Ektron.Cms.Common;
public partial class Workarea_devicepreview : System.Web.UI.Page
{
    protected EkMessageHelper _MessageHelper;
    protected long _ContentID = -1;
    protected long _FolderID = -1;
    protected FolderData _FolderData ;
    protected ContentAPI _ContentApi = new ContentAPI();
    protected List<DeviceTemplateData> deviceList = new List<DeviceTemplateData>();
    protected ContentEditData _lData = new ContentEditData();
    CmsDeviceConfigurationCriteria criteria = new CmsDeviceConfigurationCriteria();
    List<CmsDeviceConfigurationData> dList = new List<CmsDeviceConfigurationData>();
    protected long chkID;

    protected void Page_Init(object sender, System.EventArgs e)
    {
        this.RegisterCSS();
        this.RegisterJS();
    }

    protected void Page_Load(object sender, EventArgs e)
    {
        _MessageHelper = _ContentApi.EkMsgRef;
		 Utilities.ValidateUserLogin();

        if(!String.IsNullOrEmpty(Request.QueryString["cid"]))
        {
            _ContentID = Convert.ToInt64(Request.QueryString["cid"]);
        }

        if (!String.IsNullOrEmpty(Request.QueryString["fid"]))
        {
            _FolderID = Convert.ToInt64(Request.QueryString["fid"]);
            _FolderData = _ContentApi.GetFolderById(_FolderID);
        }

        CmsDeviceConfiguration cDevice = new CmsDeviceConfiguration(_ContentApi.RequestInformationRef);
        criteria.OrderByField = Ektron.Cms.Device.CmsDeviceConfigurationProperty.Order;
        criteria.OrderByDirection = EkEnumeration.OrderByDirection.Ascending;
        dList = cDevice.GetList(criteria);

        if (!Page.IsPostBack)
        {
            if (_FolderData != null)
            {
                if (dList.Count > 0)
                {
                    ddlDevices.Items.Clear();

                    foreach (CmsDeviceConfigurationData dItem in dList)
                    {
                        ddlDevices.Items.Add(new ListItem(dItem.Name, dItem.Id.ToString()));
                    }
                    if (!(ddlDevices.Items.Count > 0))
                        Response.Redirect(ContentLink().Replace("ekfrm", "id"), false);
                }
                else
                {
                    Response.Redirect(ContentLink().Replace("ekfrm", "id"), false);
                }
            }
        }
       
    }

    private string ContentLink()
    {
         string aURL = "";
         _lData = _ContentApi.GetContentForEditing(_ContentID);
         if (_lData != null)
         {
             if (_lData.Quicklink.ToLower().IndexOf("http://") > -1 || _lData.Quicklink.ToLower().IndexOf("https://") > -1 || _lData.Quicklink.StartsWith(_ContentApi.SitePath))
             {
                 aURL = _lData.Quicklink + "&cmsMode=Preview&langtype=" + _lData.LanguageId;
             }
             else
             {
                 aURL = _ContentApi.SitePath + _lData.Quicklink  + "&cmsMode=Preview&langtype=" + _lData.LanguageId;
             }
         }
         return aURL;
    }

    private bool GetDeviceItem(CmsDeviceConfigurationData devItem)
    {
        if (devItem.Id == chkID)
        {
            return true;
        }
        else
        {
            return false;
        }
    }


    protected void btnPreview_Click(object sender, EventArgs e)
    {
        chkID = Convert.ToInt64(ddlDevices.SelectedValue);
        CmsDeviceConfigurationData deviceMatch = dList.Find(GetDeviceItem);
        string cLink = ContentLink();
        if (deviceMatch != null)
        {
            if (deviceMatch.Id != 1)
            {

                string tempTemplate = Ektron.Cms.UrlAliasing.UrlAliasCommonApi.GetUrlForDeviceModel(deviceMatch.Models[0], System.IO.Path.GetFileName(cLink).Split((new char[] { '?' }))[0]);
                if (rbList.Items[0].Selected == true)
                {
                    ClientScript.RegisterStartupScript(this.GetType(), "WindowOpenerScript", "<script language=JavaScript>window.open('', 'Device400').close(); window.open('" + (tempTemplate != string.Empty ? (_ContentApi.SitePath + tempTemplate + "?id=" + _ContentID + "&cmsMode=Preview") : cLink) + "', 'Device400', 'width=" + deviceMatch.Width + " , height=" + deviceMatch.Height + ",scrollbars=1');</script>", false);
                }
                else
                {
                    ClientScript.RegisterStartupScript(this.GetType(), "WindowOpenerScript", "<script language=JavaScript>window.open('', 'Device400').close(); window.open('" + (tempTemplate != string.Empty ? (_ContentApi.SitePath + tempTemplate + "?id=" + _ContentID + "&cmsMode=Preview") : cLink) + "', 'Device400', 'width=" + deviceMatch.Height + " , height=" + deviceMatch.Width + ",scrollbars=1');</script>", false);
                }
            }
            else
            {
                ClientScript.RegisterStartupScript(this.GetType(), "WindowOpenerScript", "<script language=JavaScript> window.open('', 'Device400').close(); window.open('" + cLink + "', 'Device400', '');</script>", false);
            }
        }
    }


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
    }

    #endregion
}
