// Copyright 2000-2004, Ektron, Inc.
// Revision Date: 2004-05-10

// Media File Upload and Selection Functionality
// Modify this file to customize file upload capability.

/////////////////////////////////////////////////////////////////////
// Current Built-In Functionality:
//
// If you wish to specify the image and link page paths then
// add your page paths to the 
//     ewep_media_image_page 
// and
//     ewep_media_filelink_page 
// variables.  You can do this here in the JavaScript 
// or set them at run time, since they are
// global variables.
//
// Call: 
//    eWebEditProUseFileLink 
// to add a link to a Data Design file link only element.
//
// Call:
//    eWebEditProUseImageFile 
// to add an image to a Data Design image only element 
// or to the selection of an image in HTML content.
//
// The eWebEditProMediaSelection method determines whether the 
// 
/////////////////////////////////////////////////////////////////////


/////////////////////////////////////////////////////////////////////
// Optionally, place your library selection page URLs into these 
// variables for loading.
//
// Placing values in these variable is ONLY necessary if the entry
// in the configuration data is not sufficient for the needs of the
// editor implementation.
//
// DO NOT use the path macros, such as [eWebEditProPath] for they
// will not work.
//
// You can also add your own URL parameters here, if you need them.

// Assign the image selection page here.  Ex:  ewep_media_image_page = "/webpub/library/popupimagesel.jsp?security=1";
var ewep_media_image_page = "";     

// Assign the Data Design file link page here.  Ex:  ewep_media_filelink_page = "/webpub/library/popuplinksel.jsp";
var ewep_media_filelink_page = "";  
/////////////////////////////////////////////////////////////////////


/////////////////////////////////////////////////////////////////////
// Use to place a selected link into the current file link element.
// Call like this (each string comes from your database or UI):
//    top.opener.eWebEditProUseFileLink("edit_xml", "c:\\test.txt", 
//                "This is a test link", true);
/////////////////////////////////////////////////////////////////////
function eWebEditProUseFileLink(sEditorName, sFilePath, sLinkedText, bLocalFile)
{
    eWebEditProUseFileLink_call(sEditorName, sFilePath, sLinkedText, bLocalFile);
}
/////////////////////////////////////////////////////////////////////
/////////////////////////////////////////////////////////////////////

/////////////////////////////////////////////////////////////////////
// Use to place an image into the current image only element.
// Call like this (each string comes from your database or UI):
//    top.opener.eWebEditProUseFileLink("edit_xml", "c:\\test.txt", true,
//                "This is a test image", 0, 0);  // 0 means use the image's size.
/////////////////////////////////////////////////////////////////////
function eWebEditProUseImageFile(sEditorName, strSrcFileLocation, bLocalFile, 
        strFileTitle, nWidth, nHeight)
{
    eWebEditProUseImageFile_call(sEditorName, strSrcFileLocation, bLocalFile, 
            strFileTitle, nWidth, nHeight);
}
/////////////////////////////////////////////////////////////////////
/////////////////////////////////////////////////////////////////////




/////////////////////////////////////////////////////////////////////
/////////////////////////////////////////////////////////////////////

