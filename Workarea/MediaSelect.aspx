<%@ Page Language="C#" AutoEventWireup="true" CodeFile="MediaSelect.aspx.cs" Inherits="MediaSelect" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml" >
<head runat="server"> 
    <title>Untitled Page</title>
    <meta http-equiv="content-type" content="text/html; charset=UTF-8" />
	<asp:literal id="StyleSheetJS" runat="server"/>
	<style type="text/css">
	   .metaselect_row_not_highlighted{}
		.metaselect_row_highlighted{background-color:yellow;}
	</style>
	<script type="text/javascript">
	    <!--//--><![CDATA[//><!--
	    var g_metaselect_rowPrefix = "metaselect_row_";
	    var g_metaselect_checkboxPrefix = "metaselect_checkbox_";
	    var g_metaselect_titlePrefix = "metaselect_title_";
	    var g_metaselect_idNameLinkPrefix = "metaselect_idNameLink_";
	    var retry_counter = 0;
	    var parent_node = document.getElementById("MediaSelectTableBody");
    
	    function metaselect_initialize()
	    {
		    var mainWindow = metaselect_getMainWindow()
		    if (mainWindow)
		    {
			    var metadataFormTagId = mainWindow.ek_ma_getSelectedFormTagId();
			    var separator = mainWindow.ek_ma_getDelimiter(metadataFormTagId);
			    var idNameLinks = mainWindow.ek_ma_getId(metadataFormTagId);
			    var titles = mainWindow.ek_ma_getTitle(metadataFormTagId);

			    // Prepopulate the selection (based on previous values, if any)...
			    // if there's multiple items, split the idNames and the titles using delimiters:
			    if (metadataFormTagId && idNameLinks && idNameLinks.length)
			    {
				    if (separator && separator.length)
				    {
					    var namIdArray = idNameLinks.split(separator);
					    var titleArray = titles.split(separator);
					    for (idx=0; idx < namIdArray.length; idx++)
					    {
						    //var itemId = "prepop=1&id=" + namIdArray[idx];
						    var itemId = namIdArray[idx];
						    var itemTitle;
						    if (titleArray && titleArray[idx])
						    {
							    itemTitle = titleArray[idx];
						    }
						    else
						    {
							    itemTitle = "";
						    }
						    metaselect_addMetaSelectRow(itemId, itemTitle, metadataFormTagId, separator)
					    }
				    }
				    else
				    {
					    metaselect_addMetaSelectRow(idNameLinks, titles, metadataFormTagId, separator)
				    }
			    }
		    }
		    else
		    {
			    // give-up after 20 seconds:
			    if (retry_counter < 200)
			    {
				    ++retry_counter;
				    setTimeout("metaselect_initialize()", 100);
			    }
		    }
    	
		    tweakUI();
	    }
    	
	    // Setup the UI for the current use mode:
	    function tweakUI() {
		    var hdrObj;
		    if (window.parent
			    && window.parent.frames
			    && window.parent.frames[0]
			    && window.parent.frames[0].location
			    && window.parent.frames[0].location.href
			    && (0 < window.parent.frames[0].location.href.indexOf("SelectFolder.aspx"))){

			    hdrObj = document.getElementById("link_title_hdr");
			    if (hdrObj && hdrObj.innerText){
				    hdrObj.innerText = '<asp:literal id="ltr_folder" runat="server"/>';
			    }

			    hdrObj = document.getElementById("link_id_col_hdr");
			    if (hdrObj && hdrObj.innerText){
				    hdrObj.innerText =  '<asp:literal id="ltr_folderId" runat="server" />';
			    }
		    }
	    }
    	
	    function metaselect_addMetaSelectRow(idNameLink, title, formTagId, separator)
	    {
		    // Only add unique rows (checked after parsing), 
		    // and it must have an id/link (title optional):
		    if (idNameLink && idNameLink.length)
		    {
			    // Attempt ID cleanup before display:					
		        var mainWindow = metaselect_getMainWindow();
			    if (mainWindow && mainWindow.ek_ma_parseId 
				    && mainWindow.ek_ma_isContentType
				    && mainWindow.ek_ma_isContentType(formTagId))
			    {
				    idNameLink = mainWindow.ek_ma_parseId(idNameLink);
			    }

			    if (idNameLink.length && !DoesRowExist(idNameLink, title) && title !== "")
			    {

				    var uniqueRowId = parseInt(document.getElementById("MediaSelect_uniqueRowId").value, 10) + 1;
				    document.getElementById("MediaSelect_uniqueRowId").value = uniqueRowId.toString();
    			
				    document.getElementById("MediaSelect_FormTagId").value = formTagId;
				    document.getElementById("MediaSelect_Separator").value = separator;
    				
				    var parent_node = document.getElementById("MediaSelectTableBody");	
				    if (parent_node)
				    {
					    var el, rowNode, cellNode, inputNode, textNode;

					    rowNode = document.createElement("tr");
					    rowNode.setAttribute("id", g_metaselect_rowPrefix + uniqueRowId.toString());
					    rowNode = parent_node.appendChild(rowNode);
					    rowNode.className = "metaselect_row_not_highlighted";
					    if (IsBrowserIE())
					    {
						    rowNode.attachEvent("onclick", metaselect_highlightRow);
					    }
					    else
					    {
						    rowNode.addEventListener("click", metaselect_highlightRow, false);
					    }
					    cellNode = document.createElement("td");
					    cellNode = rowNode.appendChild(cellNode);
					    //
					    inputNode = document.createElement("input");
					    inputNode.setAttribute("type", "checkbox");
					    inputNode.setAttribute("id", g_metaselect_checkboxPrefix + uniqueRowId.toString());
					    inputNode = cellNode.appendChild(inputNode);
					    inputNode.setAttribute("checked", true);
					    //
					    cellNode = document.createElement("td");
					    cellNode.setAttribute("id", g_metaselect_titlePrefix + uniqueRowId.toString());
					    cellNode = rowNode.appendChild(cellNode);
					    textNode = document.createTextNode(title);
					    textNode = cellNode.appendChild(textNode);
					    //
					    cellNode = document.createElement("td");
					    cellNode.setAttribute("id", g_metaselect_idNameLinkPrefix + uniqueRowId.toString());
					    cellNode = rowNode.appendChild(cellNode);
					    textNode = document.createTextNode(idNameLink.toString());
					    textNode = cellNode.appendChild(textNode);
				    }
			    }
		    }
	    }
    	
	    var g_metaselect_oldHighlightedElement = null;
	    function metaselect_highlightRow(el)
	    {
		    if (IsBrowserIE())
		    {
			    if (g_metaselect_oldHighlightedElement != null)
			    {
				    g_metaselect_oldHighlightedElement.className = "metaselect_row_not_highlighted";
			    }
			    if (el 
				    && el.srcElement 
				    && el.srcElement.parentElement
				    && el.srcElement.parentElement.tagName == "TR"
				    && g_metaselect_oldHighlightedElement != el.srcElement.parentElement)
			    {
				    g_metaselect_oldHighlightedElement = el.srcElement.parentElement;
				    el.srcElement.parentElement.className = "metaselect_row_highlighted";
			    }
			    else
			    {
				    g_metaselect_oldHighlightedElement = null;
			    }
		    }
		    else
		    {
			    // Not IE:
			    if (g_metaselect_oldHighlightedElement != null)
			    {
				    g_metaselect_oldHighlightedElement.className = "metaselect_row_not_highlighted";
			    }
			    if (el 
				    && el.target 
				    && el.target.parentNode
				    && el.target.parentNode.tagName == "TR"
				    && g_metaselect_oldHighlightedElement != el.target.parentNode)
			    {
				    g_metaselect_oldHighlightedElement = el.target.parentNode;
				    el.target.parentNode.className = "metaselect_row_highlighted";
			    }
			    else
			    {
				    g_metaselect_oldHighlightedElement = null;
			    }
		    }
	    }
    	
	    function metaselect_toggleCheckBoxes()
	    {
		    var master_box_status = document.getElementById("header_checkbox").checked;
		    var parent_node = document.getElementById("MediaSelectTableBody");	
		    if (parent_node)
		    {
			    var childBoxNodes = document.getElementsByTagName("input");
			    for (var cnt = 0; cnt < (childBoxNodes.length); cnt++)
			    {
				    if ("header_checkbox" != childBoxNodes[cnt].id)
				    {
					    childBoxNodes[cnt].checked = master_box_status;
				    }
			    }
		    }
	    }

	    function metaselect_saveChecked()
	    {
		    var parent_node = document.getElementById("MediaSelectTableBody");	

		    var mainWindow = metaselect_getMainWindow();
		    if (mainWindow && mainWindow.ek_ma_getDelimiter)
		    {
			    var formTagId = mainWindow.ek_ma_getSelectedFormTagId();
			    var separator = mainWindow.ek_ma_getDelimiter(formTagId);
		    }
		    else
		    {
			    var formTagId = document.getElementById("MediaSelect_FormTagId").value;
			    var separator = document.getElementById("MediaSelect_Separator").value;
		    }
    		
		    var itemTitle = "";
		    var itemIdName = "";
		    if (separator == null || 0 == separator.length)
		    {
			    separator = ";"; // default?
		    }
		    if (parent_node)
		    {
			    var childRowNodes = document.getElementsByTagName("tr");
			    var prefix = g_metaselect_rowPrefix;
			    var prefixLen = prefix.length;
			    for (var cnt = 0; cnt < (childRowNodes.length); cnt++)
			    {
				    var childNodeId = childRowNodes[cnt].id;
				    var startIndex = childNodeId.indexOf(g_metaselect_rowPrefix, 0);
    				
				    if ((startIndex != -1) && (childNodeId != "header_row"))
				    {
					    var idNumber = parseInt(childNodeId.substring(startIndex + prefixLen), 10);
					    var chkbx = document.getElementById(g_metaselect_checkboxPrefix + idNumber.toString());
					    if (chkbx)
					    {
						    if (chkbx.checked)
						    {
							    itemTitle += metaselect_getRowTitle(idNumber) + separator;
							    itemIdName += metaselect_getRowidName(idNumber) + separator;
						    }
					    }
				    }
			    }
    			
			    if (itemIdName && itemIdName.length)
			    {
				    if (separator == itemIdName.charAt(itemIdName.length-1))
				    {
					    itemIdName = itemIdName.substring(0, itemIdName.length-1);
				    }
			    }
			    if ((itemTitle) && (itemTitle.length))
			    {
				    if (separator == itemTitle.charAt(itemTitle.length-1))
				    {
					    itemTitle = itemTitle.substring(0, itemTitle.length-1);
				    }
			    }
			    var elementTitle = mainWindow.document.getElementById('__EkElementTitle');
			    var elementHidden = mainWindow.document.getElementById('__EkPersonalizationEditValue');
    			
			    if( elementTitle != null ) 
			    {
			        elementTitle.innerHTML = itemIdName.replace(/;/g, ",");
			        elementHidden.value = itemIdName.replace(/;/g, ",");
			    }
    			
			    if (itemIdName && itemIdName.length)
			    {
				    metaselect_saveValues(itemIdName, itemTitle, formTagId);
			    }
			    else
			    {
				    // Nothing selected, must clear original values:
				    var mainWindow = metaselect_getMainWindow();
				    if ((mainWindow != null) 
					    && (mainWindow.ek_ma_ClearSelection != null) 
					    && (typeof(mainWindow.ek_ma_ClearSelection) != 'undefined'))
				    {
					    mainWindow.ek_ma_ClearSelection(formTagId, "None selected");
					    metaselect_close();
				    }
			    }
		    }
	    }

		var mainWindow = metaselect_getMainWindow()
		if (mainWindow && mainWindow.ek_ma_getDelimiter)
		{
			var formTagId = mainWindow.ek_ma_getSelectedFormTagId();
			var separator = mainWindow.ek_ma_getDelimiter(formTagId);
		}
		else
		{
			var formTagId = document.getElementById("MediaSelect_FormTagId").value;
			var separator = document.getElementById("MediaSelect_Separator").value;
		}
		
		var itemTitle = "";
		var itemIdName = "";
		if (separator == null || 0 == separator.length)
		{
			separator = ";"; // default?
		}
		if (parent_node)
		{
			var childRowNodes = document.getElementsByTagName("tr");
			var prefix = g_metaselect_rowPrefix;
			var prefixLen = prefix.length;
			for (var cnt = 0; cnt < (childRowNodes.length); cnt++)
			{
				var childNodeId = childRowNodes[cnt].id;
				var startIndex = childNodeId.indexOf(g_metaselect_rowPrefix, 0);
				
				if ((startIndex != -1) && (childNodeId != "header_row"))
				{
					var idNumber = parseInt(childNodeId.substring(startIndex + prefixLen), 10);
					var chkbx = document.getElementById(g_metaselect_checkboxPrefix + idNumber.toString());
					if (chkbx)
					{
						if (chkbx.checked)
						{
							itemTitle += metaselect_getRowTitle(idNumber) + separator;
							itemIdName += metaselect_getRowidName(idNumber) + separator;
						}
					}
				}
			}
			
			if (itemIdName && itemIdName.length)
			{
				if (separator == itemIdName.charAt(itemIdName.length-1))
				{
					itemIdName = itemIdName.substring(0, itemIdName.length-1);
				}
			}
			if ((itemTitle) && (itemTitle.length))
			{
				if (separator == itemTitle.charAt(itemTitle.length-1))
				{
					itemTitle = itemTitle.substring(0, itemTitle.length-1);
				}
			}
			
			var mainWindow = metaselect_getMainWindow();
			
			var elementTitle = mainWindow.document.getElementById('__EkElementTitle');
			var elementHidden = mainWindow.document.getElementById('__EkPersonalizationEditValue');
			
			if( elementTitle != null ) {
			    elementTitle.innerHTML = itemIdName.replace(/;/g, ",");
			    elementHidden.value = itemIdName.replace(/;/g, ",");
			}
			
			if (itemIdName && itemIdName.length)
			{
				metaselect_saveValues(itemIdName, itemTitle, formTagId);
			}
			else
			{
				// Nothing selected, must clear original values:
				var mainWindow = metaselect_getMainWindow();
				if ((mainWindow != null) 
					&& (mainWindow.ek_ma_ClearSelection != null) 
					&& (typeof(mainWindow.ek_ma_ClearSelection) != 'undefined'))
				{
					mainWindow.ek_ma_ClearSelection(formTagId, "None selected");
					metaselect_close();
				}
			}
		}

	    function metaselect_getRowTitle(rowNumber)
	    {
		    var itemTitle = "";
		    var elItemTitle = document.getElementById(g_metaselect_titlePrefix + rowNumber.toString()); 
		    if (elItemTitle)
		    {
			    if ((elItemTitle.innerText) && ('string' == typeof(elItemTitle.innerText)))
			    {
				    itemTitle = elItemTitle.innerText;
			    }
			    else if ((elItemTitle.textContent) && ('string' == typeof(elItemTitle.textContent)))
			    {
				    itemTitle = elItemTitle.textContent;
			    }
		    }
		    return (itemTitle);
	    }

	    function metaselect_getRowidName(rowNumber)
	    {
		    var itemIdName = "";
		    var elItemIdName = document.getElementById(g_metaselect_idNameLinkPrefix + rowNumber.toString()); 
		    if (elItemIdName)
		    {
			    if ((elItemIdName.innerText) && ('string' == typeof(elItemIdName.innerText)))
			    {
				    itemIdName = elItemIdName.innerText;
			    }
			    else if ((elItemIdName.textContent) && ('string' == typeof(elItemIdName.textContent)))
			    {
				    itemIdName = elItemIdName.textContent;
			    }
		    }
		    return (itemIdName);
	    }
    	
	    function DoesRowExist(idNameLink, title)
	    {
		    var parent_node = document.getElementById("MediaSelectTableBody");	
		    var formTagId = document.getElementById("MediaSelect_FormTagId").value;
		    if (parent_node)
		    {
			    var childRowNodes = document.getElementsByTagName("tr");
			    var prefix = g_metaselect_rowPrefix;
			    var prefixLen = prefix.length;
			    for (var cnt = 0; cnt < (childRowNodes.length); cnt++)
			    {
				    var childNodeId = childRowNodes[cnt].id;
				    var startIndex = childNodeId.indexOf(g_metaselect_rowPrefix, 0);
    				
				    if ((startIndex != -1) && (childNodeId != "header_row"))
				    {
					    var idNumber = parseInt(childNodeId.substring(startIndex + prefixLen), 10);
					    var itemTitle = metaselect_getRowTitle(idNumber);
					    var itemIdName = metaselect_getRowidName(idNumber);
					    if ((itemTitle == title) && (itemIdName == idNameLink))
					    {
						    return (true);
					    }
				    }
			    }
		    }
		    return (false);
	    }

	    function metaselect_insideMetaSelectContainer()
	    {
		    return (window.parent && window.parent.isMetaSelectContainer
			    && ("function" == typeof window.parent.isMetaSelectContainer)
			    && window.parent.isMetaSelectContainer());
	    }
    	
	    function metaselect_getMainWindow()
	    {
		    var mainWindow = null;
		    if (metaselect_insideMetaSelectContainer())
		    {
			    mainWindow = window.parent.getMainWindow();
		    }
		    else
		    {
			    if ((top.opener != null) 
				    && (!top.opener.closed)
				    && (top.opener.ek_ma_ReturnMediaUploaderValue != null) 
				    && (typeof(top.opener.ek_ma_ReturnMediaUploaderValue) != 'undefined'))
			    {
				    mainWindow = top.opener;
			    }
		    }
		    return (mainWindow);
	    }
    	
	    function metaselect_saveValues(insertvalue, title, metadataFormTagId)
	    {
		    // Note: Need to dermine context, we may be running within the 
		    // MediaManager frameset, or we might be inside MetaSelectContainer:
		    var mainWindow = metaselect_getMainWindow()
		    if (mainWindow)
		    {
			    mainWindow.ek_ma_ReturnMediaUploaderValue(insertvalue, title, metadataFormTagId);
			    mainWindow.ek_ma_CloseChildPage();
		    }
		    // Close the mediamanager if running in the popup library dialog:
		    if (!metaselect_insideMetaSelectContainer() || metaselect_insidePopUpWindow())
		    {
			    metaselect_close();
			    //top.close();
		    }
	    }

	function metaselect_insideMetaSelectContainer()
	{
		return (window.parent && window.parent.isMetaSelectContainer
			&& ("function" == typeof window.parent.isMetaSelectContainer)
			&& window.parent.isMetaSelectContainer());
	}
	
	function metaselect_getMainWindow()
	{
		var mainWindow = null;
		if (metaselect_insideMetaSelectContainer())
		{
			mainWindow = window.parent.getMainWindow();
		}
		if(mainWindow == null)
		{
			if ((top.opener != null) 
				&& (!top.opener.closed)
				&& (top.opener.ek_ma_ReturnMediaUploaderValue != null) 
				&& (typeof(top.opener.ek_ma_ReturnMediaUploaderValue) != 'undefined'))
			{
				mainWindow = top.opener;
			}
		}
		return (mainWindow);
	}
	
	function metaselect_saveValues(insertvalue, title, metadataFormTagId)
	{
		// Note: Need to dermine context, we may be running within the 
		// MediaManager frameset, or we might be inside MetaSelectContainer:
		var mainWindow = metaselect_getMainWindow()
		if (mainWindow)
		{
			mainWindow.ek_ma_ReturnMediaUploaderValue(insertvalue, title, metadataFormTagId);
			mainWindow.ek_ma_CloseChildPage();
		}
		// Close the mediamanager if running in the popup library dialog:
		if (!metaselect_insideMetaSelectContainer() || metaselect_insidePopUpWindow())
		{
			metaselect_close();
			//top.close();
		}
	}

	    function metaselect_reorder(upFlag)
	    {
		    var hlRow = metaselect_findHighlightedRow();
    		
		    if (hlRow != null)
		    {
			    var isChecked = metaselect_isRowChecked(hlRow);
			    var parentEl = hlRow.parentNode;
			    if (parentEl)
			    {
				    if (upFlag)
				    {
					    if (hlRow != parentEl.firstChild)
					    {
						    var prevRow = hlRow.previousSibling;
						    parentEl.removeChild(hlRow);
						    parentEl.insertBefore(hlRow, prevRow);
					    }
				    }
				    else
				    {
					    if (hlRow != parentEl.lastChild)
					    {
						    var nextRow = hlRow.nextSibling;
						    if (nextRow == parentEl.lastChild)
						    {
							    parentEl.removeChild(hlRow);
							    parentEl.appendChild(hlRow);
						    }
						    else
						    {
							    nextRow = nextRow.nextSibling;
							    parentEl.removeChild(hlRow);
							    parentEl.insertBefore(hlRow, nextRow);
						    }
					    }
				    }
				    metaselect_setRowCheckStatus(hlRow, isChecked);
			    }
		    }
		    else
		    {
			    alert('Nothing selected; click on a title to move');
		    }
	    }

	    function metaselect_findHighlightedRow()
	    {
		    var parent_node = document.getElementById("MediaSelectTableBody");	
		    if (parent_node)
		    {
			    var rowNodes = document.getElementsByTagName("tr");
			    for (var cnt = 0; cnt < (rowNodes.length); cnt++)
			    {
				    if ("metaselect_row_highlighted" == rowNodes[cnt].className)
				    {
					    return (rowNodes[cnt]);
				    }
			    }
		    }
		    return (null);
	    }
    	
	    function metaselect_isRowChecked(rowObj)
	    {
		    if (rowObj != null)
		    {
			    var children = rowObj.childNodes;
			    if (children != null)
			    {
				    for (var idx=0; idx < children.length; idx++)
				    {
					    if (children[idx] 
						    && children[idx].childNodes
						    && children[idx].childNodes[0]
						    && children[idx].childNodes[0].id
						    && (children[idx].childNodes[0].id).indexOf(g_metaselect_checkboxPrefix, 0) >= 0)
					    {
						    return (children[idx].childNodes[0].checked);
					    }
				    }
			    }
		    }
		    return (false);
	    }
    	
	    function metaselect_setRowCheckStatus(rowObj, checkedFlag)
	    {
		    if (rowObj != null)
		    {
			    var children = rowObj.childNodes;
			    if (children != null)
			    {
				    for (var idx=0; idx < children.length; idx++)
				    {
					    if (children[idx] 
						    && children[idx].childNodes
						    && children[idx].childNodes[0]
						    && children[idx].childNodes[0].id
						    && (children[idx].childNodes[0].id).indexOf(g_metaselect_checkboxPrefix, 0) >= 0)
					    {
						    children[idx].childNodes[0].checked = checkedFlag;
						    return;
					    }
				    }
			    }
		    }
	    }
    	
        function IsBrowserIE() 
        {
		    //return (document.all ? true : false);
		    var ua = window.navigator.userAgent.toLowerCase();
		    return((ua.indexOf('msie') > -1) && (!(ua.indexOf('opera') > -1)));
	    }
	    function metaselect_removeChecked()
	{
		var remeoveIds="";
		var parent_node = document.getElementById("MediaSelectTableBody");	
		if (parent_node)
		{
			var childRowNodes = document.getElementsByTagName("tr");
			var prefix = g_metaselect_rowPrefix;
			var prefixLen = prefix.length;
			for (var cnt = 0; cnt < (childRowNodes.length); cnt++)
			{
				var childNodeId = childRowNodes[cnt].id;
				var startIndex = childNodeId.indexOf(g_metaselect_rowPrefix, 0);
				
				if ((startIndex != -1) && (childNodeId != "header_row"))
				{
					var idNumber = parseInt(childNodeId.substring(startIndex + prefixLen), 10);
					var chkbx = document.getElementById(g_metaselect_checkboxPrefix + idNumber.toString());
					if (chkbx)
					{
						if (chkbx.checked)
						{
							remeoveIds += idNumber.toString() + ",";
						}
					}
				}
			}
			if (remeoveIds.length > 1)
			{
				remeoveIds = remeoveIds.substring(0, remeoveIds.length - 1);
				var remeoveIdArray = remeoveIds.split(",");
				for (var cnt = 0; cnt < remeoveIdArray.length; cnt++)
				{
					var delNode = document.getElementById(g_metaselect_rowPrefix + remeoveIdArray[cnt]);
					if (delNode)
					{
						parent_node = delNode.parentNode;
						parent_node.removeChild(delNode);
					}
				}
			}
		}
	}
	function metaselect_close()
	{
		if (metaselect_insidePopUpWindow())
		{
			top.close();
		}
		else
		{
			var mainWindow = metaselect_getMainWindow()
			if (mainWindow)
			{
				if (mainWindow.ek_ma_CloseChildPage)
				{
					mainWindow.ek_ma_CloseChildPage();
				}
			}
		}
	}
	
	function metaselect_insidePopUpWindow()
	{
		return (window.parent == window.top);
	}
	//--><!]]>
	</script>
