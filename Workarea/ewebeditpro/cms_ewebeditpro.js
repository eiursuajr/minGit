// Copyright 2000-2002, Ektron, Inc.
// Revision Date: 2002-06-12

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
	return "ewebeditpromessages" + strLanguageCode + ".js";
}

function RegisterLicense(sLicense)
{
	// If we have a secondary license key, concatenate it here.
	if (typeof(LicenseKeys) != "undefined")
	{
		if(sLicense.length > 0)
		{
			LicenseKeys += ", ";
			LicenseKeys += sLicense;
		}
	}
}

if (typeof(eWebEditProIncludes) == "undefined")
{
	// Include license key(s) that are in file ewebeditprolicensekey.txt.
	//document.writeln('<script type="text/javascript" language="JavaScript1.2" src="' + 
	//					eWebEditProPath + 'ewebeditprolicensekey.txt"></script>');
	// and webimagefxlicensekey.txt
	//document.writeln('<script type="text/javascript" language="JavaScript1.2" src="' + 
	//					WebImageFXPath + 'webimagefxlicensekey.txt"></script>');
	// The above two license key values are concatinated in RegisterLicense().
	
	if (isVBScriptSupported())
	{
		document.writeln('<script type="text/vbscript" language="VBScript" src="' + 
						eWebEditProPath + 'ewep.vbs"></script>');
	}

	// Assign default messages file if not already defined.
	if ("undefined" == typeof eWebEditProMsgsFilename || !eWebEditProMsgsFilename)
	{
		eWebEditProMsgsFilename = defaultMsgsFilename();
	}
	
	var eWebEditProIncludes = [	
		"ewebeditproevents.js",
		"ewebeditprodefaults.js",
		"cms_ewebeditpromedia.js",
		eWebEditProMsgsFilename,
		"ewep.js",
		"customevents.js"
		];
	
	for (var i = 0; i < eWebEditProIncludes.length; i++)
	{
		document.writeln('<script type="text/javascript" language="JavaScript1.2" src="' + 
						eWebEditProPath + eWebEditProIncludes[i] + '"></script>');
	}
}

