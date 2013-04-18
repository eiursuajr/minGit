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
using Ektron.Editors.JavascriptEditorControls;
using Ektron.Cms.Common;
using Ektron.Cms;
using Ektron.Cms.Workarea;

	public partial class Workarea_signature : System.Web.UI.Page
	{		
		protected ContentAPI m_refContApi = new Ektron.Cms.ContentAPI();
		protected EkMessageHelper m_refMsg;
		
		protected void Page_Load(object sender, System.EventArgs e)
		{
			m_refMsg = m_refContApi.EkMsgRef;
			Utilities.ValidateUserLogin();
            if (m_refContApi.RequestInformationRef.UserId == 0)
            {
                Response.Redirect(m_refContApi.ApplicationPath + "reterror.aspx?info=" + Server.UrlEncode(m_refMsg.GetMessage("msg login cms user")), false);
                return;
            } 
			Ektron.Cms.API.JS.RegisterJS(this, Ektron.Cms.API.JS.ManagedScript.EktronJS);
			Ektron.Cms.API.JS.RegisterJS(this, Ektron.Cms.API.JS.ManagedScript.EktronStringJS);
			Ektron.Cms.API.JS.RegisterJS(this, Ektron.Cms.API.JS.ManagedScript.EktronXmlJS);

			//Setting Content Designer properties	
            cdSignature.AllowScripts = true;
            cdSignature.Height = 300;
            cdSignature.Visible = true;
            cdSignature.Toolbars = Ektron.ContentDesignerWithValidator.Configuration.Minimal;
            cdSignature.ShowHtmlMode = false;

			// Localization
			cmdOk.Value = m_refMsg.GetMessage("btn save");
			close.Value = m_refMsg.GetMessage("btn cancel");
			sSignatureTooLong.Text = m_refMsg.GetMessage("js:Please choose a shorter signature");
		}
		
	}
