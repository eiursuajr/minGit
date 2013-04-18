var INPUT_NAME_PREFIX = 'rec_'; // this is being set via script
var OPT_INPUT_NAME_PREFIX = 'opt_'; // this is being set via script
var RADIO_NAME = 'radInput'; // this is being set via script
var TABLE_NAME = 'tblRecommendations'; // this should be named in the HTML
var ROW_BASE = 1; // first number (for display)
var hasLoaded = false;

window.onload=fillInRows;

function fillInRows()
{
	hasLoaded = true;
}

// CONFIG:
// myRowObject is an object for storing information about the table rows
function myRowObject(one, two, three, four, five, six, seven)
{
	this.one = one; // text object
	this.two = two; // text object
	this.three = three; // text object
	this.four = four; // hidden text object
	this.five = five; // hidden text object
	this.six = six; // hidden text object
	this.seven = seven; // input radio object
}

/*
 * insertRowToTable
 * Insert and reorder
 */
function insertRowToTable()
{
	if (hasLoaded) {
		var tbl = document.getElementById(TABLE_NAME);
		var rowToInsertAt = tbl.tBodies[0].rows.length;
		for (var i=0; i<tbl.tBodies[0].rows.length; i++) {
            if (tbl.tBodies[0].rows[i].myRow == null) { tbl.tBodies[0].rows[i].myRow = GetRowInfo(tbl.tBodies[0].rows[i]); }
			if (tbl.tBodies[0].rows[i].myRow && tbl.tBodies[0].rows[i].myRow.seven.getAttribute('type') == 'radio' && tbl.tBodies[0].rows[i].myRow.seven.checked) {
				// rowToInsertAt = i;
				break;
			}
		}
		addRowToTable(rowToInsertAt);
		i_reorderRows(tbl, rowToInsertAt);
	}
}

/*
 * addRowToTable
 * Inserts at row 'num', or appends to the end if no arguments are passed in. Don't pass in empty strings.
 */
function addRowToTable(num, eid, etitle)
{
    var titleExist = $ektron('table#tblRecommendations').find('input:hidden');
	var numTitle = titleExist.length;
	var numI = 0;
	for(numI = 0; numI < numTitle; numI++)
	{
	    if(titleExist[numI].value == eid)
	    {
	        alert("The item you have selected, already exists in the list. Please select other item.");
	        return false;
	    }
	}
	if (hasLoaded) {
		var tbl = document.getElementById(TABLE_NAME);
		var nextRow = tbl.tBodies[0].rows.length;
		var iteration = nextRow + ROW_BASE;
		if (num == null) { 
			num = nextRow;
		} else {
			iteration = num + ROW_BASE;
		}
		
		// add the row
		var row = tbl.tBodies[0].insertRow(num);
		
		row.className = 'itemrow' + (iteration % 2);
	
		// CONFIG: This whole section can be configured
		
		// cell 0 - text
		var cell0 = row.insertCell(0);
		var textPos = document.createTextNode(iteration);
		cell0.appendChild(textPos);
		
		// cell 1 - text
		var cell1 = row.insertCell(1);
		var textId = document.createTextNode(eid);
		cell1.appendChild(textId);
		
		// cell 2 - text
		var cell2 = row.insertCell(2);
		var textEntry = document.createTextNode(iteration);
		if (etitle != null) { textEntry = document.createTextNode(etitle); }
		cell2.appendChild(textEntry);
		
		// cell 3 - hidden text
		var cell3 = row.insertCell(3);
		var hdnId = document.createElement('input');
		hdnId.setAttribute('type', 'hidden');
		hdnId.setAttribute('name', INPUT_NAME_PREFIX + iteration + '_id');
		hdnId.setAttribute('id', INPUT_NAME_PREFIX + iteration + '_id');	
		hdnId.setAttribute('value', 0);
		cell3.appendChild(hdnId);
		var hdnEntry = document.createElement('input');
		hdnEntry.setAttribute('type', 'hidden');
		hdnEntry.setAttribute('name', INPUT_NAME_PREFIX + iteration + '_entryid');
		hdnEntry.setAttribute('id', INPUT_NAME_PREFIX + iteration + '_entryid');	
		hdnEntry.setAttribute('value', eid.toString());
		cell3.appendChild(hdnEntry);
		var hdnIdx = document.createElement('input');
		hdnIdx.setAttribute('type', 'hidden');
		hdnIdx.setAttribute('name', INPUT_NAME_PREFIX + iteration + '_posidx');
		hdnIdx.setAttribute('id', INPUT_NAME_PREFIX + iteration + '_posidx');	
		hdnIdx.setAttribute('value', iteration);
		cell3.appendChild(hdnIdx);
		
		// cell 4 - input radio
		var cell4 = row.insertCell(4);
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
		cell4.appendChild(raEl);
		
		// Pass in the elements you want to reference later
		// Store the myRow object in each row
		row.myRow = new myRowObject(textPos, textId, textEntry, hdnId, hdnEntry, hdnIdx, raEl);
	}
}

