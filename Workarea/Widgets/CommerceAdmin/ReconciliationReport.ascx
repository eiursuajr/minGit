<%@ Control Language="C#" AutoEventWireup="true" CodeFile="ReconciliationReport.ascx.cs"
    Inherits="Workarea_Widgets_ReconciliationReport" %>
<div>
    <asp:Literal ID="ltr_period" runat="server" />
</div>
<div class="ektronTopSpace">
    <asp:Label ToolTip="No Records" ID="lblNoRecords" Visible="false" runat="server">
        <asp:Literal ID="ltrlNoRecords" runat="server" /></asp:Label>
</div>
<script type="text/javascript" language="javascript" id="blah">
    function ValidStartDate_10084150444247DD8DEF8D17AE326E5D(startText, endField) {
         var dtformat = $(".hdn_dt_formatclass").val();
         var startdate = $ektron.datepicker.parseDate(startText, dtformat);
        var enddate = $ektron.datepicker.parseDate($ektron("#" + endField).val(), dtformat);
        return (startdate < enddate);
    }
    function ValidEndDate_512AEFA7952244BE901A87670BB2BD6E(startField, endText) {
        var dtformat = $(".hdn_dt_formatclass").val();
        var startdate = $ektron.datepicker.parseDate($ektron("#" + startField).val(), dtformat);
        var enddate = $ektron.datepicker.parseDate(endText, dtformat);
        return (startdate < enddate);
    }
</script>
<input type="hidden" id="hdn_dt_format1" runat="server" class="hdn_dt_formatclass" />
<asp:HiddenField ID="hdn_filter_start" runat="server" />
<asp:HiddenField ID="hdn_filter_end" runat="server" />
<ul id='dateMenu_68AF888415C5452D9A2D1CBC113AD156_start' runat="server">
    <li>
        <div type="text" id="startdatepicker" runat='server'>
        </div>
    </li>
</ul>
<ul id='dateMenu_DD8615035E9E44B7BE30AF0A1E0C9AD4_end' runat="server">
    <li>
        <div type="text" id="enddatepicker" runat='server'>
        </div>
    </li>
</ul>
<asp:Panel ID="pnlData" runat="server">
    <div class="ektronTopSpace">
    </div>
    <div class="ektronPageGrid">
        <asp:DataGrid ID="grdData" runat="server" Width="100%" AutoGenerateColumns="false"
            EnableViewState="False" GridLines="None" CssClass="ektronGrid ektronBorder">
            <HeaderStyle CssClass="title-header" />
            <Columns>
                <asp:TemplateColumn>
                    <ItemTemplate>
                        <%#Util_FormatDate(DataBinder.Eval(Container.DataItem, "CapturedOn"))%>
                    </ItemTemplate>
                </asp:TemplateColumn>
                <asp:TemplateColumn HeaderStyle-HorizontalAlign="Right" ItemStyle-HorizontalAlign="Right">
                    <ItemTemplate>
                        <%#Util_FormatCard(DataBinder.Eval(Container.DataItem, "Last4Digits"))%>
                    </ItemTemplate>
                </asp:TemplateColumn>
                <asp:TemplateColumn HeaderStyle-HorizontalAlign="Right" ItemStyle-HorizontalAlign="Right">
                    <ItemTemplate>
                        <%#Util_FormatType(DataBinder.Eval(Container.DataItem, "PaymentType"))%>
                    </ItemTemplate>
                </asp:TemplateColumn>
                <asp:BoundColumn DataField="TransactionId"></asp:BoundColumn>
                <asp:TemplateColumn HeaderStyle-HorizontalAlign="Right" ItemStyle-HorizontalAlign="Right">
                    <ItemTemplate>
                        <%#Util_FormatCurrency(DataBinder.Eval(Container.DataItem, "PaymentTotal"), DataBinder.Eval(Container.DataItem, "Currency"))%>
                    </ItemTemplate>
                </asp:TemplateColumn>
                <asp:TemplateColumn HeaderStyle-HorizontalAlign="Right" ItemStyle-HorizontalAlign="Right">
                    <ItemTemplate>
                        <%#Util_FormatVoided(DataBinder.Eval(Container.DataItem, "VoidedDate"))%>
                    </ItemTemplate>
                </asp:TemplateColumn>
            </Columns>
        </asp:DataGrid>
    </div>
</asp:Panel>
