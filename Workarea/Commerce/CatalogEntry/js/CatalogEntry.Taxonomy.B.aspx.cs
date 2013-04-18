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


namespace Ektron.Cms.Commerce.Workarea.CatalogEntry
{
    public partial class CatalogEntry_Taxonomy_B_Js : System.Web.UI.Page
    {
        #region Member Variables

        private ContentAPI _ContentApi;
        private string _ApplicationPath;

        #endregion

        #region Properties

            public ContentAPI ContentApi
            {
                get
                {
                    return _ContentApi;
                }
                set
                {
                    _ContentApi = value;
                }
            }

            protected String ApplicationPath
            {
                get
                {
                    return _ApplicationPath;
                }
                set
                {
                    _ApplicationPath = value;
                }
            }

            #endregion

        #region Constructor

        protected CatalogEntry_Taxonomy_B_Js()
            {
                _ContentApi = new ContentAPI();
                _ApplicationPath = _ContentApi.ApplicationPath.TrimEnd(new char[] { '/' });
            }

            #endregion

        #region Init

        protected override void OnInit(EventArgs e)
        {
            this.Context.Response.Charset = "utf-8";
            this.Context.Response.ContentType = "application/javascript";

            //set js server variables
            this.SetJsServerVariables();

            base.OnInit(e);
        }

        #endregion

        #region Methods - set js server variables

        private void SetJsServerVariables()
        {
            string showTaxonomy = Request.QueryString["showTaxonomy"] ?? "false";
            if (showTaxonomy.ToLower() == "true")
            {
                litTaxonomyFolderId.Text = Request.QueryString["taxonomyFolderId"];
                mvTaxonomyJs.SetActiveView(vwShow);
            }
            else
            {
                mvTaxonomyJs.SetActiveView(vwHide);
            }

        }

        public string GetApplicationRoot()
        {
            return this.ApplicationPath + "/";
        }

        public string GetMenu()
        {

            if (Request.QueryString["suppress_menu"] != null && Convert.ToBoolean(Request.QueryString["suppress_menu"]) == true)
                return false.ToString().ToLower();
            else
                return true.ToString().ToLower();
        }

        #endregion
    }
}
