<%@ Control Language="C#" AutoEventWireup="true" CodeFile="Trend.ascx.cs" Inherits="Analytics_reporting_Trend" %>
<%@ Register TagPrefix="ektron" TagName="DateRangePicker" Src="../../controls/generic/date/DateRangePicker.ascx" %>
<%@ Register TagPrefix="analytics" TagName="SiteSelector" Src="../controls/SiteSelector.ascx" %> 

<asp:Label ID="lblNoRecords" Visible="false" runat="server"><asp:literal ID="ltrlNoRecords" runat="server" /></asp:Label>
<style type="text/css">
div.bar
{
	display: inline;
	float: left;
	height: 1.1em;
	margin-right: 0.5em;
	margin-top: 2px;
}
div.segment0, div.segment1
{
	background: #1A87D5;
}
div.segment2
{
	background: #FF9900;
}
div.segment3
{
	background: #52B432;
}
div.segment4
{
	background: #EDED01;
}
</style>  
    
<asp:Panel ID="pnlData" runat="server">  

    <div class="ektronPageGrid analyticsTrend">
        <div>
            <div class="InnerRegion" >
                <asp:Panel ID="ErrorPanel" runat="server" Visible="false">
                    <div class="AnalyticsErrorMessage"><asp:Literal ID="litErrorMessage" runat="server" /></div>
                </asp:Panel>
                <asp:Literal ID="litCssTweaks" runat="server" />
                <h3 class="AnalyticsReportTitle" ID="htmReportTitle" runat="server" enableviewstate="false"></h3>
                <h4 id="AnalyticsReportDateRangeDisplay" runat="server" class="AnalyticsReportDateRangeDisplay" visible="false" enableviewstate="false" ></h4>
                <h4 class="AnalyticsReportSubtitle" ID="htmReportSubtitle" runat="server" visible="false" enableviewstate="false"></h4>
                <p class="AnalyticsReportSummary" ID="htmReportSummary" runat="server" enableviewstate="false"></p>
                <asp:GridView ID="grdData" 
                    runat="server" 
                    Width="100%"
                    AutoGenerateColumns="false"         
                    EnableViewState="False"
                    GridLines="None"
                    CssClass="ektronGrid ektronBorder"
                    ShowHeader="false"
                    >
                    <AlternatingRowStyle CssClass="alternatingrowstyle" />
                    <Columns>
                        <asp:TemplateField ItemStyle-HorizontalAlign="Right" ItemStyle-Width="25%">
                            <ItemTemplate>
                                <%# Eval("Date", "{0:D}") %>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField>
                            <ItemTemplate>
                                <%# Util_ShowValue(Eval("Value"), DataBinder.Eval(Container, "DataItemIndex"))%>
                            </ItemTemplate>
                        </asp:TemplateField>
                    </Columns>
                </asp:GridView>
                <asp:GridView ID="grdSegmentData" 
                    runat="server" 
                    Width="100%"
                    AutoGenerateColumns="false"         
                    EnableViewState="False"
                    GridLines="None"
                    CssClass="ektronGrid ektronBorder"
                    ShowHeader="false"
                    Visible="false"
                    >
                    <Columns>
                        <asp:TemplateField ItemStyle-HorizontalAlign="Right" ItemStyle-Width="25%">
                            <ItemTemplate>
                                <%# Eval("Title")%>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField>
                            <ItemTemplate>
                                <%# Util_ShowValue(Eval("Value"), DataBinder.Eval(Container, "DataItemIndex"))%>
                            </ItemTemplate>
                        </asp:TemplateField>
                    </Columns>
                </asp:GridView>
            </div>
        </div>
    </div>
    
</asp:Panel>