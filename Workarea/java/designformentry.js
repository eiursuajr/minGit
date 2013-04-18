/* Copyright 2003-2008, Ektron, Inc. */

var g_design_srcPath = "";

//nodeType 
var ELEMENT_NODE = 1;
var TEXT_NODE = 3;
var CDATA_SECTION_NODE = 4;

var g_design_ektPlatformInfo = null;
function design_isSafari()
{
	if (null == g_design_ektPlatformInfo && "function" == typeof PlatformInfo)
	{
		g_design_ektPlatformInfo = new PlatformInfo;
	}

	if (g_design_ektPlatformInfo)
	{
		return (g_design_ektPlatformInfo.isSafari);
	}
	else
	{
		return (false);
	}
}

function design_onsubmitForm(form)
// form is name or index of, or reference to, the form element to validate.
// Returns true if valid element, or false.
{
	var oElem = design_validateHtmlForm(form);
	if (oElem && oElem.title != "") {
		alert(oElem.title);
		if ('function' == typeof oElem.scrollIntoView || 'object' == typeof oElem.scrollIntoView) 
		{
			oElem.scrollIntoView();
		}
		if (design_canElementReceiveFocus(oElem)) 
		{
			oElem.focus();
		}
		return false;
	}
	return true;
}

function design_validateHtmlForm(form)
// form is name or index of, or reference to, the form element to validate.
// Returns first invalid element, or null.
{
	// #34658 - Ektron Form Validation Does Not Fire On Pages with an asp:ScriptManager
	// Problem is that ScriptManager calls form onsubmit function with 'this' referring to its own object rather than the form element. That object has a '_form' property that points to the form element.
	if (form && form._form) form = form._form; 
	//  validation is supported for browser NS 6.2+ and IE 5+
	if (null == g_design_ektPlatformInfo && "function" == typeof PlatformInfo)
	{
		g_design_ektPlatformInfo = new PlatformInfo;
	}
	if (g_design_ektPlatformInfo)
	{
		if (g_design_ektPlatformInfo.isNetscape && g_design_ektPlatformInfo.browserVersion < 6.2) return null;
		if (g_design_ektPlatformInfo.isIE && g_design_ektPlatformInfo.browserVersion < 5) return null;
	}
	
	var oForm;
	switch (typeof form)
	{
	case "string":
	case "number":
		oForm = document.forms[form];
		break;
	case "object":
		oForm = form;
		break;
	default:
		oForm = document.forms[0];
		break;
	}
	if (!oForm) return null;
	return design_prevalidateElement(oForm, null);
}

function design_prevalidateElement(oElem, oFirstInvalidElem)
{
	if (!oElem) return oFirstInvalidElem;
	if ("undefined" == typeof oElem.getAttribute) return oFirstInvalidElem;
	if ("design_prototype" == oElem.className) return oFirstInvalidElem;
	
	if ("object" == typeof oElem.currentStyle && oElem.currentStyle != null) 
	{
		if ("none" == oElem.currentStyle.display) return oFirstInvalidElem;
		if ("hidden" == oElem.currentStyle.visibility) return oFirstInvalidElem;
	}
	var validation = oElem.getAttribute("ektdesignns_validation");
	if (validation && validation != "none")
	{
		oElem.removeAttribute("ektdesignns_isvalid");
		design_validate_result = true; // just in case onblur handler fails to set the result
		if ("function" == typeof oElem.onblur)
		{
			oElem.onblur(); // return value in global design_validate_result
		}
		else //if ("string" == typeof oElem.onblur) //for mac browsers
		{	 // or "object" - again, for mac (safari), see notes below.
			// Safari 2.0.4/Mac typeof oElem.onblur is "function"
			var sFn = oElem.getAttribute("onblur");
			if (sFn)
			{
				try
				{
					oElem.fnonblur = new Function(sFn);

					oElem.fnonblur();
				}
				catch (e)
				{
					// ********************************************************
					//    ATTENTION
					//      
					//		  Safari appears to invoke the new function we 
					//		create here (to handle the on-blur event (and 
					//		perform the validation) in the wrong context; 
					//      it runs in the calling windows context - which
					//      is a problem when we use things like the date
					//      picker popup, functions/etc that exist in the 
					//      main window are not available in the popup window
					//      contentext - so the code fails...
					//      
					//        If, instead, we evaluate the function then it
					//      runs in the context of this main window and behaves
					//      properly. We catch that failure and attempt to 
					//      handle it for Safari (or any other similarly 
					//      mis-behaving browser).
					//      
					//      Note that we cannot use the 'this' pointer
					//      in this case, we must replace it with the variable
					//      name that points to the element object - in this 
					//      case 'oElem' Also note that we asume that the 
					//      this pointer will only be passed once in the parameter
					//      list, that it will come before the comments, etc.,
					//      otherwise the following regular expression will need
					//      to be updated.
					//      
					// ********************************************************
					sFn = sFn.replace(/([\(\,]\s*)this(\s*[\,\)])/, '$1oElem$2');
					var fn = new Function(sFn);
					eval(sFn);
				}
			}
		}
		if (null == oFirstInvalidElem && false == design_validate_result) 
		{
			oFirstInvalidElem = oElem;
		}
	}

	if (typeof oElem.childNodes != "undefined") 
	{
		for (var i = 0; i < oElem.childNodes.length; i++)
		{
			if (ELEMENT_NODE == oElem.nodeType)
			{
				oFirstInvalidElem = design_prevalidateElement(oElem.childNodes.item(i), oFirstInvalidElem);
			}
		}
	}
	return oFirstInvalidElem;
}

var g_oElemContainerForAttributes = null;

function design_getProtectedAttribute(oElem, name)
{
	// Processes attributes that may have be protected by eWebEditPro+XML.
	var retValue; // initially undefined
	if (oElem)
	{
		var ektAttr = oElem.getAttribute("ctagattrs");
		// eg, " ektdesignns_minoccurs=@zzquote;0@zzquote;" or @zzsquo;
		if ("string" == typeof ektAttr)
		{
			if (null == g_oElemContainerForAttributes)
			{
				g_oElemContainerForAttributes = document.createElement("span");
			}
			var strAttrs = ektAttr.replace(/\@zzquote\;/g,'"');
			strAttrs = strAttrs.replace(/\@zzsquo\;/g,"'");
			strAttrs = strAttrs.replace(/\@zzamp\;/g,"&");
			strAttrs = strAttrs.replace(/\@zzlt\;/g,"<");
			strAttrs = strAttrs.replace(/\@zzgt\;/g,">");
			g_oElemContainerForAttributes.innerHTML = "<span " + strAttrs + "> </span>";
			retValue = g_oElemContainerForAttributes.firstChild.getAttribute(name);	
		}
	}
	return retValue;
}

function design_getAttribute(oElem, name)
{
	// Processes attributes that may have be protected by eWebEditPro+XML.
	var retValue; // initially undefined
	if (oElem)
	{
		switch (name)
		{
		case "selected":
			if (oElem.selected)
			{
				retValue = "selected";
			}
			break;
		case "checked":
			if (oElem.checked)
			{
				retValue = "checked";
			}
			break;
		default:
			retValue = oElem.getAttribute(name);
			if ("undefined" == typeof retValue || null == retValue)
			{
				retValue = design_getProtectedAttribute(oElem, name);
			}
		}
	}
	return retValue;
}

function design_getValue(oElem)
{
	if (!oElem) return;
	var bSupportInnerHTML = (typeof oElem.innerHTML != "undefined");
	if (typeof oElem.value != "undefined")
	{
		if ("INPUT" == oElem.tagName && ("checkbox" == oElem.type || "radio" == oElem.type))
		{
			var strValue = oElem.value + "";
			if (strValue.length > 0 && strValue != "true" && strValue != "on")
			{
				if (oElem.checked)
				{
					return strValue;
				}
				else
				{
					return "";
				}
			}
			else // boolean
			{
				if (oElem.checked)
				{
					return true;
				}
				else
				{
					return false;
				}
			}
		}
		else
		{
			return oElem.value + ""; // Note: This string conversion is needed for Safari, 
									 // as the regular expression fails to handle the value 
									 // properly without it until some value has been placed
									 // into the input field (value may then be removed and it
									 // still works!). This way it always works properly.
									 // (Thanks Doug D! -BCB)
		}
	}
	else if (typeof oElem.getAttribute != "undefined" && oElem.getAttribute("datavalue") != null)
	{
		// Needed for Opera b/c "value" is interpreted as a number and not a string, eg, "2007-01-09" is read as "2007"
		return oElem.getAttribute("datavalue");
	}
	else if (typeof oElem.getAttribute != "undefined" && oElem.getAttribute("value") != null)
	{
		// In FireFox/Mozilla/Netscape7, the .value attribute is undefined if not standard (e.g., span)
		// and .getAttribute("value") is null when .value is standard (e.g., input).
		return oElem.getAttribute("value");
	}
	else if (bSupportInnerHTML && "content-req" == design_getAttribute(oElem, "ektdesignns_validation"))
	{
		return oElem.innerHTML; // CAUTION: .innerHTML is not well-formed and cannot be processed as XML
	}
	else if (bSupportInnerHTML && "mixed" == design_getAttribute(oElem, "ektdesignns_datatype"))
	{
		return oElem.innerText; // .innerHTML needs to be converted to XHTML, that is, well-formed
	}
	else if (typeof oElem.innerText != "undefined")
	{
		return oElem.innerText;
	}
	else if (bSupportInnerHTML)
	{
		return oElem.innerHTML.replace(/\<[^>]*\>/g, "");
	}
	else
	{
		return; // no data to test
	}
}

function design_setValue(oElem, value)
{
	if (!oElem) return;
	if (typeof oElem.value != "undefined")
	{
		if ("INPUT" == oElem.tagName && ("checkbox" == oElem.type || "radio" == oElem.type))
		{
			if ("true" == value || true == value || "on" == value) // boolean
			{
				oElem.checked = true;
			}
			else if ("false" == value || false == value) // boolean
			{
				oElem.checked = false;
			}
			else
			{
				oElem.value = value;
			}
		}
		else
		{
			oElem.value = value;
		}
	}
	else if (typeof oElem.getAttribute != "undefined" && oElem.getAttribute("value") != null)
	{
		// In FireFox/Mozilla/Netscape7, the .value attribute is undefined if not standard (e.g., span)
		// and .getAttribute("value") is null when .value is standard (e.g., input).
		oElem.value = value;
	}
	else if (typeof oElem.innerHTML != "undefined" && "mixed" == design_getAttribute(oElem, "ektdesignns_datatype"))
	{
		oElem.innerHTML = value;
	}
	else if (typeof oElem.innerText != "undefined")
	{
		oElem.innerText = value;
	}
}

function design_evaluate(expression, value)
{
	var obj = new Object();
	obj.text = value + "";
	obj.fnDesignEvaluateExpression = new Function("return " + expression); 
	return obj.fnDesignEvaluateExpression();
}

function design_normalize_re(re, oElem)
{
	// only normalize for actual onblur event
	if (typeof g_design_prevalidateFormReentry == "undefined" || g_design_prevalidateFormReentry != true) 
	{
		var value = design_getValue(oElem);
		if ("undefined" == typeof value) return; // no data to test
		if ("undefined" != typeof RegExp.lastIndex) 
		{
			RegExp.lastIndex = 0;
		}
		re.lastIndex = 0;
		
		var ary = re.exec(value);
		
		value = (null == ary ? "" : ary[0]);
		design_normalize_complete(oElem, value);
	}
}

