// Copyright 2000-2007, Ektron, Inc.
// Revision Date: 2007-02-06

/* It is best NOT to modify this file. */
/*
	See the Developer's Reference Guide for details.
	
	To add your own commands, define one or more of the following:
	eWebEditProExecCommandHandlers[your_cmd_here] = function(sEditorName, strCmdName, strTextData, lData) { }
	function eWebEditProExecCommand(sEditorName, strCmdName, strTextData, lData) { }
	eWebEditPro.onexeccommand = your_custom_event_handler;
	
	To add your own media file handler, define:
	function eWebEditProMediaSelection(sEditorName, sText, lData) { } (for web page using HTTP)
	function eWebEditProMediaNotification(sEditorName) { } (for FTP)
		
	To add your own double-click element handler, define one or more of the following:
	function eWebEditProDblClickElement(oElement) { }
	function eWebEditProDblClickHyperlink(oElement) { }
	function eWebEditProDblClickImage(oElement) { }
	function eWebEditProDblClickTable(oElement) { }
	eWebEditPro.ondblclickelement = your_custom_event_handler;
*/

function onExecCommandHandler(strCmdName, strTextData, lData)
{
/*
	Defer call to actual handler for two reasons:
	1. Avoid recursion in case an action results in this same event firing.
	2. Netscape cannot effectively access the ActiveX control's methods in an event.
*/
	var sEditorName = eWebEditPro.event.srcName;
	strCmdName = strCmdName + ""; // ensure it is a string
	strTextData = strTextData + ""; // ensure it is a string
	lData = lData * 1; // ensure it is a number
	setTimeout('onExecCommandDeferred("' + sEditorName + '", "' + strCmdName + '", ' + toLiteral(strTextData) + ', ' + lData + ')', 1);
}

function onExecCommandDeferred(sEditorName, strCmdName, strTextData, lData)
{
	if ("initialize" == strCmdName)
	{
		var objInstance = eWebEditPro.instances[sEditorName];
		if (typeof objInstance != "undefined" && objInstance != null)
		{
			objInstance.receivedEvent = true;
			if (objInstance.isReady())
			{
				// Respond to the "initialize" event by sending "ready".
				// Responding is optional, but it speeds up initialization.
				// Cannot use eWebEditPro[sEditorName] during "initialize" event.
				// Sync API: objInstance.editor.ExecCommand("ready", "", 0);
				objInstance.asyncCallMethod("ExecCommand", ["ready", "", 0], null, new Function());
			}
		}
		return;
	}
	
	if ("ready" == strCmdName)
	{
		var objInstance = eWebEditPro.instances[sEditorName];
		objInstance.receivedEvent = true;
		if (objInstance.loadWhenReady)
		{
			eWebEditPro.load(objInstance);
		}
		if (objInstance.isReady())
		{
			if ("function" == typeof eWebEditProReady)
			{
				eWebEditProReady(sEditorName);
			}
			if (typeof eWebEditPro.onready != "undefined")
			{
				eWebEditPro.initEvent("onready");
				eWebEditPro.event.type = "ready"; 
				eWebEditPro.event.srcName = sEditorName;
				eWebEditPro.raiseEvent("onready");
			}
		}
		return;
	}
	
	if ("blur" == strCmdName)
	{
		// This command is raised when pressing Ctrl+Tab 
		// (unless Netscape captures the event).
		// Move focus from the editor to the next form field.
		var objInstance = eWebEditPro.instances[sEditorName];
		var objField = eWebEditPro.nextFormField(objInstance);
		if (objField)
		{
			objField.focus();
		}
		return;
	}
	
	var returnValue = true;
	if ("function" == typeof eWebEditProExecCommand)
	{
		returnValue = eWebEditProExecCommand(sEditorName, strCmdName, strTextData, lData);
	}
	
	if (returnValue != false)
	{
		var fnHandler = eWebEditProExecCommandHandlers[strCmdName];
		if ("function" == typeof fnHandler)
		{
			fnHandler(sEditorName, strCmdName, strTextData, lData);
		}
	}
		
	if (typeof eWebEditPro.onexeccommand != "undefined")
	{
		eWebEditPro.initEvent("onexeccommand");
		eWebEditPro.event.type = "execcommand"; 
		eWebEditPro.event.srcName = sEditorName;
		eWebEditPro.event.cmdName = strCmdName;
		eWebEditPro.event.textData = strTextData;
		eWebEditPro.event.data = lData;
		eWebEditPro.raiseEvent("onexeccommand");
	}
}

