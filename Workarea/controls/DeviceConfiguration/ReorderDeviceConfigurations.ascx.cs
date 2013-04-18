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

public partial class ReorderDeviceConfigurations : System.Web.UI.UserControl
{
    protected EkMessageHelper _MessageHelper;
    protected StyleHelper _StyleHelper = new StyleHelper();
    protected ContentAPI _ContentApi = new ContentAPI();
    protected CmsDeviceConfiguration cDevice;

    protected void Page_Init(object sender, System.EventArgs e)
    {
        cDevice = new CmsDeviceConfiguration(_ContentApi.RequestInformationRef);
        this.RegisterCSS();
        this.RegisterJS();
    }

    protected void Page_Load(object sender, EventArgs e)
    {
        _MessageHelper = _ContentApi.EkMsgRef;

        Up.Src = _ContentApi.AppImgPath + "../UI/Icons/arrowHeadUp.png";
        Up.Alt = _MessageHelper.GetMessage("move selection up msg");
        Down.Src = _ContentApi.AppImgPath + "../UI/Icons/arrowHeadDown.png";
        Down.Alt = _MessageHelper.GetMessage("move selection down msg");
        ltrMessage.Text = _MessageHelper.GetMessage("msg:cannot reorder generics");

        if (!Page.IsPostBack)
        {
            ReorderToolBar();
            BindData();
        }
        else
        {
            string deviceOrderList = Request.Form["DeviceOrder"];
            if (!String.IsNullOrEmpty(deviceOrderList))
            {
                string[] cgdItems = deviceOrderList.Split(new char[] { ',' });
                List<CmsDeviceConfigurationData> cDevList = new List<CmsDeviceConfigurationData>();
                CmsDeviceConfigurationData cDevData;
                for (int i = 0; i < cgdItems.Length; i++)
                {
                    if (cgdItems[i].IndexOf("|changed|") > -1)
                    {
                        cDevData = cDevice.GetItem(Convert.ToInt64(cgdItems[i].Split(new char[] { '|' })[0]));
                        cDevData.Order = Convert.ToInt32(cgdItems[i].Split(new char[] { '|' })[1]);
                        cDevList.Add(cDevData);
                    }

                }
                cDevice.Reorder(cDevList);

            }
            Response.Redirect("settings.aspx?action=viewalldeviceconfigurations", false);
        }

    }

    #region private methods

    private void ReorderToolBar()
    {
        StringBuilder sb = new StringBuilder();

        divTitleBar.InnerHtml = _StyleHelper.GetTitleBar(_MessageHelper.GetMessage("lbl reorder device configurations"));

        sb.Append("<table><tbody><tr>");

        sb.Append(_StyleHelper.GetButtonEventsWCaption(_ContentApi.AppImgPath + "../UI/Icons/back.png", "settings.aspx?action=viewalldeviceconfigurations", _MessageHelper.GetMessage("alt back button text"), _MessageHelper.GetMessage("btn back"), "", StyleHelper.BackButtonCssClass,true));
        sb.Append(_StyleHelper.GetButtonEventsWCaption(_ContentApi.AppImgPath + "../UI/Icons/save.png", "#", _MessageHelper.GetMessage("alt: update device order text"), _MessageHelper.GetMessage("btn update device order"), "onclick=\"document.forms[0].submit();\"", StyleHelper.SaveButtonCssClass,true));
        sb.Append(StyleHelper.ActionBarDivider);
        sb.Append("<td>");
        sb.Append(_StyleHelper.GetHelpButton("reorderdeviceconfigurations", ""));
        sb.Append("</td>");
        sb.Append("</tr></tbody></table>");

        divToolBar.InnerHtml = sb.ToString();

    }

    private void BindData()
    {

        CmsDeviceConfigurationCriteria criteria = new CmsDeviceConfigurationCriteria();
        List<CmsDeviceConfigurationData> cDeviceList;

        criteria.AddFilter(Ektron.Cms.Device.CmsDeviceConfigurationProperty.Id, CriteriaFilterOperator.GreaterThan, "1");
        criteria.OrderByField = Ektron.Cms.Device.CmsDeviceConfigurationProperty.Order;
        criteria.OrderByDirection = EkEnumeration.OrderByDirection.Ascending;


        cDeviceList = cDevice.GetList(criteria);

        if (cDeviceList.Count > 0)
        {
            OrderList.Size = (cDeviceList.Count < 20 ? cDeviceList.Count : 20);

            foreach (CmsDeviceConfigurationData cItem in cDeviceList)
            {
                OrderList.Items.Add(new ListItem(cItem.Name, cItem.Id.ToString() + "|" + cItem.Order));
            }
            OrderList.SelectedIndex = 0;
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
