// Copyright 2000-2002, Ektron, Inc.
// Revision Date: 2002-02-12

/*
	Best used with IE.
	
	Adds "lang" and "charset" URL param to file with editor.
	Example,
		/ewebeditpro/testlang.htm?lang=fr&charset=windows-1252
		
	The "charset" param is supported ONLY WITH IE.

	Include this file PRIOR to including ewebeditpro.js
	
	<script language="JavaScript1.2" src="ewebeditprolang.js"></script>
	<script language="JavaScript1.2" src="ewebeditpro.js"></script>
	
	BEFORE creating editor, call in JavaScript:
	
	eWebEditProSetLanguage();
	
	To see the eWebEditProMessages object in a textarea, call in JavaScript:
	
	eWebEditProShowMessages(false, 20, 120); // human readable
	eWebEditProShowMessages(true, 20, 120); // UTF-8 readable
*/

function eWebEditProUtil_parseQuery()
{
	var objQuery = new Object();
	var strQuery = top.location.search.substring(1);
	// escape() encodes space as "%20".
	// If space is encoded as "+", then use the following line
	// in your customized function.
	// strQuery = strQuery.replace(/\+/g, " ");
	var aryQuery = strQuery.split("&");
	var pair = [];
	for (var i = 0; i < aryQuery.length; i++)
	{
		pair = aryQuery[i].split("=");
		if (2 == pair.length)
		{
			objQuery[unescape(pair[0])] = unescape(pair[1]);
		}
	}
	return objQuery;
}
var objQuery = eWebEditProUtil_parseQuery();

// private
function toHexString(numValue, minLength)
// Convert numValue (Number object) to a hexidecimal representation string of 
// minimum length (minLength) padded with leading zeros as necessary.
{
	if (typeof numValue != "number")
	{
		return "Bad numValue, type: "+ typeof numValue;
	}
	if (typeof minLength != "number")
	{
		return "Bad minLength, type: " + typeof minLength;
	}
	var strValue = numValue.toString(16); // convert to hex string
	// Ensure min len
	while (strValue.length < minLength)
	{
		strValue = "0" + strValue;
	}
	return strValue; 
}

function escapeUnicode(strText)
// Escape strText to be ASCII, safe for native encoding or UTF-8.
{
	var strConverted = "";
	var lenText = strText.length;
 	var numCharCode = new Number(0);
	for (var i = 0; i < lenText; i++)
	{
		numCharCode = strText.charCodeAt(i);
 		if (numCharCode <= 0x7f)
 		{
			// ASCII, no conversion
  			strConverted += strText.charAt(i);
 		}
		else if (numCharCode <= 0xff)
		{
			// single byte Latin1
		   	strConverted += "\\x" + toHexString(numCharCode, 2); 
		}
 		else
 		{
		// double byte Unicode
  			strConverted += "\\u" + toHexString(numCharCode, 4); 
 		}
	} 
	return strConverted;
}

// private
function ewepLocaleId(strLang)
// Returns locale id given language code, eg "en"
{
	var objLangLocales = 
	{ 
		ar: "0401",
		da: "0406",
		de: "0407",
		el: "0408",
		en: "0409",
		es: "040a",
		fi: "040b",
		fr: "040c",
		he: "040d",
		hi: "0439",
		it: "0410",
		ja: "0411",
		ko: "0412",
		nl: "0413",
		pl: "0415",
		pt: "0816",
		ru: "0419",
		sv: "041d",
		zh: "0804" 
	}; 
	objLangLocales["zh-tw"] = "0404";
	
	var strLocaleId = "0000"; // default
	if (strLang && objLangLocales[strLang])
	{
		strLocaleId = objLangLocales[strLang];
	}

	return strLocaleId;
}

function eWebEditProSetLanguage(strLang, strCharset)
// Set language of eWebEditPro
// optional strLang language code, eg, "en"
{
	if (!strLang && objQuery["lang"])
	{
		strLang = objQuery["lang"];
	}
	if (strLang)
	{
		var strLocale = "locale" + ewepLocaleId(strLang) + "b.xml";
		eWebEditPro.parameters.locale = eWebEditPro.parameters.path + strLocale;
		var strIntro = "intro" + strLang + ".htm";
		eWebEditPro.parameters.installPopup.url = eWebEditPro.parameters.path + "clientinstall/" + strIntro + InformationPassingParameters();
	}

	if (!strCharset && objQuery["charset"])
	{
		strCharset = objQuery["charset"];
	}
	eWebEditPro.parameters.charset = strCharset;
}

function eWebEditProShowMessages(bEscaped, rows, cols, attrs)
// Shows value of eWebEditProMessages object in textarea field
// either as readable text or escaped Unicode chars suitable for UTF-8 encoding.
// bEscaped - true = shows text using escape \xNN and \uNNNN
// optional rows - for textarea
// optional cols - for textarea
// optional attrs - additional attributes
{
	if (rows)
	{
		rows = 'rows="' + rows + '" ';
	}
	if (cols)
	{
		cols = 'cols="' + cols + '" ';
	}


	var strText = "var eWebEditProMessages =\n{\n";
	var strValue = "";
	var strDelimiter = "\t";
	var reClientInstall = new RegExp(eWebEditProDefaults.clientInstall);

	for (var strName in eWebEditProMessages)
	{
		strValue = eWebEditProMessages[strName];
		
		// escape chars
		strValue = strValue.replace(/\\/g, "\\\\");
		strValue = strValue.replace(/\n/g, "\\n");
		strValue = strValue.replace(/\r/g, "\\r");
		strValue = strValue.replace(/\t/g, "\\t");
		// determine how to quote
		if (strValue.indexOf("\"") >= 0) 
		{
			if (strValue.indexOf("'") >= 0)
			{
				// both single and double quotes
				// escape single quotes
				strValue = strValue.replace(/'/g, "\\'");
			}
			// single quote because message contains double quotes
			strQuot = "'"; 
		}
		else
		{
			// double quote
			strQuot = "\""; 
		}

		// restore clientInstall variable
		strValue = strValue.replace(reClientInstall, strQuot + " + eWebEditProDefaults.clientInstall + " + strQuot);
		
		if (bEscaped)
		{
			strValue = escapeUnicode(strValue);
		}
		strText += strDelimiter + strName + ":\t\t" + strQuot + strValue + strQuot + "\n";
		strDelimiter = ",\t";
	}
	strText += "}";

	document.writeln('<textarea wrap="off" ' + rows + cols + attrs + '>' + strText + '</textarea>');
}

// Define eWebEditProMessages object incase eWebEditProMsgsFilename does not exist.
var eWebEditProMessages = new Object();

if (objQuery["charset"])
{
	document.charset = objQuery["charset"]; // effective with IE only
}

if (objQuery["lang"])
{
	// Force the language of the messages file. 
	// No error checking. If the file is missing a JavaScript error occurs.
	eWebEditProMsgsFilename = "ewebeditpromessages" + objQuery["lang"] + ".js";
	
	// Define property to signal this is the dummy object.
	eWebEditProMessages.eWebEditProMessages = "Could not find file: " + eWebEditProMsgsFilename;
}

