// Copyright 2000-2002, Ektron, Inc.
// Revision Date: 2002-05-03

// Media Upload Functionality
// Modify this file to customize file upload capability.

function WebImageFXMediaSelection(sEditorName)
{
	// The transfer method specifies what to load for the transfer.
	var objMedia = WebImageFX.instances[sEditorName].editor.MediaFile();
	var XferMethod = objMedia.getPropertyString("TransferMethod");
	var sPageLoad = XferMethod; 
	if (sPageLoad.indexOf("?") < 0) // no ? in the string
	{
		sPageLoad += "?";
	}
	else
	{
		sPageLoad += "&";
	}	
	
	sPageLoad += 'editorname=' + escape(sEditorName) + '&upload=' + escape(objMedia.getPropertyBoolean("AllowUpload"));
	if(XferMethod != "")
	{
		var oWin = window.open(sPageLoad, 'Images', "scrollbars,resizable,width=640,height=480");
		if (null == oWin && WebImageFXMessages.popupBlockedMessage)
		{
			alert(WebImageFXMessages.popupBlockedMessage);
		}
	}
	else
	{
		alert('The Transfer Method value is empty.  Please specify either "FTP" or a site address that will handle the file selection.');
	}
}

