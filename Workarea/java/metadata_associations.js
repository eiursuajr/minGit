var ek_ma_selectedFormTagtId = 0;

function ek_ma_getSelectedFormTagId()
{
	return (ek_ma_selectedFormTagtId);
}

function ek_personalization_clselect(WAPath)
{
    ek_ma_LoadMetaChildPage('0', '0', '0', WAPath);
}

function ek_ma_LoadMetaChildPage(type, tagtype, metadataFormTagId, WAPath) 
{
	var pageObj, frameObj
	var userGrpId = 1;
	var bShowIframe = false;
	var id = 0;
	var title = '';
	var folder = '';
	var delimeterChar = "";
	var langType = -1;
	
	if( WAPath == null ) {
	    WAPath = "";
	}
	
	// track selected tag id:
	ek_ma_selectedFormTagtId = metadataFormTagId;

	var hiddenObj = document.getElementById('Hdn_EnhancedMetadataTitle' + metadataFormTagId);
	if (ek_ma_validateObject(hiddenObj)) 
	{
		title = hiddenObj.value;
	}
	hiddenObj = document.getElementById('frm_text_' + metadataFormTagId);
	if (ek_ma_validateObject(hiddenObj)) 
	{
		id = hiddenObj.value;
	}
	hiddenObj = document.getElementById('Hdn_EnhancedMetadataFolderId' + metadataFormTagId);
	if (ek_ma_validateObject(hiddenObj)) 
	{
		folder = hiddenObj.value;
	}
	
	hiddenObj = document.getElementById('Hdn_EnhancedMetadataLangTypeId' + metadataFormTagId);
	if (ek_ma_validateObject(hiddenObj)) 
	{
		langType = hiddenObj.value;
	}
	
	//
	// get delimeter char from designated tag-group:
	hiddenObj = document.getElementById("MetaSeparator_" + metadataFormTagId);
	if (ek_ma_validateObject(hiddenObj)) 
	{
		delimeterChar = hiddenObj.value;
	}	
	// Browser is Netscape, use a seperate pop-up window:
	if (ek_ma_isListSummaryType(metadataFormTagId))
	{
		// MetaTagType_ListSummary:
		if (id < 0)
		{
			id = 0; // default to root folder.
		}
		var windObj = window.open(WAPath + 'blankredirect.aspx?MetaSelectContainer.aspx?FolderID=' + id + '&browser=1&WantXmlInfo=1&metadataFormTagId=' + metadataFormTagId,'Preview','width=' + 600 + ',height=' + 400 +',status=no,resizable=yes,scrollbars=no,location=no,toolbar=no');
	}
	else if (ek_ma_isImageType(metadataFormTagId))
	{
		// MetaTagType_Image:
		var windObj = window.open(WAPath + 'mediamanager.aspx?actiontype=library&scope=images&autonav=' + folder + '&enhancedmetaselect=1&metadataFormTagId=' + metadataFormTagId,'Preview','width=' + 760 + ',height=' + 580 +',status=no,resizable=yes,scrollbars=no,location=no,toolbar=no');
	}
	else if (ek_ma_isContentType(metadataFormTagId))
	{
		// MetaTagType_Content:
		var windObj = window.open(WAPath + 'mediamanager.aspx?actiontype=library&scope=&type=quicklinks&autonav=' + folder + '&enhancedmetaselect=1&metadataFormTagId=' + metadataFormTagId,'Preview','width=' + 760 + ',height=' + 580 +',status=no,resizable=yes,scrollbars=no,location=no,toolbar=no');
	}
	else if (ek_ma_isHyperLinkType(metadataFormTagId))
	{
		// MetaTagType_HyperLink:
		var windObj = window.open(WAPath + 'mediamanager.aspx?actiontype=library&scope=hyperlink&type=hyperlinks&autonav=' + folder + '&enhancedmetaselect=1&metadataFormTagId=' + metadataFormTagId,'Preview','width=' + 760 + ',height=' + 580 +',status=no,resizable=yes,scrollbars=no,location=no,toolbar=no');
	}
	else if (ek_ma_isFileType(metadataFormTagId))
	{
		// MetaTagType_File:
		var windObj = window.open(WAPath + 'mediamanager.aspx?actiontype=library&scope=files&type=files&autonav=' + folder + '&enhancedmetaselect=1&metadataFormTagId=' + metadataFormTagId,'Preview','width=' + 760 + ',height=' + 580 +',status=no,resizable=yes,scrollbars=no,location=no,toolbar=no');
	}
	else if (ek_ma_isMenuType(metadataFormTagId))
	{
	    var windObj = window.open(WAPath + 'MetaSelectContainer.aspx?target_page=MetaSelect.aspx&type=' + type + '&tagtype=' + tagtype + '&id=' + id + '&title=' + title + '&metadataFormTagId=' + metadataFormTagId,'Preview','width=' + 760 + ',height=' + 580 +',status=no,resizable=yes,scrollbars=no,location=no,toolbar=no');
	}
	else if (ek_ma_isUserType(metadataFormTagId))
	{
	    var windObj = window.open(WAPath + 'MetaSelectContainer.aspx?target_page=MetaSelect.aspx&type=' + type + '&LangType=' + langType + '&tagtype=' + tagtype + '&id=' + id + '&title=' + title + '&metadataFormTagId=' + metadataFormTagId,'Preview','width=' + 760 + ',height=' + 580 +',status=no,resizable=yes,scrollbars=no,location=no,toolbar=no');
	}
	else
	{
		// Collection:
		var windObj = window.open(WAPath + 'MetaSelectContainer.aspx?target_page=MetaSelect.aspx&type=' + type + '&tagtype=' + tagtype + '&id=' + id + '&title=' + title + '&metadataFormTagId=' + metadataFormTagId,'Preview','width=' + 760 + ',height=' + 580 +',status=no,resizable=yes,scrollbars=no,location=no,toolbar=no');
	}
}
 
