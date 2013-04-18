var eWebEditProPath = "/workarea/ewebeditpro/";
var WIFXPath= "/workarea/ewebeditpro/";
var WebImageFXPath = "/workarea/ewebeditpro/";
/*
	Specify license key(s) in file ewebeditprolicensekey.txt.
*/

// Copyright 2000-2004, Ektron, Inc.
// Revision Date: 2004-03-08

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

function defaultMsgsFilename(strLanguageCode)
{
	if (!strLanguageCode)
	{
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

// OverrideLicensesWithGivenParamValues() and OverridePathsWithGivenParamValues
// are called with the eval created at the end of this file.
// This is an override for specifying the above values as 
// command line parameters.
// See clientinstall/parseinstallparams.js for how to set these values.
function OverridePathsWithGivenParamValues()
{
	if("undefined" != typeof given_eWebEditProPath)
    {
        if(given_eWebEditProPath.length > 0)
        {
            eWebEditProPath = given_eWebEditProPath;
			
        }
    }

    if("undefined" != typeof given_WebImageFXPath)
    {
        if(given_WebImageFXPath.length > 0)
        {
            WebImageFXPath = given_WebImageFXPath;
        }
    }
}

if (typeof(eWebEditProIncludes) == "undefined")
{
    // This call allows for an overide of any pre-defined values.
	// It is needed before everything else so that we know where to find things.
    // See clientinstall/parseinstallparams.js for how to set these values.
	// See also OverrideLicensesWithGivenParamValues().
	document.writeln('<script language="JavaScript1.2">OverridePathsWithGivenParamValues();</script>');
	
	// Include license key(s) that are in file ewebeditprolicensekey.txt.
	if("undefined" == typeof LicenseKeys)
	{
		document.writeln('<script type="text/javascript" language="JavaScript1.2" src="' + 
						eWebEditProPath + 'ewebeditprolicensekey.txt"></script>');
	}
	// and webimagefxlicensekey.txt
	if("undefined" == typeof WifxLicenseKeys)
	{
		document.writeln('<script type="text/javascript" language="JavaScript1.2" src="' + 
						WebImageFXPath + 'webimagefxlicensekey.txt"></script>');
	}
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
		"ewebeditpromedia.js",
		eWebEditProMsgsFilename,
		"ewep.js",
		"customevents.js"];
	
	for (var i = 0; i < eWebEditProIncludes.length; i++)
	{
		document.writeln('<script type="text/javascript" language="JavaScript1.2" src="' + 
						eWebEditProPath + eWebEditProIncludes[i] + '"></script>');
	}
}
