// Copyright 2000-2003, Ektron, Inc.
// Revision Date: 2003-05-22

/* It is best NOT to modify this file. */
/*
	See the Developer's Reference Guide for details.
	
	To add your own commands, define one or more of the following:
	WebImageFXExecCommandHandlers[your_cmd_here] = function(sEditorName, strCmdName, strTextData, lData) { }
	function WebImageFXExecCommand(sEditorName, strCmdName, strTextData, lData) { }
	WebImageFX.onexeccommand = your_custom_event_handler;
	
	To add your own media file handler, define:
	function WebImageFXMediaSelection(sEditorName) { } (for web page using HTTP)
	function WebImageFXMediaNotification(sEditorName) { } (for FTP)

*/

function onExecCommandHandler(strCmdName, strTextData, lData)
{
/*
	Defer call to actual handler for two reasons:
	1. Avoid recursion in case an action results in this same event firing.
	2. Netscape cannot effectively access the ActiveX control's methods in an event.
*/
	var sEditorName = WebImageFX.event.srcName;
	strCmdName = strCmdName + ""; // ensure it is a string
	strTextData = strTextData + ""; // ensure it is a string
	lData = lData * 1; // ensure it is a number
	setTimeout('onExecCommandDeferred("' + sEditorName + '", "' + strCmdName + '", ' + toLiteral(strTextData) + ', ' + lData + ')', 1);
}

function onExecCommandDeferred(sEditorName, strCmdName, strTextData, lData)
{
	if ("initialize" == strCmdName)
	{
		var objInstance = WebImageFX.instances[sEditorName];
		if (typeof objInstance != "undefined" && objInstance != null)
		{
			objInstance.receivedEvent = true;
			if (objInstance.isReady())
			{
				// Respond to the "initialize" event by sending "ready".
				// Responding is optional, but it speeds up initialization.
				// Cannot use WebImageFX[sEditorName] during "initialize" event.
				// Sync API: objInstance.editor.ExecCommand("ready", "", 0);
				objInstance.asyncCallMethod("ExecCommand", ["ready", "", 0], null, new Function());
			}
		}
		return;
	}
	
	if ("ready" == strCmdName)
	{
		var objInstance = WebImageFX.instances[sEditorName];
		if (typeof objInstance != "undefined" && objInstance != null)
		{
			objInstance.receivedEvent = true;
			if (objInstance.isReady())
			{
				if ("function" == typeof WebImageFXReady)
				{
					WebImageFXReady(sEditorName);
				}
				if (typeof WebImageFX.onready != "undefined")
				{
					WebImageFX.initEvent("onready");
					WebImageFX.event.type = "ready"; 
					WebImageFX.event.srcName = sEditorName;
					WebImageFX.raiseEvent("onready");
				}
			}
		}
		return;
	}
	
	if ("blur" == strCmdName)
	{
		// This command is raised when pressing Ctrl+Tab 
		// (unless Netscape captures the event).
		// Move focus from the editor to the next form field.
		var objInstance = WebImageFX.instances[sEditorName];
		var objField = WebImageFX.nextFormField(objInstance);
		if (objField)
		{
			objField.focus();
		}
		return;
	}
	
	var returnValue = true;
	if ("function" == typeof WebImageFXExecCommand)
	{
		returnValue = WebImageFXExecCommand(sEditorName, strCmdName, strTextData, lData);
	}
	
	if (returnValue != false)
	{
		var fnHandler = WebImageFXExecCommandHandlers[strCmdName];
		if ("function" == typeof fnHandler)
		{
			fnHandler(sEditorName, strCmdName, strTextData, lData);
		}
	}
		
	if (typeof WebImageFX.onexeccommand != "undefined")
	{
		WebImageFX.initEvent("onexeccommand");
		WebImageFX.event.type = "execcommand"; 
		WebImageFX.event.srcName = sEditorName;
		WebImageFX.event.cmdName = strCmdName;
		WebImageFX.event.textData = strTextData;
		WebImageFX.event.data = lData;
		WebImageFX.raiseEvent("onexeccommand");
	}
}
function onEditCompleteHandler(strloadname, strsavename)
{	
	var returnValue = true;
	var sEditorName = WebImageFX.event.srcName;
	
	if ("function" == typeof WebImageFXEditComplete)
	{
		returnValue = WebImageFXEditComplete(sEditorName, strloadname, strsavename);
	}
	if (typeof WebImageFX.oneditcomplete != "undefined")
	{
		WebImageFX.initEvent("oneditcomplete");		
		WebImageFX.raiseEvent("oneditcomplete");
	}
}