function ek_ma_ClearSelection(metadataFormTagId, msgText)
{
	var childObj, tempEl, tblBodyObj, rowObj, cellObj, textObj;
	var containerObj = document.getElementById("EnhancedMetadataMultiContainer" + metadataFormTagId.toString());
	if (containerObj)
	{
		while (childObj = containerObj.lastChild)
		{
			tempEl = containerObj.removeChild(childObj);
		}
		
		if (msgText && msgText.length)
		{
			tblBodyObj = document.createElement("tbody");
			tblBodyObj = containerObj.appendChild(tblBodyObj);
			rowObj = document.createElement("tr");
			rowObj = tblBodyObj.appendChild(rowObj);
			cellObj = document.createElement("td");
			cellObj = rowObj.appendChild(cellObj);
			textObj = document.createTextNode(msgText);
			textObj = cellObj.appendChild(textObj);
		}
	}

	var obj;
	if (metadataFormTagId > 0)
	{
		obj = document.getElementById("frm_text_" + metadataFormTagId);
		if (ek_ma_validateObject(obj)) 
		{
			obj.value = "";
		}
		obj = document.getElementById("Hdn_EnhancedMetadataTitle" + metadataFormTagId);
		if (ek_ma_validateObject(obj)) 
		{
			obj.value = "";
		}
	}
}
 
// Close Meta window/layer:
function ek_ma_CloseMetaChildPage() 
{
	var pageObj = document.getElementById('MetaSelectorPageContainer');
 
	// Configure the Meta window to be invisible:
	pageObj.style.display = 'none';
	pageObj.style.width = '1px';
	pageObj.style.height = '1px';
 
	// Ensure that the transparent layer does not cover any of the parent window:
	pageObj = document.getElementById('MetaSelectorAreaOverlay');
	pageObj.style.display = 'none';
	pageObj.style.width = '1px';
	pageObj.style.height = '1px';
}
 