// global array of command handlers indexed by command name.
var eWebEditProExecCommandHandlers = new Array();

eWebEditProExecCommandHandlers["toolbarreset"] = function(sEditorName, strCmdName, strTextData, lData) 
{ 
	if (typeof eWebEditPro.ontoolbarreset != "undefined")
	{
		eWebEditPro.initEvent("ontoolbarreset");
		eWebEditPro.event.type = "toolbarreset"; 
		eWebEditPro.event.srcName = sEditorName;
		eWebEditPro.raiseEvent("ontoolbarreset");
	}
	var bValidReq = false;
	if ( eWebEditPro.isIE && eWebEditPro.browserVersion >= 5.0 ) 
	{
		var bValidReq = true;
	}
	if ( !bValidReq )
	{
		var objInstance = eWebEditPro.instances[sEditorName]; 
		var objMenu = objInstance.editor.Toolbars();
		objMenu.CommandDelete("js508table");
	}
} 

eWebEditProExecCommandHandlers["getcssrules"] = function(sEditorName, strCmdName, strTextData, lData) 
{ 
	var objInstance = eWebEditPro.instances[sEditorName]; 
	if (objInstance)
	{
		try
		{
			var strCssText = strTextData;
			var objIFrame = document.getElementById("fraCssReader");
			if (!objIFrame)
			{
				objIFrame = document.createElement("iframe");
				objIFrame.id = "fraCssReader";
				objIFrame.style.display = "none";
				objIFrame.src = eWebEditPro.resolvePath("accesscsspage.htm");
				document.body.appendChild(objIFrame);
			}
			setTimeout(function(/*objInstance, objIFrame, strCssText*/) { // delay to allow time to load .src
			try
			{
				var objDoc = objIFrame.contentWindow.document;
				var objHead = objDoc.getElementsByTagName("head")[0];
				if (null == objHead) 
				{
					setTimeout(arguments.callee, 20);
					return;
				}

				var objStyleElem = objDoc.createElement("style");
				objStyleElem.type = "text/css";
			
				strCssText = strCssText.replace(/\@import\s+url\([^\)]*\);?/gi, ""); // @import causes IE to crash and not supported by Mozilla
				if (objStyleElem.styleSheet) // IE
				{
					objStyleElem.styleSheet.cssText = strCssText;
				}
				else // Mozilla
				{
					var objCssText = objDoc.createTextNode(strCssText);
					objStyleElem.appendChild(objCssText);
				}
				
				objHead.appendChild(objStyleElem); // "Access is denied" error occurs if this line is missing
				var objRules = null;
				if (objStyleElem.styleSheet) // IE
				{
					objRules = objStyleElem.styleSheet.rules;
				}
				else // Mozilla
				{
					objRules = objDoc.styleSheets[objDoc.styleSheets.length - 1].cssRules;
				}
			
				var strRules = "";
				var objRule = null;
				for (var i = 0; i < objRules.length; i++)
				{
					objRule = objRules[i];
					try
					{
						var selText = objRule.selectorText + ""; // ensure it is a string
						if (selText.indexOf(".") >= 0) // only keep rules that define classes
						{
							var bVisible = true;
							try { if (objRule.style.visible == "false") bVisible = false; } catch (e) {};
							
							if (bVisible)
							{
								strRules += '<rule sel="' + eWebEditProUtil.HTMLEncode(objRule.selectorText);
								strRules += '" txt="' + eWebEditProUtil.HTMLEncode(objRule.style.cssText);
								try { if (objRule.style.visible) strRules += '" vis="' + eWebEditProUtil.HTMLEncode(objRule.style.visible); } catch (e) {};
								try { if (objRule.style.localeRef) strRules += '" ref="' + eWebEditProUtil.HTMLEncode(objRule.style.localeRef); } catch (e) {};
								try { if (objRule.style.caption) strRules += '" cap="' + eWebEditProUtil.HTMLEncode(objRule.style.caption); } catch (e) {};
								try { if (objRule.style.isStyleInternal) strRules += '" int="' + eWebEditProUtil.HTMLEncode(objRule.style.isStyleInternal); } catch (e) {};
								try { if (objRule.style.equivClass) strRules += '" eqv="' + eWebEditProUtil.HTMLEncode(objRule.style.equivClass); } catch (e) {};
								strRules += '" />\r\n';
							}
						}
					}
					catch (e)
					{
						// ignore and continue
					}
				}
				objInstance.asyncCallMethod("ExecCommand", ["setcssrules", strRules, 0], null, new Function());
			}
			catch (e)
			{
				//alert(e.message);
				//throw e;
			}
			}, 1); // setTimeout
		}
		catch (e)
		{
			//alert(e.message);
			//throw e;
		}
	}
} 


