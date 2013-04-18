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


	public partial class preview : System.Web.UI.Page
	{
		
		
		private void Page_Load(System.Object sender, System.EventArgs e)
		{
			CommonApi m_refAPI = new CommonApi();
			try
			{
				if (m_refAPI.GetCookieValue("site_preview") == "1")
				{
					m_refAPI.SetCookieValue("site_preview", "0");
				}
				else
				{
					m_refAPI.SetCookieValue("site_preview", "1");
				}
			}
			catch (Exception)
			{
			}
			finally
			{
				m_refAPI = null;
			}
		}
	}
	
