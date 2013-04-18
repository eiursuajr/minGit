<%@ Page Language="C#" AutoEventWireup="true" CodeFile="usermap.aspx.cs" Inherits="usermap" %>

<%@ Register TagPrefix="cms" Namespace="Ektron.Cms.Controls" Assembly="Ektron.Cms.Controls" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>User map location</title>
</head>
<body>
    <form id="form1" runat="server">
    <div>
        <cms:Map ID="map1" runat="server" StartZoomLevel="12" SearchData="User" Width="520"
            Height="520" />
    </div>
    </form>
</body>
</html>
