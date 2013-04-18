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
using System.Collections.Generic;

using Ektron.Cms;
using Ektron.Cms.API;

namespace Ektron.ContentDesigner.Dialogs
{
	public partial class CollectionTree : System.Web.UI.UserControl
	{
		protected void Page_Load(object sender, EventArgs e)
		{
			ContentAPI capi = new ContentAPI();
            if (!IsPostBack)
            {
                PageRequestData colrequest = new PageRequestData();
                colrequest.PageSize = 1000;
                colrequest.CurrentPage = 1;
                CollectionListData[] collection_list = capi.EkContentRef.GetCollectionList("", ref colrequest);
                collections.DataSource = collection_list;
                collections.DataBind();
            }


			// Register JS
			JS.RegisterJS(this, JS.ManagedScript.EktronJS);
			JS.RegisterJS(this, JS.ManagedScript.EktronTreeviewJS);

			// Register CSS
			Css.RegisterCss(this, Css.ManagedStyleSheet.EktronTreeviewCss);
		}
	}
}
