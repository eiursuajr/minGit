/*
 *
 * Ephox EditLive! JavaScript Library
 * Copyright (c) 1999-2003 Ephox Corp. All rights reserved.
 * This software is provided "AS IS," without a warranty of any kind.
 *
 */

var eljUseWebDAV = false;
var eljUseMathML = false;

/** The designer class provides a simple API for setting parameters on the designer and
 * instantiating it.
 */
function EditLiveJava(name, width, height) {

	EditLiveCommonStatic_detectBrowser();
	
	this.paramNames = new Array();
	this.paramValues = new Array();
	
	this.width = width;
	this.height = height;
	this.name = name;
	this.borderStyle = "";
	this.bAutoSubmit = true;
	
	this.setDownloadDirectory = EditLiveCommon_setDownloadDirectory;
	this.setConfigurationFile = EditLiveCommon_setXMLURL;
	this.setXMLURL = EditLiveCommon_setXMLURL;
	this.setDebugLevel = EditLiveCommon_setDebugLevel;
	this.setLogger = EditLiveCommon_setLogger;
	this.setConfigurationText = EditLiveCommon_setXML;
	this.setXML	= EditLiveCommon_setXML;
	this.setMinimumJREVersion =  EditLiveCommon_setMinimumJREVersion;
	this.setJREDownloadURL = EditLiveCommon_setJREDownloadURL;
	this.setShowSystemRequirementsError = EditLiveCommon_setShowSystemRequirementsError;
	this.setCookie = EditLiveCommon_setCookie;
	this.setLocalDeployment = EditLiveCommon_setLocalDeployment;
	this.InsertHTMLAtCursor = EditLiveCommon_InsertHTMLAtCursor;
	this.InsertHyperlinkAtCursor = EditLiveCommon_InsertHyperlinkAtCursor;
	this.setAutoSubmit = EditLiveCommon_setAutoSubmit;
	this.UploadFiles = EditLiveCommon_UploadFiles;
	this.UploadImages = EditLiveCommon_UploadFiles;
	this.setDownloadingMessage = EditLiveCommon_setDownloadingMessage;
	this.setLocale = EditLiveCommon_setLocale;
	this.setBorderStyle = EditLiveCommon_setBorderStyle;
	this.show = EditLiveCommon_show;
	this.setShowButtonText = EditLiveCommon_setShowButtonText;
	this.setShowButtonIconURL = EditLiveCommon_setShowButtonIconURL;
	this.setHideButtonText = EditLiveCommon_setHideButtonText;
	this.setHideButtonIconURL = EditLiveCommon_setHideButtonIconURL;
	this.showAsButton = EditLiveCommon_showAsButton;
	
	this.setDocument = EditLiveJava_setDocument;
	this.setBody = EditLiveJava_setBody;
	this.setStyles = EditLiveJava_setStyles;
	this.setXSD = EditLiveJava_setXSD;
	this.addXSDAsString = EditLiveJava_addXSDAsString;
	this.setReturnBodyOnly = EditLiveJava_setReturnBodyOnly;
	this.GetDocument = EditLiveJava_GetDocument;
	this.GetBody = EditLiveJava_GetBody;
	this.GetSelectedText = EditLiveJava_GetSelectedText;
	this.SetDocument = EditLiveJava_setDocument;
	this.SetBody = EditLiveJava_setBody;
	this.GetStyles = EditLiveJava_GetStyles;
	this.GetWordCount = EditLiveJava_GetWordCount;
	this.GetCharCount = EditLiveJava_GetCharCount;
	this.IsDirty = EditLiveJava_IsDirty;
	this.SetProperties = EditLiveJava_SetProperties;
	this.PostDocument = EditLiveJava_PostDocument;
	this.addView = EditLiveJava_addView;
	this.addViewWithString = EditLiveJava_addViewWithString
	this.setUseTextarea = EditLiveJava_setUseTextarea;
	this.setTextareaRows = EditLiveJava_setTextareaRows;
	this.setTextareaCols = EditLiveJava_setTextareaCols;
	this.setPreload = EditLiveJava_setPreload;
	this.setOnInitComplete = EditLiveJava_setPreload;
	this.setHead = EditLiveJava_setHead;
	this.setBaseURL = EditLiveJava_setBaseURL;
	this.GetCurrentDocumentURL = EditLiveJava_GetCurrentDocumentURL;
	this.setOutputCharset = EditLiveJava_setOutputCharset;
	this.setCommentTemplate = EditLiveJava_setCommentTemplate;
	this.addJar = EditLiveJava_addJar;
	this.setPreserveInputStructure = EditLiveJava_setPreserveInputStructure;
	this.setHttpLayerManager = EditLiveJava_setHttpLayerManager;
	
	// WebDAV Properties
	this.setUseWebDAV = EditLiveJava_setUseWebDAV;

	// MathML Properties
	this.setUseMathML = EditLiveJava_setUseMathML;

	this.started = false;
	
	this.getAppletHTML = EditLiveJava_getAppletHTML;
	this.getHiddenFields = EditLiveJava_getHiddenFields;
	
	this.started = false;
	this.editXML = false;
	this.views = new Array();
	this.xsds = new Array();
	this.extraJars = new Array();
	this.classNames = "";
}

function EditLiveJava_setCommentTemplate(val) {
	if (this.started) {
		return false;
	}
	var index = this.paramNames.length
	this.paramNames[index] = "commentTemplate";
	this.paramValues[index] = val;
	return true;
}

function EditLiveJava_addJar(jarUrl, className) {
	this.extraJars[this.extraJars.length] = jarUrl;
	this.classNames += className + " ";
}

