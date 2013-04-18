<%@ Control Language="C#" AutoEventWireup="true" CodeFile="MediaUploaderCommon.ascx.cs" Inherits="MediaUploaderCommon" %>

<script type="text/javascript"  language="JavaScript">
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
 