<%@ Page Language="C#" AutoEventWireup="true" CodeFile="productsearch.aspx.cs" Inherits="Workarea_productsearch" %>

<%@ Register Assembly="Ektron.Cms.Controls" Namespace="Ektron.Cms.Controls" TagPrefix="CMS" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Untitled Page</title>
    <meta http-equiv="content-type" content="text/html; charset=UTF-8" />
 </head>
<body id="body" runat="server">
    <%=StyleSheetJS%>
    <%=SearchStyleSheet%>
    <%=SearchJScript%>
    <form id="form1" runat="server">
        <div id="dhtmltooltip"></div>
        <div class="ektronPageHeader">
            <div class="ektronTitlebar" id="divTitleBar" runat="server"></div>
            <div class="ektronToolbar" id="divToolBar" runat="server"></div>
        </div>
        <cms:productsearch id="ProductSearch1" runat="server" TemplateProduct="ProductView.aspx" ></cms:productsearch>
    </form>
</body>
</html>

