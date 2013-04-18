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
using Ektron.Workarea;

namespace Ektron.Workarea.Controls
{
    public partial class Paging : System.Web.UI.UserControl
    {
        #region Members

        private int _CurrentPageIndex;
        private int _SelectedPage;
        private int _TotalPages;
        private Ektron.Cms.ContentAPI _ContentApi;
        private Ektron.Cms.Common.EkMessageHelper _MessageHelper;
        private string _ClientFunction = "Ektron.Workarea.Paging.click(this);";

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
        public string ClientFunction
        {
            get 
            { 
                return _ClientFunction; 
            }
            set 
            {
                _ClientFunction = value;
                SetClientFunction();
            }
        }

        #endregion

        public Paging()
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
            txtPageNumber.Text = String.IsNullOrEmpty(txtPageNumber.Text) ? "0" : txtPageNumber.Text;
            hdnSelectedPage.Value = String.IsNullOrEmpty(hdnSelectedPage.Value) ? String.IsNullOrEmpty(Request.Form[hdnSelectedPage.UniqueID]) || Request.Form[hdnSelectedPage.UniqueID] == "NaN" ? String.Empty : Request.Form[hdnSelectedPage.UniqueID] : hdnSelectedPage.Value;

            if (this.IsPostBack)
            {
                SelectedPage = String.IsNullOrEmpty(hdnSelectedPage.Value) ? Int32.Parse(txtPageNumber.Text) : Int32.Parse(hdnSelectedPage.Value);
            }

            //set link images
            ibFirstPage.ImageUrl = _ContentApi.ApplicationPath + "images/ui/icons/arrowHeadFirst.png";
            ibPreviousPage.ImageUrl = _ContentApi.ApplicationPath + "images/ui/icons/arrowHeadLeft.png";
            ibNextPage.ImageUrl = _ContentApi.ApplicationPath + "images/ui/icons/arrowHeadRight.png";
            ibLastPage.ImageUrl = _ContentApi.ApplicationPath + "images/ui/icons/arrowHeadLast.png";

            SetClientFunction();

            //set text
            litPage.Text = _MessageHelper.GetMessage("lbl pagecontrol go to page");   // "Page";
            litOf.Text = _MessageHelper.GetMessage("lbl pagecontrol of");   // "of";
            litTotalPages.Text = TotalPages.ToString();

            // set tooltips
            lbPageGo.Text = _MessageHelper.GetMessage("lbl goto");   // "Go To Page";
            lbPageGo.ToolTip = _MessageHelper.GetMessage("lbl pagecontrol go to page");   // "Go To Page";
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
            Ektron.Cms.API.JS.RegisterJSInclude(this, _ContentApi.ApplicationPath + "controls/paging/ektron.workarea.controls.paging.js", "EktronWorkareaPagingJs");
            Ektron.Cms.API.Css.RegisterCss(this, _ContentApi.ApplicationPath + "csslib/ektron.workarea.controls.paging.css", "EktronWorkareaControlsPagingCss");
        }

        protected void Page_PreRender(object sender, EventArgs e)
        {
            

            int zeroIndexFixer;
            zeroIndexFixer = TotalPages - 1;
            hdnTotalPages.Value = zeroIndexFixer.ToString();

            zeroIndexFixer = CurrentPageIndex + 1;
            txtPageNumber.Text = zeroIndexFixer.ToString();

            hdnCurrentPageIndex.Value = CurrentPageIndex.ToString();
            hdnSelectedPage.Value = SelectedPage.ToString();
            litTotalPages.Text = TotalPages.ToString(); //for display only


            // enable/disable arrows as needed
            ibFirstPage.Enabled = (TotalPages == 0) || (_CurrentPageIndex > 0);
            ibPreviousPage.Enabled = (TotalPages == 0) || (_CurrentPageIndex > 0);
            ibNextPage.Enabled = (TotalPages == 0) || (_CurrentPageIndex < (TotalPages - 1));
            ibLastPage.Enabled = (TotalPages == 0) || (_CurrentPageIndex < (TotalPages - 1));
        }

        protected void SetClientFunction()
        {
            //set client functions
            string clientFunctionWithParameters = ClientFunction;
            ibFirstPage.OnClientClick = clientFunctionWithParameters;
            ibPreviousPage.OnClientClick = clientFunctionWithParameters;
            ibNextPage.OnClientClick = clientFunctionWithParameters;
            ibLastPage.OnClientClick = clientFunctionWithParameters;
            lbPageGo.OnClientClick = clientFunctionWithParameters;
        }
    }
}