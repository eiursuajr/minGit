<%@ Page Language="C#" AutoEventWireup="true" CodeFile="usersdirectory.aspx.cs" Inherits="usersdirectory" %>
<%@ Register TagPrefix="cms" Namespace="Ektron.Cms.Controls" Assembly="Ektron.Cms.Controls" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml" >
<head runat="server">
    <title>Personal Directory</title>
    <link href="csslib/personaldirectory.css" type="text/css" rel="Stylesheet" />
</head>
<body>
    <form id="form1" runat="server">
        <table cellspacing="0" rules="all" border="0" id="FavGrid" style="background-color:White;border-color:White;border-width:0px;border-style:None;width:100%;border-collapse:collapse;">
		    <tr>
			    <td style="width:5%;">&nbsp;</td><td>&nbsp;</td><td>&nbsp;</td><td>&nbsp;</td>
		    </tr>
		    <tr>
			    <td valign="top" style="width:5%;white-space:nowrap;">&#160;</td>
			    <td valign="top" style="width:45%;white-space:nowrap;">&#160;</td>
			    <td valign="top" style="width:5%;white-space:nowrap;">&nbsp;</td>
			    <td valign="top" style="width:45%;white-space:nowrap;">&nbsp;</td>
		    </tr>
		    <tr>
			    <td valign="top" style="width:5%;white-space:nowrap;">&#160;</td>
			    <td valign="top" style="width:45%;white-space:nowrap;">
			       <a href="MyWorkspace/MyDocuments.aspx" title="Documents"><img border="0" align="left" src="images/application/my_document.gif" alt="Documents" title="Documents" /></a>&#160;<a href="MyWorkspace/MyDocuments.aspx" title="Documents"><asp:Literal ID="ltr_documents" runat="server"></asp:Literal></a>
			    </td>
			    <td valign="top" style="width:5%;white-space:nowrap;">&#160;</td>
			    <td valign="top" style="width:45%;white-space:nowrap;">
			        <a href="MyWorkspace/MyGroups.aspx" title="Community Groups"><img border="0" align="left" src="images/application/my_groups.gif" alt="Community Groups" title="Community Groups"/></a>&#160;<a href="MyWorkspace/MyGroups.aspx" title="Community Groups"><asp:Literal ID="ltr_cgroups" runat="server"></asp:Literal></a>
			    </td>
		    </tr>
		    <tr>
			    <td valign="top" style="width:5%;white-space:nowrap;">&#160;</td>
			    <td valign="top" style="width:45%;white-space:nowrap;">&#160;</td>
			    <td valign="top" style="width:5%;white-space:nowrap;">&nbsp;</td>
			    <td valign="top" style="width:45%;white-space:nowrap;">&nbsp;</td>
		    </tr>
		    <tr>
			    <td valign="top" style="width:5%;white-space:nowrap;">&#160;</td>
			    <td valign="top" style="width:45%;white-space:nowrap;">
			       <a href="MyWorkspace/MyPendingGroups.aspx" title="Pending Groups"><img border="0" align="left" src="images/application/my_pending_groups.gif" alt="Pending Groups" title="Pending Groups" /></a>&#160;<a href="MyWorkspace/MyPendingGroups.aspx" title="Pending Groups"><asp:Literal ID="ltr_pendingcgroups" runat="server"></asp:Literal></a>
			    </td>
			    <td valign="top" style="width:5%;white-space:nowrap;">&#160;</td>
			    <td valign="top" style="width:45%;white-space:nowrap;">
			        <a href="MyWorkspace/MyInvitedGroups.aspx" title="Invited Groups"><img border="0" align="left" src="images/application/my_group_invites.gif" alt="Invited Groups" title="Invited Groups"/></a>&#160;<a href="MyWorkspace/MyInvitedGroups.aspx" title="Invited Groups"><asp:Literal ID="ltr_cgroupinvites" runat="server"></asp:Literal></a>
			    </td>
		    </tr>
		    <tr>
			    <td valign="top" style="width:5%;white-space:nowrap;">&#160;</td>
			    <td valign="top" style="width:45%;white-space:nowrap;">&#160;</td>
			    <td valign="top" style="width:5%;white-space:nowrap;">&nbsp;</td>
			    <td valign="top" style="width:45%;white-space:nowrap;">&nbsp;</td>
		    </tr>
		    <tr>
			    <td valign="top" style="width:5%;white-space:nowrap;">&#160;</td>
			    <td valign="top" style="width:45%;white-space:nowrap;">
			       <a href="MyWorkspace/MyFavorites.aspx" title="Favorites"><img border="0" align="left" src="images/application/my_favorites.gif" alt="Favorites" title="Favorites" /></a>&#160;<a href="MyWorkspace/MyFavorites.aspx" title="Favorites"><asp:Literal ID="ltr_fav" runat="server"></asp:Literal></a>
			    </td>
			    <td valign="top" style="width:5%;white-space:nowrap;">&#160;</td>
			    <td valign="top" style="width:45%;white-space:nowrap;">
			        <a href="MyWorkspace/MyFriends.aspx" title="Colleagues"><img border="0" align="left" src="images/application/my_friends.gif" alt="Colleagues" title="Colleagues"/></a>&#160;<a href="MyWorkspace/MyFriends.aspx" title="Colleagues"><asp:Literal ID="ltr_friends" runat="server"></asp:Literal></a> 
			    </td>
		    </tr>
		    <tr>
			    <td valign="top" style="width:5%;white-space:nowrap;">&#160;</td>
			    <td valign="top" style="width:45%;white-space:nowrap;">&#160;</td>
			    <td valign="top" style="width:5%;white-space:nowrap;">&nbsp;</td>
			    <td valign="top" style="width:45%;white-space:nowrap;">&nbsp;</td>
		    </tr>
		    <tr>
			    <td valign="top" style="width:5%;white-space:nowrap;">&#160;</td>
			    <td valign="top" style="width:45%;white-space:nowrap;">
			        <a href="MyWorkspace/MyPendingFriends.aspx" title="Pending Colleagues"><img border="0" align="left" src="images/application/my_pendingfriends.gif" alt="Pending Colleagues" title="Pending Colleagues" /></a>&#160;<a href="MyWorkspace/MyPendingFriends.aspx" title="Pending Colleagues"><asp:Literal ID="ltr_pendingfriends" runat="server"></asp:Literal></a>
			    </td>
			    <td valign="top" style="width:5%;white-space:nowrap;">&#160;</td>
			    <td valign="top" style="width:45%;white-space:nowrap;">
			        <a href="MyWorkspace/MySentFriends.aspx" title="Sent Colleague Requests"><img border="0" align="left" src="images/application/my_friendinvites.gif" alt="Sent Colleague Requests" title="Sent Colleague Requests"/></a>&#160;<a href="MyWorkspace/MySentFriends.aspx" title="Sent Colleague Requests"><asp:Literal ID="ltr_friendinvites" runat="server"></asp:Literal></a> 
			    </td>
		    </tr>
		    <tr>
			    <td valign="top" style="width:5%;white-space:nowrap;">&#160;</td>
			    <td valign="top" style="width:45%;white-space:nowrap;">&#160;</td>
			    <td valign="top" style="width:5%;white-space:nowrap;">&nbsp;</td>
			    <td valign="top" style="width:45%;white-space:nowrap;">&nbsp;</td>
		    </tr>
		    <tr>
			    <td valign="top" style="width:5%;white-space:nowrap;">&#160;</td>
			    <td valign="top" style="width:45%;white-space:nowrap;">
			        <a href="MyWorkSpace/MyJournal.aspx" title="Journal"><img border="0" align="left" src="images/application/my_journal.gif" alt="Journal" title="Journal"/></a>&#160;<a href="MyWorkSpace/MyJournal.aspx" title="Journal"><asp:Literal ID="ltr_journal" runat="server"></asp:Literal></a>
			    </td>
			    <td valign="top" style="width:5%;white-space:nowrap;">&#160;</td>
			    <td valign="top" style="width:45%;white-space:nowrap;">
			        <a href="MyWorkspace/MyMessageBoard.aspx" title="Message Board"><img border="0" align="left" src="images/application/my_messagebd.gif" alt="Message Board" title="Message Board"/></a>&#160;<a href="MyWorkspace/MyMessageBoard.aspx" title="Message Board"><asp:Literal ID="ltr_wall" runat="server"></asp:Literal></a>
			    </td>
		    </tr>
		    <tr>
			    <td valign="top" style="width:5%;white-space:nowrap;">&#160;</td>
			    <td valign="top" style="width:45%;white-space:nowrap;">&#160;</td>
			    <td valign="top" style="width:5%;white-space:nowrap;">&nbsp;</td>
			    <td valign="top" style="width:45%;white-space:nowrap;">&nbsp;</td>
		    </tr>
		    <tr>
			    <td valign="top" style="width:5%;white-space:nowrap;">&#160;</td>
			    <td valign="top" style="width:45%;white-space:nowrap;">
			        <a href="CommunityMessaging.aspx?action=viewall" title="Colleagues"><img border="0" align="left" src="images/application/my_inbox.gif" alt="Colleagues" title="Colleagues"/></a>&#160;<a href="CommunityMessaging.aspx?action=viewall" title="Colleagues"><asp:Literal ID="ltr_messaging" runat="server"></asp:Literal></a>
			    </td>
			    <td valign="top" style="width:5%;white-space:nowrap;">&#160;</td>
			    <td valign="top" style="width:45%;white-space:nowrap;">
			        <a href="CommunityMessaging.aspx?action=viewallsent" title="Sent Messages"><img border="0" align="left" src="images/application/my_mail.gif" alt="Sent Messages" title="Sent Messages"/></a>&#160;<a href="CommunityMessaging.aspx?action=viewallsent" title="Sent Messages"><asp:Literal ID="ltr_sentmsg" runat="server"></asp:Literal></a>
			    </td>
		    </tr>
		    <tr>
			    <td valign="top" style="width:5%;white-space:nowrap;">&#160;</td>
			    <td valign="top" style="width:45%;white-space:nowrap;">&#160;</td>
			    <td valign="top" style="width:5%;white-space:nowrap;">&nbsp;</td>
			    <td valign="top" style="width:45%;white-space:nowrap;">&nbsp;</td>
		    </tr>
		    <tr>
			    <td valign="top" style="width:5%;white-space:nowrap;">&#160;</td>
			    <td valign="top" style="width:45%;white-space:nowrap;">
			        <a href="MyWorkspace/MyPhotoGallery.aspx" title="My Photos"><img border="0" align="left" src="images/application/my_photos.gif" alt="My Photos" title="My Photos"/></a>&#160;<a href="MyWorkspace/MyPhotoGallery.aspx" title="My Photos"><asp:Literal ID="ltr_photos" runat="server"></asp:Literal></a>
			    </td>
			    <td valign="top" style="width:5%;white-space:nowrap;">&#160;</td>
			    <td valign="top" style="width:45%;white-space:nowrap;">
			        &#160;
			    </td>
		    </tr>
	    </table>
        <asp:Literal runat="Server" ID="ltr_js"></asp:Literal>
    </form>
</body>
</html>