// CONFIG: this entire function is affected by myRowObject settings
// If there isn't a checkbox in your row, then this function can't be used.
function deleteChecked()
{
	if (hasLoaded) {
		var checkedObjArray = new Array();
		var cCount = 0;
	
		var tbl = document.getElementById(TABLE_NAME);
		for (var i=0; i<tbl.tBodies[0].rows.length; i++) {
		    if (tbl.tBodies[0].rows[i].myRow == null) { tbl.tBodies[0].rows[i].myRow = GetRowInfo(tbl.tBodies[0].rows[i]); }
			if (tbl.tBodies[0].rows[i].myRow && tbl.tBodies[0].rows[i].myRow.seven.getAttribute('type') == 'radio' && tbl.tBodies[0].rows[i].myRow.seven.checked) {
				checkedObjArray[cCount] = tbl.tBodies[0].rows[i];
				cCount++;
			}
		}
		if (checkedObjArray.length > 0) {
			var rIndex = checkedObjArray[0].sectionRowIndex;
			deleteRows(checkedObjArray);
			i_reorderRows(tbl, rIndex);
		}
	}
}
function deleteCurrentRow(obj)
{
	if (hasLoaded) {
		var delRow = obj.parentNode.parentNode;
		var tbl = delRow.parentNode.parentNode;
		var rIndex = delRow.sectionRowIndex;
		var rowArray = new Array(delRow);
		deleteRows(rowArray);
		i_reorderRows(tbl, rIndex);
	}
}
function reorderRows(drctn)
{
    var tbl = document.getElementById(TABLE_NAME);
    var idx = -1;
    for (var i=0; i<tbl.tBodies[0].rows.length; i++) {
        if (tbl.tBodies[0].rows[i].myRow == null) { tbl.tBodies[0].rows[i].myRow = GetRowInfo(tbl.tBodies[0].rows[i]); }
		if (tbl.tBodies[0].rows[i].myRow && tbl.tBodies[0].rows[i].myRow.seven.getAttribute('type') == 'radio' && tbl.tBodies[0].rows[i].myRow.seven.checked) {
			idx = i;
		}
	}
    if (idx > -1 && hasLoaded) {
        if (drctn == 'up')
        {
            if (idx > 0)
            {
                if (tbl.tBodies[0].rows[idx].myRow == null) { tbl.tBodies[0].rows[idx].myRow = GetRowInfo(tbl.tBodies[0].rows[idx]); }
                if (tbl.tBodies[0].rows[idx - 1].myRow == null) { tbl.tBodies[0].rows[idx - 1].myRow = GetRowInfo(tbl.tBodies[0].rows[idx - 1]); }
                var eTitleTemp = tbl.tBodies[0].rows[idx].myRow.three.data;
                var eIdTemp = tbl.tBodies[0].rows[idx].myRow.four.value;
                tbl.tBodies[0].rows[idx].myRow.three.data = tbl.tBodies[0].rows[idx - 1].myRow.three.data;
                tbl.tBodies[0].rows[idx - 1].myRow.three.data = eTitleTemp;
                tbl.tBodies[0].rows[idx].myRow.four.value = tbl.tBodies[0].rows[idx - 1].myRow.four.value;
                tbl.tBodies[0].rows[idx - 1].myRow.four.value = eIdTemp;
                WriteRowInfo(tbl.tBodies[0].rows[idx]);
                WriteRowInfo(tbl.tBodies[0].rows[idx - 1]);
                tbl.tBodies[0].rows[idx - 1].myRow.five.checked = true;                
            }
        }
        else if (drctn == 'down')
        {
            if ((idx + 1) < tbl.tBodies[0].rows.length)
            {
                if (tbl.tBodies[0].rows[idx].myRow == null) { tbl.tBodies[0].rows[idx].myRow = GetRowInfo(tbl.tBodies[0].rows[idx]); }
                if (tbl.tBodies[0].rows[idx + 1].myRow == null) { tbl.tBodies[0].rows[idx + 1].myRow = GetRowInfo(tbl.tBodies[0].rows[idx + 1]); }
                var eTitleTemp = tbl.tBodies[0].rows[idx].myRow.three.data;
                var eIdTemp = tbl.tBodies[0].rows[idx].myRow.four.value;
                tbl.tBodies[0].rows[idx].myRow.three.data = tbl.tBodies[0].rows[idx + 1].myRow.three.data;
                tbl.tBodies[0].rows[idx + 1].myRow.three.data = eTitleTemp;
                tbl.tBodies[0].rows[idx].myRow.four.value = tbl.tBodies[0].rows[idx + 1].myRow.four.value;
                tbl.tBodies[0].rows[idx + 1].myRow.four.value = eIdTemp;
                WriteRowInfo(tbl.tBodies[0].rows[idx]);
                WriteRowInfo(tbl.tBodies[0].rows[idx + 1]);
                tbl.tBodies[0].rows[idx + 1].myRow.five.checked = true;
            }
        }
        i_reorderRows(tbl, 0);
    }
}
function i_reorderRows(tbl, startingIndex)
{
	if (hasLoaded) {
		if (tbl.tBodies[0].rows[startingIndex]) {
			var count = startingIndex + ROW_BASE;
			for (var i=startingIndex; i<tbl.tBodies[0].rows.length; i++) {
			    
			    var regexCurrentCount = new RegExp(tbl.tBodies[0].rows[i].myRow.one.data, "g");
			    
			    //Start: Updating the ids and values for the hidden fields for newly added/reordered/deleted rows.
                tbl.tBodies[0].rows[i].myRow.six.id = tbl.tBodies[0].rows[i].myRow.six.id.replace(regexCurrentCount, count);
                tbl.tBodies[0].rows[i].myRow.six.value = tbl.tBodies[0].rows[i].myRow.six.value.replace(regexCurrentCount, count);
                tbl.tBodies[0].rows[i].myRow.six.name = tbl.tBodies[0].rows[i].myRow.six.name.replace(regexCurrentCount, count);
                
                tbl.tBodies[0].rows[i].myRow.five.id = tbl.tBodies[0].rows[i].myRow.five.id.replace(regexCurrentCount, count);
                tbl.tBodies[0].rows[i].myRow.five.name = tbl.tBodies[0].rows[i].myRow.five.name.replace(regexCurrentCount, count);
                
                tbl.tBodies[0].rows[i].myRow.four.id = tbl.tBodies[0].rows[i].myRow.four.id.replace(regexCurrentCount, count);
                tbl.tBodies[0].rows[i].myRow.four.name = tbl.tBodies[0].rows[i].myRow.four.name.replace(regexCurrentCount, count);
                //End: Updating the ids and values for the hidden fields for newly added/reordered/deleted rows.
                
				// CONFIG: next line is affected by myRowObject settings
				tbl.tBodies[0].rows[i].myRow.one.data = count; // text
				
				// CONFIG: next line is affected by myRowObject settings
				// tbl.tBodies[0].rows[i].myRow.two.name = INPUT_NAME_PREFIX + count; // input text
				
				// CONFIG: next line is affected by myRowObject settings
				// var tempVal = tbl.tBodies[0].rows[i].myRow.two.value.split(' '); // for debug purposes
				// tbl.tBodies[0].rows[i].myRow.two.value = count + ' was' + tempVal[0]; // for debug purposes
				
				// CONFIG: next line is affected by myRowObject settings
				tbl.tBodies[0].rows[i].myRow.seven.value = count; // input radio
				
				// CONFIG: requires class named classy0 and classy1
				tbl.tBodies[0].rows[i].className = 'itemrow' + (count % 2);
				
				count++;
			}
		}
	}
}
function deleteRows(rowObjArray) {
	if (hasLoaded) {
		for (var i=0; i<rowObjArray.length; i++) {
			var rIndex = rowObjArray[i].sectionRowIndex;
			rowObjArray[i].parentNode.deleteRow(rIndex);
		}
	}
}
function getCheckedInt() {
    var iRet = -1;
	if (hasLoaded) {
		var tbl = document.getElementById(TABLE_NAME);
		for (var i=0; i<tbl.tBodies[0].rows.length; i++) {
		    if (tbl.tBodies[0].rows[i].myRow == null) { tbl.tBodies[0].rows[i].myRow = GetRowInfo(tbl.tBodies[0].rows[i]); }
			if (tbl.tBodies[0].rows[i].myRow && tbl.tBodies[0].rows[i].myRow.seven.getAttribute('type') == 'radio' && tbl.tBodies[0].rows[i].myRow.seven.checked) {
				iRet = i;
				break;
			}
		}
	}
	return iRet;
}
function deleteChecked() {
	if (hasLoaded) {
		var checkedObjArray = new Array();
		var cCount = 0;
	
		var tbl = document.getElementById(TABLE_NAME);
		for (var i=0; i<tbl.tBodies[0].rows.length; i++) {
		    if (tbl.tBodies[0].rows[i].myRow == null) { tbl.tBodies[0].rows[i].myRow = GetRowInfo(tbl.tBodies[0].rows[i]); }
			if (tbl.tBodies[0].rows[i].myRow && tbl.tBodies[0].rows[i].myRow.seven.getAttribute('type') == 'radio' && tbl.tBodies[0].rows[i].myRow.seven.checked) {
				checkedObjArray[cCount] = tbl.tBodies[0].rows[i];
				cCount++;
			}
		}
		if (checkedObjArray.length > 0) {
			var rIndex = checkedObjArray[0].sectionRowIndex;
			deleteRows(checkedObjArray);
			i_reorderRows(tbl, rIndex);
		}
	}
}
function GetRowInfo(rowObj) 
{

	var textPos;
	var textId;
    var textEntry;
	var hdnId;
	var hdnEntry;
	var hdnIdx;
	var raEl;

    if ( rowObj.children != null ) {
    
        textPos = document.createTextNode(rowObj.children[0].innerText);
	    textId = document.createTextNode(rowObj.children[1].innerText);
        textEntry = document.createTextNode(rowObj.children[2].innerText);
	    hdnId = rowObj.children[3].children[0];
	    hdnEntry = rowObj.children[3].children[1];
	    hdnIdx = rowObj.children[3].children[2];
	    raEl = rowObj.children[4].children[0];
    
    } else {
    
        textPos = document.createTextNode(rowObj.childNodes[1].innerHTML);
	    textId = document.createTextNode(rowObj.childNodes[3].innerHTML);
        textEntry = document.createTextNode(rowObj.childNodes[5].innerHTML);
	    hdnId = rowObj.childNodes[7].childNodes[0];
	    hdnEntry = rowObj.childNodes[7].childNodes[1];
	    hdnIdx = rowObj.childNodes[7].childNodes[2];
	    raEl = rowObj.childNodes[9].childNodes[0];
    
    }
    
	return new myRowObject(textPos, textId, textEntry, hdnId, hdnEntry, hdnIdx, raEl);
	
}
function WriteRowInfo(rowObj) {	
	rowObj.children[0].innerText = rowObj.myRow.one.data;
	rowObj.children[1].innerText = rowObj.myRow.two.data;
    rowObj.children[2].innerText = rowObj.myRow.three.data;
	rowObj.children[3].innerHTML = "<input type='hidden' name='" + INPUT_NAME_PREFIX + rowObj.myRow.one.data + "_id' id='" + INPUT_NAME_PREFIX + rowObj.myRow.one.data + "_id' value='" + rowObj.myRow.one.data + "' />";
	rowObj.children[3].innerHTML += "<input type='hidden' name='" + INPUT_NAME_PREFIX + rowObj.myRow.one.data + "_entryid' id='" + INPUT_NAME_PREFIX + rowObj.myRow.one.data + "_entryid' value='" + rowObj.myRow.two.data + "' />";
	rowObj.children[3].innerHTML += "<input type='hidden' name='" + INPUT_NAME_PREFIX + rowObj.myRow.one.data + "_posidx' id='" + INPUT_NAME_PREFIX + rowObj.myRow.one.data + "_posidx' value='" + rowObj.myRow.one.data + "' />";
	rowObj.children[4].innerHTML = '<input type="radio" name="' + RADIO_NAME + '" value="' + rowObj.myRow.one.data + '">';
}
