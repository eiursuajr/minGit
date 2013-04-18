<%@ Page Language="C#" AutoEventWireup="true" CodeFile="ImageEdit.aspx.cs" Inherits="ImageTool_ImageEdit" %>

<%@ Register TagPrefix="CMS" TagName="ImageEditor" Src="ImageModification.ascx" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml" >
<head runat="server">
	<link href="../csslib/ektron.workarea.css" type="text/css" rel="STYLESHEET"/>
    <title id="titlePage" runat="server" title="Edit Image">Edit Image</title>
</head>
<!--  
    onLoad="javascript: if (self.setIframeHeight) setIframeHeight('imageaffect')" -->
<body style="margin: 0px">
    <form id="form1" runat="Server">
	<asp:Panel ID="panelEdit" runat="server">
	    <CMS:ImageEditor ID="imagetool" runat="server" />
        </asp:Panel>
        <asp:Panel ToolTip="Message" ID="panelMessage" runat="server" Visible="false">
        <table><tr><td>
            <asp:Label ToolTip="Message" ID="lblMessage" runat="server" ForeColor="red" />
        </td></tr></table>
        </asp:Panel>
    </form>
</body>
</html>
