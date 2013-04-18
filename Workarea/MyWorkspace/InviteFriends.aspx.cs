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
using Ektron.Cms.Workarea;

public partial class MyWorkspace_InviteFriends : workareabase
{
    protected void Page_Load1(object sender, System.EventArgs e)
    {
        base.Page_Load(sender, e);
		Utilities.ValidateUserLogin();
        if (!Ektron.Cms.DataIO.LicenseManager.LicenseManager.IsFeatureEnable(m_refContentApi.RequestInformationRef, Ektron.Cms.DataIO.LicenseManager.Feature.SocialNetworking))
        {
            Utilities.ShowError(m_refContentApi.EkMsgRef.GetMessage("feature locked error"));
        }
        string infoMsg = "";
        try
        {
            if (Page.IsPostBack)
            {
                InvitePanel.Visible = false;
                InivitedPanel.Visible = true;

                InvitationSendRequestData inviteData = new InvitationSendRequestData();
                string emailText = (string)((!(Page.Request.Form["inv1_inviteEmailAddress"] == null)) ? (Page.Request.Form["inv1_inviteEmailAddress"]) : string.Empty);
                string[] emails;
                int idx;
                bool missingInviteUrl = false; // As String = "http://www.msn.com"
                System.Collections.ObjectModel.Collection<Ektron.Cms.Community.Invitation> inviteResult = null;

                Ektron.Cms.Community.FriendsAPI friendsApi = null;
                string inviteMessageSubject = GetMessage("lbl invitation to website");
                string msgText = (string)((!(Page.Request.Form["inv1_inviteMessage"] == null)) ? (Page.Request.Form["inv1_inviteMessage"]) : string.Empty);
                string errorStr = "";

                if (emailText.Length > 0)
                {
                    emailText = emailText.Replace(ControlChars.CrLf, ";");
                    emailText = emailText.Replace(ControlChars.NewLine, ";");
                    emailText = emailText.Replace(ControlChars.Cr, ';');
                    emailText = emailText.Replace(ControlChars.Lf, ';');
                    emailText = emailText.Replace(ControlChars.Tab, ';');
                    emailText = emailText.Replace(ControlChars.Quote, ';');
                    emailText = (string)(emailText.Replace(" ", ";").Replace(",", ";").Replace("|", ";"));
                    emails = emailText.Split(";".ToCharArray());

                    for (idx = 0; idx <= emails.Length - 1; idx++)
                    {
                        if (emails[idx].Trim().Length > 0)
                        {
                            inviteData.EmailAddresses.Add(emails[idx]);
                        }
                    }
                }
                if (inviteData.EmailAddresses.Count > 0)
                {
                    if (missingInviteUrl)
                    {
                        infoMsg = "Error in invitation system: Server control property InviteLinkUrl is not set!";
                        inviteResult = new System.Collections.ObjectModel.Collection<Ektron.Cms.Community.Invitation>();
                    }
                    else
                    {
                        friendsApi = new Ektron.Cms.Community.FriendsAPI();
                        try
                        {

                            inviteData.OptionalText = msgText;

                            inviteData.SenderId = this.m_refContentApi.UserId;
                            inviteData.NonuserMessageId = 0;
                            inviteData.UserMessageId = 0;
                            inviteResult = friendsApi.InviteUsers(inviteData);

                            if (inviteResult.Count > 0)
                            {
                                invitedLit.Text = "";
                                int sentCount = 0;
                                System.Text.StringBuilder sb = new System.Text.StringBuilder();
                                foreach (Ektron.Cms.Community.Invitation inviteResultItem in inviteResult)
                                {
                                    sb.Append("<li>");
                                    if (inviteResultItem.Recipient != null)
                                    {
                                        sb.Append(inviteResultItem.Recipient.Email + "&#160;");
                                    }

                                    if (inviteResultItem.Errors.Count > 0)
                                    {
                                        errorStr = "";
                                        for (idx = 0; idx <= inviteResultItem.Errors.Count - 1; idx++)
                                        {
                                            errorStr += inviteResultItem.Errors[idx];
                                        }
                                        sb.Append(errorStr);
                                    }
                                    else
                                    {
                                        if (inviteResultItem.Recipient.IsUser)
                                        {
                                            sb.Append(GetMessage("lbl already a site user - sent friend request"));
                                        }
                                        else
                                        {
                                            sb.Append(GetMessage("lbl was sent an invitation"));
                                            sentCount++;
                                        }
                                    }
                                    sb.Append("</li>");
                                }
                                if (sentCount > 0)
                                {
                                    infoMsg = GetMessage("invitations were sent");
                                }
                                else
                                {
                                    infoMsg = GetMessage("lbl action completed");
                                }
                                invitedLit.Text = sb.ToString();
                                sb = null;
                            }
                            else
                            {
                                infoMsg = "Error in invitation system: Unknown cause."; //GetMessage("all recipients are members")
                            }

                        }
                        catch (Exception ex)
                        {
                            infoMsg = (string)("Error in invitation system: " + ex.Message);

                        }
                        finally
                        {
                            if (inviteResult == null)
                            {
                                inviteResult = new System.Collections.ObjectModel.Collection<Ektron.Cms.Community.Invitation>();
                            }
                        }
                    }
                }
                else
                {
                    infoMsg = GetMessage("msg email req");
                    inviteResult = new System.Collections.ObjectModel.Collection<Ektron.Cms.Community.Invitation>();
                }
            }
            else
            {
                string scriptStart = "<script type=\"text/javascript\" language=\"javascript\" >";
                string scriptEnd = "</script>";

                InvitePanel.Visible = true;
                InivitedPanel.Visible = false;
                lit_faultyEmailAddrMsg.Text = scriptStart + "faultyEmailAddrMsg = \'" + GetMessage("error: faulty email address") + "\';" + scriptEnd + Environment.NewLine;
                lit_longEmailAddrMsg.Text = scriptStart + "longEmailAddrMsg = \'" + GetMessage("error: email address long") + "\';" + scriptEnd + Environment.NewLine;
            }
            infoMsgLit.Text = infoMsg;
            SetLabels();
        }
        catch (Exception ex)
        {
            Utilities.ShowError(ex.Message + ex.StackTrace);
        }

    }

    public void SetLabels()
    {
        this.AddBackButton("MyFriends.aspx");
        this.AddHelpButton("invitenewfriends");
        this.SetTitleBarToMessage("lbl invite new friends");
    }
}
