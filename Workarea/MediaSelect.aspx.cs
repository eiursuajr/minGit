using System.Web.UI.WebControls;
using System.Configuration;
using System.Collections;

using System.Data;
using System.Web.Caching;

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

	public partial class MediaSelect : System.Web.UI.Page
	{


		protected StyleHelper m_refStyle = new StyleHelper();
		protected EkMessageHelper m_refMsg;
		protected string AppImgPath = "";
		protected ContentAPI m_refContentApi = new ContentAPI();

		protected void Page_Load(object sender, System.EventArgs e)
		{
			Utilities.ValidateUserLogin();
			if (m_refContentApi.RequestInformationRef.IsMembershipUser == 1 || m_refContentApi.RequestInformationRef.UserId == 0)
			{
				Response.Redirect("blank.htm", false);
				return;
			}
			m_refMsg = m_refContentApi.EkMsgRef;
			AppImgPath = m_refContentApi.AppImgPath;
			RegisterResources();

			ltr_folder.Text = m_refMsg.GetMessage("lbl folder");
			ltr_folderId.Text = m_refMsg.GetMessage("lbl folder-id");

			StyleSheetJS.Text = m_refStyle.GetClientScript();
			BuildToolBar();
		}

		private void BuildToolBar()
		{
			System.Text.StringBuilder result = new System.Text.StringBuilder();
			divTitleBar.InnerHtml = m_refStyle.GetTitleBar(m_refMsg.GetMessage("mediaselect title bar"));
			result.Append("<table><tr>");
			result.Append(m_refStyle.GetButtonEventsWCaption(m_refContentApi.AppPath + "images/UI/Icons/cancel.png", "#", m_refMsg.GetMessage("alt exit without selecting content"), m_refMsg.GetMessage("btn cancel"), "Onclick=\"metaselect_close();return false;\"", StyleHelper.CancelButtonCssClass, true));
			result.Append(m_refStyle.GetButtonEventsWCaption(m_refContentApi.AppPath + "images/UI/Icons/save.png", "#", m_refMsg.GetMessage("alt click here to save"), m_refMsg.GetMessage("btn save"), "Onclick=\"metaselect_saveChecked();return false;\"", StyleHelper.SaveButtonCssClass, true));

			result.Append(m_refStyle.GetButtonEventsWCaption(m_refContentApi.AppPath + "images/UI/Icons/arrowUp.png", "#", m_refMsg.GetMessage("alt click here to move the highlighted item closer to the top of the order"), m_refMsg.GetMessage("lbl move up"), "Onclick=\"metaselect_reorder(true);return false;\"", StyleHelper.UpButtonCssClass));
			result.Append(m_refStyle.GetButtonEventsWCaption(m_refContentApi.AppPath + "images/UI/Icons/arrowDown.png", "#", m_refMsg.GetMessage("alt click here to move the highlighted item closer to the bottom of the order"), m_refMsg.GetMessage("lbl move down"), "Onclick=\"metaselect_reorder(false);return false;\"", StyleHelper.DownButtonCssClass));

			result.Append(m_refStyle.GetButtonEventsWCaption(m_refContentApi.AppPath + "images/UI/Icons/delete.png", "#", m_refMsg.GetMessage("alt click here to remove selected items"), m_refMsg.GetMessage("btn delete"), "OnClick=\"metaselect_removeChecked();return false;\" ", StyleHelper.DeleteButtonCssClass));
			result.Append("</tr></table>");
			divToolBar.InnerHtml = result.ToString();
			result = null;
		}
		private void RegisterResources()
		{
			Ektron.Cms.API.JS.RegisterJS(this, Ektron.Cms.API.JS.ManagedScript.EktronJS);
			Ektron.Cms.API.JS.RegisterJS(this, m_refContentApi.ApplicationPath + "java/toolbar_roll.js", "EktronToolbarRollJS");
			Ektron.Cms.API.JS.RegisterJS(this, m_refContentApi.ApplicationPath + "java/workareahelper.js", "EktronWorkareaHelperJS");
		}
	}