function design_validate_re(re, oElem, invalidmsg)
{
	var value = design_getValue(oElem);
	if ("undefined" == typeof value) return; // no data to test
	if ("undefined" != typeof RegExp.lastIndex) 
	{
		RegExp.lastIndex = 0;
	}
	re.lastIndex = 0;

	var result = re.test(value);

	design_validate_complete(oElem, result, invalidmsg);

	return result;
}

function design_normalize_js(expression, oElem)
{
	// only normalize for actual onblur event
	if (typeof g_design_prevalidateFormReentry == "undefined" || g_design_prevalidateFormReentry != true) 
	{
		var value = design_getValue(oElem);
		if ("undefined" == typeof value) return; // no data to test
		
		var value = design_evaluate(expression, value);
		
		design_normalize_complete(oElem, value);
	}
}

function design_validate_js(expression, oElem, invalidmsg)
// value is optional
// returns true if valid or indeterminate, false if value fails reg exp.
{
	var value = design_getValue(oElem);
	if ("undefined" == typeof value) return; // no data to test
	
	var result = design_evaluate(expression, value);
	
	design_validate_complete(oElem, result, invalidmsg);
	return result;
}

function design_normalize_complete(oElem, value)
{
	design_setValue(oElem, value);
}

var design_validate_result = true;
function design_validate_complete(oElem, result, invalidmsg)
{
	design_validate_result = result;
	if (!oElem) return result;
	// Netscape 4.7 does not support oElem.title and oElem.style.
	
	if (invalidmsg && "string" == typeof oElem.title)
	{
		// Remove message from title attribute if it was appended.
		var p = oElem.title.indexOf(invalidmsg);
		if (p >= 0)
		{
			if (p > 0 && "\n" == oElem.title.charAt(p-1))
			{
				p -= 1;
			}
			oElem.title = oElem.title.substring(0, p);
		}
		// remove trailing line breaks
		p = oElem.title.length - 1;
		if (p >= 0 && "\n" == oElem.title.charAt(p))
		{
			while (p >= 0 && "\n" == oElem.title.charAt(p))
			{
				p--;
			}
			oElem.title = oElem.title.substring(0, p);
		}
	}

	if (!result)
	{
		if (invalidmsg && ("string" == typeof oElem.title))
		{
			// Append message to title attribute unless it is already present.
			if (-1 == oElem.title.indexOf(invalidmsg))
			{
				if (oElem.title.length > 0)
				{
					oElem.title += " \n";
				}
				oElem.title += invalidmsg;	
			}
		}
	}

	// Check for the presence of a customer defined validation-styling function:
	if ("function" == typeof customValidationStyle)
	{
		// call the users custom validation-styling function:
		customValidationStyle(oElem, result);
	}
	else
	{
		// use our built-in validation-styling function:
		design_validationStyle(oElem, result);
	}
}

function design_validationStyle(oElem, isValid)
{
	var parent = null;
	var elTypeName = oElem.tagName;

	// If browser is Safari, or control type is Select, but not both
	// Safari And Select-control (because Safari Select controls do not
	// generate an onBlur event) then add wrapper for border/style control:
	var specialCaseBorder = (design_isSafari() && ("INPUT" == elTypeName)) 
							|| ("SELECT" == elTypeName);
	
	if ("object" == typeof oElem)
	{
		parent = oElem.parentNode;
		
		if (("object" == typeof oElem.style) && ("object" == typeof parent))
		{
			if (isValid)
			{
				if (specialCaseBorder)
				{
					if (("SPAN" == parent.tagName) 
						&& ("design_validation_failed" == parent.className))
					{
						parent.className = "design_validation_passed";
					}
				}
				else
				{
					// Safari needs the individual properties
					oElem.style.borderTopStyle = "";
					oElem.style.borderRightStyle = "";
					oElem.style.borderBottomStyle = "";
					oElem.style.borderLeftStyle = "";
					oElem.style.borderTopColor = "";
					oElem.style.borderRightColor = "";
					oElem.style.borderBottomColor = "";
					oElem.style.borderLeftColor = "";
					oElem.style.borderTopWidth = "";
					oElem.style.borderRightWidth = "";
					oElem.style.borderBottomWidth = "";
					oElem.style.borderLeftWidth = "";
						oElem.style.margin = "2px";
						// Do not simply remove style: it doesn't 
						// re-render in IE 5.5, may destabilize it...
					}
				}
			else
			{
				if (("undefined" == typeof g_design_designMode) || (g_design_designMode != true))
				{
					if (specialCaseBorder)
					{
						// Ensure that the element is wrapped in our own 
						// span tag, so we can control the border style:
						if ((parent.tagName != "SPAN") 
							|| ((parent.className != "design_validation_failed")
								&& (parent.className != "design_validation_passed")))
						{
							var wrapper = document.createElement("span");
							wrapper = parent.insertBefore(wrapper, oElem);
							oElem = parent.removeChild(oElem);
							oElem = wrapper.appendChild(oElem);
							parent = wrapper;
						}

						parent.className = "design_validation_failed";
					}
					else
					{
						oElem.style.borderStyle = "dashed";
						oElem.style.borderColor = "red";
						oElem.style.borderWidth = "2px";
						oElem.style.margin = "0";
					}
				}
			}
		}
	}
}

function design_validate_select(minIndex, oElem, invalidmsg) 
// minIndex = -1, 0, 1 etc.. (-1 = not selected; 0 = 1st on list etc)
// returns true if valid or indeterminate, false if index is 0 or -1.
{
	if (!oElem) return;
	if ("undefined" == typeof oElem.selectedIndex)
	{
		return; // not a select element
	}

	var result = (oElem.selectedIndex >= minIndex);
	// this has no visual effect on select tag, but it is needed to set the design_validate_result (global var).
	design_validate_complete(oElem, result, invalidmsg);
	return result;
}

function design_validate_choice(minSelected, maxSelected, oElem, invalidmsg) 
// returns true if valid or indeterminate, false otherwise.
// maxSelected = -1 if it has no limits.
{
	if (!oElem) return;
	if ("undefined" == typeof oElem.getElementsByTagName) return;
	var num_checked = 0;
	var oCurrElem;
	var bUseChecked;
	var aryElements = null;
	var validation = oElem.getAttribute("ektdesignns_validation");
	if ("choice-req" == validation)
	{
		aryElements = oElem.getElementsByTagName("input");
		bUseChecked = true;
	}
	else if ("select-req" == validation) //list box
	{
		aryElements = oElem.getElementsByTagName("option");
		bUseChecked = false;
	}
	if (aryElements)
	{
		for (var i = 0; i < aryElements.length; i++)
		{
			oCurrElem = aryElements[i];
			if (bUseChecked)
			{
				if (oCurrElem.checked)
				{
					num_checked++;
				}
			}
			else //list box
			{
				if (oCurrElem.selected)
				{
					num_checked++;
				}
			}
		}
	}
	var result = (minSelected <= num_checked && (maxSelected <= 0 || num_checked <= maxSelected));	
	design_validate_complete(oElem, result, invalidmsg);
	return result;
}

function design_normalize_isbn(value)
{
	value = value + "";
	value = value.replace(/[\s\-]/g, "").toUpperCase(); // remove spaces and hyphens
	return value;
}

function design_validate_isbn(value)
// returns true if valid or indeterminate, false otherwise.
{
	var result = design_validate_isbn10(value) || design_validate_isbn13(value);
	return result;
}

function design_validate_isbn10(value)
// returns true if valid or indeterminate, false otherwise.
{
	var result = true;
	value = value + "";
	var re = new RegExp("^[0-9]{9}[0-9X]$"); // or "^[0-9 \-]{9,12}[0-9X]$"
	if (!re.test(value)) return false;
	
	// adapted from http://www.merlyn.demon.co.uk/js-misc0.htm#ISBN
	var check = 0;
	var weight = 10;
	for (var i = 0; i < value.length; i++) 
	{
		var c = value.charCodeAt(i);
		if (88 == c && 1 == weight) // final X
		{ 
			check += 10; 
			weight--;
		} 
		else if (48 <= c && c <= 57) // 0-9 
		{
			check += (c - 48) * weight--;
		}		
	}
	result = (0 == weight && 0 == (check % 11));
	return result;
}

function design_validate_isbn13(value)
// returns true if valid or indeterminate, false otherwise.
{
	value = value + "";
	var re = new RegExp("^[0-9]{13}$"); // or "^[0-9 \-]{13,17}$"
	if (!re.test(value)) return false;
	
	// adapted from http://www.merlyn.demon.co.uk/js-misc0.htm#ISBN
	var check = 0;
	var n = 13;
	var weight = 1;
	for (var i = 0; i < value.length; i++) 
	{
		var c = value.charCodeAt(i);
		if (48 <= c && c <= 57) // 0-9 
		{
			check += (c - 48) * weight;
			weight = (1 == weight ? 3 : 1); // toggle b/n 1 and 3
			n--;
		}		
	}
	return (0 == n && 0 == (check % 10));
}

function design_normalize_issn(value)
{
	value = value + "";
	value = value.replace(/[\s\-]/g, "").toUpperCase(); // remove spaces and hyphens
	return value;
}

function design_validate_issn(value)
// returns true if valid or indeterminate, false otherwise.
{
	value = value + "";
	var re = new RegExp("^[0-9]{7}[0-9X]$"); // or "^[0-9]{4}\-?[0-9]{3}[0-9X]$"
	if (!re.test(value)) return false;
	
	// adapted from http://www.merlyn.demon.co.uk/js-misc0.htm#ISBN
	var check = 0;
	var weight = 8;
	for (var i = 0; i < value.length; i++) 
	{
		var c = value.charCodeAt(i);
		if (88 == c && 1 == weight) // final X
		{ 
			check += 10; 
			weight--;
		} 
		else if (48 <= c && c <= 57) // 0-9 
		{
			check += (c - 48) * weight--;
		}		
	}
	return (0 == weight && 0 == (check % 11));
}

function design_current_date()
{
	// Returns current date in format yyyy-mm-dd
	var oCurrentDate = new Date();
	var mm = (oCurrentDate.getMonth() + 1);
	if (mm <= 9) mm = "0" + mm;
	var dd = oCurrentDate.getDate();
	if (dd <= 9) dd = "0" + dd;
	return (oCurrentDate.getFullYear() + "-" + mm + "-" + dd);
}

//function design_default_current_date(date)
//{
//	date = date + "";
//	if (date.length != 10) 
//	{
//		date = design_current_date();
//		var oTempDate = new Date(date.substr(0,4), parseInt(date.substr(5,2),10)-1, date.substr(8,2));
//		var strDate = (oTempDate.toLocaleDateString ? oTempDate.toLocaleDateString() : oTempDate.toLocaleString());
//		var oDateElem = oElem.firstChild;
//		while (oDateElem && oDateElem.tagName != "INPUT")
//		{
//			oDateElem = oDateElem.nextSibling;
//		}
//		if (oDateElem != null) oDateElem.value = strDate;
//	}
//	return date;
//}

function design_validate_future_date(date)
// returns true if valid or indeterminate, false otherwise.
{
	date = date + "";
	if (10 == date.length) 
	{
		return (date >= design_current_date());
	}
	return false;
}

// private
function design_canElementReceiveFocus(oElem)
// Returns true if form element can receive the focus, false if not.
{
	if (!oElem) return false;
	var strType = oElem.type + "";
	if ("hidden" == strType) return false;
	if ("object" == typeof oElem.currentStyle) 
	{
		if ("none" == oElem.currentStyle.display) return false;
		// Unfortunately, currentStyle.visibility may return "inherit" (even for all parents), which is not helpful.
		if ("hidden" == oElem.currentStyle.visibility) return false;
	}
	var strDisabled = oElem.disabled + "";
	if ("true" == strDisabled) return false;
	if (oElem.isDisabled) return false;
	var strIsTextEdit =  oElem.isTextEdit + "";
	if ("false" == strIsTextEdit) return false;
	var strFocusMethod = typeof oElem.focus;
	if ("function" != strFocusMethod && "object" != strFocusMethod) return false;
	return true;
}


