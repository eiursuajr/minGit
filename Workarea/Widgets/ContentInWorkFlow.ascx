<%@ Control Language="C#" AutoEventWireup="true" CodeFile="ContentInWorkFlow.ascx.cs"
    Inherits="Workarea_Widgets_ContentInWorkFlow" %>
<div style="overflow: hidden;">
    <%--     <link href="../csslib/ektronCss.ashx?id=AnalyticsReportCss+EktronPersonalziationCss+EktronWorkareaPersonalziationCss+EktronWorkareaCss+EktronFixedPositionToolbarCss+EktronModalCss"
     rel="Stylesheet" type="text/css" />--%>
    <asp:MultiView ID="ViewSet" runat="server" ActiveViewIndex="0">
        <asp:View ID="View" runat="server">
            <asp:Label ToolTip="No Records" ID="lblNoRecords" Visible="false" runat="server">
                <asp:Literal ID="ltrlNoRecords" runat="server" /></asp:Label><asp:Panel ID="pnlData"
                    runat="server">
                    <asp:LinkButton ToolTip="View All" ID="lnkViewAll" runat="server">
                        <asp:Literal ID="ltrlViewAll" runat="server" /></asp:LinkButton><div class="ektronTopSpace">
                        </div>
                    <div class="ektronTopSpace">
                    </div>
                    <div class="ektronPageGrid" style="overflow:auto;">
                        <asp:DataGrid ID="grdData" runat="server" Width="100%" AutoGenerateColumns="False"
                            EnableViewState="False" GridLines="None" CssClass="ektronGrid ektronBorder">
                            <HeaderStyle CssClass="title-header" />
                        </asp:DataGrid>
                    </div>
                </asp:Panel>
        </asp:View>
        <asp:View ID="uxEdit" runat="server">
            <div id="<%=ClientID%>_edit">
                <!-- You Need To Do ..............................  -->
                <br />
                No. of Days: <asp:TextBox ToolTip="Days Limit" ID="uxDaysLimit" runat="server" Style="width: 40%"> </asp:TextBox><br />
                <br />
                <br />
                <!-- End To Do ..............................  -->
                <asp:Button ToolTip="Cancel" ID="uxCancelButton" runat="server" Text="Cancel" OnClick="CancelButton_Click" />
                &nbsp;&nbsp; <asp:Button ToolTip="Save" ID="uxSaveButton" runat="server" Text="Save" OnClick="SaveButton_Click" />
            </div>
        </asp:View>
    </asp:MultiView>
</div>
