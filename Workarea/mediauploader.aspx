<%@ Page Language="C#" ValidateRequest="false" AutoEventWireup="true" Inherits="mediauploader"
    CodeFile="mediauploader.aspx.cs" %>
<%@ Register tagprefix="ektron" tagname="ContentDesigner" src="controls/Editor/ContentDesignerWithValidator.ascx" %>
<%@ Reference Control="controls/library/MediaUploaderCommon.ascx" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>mediauploader</title> 
    <meta http-equiv="content-type" content="text/html; charset=UTF-8" />
    <meta name="GENERATOR" content="Microsoft Visual Studio .NET 7.1"/>
    <meta name="CODE_LANGUAGE" content="Visual Basic .NET 7.1"/>
    <meta name="vs_defaultClientScript" content="JavaScript"/>
    <meta name="vs_targetSchema" content="http://schemas.microsoft.com/intellisense/ie5"/>
    <meta http-equiv="Pragma" content="no-cache" />
    <asp:literal id="jsStyleSheet" runat="server"/>
    

    <script type="text/javascript">
	<!--//--><![CDATA[//><!--
	    var svr_sEditor = '<asp:literal id="jsEditor" runat="server"/>';
	    var svr_LibType = '<asp:literal id="jsLibType" runat="server"/>';
	    var svr_folder = '<asp:literal id="jsFolder" runat="server"/>';
	    var svr_retfield = '<asp:literal id="qsRetfield" runat="server"/>';
	    var svr_SitePath = '<asp:literal id="jsSitePath" runat="server"/>';
	    var svr_LinkText = '<asp:literal id="jsLinkText" runat="server"/>';
	    var svr_ImageExtensions = '<asp:literal id="jsImageExtensions" runat="server"/>';
	    var svr_FileExtensions = '<asp:literal id="jsFileExtensions" runat="server"/>';
	    var svr_SelectLocalFile = '<asp:literal id="jsSelectLocalFile" runat="server"/>';
	    var svr_SelectFolder = '<asp:literal id="jsSelectFolder" runat="server"/>';
	    var svr_MakeSelection = '<asp:literal id="jsMakeSelection" runat="server"/>';
	    var svr_EditorClosed2 = '<asp:literal id="jsEditorClosed2" runat="server"/>';
	    
		if (parent.GetSelectedLibraryItem) {
		    var SelectedItem = parent.GetSelectedLibraryItem();
		}

		var m_jsIsMacNoIE = false;
		var eLibDesc;
		
		function EditorInsert(scope, update) {
		    var bContentDesigner = false;
		    if (typeof parent.window.radWindow != "undefined")
		    {
		        bContentDesigner = true;
		    }
		    if ((top.opener && !top.opener.closed) || true == bContentDesigner){	
				if (update) {
					//DEBUG: alert(SelectedItem.ID + ", " + SelectedItem.FolderID + ", " + SelectedItem.Type + ", " + SelectedItem.Title + ", " + SelectedItem.FileName);
					
					if ((document.forms.LibraryItem.frm_folder_overwritepermission.value == 0)
						|| (document.forms.LibraryItem.frm_folder_overwritepermission.value == "")) {
							alert('<asp:literal id="jsOverwriteItemDenied" runat="server"/>');
							return false;
					}
					if ((Trim(SelectedItem.Title) == "") 
							|| (Trim(SelectedItem.FileName) == "")
							|| (SelectedItem.ID == "")
							|| (document.forms.LibraryItem.frm_folder_overwritepermission.value == 2)) {
						alert('<asp:literal id="jsChooseItemToOverwrite" runat="server"/>');
						return false;
					}
					document.forms.LibraryItem.frm_library_id.value = SelectedItem.ID;
					if ((SelectedItem.Type != "images")
							&& (SelectedItem.Type != "files")) {
							var jsChooseImgFile = '<asp:literal id="jsChooseImgFileToOverwrite" runat="server"/>';
							alert(jsChooseImgFile + " " + SelectedItem.Type);
							return false;
					}
					document.forms.LibraryItem.frm_title.value = Trim(document.forms.LibraryItem.frm_title.value);
					if ((document.forms.LibraryItem.frm_title.value != "") && (document.forms.LibraryItem.frm_title.value != Trim(SelectedItem.Title)))	{
						if (!confirm('<asp:literal id="jsCannotOverwrite" runat="server"/>')) {
							return false;
						}
					}
					document.forms.LibraryItem.frm_title.value = SelectedItem.Title;
					if (Trim(document.forms.LibraryItem.frm_filename.value) == "") {
						alert(svr_SelectLocalFile);
						return false;
					}
					document.forms.LibraryItem.hidden_filename.value = SelectedItem.FileName;
					if  ((CheckUpdateType())
								&& (CheckLibraryForm())) {
						if (confirm('<asp:literal id="jsWarnOverwrite" runat="server"/>')) {
                            var sEditor = svr_sEditor;
						    if (true == bContentDesigner)
						    {
                                sEditor = "ContentDesigner";
						    }
							document.forms.LibraryItem.action = "mediauploader.aspx?action=overwritelibraryitem&EditorName=" + sEditor + "&scope=" + scope + "&type=" + svr_LibType + "&folder=" + svr_folder;
							return true;
						}
						else {
							return false;
						}
					}
				}
				else {
				    var retFiled = "";
			        retField = svr_retfield;
			        sitePath = svr_SitePath;
			        
					if ((Trim(document.forms.LibraryItem.frm_title.value) == "") && (Trim(document.forms.LibraryItem.frm_filename.value) == "") && (document.forms.LibraryItem.frm_insert_server_file.value == ""))  {
						alert(svr_SelectLocalFile);
						return false;
					}
					else if ((Trim(document.forms.LibraryItem.frm_title.value) != "") && (Trim(document.forms.LibraryItem.frm_filename.value) == "") && (document.forms.LibraryItem.frm_insert_server_file.value == ""))  {
						alert(svr_SelectLocalFile);
						return false;
					}
					else if ((document.forms.LibraryItem.frm_folder_id.value == "") || (document.forms.LibraryItem.frm_libtype.value == "")){
						alert(svr_SelectFolder);
						return false;
					}
					else if ((Trim(document.forms.LibraryItem.frm_title.value) == "") && (Trim(document.forms.LibraryItem.frm_filename.value) == "") && (!document.forms.LibraryItem.frm_insert_server_file.value == "")) {
					    if (retField != '')
			            {
			                retField = parent.opener.document.getElementById(retField);
			                if (eval(retField) != null)
			                {
			                    retField.value = m_FileName.replace(sitePath, '');
					            parent.close();
			                }
			            }
			            else
			            {			   
						    InsertFunction(document.forms.LibraryItem.hidden_filename.value, document.forms.LibraryItem.hidden_title.value, document.forms.LibraryItem.frm_libtype.value);
						}
						return true;
					}
					if  ((CheckFileType(document.forms.LibraryItem.frm_libtype.value, scope))
								&& (CheckLibraryForm())
								&& (CheckAddSubmission(document.forms.LibraryItem.frm_libtype.value, scope))) {
						if (retField != '')
						{
						    retField = "&retfield=" + retField;
						}
							document.forms.LibraryItem.action = "mediauploader.aspx?action=uploadlibraryitem&EditorName=" + svr_sEditor + "&scope=" + scope + "&type=" + svr_LibType + "&folder=" + svr_folder + retField + svr_LinkText;
							return true;
						//}
						//else {
						//	return false;
						//}
					}
					return false;
				}
			}
			else {
				alert(svr_EditorClosed2);
				return false;
			}		
		}

		function CheckLibraryForm() {
			var regexp1 = /"/gi;
			document.forms.LibraryItem.frm_title.value = document.forms.LibraryItem.frm_title.value.replace(regexp1, "'");
			if (Trim(document.forms.LibraryItem.frm_title.value) == "") {
				alert ('<asp:literal id="jsLibTitleReq" runat="server"/>');
				document.forms.LibraryItem.frm_title.focus();
				document.forms.LibraryItem.frm_title.value = Trim(document.forms.LibraryItem.frm_title.value);
				return false;
			}
			if (Trim(document.forms.LibraryItem.frm_filename.value) == "") {
				alert ('<asp:literal id="jsFilenameReq" runat="server"/>');
				document.forms.LibraryItem.frm_filename.focus();
				return false;
			}
			return true;
		}

		function CheckAddSubmission(LibraryType, scope) {
			if (CheckLibraryForm()) {
				if(((document.forms[0].frm_filename.value.replace(/^.*\\/, '')).indexOf('#') > -1) || ((document.forms[0].frm_filename.value.replace(/^.*\\/, '')).indexOf('&') > -1) ||  ((document.forms[0].frm_filename.value.replace(/^.*\\/, '')).indexOf(';') > -1))
				{
	        		alert("A filename cannot contain '#','&',';'");
				    return false;
				}
				if (CheckFileType(LibraryType, scope)) {
					var VerifiedType = CheckExtensions(scope);
					if ((VerifiedType == "images") || (VerifiedType == "files")) {
						if (VerifiedType != LibraryType) {
							if (VerifiedType == "images") {
								var msg = '<asp:literal id="jsUploadImgWrong" runat="server"/>';
							}
							else {
								var msg = '<asp:literal id="jsUploadFileWrong" runat="server"/>';
							}
							msg += "\n" + '<asp:literal id="jsUploadCorrectFolder" runat="server"/>';
							if (confirm(msg)) {
								if (VerifiedType == "images") {
									document.forms.LibraryItem.frm_libtype.value = "images";
								}
								else {
									document.forms.LibraryItem.frm_libtype.value = "files";
								}
								return true;
							}
							return false;
						}
						return true;
					}
				}
			}
			return false;
		}
		
		function CheckFileType(LibraryType, scope) {
			var VerifiedType = CheckExtensions(LibraryType);
			if ((VerifiedType == "images") || (VerifiedType == "files")) {
				document.forms.LibraryItem.frm_libtype.value = VerifiedType;
				return true;
			}
			if (VerifiedType != "empty") {
				if (VerifiedType == "nouploadimage") {
					alert('<asp:literal id="jsUploadImgDenied" runat="server"/>');
					return false;
				}
				if (VerifiedType == "nouploadfile") {
					alert('<asp:literal id="jsUploadFileDenied" runat="server"/>');
					return false;
				}
				if (VerifiedType == "noupload") {
					alert('<asp:literal id="jsUploadImgFileDenied" runat="server"/>');
					return false;
				}
				if (VerifiedType == "noselection") {
					alert(svr_SelectFolder);
					return false;
				}
				var msg = '<asp:literal id="jsInvalidExt" runat="server"/>';
				msg += "\n\n";
				var msg1 = "";
				if (document.forms.LibraryItem.frm_folder_imagepermission.value == 1) {
					msg1 += '<asp:literal id="jsForImg" runat="server"/>';
					msg1 += "\n";
					msg1 += svr_ImageExtensions;
					msg1 += "\n\n";
				}
				/*if (scope == "all") {*/
					if (document.forms.LibraryItem.frm_folder_filepermission.value == 1) {
						msg1 += '<asp:literal id="jsForFiles" runat="server"/>';
						msg1 += "\n";
						msg1 += svr_FileExtensions;
						msg1 += "\n\n";
					}
				//}
				alert (msg + msg1);
			}
			return false;
		}
		
		function CheckUpdateType() {
			var libExtensions = SelectedItem.FileName.split(".");
			var fileExtensions = document.forms.LibraryItem.frm_filename.value.split(".");
			var fileExtensionspath = fileExtensions[fileExtensions.length - 1];
			var libExtensionspath = libExtensions[libExtensions.length - 1];
			if(fileExtensionspath.indexOf("?") != -1)
			    fileExtensionspath = fileExtensionspath.substring(0,fileExtensionspath.indexOf("?"));
			if(libExtensionspath.indexOf("?") != -1)
			    libExtensionspath = libExtensionspath.substring(0,libExtensionspath.indexOf("?"));    
			if (fileExtensionspath != libExtensionspath) {
			    var jsErrExt = '<asp:literal id="jsErrExtOverwrite" runat="server"/>';
			    var jsLibExt = '<asp:literal id="jsLibFileExt" runat="server"/>';
				alert(jsErrExt + fileExtensionspath + "\n\n" + jsLibExt + " " + libExtensionspath);
				return false;
			}
			if (IsExtensionValid(document.forms.LibraryItem.frm_libtype.value, document.forms.LibraryItem.frm_filename.value)) {
				return true;
			}
			return false;
		}

		function IsExtensionValid(libType, filename) {
			if (libType == "images") {
				var ExtensionList = svr_ImageExtensions;
			}
			else if (libType == "files") {
				var ExtensionList = svr_FileExtensions;
			}
			else if (libType == "all") {
				var ExtensionList = svr_ImageExtensions + "," + svr_FileExtensions;
				alert(ExtensionList);
			}
			if (ExtensionList.length > 0) {
				var ExtensionArray = ExtensionList.split(",");
				var FileExtension = filename.split(".");
				for (var i = 0; i < ExtensionArray.length; i++) {
					if (FileExtension[FileExtension.length - 1].toLowerCase() == Trim(ExtensionArray[i].toLowerCase())) {
						return true;
					}
				}
				return false;
			}
		}
		
		function CheckExtensions(scope) {
			if (Trim(document.forms.LibraryItem.frm_filename.value) == "") {
				return 'empty';
			}
			if ((document.forms.LibraryItem.frm_folder_imagepermission.value > 1)
					&& (document.forms.LibraryItem.frm_folder_filepermission.value > 1)) {
				return 'noselection';
			}
			if ((document.forms.LibraryItem.frm_folder_imagepermission.value == 0)
					&& (document.forms.LibraryItem.frm_folder_filepermission.value == 0)) {
				return 'noupload';
			}
			if (IsExtensionValid("images", document.forms.LibraryItem.frm_filename.value)) {
				if (document.forms.LibraryItem.frm_folder_imagepermission.value == 1) {
					return 'images';
				}
				else {
					return 'nouploadimage';
				}
			}
			if (Trim(document.forms.LibraryItem.frm_filename.value) == "") {
				return 'empty';
			}
			if (IsExtensionValid("files", document.forms.LibraryItem.frm_filename.value)) {
				if (document.forms.LibraryItem.frm_folder_filepermission.value == 1) {
					return 'files';
				}
				else {
					return 'nouploadfile';
				}
			}
			return 'false';
		}

		function CheckKeyValue(item, keys) {
			var keyArray = keys.split(",");
			for (var i = 0; i < keyArray.length; i++) {
				if ((document.layers) || ((!document.all) && (document.getElementById))) {
					if (item.which == keyArray[i]) {
						return false;
					}
				}
				else {
					if (event.keyCode == keyArray[i]) {
						return false;
					}
				}
			}
		}
		
		function Trim (string) {
			if (string.length > 0) {
				string = RemoveLeadingSpaces (string);
			}
			if (string.length > 0) {
				string = RemoveTrailingSpaces(string);
			}
			return string;
		}

		function RemoveLeadingSpaces(string) {
			while(string.substring(0, 1) == " ") {
				string = string.substring(1, string.length);
			}
			return string;
		}

		function RemoveTrailingSpaces(string) {
			while(string.substring((string.length - 1), string.length) == " ") {
				string = string.substring(0, (string.length - 1));
			}
			return string;
		}
		
		function PreviewFunct(oldURL){
			var regexp1 = / /gi;
			if (window.navigator.userAgent.indexOf("Gecko") > -1) {
				alert('<asp:literal id="jsNoLocalPreview" runat="server"/>');
				return false;
			}
			if (document.forms.LibraryItem.frm_filename.value.length == 0) {
				alert(svr_MakeSelection);
				return false;
			}
			else {
				regexp2 = /\\/gi;
				var tempHREF = 'file:///' + document.forms.LibraryItem.frm_filename.value.replace( regexp1, '%20');
				var tempHREF = tempHREF.replace(regexp2, "/");
			}
			for (var i = 0; i < document.links.length; i++) {
				if (document.links[i].href == oldURL) {
					break;
				}
			}
			document.links[i].href = tempHREF;
			return true;
		}			
		function previewImage(scope, fileInfo) {
			var filename = "";
			//create the path to your local file
			if (fileInfo == null) {
				if (document.forms.LibraryItem.frm_filename.value != "") {
					filename = "file:///" + document.forms.LibraryItem.frm_filename.value;
				}
			} else {
				filename = fileInfo;
			}
			//check if there is a value
			if (filename == "") {
				alert(svr_MakeSelection);
				document.forms.LibraryItem.frm_filename.focus();					
			} else {
				//create the popup 
				popup = window.open('', 'imagePreview', 'width=600,height=450,left=100,top=75,screenX=100,screenY=75,scrollbars,location,menubar,status,toolbar,resizable=1');
				//start writing in the html code
				popup.document.writeln("<html><body bgcolor='#FFFFFF'>");
				//get the extension of the file to see if it has one of the image extensions					
				var VerifiedType = CheckExtensions(scope);
				if (VerifiedType == "images") {					
					popup.document.writeln("<img src='" + filename + "'>");
				}
				else {
					//if not extension fron list above write URL to file 
					popup.document.writeln("<a href='" + filename + "'>" + filename + "</a>");
				}
				popup.document.writeln("</body></html>");
				popup.document.close();
				popup.focus();
			}
		}
		function SubmitForm(FormName, Validate, ReqVal) 
		{
			if(ReqVal == true)
			{
				if (!ValidateMeta(FormName))
				{
					return false;
				}
			}		
			if (Validate.length > 0) 
			{
				if (eval(Validate)) 
				{	
					document.forms[0].submit();
					return false;
				}
				else 
				{
					return false;
				}
			}
			else 
			{
				document.forms[0].submit();
				return false;
			}
		}
		
		function InsertDeferredUpload(scope) { //sContentEditor, sSummaryEditor) {
			if (!top.opener.closed || "object" == typeof parent.window.radWindow) 
			{
					var strFileName = Trim(document.forms.LibraryItem.frm_filename.value);
					var strFolderID = document.forms.LibraryItem.frm_folder_id.value;
					var strTitle = Trim(document.forms.LibraryItem.frm_title.value);
					var strLibType = document.forms.LibraryItem.frm_libtype.value;
					var strSummary = "";
					var retFiled = svr_retfield;
			        sitePath = svr_SitePath;
			        var editor = Ektron.ContentDesigner.instances["content_teaser"];
			        strSummary = editor.getContent();
					
					if (!ValidateMeta('LibraryItem'))
					{
						return false;
					}
					if ((strTitle == "") && (strFileName == "") && (document.forms.LibraryItem.frm_insert_server_file.value == ""))  {
						alert(svr_SelectLocalFile);
						return false;
					}
					else if ((strTitle != "") && (strFileName == "") && (document.forms.LibraryItem.frm_insert_server_file.value == ""))  {
						alert(svr_SelectLocalFile);
						return false;
					}
					else if ((strFolderID == "") || (strLibType == "")){
						alert(svr_SelectFolder);
						return false;
					}
					else if ((strTitle == "") && (strFileName == "") && (!document.forms.LibraryItem.frm_insert_server_file.value == "")) {
					if (retField != '')
			            {
			                retField = parent.opener.document.getElementById(retField);
			                if (eval(retField) != null)
			                {
			                    retField.value = m_FileName.replace(sitePath, '');
					            parent.close();
			                }
			            }
			            else
			            {								    
						    InsertFunction(document.forms.LibraryItem.hidden_filename.value, document.forms.LibraryItem.hidden_title.value, strLibType);
						}
						return true;
					}
					if  ((CheckFileType(strLibType, scope))
								&& (CheckLibraryForm())
								&& (CheckAddSubmission(strLibType, scope))) {

						// clean the file name
						strFileName = strFileName.replace(/ /g, "%20");
						strFileName = strFileName.replace(/\\/g, "/");

						// get metadta/custom-search-field data:
						var metaSearchFields = ProcessMetadata(); // use original slashes
						
						// call code to move data to editor:
						//InsertFunctionLocalFile(insertvalue, title, type, metaData,teaser);
						InsertFunctionLocalFile(strFileName, strTitle, strLibType, metaSearchFields, strSummary);
						//alert('strFileName=' + strFileName + ', strTitle=' + strTitle + ', strLibType=' + strLibType + ', metaSearchFields=' + metaSearchFields + ', strSummary=' + strSummary);
						
						// Done, close this window:
						parent.close();
						
						return true;
					}
					return false;
			}
			else {
				alert(svr_EditorClosed2);
				return false;
			}		
		}
		
	function ProcessMetadata(){
		var strMetaData = '';
		var objForm = document.forms.LibraryItem;

		if (objForm)
		{
			var MetadataDelimiter = "@@ekt@@";   
			var objValidCounter = objForm.frm_validcounter;
			if (objValidCounter){
				var numMetaData = parseInt(objValidCounter.value, 10);

				for (var iCounter = 1; iCounter <= numMetaData; iCounter++)
				{
					var objElem = objForm.elements['frm_meta_type_id_' + iCounter];
					if (objElem)
					{
						strMetaData += objElem.value;
						strMetaData += MetadataDelimiter;
						strMetaData += objForm.elements['frm_text_' + iCounter].value;
						strMetaData += MetadataDelimiter;
					}
				}
			}
		}
				
		return (strMetaData);
		}
		
	function InsertFunctionLocalFile(insertvalue, title, type, metaData,teaser)
	{
		insertvalue = Trim(insertvalue);
		var path = insertvalue.replace(/\\/, "\\\/");	
		var bContentDesigner = false;
		if (typeof parent.window.radWindow != "undefined")
		{
		    bContentDesigner = true;
		}
		if ((top.opener && !top.opener.closed) || true == bContentDesigner){
			if (!document.all && document.getElementById) 
			{
				var typename = "function";
			}
			else 
			{
				var typename = "object";
			}
			var selectedText = "";
			var textsection = "content_teaser";
			if (false == bContentDesigner)
			{
			    selectedText = top.opener.eWebEditPro[textsection].getSelectedHTML();
			    if (typeof(top.opener.eWebEditPro["content_html"]) == typename) 
			    {
			        textsection = "content_html";
			    }				
			}
			if (type == "images")
			{
				insertvalue = replaceAll(insertvalue, "%20", " ");
				path = insertvalue; // = replaceAll(insertvalue, " ", "%20");
				if (true == bContentDesigner)
				{
				    parent.CloseRadDlg(insertvalue, title, type);	// content designer callback function will handle the rest.
				}
				else
				{
				    top.opener.eWebEditPro.instances[textsection].insertMediaFile(path, true, title,"IMAGE",0,0);
				    top.opener.eWebEditPro.instances[textsection].editor.MediaFile().AutomaticUpload().AddFileForUpload(insertvalue, title);
				    top.opener.eWebEditPro.instances[textsection].editor.MediaFile().AutomaticUpload().AddNamedData(insertvalue, "meta_data", metaData);
				    top.opener.eWebEditPro.instances[textsection].editor.MediaFile().AutomaticUpload().AddNamedData(insertvalue, "teaser", teaser);
				}
				//alert('path=' + path + ', title=' + title + ', metaData=' + metaData + ', teaser=' + teaser + ", insertvalue=" + insertvalue + ".");
			}
			else if (type == "hyperlinks")
			{
				if ((insertvalue.substring(0, 7) != "http://") && (insertvalue.substring(0, 8) != "https://")) {
					insertvalue = "http://" + insertvalue; 
				}
				if (selectedText == "") {
					var stuff = '<a href="' + insertvalue + '" title="' + title + '">' + title + '</a>';
				}
				else{
					var stuff = '<a href="' + insertvalue + '" title="' + title + '">' + selectedText + '</a>';
				}
				if (true == bContentDesigner)
				{
				    parent.CloseRadDlg(insertvalue, title, type);	// content designer callback function will handle the rest.
				}
				else
				{
				    top.opener.eWebEditPro[textsection].pasteHTML(stuff);
				    top.close();
				}
			}
			else{ 
				if (selectedText == ""){
					var stuff = '<a href="' + insertvalue + '" title="' + title + '">' + title + '</a>';
				}
				else{
					var stuff = '<a href="' + insertvalue + '" title="' + title + '">' + selectedText + '</a>';
				}
				if (true == bContentDesigner)
				{
				    parent.CloseRadDlg(insertvalue, title, type);	// content designer callback function will handle the rest.
				}
				else
				{
				    top.opener.eWebEditPro[textsection].pasteHTML(stuff);
				    top.close();
				}
			}
			
		}
		else{
			alert(svr_EditorClosed2);
			return false;
		}		
	}

	function replaceAll(inStr, searchStr, replaceStr){
		var retStr = inStr;
		var index = retStr.indexOf(searchStr);
		while(index>=0){
			retStr = retStr.replace(searchStr, replaceStr);
			index = retStr.indexOf(searchStr);
		}
		return (retStr);
	}

	function Trim (string) {
		if (string.length > 0) {
			string = RemoveLeadingSpaces (string);
		}
		if (string.length > 0) {
			string = RemoveTrailingSpaces(string);
		}
		return string;
	}
	
	function RemoveLeadingSpaces(string) {
		while(string.substring(0, 1) == " ") {
			string = string.substring(1, string.length);
		}
		return string;
	}

	function RemoveTrailingSpaces(string) {
		while(string.substring((string.length - 1), string.length) == " ") {
			string = string.substring(0, (string.length - 1));
		}
		return string;
	}


	//--><!]]>
    </script>

