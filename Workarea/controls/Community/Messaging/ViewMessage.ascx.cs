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
//using Ektron.Cms.Common.EkFunctions;
//using Ektron.Cms.CustomFieldsApi;
using Ektron.Cms.Content;
using Ektron.Editors;


	public partial class controls_Community_Messaging_ViewMessage : System.Web.UI.UserControl
	{
		
		
		protected StyleHelper m_refStyle = new StyleHelper();
		protected Ektron.Cms.Common.EkMessageHelper m_refMsg;
		protected string m_id = "";
		protected string m_mode = "";
		protected bool m_prev = false;
		protected bool m_next = false;
		protected long m_userId = 0;
		protected string m_msgUserId = "";
		protected bool m_sentMode = false;
		protected bool m_bCanReply = true;
		
		protected void Page_Load(object sender, System.EventArgs e)
		{
			CommonApi refCommonAPI = new CommonApi();
			m_userId = refCommonAPI.RequestInformationRef.UserId;
			m_refMsg = (new CommonApi()).EkMsgRef;
			RegisterResources();
			if (! (Request.QueryString["id"] == null))
			{
				m_id = (string) (Request.QueryString["id"].ToLower());
			}
			if (! (Request.QueryString["userid"] == null))
			{
				m_msgUserId = (string) (Request.QueryString["userid"].ToLower().Trim());
			}
			else
			{
				m_msgUserId = m_userId.ToString();
			}
			if (! (Request.QueryString["mode"] == null))
			{
				m_mode = (string) (Request.QueryString["mode"].ToLower());
			}
			if ("prev" == m_mode)
			{
				m_prev = true;
			}
			else if ("next" == m_mode)
			{
				m_next = true;
			}
			
			if (("del" == m_mode) && (m_id.Trim().Length > 0) && Ektron.Cms.Common.EkFunctions.IsNumeric(m_id))
			{
				long[] delList = new long[] {(Convert.ToInt64(m_id))};
				PrivateMessage objPM = new PrivateMessage(refCommonAPI.RequestInformationRef);
				objPM.DeleteMessageList(delList, m_sentMode);
				refCommonAPI = null;
				objPM = null;
				Response.ClearContent();
				Response.Redirect((string) ("CommunityMessaging.aspx?action=" + (SentMode ? "viewallsent" : "viewall")), false);
			}
			else
			{
				LoadMsg();
				LoadToolBar();
			}
			refCommonAPI = null;
			m_refMsg = null;
		}
		
		protected void LoadToolBar()
		{
			UserAPI m_refUserApi = new UserAPI();
			string AppImgPath = m_refUserApi.AppImgPath;
			ContentAPI refContentAPI = new ContentAPI();
			string AppPath = refContentAPI.AppPath;
			
			System.Text.StringBuilder result = new System.Text.StringBuilder();
			try
			{
				divTitleBar.InnerHtml = m_refStyle.GetTitleBar(GetMessage("lbl messages"));
				result.Append("<table><tr>");

				// Back button
				result.Append(m_refStyle.GetButtonEventsWCaption(AppPath + "images/ui/icons/back.png", (string)("CommunityMessaging.aspx?action=" + (SentMode ? "viewallsent" : "viewall")), GetMessage("btn back"), GetMessage("btn back"), "", StyleHelper.BackButtonCssClass, true));
				
				if (m_sentMode == false)
				{
					result.Append(m_refStyle.GetButtonEventsWCaption(AppPath + "images/ui/icons/emailAdd.png", "CommunityMessaging.aspx?action=editmsg", GetMessage("lbl compose a message"), GetMessage("lbl compose a message"), "", StyleHelper.AddEmailButtonCssClass, true));
					result.Append(m_refStyle.GetButtonEventsWCaption(AppPath + "images/ui/icons/emailDelete.png", (string) ("CommunityMessaging.aspx?action=" + (SentMode ? "viewsentmsg" : "viewmsg") + "&mode=del&id=" + m_id), GetMessage("btn delete"), GetMessage("btn delete"), "", StyleHelper.DeleteEmailButtonCssClass));
					if (m_bCanReply)
					{
						result.Append(m_refStyle.GetButtonEventsWCaption(AppPath + "images/ui/icons/emailReply.png", (string) ("CommunityMessaging.aspx?action=editmsg&id=" + m_id), GetMessage("lbl reply message"), GetMessage("lbl reply message"), "", StyleHelper.ReplyToEmailButtonCssClass));
					}
				}
				else
				{
					result.Append(m_refStyle.GetButtonEventsWCaption(AppPath + "images/ui/icons/emailDelete.png", (string) ("CommunityMessaging.aspx?action=" + (SentMode ? "viewsentmsg" : "viewmsg") + "&mode=del&id=" + m_id), GetMessage("btn delete"), GetMessage("btn delete"), "", StyleHelper.DeleteEmailButtonCssClass, true));
				}

				result.Append(m_refStyle.GetButtonEventsWCaption(AppPath + "images/ui/icons/emailForward.png", (string) ("CommunityMessaging.aspx?action=editmsg&mode=fwd&id=" + m_id + "&userid=" + m_msgUserId), GetMessage("lbl forward message"), GetMessage("lbl forward message"), "", StyleHelper.ForwardEmailButtonCssClass));
				
				long previousMsgID = 0;
				long nextMsgID = 0;
				refContentAPI.GetAjacentPrivateMessagesForCurrentUser(Convert.ToInt64(m_id), System.Convert.ToBoolean(!(SentMode)), ref previousMsgID, ref nextMsgID);
				if (previousMsgID > 0)
				{
					result.Append(m_refStyle.GetButtonEventsWCaption(AppPath + "images/ui/icons/arrowLeft.png", (string) ("CommunityMessaging.aspx?action=" + (SentMode ? "viewsentmsg" : "viewmsg") + "&id=" + previousMsgID.ToString() + "&userid=" + m_msgUserId.ToString()), GetMessage("lbl previous message"), GetMessage("lbl previous message"), "", StyleHelper.PreviousButtonCssClass));
				}
				else
				{
					result.Append(m_refStyle.GetButtonEventsWCaption(AppPath + "images/ui/icons/arrowLeftOff.png", "#", GetMessage("lbl previous message"), GetMessage("lbl previous message"), "", StyleHelper.PreviousDisabledButtonCssClass));
				}

				if (nextMsgID > 0)
				{
					result.Append(m_refStyle.GetButtonEventsWCaption(AppPath + "images/ui/icons/arrowRight.png", (string) ("CommunityMessaging.aspx?action=" + (SentMode ? "viewsentmsg" : "viewmsg") + "&id=" + nextMsgID.ToString() + "&userid=" + m_msgUserId.ToString()), GetMessage("lbl next message"), GetMessage("lbl next message"), "", StyleHelper.NextButtonCssClass));
				}
				else
				{
					result.Append(m_refStyle.GetButtonEventsWCaption(AppPath + "images/ui/icons/arrowRightOff.png", "#", GetMessage("lbl next message"), GetMessage("lbl next message"), "", StyleHelper.NextDisabledButtonCssClass));
				}

				//CommunityMessaging.aspx?action=" + IIf(SentMode, "viewsentmsg", "viewmsg") + "&id=" + aMessages(idx).ID.ToString()
				
				result.Append(m_refStyle.GetButtonEventsWCaption(AppPath + "images/ui/icons/print.png", "javascript:window.print();", GetMessage("lbl print message"), GetMessage("lbl print message"), "", StyleHelper.PrintButtonCssClass));
				result.Append(StyleHelper.ActionBarDivider);
				result.Append("<td>");
				if (SentMode)
				{
					result.Append(m_refStyle.GetHelpButton("view_sent_msg", ""));
				}
				else
				{
					result.Append(m_refStyle.GetHelpButton("view_msg", ""));
				}
				result.Append("</td>");
				result.Append("</tr></table>");
				
				divToolBar.InnerHtml = result.ToString();
				result = null;
			}
			catch (Exception ex)
			{
				Utilities.ShowError(ex.Message);
			}
			finally
			{
				refContentAPI = null;
			}
		}
		
		protected void LoadMsg()
		{
			try
			{
				if ((m_id.Length > 0) && (Ektron.Cms.Common.EkFunctions.IsNumeric(m_id)))
				{
					PrivateMessage aMsg = new PrivateMessage();
					CommonApi refCommonAPI = new CommonApi();
					PrivateMessage objPM = new PrivateMessage(refCommonAPI.RequestInformationRef);
					objPM.GetByMessageID(Convert.ToInt64(m_id));
					
					aMsg = IsMyMessage(objPM);
					if (aMsg.FromUserDeleted)
					{
						m_bCanReply = false;
					}
					StringBuilder msgHeader = new StringBuilder();
					msgHeader.AppendLine("<table class=\"ViewMsgHeader ektronGrid\" cellspacing=\"0\">");
					msgHeader.AppendLine("<tbody>");
					// Add From and Sent
					msgHeader.AppendLine("<tr>");
					msgHeader.AppendLine("<td class=\"ViewMsgLabel label\">" + m_refMsg.GetMessage("generic from label") + "</td>");
					msgHeader.AppendLine("<td><div class=\"ViewMsgDate\"><span class=\"ViewMsgLabel\">" + m_refMsg.GetMessage("generic sent label") + "</span>" + aMsg.DateCreated + "</div>" + aMsg.FromUserDisplayName + "</td>");
					msgHeader.AppendLine("</tr>");
					// Add To
					msgHeader.AppendLine("<tr>");
					msgHeader.Append("<td class=\"ViewMsgLabel label\">" + m_refMsg.GetMessage("generic to label") + "</td>");
					msgHeader.Append("<td>" + aMsg.GetFormattedRecipientList(";") + "</td>");
					msgHeader.AppendLine("</tr>");
					// Add Subject
					msgHeader.AppendLine("<tr>");
					msgHeader.Append("<td class=\"ViewMsgLabel label\">" + m_refMsg.GetMessage("generic subject label") + "</td>");
					msgHeader.Append("<td>" + aMsg.Subject + "</td>");
					msgHeader.AppendLine("</tr>");
					msgHeader.AppendLine("</tbody>");
					msgHeader.AppendLine("</table>");
					
					ltrMsgView.Text = "<div class=\"ViewMsgContainer\">";
					ltrMsgView.Text += msgHeader.ToString();
					ltrMsgView.Text += "<hr class=\"ViewMsgHR\"/>";
					ltrMsgView.Text += "<div class=\"ViewMsgMessage\">" + HttpUtility.UrlDecode(aMsg.Message) + "</div>";
					ltrMsgView.Text += "</div>";
					aMsg.MarkAsRead();
				}
				
			}
			catch (Exception)
			{
			}
			finally
			{
			}
		}
		
		protected string GetMessage(string resource)
		{
			return this.m_refMsg.GetMessage(resource);
		}
		
		public bool SentMode
		{
			get
			{
				return m_sentMode;
			}
			set
			{
				m_sentMode = value;
			}
		}
		protected PrivateMessage IsMyMessage(PrivateMessage msg)
		{
			PrivateMessage returnValue;
			PrivateMessage result = null;
			try
			{

                if (msg.FromUserID == m_userId || (msg.IsARecipient(m_userId)) || (Ektron.Cms.Common.EkFunctions.IsNumeric(m_msgUserId) && msg.IsARecipient(Convert.ToInt64(m_msgUserId))))
				{
					result = msg;
				}
			}
			catch (Exception)
			{
				result = null;
			}
			finally
			{
				if (result == null)
				{
					result = new PrivateMessage();
				}
				returnValue = result;
			}
			return returnValue;
		}
		protected PrivateMessage GetMyMsg(PrivateMessage[] msgs)
		{
			PrivateMessage returnValue;
			PrivateMessage result = null;
			int idx;
			try
			{
				if ((msgs != null) && (msgs.Length > 0))
				{
					for (idx = 0; idx <= msgs.Length - 1; idx++)
					{
						if ((msgs[idx].IsARecipient(m_userId)) || (Ektron.Cms.Common.EkFunctions.IsNumeric(m_msgUserId) && msgs[idx].IsARecipient(Convert.ToInt64(m_msgUserId))))
						{
							result = msgs[idx];
							break;
						}
					}
				}
				
			}
			catch (Exception)
			{
				result = null;
			}
			finally
			{
				if (result == null)
				{
					result = new Ektron.Cms.Content.PrivateMessage();
				}
				returnValue = result;
			}
			return returnValue;
		}
		
		protected string WrapLongSubject(string msg)
		{
			int wrapLength = 45;
			string result = "";
			string thisLine = "";
			string[] words;
			int idx = 0;
			
			words = msg.Split(" ".ToCharArray());
			for (idx = 0; idx <= words.Length - 1; idx++)
			{
				if ((thisLine + words[idx].Trim()).Length < wrapLength)
				{
					result += " " + words[idx].Trim();
					thisLine += " " + words[idx].Trim();
				}
				else
				{
					result += "<br />" + words[idx].Trim();
					thisLine = words[idx].Trim();
				}
			}
			
			return result;
		}
		private void RegisterResources()
		{
			Ektron.Cms.API.JS.RegisterJS(this, Ektron.Cms.API.JS.ManagedScript.EktronJFunctJS);
			Ektron.Cms.API.JS.RegisterJS(this, Ektron.Cms.API.JS.ManagedScript.EktronToolBarRollJS);
			Ektron.Cms.API.JS.RegisterJS(this, Ektron.Cms.API.JS.ManagedScript.EktronStyleHelperJS);
			Ektron.Cms.API.JS.RegisterJS(this, Ektron.Cms.API.JS.ManagedScript.EktronWorkareaHelperJS);
		}
	}