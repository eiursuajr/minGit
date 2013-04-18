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

namespace Ektron.Controls
{
    public partial class ClientPaging : System.Web.UI.UserControl
    {
        #region Members

        private int _CurrentPageIndex;
        private int _SelectedPage;
        private int _TotalPages;
        private Ektron.Cms.ContentAPI _ContentApi;
        private Ektron.Cms.Common.EkMessageHelper _MessageHelper;

        #endregion

        #region Properties

        public int CurrentPageIndex
        {
            get
            {
                return _CurrentPageIndex;
            }
            set
            {
                _CurrentPageIndex = value;
            }
        }
        public int SelectedPage
        {
            get
            {
                return _SelectedPage;
            }
            set
            {
                _SelectedPage = value;
            }
        }
        public int TotalPages
        {
            get
            {
                return _TotalPages;
            }
            set
            {
                _TotalPages = value;
            }
        }

        #endregion

        public ClientPaging()
        {
            CurrentPageIndex = 0;
            SelectedPage = 0;
            TotalPages = 0;
            _ContentApi = new Ektron.Cms.ContentAPI();
            _MessageHelper = _ContentApi.EkMsgRef;
        }

        protected void Page_Init(object sender, EventArgs e)
        {
            //get selected page
            //txtPageNumber.Text = String.IsNullOrEmpty(txtPageNumber.Text) ? "0" : txtPageNumber.Text;
            //hdnSelectedPage.Value = String.IsNullOrEmpty(hdnSelectedPage.Value) ? String.IsNullOrEmpty(Request.Form[hdnSelectedPage.UniqueID]) || Request.Form[hdnSelectedPage.UniqueID] == "NaN" ? String.Empty : Request.Form[hdnSelectedPage.UniqueID] : hdnSelectedPage.Value;

            //set link images
            ibFirstPage.ImageUrl = _ContentApi.ApplicationPath + "images/ui/icons/arrowHeadFirst.png";
            ibPreviousPage.ImageUrl = _ContentApi.ApplicationPath + "images/ui/icons/arrowHeadLeft.png";
            ibNextPage.ImageUrl = _ContentApi.ApplicationPath + "images/ui/icons/arrowHeadRight.png";
            ibLastPage.ImageUrl = _ContentApi.ApplicationPath + "images/ui/icons/arrowHeadLast.png";
            ibPageGo.ImageUrl = _ContentApi.ApplicationPath + "images/ui/icons/forward.png";

            //set text
            litPage.Text = _MessageHelper.GetMessage("lbl pagecontrol page");   // "Page";
            litOf.Text = _MessageHelper.GetMessage("lbl pagecontrol of");   // "of";
            litTotalPages.Text = TotalPages.ToString();

            // set tooltips
            ibPageGo.AlternateText = _MessageHelper.GetMessage("lbl pagecontrol go to page");   // "Go To Page";
            ibPageGo.ToolTip = _MessageHelper.GetMessage("lbl pagecontrol go to page");   // "Go To Page";
            ibFirstPage.AlternateText = _MessageHelper.GetMessage("lbl pagecontrol first page");   // "First Page";
            ibFirstPage.ToolTip = _MessageHelper.GetMessage("lbl pagecontrol first page");   // "First Page";
            ibPreviousPage.AlternateText = _MessageHelper.GetMessage("lbl pagecontrol previous page");   // "Previous Page";
            ibPreviousPage.ToolTip = _MessageHelper.GetMessage("lbl pagecontrol previous page");   // "Previous Page";
            ibNextPage.AlternateText = _MessageHelper.GetMessage("lbl pagecontrol next page");   // "Next Page";
            ibNextPage.ToolTip = _MessageHelper.GetMessage("lbl pagecontrol next page");   // "Next Page";
            ibLastPage.AlternateText = _MessageHelper.GetMessage("lbl pagecontrol last page");   // "Last Page";
            ibLastPage.ToolTip = _MessageHelper.GetMessage("lbl pagecontrol last page");   // "Last Page";

            //register page components
			Ektron.Cms.API.JS.RegisterJSInclude(this, Ektron.Cms.API.JS.ManagedScript.EktronJS);
            Ektron.Cms.API.JS.RegisterJSInclude(this, Ektron.Cms.API.JS.ManagedScript.EktronJsonJS);
            Ektron.Cms.API.JS.RegisterJSInclude(this, _ContentApi.ApplicationPath + "controls/paging/clientpaging/ektron.controls.clientpaging.js", "EktronWorkareaClientPagingJs");
            Ektron.Cms.API.Css.RegisterCss(this, _ContentApi.ApplicationPath + "csslib/ektron.workarea.controls.paging.css", "EktronWorkareaControlsPagingCss");
        }

        protected void Page_PreRender(object sender, EventArgs e)
        {
            
            //txtPageNumber.Text = CurrentPageIndex.ToString();
            litTotalPages.Text = TotalPages.ToString(); //for display only

            StringBuilder jsInit = new StringBuilder();
            jsInit.AppendFormat("Ektron.Controls.ClientPaging.totalPages = {0};\r\n", TotalPages);
            jsInit.AppendFormat("Ektron.Controls.ClientPaging.selectedPage = {0};\r\n", SelectedPage);
            jsInit.AppendFormat("Ektron.Controls.ClientPaging.currentPageIndex = {0};\r\n", CurrentPageIndex);
            jsInit.AppendLine("Ektron.Controls.ClientPaging.setupNavigation();");

            Ektron.Cms.API.JS.RegisterJSBlock(this, jsInit.ToString(), "ektron.controls.ClientPaging.init");
        }

    }
}