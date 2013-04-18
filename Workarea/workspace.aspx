<%@ Page Language="C#" AutoEventWireup="true" CodeFile="workspace.aspx.cs" Inherits="Workarea_workspace" %>
<%@ Register Assembly="Ektron.Cms.Controls" Namespace="Ektron.Cms.Controls" TagPrefix="CMS" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml" >
<head runat="server">
    <title>View</title>
    <style type="text/css">
        #ContentBlock1 div.MediaPlayerArea{
            display: none;
        }
        iframe
        {
            height: 400px;
        }
        td
        {
            vertical-align:top;
        }
    </style>
</head>
<body>
    <form id="form1" runat="server">
    <p>&#160;</p>
    <CMS:ContentBlock id="ContentBlock1" runat="server" DynamicParameter="id"></CMS:ContentBlock>
    </form>
</body>
</html>