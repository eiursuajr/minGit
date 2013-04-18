/* Modify this file to implement your custom event handler. */
/*
	See the Developer's Reference Guide for details.
	
	To access the editor as soon as it is ready, remove the comments around this function:
	function eWebEditProReady(sEditorName) { }
	You can also set eWebEditPro.onready.

	To add your own commands, define your own handler function:
	eWebEditProExecCommandHandlers[your_cmd_here] = function(sEditorName, strCmdName, strTextData, lData) { }
	or, remove the comments around this function:
	function eWebEditProExecCommand(sEditorName, strCmdName, strTextData, lData) { }

	To add your own double-click element handler, remove the comments around one or more of the following:
	function eWebEditProDblClickElement(oElement) { }
	function eWebEditProDblClickHyperlink(oElement) { }
	function eWebEditProDblClickImage(oElement) { }
	function eWebEditProDblClickTable(oElement) { }
*/
var CommentPopUpPage="commentpopup.htm";
var CommentSaveType="";
var ValidationPopUpPage="validation_main.aspx";
var docid="";
var folderId = "";

/*
function eWebEditProReady(sEditorName)
{
	// This is called by ewebeditproevents.js, if you have defined this function,
	// when each eWebEditPro signals that it is ready to interact with the scripts
	// and the end user.
	// This function is called for every instance of the editor on a page.
}
*/

eWebEditProExecCommandHandlers["cmdinsertwmv"] = function(sEditorName, strCmdName, strTextData, lData) 
{
	var lToolBar = "toolbar=0,location=0,directories=0,,menubar=0,scrollbars=1,resizable=1,width=" + 400 + ",height=" + 200;
		ewebchildwin = window.open(eWebEditProPath + 'WindowsMediaVideo.aspx?content=Insert%20WMV' + "&EditorName=" + sEditorName, 'Images', lToolBar);
} 

eWebEditProExecCommandHandlers["cmdlibrary"] = function(sEditorName, strCmdName, strTextData, lData) 
{ 
	var lToolBar = "toolbar=0,location=0,directories=0,,menubar=0,scrollbars=1,resizable=1,width=" + 760 + ",height=" + 580;
		ewebchildwin = window.open('mediamanager.aspx?actiontype=library&scope=all&autonav=' + escape(AutoNav) + "&EditorName=" + sEditorName, 'Images', lToolBar);
} 

eWebEditProExecCommandHandlers["jshyperlink"] = function(sEditorName, strCmdName, strTextData, lData)
{
	window.open(eWebEditProPath + 'hyperlinkpopup.htm?editorName=' + escape(sEditorName), 'HyperlinkList', '');
}
eWebEditProExecCommandHandlers["jscmscomment"] = function(sEditorName, strCmdName, strTextData, lData) 
{
	showCMSCommentDialog(sEditorName);
}
eWebEditProExecCommandHandlers["jscmswiki"] = function(sEditorName, strCmdName, strTextData, lData) 
{
	cmsWikiLink(sEditorName);
}
eWebEditProExecCommandHandlers["jscmswiki_quick"] = function(sEditorName, strCmdName, strTextData, lData) 
{
	cmsWikiLinkQuick(sEditorName);
}
eWebEditProExecCommandHandlers["jscmstranslate"] = function(sEditorName, strCmdName, strTextData, lData) 
{
	eWebEditPro.instances[sEditorName].save(); // copy content to hidden field now in case images need to be uploaded.
	var lToolBar = "toolbar=0,location=0,directories=0,menubar=0,scrollbars=1,resizable=1,width=408,height=372";
	ewebchildwin = window.open('worldlingo.aspx?LangType=' + eWebEditProLangType + '&DefaultContentLanguage=' + eWebEditProDefaultContentLanguage + '&htmleditor=' + escape(sEditorName) +'&id=' + docid, 'Translation', lToolBar);
}

eWebEditProExecCommandHandlers["jsvalidation"] = function(sEditorName, strCmdName, strTextData, lData)
{
	showValidationDialog(sEditorName);//window.open(eWebEditProPath + '../validation.htm?editorName=' + escape(sEditorName), 'Validation', 'width=450,height=175,resizable,scrollbars,status,titlebar');
}
function wikitrim(str)
{
   return str.replace(/^\s*|\s*$/g,"");
}


