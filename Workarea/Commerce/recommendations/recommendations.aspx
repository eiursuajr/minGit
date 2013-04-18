<%@ Page Language="C#" AutoEventWireup="true" CodeFile="recommendations.aspx.cs" Inherits="Commerce_recommendations" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml" >
<head runat="server">
    <title>Recommendations</title>
    <asp:literal id="ltr_js" runat="server"/>
</head>
<body onclick="MenuUtil.hide()" style="min-height:30em;height:auto;padding-bottom:15em;background-color:White;">
    <form id="form1" runat="server">
    <div>
        <asp:Literal ID="ltr_recommendations" runat="server"/>
    </div>
    </form>
</body>
</html>

