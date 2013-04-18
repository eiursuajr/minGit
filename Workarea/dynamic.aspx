<%@ Page Language="C#" AutoEventWireup="true" CodeFile="dynamic.aspx.cs" Inherits="dynamic"
    Title="Dynamic" %>

<%@ Register Assembly="Ektron.Cms.Controls" Namespace="Ektron.Cms.Controls" TagPrefix="CMS" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>CMS400 8.5 Workarea</title>
    <meta http-equiv="content-type" content="text/html; charset=UTF-8" />
    <meta content="False" name="vs_showGrid" />
    <meta content="Microsoft Visual Studio .NET 7.1" name="GENERATOR" />
    <meta content="Visual Basic .NET 7.1" name="CODE_LANGUAGE" />
    <meta content="JavaScript" name="vs_defaultClientScript" />
    <meta content="http://schemas.microsoft.com/intellisense/ie5" name="vs_targetSchema" />
</head>
<body style="text-align: center">
    <form id="Form1" method="post" runat="server">
    <div id="pagemid_welcome_text">
        <CMS:ContentBlock ID="ContentBlock1" runat="server" DefaultContentID="33" DynamicParameter="id">
        </CMS:ContentBlock>
    </div>
    </form>
</body>
</html>