function EditLiveJava_getAppletHTML() {
	if (!this.cookie) {
		this.setCookie(document.cookie);
	}
	
	// Determine the applet class to use.
	var appletClass;
	if (IsMac) {
		appletClass = "com.ephox.editlive.osx.EditLiveJava";
	} else if (IsLinux) {
		appletClass = "com.ephox.editlive.linux.EditLiveJava";
	} else if (IsSolaris) {
		appletClass = "com.ephox.editlive.solaris.EditLiveJava";
	} else {
		appletClass = "com.ephox.editlive.win.EditLiveJava";
	}
	
	// Determine the classpath.
	var needXML = false;
	jarList = this.downloadDirectory + "editlivejava.jar";
	if (eljUseWebDAV) {
		needXML = true;
	}
	if (eljUseMathML) {
		jarList += "," + this.downloadDirectory + "WebEQEphox.jar";
	}
	if ((IsMac && IsMSIE) || sMinimumJREVersion == "1.3.1") {
		jarList += "," + this.downloadDirectory + "xml-apis.jar";
	}
	if (this.editXML) {
		needXML = false;
		if ((IsMac && IsMSIE) || sMinimumJREVersion == "1.3.1") {
			jarList += "," + this.downloadDirectory + "crimson.jar";
		}
	} else if (needXML) {
		if ((IsMac && IsMSIE) || sMinimumJREVersion == "1.3.1") {
			jarList += "," + this.downloadDirectory + "crimson.jar";
		}
	}
	
	var urlCounter;
	for (urlCounter = 0; urlCounter < this.extraJars.length; urlCounter++) {
		jarList += "," + this.extraJars[urlCounter];
	}
	
	var paramLength = this.paramNames.length;
	this.paramNames[paramLength] = "BeanListenerClasses";
	this.paramValues[paramLength] = this.classNames;
	
	classpath = jarList;
	var appletName = this.name + "_elj";
	hiddenName = this.name;

	if (this.editXML) {
		var i = 0;
		var nameParam = "";
		var valueParam = "";
		var isUrlParam = "";
		for (i = 0; i < this.views.length; i++) {
			var view = this.views[i];
			nameParam += escape(view[0]) + ":";
			valueParam += escape(view[1]) + ":";
			isUrlParam += view[2] + ":";
		}
		var propLength = this.paramNames.length;
		this.paramNames[propLength] = "viewNames";
		this.paramValues[propLength] = nameParam;
		propLength++;
		this.paramNames[propLength] = "viewValues";
		this.paramValues[propLength] = valueParam;
		propLength++;
		this.paramNames[propLength] = "viewIsURLs";
		this.paramValues[propLength] = isUrlParam;
		propLength++;
		var xsdParam = "";
		for (i = 0; i < this.xsds.length; i++) {
			xsdParam += this.xsds[i];
		}
		this.paramNames[propLength] = "XSDString";
		this.paramValues[propLength] = xsdParam;
	}
	
	var appletTag = EditLiveCommonStatic_generateAppletTag(this.width, this.height, appletName, classpath,
		appletClass, this.paramNames, this.paramValues);
	
	var result = EditLiveCommonStatic_getEditorLayout(appletTag, this.borderStyle, this.width, this.height);
	return result;
}

function EditLiveJava_getHiddenFields() {
	var fields = new Array();
	fields[0] = this.name;
	fields[1] = this.name + "_styles";
	return fields;
}

function EditLiveJava_setUseWebDAV(bValue){
	if (this.started == true){
		return false;
	}
	dalert("Use WebDAV: " + bValue);
	eljUseWebDAV = bValue;
}

function EditLiveJava_setUseMathML(bValue) {
	if (this.started == true) {
		return false;
	}
	dalert("Use MathML: " + bValue);
	eljUseMathML = bValue;
}

function EditLiveJava_setOutputCharset(charset) {
	if (this.started == true) {
		return false;
	}
	dalert("Output Charset: " + charset);
	var index = this.paramNames.length;
	this.paramNames[index] = "outputCharset";
	this.paramValues[index] = charset;
}

function EditLiveJava_setDocument(src){
	if(this.started == true){
		EditLiveCommonStatic_CustomAction("setdocument", src, this.name + "_elj");
	} else {
		dalert("Document    " + src);
		var index = this.paramNames.length
		this.paramNames[index] = "Document";
		this.paramValues[index] = src;
	}
} //setDocument

function EditLiveJava_setBody(src){
	if(this.started == true){
		EditLiveCommonStatic_CustomAction("setbody", src, this.name + "_elj");
	} else {
		dalert("Body    " + src);
		var index = this.paramNames.length
		this.paramNames[index] = "Body";
		this.paramValues[index] = src;
	}
} //setBody

function EditLiveJava_setStyles(src){
	if(this.started == true){
		return false;
	}//end if
	dalert("Styles   "+src);
	var index = this.paramNames.length
	this.paramNames[index] = "Styles";
	this.paramValues[index] = src;
}//setStyles


function EditLiveJava_setReturnBodyOnly(bValue){
	if(this.started == true){
		return false;
	} //end if
	dalert("ReturnBodyOnly    " + bValue);
	var index = this.paramNames.length
	this.paramNames[index] = "setReturnBodyOnly";
	this.paramValues[index] = bValue;
} //setReturnBodyOnly



function EditLiveJava_setPreserveInputStructure(bValue) {
	if(this.started == true){
		return false;
	}
	dalert("PreserveInputStructure    " + bValue);
	var index = this.paramNames.length
	this.paramNames[index] = "setPreserveInputStructure";
	this.paramValues[index] = bValue;
}

function EditLiveJava_setXSD(val) {
	if (this.started == true) {
		return false;
	}
	var index = this.paramNames.length;
	this.paramNames[index] = "XSD";
	this.paramValues[index] = val;
}

function EditLiveJava_addXSDAsString(val) {
	if (this.started == true) {
		return false;
	}
	this.xsds[this.xsds.length] = val;
}

function EditLiveJava_GetDocument(){
	if(arguments.length == 1) {
		EditLiveCommonStatic_CustomAction("getdocument", arguments[0], this.name + "_elj");
	} else if(arguments.length == 2) {
		var sArgs = arguments[0];
		EditLiveCommonStatic_CustomAction("getdocument", arguments[0] + "##ephox##" + arguments[1], this.name + "_elj");
	}
} //EditLiveJava_GetDocument

function EditLiveJava_GetBody(){
	if(arguments.length == 1) {
		EditLiveCommonStatic_CustomAction("getbody", arguments[0], this.name + "_elj");
	} else if(arguments.length == 2) {
		var sArgs = arguments[0];
		EditLiveCommonStatic_CustomAction("getbody", arguments[0] + "##ephox##" + arguments[1], this.name + "_elj");
	}
} //EditLiveJava_GetBody

function EditLiveJava_GetSelectedText(){
	EditLiveCommonStatic_CustomAction("getselectedtext", arguments[0], this.name + "_elj");
} //EditLiveJava_GetSelectedText

function EditLiveJava_GetCurrentDocumentURL(sActionValue){
	EditLiveCommonStatic_CustomAction("getcurrentfile", sActionValue, this.name + "_elj");
} //EditLiveJava_GetCurrentDocumentURL

