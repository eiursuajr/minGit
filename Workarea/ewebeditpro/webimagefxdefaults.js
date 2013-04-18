// Copyright 2000-2002, Ektron, Inc.
// Revision Date: 2002-07-23

/* Modify this file to set your preferred defaults. */

function defaultInstallFilename()
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
	var strLoadPage;
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
	strLoadPage = "intro" + sExt + strLanguageCode + ".htm" + WifxInformationPassingParameters();
    return strLoadPage;
}

function WifxInformationPassingParameters()
{
    var strLoadPage = "?0=0";

	if("undefined" != typeof LicenseKeys)
    {
		if(LicenseKeys.length > 0)
        {
		    strLoadPage += "&licnewep=";
		    strLoadPage += LicenseKeys;
		}
    }
    if("undefined" != typeof WIFXPath)
    {
        if(WIFXPath.length > 0)
        {
            strLoadPage += "&instwifx=";
            strLoadPage += WIFXPath;
			strLoadPage += "&instewep=";
    		strLoadPage += WIFXPath;
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

function WebImageFXDefaults()
{
	this.path = WIFXPath; // from webimagefx.js
	
	//Some security checkers might detect the program name and block this file, so the name is obfuscated.
	var strCIFilename = "ewebeditproclient";
	var strCIe = ".e";
	this.clientInstall = this.path + "clientinstall/" + strCIFilename + strCIe + "xe";
	var strCIm = ".m";
	this.clientMsiInstall = this.path + "clientinstall/" + strCIFilename + strCIm + "si";

	// properties for WebImageFX.parameters.installPopup
	this.installPopupUrl = this.path + "clientinstall/" + defaultInstallFilename(); // parameters.installPopup.url
	this.installPopupWindowName = ""; // parameters.installPopup.windowName
	this.installPopupWindowFeatures = "height=540,width=680,resizable,scrollbars,status"; // parameters.installPopup.windowFeatures
	this.installPopupQuery = ""; // parameters.installPopup.query
	
	// properties for WebImageFX.parameters.popup
	this.popupUrl = this.path + "webimagefxpopup.htm"; // parameters.popup.url
	this.popupWindowName = ""; // parameters.popup.windowName
	this.popupWindowFeatures = "width=720,height=600,scrollbars,status,resizable"; // parameters.popup.windowFeatures
	this.popupQuery = ""; // parameters.popup.query
	
	// properties for WebImageFX.parameters.buttonTag
	// valid types: "inputbutton", "button", "image", "imagelink", "hyperlink", "custom"
	this.popupButtonTagType = "inputbutton"; // parameters.buttonTag.type
	this.popupButtonTagTagAttributes = ""; // parameters.buttonTag.tagAttributes
	/*
	For a custom graphic for "image" or "imagelink", set the imageTag object's properties to IMG attributes.
	this.popupButtonTagImageTag = { src:"myimage.gif", width:40, height:20 }; // parameters.buttonTag.imageTag.src
	
	For "custom", set start and end. 
	The string 'WebImageFX.edit("the-element-name")' will be inserted between start and end.
	this.popupButtonTagStart = '...'; // parameters.buttonTag.start
	this.popupButtonTagEnd = '...'; // parameters.buttonTag.end
	*/
	
	this.maxContentSize = 65000;	// maximum number of characters of HTML content that can be saved.
	
	this.embedAttributes = "";
	this.objectAttributes = "";
	this.textareaAttributes = "";
	
	this.license = WifxLicenseKeys; // from webimagefx.js
	if ("" == this.license)
	{
		this.license = "Invalid License";
	}
	
	this.srcPath = this.path;
	this.locale = this.path + "";
	this.config = this.path + "ImageEditConfig.xml";
	//this.baseURL = "";
	//this.imgEditPath = WIFXPath;
		
	// Arguments must be all lowercase.
	this.ondblclickelement/*(oelement)*/ = "";//onDblClickElementHandler(oelement)";
	this.onexeccommand/*(strcmdname, strtextdata, ldata)*/ = "onExecCommandHandler(strcmdname, strtextdata, ldata)";
	this.onfocus = "";
	this.onblur = "";
	this.EditComplete = "onEditCompleteHandler(strloadname, strsavename)";
	this.EditCommandComplete = "onEditCommandCompleteHandler(strcmdname)";
	this.EditCommandStart = "onEditCommandStartHandler(strcmdname)";
	this.ImageError = "onImageErrorHandler(strerrorid, strerrdesc, strimagename, strcmdname)";
	this.LoadingImage = "onLoadingImageHandler(strimagename, strsavefilename, stroldimagename, strsavename)";
	this.SavingImage = "onSavingImageHandler(strimagename, strsavefilename)";
	this.UpdateImage = "onUpdateImageHandler(strimagename, strsavefilename)";
	this.LicenseValidity = "onLicenseValidityHandler(strisvalid, strlicense)";
	
	this.editorGetMethod = "PublishHtml"; 
}

var WebImageFXDefaults = new WebImageFXDefaults;
