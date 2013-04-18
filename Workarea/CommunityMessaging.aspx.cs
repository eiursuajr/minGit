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
using Ektron.Cms.API;
using Ektron.Cms.Common;
//using Ektron.Cms.Common.EkFunctions;
//using Ektron.Cms.CustomFieldsApi;
using Ektron.Cms.Content;


	public partial class CommunityMessaging : System.Web.UI.Page
	{
		protected string m_action = "";
		protected EkMessageHelper m_refMsg;
		protected long m_userId = 0;
		protected string m_id = "";
        protected StyleHelper m_refStyle = new StyleHelper();
		
		protected void Page_Init(object sender, System.EventArgs e)
		{
			CommonApi refCommonAPI = new CommonApi();
			
			RegisterResources();
			Utilities.ValidateUserLogin();
			if (refCommonAPI.RequestInformationRef.IsMembershipUser == 1 || refCommonAPI.RequestInformationRef.UserId == 0)
			{
				Response.Redirect(refCommonAPI.SitePath + "login.aspx");
				return;
			}
			else
			{
				if (! Ektron.Cms.DataIO.LicenseManager.LicenseManager.IsFeatureEnable(refCommonAPI.RequestInformationRef, Ektron.Cms.DataIO.LicenseManager.Feature.SocialNetworking))
				{
					Utilities.ShowError(refCommonAPI.EkMsgRef.GetMessage("feature locked error"));
				}
				m_userId = refCommonAPI.RequestInformationRef.UserId;
				if (! (Request.QueryString["id"] == null))
				{
					m_id = (string) (Request.QueryString["id"].ToLower());
				}
				m_refMsg = (new CommonApi()).EkMsgRef;
				msgJSContainer.Text = m_refStyle.GetClientScript();
				
				if (! (Request.QueryString["action"] == null))
				{
					m_action = (string) (Request.QueryString["action"].ToLower());
				}

                controls_Community_Messaging_EditMessage m_EditMessage;
                Messaging_ViewMessages m_ViewMessages;
                controls_Community_Messaging_ViewMessage m_ViewMessage;
               
                
				switch (m_action)
				{
					case "viewall":
                        m_ViewMessages = (Messaging_ViewMessages)(LoadControl("Controls/Community/Messaging/ViewMessages.ascx"));
                        MsgCtlHolder.Controls.Add(m_ViewMessages);
						break;
					case "viewallsent":
                        m_ViewMessages = (Messaging_ViewMessages)(LoadControl("Controls/Community/Messaging/ViewMessages.ascx"));
                        m_ViewMessages.SentMode = true;
                        MsgCtlHolder.Controls.Add(m_ViewMessages);
						break;
					case "viewmsg":
                        m_ViewMessage = (controls_Community_Messaging_ViewMessage)(LoadControl("Controls/Community/Messaging/ViewMessage.ascx"));
                        MsgCtlHolder.Controls.Add(m_ViewMessage);
						break;
					case "viewsentmsg":
                        m_ViewMessage = (controls_Community_Messaging_ViewMessage)(LoadControl("Controls/Community/Messaging/ViewMessage.ascx"));
                        MsgCtlHolder.Controls.Add(m_ViewMessage);
                        m_ViewMessage.SentMode = true;
						break;
					case "editmsg":
                        m_EditMessage = (controls_Community_Messaging_EditMessage)(LoadControl("Controls/Community/Messaging/EditMessage.ascx"));
                        MsgCtlHolder.Controls.Add(m_EditMessage);
						break;
					case "replymsg":
						break;
						
					default:
                        m_ViewMessages = (Messaging_ViewMessages)(LoadControl("Controls/Community/Messaging/ViewMessages.ascx"));
                        MsgCtlHolder.Controls.Add(m_ViewMessages);
						break;
				}
			}
			
		}
		
		protected void Page_Load(object sender, System.EventArgs e)
		{
			Response.Expires = 0;
			Response.Cache.SetCacheability(HttpCacheability.NoCache);
		}
		
		public long UserId
		{
			get
			{
				return (m_userId);
			}
		}
		
		protected void RegisterResources()
		{
			// register JS
			JS.RegisterJS(this, JS.ManagedScript.EktronJS);
			
			// register CSS
			Css.RegisterCss(this, Css.ManagedStyleSheet.EktronCommunityCss);
			Css.RegisterCss(this, Css.ManagedStyleSheet.EktronCommunitySearchCss);
			Css.RegisterCss(this, Css.ManagedStyleSheet.EktronFixedPositionToolbarCss);
		}
	}
	

