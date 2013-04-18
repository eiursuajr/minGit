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
using Ektron.Cms.Common;

	public partial class viewxslt : System.Web.UI.Page
	{
		protected ContentAPI m_refContentApi = new ContentAPI();
		protected string AppImgPath = "";
		protected StyleHelper m_refStyle = new StyleHelper();
		protected EkMessageHelper m_refMsg;
		protected long m_intId = 0;

		private void Page_Load(System.Object sender, System.EventArgs e)
		{
			XmlConfigData xml_config_data;
			try
			{
				m_refMsg = m_refContentApi.EkMsgRef;
				Utilities.ValidateUserLogin();
				StyleSheetJS.Text = m_refStyle.GetClientScript();
				RegisterResources();
				if (Request.QueryString["id"] != null)
				{
					m_intId = Convert.ToInt64(Request.QueryString["id"]);
				}
				AppImgPath = m_refContentApi.AppImgPath;
				ViewXsltToolBar();
				xml_config_data = m_refContentApi.GetXmlConfiguration(m_intId);
				if (!(xml_config_data == null))
				{
					display_xslt.Value = xml_config_data.PackageDisplayXslt;
				}
			}
			catch (Exception ex)
			{
				Utilities.ShowError(ex.Message);
			}
		}
		private void ViewXsltToolBar()
		{
			System.Text.StringBuilder result = new System.Text.StringBuilder();
			divTitleBar.InnerHtml = m_refStyle.GetTitleBar("Display Xslt");
			result.Append("<table><tr>");
            result.Append(m_refStyle.GetButtonEventsWCaption(AppImgPath + "../UI/Icons/back.png", "javascript:history.back()", "Go Back", m_refMsg.GetMessage("btn back"), "", StyleHelper.BackButtonCssClass, true));
			result.Append(StyleHelper.ActionBarDivider);
			result.Append("<td>" + m_refStyle.GetHelpButton("DisplayXSLT", "") + "</td>");
			result.Append("</tr></table>");
			divToolBar.InnerHtml = result.ToString();
			result = null;
		}
		private void RegisterResources()
		{
			Ektron.Cms.API.JS.RegisterJS(this, m_refContentApi.ApplicationPath + "java/jfunct.js", "EktronJFunctJS");
			Ektron.Cms.API.JS.RegisterJS(this, m_refContentApi.ApplicationPath + "java/toolbar_roll.js", "EktronToolbarRollJS");
			Ektron.Cms.API.JS.RegisterJS(this, m_refContentApi.ApplicationPath + "java/workareahelper.js", "EktronWorkareaHelperJS");
		}
	}