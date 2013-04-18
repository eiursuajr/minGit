<%@ Page Language="C#" AutoEventWireup="true" CodeFile="ADDomains.aspx.cs" Inherits="AD_ADDomains" ValidateRequest="false" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml" >
<head runat="server">
    <meta http-equiv="content-type" content="text/html; charset=UTF-8" />
    <title>Active Directory Domains</title>
    <style type="text/css">
        .ektronForm { display:block !important;}
    </style>
</head>
<body>
    <form id="form1" runat="server">
        <script language="javascript" type="text/javascript">
            <asp:Literal ID="ltr_add_domain_js" runat="server" />
        </script>
        <div class="ektronPageContainer ektronPageInfo">
            <asp:Label ToolTip="<%=MyBase.GetMessage('entrprise license with AD required msg') %>" CssClass="important" id="lbl_msg" runat="server" Visible="false" />        
            <asp:Label id="lbl_add_domain" Runat="server" />		        
        </div>
    </form>
</body>
</html>