function ek_ma_GetEnhancedMetadataTagTypeId(metadataFormTagId) 
{
		var obj;
		obj = document.getElementById('Hdn_EnhancedMetadataTagTypeId' + metadataFormTagId);
		if (ek_ma_validateObject(obj)) 
		{
			return (obj.value);
		}
		else
		{
			return (-1);
		}
}
	
function ek_ma_AdjustContainerSize(height) 
{
	if (ek_ma_IsBrowserIE()) 
	{
		frameObj = document.getElementById('MetaSelectorPage');
		if (ek_ma_validateObject(frameObj)) 
		{
			frameObj.style.height = height;
		}
	} 
}
 
function ek_ma_validateObject(obj)
{
	return ((obj != null) &&
		((typeof(obj)).toLowerCase() != 'undefined') &&
		((typeof(obj)).toLowerCase() != 'null'))
}
 
// NOTE:
// Function ReturnChildValue is used by listsumarry (Do Not Change Name!):
function ReturnChildValue(folderid, folderpath, metadataFormTagId)
{
		ek_ma_ReturnMediaUploaderValue(folderid, folderpath, metadataFormTagId);
		ek_ma_CloseChildPage();
}
 
//function UpdateMetaSelection(selectedId, title, metadataFormTagId) 
//{
//	ek_ma_ReturnMediaUploaderValue(selectedId, title, metadataFormTagId);
//}
	
function ek_ma_ReturnMediaUploaderValue(selectedIdName, title, metadataFormTagId)
{
	var obj, testObj;
	var delimeterChar = "";
	var namIdArray, titleArray;
	var idx;
	
	// clear original values:
	ek_ma_ClearSelection(metadataFormTagId, "");
	
	// update as appropriate (keeping some items in the packed/delimiterized strings):
	//
	// get delimiter char from designated tag-group:
	obj = document.getElementById("MetaSeparator_" + metadataFormTagId);
	if (ek_ma_validateObject(obj)) 
	{
		delimeterChar = obj.value;
	}
	//
	// split the idNames and the titles using delimiters:
	if (delimeterChar && delimeterChar.length)
	{
		namIdArray = selectedIdName.split(delimeterChar);
		titleArray = title.split(delimeterChar);
		for (idx=0; idx < namIdArray.length; idx++)
		{
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
			ek_ma_addMetaRow(itemId, itemTitle, metadataFormTagId)
		
			// update the returned metadata (hidden field):
			obj = document.getElementById("frm_text_" + metadataFormTagId);
			if (ek_ma_validateObject(obj)) 
			{
				if (obj.value.length)
				{
					obj.value += delimeterChar;
				}
				// Content items need to have the ID extracted from the link:
				if (ek_ma_isContentType(metadataFormTagId))
				{
					obj.value += ek_ma_parseId(itemId);
				}
				else
				{
					obj.value += itemId;
				}
			}

			// update the hidden title value (used for pre-populating selector windows):
			obj = document.getElementById("Hdn_EnhancedMetadataTitle" + metadataFormTagId);
			if (ek_ma_validateObject(obj)) 
			{
				if (obj.value.length)
				{
					obj.value += delimeterChar;
				}
				obj.value += itemTitle;
			}
		}
	}
	else
	{
		// update the returned metadata (hidden field):
		obj = document.getElementById("frm_text_" + metadataFormTagId);
		if (ek_ma_validateObject(obj)) 
		{
			obj.value = selectedIdName;
		}

		// update the hidden title value (used for pre-populating selector windows):
		obj = document.getElementById("Hdn_EnhancedMetadataTitle" + metadataFormTagId);
		if (ek_ma_validateObject(obj)) 
		{
			obj.value = title;
		}
	}

	// finished - no tag selected:
	ek_ma_selectedFormTagtId = 0;
}			