function cmsWikiLinkQuick(sEditorName)
{
	if("function" == typeof eWebEditPro.instances[sEditorName].blur)
		eWebEditPro.instances[sEditorName].blur();

	var strTextData = eWebEditPro[sEditorName].getSelectedHTML();
	var strCheckSpace = eWebEditPro[sEditorName].getSelectedText();
	var bEndSpace = false;
	var bStartSpace = false;	
	if (strCheckSpace.length > 0 && (strCheckSpace.charAt(strCheckSpace.length - 1) == " " || strCheckSpace.charAt(strCheckSpace.length - 1) == "\n"))
	{
	    bEndSpace = true;
	}
	
	if (strCheckSpace.length > 0 && (strCheckSpace.charAt(0) == " " || strCheckSpace.charAt(0) == "\n"))
	{
	    bStartSpace = true;
	}
	strTextData = wikitrim(strTextData);
	if ( strTextData == '' ) 
	{
        alert('You must select some text.');
    }
    else
    {
        var myRe = new RegExp ("folderid=\"([^\">])*");
		var ar = myRe.exec(strTextData)
		var wikititle = '';
		var target = '';
		if (ar != null && ar.length > 0)
		{
		    folderId = ar[0].replace("folderid=\"", "");
		} 
		myRe = new RegExp ("wikititle=\"([^\">])*");
		var ar = myRe.exec(strTextData)
		if (ar != null && ar.length > 0)
		{
		    wikititle = "&wikititle=" + ar[0].replace("wikititle=\"", "");
		}  
		myRe = new RegExp ("target=\"([^\">])*");
		var ar = myRe.exec(strTextData)
		if (ar != null && ar.length > 0)
		{
		    target = "&target=" + ar[0].replace("target=\"", "");
		} 
 	    strTextData = '<span class="MakeLink" category="" target="_self" wikititle="' + strTextData + '">' + strTextData;   
 	    if (bStartSpace)
 	    {
 	        strTextData = ' ' + strTextData;
 	    }
 	    if (bEndSpace == true)
 	    {
 	        strTextData = strTextData + '</span> ';  
 	    }
 	    else
 	    {
 	        strTextData = strTextData + '</span>';   
 	    }
 	   //alert(strTextData);
     eWebEditPro[sEditorName].pasteHTML(strTextData);
 	
  }
}
function cmsWikiLink(sEditorName)
{
	if("function" == typeof eWebEditPro.instances[sEditorName].blur)
		eWebEditPro.instances[sEditorName].blur();

	var strTextData = eWebEditPro[sEditorName].getSelectedHTML();
	var strCheckSpace = eWebEditPro[sEditorName].getSelectedText();
	var bEndSpace = false;
	var bStartSpace = false;	
	if (strCheckSpace.length > 0 && (strCheckSpace.charAt(strCheckSpace.length - 1) == " " || strCheckSpace.charAt(strCheckSpace.length - 1) == "\n"))
	{
	    bEndSpace = true;
	}
	
	if (strCheckSpace.length > 0 && (strCheckSpace.charAt(0) == " " || strCheckSpace.charAt(0) == "\n"))
	{
	    bStartSpace = true;
	}
	strTextData = wikitrim(strTextData);
	if ( strTextData == '' ) 
	{
        alert('You must select some text.');
    }
    else
    {
        var myRe = new RegExp ("folderid=\"([^\">])*");
		var ar = myRe.exec(strTextData)
		var wikititle = '';
		var target = '';
		if (ar != null && ar.length > 0)
		{
		    folderId = ar[0].replace("folderid=\"", "");
		} 
		myRe = new RegExp ("wikititle=\"([^\">])*");
		var ar = myRe.exec(strTextData)
		if (ar != null && ar.length > 0)
		{
		    wikititle = "&wikititle=" + ar[0].replace("wikititle=\"", "");
		} 
		myRe = new RegExp ("target=\"([^\">])*");
		var ar = myRe.exec(strTextData)
		if (ar != null && ar.length > 0)
		{
		    target = "&target=" + ar[0].replace("target=\"", "");
		} 
	//if ((strTextData.indexOf('class="MakeLink') > -1) || (strTextData.indexOf('class=MakeLink') > -1))
	//{
	    window.open(eWebEditProPath + 'wikipopup.aspx?editorName=' + escape(sEditorName) + '&FolderID=' + folderId + wikititle + target, 'EditWikiLink', 'toolbar=0,location=0,directories=0,menubar=0,scrollbars=1,resizable=1,width=680,height=385');
	}
//	else if (strTextData != '')
//	{
//	    strTextData = '<span class="MakeLink">' + strTextData;   
//	    if (bStartSpace)
//	    {
//	        strTextData = ' ' + strTextData;
//	    }
//	    if (bEndSpace == true)
//	    {
//	        strTextData = strTextData + '</span> ';  
//	    }
//	    else
//	    {
//	        strTextData = strTextData + '</span>';   
//	    }
//	    eWebEditPro[sEditorName].pasteHTML(strTextData);
//	}
//	else
//	{
//	    alert('You must select some text.');
//	}
}
function showCMSCommentDialog(sEditorName)
{
	if((CommentSaveType=="add") || (CommentSaveType=="AddTask")){
		if (CommentSaveType=="add"){
			alert("Content must be saved before adding comments!");
		}else{
			alert("Task must be saved before adding comments!");
		}
		return false;
	}
	
	if (CommentPopUpPage!="commentpopup.htm")
	{
		eWebEditPro.openDialog(sEditorName, eWebEditPro.resolvePath(CommentPopUpPage+"&editor_name="+sEditorName), "", "",
			"width=650,height=350,resizable,scrollbars,status,titlebar");
	}
	else
	{
		eWebEditPro.openDialog(sEditorName, eWebEditPro.resolvePath(CommentPopUpPage), "", "",
			"width=650,height=350,resizable,scrollbars,status,titlebar");	
	}
}
function showValidationDialog(sEditorName) {
	if (CommentSaveType=="add"){
			alert("Content must be saved before adding validation!");
		return false;
	}
	
	eWebEditPro.openDialog(sEditorName, eWebEditPro.resolvePath(ValidationPopUpPage), "", "",
			"width=425,height=175,resizable,scrollbars,status,titlebar");
}	

