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
using Ektron.Cms.Common;
using Ektron.Cms.Content;
using Ektron.Cms.API;



	public partial class SEO_seo_w3c : System.Web.UI.UserControl
	{
		
		
		
		
		protected void Page_Load(object sender, System.EventArgs e)
		{
			Ektron.Cms.ContentAPI contentAPI = new Ektron.Cms.ContentAPI();
			
			if ((contentAPI.UserId > 0) && (contentAPI.IsLoggedIn))
			{
				if (! System.Convert.ToBoolean(contentAPI.RequestInformationRef.IsMembershipUser))
				{
					string searchLink = EkFunctions.UrlEncode(Request.Url.AbsoluteUri);
					seolink.Text = "<a href=\"#\" onclick=\"window.open(\'" + contentAPI.AppPath + "seo/seo.aspx?url=" + EncodeJavascriptString(searchLink) + "\', \'SEOManager\',\'toolbar=0,location=0,directories=0,status=1,menubar=0,scrollbars=1,resizable=1,width=1000,height=800\');return false;\"><img src=\'" + contentAPI.AppPath + "seo/seo-button.gif\' border=\'0\' alt=\'\' /> </a>";
				}
				
				
			}
			
		}
		
		private string EncodeJavascriptString(string str)
		{
			string result;
			result = str.Replace("\'", "\\\'");
			return result;
		}
	}
	
