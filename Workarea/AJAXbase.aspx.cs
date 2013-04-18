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
using Ektron.Cms.Content;
using Ektron.Cms.Common;

	public partial class AJAXbase : System.Web.UI.Page
	{
		private string m_sAction = "";
		private StringBuilder m_sbResponse = new StringBuilder();
		
		private void Page_Load(System.Object sender, System.EventArgs e)
		{
			try
			{
				if (Request.QueryString["action"] != "")
				{
					m_sAction = EkFunctions.HtmlEncode((string) (Request.QueryString["action"].ToLower()));
				}
				
				if (m_sAction.ToLower() == "getcontenttemplates")
				{
					//we return pre formtted html for get content template
				}
				else
				{
					Response.ContentType = "text/xml";
					Response.CacheControl = "no-cache";
					Response.AddHeader("Pragma", "no-cache");
					Response.Expires = -1;
					
					m_sbResponse.Append("<?xml version=\"1.0\" encoding=\"UTF-8\" standalone=\"yes\"?>").Append(Environment.NewLine);
					m_sbResponse.Append("<response>").Append(Environment.NewLine);
				}
				
				switch (this.m_sAction)
				{
					case "existinguser":
						m_sbResponse.Append(Handler_ExistingUser());
						break;
					case "existingfolder":
						m_sbResponse.Append(Handler_ExistingFolder());
						break;
					case "existingrule":
						m_sbResponse.Append(Handler_ExistingRule());
						break;
					case "existingruleset":
						m_sbResponse.Append(Handler_ExistingRuleset());
						break;
					case "existinguserrank":
						m_sbResponse.Append(Handler_ExistingUserRank());
						break;
                    case "addeditcontentrating": 
						m_sbResponse.Append(Handler_AddEditContentRating());
						break;
                    case "addeditcontentflag": 
						m_sbResponse.Append(Handler_AddEditContentFlag());
						break;
					case "addpersonalfolder":
						m_sbResponse.Append(Handler_AddPersonalFolder());
						break;
					case "removeitems":
						m_sbResponse.Append(Handler_RemoveItems());
						break;
					case "addfavorite":
						m_sbResponse.Append(Handler_AddFavorite());
						break;
					case "addfriend":
						m_sbResponse.Append(Handler_AddFriend());
						break;
					case "addto": 
						m_sbResponse.Append(Handler_AddTo());
						break;
					case "shareuserblog":
						m_sbResponse.Append(Hander_ShareUserBlog());
						break;
					case "getcontenttemplates":
						m_sbResponse.Append(Handler_GetContentTemplates());
						break;
				}
				
				
				if (m_sAction.ToLower() != "getcontenttemplates")
				{
					m_sbResponse.Append("</response>").Append(Environment.NewLine);
				}
				
				Response.Write(m_sbResponse.ToString());
			}
			catch (Exception)
			{
				
			}
		}
		
		#region Handlers
		public string Handler_GetContentTemplates()
		{
			long id = 0;
			StringBuilder sbRet = new StringBuilder();
			int i = 0;
			Ektron.Cms.UrlAliasing.UrlAliasManualApi m_aliasAPI = new Ektron.Cms.UrlAliasing.UrlAliasManualApi();
			System.Collections.Generic.List<LibraryData> alias_templates;
			ContentAPI apiContent = new ContentAPI();
			string workareaDir = string.Empty;
			long.TryParse(Request.QueryString["id"], out id);
			alias_templates = m_aliasAPI.GetTemplateList(id);
			workareaDir = apiContent.RequestInformationRef.WorkAreaDir;
			
			for (i = 0; i <= alias_templates.Count - 1; i++)
			{
				if (alias_templates[i].FileName.ToLower().IndexOf("downloadasset.aspx?") != -1)
				{
					sbRet.Append("<option value=\"").Append(alias_templates[i].Id).Append("\">").Append(workareaDir + alias_templates[i].FileName).Append("</option>");
				}
				else
				{
                    sbRet.Append("<option value=\"").Append(alias_templates[i].Id).Append("\">").Append(alias_templates[i].FileName).Append("</option>");
				}
			}
			return sbRet.ToString();
		}
		public string Hander_ShareUserBlog()
		{
			StringBuilder resp = new StringBuilder("<method>AJAX_ShareUserBlog</method>");
			ContentAPI apiContent = new ContentAPI();
			long BlogId;
			int shareOption;
			try
			{
				if (Request.QueryString["BlogId"] != "")
				{
					BlogId = Convert.ToInt64(Request.QueryString["BlogId"]);
					shareOption = Convert.ToInt32(Request.QueryString["shareoption"]);
					System.Collections.Generic.List<string> list = new System.Collections.Generic.List<string>();
					list.Add(BlogId.ToString() + "," + shareOption.ToString());
					apiContent.SetWorkSpaceShare(apiContent.RequestInformationRef.UserId, Ektron.Cms.Common.EkEnumeration.WorkSpace.User, list);
				}
			}
			catch (Exception ex)
			{
				resp.Append("<result>-1</result>");
				resp.Append("<returnmsg>" + ex.Message.ToString() + "</returnmsg>");
			}
			return resp.ToString();
		}
		
		public string Handler_AddTo()
		{
			StringBuilder sbRet = new StringBuilder();
			ContentAPI apiContent = new ContentAPI();
			Ektron.Cms.Community.CommunityGroupAPI apiCommunityGroup = new Ektron.Cms.Community.CommunityGroupAPI();
			Ektron.Cms.Community.FavoritesAPI apiFavorites = new Ektron.Cms.Community.FavoritesAPI();
			Ektron.Cms.Community.FriendsAPI apiFriends = new Ektron.Cms.Community.FriendsAPI();
			long iObjID = 0;
			Ektron.Cms.Common.EkEnumeration.CMSSocialBarTypes sbObjType = Ektron.Cms.Common.EkEnumeration.CMSSocialBarTypes.Content;
			string sMode = "";
			string sMsg = "";
			string sKey = "";
			string sResult = "";
			int iIdx = 0;
			bool bIsMine = false;
			long bIsMyLinkID = 0;
			string sRetMsg = "";
			bool bAuth = false;
			int iLang = 0;
			string title = "";
            string link = "";
            string action = "";
			try
			{
				iObjID = Convert.ToInt64(Request.QueryString["oid"]);
                int oType = 0;
                int.TryParse(EkFunctions.HtmlEncode(Request.QueryString["otype"]), out oType);
                switch (oType)
                {
                    case 0:
                        sbObjType = Ektron.Cms.Common.EkEnumeration.CMSSocialBarTypes.Content;
                        break;
                    case 1:
                        sbObjType = Ektron.Cms.Common.EkEnumeration.CMSSocialBarTypes.User;
                        break;
                    case 19:
                        sbObjType = Ektron.Cms.Common.EkEnumeration.CMSSocialBarTypes.CommunityGroup;
                        break;
                }
                
				sMode = EkFunctions.HtmlEncode(Request.QueryString["mode"]);
				sKey = EkFunctions.HtmlEncode(Request.QueryString["key"]);
				iIdx = Convert.ToInt32(Request.QueryString["idx"]);
				title = EkFunctions.HtmlEncode(Request.QueryString["title"]);
				link = EkFunctions.HtmlEncode(Request.QueryString["link"]);
				
				if (iObjID > 0)
				{
					sbRet.Append("  <method>AJAX_AddTo</method>").Append(Environment.NewLine);
				}
				else if (iObjID == 0)
				{
					sbRet.Append("  <method>AJAX_AddLinkTo</method>").Append(Environment.NewLine);
				}
				
				bAuth = System.Convert.ToBoolean((apiContent.LoadPermissions(0, "users", 0)).IsLoggedIn);
				if (bAuth)
				{
					if (sbObjType == Ektron.Cms.Common.EkEnumeration.CMSSocialBarTypes.CommunityGroup)
					{
						Ektron.Cms.Common.EkEnumeration.GroupMemberStatus mMemberStatus = Ektron.Cms.Common.EkEnumeration.GroupMemberStatus.NotInGroup;
						mMemberStatus = apiCommunityGroup.GetGroupMemberStatus(iObjID, apiContent.UserId);
						if (mMemberStatus == Ektron.Cms.Common.EkEnumeration.GroupMemberStatus.Approved)
						{
							if (sMode == "remove")
							{
								apiCommunityGroup.RemoveUserFromCommunityGroup(iObjID, apiContent.UserId);
								Ektron.Cms.Common.Cache.ApplicationCache.Invalidate((string) ("GroupAccess_" + iObjID.ToString() + "_" + apiContent.UserId.ToString()));
								sMsg = apiContent.EkMsgRef.GetMessage("lbl left group");
								sRetMsg = apiContent.EkMsgRef.GetMessage("lbl join group");
								sResult = "1";
							}
							else if (sMode == "add")
							{
								sMsg = apiContent.EkMsgRef.GetMessage("lbl already in group");
								sRetMsg = apiContent.EkMsgRef.GetMessage("lbl leave group");
								sResult = "-1";
							}
						}
						else if (mMemberStatus == Ektron.Cms.Common.EkEnumeration.GroupMemberStatus.Leader)
						{
							sMsg = apiContent.EkMsgRef.GetMessage("lbl leader of group");
							sRetMsg = apiContent.EkMsgRef.GetMessage("lbl leader of group");
							sResult = "-1";
						}
						else if (mMemberStatus == Ektron.Cms.Common.EkEnumeration.GroupMemberStatus.NotInGroup)
						{
							if (sMode == "remove")
							{
								sMsg = apiContent.EkMsgRef.GetMessage("lbl not in group");
								sRetMsg = apiContent.EkMsgRef.GetMessage("lbl join group");
								sResult = "-1";
							}
							else if (sMode == "add")
							{
								apiCommunityGroup.AddUserToCommunityGroup(iObjID, apiContent.UserId);
								Ektron.Cms.Common.Cache.ApplicationCache.Invalidate((string) ("GroupAccess_" + iObjID.ToString() + "_" + apiContent.UserId.ToString()));
								mMemberStatus = apiCommunityGroup.GetGroupMemberStatus(iObjID, apiContent.UserId);
								if (mMemberStatus == Ektron.Cms.Common.EkEnumeration.GroupMemberStatus.Pending)
								{
									sRetMsg = apiContent.EkMsgRef.GetMessage("lbl cgroup cancel join req");
									sMsg = apiContent.EkMsgRef.GetMessage("lbl leave group");
									sResult = "0";
								}
								else
								{
									sRetMsg = apiContent.EkMsgRef.GetMessage("lbl leave group");
									sMsg = apiContent.EkMsgRef.GetMessage("lbl joined group");
									sResult = "0";
								}
							}
						}
						else if (mMemberStatus == Ektron.Cms.Common.EkEnumeration.GroupMemberStatus.Pending)
						{
							if (sMode == "remove")
							{
								apiCommunityGroup.CancelJoinRequestForCommunityGroup(iObjID, apiContent.UserId);
								Ektron.Cms.Common.Cache.ApplicationCache.Invalidate((string) ("GroupAccess_" + iObjID.ToString() + "_" + apiContent.UserId.ToString()));
								sMsg = apiContent.EkMsgRef.GetMessage("lbl cancel group join");
								sRetMsg = apiContent.EkMsgRef.GetMessage("lbl join group");
								sResult = "1";
							}
							else if (sMode == "add")
							{
								sMsg = apiContent.EkMsgRef.GetMessage("lbl requested join group");
								sRetMsg = apiContent.EkMsgRef.GetMessage("lbl cgroup cancel join req");
								sResult = "-1";
							}
						}
						else if (mMemberStatus == Ektron.Cms.Common.EkEnumeration.GroupMemberStatus.InvitedMember)
						{
							apiCommunityGroup.AddUserToCommunityGroup(iObjID, apiContent.UserId);
							Ektron.Cms.Common.Cache.ApplicationCache.Invalidate((string) ("GroupAccess_" + iObjID.ToString() + "_" + apiContent.UserId.ToString()));
							mMemberStatus = apiCommunityGroup.GetGroupMemberStatus(iObjID, apiContent.UserId);
							if (mMemberStatus == Ektron.Cms.Common.EkEnumeration.GroupMemberStatus.Pending)
							{
								sRetMsg = apiContent.EkMsgRef.GetMessage("lbl cgroup cancel join req");
								sMsg = apiContent.EkMsgRef.GetMessage("lbl leave group");
								sResult = "0";
							}
							else
							{
								sRetMsg = apiContent.EkMsgRef.GetMessage("lbl leave group");
								sMsg = apiContent.EkMsgRef.GetMessage("lbl cgrp accept inv");
								sResult = "0";
							}
						}
					}
					else if (sbObjType == Ektron.Cms.Common.EkEnumeration.CMSSocialBarTypes.Content)
					{
						if (Request.QueryString["lang"] != "" && Information.IsNumeric(Request.QueryString["lang"]))
						{
							iLang = Convert.ToInt32(Request.QueryString["lang"]);
						}
						if (iLang > 0)
						{
							apiFavorites.ContentLanguage = iLang;
						}
						if (iObjID > 0)
						{
							bIsMine = apiFavorites.IsMyContentFavorite(iObjID);
						}
						else
						{
							bIsMyLinkID = apiFavorites.GetFavoriteId(title, link, apiContent.UserId);
						}
						
						
						if (sMode == "remove" && bIsMine)
						{
							apiFavorites.DeleteMyContentFavorite(iObjID);
							sMsg = apiContent.EkMsgRef.GetMessage("lbl no longer fav");
							sRetMsg = apiContent.EkMsgRef.GetMessage("lbl add fav");
							sResult = "1";
                            action = "addFavorite";
						}
						else if (sMode == "remove" && ! bIsMine)
						{
							sMsg = apiContent.EkMsgRef.GetMessage("lbl not fav");
							sRetMsg = apiContent.EkMsgRef.GetMessage("lbl add fav");
							sResult = "-1";
						}
						else if (sMode == "add" && bIsMine)
						{
							sMsg = apiContent.EkMsgRef.GetMessage("lbl already fav");
							sRetMsg = apiContent.EkMsgRef.GetMessage("lbl remove fav");
							sResult = "-1";
						}
						else if (sMode == "add" && ! bIsMine)
						{
							apiFavorites.AddContentFavorite(iObjID);
							sMsg = apiContent.EkMsgRef.GetMessage("lbl now fav");
							sRetMsg = apiContent.EkMsgRef.GetMessage("lbl remove fav");
							sResult = "0";
                            action = "removeFavorite";
						}
						else if (sMode == "addlink" && bIsMyLinkID == 0)
						{
							if (title != "")
							{
								apiFavorites.AddFavoriteLink(apiContent.UserId, 0, iLang, title, link, "");
							}
							sMsg = apiContent.EkMsgRef.GetMessage("lbl now fav");
							sRetMsg = apiContent.EkMsgRef.GetMessage("lbl remove fav");
							sResult = "0";
						}
						else if (sMode == "addlink" && bIsMyLinkID > 0)
						{
							sMsg = apiContent.EkMsgRef.GetMessage("lbl already fav");
							sRetMsg = apiContent.EkMsgRef.GetMessage("lbl remove fav");
							sResult = "-1";
						}
						else if (sMode == "removelink" && bIsMyLinkID > 0)
						{
							apiFavorites.DeleteFavoriteLink(bIsMyLinkID);
							sMsg = apiContent.EkMsgRef.GetMessage("lbl no longer fav");
							sRetMsg = apiContent.EkMsgRef.GetMessage("lbl add fav");
							sResult = "1";
						}
						else if (sMode == "removelink" && bIsMyLinkID == 0)
						{
							sMsg = apiContent.EkMsgRef.GetMessage("lbl not fav");
							sRetMsg = apiContent.EkMsgRef.GetMessage("lbl add fav");
							sResult = "-1";
						}
					}
					else if (sbObjType == Ektron.Cms.Common.EkEnumeration.CMSSocialBarTypes.User)
					{
						Ektron.Cms.Common.EkEnumeration.FriendStatus fFriendStatus = Ektron.Cms.Common.EkEnumeration.FriendStatus.NotFriend;
						// bIsMine = apiContent.IsMyFriend(iObjID)
						fFriendStatus = apiFriends.GetFriendStatus(iObjID, apiContent.UserId);
						if (sMode == "remove" && fFriendStatus == Ektron.Cms.Common.EkEnumeration.FriendStatus.Approved)
						{
							apiFriends.DeleteMyFriend(iObjID);
							sMsg = apiContent.EkMsgRef.GetMessage("lbl no longer friend");
							sRetMsg = apiContent.EkMsgRef.GetMessage("lbl add friend");
							sResult = "1";
						}
						else if (sMode == "remove" && fFriendStatus == Ektron.Cms.Common.EkEnumeration.FriendStatus.NotFriend)
						{
							sMsg = apiContent.EkMsgRef.GetMessage("lbl not friend");
							sRetMsg = apiContent.EkMsgRef.GetMessage("lbl add friend");
							sResult = "-1";
						}
						else if (sMode == "add" && fFriendStatus == Ektron.Cms.Common.EkEnumeration.FriendStatus.Approved)
						{
							sMsg = apiContent.EkMsgRef.GetMessage("lbl already friend");
							sRetMsg = apiContent.EkMsgRef.GetMessage("lbl remove friend");
							sResult = "-1";
						}
						else if (sMode == "add" && fFriendStatus == Ektron.Cms.Common.EkEnumeration.FriendStatus.NotFriend)
						{
							apiFriends.AddPendingFriend(iObjID);
							sMsg = apiContent.EkMsgRef.GetMessage("lbl now friend req");
							sRetMsg = apiContent.EkMsgRef.GetMessage("lbl remove friend req");
							sResult = "0";
						}
						else if (sMode == "remove" && fFriendStatus == Ektron.Cms.Common.EkEnumeration.FriendStatus.Pending)
						{
							apiFriends.DeletePendingFriendRequest(iObjID);
							sMsg = apiContent.EkMsgRef.GetMessage("lbl no longer pending friend");
							sRetMsg = apiContent.EkMsgRef.GetMessage("lbl add friend");
							sResult = "1";
						}
						else if (sMode == "add" && fFriendStatus == Ektron.Cms.Common.EkEnumeration.FriendStatus.Pending)
						{
							apiFriends.AcceptPendingFriend(iObjID);
							sMsg = apiContent.EkMsgRef.GetMessage("lbl already request friend");
							sRetMsg = apiContent.EkMsgRef.GetMessage("lbl remove friend");
							sResult = "0";
						}
						else if (sMode == "remove" && fFriendStatus == Ektron.Cms.Common.EkEnumeration.FriendStatus.Invited)
						{
							apiFriends.DeleteSentFriendRequest(iObjID);
							sMsg = apiContent.EkMsgRef.GetMessage("lbl no longer pending friend");
							sRetMsg = apiContent.EkMsgRef.GetMessage("lbl add friend");
							sResult = "1";
						}
						else if (sMode == "add" && fFriendStatus == Ektron.Cms.Common.EkEnumeration.FriendStatus.Invited)
						{
							sMsg = apiContent.EkMsgRef.GetMessage("lbl already request friend");
							sRetMsg = apiContent.EkMsgRef.GetMessage("lbl remove friend");
							sResult = "0";
						}
					}
				}
				else
				{
					sMsg = apiContent.EkMsgRef.GetMessage("lbl not logged in");
					sRetMsg = apiContent.EkMsgRef.GetMessage("lbl not logged in");
					sResult = "-1";
				}
				sbRet.Append("  <result>").Append(sResult).Append("</result>").Append(Environment.NewLine);
				sbRet.Append("  <returnmsg>").Append(sMsg).Append("</returnmsg>").Append(Environment.NewLine);
				sbRet.Append("  <oid>").Append(iObjID).Append("</oid>").Append(Environment.NewLine);
                sbRet.Append("  <otype>").Append(oType.ToString()).Append("</otype>").Append(Environment.NewLine);	
				sbRet.Append("  <ilang>").Append(iLang.ToString()).Append("</ilang>").Append(Environment.NewLine);
				sbRet.Append("  <idx>").Append(iIdx).Append("</idx>").Append(Environment.NewLine);
				sbRet.Append("  <retmsg>").Append(sRetMsg).Append("</retmsg>").Append(Environment.NewLine);
				sbRet.Append("  <key>").Append(sKey).Append("</key>").Append(Environment.NewLine);
                sbRet.Append("  <action>").Append(action).Append("</action>").Append(Environment.NewLine);
				if (iObjID == 0)
				{
					sbRet.Append("  <title>").Append(title.ToString()).Append("</title>").Append(Environment.NewLine);
					sbRet.Append("  <link>").Append(link).Append("</link>").Append(Environment.NewLine);
				}
			}
			catch (Exception ex)
			{
				sbRet.Append("  <method>AJAX_AddTo</method>").Append(Environment.NewLine);
				sbRet.Append("  <result>error</result>").Append(Environment.NewLine);
				sbRet.Append("  <returnmsg>").Append(ex.Message).Append("</returnmsg>").Append(Environment.NewLine);
				sbRet.Append("  <oid>").Append(iObjID).Append("</oid>").Append(Environment.NewLine);
				sbRet.Append("  <otype>").Append(sbObjType).Append("</otype>").Append(Environment.NewLine);
				sbRet.Append("  <ilang>").Append(iLang.ToString()).Append("</ilang>").Append(Environment.NewLine);
				sbRet.Append("  <idx>").Append(iIdx).Append("</idx>").Append(Environment.NewLine);
				sbRet.Append("  <retmsg>").Append(sRetMsg).Append("</retmsg>").Append(Environment.NewLine);
				sbRet.Append("  <key>").Append(sKey).Append("</key>").Append(Environment.NewLine);
			}
			return sbRet.ToString();
		}
		public string Handler_AddFavorite()
		{
			StringBuilder sbRet = new StringBuilder();
			ContentAPI apiContent = new ContentAPI();
			Ektron.Cms.Community.FavoritesAPI apiFavorites = new Ektron.Cms.Community.FavoritesAPI();
			long iID = 0;
			string sMode = "";
			string sKey = "";
			string sResult = "";
			try
			{
				iID = Convert.ToInt64(Request.QueryString["node"]);
				sMode = EkFunctions.HtmlEncode(Request.QueryString["mode"]);
				sKey = EkFunctions.HtmlEncode(Request.QueryString["key"]);
				
				sbRet.Append("  <method>AddtoFavorite</method>").Append(Environment.NewLine);
				if (sMode == "check")
				{
					if (apiFavorites.IsMyContentFavorite(iID))
					{
						sResult = Ektron.Cms.Common.EkFunctions.HtmlEncode("Favorite has been added");
					}
					else
					{
						sResult = Ektron.Cms.Common.EkFunctions.HtmlEncode("<a href=\"javascript:pdhdlr(\'addfav\', \'" + sKey + "\');\">Add to Favorites</a>");
					}
				}
				else if (apiFavorites.IsMyContentFavorite(iID))
				{
					sResult = "Already a Favorite";
				}
				else
				{
					apiFavorites.AddContentFavorite(iID);
					sResult = "Favorite has been added";
				}
				sbRet.Append("  <result>").Append(sResult).Append("</result>").Append(Environment.NewLine);
				sbRet.Append("  <key>").Append(sKey).Append("</key>").Append(Environment.NewLine);
			}
			catch (Exception)
			{
				sbRet.Append("  <method>AddtoFavorite</method>").Append(Environment.NewLine);
				sbRet.Append("  <result>-1</result>").Append(Environment.NewLine);
				sbRet.Append("  <key>").Append(sKey).Append("</key>").Append(Environment.NewLine);
			}
			return sbRet.ToString();
		}
		public string Handler_AddFriend()
		{
			StringBuilder sbRet = new StringBuilder();
			ContentAPI apiContent = new ContentAPI();
			Ektron.Cms.Community.FavoritesAPI apiFavorites = new Ektron.Cms.Community.FavoritesAPI();
			long iID = 0;
			string sMode = "";
			string sKey = "";
			string sResult = "";
			try
			{
				iID = Convert.ToInt64(Request.QueryString["node"]);
				sMode = EkFunctions.HtmlEncode(Request.QueryString["mode"]);
				sKey = EkFunctions.HtmlEncode(Request.QueryString["key"]);
				
				sbRet.Append("  <method>AddtoFriend</method>").Append(Environment.NewLine);
				if (sMode == "check")
				{
					if (apiFavorites.IsMyContentFavorite(iID))
					{
						sResult = Ektron.Cms.Common.EkFunctions.HtmlEncode("Friend has been added");
					}
					else
					{
						sResult = Ektron.Cms.Common.EkFunctions.HtmlEncode("<a href=\"javascript:pdhdlr(\'addfr\', \'" + sKey + "\');\">Add to Friends</a>");
					}
				}
				else if (apiFavorites.IsMyContentFavorite(iID))
				{
					sResult = "Already a Friend";
				}
				else
				{
					apiFavorites.AddContentFavorite(iID);
					sResult = "Friend has been added";
				}
				sbRet.Append("  <result>").Append(sResult).Append("</result>").Append(Environment.NewLine);
				sbRet.Append("  <key>").Append(sKey).Append("</key>").Append(Environment.NewLine);
			}
			catch (Exception)
			{
				sbRet.Append("  <method>AddtoFriend</method>").Append(Environment.NewLine);
				sbRet.Append("  <result>-1</result>").Append(Environment.NewLine);
				sbRet.Append("  <key>").Append(sKey).Append("</key>").Append(Environment.NewLine);
			}
			return sbRet.ToString();
		}
		public string Handler_RemoveItems()
		{
			StringBuilder sbRet = new StringBuilder();
			ContentAPI apiContent = new ContentAPI();
			string sItemList = "";
			int iRet = 0;
			string sKey = "";
			long iNode = 0;
			try
			{
				sItemList = Request.QueryString["itemlist"];
				sKey = EkFunctions.HtmlEncode(Request.QueryString["key"]);
				
				try
				{
					string[] aValues;
					aValues = sItemList.Split(',');
					if ((aValues != null)&& aValues.Length > 0)
					{
						for (int i = 0; i <= (aValues.Length - 1); i++)
						{
							if (aValues[i].ToLower().IndexOf(sKey + "_i") == 0)
							{
								TaxonomyRequest tReq = new TaxonomyRequest();
								string[] aVal = (aValues[i]).Split('_');
								int iType = 0;
								
								iNode = long.Parse(Request.QueryString["node"]);
								tReq.TaxonomyIdList = aVal[1].Substring(1);
								iType = int.Parse(aVal[2]);
								switch (iType)
								{
									case 7:
										break;
										
									default: // 1 - content
										tReq.TaxonomyItemType = (EkEnumeration.TaxonomyItemType)(Ektron.Cms.Common.EkEnumeration.CMSObjectTypes.Content);
										break;
								}
								tReq.TaxonomyId = iNode;
								tReq.TaxonomyLanguage = apiContent.ContentLanguage;
								apiContent.RemoveTaxonomyItem(tReq);
							}
							else if (aValues[i].ToLower().IndexOf(sKey + "_f") == 0)
							{
								TaxonomyRequest tReq = new TaxonomyRequest();
								iNode = int.Parse(aValues[i].Substring(((sKey + "_f").Length) + 1 - 1));
								tReq.TaxonomyId = iNode;
								tReq.TaxonomyLanguage = apiContent.ContentLanguage;
								if (iNode > 0)
								{
									apiContent.DeleteTaxonomy(tReq);
								}
							}
						}
					}
					iRet = 0;
				}
				catch (Exception ex)
				{
					EkException.ThrowException(ex);
				}
				sbRet.Append("  <method>RemoveItems</method>").Append(Environment.NewLine);
				sbRet.Append("  <result>").Append(iRet).Append("</result>").Append(Environment.NewLine);
				sbRet.Append("  <key>").Append(sKey).Append("</key>").Append(Environment.NewLine);
				return sbRet.ToString();
			}
			catch (Exception)
			{
				
			}
			return sbRet.ToString();
		}
		public string Handler_AddPersonalFolder()
		{
			StringBuilder sbRet = new StringBuilder();
			ContentAPI apiContent = new ContentAPI();
			long iNode = 0;
			int iFolder = 0;
			string sName = "";
			string sDesc = "";
			string sKey = "";
			try
			{
				iNode = Convert.ToInt32(Request.QueryString["node"]);
				if (Request.QueryString["name"] != "")
				{
					sName = Request.QueryString["name"];
				}
				sKey = EkFunctions.HtmlEncode(Request.QueryString["key"]);
				if (Request.QueryString["desc"] != "")
				{
					sDesc = Request.QueryString["desc"];
				}
				
				try
				{
					iFolder = (Int32)apiContent.AddPersonalDirectoryFolder(iNode, sName, sDesc);
				}
				catch (Exception ex)
				{
					EkException.ThrowException(ex);
				}
				sbRet.Append("  <method>AddPersonalFolder</method>").Append(Environment.NewLine);
				sbRet.Append("  <result>").Append(iFolder).Append("</result>").Append(Environment.NewLine);
				sbRet.Append("  <key>").Append(sKey).Append("</key>").Append(Environment.NewLine);
				return sbRet.ToString();
			}
			catch (Exception)
			{
				
			}
			return sbRet.ToString();
		}
		public string Handler_AddEditContentFlag()
		{
			StringBuilder sbRet = new StringBuilder();
			ContentAPI apiContent = new ContentAPI();
			Ektron.Cms.Community.FlaggingAPI apiFlagging = new Ektron.Cms.Community.FlaggingAPI();
			long iFlag = 0;
			long iContent = 0;
			int iLang = 0;
			long iret = 0;
			string sComment = "";
			string sKey = "";
			ContentFlagData cfFlag = new ContentFlagData();
			try
			{
				string visitorid;
				
				// Check and see if the cookie has been set by the user
				if (Page.Request.Cookies["ekContentRatingID"] == null && ! Page.IsPostBack)
				{
					visitorid = (string) (System.Guid.NewGuid().ToString().Replace("-", ""));
					Page.Response.Cookies.Add(new System.Web.HttpCookie("ekContentRatingID", visitorid));
					Page.Response.Cookies["ekContentRatingID"].Expires = DateTime.MaxValue;
				}
				else
				{
					try
					{
						visitorid = Page.Request.Cookies["ekContentRatingID"].Value;
					}
					catch (Exception)
					{
						visitorid = string.Empty;
					}
				}
				if (Request.QueryString["comment"] != "")
				{
					sComment = Ektron.Cms.Common.EkFunctions.HtmlEncode(Request.QueryString["comment"]);
				}
				if (Request.QueryString["lang"] != "")
				{
					iLang = Convert.ToInt32(Request.QueryString["lang"]);
				}
				iFlag = Convert.ToInt64(Request.QueryString["flag"]);
				iContent = Convert.ToInt64(Request.QueryString["contentid"]);
				sKey = EkFunctions.HtmlEncode(Request.QueryString["key"]);
				if (apiFlagging.ContentLanguage < 1 && iLang > 0)
				{
					apiFlagging.ContentLanguage = iLang;
				}
				ObjectFlagData objectFlag = new ObjectFlagData();
				ContentFlagData fdRet = null;
				try
				{
					fdRet = apiFlagging.GetContentFlagData(iContent, apiContent.UserId, visitorid);
					
					if (fdRet.EntryId == 0)
					{
						objectFlag.FlagComment = sComment;
						objectFlag.FlagItemId = iFlag;
						objectFlag.VisitorID = visitorid;
						objectFlag.UserId = apiContent.UserId;
						objectFlag.ObjectId = iContent;
						objectFlag.ObjectType = Ektron.Cms.Common.EkEnumeration.CMSObjectTypes.Content;
						objectFlag.ObjectLanguageId = iLang;
						iret = apiFlagging.AddFlagObject(objectFlag);
					}
					else
					{
						objectFlag.FlagComment = sComment;
						objectFlag.FlagItemId = iFlag;
						objectFlag.FlagId = fdRet.FlagDefinition.ID;
						objectFlag.VisitorID = fdRet.VisitorID;
						objectFlag.UserId = fdRet.FlaggedUser.Id;
						objectFlag.ObjectId = fdRet.Id;
						objectFlag.ObjectType = Ektron.Cms.Common.EkEnumeration.CMSObjectTypes.Content;
						objectFlag.ObjectLanguageId = fdRet.LanguageId;
						objectFlag.FlagEntryId = fdRet.EntryId;
						apiFlagging.UpdateFlagObject(objectFlag);
						iret = fdRet.EntryId;
					}
				}
				catch (Exception ex)
				{
					EkException.ThrowException(ex);
				}
				//cfFlag = apiContent.AddContentFlag(iContent, visitorid, iFlag, sComment)
				
				sbRet.Append("  <method>AddEditFlag</method>").Append(Environment.NewLine);
				sbRet.Append("  <result>").Append(iFlag).Append("</result>").Append(Environment.NewLine);
				sbRet.Append("  <key>").Append(sKey).Append("</key>").Append(Environment.NewLine);
				return sbRet.ToString();
			}
			catch (Exception)
			{
				
			}
			
			return sbRet.ToString();
		}
		
		public string Handler_AddEditContentRating()
		{
			StringBuilder sbRet = new StringBuilder();
			ContentAPI apiContent = new ContentAPI();
			int iRating = 0;
			long iContent = 0;
			int iret = 0;
			bool bapproved = true;
			string sReview = "";
			string skey = "";
			try
			{
				string visitorid;
				// Check and see if the cookie has been set by the user
				if (Page.Request.Cookies["ekContentRatingID"] == null && ! Page.IsPostBack)
				{
					visitorid = (string) (System.Guid.NewGuid().ToString().Replace("-", ""));
					Page.Response.Cookies.Add(new System.Web.HttpCookie("ekContentRatingID", visitorid));
					Page.Response.Cookies["ekContentRatingID"].Expires = DateTime.MaxValue;
				}
				else
				{
					try
					{
						visitorid = Page.Request.Cookies["ekContentRatingID"].Value;
					}
					catch (Exception)
					{
						visitorid = string.Empty;
					}
				}
				if (Request.QueryString["review"] != "")
				{
					sReview = EkFunctions.HtmlEncode(Request.QueryString["review"]);
				}
				if (Request.QueryString["key"] != "")
				{
					skey = EkFunctions.HtmlEncode(Request.QueryString["key"]);
				}
				iRating = Convert.ToInt32(Request.QueryString["rating"]);
				iContent = Convert.ToInt64(Request.QueryString["contentid"]);
				
				if ((Request.QueryString["LangType"] != null)&& Request.QueryString["LangType"] != "")
				{
					apiContent.ContentLanguage = Convert.ToInt32(Page.Request.QueryString["LangType"]);
				}
				else
				{
					System.Web.HttpCookie ecmCookie = Ektron.Cms.CommonApi.GetEcmCookie();
					apiContent.ContentLanguage = Convert.ToInt32(ecmCookie["SiteLanguage"].ToString());
				}
				
				bapproved = Convert.ToBoolean(Request.QueryString["approved"]);
				if (bapproved == false && sReview == "")
				{
					bapproved = true; //if there is no review text, it goes live auto.
				}
				iret = (Int32)apiContent.AddEditContentRating(iContent, visitorid, iRating, bapproved, sReview);
				sReview = HttpUtility.UrlDecode(sReview);
				
				sbRet.Append("  <method>AddEditRating</method>").Append(Environment.NewLine);
				sbRet.Append("  <result>").Append(iRating).Append("</result>").Append(Environment.NewLine);
				sbRet.Append("  <key>").Append(skey).Append("</key>").Append(Environment.NewLine);
				return sbRet.ToString();
			}
			catch (Exception)
			{
				
			}
			
			return sbRet.ToString();
		}
		
		public string Handler_ExistingUserRank()
		{
			StringBuilder sbRet = new StringBuilder();
			ContentAPI apiContent = new ContentAPI();
			Ektron.Cms.Content.EkContent brContent = new Ektron.Cms.Content.EkContent(apiContent.RequestInformationRef);
			int iRet = 0;
			long iUserRank = 0;
			int iPosts = 0;
			string sUserRank = "";
			long iboard = 0;
			bool bisstart = false;
			UserRank[] aUserRank = (UserRank[]) Array.CreateInstance(typeof(Ektron.Cms.UserRank), 0);
			
			try
			{
				iUserRank = Convert.ToInt64(Request.QueryString["urid"]);
				sUserRank = Request.QueryString["urname"];
				iboard = Convert.ToInt64(Request.QueryString["boardid"]);
				if (Request.QueryString["posts"] != "")
				{
					iPosts = Convert.ToInt32(Request.QueryString["posts"]);
				}
				if (Request.QueryString["isstart"] != "")
				{
					bisstart = Convert.ToBoolean(Request.QueryString["isstart"]);
				}
				
				aUserRank = brContent.SelectUserRankByBoard(iboard);
				
				if (aUserRank.Length > 0)
				{
					for (int i = 0; i <= (aUserRank.Length - 1); i++)
					{
						// check for existing name
						if (aUserRank[i].Name == sUserRank && !(iUserRank == aUserRank[i].ID))
						{
							iRet = 2; // name conflict
							break;
						}
						// check for number of posts
						if (iPosts > 0) // if its not a ladder rank, we don't do this check
						{
							if (aUserRank[i].Posts > 0 && aUserRank[i].Posts == iPosts && !(iUserRank == aUserRank[i].ID))
							{
								iRet = 1; // post conflict
								break;
							}
						}
						// check for starting rank
						if (bisstart == true && aUserRank[i].StartGroup == true && !(iUserRank == aUserRank[i].ID))
						{
							iRet = 3; // start conflict
							break;
						}
					}
				}
				
				sbRet.Append("  <method>checkUserRank</method>").Append(Environment.NewLine);
				sbRet.Append("  <result>").Append(iRet.ToString()).Append("</result>").Append(Environment.NewLine);
				
				return sbRet.ToString();
			}
			catch (Exception)
			{
				
			}
			
			return sbRet.ToString();
		}
		
		public string Handler_ExistingRule()
		{
			StringBuilder sbRet = new StringBuilder();
			ContentAPI apiContent = new ContentAPI();
			Ektron.Cms.DataIO.EkModuleRW rwModule;
			bool bRet = false;
			long iRule = 0;
			string sRule = "";
			
			try
			{
				iRule = Convert.ToInt64(Request.QueryString["rid"]);
				sRule = EkFunctions.HtmlEncode(Request.QueryString["rname"]);
				
				rwModule = new Ektron.Cms.DataIO.EkModuleRW(apiContent.RequestInformationRef);
				
				bRet = rwModule.GetRuleByName(sRule, iRule);
				
				sbRet.Append("  <method>checkRule</method>").Append(Environment.NewLine);
				sbRet.Append("  <result>").Append(Convert.ToInt32(bRet)).Append("</result>").Append(Environment.NewLine);
				
				return sbRet.ToString();
			}
			catch (Exception)
			{
				
			}
			
			return sbRet.ToString();
		}
		
		public string Handler_ExistingRuleset()
		{
			StringBuilder sbRet = new StringBuilder();
			ContentAPI apiContent = new ContentAPI();
			Ektron.Cms.DataIO.EkModuleRW rwModule;
			bool bRet = false;
			long iRuleset = 0;
			string sRuleset = "";
			
			try
			{
				iRuleset = Convert.ToInt64(Request.QueryString["rid"]);
				sRuleset = EkFunctions.HtmlEncode(Request.QueryString["rname"]);
				
				rwModule = new Ektron.Cms.DataIO.EkModuleRW(apiContent.RequestInformationRef);
				
				bRet = rwModule.GetRuleSetByName(sRuleset, iRuleset);
				
				sbRet.Append("  <method>checkRuleset</method>").Append(Environment.NewLine);
				sbRet.Append("  <result>").Append(Convert.ToInt32(bRet)).Append("</result>").Append(Environment.NewLine);
				
				return sbRet.ToString();
			}
			catch (Exception)
			{
				
			}
			
			return sbRet.ToString();
		}
		
		public string Handler_ExistingFolder()
		{
			StringBuilder sbRet = new StringBuilder();
			ContentAPI apiContent = new ContentAPI();
			EkContent cContent;
			long iParent = 0;
			long iFolder = 0;
			string sFolder = "";
			bool bExists = false;
			
			try
			{
				iParent = Convert.ToInt64(Request.QueryString["pid"]);
				sFolder = EkFunctions.HtmlEncode(Request.QueryString["fname"]);
				
				cContent = apiContent.EkContentRef;
				bExists = cContent.DoesFolderExistsWithName(sFolder, iParent, ref iFolder);
				
				sbRet.Append("  <method>checkName</method>").Append(Environment.NewLine);
				sbRet.Append("  <result>").Append(iFolder.ToString()).Append("</result>").Append(Environment.NewLine);
			}
			catch (Exception)
			{
				
			}
			return sbRet.ToString();
		}
		
		public string Handler_ExistingUser()
		{
			
			return "";
		}
		
		#endregion
		
	}