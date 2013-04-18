<%@ Page Language="C#" validateRequest="false" AutoEventWireup="true" CodeFile="SimpleEditContent.aspx.cs" Inherits="SimpleEditContent" %>
<%@ Register Assembly="Ektron.Cms.Controls" Namespace="Ektron.Cms.Controls" TagPrefix="CMS" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml" >
<!-- Add:  Page Language="C#" validateRequest="false" ...  to allow posts. -->

<head runat="server">
    <title>HTML Editor Page</title>
    <link href="/CMS400Example/WorkArea/csslib/calendarStyles.css" rel="stylesheet" type="text/css" />
</head>
<body>
    <form id="form1" runat="server">
    <div id="EditHtmlUI" runat="server">
        <div>
            <h2 title="Simple Content Edit">
                Simple Content Edit
            </h2>
            <hr />  
        </div>
        <br />
        <CMS:HtmlEditor ID="HtmlEditor1" runat="server" ContentID="12">
        </CMS:HtmlEditor>
        <br />
        <br />
        <CMS:Login ID="Login1" runat="server" />
    </div>  
    
     
    </form>
</body>
</html>

