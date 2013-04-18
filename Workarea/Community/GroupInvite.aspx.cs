using System;
using System.Web.UI;
//using System.DateTime;
using Ektron.Cms;
using Ektron.Cms.Community;
using Ektron.Cms.Framework.UI;
//using Ektron.Cms.Common.EkFunctions;
//using Ektron.Cms.CustomFieldsApi;
//using Ektron.Cms.Common.EkConstants;
//using Ektron.Cms.Common.EkEnumeration;

	public partial class Community_GroupInvite : System.Web.UI.Page
	{
		protected long m_userId = 0;
		protected long m_groupId = 0;
		protected UserAPI m_refUserApi = new UserAPI();
		protected ContentAPI m_refContentAPI = new ContentAPI();
		protected Ektron.Cms.Community.CommunityGroup m_refCommunityGroup;
		protected bool _isGroupMember = false;
		
		protected void Page_Load(object sender, System.EventArgs e)
		{
			m_userId = m_refContentAPI.RequestInformationRef.UserId;
			if (! Utilities.ValidateUserLogin())
			{
				return;
			}
			
			if ((! (Request.QueryString["groupid"] == null) ) && Ektron.Cms.Common.EkFunctions.IsNumeric(Request.QueryString["groupid"]))
			{
				m_groupId = long.Parse(Request.QueryString["groupid"]);
			}
			m_refCommunityGroup = new Ektron.Cms.Community.CommunityGroup(m_refContentAPI.RequestInformationRef);
			if (m_groupId != 0)
			{
				_isGroupMember = m_refCommunityGroup.IsGroupUser(m_groupId, m_userId);
			}
			if (! m_refContentAPI.IsAdmin() && ! _isGroupMember && ! m_refContentAPI.IsARoleMember(Ektron.Cms.Common.EkEnumeration.CmsRoleIds.CommunityGroupAdmin))
			{
				Response.Redirect(m_refContentAPI.ApplicationPath + "reterror.aspx?info=" + m_refContentAPI.EkMsgRef.GetMessage("msg not invite user"), true);
				return;
			}
			
			if ((! Page.IsPostBack) && (! Page.IsCallback))
			{
				SendInviteBtn.Attributes.Add("onclick", "return (GroupInvite_ValidateInvitiations(\'" + Invite_UsrSel.ControlId + "\'))");
			}

            RegisterResources();
		}

        protected void RegisterResources()
        {
            Packages.EktronCoreJS.Register(this);
        }
		
		protected void SendInviteBtn_Click(object sender, System.EventArgs e)
		{
			// Process invitations:
			string[] userIds = null;
			string[] userEmails = null;
			Ektron.Cms.Community.CommunityGroupAPI comGrpApi = null;
			Ektron.Cms.Common.InvitationSendRequestData invDat = null;
			System.Collections.ObjectModel.Collection<Ektron.Cms.Community.Invitation> inviteResult = null;
			//System.Collections.ObjectModel.Collection<Ektron.Cms.Community.Invitation> inviteResult = null;
			string itemText = "";
			string optionalText = "";
			bool sendInvites = false;
			string infoMsg = "";
			long userMessageId = 10;
			long nonuserMessageId = 12;
			
			try
			{
				Invite_UsrSel_panel.Enabled = false;
				
				comGrpApi = new Ektron.Cms.Community.CommunityGroupAPI();
				invDat = new Ektron.Cms.Common.InvitationSendRequestData();
				
				if ((! (Request.QueryString["usrmsgid"] == null) ) && Ektron.Cms.Common.EkFunctions.IsNumeric(Request.QueryString["usrmsgid"]))
				{
					userMessageId = long.Parse(Request.QueryString["usrmsgid"]);
				}
				if ((! (Request.QueryString["nusrmsgid"] == null) ) && Ektron.Cms.Common.EkFunctions.IsNumeric(Request.QueryString["nusrmsgid"]))
				{
					nonuserMessageId = long.Parse(Request.QueryString["nusrmsgid"]);
				}
				
				if (! (Request.Form["GroupInvite_UserIds"] == null))
				{
					userIds = Request.Form["GroupInvite_UserIds"].Trim().Split(",".ToCharArray());
				}
				
				if (! (Request.Form["GroupInviteOptionalText"] == null))
				{
					optionalText = (string) (Request.Form["GroupInviteOptionalText"].Trim());
				}
				
				if (! (Request.Form["GroupInvite_Emails"] == null))
				{
					string rawEmails = (string) (Request.Form["GroupInvite_Emails"].Trim());
					rawEmails = rawEmails.Replace(Environment.NewLine, ";");
					rawEmails = rawEmails.Replace("\'", ";");
					rawEmails = rawEmails.Replace("\"", ";");
					rawEmails = (string) (rawEmails.Replace(" ", ";").Replace(",", ";").Replace("|", ";"));
					userEmails = rawEmails.Split(";".ToCharArray());
				}
				
				if ((m_userId > 0) && (m_groupId > 0))
				{
					invDat.SenderId = m_userId;
					invDat.OptionalText = optionalText;
					invDat.UserMessageId = userMessageId;
					invDat.NonuserMessageId = nonuserMessageId;
					
					foreach (string tempLoopVar_itemText in userIds)
					{
						itemText = tempLoopVar_itemText;
						if (Ektron.Cms.Common.EkFunctions.IsNumeric(itemText))
						{
							invDat.UserIds.Add(long.Parse(itemText));
							sendInvites = true;
						}
					}
					
					foreach (string tempLoopVar_itemText in userEmails)
					{
						itemText = tempLoopVar_itemText;
						if (itemText.Trim().Length > 0)
						{
							invDat.EmailAddresses.Add(itemText.Trim());
							sendInvites = true;
						}
					}
					
					if (sendInvites)
					{
						inviteResult = comGrpApi.InviteUsers(invDat, m_groupId);
						if (inviteResult.Count > 0)
						{
							
							int sentCount = 0;
							
							foreach (Ektron.Cms.Community.Invitation inviteResultItem in inviteResult)
							{
								if (0 == inviteResultItem.Errors.Count)
								{
									if (! inviteResultItem.Recipient.IsUser)
									{
										sentCount++;
									}
								}
							}
							
							if (sentCount > 0)
							{
								infoMsg = GetMessage("invitations were sent");
							}
							else
							{
								infoMsg = GetMessage("lbl action completed");
							}
							
						}
						else
						{
							infoMsg = "There was a problem sending the invitation: Unknown cause";
						}
					}
				}
				
			}
			catch (Exception ex)
			{
				infoMsg = (string) ("There was a problem sending the invitation: " + ex.Message);
			}
			finally
			{
				BuildResultUI(infoMsg, inviteResult);
				comGrpApi = null;
			}
		}
		
		protected void BuildResultUI(string msg, System.Collections.ObjectModel.Collection<Invitation> inviteResult)
		{
			System.Text.StringBuilder sb = new System.Text.StringBuilder();
			int idx = 0;
			string uiStr = msg;
			string errorStr = "";
			
			try
			{
				GroupInviteStartupUiPanel.Visible = false;
				GroupInviteResultUiPanel.Visible = true;
				
				sb.Append("<ul>" + Environment.NewLine);
				foreach (Ektron.Cms.Community.Invitation inviteResultItem in inviteResult)
				{
					sb.Append(" <li>" + Environment.NewLine);
					if (inviteResultItem.Recipient != null)
					{
						if (inviteResultItem.Recipient.IsUser)
						{
							sb.Append(inviteResultItem.Recipient.DisplayName);
						}
						else
						{
							sb.Append((inviteResultItem.Recipient.Email != null) ? inviteResultItem.Recipient.Email : "");
						}
					}
					if (inviteResultItem.Errors.Count > 0)
					{
						errorStr = "";
						for (idx = 0; idx <= inviteResultItem.Errors.Count - 1; idx++)
						{
							errorStr += inviteResultItem.Errors[idx];
						}
						if (errorStr.Length > 0)
						{
							sb.Append("<span class=\"ekError\">" + errorStr + "</span>");
						}
					}
					if (inviteResultItem.Recipient != null)
					{
						if (inviteResultItem.Recipient.IsUser)
						{
							sb.Append("&#160;" + GetMessage("lbl a site user - sent group request") + Environment.NewLine);
						}
						else
						{
							sb.Append("&#160;" + GetMessage("lbl was sent an invitation") + Environment.NewLine);
						}
					}
					
					sb.Append(" </li>" + Environment.NewLine);
				}
				sb.Append("</ul>" + Environment.NewLine);
				uiStr += (string) ("<br />" + sb.ToString());
				
			}
			catch (Exception)
			{
				
			}
			finally
			{
				GroupInviteResults.Text = uiStr;
				sb = null;
			}
		}
		
		protected string GetMessage(string resource)
		{
			return m_refContentAPI.EkMsgRef.GetMessage(resource);
		}
		
	}
