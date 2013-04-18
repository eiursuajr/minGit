<%@ Page Language="C#" AutoEventWireup="true" CodeFile="Report.aspx.cs" Inherits="msdashboard_Report" %>
<%@ Import Namespace="Ektron.Cms.BusinessObjects.Localization" %>
<%@ Register TagPrefix="Loc" TagName="ReportGrid" Src="ReportGrid.ascx" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Localization Report</title>
    <style type="text/css">
        .print 
        {
        	font: normal 9pt calibri, tahoma, helvetica;
        }
        .print table 
        {
        	border-left: 1px solid #e0e0e0;
        	border-bottom: 1px solid #e0e0e0;
        }
        .print td 
        {
        	padding: 1px 2px;
        	vertical-align: top;
        }
        .print .head 
        {
        	background-color: #808080;
        }
        .print .head td 
        {
        	font-weight: bold;
        	color: #ffffff;
        }
        .print .row 
        {
        }
        .print .rowAlt 
        {
        	background-color: #f0f0f0;
        }
        .print .row td
        {
        	border-right: 1px solid #e0e0e0;
        }
        .smallprint
        {
        	font-size: 0.8em;
        }
    </style>
</head>
<body onload="window.print();window.close();">
    <form id="form1" runat="server">
    <div>
<asp:Panel runat="server" ID="pnlPrint" CssClass="print">
<h3>Localization Report</h3>
<table cellpadding="0" cellspacing="0" border="0">
<tr class="head">
    <td>Title</td>
    <td>Status</td>
    <td>Translation Status</td>
    <td>Locales</td>
    <td>Last Modified</td>
    <td>Date Created</td>
    <td>Content ID</td>
    <td>Author</td>
</tr>
<asp:Repeater runat="server" ID="rptPrint">
<ItemTemplate><tr class="row">
<td><%# Eval("Title") %><%# this.IncludeFolderPath ? ("<br/><span class=\"smallprint\">" + SafeString(Eval("FolderPath") + "</span>")) : string.Empty %></td>
<td><%# Eval("ContentStatus") %></td>
<td><%# GetLocStatus(Eval("TranslationStatus"))%></td>
<td><%# GetLocales((System.Collections.Generic.List<int>)Eval("Locale"), -1) %></td>
<td><%# FormatDate(Eval("LastModified")) %></td>
<td><%# FormatDate(Eval("DateCreated")) %></td>
<td><%# Eval("ContentId")%></td>
<td><%# Eval("AuthorDisplayName") %></td>
</tr></ItemTemplate>
<AlternatingItemTemplate><tr class="row rowAlt">
<td><%# Eval("Title") %><%# this.IncludeFolderPath ? ("<br/><span class=\"smallprint\">" + SafeString(Eval("FolderPath") + "</span>")) : string.Empty %></td>
<td><%# Eval("ContentStatus") %></td>
<td><%# GetLocStatus(Eval("TranslationStatus"))%></td>
<td><%# GetLocales((System.Collections.Generic.List<int>)Eval("Locale"), -1) %></td>
<td><%# FormatDate(Eval("LastModified")) %></td>
<td><%# FormatDate(Eval("DateCreated")) %></td>
<td><%# Eval("ContentId")%></td>
<td><%# Eval("AuthorDisplayName") %></td>
</tr></AlternatingItemTemplate>
</asp:Repeater>
</table>
<asp:Literal runat="server" ID="lResultCount"></asp:Literal>
</asp:Panel>
<asp:Panel runat="server" ID="pnlExcel">
<asp:PlaceHolder runat="server" ID="plcExcel">
<html>
<head>
    <meta http-equiv=Content-Type content=""text/html; charset=utf-8"" />
    <meta name=ProgId content=Excel.Sheet />
    <style type="text/css">
        .head 
        {
        	background-color: #808080;
        }
        .head td 
        {
        	font-weight: bold;
        	color: #ffffff;
        }
        .rowAlt
        {
        	background-color: #f0f0f0;
        }
    </style>
</head>
<body>
<table>
<tr class="head">
    <td>Title</td>
    <td>Type</td>
    <td>Status</td>
    <td>Translation Status</td>
    <td>Locales</td>
    <td>Last Modified</td>
    <td>Date Created</td>
    <td>Content ID</td>
    <td>Author</td>
    <td>Preview URL</td>
<%= this.IncludeFolderPath ? "    <td>Folder Path</td>" : string.Empty %>
</tr>
<asp:Repeater runat="server" ID="rptExcel">
<ItemTemplate><tr class="row">
<td><%# Eval("Title") %></td>
<td><%# GetContentTypeIcon((ReportingData)Container.DataItem, true) %></td>
<td><%# Eval("ContentStatus") %></td>
<td><%# GetLocStatus((Ektron.Cms.Localization.LocalizationState)(byte)Eval("TranslationStatus")) %></td>
<td><%# GetLocales((System.Collections.Generic.List<int>)Eval("Locale"), -1) %></td>
<td><%# FormatDate(Eval("LastModified")) %></td>
<td><%# FormatDate(Eval("DateCreated")) %></td>
<td><%# Eval("ContentId")%></td>
<td><%# Eval("AuthorDisplayName") %></td>
<td><a href="<%# GenerateAbsoluteUrl(GetPreviewUrl((ReportingData)Container.DataItem)) %>"><%# GenerateAbsoluteUrl(GetPreviewUrl((ReportingData)Container.DataItem)) %></a></td>
<%# this.IncludeFolderPath ? ("<td>" + SafeString(Eval("FolderPath") + "</td>")) : string.Empty %>
</tr></ItemTemplate>
<AlternatingItemTemplate><tr class="rowAlt">
<td><%# Eval("Title") %></td>
<td><%# GetContentTypeIcon((ReportingData)Container.DataItem, true) %></td>
<td><%# Eval("ContentStatus") %></td>
<td><%# GetLocStatus((Ektron.Cms.Localization.LocalizationState)(byte)Eval("TranslationStatus")) %></td>
<td><%# GetLocales((System.Collections.Generic.List<int>)Eval("Locale"), -1) %></td>
<td><%# FormatDate(Eval("LastModified")) %></td>
<td><%# FormatDate(Eval("DateCreated")) %></td>
<td><%# Eval("ContentId")%></td>
<td><%# Eval("AuthorDisplayName") %></td>
<td><a href="<%# GenerateAbsoluteUrl(GetPreviewUrl((ReportingData)Container.DataItem)) %>"><%# GenerateAbsoluteUrl(GetPreviewUrl((ReportingData)Container.DataItem)) %></a></td>
<%# this.IncludeFolderPath ? ("<td>" + SafeString(Eval("FolderPath") + "</td>")) : string.Empty %>
</tr></AlternatingItemTemplate>
</asp:Repeater>
</table>
</body>
</html>
</asp:PlaceHolder></asp:Panel>
    </div>
    </form>
</body>
</html>
