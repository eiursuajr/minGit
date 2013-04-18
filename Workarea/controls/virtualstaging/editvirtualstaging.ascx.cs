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
//using Ektron.Cms.Common.EkConstants;

	public partial class editvirtualstaging : System.Web.UI.UserControl
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
			jsUniqueID.Text = "editvirtualstaging";
			m_refMsg = (new CommonApi()).EkMsgRef;
		}
		private bool DisplayEditScreen()
		{
			SiteAPI m_refSiteApi = new SiteAPI();
			UserAPI m_refUserApi = new UserAPI();
			
			try
			{
				AppImgPath = m_refSiteApi.AppImgPath;
				AppName = m_refSiteApi.AppName;
				SITEPATH = m_refSiteApi.SitePath;
				//jsContentLanguage.Text = Convert.ToString(settings_data.Language)
				
				td_asset_loc.InnerHtml = "<input type=\"text\" size=\"50\" maxlength=\"255\" name=\"asset_loc\" id=\"asset_loc\" value=\"" + "assets" + "\">";
				td_private_asset_loc.InnerHtml = "<input type=\"text\" id=\"private_asset_loc\" name=\"private_asset_loc\" size=\"50\" maxlength=\"255\" value=\"" + "privateassets" + "\">";
				td_DomainUserName.InnerHtml = "<input type=\"text\" id=\"DomainUserName\" name=\"DomainUserName\" size=\"50\" maxlength=\"255\" value=\"" + "user name" + "\">";
				td_ConfirmPassword.InnerHtml = "<input type=\"password\" id=\"ConfirmPassword\" name=\"ConfirmPassword\" size=\"50\" maxlength=\"255\" value=\"" + "user1" + "\">";
				td_Password.InnerHtml = "<input type=\"password\" id=\"Password\" name=\"Password\" size=\"50\" maxlength=\"255\" value=\"" + "user1" + "\">";
				
				return false;
			}
			catch (Exception)
			{
                return false;
			}
		}
		public bool EditVirtualStagingControl()
		{
			bool result = false;
			this.ID = "editvirtualstaging";
			if (!IsPostBack)
			{
				
				result = DisplayEditScreen();
			}
			else
			{
				result = ProcessSubmission();
			}
			return result;
		}
		public bool ProcessSubmission()
		{
			SiteAPI m_refSiteApi = new SiteAPI();
			
			//read form fields here and call update api
			return true;
		}		
	}