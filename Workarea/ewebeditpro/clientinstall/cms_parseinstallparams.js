// Copyright 2003 Ektron, Inc.
// Revision Date: 2003-12-02 

var g_EktronOverrideParamList = ParseParameterString();

// These are the values extracted
//var given_eWebEditProPath = "";
//var given_WebImageFXPath = "";
//var given_LicenseKeys = "";  // These make the intro pages happy.
//var given_WifxLicenseKeys = "";
var LicenseKeys = "";
var WifxLicenseKeys = "";


// This will replace the parameters sent.
ReplaceParameters();


function ParseParameterString()  
{
	var objQuery = new Object();
	var strQuery = location.search.substring(1);
	var aryQuery = strQuery.split("&");
	var pair = [];
	
	for (var i = 0; i < aryQuery.length; i++)
	{
		pair = aryQuery[i].split("=");
		if (pair.length == 2)
		{
			objQuery[unescape(pair[0])] = unescape(pair[1]); 
		}
	}
	return objQuery;
}

function ReplaceParameters()
{
    // Looking for these parameters:
    var sValue;
    var sPath;
    
    //     "instewep"
    if("undefined" != typeof g_EktronOverrideParamList.instewep)
    {
        sValue = g_EktronOverrideParamList.instewep;
        if(sValue.length > 0)
        {
            sPath = ValidEktronPath(sValue);
            if(sPath.length > 0)
            {
                given_eWebEditProPath = sPath;
            }
        }   
    }

    //     "instwifx"
    if("undefined" != typeof g_EktronOverrideParamList.instwifx)
    {
        sValue = g_EktronOverrideParamList.instwifx;
        if(sValue.length > 0)
        {
            sPath = ValidEktronPath(sValue);
            if(sPath.length > 0)
            {
                given_WebImageFXPath = sPath;
            }
        }   
    }

    //     "instewep"
    if("undefined" != typeof g_EktronOverrideParamList.licnewep)
    {
        sValue = g_EktronOverrideParamList.licnewep;
        if(sValue.length > 0)
        {
            // In this situation, all of the keys are grouped together
            // into both key values.  In this way, there is no confusion
            // of which to use when and where.
            if(given_LicenseKeys.length > 0)
            {
                given_LicenseKeys += ",";
            }
            given_LicenseKeys += sValue;
 
            if(given_WifxLicenseKeys.length > 0)
            {
                given_WifxLicenseKeys += ",";
            }
            given_WifxLicenseKeys += sValue;    
        }   
    }

    //     "instewep"
    if("undefined" != typeof g_EktronOverrideParamList.licnwifx)
    {
        sValue = g_EktronOverrideParamList.licnwifx;
        if(sValue.length > 0)
        {
            // In this situation, all of the keys are grouped together
            // into both key values.  In this way, there is no confusion
            // of which to use when and where.
            if(given_LicenseKeys.length > 0)
            {
                given_LicenseKeys += ",";
            }
            given_LicenseKeys += sValue;
 
            if(given_WifxLicenseKeys.length > 0)
            {
                given_WifxLicenseKeys += ",";
            }
            given_WifxLicenseKeys += sValue;
        }   
    }
	
	// The official values.
	LicenseKeys = given_LicenseKeys;
	WifxLicenseKeys = given_WifxLicenseKeys;
}

function ValidEktronPath(sValue)
{
    var sPath = sValue;

    // Make sure we have slashes at both ends.
    if((sPath.charAt(sPath.length - 1) != "/") && (sPath.charAt(sPath.length - 1) != "\\"))
    {
        sPath += "/";
    }
    if((sPath.charAt(0) != "/") && (sPath.charAt(0) != "\\"))
    {
        sPath = "/" + sPath;
    }
    
    return(sPath);
}

function loadMsXml()
{
	var strHtml = "";
	strHtml += '<object classid="clsid:88d969c0-f192-11d4-a65f-0040963251e5" height="0" width="0" \n'; 	
	strHtml += 'codebase="' + given_eWebEditProPath + 'msxml4.cab#version=4,10,9404,0"></object> \n';
	document.write(strHtml);
}

