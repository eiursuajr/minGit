using System.Web.UI.WebControls;
using System.Configuration;
using System.Collections;
using System.Linq;
using System.Data;
using System.Web.Caching;
using System.Xml.Linq;
using System.Web.UI;
using System.Diagnostics;
using System.Web.Security;
using System;
using System.Text;
using Microsoft.VisualBasic;
using System.Web.UI.HtmlControls;
using System.Web.SessionState;
using System.Text.RegularExpressions;
using System.Web.Profile;
using System.Collections.Generic;
using System.Web.UI.WebControls.WebParts;
using System.Collections.Specialized;
using System.Web;



	public partial class threadeddisc_print_thread : System.Web.UI.Page
	{
		
		
		private Ektron.Cms.ContentAPI m_refContentApi = new Ektron.Cms.ContentAPI();
		
		protected override void OnInit(System.EventArgs e)
		{
			base.OnInit(e);
		}
		
		protected void Page_Load(object sender, System.EventArgs e)
		{
			Page.Header.Title = (string) Forum1.Title;
			
			string css = "" + "img[src=\"[AppPath]/images/UI/Icons/email.png\"]," + "img[src=\"[AppPath]/images/application/feed-icon16x16.gif\"]," + "img[src=\"[AppPath]/images/UI/Icons/print.png\"]," + "img[src=\"[AppPath]/images/application/branch_view.gif\"]," + "img[src=\"[AppPath]/threadeddisc/themes/b_addpost.gif\"]," + "img[src=\"[AppPath]/threadeddisc/themes/b_edit_post.gif\"]," + "img[src=\"[AppPath]/threadeddisc/themes/b_post_reply.gif\"]," + "img[src=\"[AppPath]/threadeddisc/themes/b_delete_post.gif\"]," + "img[src=\"[AppPath]/threadeddisc/themes/p_pm.gif\"]," + "img[src=\"[AppPath]/threadeddisc/themes/b_quote_post.gif\"]" + "{ display:none; } ";
			
			string AppPath = m_refContentApi.RequestInformationRef.ApplicationPath;
			if ((AppPath[AppPath.Length - 1].ToString()) == "/")
			{
				AppPath = AppPath.Substring(0, AppPath.Length - 1);
			}
			
			css = css.Replace("[AppPath]", AppPath);
			
			this.print_style.InnerHtml = css;
			
		}
	}
	