eWebEditProExecCommandHandlers["jstm"] = function(sEditorName, strCmdName, strTextData, lData) 
{ 
	// Sync API: eWebEditPro.instances[sEditorName].editor.pasteHTML('<sup><small>TM</small></sup>');
	eWebEditPro.instances[sEditorName].asyncCallMethod("pasteHTML", ['<sup><small>TM</small></sup>'], null, new Function());
} 

eWebEditProExecCommandHandlers["jshyperlink"] = function(sEditorName, strCmdName, strTextData, lData)
{
	eWebEditPro.openDialog(sEditorName, eWebEditPro.resolvePath("hyperlinkpopup.htm"), "", "HyperlinkList", "width=500,height=200");
}

eWebEditProExecCommandHandlers["cmdmfumedia"] = function(sEditorName, strCmdName, strTextData, lData)
{
	if (!eWebEditPro.instances[sEditorName].isEditor())
	{
		return; // write async
	}
	if (eWebEditPro.instances[sEditorName].editor.MediaFile().getPropertyBoolean("HandledInternally") == false)
	{
        // This is for backwards compatibility.
        // We no longer provide this, but the customer may 
        // have created their own.
		if ("function" == typeof eWebEditProMediaSelection)
		{
			eWebEditProMediaSelection(sEditorName, strTextData, lData);
		}
	}
	else
	{
		if ("function" == typeof eWebEditProMediaNotification)
		{
			eWebEditProMediaNotification(sEditorName);
		}
	}
}

eWebEditProExecCommandHandlers["js508table"] = function(sEditorName, strCmdName, strTextData, lData) 
{
	if ( eWebEditPro.isIE && eWebEditPro.browserVersion >= 5.0 ) 
	{
		var strTextData = eWebEditPro[sEditorName].getSelectedHTML();
		var strTemp = strTextData.substr(0, 10);
		var iPos = strTemp.indexOf("<");
		var bValidTable = false;
		if ( iPos > 0 )
		{
			strTemp = strTextData.substring(iPos, iPos+6);
			strTemp = strTemp.toUpperCase();
			if ( "<TABLE" == strTemp ) 
			{ 
				// if TABLE is the first tag in the selected HTML
				// confirm that any leading chars are white space
				for ( var i = 0; i < iPos; i++ ) 
				{
					if ( strTextData.charCodeAt(i)==10 || strTextData.charCodeAt(i)==13 || strTextData.charCodeAt(i)==32 )
					{
						bValidTable = true;
					}
				}
			}
		}
		if ( bValidTable )
		{
			eWebEditPro.openDialog(sEditorName, eWebEditPro.resolvePath("section508table.htm"), "", "", "width=440,height=320,scrollbars=no,resizable=no, location=no,toolbar=no");
		}
		else 
		{
			if ( "object" == typeof eWebEditProMessages )
			{		
				alert(eWebEditProMessages.MsgsTableNotSelected);
			}
		}
	}
}

eWebEditProExecCommandHandlers["jsformform"] = function(sEditorName, strCmdName, strTextData, lData) 
{
	showFormElementDialog(sEditorName, 2, "frmForm", 620, 350);
}

eWebEditProExecCommandHandlers["jsformbutton"] = function(sEditorName, strCmdName, strTextData, lData) 
{
	showFormElementDialog(sEditorName, 9, "frmBBtn", 400, 200);
}

eWebEditProExecCommandHandlers["jsformsubmit"] = function(sEditorName, strCmdName, strTextData, lData) 
{
	showFormElementDialog(sEditorName, 0, "frmSBtn", 400, 200);
}

