<%@ Control Language="C#" %>
<script runat="server">
    private bool _nodetypevisible = true;
    private bool _typeelementenabled = true;
    private bool _typeattributeenabled = true;
    private bool _typecontentenabled = true;

    public bool NodeTypeVisible
    {
        get { return _nodetypevisible; }
        set { _nodetypevisible = value; }
    }
    public bool TypeElementEnabled
    {
        get { return _typeelementenabled; }
        set { _typeelementenabled = value; }
    }
    public bool TypeAttributeEnabled
    {
        get { return _typeattributeenabled; }
        set { _typeattributeenabled = value; }
    }
    public bool TypeContentEnabled
    {
        get { return _typecontentenabled; }
        set { _typecontentenabled = value; }
    }
	
    protected void Page_Load(Object src, EventArgs e)
    {
		Ektron.Cms.Common.EkMessageHelper refMsg = (new Ektron.Cms.ContentAPI()).EkMsgRef;
		this.lblFieldName.InnerHtml = refMsg.GetMessage("lbl field name");
		this.lblFieldName.Attributes["for"] = this.ClientID + "_txtFldName";
		this.lblType.InnerHtml = refMsg.GetMessage("generic type");
		this.rdElement.Text = refMsg.GetMessage("lbl element");
        this.rdElement.ToolTip = "Select " + refMsg.GetMessage("lbl element");
		this.rdAttribute.Text = refMsg.GetMessage("lbl attribute");
        this.rdAttribute.ToolTip = "Select " + refMsg.GetMessage("lbl attribute");
		this.rdContent.Text = refMsg.GetMessage("lbl content");
        this.rdContent.ToolTip = "Select " + refMsg.GetMessage("lbl content");
		this.lblRootTag.InnerHtml = refMsg.GetMessage("lbl root tag");
        this.rdTagAsElement.Text = refMsg.GetMessage("lbl use root");
        this.rdTagAsElement.ToolTip = "Use <root> as the root tag";
        this.rdTagAsRoot.Text = refMsg.GetMessage("lbl use as root");
        this.rdTagAsRoot.ToolTip = refMsg.GetMessage("lbl use as root");

        FieldNodeType.Visible = _nodetypevisible;
		rdElement.Enabled = _typeelementenabled;
		rdAttribute.Enabled = _typeattributeenabled;
		rdContent.Enabled = _typecontentenabled;
        if (!Page.ClientScript.IsClientScriptBlockRegistered("EkFieldAdvancedScript"))
		{
			StringBuilder sbScript = new StringBuilder();
			sbScript.AppendLine();
			sbScript.AppendLine(@"var EkFieldAdvancedResourceText = ");
			sbScript.AppendLine(@"{");
			sbScript.Append(@"	sValue: """);
			sbScript.Append(refMsg.GetMessage("lbl content"));
			sbScript.AppendLine(@"""");
			sbScript.Append(@",	sField: """);
			sbScript.Append(refMsg.GetMessage("lbl field"));
			sbScript.AppendLine(@"""");
			sbScript.Append(@",	sContentInParens: """);
			sbScript.Append(refMsg.GetMessage("lbl content in parens"));
			sbScript.AppendLine(@"""");
			sbScript.Append(@",	sEg: """);
			sbScript.Append(refMsg.GetMessage("generic eg"));
			sbScript.AppendLine(@"""");
			sbScript.AppendLine(@"};");
			sbScript.AppendLine(EkFieldAdvancedScript.InnerText.Replace("<%=this.ClientID%>", this.ClientID));
            Page.ClientScript.RegisterClientScriptBlock(this.GetType(), "EkFieldAdvancedScript", sbScript.ToString(), true);
		}
        EkFieldAdvancedScript.Visible = false;
	}
</script>
<clientscript id="EkFieldAdvancedScript" runat="server">
function EkFieldAdvancedControl(clientID)
{
	this.myform = document.forms[0];
	this.clientID = clientID;
	this.name = "";
	this.advancedName = "";
	this.sDefaultVal = EkFieldAdvancedResourceText.sValue;
	this.nameCtlObj = null;
	this.attributeTypeEnabled; // undefined
	this.contentTypeEnabled; // undefined
	this.contentNodeType = "text"; // "text" or "mixed"
	this.repeatable = false;
	
	this.getName = function()
	{
        var name = (document.getElementById(this.clientID + "_txtFldName").value || this.name);
        var nodeType = this.getNodeType();
        if ("@" == name.substr(0, 1))
        {
            name = name.substr(1);
        }
        else if ("text" == nodeType || "mixed" == nodeType)
        {
            name = this.name;
        }
        this.name = name;
        return name;
	};
	this.setName = function(name)
	{
	    this.name = name;
	    document.getElementById(this.clientID + "_txtFldName").value = name;
	};
	this.getAdvancedName = function()
	{
	    var name = this.getName();
        var nodeType = this.getNodeType();
        if ("attribute" == nodeType)
        {
            name = "@" + name;
        }
        else if ("text" == nodeType || "mixed" == nodeType)
        {
            name = EkFieldAdvancedResourceText.sContentInParens;
        }
        this.advancedName = name;
        return name;
	};
	
	this.getNodeType = function()
	{
	    var nodeType = "";
	    if (document.getElementById(this.clientID + "_rdElement").checked)
        {
            nodeType = "element";
        }
        else if (document.getElementById(this.clientID + "_rdAttribute").checked)
        {
            nodeType = "attribute";
        }
        else if (document.getElementById(this.clientID + "_rdContent").checked)
        {
			nodeType = this.contentNodeType; // "text" or "mixed"
        }
        return nodeType;
	};
	
	this.getValueOption = function()
	{
	    for (var i = 0; i < this.myform.rdValue.length; i++)
        {
            if (this.myform.rdValue[i].checked)
            {
                return this.myform.rdValue[i].value;
            }
        }
	};
	
	this.getUseAsRoot = function()
	{
        if (document.getElementById("spRoot") != null && "" == document.getElementById("FieldRootTag").style.display)
        {
            var rdRootVal = "";
	        if (document.getElementById(this.clientID + "_rdTagAsRoot").checked)
            {
                rdRootVal = "root";
            }
            else if (document.getElementById(this.clientID + "_rdTagAsElement").checked)
            {
                rdRootVal = "element";
            }
            return rdRootVal;
        }
        else
        {
            return "";
        }
	};
	
	this.updateFieldAllowControl = function(objCtl)
	{
	    var bEnabled = (this.getUseAsRoot() != "root") && ("element" == this.getNodeType());
	    if (bEnabled)
	    {
			objCtl.enable();
	    }
	    else
	    {
			objCtl.disable();
	    }	    
	};

    this.updateFieldUseControl = function(objCtl)
	{
	    var bEnabled = (this.getUseAsRoot() != "root") && ("element" == this.getNodeType());
	    if (bEnabled)
	    {
			objCtl.enable();
	    }
	    else
	    {
			objCtl.disable(true);
	    }	    
	};
	
	this.updateFieldNameControl = function(objCtl)
	{
	    objCtl.setName(this.getName());
	    objCtl.setAdvancedName(this.getAdvancedName());
	    var bEnabled = ("element" == this.getNodeType());	    
	    objCtl.enableFieldName(bEnabled);
	};
	
	this.updateControl = function(objNameCtl, bRepeatable)
	{
	    this.nameCtlObj = objNameCtl;
	    var fieldName = objNameCtl.getName();
	    var nodeType = this.getNodeType()
        if ("text" == nodeType || "mixed" == nodeType)
        {
            fieldName = this.getName();    
        }
        else
        {
            if (0 == fieldName.length)
            {
                var displayName = objNameCtl.getDisplayName();
                if (0 == displayName.length)
                {
                    fieldName = EkFieldAdvancedResourceText.sField;
                }
                else
                {
                    fieldName = EkFormFields_FixId(displayName);
                }
            }
            //this.setName(fieldName);
        }
        this.setName(fieldName);
        this.repeatable = (true === bRepeatable);
        this.updateOptions();
	};
	
	this.setDefaultVal = function(value)
	{
	    this.sDefaultVal = value;    
	};
	
	this.setRootTagVisible = function(bVisible)
	{
	    if (bVisible)
	    {
	        document.getElementById("FieldRootTag").style.display = "";
	    }
	    else
	    {
	        document.getElementById("FieldRootTag").style.display = "none";
	    }
	};
	
	this.read = function(oFieldElem)
	{
	    var nodeType = $ektron.toStr(oFieldElem.getAttribute("ektdesignns_nodetype"), "element");
	    this.name = (oFieldElem.id || EkFieldAdvancedResourceText.sField);
	    if ("element" == nodeType)
        {
            document.getElementById(this.clientID + "_rdElement").checked = true;
        }
        else if ("attribute" == nodeType)
        {
            document.getElementById(this.clientID + "_rdAttribute").checked = true;
        }
        else if ("text" == nodeType)
        {
            document.getElementById(this.clientID + "_rdContent").checked = true;
        }
	    var useAsRoot = oFieldElem.getAttribute("ektdesignns_role");
	    if (document.getElementById("spRoot") != null)
	    {
	        if ("root" == useAsRoot)
	        {
	            document.getElementById(this.clientID + "_rdTagAsRoot").checked = true;
	        } 
	        else
	        {
	            document.getElementById(this.clientID + "_rdTagAsElement").checked = true;    
	        }   
	    }
	    this.enableFieldName(nodeType != "text");
	    var maxoccurs = $ektron.toStr(oFieldElem.getAttribute("ektdesignns_maxoccurs"), "1");
	    this.repeatable = (maxoccurs != "1");
	};
	
	this.update = function(oFieldElem)
	{
	    oFieldElem.setAttribute("ektdesignns_nodetype", this.getNodeType());
	    oFieldElem.removeAttribute("ektdesignns_role");
	    if ("root" == this.getUseAsRoot()) 
	    {
	        oFieldElem.setAttribute("ektdesignns_role", "root");
	    }
	    this.nameCtlObj = null;
	};
	
    this.updateOptions = function()
	{
		var fieldName = (document.getElementById(this.clientID + "_txtFldName").value || this.name);
		var fieldDefault = this.sDefaultVal;
		var oElementEx = document.getElementById("spElement");
		var oAttributeEx = document.getElementById("spAttribute");
		var oContentEx = document.getElementById("spContent");
		var oRootEx = document.getElementById("spRoot");
		var oElemRootEx = document.getElementById("spElemRoot");
		var sEg = EkFieldAdvancedResourceText.sEg;
		var bEnabled;
		if (oElementEx != null)
		{
		    oElementEx.innerHTML = sEg + " &amp;lt;" + fieldName + "&amp;gt;" + fieldDefault + "&amp;lt;\/" + fieldName + "&amp;gt;";
		    oAttributeEx.innerHTML = sEg + " &amp;lt;root " + fieldName + "=\"" + fieldDefault + "\"&amp;gt;...&amp;lt;/root&amp;gt;";
			if ("undefined" == typeof this.attributeTypeEnabled)
			{
				this.attributeTypeEnabled = !document.getElementById(this.clientID + "_rdAttribute").disabled;
			}
			bEnabled = this.attributeTypeEnabled && (this.getUseAsRoot() != "root") && !this.repeatable;
			document.getElementById(this.clientID + "_rdAttribute").disabled = !bEnabled;
			oAttributeEx.style.display = (bEnabled ? "" : "none"); 
		    oContentEx.innerHTML = sEg + " &amp;lt;root&amp;gt;" + fieldDefault + "&amp;lt;/root&amp;gt;";
		    if ("undefined" == typeof this.contentTypeEnabled)
		    {
				this.contentTypeEnabled = !document.getElementById(this.clientID + "_rdContent").disabled;
		    }
		    bEnabled = this.contentTypeEnabled && (this.getUseAsRoot() != "root") && !this.repeatable;
		    document.getElementById(this.clientID + "_rdContent").disabled = !bEnabled;
			oContentEx.style.display = (bEnabled ? "" : "none"); 
		}
		if (oRootEx != null)
		{
		    oRootEx.innerHTML = sEg + " &amp;lt;root&amp;gt;&amp;lt;" + fieldName + "&amp;gt;...&amp;lt;/" + fieldName + "&amp;gt;&amp;lt;/root&amp;gt;";
		    oElemRootEx.innerHTML = sEg + " &amp;lt;" + fieldName + "&amp;gt;...&amp;lt;/" + fieldName + "&amp;gt;";
			bEnabled = ("element" == this.getNodeType() && !this.repeatable);
			document.getElementById(this.clientID + "_rdTagAsRoot").disabled = !bEnabled;
			oElemRootEx.style.display = (bEnabled ? "" : "none"); 
        }
        if (this.nameCtlObj && fieldName != this.name)
        {
            this.name = fieldName;
            this.nameCtlObj.setName(fieldName);
        }
	};
	
	this.updateDisplay = function(elementEx, attributeEx, contentEx)
	{
	    var oElementEx = document.getElementById("spElement");
		var oAttributeEx = document.getElementById("spAttribute");
		var oContentEx = document.getElementById("spContent");
		oElementEx.innerHTML = elementEx;
		oAttributeEx.innerHTML = attributeEx;
		oContentEx.innerHTML = contentEx;   
	};
	
	this.enableFieldName = function()
	{
		var nodeType = this.getNodeType();
	    var bEnabled = (nodeType != "text" && nodeType != "mixed");
	    document.getElementById(this.clientID + "_txtFldName").disabled = !bEnabled;
	};
	
	this.enableAttributeType = function(bEnabled)
	{
		this.attributeTypeEnabled = bEnabled;
		bEnabled = bEnabled && (this.getUseAsRoot() != "root");
	    document.getElementById(this.clientID + "_rdAttribute").disabled = !bEnabled;
	    document.getElementById("spAttribute").style.display = (bEnabled ? "" : "none"); 
	};
}
var ekFieldAdvancedControl = new EkFieldAdvancedControl("&lt;%=this.ClientID%&gt;");
</clientscript>
<tr>
    <td><label title="Field Name" for="txtFldName" class="Ektron_StandardLabel" id="lblFieldName" runat="server">Field Name:</label></td>
	<td><input title="Enter Field Name here" type="text" name="txtFldName" class="Ektron_StandardTextBox" id="<%=this.ClientID%>_txtFldName" disabled="disabled" readonly="readonly" /></td>
</tr>
<asp:PlaceHolder ID="FieldNodeType" runat="server">
<tr>
    <td colspan="2">
        <fieldset class="Ektron_TopSpaceVeryVerySmall">
            <legend id="lblType" runat="server" title="Type">Type</legend>
            <div class="Ektron_TopSpaceVerySmall">
                <asp:RadioButton ID="rdElement" GroupName="rdType" Checked="true" onclick="ekFieldAdvancedControl.updateOptions()" runat="server" /><br />
                &#160;&#160;&#160;&#160;&#160;&#160;&#160;&#160;<span id="spElement"></span><br />
                <asp:RadioButton ID="rdAttribute" GroupName="rdType" onclick="ekFieldAdvancedControl.updateOptions()" runat="server" /><br />
                &#160;&#160;&#160;&#160;&#160;&#160;&#160;&#160;<span id="spAttribute"></span><br />
                <asp:RadioButton ID="rdContent" GroupName="rdType" onclick="ekFieldAdvancedControl.updateOptions()" runat="server" /><br />
                &#160;&#160;&#160;&#160;&#160;&#160;&#160;&#160;<span id="spContent"></span><br />
            </div>
        </fieldset>
	</td>
</tr>
</asp:PlaceHolder>
<tr>
    <td colspan="2">
        <fieldset id="FieldRootTag" class="Ektron_TopSpaceVeryVerySmall">
            <legend id="lblRootTag" runat="server" title="Root Tag">Root Tag</legend>
            <div class="Ektron_TopSpaceVerySmall">
                <asp:RadioButton ID="rdTagAsElement" GroupName="rdRoot" Checked="true" onclick="ekFieldAdvancedControl.updateOptions()" runat="server" /><br />
                &#160;&#160;&#160;&#160;&#160;&#160;&#160;&#160;<span id="spRoot"></span><br />
                <asp:RadioButton ID="rdTagAsRoot" GroupName="rdRoot" onclick="ekFieldAdvancedControl.updateOptions()" runat="server" /><br />
                &#160;&#160;&#160;&#160;&#160;&#160;&#160;&#160;<span id="spElemRoot"></span><br />
            </div>
        </fieldset>
	</td>
</tr>
