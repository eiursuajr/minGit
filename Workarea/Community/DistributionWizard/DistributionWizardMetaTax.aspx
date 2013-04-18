<%@ Page Language="C#" MasterPageFile="DistributionWizard.master" AutoEventWireup="true" CodeFile="DistributionWizardMetaTax.aspx.cs" Inherits="Community_DistributionWizard_DistributionWizardMetaTax" Title="Distribution Wizard" %>
<%@ Register Src="Metadata.ascx" TagName="Metadata" TagPrefix="uc2" %>
<%@ Register Src="SelectTaxonomy.ascx" TagName="SelectTaxonomy" TagPrefix="uc1" %>
<%@ Assembly Name='Microsoft.VisualBasic, Version=8.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a'%>
<%@ MasterType VirtualPath="DistributionWizard.master" %>

<asp:Content ID="contentMetaTax" ContentPlaceHolderID="cphDistributionWizardContent" Runat="Server">
    
    <script type="text/javascript">
        
        var executeUnloadAction = true;
        
        var g_initialPaneToShow = "tabMetadata";
		
		function ShowTab(tabID) 
		{			
			// For Netscape/FireFox: Objects appear to get destroyed when "display" is set to "none" and re-created 
			// when "display" is set to "block." Instead will use the appropriate style-sheet 
			// class to move the unselected items to a position where they are not visible.
			// For IE: If the ActiveX control is display="none" programmatically rather than by user click,
			// the ActiveX control seems to uninitialize, for example, the DHTML Edit Control (DEC) is gone.
			var aryTabs = ["tabMetadata", "tabTaxonomy"];
			
			for (var i = 0; i < aryTabs.length; i++) 
			{
				SetPaneVisible(aryTabs[i], false);
				SetPaneVisible(aryTabs[i], (tabID == aryTabs[i]));
			}
			g_initialPaneToShow = tabID; // remember which tab is selected if editor is reloaded
		}
		
		function SetPaneVisible(tabID, bVisible)
		{
			var objElem = null;
			objElem = document.getElementById(tabID);
			if (objElem != null) 
			{
				objElem.className = (bVisible ? "tabActivated" : "tabDeactivated");
			}
			objElem = document.getElementById("_" + tabID);
			if (objElem != null) 
			{
				// For Safari on the Mac (to fix Ephox Editor issues), 
				// the actual class names are overridden in the code behind
				// (uses special classes when Safari on the Mac is detected):
				objElem.className = (bVisible ? "visibleTabContents" : "hiddenTabContents");
			}
		}
		
		function ValidateInput()
		{	    
		    // Validate meta is made available through jfunc.js   
		    var metadataValid = ValidateMeta('aspnetForm');
		    
		    var taxonomyRequired = "<%= TaxonomyRequired %>";
		    var taxonomyValid = true;
		    if( taxonomyRequired.toLowerCase() == "true" )
		    {
		        if ( (typeof(categoryIsSelected)).toLowerCase() == 'function')
		        {
		            // categoryIsSelected is made available through the
		            // SelectTaxonomy control.
		            if( !categoryIsSelected() )
		            {
		                // Since we're halting the postback at this point, we
		                // need to reset the executeUnloadAction flag
		                executeUnloadAction = true; 
		            
		                taxonomyValid = false;
		                alert('The folder requires that at least one taxonomy category is selected.');   
		            }
	            }
		    }
		    
		    return metadataValid && taxonomyValid;
		}
		
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
    
    <div class="DistributionWizardErrorMessage"><asp:Label ToolTip="Error" ID="lblErrorMessage" runat="server"></asp:Label></div>
    <div id="DistributionWizardMetaTax">
        <table class="tabs" cellspacing="0">
            <tr>
                <td class="tabSpacer">&nbsp;</td>
                <td id="tabMetadata" class="tabActivated" onclick="ShowTab(this.id);">Metadata</td>
                <td class="tabSpacer">&nbsp;</td>
                <td id="tabTaxonomy" class="tabDeactivated" onclick="ShowTab(this.id);">Category</td>
                <td class="tabFiller">&nbsp;</td>
            </tr>
        </table>
        <div id="_tabMetadata" class="tabActivated">
            <asp:Label ToolTip="Metadata Message" ID="lblMetadataMessage" runat="server"></asp:Label>
            <uc2:Metadata ForceNewWindow="true" id="inputMetadata" runat="server" />
        </div>
        <div id="_tabTaxonomy" class="tabDeactivated">
            <asp:Label ToolTip="Taxonomy Message" ID="lblTaxonomyMessage" runat="server"></asp:Label>
            <uc1:SelectTaxonomy ID="selectTaxonomy" runat="server" />
        </div>
    </div>
    <div id="DistributionWizardFooter"> 
        <asp:Button ToolTip="Back" ID="btnBack" Text="Back" runat="server" OnClick="btnBack_Click" OnClientClick="javascript:executeUnloadAction = false;" />&nbsp;<asp:Button ToolTip="Next" ID="btnNext" runat="server" Text="Next" OnClick="btnNext_Click" OnClientClick="javascript:executeUnloadAction = false; return ValidateInput();" />&nbsp;&nbsp;<asp:Button ToolTip="Done" ID="btnDone" runat="server" Text="Done" Enabled="false" />&nbsp;<asp:Button ToolTip="Cancel" ID="btnCancel" runat="server" Text="Cancel" OnClick="btnCancel_Click" OnClientClick="javascript:executeUnloadAction = false;" />
    </div>
    
    <script type="text/javascript" language="javascript">
        ShowTab("tabMetadata");
    </script>
</asp:Content>