</head>
<body id="main_body" class="PopupLibrary" onload="setTimeout('metaselect_initialize()', 100);">
    <form id="form1" runat="server">
        <div id="dhtmltooltip"></div>
        <div class="ektronPageHeader">
            <div class="ektronTitlebar" id="divTitleBar" runat="server"></div>
		    <div class="ektronToolbar" id="divToolBar" runat="server"></div>
		</div>
        <div class="ektronPageContainer ektronPageGrid">
			<table id="MediaSelectTable" class="ektronGrid" style="width:99.9%;">
				<thead>
					<tr class="title-header">
						<td style="width:1%;"><input type="checkbox" id="header_checkbox" checked="checked" onclick="metaselect_toggleCheckBoxes()"/></td>
						<td id="link_title_hdr" title="Title"><%=m_refMsg.GetMessage("generic title")%></td>
						<td id="link_id_col_hdr" title="Link"><%=m_refMsg.GetMessage("lbl link")%></td>
					</tr>
				</thead>
				<tbody  id="MediaSelectTableBody">
				</tbody>
			</table>
		</div>
		<input type="hidden" id="MediaSelect_FormTagId" value="" />
		<input type="hidden" id="MediaSelect_uniqueRowId" value="0" />
		<input type="hidden" id="MediaSelect_Separator" value="" />
    </form>
</body>
</html>

