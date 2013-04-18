<%@ Page Language="C#" AutoEventWireup="true" CodeFile="PaymentReports.aspx.cs" Inherits="Commerce_reporting_PaymentReports" %>
<%@ Register Src="../../Widgets/CommerceAdmin/ReconciliationReport.ascx" TagName="KPI" TagPrefix="Ektron" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
    <head id="Head1" runat="server">
        <meta content="text/html; charset=UTF-8" http-equiv="content-type" />
        <title id="titleTag" runat="server"></title>
        <link type="text/css" rel="Stylesheet" href="../../Personalization/css/ektron.personalization.css" />
        <script type="text/javascript" language="javascript">
            Ektron.ready(function() {
                $ektron("div.EktronPersonalization div.widget > div.content").show();
            });
        </script>
        <style type="text/css" >
            div.EktronPersonalization {min-width:880px;}
        </style>
        <asp:literal id="StyleSheetJS" runat="server" />
    </head>
    <body>
        <form id="form1" runat="server">
    	<div id="dhtmltooltip"></div>
    	<div class="ektronPageHeader">
    	    <div class="ektronTitlebar" id="divTitleBar" runat="server"></div>
    	    <div class="ektronToolbar" id="divToolBar" runat="server"></div>
    	</div>

    	<div class="ektronPageContainer ektronPageInfo">

            <div class="EktronPersonalization report-container">
                <div class="widget">
                    <div class="header">
                        <div class="buttons">
                            <h4>
                                <span><asp:Literal ID="litTitle" runat="server" /></span>
                            </h4>
                        </div>
                    </div>
                    <div class="content" >
                        <Ektron:KPI ID="kpi1" runat="server" />
                    </div>
                </div>
            </div>
            <div style="position: relative;" >
                <div style="position: absolute; left:-1000px; top: -100px;" >
     	            <asp:linkbutton ToolTip="Test" id="Test" runat="server" text="." onclick="Test_Click" visible="true" />
                </div>
            </div>
        
        </div>
        </form>
    </body>
</html>
