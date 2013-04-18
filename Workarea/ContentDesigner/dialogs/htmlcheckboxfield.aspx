<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<%@ Page language="c#" CodeFile="htmlcheckboxfield.aspx.cs" Inherits="Ektron.ContentDesigner.Dialogs.HtmlCheckboxField" AutoEventWireup="false" %>
<%@ Register TagPrefix="ek" TagName="FieldNameControl" Src="ucFieldName.ascx" %>
<%@ Register TagPrefix="ek" TagName="FieldValidateControl" Src="ucFieldValidation.ascx" %>
<%@ Register TagPrefix="ek" TagName="FieldDialogButtons" Src="ucFieldDialogButtons.ascx" %>
<%@ Register TagPrefix="radTS" Namespace="Telerik.WebControls" Assembly="RadTabStrip.NET2" %>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
<meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
<title id="Title" runat="server">Checkbox Field</title>
</head>
<body class="dialog">
<form id="form1" runat="server">
    
    <div class="Ektron_DialogTabstrip_Container">
    <radTS:RadTabStrip id="RadTabStrip1" runat="server" MultiPageID="RadMultiPage1" SelectedIndex="0" ReorderTabRows="true" OnClientTabSelected="ClientTabSelectedHandler" SkinID="TabstripDialog">

            <Tabs>
                <radTS:Tab ID="General" Text="General" Value="General" />
                <radTS:Tab ID="Validation" Text="Validation" Value="Validation" />
            </Tabs>
        </radTS:RadTabStrip>
    </div>
    <div class="Ektron_Dialog_Tabs_BodyContainer">
        <radTS:RadMultiPage id="RadMultiPage1" runat="server" SelectedIndex="0">
            <radTS:PageView id="Pageview1" runat="server">                    
        <table width="100%">
            <ek:FieldNameControl ID="Name" runat="server" IndexedEnabled="false" />
            <tr>
                <td colspan="3">
                    <fieldset class="Ektron_TopSpaceVeryVerySmall Ektron_BottomSpaceSmall">
		                        <legend title="Default Value" id="lblDefVal" runat="server">Default value</legend>
                        <div>
		                            <input title="Select True as the Defaut Value" type="radio" name="optDefVal" id="optDefValT" value="True" /><label title="True (checked)" for="optDefValT" id="lblDefValT" runat="server">True (checked)</label><br />
		                            <input title="Select False as the Default Value" type="radio" name="optDefVal" id="optDefValF" value="False" checked="checked" /><label title="False (unchecked)" for="optDefValF" id="lblDefValF" runat="server">False (unchecked)</label><br />
                        </div>
                    </fieldset>
                </td>
            </tr>
            <tr>
		                <td><label title="Caption" for="txtCaption" class="Ektron_StandardLabel" id="lblCaption" runat="server">Caption:</label></td>
                <td colspan="2"><input title="Enter Caption here" type="text" name="txtCaption" id="txtCaption" value="" class="Ektron_StandardTextBox" /></td>
            </tr>
        </table>
	        </radTS:PageView>
	        <radTS:PageView id="Pageview2" runat="server">
                <table width="100%">
                    <ek:FieldValidateControl id="validateControl" runat="server" Enabled="true" />
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
<script language="javascript" type="text/javascript">
<!--
	Ektron.ready(function(){
        initField();
        window.focus();
        $ektron("div#RadTabStrip1 a").eq(0).focus();
        BindOnRadWindowKeyDown();
    });
    
	var m_objFormField = null;
	var m_oFieldElem = null;
	var m_oCaptionElem = null;
	
	function initField()
	{
	    m_objFormField = new EkFormFields();
		m_oFieldElem = null;
		m_oCaptionElem = null;
		var oContentElement = null;
		var oFieldElem = null;
	    var bIsRootLoc = false;
	    var sContentTree = "";
	    var sDefaultId = "";
	    var sDefaultPrefix = "";
		var oForm = document.forms[0];

	    var args = GetDialogArguments();
	    if (args)
	    {
	    	oContentElement = args.contentElement;
	        oFieldElem = args.selectedField;
	        IsRootLoc = args.isRootLocation;
	        sDefaultPrefix = args.fieldPrefix;
	        sDefaultId = args.fieldId;
	        sContentTree = args.contentTree;
	    }
	    ekFieldValidationControl.init(oContentElement);
	    ekFieldValidationControl.loadContentTree(sContentTree);
	    ekFieldValidationControl.setDefaultVal("boolean");
	    
	    if (m_objFormField.isDDFieldElement(oFieldElem) && "INPUT" == oFieldElem.tagName && "checkbox" == oFieldElem.type)
	    {
			var oCaptionElem = m_objFormField.findLabelElement(oFieldElem.id, oFieldElem.ownerDocument);
			ekFieldNameControl.read(oFieldElem);
			ekFieldValidationControl.read(oFieldElem);
            var bChecked = oFieldElem.checked;
		    oForm.optDefVal[0].checked = bChecked;
		    oForm.optDefVal[1].checked = !bChecked;
			if (oCaptionElem)
			{
				document.getElementById("txtCaption").value = getLabelCaption(oCaptionElem);
			}
			m_oFieldElem = oFieldElem;
			m_oCaptionElem = oCaptionElem;
			ekFieldNameControl.initSetting(true);
		}
		else
		{
		    ekFieldNameControl.setDefaultFieldNames(sDefaultPrefix, sDefaultId);
		    document.getElementById("txtCaption").value = sDefaultPrefix + " " + sDefaultId;
		    ekFieldNameControl.initSetting(false);
		}
	}
	
	function insertField()
	{
		if (false == validateDialog())
	    {
	        return false;
	    }
	    
		var oFieldElem = m_oFieldElem;
		var oCaptionElem = m_oCaptionElem;
		if (null == oFieldElem)
		{
			if (window.radWindow && window.radWindow.IsIE)
			{
				// "The NAME attribute cannot be set at run time on elements dynamically created with the createElement method." - MSDN
                oFieldElem = $ektron("<input type=\"checkbox\" name=\"" + ekFieldNameControl.getName() + "\" />").get(0);
			}
			else
			{
				oFieldElem = document.createElement("input");
				oFieldElem.type = "checkbox";
				oFieldElem.name = ekFieldNameControl.getName();
			}
		}
		else if (window.radWindow && window.radWindow.IsIE && oFieldElem.name && oFieldElem.name != ekFieldNameControl.getName())
		{
			// "The NAME attribute cannot be set at run time on elements dynamically created with the createElement method." - MSDN
            oFieldElem = $ektron("<input type=\"checkbox\" name=\"" + ekFieldNameControl.getName() + "\" />").get(0);
			for (var i = 0; i < m_oFieldElem.attributes.length; i++)
			{
				var attr = m_oFieldElem.attributes[i];
				if (attr.specified && attr.name != "type")
				{
					oFieldElem.setAttribute(attr.name, attr.value);
				}
			}
		}
		ekFieldNameControl.update(oFieldElem);
		oFieldElem.setAttribute("ektdesignns_nodetype", "element");
		ekFieldValidationControl.update(oFieldElem);
		
		var bChecked = document.getElementById("optDefValT").checked
		oFieldElem.checked = bChecked;
		if (bChecked)
		{
			oFieldElem.setAttribute("checked", "checked");
		}
		else
		{
			oFieldElem.removeAttribute("checked");
		}
		
        var sLabelCaption = document.getElementById("txtCaption").value;
        if (null == oCaptionElem)
		{
		    oCaptionElem = oFieldElem.ownerDocument.createElement("label");
		}
		if (oCaptionElem)
		{
			oCaptionElem.htmlFor = oFieldElem.id;
			setLabelCaption(oCaptionElem, sLabelCaption);
		}
		
		if (oFieldElem != m_oFieldElem && oCaptionElem)
		{
			CloseDlg( [oFieldElem, oCaptionElem] );	
		}
		else
		{
			CloseDlg(oFieldElem);	
		}
	}
	
	function validateDialog()
	{
	    var bContinue = true;
	    var ret = [];
	    $ektron(document).trigger("onValidateDialog", [ret]);
	    if (ret && ret.length > 0)
	    {
            if ("ucFieldValidation" == ret[0].name)
            {
                var oTabCtl = <%= RadTabStrip1.ClientID %>;
                var currentTab = oTabCtl.SelectedTab;
                if (currentTab.Value != "Validation")
                {
                    var tab = oTabCtl.FindTabById("<%= RadTabStrip1.ClientID %>_Validation");
                    if (tab)
                    {
                        tab.SelectParents();
                    }
                }  
                bContinue = EkFormFields_PromptOnValidateAction(ret[0]); 
            } 
	    }
	    return bContinue;
	}
	
    function setLabelCaption(oLabelElem, strCaption)
    {
		if (!oLabelElem) return;
		$ektron(oLabelElem).text(strCaption);
    }
    
    function getLabelCaption(oLabelElem)
    {
		// returns string
		if (!oLabelElem) return "";
		return $ektron(oLabelElem).text();
	}
	
	function ClientTabSelectedHandler(sender, eventArgs)
	{        
	    var tab = eventArgs.Tab;  
	    var tabSelected = tab.Value.toLowerCase();
	    switch(tabSelected)
	    {
	        case "validation":
	            ekFieldValidationControl.reloadV8nBox("CheckBoxField", "checkbox");
	            break;
			case "general":
				break;
	        default:
	            break;
	    }
	}
//-->
</script>
</body>
</html>
