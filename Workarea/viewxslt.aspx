<%@ Page Language="C#" AutoEventWireup="true" Inherits="viewxslt" CodeFile="viewxslt.aspx.cs" %>
<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN">
<html xmlns="http://www.w3.org/1999/xhtml">
  <head runat="server">
    <title>viewxslt</title>
    <meta name="GENERATOR" content="Microsoft Visual Studio .NET 7.1"/>
    <meta name="CODE_LANGUAGE" content="Visual Basic .NET 7.1"/>
    <meta name="vs_defaultClientScript" content="JavaScript"/>
    <meta name="vs_targetSchema" content="http://schemas.microsoft.com/intellisense/ie5"/>
    <asp:literal id="StyleSheetJS" runat="server"/>
  </head>
  <body>
    <form id="Form1" method="post" runat="server">
        <div id="dhtmltooltip"></div>
        <div class="ektronPageHeader">
            <div class="ektronTitlebar" id="divTitleBar" runat="server"></div>
            <div class="ektronToolbar" id="divToolBar" runat="server"></div>
        </div>
        <div class="ektronPageContainer ektronPageInfo">
            <textarea readonly wrap="soft" name="display_xslt" id="display_xslt" style="width:100%;height:600px" runat="server"></textarea>
		</div>
    </form>
  </body>
</html>