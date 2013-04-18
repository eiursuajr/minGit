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

	public partial class newformwizard : System.Web.UI.UserControl
	{
		
		
		
		#region Private Members
		protected Ektron.Cms.Common.EkMessageHelper m_refmsg;
		#endregion
		
		private void Page_Load(System.Object sender, System.EventArgs e)
		{
			StyleHelper objStyle = new StyleHelper();
			m_refmsg = (new Ektron.Cms.CommonApi()).EkMsgRef;
			
			HelpButton1.Text = objStyle.GetHelpButton("FormWizardStep1", "");
			HelpButton2.Text = objStyle.GetHelpButton("FormWizardStep2", "");
			HelpButton3.Text = objStyle.GetHelpButton("FormWizardStep3", "");
			HelpButton4.Text = objStyle.GetHelpButton("FormWizardStep4", "");
			HelpButton5.Text = objStyle.GetHelpButton("FormWizardStep5", "");
			objStyle = null;
		}
		
	}
	

