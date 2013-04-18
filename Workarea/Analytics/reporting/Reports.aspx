<%@ Page Language="C#" AutoEventWireup="true" CodeFile="Reports.aspx.cs" Inherits="Analytics_Reporting_Reports" %>
<%@ Register TagPrefix="analytics" TagName="Toolbar" Src="../controls/AnalyticsToolbar.ascx" %>
<%@ Register TagPrefix="analytics" TagName="Report" Src="Report.ascx" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title id="pagetitle" runat="server"></title>
    <link type="text/css" rel="Stylesheet" href="../../Personalization/css/ektron.personalization.css" />
<script type="text/javascript" language="javascript">
    Ektron.ready(function() {
        $ektron("div.EktronPersonalization div.widget > div.content").show();
        $ektron("table.ektronGrid").show();
    });
    
</script>
<script type="text/javascript" language="javascript">
        $(document).ready(function() {
            // hook ASP.NET Ajax begin and end events, to show busy signal when update panel makes a callback:
            if ("undefined" != typeof Sys.WebForms.PageRequestManager.getInstance) {
                Sys.WebForms.PageRequestManager.getInstance().add_beginRequest(BeginCallbackHandler);
                Sys.WebForms.PageRequestManager.getInstance().add_endRequest(EndCallbackHandler);
            }
        });
        
        function BeginCallbackHandler() {
            $(".UpdateProgressContainer").show();
        }

        function EndCallbackHandler() {
            $(".UpdateProgressContainer").hide();
        }
</script>
<style type="text/css" >
    .UpdateProgressContainer { position: absolute; top: 14em; left: 12em; border: solid 1px #D7E0E7; margin: 1em; padding: 2em; background-color: #E7F0F7; z-index:1; }
    .EktronPersonalization .workareaReportFixedArea { min-width: 595px;}
    div.EktronPersonalization div.widget div.ektronPageContainer {top: 0px !important}
</style>
</head>
<body>
    <form id="form1" runat="server">
    <div class="UpdateProgressContainer" style="display: none;">
	    <h2><asp:Literal ID="litLoadingMessage" runat="server" /></h2>
	</div>
    
    <asp:ScriptManager ID="ScriptManager1" runat="server" />
    <div class="EktronPersonalization" >
    
        <div class="widget" style="position: static; top: 0px !important; left: 0px !important; display: block !important; border: 0; margin: 0;">
            <div class="ektronPageContainer">
                <div class="ektronTitlebar" style="min-width: 590px;">
                    <table>
                        <tr><td class="AnalyticsReportTitlebar">
                            <asp:Literal ID="litTitle" runat="server" />
                        </td></tr>
                    </table>
                </div>
                <div class="content workareaReportFixedArea" >
			        <asp:UpdatePanel ID="UpdatePanel1" ChildrenAsTriggers="false" UpdateMode="Conditional" runat="server">
				        <%--<Triggers>
					        <asp:AsyncPostBackTrigger ControlID="AnalyticsToolbar" EventName="SelectionChanged" />
				        </Triggers>--%>
			           <ContentTemplate>
			                <analytics:Toolbar ID="AnalyticsToolbar" OnDateFormatError="DateFormatError" runat="server" />
					        <asp:Panel ID="ErrorPanel" runat="server" Visible="false" EnableViewState="false">
						        <div class="AnalyticsErrorMessage"><asp:Literal ID="litErrorMessage" runat="server" EnableViewState="false" /></div>
					        </asp:Panel>
					        <div>
					            <analytics:Report ID="AnalyticsReport" LineChartWidth="560px" runat="server"  />
					        </div>
				        </ContentTemplate>
			        </asp:UpdatePanel>
                </div>
            </div>
        </div>
    </div>
    </form>
</body>
</html>
