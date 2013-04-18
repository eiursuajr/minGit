<%@ Control Language="C#" %>
<script runat="server">
    private bool _enabled = true;

    public bool Enabled
    {
        get { return _enabled; }
        set { _enabled = value; }
    }

    protected void Page_Load(Object src, EventArgs e)
    {
		Ektron.Cms.Common.EkMessageHelper refMsg = (new Ektron.Cms.ContentAPI()).EkMsgRef;
		this.lblUse.InnerHtml = refMsg.GetMessage("lbl use");
		this.lblMayNotBeRemoved.InnerHtml = refMsg.GetMessage("lbl may not be removed");
		this.lblMayBeRemoved.InnerHtml = refMsg.GetMessage("lbl may be removed");
		
		UsePanel.Enabled = _enabled;
		if (!Page.ClientScript.IsClientScriptBlockRegistered("EkFieldUseScript"))
		{
			Page.ClientScript.RegisterClientScriptBlock(this.GetType(), "EkFieldUseScript", EkFieldUseScript.InnerText, true);
		}
		EkFieldUseScript.Visible = false;
	}
</script>
<clientscript id="EkFieldUseScript" runat="server">
function EkFieldUseControl()
{
	this.read = function(oFieldElem) 
	{
        if ("root" == oFieldElem.getAttribute("ektdesignns_role"))
		{
			this.disable();
		}
	    if ("0" === $ektron.toStr(oFieldElem.getAttribute("ektdesignns_minoccurs")))
	    {   
	        document.getElementById("optUse2").checked = true;
	    }
	    else
	    {
			document.getElementById("optUse1").checked = true;
	    }
	}
	this.update = function(oFieldElem)
	{
		if (document.getElementById("optUse2").checked && !document.getElementById("optUse2").disabled)
		{
			oFieldElem.setAttribute("ektdesignns_minoccurs", "0");
		}
		else
		{
			oFieldElem.removeAttribute("ektdesignns_minoccurs");
		}
	}
	this.enable = function()
	{
        document.getElementById("UseContainer").disabled = false;
        document.getElementById("optUse1").disabled = false;
        document.getElementById("optUse2").disabled = false;
	}
	this.disable = function(bResetOption)
	{
        if (typeof bResetOption != "undefined" && true == bResetOption)
        {
            document.getElementById("optUse1").checked = true;
        }
        document.getElementById("UseContainer").disabled = true;
        document.getElementById("optUse1").disabled = true;
        document.getElementById("optUse2").disabled = true;
	}
}
var ekFieldUseControl = new EkFieldUseControl();
</clientscript>

<asp:Panel ID="UsePanel" runat="server">
    <fieldset id="UseContainer">
		<legend title="Use" id="lblUse" runat="server">Use</legend>
		<div class="Ektron_TopSpaceVeryVerySmall">
			<input type="radio" title="Select May not remove Option" name="optUse" id="optUse1" value="True" checked="checked" /><label title="May not be removed" for="optUse1" id="lblMayNotBeRemoved" runat="server">May not be removed</label><br />
			<input type="radio" title="Select May remove Option" name="optUse" id="optUse2" value="False" /><label title="May be removed" for="optUse2" id="lblMayBeRemoved" runat="server">May be removed</label><br />
		</div>
    </fieldset>
</asp:Panel>
