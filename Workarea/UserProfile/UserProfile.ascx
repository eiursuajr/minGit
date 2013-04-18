<%@ Control Language="C#" AutoEventWireup="true" CodeFile="UserProfile.ascx.cs" Inherits="UserProfile_UserProfile" %>
<%@ Register Assembly="Ektron.Cms.Controls" Namespace="Ektron.Cms.Controls" TagPrefix="CMS" %>
<div id="EkLeftPanel">
    <div><a title="Profile" href="?action=profile">Profile</a></div>
    <div><a title="Friends" href="?action=friends"><img runat="server" src="~/workarea/images/application/disc-topic.gif" alt="Friends" title="Friends" />Friends</a></div>
    <div><a title="Favorites" href="?action=favorites">Favorites</a></div>
    <div><a title="Groups" href="?action=groups">Groups</a></div>
    <div><a title="Photos" href="?action=photos">Photos</a></div>
    <div><a title="Documents" href="?action=documents">Documents</a></div>
</div>
<div id="EkMainPanel">
    <div id="EkUserProfile">
        <CMS:UserProfile ID="UserProfile1" runat="server" DynamicParameter="id" Visible="false" />        
    </div>
    <div id="EkFriends">
        <CMS:Friends ID="Friends1" runat="server" Visible="false" />
    </div>
    <div id="EkFavorites">
        <CMS:Favorites ID="Favorites1" runat="server" Visible="false" />
    </div>
    <div id="EkGroups">
        <CMS:CommunityGroupList ID="CommunityGroupList1" Link="../../Developer/Community/CommunityGroups.aspx?id={0}" runat="server" Visible="false" />
    </div>
    <div id="EkDocuments">
        <CMS:CommunityDocuments ID="Workspace1" runat="server" Visible="false" />
    </div>
</div>
