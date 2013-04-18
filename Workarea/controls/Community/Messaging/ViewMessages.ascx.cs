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

	public partial class Messaging_ViewMessages : System.Web.UI.UserControl
	{
		
		
		protected bool _SentMode = false;
		protected StyleHelper m_refStyle = new StyleHelper();
		protected EkMessageHelper m_refMsg;
		protected string m_mode = "";
		protected long m_userId = 0;
		protected string m_action = "";
		
		protected void Page_Load(object sender, System.EventArgs e)
		{
			CommonApi refCommonAPI = new CommonApi();
			m_userId = refCommonAPI.RequestInformationRef.UserId;
			m_refMsg = (new CommonApi()).EkMsgRef;
			RegisterResources();
			if (! (Request.QueryString["mode"] == null))
			{
				m_mode = (string) (Request.QueryString["mode"].ToLower());
			}
			if (! (Request.QueryString["action"] == null))
			{
				m_action = (string) (Request.QueryString["action"].ToLower());
			}
			
			if (("del" == m_mode) && (! (Request.Form["MsgInboxSelCBHdn"] == null) ) && (Request.Form["MsgInboxSelCBHdn"].Trim().Length > 0))
			{
				string[] sDelList = Request.Form["MsgInboxSelCBHdn"].Trim().Split(",".ToCharArray());
				int idx;
                long[] delList = new long[sDelList.Length];
                //long[] delList = Array.CreateInstance(typeof(long), sDelList.Length);
				for (idx = 0; idx <= sDelList.Length - 1; idx++)
				{
					if (Ektron.Cms.Common.EkFunctions.IsNumeric(sDelList[idx]))
					{
						delList.SetValue(Convert.ToInt64(sDelList[idx]), idx);
					}
				}
				PrivateMessage objPM = new PrivateMessage(refCommonAPI.RequestInformationRef);
				objPM.DeleteMessageList(delList, _SentMode);
				refCommonAPI = null;
				objPM = null;
				Response.ClearContent();
				if (m_action.Length == 0)
				{
					m_action = "viewall";
				}
				Response.Redirect((string) ("CommunityMessaging.aspx?action=" + m_action), false);
			}
			else
			{
				LoadToolBar();
				LoadGrid();
			}
		}
		
		public bool SentMode
		{
			get
			{
				return _SentMode;
			}
			set
			{
				_SentMode = value;
			}
		}
		
		protected void LoadToolBar()
		{
			UserAPI m_refUserApi = new UserAPI();
			string AppImgPath = m_refUserApi.AppImgPath;
			System.Text.StringBuilder result = new System.Text.StringBuilder();
			string helpBtnText = string.Empty;
			try
			{
				
				divTitleBar.InnerHtml = m_refStyle.GetTitleBar((string) (SentMode ? (GetMessage("lbl sent messages")) : (GetMessage("lbl inbox"))));
				result.Append("<table ><tr>");
				if (Request.QueryString["action"] == "viewallsent")
				{
					result.Append(m_refStyle.GetButtonEventsWCaption(AppImgPath + "../UI/Icons/back.png", "CommunityMessaging.aspx?action=viewall", m_refMsg.GetMessage("alt back button text"), m_refMsg.GetMessage("btn back"), "", StyleHelper.BackButtonCssClass, true)); 
					result.Append(m_refStyle.GetButtonEventsWCaption(AppImgPath + "../UI/Icons/emailDelete.png", "javascript:DelSelMsgs(" + (SentMode ? "true" : "false") + ");", m_refMsg.GetMessage("lbl del sel"), m_refMsg.GetMessage("lbl del sel"), "", StyleHelper.DeleteEmailButtonCssClass, true));
				}
				else
				{
					result.Append(m_refStyle.GetButtonEventsWCaption(AppImgPath + "../UI/Icons/emailAdd.png", "CommunityMessaging.aspx?action=editmsg", GetMessage("lbl compose a message"), GetMessage("lbl compose a message"), "", StyleHelper.AddEmailButtonCssClass, true));
					result.Append(m_refStyle.GetButtonEventsWCaption(AppImgPath + "../UI/Icons/emailDelete.png", "javascript:DelSelMsgs(" + (SentMode ? "true" : "false") + ");", m_refMsg.GetMessage("lbl del sel"), m_refMsg.GetMessage("lbl del sel"), "", StyleHelper.DeleteEmailButtonCssClass));
					result.Append(m_refStyle.GetButtonEventsWCaption(AppImgPath + "../UI/Icons/emailSent.png", "CommunityMessaging.aspx?action=viewallsent", m_refMsg.GetMessage("lbl sent messages"), m_refMsg.GetMessage("lbl sent messages"), "", StyleHelper.SentEmailsButtonCssClass));
					helpBtnText = "messaging_inbox";
				}
				result.Append(StyleHelper.ActionBarDivider);
				result.Append("<td>");
				result.Append(m_refStyle.GetHelpButton(helpBtnText, ""));
				result.Append("</td>");
				result.Append("</tr></table>");
				
				divToolBar.InnerHtml = result.ToString();
				result = null;
			}
			catch (Exception ex)
			{
				Utilities.ShowError(ex.Message);
			}
		}
		
		protected void LoadGrid()
		{
			System.Web.UI.WebControls.BoundColumn cb = new System.Web.UI.WebControls.BoundColumn();
			cb.DataField = "fDelete";
			cb.HeaderText = "<input type=\"checkbox\" onclick=\"MsgInboxToggleAllCB(this);\" name=\"MsgInboxMasterCB\" value=\"" + "ID" + "\" runat=\"Server\"/>";
			cb.Initialize();
			cb.HeaderStyle.CssClass = "center checkBoxColumn";
			cb.ItemStyle.CssClass = "center checkBoxColumn";
			_dg.Columns.Add(cb);
			
			cb = new System.Web.UI.WebControls.BoundColumn();
			cb.DataField = "fFrom";
			if (SentMode)
			{
				cb.HeaderText = GetMessage("lbl generic to");
			}
			else
			{
				cb.HeaderText = GetMessage("lbl generic from");
			}
			cb.Initialize();
			_dg.Columns.Add(cb);
			
			cb = new System.Web.UI.WebControls.BoundColumn();
			cb.DataField = "fSubject";
			cb.HeaderText = GetMessage("lbl generic subject");
			cb.Initialize();
			_dg.Columns.Add(cb);
			
			cb = new System.Web.UI.WebControls.BoundColumn();
			cb.DataField = "fDate";
			cb.HeaderText = GetMessage("lbl generic date");
			cb.Initialize();
			_dg.Columns.Add(cb);
			
			_dg.DataSource = CreateMsgData();
			_dg.DataBind();
		}
		
		protected ICollection CreateMsgData()
		{
			ICollection returnValue;
			DataTable dt = new DataTable();
			DataRow dr;
			string ListCheckboxes = "";
			string Name = "";
			string msgUserId = "";
			
			try
			{
				// header:
				dt.Columns.Add(new DataColumn("fDelete", typeof(string)));
				dt.Columns.Add(new DataColumn("fFrom", typeof(string)));
				dt.Columns.Add(new DataColumn("fSubject", typeof(string)));
				dt.Columns.Add(new DataColumn("fDate", typeof(string)));
				
				// data:
				CommonApi m_refCommonAPI = new CommonApi();
				PrivateMessage objPM = new PrivateMessage(m_refCommonAPI.RequestInformationRef);
                PrivateMessage[] aMessages = new PrivateMessage[0];

				//PrivateMessage[] aMessages = Array.CreateInstance(typeof(Ektron.Cms.Content.PrivateMessage), 0);
				aMessages = objPM.GetMessagesForMe(SentMode ? 1 : 0, 0); // inbox=0/sent=1.
				int idx = 0;
				for (idx = 0; idx <= aMessages.Length - 1; idx++)
				{
					dr = dt.NewRow();
					Name = (string) ("MsgInboxCB_" + idx.ToString());
					if (ListCheckboxes.Length > 0)
					{
						ListCheckboxes = ListCheckboxes + "," + Name;
					}
					else
					{
						ListCheckboxes = Name;
					}
					dr[0] = "<input type=\"checkbox\" onclick=\"MsgInboxToggleCB(this);\" className=\"inboxRowCB\" name=\"" + Name + "\" value=\"" + aMessages[idx].ID.ToString() + "\" runat=\"Server\"/>";
					if (SentMode)
					{
						dr[1] = aMessages[idx].GetFormattedRecipientList(";");
					}
					else
					{
						dr[1] = aMessages[idx].FromUserDisplayName;
					}
					// msgUserId = aMessages(idx).ToUserID.ToString()
					dr[2] = "<a href=\"CommunityMessaging.aspx?action=" + (SentMode ? "viewsentmsg" : "viewmsg") + "&id=" + aMessages[idx].ID.ToString() + "&userid=" + msgUserId + "\" target=\"_self\" >" + FormatByStatus(ref aMessages[idx], SentMode) + "</a>";
					dr[3] = aMessages[idx].DateCreated;
					dt.Rows.Add(dr);
				}
				
			}
			catch (Exception)
			{
			}
			finally
			{
				returnValue = new DataView(dt);
			}
			return returnValue;
		}
		
		protected string FormatByStatus(ref PrivateMessage msg, bool sendingMode)
		{
			string result = LimitSubjectLength(msg.Subject);
			
			if (! sendingMode)
			{
				if (HasRead(msg))
				{
					result = "<i>" + result + "</i>";
				}
				else
				{
					result = "<b>" + result + "</b><img src=\'images\\UI\\Icons\\email.png\' style=\'margin-left: 15px;\' />";
				}
			}
			
			return result;
		}
		
		protected bool HasRead(PrivateMessage msg)
		{
			bool result = false;
			foreach (Ektron.Cms.PrivateMessageRecipientData target in msg.Recipients)
			{
				if (target.ToUserID == this.m_userId)
				{
					result = target.Read;
					break;
				}
			}
			return result;
		}
		
		protected string GetMessage(string resource)
		{
			return this.m_refMsg.GetMessage(resource);
		}
		
		protected string LimitSubjectLength(string msg)
		{
			string result = msg;
			int clipLength = 45;
			if (result.Length > clipLength)
			{
				result = (string) (result.Substring(0, clipLength - 3) + "...");
			}
			
			return result;
		}
		private void RegisterResources()
		{
			Ektron.Cms.API.JS.RegisterJS(this, Ektron.Cms.API.JS.ManagedScript.EktronJFunctJS);
			Ektron.Cms.API.JS.RegisterJS(this, Ektron.Cms.API.JS.ManagedScript.EktronToolBarRollJS);
			Ektron.Cms.API.JS.RegisterJS(this, Ektron.Cms.API.JS.ManagedScript.EktronStyleHelperJS);
			Ektron.Cms.API.JS.RegisterJS(this, Ektron.Cms.API.JS.ManagedScript.EktronWorkareaHelperJS);
			Ektron.Cms.API.Css.RegisterCss(this, Ektron.Cms.API.Css.ManagedStyleSheet.EktronWorkareaCss);
			Ektron.Cms.API.Css.RegisterCss(this, Ektron.Cms.API.Css.ManagedStyleSheet.EktronWorkareaIeCss, Ektron.Cms.API.Css.BrowserTarget.AllIE);
		}
	}