function ek_ma_addMetaRow(id, title, metadataFormTagId)
{
	var tblBodyObj, rowObj, cellObj, textObj;
	var thumbnail, idx, textStr, obj;
	var cellBgColor = "";
	var containerObj = document.getElementById("EnhancedMetadataMultiContainer" 
			+ metadataFormTagId.toString());
	if (containerObj)
	{
		if (id && id.length)
		{
			// if no table-body, must add one:
			tblBodyObj = containerObj.firstChild;
			if (null == tblBodyObj)
			{
				tblBodyObj = document.createElement("tbody");
				tblBodyObj = containerObj.appendChild(tblBodyObj);
			}
			
			// determine background color based on odd/even current row count:
			//if (tblBodyObj.children && (tblBodyObj.children.length & 1))
			if (tblBodyObj.childNodes && (tblBodyObj.childNodes.length & 1))
			{
				cellBgColor = "#eeeeee";
			}
			
			// add cell with title and id (with appropriate background color):
			rowObj = document.createElement("tr");
			rowObj = tblBodyObj.appendChild(rowObj);
			cellObj = document.createElement("td");
			cellObj = rowObj.appendChild(cellObj);
			if (cellBgColor.length) // && cellObj.bgColor)
			{
				cellObj.bgColor = cellBgColor;
			}
			
			// Content items need to have the ID extracted from the link:
			if (ek_ma_isContentType(metadataFormTagId))
			{
				id = ek_ma_parseId(id);
			}
			
			textStr = title + " (ID: " + id + ")";
			textObj = document.createTextNode(textStr);
			textObj = cellObj.appendChild(textObj);
			

			// show images as appropriate:
			if (ek_ma_isImageType(metadataFormTagId))
			{
				// build an element for the image thumbnail:
				obj = document.createElement("br");
				cellObj.appendChild(obj);
				obj = document.createElement("img");
				obj = cellObj.appendChild(obj);
				// now convert the name & type as necessary:
				idx = id.lastIndexOf("/");
				if (0 <= idx)
				{
					thumbnail = id.substring(0, idx);
				}
				else
				{
					idx = 0;
				}
				if (id.length > idx) {
					thumbnail += "/" + id.substring(idx + 1, id.length);
					// convert gifs as necessary:
					idx = thumbnail.lastIndexOf(".gif");
					if ((thumbnail.length - 4) == idx)
					{
						thumbnail = thumbnail.substring(0, idx) + ".png";
					}
					obj.src = thumbnail + "?sz=thumb";
				}
			}
		}
	}
}
////		 
function ek_ma_parseId(id)
{
	var tempStr = id.toLowerCase();
	var testVal;
	var result = ""
	var idx;
	// using auto alias
	testVal = "_ektid";
	
	idx = tempStr.lastIndexOf(testVal);
	if (0 <= idx)
	{
	    tempStr = tempStr.split(testVal);
	    if (tempStr.length > 0)
	    {
	        tempStr = tempStr[1].split('.');
	        result = tempStr[0];
	    }
		if (result.length)
		{
			if (isNaN(result))
			{
				result = "";
			}
		}
		return (result);
	}
	
	// using linkit.aspx & ItemID:
	testVal = "&itemid=";
	idx = tempStr.lastIndexOf(testVal);
	if (0 <= idx)
	{
		result = tempStr.substring(idx + testVal.length);
		if (result.length)
		{
			result = (parseInt(result, 10)).toString();
			if (isNaN(result))
			{
				result = "";
			}
		}
		return (result);
	}
	
	// using id:
	testVal = "&id=";
	idx = tempStr.lastIndexOf(testVal);
	if (0 <= idx)
	{
		result = tempStr.substring(idx + testVal.length);
		if (result.length)
		{
			result = (parseInt(result, 10)).toString();
			if (isNaN(result))
			{
				result = "";
			}
		}
		return (result);
	}

	// using id:
	testVal = "?id=";
	idx = tempStr.lastIndexOf(testVal);
	if (0 <= idx)
	{
		result = tempStr.substring(idx + testVal.length);
		if (result.length)
		{
			result = (parseInt(result, 10)).toString();
			if (isNaN(result))
			{
				result = "";
			}
		}
		return (result);
	}
	
	// don't know how to handle, return original:
	return (id);
}

