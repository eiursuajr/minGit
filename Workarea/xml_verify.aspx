<%@ Page Language="C#" AutoEventWireup="true" Inherits="xml_verify" CodeFile="xml_verify.aspx.cs" %>

<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN">
<html>
<head runat="server">
    <title>xml_verify</title>
    <meta name="GENERATOR" content="Microsoft Visual Studio .NET 7.1" />
    <meta name="CODE_LANGUAGE" content="c#" />
    <meta name="vs_defaultClientScript" content="JavaScript" />
    <meta name="vs_targetSchema" content="http://schemas.microsoft.com/intellisense/ie5" />
</head>
<body onload="javascript:CompleteParserVerify()">
    <form id="Form1" method="post" runat="server">
        <br />
        <asp:Literal ID="verifydata" runat="server" />
        <br />
        <br />
        <input type="button" value="close" onclick="javascript:self.close();" />
    </form>
</body>
</html>
