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



	public partial class naviconbar : System.Web.UI.Page
	{


        protected Ektron.Cms.Common.EkMessageHelper m_MsgHelper;
		protected string StyleSheetJS = "";
		protected CommonApi m_refAPI = new CommonApi();
		
		private void Page_Load(System.Object sender, System.EventArgs e)
		{
			StyleHelper objStyle;
			try
			{
				m_MsgHelper = m_refAPI.EkMsgRef;
				objStyle = new StyleHelper();
				StyleSheetJS = objStyle.GetClientScript();
				jsAppImgPath.Text = m_refAPI.AppImgPath;
				Utilities.ValidateUserLogin();
			}
			catch (Exception)
			{
			}
			finally
			{
				objStyle = null;
			}
		}
	}

