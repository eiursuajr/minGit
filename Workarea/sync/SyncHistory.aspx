<%@ Page Language="C#" AutoEventWireup="true" CodeFile="SyncHistory.aspx.cs" Inherits="SyncHistory" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Synchronization History</title>
    <!-- Ektron Client Script -->
    <asp:Literal ID="ektronClientScript" runat="server"></asp:Literal>
    <script type="text/javascript">
        function showException() {
            current = document.getElementById("dvException").style.visibility;
            if (current == "visible") {
                document.getElementById("dvException").style.visibility = "hidden";
            }
            else {
                document.getElementById("dvException").style.visibility = "visible";
            }
        }
    </script>
</head>
<body>
    <form id="form1" runat="server">
    <div class="ektronPageHeader">
        <div id="dhtmltooltip">
        </div>
        <div class="ektronTitlebar" id="divTitleBar" runat="server">
        </div>
        <div class="ektronToolbar" id="divToolBar" runat="server">
            <table>
                <tr id="rowToolbarButtons" runat="server">
                </tr>
            </table>
        </div>
    </div>
    <div class="ektronPageContainer">
        <asp:Panel ID="pnlLogList" runat="server">
            <asp:GridView ID="gvLogData" runat="server" AllowPaging="true" PageSize="50" AutoGenerateColumns="False"
                BorderWidth="1px" CssClass="ektronGrid" BackColor="White" CellPadding="3" BorderStyle="Solid"
                BorderColor="#CCCCCC" Font-Names="Arial" OnPageIndexChanging="gvLogData_PagingIndexChanging">
                <PagerSettings Visible="true" Mode="Numeric"></PagerSettings>
                <FooterStyle ForeColor="#000066" BackColor="White"></FooterStyle>
                <PagerStyle ForeColor="#000066" HorizontalAlign="Left" BackColor="White"></PagerStyle>
                <HeaderStyle ForeColor="#235478" Font-Bold="True" Font-Size="12px" BackColor="#D5E7F4"
                    Height="15px"></HeaderStyle>
                <SelectedRowStyle ForeColor="White" Font-Bold="True" BackColor="#669999"></SelectedRowStyle>
                <RowStyle ForeColor="#000066"></RowStyle>
            </asp:GridView>
        </asp:Panel>
        <asp:Panel ID="pnlLogDetails" runat="server">
            <asp:Literal ID="ltrDCCMessage" runat="server" Visible="false" ></asp:Literal>
            <table class="ektronGrid">
                <tr>
                    <td title="Type" class="label" id="cellTypeLabel" runat="server">
                    </td>
                    <td id="cellType" runat="server">
                    </td>
                </tr>
                <tr>
                    <td title="Position" class="label" id="cellPositionLabel" runat="server">
                    </td>
                    <td id="cellPosition" runat="server">
                    </td>
                </tr>
                <tr>
                    <td title="Start Time" class="label" id="cellStartTimeLabel" runat="server">
                    </td>
                    <td id="cellStartTime" runat="server">
                    </td>
                </tr>
                <tr>
                    <td title="End Time" class="label" id="cellEndTimeLabel" runat="server">
                    </td>
                    <td id="cellEndTime" runat="server">
                    </td>
                </tr>
                <tr>
                    <td title="Statistics" class="label" id="cellStatisticsLabel" runat="server">
                    </td>
                    <td id="cellStatistics" runat="server">
                    </td>
                </tr>
                <tr>
                    <td title="Reasons" class="label" id="cellReasonsLabel" runat="server">
                    </td>
                    <td id="cellReasons" runat="server">
                    </td>
                </tr>
                <tr>
                    <td colspan="2">
                        <div id="dvException" style="visibility: hidden">
                            <asp:GridView ID="grdSync" runat="server" AllowPaging="true" PageSize="50" AutoGenerateColumns="False"
                                BorderWidth="1px" BackColor="White" CellPadding="3" BorderStyle="Solid" BorderColor="#CCCCCC"
                                Font-Names="Arial" CssClass="ektronGrid" OnPageIndexChanging="grdSync_PageIndexChanging" >
                                <PagerSettings Visible="true" Mode="Numeric"></PagerSettings>
                                <FooterStyle ForeColor="#000066" BackColor="White"></FooterStyle>
                                <PagerStyle ForeColor="#000066" HorizontalAlign="Left" BackColor="White"></PagerStyle>
                                <HeaderStyle ForeColor="White" Font-Bold="True" Font-Size="12px" BackColor="#006699"
                                    Height="15px"></HeaderStyle>
                                <Columns>
                                    <asp:BoundField HeaderText="TableName" DataField="TableName">
                                        <ItemStyle HorizontalAlign="Left" VerticalAlign="Middle" Width="3%"></ItemStyle>
                                    </asp:BoundField>
                                    <asp:BoundField HeaderText="Stage" DataField="Stage">
                                        <ItemStyle HorizontalAlign="Left" VerticalAlign="Middle" Width="3%"></ItemStyle>
                                    </asp:BoundField>
                                    <asp:BoundField HeaderText="ErrorMessage" DataField="ErrorMessage">
                                        <ItemStyle HorizontalAlign="Left" VerticalAlign="Middle"></ItemStyle>
                                    </asp:BoundField>
                                    <asp:TemplateField HeaderText="RemoteChanges">
                                        <ItemTemplate>
                                            <itemstyle horizontalalign="Left" verticalalign="Middle" width="15%"></itemstyle>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="LocalChanges">
                                        <ItemTemplate>
                                            <itemstyle horizontalalign="Left" verticalalign="Middle" width="15%"></itemstyle>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                </Columns>
                                <SelectedRowStyle ForeColor="White" Font-Bold="True" BackColor="#669999"></SelectedRowStyle>
                                <RowStyle ForeColor="#000066"></RowStyle>
                            </asp:GridView>
                        </div>
                    </td>
                </tr>
            </table>
        </asp:Panel>
    </div>
    </form>
</body>
</html>
