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


	public partial class navbuttons : System.Web.UI.Page
	{
		
		
		protected string m_strStyleSheetJS = "";
		protected CommonApi m_refAPI = new CommonApi();
		
		private void Page_Load(System.Object sender, System.EventArgs e)
		{
			StyleHelper objStyle;
			Ektron.Cms.Common.EkMessageHelper m_refMsg;
			try
			{
				Utilities.ValidateUserLogin();
				if ((m_refAPI.RequestInformationRef.IsMembershipUser >0) || (m_refAPI.RequestInformationRef.UserId == 0))
				{
					Response.Redirect("blank.htm", false);
					return;
				}
				m_refMsg = m_refAPI.EkMsgRef;
				objStyle = new StyleHelper();
				m_strStyleSheetJS = objStyle.GetClientScript();
				jsAppImgPath.Text = m_refAPI.AppImgPath;
				Button0.InnerHtml = "<img style=\"MARGIN-LEFT: 3px;\" id=\"iconImage0\" title=\"" + m_refMsg.GetMessage("alt select contenttree button text") + "\" src=\"images/application/navbar/icon_content.gif\" align=\"absmiddle\"> " + m_refMsg.GetMessage("content button text");
				if (Ektron.Cms.DataIO.LicenseManager.LicenseManager.IsFeatureEnable(m_refAPI.RequestInformationRef, Ektron.Cms.DataIO.LicenseManager.Feature.SocialNetworking))
				{
					Button4.InnerHtml = "<img style=\"MARGIN-LEFT: 3px;\" id=\"Img5\" title=\"" + m_refMsg.GetMessage("alt select workspacetree button text") + "\" src=\"images/application/user.gif\" align=\"absmiddle\" width=\"16\" height=\"16\"> " + m_refMsg.GetMessage("lbl my stuff");
				}
				Button1.InnerHtml = "<img style=\"MARGIN-LEFT: 3px;\" id=\"Img2\" title=\"" + m_refMsg.GetMessage("alt select librarytree button text") + "\" src=\"images/application/navbar/icon_library.gif\" align=\"absmiddle\"> " + m_refMsg.GetMessage("library button text");
				Button2.InnerHtml = "<img style=\"MARGIN-LEFT: 3px;\" id=\"Img3\" title=\"" + m_refMsg.GetMessage("alt select moduletree button text") + "\" src=\"images/application/navbar/icon_modules.gif\" align=\"absmiddle\"> " + m_refMsg.GetMessage("modules button text");
				Button3.InnerHtml = "<img style=\"MARGIN-LEFT: 3px;\" id=\"Img4\" title=\"" + m_refMsg.GetMessage("alt select admintree button text") + "\" src=\"images/application/navbar/icon_admin.gif\" align=\"absmiddle\"> " + m_refMsg.GetMessage("administrate button text");
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
	

