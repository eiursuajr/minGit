<%@ Control Language="C#" %>
<script runat="server">
    protected void Page_Load(Object src, EventArgs e)
    {
		Ektron.Cms.Common.EkMessageHelper refMsg = (new Ektron.Cms.ContentAPI()).EkMsgRef;
		this.lblValue.InnerHtml = refMsg.GetMessage("lbl value");
		this.lblValueIsElement.InnerHtml = refMsg.GetMessage("lbl value is element");
		this.lblValueIsElement.Attributes["for"] = this.ClientID + "_rdValueElement";
		this.lblValueIsText.InnerHtml = refMsg.GetMessage("lbl value is text");
		this.lblValueIsText.Attributes["for"] = this.ClientID + "_rdValueText";
		
		if (!Page.ClientScript.IsClientScriptBlockRegistered("EkFieldValueOptionScript"))
		{
			StringBuilder sbScript = new StringBuilder();
			sbScript.AppendLine();
			sbScript.AppendLine(@"var EkFieldValueOptionResourceText = ");
			sbScript.AppendLine(@"{");
			sbScript.Append(@"	sEg: """);
			sbScript.Append(refMsg.GetMessage("generic eg"));
			sbScript.AppendLine(@"""");
			sbScript.Append(@",	sUrl: """);
			sbScript.Append(refMsg.GetMessage("lbl url"));
			sbScript.AppendLine(@"""");
			sbScript.AppendLine(@"};");
			sbScript.AppendLine(EkFieldValueOptionScript.InnerText.Replace("<%=this.ClientID%>", this.ClientID));
            Page.ClientScript.RegisterClientScriptBlock(this.GetType(), "EkFieldValueOptionScript", sbScript.ToString(), true);
		}
        EkFieldValueOptionScript.Visible = false;
	}
</script>
<clientscript id="EkFieldValueOptionScript" runat="server">
function EkFieldValueOptionControl(clientID)
{
	this.clientID = clientID;
	this.defaultElemVal = "";
	this.defaultTextVal = "";
	this.elemEx4ElemVal = "";
	this.contEx4ElemVal = "";
	this.elemEx4TextVal = "";
	this.attrEx4TextVal = "";
	this.contEx4TextVal = "";
	this.UrlOnly = false;
	this.attrVal = "";
	this.attrExt = "";
	this.read = function(oFieldElem) 
	{
	    var strValue = $ektron.toStr(oFieldElem.getAttribute("ektdesignns_content"));
        if (strValue.indexOf("@") > -1)
        {
            document.getElementById(this.clientID + "_rdValueText").checked = true; //ContentValue_Text
            this.UrlOnly = true;
        }
        else
        {
            document.getElementById(this.clientID + "_rdValueElement").checked = true; //ContentValue_Element
            this.UrlOnly = false;
        }
	}
	this.update = function(oFieldElem)
	{
        var strValue = this.attrVal;
        this.UrlOnly = false;
        if (document.getElementById(this.clientID + "_rdValueText").checked) 
        {
            strValue += this.attrExt;
            this.UrlOnly = true;
        }
        oFieldElem.setAttribute("ektdesignns_content", strValue);
	}
	this.updateOptions = function(objAdvCtl)
	{
	    var nodeType = objAdvCtl.getNodeType();
	    var oValElementEx = document.getElementById("spValElement");
	    var oValTextEx = document.getElementById("spValText");
	    var oValElement = document.getElementById(this.clientID + "_rdValueElement");
	    if (oValElement.checked)
	    {
			objAdvCtl.contentNodeType = "mixed";
			objAdvCtl.setDefaultVal(this.defaultElemVal);
	        objAdvCtl.enableAttributeType(false);  
	    }
	    else //(document.getElementById(this.clientID + "_rdValueText").checked)
	    {
			objAdvCtl.contentNodeType = "text";
			objAdvCtl.setDefaultVal(this.defaultTextVal);
	        objAdvCtl.enableAttributeType(true);
	    }    
		oValElement.disabled = ("attribute" == nodeType);
        if ("element" == nodeType)
        {
            oValElementEx.innerHTML = this.elemEx4ElemVal;
            oValTextEx.innerHTML = this.elemEx4TextVal;
        }
        else if ("attribute" == nodeType)
        {
            oValElementEx.innerHTML = "";
            oValTextEx.innerHTML = this.attrEx4TextVal;
        }
        else if ("text" == nodeType)
        {
            oValElementEx.innerHTML = this.contEx4ElemVal;
            oValTextEx.innerHTML = this.contEx4TextVal;
        }
	}
	this.updateDisplay = function(objAdvCtl)
	{
	    if (document.getElementById(this.clientID + "_rdValueElement").checked)
	    {
	        objAdvCtl.updateDisplay(this.elemEx4ElemVal, "", this.contEx4ElemVal);  
	    }
	    else //(document.getElementById(this.clientID + "_rdValueText").checked)
	    {
	        objAdvCtl.updateDisplay(this.elemEx4TextVal, this.attrEx4TextVal, this.contEx4TextVal);
	    } 
	}
	this.setDefaultVal = function(objAdvCtl, sElementDefault)
	{
	    var fieldName = objAdvCtl.getName();
	    this.defaultTextVal = EkFieldValueOptionResourceText.sUrl;
	    this.defaultElemVal = $ektron.htmlEncode(sElementDefault);
	    var sEg = EkFieldValueOptionResourceText.sEg;
	    this.elemEx4ElemVal = sEg + " &amp;lt;" + fieldName + "&amp;gt;" + this.defaultElemVal + "&amp;lt;\/" + fieldName + "&amp;gt;";
	    this.contEx4ElemVal = sEg + " &amp;lt;root&amp;gt;" + this.defaultElemVal + "&amp;lt;\/root&amp;gt;";
	    this.elemEx4TextVal = sEg + " &amp;lt;" + fieldName + "&amp;gt;" + this.defaultTextVal + "&amp;lt;\/" + fieldName + "&amp;gt;";
	    this.attrEx4TextVal = sEg + " &amp;lt;root " + fieldName + "=\"" + this.defaultTextVal + "\"&amp;gt;...&amp;lt;\/root&amp;gt;";
	    this.contEx4TextVal = sEg + " &amp;lt;root&amp;gt;" + this.defaultTextVal + "&amp;lt;\/root&amp;gt;";
	    if ("&lt;a&gt;&lt;/a&gt;" == sElementDefault)
	    {
	        this.attrVal = "element=a";
	        this.attrExt = "/@href";
	    }
	    else if ("&lt;img /&gt;" == sElementDefault)
	    {
	        this.attrVal = "element=img";
	        this.attrExt = "/@src";
	    }
	    else
	    {
	        alert("unknown default value: '" + sElementDefault + "'");
	        this.attrVal = "";
	        this.attrExt = "";
	    }
	}
	this.isUrlOnly = function()
	{
	    return this.UrlOnly;
	}
}
var ekFieldValueOptionControl = new EkFieldValueOptionControl("&lt;%=this.ClientID%&gt;");
</clientscript>
<script language="javascript" type="text/javascript">
<!--
    function EnableDefaultDesc(bEnabled)
    {
        var objDesc = document.getElementById("txtDescription");
        var objTarget = document.getElementById("cboTarget");
        if (objDesc)
        {
            objDesc.disabled = !bEnabled;
        }
        if (objTarget)
        {
            objTarget.disabled = !bEnabled;
        }
    }
//-->
</script>
<asp:PlaceHolder ID="FieldValue" runat="server">
<tr>
    <td colspan="3">
    <fieldset class="Ektron_TopSpaceVeryVerySmall">
        <legend title="Value" id="lblValue" runat="server">Value</legend>
        <div>
            <input type="radio" title="Select Value as an element" name="rdValue" id="<%=this.ClientID%>_rdValueElement" checked="checked" value="element" onclick="EnableDefaultDesc(true)" /><label title="Value is an element" for="rdValueElement" id="lblValueIsElement" runat="server">Value is an element</label><br />
            &#160;&#160;&#160;&#160;&#160;&#160;&#160;&#160;<span id="spValElement"></span><br />
            <input type="radio" title="Select Value as plain text" name="rdValue" id="<%=this.ClientID%>_rdValueText" value="text" onclick="EnableDefaultDesc(false)" /><label title="Value is plain text" for="rdValueText" id="lblValueIsText" runat="server">Value is plain text</label><br />
            &#160;&#160;&#160;&#160;&#160;&#160;&#160;&#160;<span id="spValText"></span><br />
        </div>
    </fieldset>
	</td>
</tr>
</asp:PlaceHolder> 