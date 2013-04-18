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
using Ektron.Cms;

	public partial class viewvirtualstaging : System.Web.UI.UserControl
	{
		protected StyleHelper m_refStyle = new StyleHelper();
		protected Ektron.Cms.Common.EkMessageHelper m_refMsg;
		protected string AppImgPath = "";
		protected string AppName = "";
		protected string SITEPATH = "";
		protected string VerifyTrue = "";
		protected string VerifyFalse = "";
		
		private void Page_Load(System.Object sender, System.EventArgs e)
		{
			SiteAPI m_refSiteApi = new SiteAPI();
			UserAPI m_refUserApi = new UserAPI();
			m_refMsg = m_refSiteApi.EkMsgRef;
			AppImgPath = m_refSiteApi.AppImgPath;
			AppName = m_refSiteApi.AppName;
			SITEPATH = m_refSiteApi.SitePath;
			
			//call api and display values
			
			td_asset_loc.InnerHtml = "assets";
			td_private_asset_loc.InnerHtml = "private";
			td_domain_username.InnerHtml = "user1";
		}
	}

