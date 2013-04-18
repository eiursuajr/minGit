<%@ Page Language="C#" AutoEventWireup="true" CodeFile="EditUserProfile.aspx.cs" Inherits="Workarea_EditUserProfile" %>

<%@ Register Assembly="Ektron.Cms.Controls" Namespace="Ektron.Cms.Controls" TagPrefix="CMS" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml" >
<head runat="server">
    <title>Untitled Page</title>
     <script src="java/Ektron.js" type="text/javascript">
</script>
    <script src="java/thickbox.js" type="text/javascript">
</script>
</head>
<body>
    <form id="form1" runat="server">
    <div>
        <CMS:Membership ID="Membership1" DynamicParameter ="id"  runat="server" />
        <asp:Literal ID="ltrMessage" runat="server" ></asp:Literal>
    </div>
    </form>
</body>
</html>