// This is called automatically by the eWebEditPro core JavaScript.
// You do not need to call it.
function eWebEditProMediaSelection(sEditorName, sText, lData)
{
	var objMedia = null;
    var XferMethod = "";
    var sPageLoad = ""; 
	
	// Error check
	if(("string" == typeof sEditorName) && (sEditorName.length > 0))
	{
		var objInstance = eWebEditPro.instances[sEditorName];
		if(("object" == typeof objInstance) && (null != objInstance))
		{
			objMedia = objInstance.editor.MediaFile();
		}
	}
	if(null == objMedia) return;

    
    if((ewep_media_image_page.length > 0) || (ewep_media_filelink_page.length > 0))
    {
        // If pages were externally defined, then we will call them.
        switch(lData)
        {
            case 1:
                // 1 denotes a link selection.
                XferMethod = ewep_media_filelink_page;
                break;
                
            default:
                // 0 denotes an image selection.
                XferMethod = ewep_media_image_page;
                break;
        }
    }
    
    // If we weren't given something then get from the property.
    if(XferMethod.length == 0)
    {
    	// The transfer method specifies what to load for the transfer.
    	XferMethod = objMedia.getPropertyString("TransferMethod");
    }
    
    sPageLoad = XferMethod; 	
    if (sPageLoad.indexOf("?") < 0) // no ? in the string
	{
		sPageLoad += "?";
	}
	else
	{
		sPageLoad += "&";
	}	
	switch(lData)
    {
		
        case 1:
            // 1 denotes a link selection.
            sPageLoad += "scope=files";
            break;
        case 0:
            // 0 denotes an image selection.
            sPageLoad += "scope=images";			
			break;
        default:
            sPageLoad += "scope=all";
            break;
    }
    //sPageLoad = XferMethod; 	
    if (sPageLoad.indexOf("?") < 0) // no ? in the string
	{
		sPageLoad += "?";
	}
	else
	{
		sPageLoad += "&";
	}
	
	sPageLoad += 'editorname=' + escape(sEditorName) + '&upload=' + escape(objMedia.getPropertyBoolean("AllowUpload"));
    if("string" == typeof(sText))
    {
        if(sText.length > 0)
            sPageLoad += '&text=' + escape(sText);
    }
    if("number" == typeof(lData))
    {
        sPageLoad += '&ldata=' + escape(lData);
    }
    
    if(XferMethod != "")
	{
    	if("function" == typeof eWebEditPro.instances[sEditorName].blur)
    		eWebEditPro.instances[sEditorName].blur();


		var oWin = window.open(sPageLoad, 'Images', "scrollbars,resizable,width=640,height=480");
		if (null == oWin && eWebEditProMessages.popupBlockedMessage)
		{
			alert(eWebEditProMessages.popupBlockedMessage);
		}
	}
	else
	{
		alert('The Transfer Method value is empty.  Please specify either "FTP" or a site address that will handle the file selection.');
	}
}

function eWepExtsContainsNonImage(sExt)
{
    var sImgExts = "|gif|jpg|jpeg|jpe|png|tif|tiff|bmp|tga|emf|wmf|img|pic|pcx|";
    var arySentExts = sExt.split(",");
    var idx;
    var bOutside = false;
    
    for(idx = 0; idx < arySentExts.length; idx++)
    {
        if(sImgExts.indexOf("|" + arySentExts[idx] + "|") == -1)
        {
            bOutside = true;
        }
    }
    
    return(bOutside);
}

function eWebEditProUseImageFile_call(sEditorName, strSrcFileLocation, bLocalFile, 
        strFileTitle, nWidth, nHeight)
{
	if(("string" == typeof sEditorName) && (sEditorName.length > 0))
	{
		var objInstance = eWebEditPro.instances[sEditorName];
		if(("object" == typeof objInstance) && (null != objInstance))
		{
			objInstance.insertMediaFile(strSrcFileLocation, bLocalFile, 
        		strFileTitle, "IMAGE", nWidth, nHeight);
		}
	}
}

function eWebEditProUseFileLink_call(sEditorName, sFilePath, sLinkedText, bLocalFile)
{
    // This must be called from the popup with the line below.
	//top.opener.eWebEditProUseFileLink(sEditorName, sFilePath, sLinkedText, bLocalFile);

	setTimeout('eWebEditProFileLinkFromPopup_Deferred(' + 
            toLiteral(sEditorName) + ', ' +
			toLiteral(sFilePath) + ', ' + 
            bLocalFile + ', ' + 
            toLiteral(sLinkedText) + ')', 1);
}

function eWebEditProFileLinkFromPopup_Deferred(sEditorName, strSrcFileLocation, bLocalFile, strLinkedText)
{
    if(eWebEditPro.isEditor(sEditorName))
    {
        var objEditor = eWebEditPro.instances[sEditorName];
        var objMedia = objEditor.editor.MediaFile();
    	if(objMedia != null)
        {
            // Set the basic settings.
        	objMedia.setProperty("IsLocal", bLocalFile);
        	objMedia.setProperty("SrcFileLocationName", strSrcFileLocation);
        	objMedia.setProperty("FileTitle", strLinkedText);
            
            eWebEditPro.instances[sEditorName].editor.ExecCommand("cmdmfumedia", strSrcFileLocation, 1);
        }
    }
}
