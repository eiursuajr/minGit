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

//namespace Ektron.Cms.Commerce.Workarea.CatalogEntry
//{
    public partial class Taxonomy_Tree_A_aspx : System.Web.UI.Page
    {
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
            litFolderId.Text = Request.QueryString["folderId"];
            litTaxonomyOverrideId.Text = Request.QueryString["taxonomyOverrideId"];
            litTaxonomyTreeIdList.Text = Server.UrlDecode(Request.QueryString["taxonomyTreeIdList"]);
            litTaxonomyTreeParentIdList.Text = Server.UrlDecode(Request.QueryString["taxonomyTreeParentIdList"]);
        }

        #endregion
    }
//}
