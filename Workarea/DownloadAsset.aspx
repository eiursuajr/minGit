<%@ Page Language="C#" AutoEventWireup="true" CodeFile="DownloadAsset.aspx.cs" Inherits="DownloadAsset" %>

<%@ Register Assembly="Ektron.Cms.Controls" Namespace="Ektron.Cms.Controls" TagPrefix="CMS" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Untitled Page</title>
    <meta http-equiv="content-type" content="text/html; charset=UTF-8" />
</head>
<body>
    <form id="form1" runat="server">
    <div style="width:100%; text-align:center;padding: .25em .5em; margin: .25em 0">
    <asp:Panel ID="Message" runat="server" Width="500px" HorizontalAlign="Left" BorderStyle="Solid" BorderWidth="1px" BorderColor="Black">
        <div class="ui-state-error ui-corner-all ui-helper-clearfix" style="padding: .25em .5em;">
            <span class="ui-icon ui-icon-alert" style="float: left; margin-right: 0.3em; margin-top: .2em">
            </span>
            <asp:Literal ID="ErrorText" runat="server" Text="Error" />
        </div>
        <div style="padding: .25em .5em;">
            <asp:Label runat="server" ID="notification_message" Font-Bold="True" ForeColor="Red" />
            <CMS:Login ID="Login" HorizontalAlign="Center" runat="server" Visible="false" WrapTag="div" SuppressHelpButton="true" />
        </div>
    </asp:Panel>
    </div>
    </form>
</body>
</html>