function EditLiveJava_GetStyles(sActionValue){
	EditLiveCommonStatic_CustomAction("getstyles", sActionValue, this.name + "_elj");
} //EditLiveJava_GetStyles

function EditLiveJava_GetWordCount(sActionValue){
	EditLiveCommonStatic_CustomAction("getwordcount", sActionValue, this.name + "_elj");
} //EditLiveJava_GetStyles

function EditLiveJava_GetCharCount(sActionValue){
	EditLiveCommonStatic_CustomAction("getcharcount", sActionValue, this.name + "_elj");
} //EditLiveJava_GetStyles

function EditLiveJava_SetProperties(sActionValue) {
	EditLiveCommonStatic_CustomAction("setproperties", sActionValue, this.name + "_elj");
}

function EditLiveJava_setHttpLayerManager(value) {

	if(this.started == true){
		EditLiveCommonStatic_CustomAction("sethttplayer", sActionValue, this.name + "_elj");
	} else {
        
		dalert("HttpLayer    " + value);
		var index = this.paramNames.length
		this.paramNames[index] = "HttpLayer";
		this.paramValues[index] = value;
	}
}

function EditLiveJava_IsDirty(sActionValue){
	EditLiveCommonStatic_CustomAction("isdirty", sActionValue, this.name + "_elj");
} //EditLiveJava_IsDirty

function EditLiveJava_PostDocument() {
	var val = "";
	var argPosition;
	for (argPosition = 0; argPosition < arguments.length; argPosition++) {
		val += arguments[argPosition];
		if (argPosition + 1 < arguments.length) {
			val += "##ephox##";
		}
	}
	EditLiveCommonStatic_CustomAction("postdocument", val, this.name + "_elj");
}

function EditLiveJava_addViewWithString(name, value) {
	if (this.started == true) {
		return false;
	}
	dalert("Add view: " + name + " - " + value);
	var view = new Array();
	view[0] = name;
	view[1] = value;
	view[2] = "false";
	this.views[this.views.length] = view;
}

function EditLiveJava_addView(name, value) {
	if (this.started == true) {
		return false;
	}
	dalert("Add view: " + name + " - " + value);
	var view = new Array();
	view[0] = name;
	view[1] = value;
	view[2] = "true";
	this.views[this.views.length] = view;
}

function EditLiveJava_Print(html) {
	var newWin;
	newWin = window.open("", "", "toolbar=no"); //, "toolbar=no,location=no,menu=no")
	newWin.document.open();
	newWin.document.write('<html><head><title>Print Preview</title></head><frameset rows="60, *"><frame name="Print"><frame name="content"></frameset></html>');
	newWin.document.close();
	newWin['Print'].document.open();
	newWin['Print'].document.write('<html><body><form><input type="button" value="Print" onclick="parent[\'content\'].focus();parent[\'content\'].print();"><input type="button" value="Close" onclick="parent.close();"></form></body></html>');
	newWin['Print'].document.close();
	newWin['content'].document.open();
	newWin['content'].document.write(html);
	newWin['content'].document.close();
	
}

// BEGIN SHARED SECTION
// Script below here must be identical in each of the javascript files.

var DEBUG = false;

var bOnsubmit = false;
var bGetContent = false;
var hiddenName;
var sDebugLevel = "off";
var sLogger = "console";
var bLocalDeploy = false;
var sJREDownloadURL = "";
var sMinimumJREVersion = "";
var bShowErrorMessage = "true";
var eljLoadingMessage = "Updating components and initializing...";
var createdDiv = false;
var bUseTextArea = false;
var bForceUseTextArea = false;
var iTARows = 17;
var iTACols = 55;
var cookie = false;

// The classpath for applets.
var classpath;

// The Path to the EditLive download directory.
var downloadPath;

// The original onsubmit function.
var fOnSubmit;

// The new window opened for submitting on Netscape 4.
var wNewWindow;

// True if we are in Netscape.
var IsNetscape = false;

// True if we are on windows.
var IsWindows = false;

// True if we are on mac.
var IsMac = false;

// True if this is Mac OS X.
var IsOSX = false;

// True if we are on Solaris.
var IsSolaris = false;

// True if we are on linux.
var IsLinux = false;

// True if we are in IE.
var IsMSIE = false;

// True if we are in safari.
var IsSafari = false;

// True is we are in Opera
var IsOpera = false;

// True if we are in the IE DHTML control.
var IsMSIEDHTML = false;

// The PageID
var iPageID = 0;

var AllowMacNetscape = false;

function setAllowNetscapeOnMac(value) {
	AllowMacNetscape = value;
}

/********************************************************
 Globals
*********************************************************/

function EditLiveCommon_safariSubmit(url) {
	var win = window.open("", "eljSubmit", "toolbar=no,location=no,directories=no,status=no,menubar=no,scrollbars=no,resizable=no,width=1,height=1");
	win.location = url;
}

function EditLiveCommon_setDownloadingMessage(message) {
	eljLoadingMessage = message;
}

function EditLiveCommon_setDownloadDirectory(sDir)
{
	combineDownloadURL(sDir);
	if(sDir.charAt(sDir.length - 1) != "/"){
		sDir += "/";
	} //end if
	downloadPath = sDir;
	this.downloadDirectory = sDir;
}

function EditLiveCommon_setLocalDeployment(bDeploy)
{
	bLocalDeploy = bDeploy;
}

function EditLiveCommon_setAutoSubmit(val)
{
	this.bAutoSubmit = val;
}

function EditLiveCommon_setMinimumJREVersion(val)
{
	if(val == "1.3.1" || val == "1.4.1" || val == "1.4.2"){
		sMinimumJREVersion = val;
	}
}

function EditLiveCommon_setJREDownloadURL(val) {
	sJREDownloadURL = val;
}

function EditLiveCommon_setShowSystemRequirementsError(val) {
	bShowErrorMessage = val;
}

function EditLiveCommon_setDebugLevel(val) {
	sDebugLevel = val;
}

function EditLiveCommon_setLogger(val) {
	sLogger = val;
}

//******************************



function EditLiveCommon_setXMLURL(strXMLURL){
	if(this.started == true){
		return false;
	} //end if
	var index = this.paramNames.length
	this.paramNames[index] = "setXMLURL";
	this.paramValues[index] = strXMLURL;
} //setXMLURL


