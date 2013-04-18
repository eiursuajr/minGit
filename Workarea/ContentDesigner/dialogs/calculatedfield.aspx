<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<%@ Page language="c#" CodeFile="calculatedfield.aspx.cs" Inherits="Ektron.ContentDesigner.Dialogs.CalculatedField" AutoEventWireup="false" %>
<%@ Register TagPrefix="ek" TagName="FieldNameControl" Src="ucFieldName.ascx" %>
<%@ Register TagPrefix="ek" TagName="FieldValidateControl" Src="ucFieldValidation.ascx" %>
<%@ Register TagPrefix="ek" TagName="FieldTreeViewControl" Src="ucFieldTreeView.ascx" %>
<%@ Register TagPrefix="ek" TagName="FieldDataStyleControl" Src="ucFieldDataStyle.ascx" %>
<%@ Register TagPrefix="ek" TagName="FieldDialogButtons" Src="ucFieldDialogButtons.ascx" %>
<%@ Register TagPrefix="ek" TagName="FieldXPath" Src="ucFieldXPath.ascx" %>
<%@ Register TagPrefix="radcb" Namespace="Telerik.WebControls" Assembly="RadComboBox.NET2" %>
<%@ Register TagPrefix="radTS" Namespace="Telerik.WebControls" Assembly="RadTabStrip.NET2" %>
<%@ Register TagPrefix="ek" TagName="FieldAdvancedControl" Src="ucFieldAdvanced.ascx" %>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
<meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
<title id="Title" runat="server">Calculated Field</title>
</head>
<body class="dialog">
<form id="Form1" runat="server">
    <div class="Ektron_DialogTabstrip_Container">
	<radTS:RadTabStrip id="RadTabStrip1" runat="server" MultiPageID="RadMultiPage1" SelectedIndex="0" ReorderTabRows="true" 
			OnClientTabSelected="ClientTabSelectedHandler" SkinID="TabstripDialog">
		<Tabs>
			<radTS:Tab ID="General" Text="General" Value="General" />
			<radTS:Tab ID="Validation" Text="Validation" Value="Validation" />
			<radTS:Tab ID="DataStyle" Text="Data Style" Value="DataStyle" />
			<radTS:Tab ID="Advanced" Text="Advanced" Value="Advanced" />
		</Tabs>
	</radTS:RadTabStrip>
    </div>
    <div class="Ektron_Dialog_Tabs_BodyContainer">
        <radTS:RadMultiPage id="RadMultiPage1" runat="server" SelectedIndex="0">
            <radTS:PageView id="Pageview1" runat="server"> 
	            <table width="100%">
	                <ek:FieldNameControl id="Name" runat="server" />
		            <tr>
		                <td colspan="3">
		                    <div class="Ektron_TopSpaceVeryVerySmall">
		                        <fieldset>
                                    <legend id="lblCalculation" runat="server" title="Calculation">Calculation</legend>
									<ek:FieldXPath id="Calculation" FieldType="CalculatedField" runat="server" />
                                </fieldset>
                            </div>
		                </td>
		            </tr>
		        </table>
	        </radTS:PageView>
            <radTS:PageView id="Pageview2" runat="server">
                <table width="100%">
					<ek:FieldValidateControl id="validateControl" runat="server" Enabled="true" />
                </table>  
	        </radTS:PageView>
            <radTS:PageView id="Pageview3" runat="server">
                <table width="100%">
                    <ek:FieldDataStyleControl runat="server" />
                </table> 
	        </radTS:PageView>
            <radTS:PageView id="Pageview4" runat="server">
                <table width="100%">
                    <ek:FieldAdvancedControl ID="fieldAdvanced" NodeTypeVisible="true" runat="server" />
                </table> 
            </radTS:PageView>
        </radTS:RadMultiPage>
	</div>
	
    <div class="Ektron_Dialogs_LineContainer">
        <div class="Ektron_TopSpaceSmall"></div>
        <div class="Ektron_StandardLine"></div>
    </div>		
	
	<ek:FieldDialogButtons ID="btnSubmit" OnOK="return insertField();" runat="server" />
