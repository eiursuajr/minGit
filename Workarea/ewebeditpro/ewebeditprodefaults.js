// Copyright 2000-2005, Ektron, Inc.
// Revision Date: 2005-11-29

/* Modify this file to set your preferred defaults. */

function defaultInstallFilename(strLanguageCode)
{
    var strLoadPage = "";
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
	var ua = window.navigator.userAgent;
	var isWinXPSP2 = (ua.indexOf("SV1") > -1);
	var isWinVista = (ua.indexOf("Windows NT 6.") > -1);
	var sExt = "";
	if (isWinXPSP2)
	{
		sExt = "xpsp2";
	}
	if (isWinVista)
	{
		sExt = "msi";
	}
	strLoadPage = "intro" + sExt + strLanguageCode + ".htm" + InformationPassingParameters();
	    
    return strLoadPage;
}

function InformationPassingParameters()
{
    var strLoadPage = "";
    
    strLoadPage += "?instewep=";
    strLoadPage += eWebEditProPath;
    
    strLoadPage += "&licnewep=";
    strLoadPage += LicenseKeys;
    
    if("undefined" != typeof WebImageFXPath)
    {
        if(WebImageFXPath.length > 0)
        {
            strLoadPage += "&instwifx=";
            strLoadPage += WebImageFXPath;
        }
    }
    
    if("undefined" != typeof WifxLicenseKeys)
    {
        if(WifxLicenseKeys.length > 0)
        {
            strLoadPage += "&licnwifx=";
            strLoadPage += WifxLicenseKeys;
        }
    }
    
    return(strLoadPage);
}

function eWebEditProDefaults()
{
	this.path = eWebEditProPath; // from ewebeditpro.js
	
	// Some security checkers might detect the program name and block this file, so the name is obfuscated.
	var strCIFilename = "ewebeditproclient";
	var strCIe = ".e";
	this.clientInstall = this.path + "clientinstall/" + strCIFilename + strCIe + "xe";
	var strCIm = ".m";
	this.clientMsiInstall = this.path + "clientinstall/" + strCIFilename + strCIm + "si";

	var ua = window.navigator.userAgent.toLowerCase();
	if(ua.indexOf("windows nt 6.") > -1)  // Vista
	{
		this.clientMsiInstall = this.clientInstall;
	}
	
	// properties for eWebEditPro.parameters.installPopup
	this.installPopupUrl = this.path + "clientinstall/" + defaultInstallFilename(); // parameters.installPopup.url
	this.installPopupWindowName = ""; // parameters.installPopup.windowName
	this.installPopupWindowFeatures = "height=540,width=680,resizable,scrollbars,status"; // parameters.installPopup.windowFeatures
	this.installPopupQuery = ""; // parameters.installPopup.query
	
	// properties for eWebEditPro.parameters.popup
	this.popupUrl = this.path + "ewebeditpropopup.htm"; // parameters.popup.url
	this.popupWindowName = ""; // parameters.popup.windowName
	this.popupWindowFeatures = "width=720,height=600,scrollbars,status,resizable"; // parameters.popup.windowFeatures
	this.popupQuery = ""; // parameters.popup.query
	
	// properties for eWebEditPro.parameters.buttonTag
	// valid types: "inputbutton", "button", "image", "imagelink", "hyperlink", "custom"
	this.popupButtonTagType = "inputbutton"; // parameters.buttonTag.type
	this.popupButtonTagTagAttributes = ""; // parameters.buttonTag.tagAttributes
	/*
	For a custom graphic for "image" or "imagelink", set the imageTag object's properties to IMG attributes.
	this.popupButtonTagImageTag = { src:"myimage.gif", width:40, height:20 }; // parameters.buttonTag.imageTag.src
	
	For "custom", set start and end. 
	The string 'eWebEditPro.edit("the-element-name")' will be inserted between start and end.
	this.popupButtonTagStart = '...'; // parameters.buttonTag.start
	this.popupButtonTagEnd = '...'; // parameters.buttonTag.end
	*/
	
	//IMPORTANT: if the application is set to be used in Netscape, the maxContentSize need to be set to 65000
	//this.maxContentSize = 65000; 
	this.maxContentSize = 0;	// maximum number of characters of HTML content that can be saved. (0 = unlimited)
	
	this.embedAttributes = "";
	this.objectAttributes = "";
	this.textareaAttributes = "";

	this.license = LicenseKeys; // from ewebeditpro.js
	
	this.srcPath = this.path;
	this.locale = this.path + "";
	this.config = this.path + "config.xml";
	this.xmlInfo = "";
	this.baseURL = "";
	this.charset = "";
	this.title = "";
	this.styleSheet = "";
	this.bodyStyle = "";
	this.hideAboutButton = "";
	this.wddx = "";
	this.readOnly = "";
	var sWifxPath = "";
	if("undefined" != typeof WebImageFXPath)
    {
        if(WebImageFXPath.length > 0)
        {
			sWifxPath = WebImageFXPath;
		}
	}
	this.imgEditPath = sWifxPath;
	this.name = "";
		
	// Arguments must be all lowercase.
	this.ondblclickelement/*(oelement)*/ = "onDblClickElementHandler(oelement)";
	this.onexeccommand/*(strcmdname, strtextdata, ldata)*/ = "onExecCommandHandler(strcmdname, strtextdata, ldata)";
	this.onfocus = "";
	this.onblur = "";
	
	this.editorGetMethod = "getBodyHTML"; // "getBodyHTML" or "getDocument"
}

var eWebEditProDefaults = new eWebEditProDefaults;
