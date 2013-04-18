<%@ Page Language="C#" MasterPageFile="DistributionWizard.master" AutoEventWireup="true" CodeFile="DistributionWizardConfirm.aspx.cs" Inherits="Community_DistributionWizard_DistributionWizardConfirm" Title="Untitled Page" %>
<%@ MasterType VirtualPath="DistributionWizard.master" %>
<%@ Assembly Name='Microsoft.VisualBasic, Version=8.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a'%>


<asp:Content ID="contentConfirmation" ContentPlaceHolderID="cphDistributionWizardContent" Runat="Server">
    <script language="javascript" type="text/javascript">
        var executeUnloadAction = true;
        function ExecWindowCloseAction()
        {
            // This function executes as a handler for the body element onbeforeunload event (see 
		    // master page). 
		    //
		    // If the user cancels distribution via closing the thickbox, page refresh,
		    // or closing the window we need to do some clean up. 
		    //
		    // If we are explicitly unloading the page (e.g. moving to the next step in the
		    // wizard, etc.) the executeUnloadAction flag should be set to false to prevent
		    // the distribution process from being rolled back.
		    
            if( executeUnloadAction )
            {
                ExecHttpRequest(
                    "GET",
                    "DistributionWizardClose.aspx?action=cancel&mode=<%= DistributionModeString %>",
                    false);
            }
        }
    </script>
    <div id="DistributionWizardConfirmationDetails">
        <asp:Literal ID="ltrConfirmationDetails" runat="server"></asp:Literal>
    </div>
    <div id="DistributionWizardFooter"> 
        <asp:Button ToolTip="<%=_messageHelper.GetMessage('btn back')%>" ID="btnBack" Text="Back" runat="server" OnClick="btnBack_Click" OnClientClick="javascript:executeUnloadAction = false;" />&nbsp;<asp:Button ToolTip="<%=_messageHelper.GetMessage('btn next')%>" ID="btnNext" runat="server" Text="Next" Enabled="false" OnClientClick="javascript:executeUnloadAction = false;" />&nbsp;&nbsp;<asp:Button ToolTip="<%=_messageHelper.GetMessage('btn done')%>" ID="btnDone" runat="server" Text="Done" OnClick="btnDone_Click" OnClientClick="javascript:executeUnloadAction = false;" />&nbsp;<asp:Button ToolTip="<%=_messageHelper.GetMessage('btn cancel')%>" ID="btnCancel" runat="server" Text="Cancel" OnClick="btnCancel_Click" OnClientClick="javascript:executeUnloadAction = false;" />
    </div>
</asp:Content>