eWebEditProExecCommandHandlers["jsformreset"] = function(sEditorName, strCmdName, strTextData, lData) 
{
	showFormElementDialog(sEditorName, 1, "frmRBtn", 400, 200);
}

eWebEditProExecCommandHandlers["jsformhidden"] = function(sEditorName, strCmdName, strTextData, lData) 
{
	showFormElementDialog(sEditorName, 4, "frmHiddenFld", 400, 200);
}

eWebEditProExecCommandHandlers["jsformtext"] = function(sEditorName, strCmdName, strTextData, lData) 
{
	showFormElementDialog(sEditorName, 3, "frmTextFld", 400, 200);
}

eWebEditProExecCommandHandlers["jsformpassword"] = function(sEditorName, strCmdName, strTextData, lData) 
{
	showFormElementDialog(sEditorName, 8, "frmPasswordFld", 400, 200);
}

eWebEditProExecCommandHandlers["jsformtextarea"] = function(sEditorName, strCmdName, strTextData, lData) 
{
	showFormElementDialog(sEditorName, 5, "frmTextarea", 400, 250);
}

eWebEditProExecCommandHandlers["jsformradio"] = function(sEditorName, strCmdName, strTextData, lData) 
{
	showFormElementDialog(sEditorName, 6, "frmOptionBox", 400, 250);
}

eWebEditProExecCommandHandlers["jsformcheckbox"] = function(sEditorName, strCmdName, strTextData, lData) 
{
	showFormElementDialog(sEditorName, 7, "frmCheckbox", 400, 250);
}

eWebEditProExecCommandHandlers["jsformselect"] = function(sEditorName, strCmdName, strTextData, lData) 
{
	showFormElementDialog(sEditorName, 11, "frmDropList", 400, 600);
}

eWebEditProExecCommandHandlers["jsformfile"] = function(sEditorName, strCmdName, strTextData, lData) 
{
	showFormElementDialog(sEditorName, 10, "frmFormFile", 400, 200);
}

function showFormElementDialog(sEditorName, sFormElement, sWin, width, height)
{
	var bNetscape6 = (eWebEditPro.isNetscape && (eWebEditPro.browserVersion >= 6.0));
	if (bNetscape6)
	{
		onExecCommandHandler("blur", "", 0);
	}		
	var sWindowFeatures = "scrollbars,resizable,width=" + width + ",height=" + height;
	var sFilename = "formelementinsert.htm";
	if (11 == sFormElement)
	{
		sFilename = "formelementinsertframe.htm";
	}
	eWebEditPro.openDialog(sEditorName, eWebEditPro.resolvePath(sFilename), "formelement=" + escape(sFormElement), sWin, sWindowFeatures);
}

eWebEditProExecCommandHandlers["jscomment"] = function(sEditorName, strCmdName, strTextData, lData) 
{
	showCommentDialog(sEditorName);
}

function eWebEditProInsertButton(sEditorName, name, caption, attributes)
{
	var sHTML = "<button name=" + toLiteral(name)+ " " + attributes + ">" + caption + "</button>";
	eWebEditPro.instances[sEditorName].asyncCallMethod("pasteHTML", [sHTML], null, new Function());
}

eWebEditProExecCommandHandlers["mybtn"] = function(sEditorName, strCmdName, strTextData, lData)
{
	eWebEditProInsertButton(sEditorName, strCmdName, "Button", "");
}

