<%@ Page Language="C#" AutoEventWireup="true" CodeFile="ContentAnalytics.aspx.cs"
    Inherits="ContentAnalytics" %>

<%@ Register Src="controls/analytics/Global.ascx" TagName="Global" TagPrefix="uc3" %>
<%@ Register Src="controls/analytics/Page.ascx" TagName="Page" TagPrefix="uc4" %>
<%@ Register Src="controls/analytics/Referring_url.ascx" TagName="Referring_url"
    TagPrefix="uc5" %>
<%@ Register Src="controls/analytics/ContentReports.ascx" TagName="ContentReports"
    TagPrefix="uc2" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <title>Untitled Page</title>
    <meta http-equiv="content-type" content="text/html; charset=UTF-8" />
    <asp:literal id="StyleSheetJS" runat="server" />
</head>
<body>
    <form id="form1" name="form1" runat="server">
        <div id="dhtmltooltip">
        </div>
        <div class="ektronPageHeader">
            <div class="ektronTitlebar" id="divTitleBar" runat="server">
            </div>
            <div class="ektronToolbar" id="divToolBar" runat="server">
            </div>
        </div>
        <div class="ektronPageContainer ektronPageInfo">
            <table class="ektronForm">
                <tr>
                    <td class="label" title="Quick View">
                        <asp:Literal ID="lblQuickView" runat="server" EnableViewState="False" /></td>
                    <td class="readOnlyValue">
                        <asp:LinkButton ToolTip="View Day" ID="ctlDay" runat="server" EnableViewState="False"
                            Font-Bold="False" OnClick="ctlDay_Click">[Day]</asp:LinkButton>
                        <asp:LinkButton ToolTip="View Week" ID="ctlWeek" runat="server" EnableViewState="False"
                            OnClick="ctlWeek_Click">[Week]</asp:LinkButton>
                        <asp:LinkButton ToolTip="View Month" ID="ctlMonth" runat="server" EnableViewState="False"
                            OnClick="ctlMonth_Click">[Month]</asp:LinkButton>
                        <asp:LinkButton ToolTip="View Year" ID="ctlYear" runat="server" EnableViewState="False"
                            OnClick="ctlYear_Click">[Year]</asp:LinkButton>
                    </td>
                </tr>
                <tr>
                    <td class="label">
                        <asp:Literal ID="lblJumpTo" runat="server" EnableViewState="False" /></td>
                    <td class="readOnlyValue">
                        <asp:LinkButton ToolTip="Jump to Previous Day" ID="linkPrevious" runat="server" OnClick="linkPrevious_Click">[Previous Day]</asp:LinkButton>
                        <asp:LinkButton ToolTip="Jump to Next Day" ID="linkNext" runat="server" OnClick="linkNext_Click">[Next Day]</asp:LinkButton>
                        <asp:LinkButton ToolTip="Jump to Today" ID="linkToday" runat="server" OnClick="linkToday_Click">[Today]</asp:LinkButton>
                    </td>
                </tr>
                <asp:Literal ID="lblViewing" runat="server" EnableViewState="False" />
            </table>
            <asp:Button ToolTip="Run Custom Range" ID="Button1" runat="server" Text="Run Custom Range"
                OnClick="Button1_Click" />
            <div id="td_ecp_search" runat="server">
            </div>
            <hr />
            <asp:Label ID="Description" runat="server" Width="100%" Text="Label" />
            <br />
            <asp:Literal ID="navBar" runat="server" EnableViewState="False" />
            <uc3:Global ID="Global1" runat="server" Visible="false" />
            <uc4:Page ID="Page1" runat="server" Visible="false" />
            <uc2:ContentReports ID="ContentReports1" runat="server" Visible="false" />
            <uc5:Referring_url ID="Referring_url1" runat="server" Visible="false" />
        </div>
        <input type="hidden" id="start_date" name="start_date" />
        <input type="hidden" id="end_date" name="end_date" />
    </form>
</body>
</html>
