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
using Ektron.Cms.Device;
using Ektron.Cms;
using System.Text;
using Ektron.Cms.Common;
using System.Collections.Generic;

public partial class DeleteDeviceConfiguration : System.Web.UI.UserControl
{
    protected ContentAPI _ContentApi = new ContentAPI();
    protected EkMessageHelper _MessageHelper;
    protected StyleHelper _StyleHelper;

    protected void Page_Init(object sender, System.EventArgs e)
    {
        this.RegisterCSS();
        this.RegisterJS();
    }

    protected void Page_Load(object sender, EventArgs e)
    {
        _MessageHelper = _ContentApi.EkMsgRef;
        _StyleHelper = new StyleHelper();

        ltrMsgSelItem.Text = _MessageHelper.GetMessage("js:alert please select a config to delete");
        ltrMsgDelConfig.Text = _MessageHelper.GetMessage("js:alert you sure you wish to delete this configuration Continue?");

        CmsDeviceConfiguration cDevice = new CmsDeviceConfiguration(_ContentApi.RequestInformationRef);

        if (!Page.IsPostBack)
        {
            long _id = -1;
            if (!String.IsNullOrEmpty(Request.QueryString["id"]))
            {
                _id = Convert.ToInt64(Request.QueryString["id"]);
            }
            if (_id > -1)
            {
                cDevice.Delete(_id);
                Response.Redirect("settings.aspx?action=viewalldeviceconfigurations", false);
            }

            DeleteDevicesToolBar();
            BindData();
        }
        else
        {

            string _deviceIds = Request.Form["deleteConfigurationId"];
            string[] _deviceValues;
         
            if (!String.IsNullOrEmpty(_deviceIds))
            {
                _deviceValues = _deviceIds.Split(new char[] { ',' });
                for (int i = 0; i < _deviceValues.Length; i++)
                {
                    cDevice.Delete(Convert.ToInt64(_deviceValues[i]));
                }
             
                Response.Redirect("settings.aspx?action=viewalldeviceconfigurations", false);
            }

        }
    }

    private void DeleteDevicesToolBar()
    {
        StringBuilder sb = new StringBuilder();

        divTitleBar.InnerHtml = _StyleHelper.GetTitleBar(_MessageHelper.GetMessage("lbl Delete device configurations"));

        sb.Append("<table><tbody><tr>");
        sb.Append(_StyleHelper.GetButtonEventsWCaption(_ContentApi.AppImgPath + "../UI/Icons/back.png", "#", _MessageHelper.GetMessage("alt back button text"), _MessageHelper.GetMessage("btn back"), "onclick=\"javascript:history.back(1);return false;\"", StyleHelper.BackButtonCssClass,true));
        sb.Append(_StyleHelper.GetButtonEventsWCaption(_ContentApi.AppImgPath + "../UI/Icons/delete.png", "#", _MessageHelper.GetMessage("alt delete button text (device configurations)"), _MessageHelper.GetMessage("lbl Delete device configurations"), "onclick=\"return ConfirmConfigurationDelete();\"", StyleHelper.DeleteButtonCssClass,true));
        sb.Append(StyleHelper.ActionBarDivider);
        sb.Append("<td>");
        sb.Append(_StyleHelper.GetHelpButton("deletedeviceconfigurations", ""));
        sb.Append("</td>");
        sb.Append("</tr></tbody></table>");

        divToolBar.InnerHtml = sb.ToString();
    }

    private void BindData()
    {
        CmsDeviceConfiguration cDevice = new CmsDeviceConfiguration(_ContentApi.RequestInformationRef);
        CmsDeviceConfigurationCriteria criteria = new CmsDeviceConfigurationCriteria();
        List<CmsDeviceConfigurationData> cDeviceList;
        StringBuilder sBuilder = new StringBuilder();

        criteria.AddFilter(Ektron.Cms.Device.CmsDeviceConfigurationProperty.Id, CriteriaFilterOperator.GreaterThan, 1);
        criteria.OrderByField = Ektron.Cms.Device.CmsDeviceConfigurationProperty.Order;
        criteria.OrderByDirection = EkEnumeration.OrderByDirection.Ascending;
        cDeviceList = cDevice.GetList(criteria);


        System.Web.UI.WebControls.BoundColumn colBound = new System.Web.UI.WebControls.BoundColumn();
        colBound.DataField = "Delete";
        colBound.ItemStyle.Width = Unit.Percentage(3);
        colBound.HeaderStyle.CssClass = "left";
        colBound.ItemStyle.CssClass = "left";
        colBound.HeaderText = _MessageHelper.GetMessage("lbl Delete");
        DeviceListGrid.Columns.Add(colBound);

        colBound = new System.Web.UI.WebControls.BoundColumn();
        colBound.DataField = "Device";
        colBound.HeaderStyle.CssClass = "left";
        colBound.ItemStyle.CssClass = "left";
        colBound.HeaderText = _MessageHelper.GetMessage("lbl Device");
        DeviceListGrid.Columns.Add(colBound);

        colBound = new System.Web.UI.WebControls.BoundColumn();
        colBound.DataField = "Models";
        colBound.HeaderStyle.CssClass = "center";
        colBound.ItemStyle.CssClass = "center";
        colBound.HeaderText = _MessageHelper.GetMessage("lbl Device Models");
        DeviceListGrid.Columns.Add(colBound);

        DataTable dt = new DataTable();
        DataRow dr;

        dt.Columns.Add(new DataColumn("Delete", typeof(string)));
        dt.Columns.Add(new DataColumn("Device", typeof(string)));
        dt.Columns.Add(new DataColumn("Models", typeof(string)));

       
        for (int i = 0; i <= cDeviceList.Count - 1; i++)
        {
            sBuilder = new StringBuilder();
            dr = dt.NewRow();
            dr[0] = "<input type=\"checkbox\" name=\"deleteConfigurationId\" value=\"" + cDeviceList[i].Id + "\">";
            dr[1] = "<a href=\'settings.aspx?action=viewdeviceconfiguration&id=" + cDeviceList[i].Id + "\' title=\'" + EkFunctions.HtmlEncode(cDeviceList[i].Name) + "\'>" + EkFunctions.HtmlEncode(cDeviceList[i].Name) + "</a>";
          
            foreach (string cModel in cDeviceList[i].Models)
            {
                sBuilder.Append(cModel).Append(",");
            }
            dr[2] = sBuilder.ToString().TrimEnd(new char[] { ',' });

            dt.Rows.Add(dr);
        }
        DataView dv = new DataView(dt);
        DeviceListGrid.DataSource = dv;
        DeviceListGrid.DataBind();
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
        Ektron.Cms.API.JS.RegisterJS(this, Ektron.Cms.API.JS.ManagedScript.EktronJFunctJS);
    }

    #endregion
}