function onEditCommandCompleteHandler(strcmdname)
{	
	var returnValue = true;
	var sEditorName = WebImageFX.event.srcName;
	
	if ("function" == typeof WebImageFXEditCommandComplete)
	{
		returnValue = WebImageFXEditCommandComplete(sEditorName, strcmdname);
	}
	if (typeof WebImageFX.oneditcommandcomplete != "undefined")
	{
		WebImageFX.initEvent("oneditcommandcomplete");		
		WebImageFX.raiseEvent("oneditcommandcomplete");
	}
}
function onEditCommandStartHandler(strcmdname)
{	
	var returnValue = true;
	var sEditorName = WebImageFX.event.srcName;
	
	if ("function" == typeof WebImageFXEditCommandStart)
	{
		returnValue = WebImageFXEditCommandStart(sEditorName, strcmdname);
	}
	if (typeof WebImageFX.oneditcommandstart != "undefined")
	{
		WebImageFX.initEvent("oneditcommandstart");		
		WebImageFX.raiseEvent("oneditcommandstart");
	}
}
function onImageErrorHandler(strerrorid, strerrdesc, strimagename, strcmdname)
{
	var returnValue = true;
	var sEditorName = WebImageFX.event.srcName;
	
	if ("function" == typeof WebImageFXImageError)
	{
		returnValue = WebImageFXImageError(sEditorName, strerrorid, strerrdesc, strimagename, strcmdname);
	}
	if (typeof WebImageFX.onimageerror != "undefined")
	{
		WebImageFX.initEvent("onimageerror");		
		WebImageFX.raiseEvent("onimageerror");
	}
}
function onLoadingImageHandler(strimagename, strsavefilename, stroldimagename, strsavename)
{
	alert("onLoadingImageHandler In");
	var returnValue = true;
	var sEditorName = WebImageFX.event.srcName;
	
	if ("function" == typeof WebImageFXLoadingImage)
	{
		returnValue = WebImageFXLoadingImage(sEditorName, strimagename, strsavefilename, stroldimagename, strsavename);
	}
	if (typeof WebImageFX.onloadingimage != "undefined")
	{
		WebImageFX.initEvent("onloadingimage");		
		WebImageFX.raiseEvent("onloadingimage");
	}
}
function onSavingImageHandler(strimagename, strsavefilename)
{
	alert("onSavingImageHandler In");
	var returnValue = true;
	var sEditorName = WebImageFX.event.srcName;
	
	if ("function" == typeof WebImageFXSavingImage)
	{
		returnValue = WebImageFXSavingImage(sEditorName, strimagename, strsavefilename);
	}
	if (typeof WebImageFX.onsavingimage != "undefined")
	{
		WebImageFX.initEvent("onsavingimage");		
		WebImageFX.raiseEvent("onsavingimage");
	}
}
function onUpdateImageHandler(strimagename, strsavefilename)
{
	var returnValue = true;
	var sEditorName = WebImageFX.event.srcName;
	
	if ("function" == typeof WebImageFXUpdateImage)
	{
		returnValue = WebImageFXUpdateImage(sEditorName, strimagename, strsavefilename);
	}
	if (typeof WebImageFX.onupdateimage != "undefined")
	{
		WebImageFX.initEvent("onupdateimage");		
		WebImageFX.raiseEvent("onupdateimage");
	}
}
function onLicenseValidityHandler(strisvalid, strlicense)
{
	var returnValue = true;
	var sEditorName = WebImageFX.event.srcName;
	
	if ("function" == typeof WebImageFXLicenseValidity)
	{
		returnValue = WebImageFXLicenseValidity(sEditorName, strisvalid, strlicense);
	}
	if (typeof WebImageFX.onlicensevalidity != "undefined")
	{
		WebImageFX.initEvent("onlicensevalidity");		
		WebImageFX.raiseEvent("onlicensevalidity");
	}
}
// global array of command handlers indexed by command name.
var WebImageFXExecCommandHandlers = new Array();

WebImageFXExecCommandHandlers["toolbarreset"] = function(sEditorName, strCmdName, strTextData, lData) 
{ 
	if (typeof WebImageFX.ontoolbarreset != "undefined")
	{
		WebImageFX.initEvent("ontoolbarreset");
		WebImageFX.event.type = "toolbarreset"; 
		WebImageFX.event.srcName = sEditorName;
		WebImageFX.raiseEvent("ontoolbarreset");
	}
} 


WebImageFXExecCommandHandlers["cmdmfumedia"] = function(sEditorName, strCmdName, strTextData, lData)
{
	if (!WebImageFX.instances[sEditorName].isEditor())
	{
		return; // TODO write async
	}
	if (WebImageFX.instances[sEditorName].editor.MediaFile().getPropertyBoolean("HandledInternally") == false)
	{
		if ("function" == typeof WebImageFXMediaSelection)
		{
			WebImageFXMediaSelection(sEditorName);
		}
	}
	else
	{
		if ("function" == typeof WebImageFXMediaNotification)
		{
			WebImageFXMediaNotification(sEditorName);
		}
	}
}

