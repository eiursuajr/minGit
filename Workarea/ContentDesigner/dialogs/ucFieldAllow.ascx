<%@ Control Language="C#" %>
<script runat="server">
    private bool _enabled = true;
    private bool _allowMaxOccursLimit = false;

    public bool Enabled
    {
        get { return _enabled; }
        set { _enabled = value; }
    }

    public bool AllowMaxOccursLimit
    {
        get { return _allowMaxOccursLimit; }
        set { _allowMaxOccursLimit = value; }
    }
    
    protected void Page_Load(Object src, EventArgs e)
    {
		Ektron.Cms.Common.EkMessageHelper refMsg = (new Ektron.Cms.ContentAPI()).EkMsgRef;
		this.lblAllow.InnerHtml = refMsg.GetMessage("lbl allow");
		this.lblOnlyOne.InnerHtml = refMsg.GetMessage("lbl only one");
		this.lblMoreThanOne.InnerHtml = refMsg.GetMessage("lbl more than one");
			
        AllowPanel.Enabled = _enabled;
        dvMaxOccur.Visible = _allowMaxOccursLimit;
		if (!Page.ClientScript.IsClientScriptBlockRegistered("EkFieldAllowScript"))
		{
			Page.ClientScript.RegisterClientScriptBlock(this.GetType(), "EkFieldAllowScript", EkFieldAllowScript.InnerText, true);
		}
		EkFieldAllowScript.Visible = false;
	}
</script>
<clientscript id="EkFieldAllowScript" runat="server">
function EkFieldAllowControl()
{
	this.read = function(oFieldElem) 
	{
		if ("root" == oFieldElem.getAttribute("ektdesignns_role"))
		{
			this.disable();
		}
	    else if ("unbounded" == oFieldElem.getAttribute("ektdesignns_maxoccurs"))
	    {   
	        document.getElementById("optAllow2").checked = true;
            if (document.getElementById("chkUnlimitedNum") != null)
            {
                document.getElementById("chkUnlimitedNum").checked = true;
            }
	    }
        else if (document.getElementById("txtMaxNum") != null)
        {
            var iMaxNum = parseInt(oFieldElem.getAttribute("ektdesignns_maxoccurs"), 10);
            if (!isNaN(iMaxNum) && iMaxNum > 1)
            {
                document.getElementById("optAllow2").checked = true;
                document.getElementById("chkUnlimitedNum").checked = false;
                document.getElementById("txtMaxNum").value = iMaxNum;
                document.getElementById("txtMaxNum").disabled = false;
            }
            else
            {
                document.getElementById("optAllow1").checked = true;
                if (document.getElementById("txtMaxNum") != null)
                {
                    this.enableMaxNumField(false);
                }
            }
        }
	    else
	    {
			document.getElementById("optAllow1").checked = true;
            if (document.getElementById("txtMaxNum") != null)
            {
                this.enableMaxNumField(false);
            }
	    }
	};
	this.update = function(oFieldElem)
	{
		if (document.getElementById("optAllow2").checked)
		{
            if (document.getElementById("txtMaxNum") != null)
            {
                var iMaxNum = parseInt(document.getElementById("txtMaxNum").value, 10);
                if (false == document.getElementById("chkUnlimitedNum").checked && !isNaN(iMaxNum))
                {
			        if (iMaxNum < 2)
                    {
                        // do not need this attribute when max occurs is one (1).
                        oFieldElem.removeAttribute("ektdesignns_maxoccurs");
                    }
                    else
                    {
                        oFieldElem.setAttribute("ektdesignns_maxoccurs", iMaxNum);
                    }
                }
                else
                {
                    oFieldElem.setAttribute("ektdesignns_maxoccurs", "unbounded");
                }
            }
            else
            {
                oFieldElem.setAttribute("ektdesignns_maxoccurs", "unbounded");
            }
		}
		else
		{
			oFieldElem.removeAttribute("ektdesignns_maxoccurs");
		}
	};
	this.getMaxOccurs = function()
	{
		return (document.getElementById("optAllow2").checked ? "unbounded" : 1);
	};
	this.isRepeatable = function()
	{
		return (document.getElementById("optAllow2").checked);
	};
	this.enable = function()
	{
        document.getElementById("AllowContainer").disabled = false;
        document.getElementById("optAllow1").disabled = false;
        document.getElementById("optAllow2").disabled = false;
	};
	this.disable = function()
	{
		document.getElementById("optAllow1").checked = true;
        document.getElementById("AllowContainer").disabled = true;
        document.getElementById("optAllow1").disabled = true;
        document.getElementById("optAllow2").disabled = true;
	};
    this.getMax = function()
	{
		if (document.getElementById("chkUnlimitedNum").checked)
		{
			return "unbounded";
		}
		else
		{
			var maxNum = $ektron.toInt(document.getElementById("txtMaxNum").value, 1);
			if (maxNum < 2) maxNum = 2; //min number is 2.
			var minNum = 2;
			if (maxNum < minNum) maxNum = minNum;
			return maxNum;
		}
	};
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
	};
    this.enableMaxNumField = function(bEnbaled)
    {
        if (document.getElementById("txtMaxNum") != null)
        {
            document.getElementById("chkUnlimitedNum").disabled = !bEnbaled;
            document.getElementById("txtMaxNum").disabled = !bEnbaled;
            if (true == document.getElementById("chkUnlimitedNum").disabled)
            {
                document.getElementById("txtMaxNum").disabled = true;
            }
            else if (true == document.getElementById("chkUnlimitedNum").checked)
            {
                document.getElementById("txtMaxNum").disabled = true;
            }
        }
    };
    this.updateMaxNumField = function()
    {
        this.enableMaxNumField(document.getElementById("optAllow2").checked);
        if (document.getElementById("chkUnlimitedNum").disabled) return;
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
    };
}
var ekFieldAllowControl = new EkFieldAllowControl();
</clientscript>
<asp:Panel ID="AllowPanel" runat="server">
	<fieldset id="AllowContainer">
		<legend title="Allow" id="lblAllow" runat="server">Allow</legend>
		<div class="Ektron_TopSpaceVeryVerySmall">
			<input title="Only one" type="radio" name="optAllow" id="optAllow1" value="True" checked="checked" onclick="ekFieldAllowControl.enableMaxNumField(!this.checked);" /><label title="Only one" for="optAllow1" id="lblOnlyOne" runat="server">Only one</label><br />
			<input title="More than one" type="radio" name="optAllow" id="optAllow2" value="False" onclick="ekFieldAllowControl.enableMaxNumField(this.checked);" /><label title="More than one" for="optAllow2" id="lblMoreThanOne" runat="server">More than one</label><br />
            <div id="dvMaxOccur" runat="server" visible="false">
            <label title="Maximum Number" for="txtMaxNum" class="Ektron_StandardLabel" id="lblMaxNum" runat="server">Max.</label> 
            <input title="Enter Maximum Number here" type="text" name="txtMaxNum" id="txtMaxNum" value="" class="Ektron_NumberTextBox" disabled="disabled" />&#160;<input title="Unlimited Option" type="checkbox" name="chkUnlimitedNum" id="chkUnlimitedNum" checked="checked" onclick="ekFieldAllowControl.updateMaxNumField();" /><label title="Unlimited" for="chkUnlimitedNum" id="lblUnlimited" runat="server">Unlimited</label>
            </div>
		</div>
	</fieldset>
</asp:Panel> 