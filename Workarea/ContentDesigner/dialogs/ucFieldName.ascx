<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<%@ Control language="c#" CodeFile="ucFieldName.ascx.cs" Inherits="Ektron.ContentDesigner.Dialogs.ucFieldName" AutoEventWireup="false" %>

<script type="text/javascript" language="javascript">		
<!--
function EkFieldNameControl(idName, idDisplayName, idToolTip, idIndexed)
{
	var bPrimary = ("undefined" == typeof idName);
	this.idName = (bPrimary ? "txtName" : idName);
	this.idDisplayName = (bPrimary ? "txtDispName" : idDisplayName);
	this.idToolTip = (bPrimary ? "txtToolTip" : idToolTip);
	this.idIndexed = (bPrimary ? "optIndexed" : idIndexed);
	this.name = "";
	this.bKeepNameInSync = false;
    this.bKeepToolTipInSync = false;
            
	
    this.maxFieldNameLength = 45;
	this.getName = function()
	{
		if (!this.idName) return "";
		var name = $ektron.trim(document.getElementById(this.idName).value);
		name = EkFormFields_FixId(name);
		name = name.substr(0, this.maxFieldNameLength);
		if (true == document.getElementById(this.idName).disabled)
		{
		    return this.name;
		}
		else
		{
		    return name;
		}
	};
	this.setDefaultFieldNames = function(sPrefix, sId)
	{
	    this.setName(sPrefix, sId);
	    var sDefaultName = sPrefix + " " + sId;
	    if (this.idDisplayName)
	    {
	        document.getElementById(this.idDisplayName).value = sDefaultName;
	    }
	    if (this.idToolTip && document.getElementById(this.idToolTip) != null)
	    {
	        document.getElementById(this.idToolTip).value = sDefaultName;
	    }
	};
	this.setName = function(sPrefix, sId)
	{
	    // sometimes sPrefix already contains the sId. sId is not passing in here.
	    if ("undefined" == typeof sId) 
	    {
	        sId = "";
	    }
	    this.name = sPrefix + sId;
	    document.getElementById(this.idName).value = this.name;
	};
	this.updateName = function(objAdvancedCtl)
	{
	    var fieldName = this.getName();
	    document.getElementById(this.idName).value = fieldName;
        objAdvancedCtl.setName(fieldName);
	};
	this.setAdvancedName = function(name)
	{
	    document.getElementById(this.idName).value = name;
	};
	this.getDisplayName = function()
	{
		if (!this.idDisplayName) return "";
		var ret = $ektron.trim(document.getElementById(this.idDisplayName).value);
		return $ektron.removeTags(ret);
	};
	this.getToolTip = function()
	{
		if (!this.idToolTip) return "";
	    var oToolTip = document.getElementById(this.idToolTip);
	    if (!oToolTip) this.idToolTip = "";
		return (oToolTip ? $ektron.removeTags(oToolTip.value) : ""); 
	};
	
	this.enableFieldName = function(bEnabled)
	{
	    document.getElementById(this.idName).disabled = !bEnabled;
	};
	
	this.read = function(oFieldElem) 
	{
		var strId = oFieldElem.id;
		var strName = (strId || oFieldElem.name || oFieldElem.getAttribute("name") || "");
		var strDispName = $ektron.toStr(oFieldElem.getAttribute("ektdesignns_caption"), strName);
        document.getElementById(this.idName).value = strName;
	    document.getElementById(this.idDisplayName).value = strDispName;
		if (this.idToolTip) 
		{
			var oToolTip = document.getElementById(this.idToolTip);
			if (!oToolTip) this.idToolTip = "";
			if (oToolTip) oToolTip.value = oFieldElem.title;
		}
		if (this.idIndexed)
		{
			var oIndexed = document.getElementById(this.idIndexed);
			if (!oIndexed) this.idIndexed = "";
			if (oIndexed) oIndexed.checked = $ektron.toBool(oFieldElem.getAttribute("ektdesignns_indexed"));
		}
	};
	
	this.update = function(oFieldElem)
	{
		// 'name' attribute is read-only in IE
        oFieldElem.id = this.getName();
	    oFieldElem.setAttribute("name", oFieldElem.id);
	    oFieldElem.setAttribute("ektdesignns_caption", this.getDisplayName());
	    oFieldElem.setAttribute("ektdesignns_name", oFieldElem.id);
		if (this.idToolTip) 
		{
			oFieldElem.title = this.getToolTip();
		}
		if (this.idIndexed)
		{
			var oIndexed = document.getElementById(this.idIndexed);
			if (!oIndexed) this.idIndexed = "";
			if (oIndexed) oFieldElem.setAttribute("ektdesignns_indexed", oIndexed.checked + "");
		}
	};
	
	this.initSetting = function(bReload)
	{
	    if (bReload)
	    {
            //reload dialog
            var txtDisplayName;
            if (this.idDisplayName)
            {
	            txtDisplayName = document.getElementById(this.idDisplayName).value;
                if (0 == txtDisplayName.length)
                {
                    document.getElementById(this.idDisplayName).value = document.getElementById(this.idName).value;
                }
            }
            if (this.idToolTip && this.idDisplayName)
            {
                if (0 == document.getElementById(this.idToolTip).value.length && txtDisplayName.length > 0)
                {
                    document.getElementById(this.idToolTip).value = txtDisplayName;
                }
            }
            this.bKeepNameInSync = this.areDispNameAndNameEqual()
            this.bKeepToolTipInSync = this.areDispNameAndToolTipEqual()
        }
        else
        {
            //fresh dialog
            this.bKeepNameInSync = true;
            this.bKeepToolTipInSync = true;
        }
	};
	
    this.areDispNameAndNameEqual = function()// As Boolean
    {
        var txtDisplayName = document.getElementById(this.idDisplayName).value;
        var txtName = document.getElementById(this.idName).value;
        return (EkFormFields_FixId(txtDisplayName) == txtName || 0 == txtName.length);
    };

    this.areDispNameAndToolTipEqual = function()// As Boolean
    {
        if ("" == this.idToolTip) return false;
        var txtDisplayName = document.getElementById(this.idDisplayName).value;
        var txtToolTip = document.getElementById(this.idToolTip).value;
        return (txtDisplayName == txtToolTip || 0 == txtToolTip.length);
    };

    this.keepNameInSync = function(objGroup)
    {
        if ("undefined" == typeof objGroup)
        {
            objGroup = this;
        }
        var txtDisplayName = document.getElementById(objGroup.idDisplayName).value;
        if (objGroup.bKeepNameInSync)
        {
            if (txtDisplayName.length > objGroup.maxFieldNameLength)
            {
                objGroup.bKeepNameInSync = false;
            }
            else if (objGroup.getName() != "")
            {
                if (false == document.getElementById(objGroup.idName).disabled)
                {
                    objGroup.setName(EkFormFields_FixId(txtDisplayName));
                }
            }
        }
        if (objGroup.bKeepToolTipInSync)
        {
            if (objGroup.idToolTip && document.getElementById(objGroup.idToolTip) != null)
            {
                document.getElementById(objGroup.idToolTip).value = txtDisplayName;
            }
        }
    };
}
var ekFieldNameControl = new EkFieldNameControl();	
//-->
</script>