eWebEditProExecCommandHandlers["clicktag"] = function(sEditorName, strCmdName, strTextData, lData) 
{
	var objXmlDoc = eWebEditPro.instances[sEditorName].editor.XMLProcessor();
	var objXmlTag = objXmlDoc.ActiveTag();
	if((typeof objXmlTag != "undefined") && (objXmlTag != null))
	{
		var sXPath = objXmlTag.GetXPath();
		if ("mybtn" == strTextData)
		{
			sXPath += "/Field1";
			var objDataFld = objXmlDoc.FindDataField(sXPath);
			if (objDataFld)
			{
				var sValue = objDataFld.getPropertyString("Content");
				alert(sValue);
			}
			else
			{
				alert("Could not find field with XPath: " + sXPath);
			}
		}
	}
}
/*
eWebEditProExecCommandHandlers["dblclicktag"] = function(sEditorName, strCmdName, strTextData, lData) 
{
	var objXmlTag = eWebEditPro.instances[sEditorName].editor.XMLProcessor().ActiveTag();
	if((typeof objXmlTag != "undefined") && (objXmlTag != null) && (true == objXmlTag.IsValid()))
	{
		if ("mycomment" == objXmlTag.getPropertyString("TagName"))
		{
			showCommentDialog(sEditorName);
		}
	}
}

function showCommentDialog(sEditorName)
{
	eWebEditPro.openDialog(sEditorName, eWebEditPro.resolvePath("commentpopup.htm"), "", "",
			"width=650,height=350,resizable,scrollbars,status,titlebar");
}
*/
// XHTML 1.0 Entities
var Ektron_Xml_htmlEntity = 
{
	// A.2.1. Latin-1 characters (xhtml-lat1.ent)
	nbsp:160, iexcl:161, cent:162, pound:163, curren:164, yen:165, brvbar:166, sect:167, uml:168, 
	copy:169, ordf:170, laquo:171, not:172, shy:173, reg:174, macr:175, deg:176, plusmn:177, 
	sup2:178, sup3:179, acute:180, micro:181, para:182, middot:183, cedil:184, sup1:185, ordm:186, 
	raquo:187, frac14:188, frac12:189, frac34:190, iquest:191, 
	Agrave:192, Aacute:193, Acirc:194, Atilde:195, Auml:196, Aring:197, AElig:198, Ccedil:199, 
	Egrave:200, Eacute:201, Ecirc:202, Euml:203, Igrave:204, Iacute:205, Icirc:206, Iuml:207, ETH:208, 
	Ntilde:209, Ograve:210, Oacute:211, Ocirc:212, Otilde:213, Ouml:214, times:215, Oslash:216, 
	Ugrave:217, Uacute:218, Ucirc:219, Uuml:220, Yacute:221, THORN:222, szlig:223, 
	agrave:224, aacute:225, acirc:226, atilde:227, auml:228, aring:229, aelig:230, ccedil:231, 
	egrave:232, eacute:233, ecirc:234, euml:235, igrave:236, iacute:237, icirc:238, iuml:239, eth:240, 
	ntilde:241, ograve:242, oacute:243, ocirc:244, otilde:245, ouml:246, divide:247, oslash:248, 
	ugrave:249, uacute:250, ucirc:251, uuml:252, yacute:253, thorn:254, yuml:255, 
	// A.2.2. Special characters (xhtml-special.ent)
	OElig:338, oelig:339, Scaron:352, scaron:353, Yuml:376, circ:710, tilde:732, 
	ensp:8194, emsp:8195, thinsp:8201, zwnj:8204, zwj:8205, lrm:8206, rlm:8207, 
	ndash:8211, mdash:8212, lsquo:8216, rsquo:8217, sbquo:8218, ldquo:8220, rdquo:8221, bdquo:8222, 
	dagger:8224, Dagger:8225, permil:8240, lsaquo:8249, rsaquo:8250, euro:8364, 
	// A.2.3. Symbols (xhtml-symbol.ent)
	fnof:402, Alpha:913, Beta:914, Gamma:915, Delta:916, Epsilon:917, Zeta:918, Eta:919, 
	Theta:920, Iota:921, Kappa:922, Lambda:923, Mu:924, Nu:925, Xi:926, Omicron:927, Pi:928, 
	Rho:929, Sigma:931, Tau:932, Upsilon:933, Phi:934, Chi:935, Psi:936, Omega:937, 
	alpha:945, beta:946, gamma:947, delta:948, epsilon:949, zeta:950, eta:951, 
	theta:952, iota:953, kappa:954, lambda:955, mu:956, nu:957, xi:958, omicron:959, pi:960, 
	rho:961, sigmaf:962, sigma:963, tau:964, upsilon:965, phi:966, chi:967, psi:968, omega:969, 
	thetasym:977, upsih:978, piv:982, bull:8226, hellip:8230, prime:8242, Prime:8243, 
	oline:8254, frasl:8260, weierp:8472, image:8465, real:8476, trade:8482, alefsym:8501, 
	larr:8592, uarr:8593, rarr:8594, darr:8595, harr:8596, crarr:8629, 
	lArr:8656, uArr:8657, rArr:8658, dArr:8659, hArr:8660, 
	forall:8704, part:8706, exist:8707, empty:8709, nabla:8711, isin:8712, 
	notin:8713, ni:8715, prod:8719, sum:8721, minus:8722, lowast:8727, radic:8730, prop:8733, 
	infin:8734, ang:8736, and:8743, or:8744, cap:8745, cup:8746, "int":8747, there4:8756, 
	sim:8764, cong:8773, asymp:8776, ne:8800, equiv:8801, le:8804, ge:8805, 
	sub:8834, sup:8835, nsub:8836, sube:8838, supe:8839, oplus:8853, otimes:8855, 
	perp:8869, sdot:8901, lceil:8968, rceil:8969, lfloor:8970, rfloor:8971, 
	lang:9001, rang:9002, loz:9674, spades:9824, clubs:9827, hearts:9829, diams:9830
};
var Ektron_RegExp_Entity_entityName = /&(\w+);/g;

