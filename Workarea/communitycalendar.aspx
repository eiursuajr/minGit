<%@ Page Language="C#" AutoEventWireup="true" CodeFile="communitycalendar.aspx.cs" Inherits="Workarea_communitycalendar" %>
<%@ Register Assembly="Ektron.Cms.Controls" Namespace="Ektron.Cms.Controls" TagPrefix="CMS" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>View</title>
</head>
<body>
    <form id="form1" runat="server">
    <div>
        <CMS:ContentBlock id="ContentBlock1" runat="server" DynamicParameter="id"></CMS:ContentBlock>
    </div>
    </form>
</body>
</html>
