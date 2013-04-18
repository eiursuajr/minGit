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

public partial class Multimedia_windowsmediaparams : System.Web.UI.UserControl
	{
		
		private string m_MediaText;
		private Ektron.Cms.CommonApi m_refContentApi = new Ektron.Cms.CommonApi();
		public string MediaText
		{
			set
			{
				m_MediaText = value;
			}
		}
		
		protected void Page_Load(object sender, System.EventArgs e)
		{
			//Me.ltWMPreview.Text = "<span id=""Results_WindowsMedia"" name=""Results_WindowsMedia"">" & m_MediaText & "</span>"
			lblContextMenu.Text = m_refContentApi.EkMsgRef.GetMessage("lbl context menu");
			lblPlayCount.Text = m_refContentApi.EkMsgRef.GetMessage("lbl playcount");
			lblEnabled.Text = m_refContentApi.EkMsgRef.GetMessage("enabled");
			lblMode.Text = m_refContentApi.EkMsgRef.GetMessage("lbl uimode");
			lblWindowless.Text = m_refContentApi.EkMsgRef.GetMessage("lbl windowless");
            Ektron.Cms.API.JS.RegisterJS(this, m_refContentApi.AppPath + "controls/media/wmplayerparams.js", "EktronMediaPlayerParamsJS");

		}
	}
	

