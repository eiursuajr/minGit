<%@ Page Language="C#" AutoEventWireup="true" CodeFile="FriendSearch.aspx.cs" Inherits="MyWorkspace_FriendSearch" %>
<%@ Register TagPrefix="cms" Namespace="Ektron.Cms.Controls" Assembly="Ektron.Cms.Controls" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml" >
<head runat="server">
    <title> </title>
</head>
<body>
    <form id="form1" runat="server">
    <div>
        <cms:CommunitySearch ID="cs1" EnableMap="false" MembersOnly="false" runat="server" />
    </div>
    </form>
</body>
</html>

