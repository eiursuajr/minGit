<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<%@ Control language="c#" CodeFile="ucFieldMinMax.ascx.cs" Inherits="Ektron.ContentDesigner.Dialogs.ucFieldMinMax" AutoEventWireup="false" %>

<script type="text/javascript" language="javascript">		
<!--
function EkFieldMinMaxControl()
{
	this.read = function(oElem) 
	{
        this.setMin(oElem.getAttribute("ektdesignns_minoccurs"));
        this.setMax(oElem.getAttribute("ektdesignns_maxoccurs"));
	}
	this.update = function(oElem)
	{
		var minNum = this.getMin();
		if (1 == minNum) 
		{
			oElem.removeAttribute("ektdesignns_minoccurs");
		}
		else
		{
			oElem.setAttribute("ektdesignns_minoccurs", minNum + "");
		}
		
		var maxNum = this.getMax();
		if ("unbounded" === maxNum)
		{
			oElem.setAttribute("ektdesignns_maxoccurs", "unbounded");
		}
		else if (1 == maxNum) 
		{
			oElem.removeAttribute("ektdesignns_maxoccurs");
		}
		else
		{
			oElem.setAttribute("ektdesignns_maxoccurs", maxNum + "");
		}
	}
	
	this.getMin = function()
	{
		var minNum = $ektron.toInt(document.getElementById("txtMinNum").value, 0);
		if (minNum < 0) minNum = 0;
		return minNum;
	}
	this.getMax = function()
	{
		if (document.getElementById("chkUnlimitedNum").checked)
		{
			return "unbounded";
		}
		else
		{
			var maxNum = $ektron.toInt(document.getElementById("txtMaxNum").value, 1);
			if (maxNum < 1) maxNum = 1;
			var minNum = this.getMin();
			if (maxNum < minNum) maxNum = minNum;
			return maxNum;
		}
	}
	this.setMin = function(value)
	{
		document.getElementById("txtMinNum").value = $ektron.toInt(value, 1);
	}
	this.setMax = function(value)
	{
        if ("unbounded" === value)
        {
			document.getElementById("txtMaxNum").value = "";
            document.getElementById("chkUnlimitedNum").checked = true;
        }
        else
        {
            document.getElementById("chkUnlimitedNum").checked = false;
			document.getElementById("txtMaxNum").value = $ektron.toInt(value, 1);
        }
        this.updateMaxNumField();
	}
	this.updateMaxNumField = function()
    {
		var bUnbounded = document.getElementById("chkUnlimitedNum").checked;
		var bIsDisabled = document.getElementById("txtMaxNum").disabled;
		if (bIsDisabled != bUnbounded)
		{
			document.getElementById("txtMaxNum").disabled = bUnbounded;
			if (!bUnbounded)
			{
				this.setMax(this.getMax());
			}
		}
    }
    
    /*
    this.validate = function()
    {
        var minNum = this.getMin();
        if (minNum < 0) return false;
		var maxNum = this.getMax();
		if ("unbounded" === maxNum) return true;
        if (maxNum < 1) return false;
	    return (minNum <= maxNum);
    }
    */
}
var ekFieldMinMaxControl = new EkFieldMinMaxControl();	
//-->
</script>

<tr>
    <td><label title="Minimum Number" for="txtMinNum" class="Ektron_StandardLabel" id="lblMinNum" runat="server">Minimum Number:</label></td>
    <td><input title="Enter Minimum Number here" type="text" name="txtMinNum" id="txtMinNum" value="0" class="Ektron_NumberTextBox" /></td>
</tr>
<tr>
    <td><label title="Maximum Number" for="txtMaxNum" class="Ektron_StandardLabel" id="lblMaxNum" runat="server">Maximum Number:</label></td>
    <td><input title="Enter Maximum Number here" type="text" name="txtMaxNum" id="txtMaxNum" value="" class="Ektron_NumberTextBox" disabled="disabled" />&#160;
        <input title="Unlimited Option" type="checkbox" name="chkUnlimitedNum" id="chkUnlimitedNum" checked="checked" onclick="ekFieldMinMaxControl.updateMaxNumField();" /><label title="Unlimited" for="chkUnlimitedNum" id="lblUnlimited" runat="server">Unlimited</label>
    </td>
</tr>