// http://www.w3.org/TR/2006/REC-xml-20060816/#NT-Char
// Char    ::=    #x9 | #xA | #xD | [#x20-#xD7FF] | [#xE000-#xFFFD] | [#x10000-#x10FFFF] 
var Ektron_RegExp_illegalXmlCharacters = /[^\x09\x0A\x0D\x20-\uD7FF\uE000-\uFFFD]/g;

function Ektron_Xml_fixIllegalCharacters(xml)
{
	return xml.replace(Ektron_RegExp_illegalXmlCharacters, "");
}

function Ektron_Xml_fixUndeclaredNamespacePrefixes(xml)
{
	// A common problem is undeclared namespaces, particularly when 
	// MS Word content is pasted that has smart tags.
	// Get list of namespace prefixes
	var strNamespaces = "";
	var prefixes = {};
	function savePrefix($0_match, $1_prefix)
	{
		try
		{
			prefixes[$1_prefix] = true;
		}
		catch (ex)
		{
			// just in case some illegal prefix is found
		}
		return $0_match; // not actually changing anything
	};
	// Match tag name prefixes
	xml.replace(/<(\w+):\w+/g, savePrefix);
	// Match attribute prefixes
	xml.replace(/\s(\w+):\w+=(?=[^<>]*>)/g, savePrefix);
	
	for (var p in prefixes)
	{
		if (p != "xml" && p != "xmlns")
		{
			var re = new RegExp("xmlns:" + p + "=");
			if (!re.test(xml))
			{
				strNamespaces += " xmlns:" + p + "=\"urn:unknown:" + p + "\"";
			}
		}
	}
	if (strNamespaces.length > 0)
	{
		xml = "<root" + strNamespaces + ">" + xml + "</root>";
	}
	return xml;
};

function Ektron_Xml_fixComments(xml)
{
	xml = xml.replace(/<\!--[\-]+/, "<!\x2D\x2D");
	xml = xml.replace(/--[\-]+>/, "\x2D\x2D>");
	xml = xml.replace(/(<\!--)([\w\W]*?)(-->)/g, function($0_match, $1_open, $2_data, $3_close)
	{
		// "--" is not allowed in comments, even if valid JavaScript :-(
		return $1_open + $2_data.replace(/-{2,}/g, function($0_match)
		{
			return $0_match.replace(/-/g, "=");
		}) + $3_close; 
	});
	return xml;
}

