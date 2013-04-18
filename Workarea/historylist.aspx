<%@ Page Language="C#" AutoEventWireup="true" Inherits="historylist" CodeFile="historylist.aspx.cs" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>History</title>
    <meta http-equiv="content-type" content="text/html; charset=UTF-8" />
    <link href="../Workarea/csslib/ektron.workarea.css" rel="stylesheet" type="text/css" />
    <meta name="GENERATOR" content="Microsoft Visual Studio .NET 7.1" />
    <meta name="CODE_LANGUAGE" content="Visual Basic .NET 7.1" />
    <meta name="vs_defaultClientScript" content="JavaScript" />
    <meta name="vs_targetSchema" content="http://schemas.microsoft.com/intellisense/ie5" />
</head>
<body class="FolderArea">
    <asp:DataGrid ID="HistoryListGrid" runat="server" GridLines="None" AutoGenerateColumns="False"
        EnableViewState="False">
    </asp:DataGrid>
</body>
</html>
