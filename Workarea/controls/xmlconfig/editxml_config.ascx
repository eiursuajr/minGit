<%@ Control Language="C#" AutoEventWireup="true" Inherits="editxml_config" CodeFile="editxml_config.ascx.cs" %>

<script type="text/javascript">
<!--
	var m_ShrinkNav = false;
	function MyStartup() {
		//if (m_ShrinkNav == true) {
			//if ((typeof(top.ShrinkFrame) == "function")) {
				//top.ShrinkFrame();
			//}
		//}

		if (document.forms.length > 0) {
			//alert(document.forms.length);
			for (x in document.forms) {
				//alert(x.name);
				//document.forms[document.forms.length] = "xxxx"
			}

		}
	}
	function PopUpWindow (url, hWind, nWidth, nHeight, nScroll, nResize) {
	var cToolBar = "toolbar=0,location=0,directories=0,status=" + nResize + ",menubar=0,scrollbars=" + nScroll + ",resizable=" + nResize + ",width=" + nWidth + ",height=" + nHeight;
	var popupwin = window.open(url, hWind, cToolBar);
	return popupwin;
	}
	function ecmPopUpWindow (url, hWind, nWidth, nHeight, nScroll, nResize) {
	var cToolBar = 'toolbar=0,location=0,directories=0,status=' + nResize + ',menubar=0,scrollbars=' + nScroll + ',resizable=' + nResize + ',width=' + nWidth + ',height=' + nHeight;
	var popupwin = window.open(url, hWind, cToolBar);
	return popupwin;
	}


	function VerifyXmlForm () {
		document.forms[0].frm_xmltitle.value = Trim(document.forms[0].frm_xmltitle.value);
		if (document.forms[0].frm_xmltitle.value == "") {
			alert("<%= (m_refMsg.GetMessage("js: alert title required")) %>");
			document.forms[0].frm_xmltitle.focus();
			return false;
		}
		var objForm = document.forms[0].frm_xmltitle
		if (objForm != null && objForm.value != ""){
		    if((objForm.value.indexOf('\\') > -1) || (objForm.value.indexOf('*') > -1) || (objForm.value.indexOf('>') > -1)||(objForm.value.indexOf('<') > -1)||(objForm.value.indexOf('|') > -1)||(objForm.value.indexOf('\"') > -1) || (objForm.value.indexOf('/') > -1 ) ){
               alert("The title cannot contain '\\', '/', '*','>','<','|','\"'.");
               document.forms[0].frm_xmltitle.focus();
               return false;
               }
             }
		return true;
	}
	function CheckKeyValue(item, keys) {
		var keyArray = keys.split(",");
		for (var i = 0; i < keyArray.length; i++) {
			if ((document.layers) || ((!document.all) && (document.getElementById))) {
				if (item.which == keyArray[i]) {
					return false;
				}
			}
			else {
				if (event.keyCode == keyArray[i]) {
					return false;
				}
			}
		}
	}

	function Trim (string) {
		if (string.length > 0) {
			string = RemoveLeadingSpaces (string);
		}
		if (string.length > 0) {
			string = RemoveTrailingSpaces(string);
		}
		return string;
	}

	function RemoveLeadingSpaces(string) {
		while(string.substring(0, 1) == " ") {
			string = string.substring(1, string.length);
		}
		return string;
	}

	function RemoveTrailingSpaces(string) {
		while(string.substring((string.length - 1), string.length) == " ") {
			string = string.substring(0, (string.length - 1));
		}
		return string;
	}

	function SubmitForm(FormName, Validate) {
		if (Validate.length > 0) {
			if (eval(Validate)) {
				document.forms[0].submit();
				return false;
			}
			else {
				return false;
			}
		}
		else {
			document.forms[0].submit();
			return false;
		}
	}

	var bVerifying = false;
	var bPassed = true;
	var numOfVerifyLoops = 0;
	var strXslErrorMsg = "";
	var unique = 0;

	function VerifyXsltCallback (formFieldName, displayMsg) {
		if (bVerifying) {
			if (numOfVerifyLoops < 350) {
				setTimeout("VerifyXsltCallback('" + formFieldName + "', " + displayMsg + ")", 100);
				numOfVerifyLoops++;
				return false;
			}
		}
		bVerifying = false;
		if (bPassed) {
			if (displayMsg) {
				document.images["img_" + formFieldName].src="<%=(AppPath)%>images/UI/Icons/check.png";
				alert("<%=(m_refMsg.GetMessage("verification succeeded msg"))%>");
			}
			return (true);
		}
		else {
			if (displayMsg) {
				document.images["img_" + formFieldName].src="<%=(AppPath)%>images/UI/Icons/remove.png";
				alert (strXslErrorMsg);
			}
			return (false);
		}

	}

	function VerifyXslt(formFieldName) {
		var extension;
		var urlExtension;
		var thisExtension;
		var xslPath;

		if (bVerifying) {
			return false;
		}
		document.forms.xmlconfiguration[formFieldName].value = Trim(document.forms.xmlconfiguration[formFieldName].value);
		xslPath = document.forms.xmlconfiguration[formFieldName].value;
		if (xslPath.length == 0) {
			return false;
		}

		extension = xslPath.split("?");
		extension = extension[0].split(".");
		thisExtension = extension[extension.length - 1];
		if (((thisExtension == "asp") || (thisExtension == "aspx")
			|| (thisExtension == "cfm") || (thisExtension == "php"))
			&& ((xslPath.substring(0,7) != "http://") && (xslPath.substring(0,8) != "https://"))) {

			alert("<%=(m_refMsg.GetMessage("error: dynamic xslt need full path"))%>");
			return false;
		}
		unique++;
		if (document.all) {
			document.all["iframe1"].src="xml_verify.aspx?path=" + escape(xslPath);

		}
		else if (document.getElementById) {
			document.getElementById("iframe1").src="xml_verify.aspx?path=" + escape(xslPath) + "&num=" + unique;
		}
		else {
			document.layers["iframe1"].load("xml_verify.aspx?path=" + escape(xslPath) + "&num=" + unique, "100%");
		}
		bVerifying = true;
		bPassed = false;
		numOfVerifyLoops = 0;
		strXslErrorMsg = "Timeout";
		setTimeout("VerifyXsltCallback('" + formFieldName + "', " + true + ")", 100);
	}

	function MakeNoVerify (formName, item, keys) {
		if (document.forms.xmlconfiguration[formName.name + "_length"].value != formName.value) {
			document.images["img_" + formName.name].src = "<%=(AppPath)%>images/UI/Icons/contentValidate.png";
		}
		document.forms.xmlconfiguration[formName.name + "_length"].value = formName.value;
	}