function EditLiveCommon_setXML(strXML) {
	if(this.started == true){
		return false;
	} //end if
	var index = this.paramNames.length
	this.paramNames[index] = "setXML";
	this.paramValues[index] = strXML;
} //setXML


function EditLiveCommon_setCookie(cookie_) {
	dalert("in setCookie");
	if(this.started == true)
		return false;
	if(cookie_ != "") {
		dalert("Cookie:    " + cookie_);
		this.cookie = true;
		var index = this.paramNames.length
		this.paramNames[index] = "Cookie";
		this.paramValues[index] = cookie_;
	}
}

function EditLiveCommon_setLocale(strLocale) {
	if (this.started == true) {
		return false;
	} //end if
	var index = this.paramNames.length
	this.paramNames[index] = "setLocale";
	this.paramValues[index] = strLocale;
}

function EditLiveJava_setUseTextarea(bUseText) {
	bForceUseTextArea = bUseText;
}


function EditLiveJava_setTextareaRows(iRows) {
	iTARows = iRows;
}

function EditLiveJava_setTextareaCols(iCols) {
	iTACols = iCols;
}

function EditLiveJava_setPreload(sPreload){
	if(this.started == true){
		return false;
	}
	var index = this.paramNames.length
	dalert("Preload:   " + sPreload);
	this.paramNames[index] = "setPreload";
	this.paramValues[index] = sPreload;
}

function EditLiveJava_setHead(src){
	if(this.started == true){
		return false;
	}
	dalert("Head    " + src);
	var index = this.paramNames.length
	this.paramNames[index] = "Head";
	this.paramValues[index] = src;
} //setHead

function EditLiveJava_setBaseURL(url){
	if(this.started == true){
		return false;
	}
	dalert("Base    " + url);
	var index = this.paramNames.length
	this.paramNames[index] = "EphoxBaseURL";
	this.paramValues[index] = url;
} //setHead

//****************************** JavaScript API Routines ******************************

function EditLiveCommon_InsertHTMLAtCursor(sActionValue){
	EditLiveCommonStatic_CustomAction("inserthtmlatcursor", sActionValue, this.name + "_elj");
}

function EditLiveCommon_InsertHyperlinkAtCursor(){
	if(arguments.length == 1) {
		EditLiveCommonStatic_CustomAction("inserthyperlinkatcursor", arguments[0], this.name + "_elj");
	} else {
		var i = 1;
		var sHyperlinkString = arguments[0];
		for(i = 1; i < arguments.length; i++) {
			sHyperlinkString += "##ephox##";
			sHyperlinkString += arguments[i];
		}
		EditLiveCommonStatic_CustomAction("inserthyperlinkatcursor", sHyperlinkString, this.name + "_elj");
	}
}

function EditLiveCommon_UploadFiles(sActionValue){
	EditLiveCommonStatic_CustomAction("uploadImages", sActionValue, this.name + "_elj");
}

function EditLiveCommonStatic_DoCustomAction() {
	var paramNames = new Array();
	var paramValues = new Array();
	
	paramNames[0] = "CustomAction";
	paramValues[0] = "customaction:" + ephoxGAction + ":" + ephoxGValue;
	paramNames[1] = "CustomActionTarget";
	paramValues[1] = ephoxGName;
	
	EditLiveCommonStatic_runPostApplet(paramNames, paramValues);
}

var ephoxGAction;
var ephoxGValue;
var ephoxGName;

function EditLiveCommonStatic_CustomAction(action, value, name) {
	ephoxGAction = action;
	ephoxGValue = value;
	ephoxGName = name;
    setTimeout("EditLiveCommonStatic_DoCustomAction()", 10);
}

//**************************************************************************************

function EditLiveCommonStatic_detectBrowser() {
	var version = navigator.appVersion;
	if (version != "") {
		var iParen = version.indexOf("(", 0);
		var sUsrAgent = new String(navigator.userAgent);
		sUsrAgent = sUsrAgent.toLowerCase();

		navigator.clientVersion = version.substring(0, iParen - 1);
		if (sUsrAgent.indexOf("msie", 0) > 0) {
			IsMSIE = true;
			if (navigator.clientVersion.substring(0, 1) >= 4) {
				IsMSIEDHTML = true;
			}
		} else if (sUsrAgent.indexOf("safari", 0) >= 0) {
			IsSafari = true;
		}else if (sUsrAgent.indexOf("mozilla", 0) >= 0) {
			IsNetscape = true;
		} else if (sUsrAgent.indexOf("opera", 0) >= 0) {
			IsMSIE = true;
			IsOpera = true;
		}

		if (sUsrAgent.indexOf("win", 0) > 0) {
		  IsWindows = true;
		} else if (sUsrAgent.indexOf("mac", 0) > 0){
		    IsMac = true;
		    if (IsSafari || IsNetscape) {
		    	IsOSX = true;
		    } else {
				for(i = 0; i < navigator.plugins.length; i++){
					if(navigator.plugins[i].name == "Default Plugin Carbon.cfm") {
						IsOSX = true;
					}
					if (navigator.plugins[i].name.indexOf("OS X") > -1) {
						IsOSX = true;
					}
				}
			}
		} else if (sUsrAgent.indexOf("sunos", 0) > 0){
		    IsSolaris = true;
		} else if (sUsrAgent.indexOf("linux", 0) > 0){
		    IsLinux = true;
		}
	}
}

// Determines whether or not the current browser is supported.
function EditLiveCommonStatic_isSupportedBrowser() {
	if (IsWindows) {
		return IsMSIE || IsNetscape;
	} else if (IsOSX) {
		return IsMSIE || IsSafari;
	} else if (IsSolaris) {
		return IsNetscape;
	} else if (IsLinux) {
		return IsNetscape;
	} else {
		return false;
	}
}


