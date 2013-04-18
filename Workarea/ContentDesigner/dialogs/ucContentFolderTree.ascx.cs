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
using Ektron.Cms.API;

namespace Ektron.ContentDesigner.Dialogs
{
	public partial class ContentFolderTree : System.Web.UI.UserControl
	{
		public string Filter = "";
		protected SiteAPI m_refSiteApi = new SiteAPI();
		protected Ektron.Cms.Common.EkMessageHelper m_refMsg;

		protected void Page_Load(object sender, EventArgs e)
		{

			// Register JS
			JS.RegisterJSInclude(this, JS.ManagedScript.EktronJS);
			JS.RegisterJSInclude(this, JS.ManagedScript.EktronTreeviewJS);

			// Register CSS
			Css.RegisterCss(this, Css.ManagedStyleSheet.EktronTreeviewCss);

		}
	}
}
