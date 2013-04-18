<%@ Page Language="C#" AutoEventWireup="true" CodeFile="productview.aspx.cs" Inherits="Workarea_productview" %>

<%@ Register Assembly="Ektron.Cms.Controls" Namespace="Ektron.Cms.Controls" TagPrefix="CMS" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Product View</title>
    <meta http-equiv="content-type" content="text/html; charset=UTF-8" />
 </head>
<body id="body" runat="server">
    <%=StyleSheetJS%>
    <%=ViewStyleSheet%>
    <%=ViewJScript%>
    <form id="form1" runat="server">
        <div id="dhtmltooltip"></div>
        <div class="ektronPageHeader">
            <div class="ektronTitlebar" id="divTitleBar" runat="server"></div>
            <div class="ektronToolbar" id="divToolBar" runat="server"></div>
        </div>
        <div style="padding-top: 20px;">
            <cms:product id="p1" runat="server" 
                defaultproductid="1013" 
                DynamicParameter="id" DisplayXslt="XSLT/WA_Product.xsl"
                />
        </div>
    </form>
</body>
</html>
