<%@ Page Language="C#" AutoEventWireup="true" Inherits="history" CodeFile="history.aspx.cs" %>

<%@ Reference Control="controls/history/ViewHistoryList.ascx" %>
<%@ Reference Control="controls/history/ViewHistory.ascx" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>History</title>
    <meta content="text/html; charset=UTF-8" http-equiv="content-type" />
    <asp:Literal ID="StyleSheetJS" runat="server" />

    <script type="text/javascript">
		    <!--//--><![CDATA[//><!--
		        Ektron.ready( function() {
		            // $ektron("input").attr("disabled","disabled");
		            $ektron("ol.design_list_vertical").attr("onclick","");
		        });
		    //--><!]]>
    </script>

    <style type="text/css">
        .contentHistoryTitle, .contentHistoryComment
        {
            width: 25%;
        }
    </style>
</head>
<body>
    <form id="frmHistory" runat="server">
    <asp:Literal ID="SetFrame" runat="server" />
    <asp:PlaceHolder ID="DataHolder" runat="server" />
    </form>
</body>
</html>