function design_HTMLEncode(s)
{
	var strHTML = s + "";
	strHTML = strHTML.replace(/\&/g, "&amp;");
	strHTML = strHTML.replace(/\</g, "&lt;");
	strHTML = strHTML.replace(/\>/g, "&gt;");
	strHTML = strHTML.replace(/\"/g, "&quot;");
	return strHTML;
}

function design_serializeHTMLAttribute(oElem, name)
{
	if (!oElem) return "";
	try
	{
		var attr = "";
		if ("class" == name)
		{
			attr = oElem.className;
		}
		else
		{
			attr = design_getAttribute(oElem, name);
		}
		if ("string" == typeof attr && attr.length > 0)
		{
			return " " + name + "=\"" + design_HTMLEncode(attr) + "\"";
		}
		else if ("boolean" == typeof attr && true == attr)
		{
			return " " + name + "=\"" + name + "\"";
		}
		else
		{
			return "";
		}
	}
	catch (e)
	{
		return "";
	}
}

function design_serializeHTMLElement(oElem, content)
{
	if (!oElem) return "";
	var tagName = oElem.tagName.toLowerCase();
	var sAttrs = "";
	var attrNames = ["ektdesignns_bind","ektdesignns_nodetype","ektdesignns_content","class","type","value","selected","checked"];
	for (var i = 0; i < attrNames.length; i++)
	{
		sAttrs += design_serializeHTMLAttribute(oElem, attrNames[i]);
	}
	if ("undefined" == typeof content)
	{
		// Recurse through children and serialize each.
		content = "";
		for (var i = 0; i < oElem.childNodes.length; i++)
		{
			var oChild = oElem.childNodes[i];
			switch (oChild.nodeType)
			{
				case ELEMENT_NODE:
					content += design_serializeHTMLElement(oChild);
					break;
				case TEXT_NODE:
					content += oChild.nodeValue;
					break;
//				case CDATA_SECTION_NODE:
//					content += ?;
//					break;
				default:
					// ignore. note, attributes are handled above
					break;
			}
		}
	}
	return design_serializeElement(tagName, content, sAttrs);
}

function design_serializeElement(tagName, content, attributes)
{
	if ("undefined" == typeof attributes) attributes = "";
	if ("undefined" == typeof content || ("string" == typeof content && 0 == content.length) || (null == content))
	{
		return "<" + tagName + attributes + " />\n";
	}
	else
	{
		return "<" + tagName + attributes + ">" + content + "</" + tagName + ">\n";
	}
}

/* XML functionality dependent on Sarissa */

function design_xml_loadXML(xml)
{
	try
	{
		if (typeof xml != "string") return null;
		if (xml.length <= 2) return null;
		var xmlDoc = Sarissa.getDomDocument();
		if ("string" == typeof xmlDoc || null == xmlDoc) return "Unable to create XML DOM Document";
		xmlDoc.async = false;
		if (xml.indexOf("<") >= 0)
		{
			var objParser = new DOMParser();
			xmlDoc = objParser.parseFromString(xml, "text/xml");
			if (Sarissa.getParseErrorText(xmlDoc) != Sarissa.PARSED_OK)
			{
				// add root tags in case that is the problem
				xml = "<root>" + xml + "</root>";
				xmlDoc = objParser.parseFromString(xml, "text/xml");
			}
		}
		else // URL
		{
			var url = xml;
			url = url.replace(/.*(\[|%5B)srcpath(\]|%5D)\/?/i, g_design_srcPath);
			url = url.replace(/.*(\[|%5B)eWebEditProPath(\]|%5D)\/?/i, g_design_srcPath);
			xmlDoc.load(url);
		}
		var strErrMsg = Sarissa.getParseErrorText(xmlDoc);
		if (strErrMsg != Sarissa.PARSED_OK)
		{
//alert(strErrMsg);
			return strErrMsg;
		}
		return xmlDoc;
	}
	catch (e)
	{
//alert(e.message);
		return e.message;
	}
}

function design_xml_loadXSLT(xslt)
{
	try
	{
		if (typeof xslt != "string") return null;
		if (xslt.length <= 2) return null;
		var xslDoc = Sarissa.getXsltDocument();
		if ("string" == typeof xslDoc || null == xslDoc) return "Unable to create XSLT DOM Document";
		xslDoc.async = false;
		if (xslt.indexOf("<") >= 0)
		{
			if (typeof xslDoc.loadXML != "undefined")
			{
				xslDoc.loadXML(xslt);
			}
			else
			{
				var objParser = new DOMParser();
				xslDoc = objParser.parseFromString(xslt, "text/xml");
			}
		}
		else // URL
		{
			var url = xslt;
			url = url.replace(/.*(\[|%5B)srcpath(\]|%5D)\/?/i, g_design_srcPath);
			url = url.replace(/.*(\[|%5B)eWebEditProPath(\]|%5D)\/?/i, g_design_srcPath);
			xslDoc.load(url);
		}
		var strErrMsg = Sarissa.getParseErrorText(xslDoc);
		if (strErrMsg != Sarissa.PARSED_OK)
		{
//alert(strErrMsg);
			return strErrMsg;
		}
		return xslDoc;
	}
	catch (e)
	{
//alert(e.message);
		return e.message;
	}
}

function design_transformToDocument(xml, xslt)
{
	try
	{
		var xmlDoc = design_xml_loadXML(xml);
		if ("string" == typeof xmlDoc) return xmlDoc;
		if (null == xmlDoc) return "Unable to load XML document";
		
		var xsltDoc = design_xml_loadXSLT(xslt);
		if ("string" == typeof xsltDoc) return xsltDoc;
		if (null == xsltDoc) return "Unable to load XSLT document";
		
		var processor = new XSLTProcessor();
		processor.importStylesheet(xsltDoc);
		var newDoc = processor.transformToDocument(xmlDoc);
		return newDoc;
	}
	catch (e)
	{
//alert(e.message);
		return e.message;
	}
}

function design_transform(xml, xslt)
{
	try
	{
		var xmlDoc = design_xml_loadXML(xml);
		if ("string" == typeof xmlDoc) return xmlDoc;
		if (null == xmlDoc) return "Unable to load XML document";
		
		var xsltDoc = design_xml_loadXSLT(xslt);
		if ("string" == typeof xsltDoc) return xsltDoc;
		if (null == xsltDoc) return "Unable to load XSLT document";
		
		var processor = new XSLTProcessor();
		processor.importStylesheet(xsltDoc);
		var ownerDoc = Sarissa.getDomDocument();
		var newDoc = processor.transformToFragment(xmlDoc, ownerDoc);
		
		if ("string" == typeof newDoc) return newDoc;
		var result = (new XMLSerializer()).serializeToString(newDoc);
		// Mozilla may wrap output with transformiix:result tags, so remove them
		result = result.replace(/<transformiix:result[^>]*>/,"").replace("</transformiix:result>","");
		// Mozilla ignores namespace-alias, so use reg exp to manually 'alias'.
		result = result.replace(/xslout:/g,"xsl:");
		result = result.replace(/<\?[^\?]*\?>/,""); // remove xml declaration, if it exists
		// Can't use js extension to call xpathLiteralString in the XSLT, so do it here.
		result = result.replace(/xpathLiteralString(.*?)gnirtSlaretiLhtapx/g, 
									function(s,p1) { return xpathLiteralString(p1); } );
		return result;

// Older code for IE		
//		var xmlDoc = design_xml_loadXML(xml);
//		if ("string" == typeof xmlDoc) return xmlDoc;
//		if (null == xmlDoc) return "Unable to load XML document";
//		
//		var xsltDoc = design_xml_loadXML(xslt);
//		if ("string" == typeof xsltDoc) return xsltDoc;
//		if (null == xsltDoc) return "Unable to load XSLT document";
//		
//		var result = xmlDoc.transformNode(xsltDoc) + "";
//		result = result.replace(/<\?[^\?]*\?>/,""); // remove xml declaration, if it exists
//		return result;
	}
	catch (e)
	{
//alert(e.message);
		return e.message;
	}
}

function xpathLiteralString(s)
{
	if (s.indexOf("'") >= 0)
	{
		return "concat('" + s.replace(/\'/g, "',&quot;'&quot;,'") + "')";
	}
	else
	{
		return "'" + s + "'";
	}
}

/* Dynamic Data Lists */

function design_replaceDataLists()
{
	if (!document || !document.body)
	{
		setTimeout('design_replaceDataLists()', 200); // too soon, try again later
		return;
	}
	var aryDatalistCache = new Array();
	var aryTags = new Array();
	aryTags[0] = document.body.getElementsByTagName("select");
	// #38479 - Language choice field selection is not displayed in the postback message
	//aryTags[1] = document.body.getElementsByTagName("ektdesignns_choices");
	//aryTags[2] = document.body.getElementsByTagName("ektdesignns_checklist");
	for (var iTag = 0; iTag < aryTags.length; iTag++)
	{
		var aryElems = aryTags[iTag];
		for (var iElem = 0; iElem < aryElems.length; iElem++)
		{
			var oElem = aryElems[iElem];
			var datasrc = design_getAttribute(oElem, "ektdesignns_datasrc");
			if ("string" == typeof datasrc && datasrc.length > 0)
			{
				var datalist = design_getAttribute(oElem, "ektdesignns_datalist");
				if ("string" == typeof datalist && datalist.length > 0)
				{
					if ("undefined" == typeof aryDatalistCache[datalist])
					{
						var strSelect = design_getAttribute(oElem, "ektdesignns_dataselect");
						var strCaptionXPath = design_getAttribute(oElem, "ektdesignns_captionxpath");
						var strValueXPath = design_getAttribute(oElem, "ektdesignns_valuexpath");
						var strNamespaces = design_getAttribute(oElem, "ektdesignns_datanamespaces");
						aryDatalistCache[datalist] = design_getDataList(oElem.tagName, datasrc, strSelect, strCaptionXPath, strValueXPath, strNamespaces);
						// Datalist will be empty if running in editor b/c access is denied.
					}
					if (aryDatalistCache[datalist].length > 0)
					{
						if ("SELECT" == oElem.tagName)
						{
							var nNumOrigItemsToKeep = 0;
							var validation = design_getAttribute(oElem, "ektdesignns_validation");
							if ("select-req" == validation)
							{
								nNumOrigItemsToKeep = 1;
							}
							var strOrigDataList = "";
							for (var iOption = 0; iOption < oElem.options.length; iOption++)
							{
								var oOption = oElem.options[iOption];
								strOrigDataList += design_serializeHTMLElement(oOption, design_HTMLEncode(oOption.text));
							}
							var xmlDoc = design_transformDataList(strOrigDataList, aryDatalistCache[datalist], nNumOrigItemsToKeep);
							if (typeof xmlDoc != "string")
							{
								var newOptions = xmlDoc.getElementsByTagName("option");
								var numOptions = (newOptions != null ? newOptions.length : 0);
								if (oElem.options.length > numOptions)
								{
									oElem.options.length = numOptions;
								}
								if (oElem.multiple && oElem.size < 2 && numOptions > 12)
								{
									oElem.size = 12;
								}
								for (var iOption = 0; iOption < numOptions; iOption++)
								{
									var newOption = newOptions[iOption];
									var attrs = newOption.attributes;
									var attr;
									var text = (newOption.firstChild ? newOption.firstChild.nodeValue : "");
									attr = attrs.getNamedItem("value");
									var value = (attr ? attr.nodeValue : "");
									attr = attrs.getNamedItem("selected");
									var bSelected = ("selected" == (attr ? attr.nodeValue : ""));
									oElem.options[iOption] = new Option(text, value, bSelected, bSelected);
								}
								// oElem.innerHTML NOTE: setting innerHTML for select element is bug-ridden in all browsers
							}
							else
							{
								alert(xmlDoc); // transformation error
							}
						}
						else // choices and checklist
						{
							var oOrigListElem = oElem.nextSibling; // oElem is custom tag and does not contain its lexical children
							while (oOrigListElem.tagName != "OL") oOrigListElem = oOrigListElem.nextSibling;
							var strOrigDataList = design_serializeHTMLElement(oOrigListElem);
							// Can't mix DHTML DOM and XML DOM nodes, so transform returns string of HTML.
							var strHtml = design_transformChoiceDataList(strOrigDataList, aryDatalistCache[datalist]);
							strHtml = strHtml.replace(/<ol[^>]*>/,"").replace("</ol>",""); // remove OL tag, it's same as orig
							oOrigListElem.innerHTML = strHtml; // replace LI elements
						}
					}
				} // if datalist
			} // if datasrc
		} // for
	} // for
}
setTimeout('design_replaceDataLists()',1); // run this automatically after page is loaded
    
function design_transformChoiceDataList(strOrigDataList, strNewDataList)
{
    // Sarissa bug The document('') function is the web page in IE6 and the xml source document in Mozilla
    var strXSLT = "";
    strXSLT = "";
    strXSLT += "<?xml version='1.0'?>\n";
    strXSLT += "<xsl:stylesheet version=\"1.0\" xmlns:xsl=\"http://www.w3.org/1999/XSL/Transform\" xmlns:xslout=\"alias\">\n";
    strXSLT += "<xsl:namespace-alias stylesheet-prefix=\"xslout\" result-prefix=\"xsl\"/>\n";
    strXSLT += "<xsl:template match=\"ol\">\n";
    strXSLT += "<xslout:variable name=\"nameID\" select=\"'{li/input/@name}'\"/>\n";
    strXSLT += "<xslout:variable name=\"inputType\" select=\"'{li/input/@type}'\"/>\n";
    strXSLT += "</xsl:template>\n";
    strXSLT += "<xsl:template match=\"text()\"/>\n";
    strXSLT += "</xsl:stylesheet>\n";
	var strVariables = design_transform(strOrigDataList, strXSLT);

    strXSLT = "";
    strXSLT += "<?xml version='1.0'?>\n";
    strXSLT += "<xsl:stylesheet version=\"1.0\" xmlns:xsl=\"http://www.w3.org/1999/XSL/Transform\" xmlns:xslout=\"alias\">\n";
    strXSLT += "<xsl:namespace-alias stylesheet-prefix=\"xslout\" result-prefix=\"xsl\"/>\n";
    strXSLT += "<xsl:template match=\"/\">\n";
    strXSLT += "	<xsl:apply-templates/>\n";
    strXSLT += "</xsl:template>\n";
    strXSLT += "<xsl:template match=\"ol/li/input[@checked]\">\n";
    strXSLT += "	<xslout:if test=\"not(option[@value=xpathLiteralString{@value}gnirtSlaretiLhtapx])\">\n"; 
    strXSLT += "    	<xsl:copy-of select=\"..\"/>\n"; // copy LI element
    strXSLT += "	</xslout:if>\n";
    strXSLT += "</xsl:template>\n";
    strXSLT += "<xsl:template match=\"text()\"/>\n";
    strXSLT += "</xsl:stylesheet>\n";
	var strOldSelectedSnip = design_transform(strOrigDataList, strXSLT);

    strXSLT = "";
    strXSLT += "<?xml version='1.0'?>\n";
    strXSLT += "<xsl:stylesheet version=\"1.0\" xmlns:xsl=\"http://www.w3.org/1999/XSL/Transform\" xmlns:xslout=\"alias\">\n";
    strXSLT += "<xsl:namespace-alias stylesheet-prefix=\"xslout\" result-prefix=\"xsl\"/>\n";
    strXSLT += "<xsl:template match=\"/\">\n";
    strXSLT += "	<xsl:for-each select=\"ol/li/input[1]/@*[starts-with(name(),'ektdesignns_')]\">\n";
    strXSLT += "		<xslout:attribute name=\"{name()}\"><xsl:value-of select=\".\"/></xslout:attribute>\n";
    strXSLT += "	</xsl:for-each>\n";
    strXSLT += "	<xsl:apply-templates/>\n";
    strXSLT += "</xsl:template>\n";
    strXSLT += "<xsl:template match=\"ol/li/input[@checked]\">\n";
    strXSLT += "	<xslout:if test=\"@value=xpathLiteralString{@value}gnirtSlaretiLhtapx\">\n"; 
    strXSLT += "           <xslout:attribute name=\"checked\">checked</xslout:attribute>\n";
    strXSLT += "	</xslout:if>\n";
    strXSLT += "</xsl:template>\n";
    strXSLT += "<xsl:template match=\"text()\"/>\n";
    strXSLT += "</xsl:stylesheet>\n";
	var strNewSelectedSnip = design_transform(strOrigDataList, strXSLT);

	strXSLT = "";
    strXSLT += "<?xml version='1.0'?>\n";
    strXSLT += "<xsl:stylesheet version=\"1.0\" xmlns:xsl=\"http://www.w3.org/1999/XSL/Transform\" exclude-result-prefixes=\"ektdesign\" xmlns:ektdesign=\"urn:ektdesign\">\n";
    strXSLT += "<xsl:output method=\"xml\" version=\"1.0\" indent=\"yes\" omit-xml-declaration=\"yes\"/>\n";
////    strXSLT += "<ektdesign:datalist>\n";
////    strXSLT += strOrigDataList; // original ol/li list
////    strXSLT += "</ektdesign:datalist>\n";
    // Sarissa bug The document('') function is the web page in IE6 and the xml source document in Mozilla
////    strXSLT += "<xsl:variable name=\"oldDataList\" select=\"document('')/xsl:stylesheet/ektdesign:datalist\"/>\n";
////    strXSLT += "<xsl:variable name=\"nameID\" select=\"$oldDataList/ol/li/input/@name\"/>\n";
////    strXSLT += "<xsl:variable name=\"inputType\" select=\"$oldDataList/ol/li/input/@type\"/>\n";
////    strXSLT += "<xsl:variable name=\"ektAttr\" select=\"$oldDataList/ol/li/input/@*[starts-with(name(),'ektdesignns_')]\"/>\n";
    strXSLT += strVariables;
    // copy original OL tag
    strXSLT += "<xsl:template match=\"/\">\n";
////    strXSLT += "    <ol>\n";
////    strXSLT += "        <xsl:copy-of select=\"$oldDataList/ol/@*\"/>\n";
	var strOlTag = strOrigDataList.match(/<ol[^>]*>/);
	strXSLT += "	" + strOlTag + "\n";
    strXSLT += "        <xsl:apply-templates/>\n";
    strXSLT += "    </ol>\n";
    strXSLT += "</xsl:template>\n";
    // copy checked values that are not in the new data list
    strXSLT += "<xsl:template match=\"select\">\n";
////    strXSLT += "    <xsl:copy-of select=\"$oldDataList/ol/li/input[@checked and not(@value=current()/option/@value)]\"/>\n";
    strXSLT += strOldSelectedSnip;
    strXSLT += "    <xsl:apply-templates select=\"node()\"/>\n";
    strXSLT += "</xsl:template>\n";
    // process option tags
    strXSLT += "<xsl:template match=\"option\">\n";
    strXSLT += "    <xsl:variable name=\"modelID\" select=\"generate-id()\"/>\n";
    strXSLT += "    <xsl:variable name=\"displayOption\" select=\"text()\"/>\n";
    strXSLT += "    <xsl:variable name=\"valueOption\" select=\"@value\"/>\n";
    strXSLT += "     <li>\n";
    // copy attributes except 'checked'
    strXSLT += "    <input type=\"{$inputType}\" id=\"{$modelID}\" title=\"{$displayOption}\" value=\"{$valueOption}\" name=\"{$nameID}\">\n";
////    strXSLT += "    <xsl:copy-of select=\"$ektAttr\"/>\n";
    // check if checked in the old data list
////    strXSLT += "    <xsl:if test=\"$oldDataList/ol/li/input[@value=current()/@value]/@checked\">\n";
////    strXSLT += "        <xsl:attribute name=\"checked\">checked</xsl:attribute>\n";
////    strXSLT += "    </xsl:if>\n";
    strXSLT += strNewSelectedSnip;
    strXSLT += "    </input>\n";
    strXSLT += "    <label for=\"{$modelID}\"><xsl:value-of select=\"$displayOption\"/></label>\n";
    strXSLT += "    </li>\n";
    strXSLT += "</xsl:template>\n";
    // copy the text
    strXSLT += "<xsl:template match=\"*\">\n";
    strXSLT += "   <xsl:copy>\n";
    strXSLT += "       <xsl:copy-of select=\"@*\"/>\n";
    strXSLT += "       <xsl:apply-templates select=\"node()\"/>\n";
    strXSLT += "   </xsl:copy>\n";
    strXSLT += "</xsl:template>\n";
    
    strXSLT += "</xsl:stylesheet>\n";

    // Transform
    var strHtml = design_transform(strNewDataList, strXSLT);
    return strHtml;
}

function design_transformDataList(strOrigDataList, strNewDataList)
{
    // Sarissa bug The document('') function is the web page in IE6 and the xml source document in Mozilla
    strOrigDataList = "<select>" + strOrigDataList + "</select>";
    var strOldPredicate = "@selected";
    if ("number" == typeof nNumOrigItemsToKeep)
    {
		strOldPredicate += " or position()&lt;=" + nNumOrigItemsToKeep;
    }
    var strXSLT = "";
    strXSLT = "";
    strXSLT += "<?xml version='1.0'?>\n";
    strXSLT += "<xsl:stylesheet version=\"1.0\" xmlns:xsl=\"http://www.w3.org/1999/XSL/Transform\" xmlns:xslout=\"alias\">\n";
    strXSLT += "<xsl:namespace-alias stylesheet-prefix=\"xslout\" result-prefix=\"xsl\"/>\n";
    strXSLT += "<xsl:template match=\"/\">\n";
    strXSLT += "	<xsl:apply-templates/>\n";
    strXSLT += "</xsl:template>\n";
    strXSLT += "<xsl:template match=\"option[" + strOldPredicate + "]\">\n";
    strXSLT += "	<xslout:if test=\"not(option[@value=xpathLiteralString{@value}gnirtSlaretiLhtapx])\">\n"; 
    strXSLT += "    	<xsl:copy-of select=\".\"/>\n";
    strXSLT += "	</xslout:if>\n";
    strXSLT += "</xsl:template>\n";
    strXSLT += "<xsl:template match=\"text()\"/>\n";
    strXSLT += "</xsl:stylesheet>\n";
	var strOldSelectedSnip = design_transform(strOrigDataList, strXSLT);

    strXSLT = "";
    strXSLT += "<?xml version='1.0'?>\n";
    strXSLT += "<xsl:stylesheet version=\"1.0\" xmlns:xsl=\"http://www.w3.org/1999/XSL/Transform\" xmlns:xslout=\"alias\">\n";
    strXSLT += "<xsl:namespace-alias stylesheet-prefix=\"xslout\" result-prefix=\"xsl\"/>\n";
    strXSLT += "<xsl:template match=\"/\">\n";
    strXSLT += "	<xsl:apply-templates/>\n";
    strXSLT += "</xsl:template>\n";
    strXSLT += "<xsl:template match=\"option[@selected]\">\n";
    strXSLT += "	<xslout:if test=\"@value=xpathLiteralString{@value}gnirtSlaretiLhtapx\">\n"; 
    strXSLT += "           <xslout:attribute name=\"selected\">selected</xslout:attribute>\n";
    strXSLT += "	</xslout:if>\n";
    strXSLT += "</xsl:template>\n";
    strXSLT += "<xsl:template match=\"text()\"/>\n";
    strXSLT += "</xsl:stylesheet>\n";
	var strNewSelectedSnip = design_transform(strOrigDataList, strXSLT);
	
    strXSLT = "";
    strXSLT += "<?xml version='1.0'?>\n";
    strXSLT += "<xsl:stylesheet version=\"1.0\" xmlns:xsl=\"http://www.w3.org/1999/XSL/Transform\" exclude-result-prefixes=\"ektdesign\" xmlns:ektdesign=\"urn:ektdesign\">\n";
    strXSLT += "<xsl:output method=\"xml\" version=\"1.0\" indent=\"yes\" omit-xml-declaration=\"yes\"/>\n";
////    strXSLT += "<ektdesign:datalist>\n";
////    strXSLT += strOrigDataList; // original OPTION list
////    strXSLT += "</ektdesign:datalist>\n";
    // Sarissa bug The document('') function is the web page in IE6 and the xml source document in Mozilla
////    strXSLT += "<xsl:variable name=\"oldDataList\" select=\"document('')/xsl:stylesheet/ektdesign:datalist\"/>\n";
    strXSLT += "<xsl:template match=\"select\">\n";
    strXSLT += "   <select>\n"; // Sarissa requires a root in the output
    strXSLT += "   <!-- copy selected values that are not in the new data list -->\n";
////    strXSLT += "   <xsl:copy-of select=\"$oldDataList/option[@selected and not(@value=current()/option/@value)]\"/>\n";
    strXSLT += strOldSelectedSnip;
    
    strXSLT += "   <!-- process option tags -->\n";
    strXSLT += "   <xsl:apply-templates select=\"node()\"/>\n";
    strXSLT += "   </select>\n";
    strXSLT += "</xsl:template>\n";
    strXSLT += "<xsl:template match=\"option\">\n";
    strXSLT += "   <xsl:copy>\n";
    strXSLT += "       <!-- copy attributes except 'selected' -->\n";
    strXSLT += "       <xsl:copy-of select=\"@*[name() != 'selected']\"/>\n";
    strXSLT += "       <!-- check if selected in the old data list -->\n";
////    strXSLT += "       <xsl:if test=\"$oldDataList/option[@value=current()/@value]/@selected\">\n";
////    strXSLT += "           <xsl:attribute name=\"selected\">selected</xsl:attribute>\n";
////    strXSLT += "       </xsl:if>\n";
    strXSLT += strNewSelectedSnip;
    
    strXSLT += "       <!-- copy the text -->\n";
    strXSLT += "       <xsl:copy-of select=\"node()\"/>\n";
    strXSLT += "       </xsl:copy>\n";
    strXSLT += "</xsl:template>\n";
    strXSLT += "<xsl:template match=\"*\"> \n";
    strXSLT += "   <xsl:copy>\n";
    strXSLT += "       <xsl:copy-of select=\"@*\"/>\n";
    strXSLT += "       <xsl:apply-templates select=\"node()\"/>\n";
    strXSLT += "   </xsl:copy>\n";
    strXSLT += "</xsl:template>\n";
    strXSLT += "</xsl:stylesheet>";
    
    // Transform
    var xmlDoc = design_transformToDocument(strNewDataList, strXSLT);
    return xmlDoc;
}

function design_getDataList(strDDFieldTagName, strSource, strSelect, strCaptionXPath, strValueXPath, strNamespaces)
{
// Fetches data list from strSource
    // assert("SELECT" == strDDFieldTagName || "DIV" == strDDFieldTagName)
    
    var strPrefixes = "";
    if ("undefined" == typeof strNamespaces || null == strNamespaces)
    {
		strNamespaces = "";
    }
    else
    {
		strPrefixes = design_extractPrefixesFromNamespaces(strNamespaces);
		if (strPrefixes.length > 0) 
		{
			strPrefixes = " exclude-result-prefixes=\"" + strPrefixes + "\"";
		}
    }
    
    var strXSLT = "";
    strXSLT += "<?xml version=\"1.0\"?>\n";
    strXSLT += "<xsl:stylesheet version=\"1.0\" " + strPrefixes + " xmlns:xsl=\"http://www.w3.org/1999/XSL/Transform\" " + strNamespaces + ">\n";
    strXSLT += "<xsl:output method=\"xml\" version=\"1.0\" omit-xml-declaration=\"yes\" indent=\"yes\"/>\n";
    strXSLT += "<xsl:template match=\"/\">\n";
    strXSLT += "  <select>\n"; // Sarissa requires a root in the output
    strXSLT += "  <xsl:for-each select=\"" + strSelect + "\">\n";
    strXSLT += "    <option>\n";
    strXSLT += "      <xsl:if test=\"" + strValueXPath + "\">\n";
    strXSLT += "        <xsl:attribute name=\"value\">\n";
    strXSLT += "          <xsl:value-of select=\"" + strValueXPath + "\"/>\n";
    strXSLT += "        </xsl:attribute>\n";
    strXSLT += "      </xsl:if>\n";
    strXSLT += "      <xsl:value-of select=\"" + strCaptionXPath + "\"/>\n";
    strXSLT += "    </option>\n";
    strXSLT += "  </xsl:for-each>\n";
    strXSLT += "  </select>\n";
    strXSLT += "</xsl:template>\n";
    strXSLT += "</xsl:stylesheet>";
    
    // Transform
    var strHtml = design_transform(strSource, strXSLT);
    strHtml = strHtml.replace(/^\s+/,"").replace(/\s+$/,""); // trim
    
    if (strHtml.indexOf("<option") >= 0 || 0 == strHtml.length) 
    {
        return strHtml;
    } 
    else 
    {
//alert(strHtml);
        return ""; // transform error
    }
}

function design_extractPrefixesFromNamespaces(strNamespaces)
// strNamespaces is a delimited list of namespace declarations
{
	var aryNSPrefix = new Array();
	var aryAllNS = strNamespaces.match(/xmlns:\w+=['"][^'"]*['"]/g);
	if (null == aryAllNS) return "";
	aryAllNS.sort();
	var prev = "";
	for (var i = 0; i < aryAllNS.length; i++)
	{
		if (aryAllNS[i] != prev) // exclude duplicates
		{
			var aryNS = aryAllNS[i].match(/xmlns:(\w+)=['"]([^'"]*)['"]/);
			aryNSPrefix[aryNSPrefix.length] = aryNS[1];
			// URI = aryNS[2];
			prev = aryAllNS[i];
		}
	}
	return aryNSPrefix.join(" ");
}

/* Sarissa */

/**
 * ====================================================================
 * About
 * ====================================================================
 * Sarissa is an ECMAScript library acting as a cross-browser wrapper for native XML APIs.
 * The library supports Gecko based browsers like Mozilla and Firefox,
 * Internet Explorer (5.5+ with MSXML3.0+), Konqueror, Safari and a little of Opera
 * @version 0.9.7.8
 * @author: Manos Batsis, mailto: mbatsis at users full stop sourceforge full stop net
 * ====================================================================
 * Licence
 * ====================================================================
 * Sarissa is free software distributed under the GNU GPL version 2 (see <a href="gpl.txt">gpl.txt</a>) or higher, 
 * GNU LGPL version 2.1 (see <a href="lgpl.txt">lgpl.txt</a>) or higher and Apache Software License 2.0 or higher 
 * (see <a href="asl.txt">asl.txt</a>). This means you can choose one of the three and use that if you like. If 
 * you make modifications under the ASL, i would appreciate it if you submitted those.
 * In case your copy of Sarissa does not include the license texts, you may find
 * them online in various formats at <a href="http://www.gnu.org">http://www.gnu.org</a> and 
 * <a href="http://www.apache.org">http://www.apache.org</a>.
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY 
 * KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE 
 * WARRANTIES OF MERCHANTABILITY,FITNESS FOR A PARTICULAR PURPOSE 
 * AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR 
 * COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
 * LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR 
 * OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE 
 * SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
 */
/**
 * <p>Sarissa is a utility class. Provides "static" methods for DOMDocument, 
 * DOM Node serialization to XML strings and other utility goodies.</p>
 * @constructor
 */
function Sarissa(){};
Sarissa.VERSION = "0.9.7.8";
Sarissa.PARSED_OK = "Document contains no parsing errors";
Sarissa.PARSED_EMPTY = "Document is empty";
Sarissa.PARSED_UNKNOWN_ERROR = "Not well-formed or other error";
Sarissa.IS_ENABLED_TRANSFORM_NODE = false;
var _sarissa_iNsCounter = 0;
var _SARISSA_IEPREFIX4XSLPARAM = "";
var _SARISSA_HAS_DOM_IMPLEMENTATION = document.implementation && true;
var _SARISSA_HAS_DOM_CREATE_DOCUMENT = _SARISSA_HAS_DOM_IMPLEMENTATION && document.implementation.createDocument;
var _SARISSA_HAS_DOM_FEATURE = _SARISSA_HAS_DOM_IMPLEMENTATION && document.implementation.hasFeature;
var _SARISSA_IS_MOZ = _SARISSA_HAS_DOM_CREATE_DOCUMENT && _SARISSA_HAS_DOM_FEATURE;
var _SARISSA_IS_SAFARI = (navigator.userAgent && navigator.vendor && (navigator.userAgent.toLowerCase().indexOf("applewebkit") != -1 || navigator.vendor.indexOf("Apple") != -1));
var _SARISSA_IS_IE = document.all && window.ActiveXObject && navigator.userAgent.toLowerCase().indexOf("msie") > -1  && navigator.userAgent.toLowerCase().indexOf("opera") == -1;
if(!window.Node || !Node.ELEMENT_NODE){
    Node = {ELEMENT_NODE: 1, ATTRIBUTE_NODE: 2, TEXT_NODE: 3, CDATA_SECTION_NODE: 4, ENTITY_REFERENCE_NODE: 5,  ENTITY_NODE: 6, PROCESSING_INSTRUCTION_NODE: 7, COMMENT_NODE: 8, DOCUMENT_NODE: 9, DOCUMENT_TYPE_NODE: 10, DOCUMENT_FRAGMENT_NODE: 11, NOTATION_NODE: 12};
};

if(typeof XMLDocument == "undefined" && typeof Document !="undefined"){ XMLDocument = Document; } 

// IE initialization
if(_SARISSA_IS_IE){
    // for XSLT parameter names, prefix needed by IE
    _SARISSA_IEPREFIX4XSLPARAM = "xsl:";
    // used to store the most recent ProgID available out of the above
    var _SARISSA_DOM_PROGID = "";
    var _SARISSA_XMLHTTP_PROGID = "";
    var _SARISSA_DOM_XMLWRITER = "";
    /**
     * Called when the Sarissa_xx.js file is parsed, to pick most recent
     * ProgIDs for IE, then gets destroyed.
     * @private
     * @param idList an array of MSXML PROGIDs from which the most recent will be picked for a given object
     * @param enabledList an array of arrays where each array has two items; the index of the PROGID for which a certain feature is enabled
     */
    Sarissa.pickRecentProgID = function (idList){
        // found progID flag
        var bFound = false;
        for(var i=0; i < idList.length && !bFound; i++){
            try{
                var oDoc = new ActiveXObject(idList[i]);
                o2Store = idList[i];
                bFound = true;
            }catch (objException){
                // trap; try next progID
            };
        };
        if (!bFound) {
			alert("Failed to create XML parser: " + idList[0]);
            //throw "Could not retreive a valid progID of Class: " + idList[idList.length-1]+". (original exception: "+e+")";
        };
        idList = null;
        return o2Store;
    };
    // pick best available MSXML progIDs
    _SARISSA_DOM_PROGID = null;
    _SARISSA_THREADEDDOM_PROGID = null;
    _SARISSA_XSLTEMPLATE_PROGID = null;
    _SARISSA_XMLHTTP_PROGID = null;
    if(!window.XMLHttpRequest){
        /**
         * Emulate XMLHttpRequest
         * @constructor
         */
        XMLHttpRequest = function() {
            if(!_SARISSA_XMLHTTP_PROGID){
                _SARISSA_XMLHTTP_PROGID = Sarissa.pickRecentProgID(["MSXML2.XMLHTTP.6.0", "MSXML2.XMLHTTP.4.0", "MSXML2.XMLHTTP.5.0", "MSXML2.XMLHTTP.3.0"]);
            };
            return new ActiveXObject(_SARISSA_XMLHTTP_PROGID);
        };
    };
    // we dont need this anymore
    //============================================
    // Factory methods (IE)
    //============================================
    // see non-IE version
    Sarissa.getDomDocument = function(sUri, sName){
        if(!_SARISSA_DOM_PROGID){
            _SARISSA_DOM_PROGID = Sarissa.pickRecentProgID(["MSXML2.DOMDocument.6.0", "MSXML2.DOMDocument.4.0", "MSXML2.DOMDocument.5.0", "MSXML2.DOMDocument.3.0"]);
        };
        var oDoc = new ActiveXObject(_SARISSA_DOM_PROGID);
        oDoc.resolveExternals = true; // for MSXML 6.0
        // if a root tag name was provided, we need to load it in the DOM object
        if (sName){
            // create an artificial namespace prefix 
            // or reuse existing prefix if applicable
            var prefix = "";
            if(sUri){
                if(sName.indexOf(":") > 1){
                    prefix = sName.substring(0, sName.indexOf(":"));
                    sName = sName.substring(sName.indexOf(":")+1); 
                }else{
                    prefix = "a" + (_sarissa_iNsCounter++);
                };
            };
            // use namespaces if a namespace URI exists
            if(sUri){
                oDoc.loadXML('<' + prefix+':'+sName + " xmlns:" + prefix + "=\"" + sUri + "\"" + " />");
            } else {
                oDoc.loadXML('<' + sName + " />");
            };
        };
        return oDoc;
    };
    Sarissa.getXsltDocument = function(sUri, sName){
        if(!_SARISSA_THREADEDDOM_PROGID){
            _SARISSA_THREADEDDOM_PROGID = Sarissa.pickRecentProgID(["MSXML2.FreeThreadedDOMDocument.6.0", "MSXML2.FreeThreadedDOMDocument.4.0", "MSXML2.FreeThreadedDOMDocument.5.0", "MSXML2.FreeThreadedDOMDocument.3.0"]);
        };
        var oDoc = new ActiveXObject(_SARISSA_THREADEDDOM_PROGID);
        Sarissa.setXpathNamespaces(oDoc, "xmlns:xsl='http://www.w3.org/1999/XSL/Transform'");
        oDoc.resolveExternals = true; // MSXML 2.0 and later
        if ("MSXML2.FreeThreadedDOMDocument.6.0" == _SARISSA_THREADEDDOM_PROGID) { 
            oDoc.setProperty("AllowDocumentFunction", true); // This property is supported in MSXML 3.0 SP4, 4.0 SP2, 5.0, and 6.0. The default value is true for 3.0, 4.0, and 5.0. The default value is false for 6.0.
            oDoc.setProperty("AllowXsltScript", true); // This property is supported in MSXML 3.0 SP8, 5.0 SP2, and 6.0. The default value is true for 3.0 and 5.0. The default value is false for 6.0.
            oDoc.setProperty("ProhibitDTD", false); // This property is supported in MSXML 3.0 SP5, 4.0 SP3, 5.0 SP2, and 6.0. The default value is false for 3.0, 4.0, and 5.0. The default value is true for 6.0.
        }
        // if a root tag name was provided, we need to load it in the DOM object
        if (sName){
            // create an artifical namespace prefix 
            // or reuse existing prefix if applicable
            var prefix = "";
            if(sUri){
                if(sName.indexOf(":") > 1){
                    prefix = sName.substring(0, sName.indexOf(":"));
                    sName = sName.substring(sName.indexOf(":")+1); 
                }else{
                    prefix = "a" + (_sarissa_iNsCounter++);
                };
            };
            // use namespaces if a namespace URI exists
            if(sUri){
                oDoc.loadXML('<' + prefix+':'+sName + " xmlns:" + prefix + "=\"" + sUri + "\"" + " />");
            } else {
                oDoc.loadXML('<' + sName + " />");
            };
        };
        return oDoc;
    };
    // see non-IE version   
    Sarissa.getParseErrorText = function (oDoc) {
        var parseErrorText = Sarissa.PARSED_OK;
        if(oDoc && oDoc.parseError && oDoc.parseError.errorCode && oDoc.parseError.errorCode != 0){
            parseErrorText = "XML Parsing Error: " + oDoc.parseError.reason + 
                "\nLocation: " + oDoc.parseError.url + 
                "\nLine Number " + oDoc.parseError.line + ", Column " + 
                oDoc.parseError.linepos + 
                ":\n" + oDoc.parseError.srcText +
                "\n";
            for(var i = 0;  i < oDoc.parseError.linepos;i++){
                parseErrorText += "-";
            };
            parseErrorText +=  "^\n";
        }
        else if(oDoc.documentElement == null){
            parseErrorText = Sarissa.PARSED_EMPTY;
        };
        return parseErrorText;
    };
    // see non-IE version
    Sarissa.setXpathNamespaces = function(oDoc, sNsSet) {
        oDoc.setProperty("SelectionLanguage", "XPath");
        oDoc.setProperty("SelectionNamespaces", sNsSet);
    };   
    /**
     * Basic implementation of Mozilla's XSLTProcessor for IE. 
     * Reuses the same XSLT stylesheet for multiple transforms
     * @constructor
     */
    XSLTProcessor = function(){
        if(!_SARISSA_XSLTEMPLATE_PROGID){
            _SARISSA_XSLTEMPLATE_PROGID = Sarissa.pickRecentProgID(["MSXML2.XSLTemplate.6.0", "MSXML2.XSLTemplate.4.0", "MSXML2.XSLTemplate.5.0", "MSXML2.XSLTemplate.3.0"]);
        };
        this.template = new ActiveXObject(_SARISSA_XSLTEMPLATE_PROGID);
        this.processor = null;
    };
    /**
     * Imports the given XSLT DOM and compiles it to a reusable transform
     * <b>Note:</b> If the stylesheet was loaded from a URL and contains xsl:import or xsl:include elements,it will be reloaded to resolve those
     * @argument xslDoc The XSLT DOMDocument to import
     */
    XSLTProcessor.prototype.importStylesheet = function(xslDoc){
		// xslDoc MUST be created using getXsltDocument()
        xslDoc.setProperty("SelectionLanguage", "XPath");
        xslDoc.setProperty("SelectionNamespaces", "xmlns:xsl='http://www.w3.org/1999/XSL/Transform'");
        var output = xslDoc.selectSingleNode("//xsl:output");
        this.outputMethod = output ? output.getAttribute("method") : "html";
        this.template.stylesheet = xslDoc;
        this.processor = this.template.createProcessor();
        // for getParameter and clearParameters
        this.paramsSet = new Array();
    };

    /**
     * Transform the given XML DOM and return the transformation result as a new DOM document
     * @argument sourceDoc The XML DOMDocument to transform
     * @return The transformation result as a DOM Document
     */
    XSLTProcessor.prototype.transformToDocument = function(sourceDoc){
        // fix for bug 1549749
        if(_SARISSA_THREADEDDOM_PROGID){
            this.processor.input=sourceDoc;
            var outDoc=new ActiveXObject(_SARISSA_DOM_PROGID);
            this.processor.output=outDoc;
            this.processor.transform();
            return outDoc;
        }
        else{
            if(!_SARISSA_DOM_XMLWRITER){
                _SARISSA_DOM_XMLWRITER = Sarissa.pickRecentProgID(["MSXML2.MXXMLWriter.6.0", "MSXML2.MXXMLWriter.3.0", "MSXML2.MXXMLWriter", "MSXML.MXXMLWriter", "Microsoft.XMLDOM"]);
            };
            this.processor.input = sourceDoc;
            var outDoc = new ActiveXObject(_SARISSA_DOM_XMLWRITER);
            this.processor.output = outDoc; 
            this.processor.transform();
            var oDoc = new ActiveXObject(_SARISSA_DOM_PROGID);
            oDoc.loadXML(outDoc.output+"");
            return oDoc;
        };
    };
    
    /**
     * Transform the given XML DOM and return the transformation result as a new DOM fragment.
     * <b>Note</b>: The xsl:output method must match the nature of the owner document (XML/HTML).
     * @argument sourceDoc The XML DOMDocument to transform
     * @argument ownerDoc The owner of the result fragment
     * @return The transformation result as a DOM Document
     */
    XSLTProcessor.prototype.transformToFragment = function (sourceDoc, ownerDoc) {
        this.processor.input = sourceDoc;
        this.processor.transform();
        var s = this.processor.output;
        var f = ownerDoc.createDocumentFragment();
        if (this.outputMethod == 'text') {
            f.appendChild(ownerDoc.createTextNode(s));
        } else if (ownerDoc.body && ownerDoc.body.innerHTML) {
            var container = ownerDoc.createElement('div');
            container.innerHTML = s;
            while (container.hasChildNodes()) {
                f.appendChild(container.firstChild);
            }
        }
        else {
            var oDoc = new ActiveXObject(_SARISSA_DOM_PROGID);
            if (s.substring(0, 5) == '<?xml') {
                s = s.substring(s.indexOf('?>') + 2);
            }
            var xml = ''.concat('<my>', s, '</my>');
            oDoc.loadXML(xml);
            var container = oDoc.documentElement;
            while (container.hasChildNodes()) {
                f.appendChild(container.firstChild);
            }
        }
        return f;
    };
    
    /**
     * Set global XSLT parameter of the imported stylesheet
     * @argument nsURI The parameter namespace URI
     * @argument name The parameter base name
     * @argument value The new parameter value
     */
    XSLTProcessor.prototype.setParameter = function(nsURI, name, value){
        // make value a zero length string if null to allow clearing
        value = value ? value : "";
        // nsURI is optional but cannot be null 
        if(nsURI){
            this.processor.addParameter(name, value, nsURI);
        }else{
            this.processor.addParameter(name, value);
        };
        // update updated params for getParameter 
        if(!this.paramsSet[""+nsURI]){
            this.paramsSet[""+nsURI] = new Array();
        };
        this.paramsSet[""+nsURI][name] = value;
    };
    /**
     * Gets a parameter if previously set by setParameter. Returns null
     * otherwise
     * @argument name The parameter base name
     * @argument value The new parameter value
     * @return The parameter value if reviously set by setParameter, null otherwise
     */
    XSLTProcessor.prototype.getParameter = function(nsURI, name){
        nsURI = "" + nsURI;
        if(this.paramsSet[nsURI] && this.paramsSet[nsURI][name]){
            return this.paramsSet[nsURI][name];
        }else{
            return null;
        };
    };
    /**
     * Clear parameters (set them to default values as defined in the stylesheet itself)
     */
    XSLTProcessor.prototype.clearParameters = function(){
        for(var nsURI in this.paramsSet){
            for(var name in this.paramsSet[nsURI]){
                if(nsURI){
                    this.processor.addParameter(name, "", nsURI);
                }else{
                    this.processor.addParameter(name, "");
                };
            };
        };
        this.paramsSet = new Array();
    };
}else{ /* end IE initialization, try to deal with real browsers now ;-) */
    if(_SARISSA_HAS_DOM_CREATE_DOCUMENT){
        /**
         * <p>Ensures the document was loaded correctly, otherwise sets the
         * parseError to -1 to indicate something went wrong. Internal use</p>
         * @private
         */
        Sarissa.__handleLoad__ = function(oDoc){
            Sarissa.__setReadyState__(oDoc, 4);
        };
        /**
        * <p>Attached by an event handler to the load event. Internal use.</p>
        * @private
        */
        _sarissa_XMLDocument_onload = function(){
            Sarissa.__handleLoad__(this);
        };
        /**
         * <p>Sets the readyState property of the given DOM Document object.
         * Internal use.</p>
         * @private
         * @argument oDoc the DOM Document object to fire the
         *          readystatechange event
         * @argument iReadyState the number to change the readystate property to
         */
        Sarissa.__setReadyState__ = function(oDoc, iReadyState){
            oDoc.readyState = iReadyState;
            oDoc.readystate = iReadyState;
            if (oDoc.onreadystatechange != null && typeof oDoc.onreadystatechange == "function")
                oDoc.onreadystatechange();
        };
        Sarissa.getDomDocument = function(sUri, sName){
            var oDoc = document.implementation.createDocument(sUri?sUri:null, sName?sName:null, null);
            if(!oDoc.onreadystatechange){
            
                /**
                * <p>Emulate IE's onreadystatechange attribute</p>
                */
                oDoc.onreadystatechange = null;
            };
            if(!oDoc.readyState){
                /**
                * <p>Emulates IE's readyState property, which always gives an integer from 0 to 4:</p>
                * <ul><li>1 == LOADING,</li>
                * <li>2 == LOADED,</li>
                * <li>3 == INTERACTIVE,</li>
                * <li>4 == COMPLETED</li></ul>
                */
                oDoc.readyState = 0;
            };
            oDoc.addEventListener("load", _sarissa_XMLDocument_onload, false);
            return oDoc;
        };
        Sarissa.getXsltDocument = Sarissa.getDomDocument;
        if(window.XMLDocument){
            // do nothing
        }// TODO: check if the new document has content before trying to copynodes, check  for error handling in DOM 3 LS
        else if(_SARISSA_HAS_DOM_FEATURE && window.Document && !Document.prototype.load && document.implementation.hasFeature('LS', '3.0')){
            //Opera 9 may get the XPath branch which gives creates XMLDocument, therefore it doesn't reach here which is good
            /**
            * <p>Factory method to obtain a new DOM Document object</p>
            * @argument sUri the namespace of the root node (if any)
            * @argument sUri the local name of the root node (if any)
            * @returns a new DOM Document
            */
            Sarissa.getDomDocument = function(sUri, sName){
                var oDoc = document.implementation.createDocument(sUri?sUri:null, sName?sName:null, null);
                return oDoc;
            };
            Sarissa.getXsltDocument = Sarissa.getDomDocument;
        }
        else {
            Sarissa.getDomDocument = function(sUri, sName){
                var oDoc = document.implementation.createDocument(sUri?sUri:null, sName?sName:null, null);
                // looks like safari does not create the root element for some unknown reason
                if(oDoc && (sUri || sName) && !oDoc.documentElement){
                    oDoc.appendChild(oDoc.createElementNS(sUri, sName));
                };
                return oDoc;
            };
            Sarissa.getXsltDocument = Sarissa.getDomDocument;
        };
    };//if(_SARISSA_HAS_DOM_CREATE_DOCUMENT)
};
//==========================================
// Common stuff
//==========================================
if(!window.DOMParser){
    if(_SARISSA_IS_SAFARI){
        /*
         * DOMParser is a utility class, used to construct DOMDocuments from XML strings
         * @constructor
         */
        DOMParser = function() { };
        /** 
        * Construct a new DOM Document from the given XMLstring
        * @param sXml the given XML string
        * @param contentType the content type of the document the given string represents (one of text/xml, application/xml, application/xhtml+xml). 
        * @return a new DOM Document from the given XML string
        */
        DOMParser.prototype.parseFromString = function(sXml, contentType){
            var xmlhttp = new XMLHttpRequest();
            xmlhttp.open("GET", "data:text/xml;charset=utf-8," + encodeURIComponent(sXml), false);
            xmlhttp.send(null);
            return xmlhttp.responseXML;
        };
    }else if(Sarissa.getDomDocument && Sarissa.getDomDocument() && Sarissa.getDomDocument(null, "bar").xml){
        DOMParser = function() { };
        DOMParser.prototype.parseFromString = function(sXml, contentType){
			var doc = Sarissa.getDomDocument();
            doc.loadXML(sXml);
            return doc;
        };
    };
};

if((typeof(document.importNode) == "undefined") && _SARISSA_IS_IE){
    try{
        /**
        * Implementation of importNode for the context window document in IE.
        * If <code>oNode</code> is a TextNode, <code>bChildren</code> is ignored.
        * @param oNode the Node to import
        * @param bChildren whether to include the children of oNode
        * @returns the imported node for further use
        */
        document.importNode = function(oNode, bChildren){
            var tmp;
            if (oNode.nodeName=='#text') {
                return document.createTextElement(oNode.data);
            }
            else {
                if(oNode.nodeName == "tbody" || oNode.nodeName == "tr"){
                    tmp = document.createElement("table");
                }
                else if(oNode.nodeName == "td"){
                    tmp = document.createElement("tr");
                }
                else if(oNode.nodeName == "option"){
                    tmp = document.createElement("select");
                }
                else{
                    tmp = document.createElement("div");
                };
                if(bChildren){
                    tmp.innerHTML = oNode.xml ? oNode.xml : oNode.outerHTML;
                }else{
                    tmp.innerHTML = oNode.xml ? oNode.cloneNode(false).xml : oNode.cloneNode(false).outerHTML;
                };
                return tmp.getElementsByTagName("*")[0];
            };
            
        };
    }catch(e){ };
};
if(!Sarissa.getParseErrorText){
    /**
     * <p>Returns a human readable description of the parsing error. Usefull
     * for debugging. Tip: append the returned error string in a &lt;pre&gt;
     * element if you want to render it.</p>
     * <p>Many thanks to Christian Stocker for the initial patch.</p>
     * @argument oDoc The target DOM document
     * @returns The parsing error description of the target Document in
     *          human readable form (preformated text)
     */
    Sarissa.getParseErrorText = function (oDoc){
        var parseErrorText = Sarissa.PARSED_OK;
        if(!oDoc.documentElement){
            parseErrorText = Sarissa.PARSED_EMPTY;
        } else if(oDoc.documentElement.tagName == "parsererror"){
            parseErrorText = oDoc.documentElement.firstChild.data;
            parseErrorText += "\n" +  oDoc.documentElement.firstChild.nextSibling.firstChild.data;
        } else if(oDoc.getElementsByTagName("parsererror").length > 0){
            var parsererror = oDoc.getElementsByTagName("parsererror")[0];
            parseErrorText = Sarissa.getText(parsererror, true)+"\n";
        } else if(oDoc.parseError && oDoc.parseError.errorCode != 0){
            parseErrorText = Sarissa.PARSED_UNKNOWN_ERROR;
        };
        return parseErrorText;
    };
};
Sarissa.getText = function(oNode, deep){
    var s = "";
    var nodes = oNode.childNodes;
    for(var i=0; i < nodes.length; i++){
        var node = nodes[i];
        var nodeType = node.nodeType;
        if(nodeType == Node.TEXT_NODE || nodeType == Node.CDATA_SECTION_NODE){
            s += node.data;
        } else if(deep == true
                    && (nodeType == Node.ELEMENT_NODE
                        || nodeType == Node.DOCUMENT_NODE
                        || nodeType == Node.DOCUMENT_FRAGMENT_NODE)){
            s += Sarissa.getText(node, true);
        };
    };
    return s;
};
if(!window.XMLSerializer 
    && Sarissa.getDomDocument 
    && Sarissa.getDomDocument("","foo", null).xml){
    /**
     * Utility class to serialize DOM Node objects to XML strings
     * @constructor
     */
    XMLSerializer = function(){};
    /**
     * Serialize the given DOM Node to an XML string
     * @param oNode the DOM Node to serialize
     */
    XMLSerializer.prototype.serializeToString = function(oNode) {
        return oNode.xml;
    };
};

/**
 * strips tags from a markup string
 */
Sarissa.stripTags = function (s) {
    return s.replace(/<[^>]+>/g,"");
};
/**
 * <p>Deletes all child nodes of the given node</p>
 * @argument oNode the Node to empty
 */
Sarissa.clearChildNodes = function(oNode) {
    // need to check for firstChild due to opera 8 bug with hasChildNodes
    while(oNode.firstChild) {
        oNode.removeChild(oNode.firstChild);
    };
};
/**
 * <p> Copies the childNodes of nodeFrom to nodeTo</p>
 * <p> <b>Note:</b> The second object's original content is deleted before 
 * the copy operation, unless you supply a true third parameter</p>
 * @argument nodeFrom the Node to copy the childNodes from
 * @argument nodeTo the Node to copy the childNodes to
 * @argument bPreserveExisting whether to preserve the original content of nodeTo, default is false
 */
Sarissa.copyChildNodes = function(nodeFrom, nodeTo, bPreserveExisting) {
    if((!nodeFrom) || (!nodeTo)){
        throw "Both source and destination nodes must be provided";
    };
    if(!bPreserveExisting){
        Sarissa.clearChildNodes(nodeTo);
    };
    var ownerDoc = nodeTo.nodeType == Node.DOCUMENT_NODE ? nodeTo : nodeTo.ownerDocument;
    var nodes = nodeFrom.childNodes;
    if(typeof(ownerDoc.importNode) != "undefined")  {
        for(var i=0;i < nodes.length;i++) {
            nodeTo.appendChild(ownerDoc.importNode(nodes[i], true));
        };
    } else {
        for(var i=0;i < nodes.length;i++) {
            nodeTo.appendChild(nodes[i].cloneNode(true));
        };
    };
};

/**
 * <p> Moves the childNodes of nodeFrom to nodeTo</p>
 * <p> <b>Note:</b> The second object's original content is deleted before 
 * the move operation, unless you supply a true third parameter</p>
 * @argument nodeFrom the Node to copy the childNodes from
 * @argument nodeTo the Node to copy the childNodes to
 * @argument bPreserveExisting whether to preserve the original content of nodeTo, default is
 */ 
Sarissa.moveChildNodes = function(nodeFrom, nodeTo, bPreserveExisting) {
    if((!nodeFrom) || (!nodeTo)){
        throw "Both source and destination nodes must be provided";
    };
    if(!bPreserveExisting){
        Sarissa.clearChildNodes(nodeTo);
    };
    var nodes = nodeFrom.childNodes;
    // if within the same doc, just move, else copy and delete
    if(nodeFrom.ownerDocument == nodeTo.ownerDocument){
        while(nodeFrom.firstChild){
            nodeTo.appendChild(nodeFrom.firstChild);
        };
    } else {
        var ownerDoc = nodeTo.nodeType == Node.DOCUMENT_NODE ? nodeTo : nodeTo.ownerDocument;
        if(typeof(ownerDoc.importNode) != "undefined") {
           for(var i=0;i < nodes.length;i++) {
               nodeTo.appendChild(ownerDoc.importNode(nodes[i], true));
           };
        }else{
           for(var i=0;i < nodes.length;i++) {
               nodeTo.appendChild(nodes[i].cloneNode(true));
           };
        };
        Sarissa.clearChildNodes(nodeFrom);
    };
};

/** 
 * <p>Serialize any object to an XML string. All properties are serialized using the property name
 * as the XML element name. Array elements are rendered as <code>array-item</code> elements, 
 * using their index/key as the value of the <code>key</code> attribute.</p>
 * @argument anyObject the object to serialize
 * @argument objectName a name for that object
 * @return the XML serializationj of the given object as a string
 */
Sarissa.xmlize = function(anyObject, objectName, indentSpace){
    indentSpace = indentSpace?indentSpace:'';
    var s = indentSpace  + '<' + objectName + '>';
    var isLeaf = false;
    if(!(anyObject instanceof Object) || anyObject instanceof Number || anyObject instanceof String 
        || anyObject instanceof Boolean || anyObject instanceof Date){
        s += Sarissa.escape(""+anyObject);
        isLeaf = true;
    }else{
        s += "\n";
        var itemKey = '';
        var isArrayItem = anyObject instanceof Array;
        for(var name in anyObject){
            s += Sarissa.xmlize(anyObject[name], (isArrayItem?"array-item key=\""+name+"\"":name), indentSpace + "   ");
        };
        s += indentSpace;
    };
    return s += (objectName.indexOf(' ')!=-1?"</array-item>\n":"</" + objectName + ">\n");
};

/** 
 * Escape the given string chacters that correspond to the five predefined XML entities
 * @param sXml the string to escape
 */
Sarissa.escape = function(sXml){
    return sXml.replace(/&/g, "&amp;").replace(/</g, "&lt;").replace(/>/g, "&gt;").replace(/\"/g, "&quot;").replace(/\'/g, "&apos;");
};

/** 
 * Unescape the given string. This turns the occurences of the predefined XML 
 * entities to become the characters they represent correspond to the five predefined XML entities
 * @param sXml the string to unescape
 */
Sarissa.unescape = function(sXml){
    return sXml.replace(/&apos\;/g,"'").replace(/&quot\;/g,"\"").replace(/&gt\;/g,">").replace(/&lt\;/g,"<").replace(/&amp\;/g,"&");
};

/* copy of sarissa_ieemu_xpath.js */
/**
 * ====================================================================
 * About
 * ====================================================================
 * Sarissa cross browser XML library - IE XPath Emulation 
 * @version ${project.version}
 * @author: Manos Batsis, mailto: mbatsis at users full stop sourceforge full stop net
 *
 * This script emulates Internet Explorer's selectNodes and selectSingleNode
 * for Mozilla. Associating namespace prefixes with URIs for your XPath queries
 * is easy with IE's setProperty. 
 * USers may also map a namespace prefix to a default (unprefixed) namespace in the
 * source document with Sarissa.setXpathNamespaces
 *
 *
 * ====================================================================
 * Licence
 * ====================================================================
 * Sarissa is free software distributed under the GNU GPL version 2 (see <a href="gpl.txt">gpl.txt</a>) or higher, 
 * GNU LGPL version 2.1 (see <a href="lgpl.txt">lgpl.txt</a>) or higher and Apache Software License 2.0 or higher 
 * (see <a href="asl.txt">asl.txt</a>). This means you can choose one of the three and use that if you like. If 
 * you make modifications under the ASL, i would appreciate it if you submitted those.
 * In case your copy of Sarissa does not include the license texts, you may find
 * them online in various formats at <a href="http://www.gnu.org">http://www.gnu.org</a> and 
 * <a href="http://www.apache.org">http://www.apache.org</a>.
 *
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY 
 * KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE 
 * WARRANTIES OF MERCHANTABILITY,FITNESS FOR A PARTICULAR PURPOSE 
 * AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR 
 * COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
 * LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR 
 * OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE 
 * SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
 */
if(_SARISSA_HAS_DOM_FEATURE && document.implementation.hasFeature("XPath", "3.0")){
    /**
    * <p>SarissaNodeList behaves as a NodeList but is only used as a result to <code>selectNodes</code>,
    * so it also has some properties IEs proprietery object features.</p>
    * @private
    * @constructor
    * @argument i the (initial) list size
    */
    function SarissaNodeList(i){
        this.length = i;
    };
    /** <p>Set an Array as the prototype object</p> */
    SarissaNodeList.prototype = new Array(0);
    /** <p>Inherit the Array constructor </p> */
    SarissaNodeList.prototype.constructor = Array;
    /**
    * <p>Returns the node at the specified index or null if the given index
    * is greater than the list size or less than zero </p>
    * <p><b>Note</b> that in ECMAScript you can also use the square-bracket
    * array notation instead of calling <code>item</code>
    * @argument i the index of the member to return
    * @returns the member corresponding to the given index
    */
    SarissaNodeList.prototype.item = function(i) {
        return (i < 0 || i >= this.length)?null:this[i];
    };
    /**
    * <p>Emulate IE's expr property
    * (Here the SarissaNodeList object is given as the result of selectNodes).</p>
    * @returns the XPath expression passed to selectNodes that resulted in
    *          this SarissaNodeList
    */
    SarissaNodeList.prototype.expr = "";
    /** dummy, used to accept IE's stuff without throwing errors */
    if(window.XMLDocument && (!XMLDocument.prototype.setProperty)){
        XMLDocument.prototype.setProperty  = function(x,y){};
    };
    /**
    * <p>Programmatically control namespace URI/prefix mappings for XPath
    * queries.</p>
    * <p>This method comes especially handy when used to apply XPath queries
    * on XML documents with a default namespace, as there is no other way
    * of mapping that to a prefix.</p>
    * <p>Using no namespace prefix in DOM Level 3 XPath queries, implies you
    * are looking for elements in the null namespace. If you need to look
    * for nodes in the default namespace, you need to map a prefix to it
    * first like:</p>
    * <pre>Sarissa.setXpathNamespaces(oDoc, &quot;xmlns:myprefix=&amp;aposhttp://mynsURI&amp;apos&quot;);</pre>
    * <p><b>Note 1 </b>: Use this method only if the source document features
    * a default namespace (without a prefix), otherwise just use IE's setProperty
    * (moz will rezolve non-default namespaces by itself). You will need to map that
    * namespace to a prefix for queries to work.</p>
    * <p><b>Note 2 </b>: This method calls IE's setProperty method to set the
    * appropriate namespace-prefix mappings, so you dont have to do that.</p>
    * @param oDoc The target XMLDocument to set the namespace mappings for.
    * @param sNsSet A whilespace-seperated list of namespace declarations as
    *            those would appear in an XML document. E.g.:
    *            <code>&quot;xmlns:xhtml=&apos;http://www.w3.org/1999/xhtml&apos;
    * xmlns:&apos;http://www.w3.org/1999/XSL/Transform&apos;&quot;</code>
    * @throws An error if the format of the given namespace declarations is bad.
    */
    Sarissa.setXpathNamespaces = function(oDoc, sNsSet) {
        //oDoc._sarissa_setXpathNamespaces(sNsSet);
        oDoc._sarissa_useCustomResolver = true;
        var namespaces = sNsSet.indexOf(" ")>-1?sNsSet.split(" "):new Array(sNsSet);
        oDoc._sarissa_xpathNamespaces = new Array(namespaces.length);
        for(var i=0;i < namespaces.length;i++){
            var ns = namespaces[i];
            var colonPos = ns.indexOf(":");
            var assignPos = ns.indexOf("=");
            if(colonPos > 0 && assignPos > colonPos+1){
                var prefix = ns.substring(colonPos+1, assignPos);
                var uri = ns.substring(assignPos+2, ns.length-1);
                oDoc._sarissa_xpathNamespaces[prefix] = uri;
            }else{
                throw "Bad format on namespace declaration(s) given";
            };
        };
    };
    /**
    * @private Flag to control whether a custom namespace resolver should
    *          be used, set to true by Sarissa.setXpathNamespaces
    */
    XMLDocument.prototype._sarissa_useCustomResolver = false;
    /** @private */
    XMLDocument.prototype._sarissa_xpathNamespaces = new Array();
    /**
    * <p>Extends the XMLDocument to emulate IE's selectNodes.</p>
    * @argument sExpr the XPath expression to use
    * @argument contextNode this is for internal use only by the same
    *           method when called on Elements
    * @returns the result of the XPath search as a SarissaNodeList
    * @throws An error if no namespace URI is found for the given prefix.
    */
    XMLDocument.prototype.selectNodes = function(sExpr, contextNode, returnSingle){
        var nsDoc = this;
        var nsresolver = this._sarissa_useCustomResolver
        ? function(prefix){
            var s = nsDoc._sarissa_xpathNamespaces[prefix];
            if(s)return s;
            else throw "No namespace URI found for prefix: '" + prefix+"'";
            }
        : this.createNSResolver(this.documentElement);
        var result = null;
        if(!returnSingle){
            var oResult = this.evaluate(sExpr,
                (contextNode?contextNode:this),
                nsresolver,
                XPathResult.ORDERED_NODE_SNAPSHOT_TYPE, null);
            var nodeList = new SarissaNodeList(oResult.snapshotLength);
            nodeList.expr = sExpr;
            for(var i=0;i<nodeList.length;i++)
                nodeList[i] = oResult.snapshotItem(i);
            result = nodeList;
        }
        else {
            result = oResult = this.evaluate(sExpr,
                (contextNode?contextNode:this),
                nsresolver,
                XPathResult.FIRST_ORDERED_NODE_TYPE, null).singleNodeValue;
        };
        return result;      
    };
    /**
    * <p>Extends the Element to emulate IE's selectNodes</p>
    * @argument sExpr the XPath expression to use
    * @returns the result of the XPath search as an (Sarissa)NodeList
    * @throws An
    *             error if invoked on an HTML Element as this is only be
    *             available to XML Elements.
    */
    Element.prototype.selectNodes = function(sExpr){
        var doc = this.ownerDocument;
        if(doc.selectNodes)
            return doc.selectNodes(sExpr, this);
        else
            throw "Method selectNodes is only supported by XML Elements";
    };
    /**
    * <p>Extends the XMLDocument to emulate IE's selectSingleNode.</p>
    * @argument sExpr the XPath expression to use
    * @argument contextNode this is for internal use only by the same
    *           method when called on Elements
    * @returns the result of the XPath search as an (Sarissa)NodeList
    */
    XMLDocument.prototype.selectSingleNode = function(sExpr, contextNode){
        var ctx = contextNode?contextNode:null;
        return this.selectNodes(sExpr, ctx, true);
    };
    /**
    * <p>Extends the Element to emulate IE's selectSingleNode.</p>
    * @argument sExpr the XPath expression to use
    * @returns the result of the XPath search as an (Sarissa)NodeList
    * @throws An error if invoked on an HTML Element as this is only be
    *             available to XML Elements.
    */
    Element.prototype.selectSingleNode = function(sExpr){
        var doc = this.ownerDocument;
        if(doc.selectSingleNode)
            return doc.selectSingleNode(sExpr, this);
        else
            throw "Method selectNodes is only supported by XML Elements";
    };
    Sarissa.IS_ENABLED_SELECT_NODES = true;
};

/* ************************************************************* */


