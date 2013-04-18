<%@ Page Language="C#" AutoEventWireup="true" Inherits="taskhistory" CodeFile="taskhistory.aspx.cs" %>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta name="vs_targetSchema" content="http://schemas.microsoft.com/intellisense/ie5" />
    <meta http-equiv="pragma" content="no-cache" />
    <script type="text/javascript" src="java/stylehelper.js"></script>
    <asp:Literal ID="ltrScript" runat="server"></asp:Literal>
    <script type="text/javascript" src="java/toolbar_roll.js"></script>
    <link href="csslib/ektron.fixedPositionToolbar.css" rel="stylesheet" type="text/css" />
</head>
<body>
    <div class="ektronPageHeader">
        <div class="ektronTitlebar">
            <asp:Literal ID="ltrTitleBar" runat="server"></asp:Literal>
        </div>
    </div>
    <div class="ektronPageContainer">
        <table width="100%" class="ektronGrid">
            <tr class="title-header">
                <td>
                    <a href="taskhistory.aspx?action=<%=action%>&orderby=change_date&tid=<%=TaskID%>&starttime=<%=StartTime%>&endtime=<%=EndTime%>"
                        title="<%=(MsgHelper.GetMessage("click to sort msg"))%>" title="<%=(MsgHelper.GetMessage("click to sort msg"))%>">
                        Date</a>
                </td>
                <td>
                    <a href="taskhistory.aspx?action=<%=action%>&orderby=action_by_id&tid=<%=TaskID%>&starttime=<%=StartTime%>&endtime=<%=EndTime%>"
                        title="<%=(MsgHelper.GetMessage("click to sort msg"))%>" title="<%=(MsgHelper.GetMessage("click to sort msg"))%>">
                        Initiator</a>
                </td>
                <td>
                    <a href="taskhistory.aspx?action=<%=action%>&orderby=change_type&tid=<%=TaskID%>&starttime=<%=StartTime%>&endtime=<%=EndTime%>"
                        title="<%=(MsgHelper.GetMessage("click to sort msg"))%>" title="<%=(MsgHelper.GetMessage("click to sort msg"))%>">
                        Activity</a>
                </td>
                <td>
                    Information
                </td>
            </tr>
            <asp:Literal ID="ltrTasks" runat="server"></asp:Literal>
        </table>
    </div>
</body>
</html>