function EditLiveCommonStatic_generateAppletTag(width, height, name, classpath, applet, paramNames, paramValues) {
	if(iPageID == 0) {

		var dTemp = new Date();
		iPageID = dTemp.getTime();
	}
	// Set the JRE download URL if it hasn't already been set.
	if (sJREDownloadURL == "") {
		sJREDownloadURL = downloadPath + "j2re-1_4_2_05-windows-i586-p.exe";
	}
	
	var str = "";
	var index = paramNames.length
	var bUpgradeOSXError = false;
	var bLinuxError = false;
	
	if (IsWindows == true) {
		if(IsMSIE == true) {
			//OBJECT start tag
			str += '<object';
			str += ' classid="clsid:8AD9C840-044E-11D1-B3E9-00805F499D93"';
			str += ' width="' + width +'"';
			str += ' height="' + height +'"';
			str += ' name="' + name + '"';
			
			if(bLocalDeploy) {
				if(sMinimumJREVersion == "1.4.2"){
			   		str += ' codebase="' + sJREDownloadURL + '#Version=1,4,2"'
			   	} else 	if(sMinimumJREVersion == "1.4.1"){
			   		str += ' codebase="' + sJREDownloadURL + '#Version=1,4,1"'
			   	} else {
			   		str += ' codebase="' + sJREDownloadURL + '#Version=1,4,0"'
			   	}				
			} else {
				if(sMinimumJREVersion == "1.3.1"){
					str += ' codebase="https://java.sun.com/products/plugin/autodl/jinstall-1_3_1_01-win.cab#Version=1,3,1,01"'
				} else if(sMinimumJREVersion == "1.4.2"){
			   		str += ' codebase="https://java.sun.com/update/1.4.2/jinstall-1_4_2_05-windows-i586.cab#Version=1,4,2,50"'
			   	} else if(sMinimumJREVersion == "1.4.1"){
					str += ' codebase="https://java.sun.com/products/plugin/autodl/jinstall-1_4_1_01-windows-i586.cab#version=1,4,1"'
				} else {
					str += ' codebase="https://java.sun.com/update/1.4.2/jinstall-1_4_2_05-windows-i586.cab#Version=1,4,0"'
				}
			}

			str += '>';

			//Standard PARAM elements
			str += '<param name="code" value="' + applet + '" >';
			str += '<param name="codebase" value="' + getURLbase() + '" >';
			if(sMinimumJREVersion == "1.3.1"){
				str += '<param name="type" value="application/x-java-applet;version=1.3">';
			} else if(sMinimumJREVersion == "1.4.2"){
			   	str += '<param name="type" value="application/x-java-applet;version=1.4.2">';
			} else if(sMinimumJREVersion == "1.4.1"){
				str += '<param name="type" value="application/x-java-applet;version=1.4.1">';
			} else {
				str += '<param name="type" value="application/x-java-applet;version=1.4">';
			}
			str += '<param name="archive" value="' + classpath + '" >'; 
			str += '<param name="cache_option" value="Plugin" >';
			str += '<param name="cache_archive" value="' + classpath + '" >';
			str += '<param name="name" value="' + name + '" >';
			if(IsOpera) {
				str += '<param name="scriptable" value="true">';
			} else {
				str += '<param name="scriptable" value="false">';
			}
			str += '<param name="MAYSCRIPT" value="true">';
			str += '<param name="progressbar" value="true">';
			str += '<param name="boxmessage" value="' + eljLoadingMessage + '">';
			str += '<param name="UserAgent" value="' + escape(navigator.userAgent) +'">';
			str += '<param name="DebugLevel" value="' + sDebugLevel + '">';
			str += '<param name="Logger" value="' + sLogger + '">';
			str += '<param name="PageID" value="' + iPageID + '">';
			
			//Custom PARAM elements
			var index = 0;
			while(index < paramNames.length) {
				str += '<param';
				str += ' name="' + paramNames[index] + '"';
				str += ' value="' + paramValues[index] + '"'
				str += '>';
				index++;
			} //end while

			//OBJECT end tag
			str += '</object>';

		} else if (IsNetscape == true) {
			//Start of EMBED start tag
			str += '<EMBED';

			//Standard attributes
			if(sMinimumJREVersion == "1.3.1"){
				str += ' type="application/x-java-applet;version=1.3"';
			} else if(sMinimumJREVersion == "1.4.2"){
				str += ' type="application/x-java-applet;version=1.4.2"';
			} else if(sMinimumJREVersion == "1.4.1"){
				str += ' type="application/x-java-applet;version=1.4.1"';
			} else {
				str += ' type="application/x-java-applet;version=1.4"';
			}
			str += ' code="' + applet + '" ';
			str += ' codebase="' + getURLbase() + '"';
			str += ' cache_archive="' + classpath + '"';
			str += ' archive="' + classpath + '"';
			str += ' cache_option="Plugin"';
			str += ' name="' + name + '"';
			str += ' width="' + width + '"';
			str += ' height="' + height + '"';
			str += ' scriptable="true" ';
			str += ' MAYSCRIPT=true ';
			
			str += ' UserAgent="' + escape(navigator.userAgent) +'"';
			str += ' DebugLevel="' + sDebugLevel + '"';
			str += ' Logger="' + sLogger + '"';
			str += ' PageID="' + iPageID + '" ';
			
			//Custom attributes
			var index = 0;
			while(index < paramNames.length){
				//Attribute name
				str += paramNames[index];
				//Attribute value
				str += '="' + paramValues[index] + '" ';
				index++;
			} //end while

			//Java plug-in install instructions for Netscape
			str += ' pluginspage="' + getURLbase() + downloadPath + 'plugin-install.html"';

			//End of EMBED start tag
			str += '>';

			//NOEMBED element
			str += '<NOEMBED>';
			str += '</NOEMBED>';

			//EMBED end tag
			str += '</EMBED>';
			

		} else {
			// Unknown windows browser.
			str = '';
		}

	} else if (IsMac == true) {
		if(IsOSX) {
			if (IsSafari == true || (AllowMacNetscape && IsNetscape)) {
				//Start APPLET start tag
				str += '<applet';

				//Standard attributes
				str += ' code="' + applet + '"';
				
				str += ' cache_archive="' + classpath + '"';
				str += ' cache_option="Plugin"';
				str += ' archive="' + classpath + '"';
				str += ' codebase="' + getURLbase() + '"';
				str += ' name="' + name + '"';
				str += ' width="' + width + '"';
				str += ' height="' + height + '"';

				//End APPLET start tag
				str += ' MAYSCRIPT>';

				//Custom PARAM elements
				var index = 0;
				while(index < paramNames.length) {
					str += '<param';
					str += ' name="' + paramNames[index] + '" ';
					str += ' value="' + paramValues[index] + '"';
					str += '>';
					index++;
				} //end while
				
				str += '<param name="UserAgent" value="' + escape(navigator.userAgent) +'">';
				str += '<param name="DebugLevel" value="' + sDebugLevel + '">';
				str += '<param name="Logger" value="' + sLogger + '">';
				str += '<param name="progressbar" value="true">';
				str += '<param name="boxmessage" value="' + eljLoadingMessage + '">';
				str += '<param name="PageID" value="' + iPageID + '">';
				
				//write out window loc
				var sFrameLoc = "";
				var sTemp = ".";
				var wCurrent = self;
				var wParent = self.parent;
				while (wParent != wCurrent) {
					
					for (i = 0; i < wParent.length; i++) {
						if (wParent.frames[i] == wCurrent) {
							sTemp = (".frames[" + i + "]") + sTemp;
						}
					}
					wCurrent = wParent;
					wParent = wParent.parent;
				}
				sTemp = "window" + sTemp;
				if (window.parent != self) {
					sFrameLoc = sTemp;
				}
				
				str += '<param name="JSWindowLoc" ';
				str += ' value="' + sFrameLoc + '"';
				str += '>';

				//APPLET end tag
				str +='</applet>';
			} else {
				str = '';
			}
		} else {
			str = '';
		}
	} else if(IsSolaris == true) {
		if(IsNetscape == true) {
			//Start of EMBED start tag
			str += '<EMBED';

			//Standard attributes
			str += ' type="application/x-java-applet;version=1.4"';
			str += ' code="' + applet + '" ';
			str += ' codebase="' + getURLbase() + '"';
			str += ' cache_archive="' + classpath + '"';
			str += ' archive="' + classpath + '"';
			str += ' cache_option="Plugin"';
			str += ' name="' + name + '"';
			str += ' width="' + width + '"';
			str += ' height="' + height + '"';
			str += ' scriptable="true" ';
			str += ' MAYSCRIPT=true ';
			str += ' UserAgent="' + escape(navigator.userAgent) +'"';
			str += ' DebugLevel="' + sDebugLevel + '"';
			str += ' Logger="' + sLogger + '"';
			str += ' PageID="' + iPageID + '"';


			//Custom attributes
			var index = 0;
			while(index < paramNames.length) {
				//Attribute name
				str += paramNames[index];
				//Attribute value
				str += '="' + paramValues[index] + '" ';
				index++;
			} //end while

			//Java plug-in install instructions for Netscape
			str += ' pluginspage="https://java.sun.com/products/plugin/index.html#download"';

			//End of EMBED start tag
			str += '>';

			//NOEMBED element
			str += '<NOEMBED>';
			str += '</NOEMBED>';

			//EMBED end tag
			str += '</EMBED>';
		} else {
			// Unsupported browser.
			str = '';
		}
	} else if (IsLinux == true) {
			if (IsNetscape == true) {
				//Start of EMBED start tag
				str += '<EMBED';
	
				//Standard attributes
				str += ' type="application/x-java-applet;version=1.4"';
				str += ' code="' + applet + '" ';
				str += ' codebase="' + getURLbase() + '"';
				str += ' cache_archive="' + classpath + '"';
				str += ' archive="' + classpath + '"';
				str += ' cache_option="Plugin"';
				str += ' name="' + name + '"';
				str += ' width="' + width + '"';
				str += ' height="' + height + '"';
				str += ' scriptable="true" ';
				str += ' MAYSCRIPT=true ';
				str += ' UserAgent="' + escape(navigator.userAgent) +'"';
				str += ' DebugLevel="' + sDebugLevel + '"';
				str += ' Logger="' + sLogger + '"';
				str += ' PageID="' + iPageID + '"';
	
	
				//Custom attributes
				var index = 0;
				while (index < paramNames.length) {
					//Attribute name
					str += paramNames[index];
					//Attribute value
					str += '="' + paramValues[index] + '" ';
					index++;
				} //end while
	
				//Java plug-in install instructions for Netscape
				str += ' pluginspage="https://java.sun.com/products/plugin/index.html#download"';
	
				//End of EMBED start tag
				str += '>';
	
				//NOEMBED element
				str += '<NOEMBED>';
				str += '</NOEMBED>';
	
				//EMBED end tag
				str += '</EMBED>';
			} else {
				// Unsupported browser.
				str = '';
			}
	} //end if
	if(bForceUseTextArea) {
		bUseTextArea = true;
		bForceUseTextArea = false;
		bUpgradeOSXError = false;
		bLinuxError = false;
		bShowErrorMessage = false;
		str = '';
	}
	if (str == '') {
		if (bShowErrorMessage) {
			if(IsMac && !IsOSX){
				str += "<p>The minimum operating system required to run EditLive! for Java on Apple Macintosh is MacOS X Update 10.1.1.<br>EditLive! for Java only supports Safari. Please ensure you are using Safari to browse.</p>"
			} else if(IsLinux){
				str += "<p>EditLive! for Java only supports Netscape Navigator 7.1 or Mozilla 1.4 running JRE 1.4.2 on Linux. Please ensure you are using the correct browser and JRE.</p>"
			} else {
				str += "<p>This system does not meet the minimum requirements to run EditLive! for Java. Now using a textarea instead.</p>";
			}
		}
		str += EditLiveCommonStatic_getTextArea(paramNames, paramValues);
	}
	return str;
}

