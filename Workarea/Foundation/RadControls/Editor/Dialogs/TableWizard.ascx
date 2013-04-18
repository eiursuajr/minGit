<%@ Control Language="c#" AutoEventWireup="false" Codebehind="TableWizard.ascx.cs" Inherits="Ektron.Telerik.WebControls.EditorDialogControls.TableWizard" TargetSchema="http://schemas.microsoft.com/intellisense/ie5" %>
<%@ Register TagPrefix="telerik" NameSpace="Ektron.Telerik.WebControls.EditorControls" Assembly="Ektron.RadEditor"%>
<%@ Register TagPrefix="telerik" TagName="TabControl" Src="../Controls/TabControl.ascx"%>
<%@ Register TagPrefix="telerik" TagName="TablePropertiesControl" Src="../Controls/TablePropertiesControl.ascx"%>
<%@ Register TagPrefix="telerik" TagName="CellPropertiesControl" Src="../Controls/CellPropertiesControl.ascx"%>
<%@ Register TagPrefix="telerik" TagName="AccessibleTable" Src="../Controls/AccessibleTable.ascx"%>
<%@ Register TagPrefix="telerik" TagName="TableDesignControl" Src="../Controls/TableDesignControl.ascx"%>

<div id="MainContainer">
    <div class="Ektron_DialogTabstrip_Container">
		<telerik:tabcontrol id="TabHolder" runat="server" ResizeControlId="MainContainer">
			<telerik:tab elementid="TabbedDesigner" onclientclick="SwitchTableDesigner();" selected="True" text="<script>localization.showText('Tab1HeaderText');</script>" image="Dialogs/TabIcons/TableTab1.gif"/>
			<telerik:tab elementid="TabbedTableProperties" onclientclick="SwitchTableProperties();" text="<script>localization.showText('Tab2HeaderText');</script>" image="Dialogs/TabIcons/TableTab2.gif"/>
			<telerik:tab elementid="TabbedCellProperties" onclientclick="SwitchCellProperties();" text="<script>localization.showText('Tab3HeaderText');</script>" image="Dialogs/TabIcons/TableTab3.gif"/>
			<telerik:tab elementid="Tabbed508" onclientclick="SwitchAccessibleTable();" text="<script>localization.showText('Tab4HeaderText');</script>" image="Dialogs/TabIcons/TableTab4.gif"/>
		</telerik:tabcontrol>
	</div> 
	<div class="Ektron_DialogTabBodyContainer">
		<table id="TabbedDesigner" width="346" cellpadding="0" cellspacing="0">
			<tr>
				<td>
					<telerik:TableDesignControl
						id="theTableDesignControl"
						runat="server"/>
				</td>
			</tr>
		</table>
		<table id="TabbedTableProperties" border="0" cellpadding="0" cellspacing="0">
			<tr>
				<td valign="top">
					<telerik:TablePropertiesControl
						id="theTablePropertiesControl"
						runat="server"/>
				</td>
			</tr>
		</table>
		<table id="TabbedCellProperties" border="0"  cellpadding="0" cellspacing="0">
			<tr>
				<td>
					<span id="<%=this.ClientID%>_CellPropertiesPreviewTableHolder"></span>
					<telerik:CellPropertiesControl
						id="theCellPropertiesControl"
						runat="server"/>
				</td>
			</tr>
		</table>
		<table id="Tabbed508" border="0"  cellpadding="0" cellspacing="0"> 
			<tr>
				<td valign="top">
					<telerik:accessibletable
						id="theAccessibleTable"
						runat="server"/>
				</td>
			</tr>
		</table>
	</div>
	<div class="Ektron_DialogButtonContainer">
<!-- Ektron Editor starts -->
		<button class="Button" title="Ok" onclick="UpdateTable();" type=button>
<!-- Ektron Editor ends -->
			<script>localization.showText('OK');</script>
		</button>
        <span class="Ektron_LeftSpaceSmall"></span>						
		<button class="Button"  title="Cancel" onclick="javascript:CancelChanges();" type=button>
			<script>localization.showText('Cancel');</script>
		</button>
	</div>
	