function writeLoader(sCaption, sWifxCaption)
{
	loadMsXml();
	var strHtml = "";
	strHtml += '<object \n';
    strHtml += 'id="ewepMyContent1" \n';
    strHtml += 'name="ewepMyContent1" \n';
    strHtml += 'classid="CLSID:DB1B4C3B-8690-43B2-9045-91EDA7A12580" \n';
    strHtml += 'codebase="' + given_eWebEditProPath + 'ewebeditpro4.cab#version=' + cVERSION + '" \n';
    strHtml += 'width="100%" \n';
    strHtml += 'height="180"> \n';
    strHtml += '<param name="Caption" value="' + sCaption + '"> \n';
	strHtml += '</object> \n';
	strHtml += '<script language="JavaScript1.2" type="text/javascript" for="ewepMyContent1" event="LoaderReady()"> \n';
   	strHtml += 'OnReadyHandler(); \n';
	strHtml += '</sc' + 'ript> \n';
	document.write(strHtml);
	strHtml = "";
	strHtml += '<object \n';
    strHtml += 'id="ewepMyImage1" \n';
    strHtml += 'name="ewepMyImage1" \n';
    strHtml += 'classid="CLSID:B40B74C9-C9B3-445C-9397-EC8285292947" \n';
    strHtml += 'codebase="' + given_WebImageFXPath + 'webimagefx.cab#version=' + cIMGEDVERSION + '" \n';
    strHtml += 'width="100%" \n';
    strHtml += 'height="180"> \n';
    strHtml += '<param name="Caption" value="' + sWifxCaption + '"> \n';
	strHtml += '</object> \n';
	strHtml += '<scr' + 'ipt language="JavaScript1.2" type="text/javascript" for="ewepMyImage1" event="LoaderReady()"> \n';
   	strHtml += 'OnWifxReadyHandler(); \n';
	strHtml += '</sc' + 'ript> \n';
	document.write(strHtml);
	strHtml = "";
	// in the case of webimagefx.cab already installed, no OnWifxReadyHandler is fired 
	// (b/c WebImageFX.Loader object is not created)
	// and eWebEditPro is not drawn (b/c the loadingMsg animated GIF get the focus).
	// this is to toggle the focus from the animated GIF to activate the ewebeditpro, 
	// and to fire the onWIFXReady handler if WebImageFX is installed.
	strHtml += '<scr' + 'ipt language="JavaScript1.2" type="text/javascript">';
	// Toggle the focus of the animated GIF in 3 sec.
	strHtml += 'setTimeout("simulateClick()", 3000);\n';
	// Test if WebImageFX is installed in the client machine
	strHtml += 'var objAXPluginElement = new ActiveXElement("WebImageFX.WIFXLoader"); \n';
	strHtml += 'g_bWIFXInstalled = objAXPluginElement.isInstalled; \n';
	strHtml += 'if (true == g_bWIFXInstalled)\n';
	// Fire the ready event if WebImageFX is installed.
	strHtml += '{\n';
	strHtml += 'OnWifxReadyHandler(); \n';
	strHtml += '}\n';
	// Toggle the animated GIF focus by changing its visibility.
	strHtml += 'function simulateClick() \n'; 
	strHtml += '{\n';
	strHtml += 'document.loadingMsg.style.visibility = "hidden"; \n';
	strHtml += 'document.loadingMsg.style.visibility = "visible"; \n';
	strHtml += '}\n';
	strHtml += '</scr' + 'ipt>';
	document.write(strHtml);
	strHtml = "";
}

// Performs a quick check to ensure that the name is valid
// and that we are the control object and not a text area field.
function ValidEditorObject(sEditor)
{
	if (typeof eWebEditPro == "object")
	{
	        if (typeof eWebEditPro.instances[sEditor] == "object")
	        {
	            if (eWebEditPro.instances[sEditor].isEditor())
	            {
	                return(true);
	            }
	        }
	}
    return(false);
}