function EditLiveCommonStatic_getTextArea(paramNames, paramValues) {
	var str = "";
	str += '<textarea name="'+hiddenName+'" rows='+ iTARows +' cols='+ iTACols +'>';
	var index = 0;
	while(index < paramNames.length) {
		if(paramNames[index] == "Body" || paramNames[index] == "Document"){
			var docSource = paramValues[index];
			str += unescape(docSource.replace(/\+/gi, "%20"));
		}
		index++;
	} //end while
	str += '</textarea>';
	bUseTextArea = true;
	return str;
}

function EditLiveCommonStatic_getAuxDiv() {
	if (!createdDiv) {
		createdDiv = true;
		return EditLiveCommonStatic_createDiv("eLAuxDiv");
	}
}

function EditLiveCommonStatic_createDiv(name) {
	var str = "";
	if(IsMSIE || IsSafari)
	{
		str += '<div id="' + name + '" name="' + name + '" width="1" height="1"></div>';
	}
	else
	{
		var version = navigator.appVersion;
		if(version.charAt(0) != "4") {
			str += '<div id="' + name + '" name="' + name + '"></div>';
		} else {
			//str += '<layer id="' + name + '" name="' + name + '"></layer>';
		} //end if
	} //end if
	return str;
}

function EditLiveCommonStatic_getEditorLayout(appletTag, borderStyle, width, height) {
	var str = "";
	if (IsMac) {
		str = '<table border="0" width="';
		str += width;
		str += '" height="';
		str += height;
		str += '" ><tr><td>';
		str += '<div style="';
		str += borderStyle;
		str += '">';
		str += appletTag;
		str += '</div>';
		var auxDiv = EditLiveCommonStatic_getAuxDiv();
		if (auxDiv) {
			str += '</td><td>';
			str += auxDiv;
		}
		str += '</td></tr></table>';
	} else {
		str = '<div style="';
		str += borderStyle;
		str += '">';
		str += appletTag;
		str += '</div>';
		var auxDiv = EditLiveCommonStatic_getAuxDiv();
		if (auxDiv) {
			str += auxDiv;
		}
	}
	return str;
}

