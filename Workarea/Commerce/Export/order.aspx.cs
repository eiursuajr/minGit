using System;
using System.Collections.Generic;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Ektron.Cms;
using Ektron.Cms.Commerce;
using Ektron.Cms.Common;
using Ektron.Cms.Content;

public partial class Commerce_Export_order : System.Web.UI.Page
{
    ContentAPI _contentApi;

    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsCommerceAdmin)
            return;

        long orderId = (Request.QueryString["id"] != null ? Convert.ToInt64(Request.QueryString["id"]) : 0);
        OrderApi orderAPI = new OrderApi();
        OrderData order = orderAPI.GetItem(orderId);
        string type = (Request.QueryString["type"] != null ? Request.QueryString["type"] : "pdf");
        Ektron.Cms.Common.Export.ExportManager exportManager = new Ektron.Cms.Common.Export.ExportManager();

        Response.Clear();

        switch (type)
        {

            case "csv":

                exportManager.SetProvider("CSVExportProvider");
                Response.AddHeader("Content-Disposition", "attachment;filename=order.csv");
                Response.ContentType = "text/csv";

                break;

            case "xls":

                exportManager.SetProvider("XLSExportProvider");
                Response.AddHeader("Content-Disposition", "attachment;filename=order.xls");
                Response.ContentType = "application/ms-excel";

                break;

            default:

                exportManager.SetProvider("PDFExportProvider");
                Response.AddHeader("Content-Disposition", "attachment;filename=order.pdf");
                Response.ContentType = "application/pdf";

                break;

        }

        Response.BinaryWrite(exportManager.ExportToFile(order));
        Response.End();

    }

    public ContentAPI ContentApi
    { get { return ((null == _contentApi) ? _contentApi = new ContentAPI() : _contentApi); } }

    public bool IsLoggedIn
    { get { return ContentApi.IsLoggedIn; } }

    public bool IsAdmin
    { get { return IsLoggedIn && ContentApi.IsAdmin(); } }

    public bool IsCommerceAdmin
    { get { return IsLoggedIn && (IsAdmin || ContentApi.IsARoleMember(EkEnumeration.CmsRoleIds.CommerceAdmin)); } }
}
