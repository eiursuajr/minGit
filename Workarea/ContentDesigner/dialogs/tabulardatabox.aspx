<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<%@ Page language="c#" CodeFile="tabulardatabox.aspx.cs" Inherits="Ektron.ContentDesigner.Dialogs.TabularDataBox" AutoEventWireup="false" %>
<%@ Register TagPrefix="ek" TagName="FieldNameControl" Src="ucFieldName.ascx" %>
<%@ Register TagPrefix="ek" TagName="FieldUseControl" Src="ucFieldUse.ascx" %>
<%@ Register TagPrefix="ek" TagName="FieldAllowControl" Src="ucFieldAllow.ascx" %>
<%@ Register TagPrefix="ek" TagName="FieldDialogButtons" Src="ucFieldDialogButtons.ascx" %>
<%@ Register TagPrefix="ek" TagName="FieldAdvancedControl" Src="ucFieldAdvanced.ascx" %>
<%@ Register TagPrefix="ek" TagName="FieldMinMax" Src="ucFieldMinMax.ascx" %>
<%@ Register TagPrefix="ek" TagName="FieldXPath" Src="ucFieldXPath.ascx" %>
<%@ Register TagPrefix="radTS" Namespace="Telerik.WebControls" Assembly="RadTabStrip.NET2" %>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
<meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
<title id="Title" runat="server">Tabular Data Box</title>
</head>
<body class="dialog">
<form id="Form1" runat="server">   
    
    <div class="Ektron_DialogTabstrip_Container">
    <radTS:RadTabStrip id="RadTabStrip1" runat="server" MultiPageID="RadMultiPage1" SelectedIndex="0" ReorderTabRows="true" 
			OnClientTabSelected="ClientTabSelectedHandler" SkinID="TabstripDialog">
        <Tabs>
            <radTS:Tab ID="General" Text="General" Value="General" />
            <radTS:Tab ID="Advanced" Text="Advanced" Value="Advanced" />
            <radTS:Tab ID="Relevance" Text="Relevance" Value="Relevance" />
        </Tabs>
    </radTS:RadTabStrip> 
    </div>
    <div class="Ektron_Dialog_Tabs_BodyContainer">
        <radTS:RadMultiPage id="RadMultiPage1" runat="server" SelectedIndex="0">
            <radTS:PageView id="Pageview1" runat="server"> 
	            <table width="100%">
		            <ek:FieldNameControl ID="Name" IndexedEnabled="false" runat="server" />
		            <tr>
		                <td colspan="3">
		                    <table style="width:100%" class="Ektron_TopSpaceVeryVerySmall">
	                        <tr>
	                            <td style="width:50%">
						            <ek:FieldUseControl ID="Use" runat="server" />
		                        </td>
		                        <td>&nbsp;&nbsp;</td>
	                            <td style="width:50%">
						            <ek:FieldAllowControl ID="Allow" runat="server" />
		                        </td>
	                        </tr>
		                    </table>
		                </td>
		            </tr>
		            <tr>
		                <td colspan="3">
		                <fieldset class="Ektron_TopSpaceVeryVerySmall Ektron_BottomSpaceSmall">
				            <legend id="lblRows" runat="server" title="Rows">Rows</legend>
                            <div>
                                <table>
                                <tr>
                                    <td><label title="Row Display Name" for="txtRowDispName" class="Ektron_StandardLabel" id="lblRowDispName" runat="server">Row Display Name:</label></td>
                                    <td><input title="Enter Row Display Name here" type="text" name="txtRowDispName" id="txtRowDispName" runat="server" class="Ektron_NarrowTextBox RadETextBox" value="Field 2" onblur="ekFieldNameControl.keepNameInSync(m_oRowFieldName);" /></td>
                                </tr>
                                <tr>
                                    <td><label title="Row Name" for="txtRowName" class="Ektron_StandardLabel" id="lblRowName" runat="server">Row Name:</label></td>
                                    <td><input title="Enter Row Name here" type="text" name="txtRowName" id="txtRowName" runat="server" value="Field2" class="Ektron_NarrowTextBox" /></td>
                                </tr>
                                <ek:FieldMinMax runat="server" />
                                </table>
                            </div>
		                </fieldset>
		                </td>
		            </tr>
		            <tr>
		                <td><label title="Columns" for="txtNumCols" class="Ektron_StandardLabel" id="lblColumns" runat="server">Columns:</label></td>
		                <td colspan="2"><input title="Enter the Number of Columns here" type="text" name="txtNumCols" id="txtNumCols" value="2" class="RadETextBox" size="4" /></td>
		            </tr>
		            <tr>
		                <td><label title="Caption" for="txtCaption" class="Ektron_StandardLabel" id="lblCaption" runat="server">Caption:</label></td>
		                <td colspan="2"><input title="Enter Caption here" type="text" name="txtCaption" id="txtCaption" runat="server" value="Fields" class="Ektron_StandardTextBox" /></td>
		            </tr>
		        </table>
	        </radTS:PageView>
            <radTS:PageView id="Pageview2" runat="server" >
                <table width="100%">
                    <ek:FieldAdvancedControl ID="fieldAdvanced" NodeTypeVisible="true" TypeAttributeEnabled="false" TypeContentEnabled="false" runat="server" />
                </table> 
            </radTS:PageView>
            <radTS:PageView id="Pageview3" runat="server" >
                <ek:FieldXPath id="RelevanceXPath" FieldType="Relevant" runat="server" />
            </radTS:PageView>
        </radTS:RadMultiPage>
	</div>
	
    <div class="Ektron_Dialogs_LineContainer">
        <div class="Ektron_TopSpaceSmall"></div>
        <div class="Ektron_StandardLine"></div>
    </div>	
	
	<ek:FieldDialogButtons ID="btnSubmit" OnOK="return insertField();" runat="server" />	