function EditLiveCommon_setShowButtonText(text) {
	var index = this.paramNames.length;
	this.paramNames[index] = "showButtonText";
	this.paramValues[index] = text;
}

function EditLiveCommon_setShowButtonIconURL(url) {
	var index = this.paramNames.length;
	this.paramNames[index] = "showButtonIconURL";
	this.paramValues[index] = url;
}

function EditLiveCommon_setHideButtonText(text) {
	var index = this.paramNames.length;
	this.paramNames[index] = "hideButtonText";
	this.paramValues[index] = text;
}

function EditLiveCommon_setHideButtonIconURL(url) {
	var index = this.paramNames.length;
	this.paramNames[index] = "hideButtonIconURL";
	this.paramValues[index] = url;
}

function EditLiveCommon_showAsButton(popout) {
	var index = this.paramNames.length;
	this.paramNames[index] = "showAsButton";
	this.paramValues[index] = "true";
	index++;
	this.paramNames[index] = "popout";
	this.paramValues[index] = popout;
	this.show();
}

function EditLiveCommon_show() {
	dalert("Show");
	document.write(this.getAppletHTML());
	if (!bUseTextArea) {
		var fields = this.getHiddenFields();
		var i = 0;
		while (i < fields.length) {
			document.write('<input name="' + fields[i] + '" type="hidden">');
			i++;
		}
	}
	if (this.bAutoSubmit) {
		EditLiveCommonStatic_getOnSubmit();
	}
	this.started = true;
	dalert("End Show");
}

/********************************************************
 Write to onSubmit
*********************************************************/
function EditLiveCommonStatic_getOnSubmit(){
	var name = hiddenName;
	dalert("getOnSubmit: " + name);
	
	if(!bOnsubmit && EditLiveCommonStatic_isSupportedBrowser()){
		var bFound = false;
		bOnsubmit = true;
		//find form where ELJ exists
		var sAppletName = name;
		for(var formsIndex = 0; formsIndex < document.forms.length; formsIndex++){
			for(var elementsIndex = 0; elementsIndex < document.forms[formsIndex].elements.length; elementsIndex++){
				if(document.forms[formsIndex].elements[elementsIndex].name == sAppletName){
					//found the form
					fOnSubmit = document.forms[formsIndex].onsubmit;
					document.forms[formsIndex].onsubmit = EditLiveCommonStatic_GetContent;
					bFound = true;
					dalert("Attached to onsubmit.");
				} 
				if(bFound){
					break;
				}
			} //end for
			if(bFound){
				break;
			}
		} //end for
	} //end if
} //getOnSubmit

function EditLiveCommonStatic_getThisForm() {
	var name = hiddenName;
	var bFound = false;
	bOnsubmit = true;
	//find form where ELJ exists
	var sAppletName = name;
	for(var formsIndex = 0; formsIndex < document.forms.length; formsIndex++){
		for(var elementsIndex = 0; elementsIndex < document.forms[formsIndex].elements.length; elementsIndex++){
			if(document.forms[formsIndex].elements[elementsIndex].name == sAppletName){
				//found the form
				bFound = true;
				return document.forms[formsIndex];
			} 
			if(bFound){
				break;
			}
		} //end for
		if(bFound){
			break;
		}
	} //end for
}

function EditLiveCommonStatic_submitFunction() {
	setTimeout("EditLiveCommonStatic_doSubmit()", 100);
}

function EditLiveCommonStatic_doSubmit() {
	dalert("SubmitFunction");
	if(!IsMSIE && !IsSafari) {
		var version = navigator.appVersion;
		if(version.charAt(0) == "4") {
			wNewWindow.close();
		} //end if
	} //end if
	var sAppletName = hiddenName;
	for(var formsIndex = 0; formsIndex < document.forms.length; formsIndex++){
		for(var elementsIndex = 0; elementsIndex < document.forms[formsIndex].elements.length; elementsIndex++){
			if(document.forms[formsIndex].elements[elementsIndex].name == sAppletName){
			//found the form
				if(fOnSubmit == null) {
					var submitIsObject;
					if (IsNetscape) {
						submitIsObject = ((typeof document.forms[formsIndex].submit) == "object");
					} else {
						if ((document.forms[formsIndex].submit.id == 'submit') || (document.forms[formsIndex].submit.name == 'submit')) {
							submitIsObject = true;
						} else {
							submitIsObject = false;
						}
					}
					if(submitIsObject){
						document.forms[formsIndex].onsubmit=null;
						document.forms[formsIndex].submit.click();
					} else {
						document.forms[formsIndex].submit();
					}
					bGetContent = false;					
				} else {
					var rVal = fOnSubmit();
					if(rVal != false) {
						var submitIsObject = false;
						if (IsNetscape) {
							submitIsObject = ((typeof document.forms[formsIndex].submit) == "object");
						} else {
							submitIsObject = ((document.forms[formsIndex].submit.id == 'submit') || (document.forms[formsIndex].submit.name == 'submit'));
						}
						if(submitIsObject){
							document.forms[formsIndex].onsubmit=null;
							document.forms[formsIndex].submit.click();
						} else {
							document.forms[formsIndex].submit();
						}
					} //end if
					bGetContent = false;
				}//end if
				
			} //end if
		} //end for
	} //end for
}

/********************************************************
 Form submitting functions
*********************************************************/

