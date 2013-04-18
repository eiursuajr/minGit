// JS functions for the CMS edit functionality.

var b_cms_lnReady = true;  // blnReady
var b_cms_lnEnable = true;  // blnEnable

//var g_cms_ua = window.navigator.userAgent.toLowerCase();
//if ((g_cms_ua.indexOf("msie ") > -1) && (!(g_cms_ua.indexOf("opera") > -1))) 
//{
//    b_cms_lnReady = false;
//    b_cms_lnEnable = false;
//}
 
function IsCmsEditEnable() 
{
    return(b_cms_lnEnable);
}


// Ths function is called by the editor when it is ready
// to start processing commands and dynamic settings.
// Do not make toolbar modifications in here.
// If you need to make toolbar modifications at startup, 
// use the editorToolbarReset function.
//
// sEditor    -- The name of the editor, given by the page.
// strURL     -- The URL of the page.
// strAutoURL -- The URL for the automatic upload destination.
// iFolderId  -- The default folder to use for transfers.
// strAutoNav -- the parameter to tell the library where to 
//               navigate when loaded.
function editorOnReady(sEditor, strURL, strAutoURL, iFolderId, strAutoNav)  // Replaces initTransferMethod
{
    b_cms_lnReady=true;
    b_cms_lnEnable=true;
    
    // This previously could count on the fact that this
    // was called from an existing editor.  Now, since there
    // is also a timeout used, we can't rely on the
    // value being valid.    
    if("object" != typeof eWebEditPro)
        return;
        
    var objInstance = eWebEditPro.instances[sEditor];
    if ("object" == typeof objInstance && objInstance != null)
    {
        if (objInstance.isEditor())
        {
            // Do any editor setup, which is outside
            // of the configuration, in here.
        
            if("undefined" != typeof loadSegments)
            {
                loadSegments();
            }

            var objMedia = objInstance.editor.MediaFile();
            if (null != objMedia)
            {
                var strMediaURL = AppendURLParam(strURL, "autonav", strAutoNav); //// AutoNav
                strMediaURL = AppendURLParam(strMediaURL, "defaultFolderId", iFolderId);  //// defaultFolderId
                // Since the element may not be available, we can't perform this check here.
                //if (typeof document.getElementById("Ver4Editor") == "object" && "true" == document.getElementById("Ver4Editor").value.toLowerCase())
                //{
				//	// an indicator for the library to know that it is in Data Entry mode.  So, disable thumbnail in the library.
				//	strMediaURL = AppendURLParam(strMediaURL, "dentrylink", "1");
				//}
				objMedia.setProperty("TransferMethod", strMediaURL);  ////
                var objAutoUpload = objMedia.AutomaticUpload();
                if (null != objAutoUpload)
                {
                    objAutoUpload.setProperty("TransferMethod", strAutoURL);
                    objAutoUpload.SetFieldValue("folder_id", iFolderId);
                    objAutoUpload.SetFieldValue("custom_field_meta", "");
                    objAutoUpload.SetFieldValue("custom_field_teaser", "");
                }
            }
        
            var mapEditorToTabID = [];
            mapEditorToTabID["content_html"] = "dvContent";
            mapEditorToTabID["content_teaser"] = "dvSummary";
            
            var tabID = mapEditorToTabID[sEditor];
            if ("string" == typeof tabID)
            {
                if ("dvContent" == tabID) 
                {
                    bFormEditorReady = true;
                    bContentEditorReady = true;
                }
                if ("dvSummary" == tabID) 
                {
                    bResponseEditorReady = true;
                    bTeaserEditorReady = true;
                }
                if("undefined" != typeof(g_initialPaneToShow))
                {
                    if (false == g_bResettingToolbar)
                    {
                        SetPaneVisible(tabID, g_initialPaneToShow == tabID);
                    }
                    if ("dvContent" == tabID && tabID != g_initialPaneToShow && "2" == document.forms.frmMain.content_type.value) 
                    {
                        // update again once the content editor is ready
                        updateMergeFieldList("content_html", "content_teaser");
                    }
                }
            }
            if("undefined" != typeof g_initialPaneToShow)
            {
                SetPaneVisible("dvPollWizard", g_initialPaneToShow == "dvPollWizard");
            }
        }
    }					
}