</form> 
<script language="javascript" type="text/javascript">
<!--
    Ektron.ready(function(){
        initField();
        window.focus();
        $ektron("div#RadTabStrip1 a").eq(0).focus();
        BindOnRadWindowKeyDown();
    });
    
	var ResourceText = 
	{
		sFieldHeading: "<asp:literal id="sFieldHeading" runat="server"/>"
	,	sInsertFieldHere: "<asp:literal id="sInsertFieldHere" runat="server"/>"
	};
	var m_objFormField = null;
	var m_oFieldElem = null;
	var m_oRowFieldName = new EkFieldNameControl("txtRowName", "txtRowDispName", "txtRowDispName");
	var m_sContentTree = "";
	
	function initField()
	{
	    m_objFormField = new EkFormFields();
		m_oFieldElem = null;
	    
	    var oFieldElem = null;
	    var bIsRootLoc = false;
	    var sDefaultId = "";
	    var sDefaultPrefix = "";
	    var sDefaultRowId = "";
	    var oContentElement = null; 
	    var args = GetDialogArguments();
	    if (args)
	    {
	        oContentElement = args.contentElement;
	        oFieldElem = args.selectedField;
	        bIsRootLoc = args.isRootLocation;
	        sDefaultPrefix = args.fieldPrefix;
	        sDefaultId = args.fieldId;
	        sDefaultRowId = parseInt(sDefaultId, 10) + 1;
	        m_sContentTree = args.contentTree;
	    }
	    ekFieldAdvancedControl.setRootTagVisible(bIsRootLoc);
        ekFieldXPathControl.init(oContentElement, "ektdesignns_relevant");
	    ekFieldXPathControl.loadContentTree(m_sContentTree);
	    
	    if (m_objFormField.isDDFieldElement(oFieldElem) && "TABLE" == oFieldElem.tagName)
	    {
			ekFieldNameControl.read(oFieldElem);
			ekFieldUseControl.read(oFieldElem);
			ekFieldAllowControl.read(oFieldElem);
			ekFieldAdvancedControl.read(oFieldElem);
			ekFieldXPathControl.read(oFieldElem);
		    
		    document.getElementById("txtCaption").value = getTableCaption(oFieldElem);
		    
			// repeatable row
			var oTBody = oFieldElem.tBodies[0]; 
			if (0 == oTBody.rows.length) oTBody.insertRow(0);
			var oRow = oTBody.rows[0];
			
		    m_oRowFieldName.read(oRow);
		    ekFieldMinMaxControl.read(oRow);
            
			var nColCount = 0;
			for (var iCol = 0; iCol < oRow.cells.length; iCol++)
			{
				var oCell = oRow.cells[iCol];
				nColCount += oCell.colSpan;
			}
			document.getElementById("txtNumCols").value = nColCount;
			
			m_oFieldElem = oFieldElem;
			ekFieldNameControl.initSetting(true);
			m_oRowFieldName.initSetting(true);
		}
		else
		{
		    ekFieldNameControl.setDefaultFieldNames(sDefaultPrefix, sDefaultId);
	        ekFieldAdvancedControl.setName(sDefaultPrefix + sDefaultId);
		    m_oRowFieldName.setDefaultFieldNames(sDefaultPrefix, sDefaultRowId);
		    ekFieldNameControl.initSetting(false);
		    m_oRowFieldName.initSetting(false);
		}
		ekFieldAdvancedControl.setDefaultVal("Value");
		ekFieldAdvancedControl.updateFieldNameControl(ekFieldNameControl);
		ekFieldXPathControl.initXPathExpression();
	}
	
	function insertField()
	{
	    if (false == validateDialog())
	    {
	        return false;
	    }
	    
		var oFieldElem = m_oFieldElem;
		var oTBody = null;
		if (null == oFieldElem)
		{
			oFieldElem = document.createElement("table");
			if (0 == oFieldElem.tBodies.length)
			{
			    // FireFox does not have a default tBody in the table
			    oTBody = oFieldElem.ownerDocument.createElement("tbody");
			    oFieldElem.appendChild(oTBody);
			}
		}
		if (null == oFieldElem.tHead)
		{
			var oTHead = oFieldElem.createTHead();
			oTHead.insertRow(0); // FireFox requires the index 
		}
		ekFieldAdvancedControl.updateControl(ekFieldNameControl, ekFieldAllowControl.isRepeatable());
		ekFieldAdvancedControl.updateFieldNameControl(ekFieldNameControl);
        ekFieldAdvancedControl.updateFieldAllowControl(ekFieldAllowControl);
        ekFieldAdvancedControl.updateFieldUseControl(ekFieldUseControl);
		ekFieldNameControl.update(oFieldElem);
		ekFieldUseControl.update(oFieldElem);
		ekFieldAllowControl.update(oFieldElem);
		ekFieldAdvancedControl.update(oFieldElem);
		ekFieldXPathControl.update(oFieldElem);
		
		if (!oFieldElem.cellPadding) oFieldElem.cellPadding = "0";
		if (!oFieldElem.cellSpacing) oFieldElem.cellSpacing = "0";
		if (!oFieldElem.border) oFieldElem.border = 0;
		var bHasBorder = (oFieldElem.border > 0);

        // The user may change the class attribute, so don't depend on its value.
        // Only set the class if border=0 or missing
        var strClass = oFieldElem.className;
        if ("" == strClass && !bHasBorder)
        {
			// allow show border at design-time
            oFieldElem.className = "show_design_border";
        }
        else if ("show_design_border" == strClass && bHasBorder)
        {
            // remove class attribute, it has its own border
            oFieldElem.className = "";
            oFieldElem.removeAttribute("className");
        }
        oFieldElem.summary = ekFieldNameControl.getToolTip();

		// caption
		var strCaption = $ektron.trim(document.getElementById("txtCaption").value);
		if (strCaption && null == oFieldElem.caption)
		{
			oFieldElem.createCaption();
			setTableCaption(oFieldElem, strCaption);
		}
		else if (!strCaption && oFieldElem.caption != null)
		{
			oFieldElem.deleteCaption();
		}
		else if (strCaption)
		{
		    setTableCaption(oFieldElem, strCaption);
		}
		
	    var mc_MinNumCols = 1;
        var mc_MaxNumCols = 25; // same as frmTable

	    var nCols = $ektron.toInt(document.getElementById("txtNumCols").value);
        if (nCols < mc_MinNumCols) nCols = mc_MinNumCols;
        if (nCols > mc_MaxNumCols) nCols = mc_MaxNumCols;
        
		// header
        // add new columns
        for (var iRow = 0; iRow < oFieldElem.tHead.rows.length; iRow++)
        {
			var oRow = oFieldElem.tHead.rows[iRow];
			var nColCount = 0;
			for (var iCol = 0; iCol < oRow.cells.length; iCol++)
			{
				var oCell = oRow.cells[iCol];
				nColCount += oCell.colSpan;
			}
			for (var iCol = nColCount; iCol < nCols; iCol++)
			{
				// var oCell = oRow.insertCell(oRow.cells.length); // insertCell creates 'td' not 'th'
				var oCell = oFieldElem.ownerDocument.createElement("TH");
				oCell.innerHTML = ResourceText.sFieldHeading;
				oRow.appendChild(oCell);
			}
		}
		
		// repeatable row
		oTBody = oFieldElem.tBodies[0];
		oTBody.setAttribute("ektdesignns_list", "true");
		if (0 == oTBody.rows.length) oTBody.insertRow(0);
		var oRow = oTBody.rows[0];
		
		m_oRowFieldName.update(oRow);
		oRow.setAttribute("ektdesignns_nodetype", "element");
		ekFieldMinMaxControl.update(oRow);
		
		// body
        // add new columns
        for (var iRow = 0; iRow < oTBody.rows.length; iRow++)
        {
			var oRow = oTBody.rows[iRow];
			var nColCount = 0;
			for (var iCol = 0; iCol < oRow.cells.length; iCol++)
			{
				var oCell = oRow.cells[iCol];
				nColCount += oCell.colSpan;
			}
			for (var iCol = nColCount; iCol < nCols; iCol++)
			{
				var oCell = oRow.insertCell(oRow.cells.length);
				oCell.innerHTML = (0 == iRow ? ResourceText.sInsertFieldHere : "&#160;");
			}
		}

		CloseDlg(oFieldElem);	
	}

    
    function setTableCaption(oTableElem, strCaption)
    {
		if (!oTableElem) return;
		if (!oTableElem.caption) return;
		$ektron(oTableElem.caption).text(strCaption);
    }
    
    function getTableCaption(oTableElem)
    {
		// returns string
		if (!oTableElem) return "";
		if (!oTableElem.caption) return "";
		return $ektron(oTableElem.caption).text();
	}
	
	function validateDialog()
	{
		var bContinue = true;
	    var ret = [];
	    $ektron(document).trigger("onValidateDialog", [ret]);
	    if (ret && ret.length > 0)
	    {
	        bContinue = m_objFormField.promptOnValidateAction(ret[0]);
	    }
	    return bContinue;
	}
	
	function ClientTabSelectedHandler(sender, eventArgs)
	{
	    var tab = eventArgs.Tab;  
	    var ClientID = "fieldAdvanced"; 
	    var tabSelected = tab.Value.toLowerCase();
	    switch (tabSelected)
	    {
	        case "advanced":
	            ekFieldAdvancedControl.updateControl(ekFieldNameControl, ekFieldAllowControl.isRepeatable()); 
	            break;
	        case "relevance":
	            ekFieldNameControl.updateName(ekFieldAdvancedControl); 
	            break;
            case "general":
    	        ekFieldAdvancedControl.updateFieldNameControl(ekFieldNameControl);
				ekFieldAdvancedControl.updateFieldAllowControl(ekFieldAllowControl);
                ekFieldAdvancedControl.updateFieldUseControl(ekFieldUseControl);
    	        break;
    	    default:
    	        ekFieldNameControl.updateName(ekFieldAdvancedControl); 
    	        break;
	    }
	}
//-->
</script>
</body>
</html>
