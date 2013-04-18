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



	public partial class navtoolbar : System.Web.UI.Page
	{
		
		protected CommonApi m_refAPI = new CommonApi();
        protected Ektron.Cms.Common.EkMessageHelper m_msgHelper;
		
		private void Page_Load(System.Object sender, System.EventArgs e)
		{
			StyleHelper objStyle;
			try
			{
				RegisterResources();
				Utilities.ValidateUserLogin();
				if ((m_refAPI.RequestInformationRef.IsMembershipUser > 0) || (m_refAPI.RequestInformationRef.UserId == 0))
				{
					Response.Redirect("blank.htm", false);
					return;
				}
				objStyle = new StyleHelper();
				StyleSheetJS.Text = objStyle.GetClientScript();
				jsAppImgPath.Text = m_refAPI.AppImgPath;
				HelpButton.Text = objStyle.GetHelpButton("navtoolbar_aspx", "");
				m_msgHelper = m_refAPI.EkMsgRef;
				
			}
			catch (Exception)
			{
				
			}
			finally
			{
				objStyle = null;
			}
		}
		private void RegisterResources()
		{
			Ektron.Cms.API.JS.RegisterJS(this, Ektron.Cms.API.JS.ManagedScript.EktronToolBarRollJS);
			Ektron.Cms.API.JS.RegisterJS(this, Ektron.Cms.API.JS.ManagedScript.EktronStyleHelperJS);
		}
	}