function ek_ma_isCollectionType(formTagId)
{
	var obj = document.getElementById("Hdn_EnhancedMetadata_FamilyName" + formTagId);
	return (ek_ma_validateObject(obj) && ("Collection" == obj.value));
}

function ek_ma_isListSummaryType(formTagId)
{
	var obj = document.getElementById("Hdn_EnhancedMetadata_FamilyName" + formTagId);
	return (ek_ma_validateObject(obj) && ("ListSummary" == obj.value));
}

function ek_ma_isHyperLinkType(formTagId)
{
	var obj = document.getElementById("Hdn_EnhancedMetadata_FamilyName" + formTagId);
	return (ek_ma_validateObject(obj) && ("HyperLink" == obj.value));
}

function ek_ma_isFileType(formTagId)
{
	var obj = document.getElementById("Hdn_EnhancedMetadata_FamilyName" + formTagId);
	return (ek_ma_validateObject(obj) && ("File" == obj.value));
}

function ek_ma_isContentType(formTagId)
{
	var obj = document.getElementById("Hdn_EnhancedMetadata_FamilyName" + formTagId);
	return (ek_ma_validateObject(obj) && ("Content" == obj.value));
}

function ek_ma_isImageType(formTagId)
{
	var obj = document.getElementById("Hdn_EnhancedMetadata_FamilyName" + formTagId);
	return (ek_ma_validateObject(obj) && ("Image" == obj.value));
}

function ek_ma_isMenuType(formTagId)
{
	var obj = document.getElementById("Hdn_EnhancedMetadata_FamilyName" + formTagId);
	return (ek_ma_validateObject(obj) && ("Menu" == obj.value));
}

function ek_ma_isUserType(formTagId)
{
	var obj = document.getElementById("Hdn_EnhancedMetadata_FamilyName" + formTagId);
	return (ek_ma_validateObject(obj) && ("User" == obj.value));
}

// Note: Function CloseChildPage might be
// called externally, do not change name.
function CloseChildPage()
{
	ek_ma_CloseMetaChildPage();
}

function ek_ma_CloseChildPage()
{
	ek_ma_CloseMetaChildPage();
}
 
function ek_ma_IsBrowserIE() {
  //return (document.all ? true : false);
	var ua = window.navigator.userAgent.toLowerCase();
  return((ua.indexOf('msie') > -1) && (!(ua.indexOf('opera') > -1)));
}
 
function ek_ma_setElementText(obj, text) 
{
   if(ek_ma_IsBrowserIE())
   {
	 obj.innerText=text;
   }
   else 
   {
	 if (text == '')
		{
	   obj.innerHTML='&nbsp;';
	 }
	 else 
		{
	   obj.innerHTML = text;
	 }
   }   
}

function ek_ma_getDelimiter(metadataFormTagId)
{
	var obj = document.getElementById("MetaSeparator_" + metadataFormTagId);
	if (ek_ma_validateObject(obj)) 
	{
		return (obj.value);
	}
	return (null);
}

function ek_ma_getTitle(metadataFormTagId)
{
	var hiddenObj = document.getElementById('Hdn_EnhancedMetadataTitle' + metadataFormTagId);
	if (ek_ma_validateObject(hiddenObj)) 
	{
		return (hiddenObj.value);
	}
	return ("");
}

function ek_ma_getId(metadataFormTagId)
{
	hiddenObj = document.getElementById('frm_text_' + metadataFormTagId);
	if (ek_ma_validateObject(hiddenObj)) 
	{
		return (hiddenObj.value);
	}
	return ("");
}
