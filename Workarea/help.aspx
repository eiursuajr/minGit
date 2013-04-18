<%@ Page Language="C#" AutoEventWireup="true" Inherits="help" CodeFile="help.aspx.cs" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
    <meta http-equiv="content-type" content="text/html; charset=UTF-8" />
    <meta name="GENERATOR" content="Microsoft Visual Studio .NET 7.1" />
    <meta name="CODE_LANGUAGE" content="Visual Basic .NET 7.1" />
    <meta name="vs_defaultClientScript" content="JavaScript" />
    <meta name="vs_targetSchema" content="http://schemas.microsoft.com/intellisense/ie5" />
    <asp:Literal ID="StyleSheetJS" runat="server" />
</head>
<body>
    <form id="Form1" method="post" runat="server">
    <iframe id="frmHelp" runat="server" width="100%" height="590px" frameborder="0"></iframe>
    <asp:Panel ID="pnlManuals" runat="server">
        <div class="ektronPageHeader">
            <div class="ektronTitlebar" title="Ektron CMS400 Help">
                Ektron CMS400 Help</div>
            <div class="ektronToolbar" id="divToolBar" runat="server">
            </div>
        </div>
        <div class="ektronPageContainer ektronPageInfo">
            <a href="http://dev.ektron.com" title="<%= m_refMsg.GetMessage("alt visit help dev.ektron.com") %>"
                target="Manual">
                <%=m_refMsg.GetMessage("visit help ektron.com")%></a>
            <br />
            <a href="http://<%=Request.ServerVariables["SERVER_NAME"] + ":" + Request.ServerVariables["SERVER_PORT"] + AppPath%>documents/DevelopersManual.pdf"
                title="<%= m_refMsg.GetMessage("dev manual msg") %>" target="Manual">
                <%=m_refMsg.GetMessage("dev manual msg")%></a>
            <br />
            <a href="http://<%=Request.ServerVariables["SERVER_NAME"] + ":" + Request.ServerVariables["SERVER_PORT"] + AppPath%>documents/adminquick.pdf"
                title="<%= m_refMsg.GetMessage("admin quick manual msg") %>" target="Manual">
                <%=m_refMsg.GetMessage("admin quick manual msg")%></a>
            <br />
            <a href="http://<%=Request.ServerVariables["SERVER_NAME"] + ":" + Request.ServerVariables["SERVER_PORT"] + AppPath%>documents/adminmanual.pdf"
                title="<%= m_refMsg.GetMessage("admin manual msg") %>" target="Manual">
                <%=m_refMsg.GetMessage("admin manual msg")%></a>
            <br />
            <a href="http://<%=Request.ServerVariables["SERVER_NAME"] + ":" + Request.ServerVariables["SERVER_PORT"] + AppPath%>documents/usermanual.pdf"
                title="<%= m_refMsg.GetMessage("user manual msg") %>" target="Manual">
                <%=m_refMsg.GetMessage("user manual msg")%></a>
            <br />
            <a href="http://<%=Request.ServerVariables["SERVER_NAME"] + ":" + Request.ServerVariables["SERVER_PORT"] + AppPath%>documents/SetupManual.pdf"
                title="<%= m_refMsg.GetMessage("setup manual msg") %>" target="Manual">
                <%=m_refMsg.GetMessage("setup manual msg")%></a>
            <br />
            <a href="http://<%=Request.ServerVariables["SERVER_NAME"] + ":" + Request.ServerVariables["SERVER_PORT"] + AppPath%>documents/APIReference.pdf"
                title="<%= m_refMsg.GetMessage("api reference manual msg") %>" target="Manual">
                <%=m_refMsg.GetMessage("api reference manual msg")%></a>
            <br />
            <a href="<%=ConfigurationManager.AppSettings["ek_helpDomainPrefix"]%>index.html"
                title="<%= m_refMsg.GetMessage("user and admin manual msg") %>" target="Manual">
                <%=m_refMsg.GetMessage("user and admin manual msg")%></a>
        </div>
    </asp:Panel>
    </form>
</body>
</html>