// -->
</script>
<script type="text/javascript">
<!--
	function ShowPane(tabID) {
		var arTab = new Array("dvProperties", "dvDisplayInfo", "dvPreview");
		var dvShow; //tab
		var _dvShow; //pane
		var dvHide;
		var _dvHide;
		for (var i=0; i < arTab.length; i++) {
			if (tabID == arTab[i]) {
				dvShow = eval('document.getElementById("' + arTab[i] + '");');
				_dvShow = eval('document.getElementById("_' + arTab[i] + '");');
			} else {

				dvHide = eval('document.getElementById("' + arTab[i] + '");');
				if (dvHide != null) {
					dvHide.className = "tab_disabled";
				}
				_dvHide = eval('document.getElementById("_' + arTab[i] + '");');
				if (_dvHide != null) {
					_dvHide.style.display = "none";
				}
			}
		}
		_dvShow.style.display = "block";
		dvShow.className = "tab_actived";
	}
// -->
</script>

<div id="dhtmltooltip"></div>
<div class="ektronPageHeader">
    <div class="ektronTitlebar" id="txtTitleBar" runat="server"></div>
    <div class="ektronToolbar" id="htmToolBar" runat="server"></div>
</div>
<div class="ektronPageContainer ektronPageInfo">
    <fieldset>
		<legend><%=m_refMsg.GetMessage("general information")%></legend>
		<table class="ektronGrid">
		<tbody>
	    <asp:Literal runat="server" ID="lbl_addXmlError" Visible="false" />
		<tr>
		    <td class="label" title="Title"><%=m_refMsg.GetMessage("generic title label")%></td>
		    <td><input title="Title Text" maxlength="75" type="text" name="frm_xmltitle" value="<%=m_strTitle%>" onkeypress="return CheckKeyValue(event, '34,13');"/></td>
	    </tr>
	    <tr id="tr_desc" runat="server" visible="false">
		    <td class="label" valign="top" title="Description"><%=m_refMsg.GetMessage("generic description")%><asp:Literal ID="lbl_desc" runat="Server" />:</td>
		    <td><asp:TextBox ToolTip="Description Text" ID="txt_desc" runat="server" MaxLength="255" /></td>
	    </tr>
	<%if (((PageAction == "editxmlconfiguration") | (PageAction == "editproducttype")) & !(Page.IsPostBack)) {%>
	    <tr>
		    <td class="label"><%=m_refMsg.GetMessage("id label")%></td>
		    <td><%=cXmlCollection.Id%></td>
	    </tr>
	    <tr>
		    <td class="label" title="Description"><%=m_refMsg.GetMessage("description label")%></td>
		    <td><input title="Description Text" maxlength="255" type="text" name="frm_xmldescription" value="<%=cXmlCollection.Description%>" onkeypress="return CheckKeyValue(event,'34');"/></td>
	    </tr>
		</tbody>
	    </table>	
	</fieldset>
    <% if (pkDisplay.Length == 0) { //there is no package for this configuration , it is a lagacy configuration  = cXmlCollection("PackageDisplayXslt") %>
    <fieldset>
		<legend><%=m_refMsg.GetMessage("editor info label") + " <span class=\"ektronCaption\">(" + m_refMsg.GetMessage("files prefixed with msg") + " " + XmlPath + ")</span>"%></legend>
		<table class="ektronGrid">
		<tbody>
        <tr>
	        <td class="label" title="Edit XSLT"><%=m_refMsg.GetMessage("edit xslt label")%></td>
	        <td>
		        <input maxlength="255" type="text" title="Edit XSLT Text" name="frm_editxslt" value="<%=cXmlCollection.EditXslt%>" onkeyup="MakeNoVerify(this);" onkeypress="return CheckKeyValue(event,'34');"/>
		        <a href="#" onclick="VerifyXslt('frm_editxslt'); return false">
			        <img title="<%=(m_refMsg.GetMessage("alt text for xsl or schema verification"))%>" alt="<%=(m_refMsg.GetMessage("alt text for xsl or schema verification"))%>" src="<%=(AppPath)%>images/UI/Icons/contentValidate.png" border="0" name="img_frm_editxslt"></a>
	        </td>
        </tr>
        <tr>
	        <td class="label" title="Save XSLT"><%=m_refMsg.GetMessage("save xslt label")%></td>
	        <td>
		        <input maxlength="255" type="text" name="frm_savexslt" value="<%=cXmlCollection.SaveXslt%>" onkeyup="MakeNoVerify(this);" onkeypress="return CheckKeyValue(event,'34');"/>
		        <a href="#" onclick="VerifyXslt('frm_savexslt'); return false">
			        <img title="<%=(m_refMsg.GetMessage("alt text for xsl or schema verification"))%>" alt="<%=(m_refMsg.GetMessage("alt text for xsl or schema verification"))%>" src="<%=(AppPath)%>images/UI/Icons/contentValidate.png" border="0" name="img_frm_savexslt"></a>
	        </td>
        </tr>
        <tr>
	        <td class="label" title="Advanced Configuration"><%=m_refMsg.GetMessage("advanced config label")%></td>
	        <td valign="bottom"><input title="Advanced Configuration Text" maxlength="255" type="text" name="frm_xmladvconfig" value="<%=cXmlCollection.XmlAdvConfig%>" onkeypress="return CheckKeyValue(event,'34');"/></td>
        </tr>
		</tbody>
		</table>
    </fieldset>
    <fieldset>
        <legend><%=m_refMsg.GetMessage("validation info label") + " <span class=\'caption\'>(" + m_refMsg.GetMessage("files prefixed with msg") + " " + XmlPath + ")</span>"%></legend>
		<table class="ektronGrid">
		<tbody>
        <tr>
	        <td class="label" title="XML Schema"><%=m_refMsg.GetMessage("xml schema label")%></td>
	        <td>
		        <input maxlength="255" type="text" title="XML Schema Text" name="frm_xmlschema" value="<%=cXmlCollection.XmlSchema%>" onkeyup="MakeNoVerify(this);" onkeypress="return CheckKeyValue(event,'34');"/>
		        <a href="#" onclick="VerifyXslt('frm_xmlschema'); return false">
			    <image title="<%=(m_refMsg.GetMessage("alt text for xsl or schema verification"))%>" alt="<%=(m_refMsg.GetMessage("alt text for xsl or schema verification"))%>" src="<%=(AppPath)%>images/UI/Icons/contentValidate.png" border="0" name="img_frm_xmlschema"></a>
	        </td>
        </tr>
        <tr>
	        <td class="label" title="Target Namespace"><%=m_refMsg.GetMessage("target namespace label")%></td>
	        <td><input maxlength="255" title="Target Namespace Text" type="text" name="frm_xmlnamespace" value="<%=cXmlCollection.XmlNameSpace%>" onkeypress="return CheckKeyValue(event,'34');"/></td>
        </tr>
		</tbody>
		</table>
	</fieldset>
    <%}else{%>
    <input type="hidden" name="frm_editxslt" value="<%=cXmlCollection.EditXslt%>"/>
    <input type="hidden" name="frm_savexslt" value="<%=cXmlCollection.SaveXslt%>"/>
    <input type="hidden" name="frm_xmladvconfig" value="<%=cXmlCollection.XmlAdvConfig%>"/>
    <input type="hidden" name="frm_xmlschema" value="<%=cXmlCollection.XmlSchema%>"/>
    <input type="hidden" name="frm_xmlnamespace" value="<%=cXmlCollection.XmlNameSpace%>"/>
    <%}%>
    <fieldset>
        <legend title="Display Information"><%=m_refMsg.GetMessage("display info label") + " <span class=\"ektronCaption\">(" + m_refMsg.GetMessage("files prefixed with msg") + " " + XmlPath + ")</span>"%></legend>
        <span class="caption"><%=m_refMsg.GetMessage("default label")%></span>
	    <table class="ektronForm">
		<tbody>
		    <tr>
			    <td class="label">
				    <input type="radio" title="Selelct XSLT 1" name="frm_xsltdefault" value="1" <%if (cXmlCollection.DefaultXslt == "1" & cXmlCollection.Xslt1 != "") {
			    bDefaultXsltExists = true; %>checked<%}%>/>
				    <%=m_refMsg.GetMessage("xslt 1 label")%>
			    </td>
			    <td class="value">
				    <input title="XSLT 1 Text" maxlength="255" type="text" name="frm_Xslt1" value="<%=cXmlCollection.Xslt1%>" onkeyup="MakeNoVerify(this);" onkeypress="return CheckKeyValue(event,'34');"/>
				    <a href="#" title="Click here to verify this XSLT or Schema" onclick="VerifyXslt('frm_Xslt1'); return false">
					    <image title="<%=(m_refMsg.GetMessage("alt text for xsl or schema verification"))%>" alt="<%=(m_refMsg.GetMessage("alt text for xsl or schema verification"))%>" src="<%=(AppPath)%>images/UI/Icons/contentValidate.png" border="0" name="img_frm_Xslt1"></a>
			    </td>
		    </tr>
		    <tr>
			    <td class="label">
				    <input type="radio" title="Selelct XSLT 2" name="frm_xsltdefault" value="2" <%if (cXmlCollection.DefaultXslt == "2" & cXmlCollection.Xslt2 != "") {
			    bDefaultXsltExists = true; %>checked<%}%>/>
				    <%=m_refMsg.GetMessage("xslt 2 label")%>
			    </td>
			    <td class="value">
				    <input title="XSLT 2 Text" maxlength="255" type="text" name="frm_Xslt2" value="<%=cXmlCollection.Xslt2%>" onkeyup="MakeNoVerify(this);" onkeypress="return CheckKeyValue(event,'34');"/>
				    <a title="Click here to verify this XSLT or Schema" href="#" onclick="VerifyXslt('frm_Xslt2'); return false">
					    <image title="<%=(m_refMsg.GetMessage("alt text for xsl or schema verification"))%>" alt="<%=(m_refMsg.GetMessage("alt text for xsl or schema verification"))%>" src="<%=(AppPath)%>images/UI/Icons/contentValidate.png" border="0" name="img_frm_Xslt2"></a>
			    </td>
		    </tr>
		    <tr>
			    <td class="label">
				    <input type="radio" title="Selelct XSLT 3" name="frm_xsltdefault" value="3" <%if (cXmlCollection.DefaultXslt == "3" & cXmlCollection.Xslt3 != "") {
			    bDefaultXsltExists = true; %>checked<%}%>/>
				    <%=m_refMsg.GetMessage("xslt 3 label")%>
			    </td>
			    <td class="value">
				    <input title="XSLT 3 Text" maxlength="255" type="text" name="frm_Xslt3" value="<%=cXmlCollection.Xslt3%>" onkeyup="MakeNoVerify(this);" onkeypress="return CheckKeyValue(event,'34');"/>
				    <a title="Click here to verify this XSLT or Schema" href="#" onclick="VerifyXslt('frm_Xslt3'); return false">
					    <image title="<%=(m_refMsg.GetMessage("alt text for xsl or schema verification"))%>" alt="<%=(m_refMsg.GetMessage("alt text for xsl or schema verification"))%>" src="<%=(AppPath)%>images/UI/Icons/contentValidate.png" border="0" name="img_frm_Xslt3"></a>
			    </td>
		    </tr>
		    <tr>
			    <td class="label">
				    <input type="radio" title="Set XSLT as Default" name="frm_xsltdefault" value="0" <%if (cXmlCollection.DefaultXslt == "0" | !bDefaultXsltExists) {%>checked<%}%>/>
				    <%=m_refMsg.GetMessage("lbl XSLT packaged")%>:
			    </td>
			    <td>&nbsp;</td>
		    </tr>
	    </tbody>
	    </table>
    </fieldset>
    <input type="hidden" onkeypress="return CheckKeyValue(event,'34');" name="netscapefix"/>
    <input type="hidden" name="frm_collectionid" value="<%=(m_intId)%>"/>
    <input type="hidden" name="frm_Xslt4" value=""/>
    <input type="hidden" name="frm_Xslt5" value=""/>
    <input type="hidden" name="frm_editxslt_length" value="<%=cXmlCollection.EditXslt%>"/>
    <input type="hidden" name="frm_savexslt_length" value="<%=cXmlCollection.SaveXslt%>"/>
    <input type="hidden" name="frm_Xslt1_length" value="<%=cXmlCollection.Xslt1%>"/>
    <input type="hidden" name="frm_Xslt2_length" value="<%=cXmlCollection.Xslt2%>"/>
    <input type="hidden" name="frm_Xslt3_length" value="<%=cXmlCollection.Xslt3%>"/>
    <input type="hidden" name="frm_Xslt4_length" value="<%=cXmlCollection.Xslt4%>"/>
    <input type="hidden" name="frm_Xslt5_length" value="<%=cXmlCollection.Xslt5%>"/>
    <input type="hidden" name="frm_xmlschema_length" value="<%=cXmlCollection.XmlSchema%>"/>
    <%}else{%>
    		</tbody>
	    </table>	
	</fieldset>
    <%}%>
</div>

<iframe src="xml_config.aspx" hidefocus noresize width="1" height="1" name="iframe1" id="iframe1" frameborder="0">
</iframe>
