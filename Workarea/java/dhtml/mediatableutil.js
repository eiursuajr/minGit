// JScript File

 var hasLoaded = false;
    window.onload=fillInRows;
    var ROW_BASE = 1; // first number (for display)
    var MEDIA_INPUT_NAME_PREFIX = 'txtT';
    var RADIO_NAME = 'radInput'; // this is being set via script
    
    function fillInRows()
    {
	    hasLoaded = true;
    }
function myMediaObject(one, two, three, four, five, six, seven, eight, nine)
{
	this.one = one; // text object
	this.two = two; // text object
	this.three = three; // text object
	this.four = four; // text object
	this.five = five; // hidden text object
	this.six = six; // hidden text object
	this.seven = seven; // hidden text object
	this.eight = eight; // hidden text object
	this.nine = nine; // input radio object
}
    function editRowInMediaTable(idx, tname, awidth, aheight)
    {
        idx = idx - 1;
        if (hasLoaded) 
        {
            var tbl = document.getElementById('tblMedia');
            if (tbl.tBodies[0].rows[idx].myRow == null) { tbl.tBodies[0].rows[idx].myRow = GetMediaRowInfo(tbl.tBodies[0].rows[idx]); }
	        tbl.tBodies[0].rows[idx].myRow.two.data = tname;
	        tbl.tBodies[0].rows[idx].myRow.three.data = awidth;
	        tbl.tBodies[0].rows[idx].myRow.four.data = aheight;
	        tbl.tBodies[0].rows[idx].myRow.six.value = tname;
	        tbl.tBodies[0].rows[idx].myRow.seven.value = awidth;
	        tbl.tBodies[0].rows[idx].myRow.eight.value = aheight;
    	    
	        UpdateMediaRowInfo(tbl.tBodies[0].rows[idx]);
    }
    }
    
    function addRowToMediaTable(num, aid, tname, awidth, aheight)
    {
	if (hasLoaded) {
		var tbl = document.getElementById('tblMedia');
		var nextRow = tbl.tBodies[0].rows.length;
		var iteration = nextRow + ROW_BASE;
		if (num == null) { 
			num = nextRow;
		} else {
			iteration = num + ROW_BASE;
		}
		var row = tbl.tBodies[0].insertRow(num); // add the row
		
		row.className = 'itemrow' + (iteration % 2);
		
		// cell 0 - text
		var cell0 = row.insertCell(0);
		var textAPos = document.createTextNode(iteration);
		cell0.appendChild(textAPos);
		
		// cell 1 - text
		var cell1 = row.insertCell(1);
		var textTName = document.createTextNode(tname);
		cell1.appendChild(textTName);
		
		// cell 2 - text
		var cell2 = row.insertCell(2);
		var textTType = document.createTextNode(awidth);
		cell2.appendChild(textTType);
		// cell 3 - text
		var cell3 = row.insertCell(3);
		var textTDef = document.createTextNode(aheight);
		cell3.appendChild(textTDef);		
		
		// cell 4 - 4 hidden texts
		var cell4 = row.insertCell(4);
		var hdnTIdField = document.createElement('input');
		hdnTIdField.setAttribute('type', 'hidden');
		hdnTIdField.setAttribute('name', MEDIA_INPUT_NAME_PREFIX + 'Id' + iteration);
		hdnTIdField.setAttribute('id', MEDIA_INPUT_NAME_PREFIX + 'Id' + iteration);	
		hdnTIdField.setAttribute('value', aid.toString());
		cell4.appendChild(hdnTIdField);
		var hdnTNameField = document.createElement('input');
		hdnTNameField.setAttribute('type', 'hidden');
		hdnTNameField.setAttribute('name', MEDIA_INPUT_NAME_PREFIX + 'Name' + iteration);
		hdnTNameField.setAttribute('id', MEDIA_INPUT_NAME_PREFIX + 'Name' + iteration);	
		hdnTNameField.setAttribute('value', tname);
		cell4.appendChild(hdnTNameField);
		var hdnTTypeField = document.createElement('input');
		hdnTTypeField.setAttribute('type', 'hidden');
		hdnTTypeField.setAttribute('name', MEDIA_INPUT_NAME_PREFIX + 'Type' + iteration);
		hdnTTypeField.setAttribute('id', MEDIA_INPUT_NAME_PREFIX + 'Type' + iteration);	
		hdnTTypeField.setAttribute('value', awidth);
		cell4.appendChild(hdnTTypeField);
		var hdnTDefField = document.createElement('input');
		hdnTDefField.setAttribute('type', 'hidden');
		hdnTDefField.setAttribute('name', MEDIA_INPUT_NAME_PREFIX + 'Def' + iteration);
		hdnTDefField.setAttribute('id', MEDIA_INPUT_NAME_PREFIX + 'Def' + iteration);	
		hdnTDefField.setAttribute('value', aheight);
		cell4.appendChild(hdnTDefField);
		
		// cell 5 - input radio
		var cell5 = row.insertCell(5);
		var raEl;
		try {
			raEl = document.createElement('<input type="radio" name="' + RADIO_NAME + '" value="' + iteration + '">');
			var failIfNotIE = raEl.name.length;
		} catch(ex) {
			raEl = document.createElement('input');
			raEl.setAttribute('type', 'radio');
			raEl.setAttribute('name', RADIO_NAME);
			raEl.setAttribute('value', iteration);
		}
		cell5.appendChild(raEl);

	    row.myRow = new myMediaObject(textAPos, textTName, textTType, textTDef, hdnTIdField, hdnTNameField, hdnTTypeField, hdnTDefField, raEl);
	    }
    }
    
    function getMediaCheckedInt(vithValue)
{
    var iRet = -1;
	if (hasLoaded) {
		var tbl = document.getElementById('tblMedia');
		for (var i=0; i<tbl.tBodies[0].rows.length; i++) {
		    if (tbl.tBodies[0].rows[i].myRow == null) { tbl.tBodies[0].rows[i].myRow = GetMediaRowInfo(tbl.tBodies[0].rows[i]); }
			if (tbl.tBodies[0].rows[i].myRow && tbl.tBodies[0].rows[i].myRow.nine.getAttribute('type') == 'radio' && tbl.tBodies[0].rows[i].myRow.nine.checked) {
				if (vithValue) { iRet = tbl.tBodies[0].rows[i].myRow.five.value; } else { iRet = i; } 
				break;
			}
		}
	}
	return iRet;
}