// This function is used to modify the toolbar.
// Add functions and toolbars, remove functions and toolbars,
// disable functions and toolbars here.
// There is no need to do any other type of initialization.
// If there is other initialization, then use editorOnReady.
function editorToolbarReset(sEditorName)  // Replaces initTransferMethod
{
    // This previously could count on the fact that this
    // was called from an existing editor.  Now, since there
    // is also a timeout used, we can't rely on the
    // value being valid.    
    if("object" != typeof eWebEditPro)
        return;
        
    if (typeof bPageClosing != "undefined")
    {
		if(bPageClosing) return;
	}
    var objInstance = eWebEditPro.instances[sEditorName];
    if ("object" == typeof objInstance && objInstance != null)
    {
        if (objInstance.isEditor())
        {
            var objToolbar = objInstance.editor.Toolbars();
            if (objToolbar != null)
            {
                // Do any toolbar modifications in here.
            }
        }
        
        if ("function" == typeof frmDesEnableFormDesignDirection)
		{
			//defect #30817: Error or crash during HTML form creation or edit
			//This is the fix for the FireFox part. It takes a long time to load the 2 editors
			//on edit.aspx.  Five seconds is a long period of time but it is the min time required
			//on my test machine. The radio buttons on the response tab can only be enabled
			//after the content_teaser content is loaded. 
			setTimeout('frmDesEnableFormDesignDirection()', 5000);
		}
    }					
}

function initTransferMethod(sEditor, strURL, strAutoURL)  // Obsolete, replaced with editorOnReady and editorToolbarReset
{
    ///////////////////////////////////////
    // Do not use, if at all possible
    ///////////////////////////////////////
    editorOnReady(sEditor, strURL, strAutoURL, defaultFolderId, AutoNav);
    editorToolbarReset(sEditor);    
}

function initDisableUpload(sEditorName, strURL, strAutoURL)
{
    // <asp:literal id="jsDisableUpload" runat="server"/>
    var objMedia = eWebEditPro.instances[sEditorName].editor.MediaFile();
    if(objMedia != null)
    {
        var objAutoUpload = objMedia.AutomaticUpload();
        if(objAutoUpload != null)
        {
            objAutoUpload.setProperty("TransferMethod", "none");
            var objMenu = eWebEditPro.instances[sEditorName].editor.Toolbars();
            if(objMenu != null)
            {
                var objCommand = objMenu.CommandItem("cmdmfuuploadall");
                if(objCommand != null)
                {
                    objCommand.setProperty("CmdGray", true);
                }                
                var objCommand = objMenu.CommandItem("cmdmfuuploadcontent");
                if(objCommand != null)
                {
                    objCommand.setProperty("CmdGray", true);
                }

            }
        }
    }
}