<script language="javascript">
	function SwitchCellProperties()
	{
		<%=this.ClientID%>.InitCellProperties();
	}

	function SwitchTableProperties()
	{
		<%=this.ClientID%>.InitTableProperties();
	}
	
	function SwitchAccessibleTable()
	{
		<%=this.ClientID%>.InitAccessibleTable();
	}
	
	function SwitchTableDesigner()
	{
		if (!<%=this.ClientID%>)
		{
			InitControl();
		}
		<%=this.ClientID%>.InitDesigner();
	}
	
	function CancelChanges()
	{
		<%=this.ClientID%>.RestoreOriginalTable();
		CloseDlg();
	}

	var <%=this.ClientID%>;
	function InitControl()
	{
		if (!<%=this.ClientID%>)
		{
			<%=this.ClientID%> = new TableWizard('<%=this.ClientID%>', <%=this.theTableDesignControl.ClientID%>, <%=this.theTablePropertiesControl.ClientID%>, <%=this.theCellPropertiesControl.ClientID%>, <%=this.theAccessibleTable.ClientID%>);
			var args = GetDialogArguments();
			<%=this.ClientID%>.Initialize(args.tableToModify, args.CssClasses, args.CellCssClasses, args.EditorObj, args.ColorsArray, args.CanAddCustomColors);
		}
	}

	AttachEvent(window, "load", InitControl);
	
	//Ektron Editor starts   
	function SetAccessibleTabFocus(ShowTblTab, ShowCellTab)
	{
	    document.getElementById("TabbedDesigner").style.display = "none";
        document.getElementById("TabbedTableProperties").style.display = "none";
	    if (ShowTblTab)
	    {
	        if (window.netscape)
	        {
	            document.getElementById("Tabbed508").style.display = "table-row";//block
	        }
	        else
	        {
	            document.getElementById("Tabbed508").style.display = "";
	        }
	        document.getElementById("TabbedCellProperties").style.display = "none";
	        TabHolder.SetTabSelected(3);
	    }
	    else if (ShowCellTab)
	    {
            if (window.netscape)
	        {
                document.getElementById("TabbedCellProperties").style.display = "table-row";//block
            }
            else
            {
                document.getElementById("TabbedCellProperties").style.display = "";
            }
            document.getElementById("Tabbed508").style.display = "none";
            TabHolder.SetTabSelected(2);
        }
	}
	
	function UpdateTable()
	{
	    var objTbl = <%=this.ClientID%>; //this is a TableWizard JavaScript object.
	    var objCaption = document.getElementById("TXT_CAPTION_<%=this.ClientID%>_theAccessibleTable");
	    var objSummary = document.getElementById("TXT_SUMMARY_<%=this.ClientID%>_theAccessibleTable");
        var objHeaderRows = document.getElementById("TXT_HEAD_ROWS_<%=this.ClientID%>_theAccessibleTable");
        var objHeaderCols = document.getElementById("TXT_HEAD_COLS_<%=this.ClientID%>_theAccessibleTable");
	    var bUpdateTbl = true;
        var paramList = EkUtil_parseQuery();
        if (typeof paramList.AccessChecks != "undefined")
        {
            var AccessChecks = paramList.AccessChecks.toLowerCase();
            if (AccessChecks != "none")
            {
                var bTblAccessFldBlank = false;		  
                var bCellAccessFldBlank = false;
                var bTblAccessFldValid = true;
                var bCellAccessFldValid = true;
                SetAccessibleTabFocus(true, false);
                var iHeaderRows = parseInt(objHeaderRows.value, 10);
                var iHeaderCols = parseInt(objHeaderCols.value, 10);
                // #52957: Row and column headers are used to identify for data tables.  Caption and Summary are required only if it is data tables. 
                // They are not required if it is markup tables.
                if ("" == objCaption.value || "" == objSummary.value)
                {
                    bTblAccessFldBlank = true;
                }
                if ((iHeaderRows > 0 || iHeaderCols > 0) && true == bTblAccessFldBlank) // Data Table Accessibility Required field(s) is blank.
                {
                    bTblAccessFldValid = false;
                }
                // abbreviation is required for TH cells when categories is required for TD cells.
                //if ("" == objCellTab.abbreviationBox.GetValue() || "" == objCellTab.categoriesBox.GetValue())
                //{
                //    bCellAccessFldBlank = true;
                //}
                if (false == bTblAccessFldValid || false == bCellAccessFldValid)
                {
                    switch (AccessChecks)
                    {
                        case "warn":
                            if (confirm("The content is not accessibility compliance. Would you like to correct it?"))//TODO:localization["AccessChecksWarn"]
                            {
                                SetAccessibleTabFocus(bTblAccessFldBlank, bCellAccessFldBlank);
                                bUpdateTbl = false; //TODO: set Accessibility Required field in focus.
                            }
                            else
                            {
                                <%=this.ClientID%>.RestoreOriginalTable();
                                bUpdateTbl = true;
                            }
                            break;
                        case "enforce":
                            alert("Please fill in the accessibility compliance field(s)"); //TODO:localization["AccessChecksEnforce"]
                            SetAccessibleTabFocus(bTblAccessFldBlank, bCellAccessFldBlank);
                            bUpdateTbl = false;
                            break;
                        case "none":
                        default:
		                    bUpdateTbl = true;
		                    break;
		            }
		        }
            }
        } 
        if (true == bUpdateTbl)
        {
	        <%=this.ClientID%>.InsertTable();
        }
	}
	//Ektron Editor ends
</script>