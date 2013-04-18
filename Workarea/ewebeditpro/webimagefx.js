var WIFXPath = "/ewep5/";
/*
	Specify license key(s) in file webimagefxlicensekey.txt.
*/

// Copyright 2000-2003, Ektron, Inc.
// Revision Date: 2003-07-10

function isVBScriptSupported()
{
	var isWindows = (window.navigator.platform.indexOf("Win") > -1);
	var isIE = false;
	var ua = window.navigator.userAgent;
	var pOpera = ua.indexOf("Opera");
	if (pOpera == -1)
	{
		var pIE = ua.indexOf("MSIE ");
		isIE = (pIE > -1);
	}
	return (isWindows && isIE);
}

function defaultMsgsFilename()
{
	var strLanguageCode = "";

	if (navigator.language) // for Netscape
	{
    	strLanguageCode = navigator.language;
	}
 	if (navigator.userLanguage) // for IE
	{
    	strLanguageCode = navigator.userLanguage;
	}
	var strTranslatedLangCodes = "zh-tw";
	if (strTranslatedLangCodes.indexOf(strLanguageCode) == -1)
	{
	    strLanguageCode = strLanguageCode.substring(0,2);
		var strTranslatedLanguages = "ar,da,de,es,fr,he,it,ja,ko,nl,pt,ru,sv,zh";
		if (strTranslatedLanguages.indexOf(strLanguageCode) == -1)
		{
			// not a translated language
			strLanguageCode = ""; // use default (English)
		}
	}
	return "webimagefxmessages" + strLanguageCode + ".js";
}


if (typeof(WebImageFXIncludes) == "undefined")
{
	// Include license key(s) that are in file webimagefxlicensekey.txt
	document.writeln('<script type="text/javascript" language="JavaScript1.2" src="' + 
						WIFXPath + 'webimagefxlicensekey.txt"></script>');
	// The above two license key values are concatinated in RegisterLicense().
	
	if (isVBScriptSupported())
	{
		document.writeln('<script type="text/vbscript" language="VBScript" src="' + 
						WIFXPath + 'wifx.vbs"></script>');
	}

	// Assign default messages file if not already defined.
	if ("undefined" == typeof WebImageFXMsgsFilename || !WebImageFXMsgsFilename)
	{
		WebImageFXMsgsFilename = defaultMsgsFilename();
	}
	
	var WebImageFXIncludes = [	
		"webimagefxevents.js",
		"webimagefxdefaults.js",
		WebImageFXMsgsFilename,
		"wifx.js"]; 
	
	for (var i = 0; i < WebImageFXIncludes.length; i++)
	{
		document.writeln('<script type="text/javascript" language="JavaScript1.2" src="' + 
						WIFXPath + WebImageFXIncludes[i] + '"></script>');
	}
}
