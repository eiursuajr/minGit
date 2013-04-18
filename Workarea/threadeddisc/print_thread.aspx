<%@ Page Language="C#" AutoEventWireup="true" CodeFile="print_thread.aspx.cs" Inherits="threadeddisc_print_thread" %>

<%@ Register Assembly="Ektron.Cms.Controls" Namespace="Ektron.Cms.Controls" TagPrefix="CMS" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml" >
<head runat="server">
    <title>Untitled Page</title>
    <script type="text/javascript">
        window.print();
    </script>
    <style type="text/css" id="print_style" runat="server">
    
    </style>
</head>
<body>
    <form id="form1" runat="server">
    <div>
        <CMS:Forum ID="Forum1" runat="server" DynamicParameter="id" />
    </div>
    </form>
</body>
</html>

