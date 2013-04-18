<%@ Page Language="C#" AutoEventWireup="true" CodeFile="Overview.aspx.cs" Inherits="Analytics_reporting_Overview" %>
<%@ Register TagPrefix="analytics" TagName="Toolbar" Src="../controls/AnalyticsToolbar.ascx" %>
<%@ Register TagPrefix="analytics" TagName="Report" Src="Report.ascx" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
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
    .analyticsOverview .InnerRegion .reportArea {/*padding: 5em;*/}
    .EktronPersonalization .workareaReportFixedArea { min-width: 950px; overflow: hidden;}
    div.EktronPersonalization div.widget div.ektronPageContainer {top: 0px !important}
</style>
</head>
<body>
    <div class="UpdateProgressContainer" style="display: none;">
	    <h2><asp:Literal ID="litLoadingMessage" runat="server" /></h2>
	</div>
    <form id="form1" runat="server">
    
    <asp:ScriptManager ID="ScriptManager1" runat="server" />
    
    <div class="EktronPersonalization analyticsOverview">
    <div class="widget" style="position: static; top: 0px !important; left: 0px !important; display: block !important; border: 0; margin: 0;">
        <div class="widget" style="position: static; top: 0px !important; left: 0px !important; display: block !important; border: 0; margin: 0;">
        <div class="ektronPageContainer">
			<div class="ektronTitlebar" style="min-width: 945px;">
				<table>
                    <tr><td class="AnalyticsReportTitlebar" >
						<asp:Literal ID="litTitle" runat="server" />
					</td></tr>
				</table>
			</div>
			    <div class="content workareaReportFixedArea">
				    <asp:UpdatePanel ID="UpdatePanel1" ChildrenAsTriggers="false" UpdateMode="Conditional" runat="server">
					  <%--  <Triggers>
						    <asp:AsyncPostBackTrigger ControlID="AnalyticsToolbar" EventName="SelectionChanged" />
					    </Triggers>--%>
				       <ContentTemplate>
						    <analytics:Toolbar ID="AnalyticsToolbar" OnDateFormatError="DateFormatError" runat="server" />
						    <asp:Panel ID="ErrorPanel" runat="server" Visible="false" EnableViewState="false">
							    <div class="AnalyticsErrorMessage"><asp:Literal ID="litErrorMessage" runat="server" EnableViewState="false" /></div>
						    </asp:Panel>
						    <div class="reportArea">
						    <table width="100%" border="0" cellpadding="0" cellspacing="0" >
							    <tbody>
								    <tr>
									    <td style="vertical-align: top; padding-right: 20px;">
										    <analytics:Report ID="AnalyticsReport1" Report="Direct" LineChartWidth="410px" runat="server" />
									    </td>
									    <td style="vertical-align: top;">
										    <analytics:Report ID="AnalyticsReport2" Report="TopContent" View="Percentage" ShowPieChart="false" runat="server" />
									    </td>
								    </tr>
							    </tbody>
						    </table>
						    </div>
					    </ContentTemplate>
				    </asp:UpdatePanel>
				</div>
		    </div>
		</div>
    </div>
    </div>

    </form>
</body>
</html>
