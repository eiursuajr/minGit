<%@ Page Language="C#" AutoEventWireup="true" CodeFile="WorkflowImage.aspx.cs" Inherits="WorkflowImage" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Untitled Page</title>
    <style type="text/css">
        html, body
        {
            margin: 0;
            padding: 0;
        }
        #form1
        {
            display: block;
            margin: 0;
            padding: 0;
        }
        .blueBoxBlueBorder
        {
            background: #C1DAEE;
            border: solid 5px #84B6DE;
            padding: 0;
            font-size: 140%;
            float: left;
        }
        .clearFix:after
        {
            content: " ";
            display: block;
            height: 0;
            clear: both;
            visibility: hidden;
            font-size: 0;
        }
        .clearfix
        {
            display: inline-block;
        }
        /* Hides from IE-mac \*/* html .clearfix
        {
            height: 1%;
        }
        .clearfix
        {
            display: block;
        }
        /* End hide from IE-mac */</style>
</head>
<body>
    <form id="form1" runat="server">
    <div class="box clearFix">
        <div class="blueBoxBlueBorder">
            <img alt="workflow image" src="<%=wfimageUrl%>" />
        </div>
    </div>
    </form>
</body>
</html>