function deleteCheckedMedia()
{
	if (hasLoaded) {
		var checkedObjArray = new Array();
		var cCount = 0;
	
		var tbl = document.getElementById('tblMedia');
		for (var i=0; i<tbl.tBodies[0].rows.length; i++) {
		    if (tbl.tBodies[0].rows[i].myRow == null) { tbl.tBodies[0].rows[i].myRow = GetMediaRowInfo(tbl.tBodies[0].rows[i]); }
			if (tbl.tBodies[0].rows[i].myRow && tbl.tBodies[0].rows[i].myRow.nine.getAttribute('type') == 'radio' && tbl.tBodies[0].rows[i].myRow.nine.checked) {
				checkedObjArray[cCount] = tbl.tBodies[0].rows[i];
				cCount++;
			}
		}
		if (checkedObjArray.length > 0) {
			var rIndex = checkedObjArray[0].sectionRowIndex;
			deleteMediaRows(checkedObjArray);
			i_reorderMediaRows(tbl, rIndex);
		}
	}
}
function i_reorderMediaRows(tbl, startingIndex)
{
	if (hasLoaded) {
		if (tbl.tBodies[0].rows[startingIndex]) {
			var count = startingIndex + ROW_BASE;
			for (var i=startingIndex; i<tbl.tBodies[0].rows.length; i++) {
			
				// CONFIG: next line is affected by myRowObject settings
				tbl.tBodies[0].rows[i].myRow.one.data = count; // text
				
				// CONFIG: next line is affected by myRowObject settings
				// tbl.tBodies[0].rows[i].myRow.two.name = MEDIA_INPUT_NAME_PREFIX + count; // input text
				
				// CONFIG: next line is affected by myRowObject settings
				// var tempVal = tbl.tBodies[0].rows[i].myRow.two.value.split(' '); // for debug purposes
				// tbl.tBodies[0].rows[i].myRow.two.value = count + ' was' + tempVal[0]; // for debug purposes
				
				// CONFIG: next line is affected by myRowObject settings
				tbl.tBodies[0].rows[i].myRow.nine.value = count; // input radio
				
				// CONFIG: requires class named classy0 and classy1
				tbl.tBodies[0].rows[i].className = 'itemrow' + (count % 2);
				
				count++;
			}
		}
	}
}
 function deleteMediaRows(rowObjArray) {
	if (hasLoaded) {
		for (var i=0; i<rowObjArray.length; i++) {
			var rIndex = rowObjArray[i].sectionRowIndex;
			rowObjArray[i].parentNode.deleteRow(rIndex);
		}
	}
}
function GetMediaRowInfo(rowObj) 
{
    
    var textTPos;
    var textTName;
    var textTType;
    var textTDef;
    var hdnTIdField;
    var hdnTNameField;
    var hdnTTypeField;
    var hdnTDefField;
    var raEl;

    if (rowObj.children != null) 
    {
    
        textTPos = document.createTextNode(rowObj.children[0].innerText);
        textTName = document.createTextNode(rowObj.children[1].innerText);
        textTType = document.createTextNode(rowObj.children[2].innerText);
        textTDef = document.createTextNode(rowObj.children[3].innerText);
        hdnTIdField = rowObj.children[4].children[0];
        hdnTNameField = rowObj.children[4].children[1];
        hdnTTypeField = rowObj.children[4].children[2];
        hdnTDefField = rowObj.children[4].children[3];
        raEl = rowObj.children[5].children[0];
        
    } else {
    
        textTPos = document.createTextNode(rowObj.childNodes[1].innerHTML);
        textTName = document.createTextNode(rowObj.childNodes[3].innerHTML);
        textTType = document.createTextNode(rowObj.childNodes[5].innerHTML);
        textTDef = document.createTextNode(rowObj.childNodes[7].innerHTML);
        hdnTIdField = rowObj.childNodes[9].childNodes[0];
        hdnTNameField = rowObj.childNodes[9].childNodes[1];
        hdnTTypeField = rowObj.childNodes[9].childNodes[2];
        hdnTDefField = rowObj.childNodes[9].childNodes[3];
        raEl = rowObj.childNodes[11].childNodes[0];
    
    }
    
	return new myMediaObject(textTPos, textTName, textTType, textTDef, hdnTIdField, hdnTNameField, hdnTTypeField, hdnTDefField, raEl);
	
}
function UpdateMediaRowInfo(rowObj) 
{
    
    if (rowObj.children != null) 
    {
	    rowObj.children[1].innerText = rowObj.myRow.two.data;
        rowObj.children[2].innerText = rowObj.myRow.three.data;
        rowObj.children[3].innerText = rowObj.myRow.four.data;
       
	    rowObj.children[4].children[1].value = rowObj.myRow.two.data;
	    rowObj.children[4].children[2].value = rowObj.myRow.seven.value;
	    rowObj.children[4].children[3].value = rowObj.myRow.four.data;
	    
	} else {
	
	    rowObj.childNodes[1].innerHTML = rowObj.myRow.two.data;
        rowObj.childNodes[2].innerHTML = rowObj.myRow.three.data;
        rowObj.childNodes[3].innerHTML = rowObj.myRow.four.data;
       
	    rowObj.childNodes[4].childNodes[1].value = rowObj.myRow.two.data;
	    rowObj.childNodes[4].childNodes[2].value = rowObj.myRow.seven.value;
	    rowObj.childNodes[4].childNodes[3].value = rowObj.myRow.four.data;
	
	}
	
}