function EditLiveCommonStatic_runPostApplet(paramNames, paramValues) {

	if(IsMSIE)
	{
		document.all.item("eLAuxDiv").innerHTML = "";
	} //end if

	var appletName = 'POSTApplet';
	var appletClass = 'com.ephox.editlive.java2.POSTApplet.class';
	var appletWidth = 1;
	var appletHeight = 1;
	if (IsMac) {
		appletHeight = 90;
	}
	
	
	var appletTag = EditLiveCommonStatic_generateAppletTag(appletWidth, appletHeight, appletName, classpath, appletClass,
			paramNames, paramValues, downloadPath);
	
	if (IsWindows && IsMSIE) {
		var myDiv = document.getElementById("eLAuxDiv");
		myDiv.innerHTML = appletTag;
	} else if (IsMSIE) {
		document.all.item("eLAuxDiv").innerHTML = appletTag;
	} else {
		var version = navigator.appVersion;
		if(version.charAt(0) == "4" && !IsSafari) {
			wNewWindow = window.open('','EditLive','width=' + 1 + ',height=' + 1 +',status=no,resizable=no,scrollbars=no,location=no,toolbar=no');
			wNewWindow.document.open();
			wNewWindow.document.write(appletTag);
			wNewWindow.document.close();
		} else {
			var myDiv = document.getElementById("eLAuxDiv");
			myDiv.innerHTML = appletTag;
		} //end if
	} //end if
}

function EditLiveCommonStatic_GetContent() {
dalert("Get Content");
	if(bGetContent){
		dalert("Already getting content.");
		return false;
	} //end if
	bGetContent = true;

	var paramNames = new Array();
	var paramValues = new Array();
	
	paramNames[0] = "GetContent";
	paramValues[0] = "true";
	
	EditLiveCommonStatic_runPostApplet(paramNames, paramValues);
	return false;
}

function EditLiveCommon_setBorderStyle(style) {
	this.borderStyle = style;
}

//**************************** Support Functions ***********************
//This function is a regular combineURL but to make life easier it uses window.location.href
// as the absolute URL so we can use things like window.location.hostname instead of writing
// those functions somewhere else.
function combineDownloadURL ( sDirectory ) {

	var relURL = sDirectory;

	//every other reference in this file assumes downloadDirectory has no
	// trailing "/".  Rather than try to fix everywhere else, we'll just
	// rip out the trailing "/" if it is there.
	if (relURL.charAt(relURL.length - 1) == "/") {
		relURL = relURL.substr(0, relURL.length - 1);
	}


	//if relURL is absolute, forget about combining
	if (relURL.indexOf("://") != -1) return relURL;
	
	//we only want the path of the URL
	var absURL = removeFilename(window.location.href);
	
	//do the combining work
	switch(relURL.charAt(0)) {
	
		//relative to hostname
		case '/':
			return window.location.protocol + "//" +
				window.location.hostname + relURL;
			break;

		//relative to absolute path
		case '.':
			var sOldURL;
			//cycle through relURL ripping directories off the absURL as we
			// go up
			while(relURL.substr(0,3) == "../") {
				sOldURL = absURL;
				absURL = absURL.substring(0, absURL.lastIndexOf('/'));
				relURL = relURL.substr(relURL.indexOf('/') + 1);
				//if the final two chars are "//" then the relative
				// URL goes back too many folders, undo the previous rip
				if (absURL.charAt(absURL.lastIndexOf('/') - 1) == ':') {
					absURL = sOldURL
				}
			}
			return absURL + "/" + relURL;
			break;
			
		//is a subdir of the current dir
		default:
			return absURL + "/" + relURL;
			break;
	}

}

//get the main path of the URL, minus the xx.htm etc
function removeFilename ( sURL ) {
	if (sURL.lastIndexOf('.') > sURL.lastIndexOf('/')) {
		return sURL.substring(0, sURL.lastIndexOf('/'));
	} else {
		return sURL;
	}
}

//Set value for hidden form field

function setFormValue(sName, sValue) {
	dalert("Set form item: " + sName + " to " + sValue);
	for(var formsIndex = 0; formsIndex < document.forms.length; formsIndex++){
		for(var elementsIndex = 0; elementsIndex < document.forms[formsIndex].elements.length; elementsIndex++){
			if(document.forms[formsIndex].elements[elementsIndex].name == sName){
				document.forms[formsIndex].elements[elementsIndex].value=sValue;
			} 
		} //end for
	} //end for
	return false;
} //setFormValue



function eljTransferFocus() {
	var myDiv;
	if(IsMSIE)
	{
		myDiv = document.all.item("eLAuxDiv");
	}
	else
	{
		var version = navigator.appVersion;
		if(version.charAt(0) == "4") {
			myDiv = document.ids.eLAuxDiv;
		} else {
			myDiv = document.getElementById("eLAuxDiv");
		} //end if
	}
	
	// Find the next element we can focus on.
	if (IsMSIE) {
		var parent = myDiv.parentElement;
		var num = myDiv.sourceIndex + 1;
		var nextElem = document.all[num];
		while (!isFocusable(nextElem)) {
			num++;
			nextElem = document.all[num];
		}
		nextElem.focus();
	} else {
		var parent = myDiv.parentNode;
		var foundFocusable = false;
		while (!foundFocusable) {
			var i = 0;
			var foundMyDiv = false;
			var firstDivMatch = true;
			while (i < parent.childNodes.length && !foundFocusable) {
				var child = parent.childNodes[i];
				if (child == myDiv) {
					foundMyDiv = true;
				}
				if (foundMyDiv) {
					if (firstDivMatch) {
						firstDivMatch = false;
					} else {
						var focusable = eljSearchTree(child);
						if (focusable) {
							myDiv = focusable;
							foundFocusable = true;
							break;
						}
					}
				}
				i++;
			}
			if (!foundFocusable) {
				myDiv = parent;
				parent = parent.parentNode;
			}
		}
		
		//alert(myDiv.tagName);
		//var win2 = window.open('','EditLive','width=' + 1 + ',height=' + 1 +',status=no,resizable=no,scrollbars=no,location=no,toolbar=no');
		//win2.close();
		myDiv.focus();
	}
}

function getURLbase() {
	var src = window.location.href;
	var indLoc = src.lastIndexOf("/");
	var URLBase = src.substr(0, indLoc + 1);

	strPath = new String(document.location);
	strPath = strPath.substr(0, strPath.lastIndexOf("/") + 1);

	return URLBase;
}

function dalert(message) {
	if (DEBUG) {
		alert(message);
	}
}