<%@ Page Language="C#" AutoEventWireup="true" Inherits="isearch" CodeFile="isearch.aspx.cs"
    ValidateRequest="False" %>

<%@ Register TagPrefix="cms" Namespace="Ektron.Cms.Controls" Assembly="Ektron.Cms.Controls" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Search</title>
    <meta content="text/html; charset=UTF-8" http-equiv="Content-Type" />
    <meta http-equiv="Pragma" content="no-cache" />
    <style type="text/css">
        input.button
        {
            display: block;
            background-color: #f5f5f5;
            border: 1px solid #dedede;
            border-top: 1px solid #eee;
            border-left: 1px solid #eee;
            line-height: 100%;
            text-decoration: none;
            color: #565656;
            cursor: pointer;
            padding: .5em 1em .5em 2.25em;
            margin: 0 0 0 .75em;
            background-repeat: no-repeat;
        }
        input.buttonInline
        {
            display: inline;
        }
        input.blueHover:hover
        {
            background-color: #dff4ff;
            border: 1px solid #c2e1ef;
            color: #336699;
        }
        input.buttonSearch
        {
            background-image: url(images/ui/icons/magnifier.png);
            background-position: .6em center;
            margin: .25em;
            padding-top: .25em;
            padding-bottom: .25em;
        }
    </style>

    <script type="text/javascript">
        var lbScope = '<asp:literal id="jsScope" runat="server"/>';
	    var sMediaEditor = '<asp:literal id="jsEditorName" runat="server"/>';
	    var lDEntryLink = '<asp:literal id="jsDEntrylink" runat="server"/>';
	    var flagEnhancedMetaSelect = '';
	    var metadataFormTagId = '';
	    var separator = '';

	    function InsertFunction(insertvalue, title, type, passedId)
	    {	
	        var selectedText = "";
		    if ((parent.frames[0].document) && (parent.frames[0].document.forms[0]))
		    {
			    if (parent.frames[0].document.forms[0].enhancedmetaselect != null)
			        flagEnhancedMetaSelect = parent.frames[0].document.forms[0].enhancedmetaselect.value;
			    if (parent.frames[0].document.forms[0].metadataformtagid != null)
			        metadataFormTagId = parent.frames[0].document.forms[0].metadataformtagid.value;
			    if (parent.frames[0].document.forms[0].separator != null)
			        separator = parent.frames[0].document.forms[0].separator.value;
		    }
    		
		    if (lbScope == '')
		    {
			    lbScope = "all";
		    }
    		
		    var ephox = "false";
    		
		    if ((top.opener != null && typeof top.opener != "undefined" && !top.opener.closed) || "object" == typeof parent.window.radWindow)
		    { 
		        if (flagEnhancedMetaSelect == '')
			    {
			        var sEphoxFieldType = "undefined";
			        try{
				            sEphoxFieldType = typeof(top.opener.document.forms[0].ephox);
				       }
				    catch(e){}
    				
				    if (sEphoxFieldType.toLowerCase() != "undefined")
				    {
					    if (typeof(top.opener.document.forms[0].ephox.value) != "undefined"){
						    ephox = top.opener.document.forms[0].ephox.value.toLowerCase();
					    }
					    if (ephox == "true")
					    {
						    if (("undefined" != typeof top.opener.document.forms[0].selectedtext)
							    && ("undefined" != typeof top.opener.document.forms[0].selectedtext.value))
						    {
							    selectedText = top.opener.document.forms[0].selectedtext.value;
						    }
					    }
				    }
				    var bContentDesigner = false;
                    try
                        {
                            var args = parent.GetDialogArguments();
                            if(args)              
                              bContentDesigner = true;
                          
                        }
                        catch(e)
                        {
                        }
                    
				    if (!document.all && document.getElementById) {
					    var typename = "function";
				    }
				    else {
					    var typename = "object";
				    }
				    if (ephox != "true"){
					    if (sMediaEditor == "") {
						    var textsection = "content_html";
					    }
					    else {
						    var textsection = sMediaEditor;
					    }
				    }
				    if (type == "images")
				    {
				        //alert("insertvalue: " + insertvalue);
					    if (ephox == "true")
					    {	
						    parent.opener.insertImage(insertvalue, title);
						    parent.close();
					    }
				        else if (parent.location.href.indexOf("&productmode=true") >= 0)
				        {
				            // add image to the products' media tab
				            var imagePath = insertvalue;
				            if (imagePath.indexOf("?") > 0)
				                imagePath = imagePath.substr(0, imagePath.indexOf("?"));
				            var id = ""; 
				            if (null != passedId && "undefined" != typeof passedId)
				                id = passedId;
                            var newImageObj = {"id":id,"title":title,"altText":title,"path":imagePath,"width":"0","height":"0"};
    				        
				            var iframeLocation = "./Commerce/CatalogEntry/Media/AddLibraryImage.aspx?productTypeId=" + getQueryVariable("productTypeId") + "&imageId=" + newImageObj.id;
				            var iframe = $ektron("<iframe src=\"" + iframeLocation + "\" style=\"position:absolute;margin-left:-10000px;\" />");
				            $ektron("body").append(iframe);
				        }
				        else if (true == bContentDesigner)
				        {
				            parent.CloseRadDlg(insertvalue, title, type);	// content designer callback function will handle the rest.
				        }
					    else 
					    {
						    parent.opener.eWebEditPro.instances[textsection].insertMediaFile(insertvalue,false,title,"IMAGE",0,0);
						    parent.close();
					    }
				    }
				    else if ((type == "files") && (lbScope != "all")) 
				    {
					    if (ephox == "true")
					    {	
						    parent.opener.InsertHTMLAtCursor(escape("<a href=\"" + insertvalue + "\" alt=\"" + title + "\" title=\"" + title + "\">"));
						    parent.close();
					    }
					    else if (true == bContentDesigner)
				        {
				            parent.CloseRadDlg(insertvalue, title, type);	// content designer callback function will handle the rest.
				        }
					    else 
					    {				
						    parent.opener.eWebEditProUseFileLink(textsection, insertvalue, title, true);
						    parent.close();
					    }
				    }
				    else if (type == "hyperlinks"){
					    if ((insertvalue.substring(0, 7) != "http://") && (insertvalue.substring(0, 8) != "https://") && (insertvalue.substring(0, 1) != "/")) {
						    insertvalue = "http://" + insertvalue; 
					    }
					    if (ephox == "true"){	
						    if (selectedText == "") {
							    parent.opener.insertOther(insertvalue,title);
						    }
						    else {
							    parent.opener.insertOther(insertvalue,selectedText);
						    }
						    parent.close();
					    }
					    else if (true == bContentDesigner)
				        {
				            parent.CloseRadDlg(insertvalue, title, type);	// content designer callback function will handle the rest.
				        }
					    else {
					        if ('1' == lDEntryLink) {
							    parent.opener.eWebEditProUseFileLink(textsection, insertvalue, title, true);
						    }
						    else {
							    selectedText = parent.opener.eWebEditPro[textsection].getSelectedText();
							    if (selectedText == "") {
								    selectedText = parent.opener.eWebEditPro[textsection].getSelectedHTML();
							    }
							    if (selectedText == "") {
								    var stuff = '<a href="' + insertvalue + '" title="' + title + '">' + title + '</a>';
							    }
							    else {
								    var stuff = '<a href="' + insertvalue + '" title="' + title + '">' + selectedText + '</a>';
							    }
							    parent.opener.eWebEditPro[textsection].pasteHTML(stuff);
						    }
						    parent.close();
					    }
				    }
				    else if (type == "quicklinks") { 
					    if (ephox == "true"){	
						    if (selectedText == "") {
							    parent.opener.insertOther(insertvalue,title);
						    }
						    else {
							    parent.opener.insertOther(insertvalue,selectedText);
						    }
						    parent.close();
					    }
					    else if (true == bContentDesigner)
				        {
				            parent.CloseRadDlg(insertvalue, title, type);	// content designer callback function will handle the rest.
				        }
					    else 
					    {
						    if ('1' == lDEntryLink) 
				            {
							    parent.opener.eWebEditProUseFileLink(textsection, insertvalue, title, true);
						    }
						    else 
						    {
							    selectedText = parent.opener.eWebEditPro[textsection].getSelectedText();
							    if (selectedText == "") {
								    selectedText = parent.opener.eWebEditPro[textsection].getSelectedHTML();
							    }
							    if (selectedText == "") {
								    var stuff = '<a href="' + insertvalue + '" title="' + title + '">' + title + '</a>';
							    }
							    else {
								    var stuff = '<a href="' + insertvalue + '" title="' + title + '">' + selectedText + '</a>';
							    }					
							    parent.opener.eWebEditPro[textsection].pasteHTML(stuff);
						    }
						    parent.close();
					    }
				    }
				    else {
					    if (ephox == "true"){	
						    if (selectedText == ""){
							    parent.opener.insertOther(insertvalue,title);
						    }
						    else{
							    parent.opener.insertOther(insertvalue,selectedText);
						    }
						    parent.close();
					    }
					    else if (true == bContentDesigner)
				        {
				            parent.CloseRadDlg(insertvalue, title, type);	// content designer callback function will handle the rest.
				        }
					    else 
					    {	
					        if ('1' == lDEntryLink) 
						    {
							    parent.opener.eWebEditProUseFileLink(textsection, insertvalue, title, true);
						    }
						    else 
						    {
							    selectedText = parent.opener.eWebEditPro[textsection].getSelectedText();
							    if (selectedText == "") {
								    selectedText = parent.opener.eWebEditPro[textsection].getSelectedHTML();
							    }
							    if (selectedText == "") {
								    var stuff = '<a href="' + insertvalue + '" title="' + title + '">' + title + '</a>';
							    }
							    else {
								    var stuff = '<a href="' + insertvalue + '" title="' + title + '">' + selectedText + '</a>';
							    }
							    parent.opener.eWebEditPro[textsection].pasteHTML(stuff);
						    }
						    parent.close();
					    }
				    }
			    }
			    else 
			    {
				    if ((parent.frames) && (parent.frames[2]) && ("MediaSelect" == parent.frames[2].name))
				    {
					    //alert ('insertvalue=' + insertvalue + ', title=' + title + ', metadataFormTagId=' + metadataFormTagId);
					    parent.frames[2].metaselect_addMetaSelectRow(insertvalue, title, metadataFormTagId, separator);
				    }
				    else
				    {
					    // used by Enhanced Metadata selection:
					    if ((parent.opener.ek_ma_ReturnMediaUploaderValue != null) && (typeof(parent.opener.ek_ma_ReturnMediaUploaderValue) != 'undefined'))
					    {
						    if ((metadataFormTagId != '') && (metadataFormTagId > 0))
						    {
							    parent.opener.ek_ma_ReturnMediaUploaderValue(insertvalue, title, metadataFormTagId);
							    parent.opener.CloseChildPage();
						    }
					    }
					    parent.close();
				    }
			    }
		    }
		    else 
		    {
			    alert("<asp:literal id="jsEditorClosed" runat="server"/>");
			    return false;
		    }		
	    }
    	
	    function getQueryVariable(variable) { 
            var query = parent.location.search.substring(1); 
            var vars = query.split("&"); 
            for (var i=0;i<vars.length;i++) { 
                var pair = vars[i].split("="); 
                if (pair[0] == variable) { 
                    return pair[1]; 
                } 
            } 
        } 
	    function InserValueToField(filename, previewPath, sitePath, retFieldID)
	    {
	        var thumbnail = "";
	        var retField = parent.opener.document.getElementById(retFieldID);
            if (eval(retField) != null)
            {
                if (filename.indexOf(sitePath) == 0) {
                    retField.value = filename.replace(sitePath, '');
                } 
                else
                {          
                    retField.value = filename;
                }
                thumbnail = eval(parent.opener.document.getElementById(retFieldID + "_thumb"));
                if (thumbnail != null)
                {
                    thumbnail.src = previewPath;  
                }
                parent.close();
            }
            return false;
	    }
    	
	    function CommerceMediaTabAddLibraryImage(newImageObj)
	    {
	        parent.opener.Ektron.Commerce.MediaTab.Images.addNewImage(newImageObj);
		    parent.close();
	    }
    </script>
