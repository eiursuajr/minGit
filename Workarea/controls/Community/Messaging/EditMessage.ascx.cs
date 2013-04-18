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
//using System.DateTime;
using Ektron.Cms;
using Ektron.Cms.Common;
//using Ektron.Cms.Common.EkFunctions;
//using Ektron.Cms.CustomFieldsApi;
using Ektron.Cms.Content;
//using Ektron.Cms.Common.EkConstants;
//using Ektron.Cms.Common.EkEnumeration;
using Ektron.Editors;



	public partial class controls_Community_Messaging_EditMessage : System.Web.UI.UserControl
	{
        

        protected UserData[] m_user_list = (UserData[])Array.CreateInstance(typeof(Ektron.Cms.UserData), 0);

		protected StyleHelper m_refStyle = new StyleHelper();
		protected EkMessageHelper m_refMsg;
		protected string m_id = "";
		protected string m_mode = "";
		protected bool m_replying = false;
		protected bool m_forwarding = false;
		protected long m_userId = 0;
		protected UserAPI m_refUserApi = null;
		protected bool m_callbackwrap = true;
		protected string m_callbackresult = "";
		protected bool m_ExecuteFillOnCallBack = false;
		protected System.Collections.Specialized.NameValueCollection m_PostBackData = null;
		protected string m_recipientsPageStr = "1";
		protected int m_recipientsPage = 1;
		protected bool m_friendsOnly = false;
		protected CommonApi m_refCommon = new CommonApi();
		protected ContentAPI m_refContentAPI = new ContentAPI();
		protected EkEnumeration.UserTypes m_UserType = Ektron.Cms.Common.EkEnumeration.UserTypes.AuthorType;
		
		protected string m_strSearchText = "";
		protected string m_strKeyWords = "";
		protected int m_intTotalPages = 0;
		protected string m_uniqueId = "";
		protected string m_searchMode = "display_name";
		protected string cssFilesPath = "";
		
		protected void Page_Load(object sender, System.EventArgs e)
		{
			CommonApi refCommonAPI = new CommonApi();
			m_userId = refCommonAPI.RequestInformationRef.UserId;
			m_refMsg = (new CommonApi()).EkMsgRef;
			m_refUserApi = new UserAPI();
			string msgSubject = "";
			string msgText = "";
            if (!Page.IsCallback)
			    RegisterResources();
			m_uniqueId = this.ClientID;
			MsgToLabel.InnerText = m_refMsg.GetMessage("lbl generic to") + ":";
			MsgSubjectLabel.InnerText = m_refMsg.GetMessage("lbl generic subject") + ":";
			
			cgae_userselect_done_btn.Attributes.Add("onclick", "GetCommunityMsgObject(\'" + m_uniqueId + "\').MsgSaveMessageTargetUI(); return false");
			cgae_userselect_done_btn.Attributes.Add("class", "EktMsgTargetsDoneBtn");
			cgae_userselect_done_btn.Text = m_refMsg.GetMessage("btn done");
			cgae_userselect_done_btn.ToolTip = m_refMsg.GetMessage("btn done");
			
			cgae_userselect_cancel_btn.Attributes.Add("onclick", "GetCommunityMsgObject(\'" + m_uniqueId + "\').MsgCancelMessageTargetUI(); return false");
			cgae_userselect_cancel_btn.Attributes.Add("class", "EktMsgTargetsCancelBtn");
			cgae_userselect_cancel_btn.Text = m_refMsg.GetMessage("btn cancel");
			cgae_userselect_cancel_btn.ToolTip = m_refMsg.GetMessage("btn cancel");
			
			
			if (! (Request.QueryString["id"] == null))
			{
				m_id = (string) (Request.QueryString["id"].ToLower());
			}
			if (! (Request.QueryString["mode"] == null))
			{
				m_mode = (string) (Request.QueryString["mode"].ToLower());
			}
			m_replying = System.Convert.ToBoolean((m_id.Trim().Length > 0) && Ektron.Cms.Common.EkFunctions.IsNumeric(m_id));
			if (m_replying && ("fwd" == m_mode))
			{
				m_forwarding = true;
				m_replying = false;
			}
			
			if (Page.IsCallback)
			{
			}
			else
			{
				if ((IsPostBack) && (! (Request.Form[(string) ("hdnRecipientsValidated" + m_uniqueId)] == null) ) && ("1" == Request.Form[(string) ("hdnRecipientsValidated" + m_uniqueId)]))
				{
					if (! (Request.Form[(string) ("msg_subject" + m_uniqueId)] == null))
					{
						msgSubject = Request.Form[(string) ("msg_subject" + m_uniqueId)];
					}
					msgText = (string) cdContent_teaser.Content;
					cssFilesPath = this.m_refContentAPI.ApplicationPath + "csslib/ektron.workarea.css";
					cdContent_teaser.Stylesheet = cssFilesPath;
					RenderSentUI(SendMessage(msgSubject, msgText));
				}
				else
				{
					RenderEditorUI();
					LoadMsg();
				}
			}
			
			refCommonAPI = null;
		}
		
		protected void RenderEditorUI()
		{
			LoadToolBar();
			
			string browseStr = (string) (m_friendsOnly ? (GetMessage("lbl browse friends")) : (GetMessage("lbl browse users")));
			
			ltrBrowseFriends.Text = "<a href=\"#EkTB_inline?height=480&width=550&caption=false&inlineId=MessageTargetUI" + ClientID + "&modal=true\" class=\"ek_thickbox\" onclick=\"return GetCommunityMsgObject(\'" + this.ClientID + "\').MsgShowMessageTargetUI(\'ektouserid" + m_uniqueId + "\', true)\" >" + "<img alt=\"" + browseStr + ("\" src=\"images/ui/icons/usersMembership.png\" /></a> <a href=\"#EkTB_inline?height=480&width=550&caption=false&inlineId=MessageTargetUI" + ClientID + "&modal=true\" class=\"ek_thickbox\" onclick=\"return GetCommunityMsgObject(\'" + this.ClientID + "\').MsgShowMessageTargetUI(\'ektouserid" + m_uniqueId + "\', true)\" >") + browseStr + "</a>";
			
			ltrMsgJSObjectId.Text = "<script type=\"text/javascript\" language=\"javascript\">" + Environment.NewLine + "GetCommunityMsgObject(\'" + m_uniqueId + "\').SetUserSelectId(\'" + Invite_UsrSel.ControlId + "\');" + Environment.NewLine + "</script>";
		}
		
		protected void RenderSentUI(bool success)
		{
			if (success)
			{
				ltrMsgView.Text = GetMessage("lbl message sent");
				Response.ClearContent();
				Response.Redirect("CommunityMessaging.aspx?action=viewall", false);
			}
			else
			{
				ltrMsgView.Text = GetMessage("lbl message sent error");
			}
		}
		
		protected void LoadToolBar()
		{
			string AppImgPath = m_refUserApi.AppPath;
			System.Text.StringBuilder sb = new System.Text.StringBuilder();
			
			try
			{
				divTitleBar.InnerHtml = m_refStyle.GetTitleBar((string) (m_replying ? (GetMessage("lbl reply message")) : (m_forwarding ? (GetMessage("lbl forward message")) : (GetMessage("lbl send message")))));
				sb.Append("<table><tr>");
				sb.Append(m_refStyle.GetButtonEventsWCaption(AppImgPath + "images/UI/Icons/back.png", "javascript:history.back();", GetMessage("btn back"), GetMessage("btn back"), "", StyleHelper.BackButtonCssClass,true));
                sb.Append(m_refStyle.GetButtonEventsWCaption(AppImgPath + "images/UI/Icons/emailSend.png", "javascript:GetCommunityMsgObject(\'" + this.ClientID + "\').SendMessage();", GetMessage("lbl send this message"), GetMessage("lbl send this message"), "", StyleHelper.EmailButtonCssClass,true));
				sb.Append(StyleHelper.ActionBarDivider);
				sb.Append("<td>");
				sb.Append(m_refStyle.GetHelpButton("composecommunitymsg", ""));
				sb.Append("</td>");
				sb.Append("</tr></table>");
				
				divToolBar.InnerHtml = sb.ToString();
			}
			catch (Exception ex)
			{
				Utilities.ShowError(ex.Message);
			}
			finally
			{
				sb = null;
			}
		}
		
		protected void LoadMsg()
		{
			try
			{
				string msgTarget = "";
				string msgTargetIds = "";
				string msgSubject = "";
				string msgText = "";
				cdContent_teaser.Visible = true;
				cssFilesPath = this.m_refContentAPI.ApplicationPath + "csslib/ektron.workarea.css";
				cdContent_teaser.Stylesheet = cssFilesPath;
				
				if (m_replying || m_forwarding)
				{
					PrivateMessage pm = GetMsg();
					if ((pm != null) && pm.ID > 0)
					{
						msgText += "<p></p><hr/><p>";
						msgText += (string) (GetMessage("generic from label") + " " + (pm.FromUserDisplayName + "<br />"));
						msgText += (string) (GetMessage("generic sent label") + " " + pm.DateCreated.ToLongDateString() + " " + pm.DateCreated.ToShortTimeString() + "<br />");
						msgText += (string) (GetMessage("generic to label") + " " + pm.GetFormattedRecipientList(";") + "<br />");
						msgText += (string) (GetMessage("generic subject label") + " " + pm.Subject);
						msgText += (string) ("</p>" + pm.Message);
						
						if (m_replying)
						{
							if (pm.FromUserDeleted == false)
							{
								msgTarget = pm.FromUserDisplayName;
								msgTargetIds = pm.FromUserID.ToString();
							}
							msgSubject = (string) (((pm.Subject.ToLower().Contains("re:")) ? "" : "re: ") + pm.Subject);
						}
						else
						{
							msgSubject = (string) (((pm.Subject.ToLower().Contains("fwd:")) ? "" : "fwd: ") + pm.Subject);
						}
					}
					cdContent_teaser.Content = msgText;
					//ftbEditor.Text = msgText
				}
				//ltrMsgView.Text += ftbEditor.ToString()
				ltrMsgTo.Text = "<input name=\"ekpmsgto" + m_uniqueId + "\" id=\"ekpmsgto" + m_uniqueId + "\" disabled=\"disabled\" value=\"" + msgTarget + "\" type=\"text\" size=\"40%\" />";
				ltrMsgSubject.Text = "<input type=\"text\" name=\"msg_subject" + m_uniqueId + "\" id=\"msg_subject" + m_uniqueId + "\" value=\"" + msgSubject + "\" size=\"40%\" />";
				litHdnToUserIds.Text = "<input type=\"hidden\" name=\"ektouserid" + m_uniqueId + "\" id=\"ektouserid" + m_uniqueId + "\" value=\"" + msgTargetIds + "\" />";
				
			}
			catch (Exception)
			{
			}
			finally
			{
			}
		}
		
		public bool SendMessage(string msgSubject, string msgText)
		{
			bool returnValue;
			bool result = true;
			CommonApi refCommonAPI = new CommonApi();
			PrivateMessage objPM = new PrivateMessage(refCommonAPI.RequestInformationRef);
			UserData[] aUsers;
			string[] userIds = null;
			int idx = 0;
			
			try
			{
				if (! (Request.Form["ektouserid" + m_uniqueId] == null))
				{
					userIds = Request.Form["ektouserid" + m_uniqueId].Split(",".ToCharArray());
				}
				if ((userIds == null) || (userIds.Length == 0) || ("" == userIds[0]))
				{
					// no recipients selected!
					result = false;
				}
				else
				{
                    aUsers = (UserData[])Array.CreateInstance(typeof(Ektron.Cms.UserData), userIds.Length);
					if (0 == msgSubject.Length)
					{
						msgSubject = m_refMsg.GetMessage("lbl no subject");
					}
					objPM.Subject = msgSubject;
					objPM.Message = msgText;
					objPM.FromUserID = refCommonAPI.RequestInformationRef.UserId;
					for (idx = 0; idx <= userIds.Length - 1; idx++)
					{
						aUsers[idx] = new Ektron.Cms.UserData();
						aUsers[idx].Id = Convert.ToInt64((userIds[idx].Trim()));
					}
					objPM.Send(aUsers);
				}
				
			}
			catch (Exception)
			{
				result = false;
			}
			finally
			{
				returnValue = result;
				refCommonAPI = null;
				objPM = null;
			}
			return returnValue;
		}
		
		protected PrivateMessage GetMsg()
		{
			PrivateMessage returnValue;
			PrivateMessage result = null;
			CommonApi refCommonAPI = null;
			PrivateMessage objPM = null;
			try
			{
				if (m_replying || m_forwarding)
				{
					// recover existing message:
					refCommonAPI = new CommonApi();
					objPM = new PrivateMessage(refCommonAPI.RequestInformationRef);
					objPM.GetByMessageID(Convert.ToInt64( m_id));
					
					result = IsMyMessage(objPM);
				}
				else
				{
					// create a new message:
					result = new Ektron.Cms.Content.PrivateMessage();
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
				refCommonAPI = null;
				objPM = null;
				//aMessages = Nothing
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
				if (m_forwarding)
				{
					if ((msgs != null) && (msgs.Length > 0))
					{
						for (idx = 0; idx <= msgs.Length - 1; idx++)
						{
							if (msgs[idx].IsARecipient(Convert.ToInt64( Request.QueryString["userid"])))
							{
								result = msgs[idx];
								break;
							}
						}
					}
				}
				if (m_forwarding == false)
				{
					if ((msgs != null) && (msgs.Length > 0))
					{
						for (idx = 0; idx <= msgs.Length - 1; idx++)
						{
							if (msgs[idx].IsARecipient(m_userId))
							{
								result = msgs[idx];
								break;
							}
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
					result = new PrivateMessage();
				}
				returnValue = result;
			}
			return returnValue;
		}
		
		protected PrivateMessage IsMyMessage(PrivateMessage msg)
		{
			PrivateMessage returnValue;
			PrivateMessage result = null;
			try
			{
				
				if (msg.FromUserID == m_userId || msg.IsARecipient(m_userId))
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
					result = new Ektron.Cms.Content.PrivateMessage();
				}
				returnValue = result;
			}
			return returnValue;
		}
		
		protected string GetMessage(string resource)
		{
			return this.m_refMsg.GetMessage(resource);
		}
		private void RegisterResources()
		{
			// register JS
			Ektron.Cms.API.JS.RegisterJS(this, Ektron.Cms.API.JS.ManagedScript.EktronJFunctJS);
			Ektron.Cms.API.JS.RegisterJS(this, Ektron.Cms.API.JS.ManagedScript.EktronToolBarRollJS);
			Ektron.Cms.API.JS.RegisterJS(this, Ektron.Cms.API.JS.ManagedScript.EktronStyleHelperJS);
			Ektron.Cms.API.JS.RegisterJS(this, Ektron.Cms.API.JS.ManagedScript.EktronJS);
			Ektron.Cms.API.JS.RegisterJS(this, Ektron.Cms.API.JS.ManagedScript.EktronThickBoxJS);
			Ektron.Cms.API.JS.RegisterJS(this, Ektron.Cms.API.JS.ManagedScript.EktronWorkareaHelperJS);
			Ektron.Cms.API.JS.RegisterJS(this, Ektron.Cms.API.JS.ManagedScript.EktronWorkareaJS);
			
			// register CSS
			Ektron.Cms.API.Css.RegisterCss(this, this.m_refContentAPI.ApplicationPath + "csslib/box.css", "EktronBoxCSS");
			Ektron.Cms.API.Css.RegisterCss(this, Ektron.Cms.API.Css.ManagedStyleSheet.EktronWorkareaCss);
			Ektron.Cms.API.Css.RegisterCss(this, Ektron.Cms.API.Css.ManagedStyleSheet.EktronWorkareaIeCss, Ektron.Cms.API.Css.BrowserTarget.AllIE);
			Ektron.Cms.API.Css.RegisterCss(this, Ektron.Cms.API.Css.ManagedStyleSheet.EktronFixedPositionToolbarCss);
		}
	}
	

