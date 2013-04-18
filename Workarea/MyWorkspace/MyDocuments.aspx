<%@ Page Language="C#" AutoEventWireup="true" CodeFile="MyDocuments.aspx.cs" Inherits="MyWorkspace_MyDocuments" %>
<%@ Register TagPrefix="cms" Namespace="Ektron.Cms.Controls" Assembly="Ektron.Cms.Controls" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml" >
<head runat="server">
    <title>Documents</title>
    <asp:Literal id="StyleSheet" runat="server"></asp:Literal>
</head>
<body>
    <form id="form1" runat="server">
        <div id="dhtmltooltip"></div>
        <div class="ektronTitlebar" id="divTitleBar" runat="server"></div>
        <div class="ektronToolbar" id="divToolBar" runat="server"></div>

        <cms:CommunityDocuments runat="server" ID="MyDocuments" />
    </form>
</body>
</html>