//This CheckContentSize function is used to test the editor content at edit.aspx on the click of 
//save, checkin or publish.  However, when it is eWebEditPro.save, it should use the build-in 
//source code in ewep.js        
function CheckContentSize()
{	
    if ("object"==(typeof eWebEditPro))
    {
        if ("object"==(typeof eWebEditPro.instances["content_html"]))
        {  	
            var obj_Editor=eWebEditPro.instances["content_html"];  // Using this elsewhere
            if(null != obj_Editor.editor)
            {
                var lngContentSize = obj_Editor.editor.EstimateContentSize("text");
                var bSizeExceeded = obj_Editor.isSizeExceeded(lngContentSize);
			    if (bSizeExceeded)
			    {
                    alert("Content is too large to save. Please reduce the size and try again.");
                    return false;
                }
            }
            
            if ("object"==(typeof eWebEditPro.instances["content_teaser"]))
            {
                var obj_EditorT=eWebEditPro.instances["content_teaser"];  // Using this elsewhere
				if(null != obj_EditorT.editor)
				{
					var lngTeaserSize = obj_EditorT.editor.EstimateContentSize("text");
					var bSizeExceeded = obj_EditorT.isSizeExceeded(lngTeaserSize);
					if (bSizeExceeded)
					{
						alert("Summary is too large to save. Please reduce the size and try again.");
						return false;
					}
				}
                
            }
        }
    }					
    if ("function" == typeof Page_ClientValidate) 
    {
		var validationResult = Page_ClientValidate();
		if (!validationResult)
		{
			return false;
		}
	}				
    return true;
}
            
            
function ShutdownImageEditor() 
{
    //**************************************************************
    // Block to support deferred inage upload:
    if(eWebEditPro.instances['content_html'])
    {
        if(null == eWebEditPro.instances['content_html'].editor)
            return;
            
        var objMedia = eWebEditPro.instances['content_html'].editor.MediaFile();
        if(objMedia != null)
        {
            var objAutoUpload = objMedia.AutomaticUpload();
            if(objAutoUpload != null)
            {
                var metaSave = "";
                var TeaserSave = "";
                var path;
                var mask = 63; //returns all files currently...
                var filestoupload = objAutoUpload.ListFilesWithStatus(mask, "|");
                if(filestoupload.length > 0)
                {
                    var filearray  = filestoupload.split("|");
                    for(var i = 0; i < filearray.length; i+=2)
                    {
                        path = filearray[i];
                        path = path.replace(/\\/, "/");
                        path = replaceAll(path, "\\", "/");
                        path = replaceAll(path, "%20 ", " ");
                        metaSave = metaSave + filearray[i] + "|-|" + objAutoUpload.ReadNamedData(path, "meta_data") + "|-|";
                        TeaserSave = TeaserSave + filearray[i] + "|-|" + objAutoUpload.ReadNamedData(path, "teaser") + "|-|";
                    }
                }
                
                objAutoUpload.SetFieldValue("custom_field_teaser", TeaserSave);
                objAutoUpload.SetFieldValue("custom_field_meta", metaSave);
            }
        }
    }
    
    //**************************************************************
    // now we need to check for the image editor and close it before saving
    // this will cause the present edits on the image to save
    if (eWebEditPro.isInstalled) 
    {
        var objInstance = eWebEditPro.instances['content_html'];
        if (objInstance)
        {
            if(null != objInstance.editor)
            {
                var objImageEdit = objInstance.editor.ImageEditor();
                if(null != objImageEdit)
                {
                    if (objImageEdit.IsPresent()) 
                    {
                        if (objImageEdit.IsVisible()) 
                        {
                            objImageEdit.ExecCommand("cmdexit", "", 0);
                        }
                    }
                }
            }
        }
    }
}


function replaceAll(inStr, searchStr, replaceStr)
{
    var retStr = inStr;
    var index = retStr.indexOf(searchStr);
    while(index>=0)
    {
        retStr = retStr.replace(searchStr, replaceStr);
        index = retStr.indexOf(searchStr);
    }
    return (retStr);
}




function AppendURLParam(strURL, strParam, strAddValue)
{
    var strValue = " ";
    if(typeof(strAddValue) == "number")
        strValue = strAddValue.toString();
    else
        strValue = strAddValue;
    if(strValue.length > 0)	
    {
        var sDelim = "?";
        var sAssign = "=";
        if(strURL.indexOf("?") > 0)
            sDelim = "&";
        if(strParam.indexOf("=") > 0)
            sAssign = "";	
        strURL += sDelim + strParam + sAssign + escape(strValue);
    }
    return(strURL);
}
	

		

