using System;
using System.Collections.Generic;
using System.Web.UI;
using System.Web.UI.WebControls;
using Ektron.Cms;
using Ektron.Cms.Commerce;
using Ektron.Cms.Common;
using Ektron.Cms.Widget;

public partial class Workarea_Widgets_RecentOrders : WorkareaWidgetBaseControl, IWidget
{
    List<OrderData> _data;
    protected bool bcommerceAdmin = false;
    protected ContentAPI m_refcontentapi = new ContentAPI();

    protected void Page_Load(object sender, EventArgs e)
    {

        bcommerceAdmin = m_refcontentapi.IsARoleMember(EkEnumeration.CmsRoleIds.CommerceAdmin);
        if (!bcommerceAdmin)
        {
            lblNoRecords.Visible = true;
            lblNoRecords.Text = GetMessage("err not role commerce-admin");
            return;
        }
        Criteria<OrderProperty> orderCriteria = new Criteria<OrderProperty>();
        orderCriteria.PagingInfo = new PagingInfo(10);
        _data = new OrderApi().GetList(orderCriteria);
        if (_data.Count > 0)
        {
            LoadData();
        }
        else
        {
            lblNoRecords.Visible = true;
            pnlData.Visible = false;
        }

        string title = GetMessage("lbl recent orders") + " (" + _data.Count.ToString() + ")";
        SetTitle(title);

        ltrlViewAll.Text = GetMessage("lbl view all");
        ltrlNoRecords.Text = GetMessage("lbl no records");
        lnkViewAll.OnClientClick = "top.showContentInWorkarea('" + EkFunctions.UrlEncode("Commerce/fulfillment.aspx?action=mostrecent") + "', 'Reports')";
    }

    private void LoadData()
    {
        grdData.DataSource = _data;
        grdData.DataBind();
    }

    public string FormatCurrency(object price, object cultureCode)
    {
        return EkFunctions.FormatCurrency(Convert.ToDecimal(price), cultureCode.ToString());
    }

    public void HandleItemDataBound(object sender, DataGridItemEventArgs e)
    {
        if (e.Item.ItemType == ListItemType.Header)
        {
            //e.Item.Cells[0].Text = ""; // blank
            //e.Item.Cells[1].Text = GetMessage("generic name");
            //e.Item.Cells[2].Text = GetMessage("lbl orders");
            //e.Item.Cells[3].Text = GetMessage("lbl order value");
            //e.Item.Cells[4].Text = GetMessage("generic datecreated");
        }
        else if (e.Item.ItemType != ListItemType.Footer)
        {
            // ID column:
            string url = ((HyperLink)e.Item.Cells[1].Controls[0]).NavigateUrl + "&page=workarea";
            ((HyperLink)e.Item.Cells[0].Controls[0]).NavigateUrl = "javascript:top.showContentInWorkarea('" + Server.UrlDecode(url) + "', 'Settings', 'commerce\\\\fulfillment')";

            // date column:
            ((HyperLink)e.Item.Cells[1].Controls[0]).NavigateUrl = "javascript:top.showContentInWorkarea('" + Server.UrlDecode(url) + "', 'Settings', 'commerce\\\\fulfillment')";
        }
    }
}