<tr>
    <td><label title="Descriptive Name" for="txtDispName" class="Ektron_StandardLabel" id="lblDescName" runat="server">Descriptive Name:</label></td>
	<td><input title="Enter Descriptive Name here" type="text" name="txtDispName" class="Ektron_StandardTextBox" id="txtDispName" value="" onblur="ekFieldNameControl.keepNameInSync();" /></td>
    <td>
    <asp:PlaceHolder ID="IndexedArea" runat="server">
        <input title="Indexed Option" type="checkbox" name="optIndexed" id="optIndexed" /><label title="Indexed" for="optIndexed" id="lblIndexed" runat="server">Indexed</label>
    </asp:PlaceHolder>&#160;
    </td>
</tr>
<tr>
    <td><label title="Field Name" for="txtName" class="Ektron_StandardLabel" id="lblFieldName" runat="server">Field Name:</label></td>
    <td colspan="2"><input title="Enter Field Name here" type="text" name="txtName" class="Ektron_StandardTextBox" id="txtName" value="" /></td>
</tr>
<asp:PlaceHolder ID="ToolTipArea" runat="server">
<tr>
    <td><label title="Tool Tip Text" for="txtToolTip" class="Ektron_StandardLabel" id="lblToolTip" runat="server">Tool Tip Text:</label></td>
    <td colspan="2"><input title="Enter Tool Tip Text here" type="text" name="txtToolTip" class="Ektron_StandardTextBox" id="txtToolTip" value="" /></td>
</tr>
</asp:PlaceHolder>