function Ektron_Xml_fixUnknownEntityNames(xml)
{
	// A common problem is "&" in URL that should be "&amp;"
	// Likewise, &nbsp; or other HTML entity name is used.
	// Convert & to &amp; unless it is an XML entity or cdata/comment.
	function protectAmp($0_match, $1_open, $2_data, $3_close)
	{
		return $1_open + $2_data.replace(/&/g, "ektTempAmp") + $3_close; 
	};
	xml = xml.replace(Ektron_RegExp_Entity_entityName, function($0_match, $1_name)
	{
		var codePoint = Ektron_Xml_htmlEntity[$1_name];
		if (codePoint)
		{
			return "&#" + codePoint + ";";
		}
		else
		{
			return $0_match;
		}
	});
	xml = xml.replace(/(<\!--)([\w\W]*?)(-->)/g, protectAmp);
	xml = xml.replace(/(<\!\[CDATA\[)([\w\W]*?)(\]\]>)/g, protectAmp);
	xml = xml.replace(/&(?!#|amp;|lt;|gt;|quot;|apos;)/g, "&amp;");
	xml = xml.replace(/ektTempAmp/g, "&");
	return xml;
};

function Ektron_Xml_fixXml(xml)
{
	// Returns XML string with some common problems fixed.
	try
	{
		xml = Ektron_Xml_fixIllegalCharacters(xml);
		xml = Ektron_Xml_fixUndeclaredNamespacePrefixes(xml);
		xml = Ektron_Xml_fixComments(xml);
		xml = Ektron_Xml_fixUnknownEntityNames(xml);
	}
	catch (ex) { }
	return xml;
};

function onDblClickElementHandler(oElement)
{
/*
	Netscape cannot effectively access the ActiveX control's methods unless called from setTimeout().
	However, there would not be any access to the oElement if called from setTimeout().
*/
	if (oElement && "PreXSLTFilterSourceCode" == oElement.id)
	{
		oElement.value = Ektron_Xml_fixXml(oElement.value);
		return; // do not process as DblClickEvent
	}

	var returnValue = true;
	if ("function" == typeof eWebEditProDblClickElement)
	{
		returnValue = eWebEditProDblClickElement(oElement);
	}
	
	if (returnValue != false)
	{
		eWebEditProDblClickElementDispatcher(oElement);
	}
		
	if (typeof eWebEditPro.ondblclickelement != "undefined")
	{
		eWebEditPro.initEvent("ondblclickelement");
		//eWebEditPro.event.type = "dblclickelement"; 
		//eWebEditPro.event.srcName = eWebEditPro.event.srcName;
		eWebEditPro.event.srcElement = oElement;
		eWebEditPro.raiseEvent("ondblclickelement");
	}
}

function eWebEditProDblClickElementDispatcher(oElement)
{
	var sTagName = oElement.tagName + ""; 
	sTagName = sTagName.toUpperCase(); 
	
	var returnValue = true;
	if ("A" == sTagName)
	{
		if ("function" == typeof eWebEditProDblClickHyperlink)
		{
			returnValue = eWebEditProDblClickHyperlink(oElement);
		}
		if (returnValue != false)
		{
			onDblClickHyperlinkHandler(oElement);
		}
	}
	else if ("IMG" == sTagName)
	{
		if ("function" == typeof eWebEditProDblClickImage)
		{
			returnValue = eWebEditProDblClickImage(oElement);
		}
	}
	else if ("TABLE" == sTagName)
	{
		if ("function" == typeof eWebEditProDblClickTable)
		{
			returnValue = eWebEditProDblClickTable(oElement);
		}
	}
}

function onDblClickHyperlinkHandler(oElement)
{
	var sProtocol = oElement.protocol + ""; 
	var sHost = oElement.host + ""; 
	var sUrl = oElement.href + ""; 
	
	if (sUrl)
	{
		if (/\/\#$/.test(sUrl)) // ends in "/#"
		{	
			sUrl = "#";
			return; // not a real URL, just a hyperlink with onclick
		}
		var oWin;
		if ("mailto:" == sProtocol.toLowerCase())
		{
			oWin = window.open(sUrl, "", "width=2,height=2");
			if (null == oWin && eWebEditProMessages.popupBlockedMessage)
			{
				alert(eWebEditProMessages.popupBlockedMessage);
			}
			else if (oWin != null)
			{
				oWin.close();
			}
		}
		else if (sUrl.substring(0,1) == "#")
		{
			alert("Internal hyperlink to '" + sUrl.substring(1) + "'.");
		}
		else
		{
			if (!sHost)
			{
				if (sUrl.substring(0,1) != "/")
				{
					var sPath = window.document.location.pathname + "";
					var iEndOfPath = sPath.lastIndexOf("/");
					if (iEndOfPath > -1)
					{
						sPath = sPath.substring(0, iEndOfPath + 1);
						sUrl = sPath + sUrl;
					}
				}
				sHost = window.document.location.host + "";
				sUrl = sHost + sUrl;
			}
			if (!sProtocol)
			{
				sProtocol = window.document.location.protocol + "";
				sUrl = sProtocol + "//" + sUrl;
			}
			oWin = window.open(sUrl, "", "location,resizable,scrollbars,status");
			if (null == oWin && eWebEditProMessages.popupBlockedMessage)
			{
				alert(eWebEditProMessages.popupBlockedMessage);
			}
		}
	}
}