</head>
<% if(!IsMembershipUser) { %>
<body class="library" onload="if (parent.SetLoadStatus) {parent.SetLoadStatus('uploader')};">
<% } %>
<% else  {%>
<body class="library">
<% } %>
    <form id="LibraryItem" name="LibraryItem" method="post" runat="server">
        <div id="dhtmltooltip"></div>
        <div class="ektronPageHeader">
            <div class="ektronTitlebar" id="divTitleBar" runat="server"></div>
            <div class="ektronToolbar" id="divToolBar" runat="server"></div>
        </div>
        <div class="ektronPageContainer ektronPageInfo">
            <table class="ektronForm">
                <tr>
                    <td class="label" title="Title">
                        <%=m_refMsg.GetMessage("generic title label")%>
                    </td>
                    <td class="value">
                        <input type="text" size="20" maxlength="200" name="frm_title" onkeypress="javascript:return CheckKeyValue(event,'34');"/>
                    </td>
                </tr>
                <tr>
                    <td class="label" title="Filename">
                        <%=m_refMsg.GetMessage("filename label")%>
                    </td>
                    <td class="value">
                        <input type="file" size="50" maxlength="255" id="frm_filename" name="frm_filename"
                            onkeypress="javascript:return CheckKeyValue(event,'34');" onclick="javascript:CheckFileType1();"
                            runat="server"/>
                        <input type="hidden" name="hidden_title"/>
                        <input type="hidden" name="hidden_filename"/>
                        <input type="hidden" name="frm_content_id"/>
                        <input type="hidden" name="frm_library_id"/>
                    </td>
                </tr>
                <tr>
                    <td class="label" title="Description">
                    <% if (!IsMembershipUser){ %>
                        <%=m_refMsg.GetMessage("description label")%>
                    <% } %>
                    </td>
                    <td class="value">
                        <asp:PlaceHolder ID="EditSummaryHtml" runat="server" />
                    </td>
                </tr>
                <tr>
                    <td class="label" title="Metadata">
                        <%=m_refMsg.GetMessage("content metadata label")%>
                    </td>
                    <td class="value">
                        <asp:Literal ID="litCustomMeta" runat="server" />
                    </td>
                </tr>
            </table>
        </div>
        <asp:PlaceHolder ID="DataHolder" runat="server"></asp:PlaceHolder>
        <asp:literal id="JSInsertFn" runat="server"/>
        <input type="hidden" name="frm_folder_imagepermission" value="1"/>
        <input type="hidden" name="frm_folder_filepermission" value="1"/>
        <input type="hidden" name="frm_folder_overwritepermission" value="1"/>
        <input type="hidden" name="frm_folder_id" value="<%=m_folder%>"/>
        <input type="hidden" name="frm_folder_name"/>
        <input type="hidden" name="frm_insert_server_file"/>
        <input type="hidden" name="frm_libtype" value="<%=(m_LibType)%>"/>
        <input type="hidden" name="frm_scope" value="<%=(scope)%>"/>
        <input type="hidden" name="netscape" onkeypress="javascript:return CheckKeyValue(event,'34,13');"/>
        <input type="hidden" name="frm_update" value="0"/>

        <script type="text/javascript">
		<!--//--><![CDATA[//><!--
			function CheckFileType1(){
				CheckFileType('<%=m_LibType%>', '<%=scope%>');
			}
			document.forms.LibraryItem.frm_filename.onkeypress = document.forms.LibraryItem.netscape.onkeypress;
			document.forms.LibraryItem.frm_title.onkeypress = document.forms.LibraryItem.netscape.onkeypress;
		//--><!]]>
        </script>

    </form>
</body>
</html>

