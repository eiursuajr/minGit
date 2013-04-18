<%@ Page Language="C#" AutoEventWireup="true" CodeFile="KeyPerformanceIndicators.aspx.cs"
    Inherits="Commerce_reporting_KeyPerformanceIndicators" %>

<%@ Register Src="../../Widgets/CommerceAdmin/KeyPerformanceIndicators.ascx" TagName="KPI"
    TagPrefix="Ektron" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title id="titleTag" runat="server"></title>
    <style type="text/css">
        div.EktronPersonalization { min-width: 880px; }
        .KeyPerformanceIndicatorContainer .KPIHideBusyImage {display: none;}
        .KeyPerformanceIndicatorContainer .ektronPageGrid .grdData { min-width: 560px !important; }
        .ektronGrid { display: block !important; }
    </style>
</head>
<body>
    <form id="form1" runat="server">
    <asp:ScriptManager ID="sm1" runat="server" />
    <div>
        <asp:UpdatePanel ID="upKeyPerformanceIndicators" runat="server" UpdateMode="Conditional"
            ChildrenAsTriggers="true">
            <ContentTemplate>
                <asp:Panel ID="headerPanel" Visible="false" runat="server">
                    <div class="ektronPageHeader">
                        <div class="ektronTitlebar" id="divTitleBar" runat="server"></div>
                        <div class="ektronToolbar" id="divToolBar" runat="server"></div>
                    </div>
                </asp:Panel>
                <asp:Panel ID="contentPanel" Visible="true" runat="server">
                    <div class="content">
                        <div class="KeyPerformanceIndicatorContainer" style="position: static; top: 0; left: 0;">
                                <div style="background-color: transparent; position: absolute; top: 26px; left: 300px;">
                                    <asp:Image ToolTip="Busy" ID="imgBusyImage" runat="server" CssClass="KPIHideBusyImage" />
                                </div>
                            <div class="ektronPageGrid kpi">
                                <asp:PlaceHolder ID="phTableContainer" runat="server" />
                            </div>
                        </div>
                        <div style="display: none;">
                            <asp:LinkButton ToolTip="Submit" ID="btnForSubmit" runat="server" OnClick="btnForSubmit_Click" />
                        </div>
                    </div>
                </asp:Panel>
                <asp:Panel ID="errorPanel" Visible="false" runat="server">
                    <asp:Literal ID="litError" runat="server" />
                </asp:Panel>
            </ContentTemplate>
        </asp:UpdatePanel>
    </div>
    </form>
</body>
</html>