eWebEditProExecCommandHandlers["dblclicktag"] = function(sEditorName, strCmdName, strTextData, lData) 
{
    var objXML = eWebEditPro.instances[sEditorName].editor.XMLProcessor();
    if((typeof objXML != "undefined") && (objXML != null))
    {
    	var objXmlTag = objXML.ActiveTag();
    	if((typeof objXmlTag != "undefined") && (objXmlTag != null))
    	{
            if(true == objXmlTag.IsValid())
            {
        		if ("mycomment" == objXmlTag.getPropertyString("TagName"))
        		{
        			showCMSCommentDialog(sEditorName);
        		}
            }
    	}
    }
}
/*
function eWebEditProExecCommand(sEditorName, strCmdName, strTextData, lData)
{
	return true; // false = event handled; true = continue normal processing
}
*/

/*
function eWebEditProDblClickElement(oElement) 
{ 
	var sTagName = oElement.tagName + ""; 
	sTagName = sTagName.toUpperCase(); 

	return true; // false = event handled; true = continue normal processing
}
*/

/*
function eWebEditProDblClickHyperlink(oElement) 
{ 
	return true; // false = event handled; true = continue normal processing
}
*/

/*
function eWebEditProDblClickImage(oElement) 
{ 
	return true; // false = event handled; true = continue normal processing
}
*/

/*
function eWebEditProDblClickTable(oElement) 
{ 
	return true; // false = event handled; true = continue normal processing
}
*/


/*  
	for KB "Resolve URLs when content may be displayed in more than one template in different directories"
	http://dev.ektron.com/kb_article.aspx?id=8372 
	uncomment xsl:include href="ektfiltercustom.xslt" in ektfilter.xslt
	uncomment eWebEditPro.oncreate in customevents.js
	uncomment xsl:template match="a/@href..." below
*/
/*
eWebEditPro.oncreate = function()
{
	eWebEditPro.event.parameters.baseURL = "/AREA/SECTION/";
}
*/

eWebEditProExecCommandHandlers["cmdaddemoticon"] = function(sEditorName) 
{
	cmsAddEmoticon(sEditorName);
}

cmsAddEmoticon = function(sEditorName)
{      
    window.open(ek_tdPath + '/emoticon_select.aspx?editorName=ewebeditpro&sEditorName=' + sEditorName + '&FolderID=0', 'EmoticonSelect', 'toolbar=0,location=0,directories=0,menubar=0,scrollbars=0,resizable=yes,width=260,height=65');
}