</form> 
<script type="text/javascript">
<!--
    Ektron.ready(function(){
        initField();
        window.focus();
        $ektron("div#RadTabStrip1 a").eq(0).focus();
        BindOnRadWindowKeyDown();
    });
    
	var ResourceText = 
	{
		sFormula: "<asp:literal id="sFormula" runat="server"/>"
	,	sValue: "<asp:literal id="sValue" runat="server"/>"
	};
	
	var RadTabStrip1_ClientID = "<%=RadTabStrip1.ClientID %>";

    var m_cal_ekXPathExpr = new Ektron.XPathExpression();
    var m_objFormField = null;
    var m_oFieldElem = null;
    var sContentTree = "";
    
	function initField()
	{	
	    m_objFormField = new EkFormFields();
	    m_oFieldElem = null;
        
        var oContentElement = null;
        var oFieldElem = null;
	    var bIsRootLoc = false;
	    var sDefaultId = "";
	    var sDefaultPrefix = "";
        var args = GetDialogArguments();
	    if (args)
	    {
			oContentElement = args.contentElement;
	        oFieldElem = args.selectedField;
	        bIsRootLoc = args.isRootLocation;
	        sDefaultPrefix = args.fieldPrefix;
	        sDefaultId = args.fieldId;
	        sContentTree = args.contentTree;
	    }
	    ekFieldAdvancedControl.setRootTagVisible(bIsRootLoc);
	    ekFieldXPathControl.init(oContentElement, "ektdesignns_calculate");
	    ekFieldXPathControl.loadContentTree(sContentTree);
	    ekFieldValidationControl.init(oContentElement);
	    ekFieldValidationControl.setDefaultVal("integer");
	    
        if (m_objFormField.isDDFieldElement(oFieldElem) && "INPUT" == oFieldElem.tagName && $ektron(oFieldElem).hasClass("design_calculation"))
        {
            ekFieldNameControl.read(oFieldElem);
            ekFieldXPathControl.read(oFieldElem);
            ekFieldAdvancedControl.read(oFieldElem);
            ekFieldDataStyleControl.read(oFieldElem);
            ekFieldValidationControl.read(oFieldElem);

		    m_oFieldElem = oFieldElem;
		    ekFieldNameControl.initSetting(true);
		}
		else
		{
		    ekFieldNameControl.setDefaultFieldNames(sDefaultPrefix, sDefaultId);
	        ekFieldAdvancedControl.setName(sDefaultPrefix + sDefaultId);
	        ekFieldNameControl.initSetting(false);
		}
		ekFieldAdvancedControl.setDefaultVal(ResourceText.sValue);
		ekFieldAdvancedControl.updateFieldNameControl(ekFieldNameControl);
		ekFieldXPathControl.initXPathExpression();
		ekFieldXPathControl.SetComboDefaultText();
	}
	
	function insertField()
	{
	    if (false == validateDialog())
	    {
	        return false;
	    }
	    
	    var oFieldElem = m_oFieldElem;
	    if (null == oFieldElem)
		{
			oFieldElem = document.createElement("input");
			oFieldElem.className = "design_calculation";
			oFieldElem.style.width = "73px";
			oFieldElem.style.height= "20px";
            oFieldElem.setAttribute("readonly", "readonly");
            oFieldElem.size = 8;
            oFieldElem.type = "text";
		}
		ekFieldAdvancedControl.updateControl(ekFieldNameControl);
		ekFieldAdvancedControl.updateFieldNameControl(ekFieldNameControl);
	    ekFieldNameControl.update(oFieldElem);
	    ekFieldAdvancedControl.update(oFieldElem);
	    ekFieldDataStyleControl.update(oFieldElem);
	    ekFieldValidationControl.update(oFieldElem);
	    ekFieldXPathControl.update(oFieldElem);

		CloseDlg(oFieldElem);	
	}
    function NotClose()
    {
        return false;
    }
    
    function validateDialog()
	{
	    var bContinue = true;
	    var ret = [];
	    $ektron(document).trigger("onValidateDialog", [ret]);
	    if (ret && ret.length > 0)
	    {
            if ("CalculatedField" == ret[0].name)
            {
                var oTabCtl = window[RadTabStrip1_ClientID];
                var currentTab = oTabCtl.SelectedTab;
                if (currentTab.Value != "General")
                {
                    var tab = oTabCtl.FindTabById(RadTabStrip1_ClientID + "_General");
                    if (tab)
                    {
                        tab.SelectParents();
                    }
                }  
            } 
            else if ("ucFieldValidation" == ret[0].name)
            {
                var oTabCtl = window[RadTabStrip1_ClientID];
                var currentTab = oTabCtl.SelectedTab;
                if (currentTab.Value != "Validation")
                {
                    var tab = oTabCtl.FindTabById(RadTabStrip1_ClientID + "_Validation");
                    if (tab)
                    {
                        tab.SelectParents();
                    }
                }  
            } 
            bContinue = EkFormFields_PromptOnValidateAction(ret[0]);
	    }
	    return bContinue;
	}
	
	function ClientTabSelectedHandler(sender, eventArgs)
	{	
	    var tab = eventArgs.Tab;  
	    var tabSelected = tab.Value.toLowerCase();
	    switch(tabSelected)
	    {
	        case "advanced":
	            ekFieldAdvancedControl.updateControl(ekFieldNameControl); 
	            break;
	        case "validation":
	            ekFieldNameControl.updateName(ekFieldAdvancedControl); 
			    ekFieldValidationControl.loadContentTree(sContentTree);
	            ekFieldValidationControl.reloadV8nBox("CalculatedField", "calculation");
	            break;
	        case "general":
	            ekFieldAdvancedControl.updateFieldNameControl(ekFieldNameControl);
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