</head>
<body id="body" runat="server">
    <%=StyleSheetJS%>
    <%=SearchStyleSheet%>
    <%=SearchJScript%>

    <script type="text/javascript">
       Ektron.ready(function() {
            if ($ektron("table#shared_group").length > 0 && navigator.appName.indexOf("Internet Explorer") != -1) {
                alignTables();
            }
        });
        var pageAction = '<asp:Literal runat="server" id="ltrAction" />';
        if (top && "function" == typeof top.HideDragDropWindow) {
	        top.HideDragDropWindow(); //hide drag drop uploader frame
        }
        var m_LibID ;
        var m_Folder ;
        var m_Title = '';
        var m_FileName = '';
        var m_Type = '';
        var m_PreviewThumbnail = '';

        function SubmitForm() {
	        document.forms[0].submit();
	        return false;
        }

        function ThumbnailForContentImage(thumbnail){
	        m_PreviewThumbnail = thumbnail;
        }

        var  m_Title;

        function Insert_thumb(filename,thumb_filename) {
            var pastevalue;
	        var popupscript;
	        var ephox = "false";
	        var bContentDesigner = false;
	        try {
                var args = parent.GetDialogArguments();
                if(args) {
                    bContentDesigner = true;
	            }
	        }
	        catch(e){}

	        if ("undefined" == typeof m_Title) {
	            m_Title = filename;
	        }

	        var strTitle = $ektron.htmlEncode(m_Title);
	        popupscript = "try{window.open('" + encodeURI(filename) + "', 'MyImage', 'resizable=yes, scrollbars=yes, width=790, height=580')}catch(e){};return false;";
	        pastevalue = '<a href="#" onclick="' + popupscript + '" onkeypress="this.onclick();" title="' + strTitle + '" >';
	        pastevalue += "<img src=\"" + $ektron.htmlEncode(thumb_filename) + "\" border=\"0\" alt=\"" + strTitle + "\" title=\"" + strTitle + "\" /></a>";

	        try {
                var sEphoxFieldType = typeof(top.opener.document.forms[0].ephox);
                if (sEphoxFieldType.toLowerCase() != "undefined") {
                    if (typeof top.opener.document.forms[0].ephox.value != "undefined") {
                        ephox = top.opener.document.forms[0].ephox.value.toLowerCase();
                    }
                }
            }
	        catch(e){}

	        if (ephox == "true") {
		        top.opener.insertHTML(pastevalue);
		        top.close();
	        } else if (bContentDesigner == true)  {
	            //content designer
                parent.CloseRadDlg(pastevalue, m_Title, "thumbnail");
            } else {
		        if (!eWebEditProUtil.isOpenerAvailable()) {
		            alert("Your image could not be inserted because the editor page has been closed.");
		        } else {
			        parent.opener.eWebEditPro.instances['<%=sEditor%>'].editor.pasteHTML(pastevalue);
			        parent.close();
		        }
	        }
        }


        function Insert(libraryid, folder, title, filename, type, contentid){

            var Source = "<%=m_strSource%>";
	        m_LibID = libraryid;
	        m_Folder = folder;
	        m_Title = title;
	        m_FileName = filename;
	        m_Type = type;
			document.forms[0].contentID.value = contentid;
			
	        if (Source == "libinsert") {
		        top.opener.selectLibraryItem(libraryid, folder, title, filename, type);
		        var szPreviewFormType = typeof(document.forms[0]);
		        if (szPreviewFormType.toLowerCase() != "undefined") {
			        m_FileName = filename ;
			        m_Title = title ;
			        m_Type = type ;
		        }
	        } else {
		        var szPreviewFormType = typeof(document.forms[0]);
		        if (szPreviewFormType.toLowerCase() != "undefined") {
			        m_FileName = filename ;
			        m_Title = title ;
			        m_Type = type ;
					m_LibID = libraryid;
		        }
	        }
        }

        function SubmitInsert() {
	        var Source = "<%=m_strSource%>";
			var caller = "<%=caller%>";
	        var retFiled = "";
	        var sitePath = "";
	        retField = '<%=Request.QueryString["retfield"]%>';
	        sitePath = '<%=SitePath%>';
	        if (Source == "libinsert") {
		        top.close();
	        } else {
		        if ((m_Type != "") && (m_FileName != "") && (m_Title != "")) {
		            <% if (sEditor == "JSEditor") {%>
	                    <% if (sLinkText != "") { %>
	                        var slinktext = '<%Response.Write(sLinkText.Replace("\'","\\\'"));%>';
	                    <% }else {%>
	                        var slinktext = m_Title;
	                    <% } %>

	                    var sval = '';
	                    if (m_Type == 'images') {
	                        try {
	                            window.opener.JSEIMGInsert(escape(m_FileName),m_Title);
	                        }
	                        catch(ex) {}
	                    } else {
	                        try {
	                            window.opener.JSEURLInsert(m_FileName,slinktext);
	                        }
	                        catch(ex) {}
	                    }
	                    self.close();

	                <% }else{ %>

	                    if (caller != "editor")
                        {
	                        if (retField != "") {
	                            InserValueToField(m_FileName,m_PreviewThumbnail,sitePath, retField);
	                        } else {
			                    InsertFunction(m_FileName, m_Title, m_Type,m_LibID) ;
			                }
                        }
                        else{
	                        if ((m_Type != "") && (m_Type.toLowerCase() == "quicklinks" || m_Type.toLowerCase() == "forms" )) 
			                {
			                    if(m_FileName.toLowerCase().indexOf("linkit.aspx") > -1)
			                        m_FileName = m_FileName + "&libID=" + m_LibID ;
			                    else
			                        m_FileName = m_FileName;
    			 
			                  showSelAliasdialog();
			                  return false;
    			  
			                }
			                else
			                {
			                  InsertValue();
			                }
			            }

			        <% } %>
		        } else {
			        alert("<%= m_refMsg.GetMessage("js: alert double click lib name") %>");
			        return false;
		        }
	        }
        }
		function InsertValue()
		{
            //debugger;
			if ((m_Type != "") && (m_FileName != "") && (m_Title != ""))
			{
			    if (retField != '')
			    {
			        InserValueToField(m_FileName,m_PreviewThumbnail, sitePath, retField);
			    }
			    else
			    {
				    InsertFunction(m_FileName, m_Title, m_Type, m_LibID);
				}
			}
		}
        Ektron.ready(function() 
		{
		    $ektron("#selAliasDialog").modal({
                modal: true,
                overlay: 0,
                trigger: ""
                });
                $ektron("img[src *= 'delete.png']").closest("td").hide();
		});
		
		function showSelAliasdialog()
		{
		  $ektron.ajax({
              url: "urlaliasdialogHandler.ashx?action=getaliaslist&contID=" + document.forms[0].contentID.value + "&LangType=" + document.forms[0].contentLangId.value,
              cache: false,
              success: function(html){
                    if (html.indexOf("<error>") == -1) {
                        $ektron("#divAliasList").empty();
                        $ektron("#divAliasList").append("<p>" + html + "</p>");
                        if(html.indexOf("<aliascount>") != -1) {
                          if(html.indexOf("<linkmanage>")!=-1) {
                                $ektron("#selAliasDialog").modalShow();
                                return false;
                          }
                          else {
                                getRadioValue(1);
                                return false;
                          }
                        }
                        else {
                          $ektron("#selAliasDialog").modalShow();
                          return false;
                        }
                    } else{
                        InsertValue();
                        //SubmitInsert();
                    }
              }
            });
		 }
		 
		 function SaveAlias(selradio)
		 {
		    document.forms[0].preview_filename.value= selradio;
		    m_FileName = document.forms[0].preview_filename.value;
            if ('<%=Request.QueryString["enhancedmetaselect"]%>' != '' && '<%=Request.QueryString["enhancedmetaselect"]%>' == '1') {
                m_FileName = m_FileName + '?id=' + document.forms[0].contentID.value;
            }
            InsertValue();  
		 }
		 
		 function getRadioValue(count) 
		 {
		    if(count ==1 ) {
		      var radioValue = document.forms[0].aliasSelect.value;
		      SaveAlias(radioValue);
			  return false;
		    } else {
		        var index;
		        for(index=0; index < document.forms[0].aliasSelect.length; index++)
		        {
			        if (document.forms[0].aliasSelect[index].checked) {
					        var radioValue = document.forms[0].aliasSelect[index].value;
					        SaveAlias(radioValue);
					        break;
				    }
			      }  		       	
    		   }   
	       }
        function previewImage(scope) {

	        var type = "images";
	        var Source = "<%=m_strSource%>";
	        if (Source == "libinsert") {
	            //do nothing
	        } else {
		        if (scope == "") {
			        if ((m_Type != "") && (m_FileName != "") && (m_Title != "")) {
				        type = m_Type;
			         }
		        }
	        }

	        var filename = "";
	        filename = m_FileName;

	        //check if there is a value
	        if (filename == "") {
		        alert("<%= m_refMsg.GetMessage("js: alert single click lib name") %>");
		        return false;
	        } else {
		        //create the popup
		        popup = window.open('', 'imagePreview', 'width=600,height=450,left=100,top=75,screenX=100,screenY=75,scrollbars,location,menubar,status,toolbar,resizable=1');
		        //start writing in the html code
		        popup.document.writeln("<html><body bgcolor='#FFFFFF'>");
		        //get the extension of the file to see if it has one of the image extensions
		        if (type == "hyperlinks") {
			        if (filename.indexOf("://") == -1) {
				        popup.document.writeln("<a href='http://" + filename + "'>" + filename + "</a>");
			        }
			        else {
				        popup.document.writeln("<a href='" + filename + "'>" + filename + "</a>");
			        }
		        }
		        else if(type == "images") {
			        if (IsExtensionValid("images", filename)) {
				        popup.document.writeln("<img src='" + filename + "' />");
			        }
		        }
		        else {
			        //if not extension fron list above write URL to file
			        popup.document.writeln("<a href='" + filename + "'>" + filename + "</a>");
		        }
		        popup.document.writeln("</body></html>");
		        popup.document.close();
		        popup.focus();
	        }
	        return false;
        }

	    <asp:literal id="IsExtensionValid" runat="server"/>

        function PreviewFunct(oldURL){
            var regexp1 = / /gi;
            if (document.forms[0].preview_filename.value == "") {
	            alert("<%= "js: alert single click lib name" %>");
	            return false;
            }
            else {
	            if (document.forms[0].preview_type.value == "quicklinks") {
		            if (document.forms[0].preview_filename.value.indexOf("?") != -1) {
			            var tempHREF = document.forms[0].preview_filename.value.replace(regexp1, "%20") + "&Preview=True";
		            }
		            else {
			            var tempHREF = document.forms[0].preview_filename.value.replace(regexp1, "%20") + "?Preview=True";
		            }
	            }
	            else if ((document.forms[0].preview_type.value == "files")
			            || (document.forms[0].preview_type.value == "images")) {
		            var tempHREF = document.forms[0].preview_filename.value.replace(regexp1, "%20");
	            }
	            else if (document.forms[0].preview_type.value == "hyperlinks") {
		            var tempString = document.forms[0].preview_filename.value.toLowerCase();
		            if ((tempString.substring(0,7) == "http://")
				            || (tempString.substring(0,8) == "https://")) {
			            var tempHREF = document.forms[0].preview_filename.value.replace(regexp1, "%20");
		            }
		            else {
			            var tempHREF = "http://" + document.forms[0].preview_filename.value.replace(regexp1, "%20");
		            }
	            }
	            for (var i = 0; i < document.links.length; i++) {
		            if (document.links[i].href == oldURL) {
			            break;
		            }
	            }
	            document.links[i].href = tempHREF;
	            return true;
            }
        }

        var lastSelected = null;
        var lastSelectedColor;

        function Blink(CellName, Color) {
            if (lastSelected != CellName) {
	            if (lastSelected != null) {
		            if (document.getElementById) {
			            var MyObj = document.getElementById(lastSelected);
			            MyObj.style.background = lastSelectedColor;
			            var MyObj = document.getElementById(lastSelected + "_0");
			            MyObj.style.background = lastSelectedColor;
			            var MyObj = document.getElementById(lastSelected + "_1");
			            MyObj.style.background = lastSelectedColor;
			            var MyObj = document.getElementById(lastSelected + "_2");
			            MyObj.style.background = lastSelectedColor;
		            } else {
			            var layername = "layer" + lastSelected;
			            var NsObj = document.layers[layername];
			            NsObj.bgColor = lastSelectedColor;
			            var NsObj = document.layers[layername + "_0"];
			            NsObj.bgColor = lastSelectedColor;
			            var NsObj = document.layers[layername + "_1"];
			            NsObj.bgColor = lastSelectedColor;
			            var NsObj = document.layers[layername + "_2"];
			            NsObj.bgColor = lastSelectedColor;
		            }
	            }

	            lastSelected = CellName;

	            if (document.getElementById) {
		            var MyObj = document.getElementById(CellName);
		            lastSelectedColor = MyObj.style.background;
	            } else {
		            var layername = "layer" + CellName;

		            var NsObj = document.layers[layername];
		            lastSelectedColor = NsObj.bgColor;
	            }
            }

	        lastSelected = CellName;

            if (document.getElementById) {
	            var MyObj = document.getElementById(CellName);
	            MyObj.style.background = Color;
	            var MyObj = document.getElementById(CellName + "_0");
	            MyObj.style.background = Color;
	            var MyObj = document.getElementById(CellName + "_1");
	            MyObj.style.background = Color;
	            var MyObj = document.getElementById(CellName + "_2");
	            MyObj.style.background = Color;
            } else {
	            var layername = "layer" + CellName;
	            var NsObj = document.layers[layername];
	            NsObj.bgColor = Color;
	            var NsObj = document.layers[layername + "_0"];
	            NsObj.bgColor = Color;
	            var NsObj = document.layers[layername + "_1"];
	            NsObj.bgColor = Color;
	            var NsObj = document.layers[layername + "_2"];
	            NsObj.bgColor = Color;
            }
        }

        function updateFolders(Folder, folderType, imagepermission, filepermission, overwritepermission, libid){
            var Source = "<%=m_strSource%>";
            if (Source == "libinsert") {
	            if ((imagepermission != 0) && (imagepermission != 2)) {
		            imagepermission = 1;
	            }
	            if ((filepermission != 0) && ((filepermission != 2))) {
		            filepermission = 1;
	            }
	            if ((overwritepermission != 0) && ((overwritepermission != 2))) {
		            overwritepermission = 1;
	            }
            } else {
	            m_Folder = Folder;
	            if ((imagepermission != 0) && (imagepermission != 2)) {
		            imagepermission = 1;
	            }
	            if ((filepermission != 0) && ((filepermission != 2))) {
		            filepermission = 1;
	            }
	            if ((overwritepermission != 0) && ((overwritepermission != 2))) {
		            overwritepermission = 1;
	            }
            }
        }

        function ClearFolderInfo() {
            return true;
        }

        function resetPostback(){
            document.forms[0].isPostData.value = "";
            var spageLink = document.getElementById("pageLink");
            spageLink.value = "pageLink"
        }

        function processContentIsearch() {
            return false;
        }

        function alignTables(){
            var columns = new Array();
            columns[0] = $ektron("table#shared_group").find("col")[0];
            columns[1] = $ektron("table#shared_group").find("col")[1];

            columns[0].width = "85%";
            columns[1].width = "15%";
        }
    </script>

    <form id="ecmSearchAllAssets" method="post" runat="server" onsubmit="processContentIsearch();">
	<div class="ektronWindow" id="selAliasDialog">
		    <div class="ektronModalHeader" >
		        <h6><strong title="Insert Quicklink"><%=m_refContentApi.EkMsgRef.GetMessage("btn insert")%>  <%=m_refContentApi.EkMsgRef.GetMessage("lbl quicklink")%></strong></h6>
		        <a href="#" class="ektronModalClose"></a>
		    </div>
            <div class="divAliasList" id="divAliasList" runat="server"></div>
            <div class="divOk" id="divOk" runat="server">
                <input type="submit" title="Ok" name="aliasSubmit" id="aliasSubmit"  size="20" value="<%=m_refContentApi.EkMsgRef.GetMessage("lbl ok")%>"  onclick="getRadioValue(0);" />
            </div>
        </div>
    <div id="dhtmltooltip">
    </div>
    <div class="ektronPageHeader">
        <div class="ektronTitlebar" id="divTitleBar" runat="server">
        </div>
        <div class="ektronToolbar" id="divToolBar" runat="server">
        </div>
    </div>
    <input type="hidden" class="selectedTab" id="hdnSelectedTab" runat="server" />
    <div class="ektronPageContainer ektronPageTabbed">
        <div class="tabContainerWrapper">
            <div class="tabContainer">
                <ektronUI:Tabs id="uxSearchTabs" runat="server">
                    
                    <ektronUI:Tab ID="uxTabBasic" runat="server" OnClick="lbSearchPublished_Click">
                        <ContentTemplate>
                        <cms:WebSearch ID="websearch1" MaxCharacters="200" EnableAdvancedLink="false" runat="server" IsInWorkArea="true" />
                        </ContentTemplate>
                    </ektronUI:Tab>

                    <ektronUI:Tab ID="uxTabAdvanced" OnClick="lbSearchAdvanced_Click" runat="server">
                        <ContentTemplate>
                        <div>
                            <div id="TR_showdlg" runat="server">
                                <asp:Literal ID="CustFieldsContentLit" runat="Server" />
                            </div>
                            <div id="TR_showLibdlg" runat="server">
                                <table class="ektronGrid">
                                    <tr>
                                        <td class="label">
                                            <%=m_refMsg.GetMessage("lbl enter keyword")%>
                                            :
                                        </td>                                       
                                            <td class="value">
                                            <input name="frm_library_title" type="text" maxlength="75" size="50" />
                                            <input name="Search" type="submit" id="Search" value="<%=m_refMsg.GetMessage("res_isrch_btn")%>" />
                                        </td>
                                    </tr>
                                    <tr>
                                        <td class="label">
                                            <%=m_refMsg.GetMessage("lbl search options")%>
                                        </td>
                                        <td>
                                            <input title="Search All Types" type="radio" name="frm_libtype_id" value="0" checked="checked" /><%=m_refMsg.GetMessage("lbl all types")%>
                                            <br />
                                            <input title="Search Images only" type="radio" name="frm_libtype_id" value="1" /><%=m_refMsg.GetMessage("lbl images only")%>
                                            <br />
                                            <input title="Search Quicklinks only" type="radio" name="frm_libtype_id" value="4" /><%=m_refMsg.GetMessage("lbl quicklinks only")%>
                                            <br />
                                            <input title="Search Form links only" type="radio" name="frm_libtype_id" value="5" /><%=m_refMsg.GetMessage("lbl form links only")%>
                                            <br />
                                            <input title="Search Files only" type="radio" name="frm_libtype_id" value="2" /><%=m_refMsg.GetMessage("lbl files only")%>
                                            <br />
                                            <input title="Search Hyperlinks only" type="radio" name="frm_libtype_id" value="3" /><%=m_refMsg.GetMessage("lbl hyperlinks only")%>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td style="color: #1d5987; font-weight: bold;">
                                            <%=m_refMsg.GetMessage("msg field search title")%>:
                                        </td>
                                        <td>
                                            <input type="checkbox" title="Description Search" name="frm_library_description"
                                                value="1" /><%=m_refMsg.GetMessage("lbl description search")%>
                                            <br />
                                            <input type="checkbox" title="Filename search" name="frm_library_link" value="1" /><%=m_refMsg.GetMessage("lbl filename search")%>
                                            <br />
                                            <input type="checkbox" title="Only search items last edited by myself" name="frm_user_only_content"
                                                value="1" /><%=m_refMsg.GetMessage("alt only search items")%>
                                            <div class="ektronTopSpace">
                                            </div>
                                        </td>
                                    </tr>
                                </table>
                                <div style="background-color: #d0e5f5; border: #79b7e7 1px solid; border-top: none;">
                                    <asp:Literal ID="CustFieldsLibraryLit" runat="Server" />
                                </div>
                            </div>
                            <div class="ektronPageGrid">
                                <asp:DataGrid ID="SearchResultGrid" Width="100%" AutoGenerateColumns="False" runat="server"
                                    CssClass="ektronGrid" OnItemDataBound="SearchResultGrid_ItemDataBound" EnableViewState="False"
                                    GridLines="None">
                                    <HeaderStyle CssClass="title-header" />
                                </asp:DataGrid>
                                <p class="pageLinks">
                                    <asp:Label ToolTip="Page" runat="server" ID="PageLabel" Visible="false"><%=m_refMsg.GetMessage("page lbl")%></asp:Label>
                                    <asp:Label ID="CurrentPage" CssClass="pageLinks" runat="server" />
                                    <asp:Label ToolTip="of" runat="server" ID="OfLabel" Visible="false"><%=m_refMsg.GetMessage("lbl of")%></asp:Label>
                                    <asp:Label ID="TotalPages" CssClass="pageLinks" runat="server" />
                                </p>
                                <asp:LinkButton ToolTip="First Page" runat="server" CssClass="pageLinks" ID="ctrlFirstPage"
                                    Text="[First Page]" OnCommand="NavigationLink_Click" CommandName="First" OnClientClick="resetPostback()"
                                    Visible="false" />
                                <asp:LinkButton ToolTip="Previous Page" runat="server" CssClass="pageLinks" ID="ctrlPreviousPage"
                                    Text="[Previous Page]" OnCommand="NavigationLink_Click" CommandName="Prev" OnClientClick="resetPostback()"
                                    Visible="false" />
                                <asp:LinkButton ToolTip="Next Page" runat="server" CssClass="pageLinks" ID="ctrlNextPage"
                                    Text="[Next Page]" OnCommand="NavigationLink_Click" CommandName="Next" OnClientClick="resetPostback()"
                                    Visible="false" />
                                <asp:LinkButton ToolTip="Last Page" runat="server" CssClass="pageLinks" ID="ctrlLastPage"
                                    Text="[Last Page]" OnCommand="NavigationLink_Click" CommandName="Last" OnClientClick="resetPostback()"
                                    Visible="false" />
                                <br />
                                <asp:Literal ID="resultLit" runat="Server" />
                                <div id="pageDiv">
                                    <asp:Literal ID="HiddenData" runat="Server" />
                                </div>
                                <div id="iconList">
                                    <div id="iconListOutput">
                                        <asp:Literal ID="iconListOutputLit" runat="Server" /></div>
                                </div>
                            </div>
                        </div>
                        </ContentTemplate>
                    </ektronUI:Tab>
                </ektronUI:Tabs>
            </div>
        </div>
    </div>
    <input type="hidden" name="frm_object_type" id="frm_object_type" value="" runat="server" />
    <input type="hidden" name="frm_folder_id" id="frm_folder_id" value="" runat="server" />
    <input type="hidden" name="preview_type" id="preview_type" value="" runat="server" />
    <input type="hidden" name="preview_filename" id="preview_filename" value="" runat="server" />
    <input type="hidden" name="source" value="<%=m_strSource%>" />
    <input type="hidden" runat="server" id="isPostData" name="isPostData" value="true" />
    <input type="hidden" id="hCurrentPage" name="CurrentPage" value="" runat="server" />
    <input type="hidden" id="hTotalPages" name="TotalPages" value="" runat="server" />
    <input type="hidden" id="hmenuSelected" name="hmenuSelected" runat="server" />
    <input type="hidden" id="pageLink" name="pageLink" runat="server" />
    <input type="hidden" id="pageMode" name="pageMode" runat="server" />
	<input type="hidden" name="contentID" id="contentID" />
    <input type="hidden" value="<%= contLangID %>" name="contentLangId" id="contentLangId" />
    </form>
</body>
</html>
