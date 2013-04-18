<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<%@ Page language="c#" CodeFile="groupbox.aspx.cs" Inherits="Ektron.ContentDesigner.Dialogs.GroupBox" AutoEventWireup="false" %>
<%@ Register TagPrefix="ek" TagName="FieldNameControl" Src="ucFieldName.ascx" %>
<%@ Register TagPrefix="ek" TagName="FieldUseControl" Src="ucFieldUse.ascx" %>
<%@ Register TagPrefix="ek" TagName="FieldAllowControl" Src="ucFieldAllow.ascx" %>
<%@ Register TagPrefix="ek" TagName="FieldDialogButtons" Src="ucFieldDialogButtons.ascx" %>
<%@ Register TagPrefix="ek" TagName="FieldAdvancedControl" Src="ucFieldAdvanced.ascx" %>
<%@ Register TagPrefix="ek" TagName="FieldXPath" Src="ucFieldXPath.ascx" %>
<%@ Register TagPrefix="radTS" Namespace="Telerik.WebControls" Assembly="RadTabStrip.NET2" %>

<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
<meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
<title id="Title" runat="server">Group Box</title>
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
	            <table>
		            <ek:FieldNameControl ID="Name" ToolTipEnabled="false" IndexedEnabled="false" runat="server" />
		            <tr>
		                <td colspan="3">
		                    <table style="width:100%" class="Ektron_TopSpaceVeryVerySmall">
	                        <tr>
	                            <td style="width:50%; vertical-align:top;">
	                            <ek:FieldUseControl ID="use" runat="server" />
		                        </td>
		                        <td>&nbsp;&nbsp;</td>
	                            <td style="width:50%">
	                            <ek:FieldAllowControl ID="allow" runat="server" AllowMaxOccursLimit="true" />
		                        </td>
	                        </tr>
		                    </table>
		                </td>
		            </tr>
		            <tr>
		                <td colspan="3">
		                <fieldset class="Ektron_TopSpaceVeryVerySmall">
		                <legend id="lblAppearance" runat="server" title="Appearance">Appearance</legend>
                            <div>
                                <input title="Select No Boarder" type="radio" name="optApprnce" id="optApprnce1" value="False" onclick="enableCaptionField(false)" /><label title="No border" for="optApprnce1" id="lblNoBorder" runat="server">No border</label><br />
                                <input title="Select Show Boarder and Caption" type="radio" name="optApprnce" id="optApprnce2" value="True" onclick="enableCaptionField(true)" checked="checked" /><label  title="Show border and caption" for="optApprnce2" id="lblShowBorder" runat="server">Show border and caption</label><br />
                                <label title="Caption" for="txtCaption" class="Ektron_StandardLabel" id="lblCaption" runat="server">Caption:</label><input title="Enter Caption here" type="text" name="txtCaption" id="txtCaption" runat="server" class="RadETextBox" value="Fields" style="width:70%" /><br />
                            </div>
		                </fieldset>
		                </td>
		            </tr>
	            </table>
	        </radTS:PageView>
            <radTS:PageView id="Pageview2" runat="server">
                <table width="100%">
                    <ek:FieldAdvancedControl ID="fieldAdvanced" NodeTypeEnabled="true" TypeAttributeEnabled="false" TypeContentEnabled="false" runat="server" />
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
		sEnterCaption: "<asp:literal id="sEnterCaption" runat="server"/>"
	};
    var m_objFormField = null;
    var m_oFieldElem = null;
    var m_strInitialContent = "&#160;"; //need this to click inside Fieldset in FF
	var m_sContentTree = "";
	
	function initField()
	{
	    m_objFormField = new EkFormFields();
	    m_oFieldElem = null;
	    m_oCaptionElem = null;
	    
	    var oFieldElem = null;
	    var bIsRootLoc = false;
	    var sDefaultId = "";
	    var sDefaultPrefix = "";
	    var oContentElement = null; 
	    var args = GetDialogArguments();
	    if (args)
	    {
	        oContentElement = args.contentElement;
	        oFieldElem = args.selectedField;
	        bIsRootLoc = args.isRootLocation;
	        sDefaultPrefix = args.fieldPrefix;
	        sDefaultId = args.fieldId;
	        m_sContentTree = args.contentTree;
	    }
	    ekFieldAdvancedControl.setRootTagVisible(bIsRootLoc);
	    ekFieldXPathControl.init(oContentElement, "ektdesignns_relevant");
	    ekFieldXPathControl.loadContentTree(m_sContentTree);

	    if (m_objFormField.isDDFieldElement(oFieldElem) && "FIELDSET" == oFieldElem.tagName)
	    {
	        ekFieldNameControl.read(oFieldElem);
	        ekFieldUseControl.read(oFieldElem);
	        ekFieldAllowControl.read(oFieldElem);
	        ekFieldAdvancedControl.read(oFieldElem);
	        ekFieldXPathControl.read(oFieldElem);
		    document.forms[0].optApprnce[1].checked = true; // default: show border
		    var joFieldElem = $ektron(oFieldElem);
		    if (joFieldElem.hasClass("design_group"))
		    {
		        document.forms[0].optApprnce[0].checked = true; // no border
		        enableCaptionField(false);
		    }
		    else
		    {
		        document.getElementById("txtCaption").value = joFieldElem.children("legend").html(); //TODO: test if there is no legend tag.
		        enableCaptionField(true);
		    }
		    m_strInitialContent = "";
		    var oDiv = GetContentDiv(oFieldElem);
		    if (oDiv != null)
		    {
		        m_strInitialContent = oDiv.innerHTML;
		    }
            m_oFieldElem = oFieldElem;
            ekFieldNameControl.initSetting(true);
		}
		else
		{
		    ekFieldNameControl.setDefaultFieldNames(sDefaultPrefix, sDefaultId);
	        ekFieldAdvancedControl.setName(sDefaultPrefix + sDefaultId);
	        ekFieldNameControl.initSetting(false);
            ekFieldAllowControl.enableMaxNumField(false);
		}
		ekFieldAdvancedControl.setDefaultVal("...");
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
		var objLegend = null;
		var objDiv = null;
	    if (null == oFieldElem) //new field
	    {
		    oFieldElem = document.createElement("fieldset");
		    oFieldElem.setAttribute("contentEditable", false);
            oFieldElem.className = "";
            oFieldElem.removeAttribute("className");
            if (true == document.forms[0].optApprnce[1].checked) //with legend
            {
		        objLegend = document.createElement("legend");
		        objLegend.setAttribute("contentEditable", true);
		        oFieldElem.appendChild(objLegend); 
		    }
		    else // no border
		    {   
		        oFieldElem.className = "design_group";
		    }
		    
		    var objDiv = document.createElement("div");
		    objDiv.className = "design_membrane";
		    objDiv.setAttribute("contentEditable", true);
		    objDiv.setAttribute("ektdesignns_nodetype", "content");
		    oFieldElem.appendChild(objDiv);
	    }
	    else // existing field
	    {
	        objDiv = $ektron(oFieldElem).children("div").get(0);
	        oFieldElem.className = "";
	        oFieldElem.removeAttribute("className");
	        if (true == document.forms[0].optApprnce[1].checked) // with legend
	        {
	            var $legend = $ektron("legend", oFieldElem);
	            if ($legend.length > 0)
	            {
	                objLegend = $legend.get(0); 
	            }
	            else
	            {
	                objLegend = oFieldElem.ownerDocument.createElement("legend");
		            objLegend.setAttribute("contentEditable", true);
		            oFieldElem.insertBefore(objLegend, objDiv); 
	            }
	        }
	        else // no border
		    {   
		        oFieldElem.className = "design_group";
		        if ("LEGEND" == oFieldElem.firstChild.tagName)
	            {
	                oFieldElem.removeChild(oFieldElem.firstChild);
	            }
		    }
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
		if (objLegend)
		{
	        objLegend.innerHTML = document.getElementById("txtCaption").value;
	    }
	    objDiv.innerHTML = m_strInitialContent;
        
		CloseDlg(oFieldElem);	
	}
    function GetContentDiv(args)
    {
        var aDiv = args.getElementsByTagName("DIV");
        if (aDiv.length > 0)
        {
            var idx = 0;
            while (aDiv[idx].getAttribute("ektdesignns_nodetype") != "content")
            {
                idx ++;
            }
            return aDiv[idx];
        }
        return null;
    }
    function enableCaptionField(bEnable)
    {
        document.getElementById("txtCaption").disabled = (!bEnable);
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
	    if (true == bContinue && true == document.forms[0].optApprnce[1].checked)
	    {
	        if (0 == document.getElementById("txtCaption").value.length)
	        {
	            alert(ResourceText.sEnterCaption);
	            document.getElementById("txtCaption").focus();
	            bContinue = false;
	        }
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
